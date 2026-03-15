using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFInterfaces;
using System.Collections;
using System.Windows.Controls;

namespace AnimationExplorer.ComponentService
{
    public interface IAnimationExplorer : IUFInterfaceBase
    {
        UserControl control { get; }
        void Activate();
    }
}
