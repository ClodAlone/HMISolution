using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;

namespace VFS
{
    public class FileManagerFolder : FileManagerItem, IEquatable<FileManagerFolder>
    {
        public FileManagerFolder(FileSystemProviderBase provider, string relativeName)
            : base(provider, relativeName) { }
        public FileManagerFolder(FileSystemProviderBase provider, FileManagerFolder parentFolder, string name)
            : base(provider, parentFolder, name) { }
        protected override string GetName()
        {
            if (string.IsNullOrEmpty(RelativeName))
                return Provider.RootFolderDisplayName;
            else
            {
                int sepIndex = RelativeName.LastIndexOf('\\');
                return sepIndex > -1 ? RelativeName.Remove(0, sepIndex + 1) : RelativeName;
            }
        }
        [Description("Gets the immediate parent folder to which the current folder belongs.")]
        public FileManagerFolder Parent
        {
            get
            {
                return string.IsNullOrEmpty(RelativeName)
                    ? null
                    : new FileManagerFolder(Provider, GetParentName(RelativeName));
            }
        }
        public FileManagerFolder[] GetFolders()
        {
            return new List<FileManagerFolder>(Provider.GetFolders(this)).ToArray();
        }
        public FileManagerFile[] GetFiles()
        {
            return new List<FileManagerFile>(Provider.GetFiles(this)).ToArray();
        }
        public bool Equals(FileManagerFolder other)
        {
            return Provider.Equals(other.Provider) && RelativeName == other.RelativeName;
        }
    }
    public class FileManagerFile : FileManagerItem, IEquatable<FileManagerFile>
    {
        public FileManagerFile(FileSystemProviderBase provider, string relativeName)
            : base(provider, relativeName) { }
        public FileManagerFile(FileSystemProviderBase provider, FileManagerFolder parentFolder, string fileName)
            : base(provider, parentFolder, fileName) { }
        [Description("Gets the parent folder to which the current file belongs.")]
        public FileManagerFolder Folder { get { return new FileManagerFolder(Provider, GetParentName(RelativeName)); } }
        [Description("Gets the string that specifies the extension of the file.")]
        public string Extension { get { return Path.GetExtension(Name); } }
        protected override string GetName()
        {
            return Path.GetFileName(RelativeName);
        }
        public bool Equals(FileManagerFile other)
        {
            return Provider.Equals(other.Provider) && RelativeName == other.RelativeName;
        }
    }
    public abstract class FileManagerItem
    {
        internal static char[] Separators = new char[] { '\\', '/' };
        string relativeName;
        FileSystemProviderBase provider;
        public FileManagerItem(FileSystemProviderBase provider, FileManagerFolder parentFolder, string name)
            : this(provider, Path.Combine(parentFolder.RelativeName, name.Trim(Separators))) { }
        public FileManagerItem(FileSystemProviderBase provider, string relativeName)
        {
            this.relativeName = relativeName.Trim(Separators).Replace('/', '\\');
            this.provider = provider;
        }
        [Description("Gets a relative name of the current item.")]
        public string RelativeName { get { return this.relativeName; } }
        [Description("Gets the full name of the current item.")]
        public string FullName
        {
            get
            {
                return Path.Combine(Provider.RootFolder, RelativeName).Replace('/', '\\');
            }
        }
        [Description("Gets the name of the current item.")]
        public string Name { get { return GetName(); } }
        protected abstract string GetName();
        protected internal FileSystemProviderBase Provider { get { return provider; } }
        protected internal static string GetParentName(string path)
        {
            return path.LastIndexOfAny(Separators) > -1
                ? path.Substring(0, path.LastIndexOfAny(Separators))
                : string.Empty;
        }
        public override string ToString()
        {
            return FullName;
        }
    }
}
