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
using System.Drawing;
using System.Drawing.Drawing2D;
using Syncfusion.Documentation;
using System.Xml;

namespace Syncfusion.Windows.Forms.Chart.SvgBase
{
    /// <summary>
    /// Inherited from <see cref="SuperElement"/> element of SVG DOM.
    /// </summary>
    /// <internalonly/>        
    [Syncfusion.Documentation.DocumentationExclude()]
    public class RadialGradientElement : SuperElement
    {
        #region Proprties
        /// <summary>
        /// Gets or sets the X coordinate of gradient center.
        /// </summary>
        /// <value>The X coordinate of center.</value>
        public Length CX
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_CX, new Length("50%"));
            }

            set
            {
                SetAttribute(SVG.ATTR_CX, value, new Length("50%"));
            }

        }

        /// <summary>
        /// Gets or sets the Y coordinate of gradient center.
        /// </summary>
        /// <value>The Y coordinate of center.</value>
        public Length CY
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_CY, new Length("50%"));
            }

            set
            {
                SetAttribute(SVG.ATTR_CY, value, new Length("50%"));
            }
        }

        /// <summary>
        /// Gets or sets the X coordinate of gradient factor.
        /// </summary>
        /// <value>The X coordinate of factor.</value>
        public Length FX
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_FX, new Length("50%"));
            }

            set
            {
                SetAttribute(SVG.ATTR_FX, value, new Length("50%"));
            }
        }

        /// <summary>
        /// Gets or sets the Y coordinate of gradient factor.
        /// </summary>
        /// <value>The Y coordinate of factor.</value>
        public Length FY
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_FY, new Length("50%"));
            }

            set
            {
                SetAttribute(SVG.ATTR_FY, value, new Length("50%"));
            }
        }

        /// <summary>
        /// Gets or sets the radius of gradient.
        /// </summary>
        /// <value>The radius of gradient.</value>
        public Length R
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_R, new Length("100%"));
            }

            set
            {
                SetAttribute(SVG.ATTR_R, value, new Length("100%"));
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
        /// Gets or sets the referense of inherited element.
        /// </summary>
        /// <value>The referense.</value>
        public string HRef
        {
            get
            {
                return (string)GetAttribute(SVG.ATTR_HREF, "");
            }

            set
            {
                SetAttribute(SVG.ATTR_HREF, value, "");
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="RadialGradientElement"/> class.
        /// </summary>
        public RadialGradientElement()
            : base(SVG.NAME_RADIAL_GRADIENT)
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Creates the GDI+ brush.
        /// </summary>
        /// <returns>Returns Brush.</returns>
        [Obsolete]
        public Brush GetGDIBrush()
        {
            //      m_parent.Attributes
            //
            //
            //      PointF pt1 = new PointF( X1.Value, Y1.Value );
            //      PointF pt2 = new PointF( X2.Value, Y2.Value );
            //
            //      RectangleF rc = new RectangleF( pt1, new SizeF( pt2.X-pt1.X, 1 ));
            //
            //      RadialGradientBrush br = new RadialGradientBrush( rc, Color.Black, Color.Black, 1f );
            //
            //      #region Get InterpolationColors
            //      if( Children.Count > 0 )
            //      {
            //        ColorBlend cb = new ColorBlend( Children.Count );
            //
            //        for( int i = 0, len = Children.Count; i < len; i ++ )
            //        {
            //          StopElement se = (StopElement)Children[ i ];
            //
            //          cb.Colors[ i ] = Color.FromArgb( se.StopOpacity.Alpha, se.StopColor.Color );
            //          cb.Positions[ i ] = se.Offset.GetValue( 1f );
            //        }
            //
            //        br.InterpolationColors = cb;
            //      }
            //      #endregion
            //
            //      if( ( SpreadMethod == SpreadMethod.pad )
            //        &&( GradientUnits == EUnits.ObjectBoundingBox ) )
            //      {
            //        br.WrapMode = WrapMode.Clamp;
            //      }
            //      else if( ( SpreadMethod == SpreadMethod.repeat )
            //        &&( GradientUnits == EUnits.UserSpaceOnUse ) )
            //      {
            //        br.WrapMode = WrapMode.Tile;
            //      }
            //
            //      br.Transform = GradientTransform.Matrix;
            //
            //      return br;

            return null;
        }

        /// <summary>
        /// Creates the <see cref="RadialGradientElement"/> by the specified GDI+ brush.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <returns>Returns RadialGradientElement from PathGradientBrush.</returns>
        public static RadialGradientElement FromBrush(PathGradientBrush brush)
        {
            RadialGradientElement res = new RadialGradientElement();

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

            res.AddChild(StopElement.FromColor(
              brush.CenterColor, new Length(0, LengthType.Percentage)));

            if (brush.SurroundColors != null)
            {
                for (int i = 0; i < brush.SurroundColors.Length; i++)
                {
                    res.AddChild(StopElement.FromColor(
                      brush.SurroundColors[i], new Length(100 * (i + 1) / brush.SurroundColors.Length, LengthType.Percentage)));
                }
            }
            
            return res;
        }

        /// <summary>
        /// Parses the XML document.
        /// </summary>
        /// <param name="node">The <see cref="XmlNode"/>.</param>
        internal override void ParseXml(XmlNode node)
        {
            XmlAttribute attrCX = node.Attributes[SVG.ATTR_CX];
            XmlAttribute attrCY = node.Attributes[SVG.ATTR_CY];
            XmlAttribute attrFX = node.Attributes[SVG.ATTR_FX];
            XmlAttribute attrFY = node.Attributes[SVG.ATTR_FY];
            XmlAttribute attrR = node.Attributes[SVG.ATTR_R];
            XmlAttribute attrGradientTransform = node.Attributes[SVG.ATTR_GRADIENT_TRANSFORM];
            XmlAttribute attrGradientUnits = node.Attributes[SVG.ATTR_GRADIENT_UNITS];
            XmlAttribute attrSpreadMethod = node.Attributes[SVG.ATTR_SPREAD_METHOD];
            XmlAttribute attrHRef = node.Attributes[SVG.ATTR_HREF];

            if (attrCX != null)
            {
                CX = new Length(attrCX.Value);
            }

            if (attrCY != null)
            {
                CY = new Length(attrCY.Value);
            }

            if (attrFX != null)
            {
                FX = new Length(attrFX.Value);
            }

            if (attrFY != null)
            {
                FY = new Length(attrFY.Value);
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
        /// Draws the element and the inner elements to the specified <see cref="Graphics"/>.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/>.</param>
        public override void Draw(Graphics g)
        {
        }
        #endregion
    }
}
