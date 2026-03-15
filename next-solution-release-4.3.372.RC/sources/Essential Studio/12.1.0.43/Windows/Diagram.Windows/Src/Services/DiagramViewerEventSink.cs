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
    /// Inherits from ViewerEventSink and contains additional tool events handles.
    /// </summary>
    public class DiagramViewerEventSink
        : ViewerEventSink
    {
        #region Class EventKeys
        private static readonly EventKey m_keyToolActivated;
        private static readonly EventKey m_keyToolDeactivated;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes static members of the <see cref="DiagramViewerEventSink"/> class.
        /// </summary>
        static DiagramViewerEventSink()
        {
            m_keyToolActivated = new EventKey();
            m_keyToolDeactivated = new EventKey();
        }
        #endregion

        #region Class events
        /// <summary>
        /// Occurs when tool is activated.
        /// </summary>
        public event ToolEventHandler ToolActivated
        {
            add { AddHandler(m_keyToolActivated, value); }
            remove { RemoveHandler(m_keyToolActivated, value); }
        }

        /// <summary>
        /// Occurs when tool is deactivated.
        /// </summary>
        public event ToolEventHandler ToolDeactivated
        {
            add { AddHandler(m_keyToolDeactivated, value); }
            remove { RemoveHandler(m_keyToolDeactivated, value); }
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Raises the tool activated.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.ToolEventArgs"/> instance containing the event data.</param>
        public void RaiseToolActivated(ToolEventArgs evtArgs)
        {
            RaiseToolEvent(m_keyToolActivated, evtArgs);
        }

        /// <summary>
        /// Raises the tool deactivated.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.ToolEventArgs"/> instance containing the event data.</param>
        public void RaiseToolDeactivated(ToolEventArgs evtArgs)
        {
            RaiseToolEvent(m_keyToolDeactivated, evtArgs);
        }
        #endregion

        #region Class helper methods
        private void RaiseToolEvent(EventKey evtKey, ToolEventArgs evtArgs)
        {
            // execute high priority handlers anyway
            ToolEventHandler handler = this.HighPriorityHandlers[evtKey] as ToolEventHandler;

            if (handler != null)
            {
                handler(evtArgs);
            }

            if (this.ServiceStatus == Diagram.ServiceStatus.Started || this.ServiceStatus == Diagram.ServiceStatus.Resumed)
            {
                // Raise NodesChanged event
                handler = this.Handlers[evtKey] as ToolEventHandler;

                if (handler != null)
                {
                    handler(evtArgs);
                }
            }
        }
        #endregion
    }
}
