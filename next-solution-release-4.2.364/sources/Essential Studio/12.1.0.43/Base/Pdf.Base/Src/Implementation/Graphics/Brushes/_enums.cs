#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;

namespace Syncfusion.Pdf.Graphics
{
    /// <summary>
    /// Specifies the gradient direction of the linear gradient brush.
    /// </summary>
    public enum PdfLinearGradientMode
    {
        /// <summary>
        /// Specifies a gradient from upper right to lower left.
        /// </summary>
        BackwardDiagonal,

        /// <summary>
        /// Specifies a gradient from upper left to lower right.
        /// </summary>
        ForwardDiagonal,

        /// <summary>
        /// Specifies a gradient from left to right.
        /// </summary>
        Horizontal,

        /// <summary>
        /// Specifies a gradient from top to bottom.
        /// </summary>
        Vertical,
    }

    /// <summary>
    /// Specifies the constant values specifying whether to extend the shading
    /// beyond the starting and ending points of the axis.
    /// </summary>
    [Flags]
    public enum PdfExtend
    {
        /// <summary>
        /// Do not extend any point.
        /// </summary>
        None = 0,

        /// <summary>
        /// Extend start point.
        /// </summary>
        Start = 1,

        /// <summary>
        /// Extend end point.
        /// </summary>
        End = 2,

        /// <summary>
        /// Extend both start and end points.
        /// </summary>
        Both = Start | End,
    }

    /// <summary>
    /// Shading type constants.
    /// </summary>
    internal enum ShadingType
    {
        /// <summary>
        /// Function-based shading.
        /// </summary>
        Function = 1,

        /// <summary>
        /// Axial shading.
        /// </summary>
        Axial = 2,

        /// <summary>
        /// Radial shading.
        /// </summary>
        Radial = 3,

        /// <summary>
        /// Free-form Gouraud-shaded triangle mesh
        /// </summary>
        FreeForm = 4,

        /// <summary>
        /// Lattice-form Gouraud-shaded triangle mesh.
        /// </summary>
        LatticeForm = 5,

        /// <summary>
        /// Coons patch mesh.
        /// </summary>
        Coons = 6,

        /// <summary>
        /// Tensor-product patch mesh.
        /// </summary>
        Tensor = 7,
    }
}
