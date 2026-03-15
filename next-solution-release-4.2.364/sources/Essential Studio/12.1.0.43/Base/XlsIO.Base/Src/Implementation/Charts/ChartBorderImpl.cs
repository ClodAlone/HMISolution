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
using Syncfusion.XlsIO.Implementation.Shapes;

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
using Syncfusion.XlsIO.Implementation.Exceptions;
#endif

#if  SILVERLIGHT
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
using Syncfusion.XlsIO.Implementation.Exceptions;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
using Syncfusion.XlsIO.Implementation.Exceptions;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif

namespace Syncfusion.XlsIO.Implementation.Charts
{
  /// <summary>
  /// Represents chart border object.
  /// </summary>
  public class ChartBorderImpl
    : CommonObject
    , IChartBorder
    , ICloneParent
  {
    #region Class constants
    /// <summary>
    /// Represents default color index.
    /// </summary>
    private const ExcelKnownColors DEF_COLOR_INEDX = ( ExcelKnownColors )77;
    #endregion

    #region Class members
    /// <summary>
    /// Line format.
    /// </summary>
    private ChartLineFormatRecord m_lineFormat = ( ChartLineFormatRecord )
      BiffRecordFactory.GetRecord( TBIFFRecord.ChartLineFormat );
    /// <summary>
    /// Represents parent workbook.
    /// </summary>
    private WorkbookImpl m_parentBook;
    /// <summary>
    /// Represents parent Series format.
    /// </summary>
    private ChartSerieDataFormatImpl m_serieFormat;
    /// <summary>
    /// Represents line color.
    /// </summary>
    private ColorObject m_color;
    /// <summary>
    /// Represents the default transparency level for the solid type
    /// </summary>
    private double m_solidTransparency = 0.0;
    /// <summary>
    /// Preserve Gradient Stops (Supported in Excel 2007 and higher)
    /// </summary>
    private IInternalFill m_fill;
    /// <summary>
    /// border edge join type
    /// </summary>
    private Excel2007BorderJoinType m_joinType;  
    private string m_lineWeightString;
    private bool m_lineProperties;
        
    #endregion

    #region Class initialize methods
    /// <summary>
    /// Creates new instance of class.
    /// </summary>
    /// <param name="application">Represents current application.</param>
    /// <param name="parent">Represents parent object.</param>
    public ChartBorderImpl( IApplication application, object parent )
      : base( application, parent )
    {
      m_lineFormat = ( ChartLineFormatRecord )
        BiffRecordFactory.GetRecord( TBIFFRecord.ChartLineFormat );

      Fill = new ShapeFillImpl(application, parent);
      SetParents();
    }
    /// <summary>
    /// Creates new instance of class.
    /// </summary>
    /// <param name="application">Represents current application.</param>
    /// <param name="parent">Represents parent object.</param>
    /// <param name="line">Represents line record.</param>
    [ CLSCompliant( false ) ]
    public ChartBorderImpl( IApplication application, object parent, ChartLineFormatRecord line )
      : base( application, parent )
    {
      if( line == null )
        throw new ArgumentNullException( "line" );

      m_lineFormat = line;
      SetParents();
    }
    /// <summary>
    /// Creates new instance of class.
    /// </summary>
    /// <param name="application">Represents current application.</param>
    /// <param name="parent">Represents parent object.</param>
    /// <param name="data">Represents record storage.</param>
    /// <param name="iPos">Represents position in storage.</param>
    public ChartBorderImpl( IApplication application, object parent, IList<BiffRecordRaw> data, ref int iPos )
      : base( application, parent )
    {
      Parse( data, ref iPos );
      SetParents();
    }
    #endregion

    #region Parse\Serialize methods
    /// <summary>
    /// Parsing current object.
    /// </summary>
    /// <param name="data">Records offset.</param>
    /// <param name="iPos">Position in offset.</param>
    public void Parse( IList<BiffRecordRaw> data, ref int iPos )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      BiffRecordRaw record = ( BiffRecordRaw )data[ iPos ];
      record.CheckTypeCode( TBIFFRecord.ChartLineFormat );

      m_lineFormat = ( ChartLineFormatRecord )record;

      iPos++;
    }
    /// <summary>
    /// Serialize current object.
    /// </summary>
    /// <param name="records">Records offset.</param>
    public void Serialize( IList<IBiffStorage> records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      if( m_lineFormat != null )
      {
        UpdateColor();
        records.Add( ( IBiffStorage )m_lineFormat.Clone() );
      }
    }
    /// <summary>
    /// Finds parent objects.
    /// </summary>
    private void SetParents()
    {
      m_parentBook = ( WorkbookImpl )FindParent( typeof( WorkbookImpl ) );
      m_serieFormat = ( ChartSerieDataFormatImpl )FindParent( typeof( ChartSerieDataFormatImpl ) );

      if( m_parentBook == null )
        throw new ApplicationException( "cannot find parent objects." );

      m_color = new ColorObject( ( ExcelKnownColors )m_lineFormat.ColorIndex );
      m_color.AfterChange += UpdateColor;
    }
    /// <summary>
    /// Updates internal record for Excel97 file format.
    /// </summary>
    private void UpdateColor()
    {
      m_lineFormat.ColorIndex = ( ushort )m_color.GetIndexed( m_parentBook );
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Color of line.
    /// </summary>
    public Color LineColor
    {
      get
      {
        return m_color.GetRGB( m_parentBook );
      }
      set
      {
        if( m_color.ColorType != ColorType.RGB || value.ToArgb() != m_color.Value || AutoFormat )
        {
          AutoFormat = false;
          m_color.SetRGB( value, m_parentBook );
          m_lineFormat.IsAutoLineColor = false;
          HasLineProperties = true;

          if( m_serieFormat != null )
            m_serieFormat.ClearOnPropertyChange();
          //ColorIndex = m_parentBook.GetNearestColor( value, BorderImpl.DEF_MAXBADCOLOR );
        }
      }
    }
    /// <summary>
    /// Line pattern.
    /// </summary>
    public ExcelChartLinePattern LinePattern
    {
      get
      {
        return m_lineFormat.LinePattern;
      }
      set
      {
        if( value != LinePattern || AutoFormat )
        {
          m_lineFormat.LinePattern = value;
          AutoFormat = false;

          if( m_serieFormat != null )
            m_serieFormat.ClearOnPropertyChange();

          HasLineProperties = true;
        }
      }
    }
    /// <summary>
    /// Weight of line.
    /// </summary>
    public ExcelChartLineWeight LineWeight
    {
      get
      {
        return m_lineFormat.LineWeight;
      }
      set
      {
        if( value != LineWeight || AutoFormat )
        {
          m_lineFormat.LineWeight = value;
          AutoFormat = false;

          if( m_serieFormat != null )
            m_serieFormat.ClearOnPropertyChange();
        }
      }
    }
    /// <summary>
    /// preserve fill and gradient stops (Supported in Excel 2007 and higher)
    /// </summary>
   internal IInternalFill Fill
    {
        get
        {
            return m_fill;
        }
        set
        {
            m_fill = value;
        }
    }
    /// <summary>
    /// indicates whether the line filled with gradient fill
    /// </summary>
   internal bool HasGradientFill
   {
       get
       {
         return m_fill != null && m_fill.FillType == ExcelFillType.Gradient;
       }
   }
   internal bool HasLineProperties
   {
       get
       {
           return m_lineProperties;

       }
       set
       {
           m_lineProperties = value;
       }
   }
    /// <summary>
    /// Border edge join type
    /// </summary>
   internal Excel2007BorderJoinType JoinType
   {
       get
       {
           return m_joinType;
       }
       set
       {
           m_joinType = value;
       }
   }
    /// <summary>
    /// If true - default format; otherwise custom.
    /// </summary>
    public bool AutoFormat
    {
      get
      {
        return m_lineFormat.AutoFormat;
      }
      set
      {
        if( AutoFormat != value )
        {
          m_lineFormat.AutoFormat = value;

          if( value )
          {
            m_lineFormat.LineWeight = ExcelChartLineWeight.Hairline;
            m_lineFormat.LinePattern = ExcelChartLinePattern.Solid;
            IsAutoLineColor = true; 
          }
          //for line, skater, radar chart types updates color in Series data format records.
          else if( m_serieFormat != null && !m_serieFormat.ParentChart.TypeChanging )
          {
            int result = m_serieFormat.UpdateLineColor();

            if( result != -1 )
            {
              m_lineFormat.ColorIndex = ( ushort )result;
              m_lineFormat.IsAutoLineColor = false;
            }
          }

          if( m_serieFormat != null && !m_serieFormat.ParentChart.TypeChanging )
            m_serieFormat.ClearOnPropertyChange();
        }
      }
    }
    /// <summary>
    /// True to draw tick labels on this axis.
    /// </summary>
    public bool DrawTickLabels
    {
      get
      {
        return m_lineFormat.DrawTickLabels;
      }
      set
      {
        m_lineFormat.DrawTickLabels = value;
      }
    }
    /// <summary>
    /// Custom format for line color.
    /// </summary>
    public bool IsAutoLineColor
    {
      get
      {
        return m_lineFormat.IsAutoLineColor;
      }
      set
      {
        m_lineFormat.IsAutoLineColor = value;

        if( value )
        {
          m_lineFormat.ColorIndex = ( ushort )DEF_COLOR_INEDX;
        }

        if( m_serieFormat != null )
          m_serieFormat.ClearOnPropertyChange();
      }
    }
    /// <summary>
    /// Line color index.
    /// </summary>
    public ExcelKnownColors ColorIndex
    {
      get
      {
        return m_color.GetIndexed( m_parentBook );
      }
      set
      {
        if( m_color.ColorType != ColorType.Indexed || ColorIndex != value || AutoFormat )
        {
          value = ChartFrameFormatImpl.UpdateLineColor( value );

          AutoFormat = false;
          m_color.SetIndexed( value );
          m_lineFormat.IsAutoLineColor = false;

          if( m_serieFormat != null )
            m_serieFormat.ClearOnPropertyChange();
        }
      }
    }
    /// <summary>
    /// Returns border color object. Read-only.
    /// </summary>
    public ColorObject Color
    {
      get
      {
        return m_color;
        // NOTE - update event.
      }
    }
    /// <summary>
    /// Returns the transparency level of the specified Solid color shaded fill as a floating-point
    /// value from 0.0 (light) through 1.0(dark)
    /// </summary>
    public double Transparency
    {
      get
      {
        return m_solidTransparency;
      }
      set
      {
        if( value < 0 || value > 1 )
          throw new ArgumentOutOfRangeException( "Transparency is out of range" );

        m_solidTransparency = value;
      }
    }
    /// <summary>
    /// Gets or sets the line weight string.
    /// </summary>
    /// <value>The line weight string.</value>
    internal string LineWeightString
    {
        get
        {
            return m_lineWeightString;
        }
        set
        {
            m_lineWeightString = value;
      }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Clones current object.
    /// </summary>
    /// <param name="parent">Represents parent object.</param>
    /// <returns>Returns cloned object.</returns>
    public ChartBorderImpl Clone( object parent )
    {
      if( parent == null )
        throw new ArgumentNullException( "parent" );

      ChartBorderImpl result = ( ChartBorderImpl )MemberwiseClone();

      result.m_lineFormat = ( ChartLineFormatRecord )CloneUtils.CloneCloneable( m_lineFormat );

      
      result.SetParent( parent );
      result.SetParents();
      result.m_color = m_color.Clone();

      return result;
    }

    internal void ClearAutoColor()
    {
      IsAutoLineColor = false;
    }
    #endregion

    #region ICloneParent Members
    /// <summary>
    ///  Makes complete copy of the current object and updates its parent.
    /// </summary>
    /// <param name="parent">Parent object to set.</param>
    /// <returns>Cloned object.</returns>
    object ICloneParent.Clone( object parent )
    {
      return Clone( parent );
    }

    #endregion
  }
}
