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

#if WinRT
using Windows.UI.Xaml;
#elif WPF
using System.Windows.Input;
#endif

namespace Syncfusion.UI.Xaml.Grid
{
    [ClassReference(IsReviewed = false)]
    public class GridRowHeaderCell : GridCell
    {

        /// <summary>
        /// Gets or sets RowErrorMessage showing in ToolTip.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public string RowErrorMessage
        {
            get { return (string)GetValue(RowErrorMessageProperty); }
            set { SetValue(RowErrorMessageProperty, value); }
        }

        public static readonly DependencyProperty RowErrorMessageProperty =
            DependencyProperty.Register("RowErrorMessage", typeof(string), typeof(GridRowHeaderCell), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets RowIndex of the cell.
        /// </summary>
        /// <value></value>
        /// <remarks>can be used to number the row like excel</remarks>
        public int RowIndex
        {
            get { return (int)GetValue(RowIndexProperty); }
            set { SetValue(RowIndexProperty, value); }
        }

        public static readonly DependencyProperty RowIndexProperty =
            DependencyProperty.Register("RowIndex", typeof(int), typeof(GridRowHeaderCell), new PropertyMetadata(0));

        public string State { get; set; }

        public GridRowHeaderCell()
        {
            this.DefaultStyleKey = typeof(GridRowHeaderCell);
            this.IsTabStop = false;
        }

        #region Overrides

#if WinRT
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            ApplyVisualState();
        }

#if WPF

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
#elif WinRT

        protected override void OnTapped(Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        protected override void OnDoubleTapped(Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            e.Handled = true;
        }
#endif

        #endregion

        public void ApplyVisualState()
        {
            switch (State)
            {
                case "Error_CurrentRow":
                    VisualStateManager.GoToState(this, "Error_CurrentRow", true);
                    break;
                case "Error":
                    VisualStateManager.GoToState(this, "Error", true);
                    break;
                case "CurrentRow":
                    VisualStateManager.GoToState(this, "CurrentRow", true);
                    break;
                case "EditingRow":
                    VisualStateManager.GoToState(this, "EditingRow", true);
                    break;
                case "Normal":
                    VisualStateManager.GoToState(this, "Normal", true);
                    break;
                case "Footer":
                    VisualStateManager.GoToState(this, "Footer", true);
                    break;
                case "AddNewRow":
                    VisualStateManager.GoToState(this, "AddNewRow", true);
                    break;
            }
        }
    }

    [ClassReference(IsReviewed = false)]
    public class GridRowHeaderIndentCell : GridCell
    {
        #region Ctor

        public GridRowHeaderIndentCell()
        {
            this.DefaultStyleKey = typeof(GridRowHeaderIndentCell);
            this.IsTabStop = false;
        }

        #endregion
    }


}
