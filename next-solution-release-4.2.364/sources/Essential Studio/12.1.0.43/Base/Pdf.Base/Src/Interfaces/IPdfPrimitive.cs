#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using Syncfusion.Pdf.IO;

namespace Syncfusion.Pdf.Primitives
{
    /// <summary>
    /// Defines the basic interace of the various Primitive..
    /// </summary>
#if NETFX_CORE || WP
    public interface IPdfPrimitive
#else
    internal interface IPdfPrimitive
#endif
    {
        /// <summary>
        /// Specfies the status of the IPdfPrmitive. Status is registered if it has a reference or else none.
        /// </summary>
        ObjectStatus Status
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this document is saving or not.
        /// </summary>
        bool IsSaving
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the integer value of the specified object.
        /// </summary>
        int ObjectCollectionIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Stores the cloned object for future use.
        /// </summary>
        IPdfPrimitive ClonedObject
        {
            get;
        }

        /// <summary>
        /// Saves the object using the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        void Save(IPdfWriter writer);

        /// <summary>
        /// Creates a deep copy of the IPdfPrimitive object.
        /// </summary>
        IPdfPrimitive Clone(PdfCrossTable crossTable);

        /// <summary>
        /// Gets or sets the position of the object.
        /// </summary>
        int Position
        {
            get;
            set;
        }
    }
}
