using DevExpress.Xpf.Core;
using GPIO.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using UFInterfaces.Editors;
using Utilities;
using Utilities.WPF;
using WPFUtilities;

namespace GPIO.Controls
{
    /// <summary>
    /// Interaction logic for ControlEditor.xaml
    /// </summary>
    public partial class ControlEditor : UserControl, ISelectEntityReference
    {
        ObservableCollection<PINSettings> collection = new ObservableCollection<PINSettings>();
        readonly GPIO variables;
        bool bLoaded;
        public ControlEditor(GPIO v)
        {
            InitializeComponent();
            variables = v;
            if (v.CurrentDocument != null && v.CurrentDocument.ListPIN != null)
                collection = new ObservableCollection<PINSettings>(v.CurrentDocument.ListPIN);

            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    bLoaded = true;

                    //if (v.CurrentDocument != null && v.CurrentDocument.ListPIN != null)
                    //    collection = new ObservableCollection<PINSettings>(v.CurrentDocument.ListPIN);
                    gridControl.ItemsSource = collection;
                    UpdateSelectedItem();
                    collection.CollectionChanged += (ob, ev) =>
                    {
                        variables.CurrentDocument.ListPIN = collection.ToList();
                    };

                    var wnd = this.FindParent<Window>();
                    if (wnd != null)
                    {
                        wnd.Closing += (s, c) =>
                        {
                            var selected = tableView.FocusedRow as PINSettings;
                            if (selected != null)
                            {
                                SelectedReference = variables.GetReference(selected.Name);
                                SelectedReferences = null;
                            }
                            else
                            {
                                SelectedReference = null;
                                SelectedReferences = null;
                            }
                        };
                    }
                }
            };
        }

        private void DeleteCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            if (tableView.FocusedRow != null && collection.Contains(tableView.FocusedRow))
                collection.Remove(tableView.FocusedRow as PINSettings);
        }

        private void DeleteCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tableView.FocusedRow != null;
        }

        private void MoveUpCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var command = tableView.FocusedRow as PINSettings;
            int index = collection.IndexOf(command);
            collection.Remove(command);
            collection.Insert(index - 1, command);
            tableView.FocusedRow = command;
        }

        private void MoveDownCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var command = tableView.FocusedRow as PINSettings;
            int index = collection.IndexOf(command);
            collection.Remove(command);
            collection.Insert(index + 1, command);
            tableView.FocusedRow = command;
        }

        private void MoveUpCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tableView.FocusedRow != null && collection.Contains(tableView.FocusedRow) && collection.IndexOf(tableView.FocusedRow as PINSettings) > 0;
        }

        private void MoveDownCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tableView.FocusedRow != null && collection.Contains(tableView.FocusedRow) && collection.IndexOf(tableView.FocusedRow as PINSettings) < collection.Count - 1;
        }

        private void CopyCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var command = tableView.FocusedRow as PINSettings;
            var list = new List<PINSettings>();
            list.Add(command);
            var str = list.ToXml();
            Clipboard.SetText(str, TextDataFormat.UnicodeText);
        }

        private void PasteCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var command = tableView.FocusedRow as PINSettings;
            int index = collection.Count;
            if (command != null && collection.Contains(command))
                index = collection.IndexOf(command);

            var str = Clipboard.GetText(TextDataFormat.UnicodeText);
            var list = str.FromXml<List<PINSettings>>();
            foreach (var cmd in list)
            {
                collection.Insert(index, cmd);
            }
            if (list.Count > 0)
                tableView.FocusedRow = list[list.Count - 1];
        }

        private void CopyCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tableView.FocusedRow != null;
        }

        private void PasteCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                string str = Clipboard.GetText(TextDataFormat.UnicodeText);
                e.CanExecute = str.FromXml<List<PINSettings>>() != null;
            }
            catch (Exception ex)
            {
                e.CanExecute = false;
            }
        }

        private void NewCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var command = tableView.FocusedRow as PINSettings;
            int index = collection.Count;
            if (command != null && collection.Contains(command))
                index = collection.IndexOf(command);
            var newitem = new PINSettings() { DebounceTimeout = 50 };
            while(true)
            {
                var found = (from c in collection where c.Pin == newitem.Pin select c).ToList();
                if (found.Count == 0)
                    break;

                newitem.Pin++;
            }
            collection.Insert(index, newitem);
            tableView.FocusedRow = newitem;
        }

        private void NewCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void tableView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var wnd = this.FindParent<Window>();
            if (wnd != null)
            {
                wnd.DialogResult = true;
                wnd.Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var parent = this.FindParent<Window>();
            var wnd = new DXWindow()
            {
                Title = Properties.Resources.PubNubSettingsHeader,
                Content = new PubNubSettings() { DataContext = variables.CurrentDocument },
                BorderEffect = BorderEffect.Default,
                SizeToContent = System.Windows.SizeToContent.WidthAndHeight,
                Owner = parent
            };

            ThemeHelper.SetTheme(wnd);

            if (wnd.ShowDialog() == true)
                return;
        }

        #region ISelectEntityReference
        public object SelectedReference { get; set; }

        public List<object> SelectedReferences { get; set; }
        PINSettings selectedInstanceOnEdit;
        public void BringIntoView(object selectedReference)
        {
            selectedInstanceOnEdit = (from t in collection where t.Name == (selectedReference as OPCUAViewModel.OPCUAEntityReference)?.HumanReadableNoProject select t).FirstOrDefault();
            if(bLoaded)
                UpdateSelectedItem();
        }
        void UpdateSelectedItem()
        {
            if (bLoaded && selectedInstanceOnEdit != null)
                gridControl.SelectedItem = selectedInstanceOnEdit;
        }
        #endregion
    }
}
