using System;
using System.Collections.Generic;
using System.Linq;
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
using Utilities;
using Mindscape.WpfElements.Themes;
using Mindscape.WpfElements.WpfPropertyGrid;
using System.ComponentModel;
using Mindscape.WpfElements.PropertyEditing;
using UFInterfaces.PropertyControl;

namespace UFUAEditor.Controls
{
    /// <summary>
    /// Interaction logic for NewTransport.xaml
    /// </summary>
    public partial class NewTransport : UserControl
    {
        #region Declarations
        bool bLoaded = false;
        #endregion

        public NewTransport()
        {
            InitializeComponent();

            SetGridFilter();

            var style = ApplicationPropertiesHelper.GetProperty("CurrentSkin") as String;
            if (style == "Blend")
            {
                propertyGridShadow.Resources.MergedDictionaries.Add(new Alloy());
            }

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
                    Node node = ((PropertyGridRow)obj).Node;

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
