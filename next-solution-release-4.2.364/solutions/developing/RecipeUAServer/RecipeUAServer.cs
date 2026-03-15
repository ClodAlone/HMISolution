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

namespace RecipeUAServer
{
    public partial class RecipeUAServer : UAServer, IDisposable
    {
        #region Declarations

        #endregion

        /// <summary>
        /// Loads the non-configurable properties for the application.
        /// </summary>
        /// <remarks>
        /// These properties are exposed by the server but cannot be changed by administrators.
        /// </remarks>
        protected override ServerProperties LoadServerProperties()
        {
            //base.LoadServerProperties();
            using (new StopWatcherLogger(Utilities.Properties.Resources.RecipeService, LoggerDestination.RecipeService,
                Properties.Resources.LoadingServerProperties,
                Properties.Resources.LoadedServerProperties))
            {
                ServerProperties properties = new ServerProperties();

                using (var ufw = new UnitOfWork(UFUAServer.DefaultDataLayer))
                {
                    id = XpoHelpers.XpoHelper.GetProtectionCode(ufw);

                    var configurations = (from tag in new XPQuery<UFRecipeSettings.UFRecipeModel.RecipeUAConfiguration>(ufw).AsParallel() select tag).ToList();
                    if (configurations.Count == 0)
                        configurations.Add(new UFRecipeSettings.UFRecipeModel.RecipeUAConfiguration(ufw));

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
                var configurations = (from tag in new XPQuery<UFRecipeSettings.UFRecipeModel.RecipeUAConfiguration>(ufw).AsParallel() select tag).ToList();

                if (configurations.Count > 0)
                    UFUAConfiguration = configurations[0];
                else
                    UFUAConfiguration = new UFRecipeSettings.UFRecipeModel.RecipeUAConfiguration(ufw);
            }

            List<INodeManager> nodeManagers = new List<INodeManager>();

            // create the custom node managers.
            uaNodeManager = new RecipeUANodeManager(this, server, configuration);
            nodeManagers.Add(uaNodeManager);
            
            // create master node manager.
            return new MasterNodeManager(server, configuration, null, nodeManagers.ToArray());
        }

#if !DEBUG
        bool bLog = false;
        protected override bool EndVirtualDisk()
        {
            var mode = MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxLpZ9HLAzv1ul2RsoVlDyXw=="/* RCP */);
            if (mode == false && !bLog)
            {
                bLog = true;
                RecipeServer.OnSystemEvent(ObjectIds.Server, Properties.Resources.NoSvrLicence, EventSeverity.Medium, ObjectTypeIds.SystemStatusChangeEventType, null,
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
        }

        protected override void OnServerStopping()
        {
            base.OnServerStopping();
        }

        #region overrides
        internal new bool GetUserLevel(String user, ref int accessMask, ref int accessLevel)
        {
            return base.GetUserLevel(user, ref accessMask, ref accessLevel);
        }

        internal new bool IsUserManagerEnabled()
        {
            return base.IsUserManagerEnabled();
        }
        #endregion

        #region Properties
        internal RecipeServer RecipeServer;

        string safeDataConnectionString;
        protected override string SafeDataConnectionString
        {
            get
            {
                if (safeDataConnectionString == null)
                {
                    if (!XpoHelpers.XpoHelper.IsSQlDataProvider(ActiveConnectionString))
                    {
                        var ds = XpoHelpers.XpoHelper.GetDataSourceServer(ActiveConnectionString, checkprovidertype: false);
#if !NET_CORE
                        var path = System.IO.Path.GetDirectoryName(ds.Replace('/', System.IO.Path.DirectorySeparatorChar));
#else
                        var path = System.IO.Path.GetDirectoryName(ds.Replace('\\', System.IO.Path.DirectorySeparatorChar));
#endif
                        var index = path.LastIndexOf(System.IO.Path.DirectorySeparatorChar);
                        if (index != -1)
                        {
                            path = String.Format("{0}{1}{2}", ds.Substring(0, index), System.IO.Path.DirectorySeparatorChar, RecipeServer.application.ConfigSectionName);
                            var newds = System.IO.Path.Combine(path, System.IO.Path.GetFileName(ds));
                            safeDataConnectionString = XpoHelpers.XpoHelper.SetDataSourceServer(ActiveConnectionString, newds, checkprovidertype: false);
                        }
                    }
                    else
                        safeDataConnectionString = base.SafeDataConnectionString;
                }

                return safeDataConnectionString;
            }
        }
#endregion

#region IDisposable Members

        void  IDisposable.Dispose()
        { }

#endregion
    }
}
