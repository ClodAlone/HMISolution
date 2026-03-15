using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Threading;

namespace MSZUnitTests
{
    [TestClass]
    public class DemoModeTests
    {
        #region Declarations
        private static string[] modulesNotAvailableInDemoMode = 
        { 
            "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxQ+LlYKBrULWLfRbHkuX7FA=="/* RT */, 
            "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxWuNSo2Dv1fX3PHz66oYOgA=="/* DEV */,
            "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxbsDOBmBOlIRVAC/8OSgHDQ=="/* NET */,
            "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxNGBzfZpWLG9DRUXidtfCTQ=="/* SN */,
            "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxSRSfdPty4M/h8yKtCnKfpw=="/* CMD */
        };

        private static string[] modulesAvailableInDemoMode = 
        {
            "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxJ7DsSf4A330xNg3Xqv3ofQ==" /* SVR */,
            "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxIRk/KJdyld2yhb2z/b0ClA==" /* DLR */,
            "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxLpZ9HLAzv1ul2RsoVlDyXw==" /* RCP */,
            "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxN+rOYP4u2+aLWnhBhIXJ4A==" /* VB */,
            "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNx214/TMTBEjcA8SkKPNVPhw==" /* SCD */,
            "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNx4d7+VUFwiyVTcSDJrU32bg==" /* NTW */,
            "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNx0C2Ztv8mu5UryF60CTN1ow==" /* RED */,
            "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNx+myJYPxeGnrgbhGui/FFig==" /* GEO */,
            "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNx+n8+5uFpK3i1pkn2kvC+hQ==" /* G3D */,
            "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxcPspvRaFavRuuz2KyDjJ8w==" /* REP */,
            "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxkOOLPaEzFVSCMAw5TuHTDA==" /* DIS */,
            "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNx/IoqCaj13T3tqZw/KR6gYQ==" /* STA */,
            "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxfxav9X3tB+AOBzr/4DXPTMoN95thR8kU5zJrBXMgW9A=" /* OUAS */,
            "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxvQ+f/dCo/tfcUQt0VSH97CjTYKwLVt2qrQp2IkW/O4s=" /* WDEP */,
            "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxQ0HNh4VycfQXLY5Z6lNe+QBGr0uKoS0SZW/4j3jqDSg=" /* WCL5 */,
            "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxnwyoQ0Rxy6YF77hEjYEW8A==" /* WCL */,
            "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxxJm6Pl2n3HEE5KHmXWpASw==" /* STG */,
            "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxc85bCd+oTeUhbJmLGsEXBw==" /* CTG */,
            "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNx8EE1k87k7s2L4ud471OrRg==" /* DRV */,
            "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNx47GuMgQbSRsJkASLqdvNn+GZ5/UKoeW6jV/sPhaKu1I=" /* CHLD */,
            "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxJeSCTSIQynLFbrlTy2hpfA==" /* SCR */,
            "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxNLOWKl78+jQgKGUlcim5yQ==" /* ALR */
        };
        #endregion 

        #region Generals
        public void EnsureDemoMode()
        {
            var result = MSZ.MSZView.CheckState(true);
            if (!result)
                Assert.Inconclusive("This test require not any harware/software license installed!");
        }
        #endregion 

        [TestMethod]
        public void TestDemoMode()
        {
            EnsureDemoMode();
            var result = MSZ.MSZView.CheckState(false);
            Assert.IsTrue(result);
            var demoMaxIntValue = MSZ.MSZView.GetDemoMaxIntValue();
            Assert.IsTrue(demoMaxIntValue > 0);
            var keyData = MSZ.MSZView.GetKeyData();
            Assert.IsTrue(keyData == String.Empty);
            var netState = MSZ.MSZView.GetNetState();
            Assert.IsTrue(netState == String.Empty);
            var ret = MSZ.MSZView.GetSerial();
            var serial = Convert.ToInt32(ret);
            Assert.IsTrue(serial == 0);
        }

        #region Test for 'MSZ.MSZView.GetModule()' method
        [TestMethod]
        public void TestInvalidModuleInDemoMode()
        {
            EnsureDemoMode();
            var demoMaxIntValue = MSZ.MSZView.GetDemoMaxIntValue();
            var value = MSZ.MSZView.GetModule("InvalidModuleCode");
            Assert.AreEqual(demoMaxIntValue, value);
        }

        [TestMethod]
        public void TestModuleNotAvailableInDemoMode()
        {
            EnsureDemoMode();
            for (int ii = 0; ii < modulesNotAvailableInDemoMode.Length; ii++)
            {
                var module = modulesNotAvailableInDemoMode[ii];
                var value = MSZ.MSZView.GetModule(module);
                Assert.IsTrue(value == 0, "The Module '{0}' shouldn't be available in demo mode", WPFUtilities.CryptString.CryptString.DecryptString(module));
            }
        }

        [TestMethod]
        public void TestModuleAvailableInDemoMode()
        {
            EnsureDemoMode();
            var demoMaxIntValue = MSZ.MSZView.GetDemoMaxIntValue();
            for (int ii = 0; ii < modulesAvailableInDemoMode.Length; ii++)
            {
                var module = modulesAvailableInDemoMode[ii];
                var value = MSZ.MSZView.GetModule(module);
                Assert.AreEqual(demoMaxIntValue, value, "The Module '{0}' should be available in demo mode", WPFUtilities.CryptString.CryptString.DecryptString(module));
            }
        }
        #endregion

        #region Test for 'MSZ.MSZView.GetModules()' method
        public void TestInvalidModulesInDemoMode()
        {
            EnsureDemoMode();
            var value = MSZ.MSZView.GetModules("InvalidModuleCode");
            Assert.IsTrue(value);
        }

        [TestMethod]
        public void TestModulesNotAvailableInDemoMode()
        {
            EnsureDemoMode();
            for (int ii = 0; ii < modulesNotAvailableInDemoMode.Length; ii++)
            {
                var module = modulesNotAvailableInDemoMode[ii];
                var value = MSZ.MSZView.GetModules(module);
                Assert.IsFalse(value, "The Module '{0}' shouldn't be available in demo mode", WPFUtilities.CryptString.CryptString.DecryptString(module));
            }
        }

        [TestMethod]
        public void TestModulesAvailableInDemoMode()
        {
            EnsureDemoMode();
            for (int ii = 0; ii < modulesAvailableInDemoMode.Length; ii++)
            {
                var module = modulesAvailableInDemoMode[ii];
                var value = MSZ.MSZView.GetModules(module);
                Assert.IsTrue(value, "The Module '{0}' should be available in demo mode", WPFUtilities.CryptString.CryptString.DecryptString(module));
            }
        }
        #endregion

        #region Test for 'MSZ.MSZView.GetMultiModule()' method
        [TestMethod]
        public void TestInvalidMultiModuleInDemoMode()
        {
            EnsureDemoMode();
            Assert.Inconclusive("TODO");
        }

        [TestMethod]
        public void TestMultiModuleNotAvailableInDemoMode()
        {
            EnsureDemoMode();
            Assert.Inconclusive("TODO");
        }

        [TestMethod]
        public void TestMultiModuleAvailableInDemoMode()
        {
            EnsureDemoMode();
            Assert.Inconclusive("TODO");
        }
        #endregion

        #region Test for 'MSZ.MSZView.GetMultiModules()' method
        [TestMethod]
        public void TestInvalidMultiModulesInDemoMode()
        {
            EnsureDemoMode();
            Assert.Inconclusive("TODO");
        }

        [TestMethod]
        public void TestMultiModulesNotAvailableInDemoMode()
        {
            EnsureDemoMode();
            Assert.Inconclusive("TODO");
        }

        [TestMethod]
        public void TestMultiModulesAvailableInDemoMode()
        {
            EnsureDemoMode();
            Assert.Inconclusive("TODO");
        }
        #endregion

        #region Test Working Thread
        [TestMethod]
        public void TestStartStopWorkingThread()
        {
            EnsureDemoMode();
            Parallel.For(0, 10, (ii) =>
            {
                //var result = MSZ.MSZView.Init();
                //Assert.IsTrue(result);
                var result = MSZ.MSZView.ThreadRunning();
                Assert.IsTrue(result);
                MSZ.MSZView.Read();
            });

            //Parallel.For(0, 10, (ii) =>
            //{
            //    MSZ.MSZView.Terminate();
            //    var result = MSZ.MSZView.ThreadRunning();
            //    Assert.IsFalse(result);
            //    MSZ.MSZView.Close();
            //});
        }

        [TestMethod]
        public void TestKeyChangeEvent()
        {
            EnsureDemoMode();

            int counter = 0;
            MSZ.MSZView.KeyChangeEvent += (o, e) =>
            {
                Assert.IsTrue(e.MSZState);
                counter++;
            };

            var task = Task.Run(() => 
            {
                MSZ.MSZView.Read();
                System.Threading.Thread.Sleep(5000);
            });
            task.Wait();

            Assert.IsTrue(counter > 0);
        }
        #endregion
    }
}
