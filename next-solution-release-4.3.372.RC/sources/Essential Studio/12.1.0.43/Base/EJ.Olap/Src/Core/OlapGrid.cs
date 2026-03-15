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
using Syncfusion.Olap.Data;
using Syncfusion.Olap.Engine;
using Syncfusion.Olap.Reports;
using System.Globalization;
using Syncfusion.JavaScript.Shared;
using System.Web.UI;
using System.Web;
using Syncfusion.XlsIO;
using System.Drawing;
using System.Text.RegularExpressions;
using Syncfusion.JavaScript.Olap.Models;
using Syncfusion.Olap.Common;

namespace Syncfusion.JavaScript.Olap
{
    public class OlapGrid : Syncfusion.JavaScript.Control
    {
        #region wrapperclass
        public OlapGridProperties OlapGridModel
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
            get { return "ejOlapGrid"; }
        }
        protected override object Model
        {
            get { return this.OlapGridModel; }
        }

        public OlapGrid() { }
        public OlapGrid(String id, OlapGridProperties propModel)
        {
            this.ID = id;
            this.OlapGridModel = propModel;
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

        private Dictionary<string, object> GetJsonData(string action, OlapDataManager dataManager, string cellPosition, string headerInfo, string gridLayout)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            if (action == "initializeGrid")
            {
                if (gridLayout != null)
                    dataManager.ExecuteOlapTable(GetLayoutProperties(gridLayout));
                else
                    dataManager.ExecuteOlapTable();
                if (dataManager.PivotEngine != null)
                {
                    dict.Add("PivotRecords", GenerateJSONData(dataManager));
                    dict.Add("OlapReport", Utils.SerializeOlapReport(dataManager.CurrentReport));
                    if (dataManager.CurrentReport.EnablePaging)
                    {
                        dict.Add("PageSettings", new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(dataManager.CurrentReport.PagerOptions));
                        dict.Add("HeaderCounts", new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(dataManager.ExecuteCount()));
                    }
                }
            }
            else if (action == "drillDownGrid")
            {
                UpdateDrilledReport(dataManager, headerInfo);
                if (gridLayout != null)
                    dataManager.ExecuteOlapTable(GetLayoutProperties(gridLayout));
                else
                    dataManager.ExecuteOlapTable();
                if (dataManager.PivotEngine != null)
                {
                    UpdateReportCollection(dataManager);
                    dict.Add("PivotRecords", GenerateJSONData(dataManager));
                    dict.Add("OlapReport", Utils.SerializeOlapReport(dataManager.CurrentReport));
                    dict.Add("ClientReports", Common.SerializeObject<OlapReportCollection>(dataManager.Reports).Compress());
                    if (dataManager.CurrentReport.EnablePaging)
                    {
                        dict.Add("PageSettings", new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(dataManager.CurrentReport.PagerOptions));
                        dict.Add("HeaderCounts", new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(dataManager.ExecuteCount()));
                    }
                }
            }
            else if (action == "Paging")
            {
                if (gridLayout != null)
                    dataManager.ExecuteOlapTable(GetLayoutProperties(gridLayout));
                else
                    dataManager.ExecuteOlapTable();
                if (dataManager.PivotEngine != null)
                {
                    dict.Add("PivotRecords", GenerateJSONData(dataManager));
                    dict.Add("OlapReport", Utils.SerializeOlapReport(dataManager.CurrentReport));
                }
            }
            return dict;
        }

        private GridLayout GetLayoutProperties(string gridLayout)
        {
            if (gridLayout == "ExcelLikeLayout")
                return GridLayout.ExcelLikeLayout;
            else if (gridLayout == "NoSummaries")
                return GridLayout.NoSummaries;
            else if (gridLayout == "NormalTopSummary")
                return GridLayout.NormalTopSummary;
            else if (gridLayout == "ExcelLikeLayoutWithMemberProperties")
                return GridLayout.ExcelLikeLayoutWithMemberProperties;
            else
                return GridLayout.Normal;
        }

        public Dictionary<string, object> GetJsonData(string action, OlapDataManager datamanager)
        {
            return GetJsonData(action, datamanager, null, null, null);
        }

        public Dictionary<string, object> GetJsonData(string action, OlapDataManager datamanager, string gridLayout)
        {
            return GetJsonData(action, datamanager, null, null, gridLayout);
        }

        public Dictionary<string, object> GetJsonData(string action, OlapDataManager datamanager, string cellPostion, string headerInfo)
        {
            return GetJsonData(action, datamanager, cellPostion, headerInfo, null);
        }

        public Dictionary<string, object> GetJsonData(string action, string connectionString, OlapDataManager datamanager, string cellPostion, string headerInfo, string gridLayout)
        {
            return GetJsonData(action, datamanager, cellPostion, headerInfo, gridLayout);
        }

        public void UpdateDrilledReport(OlapDataManager DataManager, string HeaderInfo)
        {
            var cellInfo = HeaderInfo.Split(new string[] { "::"}, StringSplitOptions.RemoveEmptyEntries);
            if (cellInfo.Length > 1)
            {
                Member memberObj = new Member();
                memberObj.UniqueName = cellInfo[0];
                memberObj.LevelUniqueName = cellInfo[1];
                memberObj.Caption = cellInfo[2];
                memberObj.ParentUniqueName = cellInfo[3];
                memberObj.ParentCaption = cellInfo[4];
                var cellType = (PivotCellDescriptorType)int.Parse(cellInfo[5]);

                if (DataManager.CurrentReport.DrillType == Syncfusion.Olap.Reports.DrillType.DrillPosition)
                {
                    var positionInfo = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<List<PositionInfo>>(cellInfo[6]);
                    memberObj.ParentHierarchy = cellInfo[7];
                    var expandState = (ExpandableState)int.Parse(cellInfo[8]);
                    DataManager.ToggleExpandableStateOnDrillPosition(memberObj, cellType, positionInfo, expandState);
                }
                else
                    DataManager.ToggleExpandableState(cellType, memberObj, GridLayout.Normal, true);
            }
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
        public string GenerateJSONData(OlapDataManager DataManager)
        {
            if (DataManager.PivotEngine != null)
            {
                var pivotEngineRecords = DataManager.PivotEngine.TableColumns.SelectMany(m => m.Cells.Select(n => new
                {
                    Index = n.CellIndex + "," + n.PivotRowIndex,
                    CSS = this.SetKPI(n),
                    Value = this.SetKPI(n).Contains(OlapGridKPIIcons.KpiIconValueCell) ? string.Empty : n.CellValue,
                    Info = DataManager.CurrentReport.DrillType == DrillType.DrillPosition ? Utils.GetDrillData(n, DataManager.GetPositionsInfo(n)) : Utils.GetDrillData(n, null),
                    State = n.ExpandableState,
                    RowSpan = n.Range.Height,
                    ColSpan = n.Range.Width,
                    Span = "None"
                })).ToList();

                var tempTags = DataManager.PivotEngine.TableColumns.SelectMany(m => m.Cells.Where(ab => ab.CellType == PivotCellDescriptorType.RowHeader && ab.CellValue != "MeasuresLevel").Select(n => new
                {
                    Members = n.Tag,
                    State = n.ExpandableState
                })).ToList();

                var distTags = tempTags.Distinct().ToList();
                distTags.RemoveAll(m => m.Members == null);
                var labelTags = distTags.Select(m => new
                {
                    Value = (m.Members as Member).UniqueName + "::" + (m.Members as Member).LevelUniqueName + 
                    "::" + (m.Members as Member).Caption + "::" + (m.Members as Member).ParentUniqueName + 
                    "::" + (m.Members as Member).ParentCaption + "::" + ((int)m.State).ToString()
                }).ToList();
                return new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(pivotEngineRecords);
            }
            return null;
        }

        public OlapReport SetPaging(string currentReport, string pagingInfo)
        {
            OlapReport olapReport = Utils.DeserializeOlapReport(currentReport);
            if (pagingInfo.Split(':')[0] == "series")
                olapReport.PagerOptions.SeriesCurrentPage = Int32.Parse(pagingInfo.Split(':')[1]);
            else if (pagingInfo.Split(':')[0] == "categorical")
                olapReport.PagerOptions.CategorialCurrentPage = Int32.Parse(pagingInfo.Split(':')[1]);
            return olapReport;
        }

        public void ExportToExcel(OlapDataManager DataManager, string args, string fileName, System.Web.HttpResponse response)
        {
            string olapReport = args.Split(new string[] { "&layout=" }, StringSplitOptions.None)[0].Remove(0, 14).Replace("%2F", "/").Replace("%2B", "+").Replace("%3D", "=");
            string layout = args.Split(new string[] { "&layout=" }, StringSplitOptions.None)[1].Split(new string[] { "&summaryCellColor=" }, StringSplitOptions.None)[0];
            var summaryCellColor = args.Split(new string[] { "&layout=" }, StringSplitOptions.None)[1].Split(new string[] { "&summaryCellColor=" }, StringSplitOptions.None)[1].Replace("%2C+", ",").Replace("rgb%28", "").Replace("%29", "").Split(',');
            System.Data.DataTable dTable = new System.Data.DataTable();
            GridLayout gridLayout = (GridLayout)Enum.Parse(typeof(GridLayout), layout);
            if (!string.IsNullOrEmpty(olapReport))
                DataManager.SetCurrentReport(Utils.DeserializeOlapReport(olapReport));
            PivotEngine pivotData = DataManager.PivotEngine;
            pivotData = DataManager.ExecuteOlapTable(gridLayout);
            if (pivotData != null)
            {
                for (int i = 0; i < pivotData.TableColumns.Count; i++)
                    dTable.Columns.Add(new System.Data.DataColumn(i.ToString()));
                for (int i = 0; i < pivotData.RowsCount; i++)
                {
                    System.Data.DataRow dr = dTable.NewRow();
                    for (int j = 0; j < pivotData.TableColumns.Count; j++)
                        dr[j] = (pivotData.TableColumns[j].Cells[i].CellType != PivotCellDescriptorType.Value && pivotData.TableColumns[j].Cells[i].CellCaption == null) ? "" : pivotData.TableColumns[j].Cells[i].CellValue;
                    dTable.Rows.Add(dr);
                }
            }
            IWorkbook workbook = new ExcelEngine().Excel.Workbooks.Create(2);
            if (dTable != null && dTable.Rows.Count != 0 && dTable.Columns.Count != 0)
                workbook.Worksheets[0].ImportDataTable(dTable, false, 1, 1, -1, -1, true);
            Color color = summaryCellColor.Length == 3 ? Color.FromArgb(Int32.Parse(summaryCellColor[0]), Int32.Parse(summaryCellColor[1]), Int32.Parse(summaryCellColor[2])) : Color.Transparent;
            if (pivotData != null) FormatExcel(workbook, pivotData, color);
            workbook.SaveAs(fileName, ExcelSaveType.SaveAsXLS, response, ExcelDownloadType.PromptDialog);
            response.Close();
        }
        private void FormatExcel(IWorkbook workbook, PivotEngine PivotData, Color color)
        {
            ////Body Style 
            IWorksheet sheet = workbook.Worksheets[0];
            #region Adding Styles to Excel Rows and Columns
            List<IStyle> CurrentStyle = new List<IStyle>();
            int styleindex = 0;
            foreach (PivotCellDescriptorType style in Enum.GetValues(typeof(PivotCellDescriptorType)))
            {
                CurrentStyle.Add(workbook.Styles.Add(style.ToString()));
                this.UpdateStyle(workbook, CurrentStyle, style, styleindex, color);
                styleindex++;
            }
            sheet.IsGridLinesVisible = true;
            ////To merge the Cells 
            for (int i = 1; i <= PivotData.HeaderSection.Height; i++)
            {
                for (int j = 1; j <= PivotData.TableColumns.Count; )
                {
                    PivotCellDescriptor cellDescriptor = PivotData.TableColumns[j - 1].Cells[i - 1];
                    ////Adding styles to the Column Header
                    if (cellDescriptor.Range != null)
                    {
                        StringBuilder range1 = new StringBuilder();
                        StringBuilder st = new StringBuilder();
                        st.Append(GetExcelRange((j - 1), i, ((j + cellDescriptor.Range.Right) - 1), (i + cellDescriptor.Range.Bottom)));
                        range1.Append(GetColumnRange((j - 1), i));
                        if (!sheet.Range[st.ToString()].IsMerged)
                        {
                            sheet.Range[st.ToString()].Merge();
                            switch (cellDescriptor.CellType)
                            {
                                case PivotCellDescriptorType.ColumnHeader:
                                    sheet.Range[range1.ToString()].CellStyleName = PivotCellDescriptorType.ColumnHeader.ToString();
                                    sheet.Range[range1.ToString()].HorizontalAlignment = ExcelHAlign.HAlignLeft;
                                    j += cellDescriptor.Range.Width;
                                    break;
                                case PivotCellDescriptorType.SummaryColumn:
                                    sheet.Range[range1.ToString()].CellStyleName = PivotCellDescriptorType.SummaryColumn.ToString();
                                    sheet.Range[range1.ToString()].HorizontalAlignment = ExcelHAlign.HAlignLeft;
                                    j += cellDescriptor.Range.Width;
                                    break;
                                case PivotCellDescriptorType.Any:
                                    sheet.Range[st.ToString()].Clear();
                                    sheet.Range[st.ToString()].Merge();
                                    j += cellDescriptor.Range.Width;
                                    break;
                                default:
                                    j++;
                                    break;
                            }
                        }
                        else
                        {
                            j += cellDescriptor.Range.Width;
                        }
                    }
                    else
                    {
                        j++;
                    }
                }
            }
            //Formats the Row Header
            for (int i = PivotData.RowHeaderSection.Top; i > 0 && i <= PivotData.RowsCount; i++)
            {
                for (int j = 1; j <= PivotData.RowHeaderSection.Width; )
                {
                    PivotCellDescriptor cellDescriptor = PivotData.TableColumns[j - 1].Cells[i - 1];
                    cellDescriptor.CellValue = PivotData.TableColumns[j - 1].Cells[i - 1].CellValue;
                    if (cellDescriptor.Range != null)
                    {
                        StringBuilder range1 = new StringBuilder();
                        StringBuilder st = new StringBuilder();
                        st.Append(GetExcelRange((j - 1), i, ((j + cellDescriptor.Range.Right) - 1), (i + cellDescriptor.Range.Bottom)));
                        range1.Append(GetColumnRange((j - 1), i));

                        if (!sheet.Range[st.ToString()].IsMerged)
                        {
                            sheet.Range[st.ToString()].Merge();
                            switch (cellDescriptor.CellType)
                            {
                                case PivotCellDescriptorType.RowHeader:
                                    sheet.Range[range1.ToString()].CellStyleName = PivotCellDescriptorType.RowHeader.ToString();
                                    sheet.Range[range1.ToString()].HorizontalAlignment = ExcelHAlign.HAlignLeft;
                                    j += cellDescriptor.Range.Width;
                                    break;
                                case PivotCellDescriptorType.SummaryColumn:
                                case PivotCellDescriptorType.SummaryRow:
                                    sheet.Range[range1.ToString()].CellStyleName = PivotCellDescriptorType.SummaryColumn.ToString();
                                    sheet.Range[range1.ToString()].HorizontalAlignment = ExcelHAlign.HAlignLeft;
                                    j += cellDescriptor.Range.Width;
                                    break;
                                case PivotCellDescriptorType.Value:
                                    sheet.Range[range1.ToString()].CellStyleName = PivotCellDescriptorType.Value.ToString();
                                    sheet.Range[range1.ToString()].HorizontalAlignment = ExcelHAlign.HAlignRight;
                                    j += cellDescriptor.Range.Width;
                                    break;
                                case PivotCellDescriptorType.Any:
                                    sheet.Range[range1.ToString()].CellStyleName = PivotCellDescriptorType.Any.ToString();
                                    sheet.Range[st.ToString()].Clear();
                                    j += cellDescriptor.Range.Width;
                                    break;
                                default:
                                    j++;
                                    break;
                            }
                        }
                        else
                        {
                            j++;
                        }
                    }
                    else
                    {
                        j++;
                    }
                }
            }
            for (int i = 1; i <= PivotData.RowsCount; i++)
            {
                for (int j = 1; j <= PivotData.TableColumns.Count; j++)
                {
                    PivotCellDescriptor cellDescriptor = PivotData.TableColumns[j - 1].Cells[i - 1];
                    switch (cellDescriptor.CellType)
                    {
                        case PivotCellDescriptorType.Value:
                            if ((cellDescriptor.CellExTypes.Contains(PivotCellDescriptorType.SummaryRow.ToString())) || (cellDescriptor.CellExTypes.Contains(PivotCellDescriptorType.SummaryColumn.ToString())))
                            {
                                sheet.Range[i, j].CellStyleName = PivotCellDescriptorType.SummaryRow.ToString();
                            }
                            else
                            {
                                sheet.Range[i, j].CellStyleName = PivotCellDescriptorType.Value.ToString();
                            }
                            sheet.Range[i, j].IgnoreErrorOptions = ExcelIgnoreError.All;
                            if (cellDescriptor.DoubleValue > 0)
                            {
                                sheet.Range[i, j].Text = cellDescriptor.CellValue;
                                sheet.Range[i, j].Value2 = cellDescriptor.DoubleValue;

                                var format = Regex.Replace(cellDescriptor.CellValue, "[0-9]", "#");

                                var cell = cellDescriptor.Tag as Syncfusion.Olap.Data.Cell;
                                if (cell != null && cell.FormatString == "Currency")
                                {
                                    var symbol = Regex.Match(cellDescriptor.CellValue, "(^[^0-9#]+)|([^0-9#]+$)");
                                    format = symbol.Value + (cellDescriptor.DoubleValue.ToString().Contains(CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator) ? "#,##0.00" : "#,##0");
                                }
                                else if (cell != null && cell.FormatString == "Percent")
                                {
                                    format = cellDescriptor.DoubleValue.ToString().Contains(CultureInfo.CurrentCulture.NumberFormat.PercentDecimalSeparator) ? "0.00%" : "0%";
                                }
                                else
                                    format = cellDescriptor.DoubleValue.ToString().Contains(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator) ? "#,##0.00" : "#,##0";

                                sheet.Range[i, j].NumberFormat = format;
                            }
                            else
                            {
                                var cell = cellDescriptor.Tag as Syncfusion.Olap.Data.Cell;
                                if (cell != null && !string.IsNullOrEmpty(cellDescriptor.CellValue) && cell.FormatString == "Percent")
                                {
                                    sheet.Range[i, j].Text = cellDescriptor.CellValue;
                                    sheet.Range[i, j].Value2 = cellDescriptor.DoubleValue;
                                    sheet.Range[i, j].NumberFormat = cellDescriptor.DoubleValue.ToString().Contains(CultureInfo.CurrentCulture.NumberFormat.PercentDecimalSeparator) ? "0.00%" : "0%";
                                }
                            }
                            sheet.Range[i, j].HorizontalAlignment = ExcelHAlign.HAlignRight;
                            break;
                    }
                }
            }
            ////Autofit Rows and Columns
            sheet.UsedRange.AutofitRows();
            sheet.UsedRange.AutofitColumns();
            #endregion
        }
        enum Alphabets
        {
            /// <summary>
            /// Denotes alphabet A
            /// </summary>
            A,

            /// <summary>
            /// Denotes alphabet B
            /// </summary>
            B,

            /// <summary>
            /// Denotes alphabet C
            /// </summary>
            C,

            /// <summary>
            /// Denotes alphabet D
            /// </summary>
            D,

            /// <summary>
            /// Denotes alphabet E
            /// </summary>
            E,

            /// <summary>
            /// Denotes alphabet F
            /// </summary>
            F,

            /// <summary>
            /// Denotes alphabet G
            /// </summary>
            G,

            /// <summary>
            /// Denotes alphabet H
            /// </summary>
            H,

            /// <summary>
            /// Denotes alphabet I
            /// </summary>
            I,

            /// <summary>
            /// Denotes alphabet J
            /// </summary>
            J,

            /// <summary>
            /// Denotes alphabet K
            /// </summary>
            K,

            /// <summary>
            /// Denotes alphabet L
            /// </summary>
            L,

            /// <summary>
            /// Denotes alphabet M
            /// </summary>
            M,

            /// <summary>
            /// Denotes alphabet N
            /// </summary>
            N,

            /// <summary>
            /// Denotes alphabet O
            /// </summary>
            O,

            /// <summary>
            /// Denotes alphabet P
            /// </summary>
            P,

            /// <summary>
            /// Denotes alphabet Q
            /// </summary>
            Q,

            /// <summary>
            /// Denotes alphabet R
            /// </summary>
            R,

            /// <summary>
            /// Denotes alphabet S
            /// </summary>
            S,

            /// <summary>
            /// Denotes alphabet T
            /// </summary>
            T,

            /// <summary>
            /// Denotes alphabet U
            /// </summary>
            U,

            /// <summary>
            /// Denotes alphabet V
            /// </summary>
            V,

            /// <summary>
            /// Denotes alphabet W
            /// </summary>
            W,

            /// <summary>
            /// Denotes alphabet X
            /// </summary>
            X,

            /// <summary>
            /// Denotes alphabet Y
            /// </summary>
            Y,

            /// <summary>
            /// Denotes alphabet Z
            /// </summary>
            Z
        }
        private void UpdateStyle(IWorkbook workbook, List<IStyle> CurrentStyle, PivotCellDescriptorType StyleType, int index, Color color1)
        {
            CurrentStyle[index].BeginUpdate();
            CurrentStyle[index].Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
            CurrentStyle[index].Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
            CurrentStyle[index].Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
            CurrentStyle[index].HorizontalAlignment = ExcelHAlign.HAlignCenter;
            CurrentStyle[index].VerticalAlignment = ExcelVAlign.VAlignTop;
            CurrentStyle[index].Font.FontName = "Segoe UI";
            Color color = Color.Gray;
            switch (StyleType)
            {
                case PivotCellDescriptorType.RowHeader:
                case PivotCellDescriptorType.ColumnHeader:
                    CurrentStyle[index].Font.Bold = true;
                    break;
                case PivotCellDescriptorType.SummaryColumn:
                case PivotCellDescriptorType.SummaryRow:
                    CurrentStyle[index].Font.Bold = true;
                    CurrentStyle[index].Color = color1;
                    if (color1.Name.ToLower() == "ff1f1f1f") CurrentStyle[index].Font.Color = ExcelKnownColors.Grey_25_percent;
                    break;
            }
            if (workbook.Worksheets[0].UsedRange.Cells.Length > 0)
                workbook.Worksheets[0].UsedRange.CellStyleName = StyleType.ToString();
            CurrentStyle[index].EndUpdate();
        }

        private StringBuilder GetColumnRange(int col, int row)
        {
            Type alpha = typeof(Alphabets);
            string[] alphabets = Enum.GetNames(alpha);
            StringBuilder range = new StringBuilder();
            StringBuilder st = new StringBuilder();
            if (col <= (alphabets.Length - 1))
            {
                range.Append(alphabets[col].ToString());
            }
            else
            {
                int m = col / alphabets.Length;
                int l = col % alphabets.Length;
                range.Append(alphabets[m - 1].ToString());
                range.Append(alphabets[l].ToString());
            }
            range.Append(row.ToString());
            return range;
        }

        /// <summary>
        /// Gets the Range value of the Excel Column based on the Columns and
        /// Row values specified.
        /// </summary>
        /// <param name="col1">The col1 range</param>
        /// <param name="row1">The row1 range</param>
        /// <param name="col2">The col2 range</param>
        /// <param name="row2">The row2 range</param>
        /// <returns>a StringBuilder object</returns>
        private StringBuilder GetExcelRange(int col1, int row1, int col2, int row2)
        {
            StringBuilder st = new StringBuilder();
            st.Append(GetColumnRange(col1, row1));
            st.Append(":");
            st.Append(GetColumnRange(col2, row2));
            return st;
        }

        

        private string SetKPI(PivotCellDescriptor cellDescriptor)
        {
            var cssClass = cellDescriptor.CellType == PivotCellDescriptorType.Value
                               ? ((cellDescriptor.CellExTypes.Contains(PivotCellDescriptorType.SummaryColumn.ToString()) ||
                                   cellDescriptor.CellExTypes.Contains(PivotCellDescriptorType.SummaryRow.ToString()))
                                      ? "summary value"
                                      : "value")
                               : (cellDescriptor.CellType == PivotCellDescriptorType.ColumnHeader
                                      ? "colheader"
                                      : (cellDescriptor.CellType == PivotCellDescriptorType.RowHeader
                                             ? "rowheader"
                                             : ((cellDescriptor.CellType == PivotCellDescriptorType.SummaryColumn ||
                                                 cellDescriptor.CellType == PivotCellDescriptorType.SummaryRow)
                                                    ? "summary"
                                                    : "none")));

            if (cellDescriptor.CellType == PivotCellDescriptorType.Value
               && (cellDescriptor.KpiType == KpiTypeEnum.Kpi_Status || cellDescriptor.KpiType == KpiTypeEnum.Kpi_Trend))
            {
                cssClass += OlapGridKPIIcons.KpiIconValueCell;
                var res = (KpiCompareResult)Enum.Parse(typeof(KpiCompareResult), cellDescriptor.CellValue);

                cssClass += SetKPIClass(cellDescriptor, res);
            }
            return cssClass;
        }

        private string SetKPIClass(PivotCellDescriptor cellDescriptor, KpiCompareResult res)
        {
            var cssClass = string.Empty;
            switch (res)
            {
                case KpiCompareResult.less:
                    if (cellDescriptor.KpiType == KpiTypeEnum.Kpi_Status)
                    {
                        if (cellDescriptor.KpiGraphicsStyle == KPIGraphics.RoadSigns)
                        {
                            cssClass += OlapGridKPIIcons.RedRoadImage;
                        }
                        else
                        {
                            cssClass += OlapGridKPIIcons.DiamondImage;
                        }
                    }
                    else
                    {
                        cssClass += OlapGridKPIIcons.DownArrowImage;
                    }
                    break;

                case KpiCompareResult.equal:
                    if (cellDescriptor.KpiType == KpiTypeEnum.Kpi_Status)
                    {
                        if (cellDescriptor.KpiGraphicsStyle == KPIGraphics.RoadSigns)
                        {
                            cssClass += OlapGridKPIIcons.AllColorImage;
                        }
                        else
                        {
                            cssClass += OlapGridKPIIcons.TriangleImage;
                        }
                    }
                    else
                    {
                        cssClass += OlapGridKPIIcons.RightArrowImage;
                    }
                    break;

                case KpiCompareResult.greater:
                    if (cellDescriptor.KpiType == KpiTypeEnum.Kpi_Status)
                    {
                        if (cellDescriptor.KpiGraphicsStyle == KPIGraphics.RoadSigns)
                        {
                            cssClass += OlapGridKPIIcons.GreenRoadImage;
                        }
                        else
                        {
                            cssClass += OlapGridKPIIcons.CircleImage;
                        }
                    }
                    else
                    {
                        cssClass += OlapGridKPIIcons.UpArrowImage;
                    }
                    break;
            }
            return cssClass;
        }

        private enum KpiCompareResult
        {
            /// <summary>
            /// Indicates less value.
            /// </summary>
            less = -1,

            /// <summary>
            /// Indicates equal value.
            /// </summary>
            equal = 0,

            /// <summary>
            /// Indicates greater value.
            /// </summary>
            greater = 1
        }

        private class KPIGraphics
        {
            public const string Cylinder = "Cylinder";
            public const string RoadSigns = "RoadSigns";
            public const string StandardArrow = "Standard Arrow";
        }

        private class OlapGridKPIIcons
        {
            public const string AllColorImage = " kpiallcolor";
            public const string CircleImage = " kpicircle";
            public const string DiamondImage = " kpidiamond";
            public const string DownArrowImage = " kpidownarrow";
            public const string GreenRoadImage = " kpigreenroad";
            public const string KpiIconValueCell = " kpiiconvalue";
            public const string RedRoadImage = " kpiredroad";
            public const string RightArrowImage = " kpirightarrow";
            public const string TriangleImage = " kpitriangle";
            public const string UpArrowImage = " kpiuparrow";
        }
    }
}
