// <copyright file="ParamsTable.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System.Collections.Generic;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Class to save deserialization info of object params.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public static class ParamsTable
    {
        /// <summary>
        /// Initializes the table of object parameters.
        /// </summary>
        public static Dictionary<object, object> Params = new Dictionary<object, object>();
    }
}