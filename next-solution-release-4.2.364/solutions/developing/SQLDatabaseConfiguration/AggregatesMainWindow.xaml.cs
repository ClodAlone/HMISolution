using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SQLDatabaseConfiguration
{
    /// <summary>
    /// Interaction logic for AggregatesMainWindow.xaml
    /// </summary>
    public partial class AggregatesMainWindow : DXWindow
    {
        readonly int callingProcessId;

        public AggregatesMainWindow() : 
            this(default(int))
        { }

        public AggregatesMainWindow(int callingProcessId)
        {
            InitializeComponent();

            this.callingProcessId = callingProcessId;

            Loaded += (o, e) =>
            {
                WPFUtilities.ThemeHelper.SetTheme(this);
                SubscribeToCallingProcessShutdown();
            };
        }

        void SubscribeToCallingProcessShutdown()
        {
            if (callingProcessId > 0)
            {
                try
                {
                    var process = Process.GetProcessById(callingProcessId);
                    if (process != null)
                    {
                        process.EnableRaisingEvents = true;
                        process.Exited += (o, e) =>
                        {
                            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                (Action)(() => Application.Current.MainWindow.Close()));
                        };
                    }
                }
                catch { }
            }
        }
    }
}
