using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using IpDriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using Opc.Ua;
using System.Threading.Tasks;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Threading;

namespace EtherNetIP
{
    public class EtherNetIPChannel : TcpChannelList, IDisposable
    {
        #region Constructors        
        /// <summary>
        /// Initializes the EtherNetIPChannel object.
        /// </summary>
        public EtherNetIPChannel(CommunicationDriver commdriver, EtherNetIPChannelSettings settings)
            : base(commdriver, settings)
        {
            SessionHandle = new uintUnion(0);
        }

        #endregion

        #region Members

        byte[] requestBuffer = new byte[EtherNetIpProtocol.TCP_MAX_SEGMENT_SIZE];
        ushortUnion requestBufferPointer = new ushortUnion(0);
        public uintUnion SessionHandle;
        #endregion

        #region Override Methods
        public override void SubscribeJob(CommJob job, CommJobState state)
        {
            EtherNetIPCommJob j = job as EtherNetIPCommJob;
               
            j.ReadTagStart = 0;
            j.ReadTagEnd = 0;
            j.PartialArrayStart = 0;
            j.PartialArrayEnd = 0;
            //j.ExecuteTask = false;
            j.TerminateTask = false;


            base.SubscribeJob(job, state);            
        }

        //public override bool InitPlcComm(List<CommJob> exjoblist, object thischannel)
        //{
        //    EtherNetIPCommJob etJob = exjoblist[0] as EtherNetIPCommJob;
        //    EtherNetIPStation eipStation = exjoblist[0].Station as EtherNetIPStation;
        //    if (etJob != null && eipStation != null && (etJob.AddressType == AddressTypes.TagName) && (eipStation.ConnectionIDSet != true))
        //    {
        //        if (!EtherNetIpProtocol.Logix5550ForwardOpen(eipStation, ref requestBuffer, ref requestBufferPointer, this))
        //        {
        //            //Error connection
        //            CommDriver.OnSystemEvent(ObjectIds.Server, Properties.Resources.ErrorRegisterSession, Opc.Ua.EventSeverity.High);
        //            //return false;
        //        }
        //        else
        //        {
        //            /*return */Logix5550NonBlockPhAddBuildInfoMaps(eipStation, ref requestBuffer, ref requestBufferPointer);
        //        }
        //    }
        //    return true;
        //}

        public override DriverErrorCodes CheckDevice(List<CommJob> exjoblist, object thischannel)
        {
            DriverErrorCodes conn = DriverErrorCodes.ErrorDeviceOpenFailed;

            bool isOpen = EtherNetIPIsDeviceOpen();
            if (!isOpen)
            {
                isOpen = EtherNetIPDeviceOpen();
            }

            if (isOpen)
            {

                EtherNetIPStation eipStation = exjoblist[0].Station as EtherNetIPStation;
                if (!EtherNetIPDriver.TestConnection)
                {
                    if ((((EtherNetIPCommJob)exjoblist[0]).AddressType == AddressTypes.TagName) && (eipStation.ConnectionIDSet != true))
                    {
                        if (!EtherNetIpProtocol.Logix5550ForwardOpen(eipStation, ref requestBuffer, ref requestBufferPointer, this))
                        {
                            //Error connection
                            CommDriver.OnSystemEvent(ObjectIds.Server, EtherNetIP.Properties.Resources.ErrorRegisterSession, Opc.Ua.EventSeverity.High);
                        }
                        else
                        {
                            Logix5550NonBlockPhAddBuildInfoMaps(eipStation, ref requestBuffer, ref requestBufferPointer);
                        }
                    }
                    if (((((EtherNetIPCommJob)exjoblist[0]).AddressType != AddressTypes.TagName) || (((EtherNetIPCommJob)exjoblist[0]).AddressType == AddressTypes.TagName) && (eipStation.ConnectionIDSet == true)))
                    {
                        conn = (int)DriverErrorCodes.ErrorNoError;
                    }
                }
                else
                {

                    if ((((EtherNetIPCommJob)exjoblist[0]).AddressType == AddressTypes.TagName) && (eipStation.ConnectionIDSet != true) && (eipStation.PlcType == PlcTypes.ControlLogix_CompactLogix))
                    {
                        if (!EtherNetIpProtocol.Logix5550ForwardOpen(eipStation, ref requestBuffer, ref requestBufferPointer, this))
                        {
                            //Error connection
                            CommDriver.OnSystemEvent(ObjectIds.Server, EtherNetIP.Properties.Resources.ErrorRegisterSession, Opc.Ua.EventSeverity.High);
                        }
                        else
                        {
                            Logix5550NonBlockPhAddBuildInfoMaps(eipStation, ref requestBuffer, ref requestBufferPointer);
                            conn = (int)DriverErrorCodes.ErrorNoError;
                        }
                        
                    }
                    else if ((eipStation.PlcType == PlcTypes.PLC5) || (eipStation.PlcType == PlcTypes.SLC500_MicroLogix))
                    {
                        EtherNetIpProtocol.MicroLogixGetCpuInformationDP1(eipStation, this, ref conn);
                    }

                    else if (eipStation.PlcType == PlcTypes.Micro800_series)
                    {
                        if (!EtherNetIpProtocol.Logix5000V21GetTagInstances800(eipStation, ref requestBuffer, ref requestBufferPointer, this))
                        {
                            //Error connection
                            CommDriver.OnSystemEvent(ObjectIds.Server, EtherNetIP.Properties.Resources.ErrorRegisterSession, Opc.Ua.EventSeverity.High);
                        }
                        else
                        {
                            //Logix5550NonBlockPhAddBuildInfoMaps(eipStation, ref requestBuffer, ref requestBufferPointer);
                            conn = (int)DriverErrorCodes.ErrorNoError;
                        }
                    }
                }
            }

            //}

            return conn;
        }


        //public override bool ManageConnectionError()
        //{
        //    ListJobPending.AddRange(nextlist);
        //    clearChannel();
        //    return true;
        //}
        //public override bool DeviceOpen()
        //{
        //    if (!IsDeviceOpen())
        //    {
        //        if (!base.DeviceOpen())
        //        {
        //            return false;
        //        }
        //    }

        //    if (!EtherNetIpProtocol.RegisterSession(ref requestBuffer, ref requestBufferPointer, this))
        //    {
        //        //Error connection
        //        CommDriver.OnSystemEvent(ObjectIds.Server, EtherNetIP.Properties.Resources.ErrorRegisterSession, Opc.Ua.EventSeverity.High);
        //        DeviceClose();
        //        return false;
        //    }

        //    return true;
        //}

        //List<EtherNetIPCommJob> nextlist = new List<EtherNetIPCommJob>();

        protected override void OnJobExecuted(ExecutedJobArgs e)
        {            
            EtherNetIPCommJob j = e.Job as EtherNetIPCommJob;
            //j.ExecuteTask = false;
            if (!j.TerminateTask)
            {
                j.ReadTagStart = 0;
                j.ReadTagEnd = 0;
                j.PartialArrayStart = 0;
                j.PartialArrayEnd = 0;
                j.TerminateTask = true;            
                base.OnJobExecuted(e);
            }
        }
        public bool SessionRegisterOk()
        {
            return SessionHandle.UINT != 0;
        }

        public void ClearSession()
        {
            SessionHandle.UINT = 0;

            InvalidateStationConnections();        
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Queries if a device is open. </summary>
        ///
        /// <returns>   true if a device is open, false if not. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool EtherNetIPIsDeviceOpen()
        {
            if (!IsDeviceOpen())
                return false;

            if (!SessionRegisterOk())
            {
                return false;
            }

            return true;
        }

        private IDataLayer GetOptimizedTagsMapFile(string stationName, out string filebase, out InMemoryDataStore InMemory, out bool targetIsFile)
        {            
            EtherNetIPDriver EIPCommDriver = (EtherNetIPDriver)CommDriver;
            
            return GetOptimizedTagsMapFile(stationName, EIPCommDriver.StrConnectionString, out filebase, out InMemory, out targetIsFile);
        }

        //private IDataLayer GetSpecificDataLayer(string conn, out string filebase, out InMemoryDataStore InMemory, out bool targetIsFile)
        //{
        //    IDataLayer dl = null;
        //    InMemory = null;
        //    filebase = string.Empty;
        //    ConnectionStringParser helper = new ConnectionStringParser(conn);
        //    string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);

        //    targetIsFile = false;

        //    if (providerType != InMemoryDataStore.XpoProviderTypeString)
        //    {
        //        dl = XpoDefault.GetDataLayer(conn, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
        //        targetIsFile = false;
        //    }
        //    else// if (xml)
        //    {
        //        filebase = XpoHelpers.XpoHelper.GetDataSourceFilePath(conn);
        //        InMemory = CommunicationDriver.GetDataStore(filebase);
        //        if (!string.IsNullOrWhiteSpace(filebase) && InMemory != null)
        //        {
        //            dl = new SimpleDataLayer(InMemory);
        //            targetIsFile = true;
        //        }
        //    }

        //    return dl;
        //}

        public static string GetOptimizedTagsMapFileNameOnly(string stationName)
        {
            return string.Format("{0}_EtherNetIPOptimizedTagsMap", stationName);
        }

        private IDataLayer GetOptimizedTagsMapFile(string stationName, string targerConn, out string filebase, out InMemoryDataStore InMemory, out bool targetIsFile)
        {
            EtherNetIPDriver EtherNetIPCommDriver = (EtherNetIPDriver)CommDriver;
            string FileName = GetOptimizedTagsMapFileNameOnly(stationName);
            string connect = CommunicationDriver.GetConnectionString(targerConn, "Drivers", FileName, ".xml");
            IDataLayer dl = CommunicationDriver.GetSpecificDataLayer(connect, out filebase, out InMemory, out targetIsFile);

            // from db project, use os user temporary path
            if (!targetIsFile)
                filebase = Path.Combine(Path.GetTempPath(), string.Format("{0}{1}", FileName, ".xml"));

            return (dl);
        }

        private bool CheckAttributesIsTrueLoadMaps(EtherNetIPStation s, ushort attribute1, ushort attribute2, uint attribute3, uint attribute4, uint attribute10) {
            
            bool Result = false;
            string FileMap;
            InMemoryDataStore InMemory;
            bool TargetIsFile;

            using (IDataLayer idl = GetOptimizedTagsMapFile(s.Name, out FileMap, out InMemory, out TargetIsFile))
            {
                if (idl == null)
                    return Result;

                using (UnitOfWork ufw = new UnitOfWork(idl))
                {
                    // get tia file "body" by station
                    var Maps = (from S in new XPQuery<EtherNetIPOptimizedTagsMap>(ufw).AsParallel() where S.StationName == s.Name select S).ToList();
                    EtherNetIPOptimizedTagsMap Map = (from S in new XPQuery<EtherNetIPOptimizedTagsMap>(ufw).AsParallel() where S.StationName == s.Name select S).ToList().FirstOrDefault();
                    if (Map != null)
                    {
                        //Check the settings with the last save
                        Result = (attribute1 == Map.Attribute1 && attribute2 == Map.Attribute2 && attribute3 == Map.Attribute3 && attribute4 == Map.Attribute4 && attribute10 == Map.Attribute10 && s.FirmwareVersion == Map.FirmwareVersion);
                        if (Result)
                        {
                            try
                            {
                                using (var ostrm = new MemoryStream(Map.ListProgramsTrueIstances))
                                {
                                    using (var reader = XmlDictionaryReader.CreateBinaryReader(ostrm, XmlDictionaryReaderQuotas.Max))
                                    {
                                    
                                        var formatter = new DataContractSerializer(typeof(List<ushort>));
                                        s.m_listProgramsTrueIstances = formatter.ReadObject(reader) as List<ushort>;
                                    }
                                }                            

                                using (var ostrm = new MemoryStream(Map.ProgramList))
                                {                                
                                    using (var reader = XmlDictionaryReader.CreateBinaryReader(ostrm, XmlDictionaryReaderQuotas.Max))
                                    {
                                        var formatter = new DataContractSerializer(typeof(List<string>));
                                        s.m_listPrograms = formatter.ReadObject(reader) as List<string>;
                                    }
                                }

                                using (var ostrm = new MemoryStream(Map.MapProgramsInstances))
                                {
                                    using (var reader = XmlDictionaryReader.CreateBinaryReader(ostrm, XmlDictionaryReaderQuotas.Max))
                                    {
                                        var formatter = new DataContractSerializer(typeof(Dictionary<string, ushort>));
                                        s.m_mapProgramsInstances = formatter.ReadObject(reader) as Dictionary<string, ushort>;
                                    }
                                }

                                using (var ostrm = new MemoryStream(Map.MapProgramsAddresses))
                                {
                                    using (var reader = XmlDictionaryReader.CreateBinaryReader(ostrm, XmlDictionaryReaderQuotas.Max))
                                    {
                                        var formatter = new DataContractSerializer(typeof(Dictionary<uint, ushort>));
                                        s.m_mapProgramsAddresses = formatter.ReadObject(reader) as Dictionary<uint, ushort>;
                                    }
                                }

                                using (var ostrm = new MemoryStream(Map.MapPlcTagInstanceInfo))
                                {
                                    using (var reader = XmlDictionaryReader.CreateBinaryReader(ostrm, XmlDictionaryReaderQuotas.Max))
                                    {
                                        var formatter = new DataContractSerializer(typeof(Dictionary<string, PlcTagInstanceInfo>));
                                        s.m_mapPlcTagInstanceInfo = formatter.ReadObject(reader) as Dictionary<string, PlcTagInstanceInfo>;
                                    }
                                }

                                using (var ostrm = new MemoryStream(Map.MapPlcTemplateInfo))
                                {
                                    using (var reader = XmlDictionaryReader.CreateBinaryReader(ostrm, XmlDictionaryReaderQuotas.Max))
                                    {
                                        var formatter = new DataContractSerializer(typeof(Dictionary<ushort, PlcTemplateInfo>));
                                        s.m_mapPlcTemplateInfo = formatter.ReadObject(reader) as Dictionary<ushort, PlcTemplateInfo>;
                                    }
                                }

                                //using (var ostrm = new MemoryStream(Map.TemplateBuffer))
                                //{
                                //    using (var reader = XmlDictionaryReader.CreateBinaryReader(ostrm, XmlDictionaryReaderQuotas.Max))
                                //    {
                                //        var formatter = new DataContractSerializer(typeof(byte[]));
                                //        s.m_TemplateBuffer = formatter.ReadObject(reader) as byte[];
                                //    }
                                //}
                            }
                            catch (Exception ex)
                            {
                                Result = false;
                            }
                        }

                        if (Result)
                            s.BuildInfoMaps = true;
                        else
                            s.Logix5550NonBlockPhAddEmptyInfoMaps();

                        //{
                        //    //# ifndef _WIN32_WCE
                        //    //(nTypeOS != OS_WINDOWS_DESKTOP) ||
                        //    //#else
                        //    //        (nTypeOS != OS_WINDOWS_CE) ||
                        //    //#endif

                        //    //|| (bFileLogix5550NonBlockPhAdd != (BYTE)m_bLogix5550NonBlockPhAdd) 
                        //    //|| (bFileLogix5550BlockingPhAdd != (BYTE)m_bLogix5550BlockingPhAdd))

                        //    Result = true;
                        //}
                    }
                }
            }

            return Result;
        }

        private bool Logix5550NonBlockPhAddBuildInfoMaps(EtherNetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer)
        {
            if (s.BuildInfoMaps == false)
            {
                if (s.ConnectionIDSet)
                {
                    if (s.Logix5550NonBlockPhAdd == PhisicalAddressesOptimizzations.True && s.PlcType != PlcTypes.Micro800_series)
                    {
                        s.Logix5550NonBlockPhAddEmptyInfoMaps();                        
                        if (s.FirmwareVersion >= 21)
                        {
                            // retrive all information about PLC program
                            if (EtherNetIpProtocol.Logix5000V21ReadAttributes(s, ref requestBuffer, ref requestBufferPointer, this, out ushort Attribute1, out ushort Attribute2, out uint Attribute3, out uint Attribute4, out uint Attribute10))
                            {
                                // compare PLC program info with last saved plc program info --> if is the same, load from storage
                                if (!CheckAttributesIsTrueLoadMaps(s, Attribute1, Attribute2, Attribute3, Attribute4, Attribute10))
                                {                                    
                                    // get all PLC program info (--> PLC Tags)
                                    if (EtherNetIpProtocol.Logix5000V21BuildInstancesMaps(s, ref pdu, ref pduPointer, this))
                                    {
                                        s.BuildInfoMaps = true;
                                        // save all data to storage
                                        StorageOptimizedTagsMap(s, Attribute1, Attribute2, Attribute3, Attribute4, Attribute10);
                                    }                                    
                                }                                
                            }
                        }
                    }
                }
            }
            return (s.BuildInfoMaps);
        }

        public bool EtherNetIPDeviceOpen()
        {
            if (!IsDeviceOpen())
            {
                ClearSession();
                if (!DeviceOpen())
                {
                    return false;
                }
            }

            if (!EtherNetIpProtocol.RegisterSession(ref requestBuffer, ref requestBufferPointer, this))
            {
                //Error connection
                CommDriver.OnSystemEvent(ObjectIds.Server, EtherNetIP.Properties.Resources.ErrorRegisterSession, Opc.Ua.EventSeverity.High);
                DeviceClose();
                return false;
            }

            return true;
        }

        public override bool DeviceClose()
        {
 
            EtherNetIpProtocol.UnRegisterSession(ref requestBuffer, ref requestBufferPointer, this);

            return base.DeviceClose();
        }

        private void StorageOptimizedTagsMap(EtherNetIPStation s, ushort attribute1, ushort attribute2, uint attribute3, uint attribute4, uint attribute10)
        {
            string FileMap;
            InMemoryDataStore InMemory;
            bool TargetIsFile;

            using (IDataLayer idl = GetOptimizedTagsMapFile(s.Name, out FileMap, out InMemory, out TargetIsFile))
            {
                if (idl == null)
                    return;

                using (UnitOfWork ufw = new UnitOfWork(idl))
                {
                    // get tag list from selected station
                    List<EtherNetIPOptimizedTagsMap> Maps = (from S in new XPQuery<EtherNetIPOptimizedTagsMap>(ufw).AsParallel() where S.StationName == s.Name select S).ToList();
                    if (Maps.Count > 0)
                    {
                        // before to import delete previous
                        ufw.Delete(Maps);
                        ufw.CommitChanges();
                    }

                    // get tia file "body" by station
                    EtherNetIPOptimizedTagsMap Map = new EtherNetIPOptimizedTagsMap(ufw);
                    Map.Attribute1 = attribute1;
                    Map.Attribute2 = attribute2;
                    Map.Attribute3 = attribute3;
                    Map.Attribute4 = attribute4;
                    Map.Attribute10 = attribute10;
                    Map.FirmwareVersion = s.FirmwareVersion;
                    //|| (bFileLogix5550NonBlockPhAdd != (BYTE)m_bLogix5550NonBlockPhAdd) 
                    //|| (bFileLogix5550BlockingPhAdd != (BYTE)m_bLogix5550BlockingPhAdd))

                    Map.StationName = s.Name;
                    Map.LastUpdate = DateTime.UtcNow;
                                        
                    using (var ostrm = new MemoryStream())
                    {
                        using (var writer = XmlDictionaryWriter.CreateBinaryWriter(ostrm))
                        {
                            try
                            {
                                var serializer = new DataContractSerializer(typeof(List<ushort>));
                                serializer.WriteObject(writer, s.m_listProgramsTrueIstances);
                            }
                            finally
                            {
                                writer.Close();
                            }
                        }

                        Map.ListProgramsTrueIstances = ostrm.ToArray();
                    }

                    using (var ostrm = new MemoryStream())
                    {
                        using (var writer = XmlDictionaryWriter.CreateBinaryWriter(ostrm))
                        {
                            try
                            {
                                var serializer = new DataContractSerializer(typeof(List<string>));
                                serializer.WriteObject(writer, s.m_listPrograms);
                            }
                            finally
                            {
                                writer.Close();
                            }
                        }

                         Map.ProgramList = ostrm.ToArray();
                    }

                    using (var ostrm = new MemoryStream())
                    {
                        using (var writer = XmlDictionaryWriter.CreateBinaryWriter(ostrm))
                        {
                            try
                            {
                                var serializer = new DataContractSerializer(typeof(Dictionary<string, ushort>));
                                serializer.WriteObject(writer, s.m_mapProgramsInstances);
                            }
                            finally
                            {
                                writer.Close();
                            }
                        }

                        Map.MapProgramsInstances = ostrm.ToArray();
                    }

                    using (var ostrm = new MemoryStream())
                    {
                        using (var writer = XmlDictionaryWriter.CreateBinaryWriter(ostrm))
                        {
                            try
                            {
                                var serializer = new DataContractSerializer(typeof(Dictionary<uint, ushort>));
                                serializer.WriteObject(writer, s.m_mapProgramsAddresses);
                            }
                            finally
                            {
                                writer.Close();
                            }
                        }

                        Map.MapProgramsAddresses = ostrm.ToArray();
                    }

                    using (var ostrm = new MemoryStream())
                    {
                        using (var writer = XmlDictionaryWriter.CreateBinaryWriter(ostrm))
                        {
                            try
                            {
                                var serializer = new DataContractSerializer(typeof(Dictionary<string, PlcTagInstanceInfo>));
                                serializer.WriteObject(writer, s.m_mapPlcTagInstanceInfo);
                            }
                            finally
                            {
                                writer.Close();
                            }
                        }

                        Map.MapPlcTagInstanceInfo = ostrm.ToArray();
                    }

                    using (var ostrm = new MemoryStream())
                    {
                        using (var writer = XmlDictionaryWriter.CreateBinaryWriter(ostrm))
                        {
                            try
                            {
                                var serializer = new DataContractSerializer(typeof(Dictionary<ushort, PlcTemplateInfo>));
                                serializer.WriteObject(writer, s.m_mapPlcTemplateInfo);
                            }
                            finally
                            {
                                writer.Close();
                            }
                        }

                        Map.MapPlcTemplateInfo = ostrm.ToArray();
                    }

                    //using (var ostrm = new MemoryStream())
                    //{
                    //    using (var writer = XmlDictionaryWriter.CreateBinaryWriter(ostrm))
                    //    {
                    //        try
                    //        {
                    //            var serializer = new DataContractSerializer(typeof(byte[]));
                    //            serializer.WriteObject(writer, s.m_TemplateBuffer);
                    //        }
                    //        finally
                    //        {
                    //            writer.Close();
                    //        }
                    //    }

                    //    Map.TemplateBuffer = ostrm.ToArray();
                    //}                    

                    // commit changes
                    ufw.CommitChanges();

                    if (TargetIsFile)
                        InMemory.WriteXml(FileMap);
                }
            }
        }

        #endregion

        #region methods        

        public override bool IsScheduledJobsListFull(List<CommJob> jobList)
        {
            return false;
        }

        public override void SplitInExecutionLists(List<CommJob> jobList, ref List<List<CommJob>> exList)
        {          
            int jobIndex = 0;
            while (jobIndex < jobList.Count())
            {
                bool write = false;
                bool first = true;
                string station = string.Empty;
                bool process = true;
                List<CommJob> list = new List<CommJob>();
                EtherNetIpProtocol.ReadWriteListLimitateSize frameSize = new EtherNetIpProtocol.ReadWriteListLimitateSize();
                                
                while (jobIndex < jobList.Count())
                {                    
                    EtherNetIPCommJob j = jobList.ElementAt(jobIndex) as EtherNetIPCommJob;                    
                    // try to aggregate more jobs
                    if (first)
                    {
                        write = !j.ReadRequest();
                        station = j.Station.Name;
                        first = false;
                    }

                    process = true;
                    //InputOut Job with Movicon data type smaller than plc data type --> force 1 read cycle, and than write
                    // for DataFile data type or Micro800_series support only one job at time
                    if (j.IsSingleWriteInputOutputSmallToBig() || (j.AddressType == AddressTypes.DataFile || (j.Station as EtherNetIPStation).PlcType == PlcTypes.Micro800_series))
                    {
                        process = false;
                        // if no jobs were "aggregated" before, create a "special" list with single job
                        if (list.Count == 0)
                        {
                            list.Add(j);
                            jobList.RemoveAt(jobIndex);
                            break;
                        }
                    }

                    if (process)
                    {
                        // aggregate job of the same statation
                        if (j.Station.Name == station)
                        {
                            // write
                            if (write && !j.ReadRequest())
                            {
                                if (EtherNetIpProtocol.getWriteListLimitate(ref j, ref frameSize, EtherNetIpProtocol.ReadWriteLimitate.EvaluateOnly))
                                {
                                    frameSize.TotalNrJobs++;
                                    list.Add(j);
                                    jobList.RemoveAt(jobIndex);
                                    jobIndex--;
                                }
                                else
                                {
                                    // job exceed maximum frame size
                                    break;
                                }
                            }// read
                            else if (!write && j.ReadRequest())
                            {
                                if (EtherNetIpProtocol.getReadListLimitate(ref j, ref frameSize, EtherNetIpProtocol.ReadWriteLimitate.EvaluateOnly))
                                {
                                    frameSize.TotalNrJobs++;
                                    list.Add(j);
                                    jobList.RemoveAt(jobIndex);
                                    jobIndex--;
                                }
                                else
                                {
                                    // job exceed maximum frame size
                                    break;
                                }
                            }
                        }
                    }
                    jobIndex++;
                }

                if (list.Count > 0)
                {
                    Parallel.ForEach(list, j =>
                    {
                        ((EtherNetIPCommJob)j).Init(write);
                    });
                    exList.Add(list);
                    jobIndex = 0;
                }
            }
        }

        //public void clearChannel()
        //{
        //    lock (lockThreadObject)
        //    {
        //        //foreach (EtherNetIPCommJob j in nextlist)
        //        //{
        //        //    j.ExecuteTask = false;
        //        //}
        //        //nextlist.Clear();
        //        if (ListJobPending.Count > 0)
        //        {
        //            LastErrorCode = DriverErrorCodes.ErrorTimeOut;
        //            ProcessNewDataList(ListJobPending);
        //            ListJobPending.Clear();
        //        }
        //        //NextScheduleTimeJobsList = DateTime.UtcNow;
        //        LastErrorCode = DriverErrorCodes.ErrorNoError;
        //    }
        //}

        public override bool ExecuteJobList(ref DriverErrorCodes conn, List<CommJob> exjoblist)
        {
            if (conn != DriverErrorCodes.ErrorNoError)
            {
                conn = DriverErrorCodes.ErrorTimeOut;
                return false;                
            }

            #region special case
            if (exjoblist.Count == 1)
            {
                EtherNetIPCommJob j = exjoblist[0] as EtherNetIPCommJob;
                //write a part of variable value(generally bit into IntXX); read before write
                if (j.IsSingleWriteInputOutputSmallToBig())
                {
                    if (j.GetTagListOnWritingCount() > 0)
                    {
                        // 1st loop read data
                        if (j.RWState == CommJob.RWStates.Standard)
                            j.RWState = CommJob.RWStates.ReadForRW;
                        //else //2nd loop write data; 
                        //    RWState = CommJob.RWStates.WriteForRW;    --> done automatically in DriveCodeBase after job execution in  CommJob.RWStates.ReadForRW
                    }
                    j.Init(j.RWState == CommJob.RWStates.WriteForRW);
                }
                else
                {
                    // manage big job (bi array)
                    EtherNetIpProtocol.ReadWriteListLimitateSize requestSize = new EtherNetIpProtocol.ReadWriteListLimitateSize();
                    // write
                    if (!j.ReadRequest())
                        EtherNetIpProtocol.getWriteListLimitate(ref j, ref requestSize, EtherNetIpProtocol.ReadWriteLimitate.Add);
                    else
                        EtherNetIpProtocol.getReadListLimitate(ref j, ref requestSize, EtherNetIpProtocol.ReadWriteLimitate.Add);
                }
            }
            #endregion

            ExecuteJobList(exjoblist);

            return (exjoblist.Count>0);
        }

        //protected override bool SetSynchroJobData(CommJob exjob, NodeId tagNodeId = null, object value = null)
        //{
        //    bool bRet = base.SetSynchroJobData(exjob, tagNodeId, value);
        //    if (bRet)
        //    {
        //        exjob.UpdateTagsListOnWriting();
        //        List<CommJob> wList = new List<CommJob>() { exjob };
        //        // if was scheduled before, remove from SchedulingQueue
        //        if (wList[0].IsQueued)
        //            RemoveScheduledJobs(exjob.Station, wList);
        //        List<List<CommJob>> exList = new List<List<CommJob>>();
        //        // use SplitInExecutionLists method to initialize internal's read/write parameters
        //        SplitInExecutionLists(wList, ref exList);
        //    }
        //    return bRet;
        //}

        //public override List<CommJob> ExecuteJobList(ref List<CommJob> list)
        //{
        //    List<EtherNetIPCommJob> currExecList = list.ConvertAll(x => (EtherNetIPCommJob)x);

        //    #region special case
        //    if (currExecList.Count == 1)
        //    {
        //        EtherNetIPCommJob j = currExecList[0];
        //        //write a part of variable value(generally bit into IntXX); read before write
        //        if (j.IsSingleWriteInputOutputSmallToBig())
        //        {
        //            if (j.TagsListToWrite.Count > 0)
        //            {
        //                // 1st loop read data
        //                if (j.RWState == CommJob.RWStates.Standard)
        //                    j.RWState = CommJob.RWStates.ReadForRW;
        //                //else //2nd loop write data; 
        //                //    RWState = CommJob.RWStates.WriteForRW;    --> done automatically in DriveCodeBase after job execution in  CommJob.RWStates.ReadForRW
        //            }
        //            j.Init(j.RWState == CommJob.RWStates.WriteForRW);
        //        }
        //        else
        //        {
        //            // manage big job (bi array)
        //            EtherNetIpProtocol.ReadWriteListLimitateSize requestSize = new EtherNetIpProtocol.ReadWriteListLimitateSize();
        //            // write
        //            if (!j.ReadRequest())
        //                EtherNetIpProtocol.getWriteListLimitate(ref j, ref requestSize, EtherNetIpProtocol.ReadWriteLimitate.Add);
        //            else
        //                EtherNetIpProtocol.getReadListLimitate(ref j, ref requestSize, EtherNetIpProtocol.ReadWriteLimitate.Add);
        //        }
        //    }
        //    #endregion

        //    ExecuteJobList(ref currExecList);

        //    return list;
        //}

        private void ExecuteJobList(List<CommJob> list)
        {
#if DEBUG
            System.Diagnostics.Trace.TraceInformation(string.Format("{1} ExecuteJobList begin {0}.{2}", DateTime.Now.ToLongTimeString(), Thread.CurrentThread.ManagedThreadId, DateTime.Now.Millisecond));
#endif           
            DateTime ExecutionTime = DateTime.UtcNow;            
            Parallel.ForEach(list, j =>
            {
                base.ExecuteJob(j);
                j.LastExecutionTime = ExecutionTime;
                j.StartExecutionTime = ExecutionTime;
            });

            EtherNetIPStation s = ((EtherNetIPCommJob)list[0]).Station as EtherNetIPStation;
            if (s == null)
                return;
#if DEBUG
            if (list[0].SyncroExec)
                System.Diagnostics.Trace.TraceInformation("{0} Execute Synchro", Thread.CurrentThread.ManagedThreadId);
#endif
            ReceiveClear();
            switch (((EtherNetIPCommJob)list[0]).AddressType)
            {
                case AddressTypes.DataFile:
                    if (EtherNetIpProtocol.PrepareDataFileRequest(((EtherNetIPCommJob)list[0]), ref requestBuffer, ref requestBufferPointer) == 0)
                    {                        
                        RemovePendingJob(list[0]);
                        list.RemoveAt(0);
                        return;
                    }
                    if (!EtherNetIpDeviceWrite(ref requestBuffer, ref requestBufferPointer))
                    {
                        DeviceClose();
                        return;
                    }
                    break;

                case AddressTypes.TagName:
#if DEBUG
                    if (list[0].SyncroExec)
                        System.Diagnostics.Trace.TraceInformation("{0} ExecuteJobList Synchro {1}.{2}", Thread.CurrentThread.ManagedThreadId, DateTime.Now.ToLongTimeString(), DateTime.Now.Millisecond);
#endif
                    if (EtherNetIpProtocol.Logix5550PrepareRequest(ref list, this, ref s, ref requestBuffer, ref requestBufferPointer) == 0)
                    {
                        foreach (EtherNetIPCommJob j in list)
                        {                            
                            RemovePendingJob(j);
                        }
                        list.Clear();
                        return;
                    }

                    if (!EtherNetIpDeviceWrite(ref requestBuffer, ref requestBufferPointer))
                    {
                        DeviceClose();
                        return;
                    }

                    break;
            }

            BeginDeviceRead(EtherNetIpProtocol.ENCAPSULATION_HEADER_SIZE);
#if DEBUG          
            System.Diagnostics.Trace.TraceInformation(string.Format("{1} ExecuteJobList ends {0}.{2}", DateTime.Now.ToLongTimeString(), Thread.CurrentThread.ManagedThreadId, DateTime.Now.Millisecond));
#endif
        }

        bool AtLeastOneStationIsConnected()
        {
            List<Station> connectedStationList = (from station in CommDriver.GetChannelStations(this).AsParallel()
                                                  where (((EtherNetIPStation)station).ConnectionIDSet == true)
                                                  select station).ToList();
            return (connectedStationList.Count() > 0);
        }

        public void InvalidateStationConnections()
        {
            Parallel.ForEach(CommDriver.GetChannelStations(this), s =>
            {
                ((EtherNetIPStation)s).ClearConnection();
            });
            Parallel.ForEach(CommDriver.GetChannelStations(this), s =>
            {
                ((EtherNetIPStation)s).BuildInfoMaps = false;
            });
        }

        public override bool ProcessNewDataList(DriverErrorCodes conn, List<CommJob> list)
        {
            //System.Diagnostics.Trace.TraceInformation(string.Format("{0} ProcessNewDataList begin", DateTime.Now.ToLongTimeString()));
            if (conn != DriverErrorCodes.ErrorNoError)
            {
                if (conn == DriverErrorCodes.ErrorTimeOut)
                {
                    ((EtherNetIPStation)list[0].Station).ClearConnection();
                    if (list[0].Station.LastErrorCode != DriverErrorCodes.ErrorTimeOut)
                    {
                        ClearSession();
                        DeviceClose();
                    }
                }

                foreach (EtherNetIPCommJob j in list)
                {
                    j.TerminateTask = false;
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = conn, Job = j };
                    OnJobExecuted(eJob);
                }                
                ReceiveClear();
                return true;
            }            

            if (list.Count != 0)
            {
                bool bRet = false;
                byte[] pdu = null;
                byte[] pduData = null;
                ushortUnion dim = new ushortUnion();
                EtherNetIPStation s = ((EtherNetIPCommJob)list[0]).Station as EtherNetIPStation;
                if (s != null)
                {
                    lock (lockThreadObject)
                    {
                        if (ReceiveBuffer.Count >= EtherNetIpProtocol.ENCAPSULATION_HEADER_SIZE)
                        {
                            dim = new ushortUnion(ReceiveBuffer, EtherNetIpProtocol.EDATA_LEN_OFFS);
                            if (dim.USHORT != 0)
                            {
                                pdu = new byte[EtherNetIpProtocol.ENCAPSULATION_HEADER_SIZE + dim.USHORT];
                                pduData = new byte[dim.USHORT];
                                ReceiveBuffer.CopyTo(pdu, 0);

                                bRet = true;
                            }
                        }                        
                    }                

                    if (bRet)
                    {
                        if (!DeviceRead(pduData, (uint)dim.USHORT)) //error timeout RX
                        {
                            // wait and try to read data again
                            Thread.Sleep(1);
                            if (!DeviceRead(pduData, (uint)dim.USHORT))
                                bRet = false;
                        }
                        
                        if (bRet)
                        {
                            pduData.CopyTo(pdu, EtherNetIpProtocol.ENCAPSULATION_HEADER_SIZE);
                            switch (((EtherNetIPCommJob)list[0]).AddressType)
                            {
                                case AddressTypes.DataFile:
                                    if (testDataFileTransaction(ref pdu, ((EtherNetIPCommJob)list[0]), s))
                                    {
                                        if (((EtherNetIPCommJob)list[0]).CommandType == CommandTypes.ReadCmd)
                                        {
                                            GetReadData(ref pdu, ((EtherNetIPCommJob)list[0]), s);
                                        }
                                        else
                                        {
                                            LastErrorMessage = "";
                                            ExecutedJobArgs eJob = new ExecutedJobArgs();
                                            eJob.Job = list[0];
                                            OnJobExecuted(eJob);
                                        }
                                    }

                                    break;

                                case AddressTypes.TagName:
                                    if (testTagNameTransaction(ref pdu, list, s))
                                    {
                                        if (((EtherNetIPCommJob)list[0]).CommandType == CommandTypes.ReadCmd)
                                        {
                                            GetReadData(ref pdu, list, s);
                                        }
                                        else
                                        {
                                            LastErrorMessage = "";
                                            CheckTheWritingResponse(ref pdu, list, s);
                                        }
                                    }
                                    else
                                    {
                                        EtherNetIpProtocol.Logix5550ForwardClose(s, ref requestBuffer, ref requestBufferPointer, this);
                                        s.ClearConnection();
                                    }
                                    break;
                            }
                        }
                    }
                    if (bRet == false)
                    {
                        EtherNetIpProtocol.Logix5550ForwardClose(s, ref requestBuffer, ref requestBufferPointer, this);
                        s.ClearConnection();
                        foreach (EtherNetIPCommJob j in list)
                        {
                            ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)EtherNetIpErrorCodes.ErrorNoRep, Job = j };
                            OnJobExecuted(eJob);
                        }
                    }
                }
            }

            ReceiveClear();
            //System.Diagnostics.Trace.TraceInformation(string.Format("{0} ProcessNewDataList ends", DateTime.Now.ToLongTimeString()));
            return true;
        }

        private bool testDataFileTransaction(ref byte[] pdu, EtherNetIPCommJob mJob, EtherNetIPStation s)
        {
            if (s.GetTransaction().USHORT == new ushortUnion(pdu, EtherNetIpProtocol.PCCC_RESP_TNS_OFFS).USHORT)
            {
                s.LastTransOK = AnalyzeResponse(ref pdu, s, mJob);
            }
            else
            {
                //CommDriver.OnSystemEvent(ObjectIds.Server, EtherNetIP.Properties.Resources.ErrorWrongTransaction, Opc.Ua.EventSeverity.High);
                ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)EtherNetIpErrorCodes.ErrorWrongTransaction, Job = mJob };
                OnJobExecuted(eJob);
                s.LastTransOK = false;
            }
            return s.LastTransOK;
        }

        private bool testTagNameTransaction(ref byte[] pdu, List<CommJob> list, EtherNetIPStation s)
        {
            if (s.GetTransaction().USHORT == new ushortUnion(pdu, EtherNetIpProtocol.LOGIX5550_REP_TNS_OFFS).USHORT)
            {
                if (s.PlcType != PlcTypes.Micro800_series)
                    s.LastTransOK = Logix5550TagsCheckReply(ref pdu, s, list);
                else
                    s.LastTransOK = Micro800TagsCheckReply(ref pdu, s, list);
            }
            else
                s.LastTransOK = false;

            if (!s.LastTransOK)
                foreach (EtherNetIPCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)EtherNetIpErrorCodes.ErrorWrongTransaction, Job = j };
                    OnJobExecuted(eJob);
                }
            return s.LastTransOK;
        }


        void GetReadData(ref byte[] pdu,List<CommJob> list, EtherNetIPStation s)
        {
            if (s.PlcType == PlcTypes.Micro800_series)
            {
                GetReadDataMicro800(ref pdu, list, s);
                return;
            }            
            
            ushort readJobTag;
            ushort totalPduTag = 0;
            byte[] pduLocal = pdu;
            List<ExecutedJobArgs> LocalListExecutedJob = new List<ExecutedJobArgs>();   // list jobs executed
            
            Parallel.ForEach(list, (jComm, state, index) => 
            {
                EtherNetIPCommJob j = ((EtherNetIPCommJob)jComm);
                ushort tagIndex = (ushort)index;
                ushort ExtendedError = 0;
                byte GeneralStatusError = 0;
                readJobTag = 0;
                DriverErrorCodes outErrorCode = copyTagData(j.TagsList[0], pduLocal, (ushort)(totalPduTag + tagIndex - j.ReadTagStart), j, out GeneralStatusError, out ExtendedError);
                totalPduTag += readJobTag;
                ExecutedJobArgs eJob = null;
                if (outErrorCode == DriverErrorCodes.ErrorNoError)
                {                    
                    ((EtherNetIPCommJob)j).ReadTagStart = j.ReadTagEnd;
                    j.PartialArrayStart = j.PartialArrayEnd;
                    eJob = new ExecutedJobArgs { ErrorCode = DriverErrorCodes.ErrorNoError, Job = j };
                    eJob.Values = j.answer;

                    lock (LocalListExecutedJob)
                    {
                        if (j.IsSingleWriteInputOutputSmallToBig())
                        {
                            LocalListExecutedJob.Add(eJob);
                        }
                        else
                        {
                            // if is not a big job (if it is, was completed)
                            if (j.ReadTagStart == 0 && j.ReadTagEnd == 0 && j.PartialArrayStart == 0 && j.PartialArrayEnd == 0)
                                LocalListExecutedJob.Add(eJob);
                        }
                    }
                }
                else
                {
                    j.ReadTagStart = 0;
                    j.ReadTagEnd = 0;
                    j.PartialArrayStart = 0;
                    j.PartialArrayEnd = 0;
                    eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)EtherNetIpErrorCodes.ErrorEnbeddedStation, Job = j };
                    if (j.InErrorState == false)
                    {
                        s.GetCommDriver().OnSystemEvent(null,
                                                            string.Format(Properties.Resources.ErrorEnbeddedError, GeneralStatusError, ExtendedError, j.TagName),
                                                            Opc.Ua.EventSeverity.Max);
                    }

                    lock (LocalListExecutedJob)
                    {
                        LocalListExecutedJob.Add(eJob);
                    }
                }                
            });

            foreach (ExecutedJobArgs eJob in LocalListExecutedJob)
            {
                OnJobExecuted(eJob);
            }
        }

        void GetReadDataMicro800(ref byte[] pdu, List<CommJob> list, EtherNetIPStation s)
        {
            EtherNetIPCommJob j = (EtherNetIPCommJob)list[0];
            Tag tag = (Tag)j.TagsList[0];
            DriverErrorCodes outErrorCode = copyTagDataMicro800(tag, ref pdu, j);

            if (outErrorCode == DriverErrorCodes.ErrorNoError)
            {
                //long array case --> if not all data were readed, don't close job
                if (j.PartialArrayStart != 0 || j.PartialArrayEnd != 0)
                    return;
            }

            ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = outErrorCode, Job = j };
            if (outErrorCode == DriverErrorCodes.ErrorNoError)            
                eJob.Values = j.answer;
            
            OnJobExecuted(eJob);
        }

        private DriverErrorCodes copyTagData(Tag pendingTag,  byte[] pdu, ushort tagIndex, EtherNetIPCommJob j, out byte GeneralStatusError, out ushort ExtendedError)
        {
            GeneralStatusError = 0;
            ExtendedError = 0;
            ushortUnion responseTag = new ushortUnion(pdu, EtherNetIpProtocol.LOGIX5550_REP_SERNUM_OFFS);
            if (tagIndex >= responseTag.USHORT)
            {
                return (DriverErrorCodes)EtherNetIpErrorCodes.ErrorRepTooShort;
            }
            ushortUnion ReplyOffset = new ushortUnion(pdu, (ushort)(EtherNetIpProtocol.LOGIX5550_REP_SERNUM_OFFS + tagIndex * 2 + 2));
            ReplyOffset.USHORT += (EtherNetIpProtocol.LOGIX5550_REP_SERNUM_OFFS);
            if (ReplyOffset.USHORT >= pdu.Count())
            {
                return (DriverErrorCodes)EtherNetIpErrorCodes.ErrorRepTooShort;
            }

            ushort ReplyLength;
            if (tagIndex == responseTag.USHORT - 1)
            {
                ReplyLength = (ushort)(pdu.Count() - ReplyOffset.USHORT);
            }
            else
            {
                ushortUnion nextReplyOffset = new ushortUnion(pdu, (ushort)(EtherNetIpProtocol.LOGIX5550_REP_SERNUM_OFFS + tagIndex * 2 + 4));
                nextReplyOffset.USHORT += (EtherNetIpProtocol.LOGIX5550_REP_SERNUM_OFFS);

                if ((ReplyOffset.USHORT >= nextReplyOffset.USHORT) || (nextReplyOffset.USHORT >= pdu.Count()))
                {
                    return (DriverErrorCodes)EtherNetIpErrorCodes.ErrorRepTooShort;
                }

                ReplyLength = (ushort)(nextReplyOffset.USHORT - ReplyOffset.USHORT);
            }

            DriverErrorCodes outErrorCode = CheckMessageReplyField(pdu, ReplyOffset, out GeneralStatusError, out ExtendedError);
            if (outErrorCode != DriverErrorCodes.ErrorNoError)
            {
                return outErrorCode;
            }

            outErrorCode = getDataBufferInit(pdu, ref ReplyOffset, ref ReplyLength);
            if (outErrorCode != DriverErrorCodes.ErrorNoError)
            {
                return outErrorCode;
            }

            //If the job's tag is of type string, parse the number of actual characters from the response, 
            //then place the offset on the first byte of the string data 
            UInt32 StringLength = 0;
            if(j.TagFormat == TagFormats.STRING)
            {
                if (pdu.Count() > (ReplyOffset.USHORT + 4 ))
                {
                    StringLength = BitConverter.ToUInt32(pdu, ReplyOffset.USHORT);
                    ReplyOffset.USHORT += 4;
                    ReplyLength -= 4;
                }            
            }

            ushort tagReplyLength;
            if (j.PartialArrayEnd == j.PartialArrayStart)
                tagReplyLength = j.adjSize(pendingTag) ;
            else
            {
                if (j.PartialArrayEnd != 0)
                    tagReplyLength = (ushort)((j.PartialArrayEnd - j.PartialArrayStart) * j.ElemSize);
                else
                    tagReplyLength = (ushort)(j.adjSize(pendingTag) - j.PartialArrayStart * j.ElemSize);
            }

            if (((j.TagFormat != TagFormats.STRING) && (ReplyLength != tagReplyLength)) || 
                j.answer == null)
            {
                return (DriverErrorCodes)EtherNetIpErrorCodes.ErrorRepTagSize;
            }

            if (pendingTag.ByteOffset + tagReplyLength > j.answer.Length)
            {
                ReplyLength = (ushort)(j.answer.Length - pendingTag.ByteOffset);
            }

            if (j.TagFormat == TagFormats.STRING)
            {
                ReplyLength = Math.Min((ushort)StringLength, ReplyLength);
                ReplyLength = Math.Min(tagReplyLength, ReplyLength);
                if(j.answer == null || j.answer.Length != ReplyLength)
                    j.answer = new byte[ReplyLength];
            }

            Array.Copy(pdu, ReplyOffset.USHORT, j.answer, pendingTag.ByteOffset + j.PartialArrayStart * j.ElemSize, ReplyLength);

            return DriverErrorCodes.ErrorNoError;
        }

        private DriverErrorCodes CheckMessageReplyField( byte[] pdu,ushortUnion ReplyOffset, out byte GeneralStatusError, out ushort ExtendedError)
        {
            //[ReplyOffset.USHORT]      Reply Service
            //[ReplyOffset.USHORT + 1]  Reserved
            //[ReplyOffset.USHORT + 2]  General Status
            //[ReplyOffset.USHORT + 3]  Extended Status Size
            //[ReplyOffset.USHORT + 4]  Extended Status
            //[ReplyOffset.USHORT + 5]  Extended Status
            ExtendedError = 0;
            GeneralStatusError = 0;

            if ((pdu[ReplyOffset.USHORT] == 0xCC) && (pdu[ReplyOffset.USHORT + 2] != 0x00))
            {
                GeneralStatusError = pdu[ReplyOffset.USHORT + 2];//General Status
                if (pdu[ReplyOffset.USHORT + 3] != 0x00)//Extended Status Size
                {
                    if (pdu.Count() >= (ReplyOffset.USHORT + 5))
                    {
                        ExtendedError = BitConverter.ToUInt16(pdu, ReplyOffset.USHORT + 4);//Extended Status
                    }
                }
                return DriverErrorCodes.ErrorFrameError;
            }           
            return DriverErrorCodes.ErrorNoError;
        }

        private DriverErrorCodes copyTagDataMicro800(Tag pendingTag, ref byte[] pdu, EtherNetIPCommJob j)
        {
            ushortUnion ReplyOffset = new ushortUnion(EtherNetIpProtocol.LOGIX5550_REP_SERNUM_OFFS);
            if (ReplyOffset.USHORT >= pdu.Count())
            {
                return (DriverErrorCodes)EtherNetIpErrorCodes.ErrorRepTooShort;
            }

            ushort ReplyLength = (ushort)(pdu.Count() - ReplyOffset.USHORT);

            DriverErrorCodes outErrorCode = getDataBufferInitMicro800(pdu, ref ReplyOffset, ref ReplyLength);
            if (outErrorCode != DriverErrorCodes.ErrorNoError)
            {
                return outErrorCode;
            }

                        
            //long array case 
            ushort tagReplyLength = 0;
            if (j.TagsList[0].TagNode.ArrayDimension>0) {
                if (j.PartialArrayEnd > j.PartialArrayStart) {
                    tagReplyLength = (ushort)((j.PartialArrayEnd - j.PartialArrayStart) * j.ElemSize);
                } else
                {
                    tagReplyLength = (ushort)((j.TagsList[0].TagNode.ArrayDimension - j.PartialArrayStart) * j.ElemSize);
                }
            } else {
                tagReplyLength = j.adjSize(pendingTag);
            }

            if (j.TagFormat == TagFormats.STRING)
            {
                if (ReplyLength > tagReplyLength)
                    ReplyLength = tagReplyLength;
                if(j.answer == null || j.answer.Length != ReplyLength)
                    j.answer = new byte[ReplyLength];
            }
                        
            else if (ReplyLength != tagReplyLength || j.answer == null)
            {
                return (DriverErrorCodes)EtherNetIpErrorCodes.ErrorRepTagSize;
            }
                        
            Array.Copy(pdu, ReplyOffset.USHORT, j.answer, j.PartialArrayStart * j.ElemSize, ReplyLength);

            // Special case: data cannot be contained in one single frame
            if (j.TagsList[0].TagNode.ArrayDimension > 0)
            {
                j.PartialArrayStart= j.PartialArrayEnd;
                if (j.PartialArrayEnd == 0 || (j.PartialArrayEnd * j.ElemSize) == j.adjSize(pendingTag))
                {
                    j.PartialArrayStart = 0;
                    j.PartialArrayEnd = 0;
                } 
            }
            else
            {
                j.PartialArrayStart = 0;
                j.PartialArrayEnd = 0;
            }
            

            return DriverErrorCodes.ErrorNoError;
        }

        void CheckTheWritingResponseMicro800(ref byte[] pdu, List<CommJob> list, EtherNetIPStation s)
        {
            ExecutedJobArgs eJob;

            // get write error code 
            byte MRStatus = pdu[EtherNetIpProtocol.LOGIX5550_REP_GENSTS_OFFS];
            if ((MRStatus != 0) && (MRStatus != 0x1E))
            {
                foreach (EtherNetIPCommJob j in list)
                {
                    eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)(EtherNetIpErrorCodes.ErrorMRStatus + MRStatus), Job = j };
                    OnJobExecuted(eJob);
                }
            }
            else
            {
                //long array case --> if not all data were writte, don't close job
                if (((EtherNetIPCommJob)list[0]).PartialArrayStart != 0 || ((EtherNetIPCommJob)list[0]).PartialArrayEnd != 0)
                    return;

                eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)DriverErrorCodes.ErrorNoError, Job = list[0] };

                OnJobExecuted(eJob);
            }
        }

        void CheckTheWritingResponse(ref byte[] pdu, List<CommJob> list, EtherNetIPStation s)
        {
            ushort readJobTag;
            ushort totalPduTag = 0;

            if (s.PlcType == PlcTypes.Micro800_series)
            {
                CheckTheWritingResponseMicro800(ref pdu, list, s);

                return;
            }

            //System.Diagnostics.Debug.WriteLine(string.Format("CheckTheWritingResponse Nr Jobs {0}", list.Count));
            foreach (EtherNetIPCommJob j in list)
            {
                DriverErrorCodes outErrorCode = DriverErrorCodes.ErrorNoError;

                //System.Diagnostics.Debug.WriteLine(string.Format("CheckTheWritingResponse Check {0}", j.TagsList[0].DynSettings.ToString()));
                byte GeneralStatusError = 0;
                ushort ExtendedError = 0;
                readJobTag = 0;

                for (ushort tagIndex = j.ReadTagStart; (tagIndex < (j.ReadTagEnd == 0 ? (ushort)j.TagsList.Count : j.ReadTagEnd)) && (outErrorCode == DriverErrorCodes.ErrorNoError); tagIndex++)
                {
                    if (!j.TerminateTask)
                    {
                        if (outErrorCode != DriverErrorCodes.ErrorNoError)
                            break;

                        Tag tag = (Tag)j.TagsList[tagIndex];
                        outErrorCode = CheckTheWritingResponseTagData(tag, ref pdu, (ushort)(totalPduTag + tagIndex - j.ReadTagStart), j, out GeneralStatusError, out ExtendedError);
                        if (j.TagFormat == TagFormats.STRING)
                            readJobTag++;
                    }
                    readJobTag++;
                }
                totalPduTag += readJobTag;
                ExecutedJobArgs eJob;
                if (outErrorCode == DriverErrorCodes.ErrorNoError)
                {   
                    if((j.PartialArrayEnd != 0) && (j.CommandType == CommandTypes.WriteCmd) && (j.TagsList[0].TagNode.ArrayDimension > 0))
                    {
                        //System.Diagnostics.Debug.WriteLine("--- Debug --- j.PartialArrayStart:{0} j.PartialArrayEnd:{1}", j.PartialArrayStart, j.PartialArrayEnd);
                        j.PartialArrayStart = j.PartialArrayEnd ;
                    }                    
                    eJob = new ExecutedJobArgs { ErrorCode = outErrorCode, Job = j };
                    //eJob.Values = j.answer;
                    //System.Diagnostics.Debug.WriteLine(string.Format("CheckTheWritingResponse OK {0}", j.TagsList[0].DynSettings.ToString()));
                }
                else
                {
                    //System.Diagnostics.Debug.WriteLine(string.Format("CheckTheWritingResponse Err {0}", j.TagsList[0].DynSettings.ToString()));
                    if ((j.InErrorState == false) &&(outErrorCode == (DriverErrorCodes)EtherNetIpErrorCodes.ErrorEnbeddedStation))
                    {
                        //reset Error because it is an error of the tags only and not general error
                        //if the error is general, all the jobs must be put in error
                        outErrorCode = DriverErrorCodes.ErrorNoError;

                        eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)EtherNetIpErrorCodes.ErrorEnbeddedStation, Job = j };                    
                        s.GetCommDriver().OnSystemEvent(null,
                                                         string.Format(Properties.Resources.ErrorEnbeddedError, GeneralStatusError, ExtendedError, j.TagName),
                                                         Opc.Ua.EventSeverity.Max);
                    }
                    else //phase displacement
                    {
                        //System.Diagnostics.Debug.WriteLine(string.Format("CheckTheWritingResponse {0} Job in Error {1}", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"), j.TagsList[0].DynSettings.ToString()));
                        eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)outErrorCode, Job = j };
                    }

                    j.PartialArrayStart = 0;
                    j.PartialArrayEnd = 0;
                }
                if (j.ReadTagStart == 0 && j.ReadTagEnd == 0 && j.PartialArrayStart == 0 && j.PartialArrayEnd == 0)
                {
                    //System.Diagnostics.Debug.WriteLine(string.Format("CheckTheWritingResponse OnJobExecuted {0}", j.TagsList[0].DynSettings.ToString()));
                    OnJobExecuted(eJob);
                }
            }
        }

        private DriverErrorCodes CheckTheWritingResponseTagData(Tag pendingTag, ref byte[] pdu, ushort tagIndex, EtherNetIPCommJob j, out byte GeneralStatusError, out ushort ExtendedError)
        {
            GeneralStatusError = 0;
            ExtendedError = 0;
            ushortUnion responseTag = new ushortUnion(pdu, EtherNetIpProtocol.LOGIX5550_REP_SERNUM_OFFS);
            if (tagIndex >= responseTag.USHORT)
            {
                return (DriverErrorCodes)EtherNetIpErrorCodes.ErrorRepTooShort;
            }
            ushortUnion ReplyOffset = new ushortUnion(pdu, (ushort)(EtherNetIpProtocol.LOGIX5550_REP_SERNUM_OFFS + tagIndex * 2 + 2));
            ReplyOffset.USHORT += (EtherNetIpProtocol.LOGIX5550_REP_SERNUM_OFFS);
            if (ReplyOffset.USHORT >= pdu.Count())
            {
                return (DriverErrorCodes)EtherNetIpErrorCodes.ErrorRepTooShort;
            }

            ushort ReplyLength;
            if (tagIndex == responseTag.USHORT - 1)
            {
                ReplyLength = (ushort)(pdu.Count() - ReplyOffset.USHORT);
            }
            else
            {
                ushortUnion nextReplyOffset = new ushortUnion(pdu, (ushort)(EtherNetIpProtocol.LOGIX5550_REP_SERNUM_OFFS + tagIndex * 2 + 4));
                nextReplyOffset.USHORT += (EtherNetIpProtocol.LOGIX5550_REP_SERNUM_OFFS);

                if ((ReplyOffset.USHORT >= nextReplyOffset.USHORT) || (nextReplyOffset.USHORT >= pdu.Count()))
                {
                    return (DriverErrorCodes)EtherNetIpErrorCodes.ErrorRepTooShort;
                }

                ReplyLength = (ushort)(nextReplyOffset.USHORT - ReplyOffset.USHORT);
            }
            //[ReplyOffset.USHORT]      Reply Service
            //[ReplyOffset.USHORT + 1]  Reserved
            //[ReplyOffset.USHORT + 2]  General Status
            //[ReplyOffset.USHORT + 3]  Extended Status Size
            //[ReplyOffset.USHORT + 4]  Extended Status
            //[ReplyOffset.USHORT + 5]  Extended Status
            if (pdu[ReplyOffset.USHORT] == 0xcd)//Write Tag Service (Reply)
            {
                if (pdu[ReplyOffset.USHORT + 2] != 0x00)//General Status
                {
                    GeneralStatusError = pdu[ReplyOffset.USHORT + 2];
                    if(pdu[ReplyOffset.USHORT + 3] != 0x00)//Extended Status Size
                    {
                        if (pdu.Count() >= (ReplyOffset.USHORT + 5))
                        {
                            ExtendedError = BitConverter.ToUInt16(pdu, ReplyOffset.USHORT + 4);//Extended Status
                        }                        
                    }
                    return (DriverErrorCodes)EtherNetIpErrorCodes.ErrorEnbeddedStation;
                }
            }
            else
            {
                return (DriverErrorCodes)EtherNetIpErrorCodes.ErrorRepSerCode;
            }            
            return DriverErrorCodes.ErrorNoError;
        }


        private DriverErrorCodes getDataBufferInit(byte[] pdu, ref ushortUnion ReplyOffset, ref ushort ReplyLength)
        {
            // Get the data type (abbreviated type)
            switch (pdu[ReplyOffset.USHORT])
            {
                // Structure element
                case 0xCC:
                    if (ReplyLength < 5)
                    {
                        return (DriverErrorCodes)EtherNetIpErrorCodes.ErrorRepTooShort;
                    }

                    ReplyOffset.USHORT += 4;
                    ReplyLength -= 4;

                    return getDataBufferInit( pdu, ref  ReplyOffset, ref ReplyLength);
                    

                // Structure
                case 0xA0:
                    if (ReplyLength < 5)
                    {
                        return (DriverErrorCodes)EtherNetIpErrorCodes.ErrorRepTooShort;
                    }

                    ReplyOffset.USHORT += 4;
                    ReplyLength -= 4;
                    
                    break;

                // BOOL
                case 0xC1:

                // SINT
                case 0xC2:

                // INT
                case 0xC3:

                // DINT
                case 0xC4:

                // LINT
                case 0xC5:

                // ULINT
                case 0xC9:

                // LWORD
                case 0xD4:

                // REAL
                case 0xCA:

                // Array of bits
                case 0xD3:

                    if (ReplyLength < 3)
                    {
                        return (DriverErrorCodes)EtherNetIpErrorCodes.ErrorRepTooShort;
                    }
                    ReplyOffset.USHORT += 2;
                    ReplyLength -= 2;
                    break;

                // ?
                default:
                    return (DriverErrorCodes)EtherNetIpErrorCodes.ErrorRepDataType;
            }
            return DriverErrorCodes.ErrorNoError;
        }

        private DriverErrorCodes getDataBufferInitMicro800(byte[] pdu, ref ushortUnion ReplyOffset, ref ushort ReplyLength)
        {
            
            // Get the data type (abbreviated type)
            switch (pdu[ReplyOffset.USHORT])
            {
                // String
                case 0xDA:
                    if (ReplyLength < 3)
                    {
                        return (DriverErrorCodes)EtherNetIpErrorCodes.ErrorRepTooShort;
                    }
                    ReplyLength = pdu[ReplyOffset.USHORT + 2];
                    ReplyOffset.USHORT += 3;
                    break;


                // ?
                default:
                    {
                        return getDataBufferInit(pdu, ref ReplyOffset, ref ReplyLength);
                    }
            }
            return DriverErrorCodes.ErrorNoError;
        }

        void GetReadData(ref byte[] pdu,EtherNetIPCommJob pendingjob, EtherNetIPStation s)
        {

            ushort dataOffs = EtherNetIpProtocol.PCCC_RESP_DATA_OFFS;
            if (s.PlcType != PlcTypes.SLC500_MicroLogix)
            {
                //we must skip data type/size information, as we assume that we already know it
                //this may not be true when using symbolic addressing
                int extTypeBytes = 0;
                int extSizeBytes = 0;
                if ((pdu[dataOffs] & 0x80) != 0)
                {
                    extTypeBytes = (pdu[dataOffs] & 0x70) >> 4;
                }

                if ((pdu[dataOffs] & 0x08) != 0)
                {
                    extSizeBytes = (pdu[dataOffs] & 0x07);
                }

                if (extTypeBytes > 0)
                {
                    if (pdu[dataOffs + 1] == 9)
                    {
                        dataOffs += (ushort)(extSizeBytes + 2);
                        extTypeBytes = 0;
                        extSizeBytes = 0;
                        //if array type, there is one more byte representing each element's type and size
                        if ((pdu[dataOffs] & 0x80) != 0)
                        {
                            extTypeBytes = (pdu[dataOffs] & 0x70) >> 4;
                        }

                        if ((pdu[dataOffs] & 0x08) != 0)
                        {
                            extSizeBytes = (pdu[dataOffs] & 0x07);
                        }

                    }
                }
                dataOffs += (ushort)(extTypeBytes + extSizeBytes + 1);
            }

            ExecutedJobArgs eJob = new ExecutedJobArgs();
            byte answerSize = 0;
            if (pendingjob.AddressType == AddressTypes.DataFile)
            {
                answerSize = (byte)(pdu.Count() - dataOffs);
            }
            else
            {
                if (pendingjob.isProtocolBool() && (pendingjob.ElementNumber != 0 || (uint)pendingjob.TagsList[0].TagNode.DataType.Identifier == (uint)BuiltInType.Boolean))
                {
                    answerSize = (byte)((byte)(pendingjob.TotalJobSize + 7) / 8);
                }
                else
                {
                    answerSize = (byte)(((byte)(pendingjob.TotalJobSize + 1) >> 1) << 1);
                }

            }

            byte[] answer = new byte[answerSize];
            if (pdu.Count() < dataOffs + answerSize)
            {
                eJob.ErrorCode = (DriverErrorCodes)EtherNetIpErrorCodes.ErrorRepTooShort;
            }
            else
            {
                Array.Copy(pdu, dataOffs, answer, 0, answerSize);
            }
            eJob.Values = answer;

            eJob.Job = pendingjob;
            OnJobExecuted(eJob);

            return ;

        }

        bool AnalyzeResponse(ref byte[] pdu, EtherNetIPStation s, EtherNetIPCommJob mJob)
        {

            if (AnalyzeReplyHeader(ref pdu, EtherNetIpProtocol.ecSendRRData.USHORT, SessionHandle.UINT) !=
                (EtherNetIpErrorCodes)DriverErrorCodes.ErrorNoError)
            {
                return false;
            }

            //Examine the Message Router Service Reply 
            byte MRCode = pdu[EtherNetIpProtocol.MR_SVC_REPLY_CODE_OFFS];
            if (MRCode != 0xCB && MRCode != 0xD2)
            {
                //CommDriver.OnSystemEvent(ObjectIds.Server, String.Format(EtherNetIP.Properties.Resources.ErrorWrongMRServiceReplyCode, MRCode), Opc.Ua.EventSeverity.High);
                ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)EtherNetIpErrorCodes.ErrorWrongMRServiceReplyCode, Job = mJob };
                OnJobExecuted(eJob);
                return false;
            }

            byte MRStatus = pdu[EtherNetIpProtocol.MR_SVC_REPLY_GENSTS_OFFS];
            if (MRStatus != 0)
            {
                //byte MRAddStatusSize = pdu[EtherNetIpProtocol.MR_SVC_REPLY_ADDSTSSZ_OFFS];
                //String strErr = String.Format(EtherNetIP.Properties.Resources.ErrorMRStatus, MRStatus.ToString("X"));
                //if (MRAddStatusSize > 0)
                //{
                //    String strAdd = EtherNetIP.Properties.Resources.ErrorAddStsInfo;
                //    strErr += strAdd;
                //    for (int i = 0; i < MRAddStatusSize; i++)
                //    {
                //        strAdd = String.Format("{0} Hex ", pdu[EtherNetIpProtocol.MR_SVC_REPLY_ADDSTSSZ_OFFS + i + 1].ToString("X"));
                //        strErr += strAdd;
                //    }
                //}

                //CommDriver.OnSystemEvent(ObjectIds.Server, strErr, Opc.Ua.EventSeverity.High);

                ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)(EtherNetIpErrorCodes.ErrorMRStatus + MRStatus), Job = mJob };

                return false;
            }

            //Examine the PCCC response
            byte PCCCCode = pdu[EtherNetIpProtocol.PCCC_RESP_CODE_OFFS];
            if (PCCCCode != 0x4F)
            {
                //CommDriver.OnSystemEvent(ObjectIds.Server, String.Format(EtherNetIP.Properties.Resources.ErrorWrongPCCCRespCode, MRStatus), Opc.Ua.EventSeverity.High);
                ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)EtherNetIpErrorCodes.ErrorWrongPCCCRespCode, Job = mJob };
                return false;
            }

            byte PlcStatus = pdu[EtherNetIpProtocol.PCCC_RESP_STS_OFFS];
            if (PlcStatus == 0xF0)
            {
                //vero solo se EXT STS non viene mandato assieme ai dati!!! Verificare
                byte PlcExtendedStatus = pdu[EtherNetIpProtocol.PCCC_RESP_DATA_OFFS];
                PlcResponseError(PlcStatus, PlcExtendedStatus, mJob);

                return false;
            }
            else if (PlcStatus != 0)
            {
                PlcResponseError(PlcStatus, 0,  mJob);
                return false;
            }

            return true;
        }

        bool Micro800TagsCheckReply(ref byte[] pdu, EtherNetIPStation s, List<CommJob> list)
        {
            if (AnalyzeReplyHeader(ref pdu, EtherNetIpProtocol.ecSendUnitData.USHORT, SessionHandle.UINT) != (EtherNetIpErrorCodes)DriverErrorCodes.ErrorNoError)
            {
                foreach (EtherNetIPCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)EtherNetIpErrorCodes.ErrorWrongTransaction, Job = j };
                    OnJobExecuted(eJob);
                }
                return false;
            }

            return true;
        }

        bool Logix5550TagsCheckReply(ref byte[] pdu, EtherNetIPStation s, List<CommJob> list)
        {            
            if (AnalyzeReplyHeader(ref pdu, EtherNetIpProtocol.ecSendUnitData.USHORT, SessionHandle.UINT) != (EtherNetIpErrorCodes)DriverErrorCodes.ErrorNoError)
            {
                foreach (EtherNetIPCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)EtherNetIpErrorCodes.ErrorWrongTransaction, Job = j };
                    OnJobExecuted(eJob);
                }
                return false;
            }

            byte MRStatus = pdu[EtherNetIpProtocol.LOGIX5550_REP_GENSTS_OFFS];
            if ((MRStatus != 0) && (MRStatus != 0x1E))
            {
                //CommDriver.OnSystemEvent(ObjectIds.Server, strErr, Opc.Ua.EventSeverity.High);
                foreach (EtherNetIPCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)(EtherNetIpErrorCodes.ErrorMRStatus + MRStatus), Job = j };
                    OnJobExecuted(eJob);
                }
                return false;
            }
            
            // Check the number of replied services
            ushortUnion recieveReply = new ushortUnion(pdu, EtherNetIpProtocol.LOGIX5550_REP_SERNUM_OFFS);
            if (recieveReply.USHORT != s.lastProcessedTags.USHORT)
            {
                //CommDriver.OnSystemEvent(ObjectIds.Server, String.Format(EtherNetIP.Properties.Resources.ErrorRepSerNum), Opc.Ua.EventSeverity.High);
                foreach (EtherNetIPCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)EtherNetIpErrorCodes.ErrorRepSerNum, Job = j };
                    OnJobExecuted(eJob);
                }
                return false;
            }
            
            return true;
        }

        
        //private bool Logix5550TagsCheckSingleReply(ref byte[] pdu, List<CommJob> list, ushortUnion ReplyOffset, CommandTypes CommandType)
        //{
        //    String addressInfo = string.Format(Properties.Resources.AddressInfo,
        //            ((EtherNetIPCommJob)list[0]).ABAddress,
        //            ((EtherNetIPCommJob)list[0]).TotalJobSize,
        //            ((EtherNetIPCommJob)list[0]).TagsList[0].ByteOffset);	//Remember that the transaction was OK

        //    if ((CommandType == CommandTypes.ReadCmd && pdu[ReplyOffset.USHORT] != 0xCC) ||
        //        (CommandType == CommandTypes.WriteCmd && pdu[ReplyOffset.USHORT] != 0xCD))
        //    {
        //        //CommDriver.OnSystemEvent(ObjectIds.Server, String.Format(EtherNetIP.Properties.Resources.ErrorRepSerCode, addressInfo), Opc.Ua.EventSeverity.High);
        //        return false;
        //    }

        //    byte ReplyStatus = pdu[ReplyOffset.USHORT + 2];
        //    if (ReplyStatus != 0)
        //    {
        //        //String strErr = String.Format(EtherNetIP.Properties.Resources.ErrorRepStatus, ReplyStatus);
        //        //byte AddStatusSize = pdu[ReplyOffset.USHORT + 3];
        //        //if (AddStatusSize > 0)
        //        //{
        //        //    strErr += "Additional Status information: ";
        //        //    for (int i = 0; i < AddStatusSize; i++)
        //        //    {
        //        //        strErr += String.Format("{0:X2} Hex ", pdu[ReplyOffset.USHORT + 4 + i]);
        //        //    }
        //        //}
        //        //strErr += addressInfo;

        //        //CommDriver.OnSystemEvent(ObjectIds.Server, strErr, Opc.Ua.EventSeverity.High);
        //        return false;
        //    }
        //    return true;
        //}

        void PlcResponseError(byte stsCode, byte extStsCode, EtherNetIPCommJob mJob)
        {
            //String addressInfo = string.Format(EtherNetIP.Properties.Resources.AddressInfo,
            //        mJob.ABAddress, mJob.TotalJobSize, mJob.TagsList[0].ByteOffset);

            //String str;
            EtherNetIpErrorCodes EtherNetIpErrorCode;
            
            if (stsCode > 0 && stsCode < 10)
            {
            //    str = String.Format(EtherNetIP.Properties.Resources.ErrorStatusNotZero, stsCode.ToString("X"), PlcResponseErrorList1[stsCode - 1]);
            //}
            //else if (stsCode >= 0x10  && stsCode <= 0xE0)
            //{
            //    int idx = (stsCode >> 4);
            //    str = String.Format(EtherNetIP.Properties.Resources.ErrorStatusNotZero, stsCode.ToString("X"), PlcResponseErrorList2[idx]);
                EtherNetIpErrorCode = EtherNetIpErrorCodes.ErrorStatusNotZero;
            }
            else if (stsCode == 0xF0)
            {
                if (extStsCode >= 0x01 && extStsCode <= 0x24)
                {
                    //str = String.Format(EtherNetIP.Properties.Resources.ErrorEXTSTSCodeExt, extStsCode.ToString("X"), PlcResponseErrorList3[extStsCode]);
                    EtherNetIpErrorCode = EtherNetIpErrorCodes.ErrorEXTSTSCodeExt;
                }
                else
                {
                    //str = String.Format(EtherNetIP.Properties.Resources.ErrorUnknSTSEXTCode, extStsCode.ToString("X"));
                    EtherNetIpErrorCode = EtherNetIpErrorCodes.ErrorUnknSTSEXTCode;
                }
            }
            else
            {
                //str = String.Format(EtherNetIP.Properties.Resources.ErrorUnknSTSCode, stsCode.ToString("X"));
                EtherNetIpErrorCode = EtherNetIpErrorCodes.ErrorUnknSTSCode;
            }

            //str += addressInfo;

            //CommDriver.OnSystemEvent(ObjectIds.Server, str, Opc.Ua.EventSeverity.High);
            ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)EtherNetIpErrorCode, Job = mJob };
            OnJobExecuted(eJob);
        }

        public void ReceiveClear()
        {
            lock (lockThreadObject)
                ReceiveBuffer.Clear();
            Flush();
        }
        
        public bool EtherNetIpDeviceWrite(ref byte[] TcpBuffer, ref ushortUnion TcpBufferSize)
        {
            //fill in packet data length
            if (TcpBufferSize.USHORT < EtherNetIpProtocol.ENCAPSULATION_HEADER_SIZE)
            {
                return false;
            }
            ushort TcpDataSize = TcpBufferSize.USHORT;

            TcpBufferSize.USHORT -= EtherNetIpProtocol.ENCAPSULATION_HEADER_SIZE;  
            TcpBuffer[EtherNetIpProtocol.EDATA_LEN_OFFS] = TcpBufferSize.LOBYTE;				//Enc.data length lobyte
            TcpBuffer[EtherNetIpProtocol.EDATA_LEN_OFFS + 1] = TcpBufferSize.HIBYTE;			//Enc.data length hibyte

            return DeviceWrite(TcpBuffer, TcpDataSize);
        }

        public bool EtherNetIpDeviceWriteNoInsertLength(ref byte[] TcpBuffer, ref ushortUnion TcpBufferSize)
        {
            //fill in packet data length
            if (TcpBufferSize.USHORT < EtherNetIpProtocol.ENCAPSULATION_HEADER_SIZE)
            {
                return false;
            }
            ushort TcpDataSize = TcpBufferSize.USHORT;

            TcpBufferSize.USHORT = 34 ;
            TcpBuffer[EtherNetIpProtocol.EDATA_LEN_OFFS] = TcpBufferSize.LOBYTE;				//Enc.data length lobyte
            TcpBuffer[EtherNetIpProtocol.EDATA_LEN_OFFS + 1] = TcpBufferSize.HIBYTE;			//Enc.data length hibyte

            return DeviceWrite(TcpBuffer, TcpDataSize);
        }

        public EtherNetIpErrorCodes AnalyzeReplyHeader(ref byte[] pdu, ushort Cmd, uint stationSHandle)    
        {
            ushortUnion CmdHeader = new ushortUnion(pdu, 0);
            uintUnion Status = new uintUnion(pdu, EtherNetIpProtocol.EDATA_STS_OFFS);
            uintUnion replySHandle = new uintUnion(pdu, EtherNetIpProtocol.EDATA_SESS_HND_OFFS);

            if( CmdHeader.USHORT != Cmd || stationSHandle !=replySHandle.UINT )
            {
                //CommDriver.OnSystemEvent(ObjectIds.Server, EtherNetIP.Properties.Resources.ErrorOutOfSync, Opc.Ua.EventSeverity.High);
                return EtherNetIpErrorCodes.ErrorOutOfSync;
            }
            //Examine the Encapsulation command status

            if (Status.UINT == 1)
            {
                //CommDriver.OnSystemEvent(ObjectIds.Server, EtherNetIP.Properties.Resources.ErrorEncapStatus1, Opc.Ua.EventSeverity.High);
                return EtherNetIpErrorCodes.ErrorEncapStatus1;
            }
            else if (Status.UINT == 2)
            {
                //CommDriver.OnSystemEvent(ObjectIds.Server, EtherNetIP.Properties.Resources.ErrorEncapStatus2, Opc.Ua.EventSeverity.High);
                return EtherNetIpErrorCodes.ErrorEncapStatus2;
            }
            else if (Status.UINT == 3)
            {
                //CommDriver.OnSystemEvent(ObjectIds.Server, EtherNetIP.Properties.Resources.ErrorEncapStatus3, Opc.Ua.EventSeverity.High);
                return EtherNetIpErrorCodes.ErrorEncapStatus3;
            }
            else if (Status.UINT == 0x64)
            {
                //CommDriver.OnSystemEvent(ObjectIds.Server, EtherNetIP.Properties.Resources.ErrorEncapStatus64, Opc.Ua.EventSeverity.High);
                return EtherNetIpErrorCodes.ErrorEncapStatus64;
            }
            else if (Status.UINT == 0x65)
            {
                //CommDriver.OnSystemEvent(ObjectIds.Server, EtherNetIP.Properties.Resources.ErrorEncapStatus65, Opc.Ua.EventSeverity.High);
                return EtherNetIpErrorCodes.ErrorEncapStatus65;
            }
            else if (Status.UINT == 0x69)
            {
                //CommDriver.OnSystemEvent(ObjectIds.Server, EtherNetIP.Properties.Resources.ErrorEncapStatus69, Opc.Ua.EventSeverity.High);
                return EtherNetIpErrorCodes.ErrorEncapStatus69;
            }
            else if (Status.UINT != 0)
            {
                //CommDriver.OnSystemEvent(ObjectIds.Server, String.Format(EtherNetIP.Properties.Resources.ErrorUnknEncapStatus, Status.UINT), Opc.Ua.EventSeverity.High);
                return EtherNetIpErrorCodes.ErrorUnknEncapStatus;
            }
            return (EtherNetIpErrorCodes)DriverErrorCodes.ErrorNoError;
        }

        public CommunicationDriver getCommDriver()
        {
            return CommDriver;
        }
        #endregion

                #region ErrorCodeArrayString

        protected string[] PlcResponseErrorList1 = 
        {
            Properties.Resources.ErrorSTSCode01,
            Properties.Resources.ErrorSTSCode02,
            Properties.Resources.ErrorSTSCode03,
            Properties.Resources.ErrorSTSCode04,
            Properties.Resources.ErrorSTSCode05,
            Properties.Resources.ErrorSTSCode06,
            Properties.Resources.ErrorSTSCode07,
            Properties.Resources.ErrorSTSCode08,
            Properties.Resources.ErrorSTSCode09Spare,
        };

        protected string[] PlcResponseErrorList2 = 
        {
            Properties.Resources.ErrorSTSCode10,
            Properties.Resources.ErrorSTSCode20,
            Properties.Resources.ErrorSTSCode30,
            Properties.Resources.ErrorSTSCode40,
            Properties.Resources.ErrorSTSCode50,
            Properties.Resources.ErrorSTSCode60,
            Properties.Resources.ErrorSTSCode70,
            Properties.Resources.ErrorSTSCode80,
            Properties.Resources.ErrorSTSCode90,
            Properties.Resources.ErrorSTSCodeA0,
            Properties.Resources.ErrorSTSCodeB0,
            Properties.Resources.ErrorSTSCodeC0,
            Properties.Resources.ErrorSTSCodeD0,
            Properties.Resources.ErrorSTSCodeE0,
        };

        protected string[] PlcResponseErrorList3 = 
        {
            Properties.Resources.ErrorEXTSTS01,
            Properties.Resources.ErrorEXTSTS02,
            Properties.Resources.ErrorEXTSTS03,
            Properties.Resources.ErrorEXTSTS04,
            Properties.Resources.ErrorEXTSTS05,
            Properties.Resources.ErrorEXTSTS06,
            Properties.Resources.ErrorEXTSTS07,
            Properties.Resources.ErrorEXTSTS08,
            Properties.Resources.ErrorEXTSTS09,
            Properties.Resources.ErrorEXTSTS0A,
            Properties.Resources.ErrorEXTSTS0B,
            Properties.Resources.ErrorEXTSTS0C,
            Properties.Resources.ErrorEXTSTS0D,
            Properties.Resources.ErrorEXTSTS0E,
            Properties.Resources.ErrorEXTSTS0F,
            Properties.Resources.ErrorEXTSTS10,
            Properties.Resources.ErrorEXTSTS11,
            Properties.Resources.ErrorEXTSTS12,
            Properties.Resources.ErrorEXTSTS13,
            Properties.Resources.ErrorEXTSTS14,
            Properties.Resources.ErrorEXTSTS15,
            Properties.Resources.ErrorEXTSTS16,
            Properties.Resources.ErrorEXTSTS17,
            Properties.Resources.ErrorEXTSTS18,
            Properties.Resources.ErrorEXTSTS19,
            Properties.Resources.ErrorEXTSTS1A,
            Properties.Resources.ErrorEXTSTS1B,
            Properties.Resources.ErrorEXTSTS1C,
            Properties.Resources.ErrorEXTSTS1D,
            Properties.Resources.ErrorEXTSTS1E,
            Properties.Resources.ErrorEXTSTS1F,
            Properties.Resources.ErrorEXTSTS20,
            Properties.Resources.ErrorEXTSTS21,
            Properties.Resources.ErrorEXTSTS22,
            Properties.Resources.ErrorEXTSTS23,
            Properties.Resources.ErrorEXTSTS24,
         };



        #endregion

        #region Properties

        #endregion
        #region IDisposable Interface

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void Dispose()
        {
            base.Dispose();
        }

        #endregion

    }
}

