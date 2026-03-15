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
using Syncfusion.Windows.Reports.Common;
using System.Windows.Input;
using System.Windows.Data;
using Syncfusion.Windows.Tools.Controls;
using System.Windows.Shapes;
using Syncfusion.Windows.Reports.Designer.Dialogs;
using System.Windows.Media;
using System.Windows.Documents;
using System.IO;
using System.Collections;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using Base = Syncfusion.RDL.DOM;
using Syncfusion.RDL.DOM;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Xml.Serialization;
using Syncfusion.RDL.Internal;
using Syncfusion.Windows.ReportDesigner.Resources;
using System.Globalization;

namespace Syncfusion.Windows.Reports.Designer.Controls
{

#if SyncfusionFramework4_0
    [DesignTimeVisible(false)]
#endif
    internal class DesignPanel : Control, IReportControl, INotifyPropertyChanged, INotifyPropertyChanging
    {
        #region Variables

        private Point itemEndPoint;
        private Point itemStartPoint;
        private Point selectionStartPoint;
        private Point selectionEndPoint;
        private Point canvasStartingPoint;
        private Point canvasEndPoint;
        internal List<IReportItemControl> reportItems = new List<IReportItemControl>();
        private XmlSerializer copySerializer;
        internal ReportDesignView reportDesignView;
        private AdornerLayer aLayer;
        private System.Windows.Shapes.Rectangle selectionBox;
        private DoubleCollection dashed = new DoubleCollection { 4, 4 };
        private DoubleCollection dotted = new DoubleCollection { 1.5, 1.5 };
        private ControlDialog controldialog;
        private ReportingConvertorUtil converter;

        private object propertyValue = null;
        private object dragOldSize = null;

        private int groupID = 1;

        private double maxHeaderWidthValue = 0;
        private double maxFooterWidthValue = 0;
        private double maxBodyWidthValue = 0;

        private bool isSelectionkeypressed = false;
        private bool isSelection = false;
        private bool isTablixControlSelected = false;

        internal Syncfusion.Windows.Reports.Designer.Editors.HeaderProperties headerproperties;
        internal Syncfusion.Windows.Reports.Designer.Editors.FooterProperties footerproperties;
        internal Syncfusion.Windows.Reports.Designer.Editors.BodyProperties bodyproperties;
        internal Syncfusion.Windows.Reports.Designer.Editors.ReportProperties reportproperties;

        public Canvas drawingCanvas = null;

        public int textBoxCount = 0;
        public int tablixCount = 0;
        public int imageCount = 0;
        public int rectangleCount = 0;
        public int subreportCount = 0;
        public int lineCount = 0;
        public int chartCount = 0;
        public int gaugeCount = 0;
        public int mapCount = 0;

        #endregion

        #region Properties

        internal RDL.DOM.RDLType RDLType { get; set; }

        internal string VisualStyle
        {
            get { return (string)GetValue(VisualStyleProperty); }
            set { SetValue(VisualStyleProperty, value); }
        }

        internal static readonly DependencyProperty VisualStyleProperty =
            DependencyProperty.Register("VisualStyle", typeof(string), typeof(DesignPanel), new UIPropertyMetadata("Office2007Blue", VisualStylePropertyChanged));

        internal static void VisualStylePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            DesignPanel designPanel = dependencyObject as DesignPanel;
            designPanel.UpdateStyle();
        }

        internal EditingManager EditingManager { get; set; }

        internal bool IsInternalChange { get; set; }

        private DatabaseAccessTypes DatabaseAccess { get; set; }

        internal ReportDefinition Report { get; set; }

        internal DataSources DataSources { get; set; }

        internal DataSets DataSets { get; set; }

        internal EmbeddedImages EmbeddedImages { get; set; }

        internal ReportParameters ReportParameters { get; set; }

        internal string CurrentDataSource { get; set; }

        #region Binding Properties

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

        public event PropertyChangingEventHandler PropertyChanging;

        protected void OnPropertyChanging(string name)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(name));
            }
        }

        // Using a DependencyProperty as the backing store for IsRulerVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsRulerVisibleProperty =
            DependencyProperty.Register("IsRulerVisible", typeof(bool), typeof(DesignPanel), new UIPropertyMetadata((bool)true, OnIsRulerVisiblePropertyChanged));

        public bool IsRulerVisible
        {
            get { return (bool)GetValue(IsRulerVisibleProperty); }
            set { SetValue(IsRulerVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowHelp.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowHelpProperty =
            DependencyProperty.Register("ShowHelp", typeof(bool), typeof(DesignPanel), new UIPropertyMetadata((bool)true));

        public bool ShowHelp
        {
            get { return (bool)GetValue(ShowHelpProperty); }
            set { SetValue(ShowHelpProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsHeaderVisible.
        public static readonly DependencyProperty ZoomFactorProperty =
            DependencyProperty.Register("ZoomFactor", typeof(double), typeof(DesignPanel), new UIPropertyMetadata((double)100, OnZoomFactorPropertyChanged));

        internal static void OnZoomFactorPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            DesignPanel designPanel = dependencyObject as DesignPanel;
            if (e.OldValue != e.NewValue)
            {
                designPanel.UpdatePanelZoom();
            }
        }

        internal double ZoomFactor
        {
            get { return (double)GetValue(ZoomFactorProperty); }
            set { SetValue(ZoomFactorProperty, value); }
        }

        private bool isHeaderVisible = true;

        public bool IsHeaderVisible
        {
            get
            {
                return isHeaderVisible;
            }
            set
            {
                if (value != isHeaderVisible)
                {
                    this.OnPropertyChanging("IsHeaderVisible");
                    isHeaderVisible = value;
                    this.SetHeaderVisibility(value);
                    this.OnPropertyChanged("IsHeaderVisible");
                }
            }
        }

        private bool isFooterVisible = true;

        public bool IsFooterVisible
        {
            get
            {
                return isFooterVisible;
            }
            set
            {
                if (value != isFooterVisible)
                {
                    this.OnPropertyChanging("IsFooterVisible");
                    isFooterVisible = value;
                    this.SetFooterVisibility(value);
                    this.OnPropertyChanged("IsFooterVisible");
                }
            }
        }

        double minReportWidth = 0;

        public double MinReportWidth
        {
            get
            {
                return minReportWidth;
            }
            set
            {
                if (value != minReportWidth)
                {
                    this.OnPropertyChanging("MinReportWidth");
                    minReportWidth = value;
                    this.OnPropertyChanged("MinReportWidth");
                }
            }
        }

        double minHeaderheight = 0;

        public double MinHeaderheight
        {
            get
            {
                return minHeaderheight;
            }
            set
            {
                if (value != minHeaderheight)
                {
                    this.OnPropertyChanging("MinHeaderheight");
                    minHeaderheight = value;
                    this.OnPropertyChanged("MinHeaderheight");
                }
            }
        }

        double minFooterHeight = 0;

        public double MinFooterHeight
        {
            get
            {
                return minFooterHeight;
            }
            set
            {
                if (value != minFooterHeight)
                {
                    this.OnPropertyChanging("MinFooterHeight");
                    minFooterHeight = value;
                    this.OnPropertyChanged("MinFooterHeight");
                }
            }
        }

        double minBodyHeight = 0;

        public double MinBodyHeight
        {
            get
            {
                return minBodyHeight;
            }
            set
            {
                if (value != minBodyHeight)
                {
                    this.OnPropertyChanging("MinBodyHeight");
                    minBodyHeight = value;
                    this.OnPropertyChanged("MinBodyHeight");
                }
            }
        }

        ReportUnitType _unitType = ReportUnitType.In;
        internal ReportUnitType ReportUnitType
        {
            get
            {
                return _unitType;
            }
            set
            {
                if (_unitType != value)
                {
                    ControlProperties.UnitType = value;
                    _unitType = value;
                    if (value ==  Base.ReportUnitType.Cm)
                    {
                        this.RulerHorizontal.Unit = Unit.Cm;
                        this.RulerVerticalBody.Unit = Unit.Cm;
                        this.RulerVerticalHeader.Unit = Unit.Cm;
                        this.RulerVerticalFooter.Unit = Unit.Cm;
                    }
                    else
                    {
                        this.RulerHorizontal.Unit = Unit.Inch;
                        this.RulerVerticalBody.Unit = Unit.Inch;
                        this.RulerVerticalFooter.Unit = Unit.Inch;
                        this.RulerVerticalHeader.Unit = Unit.Inch;
                    }
                }
            }
        }

        #endregion

        private Body ReportBody
        {
            get;
            set;
        }

        public DrawingReportItem SelectedReportItemType
        {
            get;
            set;
        }

        public bool ProcessDrawItem
        {
            get;
            set;
        }

        #endregion

        #region This xaml object variables

        private System.Windows.Shapes.Rectangle bodySelectionBox;
        private System.Windows.Shapes.Rectangle headerSelectionBox;
        private System.Windows.Shapes.Rectangle footerSelectionBox;

        public RowDefinition RowDefinitionRulerHeader;
        public RowDefinition RowDefinitionRulerBody;
        public RowDefinition RowDefinitionRulerFooter;
        public RowDefinition RowDefinitionHorizontalRuler;

        public ColumnDefinition HorizontalRulerColumnDefinition;
        public ColumnDefinition ColumnDefinitionVerticalRuler;

        public Ruler RulerVerticalHeader;
        public Ruler RulerVerticalBody;
        public Ruler RulerVerticalFooter;
        public Ruler RulerHorizontal;

        public Grid DesignerArea;
        public Grid DesignerInnerArea;
        public Grid GridVertialRuler;
        public Grid GridHorizontalRuler;
        public Grid HeaderGrid;
        public Grid BodyGrid;
        public Grid FooterGrid;
        public Grid drawingGrid;
        public Grid GridDesignPanel;
        public Grid GridCorner;

        public Grid GridFooterRuler;
        public Grid GridHeaderRuler;

        public System.Windows.Controls.Separator FooterRulerSeperator;
        public System.Windows.Controls.Separator HeaderRulerSeperator;

        public GridSplitter gridSplitterBody;
        public GridSplitter gridSplitterHeader;
        public GridSplitter gridSplitterFooter;
        public GridSplitter gridSplitterReport;

        public MenuItem BodyContextMenuInsertFooter;
        public MenuItem BodyContextMenuInsertHeader;

        public MenuItem MenuItemHeaderRemoveHeader;
        public MenuItem MenuItemHeaderInsertTextbox;
        public MenuItem MenuItemHeaderInsertLine;
        public MenuItem MenuItemHeaderInsertRectangle;
        public MenuItem MenuItemHeaderInsertImage;

        public MenuItem MenuItemBodyInsertTextbox;
        public MenuItem MenuItemBodyInsertLine;
        public MenuItem MenuItemBodyInsertTable;
        public MenuItem MenuItemBodyInsertMatrix;
        public MenuItem MenuItemBodyInsertRectangle;
        public MenuItem MenuItemBodyInsertList;
        public MenuItem MenuItemBodyInsertImage;
        public MenuItem MenuItemBodyInsertSubreport;
        public MenuItem MenuItemBodyInsertChart;
        public MenuItem MenuItemBodyInsertGauge;
        public MenuItem MenuItemBodyInsertMap;

        public MenuItem MenuItemFooterRemoveFooter;
        public MenuItem MenuItemFooterInsertTextbox;
        public MenuItem MenuItemFooterInsertLine;
        public MenuItem MenuItemFooterInsertRectangle;
        public MenuItem MenuItemFooterInsertImage;
        public ScrollViewer scrollViewerReportBody;

        public Canvas bodyCanvas;
        public Canvas headerCanvas;
        public Canvas footerCanvas;

        public RowDefinition headerRowDefination;
        public RowDefinition footerRowDefination;
        public RowDefinition bodyRowDefination;

        public ColumnDefinition reportWidthColumnDefinition;

        #endregion

        #region Constructor

        public DesignPanel(ReportDefinition report, ReportDesignView designview)
        {
            converter = new ReportingConvertorUtil();
            this.SelectedReportItems = new List<IReportItemControl>();
            this.Report = report;
            this.reportDesignView = designview;
            this.EmbeddedImages = this.Report.EmbeddedImages;
            this.DataSources = this.Report.DataSources;
            this.DataSets = this.Report.DataSets;
            this.ReportParameters = this.Report.ReportParameters;

            this.copySerializer = new XmlSerializer(typeof(ReportItems));
            this.EditingManager = new EditingManager(this);

            this.headerproperties = new Editors.HeaderProperties();
            this.footerproperties = new Editors.FooterProperties();
            this.bodyproperties = new Editors.BodyProperties();
            this.reportproperties = new Editors.ReportProperties();

            this.headerproperties.Name = "Page Header";
            this.footerproperties.Name = "Page Footer";
            this.bodyproperties.Name = "Body";
            this.reportproperties.Name = "Report";

            this.reportproperties.PropertyChanging += new PropertyChangingEventHandler(reportproperties_PropertyChanging);
            this.reportproperties.PropertyChanged += new PropertyChangedEventHandler(reportproperties_PropertyChanged);

            this.bodyproperties.PropertyChanging += new PropertyChangingEventHandler(bodyproperties_PropertyChanging);
            this.bodyproperties.PropertyChanged += new PropertyChangedEventHandler(bodyproperties_PropertyChanged);

            this.headerproperties.PropertyChanging += new PropertyChangingEventHandler(headerproperties_PropertyChanging);
            this.headerproperties.PropertyChanged += new PropertyChangedEventHandler(headerproperties_PropertyChanged);

            this.footerproperties.PropertyChanging += new PropertyChangingEventHandler(footerproperties_PropertyChanging);
            this.footerproperties.PropertyChanged += new PropertyChangedEventHandler(footerproperties_PropertyChanged);

            this.PropertyChanged += new PropertyChangedEventHandler(DesignPanel_PropertyChanged);
        }

        void DesignPanel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string propertyName = e.PropertyName;

            switch (propertyName)
            {
                case "MinBodyHeight":
                    this.bodyRowDefination.MinHeight = this.minBodyHeight;
                    break;
                case "MinFooterHeight":
                    this.footerRowDefination.MinHeight = this.minFooterHeight;
                    break;
                case "MinHeaderheight":
                    this.headerRowDefination.MinHeight = this.MinHeaderheight;
                    break;
                case "MinReportWidth":
                    this.reportWidthColumnDefinition.MinWidth = this.MinReportWidth;
                    break;
                case "IsFooterVisible":
                    this.FooterGrid.Visibility = converter.GetVisiblity(this.IsFooterVisible);
                    this.gridSplitterFooter.Visibility = this.FooterGrid.Visibility;
                    this.FooterRulerSeperator.Visibility = this.FooterGrid.Visibility;
                    this.GridFooterRuler.Visibility = this.FooterGrid.Visibility;
                    break;
                case "IsHeaderVisible":
                    this.HeaderGrid.Visibility = converter.GetVisiblity(this.IsHeaderVisible);
                    this.gridSplitterHeader.Visibility = this.HeaderGrid.Visibility;
                    this.HeaderRulerSeperator.Visibility = this.HeaderGrid.Visibility;
                    this.GridHeaderRuler.Visibility = this.HeaderGrid.Visibility;
                    break;
            }
        }

        void footerproperties_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            string propertyName = e.PropertyName;

            switch (propertyName)
            {
                case "DefaultBorderStyle":
                    {
                        this.propertyValue = this.footerproperties.BorderStyles.DefaultBorderStyle;
                        break;
                    }
                case "LefttBorderStyle":
                    {
                        this.propertyValue = this.footerproperties.BorderStyles.LeftBorderStyle;
                        break;
                    }
                case "RightBorderStyle":
                    {
                        this.propertyValue = this.footerproperties.BorderStyles.RightBorderStyle;
                        break;
                    }
                case "TopBorderStyle":
                    {
                        this.propertyValue = this.footerproperties.BorderStyles.TopBorderStyle;
                        break;
                    }
                case "BottomBorderStyle":
                    {
                        this.propertyValue = this.footerproperties.BorderStyles.BottomBorderStyle;
                        break;
                    }
                case "DefaultBorderWidth":
                    {
                        this.propertyValue = this.footerproperties.BorderWidths.DefaultBorderWidth;
                        break;
                    }
                case "LeftBorderWidth":
                    {
                        this.propertyValue = this.footerproperties.BorderWidths.LeftBorderWidth;
                        break;
                    }
                case "RightBorderWidth":
                    {
                        this.propertyValue = this.footerproperties.BorderWidths.RightBorderWidth;
                        break;
                    }
                case "TopBorderWidth":
                    {
                        this.propertyValue = this.footerproperties.BorderWidths.TopBorderWidth;
                        break;
                    }
                case "BottomBorderWidth":
                    {
                        this.propertyValue = this.footerproperties.BorderWidths.BottomBorderWidth;
                        break;
                    }
                case "DefaultBorderColor":
                    {
                        this.propertyValue = this.footerproperties.BorderColors.DefaultBorderColor;
                        break;
                    }
                case "LeftBorderColor":
                    {
                        this.propertyValue = this.footerproperties.BorderColors.LeftBorderColor;
                        break;
                    }
                case "RightBorderColor":
                    {
                        this.propertyValue = this.footerproperties.BorderColors.RightBorderColor;
                        break;
                    }
                case "TopBorderColor":
                    {
                        this.propertyValue = this.footerproperties.BorderColors.TopBorderColor;
                        break;
                    }
                case "BottomBorderColor":
                    {
                        this.propertyValue = this.footerproperties.BorderColors.BottomBorderColor;
                        break;
                    }
                case "BackgroundColor":
                    this.propertyValue = this.footerproperties.BackgroundColor;
                    break;
                case "FooterHeight":
                    this.propertyValue = this.footerproperties.FooterHeight;
                    break;
                case "Source":
                    this.propertyValue = this.footerproperties.BackgroundImage.Source;
                    break;
                case "ImageValue":
                    this.propertyValue = this.footerproperties.BackgroundImage.ImageValue;
                    break;
            }
        }

        void footerproperties_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            object newValue = null;
            string propertyName = e.PropertyName;

            switch (propertyName)
            {
                case "DefaultBorderStyle":
                    {
                        newValue = this.footerproperties.BorderStyles.DefaultBorderStyle;
                        break;
                    }
                case "LefttBorderStyle":
                    {
                        newValue = this.footerproperties.BorderStyles.LeftBorderStyle;
                        break;
                    }
                case "RightBorderStyle":
                    {
                        newValue = this.footerproperties.BorderStyles.RightBorderStyle;
                        break;
                    }
                case "TopBorderStyle":
                    {
                        newValue = this.footerproperties.BorderStyles.TopBorderStyle;
                        break;
                    }
                case "BottomBorderStyle":
                    {
                        newValue = this.footerproperties.BorderStyles.BottomBorderStyle;
                        break;
                    }
                case "DefaultBorderWidth":
                    {
                        newValue = this.footerproperties.BorderWidths.DefaultBorderWidth;
                        break;
                    }
                case "LeftBorderWidth":
                    {
                        newValue = this.footerproperties.BorderWidths.LeftBorderWidth;
                        break;
                    }
                case "RightBorderWidth":
                    {
                        newValue = this.footerproperties.BorderWidths.RightBorderWidth;
                        break;
                    }
                case "TopBorderWidth":
                    {
                        newValue = this.footerproperties.BorderWidths.TopBorderWidth;
                        break;
                    }
                case "BottomBorderWidth":
                    {
                        newValue = this.footerproperties.BorderWidths.BottomBorderWidth;
                        break;
                    }
                case "DefaultBorderColor":
                    {
                        newValue = this.footerproperties.BorderColors.DefaultBorderColor;
                        break;
                    }
                case "LeftBorderColor":
                    {
                        newValue = this.footerproperties.BorderColors.LeftBorderColor;
                        break;
                    }
                case "RightBorderColor":
                    {
                        newValue = this.footerproperties.BorderColors.RightBorderColor;
                        break;
                    }
                case "TopBorderColor":
                    {
                        newValue = this.footerproperties.BorderColors.TopBorderColor;
                        break;
                    }
                case "BottomBorderColor":
                    {
                        newValue = this.footerproperties.BorderColors.BottomBorderColor;
                        break;
                    }
                case "BackgroundColor":
                    newValue = this.footerproperties.BackgroundColor;
                    if (string.IsNullOrEmpty(this.footerproperties.BackgroundImage.ImageValue))
                    {
                        this.footerCanvas.Background = converter.GetBackGroundColor(this.footerproperties.BackgroundColor);
                    }
                    break;
                case "FooterHeight":
                    newValue = this.footerproperties.FooterHeight;
                    if (this.IsFooterVisible)
                    {
                        if (ControlProperties.UnitType == RDL.DOM.ReportUnitType.Cm)
                        {
                            RDL.DOM.Size size = new RDL.DOM.Size(this.footerproperties.FooterHeight);
                            this.RulerVerticalFooter.Length = converter.GetRulerWidthValue(GetConvertedValue(size.FloatValue, size.MeasurementUnit, RDL.DOM.ReportUnitType.Cm));
                            this.footerRowDefination.Height = new GridLength(converter.GetPixelValue(GetConvertedValue(size.FloatValue, size.MeasurementUnit, RDL.DOM.ReportUnitType.In)));
                        }
                        else
                        {
                            this.RulerVerticalFooter.Length = converter.GetRulerWidthValue(this.footerproperties.FooterHeight);
                            this.footerRowDefination.Height = new GridLength(converter.GetPixelValue(this.footerproperties.FooterHeight));
                        }
                    }
                    break;
                case "Source":
                    newValue = this.footerproperties.BackgroundImage.Source;
                    break;
                case "ImageValue":
                    this.UpdateBackgroundImage((sender as Syncfusion.Windows.Reports.Designer.Editors.FooterProperties).Name);
                    newValue = this.footerproperties.BackgroundImage.ImageValue;
                    break;
            }

            if (!IsInternalChange)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemPropertyChanged;
                PropertyChanage change = new PropertyChanage();
                change.PropertyObject = this.footerproperties;

                switch (propertyName.ToUpper())
                {
                    case "DEFAULTBORDERSTYLE":
                    case "LEFTBORDERSTYLE":
                    case "RIGHTBORDERSTYLE":
                    case "TOPBORDERSTYLE":
                    case "BOTTOMBORDERSTYLE":
                        change.PropertyObject = this.headerproperties.BorderStyles;
                        break;

                    case "DEFAULTBORDERCOLOR":
                    case "LEFTBORDERCOLOR":
                    case "RIGHTBORDERCOLOR":
                    case "TOPBORDERCOLOR":
                    case "BOTTOMBORDERCOLOR":
                        change.PropertyObject = this.headerproperties.BorderColors;
                        break;

                    case "DEFAULTBORDERWIDTH":
                    case "LEFTBORDERWIDTH":
                    case "RIGHTBORDERWIDTH":
                    case "TOPBORDERWIDTH":
                    case "BOTTOMBORDERWIDTH":
                        change.PropertyObject = this.headerproperties.BorderWidths;
                        break;

                    case "SOURCE":
                    case "MIMETYPE":
                    case "IMAGEVALUE":
                        change.PropertyObject = this.headerproperties.BackgroundImage;
                        break;
                }

                change.PropertyName = propertyName;
                change.OldValue = this.propertyValue;
                change.NewValue = newValue;
                action.PropertyChange = change;
                this.EditingManager.AddAction(action);
            }
        }

        void headerproperties_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            string propertyName = e.PropertyName;

            switch (propertyName)
            {
                case "DefaultBorderStyle":
                    {
                        this.propertyValue = this.headerproperties.BorderStyles.DefaultBorderStyle;
                        break;
                    }
                case "LefttBorderStyle":
                    {
                        this.propertyValue = this.headerproperties.BorderStyles.LeftBorderStyle;
                        break;
                    }
                case "RightBorderStyle":
                    {
                        this.propertyValue = this.headerproperties.BorderStyles.RightBorderStyle;
                        break;
                    }
                case "TopBorderStyle":
                    {
                        this.propertyValue = this.headerproperties.BorderStyles.TopBorderStyle;
                        break;
                    }
                case "BottomBorderStyle":
                    {
                        this.propertyValue = this.headerproperties.BorderStyles.BottomBorderStyle;
                        break;
                    }
                case "DefaultBorderWidth":
                    {
                        this.propertyValue = this.headerproperties.BorderWidths.DefaultBorderWidth;
                        break;
                    }
                case "LeftBorderWidth":
                    {
                        this.propertyValue = this.headerproperties.BorderWidths.LeftBorderWidth;
                        break;
                    }
                case "RightBorderWidth":
                    {
                        this.propertyValue = this.headerproperties.BorderWidths.RightBorderWidth;
                        break;
                    }
                case "TopBorderWidth":
                    {
                        this.propertyValue = this.headerproperties.BorderWidths.TopBorderWidth;
                        break;
                    }
                case "BottomBorderWidth":
                    {
                        this.propertyValue = this.headerproperties.BorderWidths.BottomBorderWidth;
                        break;
                    }
                case "DefaultBorderColor":
                    {
                        this.propertyValue = this.headerproperties.BorderColors.DefaultBorderColor;
                        break;
                    }
                case "LeftBorderColor":
                    {
                        this.propertyValue = this.headerproperties.BorderColors.LeftBorderColor;
                        break;
                    }
                case "RightBorderColor":
                    {
                        this.propertyValue = this.headerproperties.BorderColors.RightBorderColor;
                        break;
                    }
                case "TopBorderColor":
                    {
                        this.propertyValue = this.headerproperties.BorderColors.TopBorderColor;
                        break;
                    }
                case "BottomBorderColor":
                    {
                        this.propertyValue = this.headerproperties.BorderColors.BottomBorderColor;
                        break;
                    }
                case "BackgroundColor":
                    this.propertyValue = this.headerproperties.BackgroundColor;
                    break;
                case "HeaderHeight":
                    this.propertyValue = this.headerproperties.HeaderHeight;
                    break;
                case "Source":
                    this.propertyValue = this.headerproperties.BackgroundImage.Source;
                    break;
                case "ImageValue":
                    this.propertyValue = this.headerproperties.BackgroundImage.ImageValue;
                    break;
                case "MIMEType":
                    this.propertyValue = this.headerproperties.BackgroundImage.MIMEType;
                    break;
            }
        }

        void headerproperties_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            object newValue = null;
            string propertyName = e.PropertyName;

            switch (propertyName)
            {
                case "DefaultBorderStyle":
                    {
                        newValue = this.headerproperties.BorderStyles.DefaultBorderStyle;
                        break;
                    }
                case "LefttBorderStyle":
                    {
                        newValue = this.headerproperties.BorderStyles.LeftBorderStyle;
                        break;
                    }
                case "RightBorderStyle":
                    {
                        newValue = this.headerproperties.BorderStyles.RightBorderStyle;
                        break;
                    }
                case "TopBorderStyle":
                    {
                        newValue = this.headerproperties.BorderStyles.TopBorderStyle;
                        break;
                    }
                case "BottomBorderStyle":
                    {
                        newValue = this.headerproperties.BorderStyles.BottomBorderStyle;
                        break;
                    }
                case "DefaultBorderWidth":
                    {
                        newValue = this.headerproperties.BorderWidths.DefaultBorderWidth;
                        break;
                    }
                case "LeftBorderWidth":
                    {
                        newValue = this.headerproperties.BorderWidths.LeftBorderWidth;
                        break;
                    }
                case "RightBorderWidth":
                    {
                        newValue = this.headerproperties.BorderWidths.RightBorderWidth;
                        break;
                    }
                case "TopBorderWidth":
                    {
                        newValue = this.headerproperties.BorderWidths.TopBorderWidth;
                        break;
                    }
                case "BottomBorderWidth":
                    {
                        newValue = this.headerproperties.BorderWidths.BottomBorderWidth;
                        break;
                    }
                case "DefaultBorderColor":
                    {
                        newValue = this.headerproperties.BorderColors.DefaultBorderColor;
                        break;
                    }
                case "LeftBorderColor":
                    {
                        newValue = this.headerproperties.BorderColors.LeftBorderColor;
                        break;
                    }
                case "RightBorderColor":
                    {
                        newValue = this.headerproperties.BorderColors.RightBorderColor;
                        break;
                    }
                case "TopBorderColor":
                    {
                        newValue = this.headerproperties.BorderColors.TopBorderColor;
                        break;
                    }
                case "BottomBorderColor":
                    {
                        newValue = this.headerproperties.BorderColors.BottomBorderColor;
                        break;
                    }
                case "BackgroundColor":
                    newValue = this.headerproperties.BackgroundColor;
                    if (string.IsNullOrEmpty(this.headerproperties.BackgroundImage.ImageValue))
                    {
                        this.headerCanvas.Background = converter.GetBackGroundColor(this.headerproperties.BackgroundColor);
                    }
                    break;
                case "HeaderHeight":
                    newValue = this.headerproperties.HeaderHeight;
                    if (this.isHeaderVisible)
                    {
                        if (ControlProperties.UnitType == RDL.DOM.ReportUnitType.Cm)
                        {
                            RDL.DOM.Size size = new RDL.DOM.Size(this.headerproperties.HeaderHeight);
                            this.RulerVerticalHeader.Length = converter.GetRulerWidthValue(GetConvertedValue(size.FloatValue, size.MeasurementUnit, RDL.DOM.ReportUnitType.Cm));
                            this.headerRowDefination.Height = new GridLength(converter.GetPixelValue(GetConvertedValue(size.FloatValue, size.MeasurementUnit, RDL.DOM.ReportUnitType.In)));
                        }
                        else
                        {
                            this.RulerVerticalHeader.Length = converter.GetRulerWidthValue(this.headerproperties.HeaderHeight);
                            this.headerRowDefination.Height = new GridLength(converter.GetPixelValue(this.headerproperties.HeaderHeight));
                        }
                    }
                    break;
                case "Source":
                    newValue = this.headerproperties.BackgroundImage.Source;
                    break;
                case "ImageValue":
                    this.UpdateBackgroundImage((sender as Syncfusion.Windows.Reports.Designer.Editors.HeaderProperties).Name);
                    newValue = this.headerproperties.BackgroundImage.ImageValue;
                    break;
                case "MIMEType":
                    newValue = this.headerproperties.BackgroundImage.MIMEType;
                    break;
            }

            if (!IsInternalChange)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemPropertyChanged;
                PropertyChanage change = new PropertyChanage();
                change.PropertyObject = this.headerproperties;

                switch (propertyName.ToUpper())
                {
                    case "DEFAULTBORDERSTYLE":
                    case "LEFTBORDERSTYLE":
                    case "RIGHTBORDERSTYLE":
                    case "TOPBORDERSTYLE":
                    case "BOTTOMBORDERSTYLE":
                        change.PropertyObject = this.headerproperties.BorderStyles;
                        break;

                    case "DEFAULTBORDERCOLOR":
                    case "LEFTBORDERCOLOR":
                    case "RIGHTBORDERCOLOR":
                    case "TOPBORDERCOLOR":
                    case "BOTTOMBORDERCOLOR":
                        change.PropertyObject = this.headerproperties.BorderColors;
                        break;

                    case "DEFAULTBORDERWIDTH":
                    case "LEFTBORDERWIDTH":
                    case "RIGHTBORDERWIDTH":
                    case "TOPBORDERWIDTH":
                    case "BOTTOMBORDERWIDTH":
                        change.PropertyObject = this.headerproperties.BorderWidths;
                        break;

                    case "SOURCE":
                    case "MIMETYPE":
                    case "IMAGEVALUE":
                        change.PropertyObject = this.headerproperties.BackgroundImage;
                        break;
                }

                change.PropertyName = propertyName;
                change.OldValue = this.propertyValue;
                change.NewValue = newValue;
                action.PropertyChange = change;
                this.EditingManager.AddAction(action);
            }
        }

        void bodyproperties_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            string propertyName = e.PropertyName;

            switch (propertyName)
            {
                case "DefaultBorderStyle":
                    {
                        this.propertyValue = this.bodyproperties.BorderStyles.DefaultBorderStyle;
                        break;
                    }
                case "LefttBorderStyle":
                    {
                        this.propertyValue = this.bodyproperties.BorderStyles.LeftBorderStyle;
                        break;
                    }
                case "RightBorderStyle":
                    {
                        this.propertyValue = this.bodyproperties.BorderStyles.RightBorderStyle;
                        break;
                    }
                case "TopBorderStyle":
                    {
                        this.propertyValue = this.bodyproperties.BorderStyles.TopBorderStyle;
                        break;
                    }
                case "BottomBorderStyle":
                    {
                        this.propertyValue = this.bodyproperties.BorderStyles.BottomBorderStyle;
                        break;
                    }
                case "DefaultBorderWidth":
                    {
                        this.propertyValue = this.bodyproperties.BorderWidths.DefaultBorderWidth;
                        break;
                    }
                case "LeftBorderWidth":
                    {
                        this.propertyValue = this.bodyproperties.BorderWidths.LeftBorderWidth;
                        break;
                    }
                case "RightBorderWidth":
                    {
                        this.propertyValue = this.bodyproperties.BorderWidths.RightBorderWidth;
                        break;
                    }
                case "TopBorderWidth":
                    {
                        this.propertyValue = this.bodyproperties.BorderWidths.TopBorderWidth;
                        break;
                    }
                case "BottomBorderWidth":
                    {
                        this.propertyValue = this.bodyproperties.BorderWidths.BottomBorderWidth;
                        break;
                    }
                case "DefaultBorderColor":
                    {
                        this.propertyValue = this.bodyproperties.BorderColors.DefaultBorderColor;
                        break;
                    }
                case "LeftBorderColor":
                    {
                        this.propertyValue = this.bodyproperties.BorderColors.LeftBorderColor;
                        break;
                    }
                case "RightBorderColor":
                    {
                        this.propertyValue = this.bodyproperties.BorderColors.RightBorderColor;
                        break;
                    }
                case "TopBorderColor":
                    {
                        this.propertyValue = this.bodyproperties.BorderColors.TopBorderColor;
                        break;
                    }
                case "BottomBorderColor":
                    {
                        this.propertyValue = this.bodyproperties.BorderColors.BottomBorderColor;
                        break;
                    }
                case "BackgroundColor":
                    this.propertyValue = this.bodyproperties.BackgroundColor;
                    break;
                case "ReportWidth":
                    this.propertyValue = this.bodyproperties.ReportWidth;
                    break;
                case "BodyHeight":
                    this.propertyValue = this.bodyproperties.BodyHeight;
                    break;
                case "Source":
                    this.propertyValue = this.bodyproperties.BackgroundImage.Source;
                    break;
                case "ImageValue":
                    this.propertyValue = this.bodyproperties.BackgroundImage.ImageValue;
                    break;
                case "MIMEType":
                    this.propertyValue = this.bodyproperties.BackgroundImage.MIMEType;
                    break;
            }
        }

        void bodyproperties_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            object newValue = null;
            string propertyName = e.PropertyName;

            switch (propertyName)
            {
                case "DefaultBorderStyle":
                    {
                        newValue = this.bodyproperties.BorderStyles.DefaultBorderStyle;
                        break;
                    }
                case "LefttBorderStyle":
                    {
                        newValue = this.bodyproperties.BorderStyles.LeftBorderStyle;
                        break;
                    }
                case "RightBorderStyle":
                    {
                        newValue = this.bodyproperties.BorderStyles.RightBorderStyle;
                        break;
                    }
                case "TopBorderStyle":
                    {
                        newValue = this.bodyproperties.BorderStyles.TopBorderStyle;
                        break;
                    }
                case "BottomBorderStyle":
                    {
                        newValue = this.bodyproperties.BorderStyles.BottomBorderStyle;
                        break;
                    }
                case "DefaultBorderWidth":
                    {
                        newValue = this.bodyproperties.BorderWidths.DefaultBorderWidth;
                        break;
                    }
                case "LeftBorderWidth":
                    {
                        newValue = this.bodyproperties.BorderWidths.LeftBorderWidth;
                        break;
                    }
                case "RightBorderWidth":
                    {
                        newValue = this.bodyproperties.BorderWidths.RightBorderWidth;
                        break;
                    }
                case "TopBorderWidth":
                    {
                        newValue = this.bodyproperties.BorderWidths.TopBorderWidth;
                        break;
                    }
                case "BottomBorderWidth":
                    {
                        newValue = this.bodyproperties.BorderWidths.BottomBorderWidth;
                        break;
                    }
                case "DefaultBorderColor":
                    {
                        newValue = this.bodyproperties.BorderColors.DefaultBorderColor;
                        break;
                    }
                case "LeftBorderColor":
                    {
                        newValue = this.bodyproperties.BorderColors.LeftBorderColor;
                        break;
                    }
                case "RightBorderColor":
                    {
                        newValue = this.bodyproperties.BorderColors.RightBorderColor;
                        break;
                    }
                case "TopBorderColor":
                    {
                        newValue = this.bodyproperties.BorderColors.TopBorderColor;
                        break;
                    }
                case "BottomBorderColor":
                    {
                        newValue = this.bodyproperties.BorderColors.BottomBorderColor;
                        break;
                    }
                case "BackgroundColor":
                    newValue = this.bodyproperties.BackgroundColor;
                    if (string.IsNullOrEmpty(this.bodyproperties.BackgroundImage.ImageValue))
                    {
                        this.bodyCanvas.Background = converter.GetBackGroundColor(this.bodyproperties.BackgroundColor);
                    }
                    break;
                case "ReportWidth":
                    newValue = this.bodyproperties.ReportWidth;
                    if (ControlProperties.UnitType == RDL.DOM.ReportUnitType.Cm)
                    {
                        RDL.DOM.Size size = new RDL.DOM.Size(this.bodyproperties.ReportWidth);
                        this.RulerHorizontal.Length = converter.GetRulerWidthValue(GetConvertedValue(size.FloatValue, size.MeasurementUnit, RDL.DOM.ReportUnitType.Cm));
                        this.reportWidthColumnDefinition.Width = new GridLength(converter.GetPixelValue(GetConvertedValue(size.FloatValue, size.MeasurementUnit, RDL.DOM.ReportUnitType.In)));
                    }
                    else
                    {
                        this.RulerHorizontal.Length = converter.GetRulerWidthValue(this.bodyproperties.ReportWidth);
                        this.reportWidthColumnDefinition.Width = new GridLength(converter.GetPixelValue(this.bodyproperties.ReportWidth));
                    }
                    break;
                case "BodyHeight":
                    newValue = this.bodyproperties.BodyHeight;
                    if (ControlProperties.UnitType == RDL.DOM.ReportUnitType.Cm)
                    {
                        RDL.DOM.Size size = new RDL.DOM.Size(this.bodyproperties.BodyHeight);
                        this.RulerVerticalBody.Length = converter.GetRulerWidthValue(GetConvertedValue(size.FloatValue, size.MeasurementUnit, RDL.DOM.ReportUnitType.Cm));
                        this.bodyRowDefination.Height = new GridLength(converter.GetPixelValue(GetConvertedValue(size.FloatValue, size.MeasurementUnit, RDL.DOM.ReportUnitType.In)));
                    }
                    else
                    {
                        this.RulerVerticalBody.Length = converter.GetRulerWidthValue(this.bodyproperties.BodyHeight);
                        this.bodyRowDefination.Height = new GridLength(converter.GetPixelValue(this.bodyproperties.BodyHeight));
                    }
                    break;
                case "Source":
                    newValue = this.bodyproperties.BackgroundImage.Source;
                    break;
                case "ImageValue":
                    this.UpdateBackgroundImage((sender as Syncfusion.Windows.Reports.Designer.Editors.BodyProperties).Name);
                    newValue = this.bodyproperties.BackgroundImage.ImageValue;
                    break;
                case "MIMEType":
                    newValue = this.bodyproperties.BackgroundImage.MIMEType;
                    break;
            }

            if (!IsInternalChange)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemPropertyChanged;
                PropertyChanage change = new PropertyChanage();
                change.PropertyObject = this.bodyproperties;

                switch (propertyName.ToUpper())
                {
                    case "DEFAULTBORDERSTYLE":
                    case "LEFTBORDERSTYLE":
                    case "RIGHTBORDERSTYLE":
                    case "TOPBORDERSTYLE":
                    case "BOTTOMBORDERSTYLE":
                        change.PropertyObject = this.bodyproperties.BorderStyles;
                        break;

                    case "DEFAULTBORDERCOLOR":
                    case "LEFTBORDERCOLOR":
                    case "RIGHTBORDERCOLOR":
                    case "TOPBORDERCOLOR":
                    case "BOTTOMBORDERCOLOR":
                        change.PropertyObject = this.bodyproperties.BorderColors;
                        break;

                    case "DEFAULTBORDERWIDTH":
                    case "LEFTBORDERWIDTH":
                    case "RIGHTBORDERWIDTH":
                    case "TOPBORDERWIDTH":
                    case "BOTTOMBORDERWIDTH":
                        change.PropertyObject = this.bodyproperties.BorderWidths;
                        break;

                    case "SOURCE":
                    case "MIMETYPE":
                    case "IMAGEVALUE":
                        change.PropertyObject = this.bodyproperties.BackgroundImage;
                        break;
                }

                change.PropertyName = propertyName;
                change.OldValue = this.propertyValue;
                change.NewValue = newValue;
                action.PropertyChange = change;
                this.EditingManager.AddAction(action);
            }
        }

        void reportproperties_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            string propertyName = e.PropertyName;

            switch (propertyName)
            {
                case "DefaultBorderStyle":
                    {
                        this.propertyValue = this.reportproperties.BorderStyles.DefaultBorderStyle;
                        break;
                    }
                case "LefttBorderStyle":
                    {
                        this.propertyValue = this.reportproperties.BorderStyles.LeftBorderStyle;
                        break;
                    }
                case "RightBorderStyle":
                    {
                        this.propertyValue = this.reportproperties.BorderStyles.RightBorderStyle;
                        break;
                    }
                case "TopBorderStyle":
                    {
                        this.propertyValue = this.reportproperties.BorderStyles.TopBorderStyle;
                        break;
                    }
                case "BottomBorderStyle":
                    {
                        this.propertyValue = this.reportproperties.BorderStyles.BottomBorderStyle;
                        break;
                    }
                case "DefaultBorderWidth":
                    {
                        this.propertyValue = this.reportproperties.BorderWidths.DefaultBorderWidth;
                        break;
                    }
                case "LeftBorderWidth":
                    {
                        this.propertyValue = this.reportproperties.BorderWidths.LeftBorderWidth;
                        break;
                    }
                case "RightBorderWidth":
                    {
                        this.propertyValue = this.reportproperties.BorderWidths.RightBorderWidth;
                        break;
                    }
                case "TopBorderWidth":
                    {
                        this.propertyValue = this.reportproperties.BorderWidths.TopBorderWidth;
                        break;
                    }
                case "BottomBorderWidth":
                    {
                        this.propertyValue = this.reportproperties.BorderWidths.BottomBorderWidth;
                        break;
                    }
                case "DefaultBorderColor":
                    {
                        this.propertyValue = this.reportproperties.BorderColors.DefaultBorderColor;
                        break;
                    }
                case "LeftBorderColor":
                    {
                        this.propertyValue = this.reportproperties.BorderColors.LeftBorderColor;
                        break;
                    }
                case "RightBorderColor":
                    {
                        this.propertyValue = this.reportproperties.BorderColors.RightBorderColor;
                        break;
                    }
                case "TopBorderColor":
                    {
                        this.propertyValue = this.reportproperties.BorderColors.TopBorderColor;
                        break;
                    }
                case "BottomBorderColor":
                    {
                        this.propertyValue = this.reportproperties.BorderColors.BottomBorderColor;
                        break;
                    }
                case "LeftMargin":
                    this.propertyValue = this.reportproperties.Margins.LeftMargin;
                    break;
                case "RightMargin":
                    this.propertyValue = this.reportproperties.Margins.RightMargin;
                    break;
                case "TopMargin":
                    this.propertyValue = this.reportproperties.Margins.TopMargin;
                    break;
                case "BottomMargin":
                    this.propertyValue = this.reportproperties.Margins.BottomMargin;
                    break;
                case "PageWidth":
                    this.propertyValue = this.reportproperties.PageWidth;
                    break;
                case "PageHeight":
                    this.propertyValue = this.reportproperties.PageHeight;
                    break;
                case "Author":
                    this.propertyValue = this.reportproperties.Author;
                    break;
                case "Description":
                    this.propertyValue = this.reportproperties.Description;
                    break;
                case "AutoRefresh":
                    this.propertyValue = this.reportproperties.AutoRefresh;
                    break;
                case "Source":
                    this.propertyValue = this.reportproperties.BackgroundImage.Source;
                    break;
                case "BackgroundColor":
                    this.propertyValue = this.reportproperties.BackgroundColor;
                    break;
                case "ImageValue":
                    this.propertyValue = this.reportproperties.BackgroundImage.ImageValue;
                    break;
                case "MIMEType":
                    this.propertyValue = this.reportproperties.BackgroundImage.MIMEType;
                    break;
                case "DataElementName":
                    this.propertyValue = this.reportproperties.DataElementName;
                    break;
                case "DataElementStyle":
                    this.propertyValue = this.reportproperties.DataElementStyle;
                    break;
                case "DataTransform":
                    this.propertyValue = this.reportproperties.DataTransform;
                    break;
                case "DataSchema":
                    this.propertyValue = this.reportproperties.DataSchema;
                    break;
            }
        }

        void reportproperties_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            object newValue = null;
            string propertyName = e.PropertyName;

            switch (propertyName)
            {
                case "DefaultBorderStyle":
                    {
                        newValue = this.reportproperties.BorderStyles.DefaultBorderStyle;
                        break;
                    }
                case "LefttBorderStyle":
                    {
                        newValue = this.reportproperties.BorderStyles.LeftBorderStyle;
                        break;
                    }
                case "RightBorderStyle":
                    {
                        newValue = this.reportproperties.BorderStyles.RightBorderStyle;
                        break;
                    }
                case "TopBorderStyle":
                    {
                        newValue = this.reportproperties.BorderStyles.TopBorderStyle;
                        break;
                    }
                case "BottomBorderStyle":
                    {
                        newValue = this.reportproperties.BorderStyles.BottomBorderStyle;
                        break;
                    }
                case "DefaultBorderWidth":
                    {
                        newValue = this.reportproperties.BorderWidths.DefaultBorderWidth;
                        break;
                    }
                case "LeftBorderWidth":
                    {
                        newValue = this.reportproperties.BorderWidths.LeftBorderWidth;
                        break;
                    }
                case "RightBorderWidth":
                    {
                        newValue = this.reportproperties.BorderWidths.RightBorderWidth;
                        break;
                    }
                case "TopBorderWidth":
                    {
                        newValue = this.reportproperties.BorderWidths.TopBorderWidth;
                        break;
                    }
                case "BottomBorderWidth":
                    {
                        newValue = this.reportproperties.BorderWidths.BottomBorderWidth;
                        break;
                    }
                case "DefaultBorderColor":
                    {
                        newValue = this.reportproperties.BorderColors.DefaultBorderColor;
                        break;
                    }
                case "LeftBorderColor":
                    {
                        newValue = this.reportproperties.BorderColors.LeftBorderColor;
                        break;
                    }
                case "RightBorderColor":
                    {
                        newValue = this.reportproperties.BorderColors.RightBorderColor;
                        break;
                    }
                case "TopBorderColor":
                    {
                        newValue = this.reportproperties.BorderColors.TopBorderColor;
                        break;
                    }
                case "BottomBorderColor":
                    {
                        newValue = this.reportproperties.BorderColors.BottomBorderColor;
                        break;
                    }
                case "LeftMargin":
                    newValue = this.reportproperties.Margins.LeftMargin;
                    break;
                case "RightMargin":
                    newValue = this.reportproperties.Margins.RightMargin;
                    break;
                case "TopMargin":
                    newValue = this.reportproperties.Margins.TopMargin;
                    break;
                case "BottomMargin":
                    newValue = this.reportproperties.Margins.BottomMargin;
                    break;
                case "PageWidth":
                    newValue = this.reportproperties.PageWidth;
                    break;
                case "PageHeight":
                    newValue = this.reportproperties.PageHeight;
                    break;
                case "Author":
                    newValue = this.reportproperties.Author;
                    break;
                case "Description":
                    newValue = this.reportproperties.Description;
                    break;
                case "AutoRefresh":
                    newValue = this.reportproperties.AutoRefresh;
                    break;
                case "BackgroundColor":
                    newValue = this.reportproperties.BackgroundColor;
                    break;
                case "Source":
                    newValue = this.reportproperties.BackgroundImage.Source;
                    break;
                case "ImageValue":
                    newValue = this.reportproperties.BackgroundImage.ImageValue;
                    break;
                case "MIMEType":
                    newValue = this.reportproperties.BackgroundImage.MIMEType;
                    break;
                case "DataElementName":
                    newValue = this.reportproperties.DataElementName;
                    break;
                case "DataElementStyle":
                    newValue = this.reportproperties.DataElementStyle;
                    break;
                case "DataTransform":
                    newValue = this.reportproperties.DataTransform;
                    break;
                case "DataSchema":
                    newValue = this.reportproperties.DataSchema;
                    break;
            }

            if (!IsInternalChange)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemPropertyChanged;
                PropertyChanage change = new PropertyChanage();
                change.PropertyObject = this.reportproperties;

                switch (propertyName.ToUpper())
                {
                    case "DEFAULTBORDERSTYLE":
                    case "LEFTBORDERSTYLE":
                    case "RIGHTBORDERSTYLE":
                    case "TOPBORDERSTYLE":
                    case "BOTTOMBORDERSTYLE":
                        change.PropertyObject = this.reportproperties.BorderStyles;
                        break;

                    case "DEFAULTBORDERCOLOR":
                    case "LEFTBORDERCOLOR":
                    case "RIGHTBORDERCOLOR":
                    case "TOPBORDERCOLOR":
                    case "BOTTOMBORDERCOLOR":
                        change.PropertyObject = this.reportproperties.BorderColors;
                        break;

                    case "DEFAULTBORDERWIDTH":
                    case "LEFTBORDERWIDTH":
                    case "RIGHTBORDERWIDTH":
                    case "TOPBORDERWIDTH":
                    case "BOTTOMBORDERWIDTH":
                        change.PropertyObject = this.reportproperties.BorderWidths;
                        break;

                    case "LEFTMARGIN":
                    case "RIGHTMARGIN":
                    case "TOPMARGIN":
                    case "BOTTOMMARGIN":
                        change.PropertyObject = this.reportproperties.Margins;
                        break;

                    case "SOURCE":
                    case "MIMETYPE":
                    case "IMAGEVALUE":
                    case "BACKGROUNDREPEAT":
                        change.PropertyObject = this.reportproperties.BackgroundImage;
                        break;
                }

                change.PropertyName = propertyName;
                change.OldValue = this.propertyValue;
                change.NewValue = newValue;
                action.PropertyChange = change;
                this.EditingManager.AddAction(action);
            }
        }

        #endregion

        #region Overrriden methods

        public override void OnApplyTemplate()
        {
            #region Design to object

            RowDefinitionRulerHeader = GetTemplateChild("RowDefinitionRulerHeader") as RowDefinition;
            RowDefinitionRulerBody = GetTemplateChild("RowDefinitionRulerBody") as RowDefinition;
            RowDefinitionRulerFooter = GetTemplateChild("RowDefinitionRulerFooter") as RowDefinition;
            RowDefinitionHorizontalRuler = GetTemplateChild("RowDefinitionHorizontalRuler") as RowDefinition;

            HorizontalRulerColumnDefinition = GetTemplateChild("HorizontalRulerColumnDefinition") as ColumnDefinition;
            ColumnDefinitionVerticalRuler = GetTemplateChild("ColumnDefinitionVerticalRuler") as ColumnDefinition;

            RulerVerticalHeader = GetTemplateChild("RulerVerticalHeader") as Ruler;
            RulerVerticalBody = GetTemplateChild("RulerVerticalBody") as Ruler;
            RulerVerticalFooter = GetTemplateChild("RulerVerticalFooter") as Ruler;
            RulerHorizontal = GetTemplateChild("RulerHorizontal") as Ruler;

            DesignerArea = GetTemplateChild("DesignerAreaGrid") as Grid;
            DesignerInnerArea = GetTemplateChild("DesignerInnerAreaGrid") as Grid;
            GridVertialRuler = GetTemplateChild("GridVertialRuler") as Grid;
            GridHorizontalRuler = GetTemplateChild("GridHorizontalRuler") as Grid;
            GridCorner = GetTemplateChild("GridCorner") as Grid;

            HeaderGrid = GetTemplateChild("HeaderGrid") as Grid;
            BodyGrid = GetTemplateChild("BodyGrid") as Grid;
            FooterGrid = GetTemplateChild("FooterGrid") as Grid;

            GridDesignPanel = GetTemplateChild("GridDesignPanel") as Grid;

            GridFooterRuler = GetTemplateChild("GridFooterRuler") as Grid;
            GridHeaderRuler = GetTemplateChild("GridHeaderRuler") as Grid;

            FooterRulerSeperator = GetTemplateChild("FooterRulerSeperator") as System.Windows.Controls.Separator;
            HeaderRulerSeperator = GetTemplateChild("HeaderRulerSeperator") as System.Windows.Controls.Separator;

            gridSplitterBody = GetTemplateChild("gridSplitterBody") as GridSplitter;
            gridSplitterHeader = GetTemplateChild("gridSplitterHeader") as GridSplitter;
            gridSplitterFooter = GetTemplateChild("gridSplitterFooter") as GridSplitter;
            gridSplitterReport = GetTemplateChild("gridSplitterReport") as GridSplitter;

            BodyContextMenuInsertFooter = GetTemplateChild("BodyContextMenuInsertFooter") as MenuItem;
            BodyContextMenuInsertHeader = GetTemplateChild("BodyContextMenuInsertHeader") as MenuItem;

            MenuItemHeaderRemoveHeader = GetTemplateChild("MenuItemHeaderRemoveHeader") as MenuItem;
            MenuItemHeaderInsertTextbox = GetTemplateChild("MenuItemHeaderInsertTextbox") as MenuItem;
            MenuItemHeaderInsertLine = GetTemplateChild("MenuItemHeaderInsertLine") as MenuItem;
            MenuItemHeaderInsertRectangle = GetTemplateChild("MenuItemHeaderInsertRectangle") as MenuItem;
            MenuItemHeaderInsertImage = GetTemplateChild("MenuItemHeaderInsertImage") as MenuItem;

            MenuItemBodyInsertTextbox = GetTemplateChild("MenuItemBodyInsertTextbox") as MenuItem;
            MenuItemBodyInsertLine = GetTemplateChild("MenuItemBodyInsertLine") as MenuItem;
            MenuItemBodyInsertTable = GetTemplateChild("MenuItemBodyInsertTable") as MenuItem;
            MenuItemBodyInsertMatrix = GetTemplateChild("MenuItemBodyInsertMatrix") as MenuItem;
            MenuItemBodyInsertRectangle = GetTemplateChild("MenuItemBodyInsertRectangle") as MenuItem;
            MenuItemBodyInsertList = GetTemplateChild("MenuItemBodyInsertList") as MenuItem;
            MenuItemBodyInsertImage = GetTemplateChild("MenuItemBodyInsertImage") as MenuItem;
            MenuItemBodyInsertSubreport = GetTemplateChild("MenuItemBodyInsertSubreport") as MenuItem;
            MenuItemBodyInsertChart = GetTemplateChild("MenuItemBodyInsertChart") as MenuItem;
            MenuItemBodyInsertGauge = GetTemplateChild("MenuItemBodyInsertGauge") as MenuItem;
            MenuItemBodyInsertMap = GetTemplateChild("MenuItemBodyInsertMap") as MenuItem;

            MenuItemFooterRemoveFooter = GetTemplateChild("MenuItemFooterRemoveFooter") as MenuItem;
            MenuItemFooterInsertTextbox = GetTemplateChild("MenuItemFooterInsertTextbox") as MenuItem;
            MenuItemFooterInsertLine = GetTemplateChild("MenuItemFooterInsertLine") as MenuItem;
            MenuItemFooterInsertRectangle = GetTemplateChild("MenuItemFooterInsertRectangle") as MenuItem;
            MenuItemFooterInsertImage = GetTemplateChild("MenuItemFooterInsertImage") as MenuItem;

            scrollViewerReportBody = GetTemplateChild("scrollViewerReportBody") as ScrollViewer;

            this.bodyCanvas = GetTemplateChild("BodyCanvas") as Canvas;
            this.headerCanvas = this.GetTemplateChild("HeaderCanvas") as Canvas;
            this.footerCanvas = this.GetTemplateChild("FooterCanvas") as Canvas;

            this.headerSelectionBox = GetTemplateChild("HeaderSelectionBox") as System.Windows.Shapes.Rectangle;
            this.bodySelectionBox = GetTemplateChild("BodySelectionBox") as System.Windows.Shapes.Rectangle;
            this.footerSelectionBox = GetTemplateChild("FooterSelectionBox") as System.Windows.Shapes.Rectangle;

            this.headerRowDefination = GetTemplateChild("headerRowDefination") as RowDefinition;
            this.bodyRowDefination = GetTemplateChild("bodyRowDefination") as RowDefinition;
            this.footerRowDefination = GetTemplateChild("footerRowDefination") as RowDefinition;
            this.reportWidthColumnDefinition = GetTemplateChild("reportColumnDefination") as ColumnDefinition;

            #endregion

            this.scrollViewerReportBody.ScrollToHome();

            var splitters = from splitter in this.DesignerInnerArea.Children.OfType<GridSplitter>()
                            select splitter;

            foreach (var splitter in splitters)
            {
                Grid.SetZIndex(splitter, 1);
            }

            this.UpdateRulerVisiblity();
            this.WireEvents();

            this.PopulateDesignPanel();
            this.RaiseReportItemSelectedEvent(this, new SelectedItemEventArgs() { SelectedItem = this.reportproperties });

            base.OnApplyTemplate();

            this.UpdateStyle();
            this.UpdatePanelZoom();
            ///Reset the DirtyReport value.
            this.reportDesignView.ResetDirtyReport();
        }

        #endregion

        #region Design Panel Grid Events

        public static void OnIsRulerVisiblePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            DesignPanel designPanel = dependencyObject as DesignPanel;

            if (designPanel != null)
            {
                designPanel.UpdateRulerVisiblity();
            }
        }

        void UpdateRulerVisiblity()
        {
            if (this.GridCorner != null)
            {
                this.GridCorner.Visibility = converter.GetVisiblity(this.IsRulerVisible);
                this.GridHorizontalRuler.Visibility = this.GridCorner.Visibility;
                this.GridVertialRuler.Visibility = this.GridCorner.Visibility;
            }
        }

        void ClearSelection()
        {
            foreach (IReportItemControl element in this.SelectedReportItems)
            {
                element.IsFocusedItem = false;
                element.IsItemSelected = false;
            }

            this.SelectedReportItems.Clear();
        }

        void DesignerArea_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.isTablixControlSelected = false;
            this.SetZindex();
            if (selectionBox != null)
            {
                this.selectionBox.Visibility = System.Windows.Visibility.Collapsed;
                this.selectionBox = null;
            }

            this.UpdateDrawingCanvas();
            bool isDrawingItem = this.SelectedReportItemType != DrawingReportItem.None;

            IReportItemControl selectedReportItem = this.SelectedReportItems.Count == 1 && !isSelectionkeypressed ? this.SelectedReportItems.First() : null;

            UIElement sourceControl = e.Source as UIElement;

            if (sourceControl is System.Windows.Shapes.Line && (e.Source as FrameworkElement).Parent is LineControl)
            {
                sourceControl = (e.Source as FrameworkElement).Parent as UIElement;
            }

            if ((e.Source as FrameworkElement).Parent is CellContentsControl && !(e.Source is RectangleControl && isDrawingItem))
            {
                sourceControl = ((e.Source as FrameworkElement).Parent as CellContentsControl).Parent as UIElement;
                if (isDrawingItem)
                {
                    this.isTablixControlSelected = true;
                }
            }

            if (isDrawingItem)
            {
                selectedReportItem = null;
                this.ClearSelection();
            }
            else if (sourceControl is IReportItemControl && !Object.ReferenceEquals(sourceControl, selectedReportItem) && !isSelectionkeypressed)
            {
                this.ClearSelection();
                IReportItemControl reportItemControl = sourceControl as IReportItemControl;
                reportItemControl.IsItemSelected = true;
                reportItemControl.IsFocusedItem = false;
                this.SelectedReportItems.Add(reportItemControl);
                selectedReportItem = reportItemControl;
                e.Handled = true;
            }
            else if (sourceControl is IReportItemControl && Object.ReferenceEquals(sourceControl, selectedReportItem) && !isSelectionkeypressed)
            {
                selectedReportItem.IsFocusedItem = true;
            }

            if (isDrawingItem && this.drawingCanvas != null && !(sourceControl is TablixControl))
            {
                this.itemStartPoint = e.GetPosition(sender as Grid);

                if (sourceControl is RectangleControl)
                {
                    this.drawingCanvas = e.Source as Canvas;
                    this.itemStartPoint = e.GetPosition(sender as Grid);
                    this.selectionBox = this.drawingCanvas.Children[0] as System.Windows.Shapes.Rectangle;
                }

                this.selectionBox.Height = 0;
                this.selectionBox.Width = 0;
                this.selectionBox.Visibility = System.Windows.Visibility.Visible;
                this.selectionBox.StrokeDashArray = dotted;
            }
            else if (isSelectionkeypressed && sourceControl is IReportItemControl && this.drawingCanvas != null)
            {
                IReportItemControl reportItemControl = sourceControl as IReportItemControl;

                if (reportItemControl.IsItemSelected)
                {
                    reportItemControl.IsItemSelected = false;
                    reportItemControl.IsFocusedItem = false;
                    this.SelectedReportItems.Remove(reportItemControl);
                }
                else
                {
                    reportItemControl.IsItemSelected = true;
                    this.SelectedReportItems.Add(reportItemControl);
                }

                selectedReportItem = reportItemControl;
                e.Handled = true;
            }
            else if (sourceControl is RectangleControl)
            {
                this.drawingCanvas = e.Source as RectangleControl;
                this.itemStartPoint = e.GetPosition(sender as Grid);
                this.selectionBox = this.drawingCanvas.Children[0] as System.Windows.Shapes.Rectangle;
                this.selectionBox.Visibility = System.Windows.Visibility.Visible;
                this.selectionBox.StrokeDashArray = dashed;
                this.selectionBox.Height = 0;
                this.selectionBox.Width = 0;
                this.isSelection = true;
                this.selectionStartPoint = Mouse.GetPosition(drawingCanvas);
            }

            else if (sourceControl is Canvas && !(sourceControl is LineControl) && (this.selectionBox != null))
            {
                this.ClearSelection();

                this.itemStartPoint = e.GetPosition(sender as Grid);

                if (selectedReportItem != null)
                {
                    selectedReportItem.IsItemSelected = false;
                    selectedReportItem.IsFocusedItem = false;
                    selectedReportItem = null;
                }

                this.selectionBox.Height = 0;
                this.selectionBox.Width = 0;
                this.selectionBox.Visibility = System.Windows.Visibility.Visible;
                this.selectionBox.StrokeDashArray = dashed;
                this.isSelection = true;
                this.selectionStartPoint = Mouse.GetPosition(drawingCanvas);
            }

            else if (!(sourceControl is GridSplitter) && !(sourceControl is IReportItemControl))
            {
                this.ClearSelection();
            }

            if (this.selectionBox != null && this.drawingCanvas != null)
            {
                this.canvasStartingPoint = Mouse.GetPosition(drawingCanvas);
                Canvas.SetZIndex(this.selectionBox, 1);
                Canvas.SetLeft(this.selectionBox, canvasStartingPoint.X);
                Canvas.SetTop(this.selectionBox, canvasStartingPoint.Y);
            }

            if (selectedReportItem == null && this.SelectedReportItemType == DrawingReportItem.None)
            {
                if (!(sourceControl is GridSplitter))
                {
                    if (object.ReferenceEquals(sourceControl, headerCanvas))
                    {
                        this.RaiseReportItemSelectedEvent(this, new SelectedItemEventArgs() { SelectedItem = this.headerproperties });
                    }
                    else if (object.ReferenceEquals(sourceControl, bodyCanvas))
                    {
                        this.RaiseReportItemSelectedEvent(this, new SelectedItemEventArgs() { SelectedItem = this.bodyproperties });
                    }
                    else if (object.ReferenceEquals(sourceControl, footerCanvas))
                    {
                        this.RaiseReportItemSelectedEvent(this, new SelectedItemEventArgs() { SelectedItem = this.footerproperties });
                    }
                    else
                    {
                        this.RaiseReportItemSelectedEvent(this, new SelectedItemEventArgs() { SelectedItem = this.reportproperties });
                    }
                }
            }

            if (selectionBox != null && !(sourceControl is GridSplitter) && (isDrawingItem && selectedReportItem == null))
            {
                this.DesignerArea.CaptureMouse();
            }

        }

        public void SetZindex()
        {
            HitTestResult result = VisualTreeHelper.HitTest(this.bodyCanvas, Mouse.GetPosition(this.bodyCanvas));

            if (result == null)
            {
                result = VisualTreeHelper.HitTest(this.headerCanvas, Mouse.GetPosition(this.headerCanvas));

                if (result == null)
                {
                    result = VisualTreeHelper.HitTest(this.footerCanvas, Mouse.GetPosition(this.footerCanvas));

                    if (result != null)
                    {
                        this.drawingGrid = this.FooterGrid;
                    }
                }
                else
                {
                    this.drawingGrid = this.HeaderGrid;
                }
            }
            else
            {
                this.drawingGrid = this.BodyGrid;
            }

            if (this.drawingGrid != null)
            {
                Grid.SetZIndex(this.drawingGrid, 1);
            }
        }

        void DesignerArea_MouseMove(object sender, MouseEventArgs e)
        {
            this.canvasEndPoint = Mouse.GetPosition(drawingCanvas);
            System.Windows.Resources.StreamResourceInfo info = null;

            switch (this.SelectedReportItemType)
            {
                case DrawingReportItem.TextBox:
                    {
                        info = Application.GetResourceStream(new Uri(@"pack://application:,,,/Syncfusion.ReportDesigner.WPF;component//Images/Text.cur", UriKind.RelativeOrAbsolute));
                        break;
                    }
                case DrawingReportItem.Line:
                    {
                        info = Application.GetResourceStream(new Uri(@"pack://application:,,,/Syncfusion.ReportDesigner.WPF;component//Images/Line.cur", UriKind.RelativeOrAbsolute));
                        break;
                    }
                case DrawingReportItem.List:
                    {
                        info = Application.GetResourceStream(new Uri(@"pack://application:,,,/Syncfusion.ReportDesigner.WPF;component//Images/List.cur", UriKind.RelativeOrAbsolute));
                        break;
                    }
                case DrawingReportItem.Rectangle:
                    {
                        info = Application.GetResourceStream(new Uri(@"pack://application:,,,/Syncfusion.ReportDesigner.WPF;component//Images/Rectangle.cur", UriKind.RelativeOrAbsolute));
                        break;
                    }

                case DrawingReportItem.Image:
                    {
                        info = Application.GetResourceStream(new Uri(@"pack://application:,,,/Syncfusion.ReportDesigner.WPF;component//Images/Image.cur", UriKind.RelativeOrAbsolute));
                        break;
                    }
                case DrawingReportItem.Tablix:
                    {
                        info = Application.GetResourceStream(new Uri(@"pack://application:,,,/Syncfusion.ReportDesigner.WPF;component//Images/Table.cur", UriKind.RelativeOrAbsolute));
                        break;
                    }
                case DrawingReportItem.Chart:
                    {
                        info = Application.GetResourceStream(new Uri(@"pack://application:,,,/Syncfusion.ReportDesigner.WPF;component//Images/Chart.cur", UriKind.RelativeOrAbsolute));
                        break;
                    }
                case DrawingReportItem.DataBar:
                    {
                        info = Application.GetResourceStream(new Uri(@"pack://application:,,,/Syncfusion.ReportDesigner.WPF;component//Images/databar.cur", UriKind.RelativeOrAbsolute));
                        break;
                    }
                case DrawingReportItem.Sparkline:
                    {
                        info = Application.GetResourceStream(new Uri(@"pack://application:,,,/Syncfusion.ReportDesigner.WPF;component//Images/sparkline.cur", UriKind.RelativeOrAbsolute));
                        break;
                    }
                case DrawingReportItem.Map:
                    {
                        info = Application.GetResourceStream(new Uri(@"pack://application:,,,/Syncfusion.ReportDesigner.WPF;component//Images/Map.cur", UriKind.RelativeOrAbsolute));
                        break;
                    }
                case DrawingReportItem.Gauge:
                    {
                        info = Application.GetResourceStream(new Uri(@"pack://application:,,,/Syncfusion.ReportDesigner.WPF;component//Images/Gauge.cur", UriKind.RelativeOrAbsolute));
                        break;
                    }
                case DrawingReportItem.SubReport:
                    {
                        info = Application.GetResourceStream(new Uri(@"pack://application:,,,/Syncfusion.ReportDesigner.WPF;component//Images/SubReport.cur", UriKind.RelativeOrAbsolute));
                        break;
                    }
                case DrawingReportItem.None:
                    {
                        if (e.Source is Canvas)
                            (e.Source as Canvas).Cursor = Cursors.Arrow;
                        break;
                    }
            }

            if (this.SelectedReportItemType != DrawingReportItem.None && info != null)
            {
                if (e.Source is Canvas)
                {
                    (e.Source as Canvas).Cursor = new System.Windows.Input.Cursor(info.Stream);
                }
            }
            if (this.selectionBox != null)
            {
                var point = e.GetPosition(sender as Grid);
                if (point.X > itemStartPoint.X)
                {
                    if (!(this.SelectedReportItemType == DrawingReportItem.None && drawingCanvas != null && drawingCanvas.ActualWidth - Canvas.GetLeft(selectionBox) < selectionBox.Width && point.X - itemStartPoint.X >= selectionBox.Width))
                    {
                        selectionBox.Width = point.X - itemStartPoint.X;
                        //selectionBox.Width = Mouse.GetPosition(null).X - itemStartPoint.X;
                    }
                }
                else if (drawingCanvas != null && (this.SelectedReportItemType == DrawingReportItem.None || this.SelectedReportItemType == DrawingReportItem.Line) && canvasStartingPoint.X - Mouse.GetPosition(drawingCanvas).X > 0 && (Mouse.GetPosition(drawingCanvas).X > 0 || this.SelectedReportItemType == DrawingReportItem.Line))
                {
                    Canvas.SetLeft(selectionBox, Mouse.GetPosition(drawingCanvas).X);
                    selectionBox.Width = canvasStartingPoint.X - Mouse.GetPosition(drawingCanvas).X;
                }

                if (point.Y > itemStartPoint.Y)
                {
                    if (!(this.SelectedReportItemType == DrawingReportItem.None && drawingCanvas != null && drawingCanvas.ActualHeight - Canvas.GetTop(selectionBox) < selectionBox.Height && point.Y - itemStartPoint.Y >= selectionBox.Height))
                    {
                        selectionBox.Height = point.Y - itemStartPoint.Y;
                        //selectionBox.Height = Mouse.GetPosition(null).Y - itemStartPoint.Y;
                    }
                }
                else if (drawingCanvas != null && (this.SelectedReportItemType == DrawingReportItem.None || this.SelectedReportItemType == DrawingReportItem.Line) && canvasStartingPoint.Y - Mouse.GetPosition(drawingCanvas).Y > 0 && (Mouse.GetPosition(drawingCanvas).Y > 0 || this.SelectedReportItemType == DrawingReportItem.Line))
                {
                    Canvas.SetTop(selectionBox, Mouse.GetPosition(drawingCanvas).Y);
                    selectionBox.Height = canvasStartingPoint.Y - Mouse.GetPosition(drawingCanvas).Y;
                }
            }
        }

        void DesignerArea_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.drawingGrid != null)
            {
                Grid.SetZIndex(this.drawingGrid, 0);
                this.drawingGrid = null;
            }
            IReportItemControl selectedReportItem = this.SelectedReportItems.Count > 0 ? this.SelectedReportItems.First() : null;

            if (this.drawingCanvas != null && this.SelectedReportItemType != DrawingReportItem.None)
            {
                if ((this.drawingCanvas == bodyCanvas || this.drawingCanvas is RectangleControl) ||
                    ((this.drawingCanvas == headerCanvas ||
                    this.drawingCanvas == footerCanvas) &&
                    (this.SelectedReportItemType == DrawingReportItem.Line ||
                    this.SelectedReportItemType == DrawingReportItem.Image ||
                    this.SelectedReportItemType == DrawingReportItem.Rectangle ||
                    this.SelectedReportItemType == DrawingReportItem.TextBox)))
                {
                    this.itemEndPoint = e.GetPosition(null);
                    this.ProcessDrawItem = true;
                    IReportItemControl reportItem = this.AddReportItem();
                    EditAction editAction = new EditAction();
                    editAction.EditReportItems.Add(reportItem);
                    editAction.EditingType = EditActionType.ItemAdd;
                    this.EditingManager.AddAction(editAction);

                    this.EditingManager.IsMergeAction = true;
                    reportItem.RaiseReportItemSizeChangedEvent();
                    this.EditingManager.IsMergeAction = false;

                    reportItem.IsItemSelected = true;
                    this.SelectedReportItems.Add(reportItem);
                    this.SelectedReportItemType = DrawingReportItem.None;
                    this.RaiseReportItemDrawnEvent();



                    this.drawingCanvas = null;
                    e.Handled = true;
                }
            }
            else if (this.SelectedReportItemType == DrawingReportItem.None && this.drawingCanvas != null && !isSelectionkeypressed && isSelection)
            {
                this.selectionEndPoint = e.GetPosition(drawingCanvas);
                foreach (UIElement children in this.drawingCanvas.Children)
                {
                    if (children is IReportItemControl && IsInsideSelection(children))
                    {
                        IReportItemControl reportItemControl = children as IReportItemControl;
                        reportItemControl.IsItemSelected = true;
                        this.SelectedReportItems.Add(reportItemControl);
                    }
                }
                if (this.drawingCanvas is RectangleControl && this.selectionBox.Height > 0 && this.SelectedReportItems.Count > 1)
                {
                    IReportItemControl reportItemControl = this.drawingCanvas as RectangleControl;
                    reportItemControl.IsItemSelected = false;
                    this.SelectedReportItems.Remove(reportItemControl);
                }
                this.isSelection = false;
            }

            if (this.selectionBox != null)
            {
                this.DesignerArea.ReleaseMouseCapture();
                this.selectionBox.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        void DesignPanel_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Down || e.Key == Key.Up) && !((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift))
                {
                    EditAction action = new EditAction();
                    action.MovedReportItems = new List<MovingChange>();
                    action.EditingType = EditActionType.ItemMove;
                    this.EditingManager.AddAction(action);
                    this.EditingManager.IsMergeAction = true;

                    foreach (IReportItemControl item in this.SelectedReportItems)
                    {
                        if (!(item.IsFocusedItem && (item.ItemType == DrawingReportItem.TextBox || item.ItemType == DrawingReportItem.Tablix)))
                        {
                            item.IsItemSelected = (item.ItemType == DrawingReportItem.Line) ? false : true;
                            MovingChange change = new MovingChange();
                            change.ReportItem = item;
                            change.OldLeft = item.ItemLeft;
                            change.OldTop = item.ItemTop;
                            change.OldHeight = item.ItemHeight;
                            change.OldWidth = item.ItemWidth;
                            change.OldParent = item.Parent;

                            switch (e.Key)
                            {
                                case Key.Left:
                                    if (item.ItemType == DrawingReportItem.Line)
                                    {
                                        item.ItemLeft -= 5;
                                        item.ItemWidth -= 5;
                                    }
                                    else
                                    {
                                        item.ItemLeft -= 5;
                                    }
                                    break;
                                case Key.Right:
                                    if (item.ItemType == DrawingReportItem.Line)
                                    {
                                        item.ItemLeft += 5;
                                        item.ItemWidth += 5;
                                    }
                                    else
                                    {
                                        item.ItemLeft += 5;
                                    }
                                    break;
                                case Key.Up:
                                    if (item.ItemType == DrawingReportItem.Line)
                                    {
                                        item.ItemTop -= 5;
                                        item.ItemHeight -= 5;
                                    }
                                    else
                                    {
                                        item.ItemTop -= 5;
                                    }
                                    break;
                                case Key.Down:
                                    if (item.ItemType == DrawingReportItem.Line)
                                    {
                                        item.ItemTop += 5;
                                        item.ItemHeight += 5;
                                    }
                                    else
                                    {
                                        item.ItemTop += 5;
                                    }
                                    break;
                            }

                            item.IsItemSelected = true;
                            this.UpdateSize(item);
                            change.NewLeft = item.ItemLeft;
                            change.NewTop = item.ItemTop;
                            change.NewHeight = item.ItemHeight;
                            change.NewWidth = item.ItemWidth;
                            change.NewParent = item.Parent;
                            action.MovedReportItems.Add(change);
                        }
                        this.EditingManager.IsMergeAction = false;
                    }
                    }
                if (e.Key == Key.Delete)
                {
                    UIElement uielement = Keyboard.FocusedElement as UIElement;

                    if (uielement is RichTextBox)
                    {
                        return;
                    }
                    else
                    {
                        this.DeleteSelectedReportItems();
                    }
                }
                else if (e.Key == Key.Tab)
                {
                    if (this.SelectedReportItems.Count()>0)
                    {
                        int  next_item_pos,current_item_pos;
                        IReportItemControl element = this.SelectedReportItems.FirstOrDefault() as IReportItemControl;
                        current_item_pos = this.reportItems.IndexOf(element);
                        if (current_item_pos > 0)
                        {
                            this.reportItems[current_item_pos].IsItemSelected = false;
                            next_item_pos = (current_item_pos == this.reportItems.Count() - 1) ? 0 : current_item_pos + 1;
                            this.SelectedReportItems.Clear();
                            (this.reportItems[next_item_pos] as IReportItemControl).IsItemSelected = true;
                            this.SelectedReportItems.Add(this.reportItems[next_item_pos] as IReportItemControl);
                        }
                    }
                }
                else if (e.Key == Key.Escape)
                {
                    foreach (IReportItemControl element in this.SelectedReportItems)
                    {
                        if (element is IReportItemControl)
                        {
                            element.IsItemSelected = false;
                        }
                    }

                    this.SelectedReportItems.Clear();
                }

                if (e.Key == Key.LeftCtrl || e.Key == Key.LeftShift || e.Key == Key.RightCtrl || e.Key == Key.RightShift)
                {
                    this.isSelectionkeypressed = true;
                }

                else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.A)
                {
                    foreach (IReportItemControl element in this.SelectedReportItems)
                    {
                        if (element is IReportItemControl)
                        {
                            element.IsItemSelected = false;
                        }
                    }

                    this.SelectedReportItems.Clear();

                    foreach (UIElement children in this.bodyCanvas.Children)
                    {
                        if (children is IReportItemControl)
                        {
                            (children as IReportItemControl).IsItemSelected = true;
                            this.SelectedReportItems.Add(children as IReportItemControl);
                        }
                    }

                    foreach (UIElement children in this.headerCanvas.Children)
                    {
                        if (children is IReportItemControl)
                        {
                            (children as IReportItemControl).IsItemSelected = true;
                            this.SelectedReportItems.Add(children as IReportItemControl);
                        }
                    }

                    foreach (UIElement children in this.footerCanvas.Children)
                    {
                        if (children is IReportItemControl)
                        {
                            (children as IReportItemControl).IsItemSelected = true;
                            this.SelectedReportItems.Add(children as IReportItemControl);
                        }
                    }
                }
                else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.X)
                {
                    this.CutReportItems();
                }
                else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.C)
                {
                    this.CopyReportItems();
                }
                else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.V)
                {
                    this.PasteReportItems();
                }
                else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.Z)
                {
                    this.EditingManager.Undo();
                    this.RaiseReportItemSelectedEvent(this, new SelectedItemEventArgs() { SelectedItem = this.reportproperties });
                }
                else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.Y)
                {
                    this.EditingManager.Redo();
                    this.RaiseReportItemSelectedEvent(this, new SelectedItemEventArgs() { SelectedItem = this.reportproperties });
                }
                else if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                {
                     EditAction action = new EditAction();
                    action.MovedReportItems = new List<MovingChange>();
                    action.EditingType = EditActionType.ItemMove;
                    this.EditingManager.AddAction(action);
                    this.EditingManager.IsMergeAction = true;

                    foreach (IReportItemControl item in this.SelectedReportItems)
                    {
                        if (!(item.IsFocusedItem && (item.ItemType == DrawingReportItem.TextBox || item.ItemType == DrawingReportItem.Tablix)))
                        {
                            item.IsItemSelected = (item.ItemType == DrawingReportItem.Line) ? false : true;
                            MovingChange change = new MovingChange();
                            change.ReportItem = item;
                            change.OldLeft = item.ItemLeft;
                            change.OldTop = item.ItemTop;
                            change.OldHeight = item.ItemHeight;
                            change.OldWidth = item.ItemWidth;
                            change.OldParent = item.Parent;
                            switch (e.Key)
                            {
                                case Key.Up:
                                    if (item.ItemHeight > 7 && !(item.ItemType == DrawingReportItem.Line))
                                    {
                                        item.ItemHeight -= 5;
                                    }
                                    else if (item.ItemType == DrawingReportItem.Line)
                                    {
                                        item.ItemHeight -= 5;
                                        item.ItemWidth -= 5;
                                    }
                                     break;
                                case Key.Down:
                                    if(!(item.ItemType == DrawingReportItem.Line))
                                    {
                                    item.ItemHeight +=5;
                                    }
                                     else if (item.ItemType == DrawingReportItem.Line)
                                     {
                                        item.ItemHeight += 5;
                                        item.ItemWidth += 5;
                                     }
                                    break;
                                case Key.Left:
                                    if (item.ItemWidth > 7 && !(item.ItemType == DrawingReportItem.Line))
                                    {
                                        item.ItemWidth -= 5;
                                    }
                                    else if (item.ItemType == DrawingReportItem.Line)
                                    {
                                        item.ItemHeight += 5;
                                        item.ItemWidth -= 5;
                                    }
                                    break;
                                case Key.Right:
                                    if (!(item.ItemType == DrawingReportItem.Line))
                                    {
                                        item.ItemWidth += 5;
                                    }
                                    else if (item.ItemType == DrawingReportItem.Line)
                                    {
                                        item.ItemHeight -= 5;
                                        item.ItemWidth += 5;
                                    }
                                    break;
                            }

                            item.IsItemSelected = true;
                            this.UpdateSize(item);
                            change.NewLeft = item.ItemLeft;
                            change.NewTop = item.ItemTop;
                            change.NewHeight = item.ItemHeight;
                            change.NewWidth = item.ItemWidth;
                            change.NewParent = item.Parent;
                            action.MovedReportItems.Add(change);
                        }
                        this.EditingManager.IsMergeAction = false;
                    }
                }
        }

        void DesignPanel_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.LeftShift || e.Key == Key.RightCtrl || e.Key == Key.RightShift)
            {
                this.isSelectionkeypressed = false;
            }
        }

        public bool IsInsideSelection(UIElement element)
        {
            double selcRectLeft = Canvas.GetLeft(selectionBox);
            double selcRectTop = Canvas.GetTop(selectionBox);
            double selcRectRight = selcRectLeft + selectionBox.Width;
            double selcRectBottom = selcRectTop + selectionBox.Height;

            double top = (element as IReportItemControl).ItemTop;
            double left = (element as IReportItemControl).ItemLeft;
            double right = (element as IReportItemControl).ItemWidth + left;
            double bottom = (element as IReportItemControl).ItemHeight + top;
            return (((selcRectLeft < left && selcRectTop < top && selcRectRight > left && selcRectBottom > top))
                || (selcRectRight > left && selcRectRight < right && (selcRectBottom > top || selcRectTop > top) && (selcRectBottom < bottom || selcRectTop < bottom)
                || (selcRectTop > top && selcRectTop < bottom && selcRectRight > right && selcRectLeft < left)
                || (((selcRectTop > top || selcRectBottom > top) && (selcRectTop < bottom || selcRectBottom < bottom) && selcRectLeft > left && selcRectLeft < right))));
        }


        void DesignArea_PreviewDrop(object sender, DragEventArgs e)
        {
            try
            {
                TreeObjectCollection itemCollection = e.Data.GetData(typeof(TreeObjectCollection)) as TreeObjectCollection;
                TreeViewItemAdv treeViewItem = itemCollection[0] as TreeViewItemAdv;

                bool isHeader = Object.ReferenceEquals(this.drawingCanvas, this.headerCanvas);
                bool isFooter = Object.ReferenceEquals(this.drawingCanvas, this.footerCanvas);
                bool isBody = Object.ReferenceEquals(this.drawingCanvas, this.bodyCanvas);

                if (e.Source is RectangleControl)
                {
                    this.drawingCanvas = e.Source as Canvas;
                }

                Point mousePoint = e.GetPosition(this.drawingCanvas);

                if (treeViewItem.Tag != null && (e.Source is TablixControl == false && e.Source is ChartControl == false && e.Source is GaugeControl == false && e.Source is TextBoxControl == false && e.Source is ImageControl == false))
                {
                    if (treeViewItem.Tag is string)
                    {
                        RDL.DOM.TextBox textbox = new RDL.DOM.TextBox();
                        textbox.Style = new Base.Style();
                        textbox.Style.Color="Black";
                        textbox.Left = mousePoint.X / 96 + "in";
                        textbox.Top = mousePoint.Y / 96 + "in";
                        textbox.Width = 1 + "in";
                        textbox.Height = 0.25 + "in";
                        textbox.Paragraphs = new Paragraphs();
                        TextRun textrun = new TextRun();
                        Syncfusion.RDL.DOM.Paragraph paragraph = new Base.Paragraph();
                        paragraph.TextRuns = new TextRuns();
                        if (treeViewItem.Tag.ToString() == "Parameters" || treeViewItem.Tag.ToString() == "Parameter")
                        {
                            string chkstring = treeViewItem.ToString();
                            textbox.Name = chkstring;

                            textrun.Value = "[@" + chkstring + "]";
                            textrun.Style = new Base.Style();
                            textrun.Style.Color = "Black";
                            paragraph.TextRuns.Add(textrun);
                            textbox.Paragraphs.Add(paragraph);
                            IReportItemControl reportItem = this.AddReportItem(textbox);
                            EditAction editAction = new EditAction();
                            editAction.EditReportItems.Add(reportItem);
                            editAction.EditingType = EditActionType.ItemAdd;
                            this.EditingManager.AddAction(editAction);

                            this.EditingManager.IsMergeAction = true;
                            reportItem.RaiseReportItemSizeChangedEvent();
                            this.EditingManager.IsMergeAction = false;
                        }

                        else if (treeViewItem.Tag.ToString().StartsWith("#BuiltIn#"))
                        {
                            string CheckString = treeViewItem.Tag.ToString().Replace("#BuiltIn#", "");
                            if (((isHeader || isFooter) &&
                                (CheckString == "PageNumber" ||
                                CheckString == "TotalPages" ||
                                CheckString == "ExecutionTime" ||
                                CheckString == "ReportFolder" ||
                                CheckString == "ReportName" ||
                                CheckString == "ReportServerUrl" ||
                                CheckString == "UserID" ||
                                CheckString == "Language")) ||
                                (isBody &&
                                (CheckString == "ExecutionTime" ||
                                CheckString == "ReportFolder" ||
                                CheckString == "ReportName" ||
                                CheckString == "ReportServerUrl" ||
                                CheckString == "UserID" ||
                                CheckString == "Language")))
                            {
                                textbox.Name = CheckString;
                                textrun.Value = "[&" + CheckString + "]";
                                textrun.Style = new Base.Style();
                                textrun.Style.Color = "Black";
                                paragraph.TextRuns.Add(textrun);
                                textbox.Paragraphs.Add(paragraph);
                                IReportItemControl reportItem = this.AddReportItem(textbox);
                                EditAction editAction = new EditAction();
                                editAction.EditReportItems.Add(reportItem);
                                editAction.EditingType = EditActionType.ItemAdd;
                                this.EditingManager.AddAction(editAction);

                                this.EditingManager.IsMergeAction = true;
                                reportItem.RaiseReportItemSizeChangedEvent();
                                this.EditingManager.IsMergeAction = false;
                            }
                        }
                        else
                        {
                            var DataSet = (from dataSet in this.DataSets
                                           where dataSet.Name == treeViewItem.Tag.ToString()
                                           select dataSet).SingleOrDefault();

                            textbox.Name = "TextBox" + ++textBoxCount;

                            if (this.GetParent(drawingCanvas) is CellContentsControl)
                            {
                                TablixControl parent = (this.GetParent(drawingCanvas) as CellContentsControl).Parent as TablixControl;

                                if (parent.ReportDataSetName == null || (parent.ReportDataSetName != null && parent.ReportDataSetName == string.Empty) || parent.ReportDataSetName == DataSet.Name)
                                {
                                    if (drawingCanvas.Parent is CellContentsControl)
                                    {
                                        textrun.Value = "=Fields!" + treeViewItem.Header.ToString() + ".Value";
                                        textrun.Label = "[" + treeViewItem.Header.ToString() + "]";
                                        textrun.Style = new Base.Style();
                                        textrun.Style.Color = "Black";
                                    }
                                    else
                                    {
                                        textrun.Value = "=First(Fields!" + treeViewItem.Header.ToString() + ".Value,\"" + DataSet.Name + "\")";
                                        textrun.Style = new Base.Style();
                                        textrun.Style.Color = "Black";
                                    }
                                }
                                else
                                {
                                    return;
                                }
                            }

                            else
                            {
                                textrun.Value = "=First(Fields!" + treeViewItem.Header.ToString() + ".Value,\"" + DataSet.Name + "\")";
                            }

                            paragraph.TextRuns.Add(textrun);
                            textbox.Paragraphs.Add(paragraph);

                            IReportItemControl reportItem = this.AddReportItem(textbox);
                            EditAction editAction = new EditAction();
                            editAction.EditReportItems.Add(reportItem);
                            editAction.EditingType = EditActionType.ItemAdd;
                            this.EditingManager.AddAction(editAction);

                            this.EditingManager.IsMergeAction = true;
                            reportItem.RaiseReportItemSizeChangedEvent();
                            this.EditingManager.IsMergeAction = false;
                        }
                    }
                    else if (treeViewItem.Tag is RDL.DOM.EmbeddedImage)
                    {
                        var EmbeddedImage = (from embeddedImage in this.EmbeddedImages
                                             where embeddedImage.Name == treeViewItem.Header.ToString()
                                             select embeddedImage).SingleOrDefault();
                        RDL.DOM.Image image = new Base.Image();
                        image.Name = "Image" + ++imageCount;
                        image.Value = EmbeddedImage.Name;
                        image.Source = Source.Embedded;
                        image.Sizing = Sizing.FitProportional;
                        Base64ImageConverter imageConverter = new Base64ImageConverter();

                        if (EmbeddedImage.MIMEType != "image/emf")
                        {
                            System.Windows.Media.Imaging.BitmapImage bitmapImage = imageConverter.ConvertToImage(EmbeddedImage.ImageData);
                            image.Height = bitmapImage.Height / 96 + "in";
                            image.Width = bitmapImage.Width / 96 + "in";
                        }
                        else
                        {
                            byte[] data = Convert.FromBase64String(EmbeddedImage.ImageData);
                            using (Stream stream = new MemoryStream(data))
                            {
                                System.Windows.Forms.PictureBox pictureBox = new System.Windows.Forms.PictureBox();
                                pictureBox.Image = new System.Drawing.Imaging.Metafile(stream);
                                image.Height = pictureBox.Image.Height / 96 + "in";
                                image.Width = pictureBox.Image.Width / 96 + "in";
                            }
                        }

                        image.Left = mousePoint.X / 96 + "in";
                        image.Top = mousePoint.Y / 96 + "in";

                        IReportItemControl reportItem = this.AddReportItem(image);
                        EditAction editAction = new EditAction();
                        editAction.EditReportItems.Add(reportItem);
                        editAction.EditingType = EditActionType.ItemAdd;
                        this.EditingManager.AddAction(editAction);

                        this.EditingManager.IsMergeAction = true;
                        reportItem.RaiseReportItemSizeChangedEvent();
                        this.EditingManager.IsMergeAction = false;
                    }

                }
            }
            catch (Exception)
            {
                //MessageBox.Show("Unexpected Error occurs");
            }

            this.drawingCanvas = null;
        }

        CellContentsControl GetParent(Canvas reportControl)
        {
            if (reportControl.Parent is Canvas && !(reportControl.Parent is IReportItemControl))
            {
                return null;
            }

            else if (reportControl.Parent is CellContentsControl)
            {
                return reportControl.Parent as CellContentsControl;
            }

            else if (reportControl.Parent is IReportItemControl)
            {
                return GetParent(reportControl.Parent as Canvas);
            }

            return null;
        }


        void DesignPanel_DragOver(object sender, DragEventArgs e)
        {
            this.drawingCanvas = sender as Canvas;
        }

        void FooterGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            RulerChipHide();
        }

        void BodyGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            RulerChipHide();
        }

        void HeaderGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            RulerChipHide();
        }

        void HeaderGrid_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(sender as Grid);
            RulerHorizontal.Chip = RulerHorizontal.Unit == Unit.Cm ? DipHelper.DipToCm(p.X) : DipHelper.DipToInch(p.X);
            RulerVerticalHeader.Chip = RulerVerticalHeader.Unit == Unit.Cm ? DipHelper.DipToCm(p.Y) : DipHelper.DipToInch(p.Y);
        }

        void BodyGrid_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(sender as Grid);
            RulerHorizontal.Chip = RulerHorizontal.Unit == Unit.Cm ? DipHelper.DipToCm(p.X) : DipHelper.DipToInch(p.X);
            RulerVerticalBody.Chip = RulerVerticalBody.Unit == Unit.Cm ? DipHelper.DipToCm(p.Y) : DipHelper.DipToInch(p.Y);
        }

        void FooterGrid_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(sender as Grid);
            RulerHorizontal.Chip = RulerHorizontal.Unit == Unit.Cm ? DipHelper.DipToCm(p.X) : DipHelper.DipToInch(p.X);
            RulerVerticalFooter.Chip = RulerVerticalFooter.Unit == Unit.Cm ? DipHelper.DipToCm(p.Y) : DipHelper.DipToInch(p.Y);
        }

        #endregion

        #region MenuItem events

        private void MnuItemInsertPageFooter_Click(object sender, RoutedEventArgs e)
        {
            SetFooterVisibility();
        }

        private void MenuRemoveFooter_Click(object sender, RoutedEventArgs e)
        {
            SetFooterVisibility();
            reportDesignView.UpdateHeaderFooterTab();
        }

        private void MnuItemPageHeader_Click(object sender, RoutedEventArgs e)
        {
            SetHeaderVisibility();
        }

        private void MenuRemoveHeader_Click(object sender, RoutedEventArgs e)
        {
            SetHeaderVisibility();
            reportDesignView.UpdateHeaderFooterTab();
        }

        #endregion

        #region Helper Methods

        private void WireEvents()
        {
            this.MenuItemHeaderRemoveHeader.Click += new RoutedEventHandler(MenuRemoveHeader_Click);
            this.MenuItemFooterRemoveFooter.Click += new RoutedEventHandler(MenuRemoveFooter_Click);

            this.DesignerArea.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(DesignerArea_PreviewMouseLeftButtonDown);
            this.DesignerArea.MouseRightButtonDown += new MouseButtonEventHandler(DesignerArea_MouseRightButtonDown);
           
            this.DesignerArea.MouseMove += new MouseEventHandler(DesignerArea_MouseMove);
            this.DesignerArea.MouseLeftButtonUp += new MouseButtonEventHandler(DesignerArea_MouseLeftButtonUp);

            this.DesignerArea.PreviewDrop += new DragEventHandler(DesignArea_PreviewDrop);
            this.PreviewKeyDown += new KeyEventHandler(DesignPanel_PreviewKeyDown);
            this.PreviewKeyUp += new KeyEventHandler(DesignPanel_PreviewKeyUp);

            this.HeaderGrid.PreviewMouseMove += new MouseEventHandler(HeaderGrid_PreviewMouseMove);
            this.HeaderGrid.MouseLeave += new MouseEventHandler(HeaderGrid_MouseLeave);
            this.BodyGrid.PreviewMouseMove += new MouseEventHandler(BodyGrid_PreviewMouseMove);
            this.BodyGrid.MouseLeave += new MouseEventHandler(BodyGrid_MouseLeave);
            this.FooterGrid.PreviewMouseMove += new MouseEventHandler(FooterGrid_PreviewMouseMove);
            this.FooterGrid.MouseLeave += new MouseEventHandler(FooterGrid_MouseLeave);
            this.footerCanvas.DragOver += new DragEventHandler(DesignPanel_DragOver);
            this.bodyCanvas.DragOver += new DragEventHandler(DesignPanel_DragOver);
            this.headerCanvas.DragOver += new DragEventHandler(DesignPanel_DragOver);
            this.footerCanvas.DragLeave += new DragEventHandler(DesignerArea_DragLeave);
            this.bodyCanvas.DragLeave += new DragEventHandler(DesignerArea_DragLeave);
            this.headerCanvas.DragLeave += new DragEventHandler(DesignerArea_DragLeave);

            this.gridSplitterBody.DragStarted += new DragStartedEventHandler(gridSplitterBody_DragStarted);
            this.gridSplitterBody.DragDelta += new DragDeltaEventHandler(gridSplitterBody_DragDelta);
            this.gridSplitterBody.DragCompleted += new DragCompletedEventHandler(gridSplitterBody_DragCompleted);

            this.gridSplitterFooter.DragStarted += new DragStartedEventHandler(gridSplitterFooter_DragStarted);
            this.gridSplitterFooter.DragDelta += new DragDeltaEventHandler(gridSplitterFooter_DragDelta);
            this.gridSplitterFooter.DragCompleted += new DragCompletedEventHandler(gridSplitterFooter_DragCompleted);

            this.gridSplitterHeader.DragStarted += new DragStartedEventHandler(gridSplitterHeader_DragStarted);
            this.gridSplitterHeader.DragDelta += new DragDeltaEventHandler(gridSplitterHeader_DragDelta);
            this.gridSplitterHeader.DragCompleted += new DragCompletedEventHandler(gridSplitterHeader_DragCompleted);

            this.gridSplitterReport.DragStarted += new DragStartedEventHandler(gridSplitterReport_DragStarted);
            this.gridSplitterReport.DragDelta += new DragDeltaEventHandler(gridSplitterReport_DragDelta);
            this.gridSplitterReport.DragCompleted += new DragCompletedEventHandler(gridSplitterReport_DragCompleted);
        }

        void DesignerArea_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            UIElement sourceControl = e.Source as UIElement;

            if (!(sourceControl is GridSplitter) && !(sourceControl is IReportItemControl) &&
                this.SelectedReportItemType == DrawingReportItem.None && !(sourceControl is Canvas)
                && (sourceControl is Grid))
            {
                this.ClearSelection();
                ContextMenu contextMenu = new ContextMenu();
                MenuItem reportPropties = new MenuItem();
                reportPropties.Header = "Report Properties";
                contextMenu.Items.Add(reportPropties);
                reportPropties.Click += new RoutedEventHandler(ReportPropties_Click);
                (sourceControl as Grid).ContextMenu = contextMenu;
            }
            else if(object.ReferenceEquals(sourceControl, bodyCanvas))
            {
                ContextMenu contextMenu = new ContextMenu();
                MenuItem BodyProperties = new MenuItem();
                BodyProperties.Header = "Body Properties";
                contextMenu.Visibility = System.Windows.Visibility.Collapsed;
                contextMenu.Items.Add(BodyProperties);
                (sourceControl as Canvas).ContextMenu = contextMenu;
            }
        }

        void ReportPropties_Click(object sender, RoutedEventArgs e)
        {
            ControlProperties.UnitType = this.ReportUnitType;
            ControlProperties controlProperty = new ControlProperties(this.Report);
            this.UpdateOwnerWindow(controlProperty);

            Base.Page page = null;

            if (controlProperty.ShowDialog() == true)
            {
                if (this.RDLType == Base.RDLType.RDL2010)
                {
                    this.Report.ReportSections.First().Page = controlProperty.Report.ReportSections.First().Page;
                    page = this.Report.ReportSections.First().Page;
                }
                else
                {
                    this.Report.Page = controlProperty.Report.Page;
                    page = this.Report.Page;
                }


                this.Report.Code = controlProperty.Report.Code;
                this.Report.CodeModules = controlProperty.Report.CodeModules;
                this.Report.Classes = controlProperty.Report.Classes;
                this.Report.Variables = controlProperty.Report.Variables;
                this.ReportUnitType = ControlProperties.UnitType;

                if (ControlProperties.UnitType == RDL.DOM.ReportUnitType.Cm)
                {
                    this.RulerHorizontal.Unit = Unit.Cm;
                    this.RulerVerticalBody.Unit = Unit.Cm;
                    this.RulerVerticalHeader.Unit = Unit.Cm;
                    this.RulerVerticalFooter.Unit = Unit.Cm;
                }
                else
                {
                    this.RulerHorizontal.Unit = Unit.Inch;
                    this.RulerVerticalBody.Unit = Unit.Inch;
                    this.RulerVerticalFooter.Unit = Unit.Inch;
                    this.RulerVerticalHeader.Unit = Unit.Inch;
                }

                this.reportproperties.PageHeight = page.PageHeight.size;
                this.reportproperties.PageWidth = page.PageWidth.size;
                this.reportproperties.Margins.LeftMargin = page.LeftMargin.size;
                this.reportproperties.Margins.TopMargin = page.TopMargin.size;
                this.reportproperties.Margins.RightMargin = page.RightMargin.size;
                this.reportproperties.Margins.BottomMargin = page.BottomMargin.size;
                var size = new RDL.DOM.Size(this.bodyproperties.ReportWidth);
                this.bodyproperties.ReportWidth = GetConvertedValue(size.FloatValue, size.MeasurementUnit, ControlProperties.UnitType);
                size = new RDL.DOM.Size(this.bodyproperties.BodyHeight);
                this.bodyproperties.BodyHeight = GetConvertedValue(size.FloatValue, size.MeasurementUnit, ControlProperties.UnitType);
                size = new RDL.DOM.Size(this.headerproperties.HeaderHeight);
                this.headerproperties.HeaderHeight = GetConvertedValue(size.FloatValue, size.MeasurementUnit, ControlProperties.UnitType);
                size = new RDL.DOM.Size(this.footerproperties.FooterHeight);
                this.footerproperties.FooterHeight = GetConvertedValue(size.FloatValue, size.MeasurementUnit, ControlProperties.UnitType);
            }
        }

        void DesignerArea_DragLeave(object sender, DragEventArgs e)
        {
            this.drawingCanvas = null;
        }

        void gridSplitterReport_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.dragOldSize = this.bodyproperties.ReportWidth;
        }

        void gridSplitterReport_DragDelta(object sender, DragDeltaEventArgs e)
        {
            this.IsInternalChange = true;
            this.propertyValue = (this.reportWidthColumnDefinition.Width.Value / 96) + GetMeasuredUnit(this.bodyproperties.ReportWidth);
            this.bodyproperties.ReportWidth = converter.GetSizeValue(this.reportWidthColumnDefinition.Width.Value, this.propertyValue as string);
            this.IsInternalChange = false;
        }

        void gridSplitterReport_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (!IsInternalChange)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemPropertyChanged;
                PropertyChanage change = new PropertyChanage();
                change.PropertyObject = this.bodyproperties;
                change.PropertyName = "ReportWidth";
                change.OldValue = this.dragOldSize;
                change.NewValue = this.bodyproperties.ReportWidth;
                action.PropertyChange = change;
                this.EditingManager.AddAction(action);
            }
        }

        void gridSplitterHeader_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.dragOldSize = this.headerproperties.HeaderHeight;
        }

        void gridSplitterHeader_DragDelta(object sender, DragDeltaEventArgs e)
        {
            this.IsInternalChange = true;
            this.propertyValue = (this.headerRowDefination.Height.Value / 96) + GetMeasuredUnit(this.headerproperties.HeaderHeight);
            this.headerproperties.HeaderHeight = converter.GetSizeValue(this.headerRowDefination.Height.Value, this.propertyValue as string);
            this.IsInternalChange = false;
        }

        void gridSplitterHeader_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (!IsInternalChange)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemPropertyChanged;
                PropertyChanage change = new PropertyChanage();
                change.PropertyObject = this.headerproperties;
                change.PropertyName = "HeaderHeight";
                change.OldValue = this.dragOldSize;
                change.NewValue = this.headerproperties.HeaderHeight;
                action.PropertyChange = change;
                this.EditingManager.AddAction(action);
            }
        }

        void gridSplitterFooter_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.dragOldSize = this.footerproperties.FooterHeight;
        }

        void gridSplitterFooter_DragDelta(object sender, DragDeltaEventArgs e)
        {
            this.IsInternalChange = true;
            this.propertyValue = (this.footerRowDefination.Height.Value / 96) + GetMeasuredUnit(this.headerproperties.HeaderHeight);
            this.footerproperties.FooterHeight = converter.GetSizeValue(this.footerRowDefination.Height.Value, this.propertyValue as string);
            this.IsInternalChange = false;
        }

        void gridSplitterFooter_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (!IsInternalChange)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemPropertyChanged;
                PropertyChanage change = new PropertyChanage();
                change.PropertyObject = this.footerproperties;
                change.PropertyName = "FooterHeight";
                change.OldValue = this.dragOldSize;
                change.NewValue = this.footerproperties.FooterHeight;
                action.PropertyChange = change;
                this.EditingManager.AddAction(action);
            }
        }

        void gridSplitterBody_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.dragOldSize = this.bodyproperties.BodyHeight;
        }

        void gridSplitterBody_DragDelta(object sender, DragDeltaEventArgs e)
        {
            this.IsInternalChange = true;
            this.propertyValue = (this.bodyRowDefination.Height.Value / 96) + GetMeasuredUnit(this.headerproperties.HeaderHeight);
            this.bodyproperties.BodyHeight = converter.GetSizeValue(this.bodyRowDefination.Height.Value, this.propertyValue as string);
            this.IsInternalChange = false;
        }

        void gridSplitterBody_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (!IsInternalChange)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemPropertyChanged;
                PropertyChanage change = new PropertyChanage();
                change.PropertyObject = this.bodyproperties;
                change.PropertyName = "BodyHeight";
                change.OldValue = this.dragOldSize;
                change.NewValue = this.bodyproperties.BodyHeight;
                action.PropertyChange = change;
                this.EditingManager.AddAction(action);
            }
        }

        internal void UpdateOwnerWindow(Window window)
        {
            this.reportDesignView.UpdateOwnerWindow(window);
        }

        void ResetItemNumbers()
        {
            textBoxCount = 0;
            tablixCount = 0;
            imageCount = 0;
            rectangleCount = 0;
            subreportCount = 0;
            lineCount = 0;
            chartCount = 0;
            gaugeCount = 0;
        }

        public Canvas GetDrawingCanvas()
        {
            HitTestResult result = VisualTreeHelper.HitTest(bodyCanvas, Mouse.GetPosition(bodyCanvas));

            if (result == null)
            {
                result = VisualTreeHelper.HitTest(headerCanvas, Mouse.GetPosition(headerCanvas));

                if (result == null)
                {
                    result = VisualTreeHelper.HitTest(footerCanvas, Mouse.GetPosition(footerCanvas));

                    if (result != null)
                    {
                        return this.footerCanvas;
                    }
                }
                else
                {
                    return this.headerCanvas;
                }
            }
            else
            {
                return this.bodyCanvas;
            }

            return null;
        }

        internal void UpdateBackgroundImage(string canvasName)
        {
            if (!string.IsNullOrEmpty(canvasName) && this.EmbeddedImages.Count > 0)
            {
                ImageBrush brush;
                if (canvasName.Equals("Body"))
                {
                    if (this.bodyproperties.BackgroundImage.Source == RDL.DOM.Source.Embedded &&
                        !string.IsNullOrEmpty(this.bodyproperties.BackgroundImage.ImageValue))
                    {
                        brush = GetImageBrush(this.bodyproperties.BackgroundImage.ImageValue);
                        if (brush != null)
                        {
                            this.bodyCanvas.Background = brush;
                        }
                        else
                        {
                            this.bodyCanvas.Background = converter.GetBackGroundColor(this.bodyproperties.BackgroundColor);
                        }
                    }
                    else
                    {
                        this.bodyCanvas.Background = converter.GetBackGroundColor(this.bodyproperties.BackgroundColor);
                    }
                }
                else if (canvasName.Equals("Page Footer"))
                {
                    if (this.footerproperties.BackgroundImage.Source == RDL.DOM.Source.Embedded &&
                        !string.IsNullOrEmpty(this.footerproperties.BackgroundImage.ImageValue))
                    {
                        brush = GetImageBrush(this.footerproperties.BackgroundImage.ImageValue);
                        if (brush != null)
                        {
                            this.footerCanvas.Background = brush;
                        }
                        else
                        {
                            this.footerCanvas.Background = converter.GetBackGroundColor(this.footerproperties.BackgroundColor);
                        }
                    }
                    else
                    {
                        this.footerCanvas.Background = converter.GetBackGroundColor(this.footerproperties.BackgroundColor);
                    }
                }
                else if (canvasName.Equals("Page Header"))
                {
                    if (this.headerproperties.BackgroundImage.Source == RDL.DOM.Source.Embedded &&
                        !string.IsNullOrEmpty(this.headerproperties.BackgroundImage.ImageValue))
                    {
                        brush = GetImageBrush(this.headerproperties.BackgroundImage.ImageValue);
                        if (brush != null)
                        {
                            this.headerCanvas.Background = brush;
                        }
                        else
                        {
                            this.headerCanvas.Background = converter.GetBackGroundColor(this.headerproperties.BackgroundColor);
                        }
                    }
                    else
                    {
                        this.headerCanvas.Background = converter.GetBackGroundColor(this.headerproperties.BackgroundColor);
                    }
                }
            }
        }

        private ImageBrush GetImageBrush(string imageName)
        {
            RDL.DOM.EmbeddedImage embededimage = (from image in this.EmbeddedImages
                                              where image.Name.Equals(imageName)
                                              select image).FirstOrDefault();            
            
            if (embededimage != null && embededimage.MIMEType != "image/emf")
            {
                Base64ImageConverter imageConverter = new Base64ImageConverter();
                System.Windows.Media.Imaging.BitmapImage bitmapImage = imageConverter.ConvertToImage(embededimage.ImageData);
                ImageBrush brush = new ImageBrush(bitmapImage);
                return brush;
            }

            return null;
        }

        public void UpdateDrawingCanvas()
        {
            this.drawingCanvas = this.GetDrawingCanvas();
            this.selectionBox = null;

            if (drawingCanvas != null)
            {
                if (Object.ReferenceEquals(drawingCanvas, bodyCanvas))
                {
                    this.selectionBox = this.bodySelectionBox;
                }
                else if (Object.ReferenceEquals(drawingCanvas, headerCanvas))
                {
                    this.selectionBox = this.headerSelectionBox;
                }
                else if (Object.ReferenceEquals(drawingCanvas, footerCanvas))
                {
                    this.selectionBox = this.footerSelectionBox;
                }
            }
        }

        public IReportItemControl AddReportItem()
        {
            return this.AddReportItem(null);
        }

        internal IReportItemControl GetReportItem(DrawingReportItem type, ReportItem reportItem)
        {
            IReportItemControl reportControl = null;
            //this.GetReportItem(ReportItemControlType.TextBox, null);

            switch (type)
            {
                case DrawingReportItem.TextBox:
                    {
                        TextBoxControl textBox = new TextBoxControl();
                        reportControl = textBox;

                        if (reportItem == null)
                        {
                            do
                            {
                                textBox.ItemName = "TextBox" + ++textBoxCount;
                            } while (NameAvailabilityCheck(textBox.ItemName)) ;
                        }
                        else
                        {
                            while (NameAvailabilityCheck(reportItem.Name))
                            {
                                reportItem.Name = "TextBox" + ++textBoxCount;
                            }

                            textBox.ItemName = reportItem.Name;
                            textBox.ReportItem = reportItem;
                        }
                        break;
                    }
                case DrawingReportItem.Line:
                    {
                        LineControl line = new LineControl();
                        reportControl = line;

                        if (reportItem == null)
                        {
                            do
                            {
                                line.ItemName = "Line" + ++lineCount;
                            } while (NameAvailabilityCheck(line.ItemName)) ;
                        }
                        else
                        {
                            while (NameAvailabilityCheck(reportItem.Name))
                            {
                                reportItem.Name = "Line" + ++lineCount;
                            }

                            line.ItemName = reportItem.Name;
                            line.ReportItem = reportItem;
                        }
                        break;
                    }
                case DrawingReportItem.Chart:
                    {
                        ChartControl chart = new ChartControl(this.DataSets, this.DataSources, ChartControlType.Chart);
                        reportControl = chart;

                        if (reportItem == null)
                        {
                            do
                            {
                                chart.ItemName = "Chart" + ++chartCount;
                            } while (NameAvailabilityCheck(chart.ItemName)) ;
                        }
                        else
                        {
                            while (NameAvailabilityCheck(reportItem.Name))
                            {
                                reportItem.Name = "Chart" + ++chartCount;
                            }

                            chart.ItemName = reportItem.Name;
                            chart.ReportItem = reportItem;
                        }
                        break;
                    }
                case DrawingReportItem.DataBar:
                    {
                        ChartControl chart = new ChartControl(this.DataSets, this.DataSources, ChartControlType.DataBar);
                        reportControl = chart;

                        if (reportItem == null)
                        {
                            do
                            {
                                chart.ItemName = "DataBar" + ++chartCount;
                            } while (NameAvailabilityCheck(chart.ItemName));
                        }
                        else
                        {
                            while (NameAvailabilityCheck(reportItem.Name))
                            {
                                reportItem.Name = "DataBar" + ++chartCount;
                            }

                            chart.ItemName = reportItem.Name;
                            chart.ReportItem = reportItem;
                        }
                        break;
                    }
                case DrawingReportItem.Map:
                    {
#if !SyncfusionFramework3_5
                        MapControl map = new MapControl(this.DataSets, this.DataSources); 
                        reportControl = map;

                        if (reportItem == null)
                        {
                            do
                            {
                                map.ItemName = "Map" + ++mapCount;
                            } while (NameAvailabilityCheck(map.ItemName));
                        }
                        else
                        {
                            while (NameAvailabilityCheck(reportItem.Name))
                            {
                                reportItem.Name = "Map" + ++mapCount;
                            }

                            map.ItemName = reportItem.Name;
                            map.ReportItem = reportItem;
                        }
    #endif
                        break;
                    }
                case DrawingReportItem.Sparkline:
                    {
                        ChartControl chart = new ChartControl(this.DataSets, this.DataSources, ChartControlType.Sparkline);
                        reportControl = chart;

                        if (reportItem == null)
                        {
                            do
                            {
                                chart.ItemName = "Sparkline" + ++chartCount;
                            } while (NameAvailabilityCheck(chart.ItemName));
                        }
                        else
                        {
                            while (NameAvailabilityCheck(reportItem.Name))
                            {
                                reportItem.Name = "Sparkline" + ++chartCount;
                            }

                            chart.ItemName = reportItem.Name;
                            chart.ReportItem = reportItem;
                        }
                        break;
                    }
                case DrawingReportItem.Image:
                    {
                        ImageControl image = new ImageControl(this.EmbeddedImages);
                        reportControl = image;

                        if (reportItem == null)
                        {
                            do
                            {
                                image.ItemName = "Image" + ++imageCount;
                            } while (NameAvailabilityCheck(image.ItemName)) ;
                        }
                        else
                        {
                            while (NameAvailabilityCheck(reportItem.Name))
                            {
                                reportItem.Name = "Image" + ++imageCount;
                            }

                            image.ItemName = reportItem.Name;
                            image.ReportItem = reportItem;
                        }
                        break;
                    }
                case DrawingReportItem.Gauge:
                    {
                        GaugeControl gauge = new GaugeControl(this.DataSets, this.DataSources);
                        reportControl = gauge;

                        if (reportItem == null)
                        {
                            do
                            {
                                gauge.ItemName = "Gauge" + ++gaugeCount;
                            } while (NameAvailabilityCheck(gauge.ItemName)) ;
                        }
                        else
                        {
                            while (NameAvailabilityCheck(reportItem.Name))
                            {
                                reportItem.Name = "Gauge" + ++gaugeCount;
                            }

                            gauge.ItemName = reportItem.Name;
                            gauge.ReportItem = reportItem;
                        }
                        break;
                    }
                case DrawingReportItem.Rectangle:
                    {
                        RectangleControl rect = new RectangleControl(reportItem);
                        reportControl = rect;

                        if (reportItem == null)
                        {
                            do
                            {
                                rect.ItemName = "Rectangle" + ++rectangleCount;
                            } while (NameAvailabilityCheck(rect.ItemName)) ;
                        }
                        else
                        {
                            while (NameAvailabilityCheck(reportItem.Name))
                            {
                                reportItem.Name = "Rectangle" + ++rectangleCount;
                            }

                            rect.ItemName = reportItem.Name;
                            rect.ReportItem = reportItem;
                        }
                        break;
                    }
                case DrawingReportItem.Tablix:
                    {
                        TablixControl tablix = new TablixControl(this.DataSets, reportItem);
                        reportControl = tablix;

                        if (reportItem == null)
                        {
                            do
                            {
                                tablix.ItemName = "Tablix" + ++tablixCount;
                            } while (NameAvailabilityCheck(tablix.ItemName)) ;
                        }
                        else
                        {
                            while (NameAvailabilityCheck(reportItem.Name))
                            {
                                reportItem.Name = "Tablix" + ++tablixCount;
                            }

                            tablix.ItemName = reportItem.Name;
                            tablix.ReportItem = reportItem;
                        }
                        break;
                    }
                case DrawingReportItem.SubReport:
                    {
                        SubReportControl subreport = new SubReportControl();
                        reportControl = subreport;
                        if (reportItem == null)
                        {
                            do
                            {
                                subreport.ItemName = "Subreport" + ++subreportCount;
                            } while (NameAvailabilityCheck(subreport.ItemName)) ;
                        }
                        else
                        {
                            while (NameAvailabilityCheck(reportItem.Name))
                            {
                                reportItem.Name = "Subreport" + ++subreportCount;
                            }

                            subreport.ItemName = reportItem.Name;
                            subreport.ReportItem = reportItem;
                        }

                        break;
                    }
                case DrawingReportItem.List:
                    {
                        TablixControl tablix = new TablixControl(this.DataSets);
                        reportControl = tablix;
                        if (reportItem == null)
                        {
                            do
                            {
                                tablix.ItemName = "Tablix" + ++tablixCount;
                            } while (NameAvailabilityCheck(tablix.ItemName)) ;
                        }
                        break;
                    }
            }

            if (reportControl != null)
            {
                reportControl.Panel = this;
            }

            return reportControl;
        }

        private bool NameAvailabilityCheck(string str)
        {
            var nameStatus = ((from name in this.reportItems
                             where name.ItemName.Equals(str)
                             select name).Count()) > 0 ? true : false;

            return nameStatus;
        }

        private bool TablixNameAvailabilityCheck(string str)
        {
            TablixControl.TablixItemNameCollection.Sort();
            var nameStatus = ((from name in TablixControl.TablixItemNameCollection
                               where name.Equals(str)
                               select name).Count()) > 0 ? true : false;

            return nameStatus;

        }

        public IReportItemControl AddReportItem(ReportItem reportItem, IReportItemControl parent)
        {
            this.drawingCanvas = parent as Canvas;
            IReportItemControl reportControl = this.AddReportItem(reportItem);
            reportControl.RaiseReportItemSizeChangedEvent();
            this.drawingCanvas = null;
            return reportControl;
        }

        public IReportItemControl AddReportItem(ReportItem reportItem)
        {
            IReportItemControl reportControl = null;

            if (reportItem != null)
            {
                if (reportItem is RDL.DOM.TextBox)
                {
                    this.SelectedReportItemType = DrawingReportItem.TextBox;
                }
                else if (reportItem is RDL.DOM.Image)
                {
                    this.SelectedReportItemType = DrawingReportItem.Image;
                }
                else if (reportItem is RDL.DOM.Line)
                {
                    this.SelectedReportItemType = DrawingReportItem.Line;
                }
                else if (reportItem is RDL.DOM.GaugePanel)
                {
                    this.SelectedReportItemType = DrawingReportItem.Gauge;
                }

                else if (reportItem is RDL.DOM.Chart)
                {
                    this.SelectedReportItemType = DrawingReportItem.Chart;
                }

                else if (reportItem is RDL.DOM.Tablix)
                {
                    this.SelectedReportItemType = DrawingReportItem.Tablix;
                }
                else if (reportItem is RDL.DOM.SubReport)
                {
                    this.SelectedReportItemType = DrawingReportItem.SubReport;
                }
                else if (reportItem is RDL.DOM.Rectangle)
                {
                    this.SelectedReportItemType = DrawingReportItem.Rectangle;
                }
                else if (reportItem is RDL.DOM.Map)
                {
                    this.SelectedReportItemType = DrawingReportItem.Map;
                }
                else
                {
                    return null;
                }
            }

            reportControl = this.GetReportItem(this.SelectedReportItemType, reportItem);

            if (reportControl != null)
            {
                UIElement control = reportControl as UIElement;

                if (reportItem == null)
                {
                    reportControl.ItemLeft = canvasStartingPoint.X;
                    reportControl.ItemTop = canvasStartingPoint.Y;

                    if (reportControl.ItemType == DrawingReportItem.Line)
                    {
                        reportControl.ItemWidth = canvasEndPoint.X - canvasStartingPoint.X;
                        reportControl.ItemHeight = canvasEndPoint.Y - canvasStartingPoint.Y;
                    }
                    else
                    {
                        reportControl.ItemWidth = Math.Abs(canvasEndPoint.X - canvasStartingPoint.X);
                        reportControl.ItemHeight = Math.Abs(canvasEndPoint.Y - canvasStartingPoint.Y);
                    }
                }
                else
                {
                    reportControl.ItemLeft = reportItem.Left.PixelValue;
                    reportControl.ItemTop = reportItem.Top.PixelValue;
                    reportControl.ItemWidth = reportItem.Width.PixelValue;
                    reportControl.ItemHeight = reportItem.Height.PixelValue;
                }

                if (this.drawingCanvas == null)
                {
                    this.drawingCanvas = this.bodyCanvas;
                }

                if (drawingCanvas == headerCanvas || drawingCanvas == footerCanvas || drawingCanvas == bodyCanvas)
                {
                    reportItems.Add(control as IReportItemControl);
                }
                this.drawingCanvas.Children.Add(control);
                control.InvalidateArrange();
                control.UpdateLayout();

                if ((!this.isTablixControlSelected && reportItem == null) && (reportControl.ItemHeight == 0.0 || reportControl.ItemWidth == 0.0))
                {
                    if (reportControl.ItemType == DrawingReportItem.Line || reportControl.ItemType == DrawingReportItem.TextBox)
                    {
                        reportControl.ItemWidth = 100;
                        reportControl.ItemHeight = 30;
                    }

                    else if (reportControl.ItemType == DrawingReportItem.Image)
                    {
                        reportControl.ItemWidth = 100;
                        reportControl.ItemHeight = 50;
                    }

                    else if (reportControl.ItemType == DrawingReportItem.Tablix)
                    {
                        reportControl.ItemWidth = 300;
                        reportControl.ItemHeight = 50;
                    }

                    else if (reportControl.ItemType == DrawingReportItem.Rectangle)
                    {
                        reportControl.ItemWidth = 200;
                        reportControl.ItemHeight = 50;
                    }

                    else if (reportControl.ItemType == DrawingReportItem.SubReport)
                    {
                        reportControl.ItemWidth = 300;
                        reportControl.ItemHeight = 300;
                    }

                    else
                    {
                        reportControl.ItemWidth = 300;
                        reportControl.ItemHeight = 200;
                    }
                    control.InvalidateArrange();
                    control.UpdateLayout();
                }

                if ( reportItem==null && reportControl.ItemType == DrawingReportItem.Image)
                {
                    (reportControl as ImageControl).Properties_Click(reportControl, new RoutedEventArgs());
                }

                this.drawingCanvas.Cursor = Cursors.Arrow;
                reportControl.Parent = this.drawingCanvas;
                reportControl.Panel = this;
                if (!this.isTablixControlSelected)
                {
                    this.UpdateSize(reportControl);
                }
                reportControl.ReportItemSizeChanged += new ReportItemControlSizeHandler(reportControl_ReportItemSizeChanged);
                reportControl.ReportItemSelected += new ReportItemSelectedEvent(reportControl_ReportItemSelected);
                this.SelectedReportItemType = DrawingReportItem.None;
            }

            return reportControl;
        }

        public RDL.DOM.TextBox GetTextBox()
        {
            RDL.DOM.TextBox textbox = new RDL.DOM.TextBox();
            textbox.Name = "TextBox" + ++this.textBoxCount;
            textbox.Style = new RDL.DOM.Style();
            textbox.Style.Border = new RDL.DOM.Border();
            textbox.Style.Border.Style = RDL.DOM.BorderStyles.Solid.ToString();
            textbox.Style.Border.Color = "#D3D3D3";
            textbox.Paragraphs = new RDL.DOM.Paragraphs();
            RDL.DOM.Paragraph paragraph = new RDL.DOM.Paragraph();
            paragraph.TextRuns = new RDL.DOM.TextRuns();
            paragraph.TextRuns.Add(new RDL.DOM.TextRun());
            textbox.Paragraphs.Add(paragraph);
            textbox.KeepTogether = true;
            return textbox;
        }

        public RDL.DOM.Rectangle GetRectangle()
        {
            RDL.DOM.Rectangle rectangle = new RDL.DOM.Rectangle();
            rectangle.Name = "Rectangle" + ++this.rectangleCount;
            rectangle.Style = new RDL.DOM.Style();
            rectangle.KeepTogether = true;
            return rectangle;
        }

        public void DeleteSelectedReportItems()
        {
            if (this.SelectedReportItems.Count > 0)
            {
                this.RemoveReportItems(this.SelectedReportItems);
                this.RaiseReportItemSelectedEvent(this, new SelectedItemEventArgs() { SelectedItem = this.reportproperties });
            }
        }

        internal void UpdateTablixReportItems(TablixControl tablix)
        {
            foreach (var cellContent in tablix.Children.OfType<CellContentsControl>())
            {
                IReportItemControl childItem = (cellContent.Content as IReportItemControl);

                if (childItem != null)
                {
                    childItem.ReportItem = childItem.GetReportItem();

                    if (childItem.ItemType == DrawingReportItem.Rectangle)
                    {
                        this.UpdateRectangleReportItems(childItem as RectangleControl);
                    }
                    else if (childItem.ItemType == DrawingReportItem.Tablix)
                    {
                        this.UpdateTablixReportItems(childItem as TablixControl);
                    }
                }
            }
        }

        internal void UpdateRectangleReportItems(RectangleControl rectangle)
        {
            foreach (var childItem in rectangle.Children.OfType<IReportItemControl>())
            {
                childItem.ReportItem = childItem.GetReportItem();

                if (childItem.ItemType == DrawingReportItem.Rectangle)
                {
                    this.UpdateRectangleReportItems(childItem as RectangleControl);
                }
                else if (childItem.ItemType == DrawingReportItem.Tablix)
                {
                    this.UpdateTablixReportItems(childItem as TablixControl);
                }
            }
        }


        public void RemoveReportItems(List<IReportItemControl> reportItems)
        {
            EditAction editAction = new EditAction();

            if (!this.IsInternalChange)
            {
                editAction.EditingType = EditActionType.ItemDelete;
                this.EditingManager.AddAction(editAction);
            }

            this.EditingManager.IsMergeAction = true;

            foreach (IReportItemControl reportitem in reportItems)
            {
                reportitem.ReportItem = reportitem.GetReportItem();
                editAction.EditReportItems.Add(reportitem);
                if (reportitem.ItemType == DrawingReportItem.Tablix)
                {
                    TablixControl tablix = reportitem as TablixControl;

                    if (tablix.selectBordersList.Count == 0 || tablix.ColumnDefinitions.Count==2)
                    {
                        tablix.IsItemRemoved = true;
                        reportitem.Parent.Children.Remove(reportitem as UIElement);
                    }
                }
                else
                {
                    reportitem.Parent.Children.Remove(reportitem as UIElement);
                }
                reportitem.RaiseReportItemSizeChangedEvent();

                if (reportitem.ItemType == DrawingReportItem.Tablix)
                {
                    this.UpdateTablixReportItems(reportitem as TablixControl);
                }
                else if (reportitem.ItemType == DrawingReportItem.Rectangle)
                {
                    this.UpdateRectangleReportItems(reportitem as RectangleControl);
                }

                if (reportitem.Parent == headerCanvas || reportitem.Parent == footerCanvas || reportitem.Parent == bodyCanvas)
                {
                    this.reportItems.Remove(reportitem);
                }
            }

            this.EditingManager.IsMergeAction = false;
        }

        public void CutReportItems()
        {
            if (this.SelectedReportItems.Count > 0)
            {
                this.CopyReportItems();
                this.DeleteSelectedReportItems();
            }
        }

        public void CopyReportItems()
        {
            ReportItems reportItems = new ReportItems();

            foreach (var reportItem in this.SelectedReportItems)
            {
                reportItems.Add(reportItem.GetReportItem());
            }

            if (reportItems.Count > 0)
            {
                MemoryStream stream = new MemoryStream();
                this.copySerializer.Serialize(stream, reportItems);
                string textValue = Encoding.UTF8.GetString(stream.GetBuffer());
                ClipboardData copyContent = new ClipboardData();
                copyContent.Data = textValue;
                Clipboard.SetData(System.Windows.DataFormats.Serializable, copyContent);
            }
        }

        public void PasteReportItems()
        {
            IDataObject cliboardObj = Clipboard.GetDataObject() as IDataObject;

            if (cliboardObj != null)
            {
                object obj = Clipboard.GetData(System.Windows.DataFormats.Serializable);

                if (obj != null && obj is ClipboardData)
                {
                    ClipboardData itemText = obj as ClipboardData;
                    byte[] textValue = Encoding.UTF8.GetBytes(itemText.Data.ToCharArray());
                    MemoryStream stream = new MemoryStream(textValue);
                    ReportItems items = this.copySerializer.Deserialize(stream) as ReportItems;

                    if (items != null && items.Count > 0)
                    {
                        try
                        {
                            EditAction editAction = new EditAction();
                            double midleft = this.bodyCanvas.ActualWidth / 2;
                            double previousLeft = 0;
                            if (items.Count > 1)
                            {
                                try
                                {
                                    previousLeft = items.OrderBy(rpItem => rpItem.Left.PixelValue).First().Left.PixelValue;
                                }
                                catch { }
                            }
                            foreach (var itemControl in items)
                            {
                                double itemleft = midleft - (itemControl.Width.PixelValue / 2);
                                if (previousLeft > 0)
                                {
                                    itemleft += (itemControl.Left.PixelValue - previousLeft);
                                }

                                itemControl.Left = new Base.Size(itemleft);
                                this.drawingCanvas = this.bodyCanvas;
                                IReportItemControl reportItem = this.AddReportItem(itemControl);
                                editAction.EditReportItems.Add(reportItem);
                                this.drawingCanvas = null;
                                if ((reportItem.ItemTop + reportItem.ItemHeight) > this.bodyCanvas.ActualHeight)
                                {
                                    this.UpdateSize(reportItem);
                                }
                            }

                            editAction.EditingType = EditActionType.ItemAdd;
                            this.EditingManager.AddAction(editAction);
                        }
                        catch { }
                    }
                }
            }
        }

        void reportControl_ReportItemSelected(object sender, SelectedItemEventArgs e)
        {
            if (sender is IReportItemControl)
            {
                if (e.IsSelected)
                {
                    this.aLayer = AdornerLayer.GetAdornerLayer((UIElement)sender);
                    ResizingAdorner _ra = new ResizingAdorner((UIElement)sender, this);
                    this.aLayer.Add(_ra);
                }
                else
                {
                    try
                    {
                        if (aLayer.GetAdorners((UIElement)sender) != null)
                        {
                            if (aLayer.GetAdorners((UIElement)sender).Count() > 0)
                            {
                                this.aLayer.Remove(aLayer.GetAdorners((UIElement)sender).First());
                            }
                        }
                    }
                    catch { }
                }
            }

            this.RaiseReportItemSelectedEvent(sender, e);
        }

        void reportControl_ReportItemSizeChanged(object sender, EventArgs e)
        {
            if (sender is IReportItemControl)
            {
                this.UpdateSize(sender as IReportItemControl);
            }
        }

        void UpdateSize(IReportItemControl reportControl)
        {
            if (reportControl.Parent is RectangleControl)
            {
                RectangleControl rectangleControl = reportControl.Parent as RectangleControl;
                double top = reportControl.ItemTop;
                double bottom = top + reportControl.ItemHeight;
                double drawingAreaHeight = rectangleControl.ItemHeight;

                double left = reportControl.ItemLeft;
                double right = left + reportControl.ItemWidth;

                double drawingAreaWidth = rectangleControl.ItemWidth;

                double itemHeight = rectangleControl.ItemHeight;
                double itemWidth = rectangleControl.ItemWidth;

                bool invalidateLayout = false;

                if (bottom > drawingAreaHeight)
                {
                    itemHeight = bottom + 2;
                    invalidateLayout = true;
                }

                if (right > drawingAreaWidth)
                {
                    itemWidth = right + 2;
                    invalidateLayout = true;
                }

                if (rectangleControl.Parent is Canvas && invalidateLayout)
                {
                    rectangleControl.ItemHeight = itemHeight;
                    rectangleControl.ItemWidth = itemWidth;
                    rectangleControl.InvalidateMeasure();
                    rectangleControl.InvalidateArrange();
                    rectangleControl.UpdateLayout();
                    rectangleControl.RaiseReportItemSizeChangedEvent();
                }
                else if ((rectangleControl as FrameworkElement).Parent is CellContentsControl && invalidateLayout)
                {
                    CellContentsControl cellContent = (rectangleControl as FrameworkElement).Parent as CellContentsControl;
                    TablixControl tablix = cellContent.Parent as TablixControl;
                    int row = Grid.GetRow(cellContent);
                    int column = Grid.GetColumn(cellContent);
                    double diffWidth = itemWidth - rectangleControl.ItemWidth;
                    double diffHeight = itemHeight - rectangleControl.ItemHeight;
                    tablix.ItemWidth += diffWidth;
                    tablix.ItemHeight += diffHeight;
                    tablix.RowDefinitions[row].Height = new GridLength(itemHeight, GridUnitType.Star);
                    tablix.ColumnDefinitions[column].Width = new GridLength(itemWidth, GridUnitType.Star);
                    tablix.InvalidateMeasure();
                    tablix.InvalidateArrange();
                    tablix.UpdateLayout();
                    tablix.RaiseReportItemSizeChangedEvent();
                }
            }
            else if (reportControl.Parent != null)
            {
                double drawingAreaHeight = 0;

                if (reportControl.Parent == this.bodyCanvas)
                {
                    drawingAreaHeight = new RDL.DOM.Size(this.bodyproperties.BodyHeight).PixelValue;
                }
                else if (reportControl.Parent == this.headerCanvas)
                {
                    drawingAreaHeight = new RDL.DOM.Size(this.headerproperties.HeaderHeight).PixelValue;
                }
                else if (reportControl.Parent == this.footerCanvas)
                {
                    drawingAreaHeight = new RDL.DOM.Size(this.footerproperties.FooterHeight).PixelValue;
                }

                double top = reportControl.ItemTop;
                double bottom = top + reportControl.ItemHeight;

                if (reportControl.ItemHeight < 0)
                {
                    top = reportControl.ItemTop + reportControl.ItemHeight;
                    bottom = reportControl.ItemTop;
                }

                if (top < 0)
                {
                    bottom = -top + drawingAreaHeight;

                    foreach (var reportItemControl in reportControl.Parent.Children)
                    {
                        IReportItemControl reportItem = reportItemControl as IReportItemControl;

                        if (reportItem != null && reportItem.ItemType != DrawingReportItem.Line)
                        {
                            var topValue = reportItem.ItemTop;
                            reportItem.ItemTop = topValue - top;
                        }
                        else if (reportItem != null)
                        {
                            double heightVal = reportItem.ItemHeight;
                            reportItem.ItemTop += -top;
                            reportItem.ItemHeight = heightVal;

                            // Workaround to invalidate Adroners for Line
                            if (AdornerLayer.GetAdornerLayer(reportItem as UIElement).GetAdorners(reportItem as UIElement) != null)
                            {
                                Adorner layer = AdornerLayer.GetAdornerLayer(reportItem as UIElement).GetAdorners(reportItem as UIElement).First();
                                layer.InvalidateArrange();
                            }
                        }
                    }
                }

                double left = reportControl.ItemLeft;
                double right = left + reportControl.ItemWidth;
                double drawingAreaWidth = new RDL.DOM.Size(this.bodyproperties.ReportWidth).PixelValue;

                if (reportControl.ItemWidth < 0)
                {
                    left = reportControl.ItemLeft + reportControl.ItemWidth;
                    right = reportControl.ItemLeft;
                }

                if (left < 0)
                {
                    right = -left + drawingAreaWidth;

                    foreach (var reportItemControl in reportControl.Parent.Children)
                    {
                        IReportItemControl reportItem = reportItemControl as IReportItemControl;

                        if (reportItem != null && reportItem.ItemType != DrawingReportItem.Line)
                        {
                            var leftValue = reportItem.ItemLeft;
                            reportItem.ItemLeft = leftValue - left;
                        }
                        else if (reportItem != null)
                        {
                            double width = reportItem.ItemWidth;
                            reportItem.ItemLeft += -left;
                            reportItem.ItemWidth = width;

                            // Workaround to invalidate Adroners for Line
                            if (AdornerLayer.GetAdornerLayer(reportItem as UIElement).GetAdorners(reportItem as UIElement) != null)
                            {
                                Adorner layer = AdornerLayer.GetAdornerLayer(reportItem as UIElement).GetAdorners(reportItem as UIElement).First();
                                layer.InvalidateArrange();
                            }
                        }
                    }
                }

                double maxBottom = 0;
                double maxRight = 0;

                foreach (var reportItemControl in reportControl.Parent.Children)
                {
                    if (reportItemControl is IReportItemControl)
                    {
                        IReportItemControl reportItem = reportItemControl as IReportItemControl;
                        var leftValue = reportItem.ItemLeft;
                        double rightVal = leftValue + reportItem.ItemWidth;

                        if (reportItem.ItemType == DrawingReportItem.Line && reportItem.ItemWidth < 0)
                        {
                            leftValue = reportItem.ItemLeft + reportItem.ItemWidth;
                            rightVal = reportItem.ItemLeft;
                        }

                        var topValue = reportItem.ItemTop;
                        double bottomVal = topValue + reportItem.ItemHeight;

                        if (reportItem.ItemType == DrawingReportItem.Line && reportItem.ItemHeight < 0)
                        {
                            topValue = reportItem.ItemTop + reportItem.ItemHeight;
                            bottomVal = reportItem.ItemTop;
                        }

                        maxRight = maxRight > rightVal ? maxRight : rightVal;
                        maxBottom = maxBottom > bottomVal ? maxBottom : bottomVal;
                    }
                }

                double oldHeightValue = 0;

                if (reportControl.Parent == this.bodyCanvas)
                {
                    oldHeightValue = this.MinBodyHeight;
                    this.MinBodyHeight = maxBottom + 5;
                    this.maxBodyWidthValue = maxRight;
                }
                else if (reportControl.Parent == this.headerCanvas)
                {
                    oldHeightValue = this.MinHeaderheight;
                    this.MinHeaderheight = maxBottom + 5;
                    this.maxHeaderWidthValue = maxRight;
                }
                else if (reportControl.Parent == this.footerCanvas)
                {
                    oldHeightValue = this.MinFooterHeight;
                    this.MinFooterHeight = maxBottom + 5;
                    this.maxFooterWidthValue = maxRight;
                }

                if (bottom > drawingAreaHeight)
                {
                    double heightValue = bottom + 10;

                    if (reportControl.Parent == this.bodyCanvas)
                    {
                        this.bodyproperties.BodyHeight = converter.GetSizeValue(heightValue, this.bodyproperties.BodyHeight);
                    }
                    else if (reportControl.Parent == this.headerCanvas)
                    {
                        this.headerproperties.HeaderHeight = converter.GetSizeValue(heightValue, this.headerproperties.HeaderHeight);
                    }
                    else if (reportControl.Parent == this.footerCanvas)
                    {
                        this.footerproperties.FooterHeight = converter.GetSizeValue(heightValue, this.footerproperties.FooterHeight);
                    }
                }

                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemPropertyChanged;
                PropertyChanage change = new PropertyChanage();
                change.OldValue = oldHeightValue;
                action.PropertyChange = change;
                this.EditingManager.AddAction(action);

                if (reportControl.Parent == this.bodyCanvas)
                {
                    change.PropertyObject = this;
                    change.NewValue = this.MinBodyHeight;
                    change.PropertyName = "MinBodyHeight";
                }
                else if (reportControl.Parent == this.headerCanvas)
                {
                    change.PropertyObject = this;
                    change.NewValue = this.MinHeaderheight;
                    change.PropertyName = "MinHeaderheight";
                }
                else if (reportControl.Parent == this.footerCanvas)
                {
                    change.PropertyObject = this;
                    change.NewValue = this.MinFooterHeight;
                    change.PropertyName = "MinFooterHeight";
                }

                double oldWidthValue = this.MinReportWidth;
                this.MinReportWidth = (maxBodyWidthValue > maxHeaderWidthValue) ? ((maxBodyWidthValue > maxFooterWidthValue) ? maxBodyWidthValue : maxFooterWidthValue) : ((maxHeaderWidthValue > maxFooterWidthValue) ? maxHeaderWidthValue : maxFooterWidthValue);

                if (right > drawingAreaWidth)
                {
                    this.bodyproperties.ReportWidth = converter.GetSizeValue(right + 10, this.bodyproperties.ReportWidth);
                }

                EditAction widthAction = new EditAction();
                widthAction.EditingType = EditActionType.ItemPropertyChanged;
                PropertyChanage widthChange = new PropertyChanage();
                widthChange.OldValue = oldWidthValue;
                widthChange.PropertyObject = this;
                widthChange.NewValue = this.MinReportWidth;
                widthChange.PropertyName = "MinReportWidth";
                widthAction.PropertyChange = widthChange;
                this.EditingManager.AddAction(widthAction);
            }
        }

        internal static string GetConvertedValue(double value, MeasurementUnits unit, RDL.DOM.ReportUnitType unitType)
        {
            if (string.Equals(unit.ToString(), MeasurementUnits.In.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                value = unitType == RDL.DOM.ReportUnitType.In ? value : Math.Round(value * 2.540, 5);
            }
            else if (string.Equals(unit.ToString(), MeasurementUnits.Cm.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                value = unitType == RDL.DOM.ReportUnitType.In ? Math.Round(value * 0.3937, 5) : value;
            }
            else if (string.Equals(unit.ToString(), MeasurementUnits.Pt.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                value = unitType == RDL.DOM.ReportUnitType.In ? value * 0.0139 : Math.Round(value * 0.0353, 5);
            }
            else if (string.Equals(unit.ToString(), MeasurementUnits.Mm.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                value = unitType == RDL.DOM.ReportUnitType.In ? value * 0.03937 : Math.Round(value * 0.1, 5);
            }
            else if (string.Equals(unit.ToString(), MeasurementUnits.Pc.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                value = unitType == RDL.DOM.ReportUnitType.In ? value * 0.01042 : Math.Round(value * 0.02645, 5);
            }
            if (unit == MeasurementUnits.None)
            {
                unit = MeasurementUnits.In;
            }

            return value + unitType.ToString().ToLower();
        }

        internal static string GetMeasuredUnit(string sizeValue)
        {
            if (!string.IsNullOrEmpty(sizeValue) && !sizeValue.StartsWith("="))
            {
                var _regexChar = new System.Text.RegularExpressions.Regex("[a-zA-Z]+");
                sizeValue = sizeValue.Trim().Replace("NaN", "0");
                var m = _regexChar.Match(sizeValue);

                if (m.Success)
                {
                    return Enum.Parse(typeof(RDL.DOM.MeasurementUnits), m.Value, true).ToString().ToLower();
                }
            }
            return "in";
        }

        #region Chart Wizard Implementation

        public void AddChartThroughWizard(string title)
        {
            this.controldialog = new ControlDialog(title, this.DataSources, this.DataSets, this);
            this.UpdateOwnerWindow(controldialog);

            EditAction editAction = new EditAction();
            editAction.EditingType = EditActionType.ItemAdd;
            this.EditingManager.AddAction(editAction);
            this.EditingManager.IsMergeAction = true;

            if (this.controldialog.ShowDialog() == true)
            {
                RDL.DOM.Chart chart = new RDL.DOM.Chart();
                chart.Name = "Chart" + ++this.chartCount;
                chart.Width = 3 + "in";
                chart.Height = 2 + "in";
                chart.Left = 0.5 + "in";
                chart.Top = 0.5 + "in";
                chart.DataSetName = this.controldialog.reportDataSet.Name;
                this.UpdateChartSeries(chart);
                this.UpdateChartObj(chart);
                IReportItemControl reportItem = this.AddReportItem(chart);
                reportItem.RaiseReportItemSizeChangedEvent();
                editAction.EditReportItems.Add(reportItem);

                if (controldialog.CachedDataSources.Count > 0)
                {
                    this.RaiseDataSourceCollectionModifiedEvent();
                }

                if (controldialog.CachedDataSets.Count > 0)
                {
                    this.RaiseDataSetCollectionModifiedEvent();
                }
            }
            else
            {
                foreach (var dataSource in controldialog.CachedDataSources)
                {
                    this.DataSources.Remove(dataSource);
                }

                foreach (var dataSet in controldialog.CachedDataSets)
                {
                    this.DataSets.Remove(dataSet);
                }

                this.EditingManager.RemoveAction();
            }

            this.EditingManager.IsMergeAction = false;
        }

        private Syncfusion.RDL.DOM.Chart UpdateChartSeries(Syncfusion.RDL.DOM.Chart ChartObj)
        {
            ChartObj.ChartCategoryHierarchy = new Syncfusion.RDL.DOM.ChartCategoryHierarchy();
            ChartObj.ChartCategoryHierarchy.ChartMembers = new Syncfusion.RDL.DOM.ChartMembers();
            ChartObj.ChartSeriesHierarchy = new Syncfusion.RDL.DOM.ChartSeriesHierarchy();
            ChartObj.ChartSeriesHierarchy.ChartMembers = new Syncfusion.RDL.DOM.ChartMembers();
            ChartObj.ChartData = new Syncfusion.RDL.DOM.ChartData();
            ChartObj.ChartData.ChartSeriesCollection = new Syncfusion.RDL.DOM.ChartSeriesCollection();
            Syncfusion.RDL.DOM.ChartMember chartMember = new Syncfusion.RDL.DOM.ChartMember();
            chartMember.Label = string.Empty;
            if (this.controldialog.RowList != null)
            {
                if (this.controldialog.RowList.Count != 0)
                {
                    chartMember.Group = new Syncfusion.RDL.DOM.Group();
                    Syncfusion.RDL.DOM.GroupExpressions groupExpressions = new Syncfusion.RDL.DOM.GroupExpressions();
                    Syncfusion.RDL.DOM.GroupExpression groupExpression = new Syncfusion.RDL.DOM.GroupExpression();
                    string subFieldName = this.controldialog.RowList.ElementAt(0);


                    String chartCategoryValue = "=" + "Fields!" + subFieldName + ".value";
                    chartMember.Label = chartCategoryValue;
                    chartMember.Group.Name = "Chart1_CategoryGroup1";
                    groupExpression.Value = chartCategoryValue;

                    groupExpressions.Add(groupExpression);
                    chartMember.Group.GroupExpressions = groupExpressions;

                    ChartObj.ChartCategoryHierarchy.ChartMembers.Add(chartMember);
                }
            }
            if (this.controldialog.ColumnList != null)
            {
                if (this.controldialog.ColumnList.Count != 0)
                {
                    foreach (string seriesField in this.controldialog.ColumnList)
                    {
                        Syncfusion.RDL.DOM.ChartMember sreieschartmember = new Syncfusion.RDL.DOM.ChartMember();
                        sreieschartmember.Group = new RDL.DOM.Group();
                        sreieschartmember.Group.Name = "Chart1_SeriesGroup1";
                        sreieschartmember.Group.GroupExpressions = new RDL.DOM.GroupExpressions();
                        RDL.DOM.GroupExpression groupexpression = new RDL.DOM.GroupExpression();

                        string subFieldName = string.Empty;

                        subFieldName = seriesField;

                        groupexpression.Value = "=" + "Fields!" + subFieldName + ".value";
                        sreieschartmember.Label = "=" + "Fields!" + subFieldName + ".value";
                        sreieschartmember.Group.GroupExpressions.Add(groupexpression);
                        sreieschartmember.ChartMembers = new Syncfusion.RDL.DOM.ChartMembers();
                        foreach (string chartseries in this.controldialog.ValuesList)
                        {
                            RDL.DOM.ChartMember chartmember1 = new RDL.DOM.ChartMember();
                            int startIndex = chartseries.IndexOf("(");
                            int endIndex = chartseries.IndexOf(")");
                            chartmember1.Label = chartseries.Substring(startIndex + 1, endIndex - startIndex - 1);
                            sreieschartmember.ChartMembers.Add(chartmember1);
                        }
                        sreieschartmember.Label = "=" + "Fields!" + subFieldName + ".value";
                        ChartObj.ChartSeriesHierarchy.ChartMembers.Add(sreieschartmember);
                    }

                }
                else
                {
                    foreach (string chartseries in this.controldialog.ValuesList)
                    {
                        Syncfusion.RDL.DOM.ChartMember chartmember = new Syncfusion.RDL.DOM.ChartMember();
                        int startIndex = chartseries.IndexOf("(");
                        int endIndex = chartseries.IndexOf(")");
                        chartmember.Label = chartseries.Substring(startIndex + 1, endIndex - startIndex - 1);
                        ChartObj.ChartSeriesHierarchy.ChartMembers.Add(chartmember);
                    }
                }
            }
            foreach (string value in this.controldialog.ValuesList)
            {
                Syncfusion.RDL.DOM.ChartSeries chartseries = new Syncfusion.RDL.DOM.ChartSeries();
                chartseries.ChartDataPoints = new Syncfusion.RDL.DOM.ChartDataPoints();
                Syncfusion.RDL.DOM.ChartDataPoint ChartDataPoint = new Syncfusion.RDL.DOM.ChartDataPoint();
                ChartDataPoint.Style = new Syncfusion.RDL.DOM.Style();
                ChartDataPoint.Style = new Syncfusion.RDL.DOM.Style();
                ChartDataPoint.Style.Border = new Syncfusion.RDL.DOM.Border();
                ChartDataPoint.Style.Border = new Syncfusion.RDL.DOM.Border();
                ChartDataPoint.ChartMarker = new Syncfusion.RDL.DOM.ChartMarker();
                ChartDataPoint.ChartMarker = new Syncfusion.RDL.DOM.ChartMarker();
                ChartDataPoint.ChartMarker.Style = new Syncfusion.RDL.DOM.Style();
                ChartDataPoint.ChartMarker.Style = new Syncfusion.RDL.DOM.Style();
                ChartDataPoint.ChartDataPointValues = new Syncfusion.RDL.DOM.ChartDataPointValues();
                ChartDataPoint.ChartDataLabel = new RDL.DOM.ChartDataLabel();


                int startIndex = value.IndexOf("(");
                int endIndex = value.IndexOf(")");
                chartseries.Name = value.Substring(startIndex + 1, endIndex - startIndex - 1);

                chartseries.ChartDataLabel = new RDL.DOM.ChartDataLabel();
                chartseries.ValueAxisName = "Primary";
                chartseries.CategoryAxisName = "Primary";
                string chartSeriesValue = string.Empty;
                if (!value.Contains("Count") && !value.Contains("Sum") && !value.Contains("Avg"))
                {
                    chartSeriesValue = "=" + "Count" + "(" + "Fields!" + value + ".Value" + ")";
                }
                else
                {
                    int j = value.IndexOf("(");
                    int k = value.IndexOf(")");

                    int l1 = k - j;
                    // string functionName = value.Substring(i + 1, l - 1);
                    String fieldName = value.Substring(j + 1, l1 - 1);
                    if (value.Contains("Count"))
                    {
                        chartSeriesValue = "=" + "Count" + "(" + "Fields!" + fieldName + ".Value" + ")";
                    }
                    else if (value.Contains("Avg"))
                    {
                        chartSeriesValue = "=" + "Avg" + "(" + "Fields!" + fieldName + ".Value" + ")";
                    }
                    else
                    {
                        chartSeriesValue = "=" + "Sum" + "(" + "Fields!" + fieldName + ".Value" + ")";
                    }

                }
                ChartDataPoint.ChartDataPointValues.Y = chartSeriesValue;
                chartseries.Type = RDL.DOM.VisualizationType.Column;
                chartseries.Subtype = RDL.DOM.VisualizationSubType.Plain;
                chartseries.ChartDataPoints.Add(ChartDataPoint);
                ChartObj.ChartData.ChartSeriesCollection.Add(chartseries);
            }
            return ChartObj;

        }

        private Syncfusion.RDL.DOM.Chart UpdateChartObj(Syncfusion.RDL.DOM.Chart ChartObj)
        {
            ChartObj.ChartAreas = new Syncfusion.RDL.DOM.ChartAreas();
            Syncfusion.RDL.DOM.ChartArea chartArea = new Syncfusion.RDL.DOM.ChartArea();
            chartArea.Style = new Syncfusion.RDL.DOM.Style();
            chartArea.Name = "Default";
            chartArea.ChartCategoryAxes = new Syncfusion.RDL.DOM.ChartCategoryAxes();
            chartArea.ChartValueAxes = new Syncfusion.RDL.DOM.ChartValueAxes();
            Syncfusion.RDL.DOM.ChartAxis primaryAxis = new Syncfusion.RDL.DOM.ChartAxis();
            primaryAxis.ChartMajorGridLines = new Syncfusion.RDL.DOM.ChartMajorGridLines();
            primaryAxis.ChartMinorGridLines = new Syncfusion.RDL.DOM.ChartMinorGridLines();
            primaryAxis.ChartMajorTickMarks = new Syncfusion.RDL.DOM.ChartMajorTickMarks();
            primaryAxis.ChartMajorTickMarks.Style = new Syncfusion.RDL.DOM.Style();
            primaryAxis.ChartMajorTickMarks.Style.Border = new Syncfusion.RDL.DOM.Border();
            primaryAxis.ChartMinorTickMarks = new Syncfusion.RDL.DOM.ChartMinorTickMarks();
            primaryAxis.ChartMinorTickMarks.Style = new Syncfusion.RDL.DOM.Style();
            primaryAxis.ChartMinorTickMarks.Style.Border = new Syncfusion.RDL.DOM.Border();
            primaryAxis.Style = new Syncfusion.RDL.DOM.Style();
            primaryAxis.Style.Border = new Syncfusion.RDL.DOM.Border();
            Syncfusion.RDL.DOM.ChartAxis secondaryAxis = new Syncfusion.RDL.DOM.ChartAxis();
            secondaryAxis.ChartMajorGridLines = new Syncfusion.RDL.DOM.ChartMajorGridLines();
            secondaryAxis.ChartMinorGridLines = new Syncfusion.RDL.DOM.ChartMinorGridLines();
            secondaryAxis.ChartMajorTickMarks = new Syncfusion.RDL.DOM.ChartMajorTickMarks();
            secondaryAxis.ChartMajorTickMarks.Style = new Syncfusion.RDL.DOM.Style();
            secondaryAxis.ChartMajorTickMarks.Style.Border = new Syncfusion.RDL.DOM.Border();
            secondaryAxis.ChartMinorTickMarks = new Syncfusion.RDL.DOM.ChartMinorTickMarks();
            secondaryAxis.ChartMinorTickMarks.Style = new Syncfusion.RDL.DOM.Style();
            secondaryAxis.ChartMinorTickMarks.Style.Border = new Syncfusion.RDL.DOM.Border();
            secondaryAxis.Style = new Syncfusion.RDL.DOM.Style();
            secondaryAxis.Style.Border = new Syncfusion.RDL.DOM.Border();
            chartArea.Style.BackgroundGradientEndColor = Brushes.White.ToString();
            chartArea.Style.BackgroundGradientType = RDL.DOM.BackgroundGradientTypes.None;
            primaryAxis.Name = "Primary";
            primaryAxis.Reverse = false;
            primaryAxis.Style.Border.Color = Brushes.Black.ToString();
            primaryAxis.Style.Border.Style = Syncfusion.RDL.DOM.BorderStyles.Solid.ToString();
            primaryAxis.Style.Border.Width = "1" + "pt";
            primaryAxis.Style.FontSize = "10" + "pt";
            //    primaryAxis.Style.FontFamily = this.ChartArea.PrimaryAxis.LabelFontFamily.ToString();
            primaryAxis.Angle = 0;
            primaryAxis.ChartMajorGridLines.Enabled = RDL.DOM.BooleanOptions.False;
            primaryAxis.ChartMinorGridLines.Enabled = RDL.DOM.BooleanOptions.False;
            primaryAxis.ChartMajorTickMarks.Enabled = RDL.DOM.BooleanOptions.True;
            primaryAxis.ChartMinorTickMarks.Enabled = RDL.DOM.BooleanOptions.False;
            primaryAxis.ChartMajorTickMarks.Length = 2;
            primaryAxis.ChartMajorTickMarks.Style.Border.Color = Brushes.Black.ToString();
            // primaryAxis.ChartMajorTickMarks.Style.Border.Style = Syncfusion.RDL.DOM.BorderStyles.Solid;
            primaryAxis.ChartMajorTickMarks.Style.Border.Width = "2" + "pt";
            primaryAxis.ChartAxisTitle = new Syncfusion.RDL.DOM.ChartAxisTitle();
            primaryAxis.ChartAxisTitle.Style = new Syncfusion.RDL.DOM.Style();
            //  primaryAxis.ChartAxisTitle.Caption = this.ValueAxisTitle.Text;
            primaryAxis.ChartAxisTitle.Position = Syncfusion.RDL.DOM.ChartAxisTitlePosition.Center;
            primaryAxis.ChartAxisTitle.Style.FontSize = "10" + "pt";
            primaryAxis.ChartAxisTitle.Style.BackgroundColor = Brushes.White.ToString();
            primaryAxis.ChartAxisTitle.Style.FontWeight = RDL.DOM.FontWeight.Normal.ToString();
            primaryAxis.ChartAxisTitle.Style.FontStyle = RDL.DOM.FontStyle.Normal.ToString();
            secondaryAxis.Name = "Secondary";
            secondaryAxis.Reverse = false;
            secondaryAxis.Style.Border.Color = Brushes.Black.ToString();
            secondaryAxis.Style.Border.Style = Syncfusion.RDL.DOM.BorderStyles.Solid.ToString();
            secondaryAxis.Style.Border.Width = "1" + "pt";
            secondaryAxis.Style.FontSize = "10" + "pt";
            // secondaryAxis.Style.FontFamily = this.ChartArea.SecondaryAxis.LabelFontFamily.ToString();
            secondaryAxis.Angle = 0;
            secondaryAxis.HideLabels = false;
            secondaryAxis.ChartMajorGridLines.Enabled = RDL.DOM.BooleanOptions.False;
            secondaryAxis.ChartMinorGridLines.Enabled = RDL.DOM.BooleanOptions.False;
            secondaryAxis.ChartMajorTickMarks.Enabled = RDL.DOM.BooleanOptions.True;
            secondaryAxis.ChartMinorTickMarks.Enabled = RDL.DOM.BooleanOptions.False;
            secondaryAxis.ChartMajorTickMarks.Length = 2;
            secondaryAxis.ChartMajorTickMarks.Style.Border.Color = Brushes.Black.ToString();
            secondaryAxis.ChartMajorTickMarks.Style.Border.Style = Syncfusion.RDL.DOM.BorderStyles.Solid.ToString();
            secondaryAxis.ChartMajorTickMarks.Style.Border.Width = "2" + "pt";
            secondaryAxis.ChartAxisTitle = new Syncfusion.RDL.DOM.ChartAxisTitle();
            secondaryAxis.ChartAxisTitle.Style = new Syncfusion.RDL.DOM.Style();
            secondaryAxis.ChartAxisTitle.Position = Syncfusion.RDL.DOM.ChartAxisTitlePosition.Center;
            secondaryAxis.ChartAxisTitle.Style.FontSize = "10" + "pt";
            secondaryAxis.ChartAxisTitle.Style.BackgroundColor = Brushes.White.ToString();
            secondaryAxis.ChartAxisTitle.Style.FontWeight = RDL.DOM.FontWeight.Normal.ToString();
            secondaryAxis.ChartAxisTitle.Style.FontStyle = RDL.DOM.FontStyle.Normal.ToString();
            chartArea.ChartCategoryAxes.Add(primaryAxis);
            chartArea.ChartCategoryAxes.Add(secondaryAxis);
            chartArea.ChartValueAxes.Add(primaryAxis);
            chartArea.ChartValueAxes.Add(secondaryAxis);
            ChartObj.ChartAreas.Add(chartArea);
            ChartObj.ChartLegends = new Syncfusion.RDL.DOM.ChartLegends();
            Syncfusion.RDL.DOM.ChartLegend chartLegand = new Syncfusion.RDL.DOM.ChartLegend();
            chartLegand.Style = new Syncfusion.RDL.DOM.Style();
            chartLegand.Position = Syncfusion.RDL.DOM.Positions.TopCenter;
            ChartObj.ChartLegends.Add(chartLegand);
            ChartObj.ChartTitles = new Syncfusion.RDL.DOM.ChartTitles();
            Syncfusion.RDL.DOM.ChartTitle chartTitle = new Syncfusion.RDL.DOM.ChartTitle();
            chartTitle.Style = new Syncfusion.RDL.DOM.Style();

            chartTitle.Name = "Default";
            chartTitle.Caption = "Chart Title";
            chartTitle.Hidden = false;
            chartTitle.Position = RDL.DOM.Positions.TopCenter;
            chartTitle.Style.BackgroundColor = Brushes.White.ToString();
            chartTitle.Style.FontSize = "12" + "pt";
            chartTitle.Style.FontWeight = RDL.DOM.FontWeight.Normal.ToString();
            chartTitle.Style.FontStyle = RDL.DOM.FontStyle.Normal.ToString();
            ChartObj.ChartTitles.Add(chartTitle);
            return ChartObj;
        }
        #endregion

        #region Tablix Wizard Implementation

        public void AddTablixThroughWizard(string title)
        {
            controldialog = new ControlDialog(title, this.DataSources, this.DataSets, this);
            this.UpdateOwnerWindow(controldialog);

            EditAction editAction = new EditAction();
            editAction.EditingType = EditActionType.ItemAdd;
            this.EditingManager.AddAction(editAction);
            this.EditingManager.IsMergeAction = true;

            if (controldialog.ShowDialog() == true)
            {
                String dataSetName = controldialog.reportDataSet.Name;
                RDL.DOM.Tablix tablix = new Base.Tablix();
                tablix = CreateRDL_Wizard(controldialog);
                tablix.Name = "Tablix" + ++tablixCount;
                tablix.Width = 4.5 + "in";
                tablix.Height = 1 + "in";
                tablix.Left = 0.5 + "in";
                tablix.Top = 0.5 + "in";

                IReportItemControl reportItem = this.AddReportItem(tablix);
                editAction.EditReportItems.Add(reportItem);
                reportItem.RaiseReportItemSizeChangedEvent();

                if (controldialog.CachedDataSources.Count > 0)
                {
                    this.RaiseDataSourceCollectionModifiedEvent();
                }

                if (controldialog.CachedDataSets.Count > 0)
                {
                    this.RaiseDataSetCollectionModifiedEvent();
                }
            }
            else
            {
                foreach (var dataSource in controldialog.CachedDataSources)
                {
                    this.DataSources.Remove(dataSource);
                }

                foreach (var dataSet in controldialog.CachedDataSets)
                {
                    this.DataSets.Remove(dataSet);
                }

                this.EditingManager.RemoveAction();
            }

            this.EditingManager.IsMergeAction = false;
        }

        // Tablix Wizard Implementation
        private int detailsId = 1;

        /// <summary>
        /// create a RDL file for Tablix wizard
        /// </summary>
        /// <param name="controlDialog"></param>
        /// <param name="dataSetName"></param>
        private Syncfusion.RDL.DOM.Tablix CreateRDL_Wizard(ControlDialog controlDialog)
        {
            Syncfusion.RDL.DOM.Tablix tablix = new Syncfusion.RDL.DOM.Tablix();

            // set the data set Name
            if (controlDialog.reportDataSet.Name != null && controlDialog.reportDataSet.Name != string.Empty)
            {
                tablix.DataSetName = controlDialog.reportDataSet.Name;
            }

            tablix.TablixCorner = CreateTablixCorner_Wizard(controlDialog);
            tablix.TablixRowHierarchy = CreateTablixRowHierarchy_Wizard(controlDialog);
            tablix.TablixColumnHierarchy = CreateTablixColumn_Hierarchy_Wizard(controlDialog);
            tablix.TablixBody = CreateTablixBody_Wizard(controlDialog);

            return tablix;
        }


        /// <summary>
        /// Find whether the Row Hierarchy has Details Group or not
        /// </summary>
        /// <param name="controlDialog"></param>
        /// <returns></returns>
        private bool HasDetailsInRowHier(ControlDialog controlDialog)
        {
            if (controlDialog != null && controlDialog.ValuesList != null && controlDialog.ValuesList.Count > 0)
            {
                foreach (string value in controlDialog.ValuesList)
                {
                    // Here, the aggregate function should start with "=" by our code. If a value with no aggregation function founds, return true. Otherwise, false
                    if (!(value.StartsWith("=")))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private Syncfusion.RDL.DOM.TablixRowHierarchy CreateTablixRowHierarchy_Wizard(ControlDialog controlDialog)
        {
            if (controlDialog != null)
            {
                Syncfusion.RDL.DOM.TablixRowHierarchy tablixRowHierarchy = new Syncfusion.RDL.DOM.TablixRowHierarchy();

                // find whether Row Hierarchy has details Row or not
                bool hasDetails = HasDetailsInRowHier(controlDialog);
                tablixRowHierarchy.TablixMembers = GetTablixMembers_RowHier_Wizard(controlDialog, hasDetails);
                return tablixRowHierarchy;
            }
            else
            {
                return null;
            }
        }

        private Syncfusion.RDL.DOM.TablixCorner CreateTablixCorner_Wizard(ControlDialog controlDialog)
        {
            if (controlDialog != null && controlDialog.RowList != null && controlDialog.RowList.Count > 0
                && controlDialog.ColumnList != null && controlDialog.ColumnList.Count > 0
                && controlDialog.ValuesList != null && controlDialog.ValuesList.Count > 0)
            {
                Syncfusion.RDL.DOM.TablixCorner tablixCorner = new Syncfusion.RDL.DOM.TablixCorner();

                tablixCorner.TablixCornerRows = new Syncfusion.RDL.DOM.TablixCornerRows();
                int rowCount = controlDialog.ColumnList.Count;
                int colCount = controlDialog.RowList.Count;
                int valCount = controlDialog.ValuesList.Count;
                if (valCount > 1) rowCount++;
                for (int i = 0; i < rowCount; i++)
                {
                    Syncfusion.RDL.DOM.TablixCornerRow tablixCornerRow = new Syncfusion.RDL.DOM.TablixCornerRow();

                    if (i == rowCount - 1)
                    {
                        tablixCornerRow.TablixCornerCells = new RDL.DOM.TablixCornerCells();
                        for (int j = 0; j < colCount; j++)
                        {
                            string value = controlDialog.RowList[j];
                            if (value.StartsWith("=")) value = separateFieldfromAggFunction_Wizard(value);
                            tablixCornerRow.TablixCornerCells.Add(GetNewTablixCornerCell_Base(value));
                        }
                    }
                    else
                    {
                        tablixCornerRow.TablixCornerCells = new RDL.DOM.TablixCornerCells();
                        for (int j = 0; j < colCount; j++)
                        {
                            tablixCornerRow.TablixCornerCells.Add(GetNewTablixCornerCell_Base());
                        }
                    }

                    tablixCorner.TablixCornerRows.Add(tablixCornerRow);
                }

                return tablixCorner;
            }
            else
            {
                return null;
            }

        }

        private Syncfusion.RDL.DOM.TablixBody CreateTablixBody_Wizard(ControlDialog controlDialog)
        {
            if (controlDialog != null)
            {
                Syncfusion.RDL.DOM.TablixBody tablixBody = new Syncfusion.RDL.DOM.TablixBody();

                int colCount = 0;

                if (controlDialog.ValuesList != null && controlDialog.ValuesList.Count > 0)
                {
                    colCount = controlDialog.ValuesList.Count;
                }

                tablixBody.TablixColumns = new Syncfusion.RDL.DOM.TablixColumns();
                tablixBody.TablixRows = new Syncfusion.RDL.DOM.TablixRows();

                for (int i = 0; i < colCount; i++)
                {
                    Syncfusion.RDL.DOM.TablixColumn tablixColumn = new Syncfusion.RDL.DOM.TablixColumn();
                    tablixColumn.Width = new Syncfusion.RDL.DOM.Size(1 + "in");
                    tablixBody.TablixColumns.Add(tablixColumn);
                }

                // For header row, if only row hierarchy available
                if (!(controlDialog.ColumnList != null && controlDialog.ColumnList.Count > 0))
                {
                    Syncfusion.RDL.DOM.TablixRow headerRow = new Syncfusion.RDL.DOM.TablixRow();

                    headerRow.TablixCells = new Syncfusion.RDL.DOM.TablixCells();
                    headerRow.Height = new Syncfusion.RDL.DOM.Size(0.25 + "in");

                    for (int i = 0; i < colCount; i++)
                    {
                        Syncfusion.RDL.DOM.TablixCell tablixCell = new Syncfusion.RDL.DOM.TablixCell();
                        string value = controlDialog.ValuesList[i];
                        if (value.StartsWith("=")) value = separateFieldfromAggFunction_Wizard(value);
                        tablixCell.CellContents = GetNewCellContents_Base(TablixRegion.TablixBody, value, false, 0);
                        headerRow.TablixCells.Add(tablixCell);
                    }

                    tablixBody.TablixRows.Add(headerRow);
                }

                Syncfusion.RDL.DOM.TablixRow bodyRow = new Syncfusion.RDL.DOM.TablixRow();
                bodyRow.TablixCells = new Syncfusion.RDL.DOM.TablixCells();
                bodyRow.Height = new Syncfusion.RDL.DOM.Size(0.25 + "in");

                for (int i = 0; i < colCount; i++)
                {
                    Syncfusion.RDL.DOM.TablixCell tablixCell = new Syncfusion.RDL.DOM.TablixCell();
                    string value = controlDialog.ValuesList[i];
                    if (value.StartsWith("=")) value = value.Substring(1);
                    value = "[" + value + "]";
                    tablixCell.CellContents = GetNewCellContents_Base(TablixRegion.TablixBody, value, true, 0);
                    bodyRow.TablixCells.Add(tablixCell);
                }
                tablixBody.TablixRows.Add(bodyRow);
                return tablixBody;
            }
            else
            {
                return null;
            }
        }


        private Syncfusion.RDL.DOM.TablixColumnHierarchy CreateTablixColumn_Hierarchy_Wizard(ControlDialog controlDialog)
        {
            if (controlDialog != null)
            {
                string value;
                Syncfusion.RDL.DOM.TablixColumnHierarchy tablixColumnHierarchy = new Syncfusion.RDL.DOM.TablixColumnHierarchy();

                Syncfusion.RDL.DOM.TablixMembers tablixMembers = new Syncfusion.RDL.DOM.TablixMembers();

                if (controlDialog.ColumnList != null && controlDialog.ColumnList.Count > 0)
                {

                    // find whether the value list has more than one values or not, and add the lead TablixMembers
                    if (controlDialog.ValuesList != null && controlDialog.ValuesList.Count > 1)
                    {
                        int valueCount = controlDialog.ValuesList.Count;

                        // Header Row   
                        for (int i = 0; i < valueCount; i++)
                        {
                            Syncfusion.RDL.DOM.TablixMember tablixMember = new Syncfusion.RDL.DOM.TablixMember();
                            value = controlDialog.ValuesList[i];
                            if (value.StartsWith("=")) value = separateFieldfromAggFunction_Wizard(value);
                            tablixMember.TablixHeader = GetNewTablixHeader_Base(TablixHierarchyType.TablixColumnHierarchy, value, false, 0);

                            if (i == 0)
                            {
                                Syncfusion.RDL.DOM.TablixMembers tms = new Syncfusion.RDL.DOM.TablixMembers();
                                tms.Add(new Syncfusion.RDL.DOM.TablixMember());
                                tablixMember.TablixMembers = tms;
                                //tablixMember.TablixMembers = new Syncfusion.RDL.DOM.TablixMembers();
                                //tablixMember.TablixMembers.Add(new Syncfusion.RDL.DOM.TablixMember());
                            }

                            tablixMembers.Add(tablixMember);
                        }
                    }


                    int colHierCount = controlDialog.ColumnList.Count;


                    // Group Row  (second row)
                    for (int i = colHierCount - 1; i >= 0; i--)
                    {
                        Syncfusion.RDL.DOM.TablixMembers temp = new Syncfusion.RDL.DOM.TablixMembers();

                        Syncfusion.RDL.DOM.TablixMember tablixMember = new Syncfusion.RDL.DOM.TablixMember();
                        value = controlDialog.ColumnList[i];
                        tablixMember.Group = getGroupField_Wizard(value);
                        if (value.StartsWith("=")) value = value.Substring(1);
                        value = "[" + value + "]";
                        tablixMember.TablixHeader = GetNewTablixHeader_Base(TablixHierarchyType.TablixColumnHierarchy, value, true, 0);
                        //tablixMember.TablixMembers = new Syncfusion.RDL.DOM.TablixMembers();

                        if (i == colHierCount - 1 && controlDialog.ValuesList != null && controlDialog.ValuesList.Count == 1)
                        {
                            tablixMembers.Add(new Syncfusion.RDL.DOM.TablixMember());
                        }

                        tablixMember.TablixMembers = tablixMembers;

                        temp.Add(tablixMember);

                        tablixMembers = temp;
                    }
                }
                else
                {
                    if (controlDialog.ValuesList != null && controlDialog.ValuesList.Count > 0)
                    {
                        int valueCount = controlDialog.ValuesList.Count;

                        for (int i = 0; i < valueCount; i++)
                        {
                            tablixMembers.Add(new Syncfusion.RDL.DOM.TablixMember());
                        }
                    }
                }


                tablixColumnHierarchy.TablixMembers = tablixMembers;
                return tablixColumnHierarchy;
            }
            else
            {
                return null;
            }
        }

        public Syncfusion.RDL.DOM.TablixHeader GetNewTablixHeader_Base(TablixHierarchyType hierType, string field, bool isGroup, int row)
        {
            Syncfusion.RDL.DOM.TablixHeader tablixHeader = new Syncfusion.RDL.DOM.TablixHeader();

            if (hierType == TablixHierarchyType.TablixRowHierarchy)
            {
                tablixHeader.CellContents = GetNewCellContents_Base(TablixRegion.TablixRowHierarchy, field, isGroup, row);
                tablixHeader.Size = new Syncfusion.RDL.DOM.Size(1 + "in");
            }
            else
            {
                tablixHeader.CellContents = GetNewCellContents_Base(TablixRegion.TablixColumnHierarchy, field, isGroup, row);
                tablixHeader.Size = new Syncfusion.RDL.DOM.Size(0.25 + "in");
            }
            return tablixHeader;
        }

        /// <summary>
        /// Create a Row Hierarchy Tablix Members
        /// </summary>
        /// <param name="controlDialog"></param>
        /// <param name="hasDetails"></param>
        /// <returns></returns>
        private Syncfusion.RDL.DOM.TablixMembers GetTablixMembers_RowHier_Wizard(ControlDialog controlDialog, bool hasDetails)
        {
            if (controlDialog != null)
            {
                Syncfusion.RDL.DOM.TablixMembers tablixMembers = new Syncfusion.RDL.DOM.TablixMembers();

                int rowHierCount = 0;
                bool hasColumnHier = false;

                if (controlDialog.ColumnList != null && controlDialog.ColumnList.Count > 0)
                {
                    hasColumnHier = true;
                }

                Syncfusion.RDL.DOM.TablixMember tablixMem1 = new Syncfusion.RDL.DOM.TablixMember();
                Syncfusion.RDL.DOM.TablixMember tablixMem2 = new Syncfusion.RDL.DOM.TablixMember();
                if (hasDetails)
                {
                    tablixMem2.Group = new Syncfusion.RDL.DOM.Group();
                    tablixMem2.Group.Name = "Detalis" + detailsId++;
                }
                if (controlDialog.RowList != null && controlDialog.RowList.Count > 0)
                {
                    rowHierCount = controlDialog.RowList.Count;
                    string value;

                    if (hasColumnHier == false)
                    {
                        // Header Row   (First row)
                        for (int i = rowHierCount - 1; i >= 0; i--)
                        {
                            Syncfusion.RDL.DOM.TablixMember tablixMember = new Syncfusion.RDL.DOM.TablixMember();
                            value = controlDialog.RowList[i];
                            if (value.StartsWith("=")) value = separateFieldfromAggFunction_Wizard(value);
                            tablixMember.TablixHeader = GetNewTablixHeader_Base(TablixHierarchyType.TablixRowHierarchy, value, true, 0);

                            Syncfusion.RDL.DOM.TablixMembers tms = new Syncfusion.RDL.DOM.TablixMembers();
                            tms.Add(tablixMem1);
                            tablixMember.TablixMembers = tms;
                            //tablixMember.TablixMembers = new Syncfusion.RDL.DOM.TablixMembers();
                            //tablixMember.TablixMembers.Add(tablixMem1);
                            tablixMem1 = tablixMember;
                        }

                        if (hasDetails)
                        {
                            tablixMem1.KeepWithGroup = RDL.DOM.KeepWithGroup.After;
                        }
                    }

                    // Group Row  (second row)
                    for (int i = rowHierCount - 1; i >= 0; i--)
                    {
                        Syncfusion.RDL.DOM.TablixMember tablixMember = new Syncfusion.RDL.DOM.TablixMember();
                        value = controlDialog.RowList[i];
                        tablixMember.Group = getGroupField_Wizard(value);
                        if (value.StartsWith("=")) value = value.Substring(1);
                        value = "[" + value + "]";
                        tablixMember.TablixHeader = GetNewTablixHeader_Base(TablixHierarchyType.TablixRowHierarchy, value, true, 0);

                        Syncfusion.RDL.DOM.TablixMembers tms = new Syncfusion.RDL.DOM.TablixMembers();
                        tms.Add(tablixMem2);
                        tablixMember.TablixMembers = tms;

                        //tablixMember.TablixMembers = new Syncfusion.RDL.DOM.TablixMembers();
                        //tablixMember.TablixMembers.Add(tablixMem2);
                        tablixMem2 = tablixMember;
                    }
                }

                if (hasColumnHier == false)
                {
                    tablixMembers.Add(tablixMem1);
                }

                tablixMembers.Add(tablixMem2);
                return tablixMembers;
            }
            else
            {
                return null;
            }
        }


        private string separateFieldfromAggFunction_Wizard(string value)
        {
            if (value != null && value != string.Empty)
            {
                if (value.StartsWith("=") && value.Contains("("))
                {
                    int index = value.IndexOf("(");
                    value = value.Substring(index + 1);
                    value = value.Substring(0, value.Length - 1);
                }
            }
            return value;
        }


        /// <summary>
        /// Return the Group Member with expressions.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private Syncfusion.RDL.DOM.Group getGroupField_Wizard(string value)
        {
            if (value != null && value != string.Empty)
            {
                Syncfusion.RDL.DOM.Group group = new Syncfusion.RDL.DOM.Group();
                group.Name = "Group" + groupID++;

                Syncfusion.RDL.DOM.GroupExpressions groupExps = new Syncfusion.RDL.DOM.GroupExpressions();


                //group.GroupExpressions = new Syncfusion.RDL.DOM.GroupExpressions();
                Syncfusion.RDL.DOM.GroupExpression groupExpression = new Syncfusion.RDL.DOM.GroupExpression();

                // For aggregate Function
                if (value.StartsWith("=") && value.Contains("("))
                {
                    int index = value.IndexOf("(");
                    if (value.StartsWith("=Sum", true, null))
                    {
                        value = value.Substring(index + 1);
                        value = value.Substring(0, value.Length - 1);
                        value = TextBoxControl.FieldConverter(value, PlaceHolderType.Sum, null);
                    }
                    // if it is count
                    else if (value.StartsWith("=Count", true, null))
                    {
                        value = value.Substring(index + 1);
                        value = value.Substring(0, value.Length - 1);
                        value = TextBoxControl.FieldConverter(value, PlaceHolderType.Count, null);
                    }
                    // if it is First
                    else if (value.StartsWith("=First", true, null))
                    {
                        value = value.Substring(index + 1);
                        value = value.Substring(0, value.Length - 1);
                        value = TextBoxControl.FieldConverter(value, PlaceHolderType.First, null);
                    }
                    else if (value.StartsWith("=Avg", true, null))
                    {
                        value = value.Substring(index + 1);
                        value = value.Substring(0, value.Length - 1);
                        value = TextBoxControl.FieldConverter(value, PlaceHolderType.Avg, null);
                    }

                }
                else // For ordinary field
                {
                    value = TextBoxControl.FieldConverter(value, PlaceHolderType.Field, null);
                }

                groupExpression.Value = value;

                groupExps.Add(groupExpression);
                group.GroupExpressions = groupExps;

                //group.GroupExpressions.Add(groupExpression);
                return group;
            }
            else
            {
                return null;
            }
        }

        public Syncfusion.RDL.DOM.TablixCornerCell GetNewTablixCornerCell_Base()
        {
            Syncfusion.RDL.DOM.TablixCornerCell tablixCornerCell = new Syncfusion.RDL.DOM.TablixCornerCell();
            tablixCornerCell.CellContents = GetNewCellContents_Base(TablixRegion.TablixCorner, "", false, 0);
            return tablixCornerCell;
        }

        public Syncfusion.RDL.DOM.TablixCornerCell GetNewTablixCornerCell_Base(string field)
        {
            Syncfusion.RDL.DOM.TablixCornerCell tablixCornerCell = new Syncfusion.RDL.DOM.TablixCornerCell();
            tablixCornerCell.CellContents = GetNewCellContents_Base(TablixRegion.TablixCorner, field, false, 0);
            return tablixCornerCell;
        }

        public Syncfusion.RDL.DOM.CellContents GetNewCellContents_Base(TablixRegion tablixRegion, string field, bool isGroup, int row)
        {
            Syncfusion.RDL.DOM.CellContents cellContents = new Syncfusion.RDL.DOM.CellContents();

            cellContents.ReportItem = PopulateTextbox(field, isGroup);


            return cellContents;
        }

        public Syncfusion.RDL.DOM.TextBox PopulateTextbox(string content, bool fromGroup)
        {
            RDL.DOM.TextBox textbox = new RDL.DOM.TextBox();
            do
            {
                textbox.Name = "TextBox" + ++this.textBoxCount;
            } while (TablixNameAvailabilityCheck(textbox.Name));

            textbox.Style = new RDL.DOM.Style();
            textbox.Style.Border = new RDL.DOM.Border();
            textbox.Style.Border.Style = RDL.DOM.BorderStyles.Solid.ToString();
            textbox.Style.Border.Color = "#D3D3D3";
            textbox.Paragraphs = new RDL.DOM.Paragraphs();
            RDL.DOM.Paragraph paragraph = new RDL.DOM.Paragraph();
            paragraph.TextRuns = new RDL.DOM.TextRuns();
            RDL.DOM.TextRun run = new TextRun();

            run.Value = content;
            if (!content.StartsWith("="))
            {
                if (fromGroup)
                {
                    content = Syncfusion.Windows.Reports.Sql.SqlUtil.RemoveQuoteIdentifier(content);
                    PlaceHolderType fieldType = PlaceHolderType.Field;

                    if (content != null)
                    {
                        if (content.StartsWith("Sum("))
                        {
                            content = content.Replace("Sum(", "");
                            content = content.Substring(0, content.Length - 1);
                            fieldType = PlaceHolderType.Sum;
                        }
                        else if (content.StartsWith("Count("))
                        {
                            content = content.Replace("Count(", "");
                            content = content.Substring(0, content.Length - 1);
                            fieldType = PlaceHolderType.Count;
                        }
                        else if (content.StartsWith("Avg("))
                        {
                            content = content.Replace("Avg(", "");
                            content = content.Substring(0, content.Length - 1);
                            fieldType = PlaceHolderType.Avg;
                        }
                    }

                    run.Value = TextBoxControl.FieldConverter(content, fieldType, null);
                }
            }
            run.Style = new Base.Style();
            paragraph.TextRuns.Add(run);
            paragraph.Style = new Base.Style();
            textbox.Paragraphs.Add(paragraph);
            textbox.KeepTogether = true;
            return textbox;
        }

        #endregion

        void UpdateStyle()
        {
            if (this.bodyCanvas != null)
            {
                DesignPanelStyle style = (DesignPanelStyle)this.GridDesignPanel.Resources["DefaultStyle"];

                switch (this.VisualStyle)
                {
                    case "Metro":
                        {
                            style = (DesignPanelStyle)this.GridDesignPanel.Resources["MetroStyle"];
                            break;
                        }
                    case "Office2003":
                        {
                            style = (DesignPanelStyle)this.GridDesignPanel.Resources["Office2003Style"];
                            break;
                        }
                    case "Office2007Blue":
                        {
                            style = (DesignPanelStyle)this.GridDesignPanel.Resources["Office2007BlueStyle"];
                            break;
                        }
                    case "Office2007Black":
                        {
                            style = (DesignPanelStyle)this.GridDesignPanel.Resources["Office2007BlackStyle"];
                            break;
                        }
                    case "Office2007Silver":
                        {
                            style = (DesignPanelStyle)this.GridDesignPanel.Resources["Office2007SilverStyle"];
                            break;
                        }
                    case "ShinyRed":
                        {
                            style = (DesignPanelStyle)this.GridDesignPanel.Resources["ShinyRedStyle"];
                            break;
                        }
                    case "Blend":
                        {
                            style = (DesignPanelStyle)this.GridDesignPanel.Resources["BlendStyle"];
                            break;
                        }
                    case "ShinyBlue":
                        {
                            style = (DesignPanelStyle)this.GridDesignPanel.Resources["ShinyBlueStyle"];
                            break;
                        }
                    case "VS2010":
                        {
                            style = (DesignPanelStyle)this.GridDesignPanel.Resources["VS2010Style"];
                            break;
                        }
                    case "Office2010Blue":
                        {
                            style = (DesignPanelStyle)this.GridDesignPanel.Resources["Office2010BlueStyle"];
                            break;
                        }
                    case "Office2010Black":
                        {
                            style = (DesignPanelStyle)this.GridDesignPanel.Resources["Office2010BlackStyle"];
                            break;
                        }
                    case "Office2010Silver":
                        {
                            style = (DesignPanelStyle)this.GridDesignPanel.Resources["Office2010SilverStyle"];
                            break;
                        }
                    case "Transparent":
                        {
                            style = (DesignPanelStyle)this.GridDesignPanel.Resources["TransparentStyle"];
                            break;
                        }
                    case "SyncOrange":
                        {
                            style = (DesignPanelStyle)this.GridDesignPanel.Resources["SyncOrangeStyle"];
                            break;
                        }
                    default:
                        {
                            style = (DesignPanelStyle)this.GridDesignPanel.Resources["Default"];
                            break;
                        }
                }

                this.DesignerArea.Background = style.DesignerAreaBackGround;

                this.GridVertialRuler.Background = style.RulerAreaBackGround;
                this.GridHorizontalRuler.Background = style.RulerAreaBackGround;
                this.GridCorner.Background = style.RulerAreaBackGround;

                this.UpdateRulerStyle(this.RulerHorizontal, style);
                this.UpdateRulerStyle(this.RulerVerticalHeader, style);
                this.UpdateRulerStyle(this.RulerVerticalBody, style);
                this.UpdateRulerStyle(this.RulerVerticalFooter, style);
            }
        }

        void UpdateRulerStyle(Ruler ruler, DesignPanelStyle style)
        {
            ruler.BackGround = style.RulerBackGround;
            ruler.BorderColor = style.RulerBorderColor;
            ruler.IndicatorColor = style.RulerIndicatorColor;
            ruler.TextColor = style.RulerTextColor;
            ruler.TickColor = style.RulerTickColor;
        }

        private void UpdatePanelZoom()
        {
            if (this.DesignerArea != null)
            {
                ScaleTransform transform = new ScaleTransform();
                transform.ScaleX = this.ZoomFactor / 100;
                transform.ScaleY = this.ZoomFactor / 100;
                this.DesignerArea.LayoutTransform = transform;
                this.GridCorner.LayoutTransform = transform;
                this.GridHorizontalRuler.LayoutTransform = transform;
                this.GridVertialRuler.LayoutTransform = transform;
            }
        }

        private void RulerChipHide()
        {
            RulerHorizontal.Chip = -1;
            RulerVerticalHeader.Chip = -1;
            RulerVerticalBody.Chip = -1;
            RulerVerticalFooter.Chip = -1;
        }

        internal void SetHeaderVisibility()
        {
            this.IsHeaderVisible = !this.IsHeaderVisible;
        }

        internal void SetHeaderVisibility(bool set)
        {
            if (set)
            {
                this.headerRowDefination.MinHeight = this.MinHeaderheight;
                this.headerRowDefination.Height = new GridLength(new RDL.DOM.Size(this.headerproperties.HeaderHeight).PixelValue);
                this.RulerVerticalHeader.Length = converter.GetRulerWidthValue(this.headerproperties.HeaderHeight);

                if (!this.IsInternalChange)
                {
                    EditAction action = new EditAction();
                    action.EditingType = EditActionType.HeaderVisiblity;
                    action.HeaderVisiblity = set;
                    this.EditingManager.AddAction(action);
                }
            }
            else
            {
                if (!this.IsInternalChange)
                {
                    EditAction action = new EditAction();
                    action.EditingType = EditActionType.HeaderVisiblity;
                    action.HeaderVisiblity = set;
                    this.EditingManager.AddAction(action);
                    this.EditingManager.IsMergeAction = true;
                }

                this.headerRowDefination.MinHeight = 0;
                this.headerRowDefination.Height = new GridLength(0);
                this.RemoveReportItems(this.headerCanvas.Children.OfType<IReportItemControl>().ToList());

                this.EditingManager.IsMergeAction = false;
            }
        }

        internal void SetFooterVisibility()
        {
            this.IsFooterVisible = !this.IsFooterVisible;
        }

        internal void SetFooterVisibility(bool set)
        {
            if (set)
            {
                this.footerRowDefination.MinHeight = this.MinFooterHeight;
                this.footerRowDefination.Height = new GridLength(new RDL.DOM.Size(this.footerproperties.FooterHeight).PixelValue);

                if (!this.IsInternalChange)
                {
                    EditAction action = new EditAction();
                    action.EditingType = EditActionType.FooterVisiblity;
                    action.FooterVisiblity = set;
                    this.EditingManager.AddAction(action);
                }
            }
            else
            {
                if (!this.IsInternalChange)
                {
                    EditAction action = new EditAction();
                    action.EditingType = EditActionType.FooterVisiblity;
                    action.FooterVisiblity = set;
                    this.EditingManager.AddAction(action);
                    this.EditingManager.IsMergeAction = true;
                }

                this.footerRowDefination.MinHeight = 0;
                this.footerRowDefination.Height = new GridLength(0);
                this.RemoveReportItems(this.footerCanvas.Children.OfType<IReportItemControl>().ToList());

                this.EditingManager.IsMergeAction = false;
            }
        }

        internal ReportDefinition GetReportDefinition()
        {
            this.Report.EmbeddedImages = this.EmbeddedImages;
            this.Report.ReportParameters = this.ReportParameters;
            this.Report.DataSets = this.DataSets;
            this.Report.DataSources = this.DataSources;
            this.Report.Author = this.reportproperties.Author;
            this.Report.Description = this.reportproperties.Description;
            this.Report.AutoRefresh = this.reportproperties.AutoRefresh;
  
            if (this.reportproperties.DataElementName != null)
            {
                this.Report.DataElementName = this.reportproperties.DataElementName;
            }
            if (this.reportproperties.DataElementStyle != "Auto")
            {
                this.Report.DataElementStyle = (RDL.DOM.DataElementStyles)Enum.Parse(typeof(RDL.DOM.DataElementStyles), this.reportproperties.DataElementStyle);
            }
            if (this.reportproperties.DataSchema != null)
            {
                this.Report.DataSchema = this.reportproperties.DataSchema;
            }
            if (this.reportproperties.DataTransform != null)
            {
                this.Report.DataSchema = this.reportproperties.DataSchema;
            }

            Base.Page page = new Base.Page();
            page.Style = new Base.Style();
            page.Style.BackgroundColor = this.reportproperties.BackgroundColor;

            if (this.reportproperties.Margins != null)
            {
                if (this.reportproperties.Margins.LeftMargin != null)
                {
                    page.LeftMargin = new RDL.DOM.Size(this.reportproperties.Margins.LeftMargin);
                }
                if (this.reportproperties.Margins.RightMargin != null)
                {
                    page.RightMargin = new RDL.DOM.Size(this.reportproperties.Margins.RightMargin);
                }
                if (this.reportproperties.Margins.TopMargin != null)
                {
                    page.TopMargin = new RDL.DOM.Size(this.reportproperties.Margins.TopMargin);
                }
                if (this.reportproperties.Margins.BottomMargin != null)
                {
                    page.BottomMargin = new RDL.DOM.Size(this.reportproperties.Margins.BottomMargin);
                }
            }
            if (!string.IsNullOrEmpty(this.reportproperties.BackgroundImage.ImageValue))
            {
                page.Style.BackgroundImage = new RDL.DOM.BackgroundImage();
                page.Style.BackgroundImage.Source = this.reportproperties.BackgroundImage.Source;
                page.Style.BackgroundImage.Value = this.reportproperties.BackgroundImage.ImageValue;
                if (this.reportproperties.BackgroundImage.MIMEType != null)
                    page.Style.BackgroundImage.MIMEType = this.reportproperties.BackgroundImage.MIMEType;
            }

            page.Style.Border = new RDL.DOM.Border();
            page.Style.Border.Style = this.reportproperties.BorderStyles.DefaultBorderStyle;
            page.Style.Border.Width = this.reportproperties.BorderWidths.DefaultBorderWidth;
            page.Style.Border.Color = this.reportproperties.BorderColors.DefaultBorderColor;

            if (this.reportproperties.BorderWidths.LeftBorderWidth != null || this.reportproperties.BorderStyles.LeftBorderStyle != null 
                || this.reportproperties.BorderColors.LeftBorderColor!=null)
            {
                page.Style.LeftBorder = new RDL.DOM.LeftBorder();
                page.Style.LeftBorder.Color = this.reportproperties.BorderColors.LeftBorderColor;
                page.Style.LeftBorder.Width = this.reportproperties.BorderWidths.LeftBorderWidth;
                page.Style.LeftBorder.Style = this.reportproperties.BorderStyles.LeftBorderStyle;
            }
            if (this.reportproperties.BorderWidths.RightBorderWidth != null || this.reportproperties.BorderStyles.RightBorderStyle != null
                || this.reportproperties.BorderColors.RightBorderColor != null)
            {
                page.Style.RightBorder = new RDL.DOM.RightBorder();
                page.Style.RightBorder.Color = this.reportproperties.BorderColors.RightBorderColor;
                page.Style.RightBorder.Width = this.reportproperties.BorderWidths.RightBorderWidth;
                page.Style.RightBorder.Style = this.reportproperties.BorderStyles.RightBorderStyle;
            }
            if (this.reportproperties.BorderWidths.TopBorderWidth != null || this.reportproperties.BorderStyles.TopBorderStyle != null
                || this.reportproperties.BorderColors.TopBorderColor != null)
            {
                page.Style.TopBorder = new RDL.DOM.TopBorder();
                page.Style.TopBorder.Color = this.reportproperties.BorderColors.TopBorderColor;
                page.Style.TopBorder.Width = this.reportproperties.BorderWidths.TopBorderWidth;
                page.Style.TopBorder.Style = this.reportproperties.BorderStyles.TopBorderStyle;
            }
            if (this.reportproperties.BorderWidths.BottomBorderWidth != null || this.reportproperties.BorderStyles.BottomBorderStyle != null
                || this.reportproperties.BorderColors.BottomBorderColor != null)
            {
                page.Style.BottomBorder = new RDL.DOM.BottomBorder();
                page.Style.BottomBorder.Color = this.reportproperties.BorderColors.BottomBorderColor;
                page.Style.BottomBorder.Width = this.reportproperties.BorderWidths.BottomBorderWidth;
                page.Style.BottomBorder.Style = this.reportproperties.BorderStyles.BottomBorderStyle;
            }

            if (this.IsHeaderVisible)
            {
                page.PageHeader = new PageHeader();
                page.PageHeader.Height = new Base.Size(this.headerproperties.HeaderHeight);
                RDL.DOM.Style style = new Base.Style();

                if (style.BackgroundImage == null && !string.IsNullOrEmpty(this.headerproperties.BackgroundImage.ImageValue))
                {
                    style.BackgroundImage = new RDL.DOM.BackgroundImage();
                    style.BackgroundImage.Value = this.headerproperties.BackgroundImage.ImageValue;
                    style.BackgroundImage.Source = this.headerproperties.BackgroundImage.Source;
                    if (this.headerproperties.BackgroundImage.MIMEType != null)
                        style.BackgroundImage.MIMEType = this.headerproperties.BackgroundImage.MIMEType;
                }

                style.BackgroundColor = this.headerproperties.BackgroundColor;
                style.Border = new RDL.DOM.Border();
                style.Border.Style = this.headerproperties.BorderStyles.DefaultBorderStyle;
                style.Border.Width = this.headerproperties.BorderWidths.DefaultBorderWidth;
                style.Border.Color = this.headerproperties.BorderColors.DefaultBorderColor;

                if (this.headerproperties.BorderWidths.LeftBorderWidth != null || this.headerproperties.BorderStyles.LeftBorderStyle != null
                || this.headerproperties.BorderColors.LeftBorderColor != null)
                {
                    style.LeftBorder = new RDL.DOM.LeftBorder();
                    style.LeftBorder.Color = this.headerproperties.BorderColors.LeftBorderColor;
                    style.LeftBorder.Width = this.headerproperties.BorderWidths.LeftBorderWidth;
                    style.LeftBorder.Style = this.headerproperties.BorderStyles.LeftBorderStyle;
                }
                if (this.headerproperties.BorderWidths.RightBorderWidth != null || this.headerproperties.BorderStyles.RightBorderStyle != null
                || this.headerproperties.BorderColors.RightBorderColor != null)
                {
                    style.RightBorder = new RDL.DOM.RightBorder();
                    style.RightBorder.Color = this.headerproperties.BorderColors.RightBorderColor;
                    style.RightBorder.Width = this.headerproperties.BorderWidths.RightBorderWidth;
                    style.RightBorder.Style = this.headerproperties.BorderStyles.RightBorderStyle;
                }
                if (this.headerproperties.BorderWidths.TopBorderWidth != null || this.headerproperties.BorderStyles.TopBorderStyle != null
                || this.headerproperties.BorderColors.TopBorderColor != null)
                {
                    style.TopBorder = new RDL.DOM.TopBorder();
                    style.TopBorder.Color = this.headerproperties.BorderColors.TopBorderColor;
                    style.TopBorder.Width = this.headerproperties.BorderWidths.TopBorderWidth;
                    style.TopBorder.Style = this.headerproperties.BorderStyles.TopBorderStyle;
                }
                if (this.headerproperties.BorderWidths.BottomBorderWidth != null || this.headerproperties.BorderStyles.BottomBorderStyle != null
                || this.headerproperties.BorderColors.BottomBorderColor != null)
                {
                    style.BottomBorder = new RDL.DOM.BottomBorder();
                    style.BottomBorder.Color = this.headerproperties.BorderColors.BottomBorderColor;
                    style.BottomBorder.Width = this.headerproperties.BorderWidths.BottomBorderWidth;
                    style.BottomBorder.Style = this.headerproperties.BorderStyles.BottomBorderStyle;
                }

                page.PageHeader.PrintOnFirstPage = Convert.ToBoolean(this.headerproperties.PrintOnFirstPage);
                page.PageHeader.PrintOnLastPage = Convert.ToBoolean(this.headerproperties.PrintOnLastPage);
                page.PageHeader.Style = style;
                page.PageHeader.ReportItems = this.GetReportItems(this.headerCanvas);
            }

            if (this.IsFooterVisible)
            {
                page.PageFooter = new PageFooter();
                page.PageFooter.Height = new Base.Size(this.footerproperties.FooterHeight);
                RDL.DOM.Style style = new Base.Style();

                if (style.BackgroundImage == null && !string.IsNullOrEmpty(this.footerproperties.BackgroundImage.ImageValue))
                {
                    style.BackgroundImage = new RDL.DOM.BackgroundImage();
                    style.BackgroundImage.Value = this.footerproperties.BackgroundImage.ImageValue;
                    style.BackgroundImage.Source = this.footerproperties.BackgroundImage.Source;
                    if (this.footerproperties.BackgroundImage.MIMEType != null)
                        style.BackgroundImage.MIMEType = this.footerproperties.BackgroundImage.MIMEType;
                }

                style.BackgroundColor = this.footerproperties.BackgroundColor;
                style.Border = new RDL.DOM.Border();
                style.Border.Style = this.footerproperties.BorderStyles.DefaultBorderStyle;
                style.Border.Width = this.footerproperties.BorderWidths.DefaultBorderWidth;
                style.Border.Color = this.footerproperties.BorderColors.DefaultBorderColor;

                if (this.footerproperties.BorderWidths.LeftBorderWidth != null || this.footerproperties.BorderStyles.LeftBorderStyle != null
                || this.footerproperties.BorderColors.LeftBorderColor != null)
                {
                    style.LeftBorder = new RDL.DOM.LeftBorder();
                    style.LeftBorder.Color = this.footerproperties.BorderColors.LeftBorderColor;
                    style.LeftBorder.Width = this.footerproperties.BorderWidths.LeftBorderWidth;
                    style.LeftBorder.Style = this.footerproperties.BorderStyles.LeftBorderStyle;
                }
                if (this.footerproperties.BorderWidths.RightBorderWidth != null || this.footerproperties.BorderStyles.RightBorderStyle != null
                || this.footerproperties.BorderColors.RightBorderColor != null)
                {
                    style.RightBorder = new RDL.DOM.RightBorder();
                    style.RightBorder.Color = this.footerproperties.BorderColors.RightBorderColor;
                    style.RightBorder.Width = this.footerproperties.BorderWidths.RightBorderWidth;
                    style.RightBorder.Style = this.footerproperties.BorderStyles.RightBorderStyle;
                }
                if (this.footerproperties.BorderWidths.TopBorderWidth != null || this.footerproperties.BorderStyles.TopBorderStyle != null
                || this.footerproperties.BorderColors.TopBorderColor != null)
                {
                    style.TopBorder = new RDL.DOM.TopBorder();
                    style.TopBorder.Color = this.footerproperties.BorderColors.TopBorderColor;
                    style.TopBorder.Width = this.footerproperties.BorderWidths.TopBorderWidth;
                    style.TopBorder.Style = this.footerproperties.BorderStyles.TopBorderStyle;
                }
                if (this.footerproperties.BorderWidths.BottomBorderWidth != null || this.footerproperties.BorderStyles.BottomBorderStyle != null
                || this.footerproperties.BorderColors.BottomBorderColor != null)
                {
                    style.BottomBorder = new RDL.DOM.BottomBorder();
                    style.BottomBorder.Color = this.footerproperties.BorderColors.BottomBorderColor;
                    style.BottomBorder.Width = this.footerproperties.BorderWidths.BottomBorderWidth;
                    style.BottomBorder.Style = this.footerproperties.BorderStyles.BottomBorderStyle;
                }

                page.PageFooter.PrintOnFirstPage = Convert.ToBoolean(this.footerproperties.PrintOnFirstPage);
                page.PageFooter.PrintOnLastPage = Convert.ToBoolean(this.footerproperties.PrintOnLastPage);
                page.PageFooter.Style = style;
                page.PageFooter.ReportItems = this.GetReportItems(this.footerCanvas);
            }

            page.PageWidth = new Base.Size(this.reportproperties.PageWidth);
            page.PageHeight = new Base.Size(this.reportproperties.PageHeight);

            RDL.DOM.Body body = this.Report.Body;

            if (this.Report.RDLType == RDLType.RDL2010)
            {
                body = this.Report.ReportSections.First().Body;
            }

            if (this.Report.RDLType == RDLType.RDL2010)
            {
                this.Report.Width = null;
                this.Report.ReportSections = new ReportSections();
                ReportSection section = new ReportSection();
                section.Page = page;
                section.Width = new Base.Size(this.bodyproperties.ReportWidth);
                this.Report.ReportSections.Add(section);
                section.Body = body;
            }
            else
            {
                this.Report.Page = page;
                this.Report.Width = new Base.Size(this.bodyproperties.ReportWidth);
                this.Report.Body = body;
            }

            body.Height = new Base.Size(this.bodyproperties.BodyHeight);
            body.ReportItems = this.GetReportItems(this.bodyCanvas);

            if (body.Style == null)
            {
                body.Style = new Base.Style();
            }
            if (body.Style.BackgroundImage == null && !string.IsNullOrEmpty(this.bodyproperties.BackgroundImage.ImageValue))
            {
                body.Style.BackgroundImage = new RDL.DOM.BackgroundImage();
                body.Style.BackgroundImage.Value = this.bodyproperties.BackgroundImage.ImageValue;
                body.Style.BackgroundImage.Source = this.bodyproperties.BackgroundImage.Source;
                if (this.bodyproperties.BackgroundImage.MIMEType != null)
                    body.Style.BackgroundImage.MIMEType = this.bodyproperties.BackgroundImage.MIMEType;
            }

            body.Style.BackgroundColor = this.bodyproperties.BackgroundColor;
            body.Style.Border = new RDL.DOM.Border();
            body.Style.Border.Style = this.bodyproperties.BorderStyles.DefaultBorderStyle;
            body.Style.Border.Width = this.bodyproperties.BorderWidths.DefaultBorderWidth;
            body.Style.Border.Color = this.bodyproperties.BorderColors.DefaultBorderColor;

            if (this.bodyproperties.BorderWidths.LeftBorderWidth != null || this.bodyproperties.BorderStyles.LeftBorderStyle != null
            || this.bodyproperties.BorderColors.LeftBorderColor != null)
            {
                body.Style.LeftBorder = new RDL.DOM.LeftBorder();
                body.Style.LeftBorder.Color = this.bodyproperties.BorderColors.LeftBorderColor;
                body.Style.LeftBorder.Width = this.bodyproperties.BorderWidths.LeftBorderWidth;
                body.Style.LeftBorder.Style = this.bodyproperties.BorderStyles.LeftBorderStyle;
            }
            if (this.bodyproperties.BorderWidths.RightBorderWidth != null || this.bodyproperties.BorderStyles.RightBorderStyle != null
            || this.bodyproperties.BorderColors.RightBorderColor != null)
            {
                body.Style.RightBorder = new RDL.DOM.RightBorder();
                body.Style.RightBorder.Color = this.bodyproperties.BorderColors.RightBorderColor;
                body.Style.RightBorder.Width = this.bodyproperties.BorderWidths.RightBorderWidth;
                body.Style.RightBorder.Style = this.bodyproperties.BorderStyles.RightBorderStyle;
            }
            if (this.bodyproperties.BorderWidths.TopBorderWidth != null || this.bodyproperties.BorderStyles.TopBorderStyle != null
            || this.bodyproperties.BorderColors.TopBorderColor != null)
            {
                body.Style.TopBorder = new RDL.DOM.TopBorder();
                body.Style.TopBorder.Color = this.bodyproperties.BorderColors.TopBorderColor;
                body.Style.TopBorder.Width = this.bodyproperties.BorderWidths.TopBorderWidth;
                body.Style.TopBorder.Style = this.bodyproperties.BorderStyles.TopBorderStyle;
            }
            if (this.bodyproperties.BorderWidths.BottomBorderWidth != null || this.bodyproperties.BorderStyles.BottomBorderStyle != null
            || this.bodyproperties.BorderColors.BottomBorderColor != null)
            {
                body.Style.BottomBorder = new RDL.DOM.BottomBorder();
                body.Style.BottomBorder.Color = this.bodyproperties.BorderColors.BottomBorderColor;
                body.Style.BottomBorder.Width = this.bodyproperties.BorderWidths.BottomBorderWidth;
                body.Style.BottomBorder.Style = this.bodyproperties.BorderStyles.BottomBorderStyle;
            }

            return this.Report;
        }

        internal List<ReportItemLocationInfo> GetReportItemLocationInfo(Canvas drawingArea, bool isResize)
        {
            List<ReportItemLocationInfo> reportItemsLocation = new List<ReportItemLocationInfo>();

            bool isHeaderCanvas = Object.ReferenceEquals(drawingArea, this.headerCanvas);
            bool isFooterCanvas = Object.ReferenceEquals(drawingArea, this.footerCanvas);
            bool isBodyCanvas = Object.ReferenceEquals(drawingArea, this.bodyCanvas);

            double bodyHeight = new RDL.DOM.Size(this.bodyproperties.BodyHeight).PixelValue;
            double headerHeight = new RDL.DOM.Size(this.headerproperties.HeaderHeight).PixelValue;
            double footerHeight = new RDL.DOM.Size(this.footerproperties.FooterHeight).PixelValue;
            double reportWith = new RDL.DOM.Size(this.bodyproperties.ReportWidth).PixelValue;

            if (this.IsHeaderVisible)
            {
                ReportItemLocationInfo headerlocationInfo = new ReportItemLocationInfo();
                headerlocationInfo.ItemLeft = 0;
                headerlocationInfo.ItemRight = reportWith;
                headerlocationInfo.ItemTop = 0;
                headerlocationInfo.ItemBottom = headerHeight;

                if (isFooterCanvas)
                {
                    headerlocationInfo.ItemTop = -headerHeight - bodyHeight;
                    headerlocationInfo.ItemBottom = -bodyHeight;
                }
                else if (isBodyCanvas)
                {
                    headerlocationInfo.ItemTop = -headerHeight;
                    headerlocationInfo.ItemBottom = 0;
                }

                foreach (var reportItemLocation in this.GetReportItemLocationInfo(this.headerCanvas, isResize, null))
                {
                    if (isFooterCanvas)
                    {
                        headerlocationInfo.ItemTop = -headerHeight - bodyHeight;
                        headerlocationInfo.ItemBottom = -bodyHeight;
                        reportItemLocation.ItemTop = (reportItemLocation.ItemTop - headerHeight) - bodyHeight;
                        reportItemLocation.ItemBottom = (reportItemLocation.ItemBottom - headerHeight) - bodyHeight;
                    }
                    else if (isBodyCanvas)
                    {
                        headerlocationInfo.ItemTop = -headerHeight;
                        headerlocationInfo.ItemBottom = 0;
                        reportItemLocation.ItemTop = reportItemLocation.ItemTop - headerHeight;
                        reportItemLocation.ItemBottom = reportItemLocation.ItemBottom - headerHeight;
                    }

                    if (!isResize && reportItemLocation.ReportItem is IReportItemControl && reportItemLocation.ReportItem.ItemType == DrawingReportItem.Rectangle && !reportItemLocation.IsSelected)
                    {
                        reportItemLocation.Bounds = new Rect(reportItemLocation.ReportItem.ItemLeft, reportItemLocation.ItemTop, reportItemLocation.ReportItem.ItemWidth, reportItemLocation.ReportItem.ItemHeight);
                        reportItemLocation.ReportItemArea = reportItemLocation.ReportItem as Canvas;
                    }
                    else
                    {
                        reportItemLocation.Bounds = Rect.Empty;
                    }
                    reportItemsLocation.Add(reportItemLocation);
                }

                reportItemsLocation.Add(headerlocationInfo);

                if (!isResize)
                {
                    headerlocationInfo.ReportItemArea = this.headerCanvas;
                    headerlocationInfo.Bounds = new Rect(new Point(headerlocationInfo.ItemLeft, headerlocationInfo.ItemTop), new Point(headerlocationInfo.ItemRight, headerlocationInfo.ItemBottom));
                }
            }

            if (this.IsFooterVisible)
            {
                ReportItemLocationInfo footerlocationInfo = new ReportItemLocationInfo();
                footerlocationInfo.ItemLeft = 0;
                footerlocationInfo.ItemRight = reportWith;
                footerlocationInfo.ItemTop = 0;
                footerlocationInfo.ItemBottom = footerHeight;

                if (isHeaderCanvas)
                {
                    footerlocationInfo.ItemTop = bodyHeight + headerHeight;
                    footerlocationInfo.ItemBottom = bodyHeight + headerHeight + footerHeight;
                }
                else if (isBodyCanvas)
                {
                    footerlocationInfo.ItemTop = bodyHeight;
                    footerlocationInfo.ItemBottom = bodyHeight + footerHeight;
                }

                foreach (var reportItemLocation in this.GetReportItemLocationInfo(this.footerCanvas, isResize, null))
                {
                    if (isHeaderCanvas)
                    {
                        reportItemLocation.ItemTop = reportItemLocation.ItemTop + bodyHeight + headerHeight;
                        reportItemLocation.ItemBottom = reportItemLocation.ItemBottom + bodyHeight + headerHeight;
                    }
                    else if (isBodyCanvas)
                    {
                        reportItemLocation.ItemTop = reportItemLocation.ItemTop + bodyHeight;
                        reportItemLocation.ItemBottom = reportItemLocation.ItemBottom + bodyHeight;
                    }

                    if (reportItemLocation.ReportItem is IReportItemControl && reportItemLocation.ReportItem.ItemType == DrawingReportItem.Rectangle && !reportItemLocation.IsSelected)
                    {
                        reportItemLocation.Bounds = new Rect(reportItemLocation.ReportItem.ItemLeft, reportItemLocation.ItemTop, reportItemLocation.ReportItem.ItemWidth, reportItemLocation.ReportItem.ItemHeight);
                        reportItemLocation.ReportItemArea = reportItemLocation.ReportItem as Canvas;
                    }
                    else
                    {
                        reportItemLocation.Bounds = Rect.Empty;
                    }

                    reportItemsLocation.Add(reportItemLocation);
                }

                reportItemsLocation.Add(footerlocationInfo);

                if (!isResize)
                {
                    footerlocationInfo.ReportItemArea = this.footerCanvas;
                    footerlocationInfo.Bounds = new Rect(new Point(footerlocationInfo.ItemLeft, footerlocationInfo.ItemTop), new Point(footerlocationInfo.ItemRight, footerlocationInfo.ItemBottom));
                }
            }

            ReportItemLocationInfo bodylocationInfo = new ReportItemLocationInfo();
            bodylocationInfo.ItemLeft = 0;
            bodylocationInfo.ItemRight = reportWith;
            bodylocationInfo.ItemTop = 0;
            bodylocationInfo.ItemBottom = bodyHeight;

            if (isHeaderCanvas)
            {
                bodylocationInfo.ItemTop = headerHeight;
                bodylocationInfo.ItemBottom = bodyHeight + headerHeight;
            }
            else if (isFooterCanvas)
            {
                bodylocationInfo.ItemTop = -bodyHeight;
                bodylocationInfo.ItemBottom = 0;
            }

            foreach (var reportItemLocation in this.GetReportItemLocationInfo(this.bodyCanvas, isResize, null))
            {
                if (isHeaderCanvas)
                {
                    reportItemLocation.ItemTop = reportItemLocation.ItemTop + headerHeight;
                    reportItemLocation.ItemBottom = reportItemLocation.ItemBottom + headerHeight;
                }
                else if (isFooterCanvas)
                {
                    reportItemLocation.ItemTop = reportItemLocation.ItemTop - bodyHeight;
                    reportItemLocation.ItemBottom = reportItemLocation.ItemBottom - bodyHeight;
                }

                if (reportItemLocation.ReportItem is IReportItemControl && reportItemLocation.ReportItem.ItemType == DrawingReportItem.Rectangle && !reportItemLocation.IsSelected)
                {
                    reportItemLocation.Bounds = new Rect(reportItemLocation.ItemLeft, reportItemLocation.ItemTop, reportItemLocation.ReportItem.ItemWidth, reportItemLocation.ReportItem.ItemHeight);
                    reportItemLocation.ReportItemArea = reportItemLocation.ReportItem as Canvas;
                }
                else
                {
                    reportItemLocation.Bounds = Rect.Empty;
                }

                reportItemsLocation.Add(reportItemLocation);
            }

            reportItemsLocation.Add(bodylocationInfo);

            if (!isResize)
            {
                bodylocationInfo.ReportItemArea = this.bodyCanvas;
                bodylocationInfo.Bounds = new Rect(new Point(bodylocationInfo.ItemLeft, bodylocationInfo.ItemTop), new Point(bodylocationInfo.ItemRight, bodylocationInfo.ItemBottom));
            }

            return reportItemsLocation;
        }

        internal List<ReportItemLocationInfo> GetReportItemLocationInfo(Canvas contentCanvas, bool isResize, ReportItemLocationInfo parentInfo)
        {
            List<ReportItemLocationInfo> reportItemsLocation = new List<ReportItemLocationInfo>();

            foreach (IReportItemControl reportItem in contentCanvas.Children.OfType<IReportItemControl>())
            {
                if (isResize && this.SelectedReportItems.Contains(reportItem))
                {
                    continue;
                }

                if (reportItem.ItemType != DrawingReportItem.Tablix)
                {
                    ReportItemLocationInfo reportItemInfo = new ReportItemLocationInfo();
                    reportItemInfo.ReportItem = reportItem;
                    reportItemInfo.ItemLeft = reportItem.ItemLeft;
                    reportItemInfo.ItemRight = reportItem.ItemLeft + reportItem.ItemWidth;
                    reportItemInfo.ItemTop = reportItem.ItemTop;
                    reportItemInfo.ItemBottom = reportItem.ItemTop + reportItem.ItemHeight;

                    if (reportItem.ItemHeight < 0)
                    {
                        reportItemInfo.ItemBottom = reportItem.ItemTop;
                        reportItemInfo.ItemTop = reportItem.ItemTop + reportItem.ItemHeight;
                    }

                    if (reportItem.ItemWidth < 0)
                    {
                        reportItemInfo.ItemRight = reportItem.ItemLeft;
                        reportItemInfo.ItemLeft = reportItem.ItemLeft + reportItem.ItemWidth;
                    }

                    if (parentInfo != null)
                    {
                        reportItemInfo.ItemLeft += parentInfo.ItemLeft;
                        reportItemInfo.ItemRight += reportItemInfo.ItemLeft;
                        reportItemInfo.ItemTop += parentInfo.ItemTop;
                        reportItemInfo.ItemBottom += parentInfo.ItemTop;
                    }

                    reportItemsLocation.Add(reportItemInfo);

                    if (this.SelectedReportItems.Contains(reportItem) || (parentInfo != null && parentInfo.IsSelected))
                    {
                        reportItemInfo.IsSelected = true;
                    }

                    if (reportItem.ItemType == DrawingReportItem.Rectangle)
                    {
                        reportItemsLocation.AddRange(this.GetReportItemLocationInfo(reportItem as Canvas, isResize, reportItemInfo));
                    }
                }
                else
                {
                    bool isTablixSelected = false;

                    if (this.SelectedReportItems.Contains(reportItem) || (parentInfo != null && parentInfo.IsSelected))
                    {
                        isTablixSelected = true;
                    }

                    foreach (var cellContent in (reportItem as TablixControl).Children.OfType<CellContentsControl>())
                    {
                        ReportItemLocationInfo cellLocationInfo = this.GetTablixItemLocationInfo(cellContent as FrameworkElement, reportItem as TablixControl);
                        cellLocationInfo.IsSelected = isTablixSelected;
                        cellLocationInfo.ItemTop += reportItem.ItemTop;
                        cellLocationInfo.ItemBottom += reportItem.ItemTop;
                        cellLocationInfo.ItemLeft += reportItem.ItemLeft;
                        cellLocationInfo.ItemRight += reportItem.ItemLeft;
                        IReportItemControl cellReportItem = cellContent.Content as IReportItemControl;
                        cellLocationInfo.ReportItem = cellReportItem;

                        reportItemsLocation.Add(cellLocationInfo);

                        if (cellReportItem!=null && cellReportItem.ItemType == DrawingReportItem.Rectangle)
                        {
                            reportItemsLocation.AddRange(this.GetReportItemLocationInfo(cellReportItem as Canvas, isResize, cellLocationInfo));
                        }
                    }
                }
            }

            return reportItemsLocation;
        }

        internal ReportItemLocationInfo GetTablixItemLocationInfo(FrameworkElement cellContent, TablixControl tablix)
        {
            double topValue = 0;
            double leftValue = 0;
            int row = Grid.GetRow(cellContent);
            int column = Grid.GetColumn(cellContent);

            for (int rowIndex = 0; rowIndex < row; rowIndex++)
            {
                topValue += tablix.RowDefinitions[rowIndex].ActualHeight;
            }

            for (int colIndex = 0; colIndex < column; colIndex++)
            {
                leftValue += tablix.ColumnDefinitions[colIndex].ActualWidth;
            }

            ReportItemLocationInfo locationInfo = new ReportItemLocationInfo();
            locationInfo.ItemLeft = leftValue;
            locationInfo.ItemTop = topValue;
            locationInfo.ItemRight = leftValue + cellContent.ActualWidth;
            locationInfo.ItemBottom = topValue + cellContent.ActualHeight;

            return locationInfo;
        }

        internal ReportItemLocationInfo GetCellInfo(CellContentsControl cellContent)
        {
            ReportItemLocationInfo locationInfo = new ReportItemLocationInfo();
            TablixControl parentElement = cellContent.Parent as TablixControl;
            ReportItemLocationInfo parentInfo = new ReportItemLocationInfo();

            if (parentElement.Parent is IReportItemControl && parentElement.Parent != null)
            {
                parentInfo = this.GetParentInfo(parentElement.Parent as IReportItemControl);
            }
            else if (parentElement.Parent == null)
            {
                parentInfo = this.GetCellInfo((FrameworkElement)parentElement.Parent as CellContentsControl);
            }

            ReportItemLocationInfo cellInfo = this.GetTablixItemLocationInfo(cellContent, parentElement);
            locationInfo.ItemLeft = parentInfo.ItemLeft + cellInfo.ItemLeft + parentElement.ItemLeft;
            locationInfo.ItemTop = parentInfo.ItemTop + cellInfo.ItemTop + parentElement.ItemTop;

            return locationInfo;
        }

        internal ReportItemLocationInfo GetParentInfo(IReportItemControl reportItem)
        {
            ReportItemLocationInfo locationInfo = new ReportItemLocationInfo();

            if (reportItem.Parent is IReportItemControl || reportItem.Parent == null)
            {
                if (reportItem.Parent != null)
                {
                    ReportItemLocationInfo parentInfo = this.GetParentInfo(reportItem.Parent as IReportItemControl);
                    locationInfo.ItemLeft = reportItem.ItemLeft + parentInfo.ItemLeft;
                    locationInfo.ItemTop = reportItem.ItemTop + parentInfo.ItemTop;
                }
                else
                {
                    FrameworkElement reportItemUiElement = reportItem as FrameworkElement;
                    CellContentsControl cellContent = reportItemUiElement.Parent as CellContentsControl;
                    TablixControl parentElement = cellContent.Parent as TablixControl;
                    locationInfo = GetCellInfo(cellContent);
                }
            }
            else if (reportItem != null)
            {
                ReportItemLocationInfo parentInfo = new ReportItemLocationInfo();
                locationInfo.ItemLeft = reportItem.ItemLeft;
                locationInfo.ItemTop = reportItem.ItemTop;
            }

            return locationInfo;
        }

        internal ReportItems GetReportItems(Canvas contentArea)
        {
            ReportItems reportItems = new ReportItems();

            foreach (UIElement element in contentArea.Children)
            {
                if (element is IReportItemControl)
                {
                    reportItems.Add(((IReportItemControl)element).GetReportItem());
                }
            }

            return reportItems;
        }

        internal void PopulateDesignPanel()
        {
            this.EditingManager = new EditingManager(this);
            this.IsInternalChange = true;

            this.MinBodyHeight = 0;
            this.MinFooterHeight = 0;
            this.MinHeaderheight = 0;

            double maxRight = 0;
            double maxBottom = 0;

            this.IsHeaderVisible = false;
            this.IsFooterVisible = false;

            RDL.DOM.Page page = this.Report.Page;
            Body body = this.Report.Body;
            RDL.DOM.Size reportWidth = this.Report.Width;
            this.RDLType = this.Report.RDLType;

            if (!string.IsNullOrEmpty(this.Report.ReportUnitType) && string.Equals(this.Report.ReportUnitType, "Cm", StringComparison.InvariantCultureIgnoreCase))
            {
                this.ReportUnitType = Base.ReportUnitType.Cm;
            }

            if (this.Report.RDLType == RDLType.RDL2010)
            {
                page = this.Report.ReportSections.First().Page;
                body = this.Report.ReportSections.First().Body;
                reportWidth = this.Report.ReportSections.First().Width;
            }

            if (page.LeftMargin != null)
            {
                this.reportproperties.Margins.LeftMargin = page.LeftMargin.size;
            }
            if (page.RightMargin != null)
            {
                this.reportproperties.Margins.RightMargin = page.RightMargin.size;
            }
            if (page.TopMargin != null)
            {
                this.reportproperties.Margins.TopMargin = page.TopMargin.size;
            }
            if (page.BottomMargin != null)
            {
                this.reportproperties.Margins.BottomMargin = page.BottomMargin.size;
            }
            if (page.Style != null)
            {
                this.reportproperties.BackgroundColor = page.Style.BackgroundColor;

                if (page.Style.BackgroundImage != null)
                {
                    this.reportproperties.BackgroundImage.ImageValue = page.Style.BackgroundImage.Value;
                    this.reportproperties.BackgroundImage.Source = page.Style.BackgroundImage.Source;
                    if (page.Style.BackgroundImage.MIMEType != null)
                        this.reportproperties.BackgroundImage.MIMEType = page.Style.BackgroundImage.MIMEType;
                }
                if (page.Style.Border != null)
                {
                    if (page.Style.Border.Style != null)
                        this.reportproperties.BorderStyles.DefaultBorderStyle = page.Style.Border.Style;
                    if (page.Style.Border.Color != null)
                        this.reportproperties.BorderColors.DefaultBorderColor = page.Style.Border.Color;
                    if (page.Style.Border.Width != null)
                        this.reportproperties.BorderWidths.DefaultBorderWidth = page.Style.Border.Width.size;

                }
                if (page.Style.LeftBorder != null)
                {
                    if (page.Style.LeftBorder.Color != null)
                        this.reportproperties.BorderColors.LeftBorderColor = page.Style.LeftBorder.Color;
                    if (page.Style.LeftBorder.Width != null)
                        this.reportproperties.BorderWidths.LeftBorderWidth = page.Style.LeftBorder.Width.size;
                    if (page.Style.LeftBorder.Style != null)
                        this.reportproperties.BorderStyles.LeftBorderStyle = page.Style.LeftBorder.Style;
                }
                if (page.Style.RightBorder != null)
                {
                    if (page.Style.RightBorder.Color != null)
                        this.reportproperties.BorderColors.RightBorderColor = page.Style.RightBorder.Color;
                    if (page.Style.RightBorder.Width != null)
                        this.reportproperties.BorderWidths.RightBorderWidth = page.Style.RightBorder.Width.size;
                    if (page.Style.RightBorder.Style != null)
                        this.reportproperties.BorderStyles.RightBorderStyle = page.Style.RightBorder.Style;
                }
                if (page.Style.TopBorder != null)
                {
                    if (page.Style.TopBorder.Color != null)
                        this.reportproperties.BorderColors.TopBorderColor = page.Style.TopBorder.Color;
                    if (page.Style.TopBorder.Width != null)
                        this.reportproperties.BorderWidths.TopBorderWidth = page.Style.TopBorder.Width.size;
                    if (page.Style.TopBorder.Style != null)
                        this.reportproperties.BorderStyles.TopBorderStyle = page.Style.TopBorder.Style;
                }
                if (page.Style.BottomBorder != null)
                {
                    if (page.Style.BottomBorder.Color != null)
                        this.reportproperties.BorderColors.BottomBorderColor = page.Style.BottomBorder.Color;
                    if (page.Style.BottomBorder.Width != null)
                        this.reportproperties.BorderWidths.BottomBorderWidth = page.Style.BottomBorder.Width.size;
                    if (page.Style.BottomBorder.Style != null)
                        this.reportproperties.BorderStyles.BottomBorderStyle = page.Style.BottomBorder.Style;
                }
            }

            PageHeader pageHeader = page.PageHeader;
            PageFooter pageFooter = page.PageFooter;

            if (pageHeader != null)
            {
                this.IsHeaderVisible = true;

                if (pageHeader.ReportItems != null)
                {
                    this.headerproperties.HeaderHeight = "0in";

                    var exReportItems = (from uiElement in this.headerCanvas.Children.OfType<UIElement>()
                                         where !(uiElement is System.Windows.Shapes.Rectangle)
                                         select uiElement).ToList();

                    foreach (UIElement uiElement in exReportItems)
                    {
                        this.headerCanvas.Children.Remove(uiElement);
                    }

                    foreach (var reportItem in pageHeader.ReportItems)
                    {
                        var leftValue = reportItem.Left.PixelValue;
                        var topValue = reportItem.Top.PixelValue;
                        double rightVal = leftValue + reportItem.Width.PixelValue;
                        double bottomVal = topValue + reportItem.Height.PixelValue;

                        maxRight = maxRight > rightVal ? maxRight : rightVal;
                        maxBottom = maxBottom > bottomVal ? maxBottom : bottomVal;

                        this.drawingCanvas = this.headerCanvas;
                        IReportItemControl reportControl = this.AddReportItem(reportItem);
                        reportControl.RaiseReportItemSizeChangedEvent();
                    }

                    this.drawingCanvas = null;
                }

                maxHeaderWidthValue = maxRight;
                this.MinHeaderheight = maxBottom;
                this.headerproperties.HeaderHeight = pageHeader.Height.size;
                this.headerproperties.PrintOnFirstPage = pageHeader.PrintOnFirstPage.ToString();
                this.headerproperties.PrintOnLastPage = pageHeader.PrintOnLastPage.ToString();

                if (pageHeader.Style != null)
                {
                    this.headerproperties.BackgroundColor = pageHeader.Style.BackgroundColor;

                    if (pageHeader.Style.BackgroundImage != null)
                    {
                        this.headerproperties.BackgroundImage.ImageValue = pageHeader.Style.BackgroundImage.Value;
                        this.headerproperties.BackgroundImage.Source = pageHeader.Style.BackgroundImage.Source;
                        if (pageHeader.Style.BackgroundImage.MIMEType != null)
                            this.headerproperties.BackgroundImage.MIMEType = pageHeader.Style.BackgroundImage.MIMEType;
                    }
                    if (pageHeader.Style.Border != null)
                    {
                        if (pageHeader.Style.Border.Style != null)
                            this.headerproperties.BorderStyles.DefaultBorderStyle = pageHeader.Style.Border.Style;
                        if (pageHeader.Style.Border.Color != null)
                            this.headerproperties.BorderColors.DefaultBorderColor = pageHeader.Style.Border.Color;
                        if (pageHeader.Style.Border.Width != null)
                            this.headerproperties.BorderWidths.DefaultBorderWidth = pageHeader.Style.Border.Width.size;

                    }
                    if (pageHeader.Style.LeftBorder != null)
                    {
                        if (pageHeader.Style.LeftBorder.Color != null)
                            this.headerproperties.BorderColors.LeftBorderColor = pageHeader.Style.LeftBorder.Color;
                        if (pageHeader.Style.LeftBorder.Width != null)
                            this.headerproperties.BorderWidths.LeftBorderWidth = pageHeader.Style.LeftBorder.Width.size;
                        if (pageHeader.Style.LeftBorder.Style != null)
                            this.headerproperties.BorderStyles.LeftBorderStyle = pageHeader.Style.LeftBorder.Style;
                    }
                    if (pageHeader.Style.RightBorder != null)
                    {
                        if (pageHeader.Style.RightBorder.Color != null)
                            this.headerproperties.BorderColors.RightBorderColor = pageHeader.Style.RightBorder.Color;
                        if (pageHeader.Style.RightBorder.Width != null)
                            this.headerproperties.BorderWidths.RightBorderWidth = pageHeader.Style.RightBorder.Width.size;
                        if (pageHeader.Style.RightBorder.Style != null)
                            this.headerproperties.BorderStyles.RightBorderStyle = pageHeader.Style.RightBorder.Style;
                    }
                    if (pageHeader.Style.TopBorder != null)
                    {
                        if (pageHeader.Style.TopBorder.Color != null)
                            this.headerproperties.BorderColors.TopBorderColor = pageHeader.Style.TopBorder.Color;
                        if (pageHeader.Style.TopBorder.Width != null)
                            this.headerproperties.BorderWidths.TopBorderWidth = pageHeader.Style.TopBorder.Width.size;
                        if (pageHeader.Style.TopBorder.Style != null)
                            this.headerproperties.BorderStyles.TopBorderStyle = pageHeader.Style.TopBorder.Style;
                    }
                    if (pageHeader.Style.BottomBorder != null)
                    {
                        if (pageHeader.Style.BottomBorder.Color != null)
                            this.headerproperties.BorderColors.BottomBorderColor = pageHeader.Style.BottomBorder.Color;
                        if (pageHeader.Style.BottomBorder.Width != null)
                            this.headerproperties.BorderWidths.BottomBorderWidth = pageHeader.Style.BottomBorder.Width.size;
                        if (pageHeader.Style.BottomBorder.Style != null)
                            this.headerproperties.BorderStyles.BottomBorderStyle = pageHeader.Style.BottomBorder.Style;
                    }
                }
            }

            maxRight = 0;
            maxBottom = 0;

            if (pageFooter != null)
            {
                this.IsFooterVisible = true;
                this.footerproperties.FooterHeight = "0in";
                this.footerproperties.FooterHeight = pageFooter.Height.size;

                var exReportItems = (from uiElement in this.footerCanvas.Children.OfType<UIElement>()
                                     where !(uiElement is System.Windows.Shapes.Rectangle)
                                     select uiElement).ToList();

                foreach (UIElement uiElement in exReportItems)
                {
                    this.footerCanvas.Children.Remove(uiElement);
                }

                if (pageFooter.ReportItems != null)
                {
                    foreach (var reportItem in pageFooter.ReportItems)
                    {
                        var leftValue = reportItem.Left.PixelValue;
                        var topValue = reportItem.Top.PixelValue;
                        double rightVal = leftValue + reportItem.Width.PixelValue;
                        double bottomVal = topValue + reportItem.Height.PixelValue;

                        maxRight = maxRight > rightVal ? maxRight : rightVal;
                        maxBottom = maxBottom > bottomVal ? maxBottom : bottomVal;

                        this.drawingCanvas = this.footerCanvas;
                        IReportItemControl reportControl = this.AddReportItem(reportItem);
                        reportControl.RaiseReportItemSizeChangedEvent();
                    }

                    this.drawingCanvas = null;
                }

                maxFooterWidthValue = maxRight;
                this.MinFooterHeight = maxBottom;
                this.drawingCanvas = null;
                this.footerproperties.PrintOnFirstPage = pageFooter.PrintOnFirstPage.ToString();
                this.footerproperties.PrintOnLastPage = pageFooter.PrintOnLastPage.ToString();

                if (pageFooter.Style != null)
                {
                    this.footerproperties.BackgroundColor = pageFooter.Style.BackgroundColor;

                    if (pageFooter.Style.BackgroundImage != null)
                    {
                        this.footerproperties.BackgroundImage.ImageValue = pageFooter.Style.BackgroundImage.Value;
                        this.footerproperties.BackgroundImage.Source = pageFooter.Style.BackgroundImage.Source;
                        if (pageFooter.Style.BackgroundImage.MIMEType != null)
                            this.footerproperties.BackgroundImage.MIMEType = pageFooter.Style.BackgroundImage.MIMEType;
                    }
                    if (pageFooter.Style.Border != null)
                    {
                        if (pageFooter.Style.Border.Style != null)
                            this.footerproperties.BorderStyles.DefaultBorderStyle = pageFooter.Style.Border.Style;
                        if (pageFooter.Style.Border.Color != null)
                            this.footerproperties.BorderColors.DefaultBorderColor = pageFooter.Style.Border.Color;
                        if (pageFooter.Style.Border.Width != null)
                            this.footerproperties.BorderWidths.DefaultBorderWidth = pageFooter.Style.Border.Width.size;

                    }
                    if (pageFooter.Style.LeftBorder != null)
                    {
                        if (pageFooter.Style.LeftBorder.Color != null)
                            this.footerproperties.BorderColors.LeftBorderColor = pageFooter.Style.LeftBorder.Color;
                        if (pageFooter.Style.LeftBorder.Width != null)
                            this.footerproperties.BorderWidths.LeftBorderWidth = pageFooter.Style.LeftBorder.Width.size;
                        if (pageFooter.Style.LeftBorder.Style != null)
                            this.footerproperties.BorderStyles.LeftBorderStyle = pageFooter.Style.LeftBorder.Style;
                    }
                    if (pageFooter.Style.RightBorder != null)
                    {
                        if (pageFooter.Style.RightBorder.Color != null)
                            this.footerproperties.BorderColors.RightBorderColor = pageFooter.Style.RightBorder.Color;
                        if (pageFooter.Style.RightBorder.Width != null)
                            this.footerproperties.BorderWidths.RightBorderWidth = pageFooter.Style.RightBorder.Width.size;
                        if (pageFooter.Style.RightBorder.Style != null)
                            this.footerproperties.BorderStyles.RightBorderStyle = pageFooter.Style.RightBorder.Style;
                    }
                    if (pageFooter.Style.TopBorder != null)
                    {
                        if (pageFooter.Style.TopBorder.Color != null)
                            this.footerproperties.BorderColors.TopBorderColor = pageFooter.Style.TopBorder.Color;
                        if (pageFooter.Style.TopBorder.Width != null)
                            this.footerproperties.BorderWidths.TopBorderWidth = pageFooter.Style.TopBorder.Width.size;
                        if (pageFooter.Style.TopBorder.Style != null)
                            this.footerproperties.BorderStyles.TopBorderStyle = pageFooter.Style.TopBorder.Style;
                    }
                    if (pageFooter.Style.BottomBorder != null)
                    {
                        if (pageFooter.Style.BottomBorder.Color != null)
                            this.footerproperties.BorderColors.BottomBorderColor = pageFooter.Style.BottomBorder.Color;
                        if (pageFooter.Style.BottomBorder.Width != null)
                            this.footerproperties.BorderWidths.BottomBorderWidth = pageFooter.Style.BottomBorder.Width.size;
                        if (pageFooter.Style.BottomBorder.Style != null)
                            this.footerproperties.BorderStyles.BottomBorderStyle = pageFooter.Style.BottomBorder.Style;
                    }
                }
            }

            var bodyExReportItems = (from uiElement in this.bodyCanvas.Children.OfType<UIElement>()
                                     where !(uiElement is System.Windows.Shapes.Rectangle)
                                     select uiElement).ToList();

            foreach (UIElement uiElement in bodyExReportItems)
            {
                this.bodyCanvas.Children.Remove(uiElement);
            }

            this.bodyproperties.BodyHeight = "0in";
            this.bodyproperties.BodyHeight = body.Height.size;
            this.bodyproperties.ReportWidth = "0in";
            this.bodyproperties.ReportWidth = reportWidth.size;
            this.reportproperties.PageHeight = page.PageHeight.size;
            this.reportproperties.PageWidth = page.PageWidth.size;

            if (body.Style != null)
            {
                this.bodyproperties.BackgroundColor = body.Style.BackgroundColor;

                if (body.Style.BackgroundImage != null)
                {
                    this.bodyproperties.BackgroundImage.ImageValue = body.Style.BackgroundImage.Value;
                    this.bodyproperties.BackgroundImage.Source = body.Style.BackgroundImage.Source;
                    if (body.Style.BackgroundImage.MIMEType != null)
                        this.bodyproperties.BackgroundImage.MIMEType = body.Style.BackgroundImage.MIMEType;
                }
                if (body.Style.Border != null)
                {
                    if (body.Style.Border.Style != null)
                        this.bodyproperties.BorderStyles.DefaultBorderStyle = body.Style.Border.Style;
                    if (body.Style.Border.Color != null)
                        this.bodyproperties.BorderColors.DefaultBorderColor = body.Style.Border.Color;
                    if (body.Style.Border.Width != null)
                        this.bodyproperties.BorderWidths.DefaultBorderWidth = body.Style.Border.Width.size;

                }
                if (body.Style.LeftBorder != null)
                {
                    if (body.Style.LeftBorder.Color != null)
                        this.bodyproperties.BorderColors.LeftBorderColor = body.Style.LeftBorder.Color;
                    if (body.Style.LeftBorder.Width != null)
                        this.bodyproperties.BorderWidths.LeftBorderWidth = body.Style.LeftBorder.Width.size;
                    if (body.Style.LeftBorder.Style != null)
                        this.bodyproperties.BorderStyles.LeftBorderStyle = body.Style.LeftBorder.Style;
                }
                if (body.Style.RightBorder != null)
                {
                    if (body.Style.RightBorder.Color != null)
                        this.bodyproperties.BorderColors.RightBorderColor = body.Style.RightBorder.Color;
                    if (body.Style.RightBorder.Width != null)
                        this.bodyproperties.BorderWidths.RightBorderWidth = body.Style.RightBorder.Width.size;
                    if (body.Style.RightBorder.Style != null)
                        this.bodyproperties.BorderStyles.RightBorderStyle = body.Style.RightBorder.Style;
                }
                if (body.Style.TopBorder != null)
                {
                    if (body.Style.TopBorder.Color != null)
                        this.bodyproperties.BorderColors.TopBorderColor = body.Style.TopBorder.Color;
                    if (body.Style.TopBorder.Width != null)
                        this.bodyproperties.BorderWidths.TopBorderWidth = body.Style.TopBorder.Width.size;
                    if (body.Style.TopBorder.Style != null)
                        this.bodyproperties.BorderStyles.TopBorderStyle = body.Style.TopBorder.Style;
                }
                if (body.Style.BottomBorder != null)
                {
                    if (body.Style.BottomBorder.Color != null)
                        this.bodyproperties.BorderColors.BottomBorderColor = body.Style.BottomBorder.Color;
                    if (body.Style.BottomBorder.Width != null)
                        this.bodyproperties.BorderWidths.BottomBorderWidth = body.Style.BottomBorder.Width.size;
                    if (body.Style.BottomBorder.Style != null)
                        this.bodyproperties.BorderStyles.BottomBorderStyle = body.Style.BottomBorder.Style;
                }
            }
            if (body.ReportItems != null)
            {
                foreach (var reportItem in body.ReportItems)
                {
                    var leftValue = reportItem.Left.PixelValue;
                    var topValue = reportItem.Top.PixelValue;
                    double rightVal = leftValue + reportItem.Width.PixelValue;
                    double bottomVal = topValue + reportItem.Height.PixelValue;

                    maxRight = maxRight > rightVal ? maxRight : rightVal;
                    maxBottom = maxBottom > bottomVal ? maxBottom : bottomVal;

                    this.drawingCanvas = this.bodyCanvas;
                    IReportItemControl reportControl = this.AddReportItem(reportItem);
                    reportControl.RaiseReportItemSizeChangedEvent();
                }

                maxBodyWidthValue = maxRight;
                this.MinBodyHeight = maxBottom;
                this.drawingCanvas = null;
            }

            this.drawingCanvas = null;
            this.MinReportWidth = (maxBodyWidthValue > maxHeaderWidthValue) ? ((maxBodyWidthValue > maxFooterWidthValue) ? maxBodyWidthValue : maxFooterWidthValue) : ((maxHeaderWidthValue > maxFooterWidthValue) ? maxHeaderWidthValue : maxFooterWidthValue);
            this.IsInternalChange = false;
        }

        private string GetBuiltInFunctionsToRDL(string richTextBoxContent)
        {
            string modifiedText = "";

            if (richTextBoxContent.StartsWith("[&") && richTextBoxContent.EndsWith("]"))
            {
                modifiedText = richTextBoxContent;
                modifiedText = modifiedText.Replace("[&", "").Replace("]", "");

                if (modifiedText == "ExecutionTime" || modifiedText == "ReportFolder"
                    || modifiedText == "ReportName" || modifiedText == "ReportServerUrl"
                    || modifiedText == "PageNumber" || modifiedText == "TotalPages")
                {
                    modifiedText = "=Globals!" + modifiedText;
                }
                else if (modifiedText == "UserID" || modifiedText == "Language")
                {
                    modifiedText = "=User!" + modifiedText;
                }
                else
                {
                    modifiedText = richTextBoxContent;
                }
            }
            else
            {
                modifiedText = richTextBoxContent;
            }

            return modifiedText;
        }

        private bool IsNameExistInReport(string ElementName)
        {
            return false;
        }

        private System.Drawing.Imaging.ImageFormat PopulateImageFormatFromString(string extension)
        {
            switch (extension.ToLower())
            {
                case ".png":
                    return System.Drawing.Imaging.ImageFormat.Png;
                case ".jpeg":
                case ".jpg":
                case ".jpe":
                    return System.Drawing.Imaging.ImageFormat.Jpeg;
                case ".bmp":
                    return System.Drawing.Imaging.ImageFormat.Bmp;
                case ".gif":
                    return System.Drawing.Imaging.ImageFormat.Gif;
                case ".emf":
                    return System.Drawing.Imaging.ImageFormat.Emf;
            }

            return null;
        }

        private string ImageToBase64(System.Drawing.Image image, System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                //// Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                //// Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

        #endregion

        public List<IReportItemControl> SelectedReportItems
        {
            get;
            set;
        }

        public void DrawReportItemControl(DrawingReportItem itemType)
        {
            this.SelectedReportItemType = itemType;
        }

        public string GetShapeFile()
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "Shape Files (*.shp)|*.shp";
            string ShapeFile = string.Empty;
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ShapeFile = openFileDialog.FileName;
            }
            return ShapeFile;
        }

        public EmbeddedImage GetEmbeddedImage()
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "JPEG files (*.jpg)|*.jpg|GIF files (*.gif)|*.gif|All files (*.*)|*.*";
            System.Drawing.Imaging.ImageFormat imageformat = null;
            bool validate;

            do
            {
                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    imageformat = this.PopulateImageFormatFromString(System.IO.Path.GetExtension(openFileDialog.FileName));
                    validate = (imageformat == null) ? false : true;

                    if (!validate)
                    {
                        MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxValidImage"),SR.GetString(CultureInfo.CurrentUICulture, "titleReportDesigner"), MessageBoxButton.OK);
                    }
                }
                else
                {
                    validate = true;
                }

            } while (!validate);
            
            if (imageformat != null)
            {
                Syncfusion.RDL.DOM.EmbeddedImage embeddedImage = new Syncfusion.RDL.DOM.EmbeddedImage();
                System.Drawing.Image drawingImage = System.Drawing.Image.FromFile(openFileDialog.FileName);

                if (imageformat != System.Drawing.Imaging.ImageFormat.Emf)
                {
                    embeddedImage.ImageData = ImageToBase64(drawingImage, imageformat);
                }
                else
                {
                    System.IO.FileStream inFile = new System.IO.FileStream(openFileDialog.FileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                    byte[] binaryData = new Byte[inFile.Length];
                    long bytesRead = inFile.Read(binaryData, 0, (int)inFile.Length);
                    inFile.Close();
                    string base64String = System.Convert.ToBase64String(binaryData, 0, binaryData.Length);
                    embeddedImage.ImageData = base64String;
                }

                embeddedImage.MIMEType = "image/" + imageformat.ToString().ToLower();
                string emImgName = System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                embeddedImage.Name = emImgName;
                int nameCount = 0;
                int imageCount = 0;

                do
                {
                    nameCount = (from embedImage in this.EmbeddedImages
                                 where embedImage.Name.Equals(embeddedImage.Name)
                                 select embedImage).Count();

                    if (nameCount > 0)
                    {
                        embeddedImage.Name = emImgName + (++imageCount);
                    }

                } while (nameCount > 0);

                return embeddedImage;
            }

            return null;
        }

        public void AddEmbeddedImage(EmbeddedImage embeddedImage)
        {
            this.EmbeddedImages.Add(embeddedImage);

            foreach (var item in reportItems)
            {
                if (item.ItemType == DrawingReportItem.Image)
                {
                    (item as Syncfusion.Windows.Reports.Designer.Controls.ImageControl).UpdateImageSource();
                }
                else if (item.ItemType == DrawingReportItem.TextBox)
                {
                    (item as Syncfusion.Windows.Reports.Designer.Controls.TextBoxControl).UpdateBackgroundImage();
                }
                else if (item.ItemType == DrawingReportItem.Rectangle)
                {
                    (item as Syncfusion.Windows.Reports.Designer.Controls.RectangleControl).UpdateBackgroundImage();
                }
                else if (item.ItemType == DrawingReportItem.Chart)
                {
                    (item as Syncfusion.Windows.Reports.Designer.Controls.ChartControl).UpdateBackgroundImage();
                }
            }

            if (!this.IsInternalChange)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.EmbeddedImageAdd;
                action.EmbeddedImage = embeddedImage;
                this.EditingManager.AddAction(action);
            }
        }

        public void RemoveEmbeddedImage(EmbeddedImage embeddedImage)
        {
            this.EmbeddedImages.Remove(embeddedImage);

            if (!this.IsInternalChange)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.EmbeddedImageRemove;
                action.EmbeddedImage = embeddedImage;
                this.EditingManager.AddAction(action);
                this.EditingManager.IsMergeAction = true;

                foreach (var item in reportItems)
                {
                    if (item.ItemType == DrawingReportItem.Image)
                    {
                        var imgControl = (Syncfusion.Windows.Reports.Designer.Controls.ImageControl)item;

                        if (imgControl.ImageValue == embeddedImage.Name)
                        {
                            imgControl.ImageProperties.ImageValue = null;
                        }
                    }
                    else if (item.ItemType == DrawingReportItem.TextBox)
                    {
                        var textControl = (Syncfusion.Windows.Reports.Designer.Controls.TextBoxControl)item;

                        if (textControl.Properties.BackgroundImage.ImageValue == embeddedImage.Name)
                        {
                            textControl.Properties.BackgroundImage.ImageValue = null;
                        }
                    }
                    else if (item.ItemType == DrawingReportItem.Rectangle)
                    {
                        var rectangleControl = (Syncfusion.Windows.Reports.Designer.Controls.RectangleControl)item;

                        if (rectangleControl.Properties.BackgroundImage.ImageValue == embeddedImage.Name)
                        {
                            rectangleControl.Properties.BackgroundImage.ImageValue = null;
                        }
                    }
                    else if (item.ItemType == DrawingReportItem.Chart)
                    {
                        var chartControl = (Syncfusion.Windows.Reports.Designer.Controls.ChartControl)item;

                        if (chartControl.chartProperties.BackgroundImage.ImageValue == embeddedImage.Name)
                        {
                            chartControl.chartProperties.BackgroundImage.ImageValue = null;
                        }
                    }
                }

                this.EditingManager.IsMergeAction = false;
            }
        }

        public void AddEmbeddedImage()
        {
            EmbeddedImage embeddedImage = this.GetEmbeddedImage();

            if (embeddedImage != null)
            {
                this.AddEmbeddedImage(embeddedImage);
                this.RaiseEmbeddedImageCollectionModifiedEvent();
            }
        }

        public void RemoveEmbeddedImage(string Name)
        {
            var embeddedImages = from embedImage in this.EmbeddedImages
                                 where embedImage.Name.Equals(Name)
                                 select embedImage;

            if (embeddedImages.Count() > 0)
            {
                this.RemoveEmbeddedImage(embeddedImages.First());
                this.RaiseEmbeddedImageCollectionModifiedEvent();
            }
        }

        public RDL.DOM.DataSource GetDataSource()
        {
            DataSourceUI dataSourceUI = new DataSourceUI(this.DataSources, this);
            this.UpdateOwnerWindow(dataSourceUI);

            if (dataSourceUI.ShowDialog() == true)
            {
                return dataSourceUI.DataSource;
            }

            return null;
        }

        public void AddDataSource(RDL.DOM.DataSource reportDataSource)
        {
            this.DataSources.Add(reportDataSource);

            if (!this.IsInternalChange)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.DatasourceAdd;
                action.DataSource = reportDataSource;
                this.EditingManager.AddAction(action);
            }
        }

        public void RemoveDataSource(RDL.DOM.DataSource reportDataSource)
        {
            this.DataSources.Remove(reportDataSource);

            if (!this.IsInternalChange)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.DatasourceRemove;
                action.DataSource = reportDataSource;
                this.EditingManager.AddAction(action);
            }
        }

        public void AddDataSource()
        {
            RDL.DOM.DataSource reportDataSource = this.GetDataSource();

            if (reportDataSource != null)
            {
                this.AddDataSource(reportDataSource);
                this.RaiseDataSourceCollectionModifiedEvent();
            }
        }

        public void RemoveDataSource(string name)
        {
            var dataSources = from dataSource in this.DataSources
                              where dataSource.Name.Equals(name)
                              select dataSource;

            if (dataSources.Count() > 0)
            {
                this.RemoveDataSource(dataSources.First());
                this.RaiseDataSourceCollectionModifiedEvent();
            }
        }

        public void ModifyDataSource(string name)
        {
            var dataSources = from dataSource in this.DataSources
                              where dataSource.Name.Equals(name)
                              select dataSource;

            if (dataSources.Count() > 0)
            {
                DataSourceUI dataSourceUI = new DataSourceUI(dataSources.First(), this.DataSources, this);
                this.UpdateOwnerWindow(dataSourceUI);

                EditAction action = new EditAction();
                action.EditingType = EditActionType.DatasourceChanged;
                action.DataSourceChange = new DataSourceChange();
                action.DataSourceChange.DataSource = dataSources.First();
                action.DataSourceChange.OldValue = action.DataSourceChange.DataSource.Clone() as DataSource;

                if (dataSourceUI.ShowDialog() == true)
                {
                    this.RaiseDataSourceCollectionModifiedEvent();

                    if (!this.IsInternalChange)
                    {
                        action.DataSourceChange.NewValue = action.DataSourceChange.DataSource.Clone() as DataSource;
                        this.EditingManager.AddAction(action);
                    }
                }
            }
        }

        public void AddDataSet(RDL.DOM.DataSet dataSet)
        {
            this.DataSets.Add(dataSet);

            if (!this.IsInternalChange)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.DatasetAdd;
                action.DataSet = dataSet;
                this.EditingManager.AddAction(action);
            }
        }

        public void RemoveDataSet(RDL.DOM.DataSet dataSet)
        {
            this.DataSets.Remove(dataSet);

            if (!this.IsInternalChange)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.DatasetRemove;
                action.DataSet = dataSet;
                this.EditingManager.AddAction(action);
            }
        }

        public void AddDataSet()
        {
            try
            {
                if (reportDesignView.DesignMode == DesignMode.RDLC)
                {
                    DataSetClient dataSetClient = new DataSetClient(this.DataSets, this.DataSources, this.reportDesignView.AssemblyInfos, this.reportDesignView.ObjectInfos, this.reportDesignView.Assemblies);
                    this.UpdateOwnerWindow(dataSetClient);

                    if (dataSetClient.ShowDialog() == true)
                    {
                        if (dataSetClient.CreatedDataSource != null)
                        {
                            this.AddDataSource(dataSetClient.CreatedDataSource);
                            this.RaiseDataSourceCollectionModifiedEvent();
                            this.EditingManager.IsMergeAction = true;
                        }

                        this.AddDataSet(dataSetClient.DataSet);
                        this.RaiseDataSetCollectionModifiedEvent();

                        this.EditingManager.IsMergeAction = false;
                    }
                }
                else
                {
                    DataSetUI dataSetUI = new DataSetUI(this.DataSets, this.DataSources, this);
                    this.UpdateOwnerWindow(dataSetUI);

                    if (dataSetUI.ShowDialog() == true)
                    {
                        this.AddDataSet(dataSetUI.DataSet);

                        if (dataSetUI.ReportParameters != null && dataSetUI.ReportParameters.Count > 0)
                        {
                            foreach (var parameter in dataSetUI.ReportParameters)
                            {
                                this.AddReportParameter(parameter);
                            }

                            this.RaiseParameterCollectionModifiedEvent();
                        }

                        this.RaiseDataSetCollectionModifiedEvent();
                    }
                }
            }
            catch
            {

            }
        }

        public void RemoveDataSet(string name)
        {
            var dataSets = from dataSet in this.DataSets
                           where dataSet.Name.Equals(name)
                           select dataSet;

            if (dataSets.Count() > 0)
            {
                this.DataSets.Remove(dataSets.First());
                this.RaiseDataSetCollectionModifiedEvent();
            }
        }

        public void RemoveDatasetFields(string datasetname,string fieldname)
        {
            for (int i = 0; i < this.DataSets.Count; i++)
            {
                if (this.DataSets[i].Name == datasetname)
                {
                    var fields = from field in this.DataSets[i].Fields
                                 where field.Name.Equals(fieldname)
                                select field;
                    if (fields.Count() > 0)
                    {
                        this.DataSets[i].Fields.Remove(fields.First());
                      
                    }
                }
            }
            this.RaiseDataSetFieldCollectionModifiedEvent();
        }

        public void ModifyDataSet(string name)
        {
            var dataSets = from dataSet in this.DataSets
                           where dataSet.Name.Equals(name)
                           select dataSet;

            if (dataSets.Count() > 0)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.DatasetChanged;
                action.DataSetChange = new DataSetChange();
                action.DataSetChange.DataSet = dataSets.First();
                action.DataSetChange.OldValue = action.DataSetChange.DataSet.Clone() as DataSet;

                if (reportDesignView.DesignMode != DesignMode.RDLC)
                {
                    DataSetUI dataSetUI = new DataSetUI(dataSets.First(), this.DataSets, this.DataSources, this);
                    this.UpdateOwnerWindow(dataSetUI);

                    if (dataSetUI.ShowDialog() == true)
                    {
                        action.DataSetChange.NewValue = action.DataSetChange.DataSet.Clone() as DataSet;
                        this.EditingManager.AddAction(action);
                        this.EditingManager.IsMergeAction = true;

                        if (dataSetUI.ReportParameters != null && dataSetUI.ReportParameters.Count > 0)
                        {
                            foreach (var parameter in dataSetUI.ReportParameters)
                            {
                                this.AddReportParameter(parameter);
                            }

                            this.RaiseParameterCollectionModifiedEvent();
                        }

                        this.RaiseDataSetCollectionModifiedEvent();
                        this.EditingManager.IsMergeAction = false;
                    }
                }
                else
                {
                    DataSetClient dataSetClient = new DataSetClient(dataSets.First(), this.DataSets, this.DataSources, this.reportDesignView.AssemblyInfos, this.reportDesignView.ObjectInfos, this.reportDesignView.Assemblies);
                    this.UpdateOwnerWindow(dataSetClient);
                    if (dataSetClient.ShowDialog() == true)
                    {
                        action.DataSetChange.NewValue = action.DataSetChange.DataSet.Clone() as DataSet;
                        this.EditingManager.AddAction(action);
                        this.EditingManager.IsMergeAction = true;

                        if (dataSetClient.CreatedDataSource != null)
                        {
                            this.AddDataSource(dataSetClient.CreatedDataSource);
                            this.RaiseDataSourceCollectionModifiedEvent();
                        }

                        this.RaiseDataSetCollectionModifiedEvent();
                        this.EditingManager.IsMergeAction = false;
                    }
                }
            }
        }

        public void AddReportParameter(RDL.DOM.ReportParameter reportParameter)
        {
            if (this.ReportParameters == null)
            {
                this.ReportParameters = new ReportParameters();
            }

            this.ReportParameters.Add(reportParameter);

            if (!this.IsInternalChange)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ReportParameterAdd;
                action.ReportParameter = reportParameter;
                this.EditingManager.AddAction(action);
            }
        }

        public void RemoveReportParameter(RDL.DOM.ReportParameter reportParameter)
        {
            this.ReportParameters.Remove(reportParameter);

            if (!this.IsInternalChange)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ReportParameterRemove;
                action.ReportParameter = reportParameter;
                this.EditingManager.AddAction(action);
            }
        }

        public void AddReportParameter()
        {
            ControlProperties controlProp = new ControlProperties(this.ReportParameters, this.DataSets);
            this.UpdateOwnerWindow(controlProp);

            if (controlProp.ShowDialog() == true)
            {
                this.AddReportParameter(controlProp.ReportParameterNew);
                this.RaiseParameterCollectionModifiedEvent();
            }
        }

        public void RemoveReportParameter(string name)
        {
            var parameters = from parameter in this.ReportParameters
                             where parameter.Name.Equals(name)
                             select parameter;

            if (parameters.Count() > 0)
            {
                this.RemoveReportParameter(parameters.First());
                this.RaiseParameterCollectionModifiedEvent();
            }
        }

        public void ModifyReportParameter(string name)
        {
            var parameters = from parameter in this.ReportParameters
                             where parameter.Name.Equals(name)
                             select parameter;

            if (parameters.Count() > 0)
            {
                ControlProperties controlProp = new ControlProperties(this.ReportParameters, this.DataSets, name);
                this.UpdateOwnerWindow(controlProp);

                EditAction action = new EditAction();
                action.EditingType = EditActionType.ReportParameterChanged;
                action.ParameterChange = new ReportParameterChange();
                action.ParameterChange.ReportParameter = parameters.First();
                action.ParameterChange.OldValue = action.ParameterChange.ReportParameter.Clone() as RDL.DOM.ReportParameter;

                if (controlProp.ShowDialog() == true)
                {
                    action.ParameterChange.NewValue = action.ParameterChange.ReportParameter.Clone() as RDL.DOM.ReportParameter;
                    this.EditingManager.AddAction(action);
                    this.RaiseParameterCollectionModifiedEvent();
                }
            }
        }

        public void RaiseReportItemDrawnEvent()
        {
            if (this.ReportReportItemDrawn != null)
            {
                this.ReportReportItemDrawn(this, new ReportItemDrawnEventArgs());
            }
        }

        public void RaiseDataSourceCollectionModifiedEvent()
        {
            if (this.ReportDataSourceCollectionModified != null)
            {
                this.ReportDataSourceCollectionModified(this, new ReportDataSourceCollectionChangeEventArgs());
            }
        }

        public void RaiseDataSetCollectionModifiedEvent()
        {
            if (this.ReportDataSetCollectionModified != null)
            {
                this.ReportDataSetCollectionModified(this, new ReportDataSetCollectionChangeEventArgs());
            }
        }

        public void RaiseDataSetFieldCollectionModifiedEvent()
        {
            if (this.ReportDataSetFieldCollectionModified != null)
            {
                this.ReportDataSetFieldCollectionModified(this, new ReportDataSetCollectionChangeEventArgs());
            }
        }


        public void RaiseParameterCollectionModifiedEvent()
        {
            if (this.ReportParameterCollectionModified != null)
            {
                this.ReportParameterCollectionModified(this, new ReportParameterCollectionChangeEventArgs());
            }
        }

        public void RaiseEmbeddedImageCollectionModifiedEvent()
        {
            if (this.ReportEmbeddedImageCollectionModified != null)
            {
                this.ReportEmbeddedImageCollectionModified(this, new ReportEmbeddedImageCollectionChangeEventArgs());
            }
        }

        public void RaiseReportItemSelectedEvent(Object sender, SelectedItemEventArgs arg)
        {
            if (this.ReportItemSelected != null)
            {
                this.ReportItemSelected(sender, arg);
            }
        }

        public event ReportItemDrawnEventHanlder ReportReportItemDrawn;

        public event ReportDataSourceCollectionModifedHandler ReportDataSourceCollectionModified;

        public event ReportDataSetCollectionModifedHandler ReportDataSetCollectionModified;

        public event ReportDataSetCollectionModifedHandler ReportDataSetFieldCollectionModified;

        public event ReportParameterCollectionModifedHandler ReportParameterCollectionModified;

        public event ReportEmbeddedImageCollectionModifedHandler ReportEmbeddedImageCollectionModified;

        public event ReportItemSelectedEvent ReportItemSelected;
    }

    internal class DesignPanelStyle : DependencyObject
    {
        public static readonly DependencyProperty RulerTickColorProperty =
            DependencyProperty.Register("RulerTickColor", typeof(Brush), typeof(DesignPanelStyle), new UIPropertyMetadata(Brushes.Black, null));

        public Brush RulerTickColor
        {
            get { return (Brush)GetValue(RulerTickColorProperty); }
            set { SetValue(RulerTickColorProperty, value); }
        }

        public static readonly DependencyProperty RulerBorderColorProperty =
           DependencyProperty.Register("RulerBorderColor", typeof(Brush), typeof(DesignPanelStyle), new UIPropertyMetadata(Brushes.Black, null));

        public Brush RulerBorderColor
        {
            get { return (Brush)GetValue(RulerBorderColorProperty); }
            set { SetValue(RulerBorderColorProperty, value); }
        }

        public static readonly DependencyProperty RulerIndicatorColorProperty =
           DependencyProperty.Register("RulerIndicatorColor", typeof(Brush), typeof(DesignPanelStyle), new UIPropertyMetadata(Brushes.Black, null));

        public Brush RulerIndicatorColor
        {
            get { return (Brush)GetValue(RulerIndicatorColorProperty); }
            set { SetValue(RulerIndicatorColorProperty, value); }
        }

        public static readonly DependencyProperty RulerTextColorProperty =
           DependencyProperty.Register("RulerTextColor", typeof(Brush), typeof(DesignPanelStyle), new UIPropertyMetadata(Brushes.Black, null));

        public Brush RulerTextColor
        {
            get { return (Brush)GetValue(RulerTextColorProperty); }
            set { SetValue(RulerTextColorProperty, value); }
        }

        public static readonly DependencyProperty RulerBackGroundProperty =
           DependencyProperty.Register("RulerBackGround", typeof(Brush), typeof(DesignPanelStyle), new UIPropertyMetadata(Brushes.Black, null));

        public Brush RulerBackGround
        {
            get { return (Brush)GetValue(RulerBackGroundProperty); }
            set { SetValue(RulerBackGroundProperty, value); }
        }

        public static readonly DependencyProperty RulerAreaBackGroundProperty =
           DependencyProperty.Register("RulerAreaBackGround", typeof(Brush), typeof(DesignPanelStyle), new UIPropertyMetadata(Brushes.Black, null));

        public Brush RulerAreaBackGround
        {
            get { return (Brush)GetValue(RulerAreaBackGroundProperty); }
            set { SetValue(RulerAreaBackGroundProperty, value); }
        }

        public static readonly DependencyProperty DesignerAreaBackGroundProperty =
              DependencyProperty.Register("DesignerAreaBackGround", typeof(Brush), typeof(DesignPanelStyle), new UIPropertyMetadata(Brushes.Black, null));

        public Brush DesignerAreaBackGround
        {
            get { return (Brush)GetValue(DesignerAreaBackGroundProperty); }
            set { SetValue(DesignerAreaBackGroundProperty, value); }
        }
    }
}