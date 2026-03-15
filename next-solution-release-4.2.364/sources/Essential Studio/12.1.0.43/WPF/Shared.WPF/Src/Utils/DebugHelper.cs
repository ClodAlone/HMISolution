// <copyright file="DebugHelper.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using System.Security.Principal;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Class that stores methods used to listen WPF traces and handle exceptions.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public static class DebugHelper
    {
        #region Private Members

        /// <summary>
        /// Stores compressed wpf trace info in memory.
        /// </summary>
        private static MemoryStream m_streamInMemoryTraceInfo = new MemoryStream(1024);

        /// <summary>
        /// Stores compressed debug traces in memory.
        /// </summary>
        private static MemoryStream m_streamInMemoryDebug = new MemoryStream(1024);

        /// <summary>
        /// Specifies trace listener for WPF traces.
        /// </summary>
        private static TraceListener m_listenerForWPFTraces;

        /// <summary>
        /// Specifies trace listener for debug trace.
        /// </summary>
        private static TraceListener m_listenerForDebug;

        /// <summary>
        /// Compressor used to compress WPF traces.
        /// </summary>
        private static GZipStream m_streamCompressedTraceInfo;

        /// <summary>
        /// Compressor used to compress debug trace.
        /// </summary>
        private static GZipStream m_streamCompressedDebugOutput;

        #endregion

        #region Public Methods

        /// <summary>
        /// Attaches listeners for WPF traces, and exception handling that is able to send mail containing all info about the exception.
        /// </summary>
        public static void Attach()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);

            PresentationTraceSources.Refresh();

            m_streamCompressedTraceInfo = new GZipStream(m_streamInMemoryTraceInfo, CompressionMode.Compress);
            m_streamCompressedDebugOutput = new GZipStream(m_streamInMemoryDebug, CompressionMode.Compress);

            m_listenerForWPFTraces = new XmlWriterTraceListener(m_streamCompressedTraceInfo);
            m_listenerForDebug = new TimedListener(m_streamCompressedDebugOutput);

            AddListener(PresentationTraceSources.DataBindingSource, m_listenerForWPFTraces, null);
            AddListener(PresentationTraceSources.MarkupSource, m_listenerForWPFTraces, null);
            AddListener(PresentationTraceSources.FreezableSource, m_listenerForWPFTraces, null);
            AddListener(PresentationTraceSources.RoutedEventSource, m_listenerForWPFTraces, null);
            AddListener(PresentationTraceSources.ResourceDictionarySource, m_listenerForWPFTraces, SourceLevels.All);
            AddListener(PresentationTraceSources.DependencyPropertySource, m_listenerForWPFTraces, null);
            AddListener(PresentationTraceSources.AnimationSource, m_listenerForWPFTraces, null);

            Debug.Listeners.Add(m_listenerForDebug);
            Trace.Listeners.Add(m_listenerForDebug);
        }
        #endregion

        #region DLL Imports

        /// <summary>
        /// Determines whether [is theme active].
        /// </summary>
        /// <returns>Return 0 or 1</returns>
        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private static extern int IsThemeActive();

        /// <summary>
        /// Gets the name of the current theme.
        /// </summary>
        /// <param name="pszThemeFileName">Name of the PSZ theme file.</param>
        /// <param name="dwMaxNameChars">The dw max name chars.</param>
        /// <param name="pszColorBuff">The PSZ color buff.</param>
        /// <param name="dwMaxColorChars">The dw max color chars.</param>
        /// <param name="pszSizeBuff">The PSZ size buff.</param>
        /// <param name="cchMaxSizeChars">The CCH max size chars.</param>
        /// <returns>Return current theme name value.</returns>
        [DllImport("uxtheme.dll", CharSet = CharSet.Auto)]
        private static extern int GetCurrentThemeName(StringBuilder pszThemeFileName, int dwMaxNameChars, StringBuilder pszColorBuff, int dwMaxColorChars, StringBuilder pszSizeBuff, int cchMaxSizeChars);
        #endregion

        #region Implementation

        /// <summary>
        /// Adds listener to the trace, removes default listener, and sets the specified trace level.
        /// </summary>
        /// <param name="trace">Trace source to customize.</param>
        /// <param name="listener">Trace listener to be added to the trace source listeners collection.</param>
        /// <param name="level">Tracing level to be set. It is defaulted to SourceLevels.Information if null.</param>
        private static void AddListener(TraceSource trace, TraceListener listener, SourceLevels? level)
        {
            SourceLevels levelInternal = level ?? SourceLevels.Information;

            trace.Listeners.Clear();
            trace.Listeners.Add(listener);
            trace.Switch.Level = levelInternal;
        }

        /// <summary>
        /// Saves traces and closes streams.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Instance that is containing event data.</param>
        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            SaveAllAndClose(null);
        }

        /// <summary>
        /// Saves compressed traces and sends mail with exception if needed.
        /// </summary>
        /// <param name="exception">Exception that caused this method to be called, can be null.</param>
        private static void SaveAllAndClose(Exception exception)
        {
            m_listenerForWPFTraces.Close();
            m_listenerForDebug.Close();

            m_streamCompressedDebugOutput.Close();
            m_streamCompressedTraceInfo.Close();

            FileStream stream = new FileStream("DebugLog.txt.gz", FileMode.Create);
            FileStream stream1 = new FileStream("PresentationFoundationTraces.xml.gz", FileMode.Create);

            byte[] data = m_streamInMemoryDebug.ToArray();
            stream.Write(data, 0, data.Length);

            data = m_streamInMemoryTraceInfo.ToArray();
            stream1.Write(data, 0, data.Length);

            stream.Close();
            stream1.Close();

            if (exception != null)
            {
                StringWriter writer = new StringWriter();
                writer.NewLine = "\n";

                using (TextWriterTraceListener exceptionInfoListener = new TextWriterTraceListener(writer))
                {
                    Debug.Listeners.Add(exceptionInfoListener);
                    WriteExceptionInfoToDebug(exception);
                    Debug.Listeners.Remove(exceptionInfoListener);
                }

                string strExceptionInfo = writer.ToString();

                SendMail(strExceptionInfo, "DebugLog.txt.gz", "PresentationFoundationTraces.xml.gz");
            }
        }

        /// <summary>
        /// Writes all information regarding exception to the debug log.
        /// </summary>
        /// <param name="exception">Given exception.</param>
        private static void WriteExceptionInfoToDebug(Exception exception)
        {
            Debug.WriteLine("Exception has been raised:");
            Debug.WriteLine(exception.ToString());

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            Debug.Indent();
            Debug.WriteLine("Loaded assemblies list:");
            Debug.Indent();

            foreach (Assembly assembly in assemblies)
            {
                string fullName = assembly.FullName;
                string[] parts = fullName.Split(',');
                string name = "[" + parts[0] + "]";
                string version = parts[1].Remove(0, parts[1].IndexOf('=') + 1);
                Debug.WriteLine(name);
                Debug.Indent();
                Debug.WriteLine(version, "Version");
                Debug.WriteLine(assembly.GlobalAssemblyCache, "From GAC");
                Debug.WriteLine(assembly.ImageRuntimeVersion, "Image runtime version");

                if (!assembly.GlobalAssemblyCache)
                {
                    Debug.WriteLine(assembly.CodeBase, "CodeBase");
                }

                Debug.Unindent();
            }

            Debug.Unindent();

            string themeName;
            string themeColor;
            StringBuilder builder1 = new StringBuilder(260);
            StringBuilder builder2 = new StringBuilder(260);

            if (GetCurrentThemeName(builder1, builder1.Capacity, builder2, builder2.Capacity, null, 0) == 0)
            {
                themeName = builder1.ToString();
                themeName = Path.GetFileNameWithoutExtension(themeName);
                themeColor = builder2.ToString();
            }
            else
            {
                themeName = themeColor = string.Empty;
            }

            bool bThemeActive = 0 != IsThemeActive();

            Debug.WriteLine("Theme info:");
            Debug.Indent();
            Debug.WriteLine(bThemeActive, "Theme active");
            Debug.WriteLine(themeName, "Current theme name");
            Debug.WriteLine(themeColor, "Current theme color scheme name");
            Debug.Unindent();
            Debug.WriteLine("System info:");
            Debug.Indent();
            Debug.WriteLine(SystemParameters.UIEffects, "All UI Effects enabled");
            Debug.WriteLine(SystemParameters.PowerLineStatus, "Power line status");
            Debug.WriteLine(SystemParameters.IsTabletPC, "Is TablePC");
            Debug.WriteLine(SystemParameters.IsSlowMachine, "Is slow machine");
            Debug.WriteLine(SystemParameters.IsRemoteSession, "Is in remote session");
            Debug.WriteLine(SystemParameters.IsRemotelyControlled, "Is remotely controlled");
            Debug.Unindent();

            Debug.Unindent();
        }

        /// <summary>
        /// Handles exception.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Instance that is containing event data.</param>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = (Exception)e.ExceptionObject;

            SaveAllAndClose(exception);
        }

        /// <summary>
        /// Sends the mail.
        /// </summary>
        /// <param name="mailBody">The mail body.</param>
        /// <param name="pathDebugLog">The path debug log.</param>
        /// <param name="pathWPFTraces">The path WPF traces.</param>
        private static void SendMail(string mailBody, string pathDebugLog, string pathWPFTraces)
        {
            WindowsIdentity user = WindowsIdentity.GetCurrent();
            string userName = user.Name;

            MailAddress addressFrom = new MailAddress("tech@syncfusion.com", "WPF Bug Tracking [" + userName + "]");
            MailAddress addressTo = new MailAddress("tech@syncfusion.com", "Daniel Jebaraj");

            Application app = Application.Current;
            Window windowMain = app.Windows.Count > 0 ? app.Windows[0] : null;

            //// TODO: move port and mail server settings to the configuration file.
            SmtpClient smtpClient = new SmtpClient("syncfusion.com", 25);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

            MailMessage message = new MailMessage(addressFrom, addressTo);
            message.Priority = MailPriority.High;
            message.Subject = "WPF Sample Crashed: " +
                ((windowMain != null) ? windowMain.Title : "Unknown");
            message.Body = mailBody;

            Attachment attachmentDebugLog = new Attachment(pathDebugLog, MediaTypeNames.Application.Zip);
            Attachment attachmentWPFTraces = new Attachment(pathWPFTraces, MediaTypeNames.Application.Zip);

            message.Attachments.Add(attachmentDebugLog);
            message.Attachments.Add(attachmentWPFTraces);

            try
            {
                smtpClient.Send(message);
            }
            catch (Exception e)
            {
                MessageBox.Show("Sorry, but the mail can not be send. [Exception: " + e.Message + "]");
            }
        }
        #endregion
    }
}
