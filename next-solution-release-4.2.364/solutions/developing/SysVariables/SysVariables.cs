using Opc.Ua;
using OPCUAViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#if !WINDOWS_UWP && !NET_STANDARD
using System.Windows.Controls;
#endif
using TB.Instruments;
using UFInterfaces;
using Utilities;
using DocumentManager.ComponentService;
using DocumentManager.ComponentService.Helpers;

namespace SysVariables
{
    public class SysVariables : DataSinkInterface, IDisposable, IUFInterfaceBase
    {
#region Declarations
        Dictionary<String, MonitoredItemViewModel> mapSysVariables = new Dictionary<String, MonitoredItemViewModel>();
        Dictionary<IDocument, SysVariables> mapRunningVariables = new Dictionary<IDocument, SysVariables>();
        Object lockObject = new Object();
        IDocument currentDocument;
#if !WINDOWS_UWP
        Thread thread;
        ManualResetEvent startEvent;
#else
        Task thread;
#endif

        IDocument rootParent;
        bool bTerminate;
#endregion

#region Ctor
        public SysVariables()
        {
            AddSysVariable(SysNames.Blink200ms);
            AddSysVariable(SysNames.Blink500ms);
            AddSysVariable(SysNames.Blink1s);
            AddSysVariable(SysNames.Blink2500ms);
            AddSysVariable(SysNames.Blink5s);
            AddSysVariable(SysNames.Blink10s);
            AddSysVariable(SysNames.Blink30s);
            AddSysVariable(SysNames.Sine);
            AddSysVariable(SysNames.Square);
            AddSysVariable(SysNames.Triangle);
            AddSysVariable(SysNames.Sawtooth);
            AddSysVariable(SysNames.Pulse);
            AddSysVariable(SysNames.WhiteNoise);
            AddSysVariable(SysNames.GaussNoise);
            AddSysVariable(SysNames.DigitalNoise);

            AddSysVariable(SysNames.LastHostNameUsed);

            AddSysVariable(SysNames.CurrentTime);
            AddSysVariable(SysNames.CurrentDate);
            AddSysVariable(SysNames.CurrentLongDate);
            AddSysVariable(SysNames.CurrentUser);
            AddSysVariable(SysNames.CurrentRole);
            AddSysVariable(SysNames.CurrentAccessLevel);
            AddSysVariable(SysNames.CurrentAccessMask);
            AddSysVariable(SysNames.ActiveScreen);
            AddSysVariable(SysNames.LicenseSerialNumber);
            AddSysVariable(SysNames.MouseMove);

            AddSysVariable(SysNames.PI);
            AddSysVariable(SysNames.GPI);
            AddSysVariable(SysNames.NLP);
            AddSysVariable(SysNames.NP);

            AddSysVariable(SysNames.CurrentCulture);
            AddSysVariable(SysNames.CurrentConverter);

            AddSysVariable(SysNames.ServerConnectionError);
            AddSysVariable(SysNames.TotalConnectedClientTags);
            AddSysVariable(SysNames.PercentageOfConnectedStartupTags);

            AddSysVariable(SysNames.NumActiveWebClientUsers);

            AddSysVariable(SysNames.AlarmSoundActiveOnClient);
        }
#endregion

#region Singleton
        static Object singletonLocker = new Object();
        static SysVariables singletonInstance;
        static public SysVariables GetSysVariables()
        {
            lock(singletonLocker)
            {
                if (singletonInstance != null)
                    return singletonInstance;
                singletonInstance = new SysVariables();
                OPCUAEntityReference.RegisterDataSinkInterface(SysNames.dataSynkName, singletonInstance);
                return singletonInstance;
            }
        }

        static public SysVariables GetSysVariables(IDocument parent)
        {
            var instance = GetSysVariables();
            var rootParent = DocumentHelper.GetRootParent(parent, traverse: true);
            lock (instance.lockObject)
            {
                if (!instance.mapRunningVariables.ContainsKey(rootParent))
                    return instance;

                return instance.mapRunningVariables[rootParent];
            }
        }
        #endregion

#region Methods
        public void StartSysVariables()
        {
            lock (lockObject)
            {
                if (thread != null)
                    return;

                bTerminate = false;
#if !WINDOWS_UWP
                startEvent = new ManualResetEvent(false);
                thread = new Thread(WorkerProcedure);
                thread.IsBackground = true;
                thread.Priority = ThreadPriority.Lowest;
                thread.Start();
#else
                thread = Task.Run(() => WorkerProcedure());
#endif
            }

#if !WINDOWS_UWP
            startEvent.WaitOne();
            startEvent.Dispose();
#endif
        }

        public void EndSysVariables()
        {
            lock (lockObject)
            {
                if (thread == null)
                    return;
            }

            bTerminate = true;
#if !WINDOWS_UWP
            Thread.Sleep(0);
            thread.Join();
#else
            thread.Wait();
#endif
            lock (lockObject)
            {
                thread = null;
            }
        }

        void WorkerProcedure()
        {
            var var1 = new DataValue(new Variant(1), StatusCodes.Good);
            var var0 = new DataValue(new Variant(0), StatusCodes.Good);

            int nCounter = 0;
            bool bBlink200ms = false;
            bool bBlink500ms = false;
            bool bBlink1s = false;
            bool bBlink2500ms = false;
            bool bBlink5s = false;
            bool bBlink10s = false;
            bool bBlink30s = false;

            UpdateSysVariable(SysNames.CurrentUser, String.Empty);
            UpdateSysVariable(SysNames.CurrentRole, String.Empty);
            UpdateSysVariable(SysNames.CurrentAccessLevel, 0);
            UpdateSysVariable(SysNames.CurrentAccessMask, 0);
            UpdateSysVariable(SysNames.Blink200ms, var0);
            UpdateSysVariable(SysNames.Blink500ms, var0);
            UpdateSysVariable(SysNames.Blink1s, var0);
            UpdateSysVariable(SysNames.Blink2500ms, var0);
            UpdateSysVariable(SysNames.Blink5s, var0);
            UpdateSysVariable(SysNames.Blink10s, var0);
            UpdateSysVariable(SysNames.Blink30s, var0);
            UpdateSysVariable(SysNames.CurrentConverter, string.Empty);

#if !WINDOWS_UWP
            UpdateSysVariable(SysNames.PI, SysInfo.GetIndexPerformance());
            UpdateSysVariable(SysNames.GPI, SysInfo.GetIndexGraphicsPerformance());
            UpdateSysVariable(SysNames.NLP, SysInfo.GetNumberOfLogicalProcessors());
            UpdateSysVariable(SysNames.NP, SysInfo.GetNumberOfProcessors());
#endif

            UpdateSysVariable(SysNames.LastHostNameUsed, "");
            UpdateSysVariable(SysNames.ServerConnectionError, false);
            UpdateSysVariable(SysNames.TotalConnectedClientTags, 0);
            UpdateSysVariable(SysNames.PercentageOfConnectedStartupTags, 0.0);

            UpdateSysVariable(SysNames.NumActiveWebClientUsers, 0);

            UpdateSysVariable(SysNames.AlarmSoundActiveOnClient, true);

            UpdateSysVariable(SysNames.LicenseSerialNumber, "0");

            var sgSine = new SignalGenerator(SignalType.Sine);
            var sgSquare = new SignalGenerator(SignalType.Square);
            var sgTriangle = new SignalGenerator(SignalType.Triangle);
            var sgSawtooth = new SignalGenerator(SignalType.Sawtooth);
            var sgPulse = new SignalGenerator(SignalType.Pulse);
            var sgWhiteNoise = new SignalGenerator(SignalType.WhiteNoise);
            var sgGaussNoise = new SignalGenerator(SignalType.GaussNoise);
            var sgDigitalNoise = new SignalGenerator(SignalType.DigitalNoise);

#if !WINDOWS_UWP
            startEvent.Set();
#endif

            while (bTerminate == false)
            {
#if !WINDOWS_UWP
                Thread.Sleep(100);
#else
                Task.Delay(100).Wait();
#endif

                UpdateSysVariable(SysNames.Sine, sgSine.GetValue());
                UpdateSysVariable(SysNames.Square, sgSquare.GetValue());
                UpdateSysVariable(SysNames.Triangle, sgTriangle.GetValue());
                UpdateSysVariable(SysNames.Sawtooth, sgSawtooth.GetValue());
                UpdateSysVariable(SysNames.Pulse, sgPulse.GetValue());
                UpdateSysVariable(SysNames.WhiteNoise, sgWhiteNoise.GetValue());
                UpdateSysVariable(SysNames.GaussNoise, sgGaussNoise.GetValue());
                UpdateSysVariable(SysNames.DigitalNoise, sgDigitalNoise.GetValue());

                if (nCounter % 2 == 0)
                {
                    UpdateSysVariable(SysNames.Blink200ms, bBlink200ms ? var1 : var0);
                    bBlink200ms = !bBlink200ms;
                }
                if (nCounter % 5 == 0)
                {
                    UpdateSysVariable(SysNames.Blink500ms, bBlink500ms ? var1 : var0);
                    bBlink500ms = !bBlink500ms;
                }
                if (nCounter % 10 == 0)
                {
                    UpdateSysVariable(SysNames.Blink1s, bBlink1s ? var1 : var0);
                    bBlink1s = !bBlink1s;

                    var dt = DateTime.Now;
                    UpdateSysVariable(SysNames.CurrentTime, new DataValue(new Variant(dt.ToString(System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern)), StatusCodes.Good));
                    UpdateSysVariable(SysNames.CurrentDate, new DataValue(new Variant(dt.ToString(System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern)), StatusCodes.Good));
                    UpdateSysVariable(SysNames.CurrentLongDate, new DataValue(new Variant(dt.ToString(System.Globalization.DateTimeFormatInfo.InvariantInfo.FullDateTimePattern)), StatusCodes.Good));
                }
                if (nCounter % 25 == 0)
                {
                    UpdateSysVariable(SysNames.Blink2500ms, bBlink2500ms ? var1 : var0);
                    bBlink2500ms = !bBlink2500ms;
                }
                if (nCounter % 50 == 0)
                {
                    UpdateSysVariable(SysNames.Blink5s, bBlink5s ? var1 : var0);
                    bBlink5s = !bBlink5s;
                }
                if (nCounter % 100 == 0)
                {
                    UpdateSysVariable(SysNames.Blink10s, bBlink10s ? var1 : var0);
                    bBlink10s = !bBlink10s;
                }
                if (nCounter % 300 == 0)
                {
                    UpdateSysVariable(SysNames.Blink30s, bBlink30s ? var1 : var0);
                    bBlink30s = !bBlink30s;
                }
                ++nCounter;
            }
        }

        public bool AddSysVariable(String name)
        {
            lock(lockObject)
            {
                if (mapSysVariables.ContainsKey(name))
                    return false;

                mapSysVariables.Add(name, new MonitoredItemViewModel());
                return true;
            }
        }

        public void UpdateSysVariable(String name, bool value)
        {
            UpdateSysVariable(name, new DataValue(new Variant(value), StatusCodes.Good));
        }

        public void UpdateSysVariable(String name, double value)
        {
            UpdateSysVariable(name, new DataValue(new Variant(value), StatusCodes.Good));
        }

        public void UpdateSysVariable(String name, String value)
        {
            UpdateSysVariable(name, new DataValue(new Variant(value), StatusCodes.Good));
        }

        public void UpdateSysVariable(String name, DataValue value)
        {
            if (singletonInstance == this)
            {
                List<SysVariables> runningVariables;
                lock (lockObject)
                {
                    runningVariables = singletonInstance.mapRunningVariables.Values.ToList();
                }

                runningVariables.ForEach((data) => data.UpdateSysVariable(name, value));
            }
            else
            {
                MonitoredItemViewModel model = null;
                lock (lockObject)
                {
                    if (mapSysVariables.ContainsKey(name))
                        model = mapSysVariables[name];
                }
                if (model == null)
                    return;
                model.DataValue = value;
            }
        }

#region DataSinkInterface
        public string DataSynkName
        {
            get
            {
                return SysNames.dataSynkName;
            }
        }

        public bool IsProjectTypeAware(String projectType)
        {
            return true;
        }

        public void Start()
        {
            lock (lockObject)
            {
                if (currentDocument == null)
                    return;

                if (mapRunningVariables.ContainsKey(currentDocument))
                {
                    //if (UIInterface != null)
                    //    UIInterface.ShowWarning(Properties.Resource.CannotBeStartedTwice);
                    //log.Error(Properties.Resource.CannotBeStartedTwice);
                    return;
                }

                mapRunningVariables.Add(currentDocument, new SysVariables());
                mapRunningVariables[currentDocument].SetDocumentParent(currentDocument);
                mapRunningVariables[currentDocument].StartSysVariables();
            }
        }

        public void Stop()
        {
            lock (lockObject)
            {
                mapRunningVariables.Values.ToList().ForEach((data) => data.Dispose());
                mapRunningVariables.Clear();
            }
        }

        public void UpdateVariable(String name, DataValue value)
        {
            UpdateSysVariable(name, value);
        }

        public MonitoredItemViewModel GetVariable(String name, IDocument parent = null)
        {
            lock (lockObject)
            {
                if (parent != null)
                {
                    var rootParent = DocumentHelper.GetRootParent(parent, traverse: true);
                    if (mapRunningVariables.ContainsKey(rootParent))
                        return mapRunningVariables[rootParent].GetVariable(name);
                }
                else if (mapSysVariables.ContainsKey(name))
                    return mapSysVariables[name];
                return null;
            }
        }

        public List<MonitoredItemViewModel> GetRunningVariables()
        {
            lock (lockObject)
            {
                var ret = new List<MonitoredItemViewModel>();
                mapRunningVariables.Values.ToList().ForEach((data) => ret.AddRange(data.mapSysVariables.Values.ToList()));
                return ret;
            }
        }

        public string HumanReadableName
        {
            get
            {
                return Properties.Resource.AddressSpaceHeader;
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
                var ret = mapSysVariables.Keys.ToList();
                ret.AddRange(ServerTags.GetNames());
                return ret;
            }
        }

        public OPCUAEntityReference GetReference(String name)
        {
            if (ServerTags.Contains(name))
                return ServerTags.GetEntityReference(rootParent, name);
            else if (!CheckVariable(name))
                return new OPCUAEntityReference(null, DataSynkName, null, name, null, string.Format("{0} ({1})", name, DataSynkName), null);
            else
                return new OPCUAEntityReference(null, DataSynkName, null, name, null, name, null);
        }

#if !WINDOWS_UWP && !NET_STANDARD
        public UserControl Editor(bool bPopup = true)
        {
            return new Controls.ControlEditor(this);
        }
#endif

        public void SetDocumentParent(DocumentManager.ComponentService.IDocument parent)
        {
            rootParent = DocumentHelper.GetRootParent(parent, traverse: false);
            currentDocument = DocumentHelper.GetRootParent(parent, traverse: true);
        }

        public void DisposingDocumentParent(DocumentManager.ComponentService.IDocument parent)
        {
            lock (lockObject)
            {
                var root = DocumentHelper.GetRootParent(parent, traverse: true);
                if (mapRunningVariables.ContainsKey(root))
                {
                    mapRunningVariables[root].Dispose();
                    mapRunningVariables.Remove(root);
                }
                if (currentDocument == root)
                    currentDocument = null;
                if (rootParent == root)
                    rootParent = null;
            }
        }

#endregion
#endregion

#region IDisposable
        public void Dispose()
        {
            EndSysVariables();
            lock (lockObject)
                mapSysVariables.Clear();
        }
#endregion

#region IUFInterfaceBase
        public void Initialize()
        {
            GetSysVariables();
        }
#endregion

#if !WINDOWS_UWP && !NET_STANDARD
        public bool RemoveVariable(String name)
        {
            return false;
        }

        public void Copy(Uri uri, string newPath, bool bCopy, DocumentManager.ComponentService.IDocument parent)
        {
            
        }

        public bool Save(DocumentManager.ComponentService.IDocument parent, bool encryptFile = false)
        {
            return true;
        }

        public bool NeedsSave(IDocument parent)
        {
            return false;
        }

        public bool CheckSource(IDocument parent, object source)
        {
            return false;
        }
#endif
        public bool CheckVariable(string name)
        {
            if (ServerTags.Contains(name))
                return false;

            lock (lockObject)
                return !mapSysVariables.ContainsKey(name);
        }
    }
}
