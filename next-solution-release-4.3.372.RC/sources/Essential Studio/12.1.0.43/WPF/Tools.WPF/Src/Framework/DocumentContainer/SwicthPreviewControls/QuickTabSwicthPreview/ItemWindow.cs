// <copyright file="ItemWindow.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Windows;
using System.Windows.Input;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// This class presents item in QuickTab mode.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ItemWindow : GalleryItem
    {
        #region Constants
        /// <summary>
        /// Defines name of header in template.
        /// </summary>
        private const string HEADER_NAME = "PART_Header";
        
        /// <summary>
        /// This member presents a element's name from Template of control.
        /// </summary>
        private const string PREVIEWBORDER_NAME = "PART_PreviewBorder";
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="ItemWindow"/> class.
        /// </summary>
        /// <property name="flag" value="Finished"/>
        static ItemWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ItemWindow), new FrameworkPropertyMetadata(typeof(ItemWindow)));
        }
        #endregion

        #region Public methods
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code 
        /// or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ItemHeader header = GetTemplateChild(HEADER_NAME) as ItemHeader;
            PreviewBorder previewBorder = GetTemplateChild(PREVIEWBORDER_NAME) as PreviewBorder;

            if (null == header || null == previewBorder)
            {
                throw new NotImplementedException("Incorrect template!");
            }

            header.ApplyTemplate();
            header.CloseButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(OnCloseButtonPreviewMouseLeftButtonUp);
#if !SyncfusionFramework3_5
            //header.CloseButton.PreviewTouchUp += CloseButton_PreviewTouchUp;
#endif

            FrameworkElement dContext = (FrameworkElement)DataContext;
            SwicthPreviewControlBase.SetCustomBrush(previewBorder, dContext);
        }

        #endregion

        #region Implementation
        /// <summary>
        /// Invoked when MouseDown event is raised.
        /// </summary>
        /// <param name="e">The instance that contains the event data.</param>
        /// <property name="flag" value="Finished"/>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                IsSelected = true;
                base.OnMouseDown(e);
            }
        }

#if !SyncfusionFramework3_5

        //protected override void OnTouchDown(TouchEventArgs e)
        //{
        //    IsSelected = true;
        //    base.OnTouchDown(e);
        //}

        //void CloseButton_PreviewTouchUp(object sender, TouchEventArgs e)
        //{
        //    DependencyObject depended = (DependencyObject)DataContext;
        //    FrameworkElement element = (FrameworkElement)DataContext;
        //    bool canExecute = (null != depended) ? DocumentContainer.GetCanClose(depended) : false;
        //    DockingManager docking = element.Parent as DockingManager;
        //    if (canExecute)
        //    {
        //        if (docking != null && docking.IsTouchEnabled && docking.m_TouchDeviceId == e.TouchDevice.Id)
        //        {
        //            CloseButtonEventArgs args = new CloseButtonEventArgs(element as UIElement);
        //            docking.FireCloseButtonClick(args);

        //            if (!args.Cancel)
        //            {
        //                depended.SetValue(DockingManager.StateProperty, DockState.Hidden);
        //                Close();
        //                e.Handled = true;
        //            }
        //        }
        //    }
        //}
#endif

        /// <summary>
        /// Closes this instance.
        /// </summary>
        private void Close()
        {
            QuickTabSwicthPreviewControl panel = (QuickTabSwicthPreviewControl)VisualUtils.FindAncestor(this, typeof(QuickTabSwicthPreviewControl));
            panel.Remove(this);
        }
        
        /// <summary>
        /// Called when [close button preview mouse left button up].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnCloseButtonPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                DependencyObject depended = (DependencyObject)DataContext;
                FrameworkElement element = (FrameworkElement)DataContext;
                bool canExecute = (null != depended) ? DocumentContainer.GetCanClose(depended) : false;
                DockingManager docking = element.Parent as DockingManager;
                if (canExecute)
                {
                    if (docking != null)
                    {
                        CloseButtonEventArgs args = new CloseButtonEventArgs(element as UIElement);
                        docking.FireCloseButtonClick(args);

                        if (!args.Cancel)
                        {
                            depended.SetValue(DockingManager.StateProperty, DockState.Hidden);
                            Close();
                            e.Handled = true;
                        }
                    }
                }
            }

        }
        #endregion
    }
}
