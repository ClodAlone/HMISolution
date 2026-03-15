using System;
using DriverCodeBase;
using IpDriverCodeBase;
using System.Net.Sockets;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using DevExpress.Xpo.DB;
using DevExpress.Xpo;
using System.Text.RegularExpressions;
using DriverCodeBase.Enumerators;


namespace DICom
{
    public class DIComChannel : TcpChannel
    {
        #region Members        
        // event generated when WorkingThead is terminated
        public event EventHandler<EventArgs> Terminated;
        DICommTcpServer callBackChannel;
        public string clientIpAddress;
        private int clientTcpPort;
        public int ClientID;
        // record id, <var index, var>
        private Dictionary<int, List<DIComProtocol.DiComVar>> configRecordMaps;
        private Dictionary<int, List<DIComProtocol.DiComVar>> dataRecordMaps;
        private DIComProtocol.FunctionMode _FunctionMode;
        private bool _LogToFile;
        private uint _MessagesCollectingTimeout;
        private DateTime _MessagesCollectingStart;
        private bool _RestoredConfigRecords;
        private uint inactivityTime;
        #endregion

        #region Constructors
        public DIComChannel(CommunicationDriver commdriver, DIComChannelSettings settings, TcpClient client, DICommTcpServer callBackCh, int clientID)
            : base(commdriver, settings, false, client)
        {            
            callBackChannel = callBackCh;
            ClientID = clientID;
            clientIpAddress = (client.Client.RemoteEndPoint as IPEndPoint).Address.ToString();
            clientTcpPort = settings.TcpChannelSettingsHostPort;
            configRecordMaps = new Dictionary<int, List<DIComProtocol.DiComVar>>();
            dataRecordMaps = new Dictionary<int, List<DIComProtocol.DiComVar>>();
            _FunctionMode = ((DIComDriver)commdriver).FunctionMode;
            _LogToFile = Properties.Settings.Default.LOG_TO_FILE;
            if (settings.DataCollectingTimeout.HasValue)
                _MessagesCollectingTimeout = (uint)settings.DataCollectingTimeout;
            else
                _MessagesCollectingTimeout = 0;

            _MessagesCollectingStart = DateTime.MinValue;
            _RestoredConfigRecords = false;
            // calculate inactivity timeout (no data from device for XX sec)
            inactivityTime = (uint)((Timeout * 2) / 1000);
        }
        #endregion

        #region Override Methods              
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Working thread. </summary>
        ///
        /// <param name="data" type="object">   The data. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////        
        protected override void WorkingThread(object data)
        {
            int sleepCycle = 0;

            // setup driver's behaviour on _MessagesCollectingTimeout value
            SetWorkingMode(ref sleepCycle);
            
            if (_LogToFile)
                LogToFile("Device connect");

            DIComProtocol.ReceiveItem receiveItem = new DIComProtocol.ReceiveItem();
            //List<DIComProtocol.ReceiveItem> receiveItems = new List<DIComProtocol.ReceiveItem>();
            bool anyData = false;
            
            receiveItem.ExpectedClientMessageSize = DIComProtocol.HEADER.SIZE;
            BeginDeviceRead(receiveItem.ExpectedClientMessageSize);
            while (true)
            {               
                anyData = NewDataToAnlyze.WaitOne(sleepCycle);
                // request to stop thread ?
                if (StopWorkerThread.WaitOne(0))
                    break;

                lock (lockThreadObject)
                {
                    NewDataToAnlyze.Reset();
                }               
                                
                ReceiveLoopItem(anyData, receiveItem);                
                BeginDeviceRead(receiveItem.ExpectedClientMessageSize);

                ConsumeReceiveItem(receiveItem);

                // client disconnected (or no data for inactivityTime) ?
                if (!Connected(inactivityTime))
                    break;
            }

            // before to exit, publish collected data
            if (SomeCollectedMessagesToPublish())
                ForcePublishCollectedMessages();

            if (_LogToFile)
                LogToFile("Device disconnect");

            // send event to TcpServer so it can dispose channel
            EventHandler<EventArgs> temp = Terminated;
            if (temp != null)
            {
                temp(this, new EventArgs());
            }
        }

        public override bool TestChannelComm()
        {
            return true;
        }

        public override void Dispose()
        {
            base.Dispose();
            
            DeviceClose();            
        }

        #endregion

        #region methods          
        private void CleanUpTimeOutItem(DIComProtocol.ReceiveItem receiveItem)
        {
            if (_LogToFile)
            {
                if (receiveItem.GetRawData().Count > 0)
                {
                    LogToFile("Message Incomplete Raw Data receiveItem");
                    LogToFileDeviceMessage(receiveItem.GetRawData());
                }

                if (ReceiveBuffer.Count > 0)
                {
                    LogToFile("Message Incomplete Raw Data ReceiveBuffer");
                    LogToFileDeviceMessage(ReceiveBuffer);
                }
            }
            Flush();
            CleanUpReceiveItem(receiveItem);
        }

        private void CleanUpReceiveItem(DIComProtocol.ReceiveItem receiveItem)
        {
            receiveItem.CleanUpRawData();
            receiveItem.Init(string.Empty);
            lock (lockThreadObject)
            {
                ReceiveBuffer.Clear();
            }
            receiveItem.ExpectedClientMessageSize = DIComProtocol.HEADER.SIZE;
        }

        private void ReceiveLoopItem(bool anyData, DIComProtocol.ReceiveItem receiveItem)
        {
            if (receiveItem.ReceiveState == DIComProtocol.ReceiveStates.Init)
            {
                receiveItem.Init(clientIpAddress);
                receiveItem.ReceiveState = DIComProtocol.ReceiveStates.WaitHeader;
            }

            if (receiveItem.ReceiveState == DIComProtocol.ReceiveStates.WaitHeader)
            {
                // new data arrived ?
                byte[] header;
                lock (lockThreadObject)
                {
                    // security check --> when buffer size contain too match "data" is probably corrupted data --> discard part of it
                    if (ReceiveBuffer.Count > DIComProtocol.RECEIVE_BUFFER_SECURITY_SIZE)
                        ReceiveBuffer.RemoveRange(0, ReceiveBuffer.Count - DIComProtocol.RECEIVE_BUFFER_SECURITY_SIZE);

                    header = new byte[ReceiveBuffer.Count];
                    ReceiveBuffer.CopyTo(0, header, 0, ReceiveBuffer.Count);
                }

                DIComProtocol.HEADER Header = new DIComProtocol.HEADER();
                Header.Expand(ref header);

                switch (Header.ExpandResult)
                {
                    case DIComProtocol.ExpandResult.Ok:

                        receiveItem.Header = Header;
                        switch (Header.LoadType)
                        {
                            case DIComProtocol.LoadType.CONFIG:
                            case DIComProtocol.LoadType.DATA:
                                lock (lockThreadObject)
                                {
                                    ReceiveBuffer.RemoveRange(0, (receiveItem.Header.HeaderStartOffset + DIComProtocol.HEADER.SIZE));
                                }
                                receiveItem.ReceiveState = DIComProtocol.ReceiveStates.WaitData;
                                receiveItem.ExpectedClientMessageSize = Header.LoadSize;
                                return;

                            case DIComProtocol.LoadType.KEEP_ALIVE:
                                receiveItem.IsValid = true;
                                lock (lockThreadObject)
                                {
                                    ReceiveBuffer.RemoveRange(0, (receiveItem.Header.HeaderStartOffset + receiveItem.Header.TotalLoadSize));
                                }
                                // message completed --> return to initial state
                                receiveItem.ReceiveState = DIComProtocol.ReceiveStates.Init;
                                // manage KEEP_ALIVE short message version
                                if (receiveItem.Header.TotalLoadSize < DIComProtocol.HEADER.SIZE)
                                    receiveItem.ExpectedClientMessageSize = DIComProtocol.HEADER.SIZE - receiveItem.Header.TotalLoadSize;
                                else
                                    receiveItem.ExpectedClientMessageSize = DIComProtocol.HEADER.SIZE;

                                return;
                        }
                        break;
                    case DIComProtocol.ExpandResult.MessageParsingError:
                        if (_LogToFile)
                        {
                            if (receiveItem.GetRawData().Count > 0)
                            {
                                LogToFile("MessageParsingError Raw Data receiveItem");
                                LogToFileDeviceMessage(receiveItem.GetRawData());
                            }

                            if (ReceiveBuffer.Count > 0)
                            {
                                LogToFile("MessageParsingError Raw Data ReceiveBuffer");
                                LogToFileDeviceMessage(ReceiveBuffer);
                            }
                        }
                        CleanUpReceiveItem(receiveItem);
                        break;
                    case DIComProtocol.ExpandResult.MessageIncomplete:
                        // do nothing !!! Wait more data
                        break;
                }
            }

            if (receiveItem.ReceiveState == DIComProtocol.ReceiveStates.WaitData)
            {
                // new data arrived ?
                byte[] pdu;
                lock (lockThreadObject)
                {
                    pdu = new byte[ReceiveBuffer.Count];
                    ReceiveBuffer.CopyTo(0, pdu, 0, ReceiveBuffer.Count);
                }

                DIComProtocol.PDU Pdu = new DIComProtocol.PDU();
                switch (receiveItem.Header.LoadType)
                {
                    case DIComProtocol.LoadType.CONFIG:
                        Pdu.ExpandConfig(receiveItem.Header, ref pdu);
                        break;
                    case DIComProtocol.LoadType.DATA:
                        Pdu.ExpandData(receiveItem.Header, configRecordMaps, ref pdu);
                        break;
                }

                switch (Pdu.ExpandResult)
                {
                    case DIComProtocol.ExpandResult.Ok:
                        receiveItem.Pdu = Pdu;
#if DEBUG
                        int pendingBytes = 0;
#endif
                        switch (receiveItem.Header.LoadType)
                        {
                            case DIComProtocol.LoadType.CONFIG:
                                lock (lockThreadObject)
                                {
                                    ReceiveBuffer.RemoveRange(0, receiveItem.Header.LoadSize);
#if DEBUG
                                    pendingBytes = ReceiveBuffer.Count();
#endif
                                }

#if DEBUG
                                System.Diagnostics.Debug.WriteLine(string.Format("CONFIG remove {0}, pending {1}", receiveItem.Header.TotalLoadSize, pendingBytes));
                                System.Diagnostics.Debug.WriteLine("Raw Config Data");
                                System.Diagnostics.Debug.WriteLine(string.Join(",", receiveItem.GetRawData().Select(b => b.ToString())));
                                System.Diagnostics.Debug.WriteLine("Record ID, VarIndex, VarTpe, VarSize, Tag, Tag");
                                foreach (var v in receiveItem.Pdu.Vars)
                                    System.Diagnostics.Debug.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},", v.RecordID, v.VarIndex, v.VarType, v.VarSize, v.VarName, v.VarName));
                                System.Diagnostics.Debug.WriteLine("--------------------------------------------------------------------");
#endif
                                if (_LogToFile)
                                {
                                    LogToFile("Raw Config Data");
                                    LogToFileDeviceMessage(receiveItem.GetRawData());
                                    LogToFile("Record ID, VarIndex, VarTpe, VarSize, Tag, Tag");
                                    foreach (var v in receiveItem.Pdu.Vars)
                                        LogToFileNoDateTime(string.Format("{0},{1},{2},{3},{4},{5},", v.RecordID, v.VarIndex, v.VarType, v.VarSize, v.VarName, v.VarName));
                                }

                                receiveItem.IsValid = true;

                                //DiscardRecord(receiveItem, 3);
                                break;
                            case DIComProtocol.LoadType.DATA:
                                lock (lockThreadObject)
                                {
                                    ReceiveBuffer.RemoveRange(0, receiveItem.Header.LoadSize);
#if DEBUG
                                    pendingBytes = ReceiveBuffer.Count();
#endif
                                }
#if DEBUG
                                System.Diagnostics.Debug.WriteLine(string.Format("DATA remove {0}, pending {1}", receiveItem.Header.TotalLoadSize, ReceiveBuffer.Count()));
                                System.Diagnostics.Debug.WriteLine("Raw Data");
                                System.Diagnostics.Debug.WriteLine(string.Join(",", receiveItem.GetRawData().Select(b => b.ToString())));
                                foreach (var v in receiveItem.Pdu.Vars)
                                    System.Diagnostics.Debug.WriteLine(string.Format("Data RecordID {0} VarIndex {1} VarName {2} VarTpe {3} VarSize {4} VarValue {5}", v.RecordID, v.VarIndex, v.VarName, v.VarType, v.VarSize, v.GetVarValue()));
#endif
                                receiveItem.IsValid = true;

                                if (_LogToFile)
                                {
                                    LogToFile("Raw Data");
                                    LogToFileDeviceMessage(receiveItem.GetRawData());                                    
                                    foreach (var v in receiveItem.Pdu.Vars)
                                        LogToFileNoDateTime(string.Format("Data RecordID {0} VarIndex {1} VarName {2} VarTpe {3} VarSize {4} VarValue {5}", v.RecordID, v.VarIndex, v.VarName, v.VarType, v.VarSize, v.GetVarValue()));
                                }

                                break;
                        }
                        // message completed --> return to initial state
                        receiveItem.ReceiveState = DIComProtocol.ReceiveStates.Init;
                        receiveItem.ExpectedClientMessageSize = DIComProtocol.HEADER.SIZE;
                        break;
                    case DIComProtocol.ExpandResult.MessageParsingError:
                        if (_LogToFile)
                        {
                            if (receiveItem.GetRawData().Count > 0)
                            {
                                LogToFile("MessageParsingError Raw Data receiveItem (Pdu)");
                                LogToFileDeviceMessage(receiveItem.GetRawData());
                            }

                            if (ReceiveBuffer.Count > 0)
                            {
                                LogToFile("MessageParsingError Raw Data ReceiveBuffer (Pdu)");
                                LogToFileDeviceMessage(ReceiveBuffer);
                            }
                        }
                        CleanUpReceiveItem(receiveItem);
                        break;
                    case DIComProtocol.ExpandResult.MessageIncomplete:
                        // do nothing !!! Wait more data
                        break;
                }
            }
        }

        private void ConsumeReceiveItem(DIComProtocol.ReceiveItem receiveItem)
        {            
            if (receiveItem.IsValid)
            {
                switch (receiveItem.Header.LoadType)
                {
                    case DIComProtocol.LoadType.CONFIG:
                        lock (lockThreadObject)
                        {
                            AddConfigVars(receiveItem.IpAddress, receiveItem.Header.RecordID, receiveItem.Pdu.Vars);
                        }
                        break;
                    case DIComProtocol.LoadType.DATA:

                        // discard data record with invalid ID
                        if (IsValidDataRecordId(receiveItem.Pdu.Vars))
                        {
                            switch (_FunctionMode)
                            {
                                case DIComProtocol.FunctionMode.Standard:
                                    {
                                        if (_LogToFile)
                                            LogToFile("Data is published immediatly");

                                        foreach (var varValue in receiveItem.Pdu.Vars)
                                            callBackChannel.SetVarValue(receiveItem.IpAddress, varValue);
                                    }
                                    break;
                                case DIComProtocol.FunctionMode.PublishMessageDataWithSameDateTime:
                                    {
                                        if (_LogToFile)
                                            LogToFile("Data is published immediatly");

                                        DateTime publishDateTime = DateTime.UtcNow;
                                        foreach (var varValue in receiveItem.Pdu.Vars)
                                            callBackChannel.SetVarValue(receiveItem.IpAddress, varValue, publishDateTime, true);
                                    }
                                    break;
                                case DIComProtocol.FunctionMode.ForcePublishCollectedMessagesDataWithSameDateTime:
                                    {
                                        if (AllExpectedMessagesArrived(receiveItem, out List<DIComProtocol.DiComVar> vars))
                                        {
                                            DateTime publishDateTime = DateTime.UtcNow;
                                            foreach (var varValue in vars)
                                                callBackChannel.SetVarValue(receiveItem.IpAddress, varValue, publishDateTime, true);
                                        }
                                    }
                                    break;
                                case DIComProtocol.FunctionMode.ConfigMode:
                                    // do not publish any data
                                    break;
                            }
                        }
                        break;
                    case DIComProtocol.LoadType.KEEP_ALIVE:
                        List<byte> keepAliveMsg = new List<byte>();

                        DIComProtocol.HEADER h = new DIComProtocol.HEADER();
                        h.LoadType = DIComProtocol.LoadType.KEEP_ALIVE;
                        keepAliveMsg.AddRange(h.Pack());
                        DeviceWrite(keepAliveMsg.ToArray(), (uint)keepAliveMsg.Count);
                        break;
                }
                receiveItem.CleanUpRawData();
            }

            // check if is time to publish collected data
            if (SomeCollectedMessagesToPublish() && IsMessagesCollectingTimeOut())
                ForcePublishCollectedMessages();
        }
        
        private bool AllExpectedMessagesArrived(DIComProtocol.ReceiveItem receiveItem, out List<DIComProtocol.DiComVar> vars)
        {
            vars = new List<DIComProtocol.DiComVar>();

            // if already "collected" message arrived (Branson didn't send all message !!!), publish collected data and store it
            if (dataRecordMaps.ContainsKey(receiveItem.Header.RecordID))
            {
                if (_LogToFile)
                    LogToFile(string.Format("Some data to publish -->  Config Records Count={0}, Data Records Count={1}", configRecordMaps.Count, dataRecordMaps.Count));

                vars = GetAndRemoveVarsFromCollectedMessages(true);
                                
                dataRecordMaps[receiveItem.Header.RecordID] = receiveItem.Pdu.Vars;
            }
            else
            {
                dataRecordMaps[receiveItem.Header.RecordID] = receiveItem.Pdu.Vars;

                // all expected messages arrived ?
                if (dataRecordMaps.Count == configRecordMaps.Count)
                {
                    if (_LogToFile)
                        LogToFile(string.Format("Publish all data --> Config Records Count={0}, Data Records Count={1}", configRecordMaps.Count, dataRecordMaps.Count));

                    vars = GetAndRemoveVarsFromCollectedMessages(false);
                }
            }

            // on last data collected record, start collected data timeout
            if (dataRecordMaps.Count != 0)
                _MessagesCollectingStart = DateTime.UtcNow;

            // some data to publish ?
            return (vars.Count > 0);
        }

        private List<DIComProtocol.DiComVar> GetAndRemoveVarsFromCollectedMessages(bool addWithBadQualityUnCollctedVars)
        {
            List<DIComProtocol.DiComVar> vars = new List<DIComProtocol.DiComVar>();

            foreach (var v in dataRecordMaps.Values)
                vars.AddRange(v);

            dataRecordMaps.Clear();

            // add to collected vars also not collected (difference between collected vars and received config vars) vars with bad quality
            if (addWithBadQualityUnCollctedVars)
                AddNotPublishedVarsWithBadQuality(vars);

            return vars;
        }

        private bool IsDummyConfigRecord(List<DIComProtocol.DiComVar> vars)
        {
            // no vars into config records must be consider invalid
            if (vars.Count == 0)
            {
                if (_LogToFile)
                    LogToFile(string.Format("Config record discarded because nr vars = 0 , Config Records Count={0}", configRecordMaps.Count));

                return true;
            }
            // A dummy record should have: a) only 1 variable , and b) a generic tag name such as “Tag[record_id#]” (e.g. “Tag4”).
            else if (vars.Count == 1)
            {
                if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.DUMMY_CONFIG_RECORD_PATTERN))
                {
                    Regex TagNameParser = new Regex(@Properties.Settings.Default.DUMMY_CONFIG_RECORD_PATTERN, RegexOptions.IgnoreCase);
                    Match TagNameMatch = TagNameParser.Match(vars[0].VarName.ToUpper());
                    if (TagNameMatch.Success)
                    {
                        if (_LogToFile)
                            LogToFile(string.Format("Dummy record discarded = {0}, Config Records Count={1}", vars[0].VarName, configRecordMaps.Count));
                    }
                    return TagNameMatch.Success;
                }
            }

            return false;
        }

        private bool IsValidDataRecordId(List<DIComProtocol.DiComVar> vars)
        {
            // check if sent data record ID match to config map
            if (vars.Count > 0)
            {
                if (!configRecordMaps.ContainsKey(vars[0].RecordID))
                {
                    if (_LogToFile)
                        LogToFile(string.Format("Data Record ID {0} discarded because invalid", vars[0].RecordID));
                    return false;
                }                
            }

            return true;
        }

        private void DiscardRecord(DIComProtocol.ReceiveItem receiveItem, int recordID)
        {
            if (receiveItem.Pdu.Vars.Count > 0 && receiveItem.Pdu.Vars[0].RecordID == recordID)
            {
                if (_LogToFile)
                    LogToFile(string.Format("Discarded specific Record ID {0}", receiveItem.Pdu.Vars[0].VarName));
                CleanUpReceiveItem(receiveItem);
                receiveItem.IsValid = false;
            }
        }

        private bool IsMessagesCollectingTimeOut()
        {
            return (_MessagesCollectingStart != DateTime.MinValue && DateTime.UtcNow.Subtract(_MessagesCollectingStart).TotalMilliseconds > _MessagesCollectingTimeout);
        }

        private bool SomeCollectedMessagesToPublish()
        {
            return (_FunctionMode == DIComProtocol.FunctionMode.ForcePublishCollectedMessagesDataWithSameDateTime && dataRecordMaps.Count > 0);
        }

        private void ForcePublishCollectedMessages()
        {
            if (_LogToFile)
                LogToFile(string.Format("Force publish collected data because Collecting Data TimeOut elapsed --> Config Records Count={0}, Data Records Count={1}", configRecordMaps.Count, dataRecordMaps.Count));

            List<DIComProtocol.DiComVar> vars = GetAndRemoveVarsFromCollectedMessages(true);

            DateTime publishDateTime = DateTime.UtcNow;
            foreach (var varValue in vars)
                callBackChannel.SetVarValue(clientIpAddress, varValue, publishDateTime, true);
        }

        private void SetWorkingMode(ref int sleepCycle)
        {
            if (_MessagesCollectingTimeout == 0)
            {
                _FunctionMode = DIComProtocol.FunctionMode.PublishMessageDataWithSameDateTime;
                sleepCycle = Timeout;
            }
            else
            {
                _FunctionMode = DIComProtocol.FunctionMode.ForcePublishCollectedMessagesDataWithSameDateTime;
                sleepCycle = (int)_MessagesCollectingTimeout;
            }

            if (sleepCycle <= 0)
                sleepCycle = 1;
        }

        private void AddNotPublishedVarsWithBadQuality(List<DIComProtocol.DiComVar> publishVars)
        {
            foreach (var configVars in configRecordMaps.Values)
            {
                foreach (var configVar in configVars)
                {
                    if (publishVars.Count(v => v.RecordID == configVar.RecordID && v.VarName == configVar.VarName) == 0)
                    {
                        DIComProtocol.DiComVar newVar = (DIComProtocol.DiComVar)configVar.Clone();
                        newVar.Error = (DriverErrorCodes)DIComProtocol.DIComErrorCodes.ErrorVarNotCollected;
                        publishVars.Add(newVar);
                    }
                }
            }
        }

        private void AddConfigVars(string ipAddress, int recordID, List<DIComProtocol.DiComVar> vars)
        {
            // discard config record used for "future" scope
            if (!IsDummyConfigRecord(vars))
            {
                // if channel was started using a old configuration, discard it when device send 1st valid configuration
                if (_RestoredConfigRecords)
                {
                    configRecordMaps.Clear();
                    callBackChannel.ResetVarConfigs(ipAddress);
                    _RestoredConfigRecords = false;
                }
                // store var configuration into local map (by RecordID), so it can be used during parsing data (LoadType.DATA)
                if (configRecordMaps.ContainsKey(recordID))
                    configRecordMaps[recordID].Clear();
                else
                    configRecordMaps[recordID] = new List<DIComProtocol.DiComVar>();
                configRecordMaps[recordID].AddRange(vars);

                foreach (var varConfig in vars)
                    callBackChannel.SetVarConfig(ipAddress, varConfig);
            }
        }

        private void logToFile(string message)
        {
            CommunicationDriver.log.Debug(string.Format("{0}_{1};{2}", clientIpAddress, clientTcpPort, message));
        }

        private void LogToFile(string message)
        {
            logToFile(string.Format("{0}-{1}", DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss.fff"), message));
        }

        private void LogToFileNoDateTime(string message)
        {
            logToFile(message);
        }

        private void LogToFileDeviceMessage(List<byte> message)
        {
            logToFile(string.Join(",", message.Select(b => b.ToString())));
        }

        public void AddConfigRecords(Dictionary<int, List<DIComProtocol.DiComVar>> configs)
        {
            if (configs != null)
            {                
                foreach (var elemen in configs)
                    AddConfigVars(clientIpAddress, elemen.Key, elemen.Value);

                _RestoredConfigRecords = true;
            }
        }
        
        public Dictionary<int, List<DIComProtocol.DiComVar>> GetConfigRecords()
        {
            return configRecordMaps;
        }
        #endregion
    }
}
