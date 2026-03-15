#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.ScrollAxis;
#if WinRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
#else
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;
#endif


namespace Syncfusion.UI.Xaml.Grid.Cells
{
    [ClassReference(IsReviewed = false)]
    public class GridCellTextBlockRenderer: GridVirtualizingCellRenderer<TextBlock, TextBlock>
    {
        public GridCellTextBlockRenderer()
        {
            SupportsRenderOptimization = false;
            IsEditable = false;
            IsFocusible = false;
        }
        /// <summary>
        /// Method which is initialize the Renderer element Bindings with corresponding column values.
        /// </summary>
        /// <param name="uiElement">Corresponding Renderer Element</param>
        /// <param name="column">Column which is providing the information ofr Binding</param>
        /// <remarks></remarks>
        public override void OnInitializeEditElement(RowColumnIndex rowColumnIndex, TextBlock uiElement, GridColumn column, object dataContext)
        {
            uiElement.SetBinding(TextBlock.TextProperty, column.ValueBinding);
            base.OnInitializeEditElement(rowColumnIndex, uiElement, column, dataContext);
            var textWrappingBinding = new Binding { Path = new PropertyPath("TextWrapping"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(TextBlock.TextWrappingProperty, textWrappingBinding);
        }
    }
}
