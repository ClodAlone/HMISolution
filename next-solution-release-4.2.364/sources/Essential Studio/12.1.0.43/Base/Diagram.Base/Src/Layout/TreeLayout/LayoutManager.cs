#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.ComponentModel;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Base class for layout managers.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class provides the basic plumbing for layout managers. A layout
    /// manager is an object that controls the positioning of nodes in a model.
    /// Each layout manager object is attached to a single model by the
    /// <see cref="Syncfusion.Windows.Forms.Diagram.LayoutManager.Model"/>
    /// property.
    /// </para>
    /// <para>
    /// Layout managers can operate one of two modes: manual or
    /// auto-update. When the
    /// <see cref="Syncfusion.Windows.Forms.Diagram.LayoutManager.AutoLayout"/>
    /// flag is set to True, the layout manager responds to events in the model
    /// by automatically repositioning nodes in the model. The
    /// <see cref="Syncfusion.Windows.Forms.Diagram.LayoutManager.UpdateLayout"/>
    /// method can be called at any time to reposition the nodes in the model.
    /// If the AutoLayout flag is False, then UpdateLayout must be called manually
    /// in order to update the layout of nodes in the model.
    /// </para>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Model"/>
    /// </remarks>
    public abstract class LayoutManager : Component
    {
        #region Class members
        /// <summary>
        /// Nodes who  will lay out.
        /// </summary>
        private NodeCollection m_lstNodes;

        /// <summary>
        /// Reference to model and layout.
        /// </summary>
        protected Model mdl = null;

        /// <summary>
        /// Flag indicationg if layouting in process.
        /// </summary>
        private bool layoutUpdatingFlag = false;

        /// <summary>
        /// Flag indicating if layout is to be updated automatically.
        /// </summary>
        protected bool autoLayoutFlag = false;
        private float m_fLeftMargin = 10;
        private float m_fTopMargin = 10;
        #endregion

        #region Class event handlers
        /// <summary>
        /// Fired after the layout has been updated by the layout manager.
        /// </summary>
        public EventHandler LayoutUpdated;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the nodes collection by who will apply layout.
        /// </summary>
        /// <value>The nodes collection.</value>
        [Browsable(false)]
        public NodeCollection Nodes
        {
            get
            {
                if (m_lstNodes == null)
                {
                    m_lstNodes = new NodeCollection();
                }

                return m_lstNodes;
            }
        }

        /// <summary>
        /// Gets or sets the model attached to this layout manager.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The model referenced by this property is updated when the
        /// UpdateLayout method is called.
        /// </para>
        /// </remarks>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Model Model
        {
            get 
            { 
                return mdl; 
            }
            set
            {
                if (mdl != value)
                {
                    if (mdl != null)
                    {
                        mdl.EventSink.DocumentEndUpdate -= new EventHandler(mdl_ViewUpdate);
                        mdl.EventSink.PropertyChanged -= new PropertyChangedEventHandler(OnPropertyChanged);
                        mdl.EventSink.NodeCollectionChanged -= new CollectionExEventHandler(OnNodeCollectionChanged);
                        mdl.EventSink.ConnectionsChanged -= new CollectionExEventHandler(OnConnectionsChanged);
                    }

                    mdl = value;

                    if (mdl != null)
                    {
                        mdl.EventSink.DocumentEndUpdate += new EventHandler(mdl_ViewUpdate);
                        mdl.EventSink.PropertyChanged += new PropertyChangedEventHandler(OnPropertyChanged);
                        mdl.EventSink.NodeCollectionChanged += new CollectionExEventHandler(OnNodeCollectionChanged);
                        mdl.EventSink.ConnectionsChanged += new CollectionExEventHandler(OnConnectionsChanged);
                        // apply layput to all model.
                        this.Nodes.AddRange(mdl.Nodes);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the layout manager automatically updates the layout of the model.
        /// </summary>
        [
        Browsable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Category("Behavior"),
        Description("Determines if the layout manager automatically updates the layout of the model.")
        ]
        public bool AutoLayout
        {
            get
            {
                return this.autoLayoutFlag;
            }
            set
            {
                this.autoLayoutFlag = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether layout update state.
        /// </summary>
        protected bool UpdatingLayout
        {
            get
            {
                return this.layoutUpdatingFlag;
            }
            set
            {
                if (this.layoutUpdatingFlag != value)
                {
                    this.layoutUpdatingFlag = value;
                }
            }
        }

        /// <summary>
        /// Gets or Sets the left margin for the layout manager.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(10f)]
        [Description("Specifies layout manager left margin.")]
        public float LeftMargin
        {
            get
            {
                return m_fLeftMargin;
            }
            set
            {
                if (value != m_fLeftMargin)
                    m_fLeftMargin = value;
            }
        }

        /// <summary>
        /// Gets or Sets the top margin for the layout manager.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(10f)]
        [Description("Specifies layout manager top margin.")]
        public float TopMargin
        {
            get
            {
                return m_fTopMargin;
            }
            set
            {
                if(value != m_fTopMargin)
                    m_fTopMargin = value;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutManager"/> class.
        /// </summary>
        public LayoutManager()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutManager"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        public LayoutManager(Model model)
        {
            this.Model = model;
        }

        #endregion

        #region Class utility methods
        /// <summary>
        /// Updates the layout of the nodes in the model.
        /// </summary>
        /// <param name="contextInfo">Provides context information to help with updating the layout.</param>
        /// <returns>True if changes were made; otherwise False.</returns>
        public abstract bool UpdateLayout(object contextInfo);

        /// <summary>
        /// Called when nodes are added or removed from the model.
        /// </summary>
        /// <param name="evtArgs">Event arguments.</param>
        protected virtual void OnNodeCollectionChanged(CollectionExEventArgs evtArgs)
        {
            if (this.autoLayoutFlag && !this.UpdatingLayout && !Model.InUpdate)
            {
                LayoutModelNodes();
            }
        }

        /// <summary>
        /// Called when connection collections is changed.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.CollectionExEventArgs"/> instance containing the event data.</param>
        protected virtual void OnConnectionsChanged(CollectionExEventArgs evtArgs)
        {
            if (this.autoLayoutFlag && !this.UpdatingLayout && !Model.InUpdate)
            {
                LayoutModelNodes();
            }
        }

        /// <summary>
        /// Called after the layout has been updated by the layout manager.
        /// </summary>
        /// <param name="evtArgs">Event arguments.</param>
        protected virtual void OnLayoutUpdated(System.EventArgs evtArgs)
        {
            if (this.LayoutUpdated != null)
            {
                LayoutModelNodes();
            }
        }

        /// <summary>
        /// Called when one or more nodes in the model are has change properties.
        /// </summary>
        /// <param name="evtArgs">The event args.</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs evtArgs)
        {
            if (evtArgs.PropertyName == "Visible" && this.autoLayoutFlag && !this.UpdatingLayout
                && !this.Model.InUpdate)
            {
                LayoutModelNodes();
            }
        }

        /// <summary>
        /// Handles the ViewUpdate event of the model.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void mdl_ViewUpdate(object sender, EventArgs e)
        {
            if (autoLayoutFlag && !this.UpdatingLayout && !Model.InUpdate)
            {
                LayoutModelNodes();
            }
        }
        private void LayoutModelNodes()
        {
            if (this.Model != null && this.Model.Nodes.Count > 0)
            {
                // save previous collection
                NodeCollection nodes = m_lstNodes;
                
                // apply model collection
                m_lstNodes = this.Model.Nodes;

                this.UpdateLayout(null);
                
                // restore previous collection
                m_lstNodes = nodes;
            }
        }
        #endregion
    }
}
