//-------------------------------------------------------------------------------------------------
// <copyright file="GridFontInfo.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Syncfusion.Drawing;
using Syncfusion.Diagnostics;
using Syncfusion.Styles;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Implements the data store for the <see cref="GridFontInfo"/> object.
    /// </summary>
    /// <seealso cref="StyleInfoStore"/>
    [Serializable,
    StaticDataField("sd")]
    public class GridFontInfoStore : StyleInfoStore
    {
        // Consts
        private const int MaxOrientation = 360; // 9 bits
        private const int MaxSize = 512; // 10 bits
        private const int MaxUnit = 6; // 4 bits

        static StaticData sd = new StaticData(typeof(GridFontInfoStore), typeof(GridFontInfo), true);

        /// <summary>
        /// Provides information about the <see cref="GridFontInfo.Facename"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty FacenameProperty = sd.CreateStyleInfoProperty(typeof(string), "Facename");

        /// <summary>
        /// Provides information about the <see cref="GridFontInfo.Size"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty SizeProperty = sd.CreateStyleInfoProperty(typeof(short), "Size", MaxSize, true);

        /// <summary>
        /// Provides information about the <see cref="GridFontInfo.Bold"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty BoldProperty = sd.CreateStyleInfoProperty(typeof(bool), "Bold", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridFontInfo.Italic"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty ItalicProperty = sd.CreateStyleInfoProperty(typeof(bool), "Italic", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridFontInfo.Underline"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty UnderlineProperty = sd.CreateStyleInfoProperty(typeof(bool), "Underline", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridFontInfo.Strikeout"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty StrikeoutProperty = sd.CreateStyleInfoProperty(typeof(bool), "Strikeout", 1, true);

        /// <summary>
        /// Provides information about the <see cref="GridFontInfo.Orientation"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty OrientationProperty = sd.CreateStyleInfoProperty(typeof(short), "Orientation", MaxOrientation, true);

        /// <summary>
        /// Provides information about the <see cref="GridFontInfo.Unit"/> property. 
        /// </summary>
        public readonly static StyleInfoProperty UnitProperty = sd.CreateStyleInfoProperty(typeof(GraphicsUnit), "Unit", MaxUnit, true);

        /// <override/>
        protected override StaticData StaticDataStore
        {
            get { return sd; }
        }

        /// <overload>
        /// Initializes a <see cref="GridFontInfoStore"/>
        /// </overload>
        /// <summary>
        /// Initializes an empty <see cref="GridFontInfoStore"/>.
        /// </summary>
        public GridFontInfoStore()
        {
        }

        /// <summary>
        /// Initializes a new <see cref="GridFontInfoStore"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        private GridFontInfoStore(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
        }

        // Base class implementation of this method calls Activator.CreateInstance to achieve the same result.
        // I assume calling new directly is more efficient. Otherwise this override is obsolete.

        /// <override/>
        /// <summary>
        /// Creates an exact copy of the current object.
        /// </summary>
        /// <returns>Copied object.</returns>
        public override object Clone()
        {
            StyleInfoStore target = new GridFontInfoStore();
            CopyTo(target);
            return target;
        }

        static GridFontInfoStore()
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
    /// Provides a <see cref="StyleInfoSubObjectBase"/> object for font settings in a cell. 
    /// Each font property of the cell can be configured individually. <para/>
    /// Font properties that have not been initialized will inherit default 
    /// values from a base style.
    /// </summary>
    /// <example>
    /// The following code changes font information for cells:
    /// <code lang="C#">
    /// <para/>
    ///             model[rowIndex, colIndex].Font.Facename = "Arial";
    ///             model[rowIndex, colIndex].Font.Bold = true;
    /// </code>
    /// </example>
    [TypeConverter(typeof(StyleInfoBaseConverter))]
    [Editor(typeof(GridFontInfoEditor), typeof(UITypeEditor))]
    public class GridFontInfo : GridStyleInfoSubObject
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
        private static GridFontInfo defaultFont;

        internal static object CreateObject(StyleInfoSubObjectIdentity identity, object store)
        {
            if (store != null)
            {
                return new GridFontInfo(identity, store as GridFontInfoStore);
            }

            return new GridFontInfo(identity);
        }

        private WeakReference _font = null;

        /// <summary>
        /// Releases the all resources used by the Component.
        /// </summary>
        public override void Dispose()
        {
            // Do not call font.Dispose here, gdi font object might have been assigned to a TextBox or other cell type.
            _font = null;
            base.Dispose();
        }

        // Constructors

        /// <overload>
        /// Initializes a <see cref="GridFontInfo"/>
        /// </overload>
        /// <summary>
        /// Initializes a <see cref="GridFontInfo"/>
        /// </summary>
        [DebuggerStepThrough()]
        public GridFontInfo()
            : base(new GridFontInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new <see cref="GridFontInfo"/>  object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridFontInfo"/>.
        /// </param>
        [DebuggerStepThrough()]
        public GridFontInfo(StyleInfoSubObjectIdentity identity)
            : base(identity, new GridFontInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new <see cref="GridFontInfo"/>  object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridFontInfo"/></param>.
        /// <param name="store">A <see cref="GridFontInfoStore"/> that holds data for this <see cref="GridFontInfo"/>.
        /// All changes in this style object will saved in the <see cref="GridFontInfoStore"/> object.</param>
        [DebuggerStepThrough()]
        public GridFontInfo(StyleInfoSubObjectIdentity identity, GridFontInfoStore store)
            : base(identity, store)
        {
        }

        /// <summary>
        /// Initalizes a new <see cref="GridFontInfo"/>  object and initializes its FaceName, FontStyle, Size and
        /// Unit from the specified font object.
        /// </summary>
        /// <param name="font">A <see cref="System.Drawing.Font"/> with font information.
        /// </param>
        [DebuggerStepThrough()]
        public GridFontInfo(Font font)
            : base(new GridFontInfoStore())
        {
            Facename = font.FontFamily.Name;
            FontStyle = font.Style;
            if (font.Unit == GraphicsUnit.Inch && font.Size < 8)
            {
                Size = font.SizeInPoints;
                Unit = GraphicsUnit.Point;
            }
            else
            {
                Size = font.Size;
                Unit = font.Unit;
            }
        }

        // Base class implementation of this method calls Activator.CreateInstance to achieve the same result.
        // I assume calling new directly is more efficient. Otherwise this override is obsolete.

        /// <override/>
        /// <summary>
        /// Creates an exact copy of the current object.
        /// </summary>
        /// <param name="newOwner">The new owner style object for the copied object.</param>
        /// <param name="sip">Identifier for this object.</param>
        /// <returns>Copy of the current object.</returns>
        [DebuggerStepThrough()]
        public override IStyleInfoSubObject MakeCopy(StyleInfoBase newOwner, StyleInfoProperty sip)
        {
            return new GridFontInfo(newOwner.CreateSubObjectIdentity(sip), (GridFontInfoStore)Store.Clone());
        }

        /// <summary>
        /// Gets a default <see cref="GridFontInfo"/> to be used with a default style.
        /// </summary>
        /// <remarks>
        /// The <see cref="GridStyleInfo.Default"/> of the <see cref="GridStyleInfo"/> class
        /// will return the default border info that this method generates through its
        /// overriden version of <see cref="GetDefaultStyle"/>.
        /// </remarks>
        public static GridFontInfo Default
        {
            [DebuggerStepThrough()]
            get
            {
                if (defaultFont == null)
                {
                    defaultFont = new GridFontInfo();
                    Font font = Control.DefaultFont;
                    defaultFont.Facename = font.FontFamily.Name;
                    defaultFont.Size = font.Size;
                    defaultFont.Unit = font.Unit;
                    defaultFont.FontStyle = font.Style;
                    ////SetIncludeFlags(true);
                }

                return defaultFont;
            }
        }

        /// <summary>
        /// Gets the em-size of the specified font object in world-units.
        /// </summary>
        /// <param name="font">The font object.</param>
        /// <returns>The size in world units.</returns>
        /// <remarks>
        /// If you need to have the grid clip text in cells device-independent, thus
        /// making the print output look exactly the same as screen output you should
        /// specify GraphicsUnit.World for <see cref="GridFontInfo.Unit"/> of the standard
        /// styles <see cref="GridStyleInfo.Font"/> object.
        /// </remarks>
        /// <example>
        /// In the following code snippet, the GraphicsUnit for standard font for a grid control
        /// is change to GraphicsUnit.World.
        /// <code lang="C#">
        /// GridStyleInfo standard = model.BaseStylesMap["Standard"].StyleInfo;
        /// Font dfont = Control.DefaultFont;
        /// standard.Font.Unit = GraphicsUnit.World;
        /// standard.Font.Facename = dfont.Name;
        /// standard.Font.Size = GridFontInfo.SizeInWorldUnit(dfont);
        /// </code>
        /// <code lang="VB">
        /// Dim standard As GridStyleInfo = model.BaseStylesMap("Standard").StyleInfo
        /// Dim dfont As Font = Control.DefaultFont
        /// standard.Font.Unit = GraphicsUnit.World
        /// standard.Font.Facename = dfont.Name
        /// standard.Font.Size = GridFontInfo.SizeInWorldUnit(dfont)
        /// </code>
        /// </example>
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
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(typeof(GridFontInfo), ex))
                {
                    throw;
                }
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
        /// Gets or creates a cached GDI+ Font generated from font information of
        /// this <see cref="GridFontInfo"/> object.
        /// </summary>
        [Browsable(false)]
        [NotifyParentProperty(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Font GdipFont
        {
            ////[DebuggerStepThrough()] 
            get
            {
                Font font = null;
                if (_font != null && _font.IsAlive)
                {
                    font = (Font)_font.Target;
                }

                if (font == null)
                {
                    ////TraceUtil.TraceCurrentMethodInfo();
                    ////n = 0;
                    _font = new WeakReference(font = GetGdipFont());
                }
                ////else
                ////    TraceUtil.TraceCurrentMethodInfo(n++);
                return font;
            }
        }

        ////int n = 0;

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void ResetGdipFont()
        {
            _font = null;
        }

        Font GetGdipFont()
        {
            return FontUtil.CreateFont(Facename, (float)Size, FontStyle, Unit);
        }

        void TestGdipFont()
        {
            StyleInfoSubObjectIdentity id = this.Identity as StyleInfoSubObjectIdentity;
            GridStyleInfo cell = (id != null) ? id.Owner as GridStyleInfo : null;
            GridStyleInfoIdentity cellId = (cell != null) ? cell.CellIdentity as GridStyleInfoIdentity : null;
            GridVolatileData gdata = (cellId != null) ? cellId.Data as GridVolatileData : null;
            GridModel grid = (gdata != null) ? gdata.Grid : null;
            GridControlBase gridView = (grid != null) ? grid.ActiveGridView : null;
            if (gridView != null && gridView.IsDesignMode())
            {
                Font f = new Font(Facename, (float)Size, FontStyle, Unit);
            }
        }

        /// <override/>
        protected override void OnStyleChanged(StyleInfoProperty sip)
        {
            _font = null;
            base.OnStyleChanged(sip);
        }

        /// <summary>
        /// Override this method to return a default style object for your derived class.
        /// </summary>
        /// <returns>A default style object.</returns>
        /// <override/>
        protected override StyleInfoBase GetDefaultStyle()
        {
            return Default;
        }

        // Properties
        #region FontStyle
        private void SetFontStyle(FontStyle fontStyle)
        {
            Bold = (fontStyle & FontStyle.Bold) != 0;
            Italic = (fontStyle & FontStyle.Italic) != 0;
            Strikeout = (fontStyle & FontStyle.Strikeout) != 0;
            Underline = (fontStyle & FontStyle.Underline) != 0;
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
        /// Gets or sets style information for the font.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [MergableProperty(true)]
        [NotifyParentProperty(true)]
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
                    throw new Exception(this.FontStyle.ToString() + " is not available for " + Facename, ex);
                }
            }
        }

        #endregion
        #region Facename
        /// <summary>
        /// Gets or sets the face name of this <see cref="GridFontInfo"/> object.
        /// </summary>
        [TypeConverter(typeof(FontConverter.FontNameConverter)),
        Editor("System.Drawing.Design.FontNameEditor, System.Drawing.Design",
            "System.Drawing.Design.UITypeEditor, System.Drawing, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"),
        Browsable(true),
        Description("Gets or sets the face name of this font object."),
        SRCategory("StyleCategoryAppearance"),
        NotifyParentProperty(true)]
        public string Facename
        {
            [DebuggerStepThrough()]
            get
            {
                return (string)GetValue(GridFontInfoStore.FacenameProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridFontInfoStore.FacenameProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="Facename"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetFacename()
        {
            ResetValue(GridFontInfoStore.FacenameProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeFacename()
        {
            return HasValue(GridFontInfoStore.FacenameProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Facename"/> property has been initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasFacename
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridFontInfoStore.FacenameProperty);
            }
        }

        #endregion
        #region Size
        /// <summary>
        /// Gets or sets the size in pixels of this <see cref="GridFontInfo"/> object.
        /// </summary>
        [Browsable(true),
        Description("Gets or sets the size in pixels of this font object."),
        SRCategory("StyleCategoryAppearance"),
        NotifyParentProperty(true)]
        public float Size
        {
            [DebuggerStepThrough()]
            get
            {
                return (float)GetShortValue(GridFontInfoStore.SizeProperty) / 4.0f;
            }

            [DebuggerStepThrough()]
            set
            {
                if (value < 0.0f || value > MaxSizeF)
                {
                    throw new ArgumentOutOfRangeException("value", value, String.Format("Size must be between 0 and {0}", MaxSizeF));
                }

                SetValue(GridFontInfoStore.SizeProperty, (short)(value * 4.0f));
            }
        }

        /// <summary>
        /// Resets the <see cref="Size"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetSize()
        {
            ResetValue(GridFontInfoStore.SizeProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeSize()
        {
            return HasValue(GridFontInfoStore.SizeProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Size"/> property has been initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasSize
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridFontInfoStore.SizeProperty);
            }
        }

        #endregion
        #region Orientation
        /// <summary>
        /// Gets or sets the orientation of this <see cref="GridFontInfo"/> object.
        /// </summary>
        [Browsable(true),
        Description("Gets or sets the orientation of this font object."),
        SRCategory("StyleCategoryAppearance"),
        NotifyParentProperty(true)]
        public int Orientation
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridFontInfoStore.OrientationProperty);
            }

            [DebuggerStepThrough()]
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

                SetValue(GridFontInfoStore.OrientationProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="Orientation"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetOrientation()
        {
            ResetValue(GridFontInfoStore.OrientationProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeOrientation()
        {
            return HasValue(GridFontInfoStore.OrientationProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Orientation"/> property has been initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasOrientation
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridFontInfoStore.OrientationProperty);
            }
        }

        #endregion
        #region Bold
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="GridFontInfo"/> object is bold.
        /// </summary>
        [Browsable(true),
        Description("Gets or sets a value that indicates whether this font object is bold."),
        SRCategory("StyleCategoryAppearance"),
        NotifyParentProperty(true)]
        public bool Bold
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridFontInfoStore.BoldProperty) != 0;
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridFontInfoStore.BoldProperty, value ? 1 : 0);
                try
                {
                    TestGdipFont();
                }
                catch (Exception ex)
                {
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
            ResetValue(GridFontInfoStore.BoldProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeBold()
        {
            return HasValue(GridFontInfoStore.BoldProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Bold"/> property has been initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasBold
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridFontInfoStore.BoldProperty);
            }
        }

        #endregion
        #region Italic
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="GridFontInfo"/> object is italic.
        /// </summary>
        [Browsable(true),
        Description("Gets or sets a value that indicates whether this font object is italic."),
        SRCategory("StyleCategoryAppearance"),
        NotifyParentProperty(true)]
        public bool Italic
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridFontInfoStore.ItalicProperty) != 0;
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridFontInfoStore.ItalicProperty, value ? 1 : 0);
                try
                {
                    TestGdipFont();
                }
                catch (Exception ex)
                {
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
            ResetValue(GridFontInfoStore.ItalicProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeItalic()
        {
            return HasValue(GridFontInfoStore.ItalicProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Italic"/> property has been initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasItalic
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridFontInfoStore.ItalicProperty);
            }
        }

        #endregion
        #region Underline
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="GridFontInfo"/> object is underlined.
        /// </summary>
        [Browsable(true),
        Description("Gets or sets a value that indicates whether this font object is underlined."),
        SRCategory("StyleCategoryAppearance"),
        NotifyParentProperty(true)]
        public bool Underline
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridFontInfoStore.UnderlineProperty) != 0;
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridFontInfoStore.UnderlineProperty, value ? 1 : 0);
                try
                {
                    TestGdipFont();
                }
                catch (Exception ex)
                {
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
            ResetValue(GridFontInfoStore.UnderlineProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeUnderline()
        {
            return HasValue(GridFontInfoStore.UnderlineProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Underline"/> property has been initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasUnderline
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridFontInfoStore.UnderlineProperty);
            }
        }

        #endregion
        #region Strikeout
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="GridFontInfo"/> object 
        /// should draw a horizontal line through the text.
        /// </summary>
        [Browsable(true),
        Description("Gets or sets a value that indicates whether this font object should draw a horizontal line through the text."),
        SRCategory("StyleCategoryAppearance"),
        NotifyParentProperty(true)]
        public bool Strikeout
        {
            [DebuggerStepThrough()]
            get
            {
                return GetShortValue(GridFontInfoStore.StrikeoutProperty) != 0;
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(GridFontInfoStore.StrikeoutProperty, value ? 1 : 0);
                try
                {
                    TestGdipFont();
                }
                catch (Exception ex)
                {
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
            ResetValue(GridFontInfoStore.StrikeoutProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeStrikeout()
        {
            return HasValue(GridFontInfoStore.StrikeoutProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Strikeout"/> property has been initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasStrikeout
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridFontInfoStore.StrikeoutProperty);
            }
        }

        #endregion
        #region Unit
        /// <summary>
        /// Gets or sets the  graphics unit of this <see cref="GridFontInfo"/> object.
        /// </summary>
        [Browsable(true),
        Description("Gets or sets the graphics unit of this font object."),
        SRCategory("StyleCategoryAppearance"),
        TypeConverter(typeof(FontConverter.FontUnitConverter)),
        NotifyParentProperty(true)]
        public GraphicsUnit Unit
        {
            [DebuggerStepThrough()]
            get
            {
                return (GraphicsUnit)GetShortValue(GridFontInfoStore.UnitProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                if (!Enum.IsDefined(typeof(GraphicsUnit), value))
                {
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(GraphicsUnit));
                }

                SetValue(GridFontInfoStore.UnitProperty, (short)value);
            }
        }

        /// <summary>
        /// Resets the <see cref="Unit"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetUnit()
        {
            ResetValue(GridFontInfoStore.UnitProperty);
        }

        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        private bool ShouldSerializeUnit()
        {
            return HasValue(GridFontInfoStore.UnitProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Unit"/> property has been initialized.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasUnit
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(GridFontInfoStore.UnitProperty);
            }
        }
        #endregion
    }

    /// <summary>
    ///   Provides a user interface to select and configure a <see cref="GridFontInfo" /> object.
    /// </summary>
    public class GridFontInfoEditor : UITypeEditor
    {
        private FontDialog fontDialog;
        private GridFontInfo value;

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridFontInfoEditor()
            : base()
        {
        }

        /// <summary>
        ///   <para>Gets the editor style used by the <see cref="M:System.Drawing.Design.FontEditor.EditValue(System.ComponentModel.ITypeDescriptorContext,System.IServiceProvider,System.Object)" /> method.</para>
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that can be used to gain additional context information.</param>
        /// <returns>
        ///   <para>A <see cref="T:System.Drawing.Design.UITypeEditorEditStyle" /> value that
        /// indicates the style of editor used by <see cref="M:System.Drawing.Design.FontEditor.EditValue(System.ComponentModel.ITypeDescriptorContext,System.IServiceProvider,System.Object)" />. </para>
        /// </returns>
        public override /*UITypeEditor*/ UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        /// <summary>
        ///   <para>Edits the value of the specified object using the editor style
        /// indicated by <see cref="M:System.Drawing.Design.FontEditor.GetEditStyle(System.ComponentModel.ITypeDescriptorContext)" />.</para>
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that can be used to gain additional context information.</param>
        /// <param name="provider">An <see cref="T:System.IServiceProvider" /> that this editor can use to obtain services.</param>
        /// <param name="value">The object to edit.</param>
        /// <returns>
        ///   <para> The new value of the object. If the value of the object has not changed,
        /// this should return the same object that was passed to it.</para>
        /// </returns>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            this.value = value as GridFontInfo;
            if (provider != null)
            {
                IWindowsFormsEditorService service1 = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (service1 != null)
                {
                    if (this.fontDialog == null)
                    {
                        this.fontDialog = new FontDialog();
                        this.fontDialog.ShowApply = false;
                        this.fontDialog.ShowColor = false;
                        this.fontDialog.AllowScriptChange = false;
                        this.fontDialog.AllowSimulations = false;
                        this.fontDialog.AllowVectorFonts = false;
                        this.fontDialog.AllowVerticalFonts = false;
                    }

                    if (value != null)
                    {
                        this.value.ResetGdipFont();
                        this.fontDialog.Font = this.value.GdipFont;
                    }

                    IntPtr focusedWindow = NativeMethods.GetFocus();
                    try
                    {
                        if (this.fontDialog.ShowDialog() == DialogResult.OK)
                        {
                            GridFontInfo fntInfo = new GridFontInfo(this.fontDialog.Font);
                            if (this.fontDialog.Font.GdiVerticalFont)
                            {
                                fntInfo.Orientation = 90;
                            }

                            this.value.CopyFrom(fntInfo);
                        }
                    }
                    finally
                    {
                        if (focusedWindow != IntPtr.Zero)
                        {
                            NativeMethods.SetFocus(focusedWindow);
                        }
                    }
                }
            }

            value = this.value;
            this.value = null;
            return value;
        }
    }
}
