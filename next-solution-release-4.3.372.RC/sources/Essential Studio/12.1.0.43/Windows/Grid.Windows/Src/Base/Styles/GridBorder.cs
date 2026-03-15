//-------------------------------------------------------------------------------------------------
// <copyright file="GridBorder.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Text;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;

using Syncfusion.Diagnostics;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    ///        Specifies the style of a specific cell border in the <see cref="GridBorder"/> class.
    /// </summary>
    [Editor(typeof(GridBorderStyleEditor), typeof(System.Drawing.Design.UITypeEditor))]
    public enum GridBorderStyle
    {
        /// <summary>
        ///        No edge style is set.
        /// </summary>
        NotSet = 0, // 0x0000

        /// <summary>
        ///        No border is drawn on the cell.
        /// </summary>
        None,

        /// <summary>
        ///        Border style that consists of a dotted line: ...........
        /// </summary>
        Dashed,

        /// <summary>
        ///        Border style that consists of a dash and a dot: _._._.
        /// </summary>
        Dotted,

        /// <summary>
        ///        Border style that consists of a series of dashed lines: --------
        /// </summary>
        DashDot,

        /// <summary>
        ///        Border style that consists of a dash and two dots: _.._.._..
        /// </summary>
        DashDotDot,

        /// <summary>
        ///        Border style that consists of a solid line: ___________
        /// </summary>
        Solid,

        /// <summary>
        ///        Use border as specified in grid property.
        /// </summary>
        Standard,
    }

    /// <summary>
    ///        Specifies the weight of a specific cell border in the <see cref="GridBorder"/> class.
    /// </summary>
    [Editor(typeof(GridBorderWeightEditor), typeof(System.Drawing.Design.UITypeEditor))]
    public enum GridBorderWeight
    {
        /// <summary>
        ///        A line with 0.25 point.
        /// </summary>
        ExtraThin = 1,

        /// <summary>
        ///        A line with 0.5 point.
        /// </summary>    
        Thin = 2,

        /// <summary>
        ///        A line with 1 point.
        /// </summary>
        Medium = 3,

        /// <summary>
        ///        A line with 1.5 points.
        /// </summary>
        Thick = 4,
             
        /// <summary>
        ///        A line with 2.0 points.
        /// </summary>
        ExtraThick = 5,
     
        /// <summary>
        ///        A line with 3.0 points.
        /// </summary>
        ExtraExtraThick = 6,
    }

    /// <summary>
    /// The immutable GridBorder class holds formatting information for individual borders of a cell.
    /// </summary>
    /// <remarks>
    /// You can assign <see cref="GridBorder"/> objects to individual borders of a <see cref="GridStyleInfo.Borders"/>
    /// object in a <see cref="GridStyleInfo"/> object.<para/> The <see cref="GridBordersInfo"/> class lets you assign
    /// a different <see cref="GridBorder"/> to each <see cref="GridBorderSide"/>.
    /// <para/>
    /// GridBorder is serializable and ToString and IFormattable are implemented.
    /// </remarks>
    /// <example>
    /// The following code changes border information for cells:
    /// <code lang="C#">
    /// <para/>
    ///             GridBorder border = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(57, 73, 122));
    ///             model[rowIndex, colIndex].Borders.Bottom = border;
    ///             model[rowIndex, colIndex].Borders.Right = border;
    /// </code>
    /// The following code hides grid lines for specific cells:
    /// <code lang="C#">
    ///             GridBorder border = new GridBorder(GridBorderStyle.None);
    ///             model[rowIndex, colIndex].Borders.Bottom = border;
    ///             model[rowIndex, colIndex].Borders.Right = border;
    /// </code>
    /// </example>
    [TypeConverter(typeof(GridBorderConverter)),
        DesignerSerializer(
            typeof(GridBorderCodeDomSerializer),
            typeof(System.ComponentModel.Design.Serialization.CodeDomSerializer)),
        Serializable,
        ImmutableObject(true),
        MergableProperty(true),
        RefreshProperties(RefreshProperties.Repaint)]
    public sealed class GridBorder : IFormattable, ICloneable, ISerializable, IXmlSerializable
    {
        private GridBorderStyle _style;
        private GridBorderWeight _weight;
        private Color _color;
        private static readonly Color Black = Color.Black;
        private const char separator = ';';
        private static readonly char[] separators = { separator };

        /// <summary>
        /// An empty <see cref="GridBorder"/> object.
        /// </summary>
        public readonly static GridBorder Empty = new GridBorder();

        /// <summary>
        /// Creates an exact copy of this object.
        /// </summary>
        /// <returns>A <see cref="GridBorder"/> object.</returns>
        public object Clone()
        {
            GridBorder border = new GridBorder();
            border._color = _color;
            border._style = _style;
            border._weight = _weight;
            return border;
        }

        /// <overload>
        ///        Initializes a new instance of the GridBorder class.
        /// </overload>
        /// <summary>
        ///        Initializes a new instance of the GridBorder class.
        /// </summary>
        public GridBorder()
            : this(GridBorderStyle.NotSet, Black, GridBorderWeight.Thin)
        {
        }

        /// <summary>
        ///        Initializes a new instance of the GridBorder class with the specified <see cref="GridBorderStyle"/>.
        /// </summary>
        /// <param name="style">
        ///        The line style to be be applied to the specific border in a cell.
        /// </param>
        public GridBorder(GridBorderStyle style)
            : this(style, Black, GridBorderWeight.Thin)
        {
        }

        /// <summary>
        ///        Initializes a new instance of the GridBorder class with the
        ///        specified <see cref="GridBorderStyle"/> and
        ///        <see cref="System.Drawing.Color"/>.
        /// </summary>
        /// <param name="style">
        ///        A <see cref="GridBorderStyle"/> to be be applied to the specific border in a cell.
        /// </param>
        /// <param name="color">
        ///        A <see cref="System.Drawing.Color"/> specifying the color of the border.
        /// </param>
        public GridBorder(GridBorderStyle style, Color color)
            : this(style, color, GridBorderWeight.Thin)
        {
        }

        /// <summary>
        ///        Initializes a new instance of the GridBorder class with the
        ///        specified <see cref="GridBorderStyle"/> and
        ///        <see cref="System.Drawing.Color"/>.
        /// </summary>
        /// <param name="style">
        ///        A <see cref="GridBorderStyle"/> to be be applied to the specific border in a cell.
        /// </param>
        /// <param name="color">
        ///        A <see cref="System.Drawing.Color"/> specifying the color of the border.
        /// </param>
        /// <param name="weight">
        ///        A <see cref="GridBorderWeight"/> specifying the thickness of the border.
        /// </param>
        public GridBorder(GridBorderStyle style, Color color, GridBorderWeight weight)
        {
            if (!Enum.IsDefined(typeof(GridBorderStyle), style))
            {
                throw new ArgumentException("Invalid GridBorderStyle value");
            }

            _style = style; ////(sbyte) style;

            if (!Enum.IsDefined(typeof(GridBorderWeight), weight))
            {
                throw new ArgumentException("Invalid GridBorderWeight value");
            }

            _weight = weight; ////(sbyte) weight;

            _color = color;
        }

        /// <summary>
        /// Initializes a new <see cref="GridBorder"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        internal GridBorder(SerializationInfo info, StreamingContext context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
            _style = (GridBorderStyle)info.GetValue("Style", typeof(GridBorderStyle));
            _weight = (GridBorderWeight)info.GetValue("Weight", typeof(GridBorderWeight));
            _color = (Color)info.GetValue("Color", typeof(Color));
        }

        /// <summary>
        /// Implements the ISerializable interface and returns the data needed to serialize the <see cref="GridBorder"/>.
        /// </summary>
        /// <param name="info">A SerializationInfo object containing the information required to serialize the object.</param>
        /// <param name="context">A StreamingContext object containing the source and destination of the serialized stream.</param>
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
            info.AddValue("Style", _style); // GridBorderStyle
            info.AddValue("Weight", _weight); // GridBorderWeight
            info.AddValue("Color", _color); // Color
        }

        /// <summary>
        /// Returns a copy of this border object replacing the color with <see cref="System.Drawing.Color.Black"/>.
        /// </summary>
        /// <returns>A <see cref="GridBorder"/> with black color.</returns>
        public GridBorder MakeBlackAndWhite()
        {
            GridBorder border = new GridBorder(this.Style, Color.Black, this.Weight);
            return border;
        }

        /// <summary>
        /// Gets of ToString method.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Info
        {
            get
            {
                return ToString();
            }
        }

        /// <overload>
        ///        Returns a compact string representation of the GridBorder.
        ///        All information in GridBorder will be encoded.
        /// </overload>
        /// <summary>
        ///        Returns a compact string representation of the GridBorder.
        ///        All information in GridBorder will be encoded.
        /// </summary>
        /// <returns>
        ///        A String that can be passed to <see cref="GridBorder.Parse"/>.
        /// </returns>
        public override string ToString()
        {
            return this.ToString(null, null);
        }

        /// <summary>
        ///        Returns a compact string representation of the GridBorder.
        ///        All information in GridBorder will be encoded.
        /// </summary>
        /// <param name="provider">The Format Provider. Ignored.</param>
        /// <returns>
        ///   A String that can be passed to <see cref="GridBorder.Parse"/>.
        /// </returns>      
        public string ToString(IFormatProvider provider)
        {
            return this.ToString(null, provider);
        }

        /// <summary>
        ///        Returns a compact string representation of the GridBorder.
        ///        All information in GridBorder will be encoded.
        /// </summary>
        /// <returns>
        ///        A String that can be passed to <see cref="GridBorder.Parse"/>.
        /// </returns>
        /// <param name="format">"Compact" for compact text; default is full text version.</param>
        public string ToString(string format)
        {
            return this.ToString(format, null);
        }

        /// <summary>
        ///        Returns a compact string representation of the GridBorder.
        ///        All information in GridBorder will be encoded.
        /// </summary>
        /// <param name="format">"Compact" for compact text; default is full text version.</param>
        /// <param name="formatProvider">Format Provider. Ignored.</param>
        /// <returns>
        ///        A String that can be passed to <see cref="GridBorder.Parse"/>.
        /// </returns>       
        public string /*IFormattable*/ ToString(string format, IFormatProvider formatProvider)
        {
            string separate = separator.ToString() + " ";
            if (format != null && format == "compact")
            {
                if (Style == GridBorderStyle.None || Style == GridBorderStyle.NotSet)
                {
                    return Style.ToString();
                }
                else
                {
                    return String.Concat(new string[]
                    {
                        Style.ToString(),
                        separator.ToString(),
                        ColorConvert.ColorToString(Color, false),
                        separator.ToString(),
                        Weight.ToString()
                    });
                }
            }
            else
            {
                if (Style == GridBorderStyle.None || Style == GridBorderStyle.NotSet || Style == GridBorderStyle.Standard)
                {
                    return Enum.Format(typeof(GridBorderStyle), Style, "G");
                }
                else
                {
                    return String.Concat(new string[]
                    {
                        Enum.Format(typeof(GridBorderStyle), Style, "G"),
                        separate,
                        ColorConvert.ColorToString(Color, true),
                        separate,
                        Enum.Format(typeof(GridBorderWeight), Weight, "G"),
                    });
                }
            }
        }

        /// <summary>
        ///        Creates a GridBorder from a string.
        /// </summary>
        /// <remarks>
        ///        The output from <see cref="GridBorder.ToString()"/> should be consumable by <see cref="Parse"/>.
        /// </remarks>
        /// <param name="parseStr">The string to parse.</param>
        /// <returns>
        ///        A <see cref="GridBorder"/> that corresponds to parseStr.
        /// </returns>
        public static GridBorder Parse(string parseStr)
        {
            GridBorder border = new GridBorder();
            return border.ReadString(parseStr);
        }

        GridBorder ReadString(string parseStr)
        {
            GridBorder border = this;
            int n = parseStr.IndexOf("{");
            if (n != -1 && parseStr.EndsWith("}"))
            {
                parseStr = parseStr.Substring(n + 1, parseStr.Length - n - 2);
            }

            string[] words = parseStr.Split(separators);
            int wordCount = words.GetLength(0);

            n = 0;

            if (wordCount >= 1)
            {
                // Style.Set throws ArgumentException if value is invalid.
                border._style = (GridBorderStyle)Enum.Parse(typeof(GridBorderStyle), words[n]);
                if (wordCount == ++n)
                {
                    return border;
                }

                Color color;
                if (Char.IsDigit(words[n].Trim()[0]) && n + 3 <= wordCount)
                {
                    color = ColorConvert.ColorFromString(words[n] + ";" + words[n + 1] + ";" + words[n + 2]);
                    n += 2;
                }
                else
                {
                    color = ColorConvert.ColorFromString(words[n]);
                }

                border._color = color;
                if (wordCount == ++n)
                {
                    return border;
                }

                // Weight.Set throws ArgumentException if value is invalid.
                border._weight = (GridBorderWeight)Enum.Parse(typeof(GridBorderWeight), words[n]);

                if (wordCount != ++n)
                {
                    throw new FormatException("obsolete arguments: " + words[n]);
                }
            }

            return border;
        }

        /// <summary>
        ///        Return True if the GridBorder object passed is equal.
        /// </summary>
        /// <param name="o">The object to compare to.</param>
        /// <returns>
        ///        True if it is equal; False otherwise.
        /// </returns>
        public override bool Equals(object o)
        {
            bool eq = false;

            if ((object)this == o)
            {
                return true;
            }

            if (o is GridBorder)
            {
                GridBorder border = (GridBorder)o;

                eq = border.Color == this.Color &&
                      border.Style == this.Style &&
                      border.Weight == this.Weight;
            }

            return eq;
        }

        /// <summary cref="Equals">
        ///        The basic == operator.
        /// </summary>
        /// <param name="lhs">The left-hand side of the operator.</param>
        /// <param name="rhs">The right-hand side of the operator.</param>
        /// <returns>
        ///      returns bool
        /// </returns>
        public static bool operator ==(GridBorder lhs, GridBorder rhs)
        {
            if (Object.ReferenceEquals(lhs, rhs))
            {
                return true;
            }

            if ((object)lhs == null || (object)rhs == null)
            {
                return false;
            }

            return lhs.Equals(rhs);
        }

        /// <summary cref="Equals">
        ///        The basic != operator.
        /// </summary>
        /// <param name="lhs">The left-hand side of the operator.</param>
        /// <param name="rhs">The right-hand side of the operator.</param>
        /// <returns>
        ///   returns bool
        /// </returns>
        public static bool operator !=(GridBorder lhs, GridBorder rhs)
        {
            if (Object.ReferenceEquals(lhs, rhs))
            {
                return false;
            }

            if ((object)lhs == null || (object)rhs == null)
            {
                return true;
            }

            return !lhs.Equals(rhs);
        }

        /// <summary cref="Equals">
        ///        The basic == operator.
        /// </summary>
        /// <param name="lhs">The left-hand side of the operator.</param>
        /// <param name="rhs">The right-hand side of the operator.</param>
        /// <returns>
        ///  returns bool
        /// </returns>
        public static bool Compare(GridBorder lhs, GridBorder rhs)
        {
            if (Object.ReferenceEquals(lhs, rhs))
            {
                return true;
            }

            if ((object)lhs == null || (object)rhs == null)
            {
                return false;
            }

            return lhs.Equals(rhs);
        }

        /// <summary>
        ///        Returns the hash code for the current GridBorder instance.
        /// </summary>
        /// <returns>Hash code.</returns>
        public override int GetHashCode()
        {
            return (((Int32)this.Style) << 28) |
                (((Int32)this.Weight) << 24) |
                (((Int32)this.Color.ToArgb()) & 0x00fffff);
        }

        /// <summary>
        ///  Gets a value indicating whether this GridBorder is uninitialized.
        /// </summary>
        [Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsEmpty
        {
            get { return this == Empty; }
        }

        /// <summary>
        ///     Gets what type of border line the cell has. This value
        ///     comes from the <see cref="GridBorderStyle"/> enumeration.
        /// </summary>
        [Description("Indicates the type of border line the cell has."),
            MergableProperty(true),
            NotifyParentProperty(true)]
        public GridBorderStyle Style
        {
            get { return (GridBorderStyle)_style; }
            ////            set {
            ////                if (!Enum.IsDefined(typeof(GridBorderStyle), value))
            ////                    throw new ArgumentException("Invalid GridBorderStyle value");
            ////                _style = value; //(sbyte) value;
            ////            }
        }

        /// <summary>
        ///     Gets the weight of the border the cell has. This value
        ///     comes from the <see cref="GridBorderWeight"/> enumeration.
        /// </summary>
        [Description("Indicates the weight of the border the cell has."),
            MergableProperty(true),
            NotifyParentProperty(true)]
        public GridBorderWeight Weight
        {
            get { return (GridBorderWeight)_weight; }
            ////            set {
            ////                if (!Enum.IsDefined(typeof(GridBorderWeight), value))
            ////                    throw new ArgumentException("Invalid GridBorderWeight value");
            ////                _weight = value; // (sbyte) value;
            ////            }
        }

        /// <summary>
        ///     Gets the color to be used for the cell border. This value
        ///     comes from the <see cref="System.Drawing.Color"/> enumeration.
        /// </summary>
        [Description("Indicates the color to be used for the cell border."),
            MergableProperty(true),
            NotifyParentProperty(true)]
        public Color Color
        {
            get { return _color; }
            ////            set { _color = value; }
        }
        
        /// <summary>
        ///    Gets the width in pixels for the cell border.
        /// </summary>
        [Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Width
        {
            get
            {
                int width = 1;
                switch (this.Weight)
                {
                    case GridBorderWeight.ExtraThin:
                        width = 1;
                        break;

                    case GridBorderWeight.Thin:
                        width = 1;
                        break;

                    case GridBorderWeight.Medium:
                        width = 2;
                        break;

                    case GridBorderWeight.Thick:
                        width = 2;
                        break;

                    case GridBorderWeight.ExtraThick:
                        width = 3;
                        break;

                    case GridBorderWeight.ExtraExtraThick:
                        width = 4;
                        break;
                }

                return width;
            }
        }

        #region IXmlSerializable Members

        /// <summary>
        /// Serializes the contents of this object into an XML stream.
        /// </summary>
        /// <param name="writer">Represents the XML stream.</param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteString(this.ToString());
        }

        /// <summary>
        /// Not implemented and returns NULL.
        /// </summary>
        /// <returns>returns xml schema</returns>
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Deserializes the contents of this object from an XML stream.
        /// </summary>
        /// <param name="reader">Represents the XML stream.</param>
        public void ReadXml(XmlReader reader)
        {
            string s = reader.ReadString();
            this.ReadString(s);
        }
        #endregion
    }
}
