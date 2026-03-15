using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;

namespace VFS
{
    public class PhysicalFileSystemProvider : FileSystemProviderBase
    {
        public PhysicalFileSystemProvider(string rootFolder)
            : base(rootFolder) { }
        public override string GetRelativeFolderPath(FileManagerFolder folder, System.Web.UI.IUrlResolutionService rs)
        {
            string url = UrlUtils.ResolvePhysicalPath(rs, Path.Combine(GetResolvedRootFolderPath(), folder.RelativeName));
            return url == "./" ? string.Empty : url.EndsWith("/") ? url : url + "/";
        }
        public override bool Exists(FileManagerFolder folder)
        {
            return Directory.Exists(Path.Combine(GetResolvedRootFolderPath(), folder.RelativeName));
        }
        public override bool Exists(FileManagerFile file)
        {
            return File.Exists(Path.Combine(GetResolvedRootFolderPath(), file.RelativeName));
        }
        [Description(" [To be supplied] ")]
        public override string RootFolderDisplayName
        {
            get
            {
                string resolvedRootFolder = GetResolvedRootFolderPath().TrimEnd('\\', '/');
                int sepIndex = resolvedRootFolder.LastIndexOf('\\');
                return sepIndex > -1 ? resolvedRootFolder.Remove(0, sepIndex + 1) : resolvedRootFolder;
            }
        }
        public override IEnumerable<FileManagerFolder> GetFolders(FileManagerFolder parentFolder)
        {
            DirectoryInfo pDir = new DirectoryInfo(Path.Combine(GetResolvedRootFolderPath(), parentFolder.RelativeName));
            foreach (DirectoryInfo dir in pDir.GetDirectories())
            {
                yield return new FileManagerFolder(this, Path.Combine(parentFolder.RelativeName, dir.Name));
            }
        }
        public override IEnumerable<FileManagerFile> GetFiles(FileManagerFolder folder)
        {
            DirectoryInfo dir = new DirectoryInfo(Path.Combine(GetResolvedRootFolderPath(), folder.RelativeName));
            foreach (FileInfo f in dir.GetFiles())
            {
                yield return new FileManagerFile(this, Path.Combine(folder.RelativeName, f.Name));
            }
        }
        public override byte[] ReadFile(FileManagerFile file)
        {
            return new MemoryStream(File.ReadAllBytes(Path.Combine(GetResolvedRootFolderPath(), file.RelativeName))).ToArray();
        }
        public override DateTime GetLastWriteTime(FileManagerFile file)
        {
            return File.GetLastWriteTime(Path.Combine(GetResolvedRootFolderPath(), file.RelativeName));
        }
        public override void DeleteFile(FileManagerFile file)
        {
            try
            {
                File.Delete(Path.Combine(GetResolvedRootFolderPath(), file.RelativeName));
            }
            catch (Exception e)
            {
                SetError(e);
            }
        }
        public override void DeleteFolder(FileManagerFolder folder)
        {
            try
            {
                Directory.Delete(Path.Combine(GetResolvedRootFolderPath(), folder.RelativeName), true);
            }
            catch (Exception e)
            {
                SetError(e);
            }
        }
        public override void MoveFile(FileManagerFile file, FileManagerFolder newParentFolder)
        {
            try
            {
                File.Move(Path.Combine(GetResolvedRootFolderPath(), file.RelativeName), Path.Combine(GetResolvedRootFolderPath(), new FileManagerFile(this, newParentFolder, file.Name).RelativeName));
            }
            catch (Exception e)
            {
                SetError(e);
            }
        }
        public override void MoveFolder(FileManagerFolder folder, FileManagerFolder newParentFolder)
        {
            try
            {
                Directory.Move(Path.Combine(GetResolvedRootFolderPath(), folder.RelativeName), Path.Combine(GetResolvedRootFolderPath(), new FileManagerFolder(this, newParentFolder, folder.Name).RelativeName));
            }
            catch (Exception e)
            {
                SetError(e);
            }
        }
        public override void RenameFile(FileManagerFile file, string name)
        {
            try
            {
                File.Move(Path.Combine(GetResolvedRootFolderPath(), file.RelativeName), Path.Combine(GetResolvedRootFolderPath(), new FileManagerFile(this, file.Folder, name).RelativeName));
            }
            catch (Exception e)
            {
                SetError(e);
            }
        }
        public override void RenameFolder(FileManagerFolder folder, string name)
        {
            try
            {
                Directory.Move(Path.Combine(GetResolvedRootFolderPath(), folder.RelativeName), Path.Combine(GetResolvedRootFolderPath(), new FileManagerFolder(this, folder.Parent, name).RelativeName));
            }
            catch (Exception e)
            {
                SetError(e);
            }
        }
        public override void CreateFolder(FileManagerFolder parent, string name)
        {
            try
            {
                Directory.CreateDirectory(Path.Combine(GetResolvedRootFolderPath(), new FileManagerFolder(this, parent, name).RelativeName));
            }
            catch (Exception e)
            {
                SetError(e);
            }
        }
        public override void UploadFile(FileManagerFolder folder, string fileName, byte[] content)
        {
            try
            {
                string path = Path.Combine(GetResolvedRootFolderPath(), new FileManagerFile(this, folder, fileName).RelativeName);
                using (FileStream fileStream = File.Create(path))
                {
                    fileStream.Write(content, 0, content.Length);
                    // CommonUtils.CopyStream(content, fileStream);
                }
            }
            catch (Exception e)
            {
                SetError(e);
            }
        }
        public string GetResolvedRootFolderPath()
        {
            return UrlUtils.ResolvePhysicalPath(RootFolder);
        }
        void SetError(Exception exc)
        {
            if (exc is FileNotFoundException)
                throw new FileManagerIOException(FileManagerErrors.FileNotFound, exc);
            if (exc is UnauthorizedAccessException)
                throw new FileManagerIOException(FileManagerErrors.AccessDenied, exc);
            if (exc is DirectoryNotFoundException)
                throw new FileManagerIOException(FileManagerErrors.FolderNotFound, exc);
            if (exc is IOException)
            {
                try
                {
                    switch (GetHRForException(exc))
                    {
                        case -2147024864:
                            throw new FileManagerIOException(FileManagerErrors.UsedByAnotherProcess, exc);
                        case -2146232800:
                        case -2147024891:
                            throw new FileManagerIOException(FileManagerErrors.AccessDenied, exc);
                        case -2147024713:
                            throw new FileManagerIOException(FileManagerErrors.AlreadyExists, exc);
                    }
                }
                catch { }
                throw new FileManagerIOException(FileManagerErrors.UnspecifiedIO, exc);
            }
            throw new FileManagerException(FileManagerErrors.Unspecified);
        }
        static int GetHRForException(System.Exception exc)
        {
            return System.Runtime.InteropServices.Marshal.GetHRForException(exc);
        }
        static string MakeRelativePath(string fromPath, string toPath)
        {
            Uri fromUri = new Uri(fromPath.EndsWith("\\") ? fromPath : fromPath + "\\");
            Uri toUri = new Uri(toPath.EndsWith("\\") ? toPath : toPath + "\\");
            return Uri.UnescapeDataString(fromUri.MakeRelativeUri(toUri).ToString());
        }
    }
}
