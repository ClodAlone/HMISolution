//-------------------------------------------------------------------------------------------------
// <copyright file="Select.cs" company="syncfusion">
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

namespace Syncfusion.OlapSilverlight.Base.Report
{
    /// <summary>
    /// A Collection of elements that need to be retrieved form the datasource
    /// </summary>
    public class Select
    {
        #region Constructor
        public Select(MDXQuerySpecification querySpecificaion)
        {
            Items = new Items();
            this.QuerySpecificaion = querySpecificaion;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Collection of elements that need to retrieved
        /// </summary>
        /// <value>The items.</value>
        [DefaultValue((string)null)]
        public Items Items { get; private set; }

        /// <summary>
        /// Gets or sets the query specificaion.
        /// </summary>
        /// <value>The query specificaion.</value>
        [DefaultValue((string)null)]
        public MDXQuerySpecification QuerySpecificaion { get; set; }
        #endregion
    }
}
