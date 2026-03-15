using DocumentManager.ComponentService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Controls;
using UIMsgBoxAlertService.ComponentService;
using Utilities;
using WPFUtilities.PropertyDataTemplate;
using System.Windows.Media.Imaging;
using System.Windows.Markup;
using System.Windows;
using UFInterfaces.Converters;

namespace WPFUtilities
{
    public class DropFileHelper
    {
        #region Declarations
        readonly IDocument document;
        readonly SourceFileCopyOption copyOption;
        readonly IUIMsgBoxAlertService uiMsgBoxAlertService;
        readonly IUriToUriAbsoluteImageConverter uriToUriAbsoluteImageConverter;
        public static List<string> SupportedImageExtension { get{ return Properties.Settings.Default.SupportedImageExtension.Split('|').ToList(); } }
        #endregion

        #region Constructors
        public DropFileHelper(IDocument document, 
            IUriToUriAbsoluteImageConverter uriToUriAbsoluteImageConverter, 
            SourceFileCopyOption copyOption = SourceFileCopyOption.Always) : 
            this(document, uriToUriAbsoluteImageConverter, null, copyOption)
        { }

        public DropFileHelper(IDocument document, 
            IUriToUriAbsoluteImageConverter uriToUriAbsoluteImageConverter,
            IUIMsgBoxAlertService uiMsgBoxAlertService, 
            SourceFileCopyOption copyOption = SourceFileCopyOption.Always)
        {
            this.document = document;
            this.uriToUriAbsoluteImageConverter = uriToUriAbsoluteImageConverter;
            this.uiMsgBoxAlertService = uiMsgBoxAlertService;
            this.copyOption = copyOption;
        }
        #endregion

        #region Implementation
        public Visual DropFile(string file, out Uri uriFound)
        {
            byte[] data = null;
            Uri fileUri = new Uri(file, UriKind.RelativeOrAbsolute);
            Uri docUri = document.GetSpecialFolder(SpecialFolders.Images);
            string name = System.IO.Path.GetFileName(file);
            string dest = System.IO.Path.GetDirectoryName(file) + "\\";

            bool copyFile = copyOption == SourceFileCopyOption.Always;
            if (copyOption != SourceFileCopyOption.Never && docUri.IsBaseOf(fileUri))
            {
                name = file.Replace(docUri.GetPathString(), "");
                dest = System.IO.Path.GetDirectoryName(file) + "\\";
            }
            else
            {
                if (!copyFile && copyOption == SourceFileCopyOption.Ask)
                {
                    if (uiMsgBoxAlertService != null)
                        copyFile = uiMsgBoxAlertService.ShowYesNo(Properties.Resources.AskCopyFile,
                            UIMsgBoxAlertService.ComponentService.CustomDialogIcons.Question) == UIMsgBoxAlertService.ComponentService.CustomDialogResults.Yes;
                    else
                        copyFile = MessageBox.Show(Properties.Resources.AskCopyFile, name, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
                }

                if (copyFile)
                {
                    bool bOverwrite = true;
                    if (document.fileSystemProviderBase == null)
                    {
                        dest = System.IO.Path.GetDirectoryName(docUri.GetPathString());
                        string destfilename = System.IO.Path.Combine(dest, name);
                        if (!destfilename.Equals(file))
                        {
                            if (File.Exists(destfilename))
                            {
                                if (uiMsgBoxAlertService != null)
                                {
                                    var res = uiMsgBoxAlertService.ShowYesNoCancel(Properties.Resources.FileExistsOverwriteRename,
                                        UIMsgBoxAlertService.ComponentService.CustomDialogIcons.Question);
                                    if (res == UIMsgBoxAlertService.ComponentService.CustomDialogResults.No)
                                        bOverwrite = false;
                                    else if (res == UIMsgBoxAlertService.ComponentService.CustomDialogResults.Cancel)
                                        throw new OperationCanceledException("Overwrite file request aborted");
                                }
                                else
                                {
                                    var res = MessageBox.Show(Properties.Resources.FileExistsOverwriteRename, name, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                                    if (res == MessageBoxResult.No)
                                        bOverwrite = false;
                                    else if (res == MessageBoxResult.Cancel)
                                        throw new OperationCanceledException("Overwrite file request aborted");
                                }
                            }

                            var originalName = name;
                            var rnd = new Random();
                            bool bok = false;
                            do
                            {
                                try
                                {
                                    System.IO.File.Copy(file, destfilename, bOverwrite);
                                    bok = true;
                                }
                                catch
                                {
                                    name = rnd.Next(1000).ToString() + originalName;
                                    destfilename = System.IO.Path.Combine(dest, name);
                                }
                            } while (!bok);
                        }

                        name = System.IO.Path.GetFileName(destfilename);
                        dest = System.IO.Path.GetDirectoryName(destfilename) + "\\";
                    }
                    else
                    {
                        dest = System.IO.Path.GetDirectoryName(docUri.GetPathString());
                        string destfilename = System.IO.Path.Combine(dest, name);
                        if (destfilename.StartsWith("\\"))
                            destfilename = destfilename.Remove(0, 1);

                        if (!destfilename.Equals(file))
                        {
                            if (document.fileSystemProviderBase.Exists(new VFS.FileManagerFile(document.fileSystemProviderBase, destfilename)))
                            {
                                if (uiMsgBoxAlertService != null)
                                {
                                    var res = uiMsgBoxAlertService.ShowYesNoCancel(Properties.Resources.FileExistsOverwriteRename,
                                        UIMsgBoxAlertService.ComponentService.CustomDialogIcons.Question);
                                    if (res == UIMsgBoxAlertService.ComponentService.CustomDialogResults.No)
                                        bOverwrite = false;
                                    else if (res == UIMsgBoxAlertService.ComponentService.CustomDialogResults.Cancel)
                                        throw new OperationCanceledException("Overwrite file request aborted");
                                }
                                else
                                {
                                    var res = MessageBox.Show(Properties.Resources.FileExistsOverwriteRename, name, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                                    if (res == MessageBoxResult.No)
                                        bOverwrite = false;
                                    else if (res == MessageBoxResult.Cancel)
                                        throw new OperationCanceledException("Overwrite file request aborted");
                                }
                            }

                            if (!bOverwrite)
                            {
                                var originalName = name;
                                var rnd = new Random();
                                while (true)
                                {
                                    if (!document.fileSystemProviderBase.Exists(new VFS.FileManagerFile(document.fileSystemProviderBase, destfilename)))
                                        break;

                                    name = rnd.Next(1000).ToString() + originalName;
                                    destfilename = System.IO.Path.Combine(dest, name);
                                }
                            }

                            if (Utilities.IO.FileSystem.IsBinaryFile(file))
                            {
                                data = File.ReadAllBytes(file);
                            }
                            else
                            {
                                var text = File.ReadAllText(file);
                                data = System.Text.Encoding.Unicode.GetBytes(text);
                            }

                            document.fileSystemProviderBase.UploadFile(null, destfilename, data);
                        }

                        name = System.IO.Path.GetFileName(destfilename);
                        dest = System.IO.Path.GetDirectoryName(destfilename) + "\\";
                    }
                }
                else
                {
                    name = System.IO.Path.GetFileName(file);
                    dest = System.IO.Path.GetDirectoryName(file) + "\\";
                }
            }

            uriFound = new Uri(name, UriKind.RelativeOrAbsolute);
            var ext = System.IO.Path.GetExtension(file).ToLower();
            if (SupportedImageExtension.Contains(ext.ToLower()))
            {
                if (copyFile || docUri.IsBaseOf(fileUri))
                {
                    string subpath = name;
                    try
                    {
                        if (docUri.IsBaseOf(fileUri))
                            subpath = docUri.MakeRelativeUri(fileUri).ToString();
                    }
                    catch { }
                    var image = new Image();
                    var binding = new Binding()
                    {
                        Converter = uriToUriAbsoluteImageConverter,
                        ConverterParameter = subpath
                    };
                    image.SetBinding(Image.SourceProperty, binding);
                    return image;
                }
                else
                {
                    uriFound = new Uri(file, UriKind.RelativeOrAbsolute);
                    var image = new Image();
                    var bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.UriSource = uriFound;
                    bmp.EndInit();
                    image.Source = bmp;
                    return image;
                }
            }
            else
            {
                try
                {
                    var uriDest = new Uri(dest, UriKind.RelativeOrAbsolute);
                    var md = new MediaElement();
                    md.Source = uriFound;
                    (md as IUriContext).BaseUri = uriDest;
                    Utilities.ResourceDictionaryExtensions.SetMediaElementAutoStart(md);
                    return md;
                }
                catch
                {
                    if (document.fileSystemProviderBase != null && data != null)
                    {
                        var destFile = document.Parent != null ? document.Parent.Title : document.Title;
                        foreach (var c in System.IO.Path.GetInvalidPathChars())
                            destFile = destFile.Replace(c, '_');
                        destFile = String.Format("{0}\\{1}\\{2}\\{3}", Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), destFile, dest, name);
                        if (File.Exists(destFile))
                            File.Delete(destFile);
                        Directory.CreateDirectory(Path.GetDirectoryName(destFile));
                        File.WriteAllBytes(destFile, data);

                        dest = String.Format("{0}\\", Path.GetDirectoryName(destFile));
                        var uriDest = new Uri(dest, UriKind.RelativeOrAbsolute);
                        var md = new MediaElement();
                        md.Source = uriFound;
                        (md as IUriContext).BaseUri = uriDest;
                        Utilities.ResourceDictionaryExtensions.SetMediaElementAutoStart(md);
                        return md;
                    }

                    throw;
                }
            }
        }
        public string DropFile(string sourceFile, string rootBase)
        {
            byte[] data = null;
            Uri sourceUri = null;
            try
            {
                if (sourceFile.StartsWith("."))
                    sourceUri = new Uri(new Uri(rootBase + "\\"), System.IO.Path.GetFileName(sourceFile));
                else 
                    sourceUri = new Uri(sourceFile, UriKind.Absolute);
            }
            catch (Exception)
            {
                sourceUri = new Uri(new Uri(rootBase + "\\"), System.IO.Path.GetFileName(sourceFile));
            }

            if (!File.Exists(sourceUri.GetPathString()))
            {
                return sourceUri.GetPathString();
            }

            sourceFile = sourceUri.GetPathString();

            Uri destUri = new Uri(sourceFile, UriKind.RelativeOrAbsolute);
            Uri docUri = document.GetSpecialFolder(SpecialFolders.Images);
            Uri rootUri = new Uri(new Uri(rootBase + "\\"), System.IO.Path.GetFileName(sourceFile));
            Uri specialFileUri = new Uri(docUri, System.IO.Path.GetFileName(sourceFile));

            string name = System.IO.Path.GetFileName(sourceFile);
            string dest = System.IO.Path.GetDirectoryName(sourceFile) + "\\";

            bool copyFile = copyOption == SourceFileCopyOption.Always;
            if (copyOption != SourceFileCopyOption.Never && sourceFile == specialFileUri.GetPathString())
            {
                name = System.IO.Path.GetFileName(sourceFile);
                dest = System.IO.Path.GetDirectoryName(sourceFile) + "\\";
            }
            else
            {
                if (!copyFile && copyOption == SourceFileCopyOption.Ask)
                {
                    if (uiMsgBoxAlertService != null)
                        copyFile = uiMsgBoxAlertService.ShowYesNo(string.Format(Properties.Resources.AskCopySourceFile, sourceFile), 
                            UIMsgBoxAlertService.ComponentService.CustomDialogIcons.Question) == UIMsgBoxAlertService.ComponentService.CustomDialogResults.Yes;
                    else
                        copyFile = MessageBox.Show(string.Format(Properties.Resources.AskCopySourceFile, sourceFile), name, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
                }

                if (copyFile)
                {
                    bool bOverwrite = true;
                    if (document.fileSystemProviderBase == null)
                    {
                        dest = System.IO.Path.GetDirectoryName(docUri.GetPathString());
                        string destfilename = System.IO.Path.Combine(dest, name);
                        if (!destfilename.Equals(sourceFile))
                        {
                            if (File.Exists(destfilename))
                            {
                                if (uiMsgBoxAlertService != null)
                                {
                                    var res = uiMsgBoxAlertService.ShowYesNoCancel(Properties.Resources.FileExistsOverwriteRename,
                                        UIMsgBoxAlertService.ComponentService.CustomDialogIcons.Question);
                                    if (res == UIMsgBoxAlertService.ComponentService.CustomDialogResults.No)
                                        bOverwrite = false;
                                    else if (res == UIMsgBoxAlertService.ComponentService.CustomDialogResults.Cancel)
                                        throw new OperationCanceledException("Overwrite file request aborted");
                                }
                                else
                                {
                                    var res = MessageBox.Show(Properties.Resources.FileExistsOverwriteRename, name, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                                    if (res == MessageBoxResult.No)
                                        bOverwrite = false;
                                    else if (res == MessageBoxResult.Cancel)
                                        throw new OperationCanceledException("Overwrite file request aborted");
                                }
                            }

                            var originalName = name;
                            var rnd = new Random();
                            bool bok = false;
                            do
                            {
                                try
                                {
                                    System.IO.File.Copy(sourceFile, destfilename, bOverwrite);
                                    bok = true;
                                }
                                catch
                                {
                                    name = rnd.Next(1000).ToString() + originalName;
                                    destfilename = System.IO.Path.Combine(dest, name);
                                }
                            } while (!bok);
                        }

                        name = System.IO.Path.GetFileName(destfilename);
                        dest = System.IO.Path.GetDirectoryName(destfilename) + "\\";
                    }
                    else
                    {
                        dest = System.IO.Path.GetDirectoryName(docUri.GetPathString());
                        string destfilename = System.IO.Path.Combine(dest, name);

                        if (document.fileSystemProviderBase.Exists(new VFS.FileManagerFile(document.fileSystemProviderBase, destfilename)))
                        {
                            if (uiMsgBoxAlertService != null)
                            {
                                var res = uiMsgBoxAlertService.ShowYesNoCancel(Properties.Resources.FileExistsOverwriteRename,
                                    UIMsgBoxAlertService.ComponentService.CustomDialogIcons.Question);
                                if (res == UIMsgBoxAlertService.ComponentService.CustomDialogResults.No)
                                    bOverwrite = false;
                                else if (res == UIMsgBoxAlertService.ComponentService.CustomDialogResults.Cancel)
                                    throw new OperationCanceledException("Overwrite file request aborted");
                            }
                            else
                            {
                                var res = MessageBox.Show(Properties.Resources.FileExistsOverwriteRename, name, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                                if (res == MessageBoxResult.No)
                                    bOverwrite = false;
                                else if (res == MessageBoxResult.Cancel)
                                    throw new OperationCanceledException("Overwrite file request aborted");
                            }
                        }

                        if (!bOverwrite)
                        {
                            var originalName = name;
                            var rnd = new Random();
                            while (true)
                            {
                                if (!document.fileSystemProviderBase.Exists(new VFS.FileManagerFile(document.fileSystemProviderBase, destfilename)))
                                    break;

                                name = rnd.Next(1000).ToString() + originalName;
                                destfilename = System.IO.Path.Combine(dest, name);
                            }
                        }

                        if (Utilities.IO.FileSystem.IsBinaryFile(sourceFile))
                        {
                            data = File.ReadAllBytes(sourceFile);
                        }
                        else
                        {
                            var text = File.ReadAllText(sourceFile);
                            data = System.Text.Encoding.Unicode.GetBytes(text);
                        }

                        document.fileSystemProviderBase.UploadFile(null, destfilename, data);

                        name = System.IO.Path.GetFileName(destfilename);
                        dest = System.IO.Path.GetDirectoryName(destfilename) + "\\";
                    }
                }
                else
                {
                    name = System.IO.Path.GetFileName(sourceFile);
                    dest = System.IO.Path.GetDirectoryName(sourceFile) + "\\";
                }
            }
            if(dest != "\\")
                destUri = new Uri(new Uri(dest), name);
            else
                destUri = new Uri(name,UriKind.RelativeOrAbsolute);
            var ext = System.IO.Path.GetExtension(sourceFile);
            if (ext == ".bmp" || ext == ".png" || ext == ".gif" || ext == ".tif" || ext == ".jpg" || ext == ".tiff" || ext == ".ico")
            {
                if (copyFile || sourceFile == specialFileUri.GetPathString())
                {
                    return destUri.GetPathString();
                }
                else
                {
                    if (File.Exists(sourceFile))
                        return sourceFile;
                    else if (File.Exists(rootUri.GetPathString()))
                        return rootUri.GetPathString();
                    else if (File.Exists(specialFileUri.GetPathString()))
                        return specialFileUri.GetPathString();
                    else
                        return sourceFile;
                }
            }
            else
            {
                try
                {
                    return destUri.GetPathString();
                }
                catch
                {
                    if (document.fileSystemProviderBase != null && data != null)
                    {
                        var destFile = document.Parent != null ? document.Parent.Title : document.Title;
                        foreach (var c in System.IO.Path.GetInvalidPathChars())
                            destFile = destFile.Replace(c, '_');
                        destFile = String.Format("{0}\\{1}\\{2}\\{3}", Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), destFile, dest, name);
                        if (File.Exists(destFile))
                            File.Delete(destFile);
                        Directory.CreateDirectory(Path.GetDirectoryName(destFile));
                        File.WriteAllBytes(destFile, data);

                        dest = String.Format("{0}\\", Path.GetDirectoryName(destFile));
                        return new Uri(new Uri(dest), destFile).GetPathString();
                    }

                    throw;
                }
            }
        }
        #endregion
    }
}
