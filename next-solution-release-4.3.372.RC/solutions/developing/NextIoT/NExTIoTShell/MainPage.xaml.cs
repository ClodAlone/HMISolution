using PubNubMessaging.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UFProjectManager.Upload;
using Utilities;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace NExTIoTShell
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
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

    public sealed partial class MainPage : Page
    {
        //static readonly String bootFilePath = @"c:\Data\Users\DefaultAccount\AppData\Local\Packages\ad4b2c90-8001-4666-a2ba-bc3f23a7f228_g2y2nmaav4rdp\LocalState\Main.ufproject";
        //static readonly String bootFolder = @"c:\Data\Users\DefaultAccount\AppData\Local\Packages\ad4b2c90-8001-4666-a2ba-bc3f23a7f228_g2y2nmaav4rdp\LocalState\";

        public MainPage()
        {
            this.InitializeComponent();

            Loaded += (o, e) =>
            {
                RunOnUIThread.Run(() => LoadBootableProject());
            };
        }

        async void LoadBootableProject()
        {
            var storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var app = Application.Current as App;
            var file = String.Format("{0}\\Main{1}", storageFolder.Path, app.UFProjectManagerComponent.FileType);
            if (!File.Exists(file))
            {
                var searchPattern = String.Format("*{0}", app.UFProjectManagerComponent.FileType);
                var files = Directory.GetFiles(storageFolder.Path, searchPattern);
                if (files.Length > 0)
                    file = files[0];
                else
                {
                    var folders = Directory.GetDirectories(storageFolder.Path);
                    foreach (var folder in folders)
                    {
                        files = Directory.GetFiles(folder, searchPattern);
                        if (files.Length > 0)
                        {
                            file = files[0];
                            break;
                        }
                    }
                }
            }

            if (File.Exists(file))
            {
                using (var showProgress = new ShowControl(busyPanel))
                {
                    StatusText.Text = "Starting...";
                    await System.Threading.Tasks.Task.Run(() =>
                    {
                        try
                        {
                            app.UFProjectManagerComponent.Execute(new Uri(file, UriKind.RelativeOrAbsolute), null, DocumentManager.ComponentService.ExecutionMode.Normal, null);
                        }
                        catch (Exception ex)
                        {
                            app.UIMsgBoxAlertServiceComponent.ShowError(
                                String.Format("An error occured running the project. Error : {0}", ex.Message));
                        }
                    });
                }
            }
            else
            {
                btnOpen.Visibility = Visibility.Visible;
                btnPublish.Visibility = Visibility.Visible;
                gridDeviceName.Visibility = Visibility.Visible;
            }
        }
    
        private async void Button_OpenClick(object sender, RoutedEventArgs e)
        {
            var app = Application.Current as App;
            var storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var openPicker = new FileOpenPicker();
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add(app.UFProjectManagerComponent.FileType);
            var file = await openPicker.PickSingleFileAsync();
            if (file == null)
            {
                app.UIMsgBoxAlertServiceComponent.ShowError(
                    String.Format("This platform does allow file browsing. Please put the startup project in the following path : {0}", storageFolder.Path));
                LoadBootableProject();
            }
            else
            {
                using (var showProgress = new ShowControl(busyPanel))
                {
                    StatusText.Text = "Starting...";
                    await System.Threading.Tasks.Task.Run(() =>
                {
                    try
                    {
                        app.UFProjectManagerComponent.Execute(new Uri(file.Path, UriKind.RelativeOrAbsolute), null, DocumentManager.ComponentService.ExecutionMode.Normal, null);
                    }
                    catch (Exception ex)
                    {
                        app.UIMsgBoxAlertServiceComponent.ShowError(
                            String.Format("An error occured running the project. Error : {0}", ex.Message));
                    }
                });
                }
            }
        }

        Pubnub pubnub;
        private async void Button_PublishClick(object sender, RoutedEventArgs e)
        {
            if (pubnub == null)
                pubnub = new Pubnub(UploadData.publishKey, UploadData.subscribeKey);
            else
                pubnub.EndPendingRequests();
            var storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var app = Application.Current as App;

            var uploadData = new UploadData()
            {
                localeStoragePath = storageFolder.Path,
                ipAddress = NetworkHelpers.GetCurrentIpv4Address(),
                hostName = NetworkHelpers.GetHostName(),
                deviceId = Guid.NewGuid()
            };

            try
            {
                var deviceName = txtDeviceName.Text;
                if (String.IsNullOrEmpty(deviceName))
                    deviceName = UploadData.deviceName;
                pubnub.Publish<String>(deviceName, uploadData.ToXml(),
                            (o) =>
                            {
                                pubnub.Subscribe<String>(deviceName,
                                    (result) =>
                                    {
                                        if (!string.IsNullOrEmpty(result) && !string.IsNullOrEmpty(result.Trim()))
                                        {
                                            List<object> deserializedMessage = pubnub.JsonPluggableLibrary.DeserializeToListOfObject(result);
                                            if (deserializedMessage != null && deserializedMessage.Count > 0)
                                            {
                                                object subscribedObject = (object)deserializedMessage[0];
                                                if (subscribedObject != null)
                                                {
                                                    //IF CUSTOM OBJECT IS EXCEPTED, YOU CAN CAST THIS OBJECT TO YOUR CUSTOM CLASS TYPE
                                                    // string resultActualMessage = pubnub.JsonPluggableLibrary.SerializeToJsonString(subscribedObject);
                                                    var resultActualMessage = subscribedObject as String;
                                                    if (!String.IsNullOrEmpty(resultActualMessage))
                                                    {
                                                        var uploadDataBack = resultActualMessage.FromXml<UploadData>();

                                                        pubnub.EndPendingRequests();
                                                        RunOnUIThread.Run(() =>
                                                        {
                                                            LoadBootableProject();
                                                        });
                                                    }
                                                }
                                            }
                                        }
                                    },
                                    (result) =>
                                    {
                                    //Console.WriteLine("SUBSCRIBE REGULAR CALLBACK:");
                                    //Console.WriteLine(result);
                                    },
                                    (pubnubError) =>
                                    {
                                    //Console.WriteLine(pubnubError.StatusCode);
                                    });
                            },
                            (error) =>
                            {
                                RunOnUIThread.Run(() =>
                                {
                                    app.UIMsgBoxAlertServiceComponent.ShowError(
                                    String.Format("An error occured publishing this device. Error : {0}", error));
                                });
                            });
            }
            catch(Exception ex)
            {
                RunOnUIThread.Run(() =>
                {
                    app.UIMsgBoxAlertServiceComponent.ShowError(
                    String.Format("An error occured publishing this device. Error : {0}", ex.Message));
                });
            }
        }
    }
}
