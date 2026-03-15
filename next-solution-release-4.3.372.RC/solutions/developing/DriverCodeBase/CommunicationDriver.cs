////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	CommunicationDriver.cs
//
// summary:	Implements the communication driver class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverBaseInterfaces;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Opc.Ua;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.DB.Helpers;
using DriverCodeBase.Helpers;
using System.IO;
using System.Text.RegularExpressions;
using DriverCodeBase.Enumerators;
using Utilities;
using Amib.Threading;
using System.Windows;
using log4net;
#if !NETSTANDARD
using DriverSettingsInterfaces;
#endif

namespace DriverCodeBase
{
    
    /// <summary>   Base communication driver. </summary>
    public abstract class CommunicationDriver : ICommunicationDriver2, IStatistics, IDisposable
#if !NETSTANDARD
        , ICrossReferenceAware
#endif
    {
#region Constructors

        /// <summary>   Specialised default constructor for use only by derived class. </summary>
        protected CommunicationDriver()
        {
            _DriverName = DriverInfo.GetDriverName(this);
            _AssemblyName = DriverInfo.GetAssemblyName(this);
            driverStateCommandVariable = new StateCommandVariable();
            _DriverState = ComunicationState.Fault;
        }

        protected CommunicationDriver(string strSettingPath)
        {
            _DriverName = DriverInfo.GetDriverName(this);
            _AssemblyName = DriverInfo.GetAssemblyName(this);
            driverStateCommandVariable = new StateCommandVariable();
            _DriverState = ComunicationState.Fault;

#if NET_STANDARD
            strSettingPath = strSettingPath.Replace('\\', Path.DirectorySeparatorChar);
#endif
            strConnectionString = strSettingPath;
        }

#endregion

#region Data Members

        /// <summary>   The channels. </summary>
        readonly Dictionary<String, Channel> Channels = new Dictionary<String, Channel>();
        /// <summary>   The stations. </summary>
        readonly Dictionary<String, Station> Stations = new Dictionary<String, Station>();
        /// <summary>   The tag to station map. </summary>
        readonly Dictionary<NodeId, Station> TagToStationMap = new Dictionary<NodeId, Station>();
        /// <summary>   The tag to station map. </summary>
        List<Tag> AllDrivertags = new List<Tag>();

        /// <summary>   the TagDefinition of in use tags. </summary>
        Dictionary<NodeId, TagDefinition> inUseTags = new Dictionary<NodeId, TagDefinition>();

        /// <summary>   the TagDefinition of the tags setted in use at startup. </summary>
        List<TagDefinition> startupInUseTags = new List<TagDefinition>();

        readonly Dictionary<NodeId,  List<Station>> ObservedTagToStationsMap = new Dictionary<NodeId, List<Station>>();
        readonly Dictionary<NodeId, List<Channel>> ObservedTagToChannelsMap = new Dictionary<NodeId, List<Channel>>();

        /// <summary>   The list file to delete. </summary>
        readonly List<String> listFileToDelete = new List<String>();

        /// <summary>   true to loaded persistence dynamic jobs. </summary>
        private bool bLoadedPersistenceDynJobs;
        /// <summary>   true to running. </summary>
        private bool bRunning;
        /// <summary>   The lock maps. </summary>
        protected Object lockMaps = new Object();
        /// <summary>   The lock statistic. </summary>
        protected Object lockStatistic = new Object();

        /// <summary>   The refresh diagnostic timer. </summary>
        System.Timers.Timer RefreshDiagnosticTimer;

        /// <summary>  The State/Command variable associated to the driver. </summary>
        protected StateCommandVariable driverStateCommandVariable;
        
        /// <summary>   true to pending schedule list polling now. </summary>
        protected bool bPendingChange = false;

        private Object lockStartupSuspend = new Object();
        private int suspendCounter = 1;

        bool bRefreshingDiagnostic;
        
#endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the channels. </summary>
        ///
        /// <returns>   The channels. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public List<Channel> GetChannels()
        {
            return Channels.Values.ToList();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Adds a channel to 'value'. </summary>
        ///
        /// <param name="key">                  name of a valid counter. </param>
        /// <param name="value" type="Channel"> The value. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void AddChannel(string key, Channel value)
        {
            Channels.Add(key, value);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Removes the channel described by key. </summary>
        ///
        /// <param name="key">  name of a valid counter. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool RemoveChannel(string key)
        {
            return Channels.Remove(key);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the stations. </summary>
        ///
        /// <returns>   The stations. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public List<Station> GetStations()
        {
            return Stations.Values.ToList();
        }
        public List<Station> GetChannelStations(Channel channel)
        {
            List<Station> listStation = new List<Station>();
            foreach (Station st in GetStations())
                if (st.GetChannel() == channel)
                    listStation.Add(st);
            return listStation;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Adds a station to 'value'. </summary>
        ///
        /// <param name="key">                  name of a valid counter. </param>
        /// <param name="value" type="Station"> The value. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void AddStation(string key, Station value)
        {
            Stations.Add(key, value);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Removes the station described by key. </summary>
        ///
        /// <param name="key">  name of a valid counter. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool RemoveStation(string key)
        {
            return Stations.Remove(key);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets driver state. </summary>
        ///
        /// <returns>   The internal driver state. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public ComunicationState GetDriverState()
        {
            return DriverState;
        }

#region ICommunicationDriver Interface
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Check if the driver is enabled. </summary>
        ///
        /// <param name="strSettingPath" type="String"> Full pathname of the setting file. </param>
        ///
        /// <returns>   true if it enabled, false if it disabled. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool IsEnabled(String strSettingPath)
        {
            if (!String.IsNullOrEmpty(strSettingPath))
            {
#if NET_STANDARD
                strSettingPath = strSettingPath.Replace('\\', Path.DirectorySeparatorChar);
#endif

                using (IDataLayer idl = GetDriverDataLayer(strSettingPath))
                {
                    if (idl != null)
                    {
                        using (UnitOfWork ufw = new UnitOfWork(idl))
                        {
                            var settings = (from drvsettings in new XPQuery<DriverSettings>(ufw)/*.AsParallel()*/
                                            select drvsettings).FirstOrDefault();
                            if (settings != null)
                                return settings.Enable;
                        }
                    }
                }
            }

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Driver init interface. </summary>
        ///
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
        ///                                             null. </exception>
        ///
        /// <param name="strSettingPath">   . </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool Init(string strSettingPath)
        {
            return Init(strSettingPath, false, Guid.Empty);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Driver init interface. </summary>
        ///
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
        ///                                             null. </exception>
        ///
        /// <param name="strSettingPath">   . </param>
        /// <param name="isProtected">   . </param>
        /// <param name="protectionCode">   . </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool Init(string strSettingPath, bool isProtected, Guid protectionCode)
        {
            if (String.IsNullOrEmpty(strSettingPath))
                throw new ArgumentNullException("Active connection cannot be null or empty");

#if NET_STANDARD
            strSettingPath = strSettingPath.Replace('\\', Path.DirectorySeparatorChar);
#endif

            strConnectionString = strSettingPath;
            IDataLayer idl = GetDriverDataLayer();
            if (idl == null || !LoadDriverSettings(idl) || !Enable)
                return false;

            if (!IsBelongFromParent(isProtected, protectionCode))
#if !NET_STANDARD
                throw new FileFormatException("The settings may belong from another project.");
#else
                throw new Exception("The settings may belong from another project.");
#endif

            BuildListOfChannels();
            BuildListOfStations();

            bLoadedPersistenceDynJobs = LoadDynamicJobs();

            if (EnableStatistics)
                StartAllStatistics();
 
            return true;
        }

        /// <summary>   Initializes the diagnostic. </summary>
        public void InitDiagnosic()
        {
            if (RefreshDiagnosticTimer == null)
            {
                RefreshDiagnosticTimer = new System.Timers.Timer() { Interval = Properties.Settings.Default.RefreshDiagnosticIntervall };
                RefreshDiagnosticTimer.Elapsed += OnRefreshDiagnostic;
                RefreshDiagnosticTimer.Start();
            }
        }

        /// <summary>   Ends a diagnostic. </summary>
        public void EndDiagnosic()
        {
            if (RefreshDiagnosticTimer != null)
            {
                RefreshDiagnosticTimer.Stop();
                RefreshDiagnosticTimer.Elapsed -= OnRefreshDiagnostic;
                RefreshDiagnosticTimer = null;
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Raises the system. timers. elapsed event. </summary>
        ///
        /// <param name="sender" type="object">                     Source of the event. </param>
        /// <param name="e" type="System.Timers.ElapsedEventArgs">  Event information to send to
        ///                                                         registered event handlers. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void OnRefreshDiagnostic(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (bRefreshingDiagnostic)
                return;

            bRefreshingDiagnostic = true;
            try
            {
                if (StatisticsData != null)
                {
                    StatisticsData.Update(StatisticSetting.NodeDataNames.ElapsedTime.ToString(), StatisticsData.GetTotalTimeOn().ToString(@"d\.hh\:mm\:ss"));

                    long totalJobsInUse = 0;
                    foreach (var channelname in Channels.Keys)
                        totalJobsInUse += Channels[channelname].InUseJobs;
                    StatisticsData.Update(StatisticSetting.NodeDataNames.TotalJobsInUse.ToString(), totalJobsInUse);
                }

                foreach (var channel in GetChannels())
                {
                    channel.OnRefreshDiagnostic();
                }
                foreach (var station in GetStations())
                {
                    station.OnRefreshDiagnostic();
                }
            }
            finally
            {
                bRefreshingDiagnostic = false;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Prepares this object for use. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool Startup()
        {
            if (smartThreadPool == null)
            {
                var startupInfo = new STPStartInfo()
                {
                    ThreadPoolName = "CommunicationDrivers",
                    ThreadPriority = System.Threading.ThreadPriority.Normal,
                    MaxWorkerThreads = SysInfo.GetNumberOfLogicalProcessors(),
                    AreThreadsBackground = false
                };

                smartThreadPool = new SmartThreadPool(startupInfo);
            }

            int interlokCounter = Interlocked.Decrement(ref suspendCounter);
            if (interlokCounter != 0)
                return true;

            List<TagDefinition> copyStartupInUseTags;
            bool bRet;
            lock (lockStartupSuspend)
            {
                bRet = StartupAllStations();
                ResumeAllStatistics();

                bRunning = true;

                //steve 240811
                if (SyncAtStartup)
                    ExecuteAllJobsAtStartup();
                //++++++++++++

                if (EnableStatistics)
                    InitDiagnosic();

                copyStartupInUseTags = new List<TagDefinition>(startupInUseTags);
                OnStateChanged(ComunicationState.Running);
            }

            if (copyStartupInUseTags.Count != 0)
                InUseDynamics(copyStartupInUseTags, true);

            return bRet;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Suspends this object. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool Suspend()
        {
            int interlokCounter = Interlocked.Increment(ref suspendCounter);
            if (interlokCounter != 1)
                return true;

            List<TagDefinition> copyInUseTags;
            lock (lockMaps)
            {
                copyInUseTags = new List<TagDefinition>(inUseTags.Values.ToList());
            }
            bool bRet;
            lock (lockStartupSuspend)
            {
                startupInUseTags.Clear();
                startupInUseTags.AddRange(copyInUseTags);
                bRunning = false;
                bRet = SuspendAllStations();
                SuspendAllStatistics();
                if (EnableStatistics)
                    EndDiagnosic();
                OnStateChanged(ComunicationState.Suspend);
            }

            return bRet;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Terminates this object. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool Terminate()
        {
            bool bRet = TerminateAllStations();

            // Removed to solve FOGBUGZ 11643 (exception when the server was stopped)
            //TerminateAllStatistics();

            if (EnableStatistics)
                EndDiagnosic();

            listFileToDelete.ForEach(path =>
            {
                try
                {
                    System.IO.File.Delete(path);
                }
                catch 
                { }
            });

            return bRet;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Compare tag by dynamic. </summary>
        ///
        /// <param name="x" type="Tag"> The Tag to process. </param>
        /// <param name="y" type="Tag"> The Tag to process. </param>
        ///
        /// <returns>   An int. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static int CompareTagByDynamic(Tag x, Tag y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0; //==
                else
                    return -1;// x < y
            }
            else
            {
                //x!= null
                if (y == null)
                    return 1; //x > y
                else
                {
                    return x.TagNode.DynamicSettings.CompareTo(y.TagNode.DynamicSettings);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Enabled/disable prototype split. </summary>
        ///
        /// <returns>   true if a prototype split is enabled, false if not. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool IsPrototypeSplitEnabled()
        {
            return false;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Adds the dynamics. </summary>
        ///
        /// <param name="tags" type="IList<TagDefinition>"> The tags. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool AddDynamics(IList<TagDefinition> tags)
        {
            if (tags.Count > 0 && (bRunning || !bLoadedPersistenceDynJobs))
            {
                // Added just for debugging
                DateTime dtime0 = DateTime.UtcNow;

                Dictionary<NodeId, string> TagChangeSettingMap = new Dictionary<NodeId, string>();

                LoadChangeSettings(TagChangeSettingMap);

                TagDefinition newTag;
                AllDrivertags.Clear();
                foreach (var tagDef in tags)
                {
                    newTag = tagDef;
                    if (TagChangeSettingMap.ContainsKey(tagDef.NodeId))
                        newTag.DynamicSettings = TagChangeSettingMap[tagDef.NodeId];
                    AllDrivertags.Add(CreateTag(newTag));
                }
                AllDrivertags.Sort(CompareTagByDynamic);

                var validtags = ParseDynamicTags(AllDrivertags);

                //if (validtags > 0 && !bRunning)
                //    SaveChangeSettings();

                if (validtags > 0 && !bRunning)
                    SaveDynamicJobs();
                if (StatisticsData != null)
                {
                    if (bRunning)
                        StatisticsData.IncreaseCounter(StatisticSetting.NodeDataNames.TotalTags.ToString(), validtags);
                    else
                        StatisticsData.Update(StatisticSetting.NodeDataNames.TotalTags.ToString(), validtags);

                    long totalJobs = 0;
                    foreach (var stationname in Stations.Keys)
                        totalJobs += Stations[stationname].ListWholeJob.Count();
                    StatisticsData.Update(StatisticSetting.NodeDataNames.TotalJobs.ToString(), totalJobs);
                }

                // Added just for debugging
                DateTime dtime4 = DateTime.UtcNow;
                //System.Diagnostics.Trace.TraceInformation("AddDynamics {0} {1} ms", dtime0, (dtime4 - dtime0).TotalMilliseconds);
            }

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   In use dynamics. </summary>
        ///
        /// <param name="tags" type="IList<TagDefinition>"> The tags. </param>
        /// <param name="bInUse" type="bool">               true to in use. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool InUseDynamics(IList<TagDefinition> tags, bool bInUse)
        {
            Dictionary<NodeId, TagDefinition> copyInUseTags;
            Dictionary<NodeId, Station> copyTagToStationMap;
            lock (lockMaps)
            {
                copyInUseTags = new Dictionary<NodeId, TagDefinition>(inUseTags);
                copyTagToStationMap = new Dictionary<NodeId, Station>(TagToStationMap);
            }

            bool ret = true;
            int matchingTagsCount = 0;
            int deltaTag = 0;
            var mapInUseTags = new Dictionary<TagDefinition, bool>();
            foreach (var tag in tags)
            {
                //System.Diagnostics.Trace.TraceInformation(string.Format("{2} InUseDynamic tag:{0} inuse:{1}", tag.NodeId.ToString(), bInUse, DateTime.Now.ToLongTimeString()));
                if (copyTagToStationMap.ContainsKey(tag.NodeId))
                {
                    if (copyTagToStationMap[tag.NodeId].SetInUse(tag.NodeId, bInUse, tag.SamplingInterval))
                    {
                        matchingTagsCount = (copyInUseTags.ContainsKey(tag.NodeId) ? 1 : 0);
                        //NodeId searchNodeId = tag.NodeId;
                        //List<TagDefinition> matchingTags = (from t in copyInUseTags.AsParallel()
                        //                                    where (t.NodeId == searchNodeId)
                        //                                    select t).ToList();

                        if ((matchingTagsCount > 0) && !bInUse)
                        {
                            deltaTag--;
                            copyInUseTags.Remove(tag.NodeId);
                            mapInUseTags[tag] = false;
                        }
                        else if (!(matchingTagsCount > 0) && bInUse)
                        {
                            deltaTag++;
                            copyInUseTags[tag.NodeId] = tag;
                            mapInUseTags[tag] = true;
                        }
                    }
                    else
                    {
                        ret = false;
                        //log some message...
                    }
                }
                else if (tag.NodeId.IdType == IdType.String)
                {
                    NodeId n = tag.NodeId;
                    string s = tag.NodeId.ToString();
                    int p = s.IndexOf("?");
                    if (p != -1)
                    {
                        s = s.Substring(0, p);
                        s = s.Replace(";s=", ";g=");
                    }
                    n = new NodeId(s);

                    if (copyTagToStationMap.ContainsKey(n))
                    {
                        if (copyTagToStationMap[n].SetInUse(tag.NodeId, bInUse, tag.SamplingInterval))
                        {
                            matchingTagsCount = (copyInUseTags.ContainsKey(n) ? 1 : 0);
                            //List<TagDefinition> matchingTags = (from t in copyInUseTags.AsParallel()
                            //                                    where (t.NodeId == n)
                            //                                    select t).ToList();
                            if ((matchingTagsCount > 0) && !bInUse)
                            {
                                deltaTag--;
                                copyInUseTags.Remove(n);
                                mapInUseTags[tag] = false;
                            }
                            else if (!(matchingTagsCount > 0) && bInUse)
                            {
                                deltaTag++;
                                copyInUseTags[n] = tag;
                                mapInUseTags[tag] = true;
                            }
                        }
                        else
                        {
                            ret = false;
                            //log some message...
                        }
                    }
                }
                else
                {
                    ret = false;
                    //log some message...
                }
            }

            lock (lockMaps)
            {
                foreach (var tag in mapInUseTags.Keys)
                {
                    bool bIn = inUseTags.ContainsKey(tag.NodeId);
                    if (mapInUseTags[tag] && !bIn)
                        inUseTags[tag.NodeId] = tag;
                    else if (!mapInUseTags[tag] && bIn)
                        inUseTags.Remove(tag.NodeId);
                }
            }

            if (StatisticsData != null)
            {
                lock (lockStatistic)
                {
                    StatisticsData.IncreaseCounter(StatisticSetting.NodeDataNames.TotalTagsInUse.ToString(), deltaTag);
                    long TotalTagsInUse = GetCounterValue(StatisticSetting.NodeDataNames.TotalTagsInUse.ToString());
                    if (TotalTagsInUse > GetCounterValue(StatisticSetting.NodeDataNames.PeakOfTagsInUse.ToString()))
                        StatisticsData.Update(StatisticSetting.NodeDataNames.PeakOfTagsInUse.ToString(), TotalTagsInUse);
                }
            }

            return ret;
        }

        //public virtual bool InUseDynamics(IList<TagDefinition> tags, bool bInUse)
        //{
        //    bool ret = true;
        //    int deltaTag = 0;
        //    var mapInUseTags = new Dictionary<TagDefinition, sbyte>();
        //    Dictionary<NodeId, Station> tagToStationMap = new Dictionary<NodeId, Station>();

        //    lock (lockMaps)
        //    {
        //        foreach (var tag in tags)
        //        {
        //            NodeId n = tag.NodeId;
        //            bool inStationMap = false;

        //            if (TagToStationMap.ContainsKey(n))
        //            {
        //                inStationMap = true;
        //            }
        //            else
        //            {
        //                string s = tag.NodeId.ToString();
        //                int p = s.IndexOf("?");
        //                if (p != -1)
        //                {
        //                    s = s.Substring(0, p);
        //                    s = s.Replace(";s=", ";g=");
        //                }
        //                n = new NodeId(s);

        //                if (TagToStationMap.ContainsKey(n))
        //                {
        //                    inStationMap = true;
        //                }
        //            }

        //            if (inStationMap)
        //            {
        //                int matchingTagsCount = (inUseTags.ContainsKey(n) ? 1 : 0);

        //                tagToStationMap[n] = TagToStationMap[n];

        //                if ((matchingTagsCount > 0) && !bInUse)
        //                {
        //                    deltaTag--;
        //                    mapInUseTags[tag] = 0;
        //                }
        //                else if (!(matchingTagsCount > 0) && bInUse)
        //                {
        //                    deltaTag++;
        //                    mapInUseTags[tag] = 1;
        //                }
        //            }
        //        }
        //    }

        //    foreach (var tag in mapInUseTags.Keys)
        //    {
        //        if (!tagToStationMap[tag.NodeId].SetInUse(tag.NodeId, bInUse, tag.SamplingInterval))
        //            mapInUseTags[tag] = -1;
        //    }

        //    lock (lockMaps)
        //    {
        //        foreach (var tag in mapInUseTags.Keys)
        //        {
        //            if (mapInUseTags[tag] == 1 && !inUseTags.ContainsKey(tag.NodeId))
        //                inUseTags[tag.NodeId] = tag;
        //            else if (mapInUseTags[tag] == 0 && inUseTags.ContainsKey(tag.NodeId))
        //                inUseTags.Remove(tag.NodeId);
        //        }
        //    }

        //    if (StatisticsData != null)
        //    {
        //        lock (lockStatistic)
        //        {
        //            StatisticsData.IncreaseCounter(StatisticSetting.NodeDataNames.TotalTagsInUse.ToString(), deltaTag);
        //            long TotalTagsInUse = GetCounterValue(StatisticSetting.NodeDataNames.TotalTagsInUse.ToString());
        //            if (TotalTagsInUse > GetCounterValue(StatisticSetting.NodeDataNames.PeakOfTagsInUse.ToString()))
        //                StatisticsData.Update(StatisticSetting.NodeDataNames.PeakOfTagsInUse.ToString(), TotalTagsInUse);
        //        }
        //    }

        //    return ret;
        //}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the write tag action. </summary>
        ///
        /// <param name="tag"> the tag </param>
        /// <param name="value"> the value </param>
        /// <param name="statusCode"> result of write operation </param>
        /// <param name="timestamp"> date/time of write operation </param>
        /// <param name="ignoreWriteAsync"> force driver to ignore ignoreWriteAsync paramter </param>
        ///
        /// <returns>   An uint. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual uint OnWriteTag(TagDefinition tag, ref object value, ref StatusCode statusCode, ref DateTime timestamp, bool ignoreWriteAsync = false)
        {
            return OnWriteTag(tag, ref value, ref statusCode, ref timestamp, ignoreWriteAsync, false);
        }
        public uint OnWriteTag(TagDefinition tag, ref object value, ref StatusCode statusCode, ref DateTime timestamp, bool ignoreWriteAsync = false, bool forceSynchWrite = false)
        {
            /*
             * modify the job to fill the TagListToUpdate
             * set the job in use, if not already,
             * promote the job to HiPri list, if not.
             */
            //System.Diagnostics.Trace.TraceInformation("OnWriteTag tag:{0} value:{1}", tag.NodeId.ToString(), value.ToString());
            Station station;
            lock (lockMaps)
            {
                if (TagToStationMap.ContainsKey(tag.NodeId))
                    station = TagToStationMap[tag.NodeId];
                else
                    return StatusCodes.BadNodeIdInvalid;
            }
            if (bRunning)
                return station.OnWriteTag(tag.NodeId, ref value, ref statusCode, ref timestamp, ignoreWriteAsync, forceSynchWrite);
            else
                return station.OnUpdateTag(tag.NodeId, ref value, ref statusCode, ref timestamp);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the write values action. </summary>
        ///
        /// <param name="startingAddress" type="String">    The starting address. </param>
        /// <param name="dataValues" type="IList<object>">  The data values. </param>
        ///
        /// <returns>   An uint. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual uint OnWriteValues(String startingAddress, IList<object> dataValues)
        {
            NamedTag sourcevar = new NamedTag() { ArrayDimension = 0, Name = "tagToWrite", DataType = new NodeId(Opc.Ua.DataTypes.UInt16), NodeId = new NodeId(Guid.NewGuid()) };
            var tagToWrite = CreateTag(new TagDefinition()
            {
                DynamicSettings = startingAddress,
                DataType = sourcevar.DataType,
                NodeId = sourcevar.NodeId
            });

            if (!tagToWrite.bIsValid)
                return StatusCodes.BadInvalidArgument;

            Station station = null;
            lock (lockMaps)
            {
                if (Stations.ContainsKey(tagToWrite.DynSettings.StationName))
                    station = Stations[tagToWrite.DynSettings.StationName];
            }

            if (station == null)
                return StatusCodes.BadInvalidArgument;

            CommJob testJob = null;
            CommJob MainJob = null;
            //uint jsize = 0;

            List<object> outputvalues = new List<object>();
            outputvalues.Add(new uint());

            DynTagSettings dtCalc = tagToWrite.DynSettings;//initiate DynTagSettings, for the following DynAddress calculations
            TagDefinition previousTDef = new TagDefinition();
            int error = 0;
            uint result = 0;
            for (int i = 0; i < dataValues.Count; i++)
            {
                Variant value = (Variant)dataValues[i];

                int vdim = ((value.Value as Byte[]) != null ? (value.Value as Byte[]).Length : 0);
                NodeId dt = Opc.Ua.DataTypes.GetDataTypeId(value.Value);

                bool isString = (dt.IdType == IdType.Numeric && ((uint)dt.Identifier == (uint)BuiltInType.ByteString || (uint)dt.Identifier == (uint)BuiltInType.String));

                // define string as array of byte to calculate the correct size of job --> necessary to get next tag address 
                if (dt.IdType == IdType.Numeric && (uint)dt.Identifier == (uint)BuiltInType.ByteString) {
                    //dt = new NodeId((uint)BuiltInType.Byte);
                    dt = new NodeId((uint)BuiltInType.String);
                    vdim = 0;
                }

                uint length = 0;
                if (isString)
                {
                    var str = value.Value as string;
                    if (str != null)
                    {
                        length = (uint)str.Length;
                    }
                    else
                    {
                        var bstr = value.Value as byte[];
                        if (bstr != null)
                        {
                            length = (uint)bstr.Length;
                        }
                    }
                    dtCalc.DeviceSize = (int)length;
                }
                else
                {
                    dtCalc.DeviceSize = 0;
                }

                var tdef = new TagDefinition() { ArrayDimension = (uint)vdim, DataType = dt, NodeId = new NodeId((uint)i) };
                if (i == 0)
                {
                    //first value, assign start address as passed
                    tdef.DynamicSettings = tagToWrite.TagNode.DynamicSettings;
                    dtCalc.TryParse(tagToWrite.TagNode.DynamicSettings);
                }
                else
                    tdef.DynamicSettings = dtCalc.GetNextDynSetting(previousTDef, tdef);

                previousTDef = tdef;

                var ntag = CreateTag(tdef);
                ntag.Value.Value = Utils.Clone(value.Value);

                if (ntag.TagNode.DataType.IdType == IdType.Numeric && (uint)ntag.TagNode.DataType.Identifier == (uint)BuiltInType.String)
                {
                    //var str = ntag.Value.Value as string;
                    //if (str != null)
                    //    ntag.Size = (uint)str.Length;
                    byte[] StrBytes = ntag.Value.Value as byte[];
                    // remove character after string terminator
                    var Str = Encoding.UTF8.GetString(StrBytes).TrimEnd(new Char[] { '\0' });
                    if (Str != null)
                        ntag.Value.Value = Str;
                }
                //jsize += ntag.Size;
                if (i == 0)
                {
                    MainJob = station.CreateJob(ntag);
                    // discard invalid job --> recipe element parameters are invalid for specific driver
                    if (!MainJob.IsValid)
                        return StatusCodes.BadTypeDefinitionInvalid;
                    MainJob.Type = LinkType.UnconditionalOutput;
                    if (i == dataValues.Count - 1)
                    {
                        //last
                        result = station.ExecuteSyncroJob(MainJob, false, 0, MainJob.Type, outputvalues, false);
                        MainJob.ResetSynchro.Set(); // channel working thread can continue to process other jobs
                        if (result != StatusCodes.Good)
                            return (error == 0 ? result : StatusCodes.Bad);
                                                
                        error |= Convert.ToInt32(outputvalues[0]);
                        break;
                    }
                }
                else
                {
                    testJob = station.CreateJob(ntag);
                    // discard invalid job --> recipe element parameters are invalid for specific driver
                    if (!testJob.IsValid)
                        return StatusCodes.BadTypeDefinitionInvalid;
                    testJob.Type = LinkType.UnconditionalOutput;
                    uint newIndex = 0;
                    var aggType = MainJob.TestAggregateJob(testJob, out newIndex);

                    if (aggType != JobAggregationType.JobAggregImpossible)
                    {
                        //aggregate...
                        MainJob.AggregateJob(testJob, aggType, newIndex);
                        if (i == dataValues.Count - 1)
                        {
                            //last
                            result = station.ExecuteSyncroJob(MainJob, false, 0, MainJob.Type, outputvalues, false);
                            MainJob.ResetSynchro.Set(); // channel working thread can continue to process other jobs
                            if (result != StatusCodes.Good)
                                return (error == 0 ? result : StatusCodes.Bad);

                            error |= Convert.ToInt32(outputvalues[0]);
                            break;
                        }
                        else
                            continue;
                    }
                    else
                    {
                        //execute Mainjob, 
                        result = station.ExecuteSyncroJob(MainJob, false, 0, MainJob.Type, outputvalues, false);
                        MainJob.ResetSynchro.Set(); // channel working thread can continue to process other jobs
                        if (result != StatusCodes.Good)
                            return (error == 0 ? result : StatusCodes.Bad);

                        error |= Convert.ToInt32(outputvalues[0]);

                        if (i == dataValues.Count - 1)
                        {
                            //to end
                            result = station.ExecuteSyncroJob(testJob, false, 0, testJob.Type, outputvalues, false);
                            testJob.ResetSynchro.Set(); // channel working thread can continue to process other jobs
                            if (result != StatusCodes.Good)
                                return (error == 0 ? result : StatusCodes.Bad);

                            error |= Convert.ToInt32(outputvalues[0]);
                            break;
                        }
                        else
                        {
                            MainJob = testJob;
                            continue;
                        }
                    }
                }
            }

            return (error == 0 ? result : StatusCodes.Bad);
        }


        /// <summary>
        /// Parse readed value to manage string case
        /// </summary>
        /// <param name="tdef">data type of readed data</param>
        /// <param name="dataValue">target array</param>
        /// <param name="dataValueIndex">target array index</param>
        /// <param name="dataresult">source array with readed data</param>
        /// <param name="dataresultIndex">source array index of readed data</param>
        private void ParseOnReadValue(TagDefinition tdef, ref IList<object> dataValue, int dataValueIndex, IList<object> dataresult, int dataresultIndex)
        {
            // reconvert string into array of bytes for Movicon
            if (tdef.DataType == (uint)BuiltInType.String)
            {
                string Str = dataresult[dataresultIndex] as string;
                if (Str == null)
                    dataValue[dataValueIndex] = new byte[0];
                else
                    dataValue[dataValueIndex] = new UTF8Encoding().GetBytes(Str);
            }
            else
            {
                dataValue[dataValueIndex] = dataresult[dataresultIndex];
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the read values action. </summary>
        ///
        /// <param name="startingAddress" type="String">    The starting address. </param>
        /// <param name="dataValues" type="IList<object>">  The data values. </param>
        ///
        /// <returns>   An uint. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual uint OnReadValues(String startingAddress, IList<object> dataValues)
        {
            NamedTag sourcevar = new NamedTag() { ArrayDimension = 0, Name = "tagToRead", DataType = new NodeId(Opc.Ua.DataTypes.UInt16), NodeId = new NodeId(Guid.NewGuid()) };
            var tagToRead = CreateTag(new TagDefinition()
            {
                DynamicSettings = startingAddress,
                DataType = sourcevar.DataType,
                NodeId = sourcevar.NodeId
            });

            if (!tagToRead.bIsValid)
                return StatusCodes.BadInvalidArgument;

            Station station = null;
            lock (lockMaps)
            {
                if (Stations.ContainsKey(tagToRead.DynSettings.StationName))
                    station = Stations[tagToRead.DynSettings.StationName];
            }

            if (station == null)
                return StatusCodes.BadInvalidArgument;

            CommJob testJob = null;
            CommJob MainJob = null;
            //uint jsize = 0;

            List<object> dataresult = new List<object>();
            dataresult.Add(new uint());

            DynTagSettings dtCalc = tagToRead.DynSettings;//initiate DynTagSettings, for the following DynAddress calculations
            TagDefinition previousTDef = new TagDefinition();
            int error = 0;
            uint result = 0;
            int resultindex = 0;
            for (int i = 0; i < dataValues.Count; i++)
            {
                Variant value = (Variant)dataValues[i];

                int vdim = ((value.Value as Byte[]) != null ? (value.Value as Byte[]).Length : 0);
                NodeId dt = Opc.Ua.DataTypes.GetDataTypeId(value.Value);
                bool isString = (dt.IdType == IdType.Numeric && ((uint)dt.Identifier == (uint)BuiltInType.ByteString || (uint)dt.Identifier == (uint)BuiltInType.String));
                if (dt.IdType == IdType.Numeric && (uint)dt.Identifier == (uint)BuiltInType.ByteString)
                {
                    //dt = new NodeId((uint)BuiltInType.Byte);
                    dt = new NodeId((uint)BuiltInType.String);
                    vdim = 0;
                }

                uint length = 0;
                if (isString)
                {
                    var str = value.Value as string;
                    if (str != null)
                        length = (uint)str.Length;
                    else
                    {
                        var bstr = value.Value as byte[];
                        if (bstr != null)
                            length = (uint)bstr.Length;
                    }
                    dtCalc.DeviceSize = (int)length;
                }
                else
                {
                    dtCalc.DeviceSize = 0;
                }
                var tdef = new TagDefinition() { ArrayDimension = (uint)vdim, DataType = dt, NodeId = new NodeId((uint)i) };
                if (i == 0)
                {
                    //first value, assign start address as passed
                    tdef.DynamicSettings = tagToRead.TagNode.DynamicSettings;
                    dtCalc.TryParse(tagToRead.TagNode.DynamicSettings);
                }
                else
                    tdef.DynamicSettings = dtCalc.GetNextDynSetting(previousTDef, tdef);

                previousTDef = tdef;

                var ntag = CreateTag(tdef);
                ntag.Value.Value = Utils.Clone(value.Value);
                                
                if (i == 0)
                {
                    MainJob = station.CreateJob(ntag);
                    // discard invalid job --> recipe element parameters are invalid for specific driver
                    if (!MainJob.IsValid)
                        return StatusCodes.BadTypeDefinitionInvalid;
                    MainJob.Type = LinkType.Input;
                    dataresult.Add(dataValues[i]);
                    if (i == dataValues.Count - 1)
                    {
                        //last
                        result = station.ExecuteSyncroJob(MainJob, false, 0, MainJob.Type, dataresult);
                        MainJob.ResetSynchro.Set(); // channel working thread can continue to process other jobs
                        if (result != StatusCodes.Good)
                            return (error == 0 ? result : StatusCodes.Bad);

                        if (Convert.ToUInt32(dataresult[0]) == (int)DriverErrorCodes.ErrorNoError)
                        {
                            //update datavalues
                            for (int j = 1; j < dataresult.Count; j++)
                                //dataValues[resultindex++] = dataresult[j];
                                ParseOnReadValue(MainJob.TagsList[0].TagNode, ref dataValues, resultindex++, dataresult, j);
                        }

                        error |= Convert.ToInt32(dataresult[0]);                        
                        break;
                    }
                }
                else
                {
                    testJob = station.CreateJob(ntag);
                    // discard invalid job --> recipe element parameters are invalid for specific driver
                    if (!testJob.IsValid)
                        return StatusCodes.BadTypeDefinitionInvalid;
                    testJob.Type = LinkType.Input;
                    uint newIndex = 0;
                    var aggType = MainJob.TestAggregateJob(testJob, out newIndex);
                        
                    if (aggType != JobAggregationType.JobAggregImpossible)
                    {
                        //aggregate...
                        MainJob.AggregateJob(testJob, aggType, newIndex);
                        dataresult.Add(dataValues[i]);
                        if (i == dataValues.Count-1)
                        {
                            //last
                            result = station.ExecuteSyncroJob(MainJob, false, 0, MainJob.Type, dataresult);
                            MainJob.ResetSynchro.Set(); // channel working thread can continue to process other jobs
                            if (result != StatusCodes.Good)
                                return (error == 0 ? result : StatusCodes.Bad);

                            if (Convert.ToUInt32(dataresult[0]) == (int)DriverErrorCodes.ErrorNoError)
                            {
                                //update datavalues
                                for (int j = 1; j < dataresult.Count; j++)
                                    //dataValues[resultindex++] = dataresult[j];
                                    ParseOnReadValue(MainJob.TagsList[0].TagNode, ref dataValues, resultindex++, dataresult, j);
                            }

                            error |= Convert.ToInt32(dataresult[0]);                            
                            break;
                        }
                        else
                            continue;
                    }
                    else
                    { 
                        //execute Mainjob, 
                        result = station.ExecuteSyncroJob(MainJob, false, 0, MainJob.Type, dataresult);
                        MainJob.ResetSynchro.Set(); // channel working thread can continue to process other jobs
                        if (result != StatusCodes.Good)
                            return (error == 0 ? result : StatusCodes.Bad);

                        //update datavalues
                        for (int j = 1; j < dataresult.Count; j++)
                        {
                            ParseOnReadValue(MainJob.TagsList[0].TagNode, ref dataValues, resultindex++, dataresult, j);
                        }

                        error |= Convert.ToInt32(dataresult[0]);
                        dataresult.Clear();
                        dataresult.Add(new uint());
                        dataresult.Add(dataValues[i]);
                        if (i == dataValues.Count - 1)
                        {
                            //to end
                            result = station.ExecuteSyncroJob(testJob, false, 0, testJob.Type, dataresult);
                            testJob.ResetSynchro.Set(); // channel working thread can continue to process other jobs
                            if (result != StatusCodes.Good)
                                return (error == 0 ? result : StatusCodes.Bad);

                            if (Convert.ToUInt32(dataresult[0]) == (int)DriverErrorCodes.ErrorNoError)
                            {
                                //update datavalues
                                for (int j = 1; j < dataresult.Count; j++)
                                    //dataValues[resultindex++] = dataresult[j];
                                    ParseOnReadValue(testJob.TagsList[0].TagNode, ref dataValues, resultindex++, dataresult, j);
                            }

                            error |= Convert.ToInt32(dataresult[0]);
                            break;
                        }
                        else
                        {
                            MainJob = testJob;
                            continue;
                        }
                    }
                }
            }

            return (error == 0 ? result : StatusCodes.Bad);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the method call action. </summary>
        ///
        /// <param name="tag" type="TagDefinition">             The tag. </param>
        /// <param name="inputArguments" type="IList<object>">  The input arguments. </param>
        /// <param name="outputArguments" type="IList<object>"> The output arguments. </param>
        ///
        /// <returns>   An uint. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual uint OnMethodCall(TagDefinition tag, IList<object> inputArguments, IList<object> outputArguments)
        {
            Station station = null;
            lock (lockMaps)
            {
                if (TagToStationMap.ContainsKey(tag.NodeId))
                    station = TagToStationMap[tag.NodeId];
            }

            if (station != null)
            {
                return station.OnMethodCall(tag.NodeId, inputArguments, outputArguments);
            }
            else
            {
                //log some message...
                return StatusCodes.BadNodeIdInvalid;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the resume diagnostic action. </summary>
        ///
        /// <returns>   An uint. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual uint OnResumeDiagnostic()
        {
            ResumeAllStatistics();
            return StatusCodes.Good;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the suspend diagnostic action. </summary>
        ///
        /// <returns>   An uint. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual uint OnSuspendDiagnostic()
        {
            SuspendAllStatistics();
            return StatusCodes.Good;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the reset diagnostic action. </summary>
        ///
        /// <returns>   An uint. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual uint OnResetDiagnostic()
        {
            ResetAllStatistcs();
            return StatusCodes.Good;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets driver diagram variable. </summary>
        ///
        /// <returns>   The driver diagram variable. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual List<StatisicTag> GetDriverDiagVar()
        {
             return StatisticSetting.GetStatisicTag(StatisticNodes);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets channel diagram variable. </summary>
        ///
        /// <returns>   The channel diagram variable. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual List<StatisicTag> GetChannelDiagVar()
        {
            return StatisticSetting.GetStatisicTag(Channel.StatisticNodes);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets station diagram variable. </summary>
        ///
        /// <returns>   The station diagram variable. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual List<StatisicTag> GetStationDiagVar()
        {
            return StatisticSetting.GetStatisicTag(Station.StatisticNodes);
        }

        

        /// <summary>   Event queue for all listeners interested in Starting events. </summary>
        public event EventHandler Starting;
        /// <summary>   Executes the starting action. </summary>
        public virtual void OnStarting()
        {
            var temp = Starting;
            if (temp != null)
                temp(this, EventArgs.Empty);
        }

        /// <summary>   Event queue for all listeners interested in Started events. </summary>
        public event EventHandler Started;
        /// <summary>   Executes the started action. </summary>
        public virtual void OnStarted()
        {
            var temp = Started;
            if (temp != null)
                temp(this, EventArgs.Empty);
        }

        /// <summary>   Event queue for all listeners interested in Suspending events. </summary>
        public event EventHandler Suspending;

        /// <summary>   Event queue for all listeners interested in Suspended events. </summary>
        public event EventHandler Suspended;

        /// <summary>   Event queue for all listeners interested in Terminating events. </summary>
        public event EventHandler Terminating;

        /// <summary>   Event queue for all listeners interested in Terminated events. </summary>
        public event EventHandler Terminated;

        /// <summary>   Event queue for all listeners interested in TagChanging events. </summary>
        public event EventHandler<ChangedTagArgs> TagChanging;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the tag changing action. </summary>
        ///
        /// <param name="node" type="NodeId">   The value. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool OnTagChanging(NodeId node)
        {           
            var temp = TagChanging;
            if (temp != null)
            {
                ChangedTagArgs e = new ChangedTagArgs();
                e.driverName = DriverName;
                e.NodeId = node;
                temp(this, e);
                if (e.NodeId == node)
                    return true;
            }
            return false;
        }

        /// <summary>   Event queue for all listeners interested in TagChanged events. </summary>
        public event EventHandler<ChangedTagArgs> TagChanged;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the tag changed action. </summary>
        ///
        /// <param name="node" type="NodeId">       The value. </param>
        /// <param name="value" type="DataValue">   . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool OnTagChanged(NodeId node, DataValue value)
        {
            return (OnTagChanged(node, value, default(DateTime)));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the tag changed action. </summary>
        ///
        /// <param name="node" type="NodeId">       The value. </param>
        /// <param name="value" type="DataValue">   . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool OnTagChanged(NodeId node, DataValue value, DateTime Timestamp)
        {
            return OnTagChanged(node, value, Timestamp, false);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the tag changed action. </summary>
        ///
        /// <param name="node" type="NodeId">           The node ID of the Tag. </param>
        /// <param name="value" type="DataValue">       The new value. </param>
        /// <param name="Timestamp" type="DateTimee">   The new timestamp. </param>
        /// <param name="allowNullValues" type="bool">  Mark null value as a good value. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool OnTagChanged(NodeId node, DataValue value, DateTime Timestamp, bool allowNullValues)
        {
            var temp = TagChanged;
            if (temp != null)
            {
                ChangedTagArgs e = new ChangedTagArgs();
                e.AllowNullValues = allowNullValues;
                e.driverName = DriverName;
                e.NodeId = node;

                if (Timestamp == default(DateTime))
                {
                    value.SourceTimestamp = DateTime.UtcNow;
                    e.DataValues.Add(new DataValue(value));
                    temp(this, e);
                    return true;
                }
                else
                {
                    value.SourceTimestamp = Timestamp;
                    e.DataValues.Add(new DataValue(value));
                    temp(this, e);
                    return true;
                }
            }
            return false;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the tag changed action. </summary>
        ///
        /// <param name="node" type="NodeId">           The node ID of the Tag. </param>
        /// <param name="value" type="DataValue">       The new value. </param>
        /// <param name="quality" type="DataValue">     The new quality. </param>
        /// <param name="Timestamp" type="DateTimee">   The new timestamp. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool OnTagChangedValueQualityTimestamp(NodeId node, DataValue value, uint quality, DateTime Timestamp)
        {
            var temp = TagChanged;
            if (temp != null)
            {
                ChangedTagArgs e = new ChangedTagArgs();
                e.NodeId = node;

                if (Timestamp == default(DateTime))
                {
                    value.SourceTimestamp = DateTime.UtcNow;
                    value.StatusCode = quality;
                    e.DataValues.Add(new DataValue(value));
                    temp(this, e);
                    return true;
                }
                else
                {
                    value.SourceTimestamp = Timestamp;
                    value.StatusCode = quality;
                    e.DataValues.Add(new DataValue(value));
                    temp(this, e);
                    return true;
                }
            }
            return false;
        }
                
        /// <summary>   Event queue for all listeners interested in system events. </summary>
        public event EventHandler<SystemEventArgs> SystemEvent;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the system event action. </summary>
        ///
        /// <param name="nodeId" type="object">             Identifier for the node. </param>
        /// <param name="errMessage" type="String">         Message describing the error. </param>
        /// <param name="severity" type="EventSeverity">    The severity. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void OnSystemEvent(object nodeId, String errMessage, EventSeverity severity)
        {
            var temp = SystemEvent;
            if (temp != null)
            {
                SystemEventArgs e = new SystemEventArgs();
                if (nodeId != null)
                    e.sourceNode = (NodeId)nodeId;
                else
                    e.sourceNode = new NodeId(DriverName, NamespaceIndex);

                e.sourceName = String.Format("{0} {1}", Properties.Resources.LoggerSource, DriverName/*DriverInfo.GetDriverName()*/);
                e.EventName = errMessage;
                e.severity = severity;
                e.time = DateTime.UtcNow;
                e.eventtype = ObjectTypeIds.DeviceFailureEventType;
                temp(this, e);
            }
        }

        /// <summary>   The lock on state Changed . </summary>
        protected Object lockStateChanged = new Object();

        /// <summary>   Event queue for all listeners interested in change of drivers state. </summary>
        public event EventHandler<ComunicationStateArgs> StateChanged;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the state change event action. </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void OnStateChanged( ComunicationState newState)
        {
            bool changeState = false;
            ComunicationState oldState = ComunicationState.Fault;

            lock (lockStateChanged)
            {
                if (newState != ComunicationState.Fault)
                {
                    var stat = (from s in GetStations().AsParallel()
                                where s.InErrorState == true
                                select s).FirstOrDefault();
                    if(stat == null)
                    {
                        var chan = (from c in GetChannels().AsParallel()
                                    where c.ConsecutiveCommErrors != 0
                                    select c).FirstOrDefault();
                        if (chan != null)
                            newState = ComunicationState.Fault;
                    }
                    else
                    {
                        newState = ComunicationState.Fault;
                    }
                }

                if (newState != DriverState)
                {
                    oldState = DriverState;
                    DriverState = newState;
                    changeState = true;
                }
            }

            if (changeState)
            {
                CallStateChangedSubcriber(oldState, newState);
            }
        }

        private void CallStateChangedSubcriber(ComunicationState oldState, ComunicationState newState) 
        {
            var temp = StateChanged;
            if (temp != null) {
                ComunicationStateArgs es = new ComunicationStateArgs();
                es.OldState = oldState;
                es.NewState = newState;
                temp(this, es);
            }
        }

        /// <summary>   Event queue for all listeners interested in audi events. </summary>
        public event EventHandler<AuditEventArgs> AudiEvent;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Raises the audit event. </summary>
        ///
        /// <param name="e" type="AuditEventArgs">  Event information to send to registered event
        ///                                         handlers. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void OnAuditEvent(AuditEventArgs e)
        {
            var temp = AudiEvent;
            if (temp != null)
                temp(this, e);
        }

        /// <summary>   Event queue for all listeners interested in TagPrototypeQuery events. </summary>
        public event EventHandler<TagPrototypeArgs> TagPrototypeQuery;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the tag prototype query action. </summary>
        ///
        /// <param name="sourceNode" type="NodeId">                 Source node. </param>
        /// <param name="listTags" type="ref List<TagDefinition>">  [in,out] The list tags. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void OnTagPrototypeQuery(NodeId sourceNode, ref List<TagDefinition> listTags)
        {
            var temp = TagPrototypeQuery;
            if (temp != null)
            {
                TagPrototypeArgs e = new TagPrototypeArgs();
                e.driverName = DriverName;
                e.sourceNode = sourceNode;
                temp(this, e);
                if (e.listTags != null)
                    listTags = e.listTags.ToList();
            }

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets diagnostic folder. </summary>
        ///
        /// <param name="RootDriversGuid" type="string">    Unique identifier for the root drivers. </param>
        /// <param name="NamespaceIndex" type="ushort">     Zero-based index of the namespace. </param>
        ///
        /// <returns>   An uint. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint SetDiagnosticFolder(string RootDriversGuid, ushort NamespaceIndex)
        {
            _RootDriversGuid = RootDriversGuid;
            _NamespaceIndex = NamespaceIndex;
            return StatusCodes.Good;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets channels names. </summary>
        ///
        /// <returns>   The channels names. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public List<string> GetChannelsNames()
        {
            List<string> Names = new List<string>();
            foreach (var channel in GetChannels())
            {
                Names.Add(channel.Name);
            }
            return Names;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets stations names. </summary>
        ///
        /// <returns>   The stations names. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public List<string> GetStationsNames()
        {
            List<string> Names = new List<string>();
            foreach (var station in GetStations())
            {
                Names.Add(station.Name);
            }
            return Names;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets driver name. </summary>
        ///
        /// <returns>   The driver name. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string GetDriverName()
        {
            return DriverName;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets internal name. </summary>
        ///
        /// <returns>   The internal name. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string GetInternalName()
        {
            return _InternalDriverName;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets nodeid to observe. </summary>
        ///
        /// <returns>   The nodeid to observe. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public List<NodeId> GetObservingNodes()
        {
            List<NodeId> nodeIDs = new List<NodeId>();
            // Ask to each station the node IDs of the variables to be observed
            List<Station> stationList = GetStations();
            foreach (var station in stationList)
            {
                List<NodeId> stationNodeIDs = station.GetObservingNodes();
                if(stationNodeIDs.Count > 0)
                {
                    nodeIDs.AddRange(stationNodeIDs);
                    foreach(var node in stationNodeIDs)
                    {
                        if(!ObservedTagToStationsMap.Keys.Contains(node))
                        {
                            ObservedTagToStationsMap[node] = new List<Station>();
                        }
                        ObservedTagToStationsMap[node].Add(station);
                    }
                }
            }
            foreach (var channel in GetChannels())
            {
                List<NodeId> channelNodeIDs = channel.GetObservingNodes();
                if (channelNodeIDs.Count > 0)
                {
                    nodeIDs.AddRange(channelNodeIDs);
                    foreach (var node in channelNodeIDs)
                    {
                        if (!ObservedTagToChannelsMap.Keys.Contains(node))
                        {
                            ObservedTagToChannelsMap[node] = new List<Channel>();
                        }
                        ObservedTagToChannelsMap[node].Add(channel);
                    }
                }
            }
            // Add to the list the Node ID of the State\Command Variable of the channel
            if (driverStateCommandVariable.hasBeenSet == true)
            {
                nodeIDs.Add(driverStateCommandVariable.varNodeId);
            }
            return nodeIDs;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Updates nodeid values of observed nodeid. </summary>
        ///
        /// <returns>   Updates nodeid values of observed nodeid. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void UpdateObservedTag(NodeId node, DataValue value)
        {
            // Get the station that manages the node ID and pass to it the updated value of the variable
            if(ObservedTagToStationsMap.Keys.Contains(node) == true)
            {
                List<Station> stationList = ObservedTagToStationsMap[node];
                foreach(var station in stationList)
                {
                    station.UpdateObservedTag(node, value);
                }
            }
            // Get the channel that manages the node ID and pass to it the updated value of the variable
            if (ObservedTagToChannelsMap.Keys.Contains(node) == true)
            {
                foreach (var channel in ObservedTagToChannelsMap[node])
                {
                    channel.UpdateObservedTag(node, value);
                }
            }
            // Update the value of the state/command variable
            if ((driverStateCommandVariable.hasBeenSet == true) && (node == driverStateCommandVariable.varNodeId))
            {
                ManageUpdatedValueForTheStateCommandVariable(value);
            }
        }
        public void ManageUpdatedValueForTheStateCommandVariable(DataValue value)
        {
            bool changeBitSavedValue = false;
            bool changeBitCanBeManaged = false;
            if (!bPendingChange && driverStateCommandVariable.GetStateCommandVariableBit(ref changeBitSavedValue, (UInt16)DriverVariableBits.ChangeTagsSettings) == true)
            {
                changeBitCanBeManaged = true;
            }

            driverStateCommandVariable.varValue = value;

            if (changeBitCanBeManaged)
            {
                bool suspendBitNewValue = false;
                if (driverStateCommandVariable.GetStateCommandVariableBit(ref suspendBitNewValue, (UInt16)DriverVariableBits.ChangeTagsSettings) == true)
                {
                    if (changeBitSavedValue != suspendBitNewValue && suspendBitNewValue )
                    {
                        bPendingChange = true;
                        SmartThreadPool.QueueWorkItem(() =>
                        {
                            bool ErrorLoadTagsSettings = true;
                            Dictionary<NodeId, string> TagChangeSettingMap = new Dictionary<NodeId, string>();
                            if (LoadChangeSettings(TagChangeSettingMap))
                            {
                                if (checkChangeFile(TagChangeSettingMap))
                                {
                                    if (Suspend())
                                    {
                                        RemoveAllJobs();
                                        List<Tag> NewTags = new List<Tag>();
                                        foreach (var tag in AllDrivertags)
                                        {
                                            if (TagChangeSettingMap.ContainsKey(tag.TagNode.NodeId))
                                            {
                                                TagDefinition newTag = tag.TagNode;
                                                newTag.DynamicSettings = TagChangeSettingMap[tag.TagNode.NodeId];
                                                NewTags.Add(CreateTag(newTag));
                                            }
                                            else
                                                NewTags.Add(tag);
                                        }
                                        AllDrivertags = NewTags;
                                        AllDrivertags.Sort(CompareTagByDynamic);

                                        var validtags = ParseDynamicTags(AllDrivertags);
                                        if (validtags > 0 && !bRunning)
                                            SaveDynamicJobs();
                                        Startup();
                                        if (StatisticsData != null)
                                        {
                                            StatisticsData.Update(StatisticSetting.NodeDataNames.TotalTags.ToString(), validtags);

                                            long totalJobs = 0;
                                            foreach (var stationname in Stations.Keys)
                                                totalJobs += Stations[stationname].ListWholeJob.Count();
                                            StatisticsData.Update(StatisticSetting.NodeDataNames.TotalJobs.ToString(), totalJobs);
                                        }
                                        ErrorLoadTagsSettings = false;
                                    }
                                    else
                                        OnSystemEvent(null, string.Format(Properties.Resources.SuspendForChangeTagFailed, DriverName), EventSeverity.Medium);
                                }
                                else
                                    OnSystemEvent(null, string.Format(Properties.Resources.LoadedTagDontChange, DriverName), EventSeverity.Medium);
                            }
                            else
                                OnSystemEvent(null, string.Format(Properties.Resources.DontLoadChangeTag, DriverName), EventSeverity.Medium);

                            driverStateCommandVariable.SetStateCommandVariableBit(false, (UInt16)DriverVariableBits.ChangeTagsSettings, this);
                            driverStateCommandVariable.SetStateCommandVariableBit(ErrorLoadTagsSettings, (UInt16)DriverVariableBits.ErrorLoadTagsSettings, this);
                            bPendingChange = false;
                        });
                    }
                }
            }
        }
        public bool checkChangeFile(Dictionary<NodeId, string> TagChangeSettingMap)
        {
            foreach (var tag in AllDrivertags)
            {
                if (TagChangeSettingMap.ContainsKey(tag.TagNode.NodeId))
                    if (tag.TagNode.DynamicSettings != TagChangeSettingMap[tag.TagNode.NodeId])
                        return true;
            }

            return false;
        }
        public void RemoveAllJobs()
        {
            List<Tag> AllTags = new List<Tag>();
            foreach (var station in Stations.Values)
            {
                station.RemoveAllJobs();
            }
            TagToStationMap.Clear();
        }
#endregion

#region Abstract Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Driver base load settings. </summary>
        ///
        /// <param name="idl" type="IDataLayer">    The idl. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public abstract bool LoadDriverSettings(IDataLayer idl);
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Driver base save settings. </summary>
        ///
        /// <param name="idl" type="IDataLayer">    The idl. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public abstract void SaveDriverSettings(IDataLayer idl);
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Creates a channel. </summary>
        ///
        /// <param name="settings"> . </param>
        ///
        /// <returns>   The new channel. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public abstract Channel CreateChannel(ChannelSettings settings);
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Creates a station. </summary>
        ///
        /// <param name="settings"> . </param>
        ///
        /// <returns>   The new station. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public abstract Station CreateStation(StationSettings settings);
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Creates a tag. </summary>
        ///
        /// <param name="tagtoAdd" type="TagDefinition">    The tagto add. </param>
        ///
        /// <returns>   The new tag. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public abstract Tag CreateTag(TagDefinition tagtoAdd);
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Creates a tag. </summary>
        ///
        /// <param name="tagtoAdd" type="TagDefinition">    The tagto add. </param>
        /// <param name="byteoffset" type="uint">           The byteoffset. </param>
        /// <param name="bitoffset" type="uint">            The bitoffset. </param>
        ///
        /// <returns>   The new tag. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public abstract Tag CreateTag(TagDefinition tagtoAdd, uint byteoffset, uint bitoffset);
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Creates tag settings. </summary>
        ///
        /// <param name="session" type="Session">   The session. </param>
        /// <param name="tag" type="Tag">           The tag. </param>
        ///
        /// <returns>   The new tag settings. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public abstract TagSettings CreateTagSettings(Session session, Tag tag);

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Driver base load change settings. </summary>
        ///
        /// <param name="idl" type="IDataLayer">    The idl. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public abstract List<ChangeTag> LoadChangeSettings(IDataLayer idl);

#endregion

#region Virtual Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Driver base load settings. </summary>
        ///
        /// <param name="settings"> . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void LoadDriverSettings(DriverSettings settings)
        {
            _Enable = settings.Enable;
            _AggregationThreshold = settings.AggregationThreshold;
            _AggregationLimit = settings.AggregationLimit;
            _SyncAtStartup = settings.SyncAtStartup;
			_EnableStatistics = settings.EnableStatistics;
            _WriteAsync = settings.WriteAsync.Value;
            _StateCommandTag = settings.StateCommandTag;
            if (_StateCommandTag != null && !NodeId.IsNull(_StateCommandTag.NodeId))
            {
                driverStateCommandVariable = new StateCommandVariable(_StateCommandTag.Name, _StateCommandTag.NodeId.ToString());
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Driver base save settings. </summary>
        ///
        /// <param name="settings"> . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void SaveDriverSettings(DriverSettings settings)
        {
            settings.Enable = Enable;
            settings.AggregationThreshold = AggregationThreshold;
            settings.AggregationLimit = AggregationLimit;
            settings.SyncAtStartup = SyncAtStartup;
            settings.WriteAsync = WriteAsync;
			settings.EnableStatistics = EnableStatistics;
            settings.StateCommandTag = StateCommandTag;
        }

        /// <summary>   Loads default settings. </summary>
        public virtual void LoadDefaultSettings()
        {
            _Enable = true;
            _AggregationThreshold = 5;
            _AggregationLimit = 0;
            _SyncAtStartup = false;
            _WriteAsync = false;
            _EnableStatistics = false;
            _StateCommandTag = null;
        }

        /*
         * redifine in driver to specify errors description
         */
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets driver error information. </summary>
        ///
        /// <param name="errorcode" type="int">     The errorcode. </param>
        /// <param name="quality" type="out uint">  [out] The quality. </param>
        /// <param name="error" type="out string">  [out] The error. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void GetDriverErrorInfo(int errorcode, out uint quality, out string error)
        {
            switch ((DriverErrorCodes)errorcode)
            {
                case DriverErrorCodes.ErrorNoError:
                    quality = StatusCodes.Good;
                    error = Properties.Resources.ErrorNoError;
                    break;
                case DriverErrorCodes.ErrorTimeOut:
                    quality = StatusCodes.BadTimeout;
                    error = Properties.Resources.ErrorTimeOut;
                    break;
                case DriverErrorCodes.ErrorFrameError:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorFrameError;
                    break;
                case DriverErrorCodes.ErrorRXOverError:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorRXOverError;
                    break;
                case DriverErrorCodes.ErrorRXParityError:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorRXParityError;
                    break;
                case DriverErrorCodes.ErrorTXFullError:
                    quality = StatusCodes.BadCommunicationError;
                    error = Properties.Resources.ErrorTXFullError;
                    break;
                case DriverErrorCodes.ErrorParsingAnswer:
                    quality = StatusCodes.BadDecodingError;
                    error = Properties.Resources.ErrorParsingAnswer;
                    break;
                default:
                    quality = StatusCodes.BadUnexpectedError;
                    error = Properties.Resources.ErrorUnexpected;
                    break;
            }
        }

#endregion

#region Internal Methods
        
        /// <summary>   Builds list of channels. </summary>
        private void BuildListOfChannels()
        {
            IDataLayer idl = GetDriverDataLayer();
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                UpdateDriverSchema(ufw);
                var listsettings = (from drvsettings in new XPQuery<DriverSettings>(ufw)/*.AsParallel()*/
                                    select drvsettings).ToList();

                var channelsettings = (from drvsettings in listsettings
                                       where drvsettings.ClassInfo.AssemblyName == this.AssemblyName
                                       select drvsettings.ChannelSettings).FirstOrDefault();

                foreach (var settings in channelsettings)
                {
                    var channel = CreateChannel(settings);
                    Channels[settings.Name] = channel;
                    if (!Channels[settings.Name].Init())
                        OnSystemEvent(/*ObjectIds.Server*/null, String.Format(Properties.Resources.StationUninitailized, channel.Name), EventSeverity.High);
                }
            }
        }

        /// <summary>   Builds list of stations. </summary>
        private void BuildListOfStations()
        {
            IDataLayer idl = GetDriverDataLayer();
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var listsettings = (from drvsettings in new XPQuery<DriverSettings>(ufw)/*.AsParallel()*/
                                    select drvsettings).ToList();

                var stationsettings = (from drvsettings in listsettings
                                       where drvsettings.ClassInfo.AssemblyName == this.AssemblyName
                                       select drvsettings.StationSettings).FirstOrDefault();

                foreach (var settings in stationsettings)
                {
                    if (settings.Channel == null || !Channels.ContainsKey(settings.Channel))
                        continue;
                    var station = CreateStation(settings);
                    Stations[settings.Name] = station;
                    Channel channel = null;
                    if (Channels.ContainsKey(settings.Channel))
                        channel = Channels[settings.Channel];
                    if (!Stations[settings.Name].Init(channel))
                        OnSystemEvent(/*ObjectIds.Server*/null, String.Format(Properties.Resources.StationUninitailized, station.Name), EventSeverity.High);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Determines if we can startup all stations. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool StartupAllStations()
        {
            bool bRet = true;
            //Parallel.ForEach(Stations.Values, station =>
            foreach (var station in Stations.Values)
            {
                if (!station.Startup())
                {
                    OnSystemEvent(/*ObjectIds.Server*/null, String.Format(Properties.Resources.StationStartupFailed, station.Name), EventSeverity.High);
                    bRet = false;
                }
            };
            return bRet;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Determines if we can suspend all stations. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool SuspendAllStations()
        {
            bool bRet = true;
            //Parallel.ForEach(Stations.Values, station =>
            foreach (var station in Stations.Values)
            {
                if (!station.Suspend())
                {
                    OnSystemEvent(/*ObjectIds.Server*/null, String.Format(Properties.Resources.StationSuspendFailed, station.Name), EventSeverity.High);
                    bRet = false;
                }
            };
            return bRet;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Determines if we can terminate all stations. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool TerminateAllStations()
        {
            bool bRet = true;
            //Parallel.ForEach(Stations.Values, station =>
            foreach (var station in Stations.Values)
            {
                if (!station.Terminate())
                {
                    OnSystemEvent(/*ObjectIds.Server*/null, String.Format(Properties.Resources.StationTerminateFailed, station.Name), EventSeverity.High);
                    bRet = false;
                }
            };
            return bRet;
        }

#if !NET_STANDARD
        public static readonly ILog log = log4net.LogManager.GetLogger(Properties.Resources.DriversLog);
#else
        public static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.DriversLog);
#endif

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Parse dynamic tags. </summary>
        ///
        /// <param name="tags" type="IList<Tag>">   The tags. </param>
        ///
        /// <returns>   A long. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private long ParseDynamicTags(IList<Tag> tags)
        {
            // Added just for debugging
            DateTime dtime0 = DateTime.UtcNow;

            long validtags = 0;
            if (tags.Count > 0)
            {
                // get station's list from tag's list (valid and not)
                var stationList = from tag in tags group tag by tag.DynSettings.StationName into newGroup select newGroup.Key;

                foreach (var stationName in stationList)
                {
                    // get only valid tags
                    var tagsStation = (from tag in tags/*.AsParallel()*/
                                       where tag.bIsValid && tag.DynSettings.StationName == stationName
                                       select tag).ToList();

                    if (Stations.ContainsKey(stationName))
                    {
                        if (Stations[stationName].ParseDynamicTags(tagsStation))
                        {
                            //Update Dictionary of Tag-Station link
                            foreach (var tag in tagsStation)
                                TagToStationMap[tag.TagNode.NodeId] = Stations[stationName];

                            validtags += tagsStation.Count;
                        }
                    }
                    else
                    {
                        // invalid station
                        OnSystemEvent(null, string.Format(Properties.Resources.ErrorStationNameNotExist, stationName, DriverName), EventSeverity.Low);
                        // force station's tags to invalid state
                        Parallel.ForEach(tagsStation, tag =>
                        {
                            tag.bIsValid = false;
                            tag.InvalidReason = string.Format(Properties.Resources.ErrorTagStationNameNotExist, stationName);
                    });
                    }
                }
            }

            // force all invalid tags to BadConfigurationError quality
            var tagsInValid = (from tag in tags/*.AsParallel()*/
                               where !tag.bIsValid
                               select tag).ToList();
            foreach (var tag in tagsInValid)
            {
                OnTagChanged(tag.TagNode.NodeId, new DataValue(StatusCodes.BadConfigurationError));
                log.Error(string.Format(Properties.Resources.InvalidTagDefinition, DriverName, tag.TagNode.NodeId.Identifier, tag.InvalidReason));
            }

            // Added just for debugging
            double dtime = (DateTime.UtcNow - dtime0).TotalMilliseconds;
            System.Diagnostics.Trace.TraceInformation("ParseDynamicTags {0} {1} ms", dtime0, dtime);

            return validtags;
        }

        /// <summary>   Executes all jobs at startup operation. </summary>
        private void ExecuteAllJobsAtStartup()
        {
            //steve 240811
            foreach (var station in Stations)
            {
                var localList = station.Value.ListWholeJob.AsParallel().Where(job => ((job.Type == LinkType.Input) || (job.Type == LinkType.InputOutput))).ToList();
                localList.ForEach(job => job.ExecuteFirstTime = true);
            }
        }

        private bool IsBelongFromParent(bool isProtected, Guid id)
        {
            if (String.IsNullOrEmpty(strConnectionString))
                return true;

            ConnectionStringParser helper = new ConnectionStringParser(strConnectionString);
            string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);

            if (providerType != InMemoryDataStore.XpoProviderTypeString)
                return true;

            string filebase = helper.GetPartByName(DataSourceHeader);
            if (!File.Exists(filebase))
                return true;

            if (isProtected && Utilities.IO.FileSystem.IsXmlFile(filebase))
                return false;

            IDataLayer idl = GetDriverDataLayer();
            using (UnitOfWork ufw = new UnitOfWork(idl))
            {
                var guid = XpoHelpers.XpoHelper.GetProtectionCode(ufw);
                return guid == Guid.Empty || guid == id;
            }
        }
#endregion

#region Properties

        /// <summary>   Name of the driver. </summary>
        readonly String _DriverName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the name of the driver. </summary>
        ///
        /// <value> The name of the driver. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public String DriverName
        {
            get { return _DriverName; }
        }

        /// <summary>   Assembly Name of the driver. </summary>
        readonly String _AssemblyName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the assembly name of the driver. </summary>
        ///
        /// <value> The assembly name of the driver. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public String AssemblyName
        {
            get { return _AssemblyName; }
        }

        /// <summary>   true to enable, false to disable the driver. </summary>
        private bool _Enable;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets a value indicating whether the driver is enabled. </summary>
        ///
        /// <value> true if enable driver communication, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool Enable
        {
            get { return _Enable; }
        }

        /// <summary>   The aggregation threshold. </summary>
        private uint _AggregationThreshold;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the aggregation threshold. </summary>
        ///
        /// <value> The aggregation threshold. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint AggregationThreshold
        {
        	get	{ return _AggregationThreshold; }
        }

        /// <summary>   The aggregation limit. </summary>
        private uint _AggregationLimit;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the aggregation limit. </summary>
        ///
        /// <value> The aggregation limit. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint AggregationLimit
        {
        	get	{ return _AggregationLimit; }
        }
        
        /// <summary>   true to synchronise at startup. </summary>
        private bool _SyncAtStartup;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets a value indicating whether the synchronise at startup. </summary>
        ///
        /// <value> true if synchronise at startup, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool SyncAtStartup
        {
        	get	{ return _SyncAtStartup; }
        }
        /// <summary>   true to synchronise at startup. </summary>
        private bool _WriteAsync;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets a value indicating whether Write Job are executed Asynchronously. </summary>
        ///
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool WriteAsync
        {
            get { return _WriteAsync; }
        }
        
        /// <summary>   true to enable, false to disable the statistics. </summary>
        private bool _EnableStatistics;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets a value indicating whether the statistics is enabled. </summary>
        ///
        /// <value> true if enable statistics, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool EnableStatistics
        {
            get { return _EnableStatistics; }
        }

        /// <summary>   Unique identifier for the root drivers. </summary>
        private string _RootDriversGuid;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets a unique identifier of the root drivers. </summary>
        ///
        /// <value> Unique identifier of the root drivers. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string RootDriversGuid
        {
            get { return _RootDriversGuid; }
            set { _RootDriversGuid = value; }
        }

        /// <summary>   Zero-based index of the namespace. </summary>
        private ushort _NamespaceIndex;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the zero-based index of the namespace. </summary>
        ///
        /// <value> The namespace index. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public ushort NamespaceIndex
        {
            get { return _NamespaceIndex; }
            set { _NamespaceIndex = value; }
        }

        string strConnectionString;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary> Gets the connection string used for initialize the driver. </summary>
        ///
        /// <value> The Connection string </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string StrConnectionString
        {
            get
            {
                return strConnectionString;
            }
        }        

        /// <summary>   Name of the internal driver. </summary>
        protected string _InternalDriverName = string.Empty;

        /// <summary>   The smart thread pool. </summary>
        SmartThreadPool smartThreadPool;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the smart thread pool. </summary>
        ///
        /// <value> The smart thread pool. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        internal SmartThreadPool SmartThreadPool
        {
            get
            {
                return smartThreadPool;
            }
        }

        /// <summary>  The State-Command variable of the channel. </summary>
        private UFUAModel.TagEntityReference _StateCommandTag;
        /// <summary>
        /// Gets or sets the the State-Command variable of the channel.
        /// </summary>
        public UFUAModel.TagEntityReference StateCommandTag
        {
            get
            {
                return _StateCommandTag;
            }
            set
            {
                _StateCommandTag = value;
            }
        }

        /// <summary>  The Driver State variable of the Driver. </summary>
        private ComunicationState _DriverState ;
        /// <summary>
        /// Gets or sets the  Driver State variable of the Driver.
        /// </summary>
        public ComunicationState DriverState {
            get {
                return _DriverState;
            }
            set {
                _DriverState = value;
            }
        }

#endregion

#region Persistence Data

        /// <summary>   The data source header. </summary>
        private static readonly String DataSourceHeader = "data source";
        /// <summary>   The catalog source header. </summary>
        private static readonly String CatalogSourceHeader = "initial catalog";
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets connection string. </summary>
        ///
        /// <exception cref="ArgumentNullException">    Thrown when one or more required arguments are
        ///                                             null. </exception>
        ///
        /// <param name="activeconnection" type="String">   The activeconnection. </param>
        /// <param name="folder" type="String">             Pathname of the folder. </param>
        /// <param name="baseName" type="String">           Name of the base. </param>
        /// <param name="xmlExt" type="String">             Extent of the XML. </param>
        /// <param name="runningFilePath" type="String">    (Optional) full pathname of the running file. </param>
        ///
        /// <returns>   The connection string. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static String GetConnectionString(String activeconnection, String folder, String baseName, String xmlExt, String runningFilePath = null)
        {
            if (String.IsNullOrEmpty(activeconnection) || String.IsNullOrEmpty(baseName))
                throw new ArgumentNullException("Parameters cannot be null or empty");

            folder = folder ?? Properties.Settings.Default.TypeLabel;
            xmlExt = xmlExt ?? Properties.Settings.Default.DefaultDrvFileExt;
            string regex = string.Format("[{0}]", Regex.Escape(new string(Path.GetInvalidFileNameChars())));
            baseName = Regex.Replace(baseName, regex, ".");

            ConnectionStringParser helper = new ConnectionStringParser(activeconnection);
            string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);
            if (providerType == InMemoryDataStore.XpoProviderTypeString)
            {
                string ds = helper.GetPartByName(DataSourceHeader);
                ds = ds.Replace('/', '\\');
                int index = ds.LastIndexOf('\\');
                if (index != -1)
                {
                    var path = String.Format("{0}\\{1}", ds.Substring(0, index), folder);
#if !NET_STANDARD
                    Directory.CreateDirectory(path);
#else
                    Directory.CreateDirectory(path.Replace('\\', Path.DirectorySeparatorChar));
#endif
                    if (!String.IsNullOrEmpty(runningFilePath))
                    {
#if !NET_STANDARD
                        var fileBase = String.Format("{0}\\{1}{2}", path, baseName, xmlExt);
#else
                        var fileBase = String.Format("{0}{1}{2}{3}", path.Replace('\\', Path.DirectorySeparatorChar), Path.DirectorySeparatorChar, baseName, xmlExt);
#endif
                        var serverFile = runningFilePath;

                        try
                        {
                            serverFile = System.IO.Path.GetTempFileName();
                            System.IO.File.Copy(fileBase, serverFile, true);
                        }
                        catch 
                        {
                            serverFile = fileBase;
                        }

                        ds = serverFile;
                    }
                    else
#if !NET_STANDARD
                        ds = String.Format("{0}\\{1}{2}", path, baseName, xmlExt);
#else
                        ds = String.Format("{0}{1}{2}{3}", path.Replace('\\', Path.DirectorySeparatorChar), Path.DirectorySeparatorChar, baseName, xmlExt);
#endif
                }

                helper.UpdatePartByName(DataSourceHeader, ds);

                return helper.GetConnectionString();
            }/*
            else if (providerType == AccessConnectionProvider.XpoProviderTypeString)
            {
                string ds = helper.GetPartByName(DataSourceHeader);
                int index = ds.LastIndexOf('\\');
                if (index != -1)
                {
                    var path = String.Format("{0}\\{1}", ds.Substring(0, index), folder);
                    Directory.CreateDirectory(path);
                    ds = String.Format("{0}={1}\\{2}{3}.mdb", DataSourceHeader, path, baseName, xmlExt);
                }

                helper.RemovePartByName(DataSourceHeader);

                return String.Format("{0}{1}", helper.GetConnectionString(), ds);
            }
            else if (providerType == MSSqlConnectionProvider.XpoProviderTypeString)
            {
                string ds = helper.GetPartByName(CatalogSourceHeader);
                if (!String.IsNullOrEmpty(ds))
                    ds = String.Format("{0}={3}_{1}{2}", CatalogSourceHeader, baseName, xmlExt, helper.GetPartByName(CatalogSourceHeader));

                helper.RemovePartByName(CatalogSourceHeader);

                return String.Format("{0}{1}", helper.GetConnectionString(), ds);
            }
            else
                return helper.GetConnectionString();*/
            else
                return activeconnection;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   get driver data layer. </summary>
        /// <returns>   driver data layer. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private IDataLayer GetDriverDataLayer(string strConnectionString)
        {
            try
            {
                string conn = GetConnectionString(strConnectionString, Properties.Settings.Default.TypeLabel,
                    DriverName, Properties.Settings.Default.DefaultDrvFileExt);
#if NET_STANDARD
                conn = conn.Replace('\\',Path.DirectorySeparatorChar);
#endif
                return GetSpecificDataLayer(conn);
            }
            catch (Exception ex)
            { }

            return null;
        }

        /// <summary>   The driver data layer. </summary>
        IDataLayer drvDataLayer;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   get driver data layer. </summary>
        ///
        /// <param name="running" type="bool">  (Optional) true to running. </param>
        ///
        /// <returns>   driver data layer. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private IDataLayer GetDriverDataLayer(bool running = false)
        {
            if (drvDataLayer == null)
            {
                try
                { 
                    var runningFilePath = System.IO.Path.GetTempFileName();
                    string conn = GetConnectionString(strConnectionString, Properties.Settings.Default.TypeLabel,
                        DriverName, Properties.Settings.Default.DefaultDrvFileExt, runningFilePath);
//davidep
#if NET_STANDARD
                    conn = conn.Replace('\\',Path.DirectorySeparatorChar);
#endif
                    listFileToDelete.Add(runningFilePath);
                    drvDataLayer = GetSpecificDataLayer(conn); //XpoDefault.GetDataLayer(conn, AutoCreateOption.DatabaseAndSchema);
                }
                catch(Exception ex)
                {
                    drvDataLayer = null;
                    OnSystemEvent(null, ex.Message, EventSeverity.Medium);
                }
            }

            return drvDataLayer;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   get driver data layer. </summary>
        ///
        /// <param name="strConnectionString">  Connection String. </param>
        /// <param name="drivername">           driver name. </param>
        /// <param name="xml">                  (Optional) is xml (default = true) </param>
        ///
        /// <returns>   driver data layer. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static IDataLayer GetDriverDataLayer(string strConnectionString, string drivername, bool xml = true)
        {
            string filebase;
            InMemoryDataStore InMemory;
            return GetDriverDataLayer(strConnectionString, drivername, out filebase, out InMemory, xml);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   get driver data layer. </summary>
        /// 
        /// <param name="strConnectionString"></param>
        /// <param name="drivername"></param>
        /// <param name="filebase"></param>
        /// <param name="InMemory"></param>
        /// <param name="xml"></param>
        ///
        /// <returns>   driver data layer. </returns>
        public static IDataLayer GetDriverDataLayer(string strConnectionString, string drivername, out string filebase, out InMemoryDataStore InMemory, bool xml = true)
        {
            IDataLayer drvDLayer = null;
            filebase = string.Empty;
            InMemory = null;

            // force for compatibilty with drivers compiled with old "interface"
            xml = true;

            try
            {
                string connect = GetConnectionString(strConnectionString, null, drivername, null);
                drvDLayer = GetSpecificDataLayer(connect, out filebase, out InMemory, out bool targetIsFile, xml);
            }
            catch(Exception ex)
            {
                drvDLayer = null;
#if !NET_STANDARD
                if (Environment.UserInteractive)
                { 
                    MessageBox.Show(string.Format(Properties.Resources.ErrorOpeningDocument, strConnectionString, ex.Message));
                }
#endif
            }

            return drvDLayer;
        }
                
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets specific data layer. </summary>
        ///
        /// <param name="conn"> Connection String. </param>
        /// <param name="xml">  (Optional) is xml (default = true) </param>
        ///
        /// <returns>   The specific data layer. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        static IDataLayer GetSpecificDataLayer(string conn, bool xml = true)
        {
            //IDataLayer dl = null;
            //ConnectionStringParser helper = new ConnectionStringParser(conn);
            //string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);

            //if (providerType != InMemoryDataStore.XpoProviderTypeString)
            //{
            //    dl = XpoDefault.GetDataLayer(conn, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
            //}
            //else if(xml)
            //{
            //    string filebase = helper.GetPartByName(DataSourceHeader);
            //    InMemoryDataStore InMemory = GetDataStore(filebase);
            //    dl = new SimpleDataLayer(InMemory);
            //}
            //return dl;            
            return GetSpecificDataLayer(conn, out string filebase, out InMemoryDataStore InMemory, out bool targetIsFile, xml);
        }        

        public static IDataLayer GetSpecificDataLayer(string conn, out string filebase, out InMemoryDataStore InMemory, out bool targetIsFile, bool xml = true)
        {        
            IDataLayer dl = null;
            InMemory = null;
            filebase = string.Empty;
            ConnectionStringParser helper = new ConnectionStringParser(conn);
            string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);

            targetIsFile = false;

            if (providerType != InMemoryDataStore.XpoProviderTypeString)
            {
                dl = XpoDefault.GetDataLayer(conn, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
                targetIsFile = false;
            }
            else if (xml)
            {
                filebase = XpoHelpers.XpoHelper.GetDataSourceFilePath(conn);
                InMemory = CommunicationDriver.GetDataStore(filebase);
                if (!string.IsNullOrWhiteSpace(filebase) && InMemory != null)
                {
                    var dict = new DevExpress.Xpo.Metadata.ReflectionDictionary();
                    dict.ClassInfoChanged += Dict_ClassInfoChanged;
                    dict.GetDataStoreSchema(typeof(DriverSettings).Assembly);
                    dl = new SimpleDataLayer(dict, InMemory);
                    targetIsFile = true;
                }
            }

            return dl;
        }

        static void Dict_ClassInfoChanged(object sender, ClassInfoEventArgs e)
        {
            var ci = e.ClassInfo;
            if (ci.IsPersistent && ci.TableMapType == MapInheritanceType.OwnTable && !ci.AssemblyName.StartsWith("DevExpress.Xpo"))
            {
                var dict = (ReflectionDictionary)sender;
                dict.ClassInfoChanged -= Dict_ClassInfoChanged;
                ci.RemoveAttribute(typeof(PersistentAttribute));
                ci.AddAttribute(new PersistentAttribute(ci.ClassType.Name));
                dict.ClassInfoChanged += Dict_ClassInfoChanged;
            }
        }        

        public static void UpdateDriverSchema(Session s)
        {
            List<Type> vTypes = new List<Type>();

            foreach (var t in s.TypesManager.AllTypes.Keys)
            {
                vTypes.Add(t.ClassType);
            }
            if (vTypes.Count > 0)
                s.UpdateSchema(vTypes.ToArray());
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   get driver setting file. </summary>
        ///
        /// <param name="conn">         Connection String. </param>
        /// <param name="drivername">   driver name. </param>
        ///
        /// <returns>   The file base. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string GetFileBase(string conn, string drivername)
        {
            string connect = GetConnectionString(conn, null, drivername, null);

            return XpoHelpers.XpoHelper.GetDataSourceFilePath(connect);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Crypt String. </summary>
        ///
        /// <param name="str">  . </param>
        ///
        /// <returns>   A string. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string EncryptString(string str)
        {
            return WPFUtilities.CryptString.CryptString.EncryptString(str); ;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   get file data. </summary>
        ///
        /// <param name="filebase"> setting file. </param>
        ///
        /// <returns>   InMemoryDataStore file data. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static InMemoryDataStore GetDataStore(string filebase)
        {
            InMemoryDataStore InMemory = new InMemoryDataStore(DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema, true);
            if (File.Exists(filebase))
            {
                if (!Utilities.IO.FileSystem.IsXmlFile(filebase))
                {
                    try
                    {
                        var data = WPFUtilities.CryptString.CryptString.DecryptString(File.ReadAllText(filebase));
                        using (var reader = new MemoryStream(Convert.FromBase64String(data)))
                        {
                            var xmlreader = XmlReader.Create(reader);
                            InMemory.ReadXml(xmlreader);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
                else
                    try
                    {
                        InMemory.ReadXml(filebase);
                    }
                    catch (Exception ex)
                    {

                    }
            }
            return InMemory;
        }

        public static string SaveDriverSettings(bool protection, Guid code, UnitOfWork ufw, string fileBase, InMemoryDataStore InMemory)
        {
            string retMess = string.Empty;

            if (protection)
                XpoHelpers.XpoHelper.AddProtectionCode(ufw, code);
            else
                XpoHelpers.XpoHelper.RemoveProtectionCode(ufw);

            try
            {
                ufw.CommitChanges();
                ufw.ReloadChangedObjects();
                ufw.PurgeDeletedObjects();

                ufw.CommitChanges();
            }
            catch (Exception ex)
            {
#if !NET_STANDARD
                if (Environment.UserInteractive)
                    MessageBox.Show(ex.Message);
                else
#endif
                    retMess = ex.Message;
                if (File.Exists(fileBase))
                {
                    try
                    {
                        File.Delete(fileBase);
                    }
                    catch (Exception ex1)
                    {
#if !NET_STANDARD
                        if (Environment.UserInteractive)
                            MessageBox.Show(ex1.Message);
                        else
#endif
                            retMess = ex1.Message;
                    }
                }
                return retMess;
            }

            if (!String.IsNullOrEmpty(fileBase))
            {
                if (File.Exists(fileBase))
                {
                    try
                    {
                        File.Delete(fileBase);
                    }
                    catch (Exception ex)
                    {
#if !NET_STANDARD
                        if (Environment.UserInteractive)
                            MessageBox.Show(ex.Message);
                        else
#endif
                            retMess = ex.Message;
                        return retMess;
                    }
                }

                try
                {
                    if (protection)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            var writer = XmlWriter.Create(memoryStream);
                            InMemory.WriteXml(writer);
                            writer.Flush();//14577
                            writer.Close();
                            var str = Convert.ToBase64String(memoryStream.ToArray());
                            var toWrite = DriverCodeBase.CommunicationDriver.EncryptString(str);
                            File.WriteAllText(fileBase, toWrite);
                        }
                    }
                    else
                        InMemory.WriteXml(fileBase);
                }
                catch (Exception ex)
                {
                    //Assembly a = Assembly.GetAssembly(this.GetType());
                    //System.Windows.MessageBox.Show(ex.Message, a.GetName().Name, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    retMess = ex.Message;
                }
            }
            return retMess;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Copy File. </summary>
        ///
        /// <param name="sourceconn">   source. </param>
        /// <param name="targetconn">   target. </param>
        /// <param name="drivername">   driver name. </param>
        /// <param name="xmlExt">   extension of xml file (optional). </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool CopyFile<T>(string sourceconn, string targetconn, string drivername, string xmlExt = null) where T : XPBaseObject
        {
            string fromConn = GetConnectionString(sourceconn, null, drivername, xmlExt);
            string toConn = GetConnectionString(targetconn, null, drivername, xmlExt);

            using (IDataLayer sourceDL = GetSpecificDataLayer(fromConn))
            {
                string fileBase;
                InMemoryDataStore inMemory;

                using (IDataLayer targetDL = GetSpecificDataLayer(toConn, out fileBase, out inMemory, out bool targetIsFile))
                {
                    using (UnitOfWork sourceufw = new UnitOfWork(sourceDL))
                    {
                        using (UnitOfWork targetufw = new UnitOfWork(targetDL))
                        {
                            var cloneHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(sourceufw, targetufw, true, true, true);

                            var configuration = (from tag in new XPQuery<T>(targetufw)/*.AsParallel()*/ select tag).ToList();
                            configuration.ForEach((o) => o.Delete());

                            configuration = (from tag in new XPQuery<T>(sourceufw)/*.AsParallel()*/ select tag).ToList();
                            foreach (var item in configuration)
                                cloneHelper.Clone(item, false);
                            targetufw.CommitChanges();

                            if (!String.IsNullOrEmpty(fileBase))
                            {
                                bool crypted = false;
                                if (File.Exists(fileBase))
                                {
                                    crypted = !Utilities.IO.FileSystem.IsXmlFile(fileBase);
                                    try
                                    {
                                        File.Delete(fileBase);
                                    }
                                    catch
                                    { }
                                }

                                if (configuration.Count > 0)
                                {
                                    if (crypted)
                                    {
                                        using (var memoryStream = new MemoryStream())
                                        {
                                            var writer = XmlWriter.Create(memoryStream);
                                            inMemory.WriteXml(writer);
                                            writer.Flush();//14577
                                            writer.Close();
                                            var str = Convert.ToBase64String(memoryStream.ToArray());
                                            var toWrite = DriverCodeBase.CommunicationDriver.EncryptString(str);//WPFUtilities.CryptString.CryptString.EncryptString(str);
                                            File.WriteAllText(fileBase, toWrite);
                                        }
                                    }
                                    else
                                        inMemory.WriteXml(fileBase);
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets dynamic jobs data layer. </summary>
        ///
        /// <returns>   The dynamic jobs data layer. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private IDataLayer GetDynamicJobsDataLayer()
        {
            string conn = GetConnectionString(strConnectionString, Properties.Settings.Default.TypeLabel, DriverName/*DriverInfo.GetDriverName()*/, Properties.Settings.Default.DefaultDynFileExt);

            return GetSpecificDataLayer(conn); //XpoDefault.GetDataLayer(conn, dict, AutoCreateOption.DatabaseAndSchema);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets dynamic jobs data layer. </summary>
        ///
        /// <returns>   The dynamic jobs data layer, filebase and InMemory. </returns>
        private IDataLayer GetDynamicJobsDataLayer(out string filebase, out InMemoryDataStore InMemory)
        {
            string conn = GetConnectionString(strConnectionString, Properties.Settings.Default.TypeLabel, DriverName/*DriverInfo.GetDriverName()*/, Properties.Settings.Default.DefaultDynFileExt);

            return GetSpecificDataLayer(conn,out filebase, out InMemory, out bool targetIsFile); //XpoDefault.GetDataLayer(conn, dict, AutoCreateOption.DatabaseAndSchema);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets change settings data layer. </summary>
        ///
        /// <returns>   The change settings data layer, filebase and InMemory. </returns>
        private IDataLayer GetChangeSettingsDataLayer()
        {
            string conn = GetConnectionString(strConnectionString, Properties.Settings.Default.TypeLabel, DriverName/*DriverInfo.GetDriverName()*/, Properties.Settings.Default.DefaultExchFileExt);

            return GetSpecificDataLayer(conn); //XpoDefault.GetDataLayer(conn, dict, AutoCreateOption.DatabaseAndSchema);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets change settings data layer. </summary>
        ///
        /// <returns>   The change settings data layer, filebase and InMemory. </returns>
        private IDataLayer GetChangeSettingsDataLayer(out string filebase, out InMemoryDataStore InMemory)
        {
            string conn = GetConnectionString(strConnectionString, Properties.Settings.Default.TypeLabel, DriverName/*DriverInfo.GetDriverName()*/, Properties.Settings.Default.DefaultExchFileExt);

            return GetSpecificDataLayer(conn, out filebase, out InMemory, out bool targetIsFile); //XpoDefault.GetDataLayer(conn, dict, AutoCreateOption.DatabaseAndSchema);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Loads dynamic jobs. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool LoadDynamicJobs()
        {
            using (IDataLayer idl = GetDynamicJobsDataLayer())
            {
                using (UnitOfWork ufw = new UnitOfWork(idl))
                {

                // TODO rimosso temporaneamete nella attesa del meccanismo di invalidazione del file .dynJobs
                //
                //    var jobs = (from tag in new XPQuery<DynamicJobs>(ufw)/*.AsParallel()*/ select tag.JobSettings).ToList();

                //    if (jobs != null && jobs.Count > 0)
                //    {
                //        bool bBad;
                //        foreach (var jobsettings in jobs[0])
                //        {
                //            bBad = true;

                //            if (Stations.ContainsKey(jobsettings.Station))
                //            {
                //                var job = Stations[jobsettings.Station].CreateJob(jobsettings);
                //                if (job.IsValid)
                //                {
                //                    bBad = false;
                //                    Stations[jobsettings.Station].AddJob(job);

                //                    //Update Dictionary of Tag-Station link
                //                    if (job.GetTagCount() > 0)
                //                    {
                //                        for (int i = 0; i < job.GetTagCount(); i++)
                //                            TagToStationMap[job.GetTagNodeId(i)] = Stations[jobsettings.Station];
                //                    }
                //                }
                //                else
                //                {
                //                    //log something
                //                    OnSystemEvent((object)job.GetTagNodeId(0), job.InvalidReason, EventSeverity.Medium);
                //                }
                //            }

                //            if (bBad)
                //            {
                //                foreach (var tag in jobsettings.Tags)
                //                    OnSystemEvent((object)tag.NodeId, Properties.Resources.TagUninitailized, EventSeverity.Medium);
                //            }
                //        }
                //    }
                }
            }

            return (TagToStationMap.Count > 0);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Saves the dynamic jobs. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool SaveDynamicJobs()
        {
            // Added just for debugging
            DateTime dtime0 = DateTime.UtcNow;

            string filebase;
            InMemoryDataStore InMemory;

            //// delete dynamic jobs file for create save data
            using (IDataLayer idl = GetDynamicJobsDataLayer(out filebase, out InMemory))
            {
                if (filebase != null && InMemory != null)
                {
                    if (!String.IsNullOrEmpty(filebase))
                    {
                        if (File.Exists(filebase))
                        {
                            try
                            {
                                File.Delete(filebase);
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                }
            }

            using (IDataLayer idl = GetDynamicJobsDataLayer(out filebase, out InMemory))
            {
                using (UnitOfWork ufw = new UnitOfWork(idl))
                {
                    //var dynamicJobsList = (from tag in new XPQuery<DynamicJobs>(ufw)/*.AsParallel()*/ select tag).ToList();
                    //if (dynamicJobsList.Count > 0 /*!= null*/)
                    //    dynamicJobsList[0].Delete();

                    var dynamicJobs = new DynamicJobs(ufw);

                    foreach (var stationname in Stations.Keys)
                    {
                        foreach (var job in Stations[stationname].ListWholeJob)
                        {
                            var commJobSettings = Stations[stationname].CreateJobSettings(ufw, job);
                            foreach (var tag in job.TagsList)
                            {
                                var tagSettings = CreateTagSettings(ufw, tag);
                                commJobSettings.Tags.Add(tagSettings);
                            }
                            dynamicJobs.JobSettings.Add(commJobSettings);
                        }
                    }
                   
                    dynamicJobs.LastInteraction = DateTime.UtcNow;
                    ufw.CommitChanges();

                    if(filebase != null && InMemory != null)
                    {
                        if (!String.IsNullOrEmpty(filebase))
                        {
                            if (File.Exists(filebase))
                            {
                                try
                                {
                                    File.Delete(filebase);
                                }
                                catch (Exception ex)
                                {
                                }
                            }
                            try
                            {
                                InMemory.WriteXml(filebase);
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                }
            }

            // Added just for debugging
            double dtime = (DateTime.UtcNow - dtime0).TotalMilliseconds;
            System.Diagnostics.Trace.TraceInformation("SaveDynamicTags {0} {1} ms", dtime0, dtime);

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Loads dynamic jobs. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool LoadChangeSettings(Dictionary<NodeId, string> Map )
        {
            using (IDataLayer idl = GetChangeSettingsDataLayer())
            {

                var changeTags = LoadChangeSettings(idl);

                if (changeTags != null && changeTags.Count > 0)
                {
                    foreach (var ChangedTag in changeTags)
                    {

                        if (!Map.ContainsKey(ChangedTag.NodeId))
                            Map.Add(ChangedTag.NodeId, ChangedTag.DynamicSettings);
                    }
                }
            }
            return (Map.Count > 0);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Saves the change settings. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool SaveChangeSettings()
        {

            string filebase;
            InMemoryDataStore InMemory;

            //// delete change settings file for create save data
            using (IDataLayer idl = GetChangeSettingsDataLayer(out filebase, out InMemory))
            {
                if (filebase != null && InMemory != null)
                {
                    if (!String.IsNullOrEmpty(filebase))
                    {
                        if (File.Exists(filebase))
                        {
                            try
                            {
                                File.Delete(filebase);
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                }
            }

            using (IDataLayer idl = GetChangeSettingsDataLayer(out filebase, out InMemory))
            {
                //SaveChangeSettings(idl);
                if (filebase != null && InMemory != null)
                {
                    if (!String.IsNullOrEmpty(filebase))
                    {
                        if (File.Exists(filebase))
                        {
                            try
                            {
                                File.Delete(filebase);
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        try
                        {
                            InMemory.WriteXml(filebase);
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }

            return true;
        }

#endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Adds to the tag to station map. </summary>
        ///
        /// <param name="key">                  name of a valid counter. </param>
        /// <param name="value" type="Station"> The value. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void AddToTagToStationMap(NodeId key, Station value)      
        {
            lock (lockMaps)
            {
                if(!TagToStationMap.ContainsKey(key))
                    TagToStationMap.Add(key, value);
            }
        }

#region Statistics Management

        /// <summary>   The objects with statistics. </summary>
        List<IStatistics> ObjectsWithStatistics;
        /// <summary>   Starts all statistics. </summary>
        private void StartAllStatistics()
        {

            if (ObjectsWithStatistics == null)
                ObjectsWithStatistics = new List<IStatistics>();

            var refobject = this as IStatistics;
            if (refobject != null)
            {
                StartStatistics();
                ObjectsWithStatistics.Add(refobject);
            }

            //object lockobject = new object();
            //Parallel.ForEach(Channels.Values, channel =>
            //{
            //    refobject = channel as IStatistics;
            //    if (refobject != null)
            //    {
            //        refobject.StartStatistics();
            //        lock (lockobject)
            //            ObjectsWithStatistics.Add(refobject);
            //    }
            //});

            //Parallel.ForEach(Stations.Values, station =>
            //{
            //    refobject = station as IStatistics;
            //    if (refobject != null)
            //    {
            //        refobject.StartStatistics();
            //        lock (lockobject)
            //            ObjectsWithStatistics.Add(refobject);
            //    }
            //});

            foreach (var channel in GetChannels())
            {
                refobject = channel as IStatistics;
                if (refobject != null)
                {
                    refobject.StartStatistics();
                    ObjectsWithStatistics.Add(refobject);
                }
            }
            foreach (var station in GetStations())
            {
                refobject = station as IStatistics;
                if (refobject != null)
                {
                    refobject.StartStatistics();
                    ObjectsWithStatistics.Add(refobject);
                }
            }
        }

        /// <summary>   Terminate all statistics. </summary>
        private void TerminateAllStatistics()
        {
            if (ObjectsWithStatistics == null)
                return;

            foreach (var refobject in ObjectsWithStatistics)
            {
                refobject.TerminateStatistics();
                //ObjectsWithStatistics.Remove(refobject);
            };
            ObjectsWithStatistics.Clear();
        }

        /// <summary>   Resume all statistics. </summary>
        private void ResumeAllStatistics()
        {
            if (ObjectsWithStatistics == null)
                return;
            //Parallel.ForEach(ObjectsWithStatistics, refobject =>
            //{
            //    refobject.ResumeStatistics();
            //});
            foreach (var refobject in ObjectsWithStatistics)
            {
                refobject.ResumeStatistics();
            };
        }

        /// <summary>   Suspend all statistics. </summary>
        private void SuspendAllStatistics()
        {
            if (ObjectsWithStatistics == null)
                return;
            //Parallel.ForEach(ObjectsWithStatistics, refobject =>
            //{
            //    refobject.SuspendStatistics();
            //});
            foreach(var refobject in ObjectsWithStatistics)
            {
                refobject.SuspendStatistics();
            };
        }

        /// <summary>   Resets all statistcs. </summary>
        private void ResetAllStatistcs()
        {
            if (ObjectsWithStatistics == null)
                return;
            //Parallel.ForEach(ObjectsWithStatistics, refobject =>
            //{
            //    refobject.ResetStatistcs();
            //});
            foreach (var refobject in ObjectsWithStatistics)
            {
                refobject.ResetStatistcs();
            };
        }

#endregion

#region IStatistics Interface

        /// <summary>   Information describing the statistics. </summary>
        private StatisticCounters StatisticsData;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Allow to know if the statistics are available for this object. </summary>
        ///
        /// <returns>   true if statistics are available; otherwise, false. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool IsStatisticsAvailable()
        {
            return StatisticsData != null;
        }

        /// <summary>   Initialize statistics. </summary>
        public void StartStatistics()
        {
            if (StatisticsData == null)
            {
                StatisticsData = new StatisticCounters();
                StatisticsData.ChangedCounter += StatisticsData_ChangedCounter;
                StatisticsData.StartWatch();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by StatisticsData for changed counter events. </summary>
        ///
        /// <param name="sender" type="object">             Source of the event. </param>
        /// <param name="e" type="ChangedCounterEventArgs"> Changed counter event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void StatisticsData_ChangedCounter(object sender, ChangedCounterEventArgs e)
        {
                OnTagChanged(new NodeId(string.Format("{0}?{1}Statistics/{2}",RootDriversGuid, DriverName, e.Key), NamespaceIndex), 
                    new DataValue(e.newValue));
        }

        /// <summary>   Terminate statistics and free the counters. </summary>
        public void TerminateStatistics()
        {
            if (StatisticsData != null)
                StatisticsData.Dispose();

            StatisticsData = null;
        }

        /// <summary>   Suspend statistics watcher. </summary>
        public void SuspendStatistics()
        {
            if (StatisticsData != null)
                StatisticsData.StopWatch();
        }

        /// <summary>   Resum statistics watcher. </summary>
        public void ResumeStatistics()
        {
            if (StatisticsData != null)
                StatisticsData.StartWatch();
        }

        /// <summary>   Reset whole statistic counters to zero. </summary>
        public void ResetStatistcs()
        {
            if (StatisticsData != null)
                StatisticsData.ResetStatistcs();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Allow to retrieve a copy of the current statistics dictionary. </summary>
        ///
        /// <param name="totaltimeon" type="out TimeSpan">  [out] out value with the elapsed total time
        ///                                                 with statistics enabled. </param>
        ///
        /// <returns>   An IDictionary. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public IDictionary RetrieveStatisticCounters(out TimeSpan totaltimeon)
        {
            if (StatisticsData != null)
                return StatisticsData.RetrieveStatistcs(out totaltimeon);

            totaltimeon = new TimeSpan();
            return new Dictionary<string, long>();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Return the number of statistic values recorded. </summary>
        ///
        /// <returns>   The total counters. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int GetTotalCounters()
        {
            if (StatisticsData != null)
                return StatisticsData.GetTotalCounters();

            return 0;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Return a list with the name of each statistic counter. </summary>
        ///
        /// <returns>   The list of counters name. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public IList GetListOfCountersName()
        {
            if (StatisticsData != null)
                return StatisticsData.GetListOfCountersName();

            return new List<String>();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Return the value of a counter. </summary>
        ///
        /// <param name="key">  name of a valid counter. </param>
        ///
        /// <returns>   The counter value. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public long GetCounterValue(string key)
        {
            if (StatisticsData != null)
                return StatisticsData.GetCounterValue(key);

            return 0;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Return the elapsed total time with statistics enabled. </summary>
        ///
        /// <returns>   The total time on. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public TimeSpan GetTotalTimeOn()
        {
            if (StatisticsData != null)
                return StatisticsData.GetTotalTimeOn();

            return new TimeSpan();
        }

        /// <summary>   The statistic nodes. </summary>
        public static readonly StatisticSetting.NodeDataNames[] StatisticNodes = new StatisticSetting.NodeDataNames[]
        {
            StatisticSetting.NodeDataNames.ElapsedTime,
            StatisticSetting.NodeDataNames.TotalTags,
            StatisticSetting.NodeDataNames.TotalTagsInUse,
            StatisticSetting.NodeDataNames.PeakOfTagsInUse,
            StatisticSetting.NodeDataNames.TotalJobs,
            StatisticSetting.NodeDataNames.TotalJobsInUse,
        };


#endregion

#region IDisposable

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void Dispose()
        {
            TagToStationMap.Clear();
            //Parallel.ForEach(Stations.Values, value =>
            foreach (var value in Stations.Values)
            {
                var station = value as IDisposable;
                if (station != null)
                    station.Dispose();
            };

            //Parallel.ForEach(Channels.Values, value =>
            foreach (var value in Channels.Values)
            {
                var channel = value as IDisposable;
                if (channel != null)
                    channel.Dispose();
            };

            if (StatisticsData != null)
            {
                StatisticsData.Dispose();
                StatisticsData.ChangedCounter -= StatisticsData_ChangedCounter;
                StatisticsData = null;
            }
            
            if (smartThreadPool != null)
            {
                smartThreadPool.WaitForIdle();
                smartThreadPool.Shutdown();
                smartThreadPool = null;
            }

            EndDiagnosic();

            OnSystemEvent(null, string.Format(Properties.Resources.DriverTermination, DriverName), EventSeverity.Low);
        }
        #endregion


        #region ICrossReferenceAware
        #if !NETSTANDARD
        public Dictionary<string, string> GetTagList(ComunicationSettingsContext2 settingsContext)
        {            
            Dictionary<string, string> result = new Dictionary<string, string>();

            using (IDataLayer idl = DriverCodeBase.CommunicationDriver.GetDriverDataLayer(settingsContext.ConnectionString, DriverName, out string fileBase, out InMemoryDataStore InMemory))
            {
                using (UnitOfWork ufw = new UnitOfWork(idl))
                {
                    //try
                    //{
                    //driver specific class
                    DriverSettings configuration = (from tag in new XPQuery<DriverSettings>(ufw).AsParallel() select tag).Single();
                    if (configuration != null)
                    {
                        // General
                        if (configuration.StateCommandTag != null)
                            result.Add(DriverName, configuration.StateCommandTag.ToXml());

                        //Channels
                        var channels = (from s in configuration.ChannelSettings.AsParallel()
                                        orderby s.Name
                                        select s).ToList();
                        foreach (var ch in channels)
                        {
                            if (ch.StateCommandTag != null)
                                result.Add(string.Format("{0}.{1}", DriverName, ch.Name), ch.StateCommandTag.ToXml());
                        }

                        //Stations
                        var stations = (from s in configuration.StationSettings.AsParallel()
                                        orderby s.Name
                                        select s).ToList();
                        foreach (var st in stations)
                        {
                            if (st.StateCommandTag != null)
                                result.Add(string.Format("{0}.{1}.{2}", DriverName, st.Channel, st.Name), st.StateCommandTag.ToXml());
                        }
                    }
                    //}
                    //catch (Exception ex)
                    //{
                    //}
                }
            }
            
            return result;
        }
            
        public void SetTagList(List<string> tagMap, ComunicationSettingsContext2 settingsContext)
        {
            List<UFUAModel.TagEntityReference> tagEntityReferenceMap = new List<UFUAModel.TagEntityReference>();
            foreach (var tagString in tagMap)
                tagEntityReferenceMap.Add(tagString.FromXml<UFUAModel.TagEntityReference>());

            using (IDataLayer idl = DriverCodeBase.CommunicationDriver.GetDriverDataLayer(settingsContext.ConnectionString, DriverName, out string fileBase, out InMemoryDataStore InMemory))
            {
                using (UnitOfWork ufw = new UnitOfWork(idl))
                {
                    //try
                    //{
                        //driver specific class
                        DriverSettings configuration = (from tag in new XPQuery<DriverSettings>(ufw).AsParallel() select tag).Single();
                        if (configuration != null)
                        {
                            // General
                            if (configuration.StateCommandTag != null && !NodeId.IsNull(configuration.StateCommandTag.NodeId))
                            {
                                var tag = tagEntityReferenceMap.FirstOrDefault(t => t.NodeId == configuration.StateCommandTag.NodeId);
                                if (tag != null)
                                    configuration.StateCommandTag = tag;
                            }

                            //Channels
                            var channels = (from s in configuration.ChannelSettings.AsParallel()
                                            orderby s.Name
                                            select s).ToList();
                            foreach (var ch in channels)
                            {
                                if (ch.StateCommandTag != null && !NodeId.IsNull(ch.StateCommandTag.NodeId))
                                {
                                    var tag = tagEntityReferenceMap.FirstOrDefault(t => t.NodeId == ch.StateCommandTag.NodeId);
                                    if (tag != null)
                                        ch.StateCommandTag = tag;
                                }
                            }

                            //Stations
                            var stations = (from s in configuration.StationSettings.AsParallel()
                                            orderby s.Name
                                            select s).ToList();
                            foreach (var st in stations)
                            {
                                if (st.StateCommandTag != null && !NodeId.IsNull(st.StateCommandTag.NodeId))
                                {
                                    var tag = tagEntityReferenceMap.FirstOrDefault(t => t.NodeId == st.StateCommandTag.NodeId);
                                    if (tag != null)
                                        st.StateCommandTag = tag;
                                }
                            }

                            #region save data into storage

                            if (settingsContext.bProtected)
                                XpoHelpers.XpoHelper.AddProtectionCode(ufw, settingsContext.protectionCode);
                            else
                                XpoHelpers.XpoHelper.RemoveProtectionCode(ufw);

                            //try
                            //{
                                ufw.CommitChanges();
                                ufw.ReloadChangedObjects();
                                ufw.PurgeDeletedObjects();

                                ufw.CommitChanges();
                            //}
                            //catch (Exception ex)
                            //{
                            //    if (Environment.UserInteractive)
                            //        MessageBox.Show(ex.Message);
                            //    if (File.Exists(fileBase))
                            //    {
                            //        try
                            //        {
                            //            File.Delete(fileBase);
                            //        }
                            //        catch (Exception ex1)
                            //        {
                            //            //if (Environment.UserInteractive)
                            //            //    MessageBox.Show(ex1.Message);
                            //        }
                            //    }
                            //    return;
                            //}

                            if (!String.IsNullOrEmpty(fileBase))
                            {
                                if (File.Exists(fileBase))
                                {
                                    //try
                                    //{
                                        File.Delete(fileBase);
                                    //}
                                    //catch (Exception ex)
                                    //{
                                    //    //if (Environment.UserInteractive)
                                    //    //    MessageBox.Show(ex.Message);
                                    //    return;
                                    //}
                                }

                                //try
                                //{
                                    if (settingsContext.bProtected)
                                    {
                                        using (var memoryStream = new MemoryStream())
                                        {
                                            var writer = XmlWriter.Create(memoryStream);
                                            InMemory.WriteXml(writer);
                                            writer.Flush();//14577
                                            writer.Close();
                                            var str = Convert.ToBase64String(memoryStream.ToArray());
                                            var toWrite = DriverCodeBase.CommunicationDriver.EncryptString(str);
                                            File.WriteAllText(fileBase, toWrite);
                                        }
                                    }
                                    else
                                        InMemory.WriteXml(fileBase);
                                //}
                                //catch (Exception ex) {   }
                            }
                            #endregion
                        }
                    //}
                    //catch (Exception ex)
                    //{
                    //}
                }
            }
        }
        #endif
        #endregion
    }
}
