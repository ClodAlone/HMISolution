using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Utilities;
using Utilities.WPF;
using UFInterfaces;
using DocumentManager.ComponentService;
using UFUAEditor.ComponentService;
using UFInterfaces.Editors;
using System.Reflection;
using DriverSettingsInterfaces;

namespace UFUAEditor.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for ListDynamicSettingsEditor.xaml
    /// </summary>
    public partial class ListDynamicSettingsEditor : UserControl
    {
        #region Declaration
        readonly ObservableCollection<DynamicSettingsWrapperObject> ListEnum = new ObservableCollection<DynamicSettingsWrapperObject>();
        readonly IWorkspace workspace;
        readonly IDynamicSettingsEditing dynObject;
        #endregion

        #region Constructors
        public ListDynamicSettingsEditor(IList<String> listEnum, IWorkspace workspace, IDynamicSettingsEditing dynObject)
        {
            InitializeComponent();

            this.workspace = workspace;
            this.dynObject = dynObject;

            if (this.dynObject == null)
            {
                if (workspace.ContextObject != null)
                    this.dynObject = workspace.ContextObject as IDynamicSettingsEditing;
                else if (workspace.ContextObjects != null && workspace.ContextObjects.Count > 0)
                    this.dynObject = workspace.ContextObjects[0] as IDynamicSettingsEditing;
            }

            foreach (var item in listEnum)
                ListEnum.Add(new DynamicSettingsWrapperObject(ListEnum, this.dynObject, item));
            layoutGrid.DataContext = ListEnum;
        }
        #endregion

        #region Commands
        public static readonly RoutedCommand EditCommand = new RoutedCommand();
        public static readonly RoutedCommand RemoveCommand = new RoutedCommand();
        public static readonly RoutedCommand MoveUpCommand = new RoutedCommand();
        public static readonly RoutedCommand MoveDownCommand = new RoutedCommand();

        public static readonly RoutedCommand AddNewCommand = new RoutedCommand();
        public static readonly RoutedCommand RemoveAllCommand = new RoutedCommand();
        #endregion

        #region Methods
        void OnCanExecuteEditCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        void OnEditCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            Button button = (Button)e.OriginalSource;

            DynamicSettingsWrapperObject value = (DynamicSettingsWrapperObject)(button.Tag);
            if (value != null)
            {
                if (workspace != null)
                {
                    var doc = workspace.ContextDocument as IDocument;
                    if (doc != null && dynObject != null)
                    {
                        IUFUAEditorManager ufuaEditorManager = doc.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                        if (ufuaEditorManager != null)
                        {
                            var wrapper = new DynamicSettingsWrapperObject(dynObject, value.Value);
                            var dynamicSettings = ufuaEditorManager.GetDynamicSettingsControl(doc, wrapper);
                            if (dynamicSettings == null)
                                return;

                            GeneralDialogContent Dialog = new GeneralDialogContent(dynamicSettings)
                            {
                                Title = Properties.Resources.DynamicSettingsTitle,
                                Owner = this.FindParent<Window>()
                            };

                            var ret = (Dialog.ShowDialog() == true);
                            Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                            {
                                if (ret)
                                {
                                    var text = wrapper.DynamicSettingsForEditing;
                                    if (!String.IsNullOrEmpty(text))
                                    {
                                        var index = ListEnum.IndexOf(value);
                                        ListEnum.Insert(index, new DynamicSettingsWrapperObject(ListEnum, dynObject, text));
                                        ListEnum.Remove(value);
                                    }
                                }

                                if (dynamicSettings is IDisposable)
                                    (dynamicSettings as IDisposable).Dispose();
                            });
                        }
                    }
                }
            }
        }

        void OnCanExecuteRemoveCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        void OnRemoveCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            Button button = (Button)e.OriginalSource;

            DynamicSettingsWrapperObject value = (DynamicSettingsWrapperObject)(button.Tag);
            if (value != null)
                ListEnum.Remove(value);

            foreach (var item in ListEnum)
                item.ForceNotifyPropertyChanged();
        }

        void OnCanExecuteMoveUpCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;

            Button button = (Button)e.OriginalSource;

            DynamicSettingsWrapperObject value = (DynamicSettingsWrapperObject)(button.Tag);
            e.CanExecute = value != null && ListEnum.IndexOf(value) > 0;
        }

        void OnMoveUpCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            Button button = (Button)e.OriginalSource;

            DynamicSettingsWrapperObject value = (DynamicSettingsWrapperObject)(button.Tag);
            if (value != null)
            {
                var index = ListEnum.IndexOf(value);
                if (index > 0)
                    ListEnum.Move(index, index - 1);
            }
        }

        void OnCanExecuteMoveDownCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            Button button = (Button)e.OriginalSource;

            DynamicSettingsWrapperObject value = (DynamicSettingsWrapperObject)(button.Tag);
            e.CanExecute = value != null && ListEnum.IndexOf(value) < ListEnum.Count - 1;
        }

        void OnMoveDownCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            Button button = (Button)e.OriginalSource;

            DynamicSettingsWrapperObject value = (DynamicSettingsWrapperObject)(button.Tag);
            if (value != null)
            {
                var index = ListEnum.IndexOf(value);
                if (index < ListEnum.Count - 1)
                    ListEnum.Move(index, index + 1);
            }
        }

        void OnCanExecuteAddNewCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ListEnum != null && dynObject != null;
        }

        void OnAddNewCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            ListEnum.Add(new DynamicSettingsWrapperObject(ListEnum, dynObject));
        }

        void OnCanExecuteRemoveAllCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ListEnum != null && ListEnum.Count > 0;
        }

        void OnRemoveAllCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            ListEnum.Clear();
        }
        #endregion

        #region Properties
        public String[] CurrentEnums
        {
            get
            {
                var ret = new String[ListEnum.Count];
                for (int ii = 0; ii < ret.Length; ii++)
                    ret[ii] = ListEnum[ii].Value;

                return ret;
            }
        }
        #endregion
    }

    public class DynamicSettingsWrapperObject : StringWrapperObject, IDynamicSettingsEditing, IDataErrorInfo
    {
        #region Declarations
        readonly IEnumerable owner;
        readonly IDynamicSettingsEditing dynObject;
        #endregion

        #region Constructors
        public DynamicSettingsWrapperObject(IDynamicSettingsEditing dynObject, String dynamicSettings) :
            this(null, dynObject, dynamicSettings)
        { }

        public DynamicSettingsWrapperObject(IEnumerable owner, IDynamicSettingsEditing dynObject) :
            this(owner, dynObject,  null)
        { }

        public DynamicSettingsWrapperObject(IEnumerable owner, IDynamicSettingsEditing dynObject, String dynamicSettings) : 
            base(dynamicSettings)
        {
            this.owner = owner;
            this.dynObject = dynObject;
        }
        #endregion

        #region Public Methods
        public void ForceNotifyPropertyChanged()
        {
            OnPropertyChanged("Value");
        }
        #endregion

        #region IDynamicSettingsEditing
        public string Name { get { return dynObject.Name; } }

        [Browsable(false)]
        public string FolderPath
        {
            get { return string.Empty; }
        }
        [Browsable(false)]
        public string TagOwnerPath
        {
            get { return string.Empty; }
        }
        public uint ArrayDimension
        {
            get
            {
                return dynObject.ArrayDimension;
            }
            set
            {
                if (dynObject.ArrayDimension == value)
                    return;

                dynObject.ArrayDimension = value;
                OnPropertyChanged("ArrayDimension");
            }
        }

        public int DataType
        {
            get
            {
                return dynObject.DataType;
            }

            set
            {
                if (dynObject.DataType == value)
                    return;

                dynObject.DataType = value;
                OnPropertyChanged("DataType");
            }
        }

        public string DynamicSettingsForEditing
        {
            get
            {
                return Value;
            }
            set
            {
                if (Value == value)
                    return;

                Value = value;
                OnPropertyChanged("DynamicSettingsForEditing");
            }
        }

        public bool IsMethod
        {
            get
            {
                return dynObject.IsMethod;
            }
        }

        public bool IsObjectType
        {
            get
            {
                return dynObject.IsObjectType;
            }
        }

        public IList<IDynamicSettingsEditing> Members
        {
            get
            {
                return dynObject.Members;
            }
        }

        #endregion

        #region IDataErrorInfo
        [Browsable(false)]
        public string Error
        {
            get
            {
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null);
                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

                return !System.ComponentModel.DataAnnotations.Validator.TryValidateObject(this, context, results)
                    ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                    : null;
            }
        }

        public string this[string propertyName]
        {
            get
            {
                String s = PerformValidation(propertyName);
                if (!String.IsNullOrEmpty(s))
                    return s;
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null)
                {
                    MemberName = propertyName
                };

                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                var propertyInfo = GetType().GetProperty(propertyName);
                if (propertyInfo != null)
                {
                    var value = propertyInfo.GetValue(this, null);

                    return !System.ComponentModel.DataAnnotations.Validator.TryValidateProperty(value, context, results)
                        ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                        : null;
                }

                return null;
            }
        }
        #endregion

        #region Methods
        protected String PerformValidation(String propertyName, String prototypeName = null)
        {
            if (propertyName == "Value" && !String.IsNullOrWhiteSpace(Value))
            {
                try
                {
                    var driverName = Value;
                    if (driverName.Contains('.'))
                        driverName = driverName.Substring(0, Value.IndexOf('.'));
                    var uidll = UFUAServerInfo.UFUAServerInfo.GetDriversUIName(String.Format("{0}\\{1}.dll", UFUAServerInfo.UFUAServerInfo.GetDriversFolder(), driverName));
                    if (!System.IO.File.Exists(uidll))
                        return String.Format(Properties.Resources.InvalidDynamcSettingsMissingDriver, driverName);

                    string error = null;
                    if (dynObject != null)
                    {
                        var types = Assembly.LoadFile(uidll).GetTypes();
                        var list2 = (from t in types/*.AsParallel()*/
                                     where !t.IsAbstract && typeof(ICommunicationDriverWpfEditing2).IsAssignableFrom(t)
                                     select (ICommunicationDriverWpfEditing2)Activator.CreateInstance(t)).ToList();
                        if (list2.Count > 0)
                        {
                            error = list2[0].CheckDynamicAddress(Value, dynObject);
                            if (!String.IsNullOrEmpty(error))
                                return error;
                        }
                        else
                        {
                            var list = (from t in types/*.AsParallel()*/
                                        where !t.IsAbstract && typeof(ICommunicationDriverWpfEditing).IsAssignableFrom(t)
                                        select (ICommunicationDriverWpfEditing)Activator.CreateInstance(t)).ToList();
                            if (list.Count > 0)
                            {
                                if (dynObject.IsObjectType)
                                    error = list[0].CheckDynamicAddress(Value, unchecked((uint)(-1)), 0);
                                else if (dynObject.IsMethod)
                                    error = list[0].CheckDynamicAddress(Value, unchecked((uint)(-2)), 0);
                                else
                                    error = list[0].CheckDynamicAddress(Value, dynObject.DataType < 0 ? (uint)UFUAModel.DataType.Byte : (uint)dynObject.DataType, dynObject.ArrayDimension);
                                if (!String.IsNullOrEmpty(error))
                                    return error;
                            }
                            else
                            {
                                return String.Format(Properties.Resources.InvalidDynamcSettingsInvalidDriver, driverName);
                            }
                        }
                    }

                    if (owner != null)
                    {
                        foreach (var item in owner)
                        {
                            var wrapperObject = item as StringWrapperObject;
                            if (wrapperObject != null)
                            {
                                if (object.ReferenceEquals(wrapperObject.Value, Value))
                                    continue;

                                var itemDriverName = wrapperObject.Value;
                                if (itemDriverName.Contains('.'))
                                    itemDriverName = itemDriverName.Substring(0, wrapperObject.Value.IndexOf('.'));

                                if (driverName == itemDriverName)
                                    return String.Format(Properties.Resources.InvalidDynamcSettingsDuplicatedDriver, driverName);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    return String.Format(Properties.Resources.InvalidDynamcSettingsUnexpectedError, ex.Message);
                }
            }

            return null;
        }
        #endregion
    }
}
