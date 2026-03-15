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
using System.ComponentModel;
using System.IO;
using Syncfusion.RDL.DOM;
using Syncfusion.RDL.Data;
using Syncfusion.RDL.Internal;
using Syncfusion.RDL.ItemModel;
using Image=System.Windows.Controls.Image;
using Visibility = System.Windows.Visibility;

namespace Syncfusion.RDL.Controls
{

    internal partial class ReportingImage : UserControl
    {
        bool isPrintMode = false;

        private DOM.Action action; 

        internal bool IsPrintMode
        {
            get
            {
                return isPrintMode;
            }
            set
            {
                isPrintMode = value;
                if (isPrintMode)
                {
                    this.Margin = new Thickness(ImageModel.PrintPageInfo.ActualLeft, ImageModel.PrintPageInfo.ActualTop, 0, 0);
                }
                else
                {
                    this.Margin = new Thickness(ImageModel.PageInfo.ActualLeft, ImageModel.PageInfo.ActualTop, 0, 0);
                    this.ImageEvents(this.Content as Image);
                }
            }
        }

        internal ImageModel ImageModel
        {
            get;
            set;
        }

        public object ImageData
        {
            get;
            set;
        }

        private Base64ImageConverter base64ImageConverter = new Base64ImageConverter();

        public ReportingImage(IReportItemModeler pageContent)
        {
            this.ImageModel = pageContent as ImageModel;
            object value = (pageContent as ImageModel).ImageData;

            Image image = new Image();
            BitmapImage bitmapImage = new BitmapImage();
            image.Height = this.ImageModel.Height;
            image.Width = this.ImageModel.Width;
            image.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            image.VerticalAlignment = System.Windows.VerticalAlignment.Top;
#if SILVERLIGHT
            if (value is string)
            {
                bitmapImage = (BitmapImage)base64ImageConverter.ConvertToImage(value.ToString());
                image.Source = bitmapImage;
            }
            else
            {
                image.Source = new Base64ImageConverter().CovertByteToImage(value);
            }
            this.Content = image;
#else
            if (this.ImageModel.ImageFormat == ImageFormats.Emf)
            {
                MemoryStream stream = new MemoryStream();
                if (value is string)
                {
                    byte[] data = Convert.FromBase64String(value.ToString());
                    stream = new MemoryStream(data);
                }
                else
                {
                    stream = new MemoryStream(value as byte[]);
                }
                System.Windows.Forms.Integration.WindowsFormsHost Host = new System.Windows.Forms.Integration.WindowsFormsHost();
                System.Windows.Forms.PictureBox pictureBox = new System.Windows.Forms.PictureBox();

                pictureBox.Height = (int)image.Height;
                pictureBox.Width = (int)image.Width;
                pictureBox.Image = new System.Drawing.Imaging.Metafile(stream);
                switch (this.ImageModel.ImageSize)
                {
                    case System.Windows.Media.Stretch.None:
                        pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
                        break;
                    case System.Windows.Media.Stretch.Fill:
                        pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
                        break;
                    case System.Windows.Media.Stretch.Uniform:
                        pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
                        if (pageContent.Height < pageContent.Width)
                        {
                            Host.Width = new DOM.Size(pageContent.Height + "pt").PixelValue;
                            Host.HorizontalAlignment = HorizontalAlignment.Left;
                        }
                        else
                        {
                            Host.Height = pageContent.Width - 50;
                            Host.VerticalAlignment = VerticalAlignment.Top;
                        }
                        break;
                    default:
                        pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
                        break;
                }

                Host.Child = pictureBox;
                this.Content = Host;
            }
            else
            {
                if (this.ImageModel.ImageSize != Stretch.None)
                {
                    image.Stretch = this.ImageModel.ImageSize;
                }

                if (value is string)
                {
                    bitmapImage = (BitmapImage)base64ImageConverter.ConvertToImage(value.ToString());
                    image.Source = bitmapImage;
                }
                else
                {
                    image.DataContext = this;
                    this.ImageData = value;
                    Binding bin = new Binding();
                    bin.Source = this;
                    bin.Path = new PropertyPath("ImageData");
                    image.SetBinding(Image.SourceProperty, bin);
                }
                this.Content = image;
            }
#endif

            this.Height = this.ImageModel.Height;
            this.Width = this.ImageModel.Width;

#if !SILVERLIGHT
            this.ToolTip = this.ImageModel.ImageProperties.ToolTip;
#else
            ToolTipService.SetToolTip(this, this.ImageModel.ImageProperties.ToolTip);
#endif

            if (this.ImageModel.ImageProperties.Border.Default != null)
            {
                double left;
                double right;
                double top;
                double bottom;
                left = right = top = bottom= this.ImageModel.ImageProperties.Border.Default.Thickness;

                if (this.ImageModel.ImageProperties.Border.LeftBorder != null)
                {
                    left = this.ImageModel.ImageProperties.Border.LeftBorder.Thickness;
                }
                if (this.ImageModel.ImageProperties.Border.RightBorder != null)
                {
                    right = this.ImageModel.ImageProperties.Border.RightBorder.Thickness;
                }
                if (this.ImageModel.ImageProperties.Border.TopBorder != null)
                {
                    top = this.ImageModel.ImageProperties.Border.TopBorder.Thickness;
                }
                if (this.ImageModel.ImageProperties.Border.BottomBorder != null)
                {
                    bottom = this.ImageModel.ImageProperties.Border.BottomBorder.Thickness;
                }

                this.BorderThickness = new Thickness(left, top, right, bottom);
                this.BorderBrush = new ReportingBrushConverter().ConvertFromInvariantString(this.ImageModel.ImageProperties.Border.Default.BorderBrush);
                if (this.ImageModel.ImageProperties.Border.Default.BorderStyle == DOM.BorderStyles.None || this.ImageModel.ImageProperties.Border.Default.BorderStyle == DOM.BorderStyles.Default)
                {
                    this.BorderThickness = new Thickness(0);
                }
            }

            this.Padding = new Thickness(this.ImageModel.ImageProperties.Padding.Left, this.ImageModel.ImageProperties.Padding.Top, this.ImageModel.ImageProperties.Padding.Right, this.ImageModel.ImageProperties.Padding.Bottom);

            if (pageContent.PageInfo != null)
            {
                this.Margin = new System.Windows.Thickness(pageContent.PageInfo.ActualLeft, pageContent.PageInfo.ActualTop, 0, 0);
            }
            else if (pageContent.IsTablixChild && pageContent.IsTablixInnerChild)
            {
                this.Margin = new System.Windows.Thickness(pageContent.Left, pageContent.Top, 0, 0);
            }
            this.Visibility = pageContent.Hidden ? Visibility.Collapsed : Visibility.Visible;
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
                    image.MouseEnter += new MouseEventHandler(Image_MouseEnter);
                    image.MouseLeave += new MouseEventHandler(Image_MouseLeave);
#if SILVERLIGHT
                    image.MouseLeftButtonDown += new MouseButtonEventHandler(Image_PreviewMouseLeftButtonDown);
#else
                    image.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(Image_PreviewMouseLeftButtonDown);
#endif
                }
            }
        }

        void Image_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (this.Cursor == Cursors.Hand)
                {
                    if (!string.IsNullOrEmpty(action.Hyperlink))
                    {
#if !SILVERLIGHT
                        System.Diagnostics.Process.Start(action.Hyperlink);
#else
                            System.Windows.Browser.HtmlPage.Window.Navigate(new Uri(action.Hyperlink));
#endif
                    }
                    else if (!string.IsNullOrEmpty(action.Drillthrough.ReportName))
                    {
                        this.ImageModel.Model.DrillThroughReport(this.ImageModel.Model, action.Drillthrough);
                    }
                }
            }
            catch { }
        }

        void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
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
    }
}
