#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Syncfusion.Windows.Controls;
using Syncfusion.Windows.Shared;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Syncfusion.UI.Xaml.Grid
{
#if SILVERLIGHT
    using UpDown = NumericUpDown;
#endif
    public class PrintOptionsControl : Control, IDisposable
    {

        #region Fields

        const double cmConst = 37.79527559055;
        private PrintManagerBase printManager;
        private PrintPreviewAreaControl printDataContext;
        private bool isWired;

        #endregion

        #region Ctor

        public PrintOptionsControl()
        {
            DefaultStyleKey = typeof(PrintOptionsControl);
            Loaded += OnPrintOptionsControlLoaded;
#if !Silverlight4
            DataContextChanged += OnPrintOptionsControlDataContextChanged;
#endif
        }

        #endregion

        #region UIElements

        private ComboBox PapersCmbBox;
        private ComboBox MarginCmbBox;
        private ComboBox OrientationCmbBox;
        private Button PageSizeOkButton;
        private Button MarginOkButton;
        private UpDown PageWidthUpDown;
        private UpDown PageHeightUpDown;
        private UpDown LeftUpDown;
        private UpDown RightUpDown;
        private UpDown TopUpDown;
        private UpDown BottomUpDown;

        #endregion

        #region Override

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PapersCmbBox = GetTemplateChild("PART_PapersComboBox") as ComboBox;
            MarginCmbBox = GetTemplateChild("PART_MarginComboBox") as ComboBox;
            OrientationCmbBox = GetTemplateChild("PART_OrientationComboBox") as ComboBox;
            PageSizeOkButton = GetTemplateChild("PART_PageSizeOkButton") as Button;
            PageWidthUpDown = GetTemplateChild("PART_PageWidthUpDown") as UpDown;
            PageHeightUpDown = GetTemplateChild("PART_PageHeightUpDown") as UpDown;
            MarginOkButton = GetTemplateChild("PART_MarginOkButton") as Button;
            LeftUpDown = GetTemplateChild("PART_LeftUpDown") as UpDown;
            RightUpDown = GetTemplateChild("PART_RightUpDown") as UpDown;
            TopUpDown = GetTemplateChild("PART_TopUpDown") as UpDown;
            BottomUpDown = GetTemplateChild("PART_BottomUpDown") as UpDown;
            WireEvents();
        }

        #endregion

        #region Wire/UnWire Events

        private void WireEvents()
        {
            if (PapersCmbBox != null)
                PapersCmbBox.SelectionChanged += OnPartComboPapersSelectionChanged;

            if (MarginCmbBox != null)
                MarginCmbBox.SelectionChanged += OnMarginCmbBoxSelectionChanged;

            if (PageSizeOkButton != null)
                PageSizeOkButton.Click += OnPageSizeOkButtonClick;

            if (MarginOkButton != null)
                MarginOkButton.Click += OnMarginOkButtonClick;

            Unloaded += OnPrintOptionsControlUnloaded;

            isWired = true;
        }

        private void UnWireEvents()
        {

            if (PapersCmbBox != null)
                PapersCmbBox.SelectionChanged -= OnPartComboPapersSelectionChanged;

            if (MarginCmbBox != null)
                MarginCmbBox.SelectionChanged -= OnMarginCmbBoxSelectionChanged;

            if (PageSizeOkButton != null)
                PageSizeOkButton.Click -= OnPageSizeOkButtonClick;

            if (MarginOkButton != null)
                MarginOkButton.Click -= OnMarginOkButtonClick;

            isWired = false;
        }

        #endregion

        #region Events

        void OnPrintOptionsControlLoaded(object sender, RoutedEventArgs e)
        {
            if (!isWired)
                WireEvents();
        }

        void OnPrintOptionsControlUnloaded(object sender, RoutedEventArgs e)
        {
            if (isWired)
                UnWireEvents();
            Unloaded -= OnPrintOptionsControlUnloaded;
        }

#if !Silverlight4
        void OnPrintOptionsControlDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(!(e.NewValue is PrintPreviewAreaControl))
                return;
            printDataContext = e.NewValue as PrintPreviewAreaControl;
            printManager = printDataContext.PrintManagerBase;

        }
#endif


        private void OnPartComboPapersSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if(!(e.AddedItems[0] is ComboBoxItem))
                return;
#if Silverlight4
            if (!(DataContext is PrintPreviewAreaControl))
                return;
            printDataContext = DataContext as PrintPreviewAreaControl;
            printManager = printDataContext.PrintManagerBase;
#endif
            double width = 0;
            double height = 0;
            switch ((e.AddedItems[0] as ComboBoxItem).Tag.ToString())
            {
                case "Letter":
                    width = 29.59;
                    height = 27.94;
                    break;

                case "Legal":
                    width = 29.59;
                    height = 35.56;
                    break;

                case "Executive":
                    width = 18.41;
                    height = 26.67;
                    break;

                case "A4":
                    width = 21;
                    height = 29.7;
                    break;

                case "Envelope #10":
                    width = 10.48;
                    height = 24.13;
                    break;

                case "Envelope DL":
                    width = 11;
                    height = 22;
                    break;

                case "Envelope C5":
                    width = 16.2;
                    height = 22.9;
                    break;

                case "Envelope B5":
                    width = 17.6;
                    height = 25;
                    break;

                case "Envelope Monarch":
                    width = 9.84;
                    height = 19.05;
                    break;

                case "Custom Size":
                    OnPageSizeOkButtonClick(null, null);
                    if (PageHeightUpDown != null && PageHeightUpDown.Value != null) height = (double) PageHeightUpDown.Value;
                    if (PageWidthUpDown != null && PageWidthUpDown.Value != null) width = (double)PageWidthUpDown.Value;
                    break;

            }
            if (PageHeightUpDown != null && PageHeightUpDown.Value != null) PageHeightUpDown.Value = height;
            if (PageWidthUpDown != null && PageWidthUpDown.Value != null) PageWidthUpDown.Value = width;
            height *= cmConst;
            width *= cmConst;

            printManager.isSuspended = true;
            printManager.PrintPageWidth = width;
            printManager.PrintPageHeight = height;
            printManager.isSuspended = false;
            printManager.InValidate(false);
        }

        void OnPageSizeOkButtonClick(object sender, RoutedEventArgs e)
        {
#if Silverlight4
            if (!(DataContext is PrintPreviewAreaControl))
                return;
            printDataContext = DataContext as PrintPreviewAreaControl;
            printManager = printDataContext.PrintManagerBase;
#endif
            if (PageHeightUpDown.Value != null)
            {
                var height = (double)PageHeightUpDown.Value * cmConst;
                if (PageWidthUpDown.Value != null)
                {
                    var width = (double)PageWidthUpDown.Value * cmConst;
                    if (height < (printDataContext.PrintPageMargin.Top + printDataContext.PrintPageMargin.Bottom) || width < (printDataContext.PrintPageMargin.Left + printDataContext.PrintPageMargin.Right))
                        return;

                    printManager.isSuspended = true;
                    printDataContext.PrintPageHeight = height;
                    printDataContext.PrintPageWidth = width;
                    OrientationCmbBox.SelectedIndex = height > width ? 0 : 1;
                }
            }
            printManager.isSuspended = false;
            printManager.InValidate(false);
            PapersCmbBox.SelectedIndex = PapersCmbBox.Items.Count - 1;
        }

        void OnMarginCmbBoxSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!(e.AddedItems[0] is ComboBoxItem))
                return;

#if Silverlight4
            if (!(DataContext is PrintPreviewAreaControl))
                return;
            printDataContext = DataContext as PrintPreviewAreaControl;
            printManager = printDataContext.PrintManagerBase;
#endif

            double left = 0;
            double right = 0;
            double top = 0;
            double bottom = 0;
            switch ((e.AddedItems[0] as ComboBoxItem).Tag.ToString())
            {
                case "Normal":
                    left = 2.54;
                    right = 2.54;
                    top = 2.54;
                    bottom = 2.54;
                    break;

                case "Narrow":
                    left = 1.27;
                    right = 1.27;
                    top = 1.27;
                    bottom = 1.27;
                    break;

                case "Moderate":
                    left = 1.91;
                    right = 1.91;
                    top = 2.54;
                    bottom = 2.54;
                    break;

                case "Wide":
                    left = 5.08;
                    right = 5.08;
                    top = 2.54;
                    bottom = 2.54;
                    break;

                case "Custom Margin":
                    OnMarginOkButtonClick(null, null);
                    if (LeftUpDown != null && LeftUpDown.Value != null) left = (double)LeftUpDown.Value;
                    if (RightUpDown != null && RightUpDown.Value != null) right = (double)RightUpDown.Value;
                    if (TopUpDown != null && TopUpDown.Value != null) top = (double)TopUpDown.Value;
                    if (BottomUpDown != null && BottomUpDown.Value != null) bottom = (double)BottomUpDown.Value;
                    
                    break;

            }
            if (LeftUpDown != null && LeftUpDown.Value != null) LeftUpDown.Value = left;
            if (RightUpDown != null && RightUpDown.Value != null) RightUpDown.Value = right;
            if (TopUpDown != null && TopUpDown.Value != null) TopUpDown.Value = top;
            if (BottomUpDown != null && BottomUpDown.Value != null) BottomUpDown.Value = bottom; 

            var margin = new Thickness(left * cmConst, top * cmConst, right * cmConst, bottom * cmConst);
            printDataContext.PrintPageMargin = margin;
        }

        void OnMarginOkButtonClick(object sender, RoutedEventArgs e)
        {
#if Silverlight4
            if (!(DataContext is PrintPreviewAreaControl))
                return;
            printDataContext = DataContext as PrintPreviewAreaControl;
            printManager = printDataContext.PrintManagerBase;
#endif
            if (LeftUpDown.Value != null)
            {
                var left = (double)LeftUpDown.Value * cmConst;
                if (RightUpDown.Value != null)
                {
                    var right = (double)RightUpDown.Value * cmConst;
                    if (TopUpDown.Value != null)
                    {
                        var top = (double)TopUpDown.Value * cmConst;
                        if (BottomUpDown.Value != null)
                        {
                            var bottom = (double)BottomUpDown.Value * cmConst;

                            var margin = new Thickness(left, top, right, bottom);
                            printDataContext.PrintPageMargin = margin;
                        }
                    }
                }
            }
            MarginCmbBox.SelectedIndex = MarginCmbBox.Items.Count - 1;
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            printManager = null;
            printDataContext = null;
        }

        #endregion
    }

}
