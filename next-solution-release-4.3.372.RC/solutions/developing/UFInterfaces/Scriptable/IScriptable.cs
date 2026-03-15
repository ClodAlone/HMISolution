using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace UFInterfaces.Scriptable
{
    public interface IScriptable
    {
        String Name { get; }
        String Code { get; set; }
        int[] Breakpoints { get; set; }
        bool CanEdit { get; }
        bool CanReadMacro { get; }
        void SetListProcedures(IList<String> list);
        IList GetReferenceList();
        IList GetQuickReferenceList();
        String GetReferenceName(Object var);
    }
}
