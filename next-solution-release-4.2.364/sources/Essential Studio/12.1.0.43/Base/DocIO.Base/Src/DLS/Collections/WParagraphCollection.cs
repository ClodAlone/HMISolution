#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives
using System;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.DLS.XML;

#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents a collection of <see cref="Syncfusion.DocIO.DLS.IWParagraph"/> objects.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class WParagraphCollection
      : EntitySubsetCollection,
        IWParagraphCollection
    {
        #region Properties
        /// <summary>
        /// Gets the <see cref="Syncfusion.DocIO.DLS.IWParagraph"/> at the specified index.
        /// </summary>
        /// <value></value>
        new public WParagraph this[int index]
        {
            get
            {
                ClearIndexes();
                return (WParagraph)GetByIndex(index);
            }
        }
        /// <summary>
        /// Gets the owner <see cref="Syncfusion.DocIO.DLS.ITextBody"/> of the collection.
        /// </summary>
        /// <value>The owner text body.</value>
        internal ITextBody OwnerTextBody
        {
            get
            {
                return Owner as ITextBody;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WParagraphCollection"/> class.
        /// </summary>
        /// <param name="bodyItems">The body items.</param>
        public WParagraphCollection(BodyItemCollection bodyItems)
            : base(bodyItems, EntityType.Paragraph)
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds a paragraph to end of text body.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        /// <returns></returns>
        public int Add(IWParagraph paragraph)
        {
            Document.EnsureParagraphStyle(paragraph);
            return InternalAdd((Entity)paragraph);
        }
        /// <summary>
        /// Determines whether the <see cref="Syncfusion.DocIO.DLS.IWParagraphCollection"/> contains a specific value.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        /// <returns>
        /// 	If paragraph is found, set to <c>true</c>.
        /// </returns>
        public bool Contains(IWParagraph paragraph)
        {
            return InternalContains((Entity)paragraph);
        }
        /// <summary>
        /// Inserts a paragraph into collection at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="paragraph">The paragraph.</param>
        public void Insert(int index, IWParagraph paragraph)
        {
            Document.EnsureParagraphStyle(paragraph);
            InternalInsert(index, (Entity)paragraph);
        }
        /// <summary>
        /// Returns the zero-based index of the specified paragraph.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        /// <returns></returns>
        public int IndexOf(IWParagraph paragraph)
        {
            return InternalIndexOf((Entity)paragraph);
        }
        /// <summary>
        /// Removes the specified paragraph.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        public void Remove(IWParagraph paragraph)
        {
            InternalRemove((Entity)paragraph);
        }
        /// <summary>
        /// Removes the paragraph at the specified index from the collection.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveAt(int index)
        {
            InternalRemoveAt(index);
        }
        #endregion
    }
}