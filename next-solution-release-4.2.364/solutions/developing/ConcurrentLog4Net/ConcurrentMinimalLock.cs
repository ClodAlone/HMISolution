using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Appender;
using System.Security.AccessControl;
using System.Security.Principal;
using log4net;
using log4net.Repository.Hierarchy;

namespace ConcurrentLog4Net
{
    public class ConcurrentMinimalLock : FileAppender.MinimalLock
    {
        private string m_filename;
        private bool m_append;
        private Stream m_stream = null;
        private ConcurrentStream c_stream = null;

#if NET_STANDARD
        static long maxFileSizeInBytes = Properties.Settings.Default.MaximumFileSizeMB * 1024 * 1024;
#endif

        public override void OpenFile(string filename, bool append, Encoding encoding)
        {
            m_filename = filename;
            m_append = append;
        }
        public override void CloseFile()
        {
            // NOP
        }
        public override Stream AcquireLock()
        {
            if (m_stream == null)
            {
                try
                {
                    using (CurrentAppender.SecurityContext.Impersonate(this))
                    {
                        var rootAppender = ((Hierarchy)
#if !NET_STANDARD
                            LogManager.GetRepository()
#else
                            LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly())
#endif
                            ).Root.Appenders.OfType<FileAppender>().FirstOrDefault();
                        if (rootAppender != null)
                            m_filename = rootAppender.File;

                        string directoryFullName = Path.GetDirectoryName(m_filename);
                        if (!Directory.Exists(directoryFullName))
                        {
                            Directory.CreateDirectory(directoryFullName);
                        }

                        try
                        {
                            DirectoryInfo dInfo = new DirectoryInfo(directoryFullName);
                            DirectorySecurity dSecurity = dInfo.GetAccessControl();
                            dSecurity.AddAccessRule(new FileSystemAccessRule(WindowsIdentity.GetCurrent(false).Name,
                                                            FileSystemRights.Read | FileSystemRights.Write | FileSystemRights.Delete,
                                                            InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                                                            PropagationFlags.InheritOnly,
                                                            AccessControlType.Allow));
                            dInfo.SetAccessControl(dSecurity);
                        }
                        catch { }

#if NET_STANDARD
                        try
                        {
                            if (File.Exists(m_filename))
                            {
                                var info = new FileInfo(m_filename);
                                if (info.Length >= maxFileSizeInBytes)
                                    Utilities.DirectorySizeHelper.RollingFileHelper.RollOverFiles(m_filename, Properties.Settings.Default.MaxSizeRollBackups);
                            }
                        }
                        catch
                        { }
#endif

                        if (c_stream == null)
                        {
                            c_stream = ConcurrentStream.GetInstance(m_filename, m_append, FileAccess.Write, FileShare.Read);
                        }
                        m_stream = c_stream;
                        m_append = true;
                    }
                }
                catch (Exception e1)
                {
                    CurrentAppender.ErrorHandler.Error("Unable to acquire lock on file " + m_filename + ". " + e1.Message);
                }
            }
            return m_stream;
        }
        public override void ReleaseLock()
        {
            using (CurrentAppender.SecurityContext.Impersonate(this))
            {
                m_stream.Close();
                m_stream = null;
            }
        }
    }
}
