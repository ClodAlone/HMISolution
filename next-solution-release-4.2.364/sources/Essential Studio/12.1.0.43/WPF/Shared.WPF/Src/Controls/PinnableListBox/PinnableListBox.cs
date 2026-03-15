#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls.Primitives;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.IO;
using System.Windows.Markup;
using System.Reflection;
using System.Windows.Controls;
using System.Xml.Serialization;
using Syncfusion.Licensing;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// 
    /// </summary>

    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
    Type = typeof(PinnableListBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/PinnableListBox/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(PinnableListBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/PinnableListBox/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(PinnableListBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/PinnableListBox/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
      Type = typeof(PinnableListBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/PinnableListBox/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(PinnableListBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/PinnableListBox/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(PinnableListBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/PinnableListBox/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(PinnableListBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/PinnableListBox/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(PinnableListBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/PinnableListBox/Themes/Generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
    Type = typeof(PinnableListBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/PinnableListBox/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
    Type = typeof(PinnableListBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/PinnableListBox/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
   Type = typeof(PinnableListBox), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/PinnableListBox/Themes/TransparentStyle.xaml")]
    public class PinnableListBox : ItemsControl
    {
        static PinnableListBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PinnableListBox), new FrameworkPropertyMetadata(typeof(PinnableListBox)));
        }
        /// <summary>
        /// 
        /// </summary>
        public PinnableListBox()
        {
            if (EnvironmentTest.IsSecurityGranted)
            {
                EnvironmentTest.StartValidateLicense(typeof(PinnableListBox));
            }
            PinnedItems = new ObservableCollection<object>();
            UnpinnedItems = new ObservableCollection<object>();
        }
        /// <summary>
        /// 
        /// </summary>
        public event RoutedEventHandler PinStatusChangedEvent;
        /// <summary>
        /// 
        /// </summary>
        internal bool isCalledByUpdateItems = false;
        /// <summary>
        /// 
        /// </summary>
        internal string default_StoreFile = AppDomain.CurrentDomain.FriendlyName + ".dat";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public void SaveState(string fileName)
        {
            if (fileName != null && fileName != string.Empty)
                default_StoreFile = fileName;
            else
            {
                string appName = Assembly.GetCallingAssembly().GetName().Name;
                if (appName != null && appName != string.Empty)
                    default_StoreFile = appName + ".dat";
            }

            IsolatedStorageFile isoStorage = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            PinnableListBoxParams stateParams = new PinnableListBoxParams(this.PinItemsSortDescription, this.UnPinItemsSortDescription);
            IsolatedStorageFileStream stream = new IsolatedStorageFileStream(default_StoreFile, FileMode.Create, FileAccess.Write, isoStorage);
            XamlWriter.Save(stateParams, stream);
            stream.Close();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public void LoadState(string fileName)
        {
            IsolatedStorageFile isoStorage = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            if (fileName != null && fileName != string.Empty)
                default_StoreFile = fileName;
            else
            {
                string appName = Assembly.GetCallingAssembly().GetName().Name;
                if (appName != null && appName != string.Empty)
                    default_StoreFile = appName + ".dat";
            }
            string[] storedFiles = isoStorage.GetFileNames(default_StoreFile);

            if (storedFiles.Length > 0)
            {
                Stream stream = new IsolatedStorageFileStream(default_StoreFile, FileMode.Open, isoStorage);

                PinnableListBoxParams stateParams = XamlReader.Load(stream) as PinnableListBoxParams;

                stream.Close();

                ApplyStateParams(stateParams);
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stateparams"></param>
        internal void ApplyStateParams(PinnableListBoxParams stateparams)
        {
            this.PinItemsSortDescription = stateparams.PinItemsSortDescription;
            this.UnPinItemsSortDescription = stateparams.UnPinItemsSortDescription;


        }

        /// <summary>
        /// 
        /// </summary>
        internal PinnableListBoxItem pinnableItem;
        /// <summary>
        /// 
        /// </summary>
        internal PinnableItemsControl pinnedItems;
        /// <summary>
        /// 
        /// </summary>
        internal PinnableItemsControl unpinnedItems;
        /// <summary>
        /// 
        /// </summary>
        public override void OnApplyTemplate()
        {
            pinnedItems = GetTemplateChild("PART_PinnedItems") as PinnableItemsControl;
            unpinnedItems = GetTemplateChild("PART_UnpinnedItems") as PinnableItemsControl;
            if (pinnedItems != null)
            {
                pinnedItems.pinnableListBox = this;
                pinnedItems.ItemContainerStyle = this.ItemContainerStyle;
                pinnedItems.ItemContainerStyleSelector = this.ItemContainerStyleSelector;
                pinnedItems.ItemTemplate = this.ItemTemplate;
                pinnedItems.ItemTemplateSelector = this.ItemTemplateSelector;
                //isCalledByUpdateItems = true;   
            }
            if (unpinnedItems != null)
            {
                unpinnedItems.pinnableListBox = this;
                unpinnedItems.ItemContainerStyle = this.ItemContainerStyle;
                unpinnedItems.ItemContainerStyleSelector = this.ItemContainerStyleSelector;
                unpinnedItems.ItemTemplate = this.ItemTemplate;
                unpinnedItems.ItemTemplateSelector = this.ItemTemplateSelector;              
            }          
            base.OnApplyTemplate();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnItemsSourceChanged(System.Collections.IEnumerable oldValue, System.Collections.IEnumerable newValue)
        {
            if (PinnedItems != null)
                PinnedItems.Clear();
            if (UnpinnedItems != null)
                UnpinnedItems.Clear();

            foreach (var _item in Items)
            {
                UpdatePinItems(this, _item, false);
            }
            base.OnItemsSourceChanged(oldValue, newValue);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (var _item in e.NewItems)
                {
                    UpdatePinItems(this, _item, false);
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                if(PinnedItems != null)
                    PinnedItems.Clear();
                if(UnpinnedItems != null)
                    UnpinnedItems.Clear();
                foreach (var _item in Items)
                {
                    UpdatePinItems(this, _item, false);
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems.Count > 0)
                {
                    foreach (var _item in e.OldItems)
                    {
                        if (PinnedItems != null && PinnedItems.Contains(_item))
                            PinnedItems.Remove(_item);
                        else if (UnpinnedItems != null && UnpinnedItems.Contains(_item))
                            UnpinnedItems.Remove(_item);
                    } 
                }
            }

            base.OnItemsChanged(e);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }

        /// <summary>
        /// 
        /// </summary>
        public string PinItemsSortDescription
        {
            get { return (string)GetValue(PinItemsSortDescriptionProperty); }
            set { SetValue(PinItemsSortDescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PinItemsSortDescription.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty PinItemsSortDescriptionProperty =
            DependencyProperty.Register("PinItemsSortDescription", typeof(string), typeof(PinnableListBox), new UIPropertyMetadata(""));

        /// <summary>
        /// 
        /// </summary>
        public string UnPinItemsSortDescription
        {
            get { return (string)GetValue(UnPinItemsSortDescriptionProperty); }
            set { SetValue(UnPinItemsSortDescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UnPinItemsSortDescription.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty UnPinItemsSortDescriptionProperty =
            DependencyProperty.Register("UnPinItemsSortDescription", typeof(string), typeof(PinnableListBox), new UIPropertyMetadata(""));

        /// <summary>
        /// 
        /// </summary>
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(PinnableListBox), new UIPropertyMetadata(string.Empty));


        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<object> PinnedItems
        {
            get { return (ObservableCollection<object>)GetValue(PinnedItemsProperty); }
            set { SetValue(PinnedItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PinnedItems.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty PinnedItemsProperty =
            DependencyProperty.Register("PinnedItems", typeof(ObservableCollection<object>), typeof(PinnableListBox), new UIPropertyMetadata(null));


        /// <summary>
        /// 
        /// </summary>
        public ListSortDirection PinnedSortDirection
        {
            get { return (ListSortDirection)GetValue(PinnedSortDirectionProperty); }
            set { SetValue(PinnedSortDirectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PinnedSortDirection.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty PinnedSortDirectionProperty =
            DependencyProperty.Register("PinnedSortDirection", typeof(ListSortDirection), typeof(PinnableListBox), new UIPropertyMetadata(ListSortDirection.Ascending));


        /// <summary>
        /// 
        /// </summary>
        public ListSortDirection UnPinnedSortDirection
        {
            get { return (ListSortDirection)GetValue(UnPinnedSortDirectionProperty); }
            set { SetValue(UnPinnedSortDirectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UnPinnedSortDirection.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty UnPinnedSortDirectionProperty =
            DependencyProperty.Register("UnPinnedSortDirection", typeof(ListSortDirection), typeof(PinnableListBox), new UIPropertyMetadata(ListSortDirection.Ascending));


        

        
        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<object> UnpinnedItems
        {
            get { return (ObservableCollection<object>)GetValue(UnpinnedItemsProperty); }
            set { SetValue(UnpinnedItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UnpinnedItems.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty UnpinnedItemsProperty =
            DependencyProperty.Register("UnpinnedItems", typeof(ObservableCollection<object>), typeof(PinnableListBox), new UIPropertyMetadata(null));
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
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(PinnableListBox), new UIPropertyMetadata(null));
        /// <summary>
        /// 
        /// </summary>
        internal void FirePinStatusChanged()
        {
            if (PinStatusChangedEvent != null)
            {
                PinnableListBoxEventArgs pinargs = new PinnableListBoxEventArgs();
                pinargs.PinnablelistboxItem =pinnableItem;
                this.SelectedItem = pinnableItem;
                PinStatusChangedEvent(this, pinargs);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        /// <param name="item"></param>
        /// <param name="ispinned"></param>
        internal void UpdatePinItems(PinnableListBox control, object item, bool ispinned)
        {
            isCalledByUpdateItems = true;
            PinnableListBoxItem p_Item = item as PinnableListBoxItem;
            if (p_Item != null && control.ItemsSource != null && p_Item.DataContext != null)
                item = p_Item.DataContext;
            if (p_Item != null && control.ItemsSource == null && p_Item.DataContext == null)
            {
                if (ispinned)
                {
                    control.UnpinnedItems.Remove(p_Item);
                    if (!control.PinnedItems.Contains(p_Item))
                        control.PinnedItems.Add(p_Item);
                }
                else
                {
                    control.PinnedItems.Remove(p_Item);
                    if (!control.UnpinnedItems.Contains(p_Item))
                        control.UnpinnedItems.Add(p_Item);
                }
            }
            else
            {
                if (ispinned)
                {
                    control.UnpinnedItems.Remove(item);
                    if (!control.PinnedItems.Contains(item))
                        control.PinnedItems.Add(item);
                }
                else
                {
                    control.PinnedItems.Remove(item);
                    if (!control.UnpinnedItems.Contains(item))
                        control.UnpinnedItems.Add(item);
                }

            }
            if(p_Item != null && p_Item.IsPinned != ispinned && !ispinned)
            p_Item.IsPinned = ispinned;
            SortCollection();
            control.SelectedItem = item;
            isCalledByUpdateItems = false;
        }
        /// <summary>
        /// 
        /// </summary>
        public void SortCollection()
        {
            if (!String.IsNullOrEmpty(UnPinItemsSortDescription))
            {
                ListCollectionView col_View = CollectionViewSource.GetDefaultView(UnpinnedItems) as ListCollectionView;
                using ( col_View.DeferRefresh() )
                {
                  col_View.SortDescriptions.Clear();
                  col_View.SortDescriptions.Add( new SortDescription( UnPinItemsSortDescription, UnPinnedSortDirection ) );
                }
            }

            if (!String.IsNullOrEmpty(PinItemsSortDescription))
            {
                ListCollectionView _col_View = CollectionViewSource.GetDefaultView(PinnedItems) as ListCollectionView;
                using ( _col_View.DeferRefresh() )
                {
                  _col_View.SortDescriptions.Clear();
                  _col_View.SortDescriptions.Add( new SortDescription( PinItemsSortDescription, PinnedSortDirection ) );
                }
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class PinnableListBoxEventArgs:RoutedEventArgs
    {
        private Object pinnablelistboxItem;
        /// <summary>
        /// 
        /// </summary>
        public Object PinnablelistboxItem
        {
            get { return pinnablelistboxItem; }
            set { pinnablelistboxItem = value; }
        }   
        
    }
}
