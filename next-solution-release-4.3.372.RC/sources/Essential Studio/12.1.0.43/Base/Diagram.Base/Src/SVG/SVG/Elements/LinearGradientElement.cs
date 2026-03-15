#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;

namespace Syncfusion.SVG.IO
{
    /// <summary>
    /// LinearGradientElement class.
    /// </summary>
    public class LinearGradientElement : SuperElement
    {
        #region Proprties
        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>The length.</value>
        public Length X1
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_X1, new Length("0%"));
            }
            set
            {
                SetAttribute(SVG.ATTR_X1, value, new Length("0%"));
            }
        }

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>The length.</value>
        public Length Y1
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_Y1, new Length("0%"));
            }
            set
            {
                SetAttribute(SVG.ATTR_Y1, value, new Length("0%"));
            }
        }

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>The length.</value>
        public Length X2
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_X2, new Length("100%"));
            }
            set
            {
                SetAttribute(SVG.ATTR_X2, value, new Length("100%"));
            }
        }

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>The length.</value>
        public Length Y2
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_Y2, new Length("0%"));
            }
            set
            {
                SetAttribute(SVG.ATTR_Y2, value, new Length("0%"));
            }
        }

        /// <summary>
        /// Gets or sets the gradient transform.
        /// </summary>
        /// <value>The gradient transform.</value>
        public TransformList GradientTransform
        {
            get
            {
                return (TransformList)GetAttribute(SVG.ATTR_GRADIENT_TRANSFORM, new TransformList(new Matrix()));
            }
            set
            {
                SetAttribute(SVG.ATTR_GRADIENT_TRANSFORM, value, new TransformList(new Matrix()));
            }
        }

        /// <summary>
        /// Gets or sets the gradient units.
        /// </summary>
        /// <value>The gradient units.</value>
        public EUnits GradientUnits
        {
            get
            {
                return (EUnits)GetAttribute(SVG.ATTR_GRADIENT_UNITS, EUnits.ObjectBoundingBox);
            }
            set
            {
                SetAttribute(SVG.ATTR_GRADIENT_UNITS, value, EUnits.ObjectBoundingBox);
            }
        }

        /// <summary>
        /// Gets or sets the spread method.
        /// </summary>
        /// <value>The spread method.</value>
        public SpreadMethod SpreadMethod
        {
            get
            {
                return (SpreadMethod)GetAttribute(SVG.ATTR_SPREAD_METHOD, SpreadMethod.pad);
            }
            set
            {
                SetAttribute(SVG.ATTR_SPREAD_METHOD, value, SpreadMethod.pad);
            }
        }

        /// <summary>
        /// Gets or sets the H ref.
        /// </summary>
        /// <value>The H ref.</value>
        public string HRef
        {
            get
            {
                return (string)GetAttribute(SVG.ATTR_HREF, string.Empty);
            }
            set
            {
                SetAttribute(SVG.ATTR_HREF, value, string.Empty);
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="LinearGradientElement"/> class.
        /// </summary>
        public LinearGradientElement()
        {
            m_name = SVG.NAME_LINEAR_GRADIENT;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Gets the GDI brush.
        /// </summary>
        /// <returns>The brush</returns>
        public Brush GetGDIBrush()
        {
            PointF pt1 = new PointF(X1.Value, Y1.Value);
            PointF pt2 = new PointF(X2.Value, Y2.Value);

            RectangleF rc = new RectangleF(pt1, new SizeF(pt2.X - pt1.X, 1));

            LinearGradientBrush br = new LinearGradientBrush(rc, Color.Black, Color.Black, 1f);

            if (Children.Count > 0)
            {
                ColorBlend cb = new ColorBlend(Children.Count);

                for (int i = 0, len = Children.Count; i < len; i++)
                {
                    StopElement se = (StopElement)Children[i];

                    cb.Colors[i] = Color.FromArgb(se.StopOpacity.Alpha, se.StopColor.Color);
                    cb.Positions[i] = se.Offset.GetValue(1f);
                }

                br.InterpolationColors = cb;
            }

            if ((SpreadMethod == SpreadMethod.pad)
              && (GradientUnits == EUnits.ObjectBoundingBox))
            {
                br.WrapMode = WrapMode.Clamp;
            }
            else if ((SpreadMethod == SpreadMethod.repeat)
              && (GradientUnits == EUnits.UserSpaceOnUse))
            {
                br.WrapMode = WrapMode.Tile;
            }

            br.Transform = GradientTransform.Matrix;

            return br;
        }

        /// <summary>
        /// Froms the brush.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <returns>The brush</returns>
        public static LinearGradientElement FromBrush(LinearGradientBrush brush)
        {
            LinearGradientElement res = new LinearGradientElement();

            res.X1 = new Length(brush.Rectangle.Left);
            res.X2 = new Length(brush.Rectangle.Right);

            res.GradientTransform = new TransformList(brush.Transform);

            switch (brush.WrapMode)
            {
                case WrapMode.Clamp:
                    res.SpreadMethod = SpreadMethod.pad;
                    res.GradientUnits = EUnits.ObjectBoundingBox;
                    break;
                case WrapMode.Tile:
                    res.SpreadMethod = SpreadMethod.repeat;
                    res.GradientUnits = EUnits.UserSpaceOnUse;
                    break;
                default:
                    res.SpreadMethod = SpreadMethod.reflect;
                    res.GradientUnits = EUnits.UserSpaceOnUse;
                    break;
            }

            ColorBlend cb = null;

            try
            {
                cb = brush.InterpolationColors;
            }
            catch (Exception)
            {
            }
            finally
            {
                if (cb != null)
                {
                    for (int i = 0; i < cb.Colors.Length; i++)
                    {
                        res.AddChild(StopElement.FromColor(
                          cb.Colors[i], new Length(cb.Positions[i])));
                    }
                }
                else
                {
                    res.AddChild(StopElement.FromColor(
                      brush.LinearColors[0], new Length(0, LengthType.Percentage)));
                    res.AddChild(StopElement.FromColor(
                      brush.LinearColors[1], new Length(100, LengthType.Percentage)));
                }
            }

            return res;
        }

        /// <summary>
        /// Parses the XML.
        /// </summary>
        /// <param name="node">The node.</param>
        internal override void ParseXml(XmlNode node)
        {
            XmlAttribute attrX1 = node.Attributes[SVG.ATTR_X1];
            XmlAttribute attrY1 = node.Attributes[SVG.ATTR_Y1];
            XmlAttribute attrX2 = node.Attributes[SVG.ATTR_X2];
            XmlAttribute attrY2 = node.Attributes[SVG.ATTR_Y2];
            XmlAttribute attrGradientTransform = node.Attributes[SVG.ATTR_GRADIENT_TRANSFORM];
            XmlAttribute attrGradientUnits = node.Attributes[SVG.ATTR_GRADIENT_UNITS];
            XmlAttribute attrSpreadMethod = node.Attributes[SVG.ATTR_SPREAD_METHOD];
            XmlAttribute attrHRef = node.Attributes[SVG.ATTR_HREF];

            if (attrX1 != null)
            {
                X1 = new Length(attrX1.Value);
            }

            if (attrY1 != null)
            {
                Y1 = new Length(attrY1.Value);
            }

            if (attrX2 != null)
            {
                X2 = new Length(attrX2.Value);
            }

            if (attrY2 != null)
            {
                Y2 = new Length(attrY2.Value);
            }

            if (attrGradientTransform != null)
            {
                GradientTransform = new TransformList(attrGradientTransform.Value);
            }

            if (attrGradientUnits != null)
            {
                GradientUnits = EUnits.Parse(attrGradientUnits.Value);
            }

            if (attrSpreadMethod != null)
            {
                SpreadMethod = (SpreadMethod)SpreadMethod.Parse(typeof(SpreadMethod), attrSpreadMethod.Value, true);
            }

            if (attrHRef != null)
            {
                HRef = attrHRef.Value;
            }

            base.ParseXml(node);
        }

        /// <summary>
        /// Draws the specified graphics.
        /// </summary>
        /// <param name="g">The graphics.</param>
        public override void Draw(Graphics g)
        {
        }
        #endregion
    }
}
