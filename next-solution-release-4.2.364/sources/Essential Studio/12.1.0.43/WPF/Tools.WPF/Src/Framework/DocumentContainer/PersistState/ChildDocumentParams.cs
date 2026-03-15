// <copyright file="ChildDocumentParams.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Presents serialization's parameters for DocumentContainer's children.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [Serializable]
    public class ChildDocumentParams : DocumentParamsBase
    {
        #region Public properties
        /// <summary>
        /// Gets or sets name of the docking window.
        /// </summary>
        public string Name
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets the MDI bounds.
        /// </summary>
        /// <value>The MDI bounds.</value>
        public string MDIBounds
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets the index of the TDI.
        /// </summary>
        /// <value>The index of the TDI.</value>
        public int TDIIndex
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets the MDI minimized bounds.
        /// </summary>
        /// <value>The MDI minimized bounds.</value>
        public string MDIMinimizedBounds
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets the state of the MDI window.
        /// </summary>
        /// <value>The state of the MDI window.</value>
        public MDIWindowState MDIWindowState
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether this instance can close.
        /// </summary>
        /// <value><c>true</c> if this instance can close; otherwise, <c>false</c>.</value>
        public bool CanClose
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether [allow MDI resize].
        /// </summary>
        /// <value><c>true</c> if [allow MDI resize]; otherwise, <c>false</c>.</value>
        public bool AllowMDIResize
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>The state.</value>
        public DockState State
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        public bool IsActive
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets the TDI group orientation.
        /// </summary>
        /// <value>The TDI group orientation.</value>
        public Orientation TDIGroupOrientation
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets or sets the way of TDI group.
        /// </summary>
        /// <value>The way of TDI group.</value>
        /// <remarks>I use sting as generic list doesn't support in SAOP serializer</remarks>
        public string WayOfTDIGroup
        {
            get;
            set;
        }

        double _offset = 0.0;

        public double SplitPanelOffset
        {
            get
            {
                return _offset;
            }
            set
            {
                _offset = value;
            }
        }

        #endregion

        #region Initialization
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ChildDocumentParams"/> class.
        /// </summary>
        public ChildDocumentParams()
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ChildDocumentParams"/> class.
        /// </summary>
        /// <param name="element">The element ChildDocumentParams.</param>
        /// <param name="mode">The mode ChildDocumentParams.</param>
        /// <param name="isActive">if set to <c>true</c> [is active].</param>
        public ChildDocumentParams(FrameworkElement element, PropertiesMode mode, bool isActive)
            : base(element, mode)
        {
            IsActive = isActive;

            Name = Element.Name;
            Rect mdiBounds = DocumentContainer.GetMDIBounds(Element);
            Rect mdiMinimizedBounds = DocumentContainer.GetMDIMinimizedBounds(Element);
            MDIWindow window = VisualUtils.FindAncestor(element, typeof(MDIWindow)) as MDIWindow;

            if (window != null && !window.WasMinimizedDragged)
            {
                mdiMinimizedBounds = Rect.Empty;
            }

            RectConverter converter = new RectConverter();
            MDIBounds = converter.ConvertToInvariantString(mdiBounds);
            MDIMinimizedBounds = converter.ConvertToInvariantString(mdiMinimizedBounds);

            MDIWindowState = DocumentContainer.GetMDIWindowState(Element);
            CanClose = DocumentContainer.GetCanClose(Element);
            AllowMDIResize = DocumentContainer.GetAllowMDIResize(Element);
            State = DockingManager.GetState(Element);

            TDIIndex = (int)element.GetValue(TDILayoutPanel.TDIIndexProperty);
            IsSelected = (bool)TDILayoutPanel.GetIsSelected(element);
            TDIGroupOrientation = (Orientation)element.GetValue(TDILayoutPanel.TDIGroupOrientationProperty);
            WayOfTDIGroup = (string)element.GetValue(TDILayoutPanel.WayOfTDIGroupProperty);
            SplitPanelOffset = (double)element.GetValue(TDILayoutPanel.SplitPanelOffsetProperty);
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ChildDocumentParams"/> class.
        /// </summary>
        /// <param name="info">The info ChildDocumentParams.</param>
        /// <param name="context">The context ChildDocumentParams.</param>
        public ChildDocumentParams(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            IsActive = info.GetBoolean(ISACTIVE_PARAMNAME);
            DocParamsTable.Add(ISACTIVE_PARAMNAME, IsActive);
        }
        #endregion
    }
}
