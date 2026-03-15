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

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// A ShadowStyle is a collection of properties that define how shadows
    /// are displayed for a filled shape.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ShadowStyleConverter))]
    [Editor(typeof(ShadowStyleValueEditor), typeof(UITypeEditor))]
    public class ShadowStyle
        : PropertyContainer
    {
        #region Class members
        private bool m_bVisible;
        private float m_fOffsetX;
        private float m_fOffsetY;
        private FillStyle m_fillstyle;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="ShadowStyle"/> class.
        /// </summary>
        public ShadowStyle()
            : base()
        {
            m_bVisible = false;
            m_fOffsetX = CommonUsedValues.SHADOW_OFFSET;
            m_fOffsetY = CommonUsedValues.SHADOW_OFFSET;
            m_fillstyle = new FillStyle();
            DefaultFillStyle();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShadowStyle"/> class.
        /// </summary>
        /// <param name="src">The SRC.</param>
        public ShadowStyle(ShadowStyle src)
            : base(src)
        {
            m_bVisible = src.m_bVisible;
            m_fOffsetX = src.m_fOffsetX;
            m_fOffsetY = src.m_fOffsetY;
            m_fillstyle = new FillStyle(src.m_fillstyle);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShadowStyle"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected ShadowStyle(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            m_fillstyle = new FillStyle();
            DefaultFillStyle();
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "fillstyle":
                        m_fillstyle = (FillStyle)info.GetValue("fillstyle", typeof(FillStyle));
                        break;
                    case "visible":
                        m_bVisible = info.GetBoolean("visible");
                        break;
                    case "offsetX":
                        m_fOffsetX = info.GetSingle("offsetX");
                        break;
                    case "offsetY":
                        m_fOffsetY = info.GetSingle("offsetY");
                        break;
                }
            }
        }

        private void DefaultFillStyle()
        {
            this.ForeColor = Color.FromArgb(128, CommonUsedValues.SHADOW_COLOR);
            this.Color = Color.FromArgb(128, CommonUsedValues.SHADOW_COLOR);
        }

        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the shadow color.
        /// </summary>
        /// <value>The color.</value>
        [Browsable(true)]
        [Description("The Color used for the fill.")]
        public Color Color
        {
            get 
            { 
                return this.m_fillstyle.Color; 
            }
            set
            {
                if (this.m_fillstyle.Color != value && OnPropertyChanging(DPN.Color, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.Color);

                    // assign new value
                    this.m_fillstyle.Color = value;

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
        [DefaultValue(128)]
        public int ColorAlphaFactor
        {
            get 
            { 
                return this.m_fillstyle.ColorAlphaFactor; 
            }
            set
            {
                if (value < CommonUsedValues.TRANSPARENT || value > CommonUsedValues.OPAQUE)
                    throw new ArgumentOutOfRangeException("ColorAlphaFactor");

                if (this.m_fillstyle.ColorAlphaFactor != value && OnPropertyChanging(DPN.ColorAlphaFactor, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.ColorAlphaFactor);
                    
                    // assign new value
                    this.m_fillstyle.ColorAlphaFactor = value;

                    this.m_fillstyle.ColorAlphaFactor = value;
                    
                    // raise property changed event
                    OnPropertyChanged(DPN.ColorAlphaFactor);
                }
            }
        }

        /// <summary>
        /// Gets or sets alpha blending factor for the ForeColor.
        /// </summary>
        [Browsable(true)]
        [Description("Alpha blending factor ( 0 = transparent, 255 = opaque )")]
        [DefaultValue(128)]
        public int ForeColorAlphaFactor
        {
            get 
            { 
                return this.m_fillstyle.ForeColorAlphaFactor; 
            }
            set
            {
                if (value < CommonUsedValues.TRANSPARENT || value > CommonUsedValues.OPAQUE)
                    throw new ArgumentOutOfRangeException("ForeColorAlphaFactor");

                if (this.m_fillstyle.ForeColorAlphaFactor != value && OnPropertyChanging(DPN.ForeColorAlphaFactor, value))
                {
                    //// make history record
                    RecordPropertyChanged(DPN.ForeColorAlphaFactor);
                    //// assign new value
                    this.m_fillstyle.ForeColorAlphaFactor = value;
                    //// raise property changed event
                    OnPropertyChanged(DPN.ForeColorAlphaFactor);
                }
            }
        }

        /// <summary>
        /// Gets or sets foreground color used for the fill style.
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
                return this.m_fillstyle.ForeColor; 
            }
            set
            {
                if (this.m_fillstyle.ForeColor != value && OnPropertyChanging(DPN.ForeColor, value))
                {
                    //// make history record
                    RecordPropertyChanged(DPN.ForeColor);
                    //// assign new value
                    this.m_fillstyle.ForeColor = value;
                    //// raise property changed event
                    OnPropertyChanged(DPN.ForeColor);
                }
            }
        }

        /// <summary>
        /// Gets or sets image to use for texture fill.
        /// </summary>
        [Browsable(false)]
        [RefreshProperties(RefreshProperties.All)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Image to use for texture fill.")]
        [DefaultValue(null)]
        public Image Texture
        {
            get 
            { 
                return this.m_fillstyle.Texture; 
            }
            set
            {
                if (this.m_fillstyle.Texture != value && OnPropertyChanging(DPN.Texture, value))
                {
                    //// make history record
                    RecordPropertyChanged(DPN.Texture);
                    //// assign new value
                    this.m_fillstyle.Texture = value;
                    //// raise property changed event
                    OnPropertyChanged(DPN.Texture);
                }
            }
        }

        /// <summary>
        /// Gets or sets the mode how the texture is wrapped if fill type is set to texture.
        /// </summary>
        [Browsable(false)]
        [Description("How to wrap the texture.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(WrapMode.Tile)]
        public WrapMode TextureWrapMode
        {
            get 
            { 
                return this.m_fillstyle.TextureWrapMode; 
            }
            set
            {
                if (this.m_fillstyle.TextureWrapMode != value && OnPropertyChanging(DPN.TextureWrapMode, value))
                {
                    //// make history record
                    RecordPropertyChanged(DPN.TextureWrapMode);
                    //// assign new value
                    this.m_fillstyle.TextureWrapMode = value;
                    //// raise property changed event
                    OnPropertyChanged(DPN.TextureWrapMode);
                }
            }
        }

        /// <summary>
        /// Gets or sets the path brush style.
        /// </summary>
        /// <value>The path brush style.</value>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Documentation.DocumentationExclude()]
        public PathGradientBrushStyle PathBrushStyle
        {
            get 
            { 
                return this.m_fillstyle.PathBrushStyle; 
            }
            set
            {
                if (this.m_fillstyle.PathBrushStyle != value && OnPropertyChanging(DPN.PathBrushStyle, value))
                {
                    //// make history record
                    RecordPropertyChanged(DPN.PathBrushStyle);
                    //// assign new value
                    this.m_fillstyle.PathBrushStyle = value;
                    //// raise property changed event
                    OnPropertyChanged(DPN.PathBrushStyle);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether current instance inherit container measure units.
        /// </summary>
        /// <value>
        /// <c>true</c> if current instance inherit container measure units; otherwise, <c>false</c>.
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

        /// <summary>
        /// Gets or sets a value indicating whether the shadow is visible or not.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [Description("Indicates whether node is visible.")]
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
                    //// make history record
                    RecordPropertyChanged(DPN.Visible);
                    //// assign new value
                    m_bVisible = value;
                    //// raise property changed event
                    OnPropertyChanged(DPN.Visible);
                }
            }
        }

        /// <summary>
        /// Gets or sets the distance of the shadow offset from the shape along
        /// the X axis.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(8f)]
        [Description("Determines the distance the shadow is offset from the shape along the X axis.")]
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
                    //// make history record
                    RecordPropertyChanged(DPN.OffsetX);
                    //// assign new value
                    m_fOffsetX = value;
                    //// raise property changed event
                    OnPropertyChanged(DPN.OffsetX);
                }
            }
        }

        /// <summary>
        /// Gets or sets the distance of the shadow offset from the shape along
        /// the Y axis.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(8f)]
        [Description("Determines the distance the shadow is offset from the shape along the Y axis.")]
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
                    //// make history record
                    RecordPropertyChanged(DPN.OffsetY);
                    //// assign new value
                    m_fOffsetY = value;
                    //// raise property changed event
                    OnPropertyChanged(DPN.OffsetY);
                }
            }
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Creates a brush for filling the shadow.
        /// </summary>
        /// <param name="gfx">The graphics.</param>
        /// <param name="fillrect">The fill rect.</param>
        /// <returns>Brush matching the Shadow style.</returns>
        public Brush CreateBrush(Graphics gfx, RectangleF fillrect)
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddRectangle(fillrect);

            PathGradientBrush br = new PathGradientBrush(gp);
            br.WrapMode = WrapMode.Clamp;
            ColorBlend colorBlend = new ColorBlend(3);
            colorBlend.Colors = new Color[] { Color.Transparent, Color.FromArgb(this.ForeColorAlphaFactor, this.ForeColor), Color.FromArgb(this.ColorAlphaFactor, this.Color) };
            colorBlend.Positions = new float[] { 0f, .1f, 1f };
            br.InterpolationColors = colorBlend;
            return br;
        }

        /// <summary>
        /// Gets the name of the property container.
        /// </summary>
        /// <returns>Property container name.</returns>
        protected override string GetPropertyContainerName()
        {
            return "ShadowStyle";
        }
        #endregion

        #region Class helper methods

        #region designer serialization helpers
        /// <summary>
        /// Serialize the color.
        /// </summary>
        /// <returns>true, if serialize color.</returns>
        protected bool ShouldSerializeColor()
        {
            return (Color.LightGray != this.Color);
        }
        #endregion

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
            return new ShadowStyle(this);
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

            info.AddValue("visible", m_bVisible);
            info.AddValue("offsetX", m_fOffsetX);
            info.AddValue("offsetY", m_fOffsetY);
            info.AddValue("fillstyle", m_fillstyle);
        }

        /// <summary>
        /// Called when measure units changing.
        /// </summary>
        /// <param name="from">The old value.</param>
        /// <param name="to">The new value.</param>
        protected override void OnMeasureUnitsChanging(MeasureUnits from, MeasureUnits to)
        {
            base.OnMeasureUnitsChanging(from, to);
            m_fOffsetX = MeasureUnitsConverter.ConvertX(m_fOffsetX, from, to);
            m_fOffsetY = MeasureUnitsConverter.ConvertY(m_fOffsetY, from, to);
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
            ShadowStyle ss = obj as ShadowStyle;
            if (ss != null)
            {
                bool b = TypeDescriptor.GetConverter(ss).ConvertToString(ss) == TypeDescriptor.GetConverter(this).ConvertToString(this);
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

        /// <summary>
        /// Updates the service references.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public override void UpdateServiceReferences(IServiceReferenceProvider provider)
        {
            base.UpdateServiceReferences(provider);
            if (this.m_fillstyle != null)
                this.m_fillstyle.UpdateServiceReferences(provider);

			//Commenting out as this throws an exception for some customers
            //if (provider == null)
            //    this.m_fillstyle = null;
        }
        #endregion
    }

    /// <summary>
    /// Class for shadow style value editor.
    /// </summary>
    public class ShadowStyleValueEditor : System.Drawing.Design.UITypeEditor
    {
        #region Class members
        private IWindowsFormsEditorService edSvc = null;
        #endregion

        #region Class utility methods
        /// <summary>
        /// Sets the editor properties.
        /// </summary>
        /// <param name="editingInstance">The editing instance.</param>
        /// <param name="editor">The editor.</param>
        protected virtual void SetEditorProps(ShadowStyle editingInstance, ShadowStyleDialog editor)
        {
            editor.ShadowStyle.Color = editingInstance.Color;
            editor.ShadowStyle.ForeColor = editingInstance.ForeColor;
            editor.ShadowStyle.ColorAlphaFactor = editingInstance.ColorAlphaFactor;
            editor.ShadowStyle.ForeColorAlphaFactor = editingInstance.ForeColorAlphaFactor;
            editor.ShadowStyle.PathBrushStyle = editingInstance.PathBrushStyle;
            editor.ShadowStyle.OffsetX = editingInstance.OffsetX;
            editor.ShadowStyle.OffsetY = editingInstance.OffsetY;
            editor.ShadowStyle.Visible = editingInstance.Visible;
        }
        private static object UpdateReturnValue(ShadowStyle ss, ShadowStyleDialog ssdlg)
        {
            ss.Color = ssdlg.ShadowStyle.Color;
            ss.ForeColor = ssdlg.ShadowStyle.ForeColor;
            ss.ColorAlphaFactor = ssdlg.ShadowStyle.ColorAlphaFactor;
            ss.ForeColorAlphaFactor = ssdlg.ShadowStyle.ForeColorAlphaFactor;
            ss.OffsetX = ssdlg.ShadowStyle.OffsetX;
            ss.OffsetY = ssdlg.ShadowStyle.OffsetY;
            ss.Visible = ssdlg.ShadowStyle.Visible;
            return ss;
        }
        #endregion

        #region Class overrides methods
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
                            ShadowStyle[] ssArray = new ShadowStyle[ar.Length];
                            int i = 0;
                            foreach (object o in ar)
                            {
                                ShadowStyle ss = null;
                                
                                // get exact property name
                                // -----------------------
                                // FillStyle is used for defining BackgroundStyle also
                                
                                ss = (ShadowStyle)TypeDescriptor.GetProperties(o, false)[context.PropertyDescriptor.Name].GetValue(o);
                                if (ss != null)
                                {
                                    ssArray[i] = ss;
                                    i++;
                                }
                            }
                            if (ssArray.Length > 0)
                            {
                                ShadowStyleDialog ssdlg = new ShadowStyleDialog();
                                SetEditorProps(ssArray[0], ssdlg);
                                if (DialogResult.OK == edSvc.ShowDialog(ssdlg))
                                {
                                    foreach (ShadowStyle ss in ssArray)
                                    {
                                        value = UpdateReturnValue(ss, ssdlg);
                                    }
                                    foreach (object o in ar)
                                    {
                                        TypeDescriptor.GetProperties(o, false)["ShadowStyle"].SetValue(o, value);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        ShadowStyle ss = (ShadowStyle)TypeDescriptor.GetProperties(
                            context.Instance, false)["ShadowStyle"].GetValue(context.Instance);

                        ShadowStyleDialog fsdlg = new ShadowStyleDialog();
                        SetEditorProps(ss, fsdlg);

                        if (DialogResult.OK == edSvc.ShowDialog(fsdlg))
                        {
                            value = UpdateReturnValue(ss, fsdlg);
                            TypeDescriptor.GetProperties(context.Instance)["ShadowStyle"].SetValue(context.Instance, value);
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
