using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using UFInterfaces.Constants;

namespace UFUAEditor.RemovedVariables
{
    [CollectionDataContract
            (Name = "CollectionVariableGuids",
            ItemName = "entry",
            KeyName = "TagName",
            ValueName = "NodeId",
            Namespace = Namespaces.UriProgea)]
    internal class CollectionVariableGuids : Dictionary<String, Guid>
    { }
}
