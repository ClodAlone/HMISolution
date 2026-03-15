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

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Base shape class to visualize closed path and fill it using the FillStyle property.
    /// </summary>
    [Serializable]
    public class FilledPath
        : PathNode
    {
        #region Class members
        /// <summary>
        /// Fill Style
        /// </summary>
        private FillStyle m_styleFill;

        /// <summary>
        /// Flag to enable shading.
        /// </summary>
        protected bool m_benableshading = false;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="FilledPath"/> class.
        /// </summary>
        public FilledPath()
            : base()
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilledPath"/> class.
        /// </summary>
        /// <param name="path">The graphics path.</param>
        public FilledPath(GraphicsPath path)
        {
            Initialize(path);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilledPath"/> class.
        /// </summary>
        /// <param name="src">The source instance.</param>
        public FilledPath(FilledPath src)
            : base(src)
        {
            m_styleFill = (FillStyle)src.FillStyle.Clone();
            m_benableshading = src.EnableShading;
            UpdateBoundingRectangle();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilledPath"/> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The serialization context.</param>
        protected FilledPath(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "enableshading":
                        m_benableshading = info.GetBoolean("enableshading");
                        break;
                    case "fillStyle":
                        m_styleFill = (FillStyle)info.GetValue("fillStyle", typeof(FillStyle));
                        this.FillStyle.UpdateServiceReferences(this);
                        break;
                }
            }
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the properties used to fill the interior of regions.
        /// </summary>
        /// <value>The fill style.</value>
        /// <remarks>
        /// <para>
        /// The fill style is used to create brushes for painting interior regions of
        /// the shape.
        /// </para>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.FillStyle"/>
        /// </remarks>
        [Browsable(true)]
        [TypeConverter(typeof(FillStyleConverter))]
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Properties of the brush used to fill interior regions.")]
        public FillStyle FillStyle
        {
            get
            {
                if (m_styleFill == null)
                    m_styleFill = new FillStyle();

                return m_styleFill;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether shading should be enabled.
        /// </summary>
        /// <value><c>true</c> if enable shading; otherwise, <c>false</c>.</value>
        [
        Browsable(true),
        Category("Appearance"),
        Description("Enable shading."),
        DefaultValue(false)
        ]
        public bool EnableShading
        {
            get
            {
                return m_benableshading;
            }
            set
            {
                if (OnPropertyChanging(this.FullContainerName, DPN.EnableShading, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.EnableShading);

                    // assign new value
                    m_benableshading = value;
                    
                    // raise property changed event
                    OnPropertyChanged(this.FullContainerName, DPN.EnableShading);
                }
            }
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            return new FilledPath(this);
        }

        /// <summary>
        /// Renders shapes visual representation on given graphics.
        /// </summary>
        /// <param name="gfx">Graphics to draw on</param>
        protected override void Render(Graphics gfx)
        {
            // 1 - call base method impementation
            base.Render(gfx);

            // 2 - Draw interior
            DrawInterior(gfx);

            // 3 - Draw border
            DrawBorder(gfx);
        }

        /// <summary>
        /// Draws shape's border on given graphics.
        /// </summary>
        /// <param name="gfx">Graphics to draw on</param>
        protected virtual void DrawBorder(Graphics gfx)
        {
            if (this.LineStyle.LineWidth > 0)
            {
                using (Pen pen = this.LineStyle.CreatePen())
                {
                    try
                    {
                        gfx.DrawPath(pen, this.GraphicsPath);
                    }
                    catch(Exception)
                    {
                        this.LineStyle.DashPattern = null;
                    }
                }
            }
        }

        /// <summary>
        /// Draws shape's interior on given graphics.
        /// </summary>
        /// <param name="gfx">Graphics to draw on</param>
        protected virtual void DrawInterior(Graphics gfx)
        {
            GraphicsPath path = this.GraphicsPath;
            RectangleF fillBounds = path.GetBounds();
            if (!(this.FillStyle.Color == Color.Transparent) || this.FillStyle.Type == FillStyleType.Texture)
            {
                using (Brush brushFill = this.FillStyle.CreateBrush(gfx, fillBounds))
                    gfx.FillPath(brushFill, path);
            }
            if (this.EnableShading)
            {
                DrawShading(gfx);
            }
        }

        /// <summary>
        /// Draw the shading over node.
        /// </summary>
        /// <param name="gfx">The Graphics to draw on.</param>
        protected virtual void DrawShading(Graphics gfx)
        {
            GraphicsState st = gfx.Save();
            Region clp = new Region(this.GraphicsPath);
            gfx.Clip = clp;
            gfx.SmoothingMode = SmoothingMode.AntiAlias;
            RectangleF r = this.GraphicsPath.GetBounds();
            Color color = this.FillStyle.Color;
            int opacity = 147;
            int red = (color.R * opacity / 255) + 255 * (255 - opacity) / 255;
            int green = (color.G * opacity / 255) + 255 * (255 - opacity) / 255;
            int blue = (color.B * opacity / 255) + 255 * (255 - opacity) / 255;
            if (red == 255 && green == 255)
            {
                red -= 15;
                green -= 15;
            }
            Color newColor = Color.FromArgb(opacity, red, green, blue);

            GraphicsPath path = new GraphicsPath();
            path.AddLine(r.X, r.Y, r.X + r.Width, r.Y);
            path.AddLine(r.X + r.Width, r.Y, r.X + r.Width, r.Y + 5 + r.Height / 4);
            path.AddArc(new RectangleF(r.X, r.Y, r.Width, r.Height / 3), 0, 180);
            path.CloseFigure();

            SolidBrush brush = new SolidBrush(newColor);
            gfx.DrawPath(new Pen(newColor), path);
            gfx.FillPath(brush, path);

            RectangleF bounds = path.GetBounds();
            path.Reset();

            RectangleF ellipseRect = new RectangleF(r.X, r.Height * .6f, r.Width, r.Height - r.Height * .6f);
            path.AddEllipse(ellipseRect);

            PathGradientBrush pathBrush = new PathGradientBrush(path);
            pathBrush.CenterColor = Color.FromArgb(180, Color.White);
            pathBrush.SurroundColors = new Color[] { Color.Transparent };
            
            // pathBrush.CenterPoint = new PointF(path.GetBounds().Width/2, path.GetBounds().Height*.4f);
            gfx.FillPath(pathBrush, path);
            gfx.Restore(st);
        }

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        protected override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("fillStyle", m_styleFill);
            info.AddValue("enableshading", m_benableshading);
        }

        /// <summary>
        /// Updates the references from service provider.
        /// </summary>
        /// <param name="provider">The service provider.</param>
        public override void UpdateReferences(IServiceReferenceProvider provider)
        {
            base.UpdateReferences(provider);

            this.FillStyle.UpdateServiceReferences(provider);
            //if (provider == null)
            //    this.m_styleFill = null;
        }

        /// <summary>
        /// Gets the property container.
        /// </summary>
        /// <param name="strPropertyContainerName">Name of the property container.</param>
        /// <returns>The property container object.</returns>
        protected override object GetPropertyContainer(string strPropertyContainerName)
        {
            object objToReturn = base.GetPropertyContainer(strPropertyContainerName);

            if (objToReturn == null)
            {
                if (strPropertyContainerName == DPN.FillStyle)
                    objToReturn = this.FillStyle;
            }

            return objToReturn;
        }
        #endregion

        #region Class helper methods
        private void Initialize(GraphicsPath path)
        {
            RectangleF rectBounds = path.GetBounds();
            path = UpdateGraphicsPath(path);

            m_bIsVertexEditable = IsPathVertexEditable(path);
            m_bCanChangePath = true;
            //// assign new GraphicsPath
            m_gpPath = path;
            //// Set path point.
            this.PathPoints = (PointF[])this.GraphicsPath.PathPoints.Clone();
            //// calc pin offset
            SizeF szPinOffsetUnitIndependent = new SizeF(rectBounds.Width / 2, rectBounds.Height / 2);
            //// assign default PinLocation
            PointF ptPinPointUnitIndependent = new PointF(
                rectBounds.Location.X + szPinOffsetUnitIndependent.Width,
                rectBounds.Location.Y + szPinOffsetUnitIndependent.Height);
            //// assign node size value
            SizeF szSizeUnitIndependent = rectBounds.Size;
            //// Init BoundsInfo
            CreateBoundsInfo(ptPinPointUnitIndependent, szPinOffsetUnitIndependent, szSizeUnitIndependent);

            // update bounding rect
            UpdateBoundingRectangle();
        }
        private GraphicsPath UpdateGraphicsPath(GraphicsPath path)
        {
            // get bounding rect
            RectangleF rectBounds = path.GetBounds();
            PointF[] pts = (PointF[])path.PathPoints.Clone();
            Geometry.TranslateToOrigin(pts, rectBounds.Location);

            // assign new GraphicsPath
            // get polygon's bounding rectangle
            return new GraphicsPath(pts, path.PathTypes);
        }
        #endregion
    }
}
