#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.Grid.Utility;
using Syncfusion.UI.Xaml.ScrollAxis;
#if WinRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Controls;
#else
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if !Silverlight4
using System.Threading.Tasks;
#endif
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
#endif

namespace Syncfusion.UI.Xaml.Grid.Cells
{
    #region DataContextHelper Class
    /// <summary>
    /// DataContext Helper Class for GridCellDataTemplateRenderer.
    /// </summary>
    public class DataContextHelper : DependencyObject
    {
        #region Properties

        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(DataContextHelper), new PropertyMetadata(null));


        public object Record
        {
            get { return (object)GetValue(RecordProperty); }
            set { SetValue(RecordProperty, value); }
        }

        public static readonly DependencyProperty RecordProperty =
            DependencyProperty.Register("Record", typeof(object), typeof(DataContextHelper), new PropertyMetadata(null));
        #endregion

        #region Private Methods
        public void SetValueBinding(Binding binding, object source)
        {
            BindingOperations.SetBinding(this, ValueProperty, binding.CreateBinding(source));
        }
        #endregion
    }
    #endregion

    #region GridCellDataTemplateRenderer
    public class GridCellDataTemplateRenderer : GridCellTemplateRenderer
    {
        #region Display/Edit Binding Overrides
        /// <summary>
        /// Called when [initialize display element].
        /// </summary>
        /// <param name="rowColumnIndex">Index of the row column.</param>
        /// <param name="uiElement">The unique identifier element.</param>
        /// <param name="column">The column.</param>
        /// <param name="dataContext">The data context.</param>
        public override void OnInitializeDisplayElement(RowColumnIndex rowColumnIndex, ContentControl uiElement, GridColumn column, object dataContext)
        {
            base.OnInitializeDisplayElement(rowColumnIndex, uiElement, column, dataContext);
            uiElement.ClearValue(ContentControl.ContentProperty);
            var dataContextHelper = new DataContextHelper { Record = dataContext };
            dataContextHelper.SetValueBinding(column.DisplayBinding, dataContext);
            uiElement.Content = dataContextHelper;
        }

        /// <summary>
        /// Called when [initialize edit element].
        /// </summary>
        /// <param name="rowColumnIndex">Index of the row column.</param>
        /// <param name="uiElement">The unique identifier element.</param>
        /// <param name="column">The column.</param>
        /// <param name="dataContext">The data context.</param>
        public override void OnInitializeEditElement(RowColumnIndex rowColumnIndex, ContentControl uiElement, GridColumn column, object dataContext)
        {
            base.OnInitializeEditElement(rowColumnIndex, uiElement, column, dataContext);
            uiElement.ClearValue(ContentControl.ContentProperty);
            var dataContextHelper = new DataContextHelper { Record = dataContext };
            dataContextHelper.SetValueBinding(column.ValueBinding, dataContext);
            uiElement.Content = dataContextHelper;
        }

        /// <summary>
        /// Initializes the cell style.
        /// </summary>
        /// <param name="cellRowColumnIndex">Index of the cell row column.</param>
        /// <param name="record">The record.</param>
        /// <param name="cell">The cell.</param>
        /// <param name="column">The column.</param>
        protected override void InitializeCellStyle(RowColumnIndex cellRowColumnIndex, object record, UIElement cell, GridColumn column)
        {
            base.InitializeCellStyle(cellRowColumnIndex, record, cell, column);
            var uiElement = (cell as GridCell).Content as ContentControl;
            var gridColumn = column as GridTemplateColumn;
            if (uiElement != null && gridColumn != null)
            {
                uiElement.ClearValue(ContentControl.ContentProperty);
                var dataContextHelper = new DataContextHelper { Record = record };
                dataContextHelper.SetValueBinding(column.ValueBinding, record);
                uiElement.Content = dataContextHelper;
            }
        }
        #endregion
    }
    #endregion
}
