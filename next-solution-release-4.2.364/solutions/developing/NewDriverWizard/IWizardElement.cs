using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NewDriverWizard
{
    interface IWizardElement
    {
        bool Execute();
        bool CanShowed();
    }
}
