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
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents base class for file specification objects.
    /// </summary>
    public abstract class PdfFileSpecificationBase : IPdfWrapper
    {
        #region Fields
        /// <summary>
        /// Internal variable to store dictionary.
        /// </summary>
        private PdfDictionary m_dictionary = new PdfDictionary();
        #endregion

        #region Constructors
#if SILVERLIGHT
        /// <summary>
        /// Security Critical : Initializes a new instance of the <see cref="PdfFileSpecificationBase"/> class.
        /// </summary>
        [System.Security.SecurityCritical]
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfFileSpecificationBase"/> class.
        /// </summary>
#endif
        /// <param name="fileName">Name of the file.</param>
        public PdfFileSpecificationBase(string fileName)
            : base()
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            Initialize();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public abstract string FileName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the dictionary.
        /// </summary>
        /// <value>The dictionary.</value>
        internal PdfDictionary Dictionary
        {
            get
            {
                return m_dictionary;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Initializes instance.
        /// </summary>
        protected virtual void Initialize()
        {
            m_dictionary.SetProperty(DictionaryProperties.Type, new PdfName(DictionaryProperties.Filespec));
            m_dictionary.BeginSave += new SavePdfPrimitiveEventHandler(Dictionary_BeginSave);
        }

        /// <summary>
        /// Saves an instance.
        /// </summary>
        protected abstract void Save();

        /// <summary>
        /// Formats file name to Unix format.
        /// </summary>
        /// <param name="fileName">File name to format.</param>
        /// <param name="flag">bool value which represents the file path type.</param>
        /// <returns>Formatted file name.</returns>
        protected string FormatFileName(string fileName, bool flag)
        {
            const string OldSlash = "\\";
            const string NewSlash = "/";
            const string DriveDelimiter = ":";

            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            if (fileName.Length == 0)
            {
                throw new ArgumentException("fileName - string can not be empty");
            }

            string formated = fileName.Replace(OldSlash, NewSlash);

            formated = formated.Replace(DriveDelimiter, string.Empty);

            if (formated.Substring(0, 2) == OldSlash)
            {
                formated = formated.Remove(1, 1);
            }

            if (formated.Substring(0, 1) != NewSlash && flag == false)
            {
                formated = NewSlash + formated;
            }

            return formated;
        }

        /// <summary>
        /// Handles the BeginSave event of the m_dictionary control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="ars">The <see cref="Syncfusion.Pdf.Primitives.SavePdfPrimitiveEventArgs"/>
        /// instance containing the event data.</param>
        private void Dictionary_BeginSave(object sender, SavePdfPrimitiveEventArgs ars)
        {
            Save();
        }
        #endregion

        #region IPdfWrapper Members
        /// <summary>
        /// Gets the element.
        /// </summary>
        /// <value></value>
        IPdfPrimitive IPdfWrapper.Element
        {
            get
            {
                return m_dictionary;
            }
        }
        #endregion
    }
}
