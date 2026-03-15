//-------------------------------------------------------------------------------------------------
// <copyright file="Select.cs" company="syncfusion">
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
namespace Syncfusion.Olap.MDXQueryBuilder
#else
using Syncfusion.OlapSilverlight.Reports;
namespace Syncfusion.OlapSilverlight.MDXQueryBuilder
#endif
{
    /// <summary>
    /// A Collection of elements that need to be retrieved form the data source.
    /// </summary>
    public class Select
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Select"/> class.
        /// </summary>
        /// <param name="querySpecification">The query specification.</param>
        public Select(MDXQuerySpecification querySpecification)
        {
            Items = new Items();
            this.QuerySpecification = querySpecification;
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
        /// Gets or sets the query specification.
        /// </summary>
        /// <value>The query specification.</value>
        [DefaultValue((string)null)]
        public MDXQuerySpecification QuerySpecification { get; set; }
        #endregion
    }
}
