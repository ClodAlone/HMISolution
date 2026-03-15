// <copyright file="PseudoText3D.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Represents PseudoText3D
    /// </summary>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    internal class PseudoText3D
    {
        /// <summary>
        /// Initializes Location
        /// </summary>
        public Point3D Location = new Point3D();

        /// <summary>
        /// Initializes TextBlock
        /// </summary>
        public TextBlock TextBlock = new TextBlock();

        /// <summary>
        /// The Render method
        /// </summary>
        /// <param name="vp">The Viewport3D vp</param>
        /// <param name="dc">The DrawingContext dc</param>
        public void Render(Viewport3D vp, DrawingContext dc)
        {
            Point3D pt2 = vp.Camera.Transform.Transform(this.Location);
            VisualBrush vb = new VisualBrush(this.TextBlock);
            dc.DrawRectangle(vb, null, new Rect(pt2.X, pt2.Y, this.TextBlock.ActualWidth, this.TextBlock.ActualHeight));
        }
    }
}
