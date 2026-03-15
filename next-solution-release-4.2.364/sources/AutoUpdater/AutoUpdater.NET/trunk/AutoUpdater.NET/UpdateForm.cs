using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Microsoft.Win32;

namespace AutoUpdaterDotNET
{
    internal partial class UpdateForm : XtraForm
    {
        private System.Timers.Timer _timer;

        public UpdateForm(bool remindLater = false)
        {
            if (!remindLater)
            {
                InitializeComponent();

                SetControlImage(AutoUpdater.IsDarkSkin());
                var resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateForm));
                if (!String.IsNullOrEmpty(AutoUpdater.DialogTitle))
                    Text = AutoUpdater.DialogTitle;
                labelUpdate.Text = string.Format(resources.GetString("labelUpdate.Text", CultureInfo.CurrentCulture), AutoUpdater.AppTitle);
                labelDescription.Text =
                    string.Format(resources.GetString("labelDescription.Text", CultureInfo.CurrentCulture),
                        AutoUpdater.AppTitle, AutoUpdater.CurrentVersion, AutoUpdater.InstalledVersion);
                if (AutoUpdater.DefaultFont != null)
                {
                    Font = AutoUpdater.DefaultFont;
                    labelUpdate.Font = new System.Drawing.Font(Font, labelUpdate.Font.Style);
                    labelDescription.Font = new System.Drawing.Font(Font, labelDescription.Font.Style);
                    labelReleaseNotes.Font = new System.Drawing.Font(Font, labelReleaseNotes.Font.Style);
                    buttonSkip.Font = new System.Drawing.Font(Font, buttonSkip.Font.Style);
                    buttonRemindLater.Font = new System.Drawing.Font(Font, buttonRemindLater.Font.Style);
                    buttonUpdate.Font = new System.Drawing.Font(Font, buttonUpdate.Font.Style);
                }
            }
        }

        public override sealed string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        private void UpdateFormLoad(object sender, EventArgs e)
        {
            SetBrowserEmulation(RegistryHive.CurrentUser);
            NavigateInNewTab(AutoUpdater.FixlogURL, Properties.Resources.FixTabTitle);
            NavigateInNewTab(AutoUpdater.NewslogURL, Properties.Resources.NewsTabTitle);
        }
        private void SetControlImage(bool isDark)
        {
            if (isDark)
            {
                this.IconOptions.Image = global::AutoUpdaterDotNET.Properties.Resources.Update_Light;
                this.buttonSkip.Image = global::AutoUpdaterDotNET.Properties.Resources.hand_point_Light;
                this.buttonRemindLater.Image = global::AutoUpdaterDotNET.Properties.Resources.clock_go_32_Light;
                this.buttonUpdate.Image = global::AutoUpdaterDotNET.Properties.Resources.download_32_Light;
                this.BackgroundImage = global::AutoUpdaterDotNET.Properties.Resources.dialogWhite;
            }
            else
            {
                this.IconOptions.Image = global::AutoUpdaterDotNET.Properties.Resources.Update_Dark;
                this.buttonSkip.Image = global::AutoUpdaterDotNET.Properties.Resources.hand_point_Dark;
                this.buttonRemindLater.Image = global::AutoUpdaterDotNET.Properties.Resources.clock_go_32_Dark;
                this.buttonUpdate.Image = global::AutoUpdaterDotNET.Properties.Resources.download_32_Dark;
                this.BackgroundImage = global::AutoUpdaterDotNET.Properties.Resources.dialogDarkGray;
            }
            this.BackgroundImageLayout = ImageLayout.None;
        }
        private void NavigateInNewTab(string url, string title)
        {
            // browser tab
            DevExpress.XtraTab.XtraTabPage newBrowserTab = new DevExpress.XtraTab.XtraTabPage();

            // the underlying browser
            WebBrowser newBrowser = new WebBrowser();
            newBrowser.Dock = DockStyle.Fill;
            newBrowser.Navigated += new WebBrowserNavigatedEventHandler(newBrowser_Navigated);
            SuppressErrors(newBrowser);
            // add tab and its browser to the tab control
            newBrowserTab.Controls.Add(newBrowser);
            newBrowserTab.Text = title;
            newBrowserTab.Font = new System.Drawing.Font(new System.Drawing.FontFamily("Verdana"),12, System.Drawing.FontStyle.Bold);
            this.tabControl1.TabPages.Add(newBrowserTab);

            // navigate and focus new tab
            newBrowserTab.Select();
            newBrowser.Navigate(url);
        }

        private void newBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            SuppressErrors(sender as WebBrowser);
        }

        private void SetBrowserEmulation(RegistryHive regHive)
        {
            string path = "Software\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION";
            string appName = System.IO.Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            try
            {
                using (var hklm = Microsoft.Win32.RegistryKey.OpenBaseKey(regHive, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32))
                {
                    using (var key = hklm.OpenSubKey(path, true))
                    {
                        if (key != null)
                            key.SetValue(appName, 11001, RegistryValueKind.DWord); //emulate latest IE version
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void ButtonUpdateClick(object sender, EventArgs e)
        {
            if (AutoUpdater.OpenDownloadPage)
            {
                var processStartInfo = new ProcessStartInfo(AutoUpdater.DownloadURL);

                Process.Start(processStartInfo);
            }
            else
            {
                AutoUpdater.DownloadUpdate();
            }
        }

        private void ButtonSkipClick(object sender, EventArgs e)
        {
            RegistryKey updateKey = Registry.CurrentUser.CreateSubKey(AutoUpdater.RegistryLocation);
            if (updateKey != null)
            {
                updateKey.SetValue("version", AutoUpdater.CurrentVersion.ToString());
                updateKey.SetValue("skip", 1);
                updateKey.Close();
            }
        }

        private void ButtonRemindLaterClick(object sender, EventArgs e)
        {
            if(AutoUpdater.LetUserSelectRemindLater)
            {
                var remindLaterForm = new RemindLaterForm();

                var dialogResult = remindLaterForm.ShowDialog();

                if(dialogResult.Equals(DialogResult.OK))
                {
                    AutoUpdater.RemindLaterTimeSpan = remindLaterForm.RemindLaterFormat;
                    AutoUpdater.RemindLaterAt = remindLaterForm.RemindLaterAt;
                }
                else if(dialogResult.Equals(DialogResult.Abort))
                {
                    AutoUpdater.DownloadUpdate();
                    return;
                }
                else
                {
                    DialogResult = DialogResult.None;
                    return;
                }
            }

            RegistryKey updateKey = Registry.CurrentUser.CreateSubKey(AutoUpdater.RegistryLocation);
            if (updateKey != null)
            {
                updateKey.SetValue("version", AutoUpdater.CurrentVersion);
                updateKey.SetValue("skip", 0);
                DateTime remindLaterDateTime = DateTime.Now;
                switch (AutoUpdater.RemindLaterTimeSpan)
                {
                    case RemindLaterFormat.Days:
                        remindLaterDateTime = DateTime.Now + TimeSpan.FromDays(AutoUpdater.RemindLaterAt);
                        break;
                    case RemindLaterFormat.Hours:
                        remindLaterDateTime = DateTime.Now + TimeSpan.FromHours(AutoUpdater.RemindLaterAt);
                        break;
                    case RemindLaterFormat.Minutes:
                        remindLaterDateTime = DateTime.Now + TimeSpan.FromMinutes(AutoUpdater.RemindLaterAt);
                        break;

                }
                updateKey.SetValue("remindlater", remindLaterDateTime.ToString(CultureInfo.CreateSpecificCulture("en-US")));
                SetTimer(remindLaterDateTime);
                updateKey.Close();
            }
        }

        public void SetTimer(DateTime remindLater)
        {
            TimeSpan timeSpan = remindLater - DateTime.Now;
            _timer = new System.Timers.Timer
                {
                    Interval = (int) timeSpan.TotalMilliseconds
                };
            _timer.Elapsed += TimerElapsed;
            _timer.Start();
        }

        private void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _timer.Stop();
            AutoUpdater.Start();
        }
        private void SuppressErrors(object sender)
        {
            WebBrowser browser = (sender as WebBrowser);
            browser.ScriptErrorsSuppressed = true;

            System.Reflection.FieldInfo field = typeof(WebBrowser).GetField("_axIWebBrowser2",
                System.Reflection.BindingFlags.GetProperty |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic);
            if (field != null)
            {
                object axIWebBrowser2 = field.GetValue(browser);
                axIWebBrowser2.GetType().InvokeMember("Silent",
                    System.Reflection.BindingFlags.SetProperty, null, axIWebBrowser2, new object[] { true });
            }
        }
    }
}
