// <copyright file="RibbonItemsControl.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.Windows.Shared;
using System.Windows.Data;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents base abstract class for Ribbon ItemsControls.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public abstract class RibbonItemsControl : ItemsControl, ICollapsable
    {
        #region Private members

        /// <summary>
        /// Used to represent the Large label.
        /// </summary>
        internal string tempLabel = null;

        /// <summary>
        /// Framework element which is obligatory part of template.
        /// Represents toggle button.
        /// </summary>
        private FrameworkElement m_toggleButton;

        /// <summary>
        /// Popup which is obligatory part of template.
        /// </summary>
        private Popup m_popup;

        /// <summary>
        /// Represents the Stack object
        /// </summary>
        private Stack<object> m_stack = new Stack<object>();

        /// <summary>
        /// Flag which is used for correct popup closing and opening. 
        /// </summary>
        private bool m_wasPressed = false;

        /// <summary>
        /// Flag which indicates when popup should be opened/closed.
        /// </summary>
        private bool m_skipDropDownOpen = false;

        /// <summary>
        /// Label itemscontrol.
        /// </summary>
        private ItemsControl m_label = null;

        SystemGesture m_systemGesture;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonItemsControl"/> class.
        /// </summary>
        public RibbonItemsControl()
        {
            Mouse.AddPreviewMouseDownOutsideCapturedElementHandler(this, OnPreviewMouseDownOutsideCapturedElement);
            this.FocusVisualStyle = null;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets internal toggle button control.
        /// </summary>
        protected internal FrameworkElement ToggleButton
        {
            get
            {
                return m_toggleButton;
            }
        }

        /// <summary>
        /// Gets internal Popup control.
        /// </summary>
        protected internal Popup Popup
        {
            get
            {
                return m_popup;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is multi line.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is multi line; otherwise, <c>false</c>.
        /// </value>
        public bool IsMultiLine
        {
            get
            {
                return (bool)GetValue(IsMultiLineProperty);
            }
            set
            {
                SetValue(IsMultiLineProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [split label into two line].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [split label into two line]; otherwise, <c>false</c>.
        /// </value>
        public bool SplitLabelIntoTwoLine
        {
            get
            {
                return (bool)GetValue(SplitLabelIntoTwoLineProperty);
            }
            set
            {
                SetValue(SplitLabelIntoTwoLineProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the collapse label.
        /// </summary>
        /// <value>The collapse label.</value>
        public String CollapseLabel
        {
            get { return (String)GetValue(CollapseLabelProperty); }
            set { SetValue(CollapseLabelProperty, value); }
        }        

        #endregion

        #region Dependency properties

        /// <summary>
        /// Identifies the IsMultiline property.
        /// </summary>
        protected static readonly DependencyProperty IsMultiLineProperty =
           DependencyProperty.Register("IsMultiLine", typeof(bool), typeof(RibbonItemsControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsMultiLineChanged), new CoerceValueCallback(CoerceOnIsMultiline)));

        /// <summary>
        /// Identifies the SplitIntoTwoLine property.
        /// </summary>
        protected static readonly DependencyProperty SplitLabelIntoTwoLineProperty =
           DependencyProperty.Register("SplitLabelIntoTwoLine", typeof(bool), typeof(RibbonItemsControl), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnSplitLabelIntoTwoLineChanged)));


        /// <summary>
        /// Identifies the text that labels the control
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(RibbonItemsControl), new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback(OnLabelChanged), new CoerceValueCallback(CoerceOnLabel)));

        /// <summary>
        /// Identifies type of the control size.
        /// </summary>
        public static readonly DependencyProperty SizeFormProperty =
            DependencyProperty.Register("SizeForm", typeof(SizeForm), typeof(RibbonItemsControl), new FrameworkPropertyMetadata(SizeForm.Small, new PropertyChangedCallback(OnSizeFormChanged)));

        /// <summary>
        /// Identifies the small icon that appears in the control.
        /// </summary>
        public static readonly DependencyProperty SmallIconProperty =
            DependencyProperty.Register("SmallIcon", typeof(ImageSource), typeof(RibbonItemsControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSmallIconChanged)));

        /// <summary>
        /// Identifies the small icon is set in the control.
        /// </summary>
        protected internal static readonly DependencyProperty IsSmallImageVisibleProperty =
            DependencyProperty.Register("IsSmallImageVisible", typeof(bool), typeof(RibbonItemsControl), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Identifies the large icon that appears in the control.
        /// </summary>
        public static readonly DependencyProperty LargeIconProperty =
            DependencyProperty.Register("LargeIcon", typeof(ImageSource), typeof(RibbonItemsControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnLargeIconChanged)));

        /// <summary>
        /// Identifies whether large icon is set in the control.
        /// </summary>
        protected internal static readonly DependencyProperty IsLargeImageVisibleProperty =
            DependencyProperty.Register("IsLargeImageVisible", typeof(bool), typeof(RibbonItemsControl), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Identifies whether the dropdown popup is open.
        /// </summary>
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(RibbonItemsControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsDropDownOpenChanged), OnCoerceIsDropDownOpened));


        // Using a DependencyProperty as the backing store for CollapseLabel.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Used to represent the CollpaseLabel. It is a dependency property.
        /// </summary>
        public static readonly DependencyProperty CollapseLabelProperty =
            DependencyProperty.Register("CollapseLabel", typeof(String), typeof(RibbonItemsControl), new UIPropertyMetadata(null));
        
        #endregion

        #region DP getters & setters
        /// <summary>
        /// Gets or sets the text that labels the control.
        /// </summary>
        /// <value>
        /// Type: <see cref="String"/>
        /// Text that labels the control. The default is empty string.
        /// </value>
        /// <seealso cref="string"/>
        public string Label
        {
            get
            {
                return (string)GetValue(LabelProperty);
            }

            set
            {
                SetValue(LabelProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the size form.
        /// </summary>
        /// <value>
        /// Type: <see cref="SizeForm"/>
        /// Enumeration that specifies type of the control size.
        /// </value>
        /// <example>
        /// <code>
        /// DropDownButton button = new DropDownButton();
        /// button.Label = "Button";        
        /// button.SizeForm = SizeForm.Large;
        /// </code>
        /// </example>
        /// <remarks>
        /// Changing control's SizeForm property changes it's visual representation.  Text is only rendered in Large and Small variants.
        /// </remarks>        
        /// <seealso cref="SizeForm"/>
        public SizeForm SizeForm
        {
            get
            {
                return (SizeForm)GetValue(SizeFormProperty);
            }

            set
            {
                SetValue(SizeFormProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the small icon that appears in the control.
        /// </summary>
        /// <remarks>
        /// Many controls have more than just text in the element. Often there is an icon. 
        /// </remarks>
        /// <example>
        /// <code>
        /// // Create the image element.
        /// DropDownButton button = new DropDownButton();
        /// // Create source.
        /// BitmapImage bimage = new BitmapImage();
        /// // BitmapImage.UriSource must be in a BeginInit/EndInit block.
        /// bimage.BeginInit();
        /// bimage.UriSource = new Uri(@"/sampleImages/sample.jpg",UriKind.RelativeOrAbsolute);
        /// bimage.EndInit();
        /// // Set the image source.
        /// button.SmallIcon = bimage;
        /// </code>
        /// </example>
        /// <seealso cref="ImageSource"/>
        public ImageSource SmallIcon
        {
            get
            {
                return (ImageSource)GetValue(SmallIconProperty);
            }

            set
            {
                SetValue(SmallIconProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the large icon that appears in the control.
        /// </summary>
        /// <remarks>
        /// Many controls have more than just text in the element. Often there is an icon. 
        /// </remarks>
        /// <example>
        /// <code>
        /// // Create the image element.
        /// DropDownButton button = new DropDownButton();
        /// // Create source.
        /// BitmapImage bimage = new BitmapImage();
        /// // BitmapImage.UriSource must be in a BeginInit/EndInit block.
        /// bimage.BeginInit();
        /// bimage.UriSource = new Uri(@"/sampleImages/sample.jpg",UriKind.RelativeOrAbsolute);
        /// bimage.EndInit();
        /// // Set the image source.
        /// button.LargeIcon = bimage;
        /// </code>
        /// </example>
        /// <seealso cref="RibbonButton"/>
        /// <seealso cref="ImageSource"/>
        public ImageSource LargeIcon
        {
            get
            {
                return (ImageSource)GetValue(LargeIconProperty);
            }

            set
            {
                SetValue(LargeIconProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the dropdown popup is open.
        /// </summary>
        /// <value>
        /// Type: <see cref="Boolean"/>
        /// True is popup is open, false is closed.
        /// </value>
        /// <example>
        /// <code>
        /// DropDownButton button = new DropDownButton();
        /// // Create source.
        /// // ....
        /// button.IsDropDownOpen = true;
        /// </code>
        /// </example>
        /// <seealso cref="Boolean"/>
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

        /// <summary>
        /// Gets or sets a value indicating whether this instance small image is set.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is small image visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsSmallImageVisible
        {
            get
            {
                return (bool)GetValue(IsSmallImageVisibleProperty);
            }

            set
            {
                SetValue(IsSmallImageVisibleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance large image is set.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is large image visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsLargeImageVisible
        {
            get
            {
                return (bool)GetValue(IsLargeImageVisibleProperty);
            }

            set
            {
                SetValue(IsLargeImageVisibleProperty, value);
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when Label property is changed.
        /// </summary>
        public event PropertyChangedCallback LabelChanged;

        /// <summary>
        /// Event that is raised when SizeForm property is changed.
        /// </summary>
        public event PropertyChangedCallback SizeFormChanged;

        /// <summary>
        /// Event that is raised when SmallIcon property is changed.
        /// </summary>
        public event PropertyChangedCallback SmallIconChanged;

        /// <summary>
        /// Event that is raised when LargeIcon property is changed.
        /// </summary>
        public event PropertyChangedCallback LargeIconChanged;

        /// <summary>
        /// Event that is raised when IsDropDownOpen property is changed.
        /// </summary>
        public event PropertyChangedCallback IsDropDownOpenChanged;

        /// <summary>
        /// Event which is raised before popup is opened.
        /// </summary>
        internal event CancelEventHandler BeforeDropDownPopup;

        /// <summary>
        /// Event which is raised after popup is closed.
        /// </summary>
        internal event EventHandler AfterDropDownPopup;

        /// <summary>
        /// Occurs when [is multi line changed].
        /// </summary>
        public event PropertyChangedCallback IsMultiLineChanged;

        /// <summary>
        /// Occurs when [split label into two line changed].
        /// </summary>
        public event PropertyChangedCallback SplitLabelIntoTwoLineChanged;

        #endregion

        #region Implementation

        /// <summary>
        /// Called when [is multi line changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsMultiLineChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonItemsControl instance = (RibbonItemsControl)d;

            instance.OnIsMultiLineChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:IsMultiLineChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnIsMultiLineChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsMultiLineChanged != null)
            {
                IsMultiLineChanged(this, e);
            }
        }

        /// <summary>
        /// Called when [split into two line changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSplitLabelIntoTwoLineChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonItemsControl instance = (RibbonItemsControl)d;

            instance.OnSplitLabelIntoTwoLineChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:SplitIntoTwoLineChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnSplitLabelIntoTwoLineChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SplitLabelIntoTwoLineChanged != null)
            {
                SplitLabelIntoTwoLineChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnLabelChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonItemsControl instance = (RibbonItemsControl)d;
            instance.OnLabelChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises LabelChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnLabelChanged(DependencyPropertyChangedEventArgs e)
        {
            if (LabelChanged != null)
            {
                LabelChanged(this, e);
            }
            
           

            if (this.Tag==null || !this.Tag.ToString().Equals("Resized", StringComparison.OrdinalIgnoreCase))
            {

                this.tempLabel = this.Label;
            }
        }

        /// <summary>
        /// Coerces the on label.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="baseValue">The base value.</param>
        /// <returns></returns>
        public static object CoerceOnLabel(DependencyObject d, object baseValue)
        {
            RibbonItemsControl instance = (RibbonItemsControl)d;
            return instance.CoerceOnLabel(baseValue);
        }

        /// <summary>
        /// Coerces the on label.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected virtual object CoerceOnLabel(object value)
        {

            string coerceValue = (string)value;
            if (coerceValue != null)
            {
                if (IsMultiLine)
                {
                    coerceValue = coerceValue.Replace("\\n", "\n");
                }
                else
                {
                    coerceValue = coerceValue.Replace("\n", "\n");
                }
                if (SizeForm == SizeForm.Large)
                    coerceValue = coerceValue.Replace("\\t", " ");
            }
            return coerceValue;
        }

        /// <summary>
        /// Coerces the on is multiline.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="baseValue">The base value.</param>
        /// <returns></returns>
        public static object CoerceOnIsMultiline(DependencyObject d, object baseValue)
        {
            RibbonItemsControl instance = (RibbonItemsControl)d;
            return instance.CoerceOnIsMultiline(baseValue);
        }

        /// <summary>
        /// Coerces the on is multiline.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected virtual object CoerceOnIsMultiline(object value)
        {

            bool coerceValue = (bool)value;

            if (coerceValue)
            {
                if (Label != null)
                {
                    Label = Label.Replace("\\n", "\n");
                }
            }
            else
            {
                if (Label != null)
                {
                    Label = Label.Replace("\n", "\\n");
                }
            }


            return coerceValue;
        }

        /// <summary>
        /// Calls OnSizeFormChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSizeFormChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonItemsControl instance = (RibbonItemsControl)d;
            instance.OnSizeFormChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises SizeFormChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnSizeFormChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SizeFormChanged != null)
            {
                SizeFormChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnSmallIconChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSmallIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonItemsControl instance = (RibbonItemsControl)d;
            instance.IsSmallImageVisible = (instance.SmallIcon != null);
            instance.OnSmallIconChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises SmallIconChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnSmallIconChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SmallIconChanged != null)
            {
                SmallIconChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnLargeIconChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnLargeIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonItemsControl instance = (RibbonItemsControl)d;
            instance.IsLargeImageVisible = (instance.LargeIcon != null);
            instance.OnLargeIconChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises LargeIconChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnLargeIconChanged(DependencyPropertyChangedEventArgs e)
        {
            if (LargeIconChanged != null)
            {
                LargeIconChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnIsDropDownOpenChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonItemsControl instance = (RibbonItemsControl)d;
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
                Keyboard.Focus(this);
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
        /// Coerces IsDropDownOpen dependency property.
        /// </summary>
        /// <param name="d">Instance where the property should be
        /// coerced.</param>
        /// <param name="value">The value which is proposed to be
        /// applied. </param>
        /// <returns>Boolean value to open the drop down or not</returns>
        private static object OnCoerceIsDropDownOpened(DependencyObject d, object value)
        {
            if ((RibbonItemsControl)d != null)
            {
                RibbonItemsControl itemsControl = (RibbonItemsControl)d;
                bool isDropDownOpened = (bool)value;
                if (isDropDownOpened)
                {
                    CancelEventArgs eventData = new CancelEventArgs();
                    itemsControl.FireBeforeDropDownPopup(eventData);

                    if (eventData.Cancel)
                    {
                        return false;
                    }
                }

                if (Mouse.Captured is RibbonContextMenu && false == (bool)value)
                {
                    if (itemsControl.m_popup != null)
                    {
                        itemsControl.m_popup.IsOpen = true;
                        return true;
                    }
                }
            }
            return value;
        }

        /// <summary>
        /// Fires the before drop down popup.
        /// </summary>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        protected virtual void FireBeforeDropDownPopup(CancelEventArgs e)
        {
            if (BeforeDropDownPopup != null)
            {
                BeforeDropDownPopup(this, e);
            }
        }

        /// <summary>
        /// Raises after DropDownPopup event.
        /// </summary>
        protected virtual void FireAfterDropDownPopup()
        {
            if (AfterDropDownPopup != null)
            {
                AfterDropDownPopup(this, new EventArgs());
            }
        }

        /// <summary>
        /// Executes when the popup is opened.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" />
        /// instance containing the event data.</param>
        protected virtual void Popup_Opened(object sender, EventArgs e)
        {
            RibbonBar bar = VisualUtils.FindAncestor(this, typeof(RibbonBar)) as RibbonBar;
            if (null != bar)
            {
                if (bar.MeasureMode == MeasureMode.Compressed && bar.PanelState==RibbonBarState.Collapsed)
                {
                    bar.Tag = "Expanded Manually";
                    bar.SetToDefault();
                }
            }
            Popup popup = sender as Popup;
            FrameworkElement child = popup.Child as FrameworkElement;
            //if (child != null)
            //    child.Measure(new Size(0, 0));
        }

        /// <summary>
        /// Handles the Closed event of the Popup control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void Popup_Closed(object sender, EventArgs e)
        {
            RibbonBar bar = VisualUtils.FindAncestor(this, typeof(RibbonBar)) as RibbonBar;
            if (bar != null)
            {
                if (bar.MeasureMode == MeasureMode.Default)
                {
                    if (bar.Tag != null)
                    {
                        if (bar.Tag.Equals("Expanded Manually"))
                        {
                            bar.CompressLargeItems();
                            bar.CompressSmallItems();
                        }
                    }
                }
            }
            FireAfterDropDownPopup();
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the Popup control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void Popup_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the Popup control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void Popup_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Visual source = e.OriginalSource as Visual;
            if (source == null)
            {
                source = LogicalTreeHelper.GetParent(e.OriginalSource as DependencyObject) as Visual;
            }

            RibbonItemsControl ribbonItemsControl = VisualUtils.FindAncestor(source, typeof(RibbonItemsControl)) as RibbonItemsControl;

            if (ribbonItemsControl != null && VisualUtils.IsDescendant(ribbonItemsControl.m_toggleButton, (DependencyObject)e.OriginalSource))
            {
                return;
            }

            if (VisualUtils.FindAncestor(source, typeof(ICommandSource)) != null && Mouse.Captured != VisualUtils.FindAncestor(source, typeof(ICommandSource)))
            {
                IsDropDownOpen = false;
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the ToggleButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void ToggleButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            m_skipDropDownOpen = false;
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the ToggleButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        protected virtual void ToggleButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (m_skipDropDownOpen == false)
            {
                IsDropDownOpen = !IsDropDownOpen;
            }

            m_skipDropDownOpen = false;
        }

        /// <summary>
        /// Called when [preview mouse down outside captured element].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnPreviewMouseDownOutsideCapturedElement(object sender, MouseButtonEventArgs e)
        {
            if (m_popup.IsMouseCaptureWithin)
            {
                m_skipDropDownOpen = true;
            }

            if (e.Source == this)
            {
                IsDropDownOpen = false;
            }
        }

        #endregion

        #region Overrides
        /// <summary>
        /// When overridden in a derived class, is invoked whenever
        /// application code or internal processes call ApplyTemplate.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (m_toggleButton != null)
            {
                m_toggleButton.MouseLeftButtonDown -= new MouseButtonEventHandler(ToggleButton_MouseLeftButtonDown);
                m_toggleButton.MouseLeftButtonUp -= new MouseButtonEventHandler(ToggleButton_MouseLeftButtonUp);
            }

            if (m_popup != null)
            {
                m_popup.Opened -= new EventHandler(Popup_Opened);
                m_popup.MouseLeftButtonDown -= new MouseButtonEventHandler(Popup_MouseLeftButtonDown);
                m_popup.MouseLeftButtonUp -= new MouseButtonEventHandler(Popup_MouseLeftButtonUp);
                m_popup.Closed -= new EventHandler(Popup_Closed);
            }

            m_toggleButton = GetTemplateChild("PART_ToggleButton") as FrameworkElement;
            m_popup = GetTemplateChild("PART_Popup") as Popup;
            if (SizeForm == SizeForm.Large)
            {
                m_label = (ItemsControl)GetTemplateChild("label");
            }

            if (m_toggleButton != null)
            {
                m_toggleButton.MouseLeftButtonDown += new MouseButtonEventHandler(ToggleButton_MouseLeftButtonDown);
                m_toggleButton.MouseLeftButtonUp += new MouseButtonEventHandler(ToggleButton_MouseLeftButtonUp);
            }

            if (m_popup != null)
            {
                m_popup.Opened += new EventHandler(Popup_Opened);
                m_popup.MouseLeftButtonDown += new MouseButtonEventHandler(Popup_MouseLeftButtonDown);
                m_popup.MouseLeftButtonUp += new MouseButtonEventHandler(Popup_MouseLeftButtonUp);
                m_popup.Closed += new EventHandler(Popup_Closed);
            }

            Binding binding = new Binding("Label");
            binding.Source = this;
            TextWrappingConverter conv = new TextWrappingConverter();
            binding.Converter = conv;
            binding.ConverterParameter = SplitLabelIntoTwoLine;
            if (m_label != null)
            {
                m_label.SetBinding(ItemsControl.ItemsSourceProperty, binding);
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseRightButtonUp"/>�routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the right mouse button was released.</param>
        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(e);

            if (!(this.TemplatedParent is RibbonBar) && e.Source == this)
            {
                FrameworkElement realSource = RibbonContextMenu.GetRealSource(this);
                if (realSource != null)
                {
                    if (!realSource.IsEnabled)
                    {
                        ContextMenuService.SetShowOnDisabled(realSource, true);
                        RibbonContextMenu.CreateContextMenu(realSource);
                        return;
                    }
                }

                this.IsDropDownOpen = false;

                Ribbon ribbon = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
                FrameworkElement fe = VisualUtils.FindRootVisual(this) as FrameworkElement;
                while (ribbon == null && fe != null && fe.GetType() == VisualUtils.RootPopupType)
                {
                    Popup popup = fe.Parent as Popup;
                    fe = popup.TemplatedParent as FrameworkElement;
                    if (fe != null)
                    {
                        if (fe is Ribbon)
                            ribbon = fe as Ribbon;
                        else
                            ribbon = VisualUtils.FindAncestor(fe, typeof(Ribbon)) as Ribbon;
                        fe = VisualUtils.FindRootVisual(fe) as FrameworkElement;
                    }
                }

                if (ribbon != null)
                {
                    if (!(this.TemplatedParent is QuickAccessToolBar))
                    {
                        RibbonContextMenu.CreateContextMenu(this);
                    }
                }

                e.Handled = true;
            }
        }

         #if !SyncfusionFramework3_5
        protected override void OnTouchUp(TouchEventArgs e)
        {
            if (m_systemGesture == SystemGesture.RightTap)
            {
                base.OnTouchUp(e);

                if (!(this.TemplatedParent is RibbonBar) && e.Source == this)
                {
                    FrameworkElement realSource = RibbonContextMenu.GetRealSource(this);
                    if (realSource != null)
                    {
                        if (!realSource.IsEnabled)
                        {
                            ContextMenuService.SetShowOnDisabled(realSource, true);
                            RibbonContextMenu.CreateContextMenu(realSource);
                            return;
                        }
                    }

                    this.IsDropDownOpen = false;

                    Ribbon ribbon = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
                    FrameworkElement fe = VisualUtils.FindRootVisual(this) as FrameworkElement;
                    while (ribbon == null && fe != null && fe.GetType() == VisualUtils.RootPopupType)
                    {
                        Popup popup = fe.Parent as Popup;
                        fe = popup.TemplatedParent as FrameworkElement;
                        if (fe != null)
                        {
                            if (fe is Ribbon)
                                ribbon = fe as Ribbon;
                            else
                                ribbon = VisualUtils.FindAncestor(fe, typeof(Ribbon)) as Ribbon;
                            fe = VisualUtils.FindRootVisual(fe) as FrameworkElement;
                        }
                    }

                    if (ribbon != null && ribbon.EnableTouch)
                    {
                        if (!(this.TemplatedParent is QuickAccessToolBar))
                        {
                            RibbonContextMenu.CreateContextMenu(this);
                        }
                    }

                    e.Handled = true;
                }
            }
        }

    #endif

        protected override void OnStylusSystemGesture(StylusSystemGestureEventArgs e)
        {
            base.OnStylusSystemGesture(e);
            m_systemGesture = e.SystemGesture;
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
                if (m_stack.Count > 0 && (m_stack.Peek() is RibbonItemsControl || m_stack.Peek().GetType() == VisualUtils.RootPopupType))
                {
                    if (e.OriginalSource is ButtonBase &&
                        (((e.OriginalSource as ButtonBase).TemplatedParent is RibbonItemsControl) ||
                        (e.OriginalSource as ButtonBase).TemplatedParent is RepeatButton || 
                        (e.OriginalSource as ButtonBase).TemplatedParent is Syncfusion.Windows.Controls.Primitives.CalendarItem||
                        e.OriginalSource is Syncfusion.Windows.Controls.Primitives.CalendarDayButton||
                        e.OriginalSource is Syncfusion.Windows.Controls.Primitives.CalendarButton))
                    {
                        return;
                    }

                    UIElement p = !(e.OriginalSource is RepeatButton) ? e.OriginalSource as UIElement : e.Source as UIElement;

                    AutomationPeer peer = UIElementAutomationPeer.CreatePeerForElement(p);

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
            m_stack.Push(e.OriginalSource);

            base.OnLostMouseCapture(e);

            m_wasPressed = false;
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseEnter"/>�attached event is raised on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            if (Mouse.Captured == null || VisualUtils.FindRootVisual((Visual)this).GetType() == VisualUtils.RootPopupType)
            {
                m_skipDropDownOpen = false;
            }
        }

        /// <summary>
        /// Provides class handling for the MouseLeftButtonUp routed event that occurs 
        /// when the left mouse button is released while the mouse pointer is over this control. 
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonUp"/>�routed event reaches Ban element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the left mouse button was released.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            m_skipDropDownOpen = false;
        }

        /// <summary>
        /// Invoked when the <see cref="E:System.Windows.UIElement.KeyDown"/> event is received.
        /// </summary>
        /// <param name="e">Information about the event.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape && this.IsDropDownOpen)
            {
                IsDropDownOpen = false;
                e.Handled = true;

                Visual v = VisualUtils.FindRootVisual(this) as Visual;
                if (v.GetType() == VisualUtils.RootPopupType)
                {
                    Popup popup = (v as FrameworkElement).Parent as Popup;
                    FrameworkElement templatedParent = popup.TemplatedParent as FrameworkElement;
                    if (templatedParent != null)
                    {
                        Keyboard.Focus(templatedParent);
                    }
                }
            }

            base.OnKeyDown(e);
        }
        #endregion
    }
}
