// <copyright file="CustomMenuItem.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System.ComponentModel;
using System.Runtime.Remoting.Messaging;
using System.Windows;
using System.Windows.Controls;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents a custom menu item.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
   Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
    Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.SyncOrange,
    Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/SyncOrangeStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyRed,
    Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/ShinyRedStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyBlue,
    Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/ShinyBlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/vista.aero.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
    Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
    Type = typeof(DockingManager), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/DockingManager/Themes/TransparentStyle.xaml")]  
    public class CustomMenuItem : MenuItem
    {
        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="CustomMenuItem"/> class.
        /// </summary>
        static CustomMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomMenuItem), new FrameworkPropertyMetadata(typeof(CustomMenuItem)));
        }
        #endregion

        #region Event
        /// <summary>
        /// Event that is raised when <see cref="HeaderInIconArea"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback HeaderInIconAreaChanged;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the value of the <see cref="IconTemplateProperty"/> dependency property.
        /// </summary>
        /// <remarks>
        /// This property is used to get / set data template for the icon.
        /// </remarks>
        [Bindable(true)]
        public DataTemplate IconTemplate
        {
            get
            {
                return (DataTemplate)GetValue(IconTemplateProperty);
            }

            set
            {
                SetValue(IconTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="HeaderInIconArea"/> dependency property.
        /// </summary>
        /// <remarks>
        /// This property determines what will be shown in the left area of menu item - icon or header.
        /// If it set to true - header will be displayed instead of icon, otherwise - icon will be displayed
        /// (but only for checked item).
        /// </remarks>
        public bool HeaderInIconArea
        {
            get
            {
                return (bool)GetValue(HeaderInIconAreaProperty);
            }

            set
            {
                SetValue(HeaderInIconAreaProperty, value);
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Called when the Header property of a <see cref="CustomMenuItem"/> changes.
        /// </summary>
        /// <param name="oldHeader">The old value of the <see cref="Header"/> property.</param>
        /// <param name="newHeader">The new value of the <see cref="Header"/> property.</param>
        protected override void OnHeaderChanged(object oldHeader, object newHeader)
        {
            base.OnHeaderChanged(oldHeader, newHeader);
            //DataContext = Header;
        }

        /// <summary>
        /// Updates property value cache and raises HeaderInIconAreaChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private void OnHeaderInIconAreaChanged(DependencyPropertyChangedEventArgs e)
        {
            if (HeaderInIconAreaChanged != null)
            {
                HeaderInIconAreaChanged(this, e);
            }
        }

        
        /// <summary>
        /// Calls OnHeaderInIconAreaChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnHeaderInIconAreaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CustomMenuItem instance = (CustomMenuItem)d;
            instance.OnHeaderInIconAreaChanged(e);
        }
        #endregion

        #region Dependency Propeties
        /// <summary>
        /// Identifies the <see cref="IconTemplateProperty"/>�dependency property.
        /// </summary>
        public static DependencyProperty IconTemplateProperty =
            DependencyProperty.Register("IconTemplate", typeof(DataTemplate), typeof(CustomMenuItem));
        
        /// <summary>
        /// Identifies the <see cref="HeaderInIconAreaProperty"/>�dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderInIconAreaProperty =
            DependencyProperty.Register("HeaderInIconArea", typeof(bool), typeof(CustomMenuItem), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange, new PropertyChangedCallback(OnHeaderInIconAreaChanged)));
        #endregion
    }
}
