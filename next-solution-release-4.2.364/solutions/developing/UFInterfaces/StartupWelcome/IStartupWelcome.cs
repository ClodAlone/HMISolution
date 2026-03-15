using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFInterfaces.StartupWelcome
{
    public interface IStartupWelcome : IUFInterfaceBase
    {
        IEnumerable<Uri> GetLatestOpened();
        void AddToLatestOpened(Uri uri);

        void ShowOrActivateStartupWelcome();

        bool IsStartupOpened();
    }
}
