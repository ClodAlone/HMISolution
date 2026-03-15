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
using System.Xml;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Syncfusion.Windows.Forms.Chart.SvgBase
{
    /// <summary>
    /// Implements the "pattern" element of SVG DOM.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public sealed class PatternElement : SuperElement
    {
        #region Constants
        private const float H8 = 8;
        private const float H7 = H6 + H1;
        private const float H6 = H4 + H2;
        private const float H5 = H4 + H1;
        private const float H4 = H8 / 2;
        private const float H3 = H2 + H1;
        private const float H2 = H8 / 4;
        private const float H1 = H8 / 8;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public Length Width
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_WIDTH, Length.Empty);
            }
            set
            {
                this.SetAttribute(SVG.ATTR_WIDTH, value, Length.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public Length Height
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_HEIGHT, Length.Empty);
            }

            set
            {
                this.SetAttribute(SVG.ATTR_HEIGHT, value, Length.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the X coordinate of pettern.
        /// </summary>
        /// <value>The X coordinate.</value>
        public Length X
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_X, Length.Empty);
            }

            set
            {
                this.SetAttribute(SVG.ATTR_X, value, Length.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the Y coordinate of pettern.
        /// </summary>
        /// <value>The Y coordinate.</value>
        public Length Y
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_Y, new Length(0));
            }

            set
            {
                this.SetAttribute(SVG.ATTR_Y, value, Length.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the reference to a different "pattern" element within the current SVG document fragment.
        /// </summary>
        /// <value>The reference.</value>
        public string HRef
        {
            get
            {
                return (string)GetAttribute(SVG.ATTR_HREF, "");
            }

            set
            {
                this.SetAttribute(SVG.ATTR_HREF, value, "");
            }
        }

        /// <summary>
        /// Gets or sets the pattern transform.
        /// </summary>
        /// <value>The pattern transform.</value>
        public TransformList PatternTransform
        {
            get
            {
                return (TransformList)GetAttribute(SVG.ATTR_PATTERN_TRANSFORM, new TransformList(new Matrix()));
            }

            set
            {
                this.SetAttribute(SVG.ATTR_PATTERN_TRANSFORM, value, new TransformList(new Matrix()));
            }
        }

        /// <summary>
        /// Gets or sets the pattern content units.
        /// </summary>
        /// <value>The pattern content units.</value>
        public EUnits PatternContentUnits
        {
            get
            {
                return (EUnits)GetAttribute(SVG.ATTR_PATTERN_CONTENT_UNITS, EUnits.ObjectBoundingBox);
            }

            set
            {
                SetAttribute(SVG.ATTR_PATTERN_CONTENT_UNITS, value, EUnits.ObjectBoundingBox);
            }
        }

        /// <summary>
        /// Gets or sets the pattern units.
        /// </summary>
        /// <value>The pattern units.</value>
        public EUnits PatternUnits
        {
            get
            {
                return (EUnits)GetAttribute(SVG.ATTR_PATTERN_UNITS, EUnits.ObjectBoundingBox);
            }

            set
            {
                SetAttribute(SVG.ATTR_PATTERN_UNITS, value, EUnits.ObjectBoundingBox);
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PatternElement"/> class.
        /// </summary>
        public PatternElement()
            : base(SVG.NAME_PATTERN)
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Parses the XML document.
        /// </summary>
        /// <param name="node">The <see cref="XmlNode"/>.</param>
        internal override void ParseXml(XmlNode node)
        {
            XmlAttribute x = node.Attributes[SVG.ATTR_X];
            XmlAttribute y = node.Attributes[SVG.ATTR_Y];
            XmlAttribute width = node.Attributes[SVG.ATTR_WIDTH];
            XmlAttribute heigth = node.Attributes[SVG.ATTR_HEIGHT];
            XmlAttribute href = node.Attributes[SVG.ATTR_HREF];
            XmlAttribute patternTransform = node.Attributes[SVG.ATTR_PATTERN_TRANSFORM];
            XmlAttribute patternContentUnits = node.Attributes[SVG.ATTR_PATTERN_CONTENT_UNITS];
            XmlAttribute patternUnits = node.Attributes[SVG.ATTR_PATTERN_UNITS];

            if (x != null)
            {
                X = new Length(x.Value);
            }

            if (y != null)
            {
                Y = new Length(y.Value);
            }

            if (width != null)
            {
                Width = new Length(width.Value);
            }

            if (heigth != null)
            {
                Height = new Length(heigth.Value);
            }

            if (href != null)
            {
                HRef = href.Value;
            }

            if (patternContentUnits != null)
            {
                PatternContentUnits = EUnits.Parse(patternContentUnits.Value);
            }

            if (patternTransform != null)
            {
                PatternTransform = new TransformList(patternTransform.Value);
            }

            if (patternUnits != null)
            {
                PatternUnits = EUnits.Parse(patternUnits.Value);
            }

            base.ParseXml(node);
        }

        /// <summary>
        /// Creates the new <see cref="PatternElement"/> instance by the specified <see cref="TextureBrush"/>.
        /// </summary>
        /// <param name="brush">The <see cref="TextureBrush"/>.</param>
        /// <returns>Returns PatternElement from TextureBrush.</returns>
        public static PatternElement FormTextureBrush(TextureBrush brush)
        {
            PatternElement res = new PatternElement();

            res.Width = new Length(brush.Image.Width);
            res.Height = new Length(brush.Image.Height);
            res.PatternTransform = new TransformList(brush.Transform);

            switch (brush.WrapMode)
            {
                case WrapMode.Clamp:
                    res.PatternUnits = EUnits.ObjectBoundingBox;
                    break;
                case WrapMode.Tile:
                    res.PatternUnits = EUnits.UserSpaceOnUse;
                    break;
                default:
                    res.PatternUnits = EUnits.UserSpaceOnUse;
                    break;
            }

            ImageElement img = ImageElement.FromImage(brush.Image, 0, 0, brush.Image.Width, brush.Image.Height);
            res.AddChild(img);

            return res;
        }

        /// <summary>
        /// Creates the new <see cref="PatternElement"/> instance by the specified <see cref="HatchBrush"/>.
        /// </summary>
        /// <param name="brush">The <see cref="HatchBrush"/>.</param>
        /// <returns>Returns PatternElement from HatchBrush.</returns>
        public static PatternElement FormHatchBrush(HatchBrush brush)
        {
            PatternElement res = new PatternElement();

            res.Width = new Length(H8);
            res.Height = new Length(H8);
            res.PatternUnits = EUnits.UserSpaceOnUse;

            Element[] elems = null;

            #region Create hatch
            switch (brush.HatchStyle)
            {
                case HatchStyle.BackwardDiagonal:
                    elems = new Element[] { LineElement.FromCoordinates(H8, 0, 0, H8) };
                    break;
                case HatchStyle.Cross:
                    elems = new Element[]{ LineElement.FromCoordinates( 0, H4, H8, H4 ),
                                 LineElement.FromCoordinates( H4, 0,H4, H8 )};
                    break;
                case HatchStyle.LightDownwardDiagonal:
                case HatchStyle.DarkDownwardDiagonal:
                    elems = new Element[]{ LineElement.FromCoordinates( H4, 0, H8, H8 ),
                                 LineElement.FromCoordinates( 0, H4, H4, H8 ),
                                 LineElement.FromCoordinates( 0, 0, H8, H8 )};
                    break;
                case HatchStyle.LightHorizontal:
                case HatchStyle.DarkHorizontal:
                    elems = new Element[]{ LineElement.FromCoordinates( 0, H2, H8, H6 ),
                                 LineElement.FromCoordinates( 0, H6, H8, H6 )};
                    break;
                case HatchStyle.LightUpwardDiagonal:
                case HatchStyle.DarkUpwardDiagonal:
                    elems = new Element[]{ LineElement.FromCoordinates( 0, H4, H4, 0 ),
                                 LineElement.FromCoordinates( H4, H8, H8, H4 ),
                                 LineElement.FromCoordinates( 0, H8, H8, 0 )};
                    break;
                case HatchStyle.LightVertical:
                case HatchStyle.DarkVertical:
                    elems = new Element[]{ LineElement.FromCoordinates( H2, 0, H2, H8 ),
                                 LineElement.FromCoordinates( H6, 0, H6, H8 )};
                    break;
                case HatchStyle.DashedDownwardDiagonal:
                    elems = new Element[]{ LineElement.FromCoordinates( 0, 0, H4, H4 ),
                                 LineElement.FromCoordinates( H4, 0, H8, H4 )};
                    break;
                case HatchStyle.DashedHorizontal:
                    elems = new Element[]{ LineElement.FromCoordinates( 0, H2, H4, H2 ),
                                 LineElement.FromCoordinates( H4, H6, H8, H6 )};
                    break;
                case HatchStyle.DashedUpwardDiagonal:
                    elems = new Element[]{ LineElement.FromCoordinates( H4, 0, 0, H4 ),
                                 LineElement.FromCoordinates( H8, 0, H4, H4 )};
                    break;
                case HatchStyle.DashedVertical:
                    elems = new Element[]{ LineElement.FromCoordinates( H2, 0, H2, H4 ),
                                 LineElement.FromCoordinates( H6, H4, H6, H8 )};
                    break;
                case HatchStyle.DiagonalBrick:
                    elems = new Element[]{ LineElement.FromCoordinates( 0, H8, H8, 0 ),
                                 LineElement.FromCoordinates( 0, 0, H4, H4 )};
                    break;
                case HatchStyle.DiagonalCross:
                    elems = new Element[]{ LineElement.FromCoordinates( 0, 0, H8, H8 ),
                                 LineElement.FromCoordinates( H8, 0, 0, H8 )};
                    break;
                case HatchStyle.Divot:
                    elems = new Element[]{ LineElement.FromCoordinates( H2, H4, H4, H4 ),
                                 LineElement.FromCoordinates( H4, H4, H2, H6 )};
                    break;
                case HatchStyle.DottedDiamond:
                    elems = new Element[]{ LineElement.FromCoordinates( 0, 0, H8, H8 ),
                                 LineElement.FromCoordinates( 0, H8, H8, 0 )};
                    break;
                case HatchStyle.DottedGrid:
                    elems = new Element[]{ LineElement.FromCoordinates( H4, 0, H4, H8 ),
                                 LineElement.FromCoordinates( 0, H4, H8, H4 )};
                    break;
                case HatchStyle.ForwardDiagonal:
                    elems = new Element[] { LineElement.FromCoordinates(0, 0, H8, H8) };
                    break;
                case HatchStyle.Horizontal:
                    elems = new Element[] { LineElement.FromCoordinates(0, H4, H8, H4) };
                    break;
                case HatchStyle.HorizontalBrick:
                    elems = new Element[]{ LineElement.FromCoordinates( 0, H3, H8, H3 ),
                                 LineElement.FromCoordinates( H3, 0, H3, H3 ),	
                                 LineElement.FromCoordinates( 0, H3, 0, H7 ),
                                 LineElement.FromCoordinates( 0, H7, H7, H7 )};
                    break;
                case HatchStyle.LargeCheckerBoard:
                    elems = new Element[]{ RectElement.FromCoordinates( 0, 0, H3, H3 ),
                                 RectElement.FromCoordinates( H4, H4, H4, H4 )};
                    break;
                case HatchStyle.LargeConfetti:
                    elems = new Element[]{ RectElement.FromCoordinates( 0, 0, H1, H1 ),
                                 RectElement.FromCoordinates( H2, H3, H1, H1 ),
                                 RectElement.FromCoordinates( H5, H2, H1, H1 ),
                                 RectElement.FromCoordinates( H6, H6, H1, H1 )};
                    break;
                case HatchStyle.NarrowHorizontal:
                    elems = new Element[]{ LineElement.FromCoordinates( 0, H1, H8, H1 ),
                                 LineElement.FromCoordinates( 0, H3, H8, H3 ),
                                 LineElement.FromCoordinates( 0, H5, H8, H5 ),
                                 LineElement.FromCoordinates( 0, H7, H8, H7 )};
                    break;
                case HatchStyle.NarrowVertical:
                    elems = new Element[]{ LineElement.FromCoordinates(H1,0,H1,H8 ),
                                 LineElement.FromCoordinates(H3,0,H3,H8 ),
                                 LineElement.FromCoordinates(H5,0,H5,H8 ),
                                 LineElement.FromCoordinates(H7,0,H7,H8 )};
                    break;
                case HatchStyle.OutlinedDiamond:
                    elems = new Element[]{ LineElement.FromCoordinates(0,0,H8,H8 ),
                                 LineElement.FromCoordinates(H8,0,0,H8 )};
                    break;
                case HatchStyle.Plaid:
                    elems = new Element[]{ LineElement.FromCoordinates( 0, 0, H8, 0 ),
                                 LineElement.FromCoordinates( 0, H3, H8, H3 ),
                                 RectElement.FromCoordinates( 0, H4, H3, H3 )};
                    break;
                case HatchStyle.Shingle:
                    elems = new Element[]{ LineElement.FromCoordinates( 0, H2, H2, 0 ),
                                 LineElement.FromCoordinates(H2, 0, H7, H5 ),
                                 LineElement.FromCoordinates(0, H3, H3, H7 )};
                    break;
                case HatchStyle.SmallCheckerBoard:
                    elems = new Element[]{ RectElement.FromCoordinates( 0, 0, H1, H1 ),
                                 RectElement.FromCoordinates( H4, H4, H1, H1 ),
                                 RectElement.FromCoordinates( H4, 0, H1, H1 ),
                                 RectElement.FromCoordinates( 0, H4, H1, H1 )};
                    break;
                case HatchStyle.SmallConfetti:
                    elems = new Element[]{ LineElement.FromCoordinates( 0, 0, H2, H2 ),
                                 LineElement.FromCoordinates( H7, H3, H5, H5 ),	
                                 LineElement.FromCoordinates( H2, H6, H4, H4 )};
                    break;
                case HatchStyle.SmallGrid:
                    elems = new Element[]{ LineElement.FromCoordinates( 0, H2, H8, H2 ),
                                 LineElement.FromCoordinates( 0, H6, H8, H6 ),
                                 LineElement.FromCoordinates( H2, 0, H2, H8 ),
                                 LineElement.FromCoordinates( H6, 0, H6, H8 )};
                    break;
                case HatchStyle.SolidDiamond:
                    elems = new Element[]{ PolygonElement.FromPoints( 
                                 new PointF[]{
                                               new PointF( H3, 0 ), 
                                               new PointF( H6, H3 ),
                                               new PointF( H3, H6 ),
                                               new PointF( 0, H3 )} )};
                    break;
                case HatchStyle.Sphere:
                    elems = new Element[] { EllipseElement.FromCoordinates(H3, H3, H2, H2) };
                    break;
                case HatchStyle.Trellis:
                    elems = new Element[]{ LineElement.FromCoordinates( 0, H1, H8, H1 ),
                                 LineElement.FromCoordinates( 0, H3, H8, H3 ),
                                 LineElement.FromCoordinates( 0, H5, H8, H5 ),
                                 LineElement.FromCoordinates( 0, H7, H8, H7 )};
                    break;
                case HatchStyle.Vertical:
                    elems = new Element[] { LineElement.FromCoordinates(0, 0, 0, H8) };
                    break;
                case HatchStyle.Wave:
                    elems = new Element[]{ LineElement.FromCoordinates( 0, H4, H3, H2 ),
                                 LineElement.FromCoordinates( H3, H2, H8, H4 ) };
                    break;
                case HatchStyle.Weave:
                    elems = new Element[]{ LineElement.FromCoordinates( 0, H4, H4, 0 ),
                                 LineElement.FromCoordinates( H8, H4, H4, H8 ),
                                 LineElement.FromCoordinates( 0, 0, 0, H4 ),
                                 LineElement.FromCoordinates( 0, H4, H4, H8 )};
                    break;
                case HatchStyle.WideDownwardDiagonal:
                    elems = new Element[]{ LineElement.FromCoordinates( 0, 0, H8, H8 ),
                                 LineElement.FromCoordinates( 0, H1, H8, 9 ),
                                 LineElement.FromCoordinates( H7, 0, H8, H1 )};
                    break;
                case HatchStyle.WideUpwardDiagonal:
                    elems = new Element[]{ LineElement.FromCoordinates( H8, 0, 0, H8 ),
                                 LineElement.FromCoordinates( H8, H1, 0, H8 + H1 ),
                                 LineElement.FromCoordinates( 0, H1, -H1, 0 )};
                    break;
                case HatchStyle.ZigZag:
                    elems = new Element[]{ LineElement.FromCoordinates( 0, H4, H4, 0 ),
                                 LineElement.FromCoordinates( H4, 0, H8, H4 )};
                    break;

                case HatchStyle.Percent05:
                    elems = new Element[]{ LineElement.FromCoordinates( 0, 0, H1, 0 ),
                                 LineElement.FromCoordinates( H4, H4, H5, H4 )};
                    break;
                case HatchStyle.Percent10:
                    elems = new Element[]{ LineElement.FromCoordinates( 0, 0, H1, 0 ),
                                 LineElement.FromCoordinates( H4, H2, H5, H2 ),
                                 LineElement.FromCoordinates( H2, H4, H3, H4 ),
                                 LineElement.FromCoordinates( H6, H6, H7, H6 )};
                    break;
                case HatchStyle.Percent20:
                    elems = new Element[]{ LineElement.FromCoordinates( 0, 0, H2, 0 ),
                                 LineElement.FromCoordinates( H4, H2, H6, H2 ),
                                 LineElement.FromCoordinates( H2, H4, H4, H4 ),
                                 LineElement.FromCoordinates( H5, H6, H7, H6 )};
                    break;
                case HatchStyle.Percent25:
                    elems = new Element[]{ LineElement.FromCoordinates( 0, 0, H3, 0 ),
                                 LineElement.FromCoordinates( H4, H2, H6, H2 ),
                                 LineElement.FromCoordinates( H2, H4, H5, H4 ),
                                 LineElement.FromCoordinates( H5, H6, H7, H6 )};
                    break;
                case HatchStyle.Percent30:
                    elems = new Element[]{ RectElement.FromCoordinates( 0, 0, H3, H1 ),
                                 LineElement.FromCoordinates( H4, H2, H6, H2 ),
                                 RectElement.FromCoordinates( H2, H4, H3, H1 ),
                                 LineElement.FromCoordinates( H5, H6, H7, H6 )};
                    break;
                case HatchStyle.Percent40:
                    elems = new Element[]{ RectElement.FromCoordinates( 0, 0, H3, H1 ),
                                 RectElement.FromCoordinates( H4, H2, H3, H1 ),
                                 RectElement.FromCoordinates( H2, H4, H1, H1 ),
                                 RectElement.FromCoordinates( H5, H6, H3, H1 )};
                    break;
                case HatchStyle.Percent50:
                    elems = new Element[]{ RectElement.FromCoordinates( 0, 0, H3, H3 ),
                                 RectElement.FromCoordinates( H4, H4, H4, H4 )};
                    break;
                case HatchStyle.Percent60:
                    elems = new Element[]{ RectElement.FromCoordinates( 0, 0, H4, H3 ),
                                 RectElement.FromCoordinates( H4, H4, H4, H4 )};
                    break;
                case HatchStyle.Percent70:
                    elems = new Element[]{ RectElement.FromCoordinates( 0, 0, H4, H5 ),
                                 RectElement.FromCoordinates( H4, H4, H4, H4 )};
                    break;
                case HatchStyle.Percent75:
                    elems = new Element[]{ RectElement.FromCoordinates( 0, 0, H7, H3 ),
                                 RectElement.FromCoordinates( 0, H2, H3, H7 )};
                    break;
                case HatchStyle.Percent80:
                    elems = new Element[]{ RectElement.FromCoordinates( 0, 0, H7, H4 ),
                                 RectElement.FromCoordinates( 0, H2, H4, H7 )};
                    break;
                case HatchStyle.Percent90:
                    elems = new Element[]{ RectElement.FromCoordinates( 0, 0, H7, H5 ),
                                 RectElement.FromCoordinates( 0, H2, H5, H7 )};
                    break;

                default:
                    break;
            }
            #endregion

            if (elems != null)
            {
                RectElement back = RectElement.FromCoordinates(0, 0, H8, H8);
                (back as IFillAttributes).Fill = new NoneColor(brush.BackgroundColor);
                (back as IFillAttributes).FillOpacity = new Opacity(brush.BackgroundColor);
                (back as IStrokeAttributes).Stroke = new NoneColor(Color.Empty);

                res.AddChild(back);

                for (int i = 0; i < elems.Length; i++)
                {
                    (elems[i] as IStrokeAttributes).Stroke = new NoneColor(brush.ForegroundColor);
                    (elems[i] as IStrokeAttributes).StrokeOpacity = new Opacity(brush.ForegroundColor);
                    (elems[i] as IFillAttributes).Fill = new NoneColor(Color.Empty);

                    res.AddChild(elems[i]);
                }
            }

            return res;
        }

        /// <summary>
        /// Creates the <see cref="Brush"/> instance by the inner elements.
        /// </summary>
        /// <returns>Return Brush.</returns>
        public Brush GetGDIBrush()
        {
            Bitmap buf = new Bitmap((int)Width.Value, (int)Height.Value);
            Graphics g = Graphics.FromImage(buf);
            g.TranslateTransform(-X.Value, -Y.Value);

            foreach (Element el in Children)
            {
                el.Draw(g);
            }

            WrapMode mode = WrapMode.Tile;

            if (PatternUnits == EUnits.ObjectBoundingBox)
            {
                mode = WrapMode.Clamp;
            }

            TextureBrush res = null;
            LengthRect vb = (this as IViewBoxAttribute).ViewBox;

            if (!vb.IsEmpty)
            {
                //        RectangleF rc = new RectangleF( vb.X.Value, vb.Y.Value, vb.Width.Value, vb.Height.Value );
                res = new TextureBrush(buf, mode);
            }
            else
            {
                res = new TextureBrush(buf, mode);
            }

            return res;
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
