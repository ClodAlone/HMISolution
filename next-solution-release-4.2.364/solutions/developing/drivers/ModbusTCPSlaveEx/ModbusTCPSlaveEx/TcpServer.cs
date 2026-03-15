using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using IpDriverCodeBaseEx;
using System.Net.Sockets;
using System.Net;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;

namespace ModbusTCPSlave
{
    // TcpServer 
    public class TcpServer : Channel, IDisposable
    {
        private Dictionary<int, ModbusTCPSlaveChannel> connections;
        TcpListener listener = null;
        TcpListener listenerBackup = null;
        /// <summary>   The lock list object. </summary>
        readonly Object lockListObject = new Object();
        private ModbusTCPSlaveChannelSettings settings;
        List<ModbusTCPSlaveStation> listStation;
        List<string> listIpAddress;
        private int clientID;

        public TcpServer(CommunicationDriver commdriver, ModbusTCPSlaveChannelSettings set)
            : base(commdriver, set)
        {
            settings = set;
            connections = new Dictionary<int, ModbusTCPSlaveChannel>();
            clientID = 0;
        }

        #region override Methods

        IAsyncResult acceptTcpClientResult = null;
        bool beginAcceptTcpClientAlreadyDone = false;
        IAsyncResult acceptTcpClientBackUpResult = null;
        bool beginAcceptTcpClientBackUpAlreadyDone = false;
        object lockStream = new object();


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Queries if a device is open. </summary>
        ///
        /// <returns>   true if a device is open, false if not. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool IsDeviceOpen()
        {
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Queries if a given device open. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool DeviceOpen()
        {
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Determines if we can device close. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool DeviceClose()
        {
            return false;
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
        #endregion

        #region Methods
        public void PublicOnJobExecuted(ExecutedJobArgs e)
        {
            OnJobExecuted(e);
        }

        public bool ValidateStation(ref ReceiveItem receiveItem)
        {
            switch (listStation.Count)
            { 
                case 0:
                    return true;
                case 1:
                    receiveItem.Station = 0;
                    return true;
                default:
                    for (byte i = 0; i < listStation.Count(); i++)
                    {
                        if(listStation[i].AddressingMode == ModbusTCPSlaveStation.AddressingModes.UnitID)
                        {
                            if (listStation[i].StationID == receiveItem.Pdu.unit &&
                                !listIpAddress.Contains(receiveItem.IpAddress))
                            {
                                receiveItem.Station = i;
                                return true;
                            }
                        }
                        else if (listStation[i].IpAddress == receiveItem.IpAddress || listStation[i].BackupIpAddress == receiveItem.IpAddress)
                        {
                            receiveItem.Station = i;
                            return true;
                        }
                    }
                    return false;
            }
        }

        public byte[] getMemoryData(ref ReceiveItem receiveItem)
        {
            return listStation[receiveItem.Station].getMemoryData(ref receiveItem);
        }

        public void setMemoryData(ref ReceiveItem receiveItem, ref byte[] buffer, out List<ModbusTCPSlaveCommJob> changeJobs)
        {
            listStation[receiveItem.Station].setMemoryData(ref receiveItem, ref buffer, out changeJobs);
        }

        public void getChangedJobs(ref ReceiveItem receiveItem, out List<ModbusTCPSlaveCommJob> changeJobs)
        {
            listStation[receiveItem.Station].getChangedJobs(ref receiveItem, out changeJobs);
        }

        public void ExecuteReadJob(ref List<ModbusTCPSlaveCommJob> readJobs)
        {
            if (readJobs.Count == 0)
                return;

            foreach (ModbusTCPSlaveCommJob pendingjob in readJobs)
            {
                if (pendingjob.ReadRequest())
                {
                    ExecuteJob(pendingjob);                    
                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                    eJob.Values = (pendingjob.Station as ModbusTCPSlaveStation).getMemoryData(pendingjob);
                    eJob.Job = pendingjob;
                    OnJobExecuted(eJob);
                }
            }
        }

        public void ExecuteWriteJob(ref List<ModbusTCPSlaveCommJob> writeJobs)
        {
            if (writeJobs.Count() == 0)
                return;
            foreach (ModbusTCPSlaveCommJob pendingjob in writeJobs)
            {
                if (!pendingjob.ReadRequest())
                {
                    byte[] writeData;
                    if (pendingjob.PrepareData(out writeData))
                    {
                        ExecuteJob(pendingjob);                        
                        (pendingjob.Station as ModbusTCPSlaveStation).setMemoryData(pendingjob, ref writeData);
                        ExecutedJobArgs eJob = new ExecutedJobArgs();
                        eJob.Values = new byte[0];
                        eJob.Job = pendingjob;
                        OnJobExecuted(eJob);
                    }
                }
                else
                {
                    ExecuteJob(pendingjob);
                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                    eJob.Values = (pendingjob.Station as ModbusTCPSlaveStation).getMemoryData(pendingjob);
                    eJob.Job = pendingjob;
                    OnJobExecuted(eJob);
                }
            }
        }

        /// <summary>
        /// On fatal error (ex. tcp listner cannot be open on specificed port, all tags (and stata variables) were set in error
        /// </summary>
        private void SetChannelInFatalErrorState()
        {                       
            foreach (ModbusTCPSlaveStation st in CommDriver.GetChannelStations(this))
            {
                if (st.AddressingMode != ModbusTCPSlaveStation.AddressingModes.Invalid)
                {
                    st.SetStateCommandVariableBit(true, (UInt16)StationVariableBits.StationErrorState);
                                        
                    List<ModbusTCPSlaveCommJob> totalJobs = st.totalJobs();
                    foreach (var job in totalJobs)
                        job.SetQuality(Opc.Ua.StatusCodes.BadConfigurationError);
                }
            }

            SetStateCommandVariableBit(true, (UInt16)ChannelVariableBits.ChannelUnconnected);
        }

        private void Channel_Terminated(object sender, EventArgs e)
        {
            StopClient((ModbusTCPSlaveChannel)sender);
        }

        // call this method only when driver is closing
        public void StopClients()
        {
            List<ModbusTCPSlaveChannel> clients = new List<ModbusTCPSlaveChannel>();
            lock (lockListObject)
            {
                clients.AddRange(connections.Values);
            }
            while (clients.Count > 0)
            {
                StopClient(clients[0]);
                clients.RemoveAt(0);
            }
        }

        public void StopClient(ModbusTCPSlaveChannel client)
        {
            ModbusTCPSlaveChannel c = null;
            lock (lockListObject)
            {
                if (client == null || !connections.ContainsKey(client.ClientID))
                    return;

                c = connections[client.ClientID];
                c.Terminated -= Channel_Terminated;

                connections.Remove(client.ClientID);
            }

            try
            {
                c.Dispose();
            } catch { }
        }

        private void InitTagsAndStations()
        {
            listStation = new List<ModbusTCPSlaveStation>();
            listIpAddress = new List<string>();

            foreach (ModbusTCPSlaveStation st in CommDriver.GetChannelStations(this))
            {
                if (st.AddressingMode != ModbusTCPSlaveStation.AddressingModes.Invalid)
                {
                    listStation.Add(st as ModbusTCPSlaveStation);
                    st.createMemoryData();
                    ModbusTCPSlaveDriver drv = (st.GetCommDriver() as ModbusTCPSlaveDriver);
                    List<ModbusTCPSlaveCommJob> totalJobs = st.totalJobs();
                    foreach (var job in totalJobs)
                    {
                        List<NodeId> lista = new List<NodeId>();

                        foreach (var tag in job.TagsList)
                        {
                            tag.Value.StatusCode = Opc.Ua.StatusCodes.Good;
                            lista.Add(tag.TagNode.NodeId);
                        }

                        foreach (var n in lista)
                            drv.OnTagChanged(n, new DataValue(Opc.Ua.StatusCodes.Good));
                    }
                    if (st.AddressingMode == ModbusTCPSlaveStation.AddressingModes.IpAddres)
                    {
                        listIpAddress.Add(st.IpAddress);
                        if (!string.IsNullOrEmpty(st.BackupIpAddress))
                            listIpAddress.Add(st.BackupIpAddress);
                    }
                }
            }

        }

        #endregion

        #region Listner / client accepting section
        private bool StartUpListner()
        {
            bool start = true;

            IPAddress resolvedIPAddress;
            IPAddress resolvedBackupIPAddress;

            if (String.IsNullOrEmpty(settings.TcpChannelSettingsHostName) || !UdpChannel.GetResolvedConnecionIPAddress(settings.TcpChannelSettingsHostName, out resolvedIPAddress))
                UdpChannel.GetResolvedConnecionIPAddress("localhost", out resolvedIPAddress);

            try
            {

                listener = new TcpListener(new IPEndPoint(resolvedIPAddress, settings.TcpChannelSettingsHostPort));
                listener.Start();
                beginAcceptTcpClientAlreadyDone = false;
                BeginAcceptTcpClient(listener, acceptTcpClientResult, ref beginAcceptTcpClientAlreadyDone);

                if (!String.IsNullOrEmpty(settings.TcpChannelSettingsBackupHostName) && UdpChannel.GetResolvedConnecionIPAddress(settings.TcpChannelSettingsBackupHostName, out resolvedBackupIPAddress))
                {
                    if (resolvedBackupIPAddress != resolvedIPAddress)
                    {
                        listenerBackup = new TcpListener(new IPEndPoint(resolvedBackupIPAddress, settings.TcpChannelSettingsHostPort));
                        listenerBackup.Start();
                        // Accept the connection.
                        // BeginAcceptSocket() creates the accepted socket.
                        beginAcceptTcpClientBackUpAlreadyDone = false;
                        BeginAcceptTcpClient(listenerBackup, acceptTcpClientBackUpResult, ref beginAcceptTcpClientBackUpAlreadyDone);
                    }
                }
            }
            catch (Exception ex)
            {
                CommDriver.OnSystemEvent(null, String.Format(Properties.Resources.ErrorStartingListener, settings.TcpChannelSettingsHostName, settings.TcpChannelSettingsHostPort, ex.Message), EventSeverity.Min);

                StopListner();

                start = false;
            }

            return start;
        }

        private void StopListner()
        {
            if (listener != null)
            {
                EndAcceptTcpClient(acceptTcpClientResult);

                listener.Stop();
                listener = null;
            }
            if (listenerBackup != null)
            {
                EndAcceptTcpClient(acceptTcpClientBackUpResult);
                listenerBackup.Stop();
                listenerBackup = null;
            }
        }


        // Process the client connection.
        private void DoAcceptTcpClientCallback(IAsyncResult ar)
        {
            try
            {
                // Get the listener that handles the client request.
                TcpListener locallistener = (TcpListener)ar.AsyncState;

                // End the operation and display the received data on
                // the console.
                TcpClient client = locallistener.EndAcceptTcpClient(ar);

                //client = _listener.AcceptTcpClient();
                clientID++;
                ModbusTCPSlaveChannel channel = new ModbusTCPSlaveChannel(CommDriver, settings, client, this, clientID);
                lock (lockListObject)
                    connections[clientID] = (channel as ModbusTCPSlaveChannel);
                channel.Terminated += Channel_Terminated;
                channel.Startup();

                if (locallistener == listener)
                {
                    beginAcceptTcpClientAlreadyDone = false;
                    BeginAcceptTcpClient(listener, acceptTcpClientResult, ref beginAcceptTcpClientAlreadyDone);
                }
                else
                {
                    beginAcceptTcpClientBackUpAlreadyDone = false;
                    BeginAcceptTcpClient(listenerBackup, acceptTcpClientBackUpResult, ref beginAcceptTcpClientBackUpAlreadyDone);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private bool BeginAcceptTcpClient(TcpListener listener, IAsyncResult acceptTcpClientResult, ref bool beginAcceptTcpClientAlreadyDone)
        {
            if (listener == null)
                return (false);

            if (beginAcceptTcpClientAlreadyDone == true)
            {
                return (true);
            }

            beginAcceptTcpClientAlreadyDone = true;
            EndAcceptTcpClient(acceptTcpClientResult);
            lock (lockStream)
            {
                try
                {
                    acceptTcpClientResult = listener.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback), listener);
                }
                catch
                {
                    beginAcceptTcpClientAlreadyDone = false;
                    //https://support.microsoft.com/en-us/kb/260018
                    EndAcceptTcpClient(acceptTcpClientResult);
                    return false;
                }
            }
            return (true);
        }

        private void EndAcceptTcpClient(IAsyncResult acceptTcpClientResult)
        {
            lock (lockStream)
            {
                if (acceptTcpClientResult != null)
                {
                    acceptTcpClientResult.AsyncWaitHandle.Close();
                    acceptTcpClientResult.AsyncWaitHandle.Dispose();
                    acceptTcpClientResult = null;
                }
            }
        }
        #endregion

        #region Server
        public override bool Startup()
        {
            if (bDisposed)
                return false;
            if (bChannelStarted)
                return true;

            InitTagsAndStations();
            
            bool start = StartUpListner();
            // report fatal error on channel putting in all tags in error
            if (!start)
                SetChannelInFatalErrorState();
               
            bChannelStarted = true;

            return start;        
        }

        public override void Suspend()
        {
            StopListner();

            StopClients();

            base.Suspend();
        }        
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
            StopListner();

            StopClients();

            base.Dispose();
        }
        #endregion

    }
}
