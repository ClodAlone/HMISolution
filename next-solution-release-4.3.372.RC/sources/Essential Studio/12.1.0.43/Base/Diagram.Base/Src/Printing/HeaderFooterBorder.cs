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
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Encapsulates the header/footer border properties.
    /// </summary>
    /// <remarks>
    /// Contains properties needed to specify the border drawing for the diagram headers and footers.
    /// </remarks>
    [
    Serializable,
    TypeConverter(typeof(HFBorderStyleConverter))
    ]
    public sealed class HeaderFooterBorder : ISerializable, ICloneable
    {
        /// <summary>
        /// An empty <see cref="HeaderFooterBorder"/> object.
        /// </summary>
        public static readonly HeaderFooterBorder Empty = new HeaderFooterBorder();

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderFooterBorder"/> class.
        /// </summary>
        public HeaderFooterBorder()
            : this(DashStyle.Solid, Black, BorderWeight.Thin)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderFooterBorder"/> class.
        /// </summary>
        /// <param name="style">The line style to be be applied to the border.</param>
        public HeaderFooterBorder(DashStyle style)
            : this(style, Black, BorderWeight.Thin)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderFooterBorder"/> class.
        /// </summary>
        /// <param name="style">A <see cref="DashStyle"/> to be be applied to the specific border.</param>
        /// <param name="color">A <see cref="System.Drawing.Color"/> specifying the color of the border.</param>
        public HeaderFooterBorder(DashStyle style, Color color)
            : this(style, color, BorderWeight.Thin)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderFooterBorder"/> class.
        /// </summary>
        /// <param name="style">A <see cref="DashStyle"/> to be be applied to the specific border.</param>
        /// <param name="color">A <see cref="System.Drawing.Color"/> specifying the color of the border.</param>
        /// <param name="weight">A <see cref="BorderWeight"/> specifying the thickness of the border.</param>
        public HeaderFooterBorder(DashStyle style, Color color, BorderWeight weight)
        {
            if (!Enum.IsDefined(typeof(DashStyle), style))
            {
                throw new ArgumentException("Invalid HFBorderStyle value.");
            }
            this.style = style;

            if (!Enum.IsDefined(typeof(BorderWeight), weight))
            {
                throw new ArgumentException("Invalid BorderWeight value.");
            }
            this.weight = weight;

            this.color = color;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderFooterBorder"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        private HeaderFooterBorder(SerializationInfo info, StreamingContext context)
        {
            this.style = (DashStyle)info.GetValue("Style", typeof(DashStyle));
            try
            {
                this.weight = (BorderWeight)info.GetValue("Weight", typeof(BorderWeight));
            }
            catch (Exception)
            {
                this.weight = BorderWeight.Thin;
            }
            this.color = (Color)info.GetValue("Color", typeof(Color));
            this.showborder = info.GetBoolean("showborder");
        }

        #endregion

        #region Serialization

        /// <summary>
        /// Implements the ISerializable interface and returns the data needed to serialize the <see cref="HeaderFooterBorder"/>.
        /// </summary>
        /// <param name="info">A SerializationInfo object containing the information required to serialize the object.</param>
        /// <param name="context">A StreamingContext object containing the source and destination of the serialized stream.</param>
        [Documentation.DocumentationExclude()]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Style", this.style); // BorderStyle
            info.AddValue("Weight", this.weight); // BorderWeight
            info.AddValue("Color", this.color); // Color
            info.AddValue("showborder", this.showborder);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        public override int GetHashCode()
        {
            return (((int)this.Style) << 28) |
                (((int)this.Weight) << 24) |
                (this.Color.ToArgb() & 0x00fffff);
        }

        /// <summary>
        /// Equals the specified object.
        /// </summary>
        /// <param name="o">The object.</param>
        /// <returns>true, if equal.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        public override bool Equals(object o)
        {
            bool eq = false;

            // if(this.Equals(o))
            // {
            //      return true;
            // }
            if (o is HeaderFooterBorder)
            {
                HeaderFooterBorder border = (HeaderFooterBorder)o;

                eq = (border.Color == this.Color && border.Style == this.Style && border.Weight == this.Weight);
            }

            return eq;
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="lhs">The first header.</param>
        /// <param name="rhs">The second header.</param>
        /// <returns>The result of the operator.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        public static bool operator ==(HeaderFooterBorder lhs, HeaderFooterBorder rhs)
        {
            if ((object)lhs == null || (object)rhs == null)
            {
                return false;
            }
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="lhs">The first header.</param>
        /// <param name="rhs">The right header.</param>
        /// <returns>The result of the operator.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        public static bool operator !=(HeaderFooterBorder lhs, HeaderFooterBorder rhs)
        {
            if ((object)lhs == null || (object)rhs == null)
            {
                return true;
            }
            return !lhs.Equals(rhs);
        }

        /// <summary>
        /// Compares the specified two instance HeaderFooter borders.
        /// </summary>
        /// <param name="lhs">The first header.</param>
        /// <param name="rhs">The second header.</param>
        /// <returns>true, if equal.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        public static bool Compare(HeaderFooterBorder lhs, HeaderFooterBorder rhs)
        {
            if ((object)lhs == null || (object)rhs == null)
            {
                return false;
            }
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Returns a copy of this border object replacing the color with <see cref="System.Drawing.Color.Black"/>.
        /// </summary>
        /// <returns>A <see cref="HeaderFooterBorder"/> with black color.</returns>
        public HeaderFooterBorder MakeBlackAndWhite()
        {
            HeaderFooterBorder border = new HeaderFooterBorder(this.Style, Color.Black, this.Weight);
            return border;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the type of line used for drawing the border. 
        /// </summary>
        /// <value>
        /// A <see cref="DashStyle"/> value.
        /// </value>
        [
        Browsable(true),
        DefaultValue(DashStyle.Solid),
        Description("The type of line used for drawing the border.")
        ]
        public DashStyle Style
        {
            get { return style; }
            set { style = value; }
        }

        /// <summary>
        /// Gets or sets the thickness of the line used for drawing the border.
        /// </summary>
        /// <value>A <see cref="Syncfusion.Windows.Forms.Diagram.BorderWeight"/> value.</value>
        [
        Browsable(true),
        DefaultValue(BorderWeight.Thin),
        Description("The thickness of the border line.")
        ]
        public BorderWeight Weight
        {
            get
            {
                return weight;
            }
            set
            {
                weight = value;
            }
        }

        /// <summary>
        /// Gets the width in pixels for the border line.
        /// </summary>
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
                    case BorderWeight.Medium:
                        width = 2;
                        break;

                    case BorderWeight.Thick:
                        width = 4;
                        break;

                    case BorderWeight.Thin:
                        width = 1;
                        break;
                }
                return width;
            }
        }

        /// <summary>
        /// Gets or sets the color of the line used for drawing the border.
        /// </summary>
        /// <value>A <see cref="System.Drawing.Color"/> value.</value>
        [
        Browsable(true),
        Description("The color used for the border line.")
        ]
        public Color Color
        {
            get
            {
                return this.color;
            }
            set
            {
                this.color = value;
            }
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        private bool ShouldSerializeColor()
        {
            return (this.color != Color.Black);
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        private void ResetColor()
        {
            this.color = Color.Black;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the border will be displayed.
        /// </summary>
        [
        Browsable(true),
        DefaultValue(false),
        Description("Indiciates whether border should be displayed.")
        ]
        public bool ShowBorder
        {
            get { return showborder; }
            set { showborder = value; }
        }

        #endregion

        /// <summary>
        /// Creates the pen used for drawing the border.
        /// </summary>
        /// <returns>System.Drawing.Pen object matching the border style.</returns>
        public Pen CreatePen()
        {
            Pen retvalue = null;
            if (showborder)
            {
                retvalue = new Pen(this.Color, this.Width);
                retvalue.DashStyle = this.Style;
            }
            return retvalue;
        }

        #region Fields

        private DashStyle style;
        private BorderWeight weight;
        private Color color;
        private bool showborder;
        private static readonly Color Black = Color.Black;

        #endregion

        #region ICloneable Members

        /// <summary>
        /// Creates a shallow copy of the HeeaderFooterBorder object.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            return MemberwiseClone();
        }

        #endregion
    }
}