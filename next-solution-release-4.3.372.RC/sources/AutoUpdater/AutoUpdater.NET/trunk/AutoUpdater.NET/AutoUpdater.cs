using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Microsoft.Win32;

namespace AutoUpdaterDotNET
{
    /// <summary>
    ///     Enum representing the remind later time span.
    /// </summary>
    public enum RemindLaterFormat
    {
        /// <summary>
        ///     Represents the time span in minutes.
        /// </summary>
        Minutes,

        /// <summary>
        ///     Represents the time span in hours.
        /// </summary>
        Hours,

        /// <summary>
        ///     Represents the time span in days.
        /// </summary>
        Days
    }

    /// <summary>
    ///     Main class that lets you auto update applications by setting some static fields and executing its Start method.
    /// </summary>
    public static class AutoUpdater
    {
        internal static String[] Cultures = new String[]{ "it-IT", "en-US" };

        internal static String DialogTitle;

        internal static String FixlogURL;

        internal static String NewslogURL;

        internal static String DownloadURL;

        internal static String RegistryLocation;

        internal static String AppTitle;

        internal static Version CurrentVersion;

        internal static Version InstalledVersion;

        internal static bool IsWinFormsApplication;

        /// <summary>
        ///     URL of the xml file that contains information about latest version of the application.
        /// </summary>
        public static String AppCastURL;

        /// <summary>
        ///     Opens the download url in default browser if true. Very usefull if you have portable application.
        /// </summary>
        public static bool OpenDownloadPage;

        /// <summary>
        ///     Sets the current culture of the auto update notification window. Set this value if your application supports
        ///     functionalty to change the languge of the application.
        /// </summary>
        public static CultureInfo CurrentCulture;

        /// <summary>
        ///     Sets the default font to use for the dialog.
        /// </summary>
        public static System.Drawing.Font DefaultFont;

        /// <summary>
        ///     If this is true users see dialog where they can set remind later interval otherwise it will take the interval from
        ///     RemindLaterAt and RemindLaterTimeSpan fields.
        /// </summary>
        public static Boolean LetUserSelectRemindLater = true;

        /// <summary>
        ///     Remind Later interval after user should be reminded of update.
        /// </summary>
        public static int RemindLaterAt = 2;

        /// <summary>
        ///     Set if RemindLaterAt interval should be in Minutes, Hours or Days.
        /// </summary>
        public static RemindLaterFormat RemindLaterTimeSpan = RemindLaterFormat.Days;

        /// <summary>
        ///     A delegate type for hooking up update notifications.
        /// </summary>
        /// <param name="args">An object containing all the parameters recieved from AppCast XML file. If there will be an error while looking for the XML file then this object will be null.</param>
        public delegate void CheckForUpdateEventHandler(UpdateInfoEventArgs args);

        /// <summary>
        ///     An event that clients can use to be notified whenever the update is checked.
        /// </summary>
        public static event CheckForUpdateEventHandler CheckForUpdateEvent;

        /// <summary>
        ///     Start checking for new version of application and display dialog to the user if update is available.
        /// </summary>
        public static void Start()
        {
            Start(AppCastURL);
        }

        /// <summary>
        ///     Start checking for new version of application and display dialog to the user if update is available.
        /// </summary>
        /// <param name="appCast">URL of the xml file that contains information about latest version of the application.</param>
        public static void Start(String appCast)
        {
            AppCastURL = appCast;

            IsWinFormsApplication = Application.MessageLoop;

            var backgroundWorker = new BackgroundWorker();

            backgroundWorker.DoWork += BackgroundWorkerDoWork;
            backgroundWorker.RunWorkerCompleted += (o,e) => { };

            ShowFormThreadData data = new ShowFormThreadData();
            data.mainLookAndFeel = DevExpress.LookAndFeel.UserLookAndFeel.Default;
            backgroundWorker.RunWorkerAsync(data);
        }

        /// <summary>
        ///     Check for new version of application and display dialog to the user if update is available.
        /// </summary>
        /// <param name="appCast">URL of the xml file that contains information about latest version of the application.</param>
        public static bool StartSync(String appCast)
        {
            AppCastURL = appCast;

            IsWinFormsApplication = Application.MessageLoop;
            return DoWork(true);
        }

        private static void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            ShowFormThreadData td = e.Argument as ShowFormThreadData;

            if (td != null)
            {
                // this thread 
                DevExpress.UserSkins.BonusSkins.Register();
                DevExpress.LookAndFeel.UserLookAndFeel.Default.Assign(td.mainLookAndFeel);
            }

            DoWork();
        }

        private static bool DoWork(bool forceReading = false)
        {
            Assembly mainAssembly = Assembly.GetEntryAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(mainAssembly.Location);
            var mainversion = String.Format("{0}.{1}", fvi.FileMajorPart, fvi.FileMinorPart);

            var companyAttribute =
                (AssemblyCompanyAttribute) GetAttribute(mainAssembly, typeof (AssemblyCompanyAttribute));
            var titleAttribute = (AssemblyTitleAttribute) GetAttribute(mainAssembly, typeof (AssemblyTitleAttribute));
            AppTitle = titleAttribute != null ? titleAttribute.Title : mainAssembly.GetName().Name;
            string appCompany = companyAttribute != null ? companyAttribute.Company : "";

            var productAttribute =
                (AssemblyProductAttribute)GetAttribute(mainAssembly, typeof(AssemblyProductAttribute));
            var productName = productAttribute != null ? productAttribute.Product : AppTitle;

            RegistryLocation = !string.IsNullOrEmpty(appCompany)
                ? string.Format(@"Software\{0}\{1}\{2}\AutoUpdater", appCompany, productName, mainversion)
                : string.Format(@"Software\{0}\{1}\AutoUpdater", productName, mainversion);

            RegistryKey updateKey = Registry.CurrentUser.OpenSubKey(RegistryLocation);

#if !DEBUG
            if (updateKey != null)
            {
                object remindLaterTime = updateKey.GetValue("remindlater");

                if (remindLaterTime != null && !forceReading)
                {
                    DateTime remindLater = Convert.ToDateTime(remindLaterTime.ToString(),
                        CultureInfo.CreateSpecificCulture("en-US"));

                    int compareResult = DateTime.Compare(DateTime.Now, remindLater);

                    if (compareResult < 0)
                    {
                        var updateForm = new UpdateForm(true);
                        updateForm.SetTimer(remindLater);
                        CheckForUpdateEvent(null);
                        return false;
                    }
                }
            }
#endif
            InstalledVersion = new Version(fvi.FileVersion);
            WebRequest webRequest = WebRequest.Create(AppCastURL);
            webRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);

            WebResponse webResponse;

            try
            {
                webResponse = webRequest.GetResponse();
            }
            catch (Exception)
            {
                if (CheckForUpdateEvent != null)
                {
                    CheckForUpdateEvent(null);
                }
                return false;
            }

            Stream appCastStream = webResponse.GetResponseStream();

            var receivedAppCastDocument = new XmlDocument();

            if (appCastStream != null)
            {
                try
                {
                    receivedAppCastDocument.Load(appCastStream);
                }
                catch (Exception)
                {
                    if (CheckForUpdateEvent != null)
                    {
                        CheckForUpdateEvent(null);
                    }
                    return false;
                }
            }
            else
            {
                if (CheckForUpdateEvent != null)
                {
                    CheckForUpdateEvent(null);
                }
                return false;
            }
            var culture = CurrentCulture ?? Application.CurrentCulture;
            XmlNodeList appCastItems = receivedAppCastDocument.SelectNodes("item");

            if (appCastItems != null)
                foreach (XmlNode item in appCastItems)
                {
                    XmlNode appCastVersion = item.SelectSingleNode("version");
                    if (appCastVersion != null)
                    {
                        String appVersion = appCastVersion.InnerText;
                        CurrentVersion = new Version(appVersion);
                    }
                    else
                        continue;

                    XmlNode appCastTitle = item.SelectSingleNode("title");
                    XmlNode appCastMainVersion = item.SelectSingleNode("mainversion");

                    DialogTitle = string.Format(Properties.Resources.AppDialogTitle, appCastMainVersion?.InnerText, appCastTitle?.InnerText);

                    XmlNode appCastChangeLogPath = item.SelectSingleNode("changelogpath");

                    var changeLogURLPath = appCastChangeLogPath != null ? appCastChangeLogPath.InnerText : "";

                    XmlNode appCastFixLogFile = item.SelectSingleNode("fixlogfile");

                    var fixLogURLFile = appCastFixLogFile != null ? appCastFixLogFile.InnerText : "";

                    XmlNode appCastNewsLogFile = item.SelectSingleNode("newslogfile");

                    var newsLogURLFile = appCastNewsLogFile != null ? appCastNewsLogFile.InnerText : "";

                    if(Array.IndexOf(Cultures, culture.Name) >= 0)
                    {
                        FixlogURL = $"{changeLogURLPath}{culture.Name}{fixLogURLFile}";
                        NewslogURL = $"{changeLogURLPath}{culture.Name}{newsLogURLFile}";
                    }
                    else
                    {
                        FixlogURL = $"{changeLogURLPath}{fixLogURLFile}";
                        NewslogURL = $"{changeLogURLPath}{newsLogURLFile}";
                    }


                    XmlNode appCastUrl = item.SelectSingleNode("url");

                    DownloadURL = appCastUrl != null ? appCastUrl.InnerText : "";

                    if (IntPtr.Size.Equals(8))
                    {
                        XmlNode appCastUrl64 = item.SelectSingleNode("url64");

                        var downloadURL64 = appCastUrl64 != null ? appCastUrl64.InnerText : "";
                        
                        if(!string.IsNullOrEmpty(downloadURL64))
                        {
                            DownloadURL = downloadURL64;
                        }
                    }
                }

            if (updateKey != null)
            {
                object skip = updateKey.GetValue("skip");
                object applicationVersion = updateKey.GetValue("version");
                if (skip != null && applicationVersion != null)
                {
                    string skipValue = skip.ToString();
                    var skipVersion = new Version(applicationVersion.ToString());
                    if (skipValue.Equals("1") && CurrentVersion <= skipVersion)
                    {
                        CheckForUpdateEvent?.Invoke(null);
                        return false;
                    }
                    if (CurrentVersion > skipVersion)
                    {
                        RegistryKey updateKeyWrite = Registry.CurrentUser.CreateSubKey(RegistryLocation);
                        if (updateKeyWrite != null)
                        {
                            updateKeyWrite.SetValue("version", CurrentVersion.ToString());
                            updateKeyWrite.SetValue("skip", 0);
                        }
                    }
                }
                updateKey.Close();
            }

            if (CurrentVersion == null)
            {
                CheckForUpdateEvent?.Invoke(null);
                return false;
            }

            var args = new UpdateInfoEventArgs
            {
                DownloadURL = DownloadURL,
                NewslogURL = NewslogURL,
                FixlogURL = FixlogURL,
                CurrentVersion = CurrentVersion,
                InstalledVersion = InstalledVersion,
                IsUpdateAvailable = false,
            };

            if (CurrentVersion > InstalledVersion)
            {
                args.IsUpdateAvailable = true;
                if (CheckForUpdateEvent == null)
                {
                    var thread = new Thread(new ParameterizedThreadStart(Thread_ShowUIOpenForm));
                    thread.CurrentCulture = thread.CurrentUICulture = culture;
                    thread.SetApartmentState(ApartmentState.STA);
                    ShowFormThreadData data = new ShowFormThreadData();
                    data.mainLookAndFeel = DevExpress.LookAndFeel.UserLookAndFeel.Default;
                    thread.Start(data);
                }
            }

            CheckForUpdateEvent?.Invoke(args);
            return args.IsUpdateAvailable;
        }

        private static void Thread_ShowUIOpenForm(object data)
        {
            ShowFormThreadData td = data as ShowFormThreadData;

            if (td != null)
            {
                // this thread 
                DevExpress.UserSkins.BonusSkins.Register();
                DevExpress.LookAndFeel.UserLookAndFeel.Default.Assign(td.mainLookAndFeel);
            }

            var updateForm = new UpdateForm();
            updateForm.ShowDialog();
        }

        private static void ShowUI()
        {
            var updateForm = new UpdateForm();

            updateForm.ShowDialog();
        }

        private static Attribute GetAttribute(Assembly assembly, Type attributeType)
        {
            object[] attributes = assembly.GetCustomAttributes(attributeType, false);
            if (attributes.Length == 0)
            {
                return null;
            }
            return (Attribute) attributes[0];
        }

        /// <summary>
        ///     Opens the Download window that download the update and execute the installer when download completes.
        /// </summary>
        public static void DownloadUpdate()
        {
            var downloadDialog = new DownloadUpdateDialog(DownloadURL);

            try
            {
                downloadDialog.ShowDialog();
            }
            catch (TargetInvocationException)
            {
            }
        }

        internal static bool IsDarkSkin()
        {
            return DevExpress.Utils.Frames.FrameHelper.IsDarkSkin(DevExpress.LookAndFeel.UserLookAndFeel.Default);
        }
    }

    /// <summary>
    ///     Object of this class gives you all the details about the update useful in handling the update logic yourself.
    /// </summary>
    public class UpdateInfoEventArgs : EventArgs
    {
        /// <summary>
        ///     If new update is available then returns true otherwise false.
        /// </summary>
        public bool IsUpdateAvailable { get; set; }

        /// <summary>
        ///     Download URL of the update file.
        /// </summary>
        public string DownloadURL { get; set; }

        /// <summary>
        ///     URL of the webpage specifying changes in the new update.
        /// </summary>
        public string FixlogURL { get; set; }

        /// <summary>
        ///     URL of the webpage specifying changes in the new update.
        /// </summary>
        public string NewslogURL { get; set; }

        /// <summary>
        ///     Returns newest version of the application available to download.
        /// </summary>
        public Version CurrentVersion { get; set; }

        /// <summary>
        ///     Returns version of the application currently installed on the user's PC.
        /// </summary>
        public Version InstalledVersion { get; set; }
    }
}