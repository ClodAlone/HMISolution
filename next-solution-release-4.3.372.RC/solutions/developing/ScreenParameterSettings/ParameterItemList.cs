using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using UFInterfaces.Constants;

namespace ScreenParameterSettings
{
    [CollectionDataContract(Name = "ParameterItemsList", ItemName = "entry", Namespace = Namespaces.UriProgea)]
    public class ParameterItemList : List<ParameterItem>
    {
    }
}
