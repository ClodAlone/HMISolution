#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#region File Using Directives

using System;
using System.Collections.Generic;

#if ( WINRT )
using Windows.UI;
using Syncfusion.XlsIO.Implementation.WINRT;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.Silverlight;
#elif WP
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.WP;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif

#endregion

namespace Syncfusion.XlsIO.Implementation
{
    /// <summary>
    /// Represents a SparklineGroup.The SparklineGroup object is a member of the 
    /// SparklineGroups collection.The SparklineGroups collection contains all 
    /// the SparklineGroup objects in a workheet.
    /// </summary>
    public class SparklineGroup :
      List<ISparklines>,
      ISparklineGroup
    {

        #region Fields

        private bool m_displayAxis = false;
        private bool m_displayHiddenRC = false;
        private bool m_plotRightToLeft = false;
        private bool m_showFirstPoint = false;
        private bool m_showLastPoint = false;
        private bool m_showLowPoint = false;
        private bool m_showHighPoint = false;
        private bool m_showNegativePoint = false;
        private bool m_showMarkers = false;
        private bool m_horizontalDateAxis = false;
        private Color m_axisColor = ColorExtension.Black;
        private Color m_firstPointColor = ColorExtension.Black;
        private Color m_highPointColor = ColorExtension.Black;
        private Color m_lastPointColor = ColorExtension.Black;
        private double m_lineWeight = 0.75;
        private Color m_lowPointColor = ColorExtension.Black;
        private Color m_markersColor = ColorExtension.Black;
        private Color m_negativePointColor = ColorExtension.Black;
        private Color m_sparklineColor = ColorExtension.Black;        
        private ISparklineVerticalAxis m_verticalMaximum = null;
        private ISparklineVerticalAxis m_verticalMinimum = null;
        private SparklineType m_sparklineType = SparklineType.Line;
        private SparklineEmptyCells m_displayEmptyCellsAs = SparklineEmptyCells.Zero;
        private IRange m_horizontalDateAxisRange;
        private WorkbookImpl m_book;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether to show the sparkline horizontal axis. The horizontal axis appears if the sparkline has data that crosses the zero axis.
        /// </summary>
        /// <value><c>true</c> if [display axis]; otherwise, <c>false</c>.</value>
        public bool DisplayAxis
        {
            get
            {
                return m_displayAxis;
            }
            set
            {
                m_displayAxis = value;
            }
        }
        /// <summary>
        /// Indicates whether to show data in hidden rows and columns.
        /// </summary>
        /// <value><c>true</c> if [display hidden RC]; otherwise, <c>false</c>.</value>
        public bool DisplayHiddenRC
        {
            get
            {
                return m_displayHiddenRC;
            }
            set
            {
                m_displayHiddenRC = value;
            }
        }
        /// <summary>
        /// Indicates whether the plot data is right to left. 
        /// </summary>
        /// <value><c>true</c> if [plot right to left]; otherwise, <c>false</c>.</value>
        public bool PlotRightToLeft
        {
            get
            {
                return m_plotRightToLeft;
            }
            set
            {
                m_plotRightToLeft = value;
            }
        }
        /// <summary>
        /// Indicates whether to highlight the first point of data in the sparkline group. 
        /// </summary>
        /// <value><c>true</c> if [show first point]; otherwise, <c>false</c>.</value>
        public bool ShowFirstPoint
        {
            get
            {
                return m_showFirstPoint;
            }
            set
            {
                m_showFirstPoint = value;
            }
        }
        /// <summary>
        /// Indicates whether to highlight the last point of data in the sparkline group. 
        /// </summary>
        /// <value><c>true</c> if [show last point]; otherwise, <c>false</c>.</value>
        public bool ShowLastPoint
        {
            get
            {
                return m_showLastPoint;
            }
            set
            {
                m_showLastPoint = value;
            }
        }
        /// <summary>
        /// Indicates whether to highlight the lowest points of data in the sparkline group.
        /// </summary>
        /// <value><c>true</c> if [show low point]; otherwise, <c>false</c>.</value>
        public bool ShowLowPoint
        {
            get
            {
                return m_showLowPoint;
            }
            set
            {
                m_showLowPoint = value;
            }
        }
        /// <summary>
        ///Indicates whether to highlight the highest points of data in the sparkline group. 
        /// </summary>
        /// <value><c>true</c> if [show high point]; otherwise, <c>false</c>.</value>
        public bool ShowHighPoint
        {
            get
            {
                return m_showHighPoint;
            }
            set
            {
                m_showHighPoint = value;
            }
        }
        /// <summary>
        /// Indicates whether to highlight the negative values on the sparkline group with a different color or marker.
        /// </summary>
        /// <value><c>true</c> if [show negative point]; otherwise, <c>false</c>.</value>
        public bool ShowNegativePoint
        {
            get
            {
                return m_showNegativePoint;
            }
            set
            {
                m_showNegativePoint = value;
            }
        }
        /// <summary>
        /// Indicates whether to highlight each point in each line sparkline in the sparkline group.  
        /// </summary>
        /// <value><c>true</c> if [show markers]; otherwise, <c>false</c>.</value>
        /// <exception cref=" NotSupportedException">If Sparklinetype is not equal to Line</exception>
        public bool ShowMarkers
        {
            get
            {
                return m_showMarkers;
            }
            set
            {
                if (m_sparklineType != SparklineType.Line && !m_book.Loading)
                    throw new NotSupportedException("It is not supported for this Sparkline type.");

                m_showMarkers = value;
            }
        }
        /// <summary>
        /// The VerticalAxisMaximum property represents the Vertical Axis maximum options.
        /// </summary>
        /// <value>The VerticalAxisMaximum property gets/sets the m_verticalMaximum member.</value>
        public ISparklineVerticalAxis VerticalAxisMaximum
        {
            get
            {
                if (m_verticalMaximum == null)
                    m_verticalMaximum = new SparklineVerticalAxis();

                return m_verticalMaximum;
            }
            set
            {
                if (value != null)
                    m_verticalMaximum = value;
            }
        }
        /// <summary>
        /// The VerticalAxisMinimum property represents the Vertical Axis minimum options.
        /// </summary>
        /// <value>The VerticalAxisMinimum property gets/sets the m_verticalMinimum member.</value>
        public ISparklineVerticalAxis VerticalAxisMinimum
        {
            get
            {
                if (m_verticalMinimum == null)
                    m_verticalMinimum = new SparklineVerticalAxis();

                return m_verticalMinimum;
            }
            set
            {
                if (value != null)
                    m_verticalMinimum = value;
            }
        }
        /// <summary>
        /// Indicates the sparkline type of the sparkline group.
        /// </summary>
        /// <value>The SparklineType property gets/sets the m_sparklineType member.</value>
        public SparklineType SparklineType
        {
            get
            {
                return m_sparklineType;
            }
            set
            {
                m_sparklineType = value;
            }
        }

        /// <summary>
        /// The HorizontalDateAxis property represents the horizontal axis type as Dateaxis.
        /// </summary>
        /// <value><c>true</c> if [horizontal date axis]; otherwise, <c>false</c>.</value>
        public bool HorizontalDateAxis
        {
            get
            {
                return m_horizontalDateAxis;
            }
            set
            {
                m_horizontalDateAxis = value;
            }
        }
        /// <summary>
        /// Indicates how to display empty cells.
        /// </summary>
        /// <value>The DisplayEmptyCellsAs property gets/sets the m_displayEmptyCellsAs data member.</value>
        public SparklineEmptyCells DisplayEmptyCellsAs
        {
            get
            {
                return m_displayEmptyCellsAs;
            }
            set
            {
                m_displayEmptyCellsAs = value;
            }
        }
        /// <summary>
        /// Represents the range that contains the date values for the sparkline data.
        /// </summary>
        /// <value>The HorizontalDateAxisRange property gets/sets the m_horizontalDateAxisRange data member.</value>
        ///<exception cref=" ArgumentOutOfRangeException">
        ///if<paramref name="Value.Rows.Length"/>is not equal to 1
        ///if<paramref name="value.Column.Length"/>is not equal to 1
        ///if<paramref name="HorizontalDateAxis"/>is not true
        ///</exception>
        public IRange HorizontalDateAxisRange
        {
            get
            {
                return m_horizontalDateAxisRange;
            }
            set
            {
                if (!((HorizontalDateAxis) && ((value.Rows.Length == 1) || (value.Columns.Length == 1))))
                    throw new ArgumentOutOfRangeException("DataRange", "Date axis reference is not valid because the cells are not in the same column or row.");

                m_horizontalDateAxisRange = value;
            }
        }
        /// <summary>
        /// Gets and sets the color of the horizontal axis in the sparkline group.
        /// </summary>
        /// <value>The color of the axis.</value>
        public Color AxisColor
        {
            get
            {
                return m_axisColor;
            }
            set
            {
              if( !m_book.Loading )
                this.DisplayAxis = true;

                m_axisColor = value;
            }
        }
        /// <summary>
        /// Gets and sets the color of the first point of data in the sparkline group. 
        /// </summary>
        /// <value>The first color of the point.</value>
        public Color FirstPointColor
        {
            get
            {
                return m_firstPointColor;
            }
            set
            {
              if( !m_book.Loading )
                this.ShowFirstPoint = true;

              m_firstPointColor = value;
            }
        }
        /// <summary>
        /// Gets and sets the color of the highest points of data in the sparkline group. 
        /// </summary>
        /// <value>The color of the high point.</value>
        public Color HighPointColor
        {
            get
            {
                return m_highPointColor;
            }
            set
            {
              if( !m_book.Loading )
                this.ShowHighPoint = true;

              m_highPointColor = value;
            }
        }
        /// <summary>
        /// Gets and sets the color of the last point of data in the sparkline group.
        /// </summary>
        /// <value>The last color of the point.</value>
        public Color LastPointColor
        {
            get
            {
                return m_lastPointColor;
            }
            set
            {
              if( !m_book.Loading )
                this.ShowLastPoint = true;

              m_lastPointColor = value;
            }
        }
        /// <summary>
        /// Gets and sets the line weight in each line sparkline in the sparkline group, in the unit of points. 
        /// </summary>
        /// <value>The line weight value should be between 0 and 1584.</value>
        /// <exception cref=" ArgumentOutOfRangeException">if the value is not between 0 and 1584</exception>
        public double LineWeight
        {
            get
            {
                return m_lineWeight;
            }
            set
            {
                if ((value < 0 || value > 1584)&&(this.SparklineType != SparklineType.Line))
                    throw new ArgumentOutOfRangeException("LineWeight", "The Values should be between 0 and 1584");

                m_lineWeight = value;
            }
        }
        /// <summary>
        /// Gets and sets the color of the lowest points of data in the sparkline group.
        /// </summary>
        /// <value>The color of the low point.</value>
        public Color LowPointColor
        {
            get
            {
                return m_lowPointColor;
            }
            set
            {
              if( !m_book.Loading )
                this.ShowLowPoint = true;

              m_lowPointColor = value;
            }
        }
        /// <summary>
        ///Gets and sets the color of points in each line sparkline in the sparkline group.
        /// </summary>
        /// <value>The color of the markers.</value>
        public Color MarkersColor
        {
            get
            {
                return m_markersColor;
            }
            set
            {
              if( this.SparklineType != SparklineType.Line && !m_book.Loading )
                throw new NotSupportedException( "It is not supported for the Current SparklineType" );

              if( !m_book.Loading )
                this.ShowMarkers = true;

              m_markersColor = value;
            }
        }
        /// <summary>
        /// Gets and sets the color of the negative values on the sparkline group.
        /// </summary>
        /// <value>The color of the negative point.</value>
        public Color NegativePointColor
        {
            get
            {
                return m_negativePointColor;
            }
            set
            {
              if( !m_book.Loading )
                this.ShowNegativePoint = true;

              m_negativePointColor = value;
            }
        }
        /// <summary>
        /// Gets and sets the color of the sparklines in the sparkline group. 
        /// </summary>
        /// <value>The color of the sparkline.</value>
        public Color SparklineColor
        {
            get
            {
                return m_sparklineColor;
            }
            set
            {
                m_sparklineColor = value;
            }
        }

        #endregion

        #region Implementation
        public SparklineGroup( WorkbookImpl book )
        {
          if( book == null )
            throw new ArgumentNullException( "book" );

          m_book = book;
        }
        /// <summary>
        /// Adds Sparklines instance.
        /// </summary>
        /// <example>
        /// This Example Demostrated how to Add the Sparklines
        /// <code>
        /// ExcelEngine engine=new Excelengine();
        /// 
        /// IApplication app= engine.Excel;
        /// 
        /// app.DefaultVersion= ExcelVersion.Excel2010;
        /// 
        /// IWorkbook wkBook= app.Workbooks.Create(2); 
        /// IWorkSheet sheet= wkBook.Worksheets[0];
        /// 
        /// SparklineGroup spGroup= sheet.SparklineGroups.Add();        
        /// 
        /// //It returns the Sparklines object
        /// Sparklines spLines= spGroup.Add();
        /// 
        /// wkBook.SaveAs("Sample.xlsx");
        /// wkBook.Close();
        /// 
        /// </code>
        /// </example>
        /// <returns>Sparklines object</returns>
        public ISparklines Add()
        {
            Sparklines sparklines = new Sparklines();
            sparklines.ParentGroup = this;
            base.Add(sparklines);
            return sparklines;
        }

        #endregion

    }
}
