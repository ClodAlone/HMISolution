#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.GridCommon;
using System;

namespace Syncfusion.Windows.Controls.Cells
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
                {
                    cv.Children.Remove(el);
                    if (el is IDisposable)
                    {
                      //  ((IDisposable)el).Dispose();   //  Currently we avoid disposing the cells while it is unloded. this should be implemented seperatly without disturbing the control disposing 
                    }
                }
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

        bool isMeasureLoadedFirstTime = true;
        bool isArrangeLoadedFirstTime = true;

        public bool IsArrangeLoadedFirstTime
        {
            get { return isArrangeLoadedFirstTime; }
            set { isArrangeLoadedFirstTime = value; }
        }

        public bool IsMeasureLoadedFirstTime
        {
            get { return isMeasureLoadedFirstTime; }
            set { isMeasureLoadedFirstTime = value; }
        }
    }
}
