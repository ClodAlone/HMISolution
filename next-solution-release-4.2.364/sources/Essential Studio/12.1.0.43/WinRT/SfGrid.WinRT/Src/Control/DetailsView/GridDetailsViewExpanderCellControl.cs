#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.ScrollAxis;
using System;
#if !WinRT
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

#endif

namespace Syncfusion.UI.Xaml.Grid
{
    public class GridDetailsViewExpanderCell : Control, IDisposable
    {

        internal DataColumnBase columnBase;
        private bool useTransitions = false;

        static GridDetailsViewExpanderCell()
        {
#if WPF
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridDetailsViewExpanderCell), new FrameworkPropertyMetadata(typeof(GridDetailsViewExpanderCell)));
#endif
        }

        public GridDetailsViewExpanderCell()
        {
#if !WPF
            DefaultStyleKey = typeof (GridDetailsViewExpanderCell);
#endif
        }

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(GridDetailsViewExpanderCell), new PropertyMetadata(false, OnIsExpandedPropertyChanged));

        public Visibility ExpanderIconVisibility
        {
            get { return (Visibility)GetValue(ExpanderIconVisibilityProperty); }
            set { SetValue(ExpanderIconVisibilityProperty, value); }
        }

        public static readonly DependencyProperty ExpanderIconVisibilityProperty =
            DependencyProperty.Register("ExpanderIconVisibility", typeof(Visibility), typeof(GridDetailsViewExpanderCell), new PropertyMetadata(Visibility.Visible));

        public RowColumnIndex RowColumnIndex { get; internal set; }

        internal SfDataGrid DataGrid;

        private static void OnIsExpandedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var expander = d as GridDetailsViewExpanderCell;
            if (expander != null) expander.SetExpanderState();
        }

        internal bool SuspendChangedAction;
        private void SetExpanderState()
        {
            VisualStateManager.GoToState(this, IsExpanded ? "Expanded" : "Collapsed", useTransitions);
            if (DataGrid != null && DataGrid.DetailsViewManager != null && !SuspendChangedAction)
                DataGrid.DetailsViewManager.OnDetailsViewExpanderStateChanged(RowColumnIndex, IsExpanded);
        }

#if WinRT
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            var lovalvalue = ReadLocalValue(IsExpandedProperty);
            if (lovalvalue != DependencyProperty.UnsetValue)
                SetExpanderState();
        }

#if WinRT
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            this.Focus(FocusState.Programmatic);
            if(columnBase!=null)
                this.columnBase.RaisePointerPressed(e);
            base.OnPointerPressed(e);
        }
        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            if(columnBase!=null)
                this.columnBase.RaisePointerReleased(e);
            base.OnPointerReleased(e);
        }

        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            if (e.Handled) return;
            if (ExpanderIconVisibility != Visibility.Visible) return;
            if (!this.DataGrid.Validations.CheckForValidation(true))
                return;
            useTransitions = true;
            IsExpanded = !IsExpanded;
            useTransitions = false;
            base.OnTapped(e);
        }

#elif WPF

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (columnBase != null)
                columnBase.RaisePointerPressed(e);
            base.OnPreviewMouseDown(e);
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            if (columnBase != null)
                columnBase.RaisePointerReleased(e);
            base.OnPreviewMouseUp(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (e.Handled) return;
            if (ExpanderIconVisibility != Visibility.Visible) return;
            if (columnBase != null)
                this.columnBase.OnTapped(e);
            useTransitions = true;
            IsExpanded = !IsExpanded;
            useTransitions = false;
            base.OnMouseUp(e);
        }

#else
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if(columnBase!=null)
                this.columnBase.RaisePointerPressed(e);
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if(columnBase!=null)
            {
                if (ExpanderIconVisibility != Visibility.Visible) return;
                useTransitions = true;
                IsExpanded = !IsExpanded;
                useTransitions = false;
                this.columnBase.RaisePointerReleased(e);
                this.columnBase.OnTapped(e);
            }
            base.OnMouseLeftButtonDown(e);
        }
  
#endif

        public void Dispose()
        {
            DataGrid = null;
        }
    }
}
