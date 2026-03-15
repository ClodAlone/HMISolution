#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.ScrollAxis;
#if WinRT
using System;
using System.Text.RegularExpressions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#else
using System;
using System.Windows.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows.Documents;
using System.Windows.Input;
#endif
using System.Linq;

namespace Syncfusion.UI.Xaml.Grid.Cells
{
#if WinRT
    using Key = Windows.System.VirtualKey;
    using KeyEventArgs = KeyRoutedEventArgs;
#endif

    [ClassReference(IsReviewed = false)]
#if WPF
    public class GridCellHyperlinkRenderer : GridVirtualizingCellRenderer<TextBlock, ContentControl>
#else
    public class GridCellHyperlinkRenderer : GridVirtualizingCellRenderer<TextBlock, HyperlinkButton>
#endif
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="GridCellHyperlinkRenderer"/> class.
        /// </summary>
        public GridCellHyperlinkRenderer()
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
        /// <param name="uiElement">The UI element.</param>
        /// <param name="column">The column.</param>
        /// <param name="dataContext">The data context.</param>
#if WPF
        public override void OnInitializeEditElement(RowColumnIndex rowColumnIndex, ContentControl uiElement, GridColumn column, object dataContext)
#else 
        public override void OnInitializeEditElement(RowColumnIndex rowColumnIndex, HyperlinkButton uiElement, GridColumn column, object dataContext)
#endif
        {
            var Padding = new Binding { Path = new PropertyPath("Padding"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(FrameworkElement.MarginProperty, Padding);
#if WPF
            var textAlignmentBind = new Binding { Path = new PropertyPath("TextAlignment"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(TextBlock.TextAlignmentProperty, textAlignmentBind);
#endif
            var textAlignment = new Binding{Path=new PropertyPath("TextAlignment"), Mode=BindingMode.OneWay, Source=column, Converter=new TextAlignmentToHorizontalAlignmentConverter()};
            uiElement.SetBinding(Control.HorizontalContentAlignmentProperty, textAlignment);
            var horizontalAlignment = new Binding { Path = new PropertyPath("HorizontalAlignment"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(Control.HorizontalAlignmentProperty, horizontalAlignment);
            var verticalAlignment = new Binding { Path = new PropertyPath("VerticalAlignment"), Mode = BindingMode.TwoWay, Source = column };
            uiElement.SetBinding(Control.VerticalAlignmentProperty, verticalAlignment);            
#if WPF
            var content = new Hyperlink();
            uiElement.Content = content;
            var run=new Run();
            run.SetBinding(Run.TextProperty, column.DisplayBinding);
            uiElement.SetBinding(Hyperlink.NavigateUriProperty, column.ValueBinding);                        
            content.Tag = rowColumnIndex;
            content.Inlines.Clear();
            content.Inlines.Add(run);
#else
            var textBlock = new TextBlock();
            uiElement.Content = textBlock;
            textBlock.SetBinding(TextBlock.TextProperty, column.DisplayBinding);
            uiElement.SetBinding(HyperlinkButton.NavigateUriProperty, column.ValueBinding);
            uiElement.SetBinding(FrameworkElement.TagProperty, column.ValueBinding);
            textBlock.Tag = rowColumnIndex;
#if SILVERLIGHT
            var bind = new Binding { Path = new PropertyPath("TargetName"), Source = (GridHyperlinkColumn)column, Mode = BindingMode.TwoWay };
            uiElement.SetBinding(HyperlinkButton.TargetNameProperty, bind);
#endif
            var textPadding = new Binding { Path = new PropertyPath("Padding"), Mode = BindingMode.TwoWay, Source = column };
#endif
        }

        protected override void InitializeCellStyle(RowColumnIndex cellRowColumnIndex, object record, UIElement cell, GridColumn column)
        {
            base.InitializeCellStyle(cellRowColumnIndex, record, cell, column);
            var gridcell = cell as GridCell;
#if WPF
            if (gridcell == null || !(gridcell.Content is ContentControl)) return;
            var elememt = (gridcell.Content as ContentControl).Content as Hyperlink; 
            elememt.Tag = cellRowColumnIndex;
#else
            if (gridcell == null || !(gridcell.Content is HyperlinkButton)) return;
            var elememt = (gridcell.Content as HyperlinkButton).Content as TextBlock;            
#endif           
        }
        /// <summary>
        /// Called when [wire edit UI element].
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
#if WPF
        protected override void OnWireEditUIElement(ContentControl uiElement)
#else
        protected override void OnWireEditUIElement(HyperlinkButton uiElement)
#endif
        {
#if WPF
            var hyperlinkControl = uiElement.Content as Hyperlink;
#else
            var hyperlinkControl = uiElement as HyperlinkButton;
#endif
            if (hyperlinkControl != null) 
                hyperlinkControl.Click += OnHyperLinkClick;
        }

        protected override bool ShouldGridTryToHandleKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Space:
                    {
#if WPF
                        OnHyperLinkClick(((ContentControl)CurrentCellRendererElement).Content, e);
#else
                        OnHyperLinkClick(CurrentCellRendererElement, e);
#endif
                        return false;
                    }
            }
            return base.ShouldGridTryToHandleKeyDown(e);
        }
        #endregion

        #region Event Handlers
#if WinRT
        private void OnHyperLinkClick(object sender, RoutedEventArgs e)
#else
        void OnHyperLinkClick(object sender, EventArgs e)
#endif
        {
#if WPF
            var hyperlinkControl = (Hyperlink) sender;
#else
            var hyperlinkControl = (HyperlinkButton) sender;
#endif                                             
            var navigateText = string.Empty;
            var rowColumnIndex = RowColumnIndex.Empty;

            if(hyperlinkControl.NavigateUri != null)
            {
                navigateText = hyperlinkControl.NavigateUri.ToString();
            }
            else
            {
#if WPF
                 rowColumnIndex = (RowColumnIndex)hyperlinkControl.Tag;
#else
                 rowColumnIndex = (RowColumnIndex)((hyperlinkControl.Content as TextBlock).Tag);
#endif
                if (rowColumnIndex != null && !rowColumnIndex.IsEmpty)
                {
                    var column = this.DataGrid.Columns[this.DataGrid.ResolveToGridVisibleColumnIndex(rowColumnIndex.ColumnIndex)];
                    column.ColumnWrapper.DataContext = hyperlinkControl.DataContext;
                    if (column.ColumnWrapper.Value != null)
                        navigateText = column.ColumnWrapper.Value.ToString();
                }
            }
#if WPF
            if (DataGrid.CurrentCellRequestNavigateEvent(new CurrentCellRequestNavigateEventArgs { NavigateText = navigateText, RowData = hyperlinkControl.DataContext, RowColumnIndex = rowColumnIndex }))
                return;
#else
            if (DataGrid.CurrentCellRequestNavigateEvent(new CurrentCellRequestNavigateEventArgs { NavigateText = navigateText, RowData = hyperlinkControl.DataContext, RowColumnIndex = rowColumnIndex }))
                return;
#endif
            const string pattern = @"((http|https?|ftp|gopher|telnet|file|notes|ms-help):((//)|(\\\\))+[\w\d:#@%/;$()~_?\+-=\\\.&]*)";
            var NavigateUri = Regex.IsMatch(hyperlinkControl.Tag.ToString(), pattern)
                              ? new Uri(hyperlinkControl.Tag.ToString())
                              : null;
            if (NavigateUri != null)
                hyperlinkControl.NavigateUri = NavigateUri;
#if WPF
            if (hyperlinkControl.NavigateUri != null && NavigateUri!=null)
                Process.Start(new ProcessStartInfo(hyperlinkControl.NavigateUri.AbsoluteUri));
#endif
        }
        #endregion
    }
}
