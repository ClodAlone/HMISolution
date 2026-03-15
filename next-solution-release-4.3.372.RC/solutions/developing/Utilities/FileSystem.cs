using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Utilities.IO
{
    public static class FileSystem
    {
        readonly static String xmlSignature = "<?xml version=\"1.0\"";
        readonly static String xamlSignature = "xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"";
        readonly static String xmlSchemaSignature = "http://www.w3.org/2001/XMLSchema";
        
        public static bool IsXmlFile(string fileName)
        {
            using (var reader = new StreamReader(fileName))
            {
                string ret = "";
                while ((ret = reader.ReadLine()) != null)
                {
                    if (ret == null)
                        return true;
                    bool bret = ret.Contains(xmlSignature);
                    if (bret)
                        return bret;
                    bret = ret.Contains(xamlSignature);
                    if (bret)
                        return bret;
                    bret = ret.Contains(xmlSchemaSignature);
                    if (bret)
                        return bret;
                }
                return false;
            }
        }

        public static bool IsBinaryFile(string fullPath)
        {
            using (var file = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, 10240, FileOptions.SequentialScan))
            {
                while (true)
                {
                    int currentByte = file.ReadByte();
                    if (currentByte == -1)
                        return false;
                    if (currentByte == 0)
                        return true;
                }
            }
        }

        public static void WriteFile(string fileName, string content, Encoding encoding)
        {
            using (var writer = new StreamWriter(fileName, false, encoding))
            {
                writer.Write(content);
            }
        }

        public static void WriteFile(string fileName, string content)
        {
            WriteFile(fileName, content, Encoding.UTF8);
        }

        public static void CopyTo(List<String> sources, String target)
        {
            sources.ForEach(source =>
            {
                try
                {
                    FileAttributes attr = File.GetAttributes(source);
                    if (attr.HasFlag(FileAttributes.Directory))
                    {
                        var sourcePath = source.TrimEnd(Path.DirectorySeparatorChar, ' ');
                        var targetPath = target.TrimEnd(Path.DirectorySeparatorChar, ' ');
                        var files = Directory.EnumerateFiles(sourcePath, "*", SearchOption.AllDirectories)
                                             .GroupBy(s => Path.GetDirectoryName(s));
                        foreach (var folder in files)
                        {
                            var targetFolder = String.Format("{0}{1}", folder.Key.Replace(sourcePath, targetPath), sourcePath.Replace(Path.GetDirectoryName(sourcePath), ""));
                            Directory.CreateDirectory(targetFolder);
                            foreach (var file in folder)
                            {
                                var targetFile = Path.Combine(targetFolder, Path.GetFileName(file));
                                if (File.Exists(targetFile)) File.Delete(targetFile);
                                File.Copy(file, targetFile);
                            }
                        }
                    }
                    else
                    {
                        var targetFile = Path.Combine(target, Path.GetFileName(source));
                        if (File.Exists(targetFile)) File.Delete(targetFile);
                        File.Copy(source, targetFile);
                    }
                }
                catch (FileNotFoundException ex) { }
            });
        }
    }
}
