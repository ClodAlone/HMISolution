using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using DocumentManager.ComponentService;
using UFInterfaces;

namespace UFUserEditor.ComponentService
{
    public class UserSettingsChangedArg : EventArgs
    {
        public UserSettingsChangedArg(IDocument parent)
        {
            Parent = parent;
        }
        readonly IDocument Parent;

        IDocument Document
        {
            get
            {
                return Parent;
            }
        }
    }
}
