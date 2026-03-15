#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.ScrollAxis;
using System;
#if WinRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
#else
using System.Windows;
using System.Windows.Data;
#endif

namespace Syncfusion.UI.Xaml.Grid
{
    [ClassReference(IsReviewed = false)]
    public class DataColumn : DataColumnBase 
    {
        #region override methods

        /// <summary>
        /// Prepares the Column element based on the Visible column 
        /// </summary>
        /// <remarks></remarks>
        protected override FrameworkElement OnInitializeColumnElement(object dataContext, bool isInEdit)
        {
            UIElement element = null;
            if (IsIndentColumn)
            {
                element = new GridIndentCell();
            }
            else
            {
                this.IsEditing = isInEdit;
                element = this.Renderer.PrepareUIElements(new RowColumnIndex(this.RowIndex, this.ColumnIndex),this.ColumnElement,this.GridColumn, dataContext,this.IsEditing );
            }
            this.SetBindings(element);
            return (FrameworkElement)element;
        }

        /// <summary>
        /// When we scroll the Grid vertically row's will be recycled. 
        /// While recycling we need to update the style info of all the cell's in old row.
        /// This property change call back will update the style info of all the cell element when the row index changed.
        /// </summary>
        /// <remarks></remarks>
        public override void UpdateCellStyle()
        {
            if (this.ColumnElement != null && this.Renderer!=null)
                this.Renderer.UpdateCellStyle(new RowColumnIndex(this.RowIndex, this.ColumnIndex), this.ColumnElement, this.GridColumn);
        }

        /// <summary>
        /// Method which is update the binding and style information of the 
        /// cell when we recycle the cell for scrolling.
        /// </summary>
        /// <remarks></remarks>
        public override void UpdateBinding(object dataContext, bool updateCellStyle=true)
        {
            if (this.Renderer == null)
                return;
            var indexes = new RowColumnIndex(this.RowIndex, this.ColumnIndex);
            if(updateCellStyle)
                this.Renderer.UpdateCellStyle(indexes, this.ColumnElement, this.GridColumn);
            this.Renderer.UpdateBindingInfo(indexes, this.ColumnElement, this.GridColumn, dataContext, this.IsEditing);
        }

        #endregion
    }
}
