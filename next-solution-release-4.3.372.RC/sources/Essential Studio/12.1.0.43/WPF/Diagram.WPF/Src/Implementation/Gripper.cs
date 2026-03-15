// <copyright file="Gripper.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.ComponentModel;

namespace Syncfusion.Windows.Diagram
{
    /// <summary>
    /// Represents the Thumb used for dragging the <see cref="Node"/>. This is essentially useful when the node content is set to be HitTestVisible and due to which it becomes difficult to drag the <see cref="Node"/>.
    /// </summary>
#if !SyncfusionFramework3_5
    [DesignTimeVisible(false)]
#endif
    public class Gripper : DragProvider
    {
        #region Class fields

        /// <summary>
        /// Used to store the diagram control instance.
        /// </summary>
        private DiagramControl dc;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="Gripper"/> class.
        /// </summary>
        public Gripper()
        {
        }
       
        #endregion

        #region Overrides

        /// <summary>
        /// Raises the <see cref="OnInitialized"/> event. 
        /// This method is invoked whenever <see cref="OnInitialized"/> property is set to true internally. 
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            dc = DiagramPage.GetDiagramControl((FrameworkElement)this);
            base.OnInitialized(e);
        }
        
        #endregion
    }
}
