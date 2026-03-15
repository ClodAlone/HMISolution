using System;
using System.Diagnostics;
using UFRecipeExecutionContext;

namespace UFRecipeExecuter
{
    public enum AuditLogEntryType
    {
        Added,
        Deleted,
        Modified,
        Activated
    }

    public class AuditTraceArgs : EventArgs
    {
        public String RecipeIndex { get; set; }
        public String Details { get; set; }
        public String UserName { get; set; }
        public String UserComment { get; set; }
        public AuditLogEntryType LogEntryType { get; set; }
    }
}
