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
  /// Represents worksheet's textbox collection.
  /// </summary>
  public class CheckBoxCollection :
    CollectionBaseEx<object>,
    ICheckBoxes
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
    public CheckBoxCollection( IApplication application, object parent )
      : base( application, parent )
    {
      m_sheet = FindParent( typeof( WorksheetBaseImpl ), true ) as WorksheetBaseImpl;

      if( m_sheet == null )
        throw new ArgumentOutOfRangeException( "parent" );
    }
    /// <summary>
    /// Adds new item to the collection.
    /// </summary>
    /// <param name="checkbox">Checkbox to add.</param>
    public void AddCheckBox( ICheckBoxShape checkbox )
    {
      if( checkbox == null )
        throw new ArgumentNullException( "checkbox" );

      base.Add( checkbox );
    }
    #endregion

    #region ICheckBoxes Members
    /// <summary>
    /// Returns single item from the collection.
    /// </summary>
    /// <param name="index">Index of the item to get.</param>
    /// <returns>Single item from the collection.</returns>
    public ICheckBoxShape this[ int index ]
    {
      get
      {
        return List[ index ] as ICheckBoxShape;
      }
    }
    /// <summary>
    /// Gets single item from the collection.
    /// </summary>
    /// <param name="name">Name of the item to get.</param>
    /// <returns>Single item from the collection.</returns>
    public ICheckBoxShape this[ string name ]
    {
      get
      {
        ICheckBoxShape result = null;

        for( int i = 0, len = Count; i < len; i++ )
        {
          ICheckBoxShape currentShape = this[ i ];

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
    public ICheckBoxShape AddCheckBox( int row, int column, int height, int width )
    {
      CheckBoxShapeImpl checkBoxShape = m_sheet.Shapes.AddCheckBox() as CheckBoxShapeImpl;

      //textBox.Column = column;
      //textBox.Row = row;

      MsofbtClientAnchor clientAnchor = checkBoxShape.ClientAnchor;

      clientAnchor.LeftColumn = column - 1; // make it zero-based
      clientAnchor.TopRow = row - 1;    // make it zero-based
      clientAnchor.RightColumn = column;
      clientAnchor.BottomRow = row;
      clientAnchor.LeftOffset = 0;
      clientAnchor.RightOffset = 0;
      clientAnchor.TopOffset = 0;
      clientAnchor.BottomOffset = 0;

      checkBoxShape.Fill.BackColor = ColorExtension.Empty;
      checkBoxShape.Line.BackColor = ColorExtension.White;
      checkBoxShape.HasLineFormat = false;
      checkBoxShape.HasFill = false;
      checkBoxShape.Width = width;
      checkBoxShape.Height = height;

      //base.Add( textBox );
      return checkBoxShape;
    }
    #endregion
  }
}
