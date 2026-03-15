#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if WINRT
namespace Syncfusion.UI.Xaml.Reports
#elif MVC
namespace Syncfusion.Reports.Mvc
#else
namespace Syncfusion.Windows.Reports
#endif
{
    public sealed class ReportDataSourceInfoCollection : List<ReportDataSourceInfo>
    {
        internal ReportDataSourceInfoCollection()
        {
        }

        public ReportDataSourceInfo this[string name]
        {
            get
            {
                if (this != null)
                {
                    var datasources = from dataSource in this
                                      where dataSource.Name.Equals(name)
                                      select dataSource;

                    if (datasources.Count() > 0)
                    {
                        return datasources.First();
                    }
                }

                return null;
            }
        }
    }

    public sealed class ReportDataSourceInfo
    {
        internal ReportDataSourceInfo()
        {
        }

        public string Name { get; set; }

        public string Prompt { get; set; }
    }

    public sealed class DataSourceCredentials
    {
        public bool IntegratedSecurity { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public string UserId { get; set; }
    }
}
