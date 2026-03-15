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
using System.Threading.Tasks;
using System.ComponentModel;
using Syncfusion.JavaScript.Shared.Serializer;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript;
using System.Collections;
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript.DataVisualization
{
    public class FieldsBuilder
    {
        private Fields fields; // = new Fields();
        private BulletGraphProperties bg_model;

        public FieldsBuilder(Fields fieldOptions, BulletGraph bullet)
        {
            this.fields = fieldOptions;
            this.bg_model = bullet.BulletGraphModel;
            this.bg_model.Fields = fieldOptions;
        }

        public FieldsBuilder Datasource(Action<DataSourceBuilder> dataSource)
        {
            var ds = new DataSource();
            this.fields.DataSource = ds;
            var builder = new DataSourceBuilder(ds);
            if (dataSource != null)
                dataSource.Invoke(builder);
            return this;
        }
        public FieldsBuilder Datasource(DataSource dataSource)
        {
            this.fields.DataSource = dataSource;
            return this;
        }

        public FieldsBuilder Datasource(IEnumerable dataSource)
        {
            this.fields.DataSource = dataSource;
            return this;
        }

        public FieldsBuilder TableName(String tableName)
        {
            this.fields.TableName = tableName;
            return this;
        }

        public FieldsBuilder Query(String query)
        {
            this.fields.Query = query;
            return this;
        }

        public FieldsBuilder Category(String category)
        {
            this.fields.CategoryField = category;
            return this;
        }

        public FieldsBuilder FeatureMeasure(String featureMeasure)
        {
            this.fields.FeatureMeasureField = featureMeasure;
            return this;
        }

        public FieldsBuilder ComparativeMeasure(String compareMeasure)
        {
            this.fields.ComparativeMeasureField = compareMeasure;
            return this;
        }
    }
}
