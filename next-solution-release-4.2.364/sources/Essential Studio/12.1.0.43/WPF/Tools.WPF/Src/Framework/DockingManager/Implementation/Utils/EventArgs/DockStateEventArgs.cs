// <copyright file="DockStateEventArgs.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Windows;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Class used to pass new and old states of the docking target of
    /// the dock able element.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class DockStateEventArgs : EventArgs
    {
        #region Public members
        /// <summary>
        /// Specifies the old value of the state.
        /// </summary>
        public readonly DockState OldState;
        
        /// <summary>
        /// Specifies the new value of the state.
        /// </summary>
        public readonly DockState NewState;
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="DockStateEventArgs"/> class.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        public DockStateEventArgs(DockState oldValue, DockState newValue)
        {
            OldState = oldValue;
            NewState = newValue;
        }
        #endregion
    }

    /// <summary>
    /// Represents agument for DockWindowState event
    /// </summary>
    public class DockWindowStateEventArgs : EventArgs
    {
        #region Public members
        /// <summary>
        /// Specifies the old value of the state.
        /// </summary>
        public readonly WindowState OldState;

        /// <summary>
        /// Specifies the new value of the state.
        /// </summary>
        public readonly WindowState NewState;
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="DockStateEventArgs"/> class.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        public DockWindowStateEventArgs(WindowState oldValue, WindowState newValue)
        {
            OldState = oldValue;
            NewState = newValue;
        }
        #endregion
    }

    /// <summary>
    /// Represents agument for ActiveWindowChanging event
    /// </summary>
    public class ActiveWindowChangingEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ActiveWindowChangingEventArgs"/> is cancel.
        /// </summary>
        /// <value><c>true</c> if cancel; otherwise, <c>false</c>.</value>
        public bool Cancel
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the old value.
        /// </summary>
        /// <value>The old value.</value>
        public FrameworkElement OldValue
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the new value.
        /// </summary>
        /// <value>The new value.</value>
        public FrameworkElement NewValue
        {
            get;
            internal set;
        }
    }

    /// <summary>
    /// Represents argument of DockStateChanging event
    /// </summary>
    public class DockStateChangingEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the target element.
        /// </summary>
        /// <value>The target element.</value>
        public FrameworkElement TargetElement
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the source element.
        /// </summary>
        /// <value>The source element.</value>
        public FrameworkElement SourceElement
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the state of the target.
        /// </summary>
        /// <value>The state of the target.</value>
        public DockState TargetState
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the state of the present.
        /// </summary>
        /// <value>The state of the present.</value>
        public DockState PresentState
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="DockStateChangingEventArgs"/> is cancel.
        /// </summary>
        /// <value><c>true</c> if cancel; otherwise, <c>false</c>.</value>
        public bool Cancel
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the target side.
        /// </summary>
        /// <value>The target side.</value>
        public DockSide TargetSide
        {
            get;
            internal set;
        }
    }

    /// <summary>
    /// Represents arguments of DockPreviewOpening event args.
    /// </summary>
    public class DockProviderShownEventArgs :EventArgs
    {
        /// <summary>
        /// Gets or sets the target element.
        /// </summary>
        /// <value>The target element.</value>
        public FrameworkElement TargetElement
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the source element.
        /// </summary>
        /// <value>The source element.</value>
        public FrameworkElement SourceElement
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the state of the present.
        /// </summary>
        /// <value>The state of the present.</value>
        public DockState TargetState
        {
            get;
            internal set;
        }
    }
}
