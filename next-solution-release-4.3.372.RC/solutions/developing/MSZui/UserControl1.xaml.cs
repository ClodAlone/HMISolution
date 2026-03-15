using MSZ;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using UIMsgBoxAlertService.ComponentService;
using Utilities;

namespace MSZui
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl, IDisposable
    {
        DispatcherTimer timer;
        DateTime startup;
        public bool canClose;
        int nSeconds;
        bool bloaded;

        public UserControl1(int multiplier, string msg = null)
        {
            InitializeComponent();

            Loaded += (o, e) =>
                {
                    if(!bloaded)
                    {
                        bloaded = true;
                        try
                        {
                            var list = FindAndLoadDLL.LoadDLLs<UserControl>(String.Format("{0}Splashes\\", AppDomain.CurrentDomain.BaseDirectory),
                                                                "RuntimeSplashControl.dll", false);

                            if (list.Count > 0)
                                splashContent.Content = list[0];
                        }
                        catch (Exception ex)
                        {

                        }

                        if (msg != null)
                        {
                            evaluationLabel.FontSize = 16;
                            evaluationLabel.Content = msg;
                        }

                        nSeconds = 5 * multiplier;

                        if (nSeconds > 0)
                        {
                            btnClose.Content = nSeconds.ToString();
                            startup = DateTime.Now;
                            timer = new DispatcherTimer();
                            timer.Interval = TimeSpan.FromSeconds(1);
                            timer.Tick += timer_Elapsed;
                            timer.Start();
                        }
                        else
                        {
                            canClose = true;
                            btnClose.IsEnabled = true;
                            btnClose.Content = Properties.Resources.closeText;
                        }

                    }

                };
        }

        private void timer_Elapsed(object sender, EventArgs e)
        {
            timer.Stop();
            if (bDisposed)
                return;

            btnClose.Content = (--nSeconds).ToString();
            var timeElapsed = startup + TimeSpan.FromSeconds(5);
            // canClose = DateTime.Now < timeElapsed;
            canClose = nSeconds <= 0;
            if (canClose)
            {
                btnClose.IsEnabled = true;
                btnClose.Content = Properties.Resources.closeText;
            }
            else
                timer.Start();
        }
        public String GetText()
        {
            return Properties.Resources.licenseNotFound;
        }


        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;
            if(timer != null)
            {
                timer.Stop();
                timer.Tick -= timer_Elapsed;
                timer = null;
            }
        }
        /// <summary>
        /// This is the property bound to evaluation mode/license info text
        /// </summary>
        public string ActiveLicenseText
        {
            get
            {
                Dictionary<string, string> licenseOpts = new Dictionary<string, string>();
                foreach (System.Configuration.SettingsProperty setting in Properties.Settings.Default.Properties)
                {
                    if (setting.PropertyType == typeof(string))
                        licenseOpts.Add(setting.Name, setting.DefaultValue as string);
                }
                var licenseDataModel = new LicenseDataModel() { OptionsTags = licenseOpts};
                licenseDataModel.LoadData();

                if (MSZView.CheckState())
                    return Properties.Resources.EvaluationMode;
                else
                {
                    if (!licenseDataModel.LicenseEditor)
                        return $"{Properties.Resources.FoundLicenseSerialText} {MSZView.GetSerial()}";
                }

                return string.Empty;
            }
        }
    }
}
