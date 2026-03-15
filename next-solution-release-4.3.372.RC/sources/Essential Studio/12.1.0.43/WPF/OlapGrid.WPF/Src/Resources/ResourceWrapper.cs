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

namespace Syncfusion.Windows.Grid.Olap.Resources
{
	public sealed class ResourceWrapper
	{
		#region Constants
		
		const string OLAPGRID_DIALOG_HEADERSTYLE = "OlapGrid_Dialog_HeaderStyle";

		const string OLAPGRID_DIALOG_SUMMARYSTYLE = "OlapGrid_Dialog_SummaryStyle";

		const string OLAPGRID_DIALOG_CELLSTYLES = "OlapGrid_Dialog_CellStyles";

		const string OLAPGRID_DIALOG_COLUMNHEADER = "OlapGrid_Dialog_ColumnHeader";

		const string OLAPGRID_DIALOG_ROWHEADER = "OlapGrid_Dialog_RowHeader";

		const string OLAPGRID_DIALOG_HEADERFONT = "OlapGrid_Dialog_HeaderFont";

		const string OLAPGRID_DIALOG_COLUMNSUMMARY = "OlapGrid_Dialog_ColumnSummary";

		const string OLAPGRID_DIALOG_ROWSUMMARY = "OlapGrid_Dialog_RowSummary";

		const string OLAPGRID_DIALOG_SUMMARYFONT = "OlapGrid_Dialog_SummaryFont";

		const string OLAPGRID_DIALOG_BACKGROUND_COLOR = "OlapGrid_Dialog_Background_Color";

		const string OLAPGRID_DIALOG_FOREGROUND_COLOR = "OlapGrid_Dialog_Foreground_Color";

		const string OLAPGRID_DIALOG_FONT_NAME = "OlapGrid_Dialog_Font_Name";

		const string OLAPGRID_DIALOG_FONT_SIZE = "OlapGrid_Dialog_Font_Size";

		const string OLAPGRID_DIALOG_FONT_STYLE = "OlapGrid_Dialog_Font_Style";

		const string OLAPGRID_DIALOG_FONT_COLOR = "OlapGrid_Dialog_Font_Color";

        const string OLAPGRID_DIALOG_OK = "OlapGrid_Dialog_Ok";

        const string OLAPGRID_DIALOG_CANCEL = "OlapGrid_Dialog_Cancel";

        const string OLAPGRID_DIALOG_TITLE = "OlapGrid_Dialog_Title";
        
        const string OLAPGRID_MEASURE_TOOLTIP = "OlapGrid_Measure_Tooltip";

        const string OLAPGRID_COLUMN_TOOLTIP = "OlapGrid_Column_Tooltip";
		
        const string OLAPGRID_ROW_TOOLTIP = "OlapGrid_Row_Tooltip";
        
        const string OLAPGRID_VALUE_TOOLTIP = "OlapGrid_Value_Tooltip";

        const string OLAPGRID_CONTEXTMENU_EXPAND = "OlapGrid_ContextMenu_Expand";

        const string OLAPGRID_CONTEXTMENU_COLLAPSE = "OlapGrid_ContextMenu_Collapse";

        const string OLAPGRID_CONTEXTMENU_ENTIRELY = "OlapGrid_ContextMenu_Entirely";

        const string OLAPGRID_CONTEXTMENU_TO = "OlapGrid_ContextMenu_To";

		#endregion
		
		#region Members
		
		private string olapGridDialogHeaderStyle;

		private string olapGridDialogSummaryStyle;

		private string olapGridDialogCellStyles;

		private string olapGridDialogColumnHeader;

		private string olapGridDialogRowHeader;

		private string olapGridDialogHeaderFont;

		private string olapGridDialogColumnSummary;

		private string olapGridDialogRowSummary;

		private string olapGridDialogSummaryFont;

		private string olapGridDialogBackgroundColor;

		private string olapGridDialogForegroundColor;

		private string olapGridDialogFontName;

		private string olapGridDialogFontSize;

		private string olapGridDialogFontStyle;

		private string olapGridDialogFontColor;

        private string olapGridDialogOk;

        private string olapGridDialogCancel;

        private string olapGridDialogTitle;

        private string olapGridMeasureTooltip;

        private string olapGridColumnTooltip;
        
        private string olapGridRowTooltip;
        
        private string olapGridValueTooltip;

        private string olapGridContextMenuExpand;

        private string olapGridContextMenuCollapse;

        private string olapGridContextMenuEntirely;

        private string olapGridContextMenuTo;

		#endregion
	
		#region Constructor
		
		public ResourceWrapper()
		{
			CultureInfo ci = CultureInfo.CurrentUICulture;
			
			olapGridDialogHeaderStyle = SR.GetString(ci, OLAPGRID_DIALOG_HEADERSTYLE);

			olapGridDialogSummaryStyle = SR.GetString(ci, OLAPGRID_DIALOG_SUMMARYSTYLE);

			olapGridDialogCellStyles = SR.GetString(ci, OLAPGRID_DIALOG_CELLSTYLES);

			olapGridDialogColumnHeader = SR.GetString(ci, OLAPGRID_DIALOG_COLUMNHEADER);

			olapGridDialogRowHeader = SR.GetString(ci, OLAPGRID_DIALOG_ROWHEADER);

			olapGridDialogHeaderFont = SR.GetString(ci, OLAPGRID_DIALOG_HEADERFONT);

			olapGridDialogColumnSummary = SR.GetString(ci, OLAPGRID_DIALOG_COLUMNSUMMARY);

			olapGridDialogRowSummary = SR.GetString(ci, OLAPGRID_DIALOG_ROWSUMMARY);

			olapGridDialogSummaryFont = SR.GetString(ci, OLAPGRID_DIALOG_SUMMARYFONT);

			olapGridDialogBackgroundColor = SR.GetString(ci, OLAPGRID_DIALOG_BACKGROUND_COLOR);

			olapGridDialogForegroundColor = SR.GetString(ci, OLAPGRID_DIALOG_FOREGROUND_COLOR);

			olapGridDialogFontName = SR.GetString(ci, OLAPGRID_DIALOG_FONT_NAME);

			olapGridDialogFontSize = SR.GetString(ci, OLAPGRID_DIALOG_FONT_SIZE);

			olapGridDialogFontStyle = SR.GetString(ci, OLAPGRID_DIALOG_FONT_STYLE);

			olapGridDialogFontColor = SR.GetString(ci, OLAPGRID_DIALOG_FONT_COLOR);

            olapGridDialogOk = SR.GetString(ci, OLAPGRID_DIALOG_OK);

            olapGridDialogCancel = SR.GetString(ci, OLAPGRID_DIALOG_CANCEL);

            olapGridDialogTitle = SR.GetString(ci, OLAPGRID_DIALOG_TITLE);

            olapGridMeasureTooltip = SR.GetString(ci, OLAPGRID_MEASURE_TOOLTIP);

            olapGridColumnTooltip = SR.GetString(ci, OLAPGRID_COLUMN_TOOLTIP);
            
            olapGridRowTooltip = SR.GetString(ci, OLAPGRID_ROW_TOOLTIP);
            
            olapGridValueTooltip = SR.GetString(ci, OLAPGRID_VALUE_TOOLTIP);

            olapGridContextMenuExpand = SR.GetString(ci, OLAPGRID_CONTEXTMENU_EXPAND);
            
            olapGridContextMenuCollapse = SR.GetString(ci, OLAPGRID_CONTEXTMENU_COLLAPSE);
            
            olapGridContextMenuEntirely = SR.GetString(ci, OLAPGRID_CONTEXTMENU_ENTIRELY);
            
            olapGridContextMenuTo = SR.GetString(ci, OLAPGRID_CONTEXTMENU_TO);
		} 
		
		#endregion
		
		#region Properties
		
		public string OlapGridDialogHeaderStyle
        {
            get { return olapGridDialogHeaderStyle; }
            set { olapGridDialogHeaderStyle = value; }
        }

		public string OlapGridDialogSummaryStyle
        {
            get { return olapGridDialogSummaryStyle; }
            set { olapGridDialogSummaryStyle = value; }
        }

		public string OlapGridDialogCellStyles
        {
            get { return olapGridDialogCellStyles; }
            set { olapGridDialogCellStyles = value; }
        }

		public string OlapGridDialogColumnHeader
        {
            get { return olapGridDialogColumnHeader; }
            set { olapGridDialogColumnHeader = value; }
        }

		public string OlapGridDialogRowHeader
        {
            get { return olapGridDialogRowHeader; }
            set { olapGridDialogRowHeader = value; }
        }

		public string OlapGridDialogHeaderFont
        {
            get { return olapGridDialogHeaderFont; }
            set { olapGridDialogHeaderFont = value; }
        }

		public string OlapGridDialogColumnSummary
        {
            get { return olapGridDialogColumnSummary; }
            set { olapGridDialogColumnSummary = value; }
        }

		public string OlapGridDialogRowSummary
        {
            get { return olapGridDialogRowSummary; }
            set { olapGridDialogRowSummary = value; }
        }

		public string OlapGridDialogSummaryFont
        {
            get { return olapGridDialogSummaryFont; }
            set { olapGridDialogSummaryFont = value; }
        }

		public string OlapGridDialogBackgroundColor
        {
            get { return olapGridDialogBackgroundColor; }
            set { olapGridDialogBackgroundColor = value; }
        }

		public string OlapGridDialogForegroundColor
        {
            get { return olapGridDialogForegroundColor; }
            set { olapGridDialogForegroundColor = value; }
        }

		public string OlapGridDialogFontName
        {
            get { return olapGridDialogFontName; }
            set { olapGridDialogFontName = value; }
        }

		public string OlapGridDialogFontSize
        {
            get { return olapGridDialogFontSize; }
            set { olapGridDialogFontSize = value; }
        }

		public string OlapGridDialogFontStyle
        {
            get { return olapGridDialogFontStyle; }
            set { olapGridDialogFontStyle = value; }
        }

		public string OlapGridDialogFontColor
        {
            get { return olapGridDialogFontColor; }
            set { olapGridDialogFontColor = value; }
        }

        public string OlapGridDialogOk
        {
            get { return olapGridDialogOk; }
            set { olapGridDialogOk = value; }
        }

        public string OlapGridDialogCancel
        {
            get { return olapGridDialogCancel; }
            set { olapGridDialogCancel = value; }
        }
		
        public string OlapGridDialogTitle
        {
            get { return olapGridDialogTitle; }
            set { olapGridDialogTitle = value; }
        }

        public string OlapGridMeasureTooltip
        {
            get { return olapGridMeasureTooltip; }
            set { olapGridMeasureTooltip = value; }
        }

        public string OlapGridColumnTooltip
        {
            get { return olapGridColumnTooltip; }
            set { olapGridColumnTooltip = value; }
        }

        public string OlapGridRowTooltip
        {
            get { return olapGridRowTooltip; }
            set { olapGridRowTooltip = value; }
        }

        public string OlapGridValueTooltip
        {
            get { return olapGridValueTooltip; }
            set { olapGridValueTooltip = value; }
        }

        public string OlapGridContextMenuExpand
        {
            get { return olapGridContextMenuExpand; }
            set { olapGridContextMenuExpand = value; }
        }
        public string OlapGridContextMenuCollapse
        {
            get { return olapGridContextMenuCollapse; }
            set { olapGridContextMenuCollapse = value; }
        }
        public string OlapGridContextMenuEntirely
        {
            get { return olapGridContextMenuEntirely; }
            set { olapGridContextMenuEntirely = value; }
        }
        public string OlapGridContextMenuTo
        {
            get { return olapGridContextMenuTo; }
            set { olapGridContextMenuTo = value; }
        }

		#endregion
	}
}
