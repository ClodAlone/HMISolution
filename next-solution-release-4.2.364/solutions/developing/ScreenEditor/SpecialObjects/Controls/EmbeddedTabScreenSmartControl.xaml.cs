using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using DevExpress.Xpf.Editors;
using ScreenManager.ComponentService;
using Utilities;
using UFProjectManager.ComponentService;
using System.Windows.Threading;
using DocumentManager.ComponentService;
using System.Collections.ObjectModel;
using Utilities.WPF;

namespace ScreenManager.SpecialObjects.Controls
{
    /// <summary>
    /// Interaction logic for EmbeddedTabScreenSmartControl.xaml
    /// </summary>
    /// 

    public class ScreenData
    {
        public String Screen { get; set; }
    }

    public partial class EmbeddedTabScreenSmartControl : UserControl
    {
        #region Declarations
        readonly EmbeddedTabScreen control;
        ObservableCollection<ScreenData> datalist;
        Window parentWindow;

        IDocument document;
        IUFProjectManager editor;

        bool bPendingChanges;
        bool bLoaded;
        #endregion

        #region Constructors
        public EmbeddedTabScreenSmartControl(EmbeddedTabScreen control)
        {
            InitializeComponent();
            this.control = control;

            Loaded += (o, e) =>
            {
                control.DragScreenOver += DragScreenOver;

                if (bLoaded)
                    return;
                bLoaded = true;

                parentWindow = this.FindParent<Window>();
                if (parentWindow != null)
                {
                    parentWindow.Closing += (s, ev) =>
                    {
                        ApplyChanges();
                    };
                }

                document = ScreenSettings.ScreenDocument.GetScreenDocument(control);
                editor = document?.GetService(typeof(IUFProjectManager)) as IUFProjectManager;

                datalist = new ObservableCollection<ScreenData>();
                if (control.Screens != null)
                {
                    control.Screens.ForEach(s => datalist.Add(new ScreenData() { Screen = s }));
                }
                gridControl.ItemsSource = datalist;
            };

            Unloaded += (o, e) =>
            {
                control.DragScreenOver -= DragScreenOver;
            };
        }
        #endregion

        private void DragScreenOver(object sender, EventArgs e)
        {
            if (!bPendingChanges)
            {
                bPendingChanges = true;
                Dispatcher.CurrentDispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                {
                    datalist.Clear();
                    if (control.Screens != null)
                        control.Screens.ForEach(s => datalist.Add(new ScreenData() { Screen = s }));
                    bPendingChanges = false;
                });
            }
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

            int listIndex = gridControl.GetListIndexByRowHandle(rowHandle);
            if (listIndex >= 0)
                datalist.RemoveAt(listIndex);
        }

        private void InsertRow(int rowHandle)
        {
            int listIndex = gridControl.GetListIndexByRowHandle(rowHandle);
            if (listIndex < 0 || listIndex >= datalist.Count) listIndex = -1;
            var screenlist = GetScreenList();
            datalist.Insert(listIndex + 1, new ScreenData() { Screen = screenlist.FirstOrDefault()});
        }

        private void OnAdd(object sender, RoutedEventArgs e)
        {
            InsertRow(tableView.FocusedRowHandle);
            ApplyChanges();
        }

        private void OnDelete(object sender, RoutedEventArgs e)
        {
            DeleteRow(tableView.FocusedRowHandle);
            ApplyChanges();
        }

        private void tableView_FocusedRowChanged(object sender, DevExpress.Xpf.Grid.FocusedRowChangedEventArgs e)
        {
            int listIndex = gridControl.GetListIndexByRowHandle(tableView.FocusedRowHandle);
            btnDelete.IsEnabled = listIndex >= 0;
        }

        readonly List<ComboBoxEdit> listFilled = new List<ComboBoxEdit>();
        IEnumerable<String> listScreens;
        private void PART_Editor_DropDownOpened(object sender, RoutedEventArgs e)
        {
            var combo = sender as ComboBoxEdit;
            if (combo == null || listFilled.Contains(combo) || editor == null || document == null)
                return;
            listFilled.Add(combo);
            using (var cursor = new WaitCursor())
            {
                if (listScreens == null)
                    listScreens = editor.GetResourceList(document, System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));
                combo.ItemsSource = listScreens;
            }
        }
        private IEnumerable<string> GetScreenList()
        {
            if (listFilled.Count > 0)
                return listFilled[0].ItemsSource as IEnumerable<string>;

            // var editor = ScreenViewer.GetScreenEditor(Control) as ScreenManagerComponent;
            var doc = ScreenSettings.ScreenDocument.GetScreenDocument(control);
            var editor = doc.GetService(typeof(IUFProjectManager)) as IUFProjectManager;
            if (editor == null)
                return null;

            using (var cursor = new WaitCursor())
            {
                if (listScreens == null)
                    listScreens = editor.GetResourceList(doc, System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));
                return listScreens;
            }
        }

       
        private void MoveUp(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            int index = gridControl.GetListIndexByRowHandle(tableView.FocusedRowHandle);
            if (index <= 0)
                return;
            int newindex = index - 1;

            try
            {
                ScreenData item = datalist[index];
                datalist.RemoveAt(index);
                datalist.Insert(newindex, item);
                
                tableView.FocusedRowHandle = newindex;

                ApplyChanges();
            }
            catch (Exception)
            {
            }
        }

        private void MoveDown(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            int index = gridControl.GetListIndexByRowHandle(tableView.FocusedRowHandle);
            if (index < 0 || index >= datalist.Count - 1)
                return;
            int newindex = index + 1;

            ScreenData item = datalist[index];
            datalist.RemoveAt(index);
            datalist.Insert(newindex, item);
            
            tableView.FocusedRowHandle = newindex;

            ApplyChanges();
        }

        void ApplyChanges()
        {
            if (parentWindow == null || parentWindow.DialogResult == true)
            {
                var list = new ScreenList();
                datalist.ToList().ForEach(s => list.Add(s.Screen));
                control.Screens = list;
            }
        }

        private void tableView_CellValueChanged(object sender, DevExpress.Xpf.Grid.CellValueChangedEventArgs e)
        {
            ApplyChanges();
        }
    }
}
