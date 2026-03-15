// <copyright file="LineRouter.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

#region File using derectives

using System;
using System.Collections;
using System.ComponentModel;

#endregion

namespace Syncfusion.Windows.Diagram
{
    /// <summary>
    /// A line router provides line routing services for a diagram.
    /// View.
    /// </summary>   
    public abstract class LineRouter 
    {
        #region Fields
        /// <summary>
        /// Indicates whether line routing engine is updating routes right now.
        /// </summary>
        protected bool m_bInAction;

        /// <summary>
        /// Document to which LineRouting engine is attached.
        /// </summary>
        private DiagramView m_view;

        /// <summary>
        /// Distance from routing line to obstacles.
        /// </summary>
        private int m_nDistance;

        /// <summary>
        /// Document update requests.
        /// </summary>
        private int m_nUpdateRequests;
        
        #endregion

        #region Initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="LineRouter"/> class.
        /// </summary>
        public LineRouter()
        {
            m_nDistance = 5;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineRouter"/> class.
        /// </summary>
        /// <param name="view">View to attach line router to</param>
        public LineRouter(DiagramView view)
            : this()
        {
            if (view == null)
                throw new ArgumentNullException("view");

            this.View = view;
        }
       
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets distance to obstacles.
        /// </summary>        
        [Description("Distance from routing connector to obstacles.")]
        [DefaultValue(5)]
        public int DistanceToObstacles
        {
            get 
            { 
                return m_nDistance; 
            }
            set
            {
                if (m_nDistance != value)
                {
                    m_nDistance = value;

                    // reroute document connectors
                    RouteAllViewConnectors();
                }
            }
        }

        /// <summary>
        /// Gets or sets view the line router is attached to.        
        /// </summary>       
        internal DiagramView View
        {
            get
            {
                return m_view;
            }
            set
            {
                SetNewView(value);
            }
        }
                
        /// <summary>
        /// Gets or sets the update requests.
        /// </summary>
        /// <value>The update requests.</value>
        protected int UpdateRequests
        {
            get
            {
                return m_nUpdateRequests;
            }
            set
            {
                m_nUpdateRequests = value;
                DiagramView view = this.View;

                if (m_nUpdateRequests == 0 && view != null && view.LineRoutingEnabled)
                {
                    if (!m_bInAction)
                    {
                        m_bInAction = true;
                        RouteAllViewConnectorsInternal();                        
                        m_bInAction = false;                        
                        m_nUpdateRequests = 0;
                    }
                }
            }
        }
        
        #endregion

        #region Public Methods
        /// <summary>
        /// Reroutes all view connectors.
        /// </summary>
        internal void RouteAllViewConnectors()
        {
            DiagramView view = this.View;

            if (!m_bInAction && view != null)
            {
                m_bInAction = true;
                RouteAllViewConnectorsInternal();
                m_bInAction = false;
            }
        }
        
        #endregion

        #region Helper Methods       

        /// <summary>
        /// Route all connection from model.
        /// </summary>
        protected abstract void RouteAllViewConnectorsInternal();        

        /// <summary>
        /// Set new View to the instance.
        /// </summary>
        /// <param name="newValue">The new View.</param>
        protected virtual void SetNewView(DiagramView newValue)
        {
            m_view = newValue;
        }
       
        #endregion        
    }
}
