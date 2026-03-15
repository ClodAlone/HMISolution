using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using AnimationManager.ComponentService;
using DevExpress.Xpf.Core;
using Utilities;
using Utilities.WPF;
using DocumentManager.ComponentService;

namespace AnimationManager.UserControls
{
    /// <summary>
    /// Interaction logic for ColorAnimationPropertyEditor.xaml
    /// </summary>
    public partial class ColorAnimationPropertyEditor : UserControl, IDisposable
    {
        BindingList<ColorData> list;
        bool bDisposed;
        bool bLoaded;

        #region Document
        public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register("Document", typeof(IDocument), typeof(ColorAnimationPropertyEditor), new UIPropertyMetadata(null));
        public IDocument Document
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (IDocument)GetValue(DocumentProperty);
            }
            set
            {
                SetValue(DocumentProperty, value);
            }
        }
        #endregion

        public ColorAnimationPropertyEditor()
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;
            };

            if (AnimationManagerComponent.propertyServiceAvailable)
            {
                Document = AnimationManagerComponent.workspace.ContextDocument;
                var property = AnimationManagerComponent.propertyService.controlNoSelectionPriority;
                property.ClearValue(FrameworkElement.WidthProperty);
                property.ClearValue(FrameworkElement.HeightProperty);
                content.Content = property;

                DataContextChanged += (o, e) =>
                {
                    AnimationManagerComponent.propertyService.SetControlSelection(property, DataContext);

                    var colorAnimation = DataContext as BackColorAnimation;
                    if (colorAnimation != null)
                    {
                        list = new BindingList<ColorData>(colorAnimation.ListColors);
                        gridControl.ItemsSource = list;

                        columnText.Visible = IsTextControlAware(colorAnimation.Control);
                    }
                    else
                    {
                        var bordercolorAnimation = DataContext as BorderColorAnimation;
                        if (bordercolorAnimation != null)
                        {
                            list = new BindingList<ColorData>(bordercolorAnimation.ListColors);
                            gridControl.ItemsSource = list;

                            columnText.Visible = IsTextControlAware(bordercolorAnimation.Control);
                        }
                    }

                    gridControl.SortBy(valueCol, DevExpress.Data.ColumnSortOrder.Ascending);
                };
            }

            tableView.KeyUp += view_KeyUp;
        }

        bool IsTextControlAware(UIElement element)
        {
            if (element is ContentControl)
            {
                var contentControl = element as ContentControl;
                if (contentControl.Content is String)
                    return true;
                else
                {
                    var currentList = (from c in contentControl.GetVisualChildrenOfType<ContentControl>()
                                       where c.Content is String && c.Visibility == System.Windows.Visibility.Visible
                                       select c).ToList();
                    if (currentList.Count > 0)
                        return true;
                }
            }
            else if (element is TextBox)
                return true;
            else if (element is TextBlock)
                return true;

            return false;
        }

        void view_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                OnDelete(null, null);
            }
            if (e.Key == Key.Insert)
            {
                OnAdd(null, null);
            }
        }
        private void DeleteRow(int rowHandle)
        {
            if (tableView.IsEditing)
                return;

            int listIndex = gridControl.GetRowListIndex(rowHandle);
            if (listIndex >= 0)
                list.RemoveAt(listIndex);
        }
        private void InsertRow(int rowHandle, ColorData data)
        {
            int listIndex = gridControl.GetRowListIndex(rowHandle);
            if (listIndex < 0 || listIndex >= list.Count) listIndex = -1;
                list.Insert(listIndex + 1, data);
        }

        private void OnAdd(object sender, RoutedEventArgs e)
        {
            if (list == null)
                return;

            var listValues = (from c in list select c.Value);
            double maxValue = 1;
            try
            {
                maxValue = listValues.Max() + 1;
            }
            catch (Exception ex)
            {
                maxValue = 1;
            }

            var random = new Random();
            var color = Color.FromRgb((byte)random.Next(255),
                                            (byte)random.Next(255),
                                            (byte)random.Next(255));

            var newData = new ColorData { Value = maxValue, Color = color };
            InsertRow(tableView.FocusedRowHandle, newData);
        }

        private void OnDelete(object sender, RoutedEventArgs e)
        {
            DeleteRow(tableView.FocusedRowHandle);
        }

        private void tableView_FocusedRowChanged(object sender, DevExpress.Xpf.Grid.FocusedRowChangedEventArgs e)
        {
            int listIndex = gridControl.GetRowListIndex(tableView.FocusedRowHandle);
            btnDelete.IsEnabled = listIndex >= 0;
        }

        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            if (content.Content is IDisposable)
                (content.Content as IDisposable).Dispose();
        }
    }
}
