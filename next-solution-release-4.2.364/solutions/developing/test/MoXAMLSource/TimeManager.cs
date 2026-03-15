using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows;

namespace test
{
    /// <summary>
    /// This class is responsible for updating the time so that it displays propertly in the status bar.
    /// </summary>
    public class TimeManager : INotifyPropertyChanged, IDisposable
    {
        private DispatcherTimer _timer = null;
        private string _currentDateAndTime;

        /// <summary>
        /// Initialize a new instance of <see cref="TimeManager"/>.
        /// </summary>
        /// <param name="parent">The window that "owns" the status bar. This class uses the Dispatcher from that
        /// window.</param>
        public TimeManager(Window parent)
        {
            _timer = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Normal, new EventHandler(TimerTick), parent.Dispatcher);
        }

        /// <summary>
        /// Called when the timer updates.
        /// </summary>
        protected virtual void TimerTick(object sender, EventArgs e)
        {
            CurrentDateAndTime = DateTime.Now.ToString();
        }

        /// <summary>
        /// Called when the item updates.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        protected virtual void Changed(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Get or set the current date and time.
        /// </summary>
        public string CurrentDateAndTime
        {
            get
            {
                return _currentDateAndTime;
            }
            set
            {
                if (_currentDateAndTime != value)
                {
                    _currentDateAndTime = value;
                    Changed("CurrentDateAndTime");
                }
            }
        }
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        /// <summary>
        /// Start the timer.
        /// </summary>
        internal void Start()
        {
            if (_timer != null)
                _timer.Start();
        }

        /// <summary>
        /// Stop the timer.
        /// </summary>
        private void Stop()
        {
            if (_timer != null)
                _timer.Stop();
        }


        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
        }

        private bool _disposed = false;
        protected virtual void Dispose(bool dispose)
        {
            if (!_disposed && dispose)
            {
                Stop();
                _disposed = true;
                GC.SuppressFinalize(this);
            }
        }
        #endregion
    }
}
