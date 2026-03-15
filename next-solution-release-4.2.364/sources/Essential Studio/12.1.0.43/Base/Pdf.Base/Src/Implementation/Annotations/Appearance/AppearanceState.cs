#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.Interactive namespace contains classes used to create interactive elements.
/// </summary>
namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Represents the states of an annotation's appearance.
    /// </summary>
    public class PdfAppearanceState : IPdfWrapper
    {
        #region Fields
        /// <summary>
        /// Internal variable to store template for active (On) state.
        /// </summary>
        private PdfTemplate m_on = null;

        /// <summary>
        /// Internal variable to store template for inactive (Off) state.
        /// </summary>
        private PdfTemplate m_off = null;

        /// <summary>
        /// Internal variable to store dictionary.
        /// </summary>
        private PdfDictionary m_dictionary = new PdfDictionary();

        /// <summary>
        /// Internal variable to store dictionary name of checked state.
        /// </summary>
        private string m_onMappingName = DictionaryProperties.Yes;

        /// <summary>
        /// Internal variable to store dictionary name of unchecked state.
        /// </summary>
        private string m_offMappingName = DictionaryProperties.Off;

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the active state template.
        /// </summary>
        /// <value>The <see cref="PdfTemplate"/> object specifies an active state template.</value>
        public PdfTemplate On
        {
            get
            {
                return this.m_on;
            }

            set
            {
                if (this.m_on != value)
                {
                    this.m_on = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the inactive state.
        /// </summary>
        /// <value>The <see cref="PdfTemplate"/> object specifies an inactive state template.</value>
        public PdfTemplate Off
        {
            get
            {
                return this.m_off;
            }

            set
            {
                if (this.m_off != value)
                {
                    this.m_off = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the mapping name of the active state.
        /// </summary>
        /// <value>String specifies the mapping name of the active state.</value>
        public string OnMappingName
        {
            get
            {
                return this.m_onMappingName;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("OnMappingName");
                }

                this.m_onMappingName = value;
            }
        }

        /// <summary>
        /// Gets or sets the mapping name of the inactive state.
        /// </summary>
        /// <value>String specifies the mapping name of the inactive state.</value>
        public string OffMappingName
        {
            get
            {
                return this.m_offMappingName;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("OffMappingName");
                }

                this.m_offMappingName = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfAppearanceState"/> class.
        /// </summary>
        public PdfAppearanceState()
            : base()
        {
            this.m_dictionary.BeginSave += new SavePdfPrimitiveEventHandler(this.Dictionary_BeginSave);
        }

        /// <summary>
        /// Handles the BeginSave event of the m_dictionary control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="ars">The <see cref="Syncfusion.Pdf.Primitives.SavePdfPrimitiveEventArgs"/> instance containing the event data.</param>
        private void Dictionary_BeginSave(object sender, SavePdfPrimitiveEventArgs ars)
        {
            if (this.m_on != null)
            {
                this.m_dictionary.SetProperty(this.m_onMappingName, new PdfReferenceHolder(this.m_on));
            }

            if (this.m_off != null)
            {
                this.m_dictionary.SetProperty(this.m_offMappingName, new PdfReferenceHolder(this.m_off));
            }
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
                return this.m_dictionary;
            }
        }
        #endregion
    }
}
