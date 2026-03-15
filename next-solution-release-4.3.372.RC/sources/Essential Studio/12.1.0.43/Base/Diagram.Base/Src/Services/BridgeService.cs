#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Bridge Manager that use to generate segment bridges 
    /// for intersected nodes in model.
    /// </summary>
    /// <remarks>
    /// Collect the nodes and recreate segment bridges to it new intersection.
    /// </remarks>
    public class BridgeManager
        : Service
    {
        #region Class members
        private Model m_model;
        private int m_nSubTransactions;
        private ArrayList m_connections;
        private bool m_bGenerating;
        #endregion

        #region Class event handlers
        /// <summary>
        /// Occurs when bridge generation is started.
        /// </summary>
        public event EventHandler BridgeGenerationStarted;

        /// <summary>
        /// Occurs when bridge generation is completed.
        /// </summary>
        public event EventHandler BridgeGenerationCompleted;
        #endregion

        #region Class protected properties
        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        protected Model Model
        {
            get { return m_model; }
            set { m_model = value; }
        }

        /// <summary>
        /// Gets the connectors.
        /// </summary>
        /// <value>The connectors.</value>
        protected ArrayList Connectors
        {
            get
            {
                if (m_connections == null)
                    m_connections = new ArrayList();

                return m_connections;
            }
        }
        #endregion

        #region Class public properties
        /// <summary>
        /// Gets a value indicating whether this <see cref="BridgeManager"/> is generating.
        /// </summary>
        /// <value><c>true</c> if generating; otherwise, <c>false</c>.</value>
        public bool Generating
        {
            get { return m_nSubTransactions > 0 || m_bGenerating; }
        }
        #endregion

        #region Class initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="BridgeManager"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        public BridgeManager(Model model)
            : base()
        {
            if (model == null)
                throw new ArgumentNullException("Model parameter can't be null");

            m_model = model;

            // subscribe to model changes
            // m_model.EventSink
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Manually checks connectors intersection.
        /// </summary>
        /// <param name="connectorBase">The connector base.</param>
        public void AddToIntersectCollection(ConnectorBase connectorBase)
        {
            if (!m_bGenerating && this.ServiceStatus != ServiceStatus.Stopped && this.ServiceStatus != ServiceStatus.Paused)
            {
                if (connectorBase != null && !this.Connectors.Contains(connectorBase))
                {
                    this.BeginUpdateIntersection();
                    this.Connectors.Add(connectorBase);
                    this.EndUpdateIntersection();
                }
            }
        }

        /// <summary>
        /// Start of batch operation.
        /// </summary>
        public void BeginUpdateIntersection()
        {
            if (!m_bGenerating)
            {
                m_nSubTransactions++;
            }
        }

        /// <summary>
        /// End of batch operation..
        /// </summary>
        public void EndUpdateIntersection()
        {
            if (!m_bGenerating)
            {
                m_nSubTransactions--;

                // start generate bridges
                if (m_nSubTransactions == 0 && this.ServiceStatus != ServiceStatus.Stopped && this.ServiceStatus != ServiceStatus.Paused)
                {
                    GenerateBridges();
                }
            }
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Called when service is stoped.
        /// </summary>
        protected override void OnStop()
        {
            // clear intersection collection
            this.Connectors.Clear();
            base.OnStop();
        }
        #endregion

        #region Class virtual methods
        /// <summary>
        /// Called when bridge generation is started.
        /// </summary>
        protected virtual void OnBridgeGenerationStarted()
        {
            if (BridgeGenerationStarted != null)
                BridgeGenerationStarted(this, EventArgs.Empty);
        }

        /// <summary>
        /// Called when bridge generation is completed.
        /// </summary>
        protected virtual void OnBridgeGenerationCompleted()
        {
            if (BridgeGenerationCompleted != null)
                BridgeGenerationCompleted(this, EventArgs.Empty);
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Generates the bridges. Work only is Model.LineBridgingEnabled is enable.
        /// </summary>
        private void GenerateBridges()
        {
            if (!m_bGenerating && this.Model.LineBridgingEnabled && this.Connectors.Count > 0)
            {
                OnBridgeGenerationStarted();
                m_bGenerating = true;

                NodeCollection nodes = GetAllConnectors(this.Model);

                // more that one connector
                if (nodes.Count > 1)
                {
                    foreach (ConnectorBase connector in this.Connectors)
                    {
                        // update connector intersecting
                        UpdateConnectorsIntersecting(connector, nodes);
                    }
                }

                this.Connectors.Clear();

                m_bGenerating = false;
                OnBridgeGenerationCompleted();
            }
        }

        /// <summary>
        /// Update the connector intersecting.
        /// </summary>
        /// <param name="connector">The connector.</param>
        /// <param name="nodes">The present connectors in model.</param>
        private void UpdateConnectorsIntersecting(ConnectorBase connector, NodeCollection nodes)
        {
            ArrayList segments = connector.LineSegments;

            for (int segmentIndex = 0, nLength = segments.Count; segmentIndex < nLength; segmentIndex++)
            {
                // Update segment bridges.
                connector.IntersectSegmentWith(segmentIndex, nodes);
            }
        }

        /// <summary>
        /// Gets all connectors from composite nodes.
        /// </summary>
        /// <param name="composite">The composite.</param>
        /// <returns>The connectors from composite node.</returns>
        protected NodeCollection GetAllConnectors(ICompositeNode composite)
        {
            NodeCollection nodesToReturn = new NodeCollection();

            // iterate all group nodes and find connectors
            for (int i = 0, length = composite.ChildCount; i < length; i++)
            {
                Node node = composite.GetChild(i);
                ICompositeNode group = node as ICompositeNode;
                ConnectorBase connector = node as ConnectorBase;

                if (group != null)
                {
                    nodesToReturn.AddRange(GetAllConnectors(group));
                }
                else if (connector != null)
                {
                    // filter nodes
                    nodesToReturn.Add(node);
                }
            }

            return nodesToReturn;
        }
        #endregion
    }
}
