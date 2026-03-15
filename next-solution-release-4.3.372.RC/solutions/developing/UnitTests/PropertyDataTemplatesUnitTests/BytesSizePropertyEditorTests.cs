using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using PropertyDataTemplatesUnitTests.UFUAConfigurationPropertyHelper;
using System.Windows;
using System.Diagnostics;

namespace PropertyDataTemplatesUnitTests
{
    [TestClass]
    public class BytesSizePropertyEditorTests
    {
        //[TestMethod]
        public void TestShowedValues()
        {
            using (var propertyNodes = new UFUAConfigurationPropertyNodes())
            {
                var wnd = new Window()
                {
                    Content = propertyNodes.BytesSizePropertyEditor,
                    ShowInTaskbar = false,
                    WindowStyle = WindowStyle.None,
                    Visibility = Visibility.Hidden
                };

                try
                {
                    wnd.Show();
                    var upDownControl = propertyNodes.BytesSizePropertyEditor.FindName("updown") as DevExpress.Xpf.Editors.SpinEdit;

                    var expected = GetExptectedMaxHistoryTotalSafelyFilesSizeValue(propertyNodes.UFUAConfiguration);
                    Assert.AreEqual(expected, upDownControl.Value);

                    propertyNodes.BytesSizePropertyEditor.MinValue = 0;
                    propertyNodes.BytesSizePropertyEditor.MaxValue = 100;

                    propertyNodes.UFUAConfiguration.MaxHistoryTotalSafelyFilesSize = -100;
                    Assert.AreEqual(0, upDownControl.Value);

                    propertyNodes.UFUAConfiguration.MaxHistoryTotalSafelyFilesSize = 1024;
                    Assert.AreEqual(100, upDownControl.Value);
                }
                finally
                {
                    wnd.Close();
                }
            }
        }

        //[TestMethod]
        public void TestStartValueBoundaries()
        {
            using (var propertyNodes = new UFUAConfigurationPropertyNodes())
            {
                var wnd = new Window() 
                { 
                    Content = propertyNodes.BytesSizePropertyEditor, 
                    ShowInTaskbar = false,
                    WindowStyle = WindowStyle.None,
                    Visibility = Visibility.Hidden
                };

                try
                {
                    wnd.Show();
                    var upDownControl = propertyNodes.BytesSizePropertyEditor.FindName("updown") as DevExpress.Xpf.Editors.SpinEdit;

                    propertyNodes.UFUAConfiguration.MaxHistoryTotalSafelyFilesSize = Int64.MinValue + 1;
                    var expected = GetExptectedMaxHistoryTotalSafelyFilesSizeValue(propertyNodes.UFUAConfiguration);
                    Assert.AreEqual(expected, upDownControl.Value);

                    propertyNodes.UFUAConfiguration.MaxHistoryTotalSafelyFilesSize = Int64.MaxValue;
                    expected = GetExptectedMaxHistoryTotalSafelyFilesSizeValue(propertyNodes.UFUAConfiguration);
                    Assert.AreEqual(expected, upDownControl.Value);
                }
                finally
                {
                    wnd.Close();
                }
            }
        }

        //[TestMethod]
        public void TestMemoryLeaks()
        {
            Microsoft.Test.LeakDetection.MemorySnapshot startMemorySnapshot = null;
            for (int cc = 0; cc < 2; cc++)
            {
                for (int ii = 0; ii < 100; ii++)
                {
                    using (var propertyNodes = new UFUAConfigurationPropertyNodes())
                    {
                        var wnd = new Window()
                        {
                            Content = propertyNodes.BytesSizePropertyEditor,
                            ShowInTaskbar = false,
                            WindowStyle = WindowStyle.None,
                            Visibility = Visibility.Hidden
                        };

                        wnd.Show();
                        wnd.Close();
                    }
                }

                if ((cc % 2) == 0)
                {
                    GC.Collect(GC.MaxGeneration);
                    GC.WaitForPendingFinalizers();
                    startMemorySnapshot = Microsoft.Test.LeakDetection.MemorySnapshot.FromProcess(Process.GetCurrentProcess().Id);
                }
                else
                {
                    Assert.IsNotNull(startMemorySnapshot, "Failed to get the starting snapshot");

                    if (startMemorySnapshot != null)
                    {
                        GC.Collect(GC.MaxGeneration);
                        GC.WaitForPendingFinalizers();
                        var lastMemorySnapshot = Microsoft.Test.LeakDetection.MemorySnapshot.FromProcess(Process.GetCurrentProcess().Id);
                        var comparedMemory = lastMemorySnapshot.CompareTo(startMemorySnapshot);
                        Assert.IsFalse(comparedMemory.HandleCount > 0, "Found a Handle leak of #{0}", comparedMemory.HandleCount);
                        Assert.IsFalse(comparedMemory.ThreadCount > 0, "Found a Thread leak of #{0}", comparedMemory.ThreadCount);
                        Assert.IsFalse(comparedMemory.UserObjectCount > 0, "Found a User leak of #{0}", comparedMemory.UserObjectCount);
                        Assert.IsFalse(comparedMemory.GdiObjectCount > 0, "Found a GDI leak of #{0}", comparedMemory.GdiObjectCount);
                        //Assert.IsFalse(comparedMemory.WorkingSetBytes > 0, "Found a memory leak of #{0}", comparedMemory.WorkingSetBytes);
                        startMemorySnapshot = null;
                    }
                }
            }
        }

        //[TestMethod]
        //public void TestInputValues()
        //{
        //    using (var propertyNodes = new UFUAConfigurationPropertyNodes())
        //    {
        //        var wnd = new Window()
        //        {
        //            Content = propertyNodes.BytesSizePropertyEditor,
        //            ShowInTaskbar = false,
        //            WindowStyle = WindowStyle.None,
        //            AllowsTransparency = true,
        //            Opacity = 0.0,
        //        };

        //        try
        //        {
        //            wnd.Show();
        //            Microsoft.Test.Input.Keyboard.Type(Microsoft.Test.Input.Key.NumPad0);
        //            Microsoft.Test.Input.Keyboard.Type(Microsoft.Test.Input.Key.Enter);
        //            var upDownControl = propertyNodes.BytesSizePropertyEditor.FindName("updown") as Mindscape.WpfElements.CurrencyTextBox;
        //            Assert.AreEqual(0, upDownControl.Value);
        //        }
        //        finally
        //        {
        //            wnd.Close();
        //        }
        //    }
        //}


        #region Methods
        [TestInitialize]
        public void SetDefaultXpoSettings()
        {
            XpoDefault.ConnectionString = InMemoryDataStore.GetConnectionStringInMemory(true);
        }

        decimal GetExptectedMaxHistoryTotalSafelyFilesSizeValue(UFUAModel.UFUAConfiguration ufuaConfiguration)
        {
            var maxCounter = Enum.GetValues(typeof(WPFUtilities.Converters.BytesUnit)).Length;
            var expected = System.Convert.ToDouble(ufuaConfiguration.MaxHistoryTotalSafelyFilesSize.Value);
            while (--maxCounter > 0 && (Math.Abs(expected) / 1024) > 1.0)
                expected = (expected / 1024);

            return System.Convert.ToDecimal(expected);
        }
        #endregion
    }
}
