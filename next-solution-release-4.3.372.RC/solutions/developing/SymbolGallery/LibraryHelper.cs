using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using UFProjectManager.ComponentService;
using VFS;

namespace SymbolGallery
{
    public class LibraryHelper
    {
        public static void ShowMessage(string message, bool error = false)
        {
            var uIMsgBoxAlertService = SymbolGalleryComponent.symbolGalleryComponent.UIMsgBoxAlertService;
            if (uIMsgBoxAlertService != null)
            {
                if (error)
                    uIMsgBoxAlertService.ShowError(message);
                else
                    uIMsgBoxAlertService.ShowInformation(message);
            }
            else
                MessageBox.Show(message);
        }
        public static MessageBoxResult ShowYesNo(string message)
        {
            var uIMsgBoxAlertService = SymbolGalleryComponent.symbolGalleryComponent.UIMsgBoxAlertService;
            if (uIMsgBoxAlertService != null)
            {
                if (uIMsgBoxAlertService.ShowYesNo(message, UIMsgBoxAlertService.ComponentService.CustomDialogIcons.Question) == UIMsgBoxAlertService.ComponentService.CustomDialogResults.Yes)
                    return MessageBoxResult.Yes;
                else
                    return MessageBoxResult.No;
            }
            else
                return MessageBox.Show(message,Properties.Resources.DeleteFolder, MessageBoxButton.YesNo,MessageBoxImage.Question);
        }
    }
}
