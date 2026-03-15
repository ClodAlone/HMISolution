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
using System.Collections;
using System.Collections.Generic;
using System;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// 
    /// </summary>
    internal class ListLevels : List<Object>
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="ListLevels"/> class.
        /// </summary>
        public ListLevels()
        {
        }
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        new internal ListLevel this[int index]
        {
            get
            {
                return (base[index] as ListLevel);
            }
            set
            {
                base[index] = value;
            }
        }
        #endregion
    }
}

