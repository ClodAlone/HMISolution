using NUnit.Framework;
using System.Threading;
using TypeBrowser.Common;
using AppModules = TypeBrowser.Common.Modules;

namespace TypeBrowser.Tests
{
    [TestFixture]
    public class NavigationTests : BaseTestFixture
    {
        [Test]
        public void InitialModules()
        {
            Assert.IsNotNull(Manager.GetRegion(Regions.MainWindow).GetViewModel(AppModules.Main));
            Assert.IsNotNull(Manager.GetRegion(Regions.Navigation).GetViewModel(AppModules.Module1));
            Assert.IsNotNull(Manager.GetRegion(Regions.Navigation).GetViewModel(AppModules.Module2));
            Assert.IsNull(Manager.GetRegion(Regions.Documents).GetViewModel(AppModules.Module1));
            Assert.IsNull(Manager.GetRegion(Regions.Documents).GetViewModel(AppModules.Module2));
        }
        [Test]
        public void Navigation()
        {
            Assert.IsNull(Manager.GetRegion(Regions.Documents).GetViewModel(AppModules.Module1));
            Assert.IsNull(Manager.GetRegion(Regions.Documents).GetViewModel(AppModules.Module2));

            Manager.Navigate(Regions.Navigation, AppModules.Module1);
            Assert.IsNotNull(Manager.GetRegion(Regions.Documents).GetViewModel(AppModules.Module1));
            Assert.IsNull(Manager.GetRegion(Regions.Documents).GetViewModel(AppModules.Module2));

            Manager.Navigate(Regions.Navigation, AppModules.Module2);
            Assert.IsNotNull(Manager.GetRegion(Regions.Documents).GetViewModel(AppModules.Module1));
            Assert.IsNotNull(Manager.GetRegion(Regions.Documents).GetViewModel(AppModules.Module2));

            Manager.Navigate(Regions.Documents, AppModules.Module1);
            Assert.AreEqual(AppModules.Module1, Manager.GetRegion(Regions.Navigation).SelectedKey);
        }
    }
    [TestFixture, Category("Functional"), Apartment(ApartmentState.STA)]
    public class FunctionalNavigationTests : BaseTestFixture
    {
        protected override bool IsFunctionalTesting { get { return true; } }
        [Test]
        public void Navigation()
        {
            Manager.Navigate(Regions.Navigation, AppModules.Module1);
            DoEvents();
            var document1 = (IDocumentModule)Manager.GetRegion(Regions.Documents).GetViewModel(AppModules.Module1);
            Assert.IsTrue(document1.IsActive);

            Manager.Navigate(Regions.Navigation, AppModules.Module2);
            DoEvents();
            var document2 = (IDocumentModule)Manager.GetRegion(Regions.Documents).GetViewModel(AppModules.Module2);
            Assert.IsFalse(document1.IsActive);
            Assert.IsTrue(document2.IsActive);

            document1.IsActive = true;
            DoEvents();
            Assert.IsTrue(document1.IsActive);
            Assert.IsFalse(document2.IsActive);
            Assert.AreEqual(AppModules.Module1, Manager.GetRegion(Regions.Navigation).SelectedKey);
        }
    }
}