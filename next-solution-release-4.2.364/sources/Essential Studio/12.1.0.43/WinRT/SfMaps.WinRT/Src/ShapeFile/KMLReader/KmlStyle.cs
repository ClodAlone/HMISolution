#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Text.RegularExpressions;
using System.Xml.Linq;
#if WINRT
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#else
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
#if WINDOWSPHONE || WINDOWSPHONE_8
using Microsoft.Phone.Controls;
#elif SILVERLIGHT
using Syncfusion.Windows.Tools.Controls;
#endif
#endif

namespace Syncfusion.UI.Xaml.Maps
{
    internal class KmlStyle
    {
        internal KmlStyle()
        {
            IconStyle = new IconStyle();
            LineStyle = new LineStyle();
            PolyStyle = new PolyStyle();
        }

        internal string Id { get; set; }
        internal IconStyle IconStyle { get; set; }
        internal LineStyle LineStyle { get; set; }
        internal PolyStyle PolyStyle { get; set; }
        internal BalloonStyle BalloonStyle { get; set; }
        internal KmlPlacemark Placemark { get; set; }

        internal ShapeFileKmlReader kmlReader;
        internal XElement styleNode;

        internal void SetAttributes()
        {
            XElement iconStyleNode = kmlReader.GetKmlChildNode(styleNode, "IconStyle");
            if (iconStyleNode != null)
            {
                XElement colorNode = kmlReader.GetKmlChildNode(iconStyleNode, "color");
                if (colorNode != null)
                {
                    IconStyle.Color = GetColor(colorNode.Value);
                }
                XElement iconNode = kmlReader.GetKmlChildNode(iconStyleNode, "Icon");
                if (iconNode != null)
                {
                    IconStyle.IconUrl = iconNode.Value;
                }
            }
            XElement lineStyleNode = kmlReader.GetKmlChildNode(styleNode, "LineStyle");
            if (lineStyleNode != null)
            {
                XElement colorNode = kmlReader.GetKmlChildNode(lineStyleNode, "color");
                if (colorNode != null)
                {
                    LineStyle.LineColor = GetColor(colorNode.Value);
                }
                XElement widthNode = kmlReader.GetKmlChildNode(lineStyleNode, "width");
                if (widthNode != null)
                {
                    LineStyle.LineThickness = Double.Parse(widthNode.Value);
                }
            }

            XElement polyStyleNode = kmlReader.GetKmlChildNode(styleNode, "PolyStyle");
            if (polyStyleNode != null)
            {
                XElement colorNode = kmlReader.GetKmlChildNode(polyStyleNode, "color");
                if (colorNode != null)
                {
                    PolyStyle.FillColor = GetColor(colorNode.Value);
                }
                XElement fillNode = kmlReader.GetKmlChildNode(polyStyleNode, "fill");
                if (fillNode != null)
                {
                    PolyStyle.IsFilled = Double.Parse(fillNode.Value).Equals(1);
                }
                XElement outlineNode = kmlReader.GetKmlChildNode(polyStyleNode, "outline");
                if (outlineNode != null)
                {
                    PolyStyle.IsFilled = Double.Parse(outlineNode.Value).Equals(1);
                }
            }
            XElement balloonStyleNode = kmlReader.GetKmlChildNode(styleNode, "BalloonStyle");
            if (balloonStyleNode != null)
            {
                BalloonStyle = new BalloonStyle();
                XElement backgroundNode = kmlReader.GetKmlChildNode(balloonStyleNode, "bgColor");
                if (backgroundNode != null)
                {
                    BalloonStyle.Background = GetColor(backgroundNode.Value);
                }
                XElement foregroundNode = kmlReader.GetKmlChildNode(balloonStyleNode, "textColor");
                if (foregroundNode != null)
                {
                    BalloonStyle.Foreground = GetColor(foregroundNode.Value);
                }
                XElement textNode = kmlReader.GetKmlChildNode(balloonStyleNode, "text");
                if (textNode != null)
                {
                    BalloonStyle.textNode = textNode;
                }
            }
        }

        internal BalloonStyle GetBalloonStyle()
        {
            if (BalloonStyle != null)
            {
                BalloonStyle.SetTextContent(kmlReader, Placemark);
            }
            return BalloonStyle;
        }

        private Color GetColor(string color)
        {
            byte a = byte.Parse(color.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(color.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(color.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            byte r = byte.Parse(color.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            return Color.FromArgb(a, r, g, b);
        }

    }

    internal struct KmlStyleMap
    {
        internal KmlStyle NormalStyle;
        internal KmlStyle HighlightStyle;
    }

    internal class IconStyle
    {
        #region Color
        private Color color = Color.FromArgb(229, 229, 229, 229);
        internal Color Color
        {
            get { return color; }
            set { color = value; }
        }
        #endregion

        internal string IconUrl { get; set; }
    }

    internal class LineStyle
    {
        #region LineColor
        private Color lineColor = Color.FromArgb(193, 193, 193, 193);
        internal Color LineColor
        {
            get { return lineColor; }
            set { lineColor = value; }
        }
        #endregion

        #region LineThickness
        private double lineThickness = 1d;
        internal double LineThickness
        {
            get { return lineThickness; }
            set { lineThickness = value; }
        }
        #endregion
    }

    internal class PolyStyle
    {
        #region FillColor
        private Color fillColor = Color.FromArgb(229, 229, 229, 229);
        internal Color FillColor
        {
            get { return fillColor; }
            set { fillColor = value; }
        }
        #endregion

        #region IsFilled
        private bool isFilled = true;
        internal bool IsFilled
        {
            get { return isFilled; }
            set { isFilled = value; }
        }
        #endregion

        #region IsOutlined
        private bool isOutlined = true;
        internal bool IsOutlined
        {
            get { return isOutlined; }
            set { isOutlined = value; }
        }
        #endregion
    }

    internal class BalloonStyle
    {
        #region Background
        internal Color background = Colors.White;
        internal Color Background
        {
            get { return background; }
            set { background = value; }
        }
        #endregion

        #region Foreground
        internal Color foreground = Colors.Black;
        internal Color Foreground
        {
            get { return foreground; }
            set { foreground = value; }
        }
        #endregion

        internal string Text { get; set; }
        internal XElement textNode;

        internal void SetTextContent(ShapeFileKmlReader kmlReader, KmlPlacemark placemark)
        {
            Text = textNode.Value;
            if (placemark != null)
            {
                var regex = new Regex("[$][^$]*[]]");
                var matches = regex.Matches(Text);
                foreach (var match in matches)
                {
                    var strToReplace = match.ToString().Replace("/displayName", "");
                    string dataName = strToReplace.Substring(2, (strToReplace.Length - 3));
                    if (placemark.ExtendedData.ContainsKey(dataName))
                    {
                        Text = Text.Replace(match.ToString(), match.ToString().Contains("displayName") ?
                                                                        placemark.ExtendedData[dataName].DisplayName :
                                                                        placemark.ExtendedData[dataName].Value.ToString());
                    }
                }
            }
            string color = "#" + Background.R.ToString("X2") + Background.G.ToString("X2") + Background.B.ToString("X2");
            XElement htmlTextNode = XElement.Parse(Text);
            if (htmlTextNode.Name.LocalName == "body" && htmlTextNode.HasAttributes && htmlTextNode.Attribute("bgcolor") != null)
            {
                htmlTextNode.Attribute("bgcolor").Value = color;
            }
            else if (htmlTextNode.HasElements && htmlTextNode.Element("body") != null)
            {
                var bodyNode = htmlTextNode.Element("body");
                if (bodyNode != null && bodyNode.HasAttributes && bodyNode.Attribute("bgcolor") != null)
                {
                    bodyNode.Attribute("bgcolor").Value = color;
                }
            }

            Text = htmlTextNode.ToString();
        }

        internal FrameworkElement GetContent(KmlPlacemark placemark)
        {
            if (!string.IsNullOrEmpty(Text))
            {
#if WINDOWSPHONE_8 || WINDOWSPHONE
                var webBrowser = new WebBrowser();
                webBrowser.Background = new SolidColorBrush(Background);
                //webBrowser.Width = 100;
                //webBrowser.Height = 100;
                webBrowser.HorizontalAlignment = HorizontalAlignment.Left;
                webBrowser.VerticalAlignment = VerticalAlignment.Top;
                webBrowser.HorizontalContentAlignment = HorizontalAlignment.Left;
                webBrowser.VerticalContentAlignment = VerticalAlignment.Top;
                webBrowser.NavigateToString(Text);
                return webBrowser;
#elif WINRT
                var webView = new WebView();
                webView.NavigateToString(Text);
                webView.UpdateLayout();
                webView.VerticalAlignment = VerticalAlignment.Top;
                webView.HorizontalAlignment = HorizontalAlignment.Left;
                webView.MinHeight = 200;
                webView.MinWidth = 150;
#if SyncfusionFramework4_5_1
                webView.NavigationCompleted += webView_NavigationCompleted;
#else
                webView.LoadCompleted += webView_LoadCompleted;
#endif
                return webView;
#elif WPF
                var richTextBox = new Windows.Tools.Controls.RichTextBoxAdv()
                {
                    Background = new SolidColorBrush(Colors.Transparent),
                    HTMLText = Text,
                    Width = 250,
                    Margin = new Thickness(5, 5, 5, 10),
                    HorizontalScrollBarVisibility = false,
                    VerticalScrollBarVisibility = false,
                    IsEnabled = false
                };
                return richTextBox;
#else
                var richTextBox = new RichTextBoxAdv
                {
                    Background = new SolidColorBrush(Colors.Transparent),
                    HTMLText = Text,
                    Width = 250,
                    Margin = new Thickness(5,5,5,10),
                    HorizontalScrollBarVisibility = false,
                    VerticalScrollBarVisibility = false,
                    IsEnabled = false
                };
                return richTextBox;
#endif
            }
#if WINRT
            return new TextBlock
            {
                Text = placemark.Name + "\n" + placemark.Description,
                Foreground = new SolidColorBrush(Foreground),
                FontFamily = new FontFamily("Courier"),
                FontSize = 14,
                Padding = new Thickness(10),
                IsHitTestVisible = false
            };
#else
            return new TextBlock
            {
                Text = placemark.Name != null ? placemark.Name.ToString() : "",
                Foreground = new SolidColorBrush(Foreground),
                FontFamily = new FontFamily("Courier"),
                FontSize = 14,
                Padding = new Thickness(10),
                IsHitTestVisible = false
            };
#endif
        }

#if WINRT
#if SyncfusionFramework4_5_1
        async void webView_NavigationCompleted(WebView webView, WebViewNavigationCompletedEventArgs args)
        {
            var strHeight = webView.InvokeScriptAsync("eval", new[] { "document.body.scrollHeight.toString()" });
            int height = int.Parse(await strHeight);
            webView.Height = height;
            var strWidth = webView.InvokeScriptAsync("eval", new[] { "document.body.scrollWidth.toString()" });
            int width = int.Parse(await strWidth);
            webView.Width = width;
        }
#else
        void webView_LoadCompleted(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            if (sender is WebView)
            {
                var webView = sender as WebView;
                var strHeight = webView.InvokeScript("eval", new[] { "document.body.scrollHeight.toString()" });
                int height = int.Parse(strHeight);
                webView.Height = height;
                var strWidth = webView.InvokeScript("eval", new[] { "document.body.scrollWidth.toString()" });
                int width = int.Parse(strWidth);
                webView.Width = width;
            }
        }
#endif
#endif

    }
}
