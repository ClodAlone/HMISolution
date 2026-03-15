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


#if !SILVERLIGHT && !NETFX_CORE && !WP

#region file using directives
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Xml.Serialization;
using Syncfusion.Pdf.Primitives;
#endregion

namespace Syncfusion.Pdf.Graphics
{
    /// <property name="flag" value="Finished" />
    ///
    /// <summary>
    /// Class for representing Root transformation matrix.
    /// </summary>
    internal class PdfTransformationMatrix : ICloneable
    {
#region Constants
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Value for angle converting.
        /// </summary>
        private const double DegRadFactor = Math.PI / 180.0;
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Value for angle converting.
        /// </summary>
        private const double RadDegFactor = 180.0 / Math.PI;
        #endregion

#region Fields
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Transformation matrix.
        /// </summary>
        private Matrix m_matrix;
        #endregion

#region Properties
        /// <summary>
        /// Gets the X translation value.
        /// </summary>
        public float OffsetX
        {
            get
            {
                return m_matrix.OffsetX;
            }
        }

        /// <summary>
        /// Gets the Y translation value.
        /// </summary>
        public float OffsetY
        {
            get
            {
                return m_matrix.OffsetY;
            }
        }

        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Gets or sets the internal matrix object.
        /// </summary>
        protected internal Matrix Matrix
        {
            get
            {
                return m_matrix;
            }
            set
            {
                if (m_matrix != value)
                {
                    m_matrix = value;
                }
            }
        }
        #endregion

#region Constructors
        /// <summary>
        /// Initializes object.
        /// </summary>
        /// <property name="flag" value="Finished"/>
        public PdfTransformationMatrix()
        {
            m_matrix = new Matrix(1, 0, 0, 1, 0, 0);
        }

        /// <summary>
        /// Initializes object.
        /// </summary>
        /// <property name="flag" value="Finished"/>
        internal PdfTransformationMatrix(bool value)
        {
            m_matrix = new Matrix(1, 0, 0, -1, 0, 0);
        }
        #endregion

#region Public methods
        /// <summary>
        /// Translates coordinates by specified coordinates.
        /// </summary>
        /// <param name="offsets">Offsets for translation.</param>
        /// <property name="flag" value="Finished"/>
        /// <remarks>
        /// Order of transformation sequence is significant.
        /// </remarks>
        public void Translate(SizeF offsets)
        {
            Translate(offsets.Width, offsets.Height);
        }

        /// <summary>
        /// Translates coordinates by specified coordinates.
        /// </summary>
        /// <param name="offsetX">The X value by which to translate
        /// coordinate system.</param>
        /// <param name="offsetY">The Y value by which to translate
        /// coordinate system.</param>
        /// <property name="flag" value="Finished"/>
        /// <remarks>
        /// Order of transformation sequence is significant.
        /// </remarks>
        public void Translate(float offsetX, float offsetY)
        {
            m_matrix.Translate(offsetX, offsetY);
        }

        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Scales coordinates by specified coordinates.
        /// </summary>
        /// <param name="scales">Scaling values.</param>
        /// <remarks>
        /// Order of transformation sequence is significant.
        /// </remarks>
        public void Scale(SizeF scales)
        {
            Scale(scales.Width, scales.Height);
        }

        /// <summary>
        /// Scales coordinates by specified coordinates.
        /// </summary>
        /// <param name="scaleX">The value by which to scale coordinate
        /// system in the X axis direction.</param>
        /// <param name="scaleY">The value by which to scale coordinate
        /// system in the Y axis direction.</param>
        /// <property name="flag" value="Finished"/>
        /// <remarks>
        /// Order of transformation sequence is significant.
        /// </remarks>
        public void Scale(float scaleX, float scaleY)
        {
            m_matrix.Scale(scaleX, scaleY);
        }

        /// <summary>
        /// Rotates coordinate system in counterclockwise direction.
        /// </summary>
        /// <param name="angle">The angle of the rotation (in degrees).</param>
        /// <property name="flag" value="Finished"/>
        /// <remarks>
        /// Order of transformation sequence is significant.
        /// </remarks>
        public void Rotate(float angle)
        {
            m_matrix.Rotate(angle);
        }

        /// <summary>
        /// Skews coordinate system axes.
        /// </summary>
        /// <param name="angles">Skew angles.</param>
        /// <property name="flag" value="Finished"/>
        /// <remarks>
        /// Order of transformation sequence is significant.
        /// </remarks>
        public void Skew(SizeF angles)
        {
            Skew(angles.Width, angles.Height);
        }

        /// <summary>
        /// Skews coordinate system axes.
        /// </summary>
        /// <param name="angleX">Skews the X axis by this angle (in
        /// degrees).</param>
        /// <param name="angleY">Skews the Y axis by this angle (in
        /// degrees).</param>
        /// <property name="flag" value="Finished"/>
        /// <remarks>
        /// Order of transformation sequence is significant.
        /// </remarks>
        public void Skew(float angleX, float angleY)
        {
            float tanA = (float)Math.Tan(DegressToRadians(angleX));
            float tanB = (float)Math.Tan(DegressToRadians(angleY));
            Matrix skew = new Matrix(1, tanA, tanB, 1, 0, 0);

            m_matrix.Multiply(skew);
        }

        /// <summary>
        /// Applies the specified shear vector to this Matrix
        /// by prepending the shear transformation.
        /// </summary>
        /// <param name="shearX">The shear X factor.</param>
        /// <param name="shearY">The shear Y factor.</param>
        /// <remarks>The transformation applied in this method
        /// is a pure shear only if one of the parameters is 0.
        /// Applied to a rectangle at the origin, when the shearY
        /// factor is 0, the transformation moves the bottom edge
        /// horizontally by shearX times the height of the rectangle.
        /// When the shearX factor is 0, it moves the right edge
        /// vertically by shearY times the width of the rectangle.
        /// Caution is in order when both parameters are nonzero,
        /// because the results are hard to predict. For example,
        /// if both factors are 1, the transformation is singular
        /// (hence noninvertible), squeezing the entire plane to
        /// a single line.</remarks>
        public void Shear(float shearX, float shearY)
        {
            m_matrix.Shear(shearX, shearY);
        }

        /// <summary>
        /// Applies a clockwise rotation about the specified point.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <param name="point">The point.</param>
        public void RotateAt(float angle, PointF point)
        {
            m_matrix.RotateAt(angle, point);
        }
        #endregion

#region Overrides
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Gets PDF representation.
        /// </summary>
        /// <returns>
        /// PDF representation.
        /// </returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            char whitespace = ' ';

            for (int i = 0, len = m_matrix.Elements.Length; i < len; i++)
            {
                builder.Append(PdfNumber.FloatToString(m_matrix.Elements[i]));
                builder.Append(whitespace);
            }

            return builder.ToString();
        }
        #endregion

#region Implementation
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Multiplies matrixes (changes coordinate system.)
        /// </summary>
        /// <param name="matrix">Matrix to be multiplied.</param>
        protected internal void Multiply(PdfTransformationMatrix matrix)
        {
            m_matrix.Multiply(matrix.Matrix);
        }
        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="degreesX">The degrees X.</param>
        /// <returns>The value in radians.</returns>
        /// <property name="flag" value="Finished"/>
        public static double DegressToRadians(float degreesX)
        {
            return DegRadFactor * degreesX;
        }

        /// <summary>
        /// Converts radians to degress.
        /// </summary>
        /// <param name="radians">The radians.</param>
        /// <returns>The value in degress.</returns>
        /// <property name="flag" value="Finished"/>
        public static double RadiansToDegress(float radians)
        {
            return RadDegFactor * radians;
        }
        #endregion

#region ICloneable Members
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>The cloned instance.</returns>
        internal PdfTransformationMatrix Clone()
        {
            PdfTransformationMatrix m = MemberwiseClone() as PdfTransformationMatrix;

            m.m_matrix = m_matrix.Clone();

            return m;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        object ICloneable.Clone()
        {
            return Clone();
        }
        #endregion
    }
}
#else
using System;
using System.Drawing;
using System.Text;
using System.Xml.Serialization;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Class for representing Root transformation matrix.
    /// </summary>
    internal class PdfTransformationMatrix : ICloneable
    {
        #region Constants
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Value for angle converting.
        /// </summary>
        private const double DegRadFactor = Math.PI / 180.0;
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Value for angle converting.
        /// </summary>
        private const double RadDegFactor = 180.0 / Math.PI;
        #endregion

        #region Fields
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Transformation matrix.
        /// </summary>
        private Matrix m_matrix;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the X translation value.
        /// </summary>
        public float OffsetX
        {
            get
            {
                return (float)m_matrix.OffsetX;
            }
        }

        /// <summary>
        /// Gets the Y translation value.
        /// </summary>
        public float OffsetY
        {
            get
            {
                return (float)m_matrix.OffsetY;
            }
        }

        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Gets or sets the internal matrix object.
        /// </summary>
        protected internal Matrix Matrix
        {
            get
            {
                return m_matrix;
            }
            set
            {
                if (m_matrix != value)
                {
                    m_matrix = value;
                }
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes object.
        /// </summary>
        /// <property name="flag" value="Finished"/>
        public PdfTransformationMatrix()
        {
            m_matrix = new Matrix(1, 0, 0, 1, 0, 0);
        }
        /// <summary>
        /// Initializes object.
        /// </summary>
        /// <property name="flag" value="Finished"/>
        internal PdfTransformationMatrix(bool value)
        {
            m_matrix = new Matrix(1, 0, 0, -1, 0, 0);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Translates coordinates by specified coordinates.
        /// </summary>
        /// <param name="offsets">Offsets for translation.</param>
        /// <property name="flag" value="Finished"/>
        /// <remarks>
        /// Order of transformation sequence is significant.
        /// </remarks>
        public void Translate(SizeF offsets)
        {
            Translate(offsets.Width, offsets.Height);
        }

        /// <summary>
        /// Translates coordinates by specified coordinates.
        /// </summary>
        /// <param name="offsetX">The X value by which to translate
        /// coordinate system.</param>
        /// <param name="offsetY">The Y value by which to translate
        /// coordinate system.</param>
        /// <property name="flag" value="Finished"/>
        /// <remarks>
        /// Order of transformation sequence is significant.
        /// </remarks>
        public void Translate(float offsetX, float offsetY)
        {
            m_matrix.Translate(offsetX, offsetY);
        }

        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Scales coordinates by specified coordinates.
        /// </summary>
        /// <param name="scales">Scaling values.</param>
        /// <remarks>
        /// Order of transformation sequence is significant.
        /// </remarks>
        public void Scale(SizeF scales)
        {
            Scale(scales.Width, scales.Height);
        }

        /// <summary>
        /// Scales coordinates by specified coordinates.
        /// </summary>
        /// <param name="scaleX">The value by which to scale coordinate
        /// system in the X axis direction.</param>
        /// <param name="scaleY">The value by which to scale coordinate
        /// system in the Y axis direction.</param>
        /// <property name="flag" value="Finished"/>
        /// <remarks>
        /// Order of transformation sequence is significant.
        /// </remarks>
        public void Scale(float scaleX, float scaleY)
        {
            m_matrix.Elements[0] = scaleX;
            m_matrix.Elements[3] = scaleY;
        }

        /// <summary>
        /// Rotates coordinate system in counterclockwise direction.
        /// </summary>
        /// <param name="angle">The angle of the rotation (in degrees).</param>
        /// <property name="flag" value="Finished"/>
        /// <remarks>
        /// Order of transformation sequence is significant.
        /// </remarks>
        public void Rotate(float angle)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Skews coordinate system axes.
        /// </summary>
        /// <param name="angles">Skew angles.</param>
        /// <property name="flag" value="Finished"/>
        /// <remarks>
        /// Order of transformation sequence is significant.
        /// </remarks>
        public void Skew(SizeF angles)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Skews coordinate system axes.
        /// </summary>
        /// <param name="angleX">Skews the X axis by this angle (in
        /// degrees).</param>
        /// <param name="angleY">Skews the Y axis by this angle (in
        /// degrees).</param>
        /// <property name="flag" value="Finished"/>
        /// <remarks>
        /// Order of transformation sequence is significant.
        /// </remarks>
        public void Skew(float angleX, float angleY)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Applies the specified shear vector to this Matrix
        /// by prepending the shear transformation.
        /// </summary>
        /// <param name="shearX">The shear X factor.</param>
        /// <param name="shearY">The shear Y factor.</param>
        /// <remarks>The transformation applied in this method
        /// is a pure shear only if one of the parameters is 0.
        /// Applied to a rectangle at the origin, when the shearY
        /// factor is 0, the transformation moves the bottom edge
        /// horizontally by shearX times the height of the rectangle.
        /// When the shearX factor is 0, it moves the right edge
        /// vertically by shearY times the width of the rectangle.
        /// Caution is in order when both parameters are nonzero,
        /// because the results are hard to predict. For example,
        /// if both factors are 1, the transformation is singular
        /// (hence noninvertible), squeezing the entire plane to
        /// a single line.</remarks>
        public void Shear(float shearX, float shearY)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Applies a clockwise rotation about the specified point.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <param name="point">The point.</param>
        public void RotateAt(float angle, PointF point)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Overrides
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Gets PDF representation.
        /// </summary>
        /// <returns>
        /// PDF representation.
        /// </returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            char whitespace = ' ';

            for (int i = 0, len = m_matrix.Elements.Length; i < len; i++)
            {
                builder.Append(PdfNumber.FloatToString(m_matrix.Elements[i]));
                builder.Append(whitespace);
            }

            return builder.ToString();
        }

        #endregion

        #region Implementation
        /// <property name="flag" value="Finished" />
        ///
        /// <summary>
        /// Multiplies matrixes (changes coordinate system.)
        /// </summary>
        /// <param name="matrix">Matrix to be multiplied.</param>
        protected internal void Multiply(PdfTransformationMatrix matrix)
        {

        }
        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="degreesX">The degrees X.</param>
        /// <returns>The value in radians.</returns>
        /// <property name="flag" value="Finished"/>
        public static double DegressToRadians(float degreesX)
        {
            return DegRadFactor * degreesX;
        }

        /// <summary>
        /// Converts radians to degress.
        /// </summary>
        /// <param name="radians">The radians.</param>
        /// <returns>The value in degress.</returns>
        /// <property name="flag" value="Finished"/>
        public static double RadiansToDegress(float radians)
        {
            return RadDegFactor * radians;
        }
        #endregion

        #region ICloneable Members
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>The cloned instance.</returns>
        internal PdfTransformationMatrix Clone()
        {
            PdfTransformationMatrix matrix = new PdfTransformationMatrix();
            matrix.Matrix = m_matrix;
            return matrix;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        object ICloneable.Clone()
        {
            return Clone();
        }
        #endregion
    }

    internal class Matrix : ICloneable, IDisposable
    {
        #region Fields
        private float[] m_elements;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix"/> class.
        /// </summary>
        public Matrix()
        {
            m_elements = new float[6];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix"/> class.
        /// </summary>
        /// <param name="m11">The M11.</param>
        /// <param name="m12">The M12.</param>
        /// <param name="m21">The M21.</param>
        /// <param name="m22">The M22.</param>
        /// <param name="dx">The dx.</param>
        /// <param name="dy">The dy.</param>
        public Matrix(float m11, float m12, float m21, float m22, float dx, float dy)
            : this()
        {
            m_elements[0] = m11;
            m_elements[1] = m12;
            m_elements[2] = m21;
            m_elements[3] = m22;
            m_elements[4] = dx;
            m_elements[5] = dy;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix"/> class.
        /// </summary>
        /// <param name="elements">The elements.</param>
        public Matrix(float[] elements)
        {
            m_elements = elements;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the elements.
        /// </summary>
        /// <value>The elements.</value>
        public float[] Elements
        {
            get
            {
                return m_elements;
            }
        }

        /// <summary>
        /// Gets the off set X.
        /// </summary>
        /// <value>The off set X.</value>
        public float OffsetX
        {
            get
            {
                return m_elements[4];
            }
        }

        /// <summary>
        /// Gets the off set Y.
        /// </summary>
        /// <value>The off set Y.</value>
        public float OffsetY
        {
            get
            {
                return m_elements[5];
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Translates the specified offset X.
        /// </summary>
        /// <param name="offsetX">The offset X.</param>
        /// <param name="offsetY">The offset Y.</param>
        public void Translate(float offsetX, float offsetY)
        {
            m_elements[4] = offsetX;
            m_elements[5] = offsetY;
        }

        public void Multiply(Matrix matrix)
        {

        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            m_elements = null;
        }

        #endregion

        #region ICloneable Members

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            Matrix m = new Matrix(m_elements);
            return m;
        }

        #endregion
    }
}
#endif