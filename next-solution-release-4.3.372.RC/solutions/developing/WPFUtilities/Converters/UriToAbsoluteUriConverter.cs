using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentManager.ComponentService;
using Utilities;
using VFS;

namespace WPFUtilities.Converters
{
    public static class UriToAbsoluteUriConverter
    {
        public static Uri Convert(Uri value, IDocument document, SpecialFolders specialfolder)
        {
            if (value == null || value.ToString().Length == 0)
                return null;

            try
            {
                if (document == null)
                    return null;
                
                if (document.fileSystemProviderBase != null)
                    return GetFileSystemProviderBaseUri(value, document, specialfolder);

                var name = value.OriginalString;// System.IO.Path.GetFileName(value.OriginalString);
                if (value.IsAbsoluteUri)
                    name = System.IO.Path.GetFileName(value.OriginalString);
                if (name == null)
                    return null;
                Uri uri = null;
                try
                {
                    return new Uri(value, name);
                }
                catch
                {
                    try
                    {
                        uri = new Uri(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(document.FilePath), name));
                         if(System.IO.File.Exists(uri.GetPathString()))
                             return uri;
                         else
                         {
                             var absolutePath2 = document.GetSpecialFolder(specialfolder);

                             if (name == null || absolutePath2 == null)
                                 return null;
                             try
                             {
                                 uri = new Uri(absolutePath2, name);
                                 if (System.IO.File.Exists(uri.GetPathString()))
                                     return uri;
                                 else
                                     return null;
                             }
                             catch
                             {
                                 return null;
                             }
                         }
                    }
                    catch (Exception)
                    {
                        var absolutePath2 = document.GetSpecialFolder(specialfolder);

                        if (name == null || absolutePath2 == null)
                            return null;
                        try
                        {
                            uri = new Uri(absolutePath2, name);
                            return uri;
                        }
                        catch
                        {
                            return null;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return null;
        }
        public static Uri GetFileSystemProviderBaseUri(Uri uri, IDocument document, SpecialFolders specialfolder)
        {
            if (document == null || document.fileSystemProviderBase == null)
                return uri;
            try
            {
                string specialfolderpath = System.IO.Path.Combine(document.GetSpecialFolder(specialfolder).OriginalString, System.IO.Path.GetFileName(uri.GetPathString()));
                FileSystemProviderBase fileSystemProviderBase = document.fileSystemProviderBase;
                var vfsFile = new FileManagerFile(fileSystemProviderBase, specialfolderpath);
                if (!fileSystemProviderBase.Exists(vfsFile))
                    return uri;

                var destFile = document.Parent != null ? document.Parent.Title : document.Title;
                foreach (var c in System.IO.Path.GetInvalidPathChars())
                    destFile = destFile.Replace(c, '_');
                if (specialfolder == SpecialFolders.Images)
                    destFile = String.Format("{0}\\{1}\\{2}", Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), destFile, vfsFile.FullName);
                else
                    destFile = String.Format("{0}\\{1}\\{2}", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), destFile, vfsFile.FullName);
                if (!File.Exists(destFile))
                {
                    var data = fileSystemProviderBase.ReadFile(vfsFile);
                    Directory.CreateDirectory(Path.GetDirectoryName(destFile));
                    File.WriteAllBytes(destFile, data);
                }

                return new Uri(destFile, UriKind.RelativeOrAbsolute);
            }
            catch (Exception)
            {
                return uri;
            }
        }
    }
}
