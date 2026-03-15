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
using Syncfusion.RDL.DOM;
using Syncfusion.Windows.Reports.Designer.Dialogs;
using System.ComponentModel;
using Syncfusion.Windows.ReportDesigner.Resources;
using System.Globalization;

namespace Syncfusion.Windows.Reports.Designer.Controls
{

    internal class LineControl : Canvas, IReportItemControl, INotifyPropertyChanged
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

        private bool isFocusedItem;

        private bool isItemSelected;

        private object propertyOldValue = null;

        internal Syncfusion.Windows.Reports.Designer.Editors.LineProperties Properties;
        
        internal ReportingConvertorUtil propertyValueConvertor;

        #region Construtors

        public LineControl():base()
        {
            Canvas.SetTop(this, 0);
            Canvas.SetLeft(this, 0);

            this.propertyValueConvertor = new ReportingConvertorUtil();
            
            this.InnerLine = new System.Windows.Shapes.Line();
            this.InnerLine.Stroke = Brushes.Black;
            this.InnerLine.StrokeThickness = 1;
            this.Children.Add(InnerLine);

            this.Properties = new Editors.LineProperties();
            this.Properties.PropertyChanged += new PropertyChangedEventHandler(Properties_PropertyChanged);
            this.Properties.PropertyChanging += new PropertyChangingEventHandler(Properties_PropertyChanging);

            this.Properties.IsInternalPropertyChange = true;
            this.Properties.Name = this.ItemName;
            this.Properties.UpdatePropertyValue();
            this.ContextMenu = new ContextMenu();
            this.LineProperties = new MenuItem
            {
                Header = SR.GetString(CultureInfo.CurrentUICulture, "headerLineProperties")
            };
            this.ContextMenu.Items.Add(this.LineProperties);
            this.LineProperties.Click+=new RoutedEventHandler(LineProperties_Click);            

            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            this.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;           

            this.Loaded += new RoutedEventHandler(LineControl_Loaded);
        }



        void LineControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= new RoutedEventHandler(LineControl_Loaded);

            if (this.ReportItem != null)
            {
                this.PopulateReportItem();
            }
            this.Properties.IsInternalPropertyChange = false;
        }

        #endregion

        #region public properties

        public System.Windows.Shapes.Line InnerLine
        {
            get;
            set;
        }

        public double point
        {
            get;
            set;
        }

        public MenuItem LineProperties
        {
            get;
            set;
        }

        #endregion

        #region ReportItemControl Interface

        public ReportItem ReportItem
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
                return this.InnerLine.Y2 - this.InnerLine.Y1;
            }
            set
            {
                if (this.InnerLine.Y2 != this.ItemTop + value)
                {
                    this.Properties.IsInternalPropertyChange = true;
                    this.Properties.Vertical = propertyValueConvertor.GetSizeValue(this.ItemTop + value, this.Properties.Vertical);
                    this.Properties.IsInternalPropertyChange = false;
                }
                this.InnerLine.Y2 = this.ItemTop + value;
            }
        }

        public double ItemWidth
        {
            get
            {
                return this.InnerLine.X2 - this.InnerLine.X1;
            }
            set
            {
                if (this.InnerLine.X2 != this.ItemLeft + value )
                {
                    this.Properties.IsInternalPropertyChange = true;
                    this.Properties.Horizontal = propertyValueConvertor.GetSizeValue(this.ItemLeft + value, this.Properties.Horizontal);
                    this.Properties.IsInternalPropertyChange = false;
                }
                this.InnerLine.X2 = this.ItemLeft + value;
            }
        }

        public double ItemTop
        {
            get
            {
                return this.InnerLine.Y1;
            }
            set
            {
                if (this.InnerLine.Y1 != value)
                {
                    this.Properties.IsInternalPropertyChange = true;
                    this.Properties.Top = propertyValueConvertor.GetSizeValue(value, this.Properties.Top);
                    this.Properties.IsInternalPropertyChange = false;
                }
                this.InnerLine.Y1 = value;
            }
        }

        public double ItemLeft
        {
            get
            {
                return this.InnerLine.X1;
            }
            set
            {
                if (this.InnerLine.X1 != value)
                {
                    this.Properties.IsInternalPropertyChange = true;
                    this.Properties.Left = propertyValueConvertor.GetSizeValue(value, this.Properties.Left);
                    this.Properties.IsInternalPropertyChange = false;
                }
                this.InnerLine.X1 = value;
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
                return DrawingReportItem.Line;
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
            double height = this.ItemHeight != 0 ? this.ItemHeight : this.InnerLine.StrokeThickness;
            double width = this.ItemWidth != 0 ? this.ItemWidth : this.InnerLine.StrokeThickness;
            double left = this.ItemLeft;
            double top = this.ItemTop;

            if (height < 0)
            {
                top = this.ItemTop + this.ItemHeight;
                height = Math.Abs(top - this.ItemTop);
            }
            if ( width < 0)
            {
                left = this.ItemLeft + this.ItemWidth;
                width = Math.Abs(left - this.ItemLeft);
            }

            height = height < 1 ? 1 : height;
            width = width < 1 ? 1 : width;

            this.InnerLine.Arrange(new Rect(0, 0, this.InnerLine.ActualWidth, this.InnerLine.ActualHeight));
            this.InnerLine.UpdateLayout();
            System.Windows.Media.Imaging.RenderTargetBitmap rtb = new System.Windows.Media.Imaging.RenderTargetBitmap((int)width, (int)height, 96, 96, PixelFormats.Default);
            DrawingVisual dv = new DrawingVisual();

            using (DrawingContext ctx = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(this);
                ctx.DrawRectangle(vb, null, new Rect(new Point(), new System.Windows.Size(width, height)));
            }

            rtb.Render(dv);
            return rtb;
        }

        public ReportItem GetReportItem()
        {
            Syncfusion.RDL.DOM.Line line = new Syncfusion.RDL.DOM.Line();
            line.Name = this.Properties.Name;
            line.Visibility = new RDL.DOM.Visibility();

            if (this.Properties.ToggleItem != null)
            {
                line.Visibility.ToggleItem = this.Properties.ToggleItem;
            }
            if (this.Properties.Hidden == "True" || this.Properties.Hidden.StartsWith("="))
            {
                line.Visibility.Hidden = this.Properties.Hidden;
            }

            line.Height = new RDL.DOM.Size(this.ItemHeight / 96 + DesignPanel.GetMeasuredUnit(this.Properties.Height));
            line.Width = new RDL.DOM.Size(this.ItemWidth / 96 + DesignPanel.GetMeasuredUnit(this.Properties.Width));
            line.Left = new RDL.DOM.Size(this.ItemLeft / 96 + DesignPanel.GetMeasuredUnit(this.Properties.Left));
            line.Top = new RDL.DOM.Size(this.ItemTop / 96 + DesignPanel.GetMeasuredUnit(this.Properties.Top));
            line.Style = new RDL.DOM.Style();
            line.Style.Border = new RDL.DOM.Border();

            if (!string.IsNullOrEmpty(this.Properties.DocumentMapLabel))
            {
                line.DocumentMapLabel = this.Properties.DocumentMapLabel;
            }

            line.Style.Border.Color = this.Properties.LineColor;
            line.Style.Border.Width = new RDL.DOM.Size(this.Properties.LineWidth);
            line.Style.Border.Style = this.Properties.LineStyle;
 
            return line;
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
            this.Properties.Top = propertyValueConvertor.GetSizeValue(this.ItemTop, this.Properties.Top);
            this.Properties.Left = propertyValueConvertor.GetSizeValue(this.ItemLeft, this.Properties.Left);
            this.Properties.IsInternalPropertyChange = false;
        }

        #endregion

        public void PopulateReportItem()
        {
            Syncfusion.RDL.DOM.Line lineBase = this.ReportItem as RDL.DOM.Line;

            this.Properties.Name = lineBase.Name;
            this.Properties.DocumentMapLabel = lineBase.DocumentMapLabel;

            if (lineBase.Top != null)
            {
                this.Properties.Top = lineBase.Top.size;
            }
            if (lineBase.Left != null)
            {
                this.Properties.Left = lineBase.Left.size;
            }
            if (lineBase.Visibility != null)
            {
                if (lineBase.Visibility.ToggleItem != null)
                    this.Properties.ToggleItem = lineBase.Visibility.ToggleItem;
                if (lineBase.Visibility.Hidden != null)
                    this.Properties.Hidden = lineBase.Visibility.Hidden;
            }
            if (lineBase.Style.Border != null)
            {
                if (lineBase.Style.Border.Color != null)
                    this.Properties.LineColor = lineBase.Style.Border.Color;
                if (lineBase.Style.Border.Style != null)
                    this.Properties.LineStyle = lineBase.Style.Border.Style;

                if (lineBase.Style.Border.Width != null)
                {
                    this.Properties.LineWidth = lineBase.Style.Border.Width.size;
                }
            }
        }

        void Properties_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            object propertyValue = null;
            string propertyName = e.PropertyName.ToUpper();

            switch (propertyName)
            {
                case "LINECOLOR":
                    {
                        propertyValue =this.Properties.LineColor;
                        break;
                    }
                case "LINEWIDTH":
                    {
                        propertyValue = this.Properties.LineWidth;
                        break;
                    }
                case "LINESTYLE":
                    {
                        propertyValue = this.Properties.LineStyle;
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
                case "DOCUMENTMAPLABEL":
                    {
                        propertyValue = this.Properties.DocumentMapLabel;
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
                case "HORIZONTAL":
                    {
                        propertyValue = this.Properties.Horizontal;
                        break;
                    }
                case "VERTICAL":
                    {
                        propertyValue = this.Properties.Vertical;
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
                case "LINECOLOR":
                    {
                        propertyValue = this.Properties.LineColor;
                        this.InnerLine.Stroke = this.propertyValueConvertor.GetColor(this.Properties.LineColor);
                        break;
                    }
                case "LINEWIDTH":
                    {
                        propertyValue = this.Properties.LineWidth;
                        this.InnerLine.StrokeThickness = this.propertyValueConvertor.GetLineThickness(this.Properties.LineWidth);
                        break;
                    }
                case "LINESTYLE":
                    {
                        propertyValue = this.Properties.LineStyle;
                        this.InnerLine.StrokeDashArray = this.propertyValueConvertor.GetLineStyle(this.Properties.LineStyle);
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
                case "DOCUMENTMAPLABEL":
                    {
                        propertyValue = this.Properties.DocumentMapLabel;
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
                case "HORIZONTAL":
                    {
                        propertyValue = this.Properties.Horizontal;
                        if (!this.Properties.IsInternalPropertyChange && !string.IsNullOrEmpty(this.Properties.Horizontal))
                        {
                            var width = new RDL.DOM.Size(this.Properties.Horizontal);
                            string value = DesignPanel.GetConvertedValue(width.FloatValue, width.MeasurementUnit, ReportUnitType.In);
                            this.InnerLine.X2 = new RDL.DOM.Size(value).PixelValue;
                        }
                        break;
                    }
                case "VERTICAL":
                    {
                        propertyValue = this.Properties.Vertical;
                        if (!this.Properties.IsInternalPropertyChange && !string.IsNullOrEmpty(this.Properties.Vertical))
                        {
                            var height = new RDL.DOM.Size(this.Properties.Vertical);
                            string value = DesignPanel.GetConvertedValue(height.FloatValue, height.MeasurementUnit, ReportUnitType.In);
                            this.InnerLine.Y2 = new RDL.DOM.Size(value).PixelValue;
                        }
                        break;
                    }
            }

            if (!this.Properties.IsInternalPropertyChange)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemPropertyChanged;
                PropertyChanage change = new PropertyChanage();
                change.PropertyObject = this.Properties;
                change.PropertyName = e.PropertyName;
                change.OldValue = this.propertyOldValue;
                change.NewValue = propertyValue;
                action.PropertyChange = change;
                this.Panel.EditingManager.AddAction(action);
                if (propertyName == "TOP" || propertyName == "LEFT")
                {
                    this.Panel.EditingManager.IsMergeAction = true;
                    this.RaiseReportItemSizeChangedEvent();
                    this.Panel.EditingManager.IsMergeAction = false;
                }
            }
        }

        void LineProperties_Click(object sender, RoutedEventArgs e)
        {
            ControlProperties controlProperties = UpdateLineProperties();

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

                foreach (UIElement uiElement in controlProperties.grd_PlaceHolder.Children)
                {
                    if (uiElement is LineGeneral)
                    {
                        this.Properties.Name = controlProperties.LineGeneral.txt_GeneralName.Text;
                    }
                    if (uiElement is Syncfusion.Windows.Reports.Designer.Dialogs.LineStyle)
                    {
                        Syncfusion.Windows.Reports.Designer.Dialogs.LineStyle linestyle = (Syncfusion.Windows.Reports.Designer.Dialogs.LineStyle)uiElement;

                        if (linestyle.LineThickness != "1")
                        {
                            this.Properties.LineWidth = linestyle.LineThickness;
                        }
                        else
                        {
                            this.Properties.LineWidth = linestyle.updwn_Custom.TextValue;
                        }

                        if (linestyle.LineColor != null)
                        {
                            this.Properties.LineColor = linestyle.LineColor;
                        }
                        else
                        {
                            this.Properties.LineColor = linestyle.cpkr_Font.Text;
                        }

                        switch (linestyle.cmb_style.TextValue)
                        {
                            case "Dashed":
                                {
                                    this.Properties.LineStyle = "Dashed";
                                    break;
                                }
                            case "Dotted":
                                {
                                    this.Properties.LineStyle = "Dotted";
                                    break;
                                }
                            default:
                                {
                                    this.Properties.LineStyle = "Solid";
                                    break;
                                }
                        }

                    }
                }

                change.NewValue = this.GetReportItem();
                this.Properties.IsInternalPropertyChange = false;
            }
        }

        private ControlProperties UpdateLineProperties()
        {
            ControlProperties controlProperties = new ControlProperties(this);
            this.Panel.UpdateOwnerWindow(controlProperties);
            controlProperties.LineGeneral.txt_GeneralName.Text = this.Properties.Name;
            controlProperties.LineStyle.LineColors.Text = this.Properties.LineColor;
            controlProperties.LineStyle.updwn_Custom.TextValue = this.Properties.LineWidth;
            switch (this.Properties.LineStyle.ToLower())
            {
                case "dashed":
                    {
                        controlProperties.LineStyle.cmb_style.TextValue="Dashed";
                        break;
                    }
                case "dotted":
                    {
                        controlProperties.LineStyle.cmb_style.TextValue = "Dotted";
                        break;
                    }
                default:
                    {
                        controlProperties.LineStyle.cmb_style.TextValue = "Solid";
                        break;
                    }
            }

            return controlProperties;
        }
    }
}
