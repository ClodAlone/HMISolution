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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using Syncfusion.Windows.Forms.Diagram;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Base class for node's primitives like ControlPoint, EndPoint, ConnectionPoint etc.
    /// </summary>
    public abstract class AnchoringPrimitive
        : PropertyContainer
    {
        #region Class members
        private int m_nID;
        private Position m_position;
        private Node m_container;
        private float m_fOffsetX;
        private float m_fOffsetY;

        /// <summary>
        /// Delegate used to update container node's refresh rect.
        /// </summary>
        private UpdateCallback m_delUpdCallback;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="AnchoringPrimitive"/> class.
        /// </summary>
        public AnchoringPrimitive()
        {
            m_position = Position.Center;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnchoringPrimitive"/> class.
        /// </summary>
        /// <param name="container">The primitive container node.</param>
        public AnchoringPrimitive(Node container)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            m_container = container;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnchoringPrimitive"/> class.
        /// </summary>
        /// <param name="container">The primitive container node.</param>
        /// <param name="position">The primitive position relative to its container.</param>
        public AnchoringPrimitive(Node container, Position position)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            m_container = container;
            m_position = position;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnchoringPrimitive"/> class.
        /// </summary>
        /// <param name="src">The source instance.</param>
        public AnchoringPrimitive(AnchoringPrimitive src)
            : base(src)
        {
            m_position = src.m_position;
            m_fOffsetX = src.m_fOffsetX;
            m_fOffsetY = src.m_fOffsetY;
            m_nID = src.m_nID;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnchoringPrimitive"/> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The serialization context.</param>
        public AnchoringPrimitive(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "position":
                        m_position = (Position)entry.Value;
                        break;
                    case "offsetX":
                        m_fOffsetX = (float)info.GetValue("offsetX", typeof(float));
                        break;
                    case "offsetY":
                        m_fOffsetY = (float)info.GetValue("offsetY", typeof(float));
                        break;
                    case "ID":
                        m_nID = (int)info.GetValue("ID", typeof(int));
                        break;
                    case "container":
                        m_container = (Node)entry.Value;
                        break;
                }
            }
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the position of primitive in local coordinates.
        /// </summary>
        /// <value>The position relative to its container.</value>
        [Browsable(true)]
        [DefaultValue(Diagram.Position.Center)]
        [Description("Defines anchoring proimitive position relative to its container.")]
        public Position Position
        {
            get 
            { 
                return m_position; 
            }
            set
            {
                if (m_position != value && OnPropertyChanging(DPN.Position, value))
                {
                    RecordPropertyChanged(DPN.Position);

                    PointF[] ptsPoints = new PointF[2];

                    if (this.Container != null)
                        ptsPoints[0] = GetPosition();

                    m_position = value;

                    if (this.Container != null)
                    {
                        ptsPoints[1] = GetPosition();
                        Matrix mtxTransform = this.Container.GetLocalTransformations();
                        this.Container.AppendLocalFlipTransforms(mtxTransform);

                        mtxTransform.TransformPoints(ptsPoints);
                    }

                    // call position change   method
                    PositionChange(ptsPoints[1].X - ptsPoints[0].X, ptsPoints[1].Y - ptsPoints[0].Y);

                    OnPropertyChanged(DPN.Position);
                }
            }
        }

        /// <summary>
        /// Gets or sets the container node of primitive.
        /// </summary>
        /// <value>The container.</value>
        [Browsable(false)]
        public Node Container
        {
            get 
            { 
                return m_container; 
            }
            set
            {
                if (m_container != value)
                    m_container = value;

                if (m_container != null)
                {
                    m_delUpdCallback = (UpdateCallback)Delegate.CreateDelegate(typeof(UpdateCallback), m_container, "UpdateRefreshRect");
                }
                else
                {
                    m_delUpdCallback = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets AnchoringPrimitive offset in percents
        /// relative to its container's width from container's top left point.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(0f)]
        [Description("Specifies anchiring primitive position relative to its container's rendering origin across X axis.")]
        public float OffsetX
        {
            get 
            { 
                return m_fOffsetX; 
            }
            set
            {
                if (m_fOffsetX != value && OnPropertyChanging(DPN.OffsetX, value))
                {
                    PointF[] ptsPoints = new PointF[2];
                    ptsPoints[0] = GetPosition();

                    this.Position = Position.Custom;
                    m_fOffsetX = value;

                    ptsPoints[1] = GetPosition();

                    if (this.Container != null)
                    {
                        Matrix mtxTransform = this.Container.GetLocalTransformations();
                        this.Container.AppendLocalFlipTransforms(mtxTransform);

                        mtxTransform.TransformPoints(ptsPoints);
                    }

                    // call position change   method
                    PositionChange(ptsPoints[1].X - ptsPoints[0].X, ptsPoints[1].Y - ptsPoints[0].Y);

                    OnPropertyChanged(DPN.OffsetY);
                }
            }
        }

        /// <summary>
        /// Gets or sets AnchoringPrimitive offset in percents
        /// relative to its container's height from container's top left point.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(0f)]
        [Description("Specifies anchiring primitive position relative to its container's rendering origin across X axis.")]
        public float OffsetY
        {
            get 
            { 
                return m_fOffsetY; 
            }
            set
            {
                if (m_fOffsetY != value && OnPropertyChanging(DPN.OffsetY, value))
                {
                    PointF[] ptsPoints = new PointF[2];
                    ptsPoints[0] = GetPosition();

                    this.Position = Position.Custom;
                    m_fOffsetY = value;

                    ptsPoints[1] = GetPosition();

                    if (this.Container != null)
                    {
                        Matrix mtxTransform = this.Container.GetLocalTransformations();
                        this.Container.AppendLocalFlipTransforms(mtxTransform);

                        mtxTransform.TransformPoints(ptsPoints);
                    }

                    // call position change   method
                    PositionChange(ptsPoints[1].X - ptsPoints[0].X, ptsPoints[1].Y - ptsPoints[0].Y);

                    OnPropertyChanged(DPN.OffsetY);
                }
            }
        }

        /// <summary>
        /// Gets or sets the unique AnchoringPrimitive ID.
        /// </summary>
        /// <value>The unique ID.</value>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Gets the primitive object position in local coordinates.
        /// </summary>
        /// <returns>Position of primitive in local coordinates.</returns>
        public virtual PointF GetPosition()
        {
            PointF ptLocationToReturn = PointF.Empty;
            SizeF szContainerSize = SizeF.Empty;
            if (m_container != null)
                szContainerSize = ((IUnitIndependent)m_container).GetSize(MeasureUnits.Pixel);
            PointF ptContainerCenter = new PointF(szContainerSize.Width / 2, szContainerSize.Height / 2);

            switch (m_position)
            {
                case Position.Center:
                    ptLocationToReturn.X = ptContainerCenter.X;
                    ptLocationToReturn.Y = ptContainerCenter.Y;
                    break;
                case Position.Custom:
                    ptLocationToReturn.X = this.OffsetX;
                    ptLocationToReturn.Y = this.OffsetY;
                    break;
                case Position.TopCenter:
                    ptLocationToReturn.X = ptContainerCenter.X;
                    break;
                case Position.TopRight:
                    ptLocationToReturn.X = szContainerSize.Width;
                    break;
                case Position.MiddleLeft:
                    ptLocationToReturn.Y = ptContainerCenter.Y;
                    break;
                case Position.MiddleRight:
                    ptLocationToReturn.X = szContainerSize.Width;
                    ptLocationToReturn.Y = ptContainerCenter.Y;
                    break;
                case Position.BottomLeft:
                    ptLocationToReturn.Y = szContainerSize.Height;
                    break;
                case Position.BottomCenter:
                    ptLocationToReturn.X = ptContainerCenter.X;
                    ptLocationToReturn.Y = szContainerSize.Height;
                    break;
                case Position.BottomRight:
                    ptLocationToReturn.X = szContainerSize.Width;
                    ptLocationToReturn.Y = szContainerSize.Height;
                    break;
            }

            return ptLocationToReturn;
        }

        /// <summary>
        /// Draws primitive to the specified graphics.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        /// <remarks>
        /// Call the abstract method - Render(Graphics gfx).
        /// </remarks>
        public void Draw(Graphics gfx)
        {
            // render primitive 
            Render(gfx);
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Calling when primitive position is changed.
        /// </summary>
        /// <param name="fOffsetX">The offset by X axis.</param>
        /// <param name="fOffsetY">The offset by Y axis.</param>
        protected virtual void PositionChange(float fOffsetX, float fOffsetY)
        {
            InvokeUpdateCallback();
        }

        /// <summary>
        /// Records the property changed.
        /// </summary>
        /// <param name="strPropertyName">Name of the changed property.</param>
        protected override void RecordPropertyChanged(string strPropertyName)
        {
            if (this.HistoryService != null)
            {
                this.HistoryService.RecordPropertyChanged(this, this.FullContainerName, strPropertyName);
            }
        }

        /// <summary>
        /// Renders the primitive object to specified graphics.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        protected abstract void Render(Graphics gfx);

        /// <summary>
        /// Gets the object data to serialize instance.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The serialization context.</param>
        protected override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("position", m_position);
            info.AddValue("offsetX", m_fOffsetX);
            info.AddValue("offsetY", m_fOffsetY);
            info.AddValue("ID", m_nID);
            info.AddValue("container", m_container);
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Invokes the update refresh rectangle callback. Used for update refresh rectangle 
        /// for container by calling prospected non-parameter method UpdateRefreshRect().
        /// </summary>
        protected void InvokeUpdateCallback()
        {
            // tell container node to update its refresh rect
            if (m_delUpdCallback != null)
            {
                m_delUpdCallback();
            }
        }
        #endregion
    }
}
