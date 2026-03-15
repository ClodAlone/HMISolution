using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using UFWebClient.Service;

namespace WPFTestClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    class ShowControl : IDisposable
    {
        static Dictionary<UIElement, int> mapElementCounter = new Dictionary<UIElement, int>();
        readonly UIElement control;
        public ShowControl(UIElement c)
        {
            control = c;
            if (!mapElementCounter.ContainsKey(control))
                mapElementCounter.Add(control, 0);
            mapElementCounter[control] += 1;
            control.Visibility = Visibility.Visible;
        }

        public void Dispose()
        {
            mapElementCounter[control] -= 1;
            if (mapElementCounter[control] == 0)
                control.Visibility = Visibility.Collapsed;
        }
    }

    public class EntityData : INotifyPropertyChanged
    {
        String statusText;
        public String StatusText
        {
            get
            {
                return statusText;
            }
            set
            {
                if (statusText == value)
                    return;
                statusText = value;
                OnPropertyChanged("StatusText");
            }
        }

        BitmapImage image;
        public BitmapImage Image
        {
            get
            {
                return image;
            }
            set
            {
                if (image == value)
                    return;
                image = value;
                OnPropertyChanged("Image");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public partial class MainWindow : Window
    {
        public IHubProxy HubProxy { get; set; }
        public HubConnection Connection { get; set; }

        DispatcherTimer blinkImageTimer;
        Dictionary<String, String> mapCacheElements = new Dictionary<String, String>();
        String currentStorage;
        String currentPage = "Default";

        public MainWindow()
        {
            InitializeComponent();

            Closing += (e, o) =>
            {
                if (Connection != null)
                {
                    bInitialized = false;
                    using (var showProgress = new ShowControl(busyPanel))
                    {
                        Connection.Stop();
                        Connection.Dispose();
                        Connection = null;
                    }
                }

                if (!String.IsNullOrEmpty(currentStorage))
                {
                    SaveData(currentStorage);
                    currentStorage = null;
                }
            };

            bool bFlipFlopImageTimer = false;
            blinkImageTimer = new DispatcherTimer();
            blinkImageTimer.Tick += (o, e) =>
            {
                blinkImage.Visibility = Visibility.Collapsed;
                blinkImageOk.Visibility = Visibility.Collapsed;
                blinkImageErr.Visibility = Visibility.Collapsed;

                if (bInitialized)
                {
                    bFlipFlopImageTimer = !bFlipFlopImageTimer;
                    if (!bFlipFlopImageTimer)
                        blinkImage.Visibility = Visibility.Visible;
                    else if (bInError)
                        blinkImageErr.Visibility = Visibility.Visible;
                    else
                        blinkImageOk.Visibility = Visibility.Visible;
                }
            };
            blinkImageTimer.Interval = TimeSpan.FromSeconds(1);
            blinkImageTimer.Start();
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            var settings = new Settings();
            settings.txtServer.Text = Properties.Settings.Default.ServerUrl;
            settings.txtUser.Text = Properties.Settings.Default.UserName;
            settings.txtPassword.Password = Properties.Settings.Default.UserPassword;
            var wnd = new Window()
            {
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
                Content = settings
            };
            var ret = wnd.ShowDialog();
            if (ret != true)
                return;

            Properties.Settings.Default.ServerUrl = settings.txtServer.Text;
            Properties.Settings.Default.UserName = settings.txtUser.Text;
            Properties.Settings.Default.UserPassword = settings.txtPassword.Password;
            Properties.Settings.Default.Save();
        }

        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(Properties.Settings.Default.ServerUrl))
            {
                BtnSettings_Click(sender, e);
                return;
            }

            btnConnect.Visibility = btnSettings.Visibility = Visibility.Collapsed;
            ConnectAsync();
        }

        private async void ConnectAsync()
        {
            var serverURI = String.Format("{0}/signalr", Properties.Settings.Default.ServerUrl);

            StatusText.Text = "Connecting to server...";
            using (var showProgress = new ShowControl(busyPanel))
            {
                Connection = new HubConnection(serverURI);
                Connection.Closed += Connection_Closed;
                Connection.ConnectionSlow += Connection_ConnectionSlow;
                Connection.Error += Connection_Error;
                Connection.StateChanged += Connection_StateChanged;
                HubProxy = Connection.CreateHubProxy("ScreenHub");

                try
                {
                    await Connection.Start();

                    // MessageBox.Show(String.Format("Hub connected with transport: {0}", Connection.Transport.Name));
                }
                catch (HttpRequestException ex)
                {
                    btnConnect.Visibility = btnSettings.Visibility = Visibility.Visible;
                    StatusText.Text = "Unable to connect to server: Start server before connecting clients.";
                    //No connection: Don't enable Send button or show chat UI
                    return;
                }
            }

            Register();
        }

        Dictionary<String, ElementData> mapElements = new Dictionary<String, ElementData>();
        Dictionary<String, EntityData> mapEntities = new Dictionary<String, EntityData>();
        Dictionary<String, FrameworkElement> mapControls = new Dictionary<String, FrameworkElement>();
        bool bInitialized;
        bool bInError;
        private async void Register()
        {
            currentPage = "Default";
            if (!String.IsNullOrEmpty(currentStorage))
            {
                SaveData(currentStorage);
                currentStorage = null;
            }

            using (var showProgress = new ShowControl(busyPanel))
            {
                StatusText.Text = "Registering to server...";
                try
                {
                    var element = await HubProxy.Invoke<ElementData>("Register", (int)scrollView.ActualWidth, (int)scrollView.ActualHeight, false,
                        Properties.Settings.Default.UserName, Properties.Settings.Default.UserPassword);
                    currentStorage = element.storageid.GetHashCode().ToString();

                    if (!String.IsNullOrEmpty(currentStorage))
                        LoadData(currentStorage);
                    layoutGrid.Width = element.width;
                    layoutGrid.Height = element.height;

                    ReadingBackgroundElements();

                    HubProxy.On<List<Guid>>("PendingChanges", list =>
                    {
                        Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (Action)(() =>
                        {
                            list.ForEach(id => ReadImage(id.ToString()));
                        }));
                    });

                    HubProxy.On<List<ElementData>>("PendingStatusChanges", list =>
                    {
                        Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (Action)(() =>
                        {
                            list.ForEach(el =>
                            {
                                if (mapControls.ContainsKey(el.id))
                                {
                                    Canvas.SetTop(mapControls[el.id], el.top);
                                    Canvas.SetLeft(mapControls[el.id], el.left);
                                    mapControls[el.id].Width = el.width;
                                    mapControls[el.id].Height = el.height;
                                    SetInError(mapControls[el.id], !el.connected);
                                    mapEntities[el.id].StatusText = el.LastMessage;
                                    mapElements[el.id].connected = el.connected;
                                    mapElements[el.id].writable = el.writable;
                                    mapElements[el.id].simulateEvent = el.simulateEvent;
                                    mapElements[el.id].dataType = el.dataType;
                                    SetInError(mapControls[el.id], !el.connected);
                                }
                            });
                        }));
                    });
                }
                catch(Exception ex)
                {
                    bInError = true;
                    ShowTextAndWait(ex.Message);
                }
                StatusText.Text = String.Empty;
            }

            bInitialized = true;
        }

        void CleanAllElements()
        {
            mapElements.Clear();
            mapEntities.Clear();
            mapControls.Clear();
            mainSurface.Children.Clear();
        }

        async void ReadingBackgroundElements()
        {
            CleanAllElements();

            StatusText.Text = "Reading background...";
            try
            {
                if (mapCacheElements.ContainsKey(currentPage))
                {
                    var binaryDataElCache = Convert.FromBase64String(mapCacheElements[currentPage]);
                    var bielCache = new BitmapImage();
                    bielCache.BeginInit();
                    bielCache.StreamSource = new MemoryStream(binaryDataElCache);
                    bielCache.EndInit();

                    backGround.Source = bielCache;
                }

                var element = await HubProxy.Invoke<ElementData>("GetBackground");

                var binaryData = Convert.FromBase64String(element.imageData);
                var bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = new MemoryStream(binaryData);
                bi.EndInit();
                backGround.Source = bi;

                if (mapCacheElements.ContainsKey(currentPage))
                    mapCacheElements.Remove(currentPage);
                mapCacheElements.Add(currentPage, element.imageData);

                StatusText.Text = "Reading elements...";
                var listelements = await HubProxy.Invoke<IList<ElementData>>("GetElementData");
                var item = TryFindResource("ItemTemplate") as DataTemplate;
                foreach (var el in listelements)
                {
                    if (mapElements.ContainsKey(el.id))
                        continue;
                    mapElements.Add(el.id, el);
                    mapEntities.Add(el.id, new EntityData() { StatusText = "Fetching..." });
                    var view = item.LoadContent() as FrameworkElement;
                    mapControls.Add(el.id, view);
                    view.DataContext = mapEntities[el.id];
                    Canvas.SetTop(view, el.top);
                    Canvas.SetLeft(view, el.left);
                    view.Width = el.width;
                    view.Height = el.height;
                    mainSurface.Children.Add(view);
                    mapEntities[el.id].StatusText = el.LastMessage;
                    view.IsHitTestVisible = el.simulateEvent;
                    if (!el.connected)
                        SetInError(view, true);
                    mapElements[el.id].connected = el.connected;
                    mapElements[el.id].writable = el.writable;
                    mapElements[el.id].simulateEvent = el.simulateEvent;
                    mapElements[el.id].dataType = el.dataType;
                    ReadImage(el.id);
                }
            }
            catch(Exception ex)
            {
                bInError = true;
                ShowTextAndWait(ex.Message);
            }
        }

        private void SetInError(FrameworkElement control, bool bError)
        {
            if (bError)
            {
                control.Effect = new DropShadowEffect() { Color = Colors.Red, BlurRadius = 5, ShadowDepth = 0 };
            }
            else
            {
                control.Effect = null;
            }
        }
        private async void ReadImage(String id)
        {
            if (!bInitialized)
                return;

            try
            {
                if (mapEntities[id].Image == null && mapCacheElements.ContainsKey(id))
                {
                    var binaryDataElCache = Convert.FromBase64String(mapCacheElements[id]);
                    var bielCache = new BitmapImage();
                    bielCache.BeginInit();
                    bielCache.StreamSource = new MemoryStream(binaryDataElCache);
                    bielCache.EndInit();

                    mapEntities[id].Image = bielCache;
                }

                var readingImage = await HubProxy.Invoke<ElementData>("GetImageBase64", id);
                if (!mapEntities.ContainsKey(id) ||
                    String.IsNullOrEmpty(readingImage.imageData))
                    return;

                mapEntities[id].StatusText = String.Empty;

                var binaryDataEl = Convert.FromBase64String(readingImage.imageData);
                var biel = new BitmapImage();
                biel.BeginInit();
                biel.StreamSource = new MemoryStream(binaryDataEl);
                biel.EndInit();

                mapEntities[id].Image = biel;

                if (mapCacheElements.ContainsKey(id))
                    mapCacheElements.Remove(id);
                mapCacheElements.Add(id, readingImage.imageData);
            }
            catch (Exception ex)
            {
                bInError = true;
                ShowTextAndWait(ex.Message);
            }
        }

        async void ShowTextAndWait(String text, int SecToWait = 2)
        {
            using (var showProgress = new ShowControl(busyPanel))
            {
                StatusText.Text = text;
                await Task.Delay(TimeSpan.FromSeconds(SecToWait));
            }
        }

        ShowControl connectionStatus;
        private void Connection_StateChanged(StateChange obj)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (Action)(() =>
            {
                if (obj.NewState == ConnectionState.Connected)
                {
                    if (connectionStatus != null)
                    {
                        connectionStatus.Dispose();
                        connectionStatus = null;
                    }
                    bInError = false;
                }
                else
                {
                    if (connectionStatus == null)
                        connectionStatus = new ShowControl(busyPanel);
                    bInError = true;
                }
                ShowTextAndWait(String.Format("{0}...", obj.NewState.ToString()));
            }));
        }

        private void Connection_Error(Exception obj)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (Action)(() =>
            {
                bInError = true;
                ShowTextAndWait(obj.Message);
            }));
        }

        private void Connection_ConnectionSlow()
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (Action)(() =>
            {
                ShowTextAndWait("Slow connection has been detected");
            }));
        }

        /// <summary>
        /// If the server is stopped, the connection will time out after 30 seconds (default), and the 
        /// Closed event will fire.
        /// </summary>
        void Connection_Closed()
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (Action)(() =>
            {
                btnConnect.Visibility = btnSettings.Visibility = Visibility.Visible;
                bInitialized = false;
                CleanAllElements();
                backGround.Source = null;
                ShowTextAndWait("You have been disconnected");
            }));
        }

        String mouseDownElement;
        private async void mainSurface_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!bInitialized || Connection == null)
                return;
            var pt = e.GetPosition(mainSurface);
            mouseDownElement = GetElementIdFromPos(pt);
            try
            {
                await HubProxy.Invoke("SendMouseDownEvent", (int)pt.X, (int)pt.Y);
            }
            catch (Exception ex)
            {
                bInError = true;
                ShowTextAndWait(ex.Message);
            }
        }

        DispatcherTimer timerFetchCommand;
        DataInfo currentDataInfo;
        ShowControl sendingCommand;
        private async void mainSurface_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!bInitialized || Connection == null)
                return;
            var pt = e.GetPosition(mainSurface);
            try
            {
                currentDataInfo = await HubProxy.Invoke<DataInfo>("SendMouseUpEvent", (int)pt.X, (int)pt.Y);
            }
            catch (Exception ex)
            {
                bInError = true;
                ShowTextAndWait(ex.Message);
                return;
            }

            var upDownElement = GetElementIdFromPos(pt);
            if (upDownElement == mouseDownElement && mouseDownElement != null)
            {
                if (!mapElements[upDownElement].writable && mapElements[upDownElement].dataType == -1 || !mapElements[upDownElement].connected)
                    return;

                if (sendingCommand == null)
                    sendingCommand = new ShowControl(busyPanel);
                if (mapElements[upDownElement].simulateEvent && (mapElements[upDownElement].dataType == -1 || mapElements[upDownElement].dataType == 0))
                {
                    StatusText.Text = "Sending Commands...";
                    if (timerFetchCommand == null)
                    {
                        timerFetchCommand = new DispatcherTimer();
                        timerFetchCommand.Interval = TimeSpan.FromSeconds(0);
                        timerFetchCommand.Tick += TimerFetchCommand_Tick;
                    }
                    timerFetchCommand.Start();
                }
                else
                {
                    StatusText.Text = "Querying data info...";
                    try
                    {
                        currentDataInfo = await HubProxy.Invoke<DataInfo>("SendQueryDataInfo", mapElements[upDownElement].id);
                    }
                    catch (Exception ex)
                    {
                        if (sendingCommand != null)
                        {
                            sendingCommand.Dispose();
                            sendingCommand = null;
                        }
                        bInError = true;
                        ShowTextAndWait(ex.Message);
                        return;
                    }

                    if (sendingCommand != null)
                    {
                        sendingCommand.Dispose();
                        sendingCommand = null;
                    }

                    if (currentDataInfo.selection != null)
                    {
                        listBox.Visibility = Visibility.Visible;
                        currentDataInfo.selection.ForEach(item => listBox.Items.Add(item));
                        inputPanel.Visibility = Visibility.Visible;
                        listBox.Focus();
                    }
                    else if (mapElements[upDownElement].dataType == 0)
                    {
                        checkBox.IsChecked = currentDataInfo.value == "True";
                        checkBox.Visibility = Visibility.Visible;
                        inputPanel.Visibility = Visibility.Visible;
                        checkBox.Focus();
                    }
                    else
                    {
                        textBox.Text = currentDataInfo.value;
                        textBox.Visibility = Visibility.Visible;
                        inputPanel.Visibility = Visibility.Visible;
                        textBox.SelectAll();
                        textBox.Focus();
                    }
                }
            }
        }

        String currentUrl = String.Empty;
        private async void TimerFetchCommand_Tick(object sender, EventArgs e)
        {
            timerFetchCommand.Stop();
            var commands = await HubProxy.Invoke<IList<CommandData>>("GetListCommands", currentUrl);
            foreach(var command in commands)
            {
                bInitialized = false;
                currentUrl = command.url;
                StatusText.Text = String.Format("Opening Screen {0}...", currentUrl);
                try
                {
                    var element = await HubProxy.Invoke<ElementData>("OpenUri", currentUrl, (int)layoutGrid.Width, (int)layoutGrid.Height);
                    layoutGrid.Width = element.width;
                    layoutGrid.Height = element.height;

                    currentPage = currentUrl;
                    ReadingBackgroundElements();

                    bInitialized = true;
                }
                catch (Exception ex)
                {
                    bInError = true;
                    ShowTextAndWait(ex.Message);
                }
            }
            if (sendingCommand != null)
            {
                sendingCommand.Dispose();
                sendingCommand = null;
            }
        }

        String GetElementIdFromPos(Point pt)
        {
            foreach(var id in mapElements.Keys.Reverse())
            {
                var rect = new Rect(mapElements[id].left, mapElements[id].top, mapElements[id].width, mapElements[id].height);
                if (rect.Contains(pt))
                    return id;
            }

            return null;
        }

        private async void btnOk_Click(object sender, RoutedEventArgs e)
        {
            using (var showProgress = new ShowControl(busyPanel))
            {
                StatusText.Text = "Sending Data Value...";

                String value = String.Empty;
                if (listBox.Visibility == Visibility.Visible)
                {
                    value = listBox.SelectedItem as String;
                }
                else if (textBox.Visibility == Visibility.Visible)
                {
                    value = textBox.Text as String;
                    if (currentDataInfo != null && mapElements[mouseDownElement].dataType == 1)
                    {
                        if (currentDataInfo.minValue != currentDataInfo.maxValue)
                        {
                            try
                            {
                                var v = Convert.ToDouble(value);
                                if (v < currentDataInfo.minValue || v > currentDataInfo.maxValue)
                                {
                                    ShowTextAndWait(String.Format("Not in the range ({0}, {1})", currentDataInfo.minValue, currentDataInfo.maxValue));
                                    return;
                                }
                            }
                            catch(Exception ex)
                            {
                                ShowTextAndWait(ex.Message);
                                return;
                            }
                        }
                    }
                }
                else if (checkBox.Visibility == Visibility.Visible)
                {
                    value = checkBox.IsChecked.ToString();
                }

                try
                {
                    if (currentDataInfo != null)
                    {
                        await HubProxy.Invoke("SendDataValueProvider", currentDataInfo.X, currentDataInfo.Y, value);
                    }
                    else
                        await HubProxy.Invoke("SendDataValue", mouseDownElement, value);
                }
                catch (Exception ex)
                {
                    bInError = true;
                    ShowTextAndWait(ex.Message);
                }
            }
            inputPanel.Visibility = Visibility.Collapsed;
            listBox.Visibility = Visibility.Collapsed;
            checkBox.Visibility = Visibility.Collapsed;
            textBox.Visibility = Visibility.Collapsed;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            inputPanel.Visibility = Visibility.Collapsed;
            listBox.Visibility = Visibility.Collapsed;
            checkBox.Visibility = Visibility.Collapsed;
            textBox.Visibility = Visibility.Collapsed;
        }

        #region Storage
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

        static String GetStoreFileName(String title)
        {
            return String.Format("{0}.{1}.WebClient.dat", title, System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));
        }

        void SaveData(String title)
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

                    using (XmlWriter writer = XmlWriter.Create(stream, settings))
                    {
                        try
                        {
                            var serializer = new DataContractSerializer(typeof(IDictionary<String, String>));
                            serializer.WriteObject(writer, mapCacheElements as IDictionary<String, String>);
                        }
                        catch (Exception ex)
                        {
                            writer.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        void LoadData(String title)
        {
            mapCacheElements.Clear();
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                return;

            try
            {
                var isoStorage = GetStorage();
                if (null != isoStorage && !String.IsNullOrEmpty(title))
                {
                    using (var stream = new IsolatedStorageFileStream(GetStoreFileName(title), FileMode.OpenOrCreate, isoStorage))
                    {
                        XmlReaderSettings settings = new XmlReaderSettings
                        {
                            ConformanceLevel = ConformanceLevel.Document,
                            CloseInput = true
                        };

                        using (XmlReader reader = XmlReader.Create(stream, settings))
                        {
                            var serializer = new DataContractSerializer(typeof(IDictionary<String, String>));
                            var map = serializer.ReadObject(reader) as IDictionary<String, String>;
                            foreach (var entry in map)
                            {
                                mapCacheElements[entry.Key] = entry.Value;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion
    }
}
