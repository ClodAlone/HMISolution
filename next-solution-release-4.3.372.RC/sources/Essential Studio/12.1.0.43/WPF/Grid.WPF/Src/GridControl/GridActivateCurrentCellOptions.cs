#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System.Windows;
using System;
namespace Syncfusion.Windows.Controls.Grid
{
    /// <summary>
    /// Provides a way to activate the various options for current cell.
    /// </summary>
    public class GridActivateCurrentCellOptions
    {
        public static readonly GridActivateCurrentCellOptions Empty = new GridActivateCurrentCellOptions(true);

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="isEmpty">Marks the current cell as empty.</param>
        GridActivateCurrentCellOptions(bool isEmpty)
        {
            this.isEmpty = isEmpty;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public GridActivateCurrentCellOptions()
        {
        }

        private bool _isexternalmove = false;
        internal bool IsExternalMove
        {
            get { return _isexternalmove; }
            set { _isexternalmove = value; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="setCurrentCellOptions">A reference to <see cref="GridSetCurrentCellOptions"/> class that defines the options for GridCurrentCell.MoveTo method.</param>
        public GridActivateCurrentCellOptions(GridSetCurrentCellOptions setCurrentCellOptions)
        {
            this.setCurrentCellOptions = setCurrentCellOptions;
        }

        bool isEmpty = false;
        /// <summary>
        /// Gets or sets a value that indicates whether the current cell is empty or not.
        /// </summary>
        public bool IsEmpty
        {
            get { return isEmpty; }
            set { isEmpty = value; }
        }

        bool discardChanges = false;
        /// <summary>
        /// Gets or sets a value that indicates whether the current cell changes should be discarded.
        /// </summary>
        public bool DiscardChanges
        {
            get { return discardChanges; }
            set { discardChanges = value; }
        }

        bool isActivateTriggeredByMouseDownIntoUIElement;
        /// <summary>
        /// Specifies whether the current cell is activated by a MouseDown operation.
        /// </summary>
        public bool IsActivateTriggeredByMouseDownIntoUIElement
        {
            get { if (isEmpty) return false; else return isActivateTriggeredByMouseDownIntoUIElement; }
            set { isActivateTriggeredByMouseDownIntoUIElement = value; }
        }

        bool isActivateTriggeredByGotFocus;
        /// <summary>
        /// Specifies whether the current cell is activated by getting the focus.
        /// </summary>
        public bool IsActivateTriggeredByGotFocus
        {
            get { if (isEmpty) return false; else return isActivateTriggeredByGotFocus; }
            set { isActivateTriggeredByGotFocus = value; }
        }

        bool shouldBeginEdit;
        /// <summary>
        /// Specifies whether the current cell should be switched to editing mode when activated.
        /// </summary>
        public bool ShouldBeginEdit
        {
            get { if (isEmpty) return false; else return shouldBeginEdit; }
            set { shouldBeginEdit = value; }
        }

        UIElement element;
        /// <summary>
        /// Gets or sets the UI element for current cell.
        /// </summary>
        public UIElement Element
        {
            get { if (isEmpty) return null; else return element; }
            set { element = value; }
        }

        GridSetCurrentCellOptions setCurrentCellOptions;
        /// <summary>
        /// Defines the options for GridCurrentCell.MoveTo method.
        /// </summary>
        public GridSetCurrentCellOptions SetCurrentCellOptions
        {
            get { return setCurrentCellOptions; }
            set { setCurrentCellOptions = value; }
        }

        
    }

    /// <summary>
    /// Defines options for the <see cref="GridCurrentCell.MoveTo"/> method call.
    /// </summary>
    [Flags]
    public enum GridSetCurrentCellOptions
    {
        /// <summary>
        /// No special options.
        /// </summary>
        None = 0x00,
        /// <summary>
        /// Do not give current cell a range selection (when using Excel-like current cell).
        /// </summary>
        NoSelectRange = 0x01,
        /// <summary>
        /// Scroll new current cell into view.
        /// </summary>
        ScrollInView = 0x02,
        /// <summary>
        /// Do not set focus on text box in new current cell, ignoring <see cref="GridModelOptions.ActivateCurrentCellBehavior"/>.
        /// </summary>
        //NoSetFocus = 0x04,
        /// <summary>
        /// Try to set focus on text box in new current cell, ignoring <see cref="GridModelOptions.ActivateCurrentCellBehavior"/>.
        /// </summary>
        //SetFocus = 0x08,
        /// <summary>
        /// Do not synchronize current cell among grid views showing the same model, ignoring <see cref="GridModelOptions.ShouldSynchronizeCurrentCell"/>.
        /// </summary>
        NoSyncCurrentCell = 0x10,
        /// <summary>
        /// Force new current cell to be redrawn.
        /// </summary>
        ForceRefresh = 0x20,
        /// <summary>
        /// Sandwich current cell movement with a BeginUpdate / EndUpdate method call pair to reduce flickering.
        /// </summary>
        //BeginEndUpdate = 0x40,
        /// <summary>
        /// Do not active new current cell. Only store row and column index. 
        /// </summary>
        NoActivate = 0x80
    };


}
