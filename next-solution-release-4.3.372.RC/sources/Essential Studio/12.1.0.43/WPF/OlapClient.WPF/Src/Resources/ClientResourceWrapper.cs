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
using System.Globalization;

namespace Syncfusion.Windows.Client.Olap.Resources
{
    internal sealed class ClientResourceWrapper
    {
        #region Constants

        const string CONNECTION_OPTION_TOOLTIP = "OlapClient_Toolbar_ConnectionOption_ToolTip";
        const string NEW_REPORT_TOOLTIP = "OlapClient_Toolbar_NewReport_ToolTip";
        const string SAVE_REPORT_TOOLTIP = "OlapClient_Toolbar_SaveReport_ToolTip";
        const string SAVE_AS_REPORT_TOOLTIP = "OlapClient_Toolbar_SaveAsReport_ToolTip";
        const string ADD_REPORT_TOOLTIP = "OlapClient_Toolbar_AddReport_ToolTip";
        const string REMOVE_REPORT_TOOLTIP = "OlapClient_Toolbar_RemoveReport_ToolTip";
        const string RENAME_REPORT_TOOLTIP = "OlapClient_Toolbar_RenameReport_ToolTip";
        const string REPORT_LIST_TOOLTIP = "OlapClient_Toolbar_ReportList_ToolTip";
        const string AUTO_EXECUTE_TOOLTIP = "OlapClient_Toolbar_AutoExecute_Tooltip";
        const string SHOW_MDX_TOOLTIP = "OlapClient_Toolbar_ShowMdx_Tooltip";
        const string SHOW_CALCMEMBER_TOOLTIP = "OlapClient_Toolbar_ShowCalcMember_Tooltip";
        const string CREATE_CALCMEMBER_TOOLTIP = "OlapClient_Toolbar_CreateCalcMember_Tooltip";
        const string CREATE_VIRTUALKPI_TOOLTIP = "OlapClient_Toolbar_CreateVirtualKpi_Tooltip";
        const string SHOW_EXPANDERS_TOOLTIP = "OlapClient_Toolbar_ShowExpanders_ToolTip";
        const string COLUMN_FILTER_TOOLTIP = "OlapClient_Toolbar_ColumnFilter_ToolTip";
        const string ROW_FILTER_TOOLTIP = "OlapClient_Toolbar_RowFilter_ToolTip";
        const string SORTING_COLUMN_TOOLTIP = "OlapClient_Toolbar_SortingColumn_ToolTip";
        const string SORTING_ROW_TOOLTIP = "OlapClient_Toolbar_SortingRow_ToolTip";
        const string SHOW_APPEARANCE_DIALOG_TOOLTIP = "OlapClient_ChartToolbar_ShowAppearanceDialog_ToolTip";
        const string SHOW_LEGEND_TOOLTIP = "OlapClient_ChartToolbar_ShowLegend_ToolTip";
        const string CHART_TOOLBAR_COLOR_PALETTE_TOOLTIP = "OlapClient_ChartToolbar_ChartColorPalette_ToolTip";
        const string CHART_TOOLBAR_COLOR_PALETTE_DEFAULT = "OlapClient_ChartToolbar_ChartColorPalette_Default";
        const string CHART_TOOLBAR_COLOR_PALLETE_DEFAULTALPHA = "OlapClient_ChartToolbar_ChartColorPalette_DefaultAlpha";
        const string CHART_TOOLBAR_COLOR_PALETTE_ANALOG = "OlapChart_ChartToolbar_ChartColorPalette_Analog";
        const string CHART_TOOLBAR_COLOR_PALETTE_COLORFUL = "OlapChart_ChartToolbar_ChartColorPalette_Colorful";
        const string CHART_TOOLBAR_COLOR_PALETTE_EARTHTONE = "OlapChart_ChartToolbar_ChartColorPalette_Earthtone";
        const string CHART_TOOLBAR_COLOR_PALLETE_GRAYSCALE = "OlapChart_ChartToolbar_ChartColorPalette_GrayScale";
        const string CHART_TOOLBAR_COLOR_PALETTE_NATURE = "OlapChart_ChartToolbar_ChartColorPalette_Nature";
        const string CHART_TOOLBAR_COLOR_PALETTE_PASTEL = "OlapChart_ChartToolbar_ChartColorPalette_Pastel";
        const string CHART_TOOLBAR_COLOR_PALETTE_TRIAD = "OlapChart_ChartToolbar_ChartColorPalette_Triad";
        const string CHART_TOOLBAR_COLOR_PALETTE_WARMCOLD = "OlapChart_ChartToolbar_ChartColorPalette_WarmCold";
        const string CHART_TOOLBAR_COLOR_PALETTE_CUSTOM = "OlapChart_ChartToolbar_ChartColorPalette_Custom";
        const string CHART_TOOLBAR_ChartTYPES_TOOLTIP = "OlapClient_ChartToolbar_ChartTypes_ToolTip";
        const string CHART_TOOLBAR_CHARTTYPES_COLUMN = "OlapClient_ChartToolbar_ChartTypes_Column";
        const string CHART_TOOLBAR_CHARTTYPES_STACKINGCOLUMN = "OlapClient_ChartToolbar_ChartTypes_StackingColumn";
        const string CHART_TOOLBAR_CHARTTYPES_STACKINGCOLUMN100 = "OlapClient_ChartToolbar_ChartTypes_StackingColumn100";
        const string CHART_TOOLBAR_CHARTTYPES_BAR = "OlapClient_ChartToolbar_ChartTypes_Bar";
        const string CHART_TOOLBAR_CHARTTYPES_STACKINGBAR = "OlapClient_ChartToolbar_ChartTypes_StackingBar";
        const string CHART_TOOLBAR_CHARTTYPES_AREA = "OlapClient_ChartToolbar_ChartTypes_Area";
        const string CHART_TOOLBAR_CHARTTYPES_STACKINGAREA= "OlapClient_ChartToolbar_ChartTypes_StackingArea";
        const string CHART_TOOLBAR_CHARTTYPES_SPLINEAREA = "OlapClient_ChartToolbar_ChartTypes_SplineArea";
        const string CHART_TOOLBAR_CHARTTYPES_STEPAREA = "OlapClient_ChartToolbar_ChartTypes_StepArea";
        const string CHART_TOOLBAR_CHARTTYPES_LINE = "OlapClient_ChartToolbar_ChartTypes_Line";
        const string CHART_TOOLBAR_CHARTTYPES_SPLINE= "OlapClient_ChartToolbar_ChartTypes_Spline";
        const string CHART_TOOLBAR_CHARTTYPES_ROTATEDSPLINE = "OlapClient_ChartToolbar_ChartTypes_RotatedSpline";
        const string CHART_TOOLBAR_CHARTTYPES_STEPLINE = "OlapClient_ChartToolbar_ChartTypes_StepLine";
        const string CHART_TOOLBAR_CHARTTYPES_SCATTER = "OlapClient_ChartToolbar_ChartTypes_Scatter";
        const string CHART_TOOLBAR_CHARTTYPES_PIE = "OlapClient_ChartToolbar_ChartTypes_Pie";
        const string CHART_TOOLBAR_CHARTTYPES_RADAR = "OlapClient_ChartToolbar_ChartTypes_Radar";
        const string CHART_TOOLBAR_CHARTTYPES_FUNNEL = "OlapClient_ChartToolbar_ChartTypes_Funnel";
        const string CHART_TOOLBAR_EXPORT_TOOLTIP = "OlapClient_ChartToolbar_Export_ToolTip";
        const string CHART_TOOLBAR_PRINT_TOOLTIP = "OlapClient_ChartToolbar_Print_ToolTip";
        const string CHART_TOOLBAR_PRINT_MODE_TOOLTIP = "OlapClient_ChartToolbar_PrintMode_ToolTip";
        const string CHART_TOOLBAR_EXPORT_WORD_TOOLTIP = "OlapClient_ChartToolbar_ExportWord_ToolTip";
        const string CHART_TOOLBAR_EXPORT_PDF_TOOLTIP = "OlapClient_ChartToolbar_ExportPdf_ToolTip";
        const string GRID_TOOLBAR_GRID_STYLE_DIALOG_TOOLTIP = "OlapClient_GridToolbar_GridStyleDialog_ToolTip";
        const string GRID_TOOLBAR_GRID_STYLE_NORMAL = "OlapClient_GridToolBar_GridStyle_Normal";
        const string GRID_TOOLBAR_GRID_STYLE_EXCELLIKE = "OlapClient_GridToolBar_GridStyle_ExcelLike";
        const string GRID_TOOLBAR_GRID_STYLE_NOSUMMARY = "OlapClient_GridToolBar_GridStyle_NoSummaries";
        const string GRID_TOOLBAR_GRID_STYLE_NORMALTOPSUMMARY = "OlapClient_GridToolBar_GridStyle_NormalTopSummary";
        const string GRID_TOOLBAR_HEADERCELL_TOOLTIP = "OlapClient_GridToolbar_HeaderCellTooltip_ToolTip";
        const string GRID_TOOLBAR_VALUE_CELL_TOOLTIP = "OlapClient_GridToolbar_ValueCellTooltip_ToolTip";
        const string GRID_TOOLBAR_FREEZE_HEADERS_TOOLTIP = "OlapClient_GridToolbar_FreezeHeaders_ToolTip";
        const string GRID_TOOLBAR_GRID_LAYOUT_TOOLTIP = "OlapClient_GridToolbar_GridLayout_ToolTip";
        const string TOOLBAR_LOAD_REPORT_TOOLTIP = "OlapClient_Toolbar_LoadReport_ToolTip";
        const string GRID_TOOLBAR_EXPORT_EXCEL_TOOLTIP = "OlapClient_GridToolbar_ExportExcel_ToolTip";
        const string GRID_TOOLBAR_EXPORT_WORD_TOOLTIP = "OlapClient_GridToolbar_ExportWord_ToolTip";
        const string GRID_TOOLBAR_EXPORT_PDF_TOOLTIP = "OlapClient_GridToolbar_ExportPdf_ToolTip";
        const string CHART_TAB_HEADER = "OlapClient_ChartTabHeader";
        const string GRID_TAB_HEADER = "OlapClient_GridTabHeader"; 
        const string TOGGLE_PIVOT_TOOLTIP = "OlapClient_TogglePivot_ToolTip";
        const string CUBE_SELECTOR_HEADER_TEXT = "OlapClient_CubeSeletor_HeaderText";
        const string CUBE_SELECTOR_TOOLTIP = "OlapClient_CubeSeletor_ToolTip";
        const string CUBE_DIMENSION_BROWSER_HEADER_TEXT = "OlapClient_CubeDimensionBrowser_HeaderText";
        const string COLUMN_AXIS_HEADER_TEXT = "OlapClient_ColumnAxis_HeaderText";
        const string COLUMN_ROW_SUBSET_FILTER_TOOLTIP = "OlapClient_ColumnRowSubsetFilter_ToolTip";
        const string ROW_AXIS_HEADER_TEXT = "OlapClient_RowAxis_HeaderText";
        const string SLICER_AXIS_HEADER_TEXT = "OlapClient_SlicerAxis_HeaderText";

        #endregion

        #region Constructor
        
        public ClientResourceWrapper()
        {
            CultureInfo ci = CultureInfo.CurrentUICulture;

            connectionOptionToolTipValue = SR.GetString(ci, CONNECTION_OPTION_TOOLTIP);
            newReportToolTipValue = SR.GetString(ci, NEW_REPORT_TOOLTIP);
            saveReportToolTipValue = SR.GetString(ci, SAVE_REPORT_TOOLTIP); ;
            saveAsReportToolTipValue = SR.GetString(ci, SAVE_AS_REPORT_TOOLTIP);
            addReportToolTipValue = SR.GetString(ci, ADD_REPORT_TOOLTIP);
            removeReportToolTipValue = SR.GetString(ci, REMOVE_REPORT_TOOLTIP);
            renameReportToolTipValue = SR.GetString(ci, RENAME_REPORT_TOOLTIP);
            reportListToolTipValue = SR.GetString(ci, REPORT_LIST_TOOLTIP);
            showExpandersToolTipValue = SR.GetString(ci, SHOW_EXPANDERS_TOOLTIP);
            columnFilterToolTipValue = SR.GetString(ci, COLUMN_FILTER_TOOLTIP);
            rowFilterToolTipValue = SR.GetString(ci, ROW_FILTER_TOOLTIP);
            sortingColumnToolTipValue = SR.GetString(ci, SORTING_COLUMN_TOOLTIP);
            sortingRowToolTipValue = SR.GetString(ci, SORTING_ROW_TOOLTIP);
            showAppearanceDialogToolTipValue = SR.GetString(ci, SHOW_APPEARANCE_DIALOG_TOOLTIP);
            showLegendToolTipValue = SR.GetString(ci, SHOW_LEGEND_TOOLTIP);
            chartToolbarColorPaletteToolTipValue = SR.GetString(ci, CHART_TOOLBAR_COLOR_PALETTE_TOOLTIP);
            chartToolbarColorPalette_Default = SR.GetString(ci, CHART_TOOLBAR_COLOR_PALETTE_DEFAULT);
            chartToolbarColorPalette_DefaultAlpha = SR.GetString(ci, CHART_TOOLBAR_COLOR_PALLETE_DEFAULTALPHA);
            chartToolbarColorPalette_Analog = SR.GetString(ci, CHART_TOOLBAR_COLOR_PALETTE_ANALOG);
            chartToolbarColorPalette_Colorful = SR.GetString(ci, CHART_TOOLBAR_COLOR_PALETTE_COLORFUL);
            chartToolbarColorPalette_EarthTone = SR.GetString(ci, CHART_TOOLBAR_COLOR_PALETTE_EARTHTONE);
            chartToolbarColorPalette_GrayScale = SR.GetString(ci, CHART_TOOLBAR_COLOR_PALLETE_GRAYSCALE);
            chartToolbarColorPalette_Nature = SR.GetString(ci, CHART_TOOLBAR_COLOR_PALETTE_NATURE);
            chartToolbarColorPalette_Pastel = SR.GetString(ci, CHART_TOOLBAR_COLOR_PALETTE_PASTEL);
            chartToolbarColorPalette_Triad = SR.GetString(ci, CHART_TOOLBAR_COLOR_PALETTE_TRIAD);
            chartToolbarColorPalette_WarmCold = SR.GetString(ci, CHART_TOOLBAR_COLOR_PALETTE_WARMCOLD);
            chartToolbarColorPalette_Custom = SR.GetString(ci, CHART_TOOLBAR_COLOR_PALETTE_CUSTOM);
            chartToolbarChartTypesTooltipValue = SR.GetString(ci, CHART_TOOLBAR_ChartTYPES_TOOLTIP);
            chartToolbarChartTypes_Column=SR.GetString(ci,CHART_TOOLBAR_CHARTTYPES_COLUMN);
            chartToolbarChartTypes_StackingColumn = SR.GetString(ci, CHART_TOOLBAR_CHARTTYPES_STACKINGCOLUMN);
            chartToolbarChartTypes_StackingColumn100 = SR.GetString(ci, CHART_TOOLBAR_CHARTTYPES_STACKINGCOLUMN100);
            chartToolbarChartTypes_Bar = SR.GetString(ci, CHART_TOOLBAR_CHARTTYPES_BAR);
            chartToolbarChartTypes_StackingBar = SR.GetString(ci, CHART_TOOLBAR_CHARTTYPES_STACKINGBAR);
            chartToolbarChartTypes_Area = SR.GetString(ci, CHART_TOOLBAR_CHARTTYPES_AREA);
            chartToolbarChartTypes_StackingArea = SR.GetString(ci, CHART_TOOLBAR_CHARTTYPES_STACKINGAREA);
            chartToolbarChartTypes_SplineArea = SR.GetString(ci, CHART_TOOLBAR_CHARTTYPES_SPLINEAREA);
            chartToolbarChartTypes_StepArea = SR.GetString(ci, CHART_TOOLBAR_CHARTTYPES_STEPAREA);
            chartToolbarChartTypes_Line = SR.GetString(ci, CHART_TOOLBAR_CHARTTYPES_LINE);
            chartToolbarChartTypes_Spline = SR.GetString(ci, CHART_TOOLBAR_CHARTTYPES_SPLINE);
            chartToolbarChartTypes_RotatedSpline = SR.GetString(ci, CHART_TOOLBAR_CHARTTYPES_ROTATEDSPLINE);
            chartToolbarChartTypes_StepLine = SR.GetString(ci, CHART_TOOLBAR_CHARTTYPES_STEPLINE);
            chartToolbarChartTypes_Scatter = SR.GetString(ci, CHART_TOOLBAR_CHARTTYPES_SCATTER);
            chartToolbarChartTypes_Pie = SR.GetString(ci, CHART_TOOLBAR_CHARTTYPES_PIE);
            chartToolbarChartTypes_Radar = SR.GetString(ci, CHART_TOOLBAR_CHARTTYPES_RADAR);
            chartToolbarChartTypes_Funnel = SR.GetString(ci, CHART_TOOLBAR_CHARTTYPES_FUNNEL);
            chartToolbarExportToolTipValue = SR.GetString(ci, CHART_TOOLBAR_EXPORT_TOOLTIP);
            chartToolbarPrintToolTipValue = SR.GetString(ci, CHART_TOOLBAR_PRINT_TOOLTIP);
            chartToolbarPrintModeToolTipValue = SR.GetString(ci, CHART_TOOLBAR_PRINT_MODE_TOOLTIP);
            chartToolbarExportWordToolTipValue = SR.GetString(ci, CHART_TOOLBAR_EXPORT_WORD_TOOLTIP);
            chartToolbarExportPdfToolTipValue = SR.GetString(ci, CHART_TOOLBAR_EXPORT_PDF_TOOLTIP);
            gridToolbarGridStyleDialogToolTipValue = SR.GetString(ci, GRID_TOOLBAR_GRID_STYLE_DIALOG_TOOLTIP);
            gridToolbarGridStyleNormal = SR.GetString(ci,GRID_TOOLBAR_GRID_STYLE_NORMAL);
            gridToolbarGridStyleExcelLike = SR.GetString(ci,GRID_TOOLBAR_GRID_STYLE_EXCELLIKE);
            gridToolbarGridStyleNoSummaries = SR.GetString(ci,GRID_TOOLBAR_GRID_STYLE_NOSUMMARY);
            gridToolbarGridStyleNormalTopSummary = SR.GetString(ci,GRID_TOOLBAR_GRID_STYLE_NORMALTOPSUMMARY);
            gridToolbarHeaderCellToolTipValue = SR.GetString(ci, GRID_TOOLBAR_HEADERCELL_TOOLTIP);
            gridToolbarValueCellToolTipValue = SR.GetString(ci, GRID_TOOLBAR_VALUE_CELL_TOOLTIP);
            gridToolbarFreezeHeadersToolTipValue = SR.GetString(ci, GRID_TOOLBAR_FREEZE_HEADERS_TOOLTIP);            
            gridToolbarGridLayoutToolTipValue = SR.GetString(ci, GRID_TOOLBAR_GRID_LAYOUT_TOOLTIP);
            toolbarLoadReportToolTipValue = SR.GetString(ci, TOOLBAR_LOAD_REPORT_TOOLTIP);
            gridToolbarExportExcelToolTipValue = SR.GetString(ci, GRID_TOOLBAR_EXPORT_EXCEL_TOOLTIP);
            gridToolbarExportWordToolTipValue = SR.GetString(ci, GRID_TOOLBAR_EXPORT_WORD_TOOLTIP);
            gridToolbarExportPdfToolTipValue = SR.GetString(ci, GRID_TOOLBAR_EXPORT_PDF_TOOLTIP);
            chartTabHeaderValue = SR.GetString(ci, CHART_TAB_HEADER);
            gridTabHeaderValue = SR.GetString(ci, GRID_TAB_HEADER);
            togglePivotToolTipValue = SR.GetString(ci, TOGGLE_PIVOT_TOOLTIP);
            cubeSelectorHeaderText = SR.GetString(ci, CUBE_SELECTOR_HEADER_TEXT);
            cubeSelectorToolTipValue = SR.GetString(ci, CUBE_SELECTOR_TOOLTIP);
            cubeDimensionBrowserHeaderText = SR.GetString(ci, CUBE_DIMENSION_BROWSER_HEADER_TEXT);
            columnAxisHeaderText = SR.GetString(ci, COLUMN_AXIS_HEADER_TEXT);
            columnRowSubsetFilterToolTipValue = SR.GetString(ci, COLUMN_ROW_SUBSET_FILTER_TOOLTIP);
            rowAxisHeaderText = SR.GetString(ci, ROW_AXIS_HEADER_TEXT);
            slicerAxisHeaderText = SR.GetString(ci, SLICER_AXIS_HEADER_TEXT);
            autoExecuteTooltipValue = SR.GetString(ci, AUTO_EXECUTE_TOOLTIP);
            showMdxTooltipValue = SR.GetString(ci, SHOW_MDX_TOOLTIP);
            showCalcMemberTooltipValue = SR.GetString(ci, SHOW_CALCMEMBER_TOOLTIP);
            createCalcMemberTooltipValue = SR.GetString(ci, CREATE_CALCMEMBER_TOOLTIP);
            createVirtualKpiTooltipValue = SR.GetString(ci, CREATE_VIRTUALKPI_TOOLTIP);
        }

        #endregion

        #region Members

        private string connectionOptionToolTipValue;

        private string newReportToolTipValue;

        private string saveReportToolTipValue;

        private string saveAsReportToolTipValue;

        private string addReportToolTipValue;

        private string removeReportToolTipValue;

        private string renameReportToolTipValue;

        private string reportListToolTipValue;

        private string showExpandersToolTipValue;

        private string columnFilterToolTipValue;

        private string rowFilterToolTipValue;

        private string sortingColumnToolTipValue;

        private string sortingRowToolTipValue;

        private string showAppearanceDialogToolTipValue;

        private string showLegendToolTipValue;

        private string chartToolbarColorPaletteToolTipValue;

        private string chartToolbarColorPalette_Default;

        private string chartToolbarColorPalette_DefaultAlpha;

        private string chartToolbarColorPalette_Analog;

        private string chartToolbarColorPalette_Colorful;

        private string chartToolbarColorPalette_EarthTone;

        private string chartToolbarColorPalette_GrayScale;

        private string chartToolbarColorPalette_Nature;

        private string chartToolbarColorPalette_Pastel;

        private string chartToolbarColorPalette_Triad;

        private string chartToolbarColorPalette_WarmCold;

        private string chartToolbarColorPalette_Custom;

        private string chartToolbarChartTypesTooltipValue;

        private string chartToolbarChartTypes_Column;

        private string chartToolbarChartTypes_StackingColumn;

        private string chartToolbarChartTypes_StackingColumn100;

        private string chartToolbarChartTypes_Bar;

        private string chartToolbarChartTypes_StackingBar;

        private string chartToolbarChartTypes_Area;

        private string chartToolbarChartTypes_StackingArea;

        private string chartToolbarChartTypes_SplineArea;

        private string chartToolbarChartTypes_StepArea;

        private string chartToolbarChartTypes_Line;

        private string chartToolbarChartTypes_Spline;

        private string chartToolbarChartTypes_RotatedSpline;

        private string chartToolbarChartTypes_StepLine;

        private string chartToolbarChartTypes_Scatter;

        private string chartToolbarChartTypes_Pie;

        private string chartToolbarChartTypes_Radar;

        private string chartToolbarChartTypes_Funnel;

        private string chartToolbarExportToolTipValue;

        private string chartToolbarPrintToolTipValue;

        private string chartToolbarPrintModeToolTipValue;

        private string chartToolbarExportWordToolTipValue;

        private string chartToolbarExportPdfToolTipValue;

        private string gridToolbarGridStyleDialogToolTipValue;

        private string gridToolbarGridStyleNormal;

        private string gridToolbarGridStyleExcelLike;

        private string gridToolbarGridStyleNoSummaries;

        private string gridToolbarGridStyleNormalTopSummary;

        private string gridToolbarHeaderCellToolTipValue;

        private string gridToolbarValueCellToolTipValue;

        private string gridToolbarFreezeHeadersToolTipValue;

        private string gridToolbarGridLayoutToolTipValue;

        private string toolbarLoadReportToolTipValue;

        private string gridToolbarExportExcelToolTipValue;

        private string gridToolbarExportWordToolTipValue;

        private string gridToolbarExportPdfToolTipValue;

        private string chartTabHeaderValue;

        private string gridTabHeaderValue;

        private string togglePivotToolTipValue;

        private string cubeSelectorHeaderText;

        private string cubeSelectorToolTipValue;

        private string cubeDimensionBrowserHeaderText;

        private string columnAxisHeaderText;

        private string columnRowSubsetFilterToolTipValue;

        private string rowAxisHeaderText;

        private string slicerAxisHeaderText;

        private string autoExecuteTooltipValue;

        private string showMdxTooltipValue;

        private string showCalcMemberTooltipValue;

        private string createCalcMemberTooltipValue;

        private string createVirtualKpiTooltipValue;

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets ConnectionOptionToolTipValue for Localization use
        /// </summary>
        
        public string ConnectionOptionToolTipValue
        {
            get { return connectionOptionToolTipValue; }
            set { connectionOptionToolTipValue = value; }
        }

        /// <summary>
        /// Gets or sets NewReportToolTip for Localization use
        /// </summary>
        public string NewReportToolTipValue
        {
            get { return newReportToolTipValue; }
            set { newReportToolTipValue = value; }
        }

        /// <summary>
        /// Gets or sets SaveReportToolTip for Localization use
        /// </summary>
        public string SaveReportToolTipValue
        {
            get { return saveReportToolTipValue; }
            set { saveReportToolTipValue = value; }
        }

        /// <summary>
        /// Gets or sets SavaAsReportToolTip for Localization use
        /// </summary>
        public string SaveAsReportToolTipValue
        {
            get { return saveAsReportToolTipValue; }
            set { saveAsReportToolTipValue = value; }
        }

        /// <summary>
        /// Gets or sets AddReportToolTip for Localization use
        /// </summary>
        public string AddReportToolTipValue
        {
            get { return addReportToolTipValue; }
            set { addReportToolTipValue = value; }
        }

        /// <summary>
        /// Gets or sets  for Localization use
        /// </summary>
        public string RemoveReportToolTipValue
        {
            get { return removeReportToolTipValue; }
            set { removeReportToolTipValue = value; }
        }

        /// <summary>
        /// Gets or sets RenameReportToolTip for Localization use
        /// </summary>
        public string RenameReportToolTipValue
        {
            get { return renameReportToolTipValue; }
            set { renameReportToolTipValue = value; }
        }

        /// <summary>
        /// Gets or sets ReportListToolTip for Localization use
        /// </summary>
        public string ReportListToolTipValue
        {
            get { return reportListToolTipValue; }
            set { reportListToolTipValue = value; }
        }

        /// <summary>
        /// Gets or sets ShowExpanderToolTip for Localization use
        /// </summary>
        public string ShowExpandersToolTipValue
        {
            get { return showExpandersToolTipValue; }
            set { showExpandersToolTipValue = value; }
        }

        /// <summary>
        /// Gets or sets ColumnFilterToolTip for Localization use
        /// </summary>
        public string ColumnFilterToolTipValue
        {
            get { return columnFilterToolTipValue; }
            set { columnFilterToolTipValue = value; }
        }

        /// <summary>
        /// Gets or sets RowFilterToolTipValue for Localization use
        /// </summary>
        public string RowFilterToolTipValue
        {
            get { return rowFilterToolTipValue; }
            set { rowFilterToolTipValue = value; }
        }

        /// <summary>
        /// Gets or sets SortingColumnToolTip for Localization use
        /// </summary>
        public string SortingColumnToolTipValue
        {
            get { return sortingColumnToolTipValue; }
            set { sortingColumnToolTipValue = value; }
        }

        /// <summary>
        /// Gets or sets SortingRowToolTip for Localization use
        /// </summary>
        public string SortingRowToolTipValue
        {
            get { return sortingRowToolTipValue; }
            set { sortingRowToolTipValue = value; }
        }

        /// <summary>
        /// Gets or sets ShowAppearanceDialogToolTip for Localization use
        /// </summary>
        public string ShowAppearanceDialogToolTipValue
        {
            get { return showAppearanceDialogToolTipValue; }
            set { showAppearanceDialogToolTipValue = value; }
        }

        /// <summary>
        /// Gets or sets ShowLegendToolTip for Localization use
        /// </summary>
        public string ShowLegendToolTipValue
        {
            get { return showLegendToolTipValue; }
            set { showLegendToolTipValue = value; }
        }

        /// <summary>
        /// Gets or sets ColorPaletteToolTip for Localization use
        /// </summary>
        public string ChartToolbarColorPaletteToolTipValue
        {
            get { return chartToolbarColorPaletteToolTipValue; }
            set { chartToolbarColorPaletteToolTipValue = value; }
        }

        /// <summary>
        /// Gets or sets ColorPaletteDefault for Localization use
        /// </summary>
        public string ChartToolbarColorPalette_Default
        {
            get { return chartToolbarColorPalette_Default; }
            set { chartToolbarColorPalette_Default = value; }
        }

        /// <summary>
        /// Gets or sets ColorPaletteDefaultAlpha for Localization use
        /// </summary>
        public string ChartToolbarColorPalette_DefaultAlpha
        {
            get { return chartToolbarColorPalette_DefaultAlpha; }
            set { chartToolbarColorPalette_DefaultAlpha = value; }
        }

        /// <summary>
        /// Gets or sets ColorPaletteAnalog for Localization use
        /// </summary>
        public string ChartToolbarColorPalette_Analog
        {
            get { return chartToolbarColorPalette_Analog; }
            set { chartToolbarColorPalette_Analog = value; }
        }

        /// <summary>
        /// Gets or sets ColorPaletteColorful for Localization use
        /// </summary>
        public string ChartToolbarColorPalette_Colorful
        {
            get { return chartToolbarColorPalette_Colorful; }
            set { chartToolbarColorPalette_Colorful = value; }
        }

        /// <summary>
        /// Gets or sets ColorPaletteEarthTone for Localization use
        /// </summary>
        public string ChartToolbarColorPalette_EarthTone
        {
            get { return chartToolbarColorPalette_EarthTone; }
            set { chartToolbarColorPalette_EarthTone = value; }
        }

        /// <summary>
        /// Gets or sets ColorPaletteGrayScale for Localization use
        /// </summary>
        public string ChartToolbarColorPalette_GrayScale
        {
            get { return chartToolbarColorPalette_GrayScale; }
            set { chartToolbarColorPalette_GrayScale = value; }
        }

        /// <summary>
        /// Gets or sets ColorPaletteNature for Localization use
        /// </summary>
        public string ChartToolbarColorPalette_Nature
        {
            get { return chartToolbarColorPalette_Nature; }
            set { chartToolbarColorPalette_Nature = value; }
        }

        /// <summary>
        /// Gets or sets ColorPalettePastel for Localization use
        /// </summary>
        public string ChartToolbarColorPalette_Pastel
        {
            get { return chartToolbarColorPalette_Pastel; }
            set { chartToolbarColorPalette_Pastel = value; }
        }

        /// <summary>
        /// Gets or sets ColorPaletteTriad for Localization use
        /// </summary>
        public string ChartToolbarColorPalette_Triad
        {
            get { return chartToolbarColorPalette_Triad; }
            set { chartToolbarColorPalette_Triad = value; }
        }

        /// <summary>
        /// Gets or sets ColorPaletteWarmCold for Localization use
        /// </summary>
        public string ChartToolbarColorPalette_WarmCold
        {
            get { return chartToolbarColorPalette_WarmCold; }
            set { chartToolbarColorPalette_WarmCold = value; }
        }

        /// <summary>
        /// Gets or sets ColorPaletteCustom for Localization use
        /// </summary>
        public string ChartToolbarColorPalette_Custom
        {
            get { return chartToolbarColorPalette_Custom; }
            set { chartToolbarColorPalette_Custom = value; }
        }

        /// <summary>
        /// Gets or sets ChartTypeToolTip for Localization use
        /// </summary>
        public string ChartToolbarChartTypesTooltipValue
        {
            get { return chartToolbarChartTypesTooltipValue; }
            set { chartToolbarChartTypesTooltipValue = value; }
        }

        /// <summary>
        /// Gets or sets ChartTypeColumn for Localization use
        /// </summary>
        public string ChartToolbarChartTypes_Column
        {
            get { return chartToolbarChartTypes_Column; }
            set { chartToolbarChartTypes_Column = value; }
        }

        /// <summary>
        /// Gets or sets ChartTypeStackingColumn for Localization use
        /// </summary>
        public string ChartToolbarChartTypes_StackingColumn
        {
            get { return chartToolbarChartTypes_StackingColumn; }
            set { chartToolbarChartTypes_StackingColumn = value; }
        }

        /// <summary>
        /// Gets or sets ChartTypeStackingColumn for Localization use
        /// </summary>
        public string ChartToolbarChartTypes_StackingColumn100
        {
            get { return chartToolbarChartTypes_StackingColumn100; }
            set { chartToolbarChartTypes_StackingColumn100 = value; }
        }

        /// <summary>
        /// Gets or sets ChartTypeBar for Localization use
        /// </summary>
        public string ChartToolbarChartTypes_Bar
        {
            get { return chartToolbarChartTypes_Bar; }
            set { chartToolbarChartTypes_Bar = value; }
        }

        /// <summary>
        /// Gets or sets ChartTypeStackingBar for Localization use
        /// </summary>
        public string ChartToolbarChartTypes_StackingBar
        {
            get { return chartToolbarChartTypes_StackingBar; }
            set { chartToolbarChartTypes_StackingBar = value; }
        }

        /// <summary>
        /// Gets or sets ChartTypeArea for Localization use
        /// </summary>
        public string ChartToolbarChartTypes_Area
        {
            get { return chartToolbarChartTypes_Area; }
            set { chartToolbarChartTypes_Area = value; }
        }

        /// <summary>
        /// Gets or sets ChartTypeStackingArea for Localization use
        /// </summary>
        public string ChartToolbarChartTypes_StackingArea
        {
            get { return chartToolbarChartTypes_StackingArea; }
            set { chartToolbarChartTypes_StackingArea = value; }
        }

        /// <summary>
        /// Gets or sets ChartTypeSplineArea for Localization use
        /// </summary>
        public string ChartToolbarChartTypes_SplineArea
        {
            get { return chartToolbarChartTypes_SplineArea; }
            set { chartToolbarChartTypes_SplineArea = value; }
        }

        /// <summary>
        /// Gets or sets ChartTypeStepArea for Localization use
        /// </summary>
        public string ChartToolbarChartTypes_StepArea
        {
            get { return chartToolbarChartTypes_StepArea; }
            set { chartToolbarChartTypes_StepArea = value; }
        }

        /// <summary>
        /// Gets or sets ChartTypeLine for Localization use
        /// </summary>
        public string ChartToolbarChartTypes_Line
        {
            get { return chartToolbarChartTypes_Line; }
            set { chartToolbarChartTypes_Line = value; }
        }

        /// <summary>
        /// Gets or sets ChartTypeSpline for Localization use
        /// </summary>
        public string ChartToolbarChartTypes_Spline
        {
            get { return chartToolbarChartTypes_Spline; }
            set { chartToolbarChartTypes_Spline = value; }
        }

        /// <summary>
        /// Gets or sets ChartTypeRotatedSpline for Localization use
        /// </summary>
        public string ChartToolbarChartTypes_RotatedSpline
        {
            get { return chartToolbarChartTypes_RotatedSpline; }
            set { chartToolbarChartTypes_RotatedSpline = value; }
        }

        /// <summary>
        /// Gets or sets ChartTypeStepLine for Localization use
        /// </summary>
        public string ChartToolbarChartTypes_StepLine
        {
            get { return chartToolbarChartTypes_StepLine; }
            set { chartToolbarChartTypes_StepLine = value; }
        }

        /// <summary>
        /// Gets or sets ChartTypeScatter for Localization use
        /// </summary>
        public string ChartToolbarChartTypes_Scatter
        {
            get { return chartToolbarChartTypes_Scatter; }
            set { chartToolbarChartTypes_Scatter = value; }
        }

        /// <summary>
        /// Gets or sets ChartTypePie for Localization
        /// </summary>
        public string ChartToolbarChartTypes_Pie
        {
            get { return chartToolbarChartTypes_Pie; }
            set { chartToolbarChartTypes_Pie = value; }
        }
        /// <summary>
        /// Gets or sets ChartTypeRadar for Localization
        /// </summary>
        public string ChartToolbarChartTypes_Radar
        {
            get { return chartToolbarChartTypes_Radar; }
            set { chartToolbarChartTypes_Radar = value; }
        }
        /// <summary>
        /// Gets or sets ChartTypeFunnel for Localization
        /// </summary>
        public string ChartToolbarChartTypes_Funnel
        {
            get { return chartToolbarChartTypes_Funnel; }
            set { chartToolbarChartTypes_Funnel = value; }
        }

        /// <summary>
        /// Gets or sets ExportToolTip for Localization use
        /// </summary>
        public string ChartToolbarExportToolTipValue
        {
            get { return chartToolbarExportToolTipValue; }
            set { chartToolbarExportToolTipValue = value; }
        }

        /// <summary>
        /// Gets or sets PrintToolTip for Localization use
        /// </summary>
        public string ChartToolbarPrintToolTipValue
        {
            get { return chartToolbarPrintToolTipValue; }
            set { chartToolbarPrintToolTipValue = value; }
        }

        /// <summary>
        /// Gets or sets PrintModeToolTip for Localization use
        /// </summary>
        public string ChartToolbarPrintModeToolTipValue
        {
            get { return chartToolbarPrintModeToolTipValue; }
            set { chartToolbarPrintModeToolTipValue = value; }
        }

        /// <summary>
        /// Gets or sets WordExportToolTip for Localization use
        /// </summary>
        public string ChartToolbarExportWordToolTipValue
        {
            get { return chartToolbarExportWordToolTipValue; }
            set { chartToolbarExportWordToolTipValue = value; }
        }

        /// <summary>
        /// Gets or sets PdfExportToolTip for Localization use
        /// </summary>
        public string ChartToolbarExportPdfToolTipValue
        {
            get { return chartToolbarExportPdfToolTipValue; }
            set { chartToolbarExportPdfToolTipValue = value; }
        }

        /// <summary>
        /// Gets or sets GridStyleToolTip for Localization use
        /// </summary>
        public string GridToolbarGridStyleDialogToolTipValue
        {
            get { return gridToolbarGridStyleDialogToolTipValue; }
            set { gridToolbarGridStyleDialogToolTipValue = value; }
        }

        /// <summary>
        /// Gets or sets Normal for Localization use
        /// </summary>
        public string GridToolbarGridStyleNormal
        {
            get { return gridToolbarGridStyleNormal; }
            set { gridToolbarGridStyleNormal = value; }
        }

        /// <summary>
        /// Gets or sets ExcelLike for Localization use
        /// </summary>
        public string GridToolbarGridStyleExcelLike
        {
            get { return gridToolbarGridStyleExcelLike; }
            set { gridToolbarGridStyleExcelLike = value; }
        }

        /// <summary>
        /// Gets or sets NoSummary for Localization use
        /// </summary>
        public string GridToolbarGridStyleNoSummaries
        {
            get { return gridToolbarGridStyleNoSummaries; }
            set { gridToolbarGridStyleNoSummaries = value; }
        }

        /// <summary>
        /// Gets or sets NormalTopSummary for Localization use
        /// </summary>
        public string GridToolbarGridStyleNormalTopSummary
        {
            get { return gridToolbarGridStyleNormalTopSummary; }
            set { gridToolbarGridStyleNormalTopSummary = value; }
        }

        /// <summary>
        /// Gets or sets HeaderCellToolTip for Localization use
        /// </summary>
        public string GridToolbarHeaderCellToolTipValue
        {
            get { return gridToolbarHeaderCellToolTipValue; }
            set { gridToolbarHeaderCellToolTipValue = value; }
        }

        /// <summary>
        /// Gets or sets ValueCellToolTip for Localization use
        /// </summary>
        public string GridToolbarValueCellToolTipValue
        {
            get { return gridToolbarValueCellToolTipValue; }
            set { gridToolbarValueCellToolTipValue = value; }
        }

        /// <summary>
        /// Gets or sets FreezeHeaderToolTip for Localization use
        /// </summary>
        public string GridToolbarFreezeHeadersToolTipValue
        {
            get { return gridToolbarFreezeHeadersToolTipValue; }
            set { gridToolbarFreezeHeadersToolTipValue = value; }
        }

        /// <summary>
        /// Gets or sets GrigLayoutToolTip for Localization use
        /// </summary>
        public string GridToolbarGridLayoutToolTipValue
        {
            get { return gridToolbarGridLayoutToolTipValue; }
            set { gridToolbarGridLayoutToolTipValue = value; }
        }

        /// <summary>
        /// Gets or sets LoadReportToolTip for Localization use
        /// </summary>
        public string ToolbarLoadReportToolTipValue
        {
            get { return toolbarLoadReportToolTipValue; }
            set { toolbarLoadReportToolTipValue = value; }
        }

        /// <summary>
        /// Gets or sets ExcelExportToolTip for Localization use
        /// </summary>
        public string GridToolbarExportExcelToolTipValue
        {
            get { return gridToolbarExportExcelToolTipValue; }
            set { gridToolbarExportExcelToolTipValue = value; }
        }

        /// <summary>
        /// Gets or sets WordExportToolTip for Localization use
        /// </summary>
        public string GridToolbarExportWordToolTipValue
        {
            get { return gridToolbarExportWordToolTipValue; }
            set { gridToolbarExportWordToolTipValue = value; }
        }

        /// <summary>
        /// Gets or sets PdfExportToolTip for Localization use
        /// </summary>
        public string GridToolbarExportPdfToolTipValue
        {
            get { return gridToolbarExportPdfToolTipValue; }
            set { gridToolbarExportPdfToolTipValue = value; }
        }

        /// <summary>
        /// Gets or sets ChartTabHeaderValue for Localization use
        /// </summary>
        public string ChartTabHeaderValue
        {
            get { return chartTabHeaderValue; }
            set { chartTabHeaderValue = value; }
        }

        /// <summary>
        /// Gets or sets GridTabHeaderValue for Localization use
        /// </summary>
        public string GridTabHeaderValue
        {
            get { return gridTabHeaderValue; }
            set { gridTabHeaderValue = value; }
        }

        /// <summary>
        /// Gets or sets TogglePivotToolTipValue for Localization use
        /// </summary>
        public string TogglePivotToolTipValue
        {
            get { return togglePivotToolTipValue; }
            set { togglePivotToolTipValue = value; }
        }

        /// <summary>
        /// Gets or sets CubeSelectorHeaderText for Localization use
        /// </summary>
        public string CubeSelectorHeaderText
        {
            get { return cubeSelectorHeaderText; }
            set { cubeSelectorHeaderText = value; }
        }

        /// <summary>
        /// Gets or sets CubeSelectorToolTipValue for Localization use
        /// </summary>
        public string CubeSelectorToolTipValue
        {
            get { return cubeSelectorToolTipValue; }
            set { cubeSelectorToolTipValue = value; }
        }

        /// <summary>
        /// Gets or sets CubeDimensionBrowserHeaderText for Localization use
        /// </summary>
        public string CubeDimensionBrowserHeaderText
        {
            get { return cubeDimensionBrowserHeaderText; }
            set { cubeDimensionBrowserHeaderText = value; }
        }

        /// <summary>
        /// Gets or sets ColumnAxisHeaderText for Localization use
        /// </summary>
        public string ColumnAxisHeaderText
        {
            get { return columnAxisHeaderText; }
            set { columnAxisHeaderText = value; }
        }

        /// <summary>
        /// Gets or sets ColumnRowSubsetFillerToolTipValue for Localization use
        /// </summary>
        public string ColumnRowSubsetFilterToolTipValue
        {
            get { return columnRowSubsetFilterToolTipValue; }
            set { columnRowSubsetFilterToolTipValue = value; }
        }

        /// <summary>
        /// Gets or sets RowAxisHeaderText for Localization use
        /// </summary>
        public string RowAxisHeaderText
        {
            get { return rowAxisHeaderText; }
            set { rowAxisHeaderText = value; }
        }

        /// <summary>
        /// Gets or sets SlicerAxisHeaderText for Localization use
        /// </summary>
        public string SlicerAxisHeaderText
        {
            get { return slicerAxisHeaderText; }
            set { slicerAxisHeaderText = value; }
        }

        /// <summary>
        /// Gets or sets AutoExecuteToolTip for Localization use
        /// </summary>
        public string AutoExecuteTooltip
        {
            get { return autoExecuteTooltipValue; }
            set { autoExecuteTooltipValue = value; }
        }

        /// <summary>
        /// Gets or sets ShowMdxToolTip for Localization use
        /// </summary>
        public string ShowMdxTooltip
        {
            get { return showMdxTooltipValue; }
            set { showMdxTooltipValue = value; }
        }

        /// <summary>
        /// Gets or sets ShowColcMemberToolTip for Localization use
        /// </summary>
        public string ShowCalcMemberTooltip
        {
            get { return showCalcMemberTooltipValue; }
            set { showCalcMemberTooltipValue = value; }
        }

        /// <summary>
        /// Gets or sets CreateCalcMemberToolTip for Localization use
        /// </summary>
        public string CreateCalcMemberTooltip
        {
            get { return createCalcMemberTooltipValue; }
            set { createCalcMemberTooltipValue = value; }
        }

        /// <summary>
        /// Gets or sets CreateVirtualKpiToolTip for Localization use
        /// </summary>
        public string CreateVirtualKpiTooltip
        {
            get { return createVirtualKpiTooltipValue; }
            set { createVirtualKpiTooltipValue = value; }
        }
        #endregion
    }
}
