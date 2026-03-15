using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFInterfaces;
using System.Collections;

namespace UFProjectWizard.ComponentService
{
    public interface IUFProjectWizard : IUFInterfaceBase
    {
        Uri CreateNewProject(Uri current);
    }
}
