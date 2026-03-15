using System;
using System.Collections.Generic;
using System.Linq;
using UFInterfaces;
using System.Collections;



namespace HelpProvider.ComponentService
{
    public interface IHelpProvider : IUFInterfaceBase
    {

        Boolean IsLocalHelpEnabled { get;}
        void UseLocalHelp(bool enableLocalHelp);
        void ShowAboutBox();
        void ExecuteHelp();
        string LocalFilePath { get; }
        string WebFilePath { get; }

        void InitRootPath();
        void OpenHelpPage(Uri uri);
        void OpenDialogHelpPage(string prop, bool alone = false, bool newwin = false);

    }
}