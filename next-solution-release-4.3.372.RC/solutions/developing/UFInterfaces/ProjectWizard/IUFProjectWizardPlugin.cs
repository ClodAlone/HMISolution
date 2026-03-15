using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFInterfaces;
using System.Collections;
using System.Windows;

namespace UFProjectWizard.ComponentService
{
    public interface IUFProjectWizardPlugin : IUFInterfaceBase
    {
        Uri CreateNewProject(Uri current, Window parent, object viewmodel);

        bool IsStartable { get; }
    }
}
