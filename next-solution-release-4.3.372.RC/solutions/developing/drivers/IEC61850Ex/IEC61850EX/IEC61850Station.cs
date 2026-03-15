using System;
using System.Collections.Generic;
using DevExpress.Xpo;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;

namespace IEC61850
{
    public class IEC61850Station : Station
    {                
        #region Constructors

        /// <summary>
        /// Initializes the station object.
        /// </summary>
        public IEC61850Station(CommunicationDriver commdriver, IEC61850StationSettings settings)
            : base(commdriver, settings)
        {
            _TransportSelector = settings.TransportSelector;
            _SessionSelector = settings.SessionSelector;
            _PresentationSelector = settings.PresentationSelector;
            _ApplicationID = settings.ApplicationID;
            _AEQualifier = settings.AEQualifier;
            _AuthenticationEnabled = settings.AuthenticationEnabled;
            _AuthenticationPassword = settings.AuthenticationPassword;
            _IsConnected = false;
        }

        #endregion

        #region Abstract Methods

        public override CommJob CreateJob(CommJobSettings JobSettings)
        {
            var conf = JobSettings as IEC61850CommJobSettings;
            if (conf == null)
                throw new ArgumentException("Invalid communication job settings");

            return new IEC61850CommJob(this, conf);
        }

        public override CommJob CreateJob(Tag defTag)
        {
            var conf = defTag as IEC61850Tag;
            if (conf == null)
                throw new ArgumentException("Invalid tag object");

            return new IEC61850CommJob(this, conf);
        }

        public override Tag CreateTag(DriverBaseInterfaces.TagDefinition td)
        {
            return new IEC61850Tag(td);
        }
        public override CommJobSettings CreateJobSettings(Session session, CommJob job)
        {
            var commJob = job as IEC61850CommJob;
            if (commJob == null)
                throw new ArgumentException("Invalid job object");

            return new IEC61850CommJobSettings(session, commJob);
        }

        #endregion

        #region Override Methods
        public override void ProcessJobValues(ExecutedJobArgs e)
        {
            IEC61850CommJob iecJ = e.Job as IEC61850CommJob;
            if (iecJ == null)
                return;

            // analyzing answer if no error exists before
            if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
            {
                byte[] Answer = (byte[])e.Values;
                List<object> ChangedTags = new List<object>();                
                if (IEC61850Protocol.ParseData(Answer, ref iecJ, ref ChangedTags))
                {
                    //System.Diagnostics.Debug.WriteLine(string.Format("IEC61850 DEBUG - ProcessJobValues - OK - Message Length = {0}", (Answer == null ? "null" : Answer.Length.ToString())));
                    foreach (var tag in ChangedTags)
                    {
                        var j = tag as Tag;
                        if (j != null)
                            e.ChangedTags.Add(j);
                    }
                }
                else
                {
                    //System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - ProcessJobValues - Error in ParseData - Message Length = {0}", (Answer == null ? "null" : Answer.Length.ToString()));
                    if (ChangedTags.Count > 0)
                        e.ErrorCode = (DriverErrorCodes)((IEC61850ErrorCodes)ChangedTags[0]);
                    else
                        e.ErrorCode = DriverErrorCodes.ErrorParsingAnswer;
                }
            }

            //put all the job in error?
            e.GeneralError = (e.ErrorCode == DriverErrorCodes.ErrorTimeOut || e.ErrorCode == DriverErrorCodes.ErrorDeviceOpenFailed);

            base.ProcessJobValues(e);
        }

        public override bool ParseReceivedToArguments(byte[] receivedbuffer, CommJob job, ref List<Object> arguments)
        {
            IEC61850CommJob mj = job as IEC61850CommJob;            
            IEC61850Protocol.ParseData(receivedbuffer, ref mj, ref arguments);
            return true;
        }
        #endregion

        #region Elements
        public byte[] TransportSelectorArray = null;
        public byte[] SessionSelectorArray = null;
        public byte[] PresentationSelectorArray = null;
        public byte[] ApplicationIDArray = null;
        #endregion

        #region Specific Methods

        public void ManageGeneralError(DriverErrorCodes errorCode)
        {
            //all jobs in error
            List<CommJob> erlist = new List<CommJob>();
            lock (lockListObject)
            {
                erlist.AddRange(ListWholeJob);
            }
            foreach (var j in erlist)
            {
                /*When a general error occurs, the jobs are put in error, it is necessary 
                 * to distinguish the jobs that have conditional variables or jobs excepionOutput, 
                 * because at the resumption of communication (eg Timeout), 
                 * jobs of type exceptionoutput and jobs with conditional variables, 
                 * would keep in error the station until their execution.*/
                if ((j.Type != LinkType.ExceptionOutput) && !j.ConditionalVariableSet)
                {
                    j.SetErrorState((int)errorCode);
                }
            }
            SetStateCommandVariableBit(true, (UInt16)StationVariableBits.StationErrorState);
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

        public void SetTransportSelectorArray(byte[] selectorArray)
        {
            if(selectorArray == null)
            {
                return;
            }
            int selectorLength = selectorArray.Length;
            TransportSelectorArray = new byte[selectorLength];
            if(selectorLength > 0)
            {
                Array.Copy(selectorArray, 0, TransportSelectorArray, 0, selectorLength);
            }
        }

        public void SetSessionSelectorArray(byte[] selectorArray)
        {
            if (selectorArray == null)
            {
                return;
            }
            int selectorLength = selectorArray.Length;
            SessionSelectorArray = new byte[selectorLength];
            if (selectorLength > 0)
            {
                Array.Copy(selectorArray, 0, SessionSelectorArray, 0, selectorLength);
            }
        }

        public void SetPresentationSelectorArray(byte[] selectorArray)
        {
            if (selectorArray == null)
            {
                return;
            }
            int selectorLength = selectorArray.Length;
            PresentationSelectorArray = new byte[selectorLength];
            if (selectorLength > 0)
            {
                Array.Copy(selectorArray, 0, PresentationSelectorArray, 0, selectorLength);
            }
        }

        public void SetApplicationIDArray(byte[] idArray)
        {
            if (idArray == null)
            {
                return;
            }
            int idLength = idArray.Length;
            ApplicationIDArray = new byte[idLength];
            if (idLength > 0)
            {
                Array.Copy(idArray, 0, ApplicationIDArray, 0, idLength);
            }
        }
        #endregion

        #region Properties


        /// <summary>
        /// Client Transport Selector
        /// </summary>
        private string _TransportSelector;
        public string TransportSelector
        {
            get
            {
                return _TransportSelector;
            }
            set
            {
                _TransportSelector = value;
            }
        }

        /// <summary>
        /// Client Session Selector
        /// </summary>
        private string _SessionSelector;
        public string SessionSelector
        {
            get
            {
                return _SessionSelector;
            }
            set
            {
                _SessionSelector = value;
            }
        }

        /// <summary>
        /// Presentation Selector 
        /// </summary>
        private string _PresentationSelector;
        public string PresentationSelector
        {
            get
            {
                return _PresentationSelector;
            }
            set
            {
                _PresentationSelector = value;
            }
        }

        /// <summary>
        /// Application ID
        /// </summary>
        private string _ApplicationID;
        public string ApplicationID
        {
            get
            {
                return _ApplicationID;
            }
            set
            {
                _ApplicationID = value;
            }
        }

        /// <summary>
        /// AE Qualifier
        /// </summary>
        private uint _AEQualifier;
        public uint AEQualifier
        {
            get
            {
                return _AEQualifier;
            }
            set
            {
                _AEQualifier = value;
            }
        }

        /// <summary>
        /// Authentication Enabled
        /// </summary>
        private bool _AuthenticationEnabled;
        public bool AuthenticationEnabled
        {
            get
            {
                return _AuthenticationEnabled;
            }
            set
            {
                _AuthenticationEnabled = value;
            }
        }

        /// <summary>
        /// Authentication Password
        /// </summary>
        private string _AuthenticationPassword;
        public string AuthenticationPassword
        {
            get
            {
                return _AuthenticationPassword;
            }
            set
            {
                _AuthenticationPassword = value;
            }
        }

        /// <summary>
        /// Authentication Enabled
        /// </summary>
        private bool _IsConnected;
        public bool IsConnected
        {
            get
            {
                return _IsConnected;
            }
            set
            {
                _IsConnected = value;
            }
        }

        #endregion

    }
}
