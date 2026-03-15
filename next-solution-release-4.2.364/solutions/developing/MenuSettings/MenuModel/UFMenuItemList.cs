using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using UFInterfaces.Constants;

namespace MenuSettings.MenuModel
{
    [CollectionDataContract(Name = "UFMenuItemsList", ItemName = "entry", Namespace = Namespaces.UriProgea)]
    public class UFMenuItemList : List<UFMenuItemEntity>
    {
    }
}
