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
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;

using Syncfusion.Documentation;
using Syncfusion.Styles;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    ///    For certain chart types such as Gantt charts, it is required to have relationships between points.
    ///    These are called 'Related Points'. This class represents symbol information that links such related points.
    /// </summary>
    [ImmutableObject(true)]
    public class ChartRelatedPointSymbolInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartRelatedPointSymbolInfo"/> class.
        /// </summary>
        /// <param name="shape">The shape.</param>
        /// <param name="imageIndex">Index of the image.</param>
        /// <param name="color">The color.</param>
        /// <param name="size">The size.</param>
        public ChartRelatedPointSymbolInfo(ChartSymbolShape shape, int imageIndex, Color color, Size size)
        {
            this.shape = shape;
            this.imageIndex = imageIndex;
            this.color = color;
            this.size = size;
        }

        private ChartSymbolShape shape;
        private int imageIndex;
        private Color color;
        private Size size;

        /// <summary>
        /// Gets the shape of the symbol.
        /// </summary>
        public ChartSymbolShape Shape
        {
            get
            {
                return this.shape;
            }
        }

        /// <summary>
        /// Gets the index value of the image that is to be used by the symbol.
        /// <seealso cref="ChartStyleInfo.Images"/>
        /// </summary>
        public int ImageIndex
        {
            get
            {
                return this.imageIndex;
            }
        }

        /// <summary>
        /// Gets the color of this symbol.
        /// </summary>
        public Color Color
        {
            get
            {
                return this.color;
            }
        }

        /// <summary>
        /// Gets the size of this symbol.
        /// </summary>
        public Size Size
        {
            get
            {
                return this.size;
            }
        }
    }

    /// <summary>
    /// Implements the data store for the <see cref="ChartRelatedPointInfo"/> object.
    /// </summary>
    /// <seealso cref="StyleInfoStore"/>
    /// <internalonly/>
    [Serializable,
    StaticDataField("sd"),
    DocumentationExclude()]
    public class ChartRelatedPointInfoStore : StyleInfoStore
    {
        private static StaticData sd = new StaticData(typeof(ChartRelatedPointInfoStore), typeof(ChartRelatedPointInfo), true);
        
        /// <summary>
        /// The Points Property.
        /// </summary>
        /// <internalonly/>
        public static readonly StyleInfoProperty PointsProperty = sd.CreateStyleInfoProperty(typeof(int[]), "Points");

        /// <summary>
        /// The Color Property.
        /// </summary>
        /// <internalonly/>
        public static readonly StyleInfoProperty ColorProperty = sd.CreateStyleInfoProperty(typeof(Color), "Color");

        /// <summary>
        /// The Width Property.
        /// </summary>
        /// <internalonly/>
        public static readonly StyleInfoProperty WidthProperty = sd.CreateStyleInfoProperty(typeof(float), "Width");

        /// <summary>
        /// The Alignment Property.
        /// </summary>
        /// <internalonly/>
        public static readonly StyleInfoProperty AlignmentProperty = sd.CreateStyleInfoProperty(typeof(PenAlignment), "Alignment");

        /// <summary>
        /// The DashStyle Property.
        /// </summary>
        /// <internalonly/>
        public static readonly StyleInfoProperty DashStyleProperty = sd.CreateStyleInfoProperty(typeof(DashStyle), "DashStyle");

        /// <summary>
        /// The DashPattern Property.
        /// </summary>
        /// <internalonly/>
        public static readonly StyleInfoProperty DashPatternProperty = sd.CreateStyleInfoProperty(typeof(float[]), "DashPattern");

        /// <summary>
        /// The StartSymbol Property.
        /// </summary>
        /// <internalonly/>
        public static readonly StyleInfoProperty StartSymbolProperty = sd.CreateStyleInfoProperty(typeof(ChartRelatedPointSymbolInfo), "StartSymbol");

        /// <summary>
        /// The EndSymbol Property.
        /// </summary>
        /// <internalonly/>
        public static readonly StyleInfoProperty EndSymbolProperty = sd.CreateStyleInfoProperty(typeof(ChartRelatedPointSymbolInfo), "EndSymbol");

        /// <summary>
        /// The Border Property.
        /// </summary>
        /// <internalonly/>
        public static readonly StyleInfoProperty BorderProperty = sd.CreateStyleInfoProperty(typeof(ChartRelatedPointLineInfo), "Border");
        
        /// <summary>
        /// Static data must be declared static in derived classes (this avoids collisions
        /// when StyleInfoStore is used in the same project for different types of style
        /// classes).
        /// </summary>
        /// <value>The Static Data.</value>
        /// <override/>
        protected override StaticData StaticDataStore
        {
            get
            {
                return sd;
            }
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartRelatedPointInfoStore"/> class.
        /// </summary>
        /// <internalonly/>
        public ChartRelatedPointInfoStore()
        {
        }

        /// <summary>
        /// Initializes a new <see cref="ChartFontInfoStore"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        private ChartRelatedPointInfoStore(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            //TraceUtil.TraceCurrentMethodInfoIf(Switches.Serialization.TraceVerbose, info.FullTypeName, info.MemberCount);
        }

        // Base class implementation of this method calls Activator.CreateInstance to achieve the same result.
        // I assume calling new directly is more efficient. Otherwise this override is obsolete.

        /// <summary>
        /// Creates an exact copy of the current object.
        /// </summary>
        /// <returns>
        /// A <see cref="T:Syncfusion.Styles.StyleInfoStore"/> with same data as the current object.
        /// </returns>
        /// <override/>
        public override object Clone()
        {
            StyleInfoStore target = new ChartRelatedPointInfoStore();
            CopyTo(target);
            return target;
        }

        /// <summary>
        /// Initializes the <see cref="ChartRelatedPointInfoStore"/> class.
        /// </summary>
        static ChartRelatedPointInfoStore()
        {
            //SizeProperty.Format += new StyleInfoPropertyConvertEventHandler(SizeToString);
            //SizeProperty.Parse += new StyleInfoPropertyConvertEventHandler(SizeStringToInt);
            //SizeProperty.WriteXml += new StyleInfoPropertyWriteXmlEventHandler(SizeProperty_WriteXml);
            //SizeProperty.ReadXml += new StyleInfoPropertyReadXmlEventHandler(SizeProperty_ReadXml);
        }

        /*

            private static void SizeToString(object sender, StyleInfoPropertyConvertEventArgs cevent)
            {

                if (cevent.Handled)
                    return;

                // We can only convert to string type. Test this using the DesiredType.
                if(cevent.DesiredType != typeof(string)) return;

                // TODO: CultureInfo en-US so that it gets written with a .
                cevent.Value = (((short) cevent.Value)/4.0f).ToString();
                cevent.Handled = true;
            }

            private static void SizeStringToInt(object sender, StyleInfoPropertyConvertEventArgs cevent)
            {

                if (cevent.Handled)
                    return;

                // Convert the string back to decimal using the static Parse method.
                cevent.Value = (short) (float.Parse(cevent.Value.ToString())*4);
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
            }*/
    }

    /// <summary>
    ///    For certain chart types such as Gantt charts, it is required to have relationships between points.
    ///    These are called 'Related Points'. This class represents such related points.
    /// </summary>
    public class ChartRelatedPointInfo : ChartSubStyleInfoBase
    {
        // Static Fields
        private static ChartRelatedPointInfo defaultPoint;

        internal static object CreateObject(StyleInfoSubObjectIdentity identity, object store)
        {
            if (store != null)
            {
                return new ChartRelatedPointInfo(identity, store as ChartRelatedPointInfoStore);
            }
            return new ChartRelatedPointInfo(identity);
        }

        /// <summary>
        /// Clears all the resources used by the component.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartRelatedPointInfo"/> class.
        /// </summary>  
        [DebuggerStepThrough()]
        public ChartRelatedPointInfo()
            : base(new ChartRelatedPointInfoStore())
        {
        }

        /// <summary>
        /// Initializes a new <see cref="ChartRelatedPointInfo"/> object and associates it with an existing <see cref="StyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="StyleInfoSubObjectIdentity"/> that holds the identity for this <see cref="ChartRelatedPointInfo"/>.
        /// </param>
        [DebuggerStepThrough()]
        public ChartRelatedPointInfo(StyleInfoSubObjectIdentity identity)
            : base(identity, new ChartRelatedPointInfoStore())
        {
        }

        /// <summary>
        /// Initializes a new <see cref="ChartRelatedPointInfo"/> object and associates it with an existing <see cref="StyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="StyleInfoSubObjectIdentity"/> that holds the identity for this <see cref="ChartRelatedPointInfo"/>.
        /// <param name="store">A <see cref="ChartRelatedPointInfoStore"/> that holds data for this <see cref="ChartRelatedPointInfo"/>.
        /// All changes made in this style object will be saved in the <see cref="ChartRelatedPointInfo"/> object.</param>
        /// </param>
        [DebuggerStepThrough()]
        public ChartRelatedPointInfo(StyleInfoSubObjectIdentity identity, ChartRelatedPointInfoStore store)
            : base(identity, store)
        {
        }

        // Base class implementation of this method calls Activator.CreateInstance to achieve the same result.
        // I assume calling new directly is more efficient. Otherwise this override is obsolete.

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
            return new ChartRelatedPointInfo(newOwner.CreateSubObjectIdentity(sip), (ChartRelatedPointInfoStore)Store.Clone());
        }

        /// <summary>
        /// Returns a default <see cref="ChartRelatedPointInfo"/> that is to be used with a default style.
        /// </summary>
        /// <remarks>
        /// The <see cref="ChartStyleInfo.Default"/> of the <see cref="ChartStyleInfo"/> class
        /// will return the default border info that this method generates through its
        /// overridden version of <see cref="GetDefaultStyle"/>.
        ///  </remarks>
        public static ChartRelatedPointInfo Default
        {
            [DebuggerStepThrough()]
            get
            {
                if (defaultPoint == null)
                {
                    defaultPoint = new ChartRelatedPointInfo();
                    defaultPoint.Color = SystemColors.ControlText;
                    defaultPoint.Width = 5f;
                    defaultPoint.Alignment = PenAlignment.Center;
                    defaultPoint.DashStyle = DashStyle.Solid;
                    defaultPoint.DashPattern = null;
                    defaultPoint.StartSymbol = new ChartRelatedPointSymbolInfo(ChartSymbolShape.None, -1, Color.White, Size.Empty);
                    defaultPoint.Border = new ChartRelatedPointLineInfo();
                }

                return defaultPoint;
            }
        }

        /// <summary>
        /// Gets the GDI+ pen.
        /// </summary>
        /// <value>The GDI+ pen.</value>
        /// <internalonly/>
        [Browsable(false),
       DocumentationExclude()]
        public Pen GdipPen
        {
            get
            {
                return GetGdipPen();
            }
        }

        /// <summary>
        /// Resets the gdip font.
        /// </summary>
        internal void ResetGdipFont()
        {
            //_pen = null;
        }

        /// <summary>
        /// Gets the gdip pen.
        /// </summary>
        /// <returns>Returns GdipPen object.</returns>
        private Pen GetGdipPen()
        {
            using (Pen pen = new Pen(this.Color, this.Width))
            {
                pen.DashStyle = this.DashStyle;
                pen.Alignment = this.Alignment;

                if (this.DashStyle == DashStyle.Custom && this.DashPattern != null)
                {
                    pen.DashPattern = this.DashPattern;
                }

                return pen;
            }
        }
      
        /// <param name="sip"></param>
        /// <override/>
        protected override void OnStyleChanged(StyleInfoProperty sip)
        {
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

        #region Points

        /// <summary>
        /// Gets the number of points.
        /// </summary>
        /// <value></value>
        public int Count
        {
            get
            {
                int[] points = this.Points;

                if (points == null)
                {
                    return 0;
                }

                else
                {
                    return points.GetLength(0);
                }
            }
        }

        /// <summary>
        /// Gets or sets an array of indices of related points.
        /// </summary>
        [
        Description("Indices of related points."),
        Category("Data")
        ]

        public int[] Points
        {
            [DebuggerStepThrough()]
            get
            {
                return (int[])GetValue(ChartRelatedPointInfoStore.PointsProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(ChartRelatedPointInfoStore.PointsProperty, value);
            }

        }

        /// <summary>
        /// Resets the <see cref="Points"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetPoints()
        {
            ResetValue(ChartRelatedPointInfoStore.PointsProperty);
        }

        /// <summary>
        /// Should the serialize points.
        /// </summary>
        /// <returns>True if the instance should serialize otherwise False.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializePoints()
        {
            return HasValue(ChartRelatedPointInfoStore.PointsProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Points"/> property has been initialized.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has points; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasPoints
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartRelatedPointInfoStore.PointsProperty);
            }

        }

        #endregion

        #region Color

        /// <summary>
        /// Gets or sets the color that is to be used for any visual representation.
        /// </summary>
        /// <value>The color.</value>
        [
        Description("Color to be used for any visual representation."),
        Category("Appearance")
        ]

        public Color Color
        {
            [DebuggerStepThrough()]
            get
            {
                return (Color)GetValue(ChartRelatedPointInfoStore.ColorProperty);
            }
            [DebuggerStepThrough()]
            set
            {
                SetValue(ChartRelatedPointInfoStore.ColorProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="Color"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetColor()
        {
            ResetValue(ChartRelatedPointInfoStore.ColorProperty);
        }

        /// <summary>
        /// Should the color of the serialize.
        /// </summary>
        /// <returns>True if the instance should serialize otherwise False.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeColor()
        {
            return HasValue(ChartRelatedPointInfoStore.ColorProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Color"/> property has been initialized.
        /// </summary>
        /// <value><c>true</c> if this instance has color; otherwise, <c>false</c>.</value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasColor
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartRelatedPointInfoStore.ColorProperty);
            }
        }

        #endregion

        #region Width

        /// <summary>
        /// Gets or sets the width that is to be used for any visual representation.
        /// </summary>
        /// <value>The width.</value>
        [
        Description("Width to be used for any visual representation."),
        Category("Appearance")
        ]
        public float Width
        {
            [DebuggerStepThrough()]
            get
            {
                return (float)GetValue(ChartRelatedPointInfoStore.WidthProperty);
            }
            [DebuggerStepThrough()]
            set
            {
                SetValue(ChartRelatedPointInfoStore.WidthProperty, value);
            }

        }

        /// <summary>
        /// Resets the <see cref="Width"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetWidth()
        {
            ResetValue(ChartRelatedPointInfoStore.WidthProperty);
        }

        /// <summary>
        /// Should the width of the serialize.
        /// </summary>
        /// <returns>True if the instance should serialize otherwise False.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeWidth()
        {
            return HasValue(ChartRelatedPointInfoStore.WidthProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Width"/> property has been initialized.
        /// </summary>
        /// <value><c>true</c> if this instance has width; otherwise, <c>false</c>.</value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasWidth
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartRelatedPointInfoStore.WidthProperty);
            }
        }

        #endregion

        #region Alignment

        /// <summary>
        /// Gets or sets the pen alignment that is to be used for any visual representation.
        /// </summary>
        /// <value>The alignment.</value>
        [
        Description("Pen alignment to be used for any visual representation."),
        Category("Appearance")
        ]
        public PenAlignment Alignment
        {
            get
            {
                return (PenAlignment)GetValue(ChartRelatedPointInfoStore.AlignmentProperty);
            }

            set
            {
                SetValue(ChartRelatedPointInfoStore.AlignmentProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="Alignment"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetAlignment()
        {
            ResetValue(ChartRelatedPointInfoStore.AlignmentProperty);
        }

        /// <summary>
        /// Should the serialize alignment.
        /// </summary>
        /// <returns>True if the instance should serialize otherwise False.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeAlignment()
        {
            return HasValue(ChartRelatedPointInfoStore.AlignmentProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Alignment"/> property has been initialized.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has alignment; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasAlignment
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartRelatedPointInfoStore.AlignmentProperty);
            }
        }

        #endregion

        #region DashStyle

        /// <summary>
        /// Gets or sets the dash style that is to be used for any visual representation.
        /// </summary>
        /// <value>The dash style.</value>
        [
        Description("Dash style to be used for any visual representation."),
        Category("Appearance")
        ]
        public DashStyle DashStyle
        {
            [DebuggerStepThrough()]
            get
            {
                return (DashStyle)GetValue(ChartRelatedPointInfoStore.DashStyleProperty);
            }
            [DebuggerStepThrough()]
            set
            {
                SetValue(ChartRelatedPointInfoStore.DashStyleProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="DashStyle"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetDashStyle()
        {
            ResetValue(ChartRelatedPointInfoStore.DashStyleProperty);
        }

        /// <summary>
        /// Should the serialize dash style.
        /// </summary>
        /// <returns>True if the instance should serialize otherwise False.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeDashStyle()
        {
            return HasValue(ChartRelatedPointInfoStore.DashStyleProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="DashStyle"/> property has been initialized.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has dash style; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasDashStyle
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartRelatedPointInfoStore.DashStyleProperty);
            }
        }

        #endregion

        #region DashPattern

        /// <summary>
        /// Gets or sets the dash pattern that is to be used for any visual representation.
        /// </summary>
        /// <value>The dash pattern.</value>
        [
        Description("Dash pattern to be used for any visual representation."),
        Category("Appearance")
        ]
        public float[] DashPattern
        {
            [DebuggerStepThrough()]
            get
            {
                return (float[])GetValue(ChartRelatedPointInfoStore.DashPatternProperty);
            }
            [DebuggerStepThrough()]
            set
            {
                SetValue(ChartRelatedPointInfoStore.DashPatternProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="DashPattern"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetDashPattern()
        {
            ResetValue(ChartRelatedPointInfoStore.DashPatternProperty);
        }

        /// <summary>
        /// Should the serialize dash pattern.
        /// </summary>
        /// <returns>True if the instance should serialize otherwise False.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeDashPattern()
        {
            return HasValue(ChartRelatedPointInfoStore.DashPatternProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="DashPattern"/> property has been initialized.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has dash pattern; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasDashPattern
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartRelatedPointInfoStore.DashPatternProperty);
            }
        }

        #endregion

        #region StartSymbol

        /// <summary>
        /// Gets or sets the start symbol that is to be used for any visual representation linking this related point with others.
        /// </summary>
        /// <value>The start symbol.</value>
        [
        Description("Start symbol to be used for any visual representation linking this related point with others."),
        Category("Appearance")
        ]
        public ChartRelatedPointSymbolInfo StartSymbol
        {
            [DebuggerStepThrough()]
            get
            {
                return (ChartRelatedPointSymbolInfo)GetValue(ChartRelatedPointInfoStore.StartSymbolProperty);
            }
            [DebuggerStepThrough()]
            set
            {
                SetValue(ChartRelatedPointInfoStore.StartSymbolProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="StartSymbol"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetStartSymbol()
        {
            ResetValue(ChartRelatedPointInfoStore.StartSymbolProperty);
        }

        /// <summary>
        /// Should the serialize start symbol.
        /// </summary>
        /// <returns>True if the instance should serialize otherwise False.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeStartSymbol()
        {
            return HasValue(ChartRelatedPointInfoStore.StartSymbolProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="StartSymbol"/> property has been initialized.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has start symbol; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasStartSymbol
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartRelatedPointInfoStore.StartSymbolProperty);
            }
        }

        #endregion

        #region EndSymbol

        /// <summary>
        /// Gets or sets the end symbol that is to be used for any visual representation linking this related point with others.
        /// </summary>
        /// <value>The end symbol.</value>
        [
        Description("End symbol to be used for any visual representation linking this related point with others."),
        Category("Appearance")
        ]
        public ChartRelatedPointSymbolInfo EndSymbol
        {
            [DebuggerStepThrough()]
            get
            {
                return (ChartRelatedPointSymbolInfo)GetValue(ChartRelatedPointInfoStore.EndSymbolProperty);
            }
            [DebuggerStepThrough()]
            set
            {
                SetValue(ChartRelatedPointInfoStore.EndSymbolProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="EndSymbol"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetEndSymbol()
        {
            ResetValue(ChartRelatedPointInfoStore.EndSymbolProperty);
        }

        /// <summary>
        /// Should the serialize end symbol.
        /// </summary>
        /// <returns>True if the instance should serialize otherwise False.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeEndSymbol()
        {
            return HasValue(ChartRelatedPointInfoStore.EndSymbolProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="EndSymbol"/> property has been initialized.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has end symbol; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasEndSymbol
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartRelatedPointInfoStore.EndSymbolProperty);
            }
        }

        #endregion

        #region Border

        /// <summary>
        /// Gets or sets the border that is to be used for any visual representation linking this related point with others.
        /// </summary>
        /// <value>The border.</value>
        [
        Description("Border to be used for any visual representation linking this related point with others."),
        Category("Appearance")
        ]
        public ChartRelatedPointLineInfo Border
        {
            [DebuggerStepThrough()]
            get
            {
                return (ChartRelatedPointLineInfo)GetValue(ChartRelatedPointInfoStore.BorderProperty);
            }
            [DebuggerStepThrough()]
            set
            {
                SetValue(ChartRelatedPointInfoStore.BorderProperty, value);                    
            }
        }

        /// <summary>
        /// Resets the <see cref="Border"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetBorder()
        {
            ResetValue(ChartRelatedPointInfoStore.BorderProperty);
        }

        /// <summary>
        /// Should the serialize border.
        /// </summary>
        /// <returns>True if the instance should serialize otherwise False.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeBorder()
        {
            return HasValue(ChartRelatedPointInfoStore.BorderProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Border"/> property has been initialized.
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
                return HasValue(ChartRelatedPointInfoStore.BorderProperty);
            }
        }

        #endregion

    }

    /// <summary>
    /// This class contains border information that is to be used for any visual representation linking a related point with others.
    /// </summary>
    [ImmutableObject(true)]
    public class ChartRelatedPointLineInfo
    {

        private Color color;
        private float width;
        private PenAlignment alignment;
        private DashStyle dashStyle;
        private float[] dashPattern;

        /// <summary>
        /// Overloaded constructor.
        /// </summary>
        /// <param name="color" type="System.Drawing.Color">
        ///     <para>
        ///     Color of the border line.
        ///     </para>
        /// </param>
        /// <param name="width" type="float">
        ///     <para>
        ///     Width of the line.
        ///     </para>
        /// </param>
        /// <param name="alignment" type="System.Drawing.Drawing2D.PenAlignment">
        ///     <para>
        ///     Pen alignment to be used to render the line.
        ///     </para>
        /// </param>
        /// <param name="dashStyle" type="System.Drawing.Drawing2D.DashStyle">
        ///     <para>
        ///     Dash style of the line.
        ///     </para>
        /// </param>
        /// <param name="dashPattern" type="float[]">
        ///     <para>
        ///     Dash pattern of the line.
        ///     </para>
        /// </param>
        public ChartRelatedPointLineInfo(Color color, float width, PenAlignment alignment, DashStyle dashStyle, float[] dashPattern)
        {
            this.color = color;
            this.width = width;
            this.alignment = alignment;
            this.dashStyle = dashStyle;
            this.dashPattern = dashPattern;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartRelatedPointLineInfo"/> class.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="width">The width.</param>
        public ChartRelatedPointLineInfo(Color color, float width)
            : this(color, width, PenAlignment.Center, DashStyle.Solid, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartRelatedPointLineInfo"/> class.
        /// </summary>
        /// <param name="color">The color.</param>
        public ChartRelatedPointLineInfo(Color color)
            : this(color, 5f, PenAlignment.Center, DashStyle.Solid, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartRelatedPointLineInfo"/> class.
        /// </summary>
        public ChartRelatedPointLineInfo()
            : this(SystemColors.ControlText, 5f, PenAlignment.Center, DashStyle.Solid, null)
        {
        }

        /// <summary>
        /// Gets the GDI+ pen.
        /// </summary>
        /// <value>The GDI+ pen.</value>
        /// <internalonly/>
        [Browsable(false),
     DocumentationExclude()]

        public Pen GdipPen
        {
            get
            {
                return GetGdipPen();
            }
        }

        /// <summary>
        /// Gets the GDI+ pen.
        /// </summary>
        /// <returns>Returns the GdipPen.</returns>
        private Pen GetGdipPen()
        {
            using (Pen pen = new Pen(this.Color, this.Width))
            {
                pen.DashStyle = this.DashStyle;
                pen.Alignment = this.Alignment;

                if (this.DashStyle == DashStyle.Custom && this.DashPattern != null)
                {
                    pen.DashPattern = this.DashPattern;
                }


                return pen;
            }
        }

        // Properties

        #region Color

        /// <summary>
        /// Gets the color of the line.
        /// </summary>
        [
        Description("Color of the line."),
        Category("Appearance")
        ]

        public Color Color
        {
            [DebuggerStepThrough()]
            get
            {
                return this.color;
            }
        }

        #endregion

        #region Width

        /// <summary>
        /// Gets the width of the line.
        /// </summary>
        [
        Description("Width of the line."),
        Category("Appearance")
        ]
        public float Width
        {
            [DebuggerStepThrough()]
            get
            {
                return this.width;
            }
        }

        #endregion

        #region Alignment

        /// <summary>
        /// Gets the pen alignment of the line.
        /// </summary>
        [
        Description("Pen alignment of the line."),
        Category("Appearance")
        ]
        public PenAlignment Alignment
        {
            get
            {
                return this.alignment;
            }
        }

        #endregion

        #region DashStyle

        /// <summary>
        /// Gets the dash style of the line.
        /// </summary>
        [
        Description("Dash style of the line."),
        Category("Appearance")
        ]
        public DashStyle DashStyle
        {
            [DebuggerStepThrough()]
            get
            {
                return this.dashStyle;
            }
        }

        #endregion

        #region DashPattern

        /// <summary>
        /// Gets the dash pattern of the line.
        /// </summary>
        [
        Description("Dash pattern of the line."),
        Category("Appearance")
        ]
        public float[] DashPattern
        {
            [DebuggerStepThrough()]
            get
            {
                return this.dashPattern;
            }
        }

        #endregion

    }
}