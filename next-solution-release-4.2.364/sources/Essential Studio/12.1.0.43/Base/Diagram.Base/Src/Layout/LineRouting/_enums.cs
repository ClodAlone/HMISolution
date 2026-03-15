#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Routing mode for line routing engine.
    /// </summary>
    public enum RoutingMode
    {
        /// <summary>
        /// Defines that line routing engine
        /// will skip all document changes it is attached to.
        /// </summary>
        Inactive = 0,

        /// <summary>
        /// Defines that line routing engine
        /// will reroute all connectors in document.
        /// </summary>
        Automatic,

        /// <summary>
        /// Defines that line routing engine will
        /// reroute currently moved node connector's only.
        /// </summary>
        SemiAutomatic
    }
}
