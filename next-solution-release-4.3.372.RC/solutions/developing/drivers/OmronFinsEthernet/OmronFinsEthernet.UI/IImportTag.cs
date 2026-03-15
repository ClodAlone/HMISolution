using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace S7TCP.UI
{
    public interface IImportTag
    {
        string Name { get; }
        string Address { get; }
        IEnumerable<IImportTag> ImportTags { get; }
        int Id { get; set; }
        int parentId { get; set; }
        bool IsSelected { get; set; }

        string szDescription { get; set; }
        Opc.Ua.NodeId nType { get; set; }
        string szType { get; set; }
        uint nSize { get; set; }
        Opc.Ua.NodeId nElemType { get; set; }
        string szPreName { get; set; }
    }
}
