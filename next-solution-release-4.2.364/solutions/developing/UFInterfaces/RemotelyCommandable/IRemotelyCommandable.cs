using System.Windows;

namespace UFInterfaces.RemotelyCommandable
{
    public interface IRemotelyCommandable
    {
        bool CheckRemoteCommands(UIElement uie, bool bExecute = true);
    }
}
