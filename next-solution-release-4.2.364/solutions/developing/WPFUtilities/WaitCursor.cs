using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
#if !WINDOWS_UWP && !NET_STANDARD
using System.Windows.Threading;
using Microsoft.WindowsAPICodePack.Taskbar;
#elif WINDOWS_UWP
using Windows.UI.Xaml;
#endif

namespace Utilities
{
    public class WaitCursor : IDisposable
    {
#if !WINDOWS_UWP && !NET_STANDARD
        Cursor m_oldCursor;
        FrameworkElement m_fe;
        IInputElement lastFocusElement;

        static long nCounterProgress;
        static bool bUnsupportedWaitCursor;
        static Cursor m_OriginalCursor;
        // Keep a reference to the Taskbar instance
        private static TaskbarManager windowsTaskbar = TaskbarManager.Instance;
#endif

        public WaitCursor()
        {
            Aquire();
        }

#if !NET_STANDARD
        public WaitCursor(FrameworkElement fe)
        {
            Aquire(fe);
        }
#endif

        public virtual void Aquire(
#if !NET_STANDARD
            FrameworkElement fe = null
#endif
            )
        {
#if !WINDOWS_UWP && !NET_STANDARD
            if (bUnsupportedWaitCursor || Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
                return;

            bool bCanUseDispatcher = true;
            if (Application.Current == null || !Application.Current.Dispatcher.CheckAccess())
                bCanUseDispatcher = false;
                // return;

            var dipatcher = Dispatcher.FromThread(Thread.CurrentThread);
            if (dipatcher == null || !dipatcher.CheckAccess())
                bCanUseDispatcher = false;
                // return;

            if (m_oldCursor == null)
                m_oldCursor = Mouse.OverrideCursor;
            Mouse.OverrideCursor = Cursors.Wait;

            var nCounter = Interlocked.Increment(ref nCounterProgress);
            if (nCounter == 1)
                m_OriginalCursor = m_oldCursor;

            if (bCanUseDispatcher && TaskbarManager.IsPlatformSupported && Application.Current.MainWindow != null && nCounter == 1)
                windowsTaskbar.SetProgressState(TaskbarProgressBarState.Indeterminate, Application.Current.MainWindow);

            if (fe != null && fe.IsEnabled)
            {
                DependencyObject dobject = FocusManager.GetFocusScope(fe);
                lastFocusElement = FocusManager.GetFocusedElement(dobject);
                m_fe = fe;
                m_fe.IsEnabled = false;
            }
#endif
        }

        public virtual void Release()
        {
#if !WINDOWS_UWP && !NET_STANDARD
            if (bUnsupportedWaitCursor || Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
                return;

            bool bCanUseDispatcher = true;
            if (Application.Current == null || !Application.Current.Dispatcher.CheckAccess())
                bCanUseDispatcher = false;
                // return;

            var dipatcher = Dispatcher.FromThread(Thread.CurrentThread);
            if (dipatcher == null || !dipatcher.CheckAccess())
                bCanUseDispatcher = false;
                // return;

                // we cannot use the methods and properties below cause they are strict to the UI thread
            Mouse.OverrideCursor = m_oldCursor;
            m_oldCursor = null;
            if (m_fe != null)
            {
                m_fe.IsEnabled = true;
                if (lastFocusElement != null)
                    lastFocusElement.Focus();
            }

            var nCounter = Interlocked.Decrement(ref nCounterProgress);
            if (nCounter == 0)
                m_OriginalCursor = null;

            if (bCanUseDispatcher && TaskbarManager.IsPlatformSupported && Application.Current.MainWindow != null && nCounter == 0)
                windowsTaskbar.SetProgressState(TaskbarProgressBarState.NoProgress, Application.Current.MainWindow);
#endif
        }

#if !WINDOWS_UWP && !NET_STANDARD
        public static void SuppressWaitCursor()
        {
            bUnsupportedWaitCursor = true;
        }

        internal static void ResetCursor()
        {
            if (bUnsupportedWaitCursor || Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
                return;

            if (Interlocked.Read(ref nCounterProgress) > 0)
                Mouse.OverrideCursor = m_OriginalCursor;
        }

        internal static void RestoreCursor(Cursor cursor)
        {
            if (bUnsupportedWaitCursor || Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
                return;

            if (Interlocked.Read(ref nCounterProgress) > 0)
                Mouse.OverrideCursor = cursor;
        }
#endif

        ~WaitCursor()
        {
            // Call the method that actually does the cleanup.
            Dispose(false);
        }

        // The default Dispose implementation (shown here) is exactly
        // what you want. Overriding this method is very strongly discouraged.
        protected virtual void Dispose(bool disposing)
        {
            // The default implementation ignores the disposing argument
            // If resource was already released, just return
            // If ownsHandle is false, return
            // Set flag indicating that this resource has been released
            // Call the virtual ReleaseHandle method
            // Call GC.SuppressFinalize(this) to prevent Finalize from being called
            // If ReleaseHandle returned true, return
            // Fire the ReleaseHandleFailed Managed Debugging Assistant (MDA)
            if (disposing)
            {
                GC.SuppressFinalize(this);

                Release();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }

#if !WINDOWS_UWP && !NET_STANDARD
    public class ResetCursor : IDisposable
    {
        readonly Cursor m_oldCursor;

        public ResetCursor()
        {
            m_oldCursor = Mouse.OverrideCursor;
            WaitCursor.ResetCursor();
        }

        public void Dispose()
        {
            WaitCursor.RestoreCursor(m_oldCursor);
        }
    }
#endif
}