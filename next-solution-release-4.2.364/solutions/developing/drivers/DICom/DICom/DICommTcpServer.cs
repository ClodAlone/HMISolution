using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using IpDriverCodeBase;
using System.Net.Sockets;
using System.Net;
using Opc.Ua;
using DriverCodeBase.Enumerators;
using Utilities;
using System.Threading;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace DICom
{
    // TcpServer 
    public class DICommTcpServer : Channel, IDisposable
    {
        private object lockClients = new object();
        private Dictionary<int, DIComChannel> connectedClients = null;
        private Dictionary<string, DIComStation> allowedStations = null;
        private TcpListener listener = null;
        private DIComChannelSettings settings = null;
        private int clientID = 0;
        private new Dictionary<int, List<DIComProtocol.DiComVar>> clientConfigRecordMaps;
        private object lockStorage = new object();

        public DICommTcpServer(CommunicationDriver commdriver, DIComChannelSettings set)
            : base(commdriver, set, false)
        {
            settings = set;
            connectedClients = new Dictionary<int, DIComChannel>();
            allowedStations = new Dictionary<string, DIComStation>();
            clientConfigRecordMaps = new Dictionary<int, List<DIComProtocol.DiComVar>>();
        }

        #region override Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Working thread. </summary>
        ///
        /// <param name="data" type="object">   The data. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void WorkingThread(object data)
        {            
            int sleepCycle = Properties.Settings.Default.LISTNER_WAIT_TIME;
            if (sleepCycle <= 0)
                sleepCycle = 1;

            if (Properties.Settings.Default.LOG_TO_FILE)
                LogDriverVersion();

            GetAllowedStations();
            // it no stations available report fatal error (and stop channel) setting all tags in error
            if (allowedStations.Count == 0)
            {
                SetChannelInFatalErrorState();
                return;
            }

            DeviceOpen();
            // if local port cannot be opened report fatal error (and stop channel) setting all tags in error
            if (!IsDeviceOpen())
            {
                SetChannelInFatalErrorState();
                return;
            }

            // reload previous configuration from file (exept when launched by wizard)
            if (((DIComDriver)CommDriver).FunctionMode != DIComProtocol.FunctionMode.ConfigMode)
                RestoreClientConfigRecordsFromStorage();

            SetClientCommandState(DIComProtocol.ClientCommandState.Undefined);

            while (true)
            {
                if (listener.Pending())
                {
                    TcpClient client = listener.AcceptTcpClient();

                    if (IsClientAllowed(client))
                    {
                        StartClient(client);
                    }
                    else
                    {
                        client.Close();
                        client.Dispose();
                        client = null;
                    }
                }
                else if (StopWorkerThread.WaitOne(sleepCycle))
                {
                    break;
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Queries if a device is open. </summary>
        ///
        /// <returns>   true if a device is open, false if not. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool IsDeviceOpen()
        {
            return (listener != null);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Queries if a given device open. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool DeviceOpen()
        {
            IPAddress resolvedIPAddress = null;
            if (String.IsNullOrEmpty(settings.TcpChannelSettingsHostName) || !UdpChannel.GetResolvedConnecionIPAddress(settings.TcpChannelSettingsHostName, out resolvedIPAddress))
                UdpChannel.GetResolvedConnecionIPAddress("localhost", out resolvedIPAddress);
            try
            {
                listener = new TcpListener(new IPEndPoint(resolvedIPAddress, settings.TcpChannelSettingsHostPort));
                listener.Start();
            }
            catch (Exception ex)
            {
                CommDriver.OnSystemEvent(null, String.Format(Properties.Resources.ErrorGenericWorkingThread, settings.TcpChannelSettingsHostName, settings.TcpChannelSettingsHostPort, ex.Message), EventSeverity.Min);
                listener = null;
            }

            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Determines if we can device close. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool DeviceClose()
        {
            StopClients();

            try
            {
                if (listener != null)
                {
                    listener.Stop();
                    listener = null;
                }
            }
            catch (Exception ex)
            {
            }

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Synchronous read. </summary>
        ///
        /// <param name="Buffer">   buffer with read values. </param>
        /// <param name="Count">    number of bytes to read. </param>
        ///
        /// <returns>   if true the request byte size was read, othrewise false. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool DeviceRead(byte[] Buffer, uint Count)
        {
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Synchronous write. </summary>
        ///
        /// <param name="Buffer">   buffer with values to write. </param>
        /// <param name="Count">    number of bytes to write. </param>
        ///
        /// <returns>   if true the request write operation was performed, othrewise false. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool DeviceWrite(byte[] Buffer, uint Count)
        {
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets bytes to read. </summary>
        ///
        /// <returns>   The bytes to read. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override uint GetBytesToRead()
        {
            return 0;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets bytes to write. </summary>
        ///
        /// <returns>   The bytes to write. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override uint GetBytesToWrite()
        {
            return 0;
        }

        public override bool TestChannelComm()
        {
            return true;
        }

        #endregion

        #region Custom Methods
        private bool IsClientAllowed(TcpClient client)
        {
            lock (lockClients)
            {
                // only 1 client allowed 
                if (connectedClients.Count > 0)
                    return false;

                if (allowedStations.ContainsKey(DIComProtocol.ALL_CLIENTS_ALLOWED))
                    return true;

                // client ip match configuration ?
                return (allowedStations.ContainsKey((client.Client.RemoteEndPoint as IPEndPoint).Address.ToString()));
            }
        }

        private void StartClient(TcpClient client)
        {
            clientID++;
            DIComChannel channel = new DIComChannel(CommDriver, settings, client, this, clientID);

            channel.AddConfigRecords(RestoreClientConfigRecords());

            channel.Terminated += Channel_Terminated;
            channel.Startup();

            lock (lockClients)
            {
                connectedClients[channel.ClientID] = channel;
            }

            SetClientCommandState(DIComProtocol.ClientCommandState.Connected);
        }

        private void Channel_Terminated(object sender, EventArgs e)
        {
            StopClient((DIComChannel)sender);
        }

        // call this method only when driver is closing
        public void StopClients()
        {
            List<DIComChannel> clients = new List<DIComChannel>();
            lock (lockClients)
            {
                clients.AddRange(connectedClients.Values);
            }
            while (clients.Count > 0)
            {
                StopClient(clients[0], true);
                clients.RemoveAt(0);
            }
        }


        public void StopClient(DIComChannel client, bool closing = false)
        {
            DIComChannel c = null;

            lock (lockClients)
            {
                if (client == null || !connectedClients.ContainsKey(client.ClientID))
                    return;

                c = connectedClients[client.ClientID];
                c.Terminated -= Channel_Terminated;

                BackUpClientConfigRecords(c.GetConfigRecords());

                ResetVarConfigs(c.clientIpAddress);

                connectedClients.Remove(client.ClientID);

                SetClientCommandState(DIComProtocol.ClientCommandState.Disconnected);
            }
                
            // when driver is closing dispose immediatly
            if (closing)
            {
                c.Dispose();
            }
            else
            {
                // otherwise postpone dispose using ThreadPool
                ThreadPool.QueueUserWorkItem(o =>
                {
                    try
                    {
                        c.Dispose();
                    }
                    catch (Exception ex) { }
                });
            }            
        }

        public void SetVarConfig(string ipAddress, DIComProtocol.DiComVar var)
        {
            if (allowedStations.ContainsKey(ipAddress))
                allowedStations[ipAddress].SetVarConfig(var);
            else
                if (allowedStations.ContainsKey(DIComProtocol.ALL_CLIENTS_ALLOWED))
                    allowedStations[DIComProtocol.ALL_CLIENTS_ALLOWED].SetVarConfig(var);
        }

        public void SetVarValue(string ipAddress, DIComProtocol.DiComVar var, DateTime? timeStamp = null, bool forceValueUpdate = false)
        {
            List<DIComCommJob> jobs = null;
            if (allowedStations.ContainsKey(ipAddress))
                jobs = allowedStations[ipAddress].GetJobsMatchRecordIDAndVarIndex(var);
            else
                if (allowedStations.ContainsKey(DIComProtocol.ALL_CLIENTS_ALLOWED))
                    jobs = allowedStations[DIComProtocol.ALL_CLIENTS_ALLOWED].GetJobsMatchRecordIDAndVarIndex(var);

            if (jobs != null)
            {
                foreach (var j in jobs)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                    if (timeStamp.HasValue)
                        eJob.Timestamp = (DateTime)timeStamp;
                    j.ForceValueUpdate = forceValueUpdate;
                    j.LastExecutionTime = DateTime.UtcNow;
                    eJob.Job = j;

                    if (var.Error == DriverErrorCodes.ErrorNoError)
                        eJob.Values = var.VarValue;
                    else
                        eJob.ErrorCode = var.Error;
#if DEBUG
                    System.Diagnostics.Debug.WriteLine(string.Format("OnJobExecuted VarName={0}, DT={1}", j.VarName, eJob.Timestamp.ToString("HH:mm:ss.fff")));
#endif
                    OnJobExecuted(eJob);
                }
            }
        }

        public void ResetVarConfigs(string ipAddress)
        {
            if (allowedStations.ContainsKey(ipAddress))
                allowedStations[ipAddress].ResetVarConfigs();
            else
                if (allowedStations.ContainsKey(DIComProtocol.ALL_CLIENTS_ALLOWED))
                allowedStations[DIComProtocol.ALL_CLIENTS_ALLOWED].ResetVarConfigs();
        }


        /// <summary>
        /// On fatal error (ex. tcp listner cannot be open on specificed port, all tags (and stata variables) were set in error
        /// </summary>
        private void SetChannelInFatalErrorState()
        {
            foreach (DIComStation st in CommDriver.GetChannelStations(this))
            {
                st.SetStateCommandVariableBit(true, (UInt16)StationVariableBits.StationErrorState);
                List<CommJob> totalJobs = st.GetListWholeJobCopy();
                foreach (var job in totalJobs)
                    job.SetQuality(Opc.Ua.StatusCodes.BadConfigurationError);
            }

            SetStateCommandVariableBit(true, (UInt16)ChannelVariableBits.ChannelUnconnected);
            LastErrorCode = (DriverErrorCodes)(DIComProtocol.DIComErrorCodes.ErrorChannelFailedToStart);
        }

        public DriverErrorCodes GetLastErrorCode()
        {
            return LastErrorCode;
        }
        
        private void GetAllowedStations()
        {
            foreach (var s in CommDriver.GetChannelStations(this))
            {
                if (!allowedStations.ContainsKey(((DIComStation)s).IpAddress))
                    allowedStations[((DIComStation)s).IpAddress] = (DIComStation)s;
            }
        }

        /// <summary>
        /// Set client commmand state variable bit
        /// </summary>
        /// <param name="state">Client connection state</param>
        private void SetClientCommandState(DIComProtocol.ClientCommandState state)
        {
            switch (state)
            {
                case DIComProtocol.ClientCommandState.Undefined:
                    foreach (DIComStation s in CommDriver.GetChannelStations(this))
                    {
                        s.SetClientStateCommandVariableBit(true, (ushort)DIComProtocol.ClientCommandState.Undefined);
                        s.SetClientStateCommandVariableBit(false, (ushort)DIComProtocol.ClientCommandState.Connected);
                        s.SetClientStateCommandVariableBit(false, (ushort)DIComProtocol.ClientCommandState.Disconnected);
                    }
                    break;
                case DIComProtocol.ClientCommandState.Connected:
                    foreach (DIComStation s in CommDriver.GetChannelStations(this))
                    {
                        s.SetClientStateCommandVariableBit(false, (ushort)DIComProtocol.ClientCommandState.Undefined);
                        s.SetClientStateCommandVariableBit(true, (ushort)DIComProtocol.ClientCommandState.Connected);
                        s.SetClientStateCommandVariableBit(false, (ushort)DIComProtocol.ClientCommandState.Disconnected);
                    }
                    break;
                case DIComProtocol.ClientCommandState.Disconnected:
                    foreach (DIComStation s in CommDriver.GetChannelStations(this))
                    {
                        s.SetClientStateCommandVariableBit(false, (ushort)DIComProtocol.ClientCommandState.Undefined);
                        s.SetClientStateCommandVariableBit(false, (ushort)DIComProtocol.ClientCommandState.Connected);
                        s.SetClientStateCommandVariableBit(true, (ushort)DIComProtocol.ClientCommandState.Disconnected);
                    }
                    break;
            }
        }

        public void BackUpClientConfigRecords(Dictionary<int, List<DIComProtocol.DiComVar>> config)
        {
            if (config != null && config.Values.Count > 0)
            {
                clientConfigRecordMaps.Clear();

                clientConfigRecordMaps = new Dictionary<int, List<DIComProtocol.DiComVar>>(config);

                BackUpClientConfigRecordsToStorage();
            }
        }

        public Dictionary<int, List<DIComProtocol.DiComVar>> RestoreClientConfigRecords()
        {
            if (clientConfigRecordMaps != null)
                return new Dictionary<int, List<DIComProtocol.DiComVar>>(clientConfigRecordMaps);
            else
                return null;
        }

        private IDataLayer GetClientConfigDataLayer(string stationName, out string fileBase, out InMemoryDataStore InMemory, out bool targetIsFile)
        {
            string conn = DriverCodeBase.CommunicationDriver.GetConnectionString(CommDriver.StrConnectionString, "Drivers", CommDriver.DriverName, ".deviceConfig");

            return DriverCodeBase.CommunicationDriver.GetSpecificDataLayer(conn,out fileBase, out InMemory, out targetIsFile);
        }

        public void RestoreClientConfigRecordsFromStorage()
        {
            lock (lockStorage)
            {
                string stationName = CommDriver.GetChannelStations(this)[0].Name;

                using (IDataLayer idl = GetClientConfigDataLayer(stationName, out string fileBase, out InMemoryDataStore InMemory, out bool targetIsFile))
                {
                    using (UnitOfWork ufw = new UnitOfWork(idl))
                    {
                        DIComClientConfig cfg = null;
                        try
                        {
                            cfg = (from s in new XPQuery<DIComClientConfig>(ufw) where s.StationName == stationName select s).SingleOrDefault();
                        }
                        catch (Exception ex)
                        { }

                        if (cfg != null && cfg.Vars != null)
                        {
                            clientConfigRecordMaps.Clear();
                            foreach (DIComClientConfigVar var in cfg.Vars)
                            {
                                if (!clientConfigRecordMaps.ContainsKey(var.RecordID))
                                    clientConfigRecordMaps[var.RecordID] = new List<DIComProtocol.DiComVar>();
                                clientConfigRecordMaps[var.RecordID].Add(new DIComProtocol.DiComVar(var));
                            }
                        }
                    }
                }
            }
        }

        public void BackUpClientConfigRecordsToStorage()
        {
            lock (lockStorage)
            {
                string stationName = CommDriver.GetChannelStations(this)[0].Name;

                using (IDataLayer idl = GetClientConfigDataLayer(stationName, out string fileBase, out InMemoryDataStore InMemory, out bool targetIsFile))
                {
                    using (UnitOfWork ufw = new UnitOfWork(idl))
                    {
                        try
                        {
                            // get tag list from selected station
                            DIComClientConfig st = (from s in new XPQuery<DIComClientConfig>(ufw).AsParallel() where s.StationName == stationName select s).SingleOrDefault();
                            if (st != null)
                            {
                                List<DIComClientConfigVar> vars = (from v in new XPQuery<DIComClientConfigVar>(ufw).AsParallel() where v.DIComClientConfig == st select v).ToList();
                                if (vars != null && vars.Count > 0)                                    
                                    ufw.Delete(vars);

                                ufw.Delete(st);
                            }
                        }
                        catch (Exception ex)
                        { }

                        var clientConfig = new DIComClientConfig(ufw);
                        clientConfig.LastInteraction = DateTime.UtcNow;
                        clientConfig.StationName = stationName;
                        foreach (var recordID in clientConfigRecordMaps.Keys)
                        {
                            foreach (DIComProtocol.DiComVar var in clientConfigRecordMaps[recordID])
                                clientConfig.Vars.Add(new DIComClientConfigVar(ufw, var));
                        }

                        ufw.CommitChanges();

                        if (targetIsFile)
                        {
                            try
                            {
                                InMemory.WriteXml(fileBase);
                            }
                            catch (Exception ex)
                            { }
                        }
                    }
                }
            }
        }

        private void LogDriverVersion()
        {
            try
            {
                CommunicationDriver.log.Debug(string.Format("{0} Driver Product Version {1}", CommDriver.DriverName, FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion.ToString()));
            }
            catch { }
        }
        #endregion

        #region properties
        #endregion

        #region IDisposable
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void Dispose()
        {
            base.Dispose();

            DeviceClose();
        }
        #endregion
    }
}
