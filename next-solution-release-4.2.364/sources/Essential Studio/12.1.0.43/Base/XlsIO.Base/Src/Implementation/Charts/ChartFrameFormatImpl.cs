#region Copyright
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
//
#endregion Copyright

#region file using directives
using System;
using System.Collections;

using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.Charts;
using System.IO;
using System.Collections.Generic;

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif
#if  SILVERLIGHT
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
  /// Class used for Chart FrameFormat implementation.
  /// </summary>
  public class ChartFrameFormatImpl
    : CommonObject
    , IChartFrameFormat
    , IFillColor
  {
    #region Class members
    /// <summary>
    /// Chart frame.
    /// </summary>
    private ChartFrameRecord m_chartFrame = ( ChartFrameRecord )
      BiffRecordFactory.GetRecord( TBIFFRecord.ChartFrame );
    /// <summary>
    /// Represents chart border.
    /// </summary>
    private ChartBorderImpl m_border;
    /// <summary>
    /// Represents the 3D features
    /// </summary>
    private ThreeDFormatImpl m_3D;
    /// <summary>
    /// Represents chart interior.
    /// </summary>
    private ChartInteriorImpl m_interior;
    /// <summary>
    /// Represents Shadow
    /// </summary>
    private ShadowImpl m_shadow;
    /// <summary>
    /// Represents fill format.
    /// </summary>
    private ChartFillImpl m_fill;
    /// <summary>
    /// Parent chart.
    /// </summary>
    protected ChartImpl m_chart;
    /// <summary>
    /// Represents Excel 2007 layout data.
    /// </summary>
    private IChartLayout m_layout;
    /// <summary>
    /// Plot area layout
    /// </summary>
    protected ChartPlotAreaLayoutRecord m_plotAreaLayout;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates chart and sets its Application and Parent
    /// properties to specified values.
    /// </summary>
    /// <param name="application">Application object for the chart.</param>
    /// <param name="parent">Parent object for the chart.</param>
    public ChartFrameFormatImpl( IApplication application, object parent )
      : this( application, parent, false, false, true )
    {
    }
    /// <summary>
    /// Creates chart and sets its Application and Parent
    /// properties to specified values.
    /// </summary>
    /// <param name="application">Application object for the chart.</param>
    /// <param name="parent">Parent object for the chart.</param>
    /// <param name="bSetDefaults">Indicates whether we should set defaults for fill and border properties.</param>
    public ChartFrameFormatImpl( IApplication application, object parent, bool bSetDefaults )
      : this( application, parent, false, false, bSetDefaults )
    {
    }
    /// <summary>
    /// Creates chart and sets its Application and Parent
    /// properties to specified values.
    /// </summary>
    /// <param name="application">Application object for the chart.</param>
    /// <param name="parent">Parent object for the chart.</param>
    /// <param name="bAutoSize">Indicates is auto size.</param>
    /// <param name="bIsInteriorGrey">Indicates is interior is gray.</param>
    /// <param name="bSetDefaults">Indicates whether we should set defaults for fill and border properties.</param>
    public ChartFrameFormatImpl( IApplication application, object parent, bool bAutoSize,
      bool bIsInteriorGrey, bool bSetDefaults )
      : base( application, parent )
    {
      SetParents();

      if( !Workbook.Loading && bSetDefaults )
        SetDefaultValues( bAutoSize, bIsInteriorGrey );
    }
    /// <summary>
    /// Creates and parses current object.
    /// </summary>
    /// <param name="application">Application object for the chart.</param>
    /// <param name="parent">Parent object for the chart.</param>
    /// <param name="data">Records storage.</param>
    /// <param name="iPos">Position in storage.</param>
    public ChartFrameFormatImpl( IApplication application, object parent, IList<BiffRecordRaw> data, ref int iPos )
      : base( application, parent )
    {
      SetParents();
      Parse( data, ref iPos );
    }
    /// <summary>
    /// Searches for all necessary parent objects.
    /// </summary>
    private void SetParents()
    {
      m_chart = FindParent( typeof( ChartImpl ) ) as ChartImpl;

      if( m_chart == null )
      {
        throw new ArgumentNullException( "Can't find parent chart" );
      }
    }
    #endregion

    #region Class parse / serialize methods
    /// <summary>
    /// Parses frame.
    /// </summary>
    /// <param name="data">Array with frame records.</param>
    /// <param name="iPos">Position of the frame records.</param>
    [ CLSCompliant( false ) ]
    public void Parse( IList<BiffRecordRaw> data, ref int iPos )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      if( iPos < 0 || iPos > data.Count )
        throw new ArgumentOutOfRangeException( "iPos", "Value cannot be less than 0 and greater than data.Count" );

      BiffRecordRaw record = ( BiffRecordRaw )data[ iPos ];
      record = UnwrapRecord( record );
      record.CheckTypeCode( TBIFFRecord.ChartFrame );
      m_chartFrame = ( ChartFrameRecord )record;

      iPos++;
      record = ( BiffRecordRaw )data[ iPos ];

      int iBeginCounter = 0;
      
      
      if( CheckBegin( record ) )
      {
        iBeginCounter++;

        do
        {
          iPos++;
          record = ( BiffRecordRaw )data[ iPos ];
          ParseRecord( record, ref iBeginCounter );
        }
        while( iBeginCounter != 0 );

        iPos++;
      }
    }
    /// <summary>
    /// Checks whether specified record is begin.
    /// </summary>
    /// <param name="record">Record to check.</param>
    /// <returns>True if this is begin; false otherwise.</returns>
    [ CLSCompliant( false ) ]
    protected virtual bool CheckBegin( BiffRecordRaw record )
    {
      if( record == null )
        throw new ArgumentNullException( "record" );

      return ( record.TypeCode == TBIFFRecord.Begin );
    }
    /// <summary>
    /// Parses single record.
    /// </summary>
    /// <param name="record">Record to parse.</param>
    /// <param name="iBeginCounter">Number of not closed begin record.</param>
    [ CLSCompliant( false ) ]
    protected virtual void ParseRecord( BiffRecordRaw record, ref int iBeginCounter )
    {
      if( record == null )
        throw new ArgumentNullException( "record" );

      switch( record.TypeCode )
      {
        case TBIFFRecord.Begin:
          iBeginCounter++;
          break;

        case TBIFFRecord.End:
          iBeginCounter--;
          break;

        case TBIFFRecord.ChartAreaFormat:
          m_interior = new ChartInteriorImpl( Application, this, ( ChartAreaFormatRecord )record );
          break;

        case TBIFFRecord.ChartLineFormat:
          m_border = new ChartBorderImpl( Application, this, ( ChartLineFormatRecord )record );
          break;

        case TBIFFRecord.ChartGelFrame:
          m_fill = new ChartFillImpl( Application, this, ( ChartGelFrameRecord )record );
          break;

        case TBIFFRecord.PlotAreaLayout:
          if (m_plotAreaLayout == null)
            m_plotAreaLayout = (ChartPlotAreaLayoutRecord)BiffRecordFactory.GetRecord(TBIFFRecord.PlotAreaLayout);
          m_plotAreaLayout = (ChartPlotAreaLayoutRecord)record;
          break;
      }
    }
    /// <summary>
    /// Saves chart frame as biff records.
    /// </summary>
    /// <param name="records">OffsetArrayList that will get biff records.</param>
    [ CLSCompliant( false ) ]
    public void Serialize( IList<IBiffStorage> records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      SerializeRecord( records, m_chartFrame );
      SerializeRecord( records, BiffRecordFactory.GetRecord( TBIFFRecord.Begin ) );
      
      if( m_border != null )
        m_border.Serialize( records );

      if(m_interior !=null)
      m_interior.Serialize( records );

      if( m_fill != null )
        m_fill.Serialize( records );

      if (m_plotAreaLayout != null)
          SerializeRecord(records, m_plotAreaLayout);

      SerializeRecord( records, BiffRecordFactory.GetRecord( TBIFFRecord.End ) );
    }
    /// <summary>
    /// Serializes single record.
    /// </summary>
    /// <param name="list">OffsetArrayList that will get biff records.</param>
    /// <param name="record">Record to serialize.</param>
    [ CLSCompliant( false ) ]
    protected virtual void SerializeRecord( IList<IBiffStorage> list, BiffRecordRaw record )
    {
      if( list == null )
        throw new ArgumentNullException( "list" );

      if( record == null )
        throw new ArgumentNullException( "record" );

      list.Add( ( BiffRecordRaw )record.Clone() );
    }
    /// <summary>
    /// Unwraps record.
    /// </summary>
    /// <param name="record">Record to unwrap.</param>
    /// <returns>Unwrapped record.</returns>
    [ CLSCompliant( false ) ]
    protected virtual BiffRecordRaw UnwrapRecord( BiffRecordRaw record )
    {
      return record;
    }
    #endregion

    #region Class members
    /// <summary>
    /// Set variable to the default state.
    /// </summary>
    /// <param name="bAutoSize">Indicates whether MS Excel should calculate size of the frame.</param>
    /// <param name="bIsInteriorGray">Indicates is default interior is gray.</param>
    public void SetDefaultValues( bool bAutoSize, bool bIsInteriorGray )
    {
      m_chartFrame = ( ChartFrameRecord )
        BiffRecordFactory.GetRecord( TBIFFRecord.ChartFrame );

      m_chartFrame.AutoSize = bAutoSize;

      m_border = new ChartBorderImpl( Application, this );

      m_border.ColorIndex = ChartWallOrFloorImpl.DEF_CATEGORY_COLOR_INDEX;
      m_border.AutoFormat = !m_chart.IsChart3D;
      
      m_interior = new ChartInteriorImpl( Application, this );
      m_interior.InitForFrameFormat( bAutoSize, m_chart.IsChart3D, bIsInteriorGray );
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Gets frame record. Read-only.
    /// </summary>
    [ CLSCompliant( false ) ]
    public ChartFrameRecord FrameRecord
    {
      get
      {
        return m_chartFrame;
      }
    }
    /// <summary>
    /// Returns parent workbook. Read-only.
    /// </summary>
    public WorkbookImpl Workbook
    {
      get
      {
        return m_chart.InnerWorkbook;
      }
    }
    /// <summary>
    /// Gets / sets Excel 2007 layout data.
    /// </summary>
    public IChartLayout Layout
    {
        get
        {
            return m_layout;
        }
        set
        {
            m_layout = value;
        }
    }
    #endregion

    #region IChartFrameFormat Members
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
    /// <summary>
    /// This property indicates whether border formatting object was created. Read-only.
    /// </summary>
    public bool HasLineProperties
    {
      get
      {
        return m_border != null;
      }
      internal set
      {
        if( value )
        {
          // here we should call Border property to initialize line settings.
          IChartBorder border = Border;
        }
        else
        {
          m_border = null;
        }
      }
    }
    /// <summary>
    /// Represents chart border. Read-only.
    /// </summary>
    public IChartBorder Border
    {
      get
      {
        if( m_border == null )
          m_border = new ChartBorderImpl( Application, this );

        return m_border;
      }
    }
    /// <summary>
    /// Represents frame interior. Read-only
    /// </summary>
    public IChartInterior Interior
    {
      get
      {
        if( m_interior == null )
          m_interior = new ChartInteriorImpl( Application, this );

        return m_interior;
      }
    }
    /// <summary>
        /// Gets the chart3 D properties.
        /// </summary>
        /// <value>The chart3 D properties.</value>
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
    /// Represents fill gradient format. Read-only.
    /// </summary>
    public IFill    Fill
    {
      get
      {
        if( m_fill == null )
          m_fill = new ChartFillImpl( Application, this );

        IsAutomaticFormat = false;

        return m_fill;
      }
    }
    /// <summary>
        /// This property indicates whether the shadow object has been created 
        /// </summary>
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
        /// Gets the shadow properties.
        /// </summary>
        /// <value>The shadow properties.</value>
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
    /// Rectangle style.
    /// </summary>
    public ExcelRectangleStyle RectangleStyle
    {
      get
      {
        return m_chartFrame.Rectangle;
      }
      set
      {
        m_chartFrame.Rectangle = value;
      }
    }

    /// <summary>
    /// Microsoft Excel calculates size.
    /// </summary>
    public bool IsAutoSize
    {
      get
      {
        return m_chartFrame.AutoSize;
      }
      set
      {
        m_chartFrame.AutoSize = value;
      }
    }

    /// <summary>
    /// Microsoft Excel calculates position.
    /// </summary>
    public bool IsAutoPosition
    {
      get
      {
        return m_chartFrame.AutoPosition;
      }
      set
      {
        m_chartFrame.AutoPosition = value;
      }
    }

    /// <summary>
    /// Gets or sets flag if border corners is round.
    /// </summary>
    public bool IsBorderCornersRound
    {
      get
      {
        return Interior.SwapColorsOnNegative;
      }
      set
      {
        Interior.SwapColorsOnNegative = value;
      }
    }
    /// <summary>
    /// Represents chart border. Read-only.
    /// </summary>
    public IChartBorder LineProperties
    {
      get
      {
        return Border;
      }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Updates known colors.
    /// </summary>
    /// <param name="color">Color to update.</param>
    /// <returns>Returns updated color.</returns>
    public static ExcelKnownColors UpdateLineColor( ExcelKnownColors color )
    {
      int iColorIndex = ( int )color;

      if( iColorIndex < BorderImpl.DEF_MAXBADCOLOR )
        return ( ExcelKnownColors )( iColorIndex + BorderImpl.DEF_MAXBADCOLOR );

      return color;
    }
    /// <summary>
    /// Clears current frame.
    /// </summary>
    public void Clear()
    {
      SetDefaultValues( false, false );
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
        return ( Interior as ChartInteriorImpl ).ForegroundColorObject;
      }
    }
    /// <summary>
    /// Represents background color.
    /// </summary>
    public ColorObject BackGroundColorObject
    {
      get
      {
        return ( Interior as ChartInteriorImpl ).BackgroundColorObject;
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
    /// Represents visibility.
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

    #region Clone methods
    /// <summary>
    /// Clone current Record.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <returns>Returns clone of current object.</returns>
    public ChartFrameFormatImpl Clone( object parent )
    {
      ChartFrameFormatImpl result = ( ChartFrameFormatImpl )MemberwiseClone();
      result.SetParent( parent );
      result.SetParents();

      result.m_bIsDisposed = m_bIsDisposed;

      if( m_chartFrame != null )
      {
        result.m_chartFrame = ( ChartFrameRecord )m_chartFrame.Clone();
      }

      if( m_border != null )
        result.m_border = m_border.Clone( result );

      if( m_interior != null )
        result.m_interior = m_interior.Clone( result );

      if( m_fill != null )
        result.m_fill = ( ChartFillImpl )m_fill.Clone( result );

      return result;
    }
    #endregion
  }
}
