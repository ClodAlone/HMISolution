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
    #region delegates
    /// <summary>
    /// Delegate declaration for cancel eventsink events.
    /// </summary>
    /// <param name="evtArgs">Event args.</param>
    public delegate void CancelCollectionChangedEventHandler(CollectionExEventArgs evtArgs);

    /// <summary>
    /// Delegate declaration for collection change events.
    /// </summary>
    /// <param name="evtArgs">Event args.</param>
    public delegate void CollectionExEventHandler(CollectionExEventArgs evtArgs);

    /// <summary>
    /// Delegate declaration for vertex changed event.
    /// </summary>
    /// <param name="evtArgs">Event args.</param>
    public delegate void VertexChangedEventHandler(VertexChangedEventArgs evtArgs);

    /// <summary>
    /// Delegate declaration for vertex changing events.
    /// </summary>
    /// <param name="evtArgs">Event args.</param>
    public delegate void VertexChangingEventHandler(VertexChangingEventArgs evtArgs);

    /// <summary>
    /// Delegate declaration for ZOrder changed event.
    /// </summary>
    /// <param name="evtArgs">Event args.</param>
    public delegate void ZOrderChangedEventHandler(ZOrderChangedEventArgs evtArgs);

    /// <summary>
    /// Delegate declaration for ZOrder changing events.
    /// </summary>
    /// <param name="evtArgs">Event args.</param>
    public delegate void ZOrderChangingEventHandler(ZOrderChangingEventArgs evtArgs);

    /// <summary>
    /// Delegate declaration for property changing events.
    /// </summary>
    /// <param name="evtArgs">Event args.</param>
    public delegate void PropertyChangingEventHandler(PropertyChangingEventArgs evtArgs);

    /// <summary>
    /// Delegate declaration for property changed event.
    /// </summary>
    /// <param name="evtArgs">Event args.</param>
    public delegate void PropertyChangedEventHandler(PropertyChangedEventArgs evtArgs);

    /// <summary>
    /// Delegate declaration for pinpoint changed event.
    /// </summary>
    /// <param name="evtArgs">Event args.</param>
    public delegate void PinPointChangedEventHandler(PinPointChangedEventArgs evtArgs);

    /// <summary>
    /// Delegate declaration for pinpoint changing events.
    /// </summary>
    /// <param name="evtArgs">Event args.</param>
    public delegate void PinPointChangingEventHandler(PinPointChangingEventArgs evtArgs);

    /// <summary>
    /// Delegate declaration for pinoffset changed event.
    /// </summary>
    /// <param name="evtArgs">Event args.</param>
    public delegate void PinOffsetChangedEventHandler(PinOffsetChangedEventArgs evtArgs);

    /// <summary>
    /// Delegate declaration for pinoffset changing events.
    /// </summary>
    /// <param name="evtArgs">Event args.</param>
    public delegate void PinOffsetChangingEventHandler(PinOffsetChangingEventArgs evtArgs);

    /// <summary>
    /// Delegate declaration for size changed event.
    /// </summary>
    /// <param name="evtArgs">Event args.</param>
    public delegate void SizeChangedEventHandler(SizeChangedEventArgs evtArgs);

    /// <summary>
    /// Delegate declaration for size changing events.
    /// </summary>
    /// <param name="evtArgs">Event args.</param>
    public delegate void SizeChangingEventHandler(SizeChangingEventArgs evtArgs);

    /// <summary>
    /// Delegate declaration for rotation changed event.
    /// </summary>
    /// <param name="evtArgs">Event args.</param>
    public delegate void RotationChangedEventHandler(RotationChangedEventArgs evtArgs);

    /// <summary>
    /// Delegate declaration for rotation changing events.
    /// </summary>
    /// <param name="evtArgs">Event args.</param>
    public delegate void RotationChangingEventHandler(RotationChangingEventArgs evtArgs);

    /// <summary>
    /// Delegate declaration for flip changed event.
    /// </summary>
    /// <param name="evtArgs">Event args.</param>
    public delegate void FlipChangedEventHandler(FlipChangedEventArgs evtArgs);

    /// <summary>
    /// Delegate declaration for flip changing events.
    /// </summary>
    /// <param name="evtArgs">Event args.</param>
    public delegate void FlipChangingEventHandler(FlipChangingEventArgs evtArgs);

    /// <summary>
    /// Delegate declaration for view origin change events.
    /// </summary>
    /// <param name="evtArgs">Event args.</param>
    public delegate void ViewOriginEventHandler(ViewOriginEventArgs evtArgs);

    /// <summary>
    /// Delegate declaration for view magnification change events.
    /// </summary>
    /// <param name="evtArgs">Event args.</param>
    public delegate void ViewMagnificationEventHandler(ViewMagnificationEventArgs evtArgs);

    /// <summary>
    /// Delegate declaration for view scroll virtual bounds change events.
    /// </summary>
    /// <param name="evtArgs">Event args.</param>
    public delegate void ViewScrollVirtualBoundsEventHandler(ViewScrollVirtualBoundsEventArgs evtArgs);

    /// <summary>
    /// Delegate declaration for node selected change events.
    /// </summary>
    /// <param name="evtArgs">Node selected Event args.</param>
    public delegate void NodeSelectedEventHandler(NodeSelectedEventArgs evtArgs);

    /// <summary>
    /// Delegate declaration for node mouse action change events.
    /// </summary>
    /// <param name="evtArgs">Event args.</param>
    public delegate void NodeMouseEventHandler(NodeMouseEventArgs evtArgs);

    /// <summary>
    /// Delegate declaration for Overview control viewport changing event handler.
    /// </summary>
    /// <param name="evtArgs">Event args.</param>
    public delegate void ViewPortBoundsChangingEventHandler(ViewPortBoundsChangingEventArgs evtArgs);

    /// <summary>
    /// Delegate declaration for Overview control viewport changed event handler.
    /// </summary>
    /// <param name="evtArgs">Event args.</param>
    public delegate void ViewPortBoundsChangedEventHandler(ViewPortBoundsChangedEventArgs evtArgs);

    // public delegate void ToolEventHandler( object sender, ToolEventArgs evtArgs );
    #endregion

    #region Offset EventArgs
    /// <summary>
    /// Class containing offset changing event args.
    /// </summary>
    public class OffsetChangingEventArgs
        : OffsetChangedEventArgs
    {
        #region Class members
        private bool m_bCancel;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="OffsetChangingEventArgs"/> class.
        /// </summary>
        /// <param name="nodeChanged">The node changed.</param>
        /// <param name="szOffset">The offset.</param>
        public OffsetChangingEventArgs(Node nodeChanged, SizeF szOffset)
            : base(nodeChanged, szOffset)
        { 
        }
        #endregion

        #region public properties
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="OffsetChangingEventArgs"/> is cancel.
        /// </summary>
        /// <value><c>true</c> if cancel; otherwise, <c>false</c>.</value>
        public bool Cancel
        {
            get { return m_bCancel; }
            set { m_bCancel = value; }
        }
        #endregion
    }

    /// <summary>
    /// Class containing offset changed event args.
    /// </summary>
    public class OffsetChangedEventArgs
        : EventArgs
    {
        #region Class members
        private INode m_nodeAffected;

        /// <summary>
        /// In pixels.
        /// </summary>
        private SizeF m_szOffset;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="OffsetChangedEventArgs"/> class.
        /// </summary>
        /// <param name="nodeChanged">The node changed.</param>
        /// <param name="szOffset">The offset.</param>
        public OffsetChangedEventArgs(INode nodeChanged, SizeF szOffset)
        {
            if (nodeChanged == null)
                throw new ArgumentNullException("nodeOffsethanged");

            m_nodeAffected = nodeChanged;
            m_szOffset = szOffset;
        }
        #endregion

        #region public properties
        /// <summary>
        /// Gets the node affected.
        /// </summary>
        /// <value>The node affected.</value>
        public INode NodeAffected
        {
            get { return m_nodeAffected; }
        }

        /// <summary>
        /// Gets the offset.
        /// </summary>
        /// <value>The offset.</value>
        public SizeF Offset
        {
            get { return m_szOffset; }
        }
        #endregion
    }
    #endregion

    #region Properties EventArgs

    /// <summary>
    /// Class containing property changing event args.
    /// </summary>
    public sealed class PropertyChangingEventArgs
        : EventArgs
    {
        #region Class members
        private IPropertyContainer m_propertyContainer;
        private object m_objNewValue;
        private string m_strPropertyName;
        private bool m_bCancel;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangingEventArgs"/> class.
        /// </summary>
        /// <param name="propertyContainer">The property container.</param>
        /// <param name="strPropertyName">Name of the property.</param>
        /// <param name="newValue">The new value.</param>
        public PropertyChangingEventArgs(IPropertyContainer propertyContainer, string strPropertyName, object newValue)
        {
            if (propertyContainer == null)
                throw new ArgumentNullException("nodeAffected");

            if (strPropertyName == null)
                throw new ArgumentNullException("strPropertyName");

            m_propertyContainer = propertyContainer;
            m_strPropertyName = strPropertyName;
            m_objNewValue = newValue;
            m_bCancel = false;
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the new value.
        /// </summary>
        /// <value>The new value.</value>
        public object NewValue
        {
            get { return m_objNewValue; }
        }

        /// <summary>
        /// Gets the property container.
        /// </summary>
        /// <value>The property container.</value>
        public IPropertyContainer PropertyContainer
        {
            get { return m_propertyContainer; }
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        public string PropertyName
        {
            get { return m_strPropertyName; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PropertyChangingEventArgs"/> is cancel.
        /// </summary>
        /// <value><c>true</c> if cancel; otherwise, <c>false</c>.</value>
        public bool Cancel
        {
            get { return m_bCancel; }
            set { m_bCancel = value; }
        }
        #endregion
    }

    /// <summary>
    /// Property changed event args.
    /// </summary>
    public sealed class PropertyChangedEventArgs
        : EventArgs
    {
        #region Class members
        private IPropertyContainer m_nodeAffected;
        private string m_strPropertyName;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangedEventArgs"/> class.
        /// </summary>
        /// <param name="propertyContainer">The property container.</param>
        /// <param name="strPropertyName">Name of the property.</param>
        public PropertyChangedEventArgs(IPropertyContainer propertyContainer, string strPropertyName)
        {
            if (propertyContainer == null)
                throw new ArgumentNullException("nodeAffected");

            if (strPropertyName == null)
                throw new ArgumentNullException("strPropertyName");

            m_nodeAffected = propertyContainer;
            m_strPropertyName = strPropertyName;
        }
        #endregion

        #region public properties
        /// <summary>
        /// Gets the node affected.
        /// </summary>
        /// <value>The node affected.</value>
        public IPropertyContainer NodeAffected
        {
            get { return m_nodeAffected; }
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        public string PropertyName
        {
            get { return m_strPropertyName; }
        }
        #endregion
    }
    #endregion

    #region Vertex EventArgs
    /// <summary>
    /// Class containing vertex changing event args.
    /// </summary>
    public class VertexChangingEventArgs
        : VertexChangedEventArgs
    {
        #region Class members
        private bool m_bCancel;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="VertexChangingEventArgs"/> class.
        /// </summary>
        /// <param name="nodeVertexContainer">The node vertex container.</param>
        /// <param name="changeType">Type of the vertex change.</param>
        /// <param name="nVertexIdx">The vertex index.</param>
        /// <param name="ptVertexLocation">The vertex location.</param>
        public VertexChangingEventArgs(PathNode nodeVertexContainer, VertexChangeType changeType, int nVertexIdx, PointF ptVertexLocation)
            : base(nodeVertexContainer, changeType, nVertexIdx, ptVertexLocation)
        { 
        }
        #endregion

        #region public properties
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="VertexChangingEventArgs"/> is cancel.
        /// </summary>
        /// <value><c>true</c> if cancel; otherwise, <c>false</c>.</value>
        public bool Cancel
        {
            get { return m_bCancel; }
            set { m_bCancel = value; }
        }
        #endregion
    }

    /// <summary>
    /// Class containing Vertex changed event args.
    /// </summary>
    public class VertexChangedEventArgs
        : EventArgs
    {
        #region Class members
        private PathNode m_nodeAffected;
        private VertexChangeType m_changeType;
        private int m_nVertexIdx;
        private PointF m_ptVertexLocation;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="VertexChangedEventArgs"/> class.
        /// </summary>
        /// <param name="nodeVertexContainer">The node vertex container.</param>
        /// <param name="changeType">Type of the vertex change.</param>
        /// <param name="nVertexIdx">The vertex index.</param>
        /// <param name="ptVertexLocation">The vertex location.</param>
        public VertexChangedEventArgs(PathNode nodeVertexContainer, VertexChangeType changeType, int nVertexIdx, PointF ptVertexLocation)
        {
            if (nodeVertexContainer == null)
                throw new ArgumentNullException("nodeAffected");

            if (nVertexIdx < 0)
                throw new ArgumentOutOfRangeException("nVertexIdx");

            m_nodeAffected = nodeVertexContainer;
            m_ptVertexLocation = ptVertexLocation;
            m_changeType = changeType;
            m_nVertexIdx = nVertexIdx;
        }
        #endregion

        #region public properties
        /// <summary>
        /// Gets the node affected.
        /// </summary>
        /// <value>The node affected.</value>
        public PathNode NodeAffected
        {
            get { return m_nodeAffected; }
        }

        /// <summary>
        /// Gets the index of the vertex.
        /// </summary>
        /// <value>The index of the vertex.</value>
        public int VertexIndex
        {
            get { return m_nVertexIdx; }
        }

        /// <summary>
        /// Gets the vertex location.
        /// </summary>
        /// <value>The vertex location.</value>
        public PointF VertexLocation
        {
            get { return m_ptVertexLocation; }
        }

        /// <summary>
        /// Gets the type of the change.
        /// </summary>
        /// <value>The type of the change.</value>
        public VertexChangeType ChangeType
        {
            get { return m_changeType; }
        }
        #endregion
    }

    #endregion

    #region ZOrder EventArgs
    /// <summary>
    /// Class containing Z order changing event args.
    /// </summary>
    public class ZOrderChangingEventArgs
        : ZOrderChangedEventArgs
    {
        #region Class members
        private bool m_bCancel;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="ZOrderChangingEventArgs"/> class.
        /// </summary>
        /// <param name="nodeChanged">The node changed.</param>
        /// <param name="changeType">Type of the change.</param>
        /// <param name="nNewZOrder">The new Z order value.</param>
        public ZOrderChangingEventArgs(Node nodeChanged, ZOrderUpdate changeType, int nNewZOrder)
            : base(nodeChanged, changeType, nNewZOrder)
        { 
        }
        #endregion

        #region public properties
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ZOrderChangingEventArgs"/> is cancel.
        /// </summary>
        /// <value><c>true</c> if cancel; otherwise, <c>false</c>.</value>
        public bool Cancel
        {
            get { return m_bCancel; }
            set { m_bCancel = value; }
        }
        #endregion
    }

    /// <summary>
    /// Class containing Z order changed event args.
    /// </summary>
    public class ZOrderChangedEventArgs
        : EventArgs
    {
        #region Class members
        private Node m_nodeAffected;
        private ZOrderUpdate m_changeType;
        private int m_nNewZOrder;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="ZOrderChangedEventArgs"/> class.
        /// </summary>
        /// <param name="nodeChanged">The node changed.</param>
        /// <param name="changeType">Type of the change.</param>
        /// <param name="nNewZOrder">The new Z order value.</param>
        public ZOrderChangedEventArgs(Node nodeChanged, ZOrderUpdate changeType, int nNewZOrder)
        {
            if (nodeChanged == null)
                throw new ArgumentNullException("nodeChanged");

            m_nodeAffected = nodeChanged;
            m_changeType = changeType;
            m_nNewZOrder = nNewZOrder;
        }
        #endregion

        #region public properties
        /// <summary>
        /// Gets the node affected.
        /// </summary>
        /// <value>The node affected.</value>
        public Node NodeAffected
        {
            get { return m_nodeAffected; }
        }

        /// <summary>
        /// Gets the type of the change.
        /// </summary>
        /// <value>The type of the change.</value>
        public ZOrderUpdate ChangeType
        {
            get { return m_changeType; }
        }

        /// <summary>
        /// Gets the Z order.
        /// </summary>
        /// <value>The Z order.</value>
        public int ZOrder
        {
            get { return m_nNewZOrder; }
        }
        #endregion
    }
    #endregion

    #region Size EventArgs
    /// <summary>
    /// Class containing size changing event args.
    /// </summary>
    public class SizeChangingEventArgs
        : OffsetChangingEventArgs
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="SizeChangingEventArgs"/> class.
        /// </summary>
        /// <param name="nodeChanged">The node changed.</param>
        /// <param name="szOffset">The offset.</param>
        public SizeChangingEventArgs(Node nodeChanged, SizeF szOffset)
            : base(nodeChanged, szOffset)
        { 
        }
        #endregion
    }

    /// <summary>
    /// Class containing size changed event args.
    /// </summary>
    public class SizeChangedEventArgs
        : OffsetChangedEventArgs
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="SizeChangedEventArgs"/> class.
        /// </summary>
        /// <param name="nodeChanged">The node changed.</param>
        /// <param name="szOffset">The offset.</param>
        public SizeChangedEventArgs(INode nodeChanged, SizeF szOffset)
            : base(nodeChanged, szOffset)
        {
        }
        #endregion
    }
    #endregion

    #region PinPoint Event Args
    /// <summary>
    /// Class containing pinpoint changing event args.
    /// </summary>
    public class PinPointChangingEventArgs
        : OffsetChangingEventArgs
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="PinPointChangingEventArgs"/> class.
        /// </summary>
        /// <param name="nodeChanged">The node changed.</param>
        /// <param name="szOffset">The offset.</param>
        public PinPointChangingEventArgs(Node nodeChanged, SizeF szOffset)
            : base(nodeChanged, szOffset)
        { 
        }
        #endregion
    }

    /// <summary>
    /// Class containing pinpoint changed event args.
    /// </summary>
    public class PinPointChangedEventArgs
        : OffsetChangedEventArgs
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="PinPointChangedEventArgs"/> class.
        /// </summary>
        /// <param name="nodeChanged">The node changed.</param>
        /// <param name="szOffset">The sz offset.</param>
        public PinPointChangedEventArgs(Node nodeChanged, SizeF szOffset)
            : base(nodeChanged, szOffset)
        {
        }
        #endregion
    }
    #endregion

    #region Pin Offset Event Args
    /// <summary>
    /// Class containing pin offset changing event args.
    /// </summary>
    public class PinOffsetChangingEventArgs
        : OffsetChangingEventArgs
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="PinOffsetChangingEventArgs"/> class.
        /// </summary>
        /// <param name="nodeChanged">The node changed.</param>
        /// <param name="szOffset">The offset.</param>
        public PinOffsetChangingEventArgs(Node nodeChanged, SizeF szOffset)
            : base(nodeChanged, szOffset)
        { 
        }
        #endregion
    }

    /// <summary>
    /// Class containing pin offset changed event args.
    /// </summary>
    public class PinOffsetChangedEventArgs
        : OffsetChangedEventArgs
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="PinOffsetChangedEventArgs"/> class.
        /// </summary>
        /// <param name="nodeChanged">The node changed.</param>
        /// <param name="szOffset">The sz offset.</param>
        public PinOffsetChangedEventArgs(Node nodeChanged, SizeF szOffset)
            : base(nodeChanged, szOffset)
        {
        }
        #endregion
    }
    #endregion

    #region Rotation Event Args
    /// <summary>
    /// Class containing rotation changing changed event args.
    /// </summary>
    public class RotationChangingEventArgs
        : RotationChangedEventArgs
    {
        #region Class members
        private bool m_bCancel;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="RotationChangingEventArgs"/> class.
        /// </summary>
        /// <param name="nodeChanged">The node changed.</param>
        /// <param name="fRotationOffset">The f rotation offset.</param>
        public RotationChangingEventArgs(Node nodeChanged, float fRotationOffset)
            : base(nodeChanged, fRotationOffset)
        { 
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RotationChangingEventArgs"/> is cancel.
        /// </summary>
        /// <value><c>true</c> if cancel; otherwise, <c>false</c>.</value>
        public bool Cancel
        {
            get { return m_bCancel; }
            set { m_bCancel = value; }
        }
        #endregion
    }

    /// <summary>
    /// Class containing rotation changed event args.
    /// </summary>
    public class RotationChangedEventArgs
        : EventArgs
    {
        #region Class members
        private Node m_nodeAffected;

        /// <summary>
        /// In pixels.
        /// </summary>
        private float m_fRotationOffset;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="RotationChangedEventArgs"/> class.
        /// </summary>
        /// <param name="nodeChanged">The node changed.</param>
        /// <param name="fRotationOffset">The f rotation offset.</param>
        public RotationChangedEventArgs(Node nodeChanged, float fRotationOffset)
        {
            if (nodeChanged == null)
                throw new ArgumentNullException("nodeLocationChanged");

            m_nodeAffected = nodeChanged;
            m_fRotationOffset = fRotationOffset;
        }
        #endregion

        #region public properties
        /// <summary>
        /// Gets the node affected.
        /// </summary>
        /// <value>The node affected.</value>
        public Node NodeAffected
        {
            get { return m_nodeAffected; }
        }

        /// <summary>
        /// Gets the rotation offset.
        /// </summary>
        /// <value>The rotation offset.</value>
        public float RotationOffset
        {
            get { return m_fRotationOffset; }
        }
        #endregion
    }
    #endregion

    #region Flip Event Args
    /// <summary>
    /// Class containing flip changing event args.
    /// </summary>
    public class FlipChangingEventArgs
        : FlipChangedEventArgs
    {
        #region Class members
        /// <summary>
        /// Flag to cancel the flipping.
        /// </summary>
        private bool m_bCancel;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="FlipChangingEventArgs"/> class.
        /// </summary>
        /// <param name="nodeChanged">The node changed.</param>
        /// <param name="flipAxis">The flip axis.</param>
        /// <param name="bFlipValue">if set to <c>true</c> [b flip value].</param>
        public FlipChangingEventArgs(Node nodeChanged, FlipAxis flipAxis, bool bFlipValue)
            : base(nodeChanged, flipAxis, bFlipValue)
        { 
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FlipChangingEventArgs"/> is cancel.
        /// </summary>
        /// <value><c>true</c> if cancel; otherwise, <c>false</c>.</value>
        public bool Cancel
        {
            get { return m_bCancel; }
            set { m_bCancel = value; }
        }
        #endregion
    }

    /// <summary>
    /// Class containing flip changed event args.
    /// </summary>
    public class FlipChangedEventArgs
        : EventArgs
    {
        #region Class members
        private Node m_nodeAffected;
        private bool m_bFlipValue;

        /// <summary>
        /// Indicates whether node is flipped relative to X or Y axis.
        /// True - Y axis, False - X axis
        /// </summary>
        private FlipAxis m_flipAxis;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="FlipChangedEventArgs"/> class.
        /// </summary>
        /// <param name="nodeChanged">The node changed.</param>
        /// <param name="flipAxis">The flip axis.</param>
        /// <param name="bFlipValue">if set to <c>true</c> [b flip value].</param>
        public FlipChangedEventArgs(Node nodeChanged, FlipAxis flipAxis, bool bFlipValue)
        {
            if (nodeChanged == null)
                throw new ArgumentNullException("nodeLocationChanged");

            m_nodeAffected = nodeChanged;
            m_bFlipValue = bFlipValue;
            m_flipAxis = flipAxis;
        }
        #endregion

        #region public properties
        /// <summary>
        /// Gets the node affected.
        /// </summary>
        /// <value>The node affected.</value>
        public Node NodeAffected
        {
            get { return m_nodeAffected; }
        }

        /// <summary>
        /// Gets a value indicating whether value is flipped.
        /// </summary>
        /// <value><c>true</c> if value is flipped; otherwise, <c>false</c>.</value>
        public bool FlipValue
        {
            get { return m_bFlipValue; }
        }

        /// <summary>
        /// Gets the flip axis.
        /// </summary>
        /// <value>The flip axis.</value>
        public FlipAxis FlipAxis
        {
            get { return m_flipAxis; }
        }
        #endregion
    }
    #endregion

    #region Origin changed EventArgs
    /// <summary>
    /// Encapsulates arguments for the origin change event of a view.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The origin of a view is the point in world space that maps to the upper-left
    /// hand corner of the view.
    /// </para>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.View"/>
    /// </remarks>
    public class ViewOriginEventArgs
        : EventArgs
    {
        #region Class members
        private PointF m_ptOrig;
        private PointF m_ptNew;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewOriginEventArgs"/> class.
        /// </summary>
        /// <param name="ptOrig">Original origin.</param>
        /// <param name="ptNew">New origin.</param>
        public ViewOriginEventArgs(PointF ptOrig, PointF ptNew)
        {
            m_ptOrig = ptOrig;
            m_ptNew = ptNew;
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets origin value before the event occurred.
        /// </summary>
        public PointF OriginalOrigin
        {
            get { return m_ptOrig; }
        }

        /// <summary>
        /// Gets origin value after the event occurred.
        /// </summary>
        public PointF NewOrigin
        {
            get { return m_ptNew; }
        }

        /// <summary>
        /// Gets difference between the new origin and the original origin.
        /// </summary>
        public SizeF Offset
        {
            get { return new SizeF(this.NewOrigin.X - this.OriginalOrigin.X, this.NewOrigin.Y - this.OriginalOrigin.Y); }
        }
        #endregion
    }
    #endregion

    #region Magnification changed EventArgs
    /// <summary>
    /// Encapsulates arguments for the Magnification change event of a view.
    /// </summary>
    /// <remarks>
    /// <para>
    /// </para>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.View"/>
    /// </remarks>
    public class ViewMagnificationEventArgs
        : EventArgs
    {
        #region Class members
        private float m_mgOrig;
        private float m_mgNew;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewMagnificationEventArgs"/> class.
        /// </summary>
        /// <param name="mgOrig">Original origin.</param>
        /// <param name="mgNew">New origin.</param>
        public ViewMagnificationEventArgs(float mgOrig, float mgNew)
        {
            m_mgOrig = mgOrig;
            m_mgNew = mgNew;
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets magnification value before the event occurred.
        /// </summary>
        public float OriginalMagnification
        {
            get { return m_mgOrig; }
        }

        /// <summary>
        /// Gets magnification value after the event occurred.
        /// </summary>
        public float NewMagnification
        {
            get { return m_mgNew; }
        }
        #endregion
    }
    #endregion

    #region ScrollVirtualBounds changed EventArgs
    /// <summary>
    /// Encapsulates arguments for the Scroll Virtual Bounds change event of a view.
    /// </summary>
    /// <remarks>
    /// <para>
    /// </para>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.View"/>
    /// </remarks>
    public class ViewScrollVirtualBoundsEventArgs
        : EventArgs
    {
        #region Class members
        private RectangleF m_rcOldBounds;
        private RectangleF m_rcNewBounds;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewScrollVirtualBoundsEventArgs"/> class.
        /// </summary>
        /// <param name="rcOldBounds">The old bounds.</param>
        /// <param name="rcNewBounds">The new bounds.</param>
        public ViewScrollVirtualBoundsEventArgs(RectangleF rcOldBounds, RectangleF rcNewBounds)
        {
            m_rcOldBounds = rcOldBounds;
            m_rcNewBounds = rcNewBounds;
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets scroll Bounds value before the event occurred.
        /// </summary>
        public RectangleF OldBounds
        {
            get { return m_rcOldBounds; }
        }

        /// <summary>
        /// Gets scroll Bounds value after the event occurred.
        /// </summary>
        public RectangleF NewBounds
        {
            get { return m_rcNewBounds; }
        }
        #endregion
    }
    #endregion

    #region NodeSelected changed EventArgs
    /// <summary>
    /// Encapsulates arguments for the SelectionList change event of a controller.
    /// </summary>
    /// <remarks>
    /// <para>
    /// </para>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Controller"/>
    /// </remarks>
    public class NodeSelectedEventArgs
        : EventArgs
    {
        #region Class members
        private readonly Node m_node;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the node instance.
        /// </summary>
        /// <value>The node.</value>
        public Node Node
        {
            get { return m_node; }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeSelectedEventArgs"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        public NodeSelectedEventArgs(Node node)
        {
            m_node = node;
        }
        #endregion
    }
    #endregion

    #region NodeMouse changed EventArgs
    /// <summary>
    /// Encapsulates arguments for the mouse actions events of a diagram.
    /// </summary>
    /// <remarks>
    /// <para>
    /// </para>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Controller"/>
    /// </remarks>
    public class NodeMouseEventArgs
        : EventArgs
    {
        #region Class members
        private readonly Node m_node;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the node instance.
        /// </summary>
        /// <value>The node.</value>
        public Node Node
        {
            get { return m_node; }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeMouseEventArgs"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        public NodeMouseEventArgs(Node node)
        {
            m_node = node;
        }
        #endregion
    }
    #endregion

    #region ViewPortBounds changed event args

    /// <summary>
    /// Class containing viewport changed changed event args.
    /// </summary>
    public class ViewPortBoundsChangedEventArgs : EventArgs
    {
        #region Class Members
        private RectangleF m_bounds;
        #endregion

        #region Class Properties
        /// <summary>
        /// Gets the ViewPortBounds
        /// </summary>
        public RectangleF ViewPortBounds
        {
            get
            {
                return m_bounds;
            }
        }
        #endregion

        #region Class initialize/finalize methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewPortBoundsChangedEventArgs"/> class.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        public ViewPortBoundsChangedEventArgs(RectangleF bounds)
        {
            m_bounds = bounds;
        }
        #endregion
    }

    #endregion

    #region ViewPortBounds changing event args

    /// <summary>
    /// Class containing view port bounds changing event args.
    /// </summary>
    public class ViewPortBoundsChangingEventArgs : EventArgs
    {
        #region Class Members

        private bool m_cancel;
        private RectangleF m_bounds;
        private RectangleF m_oldBounds;

        #endregion

        #region Class Properties
        /// <summary>
        /// Gets the new ViewPortBounds.
        /// </summary>
        public RectangleF NewViewPortBounds
        {
            get
            {
                return m_bounds;
            }
        }

        /// <summary>
        /// Gets the Old ViewPortBounds.
        /// </summary>
        public RectangleF OldViewPortBounds
        {
            get
            {
                return m_oldBounds;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this is cancel.
        /// </summary>
        public bool Cancel
        {
            get
            {
                return m_cancel;
            }
            set
            {
                m_cancel = value;
            }
        }

        #endregion

        #region  Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewPortBoundsChangingEventArgs"/> class.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <param name="oldBounds">The old bounds.</param>
        public ViewPortBoundsChangingEventArgs(RectangleF bounds, RectangleF oldBounds)
        {
            m_bounds = bounds;
            m_oldBounds = oldBounds;
        }
        #endregion
    }

    #endregion
}
