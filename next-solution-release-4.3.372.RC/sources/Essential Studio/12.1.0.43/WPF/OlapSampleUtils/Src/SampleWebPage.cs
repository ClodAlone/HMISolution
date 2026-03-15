#region Copyright

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 

#endregion

using System;
using System.IO;
using System.Web.UI;
using Syncfusion.Olap.Common;
using Syncfusion.Olap.DataProvider;
using Syncfusion.Olap.Manager;

namespace SampleUtils
{
    /// <summary>
    ///     WebSample BaseWebPage
    /// </summary>
    public class SampleWebPage : Page
    {
        protected override void OnInitComplete(EventArgs e)
        {
            base.OnInitComplete(e);
            Page.Error += Page_Error;
        }

        private void Page_Error(object sender, EventArgs e)
        {
            ShowExceptionMessage(Server.GetLastError());
        }

        #region Properties

        /// <summary>
        /// Gets or sets the Data Manager.
        /// </summary>
        public static OlapDataManager DataManager { get; set; }

        /// <summary>
        /// Gets or sets the sample source object <see cref="SampleSource"/>.
        /// </summary>
        public SampleSource SampleSource { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <returns>A connection string.</returns>
        /// <exception cref="Exception">Throws an exception.</exception>
        public string GetConnectionString()
        {
            if (Request.PhysicalApplicationPath != null)
            {
                string configFilePath =
                    Request.PhysicalApplicationPath.Substring(0,
                                                              Request.PhysicalApplicationPath.ToLower()
                                                                     .LastIndexOf(@"\web\", StringComparison.Ordinal) +
                                                              1) + @"Config\OLAPSample.config";
                string samplePath = Request.PhysicalApplicationPath.Substring(0,
                                                                              Request.PhysicalApplicationPath.ToLower()
                                                                                     .LastIndexOf(@"\web\",
                                                                                                  StringComparison
                                                                                                      .Ordinal) + 1);
                if (configFilePath == string.Empty || !File.Exists(configFilePath))
                {
                    throw new Exception("Config file not found");
                }
                var source = Common.XmlToFromFile(configFilePath, typeof (SampleSource)) as SampleSource;
                if (source == null)
                {
                    throw new Exception("Error occurred while loading the sample configuration file");
                }
                return new SampleSourceHelper(source, samplePath, Platform.ASP).ConnectionString;
            }
            return string.Empty;
        }

        /// <summary>
        /// Shows exception message.
        /// </summary>
        /// <param name="exceptionMessage">Exception message.</param>
        /// <exception cref="Exception">Throws the inner exception.</exception>
        public void ShowExceptionMessage(Exception exceptionMessage)
        {
            if (exceptionMessage is DataProviderException && exceptionMessage.InnerException != null)
                throw new Exception(
                    "OLAP Cube cannot be accessed. \n\nPossible Causes are \n\n1. An application is already in execution referring same cube file path. \n2. More than one ASP.NET Development Server(server that runs ASP.NET applications locally) runs referring same cube file path \n3. Server name specified in the \"Dashboard -> Add-ons -> Business Intelligence -> Configure Sample Data source\" is not running or accessible");
        }

        /// <summary>
        /// Gets the connection string for Transational View demo sample.
        /// </summary>
        /// <returns>A connection string.</returns>
        public string ConnectionStringforTransactionView()
        {
            if (Request.PhysicalApplicationPath != null)
                return "Data Source=" +
                       Request.PhysicalApplicationPath.Substring(0,
                                                                 Request.PhysicalApplicationPath.ToLower()
                                                                        .IndexOf("\\web",
                                                                                 StringComparison.Ordinal)) +
                       "\\Common\\Data\\OfflineCube\\Sales DB.cub;Provider=msolap;";

            return string.Empty;
        }

        #endregion
    }
}