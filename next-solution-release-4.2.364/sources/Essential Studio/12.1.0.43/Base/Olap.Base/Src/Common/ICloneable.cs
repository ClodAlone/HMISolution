//-------------------------------------------------------------------------------------------------
// <copyright file="ICloneable.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------


namespace Syncfusion.Olap.Common
{
    /// <summary>
    /// Generic interface will clone the object.
    /// </summary>
    /// <typeparam name="T">Type of ICloneable interface</typeparam>
    public interface ICloneable<T>
    {
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        T Clone();
    }
}
