#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript.DataVisualization 
{
    public class NavigatorDataSourceBuilder
    {
         
        private NavigatorDataSource chartDataSource = new NavigatorDataSource();

        public NavigatorDataSourceBuilder(NavigatorDataSource chartDataSource)
        {
            this.chartDataSource = chartDataSource;
        }

        public NavigatorDataSourceBuilder Data(Action<DataSourceBuilder> dataSource)
        {
            var ds = new DataSource();
            this.chartDataSource.Data = ds;
            var builder = new DataSourceBuilder(ds);
            if (dataSource != null)
                dataSource.Invoke(builder);
            return this;
        }
        public NavigatorDataSourceBuilder Data(DataSource dataSource)
        {
            this.chartDataSource.Data= dataSource;
            return this;
        }

        public NavigatorDataSourceBuilder Data(IEnumerable dataSource)
        {
            this.chartDataSource.Data = dataSource;
            return this;
        }

        public NavigatorDataSourceBuilder XName(string xName)
        {
            this.chartDataSource.XName = xName;
            return this;
        }


        public NavigatorDataSourceBuilder YName(string yNames)
        {
            this.chartDataSource.YName = yNames;
            return this;
        }
        public NavigatorDataSourceBuilder Query(string query)
        {
            this.chartDataSource.Query = query;
            return this;
        }

    }
}
