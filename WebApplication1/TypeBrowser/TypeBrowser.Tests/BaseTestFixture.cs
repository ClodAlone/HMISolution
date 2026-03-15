using DevExpress.Mvvm.ModuleInjection;
using DevExpress.Xpf.Core;
using NUnit.Framework;
using System.Windows;
using TypeBrowser.Main;

namespace TypeBrowser.Tests
{
    public abstract class BaseTestFixture
    {
        public IModuleManager Manager { get { return ModuleManager.DefaultManager; } }
        protected virtual bool IsFunctionalTesting { get { return false; } }
        [SetUp]
        public void SetUp()
        {
            ModuleManager.DefaultManager = new ModuleManager(false, !IsFunctionalTesting);
            TestBootstrapper.TestRun(IsFunctionalTesting);
        }
        [TearDown]
        public void TearDown()
        {
            ModuleManager.DefaultManager = null;
        }
        protected void DoEvents()
        {
            if (IsFunctionalTesting)
                DispatcherHelper.UpdateLayoutAndDoEvents(TestBootstrapper.Default.MainWindow);
        }
    }
    public class TestBootstrapper : Bootstrapper
    {
        public static new TestBootstrapper Default { get { return (TestBootstrapper)Bootstrapper.Default; } protected set { Bootstrapper.Default = value; } }
        public static void TestRun(bool showRealWindow)
        {
            Default = new TestBootstrapper(showRealWindow);
            Default.RunCore();
        }

        readonly bool showRealWindow;
        public Window MainWindow { get; private set; }
        public TestBootstrapper(bool showRealWindow)
        {
            this.showRealWindow = showRealWindow;
        }
        protected override void ShowMainWindow()
        {
            if (!showRealWindow) return;
            MainWindow = new MainWindow()
            {
                WindowStyle = WindowStyle.None,
                WindowState = WindowState.Normal,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ShowActivated = false,
                AllowsTransparency = true,
                Opacity = 0d,
                ShowInTaskbar = false,
            };
            MainWindow.Show();
        }
    }
}
