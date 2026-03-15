#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Globalization;
using System.Text;
using Syncfusion.Pdf.IO;

namespace Syncfusion.Pdf.Primitives
{
#if NETFX_CORE || WP
    public class PdfNumber : IPdfPrimitive
#else
    internal class PdfNumber : IPdfPrimitive
#endif
    {
        #region Fields
        private int m_intValue;
        private float m_floatValue;
        private bool m_isInteger;
        /// <summary>
        /// Shows the type of object status whether it is object registered or other status;
        /// </summary>
        private ObjectStatus m_status;
        /// <summary>
        /// Indicates if the object is currently in saving state or not.
        /// </summary>
        private bool m_isSaving;
        /// <summary>
        /// Holds the index number of the object.
        /// </summary>
        private int m_index;

        /// <summary>
        /// Internal variable to store the position.
        /// </summary>
        private int m_position = -1;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the integer value.
        /// </summary>
        public int IntValue
        {
            get
            {
                return m_intValue;
            }
            set
            {
                m_isInteger = true;
                m_intValue = value;
                m_floatValue = (float)value;
            }
        }

        /// <summary>
        /// Gets or sets the float value.
        /// </summary>
        public float FloatValue
        {
            get
            {
                return m_floatValue;
            }
            set
            {
                m_isInteger = false;
                m_floatValue = value;
                m_intValue = (int)value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is integer.
        /// </summary>
        public bool IsInteger
        {
            get
            {
                return m_isInteger;
            }
            set
            {
                m_isInteger = value;
            }
        }

        /// <summary>
        /// Gets or sets the Status of the specified object.
        /// </summary>
        public ObjectStatus Status
        {
            get
            {
                return m_status;
            }
            set
            {
                m_status = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this document is saving or not.
        /// </summary>
        public bool IsSaving
        {
            get
            {
                return m_isSaving;
            }
            set
            {
                m_isSaving = value;
            }
        }

        /// <summary>
        /// Gets or sets the integer value of the specified object.
        /// </summary>
        public int ObjectCollectionIndex
        {
            get
            {
                return m_index;
            }
            set
            {
                m_index = value;
            }
        }

        /// <summary>
        /// Gets or sets the position of the object.
        /// </summary>
        public int Position
        {
            get
            {
                return m_position;
            }
            set
            {
                m_position = value;
            }
        }

        /// <summary>
        /// Returns cloned object.
        /// </summary>
        public IPdfPrimitive ClonedObject
        {
            get
            {
                return null;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfNumber"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        internal PdfNumber(int value)
        {
            IntValue = value;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfNumber"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        internal PdfNumber(long value)
        {
            IntValue = (int)value;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfNumber"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        internal PdfNumber(float value)
        {
            FloatValue = value;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfNumber"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        internal PdfNumber(double value)
        {
            FloatValue = (float)value;
        }
        #endregion

        #region Static methods
        /// <summary>
        /// Converts a float value to a string using Adobe PDF rules.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns></returns>
        static public string FloatToString(float number)
        {
            return number.ToString("######################.00######", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Determines the minimum of the three values.
        /// </summary>
        /// <param name="x">The 1st value.</param>
        /// <param name="y">The 2nd value.</param>
        /// <param name="z">The 3d value.</param>
        /// <returns>The min value.</returns>
        static public float Min(float x, float y, float z)
        {
            float r = Math.Min(x, y);
            return Math.Min(z, r);
        }

        /// <summary>
        /// Determines the maximum of the three values.
        /// </summary>
        /// <param name="x">The 1st value.</param>
        /// <param name="y">The 2nd value.</param>
        /// <param name="z">The 3d value.</param>
        /// <returns>The max value.</returns>
        static public float Max(float x, float y, float z)
        {
            float r = Math.Max(x, y);
            return Math.Max(z, r);
        }
        #endregion

        #region IPDFSaveable Members
        /// <summary>
        /// Saves the object.
        /// </summary>
        /// <param name="writer">PDF writer.</param>
        public void Save(IPdfWriter writer)
        {
            if (IsInteger)
            {
                writer.Write(IntValue.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                writer.Write(FloatToString(FloatValue));
            }
        }

        /// <summary>
        /// Creates a copy of PdfNumber.
        /// </summary>
        public IPdfPrimitive Clone(PdfCrossTable crossTable)
        {
            PdfNumber newNumber = null;

            if (IsInteger)
                newNumber = new PdfNumber(IntValue);
            else
                newNumber = new PdfNumber(FloatValue);

            return newNumber;
        }
        #endregion
    }
}
