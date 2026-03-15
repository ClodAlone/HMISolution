using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Xpo;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using SnmpSharpNet;

namespace SNMP
{
    public class SNMPStation : Station
    {        
        #region Constructors       
        /// <summary>
        /// Initializes the station object.
        /// </summary>
        public SNMPStation(CommunicationDriver commdriver, SNMPStationSettings settings)
            : base(commdriver, settings)
        {
            _Disconnected = false;            
            _snmpVersion = GetChannelSnmpVersion(commdriver, settings.Channel);            
            _Discovered = false;
            _Authenticated = false;
            if (_snmpVersion == SNMPVERSION.SNMPv3)
            {
                SetSecurityLevelParameters(ref _Usm, settings);
                SetSecurityLevelParameters(ref _UsmTrap, settings);
            }
        }
        #endregion

        #region Abstract Methods
                
        public override bool Startup()
        {
            // Call the base class method
            if (!base.Startup())
                return false;

            PrepareMapOIDJobs();            

            return true;
        }

        public override CommJob CreateJob(CommJobSettings JobSettings)
        {
            var conf = JobSettings as SNMPCommJobSettings;
            if (conf == null)
                throw new ArgumentException("Invalid communication job settings");

            return new SNMPCommJob(this, conf);
        }

        public override CommJob CreateJob(Tag defTag)
        {
            var conf = defTag as SNMPTag;
            if (conf == null)
                throw new ArgumentException("Invalid tag object");

            return new SNMPCommJob(this, conf);
        }

        public override Tag CreateTag(DriverBaseInterfaces.TagDefinition td)
        {
            return new SNMPTag(td);
        }

        public override CommJobSettings CreateJobSettings(Session session, CommJob job)
        {
            var commJob = job as SNMPCommJob;
            if (commJob == null)
                throw new ArgumentException("Invalid job object");

            return new SNMPCommJobSettings(session, commJob);
        }
        
        public override void ProcessJobValues(ExecutedJobArgs e)
        {
            SNMPCommJob sJ = e.Job as SNMPCommJob;
            if (sJ == null)
                return;

            // Analyze answer if no error occurred before
            if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
            {
                if (e.Values != null)
                {
                    byte[] Answer = (byte[])e.Values;
                    List<object> ChangedTags = new List<object>();
                    if (SNMPProtocol.ParseData(Answer, ref sJ, ref ChangedTags))
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

            // Put all the jobs in error?
            e.GeneralError = (e.ErrorCode == DriverErrorCodes.ErrorTimeOut);

            // use ErrorTimeOut for ChannelConnectionState
            _Disconnected = (e.ErrorCode == DriverErrorCodes.ErrorTimeOut);

            base.ProcessJobValues(e);
        }

        public override bool ParseReceivedToArguments(byte[] receivedbuffer, CommJob job, ref List<Object> arguments)
        {
            SNMPCommJob mj = job as SNMPCommJob;

            SNMPProtocol.ParseData(receivedbuffer, ref mj, ref arguments);
            return true;
        }

        #endregion

        #region Methods
        private SNMPVERSION GetChannelSnmpVersion(CommunicationDriver commdriver, string channelName)
        {
            Channel ch = commdriver.GetChannels().Find(c => c.Name == channelName);
            if (ch == null)
                return SNMPVERSION.SNMPv1;
            else
                return ((SNMPChannel)ch).snmpVersion;
        }

        public void PutAllJobsInError(DriverErrorCodes errorCode)
        {
            List<CommJob> listJobs = new List<CommJob>();
            lock (lockListObject)
                listJobs.AddRange(ListWholeJob);

            _Discovered = false;
            _Authenticated = false;
            // reset only polling _USM
            ResetAutenticationAndPrivacy(ref _Usm);
            SetStateCommandVariableBit(true, (UInt16)StationVariableBits.StationErrorState);

            if (listJobs.Count > 0)
            {                            
                foreach (SNMPCommJob job in listJobs)
                {
                        if (job.InUse && !job.InErrorState)
                        {
                            if (((job.Type != LinkType.ExceptionOutput) && !job.ConditionalVariableSet))
                                job.SetErrorState((int)errorCode, false);
                            else if (job.Type == LinkType.ExceptionOutput)
                            {
                                // Uncertain is the initial quality for jobs of type Exception Output
                                job.SetUncertainQuality();
                            }
                            ChannelBase.ChangeStateJob(job, CommJobState.PollingInError);
                        }
                    job.ResetSNMPParameters();
                }
                ResetJobsToInitialState(listJobs);
            }
        }

        /// <summary>
        /// Reset some job's internal variabile used (especially) to manage some restriction of SNMP v1
        /// </summary>
        /// <param name="listJobs"></param>
        public void ResetJobsToInitialState(List<CommJob> listJobs = null)
        {
            lock (lockListObject)
            {
                listJobs = new List<CommJob>();
                listJobs.AddRange(ListWholeJob);
            }

            Parallel.ForEach(listJobs, job =>
            {
                ((SNMPCommJob)job).FirstRequest = true;
            });
        }
        #endregion

        #region Authentication and privacy methods

        private SNMPProtocol.SNMPUSM GetUsmByPduType(SNMPProtocol.SNMPMessageGeneralData messageGeneralData)
        {
            return (messageGeneralData.IsMessageFromTrap ? _UsmTrap : _Usm);
        }

        public void SetSecurityLevelParameters(ref SNMPProtocol.SNMPUSM usm, SNMPStationSettings settings)
        {
            usm = new SNMPProtocol.SNMPUSM();
            usm.SetContextName(settings.snmpContextName);
            usm.SetUserName(settings.snmpUserName);
            usm.SecurityLevel = settings.snmpSecurityLevel;
            usm.AuthenticationProtocol = settings.snmpAuthenticationProtocol;
            usm.SetAuthenticationPassword(settings.snmpAuthenticationPassword);
            usm.PrivacyEncryptionProtocol = settings.snmpPrivacyEncryptionProtocol;
            usm.SetPrivacyEncryptionPassword(settings.snmpPrivacyEncryptionPassword);
        }

        public DriverErrorCodes CalculateAutenticationAndPrivacyKey(SNMPProtocol.SNMPMessageGeneralData messageGeneralData)
        {            
            bool result = false;
            DriverErrorCodes errorCode = (DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorInvalidDataFormat;

            try
            {
                SNMPProtocol.SNMPUSM usm = GetUsmByPduType(messageGeneralData);
                result = usm.HasValidEngineId();
                if (result)
                {
                    if (usm.SecurityLevel == SNMPProtocol.SecurityLevel.AuthNoPriv || usm.SecurityLevel == SNMPProtocol.SecurityLevel.AuthPriv)
                    {
                        result = usm.IsValidAuthenticationKey();
                        if (!result)
                        {
                            switch (usm.AuthenticationProtocol)
                            {
                                case SNMPProtocol.AuthenticationProtocol.MD5:
                                    usm.IAuthProtocol = new AuthenticationMD5();
                                    break;
                                case SNMPProtocol.AuthenticationProtocol.SHA1:
                                    usm.IAuthProtocol = new AuthenticationSHA1();
                                    break;
                            }
                            if (usm.IAuthProtocol != null)
                            {
                                usm.AuthenticationKey = usm.IAuthProtocol.PasswordToKey(usm.AuthenticationPassword, usm.EngineId);
                                result = true;
                            }
                        }

                        if (usm.SecurityLevel == SNMPProtocol.SecurityLevel.AuthPriv)
                        {
                            result = usm.IsValidPrivacyKey();
                            if (!result)
                            {
                                switch (usm.PrivacyEncryptionProtocol)
                                {
                                    case SNMPProtocol.PrivacyEncryptionProtocol.Des:
                                        usm.IPrivacyProtocol = new PrivacyDES();
                                        break;
                                }
                                if (usm.IPrivacyProtocol != null)
                                {
                                    usm.PrivacyKey = usm.IPrivacyProtocol.PasswordToKey(usm.PrivacyEncryptionPassword, usm.EngineId, usm.IAuthProtocol);
                                    result = true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // catch SnmpSharpNet error
                result = false;
                errorCode = (DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorInvalidDataFormat;
                if (ex is SnmpSharpNet.SnmpException || ex is SnmpSharpNet.SnmpErrorStatusException)
                    errorCode = (DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorEncryptDecriptFailed;
            } 
                                    
            return (result ? DriverErrorCodes.ErrorNoError : errorCode);
        }

        public DriverErrorCodes CalculateAutenticationParameters(byte[] receiveBuffer, SNMPProtocol.SNMPMessageGeneralData messageGeneralData, out byte[] authenticationParameters)
        {
            bool result = false;
            DriverErrorCodes errorCode = (DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorInvalidDataFormat;
            authenticationParameters = new byte[0];

            try
            {
                SNMPProtocol.SNMPUSM usm = GetUsmByPduType(messageGeneralData);
                result = usm.HasValidEngineId();
                if (result)
                {
                    if (usm.SecurityLevel == SNMPProtocol.SecurityLevel.AuthNoPriv || usm.SecurityLevel == SNMPProtocol.SecurityLevel.AuthPriv)
                    {
                        result = usm.IsValidAuthenticationKey();
                        if (result)
                            authenticationParameters = usm.IAuthProtocol.authenticate(usm.AuthenticationKey, receiveBuffer);
                    }
                }
            }
            catch (Exception ex)
            {
                // catch SnmpSharpNet error
                result = false;
                errorCode = (DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorInvalidDataFormat;
                if (ex is SnmpSharpNet.SnmpException || ex is SnmpSharpNet.SnmpErrorStatusException)
                    errorCode = (DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorEncryptDecriptFailed;
            }

            return (result ? DriverErrorCodes.ErrorNoError : errorCode);
        }

        public DriverErrorCodes VerifyAutenticationParameters(byte[] receiveBuffer, SNMPProtocol.SNMPMessageGeneralData messageGeneralData)
        {
            bool result = false;
            DriverErrorCodes errorCode = (DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorUserInvalid;

            try
            {
                result = (messageGeneralData.ParsedUsm.AuthenticationParameters.Length == SNMPProtocol.SNMP_USM_AUTHENTICATIONPARAMETERS_SIZE);
                if (result)
                {
                    errorCode = CalculateAutenticationAndPrivacyKey(messageGeneralData);
                    result = (errorCode == DriverErrorCodes.ErrorNoError);
                    if (result)
                    {
                        SNMPProtocol.SNMPUSM usm = GetUsmByPduType(messageGeneralData);
                        MutableByte wholeMessage = new MutableByte(receiveBuffer);
                        result = usm.IAuthProtocol.authenticateIncomingMsg(usm.AuthenticationKey, messageGeneralData.ParsedUsm.AuthenticationParameters, wholeMessage);
                        if (!result)
                            errorCode = (DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorEncryptDecriptFailed;                        
                    }
                }
            }
            catch (Exception ex)
            {
                // catch SnmpSharpNet error
                result = false;
                errorCode = (DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorInvalidDataFormat;
                if (ex is SnmpSharpNet.SnmpException || ex is SnmpSharpNet.SnmpErrorStatusException)
                    errorCode = (DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorEncryptDecriptFailed;
            }

            return (result ? DriverErrorCodes.ErrorNoError : errorCode);
        }

        public DriverErrorCodes VerifyPrivacyParameters(List<byte> receiveBuffer, int receiveLength, int parsedBytes, SNMPProtocol.SNMPMessageGeneralData messageGeneralData)
        {
            bool result = false;
            DriverErrorCodes errorCode = (DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorUserInvalid;

            try
            {
                errorCode = CalculateAutenticationAndPrivacyKey(messageGeneralData);
                result = (errorCode == DriverErrorCodes.ErrorNoError);
                if (result)
                {
                    SNMPProtocol.SNMPUSM usm = GetUsmByPduType(messageGeneralData);
                    result = (messageGeneralData.ParsedUsm.PrivacyEncryptionParameters.Length == usm.IPrivacyProtocol.PrivacyParametersLength);
                    if (result)
                    {
                        byte[] encryptedPdu = SNMPProtocol.ASN1DecodifyOctetString(receiveBuffer, receiveLength, parsedBytes, out int valueSize);
                        if (result)
                        {
                            byte[] decryptedScopedPdu = usm.IPrivacyProtocol.Decrypt(encryptedPdu, 0, encryptedPdu.Length, usm.PrivacyKey, messageGeneralData.ParsedUsm.EngineBoots, messageGeneralData.ParsedUsm.EngineTime, messageGeneralData.ParsedUsm.PrivacyEncryptionParameters);

                            int decryptedScopedPduParsedBytes = 0;

                            errorCode = SNMPProtocol.ParsePduResponsev3(decryptedScopedPdu.ToList(), decryptedScopedPdu.Length, ref decryptedScopedPduParsedBytes, messageGeneralData);
                            result = (errorCode == DriverErrorCodes.ErrorNoError);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // catch SnmpSharpNet error
                result = false;
                errorCode = (DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorInvalidDataFormat;
                if (ex is SnmpSharpNet.SnmpException || ex is SnmpSharpNet.SnmpErrorStatusException)
                    errorCode = (DriverErrorCodes)SNMPProtocol.SNMPErrorCodes.SNMPErrorEncryptDecriptFailed;
            }

            return (result ? DriverErrorCodes.ErrorNoError : errorCode);
        }
                
        public void ResetAutenticationAndPrivacy(ref SNMPProtocol.SNMPUSM usm)
        {
            usm.ResetEngine();
            if (usm.SecurityLevel != SNMPProtocol.SecurityLevel.NoAuthNoPriv)
            {
                usm.IAuthProtocol = null;
                usm.AuthenticationKey = new byte[0];
                usm.IPrivacyProtocol = null;
                usm.PrivacyKey = new byte[0];
            }
        }

        private bool prepareMapOIDJobsDone = false;
        private void PrepareMapOIDJobs()
        {
            // prepare the map only the 1st time
            if (!prepareMapOIDJobsDone)
            {
                SNMPChannel snmpChannel = (SNMPChannel)Channel;
                foreach (SNMPCommJob snmpJob in ListWholeJob)
                {
                    if (snmpJob.IsValid && (snmpJob.Type == LinkType.InputOutput || snmpJob.Type == LinkType.Input || snmpJob.snmpTrapOnly))
                        snmpChannel.AddToMapOIDJobs(snmpJob.snmpOid_Address, snmpJob);
                }
            }
        }

        public bool IsSuspended()
        {
            bool suspendBitNewValue = false;
            GetStateCommandVariableBit(ref suspendBitNewValue, (UInt16)StationVariableBits.StationActiveCommand);
            
            return suspendBitNewValue;
        }

        #endregion

        #region Properties
        private Boolean _Disconnected;
        public Boolean Disconnected
        {
            get { return _Disconnected; }
            set { _Disconnected = value; }
        }

        public string snmpUserName
        {
            get { return (_Usm == null ? string.Empty : _Usm.GetUserNameString()); }
        }

        SNMPProtocol.SNMPUSM _Usm;
        public SNMPProtocol.SNMPUSM Usm 
        {
            get { return _Usm; }
            set { _Usm = value; }
        }

        SNMPProtocol.SNMPUSM _UsmTrap;
        public SNMPProtocol.SNMPUSM UsmTrap
        {
            get { return _UsmTrap; }
            set { _UsmTrap = value; }
        }

        private SNMPVERSION _snmpVersion;
        public SNMPVERSION snmpVersion
        {
            get { return _snmpVersion; }
            set { _snmpVersion = value; }
        }

        private bool _Discovered;
        public bool Discovered
        {
            get { return _Discovered; }
            set { _Discovered = value; }
        }

        private bool _Authenticated;
        public bool Authenticated
        {
            get { return _Authenticated; }
            set { _Authenticated = value; }
        }
        #endregion
    }
}
