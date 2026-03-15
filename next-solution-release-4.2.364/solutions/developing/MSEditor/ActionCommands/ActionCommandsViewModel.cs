using CommandManager.Hepers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSEditor.ActionCommands
{
    public class ActionCommandsViewModel : CommandsViewModel
    {
        #region Declarations
        MSModel.MSScheduledAction scheduledAction;
        ActionCommandsEventType commandstype;
        #endregion

        #region Constructors
        public ActionCommandsViewModel(MSModel.MSScheduledAction scheduledAction, ActionCommandsEventType commandstype)
        {
            this.scheduledAction = scheduledAction;
            this.commandstype = commandstype;

            if (commandstype == ActionCommandsEventType.CommandsOn)
                Value = scheduledAction.CommandsOn;
            else if (commandstype == ActionCommandsEventType.CommandsOff)
                Value = scheduledAction.CommandsOff;
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected override void OnPropertyChanged(string propertyName)
        {
            if (propertyName == "Value")
            {
                if (commandstype == ActionCommandsEventType.CommandsOn)
                    scheduledAction.CommandsOn = Value;
                else if (commandstype == ActionCommandsEventType.CommandsOff)
                    scheduledAction.CommandsOff = Value;
            }

            base.OnPropertyChanged(propertyName);
        }
        #endregion
    }
}
