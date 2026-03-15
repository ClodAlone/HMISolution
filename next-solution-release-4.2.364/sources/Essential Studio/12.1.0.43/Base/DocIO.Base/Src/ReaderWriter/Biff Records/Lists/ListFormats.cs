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
using System;
using System.Collections;
using System.Collections.Generic;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// 
    /// </summary>
    internal class ListFormats : List<ListData>
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="ListFormats"/> class.
        /// </summary>
        public ListFormats()
        {
        }
        #endregion

        #region Class internal methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal ListData GetListFromId(int id)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].ListID == id)
                {
                    return this[i];
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listId"></param>
        /// <returns></returns>
        internal ListData FindListData(int listId)
        {
            ListData listData = GetListFromId(listId);
            if (listData == null)
            {
                throw new ArgumentException("List data with the specified id could not be found.");
            }
            return listData;
        }
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        new internal ListData this[int index]
        {
            get
            {
                return base[index];
            }
            set
            {
                base[index] = value;
            }
        }
        #endregion
    }
}

