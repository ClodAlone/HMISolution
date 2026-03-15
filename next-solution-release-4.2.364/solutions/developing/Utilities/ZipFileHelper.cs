using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class ZipFileHelper
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Creates an archive. </summary>
        ///
        /// <remarks>   Mauri, 13/08/2014. </remarks>
        ///
        /// <param name="folder">       Pathname of the folder. </param>
        /// <param name="exceptions">   The exceptions in lower case (use backslash for folder, for example "/obj"). </param>
        /// <param name="archiveName">  Name of the archive. </param>
        ///
        /// <returns>   The number of files zipped in the archive. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static int CreateArchive(string folder,
                IList<string> exceptions, string archiveName)
        {
            int filesCount = 0;
            string folderFullPath = Path.GetFullPath(folder);
            string archivePath = Path.Combine(folderFullPath, archiveName);
            if (File.Exists(archivePath))
                File.Delete(archivePath);
            IEnumerable<string> files = Directory.EnumerateFiles(folder,
                    "*.*", SearchOption.AllDirectories);
            using (ZipArchive archive = ZipFile.Open(archivePath, ZipArchiveMode.Create))
            {
                foreach (string file in files)
                {
                    if (!Excluded(file, exceptions))
                    {
                        try
                        {
                            var addFile = Path.GetFullPath(file);
                            if (addFile != archivePath)
                            {
                                addFile = addFile.Substring(folderFullPath.Length + 1);
                                archive.CreateEntryFromFile(file, addFile);
                                filesCount++;
                            }
                        }
                        catch (IOException ex)
                        {
                            Debug.WriteLine(String.Format(@"Failed to add {0} due to error : 
                            {1} \n Ignoring it!", file, ex.Message));
                        }
                    }
                }
            }
            return filesCount;
        }

        private static bool Excluded(string file, IList<string> exceptions)
        {
            List<String> folderNames = (from folder in exceptions
                                        where folder.StartsWith(@"\")
                                            || folder.StartsWith(@"/")
                                        select folder).ToList<string>();
            if (!exceptions.Contains(Path.GetExtension(file).ToLower()))
            {
                foreach (string folderException in folderNames)
                {
                    if (Path.GetDirectoryName(file).ToLower().Contains(folderException))
                    {
                        return true;
                    }
                }
                return false;
            }
            return true;
        }
    }
}
