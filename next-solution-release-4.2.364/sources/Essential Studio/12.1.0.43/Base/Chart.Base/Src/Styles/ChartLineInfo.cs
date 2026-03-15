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
    /// Implements the data store for the <see cref="ChartLineInfo"/> object.
    /// </summary>
    /// <seealso cref="StyleInfoStore"/>
    /// <internalonly/>
    [Serializable,
   StaticDataField("sd"),
   DocumentationExclude()]
    public class ChartLineInfoStore : StyleInfoStore
    {
        #region Constants
        /// <summary>
        /// The Static Data class.
        /// </summary>
        private static StaticData sd = new StaticData(typeof(ChartLineInfoStore), typeof(ChartLineInfo), true);

        /// <summary>
        /// The Color Property.
        /// </summary>
        /// <internalonly/>
        public static readonly StyleInfoProperty ColorProperty = sd.CreateStyleInfoProperty(typeof(Color), "Color");

        /// <summary>
        /// The Width property.
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
        /// The DashPattern Property .
        /// </summary>
        /// <internalonly/>
        public static readonly StyleInfoProperty DashPatternProperty = sd.CreateStyleInfoProperty(typeof(float[]), "DashPattern");
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
        /// Initializes a new instance of the <see cref="ChartLineInfoStore"/> class.
        /// </summary>
        /// <internalonly/>
        public ChartLineInfoStore()
            : base()
        {
        } 

        /// <summary>
        /// Initializes a new <see cref="ChartFontInfoStore"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        private ChartLineInfoStore(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Creates an exact copy of the current object.
        /// </summary>
        /// <returns>
        /// A <see cref="T:Syncfusion.Styles.StyleInfoStore"/> with same data as the current object.
        /// </returns>
        /// <override/>
        public override object Clone()
        {
            StyleInfoStore target = new ChartLineInfoStore();
            this.CopyTo(target);
            return target;
        }
        #endregion
    }

    /// <summary>
    /// Provides a <see cref="StyleInfoSubObjectBase"/> object for border line settings associated with a point.
    /// Properties that have not been initialized will inherit default
    /// values from a base style.
    /// </summary>
    public class ChartLineInfo : ChartSubStyleInfoBase
    {
        #region Class static members
        /// <summary>
        /// Store default <see cref="ChartLineInfo"/>.
        /// </summary>
        private readonly static ChartLineInfo m_defaultLine;
        #endregion

        #region Class members
        /// <summary>
        /// Store pen to draw.
        /// </summary>
        private System.Drawing.Pen m_pen = null;

        /// <summary>
        /// Store value indicates that need recreate pen.
        /// </summary>
        private bool m_updatePen;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets a default <see cref="ChartLineInfo"/> to be used with a default style.
        /// </summary>
        /// <value>The default.</value>
        /// <remarks>
        /// The <see cref="ChartStyleInfo.Default"/> of the <see cref="ChartStyleInfo"/> class
        /// will return the default line info that this method generates through its
        /// overridden version of <see cref="GetDefaultStyle"/>.
        /// </remarks>
        public static ChartLineInfo Default
        {
            [DebuggerStepThrough()]
            get
            {
                return m_defaultLine;
            }
        }

        /// <summary>
        /// Gets pen associated with style.
        /// </summary>
        /// <value>The gdip pen.</value>
        [Browsable(false),
        DocumentationExclude()]
        public Pen GdipPen
        {
            get
            {
                return GetGdipPen();
            }
        }

        #region Color

        /// <summary>
        /// Gets or sets the color of the line. For line based charts it works only when 3D is enabled.
        /// </summary>
        /// <value>The color.</value>
        [ChartTemplate(ChartTemplateSet.Simple),Category("Appearance")]
        public Color Color
        {
            [DebuggerStepThrough()]
            get
            {
                return (Color)GetValue(ChartLineInfoStore.ColorProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(ChartLineInfoStore.ColorProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="Color"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetColor()
        {
            ResetValue(ChartLineInfoStore.ColorProperty);
        }

        /// <summary>
        /// Should the color of the serialize.
        /// </summary>
        /// <returns>Returns true whether it should serialize the element else false.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeColor()
        {
            return HasValue(ChartLineInfoStore.ColorProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Color"/> property has been initialized.
        /// </summary>
        /// <value><c>True</c> if this instance has color; otherwise, <c>false</c>.</value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasColor
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartLineInfoStore.ColorProperty);
            }
        }
        #endregion

        #region Width
        /// <summary>
        /// Gets or sets the width in pixels of the line represented by this object.
        /// </summary>
        /// <value>The width.</value>
        [ChartTemplate(ChartTemplateSet.Simple),Browsable(true),
        Description("Width of the line."),
        Category("Appearance")]
        public float Width
        {
            [DebuggerStepThrough()]
            get
            {
                return (float)GetValue(ChartLineInfoStore.WidthProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(ChartLineInfoStore.WidthProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="Width"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetWidth()
        {
            ResetValue(ChartLineInfoStore.WidthProperty);
        }

        /// <summary>
        /// Should the width of the serialize.
        /// </summary>
        /// <returns>Returns true whether if it should serialize the element else false.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeWidth()
        {
            return HasValue(ChartLineInfoStore.WidthProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Width"/> property has been initialized.
        /// </summary>
        /// <value><c>True</c> if this instance has width; otherwise, <c>false</c>.</value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasWidth
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartLineInfoStore.WidthProperty);
            }
        }

        #endregion

        #region Alignment
        /// <summary>
        /// Gets or sets the pen alignment of the line represented by this object.
        /// </summary>
        /// <value>The alignment.</value>
        [ChartTemplate(ChartTemplateSet.Simple),Browsable(true),
        Description("Gets or sets the pen alignment."),
        Category("Appearance")]
        public PenAlignment Alignment
        {
            get
            {
                return (PenAlignment)GetValue(ChartLineInfoStore.AlignmentProperty);
            }

            set
            {
                SetValue(ChartLineInfoStore.AlignmentProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="Alignment"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetAlignment()
        {
            ResetValue(ChartLineInfoStore.AlignmentProperty);
        }

        /// <summary>
        /// Should the serialize alignment.
        /// </summary>
        /// <returns>Returns true whether if it should serialize the element else false.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeAlignment()
        {
            return HasValue(ChartLineInfoStore.AlignmentProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Alignment"/> property has been initialized.
        /// </summary>
        /// <value>
        ///    <c>True</c> if this instance has alignment; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasAlignment
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartLineInfoStore.AlignmentProperty);
            }
        }
        #endregion

        #region DashStyle
        /// <summary>
        /// Gets or sets the style of the line represented by this object.
        /// <seealso cref="DashStyle"/>
        /// </summary>
        /// <value>The dash style.</value>
        [ChartTemplate(ChartTemplateSet.Simple),Browsable(true),
        Description("Gets or sets the style of the line."),
        Category("Appearance")]
        public DashStyle DashStyle
        {
            [DebuggerStepThrough()]
            get
            {
                return (DashStyle)GetValue(ChartLineInfoStore.DashStyleProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(ChartLineInfoStore.DashStyleProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="DashStyle"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetDashStyle()
        {
            ResetValue(ChartLineInfoStore.DashStyleProperty);
        }

        /// <summary>
        /// Should the serialize dash style.
        /// </summary>
        /// <returns>Returns true whether if it should serialize the element else false.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeDashStyle()
        {
            return HasValue(ChartLineInfoStore.DashStyleProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="DashStyle"/> property has been initialized.
        /// </summary>
        /// <value>
        ///    <c>True</c> if this instance has dash style; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasDashStyle
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartLineInfoStore.DashStyleProperty);
            }
        }
        #endregion

        #region DashPattern
        /// <summary>
        /// Gets or sets the dash pattern of the line represented by this object.
        /// </summary>
        /// <value>The dash pattern.</value>
        [ChartTemplate(ChartTemplateSet.Collection),Browsable(true),
        Description("Gets or sets the dash pattern of the line."),
        Category("Appearance")]
        public float[] DashPattern
        {
            [DebuggerStepThrough()]
            get
            {
                return (float[])GetValue(ChartLineInfoStore.DashPatternProperty);
            }

            [DebuggerStepThrough()]
            set
            {
                SetValue(ChartLineInfoStore.DashPatternProperty, value);
            }
        }

        /// <summary>
        /// Resets the <see cref="DashPattern"/> property.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResetDashPattern()
        {
            ResetValue(ChartLineInfoStore.DashPatternProperty);
        }

        /// <summary>
        /// Should the serialize dash pattern.
        /// </summary>
        /// <returns>
        /// Returns true whether if it should serialize the element else false.
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeDashPattern()
        {
            return HasValue(ChartLineInfoStore.DashPatternProperty);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="DashPattern"/> property has been initialized.
        /// </summary>
        /// <value>
        ///    <c>True</c> if this instance has dash pattern; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasDashPattern
        {
            [DebuggerStepThrough()]
            get
            {
                return HasValue(ChartLineInfoStore.DashPatternProperty);
            }
        }
        #endregion
        #endregion

        #region Class initialize/finalize methods     
        /// <summary>
        /// Initializes the <see cref="ChartLineInfo"/> class.
        /// </summary>
        static ChartLineInfo()
        {
            m_defaultLine = ChartLineInfo.CreateDefault();
        }

        /// <summary>
        /// Overloaded. Constructor.
        /// </summary>
        [DebuggerStepThrough()]
        public ChartLineInfo()
            : base(new ChartLineInfoStore())
        {
        }

        /// <summary>
        /// Initializes a new <see cref="ChartLineInfo"/> object and associates it with an existing <see cref="StyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="StyleInfoSubObjectIdentity"/> that holds the identity for this <see cref="ChartLineInfo"/>.
        /// </param>
        [DebuggerStepThrough()]
        public ChartLineInfo(StyleInfoSubObjectIdentity identity)
            : base(identity, new ChartLineInfoStore())
        {
        }

        /// <summary>
        /// Initializes a new <see cref="ChartLineInfo"/> object and associates it with an existing <see cref="StyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="StyleInfoSubObjectIdentity"/> that holds the identity for this <see cref="ChartLineInfo"/>.
        /// <param name="store">A <see cref="ChartLineInfoStore"/> that holds data for this <see cref="ChartLineInfo"/>.
        /// All changes made in this style object will be saved in the <see cref="ChartLineInfoStore"/> object.</param>
        /// </param>
        [DebuggerStepThrough()]
        public ChartLineInfo(StyleInfoSubObjectIdentity identity, ChartLineInfoStore store)
            : base(identity, store)
        {
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Notifies the associated identity object that a specific property was changed.
        /// </summary>
        /// <param name="sip">Identifies the property to look for.</param>
        protected override void OnStyleChanged(StyleInfoProperty sip)
        {
            base.OnStyleChanged(sip);

            m_updatePen = true;
        }

        /// <summary>
        /// Calculate default style.
        /// </summary>
        /// <returns>Default style.</returns>
        protected override StyleInfoBase GetDefaultStyle()
        {
            return Default;
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
            return new ChartLineInfo(newOwner.CreateSubObjectIdentity(sip), (ChartLineInfoStore)Store.Clone());
        }
        #endregion

        #region Class static methods
        /// <summary>
        /// Resets the changes made in the ChartLineInfo class.
        /// </summary>
        /// <returns>Returns default ChartLineInfo. </returns>
        public static ChartLineInfo CreateDefault()
        {
            ChartLineInfo res = new ChartLineInfo();

            res.Color = SystemColors.ControlText;
            res.Width = 1f;
            res.Alignment = PenAlignment.Center;
            res.DashStyle = DashStyle.Solid;
            res.DashPattern = null;

            return res;
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Calulates the pen. If <see cref="m_updatePen"/> is set to TRUE create new Pen otherwise return <see cref="m_pen"/>.
        /// </summary>
        /// <returns>Pen to draw.</returns>
        private Pen GetGdipPen()
        {
            Pen m_penNew = new Pen(this.Color, this.Width);
            
            if (m_pen == null || m_updatePen)
            {
                if (m_pen == null)
                {
                    m_penNew = new Pen(this.Color, this.Width).Clone() as Pen;

                    m_penNew.LineJoin = LineJoin.Round;
                    m_penNew.StartCap = LineCap.Round;
                    m_penNew.EndCap = LineCap.Round;
                }
                else
                {
                   
                    m_penNew = m_pen.Clone() as Pen;
                    m_penNew.Color = this.Color;
                    m_penNew.Width = this.Width;
                }
               
              
                m_penNew.Alignment = this.Alignment;

                System.Drawing.Drawing2D.DashStyle dashStyle = this.DashStyle;

                if (dashStyle == DashStyle.Custom)
                {
                    if (this.DashPattern != null)
                    {
                        m_penNew.DashStyle = dashStyle;
                        m_penNew.DashPattern = this.DashPattern;
                    }
                }
                else
                {
                    m_penNew.DashStyle = dashStyle;
                }

                m_updatePen = false;
            }

            return m_penNew;
        }
        #endregion
    }
}