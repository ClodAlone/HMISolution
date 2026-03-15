using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFInterfaces;
using System.Collections;
#if !WINDOWS_UWP
using System.Windows.Controls;
#endif

namespace CommandManagerService.ComponentService
{
    public interface ICommandManagerService : IUFInterfaceBase
    {
    }
}
