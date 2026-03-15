using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDatabaseConfiguration
{
    public enum CommandExecution
    {
        None,
        ExecuteAggregation,
        RemoveAggregation,
        UpdateAggregation,
        ExecutePartition,
        RemovePartition,
        UpdatePartition,
        ParseProjectForAggregation,
        ParseProjectForPartition
    }
}
