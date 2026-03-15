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
using Syncfusion.Windows.Controls.Theming;
using UFWebClient.Helpers;
using UFWebClient.ScreenManagerServiceReference;
using UFWebClient.ViewModelLib;
using Utilities.Animations;
using ViewModelLib;
using System.Globalization;
using ComponentArt.UFSolution.Demos;

namespace UFWebClient.Views
{
    public partial class ScreenContainer : Page
    {
        ScreenManagerClient client;
        Uri uri;

        List<String> listEnabledCommands = new List<String>();
        Dictionary<String, FrameworkElement> mapNameElement = new Dictionary<String, FrameworkElement>();
        Dictionary<String, Dictionary<Guid, AnimationManager.AnimationManager>> mapNameAnimation = new Dictionary<String, Dictionary<Guid, AnimationManager.AnimationManager>>();
        Dictionary<FrameworkElement, MonitoredItemViewModel> mapDataContext = new Dictionary<FrameworkElement, MonitoredItemViewModel>();

        ScreenStorage screenStorage = new ScreenStorage();

        public ScreenContainer(Uri u)
        {
            InitializeComponent();

            SkinManager.SetVisualStyle(this, VisualStyle.Metro);

            uri = u;
        }

        public ScreenContainer()
        {
            InitializeComponent();

            SkinManager.SetVisualStyle(this, VisualStyle.Metro);
        }

        private void Initialize()
        {
            if (uri == null)
            {
                var requestedUri = App.Current.Resources["requestedUri"] as Uri;

                if (requestedUri != null)
                    uri = requestedUri;
                else
                {
                    ErrorWindow.CreateNew("Missing Uri, please enter a valid uri to request");
                    return;
                }
            }

            CustomBinding bindingHttp = new CustomBinding(
                            new PollingDuplexBindingElement()
                                        {
                                            DuplexMode = PollingDuplexMode.MultipleMessagesPerPoll,
                                            MaxPendingMessagesPerSession = int.MaxValue
                                        },
                            new BinaryMessageEncodingBindingElement(),
                            new HttpTransportBindingElement()
                                        {
                                            MaxReceivedMessageSize = int.MaxValue,
                                            MaxBufferSize = int.MaxValue,
                                            TransferMode = TransferMode.StreamedResponse
                                        });

            //CustomBinding bindingTcp = new CustomBinding(
            //                new BinaryMessageEncodingBindingElement(),
            //                new TcpTransportBindingElement()
            //                {
            //                    MaxReceivedMessageSize = int.MaxValue,
            //                    MaxBufferSize = int.MaxValue
            //                });

            EndpointAddress endpoint = GetEndPoint();
            if (endpoint.Uri.Scheme.Equals("https", StringComparison.InvariantCultureIgnoreCase))
                bindingHttp.Elements.Add(new TransportSecurityBindingElement());
            client = new ScreenManagerClient(bindingHttp, endpoint);

            bool bInStorageSaved = false;
            client.UpdateReceived += (o, ev) =>
                {
                    try
                    {
                        if (ev.update1.notFound)
                        {
                            busyIndicator.IsBusy = false;
                            ErrorWindow.CreateNew(String.Format("the Screen {0} does not exist on the server", ev.update1.uri));
                        }
                        else if (ev.update1.IsResource)
                        {
                            var xaml = ConvertXaml(ev);
                            ResourceDictionary dict = XamlReader.Load(xaml) as ResourceDictionary;
                            screenViewer.MainSurface.Resources.MergedDictionaries.Add(dict);
                            screenStorage.listResources.Add(xaml);
                        }
                        else if (ev.update1.IsBackground)
                        {
                            if (ev.update1.hashOk)
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
                                        if (uie.Effect is ShaderEffects.ReflectionEffect)
                                            (uie.Effect as ShaderEffects.ReflectionEffect).UpdateBinding(uie as FrameworkElement);

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

                                        if (obj.Rect.Left != System.Double.PositiveInfinity)
                                        {
                                            Canvas.SetLeft(fe, obj.Rect.Left);
                                            Canvas.SetTop(fe, obj.Rect.Top);
                                            fe.Width = obj.Rect.Width;
                                            fe.Height = obj.Rect.Height;
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
                                screenStorage.canvasXaml = ConvertXaml(ev);
                                UIElement uie = XamlReader.Load(screenStorage.canvasXaml) as UIElement;

                                Canvas stub = uie as Canvas;
                                foreach (var key in stub.Resources.Keys)
                                {
                                    if (screenViewer.MainSurface.Resources.Contains(key))
                                        screenViewer.MainSurface.Resources.Remove(key);
                                    screenViewer.MainSurface.Resources.Add(key, stub.Resources[key]);
                                }
                                screenViewer.MainSurface.Width = stub.Width;
                                screenViewer.MainSurface.Height = stub.Height;
                                screenViewer.MainSurface.Background = stub.Background;

                                progressDownload.Maximum = ev.update1.maxCount;
                                progressDownload.Minimum = 0;
                                progressDownload.Value = 0;
                                progressDownload.Visibility = Visibility.Visible;
                            }
                        }
                        else if (!String.IsNullOrEmpty(ev.update1.xaml) ||
                                 !String.IsNullOrEmpty(ev.update1.Name))
                        {
                            String xaml = null;
                            FrameworkElement uie = null;
                            if (!String.IsNullOrEmpty(ev.update1.xaml))
                            {
                                xaml = ConvertXaml(ev);
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

                                if (!String.IsNullOrEmpty(ev.update1.StyleResource))
                                    uie.Style = screenViewer.MainSurface.Resources[ev.update1.StyleResource] as Style;
                                if (!String.IsNullOrEmpty(ev.update1.BrushResource))
                                {
                                    if (uie is Panel)
                                        (uie as Panel).Background = screenViewer.MainSurface.Resources[ev.update1.BrushResource] as Brush;
                                    else if (uie is Control)
                                        (uie as Control).Background = screenViewer.MainSurface.Resources[ev.update1.BrushResource] as Brush;
                                    else if (uie is System.Windows.Shapes.Shape)
                                        (uie as System.Windows.Shapes.Shape).Fill = screenViewer.MainSurface.Resources[ev.update1.BrushResource] as Brush;
                                }
                                if (!String.IsNullOrEmpty(ev.update1.PenResource))
                                {
                                    if (uie is Control)
                                        (uie as Control).BorderBrush = screenViewer.MainSurface.Resources[ev.update1.PenResource] as Brush;
                                    else if (uie is System.Windows.Shapes.Shape)
                                        (uie as System.Windows.Shapes.Shape).Stroke = screenViewer.MainSurface.Resources[ev.update1.PenResource] as Brush;
                                }

                                if (uie.Effect is ShaderEffects.ReflectionEffect)
                                    (uie.Effect as ShaderEffects.ReflectionEffect).UpdateBinding(uie);
                                if (ev.update1.Left != System.Double.PositiveInfinity)
                                {
                                    Canvas.SetLeft(uie, ev.update1.Left);
                                    Canvas.SetTop(uie, ev.update1.Top);
                                    uie.Width = ev.update1.Width;
                                    uie.Height = ev.update1.Height;
                                }
                                screenViewer.MainSurface.Children.Add(uie);
                            }
                            else
                            {
                                foreach (var child in screenViewer.MainSurface.Children)
                                {
                                    uie = (child as FrameworkElement).FindName(ev.update1.Name) as FrameworkElement;
                                    if (uie != null)
                                        break;
                                }

                                if (uie == null)
                                    throw new ArgumentException(String.Format("Cannot find the element for {1}", ev.update1.Name));
                            }

                            screenStorage.listObjects.Add(new ScreenObject()
                                {
                                    Name = ev.update1.Name,
                                    Xaml = xaml,
                                    StyleResource = ev.update1.StyleResource,
                                    Rect = new Rect(ev.update1.Left, ev.update1.Top,
                                                    ev.update1.Width, ev.update1.Height),
                                    BrushResource = ev.update1.BrushResource,
                                    PenResource = ev.update1.PenResource
                                });

                            if (!String.IsNullOrEmpty(ev.update1.Name) && !mapNameElement.ContainsKey(ev.update1.Name))
                                mapNameElement.Add(ev.update1.Name, uie);

                            if (ev.update1.IsCommandSource)
                            {
                                SetCommandSource(uie, ev.update1.Name);
                                screenStorage.listCommandSources.Add(ev.update1.Name);
                            }

                            if (ev.update1.IsDynamic)
                            {
                                CreateMonItemDataContext(uie as FrameworkElement);
                                SetDynamicSource(uie);
                                screenStorage.listDynamicSources.Add(ev.update1.Name);
                            }

                            if (ev.update1.animationManagerList != null)
                            {
                                var list = AnimationManager.AnimationManager.ConvertListFromService(ev.update1.animationManagerList);
                                screenStorage.mapAnimations.Add(ev.update1.Name, list);
                                UpdateAnimationListMap(uie, ev.update1.Name, list);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorWindow.CreateNew(ex);
                    }
                    finally
                    {
                        busyIndicator.IsBusy = false;

                        if (!ev.update1.hashOk && !ev.update1.notFound &&
                            !ev.update1.IsResource && !ev.update1.IsBackground)
                        {
                            progressDownload.Value = ev.update1.ActualCount;
                        }

                        if (ev.update1.IsTermination)
                        {
                            progressDownload.Visibility = Visibility.Collapsed;

                            if (screenStorage != null && !bInStorageSaved && !ev.update1.hashOk)
                            {
                                bInStorageSaved = true;
                                screenStorage.uri = uri;
                                screenStorage.hash = ev.update1.hash;

                                var background = new BackgroundWorker();
                                background.DoWork += (i, e) =>
                                    {
                                        IsolatedStorageManager.SaveData(uri.AbsolutePath, screenStorage);
                                        screenStorage = null;
                                    };

                                background.RunWorkerCompleted += (i, e) =>
                                    {
                                        if (e.Error != null)
                                        {
                                            QuotaManager window = new QuotaManager();
                                            window.Closed += (h, f) =>
                                                {
                                                    try
                                                    {
                                                        IsolatedStorageManager.SaveData(uri.AbsolutePath, screenStorage);
                                                        screenStorage = null;
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        ErrorWindow.CreateNew(ex);
                                                    }
                                                };
                                            window.Show();
                                        }
                                    };
                                background.RunWorkerAsync();
                            }
                        }
                    }
                };

            client.RemoteRequestReceived += (o, ev) =>
                {
                    if (ev.update.request.executionMode == ExecutionMode.Synchro)
                    {
                        var container = new ScreenContainer
                            {
                                uri = ev.update.request.uri
                            };
                        var child = new ChildWindow
                        {
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
                        client.CloseAsync(uri);
                        uri = ev.update.request.uri;
                        screenViewer.MainSurface.Children.Clear();
                        mapNameElement.Clear();
                        mapNameAnimation.Clear();
                        mapDataContext.Clear();

                        OnNavigatedTo(null);
                    }
                };

            client.CommandUpdateReceived += (o, e) =>
                {
                    if (e.update.IsEnabled)
                    {
                        if (!listEnabledCommands.Contains(e.update.Name))
                        {
                            listEnabledCommands.Add(e.update.Name);
                            Execute.OnUIThread(() =>
                                {
                                    if (_Command != null)
                                        _Command.RaiseCanExecuteChanged();
                                });
                        }
                    }
                    else
                    {
                        if (listEnabledCommands.Contains(e.update.Name))
                        {
                            listEnabledCommands.Remove(e.update.Name);
                            Execute.OnUIThread(() =>
                                {
                                    if (_Command != null)
                                        _Command.RaiseCanExecuteChanged();
                                });
                        }
                    }
                };

            client.DataUpdateReceived += (o, e) =>
                {
                    status.Opacity = 1.0;
                    status.Fade(0, 2000, new BackEase() { EasingMode = EasingMode.EaseInOut });

                    if (!e.update.IsKeepAlive &&
                        mapNameElement.ContainsKey(e.update.Name))
                    {
                        if (e.update.dataValue != null && e.update.dataValue.Value.Value != null)
                        {
                            Type type = null;
                            Object value = null;
                            if (e.update.builtInType != BuiltInType.Null)
                            {
                                try
                                {
                                    type = Opc.Ua.TypeInfo.GetSystemType((Opc.Ua.BuiltInType)e.update.builtInType, e.update.valueRank);
                                    value = Convert.ChangeType(e.update.dataValue.Value.Value.Value, type, CultureInfo.InvariantCulture);
                                }
                                catch (Exception ex)
                                {
                                    ErrorWindow.CreateNew(ex);
                                }
                            }

                            FrameworkElement fe = mapNameElement[e.update.Name] as FrameworkElement;
                            if (e.update.IsDataContext)
                            {
                                var dataContext = fe.DataContext as MonitoredItemViewModel;
                                if (dataContext == null)
                                {
                                    if (mapDataContext.ContainsKey(fe))
                                    {
                                        fe.DataContext = mapDataContext[fe];
                                        dataContext = mapDataContext[fe];
                                    }
                                }

                                try
                                {
                                    dataContext.IsChanging = true;
                                    dataContext.nodeId = e.update.nodeId;
                                    dataContext.DataValue = e.update.dataValue;
                                    dataContext.Range = e.update.range;

                                    if (value != null)
                                    {
                                        dataContext.builtInType = e.update.builtInType;
                                        dataContext.valueRank = e.update.valueRank;
                                        dataContext.Value = value;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ErrorWindow.CreateNew(ex);
                                }
                                finally
                                {
                                    dataContext.IsChanging = false;
                                }
                            }
                            if (e.update.listGuid != null)
                            {
                                if (mapNameAnimation.ContainsKey(e.update.Name))
                                {
                                    try
                                    {
                                        var mapguid = mapNameAnimation[e.update.Name];
                                        foreach (var guid in e.update.listGuid)
                                        {
                                            if (!mapguid.ContainsKey(guid))
                                                continue;
                                            mapguid[guid].Range = e.update.range;
                                            mapguid[guid].ExecuteWithData(e.update.dataValue);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        ErrorWindow.CreateNew(ex);
                                    }
                                }
                            }

                            SetDynamicSource(mapNameElement[e.update.Name], false, e.update.dataValue.StatusCode.Code);
                        }
                    }
                };

            client.RegisterAsync(CultureInfo.CurrentCulture.Name);

            busyIndicator.IsBusy = true;
            client.OpenAsync(uri, GetScreenHashAndPreloadData(uri));
            client.OpenCompleted += (o, ev) =>
            {
                if (ev.Error != null)
                {
                    busyIndicator.IsBusy = false;

                    ErrorWindow.CreateNew(ev.Error);
                }
            };
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

                            client.SendDataChangeAsync(new ScreenDataUpdate()
                                                                {
                                                                    uri = uri,
                                                                    nodeId = dc.nodeId,
                                                                    dataValue = dc.DataValue,
                                                                    Name = fe.Name
                                                                });

                            client.SendDataChangeCompleted += (ob, ev) =>
                            {
                                busyIndicator.IsBusy = false;
                                if (ev.Error != null)
                                {
                                    ErrorWindow.CreateNew(ev.Error);
                                }
                            };
                        }
                    }
                };

            fe.DataContext = dataContext;
            mapDataContext.Add(fe, dataContext);
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
                    var effect = new DropShadowEffect
                    {
                        ShadowDepth = 0,
                        BlurRadius = 50,
                        Color = Colors.Red
                    };
                    uie.Effect = effect;
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
                    uie.Effect = null;
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
            uie.GetType().GetProperty("CommandParameter").SetValue(uie, name, null);
            uie.GetType().GetProperty("Command").SetValue(uie, Command, null);
        }

        private static String ConvertXaml(UpdateReceivedEventArgs ev)
        {
            String xaml = ev.update1.xaml;

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
            xaml = xaml.Replace("{DynamicResource ", "{StaticResource ");
            xaml = xaml.Replace("ComponentArt.Win.", "ComponentArt.Silverlight.");
            //xaml = xaml.Replace("<Style.Resources>", "");
            //xaml = xaml.Replace("<ResourceDictionary />", "");
            //xaml = xaml.Replace("</Style.Resources>", "");
            if (xaml.Contains("DataGrid"))
            {
                xaml = xaml.Replace("xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"",
                    "xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:sdk=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk\"");
                xaml = xaml.Replace("DataGrid", "sdk:DataGrid");
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
            if (client == null)
                return;

            try
            {
                client.CloseAsync(uri);
            }
            catch { }

            client = null;
            mapNameElement.Clear();
            mapNameAnimation.Clear();
        }

        private static EndpointAddress GetEndPoint()
        {
            return new EndpointAddress(
                "http://" +
                Application.Current.Host.Source.DnsSafeHost +
                ":10008/ScreenManager");
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

        void ExecuteCommand(Object param)
        {
            var name = param as String;
            if (name == null)
                return;
            busyIndicator.IsBusy = true;
            busyIndicator.BusyContent = ApplicationStrings.BusyIndicatorExecutingCommand;
            client.ExecuteCommandAsync(uri, name);
            client.ExecuteCommandCompleted += (o, e) =>
                {
                    busyIndicator.IsBusy = false;

                    if (e.Error != null)
                    {
                        ErrorWindow.CreateNew(e.Error);
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

        private void Grid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void Grid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
    }
}