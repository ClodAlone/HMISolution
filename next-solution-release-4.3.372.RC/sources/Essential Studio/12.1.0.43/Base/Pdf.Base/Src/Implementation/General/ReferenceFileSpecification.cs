#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.IO;
using System.Text;

using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf
{
    /// <summary>
    /// Represents specification of the references file in pdf document.
    /// </summary>
    internal class ReferenceFileSpecification : PdfFileSpecificationBase
    {
        #region Fields
        /// <summary>
        /// Internal variable to store file name.
        /// </summary>
        private string m_fileName = string.Empty;
        private PdfFilePathType m_path;
        #endregion

        #region Constructors
#if SILVERLIGHT
        /// <summary>
        /// Security Critical : Initializes a new instance of the <see cref="ReferenceFileSpecification"/> class.
        /// </summary>
        [System.Security.SecurityCritical]
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceFileSpecification"/> class.
        /// </summary>
#endif
        /// <param name="fileName">File name.</param>
        /// <param name="path">Path Type.</param>
        public ReferenceFileSpecification(string fileName, PdfFilePathType path)
            : base(fileName)
        {
            m_path = path;
            FileName = fileName;
        }
       
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceFileSpecification"/> class.
        /// </summary>
        /// <param name="fileName"></param>
        internal ReferenceFileSpecification(string fileName)
            : base(fileName)
        {
            m_fileName = fileName;
        }
        #endregion

        #region Properties
#if SILVERLIGHT
        /// <summary>
        /// Security Critical : Gets or sets the name of the file.
        /// </summary>
        //[System.Security.SecurityCritical]
#else
        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
#endif
        /// <value>The name of the file.</value>
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

#if NETFX_CORE || WP
                m_fileName = value;
#else
                if (m_path == PdfFilePathType.Absolute)
                  m_fileName = Path.GetFullPath(value);
                else if (m_path == PdfFilePathType.Relative)
                   m_fileName = value;
#endif
            }
        }    
        #endregion

        #region Implementation
        /// <summary>
        /// Saves object.
        /// </summary>
        protected override void Save()
        {
            bool flag = (this.m_path == PdfFilePathType.Relative) ? true : false;
            string formattedFileName = FormatFileName(FileName,flag);
            Dictionary.SetProperty(DictionaryProperties.UF, new PdfString(formattedFileName));
        }
        #endregion
    }
}
