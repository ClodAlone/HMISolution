#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections;

using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.Charts;
using System.Collections.Generic;

#if ( WINRT )
using Windows.UI;
using Syncfusion.XlsIO.Implementation.Exceptions;
#endif
#if  (SILVERLIGHT || WP)
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.Exceptions;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif

namespace Syncfusion.XlsIO.Implementation.Charts
{
  /// <summary>
  /// Represents chart interior.
  /// </summary>
  public class ChartInteriorImpl
    : CommonObject
    , IChartInterior
    , ICloneParent
  {
    #region Class members
    /// <summary>
    /// Represents area format record.
    /// </summary>
    private ChartAreaFormatRecord m_area;
    /// <summary>
    /// Represents parent book.
    /// </summary>
    private WorkbookImpl m_book;
    /// <summary>
    /// Represents parent Series format.
    /// </summary>
    private ChartSerieDataFormatImpl m_serieFormat;
    /// <summary>
    /// Represents series fore color.
    /// </summary>
    private ColorObject m_foreColor;
    /// <summary>
    /// Represents series back color.
    /// </summary>
    private ColorObject m_backColor;
    #endregion

    #region Class static members
    /// <summary>
    /// Represents dictionary to convert excelPattern to excelgradient pattern.
    /// key - ExcelPattern, value - ExcelGradientPattern.
    /// </summary>
    private static Dictionary<ExcelPattern, ExcelGradientPattern> m_hashPat = new Dictionary<ExcelPattern, ExcelGradientPattern>( 18 );
    #endregion

    #region Class static contstructors
    /// <summary>
    /// Initialize all static members.
    /// </summary>
    static ChartInteriorImpl()
    {
      m_hashPat.Add( ( ExcelPattern )2, ( ExcelGradientPattern )7 );
      m_hashPat.Add( ( ExcelPattern )3, ( ExcelGradientPattern )9 );
      m_hashPat.Add( ( ExcelPattern )4, ( ExcelGradientPattern )4 );
      m_hashPat.Add( ( ExcelPattern )16, ( ExcelGradientPattern )5 );
      m_hashPat.Add( ( ExcelPattern )17, ( ExcelGradientPattern )3 );
      m_hashPat.Add( ( ExcelPattern )18, ( ExcelGradientPattern )2 );

      for( int i = 5; i < 16; i++ )
      {
        m_hashPat.Add( ( ExcelPattern )i, ( ExcelGradientPattern )( i + 8 ) );
      }
    }
    #endregion

    #region Class intialize methods
    /// <summary>
    /// Creates chart interior instance.
    /// </summary>
    /// <param name="application">Represents current application.</param>
    /// <param name="parent">Represents parent object.</param>
    public ChartInteriorImpl( IApplication application, object parent )
      : base( application, parent )
    {
      m_area = ( ChartAreaFormatRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ChartAreaFormat );

      SetParents();
    }
    /// <summary>
    /// Creates chart interior instance.
    /// </summary>
    /// <param name="application">Represents current application.</param>
    /// <param name="parent">Represents parent object.</param>
    /// <param name="area">Represents area record.</param>
    [ CLSCompliant( false ) ]
    public ChartInteriorImpl( IApplication application, object parent, ChartAreaFormatRecord area )
      : base( application, parent )
    {
      if( area == null )
        throw new ArgumentNullException( "area" );
      m_area = area;

      SetParents();
    }
    /// <summary>
    /// Creates new instance of class.
    /// </summary>
    /// <param name="application">Represents current application.</param>
    /// <param name="parent">Represents parent object.</param>
    /// <param name="data">Represents record storage.</param>
    /// <param name="iPos">Represents position in storage.</param>
    public ChartInteriorImpl( IApplication application, object parent, IList<BiffRecordRaw> data, ref int iPos )
      : base( application, parent )
    {
      Parse( data, ref iPos );
      SetParents();
    }
    #endregion

    #region Class parse\serialize methods
    /// <summary>
    /// Finds parent objects.
    /// </summary>
    private void SetParents()
    {
      m_book = ( WorkbookImpl )FindParent( typeof( WorkbookImpl ) );
      m_serieFormat = FindParent( typeof( ChartSerieDataFormatImpl ) ) as ChartSerieDataFormatImpl;

      if( m_book == null )
        throw new ApplicationException( "cannot find parent object" );

      m_foreColor = new ColorObject( m_area.ForegroundColorIndex );
      m_foreColor.AfterChange += new ColorObject.AfterChangeHandler( UpdateForeColor );

      m_backColor = new ColorObject( m_area.BackgroundColorIndex );
      m_backColor.AfterChange += new ColorObject.AfterChangeHandler( UpdateBackColor );
    }

    /// <summary>
    /// Parsing current object.
    /// </summary>
    /// <param name="data">Records offset.</param>
    /// <param name="iPos">Position in offset.</param>
    public void Parse( IList<BiffRecordRaw> data, ref int iPos )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      BiffRecordRaw record = data[ iPos ];
      record.CheckTypeCode( TBIFFRecord.ChartAreaFormat );

      m_area = ( ChartAreaFormatRecord )record;

      iPos++;
    }
    /// <summary>
    /// Serialize current object.
    /// </summary>
    /// <param name="records">Records offset.</param>
    [ CLSCompliant( false ) ]
    public void Serialize( IList<IBiffStorage> records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      if( m_area != null )
        records.Add( ( BiffRecordRaw )m_area.Clone() );
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Foreground color (RGB).
    /// </summary>
    public ColorObject ForegroundColorObject
    {
      get
      {
        return m_foreColor;
      }
    }
    /// <summary>
    /// Background color (RGB).
    /// </summary>
    public ColorObject BackgroundColorObject
    {
      get
      {
        return m_backColor;
      }
    }
    /// <summary>
    /// Foreground color (RGB).
    /// </summary>
    public Color ForegroundColor
    {
      get
      {
        return m_foreColor.GetRGB( m_book );
      }
      set
      {
        m_foreColor.SetRGB( value, m_book );
      }
    }
    /// <summary>
    /// Background color (RGB).
    /// </summary>
    public Color BackgroundColor
    {
      get
      {
        return m_backColor.GetRGB( m_book );
      }
      set
      {
        m_backColor.SetRGB( value, m_book );
      }
    }
    /// <summary>
    /// Area pattern.
    /// </summary>
    public ExcelPattern Pattern
    {
      get
      {
        return ( UseAutomaticFormat ) ? ExcelPattern.Solid : m_area.Pattern;
      }
      set
      {
        if( Pattern != value )
        {
          int iPatIndex = ( int )value;
          IFill fill = ( Parent as IFillColor ).Fill;

          if( iPatIndex < 2 )
          {
            if( ( int )Pattern > 1 )
              fill.Solid();
          }
          else
          {
            fill.Patterned( m_hashPat[ value ] );
          }

          UseAutomaticFormat = false;
          m_area.Pattern = value;

//          if( m_serieFormat != null )
//            m_serieFormat.ClearOnPropertyChange();
        }
      }
    }
    /// <summary>
    /// Index of foreground color.
    /// </summary>
    public ExcelKnownColors ForegroundColorIndex
    {
      get
      {
        return m_foreColor.GetIndexed( m_book );
      }
      set
      {
        m_foreColor.SetIndexed( value );
      }
    }
    /// <summary>
    /// Background color index.
    /// </summary>
    public ExcelKnownColors BackgroundColorIndex
    {
      get
      {
        return m_backColor.GetIndexed( m_book );
      }
      set
      {
        m_backColor.SetIndexed( value );
      }
    }
    /// <summary>
    /// If true - use automatic format; otherwise custom.
    /// </summary>
    public bool   UseAutomaticFormat
    {
      get
      {
        return m_area.UseAutomaticFormat;
      }
      set
      {
        if( value != UseAutomaticFormat )
        {
          m_area.UseAutomaticFormat = value;

          if( !value && m_area.Pattern == ExcelPattern.None )
            m_area.Pattern = ExcelPattern.Solid;

//          if( m_serieFormat != null )
//            m_serieFormat.ClearOnPropertyChange();
        }
      }
    }
    /// <summary>
    /// Foreground and background are swapped when the data value is negative.
    /// </summary>
    public bool   SwapColorsOnNegative
    {
      get
      {
        return m_area.SwapColorsOnNegative;
      }
      set
      {
        m_area.SwapColorsOnNegative = value;
      }
    }
    /// <summary>
    /// Updates foreground color.
    /// </summary>
    private void UpdateForeColor()
    {
      m_area.ForegroundColorIndex = ForegroundColorIndex;
      m_area.ForegroundColor = ( ForegroundColor.ToArgb() & 0xffffff );

      UseAutomaticFormat = false;
      ( Parent as IFillColor ).Visible = true;

      //        if( m_serieFormat != null )
      //          m_serieFormat.ClearOnPropertyChange();
    }
    /// <summary>
    /// Updated background color.
    /// </summary>
    private void UpdateBackColor()
    {
      m_area.BackgroundColorIndex = BackgroundColorIndex;
      m_area.BackgroundColor = BackgroundColor;
      UseAutomaticFormat = false;
      ( Parent as IFillColor ).Visible = true;

      //        if( m_serieFormat != null )
      //          m_serieFormat.ClearOnPropertyChange();
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Initialize interior for frame format.
    /// </summary>
    /// <param name="bIsAutoSize">Indicate is autosize interior.</param>
    /// <param name="bIs3DChart">Represents is 3d chart.</param>
    /// <param name="bIsInteriorGray">Indicates is interior is gray.</param>
    public void InitForFrameFormat( bool bIsAutoSize, bool bIs3DChart, bool bIsInteriorGray )
    {
      InitForFrameFormat( bIsAutoSize, bIs3DChart, bIsInteriorGray, false );
    }
    /// <summary>
    /// Initialize interior for frame format.
    /// </summary>
    /// <param name="bIsAutoSize">Indicate is autosize interior.</param>
    /// <param name="bIs3DChart">Represents is 3d chart.</param>
    /// <param name="bIsInteriorGray">Indicates is interior is gray.</param>
    /// <param name="bIsGray50">Indicates is default color is gray_50.</param>
    public void InitForFrameFormat( bool bIsAutoSize, bool bIs3DChart, bool bIsInteriorGray, bool bIsGray50 )
    {
      m_area.Pattern = ExcelPattern.Solid;
      m_area.UseAutomaticFormat = bIs3DChart;
      m_area.SwapColorsOnNegative = false;
      m_area.ForegroundColorIndex = ( ExcelKnownColors )( bIsInteriorGray ? 22 : 1 );
      m_area.BackgroundColorIndex = ( ExcelKnownColors )( bIsAutoSize ? 79 : 77 );

      if( bIsGray50 )
        m_area.ForegroundColorIndex = ExcelKnownColors.Grey_50_percent;
    }
    /// <summary>
    /// Clones current object.
    /// </summary>
    /// <param name="parent">Represents parent object.</param>
    /// <returns>Returns cloned object.</returns>
    public ChartInteriorImpl Clone( object parent )
    {
      if( parent == null )
        throw new ArgumentNullException( "parent" );

      ChartInteriorImpl result = ( ChartInteriorImpl )MemberwiseClone();

      result.m_area = ( ChartAreaFormatRecord )CloneUtils.CloneCloneable( m_area );

      result.SetParent( parent );
      result.SetParents();

      return result;
    }
    #endregion

    #region ICloneParent Members
    /// <summary>
    /// Makes copy of the current object and update its parent.
    /// </summary>
    /// <param name="parent">Represents Parent object to set.</param>
    /// <returns>Cloned object.</returns>
    object ICloneParent.Clone( object parent )
    {
      return Clone( parent );
    }

    #endregion
  }
}
