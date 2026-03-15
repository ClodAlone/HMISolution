#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISpellEditor
    {
        /// <summary>
        /// Gets or Sets the control to be checked
        /// </summary>
        Control ControlToCheck
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets the selected text
        /// </summary>
        string SelectedText
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets the text to be checked
        /// </summary>
        string Text
        {
            get;
            set;
        }

        /// <summary>
        /// Selects the particular range of text in the editor
        /// </summary>
        /// <param name="selectionStart"></param>
        /// <param name="selectionLength"></param>
        void Select(int selectionStart, int selectionLength);

        /// <summary>
        /// Returns a value indicating whether to continue with the spell checking
        /// </summary>
        /// <returns></returns>
        bool HasMoreTextToCheck();

        /// <summary>
        /// Focuses the editor
        /// </summary>
        void Focus();

        /// <summary>
        /// Updates the text field so that SpellChecker will spell check with newly updated texts
        /// </summary>
        void UpdateTextField();
    }
}
