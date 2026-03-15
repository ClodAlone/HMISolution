using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDatabaseConfiguration
{
    public enum ResultState
    {
        None,
        Running,
        Successfully,
        Error,
        Skipped
    }

    public class TraceMessageViewModel
    {
        #region Declarations
        readonly DateTime time;
        readonly string action;
        readonly string message;
        readonly ResultState result;
        #endregion

        #region Constructors
        public TraceMessageViewModel(String action, String message, ResultState result)
        {
            this.time = DateTime.Now;
            this.action = action;
            this.message = message;
            this.result = result;
        }
        #endregion

        #region Properties
        public DateTime Time
        {
            get
            {
                return time;
            }
        }
                
        public String Action
        {
            get
            {
                return action;
            }
        }

        public String Message
        {
            get
            {
                return message;
            }
        }

        public ResultState Result
        {
            get
            {
                return result;
            }
        }
        #endregion
    }
}
