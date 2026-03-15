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
using System.Drawing.Imaging;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Base class for objects that draw a grid onto a view.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Derived classes must override the
    /// <see cref="Syncfusion.Windows.Forms.Diagram.LayoutGrid.Draw"/>
    /// method in order to draw the grid.
    /// </para>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.View"/>
    /// </remarks>
    [Serializable]
    [TypeConverter(typeof(LayoutGridConverter))]
    public class LayoutGrid
        : PropertyContainer
    {
        #region Class members
        /// <summary>
        /// View this grid renders to.
        /// </summary>
        private View m_viewer;
        private GridStyle m_gridStyle;
        private float m_fVerticalSpacing;
        private float m_fHorizontalSpacing;
        private float m_fUpdateHSpacing = float.MinValue;
        private float m_nMinPixelSpacing;
        private bool m_bVisible;
        private bool m_bSnapToGrid;
        private Color m_clrColor;
        private DashStyle m_dashStyle;
        private float m_fDashOffset;
        private RenderingStyle m_styleRendering;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutGrid"/> class.
        /// </summary>
        /// <param name="containerView">View in which to draw the grid.</param>
        public LayoutGrid(View containerView)
        {
            m_viewer = containerView;
            m_fVerticalSpacing = 10;
            m_fHorizontalSpacing = 10;
            m_nMinPixelSpacing = 4;
            m_bVisible = true;
            m_bSnapToGrid = true;
            m_clrColor = CommonUsedValues.FORE_COLOR;
            m_dashStyle = DashStyle.Dash;
            m_fDashOffset = 4;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutGrid"/> class.
        /// </summary>
        /// <param name="src">The SRC.</param>
        public LayoutGrid(LayoutGrid src)
            : base(src)
        {
            m_gridStyle = src.m_gridStyle;
            m_fVerticalSpacing = src.m_fVerticalSpacing;
            m_fHorizontalSpacing = src.m_fHorizontalSpacing;
            m_nMinPixelSpacing = src.m_nMinPixelSpacing;
            m_bVisible = src.m_bVisible;
            m_bSnapToGrid = src.m_bSnapToGrid;
            m_clrColor = src.m_clrColor;
            m_dashStyle = src.m_dashStyle;
            m_fDashOffset = src.m_fDashOffset;
            m_styleRendering = (RenderingStyle)src.RenderingStyle.Clone();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutGrid"/> class. Deserialization constructor.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected LayoutGrid(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "gridStyle":
                        m_gridStyle = (GridStyle)entry.Value;
                        break;
                    case "verticalSpacing":
                        m_fVerticalSpacing = float.Parse(entry.Value.ToString());
                        break;
                    case "horizontalSpacing":
                        m_fHorizontalSpacing = float.Parse(entry.Value.ToString());
                        break;
                    case "minPixelSpacing":
                        m_nMinPixelSpacing = float.Parse(entry.Value.ToString());
                        break;
                    case "visible":
                        m_bVisible = Boolean.Parse(entry.Value.ToString());
                        break;
                    case "snapToGrid":
                        m_bSnapToGrid = Boolean.Parse(entry.Value.ToString());
                        break;
                    case "color":
                        m_clrColor = (Color)entry.Value;
                        break;
                    case "dashStyle":
                        m_dashStyle = (DashStyle)entry.Value;
                        break;
                    case "dashOffset":
                        m_fDashOffset = float.Parse(entry.Value.ToString());
                        break;
                    case "renderingStyle":
                        m_styleRendering = (RenderingStyle)entry.Value;
                        break;
                }
            }
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the view this grid is attached to.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property is a reference to the view that the layout grid renders
        /// itself onto.
        /// </para>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.View"/>
        /// </remarks>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public View ContainerView
        {
            get { return m_viewer; }
            set { m_viewer = value; }
        }

        /// <summary>
        /// Gets or sets the appearance of the grid.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Determines the appearance of the grid.")]
        [DefaultValue(GridStyle.Point)]
        public GridStyle GridStyle
        {
            get 
            { 
                return m_gridStyle; 
            }
            set
            {
                if (m_gridStyle != value && OnPropertyChanging(DPN.GridStyle, value))
                {
                    m_gridStyle = value;

                    OnPropertyChanged(DPN.GridStyle);
                }
            }
        }

        /// <summary>
        /// Gets or sets the vertical distance between grid points.
        /// </summary>
        /// <remarks>
        /// Spacing is specified in world units.
        /// </remarks>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Determines the vertical distance between grid points.")]
        [DefaultValue(10F)]
        public float VerticalSpacing
        {
            get 
            { 
                return m_fVerticalSpacing; 
            }
            set
            {
                // update to min available value
                value = Math.Max(value, this.MinPixelSpacing);

                if (m_fVerticalSpacing != value && OnPropertyChanging(DPN.VerticalSpacing, value))
                {
                    m_fVerticalSpacing = value;

                    OnPropertyChanged(DPN.VerticalSpacing);
                }
            }
        }

        /// <summary>
        /// Gets or sets the horizontal distance between grid points.
        /// </summary>
        /// <remarks>
        /// Spacing is specified in world units.
        /// </remarks>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Determines the horizontal distance between grid points.")]
        [DefaultValue(10F)]
        public float HorizontalSpacing
        {
            get 
            { 
                return m_fHorizontalSpacing; 
            }
            set
            {
                // update to min available value
                m_fUpdateHSpacing = value;
                value = Math.Max(value, this.MinPixelSpacing);

                if (m_fHorizontalSpacing != value && OnPropertyChanging(DPN.HorizontalSpacing, value))
                {
                    m_fHorizontalSpacing = value;

                    OnPropertyChanged(DPN.HorizontalSpacing);
                }
            }
        }

        /// <summary>
        /// Gets or sets minimum spacing between grid points in device units.
        /// </summary>
        /// <remarks>
        /// This value specifies the threshold at which the grid will stop
        /// drawing itself because the spacing between grid points is too
        /// small.
        /// </remarks>
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(4)]
        [Description("Minimum spacing between grid points in device units.")]
        public float MinPixelSpacing
        {
            get 
            { 
                return m_nMinPixelSpacing; 
            }
            set
            {
                if (m_nMinPixelSpacing != value && OnPropertyChanging(DPN.MinPixelSpacing, value))
                {
                    m_nMinPixelSpacing = value;

                    // update horizontal and vertcal spacing to min value                    
                    this.HorizontalSpacing = Math.Max(m_nMinPixelSpacing, MeasureUnitsConverter.Convert(this.m_fUpdateHSpacing,MeasureUnits.Pixel,this.MeasureUnit));
                    this.VerticalSpacing = Math.Max(m_nMinPixelSpacing, this.VerticalSpacing);

                    OnPropertyChanged(DPN.MinPixelSpacing);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether controls in the grid is visible.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(true)]
        [Description("Specifies whether the grid is visible.")]
        public bool Visible
        {
            get 
            { 
                return m_bVisible; 
            }
            set
            {
                if (m_bVisible != value && OnPropertyChanging(DPN.Visible, value))
                {
                    m_bVisible = value;

                    OnPropertyChanged(DPN.Visible);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the snap to grid feature is enable or disabled.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property determines how the
        /// <see cref="Syncfusion.Windows.Forms.Diagram.LayoutGrid.GetNearestGridPoint(PointF)"/>
        /// method behaves.
        /// </para>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.LayoutGrid.GetNearestGridPoint(PointF, int)"/>
        /// </remarks>
        [Browsable(true)]
        [DefaultValue(true)]
        [Description("Specifies whether the snap to grid feature is enabled.")]
        public bool SnapToGrid
        {
            get 
            { 
                return m_bSnapToGrid; 
            }
            set
            {
                if (m_bSnapToGrid != value && OnPropertyChanging(DPN.SnapToGrid, value))
                {
                    m_bSnapToGrid = value;

                    OnPropertyChanged(DPN.SnapToGrid);
                }
            }
        }

        /// <summary>
        /// Gets or sets the color to use for drawing the grid.
        /// </summary>
        [Browsable(true)]
        [Description("Color used for drawing the grid.")]
        public Color Color
        {
            get 
            { 
                return m_clrColor; 
            }
            set
            {
                if (m_clrColor != value && OnPropertyChanging(DPN.Color, value))
                {
                    m_clrColor = value;

                    OnPropertyChanged(DPN.Color);
                }
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="System.Drawing.Drawing2D.DashStyle"/> value to use for dashed lines.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(System.Drawing.Drawing2D.DashStyle.Dash)]
        [Description("Style used for dashed lines.")]
        public DashStyle DashStyle
        {
            get 
            { 
                return m_dashStyle; 
            }
            set
            {
                if (m_dashStyle != value && OnPropertyChanging(DPN.DashStyle, value))
                {
                    m_dashStyle = value;

                    OnPropertyChanged(DPN.DashStyle);
                }
            }
        }

        /// <summary>
        /// Gets or sets distance from the start of the line to the dash pattern.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(4f)]
        [Description("Distance from the start of the line to the dash pattern.")]
        public float DashOffset
        {
            get 
            { 
                return m_fDashOffset; 
            }
            set
            {
                if (m_fDashOffset != value && OnPropertyChanging(DPN.DashOffset, value))
                {
                    m_fDashOffset = value;

                    OnPropertyChanged(DPN.DashOffset);
                }
            }
        }
        /// <summary>
        /// Gets or sets the measure unit.
        /// </summary>
        /// <value>The measure unit.</value>
        public override MeasureUnits MeasureUnit
        {
            get
            {
                return base.MeasureUnit;
            }
            set
            {
                if (base.MeasureUnit != value)
                {
                    base.MeasureUnit = value;
                    if (value != MeasureUnits.Pixel && (m_fUpdateHSpacing >= this.MinPixelSpacing))
                    {
                        this.HorizontalSpacing = MeasureUnitsConverter.Convert(m_fHorizontalSpacing, value, MeasureUnits.Pixel);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the grid rendering style.
        /// </summary>
        /// <remarks>
        /// The rendering style is used to configure graphics.
        /// </remarks>
        /// <value>The rendering style.</value>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Grid Rendering style.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RenderingStyle RenderingStyle
        {
            get
            {
                if (m_styleRendering == null)
                {
                    m_styleRendering = new RenderingStyle();
                    m_styleRendering.UpdateServiceReferences(this);
                }

                return m_styleRendering;
            }
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Takes a device point and returns the point on the grid that is nearest
        /// to that point.
        /// </summary>
        /// <param name="ptScene">Input point.</param>
        /// <returns>Point on the grid in client coordinates.</returns>
        /// <remarks>
        /// <para>
        /// Points are in device coordinates. This method is used to when the snap
        /// to grid feature is enabled.
        /// </para>
        /// <seealso cref="Syncfusion.Windows.Forms.Diagram.LayoutGrid.SnapToGrid"/>
        /// </remarks>
        public PointF GetNearestGridPoint(PointF ptScene)
        {
            PointF ptToReturn = ptScene;

            if (this.SnapToGrid && m_viewer != null && m_viewer.Model != null)
            {
                double dMagnification = m_viewer.Magnification / 100f;

                float fHrzSpacing = MeasureUnitsConverter.ToPixelX(this.HorizontalSpacing, this.MeasureUnit);
                float fVertSpacing = MeasureUnitsConverter.ToPixelX(this.VerticalSpacing, this.MeasureUnit);
                
                // Scene spacing
                double dSceneVertSpacing = fVertSpacing * dMagnification;
                double dSceneHorzSpacing = fHrzSpacing * dMagnification;

                // Scene origin
                double dPointX = ptScene.X + (m_viewer.Origin.X * dMagnification);
                double dPointY = ptScene.Y + (m_viewer.Origin.Y * dMagnification);

                // get nearest grid point
                double dWidth = dPointX % dSceneHorzSpacing;
                double dHeight = dPointY % dSceneVertSpacing;

                // if we are farer than half grid cell --> move to next grid point
                if (dWidth >= (dSceneHorzSpacing / 2d))
                {
                    ptToReturn.X = (float)(ptScene.X + dSceneHorzSpacing - dWidth);
                }
                else
                {
                    ptToReturn.X = (float)(ptScene.X - dWidth);
                }

                // if we are farer than half grid point --> move to next grid point
                if (dHeight >= (dSceneVertSpacing / 2d))
                {
                    ptToReturn.Y = (float)(ptScene.Y + dSceneVertSpacing - dHeight);
                }
                else
                {
                    ptToReturn.Y = (float)(ptScene.Y - dHeight);
                }
            }

            return ptToReturn;
        }
		
		public PointF GetNearestGridPoint(PointF ptScene,int rulerHeight)
        {
            PointF ptToReturn = GetNearestGridPoint(ptScene);
            if (this.SnapToGrid && m_viewer != null && m_viewer.Model != null)
            {
                double dMagnification = m_viewer.Magnification / 100f;

                float fHrzSpacing = MeasureUnitsConverter.ToPixelX(this.HorizontalSpacing, this.MeasureUnit);
                float fVertSpacing = MeasureUnitsConverter.ToPixelX(this.VerticalSpacing, this.MeasureUnit);

                // Scene spacing
                double dSceneVertSpacing = fVertSpacing * dMagnification;
                double dSceneHorzSpacing = fHrzSpacing * dMagnification;
                
                ptToReturn.X += (float)(rulerHeight % dSceneHorzSpacing);
                ptToReturn.Y += (float)(rulerHeight % dSceneVertSpacing);
            }
            return ptToReturn;
        }
		
        #region rendering
        /// <summary>
        /// Renders the grid onto a specified System.Drawing.Graphics object.
        /// </summary>
        /// <param name="grfx">Drawing context object.</param>
        /// <param name="rectGrid">Visible grid bounds.</param>
        /// <remarks>
        /// <para>
        /// This method is overridden in derived classes to render specific types of
        /// grids.
        /// </para>
        /// </remarks>
        public virtual void Draw(Graphics grfx, RectangleF rectGrid)
        {
            GraphicsState state = grfx.Save();
            this.RenderingStyle.ApplySettings(grfx);
            if (this.Visible && rectGrid.Width != 0 && rectGrid.Height != 0)
            {
                float fMagnification = grfx.PageScale;
                int bmpwidth = ((int)Math.Round(rectGrid.Width * fMagnification) != 0) ?
                    (int)Math.Round(rectGrid.Width * fMagnification) :
                    1;
                int bmpheight = ((int)Math.Round(rectGrid.Height * fMagnification) != 0) ?
                    (int)Math.Round(rectGrid.Height * fMagnification) :
                    1;
                Bitmap imgPreview = new Bitmap(bmpwidth, bmpheight);

                using (Graphics gfxImg = Graphics.FromImage(imgPreview))
                {
                    gfxImg.TranslateTransform(-rectGrid.X, -rectGrid.Y);
                    gfxImg.PageScale = fMagnification;
                    gfxImg.PageUnit = grfx.PageUnit;
                    gfxImg.InterpolationMode = grfx.InterpolationMode;
                    gfxImg.SmoothingMode = grfx.SmoothingMode;

                    switch (this.GridStyle)
                    {
                        case GridStyle.Point:
                            DrawPointGrid(gfxImg, rectGrid);
                            break;
                        case GridStyle.Line:
                            DrawLineGrid(gfxImg, rectGrid);
                            break;
                    }

                    grfx.DrawImage(imgPreview, rectGrid.X, rectGrid.Y);
                }
            }
            grfx.Restore(state);
        }

        /// <summary>
        /// Renders the grid onto a specified System.Drawing.Graphics object.
        /// </summary>
        /// <param name="grfx">Drawing context object.</param>
        /// <param name="rectGridBounds">Visible grid bounds.</param>
        /// <remarks>
        /// <para>
        /// Sets pixel values directly on the view's drawing surface.
        /// </para>
        /// </remarks>
        public virtual void DrawPointGrid(Graphics grfx, RectangleF rectGridBounds)
        {
            RectangleF rectClip = rectGridBounds;

            // Draw points
            DrawPoints(grfx, rectClip);
        }

        /// <summary>
        /// Renders the grid onto a specified System.Drawing.Graphics object as lines.
        /// </summary>
        /// <param name="grfx">Drawing context object.</param>
        /// <param name="rectGridBounds">The grid bounds.</param>
        protected virtual void DrawLineGrid(Graphics grfx, RectangleF rectGridBounds)
        {
            RectangleF rectClip = rectGridBounds;
            
            // draw grid outline
            rectClip.Inflate(5f, 5f);

            float fHrzSpacing = MeasureUnitsConverter.ToPixelX(this.HorizontalSpacing, this.MeasureUnit);
            float fVertSpacing = MeasureUnitsConverter.ToPixelX(this.VerticalSpacing, this.MeasureUnit);
            
            // Create pen to Draw lines
            using (Pen pen = new Pen(this.Color, 0f))
            {
                pen.DashStyle = this.DashStyle;
                pen.DashOffset = this.DashOffset;

                // Draw Lines
                if (rectClip.Width > fVertSpacing)
                    DrawVerticalLines(grfx, pen, rectClip);

                if (rectClip.Height > fHrzSpacing)
                    DrawHorizontalLines(grfx, pen, rectClip);
            }
        }

        /// <summary>
        /// Draws the points inside rectangle.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        /// <param name="rectClip">The clip rectangle.</param>
        protected void DrawPoints(Graphics gfx, RectangleF rectClip)
        {
            float fVertSpacing = MeasureUnitsConverter.ToPixelX(this.VerticalSpacing, this.MeasureUnit);
            float fHrzSpacing = MeasureUnitsConverter.ToPixelX(this.HorizontalSpacing, this.MeasureUnit);
            float fCurrentPositionX = 0;
            float fCurrentPositionY = 0;

            Brush brush = new SolidBrush(this.Color);
            while (fCurrentPositionY < rectClip.Bottom)
            {
                while (fCurrentPositionX < rectClip.Right)
                {
                    gfx.FillRectangle(brush, fCurrentPositionX - 0.5f, fCurrentPositionY - 0.5f, 1, 1);
                    fCurrentPositionX += fHrzSpacing;
                }
                fCurrentPositionY += fVertSpacing;
                fCurrentPositionX = 0;
            }                  
        }

        /// <summary>
        /// Draws the vertical lines inside rectangle.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        /// <param name="penLine">The pen line.</param>
        /// <param name="rectClip">The graphics clip rectangle.</param>
        protected void DrawVerticalLines(Graphics gfx, Pen penLine, RectangleF rectClip)
        {
            SizeF viewOffset = SizeF.Empty;
            viewOffset.Width = rectClip.X;
            viewOffset.Height = rectClip.Y;

            float fHrzSpacing = MeasureUnitsConverter.ToPixelX(this.HorizontalSpacing, this.MeasureUnit);
            int lineIndex = (int)(viewOffset.Width / fHrzSpacing);
            float fCurrentPosition = lineIndex * fHrzSpacing;

            while (fCurrentPosition < rectClip.Right)
            {
                gfx.DrawLine(penLine, fCurrentPosition, rectClip.Y, fCurrentPosition, rectClip.Bottom);
                fCurrentPosition += fHrzSpacing;
            }
        }

        /// <summary>
        /// Draws the horizontal lines inside rectangle.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        /// <param name="penLine">The pen line.</param>
        /// <param name="rectClip">The graphics clip rectangle.</param>
        protected void DrawHorizontalLines(Graphics gfx, Pen penLine, RectangleF rectClip)
        {
            SizeF viewOffset = SizeF.Empty;
            viewOffset.Width = rectClip.X;
            viewOffset.Height = rectClip.Y;

            float fVertSpacing = MeasureUnitsConverter.ToPixelX(this.VerticalSpacing, this.MeasureUnit);
            int lineIndex = (int)(viewOffset.Height / fVertSpacing);
            float fCurrentPosition = lineIndex * fVertSpacing;

            while (fCurrentPosition < rectClip.Bottom)
            {
                gfx.DrawLine(penLine, rectClip.X, fCurrentPosition, rectClip.Right, fCurrentPosition);
                fCurrentPosition += fVertSpacing;
            }
        }
        #endregion

        #endregion

        #region Class overrides
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public override object Clone()
        {
            return new LayoutGrid(this);
        }

        /// <summary>
        /// Gets the object data.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("gridStyle", m_gridStyle);
            info.AddValue("verticalSpacing", m_fVerticalSpacing);
            info.AddValue("horizontalSpacing", m_fHorizontalSpacing);
            info.AddValue("minPixelSpacing", m_nMinPixelSpacing);
            info.AddValue("visible", m_bVisible);
            info.AddValue("snapToGrid", m_bSnapToGrid);
            info.AddValue("color", m_clrColor);
            info.AddValue("dashStyle", m_dashStyle);
            info.AddValue("dashOffset", m_fDashOffset);
            info.AddValue("renderingStyle", this.RenderingStyle);
        }

        /// <summary>
        /// Gets the name of the property container.
        /// </summary>
        /// <returns>The property container name.</returns>
        protected override string GetPropertyContainerName()
        {
            return DPN.LayoutGrid;
        }

        /// <summary>
        /// Called when measure units changing.
        /// </summary>
        /// <param name="from">Old value of the measure units .</param>
        /// <param name="to">New value of the measure units.</param>
        protected override void OnMeasureUnitsChanging(MeasureUnits from, MeasureUnits to)
        {
            base.OnMeasureUnitsChanging(from, to);

            m_nMinPixelSpacing = MeasureUnitsConverter.ConvertX(m_nMinPixelSpacing, from, to);
            m_fDashOffset = MeasureUnitsConverter.ConvertX(m_fDashOffset, from, to);
            m_fHorizontalSpacing = MeasureUnitsConverter.ConvertX(m_fHorizontalSpacing, from, to);
            m_fVerticalSpacing = MeasureUnitsConverter.ConvertX(m_fVerticalSpacing, from, to);
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Serialize the color.
        /// </summary>
        /// <returns>true, if serialize the color.</returns>
        [Documentation.DocumentationExclude()]
        protected bool ShouldSerializeColor()
        {
            return this.Color != Color.Black;
        }

        /// <summary>
        /// Resets the color.
        /// </summary>
        [Documentation.DocumentationExclude()]
        protected void ResetColor()
        {
            this.Color = Color.Black;
        }
        #endregion
    }
}
