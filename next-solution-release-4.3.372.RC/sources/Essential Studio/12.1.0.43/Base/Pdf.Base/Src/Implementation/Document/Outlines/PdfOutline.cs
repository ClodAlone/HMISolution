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

namespace Syncfusion.Pdf.Interactive
{
    /// <summary>
    /// Each instance of this class represents
    /// an bookmark node in the bookmark tree.
    /// </summary>
    public class PdfBookmark : PdfBookmarkBase
    {
        #region Fields
        /// <summary>
        /// 
        /// Internal variable to store destination.
        /// </summary>
        private PdfDestination m_destination;

        /// <summary>
        /// Internal variable to store color.
        /// </summary>
        private PdfColor m_color;

        /// <summary>
        /// Internal variable to store text Style.
        /// </summary>
        private PdfTextStyle m_textStyle;

        /// <summary>
        /// Internal variable to store previous. 
        /// </summary>
        private PdfBookmark m_previous;

        /// <summary>
        /// Internal variable to store next.
        /// </summary>
        private PdfBookmark m_next;

        /// <summary>
        /// Internal variable to store parent.
        /// </summary>
        private PdfBookmarkBase m_parent;

        /// <summary>
        /// Internal variable to store action.
        /// </summary>
        private PdfAction m_action;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfBookmark"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="previous">The previous.</param>
        /// <param name="next">The next.</param>
        internal PdfBookmark(string title, PdfBookmarkBase parent, PdfBookmark previous, PdfBookmark next)
            : base()
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }

            if (title == null)
            {
                throw new ArgumentNullException("title");
            }

            m_parent = parent;

            Dictionary.SetProperty(DictionaryProperties.Parent, new PdfReferenceHolder(parent));

            Previous = previous;
            Next = next;
            Title = title;           
          
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfBookmark"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="previous">The previous.</param>
        /// <param name="next">The next.</param>
        /// <param name="dest">The dest.</param>
        internal PdfBookmark(string title, PdfBookmarkBase parent, PdfBookmark previous, PdfBookmark next, PdfDestination dest)
            : this(title, parent, previous, next)
        {
            if (dest == null)
            {
                throw new ArgumentNullException("dest");
            }

            Destination = dest;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfBookmark"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="crossTable">The cross table.</param>
        internal PdfBookmark(PdfDictionary dictionary, PdfCrossTable crossTable)
            : base(dictionary, crossTable)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the outline destination.
        /// </summary>
        public virtual PdfDestination Destination
        {
            get
            {
                return m_destination;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Destination");
                }

                m_destination = value;
                Dictionary.SetProperty(DictionaryProperties.Dest, value);
            }
        }

        /// <summary>
        /// Gets or sets the outline title.
        /// </summary>
        /// <remarks>The outline title is the text,
        /// which appears in the outline tree as a tree node.</remarks>
        public virtual string Title
        {
            get
            {
                PdfDictionary dic = Dictionary;
                PdfString title = dic[DictionaryProperties.Title] as PdfString;
                string value = null;

                if (title != null)
                {
                    value = title.Value;
                }

                return value;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Title");
                }

                Dictionary.SetString(DictionaryProperties.Title, value);
            }
        }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        public virtual PdfColor Color
        {
            get
            {
                return m_color;
            }

            set
            {
                if (m_color != value)
                {
                    m_color = value;
                    UpdateColor();
                }
            }
        }

        /// <summary>
        /// Gets or sets the text style.
        /// </summary>
        public virtual PdfTextStyle TextStyle
        {
            get
            {
                return m_textStyle;
            }

            set
            {
                if (m_textStyle != value)
                {
                    m_textStyle = value;
                    UpdateTextStyle();
                }
            }
        }

        /// <summary>
        /// Gets or sets the Action for the Outline.
        /// </summary>        
        public PdfAction Action
        {
            get
            {
                return m_action;
            }

            set
            {
                if (m_action != value)
                {
                    m_action = value;
                    Dictionary.SetProperty(DictionaryProperties.A, new PdfReferenceHolder(m_action.Dictionary));
                }
            }
        }

        /// <summary>
        /// Gets or sets the whether to expand the node or not
        /// </summary>
        public bool IsExpanded
        {
            get
            {
               return base.IsExpanded;
            }
            set
            {
                base.IsExpanded = value;
            }
        }

        /// <summary>
        /// Gets or sets the previous outline object.
        /// </summary>
        /// <remarks>The null value means that the object is the first outline.</remarks>
        internal virtual PdfBookmark Previous
        {
            get
            {
                return m_previous;
            }

            set
            {
                if (m_previous != value)
                {
                    m_previous = value;
                    Dictionary.SetProperty(DictionaryProperties.Prev, new PdfReferenceHolder(value));
                }
            }
        }

        /// <summary>
        /// Gets the parent outline base.
        /// </summary>
        internal virtual PdfBookmarkBase Parent
        {
            get
            {
                return m_parent;
            }
        }

        /// <summary>
        /// Gets or sets the next outline object.
        /// </summary>
        /// <remarks>The null value means that the object is the last outline.</remarks>
        internal virtual PdfBookmark Next
        {
            get
            {
                return m_next;
            }

            set
            {
                if (m_next != value)
                {
                    m_next = value;
                    Dictionary.SetProperty(DictionaryProperties.Next, new PdfReferenceHolder(value));
                }
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Sets the parent.
        /// </summary>
        /// <param name="parent">The parent.</param>
        internal void SetParent(PdfBookmarkBase parent)
        {
            m_parent = parent;
        }

        /// <summary>
        /// Updates the color.
        /// </summary>
        private void UpdateColor()
        {
            PdfDictionary dic = Dictionary;
            PdfArray array = dic[DictionaryProperties.C] as PdfArray;

            if (array != null && m_color.IsEmpty)
            {
                dic.Remove(DictionaryProperties.C);
            }
            else
            {
                dic[DictionaryProperties.C] = m_color.ToArray();
            }
        }

        /// <summary>
        /// Updates the outline text style.
        /// </summary>
        private void UpdateTextStyle()
        {
            if (m_textStyle == PdfTextStyle.Regular)
            {
                PdfDictionary dic = Dictionary;

                dic.Remove(DictionaryProperties.F);
            }
            else
            {
                Dictionary.SetNumber(DictionaryProperties.F, (int)m_textStyle);
            }
        }
        #endregion
    }
}
