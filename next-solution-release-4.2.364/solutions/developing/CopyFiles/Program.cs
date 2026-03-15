using log4net;
using log4net.Config;
using System;
using System.IO;

namespace CopyFiles
{
    class Program
    {
        #region Declarations
        static CommandLineOptions cl;

        static readonly ILog log = Utilities.Logger.Logger.GetDestinationLog(Utilities.Logger.LoggerDestination.Application);

        const string tmpExtension = ".tmp";
        #endregion

        static void Main(string[] args)
        {
            Utilities.LocalizationHelper.TryApplyCurrentLanguage();

#if DEBUG
            if (!System.Diagnostics.Debugger.IsAttached &&
                Environment.UserInteractive && System.Windows.Forms.MessageBox.Show("if you would like to attach a debugger now is the right moment !",
                    "DebugMe - CopyFiles", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                System.Diagnostics.Debugger.Launch();
#endif

            XmlConfigurator.Configure();

            if (args.Length > 0)
            {
                cl = new CommandLineOptions(args);
                if (cl.IsValid)
                {
                    try
                    {
                        Environment.ExitCode = Process();
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex.Message);
                        Environment.ExitCode = ex.HResult;
                    }
                }
                else
                {
                    log.Error(Properties.Resources.InvalidOptions);
                    Environment.ExitCode = -1;
                }
            }
            else
            {
                log.Error(Properties.Resources.InvalidOptionNumber);
                Environment.ExitCode = -1;
            }

            if (Environment.ExitCode != 0)
                System.Threading.Thread.Sleep(1000);
        }

        static int Process()
        {
            if (!Directory.Exists(cl.Source))
            {
                log.Error(String.Format(Properties.Resources.SourceFolderNotFound, cl.Source));
                return -1;
            }
            else if (!Directory.Exists(cl.Destination))
            {
                log.Error(String.Format(Properties.Resources.DestinationFolderNotFound, cl.Source));
                return -1;
            }
            else
            {
                int exitCode = 0;
                var sourcePath = cl.Source;
                var destPath = cl.Destination;
                
                var sourceInfo = new DirectoryInfo(sourcePath);
                var files = sourceInfo.GetFiles(cl.SearchPattern ?? "*.*", cl.SubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
                // 1. Rename files.
                foreach (FileInfo file in files)
                {
                    var fullPathName = String.Format("{0}{1}", destPath, file.FullName.Replace(sourcePath, String.Empty));
                    var tmpFileName = String.Format("{0}{1}", fullPathName, tmpExtension);

                    if (!File.Exists(fullPathName))
                        continue;

                    try
                    {
                        File.Delete(tmpFileName);
                    }
                    catch
                    { }

                    try
                    {
                        if (!File.Exists(tmpFileName))
                            File.Move(fullPathName, tmpFileName);
                        else
                            File.Delete(fullPathName);
                    }
                    catch (Exception ex)
                    {
                        exitCode = ex.HResult;
                        log.Error(ex.Message);
                    }
                }

                // 2. Move files.
                foreach (FileInfo file in files)
                {
                    var fullPathName = String.Format("{0}{1}", destPath, file.FullName.Replace(sourcePath, String.Empty));
                    var tmpFileName = String.Format("{0}{1}", fullPathName, tmpExtension);

                    try
                    {
                        File.Copy(file.FullName, fullPathName, true);
                    }
                    catch (Exception ex)
                    {
                        exitCode = ex.HResult;
                        log.Error(ex.Message);
                    }
                }

                // 2. Delete temporary files.
                foreach (FileInfo file in files)
                {
                    var fullPathName = String.Format("{0}{1}", destPath, file.FullName.Replace(sourcePath, String.Empty));
                    var tmpFileName = String.Format("{0}{1}", fullPathName, tmpExtension);

                    if (!File.Exists(tmpFileName))
                        continue;

                    try
                    {
                        File.Delete(tmpFileName);
                    }
                    catch
                    { }
                }

                return exitCode;
            }
        }
    }
}
