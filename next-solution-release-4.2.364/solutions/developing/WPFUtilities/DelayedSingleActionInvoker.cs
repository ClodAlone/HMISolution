using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace WPFUtilities
{
    /// <summary>Enforces that an operation will only happen
    /// one time within a given time span.</summary>
    public class DelayedSingleActionInvoker : DispatcherObject
    {
        #region Private Properties
        private Action ActionToInvoke { get; set; }
        private DispatcherOperation LastOperation { get; set; }
        private DispatcherPriority Priority { get; set; }
        private DispatcherTimer Timer { get; set; }
        private bool bRestartTimer { get; set; }
        #endregion

        #region Constructors
        /// <summary>Initializes a new instance of DelayedSingleActionInvoker.
        /// TimeSpan is half a second.</summary>
        /// <param name="action">The action to execute.</param>
        public DelayedSingleActionInvoker(Action action)
        {
            ActionToInvoke = action;
            Priority = DispatcherPriority.Background;

            Timer = new DispatcherTimer(Priority, Dispatcher);
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            Timer.Tick += Tick;
        }

        /// <summary>Initializes a new instance of DelayedSingleActionInvoker.</summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="waitSpan">The amount of time to wait
        /// for the action to be relistened for before firing.</param>
        public DelayedSingleActionInvoker(Action action, TimeSpan waitSpan, bool restart = true)
        {
            ActionToInvoke = action;
            Priority = DispatcherPriority.Background;
            bRestartTimer = restart;

            Timer = new DispatcherTimer(Priority, Dispatcher);
            Timer.Interval = waitSpan;
            Timer.Tick += Tick;
        }

        /// <summary>Initializes a new instance of DelayedSingleActionInvoker.</summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="waitSpan">The amount of time to wait
        /// for the action to be relistened for before firing.</param>
        /// <param name="priority">The priority.</param>
        public DelayedSingleActionInvoker(Action action, TimeSpan waitSpan, DispatcherPriority priority, bool restart = true)
        {
            ActionToInvoke = action;
            Priority = priority;
            bRestartTimer = restart;

            Timer = new DispatcherTimer(Priority, Dispatcher);
            Timer.Interval = waitSpan;
            Timer.Tick += Tick;
        }
        #endregion

        #region Events
        private void Tick(object sender, EventArgs e)
        {
            LastOperation = Dispatcher.BeginInvoke((Action)delegate
            {
                Timer.Stop();

                ActionToInvoke.Invoke();

                LastOperation = null;
            }, Priority);
        }
        #endregion

        #region Public Methods
        /// <summary>Begins invoking the action.</summary>
        public void BeginInvoke()
        {
            if (Timer.IsEnabled)
            {
                if (!bRestartTimer)
                    return;
                Timer.Stop();
            }

            if (LastOperation != null)
                LastOperation.Abort();

            Timer.Start();
        }

        public void Terminate()
        {
            if (Timer.IsEnabled)
                Timer.Stop();

            if (LastOperation != null)
                LastOperation.Abort();
        }
        #endregion
    }
}
