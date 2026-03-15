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

using System;

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures
{
    /// <summary>
    /// Summary description for _Constants.
    /// </summary>
    internal sealed class Constants
    {
        #region Class constants
        /// <summary>
        /// Size of the File Character position.
        /// </summary>
        public const int FileCharPosSize = 4;

        /// <summary>
        /// Number of bytes in single word value.
        /// </summary>
        public const int BytesInWord = 2;

        /// <summary>
        /// Number of bytes in single int value.
        /// </summary>
        public const int BytesInInt = 4;

        /// <summary>
        /// Number of bytes in formatted disk page.
        /// </summary>
        public const int DiskPageSize = 512;

        /// <summary>
        /// Number of bytes in single long value.
        /// </summary>
        public const int BytesInLong = 8;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// To prevent creating instances of this class constructor was made private.
        /// </summary>
        private Constants()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        #endregion
    }
}
