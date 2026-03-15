// <copyright file="DockingParams.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Is used for docking params serialization.
    /// </summary>
    /// <exclude/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [Serializable]
    public class DockingParams : ISerializable
    {
        #region Private members


        private bool m_IsSwapped = false;

        private int m_taborderindex = -1;

        /// <summary>
        /// element to store parameters.
        /// </summary>
        private readonly FrameworkElement m_element;

        /// <summary>
        /// presents table of docking parameters.
        /// </summary>
        private readonly Hashtable m_dockParamsTable;

        /// <summary>
        /// element to store state value.
        /// </summary>
        private DockState m_State = DockState.Dock;

        /// <summary>
        /// element to store NoHeader value.
        /// </summary>
        private bool m_NoHeader = false;

        /// <summary>
        /// element to store SideDocked value.
        /// </summary>
        private DockSide m_SideDocked = DockSide.Left;

        /// <summary>
        /// element to store SideFloating value.
        /// </summary>
        private DockSide m_SideFloating = DockSide.Left;

        /// <summary>
        /// element to store Name value.
        /// </summary>
        private string m_Name = string.Empty;

        /// <summary>
        /// element to store TargetDocked value.
        /// </summary>
        private string m_TargetDocked = string.Empty;

        /// <summary>
        /// element to store TargetFloating value.
        /// </summary>
        private string m_TargetFloating = string.Empty;

        /// <summary>
        /// element to store DesiredWidthInDockedMode value.
        /// </summary>
        private double m_DesiredWidthInDockedMode = 90d;

        /// <summary>
        /// element to store DesiredHeightInDockedMode value.
        /// </summary>
        private double m_DesiredHeightInDockedMode = 90d;

        /// <summary>
        /// element to store DesiredWidthInFloatingMode value.
        /// </summary>
        private double m_DesiredWidthInFloatingMode = 90d;

        /// <summary>
        /// element to store DesiredHeightInFloatingMode value.
        /// </summary>
        private double m_DesiredHeightInFloatingMode = 90d;

        /// <summary>
        /// element to store NoDock value.
        /// </summary>
        private bool m_NoDock = false;

        /// <summary>
        /// element to store CanDock value.
        /// </summary>
        private bool m_CanDock = true;

        /// <summary>
        /// element to store CanClose value.
        /// </summary>
        private bool m_CanClose = true;

        /// <summary>
        /// element to store IsSelectedTab value.
        /// </summary>
        private bool m_IsSelectedTab = false;

        /// <summary>
        /// element to store IsActiveWindow value.
        /// </summary>
        private bool m_IsActiveWindow = false;

        /// <summary>
        /// element to store WindowRect value.
        /// </summary>
        private string m_WindowRect = string.Empty;

        /// <summary>
        /// element to store TabGroupName value.
        /// </summary>
        private string m_TabGroupName = string.Empty;

        /// <summary>
        /// element to store IsTabGroupOwner value.
        /// </summary>
        private bool m_IsTabGroupOwner = false;

        /// <summary>
        /// element to store SideTabOrder value.
        /// </summary>
        private int m_SideTabOrder = 0;

        /// <summary>
        /// element to store IndexInDockMode value.
        /// </summary>
        private int m_IndexInDockMode = 0;

        /// <summary>
        /// element to store IndexInFloatMode value.
        /// </summary>
        private int m_IndexInFloatMode = 0;

        /// <summary>
        /// element to store TabOrderInDockMode value.
        /// </summary>
        private int m_TabOrderInDockMode = 0;

        /// <summary>
        /// element to store TabOrderInFloatMode value.
        /// </summary>
        private int m_TabOrderInFloatMode = 0;

        /// <summary>
        /// element to store MDIBounds value.
        /// </summary>
        private string m_MDIBounds = string.Empty;

        /// <summary>
        /// element to store TDIIndex value.
        /// </summary>
        private int m_TDIIndex = -1;

        /// <summary>
        /// element to store IsSelected value.
        /// </summary>
        private bool m_IsSelected = false;

        /// <summary>
        /// element to store MDIMinimizedBounds value.
        /// </summary>
        private string m_MDIMinimizedBounds = string.Empty;

        /// <summary>
        /// element to store MDIWindowState value.
        /// </summary>
        private MDIWindowState m_MDIWindowState = MDIWindowState.Normal;

        /// <summary>
        /// element to store AllowMDIResize value.
        /// </summary>
        private bool m_AllowMDIResize = true;

        /// <summary>
        /// element to store TDIGroupOrientation value.
        /// </summary>
        private Orientation m_TDIGroupOrientation = Orientation.Horizontal;

        /// <summary>
        /// element to store WayOfTDIGroup value.
        /// </summary>
        private string m_WayOfTDIGroup = string.Empty;

        /// <summary>
        /// element to store DockForSide value.
        /// </summary>
        private Dock m_DockForSide = Dock.Left;

        /// <summary>
        /// element to store PreviousIndexInDockMode value.
        /// </summary>
        private int m_PreviousIndexInDockMode = 0;

        /// <summary>
        /// element to store PreviousChildElements value.
        /// </summary>
        private List<string> m_PreviousChildElements = new List<string>();

        /// <summary>
        /// element to store PreviousSideInDockMode value.
        /// </summary>
        private DockSide m_PreviousSideInDockMode = DockSide.Left;

        /// <summary>
        /// element to store DoShift value.
        /// </summary>
        private bool m_DoShift = true;

        /// <summary>
        /// element to store SideRelativetoContainer value.
        /// </summary>
        private DockSide m_SideRelativetoContainer = DockSide.Left;

        /// <summary>
        /// element to store TabParent value.
        /// </summary>
        private string m_TabParent = string.Empty;

        /// <summary>
        /// element to store DockWindowState value.
        /// </summary>
        private WindowState m_DockWindowState = WindowState.Normal;

        /// <summary>
        /// element to store CanMaximize value.
        /// </summary>
        private bool m_CanMaximize = true;

        /// <summary>
        /// element to store CanMinimize value.
        /// </summary>
        private bool m_CanMinimize = true;

        /// <summary>
        /// element to store CanResizeInDockedState value.
        /// </summary>
        private bool m_CanResizeInDockedState = true;

        /// <summary>
        /// element to store CanResizeHeightInDockedState value.
        /// </summary>
        private bool m_CanResizeHeightInDockedState = true;

        /// <summary>
        /// element to store CanResizeWidthInDockedState value.
        /// </summary>
        private bool m_CanResizeWidthInDockedState = true;

        /// <summary>
        /// element to store CanResizeHeightInFloatState value.
        /// </summary>
        private bool m_CanResizeHeightInFloatState = true;

        /// <summary>
        /// element to store CanResizeWidthInFloatState value.
        /// </summary>
        private bool m_CanResizeWidthInFloatState = true;

        /// <summary>
        /// element to store CanResizeInFloatState value.
        /// </summary>
        private bool m_CanResizeInFloatState = true;

        private bool m_CanFloatMaximize = false;

        /// <summary>
        /// element to store IsFixedSize value.
        /// </summary>
        private bool m_IsFixedSize = false;

        /// <summary>
        /// element to store IsFixedHeight value.
        /// </summary>
        private bool m_IsFixedHeight = false;

        /// <summary>
        /// element to store IsFixedWidth value.
        /// </summary>
        private bool m_IsFixedWidth = false;

        /// <summary>
        /// element to store FixedHeight value.
        /// </summary>
        private double m_FixedHeight = 0d;

        /// <summary>
        /// element to store FixedWidth value.
        /// </summary>
        private double m_FixedWidth = 0d;

        /// <summary>
        /// element to store DockedElementsContainer size.
        /// </summary>
        private Size m_ContainerSize = new Size(0,0);

        /// <summary>
        /// element to store PreviousHostWidth value.
        /// </summary>
        private double m_PreviousHostWidth = 90d;

        /// <summary>
        /// element to store PreviousHostHeight value.
        /// </summary>
        private double m_PreviousHostHeight = 90d;

        /// <summary>
        /// element to store PreviousContainerHeight value.
        /// </summary>
        private double m_PreviousContainerHeight = 90d;

        /// <summary>
        /// element to store PreviousContainerWidth value.
        /// </summary>
        private double m_PreviousContainerWidth = 90d;

        /// <summary>
        /// element to store TargetAutoHide value.
        /// </summary>
        private string m_TargetAutoHide = string.Empty;

        private double m_PreviousDesiredWidthInDockedMode = 0;

        private double m_PreviousDesiredHeightInDockedMode = 0;

        private Size m_PreviousContainerDesiredSize = new Size(0, 0);

        private double m_offset = 0.0;

        private int m_ZorderInFloatMode = 0;

         private WindowState m_windowstate = WindowState.Normal;

        private string previousFloatingWindowRect = string.Empty;

        #endregion 

        #region Public properties
        public string PreviousFloatingWindowRect
        {
            get
            {
                return previousFloatingWindowRect;
            }
            set
            {
                previousFloatingWindowRect = value;
        }
            }
        public WindowState FloatWindowState
        {
            get
            {
                return m_windowstate;
            }
            set
            {
                m_windowstate = value;
            }
        }
        public int ZorderInFloatMode
        {
            get { return m_ZorderInFloatMode; }
            set { m_ZorderInFloatMode = value; }
        }

        public int TabOrderIndex
        {
            get
            {
                return m_taborderindex;
            }
            set
            {
                m_taborderindex = value;
            }
        }

        public Size PreviousContainerDesiredSize
        {
            get
            {
                return m_PreviousContainerDesiredSize;
            }
            set
            {
                m_PreviousContainerDesiredSize = value;
            }
        }


        public double PreviousDesiredWidthInDockedMode
        {
            get
            {
                return m_PreviousDesiredWidthInDockedMode;
            }
            set
            {
                m_PreviousDesiredWidthInDockedMode = value;
            }
        }

        public double PreviousDesiredHeightInDockedMode
        {
            get
            {
                return m_PreviousDesiredHeightInDockedMode;
            }
            set
            {
                m_PreviousDesiredHeightInDockedMode = value;
            }
        }


        /// <summary>
        /// Gets or sets name of the docking window.
        /// </summary>
        public string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                m_Name = value;
            }
        }

        /// <summary>
        /// Gets or sets state of the docking window.
        /// </summary>
        public DockState State
        {
            get
            {
                return m_State;
            }
            set
            {
                m_State = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [no header].
        /// </summary>
        /// <value><c>true</c> if [no header]; otherwise, <c>false</c>.</value>
        public bool NoHeader
        {
            get
            {
                return m_NoHeader;
            }
            set
            {
                m_NoHeader = value;
            }
        }

        /// <summary>
        /// Gets or sets side of the parent window to dock current docking window.
        /// </summary>
        public DockSide SideDocked
        {
            get
            {
                return m_SideDocked;
            }
            set
            {
                m_SideDocked = value;
            }
        }

        /// <summary>
        /// Gets or sets side of the parent window to dock current floating window.
        /// </summary>
        public DockSide SideFloating
        {
            get
            {
                return m_SideFloating;
            }
            set
            {
                m_SideFloating = value;
            }
        }

        /// <summary>
        /// Gets or sets target name in dock mode
        /// </summary>
        public string TargetDocked
        {
            get
            {
                return m_TargetDocked;
            }
            set
            {
                m_TargetDocked = value;
            }
        }

        /// <summary>
        /// Gets or sets target name in floating mode
        /// </summary>
        public string TargetFloating
        {
            get
            {
                return m_TargetFloating;
            }
            set
            {
                m_TargetFloating = value;
            }
        }

        /// <summary>
        /// Gets or sets desired width of the window in dock mode.
        /// </summary>
        public double DesiredWidthInDockedMode
        {
            get
            {
                return m_DesiredWidthInDockedMode;
            }
            set
            {
                m_DesiredWidthInDockedMode = value;
            }
        }

        /// <summary>
        /// Gets or sets desired height of the window in dock mode.
        /// </summary>
        public double DesiredHeightInDockedMode
        {
            get
            {
                return m_DesiredHeightInDockedMode;
            }
            set
            {
                m_DesiredHeightInDockedMode = value;
            }
        }

        /// <summary>
        /// Gets or sets desired width of the window in floating mode.
        /// </summary>
        public double DesiredWidthInFloatingMode
        {
            get
            {
                return m_DesiredWidthInFloatingMode;
            }
            set
            {
                m_DesiredWidthInFloatingMode = value;
            }
        }

        /// <summary>
        /// Gets or sets desired height of the window in floating mode.
        /// </summary>
        public double DesiredHeightInFloatingMode
        {
            get
            {
                return m_DesiredHeightInFloatingMode;
            }
            set
            {
                m_DesiredHeightInFloatingMode = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether if window can dock to some panel.
        /// </summary>
        public bool NoDock
        {
            get
            {
                return m_NoDock;
            }
            set
            {
                m_NoDock = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether if window can be in dock state.
        /// </summary>
        public bool CanDock
        {
            get
            {
                return m_CanDock;
            }
            set
            {
                m_CanDock = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can close.
        /// </summary>
        /// <value><c>true</c> if this instance can close; otherwise, <c>false</c>.</value>
        public bool CanClose
        {
            get
            {
                return m_CanClose;
            }
            set
            {
                m_CanClose = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether tab is selected.
        /// </summary>
        public bool IsSelectedTab
        {
            get
            {
                return m_IsSelectedTab;
            }
            set
            {
                m_IsSelectedTab = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether window is active.
        /// </summary>
        public bool IsActiveWindow
        {
            get
            {
                return m_IsActiveWindow;
            }
            set
            {
                m_IsActiveWindow = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the rectangle of the window.
        /// </summary>
        public string WindowRect
        {
            get
            {
                return m_WindowRect;
            }
            set
            {
                m_WindowRect = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether tab group name in side panel.
        /// </summary>
        public string TabGroupName
        {
            get
            {
                return m_TabGroupName;
            }
            set
            {
                m_TabGroupName = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether tab is owner of group in side panel.
        /// </summary>
        public bool IsTabGroupOwner
        {
            get
            {
                return m_IsTabGroupOwner;
            }
            set
            {
                m_IsTabGroupOwner = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether tab order in side panel group.
        /// </summary>
        public int SideTabOrder
        {
            get
            {
                return m_SideTabOrder;
            }
            set
            {
                m_SideTabOrder = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the index in dock mode.
        /// </summary>
        /// <value>The index in dock mode.</value>
        public int IndexInDockMode
        {
            get
            {
                return m_IndexInDockMode;
            }
            set
            {
                m_IndexInDockMode = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the index in float mode.
        /// </summary>
        /// <value>The index in float mode.</value>
        public int IndexInFloatMode
        {
            get
            {
                return m_IndexInFloatMode;
            }
            set
            {
                m_IndexInFloatMode = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the tab order in dock mode.
        /// </summary>
        /// <value>The tab order in dock mode.</value>
        public int TabOrderInDockMode
        {
            get
            {
                return m_TabOrderInDockMode;
            }
            set
            {
                m_TabOrderInDockMode = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the tab order in float mode.
        /// </summary>
        /// <value>The tab order in float mode.</value>
        public int TabOrderInFloatMode
        {
            get
            {
                return m_TabOrderInFloatMode;
            }
            set
            {
                m_TabOrderInFloatMode = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the MDI bounds.
        /// </summary>
        /// <value>The MDI bounds.</value>
        public string MDIBounds
        {
            get
            {
                return m_MDIBounds;
            }
            set
            {
                m_MDIBounds = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the index of the TDI.
        /// </summary>
        /// <value>The index of the TDI.</value>
        public int TDIIndex
        {
            get
            {
                return m_TDIIndex;
            }
            set
            {
                m_TDIIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected
        {
            get
            {
                return m_IsSelected;
            }
            set
            {
                m_IsSelected = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the MDI minimized bounds.
        /// </summary>
        /// <value>The MDI minimized bounds.</value>
        public string MDIMinimizedBounds
        {
            get
            {
                return m_MDIMinimizedBounds;
            }
            set
            {
                m_MDIMinimizedBounds = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the state of the MDI window.
        /// </summary>
        /// <value>The state of the MDI window.</value>
        public MDIWindowState MDIWindowState
        {
            get
            {
                return m_MDIWindowState;
            }
            set
            {
                m_MDIWindowState = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [allow MDI resize].
        /// </summary>
        /// <value><c>true</c> if [allow MDI resize]; otherwise, <c>false</c>.</value>
        public bool AllowMDIResize
        {
            get
            {
                return m_AllowMDIResize;
            }
            set
            {
                m_AllowMDIResize = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the TDI group orientation.
        /// </summary>
        /// <value>The TDI group orientation.</value>
        public Orientation TDIGroupOrientation
        {
            get
            {
                return m_TDIGroupOrientation;
            }
            set
            {
                m_TDIGroupOrientation = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the way of TDI group.
        /// </summary>
        /// <value>The way of TDI group.</value>
        /// <remarks>I use sting as generic list doesn't support in SAOP serializer</remarks>
        public string WayOfTDIGroup
        {
            get
            {
                return m_WayOfTDIGroup;
            }
            set
            {
                m_WayOfTDIGroup = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the Dock value for Element
        /// </summary>
        /// <value>The Dock For Side Element</value>
        public Dock DockForSide
        {
            get
            {
                return m_DockForSide;
            }
            set
            {
                m_DockForSide = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the PreviousIndexInDockMode value for Element
        /// </summary>
        /// <value>The Dock For Side Element</value>
        public int PreviousIndexInDockMode
        {
            get
            {
                return m_PreviousIndexInDockMode;
            }
            set
            {
                m_PreviousIndexInDockMode = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the PreviousChildElements value for Element
        /// </summary>
        /// <value>The Dock For Side Element</value>
        public List<String> PreviousChildElements
        {
            get
            {
                return m_PreviousChildElements;
            }
            set
            {
                m_PreviousChildElements = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the PreviousSideInDockMode value for Element
        /// </summary>
        /// <value>The previous  Side for Element</value>
        public DockSide PreviousSideInDockMode
        {
            get
            {
                return m_PreviousSideInDockMode;
            }
            set
            {
                m_PreviousSideInDockMode = value;
            }
        }
        /// <summary>
        /// Gets or Sets a value indicating whether the DoShift
        /// </summary>
        public bool DoShift
        {
            get
            {
                return m_DoShift;
            }
            set
            {
                m_DoShift = value;
            }
        }
        /// <summary>
        /// Gets or Sets a value indicating whether the SideRelativetoContainer
        /// </summary>
        public DockSide SideRelativetoContainer
        {
            get
            {
                return m_SideRelativetoContainer;
            }
            set
            {
                m_SideRelativetoContainer = value;
            }
        }
        /// <summary>
        /// Gets or Sets a value indicating whether the  TabParent
        /// </summary>
        public String TabParent
        {
            get
            {
                return m_TabParent;
            }
            set
            {
                m_TabParent = value;
            }
        }

        /// <summary>
        /// Gets or sets the state of the dock window.
        /// </summary>
        /// <value>The state of the dock window.</value>
        public WindowState DockWindowState
        {
            get
            {
                return m_DockWindowState;
            }
            set
            {
                m_DockWindowState = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can maximize.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can maximize; otherwise, <c>false</c>.
        /// </value>
        public bool CanMaximize
        {
            get
            {
                return m_CanMaximize;
            }
            set
            {
                m_CanMaximize = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can minimize.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can minimize; otherwise, <c>false</c>.
        /// </value>
        public bool CanMinimize
        {
            get
            {
                return m_CanMinimize;
            }
            set
            {
                m_CanMinimize = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can resize in docked state.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can resize in docked state; otherwise, <c>false</c>.
        /// </value>
        public bool CanResizeInDockedState
        {
            get
            {
                return m_CanResizeInDockedState;
            }
            set
            {
                m_CanResizeInDockedState = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can resize height in docked state.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can resize height in docked state; otherwise, <c>false</c>.
        /// </value>
        public bool CanResizeHeightInDockedState
        {
            get
            {
                return m_CanResizeHeightInDockedState;
            }
            set
            {
                m_CanResizeHeightInDockedState = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can resize width in docked state.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can resize width in docked state; otherwise, <c>false</c>.
        /// </value>
        public bool CanResizeWidthInDockedState
        {
            get
            {
                return m_CanResizeWidthInDockedState;
            }
            set
            {
                m_CanResizeWidthInDockedState = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can resize in float state.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can resize in float state; otherwise, <c>false</c>.
        /// </value>
        public bool CanResizeInFloatState
        {
            get
            {
                return m_CanResizeInFloatState;
            }
            set
            {
                m_CanResizeInFloatState = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can resize height in float state.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can resize height in float state; otherwise, <c>false</c>.
        /// </value>
        public bool CanResizeHeightInFloatState
        {
            get
            {
                return m_CanResizeHeightInFloatState;
            }
            set
            {
                m_CanResizeHeightInFloatState = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can resize width in float state.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can resize width in float state; otherwise, <c>false</c>.
        /// </value>
        public bool CanResizeWidthInFloatState
        {
            get
            {
                return m_CanResizeWidthInFloatState;
            }
            set
            {
                m_CanResizeWidthInFloatState = value;
            }
        }

        public bool CanFloatMaximize
        {
            get
            {
                return m_CanFloatMaximize;
            }
            set
            {
                m_CanFloatMaximize = value;
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether this instance is fixed size.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is fixed size; otherwise, <c>false</c>.
        /// </value>
        public bool IsFixedSize
        {
            get
            {
                return m_IsFixedSize;
            }
            set
            {
                m_IsFixedSize = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is fixed height.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is fixed height; otherwise, <c>false</c>.
        /// </value>
        public bool IsFixedHeight
        {
            get
            {
                return m_IsFixedHeight;
            }
            set
            {
                m_IsFixedHeight = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is fixed width.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is fixed width; otherwise, <c>false</c>.
        /// </value>
        public bool IsFixedWidth
        {
            get
            {
                return m_IsFixedWidth;
            }
            set
            {
                m_IsFixedWidth = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [fixed height].
        /// </summary>
        /// <value><c>true</c> if [fixed height]; otherwise, <c>false</c>.</value>
        public double FixedHeight
        {
            get
            {
                return m_FixedHeight;
            }
            set
            {
                m_FixedHeight = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [fixed width].
        /// </summary>
        /// <value><c>true</c> if [fixed width]; otherwise, <c>false</c>.</value>
        public double FixedWidth
        {
            get
            {
                return m_FixedWidth;
            }
            set
            {
                m_FixedWidth = value;
            }
        }

        /// <summary>
        /// Gets or sets the size of the container.
        /// </summary>
        /// <value>The size of the container.</value>
        public Size ContainerSize
        {
            get
            {
                return m_ContainerSize;
            }
            set
            {
                m_ContainerSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of the previous host.
        /// </summary>
        /// <value>The width of the previous host.</value>
        public double PreviousHostWidth
        {
            get
            {
                return m_PreviousHostWidth;
            }
            set
            {
                m_PreviousHostWidth = value;
            }
        }

        /// <summary>
        /// Gets or sets the height of the previous host.
        /// </summary>
        /// <value>The height of the previous host.</value>
        public double PreviousHostHeight
        {
            get
            {
                return m_PreviousHostHeight;
            }
            set
            {
                m_PreviousHostHeight = value;
            }
        }

        /// <summary>
        /// Gets or sets the height of the previous container.
        /// </summary>
        /// <value>The height of the previous container.</value>
        public double PreviousContainerHeight
        {
            get
            {
                return m_PreviousContainerHeight;
            }
            set
            {
                m_PreviousContainerHeight = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of the previous container.
        /// </summary>
        /// <value>The width of the previous container.</value>
        public double PreviousContainerWidth
        {
            get
            {
                return m_PreviousContainerWidth;
            }
            set
            {
                m_PreviousContainerWidth = value;
            }
        }

        public string TargetAutoHide
        {
            get
            {
                return m_TargetAutoHide;
            }
            set
            {
                m_TargetAutoHide = value;
            }
        }
        
        public double SplitPanelOffset
        {
            get
            {
                return m_offset;
            }
            set
            {
                m_offset = value;
            }
        }

        public bool IsSwapped
        {
            get
            {
                return m_IsSwapped;
            }
            set
            {
                m_IsSwapped = value;
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="DockingParams"/> class.
        /// </summary>
        public DockingParams()
        {
        }

        /// <summary>
        /// Gets the correct index in dock mode.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="docking">The docking.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private int GetCorrectIndexInDockMode(FrameworkElement element, DockingManager docking,int index)
        {
            if (index > docking.Children.Count - 1 && DockingManager.GetState(element)==DockState.AutoHidden)
                return DockingManager.GetPreviousIndexInDockMode(element);
            else
                return index;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DockingParams"/> class.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="doShift">if set to <c>true</c> [do shift].</param>
        public DockingParams(FrameworkElement element,bool doShift)
        {
            DockingManager docking = DockingManager.ResolveManager(element);

            m_element = element;
            Name = m_element.Name;
            DoShift = doShift;
            NoHeader = DockingManager.GetNoHeader(element);
            State = DockingManager.GetState(m_element);
            FloatWindowState = DockingManager.GetFloatWindowState(element);
            if (State == DockState.Float)
            {
                NoHeader = DockingManager.GetPreviousNoHeader(element);
            }
            
            SideDocked = DockingManager.GetSideInDockedMode(m_element);
            if (SideDocked == DockSide.Tabbed&&DockingManager.GetTabControl(m_element)==null)
            {
                if (State == DockState.Dock || State == DockState.Float)
                {
                    if (DockingManager.GetTargetElement(m_element, State) != null)
                    {
                        SideDocked = DockingManager.GetSideInDockedMode(DockingManager.GetTargetElement(m_element, State));
                    }
                }
            }
            DockForSide = DockingManager.GetSidePanelDock(m_element);
            SideFloating = DockingManager.GetSideInFloatMode(m_element);
            TargetDocked = IsSetName(docking, element, DockingManager.GetTargetNameInDockedMode(element)) ? DockingManager.GetTargetNameInDockedMode(element) : GetTarget(docking, element);
            TargetAutoHide = DockingManager.GetTargetNameInAutoHideMode(element);
            TargetFloating = DockingManager.GetTargetNameInFloatingMode(element);
            if (SideDocked == DockSide.Tabbed && State == DockState.Document)
            {
                SideDocked = DockingManager.GetPreviousSideInDockMode(m_element);
                FrameworkElement felement = docking.FindChild(TargetDocked);
                if (felement != null)
                {
                    if (DockingManager.GetState(felement as DependencyObject)==DockState.Dock||DockingManager.GetState(felement as DependencyObject)==DockState.Float)
                    {
                        DockSide side = DockingManager.GetSide(felement as DependencyObject, DockingManager.GetState(felement as DependencyObject));
                        if (side == DockSide.Tabbed)
                        {
                            TabControl tabelement = null;
                            if (DockingManager.GetTabControl(felement) != null)
                            {
                                tabelement = DockingManager.GetTabControl(felement);
                            }

                            if (tabelement != null)
                            {
                                foreach (FrameworkElement tab in tabelement.Items)
                                {
                                    if (DockingManager.GetIsSelectedTab(tab))
                                    {
                                        TargetDocked = tab.Name;
                                        DockingManager.SetTargetNameSafe(element, TargetDocked, State);
                                        DockingManager.SetSideSafe(element, SideDocked, State);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            ValidateTargetState(docking, State, element);

            DesiredWidthInFloatingMode = DockingManager.GetDesiredWidthInFloatingMode(element);
            DesiredHeightInFloatingMode = DockingManager.GetDesiredHeightInFloatingMode(element);

            bool canexecute = false;

            if (State == DockState.Float)
                canexecute = true;
            else if (State == DockState.Dock)
                canexecute = DockingManager.GetDockHost(element) != null && DockingManager.GetDockHost(element).Visibility != Visibility.Collapsed &&
                    DockingManager.GetDockHost(element).ActualHeight != 0.0 && DockingManager.GetDockHost(element).ActualWidth != 0.0;

            if (canexecute && DockingManager.GetDockHost(element) != null)
            {
                PreviousDesiredHeightInDockedMode = DockingManager.GetDesiredHeightInDockedMode(element);
                PreviousDesiredWidthInDockedMode = DockingManager.GetDesiredWidthInDockedMode(element);

                if (State == DockState.Dock && !docking.m_IsManagerLoaded)
                {
                    DesiredWidthInDockedMode = DockingManager.GetDesiredWidthInDockedMode(element);
                    DesiredHeightInDockedMode = DockingManager.GetDesiredHeightInDockedMode(element);
                }

                if (State == DockState.Dock && docking.m_IsManagerLoaded)
                {
                    DesiredWidthInDockedMode = DockingManager.GetDockHost(element).ActualWidth;
                    DesiredHeightInDockedMode = DockingManager.GetDockHost(element).ActualHeight;
                }
                else if (State != DockState.Dock)
                {
                    DesiredWidthInDockedMode = DockingManager.GetDesiredWidthInDockedMode(element);
                    DesiredHeightInDockedMode = DockingManager.GetDesiredHeightInDockedMode(element);
                }
                FrameworkElement container = DockingManager.GetDockHost(element).Parent as FrameworkElement;
                
                if (container is DockedElementsContainer)
                {
                    if (!(container as IDesiredSize).DesiredSize.IsEmpty)
                        PreviousContainerDesiredSize = (container as IDesiredSize).DesiredSize;
                    ContainerSize = new Size((container as DockedElementsContainer).ActualWidth, (container as DockedElementsContainer).ActualHeight);
                }
            }
            else
            {
                DesiredWidthInDockedMode = DockingManager.GetDesiredWidthInDockedMode(element);
                DesiredHeightInDockedMode = DockingManager.GetDesiredHeightInDockedMode(element);
            }

            NoDock = DockingManager.GetNoDock(element);
            CanDock = DockingManager.GetCanDock(element);
            CanClose = DocumentContainer.GetCanClose(element);
            IsSelectedTab = DockingManager.GetIsSelectedTab(element);
            IsActiveWindow = docking == null ? false : docking.ActiveWindow == element;
            WindowRect = string.Empty;
            TabGroupName = SidePanel.GetTabGroupName(element);

            if (string.IsNullOrEmpty(TabGroupName))
                TabGroupName = string.Empty;

            TabOrderIndex = DockingManager.GetDocumentTabOrderIndex(element);
            IsTabGroupOwner = SidePanel.GetIsTabGroupOwner(element);
            SideTabOrder = SidePanel.GetTabChildOrder(element);

            PreviousIndexInDockMode = DockingManager.GetPreviousIndexInDockMode(element);
            PreviousChildElements = DockingManager.GetPreviousChildElements(element);

            if (PreviousChildElements != null && PreviousChildElements.Count == 0)
                PreviousChildElements = null;

            PreviousSideInDockMode = DockingManager.GetPreviousSideInDockMode(element);
            SideRelativetoContainer = DockingManager.GetSideRelativetoContainer(element);
            TabParent = DockingManager.GetTabParent(element);

            if (string.IsNullOrEmpty(TabParent))
                TabParent = string.Empty;

            IndexInDockMode = GetCorrectIndexInDockMode(element, docking, DockingManager.GetIndexInDockMode(element));
            IndexInFloatMode = DockingManager.GetIndexInFloatMode(element);
            //IndexInDockMode = DockingManager.GetIndexInDockMode(element) == -1 ? docking.Children.Count - 1 : DockingManager.GetIndexInDockMode(element);
            //IndexInFloatMode = DockingManager.GetIndexInFloatMode(element) == -1 ? docking.Children.Count - 1 : DockingManager.GetIndexInFloatMode(element);
            TabOrderInDockMode = DockedElementTabbedHost.GetTabOrderInDockMode(element);
            TabOrderInFloatMode = DockedElementTabbedHost.GetTabOrderInFloatMode(element);

            element.SetValue(DockingManager.IsActiveWindowProperty, IsActiveWindow);
            Rect rectWindow = DockingManager.GetFloatingWindowRect(m_element);

            RectConverter converter = new RectConverter();

            if (!rectWindow.IsEmpty)
            {
                WindowRect = converter.ConvertToInvariantString(rectWindow);
            }

            Rect previousRect = DockingManager.GetPreviousFloatingWindowRect(element);

            if (!previousRect.IsEmpty)
            {
                PreviousFloatingWindowRect = converter.ConvertToInvariantString(previousRect);
            }
            ////DocumentContainer properties
            Rect mdiBounds = DocumentContainer.GetMDIBounds(element);
            Rect mdiMinimizedBounds = DocumentContainer.GetMDIMinimizedBounds(element);
            MDIWindow window = VisualUtils.FindAncestor(element, typeof(MDIWindow)) as MDIWindow;

            if (window != null && !window.WasMinimizedDragged)
            {
                mdiMinimizedBounds = Rect.Empty;
            }

            MDIBounds = converter.ConvertToInvariantString(mdiBounds);
            MDIMinimizedBounds = converter.ConvertToInvariantString(mdiMinimizedBounds);
            MDIWindowState = DocumentContainer.GetMDIWindowState(element);
            AllowMDIResize = DocumentContainer.GetAllowMDIResize(element);
            TDIIndex = (int)element.GetValue(TDILayoutPanel.TDIIndexProperty);
            IsSelected = (bool)TDILayoutPanel.GetIsSelected(element);
            TDIGroupOrientation = (Orientation)element.GetValue(TDILayoutPanel.TDIGroupOrientationProperty);
            WayOfTDIGroup = (string)element.GetValue(TDILayoutPanel.WayOfTDIGroupProperty);
            SplitPanelOffset = (double)element.GetValue(TDILayoutPanel.SplitPanelOffsetProperty);
            CanMaximize = (bool)DockingManager.GetCanMaximize(element);
            CanMinimize = (bool)DockingManager.GetCanMinimize(element);
            DockWindowState = (WindowState)DockingManager.GetDockWindowState(element);
            CanResizeInDockedState = (bool)DockingManager.GetCanResizeInDockedState(element);
            CanResizeInFloatState = (bool)DockingManager.GetCanResizeInFloatState(element);
            CanResizeHeightInDockedState = (bool)DockingManager.GetCanResizeHeightInDockedState(element);
            CanResizeWidthInDockedState = (bool)DockingManager.GetCanResizeWidthInDockedState(element);
            CanResizeHeightInFloatState = (bool)DockingManager.GetCanResizeHeightInFloatState(element);
            CanResizeWidthInFloatState = (bool)DockingManager.GetCanResizeWidthInFloatState(element);
            CanFloatMaximize = (bool)DockingManager.GetCanFloatMaximize(element);
            IsFixedSize = (bool)DockingManager.GetIsFixedSize(element);
            IsFixedHeight = (bool)DockingManager.GetIsFixedHeight(element);
            IsFixedWidth = (bool)DockingManager.GetIsFixedWidth(element);
            FixedHeight = DockingManager.GetFixedHeight(element);
            FixedWidth = DockingManager.GetFixedWidth(element);
            PreviousContainerHeight = DockedElementsContainer.GetPreviousContainerHeight(element);
            PreviousContainerWidth = DockedElementsContainer.GetPreviousContainerWidth(element);
            PreviousHostHeight = DockedElementTabbedHost.GetPreviousHostHeight(element);
            PreviousHostWidth = DockedElementTabbedHost.GetPreviousHostWidth(element);
            ZorderInFloatMode = DockingManager.GetZorderInFloatMode(element);
            IsSwapped = DockingManager.GetIsSwapped(element);
            
        }

        private void ValidateTargetState(DockingManager docking, DockState state,FrameworkElement element) 
        {
            FrameworkElement felement=null;

            if (!TargetDocked.Equals(String.Empty))
            {
                if (state == DockState.Dock)
                    felement = docking.FindChild(TargetDocked);
                else if (state == DockState.Float)
                    felement = docking.FindChild(TargetFloating);

                if (felement != null)
                {
                    if (State == DockState.Dock || State == DockState.Float)
                    {
                        DockSide side = DockingManager.GetSideSafe(felement as DependencyObject, State);
                        if (side == DockSide.Tabbed)
                        {
                            TabControl tabelement = null;
                            if (DockingManager.GetTabControl(felement) != null)
                            {
                                tabelement = DockingManager.GetTabControl(felement);
                            }

                            if (tabelement != null)
                            {
                                foreach (FrameworkElement tab in tabelement.Items)
                                {
                                    if (DockingManager.GetIsSelectedTab(tab))
                                    {
                                        if (state == DockState.Dock)
                                        {
                                            TargetDocked = tab.Name;
                                            DockingManager.SetTargetNameSafe(element, TargetDocked, State);
                                        }
                                        else if (state == DockState.Float)
                                        {
                                            TargetFloating = tab.Name;
                                            DockingManager.SetTargetNameSafe(element, TargetFloating, State);
                                        }
                                    }
                                }
                                
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="DockingParams"/> class. 
        /// Deserialization method called by ISerializable Interface.
        /// </summary>
        /// <param name="info">Contains values of all Dependency
        /// Properties of DockingManager class.</param>
        /// <param name="context">Describes the source and destination
        /// of a given serialized stream, provides
        /// an additional caller\-defined context.</param>
        public DockingParams(SerializationInfo info, StreamingContext context)
        {
            List<DependencyProperty> depPropertyList = DockingManager.GetListSerializedProperties();
            m_dockParamsTable = new Hashtable();

            foreach (DependencyProperty depProperty in depPropertyList)
            {
                object propValue = info.GetValue(depProperty.Name, typeof(object));
                m_dockParamsTable.Add(depProperty, propValue);
            }

            ParamsTable.Params.Add(Guid.NewGuid(), m_dockParamsTable);
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">
        /// The caller does not have the required permission.
        /// </exception>
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            List<DependencyProperty> depPropertyList = DockingManager.GetListSerializedProperties();

            foreach (DependencyProperty depProperty in depPropertyList)
            {
                object propValue = m_element.GetValue(depProperty);
                info.AddValue(depProperty.Name, propValue);
            }
        }
        #endregion
        /// <summary>
        /// Determines whether [is set name] [the specified docking].
        /// </summary>
        /// <param name="docking">The docking.</param>
        /// <param name="srcElement">The SRC element.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <returns>
        /// <c>true</c> if [is set name] [the specified docking]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsSetName(DockingManager docking, FrameworkElement srcElement, string elementName)
        {
            foreach (FrameworkElement element in docking.Children)
            {
                if (element.Name.Equals(elementName))
                {
                    if (DockingManager.GetState(element) == DockState.AutoHidden && DockingManager.GetSideInDockedMode(srcElement) == DockSide.Tabbed)
                    {
                        return true;
                    }
                    else if (DockingManager.GetState(element) == DockState.AutoHidden)
                    {
                        DockingManager.SetTargetNameInAutoHideMode(srcElement, elementName);
                        return false;
                    }
                }
            }

            return true;
        }

        private string GetTarget(DockingManager docking, FrameworkElement element)
        {
            FrameworkElement targetelement = docking.FindChild(DockingManager.GetTargetNameInDockedMode(element));
            if (targetelement != null)
            {
                if (DockingManager.GetState(targetelement) == DockState.AutoHidden)
                {
                    return GetTarget(docking, targetelement);
                }
                else
                {
                    return targetelement.Name;
                }
            }
            return string.Empty;
        }
    }
}
