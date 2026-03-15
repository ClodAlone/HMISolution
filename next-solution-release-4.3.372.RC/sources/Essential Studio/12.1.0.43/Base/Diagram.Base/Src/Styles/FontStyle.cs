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
using System.Runtime.Serialization;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Encapsulates the font properties of an object.
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(FontStyleConverter))]
    public sealed class FontStyle
        : PropertyContainer
    {
        #region Clas constants
        private const string c_strTIMES_NEW_ROMAN = "Times New Roman";
        private const float c_fDEFAULT_FONT_SIZE = 8f;
        #endregion

        #region Class members
        private string m_strFontFamily;
        private string m_strName;
        private System.Drawing.FontStyle m_styleFont;
        private float m_fSize;
        private MeasureUnits m_gUnit;
        #endregion

        #region Class initalize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="FontStyle"/> class.
        /// </summary>
        public FontStyle()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FontStyle"/> class.
        /// </summary>
        /// <param name="src">The font style.</param>
        public FontStyle(FontStyle src)
            : base(src)
        {
            m_strFontFamily = src.m_strFontFamily;
            m_strName = src.m_strName;
            m_styleFont = src.m_styleFont;
            m_fSize = src.m_fSize;
            m_gUnit = src.m_gUnit;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FontStyle"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        private FontStyle(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            m_strFontFamily = info.GetString("fontFamily");
            m_strName = info.GetString("name");
            m_styleFont = (System.Drawing.FontStyle)info.GetValue("fontStyle", typeof(System.Drawing.FontStyle));
            m_fSize = (float)info.GetValue("size", typeof(float));
            m_gUnit = (MeasureUnits)info.GetValue("unit", typeof(MeasureUnits));
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets Font family name.
        /// </summary>
        [Browsable(true)]
        [DefaultValue("Times New Roman")]
        [Description("Gets the FontFamily object associated with this Font object.")]
        [TypeConverter(typeof(FontFamilyConverter))]
        public string Family
        {
            get 
            { 
                return m_strFontFamily; 
            }
            set
            {
                if (m_strFontFamily != value && CanSetStyle(value, m_styleFont) && OnPropertyChanging(DPN.Family, value))
                {
                    //// make history record
                    RecordPropertyChanged(DPN.Family);
                    //// assign new value
                    m_strFontFamily = value;
                    //// raise property changed event
                    OnPropertyChanged(DPN.Family);
                }
            }
        }

        /// <summary>
        /// Gets or sets Name of the font.
        /// </summary>
        [Browsable(false)]
        [DefaultValue("Times New Roman")]
        [Description("Gets the face name of this Font object.")]
        [Obsolete("This property will be no more supported since next version.Use FamilyName instead.")]
        public string Name
        {
            get 
            { 
                return m_strName; 
            }
            set
            {
                if (m_strName != value && OnPropertyChanging(DPN.Name, value))
                {
                    //// make history record
                    RecordPropertyChanged(DPN.Name);
                    //// assign new value
                    m_strName = value;
                    //// raise property changed event
                    OnPropertyChanged(DPN.Name);
                }
            }
        }

        /// <summary>
        /// Gets or sets Style of the font.
        /// </summary>
        [Browsable(false)]
        public System.Drawing.FontStyle Style
        {
            get 
            { 
                return m_styleFont; 
            }
            set
            {
                if (m_styleFont != value && OnPropertyChanging(DPN.Style, value))
                {
                    //// make history record
                    RecordPropertyChanged(DPN.Style);
                    //// assign new value
                    m_styleFont = value;
                    //// raise property changed event
                    OnPropertyChanged(DPN.Style);
                }
            }
        }

        /// <summary>
        /// Gets or sets Size of font in logical units.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(8f)]
        [Description("Gets the em-size of this Font object measured in the unit of this Font object.")]
        public float Size
        {
            get 
            { 
                return m_fSize; 
            }
            set
            {
                if (value == 0)
                {
                    throw new ArgumentException("Font size cannot be set to zero");
                }

                if (m_fSize != value && OnPropertyChanging(DPN.Size, value))
                {
                    //// make history record
                    RecordPropertyChanged(DPN.Size);
                    //// assign new value
                    m_fSize = value;
                    //// raise property changed event
                    OnPropertyChanged(DPN.Size);
                }
            }
        }

        /// <summary>
        /// Gets or sets Size of font in points.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Performs conversion if FontStyle.Unit is not point.
        /// </para>
        /// </remarks>
        [Browsable(false)]
        public float PointSize
        {
            get
            {
                float fValueToReturn;

                if (this.Unit == MeasureUnits.Point)
                {
                    fValueToReturn = m_fSize;
                }
                else
                {
                    fValueToReturn = MeasureUnitsConverter.Convert(m_fSize, this.Unit, MeasureUnits.Point);
                }

                return fValueToReturn;
            }
            set
            {
                if (this.Unit == MeasureUnits.Point)
                {
                    this.Size = value;
                }
                else
                {
                    this.Size = MeasureUnitsConverter.Convert(value, MeasureUnits.Point, this.Unit);
                }
            }
        }

        /// <summary>
        /// Gets or sets Unit of measure for font size (default is points).
        /// </summary>
        [Browsable(true)]
        [Description("Logical unit of measurement.")]
        public MeasureUnits Unit
        {
            get 
            { 
                return m_gUnit; 
            }
            set
            {
                if (m_gUnit != value && OnPropertyChanging(DPN.Unit, value))
                {
                    //// make history record
                    RecordPropertyChanged(DPN.Unit);
                    float fSize = MeasureUnitsConverter.Convert(this.Size, this.Unit, value);
                    //// assign new value
                    m_gUnit = value;

                    this.Size = fSize;
                    //// raise property changed event
                    OnPropertyChanged(DPN.Unit);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the font has the Bold style set.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [Description("Indicates whether this Font object is bold.")]
        public bool Bold
        {
            get 
            { 
                return ((this.Style & System.Drawing.FontStyle.Bold) == System.Drawing.FontStyle.Bold); 
            }
            set
            {
                if (this.Bold != value)
                {
                    System.Drawing.FontStyle style = m_styleFont;
                    SetStyle(ref style, System.Drawing.FontStyle.Bold, value);

                    if (CanSetStyle(this.Family, style) && OnPropertyChanging(DPN.Bold, value))
                    {
                        // make history record
                        RecordPropertyChanged(DPN.Bold);

                        // assign new value
                        m_styleFont = style;

                        // raise property changed event
                        OnPropertyChanged(DPN.Bold);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the font has the Italic style set.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [Description("Indicates whether this Font object is italic.")]
        public bool Italic
        {
            get 
            { 
                return ((this.Style & System.Drawing.FontStyle.Italic) == System.Drawing.FontStyle.Italic); 
            }
            set
            {
                if (this.Italic != value)
                {
                    System.Drawing.FontStyle style = m_styleFont;
                    SetStyle(ref style, System.Drawing.FontStyle.Italic, value);

                    if (CanSetStyle(this.Family, style) && OnPropertyChanging(DPN.Italic, value))
                    {
                        // make history record
                        RecordPropertyChanged(DPN.Italic);

                        // assign new value
                        m_styleFont = style;

                        // raise property changed event
                        OnPropertyChanged(DPN.Italic);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the font has the Underline style set.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [Description("Indicates whether this Font object is underlined.")]
        public bool Underline
        {
            get 
            { 
                return ((this.Style & System.Drawing.FontStyle.Underline) == System.Drawing.FontStyle.Underline); 
            }
            set
            {
                if (this.Underline != value)
                {
                    System.Drawing.FontStyle style = m_styleFont;
                    SetStyle(ref style, System.Drawing.FontStyle.Underline, value);

                    if (CanSetStyle(this.Family, style) && OnPropertyChanging(DPN.Underline, value))
                    {
                        // make history record
                        RecordPropertyChanged(DPN.Underline);

                        // assign new value
                        m_styleFont = style;

                        // raise property changed event
                        OnPropertyChanged(DPN.Underline);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the font has the Strikeout style set.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [Description("Indicates whether this Font object is strikeout.")]
        public bool Strikeout
        {
            get 
            { 
                return ((this.Style & System.Drawing.FontStyle.Strikeout) == System.Drawing.FontStyle.Strikeout); 
            }
            set
            {
                if (this.Strikeout != value)
                {
                    System.Drawing.FontStyle style = m_styleFont;
                    SetStyle(ref style, System.Drawing.FontStyle.Strikeout, value);

                    if (CanSetStyle(this.Family, style) && OnPropertyChanging(DPN.Strikeout, value))
                    {
                        // make history record
                        RecordPropertyChanged(DPN.Strikeout);

                        // assign new value
                        m_styleFont = style;

                        // raise property changed event
                        OnPropertyChanged(DPN.Strikeout);
                    }
                }
            }
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
        #endregion

        #region Class utility methods
        /// <summary>
        /// Creates a font object matching the properties of this FontStyle.
        /// </summary>
        /// <returns>Font object.</returns>
        public Font CreateFont()
        {
            float fFontSize = MeasureUnitsConverter.ToPixelX(this.Size, this.Unit);

            try
            {
                return new Font(this.Family, fFontSize, this.Style, GraphicsUnit.Pixel);
            }
            finally
            {
                
            }
        }
        #endregion

        #region Class helper methods
        private void Initialize()
        {
            m_styleFont = System.Drawing.FontStyle.Regular;
            m_strName = c_strTIMES_NEW_ROMAN;
            m_strFontFamily = c_strTIMES_NEW_ROMAN;
            m_fSize = c_fDEFAULT_FONT_SIZE;
            m_gUnit = MeasureUnits.Point;
        }

        /// <summary>
        /// Set the font style flag.
        /// </summary>
        /// <param name="style">The font style.</param>
        /// <param name="flag">The style flag.</param>
        /// <param name="bValue">Then flag value].</param>
        private void SetStyle(ref System.Drawing.FontStyle style, System.Drawing.FontStyle flag, bool bValue)
        {
            if (bValue)
            {
                style |= flag;
            }
            else
            {
                style = style & (~flag);
            }
        }

        /// <summary>
        /// Gets the name of the property container.
        /// </summary>
        /// <returns>The property container name.</returns>
        protected override string GetPropertyContainerName()
        {
            return DPN.FontStyle;
        }

        /// <summary>
        /// Determines whether font style can be changed.
        /// </summary>
        /// <param name="strFamilyName">The family name.</param>
        /// <param name="style">The font style.</param>
        /// <returns>
        /// <c>true</c> if font style can be changed; otherwise, <c>false</c>.
        /// </returns>
        private bool CanSetStyle(string strFamilyName, System.Drawing.FontStyle style)
        {
            bool bSuccess = false;
            FontFamily family = null;

            try
            {
                family = new FontFamily(strFamilyName);
                bSuccess = family.IsStyleAvailable(style);
            }
            catch (ArgumentException)
            { 
            }
            finally
            {
                if (family != null)
                {
                    family.Dispose();
                }
            }

            return bSuccess;
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
            return new FontStyle(this);
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

            info.AddValue("fontFamily", m_strFontFamily);
            info.AddValue("name", m_strName);
            info.AddValue("fontStyle", m_styleFont);
            info.AddValue("size", m_fSize);
            info.AddValue("unit", m_gUnit);
        }
        #endregion
    }
}
