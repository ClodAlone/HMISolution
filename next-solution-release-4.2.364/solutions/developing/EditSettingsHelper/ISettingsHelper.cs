using System;
using System.Collections.Generic;
using System.Linq;
using UFInterfaces;
using System.Collections;



namespace EditSettingsHelper.ComponentService
{
    public interface ISettingsHelper : IUFInterfaceBase
    {
        void UpdateWriteAccessCommands();
        void ReloadRuntimeSettings();
        int EditingWriteAccessLevel { get; }
        int EditingWriteAccessMask { get; }
    }
}