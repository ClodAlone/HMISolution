using Accon.AGLink;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Xpo.Helpers;
using DriverCodeBase;
using DriverCodeBase.Enumerators;
using Opc.Ua;
using S7ImportParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Utilities;

namespace S7TIASymbolic
{
    public class PlcConnection : IDisposable
    {
        private enum State
        {
            eNotOpen = 0,
            eDevOpened,
            eDialedUp,
            eInitAdapter,
            eConnected,
            eNotInit,
        }

        private static Int32 devNrCnt = 0;
        public Int32 devNr;

        private State conState = State.eNotInit;
        public Int32 connNr = -1;
        private Int32 timeout = 1;
        private string MLFBNr ;
        private string HostName ;
        private ushort HostPort = 0;
        private IntPtr pHandleFile = IntPtr.Zero;
        CommunicationDriver CommDriver;
        protected object lockStream ;
        private string szStationName;
        private bool bErrorLoadSymbolFile = false;
        private int nErrorLibraryCode = AGL4.AGL40_SUCCESS;
        public List<string> listDataBlocksAndTables;

        // define .tia file import source
        public S7TIAImportParser.ImportSourceManagement SymbolFileImportSource = S7TIAImportParser.ImportSourceManagement.None;
        // New method of var address mapping flag
        public bool ImportFromProjectMappingFileIsPresent = false;
        public Dictionary<string,string> mapTagsPlcAndProject;
        // name of .tia file for each station --> necessary for db project to store variable file name part
        public Dictionary<string, string> mapStationTiaFileFromDB = new Dictionary<string, string>();        
        private bool? MoviconProjectSourceIsFile = null;


        /// <summary>
        /// Verbindung über TCP/IP
        /// </summary>
        public bool SetParasTcpIP()
        {
            AGL4.S7tcpipTia para = new AGL4.S7tcpipTia();

            para.Conn[0].Address = HostName;
            para.Conn[0].ConnTypeEx = AGL4.ConnTypeEx.eCT_HMI;
            para.Conn[0].PLCClassEx = AGL4.PLC_ClassEx.ePLCEx_AUTO_TIA;
            para.Conn[0].TimeOut = timeout;
            para.Conn[0].PlcNr = 1;
            para.Conn[0].PortNr = HostPort;
            para.Conn[0].OwnAddress =  0;
            para.Conn[0].OwnPortNr = 0;

            if ((nErrorLibraryCode = AGL4.SetDevType(devNr, AGL4.TYPE_S7_TCPIP_TIA)) != AGL4.AGL40_SUCCESS)
            {
                return false;
            }
            if ((nErrorLibraryCode = AGL4.SetParas(devNr, AGL4.TYPE_S7_TCPIP_TIA, (object)para)) != AGL4.AGL40_SUCCESS)
            {
                return false;
            }
            return true;
        }

        public PlcConnection()
        {
            devNr = getDevNr;
            conState = State.eNotInit;
            lockStream = new object();
            MLFBNr = "---";
            HostName = "";

            mapTagsPlcAndProject = new Dictionary<string, string>();            
        }

        public bool Init(string HostName, ushort HostPort, Int32 timeout, CommunicationDriver CommDriver, string stationName, S7TIAImportParser.ImportSourceManagement importSource)
        {
            this.HostName = HostName;
            this.HostPort = HostPort;
            this.timeout = timeout;
            this.CommDriver = CommDriver;
            this.szStationName = stationName;
            this.ImportSource = importSource;

            return true;
        }

        public bool Connect(bool bLoadFile = true)
        {                      
            Int32 RetVal = 0;

            lock (lockStream)
            {
                if (conState == State.eNotInit)
                {
                    if (GetStationName == string.Empty)
                        return false;

                    conState = State.eNotOpen;
                }                                
                                
                if (conState == State.eNotOpen)
                {
                    if (!SetParasTcpIP())
                    {
                        return false;
                    }

                    RetVal = AGL4.OpenDevice(devNr);
                    if (RetVal == AGL4.AGL40_SUCCESS)
                    {
                        conState = State.eDevOpened;
                    }
                    else
                    {
                        nErrorLibraryCode = RetVal;
                    }
                }

                if (conState == State.eDevOpened)
                {
                    RetVal = AGL4.DialUp(devNr, timeout);
                    if (RetVal == AGL4.AGL40_SUCCESS)
                    {
                        conState = State.eDialedUp;
                    }
                    else
                    {
                        nErrorLibraryCode = RetVal;
                    }
                }

                if (conState == State.eDialedUp)
                {
                    RetVal = AGL4.InitAdapter(devNr, timeout);
                    if (RetVal == AGL4.AGL40_SUCCESS)
                    {
                        conState = State.eInitAdapter;
                    }
                    else
                    {
                        nErrorLibraryCode = RetVal;
                    }
                }

                if (conState == State.eInitAdapter)
                {
                    RetVal = AGL4.PLCConnect(devNr, 1, out connNr, timeout);
                    if (RetVal == AGL4.AGL40_SUCCESS)
                    {
                        conState = State.eConnected;
                    }
                    else
                    {
                        nErrorLibraryCode = RetVal;
                    }
                }
                                
                if (conState == State.eConnected && bLoadFile)
                {
                    #region Load and create of symbolic files (.tia, .list)
                    S7TIAImportParser parserTia = new S7TIAImportParser();
                    
                    //only if not yet opened
                    if (SymbolFileImportSource == S7TIAImportParser.ImportSourceManagement.None)
                    {
                        // identify symbolic file (and data source PLC/Project); try also to convert from old driver version
                        SymbolFileImportSource = GetSymbolFileSource(GetStationName);

                        // if no import from plc was forced
                        if (_ImportSource != S7TIAImportParser.ImportSourceManagement.Project)
                        {
                            // if symbolic file come from PLC or don't exist
                            if (SymbolFileImportSource == S7TIAImportParser.ImportSourceManagement.Project || SymbolFileImportSource == S7TIAImportParser.ImportSourceManagement.None)
                            {
                                if (SaveSymbolicFile(GetStationName, S7TIAImportParser.ImportSourceManagement.Plc))
                                {
                                    // force to recreate list file because also symbolic file was updated/created
                                    SaveListDataBlockAndTable(GetStationName, true);
                                    SymbolFileImportSource = S7TIAImportParser.ImportSourceManagement.Plc;
                                }
                            }
                            // try to create file only if not exist
                            SaveListDataBlockAndTable(GetStationName, false);
                            // load list of blocks and tag tables file
                            LoadListDataBlockAndTable(GetStationName);
                        }                        

                        if (SymbolFileImportSource != S7TIAImportParser.ImportSourceManagement.None)
                        {
                            // Finally try to load symbolic file
                            if (LoadFileSymbol(GetStationName, SymbolFileImportSource))
                                // try to load project address mapping file
                                ImportFromProjectMappingFileIsPresent = LoadImportProjectTags(GetStationName, SymbolFileImportSource);
                            else
                                SymbolFileImportSource = S7TIAImportParser.ImportSourceManagement.None;
                        }
                        else
                        {                
                            // symbolic file was not found
                            string szAux = String.Format("{0} {1}", GetStationName, String.Format(Properties.Resources.ErrorSymbolFileNotFound, GetAglinkFile(GetStationName, _ImportSource)));
                            CommDriver.OnSystemEvent(ObjectIds.Server, szAux, Opc.Ua.EventSeverity.High);
                        }

                        LoadDefaultListTable();
                    }
                    #endregion
                }
            }

            if (IsConnected())
            {
                nErrorLibraryCode = AGL4.ReadMLFBNr(connNr, out MLFBNr, timeout);
                CommDriver.OnSystemEvent(null, Properties.Resources.ConnectionEstablished, EventSeverity.High);
                return true;
            }
            else
            {
                Disconnect();
                return false;
            }
        }

        public bool Disconnect()
        {
            lock (lockStream)
            {
                if (conState == State.eNotInit)
                    return false;
                Int32 _RetVal = 0;
                if (conState == State.eConnected)
                {
                    _RetVal = AGL4.PLCDisconnect(connNr, timeout);
                    if ((_RetVal != AGL4.AGL40_CONNECTION_CLOSED) &&
                         (_RetVal != AGL4.AGL40_NOT_CONNECTED) &&
                         (_RetVal != AGL4.AGL40_SUCCESS))
                    {
                        nErrorLibraryCode = _RetVal;
                    }
                    conState = State.eInitAdapter;
                }
                if (conState == State.eInitAdapter)
                {
                    _RetVal = AGL4.ExitAdapter(devNr, timeout);
                    if (_RetVal != AGL4.AGL40_SUCCESS)
                    {
                        nErrorLibraryCode = _RetVal;
                    }
                    conState = State.eDialedUp;
                }
                if (conState == State.eDialedUp)
                {
                    _RetVal = AGL4.HangUp(devNr, timeout);
                    if (_RetVal != AGL4.AGL40_SUCCESS)
                    {
                        nErrorLibraryCode = _RetVal;
                    }
                    conState = State.eDevOpened;
                }
                if (conState == State.eDevOpened)
                {
                    _RetVal = AGL4.CloseDevice(devNr);
                if (_RetVal != AGL4.AGL40_SUCCESS)
                {
                    nErrorLibraryCode = _RetVal;
                }
                    conState = State.eNotOpen;
                }

                return _RetVal == AGL4.AGL40_SUCCESS;
        }
    }

        public String GetMLFBNr()
        {
            return MLFBNr;
        }

        public bool IsConnected()
        {
            lock (lockStream)
            {
                return conState == State.eConnected;
            }
        }
        public bool IsInit()
        {
            return conState != State.eNotInit;
        }

        public string GetAglinkFileSuffix(S7TIAImportParser.ImportSourceManagement importSource)
        {
            string FileSuffix = string.Empty;

            switch (importSource)
            {
                case S7TIAImportParser.ImportSourceManagement.Plc:
                case S7TIAImportParser.ImportSourceManagement.ProjectAndPlc:
                    FileSuffix = "_Plc";
                    break;
                case S7TIAImportParser.ImportSourceManagement.Project:
                    FileSuffix = "_Prj";
                    break;
                default:
                    FileSuffix = string.Empty;
                    break;
            }

            return FileSuffix;
        }

        public S7TIAImportParser.ImportSourceManagement GetAglinkFileTypeBySuffix(string suffix)
        {
            switch (suffix)
            {
                case "_Plc":
                    return S7TIAImportParser.ImportSourceManagement.Plc;
                case "_Prj":
                    return S7TIAImportParser.ImportSourceManagement.Project;
                default:
                    return S7TIAImportParser.ImportSourceManagement.None;
            }
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
        
        public string GetAglinkFile(string stationName, S7TIAImportParser.ImportSourceManagement importSource)
        {
            string Filebase;
            InMemoryDataStore InMemory;
            bool TargetIsFile;

            using (IDataLayer idl = GetAglinkFile(stationName, importSource, out Filebase, out InMemory, out TargetIsFile))
            {
                if (idl == null)
                    return string.Empty;
            }

            return Filebase;
        }

        public IDataLayer GetAglinkFile(string stationName, S7TIAImportParser.ImportSourceManagement importSource, out string filebase, out InMemoryDataStore InMemory, out bool targetIsFile)
        {
            S7TIADriver S7TiaCommDriver = (S7TIADriver)CommDriver;

            return GetAglinkFile(stationName, importSource, S7TiaCommDriver.StrConnectionString, out filebase, out InMemory, out targetIsFile);            
        }

        private string GetAglinkFileFileNameOnly(string stationName, S7TIAImportParser.ImportSourceManagement importSource, string targerConn)
        {
            string SymbolicFile;

            // 1st time check Movicon Prject Source DB ? File ?
            if (!MoviconProjectSourceIsFile.HasValue)
            {
                InMemoryDataStore InMemory;
                bool TargetIsFile;
                S7TIADriver S7TiaCommDriver = (S7TIADriver)CommDriver;                
                string connect = CommunicationDriver.GetConnectionString(targerConn, "Drivers", stationName, ".tia");
                // test Movicon project source File ? DB ?
                using (IDataLayer dl = CommunicationDriver.GetSpecificDataLayer(connect, out SymbolicFile, out InMemory, out TargetIsFile))
                {
                    if (dl != null)
                        MoviconProjectSourceIsFile = TargetIsFile;
                    else
                        MoviconProjectSourceIsFile = true;
                }                
            }

            // default file name
            if (MoviconProjectSourceIsFile.HasValue && Convert.ToBoolean(MoviconProjectSourceIsFile))
            {
                return string.Format("{0}_S7TIASymbolic{1}", stationName, GetAglinkFileSuffix(importSource));
            }
            else
            {
                string Key = string.Format("{0}{1}", stationName, importSource);
                // file name with random part to differ from other Movicon instance
                if (!mapStationTiaFileFromDB.ContainsKey(Key))
                    mapStationTiaFileFromDB[Key] = string.Format("{0}_{1}_S7TIASymbolic{2}", stationName, Guid.NewGuid(), GetAglinkFileSuffix(importSource));

                return mapStationTiaFileFromDB[Key];
            }
        }

        public IDataLayer GetAglinkFile(string stationName, S7TIAImportParser.ImportSourceManagement importSource, string targerConn, out string filebase, out InMemoryDataStore InMemory, out bool targetIsFile)
        {
            S7TIADriver S7TiaCommDriver = (S7TIADriver)CommDriver;
            string FileName = GetAglinkFileFileNameOnly(stationName, importSource, targerConn);
            string connect = CommunicationDriver.GetConnectionString(targerConn, "Drivers", FileName, ".tia");
            IDataLayer dl = CommunicationDriver.GetSpecificDataLayer(connect, out filebase, out InMemory, out targetIsFile);

            // from db project, use os user temporary path
            if (!targetIsFile)
                filebase = Path.Combine(Path.GetTempPath(), string.Format("{0}{1}", FileName, ".tia"));

            return (dl);
        }

        public string GetAglinkFileBackUp(string stationName, out bool targetIsFile)
        {
            string filebase;
            InMemoryDataStore InMemory;
            
            S7TIADriver S7TiaCommDriver = (S7TIADriver)CommDriver;            
            string FileName = string.Format("{0}_S7TIASymbolic{1}_BackUp", stationName, GetAglinkFileSuffix(S7TIAImportParser.ImportSourceManagement.Project));
            string connect = CommunicationDriver.GetConnectionString(S7TiaCommDriver.StrConnectionString, "Drivers", FileName, ".tia");
            using (IDataLayer dl = CommunicationDriver.GetSpecificDataLayer(connect, out filebase, out InMemory, out targetIsFile))
            {
                // from db project, use os user temporary path
                if (!targetIsFile)
                    filebase = Path.Combine(Path.GetTempPath(), string.Format("{0}{1}", FileName, ".tia"));
            }

            return filebase;
        }

        public string GetListNameDBsNameTablesFileOldFormat(string stationName)
        {
            string Filebase;
            InMemoryDataStore InMemory;
            bool TargetIsFile;

            S7TIADriver S7TiaCommDriver = (S7TIADriver)CommDriver;
            string FileName = string.Format("{0}_ListDataBlocks&TablesName", stationName);
            string connect = CommunicationDriver.GetConnectionString(S7TiaCommDriver.StrConnectionString, "Drivers", FileName, ".list");
            using (IDataLayer dl = CommunicationDriver.GetSpecificDataLayer(connect, out Filebase, out InMemory, out TargetIsFile))
            {
                if (dl == null)
                    return string.Empty;
            }

            return Filebase;
        }

        public string GetListNameDBsNameTablesFileNameOnly(string stationName)
        {
            return string.Format("{0}_ListDataBlocksAndTablesName", stationName);
        }

        public IDataLayer GetListNameDBsNameTablesFile(string stationName, out string filebase, out InMemoryDataStore InMemory, out bool targetIsFile)
        {
            S7TIADriver S7TiaCommDriver = (S7TIADriver)CommDriver;            
            string connect = CommunicationDriver.GetConnectionString(S7TiaCommDriver.StrConnectionString, "Drivers", GetListNameDBsNameTablesFileNameOnly(stationName), ".xml");
            IDataLayer dl = CommunicationDriver.GetSpecificDataLayer(connect, out filebase, out InMemory, out targetIsFile);

            return (dl);
        }

        public string GetImportProjectTagsFileNameOnly(string stationName) {
            return string.Format("{0}_ImportProjectTags", stationName);
        }

        IDataLayer GetImportProjectTagsFile(string stationName, out string filebase, out InMemoryDataStore InMemory, out bool targetIsFile)
        {            
            S7TIADriver S7TiaCommDriver = (S7TIADriver)CommDriver;

            string connect = CommunicationDriver.GetConnectionString(S7TiaCommDriver.StrConnectionString, "Drivers", GetImportProjectTagsFileNameOnly(stationName), ".xml");
            IDataLayer dl = CommunicationDriver.GetSpecificDataLayer(connect, out filebase, out InMemory, out targetIsFile);

            return (dl);
        }

        private bool FileExist(string fileTia) { 
            try
            {
                return File.Exists(fileTia);                    
            } catch (Exception ex) { }

            return false;
        }

        // try to create (on local PC in user\temp directory) .tia file from database (in case of project from db)
        private void TryToGetSymbolFileFromDatabase(string StationName)
        {
            string SymbolicFile;
            InMemoryDataStore InMemory;
            bool TargetIsFile;

            using (IDataLayer idl = GetAglinkFile(StationName, S7TIAImportParser.ImportSourceManagement.None, out SymbolicFile, out InMemory, out TargetIsFile)) { 
                if (idl == null)
                    return;

                // from db
                if (!TargetIsFile)
                {
                    using (UnitOfWork ufw = new UnitOfWork(idl))
                    {
                        // get tia file "body" by station
                        S7TIATiaFile TiaFile = (from S in new XPQuery<S7TIATiaFile>(ufw).AsParallel() where S.StationName == StationName select S).ToList().FirstOrDefault();
                        if (TiaFile != null)
                        {
                            try
                            {
                                // get source type by extension stored into db
                                SymbolicFile = GetAglinkFile(StationName, GetAglinkFileTypeBySuffix(TiaFile.Source));
                                using (BinaryWriter binWriter = new BinaryWriter(File.Open(SymbolicFile, FileMode.Create)))
                                {
                                    binWriter.Write(TiaFile.FileBody);
                                }
                            }
                            catch (Exception ex) { }
                        }
                    }
                }
            }
        }

        private S7TIAImportParser.ImportSourceManagement GetSymbolFileSource(string StationName)
        {
            string projectFile = string.Empty;
            S7TIAImportParser.ImportSourceManagement ImportSource = S7TIAImportParser.ImportSourceManagement.None;

            // try to create .tia file from database (in case of Movicon project from db)
            TryToGetSymbolFileFromDatabase(StationName);

            ImportSource = S7TIAImportParser.ImportSourceManagement.Project;
            projectFile = GetAglinkFile(StationName, S7TIAImportParser.ImportSourceManagement.Project);
            if (!FileExist(projectFile)) {
                ImportSource = S7TIAImportParser.ImportSourceManagement.Plc;
                projectFile = GetAglinkFile(StationName, S7TIAImportParser.ImportSourceManagement.Plc);
                if (!FileExist(projectFile))
                {
                    // old file (no additional extension) generated by old driver --> try to rename with new naming format
                    ImportSource = S7TIAImportParser.ImportSourceManagement.None;
                    projectFile = GetAglinkFile(StationName, S7TIAImportParser.ImportSourceManagement.None);
                    if (FileExist(projectFile))
                    {
                        try
                        {
                            string projectFileNewFormat = string.Empty;
                            // driver was forced to mantain .tia imported from project 
                            if (_ImportSource == S7TIAImportParser.ImportSourceManagement.Project)
                            {
                                ImportSource = S7TIAImportParser.ImportSourceManagement.Project;
                                projectFileNewFormat = GetAglinkFile(StationName, S7TIAImportParser.ImportSourceManagement.Project);
                            }
                            else
                            {
                                ImportSource = S7TIAImportParser.ImportSourceManagement.Plc;
                                projectFileNewFormat = GetAglinkFile(StationName, S7TIAImportParser.ImportSourceManagement.Plc);
                            }
                            //rename file name using standard name
                            File.Move(projectFile, projectFileNewFormat);
                            if (FileExist(projectFileNewFormat))
                            {
                                projectFile = projectFileNewFormat;
                                string szAux = String.Format("{0} {1}", GetStationName, String.Format(Properties.Resources.SymbolFileWasRenamed, projectFile, projectFileNewFormat));
                                CommDriver.OnSystemEvent(ObjectIds.Server, szAux, Opc.Ua.EventSeverity.High);
                            }
                            else
                            {
                                // no .tia file present into driver's dir
                                ImportSource = S7TIAImportParser.ImportSourceManagement.None;
                            }
                        }
                        catch (Exception ex) { }
                    }
                }            
            }

            return ImportSource;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Load File Symbol. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool LoadFileSymbol(string StationName, S7TIAImportParser.ImportSourceManagement importSource)
        {
            int ret = AGL4.AGL40_SUCCESS;
            string projectFile = GetAglinkFile(StationName, importSource);
            if (projectFile == string.Empty)
                return false;

            if (!File.Exists(projectFile))
            {
                if(!bErrorLoadSymbolFile)
                {
                    string szAux = String.Format("{0} {1}",StationName, String.Format(Properties.Resources.ErrorSymbolFileNotFound, projectFile));
                    CommDriver.OnSystemEvent(ObjectIds.Server, szAux, Opc.Ua.EventSeverity.High);
                    bErrorLoadSymbolFile = true;
                }
               
                return false;
            }

            IntPtr rootSchemaNodeHandle = HandleFile;

            // Before accessing a new project, make sure to free a previously used handle.
            if (rootSchemaNodeHandle != IntPtr.Zero)
            {
                ret = AGL4.Symbolic_FreeHandle(rootSchemaNodeHandle);
                rootSchemaNodeHandle = IntPtr.Zero;

                if (ret != AGL4.AGL40_SUCCESS)
                {
                    if (!bErrorLoadSymbolFile)
                    {                        
                        string szAux = String.Format("{0} {1}",StationName , String.Format(Properties.Resources.ErrorFromDllAGLink, ret));
                        CommDriver.OnSystemEvent(ObjectIds.Server, szAux, Opc.Ua.EventSeverity.High);
                        bErrorLoadSymbolFile = true;
                    }
                    return false;
                }
            }

            if (rootSchemaNodeHandle == IntPtr.Zero)
            { 
                ret = AGL4.Symbolic_LoadAGLinkSymbolsFromFile(projectFile,  ref rootSchemaNodeHandle);
                if (ret == AGL4.AGL40_SUCCESS)
                {
                    HandleFile = rootSchemaNodeHandle;
                }
                else 
                {
                    if (!bErrorLoadSymbolFile)
                    {
                        string szAux = String.Format("{0} {1}", StationName, String.Format(Properties.Resources.ErrorFromDllAGLink, ret));
                        CommDriver.OnSystemEvent(ObjectIds.Server, szAux, Opc.Ua.EventSeverity.High);
                        bErrorLoadSymbolFile = true;
                    }
                    return false;
                }
            }

            return ret == AGL4.AGL40_SUCCESS;

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Freee File Symbol. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool FreeFileSymbol()
        {
            int ret = AGL4.AGL40_SUCCESS;
            if (HandleFile != IntPtr.Zero)
            {
                IntPtr rootSchemaNodeHandle = HandleFile;
                ret = AGL4.Symbolic_FreeHandle(rootSchemaNodeHandle);
                if(ret == AGL4.AGL40_SUCCESS)
                {
                    HandleFile = IntPtr.Zero;
                    conState = State.eNotInit;
                }
                else
                {                                        
                    string szAux = String.Format("{0} {1}", szStationName, String.Format(Properties.Resources.ErrorFromDllAGLink, ret));
                    CommDriver.OnSystemEvent(ObjectIds.Server, szAux, Opc.Ua.EventSeverity.High);
                }
                nErrorLibraryCode = ret;
            }

            return ret == AGL4.AGL40_SUCCESS;
        }

        public bool SaveSymbolicFile(string stationName, S7TIAImportParser.ImportSourceManagement importSource)
        {
            return SaveSymbolicFile(stationName, importSource, IntPtr.Zero);
        }

        public bool SaveSymbolicFile(string stationName,S7TIAImportParser.ImportSourceManagement importSource, IntPtr schemaNodeHandle)
        {
            int nRet;
            string SymbolicFile;
            bool InitHandle = false;
            bool TargetIsFile = false;

            try
            {                
                #region try so save into database .tia file content                
                InMemoryDataStore InMemorySubscriberID = null;
                using (IDataLayer idl = GetAglinkFile(stationName, importSource, out SymbolicFile, out InMemorySubscriberID, out TargetIsFile))
                {
                    if (idl == null)
                        return false;

                    if (TargetIsFile)
                    {
                        if (File.Exists(SymbolicFile))
                            File.Delete(SymbolicFile);

                        // .tia file generarted by old driver (file name without _Plc, _Prj)
                        string SymbolicFileOldFormat = GetAglinkFile(stationName, S7TIAImportParser.ImportSourceManagement.None);
                        if (File.Exists(SymbolicFileOldFormat))
                            File.Delete(SymbolicFileOldFormat);
                    }

                    if (schemaNodeHandle == IntPtr.Zero)
                    {
                        InitHandle = true;
                        // read data from plc to recreate .tia file
                        nRet = AGL4.Symbolic_LoadAGLinkSymbolsFromPLC(connNr, ref schemaNodeHandle);
                        if (nRet != AGL4.AGL40_SUCCESS)
                            return false;
                    }

                    // from (plc/project) data recreate .tia file
                    nRet = AGL4.Symbolic_SaveAGLinkSymbolsToFile(schemaNodeHandle, SymbolicFile);
                    if (nRet != AGL4.AGL40_SUCCESS)
                        return (false);

                    if (InitHandle)
                    {
                        AGL4.Simotion_FreeHandle(schemaNodeHandle);
                        schemaNodeHandle = IntPtr.Zero;
                    }

                    // to db
                    if (!TargetIsFile)
                    {
                        using (UnitOfWork ufw = new UnitOfWork(idl))
                        {
                            // get .tia file associated to station (only 1)
                            List<S7TIATiaFile> Files = (from S in new XPQuery<S7TIATiaFile>(ufw).AsParallel() where S.StationName == stationName select S).ToList();
                            if (Files.Count > 0)
                            {
                                // before to import delete previous data
                                ufw.Delete(Files);
                                ufw.CommitChanges();
                            }

                            S7TIATiaFile TiaFile = new S7TIATiaFile(ufw);
                            TiaFile.StationName = stationName;

                            TiaFile.Source = GetAglinkFileSuffix(importSource);
                            TiaFile.FileBody = File.ReadAllBytes(SymbolicFile);
                            TiaFile.LastUpdate = DateTime.UtcNow;

                            // commit changes
                            ufw.CommitChanges();
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                return false;
            }

            if (TargetIsFile)
            {
                #region make a copy (if necessary/present) of .tia file comning from Project; finally, remove it
                if (importSource == S7TIAImportParser.ImportSourceManagement.Project)
                {
                    try
                    {
                        if (File.Exists(SymbolicFile))
                        {
                            string projectFileBackuP = GetAglinkFileBackUp(stationName, out TargetIsFile);
                            File.Copy(SymbolicFile, projectFileBackuP, true);   // force overwrite if backup's file already exist
                        }
                    }
                    catch (Exception ex) { }
                }
                #endregion

                #region .tia file cleanup
                // try to remove .tia file coming from other source
                switch (importSource)
                {
                    case S7TIAImportParser.ImportSourceManagement.Project:
                        SymbolicFile = GetAglinkFile(stationName, S7TIAImportParser.ImportSourceManagement.Plc);
                        break;
                    case S7TIAImportParser.ImportSourceManagement.Plc:
                        SymbolicFile = GetAglinkFile(stationName, S7TIAImportParser.ImportSourceManagement.Project);
                        break;
                }
                try
                {
                    if (File.Exists(SymbolicFile))
                        File.Delete(SymbolicFile);
                }
                catch (Exception ex) { }
                #endregion
            }

            return (true);
        }

        /// <summary>
        /// Get Symbolic file contents (binary format) --> always get from file on local disk
        /// </summary>
        /// <param name="stationName"></param>
        /// <param name="importSource"></param>
        /// <returns></returns>
        private byte[] GetSymbolicFileBody(string stationName, S7TIAImportParser.ImportSourceManagement importSource) {

            byte[] FileBody = null;

            string SymbolicFile = GetAglinkFile(stationName, importSource);
            try
            {
                if (File.Exists(SymbolicFile))
                    FileBody = File.ReadAllBytes(SymbolicFile);
            } catch (Exception ex)
            {
                FileBody = null;
            }

            return FileBody;
        }

        private bool SaveSymbolicFileBody(string stationName, S7TIAImportParser.ImportSourceManagement importSource, string targetconn, byte[] fileBody)
        {
            bool Result = false;
            InMemoryDataStore InMemorySubscriberID = null;            
            string SymbolicFile;
            bool TargetIsFile;

            using (IDataLayer idl = GetAglinkFile(stationName, importSource, targetconn, out SymbolicFile, out InMemorySubscriberID, out TargetIsFile))
            {
                if (idl == null)
                    return false;

                if (TargetIsFile)
                {
                    try
                    {
                        using (BinaryWriter binWriter = new BinaryWriter(File.Open(SymbolicFile, FileMode.Create)))
                        {
                            binWriter.Write(fileBody);
                        }
                        Result = true;
                    }
                    catch (Exception ex)
                    {
                        Result = false;
                    }
                }
                else // to DB
                {
                    using (UnitOfWork ufw = new UnitOfWork(idl))
                    {
                        S7TIATiaFile TiaFile = new S7TIATiaFile(ufw);
                        TiaFile.StationName = stationName;

                        TiaFile.Source = GetAglinkFileSuffix(importSource);
                        TiaFile.FileBody = fileBody;
                        TiaFile.LastUpdate = DateTime.UtcNow;

                        // commit changes
                        ufw.CommitChanges();
                    }
                }
            }

            return Result;
        }

        public bool CopySymbolicFile(string stationName, string sourceconn, string targetconn)
        {
            bool Result = false;

            SymbolFileImportSource = GetSymbolFileSource(stationName);

            if (SymbolFileImportSource != S7TIAImportParser.ImportSourceManagement.None) {

                byte[] FileBody = GetSymbolicFileBody(stationName, SymbolFileImportSource);

                if (FileBody != null && FileBody.Length>0)
                    Result = SaveSymbolicFileBody(stationName, SymbolFileImportSource, targetconn, FileBody);
            }

            return Result;
        }

        /// <summary>
        /// Check if List of Data Blocks And Tables was previously written
        /// </summary>
        /// <param name="stationName"></param>
        /// <param name="idl"></param>
        /// <param name="ufw"></param>
        /// <returns></returns>
        private bool IsListaDataBlocksAndTablesNotEmpty(string stationName)
        {
            bool HasData = false;
            InMemoryDataStore InMemorySubscriberID = null;
            string pathListaDataBlocksAndTables;
            bool TargetIsFile;

            using (IDataLayer idl = GetListNameDBsNameTablesFile(stationName, out pathListaDataBlocksAndTables, out InMemorySubscriberID, out TargetIsFile))
            {
                if (idl == null)
                    return false;

                if (TargetIsFile)
                    TryToGetListDataBlockAndTableFromOldFormat(stationName);

                using (UnitOfWork ufw = new UnitOfWork(idl))
                {
                    // get tag list from selected station
                    List<S7TIADBsNameaAndTables> Objects = (from O in new XPQuery<S7TIADBsNameaAndTables>(ufw).AsParallel() where O.StationName == stationName select O).ToList();
                    HasData = (Objects.Count > 0);
                }
            }

            return HasData;
        }

        public bool DeleteListaDataBlocksAndTables(string stationName)
        {                                    
            InMemoryDataStore InMemorySubscriberID = null;
            string pathListaDataBlocksAndTables;
            bool TargetIsFile;

            using (IDataLayer idl = GetListNameDBsNameTablesFile(stationName, out pathListaDataBlocksAndTables, out InMemorySubscriberID, out TargetIsFile))
            {
                if (idl == null)
                    return false;

                using (UnitOfWork ufw = new UnitOfWork(idl))
                {
                    // get tag list from selected station
                    List<S7TIADBsNameaAndTables> Objects = (from S in new XPQuery<S7TIADBsNameaAndTables>(ufw).AsParallel() where S.StationName == stationName select S).ToList();
                    if (Objects.Count > 0)
                    {
                        // when all objects was deleted fill was not deleted (header and so on remains)
                        ufw.Delete(Objects);
                        ufw.CommitChanges();
                    }

                    if (TargetIsFile)
                    {

                        InMemorySubscriberID.WriteXml(pathListaDataBlocksAndTables);

                        try
                        {
                            // try also to remove old file format (.list)
                            string pathListaDataBlocksAndTablesOldFormat = GetListNameDBsNameTablesFileOldFormat(stationName);
                            if (File.Exists(pathListaDataBlocksAndTablesOldFormat))
                                File.Delete(pathListaDataBlocksAndTablesOldFormat);
                        }
                        catch (Exception ex) { }
                    }
                }
            }

            return (true);
        }

        //public bool IsPlcAccessNotProtectedByPassword(string stationName)
        //{
        //    IntPtr HandleFileTest = new IntPtr();
        //    int nRet = AGL4.Symbolic_LoadAGLinkSymbolsFromPLC(connNr, ref HandleFileTest);
        //    if (nRet != AGL4.AGL40_SUCCESS)            
        //        return false;

        //    S7TIAImportParser pareserTia = new S7TIAImportParser();
        //    List<string> dummy = new List<string>();
        //    int HasElements;
        //    pareserTia.GetListDataBlocksAndTables(HandleFileTest, ref dummy);//, out HasElements);

        //    int ret = AGL4.Symbolic_FreeHandle(HandleFile);
        //    HandleFile = IntPtr.Zero;

        //    return HasElements;
        //}

        private bool _SaveListDataBlockAndTable(string stationName, List<S7TIAImportParser.DBsNameAndTables> dataBlocksAndTables)
        {                        
            InMemoryDataStore InMemorySubscriberID = null;
            string pathListaDataBlocksAndTables;
            bool TargetIsFile;


            using (IDataLayer idl = GetListNameDBsNameTablesFile(stationName, out pathListaDataBlocksAndTables, out InMemorySubscriberID, out TargetIsFile))
            {
                if (idl == null)
                    return false;

                using (UnitOfWork ufw = new UnitOfWork(idl))
                {
                    foreach (var Obj in dataBlocksAndTables)
                    {
                        S7TIADBsNameaAndTables WriteTag = new S7TIADBsNameaAndTables(ufw);
                        WriteTag.StationName = stationName;
                        WriteTag.ObjectName = Obj.ObjectName;
                        WriteTag.ObjectType = Obj.ObjectType;
                        WriteTag.LastUpdate = DateTime.UtcNow;
                    }

                    // commit changes
                    ufw.CommitChanges();

                    if (TargetIsFile)
                        InMemorySubscriberID.WriteXml(pathListaDataBlocksAndTables);
                }
            }

            return true;
        }

        public bool SaveListDataBlockAndTable(string stationName, Int32 conn, IntPtr schemaNodeHandle ,bool forceCreateFile)
        {
            int nRet;
            bool InitHandle = false;
                        
            if (IsListaDataBlocksAndTablesNotEmpty(stationName))
            {
                if (forceCreateFile)
                    DeleteListaDataBlocksAndTables(stationName);
                else // file already exist --> exit 
                    return true;
            }

            if (schemaNodeHandle == IntPtr.Zero) {
                InitHandle = true;
                try
                {
                    nRet = AGL4.Symbolic_LoadAGLinkSymbolsFromPLC(conn, ref schemaNodeHandle);
                    if (nRet != AGL4.AGL40_SUCCESS)
                        return false;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            S7TIAImportParser parserTia = new S7TIAImportParser();
            List<S7TIAImportParser.DBsNameAndTables> DataBlocksAndTables = new List<S7TIAImportParser.DBsNameAndTables>();
            parserTia.GetListDataBlocksAndTables(schemaNodeHandle, ref DataBlocksAndTables);

            if (InitHandle)
            {
                AGL4.Simotion_FreeHandle(schemaNodeHandle);
                schemaNodeHandle = IntPtr.Zero;
            }

            return _SaveListDataBlockAndTable(stationName, DataBlocksAndTables);            
        }

        private bool SaveListDataBlockAndTable(string stationName,bool forceCreateFile)
        {
            return SaveListDataBlockAndTable(stationName, 0, IntPtr.Zero, forceCreateFile);
        }

        /// <summary>
        /// If file exist, try to convert data create by old version of driver
        /// </summary>
        /// <param name="stationName"></param>
        private void TryToGetListDataBlockAndTableFromOldFormat(string stationName)
        {
            string ObjectType;
            string ObjectName;

            string pathListaDataBlocksAndTablesOldFormat = GetListNameDBsNameTablesFileOldFormat(stationName);
            try
            {
                if (File.Exists(pathListaDataBlocksAndTablesOldFormat))
                {
                    List<S7TIAImportParser.DBsNameAndTables> DataBlocksAndTables = new List<S7TIAImportParser.DBsNameAndTables>();
                    using (var sr = new StreamReader(pathListaDataBlocksAndTablesOldFormat)) {                    
                        while (sr.Peek() >= 0) { 
                            string Line = sr.ReadLine();
                            // :datablock
                            ObjectType = string.Format(":{0}", S7TIAImportParser.TIA_PROJECT_DATABLOCK_TYPE);
                            if (Line.EndsWith(ObjectType))
                            {
                                ObjectName = Line.Substring(0, Line.Length - ObjectType.Length).Trim();
                                if (!string.IsNullOrEmpty(ObjectName))
                                    DataBlocksAndTables.Add(new S7TIAImportParser.DBsNameAndTables(S7TIAImportParser.TIA_PROJECT_DATABLOCK_TYPE, ObjectName));
                            }
                            // :tagtable
                            ObjectType = string.Format(":{0}", S7TIAImportParser.TIA_PROJECT_TAG_TABLE_TYPE);
                            if (Line.EndsWith(ObjectType))
                            {
                                ObjectName = Line.Substring(0, Line.Length - ObjectType.Length).Trim();
                                if (!string.IsNullOrEmpty(ObjectName))
                                    DataBlocksAndTables.Add(new S7TIAImportParser.DBsNameAndTables(S7TIAImportParser.TIA_PROJECT_TAG_TABLE_TYPE, ObjectName));
                            }
                        }
                    }

                    // save data into new file format
                    if (_SaveListDataBlockAndTable(stationName, DataBlocksAndTables))
                        // delete old file format
                        File.Delete(pathListaDataBlocksAndTablesOldFormat);
                }
            }
            catch (Exception ex) { }
        }

        private bool LoadListDataBlockAndTable(string stationName)
        {            
            InMemoryDataStore InMemorySubscriberID = null;
            string pathListaDataBlocksAndTables;
            bool TargetIsFile;

            listDataBlocksAndTables = new List<string>();

            using (IDataLayer idl = GetListNameDBsNameTablesFile(stationName, out pathListaDataBlocksAndTables, out InMemorySubscriberID, out TargetIsFile))
            {
                if (idl == null)
                    return false;

                if (TargetIsFile)
                    TryToGetListDataBlockAndTableFromOldFormat(stationName);

                using (UnitOfWork ufw = new UnitOfWork(idl))
                {
                    // get tag list from selected station
                    List<S7TIADBsNameaAndTables> Objects = (from O in new XPQuery<S7TIADBsNameaAndTables>(ufw).AsParallel() where O.StationName == stationName select O).ToList();
                    if (Objects.Count > 0)
                    {
                        // load only datablock type
                        listDataBlocksAndTables = (from O in Objects where O.ObjectType == S7TIAImportParser.TIA_PROJECT_DATABLOCK_TYPE && !string.IsNullOrEmpty(O.ObjectName) select O.ObjectName).ToList();
                    }
                }
            }

            return (true);
        }

        public void LoadDefaultListTable()
        {
            if (listDataBlocksAndTables == null)
                listDataBlocksAndTables = new List<string>();

            // add empty string to allow parser to correct address itself in case no block match with address
            listDataBlocksAndTables.Add(string.Empty);
        }
                
        public void SaveImportProjectTags(string stationName, List<S7TIAImportProjectTag> tags)
        {
            string FilebaseSubscriberID = string.Empty;
            InMemoryDataStore InMemorySubscriberID = null;
            bool TargetIsFile = false;

            // retrive connection to data source (file/db/ecc)
            using (IDataLayer idl = GetImportProjectTagsFile(stationName, out FilebaseSubscriberID, out InMemorySubscriberID, out TargetIsFile))
            {
                if (idl == null)
                    return;

                using (UnitOfWork ufw = new UnitOfWork(idl))
                {
                    // get tag list from selected station
                    List<S7TIAImportProjectTag> Tags = (from S in new XPQuery<S7TIAImportProjectTag>(ufw).AsParallel() where S.StationName == stationName select S).ToList();
                    if (Tags.Count > 0)
                    {
                        // before to import delete previous
                        ufw.Delete(Tags);
                        ufw.CommitChanges();
                    }

                    DateTime LastUpdate = DateTime.UtcNow;
                    // populate the list of tags to write
                    List<S7TIAImportProjectTag> TagsToWrite = new List<S7TIAImportProjectTag>();
                    foreach (var tag in tags)
                    {
                        if (tag.IsDataCorrect())
                        {
                            S7TIAImportProjectTag WriteTag = new S7TIAImportProjectTag(ufw);
                            WriteTag.StationName = stationName;
                            WriteTag.Plc = tag.Plc;
                            WriteTag.Prj = tag.Prj;
                            WriteTag.LastUpdate = LastUpdate;
                        }
                    }

                    // commit changes
                    ufw.CommitChanges();

                    if (TargetIsFile)
                        InMemorySubscriberID.WriteXml(FilebaseSubscriberID);
                }
            }
        }

        /// <summary>
        /// Load file/db table containing the list of tag imported from project with both address (plc and project)
        /// </summary>
        /// <param name="stationName"></param>
        /// <param name="importSource"></param>
        /// <returns></returns>
        public bool LoadImportProjectTags(string stationName, S7TIAImportParser.ImportSourceManagement importSource)
        {
            bool Result = false;
            string FilebaseSubscriberID = string.Empty;
            InMemoryDataStore InMemorySubscriberID = null;
            bool TargetIsFile = false;

            mapTagsPlcAndProject = new Dictionary<string, string>();

            // get tags data source 
            using (IDataLayer idl = GetImportProjectTagsFile(stationName, out FilebaseSubscriberID, out InMemorySubscriberID, out TargetIsFile))
            {
                if (idl == null)
                    return false;

                using (UnitOfWork ufw = new UnitOfWork(idl))
                {
                    try
                    {
                        // get tag list of tags from selected station
                        List<S7TIAImportProjectTag> Tags = (from S in new XPQuery<S7TIAImportProjectTag>(ufw).AsParallel() where S.StationName == stationName select S).ToList();
                        foreach (var tag in Tags)
                        {
                            // row format   <plc tag address>FILE_FIELD_SEPARATOR<plc tag address for project>:
                            if (tag.IsDataCorrect())
                            {
                                switch (importSource)
                                {
                                    case S7TIAImportParser.ImportSourceManagement.Plc:
                                        // use to convert from Project address to plc address
                                        if (!mapTagsPlcAndProject.ContainsKey(tag.Prj))
                                            mapTagsPlcAndProject[tag.Prj] = tag.Plc;
                                        break;
                                    case S7TIAImportParser.ImportSourceManagement.Project:
                                        // use to convert from plc address to project address
                                        if (!mapTagsPlcAndProject.ContainsKey(tag.Plc))
                                            mapTagsPlcAndProject[tag.Plc] = tag.Prj;
                                        break;
                                }
                            }
                        }
                        // no elements loaded
                        if (mapTagsPlcAndProject.Count > 0)
                            Result = true;
                    }
                    catch (Exception ex) { }
                }
            }

            return Result;
        }

        /// <summary>
        /// Remove Tia File created from db (next start-up driver will create a new one with a file name with random part)
        /// </summary>
        private void DeleteTiaFileCreatedFromDB()
        {
            if (mapStationTiaFileFromDB.Count != 0)
            {
                foreach (var TiaFile in mapStationTiaFileFromDB.Values)
                {
                    try
                    {
                        // Tia file created from Movicon DB project are always into local user temp directory
                        if (File.Exists(Path.Combine(Path.GetTempPath(), string.Format("{0}{1}", TiaFile, ".tia"))))
                            File.Delete(Path.Combine(Path.GetTempPath(), string.Format("{0}{1}", TiaFile, ".tia")));
                    }
                    catch (Exception ex) { }
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Dispose()
        {
            DeleteTiaFileCreatedFromDB();
        }

        public void ResetInternal()
        {            
            listDataBlocksAndTables = null;
            SymbolFileImportSource = S7TIAImportParser.ImportSourceManagement.None;
            ImportFromProjectMappingFileIsPresent = false;
            mapTagsPlcAndProject = null;
            mapStationTiaFileFromDB.Clear();
            MoviconProjectSourceIsFile = null;
        }

        /// <summary>
        /// Region Properties
        /// </summary>
        #region Properties

        private Int32 getDevNr
        {
            get
            {
                devNrCnt++;
                return devNrCnt - 1;
            }
        }
        public IntPtr HandleFile
        {
            get
            {
                return pHandleFile;
            }
            set
            {
                pHandleFile = value;
            }
        }

        public string GetStationName
        {
            get
            {
                return szStationName;
            }
        }

        public int GetErrorLibraryCode
        {
            get
            {
                return nErrorLibraryCode;
            }
        }

        // when _ImportSource == S7TIAImportParser.ImportSourceManagement.Project driver disable import from PLC
        private S7TIAImportParser.ImportSourceManagement _ImportSource;
        public S7TIAImportParser.ImportSourceManagement ImportSource
        {
            set
            {
                _ImportSource = value;
            }
            get
            {
                return _ImportSource;
            }
        }
                
        /// <summary>
        /// End Region Properties
        /// </summary>

        #endregion
    }
    /// <summary>
    /// End class PlcConnection
    /// </summary>

    public class S7TIAChannel : Channel
    {
        #region Constructors

        /// <summary>
        /// Initializes the S7TIAChannel object.
        /// </summary>
        public S7TIAChannel(CommunicationDriver commdriver, S7TIAChannelSettings settings)
            : base(commdriver, settings, false)
        {
            _TcpChannelHostName = settings.TcpChannelSettingsHostName;
            _TcpChannelHostPort = settings.TcpChannelSettingsHostPort;
            plcConnection = new PlcConnection();            
        }

        #endregion


        #region Members
        List<S7TIACommJob> nextlist = new List<S7TIACommJob>();
        List<CommJob> ListJobExec = new List<CommJob>();
        public PlcConnection plcConnection;
        bool lastConnectionError = false;        
        #endregion

        #region Abstracts Methods

        public override bool IsDeviceOpen()
        {
            bool returnValue = plcConnection.IsConnected();
            SetStateCommandVariableBit(!returnValue, (UInt16)ChannelVariableBits.ChannelUnconnected);
            return returnValue;
        }

        public override bool DeviceOpen()
        {
            if (!plcConnection.IsConnected())
            {
                plcConnection.Connect();
            }
            if (!IsDeviceOpen() && ! lastConnectionError )
                CommDriver.OnSystemEvent(ObjectIds.Server, Properties.Resources.ErrorConnection, Opc.Ua.EventSeverity.High);
            lastConnectionError = !IsDeviceOpen();
            return !lastConnectionError;
        }

        public bool DeviceOpen(bool bLoadFile = true)
        {
            if (!plcConnection.IsConnected())
            {
                plcConnection.Connect(bLoadFile);
            }
            if (!IsDeviceOpen() && !lastConnectionError)
                CommDriver.OnSystemEvent(ObjectIds.Server, Properties.Resources.ErrorConnection, Opc.Ua.EventSeverity.High);
            lastConnectionError = !IsDeviceOpen();
            return !lastConnectionError;
        }

        public override bool DeviceClose()
        {
            return plcConnection.Disconnect();
        }

        public override bool DeviceRead(byte[] Buffer, uint Count) { return true; }
        public override bool DeviceWrite(byte[] Buffer, uint Count) { return true; }
        public override uint GetBytesToRead() { return 1; }
        public override uint GetBytesToWrite() { return 1; }

        #endregion

        #region Override Methods

        public override bool TestChannelComm()
        {
            if (InitPLCConnect())
            {
                bool bRet = false;
                //on test channel disable driver to generate any kind of symbolic files (.tia, .list)
                bRet = DeviceOpen(false);
                DisConnection();

                return bRet;
            }
            return false;
        }

        protected override void WorkingThread(object data)
        {

            InitPLCConnect();

            int sleepCycle = (Int32)WaitTime;
            if (sleepCycle == 0)
            {
                sleepCycle = 1;
            }

            ListJobPending.Clear();
            ListJobExecuted.Clear();

            NextScheduleTimeJobsList = DateTime.UtcNow;
            int loop = 0;
            while (true)
            {

                nextlist.Clear();
                if (ListJobPending.Count == 0 || MultiPointProtocol)
                {
                    ScheduleListJob();
                    lock (lockThreadObject)
                    {
                        if (SynchroJob != null)
                            nextlist.Add(SynchroJob as S7TIACommJob);
                        else
                            GetNextPendingList(ref nextlist);
                    }
                    if (nextlist.Count > 0)
                        ListJobPending.AddRange(nextlist);
                }

                if (nextlist.Count > 0)
                {
                    if (!IsDeviceOpen())
                    {
                        DeviceOpen();
                    }

                    lock (lockThreadObject)
                    {
                        ExecuteJobList(nextlist);
                        bool bConnection = DeviceOpen();
                        if (!bConnection)
                        { 
                            SetConnectionError(nextlist);
                        }
                    }
                }

                lock (lockThreadObject)
                {
                    if (ListJobPending.Count > 0)
                    {
                        ListJobExec.Clear();
                        foreach (var job in ListJobPending)
                        {
                            ListJobExecuted.Add(job);
                        }
                        foreach (var job in ListJobExecuted)
                        {
                            job.LastExecutionTime = DateTime.UtcNow;
                            if (SynchroJob != null && SynchroJob == job)
                            {
                                job.ResetSynchro.WaitOne(Timeout);
                                SynchroJob = null;
                                job.ResetSynchro.Reset();
                            }
                            ListJobPending.Remove(job);
                        }
                        ListJobExecuted.Clear();
                    }
                }

                if (ListJobPending.Count == 0 && !KeepOpened && IsDeviceOpen())
                {
                    DeviceClose();
                }

                if (nextlist.Count != 0 && StopWorkerThread.WaitOne(sleepCycle, false))
                {
                    break;
                }
                else if (++loop > 4)
                {
                    loop = 0;
                    if (StopWorkerThread.WaitOne(sleepCycle, false))
                        break;
                }
                StopWorkerThread.WaitOne(0, false);
               
            }

            DisConnection();
            plcConnection.FreeFileSymbol();
            ResetInternal();
        }

        protected void SetConnectionError(List<S7TIACommJob> list)
        {
            for (int i = 0; i<list.Count; i++)
            {
                ExecutedJobArgs eJob = new ExecutedJobArgs();
                eJob.ErrorCode = DriverErrorCodes.ErrorTimeOut;
                eJob.GeneralError = true;
                eJob.Job = list[i];
                OnJobExecuted(eJob);
            }
            return;
        }

        private void ResetInternal()
        {
            plcConnection.ResetInternal();

            foreach (S7TIAStation s in CommDriver.GetChannelStations(this))
            {
                foreach (S7TIACommJob j in s.GetWholeJobsList())
                {
                    j.AccessHandle = S7TIACommJob.AccessHandleState.Undefined;
                    j.ClearStructureAGLinkWrite();
                    j.ClearStructureAGLinkRead();
                }
            }
        }

        #endregion


        #region Methods

        protected void GetNextPendingList(ref List<S7TIACommJob> list)
        {
            bool write = false;
            LinkType jLink = LinkType.InputOutput;
            string station = string.Empty;
            bool first = true;
            lock (lockScheduleFlag)
            {
                var queue = GetNextPendingQueue();

                if (queue == null)
                    return;

                while (!queue.IsEmpty && list.Count() < S7TIAProtocol.MAX_AGGREGATED_JOBS)
                {
                    S7TIACommJob j = queue.ElementAt(0) as S7TIACommJob;

                    if (j != null && j.TagsListOnWriting.Count == 0)
                    {
                        if (first)
                        {
                            write = (j.TagsListToWrite.Count > 0 && (j.Type == LinkType.ExceptionOutput || j.Type == LinkType.InputOutput)) || j.Type == LinkType.UnconditionalOutput;//j.TagsListToWrite.Count > 0 || j.Type == LinkType.UnconditionalOutput;
                            station = j.Station.Name;
                            jLink = j.Type;
                            first = false;
                        }
                        bool jwrite = (j.TagsListToWrite.Count > 0 && (j.Type == LinkType.ExceptionOutput || j.Type == LinkType.InputOutput)) || j.Type == LinkType.UnconditionalOutput;
                        if (write)
                        {
                            if (j.Station != null && j.Station.Name == station && jwrite)
                            {
                                list.Add(j);
                            }
                        }
                        else
                        {
                            if (j.Station != null && j.Station.Name == station && !jwrite && j.Type != LinkType.ExceptionOutput)
                            {

                                list.Add(j);
                            }
                        }

                    }
                    CommJob dequeuedJob = null;
                    queue.TryDequeue(out dequeuedJob);
                }
            }
        }

        protected void ExecuteJobList(List<S7TIACommJob> list)
        {
            S7TIAStation s = list[0].Station as S7TIAStation;
            if (s == null)
            {
                return;
            }

            // if .tia file isn't present, all job must be set on errors
            if (plcConnection.SymbolFileImportSource == S7TIAImportParser.ImportSourceManagement.None) {
                foreach (CommJob j in list) {                     
                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                    eJob.ErrorCode = (DriverErrorCodes)S7ErrorCodes.ErrorAGLinkTiaFileNotPresent;
                    base.ExecuteJob(j);
                    eJob.Job = j;
                    OnJobExecuted(eJob);
                }
                return;
            }

            if (!IsDeviceOpen() && 
                s.LastErrorCode == (DriverErrorCodes)S7ErrorCodes.ErrorConnectionToDevice &&
                (DateTime.UtcNow - s.LastErrorTime).TotalMilliseconds < 2000)
            {
                return;
            }           

            if (!IsDeviceOpen())
            {
                //Error connection
                if (s.LastErrorCode != (DriverErrorCodes)S7ErrorCodes.ErrorConnectionToDevice)
                {
                    s.LastErrorCode = (DriverErrorCodes)S7ErrorCodes.ErrorConnectionToDevice;
                    s.LastErrorTime = DateTime.UtcNow;
                }
                return;
            }

            ExchangeData(list);
        
        }

        protected void ExchangeData(List<S7TIACommJob> list)
        {
            for(int index = 0; index < list.Count; index++)
            {
                if (list[index].Type == DriverCodeBase.Enumerators.LinkType.UnconditionalOutput)
                {
                    if (list[index].TagsListToWrite.Count == 0)
                    {
                        list[index].TagsListToWrite.AddRange(list[index].TagsList);
                    }
                }
            }
            if (list[0].Type == DriverCodeBase.Enumerators.LinkType.Input ||
               (list[0].Type == DriverCodeBase.Enumerators.LinkType.InputOutput &&
               list[0].TagsListToWrite.Count == 0))
            {
                ReadData(list);
            }
            else
            {
                WriteData(list);
            }
        }

        protected void ReadData(List<S7TIACommJob> list)
        {
            List<AGL4.SymbolicRW> rwsTmp = new List<AGL4.SymbolicRW>();                        
            List<AGLinkReadWriteElement> Elements = new List<AGLinkReadWriteElement>();

            S7TIAStation s = list[0].Station as S7TIAStation;
            if (s == null)
                return;

            foreach (S7TIACommJob j in list) 
            {
                if (j.AccessHandle == S7TIACommJob.AccessHandleState.Undefined)
                {
                    string szError = string.Empty;
                    string address = GetAddresTypePLC(s, j.StartAddress);
                    if (j.LoadAccessHandleAndSize(plcConnection.HandleFile, s.Name, address, ref szError))
                        j.AccessHandle = S7TIACommJob.AccessHandleState.Found;
                    else
                        j.AccessHandle = S7TIACommJob.AccessHandleState.NotFound;
                }

                if (j.AccessHandle == S7TIACommJob.AccessHandleState.NotFound)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                    eJob.ErrorCode = (DriverErrorCodes)S7ErrorCodes.ErrorFromDllAGLink;
                    base.ExecuteJob(j);
                    eJob.Job = j;
                    OnJobExecuted(eJob);
                }
                else
                {
                    rwsTmp.Add(j.m_stSymbolicRW);
                    // to read from PLC keep rwsTmp[] array index, job and default errorcode (for future use)
                    Elements.Add(new AGLinkReadWriteElement((rwsTmp.Count - 1), j, DriverErrorCodes.ErrorNoError));
                }
                
                if (Elements.Count >= S7TIAProtocol.MAX_AGGREGATED_JOBS)
                    break;
            }

            // no elements to exchange --> exit 
            if (Elements.Count(a=>a.HasRWSSymbolToExchange()) == 0)
                return;

            AGL4.SymbolicRW[] rws = rwsTmp.ToArray();

            int serror = 0;            
            //Read 
            Int32 nRet = AGL4.Symbolic_ReadMixEx(plcConnection.connNr, rws, ref serror, Timeout);
            if (nRet < AGL4.AGL40_SUCCESS)
            {                
                string szAux = String.Format("{0} {1}", s.Name, String.Format(Properties.Resources.ErrorFromDllAGLink, nRet));
                CommDriver.OnSystemEvent(ObjectIds.Server, szAux, Opc.Ua.EventSeverity.High);
                if ((nRet == AGL4.AGL40_TIMEOUT)||
                    (nRet == AGL4.AGL40_CONNECTION_CLOSED) ||
                    (nRet == AGL4.AGL40_NOT_CONNECTED))
                {
                    LastErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorTimeOut;
                }
                else
                {
                    LastErrorCode =  (DriverErrorCodes)S7ErrorCodes.ErrorFromDllAGLink;
                }

                DisConnection();
            }

            foreach (AGLinkReadWriteElement Element in Elements)
            {                
                ExecutedJobArgs eJob = new ExecutedJobArgs();
                S7TIACommJob j = Element.Job;

                AGL4.SymbolicRW PLCData = new AGL4.SymbolicRW();
                if (Element.HasRWSSymbolToExchange())
                    PLCData = rws[Element.SymbolicRWSIndex];

                int ErrorCode = (int)Element.ErrorCode;
                // no initial errore code
                if (ErrorCode == (int)DriverErrorCodes.ErrorNoError)
                {
                    // general error code (connection broken, ecc)
                    if (nRet < AGL4.AGL40_SUCCESS)
                    {
                        ErrorCode = (int)LastErrorCode;
                    }
                    else
                    {
                        // error of single element
                        if (PLCData.SError != 0)
                            ErrorCode = (int)PLCData.SError;
                        else if (PLCData.Result != 0)
                            ErrorCode = (int)PLCData.Result;
                        // generic error
                        else if (!Element.HasRWSSymbolToExchange())
                            ErrorCode = (int)S7ErrorCodes.ErrorFromDllAGLink;
                    }
                }

                if (ErrorCode == (int)DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError)
                {
                    eJob.ErrorCode = DriverErrorCodes.ErrorNoError;

                    switch (j.S7DataFormat)
                    {
                        case S7DataFormats.S7_DTL:
                            {
                                byte[] tmpBuffer = new byte[PLCData.BufferLen];
                                tmpBuffer[0] = PLCData.Buffer[1];
                                tmpBuffer[1] = PLCData.Buffer[0];
                                tmpBuffer[2] = PLCData.Buffer[2];
                                tmpBuffer[3] = PLCData.Buffer[3];
                                tmpBuffer[4] = PLCData.Buffer[4];
                                tmpBuffer[5] = PLCData.Buffer[5];
                                tmpBuffer[6] = PLCData.Buffer[6];
                                tmpBuffer[7] = PLCData.Buffer[7];
                                tmpBuffer[8] = PLCData.Buffer[11];
                                tmpBuffer[9] = PLCData.Buffer[10];
                                tmpBuffer[10] = PLCData.Buffer[9];
                                tmpBuffer[11] = PLCData.Buffer[8];
                                eJob.Values = tmpBuffer;
                            }
                            break;

                        case S7DataFormats.String:                            
                            if (j.TagsList[0].TagNode.ArrayDimension == 0)
                            {
                                uint numChar = (uint)PLCData.Buffer[0];
                                byte[] stringAnswer = new byte[numChar];
                                Array.Copy(PLCData.Buffer, 1, stringAnswer, 0, numChar);
                                eJob.Values = stringAnswer;
                            }
                            else
                            {
                                if (PLCData.BufferLen >= j.TotalJobSize)
                                {
                                    uint numChar = 0;
                                    uint tmpOffsetSource = 1;
                                    uint arrayElementSize = j.StringLength * tmpOffsetSource;
                                    uint arrayElementSizeDevice = (uint)(PLCData.BufferLen / j.TagsList[0].TagNode.ArrayDimension) - tmpOffsetSource;

                                    // movicon buffer
                                    byte[] tmpBuffer = new byte[j.TotalJobSize * tmpOffsetSource];

                                    for (int nuberofCyclin = 0; nuberofCyclin < j.TagsList[0].TagNode.ArrayDimension; nuberofCyclin++)
                                    {
                                        // string size (nr of chars)
                                        numChar = (uint)PLCData.Buffer[((arrayElementSizeDevice + tmpOffsetSource) * nuberofCyclin)];
                                        if (numChar > j.StringLength)
                                            numChar = j.StringLength;

                                        // real string size depend on j.S7DataFormat
                                        Array.Copy(PLCData.Buffer, ((arrayElementSizeDevice * nuberofCyclin) + ((nuberofCyclin + 1) * tmpOffsetSource)), tmpBuffer, (arrayElementSize * nuberofCyclin), (numChar * tmpOffsetSource));
                                    }
                                    eJob.Values = tmpBuffer;
                                } 
                                else
                                {
                                    eJob.Values = PLCData.Buffer;
                                }
                            }
                            break;

                        case S7DataFormats.WString:
                            {
                                if (j.TagsList[0].TagNode.ArrayDimension == 0)
                                {
                                    uint numChar = (uint)PLCData.Buffer[0] * 2;
                                    byte[] stringAnswer = new byte[numChar];
                                    Array.Copy(PLCData.Buffer, 2, stringAnswer, 0, numChar);
                                    eJob.Values = stringAnswer;
                                } 
                                else
                                {
                                    if (PLCData.BufferLen >= j.TotalJobSize)
                                    {
                                        uint numChar = 0;
                                        uint tmpOffsetSource = 2;
                                        uint arrayElementSize = j.StringLength * tmpOffsetSource;
                                        uint arrayElementSizeDevice = (uint)(PLCData.BufferLen / j.TagsList[0].TagNode.ArrayDimension) - tmpOffsetSource;

                                        // movicon buffer
                                        byte[] tmpBuffer = new byte[j.TotalJobSize * tmpOffsetSource];

                                        for (int nuberofCyclin = 0; nuberofCyclin < j.TagsList[0].TagNode.ArrayDimension; nuberofCyclin++)
                                        {
                                            // string size (nr of chars)
                                            numChar = (uint)PLCData.Buffer[((arrayElementSizeDevice + tmpOffsetSource) * nuberofCyclin)];
                                            if (numChar > j.StringLength)
                                                numChar = j.StringLength;

                                            // real string size depend on j.S7DataFormat
                                            Array.Copy(PLCData.Buffer, ((arrayElementSizeDevice * nuberofCyclin) + ((nuberofCyclin + 1) * tmpOffsetSource)), tmpBuffer, (arrayElementSize * nuberofCyclin), (numChar * tmpOffsetSource));
                                        }
                                        eJob.Values = tmpBuffer;
                                    }
                                    else
                                    {
                                        eJob.Values = PLCData.Buffer;
                                    }
                            }
                                break;
                            }

                        case S7DataFormats.S5Time:
                            {
                                byte[] tmpBuffer = new byte[j.TagsList[0].Size];

                                if (S7TIAProtocol.ConvertFromS5Time(j.Trans, PLCData.Buffer, ref tmpBuffer))                                     
                                    eJob.Values = tmpBuffer;
                                else
                                    eJob.ErrorCode = (DriverErrorCodes)S7ErrorCodes.ErrorTagDataTypeTooSmal;

                            }
                            break;

                        default:
                            eJob.Values = PLCData.Buffer;
                            break;
                    }
                }
                else
                {
                    eJob.ErrorCode = (DriverErrorCodes)ErrorCode;
                }
                base.ExecuteJob(j);
                eJob.Job = j;
                OnJobExecuted(eJob);
            }
        }

        protected void WriteData(List<S7TIACommJob> list)
        {
            List<AGL4.SymbolicRW> rwsTmp = new List<AGL4.SymbolicRW>();
            List<AGLinkReadWriteElement> Elements = new List<AGLinkReadWriteElement>();

            S7TIAStation s = list[0].Station as S7TIAStation;
            if (s == null)
                return;

            foreach (S7TIACommJob j in list) {

                object objectData = null;
                j.GetJobData(ref objectData);
                if (j.TagsListOnWriting.Count == 0)
                {
                    continue;
                }

                if (j.AccessHandle == S7TIACommJob.AccessHandleState.Undefined) {
                    string szError = string.Empty;
                    string address = GetAddresTypePLC(s, j.StartAddress);
                    if (j.LoadAccessHandleAndSize(plcConnection.HandleFile, s.Name, address, ref szError))
                        j.AccessHandle = S7TIACommJob.AccessHandleState.Found;
                    else
                        j.AccessHandle = S7TIACommJob.AccessHandleState.NotFound;
                }

                if (j.AccessHandle == S7TIACommJob.AccessHandleState.NotFound)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                    eJob.ErrorCode = (DriverErrorCodes)S7ErrorCodes.ErrorFromDllAGLink;
                    base.ExecuteJob(j);
                    eJob.Job = j;
                    OnJobExecuted(eJob);

                    continue;
                }

                #region Prepare data to send to PLC                

                byte[] jobdata = new byte[0];
                int lengthBuff = 0;
                // for all data type except WSTRING
                if (objectData is byte[])
                {
                    jobdata = (byte[])objectData;
                    lengthBuff = jobdata.Length;
                    if (jobdata.Length > j.m_stSymbolicRWrite.BufferLen)
                    {
                        lengthBuff = j.m_stSymbolicRWrite.BufferLen;
                    }
                }

                switch (j.S7DataFormat)
                {
                    case S7DataFormats.S7_DTL:
                        for (int nCyclic = 0; nCyclic < j.TagsList.Count; nCyclic++)
                        {
                            if (nCyclic > 7)
                            {
                                break;
                            }
                            byte[] jobdataTmp = new byte[j.TagsList[nCyclic].Size];
                            j.TagsList[nCyclic].GetTagBuffer(ref jobdataTmp);
                            switch (nCyclic)
                            {
                                case 0:
                                    j.m_stSymbolicRWrite.Buffer[0] = jobdataTmp[1];
                                    j.m_stSymbolicRWrite.Buffer[1] = jobdataTmp[0];
                                    break;
                                case 1:
                                case 2:
                                case 3:
                                case 4:
                                case 5:
                                case 6:
                                    j.m_stSymbolicRWrite.Buffer[nCyclic + 1] = jobdataTmp[0];
                                    break;
                                case 7:
                                    j.m_stSymbolicRWrite.Buffer[8] = jobdataTmp[3];
                                    j.m_stSymbolicRWrite.Buffer[9] = jobdataTmp[2];
                                    j.m_stSymbolicRWrite.Buffer[10] = jobdataTmp[1];
                                    j.m_stSymbolicRWrite.Buffer[11] = jobdataTmp[0];
                                    break;
                                default:
                                    break;

                            }
                        }
                        break;
                    case S7DataFormats.String:
                        {
                            if (j.TagsList[0].TagNode.ArrayDimension == 0)
                            {
                                // reset write buffer before to fill with new data
                                Array.Clear(j.m_stSymbolicRWrite.Buffer, 0, j.m_stSymbolicRWrite.Buffer.Length);

                                uint numChar = 0;
                                while ((numChar < jobdata.Length) && (jobdata[numChar] != 0))
                                    numChar++;

                                if (numChar > j.m_stSymbolicRWrite.BufferLen - 1)
                                    numChar = (uint)(j.m_stSymbolicRWrite.BufferLen - 1);
                                else if (numChar > j.StringLength)
                                    numChar = j.StringLength;

                                j.m_stSymbolicRWrite.Buffer[0] = (byte)numChar;
                                Array.Copy(jobdata, 0, j.m_stSymbolicRWrite.Buffer, 1, numChar);
                            } 
                            else
                            {
                                Array.Clear(j.m_stSymbolicRWrite.Buffer, 0, j.m_stSymbolicRWrite.Buffer.Length);

                                int tmpOffsetSource = 1;
                                int arrayElementSize = (int)(j.StringLength * tmpOffsetSource);
                                int arrayElementSizeDevice = (int)(j.m_stSymbolicRWrite.BufferLen / j.TagsList[0].TagNode.ArrayDimension) - tmpOffsetSource;

                                for (int nuberofCyclin = 0; nuberofCyclin < j.TagsList[0].TagNode.ArrayDimension; nuberofCyclin++)
                                {
                                    uint numChar = 0;
                                    for (int i = 0; i < arrayElementSize; i++) {
                                        if (jobdata[(arrayElementSize * nuberofCyclin) + i] != 0)
                                            numChar++;
                                        else
                                            break;
                                    }
                                    
                                    if (numChar > arrayElementSizeDevice)
                                        numChar = (uint)(arrayElementSizeDevice);
                                    else if (numChar > j.StringLength)
                                        numChar = j.StringLength;

                                    j.m_stSymbolicRWrite.Buffer[((arrayElementSize + tmpOffsetSource) * nuberofCyclin)] = (byte)numChar;

                                    Array.Copy(jobdata, (arrayElementSize * nuberofCyclin), j.m_stSymbolicRWrite.Buffer, ((arrayElementSize * nuberofCyclin) + ((nuberofCyclin + 1) * tmpOffsetSource)), numChar);
                                }
                            }                            
                            break;
                        }

                    case S7DataFormats.WString:
                        {
                            // invalid data from Movicon
                            if (objectData == null)
                            {
                                ExecutedJobArgs eJob = new ExecutedJobArgs();
                                eJob.ErrorCode = (DriverErrorCodes)S7ErrorCodes.ErrorTooFewData;
                                base.ExecuteJob(j);
                                eJob.Job = j;
                                OnJobExecuted(eJob);

                                continue;
                            }
                            else
                            {
                                if (j.TagsList[0].TagNode.ArrayDimension == 0)
                                {
                                    // reset write buffer before to fill with new data
                                    Array.Clear(j.m_stSymbolicRWrite.Buffer, 0, j.m_stSymbolicRWrite.Buffer.Length);

                                    string moviconString = (string)objectData;

                                    uint numChar = 0;
                                    // check that string length don't exceed plc string size
                                    if ((moviconString.Length * 2) > j.m_stSymbolicRWrite.BufferLen - 2)
                                        numChar = (uint)((j.m_stSymbolicRWrite.BufferLen - 2) / 2);
                                    else if (moviconString.Length > j.StringLength)
                                        numChar = j.StringLength;
                                    else
                                        numChar = (uint)moviconString.Length;

                                    // set string length
                                    j.m_stSymbolicRWrite.Buffer[0] = (byte)numChar;
                                    // Unicode, 1 char --> 2 bytes
                                    Array.Copy(System.Text.Encoding.Unicode.GetBytes(moviconString), 0, j.m_stSymbolicRWrite.Buffer, 2, numChar * 2);
                                }
                                else
                                {
                                    Array.Clear(j.m_stSymbolicRWrite.Buffer, 0, j.m_stSymbolicRWrite.Buffer.Length);
                                    
                                    int tmpOffsetSource = 2;
                                    int arrayElementSize = (int)(j.StringLength * tmpOffsetSource);
                                    int arrayElementSizeDevice = (int)(j.m_stSymbolicRWrite.BufferLen / j.TagsList[0].TagNode.ArrayDimension) - tmpOffsetSource;

                                    uint arrayDimension = j.TagsList[0].TagNode.ArrayDimension;

                                    Array moviconStringArr = objectData as Array;
                                    for (int nuberofCyclin = 0; nuberofCyclin < j.TagsList[0].TagNode.ArrayDimension; nuberofCyclin++)
                                    {
                                        string moviconString = moviconStringArr.GetValue(nuberofCyclin).ToString().Trim('\0');

                                        uint numChar = 0;
                                        if ((moviconString.Length * 2) > arrayElementSizeDevice)
                                            numChar = (uint)(arrayElementSizeDevice / 2);
                                        else if (numChar > j.StringLength)
                                            numChar = j.StringLength;
                                        else
                                            numChar = (uint)moviconString.Length;

                                        j.m_stSymbolicRWrite.Buffer[((arrayElementSize + tmpOffsetSource) * nuberofCyclin)] = (byte)numChar;

                                        Array.Copy(System.Text.Encoding.Unicode.GetBytes(moviconString), 0, j.m_stSymbolicRWrite.Buffer, ((arrayElementSize * nuberofCyclin) + ((nuberofCyclin + 1) * tmpOffsetSource)), numChar * 2);
                                    }                                    
                                }
                            }
                            break;
                        }

                    case S7DataFormats.S5Time:
                        if (!S7TIAProtocol.ConvertToS5Time(j.Trans, jobdata, ref j.m_stSymbolicRWrite.Buffer))
                        {
                            ExecutedJobArgs eJob = new ExecutedJobArgs();
                            eJob.ErrorCode = (DriverErrorCodes)S7ErrorCodes.ErrorTagDataTypeTooSmal;
                            base.ExecuteJob(j);
                            eJob.Job = j;
                            OnJobExecuted(eJob);

                            continue;
                        }
                        break;

                    default:
                        Array.Copy(jobdata, 0, j.m_stSymbolicRWrite.Buffer, 0, lengthBuff);
                        //System.Diagnostics.Debug.WriteLine("Value 0 {0:X}{1:X}", j.m_stSymbolicRW.Buffer[0], j.m_stSymbolicRW.Buffer[1]);
                        break;
                }
                #endregion

                rwsTmp.Add(j.m_stSymbolicRWrite);
                // to read from PLC keep rwsTmp[] array index, job and default errorcode (for feature use)
                Elements.Add(new AGLinkReadWriteElement((rwsTmp.Count - 1), j, DriverErrorCodes.ErrorNoError));

                if (Elements.Count >= S7TIAProtocol.MAX_AGGREGATED_JOBS)
                    break;
            }

            // no elements to exchange --> exit 
            if (Elements.Count(a => a.HasRWSSymbolToExchange()) == 0)
                return;

            AGL4.SymbolicRW[] rws = rwsTmp.ToArray();

            int serror = 0;            
            //Write 
            Int32 nRet = AGL4.Symbolic_WriteMixEx(plcConnection.connNr, rws, ref serror, Timeout);
            if (nRet < AGL4.AGL40_SUCCESS)
            {                
                string szAux = String.Format("{0} {1}", s.Name, String.Format(Properties.Resources.ErrorFromDllAGLink, nRet));
                CommDriver.OnSystemEvent(ObjectIds.Server, szAux, Opc.Ua.EventSeverity.High);
                if ((nRet == AGL4.AGL40_TIMEOUT) ||
                    (nRet == AGL4.AGL40_CONNECTION_CLOSED) ||
                    (nRet == AGL4.AGL40_NOT_CONNECTED))
                {
                    LastErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorTimeOut;
                }
                else
                {
                    LastErrorCode = (DriverErrorCodes)S7ErrorCodes.ErrorFromDllAGLink;
                }

                DisConnection();
            }

            foreach (AGLinkReadWriteElement Element in Elements) {
                ExecutedJobArgs eJob = new ExecutedJobArgs();
                S7TIACommJob j = Element.Job;

                AGL4.SymbolicRW PLCData = new AGL4.SymbolicRW();
                if (Element.HasRWSSymbolToExchange())
                    PLCData = rws[Element.SymbolicRWSIndex];

                int ErrorCode = (int)Element.ErrorCode;
                // no initial errore code
                if (ErrorCode == (int)DriverErrorCodes.ErrorNoError)
                {
                    // general error code (connection broken, ecc)
                    if (nRet < AGL4.AGL40_SUCCESS)
                    {
                        ErrorCode = (int)LastErrorCode;
                    }
                    else
                    {
                        // error of single element
                        if (PLCData.SError != 0)
                            ErrorCode = (int)PLCData.SError;
                        else if (PLCData.Result != 0)
                            ErrorCode = (int)PLCData.Result;
                        // generic error
                        else if (!Element.HasRWSSymbolToExchange())
                            ErrorCode = (int)S7ErrorCodes.ErrorFromDllAGLink;
                    }
                }

                base.ExecuteJob(j);
                eJob.ErrorCode = (DriverErrorCodes)ErrorCode; 
                eJob.Job = j;
                OnJobExecuted(eJob);
            }
        }

        //public string GetAddresTypePLC(S7TIAStation station, string originaAddress)
        //{
        //    string Addres;
        //    int Index = 0;
        //    int result = 0;
        //    int error_pos = 0;
        //    IntPtr AccessHandle = IntPtr.Zero;

        //    #region New method of var address (defined into dynamic setting) mapping based on external file generated during import from project
        //    if (plcConnection.ImportFromProjectMappingFileIsPresent)
        //    {
        //        Addres = originaAddress;
        //        switch (plcConnection.AGLinkBinFileImportSource)
        //        {
        //            case S7TIAImportParser.ImportSourceManagement.Plc:
        //                // try to convert from Project address to plc address using map
        //                if (!originaAddress.StartsWith("PLC."))
        //                {
        //                    if (plcConnection.mapTagsPlcAndProject.ContainsKey(originaAddress))
        //                        Addres = plcConnection.mapTagsPlcAndProject[originaAddress];
        //                }
        //                break;
        //            case S7TIAImportParser.ImportSourceManagement.Project:
        //                // try to convert from plc address to project address using map
        //                if (originaAddress.StartsWith("PLC."))
        //                {
        //                    if (plcConnection.mapTagsPlcAndProject.ContainsKey(originaAddress))
        //                        Addres = plcConnection.mapTagsPlcAndProject[originaAddress];
        //                }
        //                break;
        //        }

        //        result = AGL4.Symbolic_CreateAccessByPath(plcConnection.HandleFile, Addres, ref AccessHandle, ref error_pos);
        //        if (result == AGL4.AGL40_SUCCESS)
        //            return Addres;
        //    }
        //    #endregion

        //    #region Old method of var address (defined into dynamic setting) adjustment in case of import from Project
        //    // compatibility of specific client
        //    if (station.ImportSource == S7TIAImportParser.ImportSourceManagement.Project)
        //        return originaAddress;

        //    Addres = originaAddress;
        //    if (originaAddress.StartsWith("PLC."))
        //        return (originaAddress);

        //    // find >PLC_XX>.
        //    Index = Addres.IndexOf('.', 0);
        //    if (Index < 0)
        //        return (originaAddress);
        //    Addres = string.Format("PLC.{0}", Addres.Substring(Index + 1, Addres.Length - Index - 1));

        //    //tag tables (default and not) management
        //    if (Addres.IndexOf("PLC.Tags.", StringComparison.OrdinalIgnoreCase) >= 0)
        //    {
        //        try
        //        {
        //            //int result = 0;
        //            //int error_pos = 0;
        //            //IntPtr AccessHandle = IntPtr.Zero;

        //            Index = 0;
        //            //skip PLC.Tags. part of path
        //            string SubAddress = Addres.Substring("PLC.Tags.".Length, Addres.Length - "PLC.Tags.".Length);

        //            bool FirstTest = true;
        //            do
        //            {
        //                // first test --> try to use last element (".") of path to determine tag name
        //                if (FirstTest)
        //                {
        //                    if (SubAddress.LastIndexOf(".") > 0)
        //                        Index = SubAddress.LastIndexOf(".");
        //                    else
        //                        Index = SubAddress.LastIndexOf("\"");
        //                }
        //                else
        //                {   // try to remove element (element is a piece of path separated by '.' or '"') from path (left to right) until retun ok from AGL4.Symbolic_CreateAccessByPath function
        //                    Index++;
        //                    //check next element separator
        //                    if (SubAddress.IndexOf(".", Index) > 0)
        //                        Index = SubAddress.IndexOf(".", Index);
        //                    else
        //                        Index = SubAddress.IndexOf("\"", Index);
        //                }

        //                if (Index >= 0)
        //                {
        //                    Addres = string.Format("PLC.Tags.Table.{0}", SubAddress.Substring(Index + 1, SubAddress.Length - Index - 1));
        //                    result = AGL4.Symbolic_CreateAccessByPath(plcConnection.HandleFile, Addres, ref AccessHandle, ref error_pos);
        //                    // failed to find tag into handles's table directly, from next loop try find different path
        //                    if (FirstTest && result != AGL4.AGL40_SUCCESS)
        //                    {
        //                        FirstTest = false;
        //                        Index = 0;
        //                    }
        //                }
        //            } while (!(result == AGL4.AGL40_SUCCESS || Index < 0));

        //        }
        //        catch (Exception ex)
        //        {
        //        }
        //    }
        //    else // try to manage datablock
        //    {
        //        foreach (string line in plcConnection.listDataBlocksAndTables)
        //        {
        //            string[] words = line.Split(':');
        //            // search block adding "." before and after to get block and not other path's element
        //            Index = originaAddress.IndexOf(string.Format(".{0}.", words[0]));
        //            if (Index >= 0 && words[1] == "Datablock")
        //            {
        //                Addres = string.Format("PLC.Blocks{0}", originaAddress.Substring(Index));
        //                break;
        //            }
        //        }
        //    }
        //    #endregion

        //    return (Addres);
        //}

        public string GetAddresTypePLC(S7TIAStation station, string originaAddress)
        {
            string Address;            
            int result = 0;
            int error_pos = 0;
            IntPtr AccessHandle = IntPtr.Zero;

            #region New method of var address (defined into dynamic setting) mapping based on external file generated during import from project
            if (plcConnection.ImportFromProjectMappingFileIsPresent)
            {
                // test dynamic setting address
                Address = originaAddress;
                result = AGL4.Symbolic_CreateAccessByPath(plcConnection.HandleFile, Address, ref AccessHandle, ref error_pos);
                if (result == AGL4.AGL40_SUCCESS)
                    return Address;

                Address = string.Empty;
                // if dynamic setting address don't exit in .tia file, try to use map to convert
                switch (plcConnection.SymbolFileImportSource)
                {
                    case S7TIAImportParser.ImportSourceManagement.Plc:
                        // try to convert from Project address to plc address using map
                        if (plcConnection.mapTagsPlcAndProject.ContainsKey(originaAddress))
                            Address = plcConnection.mapTagsPlcAndProject[originaAddress];
                        break;
                    case S7TIAImportParser.ImportSourceManagement.Project:
                        // try to convert from plc address to project address using map
                        if (plcConnection.mapTagsPlcAndProject.ContainsKey(originaAddress))
                            Address = plcConnection.mapTagsPlcAndProject[originaAddress];
                        break;
                }
                // try to use a tag 'conversion' found into map
                if (!string.IsNullOrEmpty(Address)) {
                    result = AGL4.Symbolic_CreateAccessByPath(plcConnection.HandleFile, Address, ref AccessHandle, ref error_pos);
                    if (result == AGL4.AGL40_SUCCESS)
                        return Address;
                }
            }
            #endregion

            #region Old method of var address (defined into dynamic setting) adjustment when .tia file come from PLC and address from Project
            // check if defined address is valid
            result = AGL4.Symbolic_CreateAccessByPath(plcConnection.HandleFile, originaAddress, ref AccessHandle, ref error_pos);
            if (result == AGL4.AGL40_SUCCESS)
                return originaAddress;

            // if HMI Access Level was enabled, defined address --> no more parse
            if (station.ImportSource == S7TIAImportParser.ImportSourceManagement.Project)
                return originaAddress;

            if (plcConnection.SymbolFileImportSource == S7TIAImportParser.ImportSourceManagement.Plc)
            {
                // split address by <.> or </"> to get nodes
                var regex = new Regex("(?<=^|.)(\"(?:[^\"]|\"\")*\"|[^.]*)\\S");
                MatchCollection Nodes = regex.Matches(originaAddress);
                // wrong formatted address  default PLC.Blocks.TagName or PLC.Tags.TagName
                if (Nodes.Count < 3)
                    return originaAddress;

                switch (Nodes[1].Value.ToLower())
                {
                    case "tags.":
                        // remove "part/node" of var address to find plc var address
                        for (int NodeIndex = 1; NodeIndex < Nodes.Count - 1; NodeIndex++)
                        {
                            Address = string.Format("PLC.Tags.Table.{0}", originaAddress.Substring(Nodes[NodeIndex].Index + Nodes[NodeIndex].Length, originaAddress.Length - Nodes[NodeIndex].Index - Nodes[NodeIndex].Length));
                            result = AGL4.Symbolic_CreateAccessByPath(plcConnection.HandleFile, Address, ref AccessHandle, ref error_pos);
                            if (result == AGL4.AGL40_SUCCESS)
                                return Address;
                        }
                        break;
                    case "blocks.":
                        foreach (string Block in plcConnection.listDataBlocksAndTables)
                        {
                            if (string.IsNullOrEmpty(Block) || originaAddress.IndexOf(string.Format(".{0}.", Block), StringComparison.InvariantCultureIgnoreCase) > 0)
                            {
                                // find witch "part/node" is the start block; blank node was addess by default to enabled auto identification
                                int StartIndex = 1;
                                if (!string.IsNullOrEmpty(Block)) {
                                    for (int i = StartIndex; i < Nodes.Count - 1; i++) {
                                        if (string.Compare(Nodes[i].Value, string.Format("{0}.", Block), StringComparison.InvariantCultureIgnoreCase) == 0)
                                        {
                                            StartIndex = i;
                                            break;
                                        }
                                    }
                                }

                                // remove "part/node" of var address to find plc var address
                                for (int NodeIndex = StartIndex; NodeIndex < Nodes.Count - 1; NodeIndex++)
                                {
                                    Address = string.Format("PLC.Blocks.{0}", originaAddress.Substring(Nodes[NodeIndex].Index + Nodes[NodeIndex].Length, originaAddress.Length - Nodes[NodeIndex].Index - Nodes[NodeIndex].Length));
                                    result = AGL4.Symbolic_CreateAccessByPath(plcConnection.HandleFile, Address, ref AccessHandle, ref error_pos);
                                    if (result == AGL4.AGL40_SUCCESS)
                                        return Address;
                                }
                            }
                        }
                        break;

                    default:
                        // bad formatted address
                        return originaAddress;
                }
            }
            #endregion

            return originaAddress;
        }

        public bool InitPLCConnect()
        {            
            if (plcConnection.IsInit())
                return true;                

            List<Station> listStation = CommDriver.GetChannelStations(this);
            string stationName;
            if (listStation.Count == 0)
            {
                stationName = "";
                return false;
            }
            else
            {
                stationName = listStation[0].Name;
            }

            if (plcConnection.Init(TcpChannelHostName, (ushort)TcpChannelHostPort, Timeout, CommDriver, stationName,((S7TIAStation)listStation[0]).ImportSource))
            {
                return true;
            }
            else
            {
                return false;
            }            
        }

        public bool InitOffLine(S7TIAStation station)
        {
            if (station == null)
            {
                return false;
            }
                        
            if (plcConnection.Init(TcpChannelHostName, (ushort)TcpChannelHostPort, Timeout, CommDriver, station.Name, station.ImportSource))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DisConnection()
        {
            if (!plcConnection.IsInit())
            { 
                return true;
            }
            List<Station> listStation = CommDriver.GetChannelStations(this);

            string stationName;
            if (listStation.Count == 0)
                stationName = "";
            else
                stationName = listStation[0].Name;

            return plcConnection.Disconnect();
        }
                
        public string GetListNameDBsNameTablesFileNameOnly(string StationName)
        {
            return plcConnection.GetListNameDBsNameTablesFileNameOnly(StationName);
        }

        public bool DeleteListaDataBlocksAndTables(string StationName)
        {
            return plcConnection.DeleteListaDataBlocksAndTables(StationName);
        }

        public string GetImportProjectTagsFileNameOnly(string StationName)
        {
            return plcConnection.GetImportProjectTagsFileNameOnly(StationName);
        }

        public void SaveImportProjectTags(string StationName, List<S7TIAImportProjectTag> tags)
        {
            plcConnection.SaveImportProjectTags(StationName, tags);
        }

        public string GetAglinkFile(string StationName, S7TIAImportParser.ImportSourceManagement importSource)
        {
            return plcConnection.GetAglinkFile(StationName, importSource);
        }

        public string GetAglinkFileBackUp(string StationName, out bool TargetIsFile)
        {
            return plcConnection.GetAglinkFileBackUp(StationName, out TargetIsFile);
        }

        public bool SaveListDataBlockAndTableBase(string stationName, IntPtr schemaNodeHandle) { 
            return plcConnection.SaveListDataBlockAndTable(stationName, 0, schemaNodeHandle, true);
        }

        public bool SaveSymbolicFile(string stationName, S7TIAImportParser.ImportSourceManagement importSource, IntPtr schemaNodeHandle)
        {
            return plcConnection.SaveSymbolicFile(stationName, importSource,schemaNodeHandle);
        }

        public bool CopyFileSymbolicFile(string stationName, string sourceconn, string targetconn)
        {
            return plcConnection.CopySymbolicFile(stationName, stationName, targetconn);
        }

        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void Dispose()
        {
            plcConnection.Dispose();

            base.Dispose();
        }
        #region Properties
        /// <summary>   Host name. </summary>
        private string _TcpChannelHostName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the name of the TCP channel host. </summary>
        ///
        /// <value> The name of the TCP channel host. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string TcpChannelHostName
        {
            get { return _TcpChannelHostName; }
            set
            {
                _TcpChannelHostName = value;
            }
        }

        /// <summary>   Host port. </summary>
        private int _TcpChannelHostPort;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the TCP channel host port. </summary>
        ///
        /// <value> The TCP channel host port. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int TcpChannelHostPort
        {
            get
            {
                return _TcpChannelHostPort;
            }
            set
            {
                _TcpChannelHostPort = value;
            }
        }        
        #endregion
    }
}
