#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;

namespace Syncfusion.XlsIO.Implementation.PivotTables
{
    class FilterColumnFilters
    {
        List<string> Values = new List<string>();


        #region Properties

        /// <summary>
        /// Get the filter column filter based on Index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string this[int index]
        {
            get
            {
                if (Values.Count > 0)
                    return Values[index];
                else
                    throw new ArgumentException("Specified index is not valid");
            }
        }
        #endregion

        #region Methods
        public int Count()
        {
            return Values.Count;
        }



        /// <summary>
        /// Add the filter column filter to filter collections
        /// </summary>
        /// <param name="Value"></param>
        public void Add(string Value)
        {
            Values.Add(Value);
        }
        #endregion
    }
}
