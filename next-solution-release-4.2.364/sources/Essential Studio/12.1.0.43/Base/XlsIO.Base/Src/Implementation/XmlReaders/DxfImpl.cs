#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;

using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Implementation.XmlSerialization;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Parser.Biff_Records;

namespace Syncfusion.XlsIO.Implementation.XmlReaders
{
  /// <summary>
  /// Summary description for Dxf style implementation.
  /// </summary>
  public class DxfImpl
  {
    #region Class members
    /// <summary>
    /// Borders collection.
    /// </summary>
    BordersCollection m_borders;
    /// <summary>
    /// Fill implementation.
    /// </summary>
    FillImpl m_fill;
    /// <summary>
    /// Font implementation.
    /// </summary>
    FontImpl m_font;
    /// <summary>
    /// Format implementation.
    /// </summary>
    FormatImpl m_format;
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes new instance of the Dxf style.
    /// </summary>
    public DxfImpl()
    {
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets/sets Format record.
    /// </summary>
    public FormatImpl FormatRecord
    {
        get
        {
            return m_format;
        }
        set
        {
            m_format = value;
        }
    }
    /// <summary>
    /// Gets/sets fill.
    /// </summary>
    public FillImpl Fill
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
    /// Gets/sets font.
    /// </summary>
    public FontImpl Font
    {
      get
      {
        return m_font;
      }
      set
      {
        m_font = value;
      }
    }
    /// <summary>
    /// Gets/sets borders.
    /// </summary>
    public BordersCollection Borders
    {
      get
      {
        return m_borders;
      }
      set
      {
        m_borders = value;
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Copies all dxf style settings into conditional format.
    /// </summary>
    /// <param name="conFormat">Conditional format to copy data into.</param>
    internal void FillCondition( IInternalConditionalFormat conFormat )
    {
      if (m_format != null)
      {
          conFormat.NumberFormat = m_format.FormatString;
      }
      if( m_fill != null )
      {
        conFormat.FillPattern = m_fill.Pattern;
        conFormat.BackColorObject.CopyFrom( m_fill.PatternColorObject, true );
        conFormat.ColorObject.CopyFrom( m_fill.ColorObject, true );
        conFormat.IsPatternFormatPresent = true;
        conFormat.IsPatternStyleModified = true;
      }

      if( m_font != null )
      {
        if( ( int )m_font.Color != FontRecord.DefaultFontColor )
        {
          conFormat.FontColorObject.CopyFrom( m_font.ColorObject, true );
        }
        
        conFormat.IsBold = m_font.Bold;
        conFormat.IsItalic = m_font.Italic;
        conFormat.IsStrikeThrough = m_font.Strikethrough;
        conFormat.IsSubScript = m_font.Subscript;
        conFormat.IsSuperScript = m_font.Superscript;
        conFormat.Underline = m_font.Underline;
      }

      if( m_borders != null )
      {
        BorderSettingsHolder borderSeetings = ( BorderSettingsHolder )m_borders[ ExcelBordersIndex.EdgeBottom ];
        
        if( borderSeetings != null )
        {
          conFormat.BottomBorderColorObject.CopyFrom( borderSeetings.ColorObject, true );
          conFormat.BottomBorderStyle = borderSeetings.LineStyle;
        }

        borderSeetings = ( BorderSettingsHolder )m_borders[ ExcelBordersIndex.EdgeLeft ];

        if( borderSeetings != null )
        {
          conFormat.LeftBorderColorObject.CopyFrom( borderSeetings.ColorObject, true );
          conFormat.LeftBorderStyle = borderSeetings.LineStyle;
        }

        borderSeetings = ( BorderSettingsHolder )m_borders[ ExcelBordersIndex.EdgeRight ];

        if( borderSeetings != null )
        {
          conFormat.RightBorderColorObject.CopyFrom( borderSeetings.ColorObject, true );
          conFormat.RightBorderStyle = borderSeetings.LineStyle;
        }

        borderSeetings = ( BorderSettingsHolder )m_borders[ ExcelBordersIndex.EdgeTop ];

        if( borderSeetings != null )
        {
          conFormat.TopBorderColorObject.CopyFrom( borderSeetings.ColorObject, true );
          conFormat.TopBorderStyle = borderSeetings.LineStyle;
        }
      }
    }
    /// <summary>
    /// Creates copy of the current object.
    /// </summary>
    /// <returns>A copy of the current object.</returns>
    public DxfImpl Clone( WorkbookImpl book )
    {
      DxfImpl result = ( DxfImpl )MemberwiseClone();

      result.m_borders = ( BordersCollection )m_borders.Clone( book );
      result.m_fill = m_fill.Clone();
      result.m_font = ( FontImpl )m_font.Clone( book );
      result.m_format = (FormatImpl)m_format.Clone(book);

      return result;
    }
    #endregion
  }
}
