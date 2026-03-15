// <copyright file="DockPreviewManagerBase.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Syncfusion.Windows.Shared;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Input;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents docking manager's preview base.
    /// </summary>
    /// <exclude/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public abstract class DockPreviewManagerBase : DependencyObject
    {
        #region Private members
        
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Specifies docking manager, the instance is bound to.
        /// </summary>        
        private readonly DockingManager m_manager;
        
        /// <summary>
        /// Specifies docking preview record.
        /// </summary>
        private DockPreviewRecord? m_previewActive;

        internal int m_headerpanelindex = -1;

        internal DockSide m_previousdockside = DockSide.None;
        
        /// <summary>
        /// Specifies preview adorner.
        /// </summary>
        private Adorner m_adornerPreview;
        
        /// <summary>
        /// Specifies adorner layout.
        /// </summary>        
        private AdornerLayer m_adornerPreviewLayer;

        /// <summary>
        /// Represent the data return from calling AdornerHitTest Method.
        /// </summary>        
        protected AdornerHitTestResult m_prevResult = null;

        /// <summary>
        /// This member indicate or slow show provider.
        /// </summary>
        protected bool m_isLowPerform = false;

        internal Window dockpreview = null;

        #endregion

        #region Properties
        
        /// <summary>
        /// Gets the docking manager.
        /// </summary>
        /// <value>The docking manager.</value>
        public DockingManager DockingManager
        {
            get
            {
                return m_manager;
            }
        }
        
        /// <summary>
        /// Gets the client area container of the <see cref="DockPreviewManagerBase"/>.
        /// </summary>
        protected FrameworkElement ClientAreaContainer
        {
            get
            {
                return DockingManager.ClientAreaContainer;
            }
        }
        
        /// <summary>
        /// Gets a value indicating whether if preview main button visible of the <see cref="DockPreviewManagerBase"/>.
        /// </summary>
        public abstract bool IsDockPreviewMainButtonVisible
        {
            get;
        }

        /// <summary>
        /// Sets a value indicating whether this instance is low perform.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is low perform; otherwise, <c>false</c>.
        /// </value>
        internal bool IsLowPerform
        {
            set
            {
                m_isLowPerform = value;
            }
        }
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="DockPreviewManagerBase"/> class.
        /// </summary>
        /// <param name="manager">The manager.</param>
        protected DockPreviewManagerBase(DockingManager manager)
        {
            m_manager = manager;
        }
        #endregion

        #region Internal Methods
        /// <summary>
        /// Finds the docking place.
        /// </summary>
        /// <param name="element">The framework element.</param>
        /// <param name="host">The docked element tabbed host.</param>
        /// <param name="point">The point.</param>
        /// <returns>return dock preview record.</returns>
        internal DockPreviewRecord? FindDockingPlace(FrameworkElement element, DockedElementTabbedHost host, Point point)
        {
            return FindDockingPlaceInternal(element, host, point);
        }
        
        /// <summary>
        /// Creates main button in dock preview.
        /// </summary>
        /// <param name="host">the host to create preview button in</param>
        /// <param name="bShowSideButtons">true if side buttons must be shown</param>
        internal void CreateDockPreviewMainButton(FrameworkElement host, bool bShowSideButtons)
        {
            CreateDockPreviewMainButtonInternal(host, bShowSideButtons);
        }
        
        /// <summary>
        /// Hides main button in dock preview.
        /// </summary>
        /// <param name="canSwitchPerform">true if perform can be switched</param>
        internal void HideDockPreviewMainButton(bool canSwitchPerform)
        {
            HideDockPreviewMainButtonInternal(canSwitchPerform);
        }
       
        /// <summary>
        /// Shows dock preview.
        /// </summary>
        /// <param name="record">record to find preview host</param>
        internal void ShowDockPreview(DockPreviewRecord record)
        {
            if (m_previewActive != null && m_previewActive != record)
            {
                HideDockPreview();
            }

            if (m_manager.UseAdornerDockPreview && m_manager.m_mousemoveonheaderpanel && m_manager.IsVS2010DraggingEnabled)
            {
                HideDockPreview();
            }

            if (m_previewActive != record)
            {
                DockInfoInternal info = record.TargetElement != null
                    ? DockingManager.GetDockInfo(record.TargetElement) : null;

                FrameworkElement host = GetHost(record, info);

                if (null != host)
                {
                    FrameworkElement actualelement = (record.Side == DockSide.Tabbed) ? (host is DockedElementTabbedHost) ? (host as DockedElementTabbedHost).InternalDataContext != null ? (host as DockedElementTabbedHost).InternalDataContext : ((DockedElementTabbedHost)host).HostedElement : null : null;
                    bool needPreview = !(record.Side == DockSide.Tabbed && actualelement!=null 
                        && (DockingManager.GetDockAbility(actualelement)==DockAbility.Tabbed)
                        &&(DockingManager.GetDockAbility(actualelement)==DockAbility.All)&&
                        ClientAreaContainer.Equals(((DockedElementTabbedHost)host).HostedElement));

                    if (m_manager.m_draggedElement != null)
                    {
                        DockAbility elementability = DockingManager.GetDockAbility(m_manager.m_draggedElement);
                        DockSide elementside = record.Side;
                        if (elementside != DockSide.Tabbed)
                        {
                            if (m_manager.AllowDock(elementability, elementside))
                                needPreview = true;
                            else
                                needPreview = false;
                        }
                    }


                    if (needPreview && m_manager.UseAdornerDockPreview)
                    {
                        m_adornerPreviewLayer = AdornerLayer.GetAdornerLayer(host);

                        if (null != m_adornerPreviewLayer)
                        {
                            Size size = DockingManager.GetFloatingWindowRect(record.Element).Size;
                            m_adornerPreview = CreateDockPreviewAdorner(record, host, size);

                            if (null != m_adornerPreview)
                            {
                                try
                                {
                                    m_previewActive = record;
                                    m_adornerPreviewLayer.Add(m_adornerPreview);                                    
                                }
                                catch { }
                            }
                            else
                            {
                                throw new InvalidOperationException("A call to CreateDockPreviewAdorner method returned null.");
                            }
                        }
                        else
                        {
                            throw new InvalidOperationException("There is no adorner layer i the visual tree of the element's host.");
                        }
                    }
                    else if (!m_manager.UseAdornerDockPreview && dockpreview==null)
                    {
                        m_previousdockside = record.Side;
                        Size size = DockingManager.GetFloatingWindowRect(record.Element).Size;
                        dockpreview = new Window() 
                        {
                            AllowsTransparency = true,
                            IsHitTestVisible = false,
                            ShowActivated=false,
                            WindowStyle=WindowStyle.None,
                            ResizeMode=ResizeMode.NoResize,
                            Background=Brushes.Transparent
                        };
                        m_adornerPreview = CreateDockPreviewAdorner(record, host, size);                        
                        m_previewActive = record;
                        Rect rect = Rect.Empty;
                        
                        Point hostpoint = host.PointToScreen(new Point(0, 0));
                        rect = new Rect(hostpoint.X, hostpoint.Y, host.ActualWidth, host.ActualHeight);
                        if (record.Action == DragProviderAction.Center)
                        {
                            rect = CheckHostAndReturnRect(record, host);
                        }
                        
                        
                        if (record.Action == DragProviderAction.GlobalLeft || record.Action == DragProviderAction.GlobalTop)
                        {                            
                            rect = new Rect(m_manager.PointToScreen(new Point(0, 0)), m_manager.RenderSize);
                        }
                        else if (record.Action == DragProviderAction.GlobalBottom)
                        {                            
                            record.Side = DockSide.Top;
                            m_adornerPreview = CreateDockPreviewAdorner(record, host, size);
                            hostpoint=m_manager.PointToScreen(new Point(0, 0));
                            rect = new Rect(hostpoint, m_manager.RenderSize);
                            CalculatePreviewSize(ref record, host, size);                            
                            rect = new Rect(new Point(rect.BottomLeft.X, rect.BottomLeft.Y - record.PreviewSize), m_manager.RenderSize);
                            
                        }
                        else if (record.Action == DragProviderAction.GlobalRight)
                        {                            
                            record.Side = DockSide.Left;
                            m_adornerPreview = CreateDockPreviewAdorner(record, host, size);
                            hostpoint=m_manager.PointToScreen(new Point(0, 0));
                            rect = new Rect(hostpoint, m_manager.RenderSize);
                            CalculatePreviewSize(ref record, host, size);                            
                            rect = new Rect(new Point(rect.TopRight.X - record.PreviewSize, rect.TopRight.Y), m_manager.RenderSize);                            
                        }
                        else if(record.Action==DragProviderAction.Right)
                        {
                            record.Side = DockSide.Left;
                            m_adornerPreview = CreateDockPreviewAdorner(record, host, size);
                            hostpoint = host.PointToScreen(new Point(0, 0));
                            rect = new Rect(hostpoint, host.RenderSize);
                            CalculatePreviewSize(ref record, host, size);                            
                            rect = new Rect(new Point(rect.TopRight.X - record.PreviewSize, rect.TopRight.Y), host.RenderSize);                            
                        }
                        else if (record.Action == DragProviderAction.Bottom)
                        {
                            record.Side = DockSide.Top;
                            m_adornerPreview = CreateDockPreviewAdorner(record, host, size);
                            hostpoint = host.PointToScreen(new Point(0, 0));
                            rect = new Rect(hostpoint, host.RenderSize);
                            CalculatePreviewSize(ref record, host, size);                            
                            rect = new Rect(new Point(rect.BottomLeft.X, rect.BottomLeft.Y - record.PreviewSize), host.RenderSize);
                        }
                        dockpreview.Top = rect.Top;
                        dockpreview.Left = rect.Left;
                        dockpreview.Height = rect.Size.Height;
                        dockpreview.Width = rect.Size.Width;
                        
                        Border Bd = new Border() { Background = Brushes.Transparent };
                        Bd.Child = (m_adornerPreview);
                        dockpreview.Content = Bd;
                        dockpreview.Show();
                        m_headerpanelindex = m_manager.m_mouseonheaderpanelindex;
                    }
                }
                else
                {
                    throw new ArgumentException("The specified element is not initialized in the specified state.");
                }
            }
        }

        protected Rect CheckHostAndReturnRect(DockPreviewRecord record, FrameworkElement host)
        {
            Point hostpoint = host.PointToScreen(new Point(0, 0));
            if (host == DockingManager.GetDocumentContainerHost() && m_manager.IsVS2010DraggingEnabled)
            {
                if (m_manager.m_mousemoveonheaderpanel)
                {
                    Point point = Mouse.GetPosition(host);
                    HitTestResult testresult = VisualTreeHelper.HitTest(host, point);
                    if (testresult != null)
                    {
                        TDILayoutPanel tdipanel = VisualUtils.FindAncestor(testresult.VisualHit as Visual, typeof(TDILayoutPanel)) as TDILayoutPanel;
                        DocumentTabControl tabcontrol = VisualUtils.FindAncestor(testresult.VisualHit as Visual, typeof(DocumentTabControl)) as DocumentTabControl;
                        if (tabcontrol != null)
                        {
                            Point tabpoint = tabcontrol.PointToScreen(new Point(0, 0));
                            if (tdipanel != null)
                                tdipanel.ActiveTabControl = tabcontrol;
                            return new Rect(tabpoint.X, tabpoint.Y, tabcontrol.ActualWidth, tabcontrol.ActualHeight);
                        }
                    }
                }
            }
            return new Rect(hostpoint.X, hostpoint.Y, host.ActualWidth, host.ActualHeight);
        }
       
        /// <summary>
        /// Hides dock preview.
        /// </summary>
        internal void HideDockPreview()
        {
            
            if (m_previewActive != null && m_manager.UseAdornerDockPreview)
            {
                m_adornerPreviewLayer.Remove(m_adornerPreview);

                m_adornerPreviewLayer = null;
                m_adornerPreview = null;
                m_previewActive = null;
                m_prevResult = null;
            }
            if (!m_manager.UseAdornerDockPreview && dockpreview!=null && m_previewActive != null)
            {                
                dockpreview.Content = null;
                dockpreview.Close();
                m_previewActive = null;
                m_adornerPreview = null;
                m_prevResult = null;
                dockpreview = null;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Finds the place in docking window where an element can be docked to.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="host">DockedElementTabbedHost where an element can be docked to</param>
        /// <param name="point">the point where an element can be docked to</param>
        /// <returns>the place, an element can be docked to</returns>
        protected abstract DockPreviewRecord? FindDockingPlaceInternal(FrameworkElement element, DockedElementTabbedHost host, Point point);
        
        /// <summary>
        /// Creates dock preview where an element can be docked to.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="host">FrameworkElement where adorner can be created</param>
        /// <param name="size">The element size.</param>
        /// <returns>adorner with templates support</returns>
        protected abstract TemplatedAdornerBase CreateDockPreviewAdorner(DockPreviewRecord record, FrameworkElement host, Size size);
        
        /// <summary>
        /// Creates main button in internal dock preview.
        /// </summary>
        /// <param name="host">the host to create preview button in</param>
        /// <param name="bShowSideButtons">true if side buttons must be shown</param>
        protected abstract void CreateDockPreviewMainButtonInternal(FrameworkElement host, bool bShowSideButtons);
        
        /// <summary>
        /// Hides main button in internal dock preview.
        /// </summary>
        /// <param name="canSwitchPerform">true if perform can be switched</param>
        protected abstract void HideDockPreviewMainButtonInternal(bool canSwitchPerform);

        /// <summary>
        /// This method return host for preview element.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="info">Info about host.</param>
        /// <returns>return framework element.</returns>
        private FrameworkElement GetHost(DockPreviewRecord record, DockInfoInternal info)
        {
            FrameworkElement host;

            if (null != info)
            {
                switch (record.State)
                {
                    case DockState.Dock:
                        host = info.HostDock;
                        break;
                    case DockState.Float:
                        host = info.HostFloat;
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            else
            {
                host = DockingManager.MainContent;
            }

            return host;
        }

        
        private void CalculatePreviewSize(ref DockPreviewRecord record, UIElement host, Size size)
        {
            bool bIsHorizontal = record.Side == DockSide.Left || record.Side == DockSide.Right;
            double hostlength = bIsHorizontal ? host.RenderSize.Width : host.RenderSize.Height;
            double elementlength = bIsHorizontal ? size.Width : size.Height;

            if (elementlength > hostlength / 2)
            {
                elementlength = hostlength / 2;
            }           

            record.PreviewSize = elementlength;            
        }
        #endregion
    }
}
