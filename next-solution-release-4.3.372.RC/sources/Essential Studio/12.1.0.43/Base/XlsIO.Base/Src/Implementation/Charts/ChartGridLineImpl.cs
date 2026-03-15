#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections;

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
  /// Represents chart grid lines.
  /// </summary>
  public class ChartGridLineImpl
    : CommonObject
    , IChartGridLine
	{
    #region Class constants
    /// <summary>
    /// Represents default color index.
    /// </summary>
    private const ExcelKnownColors DEF_COLOR_INEDX = ( ExcelKnownColors )77;
    #endregion

    #region Class members
    /// <summary>
    /// Represents axis line record.
    /// </summary>
    private ChartAxisLineFormatRecord m_axisLine;
    /// <summary>
    /// Represents parent axis.
    /// </summary>
    private ChartAxisImpl m_parentAxis;
    /// <summary>
    /// Represents Shadow
    /// </summary>
    private ShadowImpl m_shadow;
    /// <summary>
    /// Represents parent book.
    /// </summary>
    protected WorkbookImpl m_parentBook;
    /// <summary>
    /// Represents chart border.
    /// </summary>
    private ChartBorderImpl m_border;
    /// <summary>
    /// Represents the 3D features
    /// </summary>
    private ThreeDFormatImpl m_3D;
    #endregion

    #region Class initialize methods
    /// <summary>
    /// Creates ChartGridLine object.
    /// </summary>
    /// <param name="application">Application object.</param>
    /// <param name="parent">Parent object.</param>
    /// <param name="axisType">Represents type of axisline record.</param>
    public ChartGridLineImpl( IApplication application, object parent, ExcelAxisLineIdentifier axisType )
      : base( application, parent )
    {
      if( axisType != ExcelAxisLineIdentifier.MajorGridLine
        && axisType != ExcelAxisLineIdentifier.MinorGridLine )
      {
        throw new ArgumentException( "axisType" );
      }

      m_axisLine = ( ChartAxisLineFormatRecord )
        BiffRecordFactory.GetRecord( TBIFFRecord.ChartAxisLineFormat );

      m_border = new ChartBorderImpl( application, this );

      m_border.ColorIndex = DEF_COLOR_INEDX;
      m_border.LineWeight = ExcelChartLineWeight.Hairline;

      AxisLineType = axisType;
      m_border.AutoFormat = true;

      SetParents();
    }
    /// <summary>
    /// Creates ChartGridLine object. Only for parsing.
    /// </summary>
    /// <param name="application">Application object.</param>
    /// <param name="parent">Parent object.</param>
    /// <param name="data">Represents records storage.</param>
    /// <param name="iPos">Represents position in storage.</param>
    public ChartGridLineImpl( IApplication application, object parent, IList<BiffRecordRaw> data, ref int iPos )
      : base( application, parent )
    {
      Parse( data, ref iPos );

      SetParents();
    }
    /// <summary>
    /// Finds all parent objects.
    /// </summary>
    private void SetParents()
    {
      m_parentAxis = ( ChartAxisImpl )FindParent( Parent, typeof( ChartAxisImpl ), true );
      m_parentBook = ( WorkbookImpl )FindParent( typeof( WorkbookImpl ) );

      if( m_parentBook == null )
        throw new ApplicationException( "Can't find parent objects" );
    }
    #endregion

    #region Class parse methods
    /// <summary>
    /// Parses GridLine records.
    /// </summary>
    /// <param name="data">Offset array list.</param>
    /// <param name="iPos">Current position in offset array list.</param>
    [ CLSCompliant( false ) ]
    public virtual void Parse( IList<BiffRecordRaw> data, ref int iPos )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      BiffRecordRaw record = ( BiffRecordRaw )data[ iPos ];
      record.CheckTypeCode( TBIFFRecord.ChartAxisLineFormat );

      m_axisLine = ( ChartAxisLineFormatRecord )record;
      iPos++;

      record = ( BiffRecordRaw )data[ iPos ];

      if( record.TypeCode == TBIFFRecord.ChartLineFormat )
      {
        m_border = new ChartBorderImpl( Application, this, data, ref iPos );
      }
    }
    #endregion

    #region Class serialize methods
    /// <summary>
    /// Serializes Grid lines.
    /// </summary>
    /// <param name="records">OffsetArrayList that will receive all records.</param>
    [ CLSCompliant( false ) ]
    public virtual void Serialize( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      if( m_axisLine == null )
        return;

      records.Add( ( BiffRecordRaw )m_axisLine.Clone() );

      m_border.Serialize( records );
    }
    #endregion

    #region IChartGridLine properties
    /// <summary>
    /// Gets line border. Read-only.
    /// </summary>
    public IChartBorder Border
    {
      get
      {
        if( m_border == null )
          m_border = new ChartBorderImpl( Application, this );

        m_border.HasLineProperties = true;
        return m_border;
      }
    }
    /// <summary>
    /// Gets line border. Read-only.
    /// </summary>
    public IChartBorder LineProperties
    {
      get
      {
        return Border;
      }
    }
    /// <summary>
    /// This property indicates whether line formatting object was created. Read-only.
    /// </summary>
    public bool HasLineProperties
    {
      get
      {
        return m_border != null;
      }
    }
    /// <summary>
    /// Represents the Shadow.Read-only
    /// </summary>
    public IShadow Shadow
    {
      get
      {

        if( m_shadow == null )
          m_shadow = new ShadowImpl( Application, this );
        //if (m_shadow.HasCustomShadowStyle == true)
        //    throw new NotSupportedException("It is not supported when Custom Shadow style is set to true");

        return m_shadow;
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
        if( value )
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
        if( m_3D == null )
          m_3D = new ThreeDFormatImpl( Application, this );

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
        if( value )
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
    /// This property indicates whether interior object was created. Read-only.
    /// </summary>
    public bool HasInterior
    {
      get
      {
        return false;
      }
    }
    /// <summary>
    /// Returns object, that represents area properties. Read-only.
    /// </summary>
    public IChartInterior Interior
    {
      get
      {
        throw new NotSupportedException();
      }
    }
    /// <summary>
    /// Represents fill options. Read-only.
    /// </summary>
    public IFill Fill
    {
      get
      {
        throw new NotSupportedException();
      }
    }
    #endregion

    #region IChartGridLine methods
    /// <summary>
    /// Clears current GridLines.
    /// </summary>
    public virtual void Delete()
    {
      if( m_axisLine.LineIdentifier == ExcelAxisLineIdentifier.MajorGridLine )
      {
        m_parentAxis.HasMajorGridLines = false;
      }
      else
      {
        m_parentAxis.HasMinorGridLines = false;
      }
    }
    #endregion

    #region Class helper properties
    /// <summary>
    /// Gets or sets axis line type.
    /// </summary>
    public ExcelAxisLineIdentifier AxisLineType
    {
      get
      {
        return m_axisLine.LineIdentifier;
      }
      set
      {
        m_axisLine.LineIdentifier = value;
      }
    }
    /// <summary>
    /// Returns parent axis. Read-only.
    /// </summary>
    protected ChartAxisImpl ParentAxis
    {
      get
      {
        return m_parentAxis;
      }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Clones current object.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <returns>Returns cloned object.</returns>
    public virtual object Clone( object parent )
    {
      ChartGridLineImpl result = ( ChartGridLineImpl )MemberwiseClone();
      result.SetParent( parent );
      result.SetParents();

      result.m_axisLine = ( ChartAxisLineFormatRecord )CloneUtils.CloneCloneable( m_axisLine );

      if( m_border != null )
        result.m_border = m_border.Clone( result );

      return result;
    }
    #endregion
  }
}
