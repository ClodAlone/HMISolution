using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using Opc.Ua;
using DriverBaseInterfaces;
using S7ImportParser;

namespace S7TIASymbolic
{
    public class S7TIAStation : Station
    {
        #region Constructors

        /// <summary>
        /// Initializes the station object.
        /// </summary>
        public S7TIAStation(CommunicationDriver commdriver, S7TIAStationSettings settings)
            : base(commdriver, settings)
        {
            _ImportSource = (settings.S7TiaPortalSecurityHMIAccessLevel ? S7TIAImportParser.ImportSourceManagement.Project : S7TIAImportParser.ImportSourceManagement.ProjectAndPlc);
            _Password = settings.S7TiaPortalSecurityHMIPassword;
            _HMIAccessLevel = settings.S7TiaPortalSecurityHMIAccessLevel;
        }

        #endregion

        #region Abstract Methods

        public override CommJob CreateJob(CommJobSettings JobSettings)
        {
            var conf = JobSettings as S7TIACommJobSettings;
            if (conf == null)
                throw new ArgumentException("Invalid communication job settings");

            return new S7TIACommJob(this, conf);
        }

        public override CommJob CreateJob(Tag defTag)
        {
            var conf = defTag as S7TIATag;
            if (conf == null)
                throw new ArgumentException("Invalid tag object");

            return new S7TIACommJob(this, conf);
        }

        public override Tag CreateTag(DriverBaseInterfaces.TagDefinition td)
        {
            return new S7TIATag(td);
        }

        public override CommJobSettings CreateJobSettings(Session session, CommJob job)
        {
            var commJob = job as S7TIACommJob;
            if (commJob == null)
                throw new ArgumentException("Invalid job object");

            return new S7TIACommJobSettings(session, commJob);
        }

        #endregion

        public override void ProcessJobValues(ExecutedJobArgs e)
        {
            S7TIACommJob mJ = e.Job as S7TIACommJob;
            if (mJ == null)
                return;

            // analyzing answer if no error exists before
            if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
            {
                byte[] Answer = (byte[])e.Values;
                List<object> ChangedTags = new List<object>();
                if (Answer != null)
                {
                    if (/*P*/S7TIAProtocol.ParseData(Answer, ref mJ, ref ChangedTags))
                        foreach (var tag in ChangedTags)
                        {
                            var j = tag as Tag;
                            if (j != null)
                                e.ChangedTags.Add(j);
                        }
                    //e.ChangedTags.AddRange(ChangedTags);
                    else
                        e.ErrorCode = DriverErrorCodes.ErrorParsingAnswer;
                }
            }

            //put all the job in error?
            e.GeneralError = (e.ErrorCode == DriverErrorCodes.ErrorTimeOut || (e.ErrorCode ==  (DriverErrorCodes)S7ErrorCodes.ErrorConnectionToDevice));

            base.ProcessJobValues(e);
        }

        public override bool ParseReceivedToArguments(byte[] receivedbuffer, CommJob job, ref List<Object> arguments)
        {
            S7TIACommJob mj = job as S7TIACommJob;

            S7TIAProtocol.ParseData(receivedbuffer, ref mj, ref arguments);
            return true;
        }

        public override List<Tag> SortTags(IList<Tag> tags)
        {
            return tags.ToList();
        }
        
        public override void ParseDynamicTagsStructSplit(Tag tag, List<Tag> innerList, List<Tag> lMethods)
        { 
            string szTmpS7TIADynTagSettings = ((S7TIATag)tag).S7TIADynTagSettings.ToString();
            if (tag.TagNode.DataType.IdType == IdType.Guid && CommDriver.IsPrototypeSplitEnabled() && ( !IsDTLFormat(szTmpS7TIADynTagSettings)))
            {
                //struttura
                List<TagDefinition> pList = new List<TagDefinition>();
                GetPrototypeTagList(tag.TagNode.NodeId, ref pList);
                string dynsettings = string.Empty;
                DynTagSettings dtCalc = tag.DynSettings;
                TagDefinition olddtag;
                for (int i = 0; i < pList.Count; i++)
                {

                    TagDefinition dtag = pList[i];
                    olddtag = dtag;
                    if (dtag.DynamicSettings.Length == 0)
                    {
                        if (dynsettings.Length == 0)
                        {
                            dynsettings = dtCalc.GetFirstDynSetting(tag, dtag);
                        }
                        else
                        {
                            //dtCalc.TryParse(dynsettings);
                            dynsettings = dtCalc.GetNextDynSetting(pList[i - 1], dtag);
                            //dynsettings = dtCalc.GetNextDynSetting(pList[i - 1]);
                        }
                        dtag.DynamicSettings = dynsettings;
                    }
                    //get dynsettings for each tagdefinition.
                    Tag t = GetCommDriver().CreateTag(dtag);
                    if (t.DynSettings.MethodID > -1)
                    {
                        //ricordati del metodo...
                        lMethods.Add(t);
                        if (dynsettings.Length > 0)
                            dtag = olddtag;
                        continue;
                    }
                    else
                    {
                        if (t.bIsValid)
                        {
                            innerList.Add(t);
                        }
                        else
                        {
                            CommDriver.OnTagChanged(t.TagNode.NodeId, new DataValue(StatusCodes.BadConfigurationError));
                            CommunicationDriver.OnLogEvent(string.Format(Properties.Resources.InvalidTagDefinition, CommDriver.DriverName, t.TagNode.NodeId.Identifier, t.InvalidReason), System.Diagnostics.EventLogEntryType.Error);
                        }
                    }
                }
            }
            else
            {
                if (tag.DynSettings.MethodID > -1)
                    lMethods.Add(tag);
                innerList.Add(tag);
            }             
        }

        bool IsDTLFormat(string szStrDynSetting)
        {
            long number1 = 0;
            if (szStrDynSetting == string.Empty)
            {
                return (false);
            }
            else
            {
                int pos = szStrDynSetting.IndexOf("S7Typ=");
                if(pos == -1)
                {
                    return (false);
                }
                else
                {
                    string szTemSubString = szStrDynSetting.Substring(pos + 6, 4);
                    
                    bool canConvert = long.TryParse(szTemSubString, out number1);
                    if (canConvert == false)
                    {
                        return (false);
                    }
                }
            }
            return (number1 == ((long)S7DataFormats.S7_DTL));
        }

        #region Properties

        private bool _Connected;
        public bool Connected
        {
            get { return _Connected; }
            set { _Connected = value; }
        }
        
        S7TIAImportParser.ImportSourceManagement _ImportSource = S7TIAImportParser.ImportSourceManagement.ProjectAndPlc;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the .Tia file import/creation mode. </summary>
        ///
        /// <value> .Tia file management mode </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public S7TIAImportParser.ImportSourceManagement ImportSource
        {
            get
            {
                return _ImportSource;
            }
            set
            {
                _ImportSource = value;
            }
        }

        private string _Password;
        public string Password
        {
            get { return _Password; }
        }

        private bool _HMIAccessLevel;
        public bool HMIAccessLevel
        {
            get { return _HMIAccessLevel; }
        }

        #endregion

    }
}
