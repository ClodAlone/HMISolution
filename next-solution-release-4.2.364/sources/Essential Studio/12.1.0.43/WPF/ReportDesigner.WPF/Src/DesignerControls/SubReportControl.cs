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
using Syncfusion.Windows.Reports.Designer.Dialogs;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Media;
using Syncfusion.Windows.ReportDesigner.Resources;
using System.Globalization;

namespace Syncfusion.Windows.Reports.Designer.Controls
{
#if SyncfusionFramework4_0
        [DesignTimeVisible(false)]
#endif
    internal class SubReportControl : Label, IReportItemControl, INotifyPropertyChanged
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
        internal Syncfusion.Windows.Reports.Designer.Editors.SubReportProperties Properties;
        internal ReportingConvertorUtil propertyValueConvertor;
        
        # region Variables

        //private string TagName = "Subreport";
        private string str = string.Empty;
        private bool isFocusedItem;
        private bool isItemSelected;
        private object propertyOldValue = null;


        # endregion

        # region Properties Exposed

        private System.Windows.Controls.Label label { get; set; }
        private MenuItem SubReportDelete { get; set; }
        private MenuItem SubReportProperties { get; set; }
        internal Syncfusion.RDL.DOM.SubReport subReportBase { get; set; }
        internal event DeleteSubReportClickEventHandler DeleteSubReport;

        # endregion

        # region Constructors

        public SubReportControl()
        {
            this.Properties = new Editors.SubReportProperties();
            this.propertyValueConvertor = new ReportingConvertorUtil();
            base.OnApplyTemplate();
            this.BorderBrush = Brushes.Black;
            this.BorderThickness = new Thickness(1);
            this.Background = Brushes.LightGray;
            this.Content = "<subreport>";
            this.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
            this.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
            this.label = this;

            this.SubReportDelete = new MenuItem { Header=SR.GetString(CultureInfo.CurrentUICulture, "headerDelete")};
            this.SubReportProperties = new MenuItem
            {
                Header = SR.GetString(CultureInfo.CurrentUICulture,"Properties")
            };

            this.label.ContextMenu = new ContextMenu();
            this.label.ContextMenu.Items.Add(this.SubReportDelete);
            this.label.ContextMenu.Items.Add(this.SubReportProperties);
            if (this.label != null)
            {
                this.SubReportDelete.Click += new RoutedEventHandler(SubReportDelete_Click);
                this.SubReportProperties.Click += new RoutedEventHandler(SubReportProperties_Click);
                if (str == string.Empty)
                {
                    this.subReportBase = UpdateSubReportBase();
                }
                else
                {
                    UpdateOldSubReport(this.subReportBase);
                }
            }
            this.Properties.PropertyChanged += new PropertyChangedEventHandler(Properties_PropertyChanged);
            this.Properties.PropertyChanging += new PropertyChangingEventHandler(Properties_PropertyChanging);
            this.Properties.IsInternalPropertyChange = true;
            this.Properties.UpdatePropertyValue();
            this.Loaded += new RoutedEventHandler(SubReportControl_Loaded);

        }

        void SubReportControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Properties.IsInternalPropertyChange = false;
        }

        void Properties_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            object propertyValue = null;
            string propertyName = e.PropertyName.ToUpper();

            switch (propertyName)
            {
                case "PADDINGLEFT":
                    {
                        propertyValue = this.Properties.Padding.PaddingLeft;
                        break;
                    }
                case "PADDINGRIGHT":
                    {
                        propertyValue = this.Properties.Padding.PaddingRight;
                        break;
                    }
                case "PADDINGTOP":
                    {
                        propertyValue = this.Properties.Padding.PaddingTop;
                        break;
                    }
                case "PADDINGBOTTOM":
                    {
                        propertyValue = this.Properties.Padding.PaddingBottom;
                        break;
                    }
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
                case "NAME":
                    {
                        propertyValue = this.Properties.Name;
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
                case "FORMAT":
                    {
                        propertyValue = this.Properties.Format;
                        break;
                    }
                case "REPORTNAME":
                    {
                        propertyValue = this.Properties.ReportName;
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
                case "DATAELEMENTNAME":
                    {
                        propertyValue = this.Properties.DataElementName;
                        break;
                    }
                case "DATAELEMENTOUTPUT":
                    {
                        propertyValue = this.Properties.DataElementOutput;
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
                case "PADDINGLEFT":
                    {
                        propertyValue = this.Properties.Padding.PaddingLeft;
                        break;
                    }
                case "PADDINGRIGHT":
                    {
                        propertyValue = this.Properties.Padding.PaddingRight;
                        break;
                    }
                case "PADDINGTOP":
                    {
                        propertyValue = this.Properties.Padding.PaddingTop;
                        break;
                    }
                case "PADDINGBOTTOM":
                    {
                        propertyValue = this.Properties.Padding.PaddingBottom;
                        break;
                    }
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
                case "FORMAT":
                    {
                        propertyValue = this.Properties.Format;
                        break;
                    }
                case "REPORTNAME":
                    {
                        propertyValue = this.Properties.ReportName;
                        if (!string.IsNullOrEmpty(this.Properties.ReportName))
                        {
                            this.Content = this.Properties.ReportName;
                        }
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
                case "DATAELEMENTNAME":
                    {
                        propertyValue = this.Properties.DataElementName;
                        break;
                    }
                case "DATAELEMENTOUTPUT":
                    {
                        propertyValue = this.Properties.DataElementOutput;
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
                    case "PADDINGLEFT":
                    case "PADDINGRIGHT":
                    case "PADDINGTOP":
                    case "PADDINGBOTTOM":
                        change.PropertyObject = this.Properties.Padding;
                        break;

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

        internal SubReportControl(Syncfusion.RDL.DOM.SubReport SubReportBase)
        {
            str = "DeSerialization";
            this.subReportBase = SubReportBase;
        }

        # endregion

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
                    this.RaiseReportItemSelectedEvent(new SelectedItemEventArgs() { SelectedItem = this.Properties,IsSelected=value });
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
                return DrawingReportItem.SubReport;
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
            Syncfusion.RDL.DOM.SubReport subreport = new Syncfusion.RDL.DOM.SubReport();

            subreport.Name = this.Properties.Name;
            subreport.DocumentMapLabel = this.Properties.DocumentMapLabel;
            subreport.Visibility = new RDL.DOM.Visibility();
            subreport.Style = new RDL.DOM.Style();
            subreport.ReportName = this.Properties.ReportName;
            subreport.KeepTogether = Convert.ToBoolean(this.Properties.KeepTogether);
            subreport.OmitBorderOnPageBreak = Convert.ToBoolean(this.Properties.OmitBorderOnPageBreak);

            subreport.Height = new RDL.DOM.Size(this.ItemHeight / 96 + DesignPanel.GetMeasuredUnit(this.Properties.Height));
            subreport.Width = new RDL.DOM.Size(this.ItemWidth / 96 + DesignPanel.GetMeasuredUnit(this.Properties.Width));
            subreport.Left = new RDL.DOM.Size(this.ItemLeft / 96 + DesignPanel.GetMeasuredUnit(this.Properties.Left));
            subreport.Top = new RDL.DOM.Size(this.ItemTop / 96 + DesignPanel.GetMeasuredUnit(this.Properties.Top));

            if (this.Properties.ToggleItem != null)
            {
                subreport.Visibility.ToggleItem = this.Properties.ToggleItem;
            }            
            if (this.Properties.Hidden == "True" || this.Properties.Hidden.StartsWith("="))
            {
                subreport.Visibility.Hidden = this.Properties.Hidden;
            }
            if (this.Properties.Format != null)
            {
                subreport.Style.Format = this.Properties.Format;
            }
            if (this.Properties.HorizontalAlignment == "Right" || this.Properties.HorizontalAlignment == "Center")
            {
                subreport.Style.TextAlign = this.Properties.HorizontalAlignment;
            }
            if (this.Properties.VerticalAlignment == "Top" || this.Properties.VerticalAlignment == "Middle"
                || this.Properties.VerticalAlignment == "Bottom")
            {
                subreport.Style.VerticalAlign = this.Properties.VerticalAlignment;
            }
            if (this.Properties.Padding != null)
            {
                subreport.Style.PaddingLeft = new RDL.DOM.Size(this.Properties.Padding.PaddingLeft);
                subreport.Style.PaddingRight = new RDL.DOM.Size(this.Properties.Padding.PaddingRight);
                subreport.Style.PaddingBottom = new RDL.DOM.Size(this.Properties.Padding.PaddingBottom);
                subreport.Style.PaddingTop = new RDL.DOM.Size(this.Properties.Padding.PaddingTop);
            }

            subreport.Style.Border = new RDL.DOM.Border();
            subreport.Style.Border.Color = this.Properties.BorderColors.DefaultBorderColor;
            subreport.Style.Border.Width = this.Properties.BorderWidths.DefaultBorderWidth;
            subreport.Style.Border.Style = this.Properties.BorderStyles.DefaultBorderStyle;

            if (this.Properties.BorderWidths.LeftBorderWidth != null || this.Properties.BorderStyles.LeftBorderStyle != null
                || this.Properties.BorderColors.LeftBorderColor != null)
            {
                subreport.Style.LeftBorder = new RDL.DOM.LeftBorder();
                subreport.Style.LeftBorder.Color = this.Properties.BorderColors.LeftBorderColor;
                subreport.Style.LeftBorder.Width = this.Properties.BorderWidths.LeftBorderWidth;
                subreport.Style.LeftBorder.Style = this.Properties.BorderStyles.LeftBorderStyle;
            }
            if (this.Properties.BorderWidths.RightBorderWidth != null || this.Properties.BorderStyles.RightBorderStyle != null
                || this.Properties.BorderColors.RightBorderColor != null)
            {
                subreport.Style.RightBorder = new RDL.DOM.RightBorder();
                subreport.Style.RightBorder.Color = this.Properties.BorderColors.RightBorderColor;
                subreport.Style.RightBorder.Width = this.Properties.BorderWidths.RightBorderWidth;
                subreport.Style.RightBorder.Style = this.Properties.BorderStyles.RightBorderStyle;
            }
            if (this.Properties.BorderWidths.TopBorderWidth != null || this.Properties.BorderStyles.TopBorderStyle != null
                || this.Properties.BorderColors.TopBorderColor != null)
            {
                subreport.Style.TopBorder = new RDL.DOM.TopBorder();
                subreport.Style.TopBorder.Color = this.Properties.BorderColors.TopBorderColor;
                subreport.Style.TopBorder.Width = this.Properties.BorderWidths.TopBorderWidth;
                subreport.Style.TopBorder.Style = this.Properties.BorderStyles.TopBorderStyle;
            }
            if (this.Properties.BorderWidths.BottomBorderWidth != null || this.Properties.BorderStyles.BottomBorderStyle != null
                || this.Properties.BorderColors.BottomBorderColor != null)
            {
                subreport.Style.BottomBorder = new RDL.DOM.BottomBorder();
                subreport.Style.BottomBorder.Color = this.Properties.BorderColors.BottomBorderColor;
                subreport.Style.BottomBorder.Width = this.Properties.BorderWidths.BottomBorderWidth;
                subreport.Style.BottomBorder.Style = this.Properties.BorderStyles.BottomBorderStyle;
            }
            if (!string.IsNullOrEmpty(this.Properties.DataElementName))
            {
                subreport.DataElementName = this.Properties.DataElementName;
            }
            if (this.Properties.DataElementOutput != "Auto")
            {
                subreport.DataElementOutput = (RDL.DOM.DataElementOutputs)Enum.Parse(typeof(RDL.DOM.DataElementOutputs), this.Properties.DataElementOutput);
            }

            return subreport;
        }

        public void RestoreReportItem(RDL.DOM.ReportItem reportItem)
        {
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


        protected override System.Windows.Size MeasureOverride(System.Windows.Size constraint)
        {
            if (this.label != null)
            {
                if (!double.IsInfinity(constraint.Width))
                    this.label.Width = constraint.Width;
                if (!double.IsInfinity(constraint.Height))
                    this.label.Height = constraint.Height;
            }
            return base.MeasureOverride(constraint);
        }

        # region Events

        void SubReportProperties_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ControlProperties controlProperties = new ControlProperties(this);
                this.Panel.UpdateOwnerWindow(controlProperties);

                controlProperties.SubReportGeneral.txt_GeneralName.Text = this.Properties.Name;
                controlProperties.SubReportGeneral.txt_ReportName.Text = this.Properties.ReportName;

                if (!string.IsNullOrEmpty(this.Properties.BorderStyles.DefaultBorderStyle))
                {
                    controlProperties.SubReportBorder.cmb_BorderStyle.TextValue = this.Properties.BorderStyles.DefaultBorderStyle;
                }
                else
                {
                    controlProperties.SubReportBorder.cmb_BorderStyle.TextValue = "None";
                }
                if (!string.IsNullOrEmpty(this.Properties.BorderColors.DefaultBorderColor))
                {
                    controlProperties.SubReportBorder.cpkr_BorderColor.Text = this.Properties.BorderColors.DefaultBorderColor;
                }
                else
                {
                    controlProperties.SubReportBorder.cpkr_BorderColor.Text = "Black";
                }
                if (!string.IsNullOrEmpty(this.Properties.BorderWidths.DefaultBorderWidth))
                {
                    controlProperties.SubReportBorder.updwn_BorderWidth.TextValue = this.Properties.BorderWidths.DefaultBorderWidth;
                }
                else
                {
                    controlProperties.SubReportBorder.updwn_BorderWidth.TextValue = "1pt";
                }

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

                    this.Properties.Name = controlProperties.SubReportGeneral.txt_GeneralName.Text;
                    this.Properties.BorderStyles.DefaultBorderStyle = controlProperties.SubReportBorder.cmb_BorderStyle.TextValue;
                    this.Properties.BorderColors.DefaultBorderColor = controlProperties.SubReportBorder.cpkr_BorderColor.Text;
                    this.Properties.BorderWidths.DefaultBorderWidth = controlProperties.SubReportBorder.updwn_BorderWidth.TextValue;

                    if (!string.IsNullOrEmpty(controlProperties.SubReportGeneral.txt_ReportName.Text))
                    {
                        this.Properties.ReportName = controlProperties.SubReportGeneral.txt_ReportName.Text;
                    }
                    
                    change.NewValue = this.GetReportItem();
                    this.Properties.IsInternalPropertyChange = false;
                }
            }

            catch
            {
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxError"));
            }
        }

        void SubReportDelete_Click(object sender, RoutedEventArgs e)
        {
            this.Panel.DeleteSelectedReportItems();
        }

        # endregion

        # region Common Helper Methods

        private void UpdateOldSubReport(Syncfusion.RDL.DOM.SubReport subReportObj)
        {
            if (subReportObj != null && subReportObj.ReportName != string.Empty)
            {
                this.label.Content = subReportObj.ReportName;
            }
            else
            {
                this.label.Content = "<" + "Subreport" + ">";
            }
        }

        private Syncfusion.RDL.DOM.SubReport UpdateSubReportBase()
        {
            Syncfusion.RDL.DOM.SubReport SubReport = new Syncfusion.RDL.DOM.SubReport();
            SubReport.Style = new Syncfusion.RDL.DOM.Style();
            SubReport.Style.Border = new Syncfusion.RDL.DOM.Border();
            SubReport.Style.Border.Style = RDL.DOM.BorderStyles.Solid.ToString();
            SubReport.Style.Border.Width = this.label.BorderThickness.Top.ToString() + "pt";
            SubReport.Style.Border.Color = this.label.BorderBrush.ToString();
            SubReport.Name = "SubReport1";
            return SubReport;
        }

        # endregion

        # region Custom Event Method

        internal void DeleteSubReportRaised()
        {
            if (this.DeleteSubReport != null)
            {
                this.Panel.DeleteSelectedReportItems();
            }
        }

        # endregion

        # region Custom Event Class

        internal delegate void DeleteSubReportClickEventHandler(object sender, DeleteSubReportClickEventArgs e);

        internal class DeleteSubReportClickEventArgs : System.EventArgs
        {
            public DeleteSubReportClickEventArgs()
            {
            }
        }

        # endregion
    }
}
