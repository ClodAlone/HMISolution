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

namespace Syncfusion.XlsIO.Implementation.Collections
{
  /// <summary>
  /// This class represents collection of all combo box inside single worksheet.
  /// </summary>
  public class ComboBoxCollection :
    CollectionBaseEx<IComboBoxShape>,
    IComboBoxes
  {
    #region Members
    /// <summary>
    /// Parent worksheet.
    /// </summary>
    private WorksheetBaseImpl m_sheet;
    #endregion

    #region IComboBoxes Members
    /// <summary>
    /// Adds new item to the collection.
    /// </summary>
    /// <param name="row">One-based row index of the top-left corner of the new item.</param>
    /// <param name="column">One-based column index of the top-left corner of the new item.</param>
    /// <param name="height">Height in pixels of the new item.</param>
    /// <param name="width">Width in pixels of the new item.</param>
    /// <returns>Newly added item.</returns>
    public IComboBoxShape AddComboBox( int row, int column, int height, int width )
    {
      ComboBoxShapeImpl result = m_sheet.Shapes.AddComboBox() as ComboBoxShapeImpl;

      //textBox.Column = column;
      //textBox.Row = row;

      MsofbtClientAnchor clientAnchor = result.ClientAnchor;

      clientAnchor.LeftColumn = column - 1; // make it zero-based
      clientAnchor.TopRow = row - 1;    // make it zero-based
      clientAnchor.RightColumn = column;
      clientAnchor.BottomRow = row;
      clientAnchor.LeftOffset = 0;
      clientAnchor.RightOffset = 0;
      clientAnchor.TopOffset = 0;
      clientAnchor.BottomOffset = 0;

      result.Width = width;
      result.Height = height;
      result.EvaluateTopLeftPosition();

      // TODO: should we call some update here?

      //base.Add( textBox );
      return result;
    }
    /// <summary>
    /// Gets single item from the collection.
    /// </summary>
    /// <param name="name">Name of the item to get.</param>
    /// <returns>Single item from the collection.</returns>
    public IComboBoxShape this[ string name ]
    {
      get
      {
        IComboBoxShape result = null;

        for( int i = 0, len = Count; i < len; i++ )
        {
          IComboBoxShape currentShape = this[ i ];

          if( currentShape.Name == name )
          {
            result = currentShape;
            break;
          }
        }

        return result;
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Initializes new instance of the collection.
    /// </summary>
    /// <param name="application">Application object for the new collection.</param>
    /// <param name="parent">Parent object for the new collection.</param>
    public ComboBoxCollection( IApplication application, object parent )
      : base( application, parent )
    {
      m_sheet = FindParent( typeof( WorksheetBaseImpl ), true ) as WorksheetBaseImpl;

      if( m_sheet == null )
        throw new ArgumentOutOfRangeException( "parent" );
    }
    /// <summary>
    /// Adds new item to the collection.
    /// </summary>
    /// <param name="combobox">Combobox to add.</param>
    public void AddComboBox( IComboBoxShape combobox )
    {
      if( combobox == null )
        throw new ArgumentNullException( "combobox" );

      base.Add( combobox );
    }
    #endregion
  }
}
