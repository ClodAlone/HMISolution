// <copyright file="RibbonCheckBox.cs" company="Syncfusion">
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
    /// Represents a RibbonCheckBox control.
    /// </summary>
    /// <remarks>
    /// RibbonCheckBox controls inherit from CheckBox and can have three states: checked, unchecked, 
    /// and indeterminate. 
    /// You can display CheckBox controls in a group so that users can choose from a list of options. 
    /// You can also offer users a combination of options.
    /// </remarks>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class RibbonCheckBox : CheckBox, IRibbonControl
    {
        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="RibbonCheckBox"/> class.
        /// </summary>
        static RibbonCheckBox()
        {
            EnvironmentTest.ValidateLicense(typeof(RibbonCheckBox));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonCheckBox), new FrameworkPropertyMetadata(typeof(RibbonCheckBox)));
        }

        public RibbonCheckBox()
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                if (Application.Current != null && !BindingUtils.GetEnableBindingErrors(Application.Current.MainWindow))
                    System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
            }

        }
        #endregion

        ////#region IRibbonControl Members

        /////// <summary>
        /////// Gets the value of the IRibbonControl Label property.
        /////// </summary>
        /////// <value></value>
        ////string IRibbonControl.Label
        ////{
        ////    get { return Content.ToString(); }
        ////}

        /////// <summary>
        /////// Gets the value of the IRibbonControl Image property.
        /////// </summary>
        /////// <value></value>
        ////ImageSource IRibbonControl.SmallIcon
        ////{
        ////    get { return null; }
        ////}
        ////#endregion

        #region Dependency properties
        /// <summary>
        /// Defines the text that labels RibbonCheckBox. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(RibbonCheckBox), new FrameworkPropertyMetadata(null));

        /// <summary>
        ///  Defines the icon that appears in RibbonCheckBox. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty SmallIconProperty = DependencyProperty.Register("SmallIcon", typeof(ImageSource), typeof(RibbonCheckBox), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSmallIconChanged)));

        #endregion

        #region Properties

        /// <summary>
        /// Gets the value of the Label property.
        /// </summary>
        /// <value>Get the content.</value>
        public string Label
        {
            get { return Content.ToString(); }          
        }

        /// <summary>
        /// Gets or sets the icon that appears in RibbonCheckBox.
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
        /// Event that is raised when SmallIcon property is changed.
        /// </summary>
        public event PropertyChangedCallback SmallIconChanged;

        #endregion

        #region Implementation      

        /// <summary>
        /// Calls OnIconChanged method of the instance, notifies of the
        /// dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnSmallIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonCheckBox instance = (RibbonCheckBox)d;
            instance.OnSmallIconChanged(e);
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
                RibbonContextMenu.CreateContextMenu(this);
            }

            e.Handled = true;
        }

        protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer()
        {
            return new RibbonCheckBoxAutomationPeer(this);
        }

        #endregion
    }
}
