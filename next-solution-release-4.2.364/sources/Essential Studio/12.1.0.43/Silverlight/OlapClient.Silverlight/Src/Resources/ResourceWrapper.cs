#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Globalization;

namespace Syncfusion.Silverlight.Client.Olap.Resources
{
    /// <summary>
    /// ResourceWrapper is used to apply the static resource to Silverlight-Components
    /// </summary>
    public sealed class ResourceWrapper
    {
        #region Constants
        const string EXIT_FULLSCREEN_TOOLTIP = "OlapClient_Exit_FullScreen_ToolTip";
        const string VIEW_FULLSCREEN_TOOLTIP = "OlapClient_View_FullScreen_ToolTip";
        const string CALCMEMBER_Remove = "OlapClient_CalcMember_Remove";
        const string CALCMEMBER_Edit = "OlapClient_CalcMember_Edit";
        const string NEWREPORT_TOOLTIP = "OlapToolBarButton_NewReport_ToolTip";
        const string NEWSESSION_TOOLTIP = "OlapToolBarButton_NewSession_ToolTip";
        const string LOADREPORT_TOOLTIP = "OlapToolBarButton_LoadReport_ToolTip";
        const string SAVEREPORT_TOOLTIP = "OlapToolBarButton_SaveReport_ToolTip";
        const string ADDREPORT_TOOLTIP = "OlapToolBarButton_AddReport_ToolTip";
        const string REMOVEREPORT_TOOLTIP = "OlapToolBarButton_RemoveReport_ToolTip";
        const string RENAMEREPORT_TOOLTIP = "OlapToolBarButton_RenameReport_ToolTip";
        const string TOGGLEPIVOT_TOOLTIP = "OlapToolBarButton_TogglePivot_ToolTip";
        const string AUTOEXECUTE_TOOLTIP = "OlapToolBarButton_AutoExecute_ToolTip";
        const string ENABLEPAGING_TOOLTIP = "OlapToolBarButton_EnablePaging_ToolTip";
        const string SHOWEXPANDER_TOOLTIP = "OlapToolBarButton_ShowExpander_ToolTip";
        const string REPORTLIST_TOOLTIP = "OlapToolBar_ComboBox_ReportList_ToolTip";
        const string SHOWMDX_TOOLTIP = "OlapToolBarButton_ShowMdx_ToolTip";
        const string CALCULATEDMEASURE_TOOLTIP = "OlapToolBarButton_CalcMeasure_ToolTip";

        const string CUBESELECTOR = "OlapClient_CubeBrowser_CubeSelector";
        const string CUBEDIMENSIONBROWSER = "OlapClient_CubeBrowser_CubeDimensionBrowser";

        const string AXISELEMENTBUILDER_CATEGORICAL = "OlapClient_AxisElementBuilder_Categorical";
        const string AXISELEMENTBUILDER_SERIES = "OlapClient_AxisElementBuilder_Series";
        const string AXISELEMENTBUILDER_SLICER = "OlapClient_AxisElementBuilder_Slicer";

        const string OLAPCHART_TABHEADER = "OlapClient_OlapChart_TabHeader";
        const string OLAPCHART_SHOWTOOLTIP = "OlapChart_OlapToolBarButton_ShowToolTip_ToolTip";
        const string OLAPCHART_SHOWLEGENDS = "OlapChart_OlapToolBarButton_ShowLegends_ToolTip";
        const string OLAPCHART_CHARTTYPES = "OlapChart_ComboBox_ChartTypes_ToolTip";
        const string OLAPCHARTTYPES_AREA = "OlapChart_ComboBox_ChartTypes_Area";
        const string OLAPCHARTTYPES_BAR = "OlapChart_ComboBox_ChartTypes_Bar";
        const string OLAPCHARTTYPES_COLUMN = "OlapChart_ComboBox_ChartTypes_Column";
        const string OLAPCHARTTYPES_FUNNEL = "OlapChart_ComboBox_ChartTypes_Funnel";
        const string OLAPCHARTTYPES_LINE = "OlapChart_ComboBox_ChartTypes_Line";
        const string OLAPCHARTTYPES_PIE = "OlapChart_ComboBox_ChartTypes_Pie";
        const string OLAPCHARTTYPES_POLAR = "OlapChart_ComboBox_ChartTypes_Polar";
        const string OLAPCHARTTYPES_PYRAMID = "OlapChart_ComboBox_ChartTypes_Pyramid";
        const string OLAPCHARTTYPES_RADAR = "OlapChart_ComboBox_ChartTypes_Radar";
        const string OLAPCHARTTYPES_ROTATEDSPLINE = "OlapChart_ComboBox_ChartTypes_RotatedSpline";
        const string OLAPCHARTTYPES_SCATTER = "OlapChart_ComboBox_ChartTypes_Scatter";
        const string OLAPCHARTTYPES_SPLINE = "OlapChart_ComboBox_ChartTypes_Spline";
        const string OLAPCHARTTYPES_SPLINEAREA = "OlapChart_ComboBox_ChartTypes_SplineArea";
        const string OLAPCHARTTYPES_STACKINGAREA = "OlapChart_ComboBox_ChartTypes_StackingArea";
        const string OLAPCHARTTYPES_STACKINGBAR = "OlapChart_ComboBox_ChartTypes_StackingBar";
        const string OLAPCHARTTYPES_STACKINGBAR100 = "OlapChart_ComboBox_ChartTypes_StackingBar100";
        const string OLAPCHARTTYPES_STACKINGCOLUMN = "OlapChart_ComboBox_ChartTypes_StackingColumn";
        const string OLAPCHARTTYPES_STACKINGCOLUMN100 = "OlapChart_ComboBox_ChartTypes_StackingColumn100";
        const string OLAPCHARTTYPES_STEPAREA = "OlapChart_ComboBox_ChartTypes_StepArea";
        const string OLAPCHARTTYPES_STEPLINE = "OlapChart_ComboBox_ChartTypes_StepLine";
        const string OLAPCHART_COLOTPALETTE = "OlapChart_ComboBox_ColorPalette_ToolTip";
        const string OLAPCHART_COLORPALLETTE_ANALOG = "OlapChart_ColorPalette_Analog";
        const string OLAPCHART_COLORPALLETTE_COLORFUL = "OlapChart_ColorPalette_Colorful";
        const string OLAPCHART_COLORPALLETTE_CUSTOM = "OlapChart_ColorPalette_Custom";
        const string OLAPCHART_COLORPALLETTE_DEFAULT = "OlapChart_ColorPalette_Default";
        const string OLAPCHART_COLORPALLETTE_DEFAULTALPHA = "OlapChart_ColorPalette_DefaultAlpha";
        const string OLAPCHART_COLORPALLETTE_DEFAULTDARK = "OlapChart_ColorPalette_DefaultDark";
        const string OLAPCHART_COLORPALLETTE_EARTHTONE = "OlapChart_ColorPalette_EarthTone";
        const string OLAPCHART_COLORPALLETTE_GRAYSCALE = "OlapChart_ColorPalette_GrayScale";
        const string OLAPCHART_COLORPALLETTE_METRO = "OlapChart_ColorPalette_Metro";
        const string OLAPCHART_COLORPALLETTE_NATURE = "OlapChart_ColorPalette_Nature";
        const string OLAPCHART_COLORPALLETTE_PALETTE1 = "OlapChart_ColorPalette_Palette1";
        const string OLAPCHART_COLORPALLETTE_PALETTE2 = "OlapChart_ColorPalette_Palette2";
        const string OLAPCHART_COLORPALLETTE_PALETTE3 = "OlapChart_ColorPalette_Palette3";
        const string OLAPCHART_COLORPALLETTE_PALETTE4 = "OlapChart_ColorPalette_Palette4";
        const string OLAPCHART_COLORPALLETTE_PALETTE5 = "OlapChart_ColorPalette_Palette5";
        const string OLAPCHART_COLORPALLETTE_PALETTE6 = "OlapChart_ColorPalette_Palette6";
        const string OLAPCHART_COLORPALLETTE_PALETTE7 = "OlapChart_ColorPalette_Palette7";
        const string OLAPCHART_COLORPALLETTE_PALETTE8 = "OlapChart_ColorPalette_Palette8";
        const string OLAPCHART_COLORPALLETTE_PASTEL = "OlapChart_ColorPalette_Pastel";
        const string OLAPCHART_COLORPALLETTE_TRIAD = "OlapChart_ColorPalette_Triad";
        const string OLAPCHART_COLORPALLETTE_WARMCOLD = "OlapChart_ColorPalette_WarmCold";
        const string OLAPGRID_TABHEADER = "OlapClient_OlapGrid_TabHeader";
        const string OLAPGRID_STYLEDIALOG = "OlapGrid_OlapToolBarButton_GridStyleDialog_ToolTip";
        const string OLAPGRID_FREEZEHEADER = "OlapGrid_OlapToolBarButton_FreezeHeader_ToolTip";
        const string OLAPGRID_CELLTOOLTIP = "OlapGrid_OlapToolBarButton_CellToolTip_ToolTip";
        const string OLAPGRID_EXPORTWORD = "OlapGrid_OlapToolBarButton_ExportWord_ToolTip";
        const string OLAPGRID_EXPORTEXCEL = "OlapGrid_OlapToolBarButton_ExportExcel_ToolTip";
        const string OLAPGRID_EXPORTPDF = "OlapGrid_OlapToolBarButton_ExportPDF_ToolTip";
        const string OLAPGRID_LAYOUTS = "OlapGrid_ComboBox_GridLayouts_ToolTip";
        const string OLAPGRIDLAYOUT_NORMAL = "OlapGrid_ComboBox_GridLayouts_Normal";
        const string OLAPGRIDLAYOUT_EXCELLIKE = "OlapGrid_ComboBox_GridLayouts_ExcelLike";
        const string OLAPGRIDLAYOUT_NOSUMMARIES = "OlapGrid_ComboBox_GridLayouts_NoSummaries";
        const string OLAPGRIDLAYOUT_NORMALTOPSUMMARY = "OlapGrid_ComboBox_GridLayouts_Normal_Top_Summary";

        const string CONNECTION_SERVER = "OlapClient_ConnectionDialog_Server";
        const string CONNECTION_SERVERNAME = "OlapClient_ConnectionDialog_Servername";
        const string DATABASENAME = "OlapClient_ConnectionDialog_Databasename";
        const string CONNECTION_CREDENTIALS = "OlapClient_ConnectionDialog_Credentials";
        const string CONNECTION_USERNAME = "OlapClient_ConnectionDialog_Username";
        const string CONNECTION_PASSWORD = "OlapClient_ConnectionDialog_Password";
        const string CONNECTIONSTRING = "OlapClient_ConnectionDialog_ConnectionString";

        const string CONNECTION_PROVIDER_NAME_HEADER = "OlapClient_ConnectionDialog_ProviderName";
        const string CONNECTION_PROVIDER_NAME_SSAS = "OlapClient_ConnectionDialog_ProviderSSAS";
        const string CONNECTION_PROVIDER_NAME_MONDRIAN = "OlapClient_ConnectionDialog_ProviderMondrian";
        const string CONNECTION_PROVIDER_NAME_ACTIVEPIVOT = "OlapClient_ConnectionDialog_ProviderActivePivot";
        const string CONNECTION_PROVIDER_TOOLTIP_SSAS = "OlapClient_ConnectionDialog_ProviderSSAS_ToolTip";
        const string CONNECTION_PROVIDER_TOOLTIP_MONDRIAN = "OlapClient_ConnectionDialog_ProviderMondrian_ToolTip";
        const string CONNECTION_PROVIDER_TOOLTIP_ACTIVEPIVOT = "OlapClient_ConnectionDialog_ProviderActivePivot_ToolTip";


        const string CONNECTION_OK = "OlapClient_ConnectionDialog_Button_Ok";
        const string CONNECTION_CANCEL = "OlapClient_ConnectionDialog_Button_Cancel";
        const string CONNECTION_TITLE = "OlapClient_ConnectionDialog_Title";

        const string REPORT_NAME = "OlapClient_ReportNameGetter_ReportName";
        const string REPORT_ADDREPORT = "OlapClient_ReportNameGetter_AddReport";
        const string REPORT_RENAMEREPORT = "OlapClient_ReportNameGetter_RenameReport";
        const string REPORT_NEWREPORT = "OlapClient_ReportNameGetter_NewReport";

        const string MEMBEREDITOR_CHECKALL = "OlapClient_MemberEditor_CheckAll";

        const string MDX_COPY = "OlapClient_MDXDialog_Copy";

        const string MEASUREEDITOR_MOVEUP = "OlapClient_MeasureEditor_MoveUp";
        const string MEASUREEDITOR_MOVEDOWN = "OlapClient_MeasureEditor_MoveDown";
        const string MEASUREEDITOR_DELETE = "OlapClient_MeasureEditor_Delete";

        const string CALCMEMBEREDITOR_CAPTION = "OlapClient_CalcMemberEditor_Caption";
        const string CALCMEMBEREDITOR_EXPRESSION = "OlapClient_CalcMemberEditor_Expression";
        const string CALCMEMBEREDITOR_MEMBERTYPE = "OlapClient_CalcMemberEditor_MemberType";
        const string CALCMEMBEREDITOR_FORMATSTRING = "OlapClient_CalcMemberEditor_FormatString";
        const string PROGRESSBAR_LOADING = "OlapClient_ProgressBar_Loading";

        const string COPY_MDX = "Tooltip_CopyMDXQueryToClipboard";
        const string TOOLTIP_CLOSE = "Tooltip_Close";
        #endregion

        #region Constructor
        /// <summary>
        /// Initialize a new instance of Syncfusion.OlapClient.Silverlight.Resources.ResourceWrapper class
        /// </summary>
        public ResourceWrapper()
        {
            CultureInfo culture = CultureInfo.CurrentUICulture;
            exitFullScreenToolTip = SR.GetString(culture, EXIT_FULLSCREEN_TOOLTIP);
            viewFullScreenToolTip = SR.GetString(culture, VIEW_FULLSCREEN_TOOLTIP);
            calcMemberRemove = SR.GetString(culture, CALCMEMBER_Remove);
            calcMemberEdit = SR.GetString(culture, CALCMEMBER_Edit);
            newReportToolTip = SR.GetString(culture, NEWREPORT_TOOLTIP);
            newSessionToolTip = SR.GetString(culture, NEWSESSION_TOOLTIP);
            loadReportToolTip = SR.GetString(culture, LOADREPORT_TOOLTIP);
            saveReportToolTip = SR.GetString(culture, SAVEREPORT_TOOLTIP);
            addReportToolTip = SR.GetString(culture, ADDREPORT_TOOLTIP);
            removeReportToolTip = SR.GetString(culture, REMOVEREPORT_TOOLTIP);
            renameReportToolTip = SR.GetString(culture, RENAMEREPORT_TOOLTIP);
            togglePivotToolTip = SR.GetString(culture, TOGGLEPIVOT_TOOLTIP);
            autoExecuteToolTip = SR.GetString(culture, AUTOEXECUTE_TOOLTIP);
            enablePagingToolTip = SR.GetString(culture, ENABLEPAGING_TOOLTIP);
            showExpanderToolTip = SR.GetString(culture, SHOWEXPANDER_TOOLTIP);
            reportListToolTip = SR.GetString(culture, REPORTLIST_TOOLTIP);
            showMdxToolTip = SR.GetString(culture, SHOWMDX_TOOLTIP);
            calcMeasureToolTip = SR.GetString(culture, CALCULATEDMEASURE_TOOLTIP);

            cubeSelector = SR.GetString(culture, CUBESELECTOR);
            cubeDimensionBrowser = SR.GetString(culture, CUBEDIMENSIONBROWSER);

            categoricalAxis = SR.GetString(culture, AXISELEMENTBUILDER_CATEGORICAL);
            seriesAxis = SR.GetString(culture, AXISELEMENTBUILDER_SERIES);
            slicerAxis = SR.GetString(culture, AXISELEMENTBUILDER_SLICER);

            olapChart_TabHeader = SR.GetString(culture, OLAPCHART_TABHEADER);
            olapChart_ShowToolTip = SR.GetString(culture, OLAPCHART_SHOWTOOLTIP);
            olapChart_ShowLegends = SR.GetString(culture, OLAPCHART_SHOWLEGENDS);
            olapChart_ChartTypes = SR.GetString(culture, OLAPCHART_CHARTTYPES);
            olapChartTypes_Area = SR.GetString(culture,OLAPCHARTTYPES_AREA);
            olapChartTypes_Bar = SR.GetString(culture,OLAPCHARTTYPES_BAR);
            olapChartTypes_Column = SR.GetString(culture,OLAPCHARTTYPES_COLUMN);
            olapChartTypes_Funnel = SR.GetString(culture,OLAPCHARTTYPES_FUNNEL);
            olapChartTypes_Line = SR.GetString(culture,OLAPCHARTTYPES_LINE);
            olapChartTypes_Pie = SR.GetString(culture,OLAPCHARTTYPES_PIE);
            olapChartTypes_Polar = SR.GetString(culture,OLAPCHARTTYPES_POLAR);
            olapChartTypes_Pyramid = SR.GetString(culture,OLAPCHARTTYPES_PYRAMID);
            olapChartTypes_Radar = SR.GetString(culture,OLAPCHARTTYPES_RADAR);
            olapChartTypes_RotatedSpline = SR.GetString(culture,OLAPCHARTTYPES_ROTATEDSPLINE);
            olapChartTypes_Scatter = SR.GetString(culture,OLAPCHARTTYPES_SCATTER);
            olapChartTypes_Spline = SR.GetString(culture,OLAPCHARTTYPES_SPLINE);
            olapChartTypes_SplineArea = SR.GetString(culture,OLAPCHARTTYPES_SPLINEAREA);
            olapChartTypes_StackingArea = SR.GetString(culture, OLAPCHARTTYPES_STACKINGAREA);
            olapChartTypes_StackingBar = SR.GetString(culture,OLAPCHARTTYPES_STACKINGBAR);
            olapChartTypes_StackingBar100 = SR.GetString(culture,OLAPCHARTTYPES_STACKINGBAR100);
            olapChartTypes_StackingColumn = SR.GetString(culture,OLAPCHARTTYPES_STACKINGCOLUMN);
            olapChartTypes_StackingColumn100 = SR.GetString(culture,OLAPCHARTTYPES_STACKINGCOLUMN100);
            olapChartTypes_StepArea = SR.GetString(culture,OLAPCHARTTYPES_STEPAREA);
            olapChartTypes_StepLine = SR.GetString(culture,OLAPCHARTTYPES_STEPLINE);
            olapChart_ColorPalette = SR.GetString(culture, OLAPCHART_COLOTPALETTE);
            olapChart_ColorPalette_Analog = SR.GetString(culture,OLAPCHART_COLORPALLETTE_ANALOG);
            olapChart_ColorPalette_Colorful = SR.GetString(culture, OLAPCHART_COLORPALLETTE_COLORFUL);
            olapChart_ColorPalette_Custom = SR.GetString(culture, OLAPCHART_COLORPALLETTE_CUSTOM);
            olapChart_ColorPalette_Default = SR.GetString(culture, OLAPCHART_COLORPALLETTE_DEFAULT);
            olapChart_ColorPalette_DefaultAlpha = SR.GetString(culture, OLAPCHART_COLORPALLETTE_DEFAULTALPHA);
            olapChart_ColorPalette_DefaultDark = SR.GetString(culture, OLAPCHART_COLORPALLETTE_DEFAULTDARK);
            olapChart_ColorPalette_EarthTone = SR.GetString(culture, OLAPCHART_COLORPALLETTE_EARTHTONE);
            olapChart_ColorPalette_GrayScale = SR.GetString(culture, OLAPCHART_COLORPALLETTE_GRAYSCALE);
            olapChart_ColorPalette_Metro = SR.GetString(culture, OLAPCHART_COLORPALLETTE_METRO);
            olapChart_ColorPalette_Nature = SR.GetString(culture, OLAPCHART_COLORPALLETTE_NATURE);
            olapChart_ColorPalette_Palette1 = SR.GetString(culture, OLAPCHART_COLORPALLETTE_PALETTE1);
            olapChart_ColorPalette_Palette2 = SR.GetString(culture, OLAPCHART_COLORPALLETTE_PALETTE2);
            olapChart_ColorPalette_Palette3 = SR.GetString(culture, OLAPCHART_COLORPALLETTE_PALETTE3);
            olapChart_ColorPalette_Palette4 = SR.GetString(culture, OLAPCHART_COLORPALLETTE_PALETTE4);
            olapChart_ColorPalette_Palette5 = SR.GetString(culture, OLAPCHART_COLORPALLETTE_PALETTE5);
            olapChart_ColorPalette_Palette6 = SR.GetString(culture, OLAPCHART_COLORPALLETTE_PALETTE6);
            olapChart_ColorPalette_Palette7 = SR.GetString(culture, OLAPCHART_COLORPALLETTE_PALETTE7);
            olapChart_ColorPalette_Palette8 = SR.GetString(culture, OLAPCHART_COLORPALLETTE_PALETTE8);
            olapChart_ColorPalette_Pastel = SR.GetString(culture, OLAPCHART_COLORPALLETTE_PASTEL);
            olapChart_ColorPalette_Triad = SR.GetString(culture, OLAPCHART_COLORPALLETTE_TRIAD);
            olapChart_ColorPalette_WarmCold = SR.GetString(culture, OLAPCHART_COLORPALLETTE_WARMCOLD);


            olapGrid_TabHeader = SR.GetString(culture,OLAPGRID_TABHEADER);
            olapGrid_StyleDialog = SR.GetString(culture, OLAPGRID_STYLEDIALOG);
            olapGrid_CellToolTip = SR.GetString(culture, OLAPGRID_CELLTOOLTIP);
            olapGrid_FreezeHeader = SR.GetString(culture, OLAPGRID_FREEZEHEADER);
            olapGrid_ExportWord = SR.GetString(culture, OLAPGRID_EXPORTWORD);
            olapGrid_ExportExcel = SR.GetString(culture, OLAPGRID_EXPORTEXCEL);
            olapGrid_ExportPDF = SR.GetString(culture, OLAPGRID_EXPORTPDF);
            olapGrid_Layouts = SR.GetString(culture, OLAPGRID_LAYOUTS);
            olapGridLayout_Normal = SR.GetString(culture,OLAPGRIDLAYOUT_NORMAL);
            olapGridLayout_ExcelLike = SR.GetString(culture,OLAPGRIDLAYOUT_EXCELLIKE);
            olapGridLayout_NoSummaries = SR.GetString(culture,OLAPGRIDLAYOUT_NOSUMMARIES);
            olapGridLayout_NormalTopSummary = SR.GetString(culture,OLAPGRIDLAYOUT_NORMALTOPSUMMARY);

            server = SR.GetString(culture, CONNECTION_SERVER);
            serverName = SR.GetString(culture, CONNECTION_SERVERNAME);
            databaseName = SR.GetString(culture, DATABASENAME);
            credentials = SR.GetString(culture, CONNECTION_CREDENTIALS);
            userName = SR.GetString(culture, CONNECTION_USERNAME);
            password = SR.GetString(culture, CONNECTION_PASSWORD);
            connectionString = SR.GetString(culture, CONNECTIONSTRING);
            connectionDialogTitle = SR.GetString(culture, CONNECTION_TITLE);
            connection_OK = SR.GetString(culture, CONNECTION_OK);
            connection_Cancel = SR.GetString(culture, CONNECTION_CANCEL);

            reportName = SR.GetString(culture, REPORT_NAME);
            report_AddReport = SR.GetString(culture, REPORT_ADDREPORT);
            report_RenameReport = SR.GetString(culture, REPORT_RENAMEREPORT);
            report_NewReport = SR.GetString(culture, REPORT_NEWREPORT);
            memberEditor_CheckAll = SR.GetString(culture, MEMBEREDITOR_CHECKALL);
            mdx_Copy = SR.GetString(culture, MDX_COPY);

            moveUp = SR.GetString(culture, MEASUREEDITOR_MOVEUP);
            moveDown = SR.GetString(culture, MEASUREEDITOR_MOVEDOWN);
            delete = SR.GetString(culture, MEASUREEDITOR_DELETE);

            caption = SR.GetString(culture, CALCMEMBEREDITOR_CAPTION);
            expression = SR.GetString(culture, CALCMEMBEREDITOR_EXPRESSION);
            formatString = SR.GetString(culture, CALCMEMBEREDITOR_FORMATSTRING);
            memberType = SR.GetString(culture, CALCMEMBEREDITOR_MEMBERTYPE);
            loading = SR.GetString(culture, PROGRESSBAR_LOADING);
            copyMDX = SR.GetString(culture, COPY_MDX);

            _providerHeader = SR.GetString(culture, CONNECTION_PROVIDER_NAME_HEADER);
            _providerSSAS = SR.GetString(culture, CONNECTION_PROVIDER_NAME_SSAS);
            _providerMondrian = SR.GetString(culture, CONNECTION_PROVIDER_NAME_MONDRIAN);
            _providerActivePivot = SR.GetString(culture, CONNECTION_PROVIDER_NAME_ACTIVEPIVOT);
            _providerSSASToolTip = SR.GetString(culture, CONNECTION_PROVIDER_TOOLTIP_SSAS);
            _providerMondrianToolTip = SR.GetString(culture, CONNECTION_PROVIDER_TOOLTIP_MONDRIAN);
            _providerActivePivotToolTip = SR.GetString(culture, CONNECTION_PROVIDER_TOOLTIP_ACTIVEPIVOT);

        }

        #endregion

        #region Private Variables
        private string exitFullScreenToolTip;
        private string viewFullScreenToolTip;
        private string calcMemberRemove;
        private string calcMemberEdit;
        private string newReportToolTip;
        private string newSessionToolTip;
        private string loadReportToolTip;
        private string saveReportToolTip;
        private string addReportToolTip;
        private string removeReportToolTip;
        private string renameReportToolTip;
        private string togglePivotToolTip;
        private string autoExecuteToolTip;
        private string enablePagingToolTip;
        private string showMdxToolTip;
        private string showExpanderToolTip;
        private string reportListToolTip;
        private string calcMeasureToolTip;
        private string cubeDimensionBrowser;
        private string cubeSelector;
        private string categoricalAxis;
        private string seriesAxis;
        private string slicerAxis;
        private string olapChart_TabHeader;
        private string olapChart_ShowToolTip;
        private string olapChart_ShowLegends;
        private string olapChart_ChartTypes;
        private string olapChartTypes_Area;
        private string olapChartTypes_Bar;
        private string olapChartTypes_Column;
        private string olapChartTypes_Funnel;
        private string olapChartTypes_Line;
        private string olapChartTypes_Pie;
        private string olapChartTypes_Polar;
        private string olapChartTypes_Pyramid;
        private string olapChartTypes_Radar;
        private string olapChartTypes_RotatedSpline;
        private string olapChartTypes_Scatter;
        private string olapChartTypes_Spline;
        private string olapChartTypes_SplineArea;
        private string olapChartTypes_StackingArea;
        private string olapChartTypes_StackingBar;
        private string olapChartTypes_StackingBar100;
        private string olapChartTypes_StackingColumn;
        private string olapChartTypes_StackingColumn100;
        private string olapChartTypes_StepArea;
        private string olapChartTypes_StepLine;
        private string olapChart_ColorPalette;
        private string olapChart_ColorPalette_Analog;
        private string olapChart_ColorPalette_Colorful;
        private string olapChart_ColorPalette_Custom;
        private string olapChart_ColorPalette_Default;
        private string olapChart_ColorPalette_DefaultAlpha;
        private string olapChart_ColorPalette_DefaultDark;
        private string olapChart_ColorPalette_EarthTone;
        private string olapChart_ColorPalette_GrayScale;
        private string olapChart_ColorPalette_Metro;
        private string olapChart_ColorPalette_Nature;
        private string olapChart_ColorPalette_Palette1;
        private string olapChart_ColorPalette_Palette2;
        private string olapChart_ColorPalette_Palette3;
        private string olapChart_ColorPalette_Palette4;
        private string olapChart_ColorPalette_Palette5;
        private string olapChart_ColorPalette_Palette6;
        private string olapChart_ColorPalette_Palette7;
        private string olapChart_ColorPalette_Palette8;
        private string olapChart_ColorPalette_Pastel;
        private string olapChart_ColorPalette_Triad;
        private string olapChart_ColorPalette_WarmCold;
        private string olapGrid_StyleDialog;
        private string olapGrid_TabHeader;
        private string olapGrid_FreezeHeader;
        private string olapGrid_CellToolTip;
        private string olapGrid_ExportWord;
        private string olapGrid_ExportExcel;
        private string olapGrid_ExportPDF;
        private string olapGrid_Layouts;
        private string olapGridLayout_Normal;
        private string olapGridLayout_ExcelLike;
        private string olapGridLayout_NoSummaries;
        private string olapGridLayout_NormalTopSummary;

        private string server;
        private string serverName;
        private string databaseName;
        private string credentials;
        private string userName;
        private string password;
        private string connectionString;
        private string connection_OK;
        private string connection_Cancel;
        private string connectionDialogTitle;

        private string reportName;
        private string report_AddReport;
        private string report_RenameReport;
        private string report_NewReport;

        private string memberEditor_CheckAll;
        private string mdx_Copy;

        private string moveUp;
        private string moveDown;
        private string delete;

        private string caption;
        private string expression;
        private string memberType;
        private string formatString;
        private string loading;

        private string copyMDX;
        private string close;

        private string _providerHeader;
        private string _providerSSAS;
        private string _providerMondrian;
        private string _providerActivePivot;
        private string _providerSSASToolTip;
        private string _providerMondrianToolTip;
        private string _providerActivePivotToolTip;

        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the localized ToolTip for the Exit Full Screen Button.
        /// </summary>
        public string ExitFullScreenToolTip
        {
            get { return exitFullScreenToolTip; }
            set { exitFullScreenToolTip = value; }
        }
        /// <summary>
        /// Gets or sets the localized ToolTip for the View Full Screen Button.
        /// </summary>
        public string ViewFullScreenToolTip
        {
            get { return viewFullScreenToolTip; }
            set { viewFullScreenToolTip = value; }
        }

        /// <summary>
        /// Gets or sets the CalcMenberRemove for localization use.
        /// </summary>
        public string CalcMemberRemove
        {
            get { return calcMemberRemove; }
            set { calcMemberRemove = value; }
        }
        /// <summary>
        /// Gets or sets the CalcMenberRemove for localization use.
        /// </summary>
        public string CalcMemberEdit
        {
            get { return calcMemberEdit; }
            set { calcMemberEdit = value; }
        }
        /// <summary>
        /// Gets or sets the Close for localization use.
        /// </summary>
        public string Close
        {
            get { return close; }
            set { close = value; }
        }
        
        /// <summary>
        /// Gets or sets the CopyMDX for localization use.
        /// </summary>
        public string CopyMDX
        {
            get { return copyMDX; }
            set { copyMDX = value; }
        }

        /// <summary>
        /// Gets or sets the Loading for localization use.
        /// </summary>
        public string Loading
        {
            get { return loading; }
            set { loading = value; }
        }

        /// <summary>
        /// Gets or sets the Caption for localization use
        /// </summary>
        public string Caption
        {
            get { return caption; }
            set { caption = value; }
        }

        /// <summary>
        /// Gets or sets the Expression for localization use.
        /// </summary>
        public string Expression
        {
            get { return expression; }
            set { expression = value; }
        }

        /// <summary>
        /// Gets or sets the MemberType for localization use.
        /// </summary>
        public string MemberType
        {
            get { return memberType; }
            set { memberType = value; }
        }

        /// <summary>
        /// Gets or sets the FormatString for localization use.
        /// </summary>
        public string FormatString
        {
            get { return formatString; }
            set { formatString = value; }
        }

        /// <summary>
        /// Gets or sets the MoveUp for localization use.
        /// </summary>
        public string MoveUp
        {
            get { return moveUp; }
            set { moveUp = value; }
        }

        /// <summary>
        /// Gets or sets the MoveDown for localization use.
        /// </summary>
        public string MoveDown
        {
            get { return moveDown; }
            set { moveDown = value; }
        }

        /// <summary>
        /// Gets or sets the Delete for localization use.
        /// </summary>
        public string Delete
        {
            get { return delete; }
            set { delete = value; }
        }

        /// <summary>
        /// Gets or sets the MemberEditor_CheckAll for localization use.
        /// </summary>
        public string MemberEditor_CheckAll
        {
            get { return memberEditor_CheckAll; }
            set { memberEditor_CheckAll = value; }
        }

        /// <summary>
        /// Gets or sets the Mdx_Copy for localization use.
        /// </summary>
        public string Mdx_Copy
        {
            get { return mdx_Copy; }
            set { mdx_Copy = value; }
        }

        /// <summary>
        /// Gets or sets the Connection_OK for localization use.
        /// </summary>
        public string Connection_OK
        {
            get { return connection_OK; }
            set { connection_OK = value; }
        }

        /// <summary>
        /// Gets or sets the Connection_Cancel for localization use.
        /// </summary>
        public string Connection_Cancel
        {
            get { return connection_Cancel; }
            set { connection_Cancel = value; }
        }

        /// <summary>
        /// Gets or sets the Server for localization use.
        /// </summary>
        public string Server
        {
            get { return server; }
            set { server = value; }
        }

        /// <summary>
        /// Gets or sets the ServerName for localization use.
        /// </summary>
        public string ServerName
        {
            get { return serverName; }
            set { serverName = value; }
        }

        /// <summary>
        /// Gets or sets the DatabaseName for localization use.
        /// </summary>
        public string DatabaseName
        {
            get { return databaseName; }
            set { databaseName = value; }
        }

        /// <summary>
        /// Gets or sets the Credentials for localization use.
        /// </summary>
        public string Credentials
        {
            get { return credentials; }
            set { credentials = value; }
        }

        /// <summary>
        /// Gets or sets the UserName for localization use.
        /// </summary>
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        /// <summary>
        /// Gets or sets the Password for localization use.
        /// </summary>
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        /// <summary>
        /// Gets or sets the ConnectionString for localization use.
        /// </summary>
        public string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }

        /// <summary>
        /// Gets or sets the ConnectionDialogTitle for localization use.
        /// </summary>
        public string ConnectionDialogTitle
        {
            get { return connectionDialogTitle; }
            set { connectionDialogTitle = value; }
        }

        /// <summary>
        /// Gets or sets the ReportName for localization use.
        /// </summary>
        public string ReportName
        {
            get { return reportName; }
            set { reportName = value; }
        }

        /// <summary>
        /// Gets or sets the Report_AddReport for localization use.
        /// </summary>
        public string Report_AddReport
        {
            get { return report_AddReport; }
            set { report_AddReport = value; }
        }

        /// <summary>
        /// Gets or sets the Report_RenameReport for localization use.
        /// </summary>
        public string Report_RenameReport
        {
            get { return report_RenameReport; }
            set { report_RenameReport = value; }
        }

        /// <summary>
        /// Gets or sets the Report_NewReport for localization use.
        /// </summary>
        public string Report_NewReport
        {
            get { return report_NewReport; }
            set { report_NewReport = value; }
        }

        /// <summary>
        /// Gets or sets the OlapChart_TabHeader for localization use.
        /// </summary>
        public string OlapChart_TabHeader
        {
            get { return olapChart_TabHeader; }
            set { olapChart_TabHeader = value; }
        }


        /// <summary>
        /// Gets or sets the OlapChart_ShowToolTip for localization use.
        /// </summary> 
        public string OlapChart_ShowToolTip
        {
            get { return olapChart_ShowToolTip; }
            set { olapChart_ShowToolTip = value; }
        }


        /// <summary>
        /// Gets or sets the OlapChart_ShowLegends for localization use.
        /// </summary>
        public string OlapChart_ShowLegends
        {
            get { return olapChart_ShowLegends; }
            set { olapChart_ShowLegends = value; }
        }

        /// <summary>
        /// Gets or sets the OlapChart_ChartTypes for localization use.
        /// </summary>
        public string OlapChart_ChartTypes
        {
            get { return olapChart_ChartTypes; }
            set { olapChart_ChartTypes = value; }
        }
        /// <summary>
        /// Gets or sets the OlapChartTypes_Area for localization
        /// </summary>
        public string OlapChartTypes_Area
        {
            get { return olapChartTypes_Area; }
            set { olapChartTypes_Area = value; }
        }
        /// <summary>
        /// Gets or sets the OlapChartTypes_Bar for localization
        /// </summary>
        public string OlapChartTypes_Bar
        {
            get { return olapChartTypes_Bar; }
            set { olapChartTypes_Bar = value; }
        }
        /// <summary>
        /// Gets or sets the OlapChartTypes_Column for localization
        /// </summary>
        public string OlapChartTypes_Column
        {
            get { return olapChartTypes_Column; }
            set { olapChartTypes_Column = value; }
        }
        /// <summary>
        /// Gets or sets the OlapChartTypes_Funnel for localization
        /// </summary>
        public string OlapChartTypes_Funnel
        {
            get { return olapChartTypes_Funnel; }
            set { olapChartTypes_Funnel = value; }
        }
        /// <summary>
        /// Gets or sets the OlapChartTypes_Line for localization
        /// </summary>
        public string OlapChartTypes_Line
        {
            get { return olapChartTypes_Line; }
            set { olapChartTypes_Line = value; }
        }
        /// <summary>
        /// Gets or sets the OlapChartTypes_Pie for localization
        /// </summary>
        public string OlapChartTypes_Pie
        {
            get { return olapChartTypes_Pie; }
            set { olapChartTypes_Pie = value; }
        }
        /// <summary>
        /// Gets or sets the OlapChartTypes_Polar for localization
        /// </summary>
        public string OlapChartTypes_Polar
        {
            get { return olapChartTypes_Polar; }
            set { olapChartTypes_Polar = value; }
        }
        /// <summary>
        /// Gets or sets the OlapChartTypes_Pyramid for localization
        /// </summary>
        public string OlapChartTypes_Pyramid
        {
            get { return olapChartTypes_Pyramid; }
            set { olapChartTypes_Pyramid = value; }
        }
        /// <summary>
        /// Gets or sets the OlapChartTypes_Radar for localization
        /// </summary>
        public string OlapChartTypes_Radar
        {
            get { return olapChartTypes_Radar; }
            set { olapChartTypes_Radar = value; }
        }
        /// <summary>
        /// Gets or sets the OlapChartTypes_RotatedSpline for localization
        /// </summary>
        public string OlapChartTypes_RotatedSpline
        {
            get { return olapChartTypes_RotatedSpline; }
            set { olapChartTypes_RotatedSpline = value; }
        }
        /// <summary>
        /// Gets or sets the OlapChartTypes_Scatter for localization
        /// </summary>
        public string OlapChartTypes_Scatter
        {
            get { return olapChartTypes_Scatter; }
            set { olapChartTypes_Scatter = value; }
        }
        /// <summary>
        /// Gets or sets the OlapChartTypes_Spline for localization
        /// </summary>
        public string OlapChartTypes_Spline
        {
            get { return olapChartTypes_Spline; }
            set { olapChartTypes_Spline = value; }
        }
        /// <summary>
        /// Gets or sets the OlapChartTypes_SplineArea for localization
        /// </summary>
        public string OlapChartTypes_SplineArea
        {
            get { return olapChartTypes_SplineArea; }
            set { olapChartTypes_SplineArea = value; }
        }
        /// <summary>
        /// Gets or sets the OlapChartTypes_StackingArea for localization
        /// </summary>
        public string OlapChartTypes_StackingArea
        {
            get { return olapChartTypes_StackingArea; }
            set { olapChartTypes_StackingArea = value; }
        }
        /// <summary>
        /// Gets or sets the OlapChartTypes_StackingBar for localization
        /// </summary>
        public string OlapChartTypes_StackingBar
        {
            get { return olapChartTypes_StackingBar; }
            set { olapChartTypes_StackingBar = value; }
        }
        /// <summary>
        /// Gets or sets the OlapChartTypes_StackingBar100 for localization
        /// </summary>
        public string OlapChartTypes_StackingBar100
        {
            get { return olapChartTypes_StackingBar100; }
            set { olapChartTypes_StackingBar100 = value; }
        }
        /// <summary>
        /// Gets or sets the OlapChartTypes_StackingColumn for localization
        /// </summary>
        public string OlapChartTypes_StackingColumn
        {
            get { return olapChartTypes_StackingColumn; }
            set { olapChartTypes_StackingColumn = value; }
        }
        /// <summary>
        /// Gets or sets the OlapChartTypes_StackingColumn100 for localization
        /// </summary>
        public string OlapChartTypes_StackingColumn100
        {
            get { return olapChartTypes_StackingColumn100; }
            set { olapChartTypes_StackingColumn100 = value; }
        }
        /// <summary>
        /// Gets or sets the OlapChartTypes_StepArea for localization
        /// </summary>
        public string OlapChartTypes_StepArea
        {
            get { return olapChartTypes_StepArea; }
            set { olapChartTypes_StepArea = value; }
        }
        /// <summary>
        /// Gets or sets the OlapChartTypes_StepLine
        /// </summary>
        public string OlapChartTypes_StepLine
        {
            get { return olapChartTypes_StepLine; }
            set { olapChartTypes_StepLine = value; }
        }
        /// <summary>
        /// Gets or sets the OlapChart_ColorPalette for localization use.
        /// </summary>
        public string OlapChart_ColorPalette
        {
            get { return olapChart_ColorPalette; }
            set { olapChart_ColorPalette = value; }
        }
        /// <summary>
        /// Gets or sets the OlapChart_ColorPalette_Analog for localization use
        /// </summary>
        public string OlapChart_ColorPalette_Analog
        {
            get { return olapChart_ColorPalette_Analog; }
            set { olapChart_ColorPalette_Analog = value; }
        }
        /// <summary>
        /// Gets or sets the OlapChart_ColorPalette_Colorful for localization use
        /// </summary>
        public string OlapChart_ColorPalette_Colorful
        {
            get { return olapChart_ColorPalette_Colorful; }
            set { olapChart_ColorPalette_Colorful = value; }
        }
        /// <summary>
        /// Gets or sets the OlapChart_ColorPalette_Custom for localization use
        /// </summary>
        public string OlapChart_ColorPalette_Custom
        {
            get { return olapChart_ColorPalette_Custom; }
            set { olapChart_ColorPalette_Custom = value; }
        }
        /// <summary>
        /// Gets or sets the OlapChart_ColorPalette_Default for localization use
        /// </summary>
        public string OlapChart_ColorPalette_Default
        {
            get { return olapChart_ColorPalette_Default; }
            set { olapChart_ColorPalette_Default = value; }
        }
        /// <summary>
        /// Gets or sets the OlapChart_ColorPalette_DefaultAlpha for localization use
        /// </summary>
        public string OlapChart_ColorPalette_DefaultAlpha
        {
            get { return olapChart_ColorPalette_DefaultAlpha; }
            set { olapChart_ColorPalette_DefaultAlpha = value; }
        }
        /// <summary>
        /// Gets or sets the OlapChart_ColorPalette_DefaultDark for localization use
        /// </summary>
        public string OlapChart_ColorPalette_DefaultDark
        {
            get { return olapChart_ColorPalette_DefaultDark; }
            set { olapChart_ColorPalette_DefaultDark = value; }
        }
        /// <summary>
        /// Gets or sets the OlapChart_ColorPalette_EarthTone for localization use
        /// </summary>
        public string OlapChart_ColorPalette_EarthTone
        {
            get { return olapChart_ColorPalette_EarthTone; }
            set { olapChart_ColorPalette_EarthTone = value; }
        }
        /// <summary>
        /// Gets or sets the OlapChart_ColorPalette_GrayScale for localization use
        /// </summary>
        public string OlapChart_ColorPalette_GrayScale
        {
            get { return olapChart_ColorPalette_GrayScale; }
            set { olapChart_ColorPalette_GrayScale = value; }
        }
        /// <summary>
        /// Gets or sets the OlapChart_ColorPalette_Metro for localization use
        /// </summary>
        public string OlapChart_ColorPalette_Metro
        {
            get { return olapChart_ColorPalette_Metro; }
            set { olapChart_ColorPalette_Metro = value; }
        }
        /// <summary>
        /// Gets or sets the OlapChart_ColorPalette_Nature for localization use
        /// </summary>
        public string OlapChart_ColorPalette_Nature
        {
            get { return olapChart_ColorPalette_Nature; }
            set { olapChart_ColorPalette_Nature= value; }
        }
        /// <summary>
        /// Gets or sets the OlapChart_ColorPalette_Palette1 for localization use
        /// </summary>
        public string OlapChart_ColorPalette_Palette1
        {
            get { return olapChart_ColorPalette_Palette1; }
            set { olapChart_ColorPalette_Palette1 = value; }
        }
        /// <summary>
        /// Gets or sets the OlapChart_ColorPalette_Palette2 for localization use
        /// </summary>
        public string OlapChart_ColorPalette_Palette2
        {
            get { return olapChart_ColorPalette_Palette2; }
            set { olapChart_ColorPalette_Palette2 = value; }
        }
        /// <summary>
        /// Gets or sets the OlapChart_ColorPalette_Palette3 for localization use
        /// </summary>
        public string OlapChart_ColorPalette_Palette3
        {
            get { return olapChart_ColorPalette_Palette3; }
            set { olapChart_ColorPalette_Palette3 = value; }
        }
        /// <summary>
        /// Gets or sets the OlapChart_ColorPalette_Palette4 for localization use
        /// </summary>
        public string OlapChart_ColorPalette_Palette4
        {
            get { return olapChart_ColorPalette_Palette4; }
            set { olapChart_ColorPalette_Palette4 = value; }
        }
        /// <summary>
        /// Gets or sets the OlapChart_ColorPalette_Palette5 for localization use
        /// </summary>
        public string OlapChart_ColorPalette_Palette5
        {
            get { return olapChart_ColorPalette_Palette5; }
            set { olapChart_ColorPalette_Palette5 = value; }
        }
        /// <summary>
        /// Gets or sets the OlapChart_ColorPalette_Palette6 for localization use
        /// </summary>
        public string OlapChart_ColorPalette_Palette6
        {
            get { return olapChart_ColorPalette_Palette6; }
            set { olapChart_ColorPalette_Palette6 = value; }
        }
        /// <summary>
        /// Gets or sets the OlapChart_ColorPalette_Palette7 for localization use
        /// </summary>
        public string OlapChart_ColorPalette_Palette7
        {
            get { return olapChart_ColorPalette_Palette7; }
            set { olapChart_ColorPalette_Palette7 = value; }
        }
        /// <summary>
        /// Gets or sets the OlapChart_ColorPalette_Palette8 for localization use
        /// </summary>
        public string OlapChart_ColorPalette_Palette8
        {
            get { return olapChart_ColorPalette_Palette8; }
            set { olapChart_ColorPalette_Palette8 = value; }
        }
        /// <summary>
        /// Gets or sets the OlapChart_ColorPalette_Pastel for localization use
        /// </summary>
        public string OlapChart_ColorPalette_Pastel
        {
            get { return olapChart_ColorPalette_Pastel; }
            set { olapChart_ColorPalette_Pastel = value; }
        }
        /// <summary>
        /// Gets or sets the OlapChart_ColorPalette_Triad for localization use
        /// </summary>
        public string OlapChart_ColorPalette_Triad
        {
            get { return olapChart_ColorPalette_Triad; }
            set { olapChart_ColorPalette_Triad = value; }
        }
        /// <summary>
        /// Gets or sets the OlapChart_ColorPalette_WarmCold for localization use
        /// </summary>
        public string OlapChart_ColorPalette_WarmCold
        {
            get { return olapChart_ColorPalette_WarmCold; }
            set { olapChart_ColorPalette_WarmCold = value; }
        }

        /// <summary>
        /// Gets or sets the OlapGrid_TabHeader for localization use.
        /// </summary>
        public string OlapGrid_TabHeader
        {
            get { return olapGrid_TabHeader; }
            set { olapGrid_TabHeader = value; }
        }


        /// <summary>
        /// Gets or sets the OlapGrid_StyleDialog for localization use.
        /// </summary>
        public string OlapGrid_StyleDialog
        {
            get { return olapGrid_StyleDialog; }
            set { olapGrid_StyleDialog = value; }
        }


        /// <summary>
        /// Gets or sets the OlapGrid_FreezeHeader for localization use.
        /// </summary>
        public string OlapGrid_FreezeHeader
        {
            get { return olapGrid_FreezeHeader; }
            set { olapGrid_FreezeHeader = value; }
        }

        /// <summary>
        /// Gets or sets the OlapGrid_CellToolTip for localization use.
        /// </summary>
        public string OlapGrid_CellToolTip
        {
            get { return olapGrid_CellToolTip; }
            set { olapGrid_CellToolTip = value; }
        }

        /// <summary>
        /// Gets or sets the OlapGrid_ExportWord for localization use.
        /// </summary>
        public string OlapGrid_ExportWord
        {
            get { return olapGrid_ExportWord; }
            set { olapGrid_ExportWord = value; }
        }

        /// <summary>
        /// Gets or sets the OlapGrid_ExportExcel for localization use.
        /// </summary>
        public string OlapGrid_ExportExcel
        {
            get { return olapGrid_ExportExcel; }
            set { olapGrid_ExportExcel = value; }
        }

        /// <summary>
        /// Gets or sets the OlapGrid_ExportPDF for localization use.
        /// </summary>
        public string OlapGrid_ExportPDF
        {
            get { return olapGrid_ExportPDF; }
            set { olapGrid_ExportPDF = value; }
        }

        /// <summary>
        /// Gets or sets the OlapGrid_Layouts for localization use.
        /// </summary>
        public string OlapGrid_Layouts
        {
            get { return olapGrid_Layouts; }
            set { olapGrid_Layouts = value; }
        }
        /// <summary>
        /// Gets or sets the OlapGridLayout_Normal for localization use
        /// </summary>
        public string OlapGridLayout_Normal
        {
            get { return olapGridLayout_Normal; }
            set { olapGridLayout_Normal = value; }
        }
        /// <summary>
        /// Gets or sets the OlapGridLayout_ExcelLike
        /// </summary>
        public string OlapGridLayout_ExcelLike
        {
            get { return olapGridLayout_ExcelLike; }
            set { olapGridLayout_ExcelLike = value; }
        }
        /// <summary>
        /// Gets or sets the OlapGridLayout_NoSummaries
        /// </summary>
        public string OlapGridLayout_NoSummaries
        {
            get { return olapGridLayout_NoSummaries; }
            set { olapGridLayout_NoSummaries = value; }
        }
        /// <summary>
        /// Gets or sets the OlapGridLayout_NormalTopSummary
        /// </summary>
        public string OlapGridLayout_NormalTopSummary
        {
            get { return olapGridLayout_NormalTopSummary; }
            set { olapGridLayout_NormalTopSummary = value; }
        }

        /// <summary>
        /// Gets or sets the CategoricalAxis for localization use.
        /// </summary>
        public string CategoricalAxis
        {
            get { return categoricalAxis; }
            set { categoricalAxis = value; }
        }

        /// <summary>
        /// Gets or sets the SeriesAxis for localization use.
        /// </summary>
        public string SeriesAxis
        {
            get { return seriesAxis; }
            set { seriesAxis = value; }
        }

        /// <summary>
        /// Gets or sets the SlicerAxis for localization use.
        /// </summary>
        public string SlicerAxis
        {
            get { return slicerAxis; }
            set { slicerAxis = value; }
        }

        /// <summary>
        /// Gets or sets the CubeSelector for localization use.
        /// </summary>
        public string CubeSelector
        {
            get { return cubeSelector; }
            set { cubeSelector = value; }
        }

        /// <summary>
        /// Gets or sets the CubeDimensionBrowser for localization use.
        /// </summary>
        public string CubeDimensionBrowser
        {
            get { return cubeDimensionBrowser; }
            set { cubeDimensionBrowser = value; }
        }

        /// <summary>
        /// Gets or sets the NewReportToolTip for localization use.
        /// </summary>
        public string NewReportToolTip
        {
            get { return newReportToolTip; }
            set { newReportToolTip = value; }
        }

        /// <summary>
        /// Gets or sets the NewSessionToolTip for localization use.
        /// </summary>
        public string NewSessionToolTip
        {
            get { return newSessionToolTip; }
            set { newSessionToolTip = value; }
        }

        /// <summary>
        /// Gets or sets the LoadReportToolTip for localization use.
        /// </summary>
        public string LoadReportToolTip
        {
            get { return loadReportToolTip; }
            set { loadReportToolTip = value; }
        }

        /// <summary>
        /// Gets or sets the SaveReportToolTip for localization use.
        /// </summary>
        public string SaveReportToolTip
        {
            get { return saveReportToolTip; }
            set { saveReportToolTip = value; }
        }

        /// <summary>
        /// Gets or sets the AddReportToolTip for localization use.
        /// </summary>
        public string AddReportToolTip
        {
            get { return addReportToolTip; }
            set { addReportToolTip = value; }
        }

        /// <summary>
        /// Gets or sets the RemoveReportToolTip for localization use.
        /// </summary>
        public string RemoveReportToolTip
        {
            get { return removeReportToolTip; }
            set { removeReportToolTip = value; }
        }

        /// <summary>
        /// Gets or sets the RenameReportToolTip for localization use.
        /// </summary>
        public string RenameReportToolTip
        {
            get { return renameReportToolTip; }
            set { renameReportToolTip = value; }
        }

        /// <summary>
        /// Gets or sets the TogglePivotToolTip for localization use.
        /// </summary>
        public string TogglePivotToolTip
        {
            get { return togglePivotToolTip; }
            set { togglePivotToolTip = value; }
        }

        /// <summary>
        /// Gets or sets the AutoExecuteToolTip for localization use.
        /// </summary>
        public string AutoExecuteToolTip
        {
            get { return autoExecuteToolTip; }
            set { autoExecuteToolTip = value; }
        }

        /// <summary>
        /// Gets or sets the EnablePagingToolTip for localization use.
        /// </summary>
        public string EnablePagingToolTip
        {
            get { return enablePagingToolTip; }
            set { enablePagingToolTip = value; }
        }

        /// <summary>
        /// Gets or sets the ShowMdxToolTip for localization use.
        /// </summary>
        public string ShowMdxToolTip
        {
            get { return showMdxToolTip; }
            set { showMdxToolTip = value; }
        }

        /// <summary>
        /// Gets or sets the ShowExpanderToolTip for localization use.
        /// </summary>
        public string ShowExpanderToolTip
        {
            get { return showExpanderToolTip; }
            set { showExpanderToolTip = value; }
        }

        /// <summary>
        /// Gets or sets the ReportListToolTip for localization use.
        /// </summary>
        public string ReportListToolTip
        {
            get { return reportListToolTip; }
            set { reportListToolTip = value; }
        }

        /// <summary>
        /// Gets or sets the CalcMeasureToolTip for localization use.
        /// </summary>
        public string CalcMeasureToolTip
        {
            get { return calcMeasureToolTip; }
            set { calcMeasureToolTip = value; }
        }
        
        /// <summary>
        /// Gets or sets the localized header text for provider name.
        /// </summary>
        public string ProviderHeader
        {
            get { return _providerHeader; }
            set { _providerHeader = value; }
        }

        /// <summary>
        /// Gets or sets the localized text for SSAS provider.
        /// </summary>
        public string ProviderSSAS
        {
            get { return _providerSSAS; }
            set { _providerSSAS = value; }
        }

        /// <summary>
        /// Gets or sets the localized text for Mondrian provider.
        /// </summary>
        public string ProviderMondrian
        {
            get { return _providerMondrian; }
            set { _providerMondrian = value; }
        }

        /// <summary>
        /// Gets or sets the localized text for Active Pivot provider.
        /// </summary>
        public string ProviderActivePivot
        {
            get { return _providerActivePivot; }
            set { _providerActivePivot = value; }
        }

        /// <summary>
        /// Gets or set the localized ToolTip text for SSAS service.
        /// </summary>
        public string ProviderSSASToolTip
        {
            get { return _providerSSASToolTip; }
            set { _providerSSASToolTip = value; }
        }

        /// <summary>
        /// Gets or sets the localized ToolTip text for Mondrian service.
        /// </summary>
        public string ProviderMondrianToolTip
        {
            get { return _providerMondrianToolTip; }
            set { _providerMondrianToolTip = value; }
        }

        /// <summary>
        /// Gets or sets the localized ToolTip text for the Active Pivot service.
        /// </summary>
        public string ProviderActivePivotToolTip
        {
            get { return _providerActivePivotToolTip; }
            set { _providerActivePivotToolTip = value; }
        }


        #endregion
    }
}
