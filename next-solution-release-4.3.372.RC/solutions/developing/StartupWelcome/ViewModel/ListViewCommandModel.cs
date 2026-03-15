using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StartupWelcome.ViewModel
{
    public class ListViewCommandModel
    {
        private int _selectedCommand;
        /// <summary>
        /// Gets or sets the selected Command.
        /// </summary>
        /// <value>The selected Command.</value>
        public int SelectedCommand
        {
            get { return _selectedCommand; }
            set { _selectedCommand = value; }
        }
        private string _commandTooltip = string.Empty;
        /// <summary>
        /// Gets or sets the selected CommandTooltip.
        /// </summary>
        /// <value>The selected CommandTooltip.</value>
        public string CommandTooltip
        {
            get { return _commandTooltip; }
            set { _commandTooltip = value; }
        }
        private string _commandName = string.Empty;
        /// <summary>
        /// Gets or sets the selected CommandTooltip.
        /// </summary>
        /// <value>The selected CommandTooltip.</value>
        public string CommandName
        {
            get { return _commandName; }
            set { _commandName = value; }
        }
    }
}
