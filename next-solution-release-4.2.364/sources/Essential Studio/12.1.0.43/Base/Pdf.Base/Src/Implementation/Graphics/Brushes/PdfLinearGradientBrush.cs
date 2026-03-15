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
    /// Implements linear gradient brush by using PDF axial shading pattern.
    /// </summary>
    public class PdfLinearGradientBrush : PdfGradientBrush
    {
        #region Fields
        /// <summary>
        ///  Local variable to store the point start.
        /// </summary>
        private PointF m_pointStart;

        /// <summary>
        /// Local variable to store the point end.
        /// </summary>
        private PointF m_pointEnd;

        /// <summary>
        /// Local variable to store the colours.
        /// </summary>
        private PdfColor[] m_colours;

        /// <summary>
        /// Local variable to store the colour Blend.
        /// </summary>
        private PdfColorBlend m_colourBlend;

        /// <summary>
        /// Local variable to store the blend.
        /// </summary>
        private PdfBlend m_blend;

        /// <summary>
        /// Local variable to store the boundaries.
        /// </summary>
        private RectangleF m_boundaries;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfLinearGradientBrush"/> class.
        /// </summary>
        /// <param name="point1">The starting point of the gradient.</param>
        /// <param name="point2">The end point of the gradient.</param>
        /// <param name="color1">The starting color of the gradient.</param>
        /// <param name="color2">The end color of the gradient.</param>
        public PdfLinearGradientBrush(PointF point1, PointF point2, PdfColor color1, PdfColor color2)
            : this(color1, color2)
        {
            m_pointStart = point1;
            m_pointEnd = point2;

            SetPoints(m_pointStart, m_pointEnd);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfLinearGradientBrush"/> class.
        /// </summary>
        /// <param name="rect">A RectangleF structure that specifies the bounds of the linear gradient. </param>
        /// <param name="color1">The starting color for the gradient.</param>
        /// <param name="color2">The ending color for the gradient.</param>
        /// <param name="mode">The mode.</param>
        public PdfLinearGradientBrush(RectangleF rect, PdfColor color1,
            PdfColor color2, PdfLinearGradientMode mode)
            : this(color1, color2)
        {
            m_boundaries = rect;
            //BBox = PdfArray.FromRectangle( rect );

            switch (mode)
            {
                case PdfLinearGradientMode.BackwardDiagonal:
                    m_pointStart = new PointF(rect.Right, rect.Top);
                    m_pointEnd = new PointF(rect.Left, rect.Bottom);
                    break;

                case PdfLinearGradientMode.ForwardDiagonal:
                    m_pointStart = new PointF(rect.Left, rect.Top);
                    m_pointEnd = new PointF(rect.Right, rect.Bottom);
                    break;

                case PdfLinearGradientMode.Horizontal:
                    m_pointStart = new PointF(rect.Left, rect.Top);
                    m_pointEnd = new PointF(rect.Right, rect.Top);
                    break;

                case PdfLinearGradientMode.Vertical:
                    m_pointStart = new PointF(rect.Left, rect.Top);
                    m_pointEnd = new PointF(rect.Left, rect.Bottom);
                    break;

                default:
                    throw new ArgumentException("Unsupported linear gradient mode: " + mode, "mode");
            }

            SetPoints(m_pointStart, m_pointEnd);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfLinearGradientBrush"/> class.
        /// </summary>
        /// <param name="rect">A RectangleF structure that specifies the bounds of the linear gradient.</param>
        /// <param name="color1">The starting color for the gradient.</param>
        /// <param name="color2">The ending color for the gradient.</param>
        /// <param name="angle">The angle, measured in degrees clockwise from the x-axis,
        /// of the gradient's orientation line.</param>
        public PdfLinearGradientBrush(RectangleF rect, PdfColor color1,
            PdfColor color2, float angle)
            : this(color1, color2)
        {
            m_boundaries = rect;

            angle %= 360.0f;

            if (angle == 0.0f)  // Horizontal left-to-right.
            {
                m_pointStart = new PointF(rect.Left, rect.Top);
                m_pointEnd = new PointF(rect.Right, rect.Top);
            }
            else if (angle == 90.0f) // Vertial top-to-bottom.
            {
                m_pointStart = new PointF(rect.Left, rect.Top);
                m_pointEnd = new PointF(rect.Left, rect.Bottom);
            }
            else if (angle == 180.0f) // Horizontal right-to-left.
            {
                m_pointEnd = new PointF(rect.Left, rect.Top);
                m_pointStart = new PointF(rect.Right, rect.Top);
            }
            else if (angle == 270.0f) // Vertical bottom-to-top.
            {
                m_pointEnd = new PointF(rect.Left, rect.Top);
                m_pointStart = new PointF(rect.Left, rect.Bottom);
            }
            else // General case.
            {
                double d2r = Math.PI / 180.0;
                double radAngle = angle * d2r;
                double k = Math.Tan(radAngle);

                float x = m_boundaries.Left + (m_boundaries.Right - m_boundaries.Left) / 2;
                float y = m_boundaries.Top + (m_boundaries.Bottom - m_boundaries.Top) / 2;

                PointF centre = new PointF(x, y);

                x = m_boundaries.Width / 2 * (float)Math.Cos(radAngle);
                y = (float)(k * x);

                x += centre.X;
                y += centre.Y;

                PointF p1 = new PointF(x, y);
                PointF cp1 = SubPoints(p1, centre); // P1 - P0

                PointF p = ChoosePoint(angle);
                float coef = MulPoints(SubPoints(p, centre), cp1) / MulPoints(cp1, cp1);

                m_pointStart = AddPoints(centre, MulPoint(cp1, coef)); // Parametric line equation.
                m_pointEnd = AddPoints(centre, MulPoint(cp1, -coef));
            }

            SetPoints(m_pointStart, m_pointEnd);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfLinearGradientBrush"/> class.
        /// </summary>
        /// <param name="color1">The color of the start point.</param>
        /// <param name="color2">The color of the end point.</param>
        private PdfLinearGradientBrush(PdfColor color1, PdfColor color2)
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
        /// Gets a rectangular region that defines
        /// the boundaries of the gradient.
        /// </summary>
        public RectangleF Rectangle
        {
            get
            {
                return m_boundaries;
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
        /// Adds two points to each other.
        /// </summary>
        /// <param name="point1">The point1.</param>
        /// <param name="point2">The point2.</param>
        /// <returns>The resulting point.</returns>
        private static PointF AddPoints(PointF point1, PointF point2)
        {
            float x = point1.X + point2.X;
            float y = point1.Y + point2.Y;

            PointF result = new PointF(x, y);

            return result;
        }

        /// <summary>
        /// Subs the second point from the first one.
        /// </summary>
        /// <param name="point1">The point1.</param>
        /// <param name="point2">The point2.</param>
        /// <returns>The resulting point.</returns>
        private static PointF SubPoints(PointF point1, PointF point2)
        {
            float x = point1.X - point2.X;
            float y = point1.Y - point2.Y;

            PointF result = new PointF(x, y);

            return result;
        }

        /// <summary>
        /// Makes scalar multiplication of two points.
        /// </summary>
        /// <param name="point1">The point1.</param>
        /// <param name="point2">The point2.</param>
        /// <returns>The result of multiplication.</returns>
        private static float MulPoints(PointF point1, PointF point2)
        {
            float result = point1.X * point2.X + point1.Y * point2.Y;

            return result;
        }

        /// <summary>
        /// Multiplies the point by the value specified.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="value">The value.</param>
        /// <returns>The result in point.</returns>
        private static PointF MulPoint(PointF point, float value)
        {
            point.X *= value;
            point.Y *= value;

            return point;
        }

        /// <summary>
        /// Choosts the point according to the angle.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <returns>The correct point.</returns>
        private PointF ChoosePoint(float angle)
        {
            PointF point = PointF.Empty;
            // Choose the correct point.
            if (angle < 90 && angle > 0)
            {
                point = new PointF(m_boundaries.Right, m_boundaries.Bottom);
            }
            else if (angle < 180 && angle > 90)
            {
                point = new PointF(m_boundaries.Left, m_boundaries.Bottom);
            }
            else if (angle < 270 && angle > 180)
            {
                point = new PointF(m_boundaries.Left, m_boundaries.Top);
            }
            else if (angle > 270)
            {
                point = new PointF(m_boundaries.Right, m_boundaries.Top);
            }
            else
            {
                throw new PdfException("Internal error.");
            }

            return point;
        }

        /// <summary>
        /// Sets the start and end points.
        /// </summary>
        /// <param name="point1">The point1.</param>
        /// <param name="point2">The point2.</param>
        private void SetPoints(PointF point1, PointF point2)
        {
            PdfArray points = new PdfArray();

            points.Add(new PdfNumber(point1.X));
            points.Add(new PdfNumber(PdfGraphics.UpdateY(point1.Y)));
            points.Add(new PdfNumber(point2.X));
            points.Add(new PdfNumber(PdfGraphics.UpdateY(point2.Y)));

            Shading[DictionaryProperties.Coords] = points;
        }

        /// <summary>
        /// Initializes the shading dictionary.
        /// </summary>
        private void InitShading()
        {
            //ColorSpace = PdfColorSpace.RGB;
            ColorSpace = base.ColorSpace;
            Function = m_colourBlend.GetFunction(ColorSpace);
            Shading[DictionaryProperties.ShadingType] = new PdfNumber((int)ShadingType.Axial);
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Creates a new copy of a brush.
        /// </summary>
        /// <returns>A new instance of the Brush class.</returns>
        public override PdfBrush Clone()
        {
            PdfLinearGradientBrush brush = MemberwiseClone() as PdfLinearGradientBrush;
            brush.ResetPatternDictionary(new PdfDictionary(PatternDictionary));
            brush.Shading = new PdfDictionary();
            brush.InitShading();
            brush.SetPoints(brush.m_pointStart, brush.m_pointEnd);

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
