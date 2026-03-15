using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFInterfaces;
using System.Collections;
using DocumentManager.ComponentService;

namespace ScriptManager.ComponentService
{
    public interface IScriptManager : IUFInterfaceBase
    {
        String GetScriptCode(IDocument parent, String script);
    }
}
