using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using DocumentManager.ComponentService;
using DocumentManager.ComponentService.Helpers;
using Opc.Ua;
using OPCUAViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
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
using System.Xml;
using UFInterfaces;
using UFUAEditor.ComponentService;
using Utilities;
using Utilities.WPF;
using ViewModelLib;
using WatchControl.Converters;

namespace WatchControl
{
    /// <summary>
    /// Interaction logic for WatchControl.xaml
    /// </summary>
    /// 
    public class ListItem : ViewModelBase
    {
        String name;
        public String Name
        {
            get
            {
                return name;
            }

            set
            {
                if (name == value)
                    return;
                name = value;
                OnPropertyChanged("Name");
            }
        }

        DisplayStringFormat format;
        public DisplayStringFormat Format
        {
            get
            {
                return format;
            }

            set
            {
                if (format == value)
                    return;
                format = value;
                OnPropertyChanged("Format");
                OnPropertyChanged("MaskValue");
                OnPropertyChanged("MaskValueType");
            }
        }

        MonitoredItemViewModel monitoredItemViewModel;
        public MonitoredItemViewModel MonitoredItemViewModel
        {
            get
            {
                return monitoredItemViewModel;
            }

            set
            {
                if (monitoredItemViewModel == value)
                    return;
                monitoredItemViewModel = value;
                OnPropertyChanged("MonitoredItemViewModel");
            }
        }

        public OPCUAEntityReference reference;
    }

    [DataContract(Name = "SettingsStorage")]
    public class SettingsStorage
    {
        #region Members persistance
        //[DataMember]
        //public List<String> listTags;
        [DataMember]
        public Dictionary<String,DisplayStringFormat> listTags;
        #endregion
    }

    public partial class WatchControl : UserControl, IDisposable, IEntityReference
    {
        readonly IDocument parent;
        readonly String title;
        readonly String sessionName;
        readonly IUFUAEditorManager UFUAEditor;
        DataSinkInterface datainterface;

        readonly SafeObservableCollection<ListItem> listItems = new SafeObservableCollection<ListItem>();
        readonly List<PropertyObserver<OPCUAEntityReference>> listPropertyObserverEntityReferenceResolveVariable = new List<PropertyObserver<OPCUAEntityReference>>();
        
        bool bLoaded;

        public WatchControl(IDocument parent, String title, String datasink = null)
        {
            InitializeComponent();

            this.parent = parent;
            this.title = title;
            
            var docParent = parent;
            if (parent != null)
                docParent = DocumentHelper.GetRootParent(parent, traverse: false);
            sessionName = docParent != null ? docParent.Title : Properties.Settings.Default.DefaultSessionName;

            gridControl.ItemsSource = listItems;
            UFUAEditor = parent.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;

            if (datasink != null)
                datainterface = OPCUAEntityReference.GetDataSinkInterface(datasink);
            if (datainterface != null)
            {
                btnAdd.Visibility = Visibility.Collapsed;
                btnDelete.Visibility = Visibility.Collapsed;
            }
            else
                tableView.KeyUp += view_KeyUp;

            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    bLoaded = true;

                    if (datainterface != null)
                    {
                        var list = datainterface.GetVariables(parent);
                        list.ForEach(name => AddNewWatchItem(name));
                    }

                    LoadLayout();
                }
            };
        }

        void view_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                DeleteSelectedRows();
            }
        }

        private void DeleteSelectedRows()
        {
            var SelectedRows = gridControl.SelectedItems;
            if (SelectedRows != null && SelectedRows.Count > 0)
            {
                using (var cursor = new WaitCursor())
                {
                    List<ListItem> toRemove = new List<ListItem>();
                    foreach (ListItem focusedRow in SelectedRows)
                        toRemove.Add(focusedRow as ListItem);
                    toRemove.ForEach(item => DeleteRow(item));
                }
            }
            else
            {
                var item = gridControl.CurrentItem as ListItem;
                if (item != null)
                    DeleteRow(item);
            }
        }

        private void DeleteRow(ListItem item)
        {
            if (item == null || tableView.IsEditing || !listItems.Contains(item))
                return;

            if (item.reference != null)
                item.reference.SetInUse(this, false);
            listItems.Remove(item);
        }


        private void tableView_FocusedRowChanged(object sender, DevExpress.Xpf.Grid.FocusedRowChangedEventArgs e)
        {
            int listIndex = gridControl.GetRowListIndex(tableView.FocusedRowHandle);
            btnDelete.IsEnabled = listIndex >= 0;
        }

        private void TableView_CellValueChanging(object sender, DevExpress.Xpf.Grid.CellValueEventArgs e)
        {
            TableView view = sender as TableView;
            if(e.Column.Name == "formatValue")
                view.PostEditor();
        }

        void AddNewWatchItem(OPCUAEntityReference entityReference, DisplayStringFormat format)
        {
            if (entityReference == null)
                return;

            var listItem = new ListItem() { Name = entityReference.StringRepresentationWithProject, reference = entityReference, Format = format };
            listItems.Add(listItem);
            listItem.MonitoredItemViewModel = new MonitoredItemViewModel() { DataValue = new DataValue("", StatusCodes.BadWaitingForInitialData) };
            var observer = new PropertyObserver<OPCUAEntityReference>(entityReference);
            listPropertyObserverEntityReferenceResolveVariable.Add(observer);
            observer.RegisterHandler(n => n.MonitoredItemViewModel, n =>
            {
                // observer.UnregisterHandler(p => p.MonitoredItemViewModel);
                listItem.MonitoredItemViewModel = n.MonitoredItemViewModel;
            });

            entityReference.Resolve(sessionName, parent);
            entityReference.SetInUse(this, true);

            colValue.AllowSorting = DevExpress.Utils.DefaultBoolean.True;
            colValue.AllowGrouping = DevExpress.Utils.DefaultBoolean.True;
            colTimeStamp.AllowSorting = DevExpress.Utils.DefaultBoolean.True;
            colTimeStamp.AllowGrouping = DevExpress.Utils.DefaultBoolean.True;
            colQuality.AllowSorting = DevExpress.Utils.DefaultBoolean.True;
            colQuality.AllowGrouping = DevExpress.Utils.DefaultBoolean.True;
        }

        void AddNewWatchItem(String name)
        {
            if (datainterface != null)
            {
                var monitoredItem = datainterface.GetVariable(name, parent);
                if (monitoredItem != null)
                {
                    var listItem = new ListItem() { Name = name, MonitoredItemViewModel = monitoredItem };
                    listItems.Add(listItem);
                }
                else
                {
                    var entity = datainterface.GetReference(name);
                    if (entity != null)
                        AddNewWatchItem(entity, DisplayStringFormat.Default);
                }
            }
            else
            {
                var erString = UFUAEditor.GetTagEntityReference(parent, name, null, inExecution: true);
                if (!String.IsNullOrEmpty(erString))
                {
                    try
                    {
                        var entityReference = erString.FromXml<OPCUAEntityReference>();
                        AddNewWatchItem(entityReference, DisplayStringFormat.Default);
                    }
                    catch
                    {

                    }
                }
            }
        }

        void UnsubscribeAll()
        {
            listItems.ToList().ForEach(item =>
            {
                if (item.reference != null)
                    item.reference.SetInUse(this, false);
            });
            listItems.Clear();
        }

        #region IEntityReference
        public ImageSource CollapsedImageSource
        {
            get { throw new NotImplementedException(); }
        }

        public ImageSource ExpandedImageSource
        {
            get { throw new NotImplementedException(); }
        }

        public ContextMenu contextMenu
        {
            get { throw new NotImplementedException(); }
        }

        public object Tooltip
        {
            get { throw new NotImplementedException(); }
        }

        public object ContainedObject
        {
            get { throw new NotImplementedException(); }
        }

        public object EntityParent
        {
            get { throw new NotImplementedException(); }
        }

        public string TypeDefinitionString
        {
            get { throw new NotImplementedException(); }
        }
        #endregion

        #region IDisposable
        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            SaveLayout();
            UnsubscribeAll();

            if (runtimeVariables != null && runtimeVariables is IDisposable)
                (runtimeVariables as IDisposable).Dispose();

            listPropertyObserverEntityReferenceResolveVariable.ToList().ForEach(item =>
            {
                item.Dispose();
            });
            listPropertyObserverEntityReferenceResolveVariable.Clear();
        }
        #endregion

        #region Isolated Storage
        String GetStorageName()
        {
            return String.Format("{0}_{1}", parent.Title, title);
        }

        String GetStorageNameGrid()
        {
            return String.Format("{0}_{1}_Grid", parent.Title, title);
        }

        static IsolatedStorageFile GetStorage()
        {
            try
            {
                return IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            }
            catch
            { }

            return null;
        }

        void SaveLayout()
        {
            try
            {
                var isoStorage = GetStorage();
                if (null == isoStorage)
                    return;

                if (datainterface == null)
                {
                    using (var stream = new IsolatedStorageFileStream(GetStorageName(), FileMode.Create, isoStorage))
                    {
                        XmlWriterSettings settings = new XmlWriterSettings
                        {
                            Indent = true,
                            OmitXmlDeclaration = false,
                            Encoding = Encoding.UTF8
                        };

                        using (XmlWriter writer = XmlWriter.Create(stream, settings))
                        {
                            try
                            {
                                var serializer = new DataContractSerializer(typeof(SettingsStorage));
                                var settingStorage = new SettingsStorage();
                                //settingStorage.listTags = (from c in listItems where c.reference != null select c.reference.ToXml()).ToList();
                                settingStorage.listTags = (from c in listItems where c.reference != null select c).ToDictionary(c => c.reference.ToXml(), c => c.Format);
                                serializer.WriteObject(writer, settingStorage);
                            }
                            catch (Exception ex)
                            {
                                writer.Close();
                                throw ex;
                            }
                        }
                    }
                }

                using (var stream = new IsolatedStorageFileStream(GetStorageNameGrid(), FileMode.Create, isoStorage))
                {
                    gridControl.SaveLayoutToStream(stream);
                }
            }
            catch (Exception ex)
            {

            }
        }

        void LoadLayout()
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                return;

            try
            {
                var isoStorage = GetStorage();
                if (null == isoStorage)
                    return;

                if (datainterface == null)
                {
                    using (var stream = new IsolatedStorageFileStream(GetStorageName(), FileMode.OpenOrCreate, isoStorage))
                    {
                        XmlReaderSettings settings = new XmlReaderSettings
                        {
                            ConformanceLevel = ConformanceLevel.Document,
                            CloseInput = true
                        };

                        using (XmlReader reader = XmlReader.Create(stream, settings))
                        {
                            try
                            {
                                var serializer = new DataContractSerializer(typeof(SettingsStorage));
                                var settingsStorage = serializer.ReadObject(reader) as SettingsStorage;
                                settingsStorage.listTags.Keys.ToList().ForEach(item =>
                                {
                                    try
                                    {
                                        var entityReference = item.FromXml<OPCUAEntityReference>();
                                        if (entityReference != null)
                                            AddNewWatchItem(entityReference, settingsStorage.listTags[item]);
                                        else
                                            AddNewWatchItem(item);
                                    }
                                    catch
                                    {
                                        AddNewWatchItem(item);
                                    }
                                });
                            }
                            catch (Exception ex)
                            {
                                reader.Close();
                                throw ex;
                            }
                        }
                    }
                }

                using (var stream = new IsolatedStorageFileStream(GetStorageNameGrid(), FileMode.OpenOrCreate, isoStorage))
                {
                    gridControl.RestoreLayoutFromStream(stream);
                }
            }
            catch (Exception ex)
            {

            }
        }

        #endregion

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteSelectedRows();
        }

        UserControl runtimeVariables;
        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (runtimeVariables == null)
                runtimeVariables = new StartWatchVariable(parent);
            if (runtimeVariables == null)
                return;

            var Dialog = new GeneralDialogContent(runtimeVariables)
            {
                Owner = this.FindParent<Window>(),
                DialogKeepContent = true,
                Title = Properties.Resources.SelectVariable,
                HelpLink = "TagEditor"
            };
            Dialog.SetDialogTheme(ApplicationPropertiesHelper.GetProperty("SolutionSkin") as String);
            if (Dialog.ShowDialog() != true)
                return;
            var result = runtimeVariables.DataContext;
            if (result is List<OPCUAEntityReference>)
            {
                (result as List<OPCUAEntityReference>).ToList().ForEach(tag => TryAddNewWatchItem(tag));
            }
            else if (result is OPCUAEntityReference)
                TryAddNewWatchItem(result as OPCUAEntityReference);
        }

        void TryAddNewWatchItem(OPCUAEntityReference reference)
        {
            var found = (from c in listItems where reference.ToXml() == c.reference.ToXml() select c).ToList();
            if (found.Count == 0)
                AddNewWatchItem(reference, DisplayStringFormat.Default);
            else
                tableView.FocusedRow = found[0];
        }

        private void tableView_ValidateCell(object sender, GridCellValidationEventArgs e)
        {
            if (!(e.Row is ListItem))
                return;

            if (!(e.Column.FieldName == "ColumnValue"))
                return;

            var monitoredItemVM = (e.Row as ListItem).MonitoredItemViewModel;

            if (monitoredItemVM != null)
            {
                try
                {
                    monitoredItemVM.WriteValue(e.Value);
                }
                catch (Exception ex)
                {
                    Console.Beep(300, 200);
                }
            }
        }
    }
}
