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
using System.Security;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    ///   Specifies the style of a specific point border in the <see cref="ChartBorder"/> class.
    /// </summary>
    public enum ChartBorderStyle
    {
        /// <summary>
        ///   No edge style is set.
        /// </summary>
        NotSet = 0, // 0x0000

        /// <summary>
        ///	   No border is drawn.
        /// </summary>
        None,

        /// <summary>
        ///	  Border style that consists of a dashed line.
        ///	</summary>
        Dashed,

        /// <summary>
        ///	   Border style that consists of a dotted line.
        /// </summary>
        Dotted,

        /// <summary>
        ///   Border style that consists of a series of a dash and a dot.
        ///	</summary>
        DashDot,

        /// <summary>
        ///	   Border style that consists of a series of a dash and two dots.
        /// </summary>
        DashDotDot,

        /// <summary>
        ///	   Border style that consists of a solid line.
        /// </summary>
        Solid,

        /// <summary>
        ///	   Use border as specified in the Chart.
        /// </summary>
        Standard,
    }

    /// <summary>
    ///	    Specifies the weight of a specific point border in the <see cref="ChartBorder"/> class.
    /// </summary>
    public enum ChartBorderWeight
    {
        /// <summary>
        ///	   A thin line with 1 pixel.
        /// </summary>
        Thin = 0,

        /// <summary>
        ///  A thin line with dots.
        /// </summary>
        Medium = 1,

        /// <summary>
        ///	  A thick line with 4 pixels.
        /// </summary>
        Thick = 2,
    }

    /// <summary>
    /// The ChartBorder class holds formatting information for the border associated with a point.
    /// <seealso cref="ChartBordersInfo"/>
    /// </summary>
    [
    ImmutableObject(true),
    MergableProperty(true),
    RefreshProperties(RefreshProperties.Repaint)
  
    ]

    public sealed class ChartBorder : ISerializable ////: IFormattable, ICloneable, ISerializable, IXmlSerializable
    {
        private ChartBorderStyle _style;
        private ChartBorderWeight _weight;
        private Color _color;
        private static readonly Color Black = Color.Black;
        private const char separator = ';';
        private static readonly char[] separators = { separator };

        /// <summary>
        /// An empty <see cref="ChartBorder"/> object.
        /// </summary>
        public static readonly ChartBorder Empty = new ChartBorder();

        /// <summary>
        /// Creates an exact copy of this object.
        /// </summary>
        /// <returns>A <see cref="ChartBorder"/> object.</returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <overload>
        ///		Overloaded constructor.
        /// </overload>
        /// <summary>
        ///		Initializes a new instance of the ChartBorder class.
        /// </summary>
        public ChartBorder()
            : this(ChartBorderStyle.NotSet, Black, ChartBorderWeight.Thin)
        {
        }

        /// <summary>
        ///		Initializes a new instance of the ChartBorder class with the specified <see cref="ChartBorderStyle"/>.
        /// </summary>
        /// <param name="style">
        ///		The line style that is to be applied to the border.
        /// </param>
        public ChartBorder(ChartBorderStyle style)
            : this(style, Black, ChartBorderWeight.Thin)
        {
        }

        /// <summary>
        ///		Initializes a new instance of the ChartBorder class with the
        ///		specified <see cref="ChartBorderStyle"/> and
        ///		<see cref="System.Drawing.Color"/>.
        /// </summary>
        /// <param name="style">
        ///		A <see cref="ChartBorderStyle"/> that is to be applied to the border.
        /// </param>
        /// <param name="color">
        ///		A <see cref="System.Drawing.Color"/> specifying the color of the border.
        /// </param>
        public ChartBorder(ChartBorderStyle style, Color color)
            : this(style, color, ChartBorderWeight.Thin)
        {
        }

        /// <summary>
        ///		Initializes a new instance of the ChartBorder class with the
        ///		specified <see cref="ChartBorderStyle"/> and
        ///		<see cref="System.Drawing.Color"/>.
        /// </summary>
        /// <param name="style">
        ///		A <see cref="ChartBorderStyle"/> that is to be applied to the border.
        /// </param>
        /// <param name="color">
        ///		A <see cref="System.Drawing.Color"/> specifying the color of the border.
        /// </param>
        /// <param name="weight">
        ///		A <see cref="ChartBorderWeight"/> specifying the thickness of the border.
        ///	</param>
        public ChartBorder(ChartBorderStyle style, Color color, ChartBorderWeight weight)
        {
            if (!Enum.IsDefined(typeof(ChartBorderStyle), style))
            {
                throw new ArgumentException("Invalid ChartBorderStyle value.");
            }

            _style = style;

            if (!Enum.IsDefined(typeof(ChartBorderWeight), weight))
            {
                throw new ArgumentException("Invalid ChartBorderWeight value.");
            }

            _weight = weight;

            _color = color;
        }

        /// <summary>
        /// Initializes a new <see cref="ChartBorder"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        private ChartBorder(SerializationInfo info, StreamingContext context)
        {
            _style = (ChartBorderStyle)info.GetValue("Style", typeof(ChartBorderStyle));
            _weight = (ChartBorderWeight)info.GetValue("Weight", typeof(ChartBorderWeight));
            _color = (Color)info.GetValue("Color", typeof(Color));
        }

        /// <summary>
        /// Implements the ISerializable interface and returns the data needed to serialize the <see cref="ChartBorder"/>.
        /// </summary>
        /// <param name="info">A SerializationInfo object containing the information required to serialize the object.</param>
        /// <param name="context">A StreamingContext object containing the source and destination of the serialized stream.</param>
        [SecurityCritical]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Style", _style); // ChartBorderStyle
            info.AddValue("Weight", _weight); // ChartBorderWeight
            info.AddValue("Color", _color); // Color
        }

        /// <summary>
        /// Returns a copy of this border object replacing the color with <see cref="System.Drawing.Color.Black"/>.
        /// </summary>
        /// <returns>A black colored <see cref="ChartBorder"/>.</returns>
        public ChartBorder MakeBlackAndWhite()
        {
            ChartBorder border = new ChartBorder(this.Style, Color.Black, this.Weight);
            return border;
        }

        /// <summary>
        /// Returns a compact string representation of the ChartBorder.
        /// All information in ChartBorder will be encoded.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <overload>
        /// Overloaded. Overridden. Returns a compact string representation of the ChartBorder.
        /// All information in ChartBorder will be encoded.
        /// </overload>
        public override string ToString()
        {
            return base.ToString();
        }

        /// <summary>
        /// Returns a compact string representation of the ChartBorder.
        /// All information in ChartBorder will be encoded.
        /// </summary>
        /// <param name="format">Format in which string representation should be rendered. "compact" for compact text; default is full text version.</param>
        /// <returns>The String.</returns>
        public string ToString(string format)
        {
            return base.ToString();
        }

        /// <summary>
        ///		Overridden. Returns True if the ChartBorder object passed is equal.
        /// </summary>
        /// <param name="o">The object to compare to.</param>
        /// <returns>
        ///		True if both are equal; false otherwise.
        ///	</returns>
        public override bool Equals(object o)
        {
            bool eq = false;

            if ((object)this == o)
            {
                return true;
            }

            if (o is ChartBorder)
            {
                ChartBorder border = (ChartBorder)o;

                eq = (border.Color == this.Color &&
                    border.Style == this.Style &&
                    border.Weight == this.Weight);
            }

            return eq;
        }

        /// <summary cref="Equals">
        ///		The basic == operator.
        /// </summary>
        /// <param name="lhs">The left-hand side of the operator.</param>
        /// <param name="rhs">The right-hand side of the operator.</param>
        /// <returns>
        ///		Boolean value.
        ///	</returns>
        public static bool operator ==(ChartBorder lhs, ChartBorder rhs)
        {
            if ((object)lhs == null || (object)rhs == null)
            {
                return false;
            }

            return lhs.Equals(rhs);
        }

        /// <summary cref="Equals">
        ///		The basic != operator.
        /// </summary>
        /// <param name="lhs">The left-hand side of the operator.</param>
        /// <param name="rhs">The right-hand side of the operator.</param>
        /// <returns>
        ///		Boolean value.
        ///	</returns>
        public static bool operator !=(ChartBorder lhs, ChartBorder rhs)
        {
            if ((object)lhs == null || (object)rhs == null)
            {
                return false;
            }

            return !lhs.Equals(rhs);
        }

        /// <summary cref="Equals">
        ///		The basic == operator.
        /// </summary>
        /// <param name="lhs">The left-hand side of the operator.</param>
        /// <param name="rhs">The right-hand side of the operator.</param>
        /// <returns>
        ///		Boolean value.
        ///	</returns>
        public static bool Compare(ChartBorder lhs, ChartBorder rhs)
        {
            if ((object)lhs == null || (object)rhs == null)
            {
                return false;
            }

            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Overridden. Returns the hash code for the current ChartBorder instance.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return (((Int32)this.Style) << 28) |
                (((Int32)this.Weight) << 24) |
                (((Int32)this.Color.ToArgb()) & 0x00fffff);
        }

        /// <summary>
        /// gets whether this ChartBorder is uninitialized.
        /// </summary>
        /// <value><c>True</c> if this instance is empty; otherwise, <c>false</c>.</value>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]

        public bool IsEmpty
        {
            get
            {
                return this == Empty;
            }
        }

        /// <summary>
        /// Gets what type of border line style this border has. This value
        /// comes from the <see cref="ChartBorderStyle"/> enumeration.
        /// </summary>
        /// <value>The style.</value>
        [
        MergableProperty(true),
        NotifyParentProperty(true)
        ]

        public ChartBorderStyle Style
        {
            get
            {
                return (ChartBorderStyle)_style;
            }
        }

        /// <summary>
        /// Gets the weight of the border the chart.  This value
        /// comes from the <see cref="ChartBorderWeight"/>  enumeration.
        /// </summary>
        /// <value>The weight.</value>
        [
        MergableProperty(true),
        NotifyParentProperty(true)
        ]
        public ChartBorderWeight Weight
        {
            get
            {
                return (ChartBorderWeight)_weight;
            }
        }

        /// <summary>
        /// Specifies the color of the chart border. This value
        /// comes from the <see cref="System.Drawing.Color"/>  enumeration.
        /// </summary>
        /// <value>The color.</value>
        [
        MergableProperty(true),
        NotifyParentProperty(true)
        ]
        public Color Color
        {
            get
            {
                return _color;
            }
        }

        /// <summary>
        /// Gets the width in pixels of the chart border.
        /// </summary>
        /// <value>The width.</value>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]

        public int Width
        {
            get
            {
                int width = 1;

                switch (this.Weight)
                {
                    case ChartBorderWeight.Medium:
                        width = 2;
                        break;

                    case ChartBorderWeight.Thick:
                        width = 4;
                        break;

                    case ChartBorderWeight.Thin:
                        width = 1;
                        break;
                }

                return width;
            }
        }
    }
}