//-------------------------------------------------------------------------------------------------
// <copyright file="MDXQuerySpecification.cs" company="syncfusion">
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
    /// This class specifies the specification of building an MDX Query
    /// </summary>
    /// <remarks>
    /// TODO {Aggeregates, Calculation}
    /// </remarks>
    public class MDXQuerySpecification : QuerySpecification
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MDXQuerySpecification"/> class.
        /// </summary>
        public MDXQuerySpecification()
        {
            this.Slicer = new Where(this);
            base.Select = new Select(this);
            base.Filter = new Where(this);
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the name of the cube.
        /// </summary>
        /// <value>The name of the cube.</value>
        [DefaultValue("")]
        public string CubeName { get; set; }

        [DefaultValue(true)]
        public bool IsKPI { get; set; }

        public AxisPosition KPIAxis { get; set; }

        [DefaultValue(false)]
        public bool ShowEmptyColumnData { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [inculude empty data].
        /// </summary>
        /// <value><c>true</c> if [inculude empty data]; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]
        public bool ShowEmptyRowData { get; set; }

        [DefaultValue(true)]
        public bool ShowGrandTotal { get; set; }

        /// <summary>
        /// Gets or sets the engine version.
        /// </summary>
        /// <value>The engine version.</value>
        [DefaultValue(QueryBuilderEngineVersions.Version1)]
        public QueryBuilderEngineVersions EngineVersion { get; set; }

        /// <summary>
        /// Gets or sets the Slicer 
        /// </summary>
        /// <value>The filter condition.</value>
        [DefaultValue((string)null)]
        public Where Slicer { get; set; }
        #endregion
    }
}
