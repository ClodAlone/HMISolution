#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;

namespace Syncfusion.UI.Xaml.RichTextBoxAdv
{
    #region Create New Document Command
    public class NewDocumentCommand : CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NewDocumentCommand"/> class.
        /// </summary>
        /// <param name="richTextBoxAdv">The rich text box adv.</param>
        public NewDocumentCommand(SfRichTextBoxAdv richTextBoxAdv)
            : base(richTextBoxAdv)
        {
        }
        /// <summary>
        /// Executes the create new document command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void ExecuteCommand(object parameter)
        {
            OwnerControl.CreateBlankDocument();
            OwnerControl.Focus(FocusState.Pointer);
            base.ExecuteCommand(parameter);
        }
    }
    #endregion

    #region Open Document Command
    public class OpenDocumentCommand : CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenDocumentCommand"/> class.
        /// </summary>
        /// <param name="richTextBoxAdv">The rich text box adv.</param>
        public OpenDocumentCommand(SfRichTextBoxAdv richTextBoxAdv)
            : base(richTextBoxAdv)
        {
        }
        /// <summary>
        /// Executes the open document command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void ExecuteCommand(object parameter)
        {
            OpenDocument();
            OwnerControl.Focus(FocusState.Pointer);
            base.ExecuteCommand(parameter);
        }
        /// <summary>
        /// Opens the document.
        /// </summary>
        private async void OpenDocument()
        {
            FileOpenPicker fileOpenPicker = new FileOpenPicker();
            fileOpenPicker.FileTypeFilter.Add(".doc");
            fileOpenPicker.FileTypeFilter.Add(".dot");
            fileOpenPicker.FileTypeFilter.Add(".docx");
            fileOpenPicker.FileTypeFilter.Add(".dotx");
            fileOpenPicker.FileTypeFilter.Add(".rtf");
            StorageFile stgFile = await fileOpenPicker.PickSingleFileAsync();
            if (stgFile != null)
                OwnerControl.Load(stgFile);
        }
    }
    #endregion

    #region Print Document Command
    public class PrintDocumentCommand : CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrintDocumentCommand"/> class.
        /// </summary>
        /// <param name="richTextBoxAdv">The rich text box adv.</param>
        public PrintDocumentCommand(SfRichTextBoxAdv richTextBoxAdv)
            : base(richTextBoxAdv)
        {
        }
        /// <summary>
        /// Executes the print document command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void ExecuteCommand(object parameter)
        {
            OwnerControl.PrintDocument();
            OwnerControl.Focus(FocusState.Pointer);
            base.ExecuteCommand(parameter);
        }
    }
    #endregion

    #region Save Document Command
    public class SaveDocumentCommand : CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SaveDocumentCommand"/> class.
        /// </summary>
        /// <param name="richTextBoxAdv">The rich text box adv.</param>
        public SaveDocumentCommand(SfRichTextBoxAdv richTextBoxAdv)
            : base(richTextBoxAdv)
        {
        }
        /// <summary>
        /// Executes the save document command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void ExecuteCommand(object parameter)
        {
            SaveDocument();
            OwnerControl.Focus(FocusState.Pointer);
            base.ExecuteCommand(parameter);
        }
        /// <summary>
        /// Save word document
        /// </summary>
        private async void SaveDocument()
        {
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.FileTypeChoices.Add("Word 97 - 2003 Document", new List<string>() { ".doc" });
            savePicker.FileTypeChoices.Add("Word Document", new List<string>() { ".docx" });
            savePicker.FileTypeChoices.Add("Rich Text File", new List<string>() { ".rtf" });
            savePicker.DefaultFileExtension = ".rtf";
            savePicker.SuggestedFileName = "Sample";
            StorageFile saveStgFile = await savePicker.PickSaveFileAsync();
            if (saveStgFile != null)
                OwnerControl.Save(saveStgFile);
        }
    }
    #endregion

    #region SaveAs Document Command
    public class SaveAsDocumentCommand : CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SaveAsDocumentCommand"/> class.
        /// </summary>
        /// <param name="richTextBoxAdv">The rich text box adv.</param>
        public SaveAsDocumentCommand(SfRichTextBoxAdv richTextBoxAdv)
            : base(richTextBoxAdv)
        {
        }
        /// <summary>
        /// Executes the saveas document command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void ExecuteCommand(object parameter)
        {
            if (parameter == null)
                parameter = ".rtf";
            string extension = parameter.ToString();
            SaveDocument(extension);
            OwnerControl.Focus(FocusState.Pointer);
            base.ExecuteCommand(parameter);
        }
        /// <summary>
        /// Save word document
        /// </summary>
        private async void SaveDocument(string extension)
        {
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.FileTypeChoices.Add("Word 97 - 2003 Document", new List<string>() { ".doc" });
            savePicker.FileTypeChoices.Add("Word Document", new List<string>() { ".docx" });
            savePicker.FileTypeChoices.Add("Rich Text File", new List<string>() { ".rtf" });
            savePicker.DefaultFileExtension = extension;
            savePicker.SuggestedFileName = "Sample";
            StorageFile saveStgFile = await savePicker.PickSaveFileAsync();
            if (saveStgFile != null)
                OwnerControl.Save(saveStgFile);
        }
    }
    #endregion
}
