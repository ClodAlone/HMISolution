#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives
using System;
using System.Collections;

using Syncfusion.XlsIO.Implementation.Charts;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing;
using Syncfusion.XlsIO.Parser.Biff_Records.ObjRecords;
using Syncfusion.XlsIO.Implementation.Collections;

using FOPTE = Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing.MsofbtOPT.FOPTE;
using System.Collections.Generic;

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
using System.Text;
using System.IO;
#endif
#endregion

namespace Syncfusion.XlsIO.Implementation.Shapes
{
  /// <summary>
  /// Class used for Chart Shapes.
  /// </summary>
  public class ChartShapeImpl
    : ShapeImpl
    , IChartShape
  {
    #region Class constants
    /// <summary>
    /// Instance value of MsofbtSp record.
    /// </summary>
    private const int DEF_SHAPE_INSTANCE = 201;
    /// <summary>
    /// Version value of MsofbtSp record.
    /// </summary>
    private const int DEF_SHAPE_VERSION = 2;
    /// <summary>
    /// Version value of MsofbtOPT record.
    /// </summary>
    private const int DEF_OPTIONS_VERSION = 3;
    /// <summary>
    /// Instance value of MsofbtOPT record.
    /// </summary>
    private const int DEF_OPTIONS_INSTANCE = 8;
    /// <summary>
    /// Lock against grouping option value.
    /// </summary>
    private const uint DEF_LOCK_GROUPING_VALUE = 17039620;
    /// <summary>
    /// Value of LineColor option.
    /// </summary>
    private const uint DEF_LINECOLOR = 134217805;
    /// <summary>
    /// Value of NoLineDrawDash option.
    /// </summary>
    private const uint DEF_NOLINEDRAWDASH = 524296;
    /// <summary>
    /// Value of ShadowObscured option.
    /// </summary>
    private const uint DEF_SHADOWOBSCURED = 131072;
    /// <summary>
    /// Fore color.
    /// </summary>
    private const uint DEF_FORECOLOR = 134217806;
    /// <summary>
    /// Back color.
    /// </summary>
    private const uint DEF_BACKCOLOR = 134217805;
    #endregion

    #region Class members
    /// <summary>
    /// Chart object.
    /// </summary>
    private ChartImpl m_chart;
    /// <summary>
    /// Zero-based index of top row.
    /// </summary>
    private int m_iTopRow;
    /// <summary>
    /// Zero-based index of bottom row.
    /// </summary>
    private int m_iBottomRow;
    /// <summary>
    /// Zero-based index of left column.
    /// </summary>
    private int m_iLeftColumn;
    /// <summary>
    /// Zero-based index of right column.
    /// </summary>
    private int m_iRightColumn;
    /// <summary>
    /// Parent worksheet.
    /// </summary>
    private WorksheetBaseImpl m_worksheet;
    private int m_offsetX;
    private int m_offsetY;
    private int m_extentsX;
    private int m_extentsY;
    private ChartCategoryCollection m_categories;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes new instance of the shape.
    /// </summary>
    /// <param name="application">Application object for the new shape.</param>
    /// <param name="parent">Parent object for the new shape.</param>
    /// <param name="instance">Cloned object.</param>
    /// <param name="hashNewNames">Dictionary with new names.</param>
    /// <param name="dicFontIndexes">Dictionary with new font indexes.</param>
    public ChartShapeImpl( IApplication application, object parent
      , ChartShapeImpl instance, Dictionary<string, string> hashNewNames, Dictionary<int, int> dicFontIndexes )
      : base( application, parent, instance )
    {
      //WorkbookImpl book = FindParent( parent, typeof( WorkbookImpl ) ) as WorkbookImpl;

      m_chart = instance.m_chart.Clone( hashNewNames, this, dicFontIndexes );

      m_bIsDisposed = instance.m_bIsDisposed;
      m_iBottomRow = instance.m_iBottomRow;
      m_iLeftColumn = instance.m_iLeftColumn;
      m_iRightColumn = instance.m_iRightColumn;
      m_iTopRow = instance.m_iTopRow;
    }

    /// <summary>
    /// Initializes new instance of the chart shape.
    /// </summary>
    /// <param name="application">Application object for the new shape.</param>
    /// <param name="parent">Parent object for the new shape.</param>
    public ChartShapeImpl( IApplication application, object parent )
      : base( application, parent )
    {
      m_chart = new ChartImpl( application, this );
      ShapeType = ExcelShapeType.Chart;

      BottomRow = 20;
      RightColumn = 10;
      m_bSupportOptions = false;
      //SetParents();
    }
    /// <summary>
    /// Initializes new instance of the chart shape.
    /// </summary>
    /// <param name="application">Application object for the new shape.</param>
    /// <param name="parent">Parent object for the new shape.</param>
    /// <param name="container">Shape container record that describes new shape.</param>
    /// <param name="options">Flags to create.</param>
    [ CLSCompliant( false ) ]
    public ChartShapeImpl( IApplication application, object parent, MsofbtSpContainer container
      , ExcelParseOptions options )
      : base( application, parent, container, options )
    {
      //SetParents();
      ShapeType= ExcelShapeType.Chart;
      m_bSupportOptions = false;
    }

    #endregion

    #region Properties
    /// <summary>
    /// Returns internal chart object. Read-only
    /// </summary>
    public ChartImpl ChartObject
    {
      get
      {
        return m_chart;
      }
    }
    /// <summary>
    /// Gets or sets the offset X
    /// </summary>
    internal int OffsetX
    {
        get
        {
            return m_offsetX;
        }
        set
        {
            m_offsetX = value;
        }
    }
    /// <summary>
    /// Gets or sets the offset Y
    /// </summary>
    internal int OffsetY
    {
        get
        {
            return m_offsetY;
        }
        set
        {
            m_offsetY = value;
        }
    }
    /// <summary>
    /// Gets or sets the extents of X
    /// </summary>
    internal int ExtentsX
    {
        get
        {
            return m_extentsX;
        }
        set
        {
            m_extentsX = value;
        }
    }
    /// <summary>
    /// Gets or sets the extents of Y
    /// </summary>
    internal int ExtentsY
    {
        get
        {
            return m_extentsY;
        }
        set
        {
            m_extentsY = value;
        }
    }
    #endregion

    #region IChart Members
    /// <summary>
    /// Returns or sets the rotation of the 3-D chart view
    /// (the rotation of the plot area around the z-axis, in degrees).(0 to 360 degrees).
    /// </summary>
    public int Rotation
    {
      get
      {
        return m_chart.Rotation;
      }
      set
      {
        m_chart.Rotation = value;
      }
    }
    /// <summary>
    /// Represents the series name level
    /// </summary>
    public ExcelSeriesNameLevel SeriesNameLevel
    {
        get
        {
            return m_chart.SeriesNameLevel;
        }
        set
        {
            m_chart.SeriesNameLevel = value;
        }
    }
    /// <summary>
    /// Represents the category name level
    /// </summary>
    public ExcelCategoriesLabelLevel CategoryLabelLevel
    {
        get
        {
            return m_chart.CategoryLabelLevel;
        }
        set
        {
            m_chart.CategoryLabelLevel = value;
        }
    }
    /// <summary>
    /// Represents the category collection
    /// </summary>
      public IChartCategories Categories
    {
        get
        {
            return m_chart.Categories;
        }
    }
    /// <summary>
    /// Returns or sets the elevation of the 3-D chart view, in degrees (�90 to +90 degrees).
    /// </summary>
    public int  Elevation
    {
      get
      {
        return m_chart.Elevation;
      }
      set
      {
        m_chart.Elevation = value;
      }
    }
    /// <summary>
    /// Returns or sets the perspective for the 3-D chart view.( 0 - 100 )
    /// </summary>
    public int Perspective
    {
      get
      {
        return m_chart.Perspective;
      }
      set
      {
        m_chart.Perspective = value;
      }
    }
    /// <summary>
    /// Returns or sets the height of a 3-D chart as a percentage of the chart width
    /// (between 5 and 500 percent).
    /// </summary>
    public int HeightPercent
    {
      get
      {
        return m_chart.HeightPercent;
      }
      set
      {
        m_chart.HeightPercent = value;
      }
    }
    /// <summary>
    /// Returns or sets the depth of a 3-D chart as a percentage of the chart width
    /// (between 20 and 2000 percent).
    /// </summary>
    public int DepthPercent
    {
      get
      {
        return m_chart.DepthPercent;
      }
      set
      {
        m_chart.DepthPercent = value;
      }
    }
    /// <summary>
    /// Returns or sets the distance between the data series in a 3-D chart, as a percentage of the marker width.( 0 - 500 )
    /// </summary>
    public int GapDepth
    {
      get
      {
        return m_chart.GapDepth;
      }
      set
      {
        m_chart.GapDepth = value;
      }
    }
    /// <summary>
    /// True if the chart axes are at right angles, independent of chart rotation or elevation.
    /// </summary>
    public bool   RightAngleAxes
    {
      get
      {
        return m_chart.RightAngleAxes;
      }
      set
      {
        m_chart.RightAngleAxes = value;
      }
    }
    /// <summary>
    /// True if Microsoft Excel scales a 3-D chart so that it's closer in size to the equivalent 2-D chart..
    /// </summary>
    public bool   AutoScaling
    {
      get
      {
        return m_chart.AutoScaling;
      }
      set
      {
        m_chart.AutoScaling = value;
      }
    }
    /// <summary>
    /// True if gridlines are drawn two-dimensionally on a 3-D chart.
    /// </summary>
    public bool   WallsAndGridlines2D
    {
      get
      {
        return m_chart.WallsAndGridlines2D;
      }
      set
      {
        m_chart.WallsAndGridlines2D = value;
      }
    }
    /// <summary>
    /// Returns chart shapes. Read-only.
    /// </summary>
    public IShapes Shapes
    {
      get
      {
        return m_chart.Shapes;
      }
    }
    /// <summary>
        /// Gets or sets the type of the pivot chart.
        /// </summary>
        /// <value>The type of the pivot chart.</value>
        public ExcelChartType PivotChartType
        {
            get
            {
                return m_chart.PivotChartType;
            }
            set
            {
                m_chart.PivotChartType = value;
            }
        }
        /// <summary>
        /// Gets or sets the pivot source.
        /// </summary>
        /// <value>The pivot source.</value>
        public IPivotTable PivotSource
        {
          get
          {
            return m_chart.PivotSource;
          }
          set
          {
            m_chart.PivotSource = value;
          }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show all field buttons].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show all field buttons]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowAllFieldButtons
        {
            get
            {
                return m_chart.ShowAllFieldButtons;
            }
            set
            {
                m_chart.ShowAllFieldButtons = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [show value field buttons].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show value field buttons]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowValueFieldButtons
        {
            get
            {
                return m_chart.ShowValueFieldButtons;
            }
            set
            {

                m_chart.ShowValueFieldButtons = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [show axis field buttons].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show axis field buttons]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowAxisFieldButtons
        {
            get
            {
                return m_chart.ShowAxisFieldButtons;
            }
            set
            {
                m_chart.ShowAxisFieldButtons = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [show legend field buttons].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show legend field buttons]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowLegendFieldButtons
        {
            get
            {
                return m_chart.ShowLegendFieldButtons;
            }
            set
            {
                m_chart.ShowLegendFieldButtons = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [show report filter field buttons].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show report filter field buttons]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowReportFilterFieldButtons
        {
            get
            {
                return m_chart.ShowReportFilterFieldButtons;
            }
            set
            {
                m_chart.ShowReportFilterFieldButtons = value;
            }
        }
        /// <summary>
    /// Type of the chart.
    /// </summary>
    public Syncfusion.XlsIO.ExcelChartType ChartType
    {
      get
      {
        return m_chart.ChartType;
      }
      set
      {
        m_chart.ChartType = value;
      }
    }

    /// <summary>
    /// DataRange for the chart series.
    /// </summary>
    public IRange DataRange
    {
      get
      {
        return m_chart.DataRange;
      }
      set
      {
        m_chart.DataRange = value;
      }
    }

    /// <summary>
    /// True if series are in rows in DataRange;
    /// otherwise False.
    /// </summary>
    public bool IsSeriesInRows
    {
      get
      {
        return m_chart.IsSeriesInRows;
      }
      set
      {
        m_chart.IsSeriesInRows = value;
      }
    }

    /// <summary>
    /// Title of the chart.
    /// </summary>
    public string ChartTitle
    {
      get
      {
        return m_chart.ChartTitle;
      }
      set
      {
        m_chart.ChartTitle = value;
      }
    }

    /// <summary>
    /// Gets title text area. Read-only.
    /// </summary>
    public IChartTextArea ChartTitleArea
    {
      get
      {
        return m_chart.ChartTitleArea;
      }
    }
    /// <summary>
    /// Title of the category axis.
    /// </summary>
    public string CategoryAxisTitle
    {
      get
      {
        return m_chart.CategoryAxisTitle;
      }
      set
      {
        m_chart.CategoryAxisTitle = value;
      }
    }

    /// <summary>
    /// Title of the value axis.
    /// </summary>
    public string ValueAxisTitle
    {
      get
      {
        return m_chart.ValueAxisTitle;
      }
      set
      {
        m_chart.ValueAxisTitle = value;
      }
    }

    /// <summary>
    /// Title of the secondary category axis.
    /// </summary>
    public string SecondaryCategoryAxisTitle
    {
      get
      {
        return m_chart.SecondaryCategoryAxisTitle;
      }
      set
      {
        m_chart.SecondaryCategoryAxisTitle = value;
      }
    }

    /// <summary>
    /// Title of the secondary value axis.
    /// </summary>
    public string SecondaryValueAxisTitle
    {
      get
      {
        return m_chart.SecondaryValueAxisTitle;
      }
      set
      {
        m_chart.SecondaryValueAxisTitle = value;
      }
    }

    /// <summary>
    /// Title of the series axis.
    /// </summary>
    public string SeriesAxisTitle
    {
      get
      {
        return m_chart.SeriesAxisTitle;
      }
      set
      {
        m_chart.SeriesAxisTitle = value;
      }
    }
    /// <summary>
    /// Page setup for the chart. Read-only.
    /// </summary>
    public IChartPageSetup PageSetup
    {
      get
      {
        return m_chart.PageSetup;
      }
    }

    /// <summary>
    /// X coordinate of the upper-left corner
    /// of the chart in points (1/72 inch).
    /// </summary>
    public double XPos
    {
      get
      {
        return m_chart.XPos;
      }
      set
      {
        m_chart.XPos = value;
      }
    }

    /// <summary>
    /// Y coordinate of the upper-left corner
    /// of the chart in points (1/72 inch).
    /// </summary>
    public double YPos
    {
      get
      {
        return m_chart.YPos;
      }
      set
      {
        m_chart.YPos = value;
      }
    }

    /// <summary>
    /// Width of the chart in points (1/72 inch).
    /// </summary>
    double Syncfusion.XlsIO.IChart.Width
    {
      get
      {
        return m_chart.Width;
      }
      set
      {
        m_chart.Width = value;
      }
    }

    /// <summary>
    /// Height of the chart in points (1/72 inch).
    /// </summary>
    double Syncfusion.XlsIO.IChart.Height
    {
      get
      {
        return m_chart.Height;
      }
      set
      {
        m_chart.Height = value;
      }
    }

    /// <summary>
    /// Collection of the all series of this chart. Read-only.
    /// </summary>
    public IChartSeries Series
    {
      get
      {
        return m_chart.Series;
      }
    }

    /// <summary>
    /// Returns primary category axis. Read-only.
    /// </summary>
    public IChartCategoryAxis PrimaryCategoryAxis
    {
      get
      {
        return m_chart.PrimaryCategoryAxis;
      }
    }
    /// <summary>
    /// Returns primary value axis. Read-only.
    /// </summary>
    public IChartValueAxis PrimaryValueAxis
    {
      get
      {
        return m_chart.PrimaryValueAxis;
      }
    }
    /// <summary>
    /// Returns primary series axis. Read-only.
    /// </summary>
    public IChartSeriesAxis PrimarySerieAxis
    {
      get
      {
        return m_chart.PrimarySerieAxis;
      }
    }
    /// <summary>
    /// Returns secondary category axis. Read-only.
    /// </summary>
    public IChartCategoryAxis SecondaryCategoryAxis
    {
      get
      {
        return m_chart.SecondaryCategoryAxis;
      }
    }
    /// <summary>
    /// Returns secondary value axis. Read-only.
    /// </summary>
    public IChartValueAxis SecondaryValueAxis
    {
      get
      {
        return m_chart.SecondaryValueAxis;
      }
    }
    /// <summary>
    /// Returns an object that represents the complete chart area for the chart. Read-only.
    /// </summary>
    public IChartFrameFormat ChartArea
    {
      get
      {
        return m_chart.ChartArea;
      }
    }
    /// <summary>
    /// Returns plot area frame format. Read-only.
    /// </summary>
    public IChartFrameFormat PlotArea
    {
      get
      {
        return m_chart.PlotArea;
      }
    }
    /// <summary>
    /// Returns chart format collection in primary axis.
    /// </summary>
    public ChartFormatCollection PrimaryFormats
    {
      get
      {
        return m_chart.PrimaryFormats;
      }
    }
    /// <summary>
    /// Returns chart format collection in secondary axis.
    /// </summary>
    public ChartFormatCollection SecondaryFormats
    {
      get
      {
        return m_chart.SecondaryFormats;
      }
    }
    /// <summary>
    /// Returns picture collection. Valid only for charts with own tab.
    /// </summary>
    public IPictures Pictures
    {
      get
      {
        throw new NotSupportedException();
        //return m_chart.Pictures;
      }
    }
    /// <summary>
    /// Returns charts collection. Valid only for charts with own tab.
    /// </summary>
    public IChartShapes Charts
    {
      get
      {
        throw new NotSupportedException();
        //return m_chart.Charts;
      }
    }
    /// <summary>
    /// Gets / sets tab color.
    /// </summary>
    public ExcelKnownColors TabColor
    {
      get
      {
        throw new NotSupportedException();
      }
      set
      {
        throw new NotSupportedException();
      }
    }
    /// <summary>
    /// Indicates whether worksheet is displayed right to left.
    /// </summary>
    public bool IsRightToLeft
    {
      get
      {
        return m_chart.IsRightToLeft;
      }
      set
      {
        m_chart.IsRightToLeft = value;
      }
    }

    /// <summary>
    /// Gets / sets tab color.
    /// </summary>
    public Color TabColorRGB
    {
      get
      {
        throw new NotSupportedException();
      }
      set
      {
        throw new NotSupportedException();
      }
    }   

    /// <summary>
    /// Represents chart walls. Read-only.
    /// </summary>
    public IChartWallOrFloor Walls
    {
      get
      {
        return m_chart.Walls;
      }
    }
    /// <summary>
    /// sidewall property..
    /// </summary>
    public IChartWallOrFloor SideWall
    {
        get
        {
            return m_chart.SideWall;
        }
    }
    /// <summary>
    /// backwall property..
    /// </summary>
    public IChartWallOrFloor BackWall
    {
        get
        {
            return m_chart.Walls;
        }
    }
    /// <summary>
    /// Represents chart floor. Read-only.
    /// </summary>
    public IChartWallOrFloor Floor
    {
      get
      {
        return m_chart.Floor;
      }
    }
    /// <summary>
    /// Represents charts dataTable object.
    /// </summary>
    public IChartDataTable DataTable
    {
      get
      {
        return m_chart.DataTable;
      }
    }
    /// <summary>
    /// Indicates whether tab of this sheet is selected. Read-only.
    /// </summary>
    public bool IsSelected
    {
        get
        {
          throw new NotSupportedException();
        }
    }
    /// <summary>
    /// True if the chart has a data table.
    /// </summary>
    public bool HasDataTable
    {
      get
      {
        return m_chart.HasDataTable;
      }
      set
      {
        m_chart.HasDataTable = value;
      }
    }
    /// <summary>
    /// True if the chart has a legend object.
    /// </summary>
    public bool HasLegend
    {
      get
      {
        return m_chart.HasLegend;
      }
      set
      {
        m_chart.HasLegend = value;
      }
    }
    /// <summary>
    /// Represents chart legend.
    /// </summary>
    public IChartLegend Legend
    {
      get
      {
        return m_chart.Legend;
      }
    }
    /// <summary>
    /// Indicates whether chart has plot area.
    /// </summary>
    public bool HasPlotArea
    {
      get
      {
        return m_chart.HasPlotArea;
      }
      set
      {
        m_chart.HasPlotArea = value;
      }
    }
    /// <summary>
    /// Returns index in the parent ITabSheets collection. Read-only.
    /// </summary>
    public int TabIndex
    {
      get
      {
        throw new NotSupportedException( "This property is not supported for embedded charts." );
      }
    }
    /// <summary>
    /// Control visibility of worksheet to end user.
    /// </summary>
    public WorksheetVisibility Visibility
    {
      get
      {
        throw new NotSupportedException();
      }
      set
      {
        throw new NotSupportedException();
      }
    }
    /// <summary>
    /// Makes the current sheet the active sheet. Equivalent to clicking the
    /// sheet's tab.
    /// </summary>
    public void Activate()
    {
      throw new NotSupportedException();
    }
    /// <summary>
    /// Selects current tab sheet.
    /// </summary>
    public void Select()
    {
      throw new NotSupportedException();
    }
    /// <summary>
    /// Unselects current tab sheet.
    /// </summary>
    public void Unselect()
    {
      throw new NotSupportedException();
    }
    /// <summary>
    /// Represents the way that blank cells are plotted on a chart.
    /// </summary>
    public ExcelChartPlotEmpty DisplayBlanksAs
    {
      get
      {
        return m_chart.DisplayBlanksAs;
      }
      set
      {
        m_chart.DisplayBlanksAs = value;
      }
    }
    /// <summary>
    /// True if only visible cells are plotted. False if both visible and hidden cells are plotted.
    /// </summary>
    public bool PlotVisibleOnly
    {
      get
      {
        return m_chart.PlotVisibleOnly;
      }
      set
      {
        m_chart.PlotVisibleOnly = value;
      }
    }
    /// <summary>
    /// True if Microsoft Excel resizes the chart to match the size of the chart sheet window.
    /// False if the chart size isn't attached to the window size. Applies only to chart sheets.
    /// </summary>
    public bool SizeWithWindow
    {
      get
      {
        return m_chart.SizeWithWindow;
      }
      set
      {
        m_chart.SizeWithWindow = value;
      }
    }
    /// <summary>
    /// Returns collection with all textboxes inside this worksheet. Read-only.
    /// </summary>
    public ITextBoxes TextBoxes
    {
      get
      {
        return m_chart.TextBoxes;
      }
    }
    /// <summary>
    /// Returns collection with all checkboxes inside this worksheet. Read-only.
    /// </summary>
    public ICheckBoxes CheckBoxes
    {
      get
      {
        return m_chart.CheckBoxes;
      }
    }
    /// <summary>
    /// Returns collection with all option buttons inside this worksheet. Read-only.
    /// </summary>
    public IOptionButtons OptionButtons
    {
        get
        {
            return m_chart.OptionButtons;
      }
    }
    /// <summary>
    /// Returns collection with all comboboxes inside this worksheet. Read-only.
    /// </summary>
    public IComboBoxes ComboBoxes
    {
      get
      {
        return m_chart.ComboBoxes;
      }
    }
    /// <summary>
    /// Gets code name of the chart.
    /// </summary>
    public string CodeName
    {
      get
      {
        return m_chart.CodeName;
      }
    }
    /// <summary>
    /// Indicates is current sheet is protected.
    /// </summary>
    public bool ProtectContents
    {
      get
      {
        throw new NotSupportedException();
      }
    }
    /// <summary>
    /// True if objects are protected. Read-only.
    /// </summary>
    public bool ProtectDrawingObjects
    {
      get
      {
        throw new NotSupportedException();
      }
    }
    /// <summary>
    /// True if the scenarios of the current sheet are protected. Read-only.
    /// </summary>
    public bool ProtectScenarios
    {
      get
      {
        throw new NotSupportedException();
      }
    }
    /// <summary>
    /// Gets protected options. Read-only. For sets protection options use "Protect" method.
    /// </summary>
    public ExcelSheetProtection Protection
    {
      get
      {
        throw new NotSupportedException();
      }
    }
    /// <summary>
    /// Indicates if the worksheet is password protected.
    /// </summary>
    public bool IsPasswordProtected
    {
      get
      {
        throw new NotSupportedException();
      }
    }
    /// <summary>
    /// Protects worksheet's content with password.
    /// </summary>
    /// <param name="password">Password to protect with.</param>
    public void Protect( string password )
    {
      throw new NotSupportedException();
    }
    /// <summary>
    /// Protects current worksheet.
    /// </summary>
    /// <param name="password">Represents password to protect.</param>
    /// <param name="options">Represents params to protect.</param>
    public void Protect( string password, ExcelSheetProtection options )
    {
      throw new NotSupportedException();
    }
    /// <summary>
    /// Unprotects worksheet's content with password.
    /// </summary>
    /// <param name="password">Password to unprotect.</param>
    public void Unprotect( string password )
    {
      throw new NotSupportedException();
    }
#if ((SyncfusionFramework4_0 || SyncfusionFramework4_5) && !SILVERLIGHT && !WINRT && !WP)
    /// <summary>
    /// Method saves the chart as image.
    /// </summary>
    /// <param name="imageAsStream">stream in where the image is streamed.</param>
    public void SaveAsImage(Stream imageAsStream)
    {
        IChartToImageConverter chartConverter = Application.ChartToImageConverter;
        if (chartConverter == null)
            throw new ArgumentException("IApplication.ChartToImageConverter must be instantiated");
        chartConverter.SaveAsImage(this, imageAsStream);
    }
#endif
    #endregion

    #region Class overrides
    /// <summary>
    /// Creates a clone of the current shape.
    /// </summary>
    /// <param name="parent">New parent for the shape object.</param>
    /// <param name="hashNewNames">Dictionary with new worksheet names.</param>
    /// <param name="dicFontIndexes">Dictionary with new font indexes.</param>
    /// <param name="addToCollections">Indicates whether we should add created
    /// shape into all necessary parent collections.</param>
    /// <returns>A copy of the current shape.</returns>
    public override IShape Clone( object parent, Dictionary<string, string> hashNewNames,
      Dictionary<int, int> dicFontIndexes, bool addToCollections )
    {
      ChartShapeImpl result = new ChartShapeImpl( Application, parent
        , this, hashNewNames, dicFontIndexes );

      WorksheetBaseImpl sheet = FindParent( result.Parent,
        typeof( WorksheetBaseImpl ), true ) as WorksheetBaseImpl;

      //sheet.InnerCharts.AddChart( result );

      if( addToCollections )
        sheet.InnerShapes.AddShape( result );

      return result;
    }
    /// <summary>
    /// Updates formulas after copy operation.
    /// </summary>
    /// <param name="iCurIndex">Current worksheet index.</param>
    /// <param name="iSourceIndex">Source worksheet index.</param>
    /// <param name="sourceRect">Source rectangle.</param>
    /// <param name="iDestIndex">Destination worksheet index.</param>
    /// <param name="destRect">Destination rectangle.</param>
    public override void UpdateFormula( int iCurIndex, int iSourceIndex,
      Rectangle sourceRect, int iDestIndex, Rectangle destRect )
    {
      m_chart.UpdateFormula( iCurIndex, iSourceIndex, sourceRect, iDestIndex, destRect );
    }
    /// <summary>
    /// Registers shape in all required sub collections.
    /// </summary>
    public override void RegisterInSubCollection()
    {
      m_shapes.WorksheetBase.InnerCharts.InnerAddChart( this );
    }
    /// <summary>
    /// This method is called inside of PrepareForSerialization to make shape-dependent preparations.
    /// </summary>
    protected override void OnPrepareForSerialization()
    {
      if( m_shape == null )
        m_shape = ( MsofbtSp )MsoFactory.GetRecord( MsoRecords.msofbtSp );

      m_shape.Version = DEF_SHAPE_VERSION;
      m_shape.Instance = DEF_SHAPE_INSTANCE;
      m_shape.IsHaveAnchor = true;
      m_shape.IsHaveSpt = true;
    }
    /// <summary>
    /// Parses client data record.
    /// </summary>
    /// <param name="clientData">Record to parse.</param>
    /// <param name="options">Parse options.</param>
    [ CLSCompliant( false ) ]
    protected override void ParseClientData( MsofbtClientData clientData, ExcelParseOptions options )
    {
      base.ParseClientData( clientData, options );

      int iPos = 1;
      BiffRecordRaw[] arrData = clientData.AdditionalData;

      m_chart = new ChartImpl( Application, this, arrData, ref iPos, options );

      if( ( options & ExcelParseOptions.DoNotParseCharts ) == 0 )
      {
        m_chart.Parse();
      }
    }
    /// <summary>
    /// Parses shape group container.
    /// </summary>
    /// <param name="spgrContainer">Group container.</param>
    [ CLSCompliant( false ) ]
    protected override void SerializeShape( MsofbtSpgrContainer spgrContainer )
    {
      if( spgrContainer == null )
        throw new ArgumentNullException( "spgrContainer" );

      MsofbtSpContainer spContainer = ( MsofbtSpContainer )MsoFactory.GetRecord(
          MsoRecords.msofbtSpContainer );

      MsofbtClientAnchor clientAnchor = ( MsofbtClientAnchor )MsoFactory.GetRecord(
        MsoRecords.msofbtClientAnchor );

      MsofbtClientData clientData = ( MsofbtClientData )MsoFactory.GetRecord(
        MsoRecords.msofbtClientData );

      OffsetArrayList records = new OffsetArrayList();
      ftCmo cmo = null;

      if( Obj == null )
      {
        OBJRecord obj = ( OBJRecord )BiffRecordFactory.GetRecord( TBIFFRecord.OBJ );
            
        cmo = new ftCmo();
      
        cmo.ObjectType = TObjType.otChart;
        cmo.Printable = true;
      
        //      m_book.CurrentObjectId++;
        //cmo.ID = ( ushort )Workbook.CurrentObjectId;
        ftEnd end = new ftEnd();
      
        obj.AddSubRecord( cmo );
        obj.AddSubRecord( end );
        SetObject( obj );
      }
      else
      {
        cmo = Obj.RecordsList[ 0 ] as ftCmo;
      }

      cmo.ID = ( OldObjId > 0 ) ? ( ushort )OldObjId : ( ushort )ParentWorkbook.CurrentObjectId;
      clientData.AddRecord( Obj );

      m_chart.EMUWidth = ApplicationImpl.ConvertFromPixel( ( this as IShape ).Width, MeasureUnits.Point );
      m_chart.EMUHeight = ApplicationImpl.ConvertFromPixel( ( this as IShape ).Height, MeasureUnits.Point );
      m_chart.Serialize( records );
      clientData.AddRecordRange( records );

      if( ClientAnchor == null )
      {
        clientAnchor.Options = 3;
        clientAnchor.LeftColumn   = m_iLeftColumn;
        clientAnchor.RightColumn  = m_iRightColumn;
        clientAnchor.TopRow       = m_iTopRow;
        clientAnchor.BottomRow    = m_iBottomRow;
        clientAnchor.LeftOffset   = 0;
        clientAnchor.RightOffset  = 0;
        clientAnchor.TopOffset    = 0;
        clientAnchor.BottomOffset = 0;
      }
      else
      {
        clientAnchor = ClientAnchor;
      }
      
      spContainer.AddItem( m_shape );
      
      MsofbtOPT options = SerializeOptions( spContainer );
      options.Version = DEF_OPTIONS_VERSION;
      options.Instance = DEF_OPTIONS_INSTANCE;

      if( options.Properties.Length > 0 ) spContainer.AddItem( options );
      
      spContainer.AddItem( clientAnchor );
      spContainer.AddItem( clientData );
      
      spgrContainer.AddItem( spContainer );
      //spgrContainer.AddItem( m_record );
      //throw new NotImplementedException();
    }
    /// <summary>
    /// Parses client anchor record.
    /// </summary>
    /// <param name="clientAnchor">Record to parse.</param>
    [ CLSCompliant( false ) ]
    public override void ParseClientAnchor(MsofbtClientAnchor clientAnchor)
    {
      base.ParseClientAnchor( clientAnchor );

      m_iBottomRow    = clientAnchor.BottomRow;
      m_iTopRow       = clientAnchor.TopRow;
      m_iLeftColumn   = clientAnchor.LeftColumn;
      m_iRightColumn  = clientAnchor.RightColumn;
    }
    /// <summary>
    /// Serializes options.
    /// </summary>
    /// <param name="parent">Parent record for options.</param>
    /// <returns>Create options record.</returns>
    [ CLSCompliant( false ) ]
    protected override MsofbtOPT SerializeOptions(MsoBase parent)
    {
      if( m_bUpdateLineFill || m_options == null )
      {
        MsofbtOPT result = base.SerializeOptions( parent );

        //      SerializeTextDirection( result );
        SerializeSizeTextToFit( result );
        //      SerializeOption344( result );

        SerializeOptionSorted( result, MsoOptions.ForeColor, DEF_FORECOLOR );
        SerializeOptionSorted( result, MsoOptions.BackColor, DEF_BACKCOLOR );
        SerializeHitTest( result );
        SerializeOptionSorted( result, MsoOptions.LineColor, DEF_LINECOLOR );
        SerializeOptionSorted( result, MsoOptions.NoLineDrawDash, DEF_NOLINEDRAWDASH );
        SerializeOptionSorted( result, MsoOptions.ShadowObscured, DEF_SHADOWOBSCURED );
        //      SerializeForeShadowColor( result );
        //      SerializeOption575( result );
        SerializeShapeName( result );
        //      SerializeOption959( result );

        return result;
      }

      return m_options;
    }
    /// <summary>
    /// Creates default options.
    /// </summary>
    /// <returns>Created MsofbtOPT record.</returns>
    [ CLSCompliant( false ) ]
    protected override MsofbtOPT CreateDefaultOptions()
    {
      MsofbtOPT result = base.CreateDefaultOptions ();
      result.Version = DEF_OPTIONS_VERSION;
      result.Instance = DEF_OPTIONS_INSTANCE;
      //SerializeLockGrouping( result );
      SerializeOption( result, MsoOptions.LockAgainstGrouping, DEF_LOCK_GROUPING_VALUE );

      return result;
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Searches for all parents
    /// </summary>
    protected override void SetParents()
    {
      base.SetParents();

      m_worksheet = m_shapes.WorksheetBase;

      m_worksheet.InnerCharts.InnerAddChart( this );
    }
    /// <summary>
    /// Converts chart shape into WorksheetBaseImpl.
    /// </summary>
    /// <param name="chartShape">Shape to convert.</param>
    /// <returns>Converted object.</returns>
    public static implicit operator WorksheetBaseImpl( ChartShapeImpl chartShape )
    {
      return chartShape.ChartObject;
    }
    #endregion
  }
}
