#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using Syncfusion.Windows.Forms.Diagram;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Helper class that stores connections.
    /// </summary>
    /// <remarks>
    /// Used with Grouping or UnGrouping 
    /// </remarks>
    internal class EndPointConnection
    {
        /// <summary>
        /// Connection point.
        /// </summary>
        public ConnectionPoint Port;

        /// <summary>
        /// Connection Point connections.
        /// </summary>
        public EndPointCollection EndPoints;
    }
}
