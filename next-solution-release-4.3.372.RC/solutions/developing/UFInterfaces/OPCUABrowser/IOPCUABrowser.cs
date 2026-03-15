using System;
using UFInterfaces;
using System.Windows;
using Opc.Ua;
using System.Windows.Controls;

namespace OPCUABrowser.ComponentService
{
    public interface IOPCUABrowser : IUFInterfaceBase
    {
        UserControl Editor { get; }

        UserControl MultiSelectionEditor { get; }

        bool BrowseEndpoint(String endpoint);
    }
}
