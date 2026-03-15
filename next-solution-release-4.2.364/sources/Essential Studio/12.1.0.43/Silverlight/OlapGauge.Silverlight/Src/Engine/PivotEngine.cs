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

namespace Syncfusion.OlapSilverlight.Base.Engine
{
    public class PivotEngine
    {
        public PivotEngine()
        {
            TableColumns = new List<PivotColumnDescriptor>();
            this.HeaderSection = new GridRangeInfo();
            this.RowHeaderSection = new GridRangeInfo();
        }
        public int RowsCount { get; set; }
        public GridRangeInfo HeaderSection { get; set; }
        public GridRangeInfo RowHeaderSection { get; set; }
        public SummaryLayout SummaryPosition { get; set; }
        public List<PivotColumnDescriptor> TableColumns { get; set; }
        public static PivotEngine DefaultTable
        {
            get
            {
                PivotEngine table = new PivotEngine();
                table.RowsCount = 4;
                for (int col = 0; col < 3; col++)
                {
                    PivotColumnDescriptor colDesc = new PivotColumnDescriptor();

                    for (int row = 0; row < 4; row++)
                    {
                        PivotCellDescriptor cellDesc = new PivotCellDescriptor();

                        if (row == 0)
                        {
                            cellDesc.CellValue = col == 2 ? "Measures" : "Level" + ((int)(col + 1)).ToString();
                            cellDesc.CellType = PivotCellDescriptorType.ColumnHeader;
                        }
                        else if (col == 0)
                        {
                            cellDesc.CellValue = "Level1 Member1";
                            cellDesc.CellType = PivotCellDescriptorType.RowHeader;
                        }
                        else if (col == 1)
                        {
                            if (row != 3)
                            {
                                cellDesc.CellValue = "Level2 Member" + row.ToString();
                                cellDesc.CellType = PivotCellDescriptorType.RowHeader;
                            }
                        }
                        else
                        {
                            if (row != 3)
                                cellDesc.CellValue = table.TableColumns[1].Cells[row].CellValue + " Measure";
                            else
                                cellDesc.CellValue = table.TableColumns[0].Cells[1].CellValue + " Measure";

                            cellDesc.CellType = PivotCellDescriptorType.Value;
                        }
                        colDesc.Cells.Add(cellDesc);
                    }

                    table.TableColumns.Add(colDesc);
                }

                return table;
            }
        }

        public KpiInfoCollection GetValidKpis()
        {
            KpiInfoCollection Kpis = new KpiInfoCollection();
            Kpis = this.GetKpis();
            int validKpiCount = Kpis.Count;
            for (int i = (validKpiCount - 1); i >= 0; i--)
            {
                KpiInfo kpiInfo = Kpis[i];
                if (!kpiInfo.IsValidKpi)
                {
                    Kpis.RemoveAt(i);
                }
            }
            return Kpis;
        }

        private KpiInfoCollection MergeKpiRowsWithColumns(KpiInfoCollection kpiInfoCollection)
        {
            KpiInfoCollection kpis = new KpiInfoCollection();
            KpiAxisType kpiAxis = KpiAxisType.None;
            int rowsCount = 0;
            int columnsCount = 0;
            bool breakloop = false;
            kpiAxis = this.GetKpiAxis();
            if (kpiAxis != KpiAxisType.None)
            {
                if (kpiAxis == KpiAxisType.Column)
                {
                    rowsCount = this.RowsCount;
                    columnsCount = this.TableColumns.Count;
                }
                else if (kpiAxis == KpiAxisType.Row)
                {
                    rowsCount = this.TableColumns.Count;
                    columnsCount = this.RowsCount;
                }
            }
            else
            {
                return kpis;
            }
            for (int i = 0; i < rowsCount; i++)
            {
                int j;
                KpiInfo tempKpiInfo = new KpiInfo();
                for (j = 0; j < columnsCount; j++)
                {
                    PivotCellDescriptor cellDescriptor;
                    if (kpiAxis == KpiAxisType.Column)
                    {
                        cellDescriptor = this.TableColumns[j].Cells[i];
                    }
                    else
                    {
                        cellDescriptor = this.TableColumns[i].Cells[j];
                    }

                    #region Traversing Rows
                    if (kpiAxis == KpiAxisType.Column)
                    {
                        if (cellDescriptor.CellType == PivotCellDescriptorType.RowHeader && cellDescriptor.Level > 0)
                        {
                            if (tempKpiInfo.MemberName == string.Empty || tempKpiInfo.MemberName == null)
                            {
                                tempKpiInfo.MemberName = this.TableColumns[j].Cells[i].CellCaption;
                            }
                            else
                            {
                                tempKpiInfo.MemberName = tempKpiInfo.MemberName + " - " + this.TableColumns[j].Cells[i].CellCaption;
                            }
                            tempKpiInfo.MemberRowIndex = i;
                            breakloop = false;
                        }
                        else if (cellDescriptor.CellType == PivotCellDescriptorType.SummaryColumn ||
                                cellDescriptor.CellType == PivotCellDescriptorType.SummaryRow ||
                                cellDescriptor.CellType == PivotCellDescriptorType.Any ||
                                cellDescriptor.CellType == PivotCellDescriptorType.ColumnHeader)
                        {
                            breakloop = true;
                            break;
                        }
                        else if (cellDescriptor.CellType == PivotCellDescriptorType.Value)
                        {
                            break;
                        }
                    }
                    #endregion

                    #region Traversing Rows
                    if (kpiAxis == KpiAxisType.Row)
                    {
                        if (cellDescriptor.CellType == PivotCellDescriptorType.ColumnHeader && cellDescriptor.Level > 0)
                        {
                            if (tempKpiInfo.MemberName == string.Empty || tempKpiInfo.MemberName == null)
                            {
                                tempKpiInfo.MemberName = this.TableColumns[i].Cells[j].CellCaption;
                            }
                            else
                            {
                                tempKpiInfo.MemberName = tempKpiInfo.MemberName + " - " + this.TableColumns[i].Cells[j].CellCaption;
                            }
                            tempKpiInfo.MemberRowIndex = i;
                            breakloop = false;
                        }
                        else if (cellDescriptor.CellType == PivotCellDescriptorType.SummaryColumn ||
                                cellDescriptor.CellType == PivotCellDescriptorType.SummaryRow ||
                                cellDescriptor.CellType == PivotCellDescriptorType.Any ||
                                cellDescriptor.CellType == PivotCellDescriptorType.RowHeader)
                        {
                            breakloop = true;
                            break;
                        }
                        else if (cellDescriptor.CellType == PivotCellDescriptorType.Value)
                        {
                            break;
                        }
                    }
                    #endregion

                }
                if (!breakloop)
                {
                    for (int k = 0; k < kpiInfoCollection.Count; k++)
                    {
                        if (kpiAxis == KpiAxisType.Column)
                        {
                            KpiInfo kpiInfo = new KpiInfo(kpiInfoCollection[k]);
                            kpiInfo.GoalValue = this.TableColumns[kpiInfo.GoalIndex].Cells[i].Value;
                            kpiInfo.ActualGoalValue = this.TableColumns[kpiInfo.GoalIndex].Cells[i].CellValue;
                            if (kpiInfo.MemberName == null || kpiInfo.MemberName == string.Empty)
                            {
                                kpiInfo.MemberName = tempKpiInfo.MemberName;
                            }
                            else
                            {
                                kpiInfo.MemberName = kpiInfo.MemberName + " - " + tempKpiInfo.MemberName;
                            }
                            kpiInfo.MeasureValue = this.TableColumns[kpiInfo.ValueIndex].Cells[i].Value;
                            kpiInfo.ActualMeasureValue = this.TableColumns[kpiInfo.ValueIndex].Cells[i].CellValue;
                            string s = kpiInfo.MeasureValue;
                            kpiInfo.StatusValue = int.Parse(this.TableColumns[kpiInfo.StatusIndex].Cells[i].CellValue);
                            kpiInfo.TrendValue = int.Parse(this.TableColumns[kpiInfo.TrendIndex].Cells[i].CellValue);
                            kpiInfo.MemberRowIndex = tempKpiInfo.MemberRowIndex;
                            if (!kpis.Contains(kpiInfo) && kpiInfo.Kpi_Name != string.Empty)
                            {
                                kpis.Add(kpiInfo);
                            }
                        }
                        else if (kpiAxis == KpiAxisType.Row)
                        {
                            KpiInfo kpiInfo = new KpiInfo(kpiInfoCollection[k]);
                            kpiInfo.GoalValue = this.TableColumns[i].Cells[kpiInfo.GoalIndex].Value;
                            kpiInfo.ActualGoalValue = this.TableColumns[i].Cells[kpiInfo.GoalIndex].CellValue;
                            if (kpiInfo.MemberName == null || kpiInfo.MemberName == string.Empty)
                            {
                                kpiInfo.MemberName = tempKpiInfo.MemberName;
                            }
                            else
                            {
                                kpiInfo.MemberName = kpiInfo.MemberName + " - " + tempKpiInfo.MemberName;
                            }
                            kpiInfo.MeasureValue = this.TableColumns[i].Cells[kpiInfo.ValueIndex].Value;
                            kpiInfo.ActualMeasureValue = this.TableColumns[i].Cells[kpiInfo.ValueIndex].CellValue;
                            kpiInfo.StatusValue = int.Parse(this.TableColumns[i].Cells[kpiInfo.StatusIndex].CellValue);
                            kpiInfo.TrendValue = int.Parse(this.TableColumns[i].Cells[kpiInfo.TrendIndex].CellValue);
                            kpiInfo.MemberRowIndex = tempKpiInfo.MemberRowIndex;
                            if (!kpis.Contains(kpiInfo) && kpiInfo.Kpi_Name != string.Empty)
                            {
                                kpis.Add(kpiInfo);
                            }
                        }
                    }
                }
                else
                {
                    breakloop = false;
                }
            }
            return kpis;
        }

        private KpiInfoCollection GetKpisAxisMembers()
        {
            KpiInfoCollection kpis = new KpiInfoCollection();
            KpiAxisType kpiAxis = KpiAxisType.None;
            int rowsCount = 0;
            int columnsCount = 0;
            bool breakloop = false;
            kpiAxis = this.GetKpiAxis();
            if (kpiAxis != KpiAxisType.None)
            {
                if (kpiAxis == KpiAxisType.Column)
                {
                    rowsCount = this.RowsCount;
                    columnsCount = this.TableColumns.Count;
                }
                else if (kpiAxis == KpiAxisType.Row)
                {
                    rowsCount = this.TableColumns.Count;
                    columnsCount = this.RowsCount;
                }
            }
            else
            {
                return kpis;
            }

            for (int i = 0; i < columnsCount; i++)
            {
                KpiInfo tempKpiInfo = new KpiInfo();
                for (int j = 0; j < rowsCount; j++)
                {
                    PivotCellDescriptor cellDescriptor;
                    if (kpiAxis == KpiAxisType.Column)
                    {
                        cellDescriptor = this.TableColumns[i].Cells[j];
                    }
                    else
                    {
                        cellDescriptor = this.TableColumns[j].Cells[i];
                    }

                    #region Traversing Columns
                    if (kpiAxis == KpiAxisType.Column)
                    {
                        if (cellDescriptor.CellType == PivotCellDescriptorType.ColumnHeader)
                        {
                            if (cellDescriptor.MemberObjInfo is Member)
                            {
                                Member member = (Member)cellDescriptor.MemberObjInfo;
                                if (member.KpiName != null)
                                {
                                    tempKpiInfo.Kpi_Name = member.KpiName;
                                }
                                else
                                {
                                    if (tempKpiInfo.MemberName == string.Empty || tempKpiInfo.MemberName == null)
                                    {
                                        tempKpiInfo.MemberName = member.Caption;
                                    }
                                    else
                                    {
                                        tempKpiInfo.MemberName = tempKpiInfo.MemberName + " - " + member.Caption;
                                    }
                                    tempKpiInfo.MemberColumnIndex = i;
                                }
                            }
                            switch (cellDescriptor.KpiType)
                            {
                                case KpiTypeEnum.Kpi_Goal:
                                    {
                                        tempKpiInfo.GoalCaption = cellDescriptor.CellCaption;
                                        tempKpiInfo.GoalIndex = i;
                                        break;
                                    }
                                case KpiTypeEnum.Kpi_Value:
                                    {
                                        tempKpiInfo.MeasureCaption = cellDescriptor.CellCaption;
                                        tempKpiInfo.ValueIndex = i;
                                        break;
                                    }
                                case KpiTypeEnum.Kpi_Status:
                                    {
                                        tempKpiInfo.StatusGraphic = cellDescriptor.KpiGraphicsStyle;
                                        tempKpiInfo.StatusIndex = i;
                                        break;
                                    }
                                case KpiTypeEnum.Kpi_Trend:
                                    {
                                        tempKpiInfo.TrendGraphic = cellDescriptor.KpiGraphicsStyle;
                                        tempKpiInfo.TrendIndex = i;
                                        break;
                                    }
                                case KpiTypeEnum.Kpi_None:
                                    {
                                        break;
                                    }
                            }
                            breakloop = false;
                        }
                        else if (cellDescriptor.CellType == PivotCellDescriptorType.SummaryColumn ||
                                cellDescriptor.CellType == PivotCellDescriptorType.SummaryRow)
                        {
                            breakloop = true;
                            break;
                        }
                        else if (cellDescriptor.CellType == PivotCellDescriptorType.Value ||
                                cellDescriptor.CellType == PivotCellDescriptorType.RowHeader ||
                                cellDescriptor.CellType == PivotCellDescriptorType.Any)
                        {
                            break;
                        }
                    }
                    #endregion

                    #region Traversing Rows
                    else if (kpiAxis == KpiAxisType.Row)
                    {
                        if (cellDescriptor.CellType == PivotCellDescriptorType.RowHeader)
                        {
                            if (cellDescriptor.MemberObjInfo is Member)
                            {
                                Member member = (Member)cellDescriptor.MemberObjInfo;
                                if (member.KpiName != null)
                                {
                                    tempKpiInfo.Kpi_Name = member.KpiName;
                                }
                                else
                                {
                                    if (tempKpiInfo.MemberName == string.Empty || tempKpiInfo.MemberName == null)
                                    {
                                        tempKpiInfo.MemberName = member.Caption;
                                    }
                                    else
                                    {
                                        tempKpiInfo.MemberName = tempKpiInfo.MemberName + " - " + member.Caption;
                                    }
                                    tempKpiInfo.MemberColumnIndex = i;
                                }
                            }
                            switch (cellDescriptor.KpiType)
                            {
                                case KpiTypeEnum.Kpi_Goal:
                                    {
                                        tempKpiInfo.GoalCaption = cellDescriptor.CellCaption;
                                        tempKpiInfo.GoalIndex = i;
                                        break;
                                    }
                                case KpiTypeEnum.Kpi_Value:
                                    {
                                        tempKpiInfo.MeasureCaption = cellDescriptor.CellCaption;
                                        tempKpiInfo.ValueIndex = i;
                                        break;
                                    }
                                case KpiTypeEnum.Kpi_Status:
                                    {
                                        tempKpiInfo.StatusGraphic = cellDescriptor.KpiGraphicsStyle;
                                        tempKpiInfo.StatusIndex = i;
                                        break;
                                    }
                                case KpiTypeEnum.Kpi_Trend:
                                    {
                                        tempKpiInfo.TrendGraphic = cellDescriptor.KpiGraphicsStyle;
                                        tempKpiInfo.TrendIndex = i;
                                        break;
                                    }
                                case KpiTypeEnum.Kpi_None:
                                    {
                                        break;
                                    }
                            }
                            breakloop = false;
                        }
                        else if (cellDescriptor.CellType == PivotCellDescriptorType.SummaryColumn ||
                                cellDescriptor.CellType == PivotCellDescriptorType.SummaryRow)
                        {
                            breakloop = true;
                            break;
                        }
                        else if (cellDescriptor.CellType == PivotCellDescriptorType.Value ||
                                cellDescriptor.CellType == PivotCellDescriptorType.ColumnHeader ||
                                cellDescriptor.CellType == PivotCellDescriptorType.Any)
                        {
                            break;
                        }
                    }
                    #endregion

                }
                if (!kpis.Contains(tempKpiInfo) && tempKpiInfo.Kpi_Name != string.Empty && !breakloop)
                {
                    kpis.Add(tempKpiInfo);
                }
                else
                {
                    int k = kpis.FindMemberIndexByName(tempKpiInfo.Kpi_Name, tempKpiInfo.MemberName);
                    if (k >= 0)
                    {
                        kpis[k] = kpis[k].Copy(tempKpiInfo, kpis[k]);
                    }
                    breakloop = false;
                }
            }
            return kpis;
        }

        public KpiInfoCollection GetKpis()
        {
            //this.RemoveTotalsElements();
            //this.ClearLevelHeadersArea();
            KpiInfoCollection kpiColumnCollection = new KpiInfoCollection();
            kpiColumnCollection = GetKpisAxisMembers();
            kpiColumnCollection.RemoveMeasures();
            KpiInfoCollection Kpis = new KpiInfoCollection();
            Kpis = MergeKpiRowsWithColumns(kpiColumnCollection);
            return (Kpis);
        }

        private KpiAxisType GetKpiAxis()
        {
            for (int i = 0; i < this.RowsCount; i++)
            {
                for (int j = 0; j < this.TableColumns.Count; j++)
                {
                    PivotCellDescriptor cellDescriptor = this.TableColumns[j].Cells[i];
                    if (cellDescriptor.CellType == PivotCellDescriptorType.ColumnHeader)
                    {
                        if (cellDescriptor.KpiType != KpiTypeEnum.Kpi_None)
                        {
                            return KpiAxisType.Column;
                        }
                    }
                    if (cellDescriptor.CellType == PivotCellDescriptorType.RowHeader)
                    {
                        if (cellDescriptor.KpiType != KpiTypeEnum.Kpi_None)
                        {
                            return KpiAxisType.Row;
                        }
                    }
                }
            }
            return KpiAxisType.None;
        }

    }
}
