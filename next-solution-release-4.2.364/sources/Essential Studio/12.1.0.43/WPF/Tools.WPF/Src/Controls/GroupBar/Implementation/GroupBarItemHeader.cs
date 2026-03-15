// <copyright file="GroupBarItemHeader.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

#region file using
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Syncfusion.Windows.Shared;
#endregion

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Helper class that provides possibilities to place and layout
    /// the text and the image to display in the header of the FxGroupBarItem.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif

    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
    Type = typeof(GroupBarItemHeader), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(GroupBarItemHeader), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(GroupBarItemHeader), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
    Type = typeof(GroupBarItemHeader), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(GroupBarItemHeader), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(GroupBarItemHeader), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
     Type = typeof(GroupBarItemHeader), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyBlue,
     Type = typeof(GroupBarItemHeader), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/ShinyBlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyRed,
     Type = typeof(GroupBarItemHeader), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/ShinyRedStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.SyncOrange,
     Type = typeof(GroupBarItemHeader), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/SyncOrangeStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(GroupBarItemHeader), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(GroupBarItemHeader), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/DefaultStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
     Type = typeof(GroupBarItemHeader), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
     Type = typeof(GroupBarItemHeader), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent ,
 Type = typeof(GroupBarItemHeader), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/TransparentStyle.xaml")]
   
    public class GroupBarItemHeader : TextImageControl
    {
        #region Private members
        /// <summary>
        /// Text editor for text content of the GroupBarItemHeader.
        /// </summary>
        private TextBox m_textEditor = null;
        private Border m_ImageHost = null;
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies <see cref="IsInEditMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsInEditModeProperty =
            DependencyProperty.Register("IsInEditMode", typeof(bool), typeof(GroupBarItemHeader), new UIPropertyMetadata(false, OnIsInEditModeChanged));
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this instance is in edit mode.
        /// </summary>
        /// <value>
        /// true if this instance is in edit mode; otherwise, false
        /// </value>
        public bool IsInEditMode
        {
            get
            {
                return (bool)GetValue(IsInEditModeProperty);
            }

            set
            {
                SetValue(IsInEditModeProperty, value);
            }
        }

        /// <summary>
        /// Gets the text editor.
        /// </summary>
        /// <value>The text editor.</value>
        private TextBox TextEditor
        {
            get
            {
                return m_textEditor;
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupBarItemHeader"/> class.
        /// </summary>
        public GroupBarItemHeader()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupBarItemHeader"/> class.
        /// </summary>
        /// <param name="text">The text group bar item header.</param>
        /// <param name="imageSource">The image source.</param>
        public GroupBarItemHeader(string text, ImageSource imageSource)
        {
            Text = text;
            ImageSource = imageSource;
           
        }

        /// <summary>
        /// Initializes static members of the <see cref="GroupBarItemHeader"/> class.
        /// </summary>
        static GroupBarItemHeader()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GroupBarItemHeader), new FrameworkPropertyMetadata(typeof(GroupBarItemHeader)));
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Handles to IsVisibleChanged of the text editor for GroupBarItemHeader.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void TextEditor_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.TextEditor != null && this.TextEditor.IsVisible)
            {
                this.TextEditor.Focus();
                this.TextEditor.SelectAll();
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }
        
        /// <summary>
        /// Called when <see cref="IsInEditMode"/> is changed.
        /// </summary>
        /// <param name="d">The GroupBarItemHeader object.</param>
        /// <param name="e">The instance containing the event data.</param>
        private static void OnIsInEditModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBarItemHeader header = d as GroupBarItemHeader;

            if (header != null)
            {
                GroupBarItem item = header.Parent as GroupBarItem;

                if (item != null)
                {
                    if (header.IsInEditMode)
                    {
                        item.OnBeforeEdit();
                    }
                    else
                    {
                        item.OnAfterEdit();
                    }
                }

                header.UpdateLayout();
            }
        }

     
        /// <summary>
        /// Called when text editor lost keyboard focus.
        /// </summary>
        /// <param name="sender">GroupBarItemHeader object.</param>
        /// <param name="e">The instance containing the event data.</param>
        private static void TextEditor_LostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null && textBox.TemplatedParent != null && textBox.TemplatedParent is GroupBarItemHeader)
            {
                GroupBarItemHeader header = textBox.TemplatedParent as GroupBarItemHeader;

                if (header.Parent != null && header.Parent is GroupBarItem)
                {
                    GroupBarItem item = header.Parent as GroupBarItem;
                    item.IsInEditMode = false;
                }
            }
        }
        
        /// <summary>
        /// Builds the current template's visual tree if necessary.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_textEditor = Template.FindName("TextEditor", this) as TextBox;
            m_ImageHost = Template.FindName("ImageHost", this) as Border;
            if (m_ImageHost != null && this.ImageSource==null)
            {
                m_ImageHost.Visibility = Visibility.Collapsed;

            }
            if (m_textEditor == null)
            {
                throw new ApplicationException("TextEditor not found.");
            }

            TextEditor.IsVisibleChanged += new DependencyPropertyChangedEventHandler(TextEditor_IsVisibleChanged);
            TextEditor.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(TextEditor_LostKeyboardFocus);
        }
        
        /// <summary>
        /// Invoked whenever an unhandled <see cref="System.Windows.UIElement.GotFocus"/> event reaches this element in its route. 
        /// </summary>
        /// <param name="e">The <see cref="RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            GroupBarItem item = Parent as GroupBarItem;

            if (item != null && item.LogicalParent != null)
            {
                //item.LogicalParent.SelectedObject = item;
                item.LogicalParent.SelectedTab = item;

                if (item.LogicalParent.VisualMode == VisualMode.Default
                    && !item.IsDragging && !item.IsExpanded)
                {
                    item.SetExpanded();
                    item.UpdateLayout();
                }
            }
        }
        #endregion
    }
}
