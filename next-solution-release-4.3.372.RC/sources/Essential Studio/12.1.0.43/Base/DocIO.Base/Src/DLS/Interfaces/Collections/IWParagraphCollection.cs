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
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents a collection of <see cref="Syncfusion.DocIO.DLS.IWParagraph"/>.
    /// </summary>
    public interface IWParagraphCollection : IEntityCollectionBase
    {
        /// <summary>
        /// Gets the <see cref="Syncfusion.DocIO.DLS.IWParagraph"/> at the specified index.
        /// </summary>
        /// <value></value>
        new WParagraph this[int index]
        {
            get;
        }
        /// <summary>
        /// Adds a paragraph to the end of collection.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        /// <returns></returns>
        int Add(IWParagraph paragraph);
        /// <summary>
        /// Inserts a paragraph into collection at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="paragraph">The paragraph.</param>
        void Insert(int index, IWParagraph paragraph);
        /// <summary>
        /// Returns the zero-based index of the specified paragraph.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        /// <returns></returns>
        int IndexOf(IWParagraph paragraph);
        /// <summary>
        /// Removes the paragraph at the specified index from the collection. 
        /// </summary>
        /// <param name="index">The index.</param>
        void RemoveAt(int index);
    }
}