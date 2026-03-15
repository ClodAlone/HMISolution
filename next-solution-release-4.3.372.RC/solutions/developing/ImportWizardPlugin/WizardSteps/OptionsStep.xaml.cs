using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ScreenManager;
using UriResolver.ComponentService;
using System.IO;
using System.Xml;
using System.Windows.Markup;
using System.Collections.ObjectModel;
using Utilities.WPF;
using Utilities;
using System.ComponentModel;
using ImportWizardPlugin.ComponentService;
using VFS;
using System.Drawing;
using DocumentManager.ComponentService;
using System.Text;
using System.Xml.Linq;
using ScreenSettings;
using DevExpress.Xpo.DB;
using DevExpress.Xpo;
using System.Dynamic;
using System.Diagnostics;
using UFInterfaces.Editors;
using System.Reflection;
using ImportWizardPlugin.Settings;
using DriverCodeBase;
using DriverCodeBase.Helpers;
using DriverCodeBase.Enumerators;
using DevExpress.Xpf.Editors;
using System.Globalization;
using UFProjectManager.ComponentService;
using System.Threading.Tasks;
using System.Threading;
using log4net;


namespace ImportWizardPlugin
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>

    public enum OldCondition
    {
        GreaterThanOrEqual,
        LessThanOrEqual,
        Equals,
        RateChangeDecrease,
        RateChangeIncrease,
        NotEqual,
        Between
    }
    public enum OldDataType
    {
        Bit,
        SByte,
        Byte,
        SWord,
        Word,
        SDWord,
        DWord,
        Float,
        Double,
        String,
        FixedLenghtArray,
        Structure
    }
    public enum OldFunctionCode
    {
        Coils,
        InputDiscretes,
        MultipleRegisters,
        InputRegisters,
        SingleCols,
        SingleRegisters,
        FileRecord,
        ExceptionStatus
    }
    public enum OldFlowControl
    {
        None,
        HW,
        XonXoff,
        NoneSignDisabled,
        RTSToggle
    }

    class TagOptions
    {
        private UFUAModel.DataType _dType = UFUAModel.DataType.Float;
        private string _ivalue = string.Empty;
        private string _desc = string.Empty;
        private string _unit = string.Empty;
        private bool _sByte = false;
        private bool _sWord = false;
        private int _bit = 0;
        private int _byte = 0;
        private string _station = string.Empty;
        public string Desc
        {
            get
            {
                return _desc;
            }
            set
            {
                _desc = value;
            }
        }
        public string IValue
        {
            get
            {
                return _ivalue;
            }
            set
            {
                _ivalue = value;
            }
        }
        public string Unit
        {
            get
            {
                return _unit;
            }
            set
            {
                _unit = value;
            }
        }
        public UFUAModel.DataType DType
        {
            get
            {
                return _dType;
            }
            set
            {
                _dType = value;
            }
        }
        public bool SByte
        { 
            get
            {
                return _sByte;
            } 
            set
            {
                _sByte = value;
            }
        }
        public bool SWord
        { 
            get
            {
                return _sWord;
            } 
            set
            {
                _sWord = value;
            }
        }
        public int Bit
        { 
            get
            {
                return _bit;
            } 
            set
            {
                _bit = value;
            }
        }
        public int Byte
        { 
            get
            {
                return _byte;
            } 
            set
            {
                _byte = value;
            }
        }
        public string Station
        {
            get
            {
                return _station;
            }
            set
            {
                _station = value;
            }
        }
    }

    class StringOptions
    {
        private string _fileName = string.Empty;
        private CultureInfo _culture = CultureInfo.CurrentUICulture;
        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
            }
        }
        public CultureInfo Culture
        {
            get
            {
                return _culture;
            }
            set
            {
                _culture = value;
            }
        }
    }

    public partial class ProjectWizardOptionsTemplate : UserControl, IWizardElement, IDisposable
    {
        #region DP

        #region TextMessage
        public static readonly DependencyProperty TextMessageProperty = DependencyProperty.Register("TextMessage", typeof(string), typeof(ProjectWizardOptionsTemplate), new UIPropertyMetadata(Properties.Resources.DisclaimerTitle, new PropertyChangedCallback(OnTextMessageChanged), new CoerceValueCallback(OnCoerceTextMessage)));

        private static object OnCoerceTextMessage(DependencyObject o, object value)
        {
            ProjectWizardOptionsTemplate control = o as ProjectWizardOptionsTemplate;
            if (control != null)
                return control.OnCoerceTextMessage((string)value);
            else
                return value;
        }

        private static void OnTextMessageChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ProjectWizardOptionsTemplate control = o as ProjectWizardOptionsTemplate;
            if (control != null)
                control.OnTextMessageChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceTextMessage(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnTextMessageChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public string TextMessage
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(TextMessageProperty);
            }
            set
            {
                SetValue(TextMessageProperty, value);
            }
        }

        #endregion
        
        #endregion
        #region declaration
        Dictionary<string, Guid> mapTagPrototype = new Dictionary<string, Guid>();
        Dictionary<string, Guid> mapTagFolder = new Dictionary<string, Guid>();
        
        Dictionary<string, Guid> mapAlarmAreas = new Dictionary<string, Guid>();
        Dictionary<string, Dictionary<string, Guid>> mapSourceAlarmAreas = new Dictionary<string, Dictionary<string, Guid>>();
        Dictionary<string, Guid> mapAlarmNameToSourceAlarm = new Dictionary<string, Guid>();
        Dictionary<string, string> mapAlarmNameToTagName = new Dictionary<string, string>();
        Dictionary<string, string> mapAlarmNameToTagExression = new Dictionary<string, string>();
        Dictionary<Guid, string> mapAlarmDefinitionToTextValue = new Dictionary<Guid, string>();
        
        Dictionary<string, string> mapOldDllNameToNewOne = new Dictionary<string, string>() { { "S7TCP", "S7TCP" }, { "ModbusTCPIP", "ModbusTCP" }, { "Modbus", "Modbus" } };
        Dictionary<string, string> mapOldDllNameToNewOneReference = new Dictionary<string, string>() { { "S7TCP", "S7TCP" }, { "ModbusTCPIP", "ModbusTCP" }, { "Modbus", "ModBus" } };
        Dictionary<string, string> mapDriverNameToDll = new Dictionary<string, string>();
        Dictionary<string, string> mapDllToDriverName = new Dictionary<string, string>();
        Dictionary<string, string> mapTagToStaticTaskLynk = new Dictionary<string, string>();
        Dictionary<string, string> mapTagToDynamicLynk = new Dictionary<string, string>();
        Dictionary<string, string> mapTagToFolder = new Dictionary<string, string>();
        Dictionary<string, TagOptions> mapTagToDataType = new Dictionary<string, TagOptions>();
        Dictionary<string, bool> mapDriverTSBInvertion = new Dictionary<string, bool>() { { "S7TCP", false }, { "ModbusTCP", true }, { "Modbus", true } };
        Dictionary<string, XElement> mapVariableList = new Dictionary<string, XElement>();
        Dictionary<string, XElement> mapStructureList = new Dictionary<string, XElement>();
        Dictionary<string, XElement> mapDLRList = new Dictionary<string, XElement>();
        Dictionary<string, XElement> mapAlarmList = new Dictionary<string, XElement>();
        Dictionary<string, XElement> mapDriverInfo = new Dictionary<string, XElement>();
        private static readonly ILog log = LogManager.GetLogger(Properties.Resources.ImportLog);

        readonly List<StringOptions> datalist = new List<StringOptions>();


        List<string> notImportedList = new List<string>();
        List<string> importedList = new List<string>();

        bool bInterrupted;
        bool bDirty;
        CancellationTokenSource cts;
        #endregion

        public ProjectWizardOptionsTemplate()
        {
            InitializeComponent();
            checkboxImported.IsChecked = false;
            checkboxNotImported.IsChecked = true;
            checkboxScreens.Visibility = System.Windows.Visibility.Collapsed;
            gridControl.ItemsSource = datalist;
            InitLabels();
            import.IsChecked = true;
        }

        void InitLabels()
        {
            var cultInfo = new System.Globalization.CultureInfo(System.Globalization.CultureInfo.CurrentCulture.Name, false).TextInfo;
            supportedDrivers.Text = cultInfo.ToTitleCase(Properties.Resources.SupportedDrivers);
            import.Content = cultInfo.ToTitleCase(Properties.Resources.Import);
            enableLog.Content = cultInfo.ToTitleCase(Properties.Resources.EnableLog);
            stringHeader.Header = cultInfo.ToTitleCase(Properties.Resources.StringHeader);
            stringCulture.Header = cultInfo.ToTitleCase(Properties.Resources.StringCulture);
        }
        private void checkboxAll_Checked(object sender, RoutedEventArgs e)
        {
            checkboxAlarms.IsChecked = true;
            checkboxDrivers.IsChecked = true;
            checkboxTags.IsChecked = true;
            checkboxDLRs.IsChecked = true;
            checkboxScreens.IsChecked = true;
            checkboxRecipes.IsChecked = true;

            checkboxAlarms.IsEnabled = true;
            checkboxDrivers.IsEnabled = true;
            checkboxTags.IsEnabled = true;
            checkboxDLRs.IsEnabled = true;
            checkboxScreens.IsEnabled = true;
            checkboxRecipes.IsEnabled = true;
        }

        private void checkboxAll_Unchecked(object sender, RoutedEventArgs e)
        {
            checkboxAlarms.IsChecked = false;
            checkboxDrivers.IsChecked = false;
            checkboxTags.IsChecked = false;
            checkboxDLRs.IsChecked = false;
            checkboxScreens.IsChecked = false;
            checkboxRecipes.IsChecked = false;

            checkboxAlarms.IsEnabled = false;
            checkboxDrivers.IsEnabled = false;
            checkboxTags.IsEnabled = false;
            checkboxDLRs.IsEnabled = false;
            checkboxScreens.IsEnabled = false;
            checkboxRecipes.IsEnabled = false;
        }
        public bool Execute()
        {
            if (string.IsNullOrEmpty(ImportWizardPluginComponent.NewProject.SourceProject.LocalPath) || !File.Exists(ImportWizardPluginComponent.NewProject.SourceProject.LocalPath))
                return false;

            string root = System.IO.Path.GetDirectoryName(ImportWizardPluginComponent.NewProject.SourceProject.LocalPath);
            string projectname = System.IO.Path.GetFileNameWithoutExtension(ImportWizardPluginComponent.NewProject.SourceProject.LocalPath);

            bool _checkboxAlarms = (bool)checkboxAlarms.IsChecked;
            bool _checkboxDrivers = (bool)checkboxDrivers.IsChecked;
            bool _checkboxTags = (bool)checkboxTags.IsChecked;
            bool _checkboxDLRs = (bool)checkboxDLRs.IsChecked;
            bool _checkboxString = (bool)checkboxString.IsChecked;
            bool _checkboxRecipes = (bool)checkboxRecipes.IsChecked;

            var messageControl = ImportWizardPluginComponent.ImportWizardDialog.DialogContent as Controls.ControlMessage;
            messageControl.DataContext = this;
            //messageControl.textBox.Text = Properties.Resources.Disclaimer;
            InitLog();

            cts = new CancellationTokenSource();
            var task1 = Task.Factory.StartNew(() =>
            {

                if (cts != null)
                    cts.Token.ThrowIfCancellationRequested();

                bool result;
                //Dispatcher.BeginInvokeIfRequired(() =>
                //{
                try
                {
                    if (!bInterrupted)
                        InitGeneralMaps(projectname);


                    if (_checkboxAlarms && !bInterrupted)
                    {
#if DEBUG
                        var text = String.Format("Loading Alarms took") + " : {0}";
                        using (var stopwatcher = new StopWatcher(text))
#endif
                        {
                            //Dispatcher.BeginInvokeIfRequired(() =>
                            {
                                Dispatcher.BeginInvokeIfRequired(() =>
                                    {
                                        if(!bDisposed)
                                            TextMessage = TextMessage + Environment.NewLine + Properties.Resources.LoadingAlarms;
                                    });
                                LoadAlarms(System.IO.Path.Combine(root, string.Format("{0}{1}", projectname, Properties.Settings.Default.AlarmExtension)));
                            }//);
                        }
                    }
                    if (_checkboxDrivers && !bInterrupted)
                    {
#if DEBUG
                        var text = String.Format("Loading Drivers took") + " : {0}";
                        using (var stopwatcher = new StopWatcher(text))
#endif
                        {
                            // Dispatcher.BeginInvokeIfRequired(() =>
                            {
                                Dispatcher.BeginInvokeIfRequired(() =>
                                    {
                                        if (!bDisposed)
                                            TextMessage = TextMessage + Environment.NewLine + Properties.Resources.LoadingDrivers;
                                    });
                                LoadDrivers(System.IO.Path.Combine(root, "RESOURCES", projectname), Properties.Settings.Default.DriverSettingExtension);
                            }//);
                        }
                    }
                    if (_checkboxTags && !bInterrupted)
                    {
#if DEBUG
                        var text = String.Format("Loading Tags took") + " : {0}";
                        using (var stopwatcher = new StopWatcher(text))
#endif
                        {
                            //Dispatcher.BeginInvokeIfRequired(() =>
                            {
                                LoadTags(System.IO.Path.Combine(root, string.Format("{0}{1}", projectname, Properties.Settings.Default.TagExtension)), _checkboxDrivers);
                            }//);
                        }
                    }
                    if (_checkboxDLRs && !bInterrupted)
                    {
#if DEBUG
                        var text = String.Format("Loading DLR took") + " : {0}";
                        using (var stopwatcher = new StopWatcher(text))
#endif
                        {
                            //Dispatcher.BeginInvokeIfRequired(() =>
                            {
                                Dispatcher.BeginInvokeIfRequired(() =>
                                    {
                                        if (!bDisposed)
                                            TextMessage = TextMessage + Environment.NewLine + Properties.Resources.LoadingDLRs;
                                    });
                                LoadDLR(System.IO.Path.Combine(root, string.Format("{0}{1}", projectname, Properties.Settings.Default.DLRExtension)));
                            }//);
                        }
                    }
                    if (_checkboxString && !bInterrupted)
                    {
#if DEBUG
                        var text = String.Format("Loading StringTable took") + " : {0}";
                        using (var stopwatcher = new StopWatcher(text))
#endif
                        {
                            // Dispatcher.BeginInvokeIfRequired(() =>
                            {
                                Dispatcher.BeginInvokeIfRequired(() =>
                                    {
                                        if (!bDisposed)
                                            TextMessage = TextMessage + Environment.NewLine + Properties.Resources.LoadingStringTables;
                                    });
                                LoadStringTable();
                            }///);
                        }
                    }
                    if (_checkboxRecipes && !bInterrupted)
                    {
#if DEBUG
                        var text = String.Format("Loading Recipes took") + " : {0}";
                        using (var stopwatcher = new StopWatcher(text))
#endif
                        {
                            //Dispatcher.BeginInvokeIfRequired(() =>
                            {
                                Dispatcher.BeginInvokeIfRequired(() =>
                                    {
                                        if (!bDisposed)
                                            TextMessage = TextMessage + Environment.NewLine + Properties.Resources.LoadingRecipes;
                                    });
                                LoadRecipes(System.IO.Path.Combine(root, string.Format("{0}{1}", projectname, Properties.Settings.Default.DLRExtension)));
                            }//);
                        }
                    }

                    //if (!bInterrupted)
                    //Dispatcher.BeginInvokeIfRequired(() =>
                        //{
                        //    Dispatcher.BeginInvokeIfRequired(() =>
                        //        {
                        //            if (!bDisposed)
                        //                TextMessage = TextMessage + Environment.NewLine + Properties.Resources.WritingLogs;
                        //        });
                        //    WriteLog();
                        //}//);

#if DEBUG
                        string LogPath = System.IO.Path.GetDirectoryName(ImportWizardPluginComponent.NewProject.ProjectPath);
                        string LogFile = "ImportProjectLog.log";
                        if (System.IO.File.Exists(System.IO.Path.Combine(LogPath, LogFile)))
                            Process.Start("explorer.exe", string.Format("/open,{0}", System.IO.Path.Combine(LogPath, LogFile)));
#endif
                }
                catch (Exception exx)
                {
                    AppendLog(Properties.Resources.ImportSourceProjectError,bError:true);
                    bDirty = true;
                }
                    //result = bInterrupted;
               // });
                    //return bInterrupted;
            });
            var task2 = task1.ContinueWith(ret =>
            {
                if (ImportWizardPluginComponent.ImportWizardDialog != null && !bDirty)
                    ImportWizardPluginComponent.ImportWizardDialog.DialogResult = true;
                else
                    ImportWizardPluginComponent.ImportWizardDialog.DialogResult = false;

                cts.Dispose();
                cts = null;

                return false;
            }, TaskScheduler.FromCurrentSynchronizationContext());


            bool res = (bool)ImportWizardPluginComponent.ImportWizardDialog.ShowDialog();
            if (res == false)
            {
                bInterrupted = true;
                if (cts != null)
                    cts.Cancel();
                if(bDirty)
                    ImportWizardPluginComponent.ProjectView.UIInterface.ShowError(Properties.Resources.ImportSourceProjectError);
            }

            return false;

        }

        static IEnumerable<XElement> SimpleStreamAxis(
                       string inputUrl, string matchName)
        {
            using (XmlReader reader = XmlReader.Create(inputUrl))
            {
                reader.MoveToContent();
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name == matchName)
                            {
                                XElement el = XElement.ReadFrom(reader) as XElement;
                                if (el != null)
                                    yield return el;
                            }
                            break;
                    }
                }
                reader.Close();
            }
        }
        Dictionary<string, XElement> GetElements(string filepath, string descendantid)
        {
            int index = 0;
            Dictionary<string, XElement> mapTag = new Dictionary<string, XElement>();
            if (!File.Exists(filepath))
                return mapTag;
            var keyexpandolist = SimpleStreamAxis(filepath, descendantid);
            if (keyexpandolist.Count() != 0)
            {
                keyexpandolist.ToList().ForEach(e =>
                {
                    try
                    {
                        mapTag.Add(string.Format("item{0}", index++), e);
                    }
                    catch
                    {
                    }
                });
            }

            return mapTag;
        }
        void InitGeneralMaps(string projectname)
        {
            string root = System.IO.Path.GetDirectoryName(ImportWizardPluginComponent.NewProject.SourceProject.LocalPath);
            //string projectname = System.IO.Path.GetFileNameWithoutExtension(ImportWizardPluginComponent.NewProject.SourceProject.LocalPath);
            string realtimedbpath = System.IO.Path.Combine(root, string.Format("{0}{1}", projectname, Properties.Settings.Default.TagExtension));
            //XDocument rdbdoc = XDocument.Load(realtimedbpath);

            Dispatcher.BeginInvokeIfRequired(() =>
            {
                if (!bDisposed)
                    TextMessage = TextMessage + Environment.NewLine + Properties.Resources.InitialisingMaps;
            });

#if DEBUG
            //Debug.WriteLine("Starting at: {0}",DateTime.Now);
            //var text = String.Format("Loading Tagmap took") + " : {0}";
            //using (var stopwatcher = new StopWatcher(text))
#endif
            {
                mapVariableList = GetElements(realtimedbpath, "Variable");// GetElements(rdbdoc.ToString(), "VariableList", true, "Variable", false);
            }
#if DEBUG
            //text = String.Format("Loading Structuremap took") + " : {0}";
            //using (var stopwatcher = new StopWatcher(text))
#endif
            {
                mapStructureList = GetElements(realtimedbpath, "Structure");// GetElements(rdbdoc.ToString(), "StructureList", true, "Structure", false);
            }

#if DEBUG
            //text = String.Format("Loading Alarmmap took") + " : {0}";
            //using (var stopwatcher = new StopWatcher(text))
#endif
            {
                mapAlarmList = GetElements(System.IO.Path.Combine(root, string.Format("{0}{1}", projectname, Properties.Settings.Default.AlarmExtension)), "Alarm"); //GetElements(filepath, "AlarmList", false, "Alarm", false);
            }

#if DEBUG
            //text = String.Format("Loading DriverInfommap took") + " : {0}";
            //using (var stopwatcher = new StopWatcher(text))
#endif
            {
                mapDriverInfo = GetElements(String.Format("{0}Drivers\\Drivers.xml", GetAssemblyPath()), "Driver"); //GetElements(String.Format("{0}Drivers\\Drivers.xml", GetAssemblyPath()), "DriverList", false, "Driver", false);
            }

#if DEBUG
            //text = String.Format("Loading DriverInfommap took") + " : {0}";
            //using (var stopwatcher = new StopWatcher(text))
#endif
            {
                mapDLRList = GetElements(System.IO.Path.Combine(root, string.Format("{0}{1}", projectname, Properties.Settings.Default.DLRExtension)), "DLRecipe"); //GetElements(rdbdoc.ToString(), "DLRecipeList", true, "DLRecipe", false);
            }

#if DEBUG
            //text = String.Format("Loading Maps took") + " : {0}";
            //using (var stopwatcher = new StopWatcher(text))
#endif
            {
                (from XElement tag in mapVariableList.Values.AsParallel()
                 select tag).ToList().ForEach(x =>
                 {
                     if (bInterrupted)
                         return;

                     XElement tagname = x.Element("Name");
                     OldDataType oldtype = (OldDataType)int.Parse(tagname.Attribute("Type").Value);
                     UFUAModel.DataType? dataType = GetDataType(oldtype, tagname.Attribute("ElementType"));
                     int ArrayDimension = (int)GetArrayDimension(oldtype, tagname.Attribute("Bit"));
                     string structure = tagname.Attribute("StructType") != null ? tagname.Attribute("StructType").Value : string.Empty;

                     if (dataType != null)
                     {
                         ArrayDimension = ArrayDimension == 0 ? 1 : ArrayDimension;
                         TagOptions tagoption = GetTagOption((UFUAModel.DataType)dataType, ArrayDimension);
                         if (x.Element("Name").Attribute("Description") != null)
                             tagoption.Desc = x.Element("Name").Attribute("Description").Value;
                         if (x.Element("Name").Attribute("EU") != null)
                             tagoption.Unit = x.Element("Name").Attribute("EU").Value;
                         if (x.Element("Name").Attribute("InitialValue") != null)
                             tagoption.IValue = x.Element("Name").Attribute("InitialValue").Value;

                         mapTagToDataType.Add(tagname.Value, tagoption);
                     }
                     else if (oldtype == OldDataType.Structure && !string.IsNullOrEmpty(structure))
                     {
                         TagOptions structuretagoption = new TagOptions() { SByte = true, SWord = true };
                         XElement memberlist = (from el in mapStructureList.Values.AsParallel<XElement>()
                                                where (el as XElement).Element("Name").Value == structure && (el as XElement).Element("MemberList").HasElements
                                                select (el as XElement).Element("MemberList")).FirstOrDefault();

                         if (memberlist != null)
                         {
                             memberlist.Elements().ToList().ForEach(y =>
                             {
                                 UFUAModel.DataType? memberDataType = CheckDataType((OldDataType)int.Parse(y.Element("Name").Attribute("Type").Value));
                                 TagOptions _tagoption = GetTagOption((UFUAModel.DataType)memberDataType, 1);
                                 structuretagoption.Bit += _tagoption.Bit;
                                 structuretagoption.Byte += _tagoption.Byte;
                             });

                             if (x.Element("Name").Attribute("Description") != null)
                                 structuretagoption.Desc = x.Element("Name").Attribute("Description").Value;
                             if (x.Element("Name").Attribute("EU") != null)
                                 structuretagoption.Unit = x.Element("Name").Attribute("EU").Value;
                             if (x.Element("Name").Attribute("InitialValue") != null)
                                 structuretagoption.IValue = x.Element("Name").Attribute("InitialValue").Value;

                             mapTagToDataType.Add(tagname.Value, structuretagoption);
                         }
                     }

                 });
            }
        }
        
        void LoadRecipes(string filepath)
        {
            try
            {
                AppendLog(Properties.Resources.CapTitle2);
                AppendLog(Properties.Resources.CapTitle2, false);
                AppendLog(string.Format("{0}", Properties.Resources.ImportRecipes), false);
                AppendLog(string.Format("{0}", Properties.Resources.ImportRecipes));
                AppendLog(Properties.Resources.CapTitle2);
                AppendLog(Properties.Resources.CapTitle2, false);

                String xmlfile = null;
                //if (mapDLRList.Count == 0)
                //{
                //    //XDocument rdbdoc = XDocument.Load(filepath);
                //    mapDLRList = GetElements(filepath, "DLRecipe"); //GetElements(rdbdoc.ToString(), "DLRecipeList", true, "DLRecipe", false);
                //}

                List<XElement> _mapRcp = (from item in mapDLRList.Values where (item as XElement).Element("Name").Attribute("IsRecipe").Value.Equals("1") select (item as XElement)).ToList();
                string TextMessageOr = string.Empty;
                int _max = _mapRcp.Count;
                int w = 0;

                Dispatcher.BeginInvokeIfRequired(() =>
                {
                    if (!bDisposed)
                        TextMessageOr = TextMessage;
                });

                foreach (XElement item in _mapRcp)
                {
                    if (bInterrupted)
                        break;
                    xmlfile = String.Format("{0}\\{1}\\{2}\\{3}{4}",
                                    System.IO.Path.GetDirectoryName(ImportWizardPluginComponent.ProjectUri.LocalPath),
                                    System.IO.Path.GetFileNameWithoutExtension(ImportWizardPluginComponent.ProjectUri.LocalPath),
                                    (ImportWizardPluginComponent.ProjectView.recipeEditorManager as IDocumentManager).TypeLabel,
                                    item.Element("Name").Value,
                                    (ImportWizardPluginComponent.ProjectView.recipeEditorManager as IDocumentManager).FileType);

                    IDocument doc = UFProjectManager.UFProjectDocument.FromFile(ImportWizardPluginComponent.ProjectUri.AbsolutePath, ImportWizardPluginComponent.ProjectView.projectManagerService as UFProjectManagerComponent);
                    AddRecipe(item, doc, xmlfile);
                    Dispatcher.BeginInvokeIfRequired(() =>
                    {
                        if (!bDisposed)
                            TextMessage = string.Format("{0} {1} of {2}", TextMessageOr, ++w, _max);
                    });
                }

            }
            catch (Exception e)
            {
                AppendLog(e.ToString(), bError: true);
                return;
            }
        }

        void LoadAlarms(String filepath)
        {
            try
            {
                var connString = ImportWizardPluginComponent.ProjectView.GetServerIOConnectionStringFromUri(ImportWizardPluginComponent.ProjectUri);
                if (connString != null)
                {
                    using (var dl = XpoDefault.GetDataLayer(connString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema))
                    {
                        AppendLog(Properties.Resources.CapTitle2);
                        AppendLog(Properties.Resources.CapTitle2, false);
                        AppendLog(string.Format("{0}", Properties.Resources.ImportAlarms), false);
                        AppendLog(string.Format("{0}", Properties.Resources.ImportAlarms));
                        AppendLog(Properties.Resources.CapTitle2);
                        AppendLog(Properties.Resources.CapTitle2, false);

                        string TextMessageOr = string.Empty;
                        int _max = mapAlarmList.Count;
                        int w = 0;

                        Dispatcher.BeginInvokeIfRequired(() =>
                        {
                            if (!bDisposed)
                                TextMessageOr = TextMessage;
                        });

                        if (mapAlarmList.Count == 0)
                            return;

                        using (var uow = new UnitOfWork(dl))
                        {

                            foreach (var item in mapAlarmList.Values.AsParallel())
                            {
                                if (bInterrupted)
                                {
                                    uow.CommitChanges();
                                    break;
                                }

                                XElement alarm = item as XElement;
                                XElement alarmtagname = alarm.Element("Name");
                                XElement thresholdlist = alarm.Element("ThresholdList");

                                if (thresholdlist == null)
                                {
                                    AppendLog(string.Format(Properties.Resources.ThresholdEmptyError, alarm.Name));
                                    continue;
                                }

                                int z;
                                if (alarmtagname.Attribute("ThresholdExclusive").Value.Equals("1"))
                                {
                                    IEnumerable<XElement> thresholdsHigh =
                                    from el in thresholdlist.Elements("Threshold")
                                    where (OldCondition)int.Parse(el.Element("Execution").Attribute("Condition").Value) == OldCondition.GreaterThanOrEqual
                                    orderby double.Parse(el.Element("Execution").Attribute("Threshold").Value) ascending
                                    select el;

                                    IEnumerable<XElement> thresholdsLow =
                                    from el in thresholdlist.Elements("Threshold")
                                    where (OldCondition)int.Parse(el.Element("Execution").Attribute("Condition").Value) == OldCondition.LessThanOrEqual
                                    orderby double.Parse(el.Element("Execution").Attribute("Threshold").Value) descending
                                    select el;

                                    IEnumerable<XElement> thresholds =
                                    from el in thresholdlist.Elements("Threshold")
                                    where (OldCondition)int.Parse(el.Element("Execution").Attribute("Condition").Value) != OldCondition.LessThanOrEqual &&
                                    (OldCondition)int.Parse(el.Element("Execution").Attribute("Condition").Value) != OldCondition.GreaterThanOrEqual
                                    select el;

                                    Dictionary<int, ExclusiveAlarm> exclusivethresholds = new Dictionary<int, ExclusiveAlarm>();

                                    foreach (var t in thresholds)
                                    {
                                        AddAlarm(t, alarmtagname, uow);
                                    }

                                    z = 0;
                                    int i;
                                    for (i = 0; i < (int)(thresholdsHigh.Count() / 2); i++)
                                    {
                                        ExclusiveAlarm exclusivealarm = new ExclusiveAlarm();
                                        exclusivealarm.HighLimit = thresholdsHigh.ElementAt(i * 2);
                                        exclusivealarm.HighHighLimit = thresholdsHigh.ElementAt(i * 2 + 1);
                                        exclusivethresholds.Add(i, exclusivealarm);
                                        z = i * 2 + 1;
                                    }

                                    if (i > 0)
                                        z++;

                                    if ((int)(thresholdsHigh.Count() % 2) > 0 && z < thresholdsHigh.Count())
                                    {
                                        ExclusiveAlarm exclusivealarm = new ExclusiveAlarm();
                                        exclusivealarm.HighLimit = thresholdsHigh.ElementAt(z);
                                        exclusivethresholds.Add(i, exclusivealarm);
                                    }

                                    z = 0;
                                    i = 0;
                                    for (i = 0; i < (int)(thresholdsLow.Count() / 2); i++)
                                    {
                                        ExclusiveAlarm exclusivealarm = exclusivethresholds.ContainsKey(i) ? exclusivethresholds[i] : new ExclusiveAlarm();
                                        exclusivealarm.LowLimit = thresholdsLow.ElementAt(i * 2);
                                        exclusivealarm.LowLowLimit = thresholdsLow.ElementAt(i * 2 + 1);
                                        if (!exclusivethresholds.ContainsKey(i))
                                            exclusivethresholds.Add(i, exclusivealarm);
                                        else
                                            exclusivethresholds[i] = exclusivealarm;

                                        z = i * 2 + 1;
                                    }

                                    if (i > 0)
                                        z++;

                                    if ((int)(thresholdsLow.Count() % 2) > 0 && z < thresholdsLow.Count())
                                    {
                                        ExclusiveAlarm exclusivealarm = exclusivethresholds.ContainsKey(i) ? exclusivethresholds[i] : new ExclusiveAlarm();
                                        exclusivealarm.LowLimit = thresholdsLow.ElementAt(z);
                                        if (!exclusivethresholds.ContainsKey(i))
                                            exclusivethresholds.Add(i, exclusivealarm);
                                        else
                                            exclusivethresholds[i] = exclusivealarm;
                                    }

                                    AddExclusiveAlarm(exclusivethresholds, alarmtagname, uow);
                                }
                                else
                                {
                                    IEnumerable<XElement> thresholds =
                                     from el in thresholdlist.Elements("Threshold")
                                     select el;

                                    foreach (var t in thresholds)
                                    {
                                        AddAlarm(t, alarmtagname, uow);
                                    }
                                }
                                Dispatcher.BeginInvokeIfRequired(() =>
                                {
                                    if (!bDisposed)
                                        TextMessage = string.Format("{0} {1} of {2}", TextMessageOr, ++w, _max);
                                });
                            }

                            uow.CommitChanges();

                        }
                    }
                }
            }
            catch (Exception e)
            {
                AppendLog(e.ToString(), bError: true);
                return;
            }
        }

        void LoadTags(String filepath, bool _checkboxDrivers)
        {
            try
            {
                var connString = ImportWizardPluginComponent.ProjectView.GetServerIOConnectionStringFromUri(ImportWizardPluginComponent.ProjectUri);
                if (connString != null)
                {
                    using (var dl = XpoDefault.GetDataLayer(connString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema))
                    {
                        AppendLog(Properties.Resources.CapTitle2);
                        AppendLog(Properties.Resources.CapTitle2, false);
                        AppendLog(string.Format("{0}", Properties.Resources.ImportTags), false);
                        AppendLog(string.Format("{0}", Properties.Resources.ImportTags));
                        AppendLog(Properties.Resources.CapTitle2);
                        AppendLog(Properties.Resources.CapTitle2, false);

                        string TextMessageOr = string.Empty;
                        int _max = mapStructureList.Count;
                        int w = 0;

                        using (var uow = new UnitOfWork(dl))
                        {
                            Dispatcher.BeginInvokeIfRequired(() =>
                            {
                                if (!bDisposed)
                                {
                                    TextMessage = TextMessage + Environment.NewLine + Properties.Resources.LoadingStructures;
                                    TextMessageOr = TextMessage;
                                }
                            });
#if DEBUG
                            //var text = String.Format("Adding Structures took") + " : {0}";
                            //using (var stopwatcher = new StopWatcher(text))
#endif
                            {
                                foreach (XElement item in mapStructureList.Values.AsParallel<XElement>())
                                {
                                    if (bInterrupted)
                                    {
                                        //uow.CommitChanges();
                                        break;
                                    }
                                    AddStructure(item, uow);
                                    Dispatcher.BeginInvokeIfRequired(() =>
                                    {
                                        if (!bDisposed)
                                            TextMessage = string.Format("{0} {1} of {2}", TextMessageOr, ++w, _max);
                                    });
                                }
                                uow.CommitChanges();
                            }

                            Dispatcher.BeginInvokeIfRequired(() =>
                            {
                                if (!bDisposed)
                                {
                                    TextMessage = TextMessage + Environment.NewLine + Properties.Resources.LoadingTags;
                                    TextMessageOr = TextMessage;
                                    w = 0;
                                    _max = mapVariableList.Count;
                                }
                            });
#if DEBUG
                            //text = String.Format("Adding Tags took") + " : {0}";
                            //using (var stopwatcher = new StopWatcher(text))
#endif
                            {
                                foreach (XElement item in mapVariableList.Values.AsParallel<XElement>())
                                {
                                    if (bInterrupted)
                                    {
                                        //uow.CommitChanges();
                                        break;
                                    }
                                    AddTag(item, uow, _checkboxDrivers);
                                    Dispatcher.BeginInvokeIfRequired(() =>
                                    {
                                        if (!bDisposed)
                                            TextMessage = string.Format("{0} {1} of {2}", TextMessageOr, ++w, _max);
                                    });
                                }
                                uow.CommitChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                AppendLog(e.ToString(), bError: true);
                return;
            }
        }

        void LoadDrivers(String filepath, String extension)
        {
            try
            {
                var connString = ImportWizardPluginComponent.ProjectView.GetServerIOConnectionStringFromUri(ImportWizardPluginComponent.ProjectUri);
                if (connString != null)
                {
                    using (var dl = XpoDefault.GetDataLayer(connString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema))
                    {
                        AppendLog(Properties.Resources.CapTitle2);
                        AppendLog(Properties.Resources.CapTitle2, false);
                        AppendLog(string.Format("{0}", Properties.Resources.ImportDrivers), false);
                        AppendLog(string.Format("{0}", Properties.Resources.ImportDrivers));
                        AppendLog(Properties.Resources.CapTitle2);
                        AppendLog(Properties.Resources.CapTitle2, false);

                        string root = System.IO.Path.GetDirectoryName(ImportWizardPluginComponent.NewProject.SourceProject.LocalPath);
                        string projectname = System.IO.Path.GetFileNameWithoutExtension(ImportWizardPluginComponent.NewProject.SourceProject.LocalPath);
                        string realtimedbpath = System.IO.Path.Combine(root, string.Format("{0}{1}", projectname, Properties.Settings.Default.TagExtension));
                        //XDocument rdbdoc = XDocument.Load(realtimedbpath);
                        Dictionary<string, XElement> mapDrivers = GetElements(realtimedbpath, "Driver"); //GetElements(rdbdoc.ToString(), "DriverList", true, "Driver", false);

                        foreach (var item in mapDrivers)
                        {
                            mapDriverNameToDll.Add((item.Value as XElement).Element("Name").Value, System.IO.Path.GetFileNameWithoutExtension((item.Value as XElement).Element("Name").Attribute("FileName").Value));
                            mapDllToDriverName.Add(System.IO.Path.GetFileNameWithoutExtension((item.Value as XElement).Element("Name").Attribute("FileName").Value), (item.Value as XElement).Element("Name").Value);
                        }

                        string TextMessageOr = string.Empty;
                        int _max = mapDrivers.Count;
                        int w = 0;

                        Dispatcher.BeginInvokeIfRequired(() =>
                        {
                            if (!bDisposed)
                                TextMessageOr = TextMessage;
                        });

                        foreach (var driverpath in Directory.GetFiles(filepath, string.Format("*{0}", extension)))
                        {
                            XDocument doc = new XDocument();
                            string drivername = System.IO.Path.GetFileNameWithoutExtension(driverpath);

                            if (!mapDllToDriverName.ContainsKey(drivername))
                                continue;
                            if (!mapOldDllNameToNewOne.ContainsKey(drivername))
                            {
                                AppendLog(string.Format(Properties.Resources.PluginDriverError, drivername));
                                continue;
                            }

                            try
                            {
                                //doc = XDocument.Load(driverpath);
                                //string driverdoc = doc.ToString();

                                using (var uow = new UnitOfWork(dl))
                                {
                                    if (bInterrupted)
                                    {
                                        uow.CommitChanges();
                                        break;
                                    }

                                    AddDriver(uow, driverpath, drivername, mapVariableList);
                                    Dispatcher.BeginInvokeIfRequired(() =>
                                    {
                                        if (!bDisposed)
                                            TextMessage = string.Format("{0} {1} of {2}", TextMessageOr, ++w, _max);
                                    });
                                    uow.CommitChanges();
                                }

                            }
                            catch (Exception ex)
                            {
                                AppendLog(string.Format(Properties.Resources.DriverError, drivername, ex), bError: true);
                            }
                        }

                    }
                }
            }
            catch (Exception e)
            {
                AppendLog(e.ToString(), bError: true);
                return;
            }
        }

        void LoadDLR(String filepath)
        {
            try
            {
                var connString = ImportWizardPluginComponent.ProjectView.GetServerIOConnectionStringFromUri(ImportWizardPluginComponent.ProjectUri);
                if (connString != null)
                {
                    using (var dl = XpoDefault.GetDataLayer(connString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema))
                    {
                        AppendLog(Properties.Resources.CapTitle2);
                        AppendLog(Properties.Resources.CapTitle2, false);
                        AppendLog(string.Format("{0}", Properties.Resources.ImportDLRs), false);
                        AppendLog(string.Format("{0}", Properties.Resources.ImportDLRs));
                        AppendLog(Properties.Resources.CapTitle2);
                        AppendLog(Properties.Resources.CapTitle2, false);

                        //if(mapDLRList.Count == 0)
                        //{
                        //    //XDocument rdbdoc = XDocument.Load(filepath);
                        //    mapDLRList = GetElements(filepath, "DLRecipe"); //GetElements(rdbdoc.ToString(), "DLRecipeList", true, "DLRecipe", false);
                        //}

                        List<XElement> _mapDlr = (from item in mapDLRList.Values where (item as XElement).Element("Name").Attribute("IsRecipe").Value.Equals("0") select (item as XElement)).ToList();
                        string TextMessageOr = string.Empty;
                        int _max = _mapDlr.Count;
                        int w = 0;

                        Dispatcher.BeginInvokeIfRequired(() =>
                        {
                            if (!bDisposed)
                                TextMessageOr = TextMessage;
                        });

                        using (var uow = new UnitOfWork(dl))
                        {
                            foreach (XElement item in _mapDlr)
                            {
                                if (bInterrupted)
                                {
                                    uow.CommitChanges();
                                    break;
                                }

                                AddDlr(item, uow);
                                Dispatcher.BeginInvokeIfRequired(() =>
                                {
                                    if (!bDisposed)
                                        TextMessage = string.Format("{0} {1} of {2}", TextMessageOr, ++w, _max);
                                });
                                uow.CommitChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                AppendLog(e.ToString(), bError: true);
                return;
            }
        }

        void LoadStringTable()
        {
            try
            {
                AppendLog(Properties.Resources.CapTitle2);
                AppendLog(Properties.Resources.CapTitle2, false);
                AppendLog(string.Format("{0}", Properties.Resources.ImportString), false);
                AppendLog(string.Format("{0}", Properties.Resources.ImportString));
                AppendLog(Properties.Resources.CapTitle2);
                AppendLog(Properties.Resources.CapTitle2, false);

                string xmlfile = String.Format("{0}\\{1}\\{2}\\{3}{4}",
                                System.IO.Path.GetDirectoryName(ImportWizardPluginComponent.ProjectUri.LocalPath),
                                System.IO.Path.GetFileNameWithoutExtension(ImportWizardPluginComponent.ProjectUri.LocalPath),
                                (ImportWizardPluginComponent.ProjectView.stringEditorManager as IDocumentManager).TypeLabel,
                                (ImportWizardPluginComponent.ProjectView.stringEditorManager as IDocumentManager).TypeLabel,
                                (ImportWizardPluginComponent.ProjectView.stringEditorManager as IDocumentManager).FileType);

                InMemoryDataStore InMemory = null;
                string fileBase = xmlfile;
                if (fileBase.Length > 0)
                {
                    InMemory = GetDataStore(fileBase);
                }
                if (InMemory == null)
                {
                    AppendLog(string.Format(Properties.Resources.StringTableError, Properties.Resources.AllTables, Properties.Resources.StringDocError));
                    return;
                }
                string TextMessageOr = string.Empty;
                int _max = datalist.Count;
                int w = 0;

                Dispatcher.BeginInvokeIfRequired(() =>
                {
                    if (!bDisposed)
                        TextMessageOr = TextMessage;
                });


                using (var uow = new UnitOfWork(new SimpleDataLayer(InMemory)))
                {
                    datalist.ForEach(x =>
                    {
                        if (bInterrupted)
                        {
                            SaveInMemorySettings(uow, fileBase, InMemory);
                            return;
                        }
                        StringModel.UFStringLocale locales = (from tag in new XPQuery<StringModel.UFStringLocale>(uow).AsParallel() where tag.Name == x.Culture.Name select tag).FirstOrDefault();
                        if (locales == null)
                        {
                            locales = new StringModel.UFStringLocale(uow) { Name = x.Culture.Name, Locale = x.Culture.Name };
                            //uow.CommitChanges();
                            //SaveInMemorySettings(uow, fileBase, InMemory);
                        }
                        AddString(x.FileName, locales, uow);
                        Dispatcher.BeginInvokeIfRequired(() =>
                        {
                            if (!bDisposed)
                                TextMessage = string.Format("{0} {1} of {2}", TextMessageOr, ++w, _max);
                        });
                    });
                    SaveInMemorySettings(uow, fileBase, InMemory);
                }
            }
            catch (Exception e)
            {
                AppendLog(e.ToString(), bError: true);
            }
        }

        #region Recipes
        void AddRecipe(XElement item, IDocument parent, string path)
        {
            try
            {
                XElement dlrDSN = item.Element("DSN");
                if (dlrDSN.Attribute("UseInMemoryDB").Value.Equals("1"))
                {
                    AppendLog(string.Format(Properties.Resources.RecipeIMDBError, item.Element("Name").Value));
                    return;
                }


                string name = System.IO.Path.GetFileNameWithoutExtension(path);
                List<XElement> _mapColumn = (from c in item.Element("ColumnList").Elements() select (c as XElement)).ToList();

                UFRecipeSettings.UFRecipeModel.UFRecipeEntity recipeentity = ImportWizardPluginComponent.ProjectView.recipeEditorManager.AddRecipe(new Uri(path), parent) as UFRecipeSettings.UFRecipeModel.UFRecipeEntity;
                IDocument recipedoc = (ImportWizardPluginComponent.ProjectView.recipeEditorManager as IDocumentManager).GetDocument(new Uri(path));

                if (recipedoc == null)
                {
                    AppendLog(string.Format(Properties.Resources.RecipeError, item.Element("Name").Value, Properties.Resources.RecipeDocError));
                    return;
                }
                string _status = item.Element("Variables").Attribute("Status").Value;
                string _list = item.Element("Variables").Attribute("ListRecipe").Value;
                if (!string.IsNullOrEmpty(_status))
                {
                    if (!string.IsNullOrEmpty(_status) && mapTagToFolder.ContainsKey(_status))
                    {
                        try
                        {
                            var erString = ImportWizardPluginComponent.ProjectView.UFUAEditorManager.GetTagEntityReference(parent, mapTagToFolder[_status], null);
                            if (!String.IsNullOrEmpty(erString))
                                recipeentity.TagRecipeState = erString.FromXml<OPCUAViewModel.OPCUAEntityReference>();
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
                if (!string.IsNullOrEmpty(_list))
                {
                    if (!string.IsNullOrEmpty(_list) && mapTagToFolder.ContainsKey(_list))
                    {
                        try
                        {
                            var erString = ImportWizardPluginComponent.ProjectView.UFUAEditorManager.GetTagEntityReference(parent, mapTagToFolder[_list], null);
                            if (!String.IsNullOrEmpty(erString))
                                recipeentity.TagRecipeList = erString.FromXml<OPCUAViewModel.OPCUAEntityReference>();
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }


                UFRecipeSettings.UFRecipeModel.UFGroupEntity group = ImportWizardPluginComponent.ProjectView.recipeEditorManager.AddGroup("Columns", recipedoc) as UFRecipeSettings.UFRecipeModel.UFGroupEntity;
                _mapColumn.ForEach(x =>
                {
                    try
                    {
                        if (string.IsNullOrEmpty(x.Element("Name").Attribute("Variable").Value))
                        {
                            AppendLog(string.Format(Properties.Resources.ColumnRecipeEmpty, x.Element("Name").Value, name));
                        }
                        else if (x.Element("Name").Attribute("RecipeIndex").Value.Equals("0"))
                        {
                            if (!string.IsNullOrEmpty(x.Element("Name").Attribute("Variable").Value) && mapTagToDataType.ContainsKey(x.Element("Name").Attribute("Variable").Value))
                            {
                                //if(mapTagToDynamicLynk.ContainsKey(x.Element("Name").Attribute("Variable").Value))
                                //{
                                //    group.GroupStartAddress.DynamicSettingsForEditing = mapTagToDynamicLynk[x.Element("Name").Attribute("Variable").Value];
                                //}

                                UFRecipeSettings.UFRecipeModel.UFDataValueEntity dvalue = new UFRecipeSettings.UFRecipeModel.UFDataValueEntity(true)
                                {
                                    NodeId = new Guid(),
                                    DataValueName = x.Element("Name").Value, 
                                    DataType = mapTagToDataType[x.Element("Name").Attribute("Variable").Value].DType,
                                    Description = mapTagToDataType[x.Element("Name").Attribute("Variable").Value].Desc,  
                                    DefaultValue = mapTagToDataType[x.Element("Name").Attribute("Variable").Value].IValue  
                                };

                                if (mapTagToDynamicLynk.ContainsKey(x.Element("Name").Attribute("Variable").Value))
                                {
                                    dvalue.StartingAddress = mapTagToDynamicLynk[x.Element("Name").Attribute("Variable").Value];
                                }
                                string tempVar = x.Element("Name").Attribute("TempVar").Value;
                                if (!string.IsNullOrEmpty(tempVar) && mapTagToFolder.ContainsKey(tempVar))
                                {
                                    try
                                    {
                                        var erString = ImportWizardPluginComponent.ProjectView.UFUAEditorManager.GetTagEntityReference(parent, mapTagToFolder[tempVar], null);
                                        if (!String.IsNullOrEmpty(erString))
                                            dvalue.TagDataValue = erString.FromXml<OPCUAViewModel.OPCUAEntityReference>();
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                }
                                
                                group.DataValues.Add(dvalue);
                            }

                        }
                        else if (x.Element("Name").Attribute("RecipeIndex").Value.Equals("1"))
                        {
                            string tempVar = x.Element("Name").Attribute("TempVar").Value;
                            if (!string.IsNullOrEmpty(tempVar) && mapTagToDataType.ContainsKey(tempVar) && mapTagToFolder.ContainsKey(tempVar))
                            {
                                try
                                {
                                    var erString = ImportWizardPluginComponent.ProjectView.UFUAEditorManager.GetTagEntityReference(parent, mapTagToFolder[tempVar], null);
                                    if (!String.IsNullOrEmpty(erString))
                                        recipeentity.TagRecipeIndex = erString.FromXml<OPCUAViewModel.OPCUAEntityReference>();
                                }
                                catch (Exception ex)
                                {
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        AppendLog(string.Format(Properties.Resources.ColumnRecipeError, x.Element("Name").Value, name, ex.ToString()), bError: true);
                        return;
                    }
                });

                recipeentity.Groups.Add(group);

                if (mapVariableList.Count != 0)
                {
                    List<XElement> rcptaglist = (from tag in mapVariableList.Values
                                                 where (tag as XElement).Element("DataLoggerList") != null
                                                 && (from XAttribute a in (tag as XElement).Element("DataLoggerList").Attributes() where a.Value.Equals(name) select a).FirstOrDefault() != null
                                                 select (tag as XElement)).ToList();

                    rcptaglist.ForEach(x =>
                    {
                        try
                        {
                            UFRecipeSettings.UFRecipeModel.UFGroupEntity rcpgrp = (from col in recipeentity.Groups where col.GroupName.Equals(x.Element("Name").Value) select col).FirstOrDefault();
                            if (rcpgrp == null)
                                rcpgrp = ImportWizardPluginComponent.ProjectView.recipeEditorManager.AddGroup(x.Element("Name").Value, recipedoc) as UFRecipeSettings.UFRecipeModel.UFGroupEntity;

                            //if (mapTagToDynamicLynk.ContainsKey(x.Element("Name").Value))
                            //{
                            //    rcpgrp.GroupStartAddress.DynamicSettingsForEditing = mapTagToDynamicLynk[x.Element("Name").Value];
                            //}

                            UFRecipeSettings.UFRecipeModel.UFDataValueEntity dvalue = (from col in rcpgrp.DataValues where col.DataValueName.Equals(x.Element("Name").Value) select col).FirstOrDefault();
                            if (dvalue == null)
                                dvalue = new UFRecipeSettings.UFRecipeModel.UFDataValueEntity(true) { DataValueName = x.Element("Name").Value };

                            if (mapTagToDataType.ContainsKey(x.Element("Name").Value))
                            {
                                dvalue.DataType = mapTagToDataType[x.Element("Name").Value].DType;
                                dvalue.Description = mapTagToDataType[x.Element("Name").Value].Desc;
                                dvalue.DefaultValue = mapTagToDataType[x.Element("Name").Value].IValue;
                            }

                            if (mapTagToDynamicLynk.ContainsKey(x.Element("Name").Value))
                            {
                                dvalue.StartingAddress = mapTagToDynamicLynk[x.Element("Name").Value];
                            }

                            rcpgrp.DataValues.Add(dvalue);

                            recipeentity.Groups.Add(rcpgrp);

                        }
                        catch (Exception ex)
                        {
                            AppendLog(string.Format(Properties.Resources.ColumnRecipeError, x.Element("Name").Value, name, ex.ToString()), bError: true);
                        }
                    });

                }
                ImportWizardPluginComponent.ProjectView.recipeEditorManager.SaveToFile(recipedoc);
                AppendLog(string.Format(Properties.Resources.RecipeImported, name, !string.IsNullOrEmpty(dlrDSN.Value) ? Properties.Resources.DNSWarning : string.Empty), false, bError: true);
            }
            catch (Exception e)
            {
                AppendLog(string.Format(Properties.Resources.RecipeError, item.Element("Name").Value, e.ToString()), bError: true);
                return;
            }
        }

        #endregion

        #region Strings
        void AddString(string filename, StringModel.UFStringLocale locale, UnitOfWork uow)
        {
            try
            {
                //XDocument rdbdoc = XDocument.Load(filename);
                Dictionary<string, XElement> mapString = GetElements(filename, "item"); //GetElements(rdbdoc.ToString(), "list", true, "item", false);

                foreach (XElement item in mapString.Values)
                {
                    StringModel.UFStringLocaleText loctext = new StringModel.UFStringLocaleText(uow) { Text = item.Attribute("key").Value, Locale = item.Attribute("value").Value };
                    locale.UFStringLocaleTexts.Add(loctext);
                    loctext.Culture = locale.Name;
                    //uow.CommitChanges();
                }

                AppendLog(string.Format(Properties.Resources.StringTableImported, System.IO.Path.GetFileName(filename)), false);
            }
            catch (Exception e)
            {
                AppendLog(string.Format(Properties.Resources.StringTableError, System.IO.Path.GetFileName(filename), e.ToString()), bError: true);
            }
        }
        #endregion

        #region Datalogger
        void AddDlr(XElement item, UnitOfWork uow)
        {
            XElement dlrName = item.Element("Name");
            XElement dlrDSN = item.Element("DSN");
            XElement dlrValriables = item.Element("Variables");
            XElement dlrColumns = item.Element("ColumnList");
            List<XElement> columnlist = new List<XElement>();

            if (dlrColumns != null)
                columnlist = (from el in dlrColumns.Elements("Column") select el.Element("Name")).ToList();
             
            if (dlrName.Attribute("IsRecipe").Value.Equals("1"))
            {
                return;
            }

            if (dlrDSN.Attribute("UseInMemoryDB").Value.Equals("1"))
            {
                AppendLog(string.Format(Properties.Resources.DRLIMDBError, dlrName.Value));
                return;
            }

            //if (dlrName.Attribute("RecordOnChange").Value.Equals("1"))
            //{
            //    AppendLog(string.Format(Properties.Resources.DLROnChangeError, dlrName.Value));
            //    return;
            //}

            int hour, min, sec,msec,dhour,dmin, ddays; 
            
            int.TryParse(dlrName.Attribute("TimeRecHour").Value, out hour);
            int.TryParse(dlrName.Attribute("TimeRecMin").Value, out min);
            int.TryParse(dlrName.Attribute("TimeRecSec").Value, out sec);
            int.TryParse(dlrName.Attribute("TimeRecMsec").Value, out msec);

            int.TryParse(dlrDSN.Attribute("DurationHours").Value, out dhour);
            int.TryParse(dlrDSN.Attribute("DurationMinutes").Value, out dmin);
            int.TryParse(dlrDSN.Attribute("DurationDays").Value, out ddays);

            DataLoggerModel.DataLoggerSettings dlr = new DataLoggerModel.DataLoggerSettings(uow) 
            {
                Name = dlrName.Value,
                Enable = dlrName.Attribute("Enabled").Value.Equals("1"),
                TableName = dlrDSN.Attribute("TableName").Value,
                UtcTimeColumnName = dlrDSN.Attribute("TimeCol").Value,
                LocalTimeColumnName = dlrDSN.Attribute("LocalTimeCol").Value,
                MillisecondsColumnName = dlrDSN.Attribute("MSecCol").Value,
                UserColumnName = dlrDSN.Attribute("UserColName").Value,
                ReasonColumnName = dlrDSN.Attribute("ReasonColName").Value,
                RecordingTimeInterval = new TimeSpan(0, hour,min,sec,msec),
                RecordOnDataChange = dlrName.Attribute("RecordOnChange").Value.Equals("1"),
                MaxAge = new TimeSpan(ddays, dhour, dmin, 0),
                MaxLength = int.Parse(dlrDSN.Attribute("VarCharsMax").Value),
                MaxErrorBeforeFlush = byte.Parse(dlrDSN.Attribute("MaxError").Value),
                MaxTransactionsBeforeCommit = uint.Parse(dlrDSN.Attribute("MaxNumberTrans").Value),
                WaitBeforeRetry = 10,
                MaxCacheSize = uint.Parse(dlrDSN.Attribute("MaxCacheBeforeFlush").Value)
            };

            if (!string.IsNullOrEmpty(dlrValriables.Attribute("EnableTime").Value))
                dlr.EnableRecordingTag = new UFUAModel.TagEntityReference((from tag in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel() where tag.Name.Equals(dlrValriables.Attribute("EnableTime").Value) select tag).FirstOrDefault());
            if (!string.IsNullOrEmpty(dlrValriables.Attribute("Record").Value))
                dlr.RecordingTag = new UFUAModel.TagEntityReference((from tag in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel() where tag.Name.Equals(dlrValriables.Attribute("Record").Value) select tag).FirstOrDefault());
            if (!string.IsNullOrEmpty(dlrValriables.Attribute("Reset").Value))
                dlr.ResettingTag = new UFUAModel.TagEntityReference((from tag in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel() where tag.Name.Equals(dlrValriables.Attribute("Reset").Value) select tag).FirstOrDefault());
            
            //uow.CommitChanges();

            columnlist.ForEach(e =>
                {
                    try
                    {
                        DataLoggerModel.DataLoggerColumn dlrcol = new DataLoggerModel.DataLoggerColumn(uow)
                        {
                            ColumnName = e.Value
                        };
                        
                        if (!string.IsNullOrEmpty(e.Attribute("Variable").Value))
                            dlrcol.ColumnTag = new UFUAModel.TagEntityReference((from tag in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel() where tag.Name.Equals(e.Attribute("Variable").Value) select tag).FirstOrDefault());

                        dlr.Columns.Add(dlrcol);
                        //uow.CommitChanges();
                    }
                    catch (Exception ex)
                    {
                        AppendLog(string.Format(Properties.Resources.DLRColumnError, e.Value, dlrName.Value, ex.ToString()), bError: true);
                    }
                });


            if (mapVariableList.Count != 0)
            {
                List<XElement> dlrtaglist = (from tag in mapVariableList.Values 
                                         where (tag as XElement).Element("DataLoggerList") != null
                                         && (from XAttribute a in (tag as XElement).Element("DataLoggerList").Attributes() where a.Value.Equals(dlrName.Value) select a).FirstOrDefault() != null
                                         select (tag as XElement)).ToList();

                dlrtaglist.ForEach(x => 
                    {
                        try
                        {
                            DataLoggerModel.DataLoggerColumn dlrcol;

                            dlrcol = (from col in dlr.Columns where col.Name.Equals(x.Element("Name").Value) select col).FirstOrDefault();
                            if (dlrcol == null)
                                dlrcol = new DataLoggerModel.DataLoggerColumn(uow)
                                {
                                    ColumnName = x.Element("Name").Value
                                };

                            dlrcol.ColumnTag = new UFUAModel.TagEntityReference((from tag in new XPQuery<UFUAModel.UFUATag>(uow, true).AsParallel() where tag.Name.Equals(x.Element("Name").Value) select tag).FirstOrDefault());

                            dlr.Columns.Add(dlrcol);
                            //uow.CommitChanges();
                        }
                        catch (Exception ex)
                        {
                            AppendLog(string.Format(Properties.Resources.DLRColumnError, x.Element("Name").Value, dlrName.Value, ex.ToString()), bError: true);
                        }
                    });

            }

            AppendLog(string.Format(Properties.Resources.DLRImported, dlrName.Value, !string.IsNullOrEmpty(dlrDSN.Value) ? Properties.Resources.DNSWarning : string.Empty), false);

        }
        #endregion

        #region Drivers
        #region S7TCP
        void AddS7TCPSettings(string connString, string driverdoc, string drivername)
        {
            try
            {
                Dictionary<string, XElement> mapStation = GetElements(driverdoc, "Station"); //GetElements(driverdoc, "StationList", true, "Station", false);
                Dictionary<string, XElement> mapJob = GetElements(driverdoc, "Job"); //GetElements(driverdoc, "JobList", true, "Job", false);
                Dictionary<string, XElement> mapGeneralSettings = GetElements(driverdoc, "GeneralSettings"); //GetElements(driverdoc, "DriverSettings", true, "GeneralSettings", false);
                Dictionary<string, XElement> mapDriverSettings = GetElements(driverdoc, string.Format("{0}DriverSettings", drivername)); //GetElements(driverdoc, "DriverSettings", true, string.Format("{0}DriverSettings", drivername), false);
                
                if(mapGeneralSettings.Count == 0)
                {
                    AppendLog(string.Format(Properties.Resources.DriverSettingsError, drivername, Properties.Resources.XMLError));
                    return;
                }

                var fileBase = DriverCodeBase.CommunicationDriver.GetFileBase(connString, mapOldDllNameToNewOne[drivername]);
                InMemoryDataStore InMemory = null;
                if (!String.IsNullOrEmpty(fileBase))
                {
                    InMemory = DriverCodeBase.CommunicationDriver.GetDataStore(fileBase);
                }
                if (InMemory == null)
                {
                    AppendLog(string.Format(Properties.Resources.DriverSettingsError, drivername, Properties.Resources.FileNotFoundError));
                    return;
                }

                (from XElement tag in mapJob.Values
                 select tag).ToList().ForEach(x =>
                    {
                        if (!x.Element("VariableList").HasAttributes)
                        {
                            AppendLog(string.Format(Properties.Resources.DriverListEmptyError, x.Element("Name").Value, drivername));
                        }
                        else
                        {
                            string sa = x.Element("DeviceTaskSettings").Attribute("DeviceAddress").Value;
                            x.Element("VariableList").Attributes().ToList().ForEach(v =>
                            {
                                StringBuilder _dynstring = new StringBuilder(string.Format("{0}.Sta={1}", mapDllToDriverName[drivername], x.Element("Name").Attribute("Station").Value));
                                _dynstring.Append(x.Element("Name").Attribute("OutputAtStartup") != null ? string.Format("|StartOut={0}", x.Element("Name").Attribute("OutputAtStartup").Value.Equals("false") ? "0" : "1") : string.Empty);
                                _dynstring.Append(string.Format("|TaskType={0}", x.Element("Name").Attribute("Type").Value));
                                _dynstring.Append(string.Format("|SB={0}", GetSB(x.Element("Name").Attribute("SwapByte").Value, v.Value)));
                                _dynstring.Append(string.Format("|SW={0}", GetSW(x.Element("Name").Attribute("SwapWord").Value, v.Value)));
                                _dynstring.Append(string.Format("|Addr={0}", sa));
                                sa = GetS7TCPSA(sa, v.Value);

                                if (v.Value.IndexOf(':') != -1)
                                    AppendLog(string.Format(Properties.Resources.NotSupportedStaticTaskOnSingleStructureMember, x.Element("Name").Value, drivername, v.Value));

                                if (!mapTagToStaticTaskLynk.ContainsKey(v.Value))
                                    mapTagToStaticTaskLynk.Add(v.Value, _dynstring.ToString());
                                else
                                    mapTagToStaticTaskLynk[v.Value] = _dynstring.ToString();
                            });
                        }
                    });

                using (var duow =  new UnitOfWork(new SimpleDataLayer(InMemory)))
                {
                    try
                    {
                        byte deviceid = byte.Parse((mapDriverSettings.FirstOrDefault().Value as XElement).Attribute("RemoteDeviceID").Value);
                        byte rack = byte.Parse((mapDriverSettings.FirstOrDefault().Value as XElement).Attribute("RemoteRack").Value);
                        byte slot = byte.Parse((mapDriverSettings.FirstOrDefault().Value as XElement).Attribute("RemoteSlot").Value);
                        int waitTime = int.Parse((mapGeneralSettings.FirstOrDefault().Value as XElement).Attribute("WaitTime").Value);
                        int timeout = int.Parse((mapGeneralSettings.FirstOrDefault().Value as XElement).Attribute("TimeOut").Value);
                        int pollingnotinuse = int.Parse((mapGeneralSettings.FirstOrDefault().Value as XElement).Attribute("PollingTimeNotInUse").Value);
                        int pollinginerror = int.Parse((mapGeneralSettings.FirstOrDefault().Value as XElement).Attribute("DefRefreshTimeNotInUse").Value);
                        string channel = string.Empty;

                        S7TCPDriverSettings configuration = new S7TCPDriverSettings(duow);
                        configuration.DefaultSettings();
                        configuration.AggregationThreshold = uint.Parse((mapGeneralSettings.FirstOrDefault().Value as XElement).Attribute("MinimumJobThreshold").Value);
                        configuration.AggregationLimit = uint.Parse((mapGeneralSettings.FirstOrDefault().Value as XElement).Attribute("AggregationLimit").Value);
                        duow.CommitChanges();

                        Dictionary<string, string> mapChannels = new Dictionary<string, string>();

                        foreach (XElement item in mapStation.Values)
                        {
                            try
                            {
                                string serveraddress = item.Element("Server").Attribute("ServerAddress").Value;
                                if (!mapChannels.ContainsKey(serveraddress))
                                {
                                    var ch = new S7TCPChannelSettings(duow);
                                    ch.DriverSettings = configuration;
                                    ch.DefaultSettings();
                                    ch.TcpChannelSettingsHostName = serveraddress;
                                    ch.TcpChannelSettingsHostPort = int.Parse(item.Element("Server").Attribute("ServerPort").Value);
                                    ch.Rack = rack;
                                    ch.DeviceID = deviceid;
                                    ch.Slot = slot;
                                    ch.WaitTime = waitTime;
                                    ch.Timeout = timeout;
                                    ch.PollingTimeNotInUse = pollingnotinuse;
                                    ch.PollingTimeInError = pollinginerror;
                                    duow.CommitChanges();
                                    mapChannels.Add(serveraddress, ch.Name);
                                    channel = ch.Name;
                                }
                                else
                                    channel = mapChannels[serveraddress];

                                duow.CommitChanges();

                                var st = new S7TCPStationSettings(duow);
                                st.DriverSettings = configuration;
                                st.DefaultSettings();
                                st.Name = item.Element("Name").Value;
                                st.DeviceID = byte.Parse(item.Element("DeviceStationSettings").Attribute("RemoteDeviceID").Value);
                                st.Rack = byte.Parse(item.Element("DeviceStationSettings").Attribute("RemoteRack").Value);
                                st.Slot = byte.Parse(item.Element("DeviceStationSettings").Attribute("RemoteSlot").Value);
                                st.Channel = channel;
                                st.MaxRetriesBeforeError = uint.Parse(item.Element("Name").Attribute("MaxRetries").Value);
                                //duow.CommitChanges();
                            }
                            catch (Exception ex)
                            {
                                AppendLog(string.Format(Properties.Resources.StationSettingsError, item.Element("Name").Value, ex.ToString()), bError: true);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        AppendLog(string.Format(Properties.Resources.DriverSettingsError, drivername, e.ToString()), bError: true);
                    }
                    SaveInMemorySettings(duow, fileBase, InMemory);
                }

                if (!File.Exists(fileBase))
                {
                    AppendLog(string.Format(Properties.Resources.DriverSettingsError, drivername, Properties.Resources.FileNotFoundError));
                    return;
                }

                var fileContents = System.IO.File.ReadAllText(fileBase);
                fileContents = fileContents.Replace(typeof(S7TCPDriverSettings).Namespace, mapOldDllNameToNewOneReference[drivername]);
                fileContents = fileContents.Replace(this.GetType().Namespace, mapOldDllNameToNewOneReference[drivername]);
                System.IO.File.WriteAllText(fileBase, fileContents);

                AppendLog(string.Format(Properties.Resources.DriverImported, drivername), false);
            }
            catch (Exception ex)
            {
                AppendLog(string.Format(Properties.Resources.DriverSettingsError, drivername, ex.ToString()), bError: true);
            }
        }

        #endregion
        #region Modbus
        void AddModbusSettings(string connString, string driverdoc, string drivername, Dictionary<string, XElement> mapTag)
        {
            try
            {
                //Dictionary<string, object> mapStation = GetElements(driverdoc, "StationList", true, "Station", false);
                //Dictionary<string, object> mapJob = GetElements(driverdoc, "JobList", true, "Job", false);
                //Dictionary<string, object> mapGeneralSettings = GetElements(driverdoc, "DriverSettings", true, "GeneralSettings", false);
                //Dictionary<string, object> mapDriverSettings = GetElements(driverdoc, "DriverSettings", true, string.Format("{0}DriverSettings", drivername), false);
                Dictionary<string, XElement> mapStation = GetElements(driverdoc, "Station"); //GetElements(driverdoc, "StationList", true, "Station", false);
                Dictionary<string, XElement> mapJob = GetElements(driverdoc, "Job"); //GetElements(driverdoc, "JobList", true, "Job", false);
                Dictionary<string, XElement> mapGeneralSettings = GetElements(driverdoc, "GeneralSettings"); //GetElements(driverdoc, "DriverSettings", true, "GeneralSettings", false);
                Dictionary<string, XElement> mapDriverSettings = GetElements(driverdoc, string.Format("{0}DriverSettings", drivername)); //GetElements(driverdoc, "DriverSettings", true, string.Format("{0}DriverSettings", drivername), false);

                if (mapGeneralSettings.Count == 0)
                {
                    AppendLog(string.Format(Properties.Resources.DriverSettingsError, drivername, Properties.Resources.XMLError));
                    return;
                }

                var fileBase = DriverCodeBase.CommunicationDriver.GetFileBase(connString, mapOldDllNameToNewOne[drivername]);
                InMemoryDataStore InMemory = null;
                if (!String.IsNullOrEmpty(fileBase))
                {
                    InMemory = DriverCodeBase.CommunicationDriver.GetDataStore(fileBase);
                }
                if (InMemory == null)
                {
                    AppendLog(string.Format(Properties.Resources.DriverSettingsError, drivername, Properties.Resources.FileNotFoundError));
                    return;
                }

                Dictionary<string, string> mapChannels = new Dictionary<string, string>();
                Dictionary<string, string> mapStations = new Dictionary<string, string>();

                (from XElement tag in mapJob.Values
                 select tag).ToList().ForEach(x =>
                 {
                     if (!x.Element("VariableList").HasAttributes)
                     {
                         AppendLog(string.Format(Properties.Resources.DriverListEmptyError, x.Element("Name").Value, drivername));
                     }
                     else
                     {
                         int sa = int.Parse(x.Element("ModbusMaster").Attribute("StartAddress").Value);
                         x.Element("VariableList").Attributes().ToList().ForEach(v =>
                         {
                             StringBuilder _dynstring = new StringBuilder(string.Format("{0}.Sta={1}", mapDllToDriverName[drivername], x.Element("Name").Attribute("Station").Value));
                             _dynstring.Append(x.Element("Name").Attribute("OutputAtStartup") != null ? string.Format("|StartOut={0}", x.Element("Name").Attribute("OutputAtStartup").Value.Equals("false") ? "0" : "1") : string.Empty);
                             _dynstring.Append(string.Format("|TaskType={0}", x.Element("Name").Attribute("Type").Value));
                             _dynstring.Append(string.Format("|SB={0}", GetSB(x.Element("Name").Attribute("SwapByte").Value, v.Value)));
                             _dynstring.Append(string.Format("|SW={0}", GetSW(x.Element("Name").Attribute("SwapWord").Value, v.Value)));
                             _dynstring.Append(x.Element("ModbusMaster").Attribute("FunctionCode") != null ? string.Format("|FC={0}", x.Element("ModbusMaster").Attribute("FunctionCode").Value) : string.Empty);
                             _dynstring.Append(string.Format("|SA={0}", sa));
                             sa = GetModbusSA(sa, v.Value, x.Element("ModbusMaster").Attribute("FunctionCode").Value);

                             if (v.Value.IndexOf(':') != -1)
                                 AppendLog(string.Format(Properties.Resources.NotSupportedStaticTaskOnSingleStructureMember, x.Element("Name").Value, drivername, v.Value));

                             if (!mapTagToStaticTaskLynk.ContainsKey(v.Value))
                                 mapTagToStaticTaskLynk.Add(v.Value, _dynstring.ToString());
                             else
                                 mapTagToStaticTaskLynk[v.Value] = _dynstring.ToString();
                         });
                     }
                 });


                using (var duow = new UnitOfWork(new SimpleDataLayer(InMemory)))
                {
                    try
                    {
                        int waitTime = int.Parse((mapGeneralSettings.FirstOrDefault().Value as XElement).Attribute("WaitTime").Value);
                        int timeout = int.Parse((mapGeneralSettings.FirstOrDefault().Value as XElement).Attribute("TimeOut").Value);
                        int pollingnotinuse = int.Parse((mapGeneralSettings.FirstOrDefault().Value as XElement).Attribute("PollingTimeNotInUse").Value);
                        int pollinginerror = int.Parse((mapGeneralSettings.FirstOrDefault().Value as XElement).Attribute("DefRefreshTimeNotInUse").Value);
                        string channel = string.Empty;

                        ModbusDriverSettings configuration = new ModbusDriverSettings(duow);
                        configuration.DefaultSettings();
                        configuration.AggregationThreshold = uint.Parse((mapGeneralSettings.FirstOrDefault().Value as XElement).Attribute("MinimumJobThreshold").Value);
                        configuration.AggregationLimit = uint.Parse((mapGeneralSettings.FirstOrDefault().Value as XElement).Attribute("AggregationLimit").Value);
                        duow.CommitChanges();

                        foreach (XElement item in mapStation.Values)
                        {
                            try
                            {
                                string serveraddress = item.Element("PortSettings").Attribute("PortId").Value;
                                if (!mapChannels.ContainsKey(serveraddress))
                                {
                                    ModbusChannelSettings ch = new ModbusChannelSettings(duow);
                                    ch.DriverSettings = configuration;
                                    ch.DefaultSettings();
                                    ch.CommPortName = string.Format("Com{0}", int.Parse(item.Element("PortSettings").Attribute("PortId").Value) + 1);
                                    ch.CommPortBaudRate = int.Parse(item.Element("PortSettings").Attribute("BaudRate").Value);
                                    ch.CommPortDataBits = int.Parse(item.Element("PortSettings").Attribute("ByteSize").Value);
                                    ch.CommPortParity = int.Parse(item.Element("PortSettings").Attribute("Parity").Value);
                                    ch.CommPortStopBits = int.Parse(item.Element("PortSettings").Attribute("StopBits").Value);

                                    switch ((OldFlowControl)int.Parse(item.Element("PortSettings").Attribute("FlowControl").Value))
	                                {
		                                case OldFlowControl.None:
                                            ch.CommPortHandshake =(int)System.IO.Ports.Handshake.None;
                                            ch.CommPortRtsEnable = true;
                                            ch.CommPortDtrEnable = true;
                                            break;
                                        case OldFlowControl.HW:
                                            ch.CommPortHandshake =(int)System.IO.Ports.Handshake.RequestToSend;
                                            break;
                                        case OldFlowControl.XonXoff:
                                            ch.CommPortHandshake =(int)System.IO.Ports.Handshake.XOnXOff;
                                            break;
                                        case OldFlowControl.NoneSignDisabled:
                                            ch.CommPortHandshake =(int)System.IO.Ports.Handshake.None;
                                            ch.CommPortRtsEnable = false;
                                            ch.CommPortDtrEnable = false;
                                            break;
                                        case OldFlowControl.RTSToggle:
                                            ch.CommPortHandshake =(int)System.IO.Ports.Handshake.RequestToSend;
                                            break;
                                        default:
                                            break;
	                                }

                                    ch.CommPortReadTimeout = int.Parse(item.Element("PortTimeouts").Attribute("RxTimeout").Value);
                                    ch.CommPortWriteTimeout = int.Parse(item.Element("PortTimeouts").Attribute("TxTimeout").Value);
                                    ch.KeepOpened = bool.Parse(item.Element("KeepPortOpened").Value);
                                    ch.FrameType = 0;
                                    ch.WaitTime = waitTime;
                                    ch.Timeout = timeout;
                                    ch.TurnaroundDelay = uint.Parse(item.Element("ModbusStationSettings").Attribute("TurnaroundDelay").Value);
                                    ch.PollingTimeNotInUse = pollingnotinuse;
                                    ch.PollingTimeInError = pollinginerror;
                                    duow.CommitChanges();
                                    mapChannels.Add(serveraddress, ch.Name);
                                    channel = ch.Name;
                                }
                                else
                                    channel = mapChannels[serveraddress];

                                //duow.CommitChanges();

                                var st = new ModbusStationSettings(duow);
                                st.DriverSettings = configuration;
                                st.DefaultSettings();
                                st.Name = item.Element("Name").Value;
                                st.StationID = uint.Parse(item.Element("ModbusStationSettings").Attribute("ID").Value);
                                st.Channel = channel;
                                st.MaxRetriesBeforeError = uint.Parse(item.Element("Name").Attribute("MaxRetries").Value);
                                duow.CommitChanges();

                            }
                            catch (Exception ex)
                            {
                                AppendLog(string.Format(Properties.Resources.StationSettingsError, item.Element("Name").Value, ex.ToString()), bError: true);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        AppendLog(string.Format(Properties.Resources.DriverSettingsError, drivername, e.ToString()), bError: true);
                    }
                    SaveInMemorySettings(duow, fileBase, InMemory);

                }

                if (!File.Exists(fileBase))
                {
                    AppendLog(string.Format(Properties.Resources.DriverSettingsError, drivername, Properties.Resources.FileNotFoundError));
                    return;
                }

                var fileContents = System.IO.File.ReadAllText(fileBase);
                fileContents = fileContents.Replace(typeof(ModbusDriverSettings).Namespace, mapOldDllNameToNewOneReference[drivername]);
                fileContents = fileContents.Replace(this.GetType().Namespace, mapOldDllNameToNewOneReference[drivername]);
                System.IO.File.WriteAllText(fileBase, fileContents);

                AppendLog(string.Format(Properties.Resources.DriverImported, drivername), false);
            }
            catch (Exception ex)
            {
                AppendLog(string.Format(Properties.Resources.DriverSettingsError, drivername, ex.ToString()), bError: true);
            }
        }

        int GetModbusSA(int sa, string tagname, string functioncode)
        {
            OldFunctionCode fc = (OldFunctionCode)int.Parse(functioncode);
            if (mapTagToDataType.ContainsKey(tagname))
            {
                if (fc == OldFunctionCode.Coils ||
                    fc == OldFunctionCode.SingleCols ||
                    fc == OldFunctionCode.InputDiscretes)
                {
                    sa = sa + mapTagToDataType[tagname].Bit;
                }
                else
                {
                    sa = sa + mapTagToDataType[tagname].Byte;
                }
                return sa;
            }
            return sa;
        }
        #endregion
        #region ModbusTCP
        void AddModbusTCPSettings(string connString, string driverdoc, string drivername, Dictionary<string, XElement> mapTag)
        {
            try
            {
                //Dictionary<string, object> mapStation = GetElements(driverdoc, "StationList", true, "Station", false);
                //Dictionary<string, object> mapJob = GetElements(driverdoc, "JobList", true, "Job", false);
                //Dictionary<string, object> mapGeneralSettings = GetElements(driverdoc, "DriverSettings", true, "GeneralSettings", false);
                //Dictionary<string, object> mapDriverSettings = GetElements(driverdoc, "DriverSettings", true, string.Format("{0}DriverSettings", drivername), false);
                Dictionary<string, XElement> mapStation = GetElements(driverdoc, "Station"); //GetElements(driverdoc, "StationList", true, "Station", false);
                Dictionary<string, XElement> mapJob = GetElements(driverdoc, "Job"); //GetElements(driverdoc, "JobList", true, "Job", false);
                Dictionary<string, XElement> mapGeneralSettings = GetElements(driverdoc, "GeneralSettings"); //GetElements(driverdoc, "DriverSettings", true, "GeneralSettings", false);
                Dictionary<string, XElement> mapDriverSettings = GetElements(driverdoc, string.Format("{0}DriverSettings", drivername)); //GetElements(driverdoc, "DriverSettings", true, string.Format("{0}DriverSettings", drivername), false);

                if (mapGeneralSettings.Count == 0)
                {
                    AppendLog(string.Format(Properties.Resources.DriverSettingsError, drivername, Properties.Resources.XMLError));
                    return;
                }

                var fileBase = DriverCodeBase.CommunicationDriver.GetFileBase(connString, mapOldDllNameToNewOne[drivername]);
                InMemoryDataStore InMemory = null;
                if (!String.IsNullOrEmpty(fileBase))
                {
                    InMemory = DriverCodeBase.CommunicationDriver.GetDataStore(fileBase);
                }
                if (InMemory == null)
                {
                    AppendLog(string.Format(Properties.Resources.DriverSettingsError, drivername, Properties.Resources.FileNotFoundError));
                    return;
                }

                Dictionary<string, string> mapChannels = new Dictionary<string, string>();
                Dictionary<string, string> mapStations = new Dictionary<string, string>();

                (from XElement tag in mapJob.Values
                 select tag).ToList().ForEach(x =>
                    {
                        if (!x.Element("VariableList").HasAttributes)
                        {
                            AppendLog(string.Format(Properties.Resources.DriverListEmptyError, x.Element("Name").Value, drivername));
                        }
                        else
                        {
                            int sa = int.Parse(x.Element("ModbusTCPIP").Attribute("StartAddress").Value);
                            x.Element("VariableList").Attributes().ToList().ForEach(v =>
                            {
                                StringBuilder _dynstring = new StringBuilder(string.Format("{0}.Sta={1}_{2}", mapDllToDriverName[drivername], x.Element("Name").Attribute("Station").Value, x.Element(drivername).Attribute("UnitID").Value));
                                _dynstring.Append(x.Element("Name").Attribute("OutputAtStartup") != null ? string.Format("|StartOut={0}", x.Element("Name").Attribute("OutputAtStartup").Value.Equals("false") ? "0" : "1") : string.Empty);
                                _dynstring.Append(string.Format("|TaskType={0}", x.Element("Name").Attribute("Type").Value));
                                _dynstring.Append(string.Format("|SB={0}", GetSB(x.Element("Name").Attribute("SwapByte").Value, v.Value)));
                                _dynstring.Append(string.Format("|SW={0}", GetSW(x.Element("Name").Attribute("SwapWord").Value, v.Value)));
                                _dynstring.Append(x.Element("ModbusTCPIP").Attribute("FunctionCode") != null ? string.Format("|FC={0}", x.Element("ModbusTCPIP").Attribute("FunctionCode").Value) : string.Empty);
                                _dynstring.Append(string.Format("|SA={0}", sa));
                                sa = GetModbusTCPSA(sa, v.Value, x.Element("ModbusTCPIP").Attribute("FunctionCode").Value);

                                if (v.Value.IndexOf(':') != -1)
                                    AppendLog(string.Format(Properties.Resources.NotSupportedStaticTaskOnSingleStructureMember, x.Element("Name").Value, drivername,v.Value));

                                if (!mapTagToStaticTaskLynk.ContainsKey(v.Value))
                                    mapTagToStaticTaskLynk.Add(v.Value, _dynstring.ToString());
                                else
                                    mapTagToStaticTaskLynk[v.Value] = _dynstring.ToString();

                                string key = string.Format("{0}_{1}", x.Element("Name").Attribute("Station").Value, x.Element(drivername).Attribute("UnitID").Value);
                                if (!mapStations.ContainsKey(key))
                                    mapStations.Add(key, x.Element("Name").Attribute("Station").Value);
                                if (mapTagToDataType.ContainsKey(v.Value))
                                    mapTagToDataType[v.Value].Station = key;
                            });
                        }
                    });


                using (var duow = new UnitOfWork(new SimpleDataLayer(InMemory)))
                {
                    try
                    {
                        int waitTime = int.Parse((mapGeneralSettings.FirstOrDefault().Value as XElement).Attribute("WaitTime").Value);
                        int timeout = int.Parse((mapGeneralSettings.FirstOrDefault().Value as XElement).Attribute("TimeOut").Value);
                        int pollingnotinuse = int.Parse((mapGeneralSettings.FirstOrDefault().Value as XElement).Attribute("PollingTimeNotInUse").Value);
                        int pollinginerror = int.Parse((mapGeneralSettings.FirstOrDefault().Value as XElement).Attribute("DefRefreshTimeNotInUse").Value);
                        string channel = string.Empty;

                        ModbusTCPDriverSettings configuration = new ModbusTCPDriverSettings(duow);
                        configuration.DefaultSettings();
                        configuration.AggregationThreshold = uint.Parse((mapGeneralSettings.FirstOrDefault().Value as XElement).Attribute("MinimumJobThreshold").Value);
                        configuration.AggregationLimit = uint.Parse((mapGeneralSettings.FirstOrDefault().Value as XElement).Attribute("AggregationLimit").Value);
                        duow.CommitChanges();

                        (from XElement tag in mapTag.Values.AsParallel<XElement>() 
                             where 
                             tag.Element("Name").Attribute("DynamicSettings") != null && !string.IsNullOrEmpty(tag.Element("Name").Attribute("DynamicSettings").Value) 
                             select tag).ToList().ForEach(x =>
                            {
                                string _dynstring = x.Element("Name").Attribute("DynamicSettings").Value.Replace("[DRV]", "");
                                string _drivername = _dynstring.Remove(_dynstring.IndexOf('.'));
                                string _unitid = GetValue("Unit=", _dynstring);
                                string _stationname = GetValue("Sta=", _dynstring);

                                if (drivername.Equals(mapDriverNameToDll[_drivername]))
                                {
                                    string key = string.Format("{0}_{1}", _stationname, _unitid);
                                    if (!mapStations.ContainsKey(key))
                                        mapStations.Add(key,_stationname);
                                    if (mapTagToDataType.ContainsKey(x.Element("Name").Value))
                                        mapTagToDataType[x.Element("Name").Value].Station = key;
                                }

                            });

                        foreach (XElement item in mapStation.Values)
                        {
                            try
                            {
                                string serveraddress = item.Element("Server").Attribute("ServerAddress").Value;
                                if (!mapChannels.ContainsKey(serveraddress))
                                {
                                    var ch = new ModbusTCPChannelSettings(duow);
                                    ch.DriverSettings = configuration;
                                    ch.DefaultSettings();
                                    ch.TcpChannelSettingsHostName = serveraddress;
                                    ch.TcpChannelSettingsHostPort = int.Parse(item.Element("Server").Attribute("ServerPort").Value);
                                    ch.WaitTime = waitTime;
                                    ch.Timeout = timeout;
                                    ch.PollingTimeNotInUse = pollingnotinuse;
                                    ch.PollingTimeInError = pollinginerror;
                                    duow.CommitChanges();
                                    mapChannels.Add(serveraddress, ch.Name);
                                    channel = ch.Name;
                                }
                                else
                                    channel = mapChannels[serveraddress];

                                duow.CommitChanges();

                                List<string> _keys = (from string staname in mapStations.Keys where mapStations[staname] == item.Element("Name").Value select staname).ToList();
                                if (_keys != null && _keys.Count > 0)
                                    _keys.ForEach(x =>
                                    {
                                        var st = new ModbusTCPStationSettings(duow);
                                        st.DriverSettings = configuration;
                                        st.DefaultSettings();
                                        st.Name = x;
                                        st.StationID = uint.Parse(x.Replace(string.Format("{0}_", mapStations[x]), ""));
                                        st.Channel = channel;
                                        st.MaxRetriesBeforeError = uint.Parse(item.Element("Name").Attribute("MaxRetries").Value);
                                        //duow.CommitChanges();
                                    });
                                else
                                    {
                                        var st = new ModbusTCPStationSettings(duow);
                                        st.DriverSettings = configuration;
                                        st.DefaultSettings();
                                        st.Name = item.Element("Name").Value;
                                        st.StationID = 1;
                                        st.Channel = channel;
                                        st.MaxRetriesBeforeError = uint.Parse(item.Element("Name").Attribute("MaxRetries").Value);
                                        //duow.CommitChanges();
                                    }

                            }
                            catch (Exception ex)
                            {
                                AppendLog(string.Format(Properties.Resources.StationSettingsError, item.Element("Name").Value, ex.ToString()), bError: true);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        AppendLog(string.Format(Properties.Resources.DriverSettingsError, drivername, e.ToString()), bError: true);
                    }
                    SaveInMemorySettings(duow, fileBase, InMemory);

                }

                if (!File.Exists(fileBase))
                {
                    AppendLog(string.Format(Properties.Resources.DriverSettingsError, drivername, Properties.Resources.FileNotFoundError));
                    return;
                }

                var fileContents = System.IO.File.ReadAllText(fileBase);
                fileContents = fileContents.Replace(typeof(ModbusTCPDriverSettings).Namespace, mapOldDllNameToNewOneReference[drivername]);
                fileContents = fileContents.Replace(this.GetType().Namespace, mapOldDllNameToNewOneReference[drivername]);
                System.IO.File.WriteAllText(fileBase, fileContents);

                AppendLog(string.Format(Properties.Resources.DriverImported, drivername), false);
            }
            catch (Exception ex)
            {
                AppendLog(string.Format(Properties.Resources.DriverSettingsError, drivername, ex.ToString()), bError: true);
            }
        }

        string GetSB(string swap, string tagname)
        {
            if (mapTagToDataType.ContainsKey(tagname))
                return (mapTagToDataType[tagname].SByte &&
                    swap.Equals("true")) ? "true" : "false";

            return "false";
        }

        string GetSW(string swap, string tagname)
        {
            if (mapTagToDataType.ContainsKey(tagname))
                return (mapTagToDataType[tagname].SWord &&
                    swap.Equals("true")) ? "true" : "false";

            return "false";
        }

        int GetModbusTCPSA(int sa, string tagname, string functioncode)
        {
            OldFunctionCode fc = (OldFunctionCode)int.Parse(functioncode);
            if (mapTagToDataType.ContainsKey(tagname))
            {
                if (fc == OldFunctionCode.Coils || 
                    fc == OldFunctionCode.SingleCols || 
                    fc == OldFunctionCode.InputDiscretes)
                {
                    sa = sa + mapTagToDataType[tagname].Bit;
                }
                else
                {
                    sa = sa + mapTagToDataType[tagname].Byte;
                }
                return sa;
            }
            return sa;
        }
        
        string GetS7TCPSA(string sa, string tagname)
        {
            if(mapTagToDataType.ContainsKey(tagname))
            {
                StringBuilder _sa = new StringBuilder(string.Empty);
                List<string> _areas = new List<string>(){"E","I","A","Q","PE","PA","P","M","F","D"};
                List<string> _datatypes = new List<string>(){"B","W","D","X"};
                string datatype = string.Empty;
                string byteoffset = string.Empty;
                string dataformat = string.Empty;
                string bitoffset = string.Empty;

                if(sa.IndexOf("T") != 0 || sa.IndexOf("Z") != 0 || sa.IndexOf("C") != 0)
                {
                    _sa.Append(sa.Remove(1));
                    uint timercount = uint.Parse(sa.Substring(1));
                    _sa.Append(timercount + 1);
                }
                else if(sa.IndexOf(".DB") != -1)
                {
                    List<string> la = sa.Split('.').ToList();
                    string blocknumber = la[0].Replace("DB","");

                    if (sa.Split(',').Count() > 1)
                        dataformat = string.Format(",{0}", sa.Split(',')[1]);

                    byteoffset = la[1].Substring(2);
                    _datatypes.ForEach(x =>
                    {
                        if (byteoffset.IndexOf(x) == 0)
                            datatype = x;
                    });
                    
                    if (datatype.Length != 0)
                        byteoffset = byteoffset.Substring(1);

                    byteoffset = (int.Parse(byteoffset) + mapTagToDataType[tagname].Byte).ToString();

                    if (la.Count >= 3)
                    {
                        if(int.Parse(la[2].Split(',')[0]) + mapTagToDataType[tagname].Bit > 7)
                        {
                            bitoffset = ".0";
                            byteoffset = (int.Parse(byteoffset) + mapTagToDataType[tagname].Byte + 1).ToString();
                        }
                        else
                            bitoffset = string.Format(".{0}",int.Parse(la[2].Split(',')[0]) + mapTagToDataType[tagname].Bit);
                    }

                    _sa.Append(string.Format("DB{0}.DB{1}{2}{3}{4}", blocknumber, datatype, byteoffset, bitoffset, dataformat));
                }
                else
                {
                    List<string> laa = sa.Split('.').ToList();
                    string area = string.Empty;

                    if (sa.Split(',').Count() > 1)
                        dataformat = string.Format(",{0}", sa.Split(',')[1]);

                    _areas.ForEach(x =>
                    {
                        if (laa[0].IndexOf(x) == 0)
                            area = x;
                    });
                    
                    byteoffset = laa[0].Substring(1);
                    _datatypes.ForEach(x =>
                    {
                        if (byteoffset.IndexOf(x) == 0)
                            datatype = x;
                    });
                    
                    if (datatype.Length != 0)
                        byteoffset = byteoffset.Substring(1);

                    byteoffset = (int.Parse(byteoffset) + mapTagToDataType[tagname].Byte).ToString();

                    if (laa.Count >= 2)
                    {
                        if (int.Parse(laa[1].Split(',')[0]) + mapTagToDataType[tagname].Bit > 7)
                        {
                            bitoffset = ".0";
                            byteoffset = (int.Parse(byteoffset) + mapTagToDataType[tagname].Byte + 1).ToString();
                        }
                        else
                            bitoffset = string.Format(".{0}", int.Parse(laa[1].Split(',')[0]) + mapTagToDataType[tagname].Bit);
                    }

                    _sa.Append(string.Format("{0}{1}{2}{3}{4}", area, datatype, byteoffset, bitoffset, dataformat));
                }
                
                if(_sa.Length > 0)
                    sa = _sa.ToString();
            }
                
            return sa;
        }
        #endregion


        void AddDriver(UnitOfWork uow, string driverdoc, string drivername, Dictionary<string, XElement> mapTag)
        {
            UFInterfaces.Editors.DriverXmlInfo driverInfo = GetDriverInfo(string.Format("{0}.dll", mapOldDllNameToNewOne[drivername]));
            if (driverInfo == null)
            {
                AppendLog(string.Format(Properties.Resources.PluginDriverError, drivername));
            }
            try
            {
                try
                {
                    UFUAModel.UFUAConfiguration configuration;
                    var list = (from tag in new XPQuery<UFUAModel.UFUAConfiguration>(uow, true).AsParallel() select tag).ToList();
                    if (list.Count == 0)
                        configuration = new UFUAModel.UFUAConfiguration(uow);
                    else
                        configuration = list[0];

                    UFUAModel.UFUACommunicationDriver driver = new UFUAModel.UFUACommunicationDriver(uow)
                    {
                        Factory = driverInfo.Factory,
                        FriendlyName = driverInfo.FriendlyName,
                        Name = driverInfo.AssemblyName.Substring(0, driverInfo.AssemblyName.Length - 4),
                        AssemblyName = driverInfo.AssemblyName
                    };
                    configuration.ComunicationDrivers.Add(driver);
                    //uow.CommitChanges();

                    AppendLog(string.Format(Properties.Resources.DriverImported, drivername), false);

                    var connString = ImportWizardPluginComponent.ProjectView.GetServerIOConnectionStringFromUri(ImportWizardPluginComponent.ProjectUri);
                    if (connString != null)
                    {
                        switch (mapOldDllNameToNewOne[drivername])
                        {
                            case "S7TCP":
                                AddS7TCPSettings(connString, driverdoc, drivername);
                                break;
                            case "ModbusTCP":
                                AddModbusTCPSettings(connString, driverdoc, drivername, mapTag);
                                break;
                            case "Modbus":
                                AddModbusSettings(connString, driverdoc, drivername, mapTag);
                                break;
                            default:
                                AppendLog(string.Format(Properties.Resources.PluginDriverError, drivername));
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    AppendLog(string.Format(Properties.Resources.DriverError, drivername, ex.ToString()), bError: true);
                }
            }
            catch (Exception e)
            {
                AppendLog(string.Format(Properties.Resources.DriverError, drivername, e.ToString()), bError: true);
            }
        }

        TagOptions GetTagOption(UFUAModel.DataType dataType, int ArrayDimension)
        {
            TagOptions tagoption = new TagOptions() { DType = dataType};
            switch (dataType)
            {
                case UFUAModel.DataType.Boolean:
                    tagoption.SByte = false;
                    tagoption.SWord = false;
                    tagoption.Bit = 1 * ArrayDimension;
                    tagoption.Byte = 0 * ArrayDimension;
                    break;
                case UFUAModel.DataType.Byte:
                    tagoption.SByte = false;
                    tagoption.SWord = false;
                    tagoption.Bit = 8 * ArrayDimension;
                    tagoption.Byte = 1 * ArrayDimension;
                    break;
                case UFUAModel.DataType.SByte:
                    tagoption.SByte = false;
                    tagoption.SWord = false;
                    tagoption.Bit = 8 * ArrayDimension;
                    tagoption.Byte = 1 * ArrayDimension;
                    break;
                case UFUAModel.DataType.Int16:
                    tagoption.SByte = true;
                    tagoption.SWord = false;
                    tagoption.Bit = 16 * ArrayDimension;
                    tagoption.Byte = 2 * ArrayDimension;
                    break;
                case UFUAModel.DataType.UInt16:
                    tagoption.SByte = true;
                    tagoption.SWord = false;
                    tagoption.Bit = 16 * ArrayDimension;
                    tagoption.Byte = 2 * ArrayDimension;
                    break;
                case UFUAModel.DataType.Int32:
                    tagoption.SByte = true;
                    tagoption.SWord = true;
                    tagoption.Bit = 32 * ArrayDimension;
                    tagoption.Byte = 4 * ArrayDimension;
                    break;
                case UFUAModel.DataType.UInt32:
                    tagoption.SByte = true;
                    tagoption.SWord = true;
                    tagoption.Bit = 32 * ArrayDimension;
                    tagoption.Byte = 4 * ArrayDimension;
                    break;
                case UFUAModel.DataType.Int64:
                    tagoption.SByte = true;
                    tagoption.SWord = true;
                    tagoption.Bit = 64 * ArrayDimension;
                    tagoption.Byte = 8 * ArrayDimension;
                    break;
                case UFUAModel.DataType.UInt64:
                    tagoption.SByte = true;
                    tagoption.SWord = true;
                    tagoption.Bit = 64 * ArrayDimension;
                    tagoption.Byte = 8 * ArrayDimension;
                    break;
                case UFUAModel.DataType.Float:
                    tagoption.SByte = true;
                    tagoption.SWord = true;
                    tagoption.Bit = 64 * ArrayDimension;
                    tagoption.Byte = 8 * ArrayDimension;
                    break;
                case UFUAModel.DataType.Double:
                    tagoption.SByte = true;
                    tagoption.SWord = true;
                    tagoption.Bit = 64 * ArrayDimension;
                    tagoption.Byte = 8 * ArrayDimension;
                    break;
                case UFUAModel.DataType.String:
                    tagoption.SByte = false;
                    tagoption.SWord = false;
                    tagoption.Bit = 0;
                    tagoption.Byte = 0;
                    break;
                default:
                    break;
            }
            return tagoption;
        }

        void SaveInMemorySettings(UnitOfWork ufw, string fileBase, InMemoryDataStore InMemory)
        {
            ufw.CommitChanges();
            if (!String.IsNullOrEmpty(fileBase))
            {
                InMemory.WriteXml(fileBase);
            }
        }

        DriverXmlInfo GetDriverInfo(string p)
        {
            //Dictionary<string, object> mapDriverInfo = GetElements(String.Format("{0}Drivers\\Drivers.xml", GetAssemblyPath()), "Driver"); //GetElements(String.Format("{0}Drivers\\Drivers.xml", GetAssemblyPath()), "DriverList", false, "Driver", false);
            XElement driverinfo = (XElement)(from el in mapDriverInfo.Values
                                             where (el as XElement).Attribute("AssemblyName").Value == p
                                             select el).FirstOrDefault();
            if (driverinfo == null)
                return null;

            return new DriverXmlInfo()
            {
                Factory = driverinfo.Attribute("Factory").Value,
                FriendlyName = driverinfo.Attribute("FriendlyName").Value,
                Help = driverinfo.Attribute("Help").Value,
                AssemblyName = driverinfo.Attribute("AssemblyName").Value,
                PackageType = driverinfo.Attribute("PackageType").Value
            };
        }
        #endregion

        #region Structure & Tag
        void AddStructure(XElement structure, UnitOfWork uow)
        {
            XElement structurename = structure.Element("Name");
            XElement structurelist = structure.Element("MemberList");

            if (structurelist == null)
            {
                AppendLog(string.Format(Properties.Resources.MemberStructureEmptyError, structurename.Value));
                return;
            }

            try
            {
                UFUAModel.UFUATagPrototype tp;
                if (!mapTagPrototype.ContainsKey(structurename.Value))
                {
                    tp = new UFUAModel.UFUATagPrototype(uow) { Name = structurename.Value, NodeId = Guid.NewGuid() };
                    mapTagPrototype.Add(structurename.Value, tp.NodeId);
                    uow.CommitChanges();
                }
                else
                {
                    tp = (from entry in new XPQuery<UFUAModel.UFUATagPrototype>(uow).AsParallel()
                          where entry.NodeId == mapTagPrototype[structurename.Value] && entry.UFUATagOwner == null
                          select entry).FirstOrDefault();
                }
                string membername = string.Empty;
                try
                {
                    List<XElement> structurememberlist = (from el in structurelist.Elements("Member")
                                                          select el.Element("Name")).ToList();
                    int i = 0;
                    structurememberlist.ForEach(x =>
                    {
                        UFUAModel.UFUATag at = new UFUAModel.UFUATag(uow)
                        {
                            Name = x.Value,
                            NodeId = Guid.NewGuid(),
                            DataType = CheckDataType((OldDataType)int.Parse(x.Attribute("Type").Value)),
                            MemberOrderId = i
                        };
                        membername = x.Value;
                        tp.Members.Add(at);
                        i++;
                    }
                        );

                    //uow.CommitChanges();
                    AppendLog(string.Format(Properties.Resources.StructureImported, structurename.Value), false);
                }
                catch (Exception ex)
                {
                    AppendLog(string.Format(Properties.Resources.StructMemberError, membername, ex.ToString()), bError: true);
                }
            }
            catch (Exception e)
            {
                AppendLog(string.Format(Properties.Resources.StructureError, structurename.Value, e.ToString()), bError: true);
            }
        }

        UFUAModel.DataType? CheckDataType(OldDataType dtype)
        {
            switch (dtype)
            {
                case OldDataType.Bit:
                    return UFUAModel.DataType.Boolean;
                case OldDataType.SByte:
                    return UFUAModel.DataType.SByte;
                case OldDataType.Byte:
                    return UFUAModel.DataType.Byte;
                case OldDataType.SWord:
                    return UFUAModel.DataType.Int16;
                case OldDataType.Word:
                    return UFUAModel.DataType.UInt16;
                case OldDataType.SDWord:
                    return UFUAModel.DataType.Int32;
                case OldDataType.DWord:
                    return UFUAModel.DataType.UInt32;
                case OldDataType.Float:
                    return UFUAModel.DataType.Float;
                case OldDataType.Double:
                    return UFUAModel.DataType.Double;
                case OldDataType.String:
                    return UFUAModel.DataType.String;
                default:
                    return UFUAModel.DataType.Float;
            }
        }

        void AddTag(XElement tag, UnitOfWork uow, bool _checkboxDrivers)
        {
            XElement tagname = tag.Element("Name");
            XAttribute tagfolder = tagname.Attribute("Group");
            string folder = (string)tagfolder ?? string.Empty;
            bool checkboxDrivers = _checkboxDrivers;

            try
            {
                UFUAModel.UFUATag at;
#if DEBUG
                //var text = String.Format("Adding Tag took") + " : {0}";
                //using (var stopwatcher = new StopWatcher(text))
#endif
                {
                    OldDataType oldtype = (OldDataType)int.Parse(tagname.Attribute("Type").Value);
                    at = new UFUAModel.UFUATag(uow)
                    {
                        Name = tagname.Value,
                        NodeId = Guid.NewGuid(),
                        ModelType = GetModelType(oldtype),
                        DataType = GetDataType(oldtype, tagname.Attribute("ElementType")),
                        PrototypeModel = GetDataPrototype(oldtype, tagname.Attribute("StructType"), uow),
                        ArrayDimension = GetArrayDimension(oldtype, tagname.Attribute("Bit")),
                        DynamicSettings = _checkboxDrivers ? AddDynSettings(tagname, tagfolder) : string.Empty
                    };
                    at.Description = tagname.Attribute("Description") != null ? tagname.Attribute("Description").Value : string.Empty;
                    at.InitialValue = tagname.Attribute("InitialValue") != null ? tagname.Attribute("InitialValue").Value : string.Empty;
                    at.IsRetentive = tagname.Attribute("Retentive") != null ? tagname.Attribute("Retentive").Value == "1" : false;
                }
                if (tagfolder != null)
                {
                    try
                    {
                        UFUAModel.UFUAFolder tf;
#if DEBUG
                        //text = String.Format("Creating Folder took") + " : {0}";
                        //using (var stopwatcher = new StopWatcher(text))
#endif
                        {
                            tf = CreateFolder(folder, uow);
                            at.UFUAFolder = tf;
                        }
                    }
                    catch (Exception)
                    {
                        AppendLog(string.Format(Properties.Resources.TagNotIncluded, tagname.Value, tagfolder), bError: true);
                    }
                }

                if (!mapTagToFolder.ContainsKey(tagname.Value))
                    mapTagToFolder.Add(tagname.Value, at.GetRelativeName().Replace('/','\\'));

                //uow.CommitChanges();
                AppendLog(string.Format(Properties.Resources.TagImported, tagname.Value, tagfolder), false);

#if DEBUG
                //text = String.Format("AssignAlarm took") + " : {0}";
                //using (var stopwatcher = new StopWatcher(text))
#endif
                {
                    AssignAlarm(tag.Element("AlarmList"), at, uow);
                }

            }
            catch (Exception ex)
            {
                AppendLog(string.Format(Properties.Resources.TagError, tagname.Value, tagfolder, ex.ToString()), bError: true);
            }

        }

        string AddDynSettings(XElement tagname, XAttribute tagfolder)
        {
            string _dynstring;

            if (tagname.Attribute("DynamicSettings") != null && !string.IsNullOrEmpty(tagname.Attribute("DynamicSettings").Value))
            {
                _dynstring = tagname.Attribute("DynamicSettings").Value.Replace("[DRV]", "");
            }
            else if (mapTagToStaticTaskLynk.ContainsKey(tagname.Value))
            {
                _dynstring = mapTagToStaticTaskLynk[tagname.Value];
            }
            else
                return string.Empty;

            try
            {
                string _drivername = _dynstring.Remove(_dynstring.IndexOf('.'));
                string _stationname = GetValue("Sta=", _dynstring);

                if (mapTagToDataType.ContainsKey(tagname.Value) && !string.IsNullOrEmpty(mapTagToDataType[tagname.Value].Station))
                    _stationname = mapTagToDataType[tagname.Value].Station;

                string _oldDRVName = _drivername;

                if (mapDriverNameToDll.ContainsKey(_drivername) && mapOldDllNameToNewOne.ContainsKey(mapDriverNameToDll[_drivername]))
                    _drivername = mapOldDllNameToNewOne[mapDriverNameToDll[_drivername]];
                else
                {
                    AppendLog(string.Format(Properties.Resources.DynLinkError, tagname.Value, tagfolder != null ? tagfolder.Value : string.Empty));
                    return string.Empty;
                }

                bool _outputatstartup;
                string _stoutputatstartup = GetValue("StartOut=", _dynstring);
                if (!string.IsNullOrEmpty(_stoutputatstartup))
                {
                    _outputatstartup = _stoutputatstartup.Equals("1");
                }
                else
                    _outputatstartup = false;

                int _linktype;
                string _slinktype = GetValue("TaskType=", _dynstring);
                if (!string.IsNullOrEmpty(_slinktype))
                {
                    _linktype = int.Parse(_slinktype);
                }
                else
                    _linktype = (int)LinkType.InputOutput;

                bool _swapbyte = false;
                bool _swapword = false;
                if (!string.IsNullOrEmpty(GetValue("SW=", _dynstring)))
                    _swapword = GetValue("SW=", _dynstring).Equals("true");


                DynTagSettings dynsettings;
                StringBuilder dynamicstring = new StringBuilder();
                string _startaddress = string.Empty;
                String StartAddressParameter;
                String FileNumberParameter;
                String FunctionCodeParameter;
                OldFunctionCode functionCode = OldFunctionCode.InputRegisters;
                string _filenumber = "0";

                switch (_drivername)
                {
                    case "S7TCP":
                        if (!string.IsNullOrEmpty(GetValue("SB=", _dynstring)))
                            _swapbyte = GetValue("SB=", _dynstring).Equals("true");

                        dynsettings = new DynTagSettings()
                        {
                            DriverName = _drivername,
                            StationName = _stationname,
                            SwapBytes = _swapbyte,
                            SwapWords = _swapword,
                            OutputAtStartup = _outputatstartup,
                            TagLinkType = _linktype
                        };

                        dynamicstring.Append(dynsettings.ToString());
                        dynamicstring.Append(DynamicStringParser.CharSep);

                        _startaddress = GetValue("Addr=", _dynstring);

                        StartAddressParameter = "SA";

                        dynamicstring.AppendFormat("{0}{1}{2}", StartAddressParameter, DynamicStringParser.CharAssign, _startaddress);
                        break;
                    case "ModbusTCP":
                        
                        if (!string.IsNullOrEmpty(GetValue("FC=", _dynstring)))
                            functionCode = (OldFunctionCode)int.Parse(GetValue("FC=", _dynstring));

                        if (!string.IsNullOrEmpty(GetValue("SB=", _dynstring)))
                        {
                            _swapbyte = (functionCode == OldFunctionCode.InputRegisters || functionCode == OldFunctionCode.MultipleRegisters || functionCode == OldFunctionCode.SingleRegisters) && mapTagToDataType[tagname.Value].SByte ? !GetValue("SB=", _dynstring).Equals("true") : GetValue("SB=", _dynstring).Equals("true");
                        }
                            

                        dynsettings = new DynTagSettings()
                        {
                            DriverName = _drivername,
                            StationName = _stationname,
                            SwapBytes = _swapbyte,
                            SwapWords = _swapword,
                            OutputAtStartup = _outputatstartup,
                            TagLinkType = _linktype
                        };

                        dynamicstring.Append(dynsettings.ToString());
                        dynamicstring.Append(DynamicStringParser.CharSep);

                        _startaddress = GetValue("SA=", _dynstring);

                        StartAddressParameter = "SA";
                        FileNumberParameter = "File";
                        FunctionCodeParameter = "FC";

                        //dynamicstring.Append(DynamicStringParser.CharSep);
                        dynamicstring.AppendFormat("{0}{1}{2}", FunctionCodeParameter, DynamicStringParser.CharAssign, (int)functionCode);
                        dynamicstring.Append(DynamicStringParser.CharSep);
                        dynamicstring.AppendFormat("{0}{1}{2}", StartAddressParameter, DynamicStringParser.CharAssign, _startaddress);
                        dynamicstring.Append(DynamicStringParser.CharSep);
                        dynamicstring.AppendFormat("{0}{1}{2}", FileNumberParameter, DynamicStringParser.CharAssign, _filenumber);
                        break;
                    case "Modbus":

                        if (!string.IsNullOrEmpty(GetValue("FC=", _dynstring)))
                            functionCode = (OldFunctionCode)int.Parse(GetValue("FC=", _dynstring));

                        if (!string.IsNullOrEmpty(GetValue("SB=", _dynstring)))
                        {
                            _swapbyte = (functionCode == OldFunctionCode.FileRecord || functionCode == OldFunctionCode.InputRegisters || functionCode == OldFunctionCode.MultipleRegisters || functionCode == OldFunctionCode.SingleRegisters) && mapTagToDataType[tagname.Value].SByte ? !GetValue("SB=", _dynstring).Equals("true") : GetValue("SB=", _dynstring).Equals("true");
                        }


                        dynsettings = new DynTagSettings()
                        {
                            DriverName = _drivername,
                            StationName = _stationname,
                            SwapBytes = _swapbyte,
                            SwapWords = _swapword,
                            OutputAtStartup = _outputatstartup,
                            TagLinkType = _linktype
                        };

                        dynamicstring.Append(dynsettings.ToString());
                        dynamicstring.Append(DynamicStringParser.CharSep);

                        _startaddress = GetValue("SA=", _dynstring);

                        StartAddressParameter = "SA";
                        FileNumberParameter = "File";
                        FunctionCodeParameter = "FC";

                        //dynamicstring.Append(DynamicStringParser.CharSep);
                        dynamicstring.AppendFormat("{0}{1}{2}", FunctionCodeParameter, DynamicStringParser.CharAssign, (int)functionCode);
                        dynamicstring.Append(DynamicStringParser.CharSep);
                        dynamicstring.AppendFormat("{0}{1}{2}", StartAddressParameter, DynamicStringParser.CharAssign, _startaddress);
                        dynamicstring.Append(DynamicStringParser.CharSep);
                        dynamicstring.AppendFormat("{0}{1}{2}", FileNumberParameter, DynamicStringParser.CharAssign, _filenumber);
                        break;
                    default:
                        break;
                }

                if (!mapTagToDynamicLynk.ContainsKey(tagname.Value))
                    mapTagToDynamicLynk.Add(tagname.Value,dynamicstring.ToString());
                else
                    mapTagToDynamicLynk[tagname.Value] = dynamicstring.ToString();

                return dynamicstring.ToString();
            }
            catch (Exception)
            {
                AppendLog(string.Format(Properties.Resources.DynLinkError, tagname.Value, tagfolder != null ? tagfolder.Value : string.Empty), bError: true);
                return string.Empty;
            }
        }

        string GetValue(string p, string _dynstring)
        {
            if (_dynstring.IndexOf(p) == -1)
                return string.Empty;

            string _value = _dynstring.Substring(_dynstring.IndexOf(p));
            if(_value.IndexOf("|") != -1)
                _value = _value.Remove(_value.IndexOf("|"));
            _value = _value.Replace(p, "");
            return _value;
        }

        void AssignAlarm(XElement alarmlist, UFUAModel.UFUATag at, UnitOfWork uow)
        {
            if (alarmlist!=null)
                foreach (XAttribute alarm in alarmlist.Attributes().AsParallel())
                {
                    try
                    {
                        if (mapAlarmNameToSourceAlarm.ContainsKey(alarm.Value))
                        {
                            UFUAModel.UFUAAlarmSource source = (from entry in new XPQuery<UFUAModel.UFUAAlarmSource>(uow).AsParallel()
                                                               where entry.NodeId == mapAlarmNameToSourceAlarm[alarm.Value]
                                                               select entry).FirstOrDefault();

                            if(source == null)
                                AppendLog(string.Format(Properties.Resources.AlarmAssErr, alarm.Value, at.Name));
                            else
                            {
                                foreach (var item in source.UFUAAlarmDefinitions)
                                {
                                    at.UFUAAlarmThresholds.Add(new UFUAModel.UFUAAlarmThreshold(item, uow));
                                    AppendLog(string.Format(Properties.Resources.AlarmAss, alarm.Value, at.Name), false);
                                }
                            }
                        }
                        else
                            AppendLog(string.Format(Properties.Resources.AlarmAssErr, alarm.Value, at.Name));
                    }
                    catch (Exception)
                    {
                        AppendLog(string.Format(Properties.Resources.AlarmAssErr, alarm.Value, at.Name), bError: true);
                    }
                }


            string _source = (from k in mapAlarmNameToTagName.Keys.AsParallel() where mapAlarmNameToTagName[k] == at.Name select k).FirstOrDefault();
            if (string.IsNullOrEmpty(_source) || !mapAlarmNameToSourceAlarm.ContainsKey(_source))
                return;
            
            UFUAModel.UFUAAlarmSource lsource = (from entry in new XPQuery<UFUAModel.UFUAAlarmSource>(uow).AsParallel()
                                                where entry.NodeId == mapAlarmNameToSourceAlarm[_source]
                                                select entry).FirstOrDefault();
            if (lsource == null)
                AppendLog(string.Format(Properties.Resources.AlarmAssErr, _source, at.Name));
            else
            {
                foreach (var item in lsource.UFUAAlarmDefinitions)
                {
                    if ((from alarmthreashold in at.UFUAAlarmThresholds where alarmthreashold.UFUAAlarmDefinitionRef == item select alarmthreashold).FirstOrDefault() != null)
                       continue;
                    try
                    {
                        UFUAModel.UFUAAlarmThreshold _at = new UFUAModel.UFUAAlarmThreshold(item, uow);
                        if (mapAlarmNameToTagExression.ContainsKey(_source) && mapAlarmNameToTagExression[_source].Length > 0)
                            _at.Expression = string.Format(".{0}", mapAlarmNameToTagExression[_source]);

                        if (mapAlarmDefinitionToTextValue.ContainsKey(item.NodeId) && mapAlarmDefinitionToTextValue[item.NodeId].Length > 0)
                            _at.AlarmText = mapAlarmDefinitionToTextValue[item.NodeId];

                        at.UFUAAlarmThresholds.Add(_at);
                        AppendLog(string.Format(Properties.Resources.AlarmAss, _source, at.Name), false);
                    }
                    catch (Exception)
                    {
                        AppendLog(string.Format(Properties.Resources.AlarmAssErr, _source, at.Name), bError: true);
                    }
                }
            }
        }

        UFUAModel.ModelType GetModelType(OldDataType oldtype)
        {
            if (oldtype == OldDataType.Structure)
                return UFUAModel.ModelType.ObjectType;
            else
                return UFUAModel.ModelType.Analog;
        }

        uint GetArrayDimension(OldDataType oldtype, XAttribute bit)
        {
            if (oldtype == OldDataType.FixedLenghtArray)
                return uint.Parse(bit.Value);
            else
                return 0;
        }

        string GetDataPrototype(OldDataType p, XAttribute structtype, UnitOfWork uow)
        {
            if (p == OldDataType.Structure && structtype != null && mapTagPrototype.ContainsKey(structtype.Value))

                return (from entry in new XPQuery<UFUAModel.UFUATagPrototype>(uow).AsParallel()
                        where entry.NodeId == mapTagPrototype[structtype.Value] && entry.UFUATagOwner == null
                        select entry.Name).FirstOrDefault();
            else
                return null;
        }

        UFUAModel.DataType? GetDataType(OldDataType oldtype, XAttribute elementtype)
        {
            if (oldtype == OldDataType.Structure)
                return null;
            else if (oldtype == OldDataType.FixedLenghtArray)
            {
                if (elementtype != null)
                    return CheckDataType((OldDataType)int.Parse(elementtype.Value));
                else 
                    return null;
            }
            else
                return CheckDataType(oldtype);
        }

        UFUAModel.UFUAFolder CreateFolder(string folder, UnitOfWork uow)
        {
            UFUAModel.UFUAFolder tf;
            if (!mapTagFolder.ContainsKey(folder))
            {
                if (folder.LastIndexOf('.') != -1)
                {
                    tf = CreateFolder(folder.Remove(folder.LastIndexOf('.')), uow);
                    Guid nodeid = Guid.NewGuid();
                    tf.UFUAFolders.Add(
                        new UFUAModel.UFUAFolder(uow) { Name = folder.Split('.').LastOrDefault(), NodeId = nodeid }
                        );
                    mapTagFolder.Add(folder, nodeid);
                }
                else
                {
                    tf = new UFUAModel.UFUAFolder(uow) { Name = folder, NodeId = Guid.NewGuid() };
                    mapTagFolder.Add(folder, tf.NodeId);
                }

                uow.CommitChanges();

            }
//            else
            {
                tf = (from entry in new XPQuery<UFUAModel.UFUAFolder>(uow).AsParallel()
                      where entry.NodeId == mapTagFolder[folder]
                      select entry).FirstOrDefault();
            }

            return tf;
        }
        #endregion

        #region Alarm
        void AddAlarm(XElement t, XElement alarmtagname, UnitOfWork uow)
        {
            XElement thresholdtagname = t.Element("Name");
            XElement thresholdtagexecution = t.Element("Execution");
            XElement thresholdtagstyle = t.Element("Style");
            OldCondition condition = (OldCondition)(int.Parse(thresholdtagexecution.Attribute("Condition").Value));

            //if (condition == OldCondition.Between)
            //{
            //    AppendLog(string.Format(Properties.Resources.THrError, thresholdtagname.Value, condition.ToString(), alarmtagname.Value));
            //    return;
            //}

            UFUAModel.UFUAArea aa;
            UFUAModel.UFUAAlarmSource aSource;
            string areaname = !string.IsNullOrEmpty(thresholdtagname.Attribute("Area").Value) ? thresholdtagname.Attribute("Area").Value : !string.IsNullOrEmpty(alarmtagname.Attribute("Area").Value) ? alarmtagname.Attribute("Area").Value : Properties.Resources.AlarmAreaName;

            if (!mapAlarmAreas.ContainsKey(areaname))
            {
                aa = new UFUAModel.UFUAArea(uow) { Name = areaname, NodeId = Guid.NewGuid() };
                mapAlarmAreas.Add(areaname, aa.NodeId);
                uow.CommitChanges();
            }
            else
            {
                aa = (from entry in new XPQuery<UFUAModel.UFUAArea>(uow).AsParallel()
                      where entry.NodeId == mapAlarmAreas[areaname]
                      select entry).FirstOrDefault();
            }

            if (aa == null)
            {
                AppendLog(string.Format(Properties.Resources.AlarmError, alarmtagname.Value));
                return;
            }

            string sourcename = alarmtagname.Value;
            if (!mapSourceAlarmAreas.ContainsKey(areaname))
            {
                aSource = new UFUAModel.UFUAAlarmSource(uow) { Name = sourcename, NodeId = Guid.NewGuid() };
                Dictionary<string, Guid> sourcedict = new Dictionary<string, Guid>();
                sourcedict.Add(sourcename, aSource.NodeId);
                mapSourceAlarmAreas.Add(areaname, sourcedict);
                aa.UFUAAlarmSources.Add(aSource);
                uow.CommitChanges();
            }
            else
            {
                if (mapSourceAlarmAreas[areaname].ContainsKey(sourcename))
                    aSource = (from entry in new XPQuery<UFUAModel.UFUAAlarmSource>(uow).AsParallel()
                               where entry.NodeId == mapSourceAlarmAreas[areaname][sourcename]
                               select entry).FirstOrDefault();
                else
                {
                    aSource = new UFUAModel.UFUAAlarmSource(uow) { Name = sourcename, NodeId = Guid.NewGuid() };
                    mapSourceAlarmAreas[areaname].Add(sourcename, aSource.NodeId);
                    aa.UFUAAlarmSources.Add(aSource);
                    uow.CommitChanges();
                }
            }

            if (aSource == null)
            {
                AppendLog(string.Format(Properties.Resources.AlarmError, alarmtagname.Value));
                return;
            }

            if (!mapAlarmNameToSourceAlarm.ContainsKey(sourcename))
                mapAlarmNameToSourceAlarm.Add(sourcename, aSource.NodeId);

            //if (alarmtagname.Attribute("Variable") != null && !mapAlarmNameToTagName.ContainsKey(sourcename))
            //    mapAlarmNameToTagName.Add(sourcename, alarmtagname.Attribute("Variable").Value);
            if (alarmtagname.Attribute("Variable") != null)
            {
                if(!mapAlarmNameToTagName.ContainsKey(sourcename))
                {
                    string _name = alarmtagname.Attribute("Variable").Value;
                    string _bit = string.Empty;
                    if (_name.IndexOf('.') > 0)
                    {
                        try
                        {
                            var _list = alarmtagname.Attribute("Variable").Value.Split('.');
                            _name = _list[0];
                            _bit = _list[1];
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    mapAlarmNameToTagName.Add(sourcename, _name);
                    if (!mapAlarmNameToTagExression.ContainsKey(sourcename))
                        mapAlarmNameToTagExression.Add(sourcename, _bit);
                }
            }

            try
            {
                var aAlarmDefinition = new UFUAModel.UFUAAlarmDefinition(uow) { Name = thresholdtagname.Value, NodeId = Guid.NewGuid() };

                if (!mapAlarmDefinitionToTextValue.ContainsKey(aAlarmDefinition.NodeId))
                    mapAlarmDefinitionToTextValue.Add(aAlarmDefinition.NodeId, thresholdtagname.Attribute("Title").Value);

                if (condition == OldCondition.RateChangeIncrease)
                {
                    aAlarmDefinition.AlarmType = UFUAModel.AlarmType.ExclusiveRateOfChange;
                    aAlarmDefinition.EnableHighLimit = true;
                    aAlarmDefinition.HighLimit = double.Parse(thresholdtagexecution.Attribute("Threshold").Value);
                }
                else if (condition == OldCondition.RateChangeDecrease)
                {
                    aAlarmDefinition.AlarmType = UFUAModel.AlarmType.ExclusiveRateOfChange;
                    aAlarmDefinition.EnableLowLimit = true;
                    aAlarmDefinition.LowLimit = double.Parse(thresholdtagexecution.Attribute("Threshold").Value);
                }
                else
                {
                    aAlarmDefinition.AlarmType = UFUAModel.AlarmType.TripAlarm;
                    aAlarmDefinition.ActivationLowValue = double.Parse(thresholdtagexecution.Attribute("ThresholdLow").Value);
                    aAlarmDefinition.ActivationValue = double.Parse(thresholdtagexecution.Attribute("Threshold").Value);
                    aAlarmDefinition.ConditionType = CheckCondition(condition);
                }

                if (thresholdtagstyle.Attribute("SupportAck").Value.Equals("0") && thresholdtagstyle.Attribute("SupportReset").Value.Equals("0"))
                    aAlarmDefinition.Severity = 0;
                else
                    aAlarmDefinition.Severity = int.Parse(thresholdtagexecution.Attribute("Severity").Value);
                aAlarmDefinition.SaveEventsLog = thresholdtagstyle.Attribute("Log").Value.Equals("1");
                aAlarmDefinition.SupportAck = thresholdtagstyle.Attribute("SupportAck").Value.Equals("1");
                aAlarmDefinition.SupportReset = thresholdtagstyle.Attribute("SupportReset").Value.Equals("1");
                aAlarmDefinition.Beep = thresholdtagstyle.Attribute("BeepEnabled").Value.Equals("1");
                aAlarmDefinition.DelayTimeOn = TimeSpan.FromSeconds(int.Parse(thresholdtagexecution.Attribute("SecDelay").Value));
                aSource.UFUAAlarmDefinitions.Add(aAlarmDefinition);

                uow.CommitChanges();
                AppendLog(string.Format(Properties.Resources.THrImported, thresholdtagname.Value, alarmtagname.Value), false);
            }
            catch (Exception ex)
            {
                AppendLog(string.Format(Properties.Resources.THrNotImported, thresholdtagname.Value, alarmtagname.Value, ex.ToString()), bError: true);
                return;
            }
        }

        void AddExclusiveAlarm(Dictionary<int, ExclusiveAlarm> exclusivealarm, XElement alarmtagname, UnitOfWork uow)
        {
            UFUAModel.UFUAArea aa;
            UFUAModel.UFUAAlarmSource aSource;
            string areaname = !string.IsNullOrEmpty(alarmtagname.Attribute("Area").Value) ? alarmtagname.Attribute("Area").Value : Properties.Resources.AlarmAreaName;

            if (!mapAlarmAreas.ContainsKey(areaname))
            {
                aa = new UFUAModel.UFUAArea(uow) { Name = areaname, NodeId = Guid.NewGuid() };
                mapAlarmAreas.Add(areaname, aa.NodeId);
                uow.CommitChanges();
            }
            else
            {
                aa = (from entry in new XPQuery<UFUAModel.UFUAArea>(uow).AsParallel()
                      where entry.NodeId == mapAlarmAreas[areaname]
                      select entry).FirstOrDefault();
            }

            if (aa == null)
            {
                AppendLog(string.Format(Properties.Resources.AlarmError, alarmtagname.Value));
                return;
            }

            string sourcename = alarmtagname.Value;
            if (!mapSourceAlarmAreas.ContainsKey(areaname))
            {
                aSource = new UFUAModel.UFUAAlarmSource(uow) { Name = sourcename, NodeId = Guid.NewGuid() };
                Dictionary<string, Guid> sourcedict = new Dictionary<string, Guid>();
                sourcedict.Add(sourcename, aSource.NodeId);
                mapSourceAlarmAreas.Add(areaname, sourcedict);
                aa.UFUAAlarmSources.Add(aSource);
                uow.CommitChanges();
            }
            else
            {
                if (mapSourceAlarmAreas[areaname].ContainsKey(sourcename))
                    aSource = (from entry in new XPQuery<UFUAModel.UFUAAlarmSource>(uow).AsParallel()
                               where entry.NodeId == mapSourceAlarmAreas[areaname][sourcename]
                               select entry).FirstOrDefault();
                else
                {
                    aSource = new UFUAModel.UFUAAlarmSource(uow) { Name = sourcename, NodeId = Guid.NewGuid() };
                    mapSourceAlarmAreas[areaname].Add(sourcename, aSource.NodeId);
                    aa.UFUAAlarmSources.Add(aSource);
                    uow.CommitChanges();
                }
            }

            if (aSource == null)
            {
                AppendLog(string.Format(Properties.Resources.AlarmError, alarmtagname.Value));
                return;
            }

            if (!mapAlarmNameToSourceAlarm.ContainsKey(sourcename))
                mapAlarmNameToSourceAlarm.Add(sourcename, aSource.NodeId);

            //if (alarmtagname.Attribute("Variable") != null && !mapAlarmNameToTagName.ContainsKey(sourcename))
            //    mapAlarmNameToTagName.Add(sourcename, alarmtagname.Attribute("Variable").Value);
            if (alarmtagname.Attribute("Variable") != null)
            {
                if (!mapAlarmNameToTagName.ContainsKey(sourcename))
                {
                    string _name = alarmtagname.Attribute("Variable").Value;
                    string _bit = string.Empty;
                    if (_name.IndexOf('.') > 0)
                    {
                        try
                        {
                            var _list = alarmtagname.Attribute("Variable").Value.Split('.');
                            _name = _list[0];
                            _bit = _list[1];
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    mapAlarmNameToTagName.Add(sourcename, _name);
                    if (!mapAlarmNameToTagExression.ContainsKey(sourcename))
                        mapAlarmNameToTagExression.Add(sourcename, _bit);
                }
            }

            int z=0;
            foreach (var item in exclusivealarm.Values.AsParallel())
	        {
                string thname = string.Empty;
                StringBuilder importedt = new StringBuilder(string.Empty);
                try
                {
                    var aAlarmDefinition = new UFUAModel.UFUAAlarmDefinition(uow) { Name = string.Format("{0}_{1}{2}", alarmtagname.Value,Properties.Resources.ExclusiveThresholds, z++), NodeId = Guid.NewGuid() };
                    aAlarmDefinition.AlarmType = UFUAModel.AlarmType.ExclusiveLevel;
                
                    int severity = 0;
                    int delayTimeOn = 0;
                    int k = 0;
                    bool log = false;
                    bool beep = false;
                    
                    if(item.LowLimit != null)
                    {
                        try
                        {
                            if (int.Parse(item.LowLimit.Element("Execution").Attribute("Severity").Value) > severity)
                                severity = int.Parse(item.LowLimit.Element("Execution").Attribute("Severity").Value);
                            if (int.Parse(item.LowLimit.Element("Execution").Attribute("SecDelay").Value) < delayTimeOn)
                                delayTimeOn = int.Parse(item.LowLimit.Element("Execution").Attribute("SecDelay").Value);
                            log = item.LowLimit.Element("Style").Attribute("Log").Value.Equals("1") ? true : log;
                            beep = item.LowLimit.Element("Style").Attribute("BeepEnabled").Value.Equals("1") ? true : beep;

                            importedt.Append(item.LowLimit.Element("Name").Value);
                            importedt.Append("; ");

                            aAlarmDefinition.LowLimit = double.Parse(item.LowLimit.Element("Execution").Attribute("Threshold").Value);
                            aAlarmDefinition.EnableLowLimit = true;
                        }
                        catch (Exception)
                        {
                        }
                    }
                    if(item.LowLowLimit != null)
                    {
                        try
                        {
                            if (int.Parse(item.LowLowLimit.Element("Execution").Attribute("Severity").Value) > severity)
                                severity = int.Parse(item.LowLowLimit.Element("Execution").Attribute("Severity").Value);
                            if (int.Parse(item.LowLowLimit.Element("Execution").Attribute("SecDelay").Value) < delayTimeOn)
                                delayTimeOn = int.Parse(item.LowLowLimit.Element("Execution").Attribute("SecDelay").Value);
                            log = item.LowLowLimit.Element("Style").Attribute("Log").Value.Equals("1") ? true : log;
                            beep = item.LowLowLimit.Element("Style").Attribute("BeepEnabled").Value.Equals("1") ? true : beep;
                            importedt.Append(item.LowLowLimit.Element("Name").Value);
                            importedt.Append("; ");

                            aAlarmDefinition.LowLowLimit = double.Parse(item.LowLowLimit.Element("Execution").Attribute("Threshold").Value);
                            aAlarmDefinition.EnableLowLowLimit = true;
                        }
                        catch (Exception)
                        {
                        }
                    }
                    if(item.HighLimit != null)
                    {
                        try
                        {
                            if (int.Parse(item.HighLimit.Element("Execution").Attribute("Severity").Value) > severity)
                                severity = int.Parse(item.HighLimit.Element("Execution").Attribute("Severity").Value);
                            if (int.Parse(item.HighLimit.Element("Execution").Attribute("SecDelay").Value) < delayTimeOn)
                                delayTimeOn = int.Parse(item.HighLimit.Element("Execution").Attribute("SecDelay").Value);
                            log = item.HighLimit.Element("Style").Attribute("Log").Value.Equals("1") ? true : log;
                            beep = item.HighLimit.Element("Style").Attribute("BeepEnabled").Value.Equals("1") ? true : beep;
                            importedt.Append(item.HighLimit.Element("Name").Value);
                            importedt.Append("; ");

                            aAlarmDefinition.HighLimit = double.Parse(item.HighLimit.Element("Execution").Attribute("Threshold").Value);
                            aAlarmDefinition.EnableHighLimit = true;
                        }
                        catch (Exception)
                        {
                        }
                    }
                    if(item.HighHighLimit != null)
                    {
                        try
                        {
                            if(int.Parse(item.HighHighLimit.Element("Execution").Attribute("Severity").Value) > severity)
                                severity = int.Parse(item.HighHighLimit.Element("Execution").Attribute("Severity").Value);
                            if (int.Parse(item.HighHighLimit.Element("Execution").Attribute("SecDelay").Value) < delayTimeOn)
                                delayTimeOn = int.Parse(item.HighHighLimit.Element("Execution").Attribute("SecDelay").Value);
                            log = item.HighHighLimit.Element("Style").Attribute("Log").Value.Equals("1") ? true : log;
                            beep = item.HighHighLimit.Element("Style").Attribute("BeepEnabled").Value.Equals("1") ? true : beep;
                            importedt.Append(item.HighHighLimit.Element("Name").Value);
                            importedt.Append("; ");

                            aAlarmDefinition.HighHighLimit = double.Parse(item.HighHighLimit.Element("Execution").Attribute("Threshold").Value);
                            aAlarmDefinition.EnableHighHighLimit = true;
                        }
                        catch (Exception)
                        {
                        }
                    }

                    aAlarmDefinition.Severity = severity;
                    aAlarmDefinition.SaveEventsLog = log;
                    aAlarmDefinition.Beep = beep;
                    aAlarmDefinition.DelayTimeOn = TimeSpan.FromSeconds(delayTimeOn);
                    aSource.UFUAAlarmDefinitions.Add(aAlarmDefinition);

                    uow.CommitChanges();
                    AppendLog(string.Format(Properties.Resources.THrImported, importedt.ToString(), alarmtagname.Value), false);
                }
                catch (Exception ex)
                {
                    AppendLog(string.Format(Properties.Resources.THrNotImported, importedt.ToString(), alarmtagname.Value, ex.ToString()), bError: true);
                    return;
                }
	        }

        }

        UFUAModel.ConditionType CheckCondition(OldCondition p)
        {
            switch (p)
            {
                case OldCondition.GreaterThanOrEqual:
                    return UFUAModel.ConditionType.GreaterThanOrEqual;
                case OldCondition.LessThanOrEqual:
                    return UFUAModel.ConditionType.LessThanOrEqual;
                case OldCondition.Equals:
                    return UFUAModel.ConditionType.Equals;
                case OldCondition.NotEqual:
                    return UFUAModel.ConditionType.NotEqual;
                case OldCondition.Between:
                    return UFUAModel.ConditionType.Between;
                case OldCondition.RateChangeDecrease:
                case OldCondition.RateChangeIncrease:
                default:
                    return UFUAModel.ConditionType.GreaterThanOrEqual;
            }
        }
        #endregion

        #region Methods
        static InMemoryDataStore GetDataStore(string filebase)
        {
            InMemoryDataStore InMemory = new InMemoryDataStore(DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema, true);
            try
            {
                InMemory.ReadXml(filebase);
            }
            catch (Exception ex)
            {

            }
            return InMemory;
        }

        static string GetAssemblyPath()
        {
            string basedir = AppDomain.CurrentDomain.BaseDirectory;
            return basedir;
        }

        Dictionary<string, object> GetElements(string filepath, string descendantid, bool p2, string name,bool ditinct = false)
        {
            Dictionary<string, object> mapTag = new Dictionary<string, object>();
            var keyexpandolist = GetElementsFromXml(filepath, descendantid, name, p2, ditinct);
            if (keyexpandolist.Count() != 0)
            {
                keyexpandolist.ToList().ForEach(e =>
                {
                    var regkeydictionary = e as IDictionary<string, object>;
                    regkeydictionary.ToList().ForEach(r =>
                    {
                        try
                        {
                            mapTag.Add(r.Key, r.Value);
                        }
                        catch
                        {
                        }
                    });
                });
            }
            
            return mapTag;
        }

        IEnumerable<dynamic> GetElementsFromXml(string file, string descendantid, string descendantname, bool fromcode = false, bool distinct = false)
        {
            var expandoFromXml = new List<dynamic>();
            int index = 0;
            XDocument doc = new XDocument();
            if (fromcode)
            {
                doc = XDocument.Parse(file);
            }
            else
            {
                doc = XDocument.Load(file);
            }
            foreach (var element in doc.Descendants(descendantid).AsParallel()/*.AsParallel()*/)
            {
                dynamic expandoObject = new ExpandoObject();
                var dictionary = expandoObject as IDictionary<string, object>;
                foreach (var child in element.Descendants(descendantname).AsParallel())
                {
                    //if (child.Name.Namespace == "" && child.Name == descendantname && (!distinct || (distinct && child.Parent.Name == descendantid)))
                      if (child.Name.Namespace == "" && (!distinct || (distinct && child.Parent.Name == descendantid)))
                        lock (dictionary)
                        {
                            //dictionary[child.Name.ToString()] = child.Value.Trim();
                            dictionary[string.Format("item{0}",index++)] = child;
                        }
                }
                yield return expandoObject;
            }

        }

        void AppendLog(string note, bool nImported = true, bool bError = false)
        {
            Dispatcher.BeginInvokeIfRequired(() =>
            {
                string LogPath = string.Empty;
                string LogFile = "ImportedProjectLog.log";
                string LogNotImportedFile = "NotImportedProjectLog.log";

                if (bError)
                    log.Error(note);

                if (!(bool)checkboxNotImported.IsChecked)
                    return;
                try 
	            {
                    if (ImportWizardPluginComponent.DocFileSystemProvider != null)
                    {
                        LogPath = System.IO.Path.Combine(ApplicationPropertiesHelper.GetProperty("ProjectFolder").ToString(), ImportWizardPluginComponent.NewProject.ProjectName);
                    }
                    else
                    {
                        LogPath = System.IO.Path.GetDirectoryName(ImportWizardPluginComponent.NewProject.ProjectPath);
                    }

                    if (nImported && (bool)checkboxNotImported.IsChecked)
                    {
                        File.AppendAllText(System.IO.Path.Combine(LogPath, LogNotImportedFile), string.Format("{0}{1}", note, Environment.NewLine));
                    }
                    else if (!nImported && (bool)checkboxImported.IsChecked)
                    {
                        File.AppendAllText(System.IO.Path.Combine(LogPath, LogFile), string.Format("{0}{1}", note, Environment.NewLine));
                    }
	            }
	            catch (Exception)
	            {
	            }
            }); 
        }

        void InitLog()
        {
            //Dispatcher.BeginInvokeIfRequired(() =>
            {
                if (!(bool)checkboxNotImported.IsChecked)
                    return;

                string LogPath = string.Empty;
                string LogFile = "ImportedProjectLog.log";
                string LogNotImportedFile = "NotImportedProjectLog.log";
                try
                {
                    if (ImportWizardPluginComponent.DocFileSystemProvider != null)
                    {
                        LogPath = System.IO.Path.Combine(ApplicationPropertiesHelper.GetProperty("ProjectFolder").ToString(), ImportWizardPluginComponent.NewProject.ProjectName);
                    }
                    else
                    {
                        LogPath = System.IO.Path.GetDirectoryName(ImportWizardPluginComponent.NewProject.ProjectPath);
                    }

                    if ((bool)checkboxNotImported.IsChecked)
                    {
                        if (!Directory.Exists(LogPath))
                            Directory.CreateDirectory(LogPath);

                        if (File.Exists(System.IO.Path.Combine(LogPath, LogNotImportedFile)) && new FileInfo(System.IO.Path.Combine(LogPath, LogNotImportedFile)).Length > (1024 * 1024))       // ## NOTE: 1MB max file size
                        {
                            File.Move(System.IO.Path.Combine(LogPath, LogNotImportedFile), System.IO.Path.Combine(LogPath, string.Format("{0}{1}.log", System.IO.Path.GetFileNameWithoutExtension(LogNotImportedFile), string.Format("_{0}{1}{2}_{3}{4}{5}", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second))));
                            File.Delete(System.IO.Path.Combine(LogPath, LogNotImportedFile));
                        }

                        File.AppendAllText(System.IO.Path.Combine(LogPath, LogNotImportedFile), string.Format("{0}{1}", Properties.Resources.CapTitle1, Environment.NewLine));
                        File.AppendAllText(System.IO.Path.Combine(LogPath, LogNotImportedFile), string.Format("{0}{1}", Properties.Resources.NotImportedTitle, Environment.NewLine));
                        File.AppendAllText(System.IO.Path.Combine(LogPath, LogNotImportedFile), string.Format("{0}{1}", Properties.Resources.CapTitle1, Environment.NewLine));
                    }

                    if ((bool)checkboxImported.IsChecked)
                    {

                        if (!Directory.Exists(LogPath))
                            Directory.CreateDirectory(LogPath);

                        if (File.Exists(System.IO.Path.Combine(LogPath, LogFile)) && new FileInfo(System.IO.Path.Combine(LogPath, LogFile)).Length > (1024 * 1024))       // ## NOTE: 1MB max file size
                        {
                            File.Move(System.IO.Path.Combine(LogPath, LogFile), System.IO.Path.Combine(LogPath, string.Format("{0}{1}.log", System.IO.Path.GetFileNameWithoutExtension(LogFile), string.Format("_{0}{1}{2}_{3}{4}{5}", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second))));
                            File.Delete(System.IO.Path.Combine(LogPath, LogFile));
                        }

                        if (File.Exists(System.IO.Path.Combine(LogPath, LogFile)) && new FileInfo(System.IO.Path.Combine(LogPath, LogFile)).Length > 0)
                            File.AppendAllText(System.IO.Path.Combine(LogPath, LogFile), string.Format("{0}", Environment.NewLine));

                        File.AppendAllText(System.IO.Path.Combine(LogPath, LogFile), string.Format("{0}{1}", Properties.Resources.CapTitle1, Environment.NewLine));
                        File.AppendAllText(System.IO.Path.Combine(LogPath, LogFile), string.Format("{0}{1}", Properties.Resources.ImportedTitle, Environment.NewLine));
                        File.AppendAllText(System.IO.Path.Combine(LogPath, LogFile), string.Format("{0}{1}", Properties.Resources.CapTitle1, Environment.NewLine));

                        importedList.ForEach(x => File.AppendAllText(System.IO.Path.Combine(LogPath, LogFile), string.Format("{0}{1}", x, Environment.NewLine)));
                    }
                }
                catch (Exception)
                {
                }
            }//);
        }
        #endregion

        #region Events
               
        readonly List<ComboBoxEdit> listFilled = new List<ComboBoxEdit>();
        private void CULTURE_Editor_DropDownOpened(object sender, EventArgs e)
        {
            var combo = sender as ComboBoxEdit;
            if (combo == null || listFilled.Contains(combo))
                return;
            listFilled.Add(combo);

            List<System.Globalization.CultureInfo> listAvailableLanguages = CultureInfo.GetCultures(CultureTypes.AllCultures).ToList();
            listAvailableLanguages.RemoveAll(culture => culture.IsNeutralCulture);
            if (listAvailableLanguages.Contains(CultureInfo.InvariantCulture))
                listAvailableLanguages.Remove(CultureInfo.InvariantCulture);
            combo.ItemsSource = listAvailableLanguages.OrderBy(culture => culture.DisplayName);
        }
        private void tableView_FocusedRowChanged(object sender, DevExpress.Xpf.Grid.FocusedRowChangedEventArgs e)
        {
            int listIndex = gridControl.GetRowListIndex(tableView.FocusedRowHandle);
            btnDelete.IsEnabled = listIndex >= 0;
        }
        private void OnAdd(object sender, RoutedEventArgs e)
        {
            string LogPath = string.Empty;
            
            if(ImportWizardPluginComponent.NewProject.SourceProject != null)
                LogPath = System.IO.Path.GetDirectoryName(ImportWizardPluginComponent.NewProject.SourceProject.LocalPath);

            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Title = Properties.Resources.SourceFileTitle;
            // Set filter for file extension and default file extension
            dlg.DefaultExt = "*";
            dlg.Filter = "All files (*.*)|*.*";
            dlg.ValidateNames = false;
            dlg.CheckPathExists = false;
            if (!string.IsNullOrEmpty(LogPath))
            {
                dlg.InitialDirectory = LogPath;
            }

            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();
            string _name = string.Empty;
            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                _name = dlg.FileName;
            }

            var list = (from s in datalist select s.FileName).ToList();
            if (string.IsNullOrEmpty(_name) || list.Contains(_name))
                return;

            var newData = new StringOptions { FileName = _name };
            InsertRow(tableView.FocusedRowHandle, newData);
        }

        private void OnDelete(object sender, RoutedEventArgs e)
        {
            DeleteRow(tableView.FocusedRowHandle);
        }

        void DeleteRow(int rowHandle)
        {
            if (tableView.IsEditing)
                return;

            int listIndex = gridControl.GetRowListIndex(rowHandle);
            if (listIndex >= 0)
                datalist.RemoveAt(listIndex);

            gridControl.ItemsSource = null;
            gridControl.ItemsSource = datalist;
        }

        void InsertRow(int rowHandle, StringOptions data)
        {
            int listIndex = gridControl.GetRowListIndex(rowHandle);
            if (listIndex < 0 || listIndex >= datalist.Count) listIndex = -1;
            datalist.Insert(listIndex + 1, data);

            gridControl.ItemsSource = null;
            gridControl.ItemsSource = datalist;
        }

        #endregion

        private void PART_Editor_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            if ((sender as ComboBoxEdit).SelectedIndex == -1)
                e.Handled = true;
        }

        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;

            bDisposed = true;

            if (cts != null)
                cts.Dispose();

            mapTagPrototype.Clear();
            mapTagFolder.Clear();
            mapAlarmAreas.Clear(); 
            mapSourceAlarmAreas.Clear();
            mapAlarmNameToSourceAlarm.Clear();
            mapAlarmDefinitionToTextValue.Clear();
            mapAlarmNameToTagName.Clear();
            mapAlarmNameToTagExression.Clear();
            mapOldDllNameToNewOne.Clear();
            mapOldDllNameToNewOneReference.Clear();
            mapDriverNameToDll.Clear();
            mapDllToDriverName.Clear();
            mapTagToStaticTaskLynk.Clear();
            mapTagToDynamicLynk.Clear(); 
            mapTagToDataType.Clear();
            mapDriverTSBInvertion.Clear();
            mapVariableList.Clear();
            mapStructureList.Clear();
            mapDLRList.Clear();
            mapAlarmList.Clear();
            mapDriverInfo.Clear();
            mapTagToFolder.Clear();
        }
    }
}
