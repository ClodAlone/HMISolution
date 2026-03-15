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
using System.ComponentModel;

using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Interfaces.Collections;
using Syncfusion.XlsIO.Parser.Biff_Records;
#endregion

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  ///
  /// </summary>
  public class CellFormatImpl
    : CommonObject
    , ICellFormat
    , IFont
  {
    #region Skipped
#if SKIPPED
    /// <summary>
    ///
    /// </summary>
    public object   MergeCells
    {
      get
      {
        // TODO:  Add CellFormatImpl.MergeCells getter implementation.
        throw new NotImplementedException();
      }
      set
      {
        // TODO:  Add CellFormatImpl.MergeCells setter implementation.
        throw new NotImplementedException();
      }
    }
    /// <summary>
    ///
    /// </summary>
    public void Clear()
    {
      // TODO:  Add CellFormatImpl.Clear implementation.
      throw new NotImplementedException();
    }
#endif
    #endregion

    #region Class members
    private ExtendedFormatImpl m_ExtFormat;
    private WorkbookImpl m_book;
    private int m_iExtFormatIndex;

    private object    m_objAddIndent = null;
    private IBorders  m_borders = null;
    private IFont     m_font = null;
//    private bool      m_bLocked;
    private string    m_strNumFormat;
    private string    m_strNumFormatLocal;
//    private double    m_dOrientation;
//    private bool      m_bShrinkToFit;
//    private bool      m_bWrapText;
    #endregion

    #region Class Events

    /// <summary>
    ///
    /// </summary>
    [ Category( "Property Changed" ) ]
    public event ValueChangedEventHandler AddIndentChanged;

    /// <summary>
    ///
    /// </summary>
    [ Category( "Property Changed" ) ]
    public event ValueChangedEventHandler BordersChanged;

    /// <summary>
    ///
    /// </summary>
    [ Category( "Property Changed" ) ]
    public event ValueChangedEventHandler FontChanged;

    /// <summary>
    ///
    /// </summary>
    [ Category( "Property Changed" ) ]
    public event ValueChangedEventHandler FormulaHiddenChanged;

    /// <summary>
    ///
    /// </summary>
    [ Category( "Property Changed" ) ]
    public event ValueChangedEventHandler HorizontalAlignmentChanged;

    /// <summary>
    ///
    /// </summary>
    [ Category( "Property Changed" ) ]
    public event ValueChangedEventHandler IndentLevelChanged;

    /// <summary>
    ///
    /// </summary>
    [ Category( "Property Changed" ) ]
    public event ValueChangedEventHandler LockedChanged;

    /// <summary>
    ///
    /// </summary>
    [ Category( "Property Changed" ) ]
    public event ValueChangedEventHandler NumberFormatChanged;

    /// <summary>
    ///
    /// </summary>
    [ Category( "Property Changed" ) ]
    public event ValueChangedEventHandler NumberFormatLocalChanged;

    /// <summary>
    ///
    /// </summary>
    [ Category( "Property Changed" ) ]
    public event ValueChangedEventHandler OrientationChanged;

    /// <summary>
    ///
    /// </summary>
    [ Category( "Property Changed" ) ]
    public event ValueChangedEventHandler ShrinkToFitChanged;

    /// <summary>
    ///
    /// </summary>
    [ Category( "Property Changed" ) ]
    public event ValueChangedEventHandler VerticalAlignmentChanged;

    /// <summary>
    ///
    /// </summary>
    [ Category( "Property Changed" ) ]
    public event ValueChangedEventHandler WrapTextChanged;
    #endregion

    #region Class constructors
    /// <summary>
    ///
    /// </summary>
    /// <param name="application"></param>
    /// <param name="parent"></param>
    public CellFormatImpl( IApplication application, object parent )
      : base( application, parent )
    {
      m_book = ( WorkbookImpl )FindParent( typeof( WorkbookImpl ) );

      // By default, we use only general formatting.
      m_ExtFormat = ( ExtendedFormatImpl )m_book.InnerExtFormats[ 0 ];
      m_ExtFormat.AddReference();
    }
    /// <summary>
    /// Creates an instance of CellFormatImpl and takes all needed data from parentRange.
    /// </summary>
    /// <param name="parentRange"></param>
    public CellFormatImpl( IRange parentRange )
      : this( parentRange.Application, parentRange.Parent )
    {
      RangeImpl range = ( RangeImpl )parentRange;
    }
    /// <summary>
    /// Creates an instance of CellFormatImpl and takes all needed data from parentRange.
    /// </summary>
    /// <param name="parentRange"></param>
    public CellFormatImpl( IRange parentRange, int iExtFormat )
      : this( parentRange.Application, parentRange.Parent )
    {
      RangeImpl range = ( RangeImpl )parentRange;

      // free old ExtFormat
      m_ExtFormat.ReleaseReference();

      m_ExtFormat = ( ExtendedFormatImpl )m_book.InnerExtFormats[ iExtFormat ];
      m_ExtFormat.AddReference();
    }
    #endregion

    #region Class methods
    /// <summary>
    /// This method is called before any property changes its value.
    /// </summary>
    /// <returns></returns>
    private void BeforeExtFormatPropertyChange()
    {
      if( m_ExtFormat.ReferenceCount >= 1 )
      {
        m_ExtFormat.ReleaseReference();
        ExtendedFormatImpl oldFormat = m_ExtFormat;
        m_ExtFormat = (ExtendedFormatImpl) m_book.CreateExtFormat( oldFormat );
        m_iExtFormatIndex = m_book.InnerExtFormats.Length - 1;
      }
    }
    #endregion

    #region ICellFormat properties
    /// <summary>
    ///
    /// </summary>
    public object   AddIndent
    {
      get
      {
        // TODO:  Add CellFormatImpl.AddIndent getter implementation.
        throw new NotImplementedException();
      }
      set
      {
        if( value != m_objAddIndent )
        {
          ValueChangedEventArgs args = new ValueChangedEventArgs( m_objAddIndent, value );
          m_objAddIndent = value;
          OnAddIndentChanged( args );
        }
      }
    }

    /// <summary>
    ///
    /// </summary>
    public IBorders Borders
    {
      get
      {
        // TODO:  Add CellFormatImpl.Borders getter implementation.
        throw new NotImplementedException();
      }
      set
      {
        if( value != m_borders )
        {
          ValueChangedEventArgs args = new ValueChangedEventArgs( m_borders, value );
          m_borders = value;
          OnBordersChanged( args );
        }
      }
    }

    /// <summary>
    ///
    /// </summary>
    public ExcelKnownColors Color
    {
      get
      {
        // TODO:  Add CellFormatImpl.Color getter implementation.
        return ExcelKnownColors.None;
      }
      set
      {
        // TODO:  Add CellFormatImpl.Color setter implementation.
        throw new NotImplementedException();
      }
    }
    /// <summary>
    ///
    /// </summary>
    public IFont    Font
    {
      get
      {
        return (IFont)m_ExtFormat;
      }
      set
      {
        if( value != m_font )
        {
          ValueChangedEventArgs args = new ValueChangedEventArgs( m_font, value );
          m_font = value;
          OnFontChanged( args );
        }
      }
    }

    /// <summary>
    ///
    /// </summary>
    public object   FormulaHidden
    {
      get
      {
        return m_ExtFormat.IsFormulaHidden;
      }
      set
      {
        if( (bool)value != m_ExtFormat.IsFormulaHidden )
        {
          ValueChangedEventArgs args = new ValueChangedEventArgs( m_ExtFormat.IsFormulaHidden, value );
          m_ExtFormat.IsFormulaHidden = (bool)value;
          OnFormulaHiddenChanged( args );
        }
      }
    }

    /// <summary>
    ///
    /// </summary>
    public ExcelHAlign  HorizontalAlignment
    {
      get
      {
        return m_ExtFormat.HorizontalAlignment;
      }
      set
      {
        if( value != m_ExtFormat.HorizontalAlignment )
        {
          ValueChangedEventArgs args = new ValueChangedEventArgs( m_ExtFormat.HorizontalAlignment, value );
          m_ExtFormat.HorizontalAlignment = value;
          OnHorizontalAlignmentChanged( args );
        }
      }
    }

    /// <summary>
    /// Gets or sets indent level.
    /// </summary>
    public int      IndentLevel
    {
      get
      {
        return m_ExtFormat.IndentLevel;
      }
      set
      {
        if( value != m_ExtFormat.IndentLevel )
        {
          BeforeExtFormatPropertyChange();
          ValueChangedEventArgs args = new ValueChangedEventArgs( m_ExtFormat.IndentLevel, value );
          m_ExtFormat.IndentLevel = value;
          OnIndentLevelChanged( args );
        }
      }
    }

    /// <summary>
    ///
    /// </summary>
    public object   Locked
    {
      get
      {
        return m_ExtFormat.IsLocked;
      }
      set
      {
        if( ( bool )value != m_ExtFormat.IsLocked )
        {
          BeforeExtFormatPropertyChange();
          ValueChangedEventArgs args = new ValueChangedEventArgs( m_ExtFormat.IsLocked, value );
          m_ExtFormat.IsLocked = ( bool )value;
          OnLockedChanged( args );
        }
      }
    }

    /// <summary>
    ///
    /// </summary>
    public string   NumberFormat
    {
      get
      {
        return m_strNumFormat;
      }
      set
      {
        if( value != m_strNumFormat )
        {
          ValueChangedEventArgs args = new ValueChangedEventArgs( m_strNumFormat, value );
          m_strNumFormat = value;
          OnNumberFormatChanged( args );
        }
      }
    }

    /// <summary>
    ///
    /// </summary>
    public string   NumberFormatLocal
    {
      get
      {
        return m_strNumFormatLocal;
      }
      set
      {
        if( value != m_strNumFormatLocal )
        {
          ValueChangedEventArgs args = new ValueChangedEventArgs( m_strNumFormatLocal, value );
          m_strNumFormatLocal = value;
          OnNumberFormatLocalChanged( args );
        }
      }
    }

    /// <summary>
    ///
    /// </summary>
    public int      Orientation
    {
      get
      {
        int value = m_ExtFormat.Rotation;

        if( value >= 91 && value <= 180 )
          return -( value - 90 );

        return value;
      }
      set
      {
        if( value < -90 )
          throw new ArgumentOutOfRangeException( "Orientation angle can't be less than -90 degrees." );

        if( value != m_ExtFormat.Rotation )
        {
          BeforeExtFormatPropertyChange();
          ValueChangedEventArgs args = new ValueChangedEventArgs( m_ExtFormat.Rotation, value );
          m_ExtFormat.Rotation = ( value >= 0 ) ? (ushort)value : (ushort)( -value + 90 );
          OnOrientationChanged( args );
        }
      }
    }

    /// <summary>
    ///
    /// </summary>
    public object   ShrinkToFit
    {
      get
      {
        return m_ExtFormat.ShrinkToFit;
      }
      set
      {
        if( (bool)value != m_ExtFormat.ShrinkToFit )
        {
          BeforeExtFormatPropertyChange();
          ValueChangedEventArgs args = new ValueChangedEventArgs( m_ExtFormat.ShrinkToFit, value );
          m_ExtFormat.ShrinkToFit = (bool)value;
          OnShrinkToFitChanged( args );
        }
      }
    }

    /// <summary>
    ///
    /// </summary>
    public object   VerticalAlignment
    {
      get
      {
        return m_ExtFormat.VerticalAlignment;
      }
      set
      {
        if( ( ExcelVAlign )value != m_ExtFormat.VerticalAlignment )
        {
          BeforeExtFormatPropertyChange();
          ValueChangedEventArgs args = new ValueChangedEventArgs( m_ExtFormat.VerticalAlignment, value );
          m_ExtFormat.VerticalAlignment = ( ExcelVAlign )value;
          OnVerticalAlignmentChanged( args );
        }
      }
    }

    /// <summary>
    ///
    /// </summary>
    public object   WrapText
    {
      get
      {
        return m_ExtFormat.WrapText;
      }
      set
      {
        if( ( bool )value != m_ExtFormat.WrapText )
        {
          BeforeExtFormatPropertyChange();
          ValueChangedEventArgs args = new ValueChangedEventArgs( m_ExtFormat.WrapText, value );
          WrapText = ( bool )value;
          OnWrapTextChanged( args );
        }
      }
    }

    #endregion

    #region ICellFormat methods
    #endregion

    #region Format Properties
    /// <summary>
    ///
    /// </summary>
    public int FillPatern
    {
      get
      {
        return m_ExtFormat.FillPatern;
      }
      set
      {
        m_ExtFormat.FillPatern = value;
      }
    }
    /// <summary>
    ///
    /// </summary>
    public ExcelKnownColors FillBackground
    {
      get
      {
        return m_ExtFormat.FillBackground;
      }
      set
      {
        m_ExtFormat.FillBackground = value;
      }
    }
    /// <summary>
    ///
    /// </summary>
    public ExcelKnownColors FillForeground
    {
      get
      {
        return m_ExtFormat.FillForeground;
      }
      set
      {
        m_ExtFormat.FillForeground = value;
      }
    }
    #endregion

    #region Implementation properties
    public ushort ExtendedFormatIndex
    {
      get
      {
        return m_ExtFormat.ExtendedFormatIndex;
      }
      set
      {
        if( m_ExtFormat == null || value != ExtendedFormatIndex )
        {
          if( m_ExtFormat != null ) m_ExtFormat.ReleaseReference();

          m_ExtFormat = (ExtendedFormatImpl) m_book.InnerExtFormats[ value ];
          m_ExtFormat.AddReference();
        }
      }
    }
    /// <summary>
    ///
    /// </summary>
    #endregion

    #region Class Event raisers
    /// <summary>
    ///
    /// </summary>
    protected void RaiseAddIndentChangedEvent( ValueChangedEventArgs args )
    {
      if( AddIndentChanged != null )
      {
        AddIndentChanged( this, args );
      }
    }

    /// <summary>
    ///
    /// </summary>
    protected void RaiseBordersChangedEvent( ValueChangedEventArgs args )
    {
      if( BordersChanged != null )
      {
        BordersChanged( this, args );
      }
    }

    /// <summary>
    ///
    /// </summary>
    protected void RaiseFontChangedEvent( ValueChangedEventArgs args )
    {
      if( FontChanged != null )
      {
        FontChanged( this, args );
      }
    }

    /// <summary>
    ///
    /// </summary>
    protected void RaiseFormulaHiddenChangedEvent( ValueChangedEventArgs args )
    {
      if( FormulaHiddenChanged != null )
      {
        FormulaHiddenChanged( this, args );
      }
    }

    /// <summary>
    ///
    /// </summary>
    protected void RaiseHorizontalAlignmentChangedEvent( ValueChangedEventArgs args )
    {
      if( HorizontalAlignmentChanged != null )
      {
        HorizontalAlignmentChanged( this, args );
      }
    }

    /// <summary>
    ///
    /// </summary>
    protected void RaiseIndentLevelChangedEvent( ValueChangedEventArgs args )
    {
      if( IndentLevelChanged != null )
      {
        IndentLevelChanged( this, args );
      }
    }

    /// <summary>
    ///
    /// </summary>
    protected void RaiseLockedChangedEvent( ValueChangedEventArgs args )
    {
      if( LockedChanged != null )
      {
        LockedChanged( this, args );
      }
    }

    /// <summary>
    ///
    /// </summary>
    protected void RaiseNumberFormatChangedEvent( ValueChangedEventArgs args )
    {
      if( NumberFormatChanged != null )
      {
        NumberFormatChanged( this, args );
      }
    }

    /// <summary>
    ///
    /// </summary>
    protected void RaiseNumberFormatLocalChangedEvent( ValueChangedEventArgs args )
    {
      if( NumberFormatLocalChanged != null )
      {
        NumberFormatLocalChanged( this, args );
      }
    }

    /// <summary>
    ///
    /// </summary>
    protected void RaiseOrientationChangedEvent( ValueChangedEventArgs args )
    {
      if( OrientationChanged != null )
      {
        OrientationChanged( this, args );
      }
    }

    /// <summary>
    ///
    /// </summary>
    protected void RaiseShrinkToFitChangedEvent( ValueChangedEventArgs args )
    {
      if( ShrinkToFitChanged != null )
      {
        ShrinkToFitChanged( this, args );
      }
    }

    /// <summary>
    ///
    /// </summary>
    protected void RaiseVerticalAlignmentChangedEvent( ValueChangedEventArgs args )
    {
      if( VerticalAlignmentChanged != null )
      {
        VerticalAlignmentChanged( this, args );
      }
    }

    /// <summary>
    ///
    /// </summary>
    protected void RaiseWrapTextChangedEvent( ValueChangedEventArgs args )
    {
      if( WrapTextChanged != null )
      {
        WrapTextChanged( this, args );
      }
    }
    #endregion

    #region Class overrides
    /// <summary>
    ///
    /// </summary>
    /// <param name="args"></param>
    protected virtual void OnAddIndentChanged( ValueChangedEventArgs args )
    {
      RaiseAddIndentChangedEvent( args );
    }

    /// <summary>
    ///
    /// </summary>
    protected virtual void OnBordersChanged( ValueChangedEventArgs args )
    {
      RaiseBordersChangedEvent( args );
    }

    /// <summary>
    ///
    /// </summary>
    protected virtual void OnFontChanged( ValueChangedEventArgs args )
    {
      RaiseFontChangedEvent( args );
    }

    /// <summary>
    ///
    /// </summary>
    protected virtual void OnFormulaHiddenChanged( ValueChangedEventArgs args )
    {
      RaiseFormulaHiddenChangedEvent( args );
    }

    /// <summary>
    ///
    /// </summary>
    protected virtual void OnHorizontalAlignmentChanged( ValueChangedEventArgs args )
    {
      RaiseHorizontalAlignmentChangedEvent( args );
    }

    /// <summary>
    ///
    /// </summary>
    protected virtual void OnIndentLevelChanged( ValueChangedEventArgs args )
    {
      RaiseIndentLevelChangedEvent( args );
    }

    /// <summary>
    ///
    /// </summary>
    protected virtual void OnLockedChanged( ValueChangedEventArgs args )
    {
      RaiseLockedChangedEvent( args );
    }

    /// <summary>
    ///
    /// </summary>
    protected virtual void OnNumberFormatChanged( ValueChangedEventArgs args )
    {
      RaiseNumberFormatChangedEvent( args );
    }

    /// <summary>
    ///
    /// </summary>
    protected virtual void OnNumberFormatLocalChanged( ValueChangedEventArgs args )
    {
      RaiseNumberFormatLocalChangedEvent( args );
    }

    /// <summary>
    ///
    /// </summary>
    protected virtual void OnOrientationChanged( ValueChangedEventArgs args )
    {
      RaiseOrientationChangedEvent( args );
    }

    /// <summary>
    ///
    /// </summary>
    protected virtual void OnShrinkToFitChanged( ValueChangedEventArgs args )
    {
      RaiseShrinkToFitChangedEvent( args );
    }

    /// <summary>
    ///
    /// </summary>
    protected virtual void OnVerticalAlignmentChanged( ValueChangedEventArgs args )
    {
      RaiseVerticalAlignmentChangedEvent( args );
    }

    /// <summary>
    ///
    /// </summary>
    protected virtual void OnWrapTextChanged( ValueChangedEventArgs args )
    {
      RaiseWrapTextChangedEvent( args );
    }
    #endregion

    #region IFont Members
    /// <summary>
    ///
    /// </summary>
    public bool Bold
    {
      get
      {
        return m_ExtFormat.Bold;
      }
      set
      {
        if( m_ExtFormat.Bold != value )
        {
          m_ExtFormat.Bold = value;
        }
      }
    }

    /// <summary>
    ///
    /// </summary>
    public ExcelColorIndex ColorIndex
    {
      get
      {
        return m_ExtFormat.ColorIndex;
      }
      set
      {
        m_ExtFormat.ColorIndex = value;
      }
    }

    /// <summary>
    ///
    /// </summary>
    public bool Italic
    {
      get
      {
        return m_ExtFormat.Italic;
      }
      set
      {
        m_ExtFormat.Italic = value;
      }
    }

    /// <summary>
    ///
    /// </summary>
    public bool OutlineFont
    {
      get
      {
        return m_ExtFormat.OutlineFont;
      }
      set
      {
        m_ExtFormat.OutlineFont = value;
      }
    }

    /// <summary>
    ///
    /// </summary>
    public bool Shadow
    {
      get
      {
        return m_ExtFormat.Shadow;
      }
      set
      {
        m_ExtFormat.Shadow = value;
      }
    }

    /// <summary>
    ///
    /// </summary>
    public int Size
    {
      get
      {
        return m_ExtFormat.Size;
      }
      set
      {
        m_ExtFormat.Size = value;
      }
    }

    /// <summary>
    ///
    /// </summary>
    public bool Strikethrough
    {
      get
      {
        return m_ExtFormat.Strikethrough;
      }
      set
      {
        m_ExtFormat.Strikethrough = value;
      }
    }

    /// <summary>
    ///
    /// </summary>
    public bool Subscript
    {
      get
      {
        return m_ExtFormat.Subscript;
      }
      set
      {
        m_ExtFormat.Subscript = value;
      }
    }

    /// <summary>
    ///
    /// </summary>
    public bool Superscript
    {
      get
      {
        return m_ExtFormat.Superscript;
      }
      set
      {
        m_ExtFormat.Superscript = value;
      }
    }

    /// <summary>
    ///
    /// </summary>
    public Syncfusion.XlsIO.Interfaces.ExcelUnderline Underline
    {
      get
      {
        return m_ExtFormat.Underline;
      }
      set
      {
        m_ExtFormat.Underline = value;
      }
    }

    #endregion
  }
}
