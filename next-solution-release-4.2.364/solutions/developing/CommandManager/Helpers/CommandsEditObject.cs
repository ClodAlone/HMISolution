using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFInterfaces.Commandable;

namespace CommandManager.Hepers
{
    public class CommandsEditObject : ICommandable
    {
        #region Declarations
        readonly CommandManagerList commandList;
        #endregion

        #region Constructors
        public CommandsEditObject(CommandManagerList commandList)
        {
            this.commandList = commandList;
        }
        #endregion

        #region Virtual Methods
        protected virtual void ApplyChanges()
        { }
        #endregion

        #region ICommandable Members
        string name;
        [Browsable(false)]
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name == value)
                    return;

                name = value;
            }
        }

        [Browsable(false)]
        public IEnumerable CommandList
        {
            get
            {
                return commandList;
            }
            set
            {
                commandList.Clear();
                foreach (var v in value)
                    commandList.Add(v as CommandManager);

                ApplyChanges();
            }
        }

        [Browsable(false)]
        public virtual bool WebHMISupported
        {
            get
            {
                return true;
            }
        }
        #endregion
    }
}
