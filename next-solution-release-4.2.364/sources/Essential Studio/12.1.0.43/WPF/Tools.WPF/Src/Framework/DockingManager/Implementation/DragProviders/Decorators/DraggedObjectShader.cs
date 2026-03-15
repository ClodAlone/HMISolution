// <copyright file="DraggedObjectShader.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System.Windows;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <property name="flag" value="Finished" />
    /// <summary>
    /// Helper adorner that is used as the presenter of the shading
    /// above the dragged object.
    /// </summary>
    /// <exclude/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif

    public class DraggedObjectShader
        : TemplatedAdornerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DraggedObjectShader"/> class.
        /// </summary>
        /// <param name="shadedElement">The shaded element.</param>
        public DraggedObjectShader(UIElement shadedElement)
            : base(shadedElement)
        {
        }
    }
}
