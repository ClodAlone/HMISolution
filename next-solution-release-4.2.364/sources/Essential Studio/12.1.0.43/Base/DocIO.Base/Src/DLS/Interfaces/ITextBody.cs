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
    /// Interface publish text body functionality
    /// </summary>
    public interface ITextBody : ICompositeEntity
    {
        /// <summary>
        /// Gets inner tables.
        /// </summary>
        IWTableCollection Tables { get; }
        /// <summary>
        /// Gets inner paragraphs.
        /// </summary>
        IWParagraphCollection Paragraphs { get; }
        /// <summary>
        /// Gets the form fields.
        /// </summary>
        /// <value>The form fields.</value>
        FormFieldCollection FormFields
        {
            get;
        }
        /// <summary>
        /// Gets the last paragraph.
        /// </summary>
        /// <value>The last paragraph.</value>
        IWParagraph LastParagraph
        {
            get;
        }
        /// <summary>
        /// Adds paragraph at the end of section.
        /// </summary>
        /// <returns></returns>
        IWParagraph AddParagraph();
        /// <summary>
        /// Adds the table.
        /// </summary>
        /// <returns></returns>
        IWTable AddTable();
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Inserts html at end of text body.
        /// </summary>
        void InsertXHTML(string html);
        /// <summary>
        /// Inserts html begins from paragraph specified by paragraphIndex.
        /// </summary>
        void InsertXHTML(string html, int paragraphIndex);
        /// <summary>
        /// Inserts html beginning from paragraph specified by paragraphIndex, 
        /// and after paragraph item specified by paragraphItemIndex.
        /// </summary>
        void InsertXHTML(string html, int paragraphIndex, int paragraphItemIndex);
#endif
        /// <summary>
        /// If the text body has no paragraphs, creates and appends one WParagraph. 
        /// </summary>
        void EnsureMinimum();
    }
}