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
using System.Globalization;
using System.IO;
using System.Text;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Primitives;

/// <summary>
/// The Syncfusion.Pdf.Parsing namespace contains classes, which are used to load or modify an existing PDF document.
/// </summary>
namespace Syncfusion.Pdf.Parsing
{
    /// <summary>
    /// Represents base class for field's group items.
    /// </summary>
    public class PdfLoadedFieldItem
    {
        #region Fields
        /// <summary>
        /// Field which item belongs to.
        /// </summary>
        private PdfLoadedStyledField m_field;

        /// <summary>
        /// Item index in collection.
        /// </summary>
        private int m_collectionIndex;

        private PdfDictionary m_dictionary;

        /// <summary>
        /// Local variable to hold page reference.
        /// </summary>
        private PdfPageBase m_page;
        #endregion

        #region Properties
        /// <summary>
        /// Get the current Loaded style Field.
        /// </summary>
        protected PdfLoadedStyledField Field
        {
            get
            {
                return m_field;
            }
        }
        /// <summary>
        /// Gets the parent.
        /// </summary>
        internal PdfLoadedStyledField Parent
        {
            get
            {
                return m_field;
            }
        }

        /// <summary>
        /// Gets the cross table.
        /// </summary>
        internal PdfCrossTable CrossTable
        {
            get
            {
                return Parent.CrossTable;
            }
        }

        /// <summary>
        /// Gets the dictionary.
        /// </summary>
        internal PdfDictionary Dictionary
        {
            get
            {
                return m_dictionary;
            }
        }

        /// <summary>
        /// Gets or sets the bounds.
        /// </summary>
        public RectangleF Bounds
        {
            get
            {
                int backUpIndex = m_field.DefaultIndex;
                m_field.DefaultIndex = m_collectionIndex;

                RectangleF rect = m_field.Bounds;

                m_field.DefaultIndex = backUpIndex;

                return rect;
            }
            set
            {
                int backUpIndex = m_field.DefaultIndex;
                m_field.DefaultIndex = m_collectionIndex;
                m_field.Bounds = value;
                m_field.DefaultIndex = backUpIndex;
            }
        }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        public PointF Location
        {
            get
            {
                return Bounds.Location;
            }
            set
            {
                Bounds = new RectangleF(value, Bounds.Size);
            }
        }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        public SizeF Size
        {
            get
            {
                return Bounds.Size;
            }
            set
            {
                Bounds = new RectangleF(Bounds.Location, value);
            }
        }

        /// <summary>
        /// Gets the border pen.
        /// </summary>
        internal PdfPen BorderPen
        {
            get
            {
                int backUpIndex = m_field.DefaultIndex;
                m_field.DefaultIndex = m_collectionIndex;

                PdfPen pen = m_field.BorderPen;

                m_field.DefaultIndex = backUpIndex;

                return pen;
            }
        }

        /// <summary>
        /// Gets the border style.
        /// </summary>
        internal PdfBorderStyle BorderStyle
        {
            get
            {
                int backUpIndex = m_field.DefaultIndex;
                m_field.DefaultIndex = m_collectionIndex;

                PdfBorderStyle bs = m_field.BorderStyle;

                m_field.DefaultIndex = backUpIndex;

                return bs;
            }
        }

        /// <summary>
        /// Gets the DashPatern.
        /// </summary>
        internal float[] DashPatern
        {
            get
            {
                int backUpIndex = m_field.DefaultIndex;
                m_field.DefaultIndex = m_collectionIndex;

                float[] dashPatern = m_field.DashPatern;

                m_field.DefaultIndex = backUpIndex;

                return dashPatern;
            }
        }

        /// <summary>
        /// Gets the width of the border.
        /// </summary>
        internal int BorderWidth
        {
            get
            {
                int backUpIndex = m_field.DefaultIndex;
                m_field.DefaultIndex = m_collectionIndex;

                int borderWidth = m_field.BorderWidth;

                m_field.DefaultIndex = backUpIndex;

                return borderWidth;
            }
        }

        /// <summary>
        /// Gets the string format.
        /// </summary>
        internal PdfStringFormat StringFormat
        {
            get
            {
                int backUpIndex = m_field.DefaultIndex;
                m_field.DefaultIndex = m_collectionIndex;

                PdfStringFormat sFormat = m_field.StringFormat;

                m_field.DefaultIndex = backUpIndex;

                return sFormat;
            }
        }

        /// <summary>
        /// Gets the back brush.
        /// </summary>
        internal PdfBrush BackBrush
        {
            get
            {
                int backUpIndex = m_field.DefaultIndex;
                m_field.DefaultIndex = m_collectionIndex;

                PdfBrush backBrush = m_field.BackBrush;

                m_field.DefaultIndex = backUpIndex;

                return backBrush;
            }
        }

        /// <summary>
        /// Gets the color of the fore.
        /// </summary>
        internal PdfBrush ForeBrush
        {
            get
            {
                int backUpIndex = m_field.DefaultIndex;
                m_field.DefaultIndex = m_collectionIndex;

                PdfBrush foreBrush = m_field.ForeBrush;

                m_field.DefaultIndex = backUpIndex;

                return foreBrush;
            }
        }

        /// <summary>
        /// Gets the shadow brush.
        /// </summary>
        internal PdfBrush ShadowBrush
        {
            get
            {
                int backUpIndex = m_field.DefaultIndex;
                m_field.DefaultIndex = m_collectionIndex;

                PdfBrush shadowBrush = m_field.ShadowBrush;

                m_field.DefaultIndex = backUpIndex;

                return shadowBrush;
            }
        }

        /// <summary>
        /// Gets the font.
        /// </summary>
        internal PdfFont Font
        {
            get
            {
                int backUpIndex = m_field.DefaultIndex;
                m_field.DefaultIndex = m_collectionIndex;

                PdfFont font = m_field.Font;

                m_field.DefaultIndex = backUpIndex;

                return font;
            }
        }

        /// <summary>
        /// Gets the page.
        /// </summary>
        public PdfPageBase Page
        {
            get
            {
                if (m_page == null)
                {
                    int backUpIndex = m_field.DefaultIndex;
                    m_field.DefaultIndex = m_collectionIndex;

                    m_page = m_field.Page;
                    PdfName pName = new PdfName(DictionaryProperties.P);

                    if (m_field.Kids.Count > 0 && m_dictionary.ContainsKey(pName))
                    {
                        PdfLoadedDocument doc = CrossTable.Document as PdfLoadedDocument;
                        IPdfPrimitive pageRef = CrossTable.GetObject(m_dictionary[DictionaryProperties.P]);
                        PdfDictionary pageDic = pageRef as PdfDictionary;

                        if (pageDic != null)
                            m_page = doc.Pages.GetPage(pageDic);
                    }

                    m_field.DefaultIndex = backUpIndex;
                }

                return m_page;
            }
            internal set
            {
                m_page = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLoadedFieldItem"/> class.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="index">The index.</param>
        /// <param name="dictionary">The dictionary.</param>
        internal PdfLoadedFieldItem(PdfLoadedStyledField field, int index, PdfDictionary dictionary)
        {
            m_field = field;
            m_collectionIndex = index;
            m_dictionary = dictionary;
        }
        #endregion
    }
}
