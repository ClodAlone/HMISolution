using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace VFS
{
    public class FileManagerException : Exception
    {
        FileManagerErrors error;
        public FileManagerException(FileManagerErrors error)
            : this(error, null) { }
        public FileManagerException(FileManagerErrors error, Exception innerException)
            : this(error, innerException, string.Empty)
        {
        }
        public FileManagerException(FileManagerErrors error, Exception innerException, string message)
            : base(string.IsNullOrEmpty(message) ? GetErrorText(error) : message, innerException)
        {
            this.error = error;
        }
        [Description(" [To be supplied] ")]
        public FileManagerErrors Error { get { return error; } }
        protected static string GetErrorText(FileManagerErrors result)
        {
            switch (result)
            {
                case FileManagerErrors.UnspecifiedIO:
                    return Properties.Resources.FileManager_ErrorIO;
                case FileManagerErrors.FileNotFound:
                    return Properties.Resources.FileManager_ErrorFileNotFound;
                case FileManagerErrors.FolderNotFound:
                    return Properties.Resources.FileManager_ErrorFolderNotFound;
                case FileManagerErrors.AccessDenied:
                    return Properties.Resources.FileManager_ErrorNoAccess;
                case FileManagerErrors.EmptyName:
                    return Properties.Resources.FileManager_ErrorNameCannotBeEmpty;
                case FileManagerErrors.InvalidSymbols:
                    return Properties.Resources.FileManager_ErrorInvalidSymbols;
                case FileManagerErrors.WrongExtension:
                    return Properties.Resources.FileManager_ErrorWrongExtension;
                case FileManagerErrors.UsedByAnotherProcess:
                    return Properties.Resources.FileManager_ErrorUsedByAnotherProcess;
                case FileManagerErrors.AlreadyExists:
                    return Properties.Resources.FileManager_ErrorAlreadyExists;
                case FileManagerErrors.AccessProhibited:
                    return Properties.Resources.FileManager_ErrorAccessProhibited;
                default:
                    return Properties.Resources.FileManager_ErrorOther;
            }
        }
    }
    public class FileManagerIOException : FileManagerException
    {
        public FileManagerIOException(FileManagerErrors error)
            : base(error) { }
        public FileManagerIOException(FileManagerErrors error, Exception innerException)
            : base(error, innerException) { }
    }
    public class FileManagerAccessException : FileManagerException
    {
        public FileManagerAccessException()
            : base(FileManagerErrors.AccessProhibited) { }
    }
    public class FileManagerCancelException : FileManagerException
    {
        public FileManagerCancelException(string message)
            : base(FileManagerErrors.CanceledOperation, null, message) { }
    }
    public enum FileManagerErrors
    {
        FileNotFound = 0,
        FolderNotFound = 1,
        AccessDenied = 2,
        UnspecifiedIO = 3,
        Unspecified = 4,
        EmptyName = 5,
        CanceledOperation = 6,
        InvalidSymbols = 7,
        WrongExtension = 8,
        UsedByAnotherProcess = 9,
        AlreadyExists = 10,
        AccessProhibited = 11
    }
}
