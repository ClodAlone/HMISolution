using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentManager.ComponentService;
using UFInterfaces;

namespace NewDriverWizard.ComponentService
{
    public interface IUFNewDriverWizard : IUFInterfaceBase
    {
        object ConfigureNewDriver(IDocument doc, object projectview, String conn);
    }
}
