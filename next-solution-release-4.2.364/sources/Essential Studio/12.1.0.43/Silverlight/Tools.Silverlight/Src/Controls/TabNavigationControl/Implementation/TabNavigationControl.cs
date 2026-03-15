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
using System.Windows.Media;
using System.Collections.Specialized;
using Syncfusion.Windows.Shared;

#if WPF

using Syncfusion.Licensing;

#endif

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    ///
    /// </summary>
#if SILVERLIGHT
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
      Type = typeof(TabNavigationControl), XamlResource = "/Syncfusion.Theming.Blend;component/TabNavigationControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
        Type = typeof(TabNavigationControl), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/TabNavigationControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
        Type = typeof(TabNavigationControl), XamlResource = "/Syncfusion.Theming.Office2007Black;component/TabNavigationControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
        Type = typeof(TabNavigationControl), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/TabNavigationControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
        Type = typeof(TabNavigationControl), XamlResource = "/Syncfusion.Theming.Office2010Blue;component/TabNavigationControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
        Type = typeof(TabNavigationControl), XamlResource = "/Syncfusion.Theming.Office2010Black;component/TabNavigationControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
        Type = typeof(TabNavigationControl), XamlResource = "/Syncfusion.Theming.Office2010Silver;component/TabNavigationControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
        Type = typeof(TabNavigationControl), XamlResource = "/Syncfusion.Theming.Default;component/TabNavigationControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2003,
        Type = typeof(TabNavigationControl), XamlResource = "/Syncfusion.Theming.Office2003;component/TabNavigationControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Windows7,
       Type = typeof(TabNavigationControl), XamlResource = "/Syncfusion.Theming.Windows7;component/TabNavigationControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
    Type = typeof(TabNavigationControl), XamlResource = "/Syncfusion.Theming.VS2010;component/TabNavigationControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
    Type = typeof(TabNavigationControl), XamlResource = "/Syncfusion.Theming.Metro;component/TabNavigationControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Transparent,
   Type = typeof(TabNavigationControl), XamlResource = "/Syncfusion.Theming.Transparent;component/TabNavigationControl.xaml")]
#else
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Office2007Blue,
    //  Type = typeof(TabNavigationControl), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabNavigationControl/Themes/Office2007BlueStyle.xaml")]
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Office2007Black,
    //Type = typeof(TabNavigationControl), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabNavigationControl/Themes/Office2007BlackStyle.xaml")]
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Office2007Silver,
    //Type = typeof(TabNavigationControl), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabNavigationControl/Themes/Office2007SilverStyle.xaml")]
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Office2010Blue,
    // Type = typeof(TabNavigationControl), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabNavigationControl/Themes/Office2010BlueStyle.xaml")]
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Office2010Black,
    //Type = typeof(TabNavigationControl), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabNavigationControl/Themes/Office2010BlackStyle.xaml")]
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Office2010Silver,
    //Type = typeof(TabNavigationControl), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabNavigationControl/Themes/Office2010SilverStyle.xaml")]
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Blend,
    //Type = typeof(TabNavigationControl), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabNavigationControl/Themes/BlendStyle.xaml")]
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Default,
    //Type = typeof(TabNavigationControl), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabNavigationControl/Themes/Generic.xaml")]
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.VS2010,
    //Type = typeof(TabNavigationControl), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabNavigationControl/Themes/VS2010Style.xaml")]
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.ShinyBlue,
    // Type = typeof(TabNavigationControl), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabNavigationControl/Themes/ShinyBlueStyle.xaml")]
    //[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.ShinyRed,
    //Type = typeof(TabNavigationControl), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabNavigationControl/Themes/ShinyRedStyle.xaml")]
#endif

    public class TabNavigationControl : ItemsControl
    {
#if WPF
        internal bool IsTouchTranform = false;
        internal Point TotalTransformValue;
#if SyncfusionFramework4_0

        static TabNavigationControl()
        {
            //ResourceDictionary rd = new ResourceDictionary();
            //rd.Source = new Uri("Syncfusion.Tools.WPF;component/Themes/generic.xaml", UriKind.RelativeOrAbsolute);
            //rd.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("/Syncfusion.Tools.WPF;component/Controls/TabNavigationControl/Themes/Generic.xaml", UriKind.RelativeOrAbsolute) });
        }

#endif
#endif

        /// <summary>
        ///
        /// </summary>
        public TabNavigationControl()
        {
#if WPF
            System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
#if SyncfusionFramework4_0
            ResourceDictionary rdlocal = new ResourceDictionary();
            rdlocal.Source = new Uri("/Syncfusion.Tools.WPF;component/Controls/TabNavigationControl/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);
            Style = rdlocal[typeof(TabNavigationControl)] as Style;
            m_transitionManager = new TransitionManager(this);
#endif
#endif
            DefaultStyleKey = typeof(TabNavigationControl);
            this.SizeChanged += new SizeChangedEventHandler(TabNavigationControl_SizeChanged);
            //SelectedIndex = 0;
            this.Loaded += new RoutedEventHandler(TabNavigationControl_Loaded);
#if WPF
            if (EnvironmentTestTools.IsSecurityGranted)
            {
                EnvironmentTestTools.StartValidateLicense(typeof(TabNavigationControl));
            }
#endif
        }

        private void TabNavigationControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void TabNavigationControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.UpdateRect(e.NewSize.Width, e.NewSize.Height);
        }

        private NavigationCommand nextCommand;

        private NavigationCommand previousCommand;

        internal FrameworkElement contentRoot;

        internal ContentControl oldContent;

        internal ContentControl frontOldContent;

        internal ContentControl selectedContent;

        internal int oldSelectedIndex = -1;

        #region Events

        /// <summary>
        /// SelectionChagedEventHandler when Selection changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void SelectionChangedEventHandler(object sender, TabSelectionChangedEventArgs e);

        /// <summary>
        /// SelectionChagedEventHandler when Selection changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void SelectionChangingEventHandler(object sender, TabSelectionChangingEventArgs e);

        /// <summary>
        /// Occurs when the SelectionChanged.
        /// </summary>
        public event SelectionChangingEventHandler SelectionChanging;

        /// <summary>
        /// Occurs when the SelectionChanged.
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnSelectionChanging(TabSelectionChangingEventArgs e)
        {
            if (SelectionChanging != null)
            {
                SelectionChanging(this, e);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnSelectedChanged(TabSelectionChangedEventArgs e)
        {
            if (SelectionChanged != null)
            {
                SelectionChanged(this, e);
            }
        }

        #endregion Events

#if WPF
        internal TransitionManager m_transitionManager;
#endif

        /// <summary>
        ///
        /// </summary>
        public NavigationCommand NextCommand
        {
            get
            {
                if (nextCommand == null)
                {
                    nextCommand = new NavigationCommand(param => NextExecute(), param => NextCanExecute());
                }
                return nextCommand;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public NavigationCommand PreviousCommand
        {
            get
            {
                if (previousCommand == null)
                {
                    previousCommand = new NavigationCommand(param => PreviousExecute(), param => PreviousCanExecute());
                }
                return previousCommand;
            }
        }

        private int ValidateIndex(int selectedindex)
        {
            //if (selectedindex >= 0)
            //{
            //    if ((Items[selectedindex] as TabNavigationItem).Visibility == Visibility.Visible)
            //    {
            //        SelectedIndex = selectedindex;
            //    }
            //    else
            //    {
            //        for (int i = 0; i < Items.Count && Items.Count > selectedindex; i++)
            //        {
            //            if (i < Items.Count && (Items[i] as TabNavigationItem).Visibility == Visibility.Visible)
            //            {
            //                selectedindex = i;
            //                break;
            //            }
            //        }
            //        if (selectedindex >= 0 && this.Items[selectedindex] is TabNavigationItem && (this.Items[selectedindex] as TabNavigationItem).Visibility == Visibility.Collapsed)
            //            selectedindex = -1;
            //    }
            //}
            return selectedindex;
        }

        internal int CheckValidIndex(int selectedindex, bool IsNext)
        {
            if (IsNext)
            {
                for (int i = 1; i < Items.Count && Items.Count > selectedindex + i; )
                {
                    //Condition checked to hide the next button when the Visibility of a TabNavigationItem is collapsed
                    if (ItemsSource == null && (Items[selectedindex + i] as TabNavigationItem).Visibility == Visibility.Visible)
                    {
                        selectedindex += i;
                        i++;
                        return selectedindex;
                    }
                    else
                    {
                        selectedindex += i;
                        i++;
                        return selectedindex;
                    }
                }
            }
            else
            {
                for (int i = 1; i < Items.Count && (selectedindex - i) >= 0; )
                {
                    if (ItemsSource == null && (Items[selectedindex - i] as TabNavigationItem).Visibility == Visibility.Visible)
                    {
                        selectedindex -= i;
                        i++;
                        return selectedindex;
                    }
                    else
                    {
                        selectedindex -= i;
                        i++;
                        return selectedindex;
                    }
                }
            }
            return selectedindex == 0 && (Items[selectedindex] as TabNavigationItem).Visibility == Visibility.Collapsed ? -1 : selectedindex;
        }

        internal bool IsVisibleContent(int selectedindex, bool IsNext)
        {
            if (IsNext)
            {
                for (int i = 1; i < Items.Count - selectedindex; )
                {
                    //Condition checked to hide the next button when the Visibility of a TabNavigationItem is collapsed
                    if (ItemsSource == null && (Items[selectedindex + i] as TabNavigationItem).Visibility == Visibility.Visible)
                    {
                        i++;
                        return true;
                    }
                    else
                    {
                        i++;
                        return true;
                    }
                }
            }
            else
            {
                for (int i = 1; i < Items.Count && (selectedindex - i) >= 0; )
                {
                    if (ItemsSource == null && (Items[selectedindex - i] as TabNavigationItem).Visibility == Visibility.Visible)
                    {
                        i++;
                        return true;
                    }
                    else
                    {
                        i++;
                        return true;
                    }
                }
            }
            return false;
        }

        private void NextExecute()
        {
            TabSelectionChangingEventArgs chagingEventArgs = new TabSelectionChangingEventArgs()
            {
                OldItem = this.ItemContainerGenerator.ContainerFromIndex(SelectedIndex),
                NewItem = this.ItemContainerGenerator.ContainerFromIndex(CheckValidIndex(SelectedIndex, true)),
            };

            this.OnSelectionChanging(chagingEventArgs);

            if (!chagingEventArgs.Cancel)
                SelectedIndex = CheckValidIndex(SelectedIndex, true);
            else
                return;
        }

        private bool NextCanExecute()
        {
            return SelectedIndex != Items.Count - 1 && Items.Count > 0 && IsVisibleContent(SelectedIndex, true);
        }

        private void PreviousExecute()
        {
            TabSelectionChangingEventArgs changingEventArgs = new TabSelectionChangingEventArgs()
            {
                OldItem = this.ItemContainerGenerator.ContainerFromIndex(SelectedIndex),
                NewItem = this.ItemContainerGenerator.ContainerFromIndex(CheckValidIndex(SelectedIndex, false)),
            };

            this.OnSelectionChanging(changingEventArgs);

            if (!changingEventArgs.Cancel)
                SelectedIndex = CheckValidIndex(SelectedIndex, false);
            else
                return;
        }

        private bool PreviousCanExecute()
        {
            return SelectedIndex > 0 && Items.Count > 0 && IsVisibleContent(SelectedIndex, false);
        }

        /// <summary>
        ///
        /// </summary>
        public Thickness CornerRadius
        {
            get { return (Thickness)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(Thickness), typeof(TabNavigationControl), new PropertyMetadata(null));

        /// <summary>
        ///
        /// </summary>
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(TabNavigationControl), new PropertyMetadata(-1, new PropertyChangedCallback(OnSelectedIndexChanged)));

        /// <summary>
        ///
        /// </summary>
        public Visibility NavigationButtonVisibility
        {
            get { return (Visibility)GetValue(NavigationButtonVisibilityProperty); }
            set { SetValue(NavigationButtonVisibilityProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty NavigationButtonVisibilityProperty =
            DependencyProperty.Register("NavigationButtonVisibility", typeof(Visibility), typeof(TabNavigationControl), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        ///
        /// </summary>
        public Visibility HeaderVisibility
        {
            get { return (Visibility)GetValue(HeaderVisibilityProperty); }
            set { SetValue(HeaderVisibilityProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty HeaderVisibilityProperty =
            DependencyProperty.Register("HeaderVisibility", typeof(Visibility), typeof(TabNavigationControl), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        ///
        /// </summary>
        public Visibility TabStripVisibility
        {
            get { return (Visibility)GetValue(TabStripVisibilityProperty); }
            set { SetValue(TabStripVisibilityProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty TabStripVisibilityProperty =
            DependencyProperty.Register("TabStripVisibility", typeof(Visibility), typeof(TabNavigationControl), new PropertyMetadata(Visibility.Collapsed));

        ///// <summary>
        /////
        ///// </summary>
        //[Obsolete("No longer in use. Use TabStripVisibility instead.")]
        //public bool IsTabStripVisible
        //{
        //    get { return (bool)GetValue(IsTabStripVisibleProperty); }
        //    set { SetValue(IsTabStripVisibleProperty, value); }
        //}

        /// <summary>
        ///
        /// </summary>
        [Obsolete("No longer in use. Use TabStripVisibility instead.")]
        public static readonly DependencyProperty IsTabStripVisibleProperty =
            DependencyProperty.Register("IsTabStripVisible", typeof(bool), typeof(TabNavigationControl), new PropertyMetadata(true));

        /// <summary>
        ///
        /// </summary>
        public bool IsKeyboardNavigation
        {
            get { return (bool)GetValue(IsKeyboardNavigationProperty); }
            set { SetValue(IsKeyboardNavigationProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty IsKeyboardNavigationProperty =
            DependencyProperty.Register("IsKeyboardNavigation", typeof(bool), typeof(TabNavigationControl), new PropertyMetadata(true));

        /// <summary>
        ///
        /// </summary>
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(TabNavigationControl), new PropertyMetadata(null));

        /// <summary>
        ///
        /// </summary>
        public TabNavigationItem SelectedTabItem
        {
            get { return (TabNavigationItem)GetValue(SelectedTabItemProperty); }
            set { SetValue(SelectedTabItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedTabItem.  This enables animation, styling, binding, etc...
        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty SelectedTabItemProperty =
            DependencyProperty.Register("SelectedTabItem", typeof(TabNavigationItem), typeof(TabNavigationControl), new PropertyMetadata(null, new PropertyChangedCallback(OnSelectedTabItemChanged)));

        /// <summary>
        ///
        /// </summary>
        public TransitionEffects TransitionEffect
        {
            get { return (TransitionEffects)GetValue(TransitionEffectProperty); }
            set { SetValue(TransitionEffectProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TransitionEffect.  This enables animation, styling, binding, etc...
        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty TransitionEffectProperty =
            DependencyProperty.Register("TransitionEffect", typeof(TransitionEffects), typeof(TabNavigationControl), new PropertyMetadata(TransitionEffects.Slide));

#if WPF

        public bool EnableTouch
        {
            get { return (bool)GetValue(EnableTouchProperty); }
            set { SetValue(EnableTouchProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableTouch.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableTouchProperty =
            DependencyProperty.Register("EnableTouch", typeof(bool), typeof(TabNavigationControl), new PropertyMetadata(false, new PropertyChangedCallback(OnEnableTouchChanged)));

        public double TouchThreshold
        {
            get { return (double)GetValue(TouchThresholdProperty); }
            set { SetValue(TouchThresholdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TouchThreshold.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TouchThresholdProperty =
            DependencyProperty.Register("TouchThreshold", typeof(double), typeof(TabNavigationControl), new PropertyMetadata(125.0d));

#endif

        private static void OnSelectedIndexChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            TabNavigationControl instance = sender as TabNavigationControl;

            if (instance != null && ((int)args.NewValue >= 0))
            {
                if ((int)args.OldValue != -1)
                    instance.oldSelectedIndex = (int)args.OldValue;

                if (instance.Items.Count > (int)args.NewValue)
                {
                    TabNavigationItem selectedItem = instance.ItemContainerGenerator.ContainerFromIndex((int)args.NewValue) as TabNavigationItem;

                    if (selectedItem != null)
                    {
                        if (!(selectedItem.Visibility == Visibility.Visible))
                        {
                            instance.SelectedIndex++;
                        }
                    }
                }
                TabNavigationItem navItem = instance.ItemContainerGenerator.ContainerFromIndex((int)args.NewValue) as TabNavigationItem;
                if (null != navItem)
                {
                    navItem.IsSelected = true;
                }
                foreach (var item in instance.Items)
                {
                    TabNavigationItem _navitem = instance.ItemContainerGenerator.ContainerFromItem(item) as TabNavigationItem;
                    if (_navitem != null && _navitem != navItem)
                    {
                        _navitem.IsSelected = false;
                    }
                }
                if (instance.nextCommand != null)
                    instance.nextCommand.UpdateCanExecute();
                if (instance.previousCommand != null)
                    instance.previousCommand.UpdateCanExecute();

                TabSelectionChangedEventArgs changedEventArgs = new TabSelectionChangedEventArgs()
                {
                    OldItem = instance.ItemContainerGenerator.ContainerFromIndex(instance.oldSelectedIndex),
                    NewItem = instance.ItemContainerGenerator.ContainerFromIndex(instance.SelectedIndex),
                };

                instance.OnSelectedChanged(changedEventArgs);
            }
        }

#if WPF

        private static void OnEnableTouchChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            TabNavigationControl tnControl = sender as TabNavigationControl;
        }

#endif

        private static void OnSelectedTabItemChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            TabNavigationControl instance = sender as TabNavigationControl;
            instance.OnSelectedTabItemChanged(args);
        }

        private void OnSelectedTabItemChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.ItemsSource != null && selectedContent != null && ItemTemplate != null)
            {
                this.UpdateLayout();
                FrameworkElement oldchild, newfelement, oldfelement, newchild;
                oldchild = newfelement = oldfelement = newchild = null;
                if (selectedContent.Content != null)
                {
                    oldContent.Content = selectedContent.Content;
                    oldContent.ContentTemplate = this.ItemTemplate;
                    oldContent.UpdateLayout();
                }
                SelectedTabItem.Content = SelectedTabItem.DataContext;
#if WPF
                newfelement = VisualUtils.FindDescendant(selectedContent as Visual, typeof(FrameworkElement)) as FrameworkElement;
#else
                newfelement =VisualUtil.FindDescendant(selectedContent as DependencyObject, typeof(FrameworkElement)) as FrameworkElement;
#endif
                newchild = VisualTreeHelper.GetChild(newfelement as DependencyObject, 0) as FrameworkElement;

                if (selectedContent.Content != null)
                {
#if WPF
                    oldfelement = VisualUtils.FindDescendant(oldContent as Visual, typeof(FrameworkElement)) as FrameworkElement;
#else
                     oldfelement =VisualUtil.FindDescendant(oldContent as DependencyObject, typeof(FrameworkElement)) as FrameworkElement;
#endif
                    oldchild = VisualTreeHelper.GetChild(oldfelement as DependencyObject, 0) as FrameworkElement;
                    oldchild.DataContext = newchild.DataContext;
                }
                newchild.DataContext = SelectedTabItem.DataContext;
                if ((e.NewValue as TabNavigationItem) != null)
                    (e.NewValue as TabNavigationItem).Content = newchild;
                if ((e.OldValue as TabNavigationItem) != null)
                    (e.OldValue as TabNavigationItem).Content = oldchild;
            }

#if WPF
            if (this.IsTouchTranform)
            {
                if (!IsSlidedVertical)
                {
                    m_transitionManager.PlayTransitionEffect(e.OldValue as TabNavigationItem, e.NewValue as TabNavigationItem, TransitionEffects.Slide);
                }
                else
                {
                    m_transitionManager.PlayTransitionEffect(e.OldValue as TabNavigationItem, e.NewValue as TabNavigationItem, TouchVertialTransitionEffect);
                }
            }
            else
            {
                m_transitionManager.PlayTransitionEffect(e.OldValue as TabNavigationItem, e.NewValue as TabNavigationItem, TransitionEffect);
            }
#else
                TransitionManager.PlayTransitionEffect(this,e.OldValue as TabNavigationItem, e.NewValue as TabNavigationItem, this.TransitionEffect);
#endif
        }

#if WPF

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            this.Focus();
            base.OnMouseDown(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (IsKeyboardNavigation)
            {
                if (e.Key == Key.Right)
                {
                    if (NextCommand.CanExecute(null))
                    {
                        NextCommand.Execute(null);
                    }
                    this.Focus();
                }
                if (e.Key == Key.Left)
                {
                    if (PreviousCommand.CanExecute(null))
                    {
                        PreviousCommand.Execute(null);
                        this.Focus();
                    }
                }
            }
            base.OnKeyDown(e);
        }

#endif

#if SILVERLIGHT

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.Focus();
            base.OnMouseLeftButtonDown(e);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.Key == Key.Right)
            {
                if (NextCommand.CanExecute(null))
                {
                    NextCommand.Execute(null);
                }
            }
            if (e.Key == Key.Left)
            {
                if (PreviousCommand.CanExecute(null))
                {
                    PreviousCommand.Execute(null);
                }
            }
            base.OnKeyDown(e);
        }
#endif

        internal void UpdateRect(double width, double height)
        {
            if (contentRoot != null)
            {
                contentRoot.Clip = new RectangleGeometry() { Rect = new Rect(new Point(0, 0), new Size(width, height)) };
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (this.Items != null)
                {
                    if (this.SelectedIndex >= this.Items.Count || this.SelectedIndex < -1)
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                if (Items != null && Items.Count == 0)
                {
                    SelectedItem = null;
                    SelectedTabItem = null;
                }
            }

            if (nextCommand != null)
                nextCommand.UpdateCanExecute();

            if (previousCommand != null)
                previousCommand.UpdateCanExecute();

            base.OnItemsChanged(e);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TabNavigationItem;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TabNavigationItem();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="element"></param>
        /// <param name="item"></param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            TabNavigationItem navitem = element as TabNavigationItem;
            navitem.parent = this;
            //SelectedIndex = ValidateIndex(SelectedIndex);
            if (Items.IndexOf(navitem) < 0)
            {
                if (Items.IndexOf(item) == SelectedIndex)
                {
                    navitem.IsSelected = true;
                }
            }
            else if (Items.IndexOf(navitem) == SelectedIndex)
            {
                navitem.IsSelected = true;
            }
            base.PrepareContainerForItemOverride(element, item);
        }

#if WPF

        private Point StartingPoint = new Point();
        private FrameworkElement PrevContent = null, NextContent = null, CurrentContent = null;
        private TabNavigationItem PrevNavigationItem, NextNavigationItem, CurrentNavigationItem;
        internal bool IsTouchTransformReset = false, IsLast = false, IsSlidedVertical = false;
        private TransitionEffects TouchVertialTransitionEffect;
        private double factor;

        protected override void OnManipulationStarted(ManipulationStartedEventArgs e)
        {
            if (EnableTouch)
            {
                factor = 0.6;
                IsTouchTranform = true;
                StartingPoint = e.ManipulationOrigin;

                if ((SelectedIndex - 1) >= 0)
                {
                    PrevNavigationItem = this.Items[this.SelectedIndex - 1] as TabNavigationItem;

                    if (PrevNavigationItem == null)
                        PrevNavigationItem = this.ItemContainerGenerator.ContainerFromIndex(this.SelectedIndex - 1) as TabNavigationItem;

                    PrevContent = PrevNavigationItem.Content as FrameworkElement;

                    oldContent.Content = PrevContent;
                    frontOldContent.Opacity = 0;
                }

                if (SelectedIndex + 1 <= Items.Count - 1)
                {
                    NextNavigationItem = this.Items[this.SelectedIndex + 1] as TabNavigationItem;

                    if (NextNavigationItem == null)
                        NextNavigationItem = this.ItemContainerGenerator.ContainerFromIndex(this.SelectedIndex + 1) as TabNavigationItem;

                    NextContent = NextNavigationItem.Content as FrameworkElement;

                    frontOldContent.Content = NextContent;
                    oldContent.Opacity = 0;
                }

                CurrentNavigationItem = this.ItemContainerGenerator.ContainerFromIndex(this.SelectedIndex) as TabNavigationItem;
                CurrentContent = CurrentNavigationItem.Content as FrameworkElement;

                base.OnManipulationStarted(e);
            }
        }

        protected override void OnManipulationCompleted(ManipulationCompletedEventArgs e)
        {
            if (EnableTouch)
            {
                IsTouchTransformReset = false;
                TotalTransformValue = new Point(e.TotalManipulation.Translation.X, e.TotalManipulation.Translation.Y);
                if ((e.TotalManipulation.Translation.X < 0 && !IsSlidedVertical) || e.TotalManipulation.Translation.Y < 0 && IsSlidedVertical)
                {
                    if ((Math.Abs(e.TotalManipulation.Translation.X) > (ActualWidth / 2) || Math.Abs(e.TotalManipulation.Translation.Y) > (ActualHeight / 2)) && (SelectedIndex + 1) <= Items.Count - 1)
                    {
                        TouchVertialTransitionEffect = TransitionEffects.Push;
                        SelectedIndex = SelectedIndex + 1;
                    }
                    else
                    {
                        IsTouchTransformReset = true;
                        if (this.SelectedIndex < Items.Count - 1)
                        {
                            if (!IsSlidedVertical)
                            {
                                m_transitionManager.PlayTransitionEffect(NextNavigationItem, CurrentNavigationItem, TransitionEffects.Slide);
                            }
                            else
                                m_transitionManager.PlayTransitionEffect(NextNavigationItem, CurrentNavigationItem, TransitionEffects.PushIn);
                        }
                        else
                        {
                            TotalTransformValue.X = TotalTransformValue.X * factor; TotalTransformValue.Y = TotalTransformValue.Y * factor;

                            if (!IsSlidedVertical)
                            {
                                IsLast = true;
                                m_transitionManager.PlayTransitionEffect(null, CurrentNavigationItem, TransitionEffects.Slide);
                            }
                            else
                                m_transitionManager.PlayTransitionEffect(null, CurrentNavigationItem, TransitionEffects.PushIn);
                        }
                    }
                }
                else
                {
                    if (((e.TotalManipulation.Translation.X > (ActualWidth / 2)) || e.TotalManipulation.Translation.Y > (ActualHeight / 2)) && (SelectedIndex - 1) >= 0)
                    {
                        TouchVertialTransitionEffect = TransitionEffects.PushIn;
                        SelectedIndex = SelectedIndex - 1;
                    }
                    else
                    {
                        IsTouchTransformReset = true;
                        if (this.SelectedIndex > 0)
                        {
                            if (!IsSlidedVertical)
                                m_transitionManager.PlayTransitionEffect(PrevNavigationItem, CurrentNavigationItem, TransitionEffects.Slide);
                            else
                                m_transitionManager.PlayTransitionEffect(PrevNavigationItem, CurrentNavigationItem, TransitionEffects.Push);
                        }
                        else
                        {
                            TotalTransformValue.X = TotalTransformValue.X * factor; TotalTransformValue.Y = TotalTransformValue.Y * factor;
                            if (!IsSlidedVertical)
                                m_transitionManager.PlayTransitionEffect(null, CurrentNavigationItem, TransitionEffects.Slide);
                            else
                                m_transitionManager.PlayTransitionEffect(null, CurrentNavigationItem, TransitionEffects.Push);
                        }
                    }
                }
                IsLast = false;
                IsTouchTranform = false;
                IsSlidedVertical = false;
                PrevContent = NextContent = CurrentContent = null;
                base.OnManipulationCompleted(e);
            }
        }

        protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
        {
            if (EnableTouch)
            {
                if (Math.Abs(e.CumulativeManipulation.Translation.X) > Math.Abs(e.CumulativeManipulation.Translation.Y))
                {
                    IsSlidedVertical = false;
                    TranslateItemForTouch(e.ManipulationOrigin.X, ActualWidth, e.CumulativeManipulation.Translation.X, 0, StartingPoint.X);
                    if (e.ManipulationOrigin.X < 0 || e.ManipulationOrigin.X > this.ActualWidth)
                    {
                        e.Complete();
                    }
                }
                else
                {
                    IsSlidedVertical = true;
                    TranslateItemForTouch(e.ManipulationOrigin.Y, ActualHeight - (contentRoot as Grid).RowDefinitions[0].ActualHeight, 0, e.CumulativeManipulation.Translation.Y, StartingPoint.Y);
                    if ((e.ManipulationOrigin.Y < ((contentRoot as Grid).RowDefinitions[0].ActualHeight)) || e.ManipulationOrigin.Y > this.ActualHeight)
                    {
                        e.Complete();
                    }
                }
                base.OnManipulationDelta(e);
            }
        }

        public void TranslateItemForTouch(double ManipulationOriginValue, double ActutalValue, double CumulativeX, double CumulativeY, double StatingManipulationValue)
        {
            if (ManipulationOriginValue <= StatingManipulationValue)
            {
                //To bring the End Slide effect.
                if (ManipulationOriginValue <= ActutalValue / 4)
                    factor = 0.3;
                else if (ManipulationOriginValue <= ActutalValue / 3)
                    factor = 0.4;
                else if (ManipulationOriginValue <= ActutalValue / 2)
                    factor = 0.5;

                if (CurrentContent != null && (this.SelectedIndex < this.Items.Count - 1))
                    CurrentContent.RenderTransform = new TranslateTransform(CumulativeX, CumulativeY);
                else
                    CurrentContent.RenderTransform = new TranslateTransform(CumulativeX * factor, CumulativeY * factor);

                if (NextContent != null)
                    NextContent.RenderTransform = new TranslateTransform(CumulativeX != 0 ? CumulativeX + (ActutalValue) : 0, CumulativeY != 0 ? CumulativeY + (ActutalValue) : 0);
                frontOldContent.Opacity = 1;
                oldContent.Opacity = 0;
            }
            if (ManipulationOriginValue > StatingManipulationValue)
            {
                //To bring the begin Slide effect.
                if (ManipulationOriginValue >= ActutalValue / 2)
                    factor = 0.3;
                else if (ManipulationOriginValue >= ActutalValue / 3)
                    factor = 0.4;
                else if (ManipulationOriginValue >= ActutalValue / 4)
                    factor = 0.5;

                if (CurrentContent != null && (this.SelectedIndex > 0))
                    CurrentContent.RenderTransform = new TranslateTransform(CumulativeX, CumulativeY);
                else
                    CurrentContent.RenderTransform = new TranslateTransform(CumulativeX * factor, CumulativeY * factor);

                if (PrevContent != null)
                    PrevContent.RenderTransform = new TranslateTransform(CumulativeX != 0 ? CumulativeX - (ActutalValue) : 0, CumulativeY != 0 ? CumulativeY - (ActutalValue) : 0);
                oldContent.Opacity = 1;
                frontOldContent.Opacity = 0;
            }
        }

#endif

        /// <summary>
        ///
        /// </summary>
        public override void OnApplyTemplate()
        {
#if WPF
            if (SelectedIndex == -1 && this.Items.Count > 0)
            {
                SetValue(SelectedIndexProperty, 0);
            }
#endif
            oldContent = GetTemplateChild("PART_OldContent") as ContentControl;
            frontOldContent = GetTemplateChild("PART_FrontOldContent") as ContentControl;
            selectedContent = GetTemplateChild("PART_SelectedContent") as ContentControl;
            if (selectedContent != null)
            {
                selectedContent.Unloaded -= new RoutedEventHandler(selectedContent_Unloaded);
                selectedContent.Unloaded += new RoutedEventHandler(selectedContent_Unloaded);
            }
            contentRoot = GetTemplateChild("Root") as FrameworkElement;
            base.OnApplyTemplate();
        }

        private void selectedContent_Unloaded(object sender, RoutedEventArgs e)
        {
#if SILVERLIGHT
            selectedContent.Content = null;
            selectedContent.DataContext = null;
#endif
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class TabSelectionChangingEventArgs : EventArgs
    {
        /// <summary>
        ///
        /// </summary>
        public object OldItem { get; set; }

        /// <summary>
        ///
        /// </summary>
        public object NewItem { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool Cancel { get; set; }
    }

    /// <summary>
    ///
    /// </summary>
    public class TabSelectionChangedEventArgs : EventArgs
    {
        /// <summary>
        ///
        /// </summary>
        public object OldItem { get; set; }

        /// <summary>
        ///
        /// </summary>
        public object NewItem { get; set; }
    }
}