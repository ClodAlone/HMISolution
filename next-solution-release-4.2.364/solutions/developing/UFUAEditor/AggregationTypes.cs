using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFUAEditor
{
    internal class AggregationTypes
    {
        public enum OperationType : int
        {
            None,
            AggregatesTables,
            PatitionTables
        }

        public enum CommandType : int
        {
            None,
            Add,
            Remove,
            Check,
            Update
        }
    }
}
