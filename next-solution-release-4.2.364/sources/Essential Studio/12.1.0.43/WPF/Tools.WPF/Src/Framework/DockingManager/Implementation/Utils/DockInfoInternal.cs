// <copyright file="DockInfoInternal.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents DockInfoInternal class. Is used to store information about <see cref="DockingManager"/> hosts.
    /// </summary>
    /// <exclude/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public sealed class DockInfoInternal : IDisposable
    {
        #region Private members
        /// <summary>
        /// Specifies the docking manager.
        /// </summary>
        private DockingManager m_dockingManager;

        /// <summary>
        /// Specifies the docked element tabbed host.
        /// </summary>
        private DockedElementTabbedHost m_hostDock;

        /// <summary>
        /// Specifies the docked element tab host for float.
        /// </summary>
        private DockedElementTabbedHost m_hostFloat;

        /// <summary>
        /// Specifies the float window.
        /// </summary>
        private IWindow m_floatWindow;

        /// <summary>
        /// Specifies the native Float window
        /// </summary>
        private NativeFloatWindow m_NativeFloatWindow;
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="DockInfoInternal"/> class.
        /// </summary>
        /// <param name="dockingManager">The docking manager.</param>
        public DockInfoInternal(DockingManager dockingManager)
        {
            m_dockingManager = dockingManager;
        }

        /// <summary>
        /// Releases all resources used by the DockInfoInternal.
        /// </summary>
        public void Dispose()
        {
            m_dockingManager = null;
            m_hostDock = null;
            m_hostFloat = null;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets DockingManager of the <see cref="DockInfoInternal"/>.
        /// </summary>
        public DockingManager DockingManager
        {
            get
            {
                return m_dockingManager;
            }
        }

        /// <summary>
        /// Gets HostDock of the <see cref="DockInfoInternal"/>.
        /// </summary>
        public DockedElementTabbedHost HostDock
        {
            get
            {
                return m_hostDock;
            }

            internal set
            {
                m_hostDock = value;
            }
        }

        /// <summary>
        /// Gets HostFloat of the <see cref="DockInfoInternal"/>.
        /// </summary>
        public DockedElementTabbedHost HostFloat
        {
            get
            {
                return m_hostFloat;
            }

            internal set
            {
                m_hostFloat = value;
            }
        }

        /// <summary>
        /// Gets or sets NativeFloatingWindow of the <see cref="DockInfoInternal"/>.
        /// </summary>
        internal NativeFloatWindow NativeWindow
        {
            get
            {
                return m_NativeFloatWindow; ;
            }

            set
            {
                m_NativeFloatWindow = value;
            }
        }

        /// <summary>
        /// Gets or sets FloatingWindow of the <see cref="DockInfoInternal"/>.
        /// </summary>
        public IWindow FloatingWindow
        {
            get
            {
                return m_floatWindow;
            }

            set
            {
                m_floatWindow = value;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Gets host associated with the specified state.
        /// </summary>
        /// <param name="state">State, the host should be got for. Valid
        /// values are Dock and Float.</param>
        /// <returns>
        /// Returns the associated host or null if there is no host
        /// associated with the specified state or the state is invalid.
        /// </returns>
        /// <property name="flag" value="Finished"/>
        public DockedElementTabbedHost GetHost(DockState state)
        {
            DockedElementTabbedHost result = null;

            switch (state)
            {
                case DockState.Dock:
                    result = HostDock;
                    break;
                case DockState.Float:
                    result = HostFloat;
                    break;
            }

            return result;
        }
        #endregion
    }
}
