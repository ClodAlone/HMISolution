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
using CustomWizardPlugin.ComponentService;
using VFS;
using System.Drawing;
using DocumentManager.ComponentService;
using System.Text;
using System.Xml.Linq;
using ScreenSettings;
using ScreenSettings.Entities;
using System.Runtime.Serialization;
using WPFUtilities;
using CommandManager;
using UFProjectManager;

namespace CustomWizardPlugin
{
    public enum Layout
    {
        Bottom = 0,
        Top = 1,
        Left = 2,
        Right = 3
    }

    public class LocalizedItem
    {
        public string Content { get; set; }
        public Layout Value { get; set; }
        public LocalizedItem(string content, Layout value)
        {
            Content = content;
            Value = value;
        }
        public override string ToString()
        {
            return Content;
        }
    }

    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ProjectWizardScreenTemplate : UserControl, IWizardElement
    {
        //ObservableCollection<FrameworkElement> listStyledObject = new ObservableCollection<FrameworkElement>();

        public string TemplateXaml { get; set; }
        public string defaultTemplateFilename { get; set; }
        public string defaultTopFilename { get; set; }
        public string defaultBottomFilename { get; set; }
        public string defaultLeftFilename { get; set; }
        public string defaultRightFilename { get; set; }
        public bool defaultScreenTemplate { get; set; }
        public bool clearScreenTemplate { get; set; }
        public string ProjectTempFolder { get; set; }
        bool bLoaded;
        static readonly double _DefWidthHeight = 100.0;
        UFProjectDocument parent;


        public ProjectWizardScreenTemplate()
        {
            InitializeComponent();

            //screenlistBox.ItemsSource = listStyledObject;
            defaultScreenTemplate = false;
            clearScreenTemplate = true;

            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;

                colorBrush.UIMsgBoxAlertService = colorTxtBrush.UIMsgBoxAlertService = CustomWizardPluginComponent.ProjectView.UIInterface;
                navBarLayoutType.ItemsSource = GetItemSource();
            };
        }

        List<LocalizedItem> GetItemSource()
        {
            List<LocalizedItem> items = new List<LocalizedItem>()
            {
                new LocalizedItem(Properties.Resources.NavBarBottom,Layout.Bottom),
                new LocalizedItem(Properties.Resources.NavBarTop,Layout.Top),
                new LocalizedItem(Properties.Resources.NavBarLeft,Layout.Left),
                new LocalizedItem(Properties.Resources.NavBarRight,Layout.Right),
            };
            return items;
        }

        public bool Execute()
        {
            parent = UFProjectDocument.FromFile(CustomWizardPluginComponent.ProjectUri.GetPathString(), CustomWizardPluginComponent.ProjectView.projectManagerService as UFProjectManager.ComponentService.UFProjectManagerComponent);
            if (parent == null)
                return true;
            if (spineditScreenNo.Value > 0)
            {
                String screenName = $"{(CustomWizardPluginComponent.ProjectView.screenManagerService as IDocumentManager).TypeLabel}{1}";
                String path = GetScreenPath(screenName);
                CreateDefaultDocument(path);
                if ((bool)checkboxEnTitle.IsChecked && !(bool)checkboxEnTemplate.IsChecked)
                    AddScreenTitle(path);
                
                if ((bool)checkboxEnTemplate.IsChecked && !string.IsNullOrEmpty(TemplateXaml))
                    AddTemplateNavBar(path);
                else if ((bool)checkboxCreateNavBar.IsChecked && !(bool)checkboxEnTemplate.IsChecked)
                    AddStandardNavBar(path);

                for (int i = 2; i <= spineditScreenNo.Value; i++)
                {
                    String newScreenName = $"{(CustomWizardPluginComponent.ProjectView.screenManagerService as IDocumentManager).TypeLabel}{i}";
                    String newPath = GetScreenPath(newScreenName);
                    ScreenDocument.CopyFile(path, newPath, true, parent, false);

                    if ((bool)checkboxEnTitle.IsChecked && !(bool)checkboxEnTemplate.IsChecked)
                        AddScreenTitle(newPath);
                }
            }
            parent.Dispose();
            
            return false;
        }

        void AddScreenTitle(string path)
        {
            using (ScreenDocument doc = ScreenDocument.FromFile(path, parent))
            {
                if (doc == null)
                    return;
                Canvas cnvs = doc.GetCurrentXamlDocument();

                TextBlock txtBlock = (from t in cnvs.GetChildrenOfType<TextBlock>()
                                      where t.Name == Properties.Settings.Default.ScreenTitleControlName
                                      select t).FirstOrDefault();
                if (txtBlock != null)
                    txtBlock.Text = System.IO.Path.GetFileNameWithoutExtension(path);
                else
                {
                    txtBlock = new TextBlock();
                    txtBlock.Text = System.IO.Path.GetFileNameWithoutExtension(path);
                    txtBlock.FontSize = (double)spineditScreenTitleFontSize.Value;
                    txtBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    txtBlock.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    txtBlock.Name = Properties.Settings.Default.ScreenTitleControlName;
                    AddControlOnCanvas(doc, cnvs, txtBlock as FrameworkElement, 0, 0, null, null);
                }

                doc.ResolveProblematicXamlOnCanvas(cnvs);
                doc.SetCurrentXamlDocument(XamlWriter.Save(cnvs));
                doc.NeedsSave = true;
                doc.SaveCurrentDocument(cnvs);
            }
        }

        void AddTemplateNavBar(string path)
        {
            string navBarScreenName = Properties.Settings.Default.EmbeddedNavBarScreenName;
            string navBarControlName = Properties.Settings.Default.NavBarControlName;
            string navBarButtonName = Properties.Settings.Default.NavBarButtonName;
            using (ScreenDocument doc = ScreenDocument.FromFile(path, parent))
            {
                if (doc == null)
                    return;
                Canvas cnvs = doc.GetCurrentXamlDocument();

                doc.LoadResources(cnvs);
                ResourceDictionaryExtensions.AddCommonResources(cnvs);
                doc.RefreshEntityStyleBinding(cnvs, true);

                FrameworkElement navBar = (from element in cnvs.GetChildrenOfType<FrameworkElement>()
                                           where element.Name == navBarControlName
                                           select element).FirstOrDefault();
                FrameworkElement navBarButton = (from element in cnvs.GetChildrenOfType<FrameworkElement>()
                                                 where element.Name == navBarButtonName
                                                 select element).FirstOrDefault();
                if (navBar != null && navBarButton != null)
                {
                    string xaml = navBarButton.XamlWriterFormatted();

                    double lenght;
                    System.Windows.Size size = new System.Windows.Size(navBar.Width, navBar.Height);
                    System.Windows.Size embeddedSize = new System.Windows.Size(navBar.Width, navBar.Height);
                    UpdateProblematicSize(size, doc, navBar.Name);
                    if (size.Width > size.Height)
                    {
                        lenght = (double)spineditScreenNo.Value * (Properties.Settings.Default.NavBarButtonMargin + navBarButton.Width) + Properties.Settings.Default.NavBarButtonMargin;
                        if (lenght > navBar.Width)
                            size.Width = lenght;
                    }
                    else
                    {
                        lenght = (double)spineditScreenNo.Value * (Properties.Settings.Default.NavBarButtonMargin + navBarButton.Height) + Properties.Settings.Default.NavBarButtonMargin;
                        if (lenght > (double)navBar.Height)
                            size.Height = lenght;
                    }

                    System.Windows.Media.Brush navBarBackground = navBar is Control ? (navBar as Control).Background :
                        navBar is Border ? (navBar as Border).Background :
                        navBar is Shape ? (navBar as Shape).Fill : System.Windows.Media.Brushes.Transparent;

                    double x = Canvas.GetLeft(navBar);
                    double y = Canvas.GetTop(navBar);

                    CreateTemplateNavBar(doc, navBarBackground, x, y, size, navBarButton);

                    doc.RemoveEntity(navBar);
                    doc.RemoveEntity(navBarButton);
                    cnvs.Children.Remove(navBar);
                    cnvs.Children.Remove(navBarButton);

                    AddEmbeddedScreen(doc, embeddedSize, x, y, cnvs);

                    doc.ResolveProblematicXamlOnCanvas(cnvs);
                    doc.SetCurrentXamlDocument(XamlWriter.Save(cnvs));
                    doc.NeedsSave = true;
                    doc.SaveCurrentDocument(cnvs);

                    SetProjectMainScreen();

                }
            }
        }

        void AddEmbeddedScreen(ScreenDocument doc, System.Windows.Size size, double left, double top, Canvas cnvs)
        {
            ContentControl contentControl = new ContentControl()
            {
                Name = Properties.Settings.Default.NavBarControlName,
                Width = size.Width,
                Height = size.Height,
                RenderTransformOrigin = new System.Windows.Point(0.5, 0.5)
            };
            string path;
            var toolboxXaml = CustomWizardPluginComponent.ProjectView.toolboxManager.GetCodeFromHash(Properties.Settings.Default.EmbeddedControl, 
                CustomWizardPluginComponent.ProjectView.screenManagerService as IDocumentManager, out path);

            var entity = AddControlOnCanvas(doc, cnvs, contentControl as FrameworkElement, left, top, null, toolboxXaml);
            String newScreenName = $"{(CustomWizardPluginComponent.ProjectView.screenManagerService as IDocumentManager).TypeLabel}/" +
                                   $"{Properties.Settings.Default.NavBarFolder}/{Properties.Settings.Default.EmbeddedNavBarScreenName}" +
                                   $"{(CustomWizardPluginComponent.ProjectView.screenManagerService as IDocumentManager).FileType}";
            if (entity.MapProblematicXamlWriterProperties == null)
                entity.MapProblematicXamlWriterProperties = new ScreenSettings.Entities.ProblematicXamlWriterProperties();
            AddProblematicProperty(entity, "Screens", System.Windows.Markup.XamlWriter.Save(new ScreenManager.SpecialObjects.ScreenList() { newScreenName }));

            doc.ResolveProblematicXamlOnCanvas(cnvs);
            doc.SetCurrentXamlDocument(XamlWriter.Save(cnvs));
            doc.NeedsSave = true;
            doc.SaveCurrentDocument(cnvs);
        }

        private void SetProjectMainScreen()
        {
            UFProjectDocument docParent = parent as UFProjectDocument;
            docParent.StartType = StartType.MainScreen;
            var mainScreenName = $"{(CustomWizardPluginComponent.ProjectView.screenManagerService as IDocumentManager).TypeLabel}{1}";
            docParent.MainScreen = new Uri(mainScreenName, UriKind.RelativeOrAbsolute);
            docParent.NeedsSave = false;
            docParent.SaveToFile(true);
        }

        void UpdateProblematicSize(System.Windows.Size size, ScreenDocument doc, string controlName)
        {
            if (doc.MapScreenEntities.ContainsKey(controlName) &&
                doc.MapScreenEntities[controlName].MapProblematicXamlWriterProperties != null)
            {
                if (doc.MapScreenEntities[controlName].MapProblematicXamlWriterProperties.ContainsKey("Height"))
                {
                    var value = doc.MapScreenEntities[controlName].MapProblematicXamlWriterProperties["Height"];
                    try
                    {
                        using (var reader = new StringReader(value))
                        {
                            // object obj = s.Deserialize(reader);
                            using (var textReader = new XmlTextReader(reader))
                            {
                                var obj = System.Windows.Markup.XamlReader.Load(textReader);
                                size.Height = Convert.ToDouble(obj);
                            }
                        }
                    }
                    catch
                    {

                    }
                }
                if (doc.MapScreenEntities[controlName].MapProblematicXamlWriterProperties.ContainsKey("Width"))
                {
                    var value = doc.MapScreenEntities[controlName].MapProblematicXamlWriterProperties["Width"];
                    try
                    {
                        using (var reader = new StringReader(value))
                        {
                            // object obj = s.Deserialize(reader);
                            using (var textReader = new XmlTextReader(reader))
                            {
                                var obj = System.Windows.Markup.XamlReader.Load(textReader);
                                size.Width = Convert.ToDouble(obj);
                            }
                        }
                    }
                    catch
                    {

                    }
                }
            }
        }

        void CreateTemplateNavBar(ScreenDocument sourceDocument, System.Windows.Media.Brush navBarBackground, double navX, double navY, System.Windows.Size size, FrameworkElement navButton)
        {
            String newScreenName = $"{Properties.Settings.Default.NavBarFolder}\\{Properties.Settings.Default.EmbeddedNavBarScreenName}";
            String path = GetScreenPath(newScreenName, true);
            if (string.IsNullOrEmpty(path))
                return;

            using (ScreenDocument doc = CreateSmartScreenDocument(path, size, navBarBackground))
            {
                Canvas cnvs = doc.GetCurrentXamlDocument();
                double x = Canvas.GetLeft(navButton) - navX;
                double y = Canvas.GetTop(navButton) - navY;
                System.Windows.Size buttonSize = new System.Windows.Size(navButton.Width, navButton.Height);
                UpdateProblematicSize(buttonSize, sourceDocument, navButton.Name);
                double buttonWidth = buttonSize.Width;
                double buttonHeight = buttonSize.Height;
                string xaml = navButton.XamlWriterFormatted();
                IDictionary<String, String> renamed = new Dictionary<String, String>();
                for (int i = 1; i <= spineditScreenNo.Value; i++)
                {
                    Object content = xaml.ReadUIElement();
                    UIElement uie = content as UIElement;
                    if (uie != null)
                    {
                        var fe = uie as FrameworkElement;

                        var screenName = $"{(CustomWizardPluginComponent.ProjectView.screenManagerService as IDocumentManager).TypeLabel}{i}";
                        var buttonName = $"_NavButton{i}";
                        fe.Name = buttonName;
                        cnvs.Children.Add(uie);
                        doc.MapScreenEntities.Add(buttonName, new ScreenEntity());

                        if (Utilities.WPF.XmlHelper.CheckProblematicXamlWriter(uie, uie.XamlWriterFormatted()))
                        {
                            renamed = Utilities.WPF.DependencyObjectExtensions.RegisterName(cnvs, fe, true, false);
                            doc.SetProblematicXaml(uie as FrameworkElement, xaml);
                            doc.AddDynamicEntity(fe);
                            doc.MapScreenEntities[buttonName].CopyAll(sourceDocument.MapScreenEntities[navButton.Name]);
                            doc.MapScreenEntities[buttonName].Entity = fe;
                            try
                            {
                                doc.LoadRepositoryItem(this, cnvs, buttonName, bAnimate: false);
                            }
                            catch (Exception ex)
                            {
                            }
                            doc.UpdateProblematicXamlWriterProperties(fe, buttonName);
                        }
                        else
                        {
                            renamed = Utilities.WPF.DependencyObjectExtensions.RegisterName(cnvs, fe, true, false);
                            doc.AddDynamicEntity(fe);
                            doc.MapScreenEntities[buttonName].CopyAll(sourceDocument.MapScreenEntities[navButton.Name]);
                            doc.MapScreenEntities[buttonName].Entity = fe;

                            if (!String.IsNullOrEmpty(sourceDocument.MapScreenEntities[navButton.Name].ProblematicXaml))
                                Utilities.WPF.XmlHelper.SetProblematicXamlWriter(fe, sourceDocument.MapScreenEntities[navButton.Name].ProblematicXaml);
                            try
                            {
                                doc.LoadRepositoryItem(this, cnvs, buttonName, bAnimate: false);
                            }
                            catch (Exception ex)
                            {
                            }
                        }

                        Canvas.SetTop(fe, y);
                        Canvas.SetLeft(fe, x);
                        ScreenEntity newScreenEntity = doc.MapScreenEntities[buttonName];
                        AddOpenScreenCommand(newScreenEntity, screenName);

                        if (size.Width > size.Height)
                            x += Properties.Settings.Default.NavBarButtonMargin + buttonWidth;
                        else
                            y += Properties.Settings.Default.NavBarButtonMargin + buttonHeight;

                    }
                }

                doc.AddListAssembly(sourceDocument.ListAssemblies);
                doc.ResolveProblematicXamlOnCanvas(cnvs);
                doc.SetCurrentXamlDocument(XamlWriter.Save(cnvs));
                doc.NeedsSave = true;
                doc.SaveCurrentDocument(cnvs);
            }
        }

        void AddStandardNavBar(string path)
        {
            double lenght;
            System.Windows.Size size = new System.Windows.Size();
            System.Windows.Size embeddedSize = new System.Windows.Size();
            double embeddedTop = 0;
            double embeddedLeft = 0;
            switch ((navBarLayoutType.SelectedItem as LocalizedItem).Value)
            {
                case Layout.Bottom:
                    size.Height = Properties.Settings.Default.TopBottomBarHeight;
                    lenght = (double)spineditScreenNo.Value * (Properties.Settings.Default.NavBarButtonMargin + Properties.Settings.Default.NavBarButtonWidth) + Properties.Settings.Default.NavBarButtonMargin;
                    if (lenght > (double)spineditScreenWidth.Value)
                        size.Width = lenght;
                    else
                        size.Width = (double)spineditScreenWidth.Value;

                    embeddedSize.Width = (double)spineditScreenWidth.Value; 
                    embeddedSize.Height = size.Height;
                    embeddedLeft = 0;
                    embeddedTop = (double)spineditScreenHeight.Value - size.Height - Properties.Settings.Default.NavBarButtonMargin;
                    break;
                case Layout.Top:
                    size.Height = Properties.Settings.Default.TopBottomBarHeight;
                    lenght = (double)spineditScreenNo.Value * (Properties.Settings.Default.NavBarButtonMargin + Properties.Settings.Default.NavBarButtonWidth) + Properties.Settings.Default.NavBarButtonMargin;
                    if (lenght > (double)spineditScreenWidth.Value)
                        size.Width = lenght;
                    else
                        size.Width = (double)spineditScreenWidth.Value;

                    embeddedSize.Width = (double)spineditScreenWidth.Value;
                    embeddedSize.Height = size.Height;
                    embeddedLeft = 0;
                    embeddedTop = 0 + Properties.Settings.Default.NavBarButtonMargin;
                    break;
                case Layout.Left:
                    size.Width = Properties.Settings.Default.LeftRightBarWidth;
                    lenght = (double)spineditScreenNo.Value * (Properties.Settings.Default.NavBarButtonMargin + Properties.Settings.Default.NavBarButtonHeight) + Properties.Settings.Default.NavBarButtonMargin;
                    if (lenght > (double)spineditScreenHeight.Value)
                        size.Height = lenght;
                    else
                        size.Height = (double)spineditScreenHeight.Value;

                    embeddedSize.Width = size.Width;
                    embeddedSize.Height = (double)spineditScreenHeight.Value;
                    embeddedLeft = Properties.Settings.Default.NavBarButtonMargin;
                    embeddedTop = 0;
                    break;
                case Layout.Right:
                    size.Width = Properties.Settings.Default.LeftRightBarWidth;
                    lenght = (double)spineditScreenNo.Value * (Properties.Settings.Default.NavBarButtonMargin + Properties.Settings.Default.NavBarButtonHeight) + Properties.Settings.Default.NavBarButtonMargin;
                    if (lenght > (double)spineditScreenHeight.Value)
                        size.Height = lenght;
                    else
                        size.Height = (double)spineditScreenHeight.Value;

                    embeddedSize.Width = size.Width;
                    embeddedSize.Height = (double)spineditScreenHeight.Value;
                    embeddedLeft = (double)spineditScreenWidth.Value - size.Width - Properties.Settings.Default.NavBarButtonMargin;
                    embeddedTop = 0;
                    break;
                default:
                    break;
            }

            CreateStandardNavBar(size);

            using (ScreenDocument doc = ScreenDocument.FromFile(path, parent))
            {
                if (doc == null)
                    return;
                Canvas cnvs = doc.GetCurrentXamlDocument();

                AddEmbeddedScreen(doc, embeddedSize, embeddedLeft, embeddedTop, cnvs);
            }

            SetProjectMainScreen();
        }

        void CreateStandardNavBar(System.Windows.Size size)
        {
            String newScreenName = $"{Properties.Settings.Default.NavBarFolder}\\{Properties.Settings.Default.EmbeddedNavBarScreenName}";
            string path = GetScreenPath(newScreenName, true);
            if (string.IsNullOrEmpty(path))
                return;

            using (ScreenDocument doc = CreateSmartScreenDocument(path, size, colorNavBarBackBrush.SelectedBrush ?? System.Windows.Media.Brushes.Transparent))
            {
                Canvas cnvs = doc.GetCurrentXamlDocument();
                double x = Properties.Settings.Default.NavBarButtonMargin;
                double y = Properties.Settings.Default.NavBarButtonMargin;
                string symbolPath = null;
                string fullPath;
                var toolboxXaml = CustomWizardPluginComponent.ProjectView.toolboxManager.GetCodeFromHash(Properties.Settings.Default.StandardNavBarButton, 
                    CustomWizardPluginComponent.ProjectView.screenManagerService as IDocumentManager, out fullPath);
                if (!string.IsNullOrEmpty(toolboxXaml))
                    symbolPath = CustomWizardPluginComponent.ProjectView.toolboxManager.GetCurrentSourceSymbolPath(fullPath);

                if (!string.IsNullOrEmpty(symbolPath))
                {
                    for (int i = 1; i <= spineditScreenNo.Value; i++)
                    {
                        var screenName = $"{(CustomWizardPluginComponent.ProjectView.screenManagerService as IDocumentManager).TypeLabel}{i}";
                        var buttonName = $"{Properties.Settings.Default.NavBarButtonName}{i}";
                        ContentControl contentControl = new ContentControl()
                        {
                            Name = buttonName,
                            Width = Properties.Settings.Default.NavBarButtonWidth,
                            Height = Properties.Settings.Default.NavBarButtonHeight,
                            RenderTransformOrigin = new System.Windows.Point(0.5, 0.5)
                        };

                        var entity = AddControlOnCanvas(doc, cnvs, contentControl as FrameworkElement, x, y, symbolPath, null);
                        AddOpenScreenCommand(entity, screenName);
                        if (entity.MapProblematicXamlWriterProperties == null)
                            entity.MapProblematicXamlWriterProperties = new ScreenSettings.Entities.ProblematicXamlWriterProperties();
                        var navBarButtonBackground = colorNavBarButtonBrush.SelectedBrush != null ? colorNavBarButtonBrush.SelectedBrush :
                            templateButton.Background;
                        var navBarButtonForeground = colorNavBarButtonForeBrush.SelectedBrush != null ? colorNavBarButtonForeBrush.SelectedBrush :
                            templateButton.Foreground;

                        AddProblematicProperty(entity, "Background", System.Windows.Markup.XamlWriter.Save(navBarButtonBackground));
                        AddProblematicProperty(entity, "Foreground", System.Windows.Markup.XamlWriter.Save(navBarButtonForeground));

                        if ((navBarLayoutType.SelectedItem as LocalizedItem).Value == Layout.Bottom ||
                            (navBarLayoutType.SelectedItem as LocalizedItem).Value == Layout.Top)
                            x += Properties.Settings.Default.NavBarButtonMargin + Properties.Settings.Default.NavBarButtonWidth;
                        else
                            y += Properties.Settings.Default.NavBarButtonMargin + Properties.Settings.Default.NavBarButtonHeight;
                    }
                }

                doc.ResolveProblematicXamlOnCanvas(cnvs);
                doc.SetCurrentXamlDocument(XamlWriter.Save(cnvs));
                doc.NeedsSave = true;
                doc.SaveCurrentDocument(cnvs);
            }
        }

        private void AddProblematicProperty(ScreenEntity entity, string name, string value)
        {
            if (entity.MapProblematicXamlWriterProperties == null)
                entity.MapProblematicXamlWriterProperties = new ScreenSettings.Entities.ProblematicXamlWriterProperties();
            if (entity.MapProblematicXamlWriterProperties.ContainsKey(name))
                entity.MapProblematicXamlWriterProperties[name] = value;
            else 
                entity.MapProblematicXamlWriterProperties.Add(name, value);
        }

        string GetScreenPath(string screenName, bool addNavBarFolder = false)
        {
            String path = string.Empty;

            if (CustomWizardPluginComponent.NewProject.UseFileSystemProvider)
            {
                using (var fileSystemProvider = new DataSourceFileSystemProvider("") { ConnectionString = CustomWizardPluginComponent.wizard.PathAndType.ConnectionString })
                {
                    var parentfolder = String.Format("{0}\\{1}", parent.ProjectFolder, (CustomWizardPluginComponent.ProjectView.screenManagerService as IDocumentManager).TypeLabel);
                    if (String.IsNullOrEmpty(parent.ProjectFolder))
                        parentfolder = (CustomWizardPluginComponent.ProjectView.screenManagerService as IDocumentManager).TypeLabel;
                    var fmfPath = new FileManagerFolder(fileSystemProvider, parentfolder);
                    path = String.Format("{0}\\{1}{2}", fmfPath, screenName, (CustomWizardPluginComponent.ProjectView.screenManagerService as IDocumentManager).FileType);
                    if(addNavBarFolder)
                        fileSystemProvider.CreateFolder(fmfPath, Properties.Settings.Default.NavBarFolder);
                }

            }
            else
            {
                var UriPath = String.Format("{0}\\{1}\\{2}", System.IO.Path.GetDirectoryName(CustomWizardPluginComponent.ProjectUri.LocalPath), System.IO.Path.GetFileNameWithoutExtension(CustomWizardPluginComponent.ProjectUri.LocalPath), (CustomWizardPluginComponent.ProjectView.screenManagerService as IDocumentManager).TypeLabel);
                path = String.Format("{0}\\{1}{2}", UriPath, screenName, (CustomWizardPluginComponent.ProjectView.screenManagerService as IDocumentManager).FileType);
                if (addNavBarFolder)
                    System.IO.Directory.CreateDirectory($"{UriPath}\\{Properties.Settings.Default.NavBarFolder}");
            }
            return path;
        }

        void CreateDefaultDocument(string path)
        {
            if (defaultScreenTemplate && System.IO.File.Exists(defaultTemplateFilename))
            {
                if (parent != null && parent.fileSystemProviderBase != null)
                {
                    if (!parent.fileSystemProviderBase.Exists(new FileManagerFile(parent.fileSystemProviderBase, path)))
                    {
                        var sourcesettingsFileName = ScreenDocument.GetSettingsFileName(defaultTemplateFilename);
                        var destsettingsFileName = ScreenDocument.GetSettingsFileName(path);
                        if (File.Exists(sourcesettingsFileName))
                        {
                            parent.fileSystemProviderBase.UploadFile(null, destsettingsFileName, File.ReadAllBytes(sourcesettingsFileName));
                        }

                        if (File.Exists(defaultTemplateFilename))
                        {
                            var text = File.ReadAllText(defaultTemplateFilename);
                            var data = System.Text.Encoding.Unicode.GetBytes(text);
                            parent.fileSystemProviderBase.UploadFile(null, path, data);
                        }
                    }
                }
                else if (!File.Exists(path))
                    ScreenDocument.CopyFile(defaultTemplateFilename, path, true, parent, bUploading: false);
            }
            else
            {
                System.Windows.Media.Brush backBrush = System.Windows.Media.Brushes.White;
                if (colorBrush != null && colorBrush.SelectedBrush != null)
                    backBrush = colorBrush.SelectedBrush;
                else
                {
                    var owner = this.FindParent<Window>() ?? Application.Current.MainWindow;
                    backBrush = owner.Background;
                }

                using (ScreenDocument doc = CreateSmartScreenDocument(path, new System.Windows.Size((double)spineditScreenWidth.Value, (double)spineditScreenHeight.Value), backBrush))
                {
                    Canvas cnvs = doc.GetCurrentXamlDocument();
                    doc.ResolveProblematicXamlOnCanvas(cnvs);
                    doc.SetCurrentXamlDocument(XamlWriter.Save(cnvs));
                    doc.NeedsSave = true;
                    doc.SaveCurrentDocument(cnvs);
                }
            }
        }

        public ScreenDocument CreateSmartScreenDocument(string fullPath, System.Windows.Size size, System.Windows.Media.Brush background)
        {
            ScreenDocument doc = new ScreenDocument();
            Canvas cnvs = new Canvas();
            cnvs.Background = background;
            cnvs.Width = size.Width;
            cnvs.Height = size.Height;
            doc.Width = size.Width;
            doc.Height = size.Height;
            doc.Parent = parent;
            doc.InitXamlDocument(fullPath, XamlWriter.Save(cnvs));
            return doc;
        }

        private ScreenEntity AddControlOnCanvas(ScreenDocument doc, Canvas cnvs, FrameworkElement fe, double left, double top, string symbolPath, string sProblematicXaml)
        {
            cnvs.Children.Add(fe);
            Canvas.SetTop(fe, top);
            Canvas.SetLeft(fe, left);
            ScreenEntity entity = new ScreenEntity(fe);
            doc.MapScreenEntities.Add(fe.Name, entity);
            if(!string.IsNullOrEmpty(symbolPath))
            {
                doc.SetSourceProviderPath(fe, null, symbolPath);
                entity.Entity = fe;
            }
            else if(!string.IsNullOrEmpty(sProblematicXaml))
            {
                entity.Entity = fe;
                entity.ProblematicXaml = sProblematicXaml;
            }
            return entity;
        }

        void AddOpenScreenCommand(ScreenEntity entity, string screenName)
        {
            if (!string.IsNullOrEmpty(screenName))
            {
                OpenScreenCommand openScreenCmd = new OpenScreenCommand();
                openScreenCmd.ScreenName = new Uri($"{(CustomWizardPluginComponent.ProjectView.screenManagerService as IDocumentManager).TypeLabel}/{screenName}{(CustomWizardPluginComponent.ProjectView.screenManagerService as IDocumentManager).FileType}", UriKind.RelativeOrAbsolute);
                entity.CommandList = new CommandManager.CommandManagerList() { openScreenCmd };

                var scontent = System.Windows.Markup.XamlWriter.Save(screenName);
                AddProblematicProperty(entity, "Content", scontent);
            }
        }

        private void screenTemplate_Click(object sender, RoutedEventArgs e)
        {
            var usercontrol = CustomWizardPluginComponent.ProjectView.screenManagerService.GetNewScreenTemplateControl();

            GeneralDialogContent dlg = new GeneralDialogContent(usercontrol)
            {
                Owner = this.FindParent<Window>() ?? Application.Current.MainWindow,
                Title = Properties.Resources.ScreenTemplateTitle,
                HelpLink = "CustomWizard_ScreenTemplate"
            };

            if (dlg.ShowDialog() != true || string.IsNullOrEmpty((string)usercontrol.DataContext))
            {
                defaultScreenTemplate = false;
                clearScreenTemplate = true;
                defaultTemplateFilename = string.Empty;
           //     backColorGrid.IsEnabled = true;
                return;
            }

            if (!ManageTumb((usercontrol as NewScreenType)))
            {
                defaultScreenTemplate = true;
                clearScreenTemplate = false;
            }
           // backColorGrid.IsEnabled = false;
        
        }

        private void clearTemplate_Click(object sender, RoutedEventArgs e)
        {
            ClearTemplate();
        }

        bool ManageTumb(NewScreenType template)
        {
            try
            {
                if (template == null && !string.IsNullOrEmpty(TemplateXaml))
                {
                    StringReader stringReader = new StringReader(TemplateXaml);
                    using (XmlReader xmlReader = XmlReader.Create(stringReader))
                    {
                        Object obj = XamlReader.Load(xmlReader);
                        FrameworkElement element = obj as FrameworkElement;
                        if (element != null)
                        {
                            //listStyledObject.Clear();
                            //listStyledObject.Add(element);
                            thumb.Child = element;
                            spineditScreenWidth.Value = (decimal)element.Width;
                            spineditScreenHeight.Value = (decimal)element.Height;
                            return false;
                        }
                    }
                    defaultTemplateFilename = string.Empty;
                }
                else
                {
                    var sourceFileName = template.sourcefileName;
                    TemplateXaml = (string)template.DataContext;
                    defaultTemplateFilename = sourceFileName;
                    if (File.Exists(System.IO.Path.ChangeExtension(sourceFileName, "png")))
                    {
                        System.Windows.Controls.Image element = new System.Windows.Controls.Image()
                        {
                            Source = new BitmapImage(new Uri(System.IO.Path.ChangeExtension(sourceFileName, "png")))
                        };
                        //listStyledObject.Clear();
                        //listStyledObject.Add(element);
                        thumb.Child = element;
                        //spineditScreenWidth.Value = (decimal)element.Width;
                        //spineditScreenHeight.Value = (decimal)element.Height;
                        return false;
                    }
                    else
                    {
                        StringReader stringReader = new StringReader((string)template.DataContext);
                        using (XmlReader xmlReader = XmlReader.Create(stringReader))
                        {
                            Object obj = XamlReader.Load(xmlReader);
                            FrameworkElement element = obj as FrameworkElement;
                            if (element != null)
                            {
                                //listStyledObject.Clear();
                                //listStyledObject.Add(element);
                                thumb.Child = element;
                                spineditScreenWidth.Value = (decimal)element.Width;
                                spineditScreenHeight.Value = (decimal)element.Height;
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CustomWizardPluginComponent.ProjectView.UIInterface.ShowError(ex.Message);
                return true;
            }
            return true;
        }

        private void WithoutTemplate_Checked(object sender, RoutedEventArgs e)
        {
            ClearTemplate();
        }

        void ClearTemplate()
        {
            ManageTumb(null);

            defaultScreenTemplate = false;
            clearScreenTemplate = true;
        }

        private void spineditScreenNo_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            bool enControls = spineditScreenNo.Value > 0;
            screenOption.IsEnabled = enControls;
            screenOption1.IsEnabled = enControls;
        }
    }
}
