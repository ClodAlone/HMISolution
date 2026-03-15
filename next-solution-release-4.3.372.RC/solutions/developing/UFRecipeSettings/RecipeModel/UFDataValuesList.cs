using System.Collections.Generic;
using System.Runtime.Serialization;
using UFInterfaces.Constants;

namespace UFRecipeSettings.UFRecipeModel
{
    [CollectionDataContract(Name = "UFDataValuesList", ItemName = "entry", Namespace = Namespaces.UriProgea)]
    public class UFDataValuesList : List<UFDataValueEntity> { }
}
