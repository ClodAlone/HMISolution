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
    /// End point container interface.
    /// </summary>
    public interface IEndPointContainer
    {
        /// <summary>
        /// Gets the head end point handle.
        /// </summary>
        /// <value>The head end point.</value>
        EndPoint HeadEndPoint { get; }

        /// <summary>
        /// Gets the tail end point handle.
        /// </summary>
        /// <value>The tail end point.</value>
        EndPoint TailEndPoint { get; }
    }
}
