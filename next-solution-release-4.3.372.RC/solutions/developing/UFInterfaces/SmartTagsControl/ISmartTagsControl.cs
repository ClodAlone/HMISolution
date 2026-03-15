using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFInterfaces;
using System.Collections;
using System.Windows.Controls;
using System.Windows;

namespace SmartTagsControl.ComponentService
{
    public interface ISmartTagsControl : IUFInterfaceBase
    {
        event EventHandler Selecting;
        event EventHandler Selected;
        event EventHandler PrepareChanges;
        event EventHandler AcceptChanges;
        event EventHandler CancelChanges;

        Object SelectedObject { get; set; }
        IList SelectedObjects { get; set; }
    }
}
