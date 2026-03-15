using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.ComponentModel;
using UFInterfaces.PropertyControl;
using System.Windows.Data;
using Mindscape.WpfElements.WpfPropertyGrid;
using Utilities;
using Mindscape.WpfElements.PropertyEditing;
using Mindscape.WpfElements.Themes;

namespace UFUACommonControls
{
    /// <summary>
    /// Interaction logic for NewTransport.xaml
    /// </summary>
    public partial class NewTransport : UserControl
    {
        #region Declarations
        bool bLoaded = false;
        #endregion

        public NewTransport(IList<string> applicationBaseAddresses)
        {
            InitializeComponent();

            TransportList transportList = TryFindResource("TransListData") as TransportList;
            transportList.InitList(applicationBaseAddresses);
            SetGridFilter();

            //var style = ApplicationPropertiesHelper.GetProperty("CurrentSkin") as String;
            //if (style == "Blend")
            //{
            //    propertyGridShadow.Resources.MergedDictionaries.Add(new Alloy());
            //}

            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;

                var ba = DataContext as UFUAModel.AddressBase;
                comboTransport.SelectedItem = ba != null ? ba.Transport : String.Empty;
                propertyGridShadow.SelectedObject = ba;

                if (DataContext is INotifyPropertyVisibilityChanged)
                {
                    var context = DataContext as INotifyPropertyVisibilityChanged;
                    context.PropertyVisiblityChanged += Context_PropertyVisiblityChanged;
                }
            };

            Unloaded += (o, e) =>
            {
                if (!bLoaded)
                    return;
                bLoaded = false;

                if (DataContext is INotifyPropertyVisibilityChanged)
                {
                    var context = DataContext as INotifyPropertyVisibilityChanged;
                    context.PropertyVisiblityChanged -= Context_PropertyVisiblityChanged;
                }
            };
        }

        private void Context_PropertyVisiblityChanged(object sender, PropertyChangedEventArgs e)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(propertyGridShadow.BindingView);
            view.Refresh();
        }

        void OnPropertyGridNodeExpanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem tvi = (TreeViewItem)(e.OriginalSource);
            PropertyGridRow expandingRow = (PropertyGridRow)(tvi.Header);
            SetGridFilter(expandingRow.Children);
        }

        void SetGridFilter(PropertyGridBindingView bv = null)
        {
            using (var wait = new WaitCursor())
            {
                if (bv == null)
                    bv = propertyGridShadow.BindingView;
                ICollectionView view = CollectionViewSource.GetDefaultView(bv);
                view.Filter = obj =>
                {
                    Node node = null;
                    if (obj is Node)
                        node = obj as Node;
                    else
                        node = ((PropertyGridRow)obj).Node;

                    if (node.PropertyInfo.Name == "Oid")
                        return false;

                    if (DataContext is INotifyPropertyVisibilityChanged)
                    {
                        var context = DataContext as INotifyPropertyVisibilityChanged;
                        return context[node.PropertyInfo.Name];
                    }

                    return true;
                };
            }
        }

        #region TreeListView

        private void PropertyGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            e.Handled = true;
            TreeListViewResizeColumnWidth();
        }


        private void PropertyGrid_VisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (propertyGridShadow.Visibility == Visibility.Visible)
                TreeListViewResizeColumnWidth();
        }

        void TreeListViewResizeColumnWidth()
        {
            TreeListView.SetResizeColumnWidth(propertyGridShadow, propertyGridShadow.ActualWidth / 2);
        }

        #endregion

        private void comboTransport_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var ba = DataContext as UFUAModel.AddressBase;
            String sel = (e.AddedItems.Count > 0 ? e.AddedItems[0] as String : null);
            String prev = (e.RemovedItems.Count > 0 ? e.RemovedItems[0] as String : null);
            if (ba != null && sel != null && prev != null && prev != sel)
            {
                ba.Port = ba.UFUAConfiguration.GetDefaultPort(sel);
            }
        }
     }
}
