#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Flags that indicates state of connection.
    /// </summary>
    /// <remarks>
    /// Primarily used by tools to indicate whether cloned orthogonal node
    /// can merge its control points and whether its endPoints are connected.
    /// </remarks>
    [Flags]
    public enum ConnectorState
    {
        /// <summary>
        /// Indicates the default behavior.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Indicates whether control points will be merged if they can.
        /// </summary>
        MergeControlPoints = 1,

        /// <summary>
        /// Indicates whether HeadEndPoint is connected.
        /// </summary>
        HeadEndPointConnected = 2,

        /// <summary>
        /// Indicates whether TailEndPoint is connected.
        /// </summary>
        TailEndPointConnected = 4
    }

    /// <summary>
    /// Bridge styles to render.
    /// </summary>
    public enum BridgeStyle
    {
        /// <summary>
        /// Draw arc.
        /// </summary>
        Arc = 0,

        /// <summary>
        /// Not render.
        /// </summary>
        Gap,

        /// <summary>
        /// Draw square.
        /// </summary>
        Square,

        /// <summary>
        /// Draw two lines.
        /// </summary>
        Sides2,

        /// <summary>
        /// Draw tree lines.
        /// </summary>
        Sides3,

        /// <summary>
        /// Draw four lines.
        /// </summary>
        Sides4,

        /// <summary>
        /// Draw five lines.
        /// </summary>
        Sides5,

        /// <summary>
        /// Draw six lines.
        /// </summary>
        Sides6,

        /// <summary>
        /// Draw seven lines.
        /// </summary>
        Sides7
    }
}
