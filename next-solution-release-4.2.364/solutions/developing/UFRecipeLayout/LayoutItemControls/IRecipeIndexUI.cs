using System;
using System.Text;

namespace UFRecipeLayout.LayoutItemControls
{
    public interface IRecipeIndexUI
    {
        void SetBinding(String itemsourcepath, String displaypath, String valuepath);
        event EventHandler<RecipeTextChangedEventArgs> RecipeTextChanged;
        event EventHandler<RecipeSelectionEventArgs> RecipeSelectionChanged;
        String TextValue { get; set; }
        Guid SelectedRecipeId { get; set; }
        bool AllowEdit { get; set; }
        int MaxLength { get; set; }
    }
}
