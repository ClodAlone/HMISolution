// <copyright file="HostAdornerVS2003.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents docking manager's host adorner for VS2003.
    /// </summary>
    /// <exclude/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class HostAdornerVS2003 : TemplatedAdornerBase
    {
        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="HostAdornerVS2003"/> class.
        /// </summary>
        /// <param name="element">The element.</param>
        public HostAdornerVS2003(UIElement element)
            : base(element)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets Side of the <see cref="HostAdornerVS2003"/>. This is a dependency property.
        /// </summary>
        public Dock Side
        {
            get
            {
                return (Dock)GetValue(SideProperty);
            }

            set
            {
                SetValue(SideProperty, value);
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Sets side of the dock window.
        /// </summary>
        /// <param name="side">new side to set</param>
        internal void SetDockSide(DockSide side)
        {
            switch (side)
            {
                case DockSide.Left:
                    Side = Dock.Left;
                    break;

                case DockSide.Right:
                    Side = Dock.Right;
                    break;

                case DockSide.Top:
                    Side = Dock.Top;
                    break;

                case DockSide.Bottom:
                    Side = Dock.Bottom;
                    break;

                default:
                    throw new NotSupportedException();
            }
        }
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies HostAdornerVS2003.Side dependency property.
        /// </summary>
        public static readonly DependencyProperty SideProperty =
            DependencyProperty.Register("Side", typeof(Dock), typeof(HostAdornerVS2003), new FrameworkPropertyMetadata(Dock.Left, FrameworkPropertyMetadataOptions.AffectsArrange));
        #endregion
    }
}
