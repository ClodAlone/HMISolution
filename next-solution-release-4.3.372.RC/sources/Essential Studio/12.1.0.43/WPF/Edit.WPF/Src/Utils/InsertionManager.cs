// <copyright file="InsertionManager.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;

namespace Syncfusion.Windows.Edit
{
    /// <summary>
    /// InsertionManager is class used to insert and replace the text for the EditControl.
    /// </summary>
    /// <remarks>
    /// The InsertionManager class contains insertion and replace information of EditControl.
    /// </remarks>
#if SyncfusionFramework4_0

    using System.ComponentModel;

    [DesignTimeVisible(false)]
#endif
    public class InsertionManager
    {
        /// <summary>
        /// Insert the new text into the existing text.
        /// </summary>
        /// <param name="existingtext">Gets the Existing text from the reporting source</param>
        /// <param name="insertiontext">Gets the new text from the reporting source</param>
        /// <param name="index">Gets the index position of the text to be insert</param>
        /// <returns>Returns the result string which contains the new text</returns>
        public static string InsertText(string existingtext, string insertiontext, int index)
        {
            string returnstring = existingtext.Insert(Math.Min(index, existingtext.Length), insertiontext);
            return returnstring;
        }

        /// <summary>
        /// Replace the existing text with new text in InsertionManager.
        /// </summary>
        /// <param name="existingtext">Gets the Existing text from the reporting source</param>
        /// <param name="replacetext">Gets the text to be replace</param>
        /// <param name="index">Gets the index position of the text to be replace</param>
        /// <returns>Returns the result string</returns>
        public static string ReplaceText(string existingtext, string replacetext, int index)
        {
            string returnstring = existingtext;
            if (existingtext.Length > index)
            {
                returnstring = existingtext.Remove(Math.Min(existingtext.Length, index), replacetext.Length);
            }

            returnstring = returnstring.Insert(Math.Min(existingtext.Length, index), replacetext);
            return returnstring;
        }
    }
}