// <copyright file="FontListBoxInternalItem.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FontListBoxInternalItem"/> class.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif

    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
  Type = typeof(FontListBoxInternalItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/FontListBox/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(FontListBoxInternalItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/FontListBox/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(FontListBoxInternalItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/FontListBox/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
        Type = typeof(FontListBoxInternalItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/FontListBox/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(FontListBoxInternalItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/FontListBox/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(FontListBoxInternalItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/FontListBox/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
     Type = typeof(FontListBoxInternalItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/FontListBox/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyBlue,
     Type = typeof(FontListBoxInternalItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/FontListBox/Themes/ShinyBlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyRed,
     Type = typeof(FontListBoxInternalItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/FontListBox/Themes/ShinyRedStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.SyncOrange,
     Type = typeof(FontListBoxInternalItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/FontListBox/Themes/SyncOrangeStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(FontListBoxInternalItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/FontListBox/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(FontListBoxInternalItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/FontListBox/Themes/Generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
   Type = typeof(FontListBoxInternalItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/FontListBox/Themes/MetroStyle.xaml")]
    public class FontListBoxInternalItem : ContentControl
    {
        #region Constants

        /// <summary>
        /// Stores default name of the skin.
        /// </summary>
        private const string M_cFontListBoxDefaultVisualStyle = "Default";

        #endregion Constants

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="FontListBoxInternalItem"/> class.
        /// </summary>
        static FontListBoxInternalItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FontListBoxInternalItem), new FrameworkPropertyMetadata(typeof(FontListBoxInternalItem)));
            FocusableProperty.OverrideMetadata(typeof(FontListBoxInternalItem), new FrameworkPropertyMetadata(false));
        }

        /// <summary>
        /// Initializes a new instance of the FontListBoxInternalItem class.
        /// </summary>
        public FontListBoxInternalItem()
        {
        }

        #endregion Initialization

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether Item got the focus.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// </value>
        /// <seealso cref="bool"/>
        public bool HasFocus
        {
            get
            {
                return (bool)GetValue(HasFocusProperty);
            }

            set
            {
                SetValue(HasFocusProperty, value);
            }
        }

        #endregion Properties

        #region Events

        /// <summary>
        /// Event that is raised when HasFocus property is changed.
        /// </summary>
        public event PropertyChangedCallback HasFocusChanged;

        #endregion Events

        #region Dependency properties

        /// <summary>
        /// Identifies  HasFocus dependency property.
        /// </summary>
        public static readonly DependencyProperty HasFocusProperty =
            DependencyProperty.Register("HasFocus", typeof(bool), typeof(FontListBoxInternalItem), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnHasFocusChanged)));

        #endregion Dependency properties

        #region	Implementation

        /// <summary>
        /// Calls OnHasFocusChanged method of the instance, notifies of
        /// the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnHasFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FontListBoxInternalItem instance = (FontListBoxInternalItem)d;
            instance.OnHasFocusChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises HasFocusChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnHasFocusChanged(DependencyPropertyChangedEventArgs e)
        {
            if (HasFocusChanged != null)
            {
                HasFocusChanged(this, e);
            }
        }

        #endregion
    }
}