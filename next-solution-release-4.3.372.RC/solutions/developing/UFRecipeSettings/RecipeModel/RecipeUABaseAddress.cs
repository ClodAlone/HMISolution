using System;
using DevExpress.Xpo;

namespace UFRecipeSettings.UFRecipeModel
{
    // changed the table name for compatibility reason (see https://support.progea.com/Products/default.asp?9768)
    [Persistent("RecipeUABaseAddressEx")]
    public class RecipeUABaseAddress : UFUAModel.AddressBase
    {
        public RecipeUABaseAddress(Session session)
            : base(session)
        { }
    }
}
