//-------------------------------------------------------------------------------------------------
// <copyright file="FilterElements.cs" company="syncfusion">
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
using System.Runtime.Serialization;

namespace Syncfusion.OlapSilverlight.Base.Report
{
    [DataContract]
    public class FilterElement : Element
    {
        #region Public Methods
        public FilterElement(AxisPosition Axis)
        {
            this.Elements = new ElementCollection();
            this.FilterValue = new ElementCollection();
            this.Axis = Axis;
            this.IsFilterCondition = false;
        }

        public FilterElement()
        {
            this.Elements = new ElementCollection();
            this.FilterValue = new ElementCollection();
            this.Axis = AxisPosition.Series;
            this.IsFilterCondition = false;
        }

        [DataMember]
        public AxisPosition Axis { get; set; }

        [DataMember]
        public ElementCollection Elements { get; set; }

        [DataMember]
        public FilterCase FilterCase { get; set; }

        [DataMember]
        public ElementCollection FilterValue { get; set; }

        [DataMember]
        public bool IsFilterCondition { get; set; }

        #endregion
    }
}
