using System;

namespace UFInterfaces.Service
{
    public interface IServiceControl
    {
        String Name { get; }
        String FriendlyName { get; }
        bool UseCredentialProvider { get; }
        void Install();
        void Install(string username, string password);
        void Uninstall();
        void Start();
        void Stop();
        void OpenServiceControl();
    }
}
