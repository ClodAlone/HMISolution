using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverCodeBaseEx.Extensions
{
    public static class NodeIdHelper
    {
        public static bool TryParse(string text, out Opc.Ua.NodeId nodeid)
        {
            try
            {
                nodeid = Opc.Ua.NodeId.Parse(text);
            }
            catch
            {
                nodeid = Opc.Ua.NodeId.Null;
                return false;
            }

            return true;
        }
    }
}
