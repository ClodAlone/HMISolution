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

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

using Syncfusion.Documentation;
using Syncfusion.Drawing;
using Syncfusion.Styles;
using System;
using System.Collections.Generic;


namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Abstract implementation of StyleInfoBase.
    /// </summary>
    public abstract class ChartStyleInfoBase : StyleInfoBase
    {
        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this instance should cache values
        /// for resolved base style properties.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public bool CacheValues
        {
            get
            {
                return base.CacheValues;
            }

            set
            {
                base.CacheValues = value;
            }
        }

        /// <summary>
        ///     A list of listeners that will be referenced using a WeakReference. The listeners
        ///     must implement the Syncfusion.Styles.IStyleChanged interface. When this style
        ///     object Syncfusion.Styles.StyleInfoBase.OnStyleChanged(Syncfusion.Styles.StyleInfoProperty)
        ///     method is called it will then loop through all objects in this list and call
        ///     each objects Syncfusion.Styles.IStyleChanged.StyleChanged(Syncfusion.Styles.StyleChangedEventArgs)
        ///     method.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public List<WeakReference> WeakReferenceChangedListeners
        {
            get
            {
                return base.WeakReferenceChangedListeners;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartStyleInfoBase"/> class.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <param name="store">The store.</param>
        public ChartStyleInfoBase(StyleInfoIdentityBase identity, StyleInfoStore store)
            : base(identity, store)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartStyleInfoBase"/> class.
        /// </summary>
        /// <param name="store">The store.</param>
        public ChartStyleInfoBase(StyleInfoStore store)
            : base(store)
        {
        }
        #endregion
    }

    /// <summary>
    /// Abstract implementation of StyleInfoSubObjectBase.
    /// </summary>
    public abstract class ChartSubStyleInfoBase : StyleInfoSubObjectBase
    {
        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this instance should cache values
        /// for resolved base style properties.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public bool CacheValues
        {
            get
            {
                return base.CacheValues;
            }

            set
            {
                base.CacheValues = value;
            }
        }

        /// <summary>
        ///     A list of listeners that will be referenced using a WeakReference. The listeners
        ///     must implement the Syncfusion.Styles.IStyleChanged interface. When this style
        ///     object Syncfusion.Styles.StyleInfoBase.OnStyleChanged(Syncfusion.Styles.StyleInfoProperty)
        ///     method is called it will then loop through all objects in this list and call
        ///     each objects Syncfusion.Styles.IStyleChanged.StyleChanged(Syncfusion.Styles.StyleChangedEventArgs)
        ///     method.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public List<WeakReference> WeakReferenceChangedListeners
        {
            get
            {
                return base.WeakReferenceChangedListeners;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartStyleInfoBase"/> class.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <param name="store">The store.</param>
        public ChartSubStyleInfoBase(StyleInfoSubObjectIdentity identity, StyleInfoStore store)
            : base(identity, store)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartStyleInfoBase"/> class.
        /// </summary>
        /// <param name="store">The store.</param>
        public ChartSubStyleInfoBase(StyleInfoStore store)
            : base(store)
        {
        }
        #endregion
    }

    
    /// <summary>
    /// This class contains appearance information for each ChartPoint <see cref="ChartPoint"/>.
    /// </summary>
    public class ChartStyleInfo : ChartStyleInfoBase
    {
        #region Constants
        private static readonly ChartStyleInfo c_defaultStyle = null;
        /// <summary>
        /// An empty style object.
        /// </summary>
        public static readonly ChartStyleInfo Empty = new ChartStyleInfo();
        #endregion
        private string m_Url=null;
        
        #region Constructor
        /// <summary>
        /// Initializes the <see cref="ChartStyleInfo"/> class.
        /// </summary>
        static ChartStyleInfo()
        {
            ChartStyleInfo defaultStyle = new ChartStyleInfo();

            defaultStyle = new ChartStyleInfo();
            defaultStyle.TextColor = SystemColors.WindowText;
            defaultStyle.BaseStyle = "Standard";
            defaultStyle.AltTagFormat = "";
            defaultStyle.Font = ChartFontInfo.Default;
            defaultStyle.Border = ChartLineInfo.Default;

            defaultStyle.Text = string.Empty;
            defaultStyle.TextFormat = string.Empty;
            defaultStyle.DisplayText = false;
            defaultStyle.DrawTextShape = false;
            defaultStyle.TextShape = ChartCustomShapeInfo.Default;
            defaultStyle.TextOffset = 2.5f;

            defaultStyle.ToolTip = "";
            defaultStyle.ToolTipFormat = string.Empty;

            defaultStyle.Images = null;
            defaultStyle.ImageIndex = -1;
            defaultStyle.Symbol = ChartSymbolInfo.Default;
            defaultStyle._System = false;
            defaultStyle._Name = "";
            defaultStyle.TextOrientation = ChartTextOrientation.Center;
            defaultStyle.DisplayShadow = false;

            Color shadowColor = ControlPaint.DarkDark(defaultStyle.TextColor);
            BrushInfo shadowBrush = new BrushInfo(shadowColor);
            defaultStyle.ShadowInterior = new BrushInfo(100, shadowBrush);
            defaultStyle.ShadowOffset = new Size(3, 2);

            defaultStyle.SetValue(ChartStyleInfoStore.HighlightOnMouseOverProperty, false);
            defaultStyle.HitTestRadius = 7.5f;
            defaultStyle.Label = "";
            defaultStyle.PointWidth = 1f;

            defaultStyle.SetValue(ChartStyleInfoStore.ElementBordersProperty, ChartBordersInfo.Default);
            defaultStyle.RelatedPoints = null;

            c_defaultStyle = defaultStyle;
            
        }

        /// <summary>
        /// Overloaded constructor. Initializes a new style object.
        /// </summary>
        [DebuggerStepThrough()]
        public ChartStyleInfo()
            : base(new ChartStyleInfoStore())
        {
        }

        /// <summary>
        /// Initializes a new style object and copies all data from an existing style object.
        /// </summary>
        /// <param name="style">The style object that contains the original data.</param>
        [DebuggerStepThrough()]
        public ChartStyleInfo(ChartStyleInfo style)
            : base(style.Store)
        {
        }

        /// <summary>
        /// Initializes a new style object and associates it with an existing <see cref="ChartStyleInfoStore"/>.
        /// </summary>
        /// <param name="store">A <see cref="ChartStyleInfoStore"/> that holds data for this <see cref="ChartStyleInfo"/>.
        /// All changes made in this style object will be saved in the <see cref="ChartStyleInfoStore"/> object.</param>
        [DebuggerStepThrough()]
        public ChartStyleInfo(ChartStyleInfoStore store)
            : base(store)
        {
        }

        /// <summary>
        /// Initializes a new style object and associates it with an existing <see cref="ChartStyleInfoIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="ChartStyleInfoIdentity"/> that holds the identity for this <see cref="ChartStyleInfo"/>.
        /// </param>
        [DebuggerStepThrough()]
        public ChartStyleInfo(StyleInfoIdentityBase identity)
            : base(identity, new ChartStyleInfoStore())
        {
        }

        /// <summary>
        /// Initializes a new style object and associates it with an existing <see cref="ChartStyleInfoIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="ChartStyleInfoIdentity"/> that holds the identity for this <see cref="ChartStyleInfo"/>.
        /// </param>
        /// <param name="store">A <see cref="ChartStyleInfoStore"/> that holds data for this <see cref="ChartStyleInfo"/>.
        /// All changes made in this style object will be saved in the <see cref="ChartStyleInfoStore"/> object.
        /// </param>
        [DebuggerStepThrough()]
        public ChartStyleInfo(StyleInfoIdentityBase identity, ChartStyleInfoStore store)
            : base(identity, store)
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the <see cref="ChartStyleInfoStore"/> object that holds all the data for this style object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ChartStyleInfoStore Store
        {
            get
            {
                return (ChartStyleInfoStore)base.Store;
            }
        }

        /// <summary>
        /// Override this method to create a product-specific identity object for a sub object.
        /// </summary>
        /// <param name="sip"></param>
        /// <returns>
        /// An identity object for a subobject of this style.
        /// </returns>
        /// <override/>
        [DebuggerStepThrough()]
        public override StyleInfoSubObjectIdentity CreateSubObjectIdentity(StyleInfoProperty sip)
        {
            return new StyleInfoSubObjectIdentity(this, sip);
        }

        /// <summary>
        /// Override this method to return a default style object for your derived class.
        /// </summary>
        /// <returns>A default style object.</returns>
        protected override StyleInfoBase GetDefaultStyle()
        {
            return c_defaultStyle;
        }

        /// <summary>
        /// Serializes this style to XML.
        /// </summary>
        /// <param name="xw"></param>
        public void WriteXmlSchema(XmlWriter xw)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ChartStyleInfo));
            serializer.Serialize(xw, this);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Returns a <see cref="ChartStyleInfo"/> with default settings.
        /// </summary>
        public static ChartStyleInfo Default
        {
            get
            {
                return  (ChartStyleInfo) c_defaultStyle.MemberwiseClone();
            }
        }

        ///<summary>
        /// Gets or sets custom shape in the background of the displaytext. 
        /// Before use this, enable "DrawTextShape" property of series style.
        /// </summary>
        [DefaultValue(typeof(DrawShape), "shape properties"), Category("Appearance")]
        public ChartCustomShapeInfo TextShape
        {
            [DebuggerStepThrough()]
            get 
            {
                return (ChartCustomShapeInfo)GetValue(ChartStyleInfoStore.TextShapeProperty);
            }
            set 
            {
                SetValue(ChartStyleInfoStore.TextShapeProperty, value);
            }
        }
        
        #region TextColor
        /// <summary>
        /// Gets or sets the color of the text that is to be rendered for a <see cref="ChartPoint"/>.
        /// </summary>
        [Description("The color of the text that is rendered at a ChartPoint."), Category("Appearance"),ChartTemplate(ChartTemplateSet.Simple)]
        public Color TextColor
        {
            get
            {
                return (Color)GetValue(ChartStyleInfoStore.TextColorProperty);
            }
            set
            {
                SetValue(ChartStyleInfoStore.TextColorProperty, value);
            }
        }

        /// <internalonly/>
        [DocumentationExclude()]
        public void ResetTextColor()
        {
            ResetValue(ChartStyleInfoStore.TextColorProperty);
        }

        private bool ShouldSerializeTextColor()
        {
            return HasValue(ChartStyleInfoStore.TextColorProperty);
        }

        /// <summary>
        /// Gets a value indicating whether this instance has text color.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has text color; otherwise, <c>false</c>.
        /// </value>
        /// <internalonly/>
        [DocumentationExclude()]
        public bool HasTextColor
        {
            get
            {
                return HasValue(ChartStyleInfoStore.TextColorProperty);
            }
        }
        #endregion TextColor

        #region BaseStyle

        /// <summary>
        /// Gets or sets the base style with default settings that is to be used 
        /// for the appearance of the <see cref="ChartPoint"/>.
        /// </summary>
        [Description("The base style with default settings for appearance of the ChartPoint."), Category("Style"), Browsable(false)]
        public string BaseStyle
        {
            get
            {
                return (string)GetValue(ChartStyleInfoStore.BaseStyleProperty);
            }

            set
            {
                SetValue(ChartStyleInfoStore.BaseStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the alt tag with default settings that is to be used. this is used in ASP.NET chart to define the format of "alt" tag value. 
        /// for the appearance of the <see cref="ChartPoint"/>.
        /// </summary>
        [Description("This is only for ASP.NET. which is used to to define the alt tag format in html page."), Category("Style"), Browsable(false)]
        public string AltTagFormat
        {
            get
            {
                return (string)GetValue(ChartStyleInfoStore.AltTagFormatProperty);
            }

            set
            {
                SetValue(ChartStyleInfoStore.AltTagFormatProperty, value);
            }
        }
        /// <summary>
        /// Resets the base style.
        /// </summary>
        /// <internalonly/>
        [DocumentationExclude()]
        public void ResetBaseStyle()
        {
            ResetValue(ChartStyleInfoStore.BaseStyleProperty);
        }

        private bool ShouldSerializeBaseStyle()
        {
            return HasValue(ChartStyleInfoStore.BaseStyleProperty);
        }

        /// <summary>
        /// Gets a value indicating whether this instance has base style.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has base style; otherwise, <c>false</c>.
        /// </value>
        /// <internalonly/>
        [DocumentationExclude()]
        public bool HasBaseStyle
        {
            get
            {
                return HasValue(ChartStyleInfoStore.BaseStyleProperty);
            }
        }

        #endregion // BaseStyle

        #region Font

        /// <summary>
        /// Creates or returns a cached GDI+ font generated from font information of
        /// the <see cref="ChartStyleInfo.Font"/> object.
        /// </summary>
        /// <value>The gdip font.</value>
        [Browsable(false)]
        public Font GdipFont
        {
            [DebuggerStepThrough()]
            get
            {

                if (HasFont && !Font.IsEmpty)
                {
                    return Font.GdipFont;
                }

                if (Identity != null)
                {
                    ChartStyleInfo fontInfo = this.Identity.GetBaseStyleNotEmptyExpandable(this, ChartStyleInfoStore.FontProperty) as ChartStyleInfo;

                    if (fontInfo != null)
                    {
                        return fontInfo.GdipFont;
                    }
                }

                return Default.GdipFont;
            }
        }

        /// <summary>
        /// Gets /sets the font that is to be used for drawing text.
        /// </summary>
        /// <value>The font.</value>
        [
        Description("The font for drawing text."),
        Browsable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Category("Appearance")
        ]
        public ChartFontInfo Font
        {
            [DebuggerStepThrough()]
            get
            {
                return (ChartFontInfo)GetValue(ChartStyleInfoStore.FontProperty);
            }

            set
            {
                SetValue(ChartStyleInfoStore.FontProperty, value);
            }
        }

        /// <summary>
        /// Resets font information.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetFont()
        {
            ResetValue(ChartStyleInfoStore.FontProperty);
        }

        /// <summary>
        /// Should the serialize font.
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeFont()
        {
            return HasValue(ChartStyleInfoStore.FontProperty);
        }

        /// <summary>
        /// Gets a value indicating whether font information has been initialized for the current object.
        /// </summary>
        /// <value><c>true</c> if this instance has font; otherwise, <c>false</c>.</value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasFont
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartStyleInfoStore.FontProperty);
            }
        }

        #endregion

        #region Border

        /// <summary>
        /// Creates or returns a cached GDI+ font generated from font information of
        /// the <see cref="ChartStyleInfo.Font"/> object.
        /// </summary>
        /// <value>The gdip pen.</value>
        [Browsable(false)]
        public Pen GdipPen
        {
            [DebuggerStepThrough()]
            get
            {
                if (HasBorder && !Border.IsEmpty)
                {
                    return Border.GdipPen;
                }

                if (Identity != null)
                {
                    ChartStyleInfo penInfo = this.Identity.GetBaseStyleNotEmptyExpandable(this, ChartStyleInfoStore.BorderProperty) as ChartStyleInfo;

                    if (penInfo != null)
                    {
                        return penInfo.GdipPen;
                    }
                }

                return Default.GdipPen;
            }
        }

        /// <summary>
        /// Gets or sets the information that is to be used for drawing lines.
        /// </summary>
        /// <value>The border.</value>
        [
        Description("Line information."),
        Browsable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Category("Appearance"), ChartTemplate(ChartTemplateSet.Content)
        ]
        public ChartLineInfo Border
        {
            [DebuggerStepThrough()]
            get
            {
                return (ChartLineInfo)GetValue(ChartStyleInfoStore.BorderProperty);
            }
            set
            {
                SetValue(ChartStyleInfoStore.BorderProperty, value);
            }
        }

        /// <summary>
        /// Resets line information.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetBorder()
        {
            ResetValue(ChartStyleInfoStore.BorderProperty);
        }

        /// <summary>
        /// Should the serialize border.
        /// </summary>
        /// <returns> True if the element should serialize otherwise false.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeBorder()
        {
            return HasValue(ChartStyleInfoStore.BorderProperty);
        }

        /// <summary>
        /// Gets a value indicating whether line information has been initialized for the current object.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has border; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasBorder
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartStyleInfoStore.BorderProperty);
            }
        }

        #endregion

        #region Interior

        /// <summary>
        /// Gets or sets a solid backcolor, gradient or pattern style with both back and forecolor for a <see cref="ChartPoint"/>'s background.
        /// </summary>
        /// <value>The interior.</value>
        [
        Description("Lets you specify a solid backcolor, gradient, or pattern style with both back and forecolor for a ChartPoint's background."),
        Browsable(true),
        Category("Appearance"), ChartTemplate(ChartTemplateSet.Simple)
        ]
        public BrushInfo Interior
        {
            [DebuggerStepThrough()]
            get
            {
                return (BrushInfo)GetValue(ChartStyleInfoStore.InteriorProperty);
            }

            set
            {
                SetValue(ChartStyleInfoStore.InteriorProperty, value);
            }
        }

        /// <summary>
        /// Resets interior information.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetInterior()
        {
            ResetValue(ChartStyleInfoStore.InteriorProperty);
        }
                
        /// <summary>
        /// Should the serialize interior.
        /// </summary>
        /// <returns>True if the element should serialize otherwise false.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeInterior()
        {
            return HasValue(ChartStyleInfoStore.InteriorProperty);
        }       

        /// <summary>
        /// Gets a value indicating whether interior information has been initialized for the current object.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has interior; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasInterior
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartStyleInfoStore.InteriorProperty);
            }
        }
                
        #endregion // Interior

        #region Custom Properties

        private ChartStyleInfoCustomPropertiesCollection cpl = null;

        /// <summary>
        /// Returns a collection of custom property objects that have
        /// at least one initialized value. The primary purpose of this
        /// collection is to support design-time code serialization of
        /// custom properties.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ChartStyleInfoCustomPropertiesCollection CustomProperties
        {
            get
            {

                if (cpl == null)
                {
                    cpl = new ChartStyleInfoCustomPropertiesCollection(this);
                }
                return cpl;
            }
        }

        private bool ShouldSerializeCustomProperties()
        {
            return CustomProperties.Count > 0;
        }

        #endregion Custom Properties

        #region Text

        /// <summary>
        /// Gets or sets the text that is to be associated with a <see cref="ChartPoint"/>. This text will be rendered at a position near the point if
        /// <see cref="DisplayText"/> is set to True.
        /// </summary>
        /// <value>The text.</value>
        [ChartTemplate(ChartTemplateSet.Simple),
        Description("The text associated with a ChartPoint."),
        Browsable(true),
        Category("Appearance")
        ]
        public string Text
        {
            [DebuggerStepThrough()]
            get
            {
                return (string)GetValue(ChartStyleInfoStore.TextProperty);
            }

            set
            {
                SetValue(ChartStyleInfoStore.TextProperty, value);
            }
        }

        /// <summary>
        /// Resets text information.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetText()
        {
            ResetValue(ChartStyleInfoStore.TextProperty);
        }

        /// <summary>
        /// Should the serialize text.
        /// </summary>
        /// <returns>True if the element should serialize otherwise false.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeText()
        {
            return HasValue(ChartStyleInfoStore.TextProperty);
        }

        /// <summary>
        /// Gets a value indicating whether text information has been initialized for the current object.
        /// </summary>
        /// <value><c>true</c> if this instance has text; otherwise, <c>false</c>.</value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasText
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartStyleInfoStore.TextProperty);
            }
        }
        
        #endregion // Text

        #region ToolTip

        /// <summary>
        /// Gets or sets the ToolTip that is to be associated with the <see cref="ChartPoint"/>.
        /// </summary>
        /// <value>The tool tip.</value>
        [ChartTemplate(ChartTemplateSet.Simple),
        Description("Specifies the ToolTip to be displayed for the associated ChartPoint."),
        Browsable(true),
        Category("Appearance")
        ]
        public string ToolTip
        {
            [DebuggerStepThrough()]
            get
            {
                return (string)GetValue(ChartStyleInfoStore.ToolTipProperty);
            }

            set
            {
                SetValue(ChartStyleInfoStore.ToolTipProperty, value);
            }
        }

        /// <summary>
        /// Resets ToolTip information.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetToolTip()
        {
            ResetValue(ChartStyleInfoStore.ToolTipProperty);
        }

        /// <summary>
        /// Should the serialize tool tip.
        /// </summary>
        /// <returns>True if the element should serialize otherwise false.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeToolTip()
        {
            return HasValue(ChartStyleInfoStore.ToolTipProperty);
        }

        /// <summary>
        /// Gets a value indicating whether ToolTip information has been initialized for the current object.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has tool tip; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasToolTip
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartStyleInfoStore.ToolTipProperty);
            }
        }

        #endregion // ToolTip

        #region ToolTipFormat

        /// <summary>
        /// Gets or sets the formatting that is to be applied to values that are displayed as ToolTips.
        /// </summary>
        /// <value>The tool tip format.</value>
        [ChartTemplate(ChartTemplateSet.Simple),
        Browsable(true),
        Category("Appearance")
        ]
        public string ToolTipFormat
        {
            [DebuggerStepThrough()]
            get
            {
                return (string)GetValue(ChartStyleInfoStore.ToolTipFormatProperty);
            }

            set
            {
                SetValue(ChartStyleInfoStore.ToolTipFormatProperty, value);
            }
        }

        /// <internalonly/>
        [DocumentationExclude(), DebuggerStepThrough()]
        public void ResetToolTipFormat()
        {
            ResetValue(ChartStyleInfoStore.ToolTipFormatProperty);
        }

        /// <summary>
        /// Should the serialize tool tip format.
        /// </summary>
        /// <returns>True if the element should serialize otherwise false.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeToolTipFormat()
        {
            return HasValue(ChartStyleInfoStore.ToolTipFormatProperty);
        }

        /// <summary>
        /// Gets a value indicating whether this instance has tool tip format.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has tool tip format; otherwise, <c>false</c>.
        /// </value>
        /// <internalonly/>
        [Browsable(false),
     DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),

     DocumentationExclude()]
        public bool HasToolTipFormat
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartStyleInfoStore.ToolTipFormatProperty);
            }
        }

        #endregion

        #region Images
        /// <summary>
        /// Gets or sets the imagelist that is to be associated with this <see cref="ChartPoint"/>. This property is used in conjunction with the
        /// <see cref="ImageIndex"/> property to display images associated with this point.
        /// </summary>
        /// <value>The images.</value>
        [ChartTemplate(ChartTemplateSet.Collection),
        Description("The imagelist associated with this ChartPoint. The ImageIndex property will be used in conjunction to determine the image displayed."),
        Browsable(true),
        Category("Appearance")
        ]
        public ChartImageCollection Images
        {
            [DebuggerStepThrough()]
            get
            {
                return (ChartImageCollection)GetValue(ChartStyleInfoStore.ImagesProperty);
            }

            set
            {
                SetValue(ChartStyleInfoStore.ImagesProperty, value);
            }
        }

        /// <summary>
        /// Resets ImageList information.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetImages()
        {
            ResetValue(ChartStyleInfoStore.ImagesProperty);
        }

        /// <summary>
        /// Should the serialize images.
        /// </summary>
        /// <returns>True if the element should serialize otherwise false.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeImages()
        {
            return HasValue(ChartStyleInfoStore.ImagesProperty);
        }

        /// <summary>
        /// Gets a value indicating whether ImageList information has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasImages
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartStyleInfoStore.ImagesProperty);
            }
        }

        #endregion // ImageList

        #region ImageIndex

        /// <summary>
        /// Gets or sets the image index from the associated <see cref="ImageList"/> property.
        /// </summary>
        /// <value>The index of the image.</value>
        [
        Description("Specifies the image index to be used from the image list associated with this ChartPoint."),
        Browsable(true),
        Category("Appearance")
        ]
        public int ImageIndex
        {
            [DebuggerStepThrough()]
            get
            {
                return (int)GetValue(ChartStyleInfoStore.ImageIndexProperty);
            }

            set
            {
                SetValue(ChartStyleInfoStore.ImageIndexProperty, value);
            }
        }

        /// <summary>
        /// Resets image index information.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetImageIndex()
        {
            ResetValue(ChartStyleInfoStore.ImageIndexProperty);
        }

        /// <summary>
        /// Should the index of the serialize image.
        /// </summary>
        /// <returns>True if the element should serialize otherwise false.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeImageIndex()
        {
            return HasValue(ChartStyleInfoStore.ImageIndexProperty);
        }

        /// <summary>
        /// Gets a value indicating whether image index information has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasImageIndex
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartStyleInfoStore.ImageIndexProperty);
            }
        }

        #endregion // ImageIndex

        #region Symbol

        /// <summary>
        /// Gets or sets the attributes of the symbol that is to be displayed at this point.
        /// </summary>
        /// <value>The symbol.</value>
        [
        Description("Specifies the attributes of the symbol that will be displayed at the ChartPoint."),
        Browsable(true),
        Category("Appearance")
        ]
        public ChartSymbolInfo Symbol
        {
            [DebuggerStepThrough()]
            get
            {
                return (ChartSymbolInfo)GetValue(ChartStyleInfoStore.SymbolProperty);
            }
            set
            {
                SetValue(ChartStyleInfoStore.SymbolProperty, value);
            }
        }

        /// <summary>
        /// Resets symbol information.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetSymbol()
        {
            ResetValue(ChartStyleInfoStore.SymbolProperty);
        }

        /// <summary>
        /// Should the serialize symbol.
        /// </summary>
        /// <returns>True if the element should serialize otherwise false.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeSymbol()
        {
            return HasValue(ChartStyleInfoStore.SymbolProperty);
        }

        /// <summary>
        /// Gets a value indicating whether symbol information has been initialized for the current object.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has symbol; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasSymbol
        {
            //[DebuggerStepThrough()]
            get
            {
                return HasValue(ChartStyleInfoStore.SymbolProperty);
            }
        }

        #endregion // Symbol

        #region BaseStyle attribute System

        /// <summary>
        /// Gets or sets a value indicating whether [_ system].
        /// </summary>
        /// <value><c>true</c> if [_ system]; otherwise, <c>false</c>.</value>
        [
        Browsable(true),
            //SRCategory("StyleCategoryAppearance")
        ]
        internal bool _System
        {
            [DebuggerStepThrough()]
            get
            {
                return (bool)GetValue(ChartStyleInfoStore.SystemProperty);
            }

            set
            {
                SetValue(ChartStyleInfoStore.SystemProperty, value);
            }
        }

        /// <internalonly/>
        [DebuggerStepThrough(), DocumentationExclude()]
        public void ResetSystem()
        {
            ResetValue(ChartStyleInfoStore.SystemProperty);
        }

        /// <summary>
        /// Should the serialize system.
        /// </summary>
        /// <returns>True if the element should serialize otherwise false.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeSystem()
        {
            return HasValue(ChartStyleInfoStore.SystemProperty);
        }

        /// <summary>
        /// Gets a value indicating whether [_ has system].
        /// </summary>
        /// <value><c>true</c> if [_ has system]; otherwise, <c>false</c>.</value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal bool _HasSystem
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartStyleInfoStore.SystemProperty);
            }
        }

        #endregion //BaseStyle attribute system

        #region BaseStyle attribute name

        /// <summary>
        /// Gets or sets the name of the _.
        /// </summary>
        /// <value>The name of the _.</value>
        [
        Browsable(true),
            //SRCategory("StyleCategoryAppearance")
        ]
        internal string _Name
        {
            [DebuggerStepThrough()]
            get
            {
                return (string)GetValue(ChartStyleInfoStore.NameProperty);
            }

            set
            {
                SetValue(ChartStyleInfoStore.NameProperty, value);
            }
        }

        /// <internalonly/>
        [
        DebuggerStepThrough(),

        DocumentationExclude()
        ]

        public void ResetName()
        {
            ResetValue(ChartStyleInfoStore.NameProperty);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeName()
        {
            return HasValue(ChartStyleInfoStore.NameProperty);
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal bool _HasName
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartStyleInfoStore.NameProperty);
            }
        }

        #endregion // BaseStyle attribute name

        #region TextOrientation

        /// <summary>
        /// Gets or sets the orientation of text that is to be displayed at this point.
        /// </summary>
        [ChartTemplate(ChartTemplateSet.Simple),Browsable(true), Category("Appearance")]
        public ChartTextOrientation TextOrientation
        {
            [DebuggerStepThrough()]
            get
            {
                return (ChartTextOrientation)GetValue(ChartStyleInfoStore.TextOrientationProperty);
            }

            set
            {
                SetValue(ChartStyleInfoStore.TextOrientationProperty, value);
            }
        }

        /// <internalonly/>
        [DebuggerStepThrough(), DocumentationExclude()]
        public void ResetTextOrientation()
        {
            ResetValue(ChartStyleInfoStore.TextOrientationProperty);
        }

        /// <summary>
        /// Should the serialize text orientation.
        /// </summary>
        /// <returns>True if the element should serialize otherwise false.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeTextOrientation()
        {
            return HasValue(ChartStyleInfoStore.TextOrientationProperty);
        }

        /// <summary>
        /// Gets a value indicating whether this instance has text orientation.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has text orientation; otherwise, <c>false</c>.
        /// </value>
        /// <internalonly/>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        DocumentationExclude()]
        public bool HasTextOrientation
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartStyleInfoStore.TextOrientationProperty);
            }
        }
        #endregion

        #region DisplayShadow

        /// <summary>
        /// Gets a value indicating whether a shadow should be rendered when this point is displayed.
        /// </summary>
        /// <value><c>true</c> if [display shadow]; otherwise, <c>false</c>.</value>
        [ChartTemplate(ChartTemplateSet.Simple),Browsable(true), Category("Appearance")]
        public bool DisplayShadow
        {
            [DebuggerStepThrough()]
            get
            {
                return (bool)GetValue(ChartStyleInfoStore.DisplayShadowProperty);
            }

            set
            {
                SetValue(ChartStyleInfoStore.DisplayShadowProperty, value);
            }
        }

        /// <internalonly/>
        [DebuggerStepThrough(), DocumentationExclude()]
        public void ResetDisplayShadow()
        {
            ResetValue(ChartStyleInfoStore.DisplayShadowProperty);
        }

        /// <summary>
        /// Should the serialize display shadow.
        /// </summary>
        /// <returns>True if the element should serialize otherwise false.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeDisplayShadow()
        {
            return HasValue(ChartStyleInfoStore.DisplayShadowProperty);
        }

        /// <internalonly/>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        DocumentationExclude()]
        public bool HasDisplayShadow
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartStyleInfoStore.DisplayShadowProperty);
            }
        }
        #endregion

        #region ShadowOffset
        /// <summary>
        /// Gets or sets the offset that is to be used when a shadow is rendered for this <see cref="ChartPoint"/>.
        /// </summary>
        [Browsable(true), Category("Appearance")]
        public Size ShadowOffset
        {
            [DebuggerStepThrough()]
            get
            {
                return (Size)GetValue(ChartStyleInfoStore.ShadowOffsetProperty);
            }

            set
            {
                SetValue(ChartStyleInfoStore.ShadowOffsetProperty, value);
            }
        }

        /// <internalonly/>
        [DebuggerStepThrough(), DocumentationExclude()]
        public void ResetShadowOffset()
        {
            ResetValue(ChartStyleInfoStore.ShadowOffsetProperty);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeShadowOffset()
        {
            return HasValue(ChartStyleInfoStore.ShadowOffsetProperty);
        }

        /// <summary>
        /// Gets a value indicating whether this style contains the local value of ShadowOffset property.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance contains the local value of ShadowOffset property; otherwise, <c>false</c>.
        /// </value>
        /// <internalonly/>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
         DocumentationExclude()]
        public bool HasShadowOffset
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartStyleInfoStore.ShadowOffsetProperty);
            }
        }
        #endregion

        #region ShadowInterior

        /// <summary>
        /// Gets or sets the interior attributes of the shadow displayed underneath this point.
        /// </summary>
        [Browsable(true), Category("Appearance"), ChartTemplate(ChartTemplateSet.Simple)]
        public BrushInfo ShadowInterior
        {
            [DebuggerStepThrough()]
            get
            {
                return (BrushInfo)GetValue(ChartStyleInfoStore.ShadowInteriorProperty);
            }

            set
            {
                SetValue(ChartStyleInfoStore.ShadowInteriorProperty, value);
            }
        }

        /// <internalonly/>
        [DebuggerStepThrough(),
     DocumentationExclude()]

        public void ResetShadowInterior()
        {
            ResetValue(ChartStyleInfoStore.ShadowInteriorProperty);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeShadowInterior()
        {
            return HasValue(ChartStyleInfoStore.ShadowInteriorProperty);
        }

        /// <summary>
        /// Gets a value indicating whether this style contains the local value of ShadowInterior property.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance contains the local value of ShadowInterior property; otherwise, <c>false</c>.
        /// </value>
        /// <internalonly/>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
     DocumentationExclude()]
        public bool HasShadowInterior
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartStyleInfoStore.ShadowInteriorProperty);
            }
        }

        #endregion

        /// <summary>
        /// Gets a value indicating whether this point should be highlighted when the mouse moves over it.
        /// </summary>

        #region HighlightOnMouseOver

        [Browsable(true),
        Category("Appearance")]
        [Obsolete("This property isn't used anymore."), EditorBrowsable(EditorBrowsableState.Never)]
        public bool HighlightOnMouseOver
        {
            [DebuggerStepThrough()]
            get
            {
                return (bool)GetValue(ChartStyleInfoStore.HighlightOnMouseOverProperty);
            }

            set
            {
                SetValue(ChartStyleInfoStore.HighlightOnMouseOverProperty, value);
            }
        }

        /// <internalonly/>
        [DebuggerStepThrough(),
     DocumentationExclude()]

        [Obsolete("This method isn't used anymore."), EditorBrowsable(EditorBrowsableState.Never)]
        public void ResetHighlightOnMouseOver()
        {
            ResetValue(ChartStyleInfoStore.HighlightOnMouseOverProperty);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeHighlightOnMouseOver()
        {
            return HasValue(ChartStyleInfoStore.HighlightOnMouseOverProperty);
        }

        /// <summary>
        /// Gets a value indicating whether this style contains the local value of HighlightOnMouseOver property.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance contains the local value of HighlightOnMouseOver property; otherwise, <c>false</c>.
        /// </value>
        /// <internalonly/>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
     DocumentationExclude()]
        [Obsolete("This property isn't used anymore."), EditorBrowsable(EditorBrowsableState.Never)]
        public bool HasHighlightOnMouseOver
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartStyleInfoStore.HighlightOnMouseOverProperty);
            }
        }

        #endregion

        /// <summary>
        /// Gets or sets the attributes of the brush that are to be used to highlight this point when the mouse moves over it and <see cref="HasHighlightOnMouseOver"/>
        /// is enabled.
        /// </summary>

        #region HighlightInterior

        [ChartTemplate(ChartTemplateSet.Simple),
        Browsable(true),
        Category("Appearance")
        ]

        public BrushInfo HighlightInterior
        {
            [DebuggerStepThrough()]
            get
            {
                return (BrushInfo)GetValue(ChartStyleInfoStore.HighlightInteriorProperty);
            }

            set
            {
                SetValue(ChartStyleInfoStore.HighlightInteriorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the attributes of the brush that are to be used to hide this point when the mouse moves over on other point.        
        /// </summary>
        [ChartTemplate(ChartTemplateSet.Simple),
        Browsable(true),
        Category("Appearance")
        ]
        public BrushInfo DimmedInterior
        {
            [DebuggerStepThrough()]
            get
            {
                return (BrushInfo)GetValue(ChartStyleInfoStore.DimmedInteriorProperty);
            }

            set
            {
                SetValue(ChartStyleInfoStore.DimmedInteriorProperty, value);
            }
        }

        /// <internalonly/>
        [DebuggerStepThrough(),
     DocumentationExclude()]

        public void ResetHihglightInterior()
        {
            ResetValue(ChartStyleInfoStore.HighlightInteriorProperty);
        }

        /// <internalonly/>
        [DebuggerStepThrough(),
     DocumentationExclude()]
        public void ResetDimmedInterior()
        {
            ResetValue(ChartStyleInfoStore.DimmedInteriorProperty);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeHighlightInterior()
        {
            return HasValue(ChartStyleInfoStore.HighlightInteriorProperty);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeDimmedInterior()
        {
            return HasValue(ChartStyleInfoStore.DimmedInteriorProperty);
        }

        /// <summary>
        /// Gets a value indicating whether this style contains the local value of HighlightInterior property.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance contains the local value of HighlightInterior property; otherwise, <c>false</c>.
        /// </value>
        /// <internalonly/>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
     DocumentationExclude()]
        public bool HasHighlightInterior
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartStyleInfoStore.HighlightInteriorProperty);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this style contains the local value of DimmedInterior property.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance contains the local value of DimmedInterior property; otherwise, <c>false</c>.
        /// </value>
        /// <internalonly/>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
     DocumentationExclude()]
        public bool HasDimmedInterior
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartStyleInfoStore.DimmedInteriorProperty);
            }
        }

        #endregion

        /// <summary>
        /// Controls the circle around this point that would be considered as being within the bounds of this
        /// point for hit-testing purposes.
        /// </summary>

        #region HitTestRadius

        [
        Browsable(true),
        Category("Appearance")
        ]

        public float HitTestRadius
        {
            [DebuggerStepThrough()]
            get
            {
                return (float)GetValue(ChartStyleInfoStore.HitTestRadiusProperty);
            }

            set
            {
                SetValue(ChartStyleInfoStore.HitTestRadiusProperty, value);
            }
        }

        /// <internalonly/>
        [DebuggerStepThrough(),
     DocumentationExclude()]

        public void ResetHitTestRadius()
        {
            ResetValue(ChartStyleInfoStore.HitTestRadiusProperty);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeHitTestRadius()
        {
            return HasValue(ChartStyleInfoStore.HitTestRadiusProperty);
        }

        /// <summary>
        /// Gets a value indicating whether this style contains the local value of HitTestRadius property.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance contains the local value of HitTestRadius property; otherwise, <c>false</c>.
        /// </value>
        /// <internalonly/>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
     DocumentationExclude()]
        public bool HasHitTestRadius
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartStyleInfoStore.HitTestRadiusProperty);
            }
        }

        #endregion

        #region Label

        /// <summary>
        /// Gets or sets the Label value.
        /// </summary>
        [ChartTemplate(ChartTemplateSet.Simple),
        Browsable(true),
        Category("Appearance")
        ]
        public string Label
        {
            [DebuggerStepThrough()]
            get
            {
                return (string)GetValue(ChartStyleInfoStore.LabelProperty);
            }

            set
            {
                SetValue(ChartStyleInfoStore.LabelProperty, value);
            }
        }

        /// <internalonly/>
        [DebuggerStepThrough(),
     DocumentationExclude()]
        public void ResetLabel()
        {
            ResetValue(ChartStyleInfoStore.LabelProperty);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeLabel()
        {
            return HasValue(ChartStyleInfoStore.LabelProperty);
        }

        /// <summary>
        /// Gets a value indicating whether this style contains the local value of Label property.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance contains the local value of Label property; otherwise, <c>false</c>.
        /// </value>
        /// <internalonly/>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
     DocumentationExclude()]
        public bool HasLabel
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartStyleInfoStore.LabelProperty);
            }
        }

        #endregion

        #region TextFormat

        /// <summary>
        /// Gets or sets the format that is to be applied to values that are displayed as text.
        /// </summary>
        [ChartTemplate(ChartTemplateSet.Simple),
        Browsable(true),
        Category("Appearance")
        ]
        public string TextFormat
        {
            [DebuggerStepThrough()]
            get
            {
                return (string)GetValue(ChartStyleInfoStore.TextFormatProperty);
            }

            set
            {
                SetValue(ChartStyleInfoStore.TextFormatProperty, value);
            }
        }        

        /// <internalonly/>
        [DebuggerStepThrough(),
     DocumentationExclude()]
        public void ResetTextFormat()
        {
            ResetValue(ChartStyleInfoStore.TextFormatProperty);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeTextFormat()
        {
            return HasValue(ChartStyleInfoStore.TextFormatProperty);
        }

        /// <summary>
        /// Gets a value indicating whether this style contains the local value of TextFormat property.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance contains the local value of TextFormat property; otherwise, <c>false</c>.
        /// </value>
        /// <internalonly/>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
     DocumentationExclude()]
        public bool HasTextFormat
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartStyleInfoStore.TextFormatProperty);
            }
        }

        #endregion

        #region Format

        /// <summary>
        /// Gets or sets the stringformat.
        /// </summary>
        /// <value>The format.</value>
        [ChartTemplate(ChartTemplateSet.Simple),
        Browsable(true),
        Category("Appearance")
        ]
        public StringFormat Format
        {
            [DebuggerStepThrough()]
            get
            {
                return (StringFormat)GetValue(ChartStyleInfoStore.FormatProperty);
            }

            set
            {
                SetValue(ChartStyleInfoStore.FormatProperty, value);
            }
        }

        /// <internalonly/>
        [DebuggerStepThrough(),
     DocumentationExclude()]
        public void ResetFormat()
        {
            ResetValue(ChartStyleInfoStore.FormatProperty);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeFormat()
        {
            return HasValue(ChartStyleInfoStore.FormatProperty);
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
     DocumentationExclude()]
        public bool HasFormat
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartStyleInfoStore.FormatProperty);
            }
        }

        #endregion

        #region DisplayText

        /// <summary>
        /// Gets or Sets whether text should be displayed at this point.
        /// </summary>
        [ChartTemplate(ChartTemplateSet.Simple),
        Browsable(true),
        Category("Appearance")
        ]
        public bool DisplayText
        {
            [DebuggerStepThrough()]
            get
            {
                return (bool)GetValue(ChartStyleInfoStore.DisplayTextProperty);
            }

            set
            {
                SetValue(ChartStyleInfoStore.DisplayTextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets whether text should be draw with shape in the background at this point.
        /// </summary>
        [ChartTemplate(ChartTemplateSet.Simple),
        Browsable(true),
        Category("Appearance")
        ]
        public bool DrawTextShape
        {
            [DebuggerStepThrough()]
            get
            {
                return (bool)GetValue(ChartStyleInfoStore.DrawTextShapeProperty);
            }

            set
            {
                SetValue(ChartStyleInfoStore.DrawTextShapeProperty, value);
            }
        }


        /// <internalonly/>
        [DebuggerStepThrough(),
     DocumentationExclude()]
        public void ResetDisplayText()
        {
            ResetValue(ChartStyleInfoStore.DisplayTextProperty);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeDisplayText()
        {
            return HasValue(ChartStyleInfoStore.DisplayTextProperty);
        }

        /// <summary>
        /// Gets a value indicating whether this style contains the local value of DisplayText property.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance contains the local value of DisplayText property; otherwise, <c>false</c>.
        /// </value>
        /// <internalonly/>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        DocumentationExclude()]
        public bool HasDisplayText
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartStyleInfoStore.DisplayTextProperty);
            }
        }

        #endregion //BaseStyle attribute system

        #region PointWidth

        /// <summary>
        /// Gets or sets the width of this point relative to the total width available. It is specially useful with Gantt charts to render
        /// series that overlap.
        /// </summary>
        [ChartTemplate(ChartTemplateSet.Simple),
        Browsable(true),
        Category("Appearance")
        ]
        public float PointWidth
        {
            [DebuggerStepThrough()]
            get
            {
                return (float)GetValue(ChartStyleInfoStore.PointWidthProperty);
            }

            set
            {
                SetValue(ChartStyleInfoStore.PointWidthProperty, value);
            }
        }

        /// <internalonly/>
        [DebuggerStepThrough(),
     DocumentationExclude()]

        public void ResetPointWidth()
        {
            ResetValue(ChartStyleInfoStore.PointWidthProperty);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializePointWidth()
        {
            return HasValue(ChartStyleInfoStore.PointWidthProperty);
        }

        /// <summary>
        /// Gets a value indicating whether this style contains the local value of PointWidth property.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance contains the local value of PointWidth property; otherwise, <c>false</c>.
        /// </value>
        /// <internalonly/>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
     DocumentationExclude()]
        public bool HasPointWidth
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartStyleInfoStore.PointWidthProperty);
            }
        }

        #endregion

        #region TextOffset

        /// <summary>
        /// Gets or sets the offset of the text from the position of the <see cref="ChartPoint"/>.
        /// </summary>
        [ChartTemplate(ChartTemplateSet.Simple),
        Browsable(true),
        Category("Appearance")
        ]
        public float TextOffset
        {
            [DebuggerStepThrough()]
            get
            {
                return (float)GetValue(ChartStyleInfoStore.TextOffsetProperty);
            }

            set
            {
                SetValue(ChartStyleInfoStore.TextOffsetProperty, value);
            }
        }

        /// <internalonly/>
        [DebuggerStepThrough(),
     DocumentationExclude()]
        public void ResetTextOffset()
        {
            ResetValue(ChartStyleInfoStore.TextOffsetProperty);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeTextOffset()
        {
            return HasValue(ChartStyleInfoStore.TextOffsetProperty);
        }

        /// <summary>
        /// Gets a value indicating whether this style contains the local value of TextOffset property.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance contains the local value of TextOffset property; otherwise, <c>false</c>.
        /// </value>
        /// <internalonly/>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
     DocumentationExclude()]
        public bool HasTextOffset
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartStyleInfoStore.TextOffsetProperty);
            }
        }
        #endregion

        #region ElementBorders

        /// <summary>
        /// Gets or sets the border settings for elements associated with the chart point. You can specify the inner and outer border. It is currently used
        /// only by symbols rendered by the ChartPoint.
        /// </summary>
        [
        Description("ChartPoint border settings. You can specify the inner and outer border for each ChartPoint."),
        Browsable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        EditorBrowsable(EditorBrowsableState.Never),
        Obsolete("This property isn't used anymore. Use the Symbol.Border property."),
        Category("Appearance"),
        ]

        public ChartBordersInfo ElementBorders
        {
            [DebuggerStepThrough()]
            get
            {
                return (ChartBordersInfo)GetValue(ChartStyleInfoStore.ElementBordersProperty);
            }

            set
            {
                SetValue(ChartStyleInfoStore.ElementBordersProperty, value);
            }
        }

        /// <summary>
        /// Resets border information.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetElementBorders()
        {
            ResetValue(ChartStyleInfoStore.ElementBordersProperty);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeElementBorders()
        {
            return HasValue(ChartStyleInfoStore.ElementBordersProperty);
        }

        /// <summary>
        /// Gets a value indicating whether this style contains the local value of ElementBorders property.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance contains the local value of ElementBorders property; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasElementBorders
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartStyleInfoStore.ElementBordersProperty);
            }
        }

        #endregion

        #region RelatedPoints

        /// <summary>
        /// Gets or sets the offset of the text from the position of the <see cref="ChartPoint"/>.
        /// </summary>
        [ChartTemplate(ChartTemplateSet.Simple),
        Browsable(true),
        Category("Appearance")
        ]
        public ChartRelatedPointInfo RelatedPoints
        {
            [DebuggerStepThrough()]
            get
            {
                return (ChartRelatedPointInfo)GetValue(ChartStyleInfoStore.RelatedPointsProperty);
            }

            set
            {
                SetValue(ChartStyleInfoStore.RelatedPointsProperty, value);
            }
        }

        /// <internalonly/>
        [DebuggerStepThrough(),
     DocumentationExclude()]
        public void ResetRelatedPoints()
        {
            ResetValue(ChartStyleInfoStore.RelatedPointsProperty);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeRelatedPoints()
        {
            return HasValue(ChartStyleInfoStore.RelatedPointsProperty);
        }


        /// <summary>
        /// Gets a value indicating whether this instance has related points.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has related points; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
     DocumentationExclude()]
        public bool HasRelatedPoints
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartStyleInfoStore.RelatedPointsProperty);
            }
        }
        #endregion
        /// <summary>
        /// Gets or sets the Url that is to be associated with a <see cref="ChartPoint"/>. This Url will be applied to the point if
        /// <see cref="EnableUrl"/> and <see cref="CalcRegion"/> property is set to True.This property is applicable only for ChartWeb.
		/// </summary>
        /// <value>The Url.</value>
        [ChartTemplate(ChartTemplateSet.Simple),Description("Gets or Sets the url."),Browsable(true)]

        public String Url
        {
            [DebuggerStepThrough()]
            get
            {
                return (String)GetValue(ChartStyleInfoStore.UrlProperty);
            }
            set
            {
                if (m_Url != value)
                    m_Url = value;
                if (m_Url.StartsWith("www."))
                {
                    m_Url = m_Url.Insert(0, "http://");
                }
                SetValue(ChartStyleInfoStore.UrlProperty, m_Url);
            }

        }
        /// <summary>
        /// Resets Url information.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetUrl()
        {
            ResetValue(ChartStyleInfoStore.UrlProperty);
        }
        /// <summary>
        /// Should the serialize Url.
        /// </summary>
        /// <returns>True if the element should serialize otherwise false.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeUrl()
        {
            return HasValue(ChartStyleInfoStore.UrlProperty);
        }

        /// <summary>
        /// Gets a value indicating whether Url information has been initialized for the current object.
        /// </summary>
        /// <value><c>true</c> if this instance has Url; otherwise, <c>false</c>.</value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasUrl
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartStyleInfoStore.UrlProperty);
            }
        }
        
        #endregion

       
    }
}
