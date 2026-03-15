#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;

namespace Syncfusion.Windows.Edit
{
#if SyncfusionFramework4_0

    using System.ComponentModel;

    [DesignTimeVisible(false)]
#endif

    /// <summary>
    ///
    /// </summary>
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
    Type = typeof(FindReplaceControl), XamlResource = "/Syncfusion.Edit.Wpf;component/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(FindReplaceControl), XamlResource = "/Syncfusion.Edit.Wpf;component/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(FindReplaceControl), XamlResource = "/Syncfusion.Edit.Wpf;component/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(FindReplaceControl), XamlResource = "/Syncfusion.Edit.Wpf;component/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
    Type = typeof(FindReplaceControl), XamlResource = "/Syncfusion.Edit.Wpf;component/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(FindReplaceControl), XamlResource = "/Syncfusion.Edit.Wpf;component/Themes/Generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
    Type = typeof(FindReplaceControl), XamlResource = "/Syncfusion.Edit.Wpf;component/Themes/MetroStyle.xaml")]
    public class FindReplaceControl : Control
    {
        /// <summary>
        ///
        /// </summary>
        static FindReplaceControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FindReplaceControl), new FrameworkPropertyMetadata(typeof(FindReplaceControl)));
        }

        /// <summary>
        ///
        /// </summary>
        public FindReplaceControl()
        {
            this.Unloaded += new RoutedEventHandler(FindReplaceControl_Unloaded);
            this.Loaded += new RoutedEventHandler(FindReplaceControl_Loaded);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindReplaceControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (findtext != null)
            {
                findtext.LoadHistory();
            }

            if (replacewith != null)
            {
                replacewith.LoadHistory();
            }

            this.Dispatcher.BeginInvoke((Action)delegate
            {
                this.SetFocusToFindWhat();
            }, System.Windows.Threading.DispatcherPriority.Loaded);
        }

        private void FindReplaceControl_Unloaded(object sender, RoutedEventArgs e)
        {
            FindOptions options = this.DataContext as FindOptions;
            if (options != null)
            {
                if (options.ParentWindow != null && options.ParentWindow.PlacementTarget != null && options.ParentWindow.PlacementTarget is EditControl)
                {
                    (options.ParentWindow.PlacementTarget as EditControl).Focus();
                }
            }

            if (findtext != null)
            {
                findtext.SaveHistory();
                findtext.LostFocus -= new RoutedEventHandler(findtext_LostFocus);
            }

            if (replacewith != null && replacewith.Text != "")
            {
                replacewith.SaveHistory();
                replaceButton.Click -= new RoutedEventHandler(replaceButton_Click);
            }

            if (findMenu != null)
            {
                findMenu.Click -= new RoutedEventHandler(findMenu_Click);
            }

            if (findSymbolMenu != null)
            {
                findSymbolMenu.Click -= new RoutedEventHandler(findSymbolMenu_Click);
            }

            if (replaceButton != null)
            {
                replaceButton.Click -= new RoutedEventHandler(replaceButton_Click);
                replaceButton.IsSelectedChanged -= new PropertyChangedCallback(replaceButton_IsSelectedChanged);
            }

            if (findButton != null)
            {
                findButton.MouseLeftButtonDown -= new MouseButtonEventHandler(findButton_MouseLeftButtonDown);
                findButton.IsPressedChanged -= new PropertyChangedCallback(findButton_IsPressedChanged);
            }

            if (button != null)
            {
                button.IsSelectedChanged -= new PropertyChangedCallback(button_IsSelectedChanged);
            }
        }

        private MenuItem findMenu = null;
        private MenuItem findSymbolMenu = null;
        private RibbonButton replaceButton = null;
        private DropDownButton findButton = null;
        private AutoComplete findtext = null;
        private AutoComplete replacewith = null;
        private RibbonButton button = null;

        /// <summary>
        ///
        /// </summary>
        public override void OnApplyTemplate()
        {
            findMenu = this.GetTemplateChild("PART_FindMenu") as MenuItem;
            findSymbolMenu = this.GetTemplateChild("PART_FindSymbolMenu") as MenuItem;
            replaceButton = this.GetTemplateChild("PART_ReplaceButton") as RibbonButton;
            findButton = this.GetTemplateChild("PART_FindButton") as DropDownButton;
            findtext = this.GetTemplateChild("PART_FindWhat") as AutoComplete;
            replacewith = this.GetTemplateChild("PART_ReplaceWith") as AutoComplete;

            if (findMenu != null)
            {
                findMenu.Click += new RoutedEventHandler(findMenu_Click);
            }

            if (findSymbolMenu != null)
            {
                findSymbolMenu.Click += new RoutedEventHandler(findSymbolMenu_Click);
            }

            if (replaceButton != null)
            {
                replaceButton.Click += new RoutedEventHandler(replaceButton_Click);
                replaceButton.IsSelectedChanged += new PropertyChangedCallback(replaceButton_IsSelectedChanged);
            }

            if (findButton != null)
            {
                findButton.MouseLeftButtonDown += new MouseButtonEventHandler(findButton_MouseLeftButtonDown);
                findButton.IsPressedChanged += new PropertyChangedCallback(findButton_IsPressedChanged);
            }

            button = this.GetTemplateChild("PART_RibbonFind") as RibbonButton;
            if (button != null)
            {
                button.IsSelectedChanged += new PropertyChangedCallback(button_IsSelectedChanged);
            }

            if (findtext != null)
            {
                findtext.LostFocus += new RoutedEventHandler(findtext_LostFocus);
            }

            if (replacewith != null)
            {
                replacewith.LostFocus += new RoutedEventHandler(findtext_LostFocus);
            }
        }

        private void replaceButton_IsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (replaceButton.IsSelected)
            {
                if (this.DataContext != null && this.DataContext is FindOptions)
                {
                    FindOptions options = this.DataContext as FindOptions;
                    options.IsReplaceTabActive = true;
                    options.IsFindTabActive = false;
                    options.IsFindSymbolTabActive = false;
                    SetFocusToFindWhat();
                }
            }
        }

        private void button_IsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonButton button = d as RibbonButton;
            if (button.IsSelected)
            {
                UpdateFindTab();
            }
        }

        private void UpdateFindTab()
        {
            TextBlock text = this.GetTemplateChild("PART_FindButtonText") as TextBlock;
            if (text != null && this.DataContext != null && this.DataContext is FindOptions)
            {
                FindOptions options = this.DataContext as FindOptions;
                if (text.Text.ToLower() == "quick find")
                {
                    options.IsReplaceTabActive = false;
                    options.IsFindTabActive = true;
                    options.IsFindSymbolTabActive = false;
                    findButton.ToolTip = new ToolTip() { Content = "Switch to Quick Find" };
                }
                else
                {
                    options.IsReplaceTabActive = false;
                    options.IsFindTabActive = false;
                    options.IsFindSymbolTabActive = true;
                    findButton.ToolTip = new ToolTip() { Content = "Switch to Find Symbol" };
                }
            }
            findButton.Focus();
            SetFocusToFindWhat();
        }

        private void findButton_IsPressedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (findButton.IsPressed)
            {
                FindOptions options = this.DataContext as FindOptions;
                UpdateFindTab();
                options.StatusMessage = "Ready";
            }
        }

        private void findtext_LostFocus(object sender, RoutedEventArgs e)
        {
            AutoComplete completeControl = sender as AutoComplete;
            if (completeControl != null && completeControl.Text.Trim() != string.Empty)
            {
                completeControl.AddHistory(completeControl.Text);
            }
        }

        private void findButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void replaceButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null && this.DataContext is FindOptions)
            {
                FindOptions options = this.DataContext as FindOptions;
                options.IsReplaceTabActive = true;
                options.IsFindTabActive = false;
                options.IsFindSymbolTabActive = false;
                SetFocusToFindWhat();
                options.StatusMessage = "Ready";
            }
        }

        private void findSymbolMenu_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null && this.DataContext is FindOptions)
            {
                TextBlock text = this.GetTemplateChild("PART_FindButtonText") as TextBlock;
                text.Text = "Find Symbol";
                FindOptions options = this.DataContext as FindOptions;
                options.IsReplaceTabActive = false;
                options.IsFindTabActive = false;
                options.IsFindSymbolTabActive = true;
                findButton.ToolTip = new ToolTip() { Content = "Switch to Find Symbol" };
                options.StatusMessage = "Ready";
                SetFocusToFindWhat();
            }
        }

        private void findMenu_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null && this.DataContext is FindOptions)
            {
                TextBlock text = this.GetTemplateChild("PART_FindButtonText") as TextBlock;
                text.Text = "Quick Find";
                FindOptions options = this.DataContext as FindOptions;
                options.IsReplaceTabActive = false;
                options.IsFindTabActive = true;
                options.IsFindSymbolTabActive = false;
                findButton.ToolTip = new ToolTip() { Content = "Switch to Quick Find" };
                SetFocusToFindWhat();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Window parentWindow = VisualUtils.FindAncestor(this, typeof(Window)) as Window;
                parentWindow.Close();
            }
        }

        internal void SetFocusToFindWhat()
        {
            TextBox box = VisualUtils.FindDescendant(findtext, typeof(TextBox)) as TextBox;
            if (box != null)
            {
                box.Focus();
            }
        }
    }
}