using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataValidation
{
    public class RowEventArg : EventArgs
    {
        #region Constructors
        public RowEventArg(string message) : 
            this(message, 0, 0)
        { }

        public RowEventArg(int current, int total) : 
            this(null, current, total)
        { }

        public RowEventArg(string message, int current, int total)
        {
            Message = message;
            ProcessedRows = current;
            TotalRows = total;
        }
        #endregion

        #region Properties
        public string Message { get; private set; }
        public int ProcessedRows { get; private set; }
        public int TotalRows { get; private set; }
        #endregion
    }
}
