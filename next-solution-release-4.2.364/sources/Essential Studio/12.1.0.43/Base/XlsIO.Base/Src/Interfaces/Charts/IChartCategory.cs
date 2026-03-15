#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives
using System;
#endregion

namespace Syncfusion.XlsIO
{
    public interface IChartCategory : IParentApplication
    {
        #region Interface properties
        /// <summary>
        /// Represent the category Filter.
        /// </summary>
        bool IsFiltered { get; set; }
        /// <summary>
        /// Represent the category Name.read only
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Represent the category labels read only
        /// </summary>
        IRange CategoryLabel { get; }
        /// <summary>
        /// Represent the category values.read only
        /// </summary>
        IRange Values { get; }
        #endregion
    }
}
