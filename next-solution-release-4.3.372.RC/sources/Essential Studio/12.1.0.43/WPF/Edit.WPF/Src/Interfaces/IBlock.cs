// <copyright file="IBlock.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System.Windows.Media;

namespace Syncfusion.Windows.Edit
{
    /// <summary>
    /// IBlock interface has the Block properties of Lexems
    /// </summary>
    /// <remarks>
    /// IBlock contain the properties of a lexem like StartText, EndText, Ismultiline
    /// etc.
    /// </remarks>
#if SyncfusionFramework4_0

    using System.ComponentModel;

    [DesignTimeVisible(false)]
#endif
    public interface IBlock
    {
        /// <summary>
        /// Gets or sets the start text.
        /// </summary>
        /// <value>The start text.</value>
        string StartText
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the end text.
        /// </summary>
        /// <value>The end text.</value>
        string EndText
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is multiline.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is multiline; otherwise, <c>false</c>.
        /// </value>
        bool IsMultiline
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of the lexem.
        /// </summary>
        /// <value>The type of the lexem.</value>
        EditTokenType LexemType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [contains end text].
        /// </summary>
        /// <value><c>true</c> if the Lexem contains the end text; otherwise, <c>false</c>.</value>
        bool ContainsEndText
        {
            get;
            set;
        }
    }

    /// <summary>
    /// IFormat interface has the Formats of Lexems.
    /// </summary>
    /// <remarks>
    /// IFormat contains the formatname properties which used to access format
    /// collection properties of a lexem.
    /// </remarks>
    public interface IFormat
    {
        /// <summary>
        /// Gets or sets the name of the format.
        /// </summary>
        /// <value>The name of the format.</value>
        string FormatName
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        FontFamily FontFamily
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        double FontSize
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        Brush Foreground
        {
            get;
            set;
        }
    }
}