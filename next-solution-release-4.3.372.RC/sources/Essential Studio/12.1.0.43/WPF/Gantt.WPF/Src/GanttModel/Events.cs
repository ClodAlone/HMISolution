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
using Syncfusion.Windows.Controls.Gantt.Schedule;
using System.Windows;
using Syncfusion.Windows.Controls.Gantt.Chart;
using System.Windows.Controls;

namespace Syncfusion.Windows.Controls.Gantt
{
    #region ScheduleCellCreated Event

    /// <summary>
    /// Delegate for Schedule Cell Created Event Handler
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The <see cref="Syncfusion.Windows.Controls.Gantt.ScheduleCellCreatedEventArgs"/> instance containing the event data.</param>
    public delegate void ScheduleCellCreatedEventHandler(object sender, ScheduleCellCreatedEventArgs args);

    /// <summary>
    /// Class that is used as Event args for Schedule Cell Created Event.
    /// </summary>
    public class ScheduleCellCreatedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Gets or sets the current cell.
        /// </summary>
        /// <value>The current cell.</value>
        public GanttScheduleCell CurrentCell { get; set; }
       
    }

    #endregion

    #region StripLineCreated event

    /// <summary>
    /// Delegate for Stripline created Event Handler
    /// </summary>
    public delegate void StriplineCreatedEventHandler(object sender, StriplineCreatedEventArgs args);

    /// <summary>
    /// Class which is used as Event args for Stripline Created Event
    /// </summary>
    public class StriplineCreatedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Gets or sets the current stripline.
        /// </summary>
        /// <value>The current stripline.</value>
        public StripLine CurrentStripline { get; set; }

#if SILVERLIGHT

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="StriplineCreatedEventArgs"/> is handled.
        /// </summary>
        /// <value><c>true</c> if handled; otherwise, <c>false</c>.</value>
        public bool Handled { get; set; }

#endif
    }

    #endregion

    #region Zoom Changed event

    /// <summary>
    /// Delegate for zoom changed event handler
    /// </summary>
    public delegate void ZoomChangedEventHandler(object sender, ZoomChangedEventArgs args);

    /// <summary>
    /// Class that represent the zoom chagned event args, and holds zoom info about Gantt Schedule
    /// </summary>
    public class ZoomChangedEventArgs : RoutedEventArgs
    {
        private double _baseMinLength = 20;
        private double _baseMaxLength = 100;

        /// <summary>
        /// Gets or sets the schedule header info.
        /// </summary>
        /// <value>The schedule header info.</value>
        public IList<GanttScheduleRowInfo> ScheduleHeaderInfo { get; set; }

        /// <summary>
        /// Gets or sets the length of the base cellt min.
        /// </summary>
        /// <value>The length of the base cellt min.</value>
        public double BaseCelltMinLength
        {
            get { return _baseMinLength; }
            internal set { _baseMinLength = value; }
        }

        /// <summary>
        /// Gets or sets the length of the base cellt max.
        /// </summary>
        /// <value>The length of the base cellt max.</value>
        public double BaseCelltMaxLength
        {
            get { return _baseMaxLength; }
            internal set { _baseMaxLength = value; }
        }

        /// <summary>
        /// Gets or sets the zoom factor.
        /// </summary>
        /// <value>The zoom factor.</value>
        public double ZoomFactor { get; internal set; }

#if SILVERLIGHT

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ZoomChangedEventArgs"/> is handled.
        /// </summary>
        /// <value><c>true</c> if handled; otherwise, <c>false</c>.</value>
        public bool Handled { get; set; }

#endif
    }

    #endregion

    #region  Template Applied event

    /// <summary>
    /// Delegate for Template applied event handler
    /// </summary>
    public delegate void TemplateAppliedEventHandler(object sender, TemplateAppliedEventArgs args);

    /// <summary>
    /// Event args that holds the information about the source that triggred the event
    /// </summary>
    public class TemplateAppliedEventArgs : EventArgs
    {
        public TemplateAppliedEventArgs() {}
    }
    
    #endregion

    #region Resource Container Created Event

    /// <summary>
    /// Delegate for Resource Container Created Event
    /// </summary>
    public delegate void ResourceContainerCreatedEventHandler(object sender, ResourceContainerCreatedEventArgs args);

    /// <summary>
    /// Class whis is used as the Argument for Resource Container Created event
    /// </summary>
    public class ResourceContainerCreatedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Gets or sets the current resource.
        /// </summary>
        /// <value>The current resource.</value>
        public ContentControl CurrentResource { get; set; }
    }

    #endregion

    #region Node Drag And Drop Events

    /// <summary>
    /// Delegate for Node Drag Delta and Node Drag Completed events
    /// </summary>
    public delegate void NodeDragAndDropEventHandler(object sender, NodeDragAndDropEventArgs args);

    /// <summary>
    /// Class which is used as the argument for Node Drag Delta and Node Drag Completed events
    /// </summary>
    public class NodeDragAndDropEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Gets or sets the node.
        /// </summary>
        /// <value>The node.</value>
        public GanttNode Node { get; set; }

        /// <summary>
        /// Gets or sets the horizontal change.
        /// </summary>
        /// <value>The horizontal change.</value>
        public double HorizontalChange { get; set; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        /// <value>The start time.</value>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the end time.
        /// </summary>
        /// <value>The end time.</value>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Gets or sets the start.
        /// </summary>
        /// <value>The start.</value>
        public double Start { get; set; }

        /// <summary>
        /// Gets or sets the end.
        /// </summary>
        /// <value>The end.</value>
        public double End { get; set; }

        /// <summary>
        /// Gets or sets the node parent.
        /// </summary>
        /// <value>The node parent.</value>
        public GanttChartRowItemsPresenter NodePresenter { get; internal set; }
#if SILVERLIGHT

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="StriplineCreatedEventArgs"/> is handled.
        /// </summary>
        /// <value><c>true</c> if handled; otherwise, <c>false</c>.</value>
        public bool Handled { get; set; }

#endif
    }

    #endregion

    #region Node Resize Events

    /// <summary>
    /// Delegate for Node Resizing Delta and Node Resizing Completed Events
    /// </summary>
    public delegate void NodeResizingEventHandeler(object sender,NodeResizingEventArgs args);

    /// <summary>
    /// Class which is used as the argument for Node Resizing Delta and Node Resizing Completed Events
    /// </summary>
    public class NodeResizingEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Gets or sets the node.
        /// </summary>
        /// <value>The node.</value>
        public GanttNode Node { get; set; }

        /// <summary>
        /// Gets or sets the node parent.
        /// </summary>
        /// <value>The node parent.</value>
        public GanttChartRowItemsPresenter NodePresenter { get; internal set; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        /// <value>The start time.</value>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the end time.
        /// </summary>
        /// <value>The end time.</value>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Gets or sets the start.
        /// </summary>
        /// <value>The start.</value>
        public double Start { get; set; }

        /// <summary>
        /// Gets or sets the end.
        /// </summary>
        /// <value>The end.</value>
        public double End { get; set; }

        /// <summary>
        /// Gets or sets the resizing direction.
        /// </summary>
        /// <value>The resizing direction.</value>
        public Directions ResizingDirection { get; set; }
#if SILVERLIGHT

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="StriplineCreatedEventArgs"/> is handled.
        /// </summary>
        /// <value><c>true</c> if handled; otherwise, <c>false</c>.</value>
        public bool Handled { get; set; }

#endif
    }

    #endregion

    #region Node Created Event

    /// <summary>
    /// Delegate for Node Created Event
    /// </summary>
    public delegate void NodeCreatedEventHandler(object sender,NodeCreatedEventArgs args);

    /// <summary>
    /// Class Which is used as the Argument for Node Created Event
    /// </summary>
    public class NodeCreatedEventArgs :RoutedEventArgs
    {
        /// <summary>
        /// Gets or sets the node.
        /// </summary>
        /// <value>The node.</value>
        public GanttNode Node { get; set; }

        /// <summary>
        /// Gets or sets the current task.
        /// </summary>
        /// <value>The current task.</value>
        public object CurrrentDataItem { get; set; }
    }
    #endregion
}
