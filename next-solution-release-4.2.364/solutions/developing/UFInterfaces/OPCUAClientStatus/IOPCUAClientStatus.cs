using System;
using UFInterfaces;
using System.Windows;
using System.Windows.Controls;

namespace OPCUAClientStatus.ComponentService
{
    public interface IOPCUAClientStatus : IUFInterfaceBase
    {
        UserControl GetClientStatusControl();
    }
}
