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
using System.IO;
using System.Diagnostics;

using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing;
using Syncfusion.XlsIO.Parser.Biff_Records.ObjRecords;

using Syncfusion.XlsIO.Implementation.Shapes;
using System.Collections.Generic;
using Syncfusion.XlsIO.Drawing;

#if ( WINRT )
using Windows.UI;
using Syncfusion.XlsIO;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
using System.Drawing;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
using System.Drawing;
#elif !(WINRT )
using System.Drawing;
using System.Drawing.Imaging;
#endif

#endregion

namespace Syncfusion.XlsIO.Implementation.Collections
{
  /// <summary>
  /// Collection of worksheet's shapes.
  /// </summary>
  public class ShapesCollection
    : ShapeCollectionBase
    , IShapes
    , IParentApplication
  {
    #region Constants
    /// <summary>
    /// Default start of the chart shape name.
    /// </summary>
    public const string DefaultChartNameStart = "Chart ";
    /// <summary>
    /// Default start of the textbox shape name.
    /// </summary>
    public const string DefaultTextBoxNameStart = "TextBox ";
    /// <summary>
    /// Default start of the checkbox shape name.
    /// </summary>
    public const string DefaultCheckBoxNameStart = "CheckBox ";
    /// Default start of the OptionButton shape name.
    /// </summary>
    public const string DefaultOptionButtonNameStart = "Option Button ";
    /// <summary>
    /// Default start of the combobox shape name.
    /// </summary>
    public const string DefaultComboBoxNameStart = "Drop Down ";
    /// <summary>
    /// Default start of the picture shape name.
    /// </summary>
    public const string DefaultPictureNameStart = "Picture ";
    #endregion

    #region Class members
    //    /// <summary>
//    /// String for clone operation.
//    /// </summary>
//    private RichTextString m_emptyRichString;
    /// <summary>
    /// Collection that contains all comments on the sheet.
    /// </summary>
    private CommentsCollection    m_comments;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    /// <param name="parent"></param>
    public ShapesCollection( IApplication application, object parent )
      : base( application, parent )
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    /// <param name="parent"></param>
    /// <param name="container"></param>
    /// <param name="options">Parse options.</param>
    [ CLSCompliant( false ) ]
    public ShapesCollection( IApplication application, object parent, MsofbtSpgrContainer container
      , ExcelParseOptions options )
      : base( application, parent, container, options )
    {
    }
    /// <summary>
    /// Initializes collection
    /// </summary>
    protected override void InitializeCollection()
    {
      base.InitializeCollection();

//      if( m_emptyRichString == null )
//      {
//        m_emptyRichString = new RichTextString( Application, m_sheet.ParentWorkbook );//this );
//      }

      if( m_sheet is WorksheetImpl )
      {
        m_comments = new CommentsCollection( Application, this );
      }
    }

    #endregion

    #region Class properties
    /// <summary>
    /// Returns collection of all comments in the worksheet. Read-only.
    /// </summary>
    public IComments Comments
    {
      get
      {
        return m_comments;
      }
    }
    /// <summary>
    /// Returns collection of all comments in the worksheet. Read-only.
    /// </summary>
    public CommentsCollection InnerComments
    {
      get
      {
        return m_comments;
      }
    }
    /// <summary>
    /// Code of the Biff record in which all data is stored. Read-only.
    /// </summary>
    public override TBIFFRecord RecordCode
    {
      get
      {
        return TBIFFRecord.MSODrawing;
      }
    }
    /// <summary>
    /// Returns shared shape data for all shapes in this collection. Read-only.
    /// </summary>
    public override WorkbookShapeDataImpl ShapeData
    {
      get
      {
        return Workbook.ShapesData;
      }
    }
    #endregion

    #region IShapes Members
    /// <summary>
    /// Adds picture.
    /// </summary>
    /// <param name="image">Image to adding.</param>
    /// <param name="pictureName">Name of picture.</param>
    /// <param name="imageFormat">Image format.</param>
    /// <returns>Returns IPicture object that include picture.</returns>
    public IPictureShape  AddPicture( Image image, string pictureName, ExcelImageFormat imageFormat )
    {
      int id = ShapeData.AddPicture( image, imageFormat, pictureName );
      IPictureShape picture = AddPicture( id, pictureName );
#if !SILVERLIGHT && !WINRT && !WP
      picture.Height = ( int )Math.Round( image.Height * ApplicationImpl.ConvertToPixels( 1, MeasureUnits.Inch ) / image.VerticalResolution );
      picture.Width = ( int )Math.Round( image.Width * ApplicationImpl.ConvertToPixels( 1, MeasureUnits.Inch ) / image.HorizontalResolution );
#else
      picture.Height = image.Height;
      picture.Width = image.Width;
#endif
      picture.Name = pictureName;

      return picture;

    }
#if !(WINRT )
    /// <summary>
    /// Adds new image to the collection.
    /// </summary>
    /// <param name="fileName">File name with the image.</param>
    /// <returns>Newly created picture shape.</returns>
    public IPictureShape  AddPicture( string fileName )
    {
      Image bmp = Image.FromFile( fileName );

      return AddPicture( bmp, Path.GetFileNameWithoutExtension( fileName ), ExcelImageFormat.Original );
    }
#endif
    /// <summary>
    /// Adds new Comment shape to the collection.
    /// </summary>
    /// <param name="commentText">Text of the comment.</param>
    /// <returns>Newly added shape.</returns>
    public ICommentShape  AddComment( string commentText )
    {
      return AddComment( commentText, true );
    }
    /// <summary>
    /// Adds new Comment shape to the collection.
    /// </summary>
    /// <param name="commentText">Text of the comment.</param>
    /// <param name="bIsParseOptions">Indicates is parse comment fill line options.</param>
    /// <returns>Newly added shape.</returns>
    public ICommentShape  AddComment( string commentText, bool bIsParseOptions )
    {
      CommentShapeImpl result = AppImplementation.CreateCommentShapeImpl( this, bIsParseOptions );
      result.RichText.Text = commentText;
      
      //m_comments.AddComment( result );
      return AddShape( result ) as ICommentShape;
    }
    /// <summary>
    /// Adds comment with empty text to the collection.
    /// </summary>
    /// <returns>Added comment.</returns>
    public ICommentShape  AddComment()
    {
      return AddComment( string.Empty );
    }
    /// <summary>
    /// Adds new chart shape to the collection.
    /// </summary>
    /// <returns>Newly added shape.</returns>
    public IChartShape    AddChart()
    {
      ChartShapeImpl result = new ChartShapeImpl( Application, this );
      result.Name = GenerateDefaultName( List, DefaultChartNameStart );

      AddShape( result );

      return result;
    }
    /// <summary>
    /// Adds new textbox shape to the collection.
    /// </summary>
    /// <returns>Newly added shape.</returns>
    public ITextBoxShapeEx AddTextBox()
    {
      TextBoxShapeImpl result = AppImplementation.CreateTextBoxShapeImpl( this, (m_sheet as WorksheetImpl));
      //result.RichText.Text = commentText;

      //m_comments.AddComment( result );
      AddShape( result );
      m_sheet.TypedTextBoxes.AddTextBox( result );
      result.Name = GenerateDefaultName( this, DefaultTextBoxNameStart );
      return result;
    }
    /// <summary>
    /// Adds new checkbox shape to the collection.
    /// </summary>
    /// <returns>Newly added shape.</returns>
    public ICheckBoxShape AddCheckBox()
    {
      CheckBoxShapeImpl result = AppImplementation.CreateCheckBoxShapeImpl( this );
      AddShape( result );
      m_sheet.TypedCheckBoxes.AddCheckBox( result );
      result.Name = GenerateDefaultName( this, DefaultCheckBoxNameStart );
      return result;
    }
    /// <summary>
    /// Adds new OptionButton shape to the collection.
    /// </summary>
    /// <returns>Newly added shape.</returns>
    public IOptionButtonShape AddOptionButton()
    {
      OptionButtonShapeImpl result = AppImplementation.CreateOptionButtonShapeImpl( this );
      AddShape( result );
      m_sheet.TypedOptionButtons.AddOptionButton( result );
      result.Name = GenerateDefaultName( this, DefaultOptionButtonNameStart );

      return result;
    }
    /// <summary>
    /// Adds new checkbox shape to the collection.
    /// </summary>
    /// <returns>Newly added shape.</returns>
    public IComboBoxShape AddComboBox()
    {
      ComboBoxShapeImpl result = AppImplementation.CreateComboBoxShapeImpl( this );
      AddShape( result );
      m_sheet.TypedComboBoxes.AddComboBox( result );
      result.Name = GenerateDefaultName( this, DefaultComboBoxNameStart );
      return result;
    }
    public void RegenerateComboBoxNames()
    {
      for( int i = 0, len = Count; i < len; i++ )
      {
        ComboBoxShapeImpl combo = this[ i ] as ComboBoxShapeImpl;

        if( combo != null )
        {
          string name = combo.Name;

          if( name == null || name.Length == 0 )
            combo.Name = GenerateDefaultName( this, DefaultComboBoxNameStart );
        }
        ShapeImpl shapeImpl = this[i] as CommentShapeImpl;
        if (shapeImpl != null)
        {
            shapeImpl.CheckLeftOffset();
        }
      }
    }
    #endregion

    #region Parse methods
    /// <summary>
    /// Creates new shape to object to this collection.
    /// </summary>
    /// <param name="objType">Object type to create.</param>
    /// <param name="shapeContainer">Shape container.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="subRecords">Subrecords of the shape's OBJRecord.</param>
    /// <param name="cmoIndex">Index to the cmo record inside subrecords.</param>
    /// <returns>Created shape.</returns>
    [CLSCompliant( false )]
    protected override ShapeImpl CreateShape( TObjType objType, MsofbtSpContainer shapeContainer,
      ExcelParseOptions options, List<ObjSubRecord> subRecords, int cmoIndex )
    {
      ShapeImpl newShape = null;

      switch( objType )
      {
        case TObjType.otPicture:
          //if( subRecords != null )
          //  newShape = ChoosePictureShape( shapeContainer, options, subRecords, cmoIndex );

          if( newShape == null )
          {
            newShape = new BitmapShapeImpl( Application, this, shapeContainer );
            ( m_sheet.Pictures as PicturesCollection ).AddPicture( newShape as IPictureShape );
          }
          break;

        case TObjType.otComment:
          //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, WorksheetBase.Name, "Worksheet with comment" );
          newShape = AppImplementation.CreateCommentShapeImpl( this, shapeContainer, options );
          m_comments.AddComment( ( ICommentShape )newShape );
          break;

        case TObjType.otChart:
          newShape = new ChartShapeImpl( Application, this, shapeContainer, options );
          string strName = newShape.Name;

          if( strName == null || strName.Length == 0 )
            newShape.Name = GenerateDefaultName( this, DefaultChartNameStart );
          break;

        case TObjType.otText:
          TextBoxShapeImpl textBox = new TextBoxShapeImpl( Application, this, shapeContainer, options );
          m_sheet.TypedTextBoxes.AddTextBox( textBox );
          newShape = textBox;
          break;

        case TObjType.otCheckBox:
          CheckBoxShapeImpl checkBox = new CheckBoxShapeImpl( Application, this, shapeContainer, options );
          m_sheet.TypedCheckBoxes.AddCheckBox( checkBox );
          newShape = checkBox;
          break;

        case TObjType.otOptionBtn:
          OptionButtonShapeImpl optionButton = new OptionButtonShapeImpl( Application, this, shapeContainer, options,cmoIndex );
          m_sheet.TypedOptionButtons.AddOptionButton( optionButton );
          newShape = optionButton;
          break;

        case TObjType.otComboBox:
          ComboBoxShapeImpl comboBox = new ComboBoxShapeImpl( Application, this, shapeContainer, options, subRecords );
          m_sheet.TypedComboBoxes.AddComboBox( comboBox );
          newShape = comboBox;
          break;
      }

      return newShape;
    }

    private ShapeImpl ChoosePictureShape( MsofbtSpContainer shapeContainer,
      ExcelParseOptions options, List<ObjSubRecord> subRecords, int cmoIndex )
    {
      ShapeImpl result = null;
      for( int i = cmoIndex, len = subRecords.Count; i < len; i++ )
      {
        //ftPictFmla
        ObjSubRecord subRecord = ( ObjSubRecord )subRecords[ i ];

        if( subRecord.Type == TObjSubRecordType.ftPictFmla )
        {
          ftPictFmla formula = ( ftPictFmla )subRecord;

          // Create control...
          switch( formula.Formula )
          {
            // TODO: replace text constants.
            case TextBoxShapeImpl.EmbedString:
              TextBoxShapeImpl textbox = new TextBoxShapeImpl( Application, this,
                shapeContainer, options );
              m_sheet.TypedTextBoxes.AddTextBox( textbox );
              result = textbox;
              break;
          }

          break;
        }
      }

      return result;
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Adds FormControlShape into collection.
    /// </summary>
    /// <returns>Newly created shape.</returns>
    public FormControlShapeImpl AddFormControlShape()
    {
      FormControlShapeImpl result = new FormControlShapeImpl( Application, this );
      AddShape( result );

      return result;
    }
    /// <summary>
    /// Removes comment from internal collections.
    /// </summary>
    /// <param name="comment"></param>
    public void InnerRemoveComment( ICommentShape comment )
    {
      if( comment == null )
        throw new ArgumentNullException( "comment" );

      //m_comments.Remove( comment );
      InnerList.Remove( comment );
    }
    /// <summary>
    /// Checks whether it is possible insert row or column into iIndex.
    /// </summary>
    /// <param name="iIndex">Index of row to insert.</param>
    /// <param name="iCount">Number of rows to insert.</param>
    /// <param name="bRow">Indicates whether rows are inserted.</param>
    /// <param name="iMaxIndex">Maximum possible index.</param>
    /// <returns>True if it is possible to insert row or column.</returns>
    public bool CanInsertRowColumn( int iIndex, int iCount, bool bRow, int iMaxIndex )
    {
      for( int i = Count - 1; i >= 0; i-- )
      {
        ShapeImpl shape = ( ShapeImpl )InnerList[ i ];

        if( !shape.CanInsertRowColumn( iIndex, iCount, bRow, iMaxIndex ) )
          return false;
      }

      return true;
    }
    /// <summary>
    /// Updates shapes position on insert or remove row in worksheet.
    /// </summary>
    /// <param name="iIndex">Represents row \ column index.</param>
    /// <param name="iCount">Represents count.</param>
    /// <param name="bRow">Indicates is row.</param>
    /// <param name="bRemove">Indicates is remove or insert.</param>
    public void InsertRemoveRowColumn( int iIndex, int iCount, bool bRow, bool bRemove )
    {
      for( int i = Count - 1; i >= 0; i-- )
      {
        ShapeImpl shape = ( ShapeImpl )InnerList[ i ];

        if( bRemove )
        {
          shape.RemoveRowColumn( iIndex, iCount, bRow );
        }
        else
        {
          shape.InsertRowColumn( iIndex, iCount, bRow );
        }
      }

      WorksheetImpl sheet = Worksheet;

      if( sheet != null )
      {
        AutoFiltersCollection autofilters = ( AutoFiltersCollection )sheet.AutoFilters;

        if( autofilters != null )
        {
          autofilters.UpdateFilterRange();
        }
      }
    }
    /// <summary>
    /// Adds picture to the collection by blipId.
    /// </summary>
    /// <param name="iBlipId">Picture id.</param>
    /// <param name="strPictureName">Picture name.</param>
    /// <returns>Newly created picture object.</returns>
    public BitmapShapeImpl AddPicture( int iBlipId, string strPictureName )
    {
      BitmapShapeImpl newShape = new BitmapShapeImpl( Application, this );
      //newShape.FileName = Path.GetFileNameWithoutExtension( pictureName );
      newShape.FileName = strPictureName;
      newShape.ShapeType = ExcelShapeType.Picture;
      newShape.BlipId = ( uint )iBlipId;

      base.Add( newShape );

      newShape.IsSizeWithCell = false;
      newShape.IsMoveWithCell = true;

      ( m_sheet.Pictures as PicturesCollection ).AddPicture( newShape );

      return newShape;
    }
    /// <summary>
    /// Adds picture to collections.
    /// </summary>
    /// <param name="shape">Represents picture to add.</param>
    public void AddPicture( BitmapShapeImpl shape )
    {
      if( shape == null )
        throw new ArgumentNullException( "shape" );

      base.Add( shape );

      ( m_sheet.Pictures as PicturesCollection ).AddPicture( shape );
    }
    /// <summary>
    /// Updates formulas after copy operation.
    /// </summary>
    /// <param name="iCurIndex">Current worksheet index.</param>
    /// <param name="iSourceIndex">Source worksheet index.</param>
    /// <param name="sourceRect">Source rectangle.</param>
    /// <param name="iDestIndex">Destination worksheet index.</param>
    /// <param name="destRect">Destination rectangle.</param>
    public void UpdateFormula( int iCurIndex, int iSourceIndex,
      Rectangle sourceRect, int iDestIndex, Rectangle destRect )
    {
      IList<IShape> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        ShapeImpl shape = ( ShapeImpl )list[ i ];
        shape.UpdateFormula( iCurIndex, iSourceIndex, sourceRect, iDestIndex, destRect );
      }
    }
    /// <summary>
    /// Creates copy of the collection.
    /// </summary>
    /// <param name="parent">Parent object for the new collection.</param>
    /// <returns>Copy of the collection.</returns>
    public override object Clone( object parent )
    {
      if( parent == null )
        throw new ArgumentNullException( "parent" );

      ShapesCollection result = ( ShapesCollection )base.Clone( parent );
      //result.m_emptyRichString = m_emptyRichString.Clone( result );

      if( m_comments != null )
        result.m_comments = ( CommentsCollection )this.m_comments.Clone( result );
          //result.m_comments = new CommentsCollection(Application, this);

      List<IShape> list = result.InnerList;
       ///To Do:WorkSheet clone creates duplicate comments from the below line need to check
      //for( int i = 0, len = Count; i < len; i++ )
      //{
      //  ShapeImpl shape = ( ShapeImpl )list[ i ];
      //  shape.RegisterInSubCollection();
      //}

      return result;
    }
    /// <summary>
    /// Copies or moves shape on range copy / move.
    /// </summary>
    /// <param name="destSheet">Represents destination sheet for shape.</param>
    /// <param name="rec">Represents range dimension.</param>
    /// <param name="recDest">Represents destination rectangle</param>
    /// <param name="bIsCopy">Indicates is copy or move.</param>
    public void CopyMoveShapeOnRangeCopy( WorksheetImpl destSheet, Rectangle rec, Rectangle recDest
      , bool bIsCopy )
    {
      if( destSheet == null )
        throw new ArgumentNullException( "destSheet" );

      IList<IShape> list = List;

      for( int i = list.Count - 1; i >= 0; i-- )
      {
        ShapeImpl shape = ( ShapeImpl )list[ i ];
        Rectangle newPos;
        bool bUpdatePos = shape.CanCopyShapesOnRangeCopy( rec, recDest, out newPos );

        if( bUpdatePos )
          shape.CopyMoveShapeOnRangeCopyMove( destSheet, newPos, bIsCopy );
      }
    }
    /// <summary>
    /// Removes all shapes that are out of new dimensions.
    /// </summary>
    /// <param name="version">Version to set.</param>
    public void SetVersion( ExcelVersion version )
    {
      int iRow;
      int iColumn;

      UtilityMethods.GetMaxRowColumnCount( out iRow, out iColumn, version );

      for( int i = Count - 1; i >= 0; i-- )
      {
        ShapeImpl shape = ( ShapeImpl )this[ i ];

        if( shape.LeftColumn > iColumn || shape.RightColumn > iColumn ||
          shape.TopRow > iRow || shape.BottomRow > iRow )
        {
          shape.Remove();
        }
        else if( shape.Name == null || shape.Name.Length == 0 )
        {
          shape.GenerateDefaultName();
        }
        //else if( shape is ChartShapeImpl )
        //{
        //  shape.Remove();
        //}
      }
    }
    /// <summary>
    /// Updates indexes to named ranges.
    /// </summary>
    /// <param name="arrNewIndex">New indexes.</param>
    internal void UpdateNamedRangeIndexes( int[] arrNewIndex )
    {
      for( int i = 0, len = Count; i < len; i++ )
      {
        ShapeImpl shape = this[ i ] as ShapeImpl;
        shape.UpdateNamedRangeIndexes( arrNewIndex );
      }
    }
    /// <summary>
    /// Updates indexes to named ranges.
    /// </summary>
    /// <param name="dicNewIndex">New indexes.</param>
    internal void UpdateNamedRangeIndexes( IDictionary<int, int> dicNewIndex )
    {
      for( int i = 0, len = Count; i < len; i++ )
      {
        ShapeImpl shape = this[ i ] as ShapeImpl;
        shape.UpdateNamedRangeIndexes( dicNewIndex );
      }
    }
    /// <summary>
    /// Gets shape by its id.
    /// </summary>
    /// <param name="id">Shape's id to locate.</param>
    public IShape GetShapeById( int id )
    {
      ShapeImpl result = null;

      for( int i = 0, len = Count; i < len; i++ )
      {
        ShapeImpl currentShape = ( ShapeImpl )this[ i ];

        if( currentShape.ShapeId == id )
        {
          result = currentShape;
          break;
        }
      }

      return result;
    }
    #endregion

    /// <summary>
    /// Add AutoShapes into collection
    /// </summary>
    /// <param name="autoShapeType">AutoShapeType</param>
    /// <param name="topRow">TopRow</param>
    /// <param name="leftColumn">LeftColumn</param>
    /// <param name="height">Height</param>
    /// <param name="width">Width</param>
    /// <returns></returns>
    public IShape AddAutoShapes(AutoShapeType autoShapeType, int topRow, int leftColumn, int height, int width)
    {
        return this.AddAutoShapes(autoShapeType, topRow - 1, 0, leftColumn - 1, 0, height, width);
    }
    /// <summary>
    /// Add AutoShapes into collection
    /// </summary>
    /// <param name="autoShapeType">AutoShapeType</param>
    /// <param name="topRow">TopRow</param>
    /// <param name="top">TopRowOffset</param>
    /// <param name="leftColumn">LeftColumn</param>
    /// <param name="left">LeftColumnOffset</param>
    /// <param name="height">Height</param>
    /// <param name="width">Width</param>
    /// <returns></returns>
    internal IShape AddAutoShapes(AutoShapeType autoShapeType, int topRow, int top, int leftColumn, int left, int height, int width)
    {
       
        AutoShapeImpl autoShapeImpl = new AutoShapeImpl(Application, this);
        WorksheetImpl sheet = Application.ActiveSheet as WorksheetImpl;
        autoShapeImpl.CreateShape(autoShapeType, sheet);
        autoShapeImpl.ShapeExt.IsCreated = true;
        autoShapeImpl.ShapeExt.ClientAnchor.SetAnchor(topRow, top, leftColumn, left, height, width);
        AddShape(autoShapeImpl);
        return autoShapeImpl;
    }

  }
}
