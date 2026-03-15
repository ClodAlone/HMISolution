using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.DirectoryServices;
using IISManager.Exceptions;

namespace IISManager
{
    public enum IISWebsiteStatus : int
    {
        Starting = 1,
        Started = 2,
        Stopping = 3,
        Stopped = 4,
        Pausing = 5,
        Paused = 6,
        Continuign = 7
    }

    /// <summary>
    /// IISWebsite is a class to manage websites on a IIS server. You can not create
    /// a IISWebsite instance directly with a IISWebsite constructor.
    /// 
    /// To get a IISWebsite instance, call IISWebsite.OpenWebsite to open an existing website or
    /// IISWebsite.CreateWebsite to create a new website. 
    /// 
    /// Both of these two methods would return a instance of IISWebsite so that you can use
    /// this instance to handle your website.
    /// 
    /// Author: luckzj
    /// Time  : 27/June. 2010
    /// contact: luckzj12@163.com
    /// website: http://www.soft-bin.com
    /// copy right (c) luckzj. All rights reserved
    /// </summary>
    public class IISWebsite
    {
        // website entry
        private DirectoryEntry websiteEntry = null;
        internal const string IIsWebServer = "IIsWebServer";

        /// <summary>
        /// Protect this constructor so that user can not create a IISWebsite instance directly.
        /// To get IISWebsite instance, please use IISWebsite.OpenWebsite or IISWebsite.CreateWebsite
        /// </summary>
        /// <param name="Server"></param>
        protected IISWebsite(DirectoryEntry Server)
        {
            websiteEntry = Server;
        }


        #region Properties

        /// <summary>
        /// get or set name of this website
        /// </summary>
        public string Name
        {
            get
            {
                return this.websiteEntry.Properties["ServerComment"][0].ToString();
            }
            set
            {
                this.websiteEntry.Properties["ServerComment"][0] = value;
                this.websiteEntry.CommitChanges();
            }
        }

        /// <summary>
        /// get or set website port
        /// </summary>
        public int Port
        {
            get
            {
                string port = this.websiteEntry.Properties["Serverbindings"][0].ToString();
                port = port.Substring(1);
                port = port.Remove(port.Length - 1, 1);
                return Convert.ToInt32(port);
            }
            set
            {
                this.websiteEntry.Properties["Serverbindings"][0] = ":" + value + ":";
                this.websiteEntry.CommitChanges();
            }
        }

        /// <summary>
        /// Root path
        /// </summary>
        public IISWebVirturalDir Root
        {
            get
            {
                foreach (DirectoryEntry entry in websiteEntry.Children)
                {
                    if (entry.SchemaClassName == IISWebVirturalDir.IIsVirtualDir)
                    {
                        return new IISWebVirturalDir(entry);
                    }
                }

                throw new WebsiteWithoutRootException(this.Name);
            }
        }

        /// <summary>
        /// Get iis website Status
        /// </summary>
        public IISWebsiteStatus Status
        {
            get
            {
                object status = this.websiteEntry.InvokeGet("Status");
                return (IISWebsiteStatus)status;
            }
        }

        #endregion Properties

        #region Operations

        /// <summary>
        /// Start this website
        /// </summary>
        public void Start()
        {
            this.websiteEntry.Invoke("Start");
        }

        /// <summary>
        /// Stop this website
        /// </summary>
        public void Stop()
        {
            this.websiteEntry.Invoke("Stop");
        }

        /// <summary>
        /// Parse this website
        /// </summary>
        public void Parse()
        {
            this.websiteEntry.Invoke("Pause");
        }

        /// <summary>
        /// Continue to run this website.
        /// </summary>
        public void Continue()
        {
            this.websiteEntry.Invoke("Continue");
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

            // get directory service
            DirectoryEntry Services = new DirectoryEntry("IIS://localhost/W3SVC");

            // get server name (index)
            int index = 0;
            foreach (DirectoryEntry server in Services.Children)
            {
                if (server.SchemaClassName == "IIsWebServer")
                {
                    if (server.Properties["ServerComment"][0].ToString() == name)
                    {
                        throw new Exception("website:" + name + " already exsit.");
                    }

                    if (Convert.ToInt32(server.Name) > index)
                    {
                        index = Convert.ToInt32(server.Name);
                    }
                }
            }
            index++; // new index created

            // create website
            DirectoryEntry Server = Services.Children.Add(index.ToString(), IIsWebServer);
            Server.Properties["ServerComment"].Clear();
            Server.Properties["ServerComment"].Add(name);
            Server.Properties["Serverbindings"].Clear();
            Server.Properties["Serverbindings"].Add(":" + port + ":");

            // create ROOT for website
            DirectoryEntry root = Server.Children.Add("ROOT", IISWebVirturalDir.IIsVirtualDir);
            root.Properties["path"].Clear();
            root.Properties["path"].Add(rootPath);

            // create application
            if (string.IsNullOrEmpty(appPool))
            {
                root.Invoke("appCreate", 0);
            }
            else
            {
                // use application pool
                root.Invoke("appCreate3", 0, appPool, true);
            }

            root.Properties["AppFriendlyName"].Clear();
            root.Properties["AppIsolated"].Clear();
            root.Properties["AccessFlags"].Clear();
            root.Properties["FrontPageWeb"].Clear();
            root.Properties["AppFriendlyName"].Add(root.Name);
            root.Properties["AppIsolated"].Add(2);
            root.Properties["AccessFlags"].Add(513);
            root.Properties["FrontPageWeb"].Add(1);

            // commit changes
            root.CommitChanges();
            Server.CommitChanges();

            // return the newly created website
            IISWebsite website = new IISWebsite(Server);
            return website;
        }

        /// <summary>
        /// open a website object
        /// </summary>
        /// <param name="name">name of the website to be opened</param>
        /// <returns>IISWebsite object</returns>
        public static IISWebsite OpenWebsite(string name)
        {
            // get directory service
            DirectoryEntry Services = new DirectoryEntry("IIS://localhost/W3SVC");
            IEnumerator ie = Services.Children.GetEnumerator();
            DirectoryEntry Server = null;

            // find iis website
            while (ie.MoveNext())
            {
                Server = (DirectoryEntry)ie.Current;
                if (Server.SchemaClassName == "IIsWebServer")
                {
                    // "ServerComment" means name
                    if (Server.Properties["ServerComment"][0].ToString() == name)
                    {
                        return new IISWebsite(Server);
                        break;
                    }
                }
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

                // get directory service
                DirectoryEntry Services = new DirectoryEntry("IIS://localhost/W3SVC");
                IEnumerator ie = Services.Children.GetEnumerator();
                DirectoryEntry Server = null;

                // find iis website
                while (ie.MoveNext())
                {
                    Server = (DirectoryEntry)ie.Current;
                    if (Server.SchemaClassName == "IIsWebServer")
                    {
                        // "ServerComment" means name
                        ret.Add(Server.Properties["ServerComment"][0].ToString());
                      
                    }
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

            website.websiteEntry.DeleteTree();
            return true;
        }

        public static bool Exist(string name)
        {
            // get directory service
            DirectoryEntry Services = new DirectoryEntry("IIS://localhost/W3SVC");
            IEnumerator ie = Services.Children.GetEnumerator();
            DirectoryEntry Server = null;

            // find iis website
            while (ie.MoveNext())
            {
                Server = (DirectoryEntry)ie.Current;
                if (Server.SchemaClassName == "IIsWebServer")
                {
                    // "ServerComment" means name
                    if (Server.Properties["ServerComment"][0].ToString() == name)
                    {
                        return true;
                        break;
                    }
                }
            }

            return false;
        }

        public static bool IsIIS6Available()
        {
            try
            {
                DirectoryEntry Services = new DirectoryEntry("IIS://localhost/W3SVC");
                IEnumerator ie = Services.Children.GetEnumerator();
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
