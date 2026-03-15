//-------------------------------------------------------------------------------------------------
// <copyright file="KPIElementCollection.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Syncfusion.OlapSilverlight.Base.Report
{
    [CollectionDataContract]
    public class KpiElementCollection : Collection<KpiElement>
    {
        #region Public Methods
        public KpiElement this[int index]
        {
            get
            {
                return (KpiElement)base.Items[index];
            }

            set
            {
                base.Items[index] = value;
            }
        }

        public void Add(KpiElement kpiElement)
        {
            base.Items.Add(kpiElement);
        }

        public void Remove(KpiElement kpiElement)
        {
            base.Items.Remove(kpiElement);
        }
        #endregion
    }
}
