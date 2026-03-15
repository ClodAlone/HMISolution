using CommandManager.Hepers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFUAEditor.Alarms
{
    public class AlarmCommandsViewModel : CommandsViewModel
    {
        #region Declarations
        UFUAModel.UFUAAlarmThreshold alarmThreshold;
        AlarmCommandsEventType commandstype;
        #endregion

        #region Constructors
        public AlarmCommandsViewModel(UFUAModel.UFUAAlarmThreshold alarmThreshold, AlarmCommandsEventType commandstype)
        {
            this.alarmThreshold = alarmThreshold;
            this.commandstype = commandstype;

            if (commandstype == AlarmCommandsEventType.CommandsOn)
                Value = alarmThreshold.CommandsOn;
            else if (commandstype == AlarmCommandsEventType.CommandsOff)
                Value = alarmThreshold.CommandsOff;
            else if (commandstype == AlarmCommandsEventType.CommandsAck)
                Value = alarmThreshold.CommandsAck;
            else if (commandstype == AlarmCommandsEventType.CommandsReset)
                Value = alarmThreshold.CommandsReset;
            else if (commandstype == AlarmCommandsEventType.CommandsDbClick)
                Value = alarmThreshold.CommandsDbClick;
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
                if (commandstype == AlarmCommandsEventType.CommandsOn)
                    alarmThreshold.CommandsOn = Value;
                else if (commandstype == AlarmCommandsEventType.CommandsOff)
                    alarmThreshold.CommandsOff = Value;
                else if (commandstype == AlarmCommandsEventType.CommandsAck)
                    alarmThreshold.CommandsAck = Value;
                else if (commandstype == AlarmCommandsEventType.CommandsReset)
                    alarmThreshold.CommandsReset = Value;
                else if (commandstype == AlarmCommandsEventType.CommandsDbClick)
                    alarmThreshold.CommandsDbClick = Value;
            }

            base.OnPropertyChanged(propertyName);
        }
        #endregion
    }
}
