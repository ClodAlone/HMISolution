#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;

using Syncfusion.XlsIO.Implementation.Shapes;
using Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing;
using System.Collections.Generic;

namespace Syncfusion.XlsIO.Implementation.Collections
{
  /// <summary>
  /// Represents worksheet's OptionButton collection.
  /// </summary>
  public class OptionButtonCollection :
   CollectionBaseEx<object>,
    IOptionButtons
  {
    #region Constants
    public const int AverageWidth = 140;
    public const int AverageHeight = 20;
    #endregion

    #region Members
    /// <summary>
    /// Parent worksheet.
    /// </summary>
    private WorksheetBaseImpl m_worksheet;
    #endregion

    #region Methods
    /// <summary>
    /// Initializes new instance of the collection.
    /// </summary>
    /// <param name="application">Application object for the new instance.</param>
    /// <param name="parent">Parent object for the new instance.</param>
    public OptionButtonCollection( IApplication application, object parent )
      : base( application, parent )
    {
      m_worksheet = FindParent( typeof( WorksheetBaseImpl ), true ) as WorksheetBaseImpl;

      if( m_worksheet == null )
        throw new ArgumentOutOfRangeException( "parent" );
    }
    /// <summary>
    /// Adds new item to the collection.
    /// </summary>
    /// <param name="optionButton">OptionButton to add.</param>
    public void AddOptionButton( IOptionButtonShape optionButton )
    {
      if( optionButton == null )
        throw new ArgumentNullException( "option button" );

      AddDefaultEvents( optionButton );
      base.Add( optionButton );
    }
    /// <summary>
    /// Adds default event.
    /// </summary>
    /// <param name="optionButton">OptionButton to add event.</param>
    internal void AddDefaultEvents( IOptionButtonShape optionButton )
    {
      OptionButtonShapeImpl typedButton = ( ( OptionButtonShapeImpl )optionButton );
      typedButton.InvokeEvent = true;
      typedButton.CheckStateChanged += new ValueChangedEventHandler( OptionButtonCheckStateChanged );
      typedButton.LinkedCellValueChanged += new ValueChangedEventHandler( OptionButtonLinkedCellChanged );

    }
    /// <summary>
    /// Raise when the check state changed 
    /// </summary>
    /// <param name="obj">OptionButton </param>
    /// <param name="valueChangedEventArgs">event object </param>
    private void OptionButtonCheckStateChanged( object obj, ValueChangedEventArgs valueChangedEventArgs )
    {
      IOptionButtonShape optionButton = ( IOptionButtonShape )obj;
      List<OptionButtonShapeImpl> currentGroup = new List<OptionButtonShapeImpl>();
      int actualButtonId = (int)(obj as OptionButtonShapeImpl).OldObjId;
      int nextButtonId = actualButtonId;
      int index = 0;
      bool proceed = true;

      while (proceed)
      {
          OptionButtonShapeImpl currentShape = (OptionButtonShapeImpl)this[index];

          if (currentGroup.Contains(currentShape))
              proceed = false;

          if (proceed)
          {
              if (currentShape.OldObjId == nextButtonId)
              {
                  currentGroup.Add(currentShape);
                  nextButtonId = currentShape.NextButtonId;
              }
              index++;
              if (index == Count)
                  index = 0;
          }
      } 

      IRange linkedCell = null;
      int objIndex = 0;
      for( int i = 0, len = Count; i < len; i++ )
      {
        OptionButtonShapeImpl currentShape = ( OptionButtonShapeImpl )this[ i ];
        if (currentGroup.Contains(currentShape))
        {
            currentShape.InvokeEvent = false;
            bool isLinkedCellNotNull = false;

            if ((currentShape.IsFirstButton && currentShape.LinkedCell != null) || (linkedCell != null))
            {
                if (currentShape.IsFirstButton)
                {
                    optionButton = currentShape;
                    linkedCell = null;
                    objIndex = 0;
                }

                if (linkedCell == null)
                    linkedCell = this[i].LinkedCell;

                isLinkedCellNotNull = true;
                currentShape.LinkedCell = linkedCell;
                objIndex++;
            }

            if (currentShape != obj)
            {
                currentShape.CheckState = ExcelCheckState.Unchecked;
            }
            else if (isLinkedCellNotNull && (obj as OptionButtonShapeImpl).CheckState == ExcelCheckState.Checked)
            {
                optionButton.LinkedCell.Number = objIndex;
            }

            currentShape.InvokeEvent = true;
        }
      }
      currentGroup.Clear();
      currentGroup = null;
    }
    /// <summary>
    /// Raise when the Linkeed cell changed 
    /// </summary>
    /// <param name="obj">OptionButton </param>
    /// <param name="valueChangedEventArgs">event object </param>
    private void OptionButtonLinkedCellChanged( object obj, ValueChangedEventArgs valueChangedEventArgs )
    {
      IOptionButtonShape optionButton = ( IOptionButtonShape )obj;
      IRange linkedCell = ( IRange )valueChangedEventArgs.newValue;

      for( int i = 0, len = Count; i < len; i++ )
      {
        OptionButtonShapeImpl currentShape = ( OptionButtonShapeImpl )this[ i ];
        currentShape.InvokeEvent = false;
        currentShape.LinkedCell = linkedCell;

        //this code should be implemented to work properly
        if( currentShape.CheckState == ExcelCheckState.Checked && linkedCell != null )
          linkedCell.Number = i + 1;

        currentShape.InvokeEvent = true;
      }

    }
    /// <summary>
    /// Prepares option button for serialization. We should change the option button
    /// checked status based on the group.
    /// </summary>
    internal void PrepareForSerialization()
    {
        //TODO: This process can be optimized when the GroupBox support is added. 
        List<OptionButtonCollection> optionButtonGroups = GetOptionGroups();
        foreach (OptionButtonCollection optionButtonCollection in optionButtonGroups)
        {
            IRange range=optionButtonCollection[0].LinkedCell;
            int value = (range != null) ? (int)range.Number : optionButtonCollection.Count + 1;
            if (value == 0)
            {
                foreach (OptionButtonShapeImpl optionButton in optionButtonCollection)
                {
                    optionButton.InvokeEvent = false;
                    optionButton.CheckState = ExcelCheckState.Unchecked;
                    optionButton.InvokeEvent = true;
                }
            }
            else if (value>0 && value <= optionButtonCollection.Count)
            {
                OptionButtonShapeImpl optionButton = optionButtonCollection[value - 1] as OptionButtonShapeImpl;
                optionButton.InvokeEvent = false;
                optionButton.CheckState = ExcelCheckState.Checked;
                optionButton.InvokeEvent = true;
            }
        }
    }
      /// <summary>
      /// Get the Option Button collection based on the groups.
      /// </summary>
      /// <returns>Collection of option buttons.</returns>
    internal List<OptionButtonCollection> GetOptionGroups()
    {
        ///TODO: This should be removed when the GroupBox support is implemented.
        List<OptionButtonCollection> optionButtonGroups = new List<OptionButtonCollection>();
        int firstButtonId = -1;
        OptionButtonCollection collection = new OptionButtonCollection(Application, Parent);
        foreach (OptionButtonShapeImpl optionButton in this)
        {
            collection.Add(optionButton);
            if (firstButtonId == -1)
                firstButtonId = optionButton.Index;
            else if (firstButtonId == optionButton.NextButtonId)
            {
                firstButtonId = -1;
                optionButtonGroups.Add(collection);
                collection = new OptionButtonCollection(Application, Parent);
            }
        }
        return optionButtonGroups;
    }
      
    #endregion

    #region IOptionButtons Members
    /// <summary>
    /// Returns single item from the collection.
    /// </summary>
    /// <param name="index">Index of the item to get.</param>
    /// <returns>Single item from the collection.</returns>
    public IOptionButtonShape this[ int index ]
    {
      get
      {
        return List[ index ] as IOptionButtonShape;
      }
    }
    /// <summary>
    /// Gets single item from the collection.
    /// </summary>
    /// <param name="name">Name of the item to get.</param>
    /// <returns>Single item from the collection.</returns>
    public IOptionButtonShape this[ string name ]
    {
      get
      {
        IOptionButtonShape optionButtonShape = null;

        for( int i = 0, len = Count; i < len; i++ )
        {
          IOptionButtonShape currentOptionButtonShape = this[ i ];

          if( currentOptionButtonShape.Name == name )
          {
            optionButtonShape = currentOptionButtonShape;
            break;
          }
        }

        return optionButtonShape;
      }
    }
    /// <summary>
    /// Adds new OptionButton to the collection.
    /// </summary>
    /// <param name="row">Top row for the new shape.</param>
    /// <param name="column">Left column for the new shape.</param>
    /// <param name="height">Height in pixels of the new shape.</param>
    /// <param name="width">Width in pixels of the new shape.</param>
    /// <returns>Newly created TextBox object.</returns>
    public IOptionButtonShape AddOptionButton( int row, int column, int height, int width )
    {

      OptionButtonShapeImpl optionButtonShape = m_worksheet.Shapes.AddOptionButton() as OptionButtonShapeImpl;

      MsofbtClientAnchor clientAnchor = optionButtonShape.ClientAnchor;

      clientAnchor.LeftColumn = column - 1; // make it zero-based
      clientAnchor.TopRow = row - 1;    // make it zero-based
      clientAnchor.RightColumn = column;
      clientAnchor.BottomRow = row;
      clientAnchor.LeftOffset = 0;
      clientAnchor.RightOffset = 0;
      clientAnchor.TopOffset = 0;
      clientAnchor.BottomOffset = 0;

      IFont font = ( ( TextBoxShapeBase )optionButtonShape ).Workbook.CreateFont();
      //font.FontName = ApplicationImpl.DEF_DEFAULT_FONT;
      //font.Size = ApplicationImpl.DEF_STANDARD_FONT_SIZE;
      //font.Color = ExcelKnownColors.Black;
      optionButtonShape.Fill.BackColor = ColorExtension.Empty;
      optionButtonShape.Fill.ForeColor = ColorExtension.Empty;
      optionButtonShape.Line.BackColor = ColorExtension.White;
      optionButtonShape.HasLineFormat = false;
      optionButtonShape.Width = width;
      optionButtonShape.Height = height;
      if( Count == 1 )
        optionButtonShape.IsFirstButton = true;
      // TODO: should we call some update here?

      //base.Add( textBox );
      return optionButtonShape;
      //throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Adds Option button default Dimension
    /// </summary>
    /// <returns>returns option button shape</returns>
    public IOptionButtonShape AddOptionButton()
    {
      return AddOptionButton( 10, 10, AverageHeight, AverageWidth );
    }
    /// <summary>
    /// Adds the Shape with default size
    /// </summary>
    /// <param name="row">Top row for the new shape.</param>
    /// <param name="column">Left column for the new shape</param>
    /// <returns></returns>
    public IOptionButtonShape AddOptionButton( int row, int column )
    {
      return AddOptionButton( row, column, AverageHeight, AverageWidth );
    }
    #endregion
  }
}
