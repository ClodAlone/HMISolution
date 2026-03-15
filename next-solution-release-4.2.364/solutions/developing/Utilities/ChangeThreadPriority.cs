using System;
using System.Threading;

namespace Utilities
{
    public class ChangeThreadPriority : IDisposable
    {
        #region Declarations
        readonly ThreadPriority oldPriority;
        #endregion

        #region Constructors
        public ChangeThreadPriority(ThreadPriority priority)
        {
            oldPriority = Thread.CurrentThread.Priority;
            try
            {
                Thread.CurrentThread.Priority = priority;
            }
            catch
            { }
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Frees any unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// An overrideable version of the Dispose.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    Thread.CurrentThread.Priority = oldPriority;
                }
                catch
                { }
            }
        }
        #endregion
    }
}
