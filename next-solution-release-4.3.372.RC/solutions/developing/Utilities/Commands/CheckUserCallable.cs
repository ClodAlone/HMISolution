using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Utilities.Commands
{
    public class CheckUserCallable
    {
        #region Members
        static Timer timer;
        static List<ICheckUserCallable> listAddedCommands = new List<ICheckUserCallable>();
        static List<ICheckUserCallable> listPendingCommands = new List<ICheckUserCallable>();
        static int currentPendingCommand;

        static bool breakCheckingLoop = false;
        #endregion

        #region Singleton
        static Object singletonLocker = new Object();
        static CheckUserCallable singletonInstance;
        static public CheckUserCallable GetInstance()
        {
            lock (singletonLocker)
            {
                if (singletonInstance != null)
                    return singletonInstance;
                singletonInstance = new CheckUserCallable();
                return singletonInstance;
            }
        }
        #endregion

        #region Methods
        public void Add(ICheckUserCallable item)
        {
            lock (singletonLocker)
            {
                if (listPendingCommands.Count == 0)
                    currentPendingCommand = 0;
                if (Properties.Settings.Default.MethodCallableFastChecking && !listAddedCommands.Contains(item))
                    listAddedCommands.Add(item);
                if (!listPendingCommands.Contains(item))
                    listPendingCommands.Add(item);
                if (timer == null)
                    timer = new Timer(timerCallback, item, 0, System.Threading.Timeout.Infinite);
            }
        }

        public void Remove(ICheckUserCallable item)
        {
            lock (singletonLocker)
            {
                if (listAddedCommands.Contains(item))
                    listAddedCommands.Remove(item);
                if (listPendingCommands.Contains(item))
                    listPendingCommands.Remove(item);
                if (currentPendingCommand >= listPendingCommands.Count)
                    currentPendingCommand = 0;
                if (listPendingCommands.Count == 0 && timer != null)
                {
                    timer.Dispose();
                    timer = null;
                }
                breakCheckingLoop = true;
            }
        }

        void timerCallback(Object state)
        {
            lock (singletonLocker)
            {
                if (timer != null)
                {
                    timer.Dispose();
                    timer = null;
                }
            }

            var pendingItems = new List<ICheckUserCallable>();
#if !WINDOWS_UWP
            var oldPriority = Thread.CurrentThread.Priority;
#endif
            try
            {
#if !WINDOWS_UWP
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;
#endif
                lock (singletonLocker)
                {
                    breakCheckingLoop = false;
                    pendingItems.AddRange(listAddedCommands);
                    listAddedCommands.Clear();

                    if (currentPendingCommand < listPendingCommands.Count)
                    {
                        if (!pendingItems.Contains(listPendingCommands[currentPendingCommand]))
                            pendingItems.Add(listPendingCommands[currentPendingCommand]);
                        currentPendingCommand++;
                    }
                    else
                        currentPendingCommand = 0;
                }

                foreach (var item in pendingItems)
                {
                    if (breakCheckingLoop)
                        break;

                    try
                    {
                        if (item != null)
                            item.CheckUserCallable();
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
            }
#if !WINDOWS_UWP
            Thread.CurrentThread.Priority = oldPriority;
#endif
            lock (singletonLocker)
            {
                if (timer == null && listAddedCommands.Count > 0)
                    timer = new Timer(timerCallback, this, 0, System.Threading.Timeout.Infinite);
                else if (timer == null && listPendingCommands.Count > 0)
                    timer = new Timer(timerCallback, this, 500, System.Threading.Timeout.Infinite);
            }
        }
        #endregion

    }
}
