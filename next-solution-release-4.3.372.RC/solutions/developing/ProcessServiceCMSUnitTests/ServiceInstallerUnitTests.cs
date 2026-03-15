using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Principal;

namespace ProcessServiceCMSUnitTests
{
    [TestClass]
    public class ServiceInstallerUnitTests
    {
        #region Declarations
        static string serviceName = "MyServiceTest";
        static string serviceFileName = @"C:\Windows\system32\fxssvc.exe";
        #endregion

        #region Initializations
        [ClassInitialize]
        public static void EnsureInitializeAdministrativeRight(TestContext context)
        {
            WindowsPrincipal pricipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            bool hasAdministrativeRight = pricipal.IsInRole(WindowsBuiltInRole.Administrator);

            if (!hasAdministrativeRight)
            {
                Assert.Inconclusive("Visual Studio must executed as 'Run As Administrator' or UAC should be disabled!");
            }

            try
            {
                ServiceInstaller.ServiceInstaller.Uninstall(serviceName);
            }
            catch { }
        }
        #endregion

        #region Unit Tests
        [TestMethod]
        public void TestInstallUninstall()
        {
            ServiceInstaller.ServiceInstaller.Install(serviceName, serviceName, serviceFileName);
            var status = ServiceInstaller.ServiceInstaller.GetServiceStatus(serviceName);
            Assert.AreEqual(ServiceInstaller.ServiceState.Stopped, status);

            ServiceInstaller.ServiceInstaller.Uninstall(serviceName);
            status = ServiceInstaller.ServiceInstaller.GetServiceStatus(serviceName);
            Assert.AreEqual(ServiceInstaller.ServiceState.NotFound, status);
        }

        [TestMethod]
        public void TestStartStop()
        {
            ServiceInstaller.ServiceInstaller.Install(serviceName, serviceName, serviceFileName);

            //ServiceInstaller.ServiceInstaller.StartService(serviceName);
            //var status = ServiceInstaller.ServiceInstaller.GetServiceStatus(serviceName);
            //Assert.AreEqual(ServiceInstaller.ServiceState.Running, status);
            ServiceInstaller.ServiceInstaller.StopService(serviceName);
            var status = ServiceInstaller.ServiceInstaller.GetServiceStatus(serviceName);
            Assert.AreEqual(ServiceInstaller.ServiceState.Stopped, status);

            ServiceInstaller.ServiceInstaller.Uninstall(serviceName);
        }

        [TestMethod]
        public void TestConfigInfo()
        {
            ServiceInstaller.ServiceInstaller.Install(serviceName, serviceName, serviceFileName);

            var config = ServiceInstaller.ServiceInstaller.GetServiceConfigInfo(serviceName);
            Assert.AreEqual(serviceName, config.lpDisplayName);
            Assert.AreEqual(serviceFileName, config.lpBinaryPathName);
            Assert.AreEqual(String.Empty, config.lpLoadOrderGroup);
            Assert.AreEqual(String.Empty, config.lpDependencies);
            Assert.AreEqual(ServiceInstaller.ServiceError.Normal, config.dwErrorControl);
            Assert.AreEqual(ServiceInstaller.ServiceBootFlag.AutoStart, config.dwStartType);
            Assert.AreEqual("LocalSystem", config.lpServiceStartName);
            
            ServiceInstaller.ServiceInstaller.Uninstall(serviceName);
        }

        [TestMethod]
        public void TestDelayedAutoStart()
        {
            ServiceInstaller.ServiceInstaller.Install(serviceName, serviceName, serviceFileName);

            bool isDelayedAutoStart = ServiceInstaller.ServiceInstaller.ServiceIsDelayedAutoStart(serviceName);
            Assert.IsFalse(isDelayedAutoStart);

            ServiceInstaller.ServiceInstaller.ChangeServiceDelayedAutoStart(serviceName, true);
            isDelayedAutoStart = ServiceInstaller.ServiceInstaller.ServiceIsDelayedAutoStart(serviceName);
            Assert.IsTrue(isDelayedAutoStart);

            ServiceInstaller.ServiceInstaller.ChangeServiceDelayedAutoStart(serviceName, false);
            isDelayedAutoStart = ServiceInstaller.ServiceInstaller.ServiceIsDelayedAutoStart(serviceName);
            Assert.IsFalse(isDelayedAutoStart);

            ServiceInstaller.ServiceInstaller.Uninstall(serviceName);
        }
        #endregion
    }
}
