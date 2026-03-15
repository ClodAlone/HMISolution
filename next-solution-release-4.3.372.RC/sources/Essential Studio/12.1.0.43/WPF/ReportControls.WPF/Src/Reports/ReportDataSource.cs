//-------------------------------------------------------------------------------------------------
// <copyright file="ReportDataSource.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

#if WINRT
namespace Syncfusion.UI.Xaml.Reports
#elif MVC
namespace Syncfusion.Reports.Mvc
#else
namespace Syncfusion.Windows.Reports
#endif
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Collection of Data Sources.
    /// </summary>
    public class ReportDataSourceCollection : List<ReportDataSource>
    {
    }

    /// <summary>
    /// Custom datasource of type DataSource
    /// </summary>
    public class ReportDataSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataSource"/> class.
        /// </summary>
        public ReportDataSource()
        {

        }
#if WINRT
        /// <summary>
        /// Initilizing the constructor
        /// </summary>
        /// <param name="name">Name of the dataset</param>
        /// <param name="value">Datasource</param>
        public ReportDataSource(string name, IEnumerable value)
        {
            this.Name = name;
            this.Value = value;
        }
#else

        /// <summary>
        /// Initilizing the constructor
        /// </summary>
        /// <param name="name">Name of the dataset</param>
        /// <param name="value">Datasource</param>
        public ReportDataSource(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }
#endif

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

#if WINRT
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public IEnumerable Value { get; set; }
#else
           /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value { get; set; }
#endif
    }

    public class ReportServerFormsCredential
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public ReportServerFormsCredential(string userName, string Password)
        {
            this.UserName = userName;
            this.Password = Password;
        }
    }
}