using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;

namespace WPFScreenSinkTest
{
    [TestClass]
    public class WPFScreenSinkTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            using (var screenSink = new WPFScreenSink.ScreenSink())
            {
                var uri = new Uri(@"E:\Documents\test\VentanniProject1\Screen\Main.xaml",
                    UriKind.RelativeOrAbsolute);
                var ret = screenSink.OpenUri(uri, new Size(500, 400));
                Assert.IsNotNull(ret);
                Assert.IsTrue(ret.Count > 0);
                foreach (var guid in ret)
                {
                    Assert.IsNotNull(screenSink.GetImageBase64(guid));
                }

                screenSink.SimulateEvent(uri, new Point(5, 5), System.Windows.Input.MouseButton.Left, true);
            }
        }
    }
}
