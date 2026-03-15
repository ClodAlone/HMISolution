// <copyright file="Expander.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Overrides standard <see cref="Expander"/> control.
    /// Adds <see cref="ChildHasFocus"/> property to extend functional possibilities.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ExpanderExt : Expander
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [child has focus].
        /// </summary>
        /// <value><c>true</c> if [child has focus]; otherwise, <c>false</c>.</value>
        public bool ChildHasFocus
        {
            get
            {
                return (bool)GetValue(ChildHasFocusProperty);
            }

            protected set
            {
                SetValue(ChildHasFocusPropertyKey, value);
            }
        }

        #endregion Properties

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="ExpanderExt"/> class.
        /// </summary>
        static ExpanderExt()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExpanderExt), new FrameworkPropertyMetadata(typeof(ExpanderExt)));
        }

        #endregion Initialization

        #region Implementation

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            this.Loaded += new RoutedEventHandler(ExpanderExt_Loaded);
        }

        /// <summary>
        ///  Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.GotKeyboardFocus"/>�attached
        ///  event reaches an element in its route that is derived from this class.
        ///  Sets to true <see cref="ChildHasFocus"/> property.
        /// </summary>
        /// <param name="e">The instance containing the event data.</param>
        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);
            ChildHasFocus = true;
        }

        /// <summary>
        ///  Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.LostKeyboardFocus"/>�attached
        ///  event reaches an element in its route that is derived from this class.
        ///  Sets to false <see cref="ChildHasFocus"/> property.
        /// </summary>
        /// <param name="e">The instance containing the event data.</param>
        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);
            ChildHasFocus = false;
        }

        /// <summary>
        /// This method adds header to expander.
        /// <para/>
        /// Note:
        /// <para/>
        /// Before adding new header removes it from the logical tree of the previous parent.
        /// </summary>
        /// <param name="oldHeader">Object that represents old header.</param>
        /// <param name="newHeader">Object that represents new header.</param>
        protected override void OnHeaderChanged(object oldHeader, object newHeader)
        {
            FrameworkElement element = newHeader as FrameworkElement;
            if (null != element)
            {
                TaskBarItem item = element.Parent as TaskBarItem;
                if (null != item)
                {
                    item.WrappedRemoveLogicalChild(newHeader);
                    base.OnHeaderChanged(oldHeader, newHeader);
                }
            }
        }

        /// <summary>
        /// Handles the Loaded event of the ExpanderExt control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ExpanderExt_Loaded(object sender, RoutedEventArgs e)
        {
            if (!TaskBar.GetIsOpened(this))
            {
                base.RaiseEvent(new RoutedEventArgs(ExpanderExt.CollapsedEvent, this));
            }
        }

        #endregion Implementation

        #region dependency Properties

        /// <summary>
        /// Identifies <see cref="ChildHasFocus"/> dependency property key.
        /// </summary>
        protected static readonly DependencyPropertyKey ChildHasFocusPropertyKey =
            DependencyProperty.RegisterReadOnly("ChildHasFocus", typeof(bool), typeof(ExpanderExt), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Identifies <see cref="ChildHasFocus"/> read-only dependency property.
        /// </summary>
        public static readonly DependencyProperty ChildHasFocusProperty = ChildHasFocusPropertyKey.DependencyProperty;

        #endregion dependency Properties
    }
}