//-------------------------------------------------------------------------------------------------
// <copyright file="KPIElements.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace Syncfusion.OlapSilverlight.Base.Report
{
    [DataContract]
    public class KpiElements : Element
    {
        #region Constructor
        public KpiElements(AxisPosition axis)
        {
            this.Elements = new KpiElementCollection();
            this.Axis = axis;
        }

        public KpiElements()
        {
            this.Elements = new KpiElementCollection();
        }
        #endregion

        #region Public Methods
        [DataMember]
        public AxisPosition Axis { get; set; }

        [DataMember]
        public KpiElementCollection Elements { get; set; }

        public void Add(string name, bool kpiValue, bool kpiGoal, bool kpiStatus, bool kpiTrend)
        {
            this.Elements.Add(new KpiElement
            {
                Name = name,
                ShowKPIGoal = kpiGoal,
                ShowKPITrend = kpiTrend,
                ShowKPIStatus = kpiStatus,
                ShowKPIValue = kpiValue
            });
        }

        public void Add(string name)
        {
            this.Add(name, true, true, true, true);
        }

        #endregion
    }
}
