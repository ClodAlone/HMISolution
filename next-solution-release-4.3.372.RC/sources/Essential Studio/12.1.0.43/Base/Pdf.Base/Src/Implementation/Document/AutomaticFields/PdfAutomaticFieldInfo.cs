#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Drawing;

/// <summary>
/// The Syncfusion.Pdf namespace contains classes for creating PDF document.
/// </summary>
namespace Syncfusion.Pdf
{
    /// <summary>
    /// Represents information about the automatic field.
    /// </summary>
    internal class PdfAutomaticFieldInfo
    {
        #region Fields
        /// <summary>
        /// Internal variable to store location of the field.
        /// </summary>
        private PointF m_location = PointF.Empty;

        /// <summary>
        /// Internal variable to store field.
        /// </summary>
        private PdfAutomaticField m_field = null;

        /// <summary>
        /// Internal variable to store x scaling factor.
        /// </summary>
        private float m_scalingX = 1;

        /// <summary>
        /// Internal variable to store y scaling factor.
        /// </summary>
        private float m_scalingY = 1;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfAutomaticFieldInfo"/> class.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="location">The location.</param>
        public PdfAutomaticFieldInfo(PdfAutomaticField field, PointF location)
        {
            if (field == null)
            {
                throw new ArgumentNullException("field");
            }

            m_field = field;
            m_location = location;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfAutomaticFieldInfo"/> class.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="location">The location.</param>
        /// <param name="scalingX">The scaling X.</param>
        /// <param name="scalingY">The scaling Y.</param>
        public PdfAutomaticFieldInfo(PdfAutomaticField field, PointF location, float scalingX, float scalingY)
        {
            if (field == null)
            {
                throw new ArgumentNullException("field");
            }

            m_field = field;
            m_location = location;
            m_scalingX = scalingX;
            m_scalingY = scalingY;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfAutomaticFieldInfo"/> class.
        /// </summary>
        /// <param name="fieldInfo">The field info.</param>
        public PdfAutomaticFieldInfo(PdfAutomaticFieldInfo fieldInfo)
        {
            if (fieldInfo == null)
            {
                throw new ArgumentNullException("fieldInfo");
            }

            m_field = fieldInfo.Field;
            m_location = fieldInfo.Location;
            m_scalingX = fieldInfo.ScalingX;
            m_scalingY = fieldInfo.ScalingY;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>The location.</value>
        public PointF Location
        {
            get
            {
                return m_location;
            }

            set
            {
                m_location = value;
            }
        }

        /// <summary>
        /// Gets or sets the field.
        /// </summary>
        /// <value>The field.</value>
        public PdfAutomaticField Field
        {
            get
            {
                return m_field;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Field");
                }

                m_field = value;
            }
        }

        /// <summary>
        /// Gets or sets the scaling X factor.
        /// </summary>
        /// <value>The scaling X factor.</value>
        public float ScalingX
        {
            get
            {
                return m_scalingX;
            }

            set
            {
                m_scalingX = value;
            }
        }

        /// <summary>
        /// Gets or sets the scaling Y factor.
        /// </summary>
        /// <value>The scaling Y factor.</value>
        public float ScalingY
        {
            get
            {
                return m_scalingY;
            }

            set
            {
                m_scalingY = value;
            }
        }
        #endregion
    }
}
