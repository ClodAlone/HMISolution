#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
using System.Security.Permissions;
using System.Windows.Controls;
using System.Windows.Media;
using Syncfusion.Windows.Controls.Gantt.Chart;
using Syncfusion.Windows.Controls.Gantt.Grid;
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.Windows.Controls.Gantt.Schedule;
using System.Windows.Controls.Primitives;
using Syncfusion.Windows.Shared;
using System.Collections.ObjectModel;
using System.Collections;
using System.Windows.Data;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Linq;
using Syncfusion.Windows.GridCommon;
using System.Windows.Markup;
using System.IO;
using System.Xml;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Controls.Gantt
{
    /// <summary>
    /// Represents a control, that illustrates a project schedule by displaying the progress of list of tasks/activities of a project in table view and graph notation.
    /// </summary>
    [TemplatePart(Name = "PART_GantSchedule", Type = typeof(GanttSchedule)),
    TemplatePart(Name = "PART_GanttGridSplitter", Type = typeof(GridSplitter)),
    TemplatePart(Name = "PART_GanttChart", Type = typeof(GanttChart)),
    TemplatePart(Name = "PART_GanttGrid", Type = typeof(GanttGrid)),
    TemplatePart(Name = "PART_GanttChartScrollViewer", Type = typeof(ScrollViewer)),
    TemplatePart(Name = "PART_ScheduleViewScrollViewer", Type = typeof(ScrollViewer)),
#if SILVERLIGHT
    TemplatePart(Name = "PART_LayoutGrid", Type = typeof(System.Windows.Controls.Grid))
#endif
]
#if !SILVERLIGHT
    [SkinType(SkinVisualStyle = Skin.Office2010Black, Type = typeof(GanttControl), XamlResource = "/Syncfusion.Gantt.Wpf;component/Themes/Office2010Black.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver, Type = typeof(GanttControl), XamlResource = "/Syncfusion.Gantt.Wpf;component/Themes/Office2010Silver.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend, Type = typeof(GanttControl), XamlResource = "/Syncfusion.Gantt.Wpf;component/Themes/Blend.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro, Type = typeof(GanttControl), XamlResource = "/Syncfusion.Gantt.Wpf;component/Themes/Metro.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010, Type = typeof(GanttControl), XamlResource = "/Syncfusion.Gantt.Wpf;component/Themes/VS2010.xaml")]
#endif

    public class GanttControl : Control
    {
        #region Private / Internal properties

        private GanttModel _model;
        private GanttGrid _ganttGrid;
        private ObservableCollection<object> _selectedItems;

        private bool isItemsSourceChangedBeforeOnApplyTemplate = false;
        private bool isLoadedWithInbuiltSource = true;
        private bool isTemplateApplied = false;
        private bool isScheduleTypeChandedBeforeOnApplyTemplate = false;
        private bool isTaskMappingChangedBeforeOnApplyTemplate = false;
        private bool isResourcesChangedBeforOnApplyTemplate = false;
        private bool isToolTipAppliedBeforeOnApplyTemplate = false;
        private bool isWeekBeginsOnsetBeforeOnApplyTemplate = false;
        private bool isCustSchSrcChangedBeforeOnApplyTemplate = false;
        private bool isHghItemsChangedBfrOnApplyTempalte = false;
		private bool isZoomFactorSetBeforeScheduleLoaded = false;
	    internal bool isRowHeightAppliedBeforeGridLoaded = false;
        internal bool isVisualStyleChanged = false;
        private bool isScrollChanged = false;

        internal GanttChart GanttChart;
        internal GanttSchedule GanttSchedule;
        internal ScrollViewer GanttChartScrollViewer;
        internal ScrollViewer ScheduleViewScrollViewer;
        internal ZoomChangedEventArgs ZoomInfo;

#if SILVERLIGHT
        private DateTime previousDate;
        private double previousPoint = 0;
        private bool isLayoutWidthChangedBeforeOnApplyTemplate = false;
        internal System.Windows.Controls.Grid LayoutGrid;
#endif
        #endregion

        #region Dependency Registration

        /// <summary>
        /// Dependecy property resigteration for StartPoint
        /// </summary>
        public static  DependencyProperty StartPointProperty =
            DependencyProperty.Register("StartPoint", typeof(double), typeof(GanttControl), new PropertyMetadata(0d, OnStartPointChanged));

        /// <summary>
        /// Dependecy property resigteration for EndPoint
        /// </summary>
        public static  DependencyProperty EndPointProperty =
            DependencyProperty.Register("EndPoint", typeof(double), typeof(GanttControl), new PropertyMetadata(100d, OnEndPointChanged));

        /// <summary>
        /// Dependecy property resigteration for StartTime
        /// </summary>
        public static DependencyProperty StartTimeProperty =
            DependencyProperty.Register("StartTime", typeof(DateTime), typeof(GanttControl), new PropertyMetadata(DateTime.Today.AddDays(-14), OnStartTimeChanged));

        /// <summary>
        /// Dependecy property resigteration for EndTime 
        /// </summary>
        public static DependencyProperty EndTimeProperty =
            DependencyProperty.Register("EndTime", typeof(DateTime), typeof(GanttControl), new PropertyMetadata(DateTime.Today.AddDays(14), OnEndTimeChanged));

        /// <summary>
        /// Dependecy property resigteration for Default StartTime 
        /// </summary>
        public static readonly DependencyProperty DefaultStartTimeProperty =
            DependencyProperty.Register("DefaultStartTime", typeof(TimeSpan), typeof(GanttControl), new PropertyMetadata(new TimeSpan(9, 0, 0), OnDefaulTimePropertyChanged));

        /// <summary>
        /// Dependecy property resigteration for Defualt  EndTime 
        /// </summary>
        public static readonly DependencyProperty DefaultEndTimeProperty =
            DependencyProperty.Register("DefaultEndTime", typeof(TimeSpan), typeof(GanttControl), new PropertyMetadata(new TimeSpan(18, 0, 0), OnDefaulTimePropertyChanged));

        /// <summary>
        /// Dependency registration for ItemsSource
        /// </summary>
        public static DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(object), typeof(GanttControl), new PropertyMetadata(OnItemsSourceChanged));

        /// <summary>
        /// Dependency registration for TaskAttributeMapping
        /// </summary>
        public static DependencyProperty TaskAttributeMappingProperty =
            DependencyProperty.Register("TaskAttributeMapping", typeof(TaskAttributeMapping), typeof(GanttControl), new PropertyMetadata(null, OnTaskAttributeMappingPropertyChanged));

        /// <summary>
        ///Dependency registration for IsFYNumberingEnabled
        /// </summary>
        public static DependencyProperty IsFYNumberingEnabledProperty =
            DependencyProperty.Register("IsFYNumberingEndbled", typeof(bool), typeof(GanttControl), new PropertyMetadata(false, OnIsFYNumberingPropertyChanged));

        /// <summary>
        ///Dependency registration for FiscalYearBeginsOn
        /// </summary>
        public static DependencyProperty FiscalYearBeginsOnProperty =
            DependencyProperty.Register("FiscalYearBeginsOn", typeof(Month), typeof(GanttControl), new PropertyMetadata(Month.January, OnFiscalYearPropertyChanged));

        /// <summary>
        ///Dependency registration for Holidays
        /// </summary>
        public static DependencyProperty HolidaysProperty = 
            DependencyProperty.Register("HolidaysCollection", typeof(Dictionary<string, string>), typeof(GanttControl), new PropertyMetadata(OnHolidaysPropertyPropertyChanged));

        /// <summary>
        ///Dependency registration for CurrentDateLine
        /// </summary>
        public static readonly DependencyProperty CurrentDateLineProperty =
            DependencyProperty.Register("CurrentDateLine", typeof(Line), typeof(GanttControl), new PropertyMetadata(null));

        /// <summary>
        /// Dependency registration for StickCurrentDateLineTo
        /// </summary>
        public static DependencyProperty StickCurrentDateLineToProperty =
            DependencyProperty.Register("StickCurrentDateLineTo", typeof(CurentDateLinePositions), typeof(GanttControl), new PropertyMetadata(CurentDateLinePositions.Today,OnStickCurrnetDateLineToPropertychanged));

        /// <summary>
        ///Dependency registration for ScheduleType
        /// </summary>
        public static DependencyProperty ScheduleTypeProperty =
            DependencyProperty.Register("ScheduleType", typeof(ScheduleType), typeof(GanttControl), new PropertyMetadata(ScheduleType.WeekWithDays, OnScheduleTypeChanged));

        /// <summary>
        ///Dependency registration for WeekBeginsOn
        /// </summary>
        public static readonly DependencyProperty WeekBeginsOnProperty =
            DependencyProperty.Register("WeekBeginsOn", typeof(DayOfWeek), typeof(GanttControl), new PropertyMetadata(DayOfWeek.Sunday, OnWeekBeginsOnChanged));

        /// <summary>
        ///Dependency registration for Task node background 
        /// </summary>
        public static readonly DependencyProperty TaskNodeBackgroundProperty =
            DependencyProperty.Register("TaskNodeBackground", typeof(Brush), typeof(GanttControl), new PropertyMetadata(null));

        /// <summary>
        ///Dependency registration for Connector Stroke 
        /// </summary>
        public static readonly DependencyProperty ConnectorStrokeProperty =
            DependencyProperty.Register("ConnectorStroke", typeof(Brush), typeof(GanttControl), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        ///Dependency registration for ToolTip Template 
        /// </summary>
        public static DependencyProperty ToolTipTemplateProperty = 
            DependencyProperty.Register("ToolTipTemplate", typeof(DataTemplate), typeof(GanttControl), new PropertyMetadata(OnToolTipTemplateChanged));

		/// <summary>
        ///Dependency registration for Resource property
        /// </summary>
        public static readonly DependencyProperty ResourceCollectionProperty =
            DependencyProperty.Register("ResourceCollection", typeof(ObservableCollection<Resource>), typeof(GanttControl), new PropertyMetadata(null, OnResourceCollectionChanged));

        /// <summary>
        ///Dependency registration for Schedule Background
        /// </summary>
        public static DependencyProperty ScheduleBackgroundProperty = 
            DependencyProperty.Register("ScheduleBackground", typeof(Brush), typeof(GanttControl), new PropertyMetadata(new SolidColorBrush(Colors.White)));

        /// <summary>
        ///Dependency registration for Schedule BorderThickness
        /// </summary>
        public static DependencyProperty ScheduleBorderThicknessProperty =
           DependencyProperty.Register("ScheduleBorderThickness", typeof(Thickness), typeof(GanttControl), new PropertyMetadata(new Thickness(1d)));

        /// <summary>
        ///Dependency registration for Schedule BorderBrush
        /// </summary>
        public static DependencyProperty ScheduleBorderBrushProperty =
           DependencyProperty.Register("ScheduleBorderBrush", typeof(Brush), typeof(GanttControl), new PropertyMetadata(null));

        /// <summary>
        ///Dependency registration for non working hours background
        /// </summary>
#if !SILVERLIGHT
        public static DependencyProperty NonWorkingHoursBackgroundProperty =
           DependencyProperty.Register("NonWorkingHoursBackground", typeof(Brush), typeof(GanttControl), new PropertyMetadata(((new BrushConverter().ConvertFromString("#FFEFF5FB")) as SolidColorBrush)));
#else
        public static DependencyProperty NonWorkingHoursBackgroundProperty=
            DependencyProperty.Register("NonWorkingHoursBackground",typeof(Brush),typeof(GanttControl),new PropertyMetadata( new SolidColorBrush(ColorExtensions.StringToColor("#FFEFF5FB"))));
#endif

        /// <summary>
        ///Dependency registration for Show Chart Lines
        /// </summary>
        public static readonly DependencyProperty ShowChartLinesProperty =
            DependencyProperty.Register("ShowChartLines", typeof(bool), typeof(GanttControl), new PropertyMetadata(true));

        // Using a DependencyProperty as the backing store for ShowChartStripLines.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowStripLinesProperty =
            DependencyProperty.Register("ShowStripLines", typeof(bool), typeof(GanttControl), new PropertyMetadata(false));

        /// <summary>
        ///Dependency registration for show non working Hours background
        /// </summary>
        public static readonly DependencyProperty ShowNonWorkingHoursBackgroundProperty =
            DependencyProperty.Register("ShowNonWorkingHoursBackground", typeof(bool), typeof(GanttControl), new PropertyMetadata(true));

        /// <summary>
        ///Dependency registration for progress Indicator Background brush.
        /// </summary>
        public static DependencyProperty ProgressIndicatorBackgroundProperty =
           DependencyProperty.Register("ProgressIndicatorBackground", typeof(Brush), typeof(GanttControl), new PropertyMetadata(new SolidColorBrush(Colors.Black)));  

        /// <summary>
        ///Dependency registration for VisualStyle
        /// </summary>
        public static readonly DependencyProperty VisualStyleProperty =
           DependencyProperty.Register("VisualStyle", typeof(VisualStyle), typeof(GanttControl), new PropertyMetadata(VisualStyle.Office2010Blue, OnVisualStyleChanged));

        /// <summary>
        ///Dependency registration for Resource name visibility
        /// </summary>
        public static DependencyProperty ResourceNameVisibilityProperty =
            DependencyProperty.Register("ResourceNameVisibility", typeof(Visibility), typeof(GanttControl), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Dependency registration for Grid Header Background
        /// </summary>
        public static readonly DependencyProperty GridHeaderBackgroundProperty =
            DependencyProperty.Register("GridHeaderBackground", typeof(Brush), typeof(GanttControl), new PropertyMetadata(null));

        /// <summary>
        /// Dependency registration for Grid Header Foreground
        /// </summary>
        public static readonly DependencyProperty GridHeaderForegroundProperty =
            DependencyProperty.Register("GridHeaderForeground", typeof(Brush), typeof(GanttControl), new PropertyMetadata(null));

        /// <summary>
        /// Dependency registration for Schedule Foreground
        /// </summary>
        public static readonly DependencyProperty ScheduleForegroundProperty =
            DependencyProperty.Register("ScheduleForeground", typeof(Brush), typeof(GanttControl), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// Dependency Registration for ShowAddNewColumn
        /// </summary>
        public static readonly DependencyProperty ShowAddNewColumnProperty =
            DependencyProperty.Register("ShowAddNewColumn", typeof(bool), typeof(GanttControl), new PropertyMetadata(true));

        /// <summary>
        /// Dependency Registration for Gantt Schedule ItemsSource
        /// </summary>
        public static readonly DependencyProperty CustomScheduleSourceProperty =
            DependencyProperty.Register("CustomScheduleSource", typeof(IList<GanttScheduleRowInfo>), typeof(GanttControl), new PropertyMetadata(null,OnCustomScheduleSourceChanged));

        // Using a DependencyProperty as the backing store for StripLineCollection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StripLinesProperty =
            DependencyProperty.Register("StripLines", typeof(IEnumerable), typeof(GanttControl),new PropertyMetadata(null,OnStripLinesChanged));

        /// <summary>
        /// Dependency Registration for Show Hours in Time filed of Gantt Grid
        /// </summary>
        public static readonly DependencyProperty ShowDateWithTimeProperty =
            DependencyProperty.Register("ShowDateWithTime", typeof(bool), typeof(GanttControl), new PropertyMetadata(false));

        /// <summary>
        /// Dependency Registration for Layout Mode Property
        /// </summary>
        public static readonly DependencyProperty LayoutModeProperty =
            DependencyProperty.Register("LayoutMode", typeof(GanttLayoutMode), typeof(GanttControl), new PropertyMetadata(GanttLayoutMode.Default, OnLayoutModeChanged));
        

#if !SILVERLIGHT
        /// <summary>
        /// Dependency Registration for Chart width Property
        /// </summary>
        public static readonly DependencyProperty ChartWidthProperty =
            DependencyProperty.Register("ChartWidth", typeof(GridLength), typeof(GanttControl), new PropertyMetadata(new GridLength(50, GridUnitType.Star)));

        /// <summary>
        /// Dependency Registration for Grid width Property
        /// </summary>
        public static readonly DependencyProperty GridWidthProperty =
            DependencyProperty.Register("GridWidth", typeof(GridLength), typeof(GanttControl), new PropertyMetadata(new GridLength(50, GridUnitType.Star)));
#else
        /// <summary>
        /// Dependency Registration for Chart width Property
        /// </summary>
        public static readonly DependencyProperty ChartWidthProperty =
            DependencyProperty.Register("ChartWidth", typeof(GridLength), typeof(GanttControl), new PropertyMetadata(new GridLength(50, GridUnitType.Star), OnChartWidthChanged));

        /// <summary>
        /// Dependency Registration for Grid width Property
        /// </summary>
        public static readonly DependencyProperty GridWidthProperty =
            DependencyProperty.Register("GridWidth", typeof(GridLength), typeof(GanttControl), new PropertyMetadata(new GridLength(50, GridUnitType.Star), OnGridWidthChanged));
#endif

        /// <summary>
        /// Dependency Registration for On-demand schedule Property
        /// </summary>
        public static readonly DependencyProperty UseOnDemandScheduleProperty =
            DependencyProperty.Register("UseOnDemandSchedule", typeof(bool), typeof(GanttControl), new PropertyMetadata(false));

        /// <summary>
        /// Dependency Registration for Zoom factor property
        /// </summary>
        public static readonly DependencyProperty ZoomFactorProperty =
            DependencyProperty.Register("ZoomFactor", typeof(double), typeof(GanttControl), new PropertyMetadata(100d, OnZoomFactorChanged));

        /// <summary>
        /// Dependency Registration for Base Cell Minimum length
        /// </summary>
        public static readonly DependencyProperty BaseCellMinLengthProperty =
            DependencyProperty.Register("BaseCellMinLength", typeof(double), typeof(GanttControl), new PropertyMetadata(20d));

        /// <summary>
        /// Dependency Registration for Base Cell Maximum length
        /// </summary>
        public static readonly DependencyProperty BaseCellMaxLengthProperty =
            DependencyProperty.Register("BaseCellMaxLength", typeof(double), typeof(GanttControl), new PropertyMetadata(40d));

        /// <summary>
        /// Dependency Registration for Highlighted Items Property
        /// </summary>
        public static readonly DependencyProperty HighlightedItemsProperty =
            DependencyProperty.Register("HighlightedItems", typeof(IList), typeof(GanttControl), new PropertyMetadata(OnHighlightedItemsChagned));

        /// <summary>
        /// Dependency Registration for Highlighted Items Property
        /// </summary>
#if !SILVERLIGHT
        public static readonly DependencyProperty HighlightItemBrushProperty =
            DependencyProperty.Register("HighlightItemBrush", typeof(Brush), typeof(GanttControl), new PropertyMetadata(Brushes.Red));
#else
        public static readonly DependencyProperty HighlightItemBrushProperty =
            DependencyProperty.Register("HighlightItemBrush", typeof(Brush), typeof(GanttControl), new PropertyMetadata(new SolidColorBrush(Colors.Red)));
#endif  

		/// <summary>
        /// Dependency Registration for Auto update hierarchy
        /// </summary>
        public static readonly DependencyProperty UseAutoUpdateHierarchyProperty =
            DependencyProperty.Register("UseAutoUpdateHierarchy", typeof(bool), typeof(GanttControl), new PropertyMetadata(true));

        /// <summary>
        /// Dependency Registration for Resource Container Template Property
        /// </summary>
        public static readonly DependencyProperty ResourceContainerTemplateProperty =
            DependencyProperty.Register("ResourceContainerTemplate", typeof(DataTemplate), typeof(GanttControl), new PropertyMetadata(null));

#if !SILVERLIGHT

        /// <summary>
        /// Dependency Registration for Resource Container Template Selector Property
        /// </summary>
        public static readonly DependencyProperty ResourceContainerTemplateSelectorProperty =
            DependencyProperty.Register("ResourceContainerTemplateSelector", typeof(DataTemplateSelector), typeof(GanttControl), new PropertyMetadata(null));

#endif

		 /// <summary>
        /// Dependency Registration for Show Resizing Tooltip property
        /// </summary>
        public static readonly DependencyProperty ShowResizingTooltipProperty =
            DependencyProperty.Register("ShowResizingTooltip", typeof(bool), typeof(GanttControl), new PropertyMetadata(true));

        /// <summary>
        /// Dependency registration for Chart Row Height property
        /// </summary>
        public static readonly DependencyProperty RowHeightProperty =
            DependencyProperty.Register("RowHeight", typeof(double), typeof(GanttControl), new PropertyMetadata(24d, OnRowHeightChanged));

        /// <summary>
        /// Dependency Registration for Resource Name Placement target property
        /// </summary>
        public static readonly DependencyProperty ResourceNamePlacementProperty =
            DependencyProperty.Register("ResourceNamePlacement", typeof(PlacementMode), typeof(GanttControl), new PropertyMetadata(PlacementMode.Right));

		 /// <summary>
        /// Dependency Registration for ShowGridLinesOnZooming property
        /// </summary>
        public static readonly DependencyProperty ShowGridLinesOnZoomingProperty =
            DependencyProperty.Register("ShowGridLinesOnZooming", typeof(bool), typeof(GanttControl), new PropertyMetadata(false));

        #endregion

        #region Events

        /// <summary>
        /// Occurs when [items source changed].
        /// </summary>
        public event DependencyPropertyChangedEventHandler ItemsSourceChanged;

       

        /// <summary>
        /// Occurs when [template applied].
        /// </summary>
        public event TemplateAppliedEventHandler TemplateApplied;

#if !SILVERLIGHT

        #region Registering Routed Events

        /// <summary>
        /// Registerting ScheduleCellCreated Event
        /// </summary>
        public static readonly RoutedEvent ScheduleCellCreatedEvent = EventManager.RegisterRoutedEvent("ScheduleCellCreated", RoutingStrategy.Bubble, typeof(ScheduleCellCreatedEventHandler), typeof(GanttControl));

        /// <summary>
        /// Registerting StripLineCreated Event
        /// </summary>
        public static readonly RoutedEvent StripLineCreatedevent = EventManager.RegisterRoutedEvent("StripLineCreated", RoutingStrategy.Bubble, typeof(StriplineCreatedEventHandler), typeof(GanttControl));

        /// <summary>
        /// Registerting ResourceContainerCreated Event
        /// </summary>
        public static readonly RoutedEvent ResourceContainerCreatedEvent = EventManager.RegisterRoutedEvent("ResourceContainerCreated", RoutingStrategy.Bubble, typeof(ResourceContainerCreatedEventHandler), typeof(GanttControl));

        /// <summary>
        /// Registerting ZoomChanged Event
        /// </summary>
        public static readonly RoutedEvent ZoomChangedEvent = EventManager.RegisterRoutedEvent("ZoomChanged", RoutingStrategy.Tunnel, typeof(ZoomChangedEventHandler), typeof(GanttControl));

        /// <summary>
        /// Registerting NodeCreated Event
        /// </summary>
        public static readonly RoutedEvent NodeCreatedEvent = EventManager.RegisterRoutedEvent("NodeCreated", RoutingStrategy.Tunnel, typeof(NodeCreatedEventHandler), typeof(GanttControl));

        /// <summary>
        /// Registerting NodeDragCompleted Event
        /// </summary>
        public static readonly RoutedEvent NodeDragCompletedEvent = EventManager.RegisterRoutedEvent("NodeDragCompleted", RoutingStrategy.Tunnel, typeof(NodeDragAndDropEventHandler), typeof(GanttControl));

        /// <summary>
        /// Registerting NodeDragDelta Event
        /// </summary>
        public static readonly RoutedEvent NodeDragDeltaEvent = EventManager.RegisterRoutedEvent("NodeDragDelta", RoutingStrategy.Tunnel, typeof(NodeDragAndDropEventHandler), typeof(GanttControl));

        /// <summary>
        /// Registerting NodeResizingDelta Event
        /// </summary>
        public static readonly RoutedEvent NodeResizingDeltaEvent = EventManager.RegisterRoutedEvent("NodeResizingDelta", RoutingStrategy.Tunnel, typeof(NodeResizingEventHandeler), typeof(GanttControl));

        /// <summary>
        /// Registerting NodeResizingCompleted Event
        /// </summary>
        public static readonly RoutedEvent NodeResizingCompletedEvent = EventManager.RegisterRoutedEvent("NodeResizingCompleted", RoutingStrategy.Tunnel, typeof(NodeResizingEventHandeler), typeof(GanttControl));

        #endregion

#else

        /// <summary>
        /// Occurs when [schedule cell created event].
        /// </summary>
        private event ScheduleCellCreatedEventHandler ScheduleCellCreatedEvent;

        /// <summary>
        /// Occurs when [strip line createdevent].
        /// </summary>
        private event StriplineCreatedEventHandler StripLineCreatedevent;

        /// <summary>
        /// Occurs when [resource container created event].
        /// </summary>
        private event ResourceContainerCreatedEventHandler ResourceContainerCreatedEvent;

        /// <summary>
        /// Occurs when [zoom changed event].
        /// </summary>
        private event ZoomChangedEventHandler ZoomChangedEvent;

        /// <summary>
        /// Occurs when [node created event].
        /// </summary>
        private event NodeCreatedEventHandler NodeCreatedEvent;

        /// <summary>
        /// Occurs when [node drag completed event].
        /// </summary>
        private event NodeDragAndDropEventHandler NodeDragCompletedEvent;

        /// <summary>
        /// Occurs when [node drag delta event].
        /// </summary>
        private event NodeDragAndDropEventHandler NodeDragDeltaEvent;

        /// <summary>
        /// Occurs when [node resizing delta event].
        /// </summary>
        private event NodeResizingEventHandeler NodeResizingDeltaEvent;

        /// <summary>
        /// Occurs when [node resizing completed event].
        /// </summary>
        private event NodeResizingEventHandeler NodeResizingCompletedEvent;
#endif

        #region Routed Events

        /// <summary>
        /// Occurs when [zoom changed].
        /// </summary>
        public event ZoomChangedEventHandler ZoomChanged
        {
            
#if !SILVERLIGHT
            add { this.AddHandler(ZoomChangedEvent, value); }
            remove { this.RemoveHandler(ZoomChangedEvent, value); }
#else
            add { ZoomChangedEvent += value; }
            remove { ZoomChangedEvent -= value; }
#endif
        }

        /// <summary>
        /// Occurs when [schedule cell created].
        /// </summary>
        public event ScheduleCellCreatedEventHandler ScheduleCellCreated
        {
#if !SILVERLIGHT
            add { this.AddHandler(ScheduleCellCreatedEvent, value); }
            remove { this.RemoveHandler(ScheduleCellCreatedEvent, value); }
#else
            add { this.ScheduleCellCreatedEvent += value; }
            remove { this.ScheduleCellCreatedEvent -= value; }
#endif
        }

        /// <summary>
        /// Occurs when [strip line created].
        /// </summary>
        public event StriplineCreatedEventHandler StripLineCreated
        {
#if !SILVERLIGHT
            add { this.AddHandler(StripLineCreatedevent, value); }
            remove { this.RemoveHandler(StripLineCreatedevent, value); }
#else
            add { this.StripLineCreatedevent += value; }
            remove { this.StripLineCreatedevent -= value; }
#endif
        }


        /// <summary>
        /// Occurs when [resource container created].
        /// </summary>
        public event ResourceContainerCreatedEventHandler ResourceContainerCreated
        {
#if !SILVERLIGHT
            add { this.AddHandler(ResourceContainerCreatedEvent, value); }
            remove { this.RemoveHandler(ResourceContainerCreatedEvent, value); }
#else
            add { this.ResourceContainerCreatedEvent += value; }
            remove { this.ResourceContainerCreatedEvent -= value; }
#endif
        }

        /// <summary>
        /// Occurs when [node created].
        /// </summary>
        public event NodeCreatedEventHandler NodeCreated
        {
#if !SILVERLIGHT
            add { this.AddHandler(NodeCreatedEvent, value); }
            remove { this.RemoveHandler(NodeCreatedEvent, value); }
#else
            add { this.NodeCreatedEvent += value; }
            remove { this.NodeCreatedEvent -= value; }
#endif
        }

        /// <summary>
        /// Occurs when [node drag completed].
        /// </summary>
        public event NodeDragAndDropEventHandler NodeDragCompleted
        {
#if !SILVERLIGHT
            add { this.AddHandler(NodeDragCompletedEvent, value); }
            remove { this.RemoveHandler(NodeDragCompletedEvent, value); }
#else
            add { this.NodeDragCompletedEvent += value; }
            remove { this.NodeDragCompletedEvent -= value; }
#endif
        }

        /// <summary>
        /// Occurs when [node drag delta].
        /// </summary>
        public event NodeDragAndDropEventHandler NodeDragDelta
        {
#if !SILVERLIGHT
            add { this.AddHandler(NodeDragDeltaEvent, value); }
            remove { this.RemoveHandler(NodeDragDeltaEvent, value); }
#else
            add { this.NodeDragDeltaEvent += value; }
            remove { this.NodeDragDeltaEvent -= value; }
#endif
        }

        /// <summary>
        /// Occurs when [node resizing delta].
        /// </summary>
        public event NodeResizingEventHandeler NodeResizingDelta
        {
#if !SILVERLIGHT
            add { this.AddHandler(NodeResizingDeltaEvent, value); }
            remove { this.RemoveHandler(NodeResizingDeltaEvent, value); }
#else
            add { this.NodeResizingDeltaEvent += value; }
            remove { this.NodeResizingDeltaEvent -= value; }
#endif
        }

        /// <summary>
        /// Occurs when [node resizing completed].
        /// </summary>
        public event NodeResizingEventHandeler NodeResizingCompleted
        {
#if !SILVERLIGHT
            add { this.AddHandler(NodeResizingCompletedEvent, value); }
            remove { this.RemoveHandler(NodeResizingCompletedEvent, value); }
#else
            add { this.NodeResizingCompletedEvent += value; }
            remove { this.NodeResizingCompletedEvent -= value; }
#endif
        }

        #endregion

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the gantt grid.
        /// </summary>
        /// <value>The gantt grid.</value>
        public GanttGrid GanttGrid
        {
            get
            {
                return _ganttGrid;
            }
            private set
            {
                _ganttGrid = value;
            }
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>The model.</value>
        public GanttModel Model
        {
            get
            {
                return _model ?? (_model = new GanttModel(this));
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is loaded with inbuilt source.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is loaded with inbuilt source; otherwise, <c>false</c>.
        /// </value>
        public bool IsLoadedWithInbuiltSource
        {
            get
            {
                return isLoadedWithInbuiltSource;
            }
        }

        /// <summary>
        /// Gets or sets the start point.
        /// </summary>
        /// <value>
        /// The start point.
        /// </value>
        public double StartPoint
        {
            get { return (double)GetValue(StartPointProperty); }
            set { SetValue(StartPointProperty, value); }
        }

        /// <summary>
        /// Gets or sets the end point.
        /// </summary>
        /// <value>
        /// The end point.
        /// </value>
        public Double EndPoint
        {
            get { return (Double)GetValue(EndPointProperty); }
            set { SetValue(EndPointProperty, value); }
        }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        /// <value>The start time.</value>
        public DateTime StartTime
        {
            get { return (DateTime)GetValue(StartTimeProperty); }
            set { SetValue(StartTimeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the end time.
        /// </summary>
        /// <value>The end time.</value>
        public DateTime EndTime
        {
            get { return (DateTime)GetValue(EndTimeProperty); }
            set { SetValue(EndTimeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the items source.
        /// </summary>
        /// <value>The items source.</value>
        public object ItemsSource
        {
            get { return GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// Gets or sets the task attribute mapping.
        /// </summary>
        /// <value>The task attribute mapping.</value>
        public TaskAttributeMapping TaskAttributeMapping
        {
            get { return (TaskAttributeMapping)GetValue(TaskAttributeMappingProperty); }
            set { SetValue(TaskAttributeMappingProperty, value); }
        }

        /// <summary>
        /// Gets or sets the default start time.
        /// </summary>
        /// <value>
        /// The default start time.
        /// </value>
        public TimeSpan DefaultStartTime
        {
            get { return (TimeSpan)GetValue(DefaultStartTimeProperty); }
            set { SetValue(DefaultStartTimeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the default end time.
        /// </summary>
        /// <value>
        /// The default end time.
        /// </value>
        public TimeSpan DefaultEndTime
        {
            get { return (TimeSpan)GetValue(DefaultEndTimeProperty); }
            set { SetValue(DefaultEndTimeProperty, value); }
        }


        /// <summary>
        /// Gets or sets the week begins on.
        /// </summary>
        /// <value>The week begins on.</value>
        public DayOfWeek WeekBeginsOn
        {
            get { return (DayOfWeek)GetValue(WeekBeginsOnProperty); }
            set { SetValue(WeekBeginsOnProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is FY numbering endbled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is FY numbering endbled; otherwise, <c>false</c>.
        /// </value>
        public bool IsFYNumberingEndbled
        {
            get { return (bool)GetValue(IsFYNumberingEnabledProperty); }
            set { SetValue(IsFYNumberingEnabledProperty, value); }
        }

        /// <summary>
        /// Gets or sets the fiscal year begins on.
        /// </summary>
        /// <value>The fiscal year begins on.</value>
        public Month FiscalYearBeginsOn
        {
            get { return (Month)GetValue(FiscalYearBeginsOnProperty); }
            set { SetValue(FiscalYearBeginsOnProperty, value); }
        }

        ///// <summary>
        ///// Gets or sets the holidays.
        ///// </summary>
        ///// <value>The holidays.</value>
        //public ObservableCollection<DateTime> Holidays
        //{
        //    get { return (ObservableCollection<DateTime>)GetValue(HolidaysProperty); }
        //    set { SetValue(HolidaysProperty, value); }
        //}

        /// <summary>
        /// Gets or sets the current date line.
        /// </summary>
        /// <value>The current date line.</value>
        public Line CurrentDateLine
        {
            get { return (Line)GetValue(CurrentDateLineProperty); }
            set { SetValue(CurrentDateLineProperty, value); }
        }

        /// <summary>
        /// Gets or sets the stick current date line to.
        /// </summary>
        /// <value>The stick current date line to.</value>
        public CurentDateLinePositions StickCurrentDateLineTo
        {
            get { return (CurentDateLinePositions)GetValue(StickCurrentDateLineToProperty); }
            set { SetValue(StickCurrentDateLineToProperty, value); }
        }

        /// <summary>
        /// Gets or sets the type of the schedule.
        /// </summary>
        /// <value>The type of the schedule.</value>
        public ScheduleType ScheduleType
        {
            get { return (ScheduleType)GetValue(ScheduleTypeProperty); }
            set { SetValue(ScheduleTypeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the tool tip template.
        /// </summary>
        /// <value>The tool tip template.</value>
        public DataTemplate ToolTipTemplate
        {
            get { return (DataTemplate)GetValue(ToolTipTemplateProperty); }
            set { SetValue(ToolTipTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the resource collection.
        /// </summary>
        /// <value>The resource collection.</value>
        public ObservableCollection<Resource> ResourceCollection
        {
            get { return (ObservableCollection<Resource>)GetValue(ResourceCollectionProperty); }
            set { SetValue(ResourceCollectionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the task node background.
        /// </summary>
        /// <value>The task node background.</value>
        public Brush TaskNodeBackground
        {
            get { return (Brush)GetValue(TaskNodeBackgroundProperty); }
            set { SetValue(TaskNodeBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the connector stroke.
        /// </summary>
        /// <value>The connector stroke.</value>
        public Brush ConnectorStroke
        {
            get { return (Brush)GetValue(ConnectorStrokeProperty); }
            set { SetValue(ConnectorStrokeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the visual style.
        /// </summary>
        /// <value>The visual style.</value>
        public VisualStyle VisualStyle
        {
            get { return (VisualStyle)this.GetValue(VisualStyleProperty); }
            set { this.SetValue(VisualStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the grid header background.
        /// </summary>
        /// <value>The grid header background.</value>
        public Brush GridHeaderBackground
        {
            get { return (Brush)GetValue(GridHeaderBackgroundProperty); }
            set { SetValue(GridHeaderBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the grid header foreground.
        /// </summary>
        /// <value>The grid header foreground.</value>
        public Brush GridHeaderForeground
        {
            get { return (Brush)GetValue(GridHeaderForegroundProperty); }
            set { SetValue(GridHeaderForegroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the schedule background.
        /// </summary>
        /// <value>The schedule background.</value>
        public Brush ScheduleBackground
        {
            get { return (Brush)GetValue(ScheduleBackgroundProperty); }
            set { SetValue(ScheduleBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the schedule fore ground.
        /// </summary>
        /// <value>The schedule fore ground.</value>
        public Brush ScheduleForeground
        {
            get { return (Brush)GetValue(ScheduleForegroundProperty); }
            set { SetValue(ScheduleForegroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the schedule border thickness.
        /// </summary>
        /// <value>The schedule border thickness.</value>
        public Thickness ScheduleBorderThickness
        {
            get { return (Thickness)GetValue(ScheduleBorderThicknessProperty); }
            set { SetValue(ScheduleBorderThicknessProperty, value); }
        }

        /// <summary>
        /// Gets or sets the schedule border brush.
        /// </summary>
        /// <value>The schedule border brush.</value>
        public Brush ScheduleBorderBrush
        {
            get { return (Brush)GetValue(ScheduleBorderBrushProperty); }
            set { SetValue(ScheduleBorderBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets non working hours background
        /// </summary>
        /// <value>
        /// The non working hours background.
        /// </value>
        public Brush NonWorkingHoursBackground
        {
            get { return (Brush)GetValue(NonWorkingHoursBackgroundProperty); }
            set { SetValue(NonWorkingHoursBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the progress Indicator Background brush.
        /// </summary>
        /// <value>
        /// The progress Indicator Background brush.
        /// </value>
        public Brush ProgressIndicatorBackground
        {
            get { return (Brush)GetValue(ProgressIndicatorBackgroundProperty); }
            set { SetValue(ProgressIndicatorBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [add chartlines].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [chartlines]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowChartLines
        {
            get { return (bool)GetValue(ShowChartLinesProperty); }
            set { SetValue(ShowChartLinesProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show strip lines].
        /// </summary>
        /// <value><c>true</c> if [show strip lines]; otherwise, <c>false</c>.</value>
        public bool ShowStripLines
        {
            get { return (bool)GetValue(ShowStripLinesProperty); }
            set { SetValue(ShowStripLinesProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show non working Hours background].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show non working Hours background]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowNonWorkingHoursBackground
        {
            get { return (bool)GetValue(ShowNonWorkingHoursBackgroundProperty); }
            set { SetValue(ShowNonWorkingHoursBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the resource name visibility.
        /// </summary>
        /// <value>The resource name visibility.</value>
        public Visibility ResourceNameVisibility
        {
            get { return (Visibility)GetValue(ResourceNameVisibilityProperty); }
            set { SetValue(ResourceNameVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show add new column].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show add new column]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowAddNewColumn
        {
            get { return (bool)GetValue(ShowAddNewColumnProperty); }
            set { SetValue(ShowAddNewColumnProperty, value); }
        }

        /// <summary>
        /// Gets the selected items.
        /// </summary>
        /// <value>The selected items.</value>
        public ObservableCollection<object> SelectedItems
        {
            get
            {
                return _selectedItems ?? (_selectedItems = new ObservableCollection<object>());
            }
        }

        /// <summary>
        /// Gets or sets the custom schedule source to draw the scheule.
        /// </summary>
        /// <value>The custom schedule source.</value>
        public IList<GanttScheduleRowInfo> CustomScheduleSource
        {
            get { return (IList<GanttScheduleRowInfo>)GetValue(CustomScheduleSourceProperty); }
            set { SetValue(CustomScheduleSourceProperty, value); }
        }

        /// <summary>
        /// Gets or sets the strip lines.
        /// </summary>
        /// <value>The strip lines.</value>
        public IEnumerable StripLines
        {
            get { return (IEnumerable)GetValue(StripLinesProperty); }
            set { SetValue(StripLinesProperty, value); }
        }

        /// <summary>
        /// Gets/Sets the Name of the current Project
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show date with time].
        /// </summary>
        /// <value><c>true</c> if [show date with time]; otherwise, <c>false</c>.</value>
        public bool ShowDateWithTime
        {
            get { return (bool)GetValue(ShowDateWithTimeProperty); }
            set { SetValue(ShowDateWithTimeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the width of the grid.
        /// </summary>
        /// <value>The width of the grid.</value>
        public GridLength GridWidth
        {
            get { return (GridLength)GetValue(GridWidthProperty); }
            set { SetValue(GridWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the width of the chart.
        /// </summary>
        /// <value>The width of the chart.</value>
        public GridLength ChartWidth
        {
            get { return (GridLength)GetValue(ChartWidthProperty); }
            set { SetValue(ChartWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the layout mode.
        /// </summary>
        /// <value>The layout mode.</value>
        public GanttLayoutMode LayoutMode
        {
            get { return (GanttLayoutMode)GetValue(LayoutModeProperty); }
            set { SetValue(LayoutModeProperty, value); }
        }


        /// <summary>
        /// Gets or sets a value indicating whether [use on demand schedule].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [use on demand schedule]; otherwise, <c>false</c>.
        /// </value>
        public bool UseOnDemandSchedule
        {
            get { return (bool)GetValue(UseOnDemandScheduleProperty); }
            set { SetValue(UseOnDemandScheduleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the zoom factor.
        /// </summary>
        /// <value>The zoom factor.</value>
        public double ZoomFactor
        {
            get { return (double)GetValue(ZoomFactorProperty); }
            set { SetValue(ZoomFactorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the length of the base cell min.
        /// </summary>
        /// <value>The length of the base cell min.</value>
        public double BaseCellMinLength
        {
            get { return (double)GetValue(BaseCellMinLengthProperty); }
            set { SetValue(BaseCellMinLengthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the length of the base cell max.
        /// </summary>
        /// <value>The length of the base cell max.</value>
        public double BaseCellMaxLength
        {
            get { return (double)GetValue(BaseCellMaxLengthProperty); }
            set { SetValue(BaseCellMaxLengthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the highlighted items.
        /// </summary>
        /// <value>The highlighted items.</value>
        public IList HighlightedItems
        {
            get { return (IList)GetValue(HighlightedItemsProperty); }
            set { SetValue(HighlightedItemsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the highlight brush.
        /// </summary>
        /// <value>The highlight brush.</value>
        public Brush HighlightItemBrush
        {
            get { return (Brush)GetValue(HighlightItemBrushProperty); }
            set { SetValue(HighlightItemBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [use auto update hierarchy].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [use auto update hierarchy]; otherwise, <c>false</c>.
        /// </value>
        public bool UseAutoUpdateHierarchy
        {
            get { return (bool)GetValue(UseAutoUpdateHierarchyProperty); }
            set { SetValue(UseAutoUpdateHierarchyProperty, value); }
        }

        /// <summary>
        /// Gets or sets the resource template.
        /// </summary>
        /// <value>The resource template.</value>
        public DataTemplate ResourceContainerTemplate
        {
            get { return (DataTemplate)GetValue(ResourceContainerTemplateProperty); }
            set { SetValue(ResourceContainerTemplateProperty, value); }
        }

#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets the resource template selector.
        /// </summary>
        /// <value>The resource template selector.</value>
        public DataTemplateSelector ResourceContainerTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ResourceContainerTemplateSelectorProperty); }
            set { SetValue(ResourceContainerTemplateSelectorProperty, value); }
        }
#endif

		/// <summary>
        /// Gets or sets a value indicating whether [show resizing tooltip].
        /// </summary>
        /// <value><c>true</c> if [show resizing tooltip]; otherwise, <c>false</c>.</value>
        public bool ShowResizingTooltip
        {
            get { return (bool)GetValue(ShowResizingTooltipProperty); }
            set { SetValue(ShowResizingTooltipProperty, value); }
        }

        /// <summary>
        /// Gets or sets the height of the row.
        /// </summary>
        /// <value>The height of the row.</value>
        public double RowHeight
        {
            get { return (double)GetValue(RowHeightProperty); }
            set { SetValue(RowHeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets the resource placement target.
        /// </summary>
        /// <value>The resource placement target.</value>
        public PlacementMode ResourceNamePlacement
        {
            get { return (PlacementMode)GetValue(ResourceNamePlacementProperty); }
            set { SetValue(ResourceNamePlacementProperty, value); }
        }
		
		 /// <value>
        /// 	<c>true</c> if [show grid lines on zooming]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowGridLinesOnZooming
        {
            get { return (bool)GetValue(ShowGridLinesOnZoomingProperty); }
            set { SetValue(ShowGridLinesOnZoomingProperty, value); }
        }
       
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GanttControl"/> class.
        /// </summary>
        public GanttControl()
        {
#if SILVERLIGHT
            DefaultStyleKey = typeof(GanttControl);
#else
            if (IsSecurityGranted)
            {
                ValidateLicense();
            }
#endif
            //Default CurrentDateLine value Should defined here. If defined the Default value in XAML means it will throw the Exception.
            CurrentDateLine = new Line() { VerticalAlignment = System.Windows.VerticalAlignment.Stretch, Stroke = new SolidColorBrush(Colors.Orange), StrokeThickness = 1 };
            this.TaskAttributeMapping = TaskAttributeMapping.Default;
            this.ItemsSource = this.Model.InbuiltTaskCollection;
        }

        /// <summary>
        /// Initializes the <see cref="GanttControl"/> class.
        /// </summary>
        static GanttControl()
        {
#if !SILVERLIGHT
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GanttControl), new FrameworkPropertyMetadata(typeof(GanttControl)));
#endif
        }

#if !SILVERLIGHT
        /// <summary>
        /// Checks whether security permission can be granted. Read-only.
        /// </summary>
        internal static bool IsSecurityGranted
        {
            get
            {
                SecurityPermission perm = new SecurityPermission(PermissionState.Unrestricted);
                bool bResult = false;
                try
                {
                    perm.Demand();
                    bResult = true;
                }
                catch (Exception) { }
                return bResult;
            }
        }

        /// <summary>
        /// Checks whether license is valid.
        /// </summary>
        internal static void ValidateLicense()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
#if AllowUnsafeCode
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(GanttControl));
#endif
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
        }

#endif
        #endregion

        #region Dependecny callback

        #region Start Point Call Back

        /// <summary>
        /// Called when [start point changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnStartPointChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GanttControl gantt = d as GanttControl;

            if (gantt == null)
                return;

            // Forcing the changes in Schedule and Chart
            if (gantt.GanttSchedule != null)
                gantt.GanttSchedule.Start = (double)args.NewValue;

            if (gantt.GanttChart != null)
                gantt.GanttChart.StartPoint = (double)args.NewValue;
        }

        #endregion

        #region EndPoint Call Back

        /// <summary>
        /// Called when [end point changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnEndPointChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GanttControl gantt = d as GanttControl;

            if (gantt == null)
                return;

            // Forcing the changes in Schedule and Chart
            if (gantt.GanttSchedule != null)
                gantt.GanttSchedule.End = (Double)args.NewValue;

            if (gantt.GanttChart != null)
                gantt.GanttChart.EndPoint = (double)args.NewValue;
        }

        #endregion

        #region Start Time callback

        /// <summary>
        /// Called when [start time changed].
        /// </summary>
        /// <param name="sender">The d.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnStartTimeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GanttControl gantt = sender as GanttControl;

            if (gantt == null)
                return;

            // Forcing the changes in Schedule and Chart
            if (gantt.GanttSchedule != null)
                gantt.GanttSchedule.StartTime = (DateTime)args.NewValue;

            if (gantt.GanttChart != null)
                gantt.GanttChart.StartTime = (DateTime)args.NewValue;
        }

        #endregion

        #region End time callback

        /// <summary>
        /// Called when [end time changed].
        /// </summary>
        /// <param name="sender">The d.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnEndTimeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GanttControl gantt = sender as GanttControl;

            if (gantt == null)
                return;

            // Forcing the changes in Schedule and Chart
            if (gantt.GanttSchedule != null)
                //if(gantt.GanttSchedule.IsTemplateApplied)
                gantt.GanttSchedule.EndTime = (DateTime)args.NewValue;

            if (gantt.GanttChart != null)
                gantt.GanttChart.EndTime = (DateTime)args.NewValue;
        }

        #endregion

        #region Items souce callback

        /// <summary>
        /// Called when [items source changed].
        /// </summary>
        /// <param name="sender">The d.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnItemsSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GanttControl gantt = sender as GanttControl;

            if (gantt == null)
                return;
            if (args.NewValue != gantt.Model.InbuiltTaskCollection)
                gantt.isLoadedWithInbuiltSource = false;

            if (gantt.isTemplateApplied)
            {
                gantt.ClearProperties();
                gantt.GanttSchedule.RedrawSchedule();
                gantt.SetItemsSourceOnModel(args.NewValue);

                gantt.GanttGrid.ItemsSource = gantt.ItemsSource;
                gantt.GanttChart.ItemsSource = gantt.Model.ExpandedCollection;
                gantt.GanttGrid.CalculateHeaderHeight();
            }
            else
            {
                gantt.isItemsSourceChangedBeforeOnApplyTemplate = true;
            }

            if (gantt.ItemsSourceChanged != null)
                gantt.ItemsSourceChanged(gantt, args);

        }

        private static void OnToolTipTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var gantt = sender as GanttControl;
            if (gantt == null)
                return;

            if (gantt.isTemplateApplied && gantt.GanttChart != null)
                gantt.GanttChart.ToolTipTemplate = (DataTemplate)args.NewValue;
            else
                gantt.isToolTipAppliedBeforeOnApplyTemplate = true;
        }

        #endregion

        #region Attribute Mapping call back

        /// <summary>
        /// Called when [task attribute mapping property changed].
        /// </summary>
        /// <param name="sender">The dpo.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnTaskAttributeMappingPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GanttControl gant = sender as GanttControl;

            if (gant == null) return;

            if (gant.isTemplateApplied)
            {
                gant.Model.TaskAttributeMapping = (TaskAttributeMapping)args.NewValue;

                // Generte the tool tip template based on current mapping names
                gant.GanttChart.InBuiltTooltipTemplate = gant.GenerateTooltipTemplate();
            }
            else
            {
                gant.isTaskMappingChangedBeforeOnApplyTemplate = true;
            }
        }

        #endregion

        #region OnResource Collection call back

        /// <summary>
        /// Called when [resource collection changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnResourceCollectionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GanttControl gant = sender as GanttControl;

            if (gant == null) return;

            if (gant.isTemplateApplied)
            {
                gant.Model.Resources = (ObservableCollection<Resource>)args.NewValue;
            }
            else
            {
                gant.isResourcesChangedBeforOnApplyTemplate = true;
            }
        }

        #endregion

        #region Default start & end time

        public static void OnDefaulTimePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GanttControl gantt = sender as GanttControl;
            if (gantt == null)
                return;

            if (gantt.isTemplateApplied)
                gantt.GanttChart.UpdateChartBackGround();
        }

        #endregion

        #region FY Numbering
        /// <summary>
        /// Called when IsFYNumberingEnabled Property Changed
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnIsFYNumberingPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GanttControl gantt = sender as GanttControl;
            if (gantt == null)
                return;
            if (gantt.isTemplateApplied)
            {
                gantt.IsFYNumberingEndbled = (bool)args.NewValue;
                gantt.GanttSchedule.IsWeekBeginsOnSet = true;
                gantt.GanttSchedule.IsFYNumberingEnabled = gantt.IsFYNumberingEndbled;
            }
        }

        #endregion

        #region FiscalYearProperty
        /// <summary>
        /// Called when FiscalYearStartsIn Property Changed
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnFiscalYearPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GanttControl gantt = sender as GanttControl;
            if (gantt == null)
                return;
            gantt.FiscalYearBeginsOn = (Month)args.NewValue;
            if (gantt.isTemplateApplied)
            {
                gantt.GanttSchedule.IsWeekBeginsOnSet = true;
                gantt.GanttSchedule.FiscalYearBeginsOn = gantt.FiscalYearBeginsOn;
                // gantt.GanttSchedule.InvalidateRowPresenters();

            }

        }

        public static void OnHolidaysPropertyPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            //            GanttControl gantt = sender as GanttControl;
            //            if (gantt != null)
            //            {
            //                gantt.Holidays = (ObservableCollection<DateTime>)args.NewValue;
            //                gantt.StartTime = gantt.Model.GetStartDate();
            //#if !SILVERLIGHT
            //                gantt.GanttChart.Items.Refresh();
            //                gantt.GanttGrid.ItemsSource = gantt.Model.TaskCollection;
            //#else
            //                gt.GanttGrid.ItemsSource = gt.Model.TaskCollection;
            //                gt.GanttChart.ItemsSource = gt.Model.TaskCollection;
            //                gt.GanttChart.ItemsSource = gt.Model.FlatCollection;
            //#endif
            //            }
        }

        #endregion

        #region StripLines call back

        /// <summary>
        /// Called when [strip lines changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnStripLinesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GanttControl gantt = sender as GanttControl;
            if (gantt == null || gantt.StripLines == null)
            {
                return;
            }
            else
            {
                if (gantt.GanttChart != null)
                {
                    gantt.GanttChart.StriplinePanel.ResetStripline();
                }
            }
        }

        #endregion

        #region GanttSchedule ItemsSource Call back

        private static void OnCustomScheduleSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GanttControl gantt = sender as GanttControl;

            if (gantt == null || (gantt.ScheduleType != ScheduleType.CustomDateTime && gantt.ScheduleType != ScheduleType.CustomNumeric))
                return;

            if (gantt.isTemplateApplied)
            {
                gantt.GanttSchedule.ItemsSource = (IList<GanttScheduleRowInfo>)args.NewValue;
                gantt.GanttSchedule.baseSource = new List<GanttScheduleRowInfo>(gantt.GanttSchedule.ItemsSource.Cast<GanttScheduleRowInfo>());

                // Since the Start date and End date is fully dependent on the schedule items source, 
                // InitializeStartAndEnd is invoked here on chaning the items soruce dynamically.
                if (gantt.ItemsSource != null)
                {
                    gantt.InitializeStartAndEnd();
                }

                // Reinitializing the calcuating variable in schedule based on the current start and end.
                gantt.GanttSchedule.ReInitializeUnits();
                gantt.SetScheduleType(gantt.ScheduleType, true);
            }
            else
            {
                if (gantt.ScheduleType == ScheduleType.CustomNumeric || gantt.ScheduleType == ScheduleType.CustomDateTime)
                    gantt.isCustSchSrcChangedBeforeOnApplyTemplate = true;
            }
        }
        #endregion

        #region Layout width call back

#if SILVERLIGHT
        /// <summary>
        /// Called when [grid width changed].
        /// </summary>
        /// <param name="dep">The dep.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnGridWidthChanged(DependencyObject dep, DependencyPropertyChangedEventArgs args)
        {
            GanttControl gantt = dep as GanttControl;

            if (gantt == null || gantt.LayoutGrid == null)
            {
                gantt.isLayoutWidthChangedBeforeOnApplyTemplate = true;
                return;
            }

            // specifying the width to Grid region
            if (gantt.LayoutGrid.ColumnDefinitions.Count > 1)
                gantt.LayoutGrid.ColumnDefinitions[0].Width = (GridLength)args.NewValue;
        }

        /// <summary>
        /// Called when [chart width changed].
        /// </summary>
        /// <param name="dep">The dep.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnChartWidthChanged(DependencyObject dep, DependencyPropertyChangedEventArgs args)
        {
            GanttControl gantt = dep as GanttControl;

            if (gantt == null || gantt.LayoutGrid == null)
            {
                gantt.isLayoutWidthChangedBeforeOnApplyTemplate = true;
                return;
            }

            // specifying the width to chart region
            if (gantt.LayoutGrid.ColumnDefinitions.Count > 2)
                gantt.LayoutGrid.ColumnDefinitions[2].Width = (GridLength)args.NewValue;
        }

#endif
        /// <summary>
        /// Called when [layout mode changed].
        /// </summary>
        /// <param name="dep">The dep.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnLayoutModeChanged(DependencyObject dep, DependencyPropertyChangedEventArgs args)
        {
            GanttControl gantt = dep as GanttControl;

            if (gantt == null)
                return;

            // Specifying the width based on the layout mode
            switch ((GanttLayoutMode)args.NewValue)
            {
                case GanttLayoutMode.Default:
                    gantt.GridWidth = new GridLength(50, GridUnitType.Star);
                    gantt.ChartWidth = new GridLength(50, GridUnitType.Star);
                    break;
                case GanttLayoutMode.FitToGrid:
                    gantt.GridWidth = new GridLength(50, GridUnitType.Star);
                    gantt.ChartWidth = new GridLength(0, GridUnitType.Star);
                    break;
                case GanttLayoutMode.FitToChart:
                    gantt.GridWidth = new GridLength(0, GridUnitType.Star);
                    gantt.ChartWidth = new GridLength(50, GridUnitType.Star);
                    break;
            }

#if SILVERLIGHT
            if (gantt.LayoutGrid == null)
            {
                gantt.isLayoutWidthChangedBeforeOnApplyTemplate = true;
            }
#endif
        }

#if SILVERLIGHT
        /// <summary>
        /// Sets the width of the layout.
        /// </summary>
        private void SetLayoutWidth()
        {
            if (this.LayoutGrid == null || this.LayoutGrid.ColumnDefinitions.Count < 3)
                return;

            // Specifying the width for Grid region
            this.LayoutGrid.ColumnDefinitions[0].Width = this.GridWidth;
            // Specifying the width for Chart region
            this.LayoutGrid.ColumnDefinitions[2].Width = this.ChartWidth;
        }
#endif

        #endregion

        #region Schedule Type

        /// <summary>
        /// Called when [stick currnet date line to propertychanged].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnStickCurrnetDateLineToPropertychanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GanttControl gantt = sender as GanttControl;
            if (gantt == null)
                return;
            if (gantt.GanttChart != null)
                gantt.GanttChart.StickCurrentDateLineTo = (CurentDateLinePositions)args.NewValue;
        }

        /// <summary>
        /// Called when [schedule type changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnScheduleTypeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GanttControl gantt = sender as GanttControl;

            if (gantt == null)
                return;

            if (!gantt.isTemplateApplied)
            {
                gantt.isScheduleTypeChandedBeforeOnApplyTemplate = true;
                return;
            }

            if ((ScheduleType)args.NewValue == ScheduleType.CustomDateTime)
            {
                gantt.GanttSchedule.InitializeUnitValues(gantt.CustomScheduleSource);
                gantt.GanttSchedule.ItemsSource = gantt.CustomScheduleSource;
            }

            gantt.SetScheduleType((ScheduleType)args.NewValue, true);
        }

        /// <summary>
        /// Called When WeekBeginsOn Changed
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/>instance containing event data.</param>
        public static void OnWeekBeginsOnChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GanttControl gantt = sender as GanttControl;

            if (gantt == null)
                return;

            if (gantt.isTemplateApplied)
            {
                gantt.GanttSchedule.WeekBeginsOn = (DayOfWeek)args.NewValue;
                // gantt.GanttSchedule.InvalidateRowPresenters();
                gantt.GanttChart.UpdateChartBackGround();
            }
            else
                gantt.isWeekBeginsOnsetBeforeOnApplyTemplate = true;
        }

        #endregion

        #region Visual Style changed callback

        /// <summary>
        /// Called when [visual style changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnVisualStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GanttControl gantt = d as GanttControl;

            if (gantt.GanttChart != null)
            {
                //Removing the old CurrentDateLine. Because if we didn't remove this when we change the visual style dynamicaly it will throw the Exception. 
                gantt.GanttChart.RemoveChildren();
            }

            var rd = new ResourceDictionary();
            var value = ((VisualStyle)args.NewValue).ToString();

            gantt.isItemsSourceChangedBeforeOnApplyTemplate = true;

            try
            {
                if (value == "Office2010Blue")
                {
                    gantt.ApplyVisualStyle("Generic", rd);
                }
                else 
                {
                    gantt.ApplyVisualStyle(value, rd);
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Applies the Selected visual style.
        /// </summary>
        /// <param name="styleName">Name of the style.</param>
        /// <param name="resDic">The res dic.</param>
        private void ApplyVisualStyle(string styleName, ResourceDictionary resDic)
        {
            string URI = string.Empty;
#if !SILVERLIGHT
            URI = "/Syncfusion.Gantt.Wpf;component/Themes/" + styleName + ".xaml";

#else  
             URI = "/Syncfusion.Gantt.Silverlight;component/Themes/" + styleName + ".xaml";     
                  
#endif
            resDic.Source = new Uri(URI, UriKind.RelativeOrAbsolute);

            RemoveDictionaryIfExist(this, resDic);
            this.Resources.MergedDictionaries.Add(resDic);
        }

        /// <summary>
        /// Removes the dictionary if exist.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="dictionary">The dictionary.</param>
        private static void RemoveDictionaryIfExist(GanttControl element, ResourceDictionary dictionary)
        {
            if (element != null)
            {

                for (int i = 0; i < element.Resources.MergedDictionaries.Count; i++)
                {
                    var rdic = element.Resources.MergedDictionaries[i];
                    if (rdic.Source == dictionary.Source)
                    {
                        element.Resources.MergedDictionaries.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        #endregion

        #region Zoom Factor Changed

        /// <summary>
        /// Called when [zoom factor changed].
        /// </summary>
        /// <param name="dep">The dep.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnZoomFactorChanged(DependencyObject dep, DependencyPropertyChangedEventArgs args)
        {
            GanttControl gantt = dep as GanttControl;

            if (gantt.GanttSchedule!=null && gantt.GanttSchedule.baseSource != null)
                gantt.SetZoomFactor();
            else
                gantt.isZoomFactorSetBeforeScheduleLoaded = true;
        }

        #endregion

        #region Highlighted Items 

        /// <summary>
        /// Called when [highlighted items chagned].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnHighlightedItemsChagned(object sender, DependencyPropertyChangedEventArgs args)
        {
            GanttControl gantt = sender as GanttControl;

            if (gantt.isTemplateApplied)
            {
                gantt.Model.HighlightedItems = (IList)args.NewValue;
                gantt.GanttChart.RaiseHighlightedItemsChanged(args);
            }
            else
                gantt.isHghItemsChangedBfrOnApplyTempalte = true;
        }

        #endregion

        #region Row Height Changed

        /// <summary>
        /// Called when [row height changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnRowHeightChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            GanttControl gantt = sender as GanttControl;
            if (gantt != null && gantt.GanttGrid != null)
            {
                gantt.GanttGrid.Model.RowHeights.DefaultLineSize = (double)args.NewValue;
            }
            else
                gantt.isRowHeightAppliedBeforeGridLoaded = true;
        }

        #endregion

        #endregion

        #region Helper methods

        /// <summary>
        /// Sets the items source on model.
        /// </summary>
        /// <param name="itemsSource">The items source.</param>
        private void SetItemsSourceOnModel(object itemsSource)
        {
            // populate the source list with current items source.
            Model.SetSourceList(itemsSource);

            // Initialize the start and end based on the schedule type and current items source.
            InitializeStartAndEnd();

            // Reinitialize the cell width and calculating variable in schedule
            this.GanttSchedule.ReInitializeUnits();
        }

        /// <summary>
        /// Initializes the start and end.
        /// </summary>
        internal void InitializeStartAndEnd()
        {
            // Calcuating the Start date and End date is fully dependent on the schedule items source, 
            // based on the cells per unit of the least item of cell the start date and end date will be calculated, 
            // hence this code is separated as a method and invoked on other places.

            // Sets the Start And End for DateTime Schedule
            if (this.ScheduleType != ScheduleType.CustomNumeric)
            {
                StartTime = Model.GetStartDate();
                EndTime = Model.GetEndDate();
            }

            // Sets the startpoint and endpoint for numeric schedule.
            else
            {
                StartPoint = Model.GetStartPoint();
                EndPoint = Model.GetEndPoint();
            }
        }

        private void SetZoomFactor()
        {
            // Validating the initial check points
            if (this.ScheduleType == ScheduleType.CustomNumeric || this.GanttSchedule == null || !this.GanttSchedule.IsTemplateApplied || (double)this.ZoomFactor <= 0)
                return;

            // Checking for existance of zoom info
             this.ZoomInfo = new ZoomChangedEventArgs();

            // Assigning the schedule soruce to the header info of the zoom info
            this.ZoomInfo.ScheduleHeaderInfo = this.GanttSchedule.ItemsSource as IList<GanttScheduleRowInfo>;

            // Assigning the new zoom factor
            this.ZoomInfo.ZoomFactor = this.ZoomFactor;

            // Assigning the min and max constraings of the zoom info
            this.ZoomInfo.BaseCelltMaxLength = this.BaseCellMaxLength;
            this.ZoomInfo.BaseCelltMinLength = this.BaseCellMinLength;

            // checking for delegates to invoke the zoom changed event
#if !SILVERLIGHT
            this.ZoomInfo.RoutedEvent = GanttControl.ZoomChangedEvent;
#endif
            this.RaiseZoomChangedEvent(this.ZoomInfo);

            // Validating the application level handling for zooming
            if (this.ZoomInfo.Handled)
            {
                // Assigning the new source to the schedule
                this.GanttSchedule.ItemsSource = this.ZoomInfo.ScheduleHeaderInfo;

                // Calcualting the start and end date of schedule based on the new source
                this.InitializeStartAndEnd();

                // ReInitializing the schedule values to draw the new schedule.
                this.GanttSchedule.ReInitializeUnits();

                // Recalculating the grid header height
                this.GanttGrid.CalculateHeaderHeight();
            }
            else
            {
                // Applying the zoom factor on schedule.
                this.GanttSchedule.SetZoomFactor(this.ZoomInfo);

                // Recalculating the grid header height
                this.GanttGrid.CalculateHeaderHeight();
            }

            this.GanttChart.StriplinePanel.ResetStripline();
            this.GanttChart.BackgroundPanel.ResetBackground();
        }

        /// <summary>
        /// Sets the type of the schedule.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="forceRefresh">if set to <c>true</c> [force refresh].</param>
        internal void SetScheduleType(ScheduleType type, bool forceRefresh)
        {
            GanttSchedule.ScheduleType = type;
            GanttGrid.CalculateHeaderHeight();

            if (forceRefresh)
            {
                GanttChart.BackgroundPanel.ResetBackground();
                GanttChart.StriplinePanel.ResetStripline();
#if !SILVERLIGHT
                GanttChart.Items.Refresh();
#else               
                GanttChart.InvalidateArrange();
#endif
            }
        }

        /// <summary>
        /// Ensures the properties.
        /// </summary>
        private void EnsureProperties()
        {
#if SILVERLIGHT
            if (isLayoutWidthChangedBeforeOnApplyTemplate)
            {
                // To set the layout width
                SetLayoutWidth();
            }
#endif
            if (isTaskMappingChangedBeforeOnApplyTemplate)
            {
                Model.TaskAttributeMapping = TaskAttributeMapping;

                // To generate the tool tip based on existance of custom tool tip template.
                if (this.GanttChart != null && this.ToolTipTemplate == null)
                {
                    // Generate the tool tip based on current mapping names
                    this.GanttChart.InBuiltTooltipTemplate = this.GenerateTooltipTemplate();
                }
            }

            if (isResourcesChangedBeforOnApplyTemplate)
            {
                Model.Resources = this.ResourceCollection;
            }

            // Schedule items source should be assigned before calcuating the start and end time
            if (isCustSchSrcChangedBeforeOnApplyTemplate)
            {
                this.GanttSchedule.ItemsSource = this.CustomScheduleSource;
            }

            // Since schedule items source will be calused based on its type, 
            // it should be assigned before calcuating the start and end time
            if (isScheduleTypeChandedBeforeOnApplyTemplate)
            {
                this.SetScheduleType(this.ScheduleType, false);
            }
            else if (this.ScheduleType == Gantt.ScheduleType.WeekWithDays)
            {
                this.GanttSchedule.RedrawSchedule();
            }

            if (isItemsSourceChangedBeforeOnApplyTemplate)
            {
                this.SetItemsSourceOnModel(this.ItemsSource);

                this.GanttGrid.ItemsSource = this.ItemsSource;
                this.GanttChart.ItemsSource = this.Model.ExpandedCollection;
            }

            if (this.isHghItemsChangedBfrOnApplyTempalte)
            {
                this.Model.HighlightedItems = this.HighlightedItems;
            }

            if (isWeekBeginsOnsetBeforeOnApplyTemplate)
            {
                this.GanttSchedule.WeekBeginsOn = this.WeekBeginsOn;
            }

            // Sets the initial schedule values
            this.SetScheduleValues();
        }

        /// <summary>
        /// Sets the initial schedule values.
        /// </summary>
        private void SetScheduleValues()
        {
            if (this.ScheduleType == ScheduleType.CustomNumeric)
            {
                this.GanttSchedule.Start = this.StartPoint;
                //  this.GanttChart.StartPoint = this.StartPoint;

                this.GanttSchedule.End = this.EndPoint;
                //  this.GanttChart.EndPoint = this.EndPoint;
            }
            this.GanttSchedule.IsFYNumberingEnabled = this.IsFYNumberingEndbled;
            this.GanttSchedule.FiscalYearBeginsOn = this.FiscalYearBeginsOn;
            this.GanttSchedule.WeekBeginsOn = this.WeekBeginsOn;
        }

        #region Event Helper Meethods

        /// <summary>
        /// Raises the resource created.
        /// </summary>
        /// <param name="args">The <see cref="ResourceContainerCreatedEventArgs"/> instance containing the event data.</param>
        internal void RaiseResourceCreated(ResourceContainerCreatedEventArgs args)
        {
#if !SILVERLIGHT
            this.RaiseEvent(args);
#else
            if(this.ResourceContainerCreatedEvent!=null)
                this.ResourceContainerCreatedEvent(this,args);
#endif
        }

        /// <summary>
        /// Raises the zoom changed event.
        /// </summary>
        /// <param name="args">The <see cref="Syncfusion.Windows.Controls.Gantt.ZoomChangedEventArgs"/> instance containing the event data.</param>
        internal void RaiseZoomChangedEvent(ZoomChangedEventArgs args)
        {
#if !SILVERLIGHT
            this.RaiseEvent(args);
#else
            if (this.ZoomChangedEvent != null)
                this.ZoomChangedEvent(this, args);
#endif
        }

        /// <summary>
        /// Raises the schedule cell created.
        /// </summary>
        /// <param name="args">The <see cref="Syncfusion.Windows.Controls.Gantt.ScheduleCellCreatedEventArgs"/> instance containing the event data.</param>
        internal void RaiseScheduleCellCreated(ScheduleCellCreatedEventArgs args)
        {
#if !SILVERLIGHT
            this.RaiseEvent(args);
#else
            if (this.ScheduleCellCreatedEvent != null)
                this.ScheduleCellCreatedEvent(this, args);
#endif
        }

        /// <summary>
        /// Raises the strip line created.
        /// </summary>
        /// <param name="args">The <see cref="Syncfusion.Windows.Controls.Gantt.StriplineCreatedEventArgs"/> instance containing the event data.</param>
        internal void RaiseStripLineCreated(StriplineCreatedEventArgs args)
        {
#if !SILVERLIGHT
            this.RaiseEvent(args);
#else
            if (this.StripLineCreatedevent != null)
                this.StripLineCreatedevent(this, args);
#endif
        }

        /// <summary>
        /// Raises the node created.
        /// </summary>
        /// <param name="args">The <see cref="Syncfusion.Windows.Controls.Gantt.NodeCreatedEventArgs"/> instance containing the event data.</param>
        internal void RaiseNodeCreated(NodeCreatedEventArgs args)
        {
#if !SILVERLIGHT
            this.RaiseEvent(args);
#else
            if (this.NodeCreatedEvent != null)
                this.NodeCreatedEvent(this, args);
#endif
        }

        /// <summary>
        /// Raises the node drag completed.
        /// </summary>
        /// <param name="args">The <see cref="Syncfusion.Windows.Controls.Gantt.NodeDragAndDropEventArgs"/> instance containing the event data.</param>
        internal void RaiseNodeDragCompleted(NodeDragAndDropEventArgs args)
        {
#if !SILVERLIGHT
            this.RaiseEvent(args);
#else
            if (this.NodeDragCompletedEvent != null)
                this.NodeDragCompletedEvent(this, args);
#endif
        }

        /// <summary>
        /// Raises the node drag delta.
        /// </summary>
        /// <param name="args">The <see cref="Syncfusion.Windows.Controls.Gantt.NodeDragAndDropEventArgs"/> instance containing the event data.</param>
        internal void RaiseNodeDragDelta(NodeDragAndDropEventArgs args)
        {
#if !SILVERLIGHT
            this.RaiseEvent(args);
#else
            if (this.NodeDragDeltaEvent != null)
                this.NodeDragDeltaEvent(this, args);
#endif
        }

        /// <summary>
        /// Raises the node resizing completed.
        /// </summary>
        /// <param name="args">The <see cref="Syncfusion.Windows.Controls.Gantt.NodeResizingEventArgs"/> instance containing the event data.</param>
        internal void RaiseNodeResizingCompleted(NodeResizingEventArgs args)
        {
#if !SILVERLIGHT
            this.RaiseEvent(args);
#else
            if (this.NodeResizingCompletedEvent != null)
                this.NodeResizingCompletedEvent(this, args);
#endif
        }

        /// <summary>
        /// Raises the node resizing delta.
        /// </summary>
        /// <param name="args">The <see cref="Syncfusion.Windows.Controls.Gantt.NodeResizingEventArgs"/> instance containing the event data.</param>
        internal void RaiseNodeResizingDelta(NodeResizingEventArgs args)
        {
#if !SILVERLIGHT
            this.RaiseEvent(args);
#else
            if (this.NodeResizingDeltaEvent != null)
                this.NodeResizingDeltaEvent(this, args);
#endif
        }

        #endregion


        /// <summary>
        /// Gets the Project Statistics Information
        /// </summary>
        /// <returns>ProjectInfo Object</returns>
        public ProjectInfo GetProjectStatistics()
        {
            if (this.Model == null)
                return null;

            return this.Model.GetProjectStatistics();
        }

        
        /// <summary>
        /// Scrolls the Gantt Chart to the specified Date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        public bool ScrollGanttChartTo(DateTime date)
        {
            if (ScheduleType == Gantt.ScheduleType.CustomNumeric)
                return false;
#if !SILVERLIGHT
            // To ensure the date falls within the range
            if (date.CompareTo(this.StartTime) < 0 || date.CompareTo(this.EndTime) > 0 || GanttChart == null)
            {
                return false;
            }
            isScrollChanged = true;
#else
            // To ensure the date falls within the range
            if (date.CompareTo(this.StartTime) < 0 || date.CompareTo(this.EndTime) > 0 || GanttChart == null || this.ScheduleViewScrollViewer.DesiredSize== new Size(0,0))
            {
                //if the user declares the ScrollGanttChartTo in GanttLoaded event the StartTime is not set in Silverlight.
                //So the above condition will be true. Now we storing the user defined date in variable.
                //In Silverlight internal GanttControl_Loaded event will be hooked after user defined loaded event.
                //When the GanttControl_Loaded event hooked this previous date will be stored to the date parameter. Now the user defined ScrollGanttChartTo will be done.
                previousDate = date;
                isScrollChanged = true;
                return false;
            }

            if (isScrollChanged && previousDate.CompareTo(this.StartTime) > 0 && previousDate.CompareTo(this.EndTime) < 0)
            {
                    //previousValidDate is stored to the parameter only when the user defined date falls within the range.
                    date = previousDate;
                    isScrollChanged = false;
            }
#endif
            this.ScheduleViewScrollViewer.ScrollToHorizontalOffset(this.GanttChart.GetStartPositionOfDate(date));
            return true;
        }

        /// <summary>
        /// Scrolls the Gantt Chart to the specified point.
        /// </summary>
        /// <param name="point">The pixel.</param>
        /// <returns></returns>
        public bool ScrollGanttChartTo(double point)
        {
            //if the schedule type is not CustomNumeric the ScrollGanttChartTo with Point parameter is not applied
            if (ScheduleType != Gantt.ScheduleType.CustomNumeric)
                return false;
#if !SILVERLIGHT
            // To ensure the pixel falls within the range
            if (point.CompareTo(this.StartPoint) < 0 || point.CompareTo(this.EndPoint) > 0 || GanttChart == null)
                return false;
            isScrollChanged = true;
#else
            //To ensure the pixel falls within the range and checking whether the GanttChat is null
            if (point.CompareTo(this.StartPoint) < 0 || point.CompareTo(this.EndPoint) > 0 || GanttChart == null || this.ScheduleViewScrollViewer.DesiredSize==new Size(0,0))
            {
                //In Silverlight the ScrollGanttChartTo is not working in user defined GanttLoaded and Template applied events.
                //So we storing that values is one variable and we reuse that values when the GanttControl_Loaded event fires.
                previousPoint = point;
                isScrollChanged = true;
                return false;
            }
            if (isScrollChanged && previousPoint.CompareTo(this.StartPoint) > 0 && previousPoint.CompareTo(this.EndPoint) < 0)
            {
                //Previous point stored only when the pixel value falls within the range.
                point = previousPoint;
                isScrollChanged = false;
            }

#endif
            this.ScheduleViewScrollViewer.ScrollToHorizontalOffset(this.GanttChart.GetPositionOfPoint(point));

            return true;
        }

        /// <summary>
        /// Sets the items source as null in Gantt Control.
        /// </summary>
        private void ClearProperties()
        {
            this.SelectedItems.Clear();
            this.Model.Dispose();
        }

        /// <summary>
        /// Gets the tooltip template.
        /// </summary>
        /// <returns></returns>
        DataTemplate GenerateTooltipTemplate()
        {
            // Initializing the string builder to build the data template
            StringBuilder templateString = new StringBuilder();

            // Creating the Data template using Xaml tags
            templateString.Append("<DataTemplate ");
            templateString.Append("xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' ");
#if !SILVERLIGHT
            templateString.Append("xmlns:shared='clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Shared.Wpf' ");
#else
            templateString.Append("xmlns:shared='clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Shared.Silverlight' ");
#endif
            templateString.Append("xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' >");
            templateString.Append("<Border>");
            templateString.Append("<Grid Margin='2' >");
            templateString.Append("<Grid.ColumnDefinitions>");
            templateString.Append("<ColumnDefinition Width='0.4*'/>");
            templateString.Append("<ColumnDefinition Width='Auto'/>");
            templateString.Append("</Grid.ColumnDefinitions>");
            templateString.Append("<Grid.RowDefinitions>");
            templateString.Append("<RowDefinition/>");
            templateString.Append("<RowDefinition/>");
            templateString.Append("<RowDefinition/>");
            templateString.Append("<RowDefinition/>");
            templateString.Append("</Grid.RowDefinitions>");

            //// Checking for the existance of the mapping name
            if (!string.IsNullOrEmpty(this.TaskAttributeMapping.TaskNameMapping))
            {
                templateString.Append("<TextBlock Margin='1' Text='Task:' Grid.Column='0' Grid.Row='0' />");
                templateString.Append("<TextBlock Margin='2,1,0,0' Text='{Binding " + this.TaskAttributeMapping.TaskNameMapping +
#if !SILVERLIGHT
 "}' TextTrimming='CharacterEllipsis' Grid.Column='1' Grid.Row='0' MaxWidth='200' MinWidth='100' />");
#else
 "}' TextTrimming='WordEllipsis' Grid.Column='1' Grid.Row='0' MaxWidth='200' MinWidth='100' />");
#endif
            }

            //// Checking for the schedule type to provide exact binding path
            if (this.ScheduleType != ScheduleType.CustomNumeric)
            {
                templateString.Append("<TextBlock Margin='1' Text='Start Date:' Grid.Column='0' Grid.Row='1' />");
                templateString.Append("<TextBlock Margin='2,1,0,0' Text='{Binding " + this.TaskAttributeMapping.StartDateMapping + "}' Grid.Column='1' Grid.Row='1' MaxWidth='200' MinWidth='100' />");
            }
            else
            {
                templateString.Append("<TextBlock Margin='1' Text='Start :' Grid.Column='0' Grid.Row='1' />");
                templateString.Append("<TextBlock Margin='2,1,0,0' Text='{Binding " + this.TaskAttributeMapping.StartPointMapping + "}' Grid.Column='1' Grid.Row='1' MaxWidth='200' MinWidth='100' />");
            }

            //// Checking for the schedule type to provide exact binding path
            if (this.ScheduleType != ScheduleType.CustomNumeric)
            {
                templateString.Append("<TextBlock Margin='1' Text='Finish Date:' Grid.Column='0' Grid.Row='2' />");
                templateString.Append("<TextBlock Margin='2,1,0,0' Text='{Binding " + this.TaskAttributeMapping.FinishDateMapping + "}' Grid.Column='1' Grid.Row='2' MaxWidth='200' MinWidth='100' />");
            }
            else
            {
                templateString.Append("<TextBlock Margin='1' Text='Finish:' Grid.Column='0' Grid.Row='2' />");
                templateString.Append("<TextBlock Margin='2,1,0,0' Text='{Binding " + this.TaskAttributeMapping.FinishPointMapping + "}' Grid.Column='1' Grid.Row='2' MaxWidth='200' MinWidth='100' />");
            }

            //// Checking for the existance of the mapping name
            if (!string.IsNullOrEmpty(this.TaskAttributeMapping.DurationMapping))
            {
                templateString.Append("<TextBlock Margin='1' Text='Duration:' Grid.Column='0' Grid.Row='3' />");
                templateString.Append("<shared:TimeSpanEdit x:Name='Time' Visibility='Collapsed' Value='{Binding " + this.TaskAttributeMapping.DurationMapping + "}' Format=\"d 'days'\" Grid.Column='1' Grid.Row='3'/>");
                templateString.Append("<TextBlock Margin='2,1,0,0' Text='{Binding ElementName=Time, Path=Text}' Grid.Column='1' Grid.Row='3' MaxWidth='200' MinWidth='100' />");
            }

            templateString.Append("</Grid>");
            templateString.Append("</Border>");
            templateString.Append("</DataTemplate>");

#if !SILVERLIGHT
            StringReader st = new StringReader(templateString.ToString());
            return (DataTemplate)XamlReader.Load(XmlReader.Create(st));
#else
            return (DataTemplate)XamlReader.Load(templateString.ToString());
#endif
        }

        #endregion

        #region Table View

        /// <summary>
        /// Invokes the LoadVarianceTableView method in Grid
        /// </summary>
        public void LoadVarianceTableView()
        {
            // Loads Table view only for dateTime Schedule
            if (this.ScheduleType != ScheduleType.CustomNumeric)
                this.GanttGrid.LoadVarianceTableView();
        }

        /// <summary>
        /// Invokes the DefaultTableView method in Grid
        /// </summary>
        public void LoadDefaultTableView()
        {
            // Loads Default View only for DateTime schedule.
            if (this.ScheduleType != ScheduleType.CustomNumeric)
                this.GanttGrid.LoadDefaultTableView();
        }

        #endregion

        #region MSProjectXMLExport/Import
#if !SILVERLIGHT

        private DelegateCommand<object> exportToXMLCommand;
        public DelegateCommand<object> ExportToXMLCommand
        {
            get
            {
                if (exportToXMLCommand == null)
                    exportToXMLCommand = new DelegateCommand<object>(ExportToXMLGantt,canExecute);

                return exportToXMLCommand;
            }
        }

        void ExportToXMLGantt(object parameter)
        {
            this.ExportToXML();
        }


        private DelegateCommand<object> importFromXMLCommand;
        public DelegateCommand<object> ImportFromXMLCommand
        {
            get
            {
                if (importFromXMLCommand == null)
                    importFromXMLCommand = new DelegateCommand<object>(ImportFromXML,canExecute);

                return importFromXMLCommand;
            }
        }

        void ImportFromXML(object parameter)
        {
            this.ImportFromXML();
        }

        bool canExecute(object parameter)
        {
            return true;
        }
#endif

#if SILVERLIGHT

        /// <summary>
        /// Imports from XML.
        /// </summary>
        public bool ImportFromXML()
        {
            try
            {
                ObservableCollection<TaskDetails> importedSource = null;
                importedSource = MSProjectXMLExportImport.ConvertToTaskDetails();

                if (importedSource != null && importedSource.Count > 0)
                {
                    this.TaskAttributeMapping = TaskAttributeMapping.Default;
                    this.Model.InbuiltTaskCollection.Clear();
                    this.Model.InbuiltTaskCollection = importedSource;
                    this.ItemsSource = this.Model.InbuiltTaskCollection;
                    return true;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error on Importing", MessageBoxButton.OK);
                return false;
            }

            return false;
        }

        /// <summary>
        /// Exports to XML.
        /// </summary>
        public bool ExportToXML()
        {
            try
            {
                if(this.Model == null || this.Model.SourceList == null)
                    return false;

                if (this.Model.CheckSourceType())
                {
                    IEnumerable<TaskDetails> collection = this.Model.SourceList.Cast<TaskDetails>();
                    return MSProjectXMLExportImport.ConvertToXML(collection, this.GanttSchedule.StartTime, this.GanttSchedule.EndTime);
                }
                else
                {
                    MessageBox.Show("Only the task of type TaskDetails can be export.", "XML Export", MessageBoxButton.OK);
                    return false;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error on Exporting", MessageBoxButton.OK);
                return false;
            }
        }

#else
        /// <summary>
        /// Exports to XML.
        /// </summary>
        public bool ExportToXML()
        {
            if (this.Model == null)
                return false;

            if (!this.Model.CheckSourceType())
            {
                MessageBox.Show("Only the task of type TaskDetails can be export.", "XML Export", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            try
            {
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.Filter = "XML Files (*.XML)|*.xml";

                if (saveFileDialog.ShowDialog() == true)
                {
                    if (!String.IsNullOrEmpty(saveFileDialog.FileName))
                    {
                        XMLImportExport.ExportToXML(this.Model.SourceList, saveFileDialog.FileName);
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error on Exporting", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return false;
        }

        internal bool IsImportFromXML = false;

        /// <summary>
        /// Exports to XML.
        /// </summary>
        /// <param name="FilePath">The file path.</param>
        public bool ExportToXML(string FilePath)
        {
            if (!string.IsNullOrEmpty(FilePath))
            {
                if (this.Model == null)
                    return false;

                if (!this.Model.CheckSourceType())
                {
                    MessageBox.Show("Only the task of type TaskDetails can be export.", "XML Export", MessageBoxButton.OK, MessageBoxImage.Information);
                    return false;
                }

                try
                {
                    XMLImportExport.ExportToXML(this.Model.SourceList, FilePath);
                    return true;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error on Exporting", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Imports from XML.
        /// </summary>
        public bool ImportFromXML()
        {
            try
            {
                Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
                openFileDialog.Filter = "XML Files (*.XML)|*.xml";
                if (openFileDialog.ShowDialog() == true)
                {
                    return this.ImportFromXML(openFileDialog.FileName);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error on Importing", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return false;
        }

        /// <summary>
        /// Imports from XML.
        /// </summary>
        /// <param name="FilePath">The file path.</param>
        public bool ImportFromXML(string FilePath)
        {
            try
            {
                ObservableCollection<TaskDetails> importedSource = null;
                importedSource = XMLImportExport.ImportFromXML(FilePath);
                this.TaskAttributeMapping = TaskAttributeMapping.Default;
                this.Model.InbuiltTaskCollection.Clear();
                this.Model.InbuiltTaskCollection = importedSource;
                this.ItemsSource = this.Model.InbuiltTaskCollection;
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error on Importing", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

#endif
        #endregion

        #region Override Methods

#if !SILVERLIGHT
        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }
#endif

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            GanttGrid = (GanttGrid)GetTemplateChild("PART_GanttGrid");
            GanttSchedule = (GanttSchedule)GetTemplateChild("PART_GantSchedule");
            GanttChart = (GanttChart)GetTemplateChild("PART_GanttChart");
            
            ScheduleViewScrollViewer = (ScrollViewer)GetTemplateChild("PART_ScheduleViewScrollViewer");

#if SILVERLIGHT
            LayoutGrid = (System.Windows.Controls.Grid)GetTemplateChild("PART_LayoutGrid");
#endif
            if (ScheduleViewScrollViewer != null)
            {
#if !SILVERLIGHT
                ScheduleViewScrollViewer.PreviewMouseWheel += ChartVisualScroll_PreviewMouseWheel;
                ScheduleViewScrollViewer.Loaded += ScheduleViewScrollViewer_Loaded;
#endif
                ScheduleViewScrollViewer.MouseWheel += ChartVisualScroll_MouseWheel;
            }

            if (GanttChart != null && GanttChart.ParentControl == null)
            {
                GanttChart.ParentControl = this;

                if (isToolTipAppliedBeforeOnApplyTemplate)
                    GanttChart.ToolTipTemplate = this.ToolTipTemplate;
            }

            if (GanttSchedule != null && GanttSchedule.ParentControl == null)
            {
                GanttSchedule.ParentControl = this;
                GanttSchedule.Loaded += GanttSchedule_Loaded;
            }

            if (GanttGrid != null && GanttGrid.ParentControl == null)
            {
                GanttGrid.ParentControl = this;
            }

            this.isTemplateApplied = true;
            EnsureProperties();

            // Hooking loaded event
            this.Loaded += GanttControl_Loaded;

            // Raising the template applied event
            if (this.TemplateApplied != null)
                this.TemplateApplied(this, new TemplateAppliedEventArgs());
        }

#if !SILVERLIGHT
        /// <summary>
        /// Handles the Loaded event of the ScheduleViewScrollViewer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void ScheduleViewScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            var SHScrollBar = (ScrollBar)ScheduleViewScrollViewer.Template.FindName("PART_HorizontalScrollBar", ScheduleViewScrollViewer);
            if (SHScrollBar != null)
            {
                SHScrollBar.IsVisibleChanged += SHScrollBar_IsVisibleChanged;
            }
        }

        /// <summary>
        /// Handles the IsVisibleChanged event of the SHScrollBar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        void SHScrollBar_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.GanttGrid.SetDistance();
        }
#endif

        void GanttSchedule_Loaded(object sender, RoutedEventArgs e)
        {
            if (GanttSchedule.baseSource != null && this.isZoomFactorSetBeforeScheduleLoaded)
            {
                this.SetZoomFactor();
                this.isZoomFactorSetBeforeScheduleLoaded = false;
            }
        }

        /// <summary>
        /// Handles the Loaded event of the GanttControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void GanttControl_Loaded(object sender, RoutedEventArgs e)
        {
#if !SILVERLIGHT
            //ScrollGanttChartTo method in GanttControl_loaded will only called when the user didn't called the ScrollGanttChartTo manually.
            if (isScrollChanged)
                return;
#endif
            if (this.ScheduleType != ScheduleType.CustomNumeric)
            {

                if (this.ScheduleType == ScheduleType.YearWithDays || this.ScheduleType == ScheduleType.MonthWithDays || this.ScheduleType == ScheduleType.WeekWithDays)
                    this.ScrollGanttChartTo(this.StartTime.AddDays(7));
#if !SILVERLIGHT
                    else if (this.ScheduleType == ScheduleType.DayWithHours || this.ScheduleType == Gantt.ScheduleType.DayWithMinutes || this.ScheduleType == Gantt.ScheduleType.MonthWithHours)
                        this.ScrollGanttChartTo(this.StartTime.AddDays(1));
#endif
            }
            else
            {
                this.ScrollGanttChartTo(this.StartPoint+2);
            }

        }

        void ChartVisualScroll_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
        }

        void ChartVisualScroll_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
        }

        #endregion
    }
}