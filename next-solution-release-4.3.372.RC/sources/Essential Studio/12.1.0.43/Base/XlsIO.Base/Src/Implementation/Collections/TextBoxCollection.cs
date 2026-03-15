#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using Syncfusion.XlsIO.Implementation.Shapes;
using Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing;
using Syncfusion.XlsIO.Implementation.Charts;
using Syncfusion.XlsIO.Parser.Biff_Records.Charts;

namespace Syncfusion.XlsIO.Implementation.Collections
{
  /// <summary>
  /// Represents worksheet's textbox collection.
  /// </summary>
  public class TextBoxCollection :
    CollectionBaseEx<ITextBoxShape>,
    ITextBoxes
  {
    #region Members
    /// <summary>
    /// Parent worksheet.
    /// </summary>
    private WorksheetBaseImpl m_sheet;
    #endregion

    #region Methods
    /// <summary>
    /// Initializes new instance of the collection.
    /// </summary>
    /// <param name="application">Application object for the new instance.</param>
    /// <param name="parent">Parent object for the new instance.</param>
    public TextBoxCollection( IApplication application, object parent )
      : base( application, parent )
    {
      m_sheet = FindParent( typeof( WorksheetBaseImpl ), true ) as WorksheetBaseImpl;

      if( m_sheet == null )
        throw new ArgumentOutOfRangeException( "parent" );
    }
    /// <summary>
    /// Adds new item to the collection.
    /// </summary>
    /// <param name="textbox">Textbox to add.</param>
    public void AddTextBox( ITextBoxShape textbox )
    {
      if( textbox == null )
        throw new ArgumentNullException( "textbox" );

      base.Add( textbox );
    }
    #endregion

    #region ITextBoxes Members
    /// <summary>
    /// Returns single item from the collection.
    /// </summary>
    /// <param name="index">Index of the item to get.</param>
    /// <returns>Single item from the collection.</returns>
    public ITextBoxShape this[ int index ]
    {
      get
      {
        return List[ index ] as ITextBoxShape;
      }
    }
    /// <summary>
    /// Gets single item from the collection.
    /// </summary>
    /// <param name="name">Name of the item to get.</param>
    /// <returns>Single item from the collection.</returns>
    public ITextBoxShape this[ string name ]
    {
      get
      {
        ITextBoxShape result = null;

        for( int i = 0, len = Count; i < len; i++ )
        {
          ITextBoxShape currentShape = this[ i ];

          if( currentShape.Name == name )
          {
            result = currentShape;
            break;
          }
        }

        return result;
      }
    }
    /// <summary>
    /// Adds new textbox to the collection.
    /// </summary>
    /// <param name="row">Top row for the new shape.</param>
    /// <param name="column">Left column for the new shape.</param>
    /// <param name="height">Height in pixels of the new shape.</param>
    /// <param name="width">Width in pixels of the new shape.</param>
    /// <returns>Newly created TextBox object.</returns>
    public ITextBoxShape AddTextBox( int row, int column, int height, int width )
    {
      TextBoxShapeImpl textBox = m_sheet.Shapes.AddTextBox() as TextBoxShapeImpl;

      //textBox.Column = column;
      //textBox.Row = row;

      MsofbtClientAnchor clientAnchor = textBox.ClientAnchor;

      clientAnchor.LeftColumn = column - 1; // make it zero-based
      clientAnchor.TopRow = row - 1;    // make it zero-based
      clientAnchor.RightColumn = column;
      clientAnchor.BottomRow = row;
      clientAnchor.LeftOffset = 0;
      clientAnchor.RightOffset = 0;
      clientAnchor.TopOffset = 0;
      clientAnchor.BottomOffset = 0;

      textBox.Width = width;
      textBox.Height = height;

      // TODO: should we call some update here?
      ChartImpl chartImpl = Parent as ChartImpl;
      if (chartImpl != null)
      {
          ChartParentAxisImpl parentAxisImpl=(chartImpl.PrimaryParentAxis!=null)?
              chartImpl.PrimaryParentAxis:
              chartImpl.SecondaryParentAxis;
          if (parentAxisImpl != null)
          {
              ChartAxisParentRecord parentAxisRecord = parentAxisImpl.ParentAxisRecord;
              if (parentAxisRecord.XAxisLength == 0)
              {
                  parentAxisRecord.TopLeftX = ChartImpl.DefaultPlotAreaX; ;
                  parentAxisRecord.TopLeftY = ChartImpl.DefaultPlotAreaY;
                  parentAxisRecord.XAxisLength = ChartImpl.DefaultPlotAreaXLength;
                  parentAxisRecord.YAxisLength = ChartImpl.DefaultPlotAreaYLength;
              }
          }
      }
      //List.Add( textBox );
      return textBox;
      //throw new Exception( "The method or operation is not implemented." );
    }
    #endregion
  }
}
