using AmazedSaint.Elastic;
using DocumentManager.ComponentService;
using OPCUAViewModel;
using ScreenManager.ComponentService;
using ScreenSettings;
using ScreenSettings.Entities;
using SymbolGallery;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;
using Toolbox.ComponentService;
using UFProjectManager.ComponentService;
using Utilities;
using Utilities.WPF;
using VFS;

namespace MoviconNextBuilder
{
    public struct ScreenElement
    {
        
        /// <summary>   Graphic element. </summary>
        public UIElement Element;
        /// <summary>   Entity element. </summary>
        public ScreenEntity Entity;
    }
    public class ScreenContainer : IDisposable
    {
        #region Ctor
        public ScreenContainer(IDocument parent, UFProjectManagerComponent manager)
        {
            projectdoc = (UFProjectManager.UFProjectDocument)parent;
            projectman = manager;
        }
        #endregion Ctor

        #region data
        UFProjectManager.UFProjectDocument projectdoc;
        UFProjectManagerComponent projectman;
        Dictionary<Uri, ScreenDocument> mapDocuments = new Dictionary<Uri, ScreenDocument>();
        Dictionary<Uri, Canvas> mapCanvas = new Dictionary<Uri, Canvas>();
        #endregion data

        #region Properties
        static ScreenManagerComponent screenManagerComponent = new ScreenManagerComponent();
        static public ScreenManagerComponent ScreensManagerComponent
        {
            get { return screenManagerComponent; }
        }

        static ToolboxComponent toolboxComponent = new ToolboxComponent();
        static public ToolboxComponent ToolboxComponent
        {
            get { return toolboxComponent; }
        }

        static SymbolGalleryComponent symbolgalleryComponent = new SymbolGalleryComponent();
        static public SymbolGalleryComponent SymbolGalleryComponent
        {
            get { return symbolgalleryComponent; }
        }
        #endregion

        #region Methods

        public bool AddFolder(string folder)
        {
            var path = projectdoc.GetResourcePath("ScreenManager");
            var destPath = string.Format("{0}{1}", path.OriginalString, folder);

            if (projectdoc.fileSystemProviderBase != null)
            {
                projectdoc.fileSystemProviderBase.CreateFolder(null, destPath);
                return projectdoc.fileSystemProviderBase.Exists(new VFS.FileManagerFolder(projectdoc.fileSystemProviderBase, destPath));
            }
            else
            {
                try
                {
                    return System.IO.Directory.CreateDirectory(destPath) != null;
                }
                catch (Exception ex)
                { }
            }
            
            return false;
        }
        public ScreenDocument AddScreen(string name, string modelname, String subfolder = null)
        {
            var path = projectdoc.GetResourcePath("ScreenManager", subfolder);
            // modelname: file name of the screen model
            // Es. C:\ProgramData\Progea\Movicon.NExT.3.1\NewScreenTypes\HMI\Full HD 1920x1080\Full_HD_HMI.xaml
            // also valid are: Full_HD_HMI.xaml and Full_HD_HMI
            if (!File.Exists(modelname))
            {
                String startingPath = NewScreenTypesFolder();
                if(!modelname.ToLower().EndsWith(".xaml"))
                    modelname += ".xaml";
                modelname = SearchXamlItem(modelname, startingPath);
            }
            
            var uscr = ScreensManagerComponent.CreateNewDocument(path, projectdoc, name, modelname);
            if(uscr != null)
            {
                projectdoc.disableProtection();
                //get the screen object...
                var doc = ScreenDocument.FromFile(uscr.GetPathString(), projectdoc);
                if (doc != null)
                {
                    if (doc.Parent == null)
                        doc.Parent = projectdoc;
                    mapDocuments[uscr] = doc;
                }
                projectdoc.enableProtection();
                return doc;
            }
            return null;
        }

        public Uri GetScreenUri(string name, string subfolder = null, bool relative = false)
        {
            var path = projectdoc.GetResourcePath("ScreenManager", subfolder);
            if(path != null)
            {
                //crerare l'Uri corretto per il sinottico, folders eventuali compresi
                Uri tUri = ProjectBuilder.GetUriFromName(path, name, ScreensManagerComponent, projectdoc, bCheckExists: true);
                if (tUri != null)
                    return (relative ? projectdoc.MakeRelativeUri(tUri) : tUri);
            }
            
            return null;
        }
        public ScreenDocument GetScreen(string name, String subfolder = null)
        {
            var path = projectdoc.GetResourcePath("ScreenManager", subfolder);
            //crerare l'Uri corretto per il sinottico, folders eventuali compresi
            Uri uri = ProjectBuilder.GetUriFromName(path, name, ScreensManagerComponent, projectdoc, bCheckExists: true);
            if (uri == null)
                return null;
            Uri uscr = new Uri(uri.GetPathString(), UriKind.RelativeOrAbsolute);
            if (uscr != null)
            {
                if (mapDocuments.ContainsKey(uscr))
                    return mapDocuments[uscr];
                else
                {
                    var doc = ScreenDocument.FromFile(uscr.GetPathString(), projectdoc);
                    if (doc != null)
                    { 
                        if (doc.Parent == null)
                            doc.Parent = projectdoc;
                        mapDocuments[uscr] = doc;
                    }
                    return doc;
                }
            }
                
            return null;
        }
        public bool DeleteScreen(string name, String subfolder = null)
        {
            var path = projectdoc.GetResourcePath("ScreenManager", subfolder);
            //crerare l'Uri corretto per il sinottico, folders eventuali compresi
            Uri uscr = ProjectBuilder.GetUriFromName(path, name, ScreensManagerComponent, projectdoc, bCheckExists: true);
            if (uscr != null)
            {
                ScreensManagerComponent.Delete(uscr, projectdoc);
                if (mapDocuments.ContainsKey(uscr))
                    mapDocuments.Remove(uscr);
                return true;
            }

            return false;
        }

        public string NewScreenTypesFolder()
        {
            var startingPath = ApplicationPropertiesHelper.GetProperty<String>("CommonFolder");

            Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string mainversion = String.Format("{0}.{1}", fvi.FileMajorPart, fvi.FileMinorPart);

            var folder = String.Format("{0}.{1}\\NewScreenTypes\\", startingPath, mainversion);
            return folder;
        }

        public string ToolboxFolder()
        {
            var startingPath = ApplicationPropertiesHelper.GetProperty<String>("CommonFolder");

            Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string mainversion = String.Format("{0}.{1}", fvi.FileMajorPart, fvi.FileMinorPart);
            
            var folder = String.Format("{0}.{2}\\{1}\\", startingPath, Properties.Settings.Default.ToolBoxFolder, mainversion);
            return folder;
        }

        public string SearchXamlItem(string itemname, string path)
        {
            if(Directory.Exists(path))
            {
                string item = string.Format(@"\{0}",itemname);
                if (!item.ToLower().EndsWith(".xaml"))
                    item += ".xaml";
                string[] directoryGetFiles = Directory.GetFiles(path, "*.xaml");
                if(directoryGetFiles.Length > 0)
                {
                    int i = directoryGetFiles.ToList().FindIndex(x => x.EndsWith(item));
                    if(i != -1)
                        return directoryGetFiles[i];
                }
                var list = Directory.GetDirectories(path);
                foreach(var folder in list)
                {
                    var found = SearchXamlItem(itemname, folder);
                    if (found != null)
                        return found;
                }
            }
            return null;
        }

        public ScreenElement AddSymbol(string screen, string item, string name, double x, double y, double width, double height, String subfolder = null, OPCUAEntityReference tag = null, bool linkonly = true)
        {
            ScreenElement ret;
            ret.Element = null;
            ret.Entity = null;
            var screendoc = GetScreen(screen, subfolder);
            if (screendoc != null)
            {
                if (screendoc.MapScreenEntities.ContainsKey(name))
                    return ret;

                String xamlData = String.Empty;

                if (!File.Exists(item))
                {
                    String startingPath = String.Format("{0}\\Symbols", ApplicationPropertiesHelper.GetProperty<String>("CommonFolder"));
                    //Search folders for the requested file name
                    if (!item.ToLower().EndsWith(".xaml"))
                        item += ".xaml";
                    item = SearchXamlItem(item, startingPath);
                }
                if (item == null || !item.ToLower().EndsWith(".xaml"))
                    return ret;

                //Lookup for index.dat

                bool libProtected = true;

                if (!linkonly)
                {
                    string idxPath = string.Empty;
                    var searchPath = item;
                    do
                    {
                        var idx = searchPath.LastIndexOf("\\");
                        if (idx > -1)
                        {
                            searchPath = searchPath.Substring(0, idx);
                            if (File.Exists(string.Format("{0}\\index.dat", searchPath)))
                            {
                                idxPath = string.Format("{0}\\index.dat", searchPath);
                                break;
                            }
                        }
                        else
                            break;
                    } while (searchPath.Length > 0);

                    var t1 = String.Empty;
                    if (projectdoc.fileSystemProviderBase != null)
                    {
                        var fileManagerFile = new FileManagerFile(projectdoc.fileSystemProviderBase, idxPath);
                        if (!projectdoc.fileSystemProviderBase.Exists(fileManagerFile))
                        {
                            //protected
                        }
                        else
                        {
                            var xaml = projectdoc.fileSystemProviderBase.ReadFile(fileManagerFile);
                            t1 = WPFUtilities.CryptString.CryptString.DecryptString(System.Text.Encoding.Unicode.GetString(xaml));
                        }
                    }
                    else
                    {
                        if (!File.Exists(idxPath))
                        {
                            //protected
                        }
                        else
                        {
                            try
                            {
                                File.SetAttributes(idxPath, FileAttributes.Hidden | FileAttributes.System);
                            }
                            catch (Exception ex)
                            {
                            }

                            t1 = File.ReadAllText(idxPath);
                        }
                    }

                    try
                    {
                        var exp = XElement.Parse(WPFUtilities.CryptString.CryptString.DecryptString(t1)).ToElastic();
                        //ValidateIndexFile(ret);
                        LibraryBrowser.ValidateIndexFile(exp);
                        //check key presence
                        if (exp.Key is String && String.IsNullOrEmpty(exp.Key as String))
                            libProtected = false;

                    }
                    catch (Exception ex)
                    {
                        //protected
                    }
                }
                

                if (!linkonly && libProtected)
                {
                    ret.Element = null;
                    ret.Entity = null;
                    return ret;
                }


                var all = File.ReadAllText(item);
                xamlData = WPFUtilities.CryptString.CryptString.DecryptString(all);//decrypt
                var path = WPFUtilities.CryptString.CryptString.EncryptString(item);//encript

                if(linkonly)
                    xamlData = String.Empty;
                if (!String.IsNullOrEmpty(xamlData))
                {
                    //merge
                    UIElement element = null;
                    try
                    {
                        element = xamlData.ReadUIElement();
                    }
                    catch (Exception ex)
                    {

                    }

                    if (element != null)
                    {
                        element.IsHitTestVisible = true;

                        Canvas.SetTop(element, y);
                        Canvas.SetLeft(element, x);

                        var upath = projectdoc.GetResourcePath("ScreenManager", subfolder);
                        //crerare l'Uri corretto per il sinottico, folders eventuali compresi
                        Uri uri = ProjectBuilder.GetUriFromName(upath, screen, ScreensManagerComponent, projectdoc, bCheckExists: true);
                        if (uri == null)
                            return ret;
                        Uri uscr = new Uri(uri.GetPathString(), UriKind.RelativeOrAbsolute);
                        if (uscr == null)
                            return ret;
                        Canvas cv;
                        if (mapCanvas.ContainsKey(uscr))
                        {
                            cv = mapCanvas[uscr];
                        }
                        else
                            cv = screendoc.GetCurrentXamlDocument();

                        cv.Children.Add(element);

                        var settings = SymbolGalleryComponent.GetCurrentDropSettings(item);
                        try
                        {
                            settings = WPFUtilities.CryptString.CryptString.DecryptString(settings);
                        }
                        catch
                        {
                            settings = string.Empty;
                        }
                        var provider = String.Empty;

                        
                        if (element is FrameworkElement)
                        {
                            var fe = element as FrameworkElement;
                            
                            var doc = settings.FromXml<ScreenDocument>();
                            var listEntities = doc.MapScreenEntities.Keys.ToList();
                            ScreenObjectsSettingsMap sm = new ScreenObjectsSettingsMap();

                            string newname = string.Empty;
                            while (listEntities.Count > 0)
                            {
                                var elName = listEntities[0];
                                listEntities.Remove(elName);
                                newname = elName;
                                if (elName.Substring(0, fe.Name.Length) == fe.Name)
                                    newname = string.Format("{0}{1}", name, 
                                        (elName.Length > fe.Name.Length ? elName.Substring(fe.Name.Length) : string.Empty));
                                sm.Add(newname, doc.MapScreenEntities[elName]);
                            }
                            if(sm.Count > 0)
                            {
                                doc.MapScreenEntities.Clear();
                                foreach (var i in sm)
                                    doc.MapScreenEntities.Add(i.Key, i.Value);
                            }
                            settings = doc.ToXml();
                            fe.Name = name;

                            bool bForceName = !String.IsNullOrEmpty(settings) ||
                                        !String.IsNullOrEmpty(provider) ||
                                        !String.IsNullOrEmpty(path);

                            var map = Utilities.WPF.DependencyObjectExtensions.RegisterName(cv, fe, bForceName, true);
                            var lret = screendoc.MergeDocument(settings, map);


                            screendoc.SetSourceProviderPath(fe, provider, path, true);


                            if (Utilities.WPF.XmlHelper.CheckProblematicXamlWriter(fe, xamlData))
                            {
                                screendoc.SetProblematicXaml(fe, xamlData);
                            }

                            if (element is Panel)
                                screendoc.RestoreProblematicXamlWriter(cv, (element as Panel).Name, bUpdate: true);
                            else
                            {
                                element.GetChildrenOfType<Panel>().ToList()
                                    .ForEach(panel => screendoc.RestoreProblematicXamlWriter(cv,
                                        (element as FrameworkElement).Name, bUpdate: true));
                            }
                            screendoc.CleanProblematicXamlBags();

                            foreach (var lname in lret)
                            {
                                screendoc.GetListInners(lname).ForEach(I =>
                                {
                                    UIElement uie = screendoc.FindInnerControl(cv, I);
                                    if (uie == null)
                                        return;

                                    var entity = screendoc.MapScreenEntities[I];
                                    entity.Entity = uie;
                                    entity.Document = screendoc;
                                });
                            }

                            fe.Width = width;
                            fe.Height = height;

                            mapCanvas[uscr] = cv;

                            ret.Element = element;

                            if (screendoc.MapScreenEntities.ContainsKey(fe.Name))
                            {
                                ret.Entity = screendoc.MapScreenEntities[fe.Name];
                                if (tag != null)
                                    ret.Entity.OnDropReference(tag);
                            }
                            screendoc.NeedsSave = true;
                        }

                        return ret;
                    }
                }
                else
                {
                    var element = new ContentControl();
                    element.Name = name;
                    Canvas.SetLeft(element, x);
                    Canvas.SetTop(element, y);

                    var settings = SymbolGalleryComponent.GetCurrentDropSettings(item);
                    try
                    {
                        settings = WPFUtilities.CryptString.CryptString.DecryptString(settings);
                    }
                    catch
                    {
                        settings = string.Empty;
                    }
                    
                    var provider = String.Empty;

                    var upath = projectdoc.GetResourcePath("ScreenManager", subfolder);
                    //crerare l'Uri corretto per il sinottico, folders eventuali compresi
                    Uri uri = ProjectBuilder.GetUriFromName(upath, screen, ScreensManagerComponent, projectdoc, bCheckExists: true);
                    if (uri == null)
                        return ret;
                    Uri uscr = new Uri(uri.GetPathString(), UriKind.RelativeOrAbsolute);
                    if (uscr == null)
                        return ret;
                    Canvas cv;
                    if (mapCanvas.ContainsKey(uscr))
                    {
                        cv = mapCanvas[uscr];
                    }
                    else
                        cv = screendoc.GetCurrentXamlDocument();

                    cv.Children.Add(element);

                    bool bForceName = !String.IsNullOrEmpty(settings) ||
                                          !String.IsNullOrEmpty(provider) ||
                                          !String.IsNullOrEmpty(path);
                    var map = Utilities.WPF.DependencyObjectExtensions.RegisterName(cv, element, bForceName, false);
                    var fe = element as FrameworkElement;
                    var lret = screendoc.MergeDocument(settings, map, fe.Name);


                    screendoc.SetSourceProviderPath(element, provider, path);

                    element.Width = width;
                    element.Height = height;

                    mapCanvas[uscr] = cv;

                    ret.Element = element;

                    if (screendoc.MapScreenEntities.ContainsKey(name))
                    {
                        ret.Entity = screendoc.MapScreenEntities[name];
                        if (tag != null)
                            ret.Entity.OnDropReference(tag);
                    }
                    screendoc.NeedsSave = true;
                    return ret;
                }

            }
            return ret;
        }
        public bool SetScreenEntity(string screen, string name, OPCUAEntityReference tag, String subfolder = null)
        {
            var screendoc = GetScreen(screen, subfolder);
            if (screendoc != null)
            {
                if (screendoc.MapScreenEntities.ContainsKey(name))
                {
                    var entity = screendoc.MapScreenEntities[name];
                    return entity.OnDropReference(tag);
                }
            }
            return false;
        }
        public ScreenElement AddToolboxItem(string screen, string item, string name, double x, double y, double width, double height, String subfolder = null, OPCUAEntityReference tag = null)
        {
            ScreenElement ret;
            ret.Element = null;
            ret.Entity = null;

            var screendoc = GetScreen(screen, subfolder);
            if(screendoc != null)
            {
                if (screendoc.MapScreenEntities.ContainsKey(name))
                    return ret;
                
                String xamlData = String.Empty;
                
                /*
                 * item is an existing file name, with absolute path -> go on.
                 * item is a simbol name, look for the first occurrence...
                 */
                if(!File.Exists(item))
                {
                    var toolboxpath = ToolboxFolder();
                    //Search folders for the requested file name
                    if (!item.ToLower().EndsWith(".xaml"))
                        item += ".xaml";
                    item = SearchXamlItem(item, toolboxpath);
                }
                if (item == null || !item.ToLower().EndsWith(".xaml"))
                    return ret;

                xamlData = ToolboxComponent.GetCodeFromHash(item);
                var path = ToolboxComponent.GetCurrentSourceSymbolPath(item);

                if (!String.IsNullOrEmpty(path))
                {
                    var element = new ContentControl();
                    element.Name = name;
                    Canvas.SetLeft(element, x);
                    Canvas.SetTop(element, y);
                    

                    var settings = ToolboxComponent.GetCurrentDropSettings(item);
                    var provider = ToolboxComponent.GetCurrentSourceSymbolProvider(item);

                    var upath = projectdoc.GetResourcePath("ScreenManager", subfolder);
                    //crerare l'Uri corretto per il sinottico, folders eventuali compresi
                    Uri uri = ProjectBuilder.GetUriFromName(upath, screen, ScreensManagerComponent, projectdoc, bCheckExists: true);
                    if (uri == null)
                        return ret;
                    Uri uscr = new Uri(uri.GetPathString(), UriKind.RelativeOrAbsolute);
                    if (uscr == null)
                        return ret;
                    Canvas cv;
                    if (mapCanvas.ContainsKey(uscr))
                    {
                        cv = mapCanvas[uscr];
                    }
                    else
                        cv = screendoc.GetCurrentXamlDocument();

                    
                    cv.Children.Add(element);

                    bool bForceName = !String.IsNullOrEmpty(settings) ||
                                           !String.IsNullOrEmpty(provider) ||
                                           !String.IsNullOrEmpty(path);

                    

                    var map = Utilities.WPF.DependencyObjectExtensions.RegisterName(cv, element, bForceName);

                    screendoc.SetSourceProviderPath(element, provider, path);

                    try
                    {
                        screendoc.LoadRepositoryItem(element, cv, element.Name, bSetSize: true);
                    }
                    catch (Exception ex)
                    {
                        return ret;
                    }

                    screendoc.GetListElementsUsingProviderPath(provider, path).ForEach(ename =>
                    {
                        if (ename != element.Name)
                        {
                            var el = screendoc.FindInnerControl(cv, name);
                            // var el = MainSurface.FindName(name) as FrameworkElement;
                            if (el != null)
                                screendoc.ResolveEntityBrushAndPen(cv, el);
                        }
                    });

                    element.Width = width;
                    element.Height = height;

                    mapCanvas[uscr] = cv;

                    ret.Element = element;
                    if (screendoc.MapScreenEntities.ContainsKey(name))
                    {
                        ret.Entity = screendoc.MapScreenEntities[name];
                        if (tag != null)
                            ret.Entity.OnDropReference(tag);
                    }
                    screendoc.NeedsSave = true;
                    return ret;
                }
                else
                {
                    FrameworkElement element = xamlData.ReadUIElement() as FrameworkElement;

                    element.Name = name;

                    element.IsHitTestVisible = true;

                    Canvas.SetTop(element, y);
                    Canvas.SetLeft(element, x);

                    var upath = projectdoc.GetResourcePath("ScreenManager", subfolder);
                    //crerare l'Uri corretto per il sinottico, folders eventuali compresi
                    Uri uri = ProjectBuilder.GetUriFromName(upath, screen, ScreensManagerComponent, projectdoc, bCheckExists: true);
                    if (uri == null)
                        return ret;
                    Uri uscr = new Uri(uri.GetPathString(), UriKind.RelativeOrAbsolute);
                    if (uscr == null)
                        return ret;
                    Canvas cv;
                    if (mapCanvas.ContainsKey(uscr))
                    {
                        cv = mapCanvas[uscr];
                    }
                    else
                        cv = screendoc.GetCurrentXamlDocument();
                    
                    cv.Children.Add(element);

                    if (element is FrameworkElement)
                    {
                        if (Utilities.WPF.XmlHelper.CheckProblematicXamlWriter(element, xamlData))
                        {
                            Utilities.WPF.DependencyObjectExtensions.RegisterName(cv, element as FrameworkElement, true, true);
                            screendoc.SetProblematicXaml(element, xamlData);
                        }
                        else
                            Utilities.WPF.DependencyObjectExtensions.RegisterName(cv, element as FrameworkElement, true);
                    }

                    var listAssemblies = Utilities.WPF.XmlHelper.GetAssemblyListInXaml(xamlData);
                    screendoc.AddListAssembly(listAssemblies);

                    element.Width = width;
                    element.Height = height;

                    mapCanvas[uscr] = cv;

                    ret.Element = element;
                    if (screendoc.MapScreenEntities.ContainsKey(name))
                    {
                        ret.Entity = screendoc.MapScreenEntities[name];
                        if (tag != null)
                            ret.Entity.OnDropReference(tag);
                    }

                    screendoc.NeedsSave = true;
                    return ret;
                }
            }
            return ret;
        }
        public bool AddGeoData(string name, double latitude, double longitude, OPCUAEntityReference longitudeTag = null, OPCUAEntityReference latitudeTag = null, String subfolder = null, bool overwrite = false)
        {
            var path = projectdoc.GetResourcePath("ScreenManager", subfolder);
            Uri uscr = ProjectBuilder.GetUriFromName(path, name, ScreensManagerComponent, projectdoc, bCheckExists: true);
            if (uscr != null)
            {
                if (projectdoc.ControllerDataExist(uscr.GetUrlDecodedUri()) && !overwrite)
                    return false;
                var cd = projectdoc.GetControllerData(uscr.GetUrlDecodedUri());
                if(cd != null)
                {
                    cd.Longitude = longitude;
                    cd.Latitude = latitude;
                    if (longitudeTag != null)
                        cd.LongitudeTag = longitudeTag;
                    if (latitudeTag != null)
                        cd.LatitudeTag = latitudeTag;
                    projectdoc.NeedsSave = true;
                    return true;
                }
            }
            return false;
        }

        public bool DeleteElement(string screen, string elementname, string subfolder = null)
        {
            var screendoc = GetScreen(screen, subfolder);
            if (screendoc != null)
            {
                if (!screendoc.MapScreenEntities.ContainsKey(elementname))
                    return false;

                var upath = projectdoc.GetResourcePath("ScreenManager", subfolder);
                //crerare l'Uri corretto per il sinottico, folders eventuali compresi
                Uri uri = ProjectBuilder.GetUriFromName(upath, screen, ScreensManagerComponent, projectdoc, bCheckExists: true);
                if (uri == null)
                    return false;
                Uri uscr = new Uri(uri.GetPathString(), UriKind.RelativeOrAbsolute);
                if (uscr == null)
                    return false;
                Canvas cv;
                if (mapCanvas.ContainsKey(uscr))
                {
                    cv = mapCanvas[uscr];
                }
                else
                    cv = screendoc.GetCurrentXamlDocument();

                var list = (from FrameworkElement e in cv.Children where e.Name == elementname select e).ToList();
                if(list.Count > 0)
                {
                    cv.Children.Remove(list[0]);

                    Utilities.WPF.DependencyObjectExtensions.UnregisterName(cv, list[0].Name);

                    screendoc.MapScreenEntities.Remove(elementname);

                    mapCanvas[uscr] = cv;
                    screendoc.NeedsSave = true;
                    return true;
                }
            }
            return false;
        }
        public ScreenElement GetItem(string screen, string elementname, string subfolder = null)
        {
            ScreenElement ret;
            ret.Element = null;
            ret.Entity = null;

            var doc = GetScreen(screen, subfolder);
            if (doc != null)
            {
                
                var upath = projectdoc.GetResourcePath("ScreenManager", subfolder);
                //crerare l'Uri corretto per il sinottico, folders eventuali compresi
                Uri uri = ProjectBuilder.GetUriFromName(upath, screen, ScreensManagerComponent, projectdoc, bCheckExists: true);
                if (uri == null)
                    return ret;
                Uri uscr = new Uri(uri.GetPathString(), UriKind.RelativeOrAbsolute);
                if(uscr == null)
                    return ret;
                Canvas cv;
                if (mapCanvas.ContainsKey(uscr))
                {
                    cv = mapCanvas[uscr];
                }
                else
                {
                    cv = doc.GetCurrentXamlDocument(true);
                    mapCanvas[uscr] = cv;
                }
                    

                var lEl = (from FrameworkElement e in cv.Children where e.Name == elementname select e).ToList();
                if(lEl.Count > 0)
                {
                    var xamlData = lEl[0].XamlWriterFormatted();

                    if (Utilities.WPF.XmlHelper.CheckProblematicXamlWriter(lEl[0], xamlData))
                    {
                        doc.SetProblematicXaml(lEl[0], xamlData);
                    }


                    var bSub = doc.IsSubscribePropertyChangeXamlWriterProperties(lEl[0]);
                    if (!bSub)
                        doc.SubscribePropertyChangeXamlWriterProperties(lEl[0], elementname);

                    ret.Element = lEl[0];
                    if (doc.MapScreenEntities.ContainsKey(elementname))
                        ret.Entity = doc.MapScreenEntities[elementname];
                }
            }
            return ret;
        }
        public List<string> GetItemList(string screen, string subfolder = null)
        {
            var doc = GetScreen(screen, subfolder);
            if (doc == null)
                return null;
            Canvas cv = doc.GetCurrentXamlDocument();
            var lEl = (from FrameworkElement e in cv.Children select e.Name).ToList();
            return lEl;
        }

        public bool SetBackground(string name, Brush background, string subfolder = null)
        {
            var screendoc = GetScreen(name, subfolder);
            if(screendoc != null)
            {
                var upath = projectdoc.GetResourcePath("ScreenManager", subfolder);
                //crerare l'Uri corretto per il sinottico, folders eventuali compresi
                Uri uri = ProjectBuilder.GetUriFromName(upath, name, ScreensManagerComponent, projectdoc, bCheckExists: true);
                if (uri == null)
                    return false;
                Uri uscr = new Uri(uri.GetPathString(), UriKind.RelativeOrAbsolute);
                if (uscr == null)
                    return false;
                Canvas cv;
                if (mapCanvas.ContainsKey(uscr))
                {
                    cv = mapCanvas[uscr];
                }
                else
                    cv = screendoc.GetCurrentXamlDocument();

                cv.Background = background;

                mapCanvas[uscr] = cv;
                screendoc.NeedsSave = true;
                return true;
            }

            return false;
        }
        /// <summary>
        /// saves the settings, if some have been modified in the current working session.
        /// </summary>
        public void Save()
        {
            List<Uri> toRemove = new List<Uri>();
            if (mapDocuments.Count > 0)
            {
                foreach(var doc in mapDocuments.Values)
                {
                    if (doc.NeedsSave)
                    {
                        var docuris = (from i in mapDocuments where i.Value == doc select i.Key).ToList();

                        if (docuris.Count > 0)
                        {
                            Canvas origcv;
                            if (docuris[0] != null && mapCanvas.ContainsKey(docuris[0]))
                            {
                                origcv = mapCanvas[docuris[0]];

                                Canvas cv = new Canvas
                                {
                                    Background = origcv.Background,
                                    Resources = origcv.Resources
                                };

                                cv.Height = origcv.Height;
                                cv.Width = origcv.Width;


                                NameScope.SetNameScope(cv, new NameScope());

                                doc.ResolveProblematicXamlOnCanvas(origcv);

                                foreach (FrameworkElement uie in origcv.Children)
                                {
                                    String xamlData = uie.XamlWriterFormatted();
                                    FrameworkElement element = null;
                                    try
                                    {
                                        element = xamlData.ReadUIElement() as FrameworkElement;
                                    }
                                    catch (Exception ex)
                                    {
                                        
                                    }
                                    if (element == null)
                                        continue;

                                    element.SetValue(InkCanvas.TopProperty, DependencyProperty.UnsetValue);
                                    element.SetValue(InkCanvas.LeftProperty, DependencyProperty.UnsetValue);

                                    element.Name = uie.Name;

                                    try
                                    {
                                        cv.Children.Add(element);
                                    }
                                    catch (Exception ex)
                                    {
                                        
                                    }

                                    if (element is FrameworkElement)
                                        Utilities.WPF.DependencyObjectExtensions.RegisterName(cv, element as FrameworkElement, false, false);
                                }

                                doc.RestoreProblematicXamlWriter(origcv);
                                doc.CleanProblematicXamlBags();

                                doc.CleanRepositoryItemBindings(cv);
                                

                                //doc.ResolveProblematicXamlOnCanvas(cv);//verificare eventuali problemi...
                                doc.SaveCurrentDocument(cv);
                                toRemove.Add(docuris[0]);
                            }
                        }
                        else
                            doc.SaveToFile();
                    }
                }
                
            }
            if(toRemove.Count > 0)
            {
                foreach(var u in toRemove)
                {
                    mapDocuments[u].Dispose();
                    mapDocuments.Remove(u);
                    mapCanvas[u].Dispose();
                    mapCanvas.Remove(u);
                }
                toRemove.Clear();
            }
        }

        public void Dispose()
        {
            foreach (var s in mapDocuments.Values)
                s.Dispose();
            mapDocuments.Clear();

            foreach (var c in mapCanvas.Values)
                c.Dispose();
            mapCanvas.Clear();
        }
        #endregion Methods
    }
}
