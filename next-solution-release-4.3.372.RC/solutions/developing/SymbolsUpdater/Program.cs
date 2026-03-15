
using ScreenSettings;
using System;
using System.Linq;
using System.Windows;
using UFProjectManager;
using UFProjectManager.ComponentService;
using Utilities;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Collections;
using UFInterfaces.CoreHostComponents;
using ScreenManager.ComponentService;
using UFUAEditor.ComponentService;
using UFEventEditor.ComponentService;
using MSSchedulerSettings.ComponentService;
using UFMenuEditor.ComponentService;
using DocumentManager.ComponentService;
using UFShortcutEditor.ComponentService;
using DocumentManager.ComponentService.Helpers;
using UriResolver.ComponentService;
using VFS;
using STRL;
using System.Text.RegularExpressions;
using UFUAEditor.Document;
using DevExpress.Xpo;
using UFEventEditor.Document;
using MSSchedulerSettings.Document;
using MenuSettings.Documents;
using UFShortcutSettings.Documents;
using Meters;
using System.Windows.Controls;
using Utilities.WPF;
using ScriptManager.ComponentService;
using ScriptManager.Document;
using UFUAServerInfo;
using StringManager.ComponentService;
using StringManager.Document;
using UFRecipeEditor.ComponentService;
using UFRecipeSettings.Documents;

namespace ProjectUpdater
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
#if DEBUG
            if (!System.Diagnostics.Debugger.IsAttached &&
                Environment.UserInteractive && MessageBox.Show("if you would like to attach a debugger now is the right moment !",
                    "DebugMe - ProjectUpdater", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                System.Diagnostics.Debugger.Launch();
#endif
            CommandLineOptions cl = new CommandLineOptions(args);
            if (cl.IsValid)
            {
                try
                {
                    ProjectUpdater projectUpdater = new ProjectUpdater(cl);
                    projectUpdater.UpdateProject();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(Properties.Resource.ErrorUpdatingProject, cl.Project, ex.Message);
                }
            }
        }
    }

    internal class ProjectUpdater
    {
        readonly ComponentHost componentHost;
        private CommandLineOptions cl;
        private string commonFolder;
        
        public ProjectUpdater(CommandLineOptions cl)
        {
            this.cl = cl;
            commonFolder = String.Format("{0}\\{1}\\{2}",
                             Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                             Properties.Settings.Default.CompanyName,
                             Properties.Settings.Default.CommonApplicationFolder);
            ApplicationPropertiesHelper.SetProperty("CommonFolder", commonFolder);

            componentHost = new ComponentHost();

            UriResolverComponent uriResolverComponent = new UriResolverComponent();
            componentHost.Components.Add(uriResolverComponent);
            var uriRisolver = componentHost.GetService(typeof(IUriRisolver)) as IUriRisolver;
            uriRisolver?.Initialize();
            uriResolverComponent.Container = componentHost.Components;
            uriResolverComponent.GetListInstalledDocumentManagers();
        }

        #region Project
        internal void UpdateProject()
        {
            var uFProjectManagerComponent = componentHost.GetService(typeof(IUFProjectManager)) as UFProjectManagerComponent;
            if (uFProjectManagerComponent == null)
                return;

            using (UFProjectDocument projectDocument = UFProjectDocument.FromFile(cl.Project, uFProjectManagerComponent))
            {
                if (projectDocument == null)
                    return;

                var scriptManagerComponent = componentHost.GetService(typeof(IScriptManager)) as IDocumentManager;
                if (scriptManagerComponent != null)
                    UpdateScripts(uFProjectManagerComponent, projectDocument, scriptManagerComponent);

                var screenManagerComponent = componentHost.GetService(typeof(IScreenManager)) as IDocumentManager;
                if (screenManagerComponent != null)
                    UpdateScreens(uFProjectManagerComponent, projectDocument, screenManagerComponent);

                var serverEditorManager = componentHost.GetService(typeof(IUFUAEditorManager)) as IDocumentManager;
                if (serverEditorManager != null)
                {
                    UpdateIODataServer(serverEditorManager, projectDocument);
                }

                var recipeEditorManager = componentHost.GetService(typeof(IRecipeEditorManager)) as IDocumentManager;
                if (recipeEditorManager != null)
                    UpdateRecipeServer(recipeEditorManager, projectDocument);

                var eventEditorManager = componentHost.GetService(typeof(IEventEditorManager)) as IDocumentManager;
                if (eventEditorManager != null)
                {
                    ReplaceEventExpressions(eventEditorManager, projectDocument);
                }

                var schedulerEditorManager = componentHost.GetService(typeof(ISchedulerEditorManager)) as IDocumentManager;
                if (schedulerEditorManager != null)
                {
                    ReplaceSchedulerExpressions(schedulerEditorManager, projectDocument);
                }

                var menuEditorManager = componentHost.GetService(typeof(IMenuEditorManager)) as IDocumentManager;
                if (menuEditorManager != null)
                {
                    var typeScheme = menuEditorManager.TypeScheme;
                    var menuList = uFProjectManagerComponent.GetResourceList(projectDocument, typeScheme).ToList();
                    menuList.ToList().ForEach(s =>
                    {
                        var fullPath = projectDocument.MakeAbosoluteUri(new Uri(s, UriKind.RelativeOrAbsolute)).GetPathString();
                        ReplaceMenuExpressions(fullPath, projectDocument);
                    });
                }

                var shortcutEditorManager = componentHost.GetService(typeof(IShortcutEditorManager)) as IDocumentManager;
                if (shortcutEditorManager != null)
                {
                    var typeScheme = shortcutEditorManager.TypeScheme;
                    var shortcutList = uFProjectManagerComponent.GetResourceList(projectDocument, typeScheme).ToList();
                    shortcutList.ToList().ForEach(s =>
                    {
                        var fullPath = projectDocument.MakeAbosoluteUri(new Uri(s, UriKind.RelativeOrAbsolute)).GetPathString();
                        ReplaceShortcutExpressions(fullPath, projectDocument);
                    });
                }

                var stringEditorManager = componentHost.GetService(typeof(IStringEditorManager)) as IDocumentManager;
                if (stringEditorManager != null)
                {
                    UpdateStringEditor(stringEditorManager, projectDocument);
                }
            }
        }
        #endregion

        #region I/O Data Server
        void UpdateIODataServer(IDocumentManager manager, UFProjectDocument parentDocument)
        {
            var uri = new Uri(parentDocument.ProjectFolder, UriKind.RelativeOrAbsolute);
            using (UFUAServerDocument document = UFUAServerDocument.FromFile(uri.GetPathString(), manager, parentDocument, bCreateNew: false, bCheckEmpty: false))
            {
                if (document == null)
                    return;

                document.Parent = parentDocument;
                ReplaceIODataServerExpressions(document);
                RenameIODataServerEngineeringUnits(document);

                if (document.SaveToFile())
                {
                    Console.WriteLine(Properties.Resource.IODataServerDocSuccessfullyUpdated, document.Title);
                }
                else if (document.NeedsSave)
                {
                    Console.Error.WriteLine(Properties.Resource.UpdatedIODataSeverDocCannotBeSaved, document.Title);
                }
            }
        }
        #endregion

        #region Recipe Server
        void UpdateRecipeServer(IDocumentManager manager, UFProjectDocument parentDocument)
        {
            var uri = new Uri(parentDocument.ProjectFolder, UriKind.RelativeOrAbsolute);
            using (RecipeUAServerDocument document = RecipeUAServerDocument.FromFile(uri.GetPathString(), manager, parentDocument, bCreateNew: false, bCheckEmpty: false))
            {
                if (document == null)
                    return;

                var session = document.GetSession();
                var res = (from p in new XPQuery<UFRecipeSettings.UFRecipeModel.RecipeUAConfiguration>(session, true).AsParallel() select p).ToList();
                if (res.FirstOrDefault().RedundancyPortNumber == 40000)
                    res.FirstOrDefault().RedundancyPortNumber = 40100;

                if (document.SaveToFile())
                {
                    Console.WriteLine(Properties.Resource.IODataServerDocSuccessfullyUpdated, document.Title);
                }
                else if (document.NeedsSave)
                {
                    Console.Error.WriteLine(Properties.Resource.UpdatedIODataSeverDocCannotBeSaved, document.Title);
                }
            }
        }
        #endregion

        #region Scripts
        void UpdateScripts(UFProjectManagerComponent uFProjectManagerComponent, UFProjectDocument projectDocument, IDocumentManager scriptManagerComponent)
        {
            var typeScheme = scriptManagerComponent.TypeScheme;
            var scriptList = uFProjectManagerComponent.GetResourceList(projectDocument, typeScheme).ToList();
            scriptList.ToList().ForEach(s =>
            {
                var fullPath = projectDocument.MakeAbosoluteUri(new Uri(s, UriKind.RelativeOrAbsolute)).GetPathString();
                UpdateScriptProperties(fullPath, projectDocument);
            });
        }

        void UpdateScriptProperties(string fullPath, UFProjectDocument parentDocument)
        {
            using(ScriptDocument Document = GetScriptDocument(fullPath, parentDocument))
            {
                if (Document == null)
                    return;

                Document.Parent = parentDocument;

                bool needToSave = false;

                if (Document.WriteTimeout < UFUAServerInfo.UFUAServerInfo.GetMinWriteTimeout())
                {
                    Document.WriteTimeout = UFUAServerInfo.UFUAServerInfo.GetMinWriteTimeout();
                    needToSave = true;
                }

                //Save script
                if (needToSave)
                {
                    Document.NeedsSave = true;
                    if (Document.SaveCurrentDocument(false))
                    {
                        Console.WriteLine(Properties.Resource.ScreenSuccessfullyUpdated, fullPath);
                    }
                    else
                    {
                        Console.Error.WriteLine(Properties.Resource.UpdatedScreenCannotBeSaved, fullPath);
                    }
                }
            }
        }

        ScriptDocument GetScriptDocument(string fullPath, UFProjectDocument parentDocument)
        {
            try 
            {
                return ScriptDocument.FromFile(fullPath, parentDocument);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(Properties.Resource.ErrorLoadingFile, fullPath, ex.Message);
            }
            return null;
        }
        #endregion

        #region Screens
        void UpdateScreens(UFProjectManagerComponent uFProjectManagerComponent, UFProjectDocument projectDocument, IDocumentManager screenManagerComponent)
        {
            Hashtable keyGUIDPairs = GetHashTable();
            if (keyGUIDPairs == null || keyGUIDPairs.Count == 0)
                return;

            var typeScheme = screenManagerComponent.TypeScheme;
            var screenList = uFProjectManagerComponent.GetResourceList(projectDocument, typeScheme).ToList();
            screenList.ToList().ForEach(s =>
            {
                var fullPath = projectDocument.MakeAbosoluteUri(new Uri(s, UriKind.RelativeOrAbsolute)).GetPathString();
                ReplaceSourceSymbolLinked(fullPath, keyGUIDPairs, projectDocument);
                ReplaceScreenExpressions(fullPath, projectDocument);
                UpdateScreenProperties(fullPath, projectDocument);
            });

            List<string> projectSymbolList = new List<string>();
            if (projectDocument.fileSystemProviderBase == null)
            {
                var p = DocumentHelper.GetRootParent(projectDocument, true);
                var root = projectDocument.fileSystemProviderBase != null ? string.Empty : p.rootBase;
                if (Directory.Exists($"{root}\\{STRL.STRL.projectSymbolFolder}"))
                {
                    var files = Directory.GetFiles($"{root}\\{STRL.STRL.projectSymbolFolder}", $"*{screenManagerComponent.FileType}", SearchOption.AllDirectories).ToList();
                    projectSymbolList.AddRange(files);
                }
            }
            else
            {
                var folder = new FileManagerFolder(projectDocument.fileSystemProviderBase, STRL.STRL.projectSymbolFolder);
                projectSymbolList.AddRange(GetFiles(projectDocument.fileSystemProviderBase, folder, true));
            }

            projectSymbolList.ToList().ForEach(s =>
            {
                ReplaceSourceSymbolLinked(s, keyGUIDPairs, projectDocument, true);
                ReplaceScreenExpressions(s, projectDocument, true);
            });
        }

        ScreenDocument GetDocument(bool loadFromSettings, string fullPath, UFProjectDocument parentDocument)
        {
            try
            {
                if (loadFromSettings)
                {
                    string settingsFileName = ScreenDocument.GetSettingsFileName(fullPath);

                    if (parentDocument.fileSystemProviderBase != null && parentDocument.fileSystemProviderBase.Exists(new FileManagerFile(parentDocument.fileSystemProviderBase, settingsFileName)))
                    {
                        string settings = System.Text.Encoding.Unicode.GetString(parentDocument.fileSystemProviderBase.ReadFile(new FileManagerFile(parentDocument.fileSystemProviderBase, settingsFileName)));
                        try
                        {
                            settings = WPFUtilities.CryptString.CryptString.DecryptString(settings);
                        }
                        catch
                        {
                        }
                        return settings.FromXml<ScreenDocument>();
                    }
                    else if (File.Exists(settingsFileName))
                    {
                        string settings = File.ReadAllText(settingsFileName);
                        try
                        {
                            settings = WPFUtilities.CryptString.CryptString.DecryptString(settings);
                        }
                        catch
                        {
                        }
                        return settings.FromXml<ScreenDocument>();
                    }
                }
                else
                    return ScreenDocument.FromFile(fullPath, parentDocument, false);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(Properties.Resource.ErrorLoadingFile, fullPath, ex.Message);
            }
            return null;
        }

        List<string> GetFiles(FileSystemProviderBase fileSystemProviderBase, FileManagerFolder folder, bool allDirectories)
        {
            List<string> files = new List<string>();
            if (fileSystemProviderBase.Exists(folder))
            {
                var listFiles = fileSystemProviderBase.GetFiles(folder);
                files = (from c in listFiles/*.AsParallel()*/
                             where System.IO.Path.GetExtension(c.FullName) == ".xaml"
                             orderby c.FullName
                             select c.FullName).ToList();
                if(allDirectories)
                {
                    var listFolders = fileSystemProviderBase.GetFolders(folder);
                    foreach (var f in listFolders)
                    {
                        files.AddRange(GetFiles(fileSystemProviderBase, f, allDirectories));
                    }
                }
            }
            return files;
        }
        #endregion

        #region Symbols
        private struct SectionPair
        {
            public String FilePath;
            public String GUIDStyle;
        }

        Hashtable GetHashTable()
        {
            Hashtable hashtable = new Hashtable();
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "ProjectUpdater.Resources.Guids.xaml";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string content = reader.ReadToEnd();
                    XmlDocument xmlDocument = new XmlDocument();
                    var fragment = xmlDocument.CreateDocumentFragment();
                    fragment.InnerXml = content;
                    foreach (XmlNode node in fragment.ChildNodes)
                    {
                        var tmpchildElement = node as XmlElement;
                        if (tmpchildElement != null && tmpchildElement.Attributes != null &&
                            tmpchildElement.Attributes.GetNamedItem("ID") != null &&
                            tmpchildElement.Attributes.GetNamedItem("NID") != null &&
                            !string.IsNullOrEmpty(tmpchildElement.InnerText))
                        {
                            SectionPair sectionPair;
                            sectionPair.FilePath = tmpchildElement.InnerText;
                            sectionPair.GUIDStyle = tmpchildElement.Attributes.GetNamedItem("ID").Value;
                            hashtable.Add(sectionPair, tmpchildElement.Attributes.GetNamedItem("NID").Value);
                        }
                    }
                }
            }
            return hashtable;
        }

        bool FixScreenControls(ScreenDocument Document)
        {
            bool bNeedToSave = false;
            foreach (var entityPair in Document.MapScreenEntities)
            {
                var entity = entityPair.Value;
                var name = entityPair.Key;
                if (entity.Entity == null)
                {
                    UIElement uie = null;
                    if (!string.IsNullOrEmpty(entity.ProblematicXaml))
                    {
                        try
                        {
                            uie = entity.ProblematicXaml.ReadUIElement();
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    if (uie != null)
                    {
                        entity.Entity = uie;
                        entity.Document = Document;
                    }
                }
                if (entity.Entity?.GetType() == typeof(LinearMeterControl))
                {
                    if (entity.MapProblematicXamlWriterProperties != null && entity.MapProblematicXamlWriterProperties.ContainsKey(UserControl.BackgroundProperty.Name))
                    {
                        entity.MapProblematicXamlWriterProperties.Remove(UserControl.BackgroundProperty.Name);
                        bNeedToSave = true;
                    }
                    string bck = " Background=\".*?\"";
                    var replaced = Regex.Replace(entity.ProblematicXaml, bck, " ");
                    if (replaced != entity.ProblematicXaml)
                    {
                        entity.ProblematicXaml = replaced;
                        bNeedToSave = true;
                    }
                }
            }
            return bNeedToSave;
        }

        void ReplaceSourceSymbolLinked(string fullPath, Hashtable keyGUIDPairs, UFProjectDocument parentDocument, bool isSymbolFile = false)
        {
            using (ScreenDocument Document = GetDocument(isSymbolFile, fullPath, parentDocument))
            {
                if (Document == null)
                    return;

                Document.Parent = parentDocument;

                bool needToSave = false;

                if (!isSymbolFile)
                    needToSave = FixScreenControls(Document);

                var listElementToReplace = (from entry in Document.MapScreenEntities.AsParallel()
                                            where entry.Value.SourceSymbolLinked &&
                                            !string.IsNullOrEmpty(entry.Value.SourceSymbolPath) &&
                                            !String.IsNullOrEmpty(STRL.STRL.GetSymbolStyleKey(entry.Value.SourceSymbolProvider, entry.Value.SourceSymbolPath))
                                            select entry.Key).ToList();

                if (!needToSave && listElementToReplace.Count == 0)
                {
#if DEBUG
                    if (isSymbolFile)
                        Console.WriteLine(Properties.Resource.SymbolLibraryNothingToDo, fullPath);
                    else
                        Console.WriteLine(Properties.Resource.ScreenNothingToDo, fullPath);
#endif
                    return;
                }

                listElementToReplace.ForEach(entity =>
                {
                    var entry = Document.MapScreenEntities[entity];
                    string symbolPath = Document.MapScreenEntities[entity].SourceSymbolPath;
                    bool bCrypted = false;
                    bool replace = false;
                    try
                    {
                        symbolPath = WPFUtilities.CryptString.CryptString.DecryptString(symbolPath);
                        bCrypted = true;
                    }
                    catch (Exception)
                    {
                    }

                    string[] parts = symbolPath.Split(STRL.STRL.delimiters,
                             StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length <= 2)
                        return;

                    var baseFileElement = parts[0];
                    var sourceSymbolPath = parts[1];
                    var key = parts[2];

                    if ((sourceSymbolPath.Contains("\\CheckBox\\") && baseFileElement.Contains("\\CheckBox") && !baseFileElement.Contains("\\CheckBoxControl")) ||
                        (sourceSymbolPath.Contains("\\Button\\") && baseFileElement.Contains("\\Button") && !baseFileElement.Contains("\\ButtonControl")) ||
                        (sourceSymbolPath.Contains("\\CheckBoxControl\\") && baseFileElement.Contains("\\CheckBoxControl")) ||
                        (sourceSymbolPath.Contains("\\ButtonControl\\") && baseFileElement.Contains("\\ButtonControl")))
                    {
                        SectionPair sectionPair;
                        sectionPair.FilePath = sourceSymbolPath.Replace($"&", $"_").Replace($"{commonFolder}\\", $"..\\").UpdateStyleFolder(true);
                        sectionPair.GUIDStyle = key;
                        if (keyGUIDPairs.ContainsKey(sectionPair))
                            symbolPath = symbolPath.Replace(key, keyGUIDPairs[sectionPair].ToString()).UpdateStyleFolder();
                        else
                            symbolPath = symbolPath.UpdateStyleFolder();
                        replace = true;
                        needToSave = true;
                    }
                    
                    if(replace)
                    {
                        if (bCrypted)
                            symbolPath = WPFUtilities.CryptString.CryptString.EncryptString(symbolPath);
                        Document.MapScreenEntities[entity].SourceSymbolPath = symbolPath;
                    }
                });

                if(needToSave)
                {
                    if (isSymbolFile)
                    {
                        string settingsFilename = ScreenDocument.GetSettingsFileName(fullPath);
                        string settings = Document.ToXml();
                        try
                        {
                            if (Document.fileSystemProviderBase != null)
                                Document.fileSystemProviderBase.UploadFile(null, settingsFilename,
                                    System.Text.Encoding.Unicode.GetBytes(settings));
                            else
                                File.WriteAllText(settingsFilename, WPFUtilities.CryptString.CryptString.EncryptString(settings));

                            Console.WriteLine(Properties.Resource.SymbolLibrarySuccessfullyUpdated, fullPath);
                        }
                        catch (Exception)
                        {
                            Console.Error.WriteLine(Properties.Resource.UpdatedSymbolLibraryCannotBeSaved, fullPath);
                        }
                    }
                    else
                    {
                        Document.NeedsSave = true;
                        if (Document.SaveToFile(forceEncryption: parentDocument.IsPasswordProtected(), bThrowOnError: true))
                        {
                            Console.WriteLine(Properties.Resource.ScreenSuccessfullyUpdated, fullPath);
                        }
                        else
                        {
                            Console.Error.WriteLine(Properties.Resource.UpdatedScreenCannotBeSaved, fullPath);
                        }
                    }
                }
            }
        }
        #endregion

        #region Properties
        void UpdateScreenProperties(string fullPath, UFProjectDocument parentDocument, bool isSymbolFile = false)
        {
            using (ScreenDocument Document = GetDocument(isSymbolFile, fullPath, parentDocument))
            {
                if (Document == null)
                    return;

                Document.Parent = parentDocument;

                bool needToSave = false;

                if(Document.WriteTimeout < UFUAServerInfo.UFUAServerInfo.GetMinWriteTimeout())
                {
                    Document.WriteTimeout = UFUAServerInfo.UFUAServerInfo.GetMinWriteTimeout();
                    needToSave = true;
                }

                //Save screen
                if (needToSave)
                {
                    if (isSymbolFile)
                    {
                        string settingsFilename = ScreenDocument.GetSettingsFileName(fullPath);
                        string settings = Document.ToXml();
                        try
                        {
                            if (Document.fileSystemProviderBase != null)
                                Document.fileSystemProviderBase.UploadFile(null, settingsFilename,
                                    System.Text.Encoding.Unicode.GetBytes(settings));
                            else
                                File.WriteAllText(settingsFilename, WPFUtilities.CryptString.CryptString.EncryptString(settings));

                            Console.WriteLine(Properties.Resource.SymbolLibrarySuccessfullyUpdated, fullPath);
                        }
                        catch (Exception)
                        {
                            Console.Error.WriteLine(Properties.Resource.UpdatedSymbolLibraryCannotBeSaved, fullPath);
                        }
                    }
                    else
                    {
                        Document.NeedsSave = true;
                        if (Document.SaveToFile(forceEncryption: parentDocument.IsPasswordProtected(), bThrowOnError: true))
                        {
                            Console.WriteLine(Properties.Resource.ScreenSuccessfullyUpdated, fullPath);
                        }
                        else
                        {
                            Console.Error.WriteLine(Properties.Resource.UpdatedScreenCannotBeSaved, fullPath);
                        }
                    }
                }
            }
        }
        #endregion

        #region Expressions
        void ReplaceScreenExpressions(string fullPath, UFProjectDocument parentDocument, bool isSymbolFile = false)
        {
            using (ScreenDocument Document = GetDocument(isSymbolFile, fullPath, parentDocument))
            {
                if (Document == null)
                    return;

                bool needToSave = false;
                Document.Parent = parentDocument;

                (from entry in Document.MapScreenEntities.AsParallel()
                 where !String.IsNullOrEmpty(entry.Value.Expression) || 
                 !String.IsNullOrEmpty(entry.Value.ReverseExpression) ||
                 entry.Value.HasAnimations || entry.Value.HasCommands
                 select entry.Key).AsParallel().ForAll(entry => 
                 {
                     var entity = Document.MapScreenEntities[entry];
                     if (!String.IsNullOrEmpty(entity.Expression))
                     {
                         var expression = ReplaceExpression(entity.Expression);
                         if (expression != entity.Expression)
                         {
                             needToSave = true;
                             entity.Expression = expression;
                         }
                     }
                     if (!String.IsNullOrEmpty(entity.ReverseExpression))
                     {
                         var expression = ReplaceExpression(entity.ReverseExpression);
                         if (expression != entity.ReverseExpression)
                         {
                             needToSave = true;
                             entity.ReverseExpression = expression;
                         }
                     }

                     if (entity.HasAnimations)
                     {
                         foreach (AnimationManager.AnimationManager animation in entity.AnimationList)
                         {
                             if (!String.IsNullOrEmpty(animation.Expression))
                             {
                                 var expression = ReplaceExpression(animation.Expression);
                                 if (expression != animation.Expression)
                                 {
                                     needToSave = true;
                                     animation.Expression = expression;
                                 }
                             }
                         }
                     }

                     if (entity.HasCommands)
                     {
                         foreach (CommandManager.CommandManager command in entity.CommandList)
                         {
                             if (!String.IsNullOrEmpty(command.Expression))
                             {
                                 var expression = ReplaceExpression(command.Expression);
                                 if (expression != command.Expression)
                                 {
                                     needToSave = true;
                                     command.Expression = expression;
                                 }
                             }
                         }
                     }
                 });

                if (needToSave)
                {
                    if (isSymbolFile)
                    {
                        string settingsFilename = ScreenDocument.GetSettingsFileName(fullPath);
                        string settings = Document.ToXml();
                        try
                        {
                            if (Document.fileSystemProviderBase != null)
                                Document.fileSystemProviderBase.UploadFile(null, settingsFilename,
                                    System.Text.Encoding.Unicode.GetBytes(settings));
                            else
                                File.WriteAllText(settingsFilename, WPFUtilities.CryptString.CryptString.EncryptString(settings));

                            Console.WriteLine(Properties.Resource.SymbolLibrarySuccessfullyUpdated, fullPath);
                        }
                        catch (Exception)
                        {
                            Console.Error.WriteLine(Properties.Resource.UpdatedSymbolLibraryCannotBeSaved, fullPath);
                        }
                    }
                    else
                    {
                        Document.NeedsSave = true;
                        if (Document.SaveToFile(forceEncryption: parentDocument.IsPasswordProtected(), bThrowOnError: true))
                        {
                            Console.WriteLine(Properties.Resource.ScreenSuccessfullyUpdated, fullPath);
                        }
                        else
                        {
                            Console.Error.WriteLine(Properties.Resource.UpdatedScreenCannotBeSaved, fullPath);
                        }
                    }
                }
            }
        }

        void ReplaceIODataServerExpressions(UFUAServerDocument document)
        {
            var session = document.GetSession();
            (from p in new XPQuery<UFUAModel.UFUAAlarmThreshold>(session, true).AsParallel()
             where !String.IsNullOrEmpty(p.Expression) ||
                !String.IsNullOrEmpty(p.SeverityExpression) ||
                !String.IsNullOrEmpty(p.CommandsOn) ||
                !String.IsNullOrEmpty(p.CommandsOff) ||
                !String.IsNullOrEmpty(p.CommandsAck) ||
                !String.IsNullOrEmpty(p.CommandsReset) ||
                !String.IsNullOrEmpty(p.CommandsDbClick)
             select p).AsParallel().ForAll(entry =>
             {
                 if (!String.IsNullOrEmpty(entry.Expression))
                     entry.Expression = ReplaceExpression(entry.Expression);
                 if (!String.IsNullOrEmpty(entry.SeverityExpression))
                     entry.SeverityExpression = ReplaceExpression(entry.SeverityExpression);

                 var commands = ReplaceCommandListExpression(entry.CommandsOn);
                 if (entry.CommandsOn != commands)
                     entry.CommandsOn = commands;
                 commands = ReplaceCommandListExpression(entry.CommandsOff);
                 if (entry.CommandsOff != commands)
                     entry.CommandsOff = commands;
                 commands = ReplaceCommandListExpression(entry.CommandsAck);
                 if (entry.CommandsAck != commands)
                     entry.CommandsAck = commands;
                 commands = ReplaceCommandListExpression(entry.CommandsReset);
                 if (entry.CommandsReset != commands)
                     entry.CommandsReset = commands;
                 commands = ReplaceCommandListExpression(entry.CommandsDbClick);
                 if (entry.CommandsDbClick != commands)
                     entry.CommandsDbClick = commands;
             });

            (from p in new XPQuery<DataLoggerModel.DataLoggerColumn>(session, true).AsParallel()
             where !String.IsNullOrEmpty(p.Expression)
             select p).AsParallel().ForAll(entry =>
             {
                 if (!String.IsNullOrEmpty(entry.Expression))
                     entry.Expression = ReplaceExpression(entry.Expression);
             });
        }

        void ReplaceEventExpressions(IDocumentManager manager, UFProjectDocument parentDocument)
        {
            var uri = new Uri(parentDocument.ProjectFolder, UriKind.RelativeOrAbsolute);
            using (EventEditorDocument document = EventEditorDocument.FromFile(uri.GetPathString(), manager, parentDocument, bCreateNew: false))
            {
                if (document == null)
                    return;

                document.Parent = parentDocument;
                var session = document.GetSession();

                (from p in new XPQuery<UFEventModel.UFEventObject>(session, true).AsParallel()
                 where !String.IsNullOrEmpty(p.Expression) ||
                 !String.IsNullOrEmpty(p.EventCommandList)
                 select p).AsParallel().ForAll(entry =>
                 {
                     if (!String.IsNullOrEmpty(entry.Expression))
                         entry.Expression = ReplaceExpression(entry.Expression);

                     var commands = ReplaceCommandListExpression(entry.EventCommandList);
                     if (entry.EventCommandList != commands)
                         entry.EventCommandList = commands;
                 });

                if (document.SaveToFile())
                {
                    Console.WriteLine(Properties.Resource.EventDocSuccessfullyUpdated, document.Title);
                }
                else if (document.NeedsSave)
                {
                    Console.Error.WriteLine(Properties.Resource.UpdatedEventDocCannotBeSaved, document.Title);
                }
            }
        }

        void ReplaceSchedulerExpressions(IDocumentManager manager, UFProjectDocument parentDocument)
        {
            var uri = new Uri(parentDocument.ProjectFolder, UriKind.RelativeOrAbsolute);
            using (SchedulerEditorDocument document = SchedulerEditorDocument.FromFile(uri.GetPathString(), manager, parentDocument, bCreateNew: false))
            {
                if (document == null)
                    return;

                document.Parent = parentDocument;
                var session = document.GetSession();

                (from p in new XPQuery<MSModel.MSScheduledAction>(session, true).AsParallel()
                 where !String.IsNullOrEmpty(p.CommandsOn) || !String.IsNullOrEmpty(p.CommandsOff) || 
                 !String.IsNullOrEmpty(p.ExceptionCommandsOn) || !String.IsNullOrEmpty(p.ExceptionCommandsOff)
                 select p).AsParallel().ForAll(entry =>
                 {
                     var commands = ReplaceCommandListExpression(entry.CommandsOn);
                     if (entry.CommandsOn != commands)
                         entry.CommandsOn = commands;
                     commands = ReplaceCommandListExpression(entry.CommandsOff);
                     if (entry.CommandsOff != commands)
                         entry.CommandsOff = commands;
                     commands = ReplaceCommandListExpression(entry.ExceptionCommandsOn);
                     if (entry.ExceptionCommandsOn != commands)
                         entry.ExceptionCommandsOn = commands;
                     commands = ReplaceCommandListExpression(entry.ExceptionCommandsOff);
                     if (entry.ExceptionCommandsOff != commands)
                         entry.ExceptionCommandsOff = commands;
                 });

                if (document.SaveToFile())
                {
                    Console.WriteLine(Properties.Resource.SchedulerDocSuccessfullyUpdated, document.Title);
                }
                else if (document.NeedsSave)
                {
                    Console.Error.WriteLine(Properties.Resource.UpdatedSchedulerDocCannotBeSaved, document.Title);
                }
            }
        }

        void ReplaceMenuExpressions(string fullPath, UFProjectDocument parentDocument)
        {
            using (UFMenuDocument document = UFMenuDocument.FromFile(fullPath, parentDocument))
            {
                if (document == null)
                    return;

                document.Parent = parentDocument;
                (from p in document.GetMenuItemCollection().AsParallel()
                 where p.CommandList is CommandManager.CommandManagerList && (p.CommandList as CommandManager.CommandManagerList).Count > 0
                 select p).AsParallel().ForAll(entry =>
                 {
                     document.NeedsSave |= ReplaceCommandListExpression(entry.CommandList as CommandManager.CommandManagerList);
                 });

                if (document.NeedsSave && document.SaveToFile())
                {
                    Console.WriteLine(Properties.Resource.MenuSuccessfullyUpdated, fullPath);
                }
                else if (document.NeedsSave)
                {
                    Console.Error.WriteLine(Properties.Resource.UpdatedMenuCannotBeSaved, fullPath);
                }
            }
        }

        void ReplaceShortcutExpressions(string fullPath, UFProjectDocument parentDocument)
        {
            using (UFShortcutDocument document = UFShortcutDocument.FromFile(fullPath, parentDocument))
            {
                if (document == null)
                    return;

                document.Parent = parentDocument;
                (from p in document.GetKeyCommandCollection().AsParallel()
                 where p.CommandList is CommandManager.CommandManagerList && (p.CommandList as CommandManager.CommandManagerList).Count > 0
                 select p).AsParallel().ForAll(entry =>
                 {
                     document.NeedsSave |= ReplaceCommandListExpression(entry.CommandList as CommandManager.CommandManagerList);
                 });

                if (document.NeedsSave && document.SaveToFile())
                {
                    Console.WriteLine(Properties.Resource.ShortcutSuccessfullyUpdated, fullPath);
                }
                else if (document.NeedsSave)
                {
                    Console.Error.WriteLine(Properties.Resource.UpdatedShortcutCannotBeSaved, fullPath);
                }
            }
        }

        static String ReplaceCommandListExpression(String commands)
        {
            if (String.IsNullOrEmpty(commands))
                return commands;

            var commandList = commands.FromXml<CommandManager.CommandManagerList>();
            if (ReplaceCommandListExpression(commandList))
                return commandList.ToXml();
            else
                return commands;
        }

        static bool ReplaceCommandListExpression(CommandManager.CommandManagerList commandList)
        {
            if (commandList.Count == 0)
                return false;

            bool bChanged = false;
            foreach (CommandManager.CommandManager command in commandList)
            {
                if (!String.IsNullOrEmpty(command.Expression))
                {
                    var expression = ReplaceExpression(command.Expression);
                    if (expression != command.Expression)
                    {
                        command.Expression = expression;
                        bChanged = true;
                    }
                }
            }

            return bChanged;
        }

        static String extraWhiteSpace = @"/(^\s+)|\s(?=\s+)|(\s+$)/g";

        static Dictionary<String, String> searchFunctions = new Dictionary<String, String>()
        {
            { "ACCRINT", "ACCRINTM" },
            { "AVG", "AVERAGE" },
            { "BINOMDIST", "BINOM.DIST" },
            { "CHIDIST", "CHISQ.DIST.RT" },
            { "CHIINV", "CHISQ.INV.RT" },
            { "CRITBINOM", "BINOM.INV" },
            { "CONFIDENCE", "CONFIDENCE.NORM" },
            { "COVAR", "COVARIANCE.P" },
            { "EXPONDIST", "EXPON.DIST" },
            { "FDIST", "F.DIST" },
            { "FINV", "F.INV.RT" },
            { "GAMMADIST", "GAMMA.DIST" },
            { "GAMMAINV", "GAMMA.INV" },
            { "HYPGEOMDIST", "HYPGEOM.DIST" },
            { "LOGINV", "LOGNORM.INV" },
            { "LOGNORMDIST", "LOGNORM.DIST" },
            { "MODE", "MODE.SNGL" },
            { "NEGBINOMDIST", "NEGBINOM.DIST" },
            { "NORMDIST", "NORM.DIST" },
            { "NORMINV", "NORM.INV" },
            { "NORMSDIST", "NORM.S.DIST" },
            { "NORMSINV", "NORM.S.INV" },
            { "PERCENTILE", "PERCENTILE.INC" },
            { "PERCENTRANK", "PERCENTRANK.INC" },
            { "POISSON", "POISSON.DIST" },
            { "POW", "POWER" },
            { "QUARTILE", "QUARTILE.INC" },
            { "RANK", "RANK.EQ" },
            { "STDEV", "STDEVA" },
            { "STDEVP", "STDEV.P" },
            { "VAR", "VARA" },
            { "VARP", "VAR.P" },
            { "ZTEST", "Z.TEST" },
            { "BITAND", "BITAND.QWORD" },
            { "BITOR", "BITOR.QWORD" },
            { "BITXOR", "BITXOR.QWORD" }
        };

        static Dictionary<String, String> searchOperators = new Dictionary<String, String>()
        {
            { " AND ", "AND" },
            { " OR ", "OR" },
            { " XOR ", "XOR" },
        };

        static Dictionary<String, String> searchReplaces = new Dictionary<String, String>()
        {
            { "=<", "<=" },
            { "=>", ">=" },
            { "= <", "<=" },
            { "= >", ">=" },
            { "> =", ">=" },
            { "< =", "<=" }
        };

        internal static string ReplaceExpression(string expression)
        {
            if (String.IsNullOrEmpty(expression))
                return expression;

            var replacedExpression = Regex.Replace(expression, extraWhiteSpace, String.Empty).Trim();
            var originalExpression = replacedExpression;
            foreach (var key in searchFunctions.Keys)
            {
                var index = replacedExpression.IndexOf(key, StringComparison.OrdinalIgnoreCase);
                while (index != -1)
                {
                    if (!OverlapAlias(replacedExpression, index, index + key.Length))
                    {
                        var prevChr = index > 0 ? replacedExpression[index - 1] : Char.MinValue;
                        var nextChr = index + key.Length < replacedExpression.Length ? replacedExpression[index + key.Length] : Char.MinValue;
                        if ((prevChr == Char.MinValue || !Char.IsLetterOrDigit(prevChr)) && (nextChr == '(' || Char.IsWhiteSpace(nextChr)))
                        {
                            replacedExpression = String.Format("{0}{1}{2}", replacedExpression.Substring(0, index), searchFunctions[key], replacedExpression.Substring(index + key.Length));
                        }

                    }
                    index = replacedExpression.IndexOf(key, index + 1, StringComparison.OrdinalIgnoreCase);
                }
            }

            foreach (var key in searchOperators.Keys)
            {
                var index = replacedExpression.IndexOf(key, StringComparison.OrdinalIgnoreCase);
                while (index != -1)
                {
                    if(!OverlapAlias(replacedExpression, index, index + key.Length))
                    {
                        var leftparameter = FindLeftParameter(replacedExpression.Substring(0, index));
                        var rightparameter = FindRightParameter(replacedExpression.Substring(index + key.Length));
                        var leftpart = replacedExpression.Substring(0, index - leftparameter.Length);
                        var rightpart = replacedExpression.Substring(index + key.Length + rightparameter.Length);
                        replacedExpression = String.Format("{0}{1}({2}, {3}){4}", leftpart, searchOperators[key], leftparameter, rightparameter, rightpart);
                        index = replacedExpression.IndexOf(key, StringComparison.OrdinalIgnoreCase);
                    }
                    else
                        index = replacedExpression.IndexOf(key, index + 1, StringComparison.OrdinalIgnoreCase);
                }
            }

            foreach (var key in searchReplaces.Keys)
            {
                var index = replacedExpression.IndexOf(key, StringComparison.OrdinalIgnoreCase);
                while (index != -1)
                {
                    if (!IsClosedByDoubleQuotes(replacedExpression, index, index + key.Length) && !OverlapAlias(replacedExpression, index, index + key.Length))
                    {
                        replacedExpression = String.Format("{0}{1}{2}", replacedExpression.Substring(0, index), searchReplaces[key], replacedExpression.Substring(index + key.Length));
                        index = replacedExpression.IndexOf(key, index + searchReplaces[key].Length, StringComparison.OrdinalIgnoreCase);
                    }
                    else
                        index = replacedExpression.IndexOf(key, index + key.Length, StringComparison.OrdinalIgnoreCase);
                }
            }

            if (originalExpression != replacedExpression)
                return replacedExpression;
            else
                return expression;
        }

        static string FindLeftParameter(string expression)
        {
            string parameter = string.Empty;
            int openParenthesis = 0;
            int closedParenthesis = 0;
            for (int ii = expression.Length - 1; ii >= 0; ii--)
            {
                if ((expression[ii] == '=' || expression[ii] == ' ' || expression[ii] == '(') && openParenthesis == closedParenthesis)
                {
                    break;
                }

                if (expression[ii] == ')')
                    closedParenthesis++;
                else if (expression[ii] == '(')
                    openParenthesis++;

                parameter = String.Format("{0}{1}", expression[ii], parameter);
            }

            return parameter;
        }

        static string FindRightParameter(string expression)
        {
            string parameter = string.Empty;
            int openParenthesis = 0;
            int closedParenthesis = 0;
            for (int ii = 0; ii < expression.Length; ii++)
            {
                if ((expression[ii] == '=' || expression[ii] == ' ' || expression[ii] == ')' || expression[ii] == ',') && openParenthesis == closedParenthesis)
                {
                    break;
                }

                if (expression[ii] == ')')
                    closedParenthesis++;
                else if (expression[ii] == '(')
                    openParenthesis++;

                parameter = String.Format("{0}{1}", parameter, expression[ii]);
            }

            return parameter;
        }
        static bool OverlapAlias(string expression, int start, int end)
        {
            //return false if there's not an alias, or the part from start to end do not overlap an alias
            int startAlias = expression.IndexOf("<<");
            if(startAlias == -1)
                return false;
            int endAlias = expression.IndexOf(">>");
            if((endAlias != -1) && ((end >= startAlias && end <= endAlias) || (start >= startAlias && start <= endAlias)))
                return true;
            return false;
        }
        static bool IsClosedByDoubleQuotes(string expression, int leftIndex, int rightIndex)
        {
            var leftPart = expression.Substring(0, leftIndex);
            bool bLeft = false;
            for (int ii = leftPart.Length - 1; ii >= 0; ii--)
            {
                if (leftPart[ii] == '"' && (ii == 0 || leftPart[ii - 1] != '"'))
                {
                    bLeft = true;
                    break;
                }
            }

            if (bLeft)
            {
                var rightPart = expression.Substring(rightIndex);
                for (int ii = 0; ii < rightPart.Length; ii++)
                {
                    if (rightPart[ii] == '"' && (ii == rightPart.Length || rightPart[ii + 1] != '"'))
                        return true;
                }
            }

            return false;
        }
        #endregion

        #region Engineering Units
        void RenameIODataServerEngineeringUnits(UFUAServerDocument document)
        {
            var session = document.GetSession();
            var list = (from p in new XPQuery<UFUAModel.UFUAEngineeringUnit>(session, true).AsParallel()
                        where !UFUAModel.Helpers.NameValidator.IsValidName(p.Name)
                        select p).ToList();
            list.ForEach(entry =>
            {
                var oldName = entry.Name;
                var newName = UFUAModel.Helpers.NameValidator.EnsureValidName(entry.Name);
                if (oldName != newName)
                {
                    document.EngineeringUnitsNameReplace(oldName, newName);
                    entry.Name = newName;
                }
            });
        }
        #endregion

        #region String Editor

        void UpdateStringEditor(IDocumentManager manager, UFProjectDocument parentDocument)
        {
            var uri = new Uri(parentDocument.ProjectFolder, UriKind.RelativeOrAbsolute);
            using (StringEditorDocument document = StringEditorDocument.FromFile(uri.GetPathString(), manager, parentDocument, bCreateNew: false, bCheckEmpty: false))
            {
                if (document == null)
                    return;

                document.Parent = parentDocument;
                ReplaceAlarmWindowStringID(document);

                if (document.SaveToFile())
                {
                    Console.WriteLine(Properties.Resource.StringEditorDocSuccessfullyUpdated, document.Title);
                }
                else if (document.NeedsSave)
                {
                    Console.Error.WriteLine(Properties.Resource.UpdatedStringEditorDocCannotBeSaved, document.Title);
                }
            }
        }

        void ReplaceAlarmWindowStringID(StringEditorDocument document)
        {
            var retConditionOk = document.UpdateStringID(
                Properties.Resource.AlarmWindowOldStringIdCondition, 
                Properties.Resource.AlarmWindowNewStringIdCondition);

            if (!retConditionOk)
            {
                Console.Error.WriteLine(
                    Properties.Resource.ErrorUpdatingStringIdAlreadyExists, 
                    document.Parent.Title,
                    Properties.Resource.AlarmWindowNewStringIdCondition);
            }

            var retReasonOk = document.UpdateStringID(
                Properties.Resource.AlarmWindowOldStringIdReason, 
                Properties.Resource.AlarmWindowNewStringIdReason);

            if(!retReasonOk)
            {
                Console.Error.WriteLine(
                    Properties.Resource.ErrorUpdatingStringIdAlreadyExists, 
                    document.Parent.Title,
                    Properties.Resource.AlarmWindowNewStringIdReason);
            }

        }

        #endregion
    }
}
