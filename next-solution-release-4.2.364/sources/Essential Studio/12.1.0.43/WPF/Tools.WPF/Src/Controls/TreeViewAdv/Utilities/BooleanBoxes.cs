// <copyright file="BooleanBoxes.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

#region file using

#endregion file using

namespace Syncfusion.Windows.Tools
{
    /// <summary>
    /// Represents BooleanBoxees class
    /// </summary>
    internal static class BooleanBoxes
    {
        #region Private members

        /// <summary>
        /// Presents false box
        /// </summary>
        internal static object FalseBox = false;

        /// <summary>
        /// Presents true box
        /// </summary>
        internal static object TrueBox = true;

        #endregion Private members

        #region Implementation

        /// <summary>
        /// Boxes the specified value.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <returns> object type </returns>
        internal static object Box(bool value)
        {
            if (value)
            {
                return TrueBox;
            }

            return FalseBox;
        }

        #endregion Implementation
    }
}