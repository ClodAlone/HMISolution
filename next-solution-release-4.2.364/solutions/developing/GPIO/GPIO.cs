using Opc.Ua;
using OPCUAViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UFInterfaces;
using Utilities;
using DocumentManager.ComponentService;
using GPIO.Settings;
#if !WINDOWS_UWP
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using GPIO.Controls;
#else
using Windows.Devices.Gpio;
using Opc.Ua.Helpers;
#endif
using WPFUtilities;
using PubNubMessaging.Core;
using ViewModelLib;
using UIMsgBoxAlertService.ComponentService;
using DocumentManager.ComponentService.Helpers;

namespace GPIO
{
    public class GPIO : DataSinkInterface, IDisposable, IUFInterfaceBase
    {
#region Declarations
        Dictionary<String, MonitoredItemViewModel> mapVariables = new Dictionary<String, MonitoredItemViewModel>();
        Dictionary<String, Document> mapActiveDocuments = new Dictionary<String, Document>(StringComparer.OrdinalIgnoreCase);
        List<String> errorLoadingDocumentUris = new List<String>();

        Object lockObject = new Object();
        Document currentDocument;

        Pubnub pubnub;
        List<PropertyObserver<MonitoredItemViewModel>> listObserver;

        bool bStarted;

#if !WINDOWS_UWP
        DXWindow wndDebugger;
#else
        List<GpioPin> listPin;
#endif
        #endregion

        static readonly public String dataSynkName = "GPIOVariables";

#region Ctor
        public GPIO()
        {
        }
#endregion

#region Singleton
        static Object singletonLocker = new Object();
        static GPIO singletonInstance;
        static public GPIO GetGPIOVariables()
        {
            lock (singletonLocker)
            {
                if (singletonInstance != null)
                    return singletonInstance;
                singletonInstance = new GPIO();
                OPCUAEntityReference.RegisterDataSinkInterface(dataSynkName, singletonInstance);
                return singletonInstance;
            }
        }
#endregion

#region Methods

        void MapVariables()
        {
            mapVariables.Clear();
            if (currentDocument == null || currentDocument.ListPIN == null)
                return;
            currentDocument.ListPIN.ForEach(settings =>
            {
                AddVariable(settings.Name);
                UpdateVariable(settings.Name, false);
            });
        }

        public void StartVariables()
        {
            if (currentDocument == null || currentDocument.ListPIN == null)
                return;

            lock (lockObject)
            {
                MapVariables();
            }

            var token = Guid.NewGuid();
            var listPubNub = (from c in currentDocument.ListPIN where c.PubNubPublish == true select c).ToList();
            if (listPubNub.Count > 0)
            {
                pubnub = new PubNubMessaging.Core.Pubnub(currentDocument.PubNubPublishKey, currentDocument.PubNubSubscribeKey);
                listPubNub.ForEach(settings =>
                {
                    var monitoredItem = GetVariable(settings.Name);
                    if (monitoredItem != null)
                    {
                        var channelName = settings.PubNubChannel;
                        if (String.IsNullOrEmpty(channelName))
                            channelName = String.Format("{0}-{1}", currentDocument.Parent.Title, settings.Name);

                        if (listObserver == null)
                            listObserver = new List<PropertyObserver<MonitoredItemViewModel>>();
                        var observer = new PropertyObserver<MonitoredItemViewModel>(monitoredItem);
                        listObserver.Add(observer);

                        observer.RegisterHandler(m => m.DataValue, m =>
                        {
                            if (m.DataValue != null)
                            {
                                var extDataValue = new ExtDataValue(m.DataValue, token);
                                try
                                {
                                    pubnub.Publish<String>(channelName, extDataValue.ToXml(),
                                                (o) =>
                                                {
                                                    if (!StatusCode.IsGood(monitoredItem.DataValue.StatusCode))
                                                        monitoredItem.DataValue = new DataValue(new Variant(m.DataValue.Value), StatusCodes.Good);
                                                },
                                                (error) =>
                                                {
                                                    if (StatusCode.IsGood(monitoredItem.DataValue.StatusCode))
                                                        monitoredItem.DataValue = new DataValue(new Variant(m.DataValue.Value), StatusCodes.Bad);
                                                });
                                }
                                catch
                                {
                                    if (StatusCode.IsGood(monitoredItem.DataValue.StatusCode))
                                        monitoredItem.DataValue = new DataValue(new Variant(m.DataValue.Value), StatusCodes.Bad);
                                }
                            }
                        });

#if WINDOWS_UWP
                        // if (settings.Type == GPIOType.output)
#endif
                        {
                            pubnub.Subscribe<String>(
                                channelName,
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
                                                    var dataValue = resultActualMessage.FromXml<ExtDataValue>();
                                                    if (dataValue.Token != token && !dataValue.Equals(monitoredItem.DataValue))
                                                        monitoredItem.DataValue = dataValue;
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
                                    if (StatusCode.IsGood(monitoredItem.DataValue.StatusCode))
                                        monitoredItem.DataValue = new DataValue(new Variant(monitoredItem.DataValue.Value), StatusCodes.Bad);
                                });
                        }
                    }
                });
            }

#if !WINDOWS_UWP
            if (wndDebugger != null || mapVariables.Count == 0 || Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
                return;

            wndDebugger = new DXWindow()
            {
                Title = HumanReadableName,
                Content = new Debugger(this),
                BorderEffect = BorderEffect.Default,
                SizeToContent = System.Windows.SizeToContent.WidthAndHeight
            };

            ThemeHelper.SetTheme(wndDebugger);

            wndDebugger.Show();
#else
            var gpio = GpioController.GetDefault();
            if (gpio != null)
            {
                foreach(var settings in currentDocument.ListPIN)
                {
                    var monitoredItem = GetVariable(settings.Name);
                    if (monitoredItem == null)
                        continue;

                    try
                    {
                        var pin = gpio.OpenPin(settings.Pin);
                        if (pin == null)
                            continue;
                        if (listPin == null)
                            listPin = new List<GpioPin>();
                        listPin.Add(pin);

                        if (settings.Type == GPIOType.input)
                        {
                            if (settings.DebounceTimeout > 0)
                                pin.DebounceTimeout = TimeSpan.FromMilliseconds(settings.DebounceTimeout);
                            pin.SetDriveMode(GpioPinDriveMode.Input);
                            pin.ValueChanged += (o, e) =>
                            {
                                if (bStarted)
                                {
                                    var value = pin.Read();
                                    var dataValue = new DataValue(new Variant(value == GpioPinValue.High), StatusCodes.Good);
                                    if (!dataValue.Equals(monitoredItem.DataValue))
                                        monitoredItem.DataValue = dataValue;
                                }
                            };
                        }
                        else
                        {
                            pin.SetDriveMode(GpioPinDriveMode.Output);
                            pin.Write(GpioPinValue.Low);

                            if (listObserver == null)
                                listObserver = new List<PropertyObserver<MonitoredItemViewModel>>();
                            var observer = new PropertyObserver<MonitoredItemViewModel>(monitoredItem);
                            listObserver.Add(observer);

                            observer.RegisterHandler(m => m.DataValue, m =>
                            {
                                if (bStarted)
                                {
                                    try
                                    {
                                        bool value = false;
                                        if (m.DataValue != null)
                                            value = (bool)ChangeTypeHelper.ChangeType(m.DataValue.Value, BuiltInType.Boolean);
                                        pin.Write(value ? GpioPinValue.High : GpioPinValue.Low);

                                        if (!StatusCode.IsGood(monitoredItem.DataValue.StatusCode))
                                            monitoredItem.DataValue = new DataValue(new Variant(m.DataValue.Value), StatusCodes.Good, DateTime.UtcNow);
                                    }
                                    catch
                                    {
                                        if (StatusCode.IsGood(monitoredItem.DataValue.StatusCode))
                                            monitoredItem.DataValue = new DataValue(new Variant(m.DataValue.Value), StatusCodes.Bad);
                                    }
                                }
                            });
                        }
                    }
                    catch(Exception ex)
                    {
                        if (StatusCode.IsGood(monitoredItem.DataValue.StatusCode))
                            monitoredItem.DataValue = new DataValue(new Variant(monitoredItem.DataValue.Value), StatusCodes.Bad);
                    }
                }
            }
            else
            {
                Utilities.RunOnUIThread.RunIfRequired(() =>
                {
                    var uiInterface = currentDocument.Parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                    if (uiInterface != null)
                        uiInterface.ShowError(Properties.Resources.CannotFindGPIOInterface);
                });
            }
#endif

            bStarted = true;
        }

        public void EndVariables()
        {
            bStarted = false;
            if (listObserver != null)
            {
                listObserver.ForEach(observer => observer.Dispose());
                listObserver.Clear();
            }
            if (pubnub != null)
            {
                pubnub.EndPendingRequests();

                /*
                var listPubNub = (from c in currentDocument.ListPIN where c.PubNubPublish == true select c).ToList();
                if (listPubNub.Count > 0)
                {
                    listPubNub.ForEach(settings =>
                    {
                        if (settings.Type == GPIOType.output)
                        {
                            var channelName = settings.PubNubChannel;
                            if (String.IsNullOrEmpty(channelName))
                                channelName = String.Format("{0}-{1}", currentDocument.Parent.Title, settings.Name);

                            pubnub.Unsubscribe(channelName, 
                                DisplaySubscribeReturnMessage,
                                DisplaySubscribeDisconnectStatusMessage,
                                DisplaySubscribeConnectStatusMessage,
                                DisplayErrorMessage);
                        }
                    });
                }
                */
            }

#if !WINDOWS_UWP
            if (wndDebugger == null)
                return;
            wndDebugger.Close();
            wndDebugger = null;
#else
            if (listPin != null)
            {
                listPin.ForEach(pin => pin.Dispose());
                listPin.Clear();
            }
#endif
        }

        public bool AddVariable(String name)
        {
            lock (lockObject)
            {
                if (mapVariables.ContainsKey(name))
                    return false;

                mapVariables.Add(name, new MonitoredItemViewModel() { Title = name });
                return true;
            }
        }

        public void UpdateVariable(String name, bool value)
        {
            UpdateVariable(name, new DataValue(new Variant(value), StatusCodes.Good));
        }

        public void UpdateVariable(String name, double value)
        {
            UpdateVariable(name, new DataValue(new Variant(value), StatusCodes.Good));
        }

        public void UpdateVariable(String name, String value)
        {
            UpdateVariable(name, new DataValue(new Variant(value), StatusCodes.Good));
        }

        public void UpdateVariable(String name, DataValue value)
        {
            MonitoredItemViewModel model = null;
            lock (lockObject)
            {
                if (mapVariables.ContainsKey(name))
                    model = mapVariables[name];
            }
            if (model == null)
                return;
            model.DataValue = value;
        }

        #region DataSinkInterface
        public string DataSynkName
        {
            get
            {
                return dataSynkName;
            }
        }

        public bool IsProjectTypeAware(String projectType)
        {
            return projectType != null && projectType.Contains("IoT");
        }

        public void Start()
        {
            StartVariables();
        }

        public void Stop()
        {
            EndVariables();
        }

        public MonitoredItemViewModel GetVariable(String name, IDocument parent = null)
        {
            lock (lockObject)
            {
                if (mapVariables.ContainsKey(name))
                    return mapVariables[name];
                return null;
            }
        }

        public List<MonitoredItemViewModel> GetRunningVariables()
        {
            lock (lockObject)
            {
                return mapVariables.Values.ToList();
            }
        }

        public string HumanReadableName
        {
            get
            {
                return Properties.Resources.Title;
            }
        }

        public string TypeScheme
        {
            get
            {
                return null;
            }
        }

        public List<String> GetVariables(IDocument parent = null)
        {
            lock (lockObject)
            {
                return mapVariables.Keys.ToList();
            }
        }

        public OPCUAEntityReference GetReference(String name)
        {
            return new OPCUAEntityReference(null, DataSynkName, null, name, null, name, null);
        }

#if !WINDOWS_UWP
        public UserControl Editor(bool bPopup = true)
        {
            return new Controls.ControlEditor(this);
        }
#endif

        public void SetDocumentParent(DocumentManager.ComponentService.IDocument parent)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return;
#if !WINDOWS_UWP
            if (currentDocument != null && currentDocument != doc)
                currentDocument.SaveToFile();
#endif
            currentDocument = doc;
        }

        public void DisposingDocumentParent(DocumentManager.ComponentService.IDocument parent)
        {
            var p = DocumentHelper.GetRootParent(parent, traverse: false);
            var uri = String.Format(formatFile, p.rootBase);
            if (mapActiveDocuments.ContainsKey(uri))
            {
                if (mapActiveDocuments[uri] == currentDocument)
                {
#if !WINDOWS_UWP
                    currentDocument.SaveToFile();
#endif
                    currentDocument = null;
                }
                mapActiveDocuments.Remove(uri);
            }
            if (errorLoadingDocumentUris.Contains(uri))
                errorLoadingDocumentUris.Remove(uri);
        }

#endregion
#endregion

#region IDisposable
        public void Dispose()
        {
            EndVariables();
            lock (lockObject)
                mapVariables.Clear();
        }
#endregion

#region IUFInterfaceBase
        public void Initialize()
        {
            GetGPIOVariables();
        }
        #endregion

        #region DataSinkInterface

#if !WINDOWS_UWP && !NET_STANDARD
        public bool RemoveVariable(String name)
        {
            return false;
        }
#endif
#if !WINDOWS_UWP
        public void Copy(Uri uri, string newPath, bool bCopy, DocumentManager.ComponentService.IDocument parent)
        {
            using (new WaitCursor())
            {
                lock (lockObject)
                {
                    var sourcePath = String.Format(formatFile, uri.GetPathString());
                    var destPath = newPath;
                    if (!XpoHelpers.XpoHelper.IsDataSource(newPath))
                        destPath = String.Format(formatFile, newPath);

                    Document.CopyFile(sourcePath, destPath, bCopy, parent);
                }
            }
        }

        public bool Save(DocumentManager.ComponentService.IDocument parent, bool encryptFile = false)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return false;
            return doc.SaveToFile(forceEncryption: encryptFile);
        }

        public bool NeedsSave(IDocument parent)
        {
            var doc = GetOrCreateDocument(parent);
            if (doc == null)
                return false;
            return doc.NeedsToSave;
        }

        public bool CheckSource(IDocument parent, object source)
        {
            return false;
        }
#endif
        public bool CheckVariable(string name)
        {
            return !mapVariables.ContainsKey(name);
        }
#endregion

#region Methods

        static readonly String formatFile = "{0}\\GPIODataSink.settings";
        Document GetOrCreateDocument(IDocument parent, bool bRefresh = false)
        {
            if (parent == null)
                return null;

            lock (lockObject)
            {
                var p = DocumentHelper.GetRootParent(parent, traverse: false);
                var uri = String.Format(formatFile, p.rootBase);
                Dictionary<String, Document> map = mapActiveDocuments;
                var list = (from c in map/*.AsParallel()*/
                            where c.Key == uri//  && c.Value.Parent == p && c.Value.ActiveView == null
                            select c.Value).ToList();
                Document doc = null;
                if (list.Count == 0 || bRefresh)
                {
                    try
                    {
                        doc = Document.FromFile(uri, parent, bThrowExceptions: true);
                        errorLoadingDocumentUris.Remove(uri);
                    }
                    catch (Exception ex)
                    {
                        if (!errorLoadingDocumentUris.Contains(uri))
                        {
                            errorLoadingDocumentUris.Add(uri);
                            var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                            if (uiMsgBox != null)
                                uiMsgBox.ShowError(ex.Message);
                        }
                    }
                    if (doc == null)
                        doc = new Document();
                    doc.Parent = p;
                    doc.FullPath = uri;
                    if (!map.ContainsKey(uri))
                        map.Add(uri, doc);
                }
                else
                    doc = list[0];
                return doc;
            }
        }

        public Document CurrentDocument
        {
            get
            {
                return currentDocument;
            }
        }

#endregion
    }
}
