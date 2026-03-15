using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class DirectoryHelper
    {
        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs, bool showReasonOnError = false, bool continueOnError = false, bool overwrite = false)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                try
                {
                    file.CopyTo(temppath, overwrite);
                }
                catch (Exception ex)
                {
                    if (showReasonOnError)
                    {
#if !NET_STANDARD
                        if (System.Windows.MessageBox.Show(
                            String.Format(Properties.Resources.CopyFileErrorText,
                            file.Name,
                            destDirName,
                            ex.Message).Replace("'newline'", Environment.NewLine),
                            Properties.Resources.CopyFileErrorCaption,
                            System.Windows.MessageBoxButton.YesNo) != System.Windows.MessageBoxResult.Yes)
#endif
                            throw ex;
                    }
                    else if (!continueOnError)
                        throw ex;
                }
            }

            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs, showReasonOnError, continueOnError);
                }
            }
        }

        public static bool CanRemoveDirectory(string path)
        {
            if (String.IsNullOrEmpty(path.Trim()))
            {
                return false;
            }

            if (!Directory.Exists(path))
            {
                return false;
            }

            string[] filePaths = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            if (filePaths.Length > 0)
            {
                FileStream stream = null;
                foreach (var filepath in filePaths)
                {
                    try
                    {
                        FileInfo file = new FileInfo(filepath);
                        stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                    }
                    catch (Exception ex)
                    {
                        //the file is unavailable because it is:
                        //still being written to
                        //or being processed by another thread
                        //or does not exist (has already been processed)
                        return false;
                    }
                    finally
                    {
                        if (stream != null)
                            stream.Close();
                    }
                }
            }

            return true;
        }

        public static bool IsValidDirectory(string path, bool checkIfExist)
        {
            if (String.IsNullOrEmpty(path.Trim()))
            {
                return false;
            }

            string pathname;
            try
            {
                pathname = Path.GetPathRoot(path);
            }
            catch (ArgumentException)
            {
                // GetPathRoot() and GetFileName() above will throw exceptions
                // if pathname/filename could not be parsed.

                return false;
            }

            // Not sure if additional checking below is needed, but no harm done
            if (path.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                return false;
            }

            if (checkIfExist && !Directory.Exists(path))
            {
                return false;
            }

            return true;
        }
        public static bool IsValidFile(string path, bool checkIfExist)
        {
            if (String.IsNullOrEmpty(path.Trim()))
            {
                return false;
            }

            string pathname;
            try
            {
                pathname = Path.GetDirectoryName(path);
            }
            catch (ArgumentException)
            {
                return false;
            }

            if (String.IsNullOrEmpty(pathname?.Trim()))
            {
                if (checkIfExist)
                    return false;
            }
            else if (!IsValidDirectory(pathname, checkIfExist))
                return false;

            string filename;
            try
            {
                filename = Path.GetFileName(path);
            }
            catch (ArgumentException)
            {
                // GetFileName() above will throw exceptions
                // if pathname/filename could not be parsed.

                return false;
            }
            if (String.IsNullOrEmpty(filename.Trim()))
            {
                return false;
            }

            // Not sure if additional checking below is needed, but no harm done
            if (filename.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                return false;
            }

            if (checkIfExist && !File.Exists(path))
            {
                return false;
            }

            return true;
        }
    }
}
