using System;
using Opc.Ua;
using UFUAModel;

namespace DriverCodeBaseEx
{
    public class StateCommandVariableTag
    {              
        public static bool IsTagsSet(UFUAModel.TagEntityReference tag)
        {
            return (tag != null && !NodeId.IsNull(tag.NodeId));
        }

        public static bool IsTagsSet(string tagName, string tagNodeId)
        {
            return (!String.IsNullOrEmpty(tagName) && !String.IsNullOrEmpty(tagNodeId));
        }

        public static bool IsTagsEquals(TagEntityReference tag1, TagEntityReference tag2)
        {
            return (tag1 != null && !tag1.NodeId.IsNullNodeId && tag2 != null && !tag2.NodeId.IsNullNodeId && tag1.NodeId == tag2.NodeId);
        }
    }
}
