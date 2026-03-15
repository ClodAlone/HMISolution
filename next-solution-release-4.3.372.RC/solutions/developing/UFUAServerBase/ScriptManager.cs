using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinWrap.Basic;
using Opc.Ua;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Windows.Forms;
using Opc.Ua.Server;
using log4net;

namespace UFUAServerBase
{
    class ScriptManager : IDisposable
    {
        #region Declarations
        static readonly Dictionary<ScriptManager, BasicNoUIObj> mapbasicCtl = new Dictionary<ScriptManager, BasicNoUIObj>();
        static readonly List<ScriptManager> codeList = new List<ScriptManager>();
        protected Module module;
        protected readonly String Name;
        protected readonly BlockingCollection<Action> queue = new BlockingCollection<Action>();
        readonly List<String> listDebuggingData = new List<String>();
        DateTime lastTimeDebugging = DateTime.UtcNow;
        protected bool bDisposing;
        protected readonly ServerScriptManager.ServerScriptManager manager;
        protected ILog syslog;
        protected readonly int[] Breakpoints;
        protected BasicNoUIObj basicCtl;
        protected String Code;
        bool bOnError;
        static Object lockObject = new Object();
        static Thread thread;
        ManualResetEvent terminated = new ManualResetEvent(false);

        #endregion

        public ScriptManager(String name, BaseInstanceState i, StandardServer s, ServerSystemContext sc, int[] breakpoints,
            Dictionary<String, BaseInstanceState> pn, CustomNodeManager2 nm)
        {
            Name = name;
            manager = new ServerScriptManager.ServerScriptManager(i, s, sc, pn, nm);
            manager.InUseTag += (o, e) =>
            {
                OnInUseTag(o as NodeState);
            };

            Breakpoints = breakpoints;
        }

        internal virtual void InitScriptEngine(String code)
        {
            if (bDisposing || bQuitRequest)
                return;
            Code = code;
            lock (lockObject)
            {
                codeList.Add(this);
                if (thread == null)
                {
                    thread = new Thread(ob =>
                    {
                        int counter = 0;
                        ScriptManager[] indexArray = null;

                        while (codeList.Count > 0 || mapbasicCtl.Count > 0)
                        {
                            while (codeList.Count > 0)
                            {
                                ScriptManager initManager = null;
                                lock (lockObject)
                                {
                                    initManager = codeList[0];
                                    codeList.RemoveAt(0);
                                }

                                if (initManager.bDisposing || initManager.bQuitRequest || initManager.bOnError)
                                {
                                    initManager.terminated.Set();
                                    continue;
                                }

                                InitBasicControl(initManager);
                                counter = 0;
                                indexArray = null;
                            }

                            if (mapbasicCtl.Count == 0)
                            {
                                Thread.Sleep(1000);
                                continue;
                            }

                            if (indexArray == null)
                            {
                                indexArray = new ScriptManager[mapbasicCtl.Count];
                                mapbasicCtl.Keys.CopyTo(indexArray, 0);
                            }
                            ScriptManager currentManager = null;
                            BasicNoUIObj basicCtl = null;
                            lock (lockObject)
                            {
                                currentManager = indexArray[counter];
                                basicCtl = mapbasicCtl[currentManager];

                                if (++counter >= mapbasicCtl.Keys.Count)
                                    counter = 0;
                            }

                            if (currentManager.bDisposing || currentManager.bQuitRequest || currentManager.bOnError)
                            {
                                TerminateBasicControl(currentManager, basicCtl);
                                indexArray = null;
                                if (counter >= mapbasicCtl.Keys.Count)
                                    counter = 0;
                                continue;
                            }

                            //System.Diagnostics.Debug.WriteLine("Server Script Entering DoEvents");
                            try
                            {
                                currentManager.manager.OnDoEvents(EventArgs.Empty);
                            }
                            catch (TerminatedException)
                            {
                                // script execution has been terminated
                            }
                            catch (Exception ex)
                            {
                                basicCtl.ReportError(ex);
                            }
                            //System.Diagnostics.Debug.WriteLine("Server Script Exiting DoEvents");

                            //System.Diagnostics.Debug.WriteLine("Server Script Entering Application.DoEvent");
                            Util.DoEvents();
                            // basicCtl.RunThis("DoEvents");
                            // Application.DoEvents();
                            //System.Diagnostics.Debug.WriteLine("Server Script Exiting Application.Doevents");
                            if (currentManager.queue.Count > 0)
                            {
                                //System.Diagnostics.Debug.WriteLine("Server Script Entering Action");
                                var action = currentManager.queue.Take();
                                action();
                                //System.Diagnostics.Debug.WriteLine("Server Script Exiting Action");
                            }

                            Thread.Sleep(500 / mapbasicCtl.Count);
                        }
                    })
                    {
                        IsBackground = true
                    };
                    thread.Name = "Script Executer";
                    thread.SetApartmentState(ApartmentState.STA);
                }
            }
        }

        protected static void InitBasicControl(ScriptManager scriptManager)
        {
            BasicNoUIObj basicCtl = scriptManager.basicCtl;
            if (basicCtl == null)
            {
                if (scriptManager.Breakpoints != null && scriptManager.Breakpoints.Length > 0)
                    basicCtl = new BasicIdeObj();
                else
                    basicCtl = new BasicNoUIObj();

                lock (lockObject)
                    mapbasicCtl.Add(scriptManager, basicCtl);
            }
#if DEBUG
            if (System.IO.File.Exists(@"C:\Program Files (x86)\Polar Engineering\WinWrap Basic\Certificates\Application-a67e0d79.htm"))
#endif
                basicCtl.Secret = new Guid(Properties.Settings.Default.SecretKey);

            basicCtl.Initialize();

            basicCtl.Caption = scriptManager.Name;
            basicCtl.LargeIcon = null;
            basicCtl.SmallIcon = null;
            // basicCtl.LargeIcon = Properties.Resources.ScriptDebugger;
            basicCtl.TaskbarIconMode = WinWrap.Basic.TaskbarIconModeConstants.IconNoneSysmenuNone;
            if (!Environment.UserInteractive)
                Util.IgnoreDialogs = true;

            var opcua = typeof(DataValue).Assembly;
            basicCtl.AddExtension("#", opcua);

            var referenceGetTypeAssembly = scriptManager.manager.GetType().Assembly;
            basicCtl.AddExtension("#", referenceGetTypeAssembly);
            var scriptManagerName = scriptManager.manager.GetType().UnderlyingSystemType.Name;
            basicCtl.AddExtensionObjectWithEvents(scriptManagerName, scriptManager.manager);

            var serverName = scriptManager.manager.Instance.GetType().UnderlyingSystemType.Name;
            basicCtl.AddExtensionObjectWithEvents(serverName, scriptManager.manager.Instance);

            basicCtl.FileTools = false;
            basicCtl.EventMode = true;
            basicCtl.Code = scriptManager.Code;
            basicCtl.Changed = false;
            // basicCtl.Run = true;

            if (scriptManager.Breakpoints != null && scriptManager.Breakpoints.Length > 0)
            {
                var ctrl = basicCtl as BasicIdeObj;
                ctrl.CreateOverlappedWindow();
                ctrl.WindowState = FormWindowState.Maximized;
                ctrl.BreakPoints = scriptManager.Breakpoints;
            }

            try
            {
                scriptManager.module = basicCtl.ModuleInstance("", false);

                {
                    basicCtl.Synchronizing += (o, e) =>
                    {
                        lock (scriptManager.listDebuggingData)
                            scriptManager.listDebuggingData.Add(e.Param);

                        if (scriptManager.lastTimeDebugging.AddSeconds(10) < DateTime.UtcNow)
                        {
                            basicCtl.Synchronized = false;

                            lock (scriptManager.listDebuggingData)
                                scriptManager.listDebuggingData.Clear();
                        }
                    };

                    if (scriptManager.syslog == null)
                        scriptManager.syslog = LogManager.GetLogger(String.Format(Properties.Resources.ServerScript, scriptManager.Name));

                    basicCtl.ErrorAlert += (o, e) =>
                    {
                        scriptManager.bOnError = true;

                        var error = basicCtl.Error.ToString();
                        var text = String.Format(Properties.Resources.ScriptErrorDesc, error, scriptManager.Name,
                            basicCtl.Error.Line, basicCtl.Error.Offset);

                        scriptManager.syslog.Error(text);
                        System.Diagnostics.Debug.WriteLine(text);

                        basicCtl.Run = false;

                        //MessageBox.Show(text, Properties.Resources.ScriptError, MessageBoxButtons.OK,
                        //            MessageBoxIcon.Error, MessageBoxDefaultButton.Button1,
                        //            MessageBoxOptions.ServiceNotification);
                    };

                    basicCtl.DebugPrint += (o, e) =>
                    {
                        scriptManager.syslog.Debug(e.Text);
                    };

                    basicCtl.DoEvents += (o, e) =>
                    {
                        //System.Diagnostics.Debug.WriteLine("Server Script Inside DoEvent");
                        Application.DoEvents();

                        if (scriptManager.queue.Count > 0)
                        {
                            //System.Diagnostics.Debug.WriteLine("Server Script Entering Action");
                            var action = scriptManager.queue.Take();
                            action();
                            //System.Diagnostics.Debug.WriteLine("Server Script Exiting Action");
                        }

                        //System.Diagnostics.Debug.WriteLine("Server Script NOT Inside Doevents");
                        Thread.Sleep(1);
                    };

                    basicCtl.Run = true;

                    try
                    {
                        scriptManager.manager.OnInit(EventArgs.Empty);
                    }
                    catch (TerminatedException)
                    {
                        // script execution has been terminated
                    }
                    catch (Exception ex)
                    {
                        basicCtl.ReportError(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                scriptManager.syslog.Error(ex.Message);
            }
        }

        protected static void TerminateBasicControl(ScriptManager scriptManager, BasicNoUIObj basicCtl)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Server Script Entering Terminate");

                try
                {
                    scriptManager.manager.OnTerminate(EventArgs.Empty);
                }
                catch (TerminatedException)
                {
                    // script execution has been terminated
                }
                catch (Exception ex)
                {
                    basicCtl.ReportError(ex);
                }
                System.Diagnostics.Debug.WriteLine("Server Script Exiting Terminate");

                basicCtl.Run = false;

                if (scriptManager.module != null)
                {
                    scriptManager.module.Dispose();
                    scriptManager.module = null;
                }

                basicCtl.Shutdown();
                basicCtl.Disconnect();
                basicCtl.Dispose();
                basicCtl = null;
            }
            catch (Exception ex)
            {
                scriptManager.syslog.Error(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            finally
            {
                lock (mapbasicCtl)
                    mapbasicCtl.Remove(scriptManager);
                scriptManager.terminated.Set();
            }
        }

        static internal void StartEngine()
        {
            if (thread == null)
                return;
            thread.Start();
        }

        internal virtual void Start()
        { }

        internal static Dictionary<String, Dictionary<String, NodeId>> GetSubList(List<String> prototypes)
        {
            var map = new Dictionary<String, Dictionary<String, NodeId>>();

            prototypes.ForEach(prototype =>
                {
                    var prototypeSplit = prototype.Split(' ');
                    String subName = prototypeSplit[0];
                    map.Add(subName, new Dictionary<String, NodeId>());

                    if (prototypeSplit.Length > 1)
                    {
                        var parameters = prototype.Replace(String.Format("{0} ", subName), "").Split(new String[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                        Array.ForEach(parameters, parameter =>
                            {
                                var types = parameter.Split(new String[] { " As " }, StringSplitOptions.RemoveEmptyEntries);
                                if (types.Length > 1)
                                    map[subName].Add(types[0], GetTypeFromBasicType(types[1]));
                            });
                    }
                });

            return map;
        }

        internal List<String> GetRemoteDebuggingData()
        {
            lastTimeDebugging = DateTime.UtcNow;

            var ret = new List<String>();
            lock (listDebuggingData)
            {
                ret.AddRange(listDebuggingData);
                listDebuggingData.Clear();
            }
            return ret;
        }

        internal void EnableRemoteDebugging(bool bEnable)
        {
            BasicNoUIObj basicCtl = this.basicCtl;
            if (basicCtl == null)
            {
                lock (lockObject)
                {
                    if (mapbasicCtl.ContainsKey(this))
                        basicCtl = mapbasicCtl[this];
                }
            }

            bool bEnableRemoteDebugging = !(basicCtl is BasicIdeObj);
            if (!bEnableRemoteDebugging)
                return;

            lock (listDebuggingData)
                listDebuggingData.Clear();

            queue.Add(() =>
            {
                if (bEnable)
                    lastTimeDebugging = DateTime.UtcNow;

                basicCtl.Synchronized = bEnable;
            });
        }

        internal void DebugSynchronize(List<String> data)
        {
            BasicNoUIObj basicCtl = this.basicCtl;
            if (basicCtl == null)
            {
                lock (lockObject)
                {
                    if (mapbasicCtl.ContainsKey(this))
                        basicCtl = mapbasicCtl[this];
                }
            }

            bool bEnableRemoteDebugging = !(basicCtl is BasicIdeObj);
            if (!bEnableRemoteDebugging)
                return;

            if (data != null && data.Count > 0)
            {
                queue.Add(() =>
                {
                    data.ForEach(s => basicCtl.Synchronize(s, 0));
                });
            }
        }

        ManualResetEvent manualEvent;
        internal bool MethodCall(String subname, params object[] args)
        {
            if (module == null)
                return false;

            bool bRet = true;

            lock (queue)
            {
                if (manualEvent == null)
                    manualEvent = new ManualResetEvent(false);
                else
                    manualEvent.Reset();
            }

            queue.Add(() =>
                        {
                            try
                            {
                                module.Call(subname, args);
                            }
                            catch (TerminatedException)
                            {
                                // script execution has been terminated
                            }
                            catch (Exception ex)
                            {
                                module.ReportError(ex);
                                bRet = false;
                            }

                            manualEvent.Set();
                        });

            manualEvent.WaitOne();

            return bRet;
        }

        readonly static Dictionary<String, NodeId> mapBasicTypes = new Dictionary<String, NodeId>() 
        { 
            { "Boolean", DataTypeIds.Boolean }, 
            { "Byte", DataTypeIds.Byte }, 
            { "Char", DataTypeIds.SByte }, 
            { "Integer", DataTypeIds.Int32 }, 
            { "Long", DataTypeIds.UInt32 }, 
            { "SByte", DataTypeIds.SByte }, 
            { "Short", DataTypeIds.Int16 }, 
            { "Single", DataTypeIds.Float }, 
            { "String", DataTypeIds.String }, 
            { "Double", DataTypeIds.Double } 
        };

        static NodeId GetTypeFromBasicType(String type)
        {
            if (mapBasicTypes.ContainsKey(type))
                return mapBasicTypes[type];
            return null;
        }

        protected bool bQuitRequest;
        internal void RequestTerminate()
        {
            bQuitRequest = true;
        }

        internal void Terminate()
        {
            bQuitRequest = true;
            terminated.WaitOne();
        }

        public void Dispose()
        {
            if (bDisposing)
                return;

            bDisposing = true;

            terminated.Dispose();
            terminated = null;

            if (manualEvent != null)
            {
                manualEvent.Dispose();
                manualEvent = null;
            }

            queue.Dispose();

            if (module != null)
            {
                module.Dispose();
                module = null;
            }
        }

        #region Events
        public event EventHandler InUseTag;
        #region OnInUseTag
        /// <summary>
        /// Triggers the DoEvents event.
        /// </summary>
        public virtual void OnInUseTag(NodeState node)
        {
            var e = InUseTag;
            if (e != null)
                e(node, EventArgs.Empty);
        }
        #endregion
        #endregion
    }
}
