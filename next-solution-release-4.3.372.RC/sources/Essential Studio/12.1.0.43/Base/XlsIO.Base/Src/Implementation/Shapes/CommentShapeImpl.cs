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
using System.Diagnostics;

using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing;
using Syncfusion.XlsIO.Parser.Biff_Records.ObjRecords;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Implementation.Collections;

using FOPTE = Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing.MsofbtOPT.FOPTE;
using System.Collections.Generic;

#if ( WINRT )
using Windows.UI;
using Rectangle=Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
#elif (WP)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif
#endregion

namespace Syncfusion.XlsIO.Implementation.Shapes
{
  /// <summary>
  /// Class used for comment shapes.
  /// </summary>
  public class CommentShapeImpl
    : TextBoxShapeBase
    , ICommentShape
  {
    #region Class constants
    /// <summary>
    /// Instance value of MsofbtSp record.
    /// </summary>
    internal const int ShapeInstance = 202;
    /// <summary>
    /// Version value of MsofbtSp record.
    /// </summary>
    private const int DEF_SHAPE_VERSION = 2;
    /// <summary>
    /// Version value of MsofbtOPT record.
    /// </summary>
    private const int DEF_OPTIONS_VERSION = 3;
    /// <summary>
    /// Instance value of MsofbtOPT record.
    /// </summary>
    private const int DEF_OPTIONS_INSTANCE = 10;
    /// <summary>
    /// Default offset in the cell of comment frame.
    /// </summary>
    public const int DEF_OFFSET = 0xF0;
    /// <summary>
    /// Represents value for make comment shadowed.
    /// </summary>
    private const int DEF_COMMENT_SHADOWED = 196611;
    /// <summary>
    /// Represents if comment show always.
    /// </summary>
    private const int DEF_COMMENT_SHOW_ALWAYS = 131072;
    /// <summary>
    /// Represents if comment doesn't show always.
    /// </summary>
    private const int DEF_COMMENT_NOT_SHOW_ALWAYS = 131074;
    #endregion

    #region Class members
    /// <summary>
    /// Row of the commented cell.
    /// </summary>
    private int m_iRow;
    /// <summary>
    /// Column of the commented cell.
    /// </summary>
    private int m_iColumn;
    /// <summary>
    /// Indicates whether comment is visible.
    /// </summary>
    private bool m_bVisible;
    /// <summary>
    /// Comment's author.
    /// </summary>
    private string m_strAuthor;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates new comment with specified Application and Parent objects.
    /// </summary>
    /// <param name="application">Application object for the current object.</param>
    /// <param name="parent">Parent object for the current object.</param>
    public CommentShapeImpl( IApplication application, object parent )
      : this( application, parent, true )
    {
      InitializeVariables();
      ShapeType = ExcelShapeType.Comment;

      m_bUpdateLineFill = true;
      Fill.ForeColor = ShapeFillImpl.DEF_COMENT_PARSE_COLOR;
      Line.ForeColor = ColorExtension.Black;
      Fill.BackColor = ColorExtension.Empty;
      Line.BackColor = ColorExtension.Empty;
      //HasFill = false;
      Fill.Transparency = 1.0;

      m_strAuthor = m_shapes.Worksheet.Workbook.Author;
      this.IsMoveWithCell = false;
      this.IsSizeWithCell = false;
    }
    /// <summary>
    /// Creates new comment with specified Application and Parent objects.
    /// </summary>
    /// <param name="application">Application object for the current object.</param>
    /// <param name="parent">Parent object for the current object.</param>
    /// <param name="bIsParseOptions">Indicates is parse comment fill line options.</param>
    public CommentShapeImpl( IApplication application, object parent, bool bIsParseOptions )
      : base( application, parent )
    {
      InitializeVariables();
      ShapeType = ExcelShapeType.Comment;

      if( bIsParseOptions )
      {
        m_bUpdateLineFill = true;
        Fill.ForeColor = ShapeFillImpl.DEF_COMENT_PARSE_COLOR;
        Line.ForeColor = ColorExtension.Black;

        Fill.BackColor = ColorExtension.Empty;
        Line.BackColor = ColorExtension.Empty;
        //HasFill = false;
        //Fill.Transparency = 1.0;
      }

      m_strAuthor = m_shapes.Worksheet.Workbook.Author;
      this.IsMoveWithCell = false;
      this.IsSizeWithCell = false;
      FillColor = Color.FromArgb( 0xFF, 0xFF, 0xFF, 0xE1 );
    }
    /// <summary>
    /// Creates new comment with specified text.
    /// </summary>
    /// <param name="application">Application object for the current object.</param>
    /// <param name="parent">Parent object for the current object.</param>
    /// <param name="commentText">Text of new comment.</param>
    public CommentShapeImpl( IApplication application, object parent, string commentText )
      : this( application, parent )
    {
      RichText.Text = commentText;
    }
    /// <summary>
    /// Extracts comment from MsofbtSpContainer.
    /// </summary>
    /// <param name="application">Application object for the current object.</param>
    /// <param name="parent">Parent object for the current object.</param>
    /// <param name="container">Container that represents comment.</param>
    [ CLSCompliant( false ) ]
    public CommentShapeImpl( IApplication application, object parent, MsofbtSpContainer container )
      : this( application, parent, container, ExcelParseOptions.Default )
    {
    }
    /// <summary>
    /// Extracts comment from MsofbtSpContainer.
    /// </summary>
    /// <param name="application">Application object for the current object.</param>
    /// <param name="parent">Parent object for the current object.</param>
    /// <param name="container">Container that represents comment.</param>
    /// <param name="options">Parse options.</param>
    [ CLSCompliant( false ) ]
    public CommentShapeImpl( IApplication application, object parent, MsofbtSpContainer container,
      ExcelParseOptions options )
      : base( application, parent, container, options )
    {
      ShapeType = ExcelShapeType.Comment;
      m_bSupportOptions = true;
      m_bUpdateLineFill = true;

      List<MsoBase> items = container.ItemsList;
      for( int i = 0, len = items.Count; i < len; i++ )
      {
        if( items[ i ] is MsofbtClientData )
        {
          OBJRecord obj = ( items[ i ] as MsofbtClientData ).ObjectRecord;
          ftCmo cmo = obj.RecordsList[ 0 ] as ftCmo;
          ParseNoteRecord( ( int )cmo.ID );
          return;
        }
      }
    }
    /// <summary>
    /// Initialize variables.
    /// </summary>
    protected override void InitializeVariables()
    {
      base.InitializeVariables();
      VmlShape = true;

      if( this.Worksheet.IsParsed )
        FillClientAnchor();
    }
    /// <summary>
    /// Fills client anchor with default values.
    /// </summary>
    private void FillClientAnchor()
    {
      //DetachEvents();
      ClientAnchor.Options = 3;
      ClientAnchor.LeftColumn   = Column - 1;
      ClientAnchor.RightColumn  = Column + 1;
      ClientAnchor.TopRow       = Row - 1;
      ClientAnchor.BottomRow    = Row + 3;
      ClientAnchor.LeftOffset   = DEF_OFFSET;
      ClientAnchor.RightOffset  = DEF_OFFSET;
      ClientAnchor.TopOffset    = DEF_OFFSET;
      ClientAnchor.BottomOffset = DEF_OFFSET;
      UpdateWidth();
      UpdateHeight();
      EvaluateTopLeftPosition();
      //AttachEvents();
    }
    public override void Dispose()
    {
        base.Dispose();
        if (m_graphicFrame != null)
            m_graphicFrame.Dispose();
        if (m_options != null)
            m_options.Dispose();
        if (m_shape != null)
            m_shape.Dispose();
            m_shapes = null;
            GC.SuppressFinalize(this);
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Row of the commented cell.
    /// </summary>
    public int Row
    {
      get
      {
        return m_iRow + 1;
      }
      set
      {
        m_iRow = value - 1;
      }
    }
    /// <summary>
    /// Column of the commented cell.
    /// </summary>
    public int Column
    {
      get
      {
        return m_iColumn + 1;
      }
      set
      {
        m_iColumn = value - 1;
      }
    }
    /// <summary>
    /// Indicates whether comment is visible.
    /// </summary>
    public bool IsVisible
    {
      get
      {
        return m_bVisible;
      }
      set
      {
        m_bVisible = value;
      }
    }
    /// <summary>
    /// Comment's author.
    /// </summary>
    public string Author
    {
      get
      {
        return m_strAuthor;
      }
      set
      {
        m_strAuthor = value;
      }
    }
    /// <summary>
    /// Returns instance value. Read-only.
    /// </summary>
    public override int Instance
    {
      get
      {
        return ( m_shape != null ) ?
          m_shape.Instance :
          ShapeInstance;
      }
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Registers shape in all required sub collections.
    /// </summary>
    public override void RegisterInSubCollection()
    {
      m_shapes.WorksheetBase.InnerComments.AddComment( this );
    }
    /// <summary>
    /// Creates a clone of the current shape.
    /// </summary>
    /// <param name="parent">New parent for the shape object.</param>
    /// <param name="hashNewNames">Dictionary with new worksheet names.</param>
    /// <param name="dicFontIndexes">Dictionary with new font indexes.</param>
    /// <param name="addToCollections">Indicates whether we should add created
    /// shape into all necessary parent collections.</param>
    /// <returns>A copy of the current shape.</returns>
    public override IShape Clone( object parent, Dictionary<string, string> hashNewNames,
      Dictionary<int, int> dicFontIndexes, bool addToCollections )
    {
      //int iRow = Row;
      //int iColumn = Column;
      //WorksheetImpl sheet = FindParent( parent, typeof( WorksheetImpl ) ) as WorksheetImpl;
      //RangeImpl range = ( RangeImpl )sheet.Range[ iRow, iColumn ];
      //CommentShapeImpl result = ( CommentShapeImpl )range.AddComment( m_bUpdateLineFill );

      CommentShapeImpl result = ( CommentShapeImpl )base.Clone( parent, hashNewNames,
        dicFontIndexes, addToCollections );

      result.IsVisible = IsVisible;
      result.CopyFrom( this, hashNewNames, dicFontIndexes );
      result.CopyCommentOptions( this, dicFontIndexes );

      if( addToCollections )
        result.Worksheet.InnerComments.AddComment( result );

      return result;
    }
    /// <summary>
    /// Serializes current shape.
    /// </summary>
    /// <param name="spgrContainer">Container that will receive this comment.</param>
    [ CLSCompliant( false ) ]
    protected override void SerializeShape( MsofbtSpgrContainer spgrContainer )
    {
      if( spgrContainer == null )
        throw new ArgumentNullException( "spgrContainer" );

      MsofbtSpContainer spContainer = ( MsofbtSpContainer )MsoFactory.GetRecord(
        MsoRecords.msofbtSpContainer );//new MsofbtSpContainer( spgrContainer );

      MsofbtClientData clientData = ( MsofbtClientData )MsoFactory.GetRecord(
        MsoRecords.msofbtClientData );
      
      ftCmo cmo = null;

      if( Obj == null )
      {
        OBJRecord obj = ( OBJRecord )BiffRecordFactory.GetRecord( TBIFFRecord.OBJ );
            
        cmo = new ftCmo();
      
        cmo.ObjectType = TObjType.otComment;
        cmo.Printable = true;

        cmo.Locked = true;
        cmo.AutoLine = true;
        ftEnd end = new ftEnd();
      
        obj.AddSubRecord( cmo );
        obj.AddSubRecord( end );
        clientData.AddRecord( obj );
      }
      else
      {
        cmo = Obj.RecordsList[ 0 ] as ftCmo;
        clientData.AddRecord( Obj );
      }

      cmo.ID = ( OldObjId > 0 ) ? ( ushort )OldObjId : ( ushort )ParentWorkbook.CurrentObjectId;
      spContainer.AddItem( m_shape );
      
      MsofbtOPT options = SerializeOptions( spContainer );
      //options.Version = DEF_OPTIONS_VERSION;
      //options.Instance = DEF_OPTIONS_INSTANCE;

      if( options.Properties.Length > 0 ) spContainer.AddItem( options );
      
      spContainer.AddItem( ClientAnchor );
      spContainer.AddItem( clientData );
      spContainer.AddItem( GetClientTextBoxRecord( spContainer ) );
      spgrContainer.AddItem( spContainer );

      SerializeNoteRecord( cmo.ID );
    }
    /// <summary>
    /// Serializes NoteRecord.
    /// </summary>
    /// <param name="objId">Object id for the NoteRecord.</param>
    private void SerializeNoteRecord( ushort objId )
    {
      NoteRecord note = ( NoteRecord )
        BiffRecordFactory.GetRecord( TBIFFRecord.Note );

      note.Row        = ( ushort )m_iRow;
      note.Column     = ( ushort )m_iColumn;
      note.AuthorName = Author;
      note.IsVisible  = IsVisible;
      note.ObjId      = objId;

      m_shapes.Worksheet.AddNote( note );
    }
    /// <summary>
    /// Serializes Text ID.
    /// </summary>
    /// <param name="options">MsofbtOPT record to which text ID will be added.</param>
    /// <exception cref="System.ArgumentNullException">
    /// If options argument is NULL.
    /// </exception>
    [ CLSCompliant( false ) ]
    protected void SerializeTextId( MsofbtOPT options )
    {
      if( options == null )
        throw new ArgumentNullException( "options" );

      FOPTE option = new FOPTE();
      option.Id = MsoOptions.TextId;
      option.UInt32Value = ( uint )19990000;
      option.IsValid = false;
      option.IsComplex = false;
      
      options.AddOptionsOrReplace( option );
    }
    /// <summary>
    /// Serializes option with index 344 (this is unknown option).
    /// </summary>
    /// <param name="options">MsofbtOPT record to which text ID will be added.</param>
    /// <exception cref="System.ArgumentNullException">
    /// If options argument is NULL.
    /// </exception>
    [ CLSCompliant( false ) ]
    protected void SerializeOption344( MsofbtOPT options )
    {
      if( options == null )
        throw new ArgumentNullException( "options" );

      FOPTE option = new FOPTE();
      option.Id = ( MsoOptions )344;
      option.UInt32Value = 0;
      option.IsValid = false;
      option.IsComplex = false;
      options.AddOptionsOrReplace( option );
    }
    /// <summary>
    /// Creates default options.
    /// </summary>
    /// <returns>MsofbtOPT record.</returns>
    [ CLSCompliant( false ) ]
    protected override MsofbtOPT CreateDefaultOptions()
    {
      MsofbtOPT result = base.CreateDefaultOptions();
      result.Version = 3;
      result.Instance = 2;
      //SerializeTextId( result );

      //SerializeTextDirection( result );
      //SerializeSizeTextToFit( result );
      //SerializeOption344( result );

      return result;
    }
    /// <summary>
    /// Serialize comment shadow.
    /// </summary>
    /// <param name="option">Represents option holder.</param>
    [ CLSCompliant( false ) ]
    protected override void SerializeCommentShadow( MsofbtOPT option )
    {
      if( option == null )
        throw new ArgumentNullException( "option" );

      ShapeImpl.SerializeForte( option, MsoOptions.ShadowObscured, DEF_COMMENT_SHADOWED );
      ShapeImpl.SerializeForte( option, MsoOptions.ForeShadowColor, 0 );

      int iVal = ( IsVisible )
        ? DEF_COMMENT_SHOW_ALWAYS
        : DEF_COMMENT_NOT_SHOW_ALWAYS;

      ShapeImpl.SerializeForte( option, MsoOptions.CommentShowAlways, iVal );
    }
    /// <summary>
    /// Indicates is can copy current shape.
    /// </summary>
    /// <param name="sourceRec">Represents source range dimension.</param>
    /// <param name="destRec">Represents destination range dimension.</param>
    /// <param name="newPosition">Gets new position of shape.</param>
    /// <returns>Returns true if can copy; otherwise - false.</returns>
    public override bool CanCopyShapesOnRangeCopy( Rectangle sourceRec, Rectangle destRec, out Rectangle newPosition )
    {
      newPosition = new Rectangle( 0, 0, 0 ,0 );
      int iRow = Row;
      int iColumn = Column;

      if( iRow < sourceRec.Top || iRow > sourceRec.Bottom
        || iColumn < sourceRec.Left || iColumn > sourceRec.Right )
      {
        return false;
      }

      newPosition.Y = iRow - sourceRec.Top + destRec.Top;
      newPosition.X = iColumn - sourceRec.Left + destRec.Left;
      newPosition.Width = Width;
      newPosition.Height = Height;

      return true;
    }
    /// <summary>
    /// Copies / moves shape in range copy / move.
    /// </summary>
    /// <param name="sheet">Represents destination sheet.</param>
    /// <param name="destRec">Represents position of .</param>
    /// <param name="bIsCopy">Indicates is copy.</param>
    /// <returns>Returns copied moved shape.</returns>
    public override ShapeImpl CopyMoveShapeOnRangeCopyMove( WorksheetImpl sheet, Rectangle destRec, bool bIsCopy )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );


      WorksheetImpl parentSheet = ParentShapes.Worksheet;

      IRange range = sheet[ destRec.Y, destRec.X ];
      CommentShapeImpl comment = ( CommentShapeImpl )range.AddComment();
      comment.CopyCommentOptions( this, null );
      comment.IsVisible = IsVisible;

      if( !bIsCopy )
        parentSheet.InnerComments.Remove( this );

      comment.FillClientAnchor();
      comment.Height = this.Height;
      comment.Width = this.Width;
      comment.UpdateRightColumn();
      comment.UpdateBottomRow();

      return comment;
    }
    /// <summary>
    /// Updates shape that include not size and not move flags.
    /// </summary>
    /// <param name="bRow">Indicates is row or column to update.</param>
    /// <param name="index">One-based row or column index.</param>
    /// <param name="iCount">Number of inserted/removed rows/column.</param>
    protected override void UpdateNotSizeNotMoveShape( bool bRow, int index, int iCount )
    {
      if( index == 0 ) return;
      int iLast = (bRow) ? BottomRow : RightColumn;
      bool bContinue = index <= iLast;
      index--;

      if( iCount < 0 && ( bRow && index <= m_iRow && index - iCount > m_iRow
        || !bRow && index <= m_iColumn && index - iCount > m_iColumn ) )
      {
        // here we have to remove comment.
        Remove();
      }
      else if( bRow && index <= m_iRow || !bRow && index <= m_iColumn )
      {
        int iOldRow = m_iRow;
        int iOldColumn = m_iColumn;
        int iNewRow = bRow ? iOldRow + iCount : iOldRow;
        int iNewColumn = bRow ? iOldColumn : iOldColumn + iCount;

        m_iRow = iNewRow;
        m_iColumn = iNewColumn;
        ShapesCollection shapes = ( ShapesCollection )m_shapes;
        shapes.InnerComments.ReRegisterOnAccess = true;

        //comment.FillClientAnchor();
        //comment.Height = destRec.Height;
        //comment.Width = destRec.Width;
        //comment.UpdateRightColumn();
        //comment.UpdateBottomRow();
        if (bContinue)
        {
            if (bRow)
            {
                ClientAnchor.TopRow += iCount;
                UpdateBottomRow();
            }
            else
            {
                ClientAnchor.LeftColumn += iCount;
                if (iCount == 1)
                    UpdateRightColumn(iCount);
                else
                    UpdateRightColumn();
            }
        }
      }
    }
    /// <summary>
    /// Removes shapes from collection.
    /// </summary>
    protected override void OnDelete()
    {
      base.OnDelete();
      m_shapes.WorksheetBase.InnerComments.InnerRemove( this );
    }
    protected override void CreateDefaultFillLineFormats()
    {
      m_bSupportOptions = true;
      base.CreateDefaultFillLineFormats();

      Fill.ForeColor = ShapeFillImpl.DEF_COMENT_PARSE_COLOR;
      Line.ForeColor = ColorExtension.Black;

      Fill.BackColor = ColorExtension.Empty;
      Line.BackColor = ColorExtension.Empty;
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Copy comments options.
    /// </summary>
    /// <param name="sourceComment">Represents source comment.</param>
    /// <param name="dicFontIndexes">Represents dictionary with shape indexes.</param>
    public void CopyCommentOptions( CommentShapeImpl sourceComment, Dictionary<int, int> dicFontIndexes )
    {
      if( sourceComment == null )
        throw new ArgumentNullException( "sourceComment" );

      RichTextString rtf = ( RichTextString )RichText;
      RichTextString sourceRtf = ( RichTextString )sourceComment.RichText;
      rtf.CopyFrom( sourceRtf, dicFontIndexes );
      Name = sourceComment.Name;

      if( m_bUpdateLineFill )
      {
        base.CopyFillOptions( sourceComment, dicFontIndexes );
      }
    }
    /// <summary>
    /// This method is called inside of PrepareForSerialization to make shape-dependent preparations.
    /// </summary>
    protected override void OnPrepareForSerialization()
    {
      m_shape = ( MsofbtSp )MsoFactory.GetRecord( MsoRecords.msofbtSp );
      m_shape.Version = DEF_SHAPE_VERSION;
      m_shape.Instance = ShapeInstance;
      m_shape.IsHaveAnchor = true;
      m_shape.IsHaveSpt = true;
    }
    /// <summary>
    /// Parses Note record.
    /// </summary>
    /// <param name="iObjectId">Object ID of note to parse.</param>
    private void ParseNoteRecord( int iObjectId )
    {
      NoteRecord note = ( Worksheet as WorksheetImpl ).GetNoteByObjectIndex( iObjectId );

      if( note == null )
      {
#if !SILVERLIGHT && !WINRT && !WP
        //Debug.Assert( false, "Can't find note for ObjectID: " + iObjectId.ToString() );
#endif
        return;
      }

      Author    = note.AuthorName;
      m_iRow    = note.Row;
      m_iColumn = note.Column;
      IsVisible = note.IsVisible;
    }
    #endregion

  }

  /// <summary>
  /// This class represents comment for multicell range.
  /// </summary>
  public class CommentsRange
    : CommonObject
    , ICommentShape
  {
    #region Class members
    /// <summary>
    /// Parent range.
    /// </summary>
    private IRange m_range;
    /// <summary>
    /// Represents the rich-text string
    /// </summary>
    private IRichTextString m_richTextString;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates instance for specified range.
    /// </summary>
    /// <param name="application">Application object for new object.</param>
    /// <param name="parentRange">Parent range.</param>
    public CommentsRange( IApplication application, IRange parentRange )
      : base( application, parentRange )
    {
      m_range = parentRange;
    }
    #endregion

    #region IComment Members
    /// <summary>
    /// Returns or sets the author of the comment. Read-only String.
    /// </summary>
    public string Author
    {
      get
      {
        IRange[] cells = m_range.Cells;
        string strAuthor = null;
        bool bFirst = true;

        for( int i = 0, len = cells.Length; i < len; i++ )
        {
          if( cells[ i ].Comment == null ) continue;

          if( bFirst )
          {
            strAuthor = cells[ i ].Comment.Author;
            bFirst = false;
          }
          else if( strAuthor != cells[ i ].Comment.Author )
          {
            return null;
          }
        }

        return strAuthor;
      }
    }
    /// <summary>
    /// Determines whether the object is visible. Read / write Boolean.
    /// </summary>
    public bool   IsVisible
    {
      get
      {
        IRange[] cells = m_range.Cells;
        bool bVisible = false;
        bool bFirst = true;

        for( int i = 0, len = cells.Length; i < len; i++ )
        {
          if( cells[ i ].Comment == null ) continue;

          if( bFirst )
          {
            bVisible = cells[ i ].Comment.IsVisible;
            bFirst = false;
          }
          else if( bVisible != cells[ i ].Comment.IsVisible )
          {
            return false;
          }
        }

        return bVisible;
      }
      set
      {
        for( int i = 0, len = m_range.Cells.Length; i < len; i++ )
        {
          m_range.Cells[ i ].AddComment().IsVisible = value;
        }
      }
    }
    /// <summary>
    /// Row of the commented cell. Read-only. 
    /// </summary>
    public int    Row
    {
      get
      {
        IRange[] cells = m_range.Cells;
        int iRow = int.MinValue;
        bool bFirst = true;

        for( int i = 0, len = cells.Length; i < len; i++ )
        {
          if( cells[ i ].Comment == null ) continue;

          if( bFirst )
          {
            iRow = cells[ i ].Comment.Row;
            bFirst = false;
          }
          else if( iRow != cells[ i ].Comment.Row )
          {
            return int.MinValue;
          }
        }

        return iRow;
      }
    }
    /// <summary>
    /// Column of the commented cell. Read-only.
    /// </summary>
    public int    Column
    {
      get
      {
        IRange[] cells = m_range.Cells;
        int iColumn = int.MinValue;
        bool bFirst = true;

        for( int i = 0, len = cells.Length; i < len; i++ )
        {
          if( cells[ i ].Comment == null ) continue;

          if( bFirst )
          {
            iColumn = cells[ i ].Comment.Column;
            bFirst = false;
          }
          else if( iColumn != cells[ i ].Comment.Column )
          {
            return int.MinValue;
          }
        }

        return iColumn;
      }
    }
    /// <summary>
    /// Text of the comment.
    /// </summary>
    public IRichTextString RichText
    {
      get
      {
        m_richTextString = new RTFCommentArray( Application, this );
        return m_richTextString;
      }
      set
      {
          m_richTextString = value;
      }
    }
    /// <summary>
    /// Gets or sets text.
    /// </summary>
    public string Text
    {
      get
      {
        return RichText.Text;
      }
      set
      {
        RichText.Text = value;
      }
    }
    /// <summary>
    /// Indicates whether shape must be moved with cells.
    /// </summary>
    public bool   IsMoveWithCell
    {
      get
      {
        throw new NotImplementedException();
      }
      set
      {
        throw new NotImplementedException();
      }
    }
    /// <summary>
    /// Indicates whether shape must be sized with cells.
    /// </summary>
    public bool  IsSizeWithCell
    {
      get
      {
        throw new NotImplementedException();
      }
      set
      {
        throw new NotImplementedException();
      }
    }
    /// <summary>
    /// True if the size of the specified object is changed automatically
    /// to fit text within its boundaries. Read/write Boolean.
    /// </summary>
    public bool AutoSize
    {
      get
      {
        IRange[] cells = m_range.Cells;
        bool bResult = false;
        bool bFirst = true;

        for( int i = 0, len = cells.Length; i < len; i++ )
        {
          if( cells[ i ].Comment == null ) continue;

          if( bFirst )
          {
            bResult = cells[ i ].Comment.AutoSize;
            bFirst = false;
          }
          else if( bResult != cells[ i ].Comment.AutoSize )
          {
            return false;
          }
        }

        return bResult;
      }
      set
      {
        for( int i = 0, len = m_range.Cells.Length; i < len; i++ )
        {
          m_range.Cells[ i ].AddComment().AutoSize = value;
        }
      }
    }
    #endregion

    #region IComment methods
    /// <summary>
    /// Removes this comment.
    /// </summary>
    public void Remove()
    {
      for( int i = 0,  len = m_range.Cells.Length; i < len; i++ )
      {
        ICommentShape comment = m_range.Cells[ i ].Comment;
        if( comment != null )
        {
          comment.Remove();
        }
      }
    }
    /// <summary>
    /// Scales all comments in the array.
    /// </summary>
    /// <param name="scaleWidth">Width scale in percents.</param>
    /// <param name="scaleHeight">Height scale in percents.</param>
    public void Scale( int scaleWidth, int scaleHeight )
    {
      for( int i = 0,  len = m_range.Cells.Length; i < len; i++ )
      {
        ICommentShape comment = m_range.Cells[ i ].Comment;
        if( comment != null )
        {
          comment.Scale( scaleWidth, scaleHeight );
        }
      }
    }
    #endregion

    #region IShape Members
    /// <summary>
    /// Determines whether the object is visible. Read / write Boolean.
    /// </summary>
    public bool   IsShapeVisible
    {
      get
      {
        IRange[] cells = m_range.Cells;
        bool bVisible = false;
        bool bFirst = true;

        for( int i = 0, len = cells.Length; i < len; i++ )
        {
          if( cells[ i ].Comment == null ) continue;

          if( bFirst )
          {
            bVisible = cells[ i ].Comment.IsShapeVisible;
            bFirst = false;
          }
          else if( bVisible != cells[ i ].Comment.IsShapeVisible )
          {
            return false;
          }
        }

        return bVisible;
      }
      set
      {
        for( int i = 0, len = m_range.Cells.Length; i < len; i++ )
        {

          m_range.Cells[ i ].AddComment().IsShapeVisible = value;
        }
      }
    }
    /// <summary>
    /// Gets or sets height of comment shape
    /// </summary>
    public int Height
    {
      get
      {
        IRange[] cells = m_range.Cells;
        int iResult = int.MinValue;
        bool bFirst = true;

        for( int i = 0, len = cells.Length; i < len; i++ )
        {
          if( cells[ i ].Comment == null ) continue;

          if( bFirst )
          {
            iResult = cells[ i ].Comment.Height;
            bFirst = false;
          }
          else if( iResult != cells[ i ].Comment.Height )
          {
            return int.MinValue;
          }
        }

        return iResult;
      }
      set
      {
        IRange[] cells = m_range.Cells;

        for( int i = 0, len = cells.Length; i < len; i++ )
        {
          if( cells[ i ].Comment == null ) continue;

          cells[ i ].Comment.Height = value;
        }
      }
    }

    /// <summary>
    /// Gets Comments range id.
    /// </summary>
    public int Id
    {
      get
      {
        // TODO:  Add CommentsRange.Id getter implementation
        return 0;
      }
    }

    /// <summary>
    /// Gets or sets left position of the comment
    /// </summary>
    public int Left
    {
      get
      {
        IRange[] cells = m_range.Cells;
        int iResult = int.MinValue;
        bool bFirst = true;

        for( int i = 0, len = cells.Length; i < len; i++ )
        {
          if( cells[ i ].Comment == null ) continue;

          if( bFirst )
          {
            iResult = cells[ i ].Comment.Left;
            bFirst = false;
          }
          else if( iResult != cells[ i ].Comment.Left )
          {
            return int.MinValue;
          }
        }

        return iResult;
      }
      set
      {
        IRange[] cells = m_range.Cells;

        for( int i = 0, len = cells.Length; i < len; i++ )
        {
          if( cells[ i ].Comment == null ) continue;

          cells[ i ].Comment.Left = value;
        }
      }
    }

    /// <summary>
    /// Gets or sets the comment shape name.
    /// </summary>
    public string Name
    {
      get
      {
        IRange[] cells = m_range.Cells;
        string strResult = null;
        bool bFirst = true;

        for( int i = 0, len = cells.Length; i < len; i++ )
        {
          if( cells[ i ].Comment == null ) continue;

          if( bFirst )
          {
            strResult = cells[ i ].Comment.Name;
            bFirst = false;
          }
          else if( strResult != cells[ i ].Comment.Name )
          {
            return null;
          }
        }

        return strResult;
      }
      set
      {
        IRange[] cells = m_range.Cells;

        for( int i = 0, len = cells.Length; i < len; i++ )
        {
          if( cells[ i ].Comment == null ) continue;

          cells[ i ].Comment.Name = value;
        }
      }
    }

    /// <summary>
    /// Gets or sets the Top position of the shape.
    /// </summary>
    public int Top
    {
      get
      {
        IRange[] cells = m_range.Cells;
        int iResult = int.MinValue;
        bool bFirst = true;

        for( int i = 0, len = cells.Length; i < len; i++ )
        {
          if( cells[ i ].Comment == null ) continue;

          if( bFirst )
          {
            iResult = cells[ i ].Comment.Top;
            bFirst = false;
          }
          else if( iResult != cells[ i ].Comment.Top )
          {
            return int.MinValue;
          }
        }

        return iResult;
      }
      set
      {
        IRange[] cells = m_range.Cells;

        for( int i = 0, len = cells.Length; i < len; i++ )
        {
          if( cells[ i ].Comment == null ) continue;

          cells[ i ].Comment.Top = value;
        }
      }
    }

    /// <summary>
    /// Gets or set the width of shape.
    /// </summary>
    public int Width
    {
      get
      {
        IRange[] cells = m_range.Cells;
        int iResult = int.MinValue;
        bool bFirst = true;

        for( int i = 0, len = cells.Length; i < len; i++ )
        {
          if( cells[ i ].Comment == null ) continue;

          if( bFirst )
          {
            iResult = cells[ i ].Comment.Width;
            bFirst = false;
          }
          else if( iResult != cells[ i ].Comment.Width )
          {
            return int.MinValue;
          }
        }

        return iResult;
      }
      set
      {
        IRange[] cells = m_range.Cells;

        for( int i = 0, len = cells.Length; i < len; i++ )
        {
          if( cells[ i ].Comment == null ) continue;

          cells[ i ].Comment.Width = value;
        }
      }
    }

    /// <summary>
    /// Gets shape type.
    /// </summary>
    public ExcelShapeType ShapeType
    {
      get
      {
        return ExcelShapeType.Comment;
      }
    }

    /// <summary>
    /// Gets or sets alternative text.
    /// </summary>
    public string AlternativeText
    {
      get
      {
        IRange[] cells = m_range.Cells;
        string strResult = null;
        bool bFirst = true;

        for( int i = 0, len = cells.Length; i < len; i++ )
        {
          if( cells[ i ].Comment == null ) continue;

          if( bFirst )
          {
            strResult = cells[ i ].Comment.AlternativeText;
            bFirst = false;
          }
          else if( strResult != cells[ i ].Comment.AlternativeText )
          {
            return null;
          }
        }

        return strResult;
      }
      set
      {
        IRange[] cells = m_range.Cells;

        for( int i = 0, len = cells.Length; i < len; i++ )
        {
          if( cells[ i ].Comment == null ) continue;

          cells[ i ].Comment.AlternativeText = value;
        }
      }
    }

    /// <summary>
    /// Represents fill properties. Read-only.
    /// </summary>
    public IFill Fill
    {
      get
      {
        throw new NotSupportedException( "This property doesn't support in this class" );
      }
    }
    /// <summary>
    /// Represents line format properties. Read-only.
    /// </summary>
    public IShapeLineFormat Line
    {
      get
      {
        throw new NotSupportedException("This property doesn't support in this class" );
      }
    }
    /// <summary>
    /// Gets or sets macro associated with this shape
    /// </summary>
    public string OnAction
    {
      get
      {
        throw new NotSupportedException();
      }
      set
      {
        throw new NotSupportedException();
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
            throw new NotSupportedException("This property doesn't support in this class");
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
            throw new NotSupportedException("This property doesn't support in this class");       
        }
    }
    /// <summary>
    /// Returns or sets the rotation of the shape, in degrees.
    /// </summary>
    /// <value></value>
    public int ShapeRotation
    {
        get
        {
            throw new NotSupportedException("This property doesn't support in this class");   
        }
        set
        {
            throw new NotSupportedException("This property doesn't support in this class");   
        }
    }
    /// <summary>
    /// Returns a TextFrame object that contains the 
    /// alignment and anchoring properties for the specified shape. Read-only.
    /// </summary>
    public ITextFrame TextFrame
    {
        get { throw new NotImplementedException("This property doesn't support in this class"); }
    }
    #endregion

    #region ITextBox Members
    /// <summary>
    /// Horizontal alignment of the text.
    /// </summary>
    public ExcelCommentHAlign HAlignment
    {
      get
      {
        IRange[] cells = m_range.Cells;
        ExcelCommentHAlign result = ExcelCommentHAlign.Left;
        bool bFirst = true;

        for( int i = 0, len = cells.Length; i < len; i++ )
        {
          if( cells[ i ].Comment == null )
            continue;

          if( bFirst )
          {
            result = cells[ i ].Comment.HAlignment;
            bFirst = false;
          }
          else if( result != cells[ i ].Comment.HAlignment )
          {
            return ExcelCommentHAlign.Left;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_range.Cells.Length; i < len; i++ )
        {
          m_range.Cells[ i ].AddComment().HAlignment = value;
        }
      }
    }
    /// <summary>
    /// Vertical alignment of the text.
    /// </summary>
    public ExcelCommentVAlign VAlignment
    {
      get
      {
        IRange[] cells = m_range.Cells;
        ExcelCommentVAlign result = ExcelCommentVAlign.Top;
        bool bFirst = true;

        for( int i = 0, len = cells.Length; i < len; i++ )
        {
          if( cells[ i ].Comment == null )
            continue;

          if( bFirst )
          {
            result = cells[ i ].Comment.VAlignment;
            bFirst = false;
          }
          else if( result != cells[ i ].Comment.VAlignment )
          {
            return ExcelCommentVAlign.Top;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_range.Cells.Length; i < len; i++ )
        {
          m_range.Cells[ i ].AddComment().VAlignment = value;
        }
      }
    }
    /// <summary>
    /// Text rotation.
    /// </summary>
    public ExcelTextRotation TextRotation
    {
      get
      {
        IRange[] cells = m_range.Cells;
        ExcelTextRotation result = ExcelTextRotation.LeftToRight;
        bool bFirst = true;

        for( int i = 0, len = cells.Length; i < len; i++ )
        {
          if( cells[ i ].Comment == null )
            continue;

          if( bFirst )
          {
            result = cells[ i ].Comment.TextRotation;
            bFirst = false;
          }
          else if( result != cells[ i ].Comment.TextRotation )
          {
            return ExcelTextRotation.LeftToRight;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_range.Cells.Length; i < len; i++ )
        {
          m_range.Cells[ i ].AddComment().TextRotation = value;
        }
      }
    }
    /// <summary>
    /// Indicates whether comment text is locked.
    /// </summary>
    public bool IsTextLocked
    {
      get
      {
        IRange[] cells = m_range.Cells;
        bool result = false;
        bool bFirst = true;

        for( int i = 0, len = cells.Length; i < len; i++ )
        {
          if( cells[ i ].Comment == null )
            continue;

          if( bFirst )
          {
            result = cells[ i ].Comment.IsTextLocked;
            bFirst = false;
          }
          else if( result != cells[ i ].Comment.IsTextLocked )
          {
            return false;
          }
        }

        return result;
      }
      set
      {
        for( int i = 0, len = m_range.Cells.Length; i < len; i++ )
        {
          m_range.Cells[ i ].AddComment().IsTextLocked = value;
        }
      }
    }

    #endregion
  }
}
