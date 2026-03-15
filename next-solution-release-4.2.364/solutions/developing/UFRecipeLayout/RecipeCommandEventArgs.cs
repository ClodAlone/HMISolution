using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UFRecipeLayout
{
    public class RecipeCommandEventArgs : EventArgs
    {
        public Guid RecipeID { get; internal set; }
        public EditCommandType CommandType { get; internal set; }
        public CancellationToken CancellationToken { get; internal set; }
        public Exception exception { get; set; }
    }
}
