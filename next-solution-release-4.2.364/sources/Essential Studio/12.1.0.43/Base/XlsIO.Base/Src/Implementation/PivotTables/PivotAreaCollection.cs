#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
using Syncfusion.XlsIO.Implementation.PivotTables;
namespace Syncfusion.XlsIO.Implementation.Collections
{
    class PivotAreaCollection : List<PivotArea>
    {
          #region Initialize
        public PivotAreaCollection()
        {
            
        }
        #endregion
        #region Methods
        public void AddPivotArea(PivotArea pivotArea)
        {
            base.Add(pivotArea);
        }
        #endregion
    }
}
