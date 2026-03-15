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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
#endif

namespace Syncfusion.UI.Xaml.Grid.Cells
{
    [ClassReference(IsReviewed = false)]
    public class GridCellImageRenderer : GridVirtualizingCellRenderer<Image, Image>
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="GridCellImageRenderer"/> class.
        /// </summary>
        public GridCellImageRenderer()
        {
            IsFocusible = false;
            IsEditable = false;
            SupportsRenderOptimization = false;
        }
        #endregion

        #region Override Methods
        /// <summary>
        /// Called when [initialize edit element].
        /// </summary>
        /// <param name="rowColumnIndex">Index of the row column.</param>
        /// <param name="uiElement">The unique identifier element.</param>
        /// <param name="column">The column.</param>
        /// <param name="dataContext">The data context.</param>
        public override void OnInitializeEditElement(RowColumnIndex rowColumnIndex, Image uiElement, GridColumn column, object dataContext)
        {
            InitializeEditUIElement(uiElement, (GridImageColumn)column);
            var paddingBind = new Binding { Path = new PropertyPath("Padding"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(FrameworkElement.MarginProperty, paddingBind);
            uiElement.HorizontalAlignment = TextAlignmentToHorizontalAlignment(column.TextAlignment);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Initializes the edit unique identifier element.
        /// </summary>
        /// <param name="uiElement">The unique identifier element.</param>
        /// <param name="gridImageColumn">The grid image column.</param>
        private void InitializeEditUIElement(Image uiElement, GridImageColumn gridImageColumn)
        {
            uiElement.SetBinding(Image.SourceProperty, gridImageColumn.ValueBinding);

            var stretchBind = new Binding { Path = new PropertyPath("Stretch"), Mode = BindingMode.TwoWay, Source = gridImageColumn };
            uiElement.SetBinding(Image.StretchProperty, stretchBind);

            var imageWidthBinding = new Binding { Path = new PropertyPath("ImageWidth"), Mode = BindingMode.TwoWay, Source = gridImageColumn };
            var imageHeightBinding = new Binding { Path = new PropertyPath("ImageHeight"), Mode = BindingMode.TwoWay, Source = gridImageColumn };
            if (!double.IsInfinity(gridImageColumn.ImageHeight) && !double.IsInfinity(gridImageColumn.ImageWidth))
            {
                uiElement.SetBinding(FrameworkElement.WidthProperty, imageWidthBinding);
                uiElement.SetBinding(FrameworkElement.HeightProperty, imageHeightBinding);
            }
#if WPF
            var stretchDirectionBind = new Binding { Path = new PropertyPath("StretchDirection"), Mode = BindingMode.TwoWay, Source = gridImageColumn };
            uiElement.SetBinding(Image.StretchDirectionProperty, stretchDirectionBind);
#endif
        }
        #endregion
    }
}
