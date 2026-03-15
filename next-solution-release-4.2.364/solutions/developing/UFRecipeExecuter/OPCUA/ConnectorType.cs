using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFRecipeExecuter.OPCUA
{
    public enum ConnectorType : int
    {
        None = 0x0,
        UseStandardMethods = 0x1,
        UseExtendedMethods = 0x2,
        UseAllMethods = 0x3
    }
}
