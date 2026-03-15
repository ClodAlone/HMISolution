using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using WPFUtilities;
using Utilities.WPF;
using log4net;
using UFProjectWizard.ComponentService;
using System.ComponentModel;
using Utilities;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeList;
using System.Windows.Media;
using HelpProvider.ComponentService;

namespace UFProjectWizard
{
    /// <summary>
    /// Helper class object used in the TreeViewAdvItem.Tag property.
    /// </summary>
    class CategoryTagHelper
    {
        public string category;
        public string selectedType;
        public string selectedSubType;
    }

    /// <summary>
    /// Helper class object used in the startup button's link.
    /// </summary>
    public class ButtonHelper
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public BitmapImage Image { get; set; }
        public Action Action { get; set; }
    }
    
    /// <summary>
    /// Interaction logic for ProjectWizardUI.xaml
    /// </summary>
    public partial class WizardUI : UserControl, INotifyPropertyChanged
    {
        // Fields...

        #region DP
            double _ZoomLevel = 100;
            public double ZoomLevel
            {
                get
                {
                    return _ZoomLevel;
                }
                set
                {
                    if (_ZoomLevel != value)
                    {
                        _ZoomLevel = value;
                        OnPropertyChanged("ZoomLevel");
                    }
                }
            }
            String _AssemblyName = string.Empty;
            public String AssemblyName
            {
                get
                {
                    return _AssemblyName;
                }
                set
                {
                    if (_AssemblyName != value)
                    {
                        _AssemblyName = value;
                        OnPropertyChanged("AssemblyName");
                    }
                }
            }
            String _AssemblyPath = string.Empty;
            public String AssemblyPath
            {
                get
                {
                    return _AssemblyPath;
                }
                set
                {
                    if (_AssemblyPath != value)
                    {
                        _AssemblyPath = value;
                        OnPropertyChanged("AssemblyPath");
                    }
                }
            }
            String _AssemblyDescription = string.Empty;
            public String AssemblyDescription
            {
                get
                {
                    return _AssemblyDescription;
                }
                set
                {
                    if (_AssemblyDescription != value)
                    {
                        _AssemblyDescription = value;
                        OnPropertyChanged("AssemblyDescription");
                    }
                }
            }


        
        #endregion

        #region INotifyPropertyChanged
        private void OnPropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Declarations
        TreeListControl treeViewAdv;
        List<TreeListNode> childItemsPopulated = new List<TreeListNode>();

        string StartingFolder { get; set; }
        string SelectedFolder { get; set; }
        bool bLoaded;

        static readonly ILog log = LogManager.GetLogger(Properties.Resources.WizardPluginGallery);

        public String sourcefileName { get; set; }
        Dictionary<TreeListNode, ObservableCollection<FrameworkElement>> mapFileList = new Dictionary<TreeListNode, ObservableCollection<FrameworkElement>>();
        ObservableCollection<FrameworkElement> listStyledObject = new ObservableCollection<FrameworkElement>();
        Dictionary<ButtonHelper, String> mapWizardEntities = new Dictionary<ButtonHelper, String>();
        System.Globalization.TextInfo textInfo;
        String dataExt = Properties.Settings.Default.WizardPluginInfoExtension;
        bool bScreenManagerLoaded;
        #endregion
        public WizardUI()
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    bLoaded = true;
                    textInfo = new System.Globalization.CultureInfo(System.Globalization.CultureInfo.CurrentCulture.Name, false).TextInfo;
                    ThemeHelper.SetTheme(this.FindParent<Window>());
                    bScreenManagerLoaded = ComponentService.UFProjectWizardComponent.projectWizardComponent.ScreenManagerService != null;

                    headerControl.HeaderTitle = textInfo.ToTitleCase(Properties.Resources.CreateNewProject);
                    headerControl.MinWidth = 650;
                    headerControl.MinHeight = 85;
                    headerControl.ShowSteps = false;

                    InitWizardList();
                }
            };
            Unloaded += (o, e) =>
            {
                if (bLoaded)
                {
                    bLoaded = false;
                }
            };
        }

        #region Methods

        private void InitWizardList()
        {
            var dllList = GetDllList();
            listBoxPlugins.ItemsSource = dllList;
        }

        private void listBoxPlugins_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            object selectedItem = listBoxPlugins.SelectedItem;
            if (selectedItem == null)
                return;
            if (mapWizardEntities.ContainsKey(selectedItem as ButtonHelper))
            {
                String fileName = mapWizardEntities[selectedItem as ButtonHelper];
                AssemblyPath = fileName;
                AssemblyName = DependencyObjectExtensions.AdaptName(System.IO.Path.GetFileNameWithoutExtension(fileName));
            }
        }

        private void listBoxPlugins_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var wnd = this.FindParent<Window>();
            if (wnd == null)
                return;
            wnd.DialogResult = true;
        }

        public List<ButtonHelper> GetDllList()
        {
            String extension = Properties.Settings.Default.PluginsExtension;
            String folder = String.Format("{0}{1}\\", String.Format("{0}", AppDomain.CurrentDomain.BaseDirectory), Properties.Settings.Default.PluginsFolder);
            var filter = String.Format("*{0}", extension);
            string[] directoryGetFiles = Directory.GetFiles(folder, filter);

            List<ButtonHelper> dlllist = new List<ButtonHelper>();
            foreach (var fileOn in directoryGetFiles)
            {
                var filename = Path.GetFileNameWithoutExtension(fileOn);
                if (!bScreenManagerLoaded && (filename == Properties.Settings.Default.ProLeanAssembly || filename == Properties.Settings.Default.ProEnergyAssembly))
                    continue;
                var list = FindAndLoadDLL.LoadDLLs<IUFProjectWizardPlugin>(folder, Path.GetFileName(fileOn), false);
                if (list.Count == 0)
                    continue;

                FileInfo file = new FileInfo(fileOn);
                if (file.Extension.Equals(Properties.Settings.Default.PluginsExtension))
                {
                    var filePng = System.IO.Path.ChangeExtension(fileOn, "png");
                    BitmapImage img = new BitmapImage();
                    if (!File.Exists(filePng))
                        img = UFProjectWizardComponent.GetControlImage("PWEditor");
                    else
                    {
                        img.BeginInit();
                        img.UriSource = new Uri(filePng, UriKind.RelativeOrAbsolute);
                        img.CacheOption = BitmapCacheOption.OnLoad;
                        img.EndInit();
                    }

                    var name = DependencyObjectExtensions.AdaptName(System.IO.Path.GetFileNameWithoutExtension(fileOn));

                    var fileDesc = GetFileDescription(fileOn);
                    string desc = string.Empty;
                    if (!string.IsNullOrEmpty(fileDesc))
                        desc = fileDesc;
                    else
                        desc = name;

                    var button = new ButtonHelper()
                    {
                        Name = name,
                        Description = textInfo.ToTitleCase(desc),
                        Image = img
                    };
                    if (!mapWizardEntities.ContainsKey(button))
                        mapWizardEntities.Add(button, fileOn);
                    dlllist.Add(button);
                }
            }
            return dlllist;
        }

        private string GetFileDescription(String _file)
        {
            String _description = string.Empty;
            try
            {
                String fileName = System.IO.Path.GetFileNameWithoutExtension(_file);
                String fileSettings = System.IO.Path.ChangeExtension(_file, dataExt);
                String localizedFile = $"{System.IO.Path.GetDirectoryName(_file)}\\{System.Globalization.CultureInfo.CurrentUICulture.Name}\\{fileName}.{dataExt}";
                if (System.IO.File.Exists(localizedFile))
                    fileSettings = localizedFile;
                _description = System.IO.File.ReadAllText(fileSettings);
            }
            catch (Exception ex)
            {
                _description = System.IO.Path.GetFileNameWithoutExtension(_file);
            }

            return _description;
        }
        private void footerControl_Cancel(object sender, EventArgs e)
        {
            var wnd = this.FindParent<Window>();
            if (wnd == null)
                return;
            wnd.DialogResult = false;
        }
        private void footerControl_Finish(object sender, EventArgs e)
        {
            var wnd = this.FindParent<Window>();
            if (wnd == null)
                return;
            wnd.DialogResult = true;
        }
        private void footerControl_Help(object sender, EventArgs e)
        {
            var helpProvider = ComponentService.UFProjectWizardComponent.projectWizardComponent.HelpProvider;
            if (helpProvider != null)
                helpProvider.OpenDialogHelpPage("ProjectWizardPluginManager", true, true);
        }

        #endregion
    }
}
