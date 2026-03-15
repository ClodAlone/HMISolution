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
using Syncfusion.RDL.DOM;
using Syncfusion.RDL.Data;
using Syncfusion.RDL.ItemModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Syncfusion.RDL.Internal;
using System.IO;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using Image = Windows.UI.Xaml.Controls.Image;
using Visibility = Windows.UI.Xaml.Visibility;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace Syncfusion.RDL.Controls
{
    internal sealed class ReportingImage : ContentControl
    {
        Image imageControl = null;

        private DOM.Action action; 

        internal IReportItemModeler Model
        {
            get;
            set;
        }

        internal ImageModel ImageModel
        {
            get;
            set;
        }

        ImagePropertiesExpVal ImageProperties
        {
            get;
            set;
        }

        public object ImageData
        {
            get;
            set;
        }

        public ReportingImage(IReportItemModeler model)
        {
            this.Model = model;
            this.imageControl = new Image();
            this.ImageModel = this.Model as ImageModel;
            this.ImageProperties = this.ImageModel.ImageProperties;
            this.IntializeImage();
            this.Content = this.imageControl;
            this.ImageEvents(this.imageControl);
        }

        private void IntializeImage()
        {
            BitmapImage img = new BitmapImage();
            this.Height = this.ImageModel.Height;
            this.Width = this.ImageModel.Width;

            if (this.ImageModel.PageInfo != null)
            {
                Canvas.SetLeft(this, this.ImageModel.PageInfo.ActualLeft);
                Canvas.SetTop(this, this.ImageModel.PageInfo.ActualTop);
            }
            else if (this.ImageModel.IsTablixChild && this.ImageModel.IsTablixInnerChild)
            {
                Canvas.SetLeft(this, this.ImageModel.Left);
                Canvas.SetTop(this, this.ImageModel.Top);
            }

            this.imageControl.Width = this.Width;
            this.imageControl.Height = this.Height;
            this.imageControl.Stretch = this.ImageModel.ImageSize;
         
            object value = this.ImageModel.ImageData;
            ToolTipService.SetToolTip(this, this.ImageProperties.ToolTip);

            this.Visibility = this.ImageProperties.Hidden ? Visibility.Collapsed : Visibility.Visible;

            this.imageControl.HorizontalAlignment = HorizontalAlignment.Left;
            this.imageControl.VerticalAlignment = VerticalAlignment.Top;

            this.imageControl.Source = this.GetImage(this.ImageModel.ImageData);

            if (this.ImageModel.ImageProperties.Border.Default != null)
            {
                double left;
                double right;
                double top;
                double bottom;
                left = right = top = bottom = this.ImageModel.ImageProperties.Border.Default.Thickness;

                if (this.ImageProperties.Border.LeftBorder != null)
                {
                    left = this.ImageProperties.Border.LeftBorder.Thickness;
                }
                if (this.ImageProperties.Border.RightBorder != null)
                {
                    right = this.ImageProperties.Border.RightBorder.Thickness;
                }
                if (this.ImageProperties.Border.TopBorder != null)
                {
                    top = this.ImageProperties.Border.TopBorder.Thickness;
                }
                if (this.ImageProperties.Border.BottomBorder != null)
                {
                    bottom = this.ImageProperties.Border.BottomBorder.Thickness;
                }

                this.BorderThickness = new Thickness(left, top, right, bottom);
            }

            if (this.ImageProperties.Border.Default != null)
            {
                this.BorderBrush = new ReportingBrushConverter().ConvertFromInvariantString(this.ImageProperties.Border.Default.BorderBrush);
            }
            this.Padding = new Thickness(this.ImageProperties.Padding.Left, this.ImageProperties.Padding.Top, this.ImageProperties.Padding.Right, this.ImageProperties.Padding.Bottom);
        }

        private void ImageEvents(Image image)
        {
            action = new DOM.Action();
            var actionInfo = this.ImageModel.ImageProperties.ImageActionInfo;
            if (actionInfo != null)
            {
                action.Hyperlink = actionInfo.Hyperlink;
                action.Drillthrough = new Drillthrough()
                {
                    Parameters = this.GetParameters(actionInfo.Parameters),
                    ReportName = actionInfo.ReportName
                };

                if (!string.IsNullOrEmpty(action.Hyperlink) || !string.IsNullOrEmpty(action.Drillthrough.ReportName))
                {
                    image.PointerEntered += image_PointerEntered;
                    image.PointerExited += image_PointerExited;
                    image.PointerPressed += image_PointerPressed;
                }
            }
        }

        void image_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(action.Hyperlink))
            {
                Windows.System.Launcher.LaunchUriAsync(new Uri(action.Hyperlink));
            }
            else if (!string.IsNullOrEmpty(action.Drillthrough.ReportName))
            {
                Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
                this.ImageModel.Model.DrillThroughReport(this.ImageModel.Model, action.Drillthrough);
            }
        }

        void image_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
        }

        void image_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Hand, 1);
        }

        DOM.Parameters GetParameters(List<ImageParameterExpVal> action)
        {
            if (action != null)
            {
                DOM.Parameters parameters = new Parameters();
                foreach (var para in action)
                {
                    DOM.Parameter parameter = new Parameter();
                    parameter.Name = para.Name;
                    parameter.Omit = para.Omit;
                    parameter.Value = para.Value;
                    parameters.Add(parameter);
                }
                return parameters;
            }
            return null;
        }

        BitmapImage GetImage(object value)
        {
            BitmapImage img = new BitmapImage();
            byte[] imageBytes = (byte[])value;
            try
            {
                using (var randomAccessStream = new InMemoryRandomAccessStream())
                {
                    var writeStream = randomAccessStream.AsStreamForWrite();
                    writeStream.WriteAsync(imageBytes, 0, imageBytes.Length);
                    writeStream.FlushAsync();
                    randomAccessStream.Seek(0L);
                    img.SetSource(randomAccessStream);
                }
            }
            catch
            {
                return null;
            }
            return img;
        }
    }
}
