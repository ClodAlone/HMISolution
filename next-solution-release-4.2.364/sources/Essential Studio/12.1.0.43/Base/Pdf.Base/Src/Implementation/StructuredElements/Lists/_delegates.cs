#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.Drawing;
using Syncfusion.Pdf.Graphics;

/// <summary>
/// The Syncfusion.Pdf.Lists namespace contains classes for creating structure elements in PDF document.
/// </summary>
namespace Syncfusion.Pdf.Lists
{
    #region Delegates
    /// <summary>
    /// Delegate for handling BeginItemLayoutEvent.
    /// </summary>
    /// <param name="sender">The item that begin layout.</param>
    /// <param name="args">Begin Item Layout arguments.</param>
    public delegate void BeginItemLayoutEventHandler(object sender, BeginItemLayoutEventArgs args);

    /// <summary>
    /// Delegate for handling EndItemLayoutEvent.
    /// </summary>
    /// <param name="sender">The item that end layout.</param>
    /// <param name="args">End Item Layout arguments.</param>
    public delegate void EndItemLayoutEventHandler(object sender, EndItemLayoutEventArgs args);
    #endregion

    #region EventArguments
    /// <summary>
    /// Represents begin layout event arguments.
    /// </summary>
    public class BeginItemLayoutEventArgs
    {
        #region Fields
        /// <summary>
        /// Item that layout.
        /// </summary>
        private PdfListItem m_item;

        /// <summary>
        /// The page in which item start layout.
        /// </summary>
        private PdfPage m_page;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <value>The item that layout.</value>
        public PdfListItem Item
        {
            get
            {
                return m_item;
            }
        }

        /// <summary>
        /// Gets the page.
        /// </summary>
        /// <value>The page in which item start layout.</value>
        public PdfPage Page
        {
            get
            {
                return m_page;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BeginItemLayoutEventArgs"/> class.
        /// </summary>
        /// <param name="item">The item that layout.</param>
        /// <param name="page">The page in which item start layout.</param>
        internal BeginItemLayoutEventArgs(PdfListItem item, PdfPage page)
        {
            m_item = item;
            m_page = page;
        }
        #endregion
    }

    /// <summary>
    /// Represents end layout event arguments.
    /// </summary>
    public class EndItemLayoutEventArgs
    {
        #region Fields
        /// <summary>
        /// Item that layouted.
        /// </summary>
        private PdfListItem m_item;

        /// <summary>
        /// The page in which item ended layout.
        /// </summary>
        private PdfPage m_page;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the item that layout.
        /// </summary>
        /// <value>The item that layout.</value>
        public PdfListItem Item
        {
            get
            {
                return m_item;
            }
        }

        /// <summary>
        /// Gets the page in which item ended layout.
        /// </summary>
        /// <value>The page in which item ended layout.</value>
        public PdfPage Page
        {
            get
            {
                return m_page;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EndItemLayoutEventArgs"/> class.
        /// </summary>
        /// <param name="item">The item that layout.</param>
        /// <param name="page">The page in which item end layout.</param>
        internal EndItemLayoutEventArgs(PdfListItem item, PdfPage page)
        {
            m_item = item;
            m_page = page;
        }
        #endregion
    }
    #endregion
}