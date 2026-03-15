using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using ViewModelLib;
using UFInterfaces.Constants;

namespace CommandManager
{
    [CollectionDataContract
                    (Name = "CommandManagerList",
                    ItemName = "entry",
                    Namespace = Namespaces.UriProgea)]
    [KnownType("GetKnownTypes")]
    public class CommandManagerList : List<CommandManager> 
    {
        static Type[] GetKnownTypes()
        {
            var list = CommandManager.LoadCommandTypes();
            var array = new List<Type>();
            foreach (var s in list)
                array.Add(CommandManager.GetCommandType(s));
            return array.ToArray();
        }

        public CommandManagerList(List<CommandManager> commandManagers)
            : base(commandManagers)
        {

        }

        public CommandManagerList()
        {

        }
    }
}
