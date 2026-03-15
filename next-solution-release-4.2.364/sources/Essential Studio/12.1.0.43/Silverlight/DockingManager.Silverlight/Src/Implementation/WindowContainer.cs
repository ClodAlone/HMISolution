#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the Window Container class.
    /// </summary>
    public class WindowContainer : Grid
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowContainer"/> class.
        /// </summary>
        public WindowContainer()
        {
            ExternalWindow = new List<Window>();
        }

        private DockingManager _dockingManager;
        /// <summary>
        /// Gets or sets the name of the container.
        /// </summary>
        /// <value>The name of the container.</value>
        protected internal string ContainerName
        {
            get;
            set;
        }

        /// <summary>
        /// Represents the Onapply value.
        /// </summary>
        protected internal bool OnApply = false;

        /// <summary>
        /// Represents the handled later value.
        /// </summary>
        protected internal bool handledLater = false;

        /// <summary>
        /// Gets or sets a value indicating whether [isstate trans invoke].
        /// </summary>
        /// <value><c>True</c> if [isstate trans invoke]; otherwise, <c>false</c>.</value>
        protected internal bool IsstateTransInvoke
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the external window.
        /// </summary>
        /// <value>The external window.</value>
        protected internal List<Window> ExternalWindow
        {
            get;
            set;
        }

        /// <summary>
        /// Represents the group.
        /// </summary>
        protected internal int _group;

        /// <summary>
        /// Gets or sets the DockManager .
        /// </summary>
        /// <value>The dock manager.</value>
        protected internal DockManager DockManager
        {
            get; 
            set;
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {  
            base.OnApplyTemplate();
        }

        /// <summary>
        /// Gets or sets the DockingManager.
        /// </summary>
        /// <value>The docking manager.</value>
        protected internal DockingManager DockingManager
        {
            get
            {
                return _dockingManager;
            }
            set
            {
                _dockingManager = value;
                this.Background = _dockingManager.Background;
            }
        }

        /// <summary>
        /// Gets or sets Window Object.
        /// </summary>
        /// <value>The _window.</value>
        internal Window _window
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the DockPosition.
        /// </summary>
        /// <value>The children position.</value>
        protected internal Dock ChildrenPosition
        {
            get;
            set;
        }
    }
}
