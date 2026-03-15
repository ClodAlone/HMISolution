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
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Syncfusion.RDL.DOM
{
    public class DataSets : List<DataSet>
    {
    }

    public class DataSet
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }
        public Fields Fields { get; set; }
        public Query Query { get; set; }

        [DefaultValue(BooleanOptions.Auto)]
        public BooleanOptions CaseSensitivity { get; set; }
        public string Collation { get; set; }

        [DefaultValue(BooleanOptions.Auto)]
        public BooleanOptions AccentSensitivity { get; set; }
        [DefaultValue(BooleanOptions.Auto)]
        public BooleanOptions KanatypeSensitivity { get; set; }
        [DefaultValue(BooleanOptions.Auto)]
        public BooleanOptions WidthSensitvity { get; set; }
        public Filters Filters { get; set; }
        public SharedDataSet SharedDataSet { get; set; }
        [DefaultValue(BooleanOptions.Auto)]
        public BooleanOptions InterpretSubtotalsAsDetails { get; set; }

        [XmlElement(Namespace = "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner")]
        public DataSetInfo DataSetInfo { get; set; }

        public object Clone()
        {
            DataSet dataSet = new DataSet();
            dataSet.Update(this);
            return dataSet;
        }

        internal void Update(DataSet dataSet)
        {
            this.Name = dataSet.Name;
            this.Collation = dataSet.Collation;
            this.AccentSensitivity = dataSet.AccentSensitivity;
            this.CaseSensitivity = dataSet.CaseSensitivity;
            this.KanatypeSensitivity = dataSet.KanatypeSensitivity;
            this.WidthSensitvity = dataSet.WidthSensitvity;
            this.SharedDataSet = dataSet.SharedDataSet;
            this.InterpretSubtotalsAsDetails = dataSet.InterpretSubtotalsAsDetails;
            this.DataSetInfo = dataSet.DataSetInfo;

            if (dataSet.Query != null)
                this.Query = new DOM.Query(dataSet.Query);

            this.Fields = new Fields();
            if (dataSet.Fields != null)
            {
                this.Fields.AddRange(dataSet.Fields.Clone());
            }

            this.Filters = new Filters();
            if (dataSet.Filters != null)
            {
                this.Filters.AddRange(dataSet.Filters.Clone());
            }
        }

        [XmlIgnore]
        public string DataSetObject { get; set; }

        public bool ShouldSerializeFields()
        {
            return this.Fields != null && this.Fields.Count > 0;
        }

        public void ResetFields()
        {
            this.Fields = new Fields();
        }

        public bool ShouldSerializeFilters()
        {
            return Filters != null && Filters.Count > 0;
        }

        public void ResetFilters()
        {
            this.Filters = new Filters();
        }

        public bool ShouldSerializeDataSetInfo()
        {
            return DataSetInfo != null;
        }

        public void ResetDataSetInfo()
        {
            this.DataSetInfo = null;
        }
    }

    public class DataSetInfo
    {
        public string DataSetName { get; set; }
        public string ObjectDataSourceSelectMethod { get; set; }
        public string ObjectDataSourceSelectMethodSignature { get; set; }
        public string ObjectDataSourceType { get; set; }
        public string SchemaPath { get; set; }
        public string TableAdapterFillMethod { get; set; }
        public string TableAdapterGetDataMethod { get; set; }
        public string TableAdapterName { get; set; }
        public string TableName { get; set; }
    }
}
