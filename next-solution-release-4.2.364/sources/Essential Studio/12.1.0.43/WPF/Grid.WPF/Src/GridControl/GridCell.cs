#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Syncfusion.Windows.Controls.Cells;

namespace Syncfusion.Windows.Controls.Grid
{
    /// <summary>
    /// ContentControl for DataTemplateCells
    /// </summary>
    public class GridCell : ContentControl
    {
        static GridCell()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (GridCell), new FrameworkPropertyMetadata(typeof (GridCell)));
        }

        public GridCell()
        {
            IsTabStop = false;
            KeyboardNavigation.SetIsTabStop(this, false);
        }

        protected override void OnPreviewGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            this.FocusedElement = e.OriginalSource as DependencyObject;
            base.OnPreviewGotKeyboardFocus(e);
        }

        internal DependencyObject FocusedElement { get; set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        public static readonly DependencyProperty DataSourceProperty = DependencyProperty.Register(
            "DataSource", typeof (GridDataCellBoundWrapper), typeof (GridCell),
            new PropertyMetadata(null, OnValueChanged));

        public GridDataCellBoundWrapper DataSource
        {
            get { return (GridDataCellBoundWrapper) this.GetValue(DataSourceProperty); }
            set { this.SetValue(DataSourceProperty, value); }
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null)
            {
                var gc = d as GridCell;
                if (args.NewValue is GridDataCellBoundWrapper)
                    gc.DataContext = args.NewValue;
            }
        }

        public RowColumnIndex CellRowColumnIndex { get; set; }
    }
}
