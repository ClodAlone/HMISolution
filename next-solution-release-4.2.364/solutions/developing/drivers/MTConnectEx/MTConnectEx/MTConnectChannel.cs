using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Opc.Ua;
using System.Net;
using System.IO;
using System.Xml.Linq;
using System.Text;


namespace MTConnect
{
    public class MTConnectChannel : ChannelList
    {
        private bool? MoviconProjectSourceIsFile = null;
        #region Constructors
        /// <summary>
        /// Initializes the MTConnectChannel object.
        /// </summary>
        public MTConnectChannel(CommunicationDriver commdriver, MTConnectChannelSettings settings)
            : base(commdriver, settings)
        {
            _ServerAddress = settings.ServerAddress;
            _ServerPort = settings.ServerPort;

            MTConnectCommDriver = (MTConnectDriver)CommDriver;          
        }
        #endregion

        #region Abstracts Methods
        public override bool IsDeviceOpen()
        {
            return (true);
        }

        public override bool DeviceOpen()
        {
            LastErrorCode = DriverErrorCodes.ErrorNoError;
            SetStateCommandVariableBit(false, (UInt16)ChannelVariableBits.ChannelUnconnected);
            return true;
        }

        public override bool DeviceClose()
        {
            LastErrorCode = DriverErrorCodes.ErrorNoError;
            SetStateCommandVariableBit(true, (UInt16)ChannelVariableBits.ChannelUnconnected);
            return true;
        }

        public override bool DeviceRead(byte[] Buffer, uint Count) { return true; }
        public override bool DeviceWrite(byte[] Buffer, uint Count) { return true; }
        public override uint GetBytesToRead() { return 1; }
        public override uint GetBytesToWrite() { return 1; }

        #endregion

        #region Data Members               
        public MTConnectDriver MTConnectCommDriver = null;

        #endregion

        #region Override Methods
        public override void SplitInExecutionLists(List<CommJob> jobList, ref List<List<CommJob>> exList)
        {          
            int jobIndex = 0;
            while (jobIndex < jobList.Count)
            {
                bool write = false;
                bool first = true;
                string station = string.Empty;
                //append a new list to the end of exList
                List<CommJob> list = new List<CommJob>();

                while (jobIndex < jobList.Count)
                {
                    MTConnectCommJob j = jobList.ElementAt(jobIndex) as MTConnectCommJob;
                    if (j != null && j.TagsListOnWriting.Count == 0)
                    {
                        if (first)
                        {
                            write = (j.TagsListToWrite.Count > 0 && (j.Type == LinkType.ExceptionOutput || j.Type == LinkType.InputOutput)) || j.Type == LinkType.UnconditionalOutput;//j.TagsListToWrite.Count > 0 || j.Type == LinkType.UnconditionalOutput;
                            station = j.Station.Name;
                            first = false;
                        }
                        bool jwrite = (j.TagsListToWrite.Count > 0 && (j.Type == LinkType.ExceptionOutput || j.Type == LinkType.InputOutput)) || j.Type == LinkType.UnconditionalOutput;
                        if (write)
                        {
                            if (j.Station != null && j.Station.Name == station && jwrite)
                            {
                                if (list.Count >= ((MTConnectStation)j.Station).MaxNumberOfItemsForRequest)
                                    break;

                                list.Add(j);
                                jobList.RemoveAt(jobIndex);
                                jobIndex--;
                            }
                        }
                        else
                        {
                            if (j.Station != null && j.Station.Name == station && !jwrite && j.Type != LinkType.ExceptionOutput)
                            {
                                if (list.Count >= ((MTConnectStation)j.Station).MaxNumberOfItemsForRequest)
                                    break;

                                list.Add(j);
                                jobList.RemoveAt(jobIndex);
                                jobIndex--;
                            }
                        }
                    }
                    jobIndex++;
                }

                if (list.Count > 0)
                {
                    exList.Add(list);
                    jobIndex = 0;
                }
            }
        }

        public override bool ExecuteJobList(ref DriverErrorCodes conn, List<CommJob> list)
        {
            //DateTime dt = MTConnectProtocol.GetDateTimeUtcNowNoMSec();
            //DateTime currentExecutionTime = ((MTConnectCommJob)list[0]).GetCurrentScheduleTime(dt);
                
            if (conn == DriverErrorCodes.ErrorNoError)
            {
                string msg = PrepareReadRequest(list);
                if (String.IsNullOrEmpty(msg))
                {
                    conn = DriverErrorCodes.ErrorTimeOut;
                }
                else
                {

                    try
                    {
                        string current = String.Format("http://{0}:{1}/{2}", ServerAddress, ServerPort, msg);
                        HttpWebRequest curentRequest = (HttpWebRequest)WebRequest.Create(current);
                        curentRequest.KeepAlive = false;
                        //set timeout value for the request
                        curentRequest.Timeout = Timeout;

                        WebResponse curentResponse = curentRequest.GetResponse();
                        if (((HttpWebResponse)curentResponse).StatusCode == HttpStatusCode.OK)
                        {
                            StreamReader currentReader = new StreamReader(curentResponse.GetResponseStream());
                            string doc = currentReader.ReadToEnd();

                            XElement currentDoc = XElement.Parse(doc);
                            ParseRequestResult(list, currentDoc);
                        }
                        else
                        {
                            conn = (DriverErrorCodes)MTConnectProtocol.MTConnectErrorCodes.ErrorCodeNoInternetConnection;
                            if (LastErrorCode != conn)
                            {
                                LastErrorCode = conn;
                                string errorMessage = string.Format(Properties.Resources.MTConnectErrorInternetNotAvailable.ToString(), ((HttpWebResponse)curentResponse).StatusCode, ((HttpWebResponse)curentResponse).StatusDescription);
                                CommDriver.OnSystemEvent(ObjectIds.Server, errorMessage, Opc.Ua.EventSeverity.High);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        conn = (DriverErrorCodes)MTConnectProtocol.MTConnectErrorCodes.ErrorCodeNoInternetConnection;
                        if (LastErrorCode != conn)
                        {
                            LastErrorCode = conn;
                            string errorMessage = string.Format(Properties.Resources.MTConnectErrorEthernetCardNotAvailable.ToString(), ex.Message.ToString());
                            CommDriver.OnSystemEvent(ObjectIds.Server, errorMessage, Opc.Ua.EventSeverity.High);
                        }
                    }
                }
            }            

            foreach (MTConnectCommJob j in list)
            {

                ExecutedJobArgs eJob = null;
                if (conn == DriverErrorCodes.ErrorNoError)
                {
                    DateTime timestam ;
                    GetElementRequest(j, out timestam );
                    eJob = new ExecutedJobArgs { ErrorCode = j.RequestResultError, Job = j, Values = j.RequestResultValue , Timestamp= timestam };
                }
                else
                {                    
                    eJob = new ExecutedJobArgs { ErrorCode = conn, Job = j, Values = null };
                }                
                OnJobExecuted(eJob);
            }
            
            return false;
        }

        public override bool IsScheduledJobsListFull(List<CommJob> jobList)
        {
            return (jobList.Count == ((MTConnectStation)jobList[0].Station).MaxNumberOfItemsForRequest);
        }
        #endregion

        #region Local Methods
        /// <summary>
        ///  Prepare Read Request
        /// </summary>
        /// <param name="list"></param>
        /// <returns> Return the message to send </returns>
        private string PrepareReadRequest(List<CommJob> list)
        {
            string msg = String.Empty;
            string singleElemet = String.Empty;
            foreach (CommJob job in list)
            {
                singleElemet = String.Format("{0}[@id=\"{1}\"]", ((MTConnectCommJob)job).PathParameter, ((MTConnectCommJob)job).TagName);
                if (String.IsNullOrEmpty(msg))
                {
                    msg = singleElemet;
                }
                else
                {
                    msg = String.Format("{0}|{1}", msg, singleElemet);
                }
            }
            msg = String.Format("{0}/current?path={1}", ((MTConnectStation)list[0].Station).DeviceId,msg);
            return (msg);
        }
        /// <summary>
        /// Parse Request Result
        /// </summary>
        /// <param name="list"></param>
        /// <param name="currentDoc"></param>
        /// <returns></returns>
        private bool ParseRequestResult(List<CommJob> list, XElement currentDoc)
        {
            if (ParameterRequestList == null)
            {
                ParameterRequestList = new List<ElementParamete>();
            }
            ParameterRequestList.Clear();
            foreach (var urlElement in currentDoc.Elements())
            {
                ParseElemet(urlElement);
            }
            return (true);
        }
        /// <summary>
        /// Parse Elemet Request Result
        /// </summary>
        /// <param name="child"></param>
        private void ParseElemet(XElement child)
        {
            if (child.Elements().Count() == 0)
            {                
                if (child.Attributes().Count() != 0)
                {
                    if (child.Attributes().Count() > 0)
                    {
                        XName name = XName.Get("dataItemId");
                        XName timestamp = XName.Get("timestamp");
                        XName error = XName.Get("errorCode");
                        XAttribute xAttributeName = null;
                        XAttribute xAttributeTimestamp = null;
                        ElementParamete el = null ;
                        try
                        {
                            xAttributeName = child.Attribute(name);
                            xAttributeTimestamp = child.Attribute(timestamp);

                            if (xAttributeName != null)
                            {
                                el = new ElementParamete(String.Empty, String.Empty, String.Empty, String.Empty, String.Empty);
                                el.Category = child.Name.LocalName;
                                el.Id = xAttributeName.Value;
                                el.Value = child.Value;
                                if(xAttributeTimestamp != null)
                                    el.Timestamp = xAttributeTimestamp.Value;
                                else
                                    el.Timestamp = DateTime.Now.ToString();
                            }
                            else 
                            {
                                xAttributeName = child.Attribute(error);
                                if (xAttributeName != null)
                                {
                                    el = new ElementParamete(String.Empty, String.Empty, String.Empty, String.Empty, String.Empty);
                                    el.Id = GetIdErrorString(child.Value);
                                    el.Error = xAttributeName.Parent.Value;
                                }
                            }
                            if(el != null)
                                ParameterRequestList.Add(el);
                        }
                        catch (System.Exception ex)
                        {
                            //Continue
                        }
                    }
                }
            }
            else
            {
                foreach (var urlElement in child.Elements())
                {
                    ParseElemet(urlElement);
                }
            }
        }
        /// <summary>
        /// Get the value Element Request and associat the job
        /// </summary>
        /// <param name="j"></param>
        /// <param name="timestam"></param>
        public void GetElementRequest(MTConnectCommJob j, out DateTime timestam )
        {
  
            timestam = DateTime.UtcNow;
            bool JobUpdated = false;
            foreach (var el in ParameterRequestList)
            {               
                if (!String.IsNullOrWhiteSpace(el.Id) &&
                  (el.Id.Equals(j.TagName)))                   
                {
                    if (!String.IsNullOrWhiteSpace(el.Value) || (GetMoviconTypeId(el.Category) == MTConnectProtocol.FormatDataType_E.String ))
                    {
                        JobUpdated = true;
                        if (DateTime.TryParse(el.Timestamp, out DateTime enteredDate))
                        {
                            timestam = enteredDate.ToUniversalTime();
                        }
                        else
                        {
                            timestam = DateTime.UtcNow;
                        }
                        j.RequestResultError = DriverErrorCodes.ErrorNoError;
                        switch (GetMoviconTypeId(el.Category))
                        {
                            case MTConnectProtocol.FormatDataType_E.Double:
                                {
                                    if (Double.TryParse(el.Value, out double value))
                                    {
                                        j.RequestResultValue = BitConverter.GetBytes(value);
                                    }
                                    else
                                    {
                                        j.RequestResultError = (DriverErrorCodes)MTConnectProtocol.MTConnectErrorCodes.ErrorCodeConvertValue;
                                        if (!j.InErrorState)
                                        {
                                            string errorMessage = string.Format(Properties.Resources.MTConnectErrorCodeConvertValue.ToString(), "Double", el.Value, j.TagName);
                                            CommDriver.OnSystemEvent(ObjectIds.Server, errorMessage, Opc.Ua.EventSeverity.Medium);
                                        }
                                    }
                                }
                                break;
                            case MTConnectProtocol.FormatDataType_E.DWord:
                                {
                                    if (UInt32.TryParse(el.Value, out UInt32 value))
                                    {
                                        j.RequestResultValue = BitConverter.GetBytes(value);
                                    }
                                    else
                                    {
                                        j.RequestResultError = (DriverErrorCodes)MTConnectProtocol.MTConnectErrorCodes.ErrorCodeConvertValue;
                                        if (!j.InErrorState)
                                        {
                                            string errorMessage = string.Format(Properties.Resources.MTConnectErrorCodeConvertValue.ToString(), "DWord", el.Value, j.TagName);
                                            CommDriver.OnSystemEvent(ObjectIds.Server, errorMessage, Opc.Ua.EventSeverity.Medium);
                                        }
                                    }
                                }
                                break;
                            case MTConnectProtocol.FormatDataType_E.Signed_DWord:
                                {
                                    if (Int32.TryParse(el.Value, out Int32 value))
                                    {
                                        j.RequestResultValue = BitConverter.GetBytes(value);
                                    }
                                    else
                                    {
                                        j.RequestResultError = (DriverErrorCodes)MTConnectProtocol.MTConnectErrorCodes.ErrorCodeConvertValue;                                           
                                        if (!j.InErrorState)
                                        {
                                             string errorMessage = string.Format(Properties.Resources.MTConnectErrorCodeConvertValue.ToString(), "Signed DWord", el.Value, j.TagName);
                                            CommDriver.OnSystemEvent(ObjectIds.Server, errorMessage, Opc.Ua.EventSeverity.Medium);
                                        }
                                    }
                                }
                                break;
                            case MTConnectProtocol.FormatDataType_E.String:
                                {
                                    if (string.IsNullOrWhiteSpace(el.Error))
                                        j.RequestResultValue = Encoding.UTF8.GetBytes(el.Value);
                                    else
                                    {
                                        j.RequestResultError = DriverErrorCodes.ErrorRXOverError;
                                        if (!j.InErrorState)
                                        {
                                            string errorMessage = string.Format(Properties.Resources.MTConnectErrorJobNotFound.ToString(), j.TagName, el.Error);
                                            CommDriver.OnSystemEvent(ObjectIds.Server, errorMessage, Opc.Ua.EventSeverity.Medium);
                                        }
                                    }
                                    //j.RequestResultValue = Encoding.UTF8.GetBytes(el.Value);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        if (!String.IsNullOrWhiteSpace(el.Error))
                        {
                            j.RequestResultValue = null;
                            j.RequestResultError = DriverErrorCodes.ErrorRXOverError;
                            if (!j.InErrorState)
                            {
                                string errorMessage = string.Format(Properties.Resources.MTConnectErrorJobNotFound.ToString(), j.TagName,el.Error);
                                CommDriver.OnSystemEvent(ObjectIds.Server, errorMessage, Opc.Ua.EventSeverity.Medium);
                            }
                        }
                    }
                }
                
            }
            //If the Job was not found within the response, it is placed in error.
            if (!JobUpdated)  
            {
                j.RequestResultValue = null;
                j.RequestResultError = (DriverErrorCodes)MTConnectProtocol.MTConnectErrorCodes.ErrorCodeTagNoPresent;
                if (!j.InErrorState)
                {
                    string errorMessage = string.Format(Properties.Resources.MTConnectErrorNoAccessibleOrNotExistent.ToString(),j.TagName);
                    CommDriver.OnSystemEvent(ObjectIds.Server, errorMessage, Opc.Ua.EventSeverity.Medium);
                }
            }
        }
        MTConnectProtocol.FormatDataType_E GetMoviconTypeId(string Type)
        {
            MTConnectProtocol.FormatDataType_E nType = MTConnectProtocol.FormatDataType_E.String;
            Type.Trim();

            if (Type.Length == 0)
                return (nType);

            Type = Type.ToUpper();

            if (Type.Equals("ACCELERATION"))
            {
                nType = MTConnectProtocol.FormatDataType_E.Double;
            }
            else if (Type.Equals("AMPERAGE"))
            {
                nType = MTConnectProtocol.FormatDataType_E.Double;
            }
            else if (Type.Equals("ANGLE"))
            {
                nType = MTConnectProtocol.FormatDataType_E.Double;
            }
            else if (Type.Equals("ANGULARACCELERATION"))
            {
                nType = MTConnectProtocol.FormatDataType_E.Double;
            }
            else if (Type.Equals("ANGULARVELOCITY"))
            {
                nType = MTConnectProtocol.FormatDataType_E.Double;
            }
            else if (Type.Equals("AXISFEEDRATE"))
            {
                nType = MTConnectProtocol.FormatDataType_E.Double;
            }
            else if (Type.Equals("DISPLACEMENT"))
            {
                nType = MTConnectProtocol.FormatDataType_E.Double;
            }
            else if (Type.Equals("FREQUENCY"))
            {
                nType = MTConnectProtocol.FormatDataType_E.Double;
            }
            else if (Type.Equals("LOAD"))
            {
                nType = MTConnectProtocol.FormatDataType_E.Double;
            }
            else if (Type.Equals("PATHFEEDRATE"))
            {
                nType = MTConnectProtocol.FormatDataType_E.Double;
            }
            else if (Type.Equals("POSITION"))
            {
                nType = MTConnectProtocol.FormatDataType_E.Double;
            }
            else if (Type.Equals("PRESSURE"))
            {
                nType = MTConnectProtocol.FormatDataType_E.Double;
            }
            else if (Type.Equals("SPINDLESPEED"))
            {
                nType = MTConnectProtocol.FormatDataType_E.Double;
            }
            else if (Type.Equals("TEMPERATURE"))
            {
                nType = MTConnectProtocol.FormatDataType_E.Double;
            }
            else if (Type.Equals("TORQUE"))
            {
                nType = MTConnectProtocol.FormatDataType_E.Double;
            }
            else if (Type.Equals("VELOCITY"))
            {
                nType = MTConnectProtocol.FormatDataType_E.Double;
            }
            else if (Type.Equals("VOLTAGE"))
            {
                nType = MTConnectProtocol.FormatDataType_E.Double;
            }
            else if (Type.Equals("WATTAGE"))
            {
                nType = MTConnectProtocol.FormatDataType_E.Double;
            }
            else if (Type.Equals("LINE"))
            {
                nType = MTConnectProtocol.FormatDataType_E.DWord;
            }
            else if (Type.Equals("PARTCOUNT"))
            {
                nType = MTConnectProtocol.FormatDataType_E.Signed_DWord;
            }

            return (nType);

        }

        /// <summary>
        /// GetIdErrorString
        /// </summary>
        /// <param name="errorString"></param>
        /// <returns> Returns the tag name that is in error </returns>
        private string GetIdErrorString(string errorString)
        {
            //Exsample error string:
            //The path could not be parsed. Invalid syntax: 
            //Devices/Device[@name="MFMS10-MC1"]//Components/Axes/Components/Linear/DataItems/DataItem[@id="xpme"]
            int lastSquareBracket = errorString.LastIndexOf('[') + (("@id=\"").Length) +1;
            return errorString.Substring(lastSquareBracket, (errorString.Length - lastSquareBracket - (("\"]").Length)));
        }

        #region Imported method
        /// <summary>
        /// Method that requires all the variables of the machine
        /// </summary>
        /// <param name="ServerAddress"></param>
        /// <param name="ServerPort"></param>
        /// <returns>Return streams in xml format</returns>
        public XElement GetCommandProbe(string ServerAddress, uint ServerPort)
        {
            XElement currentDoc = null;
            DriverErrorCodes conn = DriverErrorCodes.ErrorNoError;
            string errorMessage = String.Empty;
            try
            {
                string current = String.Format("http://{0}:{1}/{2}", ServerAddress, ServerPort, "probe");
                HttpWebRequest curentRequest = (HttpWebRequest)WebRequest.Create(current);
                curentRequest.KeepAlive = false;
                //set timeout value for the request
                curentRequest.Timeout = Timeout;
            
                WebResponse curentResponse = curentRequest.GetResponse();
                if (((HttpWebResponse)curentResponse).StatusCode == HttpStatusCode.OK)
                {
                    StreamReader currentReader = new StreamReader(curentResponse.GetResponseStream());
                    string doc = currentReader.ReadToEnd();

                    currentDoc = XElement.Parse(doc);
                }
                else
                {
                    
                    conn = (DriverErrorCodes)MTConnectProtocol.MTConnectErrorCodes.ErrorCodeNoInternetConnection;
                    errorMessage = string.Format(Properties.Resources.MTConnectErrorInternetNotAvailable.ToString(), ((HttpWebResponse)curentResponse).StatusCode, ((HttpWebResponse)curentResponse).StatusDescription);
                    CommDriver.OnSystemEvent(ObjectIds.Server, errorMessage, Opc.Ua.EventSeverity.High);
                }
            }
            catch (Exception ex)
            {
                conn = (DriverErrorCodes)MTConnectProtocol.MTConnectErrorCodes.ErrorCodeNoInternetConnection;
                errorMessage = string.Format(Properties.Resources.MTConnectErrorEthernetCardNotAvailable.ToString(), ex.Message.ToString());
                CommDriver.OnSystemEvent(ObjectIds.Server, errorMessage, Opc.Ua.EventSeverity.High);
            }
            return (currentDoc);
        }

        public XElement GetCommandProbeForTest(string ServerAddress, uint ServerPort, out string error )
        {
            XElement currentDoc = null;
            DriverErrorCodes conn = DriverErrorCodes.ErrorNoError;
            string errorMessage = String.Empty;
            string current = String.Format("http://{0}:{1}/{2}", ServerAddress, ServerPort, "probe");
            HttpWebRequest curentRequest = (HttpWebRequest)WebRequest.Create(current);
            curentRequest.KeepAlive = false;
            error = null;
            //set timeout value for the request
            curentRequest.Timeout = Timeout;
            try
            {
                WebResponse curentResponse = curentRequest.GetResponse();
                if (((HttpWebResponse)curentResponse).StatusCode == HttpStatusCode.OK)
                {
                    StreamReader currentReader = new StreamReader(curentResponse.GetResponseStream());
                    string doc = currentReader.ReadToEnd();

                    currentDoc = XElement.Parse(doc);
                }
                else
                {
                    error = string.Format(Properties.Resources.MTConnectErrorInternetNotAvailable.ToString(), ((HttpWebResponse)curentResponse).StatusCode, ((HttpWebResponse)curentResponse).StatusDescription);
                }
            }
            catch (Exception ex)
            {
                error = string.Format(Properties.Resources.MTConnectErrorEthernetCardNotAvailable.ToString(), ex.Message.ToString());
            }
            return (currentDoc);
        }

        #endregion

        #region Area save file
        public bool SaveSymbolicFile(string stationName, XElement importSource, string variableImportFilePath )
        {
            //if (String.IsNullOrWhiteSpace(variableImportFilePath))
            //    return (false);

            try
            {
                #region try so save into database .tia file content                
                InMemoryDataStore InMemorySubscriberID = null;
                using (IDataLayer idl = GetAglinkFile(stationName, importSource, out string symbolicFile, out InMemorySubscriberID, out bool targetIsFile))
                {
                    if (idl == null)
                        return false;

                    if (targetIsFile)
                    {
                        if (File.Exists(symbolicFile))
                            File.Delete(symbolicFile);

                        importSource.Save(symbolicFile);
                    }
                    //else // to db
                    //{
                    //    using (UnitOfWork ufw = new UnitOfWork(idl))
                    //    {
                    //        // get .tia file associated to station (only 1)
                    //        List<SimotionFile> Files = (from S in new XPQuery<SimotionFile>(ufw).AsParallel() where S.StationName == stationName select S).ToList();
                    //        if (Files.Count > 0)
                    //        {
                    //            // before to import delete previous data
                    //            ufw.Delete(Files);
                    //            ufw.CommitChanges();
                    //        }

                    //        SimotionFile SiFile = new SimotionFile(ufw);
                    //        SiFile.StationName = stationName;

                    //        SiFile.Source = GetAglinkFileSuffix(importSource);
                    //        SiFile.FileBody = File.ReadAllBytes(variableImportFilePath);
                    //        SiFile.LastUpdate = DateTime.UtcNow;

                    //        // commit changes
                    //        ufw.CommitChanges();
                    //    }
                    //}
                }
                #endregion
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
        public string GetAglinkFile(string stationName, XElement importSource)
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
        public IDataLayer GetAglinkFile(string stationName, XElement importSource, out string filebase, out InMemoryDataStore InMemory, out bool targetIsFile)
        {
            MTConnectDriver SimotionCommDriver = (MTConnectDriver)CommDriver;

            return GetAglinkFile(stationName, importSource, SimotionCommDriver.StrConnectionString, out filebase, out InMemory, out targetIsFile);
        }
        public IDataLayer GetAglinkFile(string stationName, XElement importSource, string targerConn, out string filebase, out InMemoryDataStore InMemory, out bool targetIsFile)
        {
            string FileName = GetAglinkFileFileNameOnly(stationName, importSource, targerConn);
            string connect = CommunicationDriver.GetConnectionString(targerConn, "Drivers", FileName, ".xml");
            IDataLayer dl = CommunicationDriver.GetSpecificDataLayer(connect, out filebase, out InMemory, out targetIsFile);

            // from db project, use os user temporary path
            if (!targetIsFile)
                filebase = Path.Combine(Path.GetTempPath(), string.Format("{0}{1}", FileName, ".xml"));

            return (dl);
        }
        private string GetAglinkFileFileNameOnly(string stationName, XElement importSource, string targerConn)
        {
            string SymbolicFile;

            // 1st time check Movicon Prject Source DB ? File ?
            if (!MoviconProjectSourceIsFile.HasValue)
            {
                InMemoryDataStore InMemory;
                bool TargetIsFile;
                string connect = CommunicationDriver.GetConnectionString(targerConn, "Drivers", stationName, ".xml");
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
                return string.Format("{0}_{1}", stationName, CommDriver.DriverName);
            }
            
            //else
            //{
            //    string Key = string.Format("{0}{1}", stationName, importSource);
            //    // file name with random part to differ from other Movicon instance
            //    if (!mapStationTiaFileFromDB.ContainsKey(Key))
            //        mapStationTiaFileFromDB[Key] = string.Format("{0}_{1}_{2}{3}", stationName, Guid.NewGuid(), CommDriver.DriverName, GetAglinkFileSuffix(importSource));

            //    return mapStationTiaFileFromDB[Key];
            //}
            return string.Empty;
        }
        #endregion Area save file

        #endregion
        #region Properties

        public class ElementParamete
        {
            public ElementParamete(string id, string value, string time, string error, string category)
            {
                Id = id;
                Value = value;
                Timestamp = time;
                Error = error;
                Category = category;

            }
            public string Id { get; set; }
            public string Value { get; set; }
            public string Timestamp { get; set; }
            public string Error { get; set; }
            public string Category { get; set; }

        }
        List<ElementParamete> ParameterRequestList = null;

        /// <summary>   Server Adress. </summary>
        private string _ServerAddress;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the  Server Adress. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <value> The Publish Key. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string ServerAddress
        {
            get { return _ServerAddress; }
        }

        /// <summary>   Server Port. </summary>
        private uint _ServerPort;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the  Server Port. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <value> The Publish Key. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint ServerPort
        {
            get { return _ServerPort; }
        }

        #endregion
    }
}
