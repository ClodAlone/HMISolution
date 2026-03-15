#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;

namespace Syncfusion.OlapSilverlight.Engine
{
    public class PivotRowDescriptor:IPivotCellHost 
    {
        #region class members
        private PivotCellCollection m_cells = null;
        #endregion

        #region class properties
        public PivotCellCollection Cells
        {
            get
            {
                return m_cells;
            }
        }
        #endregion

        #region class initialize/finalize methods
        public PivotRowDescriptor()
        {
            m_cells = new PivotCellCollection();
        }
        #endregion

        #region ICloneable<PivotRowDescriptor> Members
       
        #endregion
    }
}
