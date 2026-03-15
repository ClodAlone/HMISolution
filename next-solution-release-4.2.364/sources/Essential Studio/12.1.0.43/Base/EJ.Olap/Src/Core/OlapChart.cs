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
using Syncfusion.Olap.Engine;
using Syncfusion.Olap.Data;
using System.Globalization;
using Syncfusion.Olap.Manager;
using Syncfusion.Olap.Reports;
using Syncfusion.JavaScript.Shared;
using System.Web.UI;
using System.Web;
using Syncfusion.JavaScript.Olap.Models;
using Syncfusion.Olap.Common;

namespace Syncfusion.JavaScript.Olap
{
    public class OlapChart : Syncfusion.JavaScript.Control
    {
        #region wrapperclass
        public OlapChartProperties OlapChartModel
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
            get { return "ejOlapChart"; }
        }
        protected override object Model
        {
            get { return this.OlapChartModel; }
        }

        public OlapChart() { }
        public OlapChart(String id, OlapChartProperties propModel)
        {
            this.ID = id;
            this.OlapChartModel = propModel;
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

        OlapDataManager dataManager = new OlapDataManager();
        PivotEngine pivotEngine = new PivotEngine();
        List<Dictionary<string, string>> chartLables = new List<Dictionary<string, string>>();
        string drilledLevelCaption = string.Empty;
        string parentUniqueName = string.Empty;
        string drillAction = string.Empty;
        string parentCaption = string.Empty;
        string parentType = string.Empty;
        bool isDrilled = false;
        bool namedsetEnabled = false;
        string measureAxis = string.Empty;
        Item namedSet = new Item();
        int levelDepth = -1;

        private Dictionary<string, object> JsonData(string action, OlapDataManager dataManager, string drilledSeries)
        {
            this.dataManager = dataManager;
            Dictionary<string, object> dict = new Dictionary<string, object>();
            if (action == "initialize")
            {
                this.pivotEngine = this.dataManager.ExecuteOlapTable(GridLayout.NoSummaries);
                dict.Add("JsonRecords", LoadChartData(this.pivotEngine));
                dict.Add("OlapReport", Utils.SerializeOlapReport(dataManager.CurrentReport));
            }
            else if (action == "drilldown" || action == "drillup")
            {
                this.isDrilled = true; this.drillAction = action;
                dict.Add("JsonRecords", DrillChartData(drilledSeries));
                UpdateReportCollection(dataManager);
                dict.Add("OlapReport", Utils.SerializeOlapReport(dataManager.CurrentReport));
                dict.Add("ClientReports", Common.SerializeObject<OlapReportCollection>(dataManager.Reports).Compress());
            }
            return dict;
        }

        public Dictionary<string, object> GetJsonData(string action, OlapDataManager dataManager)
        {
            return (JsonData(action, dataManager, null));
        }

        public Dictionary<string, object> GetJsonData(string action, OlapDataManager dataManager, string drilledSeries)
        {
            return (JsonData(action, dataManager, drilledSeries));
        }

        private string DrillChartData(string e)
        {
            if (this.dataManager != null && this.dataManager.CurrentReport.ShowExpanders && !string.IsNullOrEmpty(e))
            {
                if (this.dataManager.ItemSource == null)
                {
                    string[] tempArgs = e.Split(new string[] { "::[" }, StringSplitOptions.None);
                    string[] args = new string[10]; int i = 0;
                    foreach (var arguments in tempArgs)
                    {
                        if (arguments.IndexOf("]::") > 1 && !arguments.StartsWith("["))
                        {
                            args[i++] = "[" + arguments.Split(new string[] { "]::" }, StringSplitOptions.None)[0] + "]";
                            args[i++] = arguments.Split(new string[] { "]::" }, StringSplitOptions.None)[1];
                            if (this.dataManager.CurrentReport.DrillType == Syncfusion.Olap.Reports.DrillType.DrillPosition && arguments.Split(new string[] { "]::" }, StringSplitOptions.None).Length > 2)
                            {
                                string[] drillPosArguments = arguments.Split(new string[] { "]::" }, StringSplitOptions.None)[2].Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
                                foreach (var _drillPosArguments in drillPosArguments)
                                    args[i++] = _drillPosArguments;
                            }
                        }
                        else
                            args[i++] = arguments;
                    }
                    if (args.Count() > 1)
                    {
                        Member memberObj = new Member();
                        memberObj.UniqueName = args[0];
                        memberObj.LevelUniqueName = args[1];
                        memberObj.Caption = args[2];
                        memberObj.ParentUniqueName = args[3];
                        memberObj.ParentCaption = args[4];
                        this.drilledLevelCaption = memberObj.Caption;
                        if (this.drillAction == "drillup")
                            this.parentUniqueName = memberObj.ParentUniqueName;
                        else
                            this.parentUniqueName = memberObj.UniqueName;
                        if (memberObj.ParentUniqueName.Split(new string[] { "[", "]", "." }, StringSplitOptions.RemoveEmptyEntries).Last().IndexOf("All") > -1)
                            this.parentType = "ALL MEMBERS";
                        if (this.dataManager.CurrentReport.DrillType == Syncfusion.Olap.Reports.DrillType.DrillPosition)
                        {
                            var positionInfo = new ObjectStateFormatter().Deserialize(args[5]) as List<PositionInfo>;
                            var cellType = (PivotCellDescriptorType)int.Parse(args[6]);
                            memberObj.ParentHierarchy = args[7];
                            var expandState = (ExpandableState)int.Parse(args[8]);
                            this.dataManager.ToggleExpandableStateOnDrillPosition(memberObj, cellType, positionInfo, expandState);
                        }
                        else
                            this.dataManager.ToggleExpandableState(PivotCellDescriptorType.RowHeader, memberObj, GridLayout.NoSummaries, true);
                        this.pivotEngine = this.dataManager.ExecuteOlapTable(GridLayout.NoSummaries);
                    }
                    else
                    {
                        if (this.pivotEngine == null)
                        {
                            CellSet cellSet = this.dataManager.ExecuteCellSet();
                            this.pivotEngine = this.dataManager.ExecuteOlapTable(cellSet, GridLayout.NoSummaries);
                        }
                    }
                }

            }
            return (LoadChartData(this.pivotEngine));
        }
        private OlapDataManager UpdateReportCollection(OlapDataManager DataManager)
        {
            int reporCount = 0;
            foreach (OlapReport olapReport in DataManager.Reports)
            {
                if (olapReport.Name == DataManager.CurrentReport.Name)
                {
                    DataManager.Reports[reporCount] = DataManager.CurrentReport;
                    break;
                }
                reporCount += 1;
            }
            return DataManager;
        }
        private string LoadChartData(PivotEngine pivotEngine)
        {
            Dictionary<string, object> chartData = new Dictionary<string, object>();
            List<List<double>> chartSeries = new List<List<double>>();
            List<string> seriesName = new List<string>();
            List<List<double>> chartSeriesPoints = new List<List<double>>();
            List<string> seriesTags = new List<string>();
            List<string> labels = new List<string>();
            List<string> labelTags = new List<string>();
            string measureUniqueValues=string.Empty;            
            string measureUniqueName = this.FindMeasureUniqueName(this.dataManager.CurrentReport);                
            if(!string.IsNullOrEmpty(measureUniqueName) && pivotEngine != null)   
            measureUniqueValues = string.Join("~", this.pivotEngine.TableColumns.SelectMany(m => m.Cells.Select(n => n).Where(n => n.CellCaption!= null && n.UniqueName!= null && n.UniqueName.In(measureUniqueName.Split('~'))).ToArray()).Select(k => k.CellCaption).Distinct().ToArray());

            if (pivotEngine != null)
            {
                GridRangeInfo valuesSection = GridRangeInfo.Cells(
                pivotEngine.HeaderSection.Bottom,
                pivotEngine.RowHeaderSection.Right + 1,
                pivotEngine.RowsCount,
                pivotEngine.TableColumns.Count);

                if (pivotEngine.TableColumns.Count > 0)
                {
                    for (int i = valuesSection.Left; i < valuesSection.Right; i++)
                    {
                        PivotColumnDescriptor column = pivotEngine.TableColumns[i];
                        List<double> chartPoints = new List<double>();
                        string legendText = string.Empty;
                        bool isFirstCell = true;
                        foreach (PivotCellDescriptor cell in column.Cells)
                        {
                            if (this.dataManager == null || this.dataManager.ItemSource == null)
                            {
                                if (cell.Tag is Cell)
                                {
                                    bool doDrillup = false;
                                    if (this.drillAction == "drillup")
                                    {
                                        foreach (CellHeaderInfo rowInfo in cell.CellData.RowInfo)
                                        {
                                            if (rowInfo.UniqueName == this.parentUniqueName)
                                                doDrillup = true;
                                            else if (rowInfo.Member.ParentUniqueName == this.parentUniqueName)
                                                doDrillup = true;
                                            //else if (namedsetEnabled && rowInfo.Member.LevelDepth == 1)
                                            //    doDrillup = true;
                                        }
                                    }
                                    if ((this.drilledLevelCaption != string.Empty && this.isDrilled && cell.CellData.Rows.Contains(this.drilledLevelCaption))
                                        || (this.isDrilled && doDrillup) || !this.isDrilled)
                                    {
                                        double cellDouble = 0f;
                                        double.TryParse(Convert.ToString((cell.Tag as Cell).Value), out cellDouble);
                                        chartPoints.Add(Convert.ToDouble(cellDouble, CultureInfo.CurrentCulture));
                                        var splitLabel = cell.CellData.Rows.Where(m => m != null).ToList();
                                        string name = string.Join("~", splitLabel.ToArray());
                                        var exsistLabels = labels.Where(m => m.Contains(name)).ToList();
                                        if (exsistLabels.Count == 0 && splitLabel.Count > 0)
                                            labels.Add(string.Join("~", splitLabel.ToArray()));
                                    }
                                }
                                else
                                {
                                    if (cell.CellValue.Length != 0 && cell.CellType != PivotCellDescriptorType.SummaryColumn && cell.CellType != PivotCellDescriptorType.SummaryRow)
                                    {
                                        if (isFirstCell)
                                        {
                                            legendText = cell.CellValue;
                                            if (cell.ExpandableState == ExpandableState.Collapsed || cell.ExpandableState == ExpandableState.Expanded)
                                            {
                                                Member member = cell.Tag as Member;
                                                seriesTags.Add(string.Format(CultureInfo.CurrentCulture, "{0}::{1}::{2}::{3}::{4}::{5}",
                                                        member.UniqueName, member.LevelUniqueName, member.Caption, member.ParentUniqueName, member.ParentCaption, ((int)cell.ExpandableState).ToString()));
                                            }
                                            isFirstCell = false;
                                        }
                                        else
                                            legendText += "~" + cell.CellValue;
                                    }
                                }
                            }
                            else
                            {
                                if (cell.CellType == PivotCellDescriptorType.ColumnHeader)
                                {
                                    legendText += "~" + cell.CellValue;
                                }
                            }

                        }

                        seriesName.Add(legendText);
                        chartSeries.Add(chartPoints);
                    }
                }

                if (chartSeries.Count > 0)
                {
                    foreach (double point in chartSeries[0])
                    {
                        chartSeriesPoints.Add(new List<double>());
                    }

                    foreach (List<double> list in chartSeries)
                    {
                        for (int j = 0; j < list.Count; j++)
                        {
                            chartSeriesPoints[j].Add(list[j]);
                        }
                    }
                }
                if(labels.Count == 0 && this.measureAxis == "Series"){
                    foreach (string measure in measureUniqueValues.Split('~'))
                        labels.Add(measure);
                }
                chartData.Add("chartLables", labels);
                chartData.Add("points_Y", chartSeriesPoints);
                chartData.Add("seriesNames", seriesName);
                chartData.Add("measureNames", measureUniqueValues);
                chartData.Add("seriesTags", seriesTags);
                chartData.Add("labelTags", this.DisplayOLAPLabels(pivotEngine));
                chartData.Add("addInfo", new { namedsetEnabled = this.namedsetEnabled });
                return new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(chartData);
            }
            return null;
        }

        private string FindMeasureUniqueName(OlapReport olapReport)
        {
            string measureNames = string.Empty;
            for (int i = 0; i < 3; i++)
            {
                Items elements = new Items();
                if (i == 0)
                    elements = olapReport.CategoricalElements; 
                else if (i == 1)
                    elements = olapReport.SeriesElements;
                else
                    elements = olapReport.SlicerElements;
                foreach (Item element in elements)
                {
                    if (element.ElementValue is MeasureElements)
                    {
                        this.measureAxis = element.Axis.ToString();
                        foreach (MeasureElement measure in (element.ElementValue as MeasureElements).Elements)
                        {
                            if (measureNames == string.Empty)
                                measureNames = measure.UniqueName;
                            else
                                measureNames += "~" + measure.UniqueName;
                        }
                    }
                    else if (element.ElementValue is NamedSetElement)
                        namedsetEnabled = true; namedSet = element;
                }
            }
            return measureNames;
        }

        private List<string> DisplayOLAPLabels(PivotEngine pivotEngine)
        {
            List<string> labelTags = new List<string>();
            if (pivotEngine != null)
            {
                int left = pivotEngine.RowHeaderSection.Left,
                  right = pivotEngine.RowHeaderSection.Right,
                  top = pivotEngine.HeaderSection.Height,
                  bottom = pivotEngine.RowsCount;
                if (pivotEngine.TableColumns.Count > 0)
                {
                    for (int i = left; i <= right; i++)
                    {
                        Dictionary<string, string> labelInfo = new Dictionary<string, string>();
                        PivotColumnDescriptor columnDescriptor = pivotEngine.TableColumns[i];
                        for (int j = top; j < bottom; j++)
                        {
                            PivotCellDescriptor cellDescriptior = columnDescriptor.Cells[j];
                            if (cellDescriptior.SpanCell == null)
                            {
                                Member member = cellDescriptior.Tag as Member;
                                var ab = labelTags.Where(m => m.Split(':')[0] == member.UniqueName).ToList();
                                if (ab.Count == 0)
                                {
                                    ab.Clear();
                                    var lTag = "";
                                    if (this.dataManager.CurrentReport.DrillType == DrillType.DrillPosition)
                                        lTag = Utils.GetDrillData(cellDescriptior, this.dataManager.GetPositionsInfo(cellDescriptior));
                                    else
                                        lTag = Utils.GetDrillData(cellDescriptior, null);
                                    var arr = lTag.Split(new string[] {"::"}, StringSplitOptions.None);
                                    arr[arr.Length-1] = ((int)cellDescriptior.ExpandableState).ToString();
                                    labelTags.Add(string.Join("::", arr));
                                }
                            }
                            else if (cellDescriptior.CellValue.Length == 0)
                            {
                                double startValue = 0.5 + j - top;
                                double endValue = 0;
                                if (this.dataManager.ItemSource == null)
                                {
                                    endValue = j + cellDescriptior.Range.Height + 0.5 - top;
                                }
                                else
                                {
                                    endValue = j + 1 + 0.5 - top;
                                }
                            }
                        }
                    }
                }
            }
            return labelTags;
        }
    }
}
