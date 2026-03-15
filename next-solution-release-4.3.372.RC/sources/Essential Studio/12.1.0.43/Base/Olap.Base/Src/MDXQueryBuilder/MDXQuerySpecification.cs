//-------------------------------------------------------------------------------------------------
// <copyright file="MDXQuerySpecification.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System.ComponentModel;



#if !SILVERLIGHT
using Syncfusion.Olap.Reports;
using Syncfusion.Olap.Manager;
namespace Syncfusion.Olap.MDXQueryBuilder
#else
using Syncfusion.OlapSilverlight.Reports;
using Syncfusion.OlapSilverlight.Manager;

namespace Syncfusion.OlapSilverlight.MDXQueryBuilder
#endif
{
    /// <summary>
    /// This class specifies the specification of building an MDX Query
    /// </summary>
    /// <remarks>
    /// TODO {Aggregates, Calculation}
    /// </remarks>
    public class MDXQuerySpecification
    {
        #region Private Variables
        Select _select;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MDXQuerySpecification"/> class.
        /// </summary>
        public MDXQuerySpecification()
        {
            this.Slicer = new Where(this);
            this.Select = new Select(this);
            this.Filter = new Where(this);
            this.With = new With(this);
            this.Page = new PagerOptions();
            this.IsPagingEnabled = false;
            this.ShowGrandTotal = true;
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the name of the cube.
        /// </summary>
        /// <value>The name of the cube.</value>
        [DefaultValue("")]
        public string CubeName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is KPI.
        /// </summary>
        /// <value><c>true</c> if this instance is KPI; otherwise, <c>false</c>.</value>
        [DefaultValue(true)]
        public bool IsKPI { get; set; }

        /// <summary>
        /// Gets or sets the KPI axis.
        /// </summary>
        /// <value>The KPI axis.</value>
        public AxisPosition KPIAxis { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show empty column data].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show empty column data]; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        public bool ShowEmptyColumnData { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [include empty data].
        /// </summary>
        /// <value><c>true</c> if [include empty data]; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]
        public bool ShowEmptyRowData { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show grand total].
        /// </summary>
        /// <value><c>true</c> if [show grand total]; otherwise, <c>false</c>.</value>
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

        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        /// <value>The filter.</value>
        public Where Filter { get; set; }

        /// <summary>
        /// Gets or sets the select clause.
        /// </summary>
        /// <value>The select.</value>
        public Select Select
        {
            get
            {
                return _select;
            }

            set
            {
                _select = value;
            }
        }

        /// <summary>
        /// Gets or sets the with clause.
        /// </summary>
        /// <value>The with.</value>
        public With With { get; set; }

        /// <summary>
        /// Gets or sets the page options.
        /// </summary>
        /// <value>The <see cref="PagerOptions"/> object.</value>
        public PagerOptions Page { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is paging enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is paging enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsPagingEnabled { get; set; }
        #endregion
    }
}
