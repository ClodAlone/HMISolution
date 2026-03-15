#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System.Drawing;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Interface to text editing components.
    /// </summary>
    public interface ITextEditor
    {
        /// <summary>
        /// Starts editing node.
        /// </summary>
        /// <param name="nodeEditing">node to edit</param>
        /// <returns>success operation value</returns>
        bool BeginEdit(Node nodeEditing);

        /// <summary>
        /// Ends node editing.
        /// </summary>
        /// <param name="bSaveChanges">if set to <c>true</c> save changes.</param>
        void EndEdit(bool bSaveChanges);

        /// <summary>
        /// Gets or sets the current text.
        /// </summary>
        string CurrentText
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the selected text is bold.
        /// </summary>
        bool Bold
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the selected text is in italics.
        /// </summary>
        bool Italic
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the selected text is underlined.
        /// </summary>
        bool Underline
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the selected text has strikeout property.
        /// </summary>
        bool Strikeout
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets name of font family.
        /// </summary>
        string FontFamily
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets size of font in points.
        /// </summary>
        float PointSize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets horizontal alignment of text.
        /// </summary>
        StringAlignment HorizontalAlignment
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether color of text.
        /// </summary>
        Color TextColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether text is Superscript.
        /// </summary>
        bool Superscript
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether text is subscript.
        /// </summary>
        bool Subscript
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets no of Characters to Offset
        /// </summary>
        int CharOffset
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether text is Lower.
        /// </summary>
        bool Lower
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether text is Upper.
        /// </summary>
        bool Upper
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Helper interface used with Clipboard Cut/Paste.
    /// </summary>
    internal interface ITextEdit
    {
        /// <summary>
        /// Performs Clipboard text Cut.
        /// </summary>
        void Cut();

        /// <summary>
        /// Performs paste text from clipboard.
        /// </summary>
        /// <param name="strText">Text to be pasted.</param>
        void Paste(string strText);
    }
}