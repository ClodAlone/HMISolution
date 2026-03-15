using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using Opc.Ua;
using SimotionImportParser;
using DevExpress.Xpo.DB;
using System.IO;
using Accon.AGLink;

namespace Simotion
{
    public class SimotionStation : Station
    {
        #region Constructors

        /// <summary>
        /// Initializes the station object.
        /// </summary>
        public SimotionStation(CommunicationDriver commdriver, SimotionStationSettings settings)
            : base(commdriver, settings)
        {
            _ImportSource = SimImportParser.ImportSourceManagement.Project;
            _SymbolicFile = settings.SymbolicFile;
            
            internalSymbolicFile = GetSymbolFileSource();
            // identify symbolic file (and data source PLC/Project); try also to convert from old driver version                            
            if (SymbolFileImportSource == SimImportParser.ImportSourceManagement.Project)
            {
                // Finally try to load symbolic file
                if (!LoadFileSymbol(internalSymbolicFile))
                    SymbolFileImportSource = SimImportParser.ImportSourceManagement.UnAvailableOrInvalid;
            }
        }

        #endregion

        #region Data Member
        // define .tia file import source
        public SimImportParser.ImportSourceManagement SymbolFileImportSource = SimImportParser.ImportSourceManagement.Project;
        private IntPtr pHandleFile = IntPtr.Zero;
        private bool bErrorLoadSymbolFile = false;
        private string internalSymbolicFile;
        #endregion

        #region Abstract Methods

        public override CommJob CreateJob(CommJobSettings JobSettings)
        {
            var conf = JobSettings as SimotionCommJobSettings;
            if (conf == null)
                throw new ArgumentException("Invalid communication job settings");

            return new SimotionCommJob(this, conf);
        }

        public override CommJob CreateJob(Tag defTag)
        {
            var conf = defTag as SimotionTag;
            if (conf == null)
                throw new ArgumentException("Invalid tag object");

            return new SimotionCommJob(this, conf);
        }

        public override Tag CreateTag(DriverBaseInterfaces.TagDefinition td)
        {
            return new SimotionTag(td);
        }

        public override CommJobSettings CreateJobSettings(Session session, CommJob job)
        {
            var commJob = job as SimotionCommJob;
            if (commJob == null)
                throw new ArgumentException("Invalid job object");

            return new SimotionCommJobSettings(session, commJob);
        }

        #endregion

        #region Override Methods

        public override void ProcessJobValues(ExecutedJobArgs e)
        {
            SimotionCommJob mJ = e.Job as SimotionCommJob;
            if (mJ == null)
                return;

            // analyzing answer if no error exists before
            if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
            {
                byte[] Answer = (byte[])e.Values;
                List<object> ChangedTags = new List<object>();
                if (Answer != null)
                {
                    if (/*P*/SimotionProtocol.ParseData(Answer, ref mJ, ref ChangedTags))
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
            e.GeneralError = (e.ErrorCode == DriverErrorCodes.ErrorTimeOut || (e.ErrorCode == (DriverErrorCodes)S7ErrorCodes.ErrorConnectionToDevice));

            base.ProcessJobValues(e);
        }

        public override bool ParseReceivedToArguments(byte[] receivedbuffer, CommJob job, ref List<Object> arguments)
        {
            SimotionCommJob mj = job as SimotionCommJob;

            SimotionProtocol.ParseData(receivedbuffer, ref mj, ref arguments);
            return true;
        }

        public override List<Tag> SortTags(IList<Tag> tags)
        {
            return tags.ToList();
        }

        public override bool Startup()
        {
            System.Diagnostics.Debug.WriteLine("{0} -- DEBUG -- Station.Startup station name: {1}", DateTime.UtcNow.ToString("HH:MM:ss.fff"), Name);
            bool suspendBitNewValue = false;
            if (GetStateCommandVariableBit(ref suspendBitNewValue, (UInt16)StationVariableBits.StationActiveCommand) == true)
            {
                if (suspendBitNewValue == true)
                {
                    return (true);
                }
            }

            var listJob = new List<CommJob>();
            lock (lockListObject)
            {
                listJob.AddRange(ListWholeJob);
            }

            // if .sti don't file exist stop station/channel
            if (SymbolFileImportSource == SimImportParser.ImportSourceManagement.UnAvailableOrInvalid)
            {
                CommDriver.OnSystemEvent(ObjectIds.Server, String.Format(Properties.Resources.ErrorStationCannotStartedSymbolicFileNotDefined, this.Name), EventSeverity.Max);
                foreach (var job in listJob)
                {
                    job.SetQuality(StatusCodes.BadResourceUnavailable);
                    job.IsPending = false;
                }
                SetStateCommandVariableBit(true, (UInt16)ChannelVariableBits.ChannelUnconnected);
                return false;
            }

            foreach (var job in listJob)
            {
                if (job.Type == LinkType.ExceptionOutput || job.Type == LinkType.UnconditionalOutput)
                    job.SetQuality(StatusCodes.Uncertain);
                else
                    job.SetInternalQuality(StatusCodes.BadWaitingForInitialData);
                job.IsPending = false;
            }

            if (ChannelBase != null)
            {
                ChannelBase.JobExecuted += OnJobExecuted;
                foreach (var job in listJob)
                {

                    List<Tag> tl = (from t in job.TagsList.AsParallel()
                                    where (t.DynSettings.MethodID != -1)
                                    select t).ToList();
                    if (tl.Count != job.TagsList.Count)
                        ChannelBase.SubscribeJob(job, (job.InUse ? CommJobState.PollingInUse : CommJobState.PollingNotInUse));
                }
            }

            return true;
        }
        #endregion

        #region Custom Methods

        /// <summary>
        /// Remove station's Tia File created from db (next start-up driver will create a new one with a file name with random part)
        /// </summary>
        private void DeleteTempSymbolicFile()
        {            
            try
            {
                if (!string.IsNullOrEmpty(internalSymbolicFile))
                {
                    if (File.Exists(internalSymbolicFile))
                        File.Delete(internalSymbolicFile);                    
                }
            }
            catch (Exception ex) { }
        }

        private IDataLayer GetAglinkFile(string conn, SimImportParser.ImportSourceManagement importSource, out string sourceSymbolicFile, out string targetSymbolicFile, out InMemoryDataStore InMemory, out bool targetIsFile)
        {
            string connect = CommunicationDriver.GetConnectionString(conn, "Drivers", CommDriver.DriverName, null);
            IDataLayer idl = CommunicationDriver.GetSpecificDataLayer(connect, out targetSymbolicFile, out InMemory, out targetIsFile);
            
            sourceSymbolicFile = this.SymbolicFile;
            
            if (idl != null)
            {
                #region get/set Symbolic file (with full path)

                // Movicon project from file ?
                if (targetIsFile)
                {                                            
                    // old driver version don't has this parameter --> try to use previous naming convention
                    if (string.IsNullOrEmpty(sourceSymbolicFile))
                    {
                        sourceSymbolicFile = Path.Combine(SimotionProtocol.GetDriverPath(conn), SimotionProtocol.GetOldFileName(this.Name));
                        if (!SimotionUISymbolicFileManagement.FileExist(sourceSymbolicFile))
                            sourceSymbolicFile = string.Empty;
                    }
                    else
                    {
                        if (SimotionProtocol.IsImportedSymbolicFile(sourceSymbolicFile))
                            sourceSymbolicFile = Path.Combine(SimotionProtocol.GetDriverPath(conn), sourceSymbolicFile);
                    }

                    targetSymbolicFile = GetTempSymbolicFileName(importSource);
                    
                }
                else // from db
                {
                    if (string.IsNullOrEmpty(sourceSymbolicFile))
                        sourceSymbolicFile = SimotionProtocol.GetOldFileName(this.Name);

                     targetSymbolicFile = GetTempSymbolicFileName(importSource);
                }
                #endregion
            }

            return idl;
        }        

        private bool TryToGetExternalSymbolFile(IDataLayer idl, string sourceSymbolicFile, ref string targetSymbolicFile)
        {
            bool result = false;
            
            try
            {
                File.Copy(sourceSymbolicFile, targetSymbolicFile);
                result = true;
            }
            catch (Exception ex)  {  }
            
            return result;
        }

        // try to create (on local PC in user\temp directory) .tia file from database (in case of project from db)
        private bool TryToGetSymbolFileFromDatabase(IDataLayer idl, string sourceSymbolicFile, ref string targetSymbolicFile)
        {
            bool result = false;

            if (SimotionProtocol.IsImportedSymbolicFile(sourceSymbolicFile))
            {
                // from db                
                using (UnitOfWork ufw = new UnitOfWork(idl))
                {
                    // get tia file "body" by station
                    SimotionStationSettings station = (from s in new XPQuery<SimotionStationSettings>(ufw).AsParallel() where s.Name == this.Name select s).ToList().FirstOrDefault();
                    if (station != null)
                    {
                        // symbolicFile is temporary target file name
                        try
                        {
                            // get source type by extension stored into db                        
                            using (BinaryWriter binWriter = new BinaryWriter(File.Open(targetSymbolicFile, FileMode.Create)))
                            {
                                binWriter.Write(station.FileBody);
                            }
                            if (File.Exists(targetSymbolicFile))
                            {
                                FileInfo fileinfo = new FileInfo(targetSymbolicFile);
                                result = (fileinfo.Length != 0);
                            }
                        }
                        catch (Exception ex) { }
                    }
                }
            }
            else
            {
                result = TryToGetExternalSymbolFile(idl, sourceSymbolicFile, ref targetSymbolicFile);
            }
            
            return result;
        }
        
        public string GetSymbolFileSource()
        {
            bool result;
            string targetSymbolicFile = String.Empty;

            using (IDataLayer idl = GetAglinkFile(CommDriver.StrConnectionString, SymbolFileImportSource, out string sourceSymbolicFile, out targetSymbolicFile, out InMemoryDataStore inMemory, out bool targetIsFile))
            {
                if (targetIsFile)
                    result = TryToGetExternalSymbolFile(idl, sourceSymbolicFile, ref targetSymbolicFile);
                else
                    // try to create .tia file from database (in case of Movicon project from db)
                    result = TryToGetSymbolFileFromDatabase(idl, sourceSymbolicFile, ref targetSymbolicFile);
            }

            if (result)
            {
                SymbolFileImportSource = SimImportParser.ImportSourceManagement.Project;
            }
            else
            {
                SymbolFileImportSource = SimImportParser.ImportSourceManagement.UnAvailableOrInvalid;
                targetSymbolicFile = string.Empty;
            }
            return targetSymbolicFile;
        }

        private string GetAglinkFileSuffix(SimImportParser.ImportSourceManagement importSource)
        {
            //string FileSuffix = string.Empty;

            //switch (importSource)
            //{
            //    //case SimImportParser.ImportSourceManagement.Plc:
            //    //case SimImportParser.ImportSourceManagement.ProjectAndPlc:
            //    //    FileSuffix = "_Plc";
            //    //    break;
            //    //case SimImportParser.ImportSourceManagement.Project:
            //    //    FileSuffix = "_Prj";
            //    //    break;
            //    default:
            //        FileSuffix = string.Empty;
            //        break;
            //}

            //return FileSuffix;
            return string.Empty;
        }

        private SimImportParser.ImportSourceManagement GetAglinkFileTypeBySuffix(string suffix)
        {
            //switch (suffix)
            //{
            //    case "_Plc":
            //        return SimImportParser.ImportSourceManagement.Plc;
            //    case "_Prj":
            //        return SimImportParser.ImportSourceManagement.Project;
            //    default:
            //        return SimImportParser.ImportSourceManagement.None;
            //}
            return SimImportParser.ImportSourceManagement.Project;
        }



        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Load File Symbol. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool LoadFileSymbol(string projectFile)
        {
            int ret = AGL4.AGL40_SUCCESS;            
            if (projectFile == string.Empty)
                return false;

            if (!File.Exists(projectFile))
            {
                if (!bErrorLoadSymbolFile)
                {
                    string szAux = String.Format("{0} {1}", this.Name, String.Format(Properties.Resources.ErrorSymbolFileNotFound, projectFile));
                    CommDriver.OnSystemEvent(ObjectIds.Server, szAux, Opc.Ua.EventSeverity.High);
                    bErrorLoadSymbolFile = true;
                }

                return false;
            }

            IntPtr rootSchemaNodeHandle = HandleFile;

            // Before accessing a new project, make sure to free a previously used handle.
            if (rootSchemaNodeHandle != IntPtr.Zero)
            {
                ret = AGL4.Simotion_FreeHandle(rootSchemaNodeHandle);
                rootSchemaNodeHandle = IntPtr.Zero;

                if (ret != AGL4.AGL40_SUCCESS)
                {
                    if (!bErrorLoadSymbolFile)
                    {
                        string szAux = String.Format("{0} {1}", this.Name, String.Format(Properties.Resources.ErrorFromDllAGLink, ret));
                        CommDriver.OnSystemEvent(ObjectIds.Server, szAux, Opc.Ua.EventSeverity.High);
                        bErrorLoadSymbolFile = true;
                    }
                    return false;
                }
            }

            if (rootSchemaNodeHandle == IntPtr.Zero)
            {
                ret = AGL4.Simotion_LoadSTISymbols(projectFile, ref rootSchemaNodeHandle);
                if (ret == AGL4.AGL40_SUCCESS)
                {
                    HandleFile = rootSchemaNodeHandle;
                }
                else
                {
                    if (!bErrorLoadSymbolFile)
                    {
                        string szAux = String.Format("{0} {1}", this.Name, String.Format(Properties.Resources.ErrorFromDllAGLink, ret));
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
        private bool FreeFileSymbol()
        {
            int ret = AGL4.AGL40_SUCCESS;
            if (HandleFile != IntPtr.Zero)
            {
                IntPtr rootSchemaNodeHandle = HandleFile;
                ret = AGL4.Simotion_FreeHandle(rootSchemaNodeHandle);
                if (ret == AGL4.AGL40_SUCCESS)
                {
                    pHandleFile = IntPtr.Zero;                    
                }
                else
                {
                    string szAux = String.Format("{0} {1}", this.Name, String.Format(Properties.Resources.ErrorFromDllAGLink, ret));
                    CommDriver.OnSystemEvent(ObjectIds.Server, szAux, Opc.Ua.EventSeverity.High);
                }
                //nErrorLibraryCode = ret;
            }

            return ret == AGL4.AGL40_SUCCESS;
        }

        private string GetTempSymbolicFileName(SimImportParser.ImportSourceManagement importSource)
        {
            string file = string.Format("{0}_{1}_{2}{3}{4}", this.Name, CommDriver.DriverName, GetAglinkFileSuffix(SymbolFileImportSource), Guid.NewGuid(), SimotionProtocol.SYMBOLIC_FILE_EXT);
            file = Path.Combine(Path.GetTempPath(), file);
            return file;
        }

        //public bool CopySymbolicFile(string sourceconn, string targetconn, UnitOfWork ufw)
        //{
        //    bool result = false;

        //    if (SymbolFileImportSource != SimImportParser.ImportSourceManagement.UnAvailableOrInvalid)
        //        result = CopySymbolicFileBody(SymbolFileImportSource, targetconn, ufw, internalSymbolicFile);

        //    return result;
        //}


        #endregion

        #region Properties
        SimImportParser.ImportSourceManagement _ImportSource = SimImportParser.ImportSourceManagement.ProjectAndPlc;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the .Tia file import/creation mode. </summary>
        ///
        /// <value> .Tia file management mode </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public SimImportParser.ImportSourceManagement ImportSource
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

        /// <summary>
        /// Region Properties
        /// </summary>
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

        string _SymbolicFile;
        public string SymbolicFile
        {
            get { return _SymbolicFile; }
        }
        #endregion

        #region Dispose
        public override void Dispose()
        {
            FreeFileSymbol();

            DeleteTempSymbolicFile();            

            base.Dispose();
        }
        #endregion
    }
}
