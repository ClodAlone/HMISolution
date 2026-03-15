// <copyright file="SplitterPage_Properties.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the TabSplitter class
    /// </summary>

    public partial class TabSplitter
    {
        #region Events

        /// <summary>
        /// Event that is raised when MouseOverBackground property is changed.
        /// </summary>
        public event PropertyChangedCallback MouseOverBackgroundChanged;

        /// <summary>
        /// Event that is raised when MouseOverBackground property is changed.
        /// </summary>
        public event PropertyChangedCallback MouseOverForegroundChanged;

        /// <summary>
        /// Occurs when [selected background changed].
        /// </summary>
        public event PropertyChangedCallback SelectedBackgroundChanged;

        /// <summary>
        /// Occurs when [selected foreground changed].
        /// </summary>
        public event PropertyChangedCallback SelectedForegroundChanged;
        #endregion;

        #region Properties

        /// <summary>
        /// Gets selected splitter item.
        /// </summary>
        internal TabSplitterItem SelectedSplitterItem
        {
            get
            {
                return (TabSplitterItem)SelectedItem;
            }
        }

        /// <summary>
        /// Gets or sets the value of the MouseOverBackground dependency property.
        /// </summary>
        public Brush MouseOverBackground
        {
            get
            {
                return (Brush)GetValue(MouseOverBackgroundProperty);
            }

            set
            {
                SetValue(MouseOverBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the mouse over foreground.
        /// </summary>
        /// <value>The mouse over foreground.</value>
        public Brush MouseOverForeground
        {
            get
            {
                return (Brush)GetValue(MouseOverForegroundProperty);
            }

            set
            {
                SetValue(MouseOverForegroundProperty, value);
            }
        }


        /// <summary>
        /// Hides the Header On Single Child
        /// </summary>
        public bool HideHeaderOnSingleChild
        {
            get
            {
                return (bool)GetValue(HideHeaderOnSingleChildProperty);
            }
            set
            {
                SetValue(HideHeaderOnSingleChildProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selected background.
        /// </summary>
        /// <value>The selected background.</value>
        public Brush SelectedBackground
        {
            get
            {
                return (Brush)GetValue(SelectedBackgroundProperty);
            }

            set
            {
                SetValue(SelectedBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selected foreground.
        /// </summary>
        /// <value>The selected foreground.</value>
        public Brush SelectedForeground
        {
            get
            {
                return (Brush)GetValue(SelectedForegroundProperty);
            }

            set
            {
                SetValue(SelectedForegroundProperty, value);
            }
        }
        #endregion

        #region DP Getters and Setters

        /// <summary>
        /// Raises MouseOverBackgroundChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnMouseOverBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (null != MouseOverBackgroundChanged)
            {
                MouseOverBackgroundChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnMouseOverBackgroundChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnMouseOverBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabSplitter instance = (TabSplitter)d;
            instance.OnMouseOverBackgroundChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:MouseOverForegroundChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnMouseOverForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (null != MouseOverForegroundChanged)
            {
                MouseOverForegroundChanged(this, e);
            }
        }

        /// <summary>
        /// Called when [mouse over foreground changed].
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnMouseOverForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabSplitter instance = (TabSplitter)d;
            instance.OnMouseOverBackgroundChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:SelectedBackgroundChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnSelectedBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (null != SelectedBackgroundChanged)
            {
                SelectedBackgroundChanged(this, e);
            }
        }

        /// <summary>
        /// Called when [mouse over foreground changed].
        /// </summary>
        /// <param name="d">The d DependencyObject Background changed.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSelectedBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabSplitter instance = (TabSplitter)d;
            instance.OnSelectedBackgroundChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:SelectedForegroundChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnSelectedForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (null != SelectedForegroundChanged)
            {
                SelectedForegroundChanged(this, e);
            }
        }

        /// <summary>
        /// Called when [mouse over foreground changed].
        /// </summary>
        /// <param name="d">The d DependencyObject of selected foreground.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSelectedForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabSplitter instance = (TabSplitter)d;
            instance.OnSelectedForegroundChanged(e);
        }

        #endregion

        #region Dependency Properies

        /// <summary>
        /// MouseOverForeground DependencyProperty
        /// </summary>
        public static readonly DependencyProperty MouseOverForegroundProperty = DependencyProperty.Register("MouseOverForeground", typeof(Brush), typeof(TabSplitter), new FrameworkPropertyMetadata(Brushes.Transparent, new PropertyChangedCallback(OnMouseOverForegroundChanged)));

        /// <summary>
        /// MouseOverBackground DependencyProperty
        /// </summary>
        public static readonly DependencyProperty MouseOverBackgroundProperty = DependencyProperty.Register("MouseOverBackground", typeof(Brush), typeof(TabSplitter), new FrameworkPropertyMetadata(Brushes.Transparent, new PropertyChangedCallback(OnMouseOverBackgroundChanged)));

        /// <summary>
        /// SelectedBackground DependencyProperty
        /// </summary>
        public static readonly DependencyProperty SelectedBackgroundProperty = DependencyProperty.Register("SelectedBackground", typeof(Brush), typeof(TabSplitter), new FrameworkPropertyMetadata(Brushes.Transparent, new PropertyChangedCallback(OnSelectedBackgroundChanged)));

        /// <summary>
        /// SelectedForeground DependencyProperty
        /// </summary>
        public static readonly DependencyProperty SelectedForegroundProperty = DependencyProperty.Register("SelectedForeground", typeof(Brush), typeof(TabSplitter), new FrameworkPropertyMetadata(Brushes.Transparent, new PropertyChangedCallback(OnSelectedForegroundChanged)));

        #endregion
    }
}
