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

using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.Charts;
using Syncfusion.XlsIO.Implementation.Exceptions;
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
#endif
#endregion

namespace Syncfusion.XlsIO.Implementation.Charts
{
	/// <summary>
	/// Represents chart walls or floor.
	/// </summary>
	public class ChartWallOrFloorImpl
    : ChartGridLineImpl
    , IChartWallOrFloor
    , IFillColor
	{
    #region Class constants
    /// <summary>
    /// Represents default line color for walls or plot.
    /// </summary>
    public const int DEF_CATEGORY_LINE_COLOR = 8421504;
    /// <summary>
    /// Represents default color index for walls or plot.
    /// </summary>
    public const ExcelKnownColors DEF_CATEGORY_COLOR_INDEX = ( ExcelKnownColors )23;
    /// <summary>
    /// Represents default line color for floor.
    /// </summary>
    private const int DEF_VALUE_LINE_COLOR = 0;
    /// <summary>
    /// Represents default foreground color for walls or plot.
    /// </summary>
    public const int DEF_CATEGORY_FOREGROUND_COLOR = 12632256;
    /// <summary>
    /// Represents default background color index for walls or plot.
    /// </summary>
    public const ExcelKnownColors DEF_CATEGORY_BACKGROUND_COLOR_INDEX = ( ExcelKnownColors )79;
    /// <summary>
    /// Represents default value background color index.
    /// </summary>
    public const ExcelKnownColors DEF_VALUE_BACKGROUND_COLOR_INDEX = ( ExcelKnownColors )77;
    /// <summary>
    /// Represents default value foreground color index.
    /// </summary>
    private const ExcelKnownColors DEF_VALUE_FOREGROUND_COLOR_INDEX = ( ExcelKnownColors )78;
    #endregion

    #region Class members
    /// <summary>
    /// Indicates if wall or floor object.
    /// </summary>
    private bool m_bWalls;
    /// <summary>
    /// Represents chart interior.
    /// </summary>
    private ChartInteriorImpl m_interior;
    /// <summary>
    /// Represents parent chart
    /// </summary>
    private ChartImpl m_parentChart;
    /// <summary>
    /// Represents the Shadow
    /// </summary>
    private ShadowImpl m_shadow;
    /// <summary>
    /// Represents the 3D features
    /// </summary>
    private ThreeDFormatImpl m_3D;
    /// <summary>
    /// Represents fill properties.
    /// </summary>
    private ChartFillImpl m_fill;
    /// <summary>
    /// Indicates if shape properties for the wall or flooe.
    /// </summary>
    private bool m_shapeProperties;
    /// <summary>
    /// It specifies the thickness of the walls or floor as a percentage of the largest dimension of the plotarea.
    /// </summary>
    private uint m_thickness;
    /// <summary>
    /// It specifies the pictureformat struct or stretch
    /// </summary>
    private ExcelChartPictureType m_PictureUnit=ExcelChartPictureType.stretch;

    #endregion

    #region Class initialize methods
    /// <summary>
    /// Creates ChartWallsOrFloor object.
    /// </summary>
    /// <param name="application">Application object.</param>
    /// <param name="parent">Parent object.</param>
    /// <param name="bWalls">If true - represents walls; otherwise - floor.</param>
    public ChartWallOrFloorImpl( IApplication application, object parent, bool bWalls )
      : base( application, parent, ExcelAxisLineIdentifier.MajorGridLine )
    {
      AxisLineType = ExcelAxisLineIdentifier.WallsOrFloor;

      m_interior = new ChartInteriorImpl( application, this );

      ExcelVersion version = m_parentBook.Version;
      bool bNewVersion = ( version !=ExcelVersion.Excel97to2003 );
      bool bIsGray = bNewVersion ? false : true;
      m_interior.InitForFrameFormat( false, true, bIsGray, !bWalls );

      m_bWalls = bWalls;

      m_parentChart = ( ChartImpl )FindParent( typeof( ChartImpl ) );
      m_fill = new ChartFillImpl( application, this );

      if( m_parentChart == null )
        throw new ApplicationException( "Can't find parent objects" );

      SetToDefault();
    }
    /// <summary>
    /// Creates ChartWallsOrFloor object.
    /// </summary>
    /// <param name="application">Application object.</param>
    /// <param name="parent">Parent object.</param>
    /// <param name="bWalls">Indicates if it is walls.</param>
    /// <param name="data">Represents record storage.</param>
    /// <param name="iPos">Represents position in storage.</param>
    public ChartWallOrFloorImpl( IApplication application, object parent, bool bWalls
      , IList<BiffRecordRaw> data, ref int iPos )
      : base( application, parent, data, ref iPos )
    {
      AxisLineType = ExcelAxisLineIdentifier.WallsOrFloor;

      m_bWalls = bWalls;

      m_parentChart = ( ChartImpl )FindParent( typeof( ChartImpl ) );

      if( m_fill == null )
        m_fill = new ChartFillImpl( application, this );

      if( m_parentChart == null )
        throw new ApplicationException( "Can't find parent objects" );

      // Parse method is called by the base class
      //Parse( data, ref iPos );
    }
    #endregion

    #region Class parse methods
    /// <summary>
    /// Parses walls or floor records.
    /// </summary>
    /// <param name="data">Offset array list.</param>
    /// <param name="iPos">Current position in offset array list.</param>
    [ CLSCompliant( false ) ]
    public override void Parse( IList<BiffRecordRaw> data, ref int iPos )
    {
      m_interior = null;

      base.Parse( data, ref iPos );

      if( AxisLineType != ExcelAxisLineIdentifier.WallsOrFloor )
        throw new ParseException( "Bad axis line type" );

      BiffRecordRaw record = data[ iPos ];

      if( record.TypeCode == TBIFFRecord.ChartAreaFormat )
        m_interior = new ChartInteriorImpl( Application, this, data, ref iPos );

      record = ( BiffRecordRaw )data[ iPos ];

      if( record.TypeCode == TBIFFRecord.ChartGelFrame )
      {
        m_fill = new ChartFillImpl( Application, this, ( ChartGelFrameRecord )record );
        iPos++;
      }

      int iCount = 1;

      while( iCount > 0 )
      {
        record = ( BiffRecordRaw )data[ iPos ];

        switch( record.TypeCode )
        {
          case TBIFFRecord.Begin:
            iCount++;
            break;

          case TBIFFRecord.End:
            iCount--;
            break;
        }

        iPos++;
      }

      iPos--;
    }
    #endregion

    #region Class serialize methods
    /// <summary>
    /// Serializes wall or floor object.
    /// </summary>
    /// <param name="records">OffsetArrayList that will receive all records.</param>
    [ CLSCompliant( false ) ]
    public override void Serialize( OffsetArrayList records )
    {
      base.Serialize( records );

      if( m_interior != null )
        m_interior.Serialize( records );

      m_fill.Serialize( records );
    }
    #endregion

    #region IChartWallOrFloor properties
    /// <summary>
    /// Represents chart interior.
    /// </summary>
    public  IChartInterior Interior
    {
      get
      {
        if( m_interior == null )
          m_interior = new ChartInteriorImpl( Application, this );

        return m_interior;
      }
    }
    /// <summary>
        /// Represents the Shadow.Read-only
        /// </summary>
        /// <value></value>
        public IShadow Shadow
        {
            get
            {
               
                if (m_shadow == null)
                    m_shadow = new ShadowImpl(Application, this);
                //if (m_shadow.HasCustomShadowStyle == true)
                //    throw new NotSupportedException("It is not supported when Custom Shadow style is set to true");

                return m_shadow;
            }
        }
        /// <summary>
        /// This property indicates whether the shadow object has been created
        /// </summary>
        /// <value></value>
        public bool HasShadowProperties
        {
            get
            {
                return m_shadow != null;
            }
            internal set
            {
                if (value)
                {
                    IShadow shadow = Shadow;
                }
                else
                {
                    m_shadow = null;
                }
            }
        }
        /// <summary>
        /// Indicates whether Shape properties has been created.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has shape properties; otherwise, <c>false</c>.
        /// </value>
        internal bool HasShapeProperties
        {
            get
            {
                return m_shapeProperties;
            }
            set
            {
                m_shapeProperties = value;
            }
        }
        /// <summary>
        /// Returns or sets the thickness of the walls or floor as a percentage of the largest dimension of the plot area.
        /// </summary>
        /// <value>The thickness.</value>
        public uint Thickness
        {
            get
            {
                return m_thickness;
            }
            set
            {
                m_thickness = value;
            }
        }
        /// <summary>
        /// Gets or Sets the picture format of the walls or floor
        /// </summary>
        public ExcelChartPictureType PictureUnit
        {
            get
            {
                return m_PictureUnit;
            }
            set
            {
                if (value == ExcelChartPictureType.stack)
                    m_PictureUnit = value;
                else
                    m_PictureUnit = ExcelChartPictureType.stretch;
            }
        }               
        /// <summary>
        /// Gets the chart3 D options.
        /// </summary>
        /// <value>The chart3 D options.</value>
        public IThreeDFormat ThreeD
        {
            get
            {
                if (m_3D == null)
                    m_3D = new ThreeDFormatImpl(Application, this);

                return m_3D;
            }
        }
        /// <summary>
        /// This property Indicates whether the Shadow object has been created(which includes the 3D properties)
        /// </summary>
        public bool Has3dProperties
        {
            get
            {
                return m_3D != null;
            }
            internal set
            {
                if (value)
                {
                    IThreeDFormat Threed = ThreeD;
                }
                else
                {
                    m_3D = null;
                }
            }
        }
    /// <summary>
    /// Represents fill properties. Read-only.
    /// </summary>
    public  IFill Fill
    {
      get
      {
        IsAutomaticFormat = false;

        return m_fill;
      }
    }
    /// <summary>
    /// This property indicates whether interior object was created. Read-only.
    /// </summary>
    public bool HasInterior
    {
      get
      {
        return m_interior != null;
      }
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Indicates if this object is walls or floor.
    /// </summary>
    private bool IsWall
    {
      get
      {
        return m_bWalls;
      }
    }
    #endregion

    #region IGridLines methods
    /// <summary>
    /// Clears current walls or floor.
    /// </summary>
    public override void Delete()
    {
      if( m_bWalls )
      {
        m_parentChart.Walls = new ChartWallOrFloorImpl( Application, m_parentChart, true );
        m_parentChart.SideWall = new ChartWallOrFloorImpl(Application, m_parentChart, true);        
      }
      else
      {
        m_parentChart.Floor = new ChartWallOrFloorImpl( Application, m_parentChart, false );
      }
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Sets to default floor or walls.
    /// </summary>
    public void SetToDefault()
    {
      if( m_bWalls )
      {
        SetToDefaultCategoryLine();
        SetToDefaultCategoryArea();
      }
      else
      {
        SetToDefaultValueLine();
        SetToDefaultValueArea();
      }
    }
    /// <summary>
    /// Sets as default line record in category axis.
    /// </summary>
    private void SetToDefaultCategoryLine()
    {
      Border.LineWeight = ExcelChartLineWeight.Narrow;
      Border.ColorIndex = DEF_CATEGORY_COLOR_INDEX;
    }
    /// <summary>
    /// Sets as default line record in value axis.
    /// </summary>
    private void SetToDefaultValueLine()
    {
      if( m_parentBook.Version == ExcelVersion.Excel97to2003 )
      {
        Border.ColorIndex = DEF_VALUE_BACKGROUND_COLOR_INDEX;
      }
      else
      {
        Border.ColorIndex = ExcelKnownColors.Grey_25_percent;
      }
    }
    /// <summary>
    /// Sets as default area record in category axis.
    /// </summary>
    private void SetToDefaultCategoryArea()
    {
      if( m_parentBook.Version == ExcelVersion.Excel97to2003 )
      {
        m_interior.Pattern = ExcelPattern.Solid;
        m_interior.ForegroundColorObject.SetIndexed( ExcelKnownColors.Grey_25_percent );
        m_interior.BackgroundColorObject.SetIndexed( DEF_CATEGORY_BACKGROUND_COLOR_INDEX );
      }
      else
      {
        m_interior.Pattern = ExcelPattern.None;
      }
      //m_interior.BackgroundColor = Color.FromArgb( 0 );
    }
    /// <summary>
    /// Sets as default area record in value axis.
    /// </summary>
    private void SetToDefaultValueArea()
    {
      Interior.UseAutomaticFormat = true;
    }
    /// <summary>
    /// Clones current object.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <returns>Returns cloned object.</returns>
    public override object Clone( object parent )
    {
      ChartWallOrFloorImpl result = ( ChartWallOrFloorImpl )base.Clone( parent );

      if( m_interior != null )
        result.m_interior = m_interior.Clone( result );

      return result;
    }

    #endregion

    #region IFillColor properties
    /// <summary>
    /// Represents foreground color.
    /// </summary>
    public ColorObject ForeGroundColorObject
    {
      get
      {
        return m_interior.ForegroundColorObject;
      }
    }
    /// <summary>
    /// Represents background color.
    /// </summary>
    public ColorObject BackGroundColorObject
    {
      get
      {
        return m_interior.BackgroundColorObject;
      }
    }
    /// <summary>
    /// Represents pattern.
    /// </summary>
    public ExcelPattern Pattern
    {
      get
      {
        return Interior.Pattern;
      }
      set
      {
        Interior.Pattern = value;
      }
    }
    /// <summary>
    /// Represents if use automatic format.
    /// </summary>
    public bool IsAutomaticFormat
    {
      get
      {
        return Interior.UseAutomaticFormat;
      }
      set
      {
        Interior.UseAutomaticFormat = value;
      }
    }
    /// <summary>
    /// Represents visible.
    /// </summary>
    public bool Visible
    {
      get
      {
        return Interior.Pattern != ExcelPattern.None;
      }
      set
      {
        if( value )
        {
          if( Interior.Pattern == ExcelPattern.None )
            Interior.Pattern = ExcelPattern.Solid;
        }
        else
        {
          Interior.Pattern = ExcelPattern.None;
        }
      }
    }
    #endregion
	}
}
