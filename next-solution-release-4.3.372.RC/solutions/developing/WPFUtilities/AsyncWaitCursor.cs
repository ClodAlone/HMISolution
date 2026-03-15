using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace Utilities
{
    public class AsyncWaitCursor : WaitCursor
    {
        #region Declarations
        //Dispatcher dispatcher;
        DispatcherPriority dispatcherPriority;

        static int counter;
        #endregion

        #region Constructors
        public AsyncWaitCursor(DispatcherPriority priority = DispatcherPriority.Background) : 
            base()
        {
            dispatcherPriority = priority;
            Interlocked.Increment(ref counter);
        }

        public AsyncWaitCursor(FrameworkElement fe, DispatcherPriority priority = DispatcherPriority.Background) : 
            base(fe)
        {
            dispatcherPriority = priority;
            Interlocked.Increment(ref counter);
        }
        #endregion

        #region Overrides
        public override void Release()
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(() => 
            {
                base.Release();
                var result = Interlocked.Decrement(ref counter);
                if (result == 0)
                    Mouse.OverrideCursor = null;
            }, dispatcherPriority);
        }
        #endregion
    }
}