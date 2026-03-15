#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Drawing;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Inherits from EventSink and contains additional node and primitives transformation events.
    /// </summary>
    public class DocumentEventSink
        : EventSink
    {
        #region Class EventKeys
        private static readonly EventKey m_keyConnectionsChanged;
        private static readonly EventKey m_keyConnectionsChanging;
        private static readonly EventKey m_keyPortsChanged;
        private static readonly EventKey m_keyLayersChanged;
        private static readonly EventKey m_keyLabelsChanged;
        private static readonly EventKey m_keyZOrderChanged;
        private static readonly EventKey m_keyZOrderChanging;
        private static readonly EventKey m_keyVertexChanged;
        private static readonly EventKey m_keyVertexChanging;
        private static readonly EventKey m_keyPinPointChanged;
        private static readonly EventKey m_keyPinPointChanging;
        private static readonly EventKey m_keyPinOffsetChanged;
        private static readonly EventKey m_keyPinOffsetChanging;
        private static readonly EventKey m_keyRotationChanged;
        private static readonly EventKey m_keyRotationChanging;
        private static readonly EventKey m_keyFlipChanged;
        private static readonly EventKey m_keyFlipChanging;
        private static readonly EventKey m_keySizeChanged;
        private static readonly EventKey m_keySizeChanging;
        private static readonly EventKey m_keyDocumentBeginUpdate;
        private static readonly EventKey m_keyDocumentEndUpdate;
        #endregion

        #region Class initalize/finalize methods
        /// <summary>
        /// Initializes static members of the <see cref="DocumentEventSink"/> class.
        /// </summary>
        static DocumentEventSink()
        {
            m_keyPinPointChanged = new EventKey();
            m_keyPinPointChanging = new EventKey();
            m_keyPinOffsetChanged = new EventKey();
            m_keyPinOffsetChanging = new EventKey();
            m_keySizeChanged = new EventKey();
            m_keySizeChanging = new EventKey();
            m_keyRotationChanged = new EventKey();
            m_keyRotationChanging = new EventKey();
            m_keyFlipChanged = new EventKey();
            m_keyFlipChanging = new EventKey();

            m_keyConnectionsChanged = new EventKey();
            m_keyConnectionsChanging = new EventKey();
            m_keyPortsChanged = new EventKey();
            m_keyLabelsChanged = new EventKey();
            m_keyLayersChanged = new EventKey();
            m_keyZOrderChanged = new EventKey();
            m_keyZOrderChanging = new EventKey();
            m_keyVertexChanged = new EventKey();
            m_keyVertexChanging = new EventKey();
            m_keyVertexChanged = new EventKey();
            m_keyVertexChanging = new EventKey();
            m_keyDocumentBeginUpdate = new EventKey();
            m_keyDocumentEndUpdate = new EventKey();
        }
        #endregion

        #region Class events
        /// <summary>
        /// Occurs when pin point changing.
        /// </summary>
        public event PinPointChangingEventHandler PinPointChanging
        {
            add { AddHandler(m_keyPinPointChanging, value); }
            remove { RemoveHandler(m_keyPinPointChanging, value); }
        }

        /// <summary>
        /// Occurs when pin point changed.
        /// </summary>
        public event PinPointChangedEventHandler PinPointChanged
        {
            add { AddHandler(m_keyPinPointChanged, value); }
            remove { RemoveHandler(m_keyPinPointChanged, value); }
        }

        /// <summary>
        /// Occurs when pin offset changing.
        /// </summary>
        public event PinOffsetChangingEventHandler PinOffsetChanging
        {
            add { AddHandler(m_keyPinOffsetChanging, value); }
            remove { RemoveHandler(m_keyPinOffsetChanging, value); }
        }

        /// <summary>
        /// Occurs when pin offset changed.
        /// </summary>
        public event PinOffsetChangedEventHandler PinOffsetChanged
        {
            add { AddHandler(m_keyPinOffsetChanged, value); }
            remove { RemoveHandler(m_keyPinOffsetChanged, value); }
        }

        /// <summary>
        /// Occurs when size changed.
        /// </summary>
        public event SizeChangedEventHandler SizeChanged
        {
            add { AddHandler(m_keySizeChanged, value); }
            remove { RemoveHandler(m_keySizeChanged, value); }
        }

        /// <summary>
        /// Occurs when size changing.
        /// </summary>
        public event SizeChangingEventHandler SizeChanging
        {
            add { AddHandler(m_keySizeChanging, value); }
            remove { RemoveHandler(m_keySizeChanging, value); }
        }

        /// <summary>
        /// Occurs when rotation angle changed.
        /// </summary>
        public event RotationChangedEventHandler RotationChanged
        {
            add { AddHandler(m_keyRotationChanged, value); }
            remove { RemoveHandler(m_keyRotationChanged, value); }
        }

        /// <summary>
        /// Occurs when rotation angle changing.
        /// </summary>
        public event RotationChangingEventHandler RotationChanging
        {
            add { AddHandler(m_keyRotationChanging, value); }
            remove { RemoveHandler(m_keyRotationChanging, value); }
        }

        /// <summary>
        /// Occurs when flip flag changed.
        /// </summary>
        public event FlipChangedEventHandler FlipChanged
        {
            add { AddHandler(m_keyFlipChanged, value); }
            remove { RemoveHandler(m_keyFlipChanged, value); }
        }

        /// <summary>
        /// Occurs when flip flag changing.
        /// </summary>
        public event FlipChangingEventHandler FlipChanging
        {
            add { AddHandler(m_keyFlipChanging, value); }
            remove { RemoveHandler(m_keyFlipChanging, value); }
        }

        /// <summary>
        /// Occurs when document begin update.
        /// </summary>
        public event EventHandler DocumentBeginUpdate
        {
            add { AddHandler(m_keyDocumentBeginUpdate, value); }
            remove { RemoveHandler(m_keyDocumentBeginUpdate, value); }
        }

        /// <summary>
        /// Occurs when document end update.
        /// </summary>
        public event EventHandler DocumentEndUpdate
        {
            add { AddHandler(m_keyDocumentEndUpdate, value); }
            remove { RemoveHandler(m_keyDocumentEndUpdate, value); }
        }

        /// <summary>
        /// Occurs when vertex changed.
        /// </summary>
        public event VertexChangedEventHandler VertexChanged
        {
            add { AddHandler(m_keyVertexChanged, value); }
            remove { RemoveHandler(m_keyVertexChanged, value); }
        }

        /// <summary>
        /// Occurs when vertex changing.
        /// </summary>
        public event VertexChangingEventHandler VertexChanging
        {
            add { AddHandler(m_keyVertexChanging, value); }
            remove { RemoveHandler(m_keyVertexChanging, value); }
        }

        /// <summary>
        /// Occurs when Z order changed.
        /// </summary>
        public event ZOrderChangedEventHandler ZOrderChanged
        {
            add { AddHandler(m_keyZOrderChanged, value); }
            remove { RemoveHandler(m_keyZOrderChanged, value); }
        }

        /// <summary>
        /// Occurs when Z order changing.
        /// </summary>
        public event ZOrderChangingEventHandler ZOrderChanging
        {
            add { AddHandler(m_keyZOrderChanging, value); }
            remove { RemoveHandler(m_keyZOrderChanging, value); }
        }

        /// <summary>
        /// Occurs when ports collections changed.
        /// </summary>
        public event CollectionExEventHandler PortsChanged
        {
            add { AddHandler(m_keyPortsChanged, value); }
            remove { RemoveHandler(m_keyPortsChanged, value); }
        }

        /// <summary>
        /// Occurs when labels collections changed.
        /// </summary>
        public event CollectionExEventHandler LabelsChanged
        {
            add { AddHandler(m_keyLabelsChanged, value); }
            remove { RemoveHandler(m_keyLabelsChanged, value); }
        }

        /// <summary>
        /// Occurs when layers collections changed.
        /// </summary>
        public event CollectionExEventHandler LayersChanged
        {
            add { AddHandler(m_keyLayersChanged, value); }
            remove { RemoveHandler(m_keyLayersChanged, value); }
        }

        /// <summary>
        /// Occurs when connections collections changed.
        /// </summary>
        public event CollectionExEventHandler ConnectionsChanged
        {
            add { AddHandler(m_keyConnectionsChanged, value); }
            remove { RemoveHandler(m_keyConnectionsChanged, value); }
        }

        /// <summary>
        /// Occurs when connections collections changing.
        /// </summary>
        public event CollectionExEventHandler ConnectionsChanging
        {
            add { AddHandler(m_keyConnectionsChanging, value); }
            remove { RemoveHandler(m_keyConnectionsChanging, value); }
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Raises the pin point changing.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.PinPointChangingEventArgs"/> instance containing the event data.</param>
        /// <returns>true, if raise pin point changing event.</returns>
        public bool RaisePinPointChanging(PinPointChangingEventArgs evtArgs)
        {
            PinPointChangingEventHandler handler;

            // execute high priority handlers anyway
            handler = this.HighPriorityHandlers[m_keyPinPointChanging] as PinPointChangingEventHandler;

            if (handler != null)
            {
                handler(evtArgs);
            }

            if (this.ServiceStatus == Diagram.ServiceStatus.Started || this.ServiceStatus == Diagram.ServiceStatus.Resumed)
            {
                // Raise NodesChanged event
                handler = this.Handlers[m_keyPinPointChanging] as PinPointChangingEventHandler;

                if (handler != null)
                {
                    handler(evtArgs);
                }
            }

            return !evtArgs.Cancel;
        }

        /// <summary>
        /// Raises the pin point changed.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.PinPointChangedEventArgs"/> instance containing the event data.</param>
        public void RaisePinPointChanged(PinPointChangedEventArgs evtArgs)
        {
            PinPointChangedEventHandler handler;

            // execute high priority handlers anyway
            handler = this.HighPriorityHandlers[m_keyPinPointChanged] as PinPointChangedEventHandler;

            if (handler != null)
            {
                handler(evtArgs);
            }

            if (this.ServiceStatus == Diagram.ServiceStatus.Started || this.ServiceStatus == Diagram.ServiceStatus.Resumed)
            {
                // Raise NodesChanged event
                handler = this.Handlers[m_keyPinPointChanged] as PinPointChangedEventHandler;

                if (handler != null)
                {
                    handler(evtArgs);
                }
            }
        }

        /// <summary>
        /// Raises the pin offset changing.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.PinOffsetChangingEventArgs"/> instance containing the event data.</param>
        /// <returns>true, if raise pin point offset changing event.</returns>
        public bool RaisePinOffsetChanging(PinOffsetChangingEventArgs evtArgs)
        {
            PinOffsetChangingEventHandler handler;

            // execute high priority handlers anyway
            handler = this.HighPriorityHandlers[m_keyPinOffsetChanging] as PinOffsetChangingEventHandler;

            if (handler != null)
            {
                handler(evtArgs);
            }

            if (this.ServiceStatus == Diagram.ServiceStatus.Started || this.ServiceStatus == Diagram.ServiceStatus.Resumed)
            {
                // Raise NodesChanged event
                handler = this.Handlers[m_keyPinOffsetChanging] as PinOffsetChangingEventHandler;

                if (handler != null)
                {
                    handler(evtArgs);
                }
            }

            return !evtArgs.Cancel;
        }

        /// <summary>
        /// Raises the pin offset changed.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.PinOffsetChangedEventArgs"/> instance containing the event data.</param>
        public void RaisePinOffsetChanged(PinOffsetChangedEventArgs evtArgs)
        {
            PinOffsetChangedEventHandler handler;

            // execute high priority handlers anyway
            handler = this.HighPriorityHandlers[m_keyPinOffsetChanged] as PinOffsetChangedEventHandler;

            if (handler != null)
            {
                handler(evtArgs);
            }

            if (this.ServiceStatus == Diagram.ServiceStatus.Started || this.ServiceStatus == Diagram.ServiceStatus.Resumed)
            {
                // Raise NodesChanged event
                handler = this.Handlers[m_keyPinOffsetChanged] as PinOffsetChangedEventHandler;

                if (handler != null)
                {
                    handler(evtArgs);
                }
            }
        }

        /// <summary>
        /// Raises the size changing.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.SizeChangingEventArgs"/> instance containing the event data.</param>
        /// <returns>true, if raise size changing event.</returns>
        public bool RaiseSizeChanging(SizeChangingEventArgs evtArgs)
        {
            SizeChangingEventHandler handler;

            // execute high priority handlers anyway
            handler = this.HighPriorityHandlers[m_keySizeChanging] as SizeChangingEventHandler;

            if (handler != null)
            {
                handler(evtArgs);
            }

            if (this.ServiceStatus == Diagram.ServiceStatus.Started || this.ServiceStatus == Diagram.ServiceStatus.Resumed)
            {
                // Raise NodesChanged event
                handler = this.Handlers[m_keySizeChanging] as SizeChangingEventHandler;

                if (handler != null)
                {
                    handler(evtArgs);
                }
            }

            return !evtArgs.Cancel;
        }

        /// <summary>
        /// Raises the size changed.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.SizeChangedEventArgs"/> instance containing the event data.</param>
        public void RaiseSizeChanged(SizeChangedEventArgs evtArgs)
        {
            SizeChangedEventHandler handler;

            // execute high priority handlers anyway
            handler = this.HighPriorityHandlers[m_keySizeChanged] as SizeChangedEventHandler;

            if (handler != null)
            {
                handler(evtArgs);
            }

            if (this.ServiceStatus == Diagram.ServiceStatus.Started || this.ServiceStatus == Diagram.ServiceStatus.Resumed)
            {
                // Raise NodesChanged event
                handler = this.Handlers[m_keySizeChanged] as SizeChangedEventHandler;

                if (handler != null)
                {
                    handler(evtArgs);
                }
            }
        }

        /// <summary>
        /// Raises the rotation changing.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.RotationChangingEventArgs"/> instance containing the event data.</param>
        /// <returns>true, if raise rotation changing event.</returns>
        public bool RaiseRotationChanging(RotationChangingEventArgs evtArgs)
        {
            RotationChangingEventHandler handler;

            // execute high priority handlers anyway
            handler = this.HighPriorityHandlers[m_keyRotationChanging] as RotationChangingEventHandler;

            if (handler != null)
            {
                handler(evtArgs);
            }

            if (this.ServiceStatus == Diagram.ServiceStatus.Started || this.ServiceStatus == Diagram.ServiceStatus.Resumed)
            {
                // Raise NodesChanged event
                handler = this.Handlers[m_keyRotationChanging] as RotationChangingEventHandler;

                if (handler != null)
                {
                    handler(evtArgs);
                }
            }

            return !evtArgs.Cancel;
        }

        /// <summary>
        /// Raises the rotation changed.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.RotationChangedEventArgs"/> instance containing the event data.</param>
        public void RaiseRotationChanged(RotationChangedEventArgs evtArgs)
        {
            RotationChangedEventHandler handler;

            // execute high priority handlers anyway
            handler = this.HighPriorityHandlers[m_keyRotationChanged] as RotationChangedEventHandler;

            if (handler != null)
            {
                handler(evtArgs);
            }

            if (this.ServiceStatus == Diagram.ServiceStatus.Started || this.ServiceStatus == Diagram.ServiceStatus.Resumed)
            {
                // Raise NodesChanged event
                handler = this.Handlers[m_keyRotationChanged] as RotationChangedEventHandler;

                if (handler != null)
                {
                    handler(evtArgs);
                }
            }
        }

        /// <summary>
        /// Raises the flip changing.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.FlipChangingEventArgs"/> instance containing the event data.</param>
        /// <returns>true, if raise flip changing event.</returns>
        public bool RaiseFlipChanging(FlipChangingEventArgs evtArgs)
        {
            FlipChangingEventHandler handler;

            // execute high priority handlers anyway
            handler = this.HighPriorityHandlers[m_keyFlipChanging] as FlipChangingEventHandler;

            if (handler != null)
            {
                handler(evtArgs);
            }

            if (this.ServiceStatus == Diagram.ServiceStatus.Started || this.ServiceStatus == Diagram.ServiceStatus.Resumed)
            {
                // Raise NodesChanged event
                handler = this.Handlers[m_keyFlipChanging] as FlipChangingEventHandler;

                if (handler != null)
                {
                    handler(evtArgs);
                }
            }

            return !evtArgs.Cancel;
        }

        /// <summary>
        /// Raises the flip changed.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.FlipChangedEventArgs"/> instance containing the event data.</param>
        public void RaiseFlipChanged(FlipChangedEventArgs evtArgs)
        {
            FlipChangedEventHandler handler;

            // execute high priority handlers anyway
            handler = this.HighPriorityHandlers[m_keyFlipChanged] as FlipChangedEventHandler;

            if (handler != null)
            {
                handler(evtArgs);
            }

            if (this.ServiceStatus == Diagram.ServiceStatus.Started || this.ServiceStatus == Diagram.ServiceStatus.Resumed)
            {
                // Raise NodesChanged event
                handler = this.Handlers[m_keyFlipChanged] as FlipChangedEventHandler;

                if (handler != null)
                {
                    handler(evtArgs);
                }
            }
        }

        /// <summary>
        /// Raises the document end update.
        /// </summary>
        public void RaiseDocumentEndUpdate()
        {
            EventArgs evtArgs = new EventArgs();

            // execute high priority handlers anyway
            EventHandler handler = this.HighPriorityHandlers[m_keyDocumentEndUpdate] as EventHandler;

            if (handler != null)
            {
                handler(this, evtArgs);
            }

            if (this.ServiceStatus == Diagram.ServiceStatus.Started || this.ServiceStatus == Diagram.ServiceStatus.Resumed)
            {
                // Raise NodesChanged event
                handler = this.Handlers[m_keyDocumentEndUpdate] as EventHandler;

                if (handler != null)
                {
                    handler(this, evtArgs);
                }
            }
        }

        /// <summary>
        /// Raises the document begin update.
        /// </summary>
        public void RaiseDocumentBeginUpdate()
        {
            EventArgs evtArgs = new EventArgs();

            // execute high priority handlers anyway
            EventHandler handler = this.HighPriorityHandlers[m_keyDocumentBeginUpdate] as EventHandler;

            if (handler != null)
            {
                handler(this, evtArgs);
            }

            if (this.ServiceStatus == Diagram.ServiceStatus.Started || this.ServiceStatus == Diagram.ServiceStatus.Resumed)
            {
                // Raise NodesChanged event
                handler = this.Handlers[m_keyVertexChanged] as EventHandler;

                if (handler != null)
                {
                    handler(this, evtArgs);
                }
            }
        }

        /// <summary>
        /// Raises the vertex changing.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.VertexChangingEventArgs"/> instance containing the event data.</param>
        /// <returns>true, if raise vertex changing event.</returns>
        public bool RaiseVertexChanging(VertexChangingEventArgs evtArgs)
        {
            VertexChangingEventHandler handler;

            // execute high priority handlers anyway
            handler = this.HighPriorityHandlers[m_keyVertexChanging] as VertexChangingEventHandler;

            if (handler != null)
            {
                handler(evtArgs);
            }

            if (this.ServiceStatus == Diagram.ServiceStatus.Started || this.ServiceStatus == Diagram.ServiceStatus.Resumed)
            {
                // Raise NodesChanged event
                handler = this.Handlers[m_keyVertexChanging] as VertexChangingEventHandler;

                if (handler != null)
                {
                    handler(evtArgs);
                }
            }

            return !evtArgs.Cancel;
        }

        /// <summary>
        /// Raises the vertex changed.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.VertexChangedEventArgs"/> instance containing the event data.</param>
        public void RaiseVertexChanged(VertexChangedEventArgs evtArgs)
        {
            VertexChangedEventHandler handler;

            // execute high priority handlers anyway
            handler = this.HighPriorityHandlers[m_keyVertexChanged] as VertexChangedEventHandler;

            if (handler != null)
            {
                handler(evtArgs);
            }

            if (this.ServiceStatus == Diagram.ServiceStatus.Started || this.ServiceStatus == Diagram.ServiceStatus.Resumed)
            {
                // Raise NodesChanged event
                handler = this.Handlers[m_keyVertexChanged] as VertexChangedEventHandler;

                if (handler != null)
                {
                    handler(evtArgs);
                }
            }
        }

        /// <summary>
        /// Raises the Z order changing.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.ZOrderChangingEventArgs"/> instance containing the event data.</param>
        /// <returns>true, if raise Z order changing event.</returns>
        public bool RaiseZOrderChanging(ZOrderChangingEventArgs evtArgs)
        {
            ZOrderChangingEventHandler handler;

            // execute high priority handlers anyway
            handler = this.HighPriorityHandlers[m_keyZOrderChanging] as ZOrderChangingEventHandler;

            if (handler != null)
            {
                handler(evtArgs);
            }

            if (this.ServiceStatus == Diagram.ServiceStatus.Started || this.ServiceStatus == Diagram.ServiceStatus.Resumed)
            {
                // Raise NodesChanged event
                handler = this.Handlers[m_keyZOrderChanging] as ZOrderChangingEventHandler;

                if (handler != null)
                {
                    handler(evtArgs);
                }
            }

            return !evtArgs.Cancel;
        }

        /// <summary>
        /// Raises the Z order changed.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.ZOrderChangedEventArgs"/> instance containing the event data.</param>
        public void RaiseZOrderChanged(ZOrderChangedEventArgs evtArgs)
        {
            ZOrderChangedEventHandler handler;

            // execute high priority handlers anyway
            handler = this.HighPriorityHandlers[m_keyZOrderChanged] as ZOrderChangedEventHandler;

            if (handler != null)
            {
                handler(evtArgs);
            }

            if (this.ServiceStatus == Diagram.ServiceStatus.Started || this.ServiceStatus == Diagram.ServiceStatus.Resumed)
            {
                // Raise NodesChanged event
                handler = this.Handlers[m_keyZOrderChanged] as ZOrderChangedEventHandler;

                if (handler != null)
                {
                    handler(evtArgs);
                }
            }
        }

        /// <summary>
        /// Raises the connections changed event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.CollectionExEventArgs"/> instance containing the event data.</param>
        public void RaiseConnectionsChangedEvent(CollectionExEventArgs evtArgs)
        {
            CollectionExEventHandler handler;

            // execute high priority handlers anyway
            handler = this.HighPriorityHandlers[m_keyConnectionsChanged] as CollectionExEventHandler;

            if (handler != null)
            {
                handler(evtArgs);
            }

            if (this.ServiceStatus == Diagram.ServiceStatus.Started || this.ServiceStatus == Diagram.ServiceStatus.Resumed)
            {
                // Raise NodesChanged event
                handler = this.Handlers[m_keyConnectionsChanged] as CollectionExEventHandler;

                if (handler != null)
                {
                    handler(evtArgs);
                }
            }
        }

        /// <summary>
        /// Raises the connections changing event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.CollectionExEventArgs"/> instance containing the event data.</param>
        public void RaiseConnectionsChangingEvent(CollectionExEventArgs evtArgs)
        {
            CollectionExEventHandler handler;

            // execute high priority handlers anyway
            handler = this.HighPriorityHandlers[m_keyConnectionsChanging] as CollectionExEventHandler;

            if (handler != null)
            {
                handler(evtArgs);
            }

            if (this.ServiceStatus == Diagram.ServiceStatus.Started || this.ServiceStatus == Diagram.ServiceStatus.Resumed)
            {
                // Raise NodesChanged event
                handler = this.Handlers[m_keyConnectionsChanging] as CollectionExEventHandler;

                if (handler != null)
                {
                    handler(evtArgs);
                }
            }
        }

        /// <summary>
        /// Raises the ports changed event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.CollectionExEventArgs"/> instance containing the event data.</param>
        public void RaisePortsChangedEvent(CollectionExEventArgs evtArgs)
        {
            CollectionExEventHandler handler;

            // execute high priority handlers anyway
            handler = this.HighPriorityHandlers[m_keyPortsChanged] as CollectionExEventHandler;

            if (handler != null)
            {
                handler(evtArgs);
            }

            if (this.ServiceStatus == Diagram.ServiceStatus.Started || this.ServiceStatus == Diagram.ServiceStatus.Resumed)
            {
                // Raise NodesChanged event
                handler = this.Handlers[m_keyPortsChanged] as CollectionExEventHandler;

                if (handler != null)
                {
                    handler(evtArgs);
                }
            }
        }

        /// <summary>
        /// Raises the labels changed event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.CollectionExEventArgs"/> instance containing the event data.</param>
        public void RaiseLabelsChangedEvent(CollectionExEventArgs evtArgs)
        {
            CollectionExEventHandler handler;

            // execute high priority handlers anyway
            handler = this.HighPriorityHandlers[m_keyLabelsChanged] as CollectionExEventHandler;

            if (handler != null)
            {
                handler(evtArgs);
            }

            if (this.ServiceStatus == Diagram.ServiceStatus.Started || this.ServiceStatus == Diagram.ServiceStatus.Resumed)
            {
                // Raise NodesChanged event
                handler = this.Handlers[m_keyLabelsChanged] as CollectionExEventHandler;

                if (handler != null)
                {
                    handler(evtArgs);
                }
            }
        }

        /// <summary>
        /// Raises the layers changed event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.CollectionExEventArgs"/> instance containing the event data.</param>
        public void RaiseLayersChangedEvent(CollectionExEventArgs evtArgs)
        {
            CollectionExEventHandler handler;

            // execute high priority handlers anyway
            handler = this.HighPriorityHandlers[m_keyLayersChanged] as CollectionExEventHandler;

            if (handler != null)
            {
                handler(evtArgs);
            }

            if (this.ServiceStatus == Diagram.ServiceStatus.Started || this.ServiceStatus == Diagram.ServiceStatus.Resumed)
            {
                // Raise NodesChanged event
                handler = this.Handlers[m_keyLayersChanged] as CollectionExEventHandler;

                if (handler != null)
                {
                    handler(evtArgs);
                }
            }
        }
        #endregion
    }
}
