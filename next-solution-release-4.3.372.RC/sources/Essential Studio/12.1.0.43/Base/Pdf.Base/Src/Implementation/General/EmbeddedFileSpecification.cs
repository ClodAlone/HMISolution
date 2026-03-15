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

using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents specification of embedded file.
    /// </summary>
    public class PdfEmbeddedFileSpecification : PdfFileSpecificationBase
    {
        #region Fields
        /// <summary>
        /// Internal variable to store description.
        /// </summary>
        private string m_description = String.Empty;

        /// <summary>
        /// Embedded file instance.
        /// </summary>
        private EmbeddedFile m_embeddedFile = null;

        /// <summary>
        /// Dictionary to store file specification properties.
        /// </summary>
        private PdfDictionary m_dictionary = new PdfDictionary();

       /// <summary>
       /// PortfolioAttributes instance.
       /// </summary>
        private PdfPortfolioAttributes m_portfolioAttributes;
        #endregion

        #region Constructors
#if SILVERLIGHT
        /// <summary>
        /// Security Critical :  Initializes new <see cref="PdfEmbeddedFileSpecification"/> instance.
        /// </summary>
        [System.Security.SecurityCritical]
#else
        /// <summary>
        ///  Initializes new <see cref="PdfEmbeddedFileSpecification"/> instance.
        /// </summary>
#endif
        /// <param name="fileName">file name</param>
        public PdfEmbeddedFileSpecification(string fileName)
            : base(fileName)
        {
            m_embeddedFile = new EmbeddedFile(fileName);
            Description = fileName;
        }

#if SILVERLIGHT
        /// <summary>
        /// Security Critical : Initializes a new instance of the <see cref="PdfEmbeddedFileSpecification"/> class.
        /// </summary>
        [System.Security.SecurityCritical]
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfEmbeddedFileSpecification"/> class.
        /// </summary>
#endif        
        /// <param name="fileName">Name of the file.</param>
        /// <param name="data">The data.</param>
        public PdfEmbeddedFileSpecification(string fileName, byte[] data)
            : base(fileName)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            m_embeddedFile = new EmbeddedFile(fileName, data);
            Description = fileName;
        }

#if SILVERLIGHT
        /// <summary>
        /// Security Critical :  Initializes a new instance of the <see cref="PdfEmbeddedFileSpecification"/> class.
        /// </summary>
        [System.Security.SecurityCritical]
#else
        /// <summary>
        ///  Initializes a new instance of the <see cref="PdfEmbeddedFileSpecification"/> class.
        /// </summary>
#endif
        /// <param name="fileName">Name of the file.</param>
        /// <param name="stream">The stream.</param>
        public PdfEmbeddedFileSpecification(string fileName, Stream stream)
            : base(fileName)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            m_embeddedFile = new EmbeddedFile(fileName, stream);
            Description = fileName;
        }
        #endregion

        #region Properties
#if SILVERLIGHT
        /// <summary>
        /// Security Critical :  Gets or sets the File Name.
        /// </summary>
        //[System.Security.SecurityCritical]
#else
        /// <summary>
        ///  Gets or sets the File Name.
        /// </summary>
#endif
        /// <value></value>
        public override string FileName
        {
            get
            {
                return m_embeddedFile.FileName;
            }

            set
            {
                m_embeddedFile.FileName = value;
            }
        }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        public byte[] Data
        {
            get
            {
                return m_embeddedFile.Data;
            }

            set
            {
                m_embeddedFile.Data = value;
            }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get
            {
                return m_description;
            }

            set
            {
                if (m_description != value)
                {
                    m_description = value;
                    Dictionary.SetString(DictionaryProperties.Description, m_description);
                }
            }
        }

        /// <summary>
        /// Gets or sets the MIME type of the embedded file.
        /// </summary>
        /// <value>The MIME type of the embedded file.</value>
        public string MimeType
        {
            get
            {
                return m_embeddedFile.MimeType;
            }

            set
            {
                m_embeddedFile.MimeType = value;
            }
        }

        /// <summary>
        /// Gets or sets creation date.
        /// </summary>
        /// <value>Creation date.</value>
        public DateTime CreationDate
        {
            get
            {
                return m_embeddedFile.Params.CreationDate;
            }

            set
            {
                m_embeddedFile.Params.CreationDate = value;
            }
        }

        /// <summary>
        /// Gets or sets modification date.
        /// </summary>
        /// <value>Modification date.</value>
        public DateTime ModificationDate
        {
            get
            {
                return m_embeddedFile.Params.ModificationDate;
            }

            set
            {
                m_embeddedFile.Params.ModificationDate = value;
            }
        }

        /// <summary>
        /// Gets or sets the Portfolio attributes.
        /// </summary>
        public PdfPortfolioAttributes PortfolioAttributes
        {
            get
            {
                return m_portfolioAttributes;
            }
            set
            {
                m_portfolioAttributes = value;
                Dictionary.SetProperty(DictionaryProperties.CI, m_portfolioAttributes);
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
            Dictionary.SetProperty(DictionaryProperties.EF, m_dictionary);
        }

        /// <summary>
        /// Saves object state.
        /// </summary>
        protected override void Save()
        {
            m_dictionary[DictionaryProperties.F] = new PdfReferenceHolder(m_embeddedFile);

            //PdfName name = new PdfName(Path.GetFileName(FileName));

            PdfString str = new PdfString(FormatFileName(Path.GetFileName(FileName), false));
            Dictionary.SetProperty(DictionaryProperties.F, str);
            //Dictionary.SetProperty(DictionaryProperties.F, new PdfString(Encoding.ASCII.GetBytes(name.ToString())) );

            //byte[] bigEndian = PdfString.ToUnicodeArray(name.Value.ToString(), true);
            // Dictionary.SetProperty(DictionaryProperties.UF, new PdfString(bigEndian));

            Dictionary.SetProperty(DictionaryProperties.UF, str);
        }
        #endregion
    }
}
