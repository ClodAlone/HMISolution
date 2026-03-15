#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.XlsIO
{
    /// <summary>
    /// Represents pivot field object.
    /// </summary>
    public interface IPivotFilters
    {
        /// <summary>
        /// Adds the page field filter
        /// </summary>
        /// <returns></returns>
        IPivotFilter Add();

        IPivotFilter this[ int index]{get;}

        /// <summary>
        /// Parent field of the filter
        /// </summary>
        IPivotField Parent { get; }

        /// <summary>
        /// Adds Value/Label based filter.
        /// </summary>
        /// <param name="filterType">Type of the filter</param>
        /// <param name="dataField">Data field to which filter is applied (Data Field must not be null for Value filter</param>
        /// <param name="Value1">Value 1 of the filter</param>
        /// <param name="Value2">Value 2 of the filter</param>
        /// <returns></returns>
        IPivotValueLableFilter Add(PivotFilterType filterType, IPivotField dataField, string Value1, string Value2);
    }
}
