#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;

using Syncfusion.XlsIO;
using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Parser.Biff_Records;

namespace Syncfusion.XlsIO.Implementation
{
	/// <summary>
	/// Summary description for RangeRichTextString.
	/// </summary>
	public class RangeRichTextString
    : RichTextString
    , IRTFWrapper
	{
    #region Class members
    /// <summary>
    /// Parent worksheet.
    /// </summary>
    private WorksheetImpl m_worksheet;
    /// <summary>
    /// Cell index.
    /// </summary>
    private long m_lCellIndex;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    /// <param name="parent"></param>
    /// <param name="row"></param>
    /// <param name="column"></param>
    public RangeRichTextString( IApplication application, object parent, int row, int column )
      : this( application, parent, RangeImpl.GetCellIndex( column, row ) )
    {
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    /// <param name="parent"></param>
    /// <param name="cellIndex">Cell index.</param>
    public RangeRichTextString( IApplication application, object parent, long cellIndex )
      : base( application, ( ( WorksheetImpl )parent ).ParentWorkbook )
    {
      m_worksheet = ( WorksheetImpl )parent;
      if (cellIndex != -1)
      {
          m_lCellIndex = cellIndex;
          m_text = m_worksheet.GetTextWithFormat(m_lCellIndex);
      }
      else
      {
          m_text = m_worksheet.GetTextWithFormat(-1);
      }

      if( m_text != null ) m_text = m_text.TypedClone();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    /// <param name="parent"></param>
    /// <param name="cellIndex">Cell index.</param>
    /// <param name="text">Formatted text object.</param>
    public RangeRichTextString( IApplication application, object parent, long cellIndex,
      TextWithFormat text )
      : base( application, ( ( WorksheetImpl )parent ).ParentWorkbook, true )
    {
      m_worksheet = ( WorksheetImpl )parent;
      m_lCellIndex = cellIndex;
      m_text = text;
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Returns default font. Read-only.
    /// </summary>
    public override FontImpl DefaultFont
    {
      get
      {
        ExtendedFormatImpl format = m_worksheet.GetExtendedFormat( m_lCellIndex );
        int iFontIndex = format.FontIndex;

        return m_book.InnerFonts[ iFontIndex ] as FontImpl;
      }
      internal set
      {
        int iRow = RangeImpl.GetRowFromCellIndex( m_lCellIndex );
        int iColumn = RangeImpl.GetColumnFromCellIndex( m_lCellIndex );
        IInternalFont intFont = m_book.AddFont( value ) as IInternalFont;
        if(iRow != 0 || iColumn != 0)
            ( m_worksheet[ iRow, iColumn ].CellStyle as ExtendedFormatWrapper ).FontIndex = intFont.Index;
      }
    }

    /// <summary>
    /// This method is called before any changes made to the rich text string.
    /// </summary>
    public override void BeginUpdate()
    {
      if( BeginCallsCount == 0 )
      {

        if( m_text != null )
        {
          SSTDictionary sst = m_book.InnerSST;
          sst.Parse();
          int iIndex = m_worksheet.GetStringIndex( m_lCellIndex );//sst.GetStringIndex( m_text );
          int iCount = sst.GetStringCount( iIndex );

          if( iCount != 1 )
          {
            // We have to create copy of the string.
            m_text = m_text.TypedClone();

            if (iIndex != -1) 
            sst.RemoveDecrease(iIndex);
          }
          else
          {
            sst.RemoveDecrease( iIndex );
          }
        }
        else
        {
          m_text = new TextWithFormat();
        }
      }

      base.BeginUpdate();
    }
    /// <summary>
    /// This method is called after any changes made to the rich text string.
    /// </summary>
    public override void EndUpdate()
    {
      base.EndUpdate();

      if( BeginCallsCount == 0 )
      {
        SSTDictionary sst = m_book.InnerSST;
        object key = ( m_text.FormattingRunsCount > 0 )
          ? ( object )m_text
          : m_text.Text;

        int iStringIndex = sst.AddIncrease( key );

        m_worksheet.SetLabelSSTIndex( m_lCellIndex, iStringIndex );
      }
    }

    #endregion

    #region Class properties
    /// <summary>
    /// Returns index of the string.
    /// </summary>
    public int Index
    {
      get
      {
        return m_worksheet.GetStringIndex( m_lCellIndex );//m_book.InnerSST.GetStringIndex( m_text );
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Performs application-defined tasks associated with freeing,
    /// releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      GC.SuppressFinalize( this );
    }
    #endregion
  }

  /// <summary>
  /// 
  /// </summary>
  public class RTFStringArray
    : IRTFWrapper
  {
    #region Class members
    /// <summary>
    /// Parent range.
    /// </summary>
    private IRange m_range;
    /// <summary>
    /// Represents an RTF string.
    /// </summary>
    private string m_rtfText;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Private constructor - to prevent user from creating instances without arguments.
    /// </summary>
    private RTFStringArray()
    {
    }
    /// <summary>
    /// Creates new instance of the RTFStringArray.
    /// </summary>
    /// <param name="range">Parent range.</param>
    public RTFStringArray( IRange range )
    {
      if( range == null )
        throw new ArgumentNullException( "range" );

      m_range = range;
    }
    #endregion

    #region IRichTextString Methods
    /// <summary>
    /// Returns font which is applied to character at the specified position.
    /// </summary>
    /// <param name="iPosition">Character index.</param>
    /// <returns>Font which is applied to character at the specified position.</returns>
    public IFont GetFont(int iPosition )
    {
      IRange[] arrCells = m_range.Cells;
      int iCount = arrCells.Length;

      if( iCount == 0 ) return null;

      if( !arrCells[ 0 ].HasRichText ) return null;

      IFont fontResult = arrCells[ 0 ].RichText.GetFont( iPosition );

      for( int i = 1; i < iCount; i++ )
      {
        if( !arrCells[ i ].HasRichText ) return null;

        if( fontResult != arrCells[ i ].RichText.GetFont( iPosition ) ) return null;
      }

      return fontResult;
    }
    /// <summary>
    /// Sets font for range of characters.
    /// </summary>
    /// <param name="iStartPos">First character of the range.</param>
    /// <param name="iEndPos">Last character of the range.</param>
    /// <param name="font">Font to set.</param>
    public void SetFont(int iStartPos, int iEndPos, IFont font)
    {
      IRange[] arrCells = m_range.Cells;

      for( int i = 0, len = arrCells.Length; i < len; i++ )
      {
        //if( arrCells[ i ].HasRichText )
      {
        arrCells[ i ].RichText.SetFont( iStartPos, iEndPos, font );
      }
      }
    }
    /// <summary>
    /// Clears string formatting.
    /// </summary>
    public void ClearFormatting()
    {
      IRange[] arrCells = m_range.Cells;

      for( int i = 0, len = arrCells.Length; i < len; i++ )
      {
        if( arrCells[ i ].HasRichText )
        {
          arrCells[ i ].RichText.ClearFormatting();
        }
      }
    }
    /// <summary>
    /// Appends rich text string with specified text and font.
    /// </summary>
    /// <param name="text">Text to append.</param>
    /// <param name="font">Font to use.</param>
    public void Append( string text, IFont font )
    {
      IRange[] arrCells = m_range.Cells;

      for( int i = 0, len = arrCells.Length; i < len; i++ )
      {
        if( arrCells[ i ].HasRichText )
        {
          arrCells[ i ].RichText.Append( text, font );
        }
      }
    }
    #endregion

    #region IRichTextString Properties
    /// <summary>
    /// Gets / sets text of the string.
    /// </summary>
    public string Text
    {
      get
      {
        IRange[] arrCells = m_range.Cells;
        int iCount = arrCells.Length;

        if( iCount == 0 ) return null;

        //if( !arrCells[ 0 ].HasRichText ) return null;

        string strResult = arrCells[ 0 ].Text;

        if( strResult != null )
        {

          for( int i = 1; i < iCount; i++ )
          {
            if( strResult != arrCells[ i ].Text )
            {
              strResult = null;
              break;
            }
          }
        }

        return strResult;
      }
      set
      {
        IRange[] arrCells = m_range.Cells;

        for( int i = 0, len = arrCells.Length; i < len; i++ )
        {
          arrCells[ i ].RichText.Text = value;
        }
      }
    }

    /// <summary>
    /// Returns text in rtf format. Read-only.
    /// </summary>
    public string RtfText
    {
      get
      {
        IRange[] arrCells = m_range.Cells;
        int iCount = arrCells.Length;

        if( iCount == 0 ) return null;

        if( !arrCells[ 0 ].HasRichText ) return null;

        m_rtfText = arrCells[ 0 ].RichText.RtfText;

        for( int i = 1; i < iCount; i++ )
        {
          if( !arrCells[ i ].HasRichText ) return null;

          if (m_rtfText != arrCells[i].RichText.RtfText) return null;
        }

        return m_rtfText;
      }
        set
        {
            m_rtfText = value;
        }
    }

    /// <summary>
    /// Indicates whether rich text string has formatting runs. Read-only.
    /// </summary>
    public bool IsFormatted
    {
      get
      {
        IRange[] arrCells = m_range.Cells;
        int iCount = arrCells.Length;

        if( iCount == 0 ) return false;

        for( int i = 0; i < iCount; i++ )
        {
          if( !arrCells[ i ].HasRichText ) return false;

          if( !arrCells[ i ].RichText.IsFormatted ) return false;
        }

        return true;
      }
    }

    #endregion

    #region IParentApplication Members
    /// <summary>
    /// Application object for this object.
    /// </summary>
    public IApplication Application
    {
      get
      {
        return m_range.Application;
      }
    }

    /// <summary>
    /// Parent object for this object.
    /// </summary>
    public object Parent
    {
      get
      {
        return m_range;
      }
    }

    #endregion

    #region IOptimizedUpdate methods
    /// <summary>
    /// 
    /// </summary>
    public void BeginUpdate()
    {
    }
    /// <summary>
    /// 
    /// </summary>
    public void EndUpdate()
    {
    }
    #endregion

    #region IDisposable methods
    /// <summary>
    /// Performs application-defined tasks associated with freeing,
    /// releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// 
    /// </summary>
    public void Clear()
    {
      IRange[] arrCells = m_range.Cells;

      for( int i = 0, len = arrCells.Length; i < len; i++ )
      {
        if( arrCells[ i ].HasRichText )
        {
          ( ( RangeRichTextString )arrCells[ i ].RichText ).Clear();
        }
      }
    }
    #endregion
  }

  /// <summary>
  /// 
  /// </summary>
  public class RTFCommentArray
    : CommonObject
    , IRichTextString
    , IParentApplication
  {
    #region Class members
    /// <summary>
    /// Parent range.
    /// </summary>
    private IRange m_range;
    /// <summary>
    /// Represents an RTF string.
    /// </summary>
    private string m_rtfText;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    /// <param name="parent"></param>
    public RTFCommentArray( IApplication application, object parent )
      : base( application, parent )
    {
      SetParents();
    }
    #endregion

    #region IRichTextString Members
    /// <summary>
    /// Returns font for character at specified position.
    /// </summary>
    /// <param name="iPosition">Position of the symbol.</param>
    /// <returns>
    /// Font for character at specified position if it is equal for all
    /// cells in the parent range, otherwise NULL is returned.
    /// </returns>
    public IFont GetFont( int iPosition )
    {
      IRange[] cells = m_range.Cells;
      bool bIsFirst = true;
      IFont result = null;

      for( int i = 0, len = cells.Length; i < len; i++ )
      {
        if( cells[ i ].Comment != null )
        {
          if( bIsFirst )
          {
            result = cells[ i ].Comment.RichText.GetFont( iPosition );
          }
          else
          {
            if( ! result.Equals( cells[ i ].Comment
              .RichText.GetFont( iPosition ) ) )
            {
              return null;
            }
          }
        }
      }

      return result;
    }
    /// <summary>
    /// Sets font for specified range of characters.
    /// </summary>
    /// <param name="iStartPos">First character to set font.</param>
    /// <param name="iEndPos">Last character to set.</param>
    /// <param name="font">Font to set.</param>
    public void SetFont( int iStartPos, int iEndPos, IFont font )
    {
      IRange[] cells = m_range.Cells;

      for( int i = 0, len = cells.Length; i < len; i++ )
      {
        if( cells[ i ].Comment != null )
        {
          cells[ i ].Comment.RichText.SetFont( iStartPos, iEndPos, font );
        }
      }
    }
    /// <summary>
    /// Clears formatting.
    /// </summary>
    public void ClearFormatting()
    {
      IRange[] cells = m_range.Cells;

      for( int i = 0, len = cells.Length; i < len; i++ )
      {
        if( cells[ i ].Comment != null )
        {
          cells[ i ].Comment.RichText.ClearFormatting();
        }
      }
    }
    /// <summary>
    /// Appends rich text string with specified text and font.
    /// </summary>
    /// <param name="text">Text to append.</param>
    /// <param name="font">Font to use.</param>
    public void Append( string text, IFont font )
    {
      IRange[] cells = m_range.Cells;

      for( int i = 0, len = cells.Length; i < len; i++ )
      {
        if( cells[ i ].Comment != null )
        {
          cells[ i ].Comment.RichText.Append( text, font );
        }
      }
    }
    /// <summary>
    /// Clears text and formatting.
    /// </summary>
    public void Clear()
    {
      IRange[] cells = m_range.Cells;

      for( int i = 0, len = cells.Length; i < len; i++ )
      {
        if( cells[ i ].Comment != null )
        {
          cells[ i ].Comment.RichText.Clear();
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public string Text
    {
      get
      {
        IRange[] cells = m_range.Cells;
        bool bIsFirst = true;
        string result = null;

        for( int i = 0, len = cells.Length; i < len; i++ )
        {
          if( cells[ i ].Comment != null )
          {
            if( bIsFirst )
            {
              result = cells[ i ].Comment.RichText.Text;
            }
            else
            {
              if( result != cells[ i ].Comment.RichText.Text )
              {
                return null;
              }
            }
          }
        }

        return result;
      }
      set
      {
        IRange[] cells = m_range.Cells;

        for( int i = 0, len = cells.Length; i < len; i++ )
        {
          cells[ i ].AddComment().RichText.Text = value;
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public string RtfText
    {
      get
      {
        IRange[] cells = m_range.Cells;
        bool bIsFirst = true;
        m_rtfText = null;

        for( int i = 0, len = cells.Length; i < len; i++ )
        {
          if( cells[ i ].Comment != null )
          {
            if( bIsFirst )
            {
                m_rtfText = cells[i].Comment.RichText.RtfText;
            }
            else
            {
                if (m_rtfText != cells[i].Comment.RichText.RtfText)
              {
                return null;
              }
            }
          }
        }

        return m_rtfText;
      }
        set
        {
            m_rtfText = value;
        }
    }
    /// <summary>
    /// Indicates whether rich text string has formatting runs. Read-only.
    /// </summary>
    public bool IsFormatted
    {
      get
      {
        IRange[] cells = m_range.Cells;
        bool bIsFirst = true;
        bool result = false;

        for( int i = 0, len = cells.Length; i < len; i++ )
        {
          if( cells[ i ].Comment != null )
          {
            if( bIsFirst )
            {
              result = cells[ i ].Comment.RichText.IsFormatted;
            }
            else
            {
              if( result != cells[ i ].Comment.RichText.IsFormatted )
              {
                return false;
              }
            }
          }
        }

        return result;
      }
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// 
    /// </summary>
    private void SetParents()
    {
      m_range = FindParent( typeof( IRange ) ) as IRange;

      if( m_range == null )
        throw new ArgumentNullException( "Can't find parent range" );
    }
    #endregion

    #region IOptimizedUpdate members
    /// <summary>
    /// 
    /// </summary>
    public void BeginUpdate()
    {
    }
    /// <summary>
    /// 
    /// </summary>
    public void EndUpdate()
    {
    }
    #endregion
  }

  /// <summary>
  /// Interface for Rich text string wrapper.
  /// </summary>
  public interface IRTFWrapper
    : IDisposable
    , IRichTextString
    , IOptimizedUpdate
  {
//    /// <summary>
//    /// Clears wrapper.
//    /// </summary>
//    void Clear();
  }

}
