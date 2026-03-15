using CommandManager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFInterfaces.Commandable;
using Utilities;
using System.Windows;
using MSModel;

namespace MSEditor.ActionCommands
{
    public class ActionCommandsEditObject : CommandManager.Hepers.CommandsEditObject
    {
        #region Declarations
        readonly MSModel.MSScheduledAction scheduledAction;
        readonly ActionCommandsEventType commandstype;
        #endregion

        #region Constructors
        public ActionCommandsEditObject(MSModel.MSScheduledAction scheduledAction, ActionCommandsEventType commandstype) : 
            base(new CommandManagerList())
        {
            this.scheduledAction = scheduledAction;
            this.commandstype = commandstype;

            PrepareCommands();
        }
        #endregion

        #region Override Methods
        protected override void ApplyChanges()
        {
            PropagateChanges();
        }
        #endregion

        #region Methods
        void PrepareCommands()
        {
            string commands = null;
            var friendlyName = new StringBuilder(scheduledAction.Name);
            if (commandstype == ActionCommandsEventType.CommandsOn)
            {
                friendlyName.AppendFormat(" - {0}", Properties.Resources.ScheduledActionCommandsOn);
                commands = scheduledAction.CommandsOn;
            }
            else if (commandstype == ActionCommandsEventType.CommandsOff)
            {
                friendlyName.AppendFormat(" - {0}", Properties.Resources.ScheduledActionCommandsOff);
                commands = scheduledAction.CommandsOff;
            }
            else if (commandstype == ActionCommandsEventType.CommandsOnEx)
            {
                friendlyName.AppendFormat(" - {0}", Properties.Resources.ScheduledActionExceptionCommandsOn);
                commands = scheduledAction.ExceptionCommandsOn;
            }
            else if (commandstype == ActionCommandsEventType.CommandsOffEx)
            {
                friendlyName.AppendFormat(" - {0}", Properties.Resources.ScheduledActionExceptionCommandsOff);
                commands = scheduledAction.ExceptionCommandsOff;
            }

            Name = friendlyName.ToString();

            try
            {
                if (!String.IsNullOrEmpty(commands))
                    CommandList = commands.FromXml<CommandManagerList>();
            }
            catch
            { }
        }

        void PropagateChanges()
        {
            string commands = null;
            var commandList = CommandList as CommandManagerList;
            if (commandList.Count > 0)
                commands = commandList.ToXml();
            else
                commands = null;

            if (commandstype == ActionCommandsEventType.CommandsOn)
                scheduledAction.CommandsOn = commands;
            else if (commandstype == ActionCommandsEventType.CommandsOff)
                scheduledAction.CommandsOff = commands;
            else if (commandstype == ActionCommandsEventType.CommandsOnEx)
                scheduledAction.ExceptionCommandsOn = commands;
            else if (commandstype == ActionCommandsEventType.CommandsOffEx)
                scheduledAction.ExceptionCommandsOff = commands;
        }
        #endregion
    }
}
