#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Globalization;
using System.Windows;
#if WPF
using System.Windows.Controls;
#elif SILVERLIGHT
using System.Windows.Controls;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Controls;
#endif


namespace Syncfusion.UI.Xaml.Grid
{
    public class GridPrintPreviewControl : Control, IDisposable
    {

        #region Fields

        private PrintManagerBase printManager;
        private bool isWired;

        #endregion

        #region UIElements

        private PrintPreviewAreaControl PartPrintPreviewAreaControl;
        private TextBox Part_TextBox;
        private Slider PartZoomSlider;
        private Button PartZoomInButton;
        private Button PartZoomOutButton;

        #endregion

        #region Ctor

        internal GridPrintPreviewControl(SfDataGrid dataGrid, PrintManagerBase printManager)
        {
            DefaultStyleKey = typeof(GridPrintPreviewControl);
            this.printManager = printManager ?? new GridPrintManager(dataGrid);
            Loaded += OnGridPrintPreviewControlLoaded;
        }

        #endregion

        #region Internal Methods

        internal void Print()
        {
            printManager.Print();
        }

        #endregion

        #region Override

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PartPrintPreviewAreaControl = GetTemplateChild("PART_PrintPreviewAreaControl") as PrintPreviewAreaControl;
            Part_TextBox = GetTemplateChild("PART_TextBox") as TextBox;
            PartZoomSlider = GetTemplateChild("PART_ZoomSlider") as Slider;
            PartZoomInButton = GetTemplateChild("PART_MinusZoomButton") as Button;
            PartZoomOutButton = GetTemplateChild("PART_PlusZoomButton") as Button;
            WireEvents();
        }

        #endregion

        #region Wire/UnWire Events

        private void WireEvents()
        {

            if (PartPrintPreviewAreaControl != null)
                PartPrintPreviewAreaControl.PrintManagerBase = printManager;

            if(Part_TextBox != null)
                Part_TextBox.LostFocus += OnPartTextBoxLostFocus;

            if(PartZoomInButton != null)
                PartZoomInButton.Click += OnZoomInClicked;

            if (PartZoomOutButton != null)
                PartZoomOutButton.Click += OnZoomOutClicked;
            
            Unloaded += OnGridPrintPreviewControlUnloaded;

            isWired = true;
        }

        private void UnWireEvents()
        {
            if (Part_TextBox != null)
                Part_TextBox.LostFocus -= OnPartTextBoxLostFocus;

            if (PartZoomInButton != null)
                PartZoomInButton.Click -= OnZoomInClicked;

            if (PartZoomOutButton != null)
                PartZoomOutButton.Click -= OnZoomOutClicked;

            isWired = false;
        }

        #endregion

        #region Events

        void OnGridPrintPreviewControlLoaded(object sender, RoutedEventArgs e)
        {
            if(!isWired)
                WireEvents();
        }

        void OnGridPrintPreviewControlUnloaded(object sender, RoutedEventArgs e)
        {
            if (isWired)
                UnWireEvents();
            Unloaded -= OnGridPrintPreviewControlUnloaded;
        }

        private void OnPartTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            int pageIndex;
            if (int.TryParse(tb.Text, out pageIndex) && pageIndex > 0 && pageIndex <= printManager.pageCount)
                PartPrintPreviewAreaControl.PageIndex = pageIndex;
            else
                tb.ClearValue(TextBox.TextProperty);
        }

        private void OnZoomInClicked(object obj, RoutedEventArgs routedEventArgs)
        {
            if(PartZoomSlider.Value - 10 < PartZoomSlider.Minimum) return;
            PartZoomSlider.Value -= 10;
        }

        private void OnZoomOutClicked(object obj, RoutedEventArgs routedEventArgs)
        {
            if (PartZoomSlider.Value + 10 > PartZoomSlider.MaxHeight) return;
            PartZoomSlider.Value += 10;
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            printManager = null;
            PartPrintPreviewAreaControl = null;
        }

        #endregion
    }

}
