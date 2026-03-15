////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	BACnetChannel.cs
//
// summary:	Implements the driver BACnet channel class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using DriverCodeBase;
using IpDriverCodeBase;
using DriverCodeBase.Enumerators;
using System.Threading;
using System.Threading.Tasks;
using Opc.Ua;

using DevExpress.Xpo;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Xpo.DB;
using System.Xml;
using System.Windows.Forms;
using System.Windows;
using System.IO;

namespace BACnet
{

    /// <summary>   Communication channel of the BACnet driver. </summary>
    public class BACnetChannel : BackNetUdpChannel // UdpChannel
    {
        #region enum
        public enum WhoIsStates
        {
            BEGIN,
            PROCESS,
            END,
        }
        public enum WhoHasStates
        {
            BEGIN,
            SEND,
            ANSWER,
            END,
        }
        public enum CovNotificationStates
        {
            BEGIN,
            SEND,
            ANSWER,
            END,
        }
        public enum ReadWritePropertyStates
        {
            BEGIN,
            SEND,
            ANSWER_READ,
            ANSWER_WRITE,
            END,
        }
        public enum ReciveStates
        {
            Init,
            Wait,
        }
        #endregion

        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes the BACnetChannel object. </summary>
        ///
        /// <param name="commdriver">   . </param>
        /// <param name="settings">     . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public BACnetChannel(CommunicationDriver commdriver, BACnetChannelSettings settings)
            : base(commdriver, settings, false)
        {
            EnableBroadcast = true;
            DeviceInstance = settings.getDeviceInstance();
            DeviceHostName = settings.UdpChannelSettingsLocalHostName;
            this.AllowOtherBACnetClients = (commdriver as BACnetDriver).AllowOtherBACnetClients;
        }

        #endregion

        #region member
        Random rndGen;
        Dictionary<UInt32, List<BACnetCommJob>> mapSubscribeIDJob;
        Dictionary<string, uint> mapSubscribeID;
        List<BACnetStation> listStation;
        Queue<BACnetStation> WhoIsStationQueue;
        Queue<BACnetCommJob> WhoHasJobQueue;
        Queue<UInt32> CovNotificationSubscribeIDQueue;
        List<UInt32> CovNotificationSubscribeIDEnqueued;
        List<BACnetCommJob> ForceInitialPollingJob;
        WhoIsStates WhoIsState;
        WhoHasStates WhoHasState;
        CovNotificationStates CovNotificationState;
        ReciveStates ReciveState;
        ReadWritePropertyStates ReadWritePropertyState;
        DateTime WhoIsTxTime;
        DateTime WhoHasTxTime;
        DateTime CovNotificationTxTime;
        DateTime ReadWritePropertyTxTime;
        byte mInvokeId;
        BACnetCommJob WhoHasOnIdleJob;
        UInt32 CovNotificationOnIdleSubscribeID;
        BACnetCommJob ReadWritePropertyOnIdleJob;

        Dictionary<string, BACnetBBMDDevice> BBMDDevices = null;

        int DeviceInstance;
        string DeviceHostName;
        BACnetDeviceServer DeviceServer = null;
        //map of device's ip managed by channel --> use map for speed reason
        Dictionary<string, string> AllowedIp = null;

        //List<string> listDebug;
        #endregion

        #region Override Methods

        protected override void WorkingThread(object data)
        {
            int sleepCycle = WaitTime;
            if (sleepCycle == 0)
                sleepCycle = 1;
            ListJobPending.Clear();
            ListJobExecuted.Clear();

            NextScheduleTimeJobsList = DateTime.UtcNow;
            WhoIsTxTime = new DateTime();
            WhoHasTxTime = new DateTime();
            CovNotificationTxTime = new DateTime();
            ReadWritePropertyTxTime = new DateTime();
            WhoIsState = WhoIsStates.BEGIN;
            WhoHasState = WhoHasStates.BEGIN;
            CovNotificationState = CovNotificationStates.BEGIN;
            ReadWritePropertyState = ReadWritePropertyStates.BEGIN;
            ReciveState = ReciveStates.Init;
            rndGen = new Random((int)(DateTime.UtcNow.Ticks % int.MaxValue));
            mapSubscribeIDJob = new Dictionary<UInt32, List<BACnetCommJob>>();
            mapSubscribeID = new Dictionary<string, uint>();
            listStation = new List<BACnetStation>();
            WhoIsStationQueue = new Queue<BACnetStation>();
            WhoHasJobQueue = new Queue<BACnetCommJob>();
            CovNotificationSubscribeIDQueue = new Queue<UInt32>();
            CovNotificationSubscribeIDEnqueued = new List<UInt32>();
            ForceInitialPollingJob = new List<BACnetCommJob>();
            ReceiveItem receiveItem = new ReceiveItem(new BACnetIPFrame(), new IPEndPoint(0, 0), new DateTime());

            foreach (Station st in CommDriver.GetChannelStations(this))
                listStation.Add(st as BACnetStation);

            BBMDInit();

            DeviceServerInit();

            InitWhoIsStationlist(listStation);

            InitAllowedIP();

            while (true)
            {

                if (base.IsDeviceOpen())
                {
                    ReceiveLoop(ref receiveItem);
                    //#if DEBUG
                    //                    if (receiveItem.isValid)
                    //                        System.Diagnostics.Debug.WriteLine(string.Format("-------------------------BACnet {0},Func:{1},Type:{2}", DateTime.Now,receiveItem.Frame.Header.BVLCfunction.ToString(), receiveItem.Frame.Header.BVLCtype.ToString("0x{0:X}")));
                    //#endif

                    if (DeviceServer != null)
                        DeviceServerWhoIsIam(ref receiveItem);

                    if (!receiveItem.isUnMappedDevice)
                    {
                        //BBMD management
                        if (BBMDDevices != null)
                            BBMDRegisterAndKeepAliveSession(ref receiveItem);

                        ReadWritePropertyLoop(ref receiveItem);
                        ConsumeCovNotification(ref receiveItem);
                        CovNotificationLoop(ref receiveItem);
                        WdWhoIsLoop(ref receiveItem);
                        //WhoIsLoop(ref receiveItem);
                        WhoHasLoop(ref receiveItem);
                    }
                    receiveItem.isValid = false;

                    if (StopWorkerThread.WaitOne(0))
                        break;
                    bool isReceivePacketQueue;
                    lock (lockThreadObject)
                        isReceivePacketQueue = (ReceivePacketQueue.Count() > 0);
                    if (isReceivePacketQueue || NewDataToAnlyze.WaitOne(sleepCycle))
                    {
                        lock (lockThreadObject)
                            NewDataToAnlyze.Reset();
                    }
                }
                else
                {
                    if (StopWorkerThread.WaitOne(2000))
                        break;
                    base.DeviceOpen();
                }
            }

            BBMDTerminate();

            DeviceClose();
        }
        
        public bool DeviceWrite(BACnetIPFrame Frame, IPEndPoint RemoteEndPoint)
        {
            byte[] buffer = Frame.Pack();
            return base.DeviceWrite(buffer, (uint)buffer.Length, RemoteEndPoint);

        }
        public override bool DeviceWrite(byte[] Buffer, uint Count = 0)
        {
            if (Count == 0)
                return base.DeviceWrite(Buffer, (uint)Buffer.Length);
            else
                return base.DeviceWrite(Buffer, Count);

        }
        public override bool DeviceRead(byte[] Buffer, uint Count = 0)
        {
            if (Count == 0)
                return base.DeviceRead(Buffer, (uint)Buffer.Length);
            else
                return base.DeviceRead(Buffer, Count);
        }

        public override bool DeviceClose()
        {
            WhoIsState = WhoIsStates.BEGIN;
            EndDeviceRead();
            EndDeviceRead_Ex();
            return base.DeviceClose();
        }

        /*
         * serial methods rule...


        public override uint GetBytesToRead()
        {
            throw new NotImplementedException();
        }

        public override uint GetBytesToWrite()
        {
            throw new NotImplementedException();
        }
        */





        #endregion

        #region Properties

        //private bool _BBMDRegister;
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>    Register to BBMD as foreign </summary>
        /////
        ///// <value> This option force the driver to register with a BBMD device as foreign, with the aim of 
        /////         being addressed with the broadcast messages originated in the BBMD BACnet network. 
        /////         This is compulsory if the device belong to a TCP/IP network different from that of the driver, 
        /////         connected through a router. Routers stops all the global broadcast and this prevent a 
        /////         correct information exchange. </value>
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        //public bool BBMDRegister
        //{
        //    get
        //    {
        //        return _BBMDRegister;
        //    }
        //    set
        //    {
        //        _BBMDRegister = value;
        //    }
        //}

        //private uint _BBMDLifetime;
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>    BBMD registration lifetime (min.)  </summary>
        /////
        ///// <value> Duration in minutes of the registration as foreign device.
        /////              durations less then 15 minutes are not allowed </value>
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        //public uint BBMDLifetime
        //{
        //    get
        //    {
        //        return _BBMDLifetime;
        //    }
        //    set
        //    {
        //        _BBMDLifetime = value;
        //    }
        //}

        //private string _BBMDAddress;
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>   BBMD IP address. </summary>
        /////
        ///// <value> IP address of the BBMD device. If it is left void, the main BACnet device IP is used. 
        /////         </value>
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        //public string BBMDAddress
        //{
        //    get
        //    {
        //        return _BBMDAddress;
        //    }
        //    set
        //    {
        //        _BBMDAddress = value;
        //    }
        //}

        #endregion

        #region methods

        public static IPAddress GetBroadcastAddress(IPAddress address)
        {
            IPAddress subnetMask = new IPAddress(ReturnSubnetmask(address));

            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));
            }
            return new IPAddress(broadcastAddress);
        }

        static public long ReturnSubnetmask(IPAddress ipaddress)
        {
            return 0xFFFFFF;
        }

        static public uint ReturnFirtsOctet(IPAddress iPAddress)
        {
            byte[] byteIP = iPAddress.GetAddressBytes();
            uint ipInUint = (uint)byteIP[0];
            return ipInUint;
        }


        byte getInvokeID()
        {
            return mInvokeId++;
        }

        private void BACnetRead(ref ReceiveItem receiveItem)
        {
            receiveItem.isValid = false;
            receiveItem.isUnMappedDevice = false;

            ReceivedPacket packet;
            lock (lockThreadObject)
            {
                if (ReceivePacketQueue.Count() > 0)
                {
                    //System.Diagnostics.Debug.WriteLine("{0} start receive  ", DateTime.Now.ToString("HH:mm:ss.fff"));
                    packet = ReceivePacketQueue.Dequeue();
                }
                else
                    return;
            }
            if (packet.Data.Count() < BACnetIPFrameHeader.Size)
                return;

            receiveItem.TimeStamp = packet.TimeStamp;
            receiveItem.RemoteEndPoint = packet.RemoteEndPoint;

            int Offset = 0;
            BACnetIPFrameHeader Header = new BACnetIPFrameHeader();
            try
            {
                Header.Expand(ref packet.Data, ref Offset);
            }
            catch (NotImplementedException notImp)
            {
                //listDebug.Add(string.Format("{0} ReceiveBuffer.Clear() 1",
                //    DateTime.Now.ToString("HH:mm:ss.fff")
                //    ));
                return;
            }
            if (Header.BVLCtype != BACnetEnums.BVLC_TYPE_BIP)
            {
                //listDebug.Add(string.Format("{0} ReceiveBuffer.Clear() 2",
                //    DateTime.Now.ToString("HH:mm:ss.fff")
                //    ));
                return;
            }
            if (Header.FrameSize > 0)
            {
                if (packet.Data.Length < BACnetIPFrameHeader.Size + Header.FrameSize)
                    return;
                byte[] bufferTmp = new byte[BACnetIPFrameHeader.Size + Header.FrameSize];
                Array.Copy(packet.Data, bufferTmp, BACnetIPFrameHeader.Size + Header.FrameSize);
                packet.Data = bufferTmp;
                Offset = 0;
                try
                {
                    receiveItem.Frame.Expand(ref packet.Data, ref Offset);
                    if (receiveItem.Frame.Header.BVLCfunction == BACnetEnums.BVLC_Functions.ForwardedNPDU)
                    {
                        // a BVLC_FORWARDED_NPDU frame by a BBMD, change the remote_address to the original one (stored in the BVLC header) 
                        // we don't care about the BBMD address
                        receiveItem.RemoteEndPoint = receiveItem.Frame.Header.OriginalEndPoint;
                    }
                }
                catch (NotImplementedException notImp)
                {
                    //listDebug.Add(string.Format("{0} ReceiveBuffer.Clear() 3",
                    //    DateTime.Now.ToString("HH:mm:ss.fff")
                    //    ));
                    return;
                }
            }
            
            receiveItem.isValid = true;
            //check if message is direct to one of channel's stations; broadcast (.255) always allowed
            if (receiveItem.RemoteEndPoint != null)
            {
                IPAddress ad = receiveItem.RemoteEndPoint.Address;
                byte LastAddressByte = receiveItem.RemoteEndPoint.Address.GetAddressBytes()[3];
                if (LastAddressByte != 255 && !AllowedIp.ContainsKey(ad.ToString()))
                {                    
                    receiveItem.isUnMappedDevice = true;
                    #if DEBUG
                        System.Diagnostics.Debug.WriteLine(string.Format("-------------------------BACnet {0},Message with IP Address {1} don't match stations on channel {2}", DateTime.Now, ad, this.Name));
#endif
                }                
            }
#if DEBUG
            System.Diagnostics.Debug.WriteLine(string.Format("-------------------------BACnet {0},Func:{1},Type:0x{2},Value:{3}", DateTime.Now, receiveItem.Frame.Header.BVLCfunction.ToString(), receiveItem.Frame.Header.BVLCtype.ToString("X"), string.Join(",", packet.Data.Select(b => b.ToString("X2")))));
#endif
            return;
        }

        void RemoveCovSubscritionIDsForStation(BACnetStation station)
        {
            if ((CovNotificationSubscribeIDQueue == null) || (CovNotificationSubscribeIDEnqueued == null))
            {
                return;
            }

            System.Diagnostics.Debug.WriteLine("{0} RemoveCovSubscritionIDsForStation {1} - Init, {2}, {3}", DateTime.Now.ToString("HH:mm:ss.fff"), station.Name,
                CovNotificationSubscribeIDEnqueued.Count, CovNotificationSubscribeIDQueue.Count);

            if (CovNotificationSubscribeIDEnqueued.Count > 0)
            {
                object synchObject = new object();
                List<UInt32> tmpCovNotificationSubscribeIDEnqueued = new List<UInt32>();
                Parallel.ForEach(CovNotificationSubscribeIDEnqueued, id =>
                {
                    if (mapSubscribeIDJob.ContainsKey(id) &&
                        mapSubscribeIDJob[id].Count() != 0)
                    {
                        BACnetStation correspondingStation = mapSubscribeIDJob[id][0].Station as BACnetStation;
                        if (correspondingStation != station)
                        {
                            lock (synchObject)
                            {
                                tmpCovNotificationSubscribeIDEnqueued.Add(id);
                            }
                        }
                    }
                });
                CovNotificationSubscribeIDEnqueued.Clear();
                if (tmpCovNotificationSubscribeIDEnqueued.Count > 0)
                {
                    CovNotificationSubscribeIDEnqueued.AddRange(tmpCovNotificationSubscribeIDEnqueued);
                    tmpCovNotificationSubscribeIDEnqueued.Clear();
                }
            }

            if (CovNotificationSubscribeIDQueue.Count > 0)
            {
                object synchObject = new object();
                Queue<UInt32> tmpCovNotificationSubscribeIDQueue = new Queue<UInt32>();
                Parallel.ForEach(CovNotificationSubscribeIDQueue, id =>
                {
                    if (mapSubscribeIDJob.ContainsKey(id) &&
                        mapSubscribeIDJob[id].Count() != 0)
                    {
                        BACnetStation correspondingStation = mapSubscribeIDJob[id][0].Station as BACnetStation;
                        if (correspondingStation != station)
                        {
                            lock (synchObject)
                            {
                                tmpCovNotificationSubscribeIDQueue.Enqueue(id);
                            }
                        }
                    }
                });
                CovNotificationSubscribeIDQueue.Clear();
                if (tmpCovNotificationSubscribeIDQueue.Count > 0)
                {
                    foreach (UInt32 id in tmpCovNotificationSubscribeIDQueue)
                    {
                        CovNotificationSubscribeIDQueue.Enqueue(id);
                    }
                    tmpCovNotificationSubscribeIDQueue.Clear();
                }
            }

            System.Diagnostics.Debug.WriteLine("{0} RemoveCovSubscritionIDsForStation {1} - End, {2}, {3}", DateTime.Now.ToString("HH:mm:ss.fff"), station.Name,
                CovNotificationSubscribeIDEnqueued.Count, CovNotificationSubscribeIDQueue.Count);
        }


        private void InitStationWithNoWhoIsData(BACnetStation station)
        {
            BACnetObjectIdentifier DeviceIdentifier = new BACnetObjectIdentifier(BACnetEnums.ObjectTypes.DEVICE, (uint)station.getDeviceInstance());
            InitStationWithWhoIsData(station, DeviceIdentifier, 0, 0, false, 0, 0, new byte[0]);
            //InitStationWithWhoIsData(station, DeviceIdentifier, 0, 0, true, 20, 6, station.getDeviceInstanceToADR());
        }

        private void InitStationWithWhoIsData(BACnetStation station, BACnetObjectIdentifier deviceIdentifier, UInt32 maxAPDULength, UInt16 vendorIdentifier, bool destinationSpecifierPresent, UInt16 dnet, byte dlen, byte[] dadr)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("-------------------------BACnet {0},Station:{1},Message WhoIs Reviced fr", DateTime.Now, station.RemoteEndPoint));

            //Report Who-IS success to Movicon for diagnostic 
            CommDriver.OnSystemEvent(null, String.Format(Properties.Resources.ErrorWhoHisSucess, station.Name), EventSeverity.Low);

            station.WhoIsDelayMs = Properties.Settings.Default.WhoIsRepeatDelaySlow;
            //Setup station, subscribe job, do WhoHas
            station.LastErrorCode = DriverErrorCodes.ErrorNoError;
            station.InErrorState = false;
            station.BACnetInitDone = true;
            station.DeviceIdentifier = deviceIdentifier;
            station.MaxAPDULength = maxAPDULength;
            station.SegmentationSupported = 0;
            station.VendorIdentifier = vendorIdentifier;
            station.DestinationSpecifierPresent = destinationSpecifierPresent;
            station.DNET = dnet;
            station.DLEN = dlen;
            station.DADR = dadr;
            station.WhoIsError = false;

            if (station.TimeSync != TimeSyncs.None)
            {
                BACnetIPFrame tymeSync = station.TimeSync == TimeSyncs.System_Time ?
                    IPFrameFactory.TimeSynch(station) : IPFrameFactory.UtcTimeSynch(station);
                DeviceWrite(tymeSync, station.RemoteEndPoint);
                StopWorkerThread.WaitOne(20);
            }

            foreach (var enqElem in station.GetListWholeJobCopy())
            {
                var job = enqElem as BACnetCommJob;
                if (job.COVEnable &&
                    BACnetEnums.isCovSupported(job.BACnetObjectType.ToString(), job.PropertyIdentifier.ToString()))
                {
                    if (!station.mapObjectSubscribeID.ContainsKey(job.ObjectFullName))
                    {
                        //job.SubscriberID = getSubscriberID();
                        AssignsSubscriberID(station, ref job);
                        mapSubscribeIDJob[job.SubscriberID] = new List<BACnetCommJob>();
                        mapSubscribeIDJob[job.SubscriberID].Add(job);
                        station.mapObjectSubscribeID[job.ObjectFullName] = job.SubscriberID;
                    }
                    else if (!mapSubscribeIDJob[station.mapObjectSubscribeID[job.ObjectFullName]].Contains(job))
                    {
                        job.SubscriberID = station.mapObjectSubscribeID[job.ObjectFullName];
                        mapSubscribeIDJob[job.SubscriberID].Add(job);
                    }
                }
                //subscribe
                List<Tag> tl = (from t in job.TagsList
                                where (t.DynSettings.MethodID != -1)
                                select t).ToList();
                if (tl.Count != enqElem.TagsList.Count)
                    SubscribeJob(job, (job.InUse ? CommJobState.PollingInUse : CommJobState.PollingNotInUse));

                //when WhoIsEnabled=false, BadTimeout quality is used to check if station is connected --> need to be resetted each time 
                //if (station.WhoIsDisabled)
                //    job.SetUncertainQuality();

                job.lastWhoHasTestTime = new DateTime(DateTime.UtcNow.Ticks - (Properties.Settings.Default.WhoIsRepeatDelay - Properties.Settings.Default.WaitWhoIs) * 10000);
                WhoHasJobQueue.Enqueue(job);
            }
            //if you want to repeat Who-Has, invalidate ObjectID in the station dictionary mapObjectID!!!
            station.mapObjectID.Clear();//trigger new Who-Has
                                        //reset queue for COV notification, after Who-Has
            station.mapSubscribeTime.Clear();
            //CovNotificationSubscribeIDEnqueued.Clear();
            //CovNotificationSubscribeIDQueue.Clear();
            RemoveCovSubscritionIDsForStation(station);
            System.Diagnostics.Debug.WriteLine("{0} WdWhoIsLoop Who-Is successful on Station {1}, CovNotification queues cleared", DateTime.Now.ToString("HH:mm:ss.fff"), station.Name);
        }

        DateTime dtWhoIsNextTime;
        int currStationIndex = -1;
        bool bWhoIsQuickLoop = true;
        List<BACnetStation> listWhoIsSent = new List<BACnetStation>();
        private BACnetErrorCodes WdWhoIsLoop(ref ReceiveItem receiveItem)
        {
            //listStation doesn't change!!!
            BACnetErrorCodes result = (BACnetErrorCodes)DriverErrorCodes.ErrorNoError;
            if (receiveItem.isValid || DateTime.UtcNow > dtWhoIsNextTime)
            {
                if (++currStationIndex >= listStation.Count())
                {
                    bWhoIsQuickLoop = false;
                    currStationIndex = 0;
                }
                BACnetStation TmpStation = listStation[currStationIndex];
                if (TmpStation.WhoIsDisabled)
                {
                    //WhoIs disabled is used to exhange data with device outside network without the use of BBMD; active station (no message to check station presence)
                    if (!TmpStation.BACnetInitDone)
                        InitStationWithNoWhoIsData(TmpStation);

                    TmpStation = null;
                }
                if (BBMDDevices != null)
                {
                    if (TmpStation.BBMDRegister && !TmpStation.IsBBMDRegistred)
                        TmpStation = null;
                }

                //some time has passed, since the last WhoIs...
                if (TmpStation != null && !listWhoIsSent.Contains(TmpStation) && (DateTime.UtcNow - TmpStation.WhoIsTxTime).TotalMilliseconds > TmpStation.WhoIsDelayMs)
                {
                    // try to add station with 'valid ip' only if was not resolved (by dns) before
                    TryToAddGoodStationToAllowedIP(TmpStation);
                    IPEndPoint re = TmpStation.RemoteEndPoint;
                    if (re != null && !DeviceWrite(IPFrameFactory.WhoIs(TmpStation), new IPEndPoint(re.Address, re.Port)))
                    {
                        result = BACnetErrorCodes.ErrorTxWrite;
                    }
                    TmpStation.WhoIsTxTime = DateTime.UtcNow;
                    listWhoIsSent.Add(TmpStation);
                }

                if (listWhoIsSent.Count > 0 && receiveItem.isValid)
                {
                    IPEndPoint ipReceived = new IPEndPoint(receiveItem.RemoteEndPoint.Address, receiveItem.RemoteEndPoint.Port);
                    //got answer for some WhoIs?
                    BACnetObjectIdentifier DeviceIdentifier = new BACnetObjectIdentifier();
                    UInt32 MaxAPDULength = 0;
                    uint SegmentationSupported = 0;
                    UInt16 VendorIdentifier = 0;
                    bool DestinationSpecifierPresent = false;
                    UInt16 DNET = 0;
                    byte DLEN = 0;
                    byte[] DADR = new byte[0];
                    if (IPFrameValidator.IAm(receiveItem.Frame, ref DeviceIdentifier, ref MaxAPDULength,
                                    ref SegmentationSupported, ref VendorIdentifier,
                                    ref DestinationSpecifierPresent, ref DNET, ref DLEN, ref DADR))
                    {
                        receiveItem.isValid = false;//already consumed

                        var lStations = (from s in listWhoIsSent where (s.getDeviceInstance() == DeviceIdentifier.instance || s.getDeviceInstance() < 0) && ipReceived.Equals(s.RemoteEndPoint) select s).ToList();
                        if (lStations.Count > 0)
                        {
                            //if alredy initialized, periodic Who Is succesful. No need to redo WhoHas and station  settings
                            if (!lStations[0].BACnetInitDone)
                            {
                                InitStationWithWhoIsData(lStations[0], DeviceIdentifier, MaxAPDULength, VendorIdentifier, DestinationSpecifierPresent, DNET, DLEN, DADR);
                                ////////////System.Diagnostics.Debug.WriteLine(string.Format("-------------------------BACnet {0},Station:{1},Message WhoIs Reviced fr", DateTime.Now, lStations[0].RemoteEndPoint));

                                ////////////lStations[0].WhoIsDelayMs = Properties.Settings.Default.WhoIsRepeatDelaySlow;
                                //////////////Setup station, subscribe job, do WhoHas
                                ////////////lStations[0].LastErrorCode = DriverErrorCodes.ErrorNoError;
                                ////////////lStations[0].InErrorState = false;
                                ////////////lStations[0].BACnetInitDone = true;
                                ////////////lStations[0].DeviceIdentifier = DeviceIdentifier;
                                ////////////lStations[0].MaxAPDULength = MaxAPDULength;
                                ////////////lStations[0].SegmentationSupported = 0;
                                ////////////lStations[0].VendorIdentifier = VendorIdentifier;
                                ////////////lStations[0].DestinationSpecifierPresent = DestinationSpecifierPresent;
                                ////////////lStations[0].DNET = DNET;
                                ////////////lStations[0].DLEN = DLEN;
                                ////////////lStations[0].DADR = DADR;
                                ////////////lStations[0].WhoIsError = false;
                                ////////////if (lStations[0].TimeSync != TimeSyncs.None)
                                ////////////{
                                ////////////    BACnetIPFrame tymeSync = lStations[0].TimeSync == TimeSyncs.System_Time ?
                                ////////////        IPFrameFactory.TimeSynch(lStations[0]) : IPFrameFactory.UtcTimeSynch(lStations[0]);
                                ////////////    DeviceWrite(tymeSync, lStations[0].RemoteEndPoint);
                                ////////////    StopWorkerThread.WaitOne(20);
                                ////////////}

                                ////////////foreach(var enqElem in lStations[0].GetListWholeJobCopy())
                                ////////////{
                                ////////////    var job = enqElem as BACnetCommJob; 
                                ////////////    if (job.COVEnable &&
                                ////////////        BACnetEnums.isCovSupported(job.BACnetObjectType.ToString(), job.PropertyIdentifier.ToString()))
                                ////////////    {
                                ////////////        if (!lStations[0].mapObjectSubscribeID.ContainsKey(job.ObjectName))
                                ////////////        {
                                ////////////            //job.SubscriberID = getSubscriberID();
                                ////////////            AssignsSubscriberID(lStations[0], ref job);
                                ////////////            mapSubscribeIDJob[job.SubscriberID] = new List<BACnetCommJob>();
                                ////////////            mapSubscribeIDJob[job.SubscriberID].Add(job);
                                ////////////            lStations[0].mapObjectSubscribeID[job.ObjectName] = job.SubscriberID;
                                ////////////        }
                                ////////////        else if (!mapSubscribeIDJob[lStations[0].mapObjectSubscribeID[job.ObjectName]].Contains(job))
                                ////////////        {
                                ////////////            job.SubscriberID = lStations[0].mapObjectSubscribeID[job.ObjectName];
                                ////////////            mapSubscribeIDJob[job.SubscriberID].Add(job);
                                ////////////        }
                                ////////////    }
                                ////////////    //subscribe
                                ////////////    List<Tag> tl = (from t in job.TagsList
                                ////////////                    where (t.DynSettings.MethodID != -1)
                                ////////////                    select t).ToList();
                                ////////////    if (tl.Count != enqElem.TagsList.Count)
                                ////////////        SubscribeJob(job, (job.InUse ? CommJobState.PollingInUse : CommJobState.PollingNotInUse));

                                ////////////    job.lastWhoHasTestTime = new DateTime(DateTime.UtcNow.Ticks - (Properties.Settings.Default.WhoIsRepeatDelay - Properties.Settings.Default.WaitWhoIs) * 10000);
                                ////////////    WhoHasJobQueue.Enqueue(job);
                                ////////////}
                                //////////////if you want to repeat Who-Has, invalidate ObjectID in the station dictionary mapObjectID!!!
                                ////////////lStations[0].mapObjectID.Clear();//trigger new Who-Has
                                //////////////reset queue for COV notification, after Who-Has
                                ////////////lStations[0].mapSubscribeTime.Clear();
                                //////////////CovNotificationSubscribeIDEnqueued.Clear();
                                //////////////CovNotificationSubscribeIDQueue.Clear();
                                ////////////RemoveCovSubscritionIDsForStation(lStations[0]);
                                ////////////System.Diagnostics.Debug.WriteLine("{0} WdWhoIsLoop Who-Is successful on Station {1}, CovNotification queues cleared", DateTime.Now.ToString("HH:mm:ss.fff"), lStations[0].Name);
                            }
                            listWhoIsSent.Remove(lStations[0]);
                        }
                    }
                }

                if (listWhoIsSent.Count > 0)
                {
                    //check timeout
                    var elaspedWhoIs = (from s in listWhoIsSent where (DateTime.UtcNow - s.WhoIsTxTime).TotalMilliseconds > Timeout select s).ToList();
                    if (elaspedWhoIs.Count > 0)
                    {
                        //WhoIs not received. Remove job's subscription, 
                        //set job quality not connected for the one not in error. 
                        //Reset the last WhoIs time for the station, in order to redo quickly the WhoIs request 
                        //station is set as not initialized
                        //foreach(var toStation in elaspedWhoIs)
                        var toStation = elaspedWhoIs[0];

                        var listJobs = toStation.GetListWholeJobCopy();
                        foreach (var elJob in listJobs)
                        {
                            var job = elJob as BACnetCommJob;
                            UnsubscribeJob(job);
                            if (!job.InErrorState)
                                job.SetBadConnectionQuality();
                            job.BACnetInitDone = false;
                        }

                        toStation.WhoIsTxTime = DateTime.UtcNow;
                        toStation.BACnetInitDone = false;
                        toStation.mapSubscribeTime.Clear();
                        toStation.mapObjectID.Clear();
                        listWhoIsSent.Remove(toStation);
                        System.Diagnostics.Debug.WriteLine("{0} Who-Is failed. Station {1} freezed.", DateTime.Now.ToString("HH:mm:ss.fff"), toStation.Name);
                        if (toStation.WhoIsError == false)
                        {
                            toStation.WhoIsError = true;
                            string errorMessage = String.Format(Properties.Resources.ErrorWhoIsFailedForStation, toStation.Name);
                            CommDriver.OnSystemEvent(null, errorMessage, EventSeverity.Low);
                            toStation.SetStateCommandVariableBit(true, (UInt16)StationVariableBits.StationErrorState);
                            toStation.LastErrorCode = (DriverErrorCodes)BACnetErrorCodes.ErrorWhoHisFailed;
                        }
                        if(toStation.InErrorState == false)
                        {
                            toStation.InErrorState = true;
                        }

                        if (BBMDDevices != null)
                            BBMDCheckLinkedStationStatus(toStation);
                    }
                }
                dtWhoIsNextTime = DateTime.UtcNow.AddMilliseconds(bWhoIsQuickLoop ? Properties.Settings.Default.WaitWhoIsQuick : Properties.Settings.Default.WaitWhoIs);
            }

            return result;
        }
        private void InitWhoIsStationlist(List<BACnetStation> list)
        {
            foreach (var enqStation in list)
            {
                if (enqStation.RemoteEndPoint != null)
                {
                    WhoIsStationQueue.Enqueue(enqStation);
                    enqStation.BACnetInitDone = false;
                    enqStation.mapObjectID.Clear();
                    enqStation.mapObjectSubscribeID.Clear();
                    Parallel.ForEach(enqStation.GetListWholeJobCopy(), job =>
                    {
                        var enqJob = job as BACnetCommJob;
                        enqJob.BACnetInitDone = false;
                        enqJob.SubscriberID = 0;
                    });
                }
            }
        }

        /// <summary>   WhoIsLoop . </summary>
        private BACnetErrorCodes WhoIsLoop(ref ReceiveItem receiveItem)
        {
            BACnetErrorCodes Error = (BACnetErrorCodes)DriverErrorCodes.ErrorNoError;
            switch (WhoIsState)
            {
                case WhoIsStates.BEGIN:
                    WhoIsState = WhoIsStates.PROCESS;
                    WhoIsTxTime = DateTime.MinValue;
                    foreach (BACnetStation enqStation in listStation)
                        if (enqStation.RemoteEndPoint != null)
                        {
                            WhoIsStationQueue.Enqueue(enqStation);
                            enqStation.BACnetInitDone = false;
                            enqStation.mapObjectID.Clear();
                            enqStation.mapObjectSubscribeID.Clear();
                            foreach (BACnetCommJob enqJob in enqStation.getListWholeJob)
                            {
                                enqJob.BACnetInitDone = false;
                                enqJob.SubscriberID = 0;
                            }
                        }
                    break;

                case WhoIsStates.PROCESS:
                    if (WhoIsStationQueue.Count() != 0)
                    {
                        if ((DateTime.UtcNow - WhoIsTxTime).TotalMilliseconds > BACnetEnums.WaitWhoIs)
                        {
                            BACnetStation TmpStation = WhoIsStationQueue.Dequeue();
                            WhoIsStationQueue.Enqueue(TmpStation);
                            if (!TmpStation.BACnetInitDone && (DateTime.UtcNow - TmpStation.WhoIsTxTime).TotalMilliseconds > BACnetEnums.repeatDelay)
                            {

                                IPEndPoint WhoIsBroadcastEndPoint = new IPEndPoint(GetBroadcastAddress(TmpStation.RemoteEndPoint.Address), TmpStation.RemoteEndPoint.Port);
                                for (int cnt = WhoIsStationQueue.Count(); cnt > 0; cnt--)
                                {
                                    TmpStation = WhoIsStationQueue.Dequeue();
                                    WhoIsStationQueue.Enqueue(TmpStation);
                                    if (TmpStation.WhoIsTxTime != DateTime.MinValue)
                                    {
                                        TmpStation.LastErrorCode = (DriverErrorCodes)BACnetErrorCodes.ErrorWhoHisFailed;
                                        TmpStation.InErrorState = true;
                                    }
                                    if (WhoIsBroadcastEndPoint.Equals(new IPEndPoint(GetBroadcastAddress(TmpStation.RemoteEndPoint.Address), TmpStation.RemoteEndPoint.Port)))
                                        TmpStation.WhoIsTxTime = DateTime.UtcNow;
                                }
                                if (!DeviceWrite(IPFrameFactory.WhoIs(TmpStation), WhoIsBroadcastEndPoint))
                                {
                                    Error = BACnetErrorCodes.ErrorTxWrite;
                                }
                                else
                                {
                                    WhoIsTxTime = DateTime.UtcNow;
                                }
                                //System.Diagnostics.Debug.WriteLine("{0} DeviceWrite WhoIs", DateTime.Now.ToString("HH:mm:ss.fff"));
                            }
                        }

                        if (receiveItem.isValid)
                        {
                            BACnetObjectIdentifier DeviceIdentifier = new BACnetObjectIdentifier();
                            UInt32 MaxAPDULength = 0;
                            uint SegmentationSupported = 0;
                            UInt16 VendorIdentifier = 0;
                            bool DestinationSpecifierPresent = false;
                            UInt16 DNET = 0;
                            byte DLEN = 0;
                            byte[] DADR = new byte[0];

                            if (IPFrameValidator.IAm(receiveItem.Frame, ref DeviceIdentifier, ref MaxAPDULength,
                                ref SegmentationSupported, ref VendorIdentifier,
                                ref DestinationSpecifierPresent, ref DNET, ref DLEN, ref DADR))
                            {
                                BACnetStation WhoIsOnIdleStation = null;

                                for (int cnt = WhoIsStationQueue.Count(); cnt > 0; cnt--)
                                {
                                    WhoIsOnIdleStation = WhoIsStationQueue.Dequeue();
                                    if (WhoIsOnIdleStation.getDeviceInstance() > 0 &&
                                        receiveItem.RemoteEndPoint.Equals(WhoIsOnIdleStation.RemoteEndPoint) &&
                                        DeviceIdentifier.instance == WhoIsOnIdleStation.getDeviceInstance())
                                    {
                                        cnt = 0;
                                    }
                                    else
                                    {
                                        WhoIsStationQueue.Enqueue(WhoIsOnIdleStation);
                                        WhoIsOnIdleStation = null;
                                    }

                                }


                                if (WhoIsOnIdleStation == null)
                                {
                                    for (int cnt = WhoIsStationQueue.Count(); cnt > 0; cnt--)
                                    {
                                        WhoIsOnIdleStation = WhoIsStationQueue.Dequeue();
                                        if (WhoIsOnIdleStation.getDeviceInstance() < 0 &&
                                            receiveItem.RemoteEndPoint.Equals(WhoIsOnIdleStation.RemoteEndPoint))
                                        {
                                            cnt = 0;
                                        }
                                        else
                                        {
                                            WhoIsStationQueue.Enqueue(WhoIsOnIdleStation);
                                            WhoIsOnIdleStation = null;
                                        }

                                    }

                                }
                                if (WhoIsOnIdleStation != null)
                                {
                                    WhoIsOnIdleStation.LastErrorCode = DriverErrorCodes.ErrorNoError;
                                    WhoIsOnIdleStation.InErrorState = false;
                                    WhoIsTxTime = DateTime.UtcNow;
                                    WhoIsOnIdleStation.BACnetInitDone = true;
                                    WhoIsOnIdleStation.DeviceIdentifier = DeviceIdentifier;
                                    WhoIsOnIdleStation.MaxAPDULength = MaxAPDULength;
                                    WhoIsOnIdleStation.SegmentationSupported = 0;
                                    WhoIsOnIdleStation.VendorIdentifier = VendorIdentifier;
                                    WhoIsOnIdleStation.DestinationSpecifierPresent = DestinationSpecifierPresent;
                                    WhoIsOnIdleStation.DNET = DNET;
                                    WhoIsOnIdleStation.DLEN = DLEN;
                                    WhoIsOnIdleStation.DADR = DADR;
                                    if (WhoIsOnIdleStation.TimeSync != TimeSyncs.None)
                                    {
                                        BACnetIPFrame tymeSync = WhoIsOnIdleStation.TimeSync == TimeSyncs.System_Time ?
                                            IPFrameFactory.TimeSynch(WhoIsOnIdleStation) : IPFrameFactory.UtcTimeSynch(WhoIsOnIdleStation);
                                        DeviceWrite(tymeSync, WhoIsOnIdleStation.RemoteEndPoint);
                                        StopWorkerThread.WaitOne(20);
                                    }
                                    foreach (BACnetCommJob enqElem in WhoIsOnIdleStation.getListWholeJob)
                                    {
                                        if (enqElem.COVEnable &&
                                            BACnetEnums.isCovSupported(enqElem.BACnetObjectType.ToString(), enqElem.PropertyIdentifier.ToString()))
                                        {
                                            if (!WhoIsOnIdleStation.mapObjectSubscribeID.ContainsKey(enqElem.ObjectFullName))
                                            {
                                                enqElem.SubscriberID = getSubscriberID();
                                                mapSubscribeIDJob[enqElem.SubscriberID] = new List<BACnetCommJob>();
                                                mapSubscribeIDJob[enqElem.SubscriberID].Add(enqElem);
                                                WhoIsOnIdleStation.mapObjectSubscribeID[enqElem.ObjectFullName] = enqElem.SubscriberID;
                                            }
                                            else if (!mapSubscribeIDJob[WhoIsOnIdleStation.mapObjectSubscribeID[enqElem.ObjectFullName]].Contains(enqElem))
                                            {
                                                enqElem.SubscriberID = WhoIsOnIdleStation.mapObjectSubscribeID[enqElem.ObjectFullName];
                                                mapSubscribeIDJob[enqElem.SubscriberID].Add(enqElem);
                                            }
                                        }
                                        enqElem.lastWhoHasTestTime = new DateTime(DateTime.UtcNow.Ticks - (BACnetEnums.repeatDelay - BACnetEnums.WaitWhoIs) * 10000);
                                        WhoHasJobQueue.Enqueue(enqElem);
                                    }
                                }

                                receiveItem.isValid = false;
                            }
                        }
                    }
                    else
                        WhoIsState = WhoIsStates.END;
                    break;

                case WhoIsStates.END:
                    break;
            }


            return Error;
        }


        /// <summary>   WhoHaSLoop . </summary>
        private BACnetErrorCodes WhoHasLoop(ref ReceiveItem receiveItem)
        {
            BACnetErrorCodes Error = (BACnetErrorCodes)DriverErrorCodes.ErrorNoError;

            switch (WhoHasState)
            {
                case WhoHasStates.BEGIN:
                    if (WhoHasJobQueue.Count() != 0)
                        WhoHasState = WhoHasStates.SEND;
                    break;
                case WhoHasStates.SEND:
                    if (WhoHasJobQueue.Count() != 0)
                    {
                        WhoHasOnIdleJob = WhoHasJobQueue.Dequeue();
                        BACnetObjectIdentifier ObjectIdentifier = (BACnetObjectIdentifier)WhoHasOnIdleJob.ObjectID.Clone();

                        if ((DateTime.UtcNow - WhoHasOnIdleJob.lastWhoHasTestTime).TotalMilliseconds < BACnetEnums.repeatDelay ||
                            !(WhoHasOnIdleJob.Station as BACnetStation).BACnetInitDone)
                            WhoHasJobQueue.Enqueue(WhoHasOnIdleJob);
                        else
                        {
                            if (WhoHasOnIdleJob.InstanceNumber >= 0)
                            {
                                if ((WhoHasOnIdleJob.Station as BACnetStation).mapObjectID.TryGetValue(WhoHasOnIdleJob.ObjectFullName, out ObjectIdentifier)) // WhoHasOnIdleJob.ObjectID))
                                {
                                    WhoHasOnIdleJob.ObjectID = ObjectIdentifier;
                                    WhoHasOnIdleJob.BACnetInitDone = true;
                                    if (WhoHasOnIdleJob.COVEnable &&
                                        BACnetEnums.isCovSupported(WhoHasOnIdleJob.BACnetObjectType.ToString(), WhoHasOnIdleJob.PropertyIdentifier.ToString()) &&
                                        !CovNotificationSubscribeIDEnqueued.Contains(WhoHasOnIdleJob.SubscriberID))
                                    {
                                        CovNotificationSubscribeIDEnqueued.Add(WhoHasOnIdleJob.SubscriberID);
                                        CovNotificationSubscribeIDQueue.Enqueue(WhoHasOnIdleJob.SubscriberID);
                                        System.Diagnostics.Debug.WriteLine("{0} WhoHasLoop WhoHasStates.SEND ID {1} added to CovNotification queues", DateTime.Now.ToString("HH:mm:ss.fff"), WhoHasOnIdleJob.SubscriberID);
                                    }
                                }
                                else
                                {
                                    if (ObjectIdentifier == null)
                                        WhoHasOnIdleJob.ObjectID = new BACnetObjectIdentifier(WhoHasOnIdleJob.BACnetObjectType, (uint)WhoHasOnIdleJob.InstanceNumber);
                                    else
                                        WhoHasOnIdleJob.ObjectID = ObjectIdentifier;

                                    (WhoHasOnIdleJob.Station as BACnetStation).mapObjectID[WhoHasOnIdleJob.ObjectFullName] = WhoHasOnIdleJob.ObjectID;
                                    WhoHasOnIdleJob.BACnetInitDone = true;
                                    if (WhoHasOnIdleJob.SubscriberID != 0 && WhoHasOnIdleJob.COVEnable &&
                                        BACnetEnums.isCovSupported(WhoHasOnIdleJob.BACnetObjectType.ToString(), WhoHasOnIdleJob.PropertyIdentifier.ToString()) &&
                                        !CovNotificationSubscribeIDEnqueued.Contains(WhoHasOnIdleJob.SubscriberID))
                                    {
                                        CovNotificationSubscribeIDEnqueued.Add(WhoHasOnIdleJob.SubscriberID);
                                        CovNotificationSubscribeIDQueue.Enqueue(WhoHasOnIdleJob.SubscriberID);
                                        System.Diagnostics.Debug.WriteLine("{0} WhoHasLoop WhoHasStates.ANSWER ID {1} added to CovNotificationSubscribeIDQueue", DateTime.Now.ToString("HH:mm:ss.fff"), WhoHasOnIdleJob.SubscriberID);
                                    }

                                    WhoHasState = WhoHasStates.SEND;
                                    if ((DriverErrorCodes)Error != DriverErrorCodes.ErrorNoError)
                                    {
                                        WhoHasOnIdleJob.setError((DriverErrorCodes)Error);
                                    }
                                    receiveItem.isValid = false;

                                    //System.Diagnostics.Debug.WriteLine("{0} DeviceWrite WhoHas", DateTime.Now.ToString("HH:mm:ss.fff"));
                                }
                            }
                            else if ((WhoHasOnIdleJob.Station as BACnetStation).mapObjectID.TryGetValue(WhoHasOnIdleJob.ObjectFullName, out ObjectIdentifier)) //WhoHasOnIdleJob.ObjectID))
                            {
                                WhoHasOnIdleJob.ObjectID = ObjectIdentifier;
                                WhoHasOnIdleJob.BACnetInitDone = true;
                                if (WhoHasOnIdleJob.COVEnable &&
                                    BACnetEnums.isCovSupported(WhoHasOnIdleJob.BACnetObjectType.ToString(), WhoHasOnIdleJob.PropertyIdentifier.ToString()) &&
                                    !CovNotificationSubscribeIDEnqueued.Contains(WhoHasOnIdleJob.SubscriberID))
                                {
                                    CovNotificationSubscribeIDEnqueued.Add(WhoHasOnIdleJob.SubscriberID);
                                    CovNotificationSubscribeIDQueue.Enqueue(WhoHasOnIdleJob.SubscriberID);
                                    System.Diagnostics.Debug.WriteLine("{0} WhoHasLoop WhoHasStates.SEND ID {1} added to CovNotification queues", DateTime.Now.ToString("HH:mm:ss.fff"), WhoHasOnIdleJob.SubscriberID);
                                }
                            }
                            else
                            {
                                if (!DeviceWrite(IPFrameFactory.WhoHas(WhoHasOnIdleJob.ObjectName, WhoHasOnIdleJob.Station as BACnetStation), (WhoHasOnIdleJob.Station as BACnetStation).RemoteEndPoint))
                                {
                                    WhoHasOnIdleJob.lastWhoHasTestTime = DateTime.UtcNow;
                                    WhoHasJobQueue.Enqueue(WhoHasOnIdleJob);
                                    Error = BACnetErrorCodes.ErrorTxWrite;
                                    WhoHasOnIdleJob.setError((DriverErrorCodes)Error);
                                }
                                else
                                {
                                    WhoHasTxTime = DateTime.UtcNow;
                                    WhoHasState = WhoHasStates.ANSWER;
                                }
                                //System.Diagnostics.Debug.WriteLine("{0} DeviceWrite WhoHas", DateTime.Now.ToString("HH:mm:ss.fff"));
                            }
                        }
                    }
                    break;
                case WhoHasStates.ANSWER:
                    if (receiveItem.isValid && receiveItem.RemoteEndPoint.Address.Equals((WhoHasOnIdleJob.Station as BACnetStation).RemoteEndPoint.Address))
                    {
                        if (IPFrameValidator.IHave(receiveItem.Frame, (WhoHasOnIdleJob.Station as BACnetStation).DeviceIdentifier, WhoHasOnIdleJob.ObjectName,
                            ref WhoHasOnIdleJob.ObjectID))
                        {
                            (WhoHasOnIdleJob.Station as BACnetStation).mapObjectID[WhoHasOnIdleJob.ObjectFullName] = WhoHasOnIdleJob.ObjectID;
                            WhoHasOnIdleJob.BACnetInitDone = true;
                                                        
                            if (WhoHasOnIdleJob.SubscriberID != 0 && WhoHasOnIdleJob.COVEnable &&
                                BACnetEnums.isCovSupported(WhoHasOnIdleJob.BACnetObjectType.ToString(), WhoHasOnIdleJob.PropertyIdentifier.ToString()) &&
                                !CovNotificationSubscribeIDEnqueued.Contains(WhoHasOnIdleJob.SubscriberID))
                            {
                                CovNotificationSubscribeIDEnqueued.Add(WhoHasOnIdleJob.SubscriberID);
                                CovNotificationSubscribeIDQueue.Enqueue(WhoHasOnIdleJob.SubscriberID);
                                System.Diagnostics.Debug.WriteLine("{0} WhoHasLoop WhoHasStates.ANSWER ID {1} added to CovNotificationSubscribeIDQueue", DateTime.Now.ToString("HH:mm:ss.fff"), WhoHasOnIdleJob.SubscriberID);
                            }

                            ((BACnetStation)WhoHasOnIdleJob.Station).SetLastDateTimeGoodAnswer();
                            WhoHasState = WhoHasStates.SEND;                            
                            if ((DriverErrorCodes)Error != DriverErrorCodes.ErrorNoError)
                            {
                                WhoHasOnIdleJob.setError((DriverErrorCodes)Error);
                            }
                            receiveItem.isValid = false;
                        }
                    }
                    if (WhoHasState == WhoHasStates.ANSWER)
                    {
                        if ((DateTime.UtcNow - WhoHasTxTime).TotalMilliseconds > Timeout)
                        {
                            WhoHasOnIdleJob.lastWhoHasTestTime = DateTime.UtcNow;
                            WhoHasJobQueue.Enqueue(WhoHasOnIdleJob);
                            WhoHasState = WhoHasStates.SEND;
                            Error = BACnetErrorCodes.ErrorWhoHasFailed;
                            WhoHasOnIdleJob.setError((DriverErrorCodes)Error);
                        }
                    }
                    break;
                case WhoHasStates.END:
                    break;
            }
            return Error;
        }
        /// <summary>   CovNotificationLoop . </summary>
        private BACnetErrorCodes CovNotificationLoop(ref ReceiveItem receiveItem)
        {
            BACnetErrorCodes Error = (BACnetErrorCodes)DriverErrorCodes.ErrorNoError;
            switch (CovNotificationState)
            {
                case CovNotificationStates.BEGIN:
                    CovNotificationState = CovNotificationStates.SEND;
                    break;
                case CovNotificationStates.SEND:
                    if (CovNotificationSubscribeIDQueue.Count() != 0)
                    {
                        CovNotificationOnIdleSubscribeID = CovNotificationSubscribeIDQueue.Dequeue();
                        System.Diagnostics.Debug.WriteLine("{0}  DEBUG - CovNotificationLoop CovNotificationStates.SEND ID {1} removed from CovNotificationSubscribeIDQueue", DateTime.Now.ToString("HH:mm:ss.fff"), CovNotificationOnIdleSubscribeID);
                        if (mapSubscribeIDJob.ContainsKey(CovNotificationOnIdleSubscribeID) &&
                            mapSubscribeIDJob[CovNotificationOnIdleSubscribeID].Count() != 0)
                        {
                            BACnetStation servedStation = mapSubscribeIDJob[CovNotificationOnIdleSubscribeID][0].Station as BACnetStation;
                            if (mapSubscribeIDJob[CovNotificationOnIdleSubscribeID][0].COVEnable &&
                                BACnetEnums.isCovSupported(mapSubscribeIDJob[CovNotificationOnIdleSubscribeID][0].BACnetObjectType.ToString(), mapSubscribeIDJob[CovNotificationOnIdleSubscribeID][0].PropertyIdentifier.ToString()))
                            {

                                if (!servedStation.mapSubscribeTime.ContainsKey(mapSubscribeIDJob[CovNotificationOnIdleSubscribeID][0].ObjectID.valueUint))
                                    servedStation.mapSubscribeTime[mapSubscribeIDJob[CovNotificationOnIdleSubscribeID][0].ObjectID.valueUint] = DateTime.MinValue;
                                //if (!servedStation.mapSubscribeErrTime.ContainsKey(mapSubscribeIDJob[CovNotificationOnIdleSubscribeID][0].ObjectID.valueUint))
                                //    servedStation.mapSubscribeErrTime[mapSubscribeIDJob[CovNotificationOnIdleSubscribeID][0].ObjectID.valueUint] = DateTime.MinValue;

                                if (!servedStation.BACnetInitDone || !mapSubscribeIDJob[CovNotificationOnIdleSubscribeID][0].BACnetInitDone ||
                                    //(DateTime.UtcNow - servedStation.mapSubscribeErrTime[mapSubscribeIDJob[CovNotificationOnIdleSubscribeID][0].ObjectID.valueUint]).TotalMilliseconds < BACnetEnums.repeatDelay ||
                                    (DateTime.UtcNow - servedStation.mapSubscribeTime[mapSubscribeIDJob[CovNotificationOnIdleSubscribeID][0].ObjectID.valueUint]).TotalSeconds < servedStation.COVInterval)
                                {
                                    CovNotificationSubscribeIDQueue.Enqueue(CovNotificationOnIdleSubscribeID);
                                    System.Diagnostics.Debug.WriteLine("{0}  DEBUG - CovNotificationLoop CovNotificationStates.SEND ID {1} not sent {2} - {3} - {4}", DateTime.Now.ToString("HH:mm:ss.fff"), CovNotificationOnIdleSubscribeID,
                                        servedStation.BACnetInitDone, mapSubscribeIDJob[CovNotificationOnIdleSubscribeID][0].BACnetInitDone,
                                        (DateTime.UtcNow - servedStation.mapSubscribeTime[mapSubscribeIDJob[CovNotificationOnIdleSubscribeID][0].ObjectID.valueUint]).TotalSeconds);
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine("{0}  DEBUG - CovNotificationLoop CovNotificationStates.SEND ID {1} sending {2} - {3} - {4}", DateTime.Now.ToString("HH:mm:ss.fff"), CovNotificationOnIdleSubscribeID,
                                        servedStation.BACnetInitDone, mapSubscribeIDJob[CovNotificationOnIdleSubscribeID][0].BACnetInitDone,
                                        (DateTime.UtcNow - servedStation.mapSubscribeTime[mapSubscribeIDJob[CovNotificationOnIdleSubscribeID][0].ObjectID.valueUint]).TotalSeconds);
                                    mapSubscribeIDJob[CovNotificationOnIdleSubscribeID][0].InvokeID = getInvokeID();
                                    if (!DeviceWrite(IPFrameFactory.confirmedSubscribeCov(mapSubscribeIDJob[CovNotificationOnIdleSubscribeID][0].InvokeID,
                                        CovNotificationOnIdleSubscribeID, mapSubscribeIDJob[CovNotificationOnIdleSubscribeID][0].ObjectID, servedStation),
                                        servedStation.RemoteEndPoint))
                                    {
                                        CovNotificationSubscribeIDQueue.Enqueue(CovNotificationOnIdleSubscribeID);
                                        System.Diagnostics.Debug.WriteLine("{0}  DEBUG - CovNotificationLoop CovNotificationStates.SEND (error) ID {1}", DateTime.Now.ToString("HH:mm:ss.fff"), CovNotificationOnIdleSubscribeID);
                                        //servedStation.mapSubscribeErrTime[mapSubscribeIDJob[CovNotificationOnIdleSubscribeID][0].ObjectID.valueUint] = DateTime.UtcNow;
                                        servedStation.mapSubscribeTime[mapSubscribeIDJob[CovNotificationOnIdleSubscribeID][0].ObjectID.valueUint] = new DateTime(DateTime.UtcNow.Ticks - servedStation.COVInterval * 10000 + BACnetEnums.repeatDelay * 10000);

                                        Error = BACnetErrorCodes.ErrorTxWrite;
                                        foreach (BACnetCommJob subscribeIDJob in mapSubscribeIDJob[CovNotificationOnIdleSubscribeID])
                                        {
                                            subscribeIDJob.setError((DriverErrorCodes)Error);
                                        }
                                    }
                                    else
                                    {
                                        CovNotificationTxTime = DateTime.UtcNow;
                                        CovNotificationState = CovNotificationStates.ANSWER;
                                        System.Diagnostics.Debug.WriteLine("{0}  DEBUG - CovNotificationLoop CovNotificationStates.SEND COV subscribed for ID {1}", DateTime.Now.ToString("HH:mm:ss.fff"), CovNotificationOnIdleSubscribeID);
                                    }
                                    //System.Diagnostics.Debug.WriteLine("{0} DeviceWrite CovNotificationStates", DateTime.Now.ToString("HH:mm:ss.fff"));
                                }
                            }
                        }
                    }
                    break;
                case CovNotificationStates.ANSWER:
                    {
                        BACnetStation servedStation = mapSubscribeIDJob[CovNotificationOnIdleSubscribeID][0].Station as BACnetStation;
                        if (receiveItem.isValid && receiveItem.RemoteEndPoint.Address.Equals(servedStation.RemoteEndPoint.Address))
                        {
                            if (IPFrameValidator.isCovSubscriptionOk(receiveItem.Frame, mapSubscribeIDJob[CovNotificationOnIdleSubscribeID][0].InvokeID))
                            {
                                servedStation.SetLastDateTimeGoodAnswer();
                                CovNotificationState = CovNotificationStates.SEND;                                
                                //servedStation.mapSubscribeErrTime[mapSubscribeIDJob[CovNotificationOnIdleSubscribeID][0].ObjectID.valueUint] = DateTime.MinValue;
                                servedStation.mapSubscribeTime[mapSubscribeIDJob[CovNotificationOnIdleSubscribeID][0].ObjectID.valueUint] = DateTime.UtcNow;
                                if (servedStation.COVInterval > 0)
                                {
                                    CovNotificationSubscribeIDQueue.Enqueue(CovNotificationOnIdleSubscribeID);
                                    System.Diagnostics.Debug.WriteLine("{0}  DEBUG - CovNotificationLoop CovNotificationStates.ANSWER (COV OK) ID {1} added to CovNotificationSubscribeIDQueue", DateTime.Now.ToString("HH:mm:ss.fff"), CovNotificationOnIdleSubscribeID);
                                }
                                foreach (BACnetCommJob subscribeIDJob in mapSubscribeIDJob[CovNotificationOnIdleSubscribeID])
                                {
                                    subscribeIDJob.setError((DriverErrorCodes)Error);

                                    if (servedStation.ForceInitialPolling)
                                        ForceInitialPollingJob.Add(subscribeIDJob);
                                }

                                receiveItem.isValid = false;
                            }
                            if (IPFrameValidator.isCovSubscriptionFailed(receiveItem.Frame, mapSubscribeIDJob[CovNotificationOnIdleSubscribeID][0].InvokeID))
                            {
                                CovNotificationState = CovNotificationStates.SEND;
                                CovNotificationSubscribeIDQueue.Enqueue(CovNotificationOnIdleSubscribeID);
                                System.Diagnostics.Debug.WriteLine("{0}  DEBUG - CovNotificationLoop CovNotificationStates.ANSWER (COV Failed) ID {1} added to CovNotificationSubscribeIDQueue", DateTime.Now.ToString("HH:mm:ss.fff"), CovNotificationOnIdleSubscribeID);
                                //servedStation.mapSubscribeErrTime[mapSubscribeIDJob[CovNotificationOnIdleSubscribeID][0].ObjectID.valueUint] = DateTime.UtcNow;
                                //servedStation.mapSubscribeTime[mapSubscribeIDJob[CovNotificationOnIdleSubscribeID][0].ObjectID.valueUint] = DateTime.MinValue;
                                servedStation.mapSubscribeTime[mapSubscribeIDJob[CovNotificationOnIdleSubscribeID][0].ObjectID.valueUint] = DateTime.UtcNow;
                                Error = BACnetErrorCodes.ErrorCovSubscriptionFailed;
                                foreach (BACnetCommJob subscribeIDJob in mapSubscribeIDJob[CovNotificationOnIdleSubscribeID])
                                {
                                    subscribeIDJob.setError((DriverErrorCodes)Error);
                                }
                                receiveItem.isValid = false;
                            }
                        }
                        if (CovNotificationState == CovNotificationStates.ANSWER)
                        {
                            if ((DateTime.UtcNow - CovNotificationTxTime).TotalMilliseconds > Timeout ||
                                IPFrameValidator.isCovSubscriptionFailed(receiveItem.Frame, mapSubscribeIDJob[CovNotificationOnIdleSubscribeID][0].InvokeID))
                            {
                                //CovNotificationState = CovNotificationStates.SEND_UNSUBSCRIVECOV;
                                CovNotificationState = CovNotificationStates.SEND;
                                CovNotificationSubscribeIDQueue.Enqueue(CovNotificationOnIdleSubscribeID);
                                System.Diagnostics.Debug.WriteLine("{0}  DEBUG - CovNotificationLoop CovNotificationStates.ANSWER (error) ID {1} added to CovNotificationSubscribeIDQueue", DateTime.Now.ToString("HH:mm:ss.fff"), CovNotificationOnIdleSubscribeID);
                                //servedStation.mapSubscribeErrTime[mapSubscribeIDJob[CovNotificationOnIdleSubscribeID][0].ObjectID.valueUint] = DateTime.UtcNow;
                                servedStation.mapSubscribeTime[mapSubscribeIDJob[CovNotificationOnIdleSubscribeID][0].ObjectID.valueUint] = DateTime.MinValue;
                                Error = BACnetErrorCodes.ErrorCovSubscriptionFailed;
                                foreach (BACnetCommJob subscribeIDJob in mapSubscribeIDJob[CovNotificationOnIdleSubscribeID])
                                {
                                    subscribeIDJob.setError((DriverErrorCodes)Error);
                                }
                            }
                        }
                    }
                    break;
                case CovNotificationStates.END:
                    break;
            }
            return Error;
        }
        private UInt32 getSubscriberID()
        {
            //CounterIDObject++;
            //if(CounterIDObject == 0)
            //{
            //    CounterIDObject++;
            //}
            //return CounterIDObject;
            UInt32 SubscriberID = (UInt32)rndGen.Next(int.MinValue, int.MaxValue);
            while (SubscriberID == 0 || mapSubscribeIDJob.ContainsKey(SubscriberID))
            {
                SubscriberID = (UInt32)rndGen.Next(int.MinValue, int.MaxValue);
            }
            return SubscriberID;
        }

        /// <summary>   ReadWritePropertyLoop . </summary>
        private BACnetErrorCodes ReadWritePropertyLoop(ref ReceiveItem receiveItem)
        {
            BACnetErrorCodes Error = (BACnetErrorCodes)DriverErrorCodes.ErrorNoError;
            switch (ReadWritePropertyState)
            {
                case ReadWritePropertyStates.BEGIN:
                    ReadWritePropertyState = ReadWritePropertyStates.SEND;
                    break;
                case ReadWritePropertyStates.SEND:
                    if (ForceInitialPollingJob.Count() == 0)
                    {
                        ScheduleListJob();
                        ReadWritePropertyOnIdleJob = GetNextPendingJob() as BACnetCommJob;
                    }
                    else
                        ReadWritePropertyOnIdleJob = ForceInitialPollingJob[0];

                    if (ReadWritePropertyOnIdleJob != null)
                    {
                        ReadWritePropertyOnIdleJob.LastExecutionTime = DateTime.UtcNow;
                        if (ReadWritePropertyOnIdleJob.BACnetInitDone)
                        {
                            lock (ReadWritePropertyOnIdleJob.retLockList())
                            {
                                base.ExecuteJob(ReadWritePropertyOnIdleJob);
                            }

                            BACnetIPFrame Frame;
                            if ((ReadWritePropertyOnIdleJob.ReadRequest() || ForceInitialPollingJob.Count() != 0) &&
                                !ReadWritePropertyOnIdleJob.IsRelinquishForced())
                            {
                                if (!ReadWritePropertyOnIdleJob.COVEnable || ForceInitialPollingJob.Count() != 0)
                                {
                                    lock (ReadWritePropertyOnIdleJob.retLockList())
                                    {
                                        ReadWritePropertyOnIdleJob.InvokeID = getInvokeID();
                                        base.ExecuteJob(ReadWritePropertyOnIdleJob);
                                        Frame = IPFrameFactory.confirmedReadProperty(ReadWritePropertyOnIdleJob);
                                    }

                                    //System.Diagnostics.Debug.WriteLine("{0} start send  ", DateTime.Now.ToString("HH:mm:ss.fff"));
                                    if (!DeviceWrite(Frame, (ReadWritePropertyOnIdleJob.Station as BACnetStation).RemoteEndPoint))
                                    {
                                        Error = BACnetErrorCodes.ErrorTxWrite;
                                        OnJobExecuted(new BACnetExecutedJobArgs
                                        {
                                            ErrorCode = (DriverErrorCodes)Error,
                                            Job = ReadWritePropertyOnIdleJob,
                                            IsRead = true
                                        });
                                    }
                                    else
                                    {
                                        //System.Diagnostics.Debug.WriteLine("{0} end send  ", DateTime.Now.ToString("HH:mm:ss.fff"));
                                        ReadWritePropertyTxTime = DateTime.UtcNow;
                                        ReadWritePropertyState = ReadWritePropertyStates.ANSWER_READ;
                                    }
                                }
                                else
                                {
                                    lock (ReadWritePropertyOnIdleJob.retLockList())
                                    {
                                        ReadWritePropertyOnIdleJob.IsPending = false;
                                    }
                                }
                            }
                            else
                            {
                                IPFrameFactory.WriteFormatResult WriteMessage;
                                string ErrorOnWriteMessage = String.Empty;
                                bool WriteOK = true;

                                lock (ReadWritePropertyOnIdleJob.retLockList())
                                {
                                    ReadWritePropertyOnIdleJob.InvokeID = getInvokeID();

                                    WriteMessage = IPFrameFactory.confirmedWriteProperty(out Frame, ReadWritePropertyOnIdleJob, out ErrorOnWriteMessage);
                                    
                                    // ERR_NO_DATA_TO_WRITE = data to write not changed
                                    if (WriteMessage == IPFrameFactory.WriteFormatResult.OK || WriteMessage == IPFrameFactory.WriteFormatResult.ERR_NO_DATA_TO_WRITE)
                                    {
                                        base.ExecuteJob(ReadWritePropertyOnIdleJob);
                                        if (!ReadWritePropertyOnIdleJob.IsRelinquishForced())
                                        {
                                            ReadWritePropertyOnIdleJob.IsRead = false;
                                        }
                                    }
                                }

                                // if message is well formatted, send it                                
                                if (WriteMessage == IPFrameFactory.WriteFormatResult.OK)
                                    WriteOK = DeviceWrite(Frame, (ReadWritePropertyOnIdleJob.Station as BACnetStation).RemoteEndPoint);
                                else // otherwise error (except no data to send)
                                    WriteOK = (WriteMessage == IPFrameFactory.WriteFormatResult.ERR_NO_DATA_TO_WRITE);
                                
                                // messagw sent or error
                                if (!WriteOK) {
                                    Error = !WriteOK ? BACnetErrorCodes.ErrorTxWrite : BACnetErrorCodes.ErrorInvalidDataType;
                                    OnJobExecuted(new BACnetExecutedJobArgs
                                    {
                                        ErrorCode = (DriverErrorCodes)Error,
                                        Job = ReadWritePropertyOnIdleJob,
                                        IsRead = false
                                    });
                                } else {
                                    ReadWritePropertyTxTime = DateTime.UtcNow;
                                    // not data to changed, no write send --> close loop here
                                    if (WriteMessage == IPFrameFactory.WriteFormatResult.ERR_NO_DATA_TO_WRITE)
                                    {
                                        ((BACnetStation)ReadWritePropertyOnIdleJob.Station).SetLastDateTimeGoodAnswer();
                                        // comment code above to leave active conditional variable (if present)
                                        //OnJobExecuted(new BACnetExecutedJobArgs
                                        //{
                                        //    ErrorCode = DriverErrorCodes.ErrorNoError,
                                        //    Job = ReadWritePropertyOnIdleJob,
                                        //    IsRead = false
                                        //});
                                        ReadWritePropertyOnIdleJob.IsPending = false;
                                        ReadWritePropertyState = ReadWritePropertyStates.SEND;
                                        receiveItem.isValid = false;
                                    }
                                    else
                                        ReadWritePropertyState = ReadWritePropertyStates.ANSWER_WRITE;
                                }
                            }
                        }
                    }
                    break;
                case ReadWritePropertyStates.ANSWER_READ:

                    if (receiveItem.isValid &&
                        receiveItem.RemoteEndPoint.Address.Equals((ReadWritePropertyOnIdleJob.Station as BACnetStation).RemoteEndPoint.Address) &&
                        IPFrameValidator.isForThisReadJob(receiveItem.Frame, ReadWritePropertyOnIdleJob.InvokeID))
                    {
                        if (IPFrameValidator.isErrorFrame(receiveItem.Frame))
                        {
                            Error = IPFrameValidator.GetError(receiveItem.Frame);
                            OnJobExecuted(new BACnetExecutedJobArgs
                            {
                                ErrorCode = (DriverErrorCodes)Error,
                                Job = ReadWritePropertyOnIdleJob,
                                IsRead = true
                            });
                        }
                        else if (IPFrameValidator.isConfirmedReadProperty(receiveItem.Frame, ReadWritePropertyOnIdleJob.InvokeID,
                            ReadWritePropertyOnIdleJob.ObjectID, ReadWritePropertyOnIdleJob.PropertyIdentifier, ReadWritePropertyOnIdleJob.ArrayIndex))
                        {
                            if (ForceInitialPollingJob.Count() != 0)
                                ForceInitialPollingJob.RemoveAt(0);

                            byte[] answer = null;
                            IPFrameValidator.ConfirmedReadProperty(receiveItem.Frame, ReadWritePropertyOnIdleJob.InvokeID,
                            ReadWritePropertyOnIdleJob.ObjectID, ReadWritePropertyOnIdleJob.PropertyIdentifier, ref answer, ReadWritePropertyOnIdleJob.ArrayIndex, ReadWritePropertyOnIdleJob);
                            OnJobExecuted(new BACnetExecutedJobArgs
                            {
                                ErrorCode = DriverErrorCodes.ErrorNoError,
                                Job = ReadWritePropertyOnIdleJob,
                                Values = answer,
                                IsRead = true
                            });                            
                        }
                        ((BACnetStation)ReadWritePropertyOnIdleJob.Station).SetLastDateTimeGoodAnswer();
                        ReadWritePropertyState = ReadWritePropertyStates.SEND;                        
                        receiveItem.isValid = false;
                    }
                    if (ReadWritePropertyState == ReadWritePropertyStates.ANSWER_READ)
                    {
                        if ((DateTime.UtcNow - ReadWritePropertyTxTime).TotalMilliseconds > Timeout)
                        {
                            ReadWritePropertyState = ReadWritePropertyStates.SEND;
                            Error = BACnetErrorCodes.ErrorBACnetTimeOut;
                            OnJobExecuted(new BACnetExecutedJobArgs
                            {
                                ErrorCode = (DriverErrorCodes)BACnetErrorCodes.ErrorBACnetTimeOut,
                                Job = ReadWritePropertyOnIdleJob,
                                IsRead = true
                            });
                        }
                    }
                    break;
                case ReadWritePropertyStates.ANSWER_WRITE:
                    if (receiveItem.isValid &&
                        receiveItem.RemoteEndPoint.Address.Equals((ReadWritePropertyOnIdleJob.Station as BACnetStation).RemoteEndPoint.Address) &&
                        IPFrameValidator.isForThisWriteJob(receiveItem.Frame, ReadWritePropertyOnIdleJob.InvokeID))
                    {
                        if (IPFrameValidator.isErrorFrame(receiveItem.Frame))
                            Error = IPFrameValidator.GetError(receiveItem.Frame);

                        OnJobExecuted(new BACnetExecutedJobArgs
                        {
                            ErrorCode = (DriverErrorCodes)Error,
                            Job = ReadWritePropertyOnIdleJob,
                            IsRead = false
                        });
                        ((BACnetStation)ReadWritePropertyOnIdleJob.Station).SetLastDateTimeGoodAnswer();
                        ReadWritePropertyState = ReadWritePropertyStates.SEND;
                        receiveItem.isValid = false;
                    }
                    if (ReadWritePropertyState == ReadWritePropertyStates.ANSWER_WRITE)
                    {
                        if ((DateTime.UtcNow - ReadWritePropertyTxTime).TotalMilliseconds > Timeout)
                        {
                            ReadWritePropertyState = ReadWritePropertyStates.SEND;
                            Error = BACnetErrorCodes.ErrorBACnetTimeOut;
                            OnJobExecuted(new BACnetExecutedJobArgs
                            {
                                ErrorCode = (DriverErrorCodes)BACnetErrorCodes.ErrorBACnetTimeOut,
                                Job = ReadWritePropertyOnIdleJob,
                                IsRead = false
                            });
                        }
                    }
                    break;
                case ReadWritePropertyStates.END:
                    break;
            }
            return Error;
        }

        private BACnetErrorCodes ConsumeCovNotification(ref ReceiveItem receiveItem)
        {
            BACnetErrorCodes Error = (BACnetErrorCodes)DriverErrorCodes.ErrorNoError;
            if (receiveItem.isValid && IPFrameValidator.isUnconfirmedCovNotification(receiveItem.Frame))
            {
                UInt32 SubscriberID = 0;
                BACnetObjectIdentifier DeviceID = new BACnetObjectIdentifier();
                BACnetObjectIdentifier ObjectID = new BACnetObjectIdentifier();
                UInt32 TimeRemaining = 0;
                ServiceTagList TagList = new ServiceTagList();
                if (IPFrameValidator.UnconfirmedCovNotification(receiveItem.Frame, ref SubscriberID, ref DeviceID,
                    ref ObjectID, ref TimeRemaining, ref TagList))
                {
                    List<BACnetCommJob> servedJoblist;
                    if (mapSubscribeIDJob.TryGetValue(SubscriberID, out servedJoblist))
                    {
                        BACnetStation servedStation = servedJoblist[0].Station as BACnetStation;
                        if (receiveItem.RemoteEndPoint.Address.Equals(servedStation.RemoteEndPoint.Address) &&
                            servedStation.DeviceIdentifier.valueUint == DeviceID.valueUint && servedJoblist[0].ObjectID.valueUint == ObjectID.valueUint)
                        {
                            foreach (BACnetCommJob JobExecute in servedJoblist)
                            {
                                byte[] answer = null;
                                if (JobExecute.PropertyIdentifier == (BACnetEnums.PropertyIdentifier)TagList.List[0].UInt)
                                    answer = TagList.List[1].TagList.List[0].Values;
                                else if (JobExecute.PropertyIdentifier == (BACnetEnums.PropertyIdentifier)TagList.List[2].UInt)
                                    answer = TagList.List[3].TagList.List[0].Values;

                                if (answer != null)
                                {
                                    OnJobExecuted(new BACnetExecutedJobArgs
                                    {
                                        ErrorCode = DriverErrorCodes.ErrorNoError,
                                        Job = JobExecute,
                                        Values = answer,
                                        IsRead = true
                                    });
                                    servedStation.SetLastDateTimeGoodAnswer();
                                }
                            }
                        }
                    }
                    receiveItem.isValid = false;

                    //listDebug.Add(string.Format("{0} ConsumeCovNotification",
                    //    DateTime.Now.ToString("HH:mm:ss.fff")
                    //    ));
                }
            }
            return Error;
        }

        /// <summary>   ReceiveLoop . </summary>
        private BACnetErrorCodes ReceiveLoop(ref ReceiveItem receiveItem)
        {
            BACnetErrorCodes Error = (BACnetErrorCodes)DriverErrorCodes.ErrorNoError;
            switch (ReciveState)
            {
                case ReciveStates.Init:
                    ReceivePacketQueue.Clear();
                    if (BeginDeviceRead(0) && BeginDeviceRead_Ex(0))
                        ReciveState = ReciveStates.Wait;
                    break;

                case ReciveStates.Wait:
                    BACnetRead(ref receiveItem);
                    break;

            }
            return Error;
        }

        #endregion
        string filebaseSubscriberID = string.Empty;
        InMemoryDataStore InMemorySubscriberID = null;
        IDataLayer idlSubscriberID = null;
        UnitOfWork ufwSubscriberID = null;
        private void AssignsSubscriberID(BACnetStation station, ref BACnetCommJob job)
        {
            if (job.SubscriberID != 0)
            {
                return;
            }
            if (idlSubscriberID == null)
            {
                idlSubscriberID = GetPathToTheFileSaveData((BACnetDriver)CommDriver, Name, out filebaseSubscriberID, out InMemorySubscriberID);
                if (idlSubscriberID == null)
                {
                    return;
                }
                ufwSubscriberID = new UnitOfWork(idlSubscriberID);
            }
            if (mapSubscribeID.Count == 0)
            {
                if (!string.IsNullOrWhiteSpace(filebaseSubscriberID))
                {
                    if (File.Exists(filebaseSubscriberID) && (InMemorySubscriberID != null))
                    {
                        InMemorySubscriberID.ReadXml(filebaseSubscriberID);
                    }
                }
                //mapSubscribeID = (from tag in new XPQuery<BACnetSubscribeId>(ufwSubscriberID).AsParallel() select tag).ToDictionary(tag => tag.Name, tag => tag.SubcribeId  );
                mapSubscribeID = (from tag in new XPQuery<BACnetSubscribeId>(ufwSubscriberID)/*.AsParallel()*/ select tag)
                                                                                                              .GroupBy(p=> p.Name)
                                                                                                              .Select(grp => grp.FirstOrDefault())
                                                                                                              .ToDictionary(tag => tag.Name, tag => tag.SubcribeId);
            }

            //string keyName = station.Name + "_" + job.ObjectName;
            string keyName = job.ObjectFullName;
            if ((mapSubscribeID.Count == 0) || (!mapSubscribeID.ContainsKey(keyName)))
            {
                job.SubscriberID = getSubscriberID();
                var subdcribedId = new BACnetSubscribeId(ufwSubscriberID);
                subdcribedId.Name = keyName;
                subdcribedId.SubcribeId = job.SubscriberID;
                ufwSubscriberID.CommitChanges();
                if (!string.IsNullOrWhiteSpace(filebaseSubscriberID) && (InMemorySubscriberID != null))
                {
                    InMemorySubscriberID.WriteXml(filebaseSubscriberID);
                }
                mapSubscribeID.Add(keyName, job.SubscriberID);
            }
            else if (mapSubscribeID.ContainsKey(keyName))
            {
                job.SubscriberID = mapSubscribeID[keyName];
            }
        }

        static IDataLayer GetPathToTheFileSaveData(BACnetDriver BACnetCommDriver, string name, out string filebase, out InMemoryDataStore InMemory)
        { 
            string connect = CommunicationDriver.GetConnectionString(BACnetCommDriver.StrConnectionString, "Drivers", name, Properties.Settings.Default.ExtentionOfTheFileCov);
            IDataLayer dl = CommunicationDriver.GetSpecificDataLayer(connect, out filebase, out InMemory, out bool targetIsFile);
            return (dl);
        }


        //static IDataLayer GetSpecificDataLayer(string conn, out string filebase, out InMemoryDataStore InMemory, bool xml = true)
        //{
        //    IDataLayer dl = null;
        //    InMemory = null;
        //    filebase = string.Empty;
        //    ConnectionStringParser helper = new ConnectionStringParser(conn);
        //    string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);

        //    if (providerType != InMemoryDataStore.XpoProviderTypeString)
        //    {
        //        dl = XpoDefault.GetDataLayer(conn, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
        //    }
        //    else if (xml)
        //    {
        //        filebase = helper.GetPartByName("data source");
        //        InMemory = CommunicationDriver.GetDataStore(filebase);
        //        dl = new SimpleDataLayer(InMemory);
        //    }
        //    return dl;
        //}


        #region BBMD

        private void BBMDInit()
        {
            BBMDDevices = new Dictionary<string, BACnetBBMDDevice>();
            BACnetBBMDDevice BBMD = null;

            foreach (BACnetStation station in listStation)
            {
                if (station.BBMDRegister)
                {
                    if (!BBMDDevices.ContainsKey(station.BBMDId))
                    {
                        //Id and Hostname now is the same parameter
                        BBMD = new BACnetBBMDDevice(this, station.BBMDId, station.BBMDAddress, station.DeviceHostPort, station.BBMDLifetime * 60, true);
                        BBMDDevices.Add(BBMD.Id, BBMD);
                    }
                    else
                    {
                        BBMD = BBMDDevices[station.BBMDId];
                        if (BBMD.Lifetime < (station.BBMDLifetime * 60))
                            BBMD.Lifetime = (station.BBMDLifetime * 60);
                    }
                    BBMD.RelatedStations.Add(station);
                }
            }

            foreach (var device in BBMDDevices)
            {
                BACnetBBMDDevice bbmd = device.Value;
                bbmd.StartKeepAliveRegistration();
            }

            if (BBMDDevices.Count == 0)
                BBMDDevices = null;
        }

        private void BBMDTerminate()
        {
            if (BBMDDevices != null)
            {
                foreach (var device in BBMDDevices)
                {
                    BACnetBBMDDevice bbmd = device.Value;
                    bbmd.StopKeepAliveRegistration();
                }
            }
        }
        //private void BBMDSetStationStatus(BACnetBBMDDevice bBMDDevice, DriverErrorCodes errorCode)
        //{
        //    foreach (BACnetStation station in bBMDDevice.RelatedStations) {

        //        station.IsBBMDRegistred = (errorCode == DriverErrorCodes.ErrorNoError);

        //        if (!station.IsBBMDRegistred)
        //        {
        //            //string AdditionalError;
        //            bool NewErrorOccured = (errorCode !=  DriverErrorCodes.ErrorNoError &&  station.LastErrorCode != errorCode);

        //            if (errorCode != DriverErrorCodes.ErrorNoError)
        //            {
        //                System.Diagnostics.Debug.WriteLine("Station {0} - Channel {1} ", station.Name, station.GetChannel().Name);
        //                if (station.LastErrorCode == DriverErrorCodes.ErrorNoError)
        //                {
        //                    station.LastErrorTime = DateTime.UtcNow;
        //                    station.LastErrorCode = errorCode;
        //                    //AdditionalError = (TagsList.Count > 0 ? TagsList[0].DynSettings.ToString() : null);
        //                }
        //                lock (((BACnetStation)station).getLockBool())
        //                {
        //                    //if (InUse)
        //                    station.InErrorState = true;
        //                    //if (((BACnetStation)Station).GetChannelBase() != null)
        //                    //    ((BACnetStation)Station).GetChannelBase().ChangeStateJob(this, CommJobState.PollingInError);

        //                }

        //            }
        //            else if (station.InErrorState)
        //            {
        //                var listJob = station.GetChannel().GetJobStateList(CommJobState.PollingInError);
        //                bool resetError = true;
        //                foreach (var commjob in listJob)
        //                {
        //                    if (commjob.InErrorState && commjob.InUse)
        //                        resetError = false;
        //                }
        //                if (resetError)
        //                {
        //                    System.Diagnostics.Debug.WriteLine("Station {0} ResetErrors - Channel {1} ResetErrors", station.Name, station.GetChannel().Name);
        //                    station.InErrorState = false;

        //                    station.LastErrorTime = DateTime.MinValue;
        //                    station.LastErrorCode = DriverErrorCodes.ErrorNoError;
        //                }
        //            }

        //            lock (station.getLockBool())
        //            {
        //                //if (!InErrorState && Station.FirstTime)
        //                if (station.FirstTime)
        //                {
        //                    station.FirstTime = false;
        //                    station.GetCommDriver().OnSystemEvent(null, String.Format(DriverCodeBase.Properties.Resources.StationResumeError, station.GetCommDriver().DriverName/*DriverInfo.GetDriverName()*/, station.Name), EventSeverity.Low);
        //                }
        //            }

        //            if (NewErrorOccured)
        //            {
        //                var listJobs = station.GetListWholeJobCopy();
        //                foreach (var elJob in listJobs)
        //                {
        //                    var job = elJob as BACnetCommJob;
        //                    UnsubscribeJob(job);
        //                    if (!job.InErrorState)
        //                        job.SetBadConnectionQuality();
        //                    job.BACnetInitDone = false;
        //                }

        //                station.WhoIsTxTime = DateTime.UtcNow;
        //                station.BACnetInitDone = false;
        //                station.mapSubscribeTime.Clear();
        //                listWhoIsSent.Remove(station);                            
        //                if (station.WhoIsError == false)
        //                {
        //                    station.WhoIsError = true;
        //                    string errorMessage = String.Format(Properties.Resources.ErrorWhoIsFailedForStation, station.Name);
        //                    CommDriver.OnSystemEvent(null, errorMessage, EventSeverity.Low);
        //                    station.SetStateCommandVariableBit(true, (UInt16)StationVariableBits.StationErrorState);
        //                }
        //            }
        //        }                
        //    }
        //}

        //private void BBMDRegisterAndKeepAliveSession(ref ReceiveItem receiveItem)
        //{
        //    foreach (var device in BBMDDevices)
        //    {
        //        BACnetBBMDDevice BBMD = device.Value;

        //        switch (BBMD.State)
        //        {
        //            case BACnetBBMDDevice.BBMDStates.RegisterAsForeignDevice:
        //                if (!DeviceWrite(IPFrameFactory.RegisterAsForeignDevice(this, (ushort)BBMD.Lifetime), BBMD.RemoteEndPoint))
        //                {
        //                    BBMDSetStationStatus(BBMD, (DriverErrorCodes)BACnetErrorCodes.ErrorRegisterToBBMD);
        //                }
        //                else
        //                {
        //                    BBMD.LastSend = DateTime.UtcNow;
        //                    BBMD.State = BACnetBBMDDevice.BBMDStates.WaitRegisterAsForeignDevice;
        //                }
        //                break;
        //            case BACnetBBMDDevice.BBMDStates.WaitRegisterAsForeignDevice:
        //                if (IPFrameValidator.isconfirmedForeignDeviceRegistration(receiveItem))
        //                {
        //                    BBMD.LastSend = DateTime.UtcNow;
        //                    //BBMD.State = BACnetBBMDDevice.BBMDStates.ReadTable;
        //                    BBMD.State = BACnetBBMDDevice.BBMDStates.ReadDFTTable;
        //                }
        //                else
        //                {
        //                    // no answer from device; retry
        //                    if ((DateTime.UtcNow.Subtract(BBMD.LastSend)).TotalMilliseconds > Timeout)
        //                    {                                
        //                        BBMDSetStationStatus(BBMD, (DriverErrorCodes)BACnetErrorCodes.ErrorRegisterToBBMD);
        //                        BBMD.State = BACnetBBMDDevice.BBMDStates.RegisterAsForeignDevice;
        //                    }
        //                }
        //                break;

        //            case BACnetBBMDDevice.BBMDStates.ReadDFTTable:
        //                if (!DeviceWrite(IPFrameFactory.ReadForeignDeviceTable(this), BBMD.RemoteEndPoint))
        //                {
        //                    BBMDSetStationStatus(BBMD, (DriverErrorCodes)BACnetErrorCodes.ErrorRegisterToBBMD);
        //                    return;
        //                }
        //                BBMD.LastSend = DateTime.UtcNow;
        //                BBMD.State = BACnetBBMDDevice.BBMDStates.WaitReadDFTTable;
        //                break;
        //            case BACnetBBMDDevice.BBMDStates.WaitReadDFTTable:
        //                if (IPFrameValidator.isconfirmedReadDFTTableDeviceRegistration(receiveItem))
        //                {
        //                    BBMD.LastSend = DateTime.UtcNow;

        //                    BBMD.State = BACnetBBMDDevice.BBMDStates.ReadBDTTable;
        //                }
        //                else
        //                {
        //                    // no answer from device; retry
        //                    if ((DateTime.UtcNow.Subtract(BBMD.LastSend)).TotalMilliseconds > Timeout)
        //                    {
        //                        BBMDSetStationStatus(BBMD, (DriverErrorCodes)BACnetErrorCodes.ErrorRegisterToBBMD);

        //                        BBMD.LastSend = DateTime.UtcNow;
        //                        BBMD.State = BACnetBBMDDevice.BBMDStates.RegisterAsForeignDevice;
        //                    }
        //                }
        //                break;

        //            case BACnetBBMDDevice.BBMDStates.ReadBDTTable:
        //                if (!DeviceWrite(IPFrameFactory.ReadBBMDDeviceTable(this), BBMD.RemoteEndPoint))
        //                {
        //                    BBMDSetStationStatus(BBMD, (DriverErrorCodes)BACnetErrorCodes.ErrorRegisterToBBMD);
        //                    return;
        //                }
        //                BBMD.LastSend = DateTime.UtcNow;
        //                BBMD.State = BACnetBBMDDevice.BBMDStates.WaitReadBDTTable;
        //                break;
        //            case BACnetBBMDDevice.BBMDStates.WaitReadBDTTable:
        //                if (IPFrameValidator.isconfirmedReadBDTTableDeviceRegistration(receiveItem))
        //                {
        //                    BBMD.LastSend = DateTime.UtcNow;
        //                    BBMD.State = BACnetBBMDDevice.BBMDStates.RefreshRegistration;

        //                    BBMDSetStationStatus(BBMD, DriverErrorCodes.ErrorNoError);
        //                    dtWhoIsNextTime = DateTime.UtcNow.Subtract(new TimeSpan(1000));
        //                }
        //                else
        //                {
        //                    // no answer from device; retry
        //                    if ((DateTime.UtcNow.Subtract(BBMD.LastSend)).TotalMilliseconds > Timeout)
        //                    {
        //                        BBMDSetStationStatus(BBMD, (DriverErrorCodes)BACnetErrorCodes.ErrorRegisterToBBMD);

        //                        BBMD.LastSend = DateTime.UtcNow;
        //                        BBMD.State = BACnetBBMDDevice.BBMDStates.RegisterAsForeignDevice;
        //                    }
        //                }
        //                break;

        //            case BACnetBBMDDevice.BBMDStates.RefreshRegistration:
        //                int LocalTimeout = 0;
        //                // BBMD.Lifetime is in min
        //                LocalTimeout = (int)(BBMD.Lifetime / 2);
        //                if ((DateTime.UtcNow.Subtract(BBMD.LastSend)).TotalSeconds > LocalTimeout)
        //                {
        //                    if (!DeviceWrite(IPFrameFactory.RegisterAsForeignDevice(this, (ushort)BBMD.Lifetime), BBMD.RemoteEndPoint))
        //                    {
        //                        BBMDSetStationStatus(BBMD, (DriverErrorCodes)BACnetErrorCodes.ErrorRegisterToBBMD);
        //                        BBMD.State = BACnetBBMDDevice.BBMDStates.RegisterAsForeignDevice;
        //                    }
        //                    else
        //                    {
        //                        BBMD.LastSend = DateTime.UtcNow;
        //                        BBMD.State = BACnetBBMDDevice.BBMDStates.WaitRefreshRegistration;
        //                    }
        //                }                        
        //                break;
        //            case BACnetBBMDDevice.BBMDStates.WaitRefreshRegistration:
        //                if (IPFrameValidator.isconfirmedForeignDeviceRegistration(receiveItem))
        //                {
        //                    BBMD.State = BACnetBBMDDevice.BBMDStates.RefreshRegistration;
        //                    BBMDSetStationStatus(BBMD, DriverErrorCodes.ErrorNoError);
        //                    dtWhoIsNextTime = DateTime.UtcNow;
        //                }
        //                else
        //                {
        //                    // no answer from device; retry
        //                    if ((DateTime.UtcNow.Subtract(BBMD.LastSend)).TotalMilliseconds > Timeout)
        //                    {
        //                        BBMDSetStationStatus(BBMD, (DriverErrorCodes)BACnetErrorCodes.ErrorRegisterToBBMD);
        //                        BBMD.State = BACnetBBMDDevice.BBMDStates.RegisterAsForeignDevice;
        //                    }
        //                }
        //                break;
        //        }
        //    }
        //}

        private void BBMDRegisterAndKeepAliveSession(ref ReceiveItem receiveItem)
        {
            if (!receiveItem.isValid)
                return;

            if (!receiveItem.Frame.Header.IsBBMDMessage)
                return;

            if (!BBMDDevices.ContainsKey(receiveItem.RemoteEndPoint.Address.ToString()))
                return;

            BACnetBBMDDevice BBMD = BBMDDevices[receiveItem.RemoteEndPoint.Address.ToString()];
            if (IPFrameValidator.isconfirmedForeignDeviceRegistration(receiveItem))
            {
                switch (BBMD.State)
                {
                    case BACnetBBMDDevice.BBMDStates.WaitRegisterAsForeignDevice:
                        ////BBMD.State = BACnetBBMDDevice.BBMDStates.ReadDFTTable;
                        //BBMD.State = BACnetBBMDDevice.BBMDStates.RefreshRegistration;
                        //dtWhoIsNextTime = DateTime.UtcNow;
                        //break;
                    case BACnetBBMDDevice.BBMDStates.WaitRefreshRegistration:
                        BBMD.SetRelatedStationStatus(DriverErrorCodes.ErrorNoError);
                        BBMD.State = BACnetBBMDDevice.BBMDStates.RefreshRegistration;
                        dtWhoIsNextTime = DateTime.UtcNow;
                        break;
                }
            }
            //else if (IPFrameValidator.isconfirmedReadDFTTableDeviceRegistration(receiveItem))
            //{
            //    if (BBMD.State == BACnetBBMDDevice.BBMDStates.WaitReadDFTTable)
            //        BBMD.State = BACnetBBMDDevice.BBMDStates.ReadBDTTable;
            //}
            //else if (IPFrameValidator.isconfirmedReadBDTTableDeviceRegistration(receiveItem))
            //{
            //    if (BBMD.State == BACnetBBMDDevice.BBMDStates.WaitReadBDTTable)
            //        BBMD.State = BACnetBBMDDevice.BBMDStates.RefreshRegistration;
            //}
            receiveItem.isValid = false;
        }


        // if all stations associated to BBMD go down, try to restart BBMD registration
        private void BBMDCheckLinkedStationStatus(Station station)
        {
            BACnetStation BCStation = station as BACnetStation;

            if (!BCStation.BBMDRegister)
                return;

            BACnetBBMDDevice BBMD = BBMDDevices[BCStation.BBMDId];

            if (listStation.Count(a => a.BBMDId == BBMD.Id && a.WhoIsError) == listStation.Count(a => a.BBMDId == BBMD.Id))
            {
                BBMD.SetRelatedStationStatus((DriverErrorCodes)BACnetErrorCodes.ErrorAllStationsOnError);
                BBMD.State = BACnetBBMDDevice.BBMDStates.RefreshRegistration;
            }
        }

        #endregion

        #region Server for WhoIs/i-Am

        private void DeviceServerInit()
        {
            if (DeviceInstance != -1)
            {
                DeviceServer = new BACnetDeviceServer(this, DeviceInstance, DeviceHostName);
                DeviceServer.Init();
            }
        }

        private void DeviceServerWhoIsIam(ref ReceiveItem receiveItem)
        {
            // only on startup send iAm message to informs others devices about our presence
            if (DeviceServer.FirstCycle)
            {
                DeviceServer.FirstCycle = false;
                if (!DeviceServer.SendiAm())
                    CommDriver.OnSystemEvent(ObjectIds.Server, Properties.Resources.ErrorSendiAmOnStartUp, Opc.Ua.EventSeverity.Low);
            }

            if (!receiveItem.isValid)
                return;

            if (IPFrameValidator.isunconfirmedWhoIsRequest(receiveItem))
            {
                receiveItem.isValid = false;
                if (!DeviceServer.SendiAm(receiveItem.RemoteEndPoint))
                    CommDriver.OnSystemEvent(ObjectIds.Server, Properties.Resources.ErrorSendiAm, Opc.Ua.EventSeverity.Low);
            }
        }
        #endregion

        #region Allowed IP

        /// <summary>
        /// Try to add station (with resolved IP) to list of allowed ip incoming message
        /// Execute on start up and before WhoIs
        /// </summary>
        /// <param name="s"></param>
        private void TryToAddGoodStationToAllowedIP(BACnetStation s)
        {
            if (s.RemoteEndPoint != null)
                if (!AllowedIp.ContainsKey(s.RemoteEndPoint.Address.ToString()))
                    AllowedIp[s.RemoteEndPoint.Address.ToString()] = s.Name;
        }

        //Init list of ip message allowed to this station
        private void InitAllowedIP()
        {
            AllowedIp = new Dictionary<string, string>();
            foreach (BACnetStation s in listStation)
                TryToAddGoodStationToAllowedIP(s);                

            if (BBMDDevices != null)
            {
                foreach (var device in BBMDDevices)
                {
                    BACnetBBMDDevice bbmd = device.Value;
                    if (!string.IsNullOrEmpty(device.Value.RemoteEndPoint.Address.ToString()))
                        if (!AllowedIp.ContainsKey(device.Value.RemoteEndPoint.Address.ToString()))
                        AllowedIp[device.Value.RemoteEndPoint.Address.ToString()] = string.Format("BBMD {0}", device.Value.RemoteEndPoint.Address.ToString());
                }
            }
        }
        #endregion

        #region Disposable
        public override void Dispose()
        {
            if (idlSubscriberID != null)
                idlSubscriberID.Dispose();

            if (ufwSubscriberID != null)
                ufwSubscriberID.Dispose();
            
            base.Dispose();
        }
        #endregion
    }
}
