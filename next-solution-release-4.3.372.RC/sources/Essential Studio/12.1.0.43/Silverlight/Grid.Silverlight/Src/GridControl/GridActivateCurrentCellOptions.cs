#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
using System;

#if !WinRT

namespace Syncfusion.Windows.Controls.Grid
#else
using Windows.UI.Xaml;

namespace Syncfusion.WinRT.Controls.Grid
#endif
{
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridActivateCurrentCellOptions
    {
        public static readonly GridActivateCurrentCellOptions Empty = new GridActivateCurrentCellOptions(true);

        GridActivateCurrentCellOptions(bool isEmpty)
        {
            this.isEmpty = isEmpty;
        }

        public GridActivateCurrentCellOptions()
        {
        }

        public GridActivateCurrentCellOptions(GridSetCurrentCellOptions setCurrentCellOptions)
        {
            this.setCurrentCellOptions = setCurrentCellOptions;
        }

        bool isEmpty = false;

        public bool IsEmpty
        {
            get { return isEmpty; }
            set { isEmpty = value; }
        }

        bool discardChanges = false;

        public bool DiscardChanges
        {
            get { return discardChanges; }
            set { discardChanges = value; }
        }

        bool isActivateTriggeredByMouseDownIntoUIElement;

        public bool IsActivateTriggeredByMouseDownIntoUIElement
        {
            get { if (isEmpty) return false; else return isActivateTriggeredByMouseDownIntoUIElement; }
            set { isActivateTriggeredByMouseDownIntoUIElement = value; }
        }

        bool isActivateTriggeredByGotFocus;

        public bool IsActivateTriggeredByGotFocus
        {
            get { if (isEmpty) return false; else return isActivateTriggeredByGotFocus; }
            set { isActivateTriggeredByGotFocus = value; }
        }

        bool shouldBeginEdit;

        public bool ShouldBeginEdit
        {
            get { if (isEmpty) return false; else return shouldBeginEdit; }
            set { shouldBeginEdit = value; }
        }

        UIElement element;

        public UIElement Element
        {
            get { if (isEmpty) return null; else return element; }
            set { element = value; }
        }

        GridSetCurrentCellOptions setCurrentCellOptions;

        public GridSetCurrentCellOptions SetCurrentCellOptions
        {
            get { return setCurrentCellOptions; }
            set { setCurrentCellOptions = value; }
        }


    }

    /// <summary>
    /// Defines options for the <see cref="GridCurrentCell.MoveTo"/> method call.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
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
