using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UFRecipeExecuter.TransactionLog
{
    public class RecipeRedundancyInitializationContext
    {
        public string TransactionLogConnectionString { get; set; }
        public TimeSpan TransactionLogMaxAge { get; set; }
        public bool IsRedudancyEnabled { get; set; }
    }
}
