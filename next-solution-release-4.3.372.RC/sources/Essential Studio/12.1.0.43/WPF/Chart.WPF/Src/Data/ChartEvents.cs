// <copyright file="ChartEvents.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using System.ComponentModel;

    #region Delegates
    /// <summary>
    /// Represents Chart mouse actions event handler.
    /// </summary>
    /// <param name="sender">The sender</param>
    /// <param name="e">The ChartMouseEventArgs</param>
    /// <seealso cref="ChartMouseEventHandler"/>
    public delegate void ChartMouseEventHandler(object sender, ChartMouseEventArgs e);

    /// <summary>
    /// Represents Chart axis range event handler.
    /// </summary>
    /// <param name="sender">The sender</param>
    /// <param name="e">The ChartMouseEventArgs</param>
    public delegate void ChartAxisRangeEventHandler(object sender, ChartAxisRangeArgs e);

    /// <summary>
    /// Represents Chart ToolBar event handler.
    /// </summary>
    /// <param name="sender">The sender</param>
    /// <param name="e">The ChartMouseEventArgs</param>
    public delegate void ChartToolBarEventHandler(object sender, ChartToolBarArgs e);


    /// <summary>
    /// Delegate declaration for ChartPropertyWindowEventHandler
    /// </summary>
    /// <param name="e"></param>
    public delegate void ChartPropertyWindowEventHandler(ChartPropertyWindowEventArgs e);
    /// <summary>
    /// Delegate declaration for ChartPropertyWindowCancelEventHandler
    /// </summary>
    /// <param name="e"></param>
    public delegate void ChartPropertyWindowCancelEventHandler(ChartPropertyWindowCancelEventArgs e);

   

    /// <summary>
    /// Represents Chart Segment event handler.
    /// </summary>
    /// <param name="sender">The sender</param>
    /// <param name="e">The SegmentDragEventArgs</param>
    public delegate void ChartSegmentDragEventHandler(object sender, SegmentDragEventArgs e);

    /// <summary>
    /// Delegate declaration for TypeChangingEventHandeler
    /// </summary>
    /// <param name="d"></param>
    /// <param name="e"></param>
    public delegate void TypeChangingEventHandeler(DependencyObject d, TypeChangingEventArgs e);

    /// <summary>
    /// Represents Chart Segment event handler.
    /// </summary>
    /// <param name="sender">The sender</param>
    /// <param name="e">The SegmentDropEventArgs</param>
    public delegate void ChartSegmentDropEventHandler(object sender, SegmentDropEventArgs e);


    /// <summary>
    /// Represents Chart Annotation event handler.
    /// </summary>
    /// <param name="sender">The sender</param>
    /// <param name="args">The ChartAnnotationDragEventArgs</param>
    public delegate void ChartAnnotationDragEventHandler(object sender, ChartAnnotationDragEventArgs args);
    /// <summary>
    /// Represents Chart Annotation event handler.
    /// </summary>
    /// <param name="sender">The sender</param>
    /// <param name="args">The ChartAnnotationDropEventArgs</param>
    public delegate void ChartAnnotationDropEventHandler(object sender, ChartAnnotationDropEventArgs args);
    
    /// <summary>
    /// Represents for QtpChartScrollPosition EventHandler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void QtpChartScrollPositionEventHandler( object sender, ChartScrollEventArgs e);

    /// <summary>
    /// Represents QtpChartZoomed EventHandler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void QtpChartZoomedEventHandler( object sender, ChartZoomedEventArgs e);

    /// <summary>
    /// Represents QtpChartZoomedOut EventHandler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void QtpChartZoomedOutEventHandler(object sender, ChartZoomedOutEventArgs e);

    /// <summary>
    /// Represents QtpChartPanning EventHandler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void QtpChartPanningEventHandler(object sender, ChartPanningEventArgs e);

    /// <summary>
    /// Represents QtpChartZoomedReset EventHandler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void QtpChartZoomResetEventHandler(object sender, ChartZoomReseteventArgs e);

    /// <summary>
    /// Represents for QtpChartZoomSector EventHandler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void QtpChartZoomSectorEventHandler(object sender, ChartZoomSectorEventArgs e);

    /// <summary>
    /// Represents for QtpLegendLocationChanged EventHandler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void QtpLegendLocationChangedEventHandler(object sender, LegendLocationChangedeventArgs e);

    /// <summary>
    /// Represents QtpInteracitveCursorLocationChanged EventHandler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void QtpInteracitveCursorLocationChangedEventHandler(object sender, CursorLocationChangedeventArgs e);
    #endregion

    #region EventArgs
    /// <summary>
    /// Represents chart axis range event arguments.
    /// </summary>
    /// <remarks>
    /// Both Primary and Secondary Axis comes with Rangechanged event. This event occurs
    /// when the Range of the axis is changed. We could get the old and new range from
    /// the RangeChanged event.
    /// </remarks>
    /// <example>
    /// <code language="C#">
    ///  Chart1.Areas[0].PrimaryAxis.RangeChanged +=new
    /// ChartAxisRangeEventHandler(PrimaryAxis_RangeChanged);
    /// void PrimaryAxis_RangeChanged(object sender, ChartAxisRangeArgs e)
    ///  {
    ///       Console.WriteLine (e.NewValue.ToString();
    ///  }
    /// </code>
    /// </example>
    /// <seealso cref="ChartAxisRangeArgs"/>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    public class ChartAxisRangeArgs
    {
        #region Members

        /// <summary>
        /// Initializes DoubleRange m_oldValue
        /// </summary>
        private DoubleRange m_oldValue=new DoubleRange(0,0);

        /// <summary>
        /// Initializes DoubleRange m_newValue
        /// </summary>
        private DoubleRange m_newValue;

        /// <summary>
        /// Initializes ChartAxis m_axis
        /// </summary>
        private ChartAxis m_axis;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartAxisRangeArgs"/> class.
        /// </summary>
        /// <param name="axis">The axis value.</param>
        /// <param name="newValue">The new value.</param>
        public ChartAxisRangeArgs(ChartAxis axis, DoubleRange newValue)
        {
            m_axis = axis;
            //m_oldValue = oldValue;
            m_newValue = newValue;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the old range value.
        /// </summary>
        /// <value>The range old value.</value>
        public DoubleRange OldValue
        {
            get
            {
                return m_oldValue;
            }
        }

        /// <summary>
        /// Gets the range new value.
        /// </summary>
        /// <value>The new range value.</value>
        public DoubleRange NewValue
        {
            get
            {
                return m_newValue;
            }
        }
        #endregion
    }

        #region ChartToolBarArgs
    /// <summary>
    /// Represents Chart ToolBar event arguments.
    /// </summary>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    public class ChartToolBarArgs
    {
        #region Members
        /// <summary>
        /// Initializes m_oldValue
        /// </summary>
        private ToolBarItem m_oldValue;

        /// <summary>
        /// Initializes m_newValue
        /// </summary>
        private ToolBarItem m_newValue;

        /// <summary>
        /// Initializes m_toolBar
        /// </summary>
        private ChartToolBar m_toolBar;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartToolBarArgs"/> class.
        /// </summary>
        /// <param name="chartToolBar">The ChartToolBar.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        internal ChartToolBarArgs(ChartToolBar chartToolBar, ToolBarItem oldValue, ToolBarItem newValue)
        {
            m_toolBar = chartToolBar;
            m_oldValue = oldValue;
            m_newValue = newValue;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the old ToolBarItem value.
        /// </summary>
        /// <value>The ToolBarItem old value.</value>
        public ToolBarItem OldValue
        {
            get
            {
                return m_oldValue;
            }
        }

        /// <summary>
        /// Gets the ToolBarItem new value.
        /// </summary>
        /// <value>The new ToolBarItem value.</value>
        public ToolBarItem NewValue
        {
            get
            {
                return m_newValue;
            }
        }
        #endregion
    }
    #endregion

        /// <summary>
        /// Class implementation for ChartPropertyWindowEventArgs
        /// </summary>
        public class ChartPropertyWindowEventArgs : EventArgs
        {
            /// <summary>
            /// Called when instance created for ChartPropertyWindowEventArgs
            /// </summary>
            /// <param name="_chartPropertyWindow"></param>
            public ChartPropertyWindowEventArgs(PropertyWindow _chartPropertyWindow)
            {
                this.PropertyWindow = (PropertyWindow)_chartPropertyWindow;
            }

            /// <summary>
            /// Get and Set PropertyWindowProperty
            /// </summary>
            public PropertyWindow PropertyWindow
            {
                get;
                private set;
            }
        }

        /// <summary>
        /// Class implementation for ChartPropertyWindowCancelEventArgs
        /// </summary>
        public class ChartPropertyWindowCancelEventArgs : CancelEventArgs
        {
            /// <summary>
            /// Called when instance created for ChartPropertyWindowCancelEventArgs
            /// </summary>
            /// <param name="_chartPropertyWindow"></param>
            public ChartPropertyWindowCancelEventArgs(PropertyWindow _chartPropertyWindow)
            {
                this.PropertyWindow = (PropertyWindow)_chartPropertyWindow;
            }

            /// <summary>
            /// Get and Set PropertyWindow Property
            /// </summary>
            public PropertyWindow PropertyWindow
            {
                get;
                private set;
            }
        }


    /// <summary>
    /// Represents chart mouse click event arguments.
    /// </summary>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    public class ChartMouseEventArgs
    {
        #region Members
        /// <summary>
        /// Initializes m_segment
        /// </summary>
        private ChartSegment m_segment;

        /// <summary>
        /// Initializes m_mouseArgs
        /// </summary>
        private MouseEventArgs m_mouseArgs;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartMouseEventArgs"/> class.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        /// <param name="segment">The segment.</param>
        /// <seealso cref="ChartMouseEventArgs"/>
        public ChartMouseEventArgs(MouseEventArgs args, ChartSegment segment)
        {
            m_mouseArgs = args;
            m_segment = segment;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the segment.
        /// </summary>
        /// <value>The segment.</value>
        public ChartSegment Segment
        {
            get
            {
                return m_segment;
            }
        }

        /// <summary>
        /// Gets the mouse event args.
        /// </summary>
        /// <value>The mouse event args.</value>
        public MouseEventArgs MouseEventArgs
        {
            get
            {
                return m_mouseArgs;
            }
        }
        #endregion
    }

        /// <summary>
        /// Class implementation for TypeChangingEventArgs
        /// </summary>
        public class TypeChangingEventArgs
        {
            /// <summary>
            /// Get and Set OldValue property
            /// </summary>
            public object OldValue { get; set; }
            /// <summary>
            /// Get and Set NewValue property
            /// </summary>
            public object NewValue { get; set; }
            /// <summary>
            /// Contructor implementation for TypeChangingEventArgs
            /// </summary>
            /// <param name="oldValue"></param>
            /// <param name="newValue"></param>
            public TypeChangingEventArgs(object oldValue,object newValue)
            {
                OldValue = oldValue;
                NewValue = newValue;
            }
        }

    /// <summary>
    /// Represents ChartAxis changed event arguments class.
    /// </summary>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    public class AxisChangedEventArgs : EventArgs
    {
        #region Members
        /// <summary>
        /// Initializes m_propertyChangedArgs
        /// </summary>
        private DependencyPropertyChangedEventArgs m_propertyChangedArgs;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="AxisChangedEventArgs"/> class.
        /// </summary>
        public AxisChangedEventArgs()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AxisChangedEventArgs"/> class.
        /// </summary>
        /// <param name="dependencyPropertyChangedEventArgs">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public AxisChangedEventArgs(DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            m_propertyChangedArgs = dependencyPropertyChangedEventArgs;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the dependency property event args.
        /// </summary>
        /// <value>The dependency property event args.</value>
        public DependencyPropertyChangedEventArgs DependencyPropertyEventArgs
        {
            get
            {
                return m_propertyChangedArgs;
            }
        }
        #endregion
    }

        /// <summary>
        /// Class implementation for ChartAnnotationDragEventArgs
        /// </summary>
        public class ChartAnnotationDragEventArgs : EventArgs
        {
            /// <summary>
            /// Constructor for ChartAnnotationDragEventArgs
            /// </summary>
            /// <param name="annotLabel"></param>
            public ChartAnnotationDragEventArgs(ChartAnnotationLabel annotLabel)
            {
                this.label = annotLabel;
            }
            /// <summary>
            /// Get and Set label property
            /// </summary>
            public ChartAnnotationLabel label
            {
                get;
                private set;
            }
        }

        /// <summary>
        /// Class implementation for ChartAnnotationDropEventArgs
        /// </summary>
        public class ChartAnnotationDropEventArgs : EventArgs
        {
            /// <summary>
            /// Constructor for ChartAnnotationDropEventArgs
            /// </summary>
            /// <param name="annotLabel"></param>
            public ChartAnnotationDropEventArgs(ChartAnnotationLabel annotLabel)
            {
                this.label = annotLabel;
            }
            /// <summary>
            /// Get and Set label property
            /// </summary>
            public ChartAnnotationLabel label
            {
                get;
                private set;
            }
        }

    /// <summary>
        /// Class implementation for SegmentDragEventArgs
    /// </summary>
    public class SegmentDragEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor implementation for SegmentDragEventArgs
        /// </summary>
        /// <param name="argSegment"></param>
        public SegmentDragEventArgs(ChartSegment argSegment)
        {
            this.segment = (ChartSegment)argSegment;
        }

        /// <summary>
        /// Get or Set segment property
        /// </summary>
        public ChartSegment segment
        {
            get;
            private set;
        }
    }


    /// <summary>
    /// Class implementation for SegmentDropEventArgs
    /// </summary>
    public class SegmentDropEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Constructor implementaion for SegmentDropEventArgs
        /// </summary>
        /// <param name="oldSegment"></param>
        public SegmentDropEventArgs(ChartSegment oldSegment)
        {
            this.OldSegment = (ChartSegment)oldSegment;
        }

        /// <summary>
        /// Get and Set OldSegment property
        /// </summary>
        public ChartSegment OldSegment
        {
            get;
            private set;
        }
    }

    /// <summary>
    /// Class implementation for ChartScrollEventArgs
    /// </summary>
    public class ChartScrollEventArgs: RoutedEventArgs
    {
        /// <summary>
        /// Construtor for ChartScrollEventArgs
        /// </summary>
        /// <param name="area"></param>
        /// <param name="axis"></param>
        public ChartScrollEventArgs(ChartArea area, ChartAxis axis)
        {
            this.chartarea = (ChartArea) area;
            this.chartaxis = (ChartAxis) axis;
        }

        /// <summary>
        /// Get and Set Chartarea property
        /// </summary>
        public ChartArea chartarea
        {
            get; 
            private set;
        }
        
        /// <summary>
        /// get and Set chartaxis property
        /// </summary>
        public ChartAxis chartaxis
        {
            get;
            private set;
        }
    }

    /// <summary>
    /// Class implementation for ChartZoomedEventArgs
    /// </summary>
    public class ChartZoomedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// constructor for ChartZoomedEventArgs
        /// </summary>
        /// <param name="area"></param>
        public ChartZoomedEventArgs(ChartArea area)
        {
            this.chartarea = (ChartArea)area;           
        }

        /// <summary>
        /// Get and Set chartarea property
        /// </summary>
        public ChartArea chartarea
        {
            get;
            private set;
        }

       
    }

    /// <summary>
    /// Class implementation for ChartZoomedOutEventArgs
    /// </summary>
    public class ChartZoomedOutEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Constructor for ChartZoomedOutEventArgs
        /// </summary>
        /// <param name="area"></param>
        public ChartZoomedOutEventArgs(ChartArea area)
        {
            this.chartarea = (ChartArea)area;           
        }

        /// <summary>
        /// Get and Set chartarea property.
        /// </summary>
        public ChartArea chartarea
        {
            get;
            private set;
        }

       
    }
    
    /// <summary>
    /// Class implementation for ChartPanningEventArgs
    /// </summary>
    public class ChartPanningEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Called when instance created for ChartPanningEventArgs
        /// </summary>
        /// <param name="area"></param>
        public ChartPanningEventArgs(ChartArea area)
        {
            this.chartarea = (ChartArea)area;
        }

        /// <summary>
        /// Get and Set chartarea property
        /// </summary>
        public ChartArea chartarea
        {
            get;
            private set;
        }


    }


    /// <summary>
    /// Class implementation for ChartZoomReseteventArgs
    /// </summary>
    public class ChartZoomReseteventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Called when instance created for ChartZoomReseteventArgs
        /// </summary>
        /// <param name="area"></param>
        public ChartZoomReseteventArgs(ChartArea area)
        {
            this.chartarea = (ChartArea)area;
        }

        /// <summary>
        /// Get and Set chartarea property
        /// </summary>
        public ChartArea chartarea
        {
            get;
            private set;
        }


    }

    /// <summary>
    /// Class implementation for ChartZoomSectorEventArgs
    /// </summary>
    public class ChartZoomSectorEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Called when instance created for ChartZoomSectorEventArgs
        /// </summary>
        /// <param name="area"></param>
        /// <param name="sectorargs"></param>
        public ChartZoomSectorEventArgs(ChartArea area,Rect sectorargs)
        {
            this.chartarea = (ChartArea)area;
            this.SectorArgs = (Rect)sectorargs;
        }

        /// <summary>
        /// Gets or Sets the chartarea property
        /// </summary>
        public ChartArea chartarea
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or Sets the SectorArgs property
        /// </summary>
        public Rect SectorArgs
        { get; private set; }


    }
    /// <summary>
    /// RoutedEvent Class implementation for LegendLocationChangedeventArgs
    /// </summary>
    public class LegendLocationChangedeventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Get or Set 
        /// </summary>
        /// <param name="legend"></param>
        public LegendLocationChangedeventArgs(ChartLegend legend)
        {            
            this.legend = (ChartLegend)legend;
        }
        
        /// <summary>
        /// Get or Set legend property 
        /// </summary>
        public ChartLegend legend
        {
            get;
            private set;
        }

    }


    /// <summary>
    /// Class implementation for CursorLocationChangedeventArgs
    /// </summary>
    public class CursorLocationChangedeventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Called when instance created for CursorLocationChangedeventArgs
        /// </summary>
        /// <param name="area"></param>
        /// <param name="cursor"></param>
        /// <param name="axis"></param>
        public CursorLocationChangedeventArgs(ChartArea area, InteractiveCursor cursor, ChartAxis axis)
        {

            this.area = (ChartArea)area;
            this.cursor = (InteractiveCursor)cursor;
            this.axis = (ChartAxis)axis;
        }
        
        /// <summary>
        /// Gets or Sets area property 
        /// </summary>
        public ChartArea area
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or Sets the cursorProperty
        /// </summary>
        public InteractiveCursor cursor
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the  axis property
        /// </summary>
        public ChartAxis axis
        {
            get;
            set;
        }
    }
    #endregion

   

}
