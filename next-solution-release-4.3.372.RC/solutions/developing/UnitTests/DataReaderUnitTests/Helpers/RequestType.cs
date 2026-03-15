using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReaderUnitTests.Helpers
{
    public enum RequestType : int
    {
        AddUserDataSource = 1,
        ModifyUserDataSource = 2,
        RemoveUserDataSource = 3,
        AddSystemDataSource = 4,
        ModifySystemDataSource = 5,
        RemoveSystemDataSource = 6
    }
}
