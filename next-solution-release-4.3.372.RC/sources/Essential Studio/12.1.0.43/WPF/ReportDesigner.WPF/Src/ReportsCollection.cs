//-------------------------------------------------------------------------------------------------
// <copyright file="ReportsCollection.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syncfusion.RDL.DOM;
using System.Net;

namespace Syncfusion.Windows.Reports.Designer
{
    /// <summary>
    /// Reports Collection class contains list of ReportDefinition Information for each tab
    /// </summary>
    class ReportsCollection : List<ReportInformation>
    { 
    }

    /// <summary>
    /// ReportDefinition Information for each tab
    /// </summary>
    class ReportInformation
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportInformation"/> class.
        /// </summary>
        public ReportInformation()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the report settings.
        /// </summary>
        /// <value>The report settings.</value>
        public ReportDefinition Report
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public string ReportName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Report URL.
        /// </summary>
        /// <value>The Report URL.</value>
        public string ReportPath
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Report Server.
        /// </summary>
        /// <value>The Report Server.</value>
        public string ReportServer
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Report Server Crendential.
        /// </summary>
        /// <value>The Report Server.</value>
        public ICredentials ReportServerCredential
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        public double Key
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the report settings dirty.
        /// </summary>
        /// <value>The report settings dirty.</value>
        public string DirtyReport
        {
            get;
            set;
        }

        #endregion
    }
}
