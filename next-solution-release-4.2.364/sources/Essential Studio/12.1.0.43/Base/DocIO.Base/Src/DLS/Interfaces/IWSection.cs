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
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents a section inside a Document.
    /// </summary>
    public interface IWSection : ICompositeEntity
    {
        /// <summary>
        /// Gets the paragraphs.
        /// </summary>
        /// <value>The paragraphs.</value>
        IWParagraphCollection Paragraphs
        {
            get;
        }
        /// <summary>
        /// Gets the tables.
        /// </summary>
        /// <value>The tables.</value>
        IWTableCollection Tables
        {
            get;
        }
        /// <summary>
        /// Gets the section body.
        /// </summary>
        /// <value>The body.</value>
        WTextBody Body
        {
            get;
        }
        /// <summary>
        /// Gets page Setup of current section.
        /// </summary>
        WPageSetup PageSetup
        {
            get;
        }
        /// <summary>
        /// Get collection of columns which logically divide page on many 
        /// printing/publishing areas.
        /// </summary>
        ColumnCollection Columns
        {
            get;
        }
        /// <summary>
        /// Gets / sets break code.
        /// </summary>
        SectionBreakCode BreakCode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [protect form].
        /// </summary>
        /// <value><c>true</c> if [protect form]; otherwise, <c>false</c>.</value>
        bool ProtectForm
        {
            get;
            set;
        }
        /// <summary>
        /// Adds new column to the section.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="spacing"></param>
        /// <returns></returns>
        Column AddColumn(float width, float spacing);
        /// <summary>
        /// Adds the paragraph.
        /// </summary>
        /// <returns></returns>
        IWParagraph AddParagraph();
        /// <summary>
        /// Adds the table.
        /// </summary>
        /// <returns></returns>
        IWTable AddTable();
        /// <summary>
        /// Clones it self.
        /// </summary>
        /// <returns></returns>
        new WSection Clone();
        /// <summary>
        /// Makes all columns in current section to be of equal width.
        /// </summary>
        void MakeColumnsEqual();
        /// <summary>
        /// Gets headers/footers of current section
        /// </summary>
        WHeadersFooters HeadersFooters { get; }
    }
}