using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFInterfaces;
using System.Collections;

namespace UIMsgBoxAlertService.ComponentService
{
    /// <summary>
    /// Available Button options. 
    /// Abstracted to allow some level of UI Agnosticness
    /// </summary>
    public enum CustomDialogButtons
    {
        OK,
        OKCancel,
        YesNo,
        YesNoCancel
    }

    /// <summary>
    /// Available Icon options.
    /// Abstracted to allow some level of UI Agnosticness
    /// </summary>
    public enum CustomDialogIcons
    {
        None,
        Information,
        Question,
        Exclamation,
        Stop,
        Warning
    }

    /// <summary>
    /// Available DialogResults options.
    /// Abstracted to allow some level of UI Agnosticness
    /// </summary>
    public enum CustomDialogResults
    {
        None,
        OK,
        Cancel,
        Yes,
        No
        // FOGBUGZ 11407
        ,YesAll
        ,NoAll
    }

    public struct ShowCredentialOptions
    {
        public String WindowTitle;
        public String MainInstruction;
        public String Content;
        public bool ShowSaveCheckBox;
        public bool ShowUIForSavedCredentials;
        public String SavedCredentialsBucket;

        public String UserName;
        public String Password;
    }

    public struct ShowCredentialResults
    {
        public CustomDialogResults result;
        public String UserName;
        public String Password;
    }

    public struct FileOpenOptions
    {
        public bool CheckFileExists;
        public bool AddExtension;
        public String DefaultExt;
    }

    public struct FileSaveOptions
    {
        public bool OverwritePrompt;
        public bool AddExtension;
        public String DefaultExt;
    }

    public interface IUIMsgBoxAlertService : IUFInterfaceBase
    {
        /// <summary>
        /// Shows an error message
        /// </summary>
        /// <param name="message">The error message</param>
        void ShowError(string message);

        /// <summary>
        /// Shows an information message
        /// </summary>
        /// <param name="message">The information message</param>
        void ShowInformation(string message);

#if !WINDOWS_UWP
        /// <summary>
        /// Shows an information message with the "Don’t show this message again" option
        /// </summary>
        /// <param name="message">The information message</param>
        /// <param name="messageId">The key string to use for storing option</param>
        bool ShowHidingInformation(string message, string messageId);
#endif

        /// <summary>
        /// Shows an warning message
        /// </summary>
        /// <param name="message">The warning message</param>
        void ShowWarning(string message);

        /// <summary>
        /// Displays a Yes/No dialog and returns the user input.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        /// <param name="icon">The icon to be displayed.</param>
        /// <returns>User selection.</returns>
        CustomDialogResults ShowYesNo(string message, CustomDialogIcons icon);

        /// <summary>
        /// Displays a Yes/No/Cancel dialog and returns the user input.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        /// <param name="icon">The icon to be displayed.</param>
        /// <returns>User selection.</returns>
        CustomDialogResults ShowYesNoCancel(string message, CustomDialogIcons icon);

        /// <summary>
        /// Displays a OK/Cancel dialog and returns the user input.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        /// <param name="icon">The icon to be displayed.</param>
        /// <returns>User selection.</returns>
        CustomDialogResults ShowOkCancel(string message, CustomDialogIcons icon);

#if !WINDOWS_UWP
        ShowCredentialResults ShowCredentialDialog(ShowCredentialOptions option);

        String ShowOpenFileDialog(String filter, FileOpenOptions? options = null);
        String ShowSaveFileDialog(String filter, FileSaveOptions? options = null);
        String[] ShowSelectFileDialog(String filter);
        String ShowBrowseFolderDialog(String selectedpath);
#endif
    }
}
