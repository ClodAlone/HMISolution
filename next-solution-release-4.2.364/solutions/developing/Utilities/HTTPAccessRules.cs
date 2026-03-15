using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class HTTPAccessRules
    {
        #region Declarations
        readonly string[] urls;
        readonly string user;

        int urlsCounter;
        List<string> parsedBaseUrls;
        List<int> parsedPortNumbers;
        List<string> parsedApplicationNames;

        /// <summary>
        /// The default user name for the access rules
        /// </summary>
        const string deafultUserName = "BUILTIN\\Users";

        /// <summary>
        /// The URI scheme for the HTTP protocol. 
        /// </summary>
        const string UriSchemeHttp = "http";

        /// <summary>
        /// The URI scheme for the HTTPS protocol. 
        /// </summary>
        const string UriSchemeHttps = "https";

        /// <summary>
        /// The URI scheme for using HTTP protocol without any security. 
        /// </summary>
        const string UriSchemeNoSecurityHttp = "nosecurityhttp";
        #endregion

        #region Constructors
        public HTTPAccessRules(string url) : 
            this (new string[] { url })
        { }

        public HTTPAccessRules(string url, string user) :
            this(new string[] { url }, user)
        { }

        public HTTPAccessRules(string[] urls) : 
            this (urls, deafultUserName)
        { }

        public HTTPAccessRules(string[] urls, string user)
        {
            this.urls = urls;
            if (user == null)
                this.user = deafultUserName;
            else
                this.user = user;
        }
        #endregion

        #region Public Methods
        public void CheckUrlsAndAddAccessRules()
        {
            if (TryParseUrls())
            {
                AddAccessRules();
            }
        }

        public void ValidateUrlsAndAddAccessRules()
        {
            ParseUrls();
            AddAccessRules();
        }
        #endregion

        #region Private Methods
        bool TryParseUrls()
        {
            ParseInitilization();

            foreach (var url in urls)
            {
                try
                {
                    ParseUrl(url);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("HTTPAccessRules : Invalid url '{0}'.", url);
                }
            }

            return parsedBaseUrls.Count > 0;
        }

        void ParseUrls()
        {
            ParseInitilization();

            foreach (var url in urls)
                ParseUrl(url);
        }

        void ParseInitilization()
        {
            if (parsedBaseUrls == null)
                parsedBaseUrls = new List<string>();
            else
                parsedBaseUrls.Clear();

            if (parsedPortNumbers == null)
                parsedPortNumbers = new List<int>();
            else
                parsedPortNumbers.Clear();

            if (parsedApplicationNames == null)
                parsedApplicationNames = new List<string>();
            else
                parsedApplicationNames.Clear();
        }

        void ParseUrl(string url)
        {
            var indexBaseUrl = url.IndexOf(':');
            if (indexBaseUrl == -1)
                throw new ArgumentException("Missing base url.");
            var indexPortNumber = url.IndexOf(':', indexBaseUrl + 1);
            if (indexPortNumber == -1)
                throw new ArgumentException("Missing port number.");
            var indexAppName = url.IndexOf('/', indexPortNumber + 1);
            //if (indexAppName == -1)
            //    throw new ArgumentException("Missing application name.");

            string port;
            if (indexAppName != -1)
                port = url.Substring(indexPortNumber + 1, indexAppName - indexPortNumber - 1);
            else
                port = url.Substring(indexPortNumber + 1);
            int portNumber;
            if (!int.TryParse(port, out portNumber))
                throw new ArgumentException("Invalid port number.");

            parsedBaseUrls.Add(url.Substring(0, indexBaseUrl));
            parsedPortNumbers.Add(portNumber);
            if (indexAppName != -1)
                parsedApplicationNames.Add(url.Substring(indexAppName + 1));
            else
                parsedApplicationNames.Add(String.Empty);
            urlsCounter += 1;
        }

        void AddAccessRules()
        {
            for (int ii = 0; ii < urlsCounter; ii++)
            {
                if (parsedBaseUrls[ii] == UriSchemeNoSecurityHttp)
                {
                    AddNoSecurityHttpAccessRules(parsedPortNumbers[ii], parsedApplicationNames[ii]);
                }
                if (parsedBaseUrls[ii] == UriSchemeHttp)
                {
                    AddHttpAccessRules(parsedPortNumbers[ii], parsedApplicationNames[ii]);
                }
                if (parsedBaseUrls[ii] == UriSchemeHttps)
                {
                    AddHttpsAccessRules(parsedPortNumbers[ii], parsedApplicationNames[ii]);
                }
            }
        }

        void AddNoSecurityHttpAccessRules(int port, string appname)
        {
            var url = String.Format("https://+:{0}/", port);
            Utilities.HttpHelper.NetshCommands.UnRegisterUrl(url);
            url = String.Format("http://+:{0}/", port);
            Utilities.HttpHelper.NetshCommands.RegisterUrl(url, user);
        }

        void AddHttpAccessRules(int port, string appname)
        {
            var url = String.Format("https://+:{0}/", port);
            Utilities.HttpHelper.NetshCommands.UnRegisterUrl(url);
            if (!String.IsNullOrEmpty(appname))
                url = String.Format("http://+:{0}/{1}/", port, appname);
            else
                url = String.Format("http://+:{0}/", port);
            Utilities.HttpHelper.NetshCommands.RegisterUrl(url, user);
        }

        void AddHttpsAccessRules(int port, string appname)
        {
            string url;
            if (!String.IsNullOrEmpty(appname))
                url = String.Format("http://+:{0}/{1}/", port, appname);
            else
                url = String.Format("http://+:{0}/", port);
            Utilities.HttpHelper.NetshCommands.UnRegisterUrl(url);
            url = String.Format("https://+:{0}/", port);
            Utilities.HttpHelper.NetshCommands.RegisterUrl(url, user);
        }
        #endregion
    }
}
