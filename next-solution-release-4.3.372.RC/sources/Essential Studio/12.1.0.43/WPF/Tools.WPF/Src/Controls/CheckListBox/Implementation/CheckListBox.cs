// <copyright file="CheckListBox.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// CheckListBox control implements a classic list box with check list box items.
    /// The control displays items with check box to select multiple items.
    /// </summary>
    /// <example>
    /// <code lang="XAML">
    /// <Window x:Class="Checklistbox.Window1"
    ///     xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    ///     xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    ///     Title="Window1" Height="300" Width="300"
    /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf">
    ///     <Grid>
    ///         <syncfusion:CheckListBox Name="myCheckListBox" />
    ///     </Grid>
    /// </Window>
    /// </code>
    /// <code lang="C#">
    /// using System;
    /// using System.Collections.Generic;
    /// using System.Linq;
    /// using System.Text;
    /// using System.Windows;
    /// using System.Windows.Controls;
    /// using System.Windows.Data;
    /// using System.Windows.Documents;
    /// using System.Windows.Input;
    /// using System.Windows.Media;
    /// using System.Windows.Media.Imaging;
    /// using System.Windows.Navigation;
    /// using System.Windows.Shapes;
    /// using Syncfusion.Windows.Tools.Controls;
    /// namespace checklistbox
    /// {
    ///     /// <summary>
    ///     /// Interaction logic for Window1.xaml
    ///     /// </summary>
    ///     public partial class Window1 : Window
    ///     {
    ///         public Window1()
    ///         {
    ///             InitializeComponent();
    ///             CheckListBox checklistbox = new CheckListBox();
    ///             this.Content = checklistbox;
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(true)]
#endif

    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
     Type = typeof(CheckListBox), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/CheckListBox/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(CheckListBox), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/CheckListBox/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(CheckListBox), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/CheckListBox/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
    Type = typeof(CheckListBox), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/CheckListBox/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(CheckListBox), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/CheckListBox/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(CheckListBox), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/CheckListBox/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
    Type = typeof(CheckListBox), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/CheckListBox/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(CheckListBox), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/CheckListBox/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.SyncOrange,
    Type = typeof(CheckListBox), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/CheckListBox/Themes/SyncOrangeStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyRed,
    Type = typeof(CheckListBox), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/CheckListBox/Themes/ShinyRedStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyBlue,
    Type = typeof(CheckListBox), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/CheckListBox/Themes/ShinyBlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(CheckListBox), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/CheckListBox/Themes/Generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
    Type = typeof(CheckListBox), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/CheckListBox/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
   Type = typeof(CheckListBox), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/CheckListBox/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
   Type = typeof(CheckListBox), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/CheckListBox/Themes/TransparentStyle.xaml")]
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(CheckListBoxItem)), Localizability(LocalizationCategory.ListBox)]
    public class CheckListBox : MultiSelector
    {
        #region class fields

        /// <summary>
        /// Presents name of fack item adorner.
        /// </summary>
        private FackItemsAdorner fackItemsAdorner;

        /// <summary>
        /// Presents name of mouse starting point.
        /// </summary>
        private Point startingMousePointAtDragStart;

        /// <summary>
        /// Presents name of application root element.
        /// </summary>
        private Window applicationRootElement = null;

        /// <summary>
        /// To get the selected items in the CheckListBox.
        /// </summary>
        //public System.Collections.IList m_selectedItems = new List<object>();

        //public int index = 0;

        //string[] contnt = new string[200];
        /// <summary>
        /// Presents the root page.
        /// </summary>
        private Page rootPage = null;

        #endregion class fields

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="CheckListBox"/> class.
        /// </summary>
        static CheckListBox()
        {
            //EnvironmentTest.ValidateLicense(typeof(CheckListBox));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CheckListBox), new FrameworkPropertyMetadata(typeof(CheckListBox)));
            ItemsPanelTemplate defaultValue = new ItemsPanelTemplate(new FrameworkElementFactory(typeof(VirtualizingStackPanel)));
            defaultValue.Seal();
            ItemsControl.ItemsPanelProperty.OverrideMetadata(typeof(CheckListBox), new FrameworkPropertyMetadata(defaultValue));
            EventManager.RegisterClassHandler(typeof(CheckListBox), Mouse.MouseUpEvent, new MouseButtonEventHandler(CheckListBox.OnMouseButtonUp), true);
            EventManager.RegisterClassHandler(typeof(CheckListBox), Mouse.MouseDownEvent, new MouseButtonEventHandler(CheckListBox.OnMouseButtonDown), true);
            EventManager.RegisterClassHandler(typeof(CheckListBox), Keyboard.GotKeyboardFocusEvent, new KeyboardFocusChangedEventHandler(CheckListBox.OnGotKeyboardFocus));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckListBox"/> class.
        /// </summary>
        public CheckListBox()
        {
            if (EnvironmentTestTools.IsSecurityGranted)
            {
                EnvironmentTestTools.StartValidateLicense(typeof(CheckListBox));
            }
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                if (Application.Current != null && !BindingUtils.GetEnableBindingErrors(Application.Current.MainWindow))
                    System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
            }
        }

        #endregion Initialization

        #region Dependency Property Setter and Getter

        /// <summary>
        /// Gets or sets the last action item.
        /// </summary>
        /// <value>The last action item.</value>
        internal object LastActionItem
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the fixed item.
        /// </summary>
        /// <value>The fixed item.</value>
        internal object FixedItem
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is check on first click.
        /// </summary>
        /// <value>
        ///  true if this instance is check on first click; otherwise, false.
        /// </value>
        public bool IsCheckOnFirstClick
        {
            get
            {
                return (bool)GetValue(IsCheckOnFirstClickProperty);
            }

            set
            {
                SetValue(IsCheckOnFirstClickProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is drag drop enabled.
        /// </summary>
        /// <value>
        /// true if this instance is drag drop enabled; otherwise, false.
        /// </value>
        public bool IsDragDropEnabled
        {
            get
            {
                return (bool)GetValue(IsDragDropEnabledProperty);
            }

            set
            {
                SetValue(IsDragDropEnabledProperty, value);
            }
        }

        /// <summary>
        /// Sets the check box alignment.
        /// </summary>
        /// <param name="obj">The obj of CheckBoxAlignment.</param>
        /// <param name="value">The value of CheckBoxAlignment.</param>
        public static void SetCheckBoxAlignment(DependencyObject obj, CheckBoxAlignment value)
        {
            obj.SetValue(CheckBoxAlignmentProperty, value);
        }

        /// <summary>
        /// Gets the check box alignment.
        /// </summary>
        /// <param name="obj">The obj of CheckBoxAlignment.</param>
        /// <returns>CheckBox Alignment</returns>
        public static CheckBoxAlignment GetCheckBoxAlignment(DependencyObject obj)
        {
            return (CheckBoxAlignment)obj.GetValue(CheckBoxAlignmentProperty);
        }

        #endregion Dependency Property Setter and Getter

        #region Dependency Properties

        /// <summary>
        /// Occurs when [is drag drop enabled changed].
        /// </summary>
        public event PropertyChangedCallback IsDragDropEnabledChanged;

        /// <summary>
        /// Represents the IsCheckonFirstClick Dependency property
        /// </summary>
        public readonly static DependencyProperty IsCheckOnFirstClickProperty = DependencyProperty.Register("IsCheckOnFirstClick", typeof(bool), typeof(CheckListBox), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Represents the CheckBoxAlignment Dependency property
        /// </summary>
        public readonly static DependencyProperty CheckBoxAlignmentProperty = DependencyProperty.RegisterAttached("CheckBoxAlignment", typeof(CheckBoxAlignment), typeof(CheckListBox), new FrameworkPropertyMetadata(CheckBoxAlignment.Left));

        /// <summary>
        /// Represents the IsDragDropEnabled Dependency property
        /// </summary>
        public readonly static DependencyProperty IsDragDropEnabledProperty = DependencyProperty.RegisterAttached("IsDragDropEnabled", typeof(bool), typeof(CheckListBox), new FrameworkPropertyMetadata(false, OnIsDragDropEnabled));

        #endregion Dependency Properties

        #region Events

        /// <summary>
        /// Represents the DragStartEvent of the CheckListBox
        /// </summary>
        public static readonly RoutedEvent DragStartEvent = EventManager.RegisterRoutedEvent(
           "DragStart",
           RoutingStrategy.Bubble,
           typeof(DragCheckListBoxHandler),
           typeof(CheckListBox));

        /// <summary>
        /// Represents the DragEndEvent of the CheckListBox
        /// </summary>
        public static readonly RoutedEvent DragEndEvent = EventManager.RegisterRoutedEvent(
            "DragEnd",
            RoutingStrategy.Bubble,
            typeof(DragCheckListBoxHandler),
            typeof(CheckListBox));

        #endregion Events

        #region Event methods

        /// <summary>
        /// Called when [is drag drop enabled].
        /// </summary>
        /// <param name="d">The d of DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsDragDropEnabled(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CheckListBox instance = (CheckListBox)d;
            instance.OnIsDragDropEnabled(e);
        }

        /// <summary>
        /// Raises the <see cref="E:IsDragDropEnabled"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnIsDragDropEnabled(DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                this.AllowDrop = true;
            }
            else
            {
                this.AllowDrop = false;
            }

            if (IsDragDropEnabledChanged != null)
            {
                IsDragDropEnabledChanged(this, e);
            }
        }

        /// <summary>
        /// Occurs when [drag start].
        /// </summary>
        public event DragCheckListBoxHandler DragStart
        {
            add
            {
                AddHandler(DragStartEvent, value);
            }

            remove
            {
                RemoveHandler(DragStartEvent, value);
            }
        }

        /// <summary>
        /// Occurs when [drag end].
        /// </summary>
        public event DragCheckListBoxHandler DragEnd
        {
            add
            {
                AddHandler(DragEndEvent, value);
            }

            remove
            {
                RemoveHandler(DragEndEvent, value);
            }
        }

        /// <summary>
        /// Represents the Drag Check List Box handler
        /// </summary>
        /// <param name="sender">The sender param.</param>
        /// <param name="e">drag check list box event args.</param>
        public delegate void DragCheckListBoxHandler(object sender, DragCheckLixtBoxEventArgs e);

        #endregion Event methods

        #region Override Methods

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>
        /// The element that is used to display the given item.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CheckListBoxItem();
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
            bool toReturn = item is CheckListBoxItem;
            return toReturn;
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        #endregion Override Methods

        #region Mouse static event handler

        /// <summary>
        /// Called when [mouse button up].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private static void OnMouseButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                CheckListBox box = (CheckListBox)sender;
                FrameworkElement lite = e.OriginalSource as FrameworkElement;
                CheckListBoxItem item = CheckListBox.GetCheckListBoxItemFromChildren(lite);

                if (item != null)
                {
                    if (e.OriginalSource is CheckBox)
                    {
                        item.IsSelected = (bool)(e.OriginalSource as CheckBox).IsChecked;
                        item.Focus();
                        item.IsFirstClick = true;
                    }
                    else
                    {
                        if (box.IsCheckOnFirstClick)
                        {
                            item.IsSelected = !item.IsSelected;
                            item.Focus();
                        }
                        else
                        {
                            if (item.IsFirstClick)
                            {
                                item.Focus();
                                item.IsFirstClick = false;
                            }
                            else
                            {
                                if (box.LastActionItem == item)
                                {
                                    item.IsSelected = !item.IsSelected;
                                    item.IsFirstClick = true;
                                    item.Focus();
                                }
                                else
                                {
                                    item.Focus();
                                    item.IsFirstClick = false;
                                }
                            }
                        }
                    }

                    box.LastActionItem = item;
                }

                box.ReleaseMouseCapture();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Called when [mouse button down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private static void OnMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            CheckListBox box = (CheckListBox)sender;
            FrameworkElement lite = e.OriginalSource as FrameworkElement;
            CheckListBoxItem item = CheckListBox.GetCheckListBoxItemFromChildren(lite);

            if (item != null)
            {
                item.Focus();
            }

            box.ReleaseMouseCapture();
            e.Handled = true;
        }

        #endregion Mouse static event handler

        #region Keyboard event handler

        /// <summary>
        /// Called when [got keyboard focus].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyboardFocusChangedEventArgs"/> instance containing the event data.</param>
        private static void OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            CheckListBox box = (CheckListBox)sender;

            if (e.NewFocus is CheckListBox)
            {
                if (box.SelectedItems.Count > 0)
                {
                    if ((box.SelectedItems[0] as CheckListBoxItem) != null)
                    {
                        (box.SelectedItems[0] as CheckListBoxItem).Focus();
                    }
                }
                else if (box.Items.Count > 0)
                {
                    CheckListBoxItem item = box.ItemContainerGenerator.ContainerFromIndex(0) as CheckListBoxItem;
                    if (item != null)
                    {
                        item.Focus();
                    }
                }
            }

            CheckListBoxItem newFocus = e.NewFocus as CheckListBoxItem;
            if ((newFocus != null) && (ItemsControl.ItemsControlFromItemContainer(newFocus) == box))
            {
                DependencyObject oldFocus = e.OldFocus as DependencyObject;
                if (oldFocus == box)
                {
                    newFocus.Focus();
                }
            }

            e.Handled = true;
        }

        /// <summary>
        /// Invoked when the <see cref="E:System.Windows.UIElement.KeyDown"/> event is received.
        /// </summary>
        /// <param name="e">Information about the event.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            Key key = e.Key;
            CheckListBoxItem originalSource = e.OriginalSource as CheckListBoxItem;
            switch (key)
            {
                case Key.Space:
                case Key.Return:
                    this.ToggleSelection(originalSource);
                    break;

                default:
                    break;
            }

            base.OnKeyDown(e);
        }

        #endregion Keyboard event handler

        #region Implementation

        /// <summary>
        /// Clears the selected items.
        /// </summary>
        private void ClearSelectedItems()
        {
            int count = base.SelectedItems.Count;
            for (int i = 0; i < count; i++)
            {
                base.SelectedItems.Remove(base.SelectedItems[0]);
            }
        }

        /// <summary>
        /// Toggles the selection.
        /// </summary>
        /// <param name="litem">The litem.</param>
        private void ToggleSelection(CheckListBoxItem litem)
        {
            if (ItemsControl.ItemsControlFromItemContainer(litem) == this)
            {
                litem.IsSelected = !litem.IsSelected;
            }
        }

        /// <summary>
        /// Gets the check list box item from children.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>Check ListBoxItem</returns>
        public static CheckListBoxItem GetCheckListBoxItemFromChildren(FrameworkElement element)
        {
            CheckListBoxItem item = null;

            if (element != null)
            {
                item = element as CheckListBoxItem;

                if (item == null)
                {
                    while (element != null)
                    {
                        element = VisualTreeHelper.GetParent(element) as FrameworkElement;

                        if (element is CheckListBoxItem)
                        {
                            item = (CheckListBoxItem)element;
                            break;
                        }
                    }
                }
            }

            return item;
        }

        //public System.Collections.IList CheckedItems
        //{
        //    get
        //    {
        //        return m_selectedItems;
        //    }
        //}

        /// <summary>
        /// Gets the check box from children.
        /// </summary>
        /// <param name="element">The element ChildrenCheckBox.</param>
        /// <returns>Check Box from Children</returns>
        public static CheckBox GetCheckBoxFromChildren(FrameworkElement element)
        {
            CheckBox item = null;

            if (element != null)
            {
                item = element as CheckBox;

                if (item == null)
                {
                    while (element != null)
                    {
                        element = VisualTreeHelper.GetParent(element) as FrameworkElement;

                        if (element is CheckBox)
                        {
                            item = (CheckBox)element;
                            break;
                        }
                    }
                }
            }

            return item;
        }

        /// <summary>
        /// Called when the selection changes.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            //if (e.RemovedItems.Count == 0)
            //{
            //    m_selectedItems.Add(e.AddedItems);
            //    for (int j = 0; j < e.AddedItems.Count; j++)
            //    {
            //         string newitem = (e.AddedItems[j] as ContentControl).Content.ToString();
            //         contnt[index] = newitem;
            //         index++;
            //    }
            //}

            //else
            //{
            //    var temp = (e.RemovedItems[0] as ContentControl).Content;
            //    for (int i = 0; i < m_selectedItems.Count; i++)
            //    {
            //        if (contnt[i] == temp)
            //        {
            //            m_selectedItems.RemoveAt(i);
            //        }
            //    }
            //}
            base.OnSelectionChanged(e);
        }

        #endregion Implementation

        #region Drag and Drop

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.DragDrop.DragEnter"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.DragEventArgs"/> that contains the event data.</param>
        protected override void OnDrop(DragEventArgs e)
        {
            if (IsDragDropEnabled)
            {
                CheckListBox box = e.Source as CheckListBox;

                if (box == null)
                {
                    box = (CheckListBox)CheckListBox.FindAncestor(typeof(CheckListBox), e.Source as FrameworkElement);
                }

                DragCheckLixtBoxEventArgs args = new DragCheckLixtBoxEventArgs(DragEndEvent);
                args.Data = (DataObject)e.Data;
                args.DragSource = CheckListBox.DragDropHandler.DragSourceCheckListBox;
                args.DropSource = this;
                RaiseEvent(args);

                if (box != null && !args.Cancel)
                {
                    object data = e.Data.GetData(typeof(CheckListItemsCollection));
                    CheckListItemsCollection collection = data as CheckListItemsCollection;

                    foreach (object obj in collection)
                    {
                        if (args.DragDropEffects == DragDropEffects.Move)
                        {
                            bool temp = (obj as CheckListBoxItem).IsSelected;
                            if (args.DragSource is CheckListBox)
                                (args.DragSource as CheckListBox).Items.Remove(obj);
                            (obj as CheckListBoxItem).IsSelected = temp;
                            if (args.DropSource is CheckListBox && !(args.DropSource as CheckListBox).Items.Contains(obj))
                                (args.DropSource as CheckListBox).Items.Add(obj);
                        }

                        if (args.DragDropEffects == DragDropEffects.Copy)
                        {
                            ICloneable clone = null;
                            clone = obj as ICloneable;
                            if (clone != null)
                            {
                                if (args.DropSource is CheckListBox)
                                    (args.DropSource as CheckListBox).Items.Add(clone.Clone());
                            }
                        }
                    }

                    if (fackItemsAdorner != null)
                    {
                        AdornerLayer.GetAdornerLayer(this).Remove(fackItemsAdorner);
                    }
                }
            }

            CheckListBox.DragDropHandler.DragSourceCheckListBox = null;
            base.OnDrop(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.DragDrop.PreviewDragLeave"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.DragEventArgs"/> that contains the event data.</param>
        protected override void OnPreviewDragLeave(DragEventArgs e)
        {
            if (CheckListBox.DragDropHandler.DragData != null && IsDragDropEnabled)
            {
                RemoveDraggedAdorner();
            }

            base.OnPreviewDragLeave(e);
            e.Handled = true;
        }

        /// <summary>
        /// Determines whether this instance [can start drag] the specified starting mouse offset.
        /// </summary>
        /// <param name="startingMouseOffset">The starting mouse offset.</param>
        /// <param name="currentMouseOffset">The current mouse offset.</param>
        /// <returns>
        /// true if this instance [can start drag] the specified starting mouse offset; otherwise, false.
        /// </returns>
        public static bool CanStartDrag(Point startingMouseOffset, Point currentMouseOffset)
        {
            return (Math.Abs(currentMouseOffset.X - startingMouseOffset.X) >= SystemParameters.MinimumHorizontalDragDistance) ||
                 (Math.Abs(currentMouseOffset.Y - startingMouseOffset.Y) >= SystemParameters.MinimumVerticalDragDistance);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.PreviewMouseMove"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (CheckListBox.DragDropHandler.DragData != null && IsDragDropEnabled)
            {
                if (CanStartDrag(startingMousePointAtDragStart, e.GetPosition(this.applicationRootElement)))
                {
                    if (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed)
                    {
                        bool temp = this.applicationRootElement.AllowDrop;
                        this.applicationRootElement.AllowDrop = true;
                        this.applicationRootElement.DragEnter += ApplicationRootElement_DragEnter;
                        this.applicationRootElement.DragOver += ApplicationRootElement_DragOver;
                        this.applicationRootElement.DragLeave += ApplicationRootElement_DragLeave;

                        DragCheckLixtBoxEventArgs args = new DragCheckLixtBoxEventArgs(DragStartEvent);
                        args.Data = new DataObject(CheckListBox.DragDropHandler.DragData);
                        args.DragSource = CheckListBox.DragDropHandler.DragSourceCheckListBox;
                        args.DropSource = this;
                        RaiseEvent(args);

                        if (CheckListBox.DragDropHandler.DragData != null && !args.Cancel)
                        {
                            DragDrop.DoDragDrop(this, CheckListBox.DragDropHandler.DragData, args.DragDropEffects);
                        }

                        RemoveDraggedAdorner();

                        this.applicationRootElement.AllowDrop = temp;
                        this.applicationRootElement.DragEnter -= ApplicationRootElement_DragEnter;
                        this.applicationRootElement.DragOver -= ApplicationRootElement_DragOver;
                        this.applicationRootElement.DragLeave -= ApplicationRootElement_DragLeave;

                        CheckListBox.DragDropHandler.DragData = null;
                    }
                }
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.PreviewMouseLeftButtonDown"/> routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the left mouse button was pressed.</param>
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (IsDragDropEnabled)
            {
                CheckListBox.DragDropHandler.DragSourceCheckListBox = (CheckListBox)CheckListBox.FindAncestor(typeof(CheckListBox), e.Source as FrameworkElement);
                CheckListBoxItem item = GetCheckListBoxItemFromChildren((e.OriginalSource as FrameworkElement));
                this.applicationRootElement = (Window)CheckListBox.FindAncestor(typeof(Window), this);

                if (applicationRootElement != null)
                {
                    startingMousePointAtDragStart = e.GetPosition(applicationRootElement);
                }
                else
                {
                    this.rootPage = (Page)CheckListBox.FindAncestor(typeof(Page), this);
                    startingMousePointAtDragStart = e.GetPosition(this.rootPage);
                }

                if (item != null)
                {
                    GetDragDataObject();
                }
            }
        }

        /// <summary>
        /// Removes the dragged adorner.
        /// </summary>
        private void RemoveDraggedAdorner()
        {
            if (this.fackItemsAdorner != null)
            {
                this.fackItemsAdorner.Remove();
                this.fackItemsAdorner = null;
            }
        }

        /// <summary>
        /// Shows the dragged adorner.
        /// </summary>
        /// <param name="currentPosition">The current position.</param>
        private void ShowDraggedAdorner(Point currentPosition)
        {
            if (this.fackItemsAdorner == null && this.Items.Count > 0)
            {
                var adornerLayer = AdornerLayer.GetAdornerLayer(this);
                this.fackItemsAdorner = new FackItemsAdorner(GetFakeItemsPresenter(), this, adornerLayer);
            }

            this.fackItemsAdorner.SetPosition(currentPosition.X, currentPosition.Y);
        }

        /// <summary>
        /// Handles the DragEnter event of the ApplicationRootElement control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
        private void ApplicationRootElement_DragEnter(object sender, DragEventArgs e)
        {
            CheckListBox box = VisualUtils.FindDescendant(sender as Visual, typeof(CheckListBox)) as CheckListBox;
            ShowDraggedAdorner(e.GetPosition(box));
            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }

        /// <summary>
        /// Handles the DragOver event of the ApplicationRootElement control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
        private void ApplicationRootElement_DragOver(object sender, DragEventArgs e)
        {
            CheckListBox box = VisualUtils.FindDescendant(sender as Visual, typeof(CheckListBox)) as CheckListBox;
            ShowDraggedAdorner(e.GetPosition(box));
            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }

        /// <summary>
        /// Handles the DragLeave event of the ApplicationRootElement control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
        private void ApplicationRootElement_DragLeave(object sender, DragEventArgs e)
        {
            RemoveDraggedAdorner();
            e.Handled = true;
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.DragDrop.DragOver"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.DragEventArgs"/> that contains the event data.</param>
        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);
        }

        /// <summary>
        /// It initializes the fake item collection.
        /// </summary>
        private CheckListItemsCollection m_fakeItemsCollection = new CheckListItemsCollection();

        /// <summary>
        /// Gets data for drag.
        /// </summary>
        private void GetDragDataObject()
        {
            UpdateDraggingItems();

            if (m_fakeItemsCollection != null && m_fakeItemsCollection.Count > 0)
            {
                CheckListBox.DragDropHandler.DragData = m_fakeItemsCollection;
            }
        }

        /// <summary>
        /// Updates the dragging items.
        /// </summary>
        private void UpdateDraggingItems()
        {
            m_fakeItemsCollection.Clear();

            if (SelectedItems != null && SelectedItems.Count > 0)
            {
                for (int i = 0; i < SelectedItems.Count; i++)
                {
                    m_fakeItemsCollection.Add(SelectedItems[i]);
                }
            }
            else if (SelectedItem != null)
            {
                m_fakeItemsCollection.Add(SelectedItem);
            }
        }

        /// <summary>
        /// Gets the fake items presenter.
        /// </summary>
        /// <returns>Framework Element</returns>
        private FrameworkElement GetFakeItemsPresenter()
        {
            StackPanel panel = null;

            if (m_fakeItemsCollection != null && m_fakeItemsCollection.Count > 0)
            {
                CheckListItemsCollection cloneData = new CheckListItemsCollection();
                ICloneable clone = null;
                panel = new StackPanel();
                panel.Orientation = Orientation.Vertical;

                for (int i = 0; i < m_fakeItemsCollection.Count; i++)
                {
                    clone = m_fakeItemsCollection[i] as ICloneable;

                    if (clone != null)
                    {
                        ContentPresenter item = new ContentPresenter();
                        item.Content = (clone.Clone() as CheckListBoxItem).Content;
                        panel.Children.Add(item);
                    }
                }
            }

            return panel;
        }

        #endregion Drag and Drop

        #region Static methods and classes

        /// <summary>
        /// Handles the DragDrop
        /// </summary>
        internal static class DragDropHandler
        {
            /// <summary>
            /// Gets or sets the drag data.
            /// </summary>
            /// <value>The drag data.</value>
            public static CheckListItemsCollection DragData
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the drag source check list box.
            /// </summary>
            /// <value>The drag source check list box.</value>
            public static CheckListBox DragSourceCheckListBox
            {
                get;
                set;
            }
        }

        /// <summary>
        /// Finds the ancestor.
        /// </summary>
        /// <param name="ancestorType">Type of the ancestor.</param>
        /// <param name="visual">The visual.</param>
        /// <returns> Framework Element </returns>
        public static FrameworkElement FindAncestor(Type ancestorType, Visual visual)
        {
            while (visual != null && !ancestorType.IsInstanceOfType(visual))
            {
                visual = (Visual)VisualTreeHelper.GetParent(visual);
            }

            return visual as FrameworkElement;
        }

        #endregion Static methods and classes
    }

    #region Custom Events

    /// <summary>
    /// Represents the DragListBoxEventArgs class
    /// </summary>
    public class DragCheckLixtBoxEventArgs : RoutedEventArgs
    {
        #region Member

        /// <summary>
        /// Contains data
        /// </summary>
        private DataObject m_data = null;

        /// <summary>
        /// Target value
        /// </summary>
        private FrameworkElement m_target;

        /// <summary>
        /// Source Value
        /// </summary>
        private FrameworkElement m_source;

        /// <summary>
        /// It has Move value of DragDropEffects
        /// </summary>
        private DragDropEffects m_dragDropEffects = DragDropEffects.Move;

        /// <summary>
        /// Cancel value
        /// </summary>
        private bool m_cancel;

        #endregion Member

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="DragCheckLixtBoxEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The routed event.</param>
        public DragCheckLixtBoxEventArgs(RoutedEvent routedEvent)
            : base(routedEvent)
        {
        }

        #endregion Initialization

        #region Properties

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data of CheckListBox.</value>
        public DataObject Data
        {
            get
            {
                return m_data;
            }

            set
            {
                if (value != m_data)
                {
                    m_data = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the drop source.
        /// </summary>
        /// <value>The drop source.</value>
        public FrameworkElement DropSource
        {
            get
            {
                return m_target;
            }

            set
            {
                if (value != m_target)
                {
                    m_target = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the drag source.
        /// </summary>
        /// <value>The drag source.</value>
        public FrameworkElement DragSource
        {
            get
            {
                return m_source;
            }

            set
            {
                if (value != m_source)
                {
                    m_source = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="DragCheckLixtBoxEventArgs"/> is cancel.
        /// </summary>
        /// <value><c>true</c> if cancel; otherwise, <c>false</c>.</value>
        public bool Cancel
        {
            get
            {
                return m_cancel;
            }

            set
            {
                if (value != m_cancel)
                {
                    m_cancel = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the drag drop effects.
        /// </summary>
        /// <value>The drag drop effects.</value>
        public DragDropEffects DragDropEffects
        {
            get
            {
                return m_dragDropEffects;
            }

            set
            {
                if (value != m_dragDropEffects)
                {
                    m_dragDropEffects = value;
                }
            }
        }

        #endregion Properties
    }

    #endregion Custom Events

    #region CheckListItemsCollection

    /// <summary>
    /// Represents the CheckListItems Collection
    /// </summary>
    public class CheckListItemsCollection : ObservableCollection<object>
    {
    }

    #endregion CheckListItemsCollection
}