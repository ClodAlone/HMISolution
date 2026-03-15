//-------------------------------------------------------------------------------------------------
// <copyright file="Where.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------


#if !SILVERLIGHT
using Syncfusion.Olap.Reports;
using System.ComponentModel;
namespace Syncfusion.Olap.MDXQueryBuilder
#else
using Syncfusion.OlapSilverlight.Reports;
using System.ComponentModel;
namespace Syncfusion.OlapSilverlight.MDXQueryBuilder
#endif
{
    /// <summary>
    /// With clause specification.
    /// </summary>
    public class With
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Where"/> class.
        /// </summary>
        /// <param name="querySpecification">The query specification.</param>
        public With(MDXQuerySpecification querySpecification)
        {
            this.QuerySpecification = querySpecification;
            this.Items = new Items();
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Elements that need to filtered from the query
        /// </summary>
        /// <value>The items.</value>
        [DefaultValue((string)null)]
        public Items Items { get; set; }

        /// <summary>
        /// Gets or sets the query specification.
        /// </summary>
        /// <value>The query specification.</value>
        [DefaultValue((string)null)]
        public MDXQuerySpecification QuerySpecification { get; set; }
        #endregion
    }
}
