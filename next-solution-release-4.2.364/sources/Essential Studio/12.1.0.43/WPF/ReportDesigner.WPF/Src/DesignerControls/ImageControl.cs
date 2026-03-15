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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Syncfusion.Windows.Reports.Designer.Dialogs;
using System.IO;
using Syncfusion.Windows.Tools.Controls;
using System.ComponentModel;
using System.Windows.Data;
using Syncfusion.RDL.Internal;
using Syncfusion.RDL.Data;
using Syncfusion.Windows.ReportDesigner.Resources;
using System.Globalization;

namespace Syncfusion.Windows.Reports.Designer.Controls
{
    #region Event Class Declaration

    internal class EmbeddedImagesChangedEventArgs : System.EventArgs
    {
        internal EmbeddedImagesChangedEventArgs(Syncfusion.RDL.DOM.EmbeddedImages embeddedImages)
        {
            this.EmbeddedImages = embeddedImages;
        }

        internal Syncfusion.RDL.DOM.EmbeddedImages EmbeddedImages { get; private set; }
    }

    #endregion

#if SyncfusionFramework4_0
    [DesignTimeVisible(false)]
#endif
    [TemplatePart(Name = "PART_InternalImage", Type = typeof(Image))]
    internal class ImageControl
        : Control, IReportItemControl, INotifyPropertyChanged
    {
        #region Private Variables
        
        internal Syncfusion.Windows.Reports.Designer.Editors.ImageProperties ImageProperties ;
        internal ReportingConvertorUtil propertyValueConvertor; 

        private bool isInternalResize = false;
        private bool isFocusedItem;
        private bool isItemSelected;
        private object propertyOldValue = null;
        #endregion

        #region public properties

        internal RDL.DOM.Action Action { get; set; }

        internal Image InternalImage { get; set; }

        private DesignerDashStyleBorder ImageBorder { get; set; }

        internal Syncfusion.RDL.DOM.ReportDefinition Report { get; set; }

        internal Syncfusion.RDL.DOM.EmbeddedImages EmbeddedImages
        {
            get;
            set;
        }

        internal string DatabaseImage { get; set; }
        
        internal MenuItem Properties { get; set; }

        internal MenuItem Delete { get; set; }

        public string ImageValue
        {
            get
            {
                return this.ImageProperties.ImageValue;
            }
            set
            {
                this.ImageProperties.ImageValue = value;
            }
        }

        #endregion

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
                    this.ImageProperties.IsInternalPropertyChange = true;
                    this.ImageProperties.Height = propertyValueConvertor.GetSizeValue(value, this.ImageProperties.Height);
                    this.ImageProperties.IsInternalPropertyChange = false;
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
                    this.ImageProperties.IsInternalPropertyChange = true;
                    this.ImageProperties.Width = propertyValueConvertor.GetSizeValue(value, this.ImageProperties.Width);
                    this.ImageProperties.IsInternalPropertyChange = false;
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
                    this.ImageProperties.IsInternalPropertyChange = true;
                    this.ImageProperties.Top = propertyValueConvertor.GetSizeValue(value, this.ImageProperties.Top);
                    this.ImageProperties.IsInternalPropertyChange = false;
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
                    this.ImageProperties.IsInternalPropertyChange = true;
                    this.ImageProperties.Left = propertyValueConvertor.GetSizeValue(value, this.ImageProperties.Left);
                    this.ImageProperties.IsInternalPropertyChange = false;
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
                    this.RaiseReportItemSelectedEvent(new SelectedItemEventArgs() { SelectedItem = this.ImageProperties , IsSelected = value});
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

        public new Canvas Parent
        { 
          get; 
          set; 
        }

        public DrawingReportItem ItemType
        {
            get
            {
                return DrawingReportItem.Image;
            }
        }

        public event ReportItemControlSizeHandler ReportItemSizeChanged;
        

        public void RaiseReportItemSizeChangedEvent()
        {
            this.UpdateSizing();

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
            Syncfusion.RDL.DOM.Image image = new Syncfusion.RDL.DOM.Image();

            if (this.ToolTip != null)
            {
                image.ToolTip = this.ImageProperties.ToolTip;
            }
            if (!string.IsNullOrEmpty(this.ImageProperties.DocumentMapLabel))
            {
                image.DocumentMapLabel = this.ImageProperties.DocumentMapLabel;
            }

            image.Name = this.ImageProperties.Name;
            image.Visibility = new RDL.DOM.Visibility();

            if (this.ImageProperties.ToggleItem != null)
            {
                image.Visibility.ToggleItem = this.ImageProperties.ToggleItem;
            }
            if (!string.IsNullOrEmpty(this.ImageProperties.Hidden) && (this.ImageProperties.Hidden == "True" || this.ImageProperties.Hidden.StartsWith("=")))
            {
                image.Visibility.Hidden = this.ImageProperties.Hidden;
            }

            image.Height = new RDL.DOM.Size(this.ItemHeight / 96 + DesignPanel.GetMeasuredUnit(this.ImageProperties.Height));
            image.Width = new RDL.DOM.Size(this.ItemWidth / 96 + DesignPanel.GetMeasuredUnit(this.ImageProperties.Width));

            image.Left = new RDL.DOM.Size(this.ItemLeft / 96 + DesignPanel.GetMeasuredUnit(this.ImageProperties.Left));
            image.Top = new RDL.DOM.Size(this.ItemTop / 96 + DesignPanel.GetMeasuredUnit(this.ImageProperties.Top));
            
            image.Source = this.ImageProperties.Source;
            image.Value = this.ImageProperties.ImageValue;

            if (image.Value == null)
            {
                image.Value = string.Empty;
            }

            image.MIMEType = this.ImageProperties.MIMEType;
            image.Sizing = this.ImageProperties.Sizing;

            image.Style = new Syncfusion.RDL.DOM.Style();

            if (this.ImageProperties.Padding != null)
            {
                image.Style.PaddingLeft = new RDL.DOM.Size(this.ImageProperties.Padding.PaddingLeft);
                image.Style.PaddingRight = new RDL.DOM.Size(this.ImageProperties.Padding.PaddingRight);
                image.Style.PaddingBottom =new RDL.DOM.Size( this.ImageProperties.Padding.PaddingBottom);
                image.Style.PaddingTop = new RDL.DOM.Size(this.ImageProperties.Padding.PaddingTop);
            }

            image.Style.Border = new RDL.DOM.Border();
            image.Style.Border.Color = this.ImageProperties.BorderColors.DefaultBorderColor;
            image.Style.Border.Width = this.ImageProperties.BorderWidths.DefaultBorderWidth;
            image.Style.Border.Style = this.ImageProperties.BorderStyles.DefaultBorderStyle;

            if (this.ImageProperties.BorderWidths.LeftBorderWidth != null || this.ImageProperties.BorderStyles.LeftBorderStyle != null
                || this.ImageProperties.BorderColors.LeftBorderColor != null)
            {
                image.Style.LeftBorder = new RDL.DOM.LeftBorder();
                image.Style.LeftBorder.Color = this.ImageProperties.BorderColors.LeftBorderColor;
                image.Style.LeftBorder.Width = this.ImageProperties.BorderWidths.LeftBorderWidth;
                image.Style.LeftBorder.Style = this.ImageProperties.BorderStyles.LeftBorderStyle;
            }
            if (this.ImageProperties.BorderWidths.RightBorderWidth != null || this.ImageProperties.BorderStyles.RightBorderStyle != null
                || this.ImageProperties.BorderColors.RightBorderColor != null)
            {
                image.Style.RightBorder = new RDL.DOM.RightBorder();
                image.Style.RightBorder.Color = this.ImageProperties.BorderColors.RightBorderColor;
                image.Style.RightBorder.Width = this.ImageProperties.BorderWidths.RightBorderWidth;
                image.Style.RightBorder.Style = this.ImageProperties.BorderStyles.RightBorderStyle;
            }
            if (this.ImageProperties.BorderWidths.TopBorderWidth != null || this.ImageProperties.BorderStyles.TopBorderStyle != null
                || this.ImageProperties.BorderColors.TopBorderColor != null)
            {
                image.Style.TopBorder = new RDL.DOM.TopBorder();
                image.Style.TopBorder.Color = this.ImageProperties.BorderColors.TopBorderColor;
                image.Style.TopBorder.Width = this.ImageProperties.BorderWidths.TopBorderWidth;
                image.Style.TopBorder.Style = this.ImageProperties.BorderStyles.TopBorderStyle;
            }
            if (this.ImageProperties.BorderWidths.BottomBorderWidth != null || this.ImageProperties.BorderStyles.BottomBorderStyle != null
                || this.ImageProperties.BorderColors.BottomBorderColor != null)
            {
                image.Style.BottomBorder = new RDL.DOM.BottomBorder();
                image.Style.BottomBorder.Color = this.ImageProperties.BorderColors.BottomBorderColor;
                image.Style.BottomBorder.Width = this.ImageProperties.BorderWidths.BottomBorderWidth;
                image.Style.BottomBorder.Style = this.ImageProperties.BorderStyles.BottomBorderStyle;
            }
            if (this.Action != null && (this.Action.BookmarkLink != null || this.Action.Drillthrough != null || this.Action.Hyperlink != null))
            {
                image.ActionInfo = new RDL.DOM.ActionInfo();
                image.ActionInfo.Actions = new RDL.DOM.Actions();
                image.ActionInfo.Actions.Clear();
                image.ActionInfo.Actions.Add(this.Action);
            }

            return image;
        }

        public void RestoreReportItem(RDL.DOM.ReportItem reportItem)
        {
            this.ImageProperties.IsInternalPropertyChange = true;
            this.ReportItem = reportItem;

            if (this.ReportItem != null)
            {
                this.PopulateReportItem();
                this.ReportItem = null;
                this.ItemHeight = reportItem.Height.PixelValue;
                this.ItemWidth = reportItem.Width.PixelValue;
                this.UpdateImageSource();
            }

            this.ImageProperties.IsInternalPropertyChange = false;
        }

        public void UpdateItemSizeProperties()
        {
            this.ImageProperties.IsInternalPropertyChange = true;
            this.ImageProperties.Width = propertyValueConvertor.GetSizeValue(this.ItemWidth, this.ImageProperties.Width);
            this.ImageProperties.Top = propertyValueConvertor.GetSizeValue(this.ItemTop, this.ImageProperties.Top);
            this.ImageProperties.Left = propertyValueConvertor.GetSizeValue(this.ItemLeft, this.ImageProperties.Left);
            this.ImageProperties.Height = propertyValueConvertor.GetSizeValue(this.ItemHeight, this.ImageProperties.Height);
            this.ImageProperties.IsInternalPropertyChange = false;
        }

        public void PopulateReportItem()
        {
            Syncfusion.RDL.DOM.Image image = this.ReportItem as RDL.DOM.Image;

            ReportModel Model = new ReportModel();

            System.Windows.Controls.Image systemImage = new Image();
            this.ImageProperties.Name = image.Name;
            this.ImageProperties.ToolTip = image.ToolTip;
            this.ImageProperties.DocumentMapLabel = image.DocumentMapLabel;

            if (image.Height != null)
            {
                this.ImageProperties.Height = image.Height.size;
            }
            if (image.Width != null)
            {
                this.ImageProperties.Width = image.Width.size;
            }
            if (image.Top != null)
            {
                this.ImageProperties.Top = image.Top.size;
            }
            if (image.Left != null)
            {
                this.ImageProperties.Left = image.Left.size;
            }
            if (image.Visibility != null)
            {
                if (image.Visibility.ToggleItem != null)
                this.ImageProperties.ToggleItem = image.Visibility.ToggleItem;
                if (image.Visibility.Hidden != null)
                    this.ImageProperties.Hidden = image.Visibility.Hidden;
            }
            if (image.ActionInfo != null)
            {
                this.Action = image.ActionInfo.Actions.FirstOrDefault();
            }
            if (image.Style != null)
            {
                string left = "0pt";
                string top = "0pt";
                string right = "0pt";
                string bottom = "0pt";

                if (image.Style.PaddingLeft != null)
                {
                    left= image.Style.PaddingLeft.size;
                }
                if (image.Style.PaddingRight != null)
                {
                    right= image.Style.PaddingRight.size;
                }
                if (image.Style.PaddingTop != null)
                {
                    top= image.Style.PaddingTop.size;
                }
                if (image.Style.PaddingBottom != null)
                {
                    bottom = image.Style.PaddingBottom.size;
                }

                this.ImageProperties.Padding.PaddingLeft=left;
                this.ImageProperties.Padding.PaddingRight = right;
                this.ImageProperties.Padding.PaddingTop = top;
                this.ImageProperties.Padding.PaddingBottom = bottom;

                if (image.Style.Border != null)
                {
                    if (image.Style.Border.Style != null)
                        this.ImageProperties.BorderStyles.DefaultBorderStyle = image.Style.Border.Style;
                    if (image.Style.Border.Color != null)
                        this.ImageProperties.BorderColors.DefaultBorderColor = image.Style.Border.Color;
                    if (image.Style.Border.Width != null)
                        this.ImageProperties.BorderWidths.DefaultBorderWidth = image.Style.Border.Width.size;

                }
                if (image.Style.LeftBorder != null)
                {
                    if (image.Style.LeftBorder.Color != null)
                        this.ImageProperties.BorderColors.LeftBorderColor = image.Style.LeftBorder.Color;
                    if (image.Style.LeftBorder.Width != null)
                        this.ImageProperties.BorderWidths.LeftBorderWidth = image.Style.LeftBorder.Width.size;
                    if (image.Style.LeftBorder.Style != null)
                        this.ImageProperties.BorderStyles.LeftBorderStyle = image.Style.LeftBorder.Style;
                }
                if (image.Style.RightBorder != null)
                {
                    if (image.Style.RightBorder.Color != null)
                        this.ImageProperties.BorderColors.RightBorderColor = image.Style.RightBorder.Color;
                    if (image.Style.RightBorder.Width != null)
                        this.ImageProperties.BorderWidths.RightBorderWidth = image.Style.RightBorder.Width.size;
                    if (image.Style.RightBorder.Style != null)
                        this.ImageProperties.BorderStyles.RightBorderStyle = image.Style.RightBorder.Style;
                }
                if (image.Style.TopBorder != null)
                {
                    if (image.Style.TopBorder.Color != null)
                        this.ImageProperties.BorderColors.TopBorderColor = image.Style.TopBorder.Color;
                    if (image.Style.TopBorder.Width != null)
                        this.ImageProperties.BorderWidths.TopBorderWidth = image.Style.TopBorder.Width.size;
                    if (image.Style.TopBorder.Style != null)
                        this.ImageProperties.BorderStyles.TopBorderStyle = image.Style.TopBorder.Style;
                }
                if (image.Style.BottomBorder != null)
                {
                    if (image.Style.BottomBorder.Color != null)
                        this.ImageProperties.BorderColors.BottomBorderColor = image.Style.BottomBorder.Color;
                    if (image.Style.BottomBorder.Width != null)
                        this.ImageProperties.BorderWidths.BottomBorderWidth = image.Style.BottomBorder.Width.size;
                    if (image.Style.BottomBorder.Style != null)
                        this.ImageProperties.BorderStyles.BottomBorderStyle = image.Style.BottomBorder.Style;
                }
            }
            
            this.ImageProperties.Source = image.Source;
            this.ImageProperties.ImageValue = image.Value;
            this.ImageProperties.MIMEType = image.MIMEType;
            this.ImageProperties.Sizing = image.Sizing;
        }

        #endregion

        internal ImageControl(Syncfusion.RDL.DOM.EmbeddedImages embeddedImages)
        {
            this.AllowDrop = false;
            this.ImageProperties = new Editors.ImageProperties();
            this.ImageProperties.PropertyChanged += new PropertyChangedEventHandler(ImageProperties_PropertyChanged);
            this.ImageProperties.PropertyChanging += new PropertyChangingEventHandler(ImageProperties_PropertyChanging);
            this.propertyValueConvertor = new ReportingConvertorUtil();
            this.EmbeddedImages = embeddedImages;
        }


        static ImageControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageControl), new FrameworkPropertyMetadata(typeof(ImageControl)));
        }

        public override void OnApplyTemplate()
        {
            ImageBorder = GetTemplateChild("PART_ImageBorder") as DesignerDashStyleBorder;
            InternalImage = GetTemplateChild("PART_InternalImage") as Image;
            this.Delete = GetTemplateChild("PART_DataMenuDelete") as MenuItem;
            this.Delete.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerDelete");
            this.Properties = GetTemplateChild("PART_DataMenuProperty") as MenuItem;
            this.Properties.Header = SR.GetString(CultureInfo.CurrentUICulture, "headerImageProperties");
            this.Delete.Click += new RoutedEventHandler(DeleteImage_Click);
            this.Properties.Click += new RoutedEventHandler(Properties_Click);
            this.AllowDrop = true;
            this.PreviewDrop += new DragEventHandler(InternalImage_PreviewDrop);
            this.PreviewDragOver += new DragEventHandler(InternalImage_PreviewDragOver);
            this.PreviewDragEnter += new DragEventHandler(InternalImage_PreviewDragEnter);
            this.InternalImage.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            this.InternalImage.HorizontalAlignment = HorizontalAlignment.Left;            
            this.InternalImage.Stretch = System.Windows.Media.Stretch.None;
            this.SetDefaultImage();
            
            base.OnApplyTemplate();
            this.ImageProperties.IsInternalPropertyChange = true;
            this.ImageProperties.Name = this.ItemName;
            this.ImageProperties.UpdatePropertyValue();

            if (this.ReportItem != null)
            {
                this.PopulateReportItem();
            }
            this.ImageProperties.IsInternalPropertyChange = false;

        }

        void DeleteImage_Click(object sender, RoutedEventArgs e)
        {
            if (this.Delete != null)
            {
                this.Panel.DeleteSelectedReportItems();
            }
        }

        private void InternalImage_PreviewDragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.All;
        }

        private void InternalImage_PreviewDragOver(object sender, DragEventArgs e)
        {
            
        }

        private void InternalImage_PreviewDrop(object sender, DragEventArgs e)
        {
            bool success = false;

            TreeObjectCollection itemCollection = e.Data.GetData(typeof(TreeObjectCollection)) as TreeObjectCollection;

            if (itemCollection != null && itemCollection.Count > 0)
            {
                TreeViewItemAdv treeViewItem = itemCollection[0] as TreeViewItemAdv;

                foreach (var dataset in this.Panel.DataSets)
                {
                    var datasbaseImage = from field in dataset.Fields where field.Name == treeViewItem.Header.ToString() select field.Name;
                    if (datasbaseImage.Count() != 0)
                    {
                        success = true;
                        DatabaseImage = "=First(Fields!" + treeViewItem.Header.ToString() + ".Value,\"" + treeViewItem.Tag.ToString() + "\")";
                        this.ImageProperties.ImageValue =DatabaseImage.ToString();
                        e.Effects = DragDropEffects.All;
                        e.Handled = true;
                    }
                }

                var embeddedImages = from embeddedImage in this.EmbeddedImages
                                     where embeddedImage.Name == treeViewItem.Header.ToString()
                                     select embeddedImage;
                if (embeddedImages.Count() > 0)
                {
                    success = true;
                    this.ImageProperties.ImageValue = treeViewItem.Header.ToString();
                    e.Effects = DragDropEffects.All;
                    e.Handled = true;
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                    e.Handled = true;
                }
            }
            else
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }

            bool mergeActionValue = this.Panel.EditingManager.IsMergeAction;

            if (DatabaseImage != null)
            {
                if (success)
                {
                    this.Panel.EditingManager.IsMergeAction = true;
                }

                DefaultDatasetImage();
                this.ImageProperties.Source = RDL.DOM.Source.Database;
                DatabaseImage = null;
                this.Panel.EditingManager.IsMergeAction = mergeActionValue;
            }
            else
            {
                if (success)
                {
                    this.Panel.EditingManager.IsMergeAction = true;
                }

                this.ImageProperties.Sizing = RDL.DOM.Sizing.AutoSize;
                this.ImageProperties.Source = RDL.DOM.Source.Embedded;
                UpdateImageSource();
                UpdateOriginalSize();
                this.ImageProperties.Sizing = RDL.DOM.Sizing.FitProportional;

                this.Panel.EditingManager.IsMergeAction = mergeActionValue;
            }
        }

        void ImageProperties_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            object propertyValue = null;
            string propertyName = e.PropertyName.ToUpper();

            switch (propertyName)
            {
                case "NAME":
                    {
                        propertyValue = this.ImageProperties.Name;
                        break;
                    }
                case "IMAGEVALUE":
                    {
                        this.UpdateImageSource();
                        propertyValue = this.ImageProperties.ImageValue;
                        break;
                    }
                case "SIZING":
                    {
                        this.UpdateOriginalSize();
                        propertyValue = this.ImageProperties.Sizing;
                        break;
                    }
                case "SOURCE":
                    {
                        propertyValue = this.ImageProperties.Source;
                        break;
                    }
                case "MIMETYPE":
                    {
                        propertyValue = this.ImageProperties.MIMEType;
                        break;
                    }
                case "PADDINGLEFT":
                    {
                        propertyValue = this.ImageProperties.Padding.PaddingLeft;
                        break;
                    }
                case "PADDINGRIGHT":
                    {
                        propertyValue = this.ImageProperties.Padding.PaddingRight;
                        break;
                    }
                case "PADDINGTOP":
                    {
                        propertyValue = this.ImageProperties.Padding.PaddingTop;
                        break;
                    }
                case "PADDINGBOTTOM":
                    {
                        propertyValue = this.ImageProperties.Padding.PaddingBottom;
                        break;
                    }
                case "DEFAULTBORDERSTYLE":
                    {
                        propertyValue = this.ImageProperties.BorderStyles.DefaultBorderStyle;
                        break;
                    }
                case "LEFTBORDERSTYLE":
                    {
                        propertyValue = this.ImageProperties.BorderStyles.LeftBorderStyle;
                        break;
                    }
                case "RIGHTBORDERSTYLE":
                    {
                        propertyValue = this.ImageProperties.BorderStyles.RightBorderStyle;
                        break;
                    }
                case "TOPBORDERSTYLE":
                    {
                        propertyValue = this.ImageProperties.BorderStyles.TopBorderStyle;
                        break;
                    }
                case "BOTTOMBORDERSTYLE":
                    {
                        propertyValue = this.ImageProperties.BorderStyles.BottomBorderStyle;
                        break;
                    }
                case "DEFAULTBORDERWIDTH":
                    {
                        propertyValue = this.ImageProperties.BorderWidths.DefaultBorderWidth;
                        break;
                    }
                case "LEFTBORDERWIDTH":
                    {
                        propertyValue = this.ImageProperties.BorderWidths.LeftBorderWidth;
                        break;
                    }
                case "RIGHTBORDERWIDTH":
                    {
                        propertyValue = this.ImageProperties.BorderWidths.RightBorderWidth;
                        break;
                    }
                case "TOPBORDERWIDTH":
                    {
                        propertyValue = this.ImageProperties.BorderWidths.TopBorderWidth;
                        break;
                    }
                case "BOTTOMBORDERWIDTH":
                    {
                        propertyValue = this.ImageProperties.BorderWidths.BottomBorderWidth;
                        break;
                    }
                case "DEFAULTBORDERCOLOR":
                    {
                        propertyValue = this.ImageProperties.BorderColors.DefaultBorderColor;
                        break;
                    }
                case "LEFTBORDERCOLOR":
                    {
                        propertyValue = this.ImageProperties.BorderColors.LeftBorderColor;
                        break;
                    }
                case "RIGHTBORDERCOLOR":
                    {
                        propertyValue = this.ImageProperties.BorderColors.RightBorderColor;
                        break;
                    }
                case "TOPBORDERCOLOR":
                    {
                        propertyValue = this.ImageProperties.BorderColors.TopBorderColor;
                        break;
                    }
                case "BOTTOMBORDERCOLOR":
                    {
                        propertyValue = this.ImageProperties.BorderColors.BottomBorderColor;
                        break;
                    }
                case "HIDDEN":
                    {
                        propertyValue = this.ImageProperties.Hidden;
                        break;
                    }
                case "TOGGLEITEM":
                    {
                        propertyValue = this.ImageProperties.ToggleItem;
                        break;
                    }
                case "DOCUMENTMAPLABEL":
                    {
                        propertyValue = this.ImageProperties.DocumentMapLabel;
                        break;
                    }
                case "LEFT":
                    {
                        propertyValue = this.ImageProperties.Left;
                        break;
                    }
                case "TOP":
                    {
                        propertyValue = this.ImageProperties.Top;
                        break;
                    }
                case "HEIGHT":
                    {
                        propertyValue = this.ImageProperties.Height;
                        break;
                    }
                case "WIDTH":
                    {
                        propertyValue = this.ImageProperties.Width;
                        break;
                    }
            }

            this.propertyOldValue = propertyValue;

        }        

        void ImageProperties_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            object propertyValue = null;
            string propertyName = e.PropertyName.ToUpper();

            switch (propertyName)
            {
                case "NAME":
                    {
                        propertyValue = this.ImageProperties.Name;
                        this.ItemName = this.ImageProperties.Name;
                        break;
                    }
                case "IMAGEVALUE":
                    {
                        this.UpdateImageSource();
                        propertyValue = this.ImageProperties.ImageValue;
                        break;
                    }
                case "MIMETYPE":
                    {
                        propertyValue = this.ImageProperties.MIMEType;
                        break;
                    }
                case "SIZING":
                    {
                        this.UpdateOriginalSize();
                        propertyValue = this.ImageProperties.Sizing;

                        if (this.InternalImage.Stretch != Stretch.None)
                        {
                            this.InternalImage.Stretch = this.propertyValueConvertor.GetSizing(this.ImageProperties.Sizing);
                        }

                        break;
                    }
                case "SOURCE":
                    {
                        propertyValue = this.ImageProperties.Source;
                        break;
                    }
                case "PADDINGLEFT":
                    {
                        propertyValue = this.ImageProperties.Padding.PaddingLeft;
                        this.UpdatePaddingValue();
                        break;
                    }
                case "PADDINGRIGHT":
                    {
                        propertyValue = this.ImageProperties.Padding.PaddingRight;
                        this.UpdatePaddingValue();
                        break;
                    }
                case "PADDINGTOP":
                    {
                        propertyValue = this.ImageProperties.Padding.PaddingTop;
                        this.UpdatePaddingValue();
                        break;
                    }
                case "PADDINGBOTTOM":
                    {
                        propertyValue = this.ImageProperties.Padding.PaddingBottom;
                        this.UpdatePaddingValue();
                        break;
                    }
                case "DEFAULTBORDERSTYLE":
                    {
                        propertyValue = this.ImageProperties.BorderStyles.DefaultBorderStyle;
                        this.ImageBorder.DashStyle = this.propertyValueConvertor.GetBorderStyle(this.ImageProperties.BorderStyles.DefaultBorderStyle);
                        break;
                    }
                case "LEFTBORDERSTYLE":
                    {
                        propertyValue = this.ImageProperties.BorderStyles.LeftBorderStyle;
                        break;
                    }
                case "RIGHTBORDERSTYLE":
                    {
                        propertyValue = this.ImageProperties.BorderStyles.RightBorderStyle;
                        break;
                    }
                case "TOPBORDERSTYLE":
                    {
                        propertyValue = this.ImageProperties.BorderStyles.TopBorderStyle;
                        break;
                    }
                case "BOTTOMBORDERSTYLE":
                    {
                        propertyValue = this.ImageProperties.BorderStyles.BottomBorderStyle;
                        break;
                    }
                case "DEFAULTBORDERWIDTH":
                    {
                        propertyValue = this.ImageProperties.BorderWidths.DefaultBorderWidth;
                        this.ImageBorder.BorderThickness = this.propertyValueConvertor.GetBorderThickness(this.ImageProperties.BorderWidths.DefaultBorderWidth);
                        break;
                    }
                case "LEFTBORDERWIDTH":
                    {
                        propertyValue = this.ImageProperties.BorderWidths.LeftBorderWidth;
                        break;
                    }
                case "RIGHTBORDERWIDTH":
                    {
                        propertyValue = this.ImageProperties.BorderWidths.RightBorderWidth;
                        break;
                    }
                case "TOPBORDERWIDTH":
                    {
                        propertyValue = this.ImageProperties.BorderWidths.TopBorderWidth;
                        break;
                    }
                case "BOTTOMBORDERWIDTH":
                    {
                        propertyValue = this.ImageProperties.BorderWidths.BottomBorderWidth;
                        break;
                    }
                case "DEFAULTBORDERCOLOR":
                    {
                        propertyValue = this.ImageProperties.BorderColors.DefaultBorderColor;
                        this.ImageBorder.BorderBrush = this.propertyValueConvertor.GetColor(this.ImageProperties.BorderColors.DefaultBorderColor);                     
                        break;
                    }
                case "LEFTBORDERCOLOR":
                    {
                        propertyValue = this.ImageProperties.BorderColors.LeftBorderColor;
                        break;
                    }
                case "RIGHTBORDERCOLOR":
                    {
                        propertyValue = this.ImageProperties.BorderColors.RightBorderColor;
                        break;
                    }
                case "TOPBORDERCOLOR":
                    {
                        propertyValue = this.ImageProperties.BorderColors.TopBorderColor;
                        break;
                    }
                case "BOTTOMBORDERCOLOR":
                    {
                        propertyValue = this.ImageProperties.BorderColors.BottomBorderColor;
                        break;
                    }
                case "HIDDEN":
                    {
                        propertyValue = this.ImageProperties.Hidden;
                        break;
                    }
                case "TOGGLEITEM":
                    {
                        propertyValue = this.ImageProperties.ToggleItem;
                        break;
                    }
                case "DOCUMENTMAPLABEL":
                    {
                        propertyValue = this.ImageProperties.DocumentMapLabel;
                        break;
                    }
                case "LEFT":
                    {
                        propertyValue = this.ImageProperties.Left;
                        if (!this.ImageProperties.IsInternalPropertyChange && !string.IsNullOrEmpty(this.ImageProperties.Left))
                            this.ItemLeft = new RDL.DOM.Size(this.ImageProperties.Left).PixelValue;
                        break;
                    }
                case "TOP":
                    {
                        propertyValue = this.ImageProperties.Top;
                        if (!this.ImageProperties.IsInternalPropertyChange && !string.IsNullOrEmpty(this.ImageProperties.Top))
                            this.ItemTop = new RDL.DOM.Size(this.ImageProperties.Top).PixelValue;
                        break;
                    }
                case "HEIGHT":
                    {
                        propertyValue = this.ImageProperties.Height;
                        if (!this.ImageProperties.IsInternalPropertyChange && !string.IsNullOrEmpty(this.ImageProperties.Height))
                            this.ItemHeight = new RDL.DOM.Size(this.ImageProperties.Height).PixelValue;
                        break;
                    }
                case "WIDTH":
                    {
                        propertyValue = this.ImageProperties.Width;
                        if (!this.ImageProperties.IsInternalPropertyChange && !string.IsNullOrEmpty(this.ImageProperties.Width))
                            this.ItemWidth = new RDL.DOM.Size(this.ImageProperties.Width).PixelValue;
                        break;
                    }
            }
            if (!this.ImageProperties.IsInternalPropertyChange)
            {
                    EditAction action = new EditAction();
                    action.EditingType = EditActionType.ItemPropertyChanged;
                    PropertyChanage change = new PropertyChanage();
                    change.PropertyObject = this.ImageProperties;

                    switch (propertyName)
                    {
                        case "PADDINGLEFT":
                        case "PADDINGRIGHT":
                        case "PADDINGTOP":
                        case "PADDINGBOTTOM":
                            change.PropertyObject = this.ImageProperties.Padding;
                            break;

                        case "DEFAULTBORDERSTYLE":
                        case "LEFTBORDERSTYLE":
                        case "RIGHTBORDERSTYLE":
                        case "TOPBORDERSTYLE":
                        case "BOTTOMBORDERSTYLE":
                            change.PropertyObject = this.ImageProperties.BorderStyles;
                            break;

                        case "DEFAULTBORDERCOLOR":
                        case "LEFTBORDERCOLOR":
                        case "RIGHTBORDERCOLOR":
                        case "TOPBORDERCOLOR":
                        case "BOTTOMBORDERCOLOR":
                            change.PropertyObject = this.ImageProperties.BorderColors;
                            break;

                        case "DEFAULTBORDERWIDTH":
                        case "LEFTBORDERWIDTH":
                        case "RIGHTBORDERWIDTH":
                        case "TOPBORDERWIDTH":
                        case "BOTTOMBORDERWIDTH":
                            change.PropertyObject = this.ImageProperties.BorderWidths;
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

        internal void Properties_Click(object sender, RoutedEventArgs e)
        {
            ControlProperties controlProperties = UpdatePropertyDialog();          

            if (controlProperties.ShowDialog() == true)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemChanged;
                ItemChange change = new ItemChange();
                change.ReportItem = this;
                change.OldValue = this.GetReportItem();
                this.Panel.EditingManager.AddAction(action);
                this.Panel.EditingManager.IsMergeAction = true;
                this.ImageProperties.IsInternalPropertyChange = true;

                foreach (var image in controlProperties.CachedEmbeddedImages)
                {
                    this.Panel.AddEmbeddedImage(image);
                }

                this.ImageProperties.Name = controlProperties.ImageGeneral.txt_GeneralName.Text;
                this.ImageProperties.ToolTip = controlProperties.ImageGeneral.ToolTip.Text;
                this.ImageProperties.BorderWidths.DefaultBorderWidth = controlProperties.ImageBorder.ImgBrdrThick.TextValue;
                this.ImageProperties.BorderColors.DefaultBorderColor = controlProperties.ImageBorder.clrpkr_BorderColor.Text;
                this.ImageProperties.BorderStyles.DefaultBorderStyle = controlProperties.ImageBorder.updwn_borderstyle.Text;
                this.ImageProperties.Source = (RDL.DOM.Source)Enum.Parse(typeof(RDL.DOM.Source), controlProperties.ImageGeneral.ImageSource.SelectedValue.ToString());
                this.ImageProperties.ImageValue = controlProperties.ImageGeneral.ImageName.Text;
                this.ImageProperties.Padding.PaddingLeft = controlProperties.ImageSize.updwn_Left.TextValue;
                this.ImageProperties.Padding.PaddingRight = controlProperties.ImageSize.updwn_Right.TextValue;
                this.ImageProperties.Padding.PaddingTop = controlProperties.ImageSize.updwn_Top.TextValue;
                this.ImageProperties.Padding.PaddingBottom = controlProperties.ImageSize.updwn_Bottom.TextValue;

                if (controlProperties.ControlAction.rbtn_None.IsChecked == false)
                {
                    if (this.Action == null)
                    {
                        this.Action = new RDL.DOM.Action();
                    }
                    if (controlProperties.ControlAction.rbtn_goToReport.IsChecked == true)
                    {
                        if (this.Action.Drillthrough == null)
                        {
                            this.Action.Drillthrough = new RDL.DOM.Drillthrough();
                        }

                        this.Action.Drillthrough.Parameters = controlProperties.Parameters;
                        this.Action.Drillthrough.ReportName = controlProperties.ControlAction.txt_reportName.Text;
                        this.Action.Hyperlink = null;
                    }
                    else if (controlProperties.ControlAction.rbtn_goToUrl.IsChecked == true)
                    {
                        this.Action.Hyperlink = controlProperties.ControlAction.cmbx_UrlPath.Text;
                        this.Action.Drillthrough = null;
                    }
                }
                if (controlProperties.ImageSize.rbtn_Clip.IsChecked == true)
                {
                    this.ImageProperties.Sizing = RDL.DOM.Sizing.Clip;
                }
                else if (controlProperties.ImageSize.rbtn_FitToSize.IsChecked == true)
                {
                    this.ImageProperties.Sizing = RDL.DOM.Sizing.Fit;
                }
                else if (controlProperties.ImageSize.rbtn_OriginalSize.IsChecked == true)
                {
                    this.ImageProperties.Sizing = RDL.DOM.Sizing.AutoSize;
                }
                else
                {
                    this.ImageProperties.Sizing = RDL.DOM.Sizing.FitProportional;
                }

                this.UpdateOriginalSize();
                change.NewValue = this.GetReportItem();
                action.ItemChange = change;
                this.ImageProperties.IsInternalPropertyChange = false;

                if (controlProperties.CachedEmbeddedImages.Count > 0)
                {
                    this.Panel.RaiseEmbeddedImageCollectionModifiedEvent();
                }
            }
            else
            {
                foreach (var embeddedImage in controlProperties.CachedEmbeddedImages)
                {
                    this.EmbeddedImages.Remove(embeddedImage);
                }
            }

            this.Panel.EditingManager.IsMergeAction = false;

        }

        internal ControlProperties UpdatePropertyDialog()
        {
            ControlProperties controlProperties = new ControlProperties(this,this.EmbeddedImages);
            this.Panel.UpdateOwnerWindow(controlProperties);

            controlProperties.ImageGeneral.txt_GeneralName.Text = this.ItemName;

            if (this.ToolTip != null)
            {
                controlProperties.ImageGeneral.ToolTip.Text = this.ToolTip.ToString();
            }
            if (this.Action != null)
            {
                if (this.Action.Drillthrough != null)
                {
                    if (!string.IsNullOrEmpty(this.Action.Drillthrough.ReportName))
                    {
                        controlProperties.ControlAction.rbtn_goToReport.IsChecked = true;
                        controlProperties.ControlAction.txt_reportName.Text = this.Action.Drillthrough.ReportName;
                    }
                }
                else if (!string.IsNullOrEmpty(this.Action.Hyperlink) || this.Panel.DataSets != null)
                {
                    if (!string.IsNullOrEmpty(this.Action.Hyperlink))
                    {
                        controlProperties.ControlAction.cmbx_UrlPath.Items.Add(this.Action.Hyperlink);
                        controlProperties.ControlAction.cmbx_UrlPath.Text = this.Action.Hyperlink;
                        controlProperties.ControlAction.rbtn_goToUrl.IsChecked = true;
                    }
                }
            }

            controlProperties.ImageBorder.ImgBrdrThick.TextValue = this.ImageProperties.BorderWidths.DefaultBorderWidth;
            controlProperties.ImageBorder.clrpkr_BorderColor.Text = this.ImageProperties.BorderColors.DefaultBorderColor;
            controlProperties.ImageBorder.updwn_borderstyle.Text = this.ImageProperties.BorderStyles.DefaultBorderStyle;
            controlProperties.ImageGeneral.cmb_ImageSource.SelectedItem = this.ImageProperties.Source;
            controlProperties.ImageGeneral.cmb_ImageName.Text = this.ImageProperties.ImageValue;
            
            if (this.ImageProperties.Sizing == RDL.DOM.Sizing.Clip)
            {
                controlProperties.ImageSize.rbtn_Clip.IsChecked = true;
            }
            else if (this.ImageProperties.Sizing == RDL.DOM.Sizing.Fit)
            {
                controlProperties.ImageSize.rbtn_FitToSize.IsChecked = true;
            }
            else if (this.ImageProperties.Sizing == RDL.DOM.Sizing.AutoSize)
            {
                controlProperties.ImageSize.rbtn_OriginalSize.IsChecked = true;
            }
            else
            {
                controlProperties.ImageSize.rbtn_FitProportional.IsChecked = true;
            }

            controlProperties.ImageSize.updwn_Left.TextValue = this.ImageProperties.Padding.PaddingLeft;
            controlProperties.ImageSize.updwn_Right.TextValue = this.ImageProperties.Padding.PaddingRight;
            controlProperties.ImageSize.updwn_Top.TextValue = this.ImageProperties.Padding.PaddingTop;
            controlProperties.ImageSize.updwn_Bottom.TextValue = this.ImageProperties.Padding.PaddingBottom;

            return controlProperties;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (this.InternalImage != null)
            {
                if (!double.IsInfinity(constraint.Height))
                {
                    this.InternalImage.Height = constraint.Height;
                }
                if (!double.IsInfinity(constraint.Width))
                {
                    this.InternalImage.Width = constraint.Width;
                }
            }

            return base.MeasureOverride(constraint);
        }

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

        private RDL.DOM.EmbeddedImage GetEmbeddedImage(string imageValue)
        {
            if (!string.IsNullOrEmpty(imageValue))
            {
                var embeddedImages = from embeddedImage in this.EmbeddedImages
                                     where embeddedImage.Name == imageValue
                                     select embeddedImage;

                if (embeddedImages.Count() > 0)
                {
                    return embeddedImages.First();
                }
            }

            return null;
        }

        private void SetDefaultImage()
        {
            BitmapImage image = new BitmapImage(new Uri(@"pack://application:,,,/Syncfusion.ReportDesigner.WPF;component//Images/NoImage.png", UriKind.Absolute));
            this.InternalImage.Source = image;
            this.InternalImage.Stretch = Stretch.None;
        }

        private void DefaultDatasetImage()
        {
            BitmapImage image = new BitmapImage(new Uri(@"pack://application:,,,/Syncfusion.ReportDesigner.WPF;component//Images/DatasetImageSource.png", UriKind.Absolute));
            this.InternalImage.Source = image;
            this.InternalImage.Stretch = Stretch.None;
        }

        internal void UpdateImageSource()
        {
            this.SetDefaultImage();        

            if (this.ImageProperties.Source == RDL.DOM.Source.Embedded)
            {
                if (!String.IsNullOrEmpty(this.ImageProperties.ImageValue) && !this.ImageProperties.ImageValue.Trim().StartsWith("="))
                {
                    RDL.DOM.EmbeddedImage embeddedImage = this.GetEmbeddedImage(this.ImageProperties.ImageValue);

                    if (embeddedImage != null)
                    {
                        if (embeddedImage.MIMEType != "image/emf")
                        {
                            Base64ImageConverter imageConverter = new Base64ImageConverter();
                            this.InternalImage.Source = this.GetBitmapImage(embeddedImage);
                        }
                        else
                        {
                            BitmapImage bitmapImage = new BitmapImage();
                            System.Drawing.Imaging.Metafile metafile = this.GetEMFImage(embeddedImage);

                            using (Stream streamEx = new MemoryStream())
                            {
                                metafile.Save(streamEx, System.Drawing.Imaging.ImageFormat.Png);
                                streamEx.Position = 0;
                                bitmapImage.BeginInit();
                                bitmapImage.StreamSource = streamEx;
                                bitmapImage.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
                                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                                bitmapImage.EndInit();
                            }
                            this.InternalImage.Source = bitmapImage;
                        }

                        this.InternalImage.Stretch = this.propertyValueConvertor.GetSizing(this.ImageProperties.Sizing);
                    }
                }
            }
        }

        void UpdateOriginalSize()
        {
            if (this.ImageProperties.Sizing == RDL.DOM.Sizing.AutoSize && this.ImageProperties.Source == RDL.DOM.Source.Embedded)
            {
                RDL.DOM.EmbeddedImage embeddedImage = this.GetEmbeddedImage(this.ImageProperties.ImageValue);

                if (embeddedImage != null)
                {
                    EditAction imageAction = new EditAction();
                    imageAction.ResizedReportItems = new List<SizingChange>();
                    SizingChange imageChange = new SizingChange();
                    imageChange.ReportItem = this;
                    imageChange.OldWidth = this.ItemWidth;
                    imageChange.OldHeight = this.ItemHeight;

                    double height = 0;
                    double width = 0;
                    if (embeddedImage.MIMEType != "image/emf")
                    {
                        System.Windows.Media.Imaging.BitmapImage bitmapImage = this.GetBitmapImage(embeddedImage);
                        height = bitmapImage.Height;
                        width = bitmapImage.Width;
                        this.ItemHeight = bitmapImage.Height;
                        this.ItemWidth = bitmapImage.Width;
                    }
                    else
                    {
                        System.Drawing.Image image = this.GetEMFImage(embeddedImage);
                        height = image.Height;
                        width = image.Width;
                        this.ItemHeight = image.Height;
                        this.ItemWidth = image.Width;
                    }

                    imageChange.NewHeight = height;
                    imageChange.NewWidth = width;
                    imageAction.ResizedReportItems.Add(imageChange);
                    imageAction.EditingType = EditActionType.ItemResize;

                    if (!this.Panel.IsInternalChange)
                    {
                        this.Panel.EditingManager.AddAction(imageAction);
                        isInternalResize = true;
                    }

                    this.UpdateLayout();
                    this.InvalidateArrange();
                    
                    if (this.isInternalResize)
                    {
                        this.ReportItemSizeChanged(this, new EventArgs());
                    }
                    isInternalResize = false;
                }
            }
        }

        System.Windows.Media.Imaging.BitmapImage GetBitmapImage(RDL.DOM.EmbeddedImage embeddedImage)
        {
            Base64ImageConverter imageConverter = new Base64ImageConverter();
            return imageConverter.ConvertToImage(embeddedImage.ImageData);
        }

        System.Drawing.Imaging.Metafile GetEMFImage(RDL.DOM.EmbeddedImage embeddedImage)
        {
            byte[] data = Convert.FromBase64String(embeddedImage.ImageData);

            using (Stream stream = new MemoryStream(data))
            {
                System.Drawing.Imaging.Metafile image = new System.Drawing.Imaging.Metafile(stream);
                return image;
            }
        }

        void UpdateSizing()
        {
            if (this.ImageProperties.Sizing == RDL.DOM.Sizing.AutoSize)
            {
                this.ImageProperties.Sizing = RDL.DOM.Sizing.FitProportional;
            }
        }

        private void UpdatePaddingValue()
        {
            double left = new RDL.DOM.Size(this.ImageProperties.Padding.PaddingLeft).PixelValue;
            double right = new RDL.DOM.Size(this.ImageProperties.Padding.PaddingLeft).PixelValue;
            double top = new RDL.DOM.Size(this.ImageProperties.Padding.PaddingLeft).PixelValue;
            double bottom = new RDL.DOM.Size(this.ImageProperties.Padding.PaddingLeft).PixelValue;
            this.Padding = new Thickness(left, top, right, bottom);
        }
    }
}