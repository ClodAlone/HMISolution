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
using DevExpress.Xpf.LayoutControl;
using DevExpress.Xpf.Core;
using System.IO.IsolatedStorage;
using System.Xml;
using System.IO;
using System.Reflection;
using DocumentManager.ComponentService;
using Utilities;
using System.Windows.Threading;
using ScreenSettings;
using ScreenManager.ComponentService;

namespace ScreenManager
{
    /// <summary>
    /// Interaction logic for TileViewer.xaml
    /// </summary>
    public partial class TileViewer : UserControl, IDisposable
    {
        #region Declarations

        bool bLoaded;
        readonly ScreenManagerComponent ScreenComponent;
        readonly List<UserControl> listViewers = new List<UserControl>();
        readonly ScreenTransitionViewer Container;
        readonly IDocument DocParent;
        readonly IScreenController screenController;
        readonly IScreenController backController;
        double timeSpan = 1500;
        bool bCultureChanged;

        #endregion

        #region Command Manager
        Dictionary<String, ITile> mapCommands = new Dictionary<String, ITile>(); 

        void AddCommand(String command, ITile tile)
        {
            if (!mapCommands.ContainsKey(command))
                mapCommands.Add(command, tile);
        }

        public List<String> GetCommandList()
        {
            return mapCommands.Keys.ToList();
        }
        
        public void ExecuteCommand(String command)
        {
            if (mapCommands.ContainsKey(command))
                mapCommands[command].Click();
        }
        #endregion

        public TileViewer(ScreenManagerComponent screenComponent, IScreenController screencont, 
                    ScreenTransitionViewer container, IDocument DocumentParent, 
                IScreenController backcont = null)
        {
            InitializeComponent();

            Container = container;
            DocParent = DocumentParent;
            ScreenComponent = screenComponent;
            screenController = screencont;
            backController = backcont;

            timeSpan = 1500;
            // Loaded += (o, e) =>
                {
                    if (!bLoaded)
                    {
                        bLoaded = true;

                        if (ScreenComponent.StringEditor != null)
                            ScreenComponent.StringEditor.CultureChanged += StringEditor_CultureChanged;

                        FillTiles();

                        tileLayoutControl.ItemPositionChanged += (ob, ev) =>
                        {
                            Dispatcher.BeginInvokeIfRequired(() => SaveLayout(screenController.GetTitle()), DispatcherPriority.ApplicationIdle);
                        };
                    }
                };

                GotFocus += (o, e) =>
                    {
                        if (bCultureChanged)
                        {
                            bCultureChanged = false;
                            FillTiles();
                        }
                    };

            //Unloaded += (o, e) =>
            //{
            //    if (bLoaded)
            //    {
            //        SaveLayout(screenController.GetTitle());
            //        bLoaded = false;
            //    }
            //};
        }

        void StringEditor_CultureChanged(object sender, EventArgs e)
        {
#if !WINDOWS_UWP
            Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
#else
            RunOnUIThread.Run(() =>
#endif
            {
                if (IsFocused)
                    FillTiles();
                else
                    bCultureChanged = true;
            });
        }

        void FillTiles()
        {
            using (var cursor = new WaitCursor())
            {
                CleanupTiles();

                var map = ScreenComponent.StringEditor.GetListStringForCulture(DocParent,
                                        ScreenComponent.StringEditor.GetActiveCulture(DocParent));

                if (backController != null)
                {
                    var tile = new Tile()
                    {
                        Name = Utilities.WPF.DependencyObjectExtensions.AdaptName(backController.GetTitle()),
                        Header = map != null && map.ContainsKey(backController.GetTitle()) ? map[backController.GetTitle()] : backController.GetTitle(),
                        AnimateContentChange = true,
                        ContentChangeInterval = TimeSpan.FromMilliseconds(timeSpan),
                        Background = new SolidColorBrush(backController.GetIdentityColor())
                    };
                    bool bLive = false;
                    switch (backController.TileSize())
                    {
                        case DocumentManager.ComponentService.TileSize.Small:
                            tile.Size = DevExpress.Xpf.LayoutControl.TileSize.Small;
                            break;
                        case DocumentManager.ComponentService.TileSize.ExtraSmall:
                            tile.Size = DevExpress.Xpf.LayoutControl.TileSize.ExtraSmall;
                            break;
                        case DocumentManager.ComponentService.TileSize.Large:
                            tile.Size = DevExpress.Xpf.LayoutControl.TileSize.Large;
                            break;
                        case DocumentManager.ComponentService.TileSize.Live:
                            bLive = true;
                            tile.Size = DevExpress.Xpf.LayoutControl.TileSize.ExtraLarge;
                            break;
                        case DocumentManager.ComponentService.TileSize.ExtraLarge:
                            tile.Size = DevExpress.Xpf.LayoutControl.TileSize.ExtraLarge;
                            break;
                    }
                    tile.Click += (ob, ev) =>
                    {
                        Container.OpenOrActivate(backController);
                    };

                    var contentSource = new FrameworkElements();
                    contentSource.Add(new TextBlock() { Text = tile.Header as String, FontFamily = new FontFamily("Segoe UI Light"), FontSize = 32, HorizontalAlignment = System.Windows.HorizontalAlignment.Center, VerticalAlignment = System.Windows.VerticalAlignment.Center, TextWrapping = TextWrapping.Wrap });

                    var file = backController.GetIconSource();
                    if (File.Exists(file))
                    {
                        try
                        {
                            var img = new BitmapImage();
                            var image = new Image()
                            {
                                Margin = new Thickness(20)
                            };

                            Dispatcher.BeginInvokeAsynchronouslyInApplicationIdle(() =>
                            {
                                if (bDisposed)
                                    return;

                                img.BeginInit();
                                img.UriSource = new Uri(file);
                                img.CacheOption = BitmapCacheOption.OnLoad;
                                img.EndInit();
                                image.Source = img;
                            });
                            contentSource.Add(image);
                        }
                        catch (Exception ex)
                        {
                        }
                    }

                    var description = backController.GetDescription();
                    if (!String.IsNullOrEmpty(description))
                        contentSource.Add(new TextBlock()
                        {
                            Text = map != null && map.ContainsKey(description) ? map[description] : description,
                            FontFamily = new FontFamily("Segoe UI Light"),
                            FontSize = 32,
                            HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                            VerticalAlignment = System.Windows.VerticalAlignment.Center,
                            TextWrapping = TextWrapping.Wrap
                        });
                    tile.ContentSource = contentSource;
                    Utilities.WPF.DependencyObjectExtensions.RegisterName(this, tile, false, false);

                    timeSpan += 500;
                    tileLayoutControl.Children.Add(tile);
                    AddCommand(tile.Header as String, tile);

                    TileLayoutControl.SetIsFlowBreak(tile, true);
                    TileLayoutControl.SetGroupHeader(tile, tile.Header as String);
                }

                screenController.GetScreenControllers().ForEach(controller2 =>
                {
                    if (controller2.IsVisible())
                    {
                        if (controller2.GetStartType() != StartType.TilePage)
                        {
                            var tile = new Tile()
                            {
                                Name = Utilities.WPF.DependencyObjectExtensions.AdaptName(controller2.GetTitle()),
                                Header = map != null && map.ContainsKey(controller2.GetTitle()) ? map[controller2.GetTitle()] : controller2.GetTitle(),
                                AnimateContentChange = true,
                                ContentChangeInterval = TimeSpan.FromMilliseconds(timeSpan),
                                Background = new SolidColorBrush(controller2.GetIdentityColor())
                            };
                            var tileSize = controller2.TileSize();
                            switch (tileSize)
                            {
                                case DocumentManager.ComponentService.TileSize.Small:
                                    tile.Size = DevExpress.Xpf.LayoutControl.TileSize.Small;
                                    break;
                                case DocumentManager.ComponentService.TileSize.ExtraSmall:
                                    tile.Size = DevExpress.Xpf.LayoutControl.TileSize.ExtraSmall;
                                    break;
                                case DocumentManager.ComponentService.TileSize.Large:
                                    tile.Size = DevExpress.Xpf.LayoutControl.TileSize.Large;
                                    break;
                                case DocumentManager.ComponentService.TileSize.Live:
                                case DocumentManager.ComponentService.TileSize.ExtraLarge:
                                    tile.Size = DevExpress.Xpf.LayoutControl.TileSize.ExtraLarge;
                                    break;
                            }
                            tile.Click += (ob, ev) =>
                            {
                                Container.OpenOrActivate(controller2, screenController);
                            };

                            var contentSource = new FrameworkElements();
                            if (tileSize == DocumentManager.ComponentService.TileSize.Live)
                            {
                                tile.AnimateContentChange = false;

                                UserControl control = null;
                                switch (controller2.GetStartType())
                                {
                                    case StartType.GeoPage:
                                        control = new GeoViewer(ScreenComponent, controller2, Container, DocParent, backController);
                                        break;
                                    //case StartType.GalleryPage:
                                    //    control = new GalleryViewer(ScreenComponent, controller2, Container, DocParent, backController);
                                    //    break;
                                }

                                if (control != null)
                                {
                                    contentSource.Add(control);
                                    listViewers.Add(control);
                                }
                            }
                            else
                            {
                                contentSource.Add(new TextBlock() { Text = tile.Header as String, FontFamily = new FontFamily("Segoe UI Light"), FontSize = 32, HorizontalAlignment = System.Windows.HorizontalAlignment.Center, VerticalAlignment = System.Windows.VerticalAlignment.Center, TextWrapping = TextWrapping.Wrap });
                                var file = controller2.GetIconSource();
                                if (File.Exists(file))
                                {
                                    try
                                    {
                                        var img = new BitmapImage();
                                        var image = new Image()
                                        {
                                            Margin = new Thickness(20)
                                        };

                                        Dispatcher.BeginInvokeAsynchronouslyInApplicationIdle(() =>
                                        {
                                            if (bDisposed)
                                                return;

                                            img.BeginInit();
                                            img.UriSource = new Uri(file);
                                            img.CacheOption = BitmapCacheOption.OnLoad;
                                            img.EndInit();
                                            image.Source = img;
                                        });
                                        contentSource.Add(image);
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                }
                            }
                            if (tileSize != DocumentManager.ComponentService.TileSize.Live)
                            {
                                var description = controller2.GetDescription();
                                if (!String.IsNullOrEmpty(description))
                                    contentSource.Add(new TextBlock()
                                    {
                                        Text = map != null && map.ContainsKey(description) ? map[description] : description,
                                        FontFamily = new FontFamily("Segoe UI Light"),
                                        FontSize = 32,
                                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                                        VerticalAlignment = System.Windows.VerticalAlignment.Center,
                                        TextWrapping = TextWrapping.Wrap
                                    });
                            }
                            tile.ContentSource = contentSource;
                            Utilities.WPF.DependencyObjectExtensions.RegisterName(this, tile, false, false);

                            timeSpan += 500;
                            tileLayoutControl.Children.Add(tile);
                            AddCommand(tile.Header as String, tile);

                            TileLayoutControl.SetIsFlowBreak(tile, true);
                            var title = controller2.GetTitle();
                            TileLayoutControl.SetGroupHeader(tile, map != null &&
                                map.ContainsKey(title) ? map[title] : title);
                        }
                        else
                        {
                            var parent = DocParent;
                            if (controller2 is IDocument)
                                parent = controller2 as IDocument;
                            AddTilesForScreenController(controller2, Container, parent, map);

                            bool bFirst = controller2.GetScreenLists().Count == 0;
                            controller2.GetScreenControllers().ForEach(controller =>
                            {
                                var tile = new Tile()
                                {
                                    Name = Utilities.WPF.DependencyObjectExtensions.AdaptName(controller.GetTitle()),
                                    Header = map != null && map.ContainsKey(controller.GetTitle()) ? map[controller.GetTitle()] : controller.GetTitle(),
                                    AnimateContentChange = true,
                                    ContentChangeInterval = TimeSpan.FromMilliseconds(timeSpan),
                                    Background = new SolidColorBrush(controller.GetIdentityColor())
                                };
                                switch (controller.TileSize())
                                {
                                    case DocumentManager.ComponentService.TileSize.Small:
                                        tile.Size = DevExpress.Xpf.LayoutControl.TileSize.Small;
                                        break;
                                    case DocumentManager.ComponentService.TileSize.ExtraSmall:
                                        tile.Size = DevExpress.Xpf.LayoutControl.TileSize.ExtraSmall;
                                        break;
                                    case DocumentManager.ComponentService.TileSize.Large:
                                        tile.Size = DevExpress.Xpf.LayoutControl.TileSize.Large;
                                        break;
                                    case DocumentManager.ComponentService.TileSize.Live:
                                    case DocumentManager.ComponentService.TileSize.ExtraLarge:
                                        tile.Size = DevExpress.Xpf.LayoutControl.TileSize.ExtraLarge;
                                        break;
                                }
                                tile.Click += (ob, ev) =>
                                {
                                    Container.OpenOrActivate(controller2, screenController);
                                };

                                var contentSource = new FrameworkElements();
                                contentSource.Add(new TextBlock() { Text = tile.Header as String, FontFamily = new FontFamily("Segoe UI Light"), FontSize = 32, HorizontalAlignment = System.Windows.HorizontalAlignment.Center, VerticalAlignment = System.Windows.VerticalAlignment.Center, TextWrapping = TextWrapping.Wrap });
                                var file = controller2.GetIconSource();
                                if (File.Exists(file))
                                {
                                    try
                                    {
                                        var img = new BitmapImage();
                                        var image = new Image()
                                        {
                                            Margin = new Thickness(20)
                                        };

                                        Dispatcher.BeginInvokeAsynchronouslyInApplicationIdle(() =>
                                        {
                                            if (bDisposed)
                                                return;

                                            img.BeginInit();
                                            img.UriSource = new Uri(file);
                                            img.CacheOption = BitmapCacheOption.OnLoad;
                                            img.EndInit();
                                            image.Source = img;
                                        });
                                        contentSource.Add(image);
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                }
                                var description = controller2.GetDescription();
                                if (!String.IsNullOrEmpty(description))
                                    contentSource.Add(new TextBlock()
                                    {
                                        Text = map != null && map.ContainsKey(description) ? map[description] : description,
                                        FontFamily = new FontFamily("Segoe UI Light"),
                                        FontSize = 32,
                                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                                        VerticalAlignment = System.Windows.VerticalAlignment.Center,
                                        TextWrapping = TextWrapping.Wrap
                                    });
                                tile.ContentSource = contentSource;
                                Utilities.WPF.DependencyObjectExtensions.RegisterName(this, tile, false, false);

                                timeSpan += 500;
                                tileLayoutControl.Children.Add(tile);
                                AddCommand(tile.Header as String, tile);

                                if (bFirst)
                                {
                                    bFirst = false;
                                    TileLayoutControl.SetIsFlowBreak(tile, true);
                                    var title = controller2.GetTitle();
                                    TileLayoutControl.SetGroupHeader(tile, map != null &&
                                        map.ContainsKey(title) ? map[title] : title);
                                }
                            });
                        }
                    }
                });

                AddTilesForScreenController(screenController, Container, DocParent, map);

                if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                    LoadLayout(screenController.GetTitle());
                else
                    SaveLayout(screenController.GetTitle());

                tileLayoutControl.Background = new SolidColorBrush(screenController.GetIdentityColor());
            }
        }

        private void AddTilesForScreenController(IScreenController screenController, 
            ScreenTransitionViewer container, IDocument DocumentParent, 
            IDictionary<String, String> map)
        {
            bool bFirst = true;
            screenController.GetScreenLists().ForEach(uri =>
            {
                var title = System.IO.Path.GetFileNameWithoutExtension(uri.GetPathString());
                var tile = new Tile()
                {
                    Name = Utilities.WPF.DependencyObjectExtensions.AdaptName(title),
                    Header = map != null && map.ContainsKey(title) ? map[title] : title,
                    AnimateContentChange = true,
                    ContentChangeInterval = TimeSpan.FromMilliseconds(timeSpan),
                    Background = new SolidColorBrush(screenController.GetIdentityColor(uri))
                };
                var tileSize = screenController.TileSize(uri);
                switch (tileSize)
                {
                    case DocumentManager.ComponentService.TileSize.Small: 
                        tile.Size = DevExpress.Xpf.LayoutControl.TileSize.Small;
                        break;
                    case DocumentManager.ComponentService.TileSize.ExtraSmall:
                        tile.Size = DevExpress.Xpf.LayoutControl.TileSize.ExtraSmall;
                        break;
                    case DocumentManager.ComponentService.TileSize.Large:
                        tile.Size = DevExpress.Xpf.LayoutControl.TileSize.Large;
                        break;
                    case DocumentManager.ComponentService.TileSize.Live:
                    case DocumentManager.ComponentService.TileSize.ExtraLarge:
                        tile.Size = DevExpress.Xpf.LayoutControl.TileSize.ExtraLarge;
                        break;
                }

                tile.Click += (ob, ev) =>
                {
                    ScreenComponent.Execute(uri, DocumentParent, ExecutionMode.Normal, null);
                    // container.OpenOrActivate(uri, parent:DocumentParent);
                };

                var contentSource = new FrameworkElements();
                if (tileSize != DocumentManager.ComponentService.TileSize.Live)
                    contentSource.Add(new TextBlock() { Text = tile.Header as String, FontFamily = new FontFamily("Segoe UI Light"), FontSize = 32, HorizontalAlignment = System.Windows.HorizontalAlignment.Center, VerticalAlignment = System.Windows.VerticalAlignment.Center, TextWrapping = TextWrapping.Wrap });

                if (tileSize == DocumentManager.ComponentService.TileSize.Live)
                {
                    tile.AnimateContentChange = false;
                    contentSource.Add(FillTile(uri, Container, DocParent));
                }
                else
                {
                    var file = System.IO.Path.ChangeExtension(uri.GetPathString(), "png");
                    if (File.Exists(file))
                    {
                        try
                        {
                            var img = new BitmapImage();
                            var image = new Image()
                            {
                                Margin = new Thickness(20)
                            };

                            Dispatcher.BeginInvokeAsynchronouslyInApplicationIdle(() =>
                            {
                                if (bDisposed)
                                    return;

                                img.BeginInit();
                                img.UriSource = new Uri(file);
                                img.CacheOption = BitmapCacheOption.OnLoad;
                                img.EndInit();
                                image.Source = img;
                            });
                            contentSource.Add(image);
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
                var description = screenController.GetDescription(uri);
                if (tileSize != DocumentManager.ComponentService.TileSize.Live && 
                    !String.IsNullOrEmpty(description))
                    contentSource.Add(new TextBlock() { Text = map != null && map.ContainsKey(description) ? map[description] : description, 
                        FontFamily = new FontFamily("Segoe UI Light"), FontSize = 32, HorizontalAlignment = System.Windows.HorizontalAlignment.Center, VerticalAlignment = System.Windows.VerticalAlignment.Center, TextWrapping = TextWrapping.Wrap });
                tile.ContentSource = contentSource;
                Utilities.WPF.DependencyObjectExtensions.RegisterName(this, tile, false, false);

                timeSpan += 500;
                tileLayoutControl.Children.Add(tile);
                if (tileSize != DocumentManager.ComponentService.TileSize.Live)
                    AddCommand(tile.Header as String, tile);

                if (bFirst)
                {
                    bFirst = false;
                    TileLayoutControl.SetIsFlowBreak(tile, true);
                    var t = screenController.GetTitle();
                    TileLayoutControl.SetGroupHeader(tile, map != null &&
                        map.ContainsKey(t) ? map[t] : t);
                }
            });
        }

        ScreenViewer FillTile(Uri uri, ScreenTransitionViewer container, IDocument DocumentParent)
        {
            using (var cursor = new WaitCursor())
            {
                var viewer = new ScreenViewer(ScreenComponent, uri, DocumentParent, container,
                    isModal: true, bIsLayout: true);
                listViewers.Add(viewer);
                return viewer;
            }
        }

        void CleanupTiles()
        {
            foreach (var control in tileLayoutControl.Children)
            {
                if (!(control is Tile))
                    continue;
                var tile = control as Tile;
                Utilities.WPF.DependencyObjectExtensions.UnregisterName(this, tile);
                var felist = tile.ContentSource as FrameworkElements;
                if (felist == null)
                    continue;
                felist.Clear();
            }
            tileLayoutControl.Children.Clear();

            (from c in listViewers.OfType<IDisposable>() select c).ToList().ForEach(viewer => viewer.Dispose());
            listViewers.Clear();
        }

        #region Isolated Storage

        static String GetStoreFileName(String title)
        {
            return String.Format("{0}.{1}.TileViewer.dat", title, System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));
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
                    var settings = new XmlWriterSettings
                    {
                        Indent = true,
                        OmitXmlDeclaration = false,
                        Encoding = Encoding.UTF8
                    };

                    using (var writer = XmlWriter.Create(stream, settings))
                    {
                        tileLayoutControl.WriteToXML(writer);
                    }
                }
            }
            catch (Exception ex)
            {

            }
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

                    using (var reader = XmlReader.Create(stream, settings))
                    {
                        tileLayoutControl.ReadFromXML(reader);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        #endregion

        #region IDisposable
        bool bDisposed;
        public void Dispose()
        {
            bDisposed = true;
            CleanupTiles();

            if (ScreenComponent.StringEditor != null)
                ScreenComponent.StringEditor.CultureChanged -= StringEditor_CultureChanged;
        }
        #endregion
    }
}
