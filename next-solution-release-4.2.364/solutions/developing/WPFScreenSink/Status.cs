using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading;
using Utilities;
using System.Xml;
using System.Windows.Markup;
using System.Windows.Controls;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Automation.Peers;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using ScreenSettings;
using System.Windows.Media.Animation;
using VFS;

namespace WPFScreenSink
{
    public class Status
    {
        public bool connected { get; set; }
        public bool simulateEvent { get; set; }
        public bool writable { get; set; }
        public bool hasImage { get; set; }
        public String LastMessage { get; set; }
        public int dataType { get; set; }
        public List<String> selection { get; set; }

        public String value { get; set; }
        public double minValue { get; set; }
        public double maxValue { get; set; }

        public bool hasValueProvider { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}
