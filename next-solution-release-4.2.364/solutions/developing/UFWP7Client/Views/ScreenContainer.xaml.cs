using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Navigation;
using UFWebClient.Helpers;
using UFWebClient.ScreenManagerPollServiceReference;
using UFWebClient.ViewModelLib;
using Utilities.Animations;
using ViewModelLib;
using System.Globalization;
using Microsoft.Phone.Controls;
using UFWP7Client;

namespace UFWebClient.Views
{
    public partial class ScreenContainer : PhoneApplicationPage
    {
        private AppSettings settings = new AppSettings();

        ScreenManagerPollClient client;
        Guid session = Guid.NewGuid();

        bool bPollingEnabled;
        Uri uri;

        List<String> listEnabledCommands = new List<String>();
        Dictionary<String, UIElement> mapNameElement = new Dictionary<String, UIElement>();
        Dictionary<String, Dictionary<Guid, AnimationManager.AnimationManager>> mapNameAnimation = new Dictionary<String, Dictionary<Guid, AnimationManager.AnimationManager>>();

        ScreenStorage screenStorage;

        public ScreenContainer(Uri u)
        {
            InitializeComponent();
            uri = u;
        }

        public ScreenContainer()
        {
            InitializeComponent();
        }

        private void Initialize()
        {
            if (uri == null && NavigationContext != null)
            {
                IDictionary<string, string> parameters = this.NavigationContext.QueryString;
                if (parameters.ContainsKey("requestedUri"))
                    uri = new Uri(parameters["requestedUri"], UriKind.RelativeOrAbsolute);
                //else
                //{
                //    ErrorWindow.CreateNew("Missing Uri, please enter a valid uri to request");
                //    return;
                //}
            }

            EndpointAddress endpoint = GetEndPoint();
            busyIndicator.IsBusy = true;
            busyIndicator.BusyContent = String.Format("Contacting the server {0}...", endpoint.Uri.Host);
            if (client == null)
            {
                CustomBinding bindingHttp = new CustomBinding(
                                new BinaryMessageEncodingBindingElement(),
                                new HttpTransportBindingElement()
                                            {
                                                MaxReceivedMessageSize = int.MaxValue,
                                                MaxBufferSize = int.MaxValue
                                            });

                //CustomBinding bindingTcp = new CustomBinding(
                //                new BinaryMessageEncodingBindingElement(),
                //                new TcpTransportBindingElement()
                //                {
                //                    MaxReceivedMessageSize = int.MaxValue,
                //                    MaxBufferSize = int.MaxValue
                //                });

                if (endpoint.Uri.Scheme.Equals("https", StringComparison.InvariantCultureIgnoreCase))
                    bindingHttp.Elements.Add(new TransportSecurityBindingElement());
                client = new ScreenManagerPollClient(bindingHttp, endpoint);

                client.RegisterAsync(session, CultureInfo.CurrentCulture.Name);
                StartFadingStatus();

                client.RegisterCompleted += (o, ev) =>
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        if (ev.Error != null)
                        {
                            busyIndicator.IsBusy = false;

                            MessageBox.Show(ev.Error.ToString());
                            // ErrorWindow.CreateNew(ev.Error);
                        }
                        else
                        {
                            OpenUri();
                        }
                    });
                };
            }
            else
                OpenUri();
        }

        bool bOpenCompleted;
        private void OpenUri()
        {
            screenStorage = new ScreenStorage();
            StartFadingStatus();

            busyIndicator.BusyContent = "Requesting the screen to the server...";
            client.OpenAsync(session, uri, GetScreenHashAndPreloadData(uri));
            if (bOpenCompleted)
                return;
            bOpenCompleted = true;
            client.OpenCompleted += (oi, eiv) =>
            {
                Dispatcher.BeginInvoke(() =>
                {
                    if (eiv.Error != null)
                    {
                        busyIndicator.IsBusy = false;

                        MessageBox.Show(eiv.Error.ToString());
                        // ErrorWindow.CreateNew(eiv.Error);
                    }
                    else
                    {
                        busyIndicator.BusyContent = "Reading screen data from the server...";

                        DecodeScreen(eiv.Result);

                        bPollingEnabled = true;
                        StartGettingCommandUpdate();
                        StartGettingDataUpdate();
                    }
                });
            };
        }

        void StartFadingStatus()
        {
            status.Opacity = 1.0;
            status.Fade(0, 2000, new BackEase() { EasingMode = EasingMode.EaseInOut });
        }

        bool bGetDataUpdateCompleted;
        void StartGettingDataUpdate()
        {
            StartFadingStatus();
            client.GetDataUpdateAsync(session, uri);
            if (bGetDataUpdateCompleted)
                return;
            bGetDataUpdateCompleted = true;
            client.GetDataUpdateCompleted += (o, e) =>
                {
                    Dispatcher.BeginInvoke(() =>
                        {
                            if (e.Error != null)
                            {
                                busyIndicator.IsBusy = false;

                                MessageBox.Show(e.Error.ToString());
                                // ErrorWindow.CreateNew(e.Error);
                            }
                            else
                            {
                                StartFadingStatus();

                                foreach(var update in e.Result)
                                {
                                    status.Opacity = 1.0;
                                    status.Fade(0, 2000, new BackEase() { EasingMode = EasingMode.EaseInOut });

                                    if (!update.IsKeepAlive &&
                                        mapNameElement.ContainsKey(update.Name))
                                    {
                                        if (update.dataValue != null && update.dataValue.Value.Value != null)
                                        {
                                            Type type = null;
                                            Object value = null;
                                            if (update.builtInType != null && update.builtInType != BuiltInType.Null)
                                            {
                                                try
                                                {
                                                    type = Opc.Ua.TypeInfo.GetSystemType((Opc.Ua.BuiltInType)update.builtInType, update.valueRank);
                                                    value = Convert.ChangeType(update.dataValue.Value.Value.Value, type, null);
                                                }
                                                catch (Exception ex)
                                                {
                                                    MessageBox.Show(ex.ToString());
                                                    // ErrorWindow.CreateNew(ex);
                                                }
                                            }

                                            FrameworkElement fe = mapNameElement[update.Name] as FrameworkElement;
                                            if (update.IsDataContext)
                                            {
                                                var dataContext = fe.DataContext as MonitoredItemViewModel;
                                                dataContext.IsChanging = true;

                                                try
                                                {
                                                    dataContext.nodeId = update.nodeId;
                                                    dataContext.DataValue = update.dataValue;
                                                    dataContext.Range = update.range;

                                                    if (value != null)
                                                    {
                                                        dataContext.builtInType = update.builtInType;
                                                        dataContext.valueRank = update.valueRank;
                                                        dataContext.Value = value;
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    MessageBox.Show(ex.ToString());
                                                    // ErrorWindow.CreateNew(ex);
                                                }
                                                finally
                                                {
                                                    dataContext.IsChanging = false;
                                                }
                                            }
                                            if (update.listGuid != null)
                                            {
                                                if (mapNameAnimation.ContainsKey(update.Name))
                                                {
                                                    try
                                                    {
                                                        var mapguid = mapNameAnimation[update.Name];
                                                        foreach (var guid in update.listGuid)
                                                        {
                                                            if (!mapguid.ContainsKey(guid))
                                                                continue;
                                                            mapguid[guid].Range = update.range;
                                                            mapguid[guid].ExecuteWithData(update.dataValue);
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        MessageBox.Show(ex.ToString());
                                                        // ErrorWindow.CreateNew(ex);
                                                    }
                                                }
                                            }

                                            SetDynamicSource(mapNameElement[update.Name], false, update.dataValue.StatusCode.Code);
                                        }
                                    }
                                }

                                if (client != null && bPollingEnabled)
                                    client.GetDataUpdateAsync(session, uri);
                            }
                        });
                };
        }

        bool bGetCommandUpdateCompleted;
        void StartGettingCommandUpdate()
        {
            client.GetCommandUpdateAsync(session, uri);
            if (bGetCommandUpdateCompleted)
                return;
            bGetCommandUpdateCompleted = true;
            client.GetCommandUpdateCompleted += (o, e) =>
                {
                    if (e.Error != null)
                        MessageBox.Show(e.Error.ToString());
                    else
                    {
                        foreach (var update in e.Result)
                        {
                            if (update.IsEnabled)
                            {
                                if (!listEnabledCommands.Contains(update.Name))
                                {
                                    listEnabledCommands.Add(update.Name);
                                    Execute.OnUIThread(() =>
                                        {
                                            _Command.RaiseCanExecuteChanged();
                                        });
                                }
                            }
                            else
                            {
                                if (listEnabledCommands.Contains(update.Name))
                                {
                                    listEnabledCommands.Remove(update.Name);
                                    Execute.OnUIThread(() =>
                                        {
                                            _Command.RaiseCanExecuteChanged();
                                        });
                                }
                            }
                        }

                        if (client != null && bPollingEnabled)
                            client.GetCommandUpdateAsync(session, uri);
                    }
                };
        }

        bool bGetRemoteRequestCompleted;
        void StartGettingRemoteExecute()
        {
            client.GetRemoteRequestAsync(session, uri);
            if (bGetRemoteRequestCompleted)
                return;
            bGetRemoteRequestCompleted = true;
            client.GetRemoteRequestCompleted += (o, e) =>
                {
                    Dispatcher.BeginInvoke(() =>
                        {
                            if (e.Error != null)
                                MessageBox.Show(e.Error.ToString());
                            else
                            {
                                foreach (var update in e.Result)
                                {
                                    if (update.request.executionMode == ExecutionMode.Synchro)
                                    {
                                        var container = new ScreenContainer
                                            {
                                                uri = update.request.uri
                                            };
                                        var child = new ChildWindow
                                        {
                                            Style = Application.Current.Resources["ChildWindowStyle"] as Style,
                                            Content = container,
                                            HasCloseButton = true,
                                            Title = System.IO.Path.GetFileNameWithoutExtension(container.uri.AbsolutePath)
                                        };
                                        child.Closed += (j, k) =>
                                            {
                                                container.OnNavigatedFrom(null);
                                            };
                                        container.OnNavigatedTo(null);
                                        container.screenViewer.viewBox.Stretch = Stretch.None;

                                        child.Show();
                                    }
                                    else
                                    {
                                        NavigationService.Navigate(new Uri(String.Format("/Views/ScreenContainer.xaml?requestedUri={0}", update.request.uri), UriKind.Relative));
                                        //OnNavigatedFrom(null);
                                        //uri = update.request.uri;
                                        //screenViewer.MainSurface.Children.Clear();

                                        //OpenUri();
                                    }
                                }
                            }

                            if (bCommandSent)
                            {
                                bCommandSent = false;
                                busyIndicator.IsBusy = false;
                            }
                        });
                };
        }

        private void DecodeScreen(IEnumerable<ScreenUpdate> Result)
        {
            bool bInStorageSaved = false;

            foreach (var update1 in Result)
            {
                busyIndicator.BusyContent = String.Format("Processing server update {0}...", update1.ActualCount);
                try
                {
                    if (update1.notFound)
                    {
                        busyIndicator.IsBusy = false;
                        MessageBox.Show(String.Format("the Screen {0} does not exist on the server", update1.uri));
                    }
                    else if (update1.IsResource)
                    {
                        var xaml = ConvertXaml(update1.xaml);
                        ResourceDictionary dict = XamlReader.Load(xaml) as ResourceDictionary;
                        screenViewer.MainSurface.Resources.MergedDictionaries.Add(dict);
                        screenStorage.listResources.Add(xaml);
                    }
                    else if (update1.IsBackground)
                    {
                        if (update1.hashOk)
                        {
                            busyIndicator.IsBusy = true;
                            busyIndicator.BusyContent = ApplicationStrings.BusyIndicatorLoadingScreenFromStorage;

                            Canvas cv = XamlReader.Load(screenStorage.canvasXaml) as Canvas;

                            screenViewer.MainSurface.Width = cv.Width;
                            screenViewer.MainSurface.Height = cv.Height;
                            screenViewer.MainSurface.Background = cv.Background;

                            screenStorage.listResources.ForEach(xaml =>
                            {
                                ResourceDictionary dict = XamlReader.Load(xaml) as ResourceDictionary;
                                screenViewer.MainSurface.Resources.MergedDictionaries.Add(dict);
                            });

                            screenStorage.listObjects.ForEach(obj =>
                            {
                                FrameworkElement fe = null;

                                if (String.IsNullOrEmpty(obj.Xaml))
                                {
                                    foreach (var child in screenViewer.MainSurface.Children)
                                    {
                                        fe = (child as FrameworkElement).FindName(obj.Name) as FrameworkElement;
                                        if (fe != null)
                                            break;
                                    }

                                    if (fe == null)
                                        throw new ArgumentException(String.Format("Cannot find the element for {1}", obj.Name));
                                }
                                else
                                {
                                    UIElement uie = XamlReader.Load(obj.Xaml) as UIElement;
                                    //if (uie.Effect is ShaderEffects.ReflectionEffect)
                                    //    (uie.Effect as ShaderEffects.ReflectionEffect).UpdateBinding(uie as FrameworkElement);

                                    screenViewer.MainSurface.Children.Add(uie);
                                    fe = uie as FrameworkElement;

                                    if (!String.IsNullOrEmpty(obj.StyleResource))
                                        fe.Style = screenViewer.MainSurface.Resources[obj.StyleResource] as Style;
                                    if (!String.IsNullOrEmpty(obj.BrushResource))
                                    {
                                        if (fe is Panel)
                                            (fe as Panel).Background = screenViewer.MainSurface.Resources[obj.BrushResource] as Brush;
                                        else if (fe is Control)
                                            (fe as Control).Background = screenViewer.MainSurface.Resources[obj.BrushResource] as Brush;
                                        else if (fe is System.Windows.Shapes.Shape)
                                            (fe as System.Windows.Shapes.Shape).Fill = screenViewer.MainSurface.Resources[obj.BrushResource] as Brush;
                                    }
                                    if (!String.IsNullOrEmpty(obj.PenResource))
                                    {
                                        if (fe is Control)
                                            (fe as Control).BorderBrush = screenViewer.MainSurface.Resources[obj.PenResource] as Brush;
                                        else if (fe is System.Windows.Shapes.Shape)
                                            (fe as System.Windows.Shapes.Shape).Stroke = screenViewer.MainSurface.Resources[obj.PenResource] as Brush;
                                    }
                                }

                                if (!String.IsNullOrEmpty(obj.Name) && !mapNameElement.ContainsKey(obj.Name))
                                    mapNameElement.Add(obj.Name, fe);

                                if (screenStorage.listCommandSources != null && screenStorage.listCommandSources.Contains(obj.Name))
                                    SetCommandSource(fe, obj.Name);

                                if (screenStorage.listDynamicSources != null && screenStorage.listDynamicSources.Contains(obj.Name))
                                {
                                    CreateMonItemDataContext(fe);
                                    SetDynamicSource(fe);
                                }

                                if (screenStorage.mapAnimations != null && screenStorage.mapAnimations.ContainsKey(fe.Name))
                                {
                                    UpdateAnimationListMap(fe, obj.Name, screenStorage.mapAnimations[obj.Name]);
                                }
                            });
                            busyIndicator.IsBusy = false;
                        }
                        else
                        {
                            screenStorage = new ScreenStorage(); // clean the previous storage
                            screenStorage.canvasXaml = update1.xaml;
                            UIElement uie = XamlReader.Load(update1.xaml) as UIElement;

                            Canvas stub = uie as Canvas;
                            screenViewer.MainSurface.Width = stub.Width;
                            screenViewer.MainSurface.Height = stub.Height;
                            screenViewer.MainSurface.Background = stub.Background;

                            progressDownload.Maximum = update1.maxCount;
                            progressDownload.Minimum = 0;
                            progressDownload.Value = 0;
                            progressDownload.Visibility = Visibility.Visible;
                        }
                    }
                    else if (!String.IsNullOrEmpty(update1.xaml) ||
                             !String.IsNullOrEmpty(update1.Name))
                    {
                        String xaml = null;
                        FrameworkElement uie = null;
                        if (!String.IsNullOrEmpty(update1.xaml))
                        {
                            xaml = ConvertXaml(update1.xaml);
                            do
                            {
                                try
                                {
                                    uie = XamlReader.Load(xaml) as FrameworkElement;
                                    break;
                                }
                                catch (XamlParseException ex)
                                {
                                    if (ex.Message.Contains("The property '"))
                                    {
                                        int start = ex.Message.IndexOf('\'');
                                        int end = ex.Message.IndexOf('\'', start + 1);

                                        String propName = ex.Message.Substring(start + 1, end - start - 1);
                                        xaml = xaml.Replace(propName, "d:" + propName);
                                    }
                                    else
                                        throw;
                                }
                            } while (true);

                            if (!String.IsNullOrEmpty(update1.StyleResource))
                                uie.Style = screenViewer.MainSurface.Resources[update1.StyleResource] as Style;
                            if (!String.IsNullOrEmpty(update1.BrushResource))
                            {
                                if (uie is Panel)
                                    (uie as Panel).Background = screenViewer.MainSurface.Resources[update1.BrushResource] as Brush;
                                else if (uie is Control)
                                    (uie as Control).Background = screenViewer.MainSurface.Resources[update1.BrushResource] as Brush;
                                else if (uie is System.Windows.Shapes.Shape)
                                    (uie as System.Windows.Shapes.Shape).Fill = screenViewer.MainSurface.Resources[update1.BrushResource] as Brush;
                            }
                            if (!String.IsNullOrEmpty(update1.PenResource))
                            {
                                if (uie is Control)
                                    (uie as Control).BorderBrush = screenViewer.MainSurface.Resources[update1.PenResource] as Brush;
                                else if (uie is System.Windows.Shapes.Shape)
                                    (uie as System.Windows.Shapes.Shape).Stroke = screenViewer.MainSurface.Resources[update1.PenResource] as Brush;
                            }

                            //if (uie.Effect is ShaderEffects.ReflectionEffect)
                            //    (uie.Effect as ShaderEffects.ReflectionEffect).UpdateBinding(uie);
                            screenViewer.MainSurface.Children.Add(uie);
                        }
                        else
                        {
                            foreach (var child in screenViewer.MainSurface.Children)
                            {
                                uie = (child as FrameworkElement).FindName(update1.Name) as FrameworkElement;
                                if (uie != null)
                                    break;
                            }

                            if (uie == null)
                                throw new ArgumentException(String.Format("Cannot find the element for {1}", update1.Name));
                        }

                        screenStorage.listObjects.Add(new ScreenObject()
                        {
                            Name = update1.Name,
                            Xaml = xaml,
                            StyleResource = update1.StyleResource,
                            BrushResource = update1.BrushResource,
                            PenResource = update1.PenResource
                        });

                        if (!String.IsNullOrEmpty(update1.Name) && !mapNameElement.ContainsKey(update1.Name))
                            mapNameElement.Add(update1.Name, uie);

                        if (update1.IsCommandSource)
                        {
                            SetCommandSource(uie, update1.Name);
                            screenStorage.listCommandSources.Add(update1.Name);
                        }

                        if (update1.IsDynamic)
                        {
                            CreateMonItemDataContext(uie as FrameworkElement);
                            SetDynamicSource(uie);
                            screenStorage.listDynamicSources.Add(update1.Name);
                        }

                        if (update1.animationManagerList != null)
                        {
                            var list = AnimationManager.AnimationManager.ConvertListFromService(update1.animationManagerList);
                            screenStorage.mapAnimations.Add(update1.Name, list);
                            UpdateAnimationListMap(uie, update1.Name, list);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    // ErrorWindow.CreateNew(ex);
                }
                finally
                {
                    busyIndicator.IsBusy = false;

                    if (!update1.hashOk && !update1.notFound &&
                        !update1.IsResource && !update1.IsBackground)
                    {
                        progressDownload.Value = update1.ActualCount;
                    }

                    if (update1.IsTermination)
                    {
                        progressDownload.Visibility = Visibility.Collapsed;

                        foreach (var child in screenViewer.MainSurface.Children)
                        {
                            child.ManipulationStarted += (o, args) =>
                            {
                                args.ManipulationContainer = screenViewer.MainSurface;
                            };

                            child.ManipulationDelta += (o, args) =>
                            {
                                FrameworkElement element = o as FrameworkElement;
                                Point translation = args.DeltaManipulation.Translation;
                                Canvas.SetLeft(element, Canvas.GetLeft(element) + translation.X);
                                Canvas.SetTop(element, Canvas.GetTop(element) + translation.Y);
                                if (args.DeltaManipulation.Scale.X != 0)
                                {
                                    var newSize = element.ActualWidth + args.DeltaManipulation.Scale.X;
                                    var curPos = Canvas.GetLeft(element);
                                    var newPos = curPos + (element.ActualWidth / 2) - (newSize / 2);
                                    element.Width = newSize;
                                    Canvas.SetLeft(element, newPos);
                                }
                                if (args.DeltaManipulation.Scale.Y != 0)
                                {
                                    var newSize = element.ActualHeight + args.DeltaManipulation.Scale.Y;
                                    var curPos = Canvas.GetTop(element);
                                    var newPos = curPos + (element.ActualHeight / 2) - (newSize / 2);
                                    element.Height = newSize;
                                    Canvas.SetTop(element, newPos);
                                }
                                args.Handled = true;
                            };
                        }

                        if (screenStorage != null && !bInStorageSaved && !update1.hashOk)
                        {
                            bInStorageSaved = true;
                            uri = update1.uri;

                            //screenStorage.uri = uri;
                            //screenStorage.hash = update1.hash;

                            //var background = new BackgroundWorker();
                            //background.DoWork += (i, e) =>
                            //{
                            //    IsolatedStorageManager.SaveData(uri.AbsolutePath, screenStorage);
                            //    screenStorage = null;
                            //};

                            //background.RunWorkerCompleted += (i, e) =>
                            //{
                            //    if (e.Error != null)
                            //    {
                            //        QuotaManager window = new QuotaManager();
                            //        window.Closed += (h, f) =>
                            //        {
                            //            try
                            //            {
                            //                IsolatedStorageManager.SaveData(uri.AbsolutePath, screenStorage);
                            //                screenStorage = null;
                            //            }
                            //            catch (Exception ex)
                            //            {
                            //                MessageBox.Show(ex.ToString());
                            //                // ErrorWindow.CreateNew(ex);
                            //            }
                            //        };
                            //        window.Show();
                            //    }
                            //};
                            //background.RunWorkerAsync();
                        }
                    }
                }
            }
        }

        private void UpdateAnimationListMap(UIElement uie, String name, AnimationManager.AnimationManagerList list)
        {
            var mapguid = new Dictionary<Guid, AnimationManager.AnimationManager>();
            list.ForEach(animation =>
                {
                    mapguid.Add(animation.ID, animation);
                    animation.Control = uie;
                });
            if (mapNameAnimation.ContainsKey(name))
                mapNameAnimation.Remove(name);
            mapNameAnimation.Add(name, mapguid);
        }

        private object ChangeType(Object v, BuiltInType builtinType)
        {
            object value = v;
            try
            {
                if (value is String && builtinType != BuiltInType.String)
                {
                    if (String.Compare(value as String, "True", StringComparison.OrdinalIgnoreCase) == 0)
                        v = 1;
                    else if (String.Compare(value as String, "False", StringComparison.OrdinalIgnoreCase) == 0)
                        v = 0;
                    else
                        v = Convert.ToDouble(v);
                }
            }
            catch { }

            switch (builtinType)
            {
                case BuiltInType.Boolean: value = Convert.ToBoolean(v); break;
                case BuiltInType.SByte: value = Convert.ToSByte(v); break;
                case BuiltInType.Byte: value = Convert.ToByte(v); break;
                case BuiltInType.Int16: value = Convert.ToInt16(v); break;
                case BuiltInType.UInt16: value = Convert.ToUInt16(v); break;
                case BuiltInType.Int32: value = Convert.ToInt32(v); break;
                case BuiltInType.UInt32: value = Convert.ToUInt32(v); break;
                case BuiltInType.Int64: value = Convert.ToInt64(v); break;
                case BuiltInType.UInt64: value = Convert.ToUInt64(v); break;
                case BuiltInType.Float: value = Convert.ToSingle(v); break;
                case BuiltInType.Double: value = Convert.ToDouble(v); break;
            }

            return value;
        }

        bool bSendDataChangeCompleted;
        void CreateMonItemDataContext(FrameworkElement fe)
        {
            var dataContext = new MonitoredItemViewModel();
            dataContext.PropertyChanged += (o, e) =>
                {
                    if (e.PropertyName == "Value")
                    {
                        var dc = fe.DataContext as MonitoredItemViewModel;
                        if (!dc.IsChanging && dc.DataValue != null && dc.nodeId != null)
                        {
                            busyIndicator.IsBusy = true;
                            busyIndicator.BusyContent = ApplicationStrings.BusyIndicatorExecutingCommand;

                            Object v = dc.Value;
                            v = ChangeType(v, dc.builtInType); // prechange type base on currenthread localization first
                            dc.DataValue.Value.Value.SetValue(v);

                            client.SendDataChangeAsync(session, new ScreenDataUpdate()
                                                                {
                                                                    uri = uri,
                                                                    nodeId = dc.nodeId,
                                                                    dataValue = dc.DataValue,
                                                                    Name = fe.Name
                                                                });

                            if (bSendDataChangeCompleted)
                                return;
                            bSendDataChangeCompleted = true;
                            client.SendDataChangeCompleted += (ob, ev) =>
                            {
                                busyIndicator.IsBusy = false;
                                if (ev.Error != null)
                                {
                                    MessageBox.Show(ev.Error.ToString());
                                    // ErrorWindow.CreateNew(ev.Error);
                                }
                            };
                        }
                    }
                };

            fe.DataContext = dataContext;
        }

        List<UIElement> mapStatus = new List<UIElement>();
        List<UIElement> mapDataWaiting = new List<UIElement>();

        private void SetDynamicSource(UIElement uie, bool bSet = true, uint Code = Opc.Ua.StatusCodes.Good)
        {
            if (Code != Opc.Ua.StatusCodes.Good)
            {
                if (!mapStatus.Contains(uie))
                {
                    mapStatus.Add(uie);
                    //var effect = new DropShadowEffect
                    //{
                    //    ShadowDepth = 0,
                    //    BlurRadius = 50,
                    //    Color = Colors.Red
                    //};
                    //uie.Effect = effect;
                    uie.BlinkScale(0.8, 250, new SineEase { EasingMode = EasingMode.EaseIn });
                    Opc.Ua.StatusCode status = new Opc.Ua.StatusCode(Code);
                    ToolTipService.SetToolTip(uie, status.ToString());
                }
            }
            else
            {
                if (mapStatus.Contains(uie))
                {
                    mapStatus.Remove(uie);
                    ToolTipService.SetToolTip(uie, null);
                    // uie.Effect = null;
                    uie.BlinkScale(0.8, 0, null);
                }
            }

            if (bSet)
            {
                if (!mapDataWaiting.Contains(uie))
                {
                    mapDataWaiting.Add(uie);
                    ToolTipService.SetToolTip(uie, "Waiting for data from the server...");
                    uie.Blink(500, 0, 1, new SineEase { EasingMode = EasingMode.EaseIn });
                    if (uie is ContentControl)
                        (uie as ContentControl).IsEnabled = false;
                }
            }
            else
            {
                if (mapDataWaiting.Contains(uie))
                {
                    mapDataWaiting.Remove(uie);
                    ToolTipService.SetToolTip(uie, null);
                    uie.Blink(0, 0, 0, null);
                    if (uie is ContentControl)
                        (uie as ContentControl).IsEnabled = true;
                }
            }
        }

        private void SetCommandSource(UIElement uie, String name)
        {
            // TODO : WP7 and SL3 don't support ICommand, we need to hack manually
            Command.ToString(); // instantiate the Command property
            if (uie is Button)
            {
                var btn = uie as Button;
                btn.Click += (o, e) =>
                    {
                        ExecuteCommand(name);
                    };
            }
            //uie.GetType().GetProperty("CommandParameter").SetValue(uie, name, null);
            //uie.GetType().GetProperty("Command").SetValue(uie, Command, null);
        }

        private static String ConvertXaml(String xaml)
        {
            if (!xaml.Contains("xmlns:mc=\"http://schemas.openxmlformats.org/markup-compatibility/2006\""))
                xaml = xaml.Replace("xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"",
                    "xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:mc=\"http://schemas.openxmlformats.org/markup-compatibility/2006\" mc:Ignorable=\"d\"");

            // TODO :
            xaml = xaml.Replace("{x:Type Button}", "Button");
            xaml = xaml.Replace("{x:Type ToggleButton}", "ToggleButton");
            xaml = xaml.Replace("{x:Type RadioButton}", "RadioButton");
            xaml = xaml.Replace("{x:Type CheckBox}", "CheckBox");
            xaml = xaml.Replace("{x:Type Border}", "Border");
            xaml = xaml.Replace("{x:Type ProgressBar}", "ProgressBar");
            xaml = xaml.Replace("{x:Type TextBlock}", "TextBlock");
            xaml = xaml.Replace(", UpdateSourceTrigger=PropertyChanged", "");
            xaml = xaml.Replace("UpdateSourceTrigger=PropertyChanged, ", "");
            xaml = xaml.Replace("UpdateSourceTrigger=PropertyChanged", "");

            
            if (xaml.Contains("Viewbox"))
            {
                xaml = xaml.Replace("xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"",
                    "xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:cs=\"clr-namespace:System.Windows.Controls;assembly=UFWP7Client\"");
                xaml = xaml.Replace("<Viewbox", "<cs:Viewbox");
                xaml = xaml.Replace("</Viewbox", "</cs:Viewbox");
            }
            else if (xaml.Contains("DataGrid"))
            {
                xaml = xaml.Replace("xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"",
                    "xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:sdk=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk\"");
                xaml = xaml.Replace("<DataGrid", "<sdk:DataGrid");
                xaml = xaml.Replace("</DataGrid", "</sdk:DataGrid");
            }
            else if (xaml.Contains("UpDown"))
            {
                xaml = xaml.Replace("<UpDown", "<tools:NumericUpDown");
                xaml = xaml.Replace("</UpDown", "</tools:NumericUpDown");
                xaml = xaml.Replace("xmlns=\"http://schemas.syncfusion.com/wpf\"", "xmlns:tools=\"clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.Silverlight\"");
            }
            else if (xaml.Contains("RangeSliderControl"))
            {
                xaml = xaml.Replace("<RangeSliderControl", "<tools:RangeSlider");
                xaml = xaml.Replace("</RangeSliderControl", "</tools:RangeSlider");
                xaml = xaml.Replace("xmlns=\"http://schemas.syncfusion.com/wpf\"", "xmlns:tools=\"clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.Silverlight\"");
            }

            return xaml;
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            bPollingEnabled = true;
            //if (!WebContext.Current.User.IsAuthenticated)
            //{
            //    WebContext.Current.Authentication.LoggedIn += (o, ev) =>
            //    {
            //        Initialize();
            //    };
            //    WebContext.Current.Authentication.LoggedOut += (o, ev) =>
            //    {
            //        NavigationService.GoBack();
            //    };
            //    var child = new LoginRegistrationWindow();
            //    child.Closed += (o, ev) =>
            //    {
            //        if (!WebContext.Current.User.IsAuthenticated)
            //            NavigationService.GoBack();
            //    };
            //    child.Show();
            //}
            //else
            Initialize();
        }

        private int GetScreenHashAndPreloadData(Uri uri)
        {
            try
            {
                screenStorage = IsolatedStorageManager.LoadData<ScreenStorage>(uri.AbsolutePath);
            }
            catch (Exception ex)
            {
                return 0;
            }

            return screenStorage.hash;
        }

        // Executes when the user navigates away from this page.
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            bPollingEnabled = false;
            if (client == null)
                return;

            try
            {
                client.CloseAsync(session, uri);
            }
            catch 
            {
                client = null;
            }

            mapNameElement.Clear();
            mapNameAnimation.Clear();
        }

        private EndpointAddress GetEndPoint()
        {
            String host = settings.ServerSetting;
            if (NavigationContext != null)
            {
                IDictionary<string, string> parameters = NavigationContext.QueryString;
                if (parameters.ContainsKey("host"))
                    host = parameters["host"];
            }

            return new EndpointAddress(
                "http://" +
                host +
                ":10010/ScreenManagerPoll");
            //return new EndpointAddress(
            //    "net.tcp://" +
            //    Application.Current.Host.Source.DnsSafeHost +
            //    ":4502/ScreenManager");
        }

        #region Commands

        RelayCommand _Command;

        public ICommand Command
        {
            get
            {
                if (_Command == null)
                {
                    _Command = new RelayCommand(
                        param => ExecuteCommand(param),
                        param => CanExecuteCommand(param)
                        );
                }
                return _Command;
            }
        }

        bool bCommandSent;
        bool bExecuteCommandCompleted;
        void ExecuteCommand(Object param)
        {
            var name = param as String;
            if (name == null)
                return;
            busyIndicator.IsBusy = true;
            busyIndicator.BusyContent = ApplicationStrings.BusyIndicatorExecutingCommand;
            client.ExecuteCommandAsync(session, uri, name);
            if (bExecuteCommandCompleted)
                return;
            bExecuteCommandCompleted = true;
            client.ExecuteCommandCompleted += (o, e) =>
                {
                    if (e.Error != null)
                    {
                        busyIndicator.IsBusy = false;

                        MessageBox.Show(e.Error.ToString());
                        // ErrorWindow.CreateNew(e.Error);
                    }
                    else
                    {
                        bCommandSent = true;
                        StartGettingRemoteExecute();
                    }
                };
        }

        bool CanExecuteCommand(Object param)
        {
            var name = param as String;
            if (name == null)
                return false;
            return listEnabledCommands.Contains(name);
        }

        #endregion Commands

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            screenViewer.viewBox.Stretch = Stretch.Fill;
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            screenViewer.viewBox.Stretch = Stretch.None;
        }
    }
}