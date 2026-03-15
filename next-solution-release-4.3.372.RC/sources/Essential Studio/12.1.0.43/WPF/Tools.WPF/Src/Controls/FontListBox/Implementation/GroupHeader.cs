// <copyright file="GroupHeader.cs" company="Syncfusion">
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
    /// Represents a single group header in a <see cref="Syncfusion.Windows.Tools.Controls.FontListBox"/> control.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif

    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
Type = typeof(GroupHeader), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/FontListBox/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(GroupHeader), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/FontListBox/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(GroupHeader), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/FontListBox/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
    Type = typeof(GroupHeader), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/FontListBox/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(GroupHeader), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/FontListBox/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(GroupHeader), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/FontListBox/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
     Type = typeof(GroupHeader), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/FontListBox/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyBlue,
     Type = typeof(GroupHeader), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/FontListBox/Themes/ShinyBlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyRed,
     Type = typeof(GroupHeader), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/FontListBox/Themes/ShinyRedStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.SyncOrange,
     Type = typeof(GroupHeader), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/FontListBox/Themes/SyncOrangeStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(GroupHeader), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/FontListBox/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(GroupHeader), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/FontListBox/Themes/Generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
   Type = typeof(GroupHeader), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/FontListBox/Themes/MetroStyle.xaml")]
    public class GroupHeader : Control
    {
        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="GroupHeader"/> class.
        /// </summary>
        static GroupHeader()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GroupHeader), new FrameworkPropertyMetadata(typeof(GroupHeader)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupHeader"/> class.
        /// </summary>
        public GroupHeader()
        {
        }

        #endregion Initialization

        #region Properties

        /// <summary>
        /// Gets or sets the text for group header. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="string"/>
        /// </value>
        /// <seealso cref="string"/>
        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }

            set
            {
                SetValue(TextProperty, value);
            }
        }

        #endregion Properties

        #region Events

        /// <summary>
        /// Event that is raised when Text property is changed.
        /// </summary>
        public event PropertyChangedCallback TextChanged;

        #endregion Events

        #region Dependency properties

        /// <summary>
        /// Identifies Text dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(GroupHeader), new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback(OnTextChanged)));

        #endregion Dependency properties

        #region	Implementation

        /// <summary>
        /// Calls OnTextChanged method of the instance, notifies of the
        /// dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupHeader instance = (GroupHeader)d;
            instance.OnTextChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises TextChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnTextChanged(DependencyPropertyChangedEventArgs e)
        {
            if (TextChanged != null)
            {
                TextChanged(this, e);
            }
        }

        #endregion
    }
}