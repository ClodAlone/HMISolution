using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Web.Administration;
using System.Linq;

namespace IIS7Manager
{
    /// <summary>
    /// IISAppPool is used to manage AppPool of IIS. We can create an AppPool with this class.
    /// I use Directory Service to manage AppPool.
    /// </summary>
    public class IISAppPool
    {
        #region Nested Enums
        public enum IISAppPoolManagedPipelineMode : int
        {
            Integrated = 0,
            Classic = 1,
        }

        public enum IISAppPoolProcessModelIdentityType : int
        {
            LocalSystem = 0,
            LocalService = 1,
            NetworkService = 2,
            SpecificUser = 3,
            ApplicationPoolIdentity = 4,
        }
        #endregion

        #region Declarations
        ServerManager serverMgr = null;
        private ApplicationPool _entry = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Private constructor. Anyone wants to Create an instance of IISAppPool should call
        /// OpenAppPool
        /// </summary>
        /// <param name="dir"></param>
        protected IISAppPool(ApplicationPool entry)
        {
            this._entry = entry;
        }

        /// <summary>
        /// Protect this constructor so that user can not create a IISAppPool instance directly.
        /// To get IISAppPool instance, please use IISAppPool.OpenAppPool or IISAppPool.CreateAppPool
        /// </summary>
        /// <param name="Server"></param>
        protected IISAppPool(ServerManager manager, ApplicationPool application)
        {
            this.serverMgr = manager;
            this._entry = application;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Get name of the App Pool
        /// </summary>
        public string Name
        {
            get
            {
                string name = _entry.Name;
                return name;
            }
        }

        /// <summary>
        /// gets or sets the version of the .NET Framework that is used for managed applications in the current application pool.
        /// </summary>
        public IISWebsiteStatus State
        {
            get
            {
                ObjectState status = this._entry.State;
                return (IISWebsiteStatus)status;
            }
        }

        /// <summary>
        /// gets or sets the version of the .NET Framework that is used for managed applications in the current application pool.
        /// </summary>
        public string ManagedRuntimeVersion
        {
            get
            { 
                return this._entry.ManagedRuntimeVersion.ToString();
            }
            set
            {
                this._entry.ManagedRuntimeVersion = value;
            }
        }

        /// <summary>
        /// gets or sets a value that indicates the pipeline mode of managed applications in the current application pool.
        /// </summary>
        public IISAppPoolManagedPipelineMode ManagedPipelineMode
        {
            get
            {
                return (IISAppPoolManagedPipelineMode)this._entry.ManagedPipelineMode;
            }
            set
            {
                this._entry.ManagedPipelineMode = (ManagedPipelineMode)value;
            }
        }

        /// <summary>
        /// Get or Set the process model indentity type of the application pool.
        /// </summary>
        public IISAppPoolProcessModelIdentityType ProcessModelIdentityType
        {
            get
            {
                return (IISAppPoolProcessModelIdentityType)this._entry.ProcessModel.IdentityType;
            }
            set
            {
                this._entry.ProcessModel.IdentityType = (ProcessModelIdentityType)value;
            }
        }

        /// <summary>
        /// Get or Set the user name of the process model of the application pool.
        /// </summary>
        public string ProcessModelUserName
        {
            get
            {
                return this._entry.ProcessModel.UserName;
            }
            set
            {
                this._entry.ProcessModel.UserName = value;
            }
        }

        /// <summary>
        /// Get or Set the password of the process model of the application pool.
        /// </summary>
        public string ProcessModelPassword
        {
            get
            {
                return this._entry.ProcessModel.Password;
            }
            set
            {
                this._entry.ProcessModel.Password = value;
            }
        }

        /// <summary>
        /// Get or Set Disable overlapped recycle option.
        /// </summary>
        public bool DisallowOverlappingRotation
        {
            get
            {
                return this._entry.Recycling.DisallowOverlappingRotation;
            }
            set
            {
                this._entry.Recycling.DisallowOverlappingRotation = value;
            }
        }
        #endregion

        #region Operations
        /// <summary>
        /// Start application pool.
        /// </summary>
        public void Start()
        {
            this._entry.Start();
        }

        /// <summary>
        /// Stop application pool.
        /// </summary>
        public void Stop()
        {
            this._entry.Stop();
        }

        /// <summary>
        /// Commit changes to web server instance.
        /// This operation is one shoot
        /// </summary>
        public void CommitChanges()
        {
            if (serverMgr != null)
            {
                serverMgr.CommitChanges();
                serverMgr.Dispose();
            }
            serverMgr = null;
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Open a application pool and return an IISAppPool instance
        /// </summary>
        /// <param name="name">application pool name</param>
        /// <returns>IISAppPool object</returns>
        public static IISAppPool OpenAppPool(string name)
        {
            ServerManager serverMgr = new ServerManager();
            if (!Exsit(name))
            {
                return null;
            }

            ApplicationPool pool = serverMgr.ApplicationPools.Where(c => c.Name == name).SingleOrDefault();
            return new IISAppPool(serverMgr, pool);
        }

        /// <summary>
        /// create app pool
        /// </summary>
        /// <param name="name">the app pool to be created</param>
        /// <returns>IISAppPool created if success, else null</returns>
        public static IISAppPool CreateAppPool(string name)
        {
            ServerManager serverMgr = new ServerManager();
            if (Exsit(name))
            {
                ApplicationPool pool = serverMgr.ApplicationPools.Where(c => c.Name == name).SingleOrDefault();
                return IISAppPool.OpenAppPool(pool.Name);
            }

            ApplicationPool appPool = serverMgr.ApplicationPools.Add(name);
            serverMgr.CommitChanges();

            return new IISAppPool(appPool);
        }

        /// <summary>
        /// if the app pool specified exsit
        /// </summary>
        /// <param name="name">name of app pool</param>
        /// <returns>true if exsit, otherwise false</returns>
        public static bool Exsit(string name)
        {
            ServerManager serverMgr = new ServerManager();
            return serverMgr.ApplicationPools.Where(c => c.Name == name).SingleOrDefault() != null;
        }

        /// <summary>
        /// Delete an app pool
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool DeleteAppPool(string name)
        {
            ServerManager serverMgr = new ServerManager();
            ApplicationPool pool = serverMgr.ApplicationPools.Where(c => c.Name == name).SingleOrDefault();
            if (pool != null)
                serverMgr.ApplicationPools.Remove(pool);
            serverMgr.CommitChanges();

            return pool != null;
        }

        /// <summary>
        /// Retreive the list of application pools.
        /// </summary>
        public static List<string> GetAppPools()
        {
            ServerManager serverMgr = new ServerManager();
            return (from c in serverMgr.ApplicationPools select c.Name).ToList();
        }
        #endregion
    }
}
