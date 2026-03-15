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
using System.Collections.Specialized;
using System.IO;
using System.Diagnostics;
using System.Text;

using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Interfaces;
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
#else
using System.Drawing;
#endif
#endregion

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// Represents the border of an object.
  /// </summary>
  public class BorderImpl
    : CommonObject
    , IBorder,IDisposable
  {
    #region skipped
#if NOT_SUPPORTED
        /// <summary>
    /// Returns or sets the color of the border. The color is specified as an
    /// index value into the current color palette, or as one of the
    /// following ExcelColorIndex constants. Read/write ushort.
    /// </summary>
    public ExcelColorIndex   ColorIndex
    {
      get
      {
        UpdateInternalFields();
        return m_usColorIndex;
      }
      set
      {
        m_usColorIndex = value;
        UpdateExtFormatFields();
      }
    }
#endif
    #endregion

    #region Class constants
    /// <summary>
    /// Maximum color index that requires modification in order to let MS Excel edit styles.
    /// </summary>
    public const int DEF_MAXBADCOLOR = 8;
    /// <summary>
    /// Increment for color value in order to let MS Excel edit styles.
    /// </summary>
    public const int DEF_BADCOLOR_INCREMENT = 64;
    #endregion

    #region Class members
    /// <summary>
    /// Index of the border.
    /// </summary>
    private ExcelBordersIndex  m_border;
    /// <summary>
    /// Extended format that contains this border.
    /// </summary>
    private IInternalExtendedFormat m_format;
    #endregion

    #region IBorder Members
    /// <summary>
    /// Returns or sets the primary color of the object, as shown in the
    /// following table. Use the RGB function to create a color value.
    /// Read/write Long.
    /// </summary>
    public ExcelKnownColors Color
    {
      get
      {
        return ColorObject.GetIndexed( m_format.Workbook );
      }
      set
      {
        value = NormalizeColor( value );
        m_format.BeginUpdate();
        ColorObject.SetIndexed( value );
        m_format.EndUpdate();
      }
    }

    /// <summary>
    /// Returns or sets the primary color of the object, as shown in the
    /// following table. Use the RGB function to create a color value.
    /// Read/write Long.
    /// </summary>
    public ColorObject ColorObject
    {
      get
      {
        switch( m_border )
        {
          case ExcelBordersIndex.EdgeTop:      return m_format.TopBorderColor;
          case ExcelBordersIndex.EdgeBottom:   return m_format.BottomBorderColor;
          case ExcelBordersIndex.EdgeLeft:     return m_format.LeftBorderColor;
          case ExcelBordersIndex.EdgeRight:    return m_format.RightBorderColor;
          case ExcelBordersIndex.DiagonalDown: return m_format.DiagonalBorderColor;
          case ExcelBordersIndex.DiagonalUp:   return m_format.DiagonalBorderColor;

          default:
            throw new ArgumentOutOfRangeException( "Border index" );
        }
      }
    }
    /// <summary>
    /// Returns or sets the primary color of the object, as shown in the
    /// following table. Use the RGB function to create a color value.
    /// Read/write Color.
    /// </summary>
    public Color              ColorRGB
    {
      get
      {
        return ColorObject.GetRGB( Workbook );
      }
      set
      {
        if ((m_format as CellStyle) != null)
            (m_format as CellStyle).AskAdjacent = false;
        m_format.BeginUpdate();
        ColorObject.SetRGB( value, Workbook );
        m_format.EndUpdate();
      }
    }

    /// <summary>
    /// Returns or sets the line style for the border. Read/write ExcelLineStyle.
    /// </summary>
    public ExcelLineStyle     LineStyle
    {
      get
      {
        switch( m_border )
        {
          case ExcelBordersIndex.EdgeTop:       return m_format.TopBorderLineStyle;
          case ExcelBordersIndex.EdgeBottom:    return m_format.BottomBorderLineStyle;
          case ExcelBordersIndex.EdgeLeft:      return m_format.LeftBorderLineStyle;
          case ExcelBordersIndex.EdgeRight:     return m_format.RightBorderLineStyle;
          case ExcelBordersIndex.DiagonalDown:  return m_format.DiagonalDownBorderLineStyle;
          case ExcelBordersIndex.DiagonalUp:    return m_format.DiagonalUpBorderLineStyle;

          default:
            throw new ArgumentOutOfRangeException( "Border index" );
        }
      }
      set
      {
        switch( m_border )
        {
          case ExcelBordersIndex.EdgeTop:
            m_format.TopBorderLineStyle = value;
            break;

          case ExcelBordersIndex.EdgeBottom:
            m_format.BottomBorderLineStyle = value;
            break;

          case ExcelBordersIndex.EdgeLeft:
            m_format.LeftBorderLineStyle = value;
            break;

          case ExcelBordersIndex.EdgeRight:
            m_format.RightBorderLineStyle = value;
            break;

          case ExcelBordersIndex.DiagonalDown:
            m_format.DiagonalDownBorderLineStyle = value;
            break;

          case ExcelBordersIndex.DiagonalUp:
            m_format.DiagonalUpBorderLineStyle = value;
            break;

          default:
            throw new ArgumentOutOfRangeException( "Border index" );
        }

        //NormalizeColor();
      }
    }

    /// <summary>
    /// This property is used only by Diagonal borders. For any other border
    /// index property will have no influence.
    /// </summary>
    public bool               ShowDiagonalLine
    {
      get
      {
        switch( m_border )
        {
          case ExcelBordersIndex.DiagonalDown:  return m_format.DiagonalDownVisible;
          case ExcelBordersIndex.DiagonalUp:    return m_format.DiagonalUpVisible;

          default:
            return false;
        }
      }
      set
      {
        switch( m_border )
        {
          case ExcelBordersIndex.DiagonalDown:
            m_format.DiagonalDownVisible = value;
            break;

          case ExcelBordersIndex.DiagonalUp:
            m_format.DiagonalUpVisible = value;
            break;
        }
      }
    }
    /// <summary>
    /// Returns border index. Read-only.
    /// </summary>
    internal ExcelBordersIndex  BorderIndex
    {
      get
      {
        return m_border;
      }
      set
      {
        m_border = value;
      }
    }
    #endregion

    #region Internal properties
    /// <summary>
    /// Returns parent workbook. Read-only.
    /// </summary>
    private WorkbookImpl Workbook
    {
      get
      {
        return m_format.Workbook;
      }
    }
    #endregion

    #region Class Initialize methods
    /// <summary>
    /// Creates a class instance and sets its Application and Parent properties.
    /// </summary>
    /// <param name="application">Application object for the Border.</param>
    /// <param name="parent">Parent object for the Border.</param>
    private BorderImpl( IApplication application, object parent )
      : base( application, parent )
    {
    }
    /// <summary>
    /// Creates border with specified index.
    /// </summary>
    /// <param name="application">Application object for the border.</param>
    /// <param name="parent">Parent object for the border.</param>
    /// <param name="borderIndex">Index of border that should be created.</param>
    private BorderImpl( IApplication application, object parent
      , ExcelBordersIndex borderIndex )
      : this( application, parent )
    {
      m_border = borderIndex;
    }
    /// <summary>
    /// Creates border from ExtendedFormat with specified border index.
    /// </summary>
    /// <param name="application">Application object for the border.</param>
    /// <param name="parent">Parent object for the border.</param>
    /// <param name="impl">
    /// ExtendedFormat that contains all information about required border.
    /// </param>
    /// <param name="borderIndex">Border index of the border.</param>
    public BorderImpl( IApplication application, object parent
      , IInternalExtendedFormat impl, ExcelBordersIndex borderIndex )
      : this( application, parent, borderIndex )
    {
      m_format = impl;
    }
    #endregion

    #region Methods
    /// <summary>
    /// Determines whether the specified Object is equal to the current Object.
    /// </summary>
    /// <param name="obj">The Object to compare with the current Object.</param>
    /// <returns>true if the specified Object is equal to the current Object; otherwise, false.</returns>
    public override bool Equals( object obj )
    {
      BorderImpl border = obj as BorderImpl;

      if( border == null ) return false;

      return border.m_border == m_border &&
        border.ShowDiagonalLine == ShowDiagonalLine &&
        border.LineStyle == LineStyle &&
        border.ColorObject == ColorObject;
    }

    /// <summary>
    /// Serves as a hash function for a particular type, suitable for use
    /// in hashing algorithms and data structures like a hash table.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
      return m_border.GetHashCode() ^
        ShowDiagonalLine.GetHashCode() ^
        LineStyle.GetHashCode() ^
        ColorObject.GetHashCode();
    }

    /// <summary>
    /// Copies all fields from baseBorder except Parent.
    /// </summary>
    /// <param name="baseBorder">Border that will be copied.</param>
    public void CopyFrom( IBorder baseBorder )
    {
      ColorObject.CopyFrom( baseBorder.ColorObject, true );
      LineStyle = baseBorder.LineStyle;
    }
    /// <summary>
    /// Normalizes border color to let MS Excel edit style.
    /// </summary>
    private void NormalizeColor()
    {
      if( LineStyle != ExcelLineStyle.None && ColorObject.ColorType == ColorType.Indexed )
      {
        ColorObject color = ColorObject;
        ExcelKnownColors index = color.GetIndexed( null );
        index = NormalizeColor( index );
        color.SetIndexed( index );
      }
    }
    /// <summary>
    /// Normalizes border color to let MS Excel edit style.
    /// </summary>
    /// <param name="color">Color to normalize.</param>
    /// <returns>New color value.</returns>
    public static ExcelKnownColors NormalizeColor( ExcelKnownColors color )
    {
      int iColor = ( int )color;

      //if( iColor < DEF_MAXBADCOLOR )
      if( iColor == 0 )
      {
        iColor += DEF_BADCOLOR_INCREMENT;
        color = ( ExcelKnownColors )iColor;
      }

      return ( ExcelKnownColors )iColor;
    }
    #endregion

    #region ICloneable members
    /// <summary>
    /// Clone current object.
    /// </summary>
    /// <param name="newFormat">New extended format.</param>
    /// <returns>Cloned border object.</returns>
    public BorderImpl Clone( StyleImpl newFormat )
    {
      BorderImpl result = MemberwiseClone() as BorderImpl;
      result.m_format = newFormat;
      return result;
    }
    #endregion

    #region IDisposable Members

    void IDisposable.Dispose()
    {
        GC.SuppressFinalize(this);
    }

    #endregion

    internal void Clear()
    {
        m_format.TopBorderColor.Dispose();
        m_format.BottomBorderColor.Dispose();
        m_format.LeftBorderColor.Dispose();
        m_format.RightBorderColor.Dispose();
        m_format.DiagonalBorderColor.Dispose();
        m_format.DiagonalBorderColor.Dispose();
    }
  }

  /// <summary>
  /// This class represents border for multicell range.
  /// </summary>
  public class BorderImplArrayWrapper
    : CommonObject
    , IBorder
  {
    #region Class members
    /// <summary>
    /// Cells of the range.
    /// </summary>
    private List<IRange> m_arrCells = new List<IRange>();
    /// <summary>
    /// Border index.
    /// </summary>
    private ExcelBordersIndex  m_border;
    /// <summary>
    /// Parent workbook.
    /// </summary>
    private WorkbookImpl m_book;
    #endregion

    #region Class constructors
    /// <summary>
    /// Creates wrapper for specified range and border index.
    /// </summary>
    /// <param name="range">Range for which this wrapper is created.</param>
    /// <param name="index">Border index.</param>
    public BorderImplArrayWrapper( IRange range, ExcelBordersIndex index )
      : base( range.Application, range )
    {
      m_border = index;
      m_arrCells.AddRange( range.Cells );
      m_book = FindParent( typeof( WorkbookImpl ) ) as WorkbookImpl;

      if( m_book == null )
        throw new ArgumentNullException( "Can't find parent workbook" );
    }
    /// <summary>
    /// Creates wrapper for specified range and border index.
    /// </summary>
    /// <param name="range">Range for which this wrapper is created.</param>
    /// <param name="index">Border index.</param>
    public BorderImplArrayWrapper(List<IRange> lstRange, ExcelBordersIndex index,IApplication application)
        : base(application, lstRange[0])
    {
        m_border = index;
        m_arrCells = lstRange;
        m_book = FindParent(typeof(WorkbookImpl)) as WorkbookImpl;

        if (m_book == null)
            throw new ArgumentNullException("Can't find parent workbook");
    }
    #endregion
    
    #region IBorder Members
    /// <summary>
    /// Returns or sets the primary color of the object.
    /// Read/write ExcelKnownColors.
    /// </summary>
    public ExcelKnownColors Color
    {
      get
      {
        ExcelKnownColors color = m_arrCells[ 0 ].Borders[ m_border ].Color;
        for( int i = 1; i < m_arrCells.Count; i++ )
        {
          if( color != m_arrCells[ i ].Borders[ m_border ].Color )
            return ExcelKnownColors.None;
        }

        return color;
      }
      set
      {
        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          IRange cell = m_arrCells[ i ];
          cell.Borders[ m_border ].Color = value;
        }
      }
    }
    /// <summary>
    /// Returns or sets the primary color of the object, as shown in the
    /// following table. Use the RGB function to create a color value.
    /// </summary>
    public ColorObject ColorObject
    {
      get
      {
        ColorObject color = m_arrCells[ 0 ].Borders[ m_border ].ColorObject;

        for( int i = 1; i < m_arrCells.Count; i++ )
        {
          if( color != m_arrCells[ i ].Borders[ m_border ].ColorObject )
            return null;
        }

        return color;
      }
    }
    /// <summary>
    /// Returns or sets the primary color of the object, as shown in the
    /// following table. Use the RGB function to create a color value.
    /// Read/write Long.
    /// </summary>
    public Color            ColorRGB
    {
      get
      {
        if (ColorObject == null)
            return m_arrCells[ 0 ].Borders[ m_border ].ColorObject.GetRGB(m_book);

        return ColorObject.GetRGB( m_book );
      }
      set
      {
        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          // Use 'as' to increase performance.
          IRange cell = m_arrCells[ i ] as IRange;
          cell.Borders[ m_border ].ColorRGB = value;
        }
        //ColorObject.SetRGB( value, m_book );
      }
    }
    /// <summary>
    /// Returns or sets the line style for the border. Read/write ExcelLineStyle.
    /// </summary>
    public ExcelLineStyle   LineStyle
    {
      get
      {
        ExcelLineStyle line = m_arrCells[ 0 ].Borders[ m_border ].LineStyle;
        
        for( int i = 1; i < m_arrCells.Count; i++ )
        {
          if( line != m_arrCells[ i ].Borders[ m_border ].LineStyle )
            return ExcelLineStyle.None;
        }
        
        return line;
      }
      set
      {
        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          // Use 'as' to increase performance.
          IRange cell = m_arrCells[ i ] as IRange;
          cell.Borders[ m_border ].LineStyle = value;
        }
      }
    }

    /// <summary>
    /// This property is used only by Diagonal borders. For any other border
    /// index property will have no influence.
    /// </summary>
    public bool             ShowDiagonalLine
    {
      get
      {
        bool line = m_arrCells[ 0 ].Borders[ m_border ].ShowDiagonalLine;
        
        for( int i = 1; i < m_arrCells.Count; i++ )
        {
          if( line != m_arrCells[ i ].Borders[ m_border ].ShowDiagonalLine )
            return false;
        }
        
        return line;
      }
      set
      {
        for( int i = 0, len = m_arrCells.Count; i < len; i++ )
        {
          // Use 'as' to increase performance.
          IRange range = m_arrCells[ i ] as IRange;
          range.Borders[ m_border ].ShowDiagonalLine = value;
        }
      }
    }
    #endregion
  }
}
