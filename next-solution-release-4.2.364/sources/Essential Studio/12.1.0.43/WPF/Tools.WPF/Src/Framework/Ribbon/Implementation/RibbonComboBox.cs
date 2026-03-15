// <copyright file="RibbonComboBox.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents a Ribbon selection control with a drop-down list that can be shown or hidden by 
    /// clicking the arrow on the control.  
    /// </summary>
    /// <remarks>
    /// The RibbonComboBox allows the user to select an item from a drop-down list or optionally to enter 
    /// new text in the text box of the control. 
    /// </remarks>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class RibbonComboBox : ComboBox, IRibbonControl
    {
        ScrollViewer PART_ScrollViewer;
        Popup Popup;

        #region Constructors

        /// <summary>
        /// Initializes static members of the RibbonComboBox class.
        /// </summary>
        static RibbonComboBox()
        {
            EnvironmentTest.ValidateLicense(typeof(RibbonComboBox));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonComboBox), new FrameworkPropertyMetadata(typeof(RibbonComboBox)));
        }

        public RibbonComboBox()
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                if (Application.Current != null && !BindingUtils.GetEnableBindingErrors(Application.Current.MainWindow))
                    System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
            }

        }
        #endregion

        #region Dependency properties
        /// <summary>
        /// Defines the text that labels RibbonComboBox. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(RibbonComboBox), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnLabelChanged)));

        /// <summary>
        ///  Defines the icon that appears in RibbonComboBox. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty SmallIconProperty = DependencyProperty.Register("SmallIcon", typeof(ImageSource), typeof(RibbonComboBox), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSmallIconChanged)));

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the text that labels RibbonComboBox.
        /// </summary>
        /// <value>
        /// Type: <see cref="String"/>
        /// Text that labels the RibbonComboBox. The default is empty string.
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
        /// Gets or sets the icon that appears in RibbonComboBox.
        /// </summary>
        /// <remarks>
        /// Many controls have more than just text in the element. Often there is an icon. 
        /// </remarks>
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

        #endregion

        #region Events

        /// <summary>
        /// Event that is raised when Label property is changed.
        /// </summary>
        public event PropertyChangedCallback LabelChanged;

        /// <summary>
        /// Event that is raised when SmallIcon property is changed.
        /// </summary>
        public event PropertyChangedCallback SmallIconChanged;

        #endregion       

        #region Implementation
        /// <summary>
        /// Calls OnLabelChanged method of the instance, notifies of the
        /// dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value and new value.</param>
        private static void OnLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonComboBox instance = (RibbonComboBox)d;
            instance.OnLabelChanged(e);
        }

        /// <summary>
        /// Calls OnIconChanged method of the instance, notifies of the
        /// dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnSmallIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonComboBox instance = (RibbonComboBox)d;
            instance.OnSmallIconChanged(e);
        }
        
        /// <summary>
        /// Updates property value cache and raises LabelChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value and new value.</param>
        protected virtual void OnLabelChanged(DependencyPropertyChangedEventArgs e)
        {
            if (LabelChanged != null)
            {
                LabelChanged(this, e);
            }
        }
        
        /// <summary>
        /// Updates property value cache and raises SmallIconChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value and new value.</param>
        protected virtual void OnSmallIconChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SmallIconChanged != null)
            {
                SmallIconChanged(this, e);
            }
        }

        #endregion

        #region Overrides

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
                FrameworkElement felement = VisualUtils.FindRootVisual(this) as FrameworkElement;
                while (ribbon == null && felement != null && felement.GetType() == VisualUtils.RootPopupType)
                {
                    Popup popup = felement.Parent as Popup;
                    felement = popup.TemplatedParent as FrameworkElement;
                    if (felement != null)
                    {
                        if (felement is Ribbon)
                            ribbon = felement as Ribbon;
                        else
                            ribbon = VisualUtils.FindAncestor(felement, typeof(Ribbon)) as Ribbon;
                        felement = VisualUtils.FindRootVisual(felement) as FrameworkElement;
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

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container. 
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>Returns true if the item is (or is eligible to be) its own container; 
        /// otherwise, false. </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is RibbonComboBoxItem;
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>The element that is used to display the given item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new RibbonComboBoxItem();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();            
            Popup = GetTemplateChild("Popup") as Popup;
            if (Popup != null)
            {
                Popup.Opened += new EventHandler(Popup_Opened);
            }   
        }

        void Popup_Opened(object sender, EventArgs e)
        {
#if !SyncfusionFramework3_5

            PART_ScrollViewer = GetTemplateChild("PART_ScrollViewer") as ScrollViewer;

            if (PART_ScrollViewer != null)
            {
                if (SkinStorage.GetEnableTouch(this))
                    PART_ScrollViewer.PanningMode = PanningMode.VerticalOnly;
                else
                    PART_ScrollViewer.PanningMode = PanningMode.None;
            }

#endif
        }

        #endregion
    }
}
