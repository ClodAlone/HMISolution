using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFRecipeLayout
{
    public class RecipeTextChangedEventArgs : EventArgs
    {
        public String RecipeName { get; internal set; }
    }
}
