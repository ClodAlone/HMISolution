//-------------------------------------------------------------------------------------------------
// <copyright file="SortElement.cs" company="syncfusion">
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
    public class SortElement : Element
    {
        #region Constructor
        public SortElement(AxisPosition axis, SortOrder orderBy, bool isSortOn)
        {
            this.Axis = axis;
            this.SortOrder = orderBy;
            this.IsSortOn = isSortOn;
            this.Element = new MeasureElement();
        }

        public SortElement()
        {
        }
        #endregion

        #region Public Properties
        public AxisPosition Axis { get; set; }
        public MeasureElement Element { get; set; }
        public bool IsSortOn { get; set; }
        public SortOrder SortOrder { get; set; }
        #endregion

    }
}
