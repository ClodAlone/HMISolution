using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using UFInterfaces.Constants;

namespace UFShortcutSettings.ShortcutModel
{
    [CollectionDataContract(Name = "UFKeyCommandsList", ItemName = "entry", Namespace = Namespaces.UriProgea)]
    public class UFKeyCommandList : List<UFKeyCommandEntity> { }
}
