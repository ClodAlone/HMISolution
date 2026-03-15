using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Utilities
{
    public static class TIKeyboardHelper
    {
        #region Declaration

        //[DllImport("kernel32.dll", SetLastError = true)]
        //private static extern bool Wow64DisableWow64FsRedirection(ref IntPtr ptr);
        //[DllImport("kernel32.dll", SetLastError = true)]
        //public static extern bool Wow64RevertWow64FsRedirection(IntPtr ptr);

        private static Process _p;
        private const string OnScreenKeyboardExe = "TabTip.exe";

        #endregion

        #region Private Methods

        static void StartOsk()
        {
            if (_p == null)
            {
                //IntPtr ptr = new IntPtr(); ;
                //bool sucessfullyDisabledWow64Redirect = false;

                // Disable x64 directory virtualization if we're on x64,
                // otherwise keyboard launch will fail.
                //if (System.Environment.Is64BitOperatingSystem)
                //{
                //    sucessfullyDisabledWow64Redirect =
                //        Wow64DisableWow64FsRedirection(ref ptr);
                //}

                _p = new Process();
                _p.Exited += OnKillProcess;
                _p.StartInfo.FileName = OnScreenKeyboardExe;
                //_p.StartInfo.Verb = "runas";
                // We must use ShellExecute to start osk from the current thread
                // with psi.UseShellExecute = false the CreateProcessWithLogon API 
                // would be used which handles process creation on a separate thread 
                // where the above call to Wow64DisableWow64FsRedirection would not 
                // have any effect.
                //
                _p.StartInfo.UseShellExecute = true;

                try
                {
                    // try to start process (catch exception if doesn't exist)
                    _p.Start();
                }
                catch (Exception ex)
                {
                    Dispose();
                }
                //finally
                //{
                //    // Re-enable directory virtualisation if it was disabled.
                //    if (System.Environment.Is64BitOperatingSystem)
                //        if (sucessfullyDisabledWow64Redirect)
                //            Wow64RevertWow64FsRedirection(ptr);
                //}
            }
        }

        static void OnKillProcess(object sender, EventArgs e)
        {
            Dispose();
        }

        static void Dispose()
        {
            if (_p != null)
            {
                _p.Exited -= OnKillProcess;
                _p.Dispose();
                _p = null;
            }
        }

        #endregion

        #region Public Methods

        public static void ToggleShow()
        {
            if (_p == null)
                Show();
            else
                Hide();
        }

        public static void Show()
        {
            // we must start osk from an MTA thread
            //if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
            //{
            //    ThreadStart start = new ThreadStart(StartOsk);
            //    Thread thread = new Thread(start);
            //    thread.SetApartmentState(ApartmentState.MTA);
            //    thread.Start();
            //    thread.Join();
            //}
            //else
            {
                StartOsk();
            }
        }

        public static void Hide()
        {
            if (_p != null)
            {
                try
                {
                    // try to kill process (catch exception if still running)
                    _p.Kill();
                }
                catch (Exception ex)
                { }

                Dispose();
            }
        }

        #endregion
    }
}
