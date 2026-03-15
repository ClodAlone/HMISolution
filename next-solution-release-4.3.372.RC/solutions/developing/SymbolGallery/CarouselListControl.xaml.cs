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
using Utilities.WPF;
using System.Collections.ObjectModel;
using Utilities;
using System.IO;
using System.Xml;
using System.Windows.Markup;
using ScreenSettings;
using Utilities.ProgressDialog;
using System.Windows.Threading;
using UFInterfaces;
using System.ComponentModel;
using CommonControls;
using VFS;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;
using System.Xml.Linq;
using System.Dynamic;
using log4net;
using System.Windows.Controls.Primitives;
using ScreenSettings.Entities;
using UFProjectManager.ComponentService;
using System.Runtime.InteropServices.WindowsRuntime;

namespace SymbolGallery
{
    /// <summary>
    /// Interaction logic for CarouselListControl.xaml
    /// </summary>
    public partial class CarouselListControl : UserControl, IDisposable
    {
        readonly Dictionary<String, ObservableCollection<UIElement>> mapLoadedSymbols = new Dictionary<string, ObservableCollection<UIElement>>();
        readonly Dictionary<UIElement, String> mapSymbolXamls = new Dictionary<UIElement, String>();
        readonly Dictionary<UIElement, ScreenDocument> mapSymbolDocument = new Dictionary<UIElement, ScreenDocument>();
        readonly Dictionary<UIElement, String> mapSymbolNames = new Dictionary<UIElement, String>();
        readonly Dictionary<UIElement, String> mapSymbolSettings = new Dictionary<UIElement, String>();
        readonly Dictionary<UIElement, String> mapSymbolCodes = new Dictionary<UIElement, String>();
        readonly Dictionary<UIElement, String> mapSymbolPaths = new Dictionary<UIElement, String>();
        readonly Dictionary<String, String> mapItems = new Dictionary<String, String>();
        readonly Dictionary<UIElement, UIElement> mapItemInGallery = new Dictionary<UIElement, UIElement>();
        readonly Dictionary<UIElement, UIElement> mapItemInCarousel = new Dictionary<UIElement, UIElement>();
        readonly Binding ZoomLevelBinding;
        List<String> listHidden = new List<String>();

        const String settingsExt = ".settings";
        const String codeExt = ".code";
        const String xamlSignature = "xmlns=";
        const String xmlSignature = "<?xml version=";

        static readonly ILog logServer = LogManager.GetLogger(Properties.Resources.SymbolGallery);

        readonly IWorkspace workspace;
        internal String FilePath { get; }

        readonly bool IsDynamic;
        readonly FileSystemProviderBase fileSystemProvider;
        int waitProgressVisibility;

        public SymbolGalleryViewModel ViewModel { get; set; }

        String dataObjectSettings;
        String dataObjectCode;
        String dataObjectPath;

        bool bCopyLink = true;
        bool bCanDrag = true;
        bool bCanEditCode = false;
        bool bCanbeRemoved = false;
        bool bLoaded;

        internal bool CopyLink 
        {
            get
            {
                return bCopyLink;
            }
            set
            {
                bCopyLink = value;
                SetAllLinkImage(Carousel, bCopyLink);
                SetAllLinkImage(Gallery, bCopyLink);
            }
        }

        internal bool CanDrag
        {
            get
            {
                return bCanDrag;
            }
            set
            {
                bCanDrag = value;
            }
        }

        internal bool CanEditCode
        {
            get
            {
                return bCanEditCode;
            }
            set
            {
                bCanEditCode = value;
                btnCode.IsEnabled = bCanEditCode;
            }
        }

        internal bool CanBeRemoved
        {
            get
            {
                return bCanbeRemoved;
            }
            set
            {
                bCanbeRemoved = value;
                btnRemove.IsEnabled = bCanbeRemoved;
            }
        }
       
        public CarouselListControl(IWorkspace w, String filePath, bool isDynamic, SymbolGalleryViewModel viewmodel,Dictionary<String, String> _mapItems, FileSystemProviderBase fp = null)
        {
            mapItems = new Dictionary<string, string>(_mapItems);
            workspace = w;
            FilePath = filePath;
            IsDynamic = isDynamic;
            fileSystemProvider = fp;
            ViewModel = viewmodel;

            InitializeComponent();

            ZoomLevelBinding = new Binding("ZoomLevel") { Source = ViewModel };

            Loaded += (o, e) =>
                {
                    listHidden = LoadCollapsedSymbols();

                    toggleAnimate.IsChecked = false;
                    toggleAnalogAnimate.IsChecked = false;

                    if (!bLoaded)
                    {
                        bLoaded = true;

                        if (IsDynamic)
                        {
                            LoadSymbolsDyn(FilePath, Carousel, Gallery);

                            tabControl.Items.Remove(tabDigital);
                            tabControl.Items.Remove(tabAnalog);
                            tabDigital.Header = Properties.Resources.DynamicTab;
                        }
                        else
                        {
                            LoadSymbols(FilePath, Carousel, Gallery);
                            ManageRemoved(true);
                        }

                        ViewModel.PropertyChanged += ViewModel_PropertyChanged;
                    }
                };
        }

        internal void UpdateTabs()
        {
            ManageRemoved();
            ViewModel_PropertyChanged("SymbolsDisplayMode");
        }

        void ManageRemoved(bool bInit = false)
        {
            bool mustRemoveAnalog = MustRemove(GetAnalogFileName());
            bool mustRemoveDigital = MustRemove(GetDigitalFileName());
            if(bInit)
                CanBeRemoved = CanBeRemoved || (mustRemoveAnalog && mustRemoveDigital);
            if (CanBeRemoved && bInit)
            {
                if (tabControl.Items.Contains(tabDigital))
                    tabControl.Items.Remove(tabDigital);
                if (tabControl.Items.Contains(tabAnalog))
                    tabControl.Items.Remove(tabAnalog);
            }
            else
            {
                if (mustRemoveAnalog)
                {
                    if (tabControl.Items.Contains(tabAnalog))
                        tabControl.Items.Remove(tabAnalog);
                }
                else if (!tabControl.Items.Contains(tabAnalog))
                    tabControl.Items.Add(tabAnalog);

                if (mustRemoveDigital)
                {
                    if (tabControl.Items.Contains(tabDigital))
                        tabControl.Items.Remove(tabDigital);
                }
                else if (!tabControl.Items.Contains(tabDigital))
                    tabControl.Items.Add(tabDigital);
            }

            bDigitalExpanded = false;
            bAnalogExpanded = false;
        }

        bool MustRemove(string FilePath)
        {
            ObservableCollection<UIElement> list;
            if (mapLoadedSymbols.TryGetValue(FilePath, out list))
            {
                return list.Count == 0;
            }

            if(fileSystemProvider != null)
            {
                try
                {
                    var listFiles = fileSystemProvider.GetFiles(new FileManagerFolder(fileSystemProvider, FilePath));
                    var listXaml = (from c in listFiles/*.AsParallel()*/
                                    where System.IO.Path.GetExtension(c.FullName) == SymbolGalleryUI.patternXaml
                                    orderby c.FullName
                                    select c.FullName).ToList();
                    return listXaml.Count == 0;
                }
                catch
                {
                    return true;
                }
            }
            else
            {
                try
                {
                    var listFiles = Directory.GetFiles(FilePath);
                    var listXaml = (from c in listFiles/*.AsParallel()*/
                                    where System.IO.Path.GetExtension(c) == SymbolGalleryUI.patternXaml
                                    orderby c
                                    select c).ToList();
                    return listXaml.Count == 0;
                }
                catch
                {
                    return true;
                }
            }

            return false;
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ViewModel_PropertyChanged(e.PropertyName);
        }

        private void ViewModel_PropertyChanged(string propertyName)
        {
            if (propertyName == "SymbolsDisplayMode")
            {
                if (IsDynamic)
                    LoadSymbolsDyn(FilePath, Carousel, Gallery);
                else
                {
                    LoadSymbols(FilePath, Carousel, Gallery);
                }
                if (bDigitalExpanded)
                {
                    if (IsDynamic)
                        LoadSymbolsDyn(GetDigitalFileName(), CarouselDigital, GalleryDigital);
                    else
                    {
                        LoadSymbols(GetDigitalFileName(), CarouselDigital, GalleryDigital);
                    }
                }
                if (bAnalogExpanded)
                {
                    if (IsDynamic)
                        LoadSymbolsDyn(GetAnalogFileName(), CarouselAnalog, GalleryAnalog);
                    else
                    {
                        LoadSymbols(GetAnalogFileName(), CarouselAnalog, GalleryAnalog);
                    }
                }
            }
            else if (propertyName == "ZoomLevel")
            {
                Carousel.ResetPositions();
                CarouselDigital.ResetPositions();
                CarouselAnalog.ResetPositions();
            }
        }


        private List<string> LoadCollapsedSymbols()
        {
            try
            {
                var doc = XElement.Load(SymbolGalleryUI.GetHiddenPluginFileName());
                var plugin = (from item in doc.Descendants("Plugin")
                              where item.HasAttributes && item.Attribute("SymbolPath") != null
                              select
                              String.Format("{0}\\{1}", Utilities.ApplicationPropertiesHelper.GetProperty<String>("CommonFolder"), item.Attribute("SymbolPath").Value)
                              ).ToList<string>();
                return plugin;
            }
            catch (Exception)
            {
                return new List<String>();
            }
        }

        private void Button_Prev(object sender, RoutedEventArgs e)
        {
            if (!ViewModel.IsCarouselMode)
                return;

            Carousel.SelectPrev();
        }

        private void Button_Next(object sender, RoutedEventArgs e)
        {
            if (!ViewModel.IsCarouselMode)
                return;

            Carousel.SelectNext();
        }

        String GetUniqueFileName(String folderPath, String name)
        {
            String filename = String.Format("{0}\\{1}.xaml", folderPath, name);
            if (fileSystemProvider != null)
            {
                if (!fileSystemProvider.Exists(new FileManagerFile(fileSystemProvider, filename)))
                    return filename;
            }
            else
            {
                if (!File.Exists(filename))
                    return filename;
            }

            int nCount = 0;
            do
            {
                filename = String.Format("{0}\\{1}{2}.xaml", folderPath, name, ++nCount);
            }
            while (fileSystemProvider != null && fileSystemProvider.Exists(new FileManagerFile(fileSystemProvider, filename)) ||
                   fileSystemProvider == null && File.Exists(filename));

            return filename;
        }

        const String pathData = "{0}@{1}?{2}";
        private void FillSymbolsDyn(String FilePath, ObservableCollection<UIElement> listUI, 
                                    CarouselControl carousel, ListBox gallery, String path = null)
        {
            using (var cursor = new WaitCursor())
            {
                var map = ResourceDictionaryExtensions.LoadFromFile(FilePath, typeof(Style));
                var list = (from entry in map where entry.Value is Style orderby entry.Key select entry.Value as Style).ToList();

                list.ForEach(style =>
                {
                    var defFile = String.Format("{0}\\{1}.sgdefault", SymbolGalleryUI.GetStartingDynamicTypeFolder(fileSystemProvider), style.TargetType.Name);
                    FrameworkElement uie;
                    try
                    {
                        uie = ReadXaml(defFile);
                        uie.ClearValue(FrameworkElement.WidthProperty);
                        uie.ClearValue(FrameworkElement.HeightProperty);
                    }
                    catch (Exception ex)
                    {
                        uie = Activator.CreateInstance(style.TargetType) as FrameworkElement;
                    }

                    if (uie != null)
                    {
                        var key = (from c in map where c.Value == style select c.Key).FirstOrDefault();
                        var keyString = key as String;
                        if (STRL.STRL.Settings.ContainsKey(keyString))
                        {
                            if (STRL.STRL.Settings[keyString].Width != null)
                                uie.Width = STRL.STRL.Settings[keyString].Width.Value;
                            if (STRL.STRL.Settings[keyString].Height != null)
                                uie.Height = STRL.STRL.Settings[keyString].Height.Value;
                        }
                        uie.Style = style;
                        uie.Name = keyString;
                        //if (uie is ContentControl)
                        //    (uie as ContentControl).Content = key;
                        mapSymbolNames.Add(uie, keyString);
                        if(mapItems.ContainsKey(keyString))
                            mapSymbolNames[uie] = mapItems[keyString];

                        listUI.Add(uie);
                        if (String.IsNullOrEmpty(path))
                            mapSymbolPaths.Add(uie, String.Format(pathData, defFile, FilePath, keyString));
                        else
                            mapSymbolPaths.Add(uie, String.Format(pathData, defFile, path, keyString));

                        //using (var doc = new ScreenDocument())
                        //{
                        //    doc.ListResources.Add(FilePath);
                        //    doc.SetDynamicEntityStyle(uie, keyString);
                        //    mapSymbolSettings[uie] = doc.ToXml();
                        //}
                    }
                });

                FillList(listUI, carousel, gallery);
            }
        }

        void LoadSymbolsDyn(String FilePath, CarouselControl carousel, ListBox gallery)
        {
            if (string.IsNullOrEmpty(FilePath))
                return;

            ObservableCollection<UIElement> listUI;
            if (mapLoadedSymbols.TryGetValue(FilePath, out listUI))
            {
                FillList(listUI, carousel, gallery);
                return;
            }

            listUI = new ObservableCollection<UIElement>();
            mapLoadedSymbols.Add(FilePath, listUI);

            using (new WaitCursor())
            {
                if (fileSystemProvider != null)
                {
                    var current = Interlocked.Increment(ref waitProgressVisibility);
                    if (current == 1)
                        progressBar.Visibility = Visibility.Visible;

                    var task1 = Task.Factory.StartNew(() =>
                    {
                        var list = new List<String>();
                        if (System.IO.Path.HasExtension(FilePath))
                        {
                            list.Add(FilePath);
                        }
                        else
                        {
                            var listFiles = fileSystemProvider.GetFiles(new FileManagerFolder(fileSystemProvider, FilePath));
                            var listXaml = (from c in listFiles/*.AsParallel()*/
                                            where System.IO.Path.GetExtension(c.FullName) == SymbolGalleryUI.patternXaml
                                            orderby c.FullName
                                            select c.FullName).ToList();
                            list.AddRange(listXaml);
                        }

                        var mapTemp = new Dictionary<String, String>();
                        list.ForEach(file =>
                            {
                                var filename = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName());
                                filename = System.IO.Path.ChangeExtension(filename, "xaml");
                                var xamlData = fileSystemProvider.ReadFile(new FileManagerFile(fileSystemProvider, file));
                                var str = System.Text.Encoding.Unicode.GetString(xamlData);
                                File.WriteAllText(filename, str);
                                mapTemp.Add(filename, file);
                            });

                        return mapTemp;
                    });
                    var task2 = task1.ContinueWith(ret =>
                    {
                        ret.Result.Keys.ToList().ForEach(filename =>
                            {
                                FillSymbolsDyn(filename, listUI, carousel, gallery, ret.Result[filename]);
                                try
                                {
                                    File.Delete(filename);
                                }
                                catch (Exception ex)
                                {

                                }
                            });

                        var currentWait = Interlocked.Decrement(ref waitProgressVisibility);
                        if (currentWait == 0)
                            progressBar.Visibility = System.Windows.Visibility.Collapsed;

                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
                else
                {
                    var current = Interlocked.Increment(ref waitProgressVisibility);
                    if (current == 1)
                        progressBar.Visibility = Visibility.Visible;

                    var task1 = Task.Factory.StartNew(() =>
                    {
                        var list = new List<String>();
                        if (System.IO.Path.HasExtension(FilePath))
                        {
                            list.Add(FilePath);
                        }
                        else
                        {
                            try
                            {
                                var listFiles = Directory.GetFiles(FilePath);
                                var listXaml = (from c in listFiles/*.AsParallel()*/
                                                where System.IO.Path.GetExtension(c) == SymbolGalleryUI.patternXaml
                                                orderby c
                                                select c).ToList();
                                list.AddRange(listXaml);
                            }
                            catch (Exception ex)
                            {
                                
                            }
                        }

                        return list;
                    });
                    var task2 = task1.ContinueWith(ret =>
                    {
                        ret.Result.ForEach(filename =>
                        {
                            if (listHidden.Contains(filename))
                                return;
                            var text = File.ReadAllText(filename);
                            if (text.Contains(xamlSignature))
                                FillSymbolsDyn(filename, listUI, carousel, gallery);
                            else
                            {
                                var filetemp = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName());
                                filetemp = System.IO.Path.ChangeExtension(filetemp, "xaml");
                                File.WriteAllText(filetemp, WPFUtilities.CryptString.CryptString.DecryptString(text));
                                try
                                {
                                    FillSymbolsDyn(filetemp, listUI, carousel, gallery, filename);
                                }
                                catch (Exception ex)
                                {
                                    SymbolGallery.LibraryHelper.ShowMessage(ex.Message,true);
                                }
                                try
                                {
                                    File.Delete(filetemp);
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                        });

                        var currentWait = Interlocked.Decrement(ref waitProgressVisibility);
                        if (currentWait == 0)
                            progressBar.Visibility = System.Windows.Visibility.Collapsed;

                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        }

        FrameworkElement ReadXaml(String filePath)
        {
            if (fileSystemProvider != null)
            {
                var xamlData = fileSystemProvider.ReadFile(new FileManagerFile(fileSystemProvider, filePath));
                var str = System.Text.Encoding.Unicode.GetString(xamlData);
                return str.ReadUIElement() as FrameworkElement;
                //using (var ms = new MemoryStream(xamlData))
                //{
                //    return XamlReader.Load(ms) as FrameworkElement;
                //}
            }
            else
            {
                var ret = File.ReadAllText(filePath);
                if (ret.Contains(xamlSignature))
                    return ret.ReadUIElement() as FrameworkElement;
                return WPFUtilities.CryptString.CryptString.DecryptString(ret).ReadUIElement() as FrameworkElement;
                //using (XmlReader xmlReader = XmlReader.Create(filePath))
                //{
                //    return XamlReader.Load(xmlReader) as FrameworkElement;
                //}
            }
        }

        String GetXaml(String filePath)
        {
            if (fileSystemProvider != null)
            {
                var xamlData = fileSystemProvider.ReadFile(new FileManagerFile(fileSystemProvider, filePath));
                return System.Text.Encoding.Unicode.GetString(xamlData);
            }
            else
            {
                var ret = File.ReadAllText(filePath);
                if (ret.Contains(xamlSignature))
                    return ret;
                return WPFUtilities.CryptString.CryptString.DecryptString(ret);
            }
        }

        internal static String GetSymbolElement(String sourceSymbolProvider, String sourceSymbolPath, String relativePath)
        {
            return STRL.STRL.GetSymbolElement(sourceSymbolProvider, sourceSymbolPath, relativePath);
        }

        internal static String GetSymbolSettings(String sourceSymbolProvider, String sourceSymbolPath, String relativePath)
        {
            return STRL.STRL.GetSymbolSettings(sourceSymbolProvider, sourceSymbolPath, relativePath);
        }

        internal static String GetSymbolCode(String sourceSymbolProvider, String sourceSymbolPath, String relativePath)
        {
            return STRL.STRL.GetSymbolCode(sourceSymbolProvider, sourceSymbolPath, relativePath);
        }

        class DataRead
        {
            public String fileXaml;
            public String XamlCode;
            public String fileSettings;
            public String fileCode;
            public String settings;
            public String code;
        }

        private void LoadUIElement(DataRead dataRead, String FilePath)
        {
            if (listHidden.Contains(dataRead.fileXaml))
                return;

            var obj = dataRead.XamlCode.ReadUIElement() as FrameworkElement;
            if (obj == null)
                return;

            if (bCanbeRemoved)
            {
                obj.ClearValue(FrameworkElement.WidthProperty);
                obj.ClearValue(FrameworkElement.HeightProperty);
            }

            // NameScope.SetNameScope(obj, new NameScope());
            if (Utilities.WPF.XmlHelper.CheckProblematicXamlWriter(obj, String.Empty))
            {
                Utilities.WPF.XmlHelper.CheckProblematicXamlWriter(obj, dataRead.XamlCode);
            }
            Utilities.WPF.DependencyObjectExtensions.RegisterName(obj, obj as FrameworkElement, true, false);

            if (!String.IsNullOrEmpty(dataRead.settings))
            {
                mapSymbolSettings[obj] = dataRead.settings;
                var document = dataRead.settings.FromXml<ScreenDocument>();
                {
                    document.FullPath = FilePath;
                    if (obj is Panel)
                        document.RestoreProblematicXamlWriter(obj as Panel, obj.Name);
                    else
                    {
                        obj.GetChildrenOfType<Panel>().ToList()
                        .ForEach(panel =>
                        {
                            document.RestoreProblematicXamlWriter(panel, obj.Name, obj, true);
                        });
                    }

                    document.IsInLibrary = true;
                    document.LoadResources(obj);
                    document.RefreshEntityStyleBinding(obj);
                    document.LoadRepositoryItem(obj, obj, obj.Name);
                    mapSymbolDocument[obj] = document;
                }
            }
            if (!String.IsNullOrEmpty(dataRead.code))
            {
                mapSymbolCodes[obj] = dataRead.code;
            }
            mapSymbolPaths[obj] = dataRead.fileXaml;
            mapSymbolXamls[obj] = dataRead.XamlCode;

            // Page and WindowStartupLocation etc are not supported
            List<Page> listPages = new List<Page>(Utilities.WPF.DependencyObjectExtensions.GetChildrenOfType<Page>(obj));
            if (obj is Page || listPages.Count > 0)
            {
                Page page = obj is Page ? obj as Page : listPages[0];
                obj = page.Content as FrameworkElement;
                page.Content = null;
                page = null;
                if (obj is Canvas)
                {
                    Canvas c = obj as Canvas;
                    if (c.Children.Count == 1 && c.Children[0] is Canvas)
                    {
                        Canvas cv = c.Children[0] as Canvas;
                        var v = new Viewbox();
                        v.Stretch = System.Windows.Media.Stretch.Fill;
                        var grid = new Grid();
                        while (cv.Children.Count > 0)
                        {
                            UIElement uie = cv.Children[0];
                            cv.Children.RemoveAt(0);
                            grid.Children.Add(uie);
                        }
                        grid.Clip = cv.Clip;
                        cv.Children.Clear();
                        v.Child = grid;

                        obj = v;
                        // obj = c.Children[0] as FrameworkElement;
                        c.Children.Clear();
                        c = null;
                    }
                }
            }
            else
            {
                List<Window> listWindow = new List<Window>(Utilities.WPF.DependencyObjectExtensions.GetChildrenOfType<Window>(obj));
                if (obj is Window || listWindow.Count > 0)
                    throw new NotSupportedException();
            }

            obj.IsHitTestVisible = false;
            // obj.ContextMenu = TryFindResource("cm") as ContextMenu;
            // obj.CacheMode = new BitmapCache() { EnableClearType = true };
            //GalleryItem item = new GalleryItem
            //{
            //    Content = obj,
            //    Caption = Path.GetFileNameWithoutExtension(file.Name),
            //};
            //item.Description = String.Format("{0}\\{1}", FilePath, item.Caption);
            //group.Items.Add(item);

            mapSymbolNames[obj] = System.IO.Path.GetFileNameWithoutExtension(dataRead.fileXaml);
            if (mapItems.ContainsKey(System.IO.Path.GetFileNameWithoutExtension(dataRead.fileXaml)))
                mapSymbolNames[obj] = mapItems[System.IO.Path.GetFileNameWithoutExtension(dataRead.fileXaml)];

            mapLoadedSymbols[FilePath].Add(obj);
        }

        void LoadSymbols(String FilePath, CarouselControl carousel, ListBox gallery)
        {
            if (string.IsNullOrEmpty(FilePath))
                return;

            ObservableCollection<UIElement> list;
            if (mapLoadedSymbols.TryGetValue(FilePath, out list))
            {
                FillList(list, carousel, gallery);
                return;
            }

            var current = Interlocked.Increment(ref waitProgressVisibility);
            if (current == 1)
                progressBar.Visibility = Visibility.Visible;

            list = new ObservableCollection<UIElement>();
            mapLoadedSymbols.Add(FilePath, list);

            FillList(list, carousel, gallery);

            var task1 = Task.Factory.StartNew(() =>
            {
                var listXaml = new List<String>();
                if (fileSystemProvider != null)
                {
                    var folder = new FileManagerFolder(fileSystemProvider, FilePath);
                    if (fileSystemProvider.Exists(folder))
                    {
                        var listFiles = fileSystemProvider.GetFiles(folder);
                        listXaml = (from c in listFiles/*.AsParallel()*/
                                    where System.IO.Path.GetExtension(c.FullName) == ".xaml"
                                    orderby c.FullName
                                    select c.FullName).ToList();

                    }
                }
                else
                {
                    if (Directory.Exists(FilePath))
                    {
                        string[] directoryGetFiles = Directory.GetFiles(FilePath, "*.xaml");
                        listXaml = (from c in directoryGetFiles orderby c select c).ToList();
                    }
                }

                var retList = new List<DataRead>();
                foreach (var fileXaml in listXaml)
                {
                    var dataRead = new DataRead();
                    retList.Add(dataRead);

                    String fileSettings = fileXaml + settingsExt;
                    String fileCode = fileXaml + codeExt;
                    String settings = null;
                    String code = null;

                    dataRead.fileXaml = fileXaml;
                    if (fileSystemProvider != null)
                    {
                        var file = new FileManagerFile(fileSystemProvider, fileSettings);
                        if (fileSystemProvider.Exists(file))
                        {
                            var data = fileSystemProvider.ReadFile(file);
                            settings = DecryptData(data); // System.Text.Encoding.Unicode.GetString(data); 
                        }
                        file = new FileManagerFile(fileSystemProvider, fileCode);
                        if (fileSystemProvider.Exists(file))
                        {
                            var data = fileSystemProvider.ReadFile(file);
                            code = DecryptData(data); //System.Text.Encoding.Unicode.GetString(data);
                        }

                        var xamlData = fileSystemProvider.ReadFile(new FileManagerFile(fileSystemProvider, fileXaml));
                        var str = DecryptData(xamlData); //System.Text.Encoding.Unicode.GetString(xamlData);

                        dataRead.XamlCode = str;
                    }
                    else
                    {
                        if (File.Exists(fileSettings))
                            settings = File.ReadAllText(fileSettings);
                        if (File.Exists(fileCode))
                            code = File.ReadAllText(fileCode);

                        dataRead.XamlCode = File.ReadAllText(fileXaml);
                    }

                    if (!dataRead.XamlCode.Contains(xamlSignature))
                        dataRead.XamlCode = WPFUtilities.CryptString.CryptString.DecryptString(dataRead.XamlCode);

                    if (!String.IsNullOrEmpty(settings))
                    {
                        try
                        {
                            if (!settings.Contains(xmlSignature))
                                settings = WPFUtilities.CryptString.CryptString.DecryptString(settings);
                        }
                        catch { }

                        using (var document = settings.FromXml<ScreenDocument>())
                        {
                            document.FullPath = FilePath;
                            document.ListAssemblies.ForEach(s =>
                            {
                                try
                                {
                                    Assembly.Load(s);
                                }
                                catch (Exception ex)
                                {

                                }
                            });
                        }
                    }

                    if (!String.IsNullOrEmpty(code))
                    {
                        try
                        {
                            if (!code.Contains(xmlSignature))
                                code = WPFUtilities.CryptString.CryptString.DecryptString(code);
                        }
                        catch { }
                    }

                    dataRead.fileSettings = fileSettings;
                    dataRead.fileCode = fileCode;
                    dataRead.settings = settings;
                    dataRead.code = code;
                }

                return retList;
            });
            var task2 = task1.ContinueWith(ret =>
            {
                ret.Result.ForEach(dataRead =>
                    {
                        Dispatcher.InvokeIfRequired(() =>
                            {
                                try
                                {
                                    LoadUIElement(dataRead, FilePath);
                                }
                                catch(Exception ex)
                                {
                                    logServer.Error(string.Format("Unable to load Symbols: {0} - {1}", dataRead.fileXaml, ex)); 
                                }
                            });
                    });

            });
            var task3 = task2.ContinueWith(ret =>
            {
                var currentWait = Interlocked.Decrement(ref waitProgressVisibility);
                if (currentWait == 0)
                    progressBar.Visibility = System.Windows.Visibility.Collapsed;

                if (ViewModel.IsCarouselMode)
                {
                    Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                        {
                            carousel.ReInitialize();
                            carousel.Refresh();
                        });
                }

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private String DecryptData(byte[] data, string str ="", string strUTF8="")
        {
            var bException = false;
            string ret = String.Empty;

            ret = System.Text.Encoding.Unicode.GetString(data);

            if (!ret.Contains(xamlSignature) && !ret.Contains(xmlSignature))
            {
                try
                {
                    ret = WPFUtilities.CryptString.CryptString.DecryptString(ret);

                    return ret;
                }
                catch (Exception ex)
                {
                    bException = true;
                }
            }

            if (bException)
            {
                ret = Encoding.UTF8.GetString(data);
                if (!ret.Contains(xamlSignature) && !ret.Contains(xmlSignature))
                {
                    ret = WPFUtilities.CryptString.CryptString.DecryptString(ret);
                }

                return ret;
            }

            //if (!str.Contains(xamlSignature))
            //{
            //    if (!strUTF8.Contains(xamlSignature))
            //    {
            //        try
            //        {
            //            return WPFUtilities.CryptString.CryptString.DecryptString(strUTF8);
            //        }
            //        catch { }                    
            //    }
            //    else
            //    {
            //        return strUTF8;
            //    }

            //    try
            //    {
            //        return WPFUtilities.CryptString.CryptString.DecryptString(str);
            //    }
            //    catch { }
            //}
            //else
            //{
            //    return str;
            //}

            return String.Empty;
        }


        private void FillList(ObservableCollection<UIElement> list, CarouselControl carousel, ListBox gallery)
        {
            using (new WaitCursor())
            {
                CleanCarousel(carousel);
                CleanGallery(gallery);

                if (ViewModel.IsCarouselMode)
                {
                    carousel.BeginInit();
                    foreach (var uie in list)
                        AddCarouselItem(carousel, uie);
                    carousel.EndInit();
                    Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                    {
                        carousel.ReInitialize();
                        carousel.Refresh();
                        if (list.Count > 0)
                            carousel.SelectElement(list[0] as FrameworkElement);
                    });
                    if(carousel == Carousel)
                        list.CollectionChanged += list_CarouselCollectionChanged;
                    else if (carousel == CarouselDigital)
                        list.CollectionChanged += list_CarouselDigitalCollectionChanged;
                    else if (carousel == CarouselAnalog)
                        list.CollectionChanged += list_CarouselAnalogCollectionChanged;

                        /*(o, e) =>
                        {
                            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                            {
                                carousel.BeginInit();
                                foreach (UIElement uie in e.NewItems)
                                {
                                    AddCarouselItem(carousel, uie);
                                    carousel.SelectElement(uie as FrameworkElement);
                                }
                                carousel.EndInit();
                            }

                            btnCode.IsEnabled = bCanEditCode;
                            btnRemove.IsEnabled = bCanbeRemoved;
                        };*/
                }
                else if (ViewModel.IsGridMode)
                {
                    gallery.BeginInit();
                    foreach (var uie in list)
                        AddGalleryItem(gallery, uie);
                    gallery.EndInit();
                    gallery.SelectedIndex = 0;

                    if (gallery == Gallery)
                        list.CollectionChanged += list_GridCollectionChanged;
                    else if (gallery == GalleryDigital)
                        list.CollectionChanged += list_GridDigitalCollectionChanged;
                    else if (gallery == GalleryAnalog)
                        list.CollectionChanged += list_GridAnalogCollectionChanged;
                    /*list.CollectionChanged += (o, e) =>
                    {
                        if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                        {
                            gallery.BeginInit();
                            foreach (UIElement uie in e.NewItems)
                            {
                                AddGalleryItem(gallery, uie);
                                gallery.Refresh();
                                // gallery.SelectedItem = mapItemInGallery[uie];
                                // gallery.ScrollIntoView(mapItemInGallery[uie]);
                            }
                            gallery.EndInit();

                            btnCode.IsEnabled = bCanEditCode;
                            btnRemove.IsEnabled = bCanbeRemoved;
                        }
                    };*/
                }

                if (list.Count == 0)
                {
                    btnCode.IsEnabled = false;
                    btnRemove.IsEnabled = false;
                }
            }
        }

        private void list_CarouselDigitalCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            AddCarouselItems(CarouselDigital, (from UIElement uie in e.NewItems select uie).ToList());
        }

        private void list_CarouselAnalogCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            AddCarouselItems(CarouselAnalog, (from UIElement uie in e.NewItems select uie).ToList());
        }

        private void list_CarouselCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                AddCarouselItems(Carousel, (from UIElement uie in e.NewItems select uie).ToList());
            }
        }

        void AddCarouselItems(CarouselControl carousel, List<UIElement> newItems)
        {
            carousel.BeginInit();
            foreach (UIElement uie in newItems)
            {
                AddCarouselItem(carousel, uie);
                carousel.SelectElement(uie as FrameworkElement);
            }
            carousel.EndInit();

            btnCode.IsEnabled = bCanEditCode;
            btnRemove.IsEnabled = bCanbeRemoved;
        }

        private void list_GridDigitalCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                AddGalleryItems(GalleryDigital, (from UIElement uie in e.NewItems select uie).ToList());
            }
        }
        private void list_GridAnalogCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                AddGalleryItems(GalleryAnalog, (from UIElement uie in e.NewItems select uie).ToList());
            }
        }
        private void list_GridCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                AddGalleryItems(Gallery, (from UIElement uie in e.NewItems select uie).ToList());
            }
        }
        void AddGalleryItems(ListBox gallery, List<UIElement> newItems)
        {
            gallery.BeginInit();
            foreach (UIElement uie in newItems)
            {
                AddGalleryItem(gallery, uie);
                gallery.Refresh();
                // gallery.SelectedItem = mapItemInGallery[uie];
                // gallery.ScrollIntoView(mapItemInGallery[uie]);
            }
            gallery.EndInit();

            btnCode.IsEnabled = bCanEditCode;
            btnRemove.IsEnabled = bCanbeRemoved;
        }

        private void CleanCarousel(CarouselControl carousel)
        {
            foreach (var c in carousel.Children)
            {
                SetLinkImage(((c as ContentControl).Content as Grid), false);
                BindingOperations.ClearAllBindings(((c as ContentControl).Content as Grid).Children?[0] as Viewbox);
                var btn = (((c as ContentControl).Content as Grid).Children?[0] as Viewbox).Child as Button;
                UIElement uie = btn.Content as UIElement;
                if (uie is IDisposable)
                    try { (uie as IDisposable).Dispose(); } catch(Exception ex) { logServer.Error($"{Properties.Resources.ErrorRemovingElement} {ex.Message}"); }
                uie = null;
                ((((c as ContentControl).Content as Grid).Children?[0] as Viewbox).Child as Button).Content = null;
                (((c as ContentControl).Content as Grid).Children?[0] as Viewbox).Child = null;
                ((c as ContentControl).Content as Grid).Children.Clear();
                (c as ContentControl).Content = null;
            }
            carousel.Children.Clear();
        }

        private void CleanGallery(ListBox gallery)
        {
            foreach (var c in gallery.Items)
            {
                SetLinkImage(((c as ContentControl).Content as Grid), false);
                BindingOperations.ClearAllBindings(((c as ContentControl).Content as Grid).Children?[0] as Viewbox);
                var btn = (((c as ContentControl).Content as Grid).Children?[0] as Viewbox).Child as Button;
                UIElement uie = btn.Content as UIElement;
                if (uie is IDisposable)
                    try { (uie as IDisposable).Dispose(); } catch (Exception ex) { logServer.Error($"{Properties.Resources.ErrorRemovingElement} {ex.Message}"); }
                uie = null;
                ((((c as ContentControl).Content as Grid).Children?[0] as Viewbox).Child as Button).Content = null;
                (((c as ContentControl).Content as Grid).Children?[0] as Viewbox).Child = null;
                ((c as ContentControl).Content as Grid).Children.Clear();
                (c as ContentControl).Content = null;
            }
            gallery.Items.Clear();
        }

        void SetAllLinkImage(CarouselControl carousel, bool bSet)
        {
            foreach (var uie in carousel.Children)
            {
                var content = uie as ContentControl;
                var stack = content.Content as Grid;
                SetLinkImage(stack, bSet);
            }
        }

        void SetAllLinkImage(ListBox gallery, bool bSet)
        {
            foreach (var uie in gallery.Items)
            {
                var content = uie as ContentControl;
                var stack = content.Content as Grid;
                SetLinkImage(stack, bSet);
            }
        }

        Image CreateLinkImage()
        {
            DataTemplate item = TryFindResource("CopyLinkImage") as DataTemplate;
            var ret = item.LoadContent() as Image;
            return ret;
        }

        void SetLinkImage(Grid grid, bool bSet)
        {
            return;
            if (bSet)
            {
                if (!(grid.Children[grid.Children.Count - 1] is Image))
                    grid.Children.Add(CreateLinkImage());
            }
            else
            {
                if (grid.Children[grid.Children.Count - 1] is Image)
                    grid.Children.RemoveAt(grid.Children.Count - 1);
            }
        }

        private ContentControl AddCarouselItem(CarouselControl carousel, UIElement uie)
        {
            DataTemplate item = TryFindResource("ItemTemplate") as DataTemplate;
            var view = item.LoadContent() as Viewbox;
            var btn = view.Child as Button;
            btn.Content = uie;

            if (mapSymbolNames.ContainsKey(uie))
                btn.ToolTip = mapSymbolNames[uie];


            //btn.SetBinding(Button.WidthProperty, ZoomLevelBinding);
            //btn.SetBinding(Button.HeightProperty, ZoomLevelBinding);
            view.SetBinding(Viewbox.WidthProperty, ZoomLevelBinding);
            view.SetBinding(Viewbox.HeightProperty, ZoomLevelBinding);

            var stack = new Grid();
            stack.Children.Add(view);
            /*
            stack.Children.Add(new TextBlock()
            {
                Text = mapSymbolNames[uie],
                HorizontalAlignment = HorizontalAlignment.Center
            });
            */

            ContentControl contentControl = new ContentControl() { Content = stack };
            carousel.Children.Add(contentControl);
            if (mapItemInCarousel.ContainsKey(uie))
                mapItemInCarousel.Remove(uie);
            mapItemInCarousel.Add(uie, contentControl);

            SetLinkImage(stack, bCopyLink);

            return contentControl;
        }

        private ContentControl AddGalleryItem(ListBox gallery, UIElement uie)
        {
            DataTemplate item = TryFindResource("ItemTemplate") as DataTemplate;
            var view = item.LoadContent() as Viewbox;
            var btn = view.Child as Button;
            btn.Content = uie;
            btn.Effect = null; // no effect in listbox mode

            if (mapSymbolNames.ContainsKey(uie))
                btn.ToolTip = mapSymbolNames[uie];

            //btn.SetBinding(Button.WidthProperty, ZoomLevelBinding);
            //btn.SetBinding(Button.HeightProperty, ZoomLevelBinding);
            view.SetBinding(Viewbox.WidthProperty, ZoomLevelBinding);
            view.SetBinding(Viewbox.HeightProperty, ZoomLevelBinding);

            var stack = new Grid();
            stack.Children.Add(view);
            /*
            stack.Children.Add(new TextBlock()
            {
                Text = mapSymbolNames[uie],
                HorizontalAlignment = HorizontalAlignment.Center
            });
            */
            ContentControl contentControl = new ContentControl() { Content = stack };
            gallery.Items.Add(contentControl);
            if (mapItemInGallery.ContainsKey(uie))
                mapItemInGallery.Remove(uie);
            mapItemInGallery.Add(uie, contentControl);

            SetLinkImage(stack, bCopyLink);

            return contentControl;
        }

        UIElement GetSelected(CarouselControl carousel, ListBox gallery)
        {
            ContentControl contentSelected = null;
            if (ViewModel.IsCarouselMode)
                contentSelected = carousel.CurrentlySelected as ContentControl;
            else if (ViewModel.IsGridMode)
                contentSelected = gallery.SelectedItem as ContentControl;
            
            if (contentSelected == null)
                return null;
            var stack = contentSelected.Content as Grid;
            return ((stack.Children[0] as Viewbox).Child as Button).Content as UIElement;
        }

        Point startPoint;
        DataObject dataObject;
        bool bMouseDown;
        private void Button_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Button btn = sender as Button;
            var uie = btn.Content as UIElement;
            if (CanDrag)
            {
                startPoint = e.GetPosition(this);

                // uie.CacheMode = new BitmapCache() { EnableClearType = true };
                String xamlData = null;
                if (mapSymbolXamls.ContainsKey(uie))
                    xamlData = mapSymbolXamls[uie];
                else
                {
                    try
                    {
                        xamlData = uie.XamlWriterFormatted();
                    }
                    catch (Exception ex)
                    {
                        SymbolGallery.LibraryHelper.ShowMessage(ex.Message, true);
                        return;
                    }
                }

                bMouseDown = true;
                // uie.CacheMode = null;
                if (bCopyLink)
                    xamlData = String.Empty;
                dataObject = new DataObject(DataFormats.Xaml, xamlData);
            }

            if (!mapSymbolSettings.TryGetValue(uie, out dataObjectSettings))
                dataObjectSettings = null;
            if (!mapSymbolCodes.TryGetValue(uie, out dataObjectCode))
                dataObjectCode = null;
            if (!mapSymbolPaths.TryGetValue(uie, out dataObjectPath))
                dataObjectPath = null;

            if (mapItemInCarousel.ContainsKey(uie) &&
                Carousel.Children.Contains(mapItemInCarousel[uie]))
            {
                Carousel.SelectElement(mapItemInCarousel[uie] as FrameworkElement);
            }
            if (mapItemInCarousel.ContainsKey(uie) &&
                CarouselDigital.Children.Contains(mapItemInCarousel[uie]))
            {
                CarouselDigital.SelectElement(mapItemInCarousel[uie] as FrameworkElement);
            }
            if (mapItemInCarousel.ContainsKey(uie) &&
                CarouselAnalog.Children.Contains(mapItemInCarousel[uie]))
            {
                CarouselAnalog.SelectElement(mapItemInCarousel[uie] as FrameworkElement);
            }

            if (mapItemInGallery.ContainsKey(uie) &&
                Gallery.Items.Contains(mapItemInGallery[uie]))
            {
                Gallery.SelectedItem = mapItemInGallery[uie];
                Gallery.ScrollIntoView(mapItemInGallery[uie]);
            }
            if (mapItemInGallery.ContainsKey(uie) &&
                GalleryDigital.Items.Contains(mapItemInGallery[uie]))
            {
                GalleryDigital.SelectedItem = mapItemInGallery[uie];
                GalleryDigital.ScrollIntoView(mapItemInGallery[uie]);
            }
            if (mapItemInGallery.ContainsKey(uie) &&
                GalleryAnalog.Items.Contains(mapItemInGallery[uie]))
            {
                GalleryAnalog.SelectedItem = mapItemInGallery[uie];
                GalleryAnalog.ScrollIntoView(mapItemInGallery[uie]);
            }

            // e.Handled = true;
        }

        private void Button_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var wnd = this.FindParent<Window>();
            if (wnd == null)
                return;
            var ret = typeof(Window).GetField("_showingAsDialog", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(wnd);
            if (ret != null && ret is bool && (bool)ret == true)
                wnd.DialogResult = true;
        }

        internal String GetCurrentDropSettings()
        {
            if (dataObjectSettings == null)
            {
                var selected = GetSelected(Carousel, Gallery);
                if (selected != null && !mapSymbolSettings.TryGetValue(selected, out dataObjectSettings))
                    dataObjectSettings = null;
            }

            return dataObjectSettings;
        }

        internal String GetCurrentSourceSymbolProvider()
        {
            if (fileSystemProvider != null)
            {
                var ret = (fileSystemProvider as DataSourceFileSystemProvider).ConnectionString;
                return WPFUtilities.CryptString.CryptString.EncryptString(ret);
            }
            else
            {
                return String.Empty;
            }
        }

        internal String GetCurrentSourceSymbolPath()
        {
            if (dataObjectPath == null)
            {
                var selected = GetSelected(Carousel, Gallery);
                if (selected != null && !mapSymbolPaths.TryGetValue(selected, out dataObjectPath))
                    dataObjectPath = null;
            }

            return dataObjectPath;
        }

        internal String GetCurrentSourceSymbolCode()
        {
            if (dataObjectCode == null)
            {
                var selected = GetSelected(Carousel, Gallery);
                if (selected != null && !mapSymbolCodes.TryGetValue(selected, out dataObjectCode))
                    dataObjectCode = null;
            }
            return dataObjectCode;
        }

        private void Button_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (dataObject != null && e.LeftButton == MouseButtonState.Pressed && bMouseDown && CanDrag)
            {
                Point currentPoint = e.GetPosition(this);
                if (Math.Abs(currentPoint.X - startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(currentPoint.Y - startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    bMouseDown = false;
                    try
                    {
                        DragDrop.DoDragDrop(this, dataObject, DragDropEffects.Copy | DragDropEffects.Move);
                    }
                    catch (Exception ex)
                    {
                        SymbolGallery.LibraryHelper.ShowMessage(ex.Message, true);
                    }
                    dataObject = null;
                }

                e.Handled = true;
            }
        }

        internal static bool UpdateSymbolToLibrary(string xaml, string settings, string provider, string path)
        {
            var connectionString = WPFUtilities.CryptString.CryptString.DecryptString(provider);
            var symbolPath = path;

            String fileSettings = symbolPath + settingsExt;
            if (String.IsNullOrEmpty(connectionString))
            {
                if (File.Exists(symbolPath))
                    File.Delete(symbolPath);
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(symbolPath));
                File.WriteAllText(symbolPath, WPFUtilities.CryptString.CryptString.EncryptString(xaml));

                if (!String.IsNullOrEmpty(settings))
                {
                    if (File.Exists(fileSettings))
                        File.Delete(fileSettings);
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fileSettings));
                    File.WriteAllText(fileSettings, WPFUtilities.CryptString.CryptString.EncryptString(settings));
                }
            }
            else
            {
                using (var fileSystemProvider = new DataSourceFileSystemProvider("")
                {
                    ConnectionString = connectionString
                })
                {
                    var file = new FileManagerFile(fileSystemProvider, symbolPath);
                    if (fileSystemProvider.Exists(file))
                        fileSystemProvider.DeleteFile(file);
                    fileSystemProvider.UploadFile(null, symbolPath,
                        System.Text.Encoding.Unicode.GetBytes(xaml));

                    if (!String.IsNullOrEmpty(settings))
                    {
                        var filesettings = new FileManagerFile(fileSystemProvider, fileSettings);
                        if (fileSystemProvider.Exists(filesettings))
                            fileSystemProvider.DeleteFile(filesettings);
                        fileSystemProvider.UploadFile(null, fileSettings,
                            System.Text.Encoding.Unicode.GetBytes(settings));
                    }
                }
            }

            return true;
        }

        internal bool AddSymbolToLibrary(string folder, string name, string xaml, string settings)
        {
            FrameworkElement obj = xaml.ReadUIElement() as FrameworkElement;
            obj.ClearValue(FrameworkElement.WidthProperty);
            obj.ClearValue(FrameworkElement.HeightProperty);
            mapSymbolXamls[obj] = xaml;
            //if (Utilities.WPF.XmlHelper.CheckProblematicXamlWriter(obj, String.Empty))
            //{
            //    Utilities.WPF.XmlHelper.CheckProblematicXamlWriter(obj, xaml);
            //}
            Utilities.WPF.DependencyObjectExtensions.RegisterName(obj, obj as FrameworkElement, true, false);

            //String startingFolder = SymbolGalleryUI.GetStartingFolder();
            //String folderPath = String.Format("{0}\\{1}", startingFolder, folder);

            if (!mapLoadedSymbols.ContainsKey(folder))
                mapLoadedSymbols.Add(folder, new ObservableCollection<UIElement>());
            String fullPath = GetUniqueFileName(folder, name);

            mapSymbolNames[obj] = System.IO.Path.GetFileNameWithoutExtension(fullPath);
            if (mapItems.ContainsKey(System.IO.Path.GetFileNameWithoutExtension(fullPath)))
                mapSymbolNames[obj] = mapItems[System.IO.Path.GetFileNameWithoutExtension(fullPath)];

            mapLoadedSymbols[folder].Add(obj);

            if (fileSystemProvider != null)
                fileSystemProvider.UploadFile(null, fullPath,
                    System.Text.Encoding.Unicode.GetBytes(xaml));
            else
            {
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fullPath));
                File.WriteAllText(fullPath, WPFUtilities.CryptString.CryptString.EncryptString(xaml));
            }
            mapSymbolPaths[obj] = fullPath;

            if (!String.IsNullOrEmpty(settings))
            {
                String settingsFilename = fullPath + settingsExt;
                if (fileSystemProvider != null)
                    fileSystemProvider.UploadFile(null, settingsFilename,
                        System.Text.Encoding.Unicode.GetBytes(settings));
                else
                    File.WriteAllText(settingsFilename, WPFUtilities.CryptString.CryptString.EncryptString(settings));
                mapSymbolSettings[obj] = settings;

                var document = settings.FromXml<ScreenDocument>();
                {
                    document.FullPath = FilePath;
                    if (obj is Panel)
                        document.RestoreProblematicXamlWriter(obj as Panel, obj.Name, obj);
                    else
                    {
                        obj.GetChildrenOfType<Panel>().ToList()
                        .ForEach(panel =>
                        {
                            document.RestoreProblematicXamlWriter(panel, obj.Name, obj, true);
                        });
                    }

                    document.IsInLibrary = true;
                    document.LoadResources(obj);
                    document.RefreshEntityStyleBinding(obj);
                    document.LoadRepositoryItem(obj, obj, obj.Name);

                    var list = document.GetListTypeDefinitionName();
                    var listAdded = new List<String>();
                    foreach (var type in list)
                    {
                        if (listAdded.Contains(type))
                            continue;
                        listAdded.Add(type);
                        var startingFolder = SymbolGalleryUI.GetStartingTypeFolder(fileSystemProvider);
                        var folderPath = String.Format("{0}\\{1}", startingFolder, DependencyObjectExtensions.AdaptName(type));

                        fullPath = GetUniqueFileName(folderPath, name);

                        if (fileSystemProvider != null)
                            fileSystemProvider.UploadFile(null, fullPath,
                                System.Text.Encoding.Unicode.GetBytes(xaml));
                        else
                        {
                            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fullPath));
                            File.WriteAllText(fullPath, WPFUtilities.CryptString.CryptString.EncryptString(xaml));
                        }

                        settingsFilename = fullPath + settingsExt;
                        if (fileSystemProvider != null)
                            fileSystemProvider.UploadFile(null, settingsFilename,
                                System.Text.Encoding.Unicode.GetBytes(settings));
                        else
                            File.WriteAllText(settingsFilename, WPFUtilities.CryptString.CryptString.EncryptString(settings));
                    }
                }
                mapSymbolDocument[obj] = document;
            }

            return true;
        }

        private void RemoveElement(UIElement selected, bool bRemoveFiles = true)
        {
            if (selected == null || !CanBeRemoved)
                return;

            if (mapSymbolPaths.ContainsKey(selected))
            {
                if (mapItemInCarousel.ContainsKey(selected) &&
                    Carousel.Children.Contains(mapItemInCarousel[selected]))
                {
                    Carousel.Children.Remove(mapItemInCarousel[selected]);
                    if (Carousel.Children.Count == 0)
                        btnRemove.IsEnabled = false;
                    else
                        Carousel.SelectPrev();
                }
                if (mapItemInGallery.ContainsKey(selected) &&
                    Gallery.Items.Contains(mapItemInGallery[selected]))
                {
                    Gallery.Items.Remove(mapItemInGallery[selected]);
                    if (Gallery.Items.Count == 0)
                        btnRemove.IsEnabled = false;
                    else
                        Gallery.SelectedIndex = 0;
                }

                var file = mapSymbolPaths[selected];
                mapSymbolPaths.Remove(selected);

                mapLoadedSymbols.Keys.ToList().ForEach(key =>
                    {
                        if (mapLoadedSymbols[key].Contains(selected))
                        {
                            mapLoadedSymbols[key].Remove(selected);
                        }
                    });

                if (mapSymbolXamls.ContainsKey(selected))
                    mapSymbolXamls.Remove(selected);
                if (mapSymbolNames.ContainsKey(selected))
                    mapSymbolNames.Remove(selected);
                if (mapSymbolSettings.ContainsKey(selected))
                    mapSymbolSettings.Remove(selected);
                if (mapSymbolDocument.ContainsKey(selected))
                {
                    mapSymbolDocument[selected].Dispose();
                    mapSymbolDocument.Remove(selected);
                }
                if (mapSymbolCodes.ContainsKey(selected))
                    mapSymbolCodes.Remove(selected);
                if (mapItemInCarousel.ContainsKey(selected))
                    mapItemInCarousel.Remove(selected);
                if (mapItemInGallery.ContainsKey(selected))
                    mapItemInGallery.Remove(selected);

                if (bRemoveFiles)
                {
                    try
                    {
                        if (fileSystemProvider != null)
                            fileSystemProvider.DeleteFile(new FileManagerFile(fileSystemProvider, file));
                        else
                            File.Delete(file);
                    }
                    catch (Exception ex)
                    {

                    }

                    var fileCode = file + codeExt;
                    try
                    {
                        if (fileSystemProvider != null)
                            fileSystemProvider.DeleteFile(new FileManagerFile(fileSystemProvider, fileCode));
                        else
                            File.Delete(fileCode);
                    }
                    catch (Exception ex)
                    {

                    }

                    var fileSettings = file + settingsExt;
                    try
                    {
                        if (fileSystemProvider != null)
                            fileSystemProvider.DeleteFile(new FileManagerFile(fileSystemProvider, fileSettings));
                        else
                            File.Delete(fileSettings);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }

        private void EditCode(UIElement selected)
        {
            if (selected == null || !CanEditCode)
                return;

            String Code;
            if (mapSymbolCodes.ContainsKey(selected))
                Code = mapSymbolCodes[selected];
            else
                Code = String.Empty;

            ScreenEntity entity = null;
            if (mapSymbolDocument.ContainsKey(selected))
            {
                try
                {
                    // using (var document = mapSymbolSettings[selected].FromXml<ScreenDocument>())
                    var document = mapSymbolDocument[selected];
                    {
                        if (document.MapScreenEntities.ContainsKey(mapSymbolNames[selected]))
                            entity = document.MapScreenEntities[mapSymbolNames[selected]];
                    }
                }
                catch
                { }
            }

            using (var droppingCode = new DroppingCode(Code, mapSymbolNames[selected], selected, entity))
            {
                if (droppingCode.Edit(this.FindParent<Window>()))
                {
                    var file = mapSymbolPaths[selected];
                    var fileCode = file + codeExt;
                    if (Code != droppingCode.Code)
                    {
                        if (fileSystemProvider != null)
                            fileSystemProvider.UploadFile(null, fileCode,
                                System.Text.Encoding.Unicode.GetBytes(droppingCode.Code));
                        else
                            File.WriteAllText(fileCode, droppingCode.Code);
                        mapSymbolCodes[selected] = droppingCode.Code;
                    }
                }
            }
        }

        private void cm_Opened(object sender, RoutedEventArgs e)
        {
            var ctx = sender as ContextMenu;
            if (ctx == null)
                return;
            var menuCode = ctx.Items[0] as MenuItem;
            var menuRemove = ctx.Items[1] as MenuItem;
            if (menuCode == null || menuRemove == null)
                return;

            var selected = GetSelected(Carousel, Gallery);
            menuCode.IsEnabled = selected != null && CanEditCode;
            menuRemove.IsEnabled = selected != null && CanBeRemoved;
        }

        private void Button_Code(object sender, RoutedEventArgs e)
        {
            var selected = GetSelected(Carousel, Gallery);
            EditCode(selected);
        }

        private void Button_Remove(object sender, RoutedEventArgs e)
        {
            var selected = GetSelected(Carousel, Gallery);
            if (LibraryHelper.ShowYesNo(Properties.Resources.RemoveSymbolConfirm) == MessageBoxResult.Yes)
                RemoveElement(selected);
        }

        private void ButtonDigital_Code(object sender, RoutedEventArgs e)
        {
            var selected = GetSelected(CarouselDigital, GalleryDigital);
            EditCode(selected);
        }

        private void ButtonAnalog_Code(object sender, RoutedEventArgs e)
        {
            var selected = GetSelected(CarouselAnalog, GalleryAnalog);
            EditCode(selected);
        }

        private void ButtonDigital_Prev(object sender, RoutedEventArgs e)
        {
            if (!ViewModel.IsCarouselMode)
                return;

            CarouselDigital.SelectPrev();
        }

        private void ButtonDigital_Next(object sender, RoutedEventArgs e)
        {
            if (!ViewModel.IsCarouselMode)
                return;

            CarouselDigital.SelectNext();
        }

        private void ButtonAnalog_Prev(object sender, RoutedEventArgs e)
        {
            if (!ViewModel.IsCarouselMode)
                return;

            CarouselAnalog.SelectPrev();
        }

        private void ButtonAnalog_Next(object sender, RoutedEventArgs e)
        {
            if (!ViewModel.IsCarouselMode)
                return;

            CarouselAnalog.SelectNext();
        }

        String GetDigitalFileName()
        {
            var search = String.Format("{0}\\", SymbolGalleryUI.GetStartingFolder(fileSystemProvider));
            var replace = String.Format("{0}\\{1}\\", SymbolGalleryUI.GetStartingDynamicTypeFolder(fileSystemProvider), "CheckBoxControl");

            var fileDigital = FilePath.Replace(search, replace);
            return fileDigital;
        }

        String GetAnalogFileName()
        {
            var search = String.Format("{0}\\", SymbolGalleryUI.GetStartingFolder(fileSystemProvider));
            var replace = String.Format("{0}\\{1}\\", SymbolGalleryUI.GetStartingDynamicTypeFolder(fileSystemProvider), "ProgressbarControl");

            var fileAnalog = FilePath.Replace(search, replace);
            return fileAnalog;
        }

        bool bDigitalExpanded;
        bool bAnalogExpanded;
        private void tabControl_SelectionChanged(object sender, DevExpress.Xpf.Core.TabControlSelectionChangedEventArgs e)
        {
            if (e.NewSelectedItem == tabDigital)
            {
                if (!bDigitalExpanded)
                {
                    bDigitalExpanded = true;
                    LoadSymbolsDyn(GetDigitalFileName(), CarouselDigital, GalleryDigital);
                }
            }
            else if (e.NewSelectedItem == tabAnalog)
            {
                if (!bAnalogExpanded)
                {
                    bAnalogExpanded = true;
                    LoadSymbolsDyn(GetAnalogFileName(), CarouselAnalog, GalleryAnalog);
                }
            }

        }

        public void Dispose()
        {
            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
            CleanCarousel(Carousel);
            Carousel.Dispose();
            CleanCarousel(CarouselDigital);
            CarouselDigital.Dispose();
            CleanCarousel(CarouselAnalog);
            CarouselAnalog.Dispose();
            CleanGallery(Gallery);
            CleanGallery(GalleryDigital);
            CleanGallery(GalleryAnalog);
            mapSymbolNames.Clear();
            mapSymbolSettings.Clear();
            mapSymbolDocument.Values.ToList().ForEach(d => d.Dispose());
            mapSymbolDocument.Clear();
            mapSymbolCodes.Clear();
            mapSymbolPaths.Clear();
            mapSymbolXamls.Clear();
            mapItemInCarousel.Values.ToList().ForEach(item =>
            {
                (from c in item.GetVisualChildrenOfType<UIElement>()
                 select c).ToList().ForEach(child =>
                 {
                     if (child is IDisposable)
                         (child as IDisposable).Dispose();
                 });
                if (item is IDisposable)
                    (item as IDisposable).Dispose();
            });
            mapItemInCarousel.Clear();
            mapSymbolDocument.Values.ToList().ForEach(d => d.Dispose());
            mapItemInGallery.Values.ToList().ForEach(item =>
            {
                (from c in item.GetVisualChildrenOfType<UIElement>()
                 select c).ToList().ForEach(child =>
                 {
                     if (child is IDisposable)
                         (child as IDisposable).Dispose();
                 });
                if (item is IDisposable)
                    (item as IDisposable).Dispose();
            });
            mapItemInGallery.Clear();
            mapItems.Clear();
            mapLoadedSymbols.Keys.ToList().ForEach(key =>
            {
                mapLoadedSymbols[key].CollectionChanged -= list_CarouselCollectionChanged;
                mapLoadedSymbols[key].CollectionChanged -= list_CarouselDigitalCollectionChanged;
                mapLoadedSymbols[key].CollectionChanged -= list_CarouselAnalogCollectionChanged;
                mapLoadedSymbols[key].CollectionChanged -= list_GridCollectionChanged;
                mapLoadedSymbols[key].CollectionChanged -= list_GridDigitalCollectionChanged;
                mapLoadedSymbols[key].CollectionChanged -= list_GridAnalogCollectionChanged;

                mapLoadedSymbols[key].ToList().ForEach(item =>
                {
                    var obj = item as FrameworkElement;
                    Utilities.WPF.DependencyObjectExtensions.UnregisterName(obj, obj as FrameworkElement, false);
                    (from c in item.GetVisualChildrenOfType<UIElement>()
                     select c).ToList().ForEach(child =>
                     {
                         if (child is IDisposable)
                             (child as IDisposable).Dispose();
                     });
                    if (item is IDisposable)
                        (item as IDisposable).Dispose();
                });
                mapLoadedSymbols[key].Clear();
            });
            mapLoadedSymbols.Clear();

            listHidden.Clear();

            DependencyObjectExtensions.CleanChildrenOfTypeCache();
        }

        internal void ReloadElement(string path)
        {
            var uies = (from c in mapSymbolPaths.Keys where mapSymbolPaths[c] == path select c).ToList();
            if (uies.Count == 0)
                return;
            var uie = uies[0];
            RemoveElement(uie, false);

            try
            {
                String fileSettings = path + settingsExt;
                String fileCode = path + codeExt;
                String settings = null;
                String code = null;

                var dataRead = new DataRead();
                dataRead.fileXaml = path;
                if (fileSystemProvider != null)
                {
                    var file = new FileManagerFile(fileSystemProvider, fileSettings);
                    if (fileSystemProvider.Exists(file))
                    {
                        var data = fileSystemProvider.ReadFile(file);
                        settings = System.Text.Encoding.Unicode.GetString(data);
                    }
                    file = new FileManagerFile(fileSystemProvider, fileCode);
                    if (fileSystemProvider.Exists(file))
                    {
                        var data = fileSystemProvider.ReadFile(file);
                        code = System.Text.Encoding.Unicode.GetString(data);
                    }

                    var xamlData = fileSystemProvider.ReadFile(new FileManagerFile(fileSystemProvider, path));
                    var str = System.Text.Encoding.Unicode.GetString(xamlData);

                    dataRead.XamlCode = str;
                }
                else
                {
                    if (File.Exists(fileSettings))
                        settings = File.ReadAllText(fileSettings);
                    if (File.Exists(fileCode))
                        code = File.ReadAllText(fileCode);

                    dataRead.XamlCode = File.ReadAllText(path);
                    if (!dataRead.XamlCode.Contains(xamlSignature))
                        dataRead.XamlCode = WPFUtilities.CryptString.CryptString.DecryptString(dataRead.XamlCode);
                }

                if (!String.IsNullOrEmpty(settings))
                {
                    if (!settings.Contains(xmlSignature))
                        settings = WPFUtilities.CryptString.CryptString.DecryptString(settings);

                    using (var document = settings.FromXml<ScreenDocument>())
                    {
                        document.FullPath = FilePath;
                        document.ListAssemblies.ForEach(s =>
                        {
                            try
                            {
                                Assembly.Load(s);
                            }
                            catch (Exception ex)
                            {

                            }
                        });
                    }
                }

                dataRead.fileSettings = fileSettings;
                dataRead.fileCode = fileCode;
                dataRead.settings = settings;
                dataRead.code = code;

                LoadUIElement(dataRead, FilePath);
            }
            catch (Exception ex)
            {
                logServer.Error(string.Format("Unable to load Symbols: {0} - {1}", path, ex));
            }
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            (from c in GalleryDigital.GetVisualChildrenOfType<CheckBox>()
                select c).ToList().ForEach(child =>
                {
                    child.IsChecked = (bool)toggleAnimate.IsChecked;
                });
            (from c in CarouselDigital.GetVisualChildrenOfType<CheckBox>()
             select c).ToList().ForEach(child =>
             {
                 child.IsChecked = (bool)toggleAnimate.IsChecked;
             });
        }
        private void ToggleAnalogButton_Click(object sender, RoutedEventArgs e)
        {
            (from c in GalleryAnalog.GetVisualChildrenOfType<ProgressBar>()
             select c).ToList().ForEach(child =>
             {
                 child.Value = (bool)toggleAnalogAnimate.IsChecked ? 50 : 0;
             });
            (from c in CarouselAnalog.GetVisualChildrenOfType<ProgressBar>()
             select c).ToList().ForEach(child =>
             {
                 child.Value = (bool)toggleAnalogAnimate.IsChecked ? 50 : 0;
             });
        }
    }
}
