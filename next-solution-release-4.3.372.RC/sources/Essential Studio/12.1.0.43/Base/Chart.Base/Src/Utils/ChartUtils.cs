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
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;

using Syncfusion.Documentation;
using Syncfusion.Drawing;
using System.Collections.Generic;
using System.Drawing.Imaging;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Default type converter.
    /// </summary>
    public class ChartInstanceConverter : TypeConverter
    {
        #region Public methods
        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="destinationType">A <see cref="T:System.Type"></see> that represents the type you want to convert to.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }
        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo"></see>. If null is passed, the current culture is assumed.</param>
        /// <param name="value">The <see cref="T:System.Object"></see> to convert.</param>
        /// <param name="destinationType">The <see cref="T:System.Type"></see> to convert the value parameter to.</param>
        /// <returns>
        /// An <see cref="T:System.Object"></see> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        /// <exception cref="T:System.ArgumentNullException">The destinationType parameter is null. </exception>
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor))
            {
                ConstructorInfo info1 = value.GetType().GetConstructor(Type.EmptyTypes);
                return new InstanceDescriptor(info1, null, false);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
        #endregion
    }

    /// <summary>
    /// Specifies a range of indices.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public struct IndexRange
    {
        #region Members
        private int m_from;
        private int m_to;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the first index of range.
        /// </summary>
        /// <value>From.</value>
        public int From
        {
            get
            {
                return m_from;
            }
            set
            {
                m_from = value;
            }
        }
        /// <summary>
        /// Gets or sets the last index of range.
        /// </summary>
        /// <value>To.</value>
        public int To
        {
            get
            {
                return m_to;
            }
            set
            {
                m_to = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="IndexRange"/> class.
        /// </summary>
        /// <param name="from">The first index of range.</param>
        /// <param name="to">The lase index of range.</param>
        public IndexRange(int from, int to)
        {
            m_from = from;
            m_to = to;
        }
        #endregion
    }

    /// <summary>
    /// Converts instances of other types to and from a <see cref="ChartMargins"/>. 
    /// </summary>
    /// <internalonly/>
    [DocumentationExclude()]
    public class ChartMarginsConverter : TypeConverter
    {
        #region Constants
        private const string PROP_LEFT = "Left";
        private const string PROP_TOP = "Top";
        private const string PROP_RIGHT = "Right";
        private const string PROP_BOTTOM = "Bottom";
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartMarginsConverter"/> class.
        /// </summary>
        public ChartMarginsConverter()
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="destinationType">A <see cref="T:System.Type"></see> that represents the type you want to convert to.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return base.CanConvertTo(context, destinationType) || (destinationType == typeof(InstanceDescriptor));
        }
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="T:System.Type"></see> that represents the type you want to convert from.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return base.CanConvertFrom(context, sourceType) || sourceType == typeof(string);
        }
        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo"></see> to use as the current culture.</param>
        /// <param name="value">The <see cref="T:System.Object"></see> to convert.</param>
        /// <returns>
        /// An <see cref="T:System.Object"></see> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                string sValue = ((string)value).Trim();

                if (sValue != "")
                {
                    if (culture == null)
                    {
                        culture = CultureInfo.CurrentCulture;
                    }

                    string[] sAttrs = sValue.Split(new char[] { culture.TextInfo.ListSeparator[0] });
                    int[] nAttrs = new int[sAttrs.Length];
                    TypeConverter intConverter = TypeDescriptor.GetConverter(typeof(int));

                    for (int i = 0; i < nAttrs.Length; i++)
                    {
                        nAttrs[i] = (int)intConverter.ConvertFromString(context, culture, sAttrs[i]);
                    }

                    return new ChartMargins(nAttrs[0], nAttrs[1], nAttrs[2], nAttrs[3]);
                }

                return null;
            }

            return base.ConvertFrom(context, culture, value);
        }
        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo"></see>. If null is passed, the current culture is assumed.</param>
        /// <param name="value">The <see cref="T:System.Object"></see> to convert.</param>
        /// <param name="destinationType">The <see cref="T:System.Type"></see> to convert the value parameter to.</param>
        /// <returns>
        /// An <see cref="T:System.Object"></see> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        /// <exception cref="T:System.ArgumentNullException">The destinationType parameter is null. </exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }
            else if (value is ChartMargins)
            {
                ChartMargins margins = value as ChartMargins;

                if (destinationType == typeof(string))
                {
                    if (culture == null)
                    {
                        culture = CultureInfo.CurrentCulture;
                    }

                    string separator = culture.TextInfo.ListSeparator + " ";
                    TypeConverter intConverter = TypeDescriptor.GetConverter(typeof(int));
                    string[] sAttrs = new string[4]{
                                           intConverter.ConvertToString(context,culture,margins.Left),
                                           intConverter.ConvertToString(context,culture,margins.Top),
                                           intConverter.ConvertToString(context,culture,margins.Right),
                                           intConverter.ConvertToString(context,culture,margins.Bottom),};

                    return string.Join(separator, sAttrs);
                }
                else if (destinationType == typeof(InstanceDescriptor))
                {
                    ConstructorInfo cInfo = typeof(ChartMargins).GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) });

                    if (cInfo != null)
                    {
                        return new InstanceDescriptor(cInfo, new object[] { margins.Left, margins.Top, margins.Right, margins.Bottom });
                    }
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
        /// <summary>
        /// Creates an instance of the type that this <see cref="T:System.ComponentModel.TypeConverter"></see> is associated with, using the specified context, given a set of property values for the object.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="propertyValues">An <see cref="T:System.Collections.IDictionary"></see> of new property values.</param>
        /// <returns>
        /// An <see cref="T:System.Object"></see> representing the given <see cref="T:System.Collections.IDictionary"></see>, or null if the object cannot be created. This method always returns null.
        /// </returns>
        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            return new ChartMargins((int)propertyValues[PROP_LEFT], (int)propertyValues[PROP_TOP], (int)propertyValues[PROP_RIGHT], (int)propertyValues[PROP_BOTTOM]);
        }
        /// <summary>
        /// Returns whether changing a value on this object requires a call to <see cref="M:System.ComponentModel.TypeConverter.CreateInstance(System.Collections.IDictionary)"></see> to create a new value, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <returns>
        /// true if changing a property on this object requires a call to <see cref="M:System.ComponentModel.TypeConverter.CreateInstance(System.Collections.IDictionary)"></see> to create a new value; otherwise, false.
        /// </returns>
        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        /// <summary>
        /// Returns a collection of properties for the type of array specified by the value parameter, using the specified context and attributes.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="value">An <see cref="T:System.Object"></see> that specifies the type of array for which to get properties.</param>
        /// <param name="attributes">An array of type <see cref="T:System.Attribute"></see> that is used as a filter.</param>
        /// <returns>
        /// A <see cref="T:System.ComponentModel.PropertyDescriptorCollection"></see> with the properties that are exposed for this data type, or null if there are no properties.
        /// </returns>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(typeof(ChartMargins), attributes).Sort(new string[] { PROP_LEFT, PROP_TOP, PROP_RIGHT, PROP_BOTTOM });
        }
        /// <summary>
        /// Returns whether this object supports properties, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <returns>
        /// true if <see cref="M:System.ComponentModel.TypeConverter.GetProperties(System.Object)"></see> should be called to find the properties of this object; otherwise, false.
        /// </returns>
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        #endregion
    }

    /// <summary>
    /// Describes the margins of a frame around a rectangle. 
    /// Four float values describe the Left, Top, Right, and Bottom sides of the rectangle, respectively. 
    /// </summary>
    /// <internalonly/>
    [DocumentationExclude()]
    [TypeConverter(typeof(ChartMarginsConverter))]
    public class ChartMargins : ICloneable
    {
        #region Members
        private int top;
        private int left;
        private int bottom;
        private int right;
        #endregion

        #region Events
        /// <summary>
        /// Occurs when properties is changed.
        /// </summary>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public event EventHandler Changed;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the top value.
        /// </summary>
        /// <value></value>
        [DefaultValue(10), NotifyParentProperty(true)]
        [Description("Specifies the top value.")]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public int Top
        {
            get
            {
                return top;
            }
            set
            {

                if (top != value)
                {
                    top = value;
                    OnChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the left value.
        /// </summary>
        /// <value></value>
        [DefaultValue(10), NotifyParentProperty(true)]
        [Description("Specifies the left value.")]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public int Left
        {
            get
            {
                return left;
            }
            set
            {

                if (left != value)
                {
                    left = value;
                    OnChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the bottom value.
        /// </summary>
        /// <value></value>
        [DefaultValue(10), NotifyParentProperty(true)]
        [Description("Specifies the bottom value.")]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public int Bottom
        {
            get
            {
                return bottom;
            }
            set
            {

                if (bottom != value)
                {
                    bottom = value;
                    OnChanged();
                }
            }
        }
        /// <summary>
        /// Gets or sets the right value.
        /// </summary>
        /// <value></value>
        [DefaultValue(10), NotifyParentProperty(true)]
        [Description("Specifies the right value.")]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public int Right
        {
            get
            {
                return right;
            }
            set
            {

                if (right != value)
                {
                    right = value;
                    OnChanged();
                }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Overloaded constructor. Creates a new <see cref="ChartMargins"/> instance.
        /// </summary>
        public ChartMargins()
            : this(10, 10, 10, 10)
        {
        }
        /// <summary>
        /// Creates a new <see cref="ChartMargins"/> instance.
        /// </summary>
        /// <param name="top">Top.</param>
        /// <param name="left">Left.</param>
        /// <param name="bottom">Bottom.</param>
        /// <param name="right">Right.</param>
        public ChartMargins(int left, int top, int right, int bottom)
        {
            this.top = top;
            this.left = left;
            this.right = right;
            this.bottom = bottom;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            ChartMargins margins = obj as ChartMargins;

            if (margins != null)
            {
                return (margins.left == left) && (margins.top == top) && (margins.right == right) && (margins.bottom == bottom);
            }

            return base.Equals(obj);
        }
        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override int GetHashCode()
        {
            return left ^ top ^ right ^ bottom;
        }
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public ChartMargins Clone()
        {
            return new ChartMargins(left, top, right, bottom);
        }
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        object ICloneable.Clone()
        {
            return new ChartMargins(left, top, right, bottom);
        }
        /// <summary>
        /// Called when properties is changed.
        /// </summary>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual void OnChanged()
        {
            if (Changed != null)
            {
                Changed(this, EventArgs.Empty);
            }
        }
        #endregion
    }

    /// <summary>
    /// Converts instances of other types to and from a <see cref="ChartThickness"/>. 
    /// </summary>
    /// <internalonly/>
    [DocumentationExclude()]
    class ChartThicknessConverter : TypeConverter
    {
        #region Constants
        private const string PROP_LEFT = "Left";
        private const string PROP_TOP = "Top";
        private const string PROP_RIGHT = "Right";
        private const string PROP_BOTTOM = "Bottom";
        #endregion

        #region Implementation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return base.CanConvertTo(context, destinationType) || (destinationType == typeof(InstanceDescriptor));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return base.CanConvertFrom(context, sourceType) || sourceType == typeof(string);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                return ChartThickness.Parse(value as string);
            }

            return base.ConvertFrom(context, culture, value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }
            else if (value is ChartThickness)
            {
                ChartThickness margins = (ChartThickness)value;

                if (destinationType == typeof(string))
                {
                    return margins.ToString();
                }
                else if (destinationType == typeof(InstanceDescriptor))
                {
                    ConstructorInfo cInfo = typeof(ChartThickness).GetConstructor(new Type[] { typeof(float), typeof(float), typeof(float), typeof(float) });

                    if (cInfo != null)
                    {
                        return new InstanceDescriptor(cInfo, new object[] { margins.Left, margins.Top, margins.Right, margins.Bottom });
                    }
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="propertyValues"></param>
        /// <returns></returns>
        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            return new ChartThickness((float)propertyValues[PROP_LEFT], (float)propertyValues[PROP_TOP], (float)propertyValues[PROP_RIGHT], (float)propertyValues[PROP_BOTTOM]);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="value"></param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(typeof(ChartThickness), attributes).Sort(new string[] { PROP_LEFT, PROP_TOP, PROP_RIGHT, PROP_BOTTOM });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        #endregion
    }

    /// <summary>
    /// Describes the thickness of a frame around a rectangle. 
    /// Four float values describe the Left, Top, Right, and Bottom sides of the rectangle, respectively. 
    /// </summary>
    [TypeConverter(typeof(ChartThicknessConverter))]
    public struct ChartThickness
    {
        #region Members
        private float m_left;
        private float m_top;
        private float m_right;
        private float m_bottom;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the left.
        /// </summary>
        /// <value>The left.</value>
        [Description("Gets the left.")]
        public float Left
        {
            get { return m_left; }
            set { m_left = value; }
        }
        /// <summary>
        /// Gets the top.
        /// </summary>
        /// <value>The top.</value>
        [Description("Gets the top.")]
        public float Top
        {
            get { return m_top; }
            set { m_top = value; }
        }
        /// <summary>
        /// Gets the right.
        /// </summary>
        /// <value>The right.</value>
        [Description("Gets the right.")]
        public float Right
        {
            get { return m_right; }
            set { m_right = value; }
        }
        /// <summary>
        /// Gets the bottom.
        /// </summary>
        /// <value>The bottom.</value>
        [Description("Gets the bottom.")]
        public float Bottom
        {
            get { return m_bottom; }
            set { m_bottom = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartThickness"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public ChartThickness(float value)
        {
            m_top = value;
            m_left = value;
            m_right = value;
            m_bottom = value;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartThickness"/> class.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="right">The right.</param>
        /// <param name="bottom">The bottom.</param>
        public ChartThickness(float left, float top, float right, float bottom)
        {
            m_top = top;
            m_left = left;
            m_right = right;
            m_bottom = bottom;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(ChartThickness x, ChartThickness y)
        {
            return x.m_top == y.m_top && x.m_left == y.m_left
                && x.m_bottom == y.m_bottom && x.m_right == y.m_right;
        }
        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(ChartThickness x, ChartThickness y)
        {
            return x.m_top != y.m_top || x.m_left != y.m_left
                || x.m_bottom != y.m_bottom || x.m_right != y.m_right;
        }
        /// <summary>
        /// Adds the specified x to the specified y.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        public static ChartThickness Add(ChartThickness x, ChartThickness y)
        {
            return new ChartThickness(x.m_left + y.m_left, x.m_top + y.m_top,
                x.m_right + y.m_right, x.m_bottom + y.m_bottom);
        }

        /// <summary>
        /// Inflates the specified rect.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <returns></returns>
        public Rectangle Inflate(Rectangle rect)
        {
            rect.X -= (int)m_left;
            rect.Y -= (int)m_top;
            rect.Width += (int)(m_left + m_right);
            rect.Height += (int)(m_top + m_bottom);

            return rect;
        }
        /// <summary>
        /// Inflates the specified size.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public Size Inflate(Size size)
        {
            size.Width += (int)(m_left + m_right);
            size.Height += (int)(m_top + m_bottom);

            return size;
        }
        /// <summary>
        /// Inflates the specified rect.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <returns></returns>
        public RectangleF Inflate(RectangleF rect)
        {
            rect.X -= m_left;
            rect.Y -= m_top;
            rect.Width += m_left + m_right;
            rect.Height += m_top + m_bottom;

            return rect;
        }
        /// <summary>
        /// Inflates the specified size.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public SizeF Inflate(SizeF size)
        {
            size.Width += m_left + m_right;
            size.Height += m_top + m_bottom;

            return size;
        }
        /// <summary>
        /// Deflates the specified rect.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <returns></returns>
        public Rectangle Deflate(Rectangle rect)
        {
            rect.X += (int)m_left;
            rect.Y += (int)m_top;
            rect.Width -= (int)(m_left + m_right);
            rect.Height -= (int)(m_top + m_bottom);

            return rect;
        }
        /// <summary>
        /// Deflates the specified rect.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <returns></returns>
        public RectangleF Deflate(RectangleF rect)
        {
            rect.X += m_left;
            rect.Y += m_top;
            rect.Width -= m_left + m_right;
            rect.Height -= m_top + m_bottom;

            return rect;
        }
        /// <summary>
        /// Deflates the specified size.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public Size Deflate(Size size)
        {
            size.Width -= (int)(m_left + m_right);
            size.Height -= (int)(m_top + m_bottom);

            return size;
        }
        /// <summary>
        /// Deflates the specified size.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public SizeF Deflate(SizeF size)
        {
            size.Width -= m_left + m_right;
            size.Height -= m_top + m_bottom;

            return size;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>
        /// true if obj and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is ChartThickness)
            {
                ChartThickness thickness = (ChartThickness)obj;

                return m_top == thickness.m_top && m_left == thickness.m_left
                    && m_bottom == thickness.m_bottom && m_right == thickness.m_right;
            }

            return false;
        }
        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            return m_top.GetHashCode() ^ m_left.GetHashCode()
                ^ m_right.GetHashCode() ^ m_bottom.GetHashCode();
        }
        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> containing a fully qualified type name.
        /// </returns>
        public override string ToString()
        {
            return string.Join("; ", new string[]{ m_left.ToString(CultureInfo.InvariantCulture), 
				m_top.ToString(CultureInfo.InvariantCulture), 
				m_right.ToString(CultureInfo.InvariantCulture), 
				m_bottom.ToString(CultureInfo.InvariantCulture)});
        }
        /// <summary>
        /// Parses the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static ChartThickness Parse(string text)
        {
            string[] values = text.Split(';', ',');
            ChartThickness thickness = new ChartThickness();

            float.TryParse(values[0].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out thickness.m_left);
            float.TryParse(values[1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out thickness.m_top);
            float.TryParse(values[2].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out thickness.m_right);
            float.TryParse(values[3].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out thickness.m_bottom);

            return thickness;
        }
        #endregion
    }

    /// <summary>
    /// Specifies the usege of Y values.
    /// </summary>
    internal enum ChartYValueUsage
    {
        /// <summary>
        /// Point value will not used.
        /// </summary>
        None,
        /// <summary>
        /// This value used Y value for types like Line, Spline, Column...
        /// </summary>
        YValue,
        /// <summary>
        /// This value used as low value for types like HiLo, Range...
        /// </summary>
        LowValue,
        /// <summary>
        /// This value used as high value for types like HiLo, Range...
        /// </summary>
        HighValue,
        /// <summary>
        /// This value used as open value for types like HiLoOpenClose, Candle...
        /// </summary>
        OpenValue,
        /// <summary>
        /// This value used as close value for types like HiLoOpenClose, Candle...
        /// </summary>
        CloseValue,
        /// <summary>
        /// This value used as error bar value for types like Column, Line...
        /// </summary>
        ErrorBarValue,
        /// <summary>
        /// This value used as size value for types like Column, Bubble...
        /// </summary>
        PointSizeValue
    }

    /// <summary>
    /// Specifies the registry of 
    /// </summary>
    internal sealed class ChartPointFormatsRegistry
    {
        #region Internal types
        /// <summary>
        /// 
        /// </summary>
        class ChartPointFormat : Dictionary<ChartYValueUsage, int>
        {
            #region Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="ChartPointFormat"/> class.
            /// </summary>
            public ChartPointFormat()
            {
            }
            /// <summary>
            /// Initializes a new instance of the <see cref="ChartPointFormat"/> class.
            /// </summary>
            /// <param name="usages">The usages.</param>
            public ChartPointFormat(params ChartYValueUsage[] usages)
            {
                for (int i = 0; i < usages.Length; i++)
                {
                    if (usages[i] != ChartYValueUsage.None)
                    {
                        if (this.ContainsKey(usages[i]))
                        {
                            throw new ArgumentException(string.Format("{0} usage is already presented.", usages[i]));
                        }

                        this.Add(usages[i], i);
                    }
                }
            }
            #endregion
        }
        #endregion

        #region Members
        private static readonly ChartPointFormat c_default;
        private Dictionary<ChartSeriesType, ChartPointFormat> m_pointFormats = new Dictionary<ChartSeriesType, ChartPointFormat>();
        private ChartSeries m_series = null;

        private int m_cacheYIndex = -1;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the index of necessary value with the specified type.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        public int this[ChartSeriesType type, ChartYValueUsage usage]
        {
            get
            {
                if (m_pointFormats.ContainsKey(type))
                {
                    return m_pointFormats[type][usage];
                }

                return c_default[usage];
            }
        }
        /// <summary>
        /// Gets the index of necessary value.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        public int this[ChartYValueUsage usage]
        {
            get
            {
                return this[m_series.Type, usage];
            }
        }
        /// <summary>
        /// Gets the index of the Y value.
        /// </summary>
        /// <value>The index of the Y value.</value>
        internal int YIndex
        {
            get
            {
                return m_cacheYIndex;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes the <see cref="ChartPointFormatsRegistry"/> class.
        /// </summary>
        static ChartPointFormatsRegistry()
        {
            ChartPointFormat format = new ChartPointFormat();

            format[ChartYValueUsage.YValue] = 0;
            format[ChartYValueUsage.LowValue] = 0;
            format[ChartYValueUsage.HighValue] = 1;
            format[ChartYValueUsage.OpenValue] = 2;
            format[ChartYValueUsage.CloseValue] = 3;
            format[ChartYValueUsage.ErrorBarValue] = 1;
            format[ChartYValueUsage.PointSizeValue] = 1;

            c_default = format;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartPointFormatsRegistry"/> class.
        /// </summary>
        /// <param name="series">The series.</param>
        internal ChartPointFormatsRegistry(ChartSeries series)
        {
            m_series = series;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Registers the points format for the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="usages">The usages.</param>
        public void Register(ChartSeriesType type, params ChartYValueUsage[] usages)
        {
            m_pointFormats[type] = new ChartPointFormat(usages);
            m_series.OnAppearanceChanged(EventArgs.Empty);
        }
        /// <summary>
        /// Called when series type changed.
        /// </summary>
        /// <param name="type">The type.</param>
        internal void OnSeriesTypeChanged(ChartSeriesType type)
        {
            m_cacheYIndex = this[type, ChartYValueUsage.YValue];
        }
        #endregion
    }

    /// <summary>
    /// This class contains appearance information of interactive zooming.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class ChartZooming
    {
        #region Members
        private float m_opacity = 0.5f;
        private bool m_showBorder = false;
        private LineInfo m_border = new LineInfo();
        private BrushInfo m_interior = new BrushInfo(SystemColors.Highlight);
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the opacity of zooming selection.
        /// </summary>
        /// <value>The opacity.</value>
        [DefaultValue(0.5f)]
        [Description("Indicates the opacity of zooming selection.")]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public float Opacity
        {
            get
            {
                return m_opacity;
            }
            set
            {
                if (m_opacity != value)
                {
                    m_opacity = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether border is shown.
        /// </summary>
        /// <value><c>true</c> if border is shown; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]
        [Description("Indicates the border visibility of zooming selection.")]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public bool ShowBorder
        {
            get
            {
                return m_showBorder;
            }
            set
            {
                if (m_showBorder != value)
                {
                    m_showBorder = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets the information on line drawn during interactive zooming.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("Contains the border attributes of zooming selection.")]
        [ChartTemplate(ChartTemplateSet.Content)]
        public LineInfo Border
        {
            get
            {
                return m_border;
            }
            set
            {
                if (m_border != value)
                {
                    m_border = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets the interior of zooming selection.
        /// </summary>
        /// <value>The <see cref="BrushInfo"/> instance.</value>
        [Description("Indicates the interior of zooming selection.")]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public BrushInfo Interior
        {
            get
            {
                return m_interior;
            }
            set
            {
                if (m_interior != value)
                {
                    m_interior = value;
                }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartZooming"/> class.
        /// </summary>
        public ChartZooming()
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Should the serialize interior.
        /// </summary>
        /// <returns></returns>
        private bool ShouldSerializeInterior()
        {
            return (m_interior.Style != BrushStyle.Solid) || (m_interior.BackColor != SystemColors.Highlight);
        }
        #endregion
    }

    /// <summary>
    /// Describes the CMYK color.
    /// </summary>
    /// <internalonly/>
    public struct ChartCMYKColor
    {
        #region Constants
        private const int BYTE_MAX_VALUE = Byte.MaxValue;
        #endregion

        #region Members
        private byte m_c;
        private byte m_m;
        private byte m_y;
        private byte m_k;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the C component of color.
        /// </summary>
        /// <value>The C component.</value>
        public byte C
        {
            get
            {
                return m_c;
            }
        }
        /// <summary>
        /// Gets the M component of color.
        /// </summary>
        /// <value>The M component.</value>
        public byte M
        {
            get
            {
                return m_m;
            }
        }
        /// <summary>
        /// Gets the Y component of color.
        /// </summary>
        /// <value>The Y component.</value>
        public byte Y
        {
            get
            {
                return m_y;
            }
        }
        /// <summary>
        /// Gets the K component of color.
        /// </summary>
        /// <value>The K component.</value>
        public byte K
        {
            get
            {
                return m_k;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Converts the CMYK color of the RGB.
        /// </summary>
        /// <returns></returns>
        public Color ToRGBColor()
        {
            return ConvertCMYKToRGB(this);
        }
        /// <summary>
        /// Creates the CMYK color by the RBG components.
        /// </summary>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <returns></returns>
        public static ChartCMYKColor FromRBG(byte r, byte g, byte b)
        {
            int ir = byte.MaxValue - r;
            int ig = byte.MaxValue - g;
            int ib = byte.MaxValue - b;
            int min = Math.Min(ir, Math.Min(ig, ib));

            return FromCMYK((byte)(ir - min), (byte)(ig - min), (byte)(ib - min), (byte)min);
        }
        /// <summary>
        /// Creates the CMYK color by the CMYK components.
        /// </summary>
        /// <param name="c">The C component.</param>
        /// <param name="m">The M component.</param>
        /// <param name="y">The Y component.</param>
        /// <param name="k">The K component.</param>
        /// <returns></returns>
        public static ChartCMYKColor FromCMYK(byte c, byte m, byte y, byte k)
        {
            ChartCMYKColor cmykColor;

            cmykColor.m_c = c;
            cmykColor.m_m = m;
            cmykColor.m_y = y;
            cmykColor.m_k = k;

            return cmykColor;
        }
        /// <summary>
        /// Converts the RGB to CMYK color.
        /// </summary>
        /// <param name="rgbColor">Color of the RGB.</param>
        /// <returns></returns>
        public static ChartCMYKColor ConvertRGBToCMYK(Color rgbColor)
        {
            int ir = BYTE_MAX_VALUE - rgbColor.R;
            int ig = BYTE_MAX_VALUE - rgbColor.G;
            int ib = BYTE_MAX_VALUE - rgbColor.B;
            int min = Math.Min(ir, Math.Min(ig, ib));

            return FromCMYK((byte)(ir - min), (byte)(ig - min), (byte)(ib - min), (byte)min);
        }
        /// <summary>
        /// Converts the CMYK to RGB color.
        /// </summary>
        /// <param name="cmykColor">Color of the cmyk.</param>
        /// <returns></returns>
        public static Color ConvertCMYKToRGB(ChartCMYKColor cmykColor)
        {
            int ck = cmykColor.m_c + cmykColor.m_k;
            int mk = cmykColor.m_m + cmykColor.m_k;
            int yk = cmykColor.m_y + cmykColor.m_k;

            return Color.FromArgb(BYTE_MAX_VALUE - ck, BYTE_MAX_VALUE - mk, BYTE_MAX_VALUE - yk);
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    internal class RenderingHelper
    {
        #region Implementation
        /// <summary>
        /// Draws the point symbol.
        /// </summary>
        /// <param name="graph">The graph.</param>
        /// <param name="style">The style.</param>
        /// <param name="point">The point.</param>
        public static void DrawPointSymbol(ChartGraph graph, ChartStyleInfo style, PointF point)
        {
            Brush brush = new SolidBrush(style.Symbol.Color);
            Pen pen = style.Symbol.Border.GdipPen;

            RenderingHelper.DrawPointSymbol(graph, style.Symbol.Shape, brush, pen,
                style.Symbol.ImageIndex, style.Images, point, style.Symbol.Size);

            brush.Dispose();
        }
        /// <summary>
        /// Draws the point symbol.
        /// </summary>
        /// <param name="graph">The graph.</param>
        /// <param name="shape">The shape.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="imgIndex">Index of the img.</param>
        /// <param name="images">The images.</param>
        /// <param name="point">The point.</param>
        /// <param name="size">The size.</param>
        public static void DrawPointSymbol(ChartGraph graph, ChartSymbolShape shape,
            Brush brush, Pen pen, int imgIndex, ChartImageCollection images, PointF point, SizeF size)
        {
            if (shape != ChartSymbolShape.None)
            {
                RectangleF bounds = ChartMath.GetRectByCenter(point, size);

                #region Draw symbol
                switch (shape)
                {
                    case ChartSymbolShape.Arrow:
                        graph.DrawPath(brush, pen, ChartSymbolHelper.GetPathArrow(bounds));
                        break;

                    case ChartSymbolShape.InvertedArrow:
                        graph.DrawPath(brush, pen, ChartSymbolHelper.GetPathInvertedArrow(bounds));
                        break;

                    case ChartSymbolShape.Circle:
                        graph.DrawPath(brush, pen, ChartSymbolHelper.GetPathCircle(bounds));
                        break;

                    case ChartSymbolShape.Cross:
                        using (Pen linePen = new Pen(brush))
                        {
                            graph.DrawLine(pen, bounds.Left, point.Y, bounds.Right, point.Y);
                            graph.DrawLine(pen, point.X, bounds.Top, point.X, bounds.Bottom);
                        }
                        break;

                    case ChartSymbolShape.HorizLine:
                        using (Pen linePen = new Pen(brush))
                        {
                            graph.DrawLine(pen, bounds.Left, point.Y, bounds.Right, point.Y);
                        }
                        break;

                    case ChartSymbolShape.VertLine:
                        using (Pen linePen = new Pen(brush))
                        {
                            graph.DrawLine(pen, point.X, bounds.Top, point.X, bounds.Bottom);
                        }
                        break;

                    case ChartSymbolShape.Diamond:
                        graph.DrawPath(brush, pen, ChartSymbolHelper.GetPathDiamond(bounds));
                        break;

                    case ChartSymbolShape.Hexagon:
                        graph.DrawPath(brush, pen, ChartSymbolHelper.GetPathHexagon(bounds));
                        break;

                    case ChartSymbolShape.InvertedTriangle:
                        graph.DrawPath(brush, pen, ChartSymbolHelper.GetPathInvertedTriangle(bounds));
                        break;

                    case ChartSymbolShape.Pentagon:
                        graph.DrawPath(brush, pen, ChartSymbolHelper.GetPathPentagon(bounds));
                        break;

                    case ChartSymbolShape.Star:
                        graph.DrawPath(brush, pen, ChartSymbolHelper.GetPathStar(bounds));
                        break;

                    case ChartSymbolShape.Square:
                        graph.DrawPath(brush, pen, ChartSymbolHelper.GetPathSquare(bounds));
                        break;

                    case ChartSymbolShape.Triangle:
                        graph.DrawPath(brush, pen, ChartSymbolHelper.GetPathTriangle(bounds));
                        break;

                    case ChartSymbolShape.Image:
                        if (images != null && imgIndex >= 0 && imgIndex < images.Count)
                        {
                            graph.DrawImage(images[imgIndex],
                                bounds.X, bounds.Y, bounds.Width, bounds.Height);
                        }
                        break;
                }
                #endregion
            }
        }
        /// <summary>
        /// Draws the marker.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/>.</param>
        /// <param name="marker">The marker.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="p1">The p1.</param>
        public static void DrawMarker(Graphics g, ChartMarker marker, PointF p2, PointF p1)
        {
            Pen pen = (Pen)marker.LineInfo.GdipPen.Clone();
            pen.EndCap = marker.LineCap;
            g.DrawLine(pen, p1.X, p1.Y, p2.X, p2.Y);
        }
        /// <summary>
        /// Draws the related point symbol.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/>.</param>
        /// <param name="symbol">The symbol.</param>
        /// <param name="border">The border.</param>
        /// <param name="imgList">The img list.</param>
        /// <param name="pt">The pt.</param>
        public static void DrawRelatedPointSymbol(Graphics g, ChartRelatedPointSymbolInfo symbol, ChartRelatedPointLineInfo border, ChartImageCollection imgList, PointF pt)
        {

            if (symbol.Shape != ChartSymbolShape.None)
            {
                pt.X -= symbol.Size.Width / 2;
                pt.Y -= symbol.Size.Height / 2;
                RectangleF bounds = new RectangleF(pt, symbol.Size);
                Brush brush = new SolidBrush(symbol.Color);

                Pen pen = border.GdipPen;

                switch (symbol.Shape)
                {
                    case ChartSymbolShape.Arrow:
                        {
                            DrawArrow(g, bounds, brush, pen);
                            break;
                        }

                    case ChartSymbolShape.InvertedArrow:
                        {
                            DrawInvertedArrow(g, bounds, brush, pen);
                            break;
                        }

                    case ChartSymbolShape.Circle:
                        {
                            DrawCircle(g, bounds, brush, pen);
                            break;
                        }

                    case ChartSymbolShape.Cross:
                        {
                            DrawCross(g, bounds, brush);
                            break;
                        }

                    case ChartSymbolShape.Diamond:
                        {
                            DrawDiamond(g, bounds, brush, pen);
                            break;
                        }

                    case ChartSymbolShape.Hexagon:
                        {
                            DrawHexagon(g, bounds, brush, pen);
                            break;
                        }

                    case ChartSymbolShape.InvertedTriangle:
                        {
                            DrawInvertedTriangle(g, bounds, brush, pen);
                            break;
                        }

                    case ChartSymbolShape.Pentagon:
                        {
                            DrawPentagon(g, bounds, brush, pen);
                            break;
                        }

                    case ChartSymbolShape.Star:
                        {
                            DrawStar(g, bounds, brush, pen);
                            break;
                        }

                    case ChartSymbolShape.Square:
                        {
                            DrawSquare(g, bounds, brush, pen);
                            break;
                        }

                    case ChartSymbolShape.Triangle:
                        {
                            DrawTriangle(g, bounds, brush, pen);
                            break;
                        }

                    case ChartSymbolShape.Image:
                        {
                            int imageIndex = symbol.ImageIndex;

                            if (imageIndex >= 0 && imageIndex < imgList.Count)
                            {
                                DrawImage(g, bounds, imgList[imageIndex]);
                            }
                            break;
                        }
                }

                pen.Dispose();
                brush.Dispose();
            }
        }
        /// <summary>
        /// Draws the point symbol.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/>.</param>
        /// <param name="style">The style.</param>
        /// <param name="pt">The pt.</param>
        /// <param name="drawMarker">if set to <c>true</c> marker will be drawn.</param>
        public static void DrawPointSymbol(Graphics g, ChartStyleInfo style, PointF pt, bool drawMarker)
        {
            ChartSymbolInfo symbol = style.Symbol;
            Brush brush = new SolidBrush(symbol.Color);
            Pen pen = style.Symbol.Border.GdipPen;
            DrawPointSymbol(g, symbol.Shape, symbol.Marker, symbol.Size, symbol.Offset, symbol.ImageIndex, brush, pen, style.Images, pt, drawMarker);
            brush.Dispose();
        }

        /// <summary>
        /// Draws the point symbol.
        /// </summary>
        /// <param name="g">The graphics object.</param>
        /// <param name="style">The style.</param>
        /// <param name="pt">The point.</param>
        /// <param name="drawMarker">if set to <c>true</c> [draw marker].</param>
        /// <param name="brushInfo">The brush info.</param>
        public static void DrawPointSymbol(Graphics g, ChartStyleInfo style, PointF pt, bool drawMarker, BrushInfo brushInfo)
        {
            ChartSymbolInfo symbol = style.Symbol;
            Brush brush = new SolidBrush(symbol.Color);            
            RectangleF bounds = new RectangleF(pt, symbol.Size);
            brush = ChartGraph.GetBrushItem(brushInfo, bounds);

            Pen pen = style.Symbol.Border.GdipPen;
            DrawPointSymbol(g, symbol.Shape, symbol.Marker, symbol.Size, symbol.Offset, symbol.ImageIndex, brush, pen, style.Images, pt, drawMarker);
            brush.Dispose();
        }

        /// <summary>
        /// Draws the point symbol.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/>.</param>
        /// <param name="symbolShape">The symbol shape.</param>
        /// <param name="symbolMarker">The symbol marker.</param>
        /// <param name="symbolSize">Size of the symbol.</param>
        /// <param name="symbolOffset">The symbol offset.</param>
        /// <param name="symbolImageIndex">Index of the symbol image.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="images">The images.</param>
        /// <param name="pt">The pt.</param>
        /// <param name="drawMarker">if set to <c>true</c> marker will be drawn.</param>
        public static void DrawPointSymbol(Graphics g, ChartSymbolShape symbolShape, ChartMarker symbolMarker, Size symbolSize, Size symbolOffset, int symbolImageIndex, Brush brush, Pen pen, ChartImageCollection images, PointF pt, bool drawMarker)
        {
            if (symbolShape != ChartSymbolShape.None)
            {
                if (drawMarker)
                {
                    PointF ptEnd = new PointF(pt.X - symbolOffset.Width + symbolSize.Width / 2, pt.Y - symbolOffset.Height + symbolSize.Height / 2);
                    DrawMarker(g, symbolMarker, ptEnd, pt);
                }
                pt.X -= symbolSize.Width / 2;
                pt.Y -= symbolSize.Height / 2;
                RectangleF bounds = new RectangleF(pt, symbolSize);

                switch (symbolShape)
                {
                    case ChartSymbolShape.Arrow:
                        {
                            DrawArrow(g, bounds, brush, pen);
                            break;
                        }
                    case ChartSymbolShape.InvertedArrow:
                        {
                            DrawInvertedArrow(g, bounds, brush, pen);
                            break;
                        }
                    case ChartSymbolShape.Circle:
                        {
                            DrawCircle(g, bounds, brush, pen);
                            break;
                        }

                    case ChartSymbolShape.Cross:
                        {
                            DrawCross(g, bounds, brush);
                            break;
                        }

                    case ChartSymbolShape.HorizLine:
                        {
                            DrawHorizLine(g, bounds, brush);
                            break;
                        }

                    case ChartSymbolShape.VertLine:
                        {
                            DrawVertLine(g, bounds, brush);
                            break;
                        }

                    case ChartSymbolShape.Diamond:
                        {
                            DrawDiamond(g, bounds, brush, pen);
                            break;
                        }

                    case ChartSymbolShape.Hexagon:
                        {
                            DrawHexagon(g, bounds, brush, pen);
                            break;
                        }

                    case ChartSymbolShape.InvertedTriangle:
                        {
                            DrawInvertedTriangle(g, bounds, brush, pen);

                            break;
                        }

                    case ChartSymbolShape.Pentagon:
                        {
                            DrawPentagon(g, bounds, brush, pen);
                            break;
                        }

                    case ChartSymbolShape.Star:
                        {
                            DrawStar(g, bounds, brush, pen);
                            break;
                        }
                    case ChartSymbolShape.Square:
                        {
                            DrawSquare(g, bounds, brush, pen);
                            break;
                        }

                    case ChartSymbolShape.Triangle:
                        {
                            DrawTriangle(g, bounds, brush, pen);
                            break;
                        }

                    case ChartSymbolShape.Image:
                        {
                            if (images != null)
                            {
                                if (symbolImageIndex >= 0 && symbolImageIndex < images.Count)
                                {
                                    DrawImage(g, bounds, (Image)images[symbolImageIndex]);
                                }
                            }
                            break;
                        }
                }
            }
        }
        /// <summary>
        /// Draws the point symbol.
        /// </summary>
        /// <param name="graph">The graphics.</param>
        /// <param name="symbolShape">The symbol shape.</param>
        /// <param name="symbolMarker">The symbol marker.</param>
        /// <param name="symbolSize">Size of the symbol.</param>
        /// <param name="symbolOffset">The symbol offset.</param>
        /// <param name="imgIndex">Index of the img.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="images">The images.</param>
        /// <param name="point">The point.</param>
        /// <param name="drawMarker">if set to <c>true</c> marker will be drawn.</param>
        public static void DrawPointSymbol(ChartGraph graph, ChartSymbolShape symbolShape, ChartMarker symbolMarker, Size symbolSize, Size symbolOffset, int imgIndex, Brush brush, Pen pen, ChartImageCollection images, PointF point, bool drawMarker)
        {
            if (symbolShape != ChartSymbolShape.None)
            {
                if (drawMarker)
                {
                    PointF ptEnd = new PointF(point.X - symbolOffset.Width + symbolSize.Width / 2,
                        point.Y - symbolOffset.Height + symbolSize.Height / 2);

                    using (Pen mPen = symbolMarker.LineInfo.GdipPen.Clone() as Pen)
                    {
                        mPen.EndCap = symbolMarker.LineCap;
                        graph.DrawLine(pen, ptEnd.X, ptEnd.Y, point.X, point.Y);
                    }
                }

                point.X -= symbolSize.Width / 2;
                point.Y -= symbolSize.Height / 2;
                RectangleF bounds = new RectangleF(point, symbolSize);

                #region Draw symbol
                switch (symbolShape)
                {
                    case ChartSymbolShape.Arrow:
                        graph.DrawPath(brush, pen, ChartSymbolHelper.GetPathArrow(bounds));
                        break;
                    case ChartSymbolShape.InvertedArrow:
                        graph.DrawPath(brush, pen, ChartSymbolHelper.GetPathInvertedArrow(bounds));
                        break;

                    case ChartSymbolShape.Circle:
                        graph.DrawPath(brush, pen, ChartSymbolHelper.GetPathCircle(bounds));
                        break;

                    case ChartSymbolShape.Cross:
                        using (Pen linePen = new Pen(brush))
                        {
                            //graph.DrawLine(pen, bounds.Left, point.Y, bounds.Right, point.Y);
                            //graph.DrawLine(pen, point.X, bounds.Top, point.X, bounds.Bottom);

                            graph.DrawLine(pen, bounds.X, bounds.Y + bounds.Height / 2, bounds.Right, bounds.Y + bounds.Height / 2);
                            graph.DrawLine(pen, bounds.X + bounds.Width / 2, bounds.Y, bounds.X + bounds.Width / 2, bounds.Bottom);
                        }
                        break;

                    case ChartSymbolShape.HorizLine:
                        using (Pen linePen = new Pen(brush))
                        {
                            graph.DrawLine(pen, bounds.Left, point.Y, bounds.Right, point.Y);
                        }
                        break;

                    case ChartSymbolShape.VertLine:
                        using (Pen linePen = new Pen(brush))
                        {
                            graph.DrawLine(pen, point.X, bounds.Top, point.X, bounds.Bottom);
                        }
                        break;

                    case ChartSymbolShape.Diamond:
                        graph.DrawPath(brush, pen, ChartSymbolHelper.GetPathDiamond(bounds));
                        break;

                    case ChartSymbolShape.Hexagon:
                        graph.DrawPath(brush, pen, ChartSymbolHelper.GetPathHexagon(bounds));
                        break;

                    case ChartSymbolShape.InvertedTriangle:
                        graph.DrawPath(brush, pen, ChartSymbolHelper.GetPathInvertedTriangle(bounds));
                        break;

                    case ChartSymbolShape.Pentagon:
                        graph.DrawPath(brush, pen, ChartSymbolHelper.GetPathPentagon(bounds));
                        break;

                    case ChartSymbolShape.Star:
                        graph.DrawPath(brush, pen, ChartSymbolHelper.GetPathStar(bounds));
                        break;

                    case ChartSymbolShape.Square:
                        graph.DrawPath(brush, pen, ChartSymbolHelper.GetPathSquare(bounds));
                        break;

                    case ChartSymbolShape.Triangle:
                        graph.DrawPath(brush, pen, ChartSymbolHelper.GetPathTriangle(bounds));
                        break;

                    case ChartSymbolShape.Image:
                        if (images != null && imgIndex >= 0 && imgIndex < images.Count)
                        {
                            graph.DrawImage(images[imgIndex],
                                bounds.X, bounds.Y, bounds.Width, bounds.Height);
                        }
                        break;
                }
                #endregion
            }
        }
        
        /// <summary>
        /// Draw Image symbol.
        /// </summary>
        /// <param name="g">The grapics.</param>
        /// <param name="bounds">Rectangle bound of the Symbol.</param>        
        /// <param name="image">The image.</param>
        public static void DrawImage(Graphics g, RectangleF bounds, Image image)
        {
            g.DrawImage(image, bounds);
        }

        /// <summary>
        /// Draw Circle symbol.
        /// </summary>
        /// <param name="g">The grapics.</param>
        /// <param name="bounds">Rectangle bound of the Symbol.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="pen">The pen.</param>
        public static void DrawCircle(Graphics g, RectangleF bounds, Brush brush, Pen pen)
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddEllipse(bounds);
            g.FillPath(brush, gp);            
            g.DrawPath(pen, gp);
        }
        /// <summary>
        /// Draw Cross Line symbol.
        /// </summary>
        /// <param name="g">The grapics.</param>
        /// <param name="bounds">Rectangle bound of the Symbol.</param>
        /// <param name="brush">The brush.</param>        
        public static void DrawCross(Graphics g, RectangleF bounds, Brush brush)
        {
            Pen pen = new Pen(brush);
            g.DrawLine(pen, bounds.X, bounds.Y + bounds.Height / 2, bounds.Right, bounds.Y + bounds.Height / 2);
            g.DrawLine(pen, bounds.X + bounds.Width / 2, bounds.Y, bounds.X + bounds.Width / 2, bounds.Bottom);
            pen.Dispose();
        }
        /// <summary>
        /// Draw Horizontal Line symbol.
        /// </summary>
        /// <param name="g">The grapics.</param>
        /// <param name="bounds">Rectangle bound of the Symbol.</param>
        /// <param name="brush">The brush.</param>        
        public static void DrawHorizLine(Graphics g, RectangleF bounds, Brush brush)
        {
            Pen pen = new Pen(brush);
            g.DrawLine(pen, bounds.X, bounds.Y + bounds.Height / 2, bounds.Right, bounds.Y + bounds.Height / 2);
            pen.Dispose();
        }
        /// <summary>
        /// Draw Vertical Line symbol.
        /// </summary>
        /// <param name="g">The grapics.</param>
        /// <param name="bounds">Rectangle bound of the Symbol.</param>
        /// <param name="brush">The brush.</param>        
        public static void DrawVertLine(Graphics g, RectangleF bounds, Brush brush)
        {
            Pen pen = new Pen(brush);
            g.DrawLine(pen, bounds.X + bounds.Width / 2, bounds.Y, bounds.X + bounds.Width / 2, bounds.Bottom);
            pen.Dispose();
        }
        /// <summary>
        /// Draw Diamond symbol.
        /// </summary>
        /// <param name="g">The grapics.</param>
        /// <param name="bounds">Rectangle bound of the Symbol.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="pen">The pen.</param>
        public static void DrawDiamond(Graphics g, RectangleF bounds, Brush brush, Pen pen)
        {
            GraphicsPath gp = new GraphicsPath();
            PointF[] points = new PointF[4]
    {
      new PointF( bounds.X + bounds.Width / 2, bounds.Y ),
      new PointF( bounds.Right, bounds.Y + bounds.Height / 2 ),
      new PointF( bounds.X + bounds.Width / 2, bounds.Bottom ),
      new PointF( bounds.X, bounds.Y + bounds.Height / 2 )
    };

            gp.AddLines(points);
            gp.CloseAllFigures();
            g.FillPath(brush, gp);
            g.DrawPath(pen, gp);
        }
        /// <summary>
        /// Draw Hexagon symbol.
        /// </summary>
        /// <param name="g">The grapics.</param>
        /// <param name="bounds">Rectangle bound of the Symbol.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="pen">The pen.</param>
        public static void DrawHexagon(Graphics g, RectangleF bounds, Brush brush, Pen pen)
        {
            GraphicsPath gp = new GraphicsPath();
            PointF[] points = new PointF[6]
    {
      new PointF( bounds.X + bounds.Width / 4, bounds.Y ),
      new PointF( bounds.X + bounds.Width * ( 3f / 4f ), bounds.Y ),
      new PointF( bounds.Right, bounds.Y + bounds.Height / 2 ),
      new PointF( bounds.X + bounds.Width * ( 3f / 4f ), bounds.Bottom ),
      new PointF( bounds.X + bounds.Width / 4, bounds.Bottom ),
      new PointF( bounds.X, bounds.Y + bounds.Height / 2 )
    };

            gp.AddLines(points);
            gp.CloseAllFigures();
            g.FillPath(brush, gp);
            g.DrawPath(pen, gp);
        }
        
        /// <summary>
        /// Draw inverted triangle symbol.
        /// </summary>
        /// <param name="g">The grapics.</param>
        /// <param name="bounds">Rectangle bound of the Symbol.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="pen">The pen.</param>
        public static void DrawInvertedTriangle(Graphics g, RectangleF bounds, Brush brush, Pen pen)
        {
            GraphicsPath gp = new GraphicsPath();
            PointF[] points = new PointF[3]
            {
              new PointF( bounds.X, bounds.Y ),
              new PointF( bounds.Right, bounds.Y ),
              new PointF( bounds.X + bounds.Width / 2, bounds.Bottom )   

            };
         

            gp.AddLines(points);
            gp.CloseAllFigures();
            g.FillPath(brush, gp);
            g.DrawPath(pen, gp);
        }

        /// <summary>
        /// Draw Arrow symbol.
        /// </summary>
        /// <param name="g">The grapics.</param>
        /// <param name="bounds">Rectangle bound of the Symbol.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="pen">The pen.</param>
        public static void DrawArrow(Graphics g, RectangleF bounds, Brush brush, Pen pen)
        {
            GraphicsPath gp = new GraphicsPath();
            float bodyheight = (bounds.Height / 3);
            float bodywidth = (bounds.Width / 3);

            PointF[] points = new PointF[4]
             { 
              new PointF(bounds.Left, bounds.Top),
              new PointF( bounds.Right,bounds.Top+(bounds.Height/2)),
              new PointF(bounds.Left, bounds.Bottom),
              new PointF( bounds.Right- bodywidth*2, bounds.Bottom-(bounds.Height/2))             
             };             

            gp.AddLines(points);
            gp.CloseAllFigures();
            g.FillPath(brush, gp);
            g.DrawPath(pen, gp);
        }

        /// <summary>
        /// Draw Inverted Arrow symbol.
        /// </summary>
        /// <param name="g">The grapics.</param>
        /// <param name="bounds">Rectangle bound of the Symbol.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="pen">The pen.</param>
        public static void DrawInvertedArrow(Graphics g, RectangleF bounds, Brush brush, Pen pen)
        {
            GraphicsPath gp = new GraphicsPath();
            float bodyheight = (bounds.Height / 3);
            float bodywidth = (bounds.Width / 3);

            PointF[] points = new PointF[4]
             { 
              new PointF(bounds.Right, bounds.Top),
              new PointF( bounds.Left,bounds.Top+(bounds.Width/2)),
              new PointF(bounds.Right, bounds.Bottom),
              new PointF( bounds.Left + bodyheight*2, bounds.Bottom-(bounds.Width/2))             
             }; 

            gp.AddLines(points);
            gp.CloseAllFigures();
            g.FillPath(brush, gp);
            g.DrawPath(pen, gp);
        }


        /// <summary>
        /// Draw Pentagon symbol.
        /// </summary>
        /// <param name="g">The grapics.</param>
        /// <param name="bounds">Rectangle bound of the Symbol.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="pen">The pen.</param>
        public static void DrawPentagon(Graphics g, RectangleF bounds, Brush brush, Pen pen)
        {
            GraphicsPath gp = new GraphicsPath();
            PointF[] points = new PointF[5]
                {
                  new PointF( bounds.X + ( float )bounds.Width / 5f, bounds.Y ),
                  new PointF( bounds.X + bounds.Width - ( float )bounds.Width / 5f, bounds.Y ),
                  new PointF( bounds.Right, bounds.Y + ( float )bounds.Height * ( 3f / 5f ) ),
                  new PointF( bounds.X + ( float )bounds.Width / 2f, bounds.Bottom ),
                  new PointF( bounds.X, bounds.Y + ( float )bounds.Height * ( 3f / 5f ) )
                };

            gp.AddLines(points);
            gp.CloseAllFigures();
            g.FillPath(brush, gp);
            g.DrawPath(pen, gp);
        }
        /// <summary>
        /// Draw Star symbol.
        /// </summary>
        /// <param name="g">The grapics.</param>
        /// <param name="bounds">Rectangle bound of the Symbol.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="pen">The pen.</param>
        public static void DrawStar(Graphics g, RectangleF bounds, Brush brush, Pen pen)
        {
            GraphicsPath gp = new GraphicsPath();
            PointF[] points = new PointF[]
    {
      new PointF( bounds.X + ( float )bounds.Width / 2f, bounds.Top ),
      new PointF( bounds.X + ( float )bounds.Width * 11/18f, bounds.Y + ( float )bounds.Height * ( 1f / 3f ) ),
      new PointF( bounds.Right, bounds.Y + ( float )bounds.Height * ( 1f / 3f ) ),
      new PointF( bounds.X + ( float )bounds.Width * 2/3f, bounds.Y + ( float )bounds.Height * ( 3f / 5f ) ),
      new PointF( bounds.X + ( float )bounds.Width * 5/6f, bounds.Bottom ),
      new PointF( bounds.X + ( float )bounds.Width / 2f, bounds.Y + ( float )bounds.Height * ( 22f / 30f ) ),
      new PointF( bounds.X + ( float )bounds.Width / 6f, bounds.Bottom ),
      new PointF( bounds.X + ( float )bounds.Width * 1/3f, bounds.Y + ( float )bounds.Height * ( 3f / 5f ) ),
      new PointF( bounds.X, bounds.Y + ( float )bounds.Height * ( 1f / 3f ) ),
      new PointF( bounds.X + ( float )bounds.Width * 7/18f, bounds.Y + ( float )bounds.Height * ( 1f / 3f ) )

    };

            gp.AddLines(points);
            gp.CloseAllFigures();
            g.FillPath(brush, gp);
            g.DrawPath(pen, gp);
        }
        /// <summary>
        /// Draw Square symbol.
        /// </summary>
        /// <param name="g">The grapics.</param>
        /// <param name="bounds">Rectangle bound of the Symbol.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="pen">The pen.</param>
        public static void DrawSquare(Graphics g, RectangleF bounds, Brush brush, Pen pen)
        {
            g.FillRectangle(brush, bounds);
            g.DrawRectangle(pen, Rectangle.Round(bounds));
        }
        /// <summary>
        /// Draw Triangle Arrow symbol.
        /// </summary>
        /// <param name="g">The grapics.</param>
        /// <param name="bounds">Rectangle bound of the Symbol.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="pen">The pen.</param>
        public static void DrawTriangle(Graphics g, RectangleF bounds, Brush brush, Pen pen)
        {
            GraphicsPath gp = new GraphicsPath();
            PointF[] points = new PointF[3]{
                                       new PointF( bounds.X + bounds.Width / 2, bounds.Y ),
                                       new PointF( bounds.X, bounds.Bottom ),
                                       new PointF( bounds.Right, bounds.Bottom )
                                     };

            gp.AddLines(points);
            gp.CloseAllFigures();
            g.FillPath(brush, gp);
            g.DrawPath(pen, gp);
        }
        /// <summary>
        /// Draw text.
        /// </summary>
        /// <param name="g">The graphics.</param>
        /// <param name="style">Style of the text.</param>
        /// <param name="pt">The point.</param>
        public static void DrawText(Graphics g, ChartStyleInfo style, PointF pt)
        {
            Brush brush = new SolidBrush(style.TextColor);

            if (style.Font.Orientation == 0)
            {
                g.DrawString(style.Text, style.Font.GdipFont, brush, pt);
            }
            else
            {
                Size sz = g.MeasureString(style.Text, style.GdipFont).ToSize();
                GraphicsContainer cont = DrawingHelper.BeginTransform(g);
                g.TranslateTransform(pt.X, pt.Y);
                g.RotateTransform(style.Font.Orientation);
                g.DrawString(style.Text, style.Font.GdipFont, brush, style.TextOffset, -style.GdipFont.Height / 2f);
                DrawingHelper.EndTransform(g, cont);
            }
            brush.Dispose();
        }
        /// <summary>
        /// Draw text.
        /// </summary>
        /// <param name="g">The graphics.</param>
        /// <param name="style">Style of the text.</param>
        /// <param name="pt">The point.</param>
        /// <param name="sz">The size.</param>
        public static void DrawText(Graphics g, ChartStyleInfo style, PointF pt, SizeF sz)
        {
            Brush brush = new SolidBrush(style.TextColor);            
            StringFormat stringFormat = style.Format;
            if (style.Font.Orientation == 0)
            {
                if (stringFormat != null)
                {
                    g.DrawString(style.Text, style.GdipFont, brush, Rectangle.Round(new RectangleF(pt, sz)), stringFormat);
                }
                else
                {
                    g.DrawString(style.Text, style.GdipFont, brush, Rectangle.Round(new RectangleF(pt, sz)), DrawingHelper.CenteredFormat);
                }
                
                
            }
            else
            {                
                GraphicsContainer cont = DrawingHelper.BeginTransform(g);
                g.TranslateTransform(pt.X, pt.Y + sz.Height / 2);
                g.RotateTransform(style.Font.Orientation);
                if (stringFormat != null)
                {
                    g.DrawString(style.Text, style.GdipFont, brush, new RectangleF(-style.TextOffset, -sz.Height / 2f, sz.Width, sz.Height), stringFormat);
                }
                else
                {
                    g.DrawString(style.Text, style.GdipFont, brush, new RectangleF(-style.TextOffset, -sz.Height / 2f, sz.Width, sz.Height), DrawingHelper.CenteredFormat);
                }               
                
                DrawingHelper.EndTransform(g, cont);
            }
            brush.Dispose();
        }

        /// <summary>
        /// Adds the text path.
        /// </summary>
        /// <param name="gp">The gp.</param>
        /// <param name="g">The g.</param>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="rect">The rect.</param>
        /// <param name="strFormat">The STR format.</param>
        public static void AddTextPath(GraphicsPath gp, Graphics g, string text,
            Font font, RectangleF rect, StringFormat strFormat)
        {
            gp.AddString(text, font.FontFamily, (int)font.Style, font.GetHeight(g), rect, strFormat);
        }
        /// <summary>
        /// Adds the text gepmetry to <see cref="GraphicsPath"/>.
        /// </summary>
        /// <param name="gp">The gp.</param>
        /// <param name="g">The g.</param>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="rect">The rect.</param>
        public static void AddTextPath(GraphicsPath gp, Graphics g, string text,
            Font font, RectangleF rect)
        {
            gp.AddString(text, font.FontFamily, (int)font.Style, GetFontSizeInPixels(font, g), rect, null);
        }
        /// <summary>
        /// Gets the font size in pixels.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="g">The <see cref="Graphics"/>.</param>
        /// <returns></returns>
        public static float GetFontSizeInPixels(Font font, Graphics g)
        {
            if (font.Unit != GraphicsUnit.Pixel)
            {
                return font.GetHeight(g) * font.FontFamily.GetEmHeight(font.Style) / font.FontFamily.GetLineSpacing(font.Style);
            }

            return font.Size;
        }
        /// <summary>
        /// Gets the font size in pixels.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <returns></returns>
        public static float GetFontSizeInPixels(Font font)
        {
            if (font.Unit == GraphicsUnit.Point)
            {
                Font testFont = new Font(font.FontFamily, 10f, font.Style, GraphicsUnit.Pixel);
                float pointSize = testFont.SizeInPoints;
                return 10f * font.Size / pointSize;
            }
            else
            {
                return font.Size;
            }
        }
        /// <summary>
        /// Gets the bounds.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        /// <param name="bounds">The bounds.</param>
        /// <returns></returns>
        public static RectangleF GetBounds(int index, int count, RectangleF bounds)
        {
            double cc = Math.Sqrt(count);
            cc = (int)((cc % 1) != 0 ? cc + 1 : cc);

            double rc = count / cc;
            rc = (int)((rc % 1) != 0 ? rc + 1 : rc);

            int i = (int)(index % cc);
            int j = (int)(index / cc);

            SizeF sz = new SizeF((float)(bounds.Width / cc), (float)(bounds.Height / rc));
            return new RectangleF(bounds.X + i * sz.Width, bounds.Y + j * sz.Height,
                sz.Width, sz.Height);
        }

        /// <summary>
        /// Gets the individual pie bounds.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        /// <param name="bounds">The bounds.</param>
        /// <returns></returns>
        public static RectangleF GetPieBounds(int index, int count, RectangleF bounds)
        {
            index += 1;
            float width = bounds.Width / 2;
            float widthPerPie = width / count;            
            float widthForSpecificPie = widthPerPie * (count - index);

            float height = bounds.Height / 2;
            float heightPerPie = height / count;            
            float heightForSpecificPie = heightPerPie * (count - index);

            RectangleF rect = RectangleF.Inflate(bounds, -widthForSpecificPie, -heightForSpecificPie);

            return rect;
        }



        #region Geometry creations
        /// <summary>
        /// Returns the <see cref="GraphicsPath"/> by the specified rectangle.
        /// </summary>
        /// <param name="rect"><see cref="RectangleF"/> specified rectanle.</param>
        /// <param name="roundCorner">The radius of corners.</param>
        /// <returns>The <see cref="GraphicsPath"/>.</returns>
        public static GraphicsPath CreateRoundRect(RectangleF rect, SizeF roundCorner)
        {
            GraphicsPath gp = new GraphicsPath();

            float xr = Math.Min(2 * roundCorner.Width, rect.Width);
            float yr = Math.Min(2 * roundCorner.Height, rect.Height);

            if (xr == 0 || yr == 0)
            {
                gp.AddLine(rect.Left, rect.Top, rect.Right, rect.Top);
                gp.AddLine(rect.Right, rect.Bottom, rect.Left, rect.Bottom);
                gp.CloseFigure();
            }
            else
            {
                gp.AddArc(rect.Left, rect.Top, xr, yr, 180, 90);
                gp.AddArc(rect.Right - xr, rect.Top, xr, yr, 270, 90);
                gp.AddArc(rect.Right - xr, rect.Bottom - yr, xr, yr, 0, 90);
                gp.AddArc(rect.Left, rect.Bottom - yr, xr, yr, 90, 90);
                gp.CloseFigure();
            }

            return gp;
        }
        /// <summary>
        /// Returns the <see cref="GraphicsPath"/> by the specified rectangle.
        /// </summary>
        /// <param name="rect"><see cref="RectangleF"/> specified rectanle.</param>
        /// <param name="radius">The radius.</param>
        /// <returns>The <see cref="GraphicsPath"/>.</returns>
        public static GraphicsPath CreateRoundRect(RectangleF rect, float radius)
        {
            GraphicsPath gp = new GraphicsPath();

            float xr = Math.Min(2 * radius, rect.Width);
            float yr = Math.Min(2 * radius, rect.Height);

            if (xr == 0 || yr == 0)
            {
                gp.AddRectangle(rect);
            }
            else if (xr > 0 && yr > 0)
            {
                gp.AddArc(rect.Left, rect.Top, xr, yr, 180, 90);
                gp.AddArc(rect.Right - xr, rect.Top, xr, yr, 270, 90);
                gp.AddArc(rect.Right - xr, rect.Bottom - yr, xr, yr, 0, 90);
                gp.AddArc(rect.Left, rect.Bottom - yr, xr, yr, 90, 90);
                gp.CloseFigure();
            }
            else
            {
                gp.CloseAllFigures();
            }

            return gp;
        }
        /// <summary>
        /// Returns the <see cref="GraphicsPath"/> by the specified rectangle.
        /// </summary>
        /// <param name="rect"><see cref="RectangleF"/> specified rectanle.</param>
        /// <param name="tlRadius">The tl radius.</param>
        /// <param name="trRadius">The tr radius.</param>
        /// <param name="brRadius">The br radius.</param>
        /// <param name="blRadius">The bl radius.</param>
        /// <returns>The <see cref="GraphicsPath"/>.</returns>
        public static GraphicsPath CreateRoundRect(RectangleF rect,
            float tlRadius, float trRadius, float brRadius, float blRadius)
        {
            GraphicsPath gp = new GraphicsPath();

            if (!rect.IsEmpty)
            {
                float tlRadiusX = tlRadius;
                float tlRadiusY = tlRadius;
                float trRadiusX = trRadius;
                float trRadiusY = trRadius;
                float brRadiusX = brRadius;
                float brRadiusY = brRadius;
                float blRadiusX = blRadius;
                float blRadiusY = blRadius;

                if (tlRadius + trRadius > rect.Width)
                {
                    tlRadiusX = tlRadius * rect.Width / (tlRadius + trRadius);
                    trRadiusX = trRadius * rect.Width / (tlRadius + trRadius);
                }

                if (trRadius + brRadius > rect.Height)
                {
                    trRadiusY = trRadius * rect.Height / (trRadius + brRadius);
                    brRadiusY = brRadius * rect.Height / (trRadius + brRadius);
                }

                if (brRadius + blRadius > rect.Width)
                {
                    blRadiusX = blRadius * rect.Width / (brRadius + blRadius);
                    brRadiusX = brRadius * rect.Width / (brRadius + blRadius);
                }

                if (tlRadius + blRadius > rect.Height)
                {
                    tlRadiusY = tlRadius * rect.Height / (tlRadius + blRadius);
                    blRadiusY = tlRadius * rect.Height / (tlRadius + blRadius);
                }

                if (tlRadiusX > 0 && tlRadiusY > 0)
                {
                    gp.AddArc(rect.Left, rect.Top, 2 * tlRadiusX, 2 * tlRadiusY, 180, 90);
                }

                gp.AddLine(rect.Left + tlRadiusX, rect.Top, rect.Right - trRadiusX, rect.Top);

                if (trRadiusX > 0 && trRadiusY > 0)
                {
                    gp.AddArc(rect.Right - 2 * trRadiusX, rect.Top, 2 * trRadiusX, 2 * trRadiusY, 270, 90);
                }

                gp.AddLine(rect.Right, rect.Top + trRadiusY, rect.Right, rect.Bottom - brRadiusY);

                if (brRadiusX > 0 && brRadiusY > 0)
                {
                    gp.AddArc(rect.Right - 2 * brRadiusX, rect.Bottom - 2 * brRadiusY, 2 * brRadiusX, 2 * brRadiusY, 0, 90);
                }

                gp.AddLine(rect.Right - brRadiusX, rect.Bottom, rect.Left + blRadiusX, rect.Bottom);

                if (blRadiusX > 0 && blRadiusY > 0)
                {
                    gp.AddArc(rect.Left, rect.Bottom - 2 * blRadiusY, 2 * blRadiusX, 2 * blRadiusY, 90, 90);
                }

                gp.AddLine(rect.Left, rect.Bottom - blRadiusY, rect.Left, rect.Top + tlRadiusY);
                gp.CloseFigure();
            }

            return gp;
        }

        /// <summary>
        /// Gets the rendom beziers points.
        /// </summary>
        /// <param name="pt1">The PT1.</param>
        /// <param name="pt2">The PT2.</param>
        /// <param name="evr">The evr.</param>
        /// <param name="fault">The fault.</param>
        public static PointF[] GetRendomBeziersPoints(PointF pt1, PointF pt2, float evr, float fault)
        {
            Random rand = new Random((int)(evr * fault));

            float dx = pt2.X - pt1.X;
            float dy = pt2.Y - pt1.Y;
            float length = (float)Math.Sqrt(dx * dx + dy * dy);
            int sectors = (int)Math.Max(4, (3 * Math.Round(length / evr) + 1));
            float nx = dy / length;
            float ny = dx / length;
            float sx = dx / (sectors - 1);
            float sy = dy / (sectors - 1);

            PointF[] points = new PointF[sectors];

            points[0] = pt1;
            points[sectors - 1] = pt2;

            for (int i = 1; i < sectors - 1; i++)
            {
                float f = (float)(2 * fault * rand.NextDouble() - fault);
                points[i] = new PointF(pt1.X + i * sx + f * nx, pt1.Y + i * sy + f * ny);
            }

            return points;
        }
        /// <summary>
        /// Gets the rendom beziers points.
        /// </summary>
        /// <param name="pt1">The PT1.</param>
        /// <param name="pt2">The PT2.</param>
        /// <param name="count">The count.</param>
        /// <param name="fault">The fault.</param>
        /// <returns></returns>
        public static PointF[] GetRendomBeziersPoints(PointF pt1, PointF pt2, int count, float fault)
        {
            Random rand = new Random(10);

            int sectors = 3 * count + 1;
            float dx = pt2.X - pt1.X;
            float dy = pt2.Y - pt1.Y;
            float length = (float)Math.Sqrt(dx * dx + dy * dy);
            float nx = dy / length;
            float ny = dx / length;
            float sx = dx / (sectors - 1);
            float sy = dy / (sectors - 1);

            PointF[] points = new PointF[sectors];

            points[0] = pt1;
            points[sectors - 1] = pt2;

            for (int i = 1; i < sectors - 1; i++)
            {
                float f = (float)(2 * fault * rand.NextDouble() - fault);
                points[i] = new PointF(pt1.X + i * sx + f * nx, pt1.Y + i * sy + f * ny);
            }

            return points;
        }
        /// <summary>
        /// Gets the rendom beziers points.
        /// </summary>
        /// <param name="pt1">The PT1.</param>
        /// <param name="pt2">The PT2.</param>
        /// <param name="evr">The evr.</param>
        /// <param name="fault">The fault.</param>
        public static PointF[] GetWaveBeziersPoints(PointF pt1, PointF pt2, float evr, float fault)
        {
            Random rand = new Random((int)(evr * fault));

            float dx = pt2.X - pt1.X;
            float dy = pt2.Y - pt1.Y;
            float length = (float)Math.Sqrt(dx * dx + dy * dy);
            int sectors = (int)Math.Max(4, (3 * Math.Round(length / evr) + 1));
            float nx = dy / length;
            float ny = dx / length;
            float sx = dx / (sectors - 1);
            float sy = dy / (sectors - 1);

            PointF[] points = new PointF[sectors];

            points[0] = pt1;
            points[sectors - 1] = pt2;

            for (int i = 1; i < sectors - 1; i++)
            {
                float f = (float)(2 * fault * rand.NextDouble() - fault);
                points[i] = new PointF(pt1.X + i * sx + f * nx, pt1.Y + i * sy + f * ny);
            }

            return points;
        }
        /// <summary>
        /// Gets the rendom beziers points.
        /// </summary>
        /// <param name="pt1">The PT1.</param>
        /// <param name="pt2">The PT2.</param>
        /// <param name="count">The count.</param>
        /// <param name="fault">The fault.</param>
        /// <returns></returns>
        public static PointF[] GetWaveBeziersPoints(PointF pt1, PointF pt2, int count, float fault)
        {
            float dx = pt2.X - pt1.X;
            float dy = pt2.Y - pt1.Y;
            float length = (float)Math.Sqrt(dx * dx + dy * dy);
            float nx = fault * dy / length;
            float ny = fault * dx / length;
            float sx = dx / count;
            float sy = dy / count;

            PointF[] points = new PointF[3 * count + 1];

            for (int i = 0; i < count; i++)
            {
                points[3 * i] = new PointF(pt1.X + sx * i, pt1.Y + sy * i);
                points[3 * i + 1] = new PointF(pt1.X + sx * (i + 0.5f) + nx, pt1.Y + sy * (i + 0.5f) + ny);
                points[3 * i + 2] = new PointF(pt1.X + sx * (i + 0.5f) - nx, pt1.Y + sy * (i + 0.5f) - ny);
            }

            points[points.Length - 1] = pt2;

            return points;
        }
        #endregion

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    internal static class DrawingHelper
    {
        #region Constants
        private static StringFormat m_stringFormat = new StringFormat();        
        public readonly static StringFormat NoClipFormat = new StringFormat(StringFormatFlags.NoClip);
        private const int COLOR_COEF = 30;
        #endregion

        #region Prroperties
        /// <summary>
        /// Gets the centered format.
        /// </summary>
        /// <value>The centered format.</value>
        public static StringFormat CenteredFormat
        {
            get
            {
                return m_stringFormat;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        static DrawingHelper()
        {
            m_stringFormat.Alignment = StringAlignment.Center;
            m_stringFormat.LineAlignment = StringAlignment.Center;
        }
        #endregion

        #region Imolementation
        /// <summary>
        /// Interpolates the colors.
        /// </summary>
        /// <param name="startColor">The start color.</param>
        /// <param name="endColor">The end color.</param>
        /// <param name="interpolator">The interpolator.</param>
        /// <returns></returns>
        public static Color LeprColor(Color startColor, Color endColor, double interpolator)
        {
            int a = (int)(startColor.A + interpolator * (endColor.A - startColor.A));
            int r = (int)(startColor.R + interpolator * (endColor.R - startColor.R));
            int g = (int)(startColor.G + interpolator * (endColor.G - startColor.G));
            int b = (int)(startColor.B + interpolator * (endColor.B - startColor.B));

            return GetColorFromARGB(a, r, g, b);
        }
        /// <summary>
        /// Clones the specified <see cref="BrushInfo"/> and changes <see cref="BrushInfo.BackColor"/> property by specified color.
        /// </summary>
        /// <param name="brInfo">Source of result.</param>
        /// <param name="color">Specified color for <see cref="BrushInfo.BackColor"/> property.</param>
        /// <returns>Cloned specified <see cref="BrushInfo"/> with the changed <see cref="BrushInfo.BackColor"/>.</returns>
        public static BrushInfo ChangeBackColor(BrushInfo brInfo, Color color)
        {
            BrushInfo result = null;

            switch (brInfo.Style)
            {
                case BrushStyle.None:
                    break;

                case BrushStyle.Solid:
                    result = new BrushInfo(color);
                    break;

                case BrushStyle.Gradient:
                    {
                        Color[] colors = (Color[])brInfo.GradientColors.ToArray(typeof(Color));
                        colors[0] = color;
                        result = new BrushInfo(brInfo.GradientStyle, colors);
                    }
                    break;

                case BrushStyle.Pattern:
                    {
                        Color[] colors = (Color[])brInfo.GradientColors.ToArray(typeof(Color));
                        colors[0] = color;
                        result = new BrushInfo(brInfo.PatternStyle, colors);
                    }
                    break;
            }

            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="brInfo"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static BrushInfo AddColor(BrushInfo brInfo, Color color)
        {
            BrushInfo result = null;

            switch (brInfo.Style)
            {
                case BrushStyle.None:
                    break;

                case BrushStyle.Solid:
                    result = new BrushInfo(AddColor(brInfo.BackColor, color));
                    break;

                case BrushStyle.Gradient:
                    {
                        Color[] colors = new Color[brInfo.GradientColors.Count];

                        for (int i = 0; i < colors.Length; i++)
                        {
                            colors[i] = AddColor(brInfo.GradientColors[i], color);
                        }

                        result = new BrushInfo(brInfo.GradientStyle, colors);
                    }
                    break;

                case BrushStyle.Pattern:
                    {
                        Color[] colors = new Color[brInfo.GradientColors.Count];

                        for (int i = 0; i < colors.Length; i++)
                        {
                            colors[i] = AddColor(brInfo.GradientColors[i], color);
                        }

                        result = new BrushInfo(brInfo.PatternStyle, colors);
                    }
                    break;
            }

            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="color1"></param>
        /// <param name="color2"></param>
        /// <returns></returns>
        public static Color AddColor(Color color1, Color color2)
        {
            int a = Math.Min(color1.A + color2.A, byte.MaxValue);
            int r = Math.Min(color1.R + color2.R, byte.MaxValue);
            int g = Math.Min(color1.G + color2.G, byte.MaxValue);
            int b = Math.Min(color1.B + color2.B, byte.MaxValue);

            return Color.FromArgb(a, r, g, b);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="brInfo"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static BrushInfo AddColor(BrushInfo brInfo, int value)
        {
            BrushInfo result = null;

            switch (brInfo.Style)
            {
                case BrushStyle.None:
                    result = brInfo;
                    break;

                case BrushStyle.Solid:
                    result = new BrushInfo(AddColor(brInfo.BackColor, value));
                    break;

                case BrushStyle.Gradient:
                    {
                        Color[] colors = new Color[brInfo.GradientColors.Count];

                        for (int i = 0; i < colors.Length; i++)
                        {
                            colors[i] = AddColor(brInfo.GradientColors[i], value);
                        }

                        result = new BrushInfo(brInfo.GradientStyle, colors);
                    }
                    break;

                case BrushStyle.Pattern:
                    {
                        Color[] colors = new Color[brInfo.GradientColors.Count];

                        for (int i = 0; i < colors.Length; i++)
                        {
                            colors[i] = AddColor(brInfo.GradientColors[i], value);
                        }

                        result = new BrushInfo(brInfo.PatternStyle, colors);
                    }
                    break;
            }

            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Color AddColor(Color color, int value)
        {
            return GetColorFromARGB(color.A, color.R + value, color.G + value, color.B + value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="brInfo"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static BrushInfo MedialColor(BrushInfo brInfo, Color color)
        {
            BrushInfo result = null;

            switch (brInfo.Style)
            {
                case BrushStyle.None:
                    break;

                case BrushStyle.Solid:
                    result = new BrushInfo(MedialColor(brInfo.BackColor, color));
                    break;

                case BrushStyle.Gradient:
                    {
                        Color[] colors = new Color[brInfo.GradientColors.Count];

                        for (int i = 0; i < colors.Length; i++)
                        {
                            colors[i] = MedialColor(brInfo.GradientColors[i], color);
                        }

                        result = new BrushInfo(brInfo.GradientStyle, colors);
                    }
                    break;

                case BrushStyle.Pattern:
                    {
                        Color[] colors = new Color[brInfo.GradientColors.Count];

                        for (int i = 0; i < colors.Length; i++)
                        {
                            colors[i] = MedialColor(brInfo.GradientColors[i], color);
                        }

                        result = new BrushInfo(brInfo.PatternStyle, colors);
                    }
                    break;
            }

            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="color1"></param>
        /// <param name="color2"></param>
        /// <returns></returns>
        public static Color MedialColor(Color color1, Color color2)
        {
            int a = (color1.A + color2.A) / 2;
            int r = (color1.R + color2.R) / 2;
            int g = (color1.G + color2.G) / 2;
            int b = (color1.B + color2.B) / 2;

            return Color.FromArgb(a, r, g, b);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Color GetColorFromARGB(int a, int r, int g, int b)
        {
            a = ChartMath.MinMax(a, byte.MinValue, byte.MaxValue);
            r = ChartMath.MinMax(r, byte.MinValue, byte.MaxValue);
            g = ChartMath.MinMax(g, byte.MinValue, byte.MaxValue);
            b = ChartMath.MinMax(b, byte.MinValue, byte.MaxValue);

            return Color.FromArgb(a, r, g, b);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="color"></param>
        /// <param name="width"></param>
        /// <param name="rect"></param>
        /// <param name="borderStyle"></param>
        public static void DrawBorder(Graphics g, Color color, int width, Rectangle rect, BorderStyle borderStyle)
        {
            int w = Math.Min(width, Math.Min(rect.Width, rect.Height) / 2);
            int w2 = w / 2;

            switch (borderStyle)
            {
                case BorderStyle.FixedSingle:
                    {
                        #region BorderStyle.FixedSingle
                        using (Pen pen = new Pen(color, width))
                        {
                            pen.Alignment = PenAlignment.Inset;
                            g.DrawRectangle(pen, rect);
                        }
                        #endregion
                        break;
                    }

                case BorderStyle.Fixed3D:
                    {
                        #region BorderStyle.Fixed3D

                        SolidBrush sbDark = new SolidBrush(AddColor(color, -COLOR_COEF));
                        SolidBrush sbLight = new SolidBrush(AddColor(color, COLOR_COEF));
                        SolidBrush sbDarkDark = new SolidBrush(AddColor(color, -2 * COLOR_COEF));
                        SolidBrush sbLightLight = new SolidBrush(AddColor(color, 2 * COLOR_COEF));

                        GraphicsPath gpl = new GraphicsPath();
                        GraphicsPath gpd = new GraphicsPath();
                        GraphicsPath gpll = new GraphicsPath();
                        GraphicsPath gpdd = new GraphicsPath();

                        gpd.AddPolygon(new Point[]{ new Point( rect.Left, rect.Top ),
                                       new Point( rect.Right, rect.Top ),
                                       new Point( rect.Right - w, rect.Top + w ),
                                       new Point( rect.Left + w, rect.Top + w ),
                                       new Point( rect.Left + w, rect.Bottom - w ),
                                       new Point( rect.Left, rect.Bottom ) });

                        gpdd.AddPolygon(new Point[]{ new Point( rect.Left + w, rect.Top + w ),
                                        new Point( rect.Right - w, rect.Top + w ),
                                        new Point( rect.Right - w2, rect.Top + w2 ),
                                        new Point( rect.Left + w2, rect.Top + w2 ),
                                        new Point( rect.Left + w2, rect.Bottom - w2 ),
                                        new Point( rect.Left + w, rect.Bottom - w ) });

                        gpl.AddPolygon(new Point[]{ new Point( rect.Right - w, rect.Bottom - w ),
                                       new Point( rect.Right - w, rect.Top + w ),
                                       new Point( rect.Right - w2, rect.Top + w2 ),
                                       new Point( rect.Right - w2, rect.Bottom - w2 ),
                                       new Point( rect.Left + w2, rect.Bottom - w2 ),
                                       new Point( rect.Left + w, rect.Bottom - w ) });

                        gpll.AddPolygon(new Point[]{ new Point( rect.Right, rect.Bottom ),
                                        new Point( rect.Right, rect.Top ),
                                        new Point( rect.Right - w, rect.Top + w ),
                                        new Point( rect.Right - w, rect.Bottom - w ),
                                        new Point( rect.Left + w, rect.Bottom - w ),
                                        new Point( rect.Left, rect.Bottom ) });


                        g.FillPath(sbDark, gpd);
                        g.FillPath(sbDarkDark, gpdd);
                        g.FillPath(sbLightLight, gpll);
                        g.FillPath(sbLight, gpl);

                        sbLightLight.Dispose();
                        sbDarkDark.Dispose();
                        sbLight.Dispose();
                        sbDark.Dispose();
                        #endregion
                        break;
                    }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public static GraphicsContainer BeginTransform(Graphics g)
        {
            SmoothingMode smoothingMode = g.SmoothingMode;
            TextRenderingHint textRenderingHint = g.TextRenderingHint;

            GraphicsContainer cont = g.BeginContainer();

            g.SmoothingMode = smoothingMode;
            g.TextRenderingHint = textRenderingHint;

            return cont;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="cont"></param>
        public static void EndTransform(Graphics g, GraphicsContainer cont)
        {
            g.EndContainer(cont);
        }
        /// <summary>
        /// Draws  rectangle with given <see cref="System.Drawing.Pen"/>.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> to render rectangle.</param>
        /// <param name="p">The <see cref="System.Drawing.Pen"/> to draw rectangle.</param>
        /// <param name="rect">Rectangle bounds to draw.</param>
        public static void DrawRectangleF(Graphics g, Pen p, RectangleF rect)
        {
            g.DrawRectangle(p, rect.X, rect.Y, rect.Width, rect.Height);
        }
        /// <summary>
        /// Shortens the text.
        /// </summary>
        /// <param name="g">The graphics</param>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="width">The width.</param>
        /// <returns></returns>
        public static string EllipsesText(Graphics g, string text, Font font, float width)
        {
            if (text.Length >= 1)
            {
                int p1 = 0, p2 = 0;

                SizeF pointsSz = g.MeasureString("...", font);

                if (width > pointsSz.Width)
                {
                    g.MeasureString(text, font, new SizeF(width - pointsSz.Width, pointsSz.Height),
                        m_stringFormat, out p1, out p2);
                }

                if (p1 <= 0)
                {
                    p1 = 1;
                }

                return p1 == text.Length ? text : text.Substring(0, p1) + "...";
            }

            return "";
        }
        #endregion
    }

    /// <summary>
    /// Represents the methods for creation of symbols geometry.
    /// </summary>
    /// <internalonly/>
    [DocumentationExclude()]
    public static class ChartSymbolHelper
    {
        #region Implementation
        /// <summary>
        /// Draw filled <see cref="Syncfusion.Windows.Forms.Chart.ChartSymbolShape"/> with its border.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> to render shape.</param>
        /// <param name="symbol">The <see cref="Syncfusion.Windows.Forms.Chart.ChartSymbolShape"/> to render.</param>
        /// <param name="bounds">The bounds of the shape.</param>
        /// <param name="pen">The <see cref="System.Drawing.Pen"/> to draw shape border.</param>
        /// <param name="brush">The <see cref="System.Drawing.Brush"/> to fill symbol.</param>
        public static void FillAndDrawSymbol(Graphics g, ChartSymbolShape symbol, Rectangle bounds, Pen pen, Brush brush)
        {
            GraphicsPath gp = GetPathSymbol(symbol, bounds);

            if (gp != null)
            {
                g.FillPath(brush, gp);
                g.DrawPath(pen, gp);
            }
        }
        /// <summary>
        /// Draw <see cref="Syncfusion.Windows.Forms.Chart.ChartSymbolShape"/>.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> to render shape.</param>
        /// <param name="symbol">The <see cref="Syncfusion.Windows.Forms.Chart.ChartSymbolShape"/> to render.</param>
        /// <param name="bounds">The bounds of the shape.</param>
        /// <param name="pen">The <see cref="System.Drawing.Pen"/> to draw shape.</param>
        public static void DrawSymbol(Graphics g, ChartSymbolShape symbol, Rectangle bounds, Pen pen)
        {
            GraphicsPath gp = GetPathSymbol(symbol, bounds);

            if (gp != null)
            {
                g.DrawPath(pen, gp);
            }
        }
        /// <summary>
        /// Draw filled <see cref="Syncfusion.Windows.Forms.Chart.ChartSymbolShape"/>.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> to render symbol.</param>
        /// <param name="symbol">The symbol to render.</param>
        /// <param name="bounds">The bounds to render symbol.</param>
        /// <param name="brush">The <see cref="System.Drawing.Brush"/> to fill the symbol.</param>
        public static void FillSymbol(Graphics g, ChartSymbolShape symbol, Rectangle bounds, Brush brush)
        {
            GraphicsPath gp = GetPathSymbol(symbol, bounds);

            if (gp != null)
            {
                g.FillPath(brush, gp);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public static GraphicsPath GetPathSymbol(ChartSymbolShape symbol, Rectangle bounds)
        {
            GraphicsPath gp = null;

            switch (symbol)
            {
                case ChartSymbolShape.Arrow:
                    gp = GetPathArrow(bounds);
                    break;
                case ChartSymbolShape.InvertedArrow:
                    gp = GetPathInvertedArrow(bounds);
                    break;
                case ChartSymbolShape.Circle:
                    gp = GetPathCircle(bounds);
                    break;
                case ChartSymbolShape.Diamond:
                    gp = GetPathDiamond(bounds);
                    break;
                case ChartSymbolShape.Hexagon:
                    gp = GetPathHexagon(bounds);
                    break;
                case ChartSymbolShape.InvertedTriangle:
                    gp = GetPathInvertedTriangle(bounds);
                    break;
                case ChartSymbolShape.Pentagon:
                    gp = GetPathPentagon(bounds);
                    break;
                case ChartSymbolShape.Square:
                    gp = GetPathSquare(bounds);
                    break;
                case ChartSymbolShape.Triangle:
                    gp = GetPathTriangle(bounds);
                    break;
                case ChartSymbolShape.Cross:
                    gp = GetPathCross(bounds);
                    break;
                case ChartSymbolShape.Star:
                    gp = GetPathStar(bounds);
                    break;

                case ChartSymbolShape.HorizLine:
                    gp = GetPathHorizLine(bounds);
                    break;
                case ChartSymbolShape.VertLine:
                    gp = GetPathVertLine(bounds);
                    break;
            }

            return gp;
        }

        /// <summary>
        /// Creates circle path for given rectangle.
        /// </summary>
        /// <param name="bounds">The rectangle to create path.</param>
        /// <returns>Circle path.</returns>
        public static GraphicsPath GetPathCircle(RectangleF bounds)
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddEllipse(bounds);
            return gp;
        }
        /// <summary>
        /// Creates diamond path for given rectangle.
        /// </summary>
        /// <param name="bounds">The rectangle to create diamond path.</param>
        /// <returns>Diamond path.</returns>
        public static GraphicsPath GetPathDiamond(RectangleF bounds)
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddPolygon(new PointF[4]{
                                    new PointF( bounds.X + bounds.Width / 2, bounds.Y ),
                                    new PointF( bounds.Right, bounds.Y + bounds.Height / 2 ),
                                    new PointF( bounds.X + bounds.Width / 2, bounds.Bottom ),
                                    new PointF( bounds.X, bounds.Y + bounds.Height / 2 )
                                  });
            return gp;
        }
        /// <summary>
        /// Creates square <see cref="System.Drawing.Drawing2D.GraphicsPath"/> for given rectangle.
        /// </summary>
        /// <param name="bounds">The rectangle to create path.</param>
        /// <returns>Square path.</returns>
        public static GraphicsPath GetPathSquare(RectangleF bounds)
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddRectangle(bounds);
            return gp;
        }
        /// <summary>
        /// Creates triangle <see cref="System.Drawing.Drawing2D.GraphicsPath"/> for given rectangle.
        /// </summary>
        /// <param name="bounds">The bounds of the path.</param>
        /// <returns>Triangle path.</returns>
        public static GraphicsPath GetPathTriangle(RectangleF bounds)
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddPolygon(new PointF[3]{
                                    new PointF( bounds.X + bounds.Width / 2, bounds.Y ),
                                    new PointF( bounds.X, bounds.Bottom ),
                                    new PointF( bounds.Right, bounds.Bottom )
                                  });
            return gp;
        }
        /// <summary>
        /// Creates inverted triangle <see cref="System.Drawing.Drawing2D.GraphicsPath"/> for given rectangle.
        /// </summary>
        /// <param name="bounds">Bounds of the path.</param>
        /// <returns>Inverted triangle path.</returns>
        public static GraphicsPath GetPathInvertedTriangle(RectangleF bounds)
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddPolygon(new PointF[3]    {
                                        new PointF( bounds.X, bounds.Y ),
                                        new PointF( bounds.Right, bounds.Y ),
                                        new PointF( bounds.X + bounds.Width / 2, bounds.Bottom )
                                      });
            return gp;
        }
        /// <summary>
        /// Creates Arrow <see cref="System.Drawing.Drawing2D.GraphicsPath"/> for given rectangle.
        /// </summary>
        /// <param name="bounds">Bounds of the path.</param>
        /// <returns>Arrow path.</returns>
        public static GraphicsPath GetPathArrow(RectangleF bounds)
        {
            GraphicsPath gp = new GraphicsPath();
            float bodyheight = (bounds.Height / 3);
            float bodywidth = (bounds.Width / 3);

            gp.AddPolygon(new PointF[4]
            { 
             new PointF(bounds.Left, bounds.Top),
              new PointF( bounds.Right,bounds.Top+(bounds.Height/2)),
              new PointF(bounds.Left, bounds.Bottom),
              new PointF( bounds.Right- bodywidth*2, bounds.Bottom-(bounds.Height/2)) 
           });
            return gp;
        }
        /// <summary>
        /// Creates inverted arrow <see cref="System.Drawing.Drawing2D.GraphicsPath"/> for given rectangle.
        /// </summary>
        /// <param name="bounds">Bounds of the path.</param>
        /// <returns>Inverted arrow path.</returns>

        public static GraphicsPath GetPathInvertedArrow(RectangleF bounds)
        {
            GraphicsPath gp = new GraphicsPath();
            float bodyheight = (bounds.Height / 3);
            float bodywidth = (bounds.Width / 3);

            gp.AddPolygon(new PointF[4]
            { 
              new PointF(bounds.Right, bounds.Top),
              new PointF( bounds.Left,bounds.Top+(bounds.Width/2)),
              new PointF(bounds.Right, bounds.Bottom),
              new PointF( bounds.Left + bodyheight*2, bounds.Bottom-(bounds.Width/2))  
           });
            return gp;
        }

        /// <summary>
        /// Creates hexagon <see cref="System.Drawing.Drawing2D.GraphicsPath"/> for given rectangle.
        /// </summary>
        /// <param name="bounds">The bounds of the hexagon path.</param>
        /// <returns>Hexagon path.</returns>
        public static GraphicsPath GetPathHexagon(RectangleF bounds)
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddPolygon(new PointF[6]{
                                    new PointF( bounds.X + bounds.Width / 4, bounds.Y ),
                                    new PointF( bounds.X + bounds.Width * ( 3f / 4f ), bounds.Y ),
                                    new PointF( bounds.Right, bounds.Y + bounds.Height / 2 ),
                                    new PointF( bounds.X + bounds.Width * ( 3f / 4f ), bounds.Bottom ),
                                    new PointF( bounds.X + bounds.Width / 4, bounds.Bottom ),
                                    new PointF( bounds.X, bounds.Y + bounds.Height / 2 )
                                  });
            gp.CloseFigure();
            return gp;
        }
        /// <summary>
        /// Creates pentagon <see cref="System.Drawing.Drawing2D.GraphicsPath"/> for given rectangle.
        /// </summary>
        /// <param name="bounds">The bounds of the path.</param>
        /// <returns>Pentagon path.</returns>
        public static GraphicsPath GetPathPentagon(RectangleF bounds)
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddPolygon(new PointF[]{
                                   new PointF( bounds.X + ( float )bounds.Width / 5f, bounds.Y ),
                                   new PointF( bounds.X + bounds.Width - ( float )bounds.Width / 5f, bounds.Y ),
                                   new PointF( bounds.Right, bounds.Y + ( float )bounds.Height * ( 3f / 5f ) ),
                                   new PointF( bounds.X + ( float )bounds.Width / 2f, bounds.Bottom ),
                                   new PointF( bounds.X, bounds.Y + ( float )bounds.Height * ( 3f / 5f ) )
                                 });
            gp.CloseFigure();
            return gp;
        }
        /// <summary>
        /// Creates cross <see cref="System.Drawing.Drawing2D.GraphicsPath"/> for given rectangle.
        /// </summary>
        /// <param name="bounds">The bounds of the path.</param>
        /// <returns>Cross path.</returns>
        public static GraphicsPath GetPathCross(RectangleF bounds)
        {
            GraphicsPath gp = new GraphicsPath();

            float w_2 = bounds.Width / 2;
            float h_2 = bounds.Height / 2;

            gp.AddLines(new PointF[]{ new PointF( bounds.X, bounds.Y + h_2 ),
                                 new PointF( bounds.X + w_2, bounds.Y + h_2 ),
                                 new PointF( bounds.X + w_2, bounds.Y ),
                                 new PointF( bounds.X + w_2, bounds.Y + h_2 ),
                                 new PointF( bounds.Right, bounds.Y + h_2 ),
                                 new PointF( bounds.X + w_2, bounds.Y + h_2 ),
                                 new PointF( bounds.X + w_2, bounds.Bottom ),
                                 new PointF( bounds.X + w_2, bounds.Y + h_2 ) });
            gp.CloseFigure();

            return gp;
        }
        /// <summary>
        /// Creates star <see cref="System.Drawing.Drawing2D.GraphicsPath"/> for given rectangle.
        /// </summary>
        /// <param name="bounds">The bounds of the path.</param>
        /// <returns>Star path.</returns>
        public static GraphicsPath GetPathStar(RectangleF bounds)
        {
            GraphicsPath gp = new GraphicsPath();

            gp.AddLines(new PointF[]
      {
        new PointF( bounds.X + ( float )bounds.Width / 2f, bounds.Top ),
        new PointF( bounds.X + ( float )bounds.Width * 11/18f, bounds.Y + ( float )bounds.Height * ( 1f / 3f ) ),
        new PointF( bounds.Right, bounds.Y + ( float )bounds.Height * ( 1f / 3f ) ),
        new PointF( bounds.X + ( float )bounds.Width * 2/3f, bounds.Y + ( float )bounds.Height * ( 3f / 5f ) ),
        new PointF( bounds.X + ( float )bounds.Width * 5/6f, bounds.Bottom ),
        new PointF( bounds.X + ( float )bounds.Width / 2f, bounds.Y + ( float )bounds.Height * ( 22f / 30f ) ),
        new PointF( bounds.X + ( float )bounds.Width / 6f, bounds.Bottom ),
        new PointF( bounds.X + ( float )bounds.Width * 1/3f, bounds.Y + ( float )bounds.Height * ( 3f / 5f ) ),
        new PointF( bounds.X, bounds.Y + ( float )bounds.Height * ( 1f / 3f ) ),
        new PointF( bounds.X + ( float )bounds.Width * 7/18f, bounds.Y + ( float )bounds.Height * ( 1f / 3f ) )
      });
            gp.CloseFigure();

            return gp;
        }
        /// <summary>
        /// Creates horizontal line <see cref="System.Drawing.Drawing2D.GraphicsPath"/> for given rectangle.
        /// </summary>
        /// <param name="bounds">The bounds of the path.</param>
        /// <returns>Horizontal line path.</returns>
        public static GraphicsPath GetPathHorizLine(RectangleF bounds)
        {
            GraphicsPath gp = new GraphicsPath();

            gp.AddLine(new PointF(bounds.X, bounds.Y + bounds.Height / 2),
                new PointF(bounds.Right, bounds.Y + bounds.Height / 2));

            return gp;
        }
        /// <summary>
        /// Creates vertical line <see cref="System.Drawing.Drawing2D.GraphicsPath"/> for given rectangle.
        /// </summary>
        /// <param name="bounds">The bounds of the path.</param>
        /// <returns>Vertical line path.</returns>
        public static GraphicsPath GetPathVertLine(RectangleF bounds)
        {
            GraphicsPath gp = new GraphicsPath();

            gp.AddLine(new PointF(bounds.X + bounds.Width / 2, bounds.Top),
                new PointF(bounds.X + bounds.Width / 2, bounds.Bottom));

            return gp;
        }
        #endregion
    }

    /// <summary>
    /// Contains the layout methods.
    /// </summary>
    static class LayoutHelper
    {
        #region Implementation
        /// <summary>
        /// Aligns the rectangle.
        /// </summary>
        /// <param name="bounds">The bounds area.</param>
        /// <param name="size">The size rectangle.</param>
        /// <param name="alignment">The alignment.</param>
        /// <returns></returns>
        public static RectangleF AlignRectangle(RectangleF bounds, SizeF size, ContentAlignment alignment)
        {
            switch (alignment)
            {
                case ContentAlignment.BottomCenter:
                    bounds = new RectangleF(bounds.Left + 0.5f * (bounds.Width - size.Width),
                        bounds.Bottom - size.Height, size.Width, size.Height);
                    break;
                case ContentAlignment.BottomLeft:
                    bounds = new RectangleF(bounds.Left,
                        bounds.Bottom - size.Height, size.Width, size.Height);
                    break;
                case ContentAlignment.BottomRight:
                    bounds = new RectangleF(bounds.Right - size.Width,
                        bounds.Bottom - size.Height, size.Width, size.Height);
                    break;

                case ContentAlignment.MiddleCenter:
                    bounds = new RectangleF(bounds.Left + 0.5f * (bounds.Width - size.Width),
                        bounds.Top + 0.5f * (bounds.Height - size.Height), size.Width, size.Height);
                    break;
                case ContentAlignment.MiddleLeft:
                    bounds = new RectangleF(bounds.Left,
                        bounds.Top + 0.5f * (bounds.Height - size.Height), size.Width, size.Height);
                    break;
                case ContentAlignment.MiddleRight:
                    bounds = new RectangleF(bounds.Right - size.Width,
                        bounds.Top + 0.5f * (bounds.Height - size.Height), size.Width, size.Height);
                    break;

                case ContentAlignment.TopCenter:
                    bounds = new RectangleF(bounds.Left + 0.5f * (bounds.Width - size.Width),
                        bounds.Top, size.Width, size.Height);
                    break;
                case ContentAlignment.TopLeft:
                    bounds = new RectangleF(bounds.Left, bounds.Top, size.Width, size.Height);
                    break;
                case ContentAlignment.TopRight:
                    bounds = new RectangleF(bounds.Left, bounds.Right - size.Width,
                        size.Width, size.Height);
                    break;
            }

            return bounds;
        }
        /// <summary>
        /// Aligns the rectangle.
        /// </summary>
        /// <param name="bounds">The bounds area.</param>
        /// <param name="size">The size rectangle.</param>
        /// <param name="horizontal">The horizontal alignment.</param>
        /// <param name="vertical">The vertical alignment.</param>
        /// <returns></returns>
        public static RectangleF AlignRectangle(RectangleF bounds, SizeF size, ChartAlignment horizontal, ChartAlignment vertical)
        {
            switch (horizontal)
            {
                case ChartAlignment.Near:
                    break;
                case ChartAlignment.Center:
                    bounds.X += 0.5f * (bounds.Width - size.Width);
                    break;
                case ChartAlignment.Far:
                    bounds.X += (bounds.Width - size.Width);
                    break;
            }

            switch (vertical)
            {
                case ChartAlignment.Near:
                    break;
                case ChartAlignment.Center:
                    bounds.Y += 0.5f * (bounds.Height - size.Height);
                    break;
                case ChartAlignment.Far:
                    bounds.Y += (bounds.Height - size.Height);
                    break;
            }

            bounds.Size = size;

            return bounds;
        }
        /// <summary>
        /// Aligns the rectangle.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="size">The size.</param>
        /// <param name="alignment">The alignment.</param>
        /// <returns></returns>
        public static RectangleF AlignRectangle(PointF point, SizeF size, ContentAlignment alignment)
        {
            switch (alignment)
            {
                case ContentAlignment.BottomCenter:
                    point = new PointF(point.X - 0.5f * size.Width, point.Y);
                    break;
                case ContentAlignment.BottomLeft:
                    point = new PointF(point.X - size.Width, point.Y);
                    break;
                case ContentAlignment.BottomRight:
                    break;
                case ContentAlignment.MiddleCenter:
                    point = new PointF(point.X - 0.5f * size.Width, point.Y - 0.5f * size.Height);
                    break;
                case ContentAlignment.MiddleLeft:
                    point = new PointF(point.X - size.Width, point.Y - 0.5f * size.Height);
                    break;
                case ContentAlignment.MiddleRight:
                    point = new PointF(point.X, point.Y - 0.5f * size.Height);
                    break;
                case ContentAlignment.TopCenter:
                    point = new PointF(point.X - 0.5f * size.Width, point.Y - size.Height);
                    break;
                case ContentAlignment.TopLeft:
                    point = new PointF(point.X - size.Width, point.Y - size.Height);
                    break;
                case ContentAlignment.TopRight:
                    point = new PointF(point.X, point.Y - size.Height);
                    break;
            }

            return new RectangleF(point, size);
        }
        /// <summary>
        /// Aligns the rectangle.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="size">The size.</param>
        /// <param name="horizontal">The horizontal alignment.</param>
        /// <param name="vertical">The vertical alignment.</param>
        /// <returns></returns>
        public static RectangleF AlignRectangle(PointF point, SizeF size, ChartAlignment horizontal, ChartAlignment vertical)
        {
            switch (horizontal)
            {
                case ChartAlignment.Near:
                    break;
                case ChartAlignment.Center:
                    point.X -= 0.5f * size.Width;
                    break;
                case ChartAlignment.Far:
                    point.X -= size.Width;
                    break;
            }

            switch (vertical)
            {
                case ChartAlignment.Near:
                    break;
                case ChartAlignment.Center:
                    point.Y -= 0.5f * size.Height;
                    break;
                case ChartAlignment.Far:
                    point.Y -= size.Height;
                    break;
            }

            return new RectangleF(point, size);
        }
        #endregion
    }

    /// <summary>
    /// Provides constants and static methods for mathematical functions.
    /// </summary>
    /// <internalonly/>
    [DocumentationExclude()]
    public static class ChartMath
    {
        #region Constants
        /// <summary>
        /// A ratio of radials to degrees.
        /// </summary>
        public const double ToRadians = Math.PI / 180;
        /// <summary>
        /// A ratio of degrees to radials.
        /// </summary>
        public const double ToDegrees = 180 / Math.PI;
        /// <summary>
        /// A double PI.
        /// </summary>
        public const double DblPI = 2 * Math.PI;
        /// <summary>
        /// A half of PI.
        /// </summary>
        public const double HlfPI = Math.PI / 2;
        /// <summary>
        /// A the minimal value of <see cref="double"/> (magical number).
        /// </summary>
        public const double Epsilon = 0.00001;
        #endregion

        #region Implementation
        /// <summary>
        /// Mods the angle.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <param name="modAngle">The mod angle.</param>
        /// <returns></returns>
        internal static double ModAngle(double angle, double modAngle)
        {
            return angle > 0 ? angle % modAngle : modAngle - angle % modAngle;
        }
        /// <summary>
        /// Rounds the specified value.
        /// </summary>
        /// <param name="value">The value to round.</param>
        /// <param name="div">The div.</param>
        /// <returns></returns>
        public static double Round(double value, double div)
        {
            double result = div * Math.Floor(value / div);

            if (Math.Abs(value - result) >= 0.5 * div)
            {
                if (value > 0)
                {
                    result += div;
                }
                else
                {
                    result -= div;
                }
            }

            return result;
        }
        /// <summary>
        /// Rounds the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="div">The div.</param>
        /// <param name="up">if set to <c>true</c> value will be rounded to greater value.</param>
        /// <returns></returns>
        public static double Round(double value, double div, bool up)
        {
            return div * (up ? Math.Ceiling(value / div) : Math.Floor(value / div));
        }
        /// <summary>
        /// Gets the indices of two closest points by the specified value.
        /// </summary>
        /// <param name="xs">The array of double.</param>
        /// <param name="point">The specified value.</param>
        /// <param name="index1">The index of first point.</param>
        /// <param name="index2">The index of second point.</param>
        public static void GetTwoClosestPoints(double[] xs, double point, out int index1, out int index2)
        {
            index1 = 0;
            index2 = 0;

            int length = xs.Length;
            if (length <= 1) return;

            int hiBound = length - 1;
            int lowBound = 0;
            int pos = xs.Length / 2;

            index2 = 1;

            while (true)
            {
                if (pos <= 0)
                    pos = 1;
                if (pos >= length - 1)
                    pos = length - 2;
                if (pos <= 0) break;


                double dx = point - xs[pos];
                double dx1 = xs[pos - 1] - xs[pos];
                double dx2 = xs[pos + 1] - xs[pos];

                if (dx < 0)
                {
                    if (dx < dx1)
                    {
                        hiBound = pos;
                    }
                    else
                    {
                        index1 = pos - 1;
                        index2 = pos;
                        return;
                    }
                }
                else
                {
                    if (dx > dx2)
                    {
                        lowBound = pos;
                    }
                    else
                    {
                        index1 = pos;
                        index2 = pos + 1;
                        return;
                    }
                }
                if ((hiBound - lowBound) == 1)
                {
                    if (point > xs[length - 1])
                    {
                        index1 = length - 2;
                        index2 = length - 1;
                        return;
                    }
                    if (point < xs[0])
                    {
                        index1 = 0;
                        index2 = 1;
                        return;
                    }

                    return;
                }

                pos = (hiBound + lowBound) / 2;
            }
        }
        /// <summary>
        /// Represents the methods of double function.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public delegate double DoubleFunc(double x);
        /// <summary>
        /// Computes the bisections by the specified function.
        /// </summary>
        /// <param name="fnc">The function handler.</param>
        /// <param name="x1">The x1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="xAccuracy">The x accuracy.</param>
        /// <param name="maxIterationCount">The max iteration count.</param>
        /// <returns></returns>
        public static double Bisection(DoubleFunc fnc, double x1, double x2, double xAccuracy, int maxIterationCount)
        {
            double dx, f, fmid, xmid, minusx;

            f = fnc(x1);
            fmid = fnc(x2);

            if (f * fmid >= 0.0) return double.NaN; //root is not between x1 and x2

            if (f >= 0)
            {
                minusx = x2;
                dx = x1 - x2;
            }
            else
            {
                minusx = x1;
                dx = x2 - x1;
            }

            for (int j = 0; j < maxIterationCount; j++)
            {
                dx /= 2;
                xmid = minusx + dx;
                fmid = fnc(xmid);
                if (fmid <= 0.0) minusx = xmid;
                if (Math.Abs(dx) < xAccuracy || fmid == 0.0) return minusx;
            }

            return double.NaN;
        }
        /// <summary>
        /// Computes the bisections by the specified function.
        /// </summary>
        /// <param name="fnc">The function handler.</param>
        /// <param name="x1">The x1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="xAccuracy">The x accuracy.</param>
        /// <param name="maxIterationCount">The max iteration count.</param>
        /// <param name="tabulCount">The table count.</param>
        /// <returns></returns>
        public static double SmartBisection(DoubleFunc fnc, double x1, double x2, double xAccuracy, int maxIterationCount, int tabulCount)
        {
            double dx = Math.Abs((x2 - x1) / tabulCount);
            double x = Math.Min(x2, x1);
            for (int i = 0; i < tabulCount - 1; i++)
            {
                double f1 = fnc(x);
                double f2 = fnc(x + dx);
                if (f1 * f2 <= 0)
                    return Bisection(fnc, x, x + dx, xAccuracy, maxIterationCount);

                x += dx;
            }

            return double.NaN;
        }
        /// <summary>
        /// Computes the point of lines intersection.
        /// </summary>
        /// <param name="a">The start point of first line.</param>
        /// <param name="b">The end point of first line.</param>
        /// <param name="c">The start point of second line.</param>
        /// <param name="d">The end point of second line.</param>
        /// <returns></returns>
        public static PointF LineSegmentIntersectionPoint(PointF a, PointF b, PointF c, PointF d)
        {
            PointF res = PointF.Empty;

            PointF ab_max = new PointF(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
            PointF ab_min = new PointF(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
            RectangleF abr = RectangleF.FromLTRB(ab_min.X, ab_min.Y, ab_max.X, ab_max.Y);

            PointF cd_max = new PointF(Math.Max(c.X, d.X), Math.Max(c.Y, d.Y));
            PointF cd_min = new PointF(Math.Min(c.X, d.X), Math.Min(c.Y, d.Y));
            RectangleF cdr = RectangleF.FromLTRB(cd_min.X, cd_min.Y, cd_max.X, cd_max.Y);

            if (!abr.IntersectsWith(cdr)) return res;


            PointF ab = new PointF(b.X - a.X, b.Y - a.Y);
            PointF ad = new PointF(d.X - a.X, d.Y - a.Y);
            PointF ac = new PointF(c.X - a.X, c.Y - a.Y);
            double adab = Determinant(ad, ab);
            double acab = Determinant(ac, ab);

            PointF ca = new PointF(a.X - c.X, a.Y - c.Y);
            PointF cb = new PointF(b.X - c.X, b.Y - c.Y);
            PointF cd = new PointF(d.X - c.X, d.Y - c.Y);
            double cacd = Determinant(ca, cd);
            double cbcd = Determinant(cb, cd);

            if ((adab * acab < 0) || (cacd * cbcd < 0))
            {
                double x = Math.Abs(acab) / (Math.Abs(acab) + Math.Abs(adab));
                res = new PointF((float)(c.X + cd.X * x), (float)(c.Y + cd.Y * x));
            }

            return res;
        }
        /// <summary>
        /// Computes the determinant by the specified matrix.
        /// </summary>
        /// <param name="p1">The first row of matrix.</param>
        /// <param name="p2">The second row of matrix.</param>
        /// <returns></returns>
        public static float Determinant(PointF p1, PointF p2)
        {
            return p1.X * p2.Y - p1.Y * p2.X;
        }
        /// <summary>
        /// Indicates whether specified rectangle is intersects with the line.
        /// </summary>
        /// <param name="r">The rectangle.</param>
        /// <param name="a">The start of line.</param>
        /// <param name="b">The end of line.</param>
        /// <returns></returns>
        public static bool RectanlgeIntersectsWithLine(RectangleF r, PointF a, PointF b)
        {
            bool res = false;

            if (r.IsEmpty || a.IsEmpty || b.IsEmpty) return res;

            PointF d1_p1 = new PointF(r.Left, r.Top);
            PointF d1_p2 = new PointF(r.Right, r.Bottom);

            PointF d2_p1 = new PointF(r.Right, r.Top);
            PointF d2_p2 = new PointF(r.Left, r.Bottom);

            if (!LineSegmentIntersectionPoint(d1_p1, d1_p2, a, b).IsEmpty)
                res = true;
            if (!LineSegmentIntersectionPoint(d2_p1, d2_p2, a, b).IsEmpty)
                res = true;

            return res;
        }

        /// <summary>
        /// Checks whether specified value is inside of specified range.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimal value.</param>
        /// <param name="max">The maximal value.</param>
        /// <returns></returns>
        public static double MinMax(double value, double min, double max)
        {
            return value > min ? (value < max ? value : max) : min;
        }
        /// <summary>
        /// Checks whether specified value is inside of specified range.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimal value.</param>
        /// <param name="max">The maximal value.</param>
        /// <returns></returns>
        public static float MinMax(float value, float min, float max)
        {
            return value > min ? (value < max ? value : max) : min;
        }
        /// <summary>
        /// Checks whether specified value is inside of specified range.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimal value.</param>
        /// <param name="max">The maximal value.</param>
        /// <returns></returns>
        public static int MinMax(int value, int min, int max)
        {
            return value > min ? (value < max ? value : max) : min;
        }

        /// <summary>
        /// Mins the max.
        /// </summary>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        public static void MinMax(double value1, double value2, out double min, out double max)
        {
            if (value1 > value2)
            {
                max = value1;
                min = value2;
            }
            else
            {
                min = value1;
                max = value2;
            }
        }

        /// <summary>
        /// Returns the minimal value from array.
        /// </summary>
        /// <param name="values">The array of values.</param>
        /// <returns>The minimal value.</returns>
        public static double Min(double[] values)
        {
            double res = double.MaxValue;

            for (int i = 0; i < values.Length; i++)
            {
                if (res > values[i])
                {
                    res = values[i];
                }
            }

            return res;
        }
        /// <summary>
        /// Returns the maximal value from array.
        /// </summary>
        /// <param name="values">The array of values.</param>
        /// <returns>The maximal value.</returns>
        public static double Max(double[] values)
        {
            double res = double.MinValue;

            for (int i = 0; i < values.Length; i++)
            {
                if (res < values[i])
                {
                    res = values[i];
                }
            }

            return res;
        }

        /// <summary>
        /// Compute the bounds of rotated rectangle.
        /// </summary>
        /// <param name="r">The rectangle.</param>
        /// <param name="angle">The rotation angle.</param>
        /// <returns></returns>
        public static RectangleF RotatedRectangleBounds(RectangleF r, double angle)
        {
            double sin = Math.Sin(angle);
            double cos = Math.Cos(angle);
            SizeF sz = r.Size;
            SizeF rotatedSize = new SizeF((float)Math.Abs(sz.Height * sin) + (float)Math.Abs(sz.Width * cos), (float)Math.Abs(sz.Width * sin) + (float)Math.Abs(sz.Height * cos));

            float x = r.Left;
            float x1 = (float)(x - sz.Height * sin);
            float x2 = (float)(x - sz.Height * sin + sz.Width * cos);
            float x3 = (float)(x + sz.Width * cos);
            x = Math.Min(x, x1);
            x = Math.Min(x, x2);
            x = Math.Min(x, x3);
            float y = r.Top;
            float y1 = (float)(y + sz.Width * sin);
            float y2 = (float)(y + sz.Width * sin + sz.Height * cos);
            float y3 = (float)(y + sz.Height * cos);
            y = Math.Min(y, y1);
            y = Math.Min(y, y2);
            y = Math.Min(y, y3);

            return new RectangleF(x, y, rotatedSize.Width, rotatedSize.Height);
        }
        /// <summary>
        /// Compute the bounds of left-center rotated rectangle.
        /// </summary>
        /// <param name="r">The rectangle.</param>
        /// <param name="angle">The rotation angle.</param>
        /// <returns></returns>
        public static RectangleF LeftCenterRotatedRectangleBounds(RectangleF r, double angle)
        {
            double sin = Math.Sin(angle);
            double cos = Math.Cos(angle);
            PointF c = new PointF(r.X, r.Y + r.Height / 2);
            SizeF sz = r.Size;
            SizeF rotatedSize = new SizeF((float)Math.Abs(sz.Height * sin) + (float)Math.Abs(sz.Width * cos), (float)Math.Abs(sz.Width * sin) + (float)Math.Abs(sz.Height * cos));

            float x = c.X;
            float x1 = (float)(x + sz.Height * sin / 2);
            float x2 = (float)(x + sz.Width * cos + sz.Height * sin / 2);
            float x3 = (float)(x + sz.Width * cos - sz.Height * sin / 2);
            float x4 = (float)(x - sz.Height * sin / 2);
            x = Math.Min(x4, x1);
            x = Math.Min(x, x2);
            x = Math.Min(x, x3);
            float y = c.Y;
            float y1 = (float)(y - sz.Height * cos / 2);
            float y2 = (float)(y + sz.Width * sin - sz.Height * cos / 2);
            float y3 = (float)(y + sz.Width * sin + sz.Height * cos / 2);
            float y4 = (float)(y + sz.Height * cos / 2);
            y = Math.Min(y4, y1);
            y = Math.Min(y, y2);
            y = Math.Min(y, y3);

            return new RectangleF(x, y, rotatedSize.Width, rotatedSize.Height);
        }
        /// <summary>
        /// Compute the bounds of center rotated rectangle.
        /// </summary>
        /// <param name="r">The rectangle.</param>
        /// <param name="angle">The rotation angle.</param>
        /// <returns></returns>
        public static RectangleF CenterRotatedRectangleBounds(RectangleF r, double angle)
        {
            double sin = Math.Sin(angle);
            double cos = Math.Cos(angle);
            PointF c = new PointF(r.X + r.Width / 2, r.Y + r.Height / 2);
            SizeF sz = r.Size;
            SizeF rotatedSize = new SizeF((float)Math.Abs(sz.Height * sin) + (float)Math.Abs(sz.Width * cos), (float)Math.Abs(sz.Width * sin) + (float)Math.Abs(sz.Height * cos));

            float x = c.X;
            float x1 = (float)(x - sz.Width * cos / 2 + sz.Height * sin / 2);
            float x2 = (float)(x + sz.Width * cos / 2 + sz.Height * sin / 2);
            float x3 = (float)(x + sz.Width * cos / 2 - sz.Height * sin / 2);
            float x4 = (float)(x - sz.Width * cos / 2 - sz.Height * sin / 2);
            x = Math.Min(x4, x1);
            x = Math.Min(x, x2);
            x = Math.Min(x, x3);
            float y = c.Y;
            float y1 = (float)(y + sz.Width * sin / 2 + sz.Height * cos / 2);
            float y2 = (float)(y - sz.Width * sin / 2 + sz.Height * cos / 2);
            float y3 = (float)(y - sz.Width * sin / 2 - sz.Height * cos / 2);
            float y4 = (float)(y + sz.Width * sin / 2 - sz.Height * cos / 2);
            y = Math.Min(y4, y1);
            y = Math.Min(y, y2);
            y = Math.Min(y, y3);

            return new RectangleF(x, y, rotatedSize.Width, rotatedSize.Height);
        }
        /// <summary>
        /// Calculates distance between two points.
        /// </summary>
        /// <param name="p1">The point to calculate distance from.</param>
        /// <param name="p2">The point to calculate distance to.</param>
        /// <returns>Distance between to point.</returns>
        public static float DistanceBetweenPoints(PointF p1, PointF p2)
        {
            return (float)Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
        }
        /// <summary>
        /// Gets the point by angle.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <param name="angle">The angle.</param>
        /// <param name="isCircle">if set to <c>true</c> [is circle].</param>
        /// <returns></returns>
        public static PointF GetPointByAngle(RectangleF rect, double angle, bool isCircle)
        {
            PointF result = rect.Location;

            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);

            double rx = 0.5f * rect.Width;
            double ry = 0.5f * rect.Height;


            if (rx != 0 && ry != 0)
            {
                if (isCircle || rx == ry)
                {
                    result = new PointF((float)(rect.X + rx + rx * cos), (float)(rect.Y + ry + ry * sin));
                }
                else
                {
                    double l = 1d / Math.Sqrt((cos * cos) / (rx * rx) + (sin * sin) / (ry * ry));

                    result = new PointF((float)(rect.X + rx + l * cos), (float)(rect.Y + ry + l * sin));
                }
            }

            return result;
        }
        /// <summary>
        /// Creates the <see cref="RectangleF"/> by specified center and radius.
        /// </summary>
        /// <param name="center">The center.</param>
        /// <param name="radius">The radius.</param>
        /// <returns></returns>
        public static RectangleF GetRectByCenter(PointF center, SizeF radius)
        {
            return new RectangleF(center.X - radius.Width, center.Y - radius.Height, 2 * radius.Width, 2 * radius.Height);
        }
        /// <summary>
        /// Returns the center of specified rectangle.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <returns></returns>
        public static PointF GetCenter(RectangleF rect)
        {
            return new PointF(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
        }
        /// <summary>
        /// Returns the half size of rectangle.
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static SizeF GetRadius(RectangleF rect)
        {
            return new SizeF(rect.Width / 2, rect.Height / 2);
        }
        /// <summary>
        /// Corrects the size of the specified rectangle.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <returns></returns>
        public static RectangleF CorrectRect(RectangleF rect)
        {
            return new RectangleF(Math.Min(rect.Left, rect.Right), Math.Min(rect.Top, rect.Bottom),
                Math.Abs(rect.Width), Math.Abs(rect.Height));
        }
        /// <summary>
        /// Creates the <see cref="RectangleF"/> by the specified points and corrects the size.
        /// </summary>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y2">The y2.</param>
        /// <returns></returns>
        public static RectangleF CorrectRect(float x1, float y1, float x2, float y2)
        {
            return new RectangleF(Math.Min(x1, x2), Math.Min(y1, y2), Math.Abs(x2 - x1), Math.Abs(y2 - y1));
        }
        /// <summary>
        /// Translates a given <see cref="PointF"/> by a specified <see cref="SizeF"/>.
        /// </summary>
        /// <param name="pt">The point.</param>
        /// <param name="sz">The size.</param>
        /// <returns></returns>
        public static PointF AddPoint(PointF pt, SizeF sz)
        {
            return new PointF(pt.X + sz.Width, pt.Y + sz.Height);
        }
        /// <summary>
        /// Translates a given <see cref="PointF"/> by a specified <see cref="Size"/>.
        /// </summary>
        /// <param name="pt">The point.</param>
        /// <param name="sz">The size.</param>
        /// <returns></returns>
        public static PointF AddPoint(PointF pt, Size sz)
        {
            return new PointF(pt.X + sz.Width, pt.Y + sz.Height);
        }
        /// <summary>
        /// Gets the normal.
        /// </summary>
        /// <param name="v1">The start point of plane.</param>
        /// <param name="v2">The v2.</param>
        /// <param name="v3">The v3.</param>
        /// <returns></returns>
        public static Vector3D GetNormal(Vector3D v1, Vector3D v2, Vector3D v3)
        {
            Vector3D n = (v1 - v2) * (v3 - v2);
            double l = n.GetLength();

            if (l < Epsilon)
            {
                l = 0;
            }

            return new Vector3D(n.X / l, n.Y / l, n.Z / l);
        }

        /// <summary>
        /// solves quadratic equation in form a*x^2 + b*x + c = 0
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="root1"></param>
        /// <param name="root2"></param>
        internal static bool SolveQuadraticEquation(double a, double b, double c, out double root1, out double root2)
        {
            double D;

            root1 = double.NaN;
            root2 = double.NaN;

            if (a != 0)
            {
                D = b * b - 4 * a * c;

                if (D >= 0)
                {
                    double sd = Math.Sqrt(D);

                    root1 = (-b - sd) / (2 * a);

                    root2 = (-b + sd) / (2 * a);

                    return true;
                }
            }
            else
            {
                if (b != 0)
                {
                    root1 = -c / b;

                    root2 = -c / b;

                    return true;
                }
            }

            return false;
        }
        /// <summary>
        /// solves quadratic equation in form a*x^2 + b*x + c = 0
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="root1"></param>
        /// <param name="root2"></param>
        internal static bool SolveQuadraticEquation(float a, float b, float c, out float root1, out float root2)
        {
            float D;

            root1 = 0;
            root2 = 0;

            if (a != 0)
            {
                D = b * b - 4 * a * c;

                if (D >= 0)
                {
                    root1 = (-b - (float)Math.Sqrt(D)) / (2 * a);

                    root2 = (-b + (float)Math.Sqrt(D)) / (2 * a);

                    return true;
                }
                else return false;
            }
            else
            {
                if (b != 0)
                {
                    root1 = -c / b;

                    root2 = -c / b;

                    return true;
                }
                else return false;
            }
        }
        /// <summary>
        /// Interpolates the bezier.
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="p3">The p3.</param>
        /// <param name="p4">The p4.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        internal static PointF[] InterpolateBezier(PointF p1, PointF p2, PointF p3, PointF p4, int count)
        {
            PointF[] pts = new PointF[count];

            float cx = 3 * (p2.X - p1.X);
            float cy = 3 * (p2.Y - p1.Y);

            float bx = 3 * (p3.X - p2.X) - cx;
            float by = 3 * (p3.Y - p2.Y) - cy;

            float ax = p4.X - p1.X - bx - cx;
            float ay = p4.Y - p1.Y - by - cy;

            for (int i = 0; i < count; i++)
            {
                float f = (float)i / (count - 1);

                float x = ax * f * f * f + bx * f * f + cx * f + p1.X;
                float y = ay * f * f * f + by * f * f + cy * f + p1.Y;

                pts[i] = new PointF(x, y);
            }

            return pts;
        }
        /// <summary>
        /// Splits the bezier curve.
        /// </summary>
        /// <param name="p0">The start point of curve.</param>
        /// <param name="p1">The first control point.</param>
        /// <param name="p2">The second control point.</param>
        /// <param name="p3">The end point of curve.</param>
        /// <param name="t0">The interpolator.</param>
        /// <param name="pb0">The start point of first output curve.</param>
        /// <param name="pb1">The first control point of first output curve.</param>
        /// <param name="pb2">The second control point of first output curve.</param>
        /// <param name="pb3">The end point of first output curve.</param>
        /// <param name="pe0">The start point of second output curve.</param>
        /// <param name="pe1">The first control point of second output curve.</param>
        /// <param name="pe2">The second control point of second output curve.</param>
        /// <param name="pe3">The end point of second output curve.</param>
        internal static void SplitBezierCurve(PointF p0, PointF p1, PointF p2, PointF p3, float t0,
            out PointF pb0, out PointF pb1, out PointF pb2, out PointF pb3,
            out PointF pe0, out PointF pe1, out PointF pe2, out PointF pe3)
        {
            int n = 4;
            float[,] x = new float[n, n];
            float[,] y = new float[n, n];

            x[0, 0] = p0.X;
            x[1, 0] = p1.X;
            x[2, 0] = p2.X;
            x[3, 0] = p3.X;

            y[0, 0] = p0.Y;
            y[1, 0] = p1.Y;
            y[2, 0] = p2.Y;
            y[3, 0] = p3.Y;

            for (int i = 1; i < n; i++)
            {
                for (int j = 0; j < n - i; j++)
                {
                    x[j, i] = x[j, i - 1] * (1 - t0) + x[j + 1, i - 1] * t0;
                    y[j, i] = y[j, i - 1] * (1 - t0) + y[j + 1, i - 1] * t0;
                }
            }

            pb0 = new PointF(x[0, 0], y[0, 0]);
            pb1 = new PointF(x[0, 1], y[0, 1]);
            pb2 = new PointF(x[0, 2], y[0, 2]);
            pb3 = new PointF(x[0, 3], y[0, 3]);

            pe0 = new PointF(x[0, 3], y[0, 3]);
            pe1 = new PointF(x[1, 2], y[1, 2]);
            pe2 = new PointF(x[2, 1], y[2, 1]);
            pe3 = new PointF(x[3, 0], y[3, 0]);
        }
        /// <summary>
        /// Splits the bezier curve.
        /// </summary>
        /// <param name="p0">The start point of curve.</param>
        /// <param name="p1">The first control point.</param>
        /// <param name="p2">The second control point.</param>
        /// <param name="p3">The end point of curve.</param>
        /// <param name="t0">The interpolator.</param>
        /// <param name="pb0">The start point of first output curve.</param>
        /// <param name="pb1">The first control point of first output curve.</param>
        /// <param name="pb2">The second control point of first output curve.</param>
        /// <param name="pb3">The end point of first output curve.</param>
        /// <param name="pe0">The start point of second output curve.</param>
        /// <param name="pe1">The first control point of second output curve.</param>
        /// <param name="pe2">The second control point of second output curve.</param>
        /// <param name="pe3">The end point of second output curve.</param>
        internal static void SplitBezierCurve(ChartPoint p0, ChartPoint p1, ChartPoint p2, ChartPoint p3, double t0,
            out ChartPoint pb0, out ChartPoint pb1, out ChartPoint pb2, out ChartPoint pb3,
            out ChartPoint pe0, out ChartPoint pe1, out ChartPoint pe2, out ChartPoint pe3)
        {
            int n = 4;
            double[,] x = new double[n, n];
            double[,] y = new double[n, n];

            x[0, 0] = p0.X;
            x[1, 0] = p1.X;
            x[2, 0] = p2.X;
            x[3, 0] = p3.X;

            y[0, 0] = p0.YValues[0];
            y[1, 0] = p1.YValues[0];
            y[2, 0] = p2.YValues[0];
            y[3, 0] = p3.YValues[0];

            for (int i = 1; i < n; i++)
            {
                for (int j = 0; j < n - i; j++)
                {
                    x[j, i] = x[j, i - 1] * (1 - t0) + x[j + 1, i - 1] * t0;
                    y[j, i] = y[j, i - 1] * (1 - t0) + y[j + 1, i - 1] * t0;
                }
            }

            pb0 = new ChartPoint(x[0, 0], y[0, 0]);
            pb1 = new ChartPoint(x[0, 1], y[0, 1]);
            pb2 = new ChartPoint(x[0, 2], y[0, 2]);
            pb3 = new ChartPoint(x[0, 3], y[0, 3]);

            pe0 = new ChartPoint(x[0, 3], y[0, 3]);
            pe1 = new ChartPoint(x[1, 2], y[1, 2]);
            pe2 = new ChartPoint(x[2, 1], y[2, 1]);
            pe3 = new ChartPoint(x[3, 0], y[3, 0]);
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    class ChartPlace
    {
        #region Members
        private ArrayList m_series = new ArrayList();
        private ChartSeriesType m_seriesType = ChartSeriesType.Custom;
        private int m_placeIndex = -1;
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public ChartSeriesType SeriesType
        {
            get
            {
                return m_seriesType;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public int PlaceIndex
        {
            get
            {
                return m_placeIndex;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public IList Series
        {
            get
            {
                return m_series;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="series"></param>
        public ChartPlace(int index, ChartSeries series)
        {
            m_placeIndex = index;
            m_series.Add(series);
            m_seriesType = series.Type;

            series.Renderer.Place = index;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="series"></param>
        public void Add(ChartSeries series)
        {
            series.Renderer.Place = m_placeIndex;
            m_series.Add(series);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        public void SetSpaceSize(int size)
        {
            foreach (ChartSeries cs in m_series)
            {
                cs.Renderer.PlaceSize = size;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="series"></param>
        /// <param name="doManyAreas"></param>
        /// <param name="chart"></param>
        /// <returns></returns>
        public static ChartPlace[] CalculateSpace(ChartSeriesCollection series, bool doManyAreas, IChartAreaHost chart)
        {
            int currIndex = 0;
            ChartPlace placeForAll = null;
            ArrayList result = new ArrayList();

            for (int i = 0; i < series.VisibleCount; i++)
            {
                ChartSeries cs = series.VisibleList[i] as ChartSeries;

                if (series.DisableStyles && chart.NeedPerformance)
                {
                    cs.EnableStyles = !series.DisableStyles;
                }

                cs.Renderer.SetChart(chart);

                if (cs.Renderer.CanRender())
                {
                    if (cs.Renderer.FillSpaceType == ChartUsedSpaceType.All)
                    {
                        if (placeForAll == null && i == 0)
                        {
                            placeForAll = new ChartPlace(0, cs);

                            if (!doManyAreas)
                            {
                                break;
                            }
                        }
                        else if (doManyAreas && placeForAll != null)
                        {
                            placeForAll.Add(cs);
                        }
                    }
                    else if (cs.Renderer.FillSpaceType == ChartUsedSpaceType.OneForAll)
                    {
                        bool needCreate = true;

                        foreach (ChartPlace plc in result)
                        {
                            if (plc.SeriesType == cs.Type)
                            {
                                plc.Add(cs);
                                needCreate = false;
                                break;
                            }
                        }

                        if (needCreate)
                        {
                            result.Add(new ChartPlace(currIndex, cs));
                            currIndex++;
                        }
                    }
                    else if (cs.Renderer.FillSpaceType == ChartUsedSpaceType.OneForOne)
                    {
                        result.Add(new ChartPlace(currIndex, cs));
                        currIndex++;
                    }
                }
            }

            if (placeForAll != null)
            {
                result.Clear();
                result.Add(placeForAll);
                currIndex = 1;                
            }

            ChartPlace[] arrayResult = new ChartPlace[result.Count];

            for (int i = 0; i < result.Count; i++)
            {
                arrayResult[i] = result[i] as ChartPlace;
                arrayResult[i].SetSpaceSize(currIndex);
            }

            return arrayResult;
        }
        #endregion
    }
}
