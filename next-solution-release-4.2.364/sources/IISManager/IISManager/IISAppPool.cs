using System;
using System.Collections.Generic;
using System.Text;
using System.DirectoryServices;

namespace IISManager
{
    /// <summary>
    /// IISAppPool is used to manage AppPool of IIS. We can create an AppPool with this class.
    /// I use Directory Service to manage AppPool.
    /// 
    /// Author: luckzj
    /// Time:   27/June 2010
    /// Email:  luckzj12@163.com
    /// Website: http://soft-bin.com
    /// </summary>
    public class IISAppPool
    {
        private DirectoryEntry _entry = null;

        public static string PipelineModeIntegrated = "Integrated";
        public static string PipelineModeClassic = "Classic";

        /// <summary>
        /// Private constructor. Anyone wants to Create an instance of IISAppPool should call
        /// OpenAppPool
        /// </summary>
        /// <param name="dir"></param>
        protected IISAppPool(DirectoryEntry entry)
        {
            this._entry = entry;
        }

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
        public string ManagedRuntimeVersion
        {
            get
            { 
                return this._entry.Properties["ManagedRuntimeVersion"][0].ToString();
            }
            set
            {
                this._entry.Properties["ManagedRuntimeVersion"][0] = value;
                this._entry.CommitChanges();
            }
        }

        /// <summary>
        /// gets or sets a value that indicates the pipeline mode of managed applications in the current application pool.
        /// </summary>
        public string ManagedPipelineMode
        {
            get
            {
                return this._entry.Properties["ManagedPipelineMode"][0].ToString();
            }
            set
            {
                if (value == PipelineModeIntegrated)
                    this._entry.Properties["ManagedPipelineMode"][0] = 0;
                else if (value == PipelineModeClassic)
                    this._entry.Properties["ManagedPipelineMode"][0] = 1;
                else
                    throw new ArgumentOutOfRangeException("ManagedPipelineMode");
                this._entry.CommitChanges();
            }
        }

        /// <summary>
        /// Start application pool.
        /// </summary>
        public void Start()
        {
            this._entry.Invoke("Start");
        }

        /// <summary>
        /// Stop application pool.
        /// </summary>
        public void Stop()
        {
            this._entry.Invoke("Stop");
        }

        /// <summary>
        /// Open a application pool and return an IISAppPool instance
        /// </summary>
        /// <param name="name">application pool name</param>
        /// <returns>IISAppPool object</returns>
        public static IISAppPool OpenAppPool(string name)
        {
            string connectStr = "IIS://localhost/W3SVC/AppPools/";
            connectStr += name;

            if (IISAppPool.Exsit(name) == false)
            {
                return null;
            }


            DirectoryEntry entry = new DirectoryEntry(connectStr); 
            return new IISAppPool(entry);
        }

        /// <summary>
        /// create app pool
        /// </summary>
        /// <param name="name">the app pool to be created</param>
        /// <returns>IISAppPool created if success, else null</returns>
        public static IISAppPool CreateAppPool(string name)
        {
            DirectoryEntry Service = new DirectoryEntry("IIS://localhost/W3SVC/AppPools");
            foreach (DirectoryEntry entry in Service.Children)
            {
                if (entry.Name.Trim().ToLower() == name.Trim().ToLower())
                {
                    return IISAppPool.OpenAppPool(name.Trim());
                }
            }

            // create new app pool
            DirectoryEntry appPool = Service.Children.Add(name, "IIsApplicationPool");
            appPool.CommitChanges();
            Service.CommitChanges();
            
            return new IISAppPool(appPool);
        }

        /// <summary>
        /// if the app pool specified exsit
        /// </summary>
        /// <param name="name">name of app pool</param>
        /// <returns>true if exsit, otherwise false</returns>
        public static bool Exsit(string name)
        {
            DirectoryEntry Service = new DirectoryEntry("IIS://localhost/W3SVC/AppPools");
            foreach (DirectoryEntry entry in Service.Children)
            {
             
                if (entry.Name.Trim().ToLower() == name.Trim().ToLower())
                {
                    return true;
                }
               
            }
            return false;
           
        }

        /// <summary>
        /// Delete an app pool
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool DeleteAppPool(string name)
        {
            if (IISAppPool.Exsit(name) == false)
            {
                return false;
            }

            IISAppPool appPool = IISAppPool.OpenAppPool(name);
            appPool._entry.DeleteTree();
            return true;
        }     
    }
}
