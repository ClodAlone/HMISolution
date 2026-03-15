#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.IO;
using Syncfusion.Pdf.IO;

namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents attachments of the Pdf document.
    /// </summary>
    public class PdfAttachment : PdfEmbeddedFileSpecification
    {
        #region Constructors
#if SILVERLIGHT
        /// <summary>
        /// Security Critical : Initializes a new instance of the <see cref="PdfAttachment"/> class.
        /// </summary>
        [System.Security.SecurityCritical]
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfAttachment"/> class.
        /// </summary>
#endif
        /// <param name="fileName">Name of the file.</param>
        public PdfAttachment(string fileName)
            : base(fileName)
        {
        }

#if SILVERLIGHT
        /// <summary>
        /// Security Critical : Initializes a new instance of the <see cref="PdfAttachment"/> class.
        /// </summary>
        [System.Security.SecurityCritical]
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfAttachment"/> class.
        /// </summary>
#endif      
        /// <param name="fileName">Name of the file.</param>
        /// <param name="data">The data to be attached as a file.</param>
        public PdfAttachment(string fileName, byte[] data)
            : base(fileName, data)
        {
        }

#if SILVERLIGHT
        /// <summary>
        /// Security Critical :  Initializes a new instance of the <see cref="PdfAttachment"/> class.
        /// </summary>
        [System.Security.SecurityCritical]
#else
        /// <summary>
        ///  Initializes a new instance of the <see cref="PdfAttachment"/> class.
        /// </summary>
#endif
        /// <param name="fileName">Name of the file.</param>
        /// <param name="stream">The stream.</param>
        public PdfAttachment(string fileName, Stream stream)
            : base(fileName, stream)
        {
        }
        #endregion
    }
}
