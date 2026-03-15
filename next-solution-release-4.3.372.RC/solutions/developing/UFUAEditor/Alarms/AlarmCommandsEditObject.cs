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

namespace UFUAEditor.Alarms
{
    public class AlarmCommandsEditObject : CommandManager.Hepers.CommandsEditObject
    {
        #region Declarations
        readonly UFUAModel.UFUAAlarmThreshold alarmThreshold;
        readonly AlarmCommandsEventType commandstype;
        #endregion

        #region Constructors
        public AlarmCommandsEditObject(UFUAModel.UFUAAlarmThreshold alarmThreshold, AlarmCommandsEventType commandstype) : 
            base(new CommandManagerList())
        {
            this.alarmThreshold = alarmThreshold;
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
            var friendlyName = new StringBuilder(alarmThreshold.Name);
            if (commandstype == AlarmCommandsEventType.CommandsOn)
            {
                friendlyName.AppendFormat(" - {0}", Properties.Resources.UFUAAlarmThresholdCommandsOn);
                commands = alarmThreshold.CommandsOn;
            }
            else if (commandstype == AlarmCommandsEventType.CommandsOff)
            {
                friendlyName.AppendFormat(" - {0}", Properties.Resources.UFUAAlarmThresholdCommandsOff);
                commands = alarmThreshold.CommandsOff;
            }
            else if (commandstype == AlarmCommandsEventType.CommandsAck)
            {
                friendlyName.AppendFormat(" - {0}", Properties.Resources.UFUAAlarmThresholdCommandsAck);
                commands = alarmThreshold.CommandsAck;
            }
            else if (commandstype == AlarmCommandsEventType.CommandsReset)
            {
                friendlyName.AppendFormat(" - {0}", Properties.Resources.UFUAAlarmThresholdCommandsReset);
                commands = alarmThreshold.CommandsReset;
            }
            else if (commandstype == AlarmCommandsEventType.CommandsDbClick)
            {
                friendlyName.AppendFormat(" - {0}", Properties.Resources.UFUAAlarmThresholdCommandsDbClick);
                commands = alarmThreshold.CommandsDbClick;
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

            if (commandstype == AlarmCommandsEventType.CommandsOn)
                alarmThreshold.CommandsOn = commands;
            else if (commandstype == AlarmCommandsEventType.CommandsOff)
                alarmThreshold.CommandsOff = commands;
            else if (commandstype == AlarmCommandsEventType.CommandsAck)
                alarmThreshold.CommandsAck = commands;
            else if (commandstype == AlarmCommandsEventType.CommandsReset)
                alarmThreshold.CommandsReset = commands;
            else if (commandstype == AlarmCommandsEventType.CommandsDbClick)
                alarmThreshold.CommandsDbClick = commands;
        }
        #endregion

        #region Override Property
        [Browsable(false)]
        public override bool WebHMISupported
        {
            get
            {
                return false;
            }
        }
        #endregion
    }
}
