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
    /// <summary>
    /// Represent the PDF null object.
    /// </summary>
    internal class PdfNull : IPdfPrimitive
    {
        #region Fields
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
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfNull"/> class.
        /// </summary>
        public PdfNull()
        {
        }
        #endregion

        #region IPDFSaveable Members

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

        /// <summary>
        /// Saves the object.
        /// </summary>
        /// <param name="writer">PDF writer.</param>
        /// <property name="flag" value="Finished"/>
        public void Save(IPdfWriter writer)
        {
            writer.Write("null");
        }

        /// <summary>
        /// Creates a copy of PdfNull.
        /// </summary>
        public IPdfPrimitive Clone(PdfCrossTable crossTable)
        {
            return new PdfNull();
        }
        #endregion
    }
}
