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
using System.Collections;
using System.Data;
using System.Threading.Tasks;
using Syncfusion.JavaScript.DataSources;
using System.Drawing;
using System.ComponentModel;
using Syncfusion.JavaScript.DataVisualization.Models;


namespace Syncfusion.JavaScript.DataVisualization
{
   public class ChartDataSourceBuilder
    {
        private ChartDataSource chartDataSource = new ChartDataSource();

        public ChartDataSourceBuilder(ChartDataSource chartDataSource)
        {
            this.chartDataSource = chartDataSource;
        }

        public ChartDataSourceBuilder Data(Action<DataSourceBuilder> dataSource)
        {
            var ds = new DataSource();
            this.chartDataSource.Data = ds;
            var builder = new DataSourceBuilder(ds);
            if (dataSource != null)
                dataSource.Invoke(builder);
            return this;
        }
        public ChartDataSourceBuilder Data(DataSource dataSource)
        {
            this.chartDataSource.Data= dataSource;
            return this;
        }

        public ChartDataSourceBuilder Data(IEnumerable dataSource)
        {
            this.chartDataSource.Data = dataSource;
            return this;
        }

        public ChartDataSourceBuilder XName(string xName)
        {
            this.chartDataSource.XName = xName;
            return this;
        }
		public ChartDataSourceBuilder High(string high)
        {
            this.chartDataSource.High = high;
            return this;
        }

        public ChartDataSourceBuilder Low(string low)
        {
            this.chartDataSource.Low = low;
            return this;
        }

        public ChartDataSourceBuilder Open(string open)
        {
            this.chartDataSource.Open = open;
            return this;
        }

        public ChartDataSourceBuilder Close(string close)
        {
            this.chartDataSource.Close = close;
            return this;
        }

        public ChartDataSourceBuilder Size(string size)
        {
            this.chartDataSource.Size = size;
            return this;
        }


        public ChartDataSourceBuilder YName(string yName)
        {
            this.chartDataSource.YName = yName;
            return this;
        }
        public ChartDataSourceBuilder Query(string query)
        {
            this.chartDataSource.Query = query;
            return this;
        }

    }
}
