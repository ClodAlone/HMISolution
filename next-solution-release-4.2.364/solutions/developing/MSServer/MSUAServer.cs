using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFUAServerBase;
using DevExpress.Xpo;
using Opc.Ua.Server;
using Opc.Ua;
using System.Threading;
using System.Threading.Tasks;
using Utilities;
using MSServerInfo;
using OPCUAViewModel;
using ViewModelLib;
using UFInterfaces;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using System.Diagnostics;
using Utilities.Logger;

namespace MSServer
{
    public class MSUAServer : UAServer, IDisposable
    {
#region Declarations

        UnitOfWork messageuow = null;
        IDataLayer messagedl = null;

#endregion

        private Dictionary<NodeId, object> _UserList = new Dictionary<NodeId, object>();
        public Dictionary<NodeId, object> UserList
        {
            get { return _UserList; }
        }

        /// <summary>
        /// Loads the non-configurable properties for the application.
        /// </summary>
        /// <remarks>
        /// These properties are exposed by the server but cannot be changed by administrators.
        /// </remarks>
        protected override ServerProperties LoadServerProperties()
        {
            //base.LoadServerProperties();
            using (new StopWatcherLogger(Properties.Resources.LoggerSource, LoggerDestination.Scheduler,
                                            Properties.Resources.LoadingServerProperties,
                                            Properties.Resources.LoadedServerProperties))
            {
                ServerProperties properties = new ServerProperties();

                using (var ufw = new UnitOfWork(UFUAServer.DefaultDataLayer))
                {
                    id = XpoHelpers.XpoHelper.GetProtectionCode(ufw);

                    var configurations = (from tag in new XPQuery<MSModel.MSGeneralSettings>(ufw).AsParallel() select tag).ToList();
                    if (configurations.Count == 0)
                        configurations.Add(new MSModel.MSGeneralSettings(ufw));

                    if (configurations.Count > 0)
                    {
                        var applicationName = String.IsNullOrEmpty(configurations[0].ApplicationName) ? Configuration.ApplicationName : configurations[0].ApplicationName;
                        configurations[0].EnsureDefaultSettings(applicationName);

                        ParentApplicationName = configurations[0].ParentApplicationName ?? String.Empty;
                        Configuration.ApplicationName = applicationName;

                        properties.ManufacturerName = configurations[0].ManufacturerName ?? String.Empty; //  "Progea srl";
                        properties.ProductName = configurations[0].ProductName ?? String.Empty; // "UF UA Server";
                        properties.ProductUri = configurations[0].ProductUri ?? String.Empty; // "http://progea.com/UFSolution/UAServer/v1.0";
                        properties.SoftwareVersion = configurations[0].SoftwareVersion ?? String.Empty; // Utils.GetAssemblySoftwareVersion();
                        properties.BuildNumber = configurations[0].BuildNumber ?? String.Empty; // Utils.GetAssemblyBuildNumber();
                        properties.BuildDate = configurations[0].BuildDate; // Utils.GetAssemblyTimestamp();
                    }
                }

                // TBD - All applications have software certificates that need to added to the properties.

                return properties;
            }
        }

        protected override MasterNodeManager CreateMasterNodeManager(IServerInternal server, ApplicationConfiguration configuration)
        {
            ViewModelBase.bDoNotCheckDispatcherObjects = true;

            
            //base.CreateMasterNodeManager(server, configuration);

            using (var ufw = new UnitOfWork(UFUAServer.DefaultDataLayer))
            {
                var configurations = (from tag in new XPQuery<MSModel.MSGeneralSettings>(ufw).AsParallel() select tag).ToList();

                if (configurations.Count > 0)
                    UFUAConfiguration = configurations[0];
                else
                    UFUAConfiguration = new MSModel.MSGeneralSettings(ufw);
            }

            List<INodeManager> nodeManagers = new List<INodeManager>();

            // create the custom node managers.
            uaNodeManager = new MSUANodeManager(this, server, configuration);
            nodeManagers.Add(uaNodeManager);
            
            // create master node manager.
            return new MasterNodeManager(server, configuration, null, nodeManagers.ToArray());
        }

#if !DEBUG
        bool bLog = false;
        protected override bool EndVirtualDisk()
        {
            var mode = MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNx214/TMTBEjcA8SkKPNVPhw=="/* SCD */);
            if (mode == false && !bLog)
            {
                bLog = true;
                OServer.OnSystemEvent(ObjectIds.Server, Properties.Resources.NoSvrLicence, EventSeverity.Medium, ObjectTypeIds.SystemStatusChangeEventType, null,
                        null, (int)System.Diagnostics.EventLogEntryType.Warning, (int)LoggerDestination.License);
                OnBalloonEvent(Properties.Resources.NoSvrLicence, System.Windows.Forms.ToolTipIcon.Warning);
            }
            else if (mode)
                bLog = false;

            return mode;
        }
#endif

        /// <summary>
        /// Called after the server has been started.
        /// </summary>
        protected override void OnServerStarted(IServerInternal server)
        {
            base.OnServerStarted(server);

            ScheduleEngine.Startup(this);
        }

        protected override void OnServerStopping()
        {
            if (scheduleEngine != null)
            {
                scheduleEngine.StopThread();
                scheduleEngine.Dispose();
                scheduleEngine = null;
            }

            base.OnServerStopping();
        }

        public void SaveToFile(InMemoryDataStore inMemory)
        {
            string fileBase = MSSchedulerSettings.Document.SchedulerEditorDocument.GetFileBase(ActiveConnectionString);
            MSSchedulerSettings.Document.SchedulerEditorDocument.SaveToFile(fileBase, inMemory);
        }

        public IDataLayer GetRuntimeDataLayer(out InMemoryDataStore datastore)
        {

            string conn = MSSchedulerSettings.Document.SchedulerEditorDocument.GetRuntimeConnectionString(ActiveConnectionString);
            return MSSchedulerSettings.Document.SchedulerEditorDocument.GetSpecificDataLayer(conn, out datastore);
        }
        public IDataLayer GetDataLayerActive()
        {
            InMemoryDataStore datastore;
            return MSSchedulerSettings.Document.SchedulerEditorDocument.GetSpecificDataLayer(ActiveConnectionString, out datastore);
        }

#region Properties

        private MSServer _OServer;
        public MSServer OServer
        {
            get { return _OServer; }
            set { _OServer = value; }
        }

        SchedulingThread scheduleEngine;
        internal SchedulingThread ScheduleEngine
        {
            get
            {
                if (scheduleEngine == null)
                    scheduleEngine = new SchedulingThread();

                return scheduleEngine;
            }
        }

#endregion

#region IDisposable Members

        void  IDisposable.Dispose()
        {
            if (messageuow != null)
            {
                messageuow.Disconnect();
                messageuow.Dispose();
                messageuow = null;
            }

            if (messagedl != null)
            {
                messagedl.Dispose();
                messagedl = null;
            }
    
        }

#endregion
    }
}
