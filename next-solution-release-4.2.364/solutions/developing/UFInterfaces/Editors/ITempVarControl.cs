using System;
using System.Collections.Generic;
using System.Linq;
using UFInterfaces;
using System.Collections;
using System.Windows.Input;

namespace TempVarriables.ComponentService
{
    public interface ITempVarControl
    {
        void AddNewFolder();
        void AddNewTag();
        void OnActivate();
        bool IsAnyItemSelected();

        String AddNewTag(String name, String Type);
        void ClearSelection();
    }
}