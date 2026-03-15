#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Event sink for diagram view.
    /// </summary>
    public class ViewerEventSink
        : EventSink
    {
        #region Class EventKeys
        private static readonly EventKey m_keyOriginChanged;
        private static readonly EventKey m_keyMagnificationChanged;
        private static readonly EventKey m_keyScrollVirtualBoundsChanged;
        private static readonly EventKey m_keyNodeSelected;
        private static readonly EventKey m_keyNodeDeselected;
        private static readonly EventKey m_keyNodeClick;
        private static readonly EventKey m_keyNodeDoubleClick;
        private static readonly EventKey m_keyNodeMouseLeave;
        private static readonly EventKey m_keyNodeMouseEnter;
        private static readonly EventKey m_keySelectionChanged;
        private static readonly EventKey m_keySelectionChanging;
        private static readonly EventKey m_keyNodeCollectionChanging;
        private static readonly EventKey m_keyNodeCollectionChanged;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes static members of the <see cref="ViewerEventSink"/> class.
        /// </summary>
        static ViewerEventSink()
        {
            m_keyOriginChanged = new EventKey();
            m_keyMagnificationChanged = new EventKey();
            m_keyScrollVirtualBoundsChanged = new EventKey();

            m_keyNodeSelected = new EventKey();
            m_keyNodeDeselected = new EventKey();
            m_keyNodeClick = new EventKey();
            m_keyNodeDoubleClick = new EventKey();
            m_keyNodeMouseLeave = new EventKey();
            m_keyNodeMouseEnter = new EventKey();
            m_keySelectionChanged = new EventKey();
            m_keySelectionChanging = new EventKey();
            m_keyNodeCollectionChanging = new EventKey();
            m_keyNodeCollectionChanged = new EventKey();
        }
        #endregion

        #region Class events
        /// <summary>
        /// Occurs when node collection changing.
        /// </summary>
        public override event CollectionExEventHandler NodeCollectionChanging
        {
            add { AddHandler(m_keyNodeCollectionChanging, value); }
            remove { RemoveHandler(m_keyNodeCollectionChanging, value); }
        }

        /// <summary>
        /// Occurs when node collection changed.
        /// </summary>
        public override event CollectionExEventHandler NodeCollectionChanged
        {
            add { AddHandler(m_keyNodeCollectionChanged, value); }
            remove { RemoveHandler(m_keyNodeCollectionChanged, value); }
        }

        /// <summary>
        /// Occurs when view origin changed.
        /// </summary>
        public event ViewOriginEventHandler OriginChanged
        {
            add { AddHandler(m_keyOriginChanged, value); }
            remove { RemoveHandler(m_keyOriginChanged, value); }
        }

        /// <summary>
        /// Occurs when view magnification changed.
        /// </summary>
        public event ViewMagnificationEventHandler MagnificationChanged
        {
            add { AddHandler(m_keyMagnificationChanged, value); }
            remove { RemoveHandler(m_keyMagnificationChanged, value); }
        }

        /// <summary>
        /// Occurs when scroll virtual bounds changed.
        /// </summary>
        public event ViewScrollVirtualBoundsEventHandler ScrollVirtualBoundsChanged
        {
            add { AddHandler(m_keyScrollVirtualBoundsChanged, value); }
            remove { RemoveHandler(m_keyScrollVirtualBoundsChanged, value); }
        }

        /// <summary>
        /// Occurs when view node selected.
        /// </summary>
        public event NodeSelectedEventHandler NodeSelected
        {
            add { AddHandler(m_keyNodeSelected, value); }
            remove { RemoveHandler(m_keyNodeSelected, value); }
        }

        /// <summary>
        /// Occurs when view node deselected.
        /// </summary>
        public event NodeSelectedEventHandler NodeDeselected
        {
            add { AddHandler(m_keyNodeDeselected, value); }
            remove { RemoveHandler(m_keyNodeDeselected, value); }
        }

        /// <summary>
        /// Occurs when click on node.
        /// </summary>
        public event NodeMouseEventHandler NodeClick
        {
            add { AddHandler(m_keyNodeClick, value); }
            remove { RemoveHandler(m_keyNodeClick, value); }
        }

        /// <summary>
        /// Occurs when double click on node.
        /// </summary>
        public event NodeMouseEventHandler NodeDoubleClick
        {
            add { AddHandler(m_keyNodeDoubleClick, value); }
            remove { RemoveHandler(m_keyNodeDoubleClick, value); }
        }

        /// <summary>
        /// Occurs when mouse enter inside node.
        /// </summary>
        public event NodeMouseEventHandler NodeMouseEnter
        {
            add { AddHandler(m_keyNodeMouseEnter, value); }
            remove { RemoveHandler(m_keyNodeMouseEnter, value); }
        }

        /// <summary>
        /// Occurs when mouse leave node.
        /// </summary>
        public event NodeMouseEventHandler NodeMouseLeave
        {
            add { AddHandler(m_keyNodeMouseLeave, value); }
            remove { RemoveHandler(m_keyNodeMouseLeave, value); }
        }

        /// <summary>
        /// Occurs when selection list changing.
        /// </summary>
        public event CollectionExEventHandler SelectionListChanging
        {
            add { AddHandler(m_keySelectionChanging, value); }
            remove { RemoveHandler(m_keySelectionChanging, value); }
        }

        /// <summary>
        /// Occurs when selection list changed.
        /// </summary>
        public event CollectionExEventHandler SelectionListChanged
        {
            add { AddHandler(m_keySelectionChanged, value); }
            remove { RemoveHandler(m_keySelectionChanged, value); }
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Raises the origin changed.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.ViewOriginEventArgs"/> instance containing the event data.</param>
        public void RaiseOriginChanged(ViewOriginEventArgs evtArgs)
        {
            // execute high priority handlers anyway
            ViewOriginEventHandler handler = this.HighPriorityHandlers[m_keyOriginChanged] as ViewOriginEventHandler;

            if (handler != null)
            {
                handler(evtArgs);
            }

            if (CanRaiseEvent())
            {
                // Raise NodesChanged event
                handler = this.Handlers[m_keyOriginChanged] as ViewOriginEventHandler;

                if (handler != null)
                {
                    handler(evtArgs);
                }
            }
        }

        /// <summary>
        /// Raises the node collection changing event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.CollectionExEventArgs"/> instance containing the event data.</param>
        /// <returns>true, if raise nodes changing event.</returns>
        public override bool RaiseNodesChangingEvent(CollectionExEventArgs evtArgs)
        {
            CollectionExEventHandler handler;

            // execute high priority handlers anyway
            handler = this.HighPriorityHandlers[m_keyNodeCollectionChanging] as CollectionExEventHandler;

            if (handler != null)
            {
                handler(evtArgs);
            }

            // if service status is started -- execute generic handlers
            if (this.ServiceStatus == Diagram.ServiceStatus.Started || this.ServiceStatus == Diagram.ServiceStatus.Resumed)
            {
                // Raise node collection changing event
                handler = this.Handlers[m_keyNodeCollectionChanging] as CollectionExEventHandler;

                if (handler != null)
                {
                    handler(evtArgs);
                }
            }

            return !evtArgs.Cancel;
        }

        /// <summary>
        /// Raises the node collection changed event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.CollectionExEventArgs"/> instance containing the event data.</param>
        public override void RaiseNodesChangedEvent(CollectionExEventArgs evtArgs)
        {
            CollectionExEventHandler handler;

            // execute high priority handlers anyway
            handler = this.HighPriorityHandlers[m_keyNodeCollectionChanged] as CollectionExEventHandler;

            if (handler != null)
            {
                handler(evtArgs);
            }

            if (this.ServiceStatus == Diagram.ServiceStatus.Started || this.ServiceStatus == Diagram.ServiceStatus.Resumed)
            {
                // Raise node collection changed event
                handler = this.Handlers[m_keyNodeCollectionChanged] as CollectionExEventHandler;

                if (handler != null)
                {
                    handler(evtArgs);
                }
            }
        }

        /// <summary>
        /// Raises the magnification changed.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.ViewMagnificationEventArgs"/> instance containing the event data.</param>
        public void RaiseMagnificationChanged(ViewMagnificationEventArgs evtArgs)
        {
            // execute high priority handlers anyway
            ViewMagnificationEventHandler handler = this.HighPriorityHandlers[m_keyMagnificationChanged] as ViewMagnificationEventHandler;

            if (handler != null)
            {
                handler(evtArgs);
            }

            if (CanRaiseEvent())
            {
                // Raise NodesChanged event
                handler = this.Handlers[m_keyMagnificationChanged] as ViewMagnificationEventHandler;

                if (handler != null)
                {
                    handler(evtArgs);
                }
            }
        }

        /// <summary>
        /// Raises the scroll virtual bounds changed.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.ViewScrollVirtualBoundsEventArgs"/> instance containing the event data.</param>
        public void RaiseScrollVirtualBoundsChanged(ViewScrollVirtualBoundsEventArgs evtArgs)
        {
            // execute high priority handlers anyway
            ViewScrollVirtualBoundsEventHandler handler = this.HighPriorityHandlers[m_keyScrollVirtualBoundsChanged] as ViewScrollVirtualBoundsEventHandler;

            if (handler != null)
            {
                handler(evtArgs);
            }

            if (CanRaiseEvent())
            {
                // Raise Bounds Changed event
                handler = this.Handlers[m_keyScrollVirtualBoundsChanged] as ViewScrollVirtualBoundsEventHandler;

                if (handler != null)
                {
                    handler(evtArgs);
                }
            }
        }

        /// <summary>
        /// Raises the node selected.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.NodeSelectedEventArgs"/> instance containing the event data.</param>
        public void RaiseNodeSelected(NodeSelectedEventArgs evtArgs)
        {
            // execute high priority handlers anyway
            NodeSelectedEventHandler handler = this.HighPriorityHandlers[m_keyNodeSelected] as NodeSelectedEventHandler;

            if (handler != null)
            {
                handler(evtArgs);
            }

            if (CanRaiseEvent())
            {
                handler = this.Handlers[m_keyNodeSelected] as NodeSelectedEventHandler;

                if (handler != null)
                {
                    handler(evtArgs);
                }
            }
        }

        /// <summary>
        /// Raises the selection list changing event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.CollectionExEventArgs"/> instance containing the event data.</param>
        public void RaiseSelectionChangingEvent(CollectionExEventArgs evtArgs)
        {
            CollectionExEventHandler handler;

            // execute high priority handlers anyway
            handler = this.HighPriorityHandlers[m_keySelectionChanging] as CollectionExEventHandler;

            if (handler != null)
            {
                handler(evtArgs);
            }

            if (this.ServiceStatus == Diagram.ServiceStatus.Started || this.ServiceStatus == Diagram.ServiceStatus.Resumed)
            {
                // Raise selection list changing event
                handler = this.Handlers[m_keySelectionChanging] as CollectionExEventHandler;

                if (handler != null)
                {
                    handler(evtArgs);
                }
            }
        }

        /// <summary>
        /// Raises the selection list changed event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.CollectionExEventArgs"/> instance containing the event data.</param>
        public void RaiseSelectionChangedEvent(CollectionExEventArgs evtArgs)
        {
            CollectionExEventHandler handler;

            // execute high priority handlers anyway
            handler = this.HighPriorityHandlers[m_keySelectionChanged] as CollectionExEventHandler;

            if (handler != null)
            {
                handler(evtArgs);
            }

            if (this.ServiceStatus == Diagram.ServiceStatus.Started || this.ServiceStatus == Diagram.ServiceStatus.Resumed)
            {
                // Raise selection list changed event
                handler = this.Handlers[m_keySelectionChanged] as CollectionExEventHandler;

                if (handler != null)
                {
                    handler(evtArgs);
                }
            }
        }

        /// <summary>
        /// Raises the node deselected.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.NodeSelectedEventArgs"/> instance containing the event data.</param>
        public void RaiseNodeDeselected(NodeSelectedEventArgs evtArgs)
        {
            // execute high priority handlers anyway
            NodeSelectedEventHandler handler = this.HighPriorityHandlers[m_keyNodeDeselected] as NodeSelectedEventHandler;

            if (handler != null)
            {
                handler(evtArgs);
            }

            if (CanRaiseEvent())
            {
                handler = this.Handlers[m_keyNodeDeselected] as NodeSelectedEventHandler;

                if (handler != null)
                {
                    handler(evtArgs);
                }
            }
        }

        /// <summary>
        /// Raises the node click.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.NodeMouseEventArgs"/> instance containing the event data.</param>
        public void RaiseNodeClick(NodeMouseEventArgs evtArgs)
        {
            RaiseNodeMouse(m_keyNodeClick, evtArgs);
        }

        /// <summary>
        /// Raises the node double click.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.NodeMouseEventArgs"/> instance containing the event data.</param>
        public void RaiseNodeDoubleClick(NodeMouseEventArgs evtArgs)
        {
            RaiseNodeMouse(m_keyNodeDoubleClick, evtArgs);
        }

        /// <summary>
        /// Raises the node mouse leave.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.NodeMouseEventArgs"/> instance containing the event data.</param>
        public void RaiseNodeMouseLeave(NodeMouseEventArgs evtArgs)
        {
            RaiseNodeMouse(m_keyNodeMouseLeave, evtArgs);
        }

        /// <summary>
        /// Raises the node mouse enter.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.NodeMouseEventArgs"/> instance containing the event data.</param>
        public void RaiseNodeMouseEnter(NodeMouseEventArgs evtArgs)
        {
            RaiseNodeMouse(m_keyNodeMouseEnter, evtArgs);
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Raise the node mouse event.
        /// </summary>
        /// <param name="key">The event key.</param>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.NodeMouseEventArgs"/> instance containing the event data.</param>
        private void RaiseNodeMouse(EventKey key, NodeMouseEventArgs evtArgs)
        {
            // execute high priority handlers anyway
            NodeMouseEventHandler handler = this.HighPriorityHandlers[key] as NodeMouseEventHandler;

            if (handler != null)
            {
                handler(evtArgs);
            }

            if (CanRaiseEvent())
            {
                handler = this.Handlers[key] as NodeMouseEventHandler;

                if (handler != null)
                {
                    handler(evtArgs);
                }
            }
        }

        /// <summary>
        /// Determines whether the event can be raised.
        /// </summary>
        /// <returns>
        /// <c>true</c> if event can be raised; otherwise, <c>false</c>.
        /// </returns>
        private bool CanRaiseEvent()
        {
            return this.ServiceStatus == Diagram.ServiceStatus.Started || this.ServiceStatus == Diagram.ServiceStatus.Resumed;
        }
        #endregion
    }
}
