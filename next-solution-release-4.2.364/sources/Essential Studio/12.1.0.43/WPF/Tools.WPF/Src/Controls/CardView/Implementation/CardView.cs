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
using System.Collections;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using Syncfusion.Windows.Tools.Controls;
using System.Reflection;
using System.ComponentModel;
using Syncfusion.Windows.Shared;
using System.Data;
using System.Diagnostics;
using System.Threading;
using Syncfusion.Licensing;


namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:CardViewControl"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:CardViewControl;assembly=CardViewControl"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:CustomControl1/>
    ///
    /// </summary>
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
       Type = typeof(CardView), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/CardView/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(CardView), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/CardView/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(CardView), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/CardView/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
    Type = typeof(CardView), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/CardView/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(CardView), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/CardView/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(CardView), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/CardView/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
   Type = typeof(CardView), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/CardView/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
  Type = typeof(CardView), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/CardView/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default ,
 Type = typeof(CardView), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/CardView/Themes/Generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
Type = typeof(CardView), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/CardView/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
Type = typeof(CardView), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/CardView/Themes/TransparentStyle.xaml")]  
    public class CardView : ItemsControl
    {
        static CardView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CardView), new FrameworkPropertyMetadata(typeof(CardView)));
        }

        public CardView()
        {
            NormalSort = new RoutedCommand("NormalSort", typeof(CardView));
            Sort = new RoutedCommand("Sort", typeof(CardView));
            ClearFilter = new RoutedCommand("ClearFilter", typeof(CardView));
            CommandBindings.Add(new CommandBinding(Sort, new ExecutedRoutedEventHandler(ExecuteSort), new CanExecuteRoutedEventHandler(CanExecuteSort)));
            CommandBindings.Add(new CommandBinding(NormalSort, new ExecutedRoutedEventHandler(ExecuteNormalSort), new CanExecuteRoutedEventHandler(CanExecuteNormalSort)));
            CommandBindings.Add(new CommandBinding(ClearFilter, new ExecutedRoutedEventHandler(ExecuteClearFilter), new CanExecuteRoutedEventHandler(CanExecuteClearFilter)));
            mgeneric = new ResourceDictionary();
            mgeneric.Source = new Uri("/Syncfusion.Tools.WPF;component/Controls/CardView/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);
            GroupboxCollection = new ObservableCollection<GroupInfo>();
#if WPF
            if (EnvironmentTestTools.IsSecurityGranted)
            {
                EnvironmentTestTools.StartValidateLicense(typeof(CardView));
            }
#endif
        }

        private RoutedCommand NoramlsortItem;

        private RoutedCommand sortItem;

        private RoutedCommand clearFilter;

        private Type objectType;

        SortDescription Normaldesc;

        SortDescription remdescription;

        public RoutedCommand NormalSort
        {
            get
            {
                return NoramlsortItem;
            }

            set
            {
                NoramlsortItem = value;
            }
        }        

        public RoutedCommand Sort
        {
            get
            {
                return sortItem;
            }

            set
            {
                sortItem = value;
            }
        }

        public RoutedCommand ClearFilter
        {
            get
            {
                return clearFilter;
            }
            set
            {
                clearFilter = value;
            }
        }

        private void CanExecuteClearFilter(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        
        private void ExecuteClearFilter(object sender, ExecutedRoutedEventArgs e)
        {
            if (_collectionView == null)
            {
                _collectionView = CollectionViewSource.GetDefaultView(ItemsSource) as ICollectionView;
            }
            if (checkedListBox != null && checkedListBox.SelectedItems.Count > 0)
            {
                 checkedListBox.SelectedItems.Clear();                 
            }
            FilterCards();
          
        }

      
        private void CanExecuteNormalSort(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }                

        private void ExecuteNormalSort(object sender, ExecutedRoutedEventArgs e)
        {

            if (CanSort == true)
            {
                SortButton btn = e.OriginalSource as SortButton;
                GroupInfo info = btn.DataContext as GroupInfo;
                string columnName = info.Name;

                if (_collectionView == null)
                {
                    _collectionView = CollectionViewSource.GetDefaultView(ItemsSource) as ICollectionView;
                }

                Normaldesc = new SortDescription();

                bool notfound = false;

                foreach (SortDescription description in _collectionView.SortDescriptions)
                {
                    if (description.PropertyName == columnName)
                    {
                        Normaldesc = description;
                        notfound = false;
                        break;
                    }
                    notfound = true;
                }

                if (notfound || _collectionView.SortDescriptions.Count == 0)
                {
                    _collectionView.SortDescriptions.Add(new SortDescription(columnName, ListSortDirection.Ascending));
                    btn.SortButtonState = SortingDirection.Ascending;

                }
                else
                {
                    if (Normaldesc.Direction == ListSortDirection.Ascending)
                    {
                        
                        _collectionView.SortDescriptions.Remove(Normaldesc);                       
                        _collectionView.SortDescriptions.Add(new SortDescription(columnName, ListSortDirection.Descending));
                        btn.SortButtonState = SortingDirection.Descending;
                    }
                    else
                    {
                        _collectionView.SortDescriptions.Remove(Normaldesc);
                        btn.SortButtonState = SortingDirection.None;
                    }
                }
                
                

            }
        }        

        private void CanExecuteSort(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
               
        private void ExecuteSort(object sender, ExecutedRoutedEventArgs e)
        {
            
            if (CanSort == true)
            {
                ToggleButton btn = e.OriginalSource as ToggleButton;
                GroupInfo info = btn.DataContext as GroupInfo;
                string columnName = info.Name;

                if (_collectionView == null)
                {
                    _collectionView = CollectionViewSource.GetDefaultView(ItemsSource) as ICollectionView;
                }
                
                remdescription = new SortDescription();

                bool notfound = false;

                foreach (SortDescription description in _collectionView.SortDescriptions)
                {
                    if (description.PropertyName == columnName)
                    {
                        remdescription = description; 
                        notfound = false;
                        break;
                    }
                    notfound = true;
                }

                if (notfound || _collectionView.SortDescriptions.Count == 0)
                {
                    _collectionView.SortDescriptions.Add(new SortDescription(columnName, ListSortDirection.Ascending));
                    
                }

                else
                {                    
                    if (remdescription.Direction == ListSortDirection.Ascending)
                    {                       
                        _collectionView.SortDescriptions.Remove(remdescription);                      
                         collectionindex = GroupboxCollection.IndexOf(info);
                        _collectionView.SortDescriptions.Insert(collectionindex, new SortDescription(columnName, ListSortDirection.Descending));                      
                    }
                    else
                    {                     
                        collectionindex = GroupboxCollection.IndexOf(info);                     
                        _collectionView.SortDescriptions.RemoveAt(collectionindex);
                    }
                }
            }
        }
     
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();            
            groupHost = GetTemplateChild("PART_GroupHost") as ItemsControl;
            groupbox = GetTemplateChild("GroupBox") as ListBox;
            grouppanel = GetTemplateChild("GroupPanel") as Grid;            
            cardViewItem = GetTemplateChild("PART_NewItem") as CardViewItem;
            filterPopup = GetTemplateChild("PART_FilterPopup") as Popup;
            grouppanel.Drop += new DragEventHandler(grouppanel_Drop);
            group = GetTemplateChild("Group") as ListBox;
            groupbox.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(groupbox_PreviewMouseLeftButtonDown);
            groupbox.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(groupbox_PreviewMouseLeftButtonUp);
            group.PreviewMouseMove += new MouseEventHandler(group_PreviewMouseMove);
            group.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(group_PreviewMouseLeftButtonDown);
            group.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(group_PreviewMouseLeftButtonUp);
            groupbox.PreviewMouseMove += new MouseEventHandler(groupbox_PreviewMouseMove);
            DataTemplate temp = group.ItemTemplate;
            //string s = SkinStorage.GetVisualStyle(this).ToString();
            //ResourceDictionary rs = new ResourceDictionary();
            //if (s == "Default")
            //    rs.Source = new Uri("/Syncfusion.Tools.WPF;component/Controls/CardView/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);
            //else
            //    rs.Source = new Uri("/Syncfusion.Tools.WPF;component/Controls/CardView/Themes/" + s + "Style.xaml", UriKind.RelativeOrAbsolute);
            //GroupStyle groupStyle = rs["GroupStyle"] as GroupStyle;
            //GroupStyle.Clear();
            //GroupStyle.Add(groupStyle);
        }

                
              
        void groupbox_PreviewMouseMove(object sender, MouseEventArgs e)
        {

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ListBoxItem item = VisualUtils.FindAncestor(e.OriginalSource as Visual, typeof(ListBoxItem)) as ListBoxItem;
                Point position, windowpos; Rect rec;
                GeneralTransform transform = groupbox.TransformToVisual(this);
                Point point = transform.Transform(new Point());
                Rect rect = new Rect(point.X, point.Y, groupbox.ActualWidth, groupbox.ActualHeight);                

                if (!(((Math.Abs((double)(currentPosition.X - e.GetPosition(null).X)) < 3) && (Math.Abs((double)(currentPosition.Y - e.GetPosition(null).Y)) < 3))))
                {
                    if (item != null)
                    {
                        if (visualbrush == null)
                        {
                            visualbrush = new VisualBrush();
                        }

                        if (borderbox == null)
                        {
                            borderbox = new Border();
                        }

                        visualbrush.Visual = item;
                        borderbox.Background = visualbrush;
                        position = new Point(Mouse.GetPosition(item).X, Mouse.GetPosition(item).Y);
                        windowpos = item.PointToScreen(position);
                        rec = new Rect(windowpos.X - item.ActualWidth / 2, windowpos.Y - item.ActualHeight * 1.3, item.ActualWidth, item.ActualHeight);
                        popup.Width = item.ActualWidth;
                        popup.Height = item.ActualHeight / 2;
                        popup.Child = borderbox;                        
                        popup.PlacementRectangle = rec;
                        popup.IsOpen = true;
                        
                        if (rect.Contains(e.GetPosition(this)))
                        {                           
                            Cursor = Cursors.No;                            
                        }                       
                        else 
                        {                           
                            Cursor = Cursors.Arrow;
                        }  
                        gi = item.Content as GroupInfo;
                    }
                }
            }            
        }                                

        void group_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {            
            popup.IsOpen = false;
            Cursor = Cursors.Arrow;
             GeneralTransform transform1 = groupbox.TransformToVisual(this);
                Point pt = transform1.Transform(new Point());
                Rect rec = new Rect(pt.X, pt.Y, groupbox.ActualWidth, groupbox.ActualHeight);
                groupname = gi;

                if (groupname != null && this.CanGroup)
                {
                    if (!groupbox.Items.Contains(GroupboxCollection))
                    {
                        if (!(((Math.Abs((double)(currentPosition.X - e.GetPosition(null).X)) < 3) && (Math.Abs((double)(currentPosition.Y - e.GetPosition(null).Y)) < 3))))
                        {
                            if (rec.Contains(e.GetPosition(this)))
                            {
                                GroupboxCollection.Add(groupname);                                                                                                                        
                                GroupCards(groupname.Name);                                
                            }
                        }
                    }
                }                        
        }
               
        void groupbox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            popup.IsOpen = false;             
            Cursor = Cursors.Arrow;
            GeneralTransform transform = groupbox.TransformToVisual(this);
            Point point = transform.Transform(new Point());
            Rect rect = new Rect(point.X, point.Y, groupbox.ActualWidth, groupbox.ActualHeight);
            ToggleButton btn = e.OriginalSource as ToggleButton;
           // GroupInfo info = btn.DataContext as GroupInfo;
            
            if (!rect.Contains(e.GetPosition(this)))
            {
               
                GroupboxCollection.Remove(gibox);

                foreach (PropertyGroupDescription description in _collectionView.GroupDescriptions)
                {
                    foreach (SortDescription sd in _collectionView.SortDescriptions)
                    {
                        if (gibox.Name == sd.PropertyName)
                        {
                            _collectionView.SortDescriptions.Remove(sd);
                            
                            goto sync;
                        }
                    }
                sync:
                        if (gibox.Name == description.PropertyName)
                        {
                            _collectionView.GroupDescriptions.Remove(description);
                            return;
                        }
                                       
                }
            }
        }                      

        void groupbox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {            
                currentPosition = e.GetPosition(null);
                ListBoxItem item = VisualUtils.FindAncestor(e.OriginalSource as Visual, typeof(ListBoxItem)) as ListBoxItem;
                
                if (item != null)
                {                    
                    gibox = item.Content as GroupInfo;
                }            
        }

        void cardViewItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (!(ItemsSource is CollectionView))
                {
                  
                }
                   
            }            
        }
                   
        private void group_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            currentPosition = e.GetPosition(null);
            ListBoxItem item = VisualUtils.FindAncestor(e.OriginalSource as Visual, typeof(ListBoxItem)) as ListBoxItem;
            if (item != null)
            {
                Popup pup = VisualUtils.FindDescendant(item as Visual, typeof(Popup)) as Popup;
                if (pup != null)
                {
                    checkedListBox = VisualUtils.FindDescendant(pup.Child as Visual, typeof(CheckListBox)) as CheckListBox;
                    if (checkedListBox != null)
                    {
                        checkedListBox.SelectionChanged -= new SelectionChangedEventHandler(checkedListBox_SelectionChanged);
                        checkedListBox.SelectionChanged += new SelectionChangedEventHandler(checkedListBox_SelectionChanged);                       
                    }
                }
            }
        }

        IList checkedFilters;
        GroupInfo ginfo;
        void checkedListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckListBox chbox = sender as CheckListBox;
            if (chbox != null)
            {
                checkedFilters = chbox.SelectedItems;
                ginfo = chbox.DataContext as GroupInfo;
                if (ginfo != null)
                {
                    ginfo.CheckedValues.Clear();
                    foreach (object items in chbox.SelectedItems)
                    {
                        ginfo.CheckedValues.Add(items);
                    }
                }
                
                FilterCards();
            }
        }

        internal void FilterCards()
        {
            if (_collectionView == null)
            {
                _collectionView = CollectionViewSource.GetDefaultView(ItemsSource) as ICollectionView;
            }

            if (checkedListBox != null)
            {
                _collectionView.Filter = new Predicate<object>(FilterItems);
            }
        }



        internal bool FilterItems(object item)
        {
            PropertyDescriptorCollection collection = TypeDescriptor.GetProperties(item);
            bool returnFlag = false;


            //var query = from PropertyDescriptor descriptor in collection
            //            where descriptor.Name == ginfo.Name
            //            select descriptor;         

            int checkedValues = 0;

            foreach (GroupInfo ginfo in GroupNames)
            {
                object selitem = (from PropertyDescriptor descriptor in collection
                                  where descriptor.Name == ginfo.Name
                                  select descriptor.GetValue(item)).FirstOrDefault() as object;

                foreach (var value in ginfo.CheckedValues)
                {
                    checkedValues++;
                    returnFlag = false;
                    if (selitem.Equals(value))
                    {
                        returnFlag = true;
                        break;
                    }
                }

            }


            if (checkedValues == 0)
                return true;

            return returnFlag;
        }

        void grouppanel_Drop(object sender, DragEventArgs e)
        {
            popup.IsOpen = false;            
        }        

        void group_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (CanGroup == true)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    ListBoxItem item = VisualUtils.FindAncestor(e.OriginalSource as Visual, typeof(ListBoxItem)) as ListBoxItem;
                    Point position, windowpos; Rect rec;
                    GeneralTransform transform = groupbox.TransformToVisual(this);
                    Point point = transform.Transform(new Point());
                    Rect rect = new Rect(point.X, point.Y, groupbox.ActualWidth, groupbox.ActualHeight);
                    GeneralTransform tr = group.TransformToVisual(this);
                    Point p = tr.Transform(new Point());
                    Rect r = new Rect(p.X, p.Y, group.ActualWidth, group.ActualHeight);
                    if (item != null)
                    {
                        if (!(((Math.Abs((double)(currentPosition.X - e.GetPosition(null).X)) < 3) && (Math.Abs((double)(currentPosition.Y - e.GetPosition(null).Y)) < 3))))
                        {
                            if (visualbrush == null)
                            {
                                visualbrush = new VisualBrush();
                            }
                            visualbrush.Visual = item;
                            border.Background = visualbrush;
                            popup.Child = border;
                            position = new Point(Mouse.GetPosition(item).X, Mouse.GetPosition(item).Y);
                            windowpos = item.PointToScreen(position);
                            rec = new Rect(windowpos.X - item.ActualWidth / 2, windowpos.Y - item.ActualHeight * 3 / 2, item.ActualWidth, item.ActualHeight);
                            popup.Width = item.ActualWidth;
                            popup.Height = item.ActualHeight;
                            popup.PlacementRectangle = rec;
                            popup.IsOpen = true;
                            gi = item.Content as GroupInfo;
                            if (!rect.Contains(e.GetPosition(this)) && (!r.Contains(e.GetPosition(this))))
                            {
                                Cursor = Cursors.No;
                            }
                            else
                            {
                                Cursor = Cursors.Arrow;
                            }  
                        }
                    }
                }
            }
        }

        private int CheckInsertionPoint(Point point)
        {
            foreach(var item in groupbox.Items)
            {
                GroupInfo info = item as GroupInfo;
                if (point.X < info.InsertionPoint)
                {
                    info.CanInsert = true;
                    return groupbox.Items.IndexOf(info);
                }
                else
                {
                    info.CanInsert = false;
                }
            }
            
            return -1;
        }

        GroupInfo gibox;

        Border border = new Border(); Border borderbox;

        Popup popup = new Popup() { AllowsTransparency = true };

        VisualBrush visualbrush = null;

        GroupInfo gi;

        int collectionindex;

        GroupInfo groupname;

        private ICollectionView _collectionView;

        private ListBox group;

        private ListBox groupbox;

        private CheckListBox checkedListBox;

        private Popup filterPopup;

        private Grid grouppanel;

        private ResourceDictionary mgeneric;

        internal CardViewItem cardViewItem;

        internal CardViewItem previousSelectedItem;

        private ItemsControl groupHost;

        private Point currentPosition;

        public Style NewItemStyle
        {
            get { return (Style)GetValue(NewItemStyleProperty); }
            set { SetValue(NewItemStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NewItemStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NewItemStyleProperty =
            DependencyProperty.Register("NewItemStyle", typeof(Style), typeof(CardView), new UIPropertyMetadata(null));


        public bool IsGrouped
        {
            get { return (bool)GetValue(IsGroupedProperty); }
            set { SetValue(IsGroupedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsGrouped.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsGroupedProperty =
            DependencyProperty.Register("IsGrouped", typeof(bool), typeof(CardView), new UIPropertyMetadata(false));

        public ObservableCollection<GroupInfo> GroupboxCollection
        {
            get { return (ObservableCollection<GroupInfo>)GetValue(GroupboxCollectionProperty); }
            set { SetValue(GroupboxCollectionProperty, value); }
        }

        public static readonly DependencyProperty GroupboxCollectionProperty =
           DependencyProperty.Register("GroupboxCollection", typeof(ObservableCollection<GroupInfo>), typeof(CardView), new UIPropertyMetadata(null));

        public ObservableCollection<GroupInfo> GroupNames
        {
            get { return (ObservableCollection<GroupInfo>)GetValue(GroupNamesProperty); }
            set { SetValue(GroupNamesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GroupNamesProperty =
            DependencyProperty.Register("GroupNames", typeof(ObservableCollection<GroupInfo>), typeof(CardView), new UIPropertyMetadata(null));

        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(CardView), new UIPropertyMetadata(null));


        public bool IsEditing
        {
            get { return (bool)GetValue(IsEditingProperty); }
            set { SetValue(IsEditingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsEditing.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEditingProperty =
            DependencyProperty.Register("IsEditing", typeof(bool), typeof(CardView), new UIPropertyMetadata(false));


        public ObservableCollection<object> Groups
        {
            get { return (ObservableCollection<object>)GetValue(GroupsProperty); }
            set { SetValue(GroupsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Groups.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GroupsProperty =
            DependencyProperty.Register("Groups", typeof(ObservableCollection<object>), typeof(CardView), new UIPropertyMetadata(null));

        public bool CanSort
        {
            get { return (bool)GetValue(CanSortProperty); }
            set { SetValue(CanSortProperty, value); }
        }        
        
        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanSortProperty =
            DependencyProperty.RegisterAttached("CanSort", typeof(bool), typeof(CardView), new UIPropertyMetadata(true));

        public bool CanGroup
        {
            get { return (bool)GetValue(CanGroupProperty); }
            set { SetValue(CanGroupProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanGroupProperty =
            DependencyProperty.Register("CanGroup", typeof(bool), typeof(CardView), new UIPropertyMetadata(true));


        public bool ShowHeader
        {
            get { return (bool)GetValue(ShowHeaderProperty); }
            set { SetValue(ShowHeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowHeader.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowHeaderProperty =
            DependencyProperty.Register("ShowHeader", typeof(bool), typeof(CardView), new UIPropertyMetadata(true));



        public DataTemplate EditItemTemplate
        {
            get { return (DataTemplate)GetValue(EditItemTemplateProperty); }
            set { SetValue(EditItemTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EditItemTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EditItemTemplateProperty =
            DependencyProperty.Register("EditItemTemplate", typeof(DataTemplate), typeof(CardView), new UIPropertyMetadata(null));

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(CardView), new UIPropertyMetadata(Orientation.Vertical));

        public bool CanEdit
        {
            get { return (bool)GetValue(CanEditProperty); }
            set { SetValue(CanEditProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CanEdit.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanEditProperty =
            DependencyProperty.Register("CanEdit", typeof(bool), typeof(CardView), new UIPropertyMetadata(false, new PropertyChangedCallback(OnCanEditChanged)));

        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set 
            { 
                //CardViewItem CVItem=value as CardViewItem;                
                SetValue(SelectedItemProperty, value);
            }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(CardView), new UIPropertyMetadata(null, new PropertyChangedCallback(OnSelectedItemChanged)));

        public event PropertyChangedCallback SelectedItemChanged;

        /// <summary>
        /// Checks the edit mode.
        /// </summary>
        private void CheckEditMode(object previousselecteditem)
        {
            if (previousselecteditem != null)
            {
                CardViewItem item = this.ItemContainerGenerator.ContainerFromItem(previousselecteditem) as CardViewItem;
                if (item != null)
                {
                    if (item.IsInEditMode)
                    {
                        item._content.ContentTemplate = this.ItemTemplate;
                        item.IsInEditMode = false;
                    }
                }
            }
        }

        private static void OnCanEditChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CardView obj = (CardView)d;
            if(!(bool)e.NewValue)
            for (int i = 0; i < obj.Items.Count; i++) 
            {
                CardViewItem item = obj.ItemContainerGenerator.ContainerFromIndex(i) as CardViewItem;
                if (item != null && item.IsInEditMode)
                {
                    item._content.ContentTemplate = obj.ItemTemplate;
                    item.IsInEditMode = false;
                }
            }
        }

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CardView Cardinstance = (CardView)d;
            Cardinstance.CheckEditMode(e.OldValue);
            Cardinstance.OnSelectedItemChanged(e);
        }

        protected virtual void OnSelectedItemChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SelectedItemChanged != null)
            {
                SelectedItemChanged(this, e);
            }
        }

        
        private PropertyDescriptorCollection propertyCollection;

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (object item in e.NewItems)
                    GetGroupNames(item);
            }
            else if ((e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset ||
                    e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                    && GroupboxCollection!=null && GroupNames!=null)
            {
                if (Items.Count == 0)
                {
                    this.GroupboxCollection.Clear();
                    this.GroupNames.Clear();
                }
            }
            base.OnItemsChanged(e);
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            if (newValue != null)
            {
                foreach (object item in newValue)
                {
                    GetGroupNames(item);
                }
                base.OnItemsSourceChanged(oldValue, newValue);
            }
        }
        private void GetGroupNames(object item)
        {
            Type myObjectType = item.GetType();
            
            
            objectType = myObjectType;
            propertyCollection = TypeDescriptor.GetProperties(item);
            GroupNames = new ObservableCollection<GroupInfo>();
           

            PropertyInfo[] pinfo=objectType.GetProperties();
             Attribute[] myattr = Attribute.GetCustomAttributes(objectType);
           
            foreach (Attribute attr in myattr)
            {
                if (attr is GroupingAttribute)
                {
                    GroupingAttribute g = (GroupingAttribute)attr;
                }
            }


            foreach (PropertyDescriptor item1 in propertyCollection)
            {
                GroupInfo info = new GroupInfo();

               
                //var a = TypeDescriptor.GetAttributes(myObjectType)[typeof(GroupingAttribute)];
                
                AttributeCollection attributes = TypeDescriptor.GetProperties(objectType)[item1.DisplayName] != null? TypeDescriptor.GetProperties(objectType)[item1.DisplayName].Attributes:null;
                GroupingAttribute myAttribute = attributes!=null? (GroupingAttribute)attributes[typeof(GroupingAttribute)]:null;

                if (myAttribute != null)
                {
                    if (myAttribute.CanGroup)
                    {
                        info.Name = item1.DisplayName;
                        info.FilterValues = GetFilterValues(item1);
                        GroupNames.Add(info);
                    }
                }

                else
                {
                    if (myAttribute == null)
                    {
                        info.Name = item1.DisplayName;
                        info.FilterValues = GetFilterValues(item1);
                        GroupNames.Add(info);
                    }
                }
              
            }
        }        
              
        private List<object> GetFilterValues(PropertyDescriptor columnname)
        {
            List<object> returnList = new List<object>();
            if (ItemsSource != null)
            {
                foreach (var item in ItemsSource)
                {
                    if (!returnList.Contains(columnname.GetValue(item)))
                    {
                        returnList.Add(columnname.GetValue(item));
                    }
                }
            }
            return returnList;
        }

        public void GroupCards(string groupname)
        {
            
                if (_collectionView == null)
                {
                    ItemsSource = _collectionView = CollectionViewSource.GetDefaultView(ItemsSource);
                }

                if (_collectionView != null)
                {
                    _collectionView.GroupDescriptions.Add(new PropertyGroupDescription(groupname));

                    _collectionView.SortDescriptions.Add(new SortDescription(groupname, ListSortDirection.Ascending));
                }
                string s = SkinStorage.GetVisualStyle(this).ToString();
                ResourceDictionary rs = new ResourceDictionary();
                if (s == "Default")
                    rs.Source = new Uri("/Syncfusion.Tools.WPF;component/Controls/CardView/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);
                else
                    rs.Source = new Uri("/Syncfusion.Tools.WPF;component/Controls/CardView/Themes/" + s + "Style.xaml", UriKind.RelativeOrAbsolute);

                GroupStyle groupStyle = rs["GroupStyle"] as GroupStyle;

                //GroupStyle.Clear();
                GroupStyle.Add(groupStyle);
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            var cardviewitem= element as CardViewItem;
            if(ItemsSource!=null && ItemTemplate==null && !string.IsNullOrEmpty(DisplayMemberPath))
            {
                var type = item.GetType();
                var prop = type.GetProperty(DisplayMemberPath);
                cardviewitem.Content = prop.GetValue(item,null);
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            CardViewItem item = new CardViewItem();
            item.cardView = this;
            return item;
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is CardViewItem;
        }        
        
    }
}
