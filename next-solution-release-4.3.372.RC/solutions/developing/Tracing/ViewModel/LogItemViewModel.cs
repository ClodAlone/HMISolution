using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tracing.Model;
using ViewModelLib;
using System.Windows.Threading;
using System.Threading;
using Utilities;
using System.IO;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace Tracing.ViewModel
{
    public class LogItemViewModel : TreeViewItemViewModel
    {
        #region Declaration
        Timer timerCleanOlderFiles;
        #endregion

        #region Constructor
        public LogItemViewModel(Dispatcher dispatcher, String logSource, TreeViewItemViewModel parent)
            : base(parent, false)
        {
            m_dispatcher = dispatcher;
            LogSource = logSource;

            CommonConstruct();
        }

        private void CommonConstruct()
        {
            MaxAgeFiles = TimeSpan.FromDays(30);
            MaxItems = 100;
            MaxFlushing = 100;
            logItems = new SafeObservableCollection<LogItem>();
        }
        #endregion

        #region IDisposable Members
        protected override void OnDispose()
        {
            base.OnDispose();

            if (timerCleanOlderFiles != null)
                timerCleanOlderFiles.Dispose();
        }
        #endregion

        #region Properties
        public String LogSource { get; private set; }
        public SafeObservableCollection<LogItem> logItems { get; private set; }

        public int MaxItems { get; set; }
        public int MaxFlushing { get; set; }
        public TimeSpan MaxAgeFiles { get; set; }

        List<LogItem> listPendingItems;
        List<LogItem> listTempItems;
        #endregion

        #region Methods
        public void AddItem(String message, DateTime timestamp, int severity, Uri uri)
        {
            AddItem(new LogItem()
            {
                Message = message,
                TimeStamp = timestamp,
                Severity = severity,
                Source = uri
            });
        }

        public void AddItem(String message, DateTime timestamp)
        {
            AddItem(new LogItem()
            {
                Message = message,
                TimeStamp = timestamp
            });
        }

        public void AddItem(String message)
        {
            AddItem(new LogItem()
            {
                Message = message, TimeStamp = DateTime.Now
            });
        }

        public void AddItem(LogItem item)
        {
            lock (lockObject)
            {
                if (listPendingItems == null)
                    listPendingItems = new List<LogItem>();
                listPendingItems.Add(item);
            }

            PromoteIdleExecution(DispatcherPriority.Invalid);
        }

        protected override void IdleExecution()
        {
            lock (lockObject)
            {
                if (listPendingItems != null && listPendingItems.Count > 0)
                {
                    if (listTempItems == null)
                        listTempItems = new List<LogItem>();
                    listTempItems.AddRange(listPendingItems);
                    listPendingItems.Clear();
                }
            }

            using (var updater = new CollectionUpdater(logItems))
            {
                for (int i = 0; i < MaxFlushing && listTempItems.Count > 0; ++i)
                {
                    LogItem logItem = listTempItems[0];
                    listTempItems.RemoveAt(0);
                    logItems.Add(logItem);
                    if (MaxItems > 0 && logItems.Count > MaxItems)
                        logItems.RemoveAt(0);

                    Tracing.SimpleLogging.WriteToLog(LogSource, logItem.Message, logItem.TimeStamp);
                }
            }

            if (listTempItems.Count > 0)
                PromoteIdleExecution(DispatcherPriority.Invalid);

            if (timerCleanOlderFiles == null)
                timerCleanOlderFiles = new Timer((o) =>
                     {
                         String[] listFiles = Tracing.SimpleLogging.GetLogFileNameList(LogSource);
                         if (listFiles != null)
                         {
                             foreach (string fileOn in listFiles)
                             {
                                 FileInfo file = new FileInfo(fileOn);
                                 if (DateTime.Now - file.LastWriteTime > MaxAgeFiles)
                                 {
                                     try
                                     {
                                         file.Delete();
                                     }
                                     catch (Exception ex)
                                     {
                                     }
                                 }
                             }
                         }
                     }, this, TimeSpan.FromDays(1), TimeSpan.FromDays(1));
        }
        #endregion

        #region Commands
        RelayCommand _jumpToCommand;
        public ICommand JumpToCommand
        {
            get
            {
                if (_jumpToCommand == null)
                {
                    _jumpToCommand = new RelayCommand(
                        param => JumpToUri(),
                        param => CanJumpTo
                        );
                }
                return _jumpToCommand;
            }
        }

        void JumpToUri()
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(logItems);
            LogItem item = view.CurrentItem as LogItem;
            if (item != null && item.Source != null)
            {
                throw new NotImplementedException(); 
            }
        }

        bool CanJumpTo
        {
            get 
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(logItems);
                LogItem item = view.CurrentItem as LogItem;
                return item != null && item.Source != null;
            }
        }

        RelayCommand _deleteCommand;
        public ICommand DeleteCommand
        {
            get
            {
                if (_deleteCommand == null)
                {
                    _deleteCommand = new RelayCommand(
                        param => DeleteCurrent(),
                        param => CanDelete
                        );
                }
                return _deleteCommand;
            }
        }

        void DeleteCurrent()
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(logItems);
            LogItem item = view.CurrentItem as LogItem;
            if (item != null && item.Source != null)
                logItems.Remove(item);
        }

        bool CanDelete
        {
            get
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(logItems);
                LogItem item = view.CurrentItem as LogItem;
                return item != null;
            }
        }

        #endregion
    }
}
