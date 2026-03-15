using DocumentManager.ComponentService;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using UFInterfaces;
using UIMsgBoxAlertService.ComponentService;
using Utilities;
using Utilities.Logger;

namespace CommandManager.Executer
{
    public class CommandsExecuter
    {
        #region Declarations
        readonly CommandManagerList commandList;
        readonly IDocument parent;
        readonly IEntityReference entity;

        static readonly ILog logCommands = Logger.GetDestinationLog(LoggerDestination.CommandManager);

        bool bPreparedExecution;
        #endregion

        #region Constructors
        public CommandsExecuter(CommandManagerList commandList, IDocument parent)
        {
            this.commandList = commandList;
            this.parent = parent;

            entity = new EntityWeakReference();
        }
        #endregion

        #region Properties
        public bool CanExecute
        {
            get
            {
                if (!bPreparedExecution)
                    return false;

                bool bCanExecute = true;
                commandList.ForEach(command => 
                {
                    if (!command.CanExecute())
                    {
                        bCanExecute = false;
                        return;
                    }
                });

                return bCanExecute;
            }
        }

        IUIMsgBoxAlertService uiInterface;
        IUIMsgBoxAlertService UIInterface
        {
            get
            {
                if (uiInterface == null)
                    uiInterface = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                return uiInterface;
            }
        }
        #endregion

        #region Static Methods
        public static CommandsExecuter CreateFromXaml(string xaml, IDocument parent)
        {
            if (!String.IsNullOrEmpty(xaml))
            {
                try
                {
                    var commandList = xaml.FromXml<CommandManagerList>();
                    return new CommandsExecuter(commandList, parent);
                }
                catch
                { }
            }

            return null;
        }
        #endregion

        #region Public Methods
        public void PrepareExecution(string sessionName)
        {
            if (bPreparedExecution)
                return;
            bPreparedExecution = true;

            commandList.ForEach(command => command.Init(entity, parent, sessionName));
        }

        public void TerminateExecution()
        {
            if (!bPreparedExecution)
                return;
            bPreparedExecution = false;

            commandList.ForEach(command => command.Terminate());
        }

        public void Execute()
        {
            if (bPreparedExecution)
            {
                bool bError = false;
                commandList.ForEach(command =>
                {
                    try
                    {
                        command.BlindExecute();
                    }
                    catch (Exception ex)
                    {
                        bError = true;
                        logCommands.ErrorFormat(Properties.Resources.ErrorExecutingCommand, command.Name, ex.Message);
                    }
                });

                if (bError)
                {
                    if (UIInterface != null)
                        UIInterface.ShowError(Properties.Resources.CommandsNotAllExecuted);
                }
            }
        }

        public void RemoteExecute()
        {
            if (bPreparedExecution)
            {
                commandList.ForEach(command => 
                {
                    var remoteExecute = command.RemoteExecute();
                    if (remoteExecute.ex != null)
                        logCommands.ErrorFormat(Properties.Resources.ErrorExecutingCommand, command.Name, remoteExecute.ex.Message);
                });
            }
        }
        #endregion
    }
}
