using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using DriverCodeBaseEx;
using DevExpress.Xpo;
using DriverCodeBaseEx.Enumerators;
using Opc.Ua;
using DriverBaseInterfaces;
using TwinCAT.Ads;

namespace TwinCAT
{
    public class TwinCATStation : Station
    {
        #region Constructors

        /// <summary>
        /// Initializes the station object.
        /// </summary>
        public TwinCATStation(CommunicationDriver commdriver, TwinCATStationSettings settings)
            : base(commdriver, settings)
        {
        }
       
        #endregion
        #region Abstract Methods

        public override CommJob CreateJob(CommJobSettings JobSettings)
        {
            var conf = JobSettings as TwinCATCommJobSettings;
            if (conf == null)
                throw new ArgumentException("Invalid communication job settings");

            return new TwinCATCommJob(this, conf);
        }

        public override CommJob CreateJob(Tag defTag)
        {
            var conf = defTag as TwinCATTag;
            if (conf == null)
                throw new ArgumentException("Invalid tag object");

            return new TwinCATCommJob(this, conf);
        }

        public override Tag CreateTag(DriverBaseInterfaces.TagDefinition td)
        {
            return new TwinCATTag(td);
        }

        public override CommJobSettings CreateJobSettings(Session session, CommJob job)
        {
            var commJob = job as TwinCATCommJob;
            if (commJob == null)
                throw new ArgumentException("Invalid job object");

            return new TwinCATCommJobSettings(session, commJob);
        }

        public override void ProcessJobValues(ExecutedJobArgs e)
        {
            TwinCATCommJob tcJ = e.Job as TwinCATCommJob;
            if (tcJ == null)
            {
                return;
            }

            if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
            {
                if (e.Values != null)
                {
                    byte[] Answer = (byte[])e.Values;
                    List<object> ChangedTags = new List<object>();
                    if (ParseReceivedData(Answer, ref tcJ, ref ChangedTags))
                    {
                        foreach (var tag in ChangedTags)
                        {
                            var j = tag as Tag;
                            if (j != null)
                            {
                                e.ChangedTags.Add(j);
                            }
                        }
                    }
                    else
                    {
                        e.ErrorCode = DriverErrorCodes.ErrorParsingAnswer;
                    }
                }
            }

            e.GeneralError = (e.ErrorCode == DriverErrorCodes.ErrorTimeOut);
            
            base.ProcessJobValues(e);
        }

        public override void ParseDynamicTagsStructSplit(Tag tag, List<Tag> innerList, List<Tag> lMethods)
        {            
            if (tag.TagNode.DataType.IdType == IdType.Guid && CommDriver.IsPrototypeSplitEnabled())
            {
                //struttura
                List<TagDefinition> pList = new List<TagDefinition>();
                GetPrototypeTagList(tag.TagNode.NodeId, ref pList);
                string dynsettings = string.Empty;
                DynTagSettings dtCalcBase = tag.DynSettings;
                TwinCATDynTagSettings dtCalc = dtCalcBase as TwinCATDynTagSettings;
                dtCalc.TwinCATVersion = ((TwinCATChannel)Channel).TwinCATVersion;
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
                            CommunicationDriver.log.Error(string.Format(Properties.Resources.InvalidTagDefinition, CommDriver.DriverName, t.TagNode.NodeId.Identifier, t.InvalidReason));
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

        #endregion

        #region Specific Methods

        private uint UpdateTagValue(NodeId tagnodeid, DataValue value)
        {
            CommJob job;
            lock (lockListObject)
            {
                if (!mapTagJob.ContainsKey(tagnodeid))
                    return StatusCodes.BadNodeIdInvalid;

                job = mapTagJob[tagnodeid];
            }
            TwinCATCommJob tcJob = job as TwinCATCommJob;
            return tcJob.UpdateTagValue(tagnodeid, value);
        }

        public override bool ParseReceivedToArguments(byte[] receivedbuffer, CommJob job, ref List<Object> arguments)
        {
            TwinCATCommJob tcj = job as TwinCATCommJob;
            ParseReceivedData(receivedbuffer, ref tcj, ref arguments);
            return true;
        }

        private bool ParseReceivedData(byte[] receivebuffer, ref TwinCATCommJob tcJ,
                                       ref List<object> items)
        {
            List<Tag> changed = new List<Tag>();
            bool areArguments = (items.Count > 0);
            if(!areArguments && (receivebuffer == null))
            {
                return (false);
            }
            else if (receivebuffer == null)
            {
                BuiltInType bt = tcJ.Station.GetBuiltInType(items[0].GetType());
                if (bt != BuiltInType.Byte && bt != BuiltInType.Double &&
                    bt != BuiltInType.Float && bt != BuiltInType.Int16 &&
                    bt != BuiltInType.Int32 && bt != BuiltInType.Int64 &&
                    bt != BuiltInType.Integer && bt != BuiltInType.Number &&
                    bt != BuiltInType.SByte && bt != BuiltInType.UInt16 &&
                    bt != BuiltInType.UInt32 && bt != BuiltInType.UInt64 &&
                    bt != BuiltInType.UInteger)
                {
                    return false;
                }

                items[0] = DriverErrorCodes.ErrorNoError;
                return (true);
            }

            if (areArguments)
            {
                items[0] = DriverErrorCodes.ErrorNoError;
            }

            uint ArraySize = tcJ.TagsList[0].TagNode.ArrayDimension;
            if (ArraySize == 0)
                ArraySize = 1;
            int TotalJobSize;
            if (!tcJ.isProtocolBool() || (tcJ.ElementNumber == 0 && (uint)tcJ.TagsList[0].TagNode.DataType.Identifier != (uint)BuiltInType.Boolean))
            {
                TotalJobSize = (int)tcJ.GetTotalJobSize(tcJ);
            }
            else
                TotalJobSize = (int)((ArraySize + 7) / 8);

            int ReceivedBytes = receivebuffer.Length;
            if (ReceivedBytes < TotalJobSize)
            {
                if (tcJ.AddressObj.DataFormat != TwinCATDataFormat.DataFormat_STRING)
                {
                    if (areArguments)
                    {
                        items[0] = DriverErrorCodes.ErrorParsingAnswer;
                    }
                    return false;
                }
            }
            else if (ReceivedBytes > TotalJobSize)
            {
                ReceivedBytes = TotalJobSize;
            }

            byte[] tempBuffer;
            if (!tcJ.isProtocolBool() || (tcJ.ElementNumber == 0 && (uint)tcJ.TagsList[0].TagNode.DataType.Identifier != (uint)BuiltInType.Boolean))
            {
                tempBuffer = new byte[ReceivedBytes];
                Array.Copy(receivebuffer, tempBuffer, ReceivedBytes);
            }
            else
            {
                ReceivedBytes = (int)tcJ.TagsList[0].TagNode.ArrayDimension;
                if (ReceivedBytes == 0)
                    ReceivedBytes = 1;
                tempBuffer = new byte[ReceivedBytes];

                //The arrays of Boolean are not possible on Bit-Adress %MX8.0
                if ((tcJ.TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.Boolean)&&
                    (tcJ.TagsList[0].TagNode.ArrayDimension > 0))
                {
                    for (ushort bitIndex = 0; bitIndex < ReceivedBytes; bitIndex++)
                    {
                        tempBuffer[bitIndex] = receivebuffer[bitIndex ];
                    }
                }
                else
                {
                    for (ushort bitIndex = 0; bitIndex < ReceivedBytes; bitIndex++)
                    {
                        tempBuffer[bitIndex] = (byte)((receivebuffer[bitIndex / 8] >> (bitIndex % 8)) & 1);
                    }
                }
      
            }

            tcJ.SetJobData(tempBuffer, ref changed);

            if (areArguments)
            {
                if (tcJ.TagsList.Count == items.Count - 1)
                {
                    for (int k = 0; k < tcJ.TagsList.Count; k++)
                    {
                        items[k + 1] = tcJ.TagsList[k].Value.Value;
                    }
                }
            }
            else
            {
                items.AddRange(changed);
            }

            return true;
        }


        public List<CommJob> GetListWholeJobCopy()
        {
            var listJob = new List<CommJob>();
            lock (lockListObject)
            {
                listJob.AddRange(ListWholeJob);
            }
            return listJob;
        }
        #endregion
    }
}
