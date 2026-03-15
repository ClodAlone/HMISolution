using System;

namespace UFRecipeExecuter
{
    public class StateChangedArgs : EventArgs
    {
        public String recipeName { get; set; }
        public RecipeExecutionStateEnum oldState { get; set; }
        public RecipeExecutionStateEnum newState { get; set; }
    }
}
