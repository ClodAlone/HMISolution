// <copyright file="SmartTagPresenter.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;

namespace Syncfusion.Windows.Design
{
    /// <summary>
    /// SmartTagPrsenter class helps us to provide the presentation of SmartTag designer support to the controls.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class SmartTagPresenter : ContentControl
    {
        #region Private Members

        /// <summary>
        /// Declaration of CloseButton
        /// </summary>
        private Button m_closeButton;

        /// <summary>
        /// Declaration of Popup
        /// </summary>
        private Popup m_popup;

        /// <summary>
        /// Declaration of Bool value for drop-down open
        /// </summary>
        private bool m_skipDropDownOpen = false;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="SmartTagPresenter"/> class.
        /// </summary>
        static SmartTagPresenter()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(SmartTagPresenter), new FrameworkPropertyMetadata(typeof(SmartTagPresenter)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SmartTagPresenter"/> class.
        /// </summary>
        public SmartTagPresenter()
        {
            Mouse.AddPreviewMouseDownOutsideCapturedElementHandler(this, OnPreviewMouseDownOutsideCapturedElement);
        }

        #endregion

        #region DP getters & setters
        
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get
            {
                return (string)GetValue(TitleProperty);
            }

            set
            {
                SetValue(TitleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is drop down open.
        /// </summary>       
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

        #region Override methods

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_closeButton = (Button)GetTemplateChild("PART_CloseButton");

            if (m_closeButton != null)
            {
                m_closeButton.Click += new RoutedEventHandler(CloseButton_Click);
            }

            m_popup = (Popup)GetTemplateChild("PART_Popup");

            if (m_popup != null)
            {
                m_popup.MouseDown += new System.Windows.Input.MouseButtonEventHandler(Popup_MouseDown);
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.PreviewMouseDown"/> attached routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that one or more mouse buttons were pressed.</param>
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (m_skipDropDownOpen)
            {
                e.Handled = true;
                m_skipDropDownOpen = false;
                return;
            }

            base.OnPreviewMouseDown(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseEnter"/> attached event is raised on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            m_skipDropDownOpen = false;
            base.OnMouseEnter(e);
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Calls OnTitleChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SmartTagPresenter instance = (SmartTagPresenter)d;
            instance.OnTitleChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises TitleChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnTitleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (TitleChanged != null)
            {
                TitleChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnIsDropDownOpenChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SmartTagPresenter instance = (SmartTagPresenter)d;
            instance.OnIsDropDownOpenChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises IsDropDownOpenChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnIsDropDownOpenChanged(DependencyPropertyChangedEventArgs e)
        {
            SmartTagBase sbase = this.Parent as SmartTagBase;
            if (IsDropDownOpen == true)
            {
                if (sbase != null)
                {
                    PrimarySelectionAdornerProviderBase.IsPopUpActive = true;
                }

                Mouse.Capture(this, CaptureMode.SubTree);

                // Keyboard.Focus( this );
            }
            else
            {
                if (sbase != null)
                {
                    PrimarySelectionAdornerProviderBase.IsPopUpActive = false;
                }

                Mouse.Capture(null);
            }

            if (IsDropDownOpenChanged != null)
            {
                IsDropDownOpenChanged(this, e);
            }
        }

        /// <summary>
        /// Handles the Click event of the CloseButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            IsDropDownOpen = false;
        }

        /// <summary>
        /// Handles the MouseDown event of the Popup control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void Popup_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// Called when [preview mouse down outside captured element].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnPreviewMouseDownOutsideCapturedElement(object sender, MouseButtonEventArgs e)
        {
            IsDropDownOpen = false;
            m_skipDropDownOpen = true;
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.PreviewKeyDown"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs"/> that contains the event data.</param>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.None && e.Key == Key.Tab)
            {
                MoveFocusToNextItem();
                e.Handled = true;
                return;
            }

            if ((e.KeyboardDevice.Modifiers == ModifierKeys.Shift) && (e.Key == Key.Tab))
            {
                MoveFocusToPreviousItem();
                e.Handled = true;
                return;
            }

            base.OnPreviewKeyDown(e);
        }

        /// <summary>
        /// Moves the focus to next item.
        /// </summary>
        private void MoveFocusToNextItem()
        {
            TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
            UIElement focusedElement = Keyboard.FocusedElement as UIElement;

            if (focusedElement != null)
            {
                focusedElement.MoveFocus(request);
            }
            else
            {
                ContentElement focusedcontentElement = Keyboard.FocusedElement as ContentElement;
                if (focusedcontentElement != null)
                {
                    focusedcontentElement.MoveFocus(request);
                }
            }
        }

        /// <summary>
        /// Moves the focus to previous item.
        /// </summary>
        private void MoveFocusToPreviousItem()
        {
            TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Previous);
            UIElement focusedElement = Keyboard.FocusedElement as UIElement;
            
            if (focusedElement != null)
            {
                focusedElement.MoveFocus(request);
            }
            else
            {
                ContentElement focusedcontentElement = Keyboard.FocusedElement as ContentElement;
                if (focusedcontentElement != null)
                {
                    focusedcontentElement.MoveFocus(request);
                }
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Event that is raised when Title property is changed.
        /// </summary>
        public event PropertyChangedCallback TitleChanged;

        /// <summary>
        /// Event that is raised when IsDropDownOpen property is changed.
        /// </summary>
        public event PropertyChangedCallback IsDropDownOpenChanged;

        #endregion

        #region Dependency properties

        /// <summary>
        /// Title of smart tag
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(SmartTagPresenter), new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback(OnTitleChanged)));

        /// <summary>
        /// DropDown box is customized
        /// </summary>
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(SmartTagPresenter), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsDropDownOpenChanged)));

        #endregion
    }
}
