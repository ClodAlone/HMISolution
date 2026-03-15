using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFInterfaces;
using System.Collections;
using System.Windows.Controls;

namespace CommandExplorer.ComponentService
{
    public interface ICommandExplorer : IUFInterfaceBase
    {
        UserControl control { get; }
        void PropagateChanges(UserControl commandlist);
        void SetSync(UserControl commandlist, bool sync);

        void Activate(int indexTab = -1);
    }
}
