#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
#if WinRT
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.Graphics.Printing;
using Windows.UI.Xaml.Media;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

#endif



namespace Syncfusion.UI.Xaml.Grid
{
    public class PrintPageControl : ContentControl, IDisposable
    {

        #region Fields

        internal double zoomHeightDelta;
        internal double zoomWidthDelta;
        private PrintManagerBase printManagerBase;
        private bool isScaleSetBeforeControlLoaded;
        #endregion

        #region Ctor

        public PrintPageControl(PrintManagerBase printManagerBase)
        {
            this.printManagerBase = printManagerBase;
            DefaultStyleKey = typeof(PrintPageControl);
            Loaded += OnLoaded;
        }

        

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the PageIndex for the Page
        /// </summary>
        public int PageIndex { get;internal set; }

        /// <summary>
        /// Gets the Total Pages for the PAge
        /// </summary>
        public int TotalPages { get; internal set; }

        #endregion

        #region UIElements

        internal Viewbox PartViewbox;
        internal Border PartScalingBorder;

        #endregion

        #region Overrides

#if WinRT
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            PartViewbox = GetTemplateChild("PART_Viewbox") as Viewbox;
            PartScalingBorder = GetTemplateChild("PartScalingBorder") as Border;
            if (DataContext is PrintManagerBase)
            {
                var dataContext = (DataContext as PrintManagerBase);
                zoomHeightDelta = dataContext.PrintPageHeight/100;
                zoomWidthDelta = dataContext.PrintPageWidth/100;
                if (isScaleSetBeforeControlLoaded)
                    dataContext.ProcessPrintPageScale(this);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (isScaleSetBeforeControlLoaded && DataContext is PrintManagerBase)
                (DataContext as PrintManagerBase).ProcessPrintPageScale(this);

        }

        #endregion

        #region Internal Methods

        internal void Zoom(double percent)
        {
            if (printManagerBase.PrintPageOrientation == PrintOrientation.Portrait)
            {
                PartViewbox.Height = percent * zoomHeightDelta;
                PartViewbox.Width = percent * zoomWidthDelta;
            }
            else
            {
                PartViewbox.Height = percent * zoomWidthDelta;
                PartViewbox.Width = percent * zoomHeightDelta;
            }
        }

        internal void Scale(double scaleX, double scaleY)
        {
            if (PartScalingBorder == null)
            {
                isScaleSetBeforeControlLoaded = true;
                return;
            }
            PartScalingBorder.RenderTransform = new ScaleTransform();
            (PartScalingBorder.RenderTransform as ScaleTransform).ScaleX = scaleX;
            (PartScalingBorder.RenderTransform as ScaleTransform).ScaleY = scaleY;
        }

        #endregion

        #region Dispose Member

        public void Dispose()
        {
            printManagerBase = null;
        }

        #endregion

    }
}
