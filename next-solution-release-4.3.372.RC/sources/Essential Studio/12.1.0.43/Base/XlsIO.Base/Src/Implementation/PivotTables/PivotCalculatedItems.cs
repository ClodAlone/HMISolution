#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
namespace Syncfusion.XlsIO.Implementation.PivotTables
{
    /// <summary>
    /// Represents the collection of calculated pivot field item in pivot table
    /// </summary>
   public  class PivotCalculatedItems: List<PivotCalculatedItemImpl>
    {
        #region Properties

        #endregion

        #region Initialization
        public PivotCalculatedItems()
        {

        }
        #endregion

        #region Methods
       /// <summary>
       /// Add the calculated item to pivot field.
       /// </summary>
       /// <param name="item">item to add to the pivot field.</param>
        public void Add(PivotCalculatedItemImpl item)
        {
            base.Add(item);
        }
        #endregion

    }
}
