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

namespace Syncfusion.Windows.Chart.Olap.Resources
{
	internal sealed class ResourceWrapper
	{
		#region Constants
		
		const string OLAPCHART_DIALOG_CHART_TAB = "OlapChart_Dialog_Chart_Tab";

		const string OLAPCHART_DIALOG_APPEARANCE_TAB = "OlapChart_Dialog_Appearance_Tab";

		const string OLAPCHART_DIALOG_POINT_LABELS_TAB = "OlapChart_Dialog_Point_Labels_Tab";

		const string OLAPCHART_DIALOG_AXIS_TAB = "OlapChart_Dialog_Axis_Tab";

		const string OLAPCHART_DIALOG_CHARTSTYLE_HEADER = "OlapChart_Dialog_ChartStyle_Header";

		const string OLAPCHART_DIALOG_LEGENDS_HEADER = "OlapChart_Dialog_Legends_Header";

		const string OLAPCHART_DIALOG_BORDERSTYLE_HEADER = "OlapChart_Dialog_BorderStyle_Header";

		const string OLAPCHART_DIALOG_BACKGROUNDSTYLE_HEADER = "OlapChart_Dialog_BackgroundStyle_Header";

		const string OLAPCHART_DIALOG_LABELVISIBILITY_HEADER = "OlapChart_Dialog_LabelVisibility_Header";

		const string OLAPCHART_DIALOG_SYMBOLVISIBILITY_HEADER = "OlapChart_Dialog_SymbolVisibility_Header";

		const string OLAPCHART_DIALOG_XAXIS_HEADER = "OlapChart_Dialog_XAxis_Header";

		const string OLAPCHART_DIALOG_YAXIS_HEADER = "OlapChart_Dialog_Yaxis_Header";

		const string OLAPCHART_DIALOG_CHARTTYPE = "OlapChart_Dialog_ChartType";

		const string OLAPCHART_DIALOG_CHARTPALETTE = "OlapChart_Dialog_ChartPalette";

		const string OLAPCHART_DIALOG_LEGENDVISIBILITY = "OlapChart_Dialog_LegendVisibility";

		const string OLAPCHART_DIALOG_LEGENDCHECKBOXVISIBLITY = "OlapChart_Dialog_LegendCheckBoxVisiblity";

		const string OLAPCHART_DIALOG_LEGENDPOSITION = "OlapChart_Dialog_LegendPosition";

		const string OLAPCHART_DIALOG_BORDERWIDTH = "OlapChart_Dialog_BorderWidth";

		const string OLAPCHART_DIALOG_BORDERCOLOR = "OlapChart_Dialog_BorderColor";

		const string OLAPCHART_DIALOG_CHARTBACKGROUND = "OlapChart_Dialog_ChartBackground";

		const string OLAPCHART_DIALOG_CHARTINTERIOR = "OlapChart_Dialog_ChartInterior";

		const string OLAPCHART_DIALOG_XVALUEASLABELCONTENT = "OlapChart_Dialog_XValueAsLabelContent";

		const string OLAPCHART_DIALOG_SERIESNAMEASLABELCONTENT = "OlapChart_Dialog_SeriesNameAsLabelContent";

		const string OLAPCHART_DIALOG_YVALUEASLABELCONTENT = "OlapChart_Dialog_YValueAsLabelContent";

		const string OLAPCHART_DIALOG_CIRCLEASSYMBOL = "OlapChart_Dialog_CircleAsSymbol";

		const string OLAPCHART_DIALOG_TRIANGLEASSYMBOL = "OlapChart_Dialog_TriangleAsSymbol";

		const string OLAPCHART_DIALOG_RECTANGLASSYMBOL = "OlapChart_Dialog_RectanglAsSymbol";

		const string OLAPCHART_DIALOG_AXISLABEL_FONTFACE = "OlapChart_Dialog_AxisLabel_FontFace";

		const string OLAPCHART_DIALOG_AXISLABEL_FONTSTYLE = "OlapChart_Dialog_AxisLabel_FontStyle";

		const string OLAPCHART_DIALOG_AXISLABEL_FONTCOLOR = "OlapChart_Dialog_AxisLabel_FontColor";

		const string OLAPCHART_DIALOG_TITLE = "OlapChart_Dialog_Title";

		const string OLAPCHART_DIALOG_OKBUTTON = "OlapChart_Dialog_OkButton";

        const string OLAPCHART_CONTEXT_MENU = "OlapChart_Context_Menu";

		const string OLAPCHART_DIALOG_CANCLEBUTTON = "OlapChart_Dialog_CancleButton";

		#endregion
		
		#region Members
		
		private string olapChartDialogChartTab;

		private string olapChartDialogAppearanceTab;

		private string olapChartDialogPointLabelsTab;

		private string olapChartDialogAxisTab;

		private string olapChartDialogChartStyleHeader;

		private string olapChartDialogLegendsHeader;

		private string olapChartDialogBorderStyleHeader;

		private string olapChartDialogBackgroundStyleHeader;

		private string olapChartDialogLabelVisibilityHeader;

		private string olapChartDialogSymbolVisibilityHeader;

		private string olapChartDialogXAxisHeader;

		private string olapChartDialogYaxisHeader;

		private string olapChartDialogChartType;

		private string olapChartDialogChartPalette;

		private string olapChartDialogLegendVisibility;

		private string olapChartDialogLegendCheckBoxVisiblity;

		private string olapChartDialogLegendPosition;

		private string olapChartDialogBorderWidth;

		private string olapChartDialogBorderColor;

		private string olapChartDialogChartBackground;

		private string olapChartDialogChartInterior;

		private string olapChartDialogXValueAsLabelContent;

		private string olapChartDialogSeriesNameAsLabelContent;

		private string olapChartDialogYValueAsLabelContent;

		private string olapChartDialogCircleAsSymbol;

		private string olapChartDialogTriangleAsSymbol;

		private string olapChartDialogRectanglAsSymbol;

		private string olapChartDialogAxisLabelFontFace;

		private string olapChartDialogAxisLabelFontStyle;

		private string olapChartDialogAxisLabelFontColor;

		private string olapChartDialogTitle;

		private string olapChartDialogOkButton;

        private string olapChartContextMenu;

		private string olapChartDialogCancleButton;

		#endregion
	
		#region Constructor
		
		public ResourceWrapper()
		{
			CultureInfo ci = CultureInfo.CurrentUICulture;
			
			olapChartDialogChartTab = SR.GetString(ci, OLAPCHART_DIALOG_CHART_TAB);

			olapChartDialogAppearanceTab = SR.GetString(ci, OLAPCHART_DIALOG_APPEARANCE_TAB);

			olapChartDialogPointLabelsTab = SR.GetString(ci, OLAPCHART_DIALOG_POINT_LABELS_TAB);

			olapChartDialogAxisTab = SR.GetString(ci, OLAPCHART_DIALOG_AXIS_TAB);

			olapChartDialogChartStyleHeader = SR.GetString(ci, OLAPCHART_DIALOG_CHARTSTYLE_HEADER);

			olapChartDialogLegendsHeader = SR.GetString(ci, OLAPCHART_DIALOG_LEGENDS_HEADER);

			olapChartDialogBorderStyleHeader = SR.GetString(ci, OLAPCHART_DIALOG_BORDERSTYLE_HEADER);

			olapChartDialogBackgroundStyleHeader = SR.GetString(ci, OLAPCHART_DIALOG_BACKGROUNDSTYLE_HEADER);

			olapChartDialogLabelVisibilityHeader = SR.GetString(ci, OLAPCHART_DIALOG_LABELVISIBILITY_HEADER);

			olapChartDialogSymbolVisibilityHeader = SR.GetString(ci, OLAPCHART_DIALOG_SYMBOLVISIBILITY_HEADER);

			olapChartDialogXAxisHeader = SR.GetString(ci, OLAPCHART_DIALOG_XAXIS_HEADER);

			olapChartDialogYaxisHeader = SR.GetString(ci, OLAPCHART_DIALOG_YAXIS_HEADER);

			olapChartDialogChartType = SR.GetString(ci, OLAPCHART_DIALOG_CHARTTYPE);

			olapChartDialogChartPalette = SR.GetString(ci, OLAPCHART_DIALOG_CHARTPALETTE);

			olapChartDialogLegendVisibility = SR.GetString(ci, OLAPCHART_DIALOG_LEGENDVISIBILITY);

			olapChartDialogLegendCheckBoxVisiblity = SR.GetString(ci, OLAPCHART_DIALOG_LEGENDCHECKBOXVISIBLITY);

			olapChartDialogLegendPosition = SR.GetString(ci, OLAPCHART_DIALOG_LEGENDPOSITION);

			olapChartDialogBorderWidth = SR.GetString(ci, OLAPCHART_DIALOG_BORDERWIDTH);

			olapChartDialogBorderColor = SR.GetString(ci, OLAPCHART_DIALOG_BORDERCOLOR);

			olapChartDialogChartBackground = SR.GetString(ci, OLAPCHART_DIALOG_CHARTBACKGROUND);

			olapChartDialogChartInterior = SR.GetString(ci, OLAPCHART_DIALOG_CHARTINTERIOR);

			olapChartDialogXValueAsLabelContent = SR.GetString(ci, OLAPCHART_DIALOG_XVALUEASLABELCONTENT);

			olapChartDialogSeriesNameAsLabelContent = SR.GetString(ci, OLAPCHART_DIALOG_SERIESNAMEASLABELCONTENT);

			olapChartDialogYValueAsLabelContent = SR.GetString(ci, OLAPCHART_DIALOG_YVALUEASLABELCONTENT);

			olapChartDialogCircleAsSymbol = SR.GetString(ci, OLAPCHART_DIALOG_CIRCLEASSYMBOL);

			olapChartDialogTriangleAsSymbol = SR.GetString(ci, OLAPCHART_DIALOG_TRIANGLEASSYMBOL);

			olapChartDialogRectanglAsSymbol = SR.GetString(ci, OLAPCHART_DIALOG_RECTANGLASSYMBOL);

			olapChartDialogAxisLabelFontFace = SR.GetString(ci, OLAPCHART_DIALOG_AXISLABEL_FONTFACE);

			olapChartDialogAxisLabelFontStyle = SR.GetString(ci, OLAPCHART_DIALOG_AXISLABEL_FONTSTYLE);

			olapChartDialogAxisLabelFontColor = SR.GetString(ci, OLAPCHART_DIALOG_AXISLABEL_FONTCOLOR);

			olapChartDialogTitle = SR.GetString(ci, OLAPCHART_DIALOG_TITLE);

			olapChartDialogOkButton = SR.GetString(ci, OLAPCHART_DIALOG_OKBUTTON);

            olapChartContextMenu = SR.GetString(ci, OLAPCHART_CONTEXT_MENU);

			olapChartDialogCancleButton = SR.GetString(ci, OLAPCHART_DIALOG_CANCLEBUTTON);
		} 
		
		#endregion
		
		#region Properties
		
		public string OlapChartDialogChartTab
		{
			get { return olapChartDialogChartTab; }
			set { olapChartDialogChartTab = value; }
		}

		public string OlapChartDialogAppearanceTab
		{
			get { return olapChartDialogAppearanceTab; }
			set { olapChartDialogAppearanceTab = value; }
		}

		public string OlapChartDialogPointLabelsTab
		{
			get { return olapChartDialogPointLabelsTab; }
			set { olapChartDialogPointLabelsTab = value; }
		}

		public string OlapChartDialogAxisTab
		{
			get { return olapChartDialogAxisTab; }
			set { olapChartDialogAxisTab = value; }
		}

		public string OlapChartDialogChartStyleHeader
		{
			get { return olapChartDialogChartStyleHeader; }
			set { olapChartDialogChartStyleHeader = value; }
		}

		public string OlapChartDialogLegendsHeader
		{
			get { return olapChartDialogLegendsHeader; }
			set { olapChartDialogLegendsHeader = value; }
		}

		public string OlapChartDialogBorderStyleHeader
		{
			get { return olapChartDialogBorderStyleHeader; }
			set { olapChartDialogBorderStyleHeader = value; }
		}

		public string OlapChartDialogBackgroundStyleHeader
		{
			get { return olapChartDialogBackgroundStyleHeader; }
			set { olapChartDialogBackgroundStyleHeader = value; }
		}

		public string OlapChartDialogLabelVisibilityHeader
		{
			get { return olapChartDialogLabelVisibilityHeader; }
			set { olapChartDialogLabelVisibilityHeader = value; }
		}

		public string OlapChartDialogSymbolVisibilityHeader
		{
			get { return olapChartDialogSymbolVisibilityHeader; }
			set { olapChartDialogSymbolVisibilityHeader = value; }
		}

		public string OlapChartDialogXAxisHeader
		{
			get { return olapChartDialogXAxisHeader; }
			set { olapChartDialogXAxisHeader = value; }
		}

		public string OlapChartDialogYaxisHeader
		{
			get { return olapChartDialogYaxisHeader; }
			set { olapChartDialogYaxisHeader = value; }
		}

		public string OlapChartDialogChartType
		{
			get { return olapChartDialogChartType; }
			set { olapChartDialogChartType = value; }
		}

		public string OlapChartDialogChartPalette
		{
			get { return olapChartDialogChartPalette; }
			set { olapChartDialogChartPalette = value; }
		}

		public string OlapChartDialogLegendVisibility
		{
			get { return olapChartDialogLegendVisibility; }
			set { olapChartDialogLegendVisibility = value; }
		}

		public string OlapChartDialogLegendCheckBoxVisiblity
		{
			get { return olapChartDialogLegendCheckBoxVisiblity; }
			set { olapChartDialogLegendCheckBoxVisiblity = value; }
		}

		public string OlapChartDialogLegendPosition
		{
			get { return olapChartDialogLegendPosition; }
			set { olapChartDialogLegendPosition = value; }
		}

		public string OlapChartDialogBorderWidth
		{
			get { return olapChartDialogBorderWidth; }
			set { olapChartDialogBorderWidth = value; }
		}

		public string OlapChartDialogBorderColor
		{
			get { return olapChartDialogBorderColor; }
			set { olapChartDialogBorderColor = value; }
		}

		public string OlapChartDialogChartBackground
		{
			get { return olapChartDialogChartBackground; }
			set { olapChartDialogChartBackground = value; }
		}

		public string OlapChartDialogChartInterior
		{
			get { return olapChartDialogChartInterior; }
			set { olapChartDialogChartInterior = value; }
		}

		public string OlapChartDialogXValueAsLabelContent
		{
			get { return olapChartDialogXValueAsLabelContent; }
			set { olapChartDialogXValueAsLabelContent = value; }
		}

		public string OlapChartDialogSeriesNameAsLabelContent
		{
			get { return olapChartDialogSeriesNameAsLabelContent; }
			set { olapChartDialogSeriesNameAsLabelContent = value; }
		}

		public string OlapChartDialogYValueAsLabelContent
		{
			get { return olapChartDialogYValueAsLabelContent; }
			set { olapChartDialogYValueAsLabelContent = value; }
		}

		public string OlapChartDialogCircleAsSymbol
		{
			get { return olapChartDialogCircleAsSymbol; }
			set { olapChartDialogCircleAsSymbol = value; }
		}

		public string OlapChartDialogTriangleAsSymbol
		{
			get { return olapChartDialogTriangleAsSymbol; }
			set { olapChartDialogTriangleAsSymbol = value; }
		}

		public string OlapChartDialogRectanglAsSymbol
		{
			get { return olapChartDialogRectanglAsSymbol; }
			set { olapChartDialogRectanglAsSymbol = value; }
		}

		public string OlapChartDialogAxisLabelFontFace
		{
			get { return olapChartDialogAxisLabelFontFace; }
			set { olapChartDialogAxisLabelFontFace = value; }
		}

		public string OlapChartDialogAxisLabelFontStyle
		{
			get { return olapChartDialogAxisLabelFontStyle; }
			set { olapChartDialogAxisLabelFontStyle = value; }
		}

		public string OlapChartDialogAxisLabelFontColor
		{
			get { return olapChartDialogAxisLabelFontColor; }
			set { olapChartDialogAxisLabelFontColor = value; }
		}

		public string OlapChartDialogTitle
		{
			get { return olapChartDialogTitle; }
			set { olapChartDialogTitle = value; }
		}

		public string OlapChartDialogOkButton
		{
			get { return olapChartDialogOkButton; }
			set { olapChartDialogOkButton = value; }
		}

        public string OlapChartContextMenu
        {
            get { return olapChartContextMenu; }
            set { olapChartContextMenu = value; }
        }

		public string OlapChartDialogCancleButton
		{
			get { return olapChartDialogCancleButton; }
			set { olapChartDialogCancleButton = value; }
		}

		#endregion
	}
}
