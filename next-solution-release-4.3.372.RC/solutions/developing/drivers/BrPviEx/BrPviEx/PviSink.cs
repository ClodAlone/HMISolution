using System;
using System.Windows;
using System.Threading;
using System.Windows.Threading;

namespace BrPvi
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

    public class PviSink : IDisposable
    {
        #region Constructors
        public PviSink(BrPviChannel channel)
        {
            callingChannel = channel;            
        }
        #endregion

        #region Data members
        BrPviChannel callingChannel;
        readonly Object lockObject = new Object();
        Window dispatcherWindow;
        Thread dispatcherThread;
        bool bDisposed = false;
        bool bThreadTerminated = false;        
        #endregion

        #region PublicMethods
        public bool PviXInitialize()
        {
            if (callingChannel == null)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - PviSink.PviXInitialize - callingChannel is null!",
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
                    callingChannel.PviXInitialize();
                }
            });

            return (true);
        }

        public bool PviXSetGlobEventMsg()
        {
            
            if (callingChannel == null)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - PviSink.PviXSetGlobEventMsg - callingChannel is null!",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"));
                return (false);
            }

            EnsureThread();
            if (dispatcherThread == null || bDisposed)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - PviSink.PviXSetGlobEventMsg - dispatcherThread is not running!",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"));
                return (false);
            }

            callingChannel.ResetConnectionStatus();
            dispatcherWindow.Dispatcher.InvokeIfRequired(() =>
            {
                lock (lockObject)
                {
                    callingChannel.SetPviGlobalEvents();
                }
            });
            callingChannel.WaitConnectionResult();

            return (true);
        }

        public bool PviXDeinitialize()
        {
            if (callingChannel == null)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - PviSink.PviXDeinitialize - callingChannel is null!",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"));
                return (false);
            }

            EnsureThread();
            if (dispatcherThread == null || bDisposed)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - PviSink.PviXDeinitialize - dispatcherThread is not running!",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"));
                return (false);
            }
            
            dispatcherWindow.Dispatcher.InvokeIfRequired(() =>
            {
                lock (lockObject)
                {
                    callingChannel.PviXDeinitialize();
                }
            });
            
            return (true);
        }

        public bool CreatePviObjects()
        {
            if (callingChannel == null)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - PviSink.CreatePviObjects - callingChannel is null!",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"));
                return (false);
            }

            EnsureThread();
            if (dispatcherThread == null || bDisposed)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - PviSink.CreatePviObjects - dispatcherThread is not running!",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"));
                return (false);
            }

            dispatcherWindow.Dispatcher.InvokeIfRequired(() =>
            {
                lock (lockObject)
                {
                    callingChannel.CreatePviObjects();
                }
            });

            return (true);
        }

        public bool WritePviObjects()
        {
            if (callingChannel == null)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - PviSink.WritePviObjects - callingChannel is null!",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"));
                return (false);
            }

            EnsureThread();
            if (dispatcherThread == null || bDisposed)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - PviSink.WritePviObjects - dispatcherThread is not running!",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"));
                return (false);
            }

            dispatcherWindow.Dispatcher.InvokeIfRequired(() =>
            {
                lock (lockObject)
                {
                    callingChannel.WritePviObjects();
                }
            });

            return (true);
        }

        public bool WriteMaskPviObjects()
        {
            if (callingChannel == null)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - PviSink.WriteMaskPviObjects - callingChannel is null!",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"));
                return (false);
            }

            EnsureThread();
            if (dispatcherThread == null || bDisposed)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - PviSink.WriteMaskPviObjects - dispatcherThread is not running!",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"));
                return (false);
            }

            dispatcherWindow.Dispatcher.InvokeIfRequired(() =>
            {
                lock (lockObject)
                {
                    callingChannel.WriteMaskPviObjects();
                }
            });

            return (true);
        }

        public bool SendStringLengthRequests()
        {
            if (callingChannel == null)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - PviSink.SendStringLengthRequests - callingChannel is null!",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"));
                return (false);
            }

            EnsureThread();
            if (dispatcherThread == null || bDisposed)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - PviSink.SendStringLengthRequests - dispatcherThread is not running!",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"));
                return (false);
            }

            dispatcherWindow.Dispatcher.InvokeIfRequired(() =>
            {
                lock (lockObject)
                {
                    callingChannel.SendStringLengthRequests();
                }
            });

            return (true);
        }

        public bool SendDeleteRequests()
        {
            if (callingChannel == null)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - PviSink.SendStringLengthRequests - callingChannel is null!",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"));
                return (false);
            }

            EnsureThread();
            if (dispatcherThread == null || bDisposed)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - {0} - PviSink.SendStringLengthRequests - dispatcherThread is not running!",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"));
                return (false);
            }

            dispatcherWindow.Dispatcher.InvokeIfRequired(() =>
            {
                lock (lockObject)
                {
                    callingChannel.SendDeleteRequests();
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

            lock (lockObject)
            {
                if (dispatcherThread != null)
                {
                    // stop dispatcher
                    dispatcherThread.Join();
                    dispatcherThread = null;
                }
            }
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
                        if(callingChannel != null)
                        {
                            dispatcherThread.Name = "DispatcherThread_" + callingChannel.Name;
                        }
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
