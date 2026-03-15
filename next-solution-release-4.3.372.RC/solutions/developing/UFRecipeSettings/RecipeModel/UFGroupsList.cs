using System.Collections.Generic;
using System.Runtime.Serialization;
using UFInterfaces.Constants;

namespace UFRecipeSettings.UFRecipeModel
{
    [CollectionDataContract(Name = "UFGroupsList", ItemName = "entry", Namespace = Namespaces.UriProgea)]
    public class UFGroupsList : List<UFGroupEntity> { }
}
