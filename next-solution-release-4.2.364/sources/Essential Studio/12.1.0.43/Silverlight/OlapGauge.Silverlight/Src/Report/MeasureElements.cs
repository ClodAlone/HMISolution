//-------------------------------------------------------------------------------------------------
// <copyright file="MeasureElements.cs" company="syncfusion">
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

namespace Syncfusion.OlapSilverlight.Base.Report
{
    public class MeasureElements : Element
    {
        #region Constructor
        public MeasureElements(AxisPosition axis)
        {
            Axis = axis;
            Elements = new MeasureElementCollection();
        }

        public MeasureElements()
        {
            Elements = new MeasureElementCollection();
        }
        #endregion

        #region Public Properties
        public AxisPosition Axis { get; set; }

        public MeasureElementCollection Elements { get; set; }
        #endregion

        #region Public Methods
        public void Add(string measureName)
        {
            this.Elements.Add(new MeasureElement { Name = measureName });
        }

        #endregion
    }
}
