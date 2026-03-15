#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Text;

using Syncfusion.Pdf.IO;

namespace Syncfusion.Pdf.Primitives
{
#if NETFX_CORE || WP
    public class PdfBoolean : IPdfPrimitive
#else
        internal class PdfBoolean : IPdfPrimitive
#endif
    {
        #region Fields
        /// <summary>
        /// The value of the PDF boolean.
        /// </summary>
        private bool m_value;
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
        /// Gets or sets the value of this instance.
        /// </summary>
        public bool Value
        {
            get
            {
                return m_value;
            }
            set
            {
                m_value = value;
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
        /// Initializes a new instance of the <see cref="T:PdfBoolean"/> class.
        /// </summary>
        internal PdfBoolean()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfBoolean"/> class.
        /// </summary>
        /// <param name="value">if it is value, set to <c>true</c>.</param>
        internal PdfBoolean(bool value)
        {
            m_value = value;
        }
        #endregion

        #region Implementation
        private string BoolToStr(bool value)
        {
            return value ? "true" : "false";
        }

        /// <summary>
        /// Creates a copy of PdfBoolean.
        /// </summary>
        public IPdfPrimitive Clone(PdfCrossTable crossTable)
        {
            PdfBoolean newBoolean = new PdfBoolean(m_value);

            return newBoolean;
        }
        #endregion

        #region IPdfSavable Members
        /// <summary>
        /// Saves the object using the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void Save(IPdfWriter writer)
        {
            writer.Write(BoolToStr(m_value));
        }
        #endregion
    }
}
