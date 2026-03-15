using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
#if !NET_STANDARD
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using CommandManager.ComponentService;
using WPFUtilities.PropertyDataTemplate;
using UFInterfaces.PropertyControl;
using DevExpress.Xpo;
#endif
using UFUAEditor.ComponentService;
using OPCUAViewModel;
using UFInterfaces;
using Utilities;
using System.IO;
using UFUserEditor.ComponentService;
using DocumentManager.ComponentService;
using System.Threading;
using ReportManager.ComponentService;

namespace CommandManager
{
#if !WINDOWS_UWP && !NET_STANDARD
    [TypeConverter(typeof(LocalizedEnumConverter))]
#endif
    public enum AlarmCommandType
    {
        AckAll,
        ConfirmAll,
        ToggleSound,
        ShowStatisticReport,
        PrintStatisticReport,
        SaveStatisticReport,
        SendStatisticReport
    }

    [DataContract(Name = "AlarmCommand", Namespace = UFInterfaces.Constants.Namespaces.UriProgea)]
    public class AlarmCommand : CommandManager
    {

        OPCUAEntityReference toggleSound = null;

        #region Properties
        [DataMember]
        AlarmCommandType type;
        public AlarmCommandType Type
        {
            get
            {
                return type;
            }
            set
            {
                if (type == value)
                    return;
                type = value;
#if !NET_STANDARD
                OnPropertyChanged("Type");
                OnPropertyChanged("CommandSummary");
                OnPropertyVisiblityChanged("Type");
#endif
            }
        }
        [DataMember]
        AlarmReportType reportType;
        public AlarmReportType ReportType
        {
            get { return reportType; }
            set
            {
                if (reportType == value)
                    return;
                reportType = value;
#if !NET_STANDARD
                OnPropertyChanged("ReportType");
#endif
            }
        }

        [DataMember]
        ExecutionMode executionMode;
        public ExecutionMode ExecutionMode
        {
            get
            {
                return executionMode;
            }
            set
            {
                if (executionMode == value)
                    return;
                executionMode = value;
#if !NET_STANDARD
                OnPropertyChanged("ExecutionMode");
#endif
            }
        }

        [DataMember(Name = "AlarmReportPeriodType")]
        ReportPeriodType periodType = ReportPeriodType.LastYear;
        public ReportPeriodType PeriodType
        {
            get { return periodType; }
            set
            {
                if (periodType == value)
                    return;
                periodType = value;
#if !NET_STANDARD
                OnPropertyChanged("PeriodType");
                OnPropertyVisiblityChanged("PeriodType");
#endif
            }
        }

        [DataMember]
        DateTime start = new DateTime(DateTime.Now.Year, 1, 1);
        public DateTime StartDate
        {
            get { return start; }
            set
            {
                if (start == value)
                    return;
                start = value;
#if !NET_STANDARD
                OnPropertyChanged("StartDate");
#endif
            }
        }

        [DataMember]
        DateTime end = new DateTime(DateTime.Now.Year, 12, 31);
        public DateTime EndDate
        {
            get { return end; }
            set
            {
                if (end == value)
                    return;
                end = value;
#if !NET_STANDARD
                OnPropertyChanged("EndDate");
#endif
            }
        }

        [DataMember]
        String alarmsSource;
        public String AlarmsSource
        {
            get
            {
                return alarmsSource;
            }
            set
            {
                if (alarmsSource == value)
                    return;
                alarmsSource = value;
#if !NET_STANDARD
                OnPropertyChanged("AlarmsSource");
#endif
            }
        }

        [DataMember]
        uint maxTake;
        public uint MaxTake
        {
            get
            {
                return maxTake;
            }
            set
            {
                if (maxTake == value)
                    return;
                maxTake = value;
#if !NET_STANDARD
                OnPropertyChanged("MaxTake");
#endif
            }
        }

        //[DataMember]
        //String printer;
        //public String Printer
        //{
        //    get { return printer; }
        //    set
        //    {
        //        if (printer == value)
        //            return;
        //        printer = value;
        //        OnPropertyChanged("Printer");
        //    }
        //}

        [DataMember]
        int commandTimeout;
        public int CommandTimeout
        {
            get
            {
                return commandTimeout;
            }
            set
            {
                if (commandTimeout == value)
                    return;
                commandTimeout = value;
#if !NET_STANDARD
                OnPropertyChanged("CommandTimeout");
#endif
            }
        }

        [DataMember]
        String folderpath;
        public String FolderPath
        {
            get
            {
                return folderpath;
            }
            set
            {
                if (folderpath == value)
                    return;
                folderpath = value;
#if !NET_STANDARD
                OnPropertyChanged("FolderPath");
#endif
            }
        }

        [DataMember]
        String filename;
        public String FileName
        {
            get { return filename; }
            set
            {
                if (filename == value)
                    return;
                filename = value;
#if !NET_STANDARD
                OnPropertyChanged("FileName");
#endif
            }
        }

        [DataMember]
        ExportFileType filetype;
        public ExportFileType FileType
        {
            get { return filetype; }
            set
            {
                if (filetype == value)
                    return;
                filetype = value;
#if !NET_STANDARD
                OnPropertyChanged("FileType");
#endif
            }
        }
        [DataMember]
        String recipient;
        public String Recipient
        {
            get { return recipient; }
            set
            {
                if (recipient == value)
                    return;
                recipient = value;
#if !NET_STANDARD
                OnPropertyChanged("Recipient");
#endif
            }
        }

        [DataMember]
        String from;
        public String From
        {
            get { return from; }
            set
            {
                if (from == value)
                    return;
                from = value;
#if !NET_STANDARD
                OnPropertyChanged("From");
#endif
            }
        }

        [DataMember]
        String fromalias;
        public String FromAlias
        {
            get { return fromalias; }
            set
            {
                if (fromalias == value)
                    return;
                fromalias = value;
#if !NET_STANDARD
                OnPropertyChanged("FromAlias");
#endif
            }
        }

        [DataMember]
        String mailsubject;
        public String MailSubject
        {
            get { return mailsubject; }
            set
            {
                if (mailsubject == value)
                    return;
                mailsubject = value;
#if !NET_STANDARD
                OnPropertyChanged("MailSubject");
#endif
            }
        }

        [DataMember]
        String mailobject;
        public String MailObject
        {
            get { return mailobject; }
            set
            {
                if (mailobject == value)
                    return;
                mailobject = value;
#if !NET_STANDARD
                OnPropertyChanged("MailObject");
#endif
            }
        }

        [DataMember]
        String connectionstring;
        public String ConnectionString
        {
            get { return connectionstring; }
            set
            {
                if (connectionstring == value)
                    return;
                connectionstring = value;
#if !NET_STANDARD
                OnPropertyChanged("ConnectionString");
#endif
            }
        }
        #endregion

        #region Methods
        MonitoredItemViewModel GetItemViewModel()
        {
            MonitoredItemViewModel monitor = null;
#if !NET_STANDARD
            if (OpcuaEntityReference == null)
            {
                if (Entity != null && Entity.ContainedObject is FrameworkElement)
                {
                    var ret = (Entity.ContainedObject as FrameworkElement).DataContext as MonitoredItemViewModel;
                    if (ret != null)
                        return ret;
                    if (Entity.ContainedObject is ContentControl)
                    {
                        var control = Entity.ContainedObject as ContentControl;
                        if (control.Content is FrameworkElement)
                            monitor = (control.Content as FrameworkElement).DataContext as MonitoredItemViewModel;
                    }
                }
            }
            else
#else
            if (OpcuaEntityReference != null)
#endif
                monitor = OpcuaEntityReference.MonitoredItemViewModel;

            if (monitor == null || !monitor.IsValid)
                return null;

            var list = monitor.ConditionStateList; // initialize the alarm client subscription
            return monitor;
        }

        private bool GetEReloadValue()
        {
            if (toggleSound != null && toggleSound.MonitoredItemViewModel != null &&
                toggleSound.MonitoredItemViewModel.DataValue != null &&
                toggleSound.MonitoredItemViewModel.DataValue.Value != null)
            {
                return Convert.ToBoolean(toggleSound.MonitoredItemViewModel.DataValue.Value);
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Overrides
#if !NET_STANDARD
        public override bool this[string propertyName]
        {
            get
            {
                if (propertyName == "Expression")
                {
                    return false;
                }
                else if (propertyName == "OpcuaEntityReference")
                {
                    return !IsAlarmStatisticCommandType();
                }
                else if (propertyName == "ReportType" || propertyName == "PeriodType" || propertyName == "ConnectionString" || propertyName == "ExecutionMode" || propertyName == "AlarmsSource" || propertyName == "MaxTake" || propertyName == "CommandTimeout")
                {
                    return IsAlarmStatisticCommandType();
                }
                else if (propertyName == "StartDate" || propertyName == "EndDate")
                {
                    return Type != AlarmCommandType.AckAll && Type != AlarmCommandType.ConfirmAll && Type != AlarmCommandType.ToggleSound && PeriodType == ReportPeriodType.Custom;
                }
                else if (propertyName == "Recipient")
                {
                    return Type == AlarmCommandType.SendStatisticReport;
                }
                else if (propertyName == "FolderPath")
                {
                    return Type == AlarmCommandType.SaveStatisticReport;
                }
                else if (propertyName == "FileName" || propertyName == "FileType")
                {
                    return Type == AlarmCommandType.SaveStatisticReport || Type == AlarmCommandType.SendStatisticReport;
                }
                else if (propertyName == "From" || propertyName == "FromAlias" || propertyName == "MailSubject" || propertyName == "MailObject")
                {
                    return Type == AlarmCommandType.SendStatisticReport;
                }
                else if (propertyName == "Printer")
                {
                    return false;
                }

                return base[propertyName];
            }
        }
#endif

        public override String CommandSummary
        {
            get
            {
                StringBuilder commandSummary = new StringBuilder(Type.ToString());

                if (OpcuaEntityReference != null && OpcuaEntityReference.IsValid && OpcuaEntityReference.ReadablePath != null && OpcuaEntityReference.AppName != null)
                {
                    Opc.Ua.NamespaceTable n = new Opc.Ua.NamespaceTable();
                    UInt16 ns = (UInt16)(n.Count + 2 - 1);
                    string oldChars = string.Format("{0}:", ns);
                    commandSummary.Append(string.Format(" {0} ({1})", (OpcuaEntityReference.ReadablePath).Replace(oldChars, ""), OpcuaEntityReference.AppName));
                }
                return commandSummary.ToString();
            }
        }

        public override String Name
        {
            get
            {
                return Properties.Resources.AlarmName;
            }
        }

        public override bool Init(IEntityReference entity, IDocument parent, String sessionname)
        {
            sessionname = String.Format("{0}-{1}", Thread.CurrentThread.ManagedThreadId, sessionname);

            base.Init(entity, parent, sessionname);

            var editor = Parent?.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (editor != null)
            {
                try
                {
                    var opcString = editor.GetNodeIdEntityReference(Parent,
                        String.Format("{0}/{1}", UFUAServerInfo.BrowserNames.AlarmsName,
                        UFUAServerInfo.BrowserNames.AlarmsSoundStateName),
                        UFUAServerInfo.Guids.SystemTagsGuid.ToString());
                    //"Alarms/SoundState", "60d54358-8bf6-485e-aaa4-c5c4b105b7b7");
                    //"AlarmSoundState", "{ns=2;s=60d54358-8bf6-485e-aaa4-c5c4b105b7b7?AlarmsSoundState}", null);
                    toggleSound = opcString.FromXml<OPCUAEntityReference>();
                }
                catch (Exception ex)
                {

                }

                if (OpcuaEntityReference == null)
                {
                    var serverXML = editor.GetServerEntityReference(Parent);
                    if (serverXML != null)
                        OpcuaEntityReference = serverXML.FromXml<OPCUAEntityReference>();
                }
            }

            if (OpcuaEntityReference != null)
            {
                OpcuaEntityReference.Resolve(SessionName, parent);
                OpcuaEntityReference.SetInUse(Entity, true);
            }

            if (toggleSound != null)
            {
                toggleSound.Resolve(SessionName);
                toggleSound.SetInUse(Entity, true);
            }

            return OpcuaEntityReference != null;
        }

        public override bool CanExecute()
        {
            if (!base.CanExecute())
                return false;

            if (Type == AlarmCommandType.AckAll ||
                Type == AlarmCommandType.ConfirmAll ||
                Type == AlarmCommandType.ToggleSound)
                return OpcuaEntityReference != null && GetItemViewModel() != null;

            return true;
        }

#if !NET_STANDARD
        public override bool ExecuteOnTouchDown()
        {
            return false;
        }
#endif

        public override RemoteExecute RemoteExecute()
        {
            if (Type == AlarmCommandType.PrintStatisticReport ||
                Type == AlarmCommandType.SaveStatisticReport ||
                Type == AlarmCommandType.SendStatisticReport ||
                Type == AlarmCommandType.ShowStatisticReport ||
                Type == AlarmCommandType.ToggleSound)
            {
                return new RemoteExecute() { ex = new Exception(String.Format(Properties.Resources.RemoteCommandNotSupported, CommandSummary)) };
            }

            Execute();
            return null;
        }

        public override void BlindExecute()
        {
            if (
#if !NET_STANDARD
                IsAccessDenied() || 
#endif
                !CanExecute()
#if !NET_STANDARD
                || !CanExecuteDelayCommand()
#endif
                )
                return;

            if (Type != AlarmCommandType.ShowStatisticReport)
            {
                Execute();
            }
#if !NET_STANDARD
            else
            {
                ExecuteOnUserInterface();
            }
#endif
        }

        public override void Execute()
        {
            if (
#if !NET_STANDARD
                IsAccessDenied() || 
#endif
                !CanExecute()
#if !NET_STANDARD
                || !CanExecuteDelayCommand()
#endif
                )
                return;

            if (Type == AlarmCommandType.AckAll)
            {
                var model = GetItemViewModel();
                if (model == null)
                    return;
                if (model.ConditionAcknowledgeAllCommand.CanExecute(null) &&
                    model.GetSubscriptionViewModelParent().GetSessionViewModelParent().Connected == true)
                {
                    model.ConditionAcknowledgeAllCommand.Execute(null);
                }
            }
            else if (Type == AlarmCommandType.ConfirmAll)
            {
                var model = GetItemViewModel();
                if (model == null)
                    return;
                if (model.ConditionConfirmAllCommand.CanExecute(null) &&
                    model.GetSubscriptionViewModelParent().GetSessionViewModelParent().Connected == true)
                {
                    model.ConditionConfirmAllCommand.Execute(null);
                }
            }
            else if (Type == AlarmCommandType.ToggleSound)
            {
                if (toggleSound != null)
                {
                    bool val = GetEReloadValue();
                    toggleSound.MonitoredItemViewModel.WriteValue(!val);
                }
            }
            else if (IsAlarmStatisticCommandType())
            {
                var parameters = new List<ReportParameters.Parameter>();
                DateTime dateFrom = DateTime.MinValue;
                DateTime dateTo = DateTime.MinValue;
                switch (PeriodType)
                {
                    case ReportPeriodType.Today:
                        dateFrom = DateTime.Today;
                        dateTo = dateFrom.AddDays(1).AddSeconds(-1);
                        break;
                    case ReportPeriodType.Yesterday:
                        dateFrom = DateTime.Today.AddDays(-1);
                        dateTo = dateFrom.AddDays(2).AddSeconds(-1);
                        break;
                    case ReportPeriodType.LastWeek:
                        dateFrom = DateTime.Today.AddDays(-7);
                        dateTo = dateFrom.AddDays(8).AddSeconds(-1);
                        break;
                    case ReportPeriodType.LastMonth:
                        dateFrom = DateTime.Today.AddMonths(-1);
                        dateTo = dateFrom.AddMonths(1).AddDays(1).AddSeconds(-1);
                        break;
                    case ReportPeriodType.LastYear:
                        dateFrom = DateTime.Today.AddYears(-1);
                        dateTo = dateFrom.AddYears(1).AddDays(1).AddSeconds(-1);
                        break;
                    default:
                        dateFrom = StartDate;
                        dateTo = EndDate;
                        break;
                }

                parameters.Add(new ReportParameters.Parameter() { Name = ReportAlarms.ParameterNames.DateFrom, Type = ReportParameters.ParameterType.DateTime, Value = dateFrom });
                parameters.Add(new ReportParameters.Parameter() { Name = ReportAlarms.ParameterNames.DateTo, Type = ReportParameters.ParameterType.DateTime, Value = dateTo });
                parameters.Add(new ReportParameters.Parameter() { Name = ReportAlarms.ParameterNames.Period, Type = ReportParameters.ParameterType.Integer, Value = (int)PeriodType });
                parameters.Add(new ReportParameters.Parameter() { Name = ReportAlarms.ParameterNames.Source, Type = ReportParameters.ParameterType.String, Value = AlarmsSource });

                var context = new Dictionary<String, Object>();
                context.Add("Parameters", parameters);

                if (MaxTake > 0)
                    context.Add("MaxTake", (int)MaxTake);
                if (CommandTimeout > 0)
                    context.Add("CommandTimeout", CommandTimeout);

                string connection = ConnectionString;
                if (string.IsNullOrEmpty(connection))
                {
                    IUFUAEditorManager editorManager = Parent.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                    if (editorManager != null)
                    {
                        connection = editorManager.GetEventDefaultConnection(Parent);
                        connection = RealTimeConnectionManagerViewModel.ReplaceServerRenamedOnDataSource(connection, SessionName);
                    }
                }
                if (!string.IsNullOrEmpty(connection))
                    context.Add("Connection", connection);

                if (Type == AlarmCommandType.SaveStatisticReport ||
                    Type == AlarmCommandType.SendStatisticReport)
                {
                    context.Add("FileType", FileType);

                    if (string.IsNullOrEmpty(FolderPath) || !DirectoryHelper.IsValidDirectory(FolderPath, false))
                        context.Add("Folder", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                    else
                        context.Add("Folder", FolderPath);

                    string filename = FileName;
                    if (String.IsNullOrEmpty(filename))
                        filename = String.Format("{0}_{1}", ReportType.ToString(), DateTime.Now);
                    filename = String.Format("{0}_{1}_{2}", filename, dateFrom, dateTo);
                    foreach (var c in System.IO.Path.GetInvalidFileNameChars())
                        filename = filename.Replace(new String(c, 1), "");
                    context.Add("FileName", string.Format("{0}.{1}", filename, FileType));
                }

                switch (Type)
                {
                    case AlarmCommandType.PrintStatisticReport:
                        {
                            context.Add("CommandType", ReportCommandType.Print);
                            break;
                        }
                    case AlarmCommandType.SaveStatisticReport:
                        {
                            context.Add("CommandType", ReportCommandType.Save);
                            break;
                        }
                    case AlarmCommandType.SendStatisticReport:
                        {
                            context.Add("CommandType", ReportCommandType.Send);
                            IUFUserEditorManager userManager = Parent.GetService(typeof(IUFUserEditorManager)) as IUFUserEditorManager;
                            if (userManager != null)
                            {
                                context.Add("SMTPSettings", userManager.GetSMTPSettings(Parent));
                                if (!string.IsNullOrEmpty(Recipient) && Recipient.Split(':').Count() > 1)
                                    context.Add("Recipient", userManager.GetUsersEmail(Parent, Recipient.Split(':')[1]));
                            }

                            context.Add("From", From);
                            context.Add("FromAlias", FromAlias);
                            context.Add("MailSubject", MailSubject);
                            context.Add("MailObject", MailObject);
                            break;
                        }
                    case AlarmCommandType.ShowStatisticReport:
                        {
                            context.Add("CommandType", ReportCommandType.Show);
                            break;
                        }
                }

                var report = new ReportAlarms.ReportAlarms(ReportType, Parent, renameFilePath: true);
                context.Add("ReportAlarmDoc", report.GetReportDocument());
                context.Add("Entity", Entity);
                IDocumentManager manager = Parent.GetService(typeof(IReportManager)) as IDocumentManager;
                if (manager != null)
                    manager.Execute(null, Parent, ExecutionMode, context);
            }
        }

        public override void Terminate()
        {
            if (OpcuaEntityReference != null)
            {
                OpcuaEntityReference.SetInUse(Entity, false);
                OpcuaEntityReference = null;
            }

            if (toggleSound != null)
            {
                toggleSound.SetInUse(Entity, false);
                toggleSound = null;
            }

            base.Terminate();
        }
        #endregion

        #region Methods
        bool IsAlarmStatisticCommandType()
        {
            return Type == AlarmCommandType.PrintStatisticReport ||
                Type == AlarmCommandType.SaveStatisticReport ||
                Type == AlarmCommandType.SendStatisticReport ||
                Type == AlarmCommandType.ShowStatisticReport;
        }
        #endregion
    }
}
