using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Input;
using System.Threading;

namespace Utilities
{
    public static class WaitForPriority
    {
        #region DoEvents using a DispatcherFrame

        public static void DoEvents(this Application application)
        {
            DoEvents(application.Dispatcher);
        }

        public static void DoEvents(this Dispatcher dispatcher)
        {
            DispatcherFrame frame = new DispatcherFrame();
            dispatcher.BeginInvoke(DispatcherPriority.Background, new ExitFrameHandler(frm => frm.Continue = false), frame);
            Dispatcher.PushFrame(frame);
        }

        private delegate void ExitFrameHandler(DispatcherFrame frame);

        #endregion

        /// <summary>
        /// Process all messages in the current dispatcher queue
        /// </summary>
        public static void DoEventsSync()
        {
            // Add an empty delegate to the
            // current thread's Dispatcher, and
            // invoke it synchronously but using a
            // a Background priority.
            // It won't return until all higher-priority
            // events in the queue are processed.
            Dispatcher.CurrentDispatcher.Invoke(
            DispatcherPriority.Background,
            new EmptyDelegate(
            delegate { }));
        }
        private delegate void EmptyDelegate();

        public static void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame(true);
            Dispatcher.CurrentDispatcher.BeginInvoke
            (
            DispatcherPriority.Background,
            (SendOrPostCallback)delegate(object arg)
            {
                var f = arg as DispatcherFrame;
                f.Continue = false;
            },
            frame
            );
            Dispatcher.PushFrame(frame);
        }

        public static void DoEventsIdle()
        {
            DispatcherFrame frame = new DispatcherFrame(true);
            Dispatcher.CurrentDispatcher.BeginInvoke
            (
            DispatcherPriority.ApplicationIdle,
            (SendOrPostCallback)delegate(object arg)
            {
                var f = arg as DispatcherFrame;
                f.Continue = false;
            },
            frame
            );
            Dispatcher.PushFrame(frame);
        }

        public static void Wait(DispatcherPriority priority, FrameworkElement fe)
        {
            IInputElement lastFocusElement = null;
            bool bIsEnable = true;
            if (fe != null && fe.IsEnabled)
            {
                bIsEnable = fe.IsEnabled;
                DependencyObject dobject = FocusManager.GetFocusScope(fe);
                lastFocusElement = FocusManager.GetFocusedElement(dobject);
                fe.IsEnabled = false;
            }
            else
                fe = null;

            try
            {
                DispatcherFrame frame = new DispatcherFrame();
                DispatcherOperation dispatcherOperation = Dispatcher.CurrentDispatcher.BeginInvoke(priority, new DispatcherOperationCallback(ExitFrameOperation), frame);
                Dispatcher.PushFrame(frame);
                if (dispatcherOperation.Status != DispatcherOperationStatus.Completed)
                {
                    dispatcherOperation.Abort();
                }
            }
            finally
            {
                if (fe != null)
                {
                    fe.IsEnabled = bIsEnable;
                    if (lastFocusElement != null)
                        lastFocusElement.Focus();
                }
            }
        }

        private static object ExitFrameOperation(object obj)
        {
            ((DispatcherFrame)obj).Continue = false;
            return null;
        }
    }
}
