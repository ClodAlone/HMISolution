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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Syncfusion.RDL.Data;
using Syncfusion.RDL.ItemModel;
using Windows.UI;
using Syncfusion.RDL.Internal;
using Syncfusion.RDL.Layout;
using Windows.UI.Xaml.Media.Imaging;
using Border = Windows.UI.Xaml.Controls.Border;
using Image = Windows.UI.Xaml.Controls.Image;
using Paragraph = Windows.UI.Xaml.Documents.Paragraph;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace Syncfusion.RDL.Controls
{
    internal sealed class ReportingTextbox : ContentControl
    {
        RichTextBlock textBox;
        TextboxModel textBoxModel;
        LayoutReportItemModel pageInfo;
        Border border = new Border();

        private DOM.Action action;

        internal RichTextBlock InternalTextBox
        {
            get;
            set;
        }

        internal IReportItemModeler Model
        {
            get;
            set;
        }

        internal int TablixRow
        {
            get; 
            set;
        }

        internal int TablixColumn
        {
            get; 
            set;
        }

        public ReportingTextbox(IReportItemModeler model)
        {
            textBoxModel = model as TextboxModel;
            pageInfo = this.textBoxModel.PrintPageInfo;
            this.IntializeControl();
        }

        public ReportingTextbox(IReportItemModeler model, bool headerFooter)
        {
            textBoxModel = model as TextboxModel;
            pageInfo = this.textBoxModel.PageInfo;
            this.IntializeControl();
        }

        void IntializeControl()
        {
            this.textBox = new RichTextBlock();
            this.textBox.IsTextSelectionEnabled = false;
            this.InternalTextBox = textBox;
            this.Intialize();
            if ( (this.textBoxModel.ToggleInfos != null && this.textBoxModel.ToggleInfos.Count > 0) || (this.textBoxModel.ToggleGroups !=null && this.textBoxModel.ToggleGroups.Count > 0))
            {           
                this.border.Child = this.GetContentCanvas();
                if (this.textBoxModel.IsTablixChild)
                {
                    this.border.BorderThickness = new Thickness(0);
                }
            }
            else
            {
                this.border.Child = this.textBox;
            }
            if (textBoxModel.ReportItem.CustomProperties != null && textBoxModel.ReportItem.CustomProperties.Count > 0)
            {
                var curvededge = (from custompro in textBoxModel.ReportItem.CustomProperties where custompro.Name.ToLower() == "cornerradius" select custompro).FirstOrDefault();

                if (curvededge != null)
                {
                    border.CornerRadius = new CornerRadius(Convert.ToDouble(curvededge.Value));
                }
            }

            this.Content = this.border;
            this.ControlEvents(this.textBox);
        }

        void Intialize()
        {
            this.Height =this.border.Height= this.textBoxModel.Height;
            this.Width =this.border.Width= this.textBoxModel.Width;
            this.border.Background = new SolidColorBrush(Colors.Red);

            if (this.pageInfo != null)
            {
                Canvas.SetLeft(this, this.pageInfo.ActualLeft);
                Canvas.SetTop(this, this.pageInfo.ActualTop);
                this.Height = this.border.Height = this.pageInfo.ActualHeight;
                this.Width = this.border.Width = this.pageInfo.ActualWidth;
            }
            else if (this.textBoxModel.IsTablixChild)
            {
                Canvas.SetLeft(this, this.textBoxModel.Left);
                Canvas.SetTop(this, this.textBoxModel.Top);
            }

            this.textBox.Blocks.Clear();
            this.textBox.Foreground = new SolidColorBrush(Colors.Black);

            foreach (var para in textBoxModel.ParaExpval)
            {
                Paragraph paragraph = new Paragraph();
                paragraph.Inlines.Clear();

                switch (para.TextAlignment)
                {
                    case "Left":
                        paragraph.TextAlignment = TextAlignment.Left;
                        break;
                    case "Right":
                        paragraph.TextAlignment = TextAlignment.Right;
                        break;
                    case "Center":
                        paragraph.TextAlignment = TextAlignment.Center;
                        break;
                    default:
                        paragraph.TextAlignment = TextAlignment.Left;
                        break;
                }

                foreach (var run in para.Runs)
                {
                    Run txtRun = new Run();
                    txtRun.Text = run.Text;

                    if (run.Style != null)
                    {
                        txtRun.Foreground = new ReportingBrushConverter().ConvertFromInvariantString(run.Style.TextColor);
                        txtRun.FontFamily = new FontFamily(run.Style.Font.FontFamily);
                        txtRun.FontSize = run.Style.Font.FontSize;

                        if (run.Style.Font.FontStyle != DOM.FontStyle.Default)
                        {
                            txtRun.FontStyle = new ReportingFontStyleConverter().ConvertFromInvariantString(run.Style.Font.FontStyle.ToString());
                        }
                        if (run.Style.Font.FontWeight != DOM.FontWeight.Default)
                        {
                            txtRun.FontWeight = new ReportingFontWeightConverter().ConvertFromInvariantString(run.Style.Font.FontWeight.ToString());
                        }
                    }

                    paragraph.Inlines.Add(txtRun);
                }

                this.textBox.Blocks.Add(paragraph);
            }

            foreach (var para in this.textBoxModel.ParaExpval)
            {
                Paragraph uiPara = new Paragraph();
                uiPara.Inlines.Clear();
                
                foreach (var run in para.Runs)
                {
                    Run uiRun = new Run();
                    uiRun.Text = run.Text;
                    uiRun.Foreground = new SolidColorBrush(Colors.Black);
                    uiPara.Inlines.Add(uiRun);
                }
            }

            this.border.Background = new ReportingBrushConverter().ConvertFromInvariantString(this.textBoxModel.TextBoxProperties.BackGroundColor);

            if (this.textBoxModel.TextBoxProperties.Border != null)
            {
                if (this.textBoxModel.TextBoxProperties.Border.Default != null)
                {
                    this.border.BorderBrush = new ReportingBrushConverter().ConvertFromInvariantString(this.textBoxModel.TextBoxProperties.Border.Default.BorderBrush);
                    if (this.textBoxModel.TextBoxProperties.Border.Default.BorderStyle != DOM.BorderStyles.None
                         && this.textBoxModel.TextBoxProperties.Border.Default.BorderStyle != DOM.BorderStyles.Default)
                    {
                        this.border.BorderThickness = new Thickness(this.textBoxModel.TextBoxProperties.Border.Default.Thickness);
                    }
                }
            }
           
            if (this.textBoxModel.TextBoxProperties.Padding != null)
            {
                this.InternalTextBox.Padding = new Thickness(this.textBoxModel.TextBoxProperties.Padding.Left, this.textBoxModel.TextBoxProperties.Padding.Top, this.textBoxModel.TextBoxProperties.Padding.Right, this.textBoxModel.TextBoxProperties.Padding.Bottom);
            }

      //      this.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center;
            //this.VerticalAlignment = this.textBoxModel.TextBoxProperties
        }

        private void ControlEvents(RichTextBlock block)
        {
            action = new DOM.Action();
            var actionInfo = this.textBoxModel.TextBoxProperties.TextboxActionInfo;
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
                    block.PointerEntered += block_PointerEntered;
                    block.PointerExited += block_PointerExited;
                    block.PointerPressed += block_PointerPressed;
                }
            }
        }

        void block_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(action.Hyperlink))
            {
                Windows.System.Launcher.LaunchUriAsync(new Uri(action.Hyperlink));
            }
            else if (!string.IsNullOrEmpty(action.Drillthrough.ReportName))
            {
                Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
                this.textBoxModel.Model.DrillThroughReport(this.textBoxModel.Model, action.Drillthrough);
            }
        }

        void block_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
        }

        void block_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Hand, 1);
        }

        DOM.Parameters GetParameters(List<TextboxParameterExpVal> action)
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

        Canvas GetContentCanvas()
        {
            if (this.textBox.Blocks.Count > 0)
            {
                this.textBox.Blocks.First().Margin = new Thickness(20, 0, 0, 0);
            }
            var buttonTag = this.GetButtonImg();
            Canvas contentCanvas = new Canvas();
            contentCanvas.Children.Add(this.textBox);
            contentCanvas.Children.Add(buttonTag);
            return contentCanvas;
        }

        Button GetButtonImg()
        {
            Button toggleButton = new Button();
            toggleButton.Style = this.GetButtonStyle();
            toggleButton.Width = 20;
            toggleButton.Height = 20;
            toggleButton.HorizontalAlignment = HorizontalAlignment.Left;
            toggleButton.VerticalAlignment = VerticalAlignment.Center;
            toggleButton.Margin = new Thickness(2, 0, 0, 0);
            toggleButton.PointerPressed += toggleButton_Click;
            toggleButton.PointerEntered += block_PointerEntered;
            toggleButton.PointerExited += block_PointerExited;
            toggleButton.Click+=toggleButton_Click;
            return toggleButton;
        }

        void toggleButton_Click(object sender, RoutedEventArgs e)
        {
            this.Model.Model.IsToggleState = true;
            (sender as Button).Style = this.GetButtonStyle();
            this.textBoxModel.IsToggled = !this.textBoxModel.IsToggled;           
            this.textBoxModel.Model.RaiseToggleChanged(this.textBoxModel, null , TablixRow , TablixColumn);
        }

        Windows.UI.Xaml.Style GetButtonStyle()
        {
            var res = new ResourceDictionary()
            {
                Source = new Uri("ms-appx:///Syncfusion.SfReportViewer.WinRT/Themes/Generic.xaml", UriKind.Absolute)
            };
            return this.textBoxModel.IsToggled ? res["Plus"] as Windows.UI.Xaml.Style : res["Minus"] as Windows.UI.Xaml.Style;
        }
    }
}
