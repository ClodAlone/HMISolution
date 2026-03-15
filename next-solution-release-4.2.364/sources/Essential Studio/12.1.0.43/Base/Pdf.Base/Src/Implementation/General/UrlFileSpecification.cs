#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;

using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf
{
    /// <summary>
    /// Represents Uri file specification.
    /// </summary>
    internal class UrlFileSpecification : PdfFileSpecificationBase
    {
        #region Fields
        /// <summary>
        /// Internal variable to store file name.
        /// </summary>
        private string m_fileName = String.Empty;
        #endregion

        #region Constructors
#if SILVERLIGHT
        /// <summary>
        /// Security Critical : Initializes a new instance of the <see cref="UrlFileSpecification"/> class.
        /// </summary>
        [System.Security.SecurityCritical]
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="UrlFileSpecification"/> class.
        /// </summary>
#endif
        /// <param name="fileName">File name.</param>
        public UrlFileSpecification(string fileName)
            : base(fileName)
        {
        }
        #endregion

        #region Properties
#if SILVERLIGHT
        /// <summary>
        /// Security Critical : File name.
        /// </summary>
        //[System.Security.SecurityCritical]
#else
        /// <summary>
        /// File name.
        /// </summary>
#endif
        /// <value></value>
        public override string FileName
        {
            get
            {
                return m_fileName;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("FileName");
                }

                if (value.Length == 0)
                {
                    throw new ArgumentException("FileName can't be empty");
                }

                if (m_fileName != value)
                {
                    m_fileName = value;
                }
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Initializes instance.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            Dictionary.SetProperty(DictionaryProperties.FS, new PdfName(DictionaryProperties.URL));
        }

        /// <summary>
        /// Saves object.
        /// </summary>
        protected override void Save()
        {
            Dictionary.SetProperty(DictionaryProperties.F, new PdfString(FileName));
        }
        #endregion
    }
}
