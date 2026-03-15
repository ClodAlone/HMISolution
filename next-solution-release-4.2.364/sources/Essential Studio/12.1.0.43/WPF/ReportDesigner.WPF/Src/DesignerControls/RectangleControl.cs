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
using System.Windows.Media;
using System.Windows.Data;
using Syncfusion.Windows.Reports.Designer.Dialogs;
using System.Windows;
using Syncfusion.Windows.Reports.Designer.Editors;
using System.ComponentModel;
using Syncfusion.RDL.Internal;
using Syncfusion.Windows.ReportDesigner.Resources;
using System.Globalization;
namespace Syncfusion.Windows.Reports.Designer.Controls
{

    internal class DesignerDashStyleBorder : Border
    {
        public static readonly DependencyProperty DashStyleProperty = DependencyProperty.Register("DashStyle", typeof(RDL.DOM.BorderStyles), typeof(DesignerDashStyleBorder), new PropertyMetadata(RDL.DOM.BorderStyles.None, DashStylePropertyChanged));

        static void DashStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            (d as Border).InvalidateVisual();
        }

        public RDL.DOM.BorderStyles DashStyle
        {
            get
            {
                return (RDL.DOM.BorderStyles)this.GetValue(DashStyleProperty);
            }
            set
            {
                if (this.DashStyle != value)
                {
                    this.SetValue(DashStyleProperty, value);
                }
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            DashStyle style = DashStyles.Dot;
            Thickness borThick = new Thickness(1);
            Brush borderBrush = Brushes.Black;

            if (this.DashStyle != RDL.DOM.BorderStyles.None && this.DashStyle != RDL.DOM.BorderStyles.Default)
            {
                switch (this.DashStyle)
                {
                    case RDL.DOM.BorderStyles.DashDot:
                        style = DashStyles.DashDot;
                        break;
                    case RDL.DOM.BorderStyles.DashDotDot:
                        style = DashStyles.DashDotDot;
                        break;
                    case RDL.DOM.BorderStyles.Dashed:
                        style = DashStyles.Dash;
                        break;
                    case RDL.DOM.BorderStyles.Dotted:
                        style = DashStyles.Dot;
                        break;
                    default:
                        style = DashStyles.Solid;
                        break;
                }
                    
                borThick = this.BorderThickness;
                if (this.BorderBrush != null)
                {
                    borderBrush = this.BorderBrush;
                }
            }

            Thickness borderThickness = borThick;
            Pen pen = new Pen(borderBrush, borderThickness.Top);
            pen.DashStyle = style;
            double x = pen.Thickness * .5;
            dc.DrawRectangle(this.Background, pen, new Rect(new Point(x, x), new Point(RenderSize.Width - x, RenderSize.Height - x)));
        }
    }

    internal class RectangleControl : Canvas, IReportItemControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        DesignerDashStyleBorder border = new DesignerDashStyleBorder();
        System.Windows.Shapes.Rectangle selectionbox = new System.Windows.Shapes.Rectangle(); 
        internal RectangleProperties Properties = new RectangleProperties();
        internal ReportingConvertorUtil propertyValueConvertor = new ReportingConvertorUtil(); 
        private bool isFocusedItem;
        private bool isItemSelected;
        private object propertyOldValue = null;

        #region ReportItemControl Interface

        public Syncfusion.RDL.DOM.ReportItem ReportItem
        {
            get;
            set;
        }

        public DesignPanel Panel
        {
            get;
            set;
        }

        public string ItemName
        {
            get
            {
                return this.Name;
            }
            set
            {
                this.Name = value;
                this.Properties.Name = value;
            }
        }

        public double ItemHeight
        {
            get
            {
                return this.ActualHeight;
            }
            set
            {
                if (this.ItemHeight != value)
                {
                    this.Properties.IsInternalPropertyChange = true;
                    this.Properties.Height = propertyValueConvertor.GetSizeValue(value, this.Properties.Height);
                    this.Properties.IsInternalPropertyChange = false;
                }
                this.Height = value;
            }
        }

        public double ItemWidth
        {
            get
            {
                return this.ActualWidth;
            }
            set
            {
                if (this.ItemWidth != value)
                {
                    this.Properties.IsInternalPropertyChange = true;
                    this.Properties.Width = propertyValueConvertor.GetSizeValue(value, this.Properties.Width);
                    this.Properties.IsInternalPropertyChange = false;
                }
                this.Width = value;
            }
        }

        public double ItemTop
        {
            get
            {
                return Canvas.GetTop(this);
            }
            set
            {
                if (this.ItemTop != value)
                {
                    this.Properties.IsInternalPropertyChange = true;
                    this.Properties.Top = propertyValueConvertor.GetSizeValue(value, this.Properties.Top);
                    this.Properties.IsInternalPropertyChange = false;
                }
                Canvas.SetTop(this, value);
            }
        }

        public double ItemLeft
        {
            get
            {
                return Canvas.GetLeft(this);
            }
            set
            {
                if (this.ItemLeft != value)
                {
                    this.Properties.IsInternalPropertyChange = true;
                    this.Properties.Left = propertyValueConvertor.GetSizeValue(value, this.Properties.Left);
                    this.Properties.IsInternalPropertyChange = false;
                }
                Canvas.SetLeft(this, value);
            }
        }

        public bool IsTablixItem
        {
            get;
            set;
        }

        public bool IsItemSelected
        {
            get
            {
                return this.isItemSelected;
            }
            set
            {
                if (this.isItemSelected != value)
                {
                    isItemSelected = value;
                    this.RaiseReportItemSelectedEvent(new SelectedItemEventArgs() { SelectedItem = this.Properties, IsSelected = value });
                    OnPropertyChanged("IsItemSelected");
                }
            }
        }

        public bool IsFocusedItem
        {
            get
            {
                return isFocusedItem;
            }
            set
            {
                if (isFocusedItem != value)
                {
                    this.isFocusedItem = value;
                    OnPropertyChanged("IsFocusedItem");
                }
            }
        }


        public new Canvas Parent { get; set; }

        public DrawingReportItem ItemType
        {
            get
            {
                return DrawingReportItem.Rectangle;
            }
        }

        public event ReportItemControlSizeHandler ReportItemSizeChanged;

        public void RaiseReportItemSizeChangedEvent()
        {
            if (this.ReportItemSizeChanged != null)
            {
                this.ReportItemSizeChanged(this, new EventArgs());
            }
        }

        public event ReportItemSelectedEvent ReportItemSelected;

        public void RaiseReportItemSelectedEvent(SelectedItemEventArgs selectedObjectArg)
        {
            if (this.ReportItemSelected != null)
            {
                this.ReportItemSelected(this, selectedObjectArg);
            }
        }

        public ImageSource GetImageSource()
        {
            this.Arrange(new Rect(0, 0, this.ActualWidth, this.ActualHeight));
            this.UpdateLayout();
            System.Windows.Media.Imaging.RenderTargetBitmap rtb = new System.Windows.Media.Imaging.RenderTargetBitmap((int)this.ActualWidth, (int)this.ActualHeight, 96, 96, PixelFormats.Default);

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext ctx = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(this);
                ctx.DrawRectangle(vb, null, new Rect(new Point(), new System.Windows.Size(this.ActualWidth, this.ActualHeight)));
            }

            rtb.Render(dv);
            return rtb;
        }

        public Syncfusion.RDL.DOM.ReportItem GetReportItem()
        {
            RDL.DOM.Rectangle rect = new RDL.DOM.Rectangle();
            rect.ReportItems = new RDL.DOM.ReportItems();

            foreach (var control in this.Children)
            {
                if (control is IReportItemControl)
                {
                    IReportItemControl reportItemControl = control as IReportItemControl;
                    rect.ReportItems.Add(reportItemControl.GetReportItem());
                }
            }

            rect.Name = this.Properties.Name;
            rect.Visibility = new RDL.DOM.Visibility();
            rect.KeepTogether = Convert.ToBoolean(this.Properties.KeepTogether);
            rect.OmitBorderOnPageBreak = Convert.ToBoolean(this.Properties.OmitBorderOnPageBreak);

            if (this.Properties.ToggleItem != null)
            {
                rect.Visibility.ToggleItem = this.Properties.ToggleItem;
            }
            if (this.Properties.Hidden == "True" || this.Properties.Hidden.StartsWith("="))
            {
                rect.Visibility.Hidden = this.Properties.Hidden;
            }

            rect.Height = new RDL.DOM.Size(this.ItemHeight / 96 + DesignPanel.GetMeasuredUnit(this.Properties.Height));
            rect.Width = new RDL.DOM.Size(this.ItemWidth / 96 + DesignPanel.GetMeasuredUnit(this.Properties.Width));
            rect.Left = new RDL.DOM.Size(this.ItemLeft / 96 + DesignPanel.GetMeasuredUnit(this.Properties.Left));
            rect.Top = new RDL.DOM.Size(this.ItemTop / 96 + DesignPanel.GetMeasuredUnit(this.Properties.Top));

            rect.Style = new RDL.DOM.Style();

            if (!string.IsNullOrEmpty(this.Properties.BackgroundImage.ImageValue))
            {
                rect.Style.BackgroundImage = new RDL.DOM.BackgroundImage();
                rect.Style.BackgroundImage.Value = this.Properties.BackgroundImage.ImageValue;
                rect.Style.BackgroundImage.Source = this.Properties.BackgroundImage.Source;
                if (this.Properties.BackgroundImage.MIMEType != null)
                    rect.Style.BackgroundImage.MIMEType = this.Properties.BackgroundImage.MIMEType;
            }
            if (!string.IsNullOrEmpty(this.Properties.DocumentMapLabel))
            {
                rect.DocumentMapLabel = this.Properties.DocumentMapLabel;
            }
            if (this.Properties.PageBreak != RDL.DOM.BreakLocation.None)
            {
                rect.PageBreak = new RDL.DOM.PageBreak();
                rect.PageBreak.BreakLocation = this.Properties.PageBreak;
            }

            rect.Style.BackgroundColor = this.Properties.BackgroundColor;
            rect.Style.Border = new RDL.DOM.Border();
            rect.Style.Border.Color = this.Properties.BorderColors.DefaultBorderColor;
            rect.Style.Border.Width = this.Properties.BorderWidths.DefaultBorderWidth;
            rect.Style.Border.Style = this.Properties.BorderStyles.DefaultBorderStyle;

            if (this.Properties.BorderWidths.LeftBorderWidth != null || this.Properties.BorderStyles.LeftBorderStyle != null
                || this.Properties.BorderColors.LeftBorderColor != null)
            {
                rect.Style.LeftBorder = new RDL.DOM.LeftBorder();
                rect.Style.LeftBorder.Color = this.Properties.BorderColors.LeftBorderColor;
                rect.Style.LeftBorder.Width = this.Properties.BorderWidths.LeftBorderWidth;
                rect.Style.LeftBorder.Style = this.Properties.BorderStyles.LeftBorderStyle;
            }
            if (this.Properties.BorderWidths.RightBorderWidth != null || this.Properties.BorderStyles.RightBorderStyle != null
                || this.Properties.BorderColors.RightBorderColor != null)
            {
                rect.Style.RightBorder = new RDL.DOM.RightBorder();
                rect.Style.RightBorder.Color = this.Properties.BorderColors.RightBorderColor;
                rect.Style.RightBorder.Width = this.Properties.BorderWidths.RightBorderWidth;
                rect.Style.RightBorder.Style = this.Properties.BorderStyles.RightBorderStyle;
            }
            if (this.Properties.BorderWidths.TopBorderWidth != null || this.Properties.BorderStyles.TopBorderStyle != null
                || this.Properties.BorderColors.TopBorderColor != null)
            {
                rect.Style.TopBorder = new RDL.DOM.TopBorder();
                rect.Style.TopBorder.Color = this.Properties.BorderColors.TopBorderColor;
                rect.Style.TopBorder.Width = this.Properties.BorderWidths.TopBorderWidth;
                rect.Style.TopBorder.Style = this.Properties.BorderStyles.TopBorderStyle;
            }
            if (this.Properties.BorderWidths.BottomBorderWidth != null || this.Properties.BorderStyles.BottomBorderStyle != null
                || this.Properties.BorderColors.BottomBorderColor != null)
            {
                rect.Style.BottomBorder = new RDL.DOM.BottomBorder();
                rect.Style.BottomBorder.Color = this.Properties.BorderColors.BottomBorderColor;
                rect.Style.BottomBorder.Width = this.Properties.BorderWidths.BottomBorderWidth;
                rect.Style.BottomBorder.Style = this.Properties.BorderStyles.BottomBorderStyle;
            }
            if (!string.IsNullOrEmpty(this.Properties.DataElementName))
            {
                rect.DataElementName = this.Properties.DataElementName;
            }
            if (this.Properties.DataElementOutput != "Auto")
            {
                rect.DataElementOutput = (RDL.DOM.DataElementOutputs)Enum.Parse(typeof(RDL.DOM.DataElementOutputs), this.Properties.DataElementOutput);
            }

            return rect;
        }

        public void RestoreReportItem(RDL.DOM.ReportItem reportItem)
        {
            this.Properties.IsInternalPropertyChange = true;

            this.ReportItem = reportItem;

            if (this.ReportItem != null)
            {
                this.PopulateReportItem();
                this.ReportItem = null;
            }

            this.Properties.IsInternalPropertyChange = false;
        }

        public void UpdateItemSizeProperties()
        {
            this.Properties.IsInternalPropertyChange = true;
            this.Properties.Width = propertyValueConvertor.GetSizeValue(this.ItemWidth, this.Properties.Width);
            this.Properties.Top = propertyValueConvertor.GetSizeValue(this.ItemTop, this.Properties.Top);
            this.Properties.Left = propertyValueConvertor.GetSizeValue(this.ItemLeft, this.Properties.Left);
            this.Properties.Height = propertyValueConvertor.GetSizeValue(this.ItemHeight, this.Properties.Height);
            this.Properties.IsInternalPropertyChange = false;
        }

        #endregion

        public RectangleControl(RDL.DOM.ReportItem reportItem)
            : base()
        {
            Binding bind = new Binding();
            bind.Source = this;
            bind.Path = new System.Windows.PropertyPath("ActualWidth");
            border.SetBinding(Border.WidthProperty, bind);

            bind = new Binding();
            bind.Source = this;
            bind.Path = new System.Windows.PropertyPath("ActualHeight");
            border.SetBinding(Border.HeightProperty, bind);
            this.ContextMenu = new ContextMenu();
            
            MenuItem rectproperties = new MenuItem { Header = SR.GetString(CultureInfo.CurrentUICulture,"headerRectangleProperties") };
            this.ContextMenu.Items.Add(rectproperties);
            rectproperties.Click += new System.Windows.RoutedEventHandler(rectproperties_Click);
            selectionbox.Stroke = Brushes.Black;
            selectionbox.StrokeThickness = 1;
            this.Children.Add(selectionbox);
            this.Children.Add(border);
            this.Properties.PropertyChanged += new PropertyChangedEventHandler(Properties_PropertyChanged);
            this.Properties.PropertyChanging += new PropertyChangingEventHandler(Properties_PropertyChanging);
            this.Properties.IsInternalPropertyChange = true;
            this.Properties.Name = this.ItemName;
            this.Properties.UpdatePropertyValue();

            if (reportItem != null)
            {
                this.ReportItem = reportItem;
                this.PopulateReportItem();
            }

            this.Loaded += new RoutedEventHandler(RectangleControl_Loaded);
        }

        void Properties_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            object propertyValue = null;
            string propertyName = e.PropertyName.ToUpper();

            switch (propertyName)
            {
                case "DEFAULTBORDERSTYLE":
                    {
                        propertyValue = this.Properties.BorderStyles.DefaultBorderStyle;
                        break;
                    }
                case "LEFTBORDERSTYLE":
                    {
                        propertyValue = this.Properties.BorderStyles.LeftBorderStyle;
                        break;
                    }
                case "RIGHTBORDERSTYLE":
                    {
                        propertyValue = this.Properties.BorderStyles.RightBorderStyle;
                        break;
                    }
                case "TOPBORDERSTYLE":
                    {
                        propertyValue = this.Properties.BorderStyles.TopBorderStyle;
                        break;
                    }
                case "BOTTOMBORDERSTYLE":
                    {
                        propertyValue = this.Properties.BorderStyles.BottomBorderStyle;
                        break;
                    }
                case "DEFAULTBORDERWIDTH":
                    {
                        propertyValue = this.Properties.BorderWidths.DefaultBorderWidth;
                        break;
                    }
                case "LEFTBORDERWIDTH":
                    {
                        propertyValue = this.Properties.BorderWidths.LeftBorderWidth;
                        break;
                    }
                case "RIGHTBORDERWIDTH":
                    {
                        propertyValue = this.Properties.BorderWidths.RightBorderWidth;
                        break;
                    }
                case "TOPBORDERWIDTH":
                    {
                        propertyValue = this.Properties.BorderWidths.TopBorderWidth;
                        break;
                    }
                case "BOTTOMBORDERWIDTH":
                    {
                        propertyValue = this.Properties.BorderWidths.BottomBorderWidth;
                        break;
                    }
                case "DEFAULTBORDERCOLOR":
                    {
                        propertyValue = this.Properties.BorderColors.DefaultBorderColor;
                        break;
                    }
                case "LEFTBORDERCOLOR":
                    {
                        propertyValue = this.Properties.BorderColors.LeftBorderColor;
                        break;
                    }
                case "RIGHTBORDERCOLOR":
                    {
                        propertyValue = this.Properties.BorderColors.RightBorderColor;
                        break;
                    }
                case "TOPBORDERCOLOR":
                    {
                        propertyValue = this.Properties.BorderColors.TopBorderColor;
                        break;
                    }
                case "BOTTOMBORDERCOLOR":
                    {
                        propertyValue = this.Properties.BorderColors.BottomBorderColor;
                        break;
                    }
                case "BACKGROUNDCOLOR":
                    {
                        propertyValue = this.Properties.BackgroundColor;
                        break;
                    }
                case "NAME":
                    {
                        propertyValue = this.Properties.Name;
                        break;
                    }
                case "TOGGLEITEM":
                    {
                        propertyValue = this.Properties.ToggleItem;
                        break;
                    }
                case "IMAGEVALUE":
                    {
                        propertyValue = this.Properties.BackgroundImage.ImageValue;
                        break;
                    }
                case "SOURCE":
                    {
                        propertyValue = this.Properties.BackgroundImage.Source;
                        break;
                    }
                case "MIMETYPE":
                    {
                        propertyValue = this.Properties.BackgroundImage.MIMEType;
                        break;
                    }
                case "BACKGROUNDREPEAT":
                    {
                        propertyValue = this.Properties.BackgroundImage.MIMEType;
                        break;
                    }
                case "HIDDEN":
                    {
                        propertyValue = this.Properties.Hidden;
                        break;
                    }
                case "KEEPTOGETHER":
                    {
                        propertyValue = this.Properties.KeepTogether;
                        break;
                    }
                case "DOCUMENTMAPLABEL":
                    {
                        propertyValue = this.Properties.DocumentMapLabel;
                        break;
                    }
                case "PAGEBREAK":
                    {
                        propertyValue = this.Properties.PageBreak;
                        break;
                    }
                case "LEFT":
                    {
                        propertyValue = this.Properties.Left;
                        break;
                    }
                case "TOP":
                    {
                        propertyValue = this.Properties.Top;
                        break;
                    }
                case "HEIGHT":
                    {
                        propertyValue = this.Properties.Height;
                        break;
                    }
                case "WIDTH":
                    {
                        propertyValue = this.Properties.Width;
                        break;
                    }
                case "OMITBORDERONPAGEBREAK":
                    {
                        propertyValue = this.Properties.OmitBorderOnPageBreak;
                        break;
                    }
            }
            this.propertyOldValue = propertyValue;

        }

        void Properties_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            object propertyValue = null;
            string propertyName = e.PropertyName.ToUpper();

            switch (propertyName)
            {
                case "DEFAULTBORDERSTYLE":
                    {
                        propertyValue = this.Properties.BorderStyles.DefaultBorderStyle;
                        this.border.DashStyle = this.propertyValueConvertor.GetBorderStyle(this.Properties.BorderStyles.DefaultBorderStyle);
                        break;
                    }
                case "LEFTBORDERSTYLE":
                    {
                        propertyValue = this.Properties.BorderStyles.LeftBorderStyle;
                        break;
                    }
                case "RIGHTBORDERSTYLE":
                    {
                        propertyValue = this.Properties.BorderStyles.RightBorderStyle;
                        break;
                    }
                case "TOPBORDERSTYLE":
                    {
                        propertyValue = this.Properties.BorderStyles.TopBorderStyle;
                        break;
                    }
                case "BOTTOMBORDERSTYLE":
                    {
                        propertyValue = this.Properties.BorderStyles.BottomBorderStyle;
                        break;
                    }
                case "DEFAULTBORDERWIDTH":
                    {
                        propertyValue = this.Properties.BorderWidths.DefaultBorderWidth;
                        this.border.BorderThickness = this.propertyValueConvertor.GetBorderThickness(this.Properties.BorderWidths.DefaultBorderWidth);
                        break;
                    }
                case "LEFTBORDERWIDTH":
                    {
                        propertyValue = this.Properties.BorderWidths.LeftBorderWidth;
                        break;
                    }
                case "RIGHTBORDERWIDTH":
                    {
                        propertyValue = this.Properties.BorderWidths.RightBorderWidth;
                        break;
                    }
                case "TOPBORDERWIDTH":
                    {
                        propertyValue = this.Properties.BorderWidths.TopBorderWidth;
                        break;
                    }
                case "BOTTOMBORDERWIDTH":
                    {
                        propertyValue = this.Properties.BorderWidths.BottomBorderWidth;
                        break;
                    }
                case "DEFAULTBORDERCOLOR":
                    {
                        propertyValue = this.Properties.BorderColors.DefaultBorderColor;
                        this.border.BorderBrush = this.propertyValueConvertor.GetColor(this.Properties.BorderColors.DefaultBorderColor);
                        break;
                    }
                case "LEFTBORDERCOLOR":
                    {
                        propertyValue = this.Properties.BorderColors.LeftBorderColor;
                        break;
                    }
                case "RIGHTBORDERCOLOR":
                    {
                        propertyValue = this.Properties.BorderColors.RightBorderColor;
                        break;
                    }
                case "TOPBORDERCOLOR":
                    {
                        propertyValue = this.Properties.BorderColors.TopBorderColor;
                        break;
                    }
                case "BOTTOMBORDERCOLOR":
                    {
                        propertyValue = this.Properties.BorderColors.BottomBorderColor;
                        break;
                    }
                case "BACKGROUNDCOLOR":
                    {
                        propertyValue = this.Properties.BackgroundColor;
                        if (string.IsNullOrEmpty(this.Properties.BackgroundImage.ImageValue))
                        {
                            this.Background = this.propertyValueConvertor.GetBackGroundColor(this.Properties.BackgroundColor);
                        }
                        break;
                    }
                case "NAME":
                    {
                        propertyValue = this.Properties.Name;
                        this.ItemName = this.Properties.Name;
                        break;
                    }
                case "HIDDEN":
                    {
                        propertyValue = this.Properties.Hidden;
                        break;
                    }
                case "TOGGLEITEM":
                    {
                        propertyValue = this.Properties.ToggleItem;
                        break;
                    }
                case "IMAGEVALUE":
                    {
                        this.UpdateBackgroundImage();
                        propertyValue = this.Properties.BackgroundImage.ImageValue;
                        break;
                    }
                case "SOURCE":
                    {
                        this.UpdateBackgroundImage();
                        propertyValue = this.Properties.BackgroundImage.Source;
                        break;
                    }
                case "MIMETYPE":
                    {
                        propertyValue = this.Properties.BackgroundImage.MIMEType;
                        break;
                    }
                case "BACKGROUNDREPEAT":
                    {
                        propertyValue = this.Properties.BackgroundImage.MIMEType;
                        break;
                    }
                case "KEEPTOGETHER":
                    {
                        propertyValue = this.Properties.KeepTogether;
                        break;
                    }
                case "DOCUMENTMAPLABEL":
                    {
                        propertyValue = this.Properties.DocumentMapLabel;
                        break;
                    }
                case "PAGEBREAK":
                    {
                        propertyValue = this.Properties.PageBreak;
                        break;
                    }
                case "LEFT":
                    {
                        propertyValue = this.Properties.Left;
                        if (!this.Properties.IsInternalPropertyChange && !string.IsNullOrEmpty(this.Properties.Left))
                            this.ItemLeft = new RDL.DOM.Size(this.Properties.Left).PixelValue;
                        break;
                    }
                case "TOP":
                    {
                        propertyValue = this.Properties.Top;
                        if (!this.Properties.IsInternalPropertyChange && !string.IsNullOrEmpty(this.Properties.Top))
                            this.ItemTop = new RDL.DOM.Size(this.Properties.Top).PixelValue;
                        break;
                    }
                case "HEIGHT":
                    {
                        propertyValue = this.Properties.Height;
                        if (!this.Properties.IsInternalPropertyChange && !string.IsNullOrEmpty(this.Properties.Height))
                            this.ItemHeight = new RDL.DOM.Size(this.Properties.Height).PixelValue;
                        break;
                    }
                case "WIDTH":
                    {
                        propertyValue = this.Properties.Width;
                        if (!this.Properties.IsInternalPropertyChange && !string.IsNullOrEmpty(this.Properties.Width))
                            this.ItemWidth = new RDL.DOM.Size(this.Properties.Width).PixelValue;
                        break;
                    }
                case "OMITBORDERONPAGEBREAK":
                    {
                        propertyValue = this.Properties.OmitBorderOnPageBreak;
                        break;
                    }
            }

            if (!this.Properties.IsInternalPropertyChange)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemPropertyChanged;
                PropertyChanage change = new PropertyChanage();
                change.PropertyObject = this.Properties;

                switch (propertyName)
                {
                    case "DEFAULTBORDERSTYLE":
                    case "LEFTBORDERSTYLE":
                    case "RIGHTBORDERSTYLE":
                    case "TOPBORDERSTYLE":
                    case "BOTTOMBORDERSTYLE":
                        change.PropertyObject = this.Properties.BorderStyles;
                        break;

                    case "DEFAULTBORDERCOLOR":
                    case "LEFTBORDERCOLOR":
                    case "RIGHTBORDERCOLOR":
                    case "TOPBORDERCOLOR":
                    case "BOTTOMBORDERCOLOR":
                        change.PropertyObject = this.Properties.BorderColors;
                        break;

                    case "DEFAULTBORDERWIDTH":
                    case "LEFTBORDERWIDTH":
                    case "RIGHTBORDERWIDTH":
                    case "TOPBORDERWIDTH":
                    case "BOTTOMBORDERWIDTH":
                        change.PropertyObject = this.Properties.BorderWidths;
                        break;

                    case "SOURCE":
                    case "MIMETYPE":
                    case "IMAGEVALUE":
                        change.PropertyObject = this.Properties.BackgroundImage;
                        break;
                }

                change.PropertyName = e.PropertyName;
                change.OldValue = this.propertyOldValue;
                change.NewValue = propertyValue;
                action.PropertyChange = change;
                this.Panel.EditingManager.AddAction(action);
                if (propertyName == "TOP" || propertyName == "LEFT" || propertyName == "HEIGHT" || propertyName == "WIDTH")
                {
                    this.Panel.EditingManager.IsMergeAction = true;
                    this.RaiseReportItemSizeChangedEvent();
                    this.Panel.EditingManager.IsMergeAction = false;
                }
            }
        }

        internal void UpdateBackgroundImage()
        {
            if (this.Properties.BackgroundImage.Source == RDL.DOM.Source.Embedded &&
                !string.IsNullOrEmpty(this.Properties.BackgroundImage.ImageValue) && this.Panel != null)
            {
                RDL.DOM.EmbeddedImage embededimage = (from image in this.Panel.EmbeddedImages
                                                  where image.Name.Equals(this.Properties.BackgroundImage.ImageValue)
                                                  select image).FirstOrDefault();

                if (embededimage != null && embededimage.MIMEType != "image/emf")
                {
                    Base64ImageConverter converter = new Base64ImageConverter();
                    System.Windows.Media.Imaging.BitmapImage bitmapImage = converter.ConvertToImage(embededimage.ImageData);
                    ImageBrush brush = new ImageBrush(bitmapImage);
                    this.Background = brush;
                }
                else
                {
                    this.Background = this.propertyValueConvertor.GetBackGroundColor(this.Properties.BackgroundColor);
                }
            }
            else
            {
                this.Background = this.propertyValueConvertor.GetBackGroundColor(this.Properties.BackgroundColor);
            }
        }

        void RectangleControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= new RoutedEventHandler(RectangleControl_Loaded);
            RDL.DOM.Rectangle rect = this.ReportItem as RDL.DOM.Rectangle;

            bool internalChange = this.Panel.IsInternalChange;
            this.Panel.IsInternalChange = true;            

            if (rect!=null && rect.ReportItems != null)
            {
                foreach (var reportItem in rect.ReportItems)
                {
                    this.Panel.AddReportItem(reportItem,this);
                }
            }
            
            this.Properties.IsInternalPropertyChange = false;
            this.Panel.IsInternalChange = internalChange;

        }

        public void PopulateReportItem()
        {
            RDL.DOM.Rectangle rectBase= this.ReportItem as RDL.DOM.Rectangle;
            this.Properties.Name = rectBase.Name;
            this.Properties.DocumentMapLabel = rectBase.DocumentMapLabel;
            this.Properties.KeepTogether = rectBase.KeepTogether.ToString();

            if (rectBase.Height != null)
            {
                this.Properties.Height = rectBase.Height.size;
            }
            if (rectBase.Width != null)
            {
                this.Properties.Width = rectBase.Width.size;
            }
            if (rectBase.Top != null)
            {
                this.Properties.Top = rectBase.Top.size;
            }
            if (rectBase.Left != null)
            {
                this.Properties.Left = rectBase.Left.size;
            }
            if (rectBase.Visibility != null)
            {
                if (rectBase.Visibility.ToggleItem != null)
                    this.Properties.ToggleItem = rectBase.Visibility.ToggleItem;
                if (rectBase.Visibility.Hidden != null)
                    this.Properties.Hidden = rectBase.Visibility.Hidden;
            }
            if (rectBase.PageBreak != null )
            {
                this.Properties.PageBreak = rectBase.PageBreak.BreakLocation;
            }
            if (rectBase.OmitBorderOnPageBreak != false)
            {
                this.Properties.OmitBorderOnPageBreak = rectBase.OmitBorderOnPageBreak.ToString();
            }
            if (rectBase.Style != null)
            {
                this.Properties.BackgroundColor = rectBase.Style.BackgroundColor;

                if (rectBase.Style.Border != null)
                {
                    if (rectBase.Style.Border.Style != null)
                        this.Properties.BorderStyles.DefaultBorderStyle = rectBase.Style.Border.Style;
                    if (rectBase.Style.Border.Color != null)
                        this.Properties.BorderColors.DefaultBorderColor = rectBase.Style.Border.Color;
                    if (rectBase.Style.Border.Width != null)
                        this.Properties.BorderWidths.DefaultBorderWidth = rectBase.Style.Border.Width.size;
                 
                }
                if (rectBase.Style.LeftBorder != null)
                {
                    if (rectBase.Style.LeftBorder.Color != null)
                        this.Properties.BorderColors.LeftBorderColor = rectBase.Style.LeftBorder.Color;
                    if (rectBase.Style.LeftBorder.Width != null)
                        this.Properties.BorderWidths.LeftBorderWidth = rectBase.Style.LeftBorder.Width.size;
                    if (rectBase.Style.LeftBorder.Style != null)
                        this.Properties.BorderStyles.LeftBorderStyle = rectBase.Style.LeftBorder.Style;
                }
                if (rectBase.Style.RightBorder != null)
                {
                    if (rectBase.Style.RightBorder.Color != null)
                        this.Properties.BorderColors.RightBorderColor = rectBase.Style.RightBorder.Color;
                    if (rectBase.Style.RightBorder.Width != null)
                        this.Properties.BorderWidths.RightBorderWidth = rectBase.Style.RightBorder.Width.size;
                    if (rectBase.Style.RightBorder.Style != null)
                        this.Properties.BorderStyles.RightBorderStyle = rectBase.Style.RightBorder.Style;
                }
                if (rectBase.Style.TopBorder != null)
                {
                    if (rectBase.Style.TopBorder.Color != null)
                        this.Properties.BorderColors.TopBorderColor = rectBase.Style.TopBorder.Color;
                    if (rectBase.Style.TopBorder.Width != null)
                        this.Properties.BorderWidths.TopBorderWidth = rectBase.Style.TopBorder.Width.size;
                    if (rectBase.Style.TopBorder.Style != null)
                        this.Properties.BorderStyles.TopBorderStyle = rectBase.Style.TopBorder.Style;
                }
                if (rectBase.Style.BottomBorder != null)
                {
                    if (rectBase.Style.BottomBorder.Color != null)
                        this.Properties.BorderColors.BottomBorderColor = rectBase.Style.BottomBorder.Color;
                    if (rectBase.Style.BottomBorder.Width != null)
                        this.Properties.BorderWidths.BottomBorderWidth = rectBase.Style.BottomBorder.Width.size;
                    if (rectBase.Style.BottomBorder.Style != null)
                        this.Properties.BorderStyles.BottomBorderStyle = rectBase.Style.BottomBorder.Style;
                }
                if (rectBase.Style.BackgroundImage != null)
                {
                    this.Properties.BackgroundImage.Source = rectBase.Style.BackgroundImage.Source;
                    this.Properties.BackgroundImage.ImageValue = rectBase.Style.BackgroundImage.Value;
                    if (rectBase.Style.BackgroundImage.MIMEType != null)
                        this.Properties.BackgroundImage.MIMEType = rectBase.Style.BackgroundImage.MIMEType;
                }
            }
        }

        void rectproperties_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ControlProperties controlProperties = UpdateRectangleProperties();

            if (controlProperties.ShowDialog() == true)
            {
                this.Properties.IsInternalPropertyChange = true;
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemChanged;
                ItemChange change = new ItemChange();
                change.ReportItem = this;
                change.OldValue = this.GetReportItem();
                action.ItemChange = change;
                this.Panel.EditingManager.AddAction(action);
                change.NewValue = this.GetReportItem();
                this.Properties.IsInternalPropertyChange = false;

                foreach (UIElement uiElement in controlProperties.grd_PlaceHolder.Children)
                {
                    if (uiElement is RectangleGeneral)
                    {
                        this.Properties.Name = controlProperties.RectangleGeneral.txt_GeneralName.Text;
                    }
                    else if (uiElement is RectangleStyle)
                    {
                        this.Properties.BorderWidths.DefaultBorderWidth = controlProperties.RectangleStyle.updwn_Custom.TextValue;
                        this.Properties.BorderColors.DefaultBorderColor = controlProperties.RectangleStyle.borderColor.Text;
                        this.Properties.BorderStyles.DefaultBorderStyle = controlProperties.RectangleStyle.cmb_style.TextValue;
                    }
                    else if (uiElement is RectangleFill)
                    {
                        this.Properties.BackgroundColor = controlProperties.RectangleFill.clrpkr_Fill.Text;
                    }
                }
            }
        }

        private ControlProperties UpdateRectangleProperties()
        {
            ControlProperties controlProperties = new ControlProperties(this);
            this.Panel.UpdateOwnerWindow(controlProperties);

            if (this.Properties.BorderColors != null)
            {
                controlProperties.RectangleStyle.borderColor.Text = this.Properties.BorderColors.DefaultBorderColor;
            }

            //controlProperties.RectangleStyle.updwn_Custom.Value = new RDL.DOM.Size(this.Properties.BorderWidth).PixelValue;
            controlProperties.RectangleStyle.updwn_Custom.TextValue = this.Properties.BorderWidths.DefaultBorderWidth;
            controlProperties.RectangleGeneral.txt_GeneralName.Text = this.Properties.Name;
            controlProperties.RectangleStyle.cmb_style.TextValue = this.Properties.BorderStyles.DefaultBorderStyle;
            if(this.Properties.BackgroundColor!=null)
            controlProperties.RectangleFill.clrpkr_Fill.Text = this.Properties.BackgroundColor;

            return controlProperties;
        }
    }
}
