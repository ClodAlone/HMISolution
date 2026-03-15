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
    /// Service status enumeration.
    /// </summary>
    public enum ServiceStatus
    {
        /// <summary>
        /// Indicates that service is stopped
        /// </summary>
        Stopped = 0,

        /// <summary>
        /// Indicates that service is running
        /// </summary>
        Started,

        /// <summary>
        /// Indicates that service is paused
        /// </summary>
        Paused,

        /// <summary>
        /// Indicates that service is resumed
        /// </summary>
        Resumed
    }

    /// <summary>
    /// Flip axis enumeration.
    /// </summary>
    public enum FlipAxis
    {
        /// <summary>
        /// X Axis 
        /// </summary>
        AxisX,

        /// <summary>
        /// Y Axis
        /// </summary>
        AxisY
    }
}
