using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFRecipeLayout
{
    public class RecipeSelectionEventArgs : EventArgs
    {
        public Guid RecipeID { get; internal set; }
        public bool Cancel { get; set; }
    }
}
