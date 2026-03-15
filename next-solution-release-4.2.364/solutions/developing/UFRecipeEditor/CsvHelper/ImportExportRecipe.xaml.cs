using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using CsvHelper;
using log4net;
using Ookii.Dialogs.Wpf;
using UFRecipeSettings.Documents;
using UFRecipeSettings.UFRecipeModel;
using UIMsgBoxAlertService.ComponentService;
using Utilities;
using ViewModelLib;

namespace UFRecipeEditor.CsvHelper
{

    /// <summary>
    /// Interaction logic for ImportExportRecipe..xaml
    /// </summary>
    public partial class ImportExportRecipe : UserControl, IDataErrorInfo
    {
        #region Declarations
        private static readonly ILog log = LogManager.GetLogger(Properties.Resources.ImportExportDialogTitle);

        bool isValidSeparatorChar;
        #endregion

        #region Dependency Properties

        #region FilePath
        public static readonly DependencyProperty FilePathProperty = DependencyProperty.Register("FilePath", typeof(string), typeof(ImportExportRecipe), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnFilePathChanged), new CoerceValueCallback(OnCoerceFilePath)));

        private static object OnCoerceFilePath(DependencyObject o, object value)
        {
            ImportExportRecipe control = o as ImportExportRecipe;
            if (control != null)
                return control.OnCoerceFilePath((string)value);
            else
                return value;
        }

        private static void OnFilePathChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ImportExportRecipe control = o as ImportExportRecipe;
            if (control != null)
                control.OnFilePathChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceFilePath(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnFilePathChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public string FilePath
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(FilePathProperty);
            }
            set
            {
                SetValue(FilePathProperty, value);
            }
        }

        #endregion

        #region SeparatorChar
        public static readonly DependencyProperty SeparatorCharProperty = DependencyProperty.Register("SeparatorChar", typeof(string), typeof(ImportExportRecipe), new UIPropertyMetadata("|", new PropertyChangedCallback(OnSeparatorCharChanged), new CoerceValueCallback(OnCoerceSeparatorChar)));

        private static object OnCoerceSeparatorChar(DependencyObject o, object value)
        {
            ImportExportRecipe control = o as ImportExportRecipe;
            if (control != null)
                return control.OnCoerceSeparatorChar((string)value);
            else
                return value;
        }

        private static void OnSeparatorCharChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ImportExportRecipe control = o as ImportExportRecipe;
            if (control != null)
                control.OnSeparatorCharChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceSeparatorChar(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnSeparatorCharChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public string SeparatorChar
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(SeparatorCharProperty);
            }
            set
            {
                SetValue(SeparatorCharProperty, value);
            }
        }

        #endregion

        #region SeparatorLenght
        public static readonly DependencyProperty SeparatorLenghtProperty = DependencyProperty.Register("SeparatorLenght", typeof(int), typeof(ImportExportRecipe), new UIPropertyMetadata(Properties.Settings.Default.CsvImportExportMaxSeparatorLenght, new PropertyChangedCallback(OnSeparatorLenghtChanged), new CoerceValueCallback(OnCoerceSeparatorLenght)));

        private static object OnCoerceSeparatorLenght(DependencyObject o, object value)
        {
            ImportExportRecipe control = o as ImportExportRecipe;
            if (control != null)
                return control.OnCoerceSeparatorLenght((int)value);
            else
                return value;
        }

        private static void OnSeparatorLenghtChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ImportExportRecipe control = o as ImportExportRecipe;
            if (control != null)
                control.OnSeparatorLenghtChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceSeparatorLenght(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnSeparatorLenghtChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public int SeparatorLenght
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(SeparatorLenghtProperty);
            }
            set
            {
                SetValue(SeparatorLenghtProperty, value);
            }
        }

        #endregion

        #region SeparatorOption
        public static readonly DependencyProperty SeparatorOptionProperty = DependencyProperty.Register("SeparatorOption", typeof(ImportExportSeparatorOptions), typeof(ImportExportRecipe), new UIPropertyMetadata(ImportExportSeparatorOptions.Semicolon, new PropertyChangedCallback(OnSeparatorOptionChanged), new CoerceValueCallback(OnCoerceSeparatorOption)));

        private static object OnCoerceSeparatorOption(DependencyObject o, object value)
        {
            ImportExportRecipe control = o as ImportExportRecipe;
            if (control != null)
                return control.OnCoerceSeparatorOption((ImportExportSeparatorOptions)value);
            else
                return value;
        }

        private static void OnSeparatorOptionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ImportExportRecipe control = o as ImportExportRecipe;
            if (control != null)
                control.OnSeparatorOptionChanged((ImportExportSeparatorOptions)e.OldValue, (ImportExportSeparatorOptions)e.NewValue);
        }

        protected virtual ImportExportSeparatorOptions OnCoerceSeparatorOption(ImportExportSeparatorOptions value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnSeparatorOptionChanged(ImportExportSeparatorOptions oldValue, ImportExportSeparatorOptions newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && (oldValue == ImportExportSeparatorOptions.Custom || newValue == ImportExportSeparatorOptions.Custom))
            {
                var binding = txtSeparatorChar.GetBindingExpression(TextBox.TextProperty);
                if (binding != null)
                    binding.UpdateSource();
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            }
        }

        public ImportExportSeparatorOptions SeparatorOption
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (ImportExportSeparatorOptions)GetValue(SeparatorOptionProperty);
            }
            set
            {
                SetValue(SeparatorOptionProperty, value);
            }
        }

        #endregion

        #region ImportActionType
        public static readonly DependencyProperty ImportActionTypeProperty = DependencyProperty.Register("ImportActionType", typeof(ImportActionType), typeof(ImportExportRecipe), new UIPropertyMetadata(ImportActionType.CleanBefore, new PropertyChangedCallback(OnSeparatorOptionChanged), new CoerceValueCallback(OnCoerceImportActionType)));

        private static object OnCoerceImportActionType(DependencyObject o, object value)
        {
            ImportExportRecipe control = o as ImportExportRecipe;
            if (control != null)
                return control.OnCoerceImportActionType((ImportActionType)value);
            else
                return value;
        }

        private static void OnImportActionTypeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ImportExportRecipe control = o as ImportExportRecipe;
            if (control != null)
                control.OnImportActionTypeChanged((ImportActionType)e.OldValue, (ImportActionType)e.NewValue);
        }

        protected virtual ImportActionType OnCoerceImportActionType(ImportActionType value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnImportActionTypeChanged(ImportActionType oldValue, ImportActionType newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public ImportActionType ImportActionType
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (ImportActionType)GetValue(ImportActionTypeProperty);
            }
            set
            {
                SetValue(ImportActionTypeProperty, value);
            }
        }

        #endregion

        #endregion

        #region Constructors
        public ImportExportRecipe()
        {
            InitializeComponent();

            comboSeparatorType.ItemsSource = Enum.GetValues(typeof(ImportExportSeparatorOptions));
            isValidSeparatorChar = true;

            Loaded += (s, e) =>
            {
                var recipeDocument = DataContext as UFRecipeDocument;
                if (recipeDocument == null)
                    return;

                LoadLayout(recipeDocument.Title);
            };

            Unloaded += (s, e) =>
            {
                var recipeDocument = DataContext as UFRecipeDocument;
                if (recipeDocument == null)
                    return;

                SaveLayout(recipeDocument.Title);
            };
        }
        #endregion

        #region Events
        public event EventHandler<CsvResultEventArgs> Executed;
        #endregion

        #region Commands
        private void SelectFile_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            VistaOpenFileDialog dialog = new VistaOpenFileDialog();
            dialog.CheckFileExists = false;
            dialog.ValidateNames = true;
            dialog.AddExtension = true;
            dialog.DefaultExt = "csv";
            dialog.FileName = FilePath;
            dialog.Filter = Properties.Resources.ImportExportCsvFilter;
            if (dialog.ShowDialog() == true)
                FilePath = dialog.FileName;
        }

        RelayCommand exportCommand;
        public ICommand ExportCommand
        {
            get
            {
                if (exportCommand == null)
                {
                    exportCommand = new RelayCommand(
                        param => Export(),
                        param => !String.IsNullOrEmpty(FilePath) && isValidSeparatorChar
                        );
                }
                return exportCommand;
            }
        }

        void Export()
        {
            var recipeDocument = DataContext as UFRecipeDocument;
            if (recipeDocument == null || String.IsNullOrEmpty(FilePath))
                return;

            var recipeEntity = recipeDocument.RecipeEntity.CreateSnapshot();
            var uiInterface = recipeDocument.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
            
            bool bError = false;
            var watcher = Stopwatch.StartNew();
            try
            {
                txtOutput.Text = String.Format(Properties.Resources.CsvExportStarted, DateTime.Now.TimeOfDay);
                using (new WaitCursor())
                {
                    using (var writer = new StreamWriter(FilePath))
                    {
                        using (var csv = new CsvWriter(writer))
                        {
                            try
                            {
                                csv.Configuration.Delimiter = GetCurrentSeparator();
                                // Register class map for recipe's types.
                                csv.Configuration.RegisterClassMap<UFRecipeEntityMap>();
                                csv.Configuration.RegisterClassMap<UFGroupEntityMap>();
                                csv.Configuration.RegisterClassMap<UFDataValueEntityMap>();

                                // Write recipe's info.
                                csv.WriteHeader<UFRecipeEntity>();
                                csv.NextRecord();
                                csv.WriteRecord<UFRecipeEntity>(recipeEntity);
                                csv.NextRecord();

                                // Write recipes's groups.
                                if (recipeEntity.Groups.Count > 0)
                                {
                                    csv.NextRecord();
                                    csv.WriteHeader<UFGroupEntity>();
                                    csv.NextRecord();
                                    csv.WriteRecords(recipeEntity.Groups);
                                }

                                // Write recipe's values.
                                var dataValues = recipeEntity.GetFlatDataValuesCollection();
                                if (dataValues.Count > 0)
                                {
                                    csv.NextRecord();
                                    csv.WriteHeader<UFDataValueEntity>();
                                    csv.NextRecord();
                                    csv.WriteRecords(dataValues);
                                }
                            }
                            catch (Exception ex)
                            {
                                bError = true;
                                AppendTraceMessage(String.Format(Properties.Resources.CsvExportErrorOnWriting, csv.Context.Row, ex.Message));
                                throw;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (!bError)
                {
                    bError = true;
                    AppendTraceMessage(ex);
                }
            }
            finally
            {
                watcher.Stop();
                AppendTraceMessage(String.Format(Properties.Resources.CsvExportTerminated, watcher.Elapsed));
            }

            OnExportExecuted(!bError);

            if (bError && uiInterface != null)
                uiInterface.ShowError(String.Format(Properties.Resources.CsvExportFailed, recipeEntity.Name));
        }

        RelayCommand importCommand;
        public ICommand ImportCommand
        {
            get
            {
                if (importCommand == null)
                {
                    importCommand = new RelayCommand(
                        param => Import(),
                        param => !String.IsNullOrEmpty(FilePath) && isValidSeparatorChar
                        );
                }
                return importCommand;
            }
        }

        void Import()
        {
            var recipeDocument = DataContext as UFRecipeDocument;
            if (recipeDocument == null || String.IsNullOrEmpty(FilePath))
                return;

            var uiInterface = recipeDocument.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
            if (uiInterface != null && ImportActionType == ImportActionType.CleanBefore &&
                uiInterface.ShowYesNo(Properties.Resources.CsvImportAskForClean, CustomDialogIcons.Question) != CustomDialogResults.Yes)
                return;

            var originalRecipe = recipeDocument.RecipeEntity;
            var recipeEntity = recipeDocument.RecipeEntity = recipeDocument.RecipeEntity.CreateSnapshot();
            if (ImportActionType == ImportActionType.CleanBefore)
            {
                recipeEntity.Groups.Clear();
                recipeEntity.DataValues.Clear();
            }

            bool bError = false;
            var watcher = Stopwatch.StartNew();
            try
            {
                txtOutput.Text = String.Format(Properties.Resources.CsvImportStarted, DateTime.Now.TimeOfDay);
                using (new WaitCursor())
                {
                    using (var reader = new StreamReader(FilePath))
                    {
                        using (var csv = new CsvReader(reader))
                        {
                            try
                            {
                                csv.Configuration.Delimiter = GetCurrentSeparator();
                                csv.Configuration.MissingFieldFound = null;

                                // Register class map for recipe's types.
                                var classMapRecipe = csv.Configuration.RegisterClassMap<UFRecipeEntityMap>();
                                classMapRecipe.Warning += (s, ev) =>
                                {
                                    AppendTraceMessage(ev.GetException());
                                };

                                var classMapGroup = csv.Configuration.RegisterClassMap<UFGroupEntityMap>();
                                classMapGroup.Warning += (s, ev) =>
                                {
                                    AppendTraceMessage(ev.GetException());
                                };

                                var classMapDataValue = csv.Configuration.RegisterClassMap<UFDataValueEntityMap>();
                                classMapDataValue.Warning += (s, ev) =>
                                {
                                    AppendTraceMessage(ev.GetException());
                                };

                                List<UFDataValueEntity> dataValues = null;
                                Dictionary<String, UFGroupEntity> mapGroups = null;
                                while (csv.Read())
                                {
                                    if (String.IsNullOrEmpty(csv.GetField(0)))
                                        continue;
                                    else if (csv.GetField(0).StartsWith(ClassMapConstant.HeaderStartingText))
                                    {
                                        csv.ReadHeader();
                                        continue;
                                    }
                                    else if (csv.Context.HeaderRecord == null)
                                        continue;

                                    switch (csv.Context.HeaderRecord[0].Substring(ClassMapConstant.HeaderStartingText.Length))
                                    {
                                        case "RecipeName":
                                            {
                                                var csvRecipeEntity = csv.GetRecord<UFRecipeEntity>();
                                                classMapRecipe.Merge(csvRecipeEntity, recipeEntity);
                                                break;
                                            }
                                        case "GroupName":
                                            {
                                                var csvGroupEntity = csv.GetRecord<UFGroupEntity>();
                                                var groupEntity = (from c in recipeEntity.Groups.AsParallel()
                                                                   where c.Name == csvGroupEntity.Name
                                                                   select c).FirstOrDefault();
                                                if (groupEntity == null)
                                                {
                                                    groupEntity = recipeDocument.AddNewGroup(csvGroupEntity.Name);
                                                    recipeEntity.Groups.Add(groupEntity);
                                                }
                                                else if (ImportActionType == ImportActionType.SkipDuplicates)
                                                {
                                                    AppendTraceMessage(String.Format(Properties.Resources.CsvImportSkipDuplicateGroup, csvGroupEntity.Name));
                                                    break;
                                                }
                                                if (mapGroups == null)
                                                    mapGroups = new Dictionary<String, UFGroupEntity>();
                                                if (!mapGroups.ContainsKey(groupEntity.Name))
                                                    mapGroups.Add(groupEntity.Name, groupEntity);
                                                classMapGroup.Merge(csvGroupEntity, groupEntity);
                                                break;
                                            }
                                        case "DataValueName":
                                            {
                                                var csvDataValueEntity = csv.GetRecord<UFDataValueEntity>();

                                                UFGroupEntity groupEntity = null;
                                                if (csvDataValueEntity.UFGroupAss != null &&
                                                    !String.IsNullOrEmpty(csvDataValueEntity.UFGroupAss.Name))
                                                {
                                                    if (mapGroups == null)
                                                        mapGroups = new Dictionary<String, UFGroupEntity>();
                                                    if (!mapGroups.ContainsKey(csvDataValueEntity.UFGroupAss.Name))
                                                    {
                                                        groupEntity = (from c in recipeEntity.Groups.AsParallel()
                                                                       where c.Name == csvDataValueEntity.UFGroupAss.Name
                                                                       select c).FirstOrDefault();
                                                        if (groupEntity == null)
                                                        {
                                                            groupEntity = recipeDocument.AddNewGroup(csvDataValueEntity.UFGroupAss.Name);
                                                            recipeEntity.Groups.Add(groupEntity);
                                                        }
                                                        if (!mapGroups.ContainsKey(groupEntity.Name))
                                                            mapGroups.Add(groupEntity.Name, groupEntity);
                                                    }
                                                    else
                                                        groupEntity = mapGroups[csvDataValueEntity.UFGroupAss.Name];
                                                }

                                                if (dataValues == null)
                                                    dataValues = recipeEntity.GetFlatDataValuesCollection();
                                                var dataValueEntity = (from c in dataValues.AsParallel()
                                                                       where c.Name == csvDataValueEntity.Name &&
                                                                       (c.UFGroupAss == null && csvDataValueEntity.UFGroupAss == null ||
                                                                       (c.UFGroupAss != null && csvDataValueEntity.UFGroupAss != null && c.UFGroupAss.Name == csvDataValueEntity.UFGroupAss.Name))
                                                                       select c).FirstOrDefault();
                                                if (dataValueEntity == null)
                                                {
                                                    dataValueEntity = recipeDocument.AddNewDataValue(groupEntity, csvDataValueEntity.Name);
                                                    dataValues.Add(dataValueEntity);
                                                    if (groupEntity != null)
                                                        groupEntity.DataValues.Add(dataValueEntity);
                                                    else
                                                        recipeEntity.DataValues.Add(dataValueEntity);
                                                }
                                                else if (ImportActionType == ImportActionType.SkipDuplicates)
                                                {
                                                    AppendTraceMessage(String.Format(Properties.Resources.CsvImportSkipDuplicateDataValue, csvDataValueEntity.Name));
                                                    break;
                                                }
                                                classMapDataValue.Merge(csvDataValueEntity, dataValueEntity);
                                                break;
                                            }
                                        default:
                                            AppendTraceMessage(String.Format(Properties.Resources.CsvImportUnknownRecordType, csv.Context.Row));
                                            break;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                bError = true;
                                AppendTraceMessage(String.Format(Properties.Resources.CsvImportInvalidRecord, csv.Context.Row, ex.Message));
                                throw;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (!bError)
                {
                    bError = true;
                    AppendTraceMessage(ex);
                }
            }
            finally
            {
                watcher.Stop();
                AppendTraceMessage(String.Format(Properties.Resources.CsvImportTerminated, watcher.Elapsed));
                recipeDocument.RecipeEntity = originalRecipe;
            }

            OnImportExecuted(recipeEntity, !bError);

            if (bError && uiInterface != null)
                uiInterface.ShowError(String.Format(Properties.Resources.CsvImportFailed, recipeEntity.Name));
        }
        #endregion

        #region Methods
        string GetCurrentSeparator()
        {
            if (SeparatorOption == ImportExportSeparatorOptions.Colon)
                return ":";
            else if (SeparatorOption == ImportExportSeparatorOptions.Comma)
                return ",";
            else if (SeparatorOption == ImportExportSeparatorOptions.Semicolon)
                return ";";
            else if (SeparatorOption == ImportExportSeparatorOptions.Tab)
                return "\t";
            else
                return SeparatorChar;
        }

        void OnImportExecuted(UFRecipeEntity recipeEntity, bool successfully)
        {
            var e = Executed;
            if (e != null)
                e(this, new CsvResultEventArgs(recipeEntity, FilePath, OperationType.Import, successfully));
        }

        void OnExportExecuted(bool successfully)
        {
            var e = Executed;
            if (e != null)
                e(this, new CsvResultEventArgs(null, FilePath, OperationType.Export, successfully));
        }

        void AppendTraceMessage(Exception ex)
        {
            AppendTraceMessage(ex.Message);
        }

        void AppendTraceMessage(String message)
        {
            log.Info(message);
            txtOutput.Dispatcher.InvokeIfRequired(() =>
            {
                message = message.Replace(Environment.NewLine, " ");
                if (String.IsNullOrEmpty(txtOutput.Text))
                    txtOutput.Text += message;
                else
                    txtOutput.Text += String.Format("{0}{1}", Environment.NewLine, message);
                scroll.ScrollToBottom();
            });
        }

        protected String PerformValidation(String propertyName)
        {
            if (propertyName == "SeparatorChar")
            {
                if (SeparatorOption == ImportExportSeparatorOptions.Custom)
                {
                    if (String.IsNullOrEmpty(SeparatorChar))
                        return Properties.Resources.CsvEmptySeparator;
                    else if (SeparatorLenght > 0 && SeparatorChar.Length > SeparatorLenght)
                        return Properties.Resources.CsvTooLongSeparator;
                    else if (SeparatorChar == ClassMapConstant.ArraySeparatorChar.ToString())
                        return Properties.Resources.CsvReservedSeparator;

                    using (var stream = new MemoryStream())
                    {
                        using (var writer = new StreamWriter(stream))
                        {
                            using (var csv = new CsvWriter(writer))
                            {
                                try
                                {
                                    csv.Configuration.Delimiter = GetCurrentSeparator();
                                    isValidSeparatorChar = true;
                                }
                                catch (Exception ex)
                                {
                                    isValidSeparatorChar = false;
                                    return string.Format(Properties.Resources.CsvInvalidSeparator, ex.Message);
                                }
                            }
                        }
                    }
                }
                else
                    isValidSeparatorChar = true;
            }                

            return null;
        }
        #endregion

        #region Isolated Storage

        static String GetStoreFileName(String title)
        {
            return String.Format("{0}.{1}.CsvImportExport.dat", title, System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));
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

        void SaveLayout(String title)
        {
            try
            {
                var isoStorage = GetStorage();
                if (null == isoStorage || string.IsNullOrEmpty(title))
                    return;

                using (var stream = new IsolatedStorageFileStream(GetStoreFileName(title), FileMode.Create, isoStorage))
                {
                    XmlWriterSettings settings = new XmlWriterSettings
                    {
                        Indent = true,
                        OmitXmlDeclaration = false,
                        Encoding = Encoding.UTF8
                    };

                    using (XmlWriter writer = XmlWriter.Create(stream, settings))
                    {
                        var serializer = new DataContractSerializer(typeof(SettingsStorage));
                        var settingsStorage = new SettingsStorage();
                        settingsStorage.FilePath = FilePath;
                        settingsStorage.SeparatorOption = SeparatorOption;
                        settingsStorage.SeparatorChar = SeparatorChar;
                        settingsStorage.ImportActionType = ImportActionType;
                        serializer.WriteObject(writer, settingsStorage);
                    }
                }
            }
            catch
            { }
        }

        void LoadLayout(String title)
        {
            try
            {
                var isoStorage = GetStorage();
                if (null == isoStorage || string.IsNullOrEmpty(title))
                    return;

                using (var stream = new IsolatedStorageFileStream(GetStoreFileName(title), FileMode.OpenOrCreate, isoStorage))
                {
                    XmlReaderSettings settings = new XmlReaderSettings
                    {
                        ConformanceLevel = ConformanceLevel.Document,
                        CloseInput = true
                    };

                    using (XmlReader reader = XmlReader.Create(stream, settings))
                    {
                        var serializer = new DataContractSerializer(typeof(SettingsStorage));
                        var settingsStorage = serializer.ReadObject(reader) as SettingsStorage;
                        FilePath = settingsStorage.FilePath;
                        SeparatorOption = settingsStorage.SeparatorOption;
                        SeparatorChar = settingsStorage.SeparatorChar;
                        ImportActionType = settingsStorage.ImportActionType;
                    }
                }
            }
            catch
            { }
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

        [Browsable(false)]
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
    }

    [DataContract(Name = "SettingsStorage")]
    class SettingsStorage
    {
        #region Members
        [DataMember]
        public String FilePath;
        [DataMember]
        public ImportExportSeparatorOptions SeparatorOption;
        [DataMember]
        public String SeparatorChar;
        [DataMember]
        public ImportActionType ImportActionType;
        #endregion
    }
}
