using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using UFInterfaces;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Utilities.Converters;

namespace ServiceInstaller
{
    public static class ServiceInstaller
    {
        private const int STANDARD_RIGHTS_REQUIRED = 0xF0000;
        private const int SERVICE_WIN32_OWN_PROCESS = 0x00000010;

        private const String NameReplace = @"[^\x20-\x7F]";

        [StructLayout(LayoutKind.Sequential)]
        private class SERVICE_STATUS
        {
            public int dwServiceType = 0;
            public ServiceState dwCurrentState = 0;
            public int dwControlsAccepted = 0;
            public int dwWin32ExitCode = 0;
            public int dwServiceSpecificExitCode = 0;
            public int dwCheckPoint = 0;
            public int dwWaitHint = 0;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class SERVICE_CONFIG
        {
            [MarshalAs(UnmanagedType.U4)]
            public int dwServiceType = 0;
            [MarshalAs(UnmanagedType.U4)]
            public ServiceBootFlag dwStartType = 0;
            [MarshalAs(UnmanagedType.U4)]
            public ServiceError dwErrorControl;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpBinaryPathName;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpLoadOrderGroup;
            [MarshalAs(UnmanagedType.U4)]
            public int dwTagId = 0;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpDependencies;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpServiceStartName;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpDisplayName;
        }

        [StructLayout(LayoutKind.Sequential)]
        private class SERVICE_DELAYED_AUTO_START_INFO
        {
            public int fDelayedAutostart = 0;
        }

        #region OpenSCManager
        [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        static extern IntPtr OpenSCManager(string machineName, string databaseName, ScmAccessRights dwDesiredAccess);
        #endregion

        #region OpenService
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, ServiceAccessRights dwDesiredAccess);
        #endregion

        #region CreateService
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr CreateService(IntPtr hSCManager, string lpServiceName, string lpDisplayName, ServiceAccessRights dwDesiredAccess, int dwServiceType, ServiceBootFlag dwStartType, ServiceError dwErrorControl, string lpBinaryPathName, string lpLoadOrderGroup, IntPtr lpdwTagId, string lpDependencies, string lp, string lpPassword);
        #endregion

        #region CloseServiceHandle
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseServiceHandle(IntPtr hSCObject);
        #endregion

        #region ChangeServiceConfig
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int ChangeServiceConfig2(IntPtr hSCManager, ServiceInfoLevel dwInfoLevel, IntPtr lpInfo);
        #endregion

        #region QueryServiceConfig 
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int QueryServiceConfig(IntPtr hSCManager, IntPtr lpServiceConfig, int dwBufSize, ref int lpdwBytesNeeded);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int QueryServiceConfig2(IntPtr hSCManager, ServiceInfoLevel dwInfoLevel, IntPtr lpInfo, int dwBufSize, ref int lpdwBytesNeeded);
        #endregion

        #region QueryServiceStatus
        [DllImport("advapi32.dll")]
        private static extern int QueryServiceStatus(IntPtr hService, SERVICE_STATUS lpServiceStatus);
        #endregion

        #region DeleteService
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteService(IntPtr hService);
        #endregion

        #region ControlService
        [DllImport("advapi32.dll")]
        private static extern int ControlService(IntPtr hService, ServiceControl dwControl, SERVICE_STATUS lpServiceStatus);
        #endregion

        #region StartService
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern int StartService(IntPtr hService, int dwNumServiceArgs, int lpServiceArgVectors);
        #endregion

        #region Public Static Methods
        public static void Uninstall(string serviceName)
        {
            IntPtr scm = OpenSCManager(ScmAccessRights.AllAccess);

            try
            {
                IntPtr service = OpenService(scm, serviceName, ServiceAccessRights.AllAccess);
                if (service == IntPtr.Zero)
                {
                    throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
                }
                try
                {
                    StopService(service);
                    if (!DeleteService(service))
                    {
                        throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
                    }
                }
                finally
                {
                    CloseServiceHandle(service);
                }
            }
            finally
            {
                CloseServiceHandle(scm);
            }
        }

        public static bool ServiceIsInstalled(string serviceName)
        {
            IntPtr scm = OpenSCManager(ScmAccessRights.Connect);

            try
            {
                IntPtr service = OpenService(scm, serviceName, ServiceAccessRights.QueryStatus);

                if (service == IntPtr.Zero)
                    return false;

                CloseServiceHandle(service);
                return true;
            }
            finally
            {
                CloseServiceHandle(scm);
            }
        }

        public static bool ServiceIsDelayedAutoStart(string serviceName)
        {
            IntPtr scm = OpenSCManager(ScmAccessRights.Connect);

            try
            {
                IntPtr service = OpenService(scm, serviceName, ServiceAccessRights.QueryConfig);

                if (service == IntPtr.Zero)
                    return false;

                try
                {
                    return ServiceIsDelayedAutoStart(service);
                }
                finally
                {
                    CloseServiceHandle(service);
                }
            }
            finally
            {
                CloseServiceHandle(scm);
            }
        }

        public static void Install(string serviceName, string displayName, string fileName, 
            string dependencies = null, string usr = null, string pwd = null)
        {
            IntPtr scm = OpenSCManager(ScmAccessRights.AllAccess);

            try
            {
                IntPtr service = OpenService(scm, serviceName, ServiceAccessRights.AllAccess);

                if (service == IntPtr.Zero)
                    service = CreateService(scm, serviceName, displayName, ServiceAccessRights.AllAccess,
                        SERVICE_WIN32_OWN_PROCESS, ServiceBootFlag.AutoStart, ServiceError.Normal,
                        fileName, null, IntPtr.Zero, dependencies, usr, pwd);

                if (service == IntPtr.Zero)
                {
                    throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
                }
                CloseServiceHandle(service);
            }
            finally
            {
                CloseServiceHandle(scm);
            }
        }

        public static void InstallAndStart(string serviceName, string displayName, string fileName,
            string dependencies = null, string usr = null, string pwd = null)
        {
            IntPtr scm = OpenSCManager(ScmAccessRights.AllAccess);

            try
            {
                IntPtr service = OpenService(scm, serviceName, ServiceAccessRights.AllAccess);

                if (service == IntPtr.Zero)
                    service = CreateService(scm, serviceName, displayName, ServiceAccessRights.AllAccess, 
                        SERVICE_WIN32_OWN_PROCESS, ServiceBootFlag.AutoStart, ServiceError.Normal, 
                        fileName, null, IntPtr.Zero, dependencies, usr, pwd);

                if (service == IntPtr.Zero)
                {
                    throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
                }
                try
                {
                    StartService(service);
                }
                finally
                {
                    CloseServiceHandle(service);
                }
            }
            finally
            {
                CloseServiceHandle(scm);
            }
        }

        public static void StartService(string serviceName)
        {
            IntPtr scm = OpenSCManager(ScmAccessRights.Connect);

            try
            {
                IntPtr service = OpenService(scm, serviceName, ServiceAccessRights.QueryStatus | ServiceAccessRights.Start);
                if (service == IntPtr.Zero)
                {
                    throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
                }
                try
                {
                    StartService(service);
                }
                finally
                {
                    CloseServiceHandle(service);
                }
            }
            finally
            {
                CloseServiceHandle(scm);
            }
        }

        public static void StopService(string serviceName)
        {
            IntPtr scm = OpenSCManager(ScmAccessRights.Connect);

            try
            {
                IntPtr service = OpenService(scm, serviceName, ServiceAccessRights.QueryStatus | ServiceAccessRights.Stop);
                if (service == IntPtr.Zero)
                {
                    throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
                }
                try
                {
                    StopService(service);
                }
                finally
                {
                    CloseServiceHandle(service);
                }
            }
            finally
            {
                CloseServiceHandle(scm);
            }
        }

        public static void ChangeServiceDelayedAutoStart(string serviceName, bool newvalue)
        {
            IntPtr scm = OpenSCManager(ScmAccessRights.Connect);

            try
            {
                IntPtr service = OpenService(scm, serviceName, ServiceAccessRights.ChangeConfig);
                if (service == IntPtr.Zero)
                {
                    throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
                }
                try
                {
                    ChangeServiceDelayedAutoStart(service, newvalue);
                }
                finally
                {
                    CloseServiceHandle(service);
                }
            }
            finally
            {
                CloseServiceHandle(scm);
            }
        }

        public static ServiceState GetServiceStatus(string serviceName)
        {
            IntPtr scm = OpenSCManager(ScmAccessRights.Connect);

            try
            {
                IntPtr service = OpenService(scm, serviceName, ServiceAccessRights.QueryStatus);
                if (service == IntPtr.Zero)
                    return ServiceState.NotFound;

                try
                {
                    return GetServiceStatus(service);
                }
                finally
                {
                    CloseServiceHandle(service);
                }
            }
            finally
            {
                CloseServiceHandle(scm);
            }
        }

        public static SERVICE_CONFIG GetServiceConfigInfo(string serviceName)
        {
            IntPtr scm = OpenSCManager(ScmAccessRights.Connect);

            try
            {
                IntPtr service = OpenService(scm, serviceName, ServiceAccessRights.QueryConfig);
                if (service == IntPtr.Zero)
                    return null;

                try
                {
                    return GetServiceConfigInfo(service);
                }
                finally
                {
                    CloseServiceHandle(service);
                }
            }
            finally
            {
                CloseServiceHandle(scm);
            }
        }

        public static string GetValidServiceName(string serviceName)
        {
            if (serviceName.Length > byte.MaxValue)
                serviceName = serviceName.Substring(0, byte.MaxValue);

            serviceName = Regex.Replace(serviceName, NameReplace, "_").Replace('\\', '_').Replace('/', '_');

            return serviceName;
        }
        #endregion

        #region Private Methods
        private static void StartService(IntPtr service)
        {
            Exception exception = null;
            int result = StartService(service, 0, 0);
            if (result == 0)
            {
                exception = new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
            }
            var changedStatus = WaitForServiceStatus(service, ServiceState.StartPending, ServiceState.Running);
            if (!changedStatus)
                throw exception ?? new ApplicationException("Unable to start service");
        }

        private static void StopService(IntPtr service)
        {
            Exception exception = null;
            SERVICE_STATUS status = new SERVICE_STATUS();
            int result = ControlService(service, ServiceControl.Stop, status);
            if (result == 0)
            {
                exception = new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
            }
            var changedStatus = WaitForServiceStatus(service, ServiceState.StopPending, ServiceState.Stopped);
            if (!changedStatus)
                throw exception ?? new ApplicationException("Unable to stop service");
        }

        private static void ChangeServiceDelayedAutoStart(IntPtr service, bool newvalue)
        {
            SERVICE_DELAYED_AUTO_START_INFO info = new SERVICE_DELAYED_AUTO_START_INFO() { fDelayedAutostart = newvalue ? 1 : 0 };
            IntPtr lpInfo = Marshal.AllocHGlobal(Marshal.SizeOf(info));
            try
            {
                Marshal.StructureToPtr(info, lpInfo, false);
                int result = ChangeServiceConfig2(service, ServiceInfoLevel.DelayedAutoStartInfo, lpInfo);
                if (result == 0)
                {
                    throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
                }
            }
            finally
            {
                Marshal.FreeHGlobal(lpInfo);
            }
        }

        private static bool ServiceIsDelayedAutoStart(IntPtr service)
        {
            int bytesNeeded = 0;
            SERVICE_DELAYED_AUTO_START_INFO info = new SERVICE_DELAYED_AUTO_START_INFO() { fDelayedAutostart = 0 };

            int retCode = QueryServiceConfig2(service, ServiceInfoLevel.DelayedAutoStartInfo, IntPtr.Zero, 0, ref bytesNeeded);
            if (retCode == 0 && bytesNeeded == 0)
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());

            IntPtr qscPtr = Marshal.AllocCoTaskMem(bytesNeeded);
            try
            {
                retCode = QueryServiceConfig2(service, ServiceInfoLevel.DelayedAutoStartInfo, qscPtr, bytesNeeded, ref bytesNeeded);
                if (retCode == 0)
                    throw new System.ComponentModel.Win32Exception();

                info = (SERVICE_DELAYED_AUTO_START_INFO)Marshal.PtrToStructure(qscPtr, typeof(SERVICE_DELAYED_AUTO_START_INFO));

                return info.fDelayedAutostart != 0;
            }
            finally
            {
                Marshal.FreeCoTaskMem(qscPtr);
            }
        }

        private static ServiceState GetServiceStatus(IntPtr service)
        {
            SERVICE_STATUS status = new SERVICE_STATUS();

            if (QueryServiceStatus(service, status) == 0)
                throw new ApplicationException("Failed to query service status.");

            return status.dwCurrentState;
        }

        private static SERVICE_CONFIG GetServiceConfigInfo(IntPtr service)
        {
            int bytesNeeded = 0;
            var config = new SERVICE_CONFIG();

            int retCode = QueryServiceConfig(service, IntPtr.Zero, 0, ref bytesNeeded);
            if (retCode == 0 && bytesNeeded == 0)
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());

            IntPtr qscPtr = Marshal.AllocCoTaskMem(bytesNeeded);
            try
            {
                retCode = QueryServiceConfig(service, qscPtr, bytesNeeded, ref bytesNeeded);
                if (retCode == 0)
                    throw new System.ComponentModel.Win32Exception();

                config = (SERVICE_CONFIG)Marshal.PtrToStructure(qscPtr, typeof(SERVICE_CONFIG));

                return config;
            }
            finally
            {
                Marshal.FreeCoTaskMem(qscPtr);
            }
        }

        private static bool WaitForServiceStatus(IntPtr service, ServiceState waitStatus, ServiceState desiredStatus)
        {
            SERVICE_STATUS status = new SERVICE_STATUS();

            QueryServiceStatus(service, status);
            if (status.dwCurrentState == desiredStatus) return true;

            int dwStartTickCount = Environment.TickCount;
            int dwOldCheckPoint = status.dwCheckPoint;

            while (status.dwCurrentState == waitStatus)
            {
                // Do not wait longer than the wait hint. A good interval is
                // one tenth the wait hint, but no less than 1 second and no
                // more than 10 seconds.

                int dwWaitTime = status.dwWaitHint / 10;

                if (dwWaitTime < 1000) dwWaitTime = 5000;
                else if (dwWaitTime > 10000) dwWaitTime = 10000;

                Thread.Sleep(dwWaitTime);

                // Check the status again.

                if (QueryServiceStatus(service, status) == 0) break;

                if (status.dwCheckPoint > dwOldCheckPoint)
                {
                    // The service is making progress.
                    dwStartTickCount = Environment.TickCount;
                    dwOldCheckPoint = status.dwCheckPoint;
                }
                else
                {
                    if (Environment.TickCount - dwStartTickCount > status.dwWaitHint)
                    {
                        // No progress made within the wait hint
                        break;
                    }
                }
            }
            return (status.dwCurrentState == desiredStatus);
        }

        private static IntPtr OpenSCManager(ScmAccessRights rights)
        {
            IntPtr scm = OpenSCManager(null, null, rights);
            if (scm == IntPtr.Zero)
                throw new ApplicationException("Could not connect to service control manager.");

            return scm;
        }
    }
    #endregion

    #region Enumarations
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum ServiceState
    {
        Unknown = -1, // The state cannot be (has not been) retrieved.
        NotFound = 0, // The service is not known on the host server.
        Stopped = 1,
        StartPending = 2,
        StopPending = 3,
        Running = 4,
        ContinuePending = 5,
        PausePending = 6,
        Paused = 7
    }

    [Flags]
    public enum ScmAccessRights
    {
        Connect = 0x0001,
        CreateService = 0x0002,
        EnumerateService = 0x0004,
        Lock = 0x0008,
        QueryLockStatus = 0x0010,
        ModifyBootConfig = 0x0020,
        StandardRightsRequired = 0xF0000,
        AllAccess = (StandardRightsRequired | Connect | CreateService |
                     EnumerateService | Lock | QueryLockStatus | ModifyBootConfig)
    }

    [Flags]
    public enum ServiceAccessRights
    {
        QueryConfig = 0x1,
        ChangeConfig = 0x2,
        QueryStatus = 0x4,
        EnumerateDependants = 0x8,
        Start = 0x10,
        Stop = 0x20,
        PauseContinue = 0x40,
        Interrogate = 0x80,
        UserDefinedControl = 0x100,
        Delete = 0x00010000,
        StandardRightsRequired = 0xF0000,
        AllAccess = (StandardRightsRequired | QueryConfig | ChangeConfig |
                     QueryStatus | EnumerateDependants | Start | Stop | PauseContinue |
                     Interrogate | UserDefinedControl)
    }

    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum ServiceBootFlag
    {
        Start = 0x00000000,
        SystemStart = 0x00000001,
        AutoStart = 0x00000002,
        DemandStart = 0x00000003,
        Disabled = 0x00000004
    }

    public enum ServiceControl
    {
        Stop = 0x00000001,
        Pause = 0x00000002,
        Continue = 0x00000003,
        Interrogate = 0x00000004,
        Shutdown = 0x00000005,
        ParamChange = 0x00000006,
        NetBindAdd = 0x00000007,
        NetBindRemove = 0x00000008,
        NetBindEnable = 0x00000009,
        NetBindDisable = 0x0000000A
    }

    public enum ServiceError
    {
        Ignore = 0x00000000,
        Normal = 0x00000001,
        Severe = 0x00000002,
        Critical = 0x00000003
    }

    public enum ServiceInfoLevel
    {
        Description = 0x00000001,
        FailureActions = 0x00000002,
        DelayedAutoStartInfo = 0x00000003,
        FailureActionsFlag = 0x00000004,
        ServiceSidInfo = 0x00000005,
        RequiredPrivilegesInfo = 0x00000006,
        PreShutdownInfo = 0x00000007,
        TriggerInfo = 0x00000008,
        PreferedNode = 0x00000009,
        LaunchProtected = 0x000000012
    }
    #endregion

    #region EnumConverter
    public class LocalizedEnumConverter : ResourceEnumConverter
    {
        public LocalizedEnumConverter(Type type)
            : base(type, Properties.Resources.ResourceManager)
        {

        }
    }
    #endregion
}
