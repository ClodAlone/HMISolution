using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentManager.ComponentService;
using UFInterfaces;
using UFUAEditor.ComponentService;
using System.Windows.Threading;
using Utilities;
using CommandManager.Executer;
using MSModel;

namespace MSEditor.ActionCommands
{
    public class ActionCommandsExecuter
    {
        #region Declarations
        readonly Dictionary<ActionCommandsEventType, CommandsExecuter> commandsExecuter = new Dictionary<ActionCommandsEventType, CommandsExecuter>();
        #endregion

        #region Constructors
        public ActionCommandsExecuter(MSModel.MSScheduledAction scheduledAction, IDocument document)
        {
            Scheduler = scheduledAction.FullName;
            ExecOnAtStartup = (scheduledAction.Type == ScheduleType.calendar || scheduledAction.Type == ScheduleType.weeklyPlan) ?
                scheduledAction.ExecOnAtStartup.Value : false;
            ExecOffAtStartup = (scheduledAction.Type == ScheduleType.calendar || scheduledAction.Type == ScheduleType.weeklyPlan) ? 
                scheduledAction.ExecOffAtStartup.Value : false;
            var executer = CommandsExecuter.CreateFromXaml(scheduledAction.CommandsOn, document);
            if (executer != null)
                commandsExecuter.Add(ActionCommandsEventType.CommandsOn, executer);

            executer = CommandsExecuter.CreateFromXaml(scheduledAction.CommandsOff, document);
            if (executer != null)
                commandsExecuter.Add(ActionCommandsEventType.CommandsOff, executer);

            executer = CommandsExecuter.CreateFromXaml(scheduledAction.ExceptionCommandsOn, document);
            if (executer != null)
                commandsExecuter.Add(ActionCommandsEventType.CommandsOnEx, executer);

            executer = CommandsExecuter.CreateFromXaml(scheduledAction.ExceptionCommandsOff, document);
            if (executer != null)
                commandsExecuter.Add(ActionCommandsEventType.CommandsOffEx, executer);
        }
        #endregion

        #region Public Properties
        public bool IsEmpty
        {
            get
            {
                return commandsExecuter.Count == 0;
            }
        }

        public string Scheduler { get; private set; }

        public bool ExecOnAtStartup { get; private set; }
        public bool ExecOffAtStartup { get; private set; }
        #endregion

        #region Public Methods
        public void PrepareExecution(string sessionName)
        {
            commandsExecuter.Values.ToList().ForEach((executer) => executer.PrepareExecution(sessionName));
        }

        public void TerminateExecution()
        {
            commandsExecuter.Values.ToList().ForEach((executer) => executer.TerminateExecution());
        }

        public void Execute(ActionCommandsEventType type)
        {
            if (commandsExecuter.ContainsKey(type))
                commandsExecuter[type].Execute();
        }

        public void RemoteExecute(ActionCommandsEventType type)
        {
            if (commandsExecuter.ContainsKey(type))
                commandsExecuter[type].RemoteExecute();
        }
        #endregion
    }
}
