#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
using System.Windows;

#if !WinRT
using System.Windows.Media;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.GridCommon;

namespace Syncfusion.Windows.Controls.Cells
#else
using Syncfusion.WinRT.Controls.Scroll;
using Syncfusion.WinRT.GridCommon;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Syncfusion.WinRT.Controls.Cells
#endif
{
    ///// <summary>
    ///// An interface for the <see cref="GetCellUIElements"/> method. <see cref="VirtualizingCellsControl"/>
    ///// implements this method.
    ///// </summary>
    //public interface ICellUIElementsHost
    //{
    //    /// <summary>
    //    /// Gets the cell visuals for a cell.
    //    /// </summary>
    //    /// <param name="rowIndex">Index of the row.</param>
    //    /// <param name="columnIndex">Index of the column.</param>
    //    /// <returns></returns>
    //    CellUIElements GetCellUIElements(int rowIndex, int columnIndex);
    //}

    /// <summary>
    /// A strong typed dictionary that maps RowColumnIndex to CellUIElements.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class CellUIElementsDictionary : RowColumnIndexValueDictionary<CellUIElements>, IRowColumnIndexValueDictionaryCallbacks<CellUIElements>
    {
        VirtualizingCellsControl cellsControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="CellUIElementsDictionary"/> class.
        /// </summary>
        /// <param name="cellsControl">The cells control.</param>
        public CellUIElementsDictionary(VirtualizingCellsControl cellsControl)
        {
            this.cellsControl = cellsControl;
            SetCallback(this);
        }

        #region IRowColumnIndexValueDictionaryCallbacks<CellUIElements> Members

        void IRowColumnIndexValueDictionaryCallbacks<CellUIElements>.OnMovedCell(RowColumnIndex cellRowColumnIndex, CellUIElements value)
        {
            value.UpdateCellRowColumnIndex(cellRowColumnIndex);
        }

        void IRowColumnIndexValueDictionaryCallbacks<CellUIElements>.OnRemoveCell(RowColumnIndex cellRowColumnIndex, CellUIElements value)
        {
            value.Renderer.UnloadUIElements(cellsControl, cellRowColumnIndex, value);
        }

        #endregion

     }

    /// <summary>
    /// Holds the list of UIElement child elements for a cell.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class CellUIElements
    {
        List<UIElement> uiElements;
        ICellRenderer renderer;
        bool isDirty = false;
        //bool needRender;

        private bool allowUnloadVisuals = true;

        /// <summary>
        /// Gets a value indicating whether the visuals of the cell can be unloaded.
        /// </summary>
        /// <value><c>true</c> if visuals of the cell can be unloaded; otherwise, <c>false</c>.</value>
        public bool AllowUnloadVisuals
        {
            get
            {
                return this.allowUnloadVisuals;
            }
            //set
            //{
            //    this.allowUnloadVisuals = value;
            //}
        }


        internal void UpdateCellRowColumnIndex(RowColumnIndex cellRowColumnIndex)
        {
            foreach (UIElement uiElement in uiElements)
            {
                VirtualizingCellsControl.SetCellRowColumnIndex(uiElement, cellRowColumnIndex);
            }
        }

        /// <summary>
        /// Gets the renderer.
        /// </summary>
        /// <value>The renderer.</value>
        public ICellRenderer Renderer
        {
            get
            {
                return renderer;
            }
            internal set
            {
                renderer = value;
            }
        }

        /// <summary>
        /// Gets the UI elements.
        /// </summary>
        /// <value>The UI elements.</value>
        public List<UIElement> UIElements
        {
            get
            {
                if (uiElements == null)
                    uiElements = new List<UIElement>();
                return uiElements;
            }
        }

        /// <summary>
        /// Unloads the cells from the host.
        /// </summary>
        /// <param name="host">The host.</param>
        public void Unload(VirtualizingCellsControl host)
        {
            foreach (UIElement el in UIElements)
            {
                ScrollControlChildFrame cv = VisualTreeHelper.GetParent(el) as ScrollControlChildFrame;
                if (cv != null)
                    cv.Children.Remove(el);
            }
        }

        /// <summary>
        /// When set to true the UIElement will be reinitialized next time
        /// OnRender is called.
        /// </summary>
        public bool IsDirty
        {
            get { return isDirty; }
            set { isDirty = value; }
        }
    }
}
