using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Web.Administration;
using System.Linq;
using IIS7Manager.Exceptions;

namespace IIS7Manager
{
    #region Nested Enumerators
    public enum IISWebsiteStatus : int
    {
        Starting = 0,
        Started = 1,
        Stopping = 2,
        Stopped = 3,
        Unknown = 4,
    }
    #endregion

    /// <summary>
    /// IISWebsite is a class to manage websites on a IIS server. You can not create
    /// a IISWebsite instance directly with a IISWebsite constructor.
    /// 
    /// To get a IISWebsite instance, call IISWebsite.OpenWebsite to open an existing website or
    /// IISWebsite.CreateWebsite to create a new website. 
    /// 
    /// Both of these two methods would return a instance of IISWebsite so that you can use
    /// this instance to handle your website.
    /// </summary>
    public class IISWebsite
    {
        #region Declarations
        ServerManager serverMgr = null;
        private Site websiteEntry = null;
        internal const string IIsWebServer = "IIsWebServer";
        #endregion

        #region Constructors
        /// <summary>
        /// Protect this constructor so that user can not create a IISWebsite instance directly.
        /// To get IISWebsite instance, please use IISWebsite.OpenWebsite or IISWebsite.CreateWebsite
        /// </summary>
        protected IISWebsite(Site Server)
        {
            websiteEntry = Server;
        }

        /// <summary>
        /// Protect this constructor so that user can not create a IISWebsite instance directly.
        /// To get IISWebsite instance, please use IISWebsite.OpenWebsite or IISWebsite.CreateWebsite
        /// </summary>
        /// <param name="Server"></param>
        protected IISWebsite(ServerManager manager, Site Server)
        {
            serverMgr = manager;
            websiteEntry = Server;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// get or set name of this website
        /// </summary>
        public string Name
        {
            get
            {
                return this.websiteEntry.Name;
            }
            set
            {
                this.websiteEntry.Name = value;
            }
        }

        /// <summary>
        /// get or set website port
        /// </summary>
        public int HttpPort
        {
            get
            {
                Binding binding = websiteEntry.Bindings.Where(b => b.Protocol == "http").FirstOrDefault();
                return binding.EndPoint.Port;
            }
            set
            {
                Binding binding = websiteEntry.Bindings.Where(b => b.Protocol == "http").FirstOrDefault();
                binding.EndPoint.Port = value;
            }
        }

        /// <summary>
        /// Root path
        /// </summary>
        /// 
        public IISWebApplication Root
        {
            get
            {
                try
                {
                    Application application = websiteEntry.Applications.First(a => a.Path == "/");
                    return new IISWebApplication(application);
                }
                catch 
                {
                    throw new WebsiteWithoutRootException(this.Name);
                }
            }
        }

        /// <summary>
        /// Get iis website Status
        /// </summary>
        public IISWebsiteStatus Status
        {
            get
            {
                ObjectState status = this.websiteEntry.State;
                return (IISWebsiteStatus)status;
            }
        }
        #endregion

        #region Operations
        /// <summary>
        /// Start this website
        /// </summary>
        public void Start()
        {
            this.websiteEntry.Start();
        }

        /// <summary>
        /// Stop this website
        /// </summary>
        public void Stop()
        {
            this.websiteEntry.Stop();
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

        /// <summary>
        /// Check whether an application exists.
        /// </summary>
        /// <param name="name">Name of app checked</param>
        /// <returns>true if exist. Otherwise false.</returns>
        public bool ExistApplication(string name)
        {
            // add back slash if necessary
            if (!name.StartsWith("/"))
                name = String.Format("/{0}", name);

            return (from c in this.websiteEntry.Applications where c.Path == name select c).ToList().Count > 0;
        }

        public IISWebApplication CreateApplication(string name, string path)
        {
            // add back slash if necessary
            if (!name.StartsWith("/"))
                name = String.Format("/{0}", name);

            return CreateApplication(name, path, null);
        }

        public IISWebApplication CreateApplication(string name, string path, string appPool)
        {
            // add back slash if necessary
            if (!name.StartsWith("/"))
                name = String.Format("/{0}", name);

            // already exist
            if (this.ExistApplication(name))
            {
                throw new ApplicationAlreadyExistException(this.OpenApplication(name), path);
            }

            // validate path
            if (System.IO.Directory.Exists(path) == false)
            {
                throw new DirNotFoundException(path);
            }

            ServerManager serverMgr = new ServerManager();
            Application application = this.websiteEntry.Applications.Add(name, path);
            if (!string.IsNullOrEmpty(appPool))
                application.ApplicationPoolName = appPool;
            serverMgr.CommitChanges();

            return new IISWebApplication(application);
        }

        /// <summary>
        /// Open an application
        /// </summary>
        /// <param name="name">Name of application to be opened. Case insensitive.</param>
        /// <returns>A IISWebApplication instance if open successfully done.Otherwise null.</returns>
        public IISWebApplication OpenApplication(string name)
        {
            // add back slash if necessary
            if (!name.StartsWith("/"))
                name = String.Format("/{0}", name);

            var list = (from c in this.websiteEntry.Applications where c.Path == name select c).ToList();
            if (list.Count > 0)
                return new IISWebApplication(list[0]);

            return null;
        }

        ///// <summary>
        ///// Enumerate applications
        ///// </summary>
        ///// <returns></returns>
        public string[] EnumApplications()
        {
            List<string> ret = new List<string>();
            foreach (Application app in this.websiteEntry.Applications)
            {
                ret.Add(app.Path);
            }

            return ret.ToArray();
        }

        /// <summary>
        /// Delete an application
        /// </summary>
        /// <param name="name">Name of the application to be deleted</param>
        /// <returns>true if successfully deleted. Otherwise false.</returns>
        public bool DeleteApplication(string name)
        {
            // add back slash if necessary
            if (!name.StartsWith("/"))
                name = String.Format("/{0}", name);

            ServerManager serverMgr = new ServerManager();
            Application application = this.websiteEntry.Applications.Where(c => c.Path == name).SingleOrDefault();
            if (application != null)
            {
                websiteEntry.Applications.Remove(application);
                serverMgr.CommitChanges();
                return true;
            }

            return false;
        }
        #endregion Operations

        #region Static Methods
        /// <summary>
        /// create a new website
        /// </summary>
        /// <param name="name">website name</param>
        /// <param name="port">website port</param>
        /// <param name="rootPath">root path</param>
        /// <returns></returns>
        public static IISWebsite CreateWebsite(string name, int port, string rootPath)
        {
            return IISWebsite.CreateWebsite(name, port, rootPath, null);
        }

        public static IISWebsite CreateWebsite(string name, int port, string rootPath, string appPool)
        {
            // validate root path
            if (System.IO.Directory.Exists(rootPath) == false)
            {
                throw new DirNotFoundException(rootPath);
            }

            ServerManager serverMgr = new ServerManager();
            foreach (var site in serverMgr.Sites)
            {
                if (site.Name == name)
                    throw new Exception(String.Format("website: {0} already exsit.", name));
            }

            Site website = serverMgr.Sites.Add(name, rootPath, port);
            //Application application = website.Applications.Add(website.ApplicationDefaults.Schema.Name, rootPath);
            //application.VirtualDirectories.Add(application.VirtualDirectoryDefaults.Schema.Name, rootPath);
            // create application pool
            if (!string.IsNullOrEmpty(appPool))
            {
                serverMgr.ApplicationPools.Add(appPool);
                website.ApplicationDefaults.ApplicationPoolName = appPool;
            }

            // commit changes
            serverMgr.CommitChanges();

            // return the newly created website
            return new IISWebsite(serverMgr, website);
        }

        /// <summary>
        /// open a website object
        /// </summary>
        /// <param name="name">name of the website to be opened</param>
        /// <returns>IISWebsite object</returns>
        public static IISWebsite OpenWebsite(string name)
        {
            ServerManager serverMgr = new ServerManager();
            foreach (var site in serverMgr.Sites)
            {
                if (site.Name == name)
                    return new IISWebsite(serverMgr, site);
            }

            return null;
        }

        /// <summary>
        /// Get exsited websites
        /// </summary>
        public static string[] ExistedWebsites
        {
            get
            {
                List<string> ret = new List<string>();

                ServerManager serverMgr = new ServerManager();
                foreach (var site in serverMgr.Sites)
                {
                    ret.Add(site.Name);
                }

                return ret.ToArray();
            }
        }

        /// <summary>
        /// Delete a website service
        /// </summary>
        /// <param name="name">the name of the website</param>
        public static bool DeleteWebsite(string name)
        {
            IISWebsite website = IISWebsite.OpenWebsite(name);
            if (website == null)
            {
                return false;
            }

            ServerManager serverMgr = new ServerManager();
            serverMgr.Sites.Remove(website.websiteEntry);
            serverMgr.CommitChanges();

            return true;
        }

        public static bool Exist(string name)
        {
            try
            {
                ServerManager serverMgr = new ServerManager();
                return (from c in serverMgr.Sites where c.Name == name select c).ToList().Count > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool IsIIS7Available()
        {
            try
            {
                ServerManager serverMgr = new ServerManager();
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
        #endregion Static Methods
    }
}
