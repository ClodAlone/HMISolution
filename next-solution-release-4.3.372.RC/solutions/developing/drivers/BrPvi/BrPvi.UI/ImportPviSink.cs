using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using System.Windows;

namespace BrPvi.UI
{
    public static class DispatcherExtensions
    {
        private static Action EmptyDelegate = delegate () { };
        public static void InvokeIfRequired(this Dispatcher dispatcher, Action action)
        {
            if (!dispatcher.CheckAccess())
            {
                dispatcher.Invoke(DispatcherPriority.Normal, action);
            }
            else
            {
                action();
            }
        }
    }

    public class ImportPviSink : IDisposable
    {
        #region Constructors
        public ImportPviSink(BrPviPlcImportParser importer) //ImportTagsEditorTree importer)
        {
            callingImporter = importer;
        }
        #endregion

        #region Data members
        //ImportTagsEditorTree callingImporter;
        BrPviPlcImportParser callingImporter;
        readonly Object lockObject = new Object();
        Window dispatcherWindow;
        Thread dispatcherThread;
        bool bDisposed = false;
        bool bThreadTerminated = false;
        #endregion

        #region Public Methods
        public bool PviXInitialize()
        {
            if (callingImporter == null)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - ImportPviSink.PviXInitialize - callingImporter is null!",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"));
                return (false);
            }

            EnsureThread();
            if (dispatcherThread == null || bDisposed)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - PviSink.PviXInitialize - dispatcherThread is not running!",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"));
                return (false);
            }

            dispatcherWindow.Dispatcher.InvokeIfRequired(() =>
            {
                lock (lockObject)
                {
                    callingImporter.PviXInitialize();
                }
            });

            return (true);
        }

        public bool PviXSetGlobEventMsg()
        {
            if (callingImporter == null)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - ImportPviSink.PviXSetGlobEventMsg - callingImporter is null!",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"));
                return (false);
            }

            EnsureThread();
            if (dispatcherThread == null || bDisposed)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - ImportPviSink.PviXSetGlobEventMsg - dispatcherThread is not running!",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"));
                return (false);
            }

            dispatcherWindow.Dispatcher.InvokeIfRequired(() =>
            {
                lock (lockObject)
                {
                    callingImporter.SetPviGlobalEvents();
                }
            });

            return (true);
        }

        public bool PviXDeinitialize()
        {
            if (callingImporter == null)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - ImportPviSink.PviXDeinitialize - callingImporter is null!",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"));
                return (false);
            }

            EnsureThread();
            if (dispatcherThread == null || bDisposed)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - ImportPviSink.PviXDeinitialize - dispatcherThread is not running!",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"));
                return (false);
            }

            dispatcherWindow.Dispatcher.InvokeIfRequired(() =>
            {
                lock (lockObject)
                {
                    callingImporter.PviXDeinitialize();
                }
            });

            return (true);
        }

        public bool CreateStationPviObjects()
        {
            if (callingImporter == null)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - ImportPviSink.CreateStationPviObjects - callingImporter is null!",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"));
                return (false);
            }

            EnsureThread();
            if (dispatcherThread == null || bDisposed)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - ImportPviSink.CreateStationPviObjects - dispatcherThread is not running!",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"));
                return (false);
            }

            dispatcherWindow.Dispatcher.InvokeIfRequired(() =>
            {
                lock (lockObject)
                {
                    callingImporter.CreateStationPviObjects();
                }
            });

            return (true);
        }

        public bool TaskListRequest()
        {
            if (callingImporter == null)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - ImportPviSink.TaskListRequest - callingImporter is null!",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"));
                return (false);
            }

            EnsureThread();
            if (dispatcherThread == null || bDisposed)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - ImportPviSink.TaskListRequest - dispatcherThread is not running!",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"));
                return (false);
            }

            dispatcherWindow.Dispatcher.InvokeIfRequired(() =>
            {
                lock (lockObject)
                {
                    callingImporter.TaskListRequest();
                }
            });

            return (true);
        }

        public bool CreatePviTaskObjects()
        {
            if (callingImporter == null)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - ImportPviSink.CreatePviTaskObjects - callingImporter is null!",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"));
                return (false);
            }

            EnsureThread();
            if (dispatcherThread == null || bDisposed)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - ImportPviSink.CreatePviTaskObjects - dispatcherThread is not running!",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"));
                return (false);
            }

            dispatcherWindow.Dispatcher.InvokeIfRequired(() =>
            {
                lock (lockObject)
                {
                    callingImporter.CreateTaskObjects();
                }
            });

            return (true);
        }

        public bool ReadVariableInfo()
        {
            if (callingImporter == null)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - ImportPviSink.ReadVariableInfo - callingImporter is null!",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"));
                return (false);
            }

            EnsureThread();
            if (dispatcherThread == null || bDisposed)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - ImportPviSink.ReadVariableInfo - dispatcherThread is not running!",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"));
                return (false);
            }

            dispatcherWindow.Dispatcher.InvokeIfRequired(() =>
            {
                lock (lockObject)
                {
                    callingImporter.RequestVariableInfo();
                }
            });

            return (true);
        }

        public bool CreatePviVariableObjects()
        {
            if (callingImporter == null)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - ImportPviSink.CreatePviVariableObjects - callingImporter is null!",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"));
                return (false);
            }

            EnsureThread();
            if (dispatcherThread == null || bDisposed)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - ImportPviSink.CreatePviVariableObjects - dispatcherThread is not running!",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"));
                return (false);
            }

            dispatcherWindow.Dispatcher.InvokeIfRequired(() =>
            {
                lock (lockObject)
                {
                    callingImporter.SendCreateVarObjects();
                }
            });

            return (true);
        }

        public bool VarTypeRequest()
        {
            if (callingImporter == null)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - ImportPviSink.VarTypeRequest - callingImporter is null!",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"));
                return (false);
            }

            EnsureThread();
            if (dispatcherThread == null || bDisposed)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - ImportPviSink.VarTypeRequest - dispatcherThread is not running!",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"));
                return (false);
            }

            dispatcherWindow.Dispatcher.InvokeIfRequired(() =>
            {
                lock (lockObject)
                {
                    callingImporter.VarTypeRequest();
                }
            });

            return (true);
        }

        public void Close()
        {
            EnsureThread();
            if (dispatcherThread == null || bDisposed)
            {
                return;
            }

            dispatcherWindow.Dispatcher.InvokeIfRequired(() =>
            {
                lock (lockObject)
                {
                    dispatcherWindow.Close();
                }
            });
        }
        #endregion

        #region PrivateMethods
        void dispatcher_Closed(object sender, EventArgs e)
        {
            dispatcherWindow.Dispatcher.InvokeShutdown();
        }

        void EnsureThread()
        {
            lock (lockObject)
            {

                if (bDisposed)
                {
                    return;
                }

                if (dispatcherThread != null)
                {
                    return;
                }

                try
                {
                    using (ManualResetEvent eventStarted = new ManualResetEvent(false))
                    {
                        dispatcherThread = new Thread(() =>
                        {
                            dispatcherWindow = new Window() { Visibility = Visibility.Hidden };
                            dispatcherWindow.Show();
                            dispatcherWindow.Closed += dispatcher_Closed;
                            eventStarted.Set();
                            System.Windows.Threading.Dispatcher.Run();
                            bThreadTerminated = true;
                            dispatcherWindow.Closed -= dispatcher_Closed;
                        });

                        dispatcherThread.SetApartmentState(ApartmentState.STA);
                        dispatcherThread.IsBackground = true;
                        dispatcherThread.Name = "DispatcherThread_BrPviImporter";
                        dispatcherThread.Start();
                        eventStarted.WaitOne();
                    }
                }
                catch
                {
                    throw;
                }
            }
        }
        #endregion

        #region IDisposable Interface
        public void Dispose()
        {
            lock (lockObject)
            {
                if (bDisposed)
                {
                    return;
                }
                bDisposed = true;

                if (dispatcherThread == null)
                {
                    return;
                }
            }

            dispatcherWindow.Dispatcher.InvokeIfRequired(() =>
            {
                dispatcherWindow.Close();
            });

            while (!bThreadTerminated)
            {
                Thread.Sleep(500);
            }

            dispatcherThread = null;
        }
        #endregion
    }
}
