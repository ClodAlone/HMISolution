#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Windows.Data;
using System.Collections;
using Syncfusion.Windows.Controls.Gantt;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;
using System.Collections.Specialized;

namespace Syncfusion.Windows.Controls.Gantt.Chart
{
    /// <summary>
    /// Graphical layer that will display the relationship between two tasks as lines.
    /// </summary>
    public class GanttNodeConnector : Control
    {
        #region Properties

        #region Internal/Private properties
        
        private Dictionary<object, object> ChangeListner = new Dictionary<object, object>();
        private Dictionary<object, List<PathFigure>> lineCollection = new Dictionary<object, List<PathFigure>>();
        private Dictionary<object, List<PathFigure>> arrowCollection = new Dictionary<object, List<PathFigure>>();

        internal List<PredecessorExt> PredecessorExts = new List<PredecessorExt>();
        internal GanttChart ParentControl { get; set; }

        #endregion

        /// <summary>
        /// Gets or sets the predecessor.
        /// </summary>
        /// <value>The predecessor.</value>
        public ObservableCollection<Predecessor> Predecessor
        {
            get { return (ObservableCollection<Predecessor>)GetValue(PredecessorProperty); }
            set { SetValue(PredecessorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Predecessor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PredecessorProperty =
            DependencyProperty.Register("Predecessor", typeof(ObservableCollection<Predecessor>), typeof(GanttNodeConnector), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the connector stroke.
        /// </summary>
        /// <value>The connector stroke.</value>
        public Brush ConnectorStroke
        {
            get { return (Brush)GetValue(ConnectorStrokeProperty); }
            set { SetValue(ConnectorStrokeProperty, value); }
        }

#if !SILVERLIGHT
        // Using a DependencyProperty as the backing store for ConnectorStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConnectorStrokeProperty =
            DependencyProperty.Register("ConnectorStroke", typeof(Brush), typeof(GanttNodeConnector), new PropertyMetadata(Brushes.Black));
#else
        // Using a DependencyProperty as the backing store for ConnectorStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConnectorStrokeProperty =
            DependencyProperty.Register("ConnectorStroke", typeof(Brush), typeof(GanttNodeConnector), new PropertyMetadata(new SolidColorBrush(Colors.Black)));
#endif
        /// <summary>
        /// Gets or sets the connector geometry.
        /// </summary>
        /// <value>The connector geometry.</value>
        public PathGeometry ConnectorGeometry
        {
            get { return (PathGeometry)GetValue(ConnectorGeometryProperty); }
            set { SetValue(ConnectorGeometryProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Geometry.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConnectorGeometryProperty =
            DependencyProperty.Register("ConnectorGeometry", typeof(PathGeometry), typeof(GanttNodeConnector), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the arrow geometry.
        /// </summary>
        /// <value>The arrow geometry.</value>
        public PathGeometry ArrowGeometry
        {
            get { return (PathGeometry)GetValue(ArrowGeometryProperty); }
            set { SetValue(ArrowGeometryProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ArrowGeometry.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ArrowGeometryProperty =
            DependencyProperty.Register("ArrowGeometry", typeof(PathGeometry), typeof(GanttNodeConnector), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the start index.
        /// </summary>
        /// <value>The start index.</value>
        public int StartIndex
        {
            get { return (int)GetValue(StartIndexProperty); }
            set { SetValue(StartIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartIndexProperty =
            DependencyProperty.Register("StartIndex", typeof(int), typeof(GanttNodeConnector), new PropertyMetadata(0));

        /// <summary>
        /// Gets or sets the end index.
        /// </summary>
        /// <value>The end index.</value>
        public int EndIndex
        {
            get { return (int)GetValue(EndIndexProperty); }
            set { SetValue(EndIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EndIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndIndexProperty =
            DependencyProperty.Register("EndIndex", typeof(int), typeof(GanttNodeConnector), new PropertyMetadata(0));

        #endregion

        #region  Constructor & overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="GanttNodeConnector"/> class.
        /// </summary>
        public GanttNodeConnector()
        {
            DefaultStyleKey = typeof(GanttNodeConnector);

            ConnectorGeometry = new PathGeometry();
            ConnectorGeometry.Figures = new PathFigureCollection();

            ArrowGeometry = new PathGeometry();
            ArrowGeometry.Figures = new PathFigureCollection();

            ConnectorGeometry.FillRule = FillRule.Nonzero;
            ArrowGeometry.FillRule = FillRule.Nonzero;  

            this.Predecessor = new ObservableCollection<Predecessor>();
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        #endregion

        #region Helper methods

        #region Methods to listen souce collection change

        /// <summary>
        /// Updates the predecessor info of overall Gantt tasks.
        /// </summary>
        internal void UpdatePredecessorInfo()
        {
            // To ensure the existence of dependency relationship
            if (string.IsNullOrEmpty(this.ParentControl.Model.TaskAttributeMapping.PredecessorMapping) || string.IsNullOrEmpty(this.ParentControl.Model.TaskAttributeMapping.TaskIdMapping))
                return;

            // To ensure the correct predecessor mapping name
            if (this.ParentControl.Model.ItemProperties != null && (this.ParentControl.Model.ItemProperties[this.ParentControl.Model.TaskAttributeMapping.PredecessorMapping] == null ||
                this.ParentControl.Model.ItemProperties[this.ParentControl.Model.TaskAttributeMapping.TaskIdMapping] == null))
                return;

            // Clearing the sate maintaining properties
            this.ClearGeometry();
            this.ChangeListner.Clear();
            this.PredecessorExts.Clear();

            // Creating predecessorExt for internal use, this will helps to draw all the connector in a single panel
            var result = this.ParentControl.Model.ExpandedCollection.Where((item) =>
            {
                return CreatePredecessorExt((item as GanttRecord).DataItem);
            });

            // This line is much important, this will invoke the abouve linq cuery and generate the PredecessorExt
            int cout = result != null ? result.Count() : 0;

            // Wiring collection changed to listen and update the dynamic objects
            ParentControl.Model.ExpandedCollection.CollectionChanged -= ExpandedCollection_CollectionChanged;
            ParentControl.Model.ExpandedCollection.CollectionChanged += ExpandedCollection_CollectionChanged;
        }

        /// <summary>
        /// Handles the CollectionChanged event of the ExpandedCollection control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        void ExpandedCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // To ensure the existence of child && the collection changed is happening on expand collapse state
            if (!this.ParentControl.Model.TaskAttributeMapping.HasChildMapping || this.ParentControl.Model.IsInExpSync)
                return;

            // To ensure the existence of dependency relationship
            if (string.IsNullOrEmpty(this.ParentControl.Model.TaskAttributeMapping.PredecessorMapping) || string.IsNullOrEmpty(this.ParentControl.Model.TaskAttributeMapping.TaskIdMapping))
                return;

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (GanttRecord record in e.NewItems)
                {
                    // Adding the new predecessors to existing predecessor collection
                    this.CreatePredecessorExt(record.DataItem);
                }
            }
        }

        /// <summary>
        /// Collections the changed.
        /// </summary>
        /// <param name="changeObjects">The obj.</param>
        /// <param name="ChangeAction">The change action.</param>
        internal void CollectionChanged(object changeObjects, NotifyCollectionChangedAction ChangeAction)
        {
            if (ChangeAction == NotifyCollectionChangedAction.Remove)
            {
                foreach (object obj in (IEnumerable)changeObjects)
                {
                    // Fetching the exact list of predecessor to be removed
                    var predecessors = this.PredecessorExts.Where(pre => pre.Item == obj);
                    if (predecessors == null)
                        continue;

                    // Unwiring the predecessor collection change listener
                    this.UnWireCollectionChanged((IEnumerable)predecessors, obj);

                    // Removing the predecessors of removed object
                    while (predecessors.ToList().Count > 0)
                    {
                        this.PredecessorExts.Remove(predecessors.ToList()[0]);
                    }
                }
            }
            else if (ChangeAction == NotifyCollectionChangedAction.Reset)
            {
                this.ClearGeometry();
            }
        }

        /// <summary>
        /// Creates the predecessor ext.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        private bool CreatePredecessorExt(object item)
        {
            if (this.ParentControl.Model.ItemProperties[this.ParentControl.Model.TaskAttributeMapping.PredecessorMapping] == null)
                return false;

            var predecessors = this.ParentControl.Model.ItemProperties[this.ParentControl.Model.TaskAttributeMapping.PredecessorMapping].GetValue(item);

            if (predecessors != null)
            {
                // To listen the change in predecessor collection of underlying source
                this.WireCollectionChanged((IEnumerable)predecessors, item);

                foreach (Predecessor pre in (predecessors as IEnumerable<Predecessor>))
                {
                    GeneratePedecessorExt(item, pre);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Generates the pedecessor ext.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="pre">The pre.</param>
        private void GeneratePedecessorExt(object item, Predecessor pre)
        {
            // Fetching the dependent item from over all task list
            GanttRecord depItem = this.ParentControl.Model.ExpandedCollection.Where(expItem =>
                  (int)this.ParentControl.Model.ItemProperties[this.ParentControl.Model.TaskAttributeMapping.TaskIdMapping].GetValue(expItem.DataItem) == pre.GanttTaskIndex).FirstOrDefault();

            object dependent = depItem != null ? depItem.DataItem : null;

            // To eliminate the self dependent predecessors
            if (dependent != null && item.Equals(dependent))
                return;

            // Adding the newly created PredecessorExt to the collection
            PredecessorExts.Add(new PredecessorExt { Item = item, Predecessor = pre, DependentItem = dependent });
        }

        /// <summary>
        /// Updates the predecessor ext.
        /// </summary>
        /// <param name="preExt">The pre ext.</param>
        /// <returns></returns>
        private bool UpdatePredecessorExt(PredecessorExt preExt)
        {
            string idMappingName = this.ParentControl.Model.TaskAttributeMapping.TaskIdMapping;

            // To ensure the existance of Task Id
            if (string.IsNullOrEmpty(idMappingName))
                return false;

            // Fetching the dependent item from over all task list

            GanttRecord depItem = this.ParentControl.Model.ExpandedCollection.Where(expItem =>
                  (int)this.ParentControl.Model.ItemProperties[idMappingName].GetValue(expItem.DataItem) == preExt.Predecessor.GanttTaskIndex).FirstOrDefault();

            preExt.DependentItem = depItem != null ? depItem.DataItem : null;

            return false;
        }

        #endregion

        #region Methods to listen dynamic Predecessor

        /// <summary>
        /// Wires the collection changed.
        /// </summary>
        /// <param name="SourceList">The source list.</param>
        /// <param name="parent">The parent.</param>
        private void WireCollectionChanged(IEnumerable SourceList, object parent)
        {
            if (SourceList is INotifyCollectionChanged)
            {
                var sourceList = SourceList as INotifyCollectionChanged;

                if (!this.ChangeListner.Keys.Contains(sourceList))
                {
                    sourceList.CollectionChanged += Predecessor_CollectionChanged;
                    this.ChangeListner.Add(sourceList, parent);
                }
            }
        }

        /// <summary>
        /// Uns the wire collection changed.
        /// </summary>
        /// <param name="SourceList">The source list.</param>
        /// <param name="parent">The parent.</param>
        private void UnWireCollectionChanged(IEnumerable SourceList, object parent)
        {
            if (SourceList is INotifyCollectionChanged)
            {
                var sourceList = SourceList as INotifyCollectionChanged;

                sourceList.CollectionChanged -= Predecessor_CollectionChanged;
                this.ChangeListner.Remove(sourceList);
            }
        }

        /// <summary>
        /// Handles the CollectionChanged event of the Predecessor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        void Predecessor_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            object parent = null;
            if (!this.ChangeListner.TryGetValue(sender, out parent))
                return;

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Predecessor pre in e.NewItems)
                    this.GeneratePedecessorExt(parent, pre);

                // To redraw the connectors of the current record
                ClearAndRedDrawConnectors(parent, false);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Predecessor pre in e.OldItems)
                {
                    var oldPredecessors = PredecessorExts.Where(p => p.Item == parent && p.Predecessor == pre);
                    if (oldPredecessors == null || oldPredecessors.Count() <= 0)
                        continue;

                    // To remove the predecessorExt of the removed predecessor
                    while (oldPredecessors.ToList().Count > 0)
                    {
                        this.PredecessorExts.Remove(oldPredecessors.ToList()[0]);
                    }
                }
                // To redraw the connectors of the current record
                ClearAndRedDrawConnectors(parent, false);
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                var oldPredecessors = PredecessorExts.Where(p => p.Item == parent);
                if (oldPredecessors == null || oldPredecessors.Count() <= 0)
                    return;
                // To redraw the connectors of the current record
                ClearAndRedDrawConnectors(parent, false);
                while (oldPredecessors.ToList().Count > 0)
                {
                    this.PredecessorExts.Remove(oldPredecessors.ToList()[0]);
                }              
            }
        }

        #endregion

        #region Refresh connectors

        /// <summary>
        /// Refreshes the connectors.
        /// </summary>
        internal void RefreshConnectors()
        {
            if (this.PredecessorExts == null || this.PredecessorExts.Count <= 0)
                return;

            var nullDependentPreExts =  this.PredecessorExts.Where(pre => pre.DependentItem == null);

            // To update the PredecessorExt which are all having the DependentItem as null.
            if (nullDependentPreExts != null && nullDependentPreExts.Count() > 0)
            {
                foreach (PredecessorExt preExt in nullDependentPreExts)
                {
                   this.UpdatePredecessorExt(preExt);
                }
            }
            if (this.StartIndex >= 0 && this.EndIndex >= 0)
            {
                ClearGeometry();
                // Draw the connector for the node that are in current view
                for (int i = StartIndex; i <= EndIndex; i++)
                {
                   UpdateConnector(this.ParentControl.Model.ExpandedCollection[i].DataItem);
                }
            }
        }

        /// <summary>
        /// Clears the geometry.
        /// </summary>
        internal void ClearGeometry()
        {
#if !SILVERLIGHT
            this.ConnectorGeometry.Clear();
            this.ArrowGeometry.Clear();
            this.lineCollection.Clear();
            this.arrowCollection.Clear();
#else
            this.ConnectorGeometry.Figures.Clear();
            this.ArrowGeometry.Figures.Clear();
            this.lineCollection.Clear();
            this.arrowCollection.Clear();
#endif
        }

        #endregion

        #region Repainting connectors

        /// <summary>
        /// Repaints the selected node connector.
        /// </summary>
        /// <param name="obj">The obj.</param>
        internal void RepaintResizedNodeConnector(object obj)
        {
            ClearAndRedDrawConnectors(obj,true);
        }

        /// <summary>
        /// Clears the and red draw connectors.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="isOnResize">if set to <c>true</c> [is on resize].</param>
        private void ClearAndRedDrawConnectors(object obj, bool isOnResize)
        {
            if (this.PredecessorExts == null || this.PredecessorExts.Count <= 0)
                return;

            List<PathFigure> linePaths;
            List<PathFigure> arrowPaths;

            // Repainting header connectors
            if (this.lineCollection.TryGetValue(obj, out linePaths))
            {
                this.RemoveExistingPaths(this.ConnectorGeometry, obj, linePaths);

                if (this.arrowCollection.TryGetValue(obj, out arrowPaths))
                    this.RemoveExistingPaths(this.ArrowGeometry, obj, arrowPaths);

                this.UpdateConnector(obj);
            }
            // This is to enforce the update when the refresh happens due to change in predecessor collection
            else if (!isOnResize)
                this.UpdateConnector(obj);

            var tails = GetTalis(obj);
            if (tails == null)
                return;

            // To repaint the tail connectors
            foreach (PredecessorExt preExt in tails)
            {
                if (!this.lineCollection.TryGetValue(preExt.Item, out linePaths))
                    continue;

                this.RemoveExistingPaths(this.ConnectorGeometry, preExt.Item, linePaths);

                if (this.arrowCollection.TryGetValue(preExt.Item, out arrowPaths))
                    this.RemoveExistingPaths(this.ArrowGeometry, preExt.Item, arrowPaths);

                this.UpdateConnector(preExt.Item);
            }
        }

        /// <summary>
        /// Removes the existing paths.
        /// </summary>
        /// <param name="pathGeomety">The path geomety.</param>
        /// <param name="obj">The obj.</param>
        /// <param name="pathFigures">The path figures.</param>
        private void RemoveExistingPaths(PathGeometry pathGeomety, object obj, List<PathFigure> pathFigures)
        {
            foreach (PathFigure path in pathFigures)
            {
                if (pathGeomety.Figures.Contains(path))
                    pathGeomety.Figures.Remove(path);
            }
        }

        #endregion

        #region Update connector of particular node

        /// <summary>
        /// Updates the connector.
        /// </summary>
        /// <param name="currentItem">The current item.</param>
        private void UpdateConnector(object currentItem)
        {
            if (currentItem != null)
            {
                GanttRecord currentRecord = this.ParentControl.Model.ExpandedCollection.RecordFromItem(currentItem);

                if (this.lineCollection.Keys.Contains(currentItem))
                    this.lineCollection[currentItem] = new List<PathFigure>();

                if (this.arrowCollection.Keys.Contains(currentItem))
                    this.arrowCollection[currentItem] = new List<PathFigure>();

                // Getting current node from the gantt chart rows. 
                GanttNode currentNode = (this.ParentControl.ItemContainerGenerator.ContainerFromItem(currentRecord) as GanttChartRow).FindElementOfType<GanttNode>();

                if (currentNode == null)
                    return;

                // Tails - The nodes which depedents on the currnet item, in other words noeds that are containing current item its predecessor.
                // Headers - The depednt nodes of current item, in other words predecessors of current item.

                // Getting the tail items from the predecessor exts collection.
                var tails = this.GetTalis(currentItem);

                // Fetching the predecessors(Header) of current item
                var tempHeaders = this.ParentControl.Model.ItemProperties[this.ParentControl.Model.TaskAttributeMapping.PredecessorMapping].GetValue(currentItem);

                IEnumerable<Predecessor> headers = tempHeaders as IEnumerable<Predecessor>;

                if (headers != null)
                {
                    foreach (Predecessor pre in headers)
                    {
                        // This code was initially used when we take the head with the help of GanttTaskIndex now its not necessary.
                        //if ((pre.GanttTaskIndex - 1) >= this.ParentControl.Model.ExpandedCollection.Count)
                        //    continue;

                        var expResult = this.PredecessorExts.Where(preExt => preExt.Predecessor == pre);

                        if (expResult == null || expResult.Count() < 1)
                            continue;

                        var head = expResult.FirstOrDefault().DependentItem;

                        // Step followed on developing this, this will helps to modify in future.
                        //var head = this.ParentControl.Model.ExpandedCollection.Where(obj =>
                        //    (int)this.ParentControl.Model.ItemProperties[this.ParentControl.Model.TaskAttributeMapping.TaskIdMapping].GetValue(obj) == pre.GanttTaskIndex).FirstOrDefault();
                        //var head = PredecessorExts.Where(p => p.Predecessor == pre).FirstOrDefault();
                        //var head = this.ParentControl.Model.ExpandedCollection[pre.GanttTaskIndex-1];

                        int index = this.ParentControl.Model.ExpandedCollection.IndexOf(head);

                        if (head == null || index < 0)
                            continue;

                        CheckForPathFigureExistance(currentItem);

                        PathFigure newLine = new PathFigure();
                        this.lineCollection[currentItem].Add(newLine);

                        PathFigure newArrow = new PathFigure();
                        this.arrowCollection[currentItem].Add(newArrow);

                        this.DrawConnector(head, currentNode, pre.GanttTaskRelationship, true, newLine, newArrow);
                    }
                }

                if (tails != null)
                {
                    foreach (PredecessorExt preExt in tails)
                    {
                        int taskIndex = this.ParentControl.Model.ExpandedCollection.IndexOf(preExt.Item);
                        if (taskIndex > -1 && (taskIndex < this.StartIndex || taskIndex > this.EndIndex))
                        {
                            var relation = preExt.Predecessor.GanttTaskRelationship;

                            // The relation ship has been changed to achieve the reverse drawing with this same drawing method
                            if (relation == GanttTaskRelationship.FinishToStart)
                                relation = GanttTaskRelationship.StartToFinish;
                            else if (relation == GanttTaskRelationship.StartToFinish)
                                relation = GanttTaskRelationship.FinishToStart;

                            CheckForPathFigureExistance(currentItem);

                            PathFigure newLine = new PathFigure();
                            this.lineCollection[currentItem].Add(newLine);

                            PathFigure newArrow = new PathFigure();
                            this.arrowCollection[currentItem].Add(newArrow);

                            this.DrawConnector(preExt.Item, currentNode, relation, false, newLine, newArrow);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks for path figure existance.
        /// </summary>
        /// <param name="currentItem">The current item.</param>
        private void CheckForPathFigureExistance(object currentItem)
        {
            if (!this.lineCollection.Keys.Contains(currentItem))
                this.lineCollection.Add(currentItem, new List<PathFigure>());

            if (!this.arrowCollection.Keys.Contains(currentItem))
                this.arrowCollection.Add(currentItem, new List<PathFigure>());
        }

        /// <summary>
        /// Gets the talis.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        private IEnumerable<PredecessorExt> GetTalis(object obj)
        {
            // Fetching the tail items of current item (obj passed as arug) from the predecessor exts collection.
            var tails = this.PredecessorExts.Where((pre) =>
            {
                int id = (int)this.ParentControl.Model.ItemProperties[this.ParentControl.Model.TaskAttributeMapping.TaskIdMapping].GetValue(obj);

                if (id == pre.Predecessor.GanttTaskIndex)
                {
                    return true;
                }
                return false;
            });
            return tails;
        }

        #endregion

        #region Draw connector & arrows

        /// <summary>
        /// Draws the connector.
        /// </summary>
        /// <param name="headItem">The head item.</param>
        /// <param name="tailNode">The tail node.</param>
        /// <param name="relation">The relation.</param>
        /// <param name="IsHeader">if set to <c>true</c> [is header].</param>
        /// <param name="line">The line.</param>
        /// <param name="arrow">The arrow.</param>
        private void DrawConnector(object headItem, GanttNode tailNode, GanttTaskRelationship relation, bool IsHeader, PathFigure line, PathFigure arrow)
        {
            if (tailNode == null || headItem == null || line == null)
                return;

            GanttRecord headRecord = this.ParentControl.Model.ExpandedCollection.RecordFromItem(headItem);
            GanttRecord tailRecord = this.ParentControl.Model.ExpandedCollection.RecordFromItem(tailNode.DataContext);
                      
            if (headRecord == null || tailRecord == null)
                return;

            bool isheadRecordHeader = this.GetItemType(headRecord);
            bool istailRecordHeader = this.GetItemType(tailRecord);

            double moveHeadHeaderPosition = isheadRecordHeader ? 5 : 0d;
            double moveTailHeaderPosition = istailRecordHeader ? 5 : 0d;

            double moveTailPosition = 0d;
            double moveHeadPosition = 0d;

            if (tailNode.StartTime == tailNode.EndTime)
            {
                moveTailPosition = 10;
            }
            List<Point> pts = new List<Point>();

            if (relation == GanttTaskRelationship.FinishToStart)
            {
                Point start = new Point();

                start.X = this.ParentControl.GetStartPositionOfDate(tailNode.StartTime) - (moveTailPosition == 0 ? moveTailHeaderPosition : moveTailPosition);
                start.Y = tailNode.TransformToVisual(ParentControl).Transform(new Point(0, 0)).Y + tailNode.ActualHeight / 2;

                var container = this.ParentControl.ItemContainerGenerator.ContainerFromItem(headRecord);
                GanttNode headerNode = (GanttNode)(container as GanttChartRow).FindElementOfType<GanttNode>();

                Point end = new Point();

                if (container != null && headerNode != null)
                {
                    if (headerNode.StartTime == headerNode.EndTime)
                    {
                        moveHeadPosition = 8; 
                    }

                    end.X = this.ParentControl.GetEndPositionOfDate(headerNode.EndTime) + (moveHeadPosition == 0 ? moveHeadHeaderPosition :moveHeadPosition );
                    end.Y = headerNode.TransformToVisual(ParentControl).Transform(new Point(0, 0)).Y + headerNode.ActualHeight / 2;
                }
                else
                {
                    DateTime temp = (DateTime)this.ParentControl.Model.ItemProperties[this.ParentControl.Model.TaskAttributeMapping.FinishDateMapping].GetValue(headItem);

                    end.X = this.ParentControl.GetEndPositionOfDate(temp);

                    int startIndex = this.ParentControl.Model.ExpandedCollection.IndexOf(tailRecord);
                    int endIndex = this.ParentControl.Model.ExpandedCollection.IndexOf(headRecord);

                    if (startIndex < endIndex)
                        end.Y = this.ParentControl.ActualHeight + 15;
                    else
                        end.Y = -15;
                }

                if (end.X < start.X && (start.X - end.X) > 20)
                {
                    pts.Add(new Point(end.X + 10, start.Y));
                    pts.Add(new Point(end.X + 10, end.Y));
                }
                else
                {
                    pts.Add(new Point(start.X - 10, start.Y));
                    if (start.Y > end.Y)
                    {
                        pts.Add(new Point(start.X - 10, end.Y + 12));
                        pts.Add(new Point(end.X + 10, end.Y + 12));
                    }
                    else
                    {
                        pts.Add(new Point(start.X - 10, end.Y - 12));
                        pts.Add(new Point(end.X + 10, end.Y - 12));
                    }
                    pts.Add(new Point(end.X + 10, end.Y));
                }
                pts.Add(end);

                PolyLineSegment poly = new PolyLineSegment();
                foreach (Point pt in pts)
                {
                    poly.Points.Add(pt);
                }
                line.StartPoint = start;
                line.Segments.Clear();
                line.Segments.Add(poly);

                this.ConnectorGeometry.Figures.Add(line);

                DrawArrow(arrow, IsHeader ? start : end, 0d);
            }
            else if (relation == GanttTaskRelationship.StartToStart)
            {
                Point start = new Point();

                start.X = this.ParentControl.GetStartPositionOfDate(tailNode.StartTime) - (moveTailPosition == 0 ? moveTailHeaderPosition : moveTailPosition);
                start.Y = tailNode.TransformToVisual(ParentControl).Transform(new Point(0, 0)).Y + tailNode.ActualHeight / 2;

                var container = this.ParentControl.ItemContainerGenerator.ContainerFromItem(headRecord);
                GanttNode headerNode = (GanttNode)(container as GanttChartRow).FindElementOfType<GanttNode>();
                Point end = new Point();

                if (container != null && headerNode != null)
                {
                    if (headerNode.StartTime == headerNode.EndTime)
                    {
                        moveHeadPosition = 8;
                    }
                    end.X = this.ParentControl.GetStartPositionOfDate(headerNode.StartTime) - (moveHeadPosition == 0 ? moveHeadHeaderPosition : moveHeadPosition);
                    end.Y = headerNode.TransformToVisual(ParentControl).Transform(new Point(0, 0)).Y + headerNode.ActualHeight / 2;
                }
                else
                {
                    DateTime temp = (DateTime)this.ParentControl.Model.ItemProperties[this.ParentControl.Model.TaskAttributeMapping.StartDateMapping].GetValue(headItem);

                    end.X = this.ParentControl.GetStartPositionOfDate(temp);

                    int startIndex = this.ParentControl.Model.ExpandedCollection.IndexOf(tailRecord);
                    int endIndex = this.ParentControl.Model.ExpandedCollection.IndexOf(headRecord);

                    if (startIndex < endIndex)
                        end.Y = this.ParentControl.ActualHeight + 15;
                    else
                        end.Y = -5;
                }

                Point leftMost = new Point(Math.Min(start.X, end.X) - 10, start.Y);
                pts.Add(leftMost);
                pts.Add(new Point(leftMost.X, end.Y));
                pts.Add(end);

                PolyLineSegment poly = new PolyLineSegment();
                foreach (Point pt in pts)
                {
                    poly.Points.Add(pt);
                }
                line.StartPoint = start;
                line.Segments.Clear();
                line.Segments.Add(poly);

                this.ConnectorGeometry.Figures.Add(line);
                DrawArrow(arrow, IsHeader ? start : end, 0d);
            }
            else if (relation == GanttTaskRelationship.FinishToFinish)
            {
                Point start = new Point();

                start.X = this.ParentControl.GetEndPositionOfDate(tailNode.EndTime) + (moveTailPosition == 0 ? moveTailHeaderPosition : moveTailPosition);
                start.Y = tailNode.TransformToVisual(ParentControl).Transform(new Point(0, 0)).Y + tailNode.ActualHeight / 2;

                var container = this.ParentControl.ItemContainerGenerator.ContainerFromItem(headRecord);
                GanttNode headerNode = (GanttNode)(container as GanttChartRow).FindElementOfType<GanttNode>();
                Point end = new Point();

                if (container != null && headerNode != null)
                {
                    if (headerNode.StartTime == headerNode.EndTime)
                    {
                        moveHeadPosition = 8;
                    }

                    end.X = this.ParentControl.GetEndPositionOfDate(headerNode.EndTime) + (moveHeadPosition == 0 ? moveHeadHeaderPosition : moveHeadPosition);
                    end.Y = headerNode.TransformToVisual(ParentControl).Transform(new Point(0, 0)).Y + headerNode.ActualHeight / 2;
                }
                else
                {
                    DateTime endTime = (DateTime)this.ParentControl.Model.ItemProperties[this.ParentControl.Model.TaskAttributeMapping.FinishDateMapping].GetValue(headItem);

                    end.X = this.ParentControl.GetEndPositionOfDate(endTime);

                    int startIndex = this.ParentControl.Model.ExpandedCollection.IndexOf(tailRecord);
                    int endIndex = this.ParentControl.Model.ExpandedCollection.IndexOf(headRecord);

                    if (startIndex < endIndex)
                        end.Y = this.ParentControl.ActualHeight + 15;
                    else
                        end.Y = -5;
                }

                Point rightMost = new Point(Math.Max(start.X, end.X) + 10, start.Y);
                pts.Add(rightMost);
                pts.Add(new Point(rightMost.X, end.Y));
                pts.Add(end);
                PolyLineSegment poly = new PolyLineSegment();
                foreach (Point pt in pts)
                {
                    poly.Points.Add(pt);
                }
                line.StartPoint = start;
                line.Segments.Clear();
                line.Segments.Add(poly);

                this.ConnectorGeometry.Figures.Add(line);
                DrawArrow(arrow, IsHeader ? start : end, 180d);
            }
            else if (relation == GanttTaskRelationship.StartToFinish)
            {
                Point start = new Point();

                start.X = this.ParentControl.GetEndPositionOfDate(tailNode.EndTime) + (moveTailPosition == 0 ? (moveTailHeaderPosition) : moveTailPosition);
                start.Y = tailNode.TransformToVisual(ParentControl).Transform(new Point(0, 0)).Y + tailNode.ActualHeight / 2;

                var container = this.ParentControl.ItemContainerGenerator.ContainerFromItem(headRecord);
                GanttNode headerNode = (GanttNode)(container as GanttChartRow).FindElementOfType<GanttNode>();
                Point end = new Point();

                if (container != null && headerNode != null)
                {
                    if (headerNode.StartTime == headerNode.EndTime)
                    {
                        moveHeadPosition = 8;
                    }
                    end.X = this.ParentControl.GetStartPositionOfDate(headerNode.StartTime) - (moveHeadPosition == 0 ? moveHeadHeaderPosition : moveHeadPosition);
                    end.Y = headerNode.TransformToVisual(ParentControl).Transform(new Point(0, 0)).Y + headerNode.ActualHeight / 2;
                }
                else
                {
                    DateTime temp = (DateTime)this.ParentControl.Model.ItemProperties[this.ParentControl.Model.TaskAttributeMapping.StartDateMapping].GetValue(headItem);

                    end.X = this.ParentControl.GetStartPositionOfDate(temp);

                    int startIndex = this.ParentControl.Model.ExpandedCollection.IndexOf(tailRecord);
                    int endIndex = this.ParentControl.Model.ExpandedCollection.IndexOf(headRecord);

                    if (startIndex < endIndex)
                        end.Y = this.ParentControl.ActualHeight + 15;
                    else
                        end.Y = -15;
                }

                if (start.X < end.X && (end.X - start.X) > 20)
                {
                    pts.Add(new Point(start.X + 10, start.Y));
                    pts.Add(new Point(start.X + 10, end.Y));
                }
                else
                {
                    pts.Add(new Point(start.X + 10, start.Y));
                    if (start.Y > end.Y)
                    {
                        pts.Add(new Point(start.X + 10, start.Y - 12));
                        pts.Add(new Point(end.X - 10, start.Y - 12));
                    }
                    else
                    {
                        pts.Add(new Point(start.X + 10, start.Y + 12));
                        pts.Add(new Point(end.X - 10, start.Y + 12));
                    }
                    pts.Add(new Point(end.X - 10, end.Y));
                }
                pts.Add(end);

                PolyLineSegment poly = new PolyLineSegment();
                foreach (Point pt in pts)
                {
                    poly.Points.Add(pt);
                }
                line.StartPoint = start;
                line.Segments.Clear();
                line.Segments.Add(poly);

                this.ConnectorGeometry.Figures.Add(line);
                DrawArrow(arrow, IsHeader ? start : end, 180d);
            }
        }

        /// <summary>
        /// Draws the arrow.
        /// </summary>
        /// <param name="arrow">The arrow.</param>
        /// <param name="end">The end.</param>
        /// <param name="angle">The angle.</param>
        private void DrawArrow(PathFigure arrow, Point end, double angle)
        {
            const double _Width = 5;
            const double _Height = 8;

            List<Point> pts = new List<Point>();
            if (angle == 0)
            {
                pts.Add(new Point(end.X - _Width, end.Y - _Height / 2));
                pts.Add(new Point(end.X - _Width, end.Y + _Height / 2));
            }
            else if (angle == 90)
            {
                pts.Add(new Point(end.X - _Height / 2, end.Y - _Width));
                pts.Add(new Point(end.X + _Height / 2, end.Y - _Width));
            }
            else if (angle == 180)
            {
                pts.Add(new Point(end.X + _Width, end.Y - _Height / 2));
                pts.Add(new Point(end.X + _Width, end.Y + _Height / 2));
            }
            else if (angle == 270)
            {
                pts.Add(new Point(end.X - _Height / 2, end.Y + _Width));
                pts.Add(new Point(end.X + _Height / 2, end.Y + _Width));
            }
            PolyLineSegment poly = new PolyLineSegment();
            foreach (Point pt in pts)
            {
                poly.Points.Add(pt);
            }
            arrow.StartPoint = end;
            this.ArrowGeometry.FillRule = FillRule.Nonzero;
            arrow.IsClosed = true;
            arrow.Segments.Clear();
            arrow.Segments.Add(poly);

            this.ArrowGeometry.Figures.Add(arrow);
        }

        /// <summary>
        /// Gets the type of the node item.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <returns></returns>
        private bool GetItemType(GanttRecord record)
        {
            if (this.ParentControl == null || this.ParentControl.ParentControl == null  || this.ParentControl.ParentControl.TaskAttributeMapping == null || string.IsNullOrEmpty(this.ParentControl.ParentControl.TaskAttributeMapping.ChildMapping))
                return false;
            try
            {
                var Children = this.ParentControl.Model.ItemProperties[this.ParentControl.ParentControl.TaskAttributeMapping.ChildMapping].GetValue(record.DataItem);

                return (Children != null && this.ParentControl.Model.GetObservableCollection(Children as IEnumerable).Count > 0);
            }
            catch (Exception)
            {
                throw new Exception("Child Mapping name is missing or mapping name is wrong.");
            }
        }

        #endregion

        #endregion
    }

    #region supporting class

    /// <summary>
    /// Internal to maintain the predecessor info
    /// </summary>
    class PredecessorExt
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PredecessorExt"/> class.
        /// </summary>
        public PredecessorExt()
        {

        }

        /// <summary>
        /// Gets or sets the predecessor.
        /// </summary>
        /// <value>The predecessor.</value>
       public  Predecessor Predecessor
        {
            get;
            set;
        }
       /// <summary>
       /// Gets or sets the item.
       /// </summary>
       /// <value>The item.</value>
        public object Item
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the dependent item.
        /// </summary>
        /// <value>The dependent item.</value>
        public object DependentItem
        {
            get;
            set;
        }
    }
    #endregion
}
