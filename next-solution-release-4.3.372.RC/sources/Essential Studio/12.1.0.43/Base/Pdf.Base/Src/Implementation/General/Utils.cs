#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.IO;

namespace Syncfusion.Pdf
{
    /// <summary>
    /// Utility class.
    /// </summary>
    internal class Utils
    {
        #region Constants
        /// <summary>
        /// Number of decimals in float rounding.
        /// </summary>
        private const int c_roundDecimals = 4;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Utils"/> class.
        /// </summary>
        private Utils()
        {
        }
        #endregion

        #region Public methods
        ///// <summary>
        ///// Rounds float value.
        ///// </summary>
        ///// <param name="val">Float value.</param>
        ///// <returns>Rounded value.</returns>
        //public static float Round( float val )
        //{
        //  float rounded = ( float )Math.Round( val, c_roundDecimals );
        //  return rounded;
        //}

        /// <summary>
        /// Checks the file path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The path if it exist.</returns>
        /// <exception cref="FileNotFoundException">It's thrown if the file wasn't found.</exception>
#if !NETFX_CORE && !WP
        public static string CheckFilePath(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            string full = System.IO.Path.GetFullPath(path);

            if (!File.Exists(full))
            {
                throw new FileNotFoundException("File can't be found");
            }

            return full;
        }
#endif
        #endregion
    }
}
