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
    /// Implements the all SVG interfaces.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public abstract class SuperElement : Element,
        IStrokeAttributes, IFillAttributes, ITransformAttribute,
        IStyleAttribute, IFontAttributes, IViewBoxAttribute,
        IOpacityAttribute, IClipingAttribute
    {
        #region Properties

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
                SetAttribute(SVG.ATTR_STROKE, value, new NoneColor(Color.Empty));
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
                this.SetAttribute(SVG.ATTR_STROKE_WIDTH, value, new Length(1));
            }
        }

        /// <summary>
        /// Gets or sets the stroke line caps.
        /// </summary>
        /// <value>The stroke line caps.</value>
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
        /// Gets or sets the stroke line join.
        /// </summary>
        /// <value>The stroke line join.</value>
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
        /// Gets or sets the stroke miter limit.
        /// </summary>
        /// <value>The stroke miter limit.</value>
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
        /// Gets or sets the stroke dash array.
        /// </summary>
        /// <value>The stroke dash array.</value>
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
        /// Gets or sets the stroke dash offset.
        /// </summary>
        /// <value>The stroke dash offset.</value>
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
        /// Gets or sets the color to fill the background.
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

        #region IStyleAttribute Members
        /// <summary>
        /// Gets or sets the style.
        /// </summary>
        /// <value>The style.</value>
        public Style Style
        {
            get
            {
                return (Style)GetAttribute(SVG.ATTR_STYLE, null);
            }

            set
            {
                SetAttribute(SVG.ATTR_STYLE, value, null);
            }
        }
        #endregion

        #region IFontAttributes Members
        /// <summary>
        /// Gets or sets the font family.
        /// </summary>
        /// <value>The font family.</value>
        string IFontAttributes.FontFamily
        {
            get
            {
                return (string)GetAttribute(SVG.ATTR_FONT_FAMILY, "");
            }

            set
            {
                SetAttribute(SVG.ATTR_FONT_FAMILY, value, "");
            }
        }

        /// <summary>
        /// Gets or sets the font style.
        /// </summary>
        /// <value>The font style.</value>
        EFontStyle IFontAttributes.FontStyle
        {
            get
            {
                return (EFontStyle)GetAttribute(SVG.ATTR_FONT_STYLE, EFontStyle.Normal);
            }

            set
            {
                SetAttribute(SVG.ATTR_FONT_STYLE, value, EFontStyle.Normal);
            }
        }

        /// <summary>
        /// Gets or sets the font variant.
        /// </summary>
        /// <value>The font variant.</value>
        EFontVariant IFontAttributes.FontVariant
        {
            get
            {
                return (EFontVariant)GetAttribute(SVG.ATTR_FONT_VARIANT, EFontVariant.Normal);
            }

            set
            {
                SetAttribute(SVG.ATTR_FONT_VARIANT, value, EFontVariant.Normal);
            }
        }

        /// <summary>
        /// Gets or sets the font weight.
        /// </summary>
        /// <value>The font weight.</value>
        EFontWeight IFontAttributes.FontWeight
        {
            get
            {
                return (EFontWeight)GetAttribute(SVG.ATTR_FONT_WEIGHT, EFontWeight.Normal);
            }

            set
            {
                SetAttribute(SVG.ATTR_FONT_WEIGHT, value, EFontWeight.Normal);
            }
        }

        /// <summary>
        /// Gets or sets the font stretch.
        /// </summary>
        /// <value>The font stretch.</value>
        EFontStretch IFontAttributes.FontStretch
        {
            get
            {
                return (EFontStretch)GetAttribute(SVG.ATTR_FONT_STRETCH, EFontStretch.Normal);
            }

            set
            {
                SetAttribute(SVG.ATTR_FONT_STRETCH, value, EFontStretch.Normal);
            }
        }

        /// <summary>
        /// Gets or sets the size of the font.
        /// </summary>
        /// <value>The size of the font.</value>
        Length IFontAttributes.FontSize
        {
            get
            {
                return (Length)GetAttribute(SVG.ATTR_FONT_SIZE, new Length(SVG.VALUE_MEDIUM));
            }

            set
            {
                SetAttribute(SVG.ATTR_FONT_SIZE, value, new Length(SVG.VALUE_MEDIUM));
            }
        }

        /// <summary>
        /// Gets or sets the font size adjust.
        /// </summary>
        /// <value>The font size adjust.</value>
        Number IFontAttributes.FontSizeAdjust
        {
            get
            {
                return (Number)GetAttribute(SVG.ATTR_FONT_SIZE_ADJUST, new Number(SVG.VALUE_NONE));
            }

            set
            {
                SetAttribute(SVG.ATTR_FONT_SIZE_ADJUST, value, new Number(SVG.VALUE_NONE));
            }
        }

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        /// <value>The font.</value>
        SFont IFontAttributes.Font
        {
            get
            {
                return null;
            }

            set
            {
            }
        }
        #endregion

        #region IViewBoxAttribute Members
        /// <summary>
        /// Gets or sets the view box.
        /// </summary>
        /// <value>The view box.</value>
        LengthRect IViewBoxAttribute.ViewBox
        {
            get
            {
                return (LengthRect)GetAttribute(SVG.ATTR_VIEW_BOX, LengthRect.Empty);
            }

            set
            {
                SetAttribute(SVG.ATTR_VIEW_BOX, value, LengthRect.Empty);
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

        #region IClipingAttribute Members
        /// <summary>
        /// Gets or sets the clip rule.
        /// </summary>
        /// <value>The clip rule.</value>
        EClipRule IClipingAttribute.ClipRule
        {
            get
            {
                return (EClipRule)GetAttribute(SVG.ATTR_CLIP_RULE, EClipRule.Nonzero);
            }

            set
            {
                SetAttribute(SVG.ATTR_CLIP_RULE, value, EClipRule.Nonzero);
            }
        }

        /// <summary>
        /// Gets or sets the identifier of clip path.
        /// </summary>
        /// <value>The clip path.</value>
        string IClipingAttribute.ClipPath
        {
            get
            {
                return (string)GetAttribute(SVG.ATTR_CLIP_PATH, "");
            }

            set
            {
                SetAttribute(SVG.ATTR_CLIP_PATH, value, "");
            }
        }
        #endregion

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SuperElement"/> class.
        /// </summary>
        /// <param name="name">The name of element.</param>
        public SuperElement(string name)
            : base(name)
        {
        }
        #endregion

        #region Pulbic methods
        /// <summary>
        /// Draws the element and the inner elements to the specified <see cref="Graphics"/>.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/>.</param>
        public override void Draw(Graphics g)
        {
            GraphicsState state = g.Save();
            TransformList transformation = (this as ITransformAttribute).Transform;

            if (transformation != null)
            {
                Matrix mtr = g.Transform;
                mtr.Multiply(transformation.Matrix);
                g.Transform = mtr;
            }

            if ((this as IClipingAttribute).ClipPath != "")
            {
                string id = Utility.GetIdFromUrl((this as IClipingAttribute).ClipPath);
                ClipPathElement clipPath = this.OwnerDocument.FindElement(id) as ClipPathElement;
                g.SetClip(clipPath.GetResultPath());
            }

            this.DrawSelf(g);
            base.Draw(g);

            g.Restore(state);
        }

        /// <summary>
        /// Parses the XML document.
        /// </summary>
        /// <param name="node">The <see cref="XmlNode"/>.</param>
        internal override void ParseXml(XmlNode node)
        {
            base.ParseXml(node);

            if (node.Attributes != null)
            {
                foreach (XmlAttribute attr in node.Attributes)
                {
                    SetXmlAttribute(attr);
                }
            }
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Sets the XML attribute.
        /// </summary>
        /// <param name="attr">The attr.</param>
        public void SetXmlAttribute(XmlAttribute attr)
        {
            switch (attr.Name)
            {
                #region For IStrokeAttributes
                case SVG.ATTR_STROKE:
                    (this as IStrokeAttributes).Stroke = new NoneColor(attr.Value);
                    break;
                case SVG.ATTR_STROKE_DASHARRAY:
                    (this as IStrokeAttributes).StrokeDasharray = new FloatArray(attr.Value);
                    break;
                case SVG.ATTR_STROKE_DASHOFFSET:
                    (this as IStrokeAttributes).StrokeDashoffset = new Length(attr.Value);
                    break;
                case SVG.ATTR_STROKE_LINECAP:
                    (this as IStrokeAttributes).StrokeLinecap = EStrokeLinecap.Parse(attr.Value);
                    break;
                case SVG.ATTR_STROKE_LINEJOIN:
                    (this as IStrokeAttributes).StrokeLinejoin = EStrokeLinejoin.Parse(attr.Value);
                    break;
                case SVG.ATTR_STROKE_MITERLIMIT:
                    (this as IStrokeAttributes).StrokeMiterlimit = new Number(attr.Value);
                    break;
                case SVG.ATTR_STROKE_OPACITY:
                    (this as IStrokeAttributes).StrokeOpacity = new Opacity(attr.Value);
                    break;
                case SVG.ATTR_STROKE_WIDTH:
                    (this as IStrokeAttributes).StrokeWidth = new Length(attr.Value);
                    break;
                #endregion

                #region For IFillAttributes
                case SVG.ATTR_FILL:
                    (this as IFillAttributes).Fill = new NoneColor(attr.Value);
                    break;
                case SVG.ATTR_FILL_OPACITY:
                    (this as IFillAttributes).FillOpacity = new Opacity(attr.Value);
                    break;
                #endregion

                #region For ITransformAttribute
                case SVG.ATTR_TRANSFORM:
                    (this as ITransformAttribute).Transform = new TransformList(attr.Value);
                    break;
                #endregion

                #region For IStyleAttribute
                case SVG.ATTR_STYLE:
                    (this as IStyleAttribute).Style = new Style(attr.Value);
                    break;
                #endregion

                #region For IFontAttributes
                case SVG.ATTR_FONT_FAMILY:
                    (this as IFontAttributes).FontFamily = attr.Value;
                    break;
                case SVG.ATTR_FONT_SIZE:
                    (this as IFontAttributes).FontSize = Length.Parse(attr.Value);
                    break;
                case SVG.ATTR_FONT_SIZE_ADJUST:
                    (this as IFontAttributes).FontSizeAdjust = Number.Parse(attr.Value);
                    break;
                case SVG.ATTR_FONT_STRETCH:
                    (this as IFontAttributes).FontStretch = EFontStretch.Parse(attr.Value);
                    break;
                #endregion

                #region For IViewBoxAttribute
                case SVG.ATTR_VIEW_BOX:
                    (this as IViewBoxAttribute).ViewBox = new LengthRect(attr.Value);
                    break;
                #endregion

                #region For IOpacityAttribute
                case SVG.ATTR_OPACITY:
                    (this as IOpacityAttribute).Opacity = new Opacity(attr.Value);
                    break;
                #endregion

                #region For IClipingAttribute
                case SVG.ATTR_CLIP_RULE:
                    (this as IClipingAttribute).ClipRule = new EClipRule(attr.Value);
                    break;

                case SVG.ATTR_CLIP_PATH:
                    (this as IClipingAttribute).ClipPath = attr.Value;
                    break;
                #endregion
            }
        }

        /// <summary>
        /// Draws the element to specified <see cref="Graphics"/>.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/>.</param>
        protected virtual void DrawSelf(Graphics g)
        {
        }
        #endregion
    }
}
