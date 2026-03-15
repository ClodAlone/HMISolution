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
using Syncfusion.Olap.Manager;
using Syncfusion.Olap.Engine;
using System.Web.Script.Serialization;
using System.Web;

namespace Syncfusion.JavaScript.Olap
{
    public class OlapGauge: Syncfusion.JavaScript.Control 
    {
        #region WrapperClass
        public OlapGaugeProperties OlapGaugeModel
        {
            get;
            set;
        }
        public override string TagName
        {
            get
            {
                return "div";
            }
        }
        public override string PluginName
        {
            get { return "ejOlapGauge"; }
        }
        protected override object Model
        {
            get { return this.OlapGaugeModel; }
        }

        public OlapGauge() { }
        public OlapGauge(String id, OlapGaugeProperties propModel)
        {
            this.ID = id;
            this.OlapGaugeModel = propModel;
        }

        public override HtmlString CreateContainer(string controlId)
        {
            StringBuilder tag = new StringBuilder();

            tag.Append("<")
               .Append(TagName)
               .Append(" id=\"")
               .Append(controlId + "\"")
               .Append("></")
               .Append(TagName)
               .Append(">");
            return new HtmlString(String.Format(tag.ToString()));
        }
        #endregion

        PivotEngine pivotEngine = new PivotEngine();
        KpiInfoCollection kpiInfo = new KpiInfoCollection();
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        public Dictionary<string, object> GetJsonData(string action, OlapDataManager DataManager)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            this.pivotEngine = DataManager.ExecuteOlapTable();
            if (this.pivotEngine != null)
                this.kpiInfo = this.pivotEngine.GetValidKpis();

            if (this.kpiInfo.Count > 0)
            {
                dict.Add("PivotRecords", serializer.Serialize(this.kpiInfo));
                dict.Add("OlapReport", Utils.SerializeOlapReport(DataManager.CurrentReport));
            }
            else
            {
                var ab = new List<object>();
                PivotColumnDescriptor tableColumns = this.pivotEngine.TableColumns[this.pivotEngine.TableColumns.Count - 1];
                ab.Add(tableColumns.Cells.Where(m => m.Value!= string.Empty).Select(m => new
                { Caption = m.CellCaption, Object = m.CellObject, CellIndex = m.CellIndex, Type = m.CellType, Measure = m.CellData.Measure,
                ClassName = m.ClassName, DoubleValue = m.DoubleValue, ExpandState = m.ExpandableState, HasChildren = m.HasChildren, Level = m.Level, RowIndex = m.PivotRowIndex,
                Range = m.Range, Span = m.SpanCell, Tag = m.Tag, UniqueName = m.UniqueName, Value = m.Value}));
                dict.Add("PivotRecords", serializer.Serialize(ab[0]));
                dict.Add("OlapReport", Utils.SerializeOlapReport(DataManager.CurrentReport));
            }
            return dict;
        }
    }
}
