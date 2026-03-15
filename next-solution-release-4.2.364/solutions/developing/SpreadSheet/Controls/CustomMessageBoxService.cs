using DevExpress.Portable;
using DevExpress.Spreadsheet;
using DevExpress.Xpf.Core;
using DevExpress.XtraSpreadsheet.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using UIMsgBoxAlertService.ComponentService;

namespace SpreadSheet.Controls
{
    public class CustomMessageBoxService : IMessageBoxService
    {
        private FrameworkElement messageOwner;
        IUIMsgBoxAlertService uIMsgBoxAlertService;
        public CustomMessageBoxService(FrameworkElement owner, IUIMsgBoxAlertService uIMsgBoxAlertService) { messageOwner = owner; this.uIMsgBoxAlertService = uIMsgBoxAlertService; }

        public PortableDialogResult ShowMessage(string message, string title, PortableMessageBoxIcon icon)
        {
            if (messageOwner == null || uIMsgBoxAlertService == null) return PortableDialogResult.Cancel;
            //DXMessageBox.Show(messageOwner, message, title, MessageBoxButton.OK, (MessageBoxImage)icon);
            uIMsgBoxAlertService.ShowError(message);
            return PortableDialogResult.OK;
        }
        public bool ShowOkCancelMessage(string message)
        {
            if (messageOwner == null || uIMsgBoxAlertService == null) return false;
            //return DXMessageBox.Show(messageOwner, message, System.Windows.Forms.Application.ProductName, MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK;
            return uIMsgBoxAlertService.ShowOkCancel(message, CustomDialogIcons.Warning) == CustomDialogResults.OK;
        }
        public bool ShowYesNoMessage(string message)
        {
            if (messageOwner == null || uIMsgBoxAlertService == null) return false;
            //return DXMessageBox.Show(messageOwner, message, System.Windows.Forms.Application.ProductName, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
            return uIMsgBoxAlertService.ShowYesNo(message, CustomDialogIcons.Warning) == CustomDialogResults.Yes;
        }
        public PortableDialogResult ShowYesNoCancelMessage(string message)
        {
            if (messageOwner == null || uIMsgBoxAlertService == null) return PortableDialogResult.Cancel;
            CustomDialogResults result = uIMsgBoxAlertService.ShowYesNoCancel(message, CustomDialogIcons.Information);
            //MessageBoxResult result = DXMessageBox.Show(messageOwner, message, System.Windows.Forms.Application.ProductName, MessageBoxButton.YesNoCancel, MessageBoxImage.Information);
            switch (result)
            {
                case CustomDialogResults.Cancel:
                    return PortableDialogResult.Cancel;
                case CustomDialogResults.Yes:
                    return PortableDialogResult.Yes;
                case CustomDialogResults.No:
                    return PortableDialogResult.No;
                default:
                    return PortableDialogResult.Cancel;
            }
        }
        public PortableDialogResult ShowDataValidationDialog(string message, string title, DevExpress.Spreadsheet.DataValidationErrorStyle errorStyle)
        {
            if (messageOwner == null || uIMsgBoxAlertService == null) return PortableDialogResult.Cancel;
            if (errorStyle == DevExpress.Spreadsheet.DataValidationErrorStyle.Stop)
            {
                //MessageBoxResult result = DXMessageBox.Show(messageOwner, message, title, MessageBoxButton.OKCancel, MessageBoxImage.Stop);
                var result = uIMsgBoxAlertService.ShowOkCancel(message, CustomDialogIcons.Exclamation);
                return result == CustomDialogResults.OK ? PortableDialogResult.No : PortableDialogResult.Cancel;
            }
            if (errorStyle == DevExpress.Spreadsheet.DataValidationErrorStyle.Warning)
            {
                //return (DialogResult)DXMessageBox.Show(messageOwner, message, title, MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                CustomDialogResults result = uIMsgBoxAlertService.ShowYesNoCancel(message, CustomDialogIcons.Warning);
                switch (result)
                {
                    case CustomDialogResults.Cancel:
                        return PortableDialogResult.Cancel;
                    case CustomDialogResults.Yes:
                        return PortableDialogResult.Yes;
                    case CustomDialogResults.No:
                        return PortableDialogResult.No;
                    default:
                        return PortableDialogResult.Cancel;
                }
            }
            //return (DialogResult)DXMessageBox.Show(messageOwner, message, title, MessageBoxButton.OKCancel, MessageBoxImage.Information);
            return uIMsgBoxAlertService.ShowOkCancel(message, CustomDialogIcons.Warning) == CustomDialogResults.OK ? PortableDialogResult.OK : PortableDialogResult.Cancel;
        }
    }
}
