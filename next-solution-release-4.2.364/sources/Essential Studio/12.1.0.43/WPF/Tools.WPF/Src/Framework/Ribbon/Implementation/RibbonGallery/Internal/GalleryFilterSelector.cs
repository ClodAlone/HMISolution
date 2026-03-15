// <copyright file="GalleryFilterSelector.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents internal GalleryFilterSelector control.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class GalleryFilterSelector : Control
    {
        #region Fields
        /// <summary>
        /// Represents the DropDown button
        /// </summary>
        private FrameworkElement m_dropDownButton;

        /// <summary>
        /// Represents the popup
        /// </summary>
        private Popup m_popup;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Is used for correct popup opening and closing.
        /// </summary>
        private Stack<object> m_stack = new Stack<object>();

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Flag which is used for correct popup closing and opening. 
        /// </summary>
        private bool m_wasPressed = false;
        #endregion

        #region Dependency properties
        /// <summary>
        /// Defines the collection of filters.
        /// </summary>
        public static readonly DependencyProperty FiltersProperty =
            DependencyProperty.Register("Filters", typeof(ObservableCollection<RibbonGalleryFilter>), typeof(GalleryFilterSelector), new UIPropertyMetadata(null));

        /// <summary>
        /// Defines the selected filter.
        /// </summary>
        public static readonly DependencyProperty SelectedFilterProperty =
            DependencyProperty.Register("SelectedFilter", typeof(RibbonGalleryFilter), typeof(GalleryFilterSelector), new UIPropertyMetadata(null));

        /// <summary>
        /// Defines whether dropdown is open.
        /// </summary>
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(GalleryFilterSelector), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsDropDownOpenChanged)));

        #endregion

        #region Properties
        
        /// <summary>
        /// Gets or sets the collection of filters.
        /// </summary>
        public ObservableCollection<RibbonGalleryFilter> Filters
        {
            get
            {
                return (ObservableCollection<RibbonGalleryFilter>)GetValue(FiltersProperty);
            }

            set
            {
                SetValue(FiltersProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets the selected filter.
        /// </summary>
        public RibbonGalleryFilter SelectedFilter
        {
            get
            {
                return (RibbonGalleryFilter)GetValue(SelectedFilterProperty);
            }

            set
            {
                SetValue(SelectedFilterProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether this instance is drop down open.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is drop down open; otherwise, <c>false</c>.
        /// </value>
        public bool IsDropDownOpen
        {
            get
            {
                return (bool)GetValue(IsDropDownOpenProperty);
            }

            set
            {
                SetValue(IsDropDownOpenProperty, value);
            }
        }
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="GalleryFilterSelector"/> class.
        /// </summary>
        static GalleryFilterSelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GalleryFilterSelector), new FrameworkPropertyMetadata(typeof(GalleryFilterSelector)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GalleryFilterSelector"/> class.
        /// </summary>
        public GalleryFilterSelector()
        {
            Mouse.AddPreviewMouseDownOutsideCapturedElementHandler(this, OnPreviewMouseDownOutsideCapturedElement);
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Provides class handling for the MouseLeftButtonDown routed event that occurs when the left mouse 
        /// button is pressed while the mouse pointer is over this control.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            m_dropDownButton = GetTemplateChild("PART_DropDownButton") as FrameworkElement;
            if (m_dropDownButton != null)
            {
                m_dropDownButton.MouseDown += new MouseButtonEventHandler(M_DropDownButton_MouseDown);

                 #if !SyncfusionFramework3_5
                m_dropDownButton.TouchDown += new EventHandler<TouchEventArgs>(m_dropDownButton_TouchDown);
                #endif
            }

            m_popup = GetTemplateChild("PART_Popup") as Popup;
        }

         #if !SyncfusionFramework3_5
        void m_dropDownButton_TouchDown(object sender, TouchEventArgs e)
        {
            IsDropDownOpen = !IsDropDownOpen;
        }
        #endif

        /// <summary>
        /// Handles the MouseDown event of the m_dropDownButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void M_DropDownButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            IsDropDownOpen = !IsDropDownOpen;
        }

        /// <summary>
        /// Handles the Click event of the m_dropDownButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void M_DropDownButton_Click(object sender, RoutedEventArgs e)
        {
            IsDropDownOpen = !IsDropDownOpen;
        }

        /// <summary>
        /// Called when [preview mouse down outside captured element].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnPreviewMouseDownOutsideCapturedElement(object sender, MouseButtonEventArgs e)
        {
            if (e.Source == this)
            {
                IsDropDownOpen = false;
            }
        }

        /// <summary>
        /// Calls OnIsDropDownOpenChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GalleryFilterSelector instance = (GalleryFilterSelector)d;
            instance.OnIsDropDownOpenChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises IsDropDownOpenChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnIsDropDownOpenChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsDropDownOpen == true)
            {
                Mouse.Capture(this, CaptureMode.SubTree);
            }
            else
            {
                Mouse.Capture(null);
            }

            if (IsDropDownOpenChanged != null)
            {
                IsDropDownOpenChanged(this, e);
            }
        }

        /// <summary>
        /// Invoked when an unhandled Mouse.GotMouseCapture attached
        /// event reaches an element in its route that is derived from
        /// this class. Implement this method to add class handling for
        /// this event.
        /// </summary>
        /// <param name="e">The MouseEventArgs that contains the event
        /// data.</param>
        protected override void OnGotMouseCapture(MouseEventArgs e)
        {
                        base.OnGotMouseCapture(e);
            if (m_wasPressed)
            {
                m_stack.Clear();
                m_popup.IsOpen = false;
            }
            else
            {
               if (m_stack.Count > 0 && (m_stack.Peek().GetType() == typeof(GalleryFilterSelector) || m_stack.Peek().ToString() == "System.Windows.Controls.Primitives.PopupRoot"))
                {
                    AutomationPeer peer = UIElementAutomationPeer.CreatePeerForElement(!(e.OriginalSource is RepeatButton) ? e.OriginalSource as UIElement : e.Source as UIElement);

                    if (peer != null && peer.IsControlElement())
                    {
                        if (peer is IInvokeProvider)
                        {
                            m_wasPressed = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Invoked when an unhandled Mouse.LostMouseCapture attached
        /// event reaches an element in its route that is derived from
        /// this class. Implement this method to add class handling for
        /// this event.
        /// </summary>
        /// <param name="e">TheMouseEventArgs that contains event data.</param>
        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            Debug.WriteLine(Name);
            Debug.WriteLine("OnLostMouseCapture e.Source " + e.Source);
            Debug.WriteLine("OnLostMouseCapture e.OriginalSource " + e.OriginalSource);

            m_stack.Push(e.OriginalSource);
            base.OnLostMouseCapture(e);
            m_wasPressed = false;
        }

        /// <summary>
        /// Event that is raised when IsDropDownOpen property is changed.
        /// </summary>
        public event PropertyChangedCallback IsDropDownOpenChanged;
        #endregion
    }
}
