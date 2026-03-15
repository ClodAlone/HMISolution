using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Collections;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using Infralution.Licensing;
using System.Windows.Data;
using System.Diagnostics;
using Mindscape.WpfElements.WpfPropertyGrid;
using System.Linq;
using Mindscape.WpfElements.PropertyEditing;
using System.Windows.Threading;
using System.Windows.Media;

namespace Mindscape.WpfElements
{
    /// <summary>
    /// A control for browsing and editing the properties of an object.
    /// </summary>
    [LicenseProvider(typeof(PublicEncryptedLicenseProvider))]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    public class PropertyGrid : Control, IExtendInPlaceEditors, IProvideEditorHosting
    {

        static PropertyGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGrid),
              new FrameworkPropertyMetadata(typeof(PropertyGrid)));
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="PropertyGrid"/> class.
        /// </summary>
        public PropertyGrid()
        {
            // Licensing
            //new WpfElementsCore(Assembly.GetCallingAssembly());
            //LicenseHelper.Attach(this, Assembly.GetCallingAssembly());
            // End licensing

            Loaded += new RoutedEventHandler(PropertyGrid_Loaded);
            Unloaded += new RoutedEventHandler(PropertyGrid_Unloaded);

            CommandBindings.Add(new CommandBinding(CollectionAddCommand, CollectionAddCommand_Executed, CollectionAddCommand_CanExecute));
            CommandBindings.Add(new CommandBinding(CollectionRemoveCommand, CollectionRemoveCommand_Executed, CollectionRemoveCommand_CanExecute));

            AddHandler(TreeViewItem.ExpandedEvent, new RoutedEventHandler(OnTreeViewItemExpanded));
            AddHandler(TreeViewItem.CollapsedEvent, new RoutedEventHandler(OnTreeViewItemCollapsed));

            Nodes = new ObservableCollection<Node>();
            BindingView = new PropertyGridBindingView(Nodes);
            _editors = new EditorCollection(this);
            _editorSelector = new EditorSelector(this);

            KeyDown += new KeyEventHandler(PropertyGrid_KeyDown);
            PreviewKeyDown += new KeyEventHandler(PropertyGrid_PreviewKeyDown);
            PreviewMouseDown += new MouseButtonEventHandler(PropertyGrid_PreviewMouseDown);

            AddDesignTimeSampleData();
        }

        bool bLoaded;
        private void PropertyGrid_Loaded(object sender, RoutedEventArgs e)
        {
            if (bLoaded)
                return;
            bLoaded = true;

            if (BindingView == null || BindingView.DefaultView == null || BindingView.DefaultView.GroupDescriptions == null)
                return;

            if (BindingView.DefaultView.GroupDescriptions.Count > 0)
            {
                using (BindingView.DefaultView.DeferRefresh())
                {
                    BindingView.DefaultView.GroupDescriptions.Clear();
                    if (Grouping != null)
                    {
                        BindingView.DefaultView.GroupDescriptions.Add(Grouping);
                    }
                }
            }
            else if (Grouping != null)
            {
                BindingView.DefaultView.GroupDescriptions.Add(Grouping);
            }
        }

        private void PropertyGrid_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        #region Licensing

        /// <summary>
        /// Performs manual licensing for specialized deployment scenarios.
        /// </summary>
        /// <param name="licenseKey">A runtime license key.</param>
        /// <remarks>In normal deployment scenarios, the <see cref="PropertyGrid"/> control is
        /// automatically licensed.  Install a license key manually only under advisement
        /// from Mindscape support.</remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void InstallLicense(string licenseKey)
        {
            LicenseHelper.SetLicenseKeyToUse(licenseKey);
        }

        #endregion

        #region Design-time view

        private void AddDesignTimeSampleData()
        {
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                AddNode("Sample Boolean", false);
                AddNode("Sample Numeric", 0, PropertyGrid.NumericUpDownEditorKey);
                AddNode("Sample Enum", ConsoleColor.DarkBlue);
                AddNode("Sample String", "Text");
            }
        }

        #endregion

        #region Keep highlighting in name column synchronised

        private void PropertyGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            UIElement originalSource = e.OriginalSource as UIElement;
            if (originalSource != null)
            {
                TreeViewItem tvitem = VisualTreeUtils.FindAncestor<TreeViewItem>(originalSource);
                if (tvitem != null)
                {
                    //tvitem.IsSelected = true;  // This seems to cause a memory leak!
                    PropertyGridRow pgr = tvitem.Header as PropertyGridRow;
                    if (pgr != null)
                    {
                        SelectedGridItem = pgr;
                    }
                }
            }
        }

        #endregion

        #region Keyboard navigation

        private void PropertyGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Source == this && e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.None)
            {
                TreeViewItem item = e.OriginalSource as TreeViewItem;
                if (item != null)
                {
                    item.IsExpanded = !item.IsExpanded;
                    e.Handled = true;
                }
            }

            if (CursorKeyMode == CursorKeyMode.PassToEditor)
            {
                if (e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Left || e.Key == Key.Right)
                {
                    PerformCursorKeyNavigation(e);
                }
            }
        }

        private void PropertyGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (CursorKeyMode == CursorKeyMode.Navigate)
            {
                PerformCursorKeyNavigation(e);
            }
        }

        private static void PerformCursorKeyNavigation(KeyEventArgs e)
        {
            TreeViewItem tvitem = VisualTreeUtils.GetAncestorOfFocusedElement<TreeViewItem>();
            if (tvitem != null)
            {
                if (Keyboard.Modifiers == ModifierKeys.None && (e.Key == Key.Up || e.Key == Key.Down))
                {
                    var linearised = Linearise(tvitem);
                    int index = linearised.IndexOf(tvitem);
                    int newIndex = index + (e.Key == Key.Up ? -1 : 1);

                    if (newIndex >= 0 && newIndex < linearised.Count)
                    {
                        TreeViewItem newItem = linearised[newIndex];
                        newItem.IsSelected = true;
                        newItem.Focus();
                    }

                    e.Handled = true;
                    return;
                }
            }
            if (e.Key == Key.Tab)
            {
                var focusNavigation = Keyboard.Modifiers == ModifierKeys.Shift ? FocusNavigationDirection.Previous : FocusNavigationDirection.Next;
                var focused = Keyboard.FocusedElement as FrameworkElement;
                if (focused != null)
                {
                    focused.MoveFocus(new TraversalRequest(focusNavigation));
                    focused = Keyboard.FocusedElement as FrameworkElement;
                    if (focused is ContentControl && (focused as ContentControl).Content is PropertyGridRow)
                        focused.MoveFocus(new TraversalRequest(focusNavigation));
                }
                e.Handled = true;
                return;
            }
        }

        #region Helpers for keyboard navigation - arranging the set of nodes of interest in a linear way so we can easily step +/-1

        // We manually linearise stuff to work around a bug in the WPF MoveFocus method when using the Up
        // traversal request on an expanded parent - it jumps to the last child of the expanded parent.

        private static IList<TreeViewItem> Linearise(TreeViewItem startingFrom)
        {
            // We need to capture the parent level (parent and all aunts), sibling level and any expanded child/nephew levels.
            // (We don't need cousins because up or down can't take you directly to a cousin, only to a parent, uncle,
            // sibling, child or nephew.)
            bool atTopLevel = false;
            var p = System.Windows.Media.VisualTreeHelper.GetParent(startingFrom);
            ItemsControl parent = VisualTreeUtils.FindContaining<TreeViewItem>(p);
            if (parent == null)
            {
                parent = VisualTreeUtils.FindContaining<TreeView>(p);
                atTopLevel = true;
            }

            ICollection siblings = parent.Items;  // includes self
            ICollection aunts = new object[0];  // includes parent

            ItemsControl grandparent = null;

            if (!atTopLevel)
            {
                p = System.Windows.Media.VisualTreeHelper.GetParent(parent);
                grandparent = VisualTreeUtils.FindContaining<TreeViewItem>(p);
                if (grandparent == null)
                {
                    grandparent = VisualTreeUtils.FindContaining<TreeView>(p);
                }
                aunts = grandparent.Items;
            }

            List<TreeViewItem> linearised = new List<TreeViewItem>(aunts.Count + siblings.Count);

            if (grandparent == null)
            {
                LineariseSiblings(parent, siblings, linearised);
            }
            else
            {
                foreach (var aunt in aunts)
                {
                    TreeViewItem tvi = grandparent.ItemContainerGenerator.ContainerFromItem(aunt) as TreeViewItem;
                    if (tvi != null)
                    {
                        linearised.Add(tvi);
                        if (tvi == parent)
                        {
                            LineariseSiblings(parent, siblings, linearised);
                        }
                    }
                }
            }

            return linearised;
        }

        private static void LineariseSiblings(ItemsControl parent, ICollection siblings, List<TreeViewItem> linearised)
        {
            foreach (var sibling in siblings)
            {
                TreeViewItem stvi = parent.ItemContainerGenerator.ContainerFromItem(sibling) as TreeViewItem;
                if (stvi != null)
                {
                    linearised.Add(stvi);
                    if (stvi.IsExpanded)
                    {
                        foreach (var nephew in stvi.Items)
                        {
                            TreeViewItem ntvi = stvi.ItemContainerGenerator.ContainerFromItem(nephew) as TreeViewItem;
                            if (ntvi != null)
                            {
                                linearised.Add(ntvi);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #endregion

        #region Collection handling

        /// <summary>
        /// A command for adding an item to a collection.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable")]
        public static readonly ICommand CollectionAddCommand = new RoutedCommand("CollectionAdd", typeof(PropertyGrid));

        /// <summary>
        /// A command for removing an item from a collection.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Type is immutable")]
        public static readonly ICommand CollectionRemoveCommand = new RoutedCommand("CollectionRemove", typeof(PropertyGrid));

        /// <summary>
        /// Identifies the <see cref="CollectionAddRequested"/> routed event.
        /// </summary>
        public static readonly RoutedEvent CollectionAddRequestedEvent =
          EventManager.RegisterRoutedEvent("CollectionAddRequested", RoutingStrategy.Bubble,
            typeof(EventHandler<CollectionAddRequestedEventArgs>), typeof(PropertyGrid));

        /// <summary>
        /// Occurs when the user selects an "add new item to collection" button in the
        /// property grid user interface.
        /// </summary>
        public event EventHandler<CollectionAddRequestedEventArgs> CollectionAddRequested
        {
            add { AddHandler(CollectionAddRequestedEvent, value); }
            remove { RemoveHandler(CollectionAddRequestedEvent, value); }
        }

        private void CollectionAddCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (AllowModifyCollections)
            {
                object collection = e.Parameter;

                CollectionAddRequestedEventArgs carea = new CollectionAddRequestedEventArgs(CollectionUtilities.GetCollectionValueType(collection));
                RaiseEvent(carea);
                if (carea.ValueSet)
                {
                    ((IList)collection).Add(carea.Value);
                }
                else
                {
                    CollectionUtilities.AddDefaultValueEntry(collection);
                }
            }
        }

        private void CollectionAddCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = AllowModifyCollections && CollectionUtilities.CanAddToCollection(e.Parameter);
        }

        private void CollectionRemoveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CollectionElement node = e.Parameter as CollectionElement;
            if (node != null && AllowModifyCollections)
            {
                node.RemoveFromParentCollection();
            }
        }

        private void CollectionRemoveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            CollectionElement collectionElement = e.Parameter as CollectionElement;
            e.CanExecute = AllowModifyCollections &&
              collectionElement != null &&
              collectionElement.CanRemoveFromParentCollection();
        }

        /// <summary>
        /// Gets or sets whether the grid should display a user interface to allow the user to
        /// add or remove items from collections.
        /// This is a dependency property.
        /// </summary>
        /// <remarks>
        /// <para>Setting this property to false does <strong>not</strong> prevent users from
        /// editing the values in collections: it only prevents adding and removing items.</para>
        /// <strong>Dependency Property Information</strong>
        /// <table>
        ///   <tr><td>Identifier field</td><td><see cref="AllowModifyCollectionsProperty"/></td></tr>
        ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
        /// </table>
        /// </remarks>
        public bool AllowModifyCollections
        {
            get { return (bool)GetValue(AllowModifyCollectionsProperty); }
            set { SetValue(AllowModifyCollectionsProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="AllowModifyCollections"/> property.
        /// </summary>
        public static readonly DependencyProperty AllowModifyCollectionsProperty =
            DependencyProperty.Register("AllowModifyCollections", typeof(bool), typeof(PropertyGrid), new UIPropertyMetadata(true));


        /// <summary>
        /// Gets or sets how the grid handles cursor keys when typed into editors.
        /// This is a dependency property.
        /// </summary>
        /// <remarks>
        /// <strong>Dependency Property Information</strong>
        /// <table>
        ///   <tr><td>Identifier field</td><td><see cref="CursorKeyModeProperty"/></td></tr>
        ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
        /// </table>
        /// </remarks>
        public CursorKeyMode CursorKeyMode
        {
            get { return (CursorKeyMode)GetValue(CursorKeyModeProperty); }
            set { SetValue(CursorKeyModeProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="CursorKeyMode"/> property.
        /// </summary>
        public static readonly DependencyProperty CursorKeyModeProperty =
            DependencyProperty.Register("CursorKeyMode", typeof(CursorKeyMode), typeof(PropertyGrid),
            new FrameworkPropertyMetadata(CursorKeyMode.Navigate));

        #endregion

        private const string GridPartName = "PART_Grid";

        /// <summary>
        /// Called by the framework when a template is applied to the control.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            SetUpEventTranslators();
        }

        #region SelectedGridItemChanged event support


        /// <summary>
        /// Gets or sets the selected grid item.
        /// This is a dependency property.
        /// </summary>
        /// <remarks>
        /// <strong>Dependency Property Information</strong>
        /// <table>
        ///   <tr><td>Identifier field</td><td><see cref="SelectedGridItemProperty"/></td></tr>
        ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
        /// </table>
        /// </remarks>
        public PropertyGridRow SelectedGridItem
        {
            get { return (PropertyGridRow)GetValue(SelectedGridItemProperty); }
            set { SetValue(SelectedGridItemProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="SelectedGridItem"/> property.
        /// </summary>
        public static readonly DependencyProperty SelectedGridItemProperty =
            DependencyProperty.Register("SelectedGridItem", typeof(PropertyGridRow),
            typeof(PropertyGrid), new FrameworkPropertyMetadata(OnSelectedGridItemChanged));

        private static void OnSelectedGridItemChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            PropertyGridRow oldSelection = e.OldValue as PropertyGridRow;
            PropertyGridRow newSelection = e.NewValue as PropertyGridRow;
            PropertyGrid grid = sender as PropertyGrid;
            grid.RaiseSelectedGridItemChangedEvent(oldSelection, newSelection);
        }

        /// <summary>
        /// Identifies the <see cref="SelectedGridItemChanged"/> event.
        /// </summary>
        public static readonly RoutedEvent SelectedGridItemChangedEvent =
          EventManager.RegisterRoutedEvent("SelectedGridItemChanged", RoutingStrategy.Bubble,
          typeof(SelectedGridItemChangedEventHandler), typeof(PropertyGrid));

        /// <summary>
        /// Occurs when the user selects a new item in the property grid.
        /// </summary>
        public event SelectedGridItemChangedEventHandler SelectedGridItemChanged
        {
            add { AddHandler(SelectedGridItemChangedEvent, value); }
            remove { RemoveHandler(SelectedGridItemChangedEvent, value); }
        }

        private void SetUpEventTranslators()
        {
            DependencyObject gridImplementation = GetTemplateChild(GridPartName);

            TreeView treeViewImplementation = gridImplementation as TreeView;
            if (treeViewImplementation != null)
            {
                treeViewImplementation.SelectedItemChanged += delegate (object sender, RoutedPropertyChangedEventArgs<object> e)
                {
                    PropertyGridRow newSelection = e.NewValue as PropertyGridRow;
                    SelectedGridItem = newSelection;
                    //e.Handled = true;
                };
            }
            else
            {
                Selector selectorImplementation = gridImplementation as Selector;
                if (selectorImplementation != null)
                {
                    selectorImplementation.SelectionChanged += delegate (object sender, SelectionChangedEventArgs e)
                    {
                        PropertyGridRow newSelection = CollectionUtilities.GetFirstOrNull<PropertyGridRow>(e.AddedItems);
                        SelectedGridItem = newSelection;
                        //e.Handled = true;
                    };
                }
            }
        }

        private void RaiseSelectedGridItemChangedEvent(PropertyGridRow oldSelection, PropertyGridRow newSelection)
        {
            SelectedGridItemChangedEventArgs e = new SelectedGridItemChangedEventArgs(oldSelection, newSelection);
            RaiseEvent(e);
        }

        #endregion

        /// <summary>
        /// Gets or sets whether the built-in toolbar should be displayed above the grid.
        /// This is a dependency property.
        /// </summary>
        /// <remarks>
        /// <strong>Dependency Property Information</strong>
        /// <table>
        ///   <tr><td>Identifier field</td><td><see cref="IsToolBarVisibleProperty"/></td></tr>
        ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
        /// </table>
        /// </remarks>
        public bool IsToolBarVisible
        {
            get { return (bool)GetValue(IsToolBarVisibleProperty); }
            set { SetValue(IsToolBarVisibleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="IsToolBarVisible"/> property.
        /// </summary>
        public static readonly DependencyProperty IsToolBarVisibleProperty =
            DependencyProperty.Register("IsToolBarVisible", typeof(bool), typeof(PropertyGrid),
            new UIPropertyMetadata(false));


        /// <summary>
        /// Gets or sets the default margin placed around editors.
        /// This is a dependency property.
        /// </summary>
        /// <remarks>
        /// <strong>Dependency Property Information</strong>
        /// <table>
        ///   <tr><td>Identifier field</td><td><see cref="DefaultMarginProperty"/></td></tr>
        ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
        /// </table>
        /// </remarks>
        public Thickness DefaultMargin
        {
            get { return (Thickness)GetValue(DefaultMarginProperty); }
            set { SetValue(DefaultMarginProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DefaultMargin"/> property.
        /// </summary>
        public static readonly DependencyProperty DefaultMarginProperty =
            DependencyProperty.Register("DefaultMargin", typeof(Thickness),
            typeof(PropertyGrid), new UIPropertyMetadata(new Thickness(-5, 0, -10, 0)));

        /// <summary>
        /// Gets whether or not default margin compensation is required.
        /// </summary>
        public bool DefaultMarginCompensationRequired
        {
            get { return true; }
        }

        #region CanDisplayReadOnlyProperties Property

        /// <summary>
        /// Gets or sets whether or not read only properties are displayed.
        /// The default is true.
        /// This is a dependency property.
        /// </summary>
        /// <remarks>
        /// <strong>Dependency Property Information</strong>
        /// <table>
        ///   <tr><td>Identifier field</td><td><see cref="CanDisplayReadOnlyPropertiesProperty"/></td></tr>
        ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
        /// </table>
        /// </remarks>
        public bool CanDisplayReadOnlyProperties
        {
            get { return (bool)GetValue(CanDisplayReadOnlyPropertiesProperty); }
            set { SetValue(CanDisplayReadOnlyPropertiesProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="CanDisplayReadOnlyProperties"/> property.
        /// </summary>
        public static readonly DependencyProperty CanDisplayReadOnlyPropertiesProperty =
          DependencyProperty.Register("CanDisplayReadOnlyProperties", typeof(bool), typeof(PropertyGrid),
          new FrameworkPropertyMetadata(true, OnCanDisplayReadOnlyPropertiesChanged));

        private static void OnCanDisplayReadOnlyPropertiesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PropertyGrid)d).OnCanDisplayReadOnlyPropertiesChanged();
        }

        private void OnCanDisplayReadOnlyPropertiesChanged()
        {
            //UnhookMultiSelectCollectionChanged(SelectedObject);
            BindObject(SelectedObject);
            //HookMultiSelectCollectionChanged(SelectedObject);
        }

        #endregion // CanDisplayReadOnlyProperties Property

        private void AutoSizeColumns()
        {
            TreeListView.SetResizeColumnWidth(this, 0); // This is to reset the auto sizing of the name column.
            TreeListView.SetResizeColumnWidth(this, Double.NaN);
        }

        #region Editor extensibility

        private EditorSelector _editorSelector;

        /// <summary>
        /// Gets the template selector used to create value editors.
        /// </summary>
        public DataTemplateSelector EditorSelector
        {
            get { return _editorSelector; }
        }

        private readonly EditorCollection _editors;
        private readonly EditorDecorationCollection _editorDecorations = new EditorDecorationCollection();

        /// <summary>
        /// Gets the collection of custom editors for this PropertyGrid instance.
        /// </summary>
        /// <remarks>Add editors to this collection to define how to edit custom types or
        /// to customise the display of specific properties.</remarks>
        public EditorCollection Editors
        {
            get { return _editors; }
        }

        /// <summary>
        /// Gets the collection of custom editor decorations for this PropertyGrid instance.
        /// </summary>
        /// <remarks>Add editor decorations to this collection to define decorations to be applied
        /// to editors.</remarks>
        public EditorDecorationCollection EditorDecorations
        {
            get { return _editorDecorations; }
        }

        /// <summary>
        /// Gets or sets the styles applied to built-in editors.
        /// This is a dependency property.
        /// </summary>
        /// <remarks>
        /// <strong>Dependency Property Information</strong>
        /// <table>
        ///   <tr><td>Identifier field</td><td><see cref="BuiltInEditorStylesProperty"/></td></tr>
        ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
        /// </table>
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public BuiltInEditorStyleCollection BuiltInEditorStyles
        {
            get { return (BuiltInEditorStyleCollection)GetValue(BuiltInEditorStylesProperty); }
            set { SetValue(BuiltInEditorStylesProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="BuiltInEditorStyles"/> property.
        /// </summary>
        public static readonly DependencyProperty BuiltInEditorStylesProperty =
          DependencyProperty.Register("BuiltInEditorStyles", typeof(BuiltInEditorStyleCollection), typeof(PropertyGrid),
          new PropertyMetadata(OnBuiltInEditorStylesChanged));

        private static void OnBuiltInEditorStylesChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            /*BuiltInEditorStyleCollection oldValue = e.OldValue as BuiltInEditorStyleCollection;
            BuiltInEditorStyleCollection newValue = e.NewValue as BuiltInEditorStyleCollection;
            if (newValue != null && newValue.Include == null)
            {
              newValue.Include = oldValue;
            }*/
        }

        #endregion

        #region Data model

        #region SelectedObject property and helpers

        /// <summary>
        /// Gets or sets the object for which the grid displays properties.  This is a dependency
        /// property.
        /// </summary>
        /// <remarks>
        /// <para>Setting this property clears any existing <see cref="SelectedObjects"/> array.
        /// After setting this property, <see cref="SelectedObjects"/> returns a one-element array
        /// containing the selected object.</para>
        /// <strong>Dependency Property Information</strong>
        /// <table>
        ///   <tr><td>Identifier field</td><td><see cref="SelectedObjectProperty"/></td></tr>
        ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
        /// </table>
        /// </remarks>
        public object SelectedObject
        {
            get { return GetValue(SelectedObjectProperty); }
            set { SetValue(SelectedObjectProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="SelectedObject"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedObjectProperty
              = DependencyProperty.Register("SelectedObject", typeof(object), typeof(PropertyGrid),
                new FrameworkPropertyMetadata(OnSelectedObjectChanged));

        private static void OnSelectedObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PropertyGrid)d).OnSelectedObjectChanged(e);
        }

        private bool _inSelectedObjectChange;

        private readonly Dictionary<Type, Dictionary<string, bool>> _expandedStates = new Dictionary<Type, Dictionary<string, bool>>();

        internal Dictionary<Type, Dictionary<string, bool>> ExpandedStates { get { return _expandedStates; } }

        private void SaveExpandedStates(Type type)
        {
            Dictionary<string, bool> expandedStates = new Dictionary<string, bool>();
            TreeListView treeList = VisualTreeUtils.GetChild<TreeListView>(this);
            if (treeList != null && treeList.ItemContainerGenerator != null)
            {
                foreach (object o in treeList.Items)
                {
                    PropertyGridRow row = o as PropertyGridRow;
                    if (row != null)
                    {
                        TreeListViewItem item = treeList.ItemContainerGenerator.ContainerFromItem(row) as TreeListViewItem;
                        if (item != null)
                        {
                            string id = row.Node.Property.DeclaringType.ToString() + " " + row.Node.Property.DisplayName + " " + item.Level;
                            expandedStates[id] = item.IsExpanded;
                            /*if (item.IsExpanded)
                            {
                              Debug.WriteLine(id);
                            }*/
                            if (row.Children.Count != 0)
                            {
                                SaveExpandedStates(row, item, expandedStates);
                            }
                        }
                    }
                }
            }
            _expandedStates[type] = expandedStates;
        }

        private void SaveExpandedStates(PropertyGridRow row, TreeListViewItem rowItem, Dictionary<string, bool> dictionary)
        {
            foreach (PropertyGridRow child in row.Children)
            {
                TreeListViewItem item = rowItem.ItemContainerGenerator.ContainerFromItem(child) as TreeListViewItem;
                if (item != null)
                {
                    string id = child.Node.Property.DeclaringType.ToString() + " " + child.Node.Property.DisplayName + " " + item.Level;
                    dictionary[id] = item.IsExpanded;
                    /*if (item.IsExpanded)
                    {
                      Debug.WriteLine(id);
                    }*/
                    if (child.Children.Count != 0)
                    {
                        SaveExpandedStates(child, item, dictionary);
                    }
                }
            }
        }

        /*private string GetID(PropertyGridRow row)
        {
          string pureID = row.Node.Property.DeclaringType.ToString() + " " + row.Node.Property.DisplayName;
          string id = pureID;
          int index = 1;
          while (_expandedStates.ContainsKey(id))
          {
            id = pureID + " " + index;
            index++;
          }
          return id;
        }*/

        /*private void ApplyExpandedStates()
        {
          if (_expandedStates != null)
          {
            TreeListView treeList = VisualTreeUtils.GetChild<TreeListView>(this);
            if (treeList != null && treeList.ItemContainerGenerator != null)
            {
              foreach (object o in treeList.Items)
              {
                PropertyGridRow row = o as PropertyGridRow;
                if (row != null)
                {
                  TreeListViewItem item = treeList.ItemContainerGenerator.ContainerFromItem(row) as TreeListViewItem;
                  if (item != null)
                  {
                    bool isExpanded = false;
                    _expandedStates.TryGetValue(row.Node.Property.DeclaringType.ToString() + " " + row.Node.Property.DisplayName, out isExpanded);
                    item.IsExpanded = isExpanded;
                  }
                }
              }
            }
          }
        }*/

        private void OnSelectedObjectChanged(DependencyPropertyChangedEventArgs e)
        {
            try
            {
                _inSelectedObjectChange = true;

                if (!ReferenceEquals(e.OldValue, e.NewValue))
                {
                    if (IsExpandedStatePersistent && e.OldValue != null)
                    {
                        SaveExpandedStates(e.OldValue.GetType());
                    }

                    UnhookMultiSelectCollectionChanged(e.OldValue);
                    BindObject(e.NewValue);
                    HookMultiSelectCollectionChanged(e.NewValue);

                    MultipleObjectWrapper oldMultiSelect = e.OldValue as MultipleObjectWrapper;
                    if (oldMultiSelect != null && oldMultiSelect.OwnedByGrid)
                    {
                        oldMultiSelect.Dispose();
                    }
                }

                if (!(e.NewValue is MultipleObjectWrapper))
                {
                    if (e.NewValue == null)
                    {
                        SelectedObjects = new object[0];
                    }
                    else
                    {
                        SelectedObjects = new object[] { e.NewValue };
                    }
                }

                SelectedGridItem = null;
            }
            finally
            {
                _inSelectedObjectChange = false;
            }
            AutoSizeColumns();

            if (GroupExpanderMode == ExpanderMode.Collapsed)
            {
                Dispatcher.BeginInvoke(new Action(CollapseGroups));
            }
        }

        private void UnhookMultiSelectCollectionChanged(object selectedObject)
        {
            MultipleObjectWrapper mow = selectedObject as MultipleObjectWrapper;
            if (mow != null)
            {
                mow.ObjectCollectionChanged -= MultiSelectCollectionChanged;
            }
        }

        private void HookMultiSelectCollectionChanged(object selectedObject)
        {
            MultipleObjectWrapper mow = selectedObject as MultipleObjectWrapper;
            if (mow != null)
            {
                mow.ObjectCollectionChanged += MultiSelectCollectionChanged;
            }
        }

        private void MultiSelectCollectionChanged(object sender, EventArgs e)
        {
            BindObject(SelectedObject);
        }

        /// <summary>
        /// Gets or sets the objects for which the grid displays properties.
        /// This is a dependency property.
        /// </summary>
        /// <remarks>
        /// <para>Setting this property clears any existing <see cref="SelectedObject"/>.
        /// After setting this property, <see cref="SelectedObject"/> returns a <see cref="MultipleObjectWrapper"/>.</para>
        /// <para>Modifications to the SelectedObjects list after it is assigned will be reflected
        /// in the grid only if the list implements INotifyCollectionChanged.</para>
        /// <strong>Dependency Property Information</strong>
        /// <table>
        ///   <tr><td>Identifier field</td><td><see cref="SelectedObjectsProperty"/></td></tr>
        ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
        /// </table>
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public IList SelectedObjects
        {
            get { return (IList)GetValue(SelectedObjectsProperty); }
            set { SetValue(SelectedObjectsProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="SelectedObjects"/> property.
        /// </summary>
        public static readonly DependencyProperty SelectedObjectsProperty =
          DependencyProperty.Register("SelectedObjects", typeof(IList), typeof(PropertyGrid),
          new FrameworkPropertyMetadata(OnSelectedObjectsChanged));

        private static void OnSelectedObjectsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PropertyGrid)d).OnSelectedObjectsChanged();
        }

        bool _inSelectedObjectsChange;

        private void OnSelectedObjectsChanged()
        {
            try
            {
                if (!_inSelectedObjectsChange)
                {
                    _inSelectedObjectsChange = true;

                    if (SelectedObjects == null)
                    {
                        if (SelectedObject != null)
                        {
                            SelectedObject = null;
                        }
                    }
                    else if (SelectedObjects.Count == 0 && SelectedObject == null && _inSelectedObjectChange)
                    {
                        // cascade notification
                    }
                    else if (SelectedObjects.Count == 1 && ReferenceEquals(SelectedObjects[0], SelectedObject))
                    {
                        // we are getting a cascade notification from somebody setting SelectedObject
                        // -- shouldn't happen now we have the guard clause -- but belt and braces...
                    }
                    else
                    {
                        SelectedObject = new MultipleObjectWrapper(SelectedObjects) { OwnedByGrid = true };
                    }
                }
            }
            finally
            {
                _inSelectedObjectsChange = false;
            }
        }

        /// <summary>
        /// Specifies how the grid handles internal node data.
        /// This is an advanced setting.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public NodeClearanceMode NodeClearanceMode { get; set; }

        /// <summary>
        /// Forces the grid to reload the selected object.  This method should not be
        /// called in normal use.  It should be called only if you have dynamically added
        /// or removed a property (for example via a type converter or an
        /// ICustomTypeDescriptor implementation) since the object was loaded.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void Refresh()
        {
            if (IsExpandedStatePersistent && SelectedObject != null)
            {
                SaveExpandedStates(SelectedObject.GetType());
            }
            BindObject(SelectedObject);
        }

        private void BindObject(object source)
        {
            if (NodeClearanceMode == NodeClearanceMode.Aggressive)
            {
                foreach (Node node in Nodes)
                {
                    node.ClearContents();
                }
            }

            Nodes.Clear();

            BindObject(source, Nodes);
        }

        private void BindObject(object source, ICollection<Node> nodes)
        {
            if (source != null)
            {
                Node node = CreateHolderNode(source, true);
                node.CanDisplayReadOnlyProperties = CanDisplayReadOnlyProperties;
                foreach (Node child in node.Children)
                {
                    nodes.Add(child);
                }
            }
        }

        private bool ShowAsExpandable(Node property)
        {
            if (typeof(Many).IsAssignableFrom(property.PropertyType))
            {
                Debug.Assert(property.PropertyType.GetGenericTypeDefinition() == typeof(Many<>));
                Debug.Assert(property.PropertyType.GetGenericArguments().Length == 1);
                //Type manyType = property.PropertyType.GetGenericArguments()[0];
                //if (manyType.IsValueType)
                //{
                //    // We can't currently edit expanded individual fields of value members in a multi-select situation
                //    return false;
                //}

                Many many = (Many)(property.Value);
                if (many.IsConsistent)
                {
                    PropertyNode node = new PropertyNode(many, many.GetType().GetProperty("Value"), property.ChildFilter);
                    return _editorSelector.GetEditSettings(node).AllowExpand;
                }
            }

            InPlaceEditing inPlaceEditing = _editorSelector.GetEditSettings(property);
            return inPlaceEditing.AllowExpand;
        }

        #region IsExpandedStatePersistent Property

        /// <summary>
        /// Gets or sets whether or not the expanded state of property nodes are persistent.
        /// When set to true, property nodes will maintain their expanded state when the SelectedObject property changes.
        /// The default is false.
        /// This is a dependency property.
        /// </summary>
        /// <remarks>
        /// <strong>Dependency Property Information</strong>
        /// <table>
        ///   <tr><td>Identifier field</td><td><see cref="IsExpandedStatePersistentProperty"/></td></tr>
        ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
        /// </table>
        /// </remarks>
        public bool IsExpandedStatePersistent
        {
            get { return (bool)GetValue(IsExpandedStatePersistentProperty); }
            set { SetValue(IsExpandedStatePersistentProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="IsExpandedStatePersistent"/> property.
        /// </summary>
        public static readonly DependencyProperty IsExpandedStatePersistentProperty =
          DependencyProperty.Register("IsExpandedStatePersistent", typeof(bool), typeof(PropertyGrid),
          new FrameworkPropertyMetadata(false, OnIsExpandedStatePersistentChanged));

        private static void OnIsExpandedStatePersistentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PropertyGrid)d).OnIsExpandedStatePersistentChanged();
        }

        private void OnIsExpandedStatePersistentChanged()
        {
        }

        #endregion // IsExpandedStatePersistent Property

        #endregion

        #region Generic data model for top-level nodes


        /// <summary>
        /// Gets or sets the set of top-level nodes in the grid.
        /// This is a dependency property.
        /// </summary>
        /// <remarks>
        /// <strong>Dependency Property Information</strong>
        /// <table>
        ///   <tr><td>Identifier field</td><td><see cref="NodesProperty"/></td></tr>
        ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
        /// </table>
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Following the normal pattern for dependency properties")]
        public ObservableCollection<Node> Nodes
        {
            get { return (ObservableCollection<Node>)GetValue(NodesProperty); }
            set { SetValue(NodesProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Nodes"/> property.
        /// </summary>
        public static readonly DependencyProperty NodesProperty =
            DependencyProperty.Register("Nodes", typeof(ObservableCollection<Node>),
            typeof(PropertyGrid));

        /// <summary>
        /// Gets or sets a collection used to generate the content of the <see cref="PropertyGrid"/>.
        /// This is a dependency property.
        /// </summary>
        /// <remarks>
        /// <strong>Dependency Property Information</strong>
        /// <table>
        ///   <tr><td>Identifier field</td><td><see cref="ItemsSourceProperty"/></td></tr>
        ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
        /// </table>
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Following the normal pattern for ItemsSource properties")]
        public IDictionary ItemsSource
        {
            get { return (IDictionary)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ItemsSource"/> property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IDictionary), typeof(PropertyGrid),
            new UIPropertyMetadata(OnItemsSourceChanged));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PropertyGrid grid = d as PropertyGrid;
            if (grid != null)
            {
                if (e.NewValue == null)
                {
                    grid.BindingView = new PropertyGridBindingView(grid.Nodes);
                }
                else
                {
                    object source = e.NewValue;
                    Node node = grid.CreateHolderNode(source, true);
                    grid.BindingView = new PropertyGridBindingView(node.Children);
                }
                grid.OnGroupingChanged();
                grid.OnSortingChanged();
            }
        }

        /// <summary>
        /// Adds a top-level node to the grid.  The node data is managed by the grid.
        /// </summary>
        /// <param name="caption">The caption or name of the node.</param>
        /// <param name="value">The initial value of the node.</param>
        /// <returns>The <see cref="Node"/> object added to the grid.</returns>
        /// <remarks>The user will be able to edit the node value using the default editor
        /// for the type of value.</remarks>
        public Node AddNode(string caption, object value)
        {
            Invariant.ArgumentNotEmpty(caption, "caption");
            Invariant.ArgumentNotNull(value, "value");

            Node node = CreateHolderNode(value, caption, false);
            Nodes.Add(node);
            return node;
        }

        /// <summary>
        /// Adds a top-level node to the grid.  The node data is managed by the grid.
        /// </summary>
        /// <param name="caption">The caption or name of the node.</param>
        /// <param name="value">The initial value of the node.</param>
        /// <param name="inPlaceEditor">The editor to be used for editing the value.</param>
        /// <returns>The <see cref="Node"/> object added to the grid.</returns>
        /// <remarks>The user will be able to edit the node value using the editor specified by
        /// the inPlaceEditor parameter.  This overrides any type editors specified in <see cref="Editors"/>
        /// collection.</remarks>
        public Node AddNode(string caption, object value, NodeEditor inPlaceEditor)
        {
            Invariant.ArgumentNotEmpty(caption, "caption");
            Invariant.ArgumentNotNull(value, "value");
            Invariant.ArgumentNotNull(inPlaceEditor, "inPlaceEditor");

            Node node = CreateHolderNode(value, caption, inPlaceEditor, false);
            Nodes.Add(node);
            return node;
        }

        /// <summary>
        /// Adds a top-level node to the grid.  The node data is managed by the grid.
        /// </summary>
        /// <param name="caption">The caption or name of the node.</param>
        /// <param name="value">The initial value of the node.</param>
        /// <param name="inPlaceEditorTemplate">A <see cref="DataTemplate"/> specifying the template
        /// to be used for presenting and editing the node value.</param>
        /// <returns>The <see cref="Node"/> object added to the grid.</returns>
        /// <remarks>The node value will be presented to the user using the data template specified.
        /// To support editing, the data template must accept user input and support two-way
        /// binding.  The property to which the data template should bind is called Value.</remarks>
        public Node AddNode(string caption, object value, DataTemplate inPlaceEditorTemplate)
        {
            Invariant.ArgumentNotNull(inPlaceEditorTemplate, "inPlaceEditorTemplate");

            StaticNodeEditor inPlaceEditor = new StaticNodeEditor();
            inPlaceEditor.EditorTemplate = inPlaceEditorTemplate;

            return AddNode(caption, value, inPlaceEditor);
        }

        /// <summary>
        /// Adds a top-level node to the grid.  The node data is managed by the grid.
        /// </summary>
        /// <param name="caption">The caption or name of the node.</param>
        /// <param name="value">The initial value of the node.</param>
        /// <param name="inPlaceEditorKey">A resource key for the <see cref="DataTemplate"/>
        /// to be used for presenting and editing the node value.</param>
        /// <returns>The <see cref="Node"/> object added to the grid.</returns>
        /// <remarks>The node value will be presented to the user using the data template specified.
        /// To support editing, the data template must accept user input and support two-way
        /// binding.  The property to which the data template should bind is called Value.</remarks>
        public Node AddNode(string caption, object value, object inPlaceEditorKey)
        {
            Invariant.ArgumentNotEmpty(caption, "caption");
            Invariant.ArgumentNotNull(value, "value");

            DynamicNodeEditor inPlaceEditor = new DynamicNodeEditor();
            inPlaceEditor.EditorTemplateKey = inPlaceEditorKey;

            return AddNode(caption, value, inPlaceEditor);
        }

        /// <summary>
        /// Adds a top-level node to the grid.  The node represents a property of an object
        /// that is not managed by the grid.
        /// </summary>
        /// <param name="source">The object whose property is to be added to the grid.</param>
        /// <param name="propertyName">The name of the property to be added to the grid.</param>
        /// <returns>The <see cref="Node"/> object added to the grid.</returns>
        public Node AddPropertyNode(object source, string propertyName)
        {
            return AddPropertyNode(source, propertyName, null);
        }

        /// <summary>
        /// Adds a top-level node to the grid.  The node represents a property of an object
        /// that is not managed by the grid.
        /// </summary>
        /// <param name="source">The object whose property is to be added to the grid.</param>
        /// <param name="propertyName">The name of the property to be added to the grid.</param>
        /// <param name="caption">The display name for the property.</param>
        /// <returns>The <see cref="Node"/> object added to the grid.</returns>
        public Node AddPropertyNode(object source, string propertyName, string caption)
        {
            Invariant.ArgumentNotNull(source, "source");
            Invariant.ArgumentNotEmpty(propertyName, "propertyName");

            PropertyInfo property = source.GetType().GetProperty(propertyName);  // TODO: or use TypeDescriptor???
            PropertyNode propertyNode = new PropertyNode(source, caption, property, ShowAsExpandable);
            Nodes.Add(propertyNode);
            return propertyNode;

        }

        /// <summary>
        /// Removes a top-level node from the grid.
        /// </summary>
        /// <param name="node">The node to remove from the grid.</param>
        public void RemoveNode(Node node)
        {
            Nodes.Remove(node);
        }

        #endregion

        /// <summary>
        /// Gets or sets the <see cref="PropertyGridBindingView"/> representing the view state
        /// of the grid.  This is a dependency property.
        /// </summary>
        /// <remarks>
        /// <strong>Dependency Property Information</strong>
        /// <table>
        ///   <tr><td>Identifier field</td><td><see cref="BindingViewProperty"/></td></tr>
        ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
        /// </table>
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Following pattern for dependency properties")]
        public PropertyGridBindingView BindingView
        {
            get { return (PropertyGridBindingView)GetValue(BindingViewProperty); }
            set { SetValue(BindingViewProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="BindingView"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BindingViewProperty =
            DependencyProperty.Register("BindingView", typeof(PropertyGridBindingView), typeof(PropertyGrid));

        #region XAML- and style-based sorting and grouping support

        /// <summary>
        /// Gets or sets property grouping settings.
        /// This is a dependency property.
        /// </summary>
        /// <remarks>
        /// <para>This property supports only single-level grouping, as this is all that is usually required
        /// for the <see cref="PropertyGrid"/>.  If multi-level grouping is required, bypass this property and
        /// use the BindingView.DefaultView.GroupDescriptions collection instead.</para>
        /// <para>If both the BindingView.DefaultView.GroupDescriptions collection and the PropertyGrid.Grouping property
        /// are set, the last one to be set takes precedence.  If BindingView.DefaultView.GroupDescriptions is
        /// modified after PropertyGrid.Grouping has been set, PropertyGrid.Grouping continues to reflect its previous
        /// value (even though this value is ignored in favour of the collection).</para>
        /// <para>The <see cref="PropertyGrouping"/> class provides implementations of common grouping strategies.</para>
        /// <strong>Dependency Property Information</strong>
        /// <table>
        ///   <tr><td>Identifier field</td><td><see cref="GroupingProperty"/></td></tr>
        ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
        /// </table>
        /// </remarks>
        public GroupDescription Grouping
        {
            get { return (GroupDescription)GetValue(GroupingProperty); }
            set { SetValue(GroupingProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Grouping"/> property.
        /// </summary>
        public static readonly DependencyProperty GroupingProperty =
          DependencyProperty.Register("Grouping", typeof(GroupDescription), typeof(PropertyGrid),
          new FrameworkPropertyMetadata(OnGroupingChanged));

        private static void OnGroupingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PropertyGrid)d).OnGroupingChanged();
        }

        private void OnGroupingChanged()
        {
            using (BindingView.DefaultView.DeferRefresh())
            {
                BindingView.DefaultView.GroupDescriptions.Clear();
                if (Grouping != null)
                {
                    BindingView.DefaultView.GroupDescriptions.Add(Grouping);
                }
            }
        }

        /// <summary>
        /// Gets or sets property sorting order.
        /// This is a dependency property.
        /// </summary>
        /// <remarks>
        /// <para>If both the BindingView.DefaultView.CustomSort property and the PropertyGrid.Sorting property
        /// are set, the last one to be set takes precedence.  If BindingView.DefaultView.CustomSort is
        /// modified after PropertyGrid.Sorting has been set, PropertyGrid.Sorting continues to reflect its previous
        /// value (even though this value is ignored).  Additionally, setting PropertyGrid.Sorting clears any
        /// previously set BindingView.DefaultView.SortDescriptions value.</para>
        /// <para>Changing this setting while the grid is displayed collapses any expanded properties.</para>
        /// <para>The <see cref="PropertySorting"/> class provides implementations of common sorting strategies.</para>
        /// <strong>Dependency Property Information</strong>
        /// <table>
        ///   <tr><td>Identifier field</td><td><see cref="SortingProperty"/></td></tr>
        ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
        /// </table>
        /// </remarks>
        public IComparer Sorting
        {
            get { return (IComparer)GetValue(SortingProperty); }
            set { SetValue(SortingProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Sorting"/> property.
        /// </summary>
        public static readonly DependencyProperty SortingProperty =
          DependencyProperty.Register("Sorting", typeof(IComparer), typeof(PropertyGrid),
          new FrameworkPropertyMetadata(OnSortingChanged));

        private static void OnSortingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PropertyGrid)d).OnSortingChanged();
        }

        private void OnSortingChanged()
        {
            ApplySorting();
        }

        private void ApplySorting()
        {
            ListCollectionView view = (ListCollectionView)(BindingView.DefaultView);
            view.CustomSort = Sorting;
        }

        /// <summary>
        /// Gets or sets whether the <see cref="Sorting"/> setting should be applied to subproperties.
        /// This is a dependency property.
        /// </summary>
        /// <remarks>
        /// <para>The default value is true.</para>
        /// <para>Changing this setting while the grid is displayed collapses any expanded properties.</para>
        /// <strong>Dependency Property Information</strong>
        /// <table>
        ///   <tr><td>Identifier field</td><td><see cref="SortSubpropertiesProperty"/></td></tr>
        ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
        /// </table>
        /// </remarks>
        public bool SortSubproperties
        {
            get { return (bool)GetValue(SortSubpropertiesProperty); }
            set { SetValue(SortSubpropertiesProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="SortSubproperties"/> property.
        /// </summary>
        public static readonly DependencyProperty SortSubpropertiesProperty =
          DependencyProperty.Register("SortSubproperties", typeof(bool), typeof(PropertyGrid),
          new FrameworkPropertyMetadata(true, OnSortSubpropertiesChanged));

        private static void OnSortSubpropertiesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PropertyGrid)d).OnSortSubpropertiesChanged();
        }

        private void OnSortSubpropertiesChanged()
        {
            ApplySorting();
        }

        private void OnTreeViewItemExpanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem tvi = (TreeViewItem)(e.OriginalSource);
            PropertyGridRow row = (PropertyGridRow)(tvi.Header);

            ListCollectionView childrenView = (ListCollectionView)(row.Children.DefaultView);
            childrenView.CustomSort = SortSubproperties ? Sorting : null;

            Node node = row.Node;
            if (node.Property.PropertyType.IsValueType)
            {
                node.RefreshChildren();
            }
            Dispatcher.BeginInvoke(new Action(AutoSizeColumns), DispatcherPriority.Loaded);
        }

        private void OnTreeViewItemCollapsed(object sender, RoutedEventArgs e)
        {
            TreeViewItem tvi = (TreeViewItem)(e.OriginalSource);
            PropertyGridRow row = (PropertyGridRow)(tvi.Header);
            Node node = row.Node;
            //node.RefreshChildren();
            Dispatcher.BeginInvoke(new Action(AutoSizeColumns), DispatcherPriority.Loaded);
        }

        #endregion

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> used to display property names.
        /// This is a dependency property.
        /// </summary>
        /// <remarks>
        /// <para>The template is responsible for including suitable expand/collapse UI if
        /// required; this is necessary to support full replacement of the expand/collapse UI.
        /// The data template receives a <see cref="PropertyGridRow"/>.</para>
        /// <strong>Dependency Property Information</strong>
        /// <table>
        ///   <tr><td>Identifier field</td><td><see cref="PropertyNameTemplateProperty"/></td></tr>
        ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
        /// </table>
        /// </remarks>
        public DataTemplate PropertyNameTemplate
        {
            get { return (DataTemplate)GetValue(PropertyNameTemplateProperty); }
            set { SetValue(PropertyNameTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="PropertyNameTemplate"/> property.
        /// </summary>
        public static readonly DependencyProperty PropertyNameTemplateProperty =
          DependencyProperty.Register("PropertyNameTemplate", typeof(DataTemplate), typeof(PropertyGrid));

        /// <summary>
        /// Gets or sets the data template for tooltips shown when the user hovers over a
        /// property name.
        /// This is a dependency property.
        /// </summary>
        /// <remarks>
        /// <para>Tooltips are shown only if ToolTipService.IsEnabled is set on the grid.
        /// The data template receives a <see cref="PropertyGridRow"/>.</para>
        /// <strong>Dependency Property Information</strong>
        /// <table>
        ///   <tr><td>Identifier field</td><td><see cref="PropertyNameToolTipTemplateProperty"/></td></tr>
        ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
        /// </table>
        /// </remarks>
        public DataTemplate PropertyNameToolTipTemplate
        {
            get { return (DataTemplate)GetValue(PropertyNameToolTipTemplateProperty); }
            set { SetValue(PropertyNameToolTipTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="PropertyNameToolTipTemplate"/> property.
        /// </summary>
        public static readonly DependencyProperty PropertyNameToolTipTemplateProperty =
          DependencyProperty.Register("PropertyNameToolTipTemplate", typeof(DataTemplate), typeof(PropertyGrid));

        #region ExpanderMode Property

        /// <summary>
        /// Gets or sets the <see cref="ExpanderMode"/> that specifies the inital expand/collapsed state of items in the <see cref="PropertyGrid"/>.
        /// The default is collapsed.
        /// This is a dependency property.
        /// </summary>
        /// <remarks>
        /// <strong>Dependency Property Information</strong>
        /// <table>
        ///   <tr><td>Identifier field</td><td><see cref="ExpanderModeProperty"/></td></tr>
        ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
        /// </table>
        /// </remarks>
        public ExpanderMode ExpanderMode
        {
            get { return (ExpanderMode)GetValue(ExpanderModeProperty); }
            set { SetValue(ExpanderModeProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ExpanderMode"/> property.
        /// </summary>
        public static readonly DependencyProperty ExpanderModeProperty =
          DependencyProperty.Register("ExpanderMode", typeof(ExpanderMode), typeof(PropertyGrid),
          new FrameworkPropertyMetadata(ExpanderMode.Collapsed, OnExpanderModeChanged));

        private static void OnExpanderModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PropertyGrid)d).OnExpanderModeChanged();
        }

        private void OnExpanderModeChanged()
        {
        }

        #endregion // ExpanderMode Property

        #region GroupExpanderMode Property

        /// <summary>
        /// Gets or sets the initial expand/collapse state of the category groups. The default is expanded.
        /// This is a dependency property.
        /// </summary>
        /// <remarks>
        /// <strong>Dependency Property Information</strong>
        /// <table>
        ///   <tr><td>Identifier field</td><td><see cref="GroupExpanderModeProperty"/></td></tr>
        ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
        /// </table>
        /// </remarks>
        public ExpanderMode GroupExpanderMode
        {
            get { return (ExpanderMode)GetValue(GroupExpanderModeProperty); }
            set { SetValue(GroupExpanderModeProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="GroupExpanderMode"/> property.
        /// </summary>
        public static readonly DependencyProperty GroupExpanderModeProperty =
          DependencyProperty.Register("GroupExpanderMode", typeof(ExpanderMode), typeof(PropertyGrid),
          new FrameworkPropertyMetadata(ExpanderMode.Expanded, OnGroupExpanderModeChanged));

        private static void OnGroupExpanderModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PropertyGrid)d).OnGroupExpanderModeChanged();
        }

        private void OnGroupExpanderModeChanged()
        {
        }

        #endregion // GroupExpanderMode Property

        /// <summary>
        /// Collapses all the category groups in the <see cref="PropertyGrid"/>.
        /// </summary>
        public void CollapseGroups()
        {
            IList<Expander> expanders = VisualTreeUtils.GetChildren<Expander>(this);
            foreach (Expander expander in expanders)
            {
                expander.IsExpanded = false;
            }
        }

        /// <summary>
        /// Expand all the category groups in the <see cref="PropertyGrid"/>.
        /// </summary>
        public void ExpandGroups()
        {
            IList<Expander> expanders = VisualTreeUtils.GetChildren<Expander>(this);
            foreach (Expander expander in expanders)
            {
                expander.IsExpanded = true;
            }
        }

        #region Helper class and factory method for realising top-level objects as property nodes

        internal class Holder<T>
        {
            public Holder(T obj)
            {
                Value = obj;
            }

            public T Value { get; set; }
        }

        internal struct HolderInfo
        {
            private object _holder;
            private PropertyInfo _holdingProperty;

            internal object Holder
            {
                get { return _holder; }
                set { _holder = value; }
            }

            internal PropertyInfo HoldingProperty
            {
                get { return _holdingProperty; }
                set { _holdingProperty = value; }
            }
        }

        private static HolderInfo CreateHolder(object obj)
        {
            Type valueType = obj.GetType();
            Type wrapperType = typeof(Holder<>);
            Type closedConstructedWrapperType = wrapperType.MakeGenericType(valueType);

            HolderInfo holderInfo = new HolderInfo();
            holderInfo.Holder = Activator.CreateInstance(closedConstructedWrapperType, obj);
            holderInfo.HoldingProperty = closedConstructedWrapperType.GetProperty("Value");

            return holderInfo;
        }

        private Node CreateHolderNode(object source, bool alwaysExpand)
        {
            HolderInfo holder = CreateHolder(source);
            return new PropertyNode(holder.Holder, holder.HoldingProperty, ShowAsExpandable) { AlwaysExpand = alwaysExpand };
        }

        private Node CreateHolderNode(object source, string caption, bool alwaysExpand)
        {
            HolderInfo holder = CreateHolder(source);
            return new PropertyNode(holder.Holder, caption, holder.HoldingProperty, ShowAsExpandable) { AlwaysExpand = alwaysExpand };
        }

        private Node CreateHolderNode(object source, string caption, NodeEditor inPlaceEditor, bool alwaysExpand)
        {
            HolderInfo holder = CreateHolder(source);
            return new PropertyNode(holder.Holder, caption, holder.HoldingProperty, ShowAsExpandable, inPlaceEditor) { AlwaysExpand = alwaysExpand };
        }

        #endregion

        #endregion

        // TODO: these keys are used by both the PRopertyGrid and the DataGrid. Should they be moved into a seperate class?

        #region ComponentResourceKeys

        /// <summary>
        /// Gets the <see cref="ResourceKey"/> for the <see cref="BuiltInEditorStyleCollection"/> containing
        /// default styles for all built in controls.
        /// </summary>
        public static ComponentResourceKey BuiltInEditorStyleCollectionKey
        {
            get { return new ComponentResourceKey(typeof(PropertyGrid), "BuiltInEditorStyleCollection"); }
        }

        /// <summary>
        /// Gets the <see cref="ResourceKey"/> for the <see cref="DataTemplate"/> that
        /// presents property names.
        /// </summary>
        public static ComponentResourceKey PropertyNameTemplateKey
        {
            get { return new ComponentResourceKey(typeof(PropertyGrid), "PropertyNameTemplate"); }
        }

        /// <summary>
        /// Gets the <see cref="ResourceKey"/> for the <see cref="Style"/> that
        /// tracks the current selection in templates based on a <see cref="TreeListView"/>.
        /// </summary>
        public static ComponentResourceKey SelectionTrackingStyleKey
        {
            get { return new ComponentResourceKey(typeof(PropertyGrid), "SelectionTrackingStyle"); }
        }

        /// <summary>
        /// Gets the <see cref="ResourceKey"/> for the <see cref="DataTemplate"/> that
        /// presents non-editable values.
        /// </summary>
        public static ComponentResourceKey ReadOnlyDisplayKey
        {
            get { return new ComponentResourceKey(typeof(PropertyGrid), "ReadOnlyDisplay"); }
        }

        /// <summary>
        /// Gets the <see cref="ResourceKey"/> for the <see cref="DataTemplate"/> that
        /// presents collections.
        /// </summary>
        public static ComponentResourceKey CollectionDisplayKey
        {
            get { return new ComponentResourceKey(typeof(PropertyGrid), "CollectionDisplay"); }
        }

        /// <summary>
        /// Gets the <see cref="ResourceKey"/> for the <see cref="DataTemplate"/> that
        /// presents collection elements.
        /// </summary>
        public static ComponentResourceKey CollectionElementEditorKey
        {
            get { return new ComponentResourceKey(typeof(PropertyGrid), "CollectionElementEditor"); }
        }

        /// <summary>
        /// Gets the <see cref="ResourceKey"/> for the <see cref="DataTemplate"/> that
        /// edits values as plain text.
        /// </summary>
        public static ComponentResourceKey SimpleTextEditorKey
        {
            get { return new ComponentResourceKey(typeof(PropertyGrid), "SimpleTextEditor"); }
        }

        /// <summary>
        /// Gets the <see cref="ResourceKey"/> for the <see cref="DataTemplate"/> that
        /// edits values as plain text and updates the value only when it loses focus.
        /// </summary>
        public static ComponentResourceKey NonAutoUpdatingTextEditorKey
        {
            get { return new ComponentResourceKey(typeof(PropertyGrid), "NonAutoUpdatingTextEditor"); }
        }

        /// <summary>
        /// Gets the <see cref="ResourceKey"/> for the <see cref="DataTemplate"/> that
        /// edits values by selecting them from a drop-down list or typing a value.
        /// </summary>
        public static ComponentResourceKey ListSelectEditorKey
        {
            get { return new ComponentResourceKey(typeof(PropertyGrid), "ListSelectEditor"); }
        }

        /// <summary>
        /// Gets the <see cref="ResourceKey"/> for the <see cref="DataTemplate"/> that
        /// edits values by selecting them from a drop-down list and does not permit typing
        /// of arbitrary values.
        /// </summary>
        public static ComponentResourceKey ListSelectNoTextEntryEditorKey
        {
            get { return new ComponentResourceKey(typeof(PropertyGrid), "ListSelectNoTextEntryEditor"); }
        }

        /// <summary>
        /// Gets the <see cref="ResourceKey"/> for the <see cref="DataTemplate"/> that
        /// edits values by selecting them from a group of radio buttons.
        /// </summary>
        public static ComponentResourceKey RadioSelectEditorKey
        {
            get { return new ComponentResourceKey(typeof(PropertyGrid), "RadioSelectEditor"); }
        }

        /// <summary>
        /// Gets the <see cref="ResourceKey"/> for the <see cref="DataTemplate"/> that
        /// edits boolean values using a check box.
        /// </summary>
        public static ComponentResourceKey CheckBoxEditorKey
        {
            get { return new ComponentResourceKey(typeof(PropertyGrid), "CheckBoxEditor"); }
        }

        /// <summary>
        /// Gets the <see cref="ResourceKey"/> for the <see cref="DataTemplate"/> that
        /// edits numeric values using a slider.
        /// </summary>
        public static ComponentResourceKey SliderEditorKey
        {
            get { return new ComponentResourceKey(typeof(PropertyGrid), "SliderEditor"); }
        }

        /// <summary>
        /// Gets the <see cref="ResourceKey"/> for the <see cref="DataTemplate"/> that
        /// edits numeric values using a text box and a pair of up-down buttons.
        /// </summary>
        public static ComponentResourceKey NumericUpDownEditorKey
        {
            get { return new ComponentResourceKey(typeof(PropertyGrid), "NumericUpDownEditor"); }
        }

        /// <summary>
        /// Gets the <see cref="ResourceKey"/> for the <see cref="DataTemplate"/> that
        /// edits dates using a calendar display.
        /// </summary>
        public static ComponentResourceKey DateEditorKey
        {
            get { return new ComponentResourceKey(typeof(PropertyGrid), "DateEditor"); }
        }

        /// <summary>
        /// Gets the <see cref="ResourceKey"/> for the <see cref="DataTemplate"/> that
        /// edits duration values using a time span picker.
        /// </summary>
        public static ComponentResourceKey TimeSpanEditorKey
        {
            get { return new ComponentResourceKey(typeof(PropertyGrid), "TimeSpanEditor"); }
        }

        /// <summary>
        /// Gets the <see cref="ResourceKey"/> for the <see cref="DataTemplate"/> that
        /// edits colors using a color picker.
        /// </summary>
        public static ComponentResourceKey ColorEditorKey
        {
            get { return new ComponentResourceKey(typeof(PropertyGrid), "ColorEditor"); }
        }

        /// <summary>
        /// Gets the <see cref="ResourceKey"/> for the <see cref="DataTemplate"/> that
        /// edits inconsistent values.
        /// </summary>
        public static ComponentResourceKey ManyEditorKey
        {
            get { return new ComponentResourceKey(typeof(PropertyGrid), "ManyEditor"); }
        }

        /// <summary>
        /// Gets the <see cref="ResourceKey"/> for the <see cref="DataTemplate"/> that
        /// edits integer values.
        /// </summary>
        public static ComponentResourceKey IntegerEditorKey
        {
            get { return new ComponentResourceKey(typeof(PropertyGrid), "IntegerEditor"); }
        }

        /// <summary>
        /// Gets the <see cref="ResourceKey"/> for the <see cref="DataTemplate"/> that
        /// edits double values.
        /// </summary>
        public static ComponentResourceKey DoubleEditorKey
        {
            get { return new ComponentResourceKey(typeof(PropertyGrid), "DoubleEditor"); }
        }

        #endregion
    }
}