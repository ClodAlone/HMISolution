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

namespace Syncfusion.Silverlight.Grid.Olap.Resources
{
    /// <summary>
    /// ResourceWrapper is used to apply the static resource to Silverlight-Components
    /// </summary>
    public sealed class ResourceWrapper
    {
        #region Constants

        const string TOOLTIP_MEASURE = "OlapGrid_ToolTip_Measure";
        const string TOOLTIP_VALUE = "OlapGrid_ToolTip_Value";
        const string TOOLTIP_COLUMN = "OlapGrid_ToolTip_Column";
        const string TOOLTIP_ROW = "OlapGrid_ToolTip_Row";

        const string HEADERSTYLE = "OlapGrid_StyleDialog_HeaderStyle";
        const string COLUMNHEADER = "OlapGrid_StyleDialog_ColumnHeader";
        const string BACKGROUNDCOLOR = "OlapGrid_StyleDialog_BackgroundColor";
        const string FOREGROUNDCOLOR = "OlapGrid_StyleDialog_ForegroundColor";
        const string ROWHEADER = "OlapGrid_StyleDialog_RowHeader";
        const string HEADERFONT = "OlapGrid_StyleDialog_HeaderFont";
        const string FONTSIZE = "OlapGrid_StyleDialog_FontSize";
        const string FONTNAME = "OlapGrid_StyleDialog_FontName";
        const string SUMMARYSTYLE = "OlapGrid_StyleDialog_SummaryStyle";
        const string COLUMNSUMMARY = "OlapGrid_StyleDialog_ColumnSummary";
        const string ROWSUMMARY = "OlapGrid_StyleDialog_RowSummary";
        const string SUMMARY_FONT = "OlapGrid_StyleDialog_SummaryFont";
        const string CELLSTYLES = "OlapGrid_StyleDialog_CellStyles";
        const string FONTSTYLE = "OlapGrid_StyleDialog_FontStyle";
        const string FONTCOLOR = "OlapGrid_StyleDialog_FontColor";
        const string COMMON = "OlapGrid_StyleDialog_Common";
        const string COMMON_STYLES = "OlapGrid_StyleDialog_CommonStyles";
        const string GRIDLINE_COLOR = "OlapGrid_StyleDialog_GridlineColor";
        const string BUTTON_OK = "OlapGrid_StyleDialog_Button_OK";
        const string BUTTON_CANCEL = "OlapGrid_StyleDialog_Button_Cancel";
        const string CELL_FONTSTYLES = "OlapGrid_StyleDialog_CellFontStyle";

        const string LOADINGINDICATOR_LOADING = "LoadingIndicator_Loading";

        #endregion

        #region Constructor

        public ResourceWrapper()
        {
            CultureInfo culture = CultureInfo.CurrentUICulture;
            tooltipColumn = SR.GetString(culture, TOOLTIP_COLUMN);
            tooltipMeasure = SR.GetString(culture, TOOLTIP_MEASURE);
            tooltipValue = SR.GetString(culture, TOOLTIP_VALUE);
            tooltipRow = SR.GetString(culture, TOOLTIP_ROW);

            headerStyle = SR.GetString(culture, HEADERSTYLE);
            columnHeader = SR.GetString(culture, COLUMNHEADER);
            background = SR.GetString(culture, BACKGROUNDCOLOR);
            foreground = SR.GetString(culture, FOREGROUNDCOLOR);
            rowHeader = SR.GetString(culture, ROWHEADER);
            headerFont = SR.GetString(culture, HEADERFONT);
            fontSize = SR.GetString(culture, FONTSIZE);
            fontName = SR.GetString(culture, FONTNAME);
            summaryStyle = SR.GetString(culture, SUMMARYSTYLE);
            columnSummary = SR.GetString(culture, COLUMNSUMMARY);
            rowSummary = SR.GetString(culture, ROWSUMMARY);
            summaryFont = SR.GetString(culture, SUMMARY_FONT);
            cellStyles = SR.GetString(culture, CELLSTYLES);
            cellFontStyles = SR.GetString(culture, CELL_FONTSTYLES);
            fontStyle = SR.GetString(culture, FONTSTYLE);
            fontColor = SR.GetString(culture, FONTCOLOR);
            common = SR.GetString(culture, COMMON);
            commonStyles = SR.GetString(culture, COMMON_STYLES);
            gridlineColor = SR.GetString(culture, GRIDLINE_COLOR);
            button_OK = SR.GetString(culture, BUTTON_OK);
            button_Cancel = SR.GetString(culture, BUTTON_CANCEL);

            loading = SR.GetString(culture, LOADINGINDICATOR_LOADING);
        }

        #endregion

        #region Private Variable

        private string tooltipMeasure;
        private string tooltipValue;
        private string tooltipColumn;
        private string tooltipRow;

        private string headerStyle;
        private string columnHeader;
        private string background;
        private string foreground;
        private string rowHeader;
        private string headerFont;
        private string fontSize;
        private string fontName;
        private string summaryStyle;
        private string columnSummary;
        private string rowSummary;
        private string summaryFont;
        private string cellStyles;
        private string fontStyle;
        private string fontColor;
        private string commonStyles;
        private string common;
        private string gridlineColor;
        private string button_OK;
        private string button_Cancel;
        private string cellFontStyles;

        private string loading;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or Sets the Loading for localization use.
        /// </summary>
        public string Loading
        {
            get { return loading; }
            set { loading = value; }
        }

        /// <summary>
        /// Gets or Sets the HeaderStyle for localization use.
        /// </summary>
        public string HeaderStyle
        {
            get { return headerStyle; }
            set { headerStyle = value; }
        }

        /// <summary>
        /// Gets or Sets the ColumnHeader for localization use.
        /// </summary>
        public string ColumnHeader
        {
            get { return columnHeader; }
            set { columnHeader = value; }
        }

        /// <summary>
        /// Gets or Sets the Background for localization use.
        /// </summary>
        public string Background
        {
            get { return background; }
            set { background = value; }
        }

        /// <summary>
        /// Gets or Sets the Foreground for localization use.
        /// </summary>
        public string Foreground
        {
            get { return foreground; }
            set { foreground = value; }
        }

        /// <summary>
        /// Gets or Sets the RowHeader for localization use.
        /// </summary>
        public string RowHeader
        {
            get { return rowHeader; }
            set { rowHeader = value; }
        }

        /// <summary>
        /// Gets or Sets the HeaderFont for localization use.
        /// </summary>
        public string HeaderFont
        {
            get { return headerFont; }
            set { headerFont = value; }
        }

        /// <summary>
        /// Gets or Sets the FontSize for localization use.
        /// </summary>
        public string FontSize
        {
            get { return fontSize; }
            set { fontSize = value; }
        }

        /// <summary>
        /// Gets or Sets the FontName for localization use.
        /// </summary>
        public string FontName
        {
            get { return fontName; }
            set { fontName = value; }
        }

        /// <summary>
        /// Gets or Sets the SummaryStyle for localization use.
        /// </summary>
        public string SummaryStyle
        {
            get { return summaryStyle; }
            set { summaryStyle = value; }
        }

        /// <summary>
        /// Gets or Sets the ColumnSummary for localization use.
        /// </summary>
        public string ColumnSummary
        {
            get { return columnSummary; }
            set { columnSummary = value; }
        }

        /// <summary>
        /// Gets or Sets the RowSummary for localization use.
        /// </summary>
        public string RowSummary
        {
            get { return rowSummary; }
            set { rowSummary = value; }
        }

        /// <summary>
        /// Gets or Sets the SummaryFont for localization use.
        /// </summary>
        public string SummaryFont
        {
            get { return summaryFont; }
            set { summaryFont = value; }
        }

        /// <summary>
        /// Gets or Sets the CellStyles for localization use.
        /// </summary>
        public string CellStyles
        {
            get { return cellStyles; }
            set { cellStyles = value; }
        }

        /// <summary>
        /// Gets or Sets the FontStyle for localization use.
        /// </summary>
        public string FontStyle
        {
            get { return fontStyle; }
            set { fontStyle = value; }
        }

        /// <summary>
        /// Gets or Sets the FontColor for localization use.
        /// </summary>
        public string FontColor
        {
            get { return fontColor; }
            set { fontColor = value; }
        }

        /// <summary>
        /// Gets or Sets the Common for localization use.
        /// </summary>
        public string Common
        {
            get { return common; }
            set { common = value; }
        }

        /// <summary>
        /// Gets or Sets the CommonStyles for localization use.
        /// </summary>
        public string CommonStyles
        {
            get { return commonStyles; }
            set { commonStyles = value; }
        }


        /// <summary>
        /// Gets or Sets the GridlineColor for localization use.
        /// </summary>
        public string GridlineColor
        {
            get { return gridlineColor; }
            set { gridlineColor = value; }
        }

        /// <summary>
        /// Gets or Sets the Button_OK for localization use.
        /// </summary>
        public string Button_OK
        {
            get { return button_OK; }
            set { button_OK = value; }
        }


        /// <summary>
        /// Gets or Sets the Button_Cancel for localization use.
        /// </summary>
        public string Button_Cancel
        {
            get { return button_Cancel; }
            set { button_Cancel = value; }
        }

        /// <summary>
        /// Gets or Sets the TooltipMeasure for localization use.
        /// </summary>
        public string TooltipMeasure
        {
            get { return tooltipMeasure; }
            set { tooltipMeasure = value; }
        }

        /// <summary>
        /// Gets or Sets the TooltipValue for localization use.
        /// </summary>
        public string TooltipValue
        {
            get { return tooltipValue; }
            set { tooltipValue = value; }
        }

        /// <summary>
        /// Gets or Sets the TooltipColumn for localization use.
        /// </summary>
        public string TooltipColumn
        {
            get { return tooltipColumn; }
            set { tooltipColumn = value; }
        }

        /// <summary>
        /// Gets or Sets the TooltipRow for localization use.
        /// </summary>
        public string TooltipRow
        {
            get { return tooltipRow; }
            set { tooltipRow = value; }
        }

        /// <summary>
        /// Gets or Sets the CellFontStyles for localization use.
        /// </summary>
        public string CellFontStyles
        {
            get { return cellFontStyles; }
            set { cellFontStyles = value; }
        }
        #endregion

    }
}
