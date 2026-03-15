//-------------------------------------------------------------------------------------------------
// <copyright file="GridWordConverterBase.cs" company="Syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Syncfusion.GridHelperClasses
{
    using System;
    using System.Collections.Generic;
    using System.Text;  
    using System.Drawing;
    using System.ComponentModel;
    using Syncfusion.Windows.Forms.Grid;
    using Syncfusion.DocIO;
    using Syncfusion.DocIO.DLS;
    using Syncfusion.Windows.Forms.Grid.Grouping;
    using Syncfusion.Grouping;

    /// <summary>
    /// The base class for exporting Grid to Word.
    /// </summary>
    /// <remarks> It has support for header and footer.</remarks>
    [ToolboxItem(false)]
    public class GridWordConverterBase
    {
        bool showHeader = false;
        bool showFooter = false;

        /// <summary>
        /// Initializes a new GridWordCoverter.
        /// </summary>
        public GridWordConverterBase()
        {
        }

        /// <summary>
        /// Initializes a new GridWordCoverter.
        /// </summary>
        /// <param name="showHeader">True if Header should be shown; Default is false.</param>
        /// <param name="showFooter">True if Footer should be shown; Default is false.</param>
        public GridWordConverterBase(bool showHeader, bool showFooter)
        {
            this.showHeader = showHeader;
            this.showFooter = showFooter;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to add document header.
        /// </summary>
        public bool ShowHeader
        {
            get
            {
                return this.showHeader;
            }

            set
            {
                this.showHeader = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to add document footer.
        /// </summary>
        public bool ShowFooter
        {
            get
            {
                return this.showFooter;
            }

            set
            {
                this.showFooter = value;
            }
        }

        /// <summary>
        /// Represents the method that handles <see cref="DrawDocHeaderFooterEventHandler"/> and <see cref="DrawDocHeaderFooterEventHandler"/> events.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="PDFHeaderFooterEventArgs"/> that contains the event data.</param>
        public delegate void DrawDocHeaderFooterEventHandler(object sender, DocHeaderFooterEventArgs e);

        /// <summary>
        /// Lets the user draw a header for the PDF Docment
        /// </summary>
        public event DrawDocHeaderFooterEventHandler DrawHeader;

        /// <summary>
        /// Lets the user draw a Footer for the PDF Docment
        /// </summary>
        public event DrawDocHeaderFooterEventHandler DrawFooter;

        /// <summary>
        /// Raises the <see cref="OnDrawHeader"/> event.
        /// </summary>
        /// <param name="e">A <see cref="DocHeaderFooterEventArgs"/> that contains the event data</param>
        protected virtual void OnDrawHeader(DocHeaderFooterEventArgs e)
        {
            if (this.DrawHeader != null)
            {
                this.DrawHeader(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="OnDrawFooter"/> event.
        /// </summary>
        /// <param name="e">A <see cref="OnDrawFooter"/> that contains the event data</param>
        protected virtual void OnDrawFooter(DocHeaderFooterEventArgs e)
        {
            if (this.DrawFooter != null)
            {
                this.DrawFooter(this, e);
            }
        }

        /// <summary>
        /// Draws the Header and Footer.
        /// </summary>
        /// <param name="wordDocument">The word document.</param>
        /// <param name="top">True if Header should be added.</param>
        /// <param name="bottom">True if Footer should be added.</param>
        protected void DrawHeaderFooter(WordDocument wordDocument, bool top, bool bottom)
        {
            if (top)
            {
                WHeadersFooters headerFooter = wordDocument.Sections[0].HeadersFooters;

                // Raise DrawHeader
                this.OnDrawHeader(new DocHeaderFooterEventArgs(headerFooter.Header));
            }

            if (bottom)
            {
                WHeadersFooters headerFooter = wordDocument.Sections[0].HeadersFooters;

                // Raise DrawFooter
                this.OnDrawFooter(new DocHeaderFooterEventArgs(headerFooter.Footer));
            }
        }
    }

    /// <summary>
    /// Provides data for the <see cref="DocHeaderFooterEventArgs"/> and <see cref="DocHeaderFooterEventArgs"/> events
    /// </summary>
    /// <remarks>To draw the Header / Footer for the created word document, handle the <see cref="DocHeaderFooterEventArgs"/> / <see cref="DocHeaderFooterEventArgs"/> events
    /// </remarks>
    public class DocHeaderFooterEventArgs : EventArgs
    {        
        HeaderFooter header, footer;

        /// <summary>
        /// Gets or sets the document header.
        /// </summary>
        public HeaderFooter Header
        {
            get
            {
                return this.header;
            }

            set
            {
                this.header = value;
            }
        }

        /// <summary>
        /// Gets or sets the document footer.
        /// </summary>
        public HeaderFooter Footer
        {
            get
            {
                return this.footer;
            }

            set
            {
                this.footer = value;
            }
        }

        /// <summary>
        /// Initializes a new <see cref="DocHeaderFooterEventArgs"/>
        /// </summary>
        /// <param name="headerFooter">Header and footer for the document.</param>
        public DocHeaderFooterEventArgs(HeaderFooter headerFooter)
        {
            this.header = headerFooter;
            this.footer = headerFooter;
        }
    }
}
