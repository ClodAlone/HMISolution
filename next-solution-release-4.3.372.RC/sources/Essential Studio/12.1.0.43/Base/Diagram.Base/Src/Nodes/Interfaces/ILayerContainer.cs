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
    /// Interface to objects that contain a collection of layers.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This interface provides methods for managing nodes across a collection
    /// of layers.
    /// </para>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Layer"/>
    /// </remarks>
    public interface ILayerContainer
    {
        /// <summary>
        /// Gets typed collection of layers.
        /// </summary>
        LayerCollection Layers
        {
            get;
        }

        /// <summary>
        /// Gets collection of active layers.
        /// </summary>
        /// <remark>
        /// Adding new node to ILayerContainer would add it to all layers 
        /// contained in this collection
        /// </remark>
        LayerCollection ActiveLayers
        {
            get;
        }
    }
}
