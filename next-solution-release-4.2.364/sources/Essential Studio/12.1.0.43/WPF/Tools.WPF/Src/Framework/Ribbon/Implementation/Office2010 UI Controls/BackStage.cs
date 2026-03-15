#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Specialized;
using System.Windows.Controls;
using System.Windows.Input;
using Syncfusion.Windows.Shared;
using System.Windows.Documents;
using System.Windows.Media;
using System.Collections;
using System.Windows.Automation.Provider;
using System.Windows.Automation.Peers;

namespace Syncfusion.Windows.Tools.Controls
{
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(Backstage), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
    Type = typeof(Backstage), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(Backstage), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
    Type = typeof(Backstage), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
    Type = typeof(Backstage), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/TransparentStyle.xaml")]

    [SkinType(SkinVisualStyle = Skin.Office2013,
   Type = typeof(Backstage), XamlResource = "/Syncfusion.Tools.WPF;component/Framework/Ribbon/Themes/Office2013Style.xaml")]

    /// <summary>
    /// Represents BackStage class.
    /// </summary>
    public class Backstage : Selector
    {
        #region Properties

        private AdornerLayer b_adornerLayer = null;

        private bool isInvoked = false;

        internal Ribbon parentRibbon;

        internal bool IsKeytipOpen = false;
        internal RibbonButton backbutton = null;

        SystemGesture msystemGesture;

        private static bool IsKeySelected = true;
        /// <summary>
        /// Stores a tree of KeuTips.
        /// </summary>
        private Dictionary<string, KeyTip> b_keyTips;

        /// <summary>
        /// Path to the current KeyTip, beginning from the top of the
        /// tree.
        /// </summary>
        private List<string> b_keyTipPath;

        /// <summary>
        /// Represents Complex string value.
        /// </summary>
        private string b_complexKeyTipString = string.Empty;

        internal bool isLostFocus = false;
        /// <summary>
        /// Stores pressed letters of the complex KeyTip.
        /// </summary>
        private string b_keyTipComplexPath = string.Empty;

        /// <summary>
        /// Current complex key tips level.
        /// </summary>
        private Dictionary<string, KeyTip> b_keyTipComplexLevel;

        /// <summary>
        /// Current key tips level(branch).
        /// </summary>
        private Dictionary<string, KeyTip> b_keyTipCurrentLevel;

        /// <summary>
        /// Boolean value that is used for checking the ribbon tab's of Key Tips.
        /// </summary>
        private bool b_ribbonflag = false;

        /// <summary>
        /// Represents dropdown button.
        /// </summary>
        private DropDownButton b_dropDownbutton = null;

        /// <summary>
        /// Represents the splitmenu button
        /// </summary>
        private SplitMenuButton b_splitMenuButton = null;

        /// <summary>
        /// Defines whether any of current level key tips starts from the
        /// specified letter.
        /// </summary>
        private bool b_complexLetterAdded;

        /// <summary>
        /// Specifies whether adorner is shown.
        /// </summary>
        internal static bool b_isAdornersShown = false;

        /// <summary>
        /// Current key tips level.
        /// </summary>
        private Dictionary<string, KeyTip> b_keyTipsCurrentLevel;

        /// <summary>
        /// Gets or sets the key tips current level.
        /// </summary>
        /// <value>
        /// The key tips current level.
        /// </value>
        internal Dictionary<string, KeyTip> KeyTipsCurrentLevel
        {
            get
            {
                return b_keyTipsCurrentLevel;
            }

            set
            {
                b_keyTipsCurrentLevel = value;
            }
        }

        /// <summary>
        /// Gets or sets the content of the selected tab.
        /// </summary>
        /// <value>The content of the selected tab.</value>
        public object SelectedTabContent
        {
            get { return (object)GetValue(SelectedTabContentProperty); }
            set { SetValue(SelectedTabContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedTabContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedTabContentProperty =
            DependencyProperty.Register("SelectedTabContent", typeof(object), typeof(Backstage), new UIPropertyMetadata(null));



        public DataTemplate TabHeaderTemplate
        {
            get { return (DataTemplate)GetValue(TabHeaderTemplateProperty); }
            set { SetValue(TabHeaderTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TabHeaderTemplateProperty =
            DependencyProperty.Register("TabHeaderTemplate", typeof(DataTemplate), typeof(Backstage), new PropertyMetadata(null));

        public Brush SelectedTabItemForeground
        {
            get { return (Brush)GetValue(SelectedTabItemForegroundProperty); }
            set { SetValue(SelectedTabItemForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedTabItemForegroundProperty =
            DependencyProperty.Register("SelectedTabItemForeground", typeof(Brush), typeof(Backstage), new PropertyMetadata(Brushes.Black ));


        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        static Backstage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Backstage), new FrameworkPropertyMetadata(typeof(Backstage)));
          KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(Backstage), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Backstage()
        {
            this.KeyDown += new KeyEventHandler(Backstage_KeyDown);
            this.KeyUp += new KeyEventHandler(Backstage_KeyUp);
            this.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(Backstage_LostKeyboardFocus);
            this.SizeChanged += new SizeChangedEventHandler(Backstage_SizeChanged);
        }

        void Backstage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            HideKeyTips();
        }

        bool isShowKeyTip = false;
        bool isShowKeyTipWithCollection = false;
        KeyTip selectedTabKeyTip = null;
        Dictionary<string, KeyTip> keyCollection = null;

        void Backstage_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            HideKeyTips();
            isLostFocus = true;
        }

        void Backstage_KeyUp(object sender, KeyEventArgs e)
        {
            bool isRibbonBarCollasped = false;
            if (e.Key == Key.Tab)
                return;
            if (e.Key == Key.System)
            {
                if (!isRibbonBarCollasped)
                {
                    if (isShowKeyTip)
                    {
                        ShowKeyTips();
                    }
                    else if (isShowKeyTipWithCollection && keyCollection != null)
                    {
                        ShowKeyTips(keyCollection.Values);
                    }

                    isShowKeyTip = false;
                    isShowKeyTipWithCollection = false;
                }

            }
            else if (!b_isAdornersShown && IsKeytipOpen && !b_ribbonflag && (e.Key != Key.Up && e.Key != Key.Down && e.Key != Key.Right && e.Key != Key.Left && e.Key != Key.Escape))
            {
                ShowKeyTips();                
                IsKeytipOpen = false;
            }
           
            base.OnKeyUp(e); 
        }

        void Backstage_KeyDown(object sender, KeyEventArgs e)
        {

            if (b_isAdornersShown)
            {
                e.Handled = true;
            }

            if (e.SystemKey >= Key.F1 && e.SystemKey <= Key.F12)
            {
                return;
            }

            if (e.Key >= Key.F1 && e.Key <= Key.F12)
            {
                return;
            }            

            if (e.Key == Key.System && b_isAdornersShown)
            {
                HideKeyTips();
                e.Handled = true;
                return;
            }

            if (e.Key == Key.System)
            {
                KeyTip keyTips;

                InitializeKeyTips();
                b_keyTipPath = new List<string>();
                char keyName = e.Key.ToString().ToUpper()[e.Key.ToString().Length - 1];
                b_keyTipCurrentLevel = FindKeyTipLevel(b_keyTipPath);

                if (!(e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt))
                {
                    b_complexKeyTipString += e.Key.ToString().ToUpper().Contains("D") && e.Key.ToString().Length == 2 ? e.Key.ToString().Substring(1, 1) : e.Key.ToString().ToUpper();
                }

                foreach (string key in b_keyTipCurrentLevel.Keys)
                {
                    if (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt)
                    {
                        HideKeyTips();
                        break;
                    }

                    keyTips = (KeyTip)b_keyTipCurrentLevel[key];
                    if (ModifierKeys.Alt == Keyboard.Modifiers)
                    {
                        selectedTabKeyTip = keyTips;
                        InvokeKeyTip(keyTips);
                        b_complexKeyTipString = string.Empty;
                    }
                }
            }


            if (e.Key == Key.F1 && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
            }

            KeyTip keyTip;
            if (ModifierKeys.Alt == Keyboard.Modifiers)
            {
                if (e.SystemKey == Key.Space)
                {
                    e.Handled = false;
                    return;
                }
            }

            if (!b_isAdornersShown && e.Key == Key.System)
            {
                isShowKeyTip = true;
            }

            if (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt || e.SystemKey == Key.F10)
            {
                b_keyTipPath.Clear();
                b_keyTipComplexPath = string.Empty;
                e.Handled = true;
                return;
            }
            var uie = e.OriginalSource as UIElement;

            if (e.Key == Key.Down)
            {
                e.Handled = true;
                if (Object.Equals(uie, this.Items[this.Items.Count - 1]))
                {
                    var ui = ((FrameworkElement)uie).Parent as UIElement;
                    if (ui != null)
                        ui.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
                }
                else
                    uie.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        
            if (e.Key == Key.Up)
            {
                e.Handled = true;
                if (Object.Equals(uie, this.Items[0]))
                {
                    var ui = ((FrameworkElement)uie).Parent as UIElement;
                    if (ui != null)
                        ui.MoveFocus(new TraversalRequest(FocusNavigationDirection.Last));
                }
                else
                    uie.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));

            }

            if (e.Key == Key.Tab && (uie is BackstageTabItem||uie is BackStageSeparator||uie is BackStageCommandButton||uie is BackStageButton))
            {
                e.Handled = true;
                uie.MoveFocus(new TraversalRequest(FocusNavigationDirection.Right));
            }
          

            if (e.Key == Key.Escape && b_isAdornersShown)
            {
                LevelUpKeyTips();
                this.parentRibbon.HideBackStage();
                this.parentRibbon.ShowKeyTips();
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Escape && !b_isAdornersShown)
            {
                
                this.parentRibbon.HideBackStage();
                
                e.Handled = true;
                return;
            }
            if ((b_isAdornersShown || e.Key == Key.System) && b_keyTipComplexPath != null && (e.Key != Key.Left && e.Key != Key.Right && e.Key != Key.Up && e.Key != Key.Down))
            {
                char keyName;

                if (e.Key != Key.System)
                {
                    keyName = e.Key.ToString().ToUpper()[e.Key.ToString().Length - 1];
                }
                else
                {
                    keyName = e.SystemKey.ToString().ToUpper()[e.SystemKey.ToString().Length - 1];
                }

                b_keyTipCurrentLevel = FindKeyTipLevel(b_keyTipPath);
                IsKeySelected = true;

                foreach (string key in b_keyTipCurrentLevel.Keys)
                {

                    keyTip = (KeyTip)b_keyTipCurrentLevel[key];
                    if (!keyTip.IsComplex && key.Length > 0 && key[0] == keyName && keyTip.IsActive)
                    {
                        if (keyTip.Text.Length == b_keyTipComplexPath.Length + 1)
                        {
                            if (keyTip.Host is BackStageCommandButton || keyTip.Host is BackstageTabItem)
                                selectedTabKeyTip = keyTip;
                            InvokeKeyTip(keyTip);

                            if (KeyTipsCurrentLevel != null && (keyTip.Host is BackstageTabItem || keyTip.Host is BackStageCommandButton) && KeyTipsCurrentLevel.Count == 0)
                            {
                                HideKeyTips(b_keyTipCurrentLevel.Values);
                                KeyTipsCurrentLevel.Clear();
                                b_keyTipPath.Clear();
                                b_keyTipComplexPath = string.Empty;
                                e.Handled = true;
                                return;
                            }
                            else if (KeyTipsCurrentLevel == null)
                            {
                                e.Handled = true;
                                return;
                            }
                        }
                        else
                        {
                            continue;
                        }
                        IsKeySelected = false;

                        HideKeyTips(b_keyTipCurrentLevel.Values);
                        if (UpdateKeyTips(ref keyTip))
                        {
                            b_keyTipPath.Add(keyTip.Text);
                            isShowKeyTipWithCollection = true;
                            keyCollection = keyTip.SubLevel;
                            isInvoked = true;
                            ShowKeyTips(keyTip.SubLevel.Values);
                            isInvoked = false;

                            KeyTipsCurrentLevel = keyTip.SubLevel;
                        }
                        else
                        {
                            //if (e.Key == Key.Escape)
                                InvokeKeyTip(keyTip);
                            if (KeyTipsCurrentLevel != null)
                                KeyTipsCurrentLevel.Clear();
                            b_keyTipPath.Clear();
                            b_keyTipComplexPath = string.Empty;
                            e.Handled = true;
                            return;
                        }
                    }

                    if (keyTip.IsComplex)
                    {
                        int index, complexLength;
                        if (b_keyTipComplexPath == string.Empty)
                        {
                            index = 0;
                            complexLength = 1;
                        }
                        else
                        {
                            if (!b_complexLetterAdded)
                            {
                                complexLength = b_keyTipComplexPath.Length + 1;
                                index = b_keyTipComplexPath.Length;
                            }
                            else
                            {
                                complexLength = b_keyTipComplexPath.Length;
                                index = b_keyTipComplexPath.Length - 1;
                            }
                        }

                        if (key.Length == complexLength)
                        {
                            if (key == b_keyTipComplexPath + keyName)
                            {
                                InvokeKeyTip(keyTip);
                                HideKeyTips(b_keyTipCurrentLevel.Values);
                                if (KeyTipsCurrentLevel != null)
                                    KeyTipsCurrentLevel.Clear();
                                b_keyTipComplexLevel.Clear();
                                b_keyTipComplexPath = string.Empty;
                                b_complexLetterAdded = false;
                                e.Handled = true;
                                return;
                            }
                        }
                        else
                        {
                            if (index < key.Length && key.Substring(0, b_keyTipComplexPath.Length) == b_keyTipComplexPath && key[index] == keyName)
                            {
                                b_keyTipComplexLevel.Add(keyTip.Text, keyTip);

                                if (!b_complexLetterAdded)
                                {
                                    string nextLetter = new string(keyTip.Text[b_keyTipComplexPath.Length], 1);
                                    b_keyTipComplexPath = b_keyTipComplexPath.Insert(b_keyTipComplexPath.Length, nextLetter);
                                    b_complexLetterAdded = true;
                                }
                            }
                        }
                    }
                }

                if (b_complexLetterAdded)
                {
                    ProcessComplexKeyTips();
                    IsKeySelected = false;
                }

                if (b_keyTipComplexPath == string.Empty && IsKeySelected)
                {
                    b_keyTipPath.Clear();

                    LevelUpKeyTips();
                    IsKeySelected = false;
                    e.Handled = true;
                    return;
                }
            }
            else
            {
                if (b_keyTipCurrentLevel != null)
                {
                    HideKeyTips(b_keyTipCurrentLevel.Values);
                    if (KeyTipsCurrentLevel != null)
                        KeyTipsCurrentLevel.Clear();
                }

                return;
            }
            e.Handled = true;

            base.OnKeyDown(e);

        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Invokes KeyTip host element.
        /// </summary>
        /// <param name="keyTip">KeyTip which will be invoked.</param>
        private void InvokeKeyTip(KeyTip keyTip)
        {
            if (keyTip != null)
            {
                GeneralInvoke(keyTip, keyTip.Host);
                b_ribbonflag = true;
            }
        }

        private DropDownButton dropdown;
        /// <summary>
        /// General invoke based on UIAutomation.
        /// </summary>
        /// <param name="keyTip">The key tip.</param>
        /// <param name="target">Target to be invoked.</param>
        /// <property name="flag" value="Finished"/>
        private void GeneralInvoke(KeyTip keyTip, UIElement target)
        {
            if (target is BackstageTabItem)
            {
                target.Focus();
                HideKeyTips(b_keyTipCurrentLevel.Values);
                if (UpdateKeyTips(ref keyTip))
                {
                    b_keyTipPath.Add(keyTip.Text);
                    isInvoked = true;
                    ShowKeyTips(keyTip.SubLevel.Values);
                    isInvoked = false;
                    KeyTipsCurrentLevel = keyTip.SubLevel;
                }

            }
            else if (target is BackStageCommandButton)
            {
                target.Focus();

                RibbonButtonAutomationPeer peer = new RibbonButtonAutomationPeer(target as RibbonButton);
                HideKeyTips(b_keyTipCurrentLevel.Values);
                if (UpdateKeyTips(ref keyTip))
                {
                    b_keyTipPath.Add(keyTip.Text);
                    isInvoked = true;
                    ShowKeyTips(keyTip.SubLevel.Values);
                    isInvoked = false;
                    KeyTipsCurrentLevel = keyTip.SubLevel;
                }
                IInvokeProvider provider = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                if (provider != null)
                    provider.Invoke();
            }
            else if (target is SplitMenuButton && KeyTip.GetSplitMenuKeyTip(target) == keyTip.Text)
            {

                b_splitMenuButton = target as SplitMenuButton;
                b_splitMenuButton.IsMenuOpen = true;
                b_splitMenuButton.IsMenuOpenChanged += new PropertyChangedCallback(m_splitMenuButton_IsMenuOpenChanged);
                HideKeyTips(b_keyTipCurrentLevel.Values);
                if (UpdateKeyTips(ref keyTip))
                {
                    b_keyTipPath.Add(keyTip.Text);
                    ShowKeyTips(keyTip.SubLevel.Values);
                    KeyTipsCurrentLevel = keyTip.SubLevel;
                }
            }
            else if (target is DropDownButton)
            {
                b_dropDownbutton = target as DropDownButton;
                b_dropDownbutton.IsDropDownOpen = true;
                b_dropDownbutton.IsDropDownOpenChanged += new PropertyChangedCallback(m_dropDownbutton_IsDropDownOpenChanged);
                HideKeyTips(b_keyTipCurrentLevel.Values);
                if (UpdateKeyTips(ref keyTip))
                {
                    b_keyTipPath.Add(keyTip.Text);
                    ShowKeyTips(keyTip.SubLevel.Values);
                    KeyTipsCurrentLevel = keyTip.SubLevel;
                }
            }

            else
            {
                if (keyTip.Host != null)
                    dropdown = (DropDownButton)VisualUtils.FindDescendant(keyTip.Host, typeof(DropDownButton));
                AutomationPeer peer = UIElementAutomationPeer.CreatePeerForElement(target);
                if (peer != null && peer.IsControlElement())
                {
                    if (peer is IInvokeProvider)
                    {
                        if (peer.GetClassName() == "RibbonButton")
                        {
                            RibbonButton ribbonButton = target as RibbonButton;
                            if (ribbonButton != null)
                            {
                                if (ribbonButton.IsToggle && ribbonButton.Command == null)
                                {
                                    ribbonButton.IsSelected = ribbonButton.IsSelected ? false : true;
                                }
                                if (ribbonButton.Command != null)
                                    if ((ribbonButton.Command is RoutedCommand && ((System.Windows.Input.RoutedCommand)(ribbonButton.Command)).Name.Contains("Toggle")) || ribbonButton.Command is DelegateCommand<object>)
                                    {
                                        ribbonButton.Command.Execute(ribbonButton.CommandParameter);
                                    }
                            }
                        }
                        IInvokeProvider provider = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                        provider.Invoke();
                        if (dropdown != null && dropdown.IsDropDownOpen)
                            dropdown.IsDropDownOpen = false;
                    }
                    if (peer is IToggleProvider)
                    {
                        IToggleProvider provider = peer.GetPattern(PatternInterface.Toggle) as IToggleProvider;
                        if (provider != null)
                        {
                            //provider.Toggle();
                        }
                        if (peer.GetClassName() == "CheckBox")
                        {
                            RibbonCheckBox ribbonCheckBox = target as RibbonCheckBox;
                            //if ((bool)ribbonCheckBox.IsChecked)
                            //{
                            //    ribbonCheckBox.IsChecked = false;
                            //}
                            //else
                            //    ribbonCheckBox.IsChecked = true;
                            RoutedEventArgs newEventArgs = new RoutedEventArgs(ButtonBase.ClickEvent);
                            ribbonCheckBox.RaiseEvent(newEventArgs);
                            if (ribbonCheckBox.Command != null)
                            {
                                ribbonCheckBox.Command.Execute(ribbonCheckBox.CommandParameter);
                            }
                        }
                    }

                    if (peer.GetClassName() == "TextBox")
                    {
                        TextBox txtBox = target as TextBox;
                        if (txtBox != null)
                        {
                            txtBox.Focus();
                            if (txtBox.Text.Length > 0)
                            {
                                txtBox.SelectAll();
                            }
                        }
                    }

                    if (peer is ISelectionItemProvider)
                    {
                        ISelectionItemProvider provider = peer.GetPattern(PatternInterface.SelectionItem) as ISelectionItemProvider;
                        if (null != provider)
                        {
                            provider.Select();
                        }
                        if (peer.GetClassName() == "RadioButton")
                        {
                            RibbonRadioButton ribbonRadioButton = target as RibbonRadioButton;

                            RoutedEventArgs newEventArgs = new RoutedEventArgs(ButtonBase.ClickEvent);
                            ribbonRadioButton.RaiseEvent(newEventArgs);
                            if (ribbonRadioButton.Command != null)
                            {
                                ribbonRadioButton.Command.Execute(ribbonRadioButton.CommandParameter);
                            }
                        }
                    }
                    HideKeyTips();
                }
            }
        }

        void m_dropDownbutton_IsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.OldValue)
            {
                HideKeyTips(KeyTipsCurrentLevel.Values);
            }

            DropDownButton button = d as DropDownButton;
            button.IsDropDownOpenChanged -= new PropertyChangedCallback(m_dropDownbutton_IsDropDownOpenChanged);
        }

        void m_splitMenuButton_IsMenuOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.OldValue)
            {
                HideKeyTips(KeyTipsCurrentLevel.Values);
            }

            SplitMenuButton button = d as SplitMenuButton;
            button.IsMenuOpenChanged -= new PropertyChangedCallback(m_splitMenuButton_IsMenuOpenChanged);
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Tries to find a key tips sublevel below the root KeyTip.
        /// </summary>
        /// <param name="rootKeyTip">Root KeyTip.</param>
        /// <returns>
        /// Returns true if attempt was successful, otherwise, false.
        /// </returns>
        private bool UpdateKeyTips(ref KeyTip rootKeyTip)
        {
            Dictionary<string,KeyTip> keyTipBranch = new Dictionary<string,KeyTip>();
            UIElement root = rootKeyTip.Host;
            FindKeyTips(ref keyTipBranch, root);
            if (keyTipBranch.Count > 0)
            {
                rootKeyTip.SubLevel = keyTipBranch;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Finds the key tips.
        /// </summary>
        /// <param name="keyTips">The key tips.</param>
        /// <param name="root">The root value.</param>
        private void FindKeyTips(ref Dictionary<string, KeyTip> keyTips, UIElement root)
        {
            if (root == null)
            {
                throw new ArgumentNullException("root element");
            }

            KeyTip keyTip;
            UIElement element;
            bool digDeeper = true, isbuttonpanel = false;

           
                foreach (object child in LogicalTreeHelper.GetChildren(root))
                {
                    element = child as UIElement;
                    if (element != null)
                    {
                        if (element is BackStageCommandButton || element is BackstageTabItem || element.GetType().Namespace.Contains("Syncfusion"))
                        {
                            digDeeper = true;
                        }                    

                        if (KeyTip.HasKeyTip(element))
                        {
                            keyTip = new KeyTip(element, KeyTip.GetKeyTip(element));
                            if (keyTips.ContainsKey(keyTip.Text))
                                keyTips.Remove(keyTip.Text);

                            keyTips.Add(keyTip.Text, keyTip);                        
                        }
                    }
                    else if (element != null && element is ButtonPanel)
                    {
                        digDeeper = true;
                        isbuttonpanel = true;
                    }
                }
            

            if (digDeeper)
            {
                if (root is ItemsControl && ((ItemsControl)root).ItemsSource != null)
                {
                    foreach (object child in ((ItemsControl)root).Items)
                    {
                        element = child as UIElement;
                        if (element != null)
                        {
                            FindKeyTips(ref keyTips, element);
                        }
                    }
                }
                else
                {
                    foreach (object child in LogicalTreeHelper.GetChildren(root))
                    {
                        element = child as UIElement;
                        if (element != null)
                        {
                            FindKeyTips(ref keyTips, element);
                        }
                    }
                }
            }

            if (isbuttonpanel)
            {
                foreach (object child in LogicalTreeHelper.GetChildren(root))
                {
                    element = child as UIElement;
                    if (element != null)
                    {
                        FindKeyTips(ref keyTips, element);
                    }
                }
            }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            this.backbutton = this.GetTemplateChild("BackstageBackButton") as RibbonButton;
            if (backbutton != null)
            {
                backbutton.Click += new RoutedEventHandler(backbutton_Click);
            }
            base.OnApplyTemplate();

            RibbonWindow window = VisualUtils.FindAncestor(this, typeof(RibbonWindow)) as RibbonWindow;
            if (window != null)
            {
                BindingUtils.SetBinding(this, window, FlowDirectionProperty, RibbonWindow.FlowDirectionProperty);
            }

        }

        void backbutton_Click(object sender, RoutedEventArgs e)
        {
            if (parentRibbon != null)
            
            {
                parentRibbon.HideBackStage();
            }

        }

        /// <summary>
        /// Raises the <see cref="E:Initialized"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            base.ItemContainerGenerator.StatusChanged += OnGeneratorStatusChanged;
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>
        /// The element that is used to display the given item.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new BackstageTabItem();
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>
        /// true if the item is (or is eligible to be) its own container; otherwise, false.
        /// </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            
            return ((item is BackstageTabItem) || (item is BackStageCommandButton) || (item is  BackStageSeparator) );
        }

        /// <summary>
        /// Updates the current selection when an item in the <see cref="T:System.Windows.Controls.Primitives.Selector"/> has changed
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            if ((e.Action == NotifyCollectionChangedAction.Remove) && (base.SelectedIndex == -1))
            {
                int startIndex = e.OldStartingIndex + 1;
                if (startIndex > base.Items.Count)
                {
                    startIndex = 0;
                }
                BackstageTabItem item = FindNextTabItem(startIndex, -1);
                if (item != null)
                {
                    item.IsSelected = true;
                }
            }
        }

        /// <summary>
        /// Called when the selection changes.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            if (e.AddedItems.Count > 0)
            {
                UpdateSelectedTabItemContent();
            }
            e.Handled = true;
        }


        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/> routed event is raised on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (e.StylusDevice == null || (ribbonTouch!=null && !ribbonTouch.EnableTouch))
            {
                base.OnMouseLeftButtonDown(e);
                HideKeyTips();
                e.Handled = true;
            }
        }

        #if !SyncfusionFramework3_5
        protected override void OnTouchDown(TouchEventArgs e)
        {
            var ribbonTouch = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            if (ribbonTouch!=null && ribbonTouch.EnableTouch && msystemGesture == SystemGesture.Tap)
            {
                base.OnTouchDown(e);
                HideKeyTips();
                e.Handled = true;
            }
        }
#endif

        protected override void OnStylusSystemGesture(StylusSystemGestureEventArgs e)
        {
            msystemGesture = e.SystemGesture;
            base.OnStylusSystemGesture(e);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Gets the current selected tab item.
        /// </summary>
        /// <returns></returns>
       internal  BackstageTabItem GetCurrentSelectedTabItem()
        {
            object selectedItem = base.SelectedItem;

            if (selectedItem == null)           
                return null;
         
            BackstageTabItem item = selectedItem as BackstageTabItem;
            if (item == null)
            {
                item = this.ItemContainerGenerator.ContainerFromItem(selectedItem) as BackstageTabItem;
                if(item == null)
                    item = FindNextTabItem(base.SelectedIndex, 1);
                base.SelectedItem = item;
            }
            if (item != null)
                item.Focus();
            return item;
        }

       /// <summary>
       /// Finds the next tab item.
       /// </summary>
       /// <param name="startIndex">The start index.</param>
       /// <param name="direction">The direction.</param>
       /// <returns></returns>
        private BackstageTabItem FindNextTabItem(int startIndex, int direction)
        {
            if (direction != 0)
            {
                int index = startIndex;
                for (int i = 0; i < base.Items.Count; i++)
                {
                    index += direction;

                    if (index >= base.Items.Count)
                        index = 0;

                    else if (index < 0)
                        index = base.Items.Count - 1;

                    BackstageTabItem item2 = base.ItemContainerGenerator.ContainerFromIndex(index) as BackstageTabItem;

                    if (((item2 != null) && item2.IsEnabled) && (item2.Visibility == Visibility.Visible))
                        return item2;
                }
            }
            return null;
        }
        
        /// <summary>
        /// Hides the key tips.
        /// </summary>
        internal void HideKeyTips()
        {
            if (b_keyTipPath != null)
            {
                HideKeyTips(FindKeyTipLevel(b_keyTipPath).Values);  
                if(KeyTipsCurrentLevel != null)
                KeyTipsCurrentLevel.Clear();
                b_isAdornersShown = false;
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Hides key tips.
        /// </summary>
        /// <param name="collection">Key tip's collection.</param>
        private void HideKeyTips(ICollection collection)
        {
            AdornerLayer adornerLayer;
            Adorner[] adorners;

            foreach (KeyTip keyTip in collection)
            {
                if (keyTip.Host != null)
                {
                    UIElement host = null;                   
                    host = keyTip.Host;                   

                    if (host != null)
                    {                 
                        adornerLayer = AdornerLayer.GetAdornerLayer(host);
                        if (adornerLayer != null)
                        {
                            adorners = adornerLayer.GetAdorners(host);

                            if (adorners != null)
                            {
                                foreach (Adorner adorner in adorners)
                                {
                                    if (adorner.Name == "PART_KeyTipAdorner")
                                    {
                                        adornerLayer.Remove(adorner);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            b_isAdornersShown = false;
        }

        /// <summary>
        /// Processing complex key tips.
        /// </summary>
        private void ProcessComplexKeyTips()
        {
            if (b_keyTipComplexLevel.Count == 1 && b_keyTipComplexLevel.ContainsKey(b_keyTipComplexPath))
            {
                InvokeKeyTip((KeyTip)b_keyTipComplexLevel[b_keyTipComplexPath]);
                b_keyTipComplexPath = string.Empty;
            }
            else
            {
                HideKeyTips(b_keyTipCurrentLevel.Values);
                ShowKeyTips(b_keyTipComplexLevel.Values);
            }

            b_complexLetterAdded = false;
            b_keyTipCurrentLevel = new Dictionary<string, KeyTip>();

            foreach (string key in b_keyTipComplexLevel.Keys)
            {
                b_keyTipCurrentLevel.Add(key, b_keyTipComplexLevel[key]);
            }

            b_keyTipComplexLevel.Clear();
            KeyTipsCurrentLevel = b_keyTipCurrentLevel;
        }

        /// <summary>
        /// Levels the up key tips.
        /// </summary>
        private void LevelUpKeyTips()
        {
            b_keyTipComplexPath = string.Empty;
            b_complexLetterAdded = false;
            b_keyTipCurrentLevel = FindKeyTipLevel(b_keyTipPath);
            HideKeyTips(b_keyTipCurrentLevel.Values);

            if (b_keyTipPath.Count > 0)
            {
                b_keyTipPath.RemoveAt(b_keyTipPath.Count - 1);
                b_keyTipCurrentLevel = FindKeyTipLevel(b_keyTipPath);
                ShowKeyTips(b_keyTipCurrentLevel.Values);
                KeyTipsCurrentLevel = b_keyTipCurrentLevel;
            }
            else
            {
                b_isAdornersShown = false;
                b_keyTipPath.Clear();
                if(KeyTipsCurrentLevel != null)
                KeyTipsCurrentLevel.Clear();
            }
        }
        /// <summary>
        /// Initializes the key tips.
        /// </summary>
        private void InitializeKeyTips()
        {
            b_keyTips = new Dictionary<string, KeyTip>();
            b_keyTipComplexLevel = new Dictionary<string, KeyTip>();
            b_keyTipCurrentLevel = new Dictionary<string, KeyTip>();
            b_keyTipPath = new List<string>();
            b_complexLetterAdded = false;
            FindKeyTips(ref b_keyTips, this);
        }

        /// <summary>
        /// Finds the key tip level.
        /// </summary>
        /// <param name="path">The path value.</param>
        /// <returns>result value</returns>
        private Dictionary<string, KeyTip> FindKeyTipLevel(List<string> path)
        {
            Dictionary<string, KeyTip> result = null;
            
            if (path != null && path.Count > 0)
            {
                result = b_keyTips;
                for (int i = 0; i < path.Count; i++)
                {
                    if (result.ContainsKey(path[i]))
                    {
                        if ((result[path[i]] as KeyTip).SubLevel != null)
                        {
                            result = (result[path[i]] as KeyTip).SubLevel;
                        }
                    }
                }
            }
            else
            {
                result = b_keyTips;
            }

            return result;
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Shows key tips.
        /// </summary>
        /// <param name="colection">Key tip's collection.</param>
        private void ShowKeyTips(ICollection colection)
        {
            RibbonAdorner adorner;
            AdornerLayer adornerLayer;
            b_ribbonflag = false;
            foreach (KeyTip keyTip in colection)
            {
                if (keyTip.Host != null)
                {                   
                    UIElement host = null;
                    
                    if (isInvoked)
                    {
                        host = keyTip.Host;
                    }
                    else if (keyTip.Host is BackStageCommandButton || keyTip.Host is BackstageTabItem)
                    {
                        var hostNew = keyTip.Host as UIElement;
                        if (hostNew != null && hostNew.Visibility == Visibility.Visible)
                            host = keyTip.Host;
                    }

                    if (host != null && host.GetType().Namespace.Contains("Syncfusion"))
                    {
                        adornerLayer = AdornerLayer.GetAdornerLayer(host);
                        if (adornerLayer == null)
                        {
                            adornerLayer = b_adornerLayer;                            
                        }

                        if (adornerLayer != null)
                        {
                            b_adornerLayer = adornerLayer;                            
                            adorner = new RibbonAdorner(host);
                            adorner.Enabled = host.IsEnabled;
                            adorner.Name = "PART_KeyTipAdorner";
                            adorner.Text = keyTip.Text;
                            LayoutKeyTipAdorner(keyTip, host, adorner);                            
                            try
                            {
                                adornerLayer.Add(adorner);
                            }
                            catch { host.InvalidateVisual(); }
                        }
                    }

                }
            }
           b_isAdornersShown = true;
        }

        /// <summary>
        /// Layouts the key tip adorner.
        /// </summary>
        /// <param name="tip">The tip value.</param>
        /// <param name="host">The host value.</param>
        /// <param name="adorner">The adorner.</param>
        private void LayoutKeyTipAdorner(KeyTip tip, UIElement host, TemplatedAdornerBase adorner)
        {
            Panel panel = VisualUtils.FindAncestor(host, typeof(LargeButtonPanel)) as Panel;
            if (panel == null)
            {
                panel = VisualUtils.FindAncestor(host, typeof(GroupPanel)) as Panel;
            }

            if (panel != null && panel.IsLoaded)
            {
                adorner.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                if (host is RibbonButton)
                {
                    if ((host as RibbonButton).SizeForm == SizeForm.ExtraSmall)
                    {
                        adorner.Margin = new Thickness(9, 0, 0, 0);
                    }
                }

                if (host is SplitButton || host is DropDownButton)
                {
                    if (host is DropDownButton && (host as DropDownButton).SizeForm == SizeForm.ExtraSmall)
                    {
                        adorner.Margin = new Thickness(5, 0, 0, 0);
                    }
                }

                Size hostSize = host.RenderSize;
                Size adornerSize = adorner.DesiredSizeInternal;
                Point pPoint = new Point(0, 0);
                Point chPoint = new Point(0, 0);
                if (PresentationSource.FromVisual(panel) != null)
                    pPoint = panel.PointToScreen(new Point(0, 0));
                if (PresentationSource.FromVisual(host) != null)
                    chPoint = host.PointToScreen(new Point(0, 0));
                Point adPoint = new Point(chPoint.X + (hostSize.Width - adornerSize.Width) / 2, chPoint.Y + (hostSize.Height - adornerSize.Height) / 2);

                double maxUp = adPoint.Y - (pPoint.Y - adornerSize.Height / 2);
                double maxDown = (pPoint.Y + panel.RenderSize.Height - adornerSize.Height / 2) - adPoint.Y;

                Thickness margin = host is FrameworkElement ? (host as FrameworkElement).Margin : new Thickness();
                double diffYUp = chPoint.Y - pPoint.Y;
                double diffYDown = (pPoint.Y + panel.DesiredSize.Height) - (chPoint.Y + hostSize.Height + margin.Bottom);
                double delta = adornerSize.Height + (hostSize.Height - adornerSize.Height) / 2;

                if (diffYUp <= adornerSize.Height)
                {                   
                   adorner.OffsetY = Math.Min(delta, maxUp);
                }

                if (diffYDown <= adornerSize.Height)
                {
                    adorner.OffsetY = Math.Min(delta, maxDown);
                }
            }

            if ((host is RibbonButton && (host as FrameworkElement).Name == "PART_DialogLauncherButton"))
            {
                adorner.OffsetY = 13;
            }
            else if ((host as FrameworkElement).Parent is DropDownButton)
            {
                adorner.OffsetY = 6;
                adorner.OffsetX = -(((host as FrameworkElement).RenderSize.Width / 2) - 20);
            }
           
            else if (host is SplitMenuButton || host is SimpleMenuButton)
            {
                adorner.OffsetY = 12;
                adorner.OffsetX = -(((host as FrameworkElement).RenderSize.Width / 2) - 30);
                if (KeyTip.HasSplitMenuKeyTip(host) && KeyTip.GetSplitMenuKeyTip(host) == tip.Text)
                    adorner.OffsetX = (((host as FrameworkElement).RenderSize.Width / 2) - 3);
            }
       
            else if (host is RibbonCheckBox)
            {
                adorner.OffsetY = -17;
                adorner.OffsetX = -(((host as FrameworkElement).RenderSize.Width / 2) - 20);
            }

            else if (host is BackStageCommandButton)
            {
                adorner.OffsetY = -12;
                adorner.OffsetX = -(((host as FrameworkElement).RenderSize.Width / 2) - 30);
            }
            else if (host is BackstageTabItem)
            {
                adorner.OffsetY = -12;
                adorner.OffsetX = -(((host as FrameworkElement).RenderSize.Width / 2) - 14);
            }


        }
            
        /// <summary>
        /// Shows the key tips.
        /// </summary>
        internal void ShowKeyTips()
        {         
            InitializeKeyTips();
            ShowKeyTips(b_keyTips.Values);
            b_isAdornersShown = true;
        }

        /// <summary>
        /// Updates the content of the selected tab item.
        /// </summary>
        private void UpdateSelectedTabItemContent()
        {
            if (base.SelectedIndex < 0)
                this.SelectedTabContent = null;
            else
            {
                BackstageTabItem selectedTabItem = this.GetCurrentSelectedTabItem();
                if (selectedTabItem != null)
                {
                    this.SelectedTabContent = selectedTabItem.Content;
                    UpdateLayout();
                }
            }
        }

        #endregion

        #region Event handling


        /// <summary>
        /// Called when [generator status changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnGeneratorStatusChanged(object sender, EventArgs e)
        {
            if (base.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                if (base.HasItems && (base.SelectedIndex == -1))
                {
                    base.SelectedIndex = 0;
                }
                this.UpdateSelectedTabItemContent();
               // ShowKeyTips();
            }
        }

        #endregion

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new BackStageAutomationPeer(this);
        }
    }

   
}