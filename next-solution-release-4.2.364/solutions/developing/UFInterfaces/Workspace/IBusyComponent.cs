using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DocumentManager.ComponentService;

namespace UFInterfaces
{
    public interface IBusyComponent : IUFInterfaceBase
    {
        bool IsBusy { get; set; }
        String BusyContent { get; set; }
        void ResetBusy();
        void RestoreBusy();
    }
}
   