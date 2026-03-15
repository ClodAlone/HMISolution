//-------------------------------------------------------------------------------------------------
// <copyright file="Item.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace Syncfusion.OlapSilverlight.Base.Report
{
    /// <summary>
    /// Parameters of select command
    /// </summary>
    [DataContract]
    public class Item
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class.
        /// </summary>
        public Item()
        {
            this.IsFilterOrSortOn = false;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the axis.
        /// </summary>
        /// <value>The axis position</value>
        [DataMember, DefaultValue(0)]
        public AxisPosition Axis { get; set; }

        /// <summary>
        /// Gets or sets the element value.
        /// </summary>
        /// <value>The element value of Item</value>
        [DataMember, DefaultValue((string)null)]
        public Element ElementValue { get; set; }

        [DataMember, DefaultValue(false)]
        public bool IsFilterOrSortOn { get; set; }
        #endregion

    }
}
