using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if !NET_STANDARD
using System.Windows;
using CommandManager.ComponentService;
using Utilities.WPF;
#endif
using DocumentManager.ComponentService;
using ReportManager.ComponentService;
using System.Runtime.Serialization;
using Utilities;
using OPCUAViewModel;
using System.Threading;
using UFInterfaces;
using System.IO;
using System.ComponentModel;
using UFUAEditor.ComponentService;
using UFUserEditor.ComponentService;

namespace CommandManager
{
    [DataContract(Name = "ReportCommand", Namespace = UFInterfaces.Constants.Namespaces.UriProgea)]
    public class ReportCommand : CommandManager
    {
        #region Properties

        [DataMember]
        Uri reportName;
        public Uri ReportName
        {
            get { return reportName; }
            set
            {
                if (value == reportName)
                    return;
                reportName = value;
#if !NET_STANDARD
                OnPropertyChanged("ReportName");
                OnPropertyChanged("CommandSummary");
#endif
            }
        }

        [DataMember]
        ReportCommandType commandType;
        public ReportCommandType CommandType
        {
            get
            {
                return commandType;
            }
            set
            {
                if (commandType == value)
                    return;
                commandType = value;
#if !NET_STANDARD
                OnPropertyChanged("CommandType");
                OnPropertyVisiblityChanged("CommandType");
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

        [DataMember]
        ReportParameters.ParameterCollection parameters;
        public ReportParameters.ParameterCollection Parameters
        {
            get
            {
                return parameters;
            }
            set
            {
                if (parameters == null)
                    parameters = new ReportParameters.ParameterCollection();

                if (parameters != value)
                {
                    parameters.Dispose();

                    parameters = value;
#if !NET_STANDARD
                    OnPropertyChanged("Parameters");
#endif
                }
            }
        }

        public List<string> ParametersXmlReferences
        {
            get
            {
                List<string> list = new List<string>();
                if (Parameters == null) return list;
                list.AddRange((from p in Parameters.AsParallel() select p.TagRefXml).ToList());
                return list;
            }
        }

        #endregion

        #region Overrides
        [Browsable(false)]
        public override List<OPCUAEntityReference> ListTags
        {
            get
            {
                var ret = base.ListTags;
                if (Parameters != null)
                    foreach (var par in Parameters)
                    {
                        if (par.TagRef != null)
                            ret.Add(par.TagRef);
                    }
                return ret;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public override void UpdateTags(OPCUAEntityReference source, OPCUAEntityReference dest)
        {
            base.UpdateTags(source, dest);

            if (Parameters != null)
                foreach (var par in Parameters)
                {
                    if (par.TagRef != null && par.TagRef.HumanReadableNoProject == source.HumanReadableNoProject)
                        par.TagRef = dest;
                }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public override Dictionary<OPCUAEntityReference, String> TagsMap
        {
            get
            {
                var ret = base.TagsMap;
                if (Parameters != null)
                    foreach (var par in Parameters)
                    {
                        if (par.TagRef != null)
                            ret.Add(par.TagRef, par.Name);
                    }
                return ret;
            }
        }
#if !NET_STANDARD
        public override bool this[string propertyName]
        {
            get
            {
                if (propertyName == "OpcuaEntityReference" || propertyName == "Expression")
                {
                    return false;
                }
                if (propertyName == "Recipient")
                {
                    return CommandType == ReportCommandType.Send;
                }
                if (propertyName == "FolderPath")
                {
                    return CommandType == ReportCommandType.Save;
                }
                if (propertyName == "FileName" || propertyName == "FileType")
                {
                    return CommandType == ReportCommandType.Save || CommandType == ReportCommandType.Send;
                }
                if (propertyName == "From" || propertyName == "FromAlias" || propertyName == "MailSubject" || propertyName == "MailObject")
                {
                    return CommandType == ReportCommandType.Send;
                }

                return base[propertyName];
            }
        }

        public override bool ExecuteOnTouchDown()
        {
            return false;
        }
#endif

        public override String CommandSummary
        {
            get
            {
                if (ReportName == null)
                    return String.Empty;
                return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(ReportName.GetPathString()), System.IO.Path.GetFileNameWithoutExtension(ReportName.GetPathString()));
            }
        }


        public override String Name
        {
            get
            {
                return Properties.Resources.ReportCommandName;
            }
        }

        public override bool Init(IEntityReference entity, IDocument parent, String sessionname)
        {
            var ret = base.Init(entity, parent, sessionname);
            if (Parameters != null)
                Parameters.PrepareExecution(SessionName, Entity, parent);

            return ret;
        }

        public override RemoteExecute RemoteExecute()
        {
            return new RemoteExecute() { ex = new Exception(String.Format(Properties.Resources.RemoteCommandNotSupported, CommandSummary)) };
        }

        public override bool IsUICommand()
        {
            return true;
        }

        bool bPendingExecution;
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

#if !NET_STANDARD
            if (CommandType != ReportCommandType.PrintDialog &&
                CommandType != ReportCommandType.Show)
#endif
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

            //if (!CommandManagerComponent.uriRisolverServiceAvailable)
            //    throw new NotImplementedException("Expecting the missing IUriResolver Interface");
            // IDocumentManager manager = CommandManagerComponent.uriRisolver.ResolveUri(reportName) as IDocumentManager;
            IDocumentManager manager = Parent?.GetService(typeof(IReportManager)) as IDocumentManager;
            if (manager != null)
            {
                var context = new Dictionary<String, Object>();
                context.Add("Entity", Entity);
                context.Add("CommandType", CommandType);
                if (CommandTimeout > 0)
                    context.Add("CommandTimeout", CommandTimeout);
                if (Parameters != null)
                    context.Add("Parameters", Parameters.ToList());
                string connection = ConnectionString;
                if (string.IsNullOrEmpty(connection))
                {
                    IUFUAEditorManager editorManager = Parent.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                    if (editorManager != null)
                    {
                        connection = editorManager.GetHistorianDefaultConnection(Parent);
                        connection = RealTimeConnectionManagerViewModel.ReplaceServerRenamedOnDataSource(connection, SessionName);
                    }
                }
                if (!string.IsNullOrEmpty(connection))
                    context.Add("Connection", connection);

                if (CommandType == ReportCommandType.Save ||
                    CommandType == ReportCommandType.Send)
                {
                    context.Add("FileType", FileType);

                    if (string.IsNullOrEmpty(FolderPath) || !DirectoryHelper.IsValidDirectory(FolderPath, false))
                        context.Add("Folder", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                    else
                        context.Add("Folder", FolderPath);

                    string filename = FileName;
                    if (String.IsNullOrEmpty(filename))
                        filename = String.Format("{0}_{1}", System.IO.Path.GetFileNameWithoutExtension(ReportName.GetPathString()), DateTime.Now);
                    foreach (var c in System.IO.Path.GetInvalidFileNameChars())
                        filename = filename.Replace(new String(c, 1), "");
                    context.Add("FileName", string.Format("{0}.{1}", filename, FileType));
                }

                if (CommandType == ReportCommandType.Send)
                {
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
                }

                manager.Execute(reportName, Parent, ExecutionMode, context);
            }
        }

        public override void Terminate()
        {
            if (Parameters != null)
            {
                Parameters.TerminateExecution(Entity);
                Parameters.Dispose();
            }

            base.Terminate();
        }

        public override bool CanExecute()
        {
            if (!base.CanExecute())
                return false;

            return Parameters == null || Parameters.IsReady;
        }
#endregion
    }
}
