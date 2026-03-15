#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections;
using System.Diagnostics;

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
  /// Represents ChartDropBar record and another records.
  /// </summary>
  public class ChartDropBarImpl
    : CommonObject
    , IChartDropBar
    , IFillColor
  {
    #region Class members
    /// <summary>
    /// Represents chart drop bar record.
    /// </summary>
    private ChartDropBarRecord m_dropBar;
    /// <summary>
    /// Represents drop bar line format.
    /// </summary>
    private ChartBorderImpl m_lineFormat;
    /// <summary>
    /// Represents drop bar area format.
    /// </summary>
    private ChartInteriorImpl m_interior;
    /// <summary>
    /// Represents parent workBook.
    /// </summary>
    private WorkbookImpl m_parentBook;
    /// <summary>
    /// Represents drop bar filling options.
    /// </summary>
    private ChartFillImpl m_fill;
        /// <summary>
        /// Represents the 3D features
        /// </summary>
        private ThreeDFormatImpl m_3D;
        /// <summary>
        /// Represents Shadow
        /// </summary>
        private ShadowImpl m_shadow;
        //private bool m_customShadow = false;
    #endregion

    #region Class initialize methods
    /// <summary>
    /// Initialize new instance.
    /// </summary>
    /// <param name="application">Current application.</param>
    /// <param name="parent">Parent object.</param>
    public ChartDropBarImpl( IApplication application, object parent )
      : base( application, parent )
    {
      SetParents();

      m_dropBar = ( ChartDropBarRecord )
        BiffRecordFactory.GetRecord( TBIFFRecord.ChartDropBar );
    }
    /// <summary>
    /// Sets parent objects.
    /// </summary>
    private void SetParents()
    {
      m_parentBook = ( WorkbookImpl )FindParent( typeof( WorkbookImpl ) );

      if( m_parentBook == null )
        throw new ArgumentNullException( "Cannot find parent object." );
    }
    #endregion

    #region Parse methods
    /// <summary>
    /// Parses current block of records.
    /// </summary>
    /// <param name="data">Offset array list.</param>
    /// <param name="iPos">Current position in offset array list.</param>
    [ CLSCompliant( false ) ]
    public void Parse( IList<BiffRecordRaw> data, ref int iPos )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      BiffRecordRaw record = ( BiffRecordRaw )data[ iPos ];
      record.CheckTypeCode( TBIFFRecord.ChartDropBar );
      m_dropBar = ( ChartDropBarRecord )data[ iPos ];

      ( ( BiffRecordRaw )data[ iPos + 1 ] ).CheckTypeCode( TBIFFRecord.Begin );

      iPos = iPos + 2;

      int iCount = 1;

      while( iCount > 0 )
      {
        record = ( BiffRecordRaw )data[ iPos ];

        switch( record.TypeCode )
        {
          case TBIFFRecord.Begin:
            iCount++;
            iPos = BiffRecordRaw.SkipBeginEndBlock( data, iPos );
            break;

          case TBIFFRecord.End:
            iCount--;
            break;

          case TBIFFRecord.ChartLineFormat:
            m_lineFormat = new ChartBorderImpl( Application, this, ( ChartLineFormatRecord )record );
            break;

          case TBIFFRecord.ChartAreaFormat:
            //m_areaFormat = ( ChartAreaFormatRecord )record;
            m_interior = new ChartInteriorImpl( Application, this, ( ChartAreaFormatRecord )record );
            break;

          default:
            //throw new ApplicationException( "Unknown record." );
            //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, record.TypeCode, "Unknown drop bar record" );
            break;
        }

        iPos++;
      }

      iPos--;
    }
    #endregion

    #region Serialize methods
    /// <summary>
    /// Serializes DropBar.
    /// </summary>
    /// <param name="records">OffsetArrayList that will receive all records.</param>
    [ CLSCompliant( false ) ]
    public void Serialize( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentException( "records" );

      if( m_dropBar == null )
        throw new ApplicationException( "Exception occured inside of ChartDropBar object." );

      records.Add( ( BiffRecordRaw )m_dropBar.Clone() );

      records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.Begin ) );

      if( m_lineFormat != null )
        m_lineFormat.Serialize( records );

      if( m_interior != null && !m_interior.UseAutomaticFormat )
        m_interior.Serialize( records );

      records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.End ) );
    }
    #endregion

    #region IChartDropBar properties
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
        /// Represents the Shadow.Read-only
        /// </summary>
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
        /// Gets a value indicating whether this instance has shadow properties.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has shadow properties; otherwise, <c>false</c>.
        /// </value>
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
    /// This property indicates whether border formatting object was created. Read-only.
    /// </summary>
    public bool HasLineProperties
    {
      get
      {
        return m_lineFormat != null;
      }
    }
    /// <summary>
    /// Drop bar gap width.
    /// </summary>
    public int Gap
    {
      get
      {
        return m_dropBar.Gap;
      }
      set
      {
        m_dropBar.Gap = ( ushort )value;
      }
    }
    ///// <summary>
    ///// Foreground color (RGB).
    ///// </summary>
    //public Color    ForegroundColor
    //{
    //  get
    //  {
    //    return m_parentBook.GetPaletteColor( ForegroundColorIndex );
    //  }
    //  set
    //  {
    //    ForegroundColorIndex = m_parentBook.GetNearestColor( value );
    //  }
    //}
    ///// <summary>
    ///// Background color (RGB).
    ///// </summary>
    //public Color    BackgroundColor
    //{
    //  get
    //  {
    //    return AreaFormat.BackgroundColor;
    //  }
    //  set
    //  {
    //    AreaFormat.BackgroundColor = value;
    //  }
    //}
    ///// <summary>
    ///// Pattern.
    ///// </summary>
    //public ExcelPattern Pattern
    //{
    //  get
    //  {
    //    return AreaFormat.Pattern;
    //  }
    //  set
    //  {
    //    AreaFormat.Pattern = value;
    //  }
    //}
    ///// <summary>
    ///// Foreground color index.
    ///// </summary>
    //public ExcelKnownColors ForegroundColorIndex
    //{
    //  get
    //  {
    //    return AreaFormat.ForegroundColorIndex;
    //  }
    //  set
    //  {
    //    AreaFormat.ForegroundColorIndex = value;
    //  }
    //}
    ///// <summary>
    ///// Background color index.
    ///// </summary>
    //public ExcelKnownColors BackgroundColorIndex
    //{
    //  get
    //  {
    //    return AreaFormat.BackgroundColorIndex;
    //  }
    //  set
    //  {
    //    AreaFormat.BackgroundColorIndex = value;
    //  }
    //}
    ///// <summary>
    ///// Automatic format or not.
    ///// </summary>
    //public bool   UseAutomaticFormat
    //{
    //  get
    //  {
    //    return AreaFormat.UseAutomaticFormat;
    //  }
    //  set
    //  {
    //    AreaFormat.UseAutomaticFormat = value;
    //  }
    //}
    ///// <summary>
    ///// Foreground and background are swapped when the data value is negative.
    ///// </summary>
    //public bool   SwapColorsOnNegative
    //{
    //  get
    //  {
    //    return AreaFormat.SwapColorsOnNegative;
    //  }
    //  set
    //  {
    //    AreaFormat.SwapColorsOnNegative = value;
    //  }
    //}
    /// <summary>
    /// Returns interior object. Read-only.
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
    /// Returns object, that represents line properties. Read-only.
    /// </summary>
    public IChartBorder LineProperties
    {
      get
      {
        if( m_lineFormat == null )
          m_lineFormat = new ChartBorderImpl( Application, this );

        return m_lineFormat;
      }
    }
    /// <summary>
    /// Represents fill options. Read-only.
    /// </summary>
    public IFill Fill
    {
      get
      {
        if( m_fill == null )
          m_fill = new ChartFillImpl( Application, this );

        return m_fill;
      }
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

    #region Class helper methods
    /// <summary>
    /// Clones current object.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <returns>Returns cloned object.</returns>
    public ChartDropBarImpl Clone( object parent )
    {
      ChartDropBarImpl result = ( ChartDropBarImpl )MemberwiseClone();

      result.SetParent( parent );
      result.SetParents();

      result.m_dropBar = ( ChartDropBarRecord )CloneUtils.CloneCloneable( m_dropBar );
      result.m_lineFormat = ( ChartBorderImpl )CloneUtils.CloneCloneable( m_lineFormat, this );
      result.m_interior = ( ChartInteriorImpl )CloneUtils.CloneCloneable( m_interior, this );

      return result;
    }
    #endregion
  }
}
