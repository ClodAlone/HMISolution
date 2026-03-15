
using ScreenSettings;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UFProjectManager;
using UFProjectManager.ComponentService;
using Utilities;
using Utilities.WPF;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using System.Threading;

namespace ScreenCompiler
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            CommandLineOptions cl = new CommandLineOptions(args);
            if (cl.IsValid && !cl.DeleteCompiledFiles)
            {
#if DEBUG
                //if (!System.Diagnostics.Debugger.IsAttached &&
                //    Environment.UserInteractive && MessageBox.Show("if you would like to attach a debugger now is the right moment !",
                //        "DebugMe - ScreenCompiler", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                //    System.Diagnostics.Debugger.Launch();
#endif
                try
                {
                    SymbolResolver symbolResolver = new SymbolResolver(cl);
                    symbolResolver.ReplaceSourceSymbolLinkedInScreen();
                }
                catch (Exception ex)
                {
                    Environment.ExitCode = -1;
                    Console.Error.WriteLine(Properties.Resource.ErrorCompilingScreen, cl.Screen, ex.Message);
                }
            }


            if(cl.DeleteCompiledFiles)
            {
                SymbolResolver symbolResolver = new SymbolResolver(cl);
                symbolResolver.RemoveCompiledFiles(cl.Screen);
            }
        }
    }

    class SymbolResolver
    {
        ThemedWindow dispatcher;
        private CommandLineOptions cl;
        public SymbolResolver(CommandLineOptions cl)
        {
            this.cl = cl;
            var commonFolder = String.Format("{0}\\{1}\\{2}",
                             Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                             Properties.Settings.Default.CompanyName,
                             Properties.Settings.Default.CommonApplicationFolder);
            ApplicationPropertiesHelper.SetProperty("CommonFolder", commonFolder);
        }

        internal void ReplaceSourceSymbolLinkedInScreen()
        {
            string fullPath = cl.Screen;
            string projectPath = cl.Project;
            var filepathMutex = projectPath.Replace("/", ".").Replace("\\", ".");
            using (var mutex = new Mutex(false, filepathMutex))
            {
                try
                {
                    mutex.WaitOne();
                }
                catch (AbandonedMutexException ex)
                {
                    Utilities.Logger.Logger.WriteToEventLog(Properties.Resource.LoggerSource,
                        ex.Message, System.Diagnostics.EventLogEntryType.Warning);
                }

                try
                {
                    using (UFProjectDocument parentDocument = UFProjectDocument.FromFile(projectPath, new UFProjectManagerComponent()))
                    {
                        try
                        {
                            ReplaceSourceSymbolLinked(fullPath, parentDocument);
                        }
                        catch
                        {
                            RemoveAllCompiledScreenFiles(fullPath);
                            throw;
                        }
                    }
                }
                finally
                {
                    mutex.ReleaseMutex();
                    dispatcher?.Close();
                }
            }
        }

        void dispatcher_Closing(object sender, EventArgs e)
        {
            if (dispatcher == null)
                return;
            dispatcher.Closing -= dispatcher_Closing;
            dispatcher.Dispatcher.BeginInvokeShutdown(DispatcherPriority.Background); //case 22521
        }

        void ReplaceSourceSymbolLinked(string fullPath, UFProjectDocument parentDocument)
        {
            var isSourceEncrypted = !Utilities.IO.FileSystem.IsXmlFile(fullPath);
            if (parentDocument.Protected && !isSourceEncrypted)
                parentDocument.disableProtection();

            using (ScreenDocument Document = ScreenDocument.FromFile(fullPath, parentDocument))
            {
                if (Document == null)
                {
                    RemoveAllCompiledScreenFiles(fullPath);
                    return;
                }

                LoadRequiredAssemblies();

                Document.Parent = parentDocument;
                var ActiveLayer = Document.GetCurrentXamlDocument(false);
                if (ActiveLayer == null)
                {
                    RemoveAllCompiledScreenFiles(fullPath);
                    return;
                }

                dispatcher = new ThemedWindow()
                {
                    Width = 0,
                    Height = 0,
                    WindowStyle = WindowStyle.None,
                    WindowState = WindowState.Minimized,
                    ShowInTaskbar = false,
                    ShowActivated = false
                };
                dispatcher.Closing += dispatcher_Closing;
                dispatcher.Show();
                
                var gridContainer = new Grid();
                dispatcher.Content = gridContainer;
                gridContainer.Children.Add(ActiveLayer);

                var listElementToReplace = (from entry in Document.MapScreenEntities.AsParallel()
                                            where entry.Value.SourceSymbolLinked &&
                                            !string.IsNullOrEmpty(entry.Value.SourceSymbolPath) &&
                                            String.IsNullOrEmpty(STRL.STRL.GetSymbolStyleKey(entry.Value.SourceSymbolProvider, entry.Value.SourceSymbolPath))
                                            select entry.Key).ToList();

                //if (listElementToReplace.Count == 0)
                //{
                //    Console.WriteLine(Properties.Resource.CompiledScreenNothingToDo, cl.Screen);
                //    return;
                //}

                FindControls(ActiveLayer, listElementToReplace, Document, ActiveLayer, true);
                Document.RefreshEntityStyleBinding(ActiveLayer);

                string extension = System.IO.Path.GetExtension(fullPath);
                string fileName = $"{System.IO.Path.GetFileNameWithoutExtension(fullPath)}{extension}.{ScreenDocument.compiledExt}";
                string screenFileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(fullPath), fileName);

                if (!string.IsNullOrEmpty(cl.ScreenCompiledPath))
                    screenFileName = cl.ScreenCompiledPath;
                else
                {
                    if (cl.OutputForTest)
                    {
                        fileName = $"{System.IO.Path.GetFileNameWithoutExtension(fullPath)}.{ScreenDocument.compiledExt}{extension}";
                        screenFileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(fullPath), fileName);
                    }
                    else if (!cl.KeepExtension)
                    {
                        fileName = $"{System.IO.Path.GetFileNameWithoutExtension(fullPath)}{extension}";
                        screenFileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(fullPath), fileName);
                    }
                }

                Document.FullPath = screenFileName;
                Document.InitXamlDocument(screenFileName, ActiveLayer.XamlWriterFormatted()/*XamlWriter.Save(cv)*/);
                Document.NeedsSave = true;
                if (Document.SaveToFile(forceEncryption: isSourceEncrypted, bThrowOnError: true))
                {
                    if (listElementToReplace.Count == 0)
                    {
                        Console.WriteLine(Properties.Resource.CompiledScreenSuccessfullyCreated, cl.Screen);
                    }
                    else
                    {
                        Console.WriteLine(Properties.Resource.CompiledScreenPartiallyCreated, cl.Screen);
                    }
                }
                else
                {
                    Environment.ExitCode = -1;
                    Console.Error.WriteLine(Properties.Resource.CompiledScreenCannotBeSaved, cl.Screen);
                }

                if (Document.fileSystemProviderBase == null)
                {
                    if(!string.IsNullOrEmpty(cl.ScreenCompiledPath) || (cl.KeepExtension && !cl.OutputForTest))
                    {
                        if (!isSourceEncrypted && cl.CreateBaml)
                        {
                            try
                            {
                                CreateBaml(screenFileName);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                        else
                        {
                            var bamlFile = ScreenDocument.GetFileWithExt(screenFileName, ScreenDocument.bamlExt);
                            if (System.IO.File.Exists(bamlFile))
                            {
                                try
                                {
                                    System.IO.File.Delete(bamlFile);
                                }
                                catch (Exception ex)
                                {
                                    Console.Error.WriteLine(Properties.Resource.ErrorTryingToRemoveBmalFile, bamlFile, ex.Message);
                                }
                            }
                        }
                    }
                }
            }
        }

        void LoadRequiredAssemblies()
        {
            if (!String.IsNullOrEmpty(Properties.Settings.Default.RequiredAssemblies))
            {
                var array = Properties.Settings.Default.RequiredAssemblies.Split(';');
                foreach (var assembly in array)
                {
                    try
                    {
                        Assembly.Load(assembly);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        void FindControls(FrameworkElement cv, List<string> listElementToReplace, ScreenDocument document, Canvas activeLayer, bool updateName = false)
        {
            var contentControlList = cv.GetVisualChildrenOfType<ContentControl>().ToList(); 
            foreach (var contentControl in contentControlList)
            {
                string euname = null;
                FrameworkElement fe = null;
                try
                {
                    euname = document.GetEntityName(contentControl, bAdd: false);
                    if (listElementToReplace.Contains(euname))
                    {
                        bool excluded;
                        fe = ReplaceContainer(document, activeLayer, euname, contentControl, out excluded);
                        if (fe != null)
                        {
                            listElementToReplace.Remove(euname);
                            if (document.MapScreenEntities.ContainsKey(euname))
                                document.MapScreenEntities[euname].SetSourceSymbolLinkedResolved();
                            FindControls(fe, listElementToReplace, document, activeLayer);
                        }
                        else if(excluded)
                            throw new Exception(String.Format(Properties.Resource.Symbol3DNotSupported, euname));
                        else
                            throw new Exception(String.Format(Properties.Resource.NotFoundSymbol, WPFUtilities.CryptString.CryptString.DecryptString(document.MapScreenEntities[euname].SourceSymbolPath)));
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(Properties.Resource.ErrorCompilingSymbol, euname, cl.Screen, ex.Message);
                }
            }


        }

        FrameworkElement ReplaceContainer(ScreenDocument document, Canvas activeLayer, string key, ContentControl contentControl, out bool bExcluded)
        {
            bExcluded = false;

            if (contentControl == null)
                return null;

            string value = null;
            byte[] data = null;
            string settings = null;
            string sourceSymbolProvider = document.MapScreenEntities[key].SourceSymbolProvider;
            string symbolPath = document.MapScreenEntities[key].SourceSymbolPath;
            try
            {
                value = STRL.STRL.GetSymbolElement(sourceSymbolProvider, symbolPath, document.rootBase);
            }
            catch (Exception ex)
            {
            }

            try
            {
                data = STRL.STRL.GetSymbolElementData(sourceSymbolProvider, symbolPath, document.rootBase);
            }
            catch (Exception ex)
            {
            }

            try
            {
                settings = STRL.STRL.GetSymbolSettings(sourceSymbolProvider, symbolPath, document.rootBase);
            }
            catch (Exception ex)
            {
            }

            try
            {
                symbolPath = WPFUtilities.CryptString.CryptString.DecryptString(symbolPath);
            }
            catch (Exception)
            {
            }

            UIElement newuie = null;
            if (data != null)
                newuie = Utilities.WPF.XmlHelper.LoadBaml<UIElement>(data);
            else if (!String.IsNullOrEmpty(value))
                newuie = value.ReadUIElement();
            bool hasViewport = newuie?.GetVisualChildrenOfType<Viewport3D>().FirstOrDefault() != null;
            
            if (hasViewport)
            {
                bExcluded = true;
                return null;
            }

            List<String> postlistInners;
            document.MergeDocumentInnerXamlPropertiesBags(settings, key, out postlistInners);

            if (newuie != null && contentControl != null)
            {
                //clear names to all symbol's childrens
                (from FrameworkElement e in newuie.GetVisualChildrenOfType<FrameworkElement>()
                 where !String.IsNullOrEmpty(e.Name)
                 select e).ToList().ForEach(e => 
                 {
                     var name = e.Name;
                     if (String.IsNullOrEmpty(e.Uid))
                        e.Uid = name;
                     e.ClearValue(FrameworkElement.NameProperty);
                 });

                //add viewbox for project library symbols
                if (newuie is Viewbox && (!String.IsNullOrEmpty((newuie as Viewbox).Name) || !String.IsNullOrEmpty((newuie as Viewbox).Uid)))
                {
                    var viewbox = (newuie as Viewbox);
                    var child = viewbox.Child;
                    viewbox.Child = null;
                    var elementName = viewbox.Name;
                    if (String.IsNullOrEmpty(elementName))
                        elementName = viewbox.Uid;
                    var newViewbox = new Viewbox() { Uid = elementName, Child = child };
                    viewbox.Child = newViewbox;
                    viewbox.ClearValue(FrameworkElement.NameProperty);
                }

                bool bContentControlKept = false;
                int n = activeLayer.Children.IndexOf(contentControl);
                if (n < 0)
                {
                    var parent = contentControl.FindFirstParent<Panel>();
                    if (parent != null)
                    {
                        contentControl.Content = newuie;
                        bContentControlKept = true;
                    }
                    else
                    {
                        var decorator = contentControl.FindFirstParent<Decorator>();
                        if (decorator != null)
                        {
                            decorator.Child = newuie;
                        }
                        else
                        {
                            var content = contentControl.FindFirstParent<ContentControl>();
                            if (content != null)
                                content.Content = newuie;
                            else
                            {
                                content = contentControl.FindFirstAncestor<ContentControl>();
                                if (content != null)
                                    content.Content = newuie;
                            }
                        }
                    }
                    contentControl.ApplyTemplate();
                    contentControl.UpdateLayout();
                }
                else
                {
                    activeLayer.Children.Insert(n, newuie);
                    activeLayer.Children.Remove(contentControl);
                }

                //just add the element, it wasn't another Canvas that we
                //added to the dataobject as a container
                var fe = newuie as FrameworkElement;
                if (fe != null)
                {
                    fe.Width = contentControl.Width;
                    fe.Height = contentControl.Height;
                }
                if (!bContentControlKept)
                {
                    if (fe != null)
                    {
                        if (!String.IsNullOrEmpty(contentControl.Name))
                            fe.Name = contentControl.Name;
                        else if (!String.IsNullOrEmpty(contentControl.Uid))
                            fe.Uid = contentControl.Uid;

                        Utilities.WPF.DependencyObjectExtensions.UnregisterName(activeLayer, contentControl as FrameworkElement);
                        Utilities.WPF.DependencyObjectExtensions.RegisterName(activeLayer, fe as FrameworkElement, bIsNested: false);

                        fe.IsManipulationEnabled = contentControl.IsManipulationEnabled;
                        fe.RenderTransformOrigin = contentControl.RenderTransformOrigin;
                        if (contentControl.RenderTransform != null)
                            fe.RenderTransform = contentControl.RenderTransform;
                    }

                    InkCanvas.SetLeft(newuie, InkCanvas.GetLeft(contentControl));
                    InkCanvas.SetTop(newuie, InkCanvas.GetTop(contentControl));
                    Canvas.SetLeft(newuie, Canvas.GetLeft(contentControl));
                    Canvas.SetTop(newuie, Canvas.GetTop(contentControl));

                    Grid.SetColumn(newuie, Grid.GetColumn(contentControl));
                    Grid.SetRow(newuie, Grid.GetRow(contentControl));
                    Grid.SetColumnSpan(newuie, Grid.GetColumnSpan(contentControl));
                    Grid.SetRowSpan(newuie, Grid.GetRowSpan(contentControl));
                }

                //document.RemoveSourceProviderPath(key);
                bool bProblematic = Utilities.WPF.XmlHelper.CheckProblematicXamlWriter(fe, "");
                if (bProblematic)
                    document.UpdateProblematicXamlWriterProperties(fe, key, bDontUpdateSize: false);
                document.PostMergeDocumentInnerXamlPropertiesBags(activeLayer, settings, key);
            }

            return newuie as FrameworkElement;
        }

        void CreateBaml(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                var str = System.IO.File.ReadAllText(filePath);
                var f1 = ScreenDocument.GetFileWithExt(filePath, ScreenDocument.bamlExt);
                var res = BamlWriter.BamlWriter.Save(str);
                System.IO.File.WriteAllBytes(f1, res);
            }
        }

        void RemoveAllCompiledScreenFiles(string fullPath)
        {
            var path = Path.GetDirectoryName(fullPath);
            var dirInfo = new DirectoryInfo(path);
            if (dirInfo.Exists)
            {
                var searchFiles = String.Format("{0}.*", Path.GetFileName(ScreenDocument.GetFileWithExt(fullPath, ScreenDocument.compiledExt)));
                var files = dirInfo.GetFiles(searchFiles, SearchOption.TopDirectoryOnly);
                foreach (var file in files)
                {
                    try
                    {
                        File.Delete(file.FullName);
                    }
                    catch
                    { }
                }
            }
        }

        public void RemoveCompiledFiles(string fullPath)
        {
            RemoveAllCompiledScreenFiles(fullPath);
        }
    }
}
