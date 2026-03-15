#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Drawing;

using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Represent radial gradient brush.
    /// </summary>
    public class PdfRadialGradientBrush : PdfGradientBrush
    {
        #region Fields
        /// <summary>
        /// Local varaible to store the point start.
        /// </summary>
        private PointF m_pointStart;

        /// <summary>
        /// Local varaible to store the point start.
        /// </summary>
        private float m_radiusStart;

        /// <summary>
        /// Local varaible to store the point End.
        /// </summary>
        private PointF m_pointEnd;

        /// <summary>
        /// Local varaible to store the radius End.
        /// </summary>
        private float m_radiusEnd;

        /// <summary>
        /// Local varaible to store the colours.
        /// </summary>
        private PdfColor[] m_colours;

        /// <summary>
        /// Local varaible to store the colour blend.
        /// </summary>
        private PdfColorBlend m_colourBlend;

        /// <summary>
        /// Local varaible to store the blend.
        /// </summary>
        private PdfBlend m_blend;

        /// <summary>
        /// Local varaible to store the boundaries.
        /// </summary>
        private RectangleF m_boundaries;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfRadialGradientBrush"/> class.
        /// </summary>
        /// <param name="centreStart">The start centre.</param>
        /// <param name="radiusStart">The start radius.</param>
        /// <param name="centreEnd">The end centre.</param>
        /// <param name="radiusEnd">The end radius.</param>
        /// <param name="colorStart">The start color.</param>
        /// <param name="colorEnd">The end color.</param>
        public PdfRadialGradientBrush(PointF centreStart, float radiusStart,
            PointF centreEnd, float radiusEnd, PdfColor colorStart, PdfColor colorEnd)
            : this(colorStart, colorEnd)
        {
            if (radiusStart < 0)
            {
                throw new ArgumentOutOfRangeException("radiusStart", "The radius can't be less then zero.");
            }

            if (radiusEnd < 0)
            {
                throw new ArgumentOutOfRangeException("radiusEnd", "The radius can't be less then zero.");
            }

            m_pointEnd = centreEnd;
            m_pointStart = centreStart;
            m_radiusStart = radiusStart;
            m_radiusEnd = radiusEnd;

            SetPoints(m_pointStart, m_pointEnd, m_radiusStart, m_radiusEnd);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfRadialGradientBrush"/> class.
        /// </summary>
        /// <param name="color1">The color1.</param>
        /// <param name="color2">The color2.</param>
        private PdfRadialGradientBrush(PdfColor color1, PdfColor color2)
            : base(new PdfDictionary())
        {
            m_colours = new PdfColor[] { color1, color2 };
            m_colourBlend = new PdfColorBlend(2);
            m_colourBlend.Positions = new float[] { 0.0f, 1.0f };
            m_colourBlend.Colors = m_colours;
            InitShading();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a PdfBlend that specifies positions
        /// and factors that define a custom falloff for the gradient.
        /// </summary>
        public PdfBlend Blend
        {
            get
            {
                return m_blend;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Blend");
                }

                if (m_colours == null)
                {
                    throw new NotSupportedException("There is no starting and ending colours specified.");
                }

                m_blend = value;
                // TODO: generate correct colour blend.
                m_colourBlend = m_blend.GenerateColorBlend(m_colours, ColorSpace);
                ResetFunction();
            }
        }

        /// <summary>
        /// Gets or sets a ColorBlend that defines a multicolor linear gradient.
        /// </summary>
        public PdfColorBlend InterpolationColors
        {
            get
            {
                return m_colourBlend;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("InterpolationColors");
                }

                m_blend = null;
                m_colours = null;
                m_colourBlend = value;
                ResetFunction();
            }
        }

        /// <summary>
        /// Gets or sets the starting and ending colors of the gradient.
        /// </summary>
        public PdfColor[] LinearColors
        {
            get
            {
                return m_colours;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("LinearColors");
                }

                if (value.Length < 2)
                {
                    throw new ArgumentException("The array is too small", "LinearColors");
                }

                if (m_colours == null)
                {
                    m_colours = new PdfColor[] { value[0], value[1] };
                }
                else
                {
                    m_colours[0] = value[0];
                    m_colours[1] = value[1];
                }

                if (m_blend == null)
                {
                    // Set correct colour blend.
                    m_colourBlend = new PdfColorBlend(2);
                    m_colourBlend.Colors = m_colours;
                    m_colourBlend.Positions = new float[] { 0.0f, 1.0f };
                }
                else
                {
                    m_colourBlend = m_blend.GenerateColorBlend(m_colours, ColorSpace);
                }

                ResetFunction();
            }
        }

        /// <summary>
        /// Gets or sets the rectangle.
        /// </summary>
        /// <value>The rectangle.</value>
        public RectangleF Rectangle
        {
            get
            {
                return m_boundaries;
            }

            set
            {
                m_boundaries = value;
                BBox = PdfArray.FromRectangle(value);
            }
        }

        /// <summary>
        /// Gets or sets the value indicating whether the gradient
        /// should extend starting and ending points.
        /// </summary>
        public PdfExtend Extend
        {
            get
            {
                PdfExtend result = PdfExtend.None;
                PdfArray extend = Shading[DictionaryProperties.Extend] as PdfArray;

                if (extend != null)
                {
                    PdfBoolean extStart = extend[0] as PdfBoolean;
                    PdfBoolean extEnd = extend[1] as PdfBoolean;

                    if (extStart.Value)
                    {
                        result |= PdfExtend.Start;
                    }

                    if (extEnd.Value)
                    {
                        result |= PdfExtend.End;
                    }
                }

                return result;
            }

            set
            {
                PdfArray extend = Shading[DictionaryProperties.Extend] as PdfArray;
                PdfBoolean extStart;
                PdfBoolean extEnd;

                if (extend == null)
                {
                    extStart = new PdfBoolean(false);
                    extEnd = new PdfBoolean(false);
                    extend = new PdfArray();
                    extend.Add(extStart);
                    extend.Add(extEnd);
                    Shading[DictionaryProperties.Extend] = extend;
                }
                else
                {
                    extStart = extend[0] as PdfBoolean;
                    extEnd = extend[1] as PdfBoolean;
                }

                extStart.Value = ((value & PdfExtend.Start) > 0);
                extEnd.Value = ((value & PdfExtend.End) > 0);
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Sets the points.
        /// </summary>
        /// <param name="pointStart">The point start.</param>
        /// <param name="pointEnd">The point end.</param>
        /// <param name="radiusStart">The radius start.</param>
        /// <param name="radiusEnd">The radius end.</param>
        private void SetPoints(PointF pointStart, PointF pointEnd, float radiusStart, float radiusEnd)
        {
            PdfArray points = new PdfArray();

            points.Add(new PdfNumber(pointStart.X));
            points.Add(new PdfNumber(PdfGraphics.UpdateY(pointStart.Y)));
            points.Add(new PdfNumber(radiusStart));
            points.Add(new PdfNumber(pointEnd.X));
            points.Add(new PdfNumber(PdfGraphics.UpdateY(pointEnd.Y)));
            if (radiusStart != radiusEnd)
                points.Add(new PdfNumber(radiusEnd));
            else
                points.Add(new PdfNumber(0));

            Shading[DictionaryProperties.Coords] = points;
        }

        /// <summary>
        /// Initializess the shading dictionary.
        /// </summary>
        private void InitShading()
        {
            //ColorSpace = PdfColorSpace.RGB;
            ColorSpace = base.ColorSpace;
            Function = m_colourBlend.GetFunction(ColorSpace);
            Shading[DictionaryProperties.ShadingType] = new PdfNumber((int)ShadingType.Radial);
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Creates a new copy of a brush.
        /// </summary>
        /// <returns>A new instance of the Brush class.</returns>
        public override PdfBrush Clone()
        {
            PdfRadialGradientBrush brush = MemberwiseClone() as PdfRadialGradientBrush;

            brush.ResetPatternDictionary(new PdfDictionary(PatternDictionary));
            brush.Shading = new PdfDictionary();
            brush.InitShading();
            brush.SetPoints(m_pointStart, m_pointEnd, m_radiusStart, m_radiusEnd);

            if (Matrix != null)
            {
                brush.Matrix = Matrix.Clone();
            }

            if (m_colours != null)
            {
                brush.m_colours = m_colours.Clone() as PdfColor[];
            }

            if (Blend != null)
            {
                brush.Blend = Blend.Clone();
            }
            else if (InterpolationColors != null)
            {
                brush.InterpolationColors = InterpolationColors.Clone();
            }

            brush.Extend = Extend;
            CloneBackgroundValue(brush);
            CloneAntiAliasingValue(brush);

            return brush;
        }

        /// <summary>
        /// Resets the function.
        /// </summary>
        internal override void ResetFunction()
        {
            Function = m_colourBlend.GetFunction(ColorSpace);
        }
        #endregion
    }
}
