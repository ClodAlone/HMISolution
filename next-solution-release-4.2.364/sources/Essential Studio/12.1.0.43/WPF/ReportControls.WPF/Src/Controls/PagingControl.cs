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
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Imaging;
using Syncfusion.RDL.DOM;
using Syncfusion.RDL.Internal;
using Syncfusion.RDL.Layout;
using System.Windows.Media;
using Syncfusion.RDL.Data;
using Syncfusion.RDL.ItemModel;
using System.Windows.Input;
using Image = System.Windows.Controls.Image;
using Visibility = System.Windows.Visibility;

namespace Syncfusion.RDL.Controls
{
    internal class ReportingPageControl : ContentControl
    {
        bool isPrintMode = false;

        private DOM.Action action;

        internal TextboxModel ReportItemModel
        {
            get;
            set;
        }

        public FrameworkElement ConetentControl
        {
            get;
            set;
        }

        public ScrollViewer ScrollViewer
        {
            get;
            set;
        }

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
                    this.Margin = new Thickness(ReportItemModel.PrintPageInfo.ActualLeft, ReportItemModel.PrintPageInfo.ActualTop, 0, 0);
                }
                else
                {
                    this.ControlEvents();
                    this.Margin = new Thickness(ReportItemModel.PageInfo.ActualLeft, ReportItemModel.PageInfo.ActualTop, 0, 0);
                }
            }
        }

        internal Dictionary<int, PageInfo> PageSizes
        {
            get
            {
                if (this.IsPrintMode)
                    return this.ReportItemModel.PrintPageSizes;
                return this.ReportItemModel.PageSizes;
            }
        }

        internal int GetPage(int page)
        {
            if (this.IsPrintMode)
            {
                return this.ReportItemModel.PrintPageInfo.BelongsTo[page];
            }

            return this.ReportItemModel.PageInfo.BelongsTo[page];
        }

        internal int TablixRow
        {
            get;
            set;
        }

        internal int TablixCol
        {
            get;
            set;
        }

        internal ReportModel Model
        {
            get;
            set;
        }

        public int CurrentPage
        {
            get;
            set;
        }

        public ReportingPageControl(IReportItemModeler pageContent)
            : this(pageContent, false, false)
        {
        }

        public ReportingPageControl(IReportItemModeler pageContent, bool isToggled, bool isPrintmode)
        {
            this.ReportItemModel = pageContent as TextboxModel;
            this.ScrollViewer = new ScrollViewer();
            var richTextBox = new ReportingTextBox(pageContent);

            //Added custom properties for textbox
            if (pageContent.ReportItem.CustomProperties != null && pageContent.ReportItem.CustomProperties.Count > 0)
            {
                var curvededge = (from custompro in pageContent.ReportItem.CustomProperties where custompro.Name.ToLower() == "cornerradius" select custompro).FirstOrDefault();

                if (curvededge != null)
                {
                    System.Windows.Controls.Border border = new System.Windows.Controls.Border();
                    border.BorderBrush = (richTextBox as RichTextBox).BorderBrush;
                    border.BorderThickness = (richTextBox as RichTextBox).BorderThickness;
                    (richTextBox as RichTextBox).BorderThickness = new Thickness(0);
                    border.Margin = richTextBox.Margin;
                    richTextBox.Margin = new Thickness(0);
                    border.CornerRadius = new CornerRadius(Convert.ToDouble(curvededge.Value));
                    border.Child = richTextBox;
                    this.ConetentControl = border;
                }
                else
                {
                    this.ConetentControl = richTextBox;
                }
            }
            else
            {
                this.ConetentControl = richTextBox;
            }

            this.ConetentControl.Margin = new Thickness(0);
            //this.Name = pageContent.Name;
            this.Padding = new Thickness(0);
            this.ScrollViewer.Content = ConetentControl;
            this.ScrollViewer.Padding = new Thickness(0);
            this.ScrollViewer.Margin = new Thickness(0);

            this.ScrollViewer.BorderBrush = new SolidColorBrush(Colors.Transparent);
            this.ScrollViewer.BorderThickness = new Thickness(0.0);

            this.ScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            this.ScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            this.ScrollViewer.IsEnabled = true;
        
            this.BorderBrush = new SolidColorBrush(Colors.Transparent);
            this.BorderThickness = new Thickness(0.0);

            if ((pageContent as TextboxModel).ToggleInfos != null && (pageContent as TextboxModel).ToggleInfos.Count > 0 && !isPrintmode)
            {
                (pageContent as TextboxModel).IsFirstToggle = true;
                this.Content = this.GetContentGrid();
            }
            else if (pageContent.IsTablixChild && isToggled)
            {
                this.Model = this.ReportItemModel.Model;
                var contentGrid = this.GetContentGrid();
                contentGrid.HorizontalAlignment = HorizontalAlignment.Left;
                contentGrid.VerticalAlignment = VerticalAlignment.Top;
                this.Content = contentGrid;
                (this.ConetentControl as RichTextBox).BorderThickness = new Thickness(0);
            }
            else
            {
                this.Content = this.ScrollViewer;
            }

            if (!this.ReportItemModel.IsTablixChild)
            {
                this.Margin = new Thickness(this.ReportItemModel.PageInfo.ActualLeft, this.ReportItemModel.PageInfo.ActualTop, 0, 0);
            }
            this.Visibility = pageContent.Hidden ? Visibility.Collapsed : Visibility.Visible;
#if !SILVERLIGHT
            this.PreviewMouseWheel += ReportingPageControl_PreviewMouseWheel;
#else
            this.MouseWheel += ReportingPageControl_PreviewMouseWheel;
#endif
        }

        void ReportingPageControl_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (this.ReportItemModel != null && this.ReportItemModel.Model !=null)
            {
                this.ReportItemModel.Model.RaiseMouseScrolling(sender, e);                
            }
        }

        Grid GetContentGrid()
        {
            var rtxtbox = (this.ConetentControl as RichTextBox);
#if SILVERLIGHT

            if (rtxtbox.Blocks.Count > 0)
            {
                rtxtbox.Margin = new Thickness(15, 0, 0, 0);
            }
#else
            if (rtxtbox.Document.Blocks.Count > 0)
            {
                rtxtbox.Document.Blocks.FirstBlock.Margin = new Thickness(15, 0, 0, 0);
            }
#endif
            var imageTag = this.GetImage();
            Grid contentGrid = new Grid();
            contentGrid.Children.Add(this.ScrollViewer);
            contentGrid.Children.Add(imageTag);
            return contentGrid;
        }

        Image GetImage()
        {
            var imgtag = new Image();
#if SILVERLIGHT
            string imgPath = "/Syncfusion.ReportViewer.Silverlight;";
            imgtag.Source = (this.ReportItemModel as TextboxModel).IsToggled ? new BitmapImage(new Uri(imgPath + "component/Icons/plus.png", UriKind.Relative)) : new BitmapImage(new Uri(imgPath + "component/Icons/minus.png", UriKind.Relative));
#else
            string imgPath = "/Syncfusion.ReportControls.WPF;";
            imgtag.Source = (this.ReportItemModel as TextboxModel).IsToggled ? new BitmapImage(new Uri(imgPath + "component/Icons/plus.gif", UriKind.Relative)) : new BitmapImage(new Uri(imgPath + "component/Icons/minus.gif", UriKind.Relative));
#endif
            imgtag.Width = 16;
            imgtag.Height = 16;
            imgtag.MouseEnter += new MouseEventHandler(Textbox_MouseEnter);
            imgtag.MouseLeave += new MouseEventHandler(Textbox_MouseLeave);
            imgtag.MouseLeftButtonDown += new MouseButtonEventHandler(imgtag_MouseLeftButtonDown);
            imgtag.HorizontalAlignment = HorizontalAlignment.Left;
            imgtag.VerticalAlignment = VerticalAlignment.Center;
            imgtag.Margin = new Thickness(3, 0, 0, 0);

            return imgtag;
        }

        void imgtag_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Model.IsToggleState = true;
#if SILVERLIGHT
            string imgPath = "/Syncfusion.ReportViewer.Silverlight;";
            (sender as Image).Source = new BitmapImage(new Uri(imgPath + ((this.ReportItemModel as TextboxModel).IsToggled ? "component/Icons/minus.png" : "component/Icons/plus.png"), UriKind.Relative));
#else
            string imgPath = "/Syncfusion.ReportControls.WPF;";
            (sender as Image).Source = new BitmapImage(new Uri(imgPath + ((this.ReportItemModel as TextboxModel).IsToggled ? "component/Icons/minus.gif" : "component/Icons/plus.gif"), UriKind.Relative));
#endif
            (this.ReportItemModel as TextboxModel).IsToggled = !(this.ReportItemModel as TextboxModel).IsToggled;
            if (!this.ReportItemModel.IsTablixChild)
            {
                this.ReportItemModel.Model.RaiseToggleChanged(this.ReportItemModel, null, -1, -1);
            }
            else
            {
                this.Model.RaiseToggleChanged(this.ReportItemModel, null, TablixRow, TablixCol);
            }
        }

        internal void ControlEvents()
        {
            action = new DOM.Action();
            var actionInfo = this.ReportItemModel.TextBoxProperties.TextboxActionInfo;
            if (this.ReportItemModel.IsDrillAction)
            {
                foreach (var paraval in this.ReportItemModel.ParaExpval)
                {
                    foreach (var val in paraval.Runs)
                    {
                        if (val.ActionInfoExpVal != null)
                        {
                            actionInfo = val.ActionInfoExpVal;                            
                        }
                    }
                }                
            }
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

                    this.MouseEnter += new MouseEventHandler(Textbox_MouseEnter);
                    this.MouseLeave += new MouseEventHandler(Textbox_MouseLeave);
#if SILVERLIGHT
                    this.MouseLeftButtonDown += new MouseButtonEventHandler(Textbox_PreviewMouseLeftButtonDown);
#else
                    this.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(Textbox_PreviewMouseLeftButtonDown);
#endif
                }
            }
        }

        internal void UpdatePages()
        {
            if (this.IsPrintMode)
            {
                this.Margin = new Thickness(this.CurrentPage % this.ReportItemModel.PrintPageColumnCount == 0 ? this.ReportItemModel.PrintPageInfo.ActualLeft : 0, this.CurrentPage < this.ReportItemModel.PrintPageColumnCount ? this.ReportItemModel.PrintPageInfo.ActualTop : 0, 0, 0);
            }
            else
            {
                this.Margin = new Thickness(this.ReportItemModel.PageInfo.ActualLeft, this.CurrentPage == 0 ? this.ReportItemModel.PageInfo.ActualTop : 0, 0, 0);
            }

            this.ScrollViewer.Width = this.PageSizes[this.CurrentPage].Width;

            if (this.CurrentPage > 0)
            {
                double offset = 0.0;
                foreach (var size in this.PageSizes)
                {
                    if (size.Key < this.CurrentPage)
                        offset += size.Value.Width;
                }
                this.ScrollViewer.ScrollToHorizontalOffset(0);
                this.ScrollViewer.UpdateLayout();
                this.ScrollViewer.ScrollToHorizontalOffset(offset);
                this.ScrollViewer.UpdateLayout();
            }
            else
            {
                this.ScrollViewer.ScrollToHorizontalOffset(0);
                this.ScrollViewer.UpdateLayout();
            }

            this.ScrollViewer = null;
            this.ConetentControl = null;
            this.ReportItemModel = null;
        }

        void Textbox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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
                        this.ReportItemModel.Model.DrillThroughReport(this.ReportItemModel.Model, action.Drillthrough);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        void Textbox_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        void Textbox_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
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
    }
}
