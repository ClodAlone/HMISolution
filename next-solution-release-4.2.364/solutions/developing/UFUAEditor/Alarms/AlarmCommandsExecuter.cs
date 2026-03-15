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

namespace UFUAEditor.Alarms
{
    public class AlarmCommandsExecuter
    {
        #region Declarations
        readonly Dictionary<AlarmCommandsEventType, CommandsExecuter> commandsExecuter = new Dictionary<AlarmCommandsEventType, CommandsExecuter>();
        #endregion

        #region Constructors
        public AlarmCommandsExecuter(UFUAModel.UFUAAlarmThreshold alarmThreshold, IDocument document)
        {
            var executer = CommandsExecuter.CreateFromXaml(alarmThreshold.CommandsOn, document);
            if (executer != null)
                commandsExecuter.Add(AlarmCommandsEventType.CommandsOn, executer);

            executer = CommandsExecuter.CreateFromXaml(alarmThreshold.CommandsOff, document);
            if (executer != null)
                commandsExecuter.Add(AlarmCommandsEventType.CommandsOff, executer);

            executer = CommandsExecuter.CreateFromXaml(alarmThreshold.CommandsAck, document);
            if (executer != null)
                commandsExecuter.Add(AlarmCommandsEventType.CommandsAck, executer);

            executer = CommandsExecuter.CreateFromXaml(alarmThreshold.CommandsReset, document);
            if (executer != null)
                commandsExecuter.Add(AlarmCommandsEventType.CommandsReset, executer);
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

        public void Execute(AlarmCommandsEventType type)
        {
            if (commandsExecuter.ContainsKey(type))
                commandsExecuter[type].Execute();
        }

        public void RemoteExecute(AlarmCommandsEventType type)
        {
            if (commandsExecuter.ContainsKey(type))
                commandsExecuter[type].RemoteExecute();
        }
        #endregion
    }
}
