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

using Syncfusion.Documentation;
using Syncfusion.Styles;
using System.Drawing.Design;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Specifies the various available symbols that may be displayed at a ChartPoint.
    /// </summary>
    [Editor(typeof(ChartSymbolShapeEditor), typeof(UITypeEditor))]
    public enum ChartSymbolShape
    {
        /// <summary>
        /// No symbol is displayed.
        /// </summary>
        None,

        /// <summary>
        /// Arrow is displayed.
        /// </summary>
        Arrow,

        /// <summary>
        /// Inverted Arrow is displayed.
        /// </summary>
        InvertedArrow,

        /// <summary>
        /// Circle is displayed.
        /// </summary>
        Circle,

        /// <summary>
        /// Cross is displayed.
        /// </summary>
        Cross,

        /// <summary>
        /// Horizontal Line is displayed.
        /// </summary>
        HorizLine,

        /// <summary>
        /// Vertical Line is displayed.
        /// </summary>
        VertLine,

        /// <summary>
        /// Diamond is displayed.
        /// </summary>
        Diamond,

        /// <summary>
        /// Square is displayed.
        /// </summary>
        Square,

        /// <summary>
        /// Triangle is displayed.
        /// </summary>
        Triangle,

        /// <summary>
        /// Inverted triangle is displayed.
        /// </summary>
        InvertedTriangle,

        /// <summary>
        /// Hexagon is displayed.
        /// </summary>
        Hexagon,

        /// <summary>
        /// Pentagon is displayed.
        /// </summary>
        Pentagon,

        /// <summary>
        /// Star is displayed.
        /// </summary>
        Star,
		
        /// <summary>
        /// Image specified in ImageIndex is displayed.
        /// </summary>
        Image
    }

    public enum ChartCustomShape
    {
        /// <summary>
        /// Circle is displayed.
        /// </summary>
        Circle,

        /// <summary>
        /// Square is displayed.
        /// </summary>
        Square,

        /// <summary>
        /// Hexagon is displayed.
        /// </summary>
        Hexagon,

        /// <summary>
        /// Pentagon is displayed.
        /// </summary>
        Pentagon
    }

    /// <summary>
    /// This class implements the data store for the <see cref="ChartSymbolInfo"/> object.
    /// </summary>
    /// <seealso cref="StyleInfoStore"/>
    /// <internalonly/>
    [Serializable
 , StaticDataField("sd")
 , DocumentationExclude()]
    public class ChartSymbolInfoStore : StyleInfoStore
    {
        #region Constants
        private static StaticData sd = new StaticData(typeof(ChartSymbolInfoStore), typeof(ChartSymbolInfo), true);
        /// <internalonly/>
        public static readonly StyleInfoProperty ShapeProperty = sd.CreateStyleInfoProperty(typeof(ChartSymbolShape), "Shape");

        /// <internalonly/>
        public static readonly StyleInfoProperty ImageIndexProperty = sd.CreateStyleInfoProperty(typeof(int), "ImageIndex");

        /// <internalonly/>
        public static readonly StyleInfoProperty ColorProperty = sd.CreateStyleInfoProperty(typeof(Color), "Color");

        /// <internalonly/>
        public static readonly StyleInfoProperty HighlightColorProperty = sd.CreateStyleInfoProperty(typeof(Color), "HighlightColor");

        /// <internalonly/>
        public static readonly StyleInfoProperty DimmedColorProperty = sd.CreateStyleInfoProperty(typeof(Color), "DimmedColor");

        /// <internalonly/>
        public static readonly StyleInfoProperty SizeProperty = sd.CreateStyleInfoProperty(typeof(Size), "Size");

        /// <internalonly/>
        public static readonly StyleInfoProperty OffsetProperty = sd.CreateStyleInfoProperty(typeof(Size), "Offset");

        /// <internalonly/>
        public static readonly StyleInfoProperty MarkerProperty = sd.CreateStyleInfoProperty(typeof(ChartMarker), "Marker");

        /// <internalonly/>
        public static readonly StyleInfoProperty BorderProperty = sd.CreateStyleInfoProperty(typeof(ChartLineInfo), "Border");
        #endregion

        #region Properties
        /// <summary>
        /// Static data must be declared static in derived classes (this avoids collisions
        /// when StyleInfoStore is used in the same project for different types of style
        /// classes).
        /// </summary>
        /// <value></value>
        /// <override/>
        protected override StaticData StaticDataStore
        {
            get
            {
                return sd;
            }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartSymbolInfoStore"/> class.
        /// </summary>
        public ChartSymbolInfoStore()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartSymbolInfoStore"/> class from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info.</param>
        private ChartSymbolInfoStore(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.Serialization.TraceVerbose, info.FullTypeName, info.MemberCount);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Creates an exact copy of the current object.
        /// Base class implementation of this method calls Activator.CreateInstance to achieve the same result.
        /// I assume calling new directly is more efficient. Otherwise this override is obsolete.
        /// </summary>
        /// <returns>
        /// A <see cref="T:Syncfusion.Styles.StyleInfoStore"/> with same data as the current object.
        /// </returns>
        /// <override/>
        public override object Clone()
        {
            StyleInfoStore target = new ChartSymbolInfoStore();
            this.CopyTo(target);
            return target;
        }
        #endregion
    }
    public class ChartCustomShapeInfoStore : StyleInfoStore
    {
        #region Constants
        private static StaticData sd = new StaticData(typeof(ChartCustomShapeInfoStore), typeof(ChartCustomShapeInfo), true);
        
        /// <internalonly/>
        public static readonly StyleInfoProperty ColorProperty = sd.CreateStyleInfoProperty(typeof(Color), "Color");

        /// <internalonly/>
        public static readonly StyleInfoProperty ShapeTypeProperty = sd.CreateStyleInfoProperty(typeof(ChartCustomShape), "ShapeType");

        /// <internalonly/>
        public static readonly StyleInfoProperty BorderProperty = sd.CreateStyleInfoProperty(typeof(ChartCustomShape), "Border");

       
        #endregion

        #region Properties
        /// <summary>
        /// Static data must be declared static in derived classes (this avoids collisions
        /// when StyleInfoStore is used in the same project for different types of style
        /// classes).
        /// </summary>
        /// <value></value>
        /// <override/>
        protected override StaticData StaticDataStore
        {
            get
            {
                return sd;
            }
        }
        #endregion

       

    }

    public class ChartCustomShapeInfo : ChartSubStyleInfoBase
    {
        #region Members
        // Static Fields.
        private static readonly ChartCustomShapeInfo c_defaultShapeInfo;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes the new instance of the <see cref="ChartCustomShapeInfo"/> class.
        /// </summary>
        static ChartCustomShapeInfo()
        {
            ChartCustomShapeInfo defaultShapeInfo = new ChartCustomShapeInfo();

            defaultShapeInfo = new ChartCustomShapeInfo();
            defaultShapeInfo.Color = SystemColors.HighlightText;
            defaultShapeInfo.Type = ChartCustomShape.Square;
            defaultShapeInfo.Border = ChartLineInfo.CreateDefault();
            c_defaultShapeInfo = defaultShapeInfo;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartSymbolInfo"/> class.
        /// </summary>
        [DebuggerStepThrough()]
        public ChartCustomShapeInfo()
            : base(new ChartCustomShapeInfoStore())
        {
        }

         /// <summary>
        /// Initializes a new <see cref="ChartSymbolInfo"/> object and associates it with an existing <see cref="StyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="StyleInfoSubObjectIdentity"/> that holds the identity for this <see cref="ChartSymbolInfo"/>.
        /// </param>
        [DebuggerStepThrough()]
        public ChartCustomShapeInfo(StyleInfoSubObjectIdentity identity)
            : base(identity, new ChartCustomShapeInfoStore())
        {
        }

        /// <summary>
        /// Initializes a new  instance of <see cref="ChartSymbolInfo"/> object and associates it with an existing <see cref="StyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="StyleInfoSubObjectIdentity"/> that holds the identity for this <see cref="ChartSymbolInfo"/>.
        /// <param name="store">A <see cref="ChartSymbolInfoStore"/> that holds data for this <see cref="ChartSymbolInfo"/>.
        /// All changes made in this style object will be saved in the <see cref="ChartSymbolInfoStore"/> object.</param>
        /// </param>
        [DebuggerStepThrough()]
        public ChartCustomShapeInfo(StyleInfoSubObjectIdentity identity, ChartCustomShapeInfoStore store)
            : base(identity, store)
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Returns <see cref="ChartCustomShapeInfo.Default"/>.
        /// </summary>
        /// <returns>A <see cref="ChartCustomShapeInfo"/> object with default values.</returns>
        protected override StyleInfoBase GetDefaultStyle()
        {
            return c_defaultShapeInfo;
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets a default <see cref="ChartSymbolInfo"/> that is to be used with a default custom shape.
        /// </summary>
        public static ChartCustomShapeInfo Default
        {
            get
            {
                return c_defaultShapeInfo;
            }
        }

        #region Color
        /// <summary>
        /// Gets or sets the color that is to be used with the symbol.
        /// </summary>
        /// <value>The color.</value>
        [Browsable(true),
       Category("Appearance"),
       Description("Color to be used with the symbol.")]
        public Color Color
        {
            [DebuggerStepThrough()]
            get
            {
                return (Color)GetValue(ChartCustomShapeInfoStore.ColorProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(ChartCustomShapeInfoStore.ColorProperty, value);
            }
        }
        #endregion

        #region ShapeType
        /// <summary>
        /// Gets or sets the style of the shape that is to be displayed.
        /// Default shape is square. 
        /// It will support the limitted shape(Square, Circle, Hexagon, Pentagon) draw around the custom point
        /// </summary>
        [
        Browsable(true),
        Category("Appearance"),
        DefaultValue("Square"), Description("The style of the shape to be displayed. It will support the limitted shape(Square, Circle, Hexagon, Pentagon) draw around the custom point")
        ]
        public ChartCustomShape Type
        {
            [DebuggerStepThrough()]
            get
            {
                return (ChartCustomShape)GetValue(ChartCustomShapeInfoStore.ShapeTypeProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(ChartCustomShapeInfoStore.ShapeTypeProperty, value);
            }
        }
        #endregion

        #region Border
        ///<summary>
        /// Gets or sets border to the custom shape.
        /// </summary>
        [DefaultValue(typeof(ChartLineInfo), "Draw border of shape"), Category("Appearance")]
        public ChartLineInfo Border
        {
            [DebuggerStepThrough()]
            get
            {
                return (ChartLineInfo)GetValue(ChartCustomShapeInfoStore.BorderProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(ChartCustomShapeInfoStore.BorderProperty, value);
            }
        }
        #endregion

        #endregion
    }
    /// <summary>
    /// This class provides a <see cref="StyleInfoSubObjectBase"/> object for symbols associated with a ChartPoint.
    /// </summary>
    public class ChartSymbolInfo : ChartSubStyleInfoBase
    {
        #region Members
        // Static Fields.
        private static readonly ChartSymbolInfo c_defaultSymbolInfo;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes the new instance of the <see cref="ChartSymbolInfo"/> class.
        /// </summary>
        static ChartSymbolInfo()
        {
            ChartSymbolInfo defaultSymbolInfo = new ChartSymbolInfo();

            defaultSymbolInfo = new ChartSymbolInfo();
            defaultSymbolInfo.Color = SystemColors.HighlightText;
            defaultSymbolInfo.HighlightColor = Color.Transparent;
            defaultSymbolInfo.DimmedColor = Color.Transparent;
            defaultSymbolInfo.ImageIndex = -1;
            defaultSymbolInfo.Size = new Size(10, 10);
            defaultSymbolInfo.Shape = ChartSymbolShape.None;
            defaultSymbolInfo.Offset = new Size(0, 0);
            defaultSymbolInfo.Marker = new ChartMarker();
            defaultSymbolInfo.Border = ChartLineInfo.CreateDefault();

            c_defaultSymbolInfo = defaultSymbolInfo;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartSymbolInfo"/> class.
        /// </summary>
        [DebuggerStepThrough()]
        public ChartSymbolInfo()
            : base(new ChartSymbolInfoStore())
        {
        }

        /// <summary>
        /// Initializes a new <see cref="ChartSymbolInfo"/> object and associates it with an existing <see cref="StyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="StyleInfoSubObjectIdentity"/> that holds the identity for this <see cref="ChartSymbolInfo"/>.
        /// </param>
        [DebuggerStepThrough()]
        public ChartSymbolInfo(StyleInfoSubObjectIdentity identity)
            : base(identity, new ChartSymbolInfoStore())
        {
        }

        /// <summary>
        /// Initializes a new  instance of <see cref="ChartSymbolInfo"/> object and associates it with an existing <see cref="StyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="StyleInfoSubObjectIdentity"/> that holds the identity for this <see cref="ChartSymbolInfo"/>.
        /// <param name="store">A <see cref="ChartSymbolInfoStore"/> that holds data for this <see cref="ChartSymbolInfo"/>.
        /// All changes made in this style object will be saved in the <see cref="ChartSymbolInfoStore"/> object.</param>
        /// </param>
        [DebuggerStepThrough()]
        public ChartSymbolInfo(StyleInfoSubObjectIdentity identity, ChartSymbolInfoStore store)
            : base(identity, store)
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Creates the new <see cref="ChartSymbolInfo"/> class.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <param name="store">The store.</param>
        /// <returns>Returns new ChartSymbolInfo instance. </returns>
        internal static object CreateObject(StyleInfoSubObjectIdentity identity, object store)
        {
            if (store != null)
            {
                return new ChartSymbolInfo(identity, store as ChartSymbolInfoStore);
            }

            return new ChartSymbolInfo(identity);
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
        /// Makes an exact copy of the current object.
        /// </summary>
        /// <param name="newOwner">The new owner style object for the copied object.</param>
        /// <param name="sip">The identifier for this object.</param>
        /// <returns>
        /// A copy of the current object registered with the new owner style object.
        /// </returns>
        /// <override/>
        [DebuggerStepThrough()]
        public override IStyleInfoSubObject MakeCopy(StyleInfoBase newOwner, StyleInfoProperty sip)
        {
            return new ChartSymbolInfo(newOwner.CreateSubObjectIdentity(sip), (ChartSymbolInfoStore)Store.Clone());
        }

        /// <summary>
        /// Returns <see cref="ChartSymbolInfo.Default"/>.
        /// </summary>
        /// <returns>A <see cref="ChartSymbolInfo"/> object with default values.</returns>
        protected override StyleInfoBase GetDefaultStyle()
        {
            return c_defaultSymbolInfo;
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets a default <see cref="ChartSymbolInfo"/> that is to be used with a default style.
        /// </summary>
        public static ChartSymbolInfo Default
        {
            get
            {
                return c_defaultSymbolInfo;
            }
        }

        #region Style

        /// <summary>
        /// Gets or sets the style of the symbol that is to be displayed.
        /// </summary>
        [
        Browsable(true),
        Category("Appearance"),
        Description("The style of the symbol to be displayed.")
        ]
        public ChartSymbolShape Shape
        {
            [DebuggerStepThrough()]
            get
            {
                return (ChartSymbolShape)GetValue(ChartSymbolInfoStore.ShapeProperty);
            }
            [DebuggerStepThrough()]
            set
            {
                SetValue(ChartSymbolInfoStore.ShapeProperty, value);
            }
        }

        /// <summary>
        /// Resets the symbol style.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetShape()
        {
            ResetValue(ChartSymbolInfoStore.ShapeProperty);
        }

        /// <summary>
        /// Should the serialize shape.
        /// </summary>
        /// <returns>True if the element should serialize otherwise False.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeShape()
        {
            return HasValue(ChartSymbolInfoStore.ShapeProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the style has been initialized.
        /// </summary>
        /// <value><c>true</c> if this instance has shape; otherwise, <c>false</c>.</value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasShape
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartSymbolInfoStore.ShapeProperty);
            }
        }

        #endregion

        #region ImageIndex

        /// <summary>
        /// Gets or sets the image index that is to be used to access the image from the associated <see cref="ChartStyleInfo"/> object's ImageList.
        /// </summary>
        /// <value>The index of the image.</value>
        [
        Browsable(true),
        Category("Appearance"),
        Description("Index of the image from the associated ChartStyleInfo's ImageList.")
        ]
        public int ImageIndex
        {
            [DebuggerStepThrough()]
            get
            {
                return (int)GetValue(ChartSymbolInfoStore.ImageIndexProperty);
            }
            [DebuggerStepThrough()]
            set
            {
                SetValue(ChartSymbolInfoStore.ImageIndexProperty, value);
            }
        }

        /// <summary>
        /// Resets the image index.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetImageIndex()
        {
            ResetValue(ChartSymbolInfoStore.ImageIndexProperty);
        }

        /// <summary>
        /// Should the index of the serialize image.
        /// </summary>
        /// <returns>True if the element should serialize otherwise False.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeImageIndex()
        {
            return HasValue(ChartSymbolInfoStore.ImageIndexProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the ImageIndex has been initialized.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has image index; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasImageIndex
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartSymbolInfoStore.ImageIndexProperty);
            }
        }

        #endregion

        #region Color
        /// <summary>
        /// Gets or sets the color that is to be used with the symbol.
        /// </summary>
        /// <value>The color.</value>
        [Browsable(true),
       Category("Appearance"),
       Description("Color to be used with the symbol.")]
        public Color Color
        {
            [DebuggerStepThrough()]
            get
            {
                return (Color)GetValue(ChartSymbolInfoStore.ColorProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(ChartSymbolInfoStore.ColorProperty, value);
            }
        }

        /// <summary>
        /// Resets the symbol's color.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetColor()
        {
            ResetValue(ChartSymbolInfoStore.ColorProperty);
        }

        /// <summary>
        /// Should the color of the serialize.
        /// </summary>
        /// <returns>True if the element should serialize otherwise False.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeColor()
        {
            return HasValue(ChartSymbolInfoStore.ColorProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the symbol's color has been initialized.
        /// </summary>
        /// <value><c>true</c> if this instance has color; otherwise, <c>false</c>.</value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasColor
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartSymbolInfoStore.ColorProperty);
            }
        }
        #endregion       

        #region HighlightColor

        /// <summary>
        /// Gets or sets the color of the highlighted symbol.
        /// </summary>
        /// <value>The color of the highlighted symbol.</value>
        [Browsable(true),
       Category("Appearance"),
       Description("Gets or sets the color of the highlighted symbol.")]
        public Color HighlightColor 
        {
            [DebuggerStepThrough()]
            get
            {
                return (Color)GetValue(ChartSymbolInfoStore.HighlightColorProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(ChartSymbolInfoStore.HighlightColorProperty, value);
            }
        }

        /// <summary>
        /// Resets the color of the highlighted symbol.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetHighlightColor()
        {
            ResetValue(ChartSymbolInfoStore.HighlightColorProperty);
        }

        /// <summary>
        /// Shoulds the color of the serialize highlighted symbol.
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeHighlightColor()
        {
            return HasValue(ChartSymbolInfoStore.HighlightColorProperty);
        }

        /// <summary>
        /// Gets a value indicating whether this instance has highlight color.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has highlight color; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasHighlightColor
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartSymbolInfoStore.HighlightColorProperty);
            }
        }
        #endregion

        #region DimmedColor

        /// <summary>
        /// Gets or sets the color of the dimmed symbol.
        /// </summary>
        /// <value>The color of the dimmed symbol.</value>
        [Browsable(true),
       Category("Appearance"),
       Description("Color to be used with the symbol.")]
        public Color DimmedColor
        {
            [DebuggerStepThrough()]
            get
            {
                return (Color)GetValue(ChartSymbolInfoStore.DimmedColorProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(ChartSymbolInfoStore.DimmedColorProperty, value);
            }
        }

        /// <summary>
        /// Resets the color of the dimmed.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetDimmedColor()
        {
            ResetValue(ChartSymbolInfoStore.DimmedColorProperty);
        }

        /// <summary>
        /// Shoulds the color of the serialize dimmed.
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeDimmedColor()
        {
            return HasValue(ChartSymbolInfoStore.DimmedColorProperty);
        }

        /// <summary>
        /// Gets a value indicating whether this instance has dimmed color.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has dimmed color; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasDimmedColor
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartSymbolInfoStore.DimmedColorProperty);
            }
        }
        #endregion

        #region Size
        /// <summary>
        /// Gets or sets the size of the symbol.
        /// </summary>
        /// <value>The size.</value>
        [Browsable(true),
       Category("Appearance"),
       Description("Specifies the size of the symbol.")]
        public Size Size
        {
            [DebuggerStepThrough()]
            get
            {
                return (Size)GetValue(ChartSymbolInfoStore.SizeProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(ChartSymbolInfoStore.SizeProperty, value);
            }
        }

        /// <summary>
        /// Resets the size.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetSize()
        {
            ResetValue(ChartSymbolInfoStore.SizeProperty);
        }

        /// <summary>
        /// Should the size of the serialize.
        /// </summary>
        /// <returns>True if the element should serialize otherwise False.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeSize()
        {
            return HasValue(ChartSymbolInfoStore.SizeProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the size of the symbol has been initialized.
        /// </summary>
        /// <value><c>true</c> if this instance has size; otherwise, <c>false</c>.</value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasSize
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartSymbolInfoStore.SizeProperty);
            }
        }
        #endregion

        #region Offset
        /// <summary>
        /// Gets or sets the offset of the symbol.
        /// </summary>
        /// <value>The offset.</value>
        [Browsable(true),
        Category("Appearance"),
        Description("Specifies the offset of the symbol.")]
        public Size Offset
        {
            [DebuggerStepThrough()]
            get
            {
                return (Size)GetValue(ChartSymbolInfoStore.OffsetProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(ChartSymbolInfoStore.OffsetProperty, value);
            }
        }

        /// <summary>
        /// Resets the offset.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetOffset()
        {
            ResetValue(ChartSymbolInfoStore.OffsetProperty);
        }

        /// <summary>
        /// Should the serialize offset.
        /// </summary>
        /// <returns>True if the element should serialize otherwise False.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeOffset()
        {
            return HasValue(ChartSymbolInfoStore.OffsetProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the offset of the symbol has been initialized.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has offset; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasOffset
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartSymbolInfoStore.OffsetProperty);
            }
        }
        #endregion

        #region Border
        /// <summary>
        /// Gets or sets the information that is to be used for drawing border.
        /// </summary>
        /// <value>The border.</value>
        [
        Description("Line information."),
        Browsable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Category("Appearance")
        ]
        public ChartLineInfo Border
        {
            [DebuggerStepThrough()]
            get
            {
                return (ChartLineInfo)GetValue(ChartSymbolInfoStore.BorderProperty);
            }

            set
            {
                SetValue(ChartSymbolInfoStore.BorderProperty, value);
            }
        }

        /// <summary>
        /// Resets line information.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetBorder()
        {
            ResetValue(ChartSymbolInfoStore.BorderProperty);
        }

        /// <summary>
        /// Should the serialize border.
        /// </summary>
        /// <returns>True if the element should serialize otherwise False.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeBorder()
        {
            return HasValue(ChartSymbolInfoStore.BorderProperty);
        }

        /// <summary>
        /// Gets a value indicating whether line information has been initialized for the current object.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasBorder
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartSymbolInfoStore.BorderProperty);
            }
        }
        #endregion

        #region Marker
        /// <summary>
        /// Gets or sets the marker of the symbol.
        /// </summary>
        [Browsable(false),
        Category("Appearance"),
        EditorBrowsable(EditorBrowsableState.Never),
        Description("Specifies the size of the symbol.")]
        public ChartMarker Marker
        {
            [DebuggerStepThrough()]
            get
            {
                return (ChartMarker)GetValue(ChartSymbolInfoStore.MarkerProperty);
            }
            [DebuggerStepThrough()]
            set
            {
                SetValue(ChartSymbolInfoStore.MarkerProperty, value);
            }
        }
        /// <summary>
        /// Resets the marker.
        /// </summary>
        [DebuggerStepThrough(),
        EditorBrowsable(EditorBrowsableState.Never)]
        public void ResetMarker()
        {
            ResetValue(ChartSymbolInfoStore.MarkerProperty);
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeMarker()
        {
            return HasValue(ChartSymbolInfoStore.MarkerProperty);
        }
        /// <summary>
        /// Indicates whether the marker of the symbol has been initialized.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasMarker
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartSymbolInfoStore.MarkerProperty);
            }
        }
        #endregion

        #endregion
    }
}