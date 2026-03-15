#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows.Forms;
using Syncfusion.Diagnostics;
using Syncfusion.Documentation;
using Syncfusion.Drawing;
using Syncfusion.Styles;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Implements the data store for the <see cref="ChartFontInfo"/> object.
    /// </summary>
    /// <seealso cref="StyleInfoStore"/>
    /// <internalonly/>
    [
    Serializable,
    StaticDataField("sd"),

    DocumentationExclude()
    ]

    public class ChartFontInfoStore : StyleInfoStore
    {

        // Consts
        private const int MaxOrientation = 360; // 9 bits
        private const int MaxSize = 512; // 10 bits
        private const int MaxUnit = 6; // 4 bits

        private static StaticData sd = new StaticData(typeof(ChartFontInfoStore), typeof(ChartFontInfo), true);

        /// <summary>
        /// Provides information about the <see cref="ChartFontInfo.Facename"/> property.
        /// </summary>
        public static readonly StyleInfoProperty FacenameProperty = sd.CreateStyleInfoProperty(typeof(string), "Facename");

        /// <summary>
        /// Provides information about the <see cref="ChartFontInfo.Size"/> property.
        /// </summary>
        public static readonly StyleInfoProperty SizeProperty = sd.CreateStyleInfoProperty(typeof(short), "Size", MaxSize, true);

        /// <summary>
        /// Provides information about the <see cref="ChartFontInfo.Bold"/> property.
        /// </summary>
        public static readonly StyleInfoProperty BoldProperty = sd.CreateStyleInfoProperty(typeof(bool), "Bold", 1, true);

        /// <summary>
        /// Provides information about the <see cref="ChartFontInfo.Italic"/> property.
        /// </summary>
        public static readonly StyleInfoProperty ItalicProperty = sd.CreateStyleInfoProperty(typeof(bool), "Italic", 1, true);

        /// <summary>
        /// Provides information about the <see cref="ChartFontInfo.Underline"/> property.
        /// </summary>
        public static readonly StyleInfoProperty UnderlineProperty = sd.CreateStyleInfoProperty(typeof(bool), "Underline", 1, true);

        /// <summary>
        /// Provides information about the <see cref="ChartFontInfo.Strikeout"/> property.
        /// </summary>
        public static readonly StyleInfoProperty StrikeoutProperty = sd.CreateStyleInfoProperty(typeof(bool), "Strikeout", 1, true);

        /// <summary>
        /// Provides information about the <see cref="ChartFontInfo.Orientation"/> property.
        /// </summary>
        public static readonly StyleInfoProperty OrientationProperty = sd.CreateStyleInfoProperty(typeof(short), "Orientation", MaxOrientation, true);

        /// <summary>
        /// Provides information about the <see cref="ChartFontInfo.Unit"/> property.
        /// </summary>
        public static readonly StyleInfoProperty UnitProperty = sd.CreateStyleInfoProperty(typeof(GraphicsUnit), "Unit", MaxUnit, true);

        /// <summary>
        /// Provides information about the <see cref="ChartFontInfo.FontFamilyTemplate"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty FontFamilyTemplateProperty = sd.CreateStyleInfoProperty(typeof(FontFamily), "FontFamilyTemplate");

        /// <override/>
        protected override StaticData StaticDataStore
        {
            get
            {
                return sd;
            }
        }

        /// <overload>
        /// Overloaded Contructor.
        /// </overload>
        /// <summary>
        /// Initializes an empty <see cref="ChartFontInfoStore"/>.
        /// </summary>
        public ChartFontInfoStore()
        {
        }

        /// <summary>
        /// Constructor. Initializes a new <see cref="ChartFontInfoStore"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        private ChartFontInfoStore(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            //TraceUtil.TraceCurrentMethodInfoIf(Switches.Serialization.TraceVerbose, info.FullTypeName, info.MemberCount);

        }

        // Base class implementation of this method calls Activator.CreateInstance to achieve the same result.
        // I assume calling new directly is more efficient. Otherwise this override is obsolete.

        /// <override/>
        public override object Clone()
        {
            StyleInfoStore target = new ChartFontInfoStore();
            CopyTo(target);
            return target;
        }

        static ChartFontInfoStore()
        {
            SizeProperty.Format += new StyleInfoPropertyConvertEventHandler(SizeToString);
            SizeProperty.Parse += new StyleInfoPropertyConvertEventHandler(SizeStringToInt);
            SizeProperty.WriteXml += new StyleInfoPropertyWriteXmlEventHandler(SizeProperty_WriteXml);
            SizeProperty.ReadXml += new StyleInfoPropertyReadXmlEventHandler(SizeProperty_ReadXml);
        }

        private static void SizeToString(object sender, StyleInfoPropertyConvertEventArgs cevent)
        {

            if (cevent.Handled)
            {
                return;
            }

            // We can only convert to string type. Test this using the DesiredType.
            if (cevent.DesiredType != typeof(string))
            {
                return;
            }

            // TODO: CultureInfo en-US so that it gets written with a .
            cevent.Value = (((short)cevent.Value) / 4.0f).ToString();
            cevent.Handled = true;
        }

        private static void SizeStringToInt(object sender, StyleInfoPropertyConvertEventArgs cevent)
        {

            if (cevent.Handled)
            {
                return;
            }

            // Convert the string back to decimal using the static Parse method.
            cevent.Value = (short)(float.Parse(cevent.Value.ToString()) * 4);
            cevent.Handled = true;
        }

        private static void SizeProperty_WriteXml(object sender, StyleInfoPropertyWriteXmlEventArgs e)
        {
            StyleInfoProperty sip = e.Sip;

            // <PropertyName>
            e.Writer.WriteStartElement(sip.PropertyName);

            e.Writer.WriteString(sip.FormatValue(e.Store.GetValue(sip)));

            // <PropertyName/>
            e.Writer.WriteEndElement();
            e.Handled = true;
        }

        private static void SizeProperty_ReadXml(object sender, StyleInfoPropertyReadXmlEventArgs e)
        {
            string xmlValue = e.Reader.ReadString();
            object value = e.Sip.ParseValue(xmlValue);
            e.Store.SetValue(e.Sip, value);
            e.Reader.Read(); // consume item end tag.
            e.Handled = true;
        }
    }

    /// <summary>
    /// Provides a <see cref="StyleInfoSubObjectBase"/> object for font settings associated with a point.
    /// Each font property of the point can be configured individually. <para/>
    /// Font properties that have not been initialized will inherit default
    /// values from a base style.
    /// </summary>
    /// <example>
    /// The following code changes font information for a point:
    /// <code lang="C#">
    ///
    ///            this.chart.Series[0].Styles[0].Font.Facename = "Arial";
    /// </code>
    /// </example>
    public class ChartFontInfo : ChartSubStyleInfoBase
    {

        // Consts
        private const int MaxOrientation = 360; // 9 bits
        private const int MaxSize = 512; // 10 bits
        private const float MaxSizeF = 512 / 4;
        private const int MaxUnit = 6; // 4 bits

        private const int AllFlags = int.MaxValue;
        private const char separator = ';';
        private const int FW_BOLD = 700;

        // Static Fields
        // FIX: OT d4716/TS #8543. Default value must be unique for each thread. 
        [ThreadStatic]
        private static ChartFontInfo defaultFont;

        internal static object CreateObject(StyleInfoSubObjectIdentity identity, object store)
        {

            if (store != null)
            {
                return new ChartFontInfo(identity, store as ChartFontInfoStore);
            }
            return new ChartFontInfo(identity);
        }

        private WeakReference _font = null;

        /// <summary>
        /// Clears all resources used by the component.
        /// </summary>
        public override void Dispose()
        {
            // Do not call font.Dispose here, gdi font object might have been assigned to a text box or other cell type.
            _font = null;
            base.Dispose();
        }

        // Constructors.
        /// <overload>
        /// Overloaded constructor.
        /// </overload>
        /// <summary>
        /// Initializes a <see cref="ChartFontInfo"/>.
        /// </summary>
        [DebuggerStepThrough()]
        public ChartFontInfo()
            : base(new ChartFontInfoStore())
        {
        }
        ~ChartFontInfo()
        {
            if (defaultFont != null)
                defaultFont = null;
        }
        /// <summary>
        /// Initalizes a new <see cref="ChartFontInfo"/> object and associates it with an existing <see cref="StyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="StyleInfoSubObjectIdentity"/> that holds the identity for this <see cref="ChartFontInfo"/>.
        /// </param>
        [DebuggerStepThrough()]
        public ChartFontInfo(StyleInfoSubObjectIdentity identity)
            : base(identity, new ChartFontInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new <see cref="ChartFontInfo"/> object and associates it with an existing <see cref="StyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="StyleInfoSubObjectIdentity"/> that holds the identity for this <see cref="ChartFontInfo"/>.
        /// <param name="store">A <see cref="ChartFontInfoStore"/> that holds data for this <see cref="ChartFontInfo"/>.
        /// All changes made in this style object will be saved in the <see cref="ChartFontInfoStore"/> object.</param>
        /// </param>
        [DebuggerStepThrough()]
        public ChartFontInfo(StyleInfoSubObjectIdentity identity, ChartFontInfoStore store)
            : base(identity, store)
        {
        }

        // Base class implementation of this method calls Activator.CreateInstance to achieve the same result.
        // I assume calling new directly is more efficient. Otherwise this override is obsolete.

        /// <override/>
        [DebuggerStepThrough()]
        public override IStyleInfoSubObject MakeCopy(StyleInfoBase newOwner, StyleInfoProperty sip)
        {
            return new ChartFontInfo(newOwner.CreateSubObjectIdentity(sip), (ChartFontInfoStore)Store.Clone());
        }

        /// <summary>
        /// Returns a default <see cref="ChartFontInfo"/> to be used with a default style.
        /// </summary>
        /// <remarks>
        /// The <see cref="ChartStyleInfo.Default"/> of the <see cref="ChartStyleInfo"/> class
        /// will return the default border info that this method generates through its
        /// overridden version of <see cref="GetDefaultStyle"/>.
        ///  </remarks>
        public static ChartFontInfo Default
        {
            [DebuggerStepThrough()]
            get
            {

                if (defaultFont == null)
                {
                    defaultFont = new ChartFontInfo();
                    Font font = Control.DefaultFont;
                    defaultFont.Facename = font.Name;
                    defaultFont.Size = font.Size;
                    defaultFont.Unit = font.Unit;
                    defaultFont.FontStyle = font.Style;
                    defaultFont.Orientation = 0;
                }

                return defaultFont;
            }
        }

        /// <summary>
        /// Gets the em-size of the specified font object in world-units.
        /// </summary>
        /// <param name="font">The font object.</param>
        /// <returns>The size in world units.</returns>
        public static float SizeInWorldUnit(Font font)
        {

            if (font.Unit == GraphicsUnit.World)
            {
                return font.Size;
            }

            float sizeInPoints = font.SizeInPoints;

            Font f2 = null;

            try
            {
                f2 = new Font(font.FontFamily, sizeInPoints * 10, FontStyle.Regular, GraphicsUnit.World);
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace, "Exception");

                if (ex.InnerException != null)
                    Debug.WriteLine(ex.InnerException.Message + Environment.NewLine + ex.InnerException.StackTrace, "Inner Exception");

                throw ex;
            }

            if (f2 == null)
            {
                f2 = FontUtil.CreateFont(font.Name, sizeInPoints * 10, FontStyle.Regular, GraphicsUnit.World);
            }

            float r = f2.SizeInPoints;
            f2.Dispose();

            return sizeInPoints * (sizeInPoints * 10) / r;
        }

        /// <summary>
        /// Creates or returns a cached GDI+ font generated from font information of
        /// this <see cref="ChartFontInfo"/> object.
        /// </summary>
        [Browsable(false)]
        public Font GdipFont
        {
            get
            {
                Font font = null;

                if (_font != null && _font.IsAlive)
                {
                    font = (Font)_font.Target;
                }

                if (font == null)
                {
                    _font = new WeakReference(font = GetGdipFont());
                }

                return font;
            }
        }

        internal void ResetGdipFont()
        {
            _font = null;
        }

        Font GetGdipFont()
        {
            FontFamily ff = this.FontFamilyTemplate;

            if (ff == null)
                return FontUtil.CreateFont(Facename, Size, FontStyle, Unit);

            else
            {
                Font _f = null;

                try
                {
                    _f = new Font(ff, Size, FontStyle, Unit);
                }

                catch (Exception ex)
                {
                    TraceUtil.TraceExceptionCatched(ex);

                    if (!ExceptionManager.RaiseExceptionCatched(typeof(FontUtil), ex))
                        throw ex;
                }

                if (_f == null)
                    _f = FontUtil.CreateFont(Facename, Size, FontStyle, Unit);

                return _f;
            }
        }

        private void TestGdipFont()
        {
        }

        /// <override/>
        protected override void OnStyleChanged(StyleInfoProperty sip)
        {
            _font = null;
            base.OnStyleChanged(sip);
        }

        /// <override/>
        protected override StyleInfoBase GetDefaultStyle()
        {
            return Default;
        }

        // Properties

        #region FontStyle

        private void SetFontStyle(FontStyle fontStyle)
        {
            Bold = ((fontStyle & FontStyle.Bold) != 0);
            Italic = ((fontStyle & FontStyle.Italic) != 0);
            Strikeout = ((fontStyle & FontStyle.Strikeout) != 0);
            Underline = ((fontStyle & FontStyle.Underline) != 0);
        }

        private FontStyle GetFontStyle()
        {
            FontStyle fontStyle = FontStyle.Regular;

            if (Bold)
            {
                fontStyle |= FontStyle.Bold;
            }

            if (Italic)
            {
                fontStyle |= FontStyle.Italic;
            }

            if (Strikeout)
            {
                fontStyle |= FontStyle.Strikeout;
            }

            if (Underline)
            {
                fontStyle |= FontStyle.Underline;
            }
            return fontStyle;
        }

        /// <summary>
        /// Gets or sets the style information for the font.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), MergableProperty(true)]

        public FontStyle FontStyle
        {
            [DebuggerStepThrough()]
            get
            {
                return GetFontStyle();
            }
            [DebuggerStepThrough()]
            set
            {
                SetFontStyle(value);

                try
                {
                    TestGdipFont();
                }

                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace, "Exception");

                    if (ex.InnerException != null)
                        Debug.WriteLine(ex.InnerException.Message + Environment.NewLine + ex.InnerException.StackTrace, "Inner Exception");

                    throw new Exception(this.FontStyle.ToString() + " is not available for " + Facename, ex);
                }
            }
        }

        #endregion

        #region Name

        /// <summary>
        /// Gets or sets the face name of this <see cref="ChartFontInfo"/> object.
        /// </summary>
        [
        TypeConverter(typeof(FontConverter.FontNameConverter)),
        Editor("System.Drawing.Design.FontNameEditor, System.Drawing.Design",
            "System.Drawing.Design.UITypeEditor, System.Drawing, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"),
        Browsable(true),
        Description("Gets or sets the face name of this font object."),
        Category("Appearance")
        ]

        public string Facename
        {
            [DebuggerStepThrough()]
            get
            {
                return (string)GetValue(ChartFontInfoStore.FacenameProperty);
            }
            [DebuggerStepThrough()]
            set
            {
                SetValue(ChartFontInfoStore.FacenameProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="Facename"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetName()
        {
            ResetValue(ChartFontInfoStore.FacenameProperty);
        }

        /// <summary>
        /// Indicates whether the Facename property should be serialized
        /// </summary>
        /// <returns></returns>
        /// <internalonly/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool ShouldSerializeName()
        {
            return HasValue(ChartFontInfoStore.FacenameProperty);
        }

        /// <summary>
        /// Indicates whether the <see cref="Facename"/> property has been initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasName
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartFontInfoStore.FacenameProperty);
            }
        }

        #endregion

        #region Size

        /// <summary>
        /// Gets or sets the size in pixels of this <see cref="ChartFontInfo"/> object.
        /// </summary>
        [
        Browsable(true),
        Description("Gets or sets the size in pixels of this font object."),
        Category("Appearance")
        ]

        public float Size
        {
            [DebuggerStepThrough()]
            get
            {
                return (float)GetShortValue(ChartFontInfoStore.SizeProperty) / 4.0f;
            }
            [DebuggerStepThrough()]
            set
            {

                if (value < 0.0f || value > MaxSizeF)
                {
                    throw new ArgumentOutOfRangeException("value", value, String.Format("Size must be between 0 and {0}.", MaxSizeF));
                }
                SetValue(ChartFontInfoStore.SizeProperty, (short)(value * 4.0f));
            }
        }

        /// <summary>
        /// Resets the <see cref="Size"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetSize()
        {
            ResetValue(ChartFontInfoStore.SizeProperty);
        }

        /// <summary>
        /// Indicates whether the Size property should be serialized
        /// </summary>
        /// <returns></returns>
        /// <internalonly/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool ShouldSerializeSize()
        {
            return HasValue(ChartFontInfoStore.SizeProperty);
        }

        /// <summary>
        /// Indicates whether the <see cref="Size"/> property has been initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasSize
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartFontInfoStore.SizeProperty);
            }
        }

        #endregion

        #region Orientation

        /// <summary>
        /// Gets or sets the orientation of this <see cref="ChartFontInfo"/> object.
        /// </summary>
        [
        Browsable(true),
        Description("Gets or sets the orientation of this font object."),
        Category("Appearance")
        ]

        public int Orientation
        {
            get
            {
                return GetShortValue(ChartFontInfoStore.OrientationProperty);
            }
            set
            {

                while (value < 0)
                {
                    value += 360;
                }

                while (value >= 360)
                {
                    value -= 360;
                }

                SetValue(ChartFontInfoStore.OrientationProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="Orientation"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetOrientation()
        {
            ResetValue(ChartFontInfoStore.OrientationProperty);
        }

        /// <summary>
        /// Indicates whether the Orientation property should be serialized
        /// </summary>
        /// <returns></returns>
        /// <internalonly/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool ShouldSerializeOrientation()
        {
            return HasValue(ChartFontInfoStore.OrientationProperty);
        }

        /// <summary>
        /// Indicates whether the <see cref="Orientation"/> property has been initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasOrientation
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartFontInfoStore.OrientationProperty);
            }
        }

        #endregion

        #region Bold

        /// <summary>
        /// Indicates whether this <see cref="ChartFontInfo"/> object is bold.
        /// </summary>
        [
        Browsable(true),
        Description("Gets or sets a value that indicates whether this font object is bold."),
        Category("Appearance")
        ]

        public bool Bold
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(ChartFontInfoStore.BoldProperty) != 0;
            }
            [DebuggerStepThrough()]
            set
            {
                SetValue(ChartFontInfoStore.BoldProperty, value ? 1 : 0);

                try
                {
                    TestGdipFont();
                }

                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace, "Exception");

                    if (ex.InnerException != null)
                        Debug.WriteLine(ex.InnerException.Message + Environment.NewLine + ex.InnerException.StackTrace, "Inner Exception");

                    throw new Exception(this.FontStyle.ToString() + " is not available for " + Facename, ex);
                }
            }
        }

        /// <summary>
        /// Resets the <see cref="Bold"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetBold()
        {
            ResetValue(ChartFontInfoStore.BoldProperty);
        }

        /// <summary>
        /// Indicates whether the Bold property should be serialized
        /// </summary>
        /// <returns></returns>
        /// <internalonly/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool ShouldSerializeBold()
        {
            return HasValue(ChartFontInfoStore.BoldProperty);
        }

        /// <summary>
        /// Indicates whether the <see cref="Bold"/> property has been initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasBold
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartFontInfoStore.BoldProperty);
            }
        }

        #endregion

        #region Italic

        /// <summary>
        /// Indicates whether this <see cref="ChartFontInfo"/> object is italic.
        /// </summary>
        [
        Browsable(true),
        Description("Gets or sets a value that indicates whether this font object is italic."),
        Category("Appearance")
        ]

        public bool Italic
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(ChartFontInfoStore.ItalicProperty) != 0;
            }
            [DebuggerStepThrough()]
            set
            {
                SetValue(ChartFontInfoStore.ItalicProperty, value ? 1 : 0);

                try
                {
                    TestGdipFont();
                }

                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace, "Exception");

                    if (ex.InnerException != null)
                        Debug.WriteLine(ex.InnerException.Message + Environment.NewLine + ex.InnerException.StackTrace, "Inner Exception");

                    throw new Exception(this.FontStyle.ToString() + " is not available for " + Facename, ex);
                }
            }
        }

        /// <summary>
        /// Resets the <see cref="Italic"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetItalic()
        {
            ResetValue(ChartFontInfoStore.ItalicProperty);
        }

        /// <summary>
        /// Indicates whether the Italic property should be serialized
        /// </summary>
        /// <returns></returns>
        /// <internalonly/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool ShouldSerializeItalic()
        {
            return HasValue(ChartFontInfoStore.ItalicProperty);
        }

        /// <summary>
        /// Indicates whether the <see cref="Italic"/> property has been initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasItalic
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartFontInfoStore.ItalicProperty);
            }
        }

        #endregion

        #region Underline

        /// <summary>
        /// Indicates whether this <see cref="ChartFontInfo"/> object is underlined.
        /// </summary>
        [
        Browsable(true),
        Description("Gets or sets a value that indicates whether this font object is underlined."),
        Category("Appearance")
        ]

        public bool Underline
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(ChartFontInfoStore.UnderlineProperty) != 0;
            }
            [DebuggerStepThrough()]
            set
            {
                SetValue(ChartFontInfoStore.UnderlineProperty, value ? 1 : 0);

                try
                {
                    TestGdipFont();
                }

                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace, "Exception");

                    if (ex.InnerException != null)
                        Debug.WriteLine(ex.InnerException.Message + Environment.NewLine + ex.InnerException.StackTrace, "Inner Exception");

                    throw new Exception(this.FontStyle.ToString() + " is not available for " + Facename, ex);
                }
            }
        }

        /// <summary>
        /// Resets the <see cref="Underline"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetUnderline()
        {
            ResetValue(ChartFontInfoStore.UnderlineProperty);
        }

        /// <summary>
        /// Indicates whether the Underline property should be serialized
        /// </summary>
        /// <returns></returns>
        /// <internalonly/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool ShouldSerializeUnderline()
        {
            return HasValue(ChartFontInfoStore.UnderlineProperty);
        }

        /// <summary>
        /// Indicates whether the <see cref="Underline"/> property has been initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasUnderline
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartFontInfoStore.UnderlineProperty);
            }
        }

        #endregion

        #region Strikeout

        /// <summary>
        /// Indicates whether this <see cref="ChartFontInfo"/> object
        /// should draw a horizontal line through the text.
        /// </summary>
        [
        Browsable(true),
        Description("Gets or sets a value that indicates whether this font object should draw a horizontal line through the text."),
        Category("Appearance")
        ]

        public bool Strikeout
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(ChartFontInfoStore.StrikeoutProperty) != 0;
            }
            [DebuggerStepThrough()]
            set
            {
                SetValue(ChartFontInfoStore.StrikeoutProperty, value ? 1 : 0);

                try
                {
                    TestGdipFont();
                }

                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace, "Exception");

                    if (ex.InnerException != null)
                        Debug.WriteLine(ex.InnerException.Message + Environment.NewLine + ex.InnerException.StackTrace, "Inner Exception");

                    throw new Exception(this.FontStyle.ToString() + " is not available for " + Facename, ex);
                }
            }
        }

        /// <summary>
        /// Resets the <see cref="Strikeout"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetStrikeout()
        {
            ResetValue(ChartFontInfoStore.StrikeoutProperty);
        }

        /// <summary>
        /// Indicates whether the Strikeout property should be serialized
        /// </summary>
        /// <returns></returns>
        /// <internalonly/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool ShouldSerializeStrikeout()
        {
            return HasValue(ChartFontInfoStore.StrikeoutProperty);
        }

        /// <summary>
        /// Indicates whether the <see cref="Strikeout"/> property has been initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasStrikeout
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartFontInfoStore.StrikeoutProperty);
            }
        }

        #endregion

        #region Unit

        /// <summary>
        /// Gets or sets the graphics unit for this <see cref="ChartFontInfo"/> object.
        /// </summary>
        [
        Browsable(true),
        Description("Gets or sets the graphics unit of this font object."),
        Category("Appearance"),
        TypeConverter(typeof(FontConverter.FontUnitConverter))
        ]

        public GraphicsUnit Unit
        {
            [DebuggerStepThrough()]
            get
            {
                return (GraphicsUnit)GetShortValue(ChartFontInfoStore.UnitProperty);
            }
            [DebuggerStepThrough()]
            set
            {

                if (!Enum.IsDefined(typeof(GraphicsUnit), value))
                {
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(GraphicsUnit));
                }
                SetValue(ChartFontInfoStore.UnitProperty, (short)(value));
            }
        }

        /// <summary>
        /// Resets the <see cref="Unit"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetUnit()
        {
            ResetValue(ChartFontInfoStore.UnitProperty);
        }

        /// <summary>
        /// Indicates whether the Unit property should be serialized
        /// </summary>
        /// <returns></returns>
        /// <internalonly/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool ShouldSerializeUnit()
        {
            return HasValue(ChartFontInfoStore.UnitProperty);
        }

        /// <summary>
        /// Indicates whether the <see cref="Unit"/> property has been initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasUnit
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartFontInfoStore.UnitProperty);
            }
        }

        #endregion

        #region FontFamily

        /// <summary>
        /// Gets or sets the font family of this <see cref="ChartFontInfo"/> object.
        /// </summary>
        [
        Browsable(true),
        Description("Gets or sets the font family of this font object."),
        Category("Appearance"),
        ]

        public FontFamily FontFamilyTemplate
        {
            [DebuggerStepThrough()]
            get
            {
                return (FontFamily)this.GetValue(ChartFontInfoStore.FontFamilyTemplateProperty);
            }
            [DebuggerStepThrough()]
            set
            {
                SetValue(ChartFontInfoStore.FontFamilyTemplateProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="FontFamilyTemplate"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetFontFamilyTemplate()
        {
            ResetValue(ChartFontInfoStore.FontFamilyTemplateProperty);
        }

        /// <summary>
        /// Indicates whether the FontFamily property should be serialized
        /// </summary>
        /// <returns></returns>
        /// <internalonly/>
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool ShouldSerializeFontFamilyTemplate()
        {
            return HasValue(ChartFontInfoStore.FontFamilyTemplateProperty);
        }

        /// <summary>
        /// Indicates whether the <see cref="FontFamilyTemplate"/> property has been initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasFontFamilyTemplate
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartFontInfoStore.FontFamilyTemplateProperty);
            }
        }

        #endregion

    } ;
}