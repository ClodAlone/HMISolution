#region Copyright Syncfusion Inc. 2001 - 2014
////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Drawing;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility.Selection
{
    /// <summary>
    /// Utility class containing information about one letter of some text and it's 
    /// position in the document.
    /// </summary>
    internal class LetterPair
    {
        #region Class members
        /// <summary>
        /// Symbol letter in the text.
        /// </summary>
        private string m_symbol;

        /// <summary>
        /// Rectangle containing the letter in the document.
        /// </summary>
        private RectangleF m_bounds;

        /// <summary>
        /// Number of the symbol in it's parent element.
        /// </summary>
        private int m_number;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the symbol letter in the document.
        /// </summary>
        internal string Symbol
        {
            get
            {
                return m_symbol;
            }
        }

        /// <summary>
        /// Gets the bounds of the letter in the document.
        /// </summary>
        internal RectangleF Bounds
        {
            get
            {
                return m_bounds;
            }
        }

        /// <summary>
        /// Gets the number of the symbol in it's parent element.
        /// </summary>
        internal int Number
        {
            get
            {
                return m_number;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Prevents a default instance of the LetterPair class from being created
        /// </summary>
        private LetterPair()
        {
        }

        /// <summary>
        /// Initializes a new instance of the LetterPair class
        /// </summary>
        /// <param name="symbol">Letter of the text in the document.</param>
        /// <param name="bounds">Bounds of the letter in the document.</param>
        /// <param name="number">Number of the symbol in it's parent element.</param>
        public LetterPair(string symbol, RectangleF bounds, int number)
        {
            if (symbol == null)
                throw new ArgumentNullException("symbol");

            m_symbol = symbol;
            m_bounds = bounds;
            m_number = number;
        }
        #endregion
    }
}
