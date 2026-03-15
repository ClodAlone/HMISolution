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
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Controls;
using System.Collections;
using System.Collections.ObjectModel;
using System.Reflection;
using Syncfusion.Windows.Shared;
using System.Windows.Input;
using System.Collections.Specialized;

#if WPF

using Syncfusion.Licensing;
using System.Windows.Media;
using System.Windows.Threading;

#endif

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    ///
    /// </summary>
#if SILVERLIGHT
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
      Type = typeof(ComboBoxAdv), XamlResource = "/Syncfusion.Theming.Blend;component/ComboBoxAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
        Type = typeof(ComboBoxAdv), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/ComboBoxAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
        Type = typeof(ComboBoxAdv), XamlResource = "/Syncfusion.Theming.Office2007Black;component/ComboBoxAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
        Type = typeof(ComboBoxAdv), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/ComboBoxAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
        Type = typeof(ComboBoxAdv), XamlResource = "/Syncfusion.Theming.Default;component/ComboBoxAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2003,
        Type = typeof(ComboBoxAdv), XamlResource = "/Syncfusion.Theming.Office2003;component/ComboBoxAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
       Type = typeof(ComboBoxAdv), XamlResource = "/Syncfusion.Theming.Office2010Blue;component/ComboBoxAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
        Type = typeof(ComboBoxAdv), XamlResource = "/Syncfusion.Theming.Office2010Black;component/ComboBoxAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
        Type = typeof(ComboBoxAdv), XamlResource = "/Syncfusion.Theming.Office2010Silver;component/ComboBoxAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Windows7,
        Type = typeof(ComboBoxAdv), XamlResource = "/Syncfusion.Theming.Windows7;component/ComboBoxAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
      Type = typeof(ComboBoxAdv), XamlResource = "/Syncfusion.Theming.VS2010;component/ComboBoxAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
     Type = typeof(ComboBoxAdv), XamlResource = "/Syncfusion.Theming.Metro;component/ComboBoxAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Transparent,
     Type = typeof(ComboBoxAdv), XamlResource = "/Syncfusion.Theming.Transparent;component/ComboBoxAdv.xaml")]
#else

    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
   Type = typeof(ComboBoxAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ComboBoxAdv/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(ComboBoxAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ComboBoxAdv/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(ComboBoxAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ComboBoxAdv/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
    Type = typeof(ComboBoxAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ComboBoxAdv/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(ComboBoxAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ComboBoxAdv/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(ComboBoxAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ComboBoxAdv/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(ComboBoxAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ComboBoxAdv/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(ComboBoxAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ComboBoxAdv/Themes/Generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
    Type = typeof(ComboBoxAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ComboBoxAdv/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
    Type = typeof(ComboBoxAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ComboBoxAdv/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
Type = typeof(ComboBoxAdv), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ComboBoxAdv/Themes/TransparentStyle.xaml")]
#endif
#if !WPF
    public class ComboBoxAdv : ItemsControl
#else
    public class ComboBoxAdv : Selector
#endif
    {
#if SILVERLIGHT
                /// <summary>
                ///
                /// </summary>
                public event SelectionChangedEventHandler SelectionChanged;
#endif

        /// <summary>
        /// Occurs when the drop-down list of the combo box closes.
        /// </summary>
        public event EventHandler DropDownClosed;

        /// <summary>
        /// Occurs when the drop-down list of the combo box opens.
        /// </summary>
        public event EventHandler DropDownOpened;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComboBoxAdv"/> class.
        /// </summary>
        public ComboBoxAdv()
        {
            DefaultStyleKey = typeof(ComboBoxAdv);
            if (SelItemsInternal == null)
            {
                SelItemsInternal = new List<object>();
            }

#if WPF
            if (EnvironmentTest.IsSecurityGranted)
            {
                EnvironmentTest.StartValidateLicense(typeof(ComboBoxAdv));
            }
            EventManager.RegisterClassHandler(typeof(ComboBoxAdv), Mouse.MouseWheelEvent, new MouseWheelEventHandler(OnMouseWheel), true);

            EventManager.RegisterClassHandler(typeof(ComboBoxAdv), Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseButtonDown), true);

#endif
        }

        internal bool IsGotKeyBoardFocus = false;

        internal object oldItem;

        internal object newItem;

        internal int itemcount = 0;

        internal bool removeFlag = false;

        internal bool internalSelect = false;

        internal bool AllowSelect = false;

        internal bool ExternalChange = true;

        private ItemsControl selectedItems;

        internal ScrollViewer DropDownScrollBar = null;

        private ContentPresenter selectedContent;

        internal TextBlock defaultText;

        private ToggleButton toggleButton = null;

#if WPF
        public string searchText = "";

        private TextBox Part_IsEdit = null;

        private int charindex = 0;

        private DispatcherTimer timer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(5) };

        public char? oldTempChar { get; set; }

        public char newTempChar { get; set; }

#endif
#if SILVERLIGHT

        internal Popup popup;

        internal Border popupBorder;

        internal SelectionChangedEventArgs SelectionChangedEvent;

#endif

#if WPF
        internal Popup popup;

        internal new SelectionChangedEventArgs SelectionChangedEvent;
#endif
        internal int index = 0;

        internal bool internalChange = false;

        protected override void OnLostFocus(RoutedEventArgs e)
        {
#if WPF
            if (Part_IsEdit != null && !IsDropDownOpen && !Part_IsEdit.IsVisible)
            {
                timer.Stop();
                searchText = "";
            }
            if (toggleButton != null && toggleButton.IsChecked == true && !popup.IsKeyboardFocusWithin)
            {
                toggleButton.IsChecked = false;
            }
#endif
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own ItemContainer.
        /// </summary>
        /// <param name="item">Specified item.</param>
        /// <returns>
        /// true if the item is its own ItemContainer; otherwise, false.
        /// </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ComboBoxItemAdv;
        }

        /// <summary>
        /// Creates or identifies the element used to display the specified item.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Windows.Controls.ComboBoxItem"/>.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ComboBoxItemAdv();
        }

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">Element used to display the specified item.</param>
        /// <param name="item">Specified item.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            ComboBoxItemAdv comboItem = element as ComboBoxItemAdv;
            ClearSelection();

            if (ItemsSource != null)
            {
                comboItem.ContentTemplate = ItemTemplate;
                if (SelectionBoxItemTemplate == null)
                {
                    SelectionBoxItemTemplate = ItemTemplate;
                }
            }
            if (comboItem.CheckBox != null)
            {
                if (!AllowMultiSelect)
                {
                    comboItem.CheckBox.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    comboItem.CheckBox.Visibility = System.Windows.Visibility.Visible;
                }
            }
#if WPF
            if (ItemsSource != null)
            {
                comboItem.ContentTemplateSelector = ItemTemplateSelector;
            }
#endif
            comboItem.Parent = this;
            if (item is ComboBoxItemAdv)
            {
                base.PrepareContainerForItemOverride(comboItem, item);
            }
            else
            {
#if !WPF
                if (DisplayMemberPath == null)
                {
                    comboItem.Content = item;
                }
                else if(item != null)
                {
                    Type type = item.GetType();
                    PropertyInfo propertyInfo = type.GetProperty(DisplayMemberPath, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
                    comboItem.Content = propertyInfo.GetValue(item, null);
                }
#else
                comboItem.Content = item;
                UpdateSelectionBox();
#endif
            }
        }

#if WPF

        /// <summary>
        /// Called when the selection changes.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            if (this.SelectedItem == null || this.SelectedIndex < 0)
            {
                this.SelectedItem = null;
                this.SelectedIndex = -1;
                this.SelectionBoxItem = null;
            }
            if (this.SelectedItem != null && this.SelItemsInternal.Count <= 0)
            {
                if (this.SelectedItem is ComboBoxItemAdv)
                {
                    if ((this.SelectedItem as ComboBoxItemAdv).Parent != null)
                        (this.SelectedItem as ComboBoxItemAdv).UpdateSelection();

                    this.SelectionBoxItem = (this.SelectedItem as ComboBoxItemAdv).Content;
#if WPF
                    if (Part_IsEdit != null && Part_IsEdit.Visibility == Visibility.Visible)
                    {
                        Part_IsEdit.Text = searchText;
                        if (searchText != null)
                        {
                            if (SelectionBoxItem == null)
                            {
                                Part_IsEdit.Text = SelectionBoxItem.ToString().Remove(searchText.Count(), (SelectionBoxItem.ToString().Count() - searchText.Count()));
                                Part_IsEdit.CaretIndex = searchText.Count();
                                Part_IsEdit.AppendText(SelectedItem.ToString().Remove(0, searchText.Count()));
                                Part_IsEdit.SelectionStart = searchText.Count();
                                Part_IsEdit.SelectionLength = SelectedItem.ToString().Count() - searchText.Count();
                            }
                            else
                            {
                                Part_IsEdit.Text = SelectionBoxItem.ToString().Remove(searchText.Count(), (SelectionBoxItem.ToString().Count() - searchText.Count()));
                                Part_IsEdit.CaretIndex = searchText.Count();
                                Part_IsEdit.AppendText(SelectionBoxItem.ToString().Remove(0, searchText.Count()));
                                Part_IsEdit.SelectionStart = searchText.Count();
                                Part_IsEdit.SelectionLength = SelectionBoxItem.ToString().Count() - searchText.Count();
                            }
                        }
                    }
#endif
                }
                else
                {
                    this.SelectionBoxItem = this.SelectedItem;
#if WPF
                    if (Part_IsEdit != null && Part_IsEdit.Visibility == Visibility.Visible)
                    {
                        if (string.IsNullOrEmpty(DisplayMemberPath))
                        {
                            if (!string.IsNullOrEmpty(searchText))
                            {
                                Part_IsEdit.Text = SelectionBoxItem.ToString().Remove(searchText.Count(), (SelectionBoxItem.ToString().Count() - searchText.Count()));
                                Part_IsEdit.CaretIndex = searchText.Count();
                                Part_IsEdit.AppendText(SelectionBoxItem.ToString().Remove(0, searchText.Count()));
                                Part_IsEdit.SelectionStart = searchText.Count();
                                Part_IsEdit.SelectionLength = SelectionBoxItem.ToString().Count() - searchText.Count();
                            }
                            else
                            {
                                Part_IsEdit.Text = SelectionBoxItem.ToString();
                                Part_IsEdit.SelectAll();
                            }
                        }
                        else
                        {
                            Type t = SelectionBoxItem.GetType();
                            var prop = t.GetProperty(DisplayMemberPath);
                            object s = prop == null ? null : prop.GetValue(SelectionBoxItem, null);
                            if (s != null)
                            {
                                Part_IsEdit.Text = s.ToString().Remove(searchText.Count(), (s.ToString().Count() - searchText.Count()));
                                Part_IsEdit.CaretIndex = searchText.Count();
                                Part_IsEdit.AppendText(s.ToString().Remove(0, searchText.Count()));
                                Part_IsEdit.SelectionStart = searchText.Count();
                                Part_IsEdit.SelectionLength = s.ToString().Count() - searchText.Count();
                            }
                        }
                    }
#endif
                }

                this.SelItemsInternal.Add(this.SelectedItem);

                if (SelectedItems == null && SelItemsInternal.Count > 0)
                {
                    ObservableCollection<object> collecion = new ObservableCollection<object>();
                    foreach (var item in SelItemsInternal)
                    {
                        collecion.Add(item);
                    }
                    SelectedItems = collecion;
                }
            }
            SelectionChangedEvent = e;
        }

#endif

        internal ComboBoxItemAdv GetItemContainer(object obj)
        {
            return ItemContainerGenerator.ContainerFromItem(obj) as ComboBoxItemAdv;
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            selectedItems = GetTemplateChild("PART_SelectedItems") as ItemsControl;
            selectedContent = GetTemplateChild("ContentPresenter") as ContentPresenter;
            defaultText = GetTemplateChild("PART_DefaultText") as TextBlock;
            popup = GetTemplateChild("PART_Popup") as Popup;
            toggleButton = GetTemplateChild("PART_ToggleButton") as ToggleButton;

#if WPF
            DropDownScrollBar = GetTemplateChild("DropDownScrollViewer") as ScrollViewer;
            Part_IsEdit = GetTemplateChild("PART_Editable") as TextBox;
            if (Part_IsEdit != null)
            {
                Part_IsEdit.TextChanged += Part_IsEdit_TextChanged;
                Part_IsEdit.PreviewKeyDown += Part_IsEdit_PreviewKeyDown;
                Part_IsEdit.LostFocus += Part_IsEdit_LostFocus;
                Part_IsEdit.GotFocus += Part_IsEdit_GotFocus;
            }
            GotFocus -= ComboBoxAdv_GotFocus;
            GotFocus += ComboBoxAdv_GotFocus;
#else
            DropDownScrollBar = GetTemplateChild("ScrollViewer") as ScrollViewer;
#endif
            if (popup != null)
            {
                popup.Closed -= new EventHandler(popup_Closed);
                popup.Closed += new EventHandler(popup_Closed);
            }
#if WPF
            Window MainWindow = VisualUtils.FindAncestor(this, typeof(Window)) as Window;
            if (MainWindow != null)
            {
                MainWindow.PreviewMouseLeftButtonUp -= new MouseButtonEventHandler(MainWindow_PreviewMouseLeftButtonUp);
                MainWindow.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(MainWindow_PreviewMouseLeftButtonUp);
                MainWindow.LocationChanged -= new EventHandler(MainWindow_LocationChanged);
                MainWindow.LocationChanged += new EventHandler(MainWindow_LocationChanged);
                MainWindow.Deactivated -= new EventHandler(MainWindow_Deactivated);
                MainWindow.Deactivated += new EventHandler(MainWindow_Deactivated);
            }
#else
            Application.Current.RootVisual.MouseLeftButtonDown -= new MouseButtonEventHandler(RootVisual_MouseLeftButtonDown);
            Application.Current.RootVisual.GotFocus -= new RoutedEventHandler(RootVisual_GotFocus);
            Application.Current.RootVisual.MouseLeftButtonDown += new MouseButtonEventHandler(RootVisual_MouseLeftButtonDown);
            Application.Current.RootVisual.GotFocus += new RoutedEventHandler(RootVisual_GotFocus);
            popup = GetTemplateChild("Popup") as Popup;
            popupBorder = GetTemplateChild("PopupBorder") as Border;
#endif
            this.Unloaded -= new RoutedEventHandler(ComboBoxAdv_Unloaded);
            this.Unloaded += new RoutedEventHandler(ComboBoxAdv_Unloaded);
            UpdateSelectionBox();
            UpdateSelectMode();
            this.SelectionChanged += new SelectionChangedEventHandler(ComboBoxAdv_SelectionChanged);
#if WPF
            timer.Tick += timer_Tick;
#endif
        }

        private void ComboBoxAdv_GotFocus(object sender, RoutedEventArgs e)
        {
#if WPF
            if (Part_IsEdit != null && Part_IsEdit.IsVisible)
            {
                Part_IsEdit.Focus();
            }
#endif
        }

#if WPF

        private void Part_IsEdit_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SelectedItem != null && sender is TextBox)
            {
                (sender as TextBox).SelectionStart = 0;
                (sender as TextBox).SelectionLength = SelectedItem.ToString().Count();
            }
        }

        private void Part_IsEdit_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox)
            {
                searchText = "";
                (sender as TextBox).SelectionStart = 0;
                (sender as TextBox).SelectionLength = 0;
            }
        }

        private bool _keypressed = false;

        private void Part_IsEdit_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Back)
                if (Char.IsLetter((e.Key).ToString().ToCharArray()[0]))
                {
                    _keypressed = true;
                }
        }

        private IEnumerable<Object> _tempcoll;
        private IEnumerable<ComboBoxItemAdv> searchCollection;

        private void Part_IsEdit_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsTextSearchEnabled)
            {
                if (_keypressed)
                {
                    List<object> Temp = new List<object>();
                    searchText = Part_IsEdit.Text;
                    bool StringFlag, MemberPathFlag;
                    object TestObj = Items[0];
                    StringFlag = TestObj is string;
                    MemberPathFlag = DisplayMemberPath.Equals(String.Empty);
                    if (Items != null)
                    {
                        if (ItemsSource == null && Items.Count > 0)
                        {
#if !SyncfusionFramework3_5
                            if (!this.IsTextSearchCaseSensitive)
                                searchCollection = this.Items.OfType<ComboBoxItemAdv>().Where(item => (item as ComboBoxItemAdv).Content.ToString().StartsWith(searchText, StringComparison.OrdinalIgnoreCase));
                            else
#endif
                                searchCollection = this.Items.OfType<ComboBoxItemAdv>().Where(item => (item as ComboBoxItemAdv).Content.ToString().StartsWith(searchText, StringComparison.Ordinal));

                            if (searchCollection.Count() > 0)
                            {
                                _keypressed = false;
                                this.SelectedItem = null;
                                this.SelectedIndex = this.Items.IndexOf(searchCollection.ToList()[0]);
                            }
                        }
                        else if (ItemsSource != null)
                        {
                            int icout = 0;
                            if (StringFlag)
                            {
                                foreach (object i in Items)
                                {
                                    if (i != null)
                                    {
                                        Temp.Add((String)i + "," + icout);
                                    }
                                }
                            }
                            else
                            {
                                foreach (object i in Items)
                                {
                                    if (!MemberPathFlag)
                                    {
                                        Type t = i.GetType();
                                        var prop = t.GetProperty(DisplayMemberPath);
                                        object s = prop == null ? null : prop.GetValue(i, null);
                                        if (s != null)
                                        {
                                            Temp.Add(s.ToString() + "," + icout);
                                        }
                                    }
                                    else if (!(SelectedValuePath.Equals("")))
                                    {
                                        Type t = i.GetType();
                                        var prop = t.GetProperty(SelectedValuePath);
                                        object s = prop.GetValue(i, null);
                                        if (s != null)
                                        {
                                            Temp.Add(s.ToString() + "," + icout);
                                        }
                                    }
                                    else
                                        Temp.Add(i + "," + icout);

                                    icout++;
                                }
                            }
#if !SyncfusionFramework3_5
                            if (!this.IsTextSearchCaseSensitive)
                                _tempcoll = Temp.Cast<object>().Where(item => searchText != null && item.ToString().StartsWith(searchText, StringComparison.OrdinalIgnoreCase));
                            else
#endif
                                _tempcoll = Temp.Cast<object>().Where(item => searchText != null && item.ToString().StartsWith(searchText, StringComparison.Ordinal));
                            if (_tempcoll.Count() > 0)
                            {
                                charindex = 0;
                                string[] s2;
                                s2 = _tempcoll.ToList()[charindex].ToString().Split(',');
                                int index = int.Parse(s2[1]);
                                _keypressed = false;
                                this.SelectedItem = null;
                                SelectedIndex = index;
                            }
                        }
                    }
                }
            }
        }

#endif

#if WPF

        private void MainWindow_Deactivated(object sender, EventArgs e)
        {
            if (IsDropDownOpen)
                IsDropDownOpen = false;
        }

        private void MainWindow_LocationChanged(object sender, EventArgs e)
        {
            if (IsDropDownOpen)
                IsDropDownOpen = false;
        }

#endif

        private void popup_Closed(object sender, EventArgs e)
        {
            if (IsDropDownOpen)
                IsDropDownOpen = false;
        }

#if SILVERLIGHT
        void RootVisual_GotFocus(object sender, RoutedEventArgs e)
        {
            Object item = e.OriginalSource;
            if (item != null && !(item is ComboBoxItemAdv) && !(item is ComboBoxAdv) && IsDropDownOpen)
                IsDropDownOpen = false;
        }
        void RootVisual_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Object item = e.OriginalSource;
            if (item != null && !(item is ComboBoxItemAdv) && !(item is ComboBoxAdv) && IsDropDownOpen)
                IsDropDownOpen = false;
        }

#endif

        private void ComboBoxAdv_Unloaded(object sender, RoutedEventArgs e)
        {
#if WPF
            Window MainWindow = VisualUtils.FindAncestor(this, typeof(Window)) as Window;
            if (MainWindow != null)
            {
                MainWindow.PreviewMouseLeftButtonUp -= new MouseButtonEventHandler(MainWindow_PreviewMouseLeftButtonUp);
                MainWindow.LocationChanged -= new EventHandler(MainWindow_LocationChanged);
                MainWindow.Deactivated -= new EventHandler(MainWindow_Deactivated);
            }
#else
            Application.Current.RootVisual.MouseLeftButtonDown -= new MouseButtonEventHandler(RootVisual_MouseLeftButtonDown);
            Application.Current.RootVisual.GotFocus -= new RoutedEventHandler(RootVisual_GotFocus);
#endif
        }

#if WPF

        private void MainWindow_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Object item = e.OriginalSource;
            if (item != null && !(item is ComboBoxItemAdv) && !(item is ComboBoxAdv) && IsDropDownOpen && !(item is ScrollViewer) && !(item is ToggleButton) && !(AllowMultiSelect && (VisualUtils.FindAncestor(e.OriginalSource as Visual, typeof(ComboBoxItemAdv))) is ComboBoxItemAdv) && !ScrollBarClicked(e.OriginalSource as Visual))
                IsDropDownOpen = false;
        }

        private bool ScrollBarClicked(Visual OriginalSource)
        {
            bool IsClicked = false;
            if (OriginalSource != null)
            {
                ScrollViewer scrollViewer = this.Template.FindName("DropDownScrollViewer", this) as ScrollViewer;
                if (scrollViewer is Visual)
                {
                    IsClicked = (scrollViewer as Visual).IsAncestorOf(OriginalSource);
                }
            }
            return IsClicked;
        }

        private static void OnMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            ComboBoxAdv instance = (sender as ComboBoxAdv);
            if (instance != null)
            {
                if (!instance.IsKeyboardFocusWithin)
                {
                    instance.Focus();
                }
                e.Handled = true;
                if (e.OriginalSource == instance)
                {
                    instance.Close();
                }
            }
        }

        private static void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ComboBoxAdv instance = (sender as ComboBoxAdv);
            if (instance != null && instance.IsKeyboardFocusWithin)
            {
                if (!instance.IsDropDownOpen)
                {
                    int index;
                    if (e.Delta < 0)
                    {
                        if ((instance.SelectedIndex + 1) > instance.Items.Count)
                            index = instance.Items.Count - 1;
                        else
                            index = instance.SelectedIndex + 1;
                    }
                    else
                    {
                        if ((instance.SelectedIndex - 1) < 0)
                            index = 0;
                        else
                            index = instance.SelectedIndex - 1;
                    }
                    instance.SelectedIndex = index;
                }
                instance.IsGotKeyBoardFocus = true;
                e.Handled = true;
            }
            else
            {
                if (instance != null && instance.IsDropDownOpen)
                {
                    e.Handled = true;
                }
            }
        }

#endif

        internal void Close()
        {
            if (IsDropDownOpen)
            {
                ClearValue(IsDropDownOpenProperty);
                if (IsDropDownOpen)
                {
                    IsDropDownOpen = false;
                    popup.IsOpen = false;
                }
            }
        }

        private void ComboBoxAdv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedIndex >= 0 && SelectedItem != null)
            {
                if (!AllowMultiSelect)
                {
                    SelItemsInternal.Clear();
#if WPF
                    bool? isSynchronizedItem = IsSynchronizedWithCurrentItem;
                    if (isSynchronizedItem != null && isSynchronizedItem == false)
#endif
                        SelItemsInternal.Add(SelectedItem);
                }
            }
            UpdateSelectionBox();
        }

        internal void UpdateSelectMode()
        {
            if (selectedItems != null && selectedContent != null)
            {
                if (AllowMultiSelect)
                {
                    selectedItems.Visibility = System.Windows.Visibility.Visible;
                    selectedContent.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    selectedItems.Visibility = System.Windows.Visibility.Collapsed;
                    selectedContent.Visibility = System.Windows.Visibility.Visible;
                }
            }
            for (int i = 0; i < Items.Count; i++)
            {
                ComboBoxItemAdv item = ItemContainerGenerator.ContainerFromIndex(i) as ComboBoxItemAdv;
                if (item != null && item.CheckBox != null)
                {
                    if (AllowMultiSelect)
                    {
                        item.CheckBox.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        item.CheckBox.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        internal void UpdateSelectionBox()
        {
            if (SelectionBoxTemplate == null && ItemsSource != null)
            {
                SelectionBoxTemplate = ItemTemplate;
            }

            if (SelectedItem == null)
            {
                SelectionBoxTemplate = null;
            }

            if (SelectedItems != null)
            {
                internalChange = false;
                ObservableCollection<object> selItems = new ObservableCollection<object>(this.SelectedItems.Cast<object>());
                if (defaultText != null)
                {
                    if (AllowMultiSelect && SelectedItems != null)
                    {
                        if (selItems.Count == 0)
                        {
                            defaultText.Visibility = System.Windows.Visibility.Visible;
                        }
                        else
                        {
                            defaultText.Visibility = System.Windows.Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        if (SelectedItem == null)
                        {
                            defaultText.Visibility = System.Windows.Visibility.Visible;
                        }
                        else
                        {
                            defaultText.Visibility = System.Windows.Visibility.Collapsed;
                        }
                    }
                }

                if (AllowMultiSelect)
                {
                    if ((SelectedItem == null || SelectedIndex < 0) && SelItemsInternal.Count > 0)
                    {
                        if (SelectedItem != SelItemsInternal[0])
                            SelectedItem = SelItemsInternal[0];
                        if (SelectedItem is ComboBoxItemAdv)
                        {
                            if ((SelectedItem as ComboBoxItemAdv).Content != null)
                                SelectionBoxItem = (SelectedItem as ComboBoxItemAdv).Content;
                        }
                        else if(SelectionBoxItem == null || SelectionBoxItem != SelectedItem)
                            SelectionBoxItem = SelectedItem;
                    }
                    if (defaultText != null)
                    {
                        if (SelectedItem == null)
                        {
                            defaultText.Visibility = System.Windows.Visibility.Visible;
                        }
                        else
                        {
                            defaultText.Visibility = System.Windows.Visibility.Collapsed;
                        }
                    }
                    if (selectedItems != null && SelectedItems != null)
                    {
                        selectedItems.Items.Clear();
                        foreach (var item in SelItemsInternal)
                        {
                            ContentControl _content = new ContentControl();
                            _content.ContentTemplate = SelectionBoxTemplate;

                            if (item != null && (this.Items.Contains(item)))
                            {
                                if (item is ComboBoxItemAdv)
                                {
                                    if ((item as ComboBoxItemAdv).CheckBox != null)
                                    {
                                        if (!(item as ComboBoxItemAdv).CheckBox.IsChecked.Value)
                                        {
                                            (item as ComboBoxItemAdv).CheckBox.IsChecked = true;
                                        }
                                    }
                                    _content.Content = (item as ComboBoxItemAdv).Content;
                                }
                                else
                                {
                                    if (String.IsNullOrEmpty(DisplayMemberPath))
                                    {
                                        _content.Content = item;
                                    }
                                    else
                                    {
                                        Type type = item.GetType();
                                        PropertyInfo propertyInfo = type.GetProperty(DisplayMemberPath, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);

                                        if (propertyInfo == null && DisplayMemberPath.Contains('.'))
                                        {
                                            string dotDownDisplayMemberPathModel = string.Empty, dotDownDisplayMemberPath = string.Empty;
                                            PropertyInfo dotDownPropertyInfoModel = null, dotDownPropertyInfo = null;

                                            if (!String.IsNullOrEmpty(DisplayMemberPath.Split('.')[0]))
                                                dotDownDisplayMemberPathModel = DisplayMemberPath.Split('.')[0];
                                            if (!String.IsNullOrEmpty(DisplayMemberPath.Split('.')[1]))
                                                dotDownDisplayMemberPath = DisplayMemberPath.Split('.')[1];

                                            if (!String.IsNullOrEmpty(dotDownDisplayMemberPathModel))
                                            {
                                                dotDownPropertyInfoModel = type.GetProperty(dotDownDisplayMemberPathModel, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
                                                var dotDownModel = dotDownPropertyInfoModel.GetValue(item, null);

                                                if (dotDownModel != null)
                                                {
                                                    Type dotDowntype = dotDownModel.GetType();
                                                    if (!String.IsNullOrEmpty(dotDownDisplayMemberPath))
                                                    {
                                                        dotDownPropertyInfo = dotDowntype.GetProperty(dotDownDisplayMemberPath, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
                                                        _content.Content = dotDownPropertyInfo.GetValue(dotDownModel, null);
                                                    }
                                                }
                                            }
                                        }
                                        else if (null != propertyInfo)
                                        {
                                            _content.Content = propertyInfo.GetValue(item, null);
                                        }
                                        else
                                        {
                                            throw new InvalidOperationException("DisplayMemberPath has invalid property name");
                                        }
                                    }
                                }

                                selectedItems.Items.Add(_content);
                                if (SelItemsInternal.IndexOf(item) < selItems.Count - 1)
                                {
                                    selectedItems.Items.Add(SelectedValueDelimiter);
                                }
                            }
                        }
                    }
                }
                else
                {
#if SILVERLIGHT
                    if (selectedItems != null && SelectedItems != null && DisplayMemberPath != null)
                    {
                        selectedItems.Items.Clear();
                        foreach (var item in SelItemsInternal)
                        {
                            ContentControl _content = new ContentControl();
                            _content.ContentTemplate = SelectionBoxTemplate;

                            if (item != null && (this.Items.Contains(item)))
                            {
                                    if (DisplayMemberPath == String.Empty)
                                    {
                                        if(item is ComboBoxItemAdv)
                                        _content.Content = (item as ComboBoxItemAdv).Content;
                                        else
                                            _content.Content = item;
                                    }
                                    else
                                    {
                                        Type type = item.GetType();
                                        PropertyInfo propertyInfo = type.GetProperty(DisplayMemberPath, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
                                        if (null != propertyInfo)
                                        {
                                            _content.Content = propertyInfo.GetValue(item, null);
                                            SelectionBoxItem = _content.Content;
                                        }
                                        else
                                        {
                                            throw new InvalidOperationException("SelectedValuePath has invalid property name");
                                        }
                                    }

                                selectedItems.Items.Add(_content);
                                if (SelItemsInternal.IndexOf(item) < selItems.Count - 1)
                                {
                                    selectedItems.Items.Add(SelectedValueDelimiter);
                                }
                            }
                        }
                    }
#else
                    if (SelectedItem != null)
                    {
                        if (SelectedItem is ComboBoxItemAdv)
                        {
                            SelectionBoxItem = (SelectedItem as ComboBoxItemAdv).Content;
                        }
                        else if (SelectionBoxItem == null || SelectionBoxItem != SelectedItem)
                            SelectionBoxItem = SelectedItem;
                    }
                    if (selectedContent != null && SelectedItem != null)
                    {
                        if (SelectionBoxTemplate != null)
                        {
                            selectedContent.ContentTemplate = SelectionBoxTemplate;
                            if (DisplayMemberPath == String.Empty)
                            {
                                selectedContent.Content = SelectedItem;
                            }
                            else
                            {
                                Type type = SelectedItem.GetType();
                                PropertyInfo propertyInfo = type.GetProperty(DisplayMemberPath, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
                                if (null != propertyInfo)
                                {
                                    selectedContent.Content = propertyInfo.GetValue(SelectedItem, null);
                                }
                                else
                                {
                                    throw new InvalidOperationException("DisplayMemberPath has invalid property name");
                                }
                            }
                        }
                    }

#endif
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            UpdateSelectionBox();
        }

#if SILVERLIGHT
        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
#else

        protected override void OnPreviewKeyDown(KeyEventArgs e)
#endif
        {
            ComboBoxItemAdv item = null;
            int index = -1;
            if (e.OriginalSource is ComboBoxItemAdv)
            {
                item = e.OriginalSource as ComboBoxItemAdv;
                if (item == null)
                {
                    item = ItemContainerGenerator.ContainerFromItem(e.OriginalSource) as ComboBoxItemAdv;
                }
                index = ItemContainerGenerator.IndexFromContainer(item);
            }
            else if (e.OriginalSource is ComboBoxAdv && (e.OriginalSource as ComboBoxAdv).SelectedItem != null)
            {
                item = (e.OriginalSource as ComboBoxAdv).SelectedItem as ComboBoxItemAdv;

                if (!AllowMultiSelect)
                {
                    if (item == null)
                    {
                        item = GetItemContainer((e.OriginalSource as ComboBoxAdv).SelectedItem);
                    }
                    index = SelectedIndex;
                }
                else if (SelItemsInternal != null && SelItemsInternal.Count > 0 && Items.Count > 0)
                {
                    if (item == null)
                    {
                        item = GetItemContainer(SelItemsInternal[SelItemsInternal.Count - 1]);
                    }
                    index = Items.IndexOf(SelItemsInternal[SelItemsInternal.Count - 1]);
                }
            }
            if (e.Key == Key.Up)
            {
#if WPF
                if (Part_IsEdit != null && Part_IsEdit.IsVisible && !IsDropDownOpen && this.SelectedIndex != 0)
                {
                    Part_IsEdit.Text = "";
                    searchText = "";
                    if (SelectedItem != null)
                        index = this.Items.IndexOf(SelectedItem);
                }
#endif

                if (index == 0)
                    index = 0;
                else if (index < 0)
                {
                    index = -1;
                }
                else
                {
                    for (int i = index; i < this.Items.Count && i > 0; i--)
                    {
                        if (Items[i - 1] != null)
                        {
                            if (Items[i - 1] is ComboBoxItemAdv)
                            {
                                var previousItem = Items[i - 1] as ComboBoxItemAdv;
                                if (previousItem.IsEnabled)
                                {
                                    index = i - 1;
                                    break;
                                }
                            }
                            else
                            {
                                index = i - 1;
                                break;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
#if SILVERLIGHT
                    if(item!=null)
                        VisualStateManager.GoToState(item, "Normal", true);
#endif
                }
                if (index >= 0)
                {
                    if (!this.IsDropDownOpen && !AllowMultiSelect)
                    {
                        SelectedIndex = index;
#if SILVERLIGHT
                    ComboBoxItemAdv Previousitem = ItemContainerGenerator.ContainerFromIndex(index) as ComboBoxItemAdv;
                    VisualStateManager.GoToState(Previousitem, "MouseOver", true);
#endif
                        ClearSelection();
                        if (SelectedItem is ComboBoxItemAdv)
                            (SelectedItem as ComboBoxItemAdv).IsSelected = true;
                        else if (ItemContainerGenerator.ContainerFromItem(SelectedItem) != null)
                        {
                            var comboBoxItemAdv = ItemContainerGenerator.ContainerFromItem(SelectedItem) as ComboBoxItemAdv;
                            if (comboBoxItemAdv != null)
                                comboBoxItemAdv.IsSelected =
                                    true;
                        }
                    }
                    else
                    {
                        ComboBoxItemAdv Previousitem =
                            ItemContainerGenerator.ContainerFromIndex(index) as ComboBoxItemAdv;
                        if (Previousitem != null)
                            Previousitem.Focus();
                    }
                }
                e.Handled = true;
            }
            else if (e.Key == Key.Down)
            {
#if WPF
                if (Part_IsEdit != null && Part_IsEdit.IsVisible && !IsDropDownOpen)
                {
                    if (SelectedIndex == this.Items.Count - 1)
                        index = SelectedIndex;
                    else
                    {
                        Part_IsEdit.Text = "";
                        searchText = "";
                        if (SelectedItem != null)
                            if (Items != null)
                                index = Items.IndexOf(SelectedItem);
                    }
                }
#endif
                if (index == Items.Count - 1)
                    index = Items.Count - 1;
                else
                {
                    for (int i = index; i + 1 < this.Items.Count; i++)
                    {
                        if (Items[i + 1] != null)
                        {
                            if (Items[i + 1] is ComboBoxItemAdv)
                            {
                                var nextItem = Items[i + 1] as ComboBoxItemAdv;
                                if (nextItem.IsEnabled)
                                {
                                    index = i + 1;
                                    break;
                                }
                            }
                            else
                            {
                                index = i + 1;
                                break;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }

#if SILVERLIGHT
                    if(item!=null)
                        VisualStateManager.GoToState(item, "Normal", true);
#endif
                }

                if (index >= 0)
                {
                    if (!this.IsDropDownOpen && !AllowMultiSelect)
                    {
                        SelectedIndex = index;
#if SILVERLIGHT
                    ComboBoxItemAdv nextitem = ItemContainerGenerator.ContainerFromIndex(index) as ComboBoxItemAdv;
                    VisualStateManager.GoToState(nextitem, "MouseOver", true);
#endif
                        ClearSelection();
                        if (SelectedItem is ComboBoxItemAdv)
                            (SelectedItem as ComboBoxItemAdv).IsSelected = true;
                        else if (ItemContainerGenerator.ContainerFromItem(SelectedItem) != null)
                        {
                            var comboBoxItemAdv = ItemContainerGenerator.ContainerFromItem(SelectedItem) as ComboBoxItemAdv;
                            if (comboBoxItemAdv != null)
                                comboBoxItemAdv.IsSelected = true;
                        }
                    }
                    else
                    {
                        ComboBoxItemAdv nextitem = ItemContainerGenerator.ContainerFromIndex(index) as ComboBoxItemAdv;
                        if (nextitem != null)
                            nextitem.Focus();
                    }
                }
                e.Handled = true;
            }
            else if (e.Key == Key.Enter)
            {
                if (this.IsDropDownOpen && item != null)
                {
                    if (!AllowMultiSelect)
                    {
                        ClearSelection();
                    }
                    if (item != null)
                        item.IsSelected = true;
                    IsDropDownOpen = false;
                }
            }
            else if (e.Key == Key.Space)
            {
                if (this.IsDropDownOpen && AllowMultiSelect && e.OriginalSource is ComboBoxItemAdv)
                {
                    ComboBoxItemAdv Checkitem = ItemContainerGenerator.ContainerFromIndex(index) as ComboBoxItemAdv;
                    if (Checkitem != null && Checkitem.CheckBox != null)
                        Checkitem.CheckBox.IsChecked = !Checkitem.CheckBox.IsChecked;
                }
            }
#if WPF
            else if (e.Key == Key.F4)
            {
                if ((e.KeyboardDevice.Modifiers & ModifierKeys.Alt) == 0)
                {
                    IsDropDownOpen = !IsDropDownOpen;
                    e.Handled = true;
                }
            }
#endif
            else if (e.Key == Key.Escape)
            {
                if (IsDropDownOpen)
                {
                    IsDropDownOpen = false;
                    e.Handled = true;
                }
            }
            else if (Char.IsLetter((e.Key).ToString().ToCharArray()[0]))
            {
#if WPF
                if (!IsDropDownOpen && Part_IsEdit != null && !Part_IsEdit.IsVisible && IsTextSearchEnabled)
                {
                    if (timer != null)
                        timer.Stop();
                    _keypressed = true;
                    TextChanged(e);
                }
#endif
            }

#if WPF
            base.OnPreviewKeyDown(e);
#else
            base.OnKeyDown(e);
#endif
        }

#if WPF

        private void timer_Tick(object sender, EventArgs e)
        {
            if (sender is DispatcherTimer)
            {
                (sender as DispatcherTimer).Stop();
                searchText = "";
            }
        }

        private void TextChanged(KeyEventArgs e)
        {
            if (IsTextSearchEnabled)
            {
                char temp;
                Char.TryParse((e.Key).ToString(), out temp);
                List<object> Temp = new List<object>();
                bool StringFlag, MemberPathFlag;
                object TestObj = Items[0];
                StringFlag = TestObj is string;
                MemberPathFlag = DisplayMemberPath.Equals(String.Empty);

                newTempChar = temp;
                searchText += newTempChar;
                if (_keypressed)
                {
                    if (Items != null)
                    {
                        if (ItemsSource == null && Items.Count > 0)
                        {
#if !SyncfusionFramework3_5
                            if (this.IsTextSearchCaseSensitive)
                                searchCollection = from item in this.Items.OfType<ComboBoxItemAdv>()
                                                   where (item as ComboBoxItemAdv).Content.ToString().StartsWith(searchText, StringComparison.Ordinal)
                                                   select item;
                            else
#endif
                                searchCollection = from item in this.Items.OfType<ComboBoxItemAdv>()
                                                   where (item as ComboBoxItemAdv).Content.ToString().ToUpper().StartsWith(searchText, StringComparison.OrdinalIgnoreCase)
                                                   select item;
                            if (searchCollection.Count() > 0)
                            {
                                this.SelectedIndex = this.Items.IndexOf(searchCollection.ToList()[0]);
                            }
                            else if (oldTempChar == newTempChar)
                            {
                                searchText = oldTempChar.ToString();
                                searchCollection = from item in this.Items.OfType<ComboBoxItemAdv>()
                                                   where (item as ComboBoxItemAdv).Content.ToString().StartsWith(searchText)
                                                   select item;

                                if (searchCollection.Count() > 0)
                                {
                                    if (charindex + 1 >= searchCollection.Count())
                                    {
                                        charindex = -1;
                                    }
                                    SelectedIndex = this.Items.IndexOf(searchCollection.ToList()[++charindex]);
                                }
                            }
                        }
                        else if (ItemsSource != null)
                        {
                            int icout = 0;
                            if (StringFlag)
                            {
                                foreach (object i in Items)
                                {
                                    if (i != null)
                                    {
                                        Temp.Add((String)i + "," + icout);
                                    }
                                }
                            }
                            else
                            {
                                foreach (object i in Items)
                                {
                                    if (!MemberPathFlag)
                                    {
                                        Type t = i.GetType();
                                        var prop = t.GetProperty(DisplayMemberPath);
                                        object s = prop == null ? null : prop.GetValue(i, null);
                                        if (s != null)
                                        {
                                            Temp.Add(s.ToString() + "," + icout);
                                        }
                                    }
                                    else if (!(SelectedValuePath.Equals("")))
                                    {
                                        Type t = i.GetType();
                                        var prop = t.GetProperty(SelectedValuePath);
                                        object s = prop.GetValue(i, null);
                                        if (s != null)
                                        {
                                            Temp.Add(s.ToString() + "," + icout);
                                        }
                                    }
                                    else
                                        Temp.Add(i + "," + icout);

                                    icout++;
                                }
                            }
#if !SyncfusionFramework3_5
                            if (this.IsTextSearchCaseSensitive)
                                _tempcoll = Temp.Cast<object>().Where(item => searchText != null && item.ToString().StartsWith(searchText, StringComparison.Ordinal));
                            else
#endif
                                _tempcoll = Temp.Cast<object>().Where(item => searchText != null && item.ToString().StartsWith(searchText, StringComparison.OrdinalIgnoreCase));

                            if (_tempcoll.Count() > 0)
                            {
                                charindex = 0;
                                string[] s2;
                                s2 = _tempcoll.ToList()[charindex].ToString().Split(',');
                                int index = int.Parse(s2[1]);
                                SelectedIndex = index;
                            }

                            else if (oldTempChar == newTempChar)
                            {
                                searchText = oldTempChar.ToString();
                                _tempcoll =
                                    Items.Cast<Object>()
                                        .Where(item => searchText != null && item.ToString().StartsWith(searchText));
                                if (_tempcoll.Count() > 0)
                                {
                                    if (charindex + 1 >= _tempcoll.Count())
                                    {
                                        charindex = -1;
                                    }
                                    SelectedIndex = this.Items.IndexOf(_tempcoll.ToList()[++charindex]);
                                }
                            }
                        }
                    }
                }

                oldTempChar = newTempChar;
                timer.Start();
                _keypressed = false;
            }
        }

#endif

        private void ClearSelection()
        {
            foreach (object comboItem in Items)
            {
                if (comboItem is ComboBoxItemAdv && (comboItem as ComboBoxItemAdv).IsHighlighted)
                {
                    (comboItem as ComboBoxItemAdv).IsHighlighted = false;
                }
                else if (GetItemContainer(comboItem) is ComboBoxItemAdv && GetItemContainer(comboItem).IsHighlighted)
                {
                    GetItemContainer(comboItem).IsHighlighted = false;
                }
            }
        }

        private static void OnAllowMultiSelectChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            ComboBoxAdv instance = sender as ComboBoxAdv;
            if (instance != null && instance.SelItemsInternal != null)
            {
                if (!instance.AllowMultiSelect)
                {
                    if (instance.SelectedItem == null && instance.SelectedIndex < 0 && instance.SelItemsInternal.Count > 0)
                    {
                        instance.SelectedItem = instance.SelItemsInternal[0];
                        if (instance.SelectedItem is ComboBoxItemAdv)
                        {
                            instance.SelectionBoxItem = (instance.SelectedItem as ComboBoxItemAdv).Content;
                        }
                        else
                            instance.SelectionBoxItem = instance.SelectedItem;
                    }
                    if (instance.SelectedItem != null && instance.SelectedIndex < 0 && instance.SelItemsInternal.Count <= 0)
                    {
                        instance.SelectedItem = null;
                    }
                    instance.SelItemsInternal.Clear();
                    foreach (object obj in instance.Items)
                    {
                        ComboBoxItemAdv item = instance.ItemContainerGenerator.ContainerFromItem(obj) as ComboBoxItemAdv;
                        if (item != null && instance.SelectedItem != null && item.Content != null)
                        {
                            if (!item.Content.Equals(instance.SelectedItem))
                                item.IsSelected = false;
                        }
                    }
                    if (instance.SelectedItem is ComboBoxItemAdv)
                    {
                        instance.SelectionBoxItem = (instance.SelectedItem as ComboBoxItemAdv).Content;
                    }
                    else
                        instance.SelectionBoxItem = instance.SelectedItem;
                }
                if (instance.SelectedItem != null && instance.SelItemsInternal.Count <= 0)
                {
                    instance.ExternalChange = true;
                    instance.SelItemsInternal.Add(instance.SelectedItem);
                    instance.ExternalChange = false;
                }
                instance.IsDropDownOpen = false;
                instance.UpdateSelectionBox();

                instance.UpdateSelectMode();
            }
        }

        private void UpdateSelectionOnDropDownOpen(ComboBoxAdv instance)
        {
            ComboBoxItemAdv comboBoxItem;
            if (instance != null && IsDropDownOpen)
            {
                instance.ClearSelection();
                if (instance.popup != null && !instance.AllowMultiSelect && instance.SelectedIndex >= 0)
                {
                    DropDownScrollBar.ScrollToVerticalOffset(instance.SelectedIndex);
                    if (instance.SelectedItem != null)
                    {
                        comboBoxItem = instance.SelectedItem as ComboBoxItemAdv;
                        if (comboBoxItem == null)
                        {
                            comboBoxItem = instance.GetItemContainer(instance.SelectedItem);
                        }
                        if (comboBoxItem != null && comboBoxItem is ComboBoxItemAdv)
                        {
#if WPF
                            comboBoxItem.IsHighlighted = true;
#else
                            VisualStateManager.GoToState(comboBoxItem, "MouseOver", true);
#endif
                            comboBoxItem.Focus();
                        }
                    }
                }
                else if (instance.SelItemsInternal.Count > 0)
                {
                    int index = instance.SelItemsInternal.Count - 1;

                    DropDownScrollBar.ScrollToVerticalOffset(instance.Items.IndexOf(instance.SelItemsInternal[index]));
                    if (instance.SelItemsInternal != null)
                    {
                        comboBoxItem = instance.SelItemsInternal[index] as ComboBoxItemAdv;
                        if (comboBoxItem == null)
                        {
                            comboBoxItem = instance.GetItemContainer(instance.SelItemsInternal[index]);
                        }
                        if (comboBoxItem != null && comboBoxItem is ComboBoxItemAdv)
                        {
#if WPF
                            comboBoxItem.IsHighlighted = true;
#else
                            VisualStateManager.GoToState(comboBoxItem, "MouseOver", true);
#endif
                            comboBoxItem.Focus();
                        }
                    }
                }
            }
        }

        private static void OnIsDropDownOpenChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            ComboBoxAdv instance = sender as ComboBoxAdv;
            if (instance != null)
            {
                instance.UpdateSelectionOnDropDownOpen(instance);

#if SILVERLIGHT
               if (instance.popup != null && instance.ActualWidth != 0)
               {
                   instance.popup.MinWidth = instance.ActualWidth;
                   instance.popupBorder.MinWidth = instance.ActualWidth;
               }
#endif
                if (instance.IsDropDownOpen && instance.DropDownOpened != null)
                    instance.DropDownOpened(sender, new EventArgs());
                else if (instance.DropDownClosed != null && instance.IsDropDownOpen == false)
                    instance.DropDownClosed(sender, new EventArgs());
            }

#if WPF
            bool newValue = (bool)args.NewValue;

            if (!newValue)
            {
                if (instance != null && instance.IsKeyboardFocusWithin)
                {
                    if (instance.IsEditable)
                    {
                        {
                            instance.Focus();
                        }
                    }
                    else
                    {
                        instance.Focus();
                    }
                }

                if (instance != null && instance.IsDropDownOpen)
                {
                    instance.DropDownClosed(sender, new EventArgs());
                }
            }
#endif
        }

        private static void OnSelectedValueDelimiterChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            ComboBoxAdv instance = sender as ComboBoxAdv;
            if (instance != null)
            {
                instance.UpdateSelectionBox();

                instance.UpdateSelectMode();
            }
        }

        private static void OnSelectionBoxTemplateChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            ComboBoxAdv instance = sender as ComboBoxAdv;
            if (instance.DisplayMemberPath != null && instance.DisplayMemberPath != "" && instance.SelectionBoxTemplate != null)
            {
#if WPF
                throw new System.Windows.Markup.XamlParseException("Cannot set both DisplayMemberPath and SelectionBoxTemplate");
#else

                //throw new System.Windows.Markup.XamlParseException();
#endif
            }
        }

        internal void NotifyComboBoxItemAdvEnter(ComboBoxItemAdv item, bool state)
        {
            if (IsDropDownOpen)
            {
                ComboBoxItemAdv selecteditem;
                if (!AllowMultiSelect)
                {
                    selecteditem = GetItemContainer(SelectedItem);
#if WPF
                    if (selecteditem != null && !selecteditem.Equals(item) && selecteditem.IsHighlighted && !selecteditem.IsMouseOver)
                    {
#else
                    if (selecteditem != null && !selecteditem.Equals(item) && selecteditem.IsHighlighted)
                    {
#endif
                        selecteditem.IsHighlighted = false;
                    }
                }
                else
                {
                    int index = SelItemsInternal.Count - 1;
                    if (index >= 0)
                    {
                        selecteditem = GetItemContainer(SelItemsInternal[SelItemsInternal.Count - 1]);
#if WPF
                        if (selecteditem != null && SelItemsInternal != null && SelItemsInternal.Count > 0 && selecteditem.IsHighlighted && !selecteditem.IsMouseOver)
                        {
#else
                        if (selecteditem != null && SelItemsInternal != null && SelItemsInternal.Count > 0 && selecteditem.IsHighlighted)
                        {
#endif
                            selecteditem.IsHighlighted = false;
                        }
                    }
                }
                item.IsHighlighted = state;
#if WPF
                if (!IsEditable && !item.IsKeyboardFocusWithin && state)
#endif
                    item.Focus();
            }
        }

#if WPF

        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsEditable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register("IsEditable", typeof(bool), typeof(ComboBoxAdv), new UIPropertyMetadata(false));

#endif

        /// <summary>
        ///
        /// </summary>
        public Double MaxDropDownHeight
        {
            get { return (Double)GetValue(MaxDropDownHeightProperty); }
            set { SetValue(MaxDropDownHeightProperty, value); }
        }

#if WPF

        // Using a DependencyProperty as the backing store for MaxDropDownHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxDropDownHeightProperty =
            DependencyProperty.Register("MaxDropDownHeight", typeof(Double), typeof(ComboBoxAdv), new PropertyMetadata((SystemParameters.MaximizedPrimaryScreenHeight / 3)));

#else
        /// <summary>
        /// Using a DependencyProperty as the backing store for MaxDropDownHeight.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MaxDropDownHeightProperty =
            DependencyProperty.Register("MaxDropDownHeight", typeof(Double), typeof(ComboBoxAdv), new PropertyMetadata(150d));
#endif

#if SILVERLIGHT

        #region SelectedValue

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty SelectedValueProperty =
              DependencyProperty.Register("SelectedValue",
                      typeof(object),
                      typeof(ComboBoxAdv), null);

        /// <summary>
        ///
        /// </summary>
        public object SelectedValue
        {
            get { return GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }

        #endregion SelectedValue

        /// <summary>
        ///
        /// </summary>
          public Object SelectedItem
        {
            get { return (Object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(Object), typeof(ComboBoxAdv), new PropertyMetadata(null,new PropertyChangedCallback(OnSelectedItemChanged)));

         internal static void OnSelectedItemChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((ComboBoxAdv)obj != null)
            {
                ((ComboBoxAdv)obj).OnSelectedItemChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
         protected void OnSelectedItemChanged(DependencyPropertyChangedEventArgs args)
         {
             List<object> newValue = new List<object>();
             List<object> oldValue = new List<object>();
             newValue.Clear();
             oldValue.Clear();
             newValue.Add(args.NewValue);
             if(!AllowMultiSelect)
             oldValue.Add(args.OldValue);

             SelectionChangedEvent = new SelectionChangedEventArgs(oldValue, newValue);
             if (this.SelectedItem != null && this.SelItemsInternal.Count <= 0)
             {
                 if (this.SelectedItem is ComboBoxItemAdv)
                     this.SelectionBoxItem = (this.SelectedItem as ComboBoxItemAdv).Content;
                 else
                     this.SelectionBoxItem = this.SelectedItem;
                 this.SelItemsInternal.Add(this.SelectedItem);
                 ComboBoxItemAdv item = this.ItemContainerGenerator.ContainerFromItem(this.SelectedItem) as ComboBoxItemAdv;
                 if (item == null)
                 {
                     this.UpdateLayout();
                 }
                 item = this.ItemContainerGenerator.ContainerFromItem(this.SelectedItem) as ComboBoxItemAdv;
                 if (item != null)
                 {
                     int index = this.ItemContainerGenerator.IndexFromContainer(item);
                     this.SelectedIndex = index;
                 }
             }
             if(this.SelectedItem == null)
             {
                 this.SelectedIndex = -1;
                 this.SelectionBoxItem = null;
             }

#if SILVERLIGHT

             if (this.SelectedItem != null
                 && !string.IsNullOrEmpty(SelectedValuePath))
             {
                 Type type = SelectedItem.GetType();
                 PropertyInfo propertyInfo = type.GetProperty(SelectedValuePath, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
                 if (null != propertyInfo)
                 {
                     SelectedValue = propertyInfo.GetValue(SelectedItem, null);
                 }
             }

#endif

             if (this.SelectionChanged != null)
                 this.SelectionChanged(this, SelectionChangedEvent);
         }

        /// <summary>
        ///
        /// </summary>
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedIndex.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(ComboBoxAdv), new PropertyMetadata(-1, new PropertyChangedCallback(OnSelectedIndexChanged)));

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnSelectedIndexChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((ComboBoxAdv)obj != null)
            {
                ((ComboBoxAdv)obj).OnSelectedIndexChanged(args);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        protected void OnSelectedIndexChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.SelectedIndex < 0)
            {
                if(!this.internalSelect)
                this.SelItemsInternal.Clear();
                this.SelectedItem = null;
            }
            if (this.SelectedIndex >= 0)
            {
                ComboBoxItemAdv item = this.ItemContainerGenerator.ContainerFromIndex(this.SelectedIndex) as ComboBoxItemAdv;
                if (item != null)
                {
                    if (item.DataContext != null && this.ItemsSource != null)
                        this.SelectedItem = item.DataContext;
                    else
                        this.SelectedItem = item;
                }
                if (SelectedItem is ComboBoxItemAdv)
                {
                    SelectionBoxItem = (SelectedItem as ComboBoxItemAdv).Content;
                }
                else
                SelectionBoxItem = SelectedItem;
            }
        }
        /// <summary>
        ///
        /// </summary>
        public string SelectedValuePath
        {
            get { return (string)GetValue(SelectedValuePathProperty); }
            set { SetValue(SelectedValuePathProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedValuePath.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedValuePathProperty =
            DependencyProperty.Register("SelectedValuePath", typeof(string), typeof(ComboBoxAdv), new PropertyMetadata(string.Empty));

#endif

        /// <summary>
        ///
        /// </summary>
        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsDropDownOpen.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(ComboBoxAdv), new PropertyMetadata(false, new PropertyChangedCallback(OnIsDropDownOpenChanged)));

        /// <summary>
        ///
        /// </summary>
        public DataTemplate SelectionBoxItemTemplate
        {
            get { return (DataTemplate)GetValue(SelectionBoxItemTemplateProperty); }
            internal set { SetValue(SelectionBoxItemTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectionBoxItemTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectionBoxItemTemplateProperty =
            DependencyProperty.Register("SelectionBoxItemTemplate", typeof(DataTemplate), typeof(ComboBoxAdv), new PropertyMetadata(null));

        /// <summary>
        ///
        /// </summary>
        public object SelectionBoxItem
        {
            get { return (object)GetValue(SelectionBoxItemProperty); }
            internal set { SetValue(SelectionBoxItemProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectionBoxItem.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectionBoxItemProperty =
            DependencyProperty.Register("SelectionBoxItem", typeof(object), typeof(ComboBoxAdv), new PropertyMetadata(null));

        /// <summary>
        ///
        /// </summary>
        public String SelectionBoxItemStringFormat
        {
            get { return (String)GetValue(SelectionBoxItemStringFormatProperty); }
            internal set { SetValue(SelectionBoxItemStringFormatProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectionBoxItemStringFormat.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectionBoxItemStringFormatProperty =
            DependencyProperty.Register("SelectionBoxItemStringFormat", typeof(String), typeof(ComboBoxAdv), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets a value indicating whether [allow multi select].
        /// </summary>
        /// <value><c>true</c> if [allow multi select]; otherwise, <c>false</c>.</value>
        public bool AllowMultiSelect
        {
            get { return (bool)GetValue(AllowMultiSelectProperty); }
            set { SetValue(AllowMultiSelectProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AllowMultiSelect.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AllowMultiSelectProperty =
            DependencyProperty.Register("AllowMultiSelect", typeof(bool), typeof(ComboBoxAdv), new PropertyMetadata(false, new PropertyChangedCallback(OnAllowMultiSelectChanged)));

        /// <summary>
        ///
        /// </summary>
        public IEnumerable SelectedItems
        {
            get { return (IEnumerable)GetValue(SelectedItemsProperty); }
            set
            {
                if (value == null)
                    value = new ObservableCollection<object>();
                SetValue(SelectedItemsProperty, value);
            }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelItems.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(IEnumerable), typeof(ComboBoxAdv), new PropertyMetadata(null, new PropertyChangedCallback(OnSelectedItemsChanged)));

        private void coll_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (AllowMultiSelect)
            {
                ObservableCollection<ComboBoxItemAdv> selComboItems = new ObservableCollection<ComboBoxItemAdv>();
                foreach (object item in SelectedItems)
                {
                    ComboBoxItemAdv cItem = this.ItemContainerGenerator.ContainerFromItem(item) as ComboBoxItemAdv;
                    if (cItem != null)
                        selComboItems.Add(cItem);
                }
                if (SelItemsInternal.Count <= 0)
                {
                    this.internalSelect = true;
                    SelectedItem = null;
                    this.internalSelect = false;
                }
                if (selComboItems.Count > 0)
                {
                    index = this.ItemContainerGenerator.IndexFromContainer(selComboItems[0] as ComboBoxItemAdv);
                    if (SelectedIndex == index)
                    {
#if WPF
                        internalChange = true;
#else
                        if (this.SelectionChanged != null && !this.ExternalChange)
                            this.SelectionChanged(this, SelectionChangedEvent);
#endif
                    }
                    else
                    {
                        SelectedIndex = index;
                        internalChange = false;
#if SILVERLIGHT
                        if (this.SelectionChanged != null && !this.ExternalChange)
                            this.SelectionChanged(this, SelectionChangedEvent);
#endif
                    }
                    if (selComboItems[0].DataContext != null && this.ItemsSource != null)
                    {
                        SelectedItem = selComboItems[0].DataContext;
                    }
                    else
                    {
                        SelectedItem = selComboItems[0];
                    }
                    if (SelectedItem is ComboBoxItemAdv)
                    {
                        SelectionBoxItem = (SelectedItem as ComboBoxItemAdv).Content;
                    }
                    else
                        SelectionBoxItem = SelectedItem;
                }
                else if (this.SelItemsInternal.Count <= 0)
                {
                    internalChange = false;
                    this.SelectedIndex = -1;
                }
                else if (!internalChange && SelectionChangedEvent != null)
                {
#if WPF

                    base.OnSelectionChanged(SelectionChangedEvent);
#else
                    this.SelectionChanged(this,SelectionChangedEvent);
#endif
                    internalChange = true;
                }
#if WPF
                if (internalChange && !ExternalChange && SelectionChangedEvent != null)
                {
                    base.OnSelectionChanged(SelectionChangedEvent);
                }
#endif
                removeFlag = false;
                if (e.OldItems != null)
                {
                    oldItem = e.OldItems[0];
                    if (this.Items.Contains(e.OldItems[0]))
                    {
                        ComboBoxItemAdv boxItem = this.ItemContainerGenerator.ContainerFromItem(e.OldItems[0]) as ComboBoxItemAdv;
                        if (boxItem != null && boxItem.CheckBox != null)
                        {
                            this.internalSelect = true;
                            boxItem.CheckBox.IsChecked = false;
                            this.internalSelect = false;
                        }
                    }
                }
                else if (e.NewItems != null && this.Items.Contains(e.NewItems[0]))
                {
                    newItem = e.NewItems[0];
                    foreach (var selItem in SelItemsInternal)
                    {
                        foreach (var item in this.Items)
                        {
                            if (item != null && item.Equals(selItem) && newItem.Equals(item))
                            {
                                if (itemcount >= 1)
                                {
                                    ComboBoxItemAdv boxItem = this.ItemContainerGenerator.ContainerFromItem(newItem) as ComboBoxItemAdv;
                                    if ((boxItem != null && boxItem.IsSelected) || boxItem == null)
                                        removeFlag = true;
                                }
                                this.itemcount++;
                            }
                        }
                    }
                    if (removeFlag)
                        if (SelItemsInternal.Count > 0)
                        {
                            ExternalChange = true;
                            SelItemsInternal.Remove(e.NewItems[0]);
                            ExternalChange = false;
                        }
                    SelectItems();
                    itemcount = 0;
                }
                else
                {
                    if (SelItemsInternal.Count > 0)
                        SelItemsInternal.Remove(e.NewItems[0]);
                    SelectItems();
                }

                UpdateSelectionBox();
                UpdateSelectMode();
            }
        }

        internal void SelectItems()
        {
            foreach (var item in SelItemsInternal)
            {
                ComboBoxItemAdv boxItem = this.ItemContainerGenerator.ContainerFromItem(item) as ComboBoxItemAdv;
                if (boxItem != null && boxItem.CheckBox != null && boxItem.CheckBox.IsChecked == false)
                {
                    this.internalSelect = true;
                    boxItem.CheckBox.IsChecked = true;
                    this.internalSelect = false;
                }
            }

            if (this.SelItemsInternal.Count <= 0)
            {
                foreach (var item in this.Items)
                {
                    ComboBoxItemAdv boxItem = this.ItemContainerGenerator.ContainerFromItem(item) as ComboBoxItemAdv;
                    if (boxItem != null && boxItem.CheckBox != null)
                    {
                        this.internalSelect = true;
                        boxItem.CheckBox.IsChecked = false;
                        this.internalSelect = false;
                    }
                }
            }
        }

        private static void OnSelectedItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ComboBoxAdv cboxAdv = sender as ComboBoxAdv;
            cboxAdv.OnSelectedItemsChanged(args);
        }

        internal IList SelItemsInternal { get; set; }

        internal void OnSelectedItemsChanged(DependencyPropertyChangedEventArgs args)
        {
            var coll = (INotifyCollectionChanged)args.NewValue;
            if (coll != null)
            {
                coll.CollectionChanged -= new NotifyCollectionChangedEventHandler(coll_CollectionChanged);
                coll.CollectionChanged += new NotifyCollectionChangedEventHandler(coll_CollectionChanged);
            }
            SelItemsInternal = (IList)coll;
            if (AllowMultiSelect && SelItemsInternal != null)
            {
                foreach (var item in SelItemsInternal)
                {
                    if (item != null && this.Items.Contains(item))
                    {
                        UpdateSelectionBox();
                        SelectItems();
                    }
                    else
                    {
                        newItem = item;
                        SelectItems();
                    }
                }
            }
            if (SelItemsInternal != null && SelItemsInternal.Count <= 0)
                SelectItems();

            if (SelItemsInternal == null)
            {
                SelectedItems = null;
                SelectItems();
                UpdateSelectionBox();
            }
        }

        /// <summary>
        ///
        /// </summary>
        public string SelectedValueDelimiter
        {
            get { return (string)GetValue(SelectedValueDelimiterProperty); }
            set { SetValue(SelectedValueDelimiterProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedValueDelimiter.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedValueDelimiterProperty =
            DependencyProperty.Register("SelectedValueDelimiter", typeof(string), typeof(ComboBoxAdv), new PropertyMetadata(" - ", new PropertyChangedCallback(OnSelectedValueDelimiterChanged)));

        /// <summary>
        ///
        /// </summary>
        public DataTemplate SelectionBoxTemplate
        {
            get { return (DataTemplate)GetValue(SelectionBoxTemplateProperty); }
            set { SetValue(SelectionBoxTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectionBoxTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectionBoxTemplateProperty =
            DependencyProperty.Register("SelectionBoxTemplate", typeof(DataTemplate), typeof(ComboBoxAdv), new PropertyMetadata(null, new PropertyChangedCallback(OnSelectionBoxTemplateChanged)));

        /// <summary>
        ///
        /// </summary>
        public string DefaultText
        {
            get { return (string)GetValue(DefaultTextProperty); }
            set { SetValue(DefaultTextProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DefaultText.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DefaultTextProperty =
            DependencyProperty.Register("DefaultText", typeof(string), typeof(ComboBoxAdv), new PropertyMetadata(String.Empty));
    }
}