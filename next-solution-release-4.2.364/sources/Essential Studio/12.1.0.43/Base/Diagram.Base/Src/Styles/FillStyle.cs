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
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Syncfusion.Documentation;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// A FillStyle is a collection of properties that define a brush used for
    /// fill operations during rendering.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(FillStyleConverter))]
    [Editor(typeof(FillStyleValueEditor), typeof(UITypeEditor))]
    public class FillStyle
        : PropertyContainer
    {
        #region Class members
        private Color m_clr;
        private Color m_clrFore;
        private int m_nColorAlphaFactor;
        private int m_nForeColorAlphaFactor;
        private Image m_imgTexture;
        private WrapMode m_textureWrapMode;
        private float m_fGradientAngle;
        private float m_fGradientCenter;
        private FillStyleType m_styleFill;
        private HatchStyle m_styleHatch;
        private PathGradientBrushStyle m_stylePathGradient;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="FillStyle"/> class.
        /// </summary>
        public FillStyle()
        {
            m_clr = Color.Yellow;
            m_clrFore = Color.LightGray;
            m_nColorAlphaFactor = 255;
            m_nForeColorAlphaFactor = 255;
            m_textureWrapMode = WrapMode.Tile;
            m_fGradientCenter = 1;
            m_fGradientAngle = 90f;
            m_styleFill = FillStyleType.Solid;
            m_styleHatch = HatchStyle.Horizontal;
            m_stylePathGradient = PathGradientBrushStyle.RectangleCenter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FillStyle"/> class.
        /// </summary>
        /// <param name="src">The SRC.</param>
        public FillStyle(FillStyle src)
            : base(src)
        {
            m_clr = src.m_clr;
            m_clrFore = src.m_clrFore;
            m_nColorAlphaFactor = src.m_nColorAlphaFactor;
            m_nForeColorAlphaFactor = src.m_nForeColorAlphaFactor;
            m_textureWrapMode = src.m_textureWrapMode;
            m_fGradientCenter = src.m_fGradientCenter;
            m_fGradientAngle = src.GradientAngle;
            m_styleFill = src.m_styleFill;
            m_styleHatch = src.m_styleHatch;
            m_stylePathGradient = src.m_stylePathGradient;

            if (src.m_imgTexture != null)
            {
                m_imgTexture = (Image)src.m_imgTexture.Clone();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FillStyle"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected FillStyle(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            m_clr = (Color)info.GetValue("color", typeof(Color));
            m_clrFore = (Color)info.GetValue("foreColor", typeof(Color));
            m_nColorAlphaFactor = info.GetInt32("colorAlphaFactor");
            m_nForeColorAlphaFactor = info.GetInt32("foreColorAlphaFactor");
            m_textureWrapMode = (WrapMode)info.GetValue("textureWrapMode", typeof(WrapMode));
            m_fGradientCenter = info.GetSingle("gradientCenter");
            m_fGradientAngle = info.GetSingle("gradientAngle");
            m_styleFill = (FillStyleType)info.GetValue("fillStyleType", typeof(FillStyleType));
            m_styleHatch = (HatchStyle)info.GetValue("hatchStyle", typeof(HatchStyle));
            m_stylePathGradient = (PathGradientBrushStyle)info.GetValue("pathGradientStyle", typeof(PathGradientBrushStyle));

            bool imagepresent;
            try
            {
                imagepresent = info.GetBoolean("imagepresent");
            }
            catch
            {
                imagepresent = true;
            }

            if (imagepresent)
            {
                try
                {
                    m_imgTexture = (Image)info.GetValue("textureImage", typeof(Image));
                }
                catch
                { 
                }
            }
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets color to use for the brush.
        /// </summary>
        /// <remarks>
        /// NOTE: If <see cref="Syncfusion.Windows.Forms.Diagram.FillStyle.Type"/> is
        /// set to FillType.LinearGradient, then this is the ending color for the
        /// gradient.
        /// </remarks>
        [Browsable(true)]
        [Description("Color to use for filled regions."), DefaultValue(typeof(Color), "White")]
        public Color Color
        {
            get 
            { 
                return m_clr; 
            }
            set
            {
                if (m_clr != value && OnPropertyChanging(DPN.Color, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.Color);

                    // assign new value
                    m_clr = value;

                    if (m_clr == Color.Transparent)
                        m_nColorAlphaFactor = 0;
                    else
                        m_nColorAlphaFactor = (value.A != m_nColorAlphaFactor) ? value.A : m_nColorAlphaFactor;

                    // raise property changed event
                    OnPropertyChanged(DPN.Color);
                }
            }
        }

        /// <summary>
        /// Gets or sets alpha blending factor.
        /// </summary>
        [Browsable(true)]
        [Description("Alpha blending factor ( 0 = transparent, 255 = opaque )")]
        [DefaultValue(255)]
        public int ColorAlphaFactor
        {
            get 
            { 
                return m_nColorAlphaFactor; 
            }
            set
            {
                if (value < CommonUsedValues.TRANSPARENT && value > CommonUsedValues.OPAQUE)
                    throw new ArgumentOutOfRangeException("ColorAlphaFactor");

                if (m_nColorAlphaFactor != value && OnPropertyChanging(DPN.ColorAlphaFactor, value))
                {
                    //// make history record
                    RecordPropertyChanged(DPN.ColorAlphaFactor);
                    //// assign new value
                    m_nColorAlphaFactor = value;

                    m_nColorAlphaFactor = value;
                    //// raise property changed event
                    OnPropertyChanged(DPN.ColorAlphaFactor);
                }
            }
        }

        /// <summary>
        /// Gets or sets alpha blending factor for the ForeColor.
        /// </summary>
        [Browsable(true)]
        [Description("Alpha blending factor ( 0 = transparent, 255 = opaque )")]
        [DefaultValue(255)]
        public int ForeColorAlphaFactor
        {
            get 
            { 
                return m_nForeColorAlphaFactor; 
            }
            set
            {
                if (value < CommonUsedValues.TRANSPARENT && value > CommonUsedValues.OPAQUE)
                    throw new ArgumentOutOfRangeException("ForeColorAlphaFactor");

                if (m_nForeColorAlphaFactor != value && OnPropertyChanging(DPN.ForeColorAlphaFactor, value))
                {
                    //// make history record
                    RecordPropertyChanged(DPN.ForeColorAlphaFactor);
                    //// assign new value
                    m_nForeColorAlphaFactor = value;
                    //// raise property changed event
                    OnPropertyChanged(DPN.ForeColorAlphaFactor);
                }
            }
        }

        /// <summary>
        /// Gets or sets the foreground color used for the fill style.
        /// </summary>
        /// <remarks>
        /// NOTE: If <see cref="Syncfusion.Windows.Forms.Diagram.FillStyle.Type"/> is
        /// set to FillType.LinearGradient, then this is the starting color for the
        /// gradient.
        /// </remarks>
        [Browsable(true)]
        [Description("The foreground color used for the fill.")]
        public Color ForeColor
        {
            get 
            { 
                return m_clrFore; 
            }
            set
            {
                if (m_clrFore != value && OnPropertyChanging(DPN.ForeColor, value))
                {
                    //// make history record
                    RecordPropertyChanged(DPN.ForeColor);
                    //// assign new value
                    m_clrFore = value;
                    if (m_clrFore == Color.Transparent)
                        m_nForeColorAlphaFactor = 0;
                    else
                        m_nForeColorAlphaFactor = (value.A != m_nForeColorAlphaFactor) ? value.A : m_nForeColorAlphaFactor;

                    //// raise property changed event
                    OnPropertyChanged(DPN.ForeColor);
                }
            }
        }

        /// <summary>
        /// Gets or sets image to use for texture fill.
        /// </summary>
        [Browsable(true)]
        [RefreshProperties(RefreshProperties.All)]
        [Description("Image to use for texture fill.")]
        [DefaultValue(null)]
        public Image Texture
        {
            get 
            { 
                return m_imgTexture; 
            }
            set
            {
                if (m_imgTexture != value && OnPropertyChanging(DPN.Texture, value))
                {
                    //// make history record
                    RecordPropertyChanged(DPN.Texture);
                    //// assign new value
                    m_imgTexture = value;
                    //// raise property changed event
                    OnPropertyChanged(DPN.Texture);
                }
            }
        }

        /// <summary>
        /// Gets or sets the mode how the texture is wrapped if fill type is set to texture.
        /// </summary>
        [Browsable(true)]
        [Description("How to wrap the texture.")]
        [DefaultValue(WrapMode.Tile)]
        public WrapMode TextureWrapMode
        {
            get 
            { 
                return m_textureWrapMode; 
            }
            set
            {
                if (m_textureWrapMode != value && OnPropertyChanging(DPN.TextureWrapMode, value))
                {
                    //// make history record
                    RecordPropertyChanged(DPN.TextureWrapMode);
                    //// assign new value
                    m_textureWrapMode = value;
                    //// raise property changed event
                    OnPropertyChanged(DPN.TextureWrapMode);
                }
            }
        }

        /// <summary>
        /// Gets or sets angle used for gradient fill.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(90f)]
        [Description("Angle used for gradient fill.")]
        public float GradientAngle
        {
            get 
            { 
                return m_fGradientAngle; 
            }
            set
            {
                if (m_fGradientAngle != value && OnPropertyChanging(DPN.GradientAngle, value))
                {
                    //// make history record
                    RecordPropertyChanged(DPN.GradientAngle);
                    //// assign new value
                    m_fGradientAngle = value;
                    //// raise property changed event
                    OnPropertyChanged(DPN.GradientAngle);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value from 0 through 1 that specifies the center of the gradient.
        /// </summary>
        [Browsable(true)]
        [Description("A value from 0 through 1 that specifies the center of the gradient.")]
        [DefaultValue(1f)]
        public float GradientCenter
        {
            get 
            { 
                return m_fGradientCenter; 
            }
            set
            {
                if (m_fGradientCenter != value && ((value >= 0) && (value <= 1))
                    && OnPropertyChanging(DPN.GradientCenter, value))
                {
                    //// make history record
                    RecordPropertyChanged(DPN.GradientCenter);
                    //// assign new value
                    m_fGradientCenter = value;
                    //// raise property changed event
                    OnPropertyChanged(DPN.GradientCenter);
                }
            }
        }

        /// <summary>
        /// Gets or sets type of brush to create for filled regions.
        /// </summary>
        [Browsable(true)]
        [RefreshProperties(RefreshProperties.All)]
        [Description("Type of brush to create.")]
        [DefaultValue(FillStyleType.Solid)]
        public FillStyleType Type
        {
            get 
            { 
                return m_styleFill; 
            }
            set
            {
                if (m_styleFill != value && OnPropertyChanging(DPN.Type, value))
                {
                    //// make history record
                    RecordPropertyChanged(DPN.Type);
                    //// assign new value
                    m_styleFill = value;
                    //// raise property changed event
                    OnPropertyChanged(DPN.Type);
                }
            }
        }

        /// <summary>
        /// Gets or sets hatch brush style to create for filled regions.
        /// </summary>
        [Browsable(true)]
        [Description("Hatch brush style to create.")]
        [DefaultValue(HatchStyle.Horizontal)]
        public HatchStyle HatchBrushStyle
        {
            get 
            { 
                return m_styleHatch; 
            }
            set
            {
                if (m_styleHatch != value && OnPropertyChanging(DPN.HatchBrushStyle, value))
                {
                    //// make history record
                    RecordPropertyChanged(DPN.HatchBrushStyle);
                    //// assign new value
                    m_styleHatch = value;
                    //// raise property changed event
                    OnPropertyChanged(DPN.HatchBrushStyle);
                }
            }
        }

        /// <summary>
        /// Gets or sets the path brush style.
        /// </summary>
        /// <value>The path brush style.</value>
        [Browsable(true)]
        [Description("Specifies style for path gradient brush.")]
        [DocumentationExclude()]
        public PathGradientBrushStyle PathBrushStyle
        {
            get 
            { 
                return m_stylePathGradient; 
            }
            set
            {
                if (m_stylePathGradient != value && OnPropertyChanging(DPN.PathBrushStyle, value))
                {
                    //// make history record
                    RecordPropertyChanged(DPN.PathBrushStyle);
                    //// assign new value
                    m_stylePathGradient = value;
                    //// raise property changed event
                    OnPropertyChanged(DPN.PathBrushStyle);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [inherit container measure units].
        /// </summary>
        /// <value>
        /// <c>true</c> if need to inherit container measure units; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool InheritContainerMeasureUnits
        {
            get { return base.InheritContainerMeasureUnits; }
            set { base.InheritContainerMeasureUnits = value; }
        }

        /// <summary>
        /// Gets or sets the measure unit.
        /// </summary>
        /// <value>The measure unit.</value>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override MeasureUnits MeasureUnit
        {
            get { return base.MeasureUnit; }
            set { base.MeasureUnit = value; }
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Creates a new brush based on the properties contained by
        /// the FillStyle object.
        /// </summary>
        /// <param name="grfx">Graphics context the brush will be used in.</param>
        /// <param name="fillBounds">Bounds of the object to be filled (needed for gradient
        /// brushes only).</param>
        /// <returns>A GDI+ brush object.</returns>
        public Brush CreateBrush(Graphics grfx, RectangleF fillBounds)
        {
            Brush br = null;
            Image texture;
            TextureBrush textureBrush;

            try
            {
                switch (this.Type)
                {
                    case FillStyleType.Solid:
                        {
                            br = new SolidBrush(Color.FromArgb(this.ColorAlphaFactor, this.Color));
                            break;
                        }

                    case FillStyleType.LinearGradient:
                        {
                            Color clrFore = System.Drawing.Color.FromArgb(this.ForeColorAlphaFactor, this.ForeColor);
                            Color clrBack = System.Drawing.Color.FromArgb(this.ColorAlphaFactor, this.Color);

                            br = new LinearGradientBrush(fillBounds, clrFore, clrBack, this.GradientAngle, true);
                            ((LinearGradientBrush)br).SetBlendTriangularShape(this.GradientCenter);
                            break;
                        }

                    case FillStyleType.Texture:
                        {
                            texture = this.Texture;
                            if (texture != null)
                            {
                                RectangleF textureBounds = new RectangleF(0, 0, texture.Width, texture.Height);
                                textureBrush = new TextureBrush(this.Texture, this.TextureWrapMode, textureBounds);
                                textureBrush.Transform = new Matrix(1, 0, 0, 1, fillBounds.X, fillBounds.Y);
                                br = textureBrush;
                            }
                            else
                            {
                                br = new SolidBrush(this.Color);
                            }
                            break;
                        }
                    case FillStyleType.Hatch:
                        br = CreateHatchBrush();
                        break;
                    case FillStyleType.PathGradient:
                        br = CreatePathGradientBrush(fillBounds);
                        break;
                }
            }
            catch (Exception)
            {
                br = new SolidBrush(this.Color);
            }

            return br;
        }

        #endregion

        #region Class helper Methods

        #region designer serialization helpers
        /// <summary>
        /// Shows whether Color will be serializable.
        /// </summary>
        /// <returns>true, if serialize color.</returns>
        [DocumentationExclude()]
        protected bool ShouldSerializeColor()
        {
            return Color.LightGray != this.Color;
        }

        /// <summary>
        /// Resets the color.
        /// </summary>
        [DocumentationExclude()]
        private void ResetColor()
        {
            this.Color = Color.DarkGray;
        }

        /// <summary>
        /// Shows whether ForeColor will be serializable.
        /// </summary>
        /// <returns>true, if serialize forecolor.</returns>
        [DocumentationExclude()]
        protected bool ShouldSerializeForeColor()
        {
            return Color.LightGray != this.ForeColor;
        }
        [DocumentationExclude()]
        private void ResetForeColor()
        {
            this.Color = Color.LightGray;
        }
        #endregion

        private Brush CreateHatchBrush()
        {
            Brush br;
            Color clrFore = System.Drawing.Color.FromArgb(this.ForeColorAlphaFactor, this.ForeColor);
            Color clrBack = System.Drawing.Color.FromArgb(this.ColorAlphaFactor, this.Color);

            br = new HatchBrush(this.HatchBrushStyle, clrFore, clrBack);
            return br;
        }
        private Brush CreatePathGradientBrush(RectangleF rectBounds)
        {
            GraphicsPath gp = CreateGraphicsPath(rectBounds);

            PathGradientBrush br = new PathGradientBrush(gp);
            br.WrapMode = WrapMode.Clamp;
            br.CenterColor = System.Drawing.Color.FromArgb(this.ForeColorAlphaFactor, this.ForeColor);
            Color[] clrSurr = { System.Drawing.Color.FromArgb(this.ColorAlphaFactor, this.Color) };
            br.SurroundColors = clrSurr;

            br.CenterPoint = DetermineCenterPoint(rectBounds);

            return br;
        }
        private GraphicsPath CreateGraphicsPath(RectangleF rectFill)
        {
            RectangleF rect = rectFill;
            GraphicsPath gp = new GraphicsPath();
            gp.StartFigure();

            switch (this.PathBrushStyle)
            {
                case PathGradientBrushStyle.RectangleCenter:
                case PathGradientBrushStyle.RectangleLeftBottom:
                case PathGradientBrushStyle.RectangleLeftTop:
                case PathGradientBrushStyle.RectangleRightBottom:
                case PathGradientBrushStyle.RectangleRightTop:
                    gp.AddRectangle(rect);
                    break;
                case PathGradientBrushStyle.CircleCenter:
                    float radius = (float)Math.Abs(Math.Sqrt(Math.Pow(rect.Width, 2) + Math.Pow((float)rect.Height, 2))) / 2;
                    RectangleF rect2 = new RectangleF(
                        new PointF(rect.X + rect.Width / 2 - radius, rect.Y + rect.Height / 2 - radius),
                        new SizeF(radius * 2, radius * 2));
                    gp.AddArc(rect2, 0, 360);
                    break;
                case PathGradientBrushStyle.CircleLeftTop:
                    // rect = new RectangleF( rectFill.X, rectFill.Y, rectFill.Width * 2, rectFill.Height * 2 );
                    radius = (float)Math.Abs(Math.Sqrt(Math.Pow(rect.Width, 2) + Math.Pow((float)rect.Height, 2)));
                    rect2 = new RectangleF(
                        new PointF(rect.X - radius, rect.Y - radius),
                        new SizeF(radius * 2, radius * 2));
                    
                    // gp.AddLine( new PointF( rect.X + rect.Width / 2, rect.Y ), new PointF( rect.X + rect.Width / 2, rect.Y + rect.Height / 2 ) );
                    // gp.AddLine( new PointF( rect.X, rect.Y + rect.Height / 2 ), new PointF( rect.X + rect.Width / 2, rect.Y + rect.Height / 2 ) );
                    gp.AddArc(rect2, 0, 360);
                    break;
                case PathGradientBrushStyle.CircleRightTop:
                    // rect = new RectangleF( rectFill.X, rectFill.Y, rectFill.Width * 2, rectFill.Height * 2 );
                    radius = (float)Math.Abs(Math.Sqrt(Math.Pow(rect.Width, 2) + Math.Pow((float)rect.Height, 2)));
                    rect2 = new RectangleF(
                        new PointF(rect.Right - radius, rect.Top - radius),
                        new SizeF(radius * 2, radius * 2));

                    // gp.AddLine( new PointF( rect.X + rect.Width / 2, rect.Y ), new PointF( rect.X + rect.Width / 2, rect.Y + rect.Height / 2 ) );
                    // gp.AddLine( new PointF( rect.X + rect.Width, rect.Y + rect.Height / 2 ), new PointF( rect.X + rect.Width / 2, rect.Y + rect.Height / 2 ) );
                    gp.AddArc(rect2, 0, 360);
                    break;
                case PathGradientBrushStyle.CircleLeftBottom:
                    // rect = new RectangleF( rectFill.X, rectFill.Y, rectFill.Width * 2, rectFill.Height * 2 );
                    // gp.AddLine( new PointF( rect.X + rect.Width / 2, rect.Y ), new PointF( rect.X + rect.Width / 2, rect.Y + rect.Height / 2 ) );
                    // gp.AddLine( new PointF( rect.X, rect.Y + rect.Height / 2 ), new PointF( rect.X + rect.Width / 2, rect.Y + rect.Height / 2 ) );
                    radius = (float)Math.Abs(Math.Sqrt(Math.Pow(rect.Width, 2) + Math.Pow((float)rect.Height, 2)));
                    rect2 = new RectangleF(
                        new PointF(rect.X - radius, rect.Bottom - radius),
                        new SizeF(radius * 2, radius * 2));
                    gp.AddArc(rect2, 0, 360);
                    break;
                case PathGradientBrushStyle.CircleRightBottom:
                    // rect = new RectangleF( rectFill.X, rectFill.Y, rectFill.Width * 2, rectFill.Height * 2 );
                    // gp.AddLine( new PointF( rect.X + rect.Width / 2, rect.Y + rect.Height ), new PointF( rect.X + rect.Width / 2, rect.Y + rect.Height / 2 ) );
                    // gp.AddLine( new PointF( rect.X + rect.Width, rect.Y + rect.Height / 2 ), new PointF( rect.X + rect.Width / 2, rect.Y + rect.Height / 2 ) );
                    // gp.AddArc( rect, 0, 90 );
                    radius = (float)Math.Abs(Math.Sqrt(Math.Pow(rect.Width, 2) + Math.Pow((float)rect.Height, 2)));
                    rect2 = new RectangleF(
                        new PointF(rect.Right - radius, rect.Bottom - radius),
                        new SizeF(radius * 2, radius * 2));
                    gp.AddArc(rect2, 0, 360);
                    break;
            }
            gp.CloseFigure();

            return gp;
        }

        private PointF DetermineCenterPoint(RectangleF rectFill)
        {
            PointF pt = new PointF();
            switch (this.PathBrushStyle)
            {
                case PathGradientBrushStyle.CircleCenter:
                case PathGradientBrushStyle.RectangleCenter:
                    pt = new PointF(rectFill.X + rectFill.Width / 2, rectFill.Y + rectFill.Height / 2);
                    break;
                case PathGradientBrushStyle.CircleLeftTop:
                case PathGradientBrushStyle.RectangleLeftTop:
                    pt = new PointF(rectFill.X, rectFill.Y);
                    break;
                case PathGradientBrushStyle.CircleLeftBottom:
                case PathGradientBrushStyle.RectangleLeftBottom:
                    pt = new PointF(rectFill.X, rectFill.Y + rectFill.Height);
                    break;
                case PathGradientBrushStyle.CircleRightTop:
                case PathGradientBrushStyle.RectangleRightTop:
                    pt = new PointF(rectFill.X + rectFill.Width, rectFill.Y);
                    break;
                case PathGradientBrushStyle.CircleRightBottom:
                case PathGradientBrushStyle.RectangleRightBottom:
                    pt = new PointF(rectFill.X + rectFill.Width, rectFill.Y + rectFill.Height);
                    break;
            }

            return pt;
        }

        /// <summary>
        /// Gets the name of the property container.
        /// </summary>
        /// <returns>Property container name.</returns>
        protected override string GetPropertyContainerName()
        {
            return DPN.FillStyle;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public override object Clone()
        {
            return new FillStyle(this);
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

            info.AddValue("color", m_clr);
            info.AddValue("foreColor", m_clrFore);
            info.AddValue("colorAlphaFactor", m_nColorAlphaFactor);
            info.AddValue("foreColorAlphaFactor", m_nForeColorAlphaFactor);
            info.AddValue("textureWrapMode", m_textureWrapMode);
            info.AddValue("gradientCenter", m_fGradientCenter);
            info.AddValue("gradientAngle", m_fGradientAngle);
            info.AddValue("fillStyleType", m_styleFill);
            info.AddValue("hatchStyle", m_styleHatch);
            info.AddValue("pathGradientStyle", m_stylePathGradient);

            info.AddValue("imagepresent", (m_imgTexture != null));
            if (m_imgTexture != null)
            {
                info.AddValue("textureImage", m_imgTexture);
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.</exception>
        public override bool Equals(object obj)
        {
            FillStyle fs = obj as FillStyle;
            if (fs != null)
            {
                bool b = TypeDescriptor.GetConverter(fs).ConvertToString(fs) == TypeDescriptor.GetConverter(this).ConvertToString(this);
                return b;
            }
            return base.Equals(obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }

    /// <summary>
    /// Property Editor for FillStyle type.
    /// </summary>
    public class FillStyleValueEditor : System.Drawing.Design.UITypeEditor
    {
        #region Constants
        protected const string c_strFILL_STYLE = "FillStyle";

        protected const string c_strBACKGROUND_STYLE = "BackgroundStyle";
        #endregion

        #region Class members
        private IWindowsFormsEditorService edSvc = null;
        #endregion

        #region Class utility methods
        private static object UpdateReturnValue(FillStyle fs, FillStyleDialog fsdlg)
        {
            fs.Color = fsdlg.FillStyle.Color;
            fs.ForeColor = fsdlg.FillStyle.ForeColor;
            fs.ColorAlphaFactor = fsdlg.FillStyle.ColorAlphaFactor;
            fs.ForeColorAlphaFactor = fsdlg.FillStyle.ForeColorAlphaFactor;
            fs.Type = fsdlg.FillStyle.Type;
            fs.GradientAngle = fsdlg.FillStyle.GradientAngle;
            fs.GradientCenter = fsdlg.FillStyle.GradientCenter;
            fs.PathBrushStyle = fsdlg.FillStyle.PathBrushStyle;
            fs.HatchBrushStyle = fsdlg.FillStyle.HatchBrushStyle;
            fs.Texture = fsdlg.FillStyle.Texture;
            fs.TextureWrapMode = fsdlg.FillStyle.TextureWrapMode;
            return fs;
        }

        /// <summary>
        /// Sets the editor properties.
        /// </summary>
        /// <param name="editingInstance">The editing instance.</param>
        /// <param name="editor">The editor.</param>
        protected virtual void SetEditorProps(FillStyle editingInstance, FillStyleDialog editor)
        {
            editor.FillStyle.Color = editingInstance.Color;
            editor.FillStyle.ForeColor = editingInstance.ForeColor;
            editor.FillStyle.ColorAlphaFactor = editingInstance.ColorAlphaFactor;
            editor.FillStyle.ForeColorAlphaFactor = editingInstance.ForeColorAlphaFactor;
            editor.FillStyle.Type = editingInstance.Type;
            editor.FillStyle.GradientAngle = editingInstance.GradientAngle;
            editor.FillStyle.GradientCenter = editingInstance.GradientCenter;
            editor.FillStyle.PathBrushStyle = editingInstance.PathBrushStyle;
            editor.FillStyle.HatchBrushStyle = editingInstance.HatchBrushStyle;
            editor.FillStyle.Texture = editingInstance.Texture;
            editor.FillStyle.TextureWrapMode = editingInstance.TextureWrapMode;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Edits the specified object's value using the editor style indicated by the <see cref="M:System.Drawing.Design.UITypeEditor.GetEditStyle"/> method.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that can be used to gain additional context information.</param>
        /// <param name="provider">An <see cref="T:System.IServiceProvider"/> that this editor can use to obtain services.</param>
        /// <param name="value">The object to edit.</param>
        /// <returns>
        /// The new value of the object. If the value of the object has not changed, this should return the same object it was passed.
        /// </returns>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context != null
                && context.Instance != null
                && provider != null)
            {
                edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (edSvc != null)
                {
                    if (context.Instance is Array)
                    {
                        Array ar = context.Instance as Array;
                        if (ar.Length > 0)
                        {
                            FillStyle[] fsArray = new FillStyle[ar.Length];
                            int i = 0;
                            foreach (object o in ar)
                            {
                                FillStyle fs = null;

                                // get exact property name
                                // -----------------------
                                // FillStyle is used for defining BackgroundStyle also
                                
                                fs = (FillStyle)TypeDescriptor.GetProperties(o, false)[context.PropertyDescriptor.Name].GetValue(o);
                                if (fs != null)
                                {
                                    fsArray[i] = fs;
                                    i++;
                                }
                            }
                            if (fsArray.Length > 0)
                            {
                                FillStyleDialog fsdlg = new FillStyleDialog();
                                SetEditorProps(fsArray[0], fsdlg);
                                if (DialogResult.OK == edSvc.ShowDialog(fsdlg))
                                {
                                    foreach (FillStyle fs in fsArray)
                                    {
                                        value = UpdateReturnValue(fs, fsdlg);
                                    }
                                    foreach (object o in ar)
                                    {
                                        TypeDescriptor.GetProperties(o, false)[context.PropertyDescriptor.Name].SetValue(o, value);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // get exact property name
                        // -----------------------
                        // FillStyle is used for defining BackgroundStyle also
                      
                        FillStyle fs = (FillStyle)TypeDescriptor.GetProperties(
                            context.Instance, false)[context.PropertyDescriptor.Name].GetValue(context.Instance);

                        FillStyleDialog fsdlg = new FillStyleDialog();
                        SetEditorProps(fs, fsdlg);

                        if (DialogResult.OK == edSvc.ShowDialog(fsdlg))
                        {
                            value = UpdateReturnValue(fs, fsdlg);
                            TypeDescriptor.GetProperties(context.Instance)[context.PropertyDescriptor.Name].SetValue(context.Instance, value);
                        }
                    }
                }
            }

            return value;
        }

        /// <summary>
        /// Gets the editor style used by the <see cref="M:System.Drawing.Design.UITypeEditor.EditValue(System.IServiceProvider,System.Object)"/> method.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that can be used to gain additional context information.</param>
        /// <returns>
        /// A <see cref="T:System.Drawing.Design.UITypeEditorEditStyle"/> value that indicates the style of editor used by the <see cref="M:System.Drawing.Design.UITypeEditor.EditValue(System.IServiceProvider,System.Object)"/> method. If the <see cref="T:System.Drawing.Design.UITypeEditor"/> does not support this method, then <see cref="M:System.Drawing.Design.UITypeEditor.GetEditStyle"/> will return <see cref="F:System.Drawing.Design.UITypeEditorEditStyle.None"/>.
        /// </returns>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context != null && context.Instance != null)
            {
                return UITypeEditorEditStyle.Modal;
            }
            return base.GetEditStyle(context);
        }
        #endregion
    }
}
