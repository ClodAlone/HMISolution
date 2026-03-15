using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinWrap.Basic;
using System.Threading;
using DocumentManager.ComponentService;
using ScriptManager.Document;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Reflection;
using System.Windows.Input;
using ScriptManager.ComponentService;
using OPCUAViewModel;
using Utilities;
using System.Windows.Threading;
using Opc.Ua;
using ScriptVariableValues;
using log4net;
using System.Diagnostics;

namespace ScriptManager
{
    class ScriptExecuter : IDisposable
    {
        #region Declarations
        readonly Object lockObject = new Object();
        readonly static Object lockStaticObject = new Object();

        readonly ScriptDocument scriptDocument;
        BasicNoUIObj basicCtl;
        bool bDontRaiseSecondError;
        bool bReportExecption;
        bool bStopRequest;
        bool bToBeDisposed;
        bool bQuitThread;
        DateTime stopRequestTime;
        bool bWindowCreated;
        bool bDoEvents;
        bool bExecutingSynchro;
        bool bCanUseBreakPoints;

        ExecutionMode lastmode = ExecutionMode.Stop;
        Handler handler;
        readonly AutoResetEvent startEvent = new AutoResetEvent(false);
        Thread thread;
        bool bExecutingShared;
        readonly ScriptManagerComponent ScriptComponent;

        private static readonly ILog logLicense = LogManager.GetLogger(Properties.Resources.LicenseManager);

        ILog syslog;
        #endregion

        #region properties
        public ScriptDocument Document
        {
            get
            {
                return scriptDocument;
            }
        }
        #endregion

        public ScriptExecuter(ScriptDocument doc, ScriptManagerComponent c)
        {
            scriptDocument = doc;
            ScriptComponent = c;

            if (Environment.UserInteractive)
            {
                bCanUseBreakPoints = !ApplicationPropertiesHelper.GetProperty<bool>("IsOnlyRuntime", false);
            }

            scriptDocument.GetTagList += (o, ev) =>
            {
                ev.list = ScriptComponent.UFUAEditor.GetFlatListTags(scriptDocument);
            };
            scriptDocument.GetPrototypeList += (o, ev) =>
            {
                ev.mapDefinitions = ScriptComponent.UFUAEditor.GetFlatListPrototypes(scriptDocument);
                ev.mapPrototypes = ScriptComponent.UFUAEditor.GetFlatListPrototypeInstances(scriptDocument);
            };
            scriptDocument.GetTagEntityReference += (o, ev) =>
            {
                var erString = ScriptComponent.UFUAEditor.GetTagEntityReference(scriptDocument, ev.Name, ev.Instance, inExecution: true);
                if (!String.IsNullOrEmpty(erString))
                    ev.entityReference = erString.FromXml<OPCUAEntityReference>();
            };
        }

        internal ExecutionMode LastExecutionMode
        {
            get
            {
                return lastmode;
            }
        }

        public void Stop(bool bDispose = false)
        {
            if (scriptDocument._IsInStoppingMode)
                return;

            try
            {
                lastmode = ExecutionMode.Stop;
                stopRequestTime = DateTime.UtcNow;
                scriptDocument._IsInStoppingMode = true;

                lock (lockObject)
                {
                    if (basicCtl != null && !bStopRequest)
                    {
                        bToBeDisposed = bDispose;
                        bDontRaiseSecondError = true;
                        bStopRequest = true;
                        startEvent.Set();
                    }
                }

                while (scriptDocument.InExecution/* || thread != null*/)
                {
                    if (stopRequestTime.AddMilliseconds(scriptDocument.StopCommandTimeout) < DateTime.UtcNow)
                    {
                        if (log != null)
                        {
                            try
                            {
                                log.WriteLine(Properties.Resources.CannotTerminateScriptWithTimeout);
                                log.Flush();
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                        break;
                    }
                    try
                    {
                        WaitForPriority.DoEventsIdle();
                    }
                    catch (Exception ex)
                    {
                        // dispatcher may not be available anymore
                    }
                    if (scriptDocument.SleepTime > 0)
                        Thread.Sleep(scriptDocument.SleepTime);
                }
            }
            finally
            {
                scriptDocument._IsInStoppingMode = false;
            }
        }

        public void Start(IDocument parent, ExecutionMode mode, Object Context)
        {
            bool bExecuteSynchroOutsideLock = false;
            var context = Context;

            lock (lockObject)
            {
                if (bStopRequest || bToBeDisposed)
                    return;

                if (context is StartupContext)
                {
                    var sc = context as StartupContext;
                    startupContext.Parameter = sc.Parameter;
                    startupContext.Context = sc.Context;
                    context = null;
                }

                if (lastmode == ExecutionMode.Stop)
                    lastmode = mode;
                else if (lastmode != mode)
                    Stop();

                if (mode == ExecutionMode.Synchro && bCanUseBreakPoints && 
                    scriptDocument.Breakpoints != null && scriptDocument.Breakpoints.Length > 0)
                    mode = ExecutionMode.Normal;
                switch (mode)
                {
                    case ExecutionMode.Synchro:
                        {
                            if (!bExecutingSynchro && !bExecutingShared && !scriptDocument.InExecution)
                            {
                                bExecuteSynchroOutsideLock = true;
                                bExecutingSynchro = true;
                            }
                        }
                        break;

                    case ExecutionMode.Shared:
                        {
                            if (!bExecutingShared && !bExecutingSynchro && !scriptDocument.InExecution)
                            {
                                scriptDocument.InExecution = true;
                                bExecutingShared = true;
                                var waitEvent = new ManualResetEvent(false);
                                ThreadPool.QueueUserWorkItem(o =>
                                    {
                                        var oldPriority = Thread.CurrentThread.Priority;
                                        Thread.CurrentThread.Priority = Document.ThreadPriority;

                                        try
                                        {
                                            lock (lockStaticObject)
                                            {
                                                InitScriptCode(parent, mode, context);
                                            }
                                            waitEvent.Set();
                                            bDoEvents = true;
                                            ExecuteScriptCode();
                                            TerminateScriptCode();
                                        }
                                        finally
                                        {
                                            bExecutingShared = false;
                                            Thread.CurrentThread.Priority = oldPriority;
                                        }

                                        if (bToBeDisposed)
                                            Dispose();
                                    });
                                waitEvent.WaitOne();
                                waitEvent.Dispose();
                            }
                            break;
                        }

                    case ExecutionMode.Normal:

                        if (thread == null)
                        {
                            var waitEvent = new ManualResetEvent(false);
                            thread = new Thread(o =>
                            {
                                Thread.CurrentThread.Priority = Document.ThreadPriority;

                                waitEvent.Set();
                                lock (lockStaticObject)
                                {
                                    InitScriptCode(parent, mode, context);
                                }

                                bDoEvents = true;
                                while (!bQuitThread && !bToBeDisposed)
                                {
                                    if (bWindowCreated)
                                    {
                                        try
                                        {
                                            WaitForPriority.DoEvents();
                                        }
                                        catch (Exception ex)
                                        {
                                            // dispatcher may not be available anymore
                                        }
                                        if (startEvent.WaitOne(50))
                                        {
                                            if (!bStopRequest && !bToBeDisposed)
                                            {
                                                var ctrl = basicCtl as BasicIdeObj;
                                                if (ctrl != null)
                                                    ctrl.ActivateWindow();
                                                ExecuteScriptCode();
                                            }
                                            else if (!bExecutingShared && !bExecutingSynchro)
                                            {
                                                TerminateScriptCode();
                                                bWindowCreated = false;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (!bToBeDisposed)
                                            startEvent.WaitOne();

                                        if (!bStopRequest && !bToBeDisposed)
                                        {
                                            if (basicCtl == null)
                                            {
                                                lock (lockStaticObject)
                                                {
                                                    InitScriptCode(parent, mode, context);
                                                }
                                            }

                                            ExecuteScriptCode();
                                        }
                                        else if (!bExecutingShared && !bExecutingSynchro)
                                        {
                                            TerminateScriptCode();
                                        }
                                    }
                                }

                                lock (lockObject)
                                {
                                    TerminateScriptCode();
                                    thread = null;

                                    if (bToBeDisposed)
                                        Dispose();
                                }
                            })
                            {
                                IsBackground = true
                            };
                            thread.SetApartmentState(ApartmentState.STA);
                            thread.Start();
                            waitEvent.WaitOne();
                            waitEvent.Dispose();
                        }
                        if (!bExecutingShared && !bExecutingSynchro)
                        {
                            scriptDocument.InExecution = true;
                            startEvent.Set();
                        }
                        break;
                }
            }

            if (bExecuteSynchroOutsideLock)
            {
                bExecutingSynchro = true;
                if (basicCtl == null)
                    InitScriptCode(parent, mode, context);
                bDoEvents = false;
                try
                {
                    ExecuteScriptCode();
                    TerminateScriptCode();
                }
                finally
                {
                    bExecutingSynchro = false;
                }
            }
        }

        void ExecuteScriptCode()
        {
            var b = basicCtl;
            if (bDontRaiseSecondError || b == null)
                return;

            var watcher = Stopwatch.StartNew();
            scriptDocument.ChangeStatus(ScriptStatus.Running, ScriptStatus.None);

            System.Diagnostics.Debug.WriteLine(String.Format("Executing Script {0}", scriptDocument.FilePath));

            b.Run = true;
            scriptDocument.InExecution = true;
            scriptDocument.UpdateSessionSettings();

            if (handler != null && handler.Exists)
            {
                int maxExecutionRestarts = scriptDocument.MaxExecutionRestarts;
                bool bErrorCaught = false;
                do
                {
                    bErrorCaught = false;
                    try
                    {
                        handler.Call();
                    }
                    catch (TerminatedException)
                    {
                        // script execution has been terminated
                    }
                    catch (Exception e)
                    {
                        bReportExecption = true;
                        try
                        {
                            handler.ReportError(e);
                        }
                        catch (Exception ex)
                        {

                        }

                        bErrorCaught = true;
                        if (maxExecutionRestarts <= 0)
                            break;
                    }
                } while(bErrorCaught && maxExecutionRestarts-- > 0);
            }

            scriptDocument.ChangeStatus(ScriptStatus.None, ScriptStatus.Running);
            watcher.Stop();
            scriptDocument.ChangeCycleTime(watcher.Elapsed);
        }

        void TerminateScriptCode()
        {
            scriptDocument.ChangeStatus(ScriptStatus.Stopping, ScriptStatus.None);
            System.Diagnostics.Debug.WriteLine(String.Format("Stopping Script {0}", scriptDocument.FilePath));

            scriptDocument.TerminateVariableValues();
            if (basicCtl != null)
            {
                //if (bWindowCreated)
                //{
                //    basicCtl.CloseWindow += (o, e) =>
                //        {
                //            bWindowCreated = false;
                //        };
                //    while (bWindowCreated)
                //        WaitForPriority.DoEvents();
                //}

                basicCtl.Run = false;
                while (basicCtl.Shutdown() < 0)
                {
                    try
                    {
                        WaitForPriority.DoEvents();
                    }
                    catch (Exception ex)
                    {
                        // dispatcher may not be available anymore
                    }
                    //return;
                }

                basicCtl.Disconnect();

                if (handler != null)
                {
                    handler.Dispose();
                    handler = null;
                }

                if (basicCtl != null)
                {
                    basicCtl.Dispose();
                    basicCtl = null;
                }

                basicCtl = null;

                if (log != null)
                {
                    log.Close();
                    log = null;
                }
            }

            scriptDocument.ChangeStatus(ScriptStatus.None, ScriptStatus.Stopping);
            System.Diagnostics.Debug.WriteLine(String.Format("Stopped Script {0}", scriptDocument.FilePath));

            bStopRequest = false;
            scriptDocument.InExecution = false;

            scriptDocument.UnsubscribeSystemVariables();

            startupContext.Parameter = null;
            startupContext.Context = null;
        }

        StreamWriter log;
        readonly StartupContext startupContext = new StartupContext();
        void InitScriptCode(IDocument parent, ExecutionMode mode, Object Context)
        {
            scriptDocument.ChangeStatus(ScriptStatus.Starting, ScriptStatus.None);
            System.Diagnostics.Debug.WriteLine(String.Format("Initializing Script {0}", scriptDocument.FilePath));

            scriptDocument.SubscribeSystemVariables();

            if (!String.IsNullOrEmpty(scriptDocument.Code))
            {

#if !DEBUG
                var enableVB = MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxN+rOYP4u2+aLWnhBhIXJ4A=="/* VB */);
                if (enableVB == false)
                {
                    logLicense.Warn(Properties.Resources.NoVBLicense);
                    return;
                }
#endif

                bDontRaiseSecondError = false;

                if (bCanUseBreakPoints && !bExecutingSynchro && scriptDocument.Breakpoints != null && scriptDocument.Breakpoints.Length > 0)
                    basicCtl = new BasicIdeObj();
                else
                    basicCtl = new BasicNoUIObj();

#if DEBUG
                if (System.IO.File.Exists(@"C:\Program Files (x86)\Polar Engineering\WinWrap Basic\Certificates\Application-a67e0d79.htm"))
#endif
                basicCtl.Secret = new Guid(Properties.Settings.Default.SecretKey);

                basicCtl.Initialize();
                basicCtl.EventMode = true;
                basicCtl.Caption = Path.GetFileNameWithoutExtension(scriptDocument.FullPath);
                basicCtl.TaskbarIconMode = WinWrap.Basic.TaskbarIconModeConstants.IconNoneSysmenuNone;
                //basicCtl.AttachToWindow(Application.Current.MainWindow, ManageConstants.All);

                //basicCtl.OverrideModalWindowOwner += (o, e) =>
                //{
                //    e.OwnerHandle = new WindowInteropHelper(Window.GetWindow(Entity)).Handle;
                //};
                if (scriptDocument.EnableSysLog)
                {
                    if (syslog == null)
                        syslog = LogManager.GetLogger(scriptDocument.Title);

                    basicCtl.DebugPrint += (o, e) =>
                    {
                        syslog.Debug(e.Text);
                    };
                }


                if (scriptDocument.EnableLog)
                {
                    var file = scriptDocument.FullPath + ".log";
                    if (File.Exists(file))
                    {
                        try
                        {
                            File.Delete(file);
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    try
                    {
                        log = new StreamWriter(file);
                    }
                    catch (Exception ex)
                    {
                        if (syslog != null)
                            syslog.Error(String.Format(Properties.Resources.FailedToOpenLogFile, scriptDocument.Title), ex);
                    }

                    basicCtl.DebugPrint += (o, e) =>
                        {
                            if (log != null)
                            {
                                try
                                {
                                    var text = String.Format("{0} - {1}", DateTime.Now, e.Text);
                                    log.WriteLine(text);
                                    log.Flush();
                                }
                                catch (Exception ex)
                                {
                                    
                                }
                            }
                        };
                }

                basicCtl.ErrorAlert += (o, e) =>
                {
                    if (!bDontRaiseSecondError)
                    {
                        bDontRaiseSecondError = true;
                        if (basicCtl is BasicIdeObj)
                        {
                            var ctrl = basicCtl as BasicIdeObj;
                            ctrl.CreateOverlappedWindow();
                            ctrl.WindowState = WindowState.Maximized;
                            ctrl.ActivateWindow();

                            bWindowCreated = true;
                            if (bReportExecption)
                                throw new Exception();
                            else
                                basicCtl.Run = true;
                        }
                        else
                        {
                            if (ScriptComponent.UIInterface != null)
                                ScriptComponent.UIInterface.ShowError(String.Format(Properties.Resources.ScriptError, Document.Title, basicCtl.Error.Description));
                            
                            //if (bReportExecption)
                            //    throw new Exception();
                        }

                        scriptDocument.ChangeStatus(ScriptStatus.Error, ScriptStatus.None, basicCtl.Error.Description);
                    }
                };
                basicCtl.DoEvents += (o, e) =>
                    {
                        if (Properties.Settings.Default.StatusVarIdle)
                            scriptDocument.ChangeStatus(ScriptStatus.DoEventing, ScriptStatus.None);
                        if (bStopRequest || bToBeDisposed)
                        {
                            // bStopRequest = false;
                            if (stopRequestTime.AddMilliseconds(scriptDocument.StopCommandTimeout) < DateTime.UtcNow)
                            {
                                if (syslog == null)
                                    syslog = LogManager.GetLogger(scriptDocument.Title);
                                syslog.Error(Properties.Resources.ForcingScriptTermination);
                                basicCtl.Run = false;
                            }
                        }

                        try
                        {
                            if (bDoEvents || bWindowCreated)
                                WaitForPriority.DoEvents();
                        }
                        catch (Exception ex)
                        {
                            // dispatcher may not be available anymore
                        }
                        if (!bWindowCreated && !bStopRequest && !bToBeDisposed && scriptDocument.SleepTime > 0)
                            Thread.Sleep(scriptDocument.SleepTime);

                        if (Properties.Settings.Default.StatusVarIdle)
                            scriptDocument.ChangeStatus(ScriptStatus.None, ScriptStatus.DoEventing);
                    };
                basicCtl.ReadMacro += (o, e) =>
                {
                    if (e.FileName.StartsWith("*"))
                    {
                        var filename = e.FileName.Replace("*", "");
                        var uri = Document.MakeAbosoluteUri(new Uri(Document.FullPath, UriKind.RelativeOrAbsolute));
                        var path = System.IO.Path.GetDirectoryName(uri.GetPathString());
                        var fileToRead = String.Format("{0}\\{1}{2}", path, filename, Properties.Settings.Default.DefaultFileExt);
                        var docMacro = ScriptDocument.FromFile(fileToRead, Document);
                        if (docMacro != null)
                        {
                            e.Code = docMacro.Code;
                            docMacro.Dispose();
                            e.Changed = true;
                            e.Cancel = false;
                        };
                    }
                };

                //Assembly wpfCoreAssembly = typeof(CommandBinding).Assembly;
                //basicCtl.AddExtension("#", wpfCoreAssembly);
                //Assembly wpfFrameworkAssembly = typeof(Window).Assembly;
                //basicCtl.AddExtension("#", wpfFrameworkAssembly);

                var opcua = typeof(DataValue).Assembly;
                basicCtl.AddExtension("#", opcua);

                basicCtl.AddExtension("$Feature ExtensionCache False", null);
                var listVariables = scriptDocument.GetVariableObjectDispatcher(true);
                listVariables.ForEach(variableValue =>
                    {
                        var name = variableValue.GetName();
                        if (String.IsNullOrEmpty(name))
                            basicCtl.AddExtension("%", variableValue);
                        else
                            basicCtl.AddExtension(String.Format("%{0}.", name), variableValue);
                    });

                var listReferences = new List<Object>();
                listReferences.Add(scriptDocument);
                listReferences.Add(parent);
                listReferences.Add(startupContext);

                listReferences.ForEach(reference =>
                {
                    Assembly referenceGetTypeAssembly = reference.GetType().Assembly;
                    basicCtl.AddExtension("#", referenceGetTypeAssembly);
                    basicCtl.AddExtensionObjectWithEvents(reference.GetType().Name, reference);
                });
                basicCtl.FileTools = false;
                basicCtl.Code = scriptDocument.Code;
                if (bCanUseBreakPoints && !bExecutingSynchro && scriptDocument.Breakpoints != null && scriptDocument.Breakpoints.Length > 0)
                {
                    var ctrl = basicCtl as BasicIdeObj;
                    ctrl.CreateOverlappedWindow();
                    ctrl.WindowState = WindowState.Maximized;
                    ctrl.ActivateWindow();
                    bWindowCreated = true;
                    ctrl.BreakPoints = scriptDocument.Breakpoints;
                }
                basicCtl.Changed = false;

                handler = basicCtl.CreateHandler(Properties.Settings.Default.EntryPointSub);
            }

            System.Diagnostics.Debug.WriteLine(String.Format("Initialized Script {0}", scriptDocument.FilePath));
            scriptDocument.ChangeStatus(ScriptStatus.None, ScriptStatus.Starting);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                scriptDocument.UnsubscribeSystemVariables();

                if (handler != null)
                {
                    handler.Dispose();
                    handler = null;
                }

                if (basicCtl != null)
                {
                    basicCtl.Dispose();
                    basicCtl = null;
                }

                bStopRequest = true;
                bQuitThread = true;
                startEvent.Set();
                if (thread != null)
                    thread.Join();
                startEvent.Dispose();
                
                if (log != null)
                {
                    log.Close();
                    log = null;
                }

                startupContext.Parameter = null;
                startupContext.Context = null;
            }
        }
        ~ScriptExecuter()
        {
            Dispose(false);
        }

    }
}
