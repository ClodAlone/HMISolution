#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Syncfusion.Windows.Controls.Scroll;
using System.Diagnostics;

namespace Syncfusion.Windows
{
    public class DrawingVisual : ContentPresenter, IDisposable
    {
        private Image _imageControl;
        private DrawingContext _drawingcontext;
        private Size lastArrangeSize;

        public DrawingVisual()
        {
            this.InitializeDrawingVisual();
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            this.InitializeDrawingVisual();
            if (this._imageControl.RenderSize == finalSize && finalSize.Width > 0 && finalSize.Height > 0)
            {
                return finalSize;
            }

            this._imageControl.Arrange(new Rect(new Point(0, 0), finalSize));
            return base.ArrangeOverride(finalSize);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            this.InitializeDrawingVisual();
            bool invalidateMeasure = false;
            if (invalidateMeasure)
            {
                this._imageControl.InvalidateMeasure();
            }


            this._imageControl.Measure(availableSize);
            return base.MeasureOverride(availableSize);
        }

        private void InitializeDrawingVisual()
        {
            if (this._imageControl == null || this.Content == null)
            {
                this._imageControl = new Image();
                this.Content = this._imageControl;
            }
        }

        public DrawingContext RenderOpen()
        {

            if (this._drawingcontext != null && this._drawingcontext._imageControl == null)
            {
                if (this._imageControl.RenderSize.Width == 0 || this._imageControl.RenderSize.Height == 0)
                {
                    this._imageControl.Width = this.ActualWidth;
                    this._imageControl.Height = this.ActualHeight;
                }

                this._drawingcontext.PushAssociatedBitmap(this._imageControl);
            }

            if (this._drawingcontext == null)
            {
                this.InitializeDrawingVisual();
                this._drawingcontext = new DrawingContext(this._imageControl, this.RenderSize);
            }

            return this._drawingcontext;
        }

        public void Dispose()
        {
            if (this._drawingcontext != null)
                this._drawingcontext.Dispose();
            this._drawingcontext = null;
            this._imageControl = null;
            //GC.Collect();
        }
    }
}
