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
    public interface IPivotValueLableFilter
    {
        /// <summary>
        /// Specifies the first value
        /// </summary>
        string Value1 { get; }

        /// <summary>
        /// 
        /// </summary>
        string Value2 { get;}

        /// <summary>
        /// 
        /// </summary>
        IPivotField DataField { get; }

        /// <summary>
        /// 
        /// </summary>
        PivotFilterType Type { get; }
    }
}
