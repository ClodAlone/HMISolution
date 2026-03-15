using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringManager.Interfaces
{
    public interface IStringEditorService
    {
        bool UpdateStringID(string oldId, string newId);
    }
}
