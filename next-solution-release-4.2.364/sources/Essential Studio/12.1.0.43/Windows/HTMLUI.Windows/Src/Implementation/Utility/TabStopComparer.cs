#region Copyright Syncfusion Inc. 2001 - 2014
////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Collections;
using System.Globalization;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility
{
    /// <summary>
    /// Comparer of elements by their order of getting focused in the document.
    /// </summary>
    internal class TabStopComparer : IComparer
    {
        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the TabStopComparer class
        /// </summary>
        public TabStopComparer()
        {
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Compares elements by their order of getting focused in the document.
        /// </summary>
        /// <param name="x">First element for comparing.</param>
        /// <param name="y">Second element for comparing.</param>
        /// <returns>
        /// See <see cref="System.Collections.IComparer.Compare"/> method for details.
        /// </returns>
        public int Compare(object x, object y)
        {
            IHTMLElement first = x as IHTMLElement;
            IHTMLElement second = y as IHTMLElement;

            if (first == null || second == null)
                throw new ArgumentException("One of elements is null or not of IHTMLElement type.");

            int result;
            int firstID = GetIntFromString(first.UniqueID);
            int secondID = GetIntFromString(second.UniqueID);

            if (first.TabIndex > 0 && second.TabIndex > 0)
            {
                // Both elements have defined TabIndex.           
                result = (first.TabIndex < second.TabIndex) ? -1 :
                  (first.TabIndex == second.TabIndex) ? CompareInt(firstID, secondID) : 1;
            }
            else if (first.TabIndex == 0 && second.TabIndex == 0)
            {
                // Both elements have default TabIndex.
                result = CompareInt(firstID, secondID);
            }           
            else if (first.TabIndex < 0 || second.TabIndex < 0)
            {
                // Some of elements is omited from TabStop list.
                result = (first.TabIndex < second.TabIndex) ? -1 :
                  (first.TabIndex == second.TabIndex) ? 0 : 1;
            }
            else
            {
                // Some of elements has default TabIndex, but another - not default.
                int firstTab = Math.Abs(first.TabIndex);
                int secondTab = Math.Abs(second.TabIndex);

                result = (-firstTab < -secondTab) ? -1 : 1;
            }

            return result;
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Converts the string value to it's integer value.
        /// </summary>
        /// <param name="value">String value of integer number.</param>
        /// <returns>Converted value.</returns>
        private int GetIntFromString(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length == 0)
                throw new ArgumentException("value - string can not be empty");

            double dResult;
            int iResult;
            bool bParsed = double.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out dResult);

            if (bParsed)
            {
                iResult = (int)dResult;
            }
            else
            {
                iResult = int.MinValue;
            }

            return iResult;
        }

        /// <summary>
        /// Compares two integer numbers.
        /// </summary>
        /// <param name="first">First number.</param>
        /// <param name="second">Second number.</param>
        /// <returns> -1 if first number is less than the second; 0 if first number equals the second;
        /// 1 if first number is greater than the second.</returns>
        private int CompareInt(int first, int second)
        {
            return (first < second) ? -1 : (first == second) ? 0 : 1;
        }
        #endregion
    }
}
