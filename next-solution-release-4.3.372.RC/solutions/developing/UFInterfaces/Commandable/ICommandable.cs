using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Collections;
using System.Windows;

namespace UFInterfaces.Commandable
{
    public interface ICommandable
    {
        String Name { get; }
        IEnumerable CommandList { get; set; }
        bool WebHMISupported { get; }
        UIElement Control { get; }
    }
}
