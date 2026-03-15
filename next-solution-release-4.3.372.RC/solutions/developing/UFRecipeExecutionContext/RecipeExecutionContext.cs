using System;
using System.Collections.Generic;
using System.Threading;
#if !NET_STANDARD
using System.Windows.Controls;
#endif

namespace UFRecipeExecutionContext
{
    public class RecipeExecutionContext
    {
        public RecipeCommandType CommandType { get; set; }
        public string Index { get; set; }
        public string FilePathName { get; set; }
        public int Timeout { get; set; }
        public Dictionary<Guid, Opc.Ua.Variant> Values = new Dictionary<Guid, Opc.Ua.Variant>();
        public bool IsSynchro { get; set; }
        public bool IsAuditTrace { get; set; }
        public ManualResetEvent IsExecuted { get; set; }
        public string UserName { get; set; }
        public string UserComment { get; set; }
        public Exception Exception { get; set; }
#if !NET_STANDARD
        public object Control { get; set; }
#endif
    }
}
