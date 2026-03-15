using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UFUAEditor.Document;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Threading;

namespace UFUAEditor.Controls
{
    /// <summary>
    /// Interaction logic for RemoteScriptDebugger.xaml
    /// </summary>
    public partial class RemoteScriptDebugger : UserControl
    {
        readonly UFUAServerDocument Document;
        readonly Guid nodeId;
        readonly List<String> listData = new List<String>();
        readonly List<String> listDataTemp = new List<String>();
        DispatcherTimer timer;
        bool bLoaded;

        public RemoteScriptDebugger(UFUAServerDocument d, Guid id)
        {
            InitializeComponent();

            Document = d;
            nodeId = id;

#if DEBUG
            if (System.IO.File.Exists(@"C:\Program Files (x86)\Polar Engineering\WinWrap Basic\Certificates\Application-a67e0d79.htm"))
#endif
            basicIdeCtl.Secret = new Guid(Properties.Settings.Default.SecretKey);

            Loaded += (o, e) =>
                {
                    if (!bLoaded)
                    {
                        basicIdeCtl.Synchronizing += (ob, ev) =>
                        {
                            lock (listData)
                                listData.Add(ev.Param);
                        };
                        basicIdeCtl.Synchronized = true;

                        bLoaded = true;
                        Document.EnableScriptDebugging(nodeId, true);
                        StartPolling();
                    }
                };
            Unloaded += (o, e) =>
                {
                    if (bLoaded)
                    {
                        bLoaded = false;
                        StopPolling();
                        Document.EnableScriptDebugging(nodeId, false);
                    }
                };
        }

        void StartPolling()
        {
            if (timer != null)
                return;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += (o, e) =>
                {
                    lock (listData)
                    {
                        listDataTemp.Clear();
                        listDataTemp.AddRange(listData);
                        listData.Clear();
                    }
                    var ret = Document.ScriptSynchronizing(nodeId, listDataTemp);
                    if (ret != null)
                        ret.ForEach(s => basicIdeCtl.Synchronize(s, 0));
                };
            timer.Start();
        }

        void StopPolling()
        {
            if (timer == null)
                return;
            timer.Stop();
            timer = null;
        }
    }
}
