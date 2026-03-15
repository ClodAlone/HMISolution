using System;
using System.Collections.Generic;
using System.Linq;
using ReportSettings.Documents;
using DocumentManager.ComponentService;
using System.Threading;
using DevExpress.XtraReports.UI;
using System.IO;
using ReportManager.ComponentService;
using System.Windows;
using System.Threading.Tasks;
using ReportManager.ReportService;
//using System.Net.Mail;
using System.Text.RegularExpressions;
#if !NET_STANDARD
using UIMsgBoxAlertService.ComponentService;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
#endif
using log4net;
using Utilities;
using UFInterfaces;
using SmtpSender;

namespace ReportManager
{
    class ReportExecuter : IDisposable
    {
        #region Declarations
        //readonly Object lockObject = new Object();
        readonly ReportManagerComponent reportManager;
        internal readonly ReportDocument reportDocument;
        readonly ReportServiceHelper reportHelper;
#if !NET_STANDARD
        readonly IUIMsgBoxAlertService uiInterface;
        readonly IBusyComponent busyComponent;
        private static readonly ILog log = LogManager.GetLogger(Properties.Resources.ReportExecuter);
#else
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.ReportExecuter);
#endif
        List<ReportParameters.Parameter> parameters = null;

        readonly List<Action> pendingActions = new List<Action>();
        readonly CancellationTokenSource ctsPendingTasks = new CancellationTokenSource();
        Task pendingTask;
        ManualResetEvent pendingTaskTerminated;
        #endregion

        public ReportExecuter(ReportManagerComponent reportManager, ReportDocument reportDocument, DefaultSourceType sourceType = DefaultSourceType.Undefined, String title = null)
        {
            this.reportManager = reportManager;
            this.reportDocument = reportDocument;
            this.reportHelper = new ReportServiceHelper(reportDocument, this.reportManager.UFUAEditor, sourceType, throwOnException: true);
#if !NET_STANDARD
            uiInterface = reportDocument.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
            busyComponent = reportDocument.GetService(typeof(IBusyComponent)) as IBusyComponent;
#endif
        }

        public void Start(IDocument parent, ExecutionMode mode, Object Context, string title = null)
        {
            var type = ReportCommandType.Print;
            string connection = string.Empty;
            String folder = string.Empty;
            String filename = string.Empty;
            String from = string.Empty;
            String fromalias = string.Empty;
            String subject = string.Empty;
            String mobject = string.Empty;
            List<String> smtpsettings = null;
            IEntityReference entity = null;
            parameters = null;
            reportHelper.MaxTake = 0;
            reportHelper.CommandTimeout = 0;

            Dictionary<string, string> recipient = new Dictionary<string, string>();
            ExportFileType filetype = ExportFileType.Pdf;

            if (Context is IDictionary<String, Object>)
            {
                var map = Context as IDictionary<String, Object>;
                if (map.ContainsKey("Connection"))
                    connection = (string)map["Connection"];
                if (map.ContainsKey("Folder"))
                    folder = (String)map["Folder"];
                if (map.ContainsKey("FileName"))
                    filename = (String)map["FileName"];
                if (map.ContainsKey("FileType"))
                    filetype = (ExportFileType)map["FileType"];
                if (map.ContainsKey("CommandType"))
                    type = (ReportCommandType)map["CommandType"];
                if (map.ContainsKey("Parameters"))
                    parameters = map["Parameters"] as List<ReportParameters.Parameter>;
                if (map.ContainsKey("MaxTake"))
                    reportHelper.MaxTake = (int)map["MaxTake"];
                if (map.ContainsKey("CommandTimeout"))
                    reportHelper.CommandTimeout = (int)map["CommandTimeout"];
                if (map.ContainsKey("Entity"))
                    entity = map["Entity"] as IEntityReference;
            }

#if !NET_STANDARD
            Window curwnd = null;
            try
            {
                var target = entity?.ContainedObject as DependencyObject;
                if (target == null)
                    target = System.Windows.Input.Keyboard.FocusedElement as DependencyObject;
                if (target != null)
                    curwnd = Window.GetWindow(target);
                if (curwnd == null)
                    curwnd = Application.Current.MainWindow;
            }
            catch 
            { }

            Dispatcher currentDispatcher = curwnd?.Dispatcher;
            if (currentDispatcher == null)
                currentDispatcher = Dispatcher.FromThread(System.Threading.Thread.CurrentThread);
#endif

            switch (type)
            {
                case ReportCommandType.Send:
                    {
                        if (Context is IDictionary<String, Object>)
                        {
                            var map = Context as IDictionary<String, Object>;
                            if (map.ContainsKey("SMTPSettings"))
                                smtpsettings = map["SMTPSettings"] as List<String>;
                            if (map.ContainsKey("Recipient"))
                                recipient = map["Recipient"] as Dictionary<string, string>;

                            if (recipient == null || recipient.Count() == 0 || smtpsettings == null || smtpsettings.Count != 6)
                            {
                                log.ErrorFormat(Properties.Resources.MissingParameterExecutingReportCommand, type);
                                return;
                            }

                            if (map.ContainsKey("From"))
                                from = IsValidEmailAddress((String)map["From"]) ? (String)map["From"] : smtpsettings[0];
                            if (map.ContainsKey("FromAlias"))
                                fromalias = !string.IsNullOrEmpty((String)map["FromAlias"]) ? (String)map["FromAlias"] : Properties.Resources.FromAlias;
                            if (map.ContainsKey("MailSubject"))
                                subject = (String)map["MailSubject"];
                            if (map.ContainsKey("MailObject"))
                                mobject = (String)map["MailObject"];
                        }

                        if (!IsValidEmailAddress(from) || string.IsNullOrEmpty(filename))
                        {
                            var error = string.Format(Properties.Resources.MissingParameterExecutingReportCommand, type);
                            log.Error(error);
#if !NET_STANDARD
                            if (uiInterface != null)
                            {
                                if (mode == ExecutionMode.Synchro)
                                    uiInterface.ShowError(error);
                            }
#endif
                            return;
                        }

                        var action = new Action(() =>
                        {
                            try
                            {
                                using (var report = reportHelper.PrepareXtraReportDocument(parameters))
                                {
                                    report.CreateDocument();

                                    using (MemoryStream mem = new MemoryStream())
                                    {
                                        ServerSettings serverSettings = new ServerSettings();
                                        MailSettings mail = new MailSettings();
                                        switch (filetype)
                                        {
                                            case ExportFileType.Csv:
                                                report.ExportToCsv(mem);
                                                mem.Seek(0, System.IO.SeekOrigin.Begin);
                                                mail.AddAttachment(mem, filename, "application/csv");
                                                break;
                                            case ExportFileType.Html:
                                                report.ExportToHtml(mem);
                                                mem.Seek(0, System.IO.SeekOrigin.Begin);
                                                mail.AddAttachment(mem, filename, "application/html");
                                                break;
                                            case ExportFileType.Xls:
                                                report.ExportToXls(mem);
                                                mem.Seek(0, System.IO.SeekOrigin.Begin);
                                                mail.AddAttachment(mem, filename, "application/xls");
                                                break;
                                            case ExportFileType.Pdf:
                                                report.ExportToPdf(mem);
                                                mem.Seek(0, System.IO.SeekOrigin.Begin);
                                                mail.AddAttachment(mem, filename, "application/pdf");
                                                break;
                                            default:
                                                log.ErrorFormat(Properties.Resources.MissingParameterExecutingReportCommand, type);
                                                return;
                                        }
                                        //smtpsettings[0].StaticFromAddress);
                                        //smtpsettings[1].ServerAddress);
                                        //smtpsettings[2].PortNumber.ToString());
                                        //smtpsettings[3].EnAutentication.ToString());
                                        //smtpsettings[4].UserName);
                                        //smtpsettings[5].Password);

                                        serverSettings.ServerAddress = smtpsettings[1];
                                        serverSettings.ServerPort = int.Parse(smtpsettings[2]);
                                        if(bool.Parse(smtpsettings[3]))
                                        {
                                            serverSettings.Password = smtpsettings[5];
                                            serverSettings.User = smtpsettings[4];
                                        }
                                        if (fromalias.Length > 0)
                                            mail.From = string.Format("{0} <{1}>", fromalias, from);
                                        else
                                            mail.From = from;

                                        foreach (var key in recipient.Keys)
                                        {
                                            if (IsValidEmailAddress(recipient[key]))
                                                mail.Address.Add(recipient[key]);
                                        }
                                        mail.Subject = (subject == null ? "" : subject);
                                        mail.Message = (mobject == null ? "" : mobject);
                                        mail.Name = !string.IsNullOrEmpty(title) ? title : reportDocument.Title;
                                        using (var smtp = new SmtpClientModule())
                                        {
                                            smtp.Init(serverSettings);
                                            var result = smtp.SendMail(mail);
                                            var info = string.Format("{0} {1}", Properties.Resources.MessageSent, !string.IsNullOrEmpty(title) ? title : reportDocument.Title);
                                            if (result != SmtpErrors.NoError)
                                                info = string.Format("{0} {1} '{2}'", Properties.Resources.MessageError, !string.IsNullOrEmpty(title) ? title : reportDocument.Title, smtp.ErrorMessage);
                                            log.Info(info);
#if !NET_STANDARD
                                            if (uiInterface != null)
                                            {
                                                if (mode == ExecutionMode.Synchro)
                                                    uiInterface.ShowInformation(info);
                                            }
#endif
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                var error = string.Format(Properties.Resources.ReportDataSendingError, ex.Message);
                                log.Error(error, ex);
#if !NET_STANDARD
                                if (uiInterface != null)
                                {
                                    if (mode == ExecutionMode.Synchro)
                                        uiInterface.ShowError(error);
                                }
#endif
                            }
                        });
#if !NET_STANDARD
                        if (currentDispatcher != null && mode == ExecutionMode.Synchro)
                        {
                            currentDispatcher.InvokeIfRequired(() => 
                            {
                                if (busyComponent != null)
                                {
                                    busyComponent.IsBusy = true;
                                    busyComponent.BusyContent = Properties.Resources.ReportDataSendInProgress;
                                }

                                try
                                {
                                    action();
                                }
                                finally
                                {
                                    if (busyComponent != null)
                                        busyComponent.IsBusy = false;
                                }
                            });
                        }
                        else
#endif
                        {
                            try
                            {
                                StartAsyncOperations(action);
                            }
                            catch (Exception ex)
                            {
                                var error = string.Format(Properties.Resources.ReportDataSendingError, ex.Message);
                                log.Error(error);
#if !NET_STANDARD
                                if (uiInterface != null)
                                {
                                    if (mode == ExecutionMode.Synchro)
                                        uiInterface.ShowError(error);
                                }
#endif
                            }
                        }
                    }
                    break;
                case ReportCommandType.Save:
                    {
                        if (string.IsNullOrEmpty(folder) || string.IsNullOrEmpty(filename))
                        {
                            var error = string.Format(Properties.Resources.MissingParameterExecutingReportCommand, type);
                            log.Error(error);
#if !NET_STANDARD
                            if (uiInterface != null)
                            {
                                if (mode == ExecutionMode.Synchro)
                                    uiInterface.ShowError(error);
                            }
#endif
                            return;
                        }

                        var action = new Action(() =>
                        {
                            try
                            {
                                var report = reportHelper.PrepareXtraReportDocument(parameters);
                                report.CreateDocument();

                                if (!Directory.Exists(folder))
                                    Directory.CreateDirectory(folder);

                                bool bSaved;
                                switch (filetype)
                                {
                                    case ExportFileType.Csv:
                                        report.ExportToCsv(Path.Combine(folder, filename));
                                        bSaved = true;
                                        break;
                                    case ExportFileType.Html:
                                        report.ExportToHtml(Path.Combine(folder, filename));
                                        bSaved = true;
                                        break;
                                    case ExportFileType.Xls:
                                        report.ExportToXls(Path.Combine(folder, filename));
                                        bSaved = true;
                                        break;
                                    case ExportFileType.Pdf:
                                        report.ExportToPdf(Path.Combine(folder, filename));
                                        bSaved = true;
                                        break;
                                    default:
                                        bSaved = false;
                                        log.ErrorFormat(Properties.Resources.MissingParameterExecutingReportCommand, type);
                                        break;
                                }

                                if (bSaved)
                                {
                                    var info = string.Format("{0} {1}", Properties.Resources.ReportSaved, !string.IsNullOrEmpty(title) ? title : reportDocument.Title);
                                    log.Info(info);
#if !NET_STANDARD
                                    if (uiInterface != null)
                                    {
                                        if (mode == ExecutionMode.Synchro)
                                            uiInterface.ShowInformation(info);
                                    }
#endif
                                }
                            }
                            catch (Exception ex)
                            {
                                var error = string.Format(Properties.Resources.ReportDataSavingError, ex.Message);
                                log.Error(error, ex);
#if !NET_STANDARD
                                if (uiInterface != null)
                                {
                                    if (mode == ExecutionMode.Synchro)
                                        uiInterface.ShowError(error);
                                }
#endif
                            }
                        });

#if !NET_STANDARD
                        if (currentDispatcher != null && mode == ExecutionMode.Synchro)
                        {
                            currentDispatcher.InvokeIfRequired(() =>
                            {
                                if (busyComponent != null)
                                {
                                    busyComponent.IsBusy = true;
                                    busyComponent.BusyContent = Properties.Resources.ReportDataSavingInProgress;
                                }

                                try
                                {
                                    action();
                                }
                                finally
                                {
                                    if (busyComponent != null)
                                        busyComponent.IsBusy = false;
                                }
                            });
                        }
                        else
#endif
                        {
                            try
                            {
                                StartAsyncOperations(action);
                            }
                            catch (Exception ex)
                            {
                                var error = string.Format(Properties.Resources.ReportDataSavingError, ex.Message);
                                log.Error(error);
#if !NET_STANDARD
                                if (uiInterface != null)
                                {
                                    if (mode == ExecutionMode.Synchro)
                                        uiInterface.ShowError(error);
                                }
#endif
                            }
                        }
                    }
                    break;
                case ReportCommandType.Show:
                    {
#if !NET_STANDARD
                        if (currentDispatcher == null)
#endif
                        {
                            var error = string.Format(Properties.Resources.MissingUIThreadExecutingReportCommand, type);
                            log.Error(error);
#if !NET_STANDARD
                            if (uiInterface != null)
                                uiInterface.ShowError(error);
#endif
                            return;
                        }
#if !NET_STANDARD
                        currentDispatcher.InvokeIfRequired(() =>
                        {
                            var viewer = new ReportViewerUI(reportManager, reportDocument, parent, parent.ActiveView, parameters, title, reportHelper);
                            var wnd = new DXWindow()
                            {
                                Owner = curwnd,
                                Content = viewer
                            };

                            wnd.Closed += (ob, eve) =>
                            {
                                viewer.Dispose();
                            };

                            if (mode == ExecutionMode.Synchro)
                            {
                                wnd.ShowDialog();
                                wnd.Close();
                            }
                            else
                                wnd.Show();
                        });
#endif
                    }
                    break;
#if NET_STANDARD
                case ReportCommandType.PrintDialog:
#endif
                case ReportCommandType.Print:
                    {
#if !NET_STANDARD
                        if (currentDispatcher != null && mode == ExecutionMode.Synchro)
                        {
                            currentDispatcher.InvokeIfRequired(() =>
                            {
                                var viewer = new PrintingProgressBar();
                                var wnd = new DXWindow()
                                {
                                    Owner = curwnd,
                                    Content = viewer,
                                    SizeToContent = SizeToContent.WidthAndHeight,
                                    WindowStyle = WindowStyle.None,
                                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                                    ShowInTaskbar = false,
                                    ResizeMode = ResizeMode.NoResize
                                };

                                wnd.Show();

                                try
                                {
                                    PrintReport(false, wnd);
                                    var info = string.Format("{0} {1}", Properties.Resources.ReportPrinted, !string.IsNullOrEmpty(title) ? title : reportDocument.Title);
                                    log.Info(info);
                                }
                                catch (Exception ex)
                                {
                                    var error = string.Format(Properties.Resources.ReportDataPrintingError, ex.Message);
                                    wnd.Close();
                                    log.Error(error, ex);
                                    if (uiInterface != null)
                                        uiInterface.ShowError(error);
                                }
                                finally
                                {
                                    wnd.Close();
                                }
                            });
                        }
                        else
#endif
                        {
                            var action = new Action(() =>
                            {
                                try
                                {
                                    PrintReport(false);
                                    var info = string.Format("{0} {1}", Properties.Resources.ReportPrinted, !string.IsNullOrEmpty(title) ? title : reportDocument.Title);
                                    log.Info(info);
#if !NET_STANDARD
                                    if (uiInterface != null)
                                    {
                                        if (mode == ExecutionMode.Synchro)
                                            uiInterface.ShowInformation(info);
                                    }
#endif
                                }
                                catch (Exception ex)
                                {
                                    var error = string.Format(Properties.Resources.ReportDataPrintingError, ex.Message);
                                    log.Error(error, ex);
#if !NET_STANDARD
                                    if (uiInterface != null)
                                    {
                                        if (mode == ExecutionMode.Synchro)
                                            uiInterface.ShowError(error);
                                    }
#endif
                                }
                            });

                            try
                            {
                                StartAsyncOperations(action);
                            }
                            catch (Exception ex)
                            {
                                var error = string.Format(Properties.Resources.ReportDataSavingError, ex.Message);
                                log.Error(error);
#if !NET_STANDARD
                                if (uiInterface != null)
                                {
                                    if (mode == ExecutionMode.Synchro)
                                        uiInterface.ShowError(error);
                                }
#endif
                            }
                        }
                    }
                    break;
#if !NET_STANDARD
                case ReportCommandType.PrintDialog:
                    {
                        if (currentDispatcher == null)
                        {
                            var error = string.Format(Properties.Resources.MissingUIThreadExecutingReportCommand, type);
                            log.Error(error);
                            if (uiInterface != null)
                                uiInterface.ShowError(error);
                            return;
                        }

                        currentDispatcher.InvokeIfRequired(() =>
                        {
                            var viewer = new PrintingProgressBar();
                            var wnd = new DXWindow
                            {
                                Owner = curwnd,
                                Content = viewer,
                                SizeToContent = SizeToContent.WidthAndHeight,
                                WindowStyle = WindowStyle.None,
                                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                                ShowInTaskbar = false,
                                ResizeMode = ResizeMode.NoResize
                            };

                            wnd.Show();

                            try
                            {
                                PrintReport(true, wnd);
                                var info = string.Format("{0} {1}", Properties.Resources.ReportPrinted, !string.IsNullOrEmpty(title) ? title : reportDocument.Title);
                                log.Info(info);
                            }
                            catch (Exception ex)
                            {
                                var error = string.Format(Properties.Resources.ReportDataPrintingError, ex.Message);
                                log.Error(error, ex);
                                if (mode == ExecutionMode.Synchro)
                                {
                                    wnd.Close();
                                    if (uiInterface != null)
                                        uiInterface.ShowError(error);
                                }
                            }
                            finally
                            {
                                wnd.Close();
                            }
                        });
                    }
                    break;
#endif
            }
        }

        bool IsValidEmailAddress(string s)
        {
            var regex = new Regex(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");
            return regex.IsMatch(s);
        }

        void PrintReport(bool showdialog
#if !NET_STANDARD
            , Window progressbar = null
#endif
            )
        {
            using (var report = reportHelper.PrepareXtraReportDocument(parameters))
            {
#if !NET_STANDARD
                if (progressbar != null && progressbar.Content is PrintingProgressBar)
                    (progressbar.Content as PrintingProgressBar).Report = report;

                if (showdialog)
                {
                    progressbar.Hide();
                    report.PrintDialog();
                }
                else
#endif
                {
                    report.Print();
                }
            }
        }

        void StartAsyncOperations(Action action)
        {
            lock (pendingActions)
            {
                if (bDisposed)
                    return;

                if (pendingActions.Count >= Properties.Settings.Default.MaxPendingOperations)
                {
                    var message = String.Format(Properties.Resources.MaxPendingOperationsExceeded, reportDocument.Title);
                    throw new OverflowException(message);
                }
                else
                {
                    pendingActions.Add(action);
                }

                EnsureTaskExecution();
            }
        }

        void EnsureTaskExecution()
        {
            if (ctsPendingTasks.IsCancellationRequested)
                return;

            if (pendingTask == null)
            {
                pendingTask = Task.Factory.StartNew(() =>
                {
                    Action action = null;
                    lock (pendingActions)
                    {
                        action = pendingActions[0];
                        pendingActions.RemoveAt(0);
                    }
                    action();
                }, ctsPendingTasks.Token);

                pendingTask.ContinueWith((T) =>
                {
                    lock (pendingActions)
                    {
                        pendingTask = null;
                        if (pendingActions.Count > 0)
                            EnsureTaskExecution();
                        else if (pendingTaskTerminated != null)
                            pendingTaskTerminated.Set();
                    }
                }, ctsPendingTasks.Token);
            }
        }

        void StopAsyncOperations()
        {
            lock (pendingActions)
            {
                if (pendingActions.Count > 0)
                    pendingTaskTerminated = new ManualResetEvent(false);
            }

            if (pendingTaskTerminated != null)
            {
                bool terminated = pendingTaskTerminated.WaitOne(Properties.Settings.Default.MaxWaitPendingOperationsOnClosing);
                if (!terminated)
                {
                    log.WarnFormat(Properties.Resources.AbortedPendingOperations, reportDocument.Title);
                    ctsPendingTasks.Cancel();
                }
            }

            Task task = null;
            lock (pendingActions)
            {
                ctsPendingTasks.Cancel();
                task = pendingTask;
            }

            if (task != null)
                task.Wait();

            ctsPendingTasks.Dispose();
            if (pendingTaskTerminated != null)
                pendingTaskTerminated.Dispose();
        }

        #region IDisposable
        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            StopAsyncOperations();
        }
        #endregion
    }
}
