using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Printing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using UIMsgBoxAlertService.ComponentService;
using Utilities;
using ViewModelLib;
using WPFUtilities;

namespace DataValidation
{
    public class DataValidationViewModel : INotifyPropertyChanged
    {
        #region Declarations
        readonly List<BaseValidation> validationItems = new List<BaseValidation>();
        readonly bool isFakeInstance;

        DelayedSingleActionInvoker delayInvoker;
        CancellationTokenSource cts;
        bool bFilledBackupFiles;
        #endregion

        #region Constructors
        public DataValidationViewModel()
        {
            isFakeInstance = true;
        }

        public DataValidationViewModel(IEnumerable<BaseValidation> validationItems)
        {
            this.validationItems.AddRange(validationItems);
        }
        #endregion

        #region Methods
        static string userSid;
        static string GetCFR21UserSID()
        {
            return GetCFR21UserSID(null, null);
        }

        static string GetCFR21UserSID(string usernName, string password)
        {
            if (userSid != null)
                return userSid;

            var domainName = UFUAServerInfo.UFUAServerInfo.GetCFR21DomainName();
            var contextType = domainName == null ? ContextType.Machine : ContextType.Domain;
            using (PrincipalContext context = new PrincipalContext(contextType, domainName ?? System.Environment.MachineName, usernName, password))
            {
                var user = UserPrincipal.FindByIdentity(context, UFUAServerInfo.UFUAServerInfo.GetCFR21UserName());
                if (user != null)
                {
                    var byteSid = new byte[user.Sid.BinaryLength];
                    user.Sid.GetBinaryForm(byteSid, 0);
                    userSid = BitConverter.ToString(byteSid).Replace("-", "");
                }
            }

            return userSid;
        }

        void CurrentItem_ProcessingRow(object sender, RowEventArg e)
        {
            WaitText = e.Message;
            ProcessedRows = e.ProcessedRows;
            TotalRows = e.TotalRows;
        }
        #endregion

        #region Properties
        public IReadOnlyList<BaseValidation> ItemsSource
        {
            get
            {
                return validationItems.AsReadOnly();
            }
        }

        BaseValidation currentItem;
        public BaseValidation CurrentItem
        {
            get
            {
                return currentItem;
            }
            set
            {
                if (currentItem == value)
                    return;
                currentItem = value;

                if (IsRunning)
                {
                    AbortValidateData();
                }
                else
                {
                    ValidationResult = ValidationResults.None;
                    LastError = null;
                }

                bFilledBackupFiles = false;
                if (ShowBackupFilesMode)
                    FillBackupFiles();

                OnPropertyChanged("CurrentItem");
            }
        }

        bool allowChangeItemSource = true;
        public bool AllowChangeItemSource
        {
            get
            {
                return allowChangeItemSource;
            }
            set
            {
                if (allowChangeItemSource == value)
                    return;
                allowChangeItemSource = value;
                OnPropertyChanged("AllowChangeItemSource");
            }
        }


        string waitText;
        public string WaitText
        {
            get
            {
                if (isFakeInstance || ShowBackupFilesMode)
                    return Properties.Resources.LoadingWaitText;
                else if (cts != null && cts.IsCancellationRequested)
                    return Properties.Resources.AbortingWaitText;
                else if (waitText != null)
                    return waitText;
                else
                    return Properties.Resources.ValidateWaitText;
            }
            private set
            {
                if (waitText == value)
                    return;
                waitText = value;

                if (delayInvoker == null)
                    OnPropertyChanged("WaitText");
                else
                    delayInvoker.BeginInvoke();
            }
        }

        public int processedRows;
        public int ProcessedRows
        {
            get
            {
                return processedRows;
            }
            private set
            {
                if (processedRows == value)
                    return;
                processedRows = value;

                if (delayInvoker == null)
                    OnPropertyChanged("ProcessedRows");
                else
                    delayInvoker.BeginInvoke();
            }
        }

        public int totalRows;
        public int TotalRows
        {
            get
            {
                return totalRows;
            }
            private set
            {
                if (totalRows == value)
                    return;
                totalRows = value;

                if (delayInvoker == null)
                    OnPropertyChanged("TotalRows");
                else
                    delayInvoker.BeginInvoke();
            }
        }

        public long totalUnauthorizedAccess;
        public long TotalUnauthorizedAccess
        {
            get
            {
                return totalUnauthorizedAccess;
            }
            private set
            {
                if (totalUnauthorizedAccess == value)
                    return;
                totalUnauthorizedAccess = value;

                OnPropertyChanged("TotalUnauthorizedAccess");
            }
        }

        bool isRunning;
        public bool IsRunning
        {
            get
            {
                return isFakeInstance || isRunning;
            }
            set
            {
                if (isRunning == value)
                    return;
                isRunning = value;

                if (isRunning)
                {
                    ValidationResult = ValidationResults.None;
                    LastError = null;
                    BackupFilesError = null;
                    waitText = null;
                    ProcessedRows = 0;
                    TotalRows = 0;
                }

                OnPropertyChanged("IsRunning");
                OnPropertyChanged("WaitText");

                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            }
        }

        ValidationResults validationResult = ValidationResults.None;
        public ValidationResults ValidationResult
        {
            get
            {
                return validationResult;
            }
            private set
            {
                if (validationResult == value)
                    return;
                validationResult = value;
                OnPropertyChanged("ValidationResult");
            }
        }

        string lastError;
        public string LastError
        {
            get
            {
                return lastError;
            }
            private set
            {
                if (lastError == value)
                    return;
                lastError = value;
                OnPropertyChanged("LastError");
            }
        }

        bool showBackupFilesMode;
        public bool ShowBackupFilesMode
        {
            get
            {
                return showBackupFilesMode;
            }
            set
            {
                if (showBackupFilesMode == value)
                    return;
                showBackupFilesMode = value;
                if (showBackupFilesMode && !bFilledBackupFiles)
                    FillBackupFiles();
                OnPropertyChanged("ShowBackupFilesMode");
            }
        }

        string backupFilesError;
        public string BackupFilesError
        {
            get
            {
                return backupFilesError;
            }
            private set
            {
                if (backupFilesError == value)
                    return;
                backupFilesError = value;
                OnPropertyChanged("BackupFilesError");
            }
        }

        ObservableCollection<BackupFileViewModel> backupFiles;
        public ObservableCollection<BackupFileViewModel> BackupFiles
        {
            get
            {
                if (backupFiles == null)
                    backupFiles = new ObservableCollection<BackupFileViewModel>();
                return backupFiles;
            }
        }

        public int maxUnauthorizedAccess = Properties.Settings.Default.MaxUnauthorizedAccess;
        public int MaxUnauthorizedAccess
        {
            get
            {
                return maxUnauthorizedAccess;
            }
            set
            {
                if (maxUnauthorizedAccess == value)
                    return;
                maxUnauthorizedAccess = value;
                OnPropertyChanged("MaxUnauthorizedAccess");
            }
        }

        public int queryTimeout = Properties.Settings.Default.DefaultQueryTimeout;
        public int QueryTimeout
        {
            get
            {
                return queryTimeout;
            }
            set
            {
                if (queryTimeout == value)
                    return;
                queryTimeout = value;
                OnPropertyChanged("QueryTimeout");
            }
        }

        /// <summary>
        /// SDDL string for the SID used to validate the data
        /// </summary>
        public string ValidationSID { get; internal set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public bool AbortOnFirstError { get; set; }

        public IUIMsgBoxAlertService UIInterface { get; set; }

        #endregion

        #region Commands
        RelayCommand validateDataCommand;
        public ICommand ValidateDataCommand
        {
            get
            {
                if (!isFakeInstance && validateDataCommand == null)
                {
                    validateDataCommand = new RelayCommand(
                        param => StartValidateData(),
                        param => CurrentItem != null && !IsRunning
                        );
                }
                return validateDataCommand;
            }
        }

        RelayCommand loadDataCommand;
        public ICommand LoadDataCommand
        {
            get
            {
                if (!isFakeInstance && loadDataCommand == null)
                {
                    loadDataCommand = new RelayCommand(
                        param => LoadData((DateSpan)param),
                        param => CurrentItem != null && !IsRunning
                        );
                }
                return loadDataCommand;
            }
        }

        RelayCommand abortCommand;
        public ICommand AbortCommand
        {
            get
            {
                if (!isFakeInstance && abortCommand == null)
                {
                    abortCommand = new RelayCommand(
                        param => AbortValidateData(),
                        param => IsRunning
                        );
                }
                return abortCommand;
            }
        }

        public void LoadData(DateSpan dateSpan)
        {
            StartComputingData(null, null, dateSpan, true);
        }

        public void StartValidateData()
        {
            StartComputingData(null, null, DateSpan.None, false);
        }

        void SetTimeSpan(DateSpan span)
        {
            var date1 = StartDateTime.HasValue ? StartDateTime.Value : DateTime.MinValue;
            var date2 = EndDateTime.HasValue ? EndDateTime.Value : DateTime.MaxValue;

            switch (span)
            {
                case DateSpan.Minute:
                    DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, WPFUtilities.DateSpan.Minute);
                    break;
                case DateSpan.Hour:
                    DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, WPFUtilities.DateSpan.Hour);
                    break;
                case DateSpan.Day:
                    DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, WPFUtilities.DateSpan.Day);
                    break;
                case DateSpan.Week:
                    DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, WPFUtilities.DateSpan.Week);
                    break;
                case DateSpan.Month:
                    DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, WPFUtilities.DateSpan.Month);
                    break;
                case DateSpan.Year:
                    DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, WPFUtilities.DateSpan.Year);
                    break;
                case DateSpan.All:
                default:
                    StartDateTime = null;
                    EndDateTime = null;
                    OnPropertyChanged("StartDateTime");
                    OnPropertyChanged("EndDateTime");
                    return;
            };

            StartDateTime = date1;
            EndDateTime = date2;
            OnPropertyChanged("StartDateTime");
            OnPropertyChanged("EndDateTime");
        }

        void StartComputingData(string userName, string password, DateSpan dateSpan, bool bSkipValidation)
        {
            var currentItem = CurrentItem;
            if (currentItem == null || IsRunning)
                return;

            if (dateSpan != DateSpan.None)
                SetTimeSpan((DateSpan)Enum.Parse(typeof(DateSpan), dateSpan.ToString(), true));

            ShowBackupFilesMode = false;
            IsRunning = true;

            cts = new CancellationTokenSource();
            currentItem.Token = cts.Token;
            currentItem.ProcessingRow += CurrentItem_ProcessingRow;
            currentItem.UnauthorizedAccess += CurrentItem_UnauthorizedAccess;
            delayInvoker = new DelayedSingleActionInvoker(() =>
            {
                OnPropertyChanged("WaitText");
                OnPropertyChanged("ProcessedRows");
                OnPropertyChanged("TotalRows");
            }, TimeSpan.FromMilliseconds(100), restart: false);

            string userSid = null;
            bool bShowCredential = false;
            var task1 = Task.Factory.StartNew(() =>
            {
                var oldPriority = Thread.CurrentThread.Priority;
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;

                try
                {
                    var startDateTime = StartDateTime.HasValue ? StartDateTime.Value : DateTime.MinValue;
                    var endDateTime = EndDateTime.HasValue ? EndDateTime.Value : DateTime.MaxValue;
                    currentItem.queryTimeout = QueryTimeout;
                    currentItem.skipValidation = bSkipValidation;
                    if (!currentItem.skipValidation)
                    {
                        if (!String.IsNullOrEmpty(ValidationSID))
                        {
                            try
                            {
                                var sid = new SecurityIdentifier(ValidationSID);
                                var byteSid = new byte[sid.BinaryLength];
                                sid.GetBinaryForm(byteSid, 0);
                                userSid = BitConverter.ToString(byteSid).Replace("-", String.Empty);
                            }
                            catch
                            {
                                throw new ArgumentException(String.Format(Properties.Resources.InvalidValidationSID, ValidationSID));
                            }
                        }
                        else
                        {
                            try
                            {
                                userSid = GetCFR21UserSID(userName, password);
                            }
                            catch
                            {
                                bShowCredential = true;
                                throw;
                            }
                        }
                        currentItem.userSid = userSid;
                        var backupFiles = currentItem.GetBackupFiles();
                        foreach (var backupFile in BackupFiles.ToList())
                        {
                            if (!backupFiles.Contains(backupFile.FilePath))
                                backupFiles.Add(backupFile.FilePath);
                        }
                        currentItem.backupFiles = backupFiles.ToArray();
                    }
                    return currentItem.ValidateData(startDateTime, endDateTime, !AbortOnFirstError);
                }
                finally
                {
                    Thread.CurrentThread.Priority = oldPriority;
                }
            }, cts.Token, TaskCreationOptions.None, TaskScheduler.Default);

            task1.ContinueWith(ret =>
            {
                var aborted = cts.IsCancellationRequested;
                currentItem.ProcessingRow -= CurrentItem_ProcessingRow;
                currentItem.UnauthorizedAccess -= CurrentItem_UnauthorizedAccess;
                delayInvoker = null;

                cts.Dispose();
                cts = null;

                IsRunning = false;

                TotalUnauthorizedAccess = currentItem.totalUnauthorizedAccess;

                if (TotalUnauthorizedAccess > MaxUnauthorizedAccess)
                {
                    LastError = Properties.Resources.MaxUnauthorizedAccessError;
                    ValidationResult = ValidationResults.Failed;
                }
                else if (aborted)
                    ValidationResult = ValidationResults.Aborted;
                else if (ret.Exception != null)
                {
                    if (bShowCredential && !currentItem.skipValidation && userSid == null)
                    {
                        if (UIInterface != null)
                        {
                            ShowCredentialOptions options = new ShowCredentialOptions();
                            options.WindowTitle = Properties.Resources.ShowCredentialTitle;
                            options.MainInstruction = Properties.Resources.ShowCredentialMainInstruction;
                            options.Content = String.Format(Properties.Resources.ShowCredentialContent, UFUAServerInfo.UFUAServerInfo.GetCFR21DomainName() ?? Environment.MachineName);
                            options.SavedCredentialsBucket = Properties.Resources.ShowCredentialTitle;

                            ShowCredentialResults res = UIInterface.ShowCredentialDialog(options);
                            if (res.result == CustomDialogResults.OK)
                            {
                                StartComputingData(res.UserName, res.Password, dateSpan, bSkipValidation);
                                return;
                            }
                        }
                    }

                    var exception = ret.Exception.InnerException;
                    while (exception.InnerException != null)
                        exception = exception.InnerException;
                    LastError = exception.Message;
                    ValidationResult = ValidationResults.Error;
                }
                else
                    ValidationResult = currentItem.skipValidation ? ValidationResults.Error : ret.Result ? ValidationResults.Successful : ValidationResults.Failed;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void CurrentItem_UnauthorizedAccess(object sender, long e)
        {
            if (e > MaxUnauthorizedAccess)
                AbortValidateData();
        }

        public void AbortValidateData()
        {
            if (cts != null)
                cts.Cancel();

            OnPropertyChanged("WaitText");
        }

        public void FillBackupFiles()
        {
            var currentItem = CurrentItem;
            if (currentItem == null || IsRunning)
                return;

            IsRunning = true;

            cts = new CancellationTokenSource();
            currentItem.Token = cts.Token;
            BackupFiles.Clear();

            var task1 = Task.Factory.StartNew(() =>
            {
                var oldPriority = Thread.CurrentThread.Priority;
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;

                try
                {
                    return currentItem.GetBackupFiles();
                }
                finally
                {
                    Thread.CurrentThread.Priority = oldPriority;
                }
            }, cts.Token);
            task1.ContinueWith(ret =>
            {
                var aborted = cts.IsCancellationRequested;

                cts.Dispose();
                cts = null;

                IsRunning = false;

                if (aborted)
                    return;

                if (ret.Exception != null)
                {
                    var exception = ret.Exception.InnerException;
                    while (exception.InnerException != null)
                        exception = exception.InnerException;
                    BackupFilesError = exception.Message;
                }
                else if (ret.Result != null)
                {
                    foreach (var filePath in ret.Result)
                        BackupFiles.Add(new BackupFileViewModel(filePath, true));
                    bFilledBackupFiles = true;
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        #endregion
    }
}
