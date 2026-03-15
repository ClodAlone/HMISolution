#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections;
using System.Drawing;
using System.Text.RegularExpressions;

namespace Syncfusion.SVG.IO
{
    /// <summary>
    /// Class for Style.
    /// </summary>
    public class Style :
      IFillAttributes, IStrokeAttributes, ITransformAttribute,
      IOpacityAttribute
    {
        #region Members
        private static Regex m_divisor;
        private Hashtable m_primaryAttrs = new Hashtable();
        private Hashtable m_attributes = new Hashtable();
        #endregion

        #region Properties
        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <value>The attributes.</value>
        public Hashtable Attributes
        {
            get
            {
                return m_attributes;
            }
        }

        /// <summary>
        /// Gets the divisor.
        /// </summary>
        /// <value>The divisor.</value>
        public static Regex Divisor
        {
            get
            {
                if (m_divisor == null)
                {
                    m_divisor = new Regex(
                                  @"\s*(?<text>[^:; ]+)\s*[:;]?",
                                  RegexOptions.Compiled | RegexOptions.IgnoreCase);
                }

                return m_divisor;
            }
        }

        #region IStrokeAttributes Members
        /// <summary>
        /// Gets or sets the stroke.
        /// </summary>
        /// <value>The stroke.</value>
        NoneColor IStrokeAttributes.Stroke
        {
            get
            {
                return (NoneColor)GetAttribute(SVG.ATTR_STROKE, new NoneColor(Color.Empty));
            }
            set
            {
                SetAttribute(m_attributes[SVG.ATTR_STROKE], value, new NoneColor(Color.Empty));
            }
        }

        /// <summary>
        /// Gets or sets the width of the stroke.
        /// </summary>
        /// <value>The width of the stroke.</value>
        Length IStrokeAttributes.StrokeWidth
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_STROKE_WIDTH, new Length(1));
            }
            set
            {
                m_attributes[SVG.ATTR_STROKE_WIDTH] = value;
            }
        }

        /// <summary>
        /// Gets or sets the stroke linecap.
        /// </summary>
        /// <value>The stroke linecap.</value>
        EStrokeLinecap IStrokeAttributes.StrokeLinecap
        {
            get
            {
                return (EStrokeLinecap)GetAttribute(SVG.ATTR_STROKE_LINECAP, EStrokeLinecap.Butt);
            }
            set
            {
                SetAttribute(SVG.ATTR_STROKE_LINECAP, value, EStrokeLinecap.Butt);
            }
        }

        /// <summary>
        /// Gets or sets the stroke linejoin.
        /// </summary>
        /// <value>The stroke linejoin.</value>
        EStrokeLinejoin IStrokeAttributes.StrokeLinejoin
        {
            get
            {
                return (EStrokeLinejoin)GetAttribute(SVG.ATTR_STROKE_LINEJOIN, EStrokeLinejoin.Miter);
            }
            set
            {
                SetAttribute(SVG.ATTR_STROKE_LINEJOIN, value, EStrokeLinejoin.Miter);
            }
        }

        /// <summary>
        /// Gets or sets the stroke miterlimit.
        /// </summary>
        /// <value>The stroke miterlimit.</value>
        Number IStrokeAttributes.StrokeMiterlimit
        {
            get
            {
                return (Number)GetAttribute(SVG.ATTR_STROKE_MITERLIMIT, new Number(4f));
            }
            set
            {
                SetAttribute(SVG.ATTR_STROKE_MITERLIMIT, value, new Number(4f));
            }
        }

        /// <summary>
        /// Gets or sets the stroke dasharray.
        /// </summary>
        /// <value>The stroke dasharray.</value>
        FloatArray IStrokeAttributes.StrokeDasharray
        {
            get
            {
                return (FloatArray)GetAttribute(SVG.ATTR_STROKE_DASHARRAY, null);
            }
            set
            {
                SetAttribute(SVG.ATTR_STROKE_DASHARRAY, value, null);
            }
        }

        /// <summary>
        /// Gets or sets the stroke dashoffset.
        /// </summary>
        /// <value>The stroke dashoffset.</value>
        Length IStrokeAttributes.StrokeDashoffset
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_STROKE_DASHOFFSET, new Length(0));
            }
            set
            {
                SetAttribute(SVG.ATTR_STROKE_DASHOFFSET, value, new Length(0));
            }
        }

        /// <summary>
        /// Gets or sets the stroke opacity.
        /// </summary>
        /// <value>The stroke opacity.</value>
        Opacity IStrokeAttributes.StrokeOpacity
        {
            get
            {
                return (Opacity)GetAttribute(SVG.ATTR_STROKE_OPACITY, new Opacity(1f));
            }
            set
            {
                SetAttribute(SVG.ATTR_STROKE_OPACITY, value, new Opacity(1f));
            }
        }
        #endregion

        #region IFillAttributes Members
        /// <summary>
        /// Gets or sets the fill.
        /// </summary>
        /// <value>The fill.</value>
        NoneColor IFillAttributes.Fill
        {
            get
            {
                return (NoneColor)GetAttribute(SVG.ATTR_FILL, new NoneColor(Color.Black));
            }
            set
            {
                SetAttribute(SVG.ATTR_FILL, value, new NoneColor(Color.Black));
            }
        }

        /// <summary>
        /// Gets or sets the fill opacity.
        /// </summary>
        /// <value>The fill opacity.</value>
        Opacity IFillAttributes.FillOpacity
        {
            get
            {
                return (Opacity)GetAttribute(SVG.ATTR_FILL_OPACITY, new Opacity(1f));
            }
            set
            {
                SetAttribute(SVG.ATTR_FILL_OPACITY, value, new Opacity(1f));
            }
        }
        #endregion

        #region ITransformAttribute Members
        /// <summary>
        /// Gets or sets the transform.
        /// </summary>
        /// <value>The transform.</value>
        TransformList ITransformAttribute.Transform
        {
            get
            {
                return (TransformList)GetAttribute(SVG.ATTR_TRANSFORM, null);
            }
            set
            {
                SetAttribute(SVG.ATTR_TRANSFORM, value, null);
            }
        }
        #endregion

        #region IOpacityAttribute Members
        /// <summary>
        /// Gets or sets the opacity.
        /// </summary>
        /// <value>The opacity.</value>
        Opacity IOpacityAttribute.Opacity
        {
            get
            {
                return (Opacity)GetAttribute(SVG.ATTR_OPACITY, new Opacity(1f));
            }
            set
            {
                SetAttribute(SVG.ATTR_OPACITY, value, new Opacity(1f));
            }
        }
        #endregion

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Style"/> class.
        /// </summary>
        public Style()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Style"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Style(string value)
        {
            ParseString(value);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds the specified attr.
        /// </summary>
        /// <param name="attr">The attr.</param>
        /// <param name="value">The value.</param>
        public void Add(string attr, object value)
        {
            m_attributes.Add(attr, value);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            string res = string.Empty;

            foreach (object key in m_attributes.Keys)
            {
                object val = m_attributes[key];

                res += key.ToString() + ":" + val.ToString() + ";";
            }

            return res;
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Gets the attribute.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defValue">The def value.</param>
        /// <returns>The object.</returns>
        protected object GetAttribute(object key, object defValue)
        {
            object res = m_attributes[key];

            if (res == null)
            {
                res = defValue;
            }

            return res;
        }

        /// <summary>
        /// Sets the attribute.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="defValue">The def value.</param>
        protected void SetAttribute(object key, object value, object defValue)
        {
            if (value == defValue)
            {
                m_attributes.Remove(key);
            }
            else
            {
                m_attributes[key] = value;
            }
        }

        private void ParseString(string value)
        {
            MatchCollection coll = Divisor.Matches(value);

            for (int i = 0, c = coll.Count / 2; i < c; i++)
            {
                string par = coll[2 * i].Groups["text"].Value;
                string val = coll[2 * i + 1].Groups["text"].Value;

                m_primaryAttrs.Add(par, val);
            }

            foreach (string par in m_primaryAttrs.Keys)
            {
                string val = (string)m_primaryAttrs[par];

                switch (par)
                {
                    case SVG.ATTR_FILL:
                        m_attributes.Add(SVG.ATTR_FILL, new NoneColor(val));
                        break;
                    case SVG.ATTR_STROKE:
                        m_attributes.Add(SVG.ATTR_STROKE, new NoneColor(val));
                        break;
                    case SVG.ATTR_STROKE_WIDTH:
                        m_attributes.Add(SVG.ATTR_STROKE_WIDTH, new Length(val));
                        break;
                    case SVG.ATTR_STROKE_OPACITY:
                        m_attributes.Add(SVG.ATTR_STROKE_OPACITY, new Opacity(val));
                        break;
                    case SVG.ATTR_STROKE_MITERLIMIT:
                        m_attributes.Add(SVG.ATTR_STROKE_MITERLIMIT, new Number(val));
                        break;
                    case SVG.ATTR_STROKE_LINEJOIN:
                        m_attributes.Add(SVG.ATTR_STROKE_LINEJOIN, EStrokeLinejoin.Parse(val));
                        break;
                }
            }
        }
        #endregion
    }
}
