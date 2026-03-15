using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFInterfaces;
using System.Collections;
using UIMsgBoxAlertService.ComponentService;

namespace OPCUAViewModelService.ComponentService
{
    public interface IOPCUAViewModelService : IUFInterfaceBase
    {
        void QueryComponentInterfaces();
        bool IsUiInterfaceAvailable();
        void ShowError(string error);
        void ShowInformation(string info);
        CustomDialogResults ShowYesNo(string message, CustomDialogIcons icon);
    }
}
