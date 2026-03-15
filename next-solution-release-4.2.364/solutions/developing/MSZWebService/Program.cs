using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.ServiceProcess;
using Utilities.Logger;
using System.Web.Mail;

namespace MSZWebService
{
    class Program
    {
        const int MAX_TOOLTIP_CHAR_LENGTH = 64;

        static object staticLock = new object();
        static string logFile;

        static System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon();
        static Timer notifyIconTimer;
        static bool bLogExceptions = false;
        static Dictionary<string, List<DateTime>> requestPerClientMap = new Dictionary<string, List<DateTime>>();
        static Dictionary<string, DateTime> requestPerSerialMail = new Dictionary<string, DateTime>();
        static TimeSpan clientPingTimeout = Properties.Settings.Default.ClientPingTimeout;
        static TimeSpan freezeEmailAtLeastFor = Properties.Settings.Default.FreezeEmailAtLeastFor;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            logFile = AppDomain.CurrentDomain.BaseDirectory + "Log\\ServiceLog_";

            Utilities.LocalizationHelper.TryApplyCurrentLanguage();
            Utilities.RegistryKeysHelper.ReadLogException(ref bLogExceptions);

            var ServicesToRun = new MSZService(Properties.Resources.Server);

            var starting = String.Format(Properties.Resources.ServerStarting, Environment.Is64BitProcess ? "64" : "32", DateTime.Now.ToString("HH:mm:ss"));
            LogServer(starting);
            
#if !DEBUG
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
#endif
#if DEBUG
            if (!System.Diagnostics.Debugger.IsAttached &&
                Environment.UserInteractive && System.Windows.Forms.MessageBox.Show("if you would like to attach a debugger now is the right moment !", 
                    String.Format("DebugMe - {0}", ServicesToRun.ServiceName), System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                System.Diagnostics.Debugger.Launch();
#endif
            ServicesToRun.StartingService += (ob, ev) =>
            {
                #region SysTray

                notifyIcon.Visible = true;
                notifyIcon.Text = ServicesToRun.Title?.Substring(0, Math.Min(ServicesToRun.Title.Length, MAX_TOOLTIP_CHAR_LENGTH - 1));
                notifyIcon.Icon = Properties.Resources.CommNeutro;
                notifyIcon.ShowBalloonTip(5000, ServicesToRun.Title, starting, System.Windows.Forms.ToolTipIcon.Info);

                bool bBlink = false;
                var lastStatus = ServicesToRun.CommunicationStatus;

                notifyIconTimer = new Timer((o) =>
                {
                    lock (notifyIconTimer)
                    {
                        if (notifyIcon == null)
                            return;

                        var status = ServicesToRun.CommunicationStatus;
                        if (status != lastStatus)
                        {
                            lastStatus = status;
                            notifyIcon.ShowBalloonTip(5000, ServicesToRun.Title,
                                String.Format(Properties.Resources.ServerStatusChanged, ServicesToRun.StatusText),
                                System.Windows.Forms.ToolTipIcon.Info);
                        }
                        else
                            if (ServicesToRun.BalloonMessage.Length > 0)
                            {
                                notifyIcon.ShowBalloonTip(30000, ServicesToRun.Title,
                                ServicesToRun.BalloonMessage,
                                ServicesToRun.BalloonIcon);
                                ServicesToRun.BalloonMessage = String.Empty;
                            }

                        var prevIcon = notifyIcon.Icon;
                        notifyIcon.Icon = bBlink ?
                            (status ? Properties.Resources.CommOK : Properties.Resources.CommError)
                                : Properties.Resources.CommNeutro;
                        notifyIcon.Visible = true;
                        notifyIcon.Text = ServicesToRun.Title?.Substring(0, Math.Min(ServicesToRun.Title.Length, MAX_TOOLTIP_CHAR_LENGTH - 1));
                        bBlink = !bBlink;
                        if (prevIcon != null)
                            prevIcon.Dispose();
                    }
                }, null, 2000, 1000);

                #endregion
            };

            ServicesToRun.StoppingService += (o, e) =>
            {
                LogServer(string.Format(Properties.Resources.ServerStopping, DateTime.Now.ToString("HH:mm:ss")));

                if (notifyIcon != null)
                {
                    notifyIcon.ShowBalloonTip(5000, ServicesToRun.Title,
                        string.Format(Properties.Resources.ServerStopping, DateTime.Now.ToString("HH:mm:ss")),
                        System.Windows.Forms.ToolTipIcon.Info);
                }

                lock (notifyIconTimer)
                {
                    notifyIconTimer.Dispose();
                    notifyIcon.Dispose();
                    notifyIcon = null;
                }

                LogServer(Properties.Resources.ServerStopped);
            };

            bool isRedirected;

            try
            {
                isRedirected = Console.CursorVisible && false;
            }
            catch
            {
                isRedirected = true;
            }

            ServicesToRun.RequestService += (o, e) =>
            {
                string requesttype = (e as MSZRequestArgs).MSZWRequest.RequestType;
                string requestID = (e as MSZRequestArgs).MSZWRequest.RequestID;
                string requestID2 = (e as MSZRequestArgs).MSZWRequest.RequestID2; 
                string request = (e as MSZRequestArgs).MSZWRequest.Request;
                requestID = WPFUtilities.CryptString.CryptString.DecryptString(requestID);
                requestID2 = WPFUtilities.CryptString.CryptString.DecryptString(requestID2);
                request = WPFUtilities.CryptString.CryptString.DecryptString(request);

                if(request != "0")
                {
                    if (requesttype == "GggXJhcbIDBw8PDMdtXGPw==")
                    {
                        //remove old requests
                        foreach (var serial in requestPerClientMap.Keys)
                        {
                            (from date in requestPerClientMap[serial] where DateTime.Compare(date.Add(clientPingTimeout), DateTime.UtcNow) < 0 select date).ToList().ForEach(date =>
                            {
                                requestPerClientMap[serial].Remove(date);
                            });
                            if (requestPerClientMap[serial].Count == 0)
                                requestPerClientMap.Remove(serial);
                        }

                        foreach (var serial in requestPerSerialMail.Keys)
                        {
                            if (DateTime.Compare(requestPerSerialMail[request].Add(freezeEmailAtLeastFor), DateTime.UtcNow) < 0)
                                requestPerSerialMail.Remove(serial);
                        }

                        if (string.IsNullOrEmpty(request))
                            ServicesToRun.ServerKrytpState = true;
                        else if (requestPerClientMap.ContainsKey(request))
                        {
                            requestPerClientMap[request].RemoveAt(0);
                            if (requestPerClientMap[request].Count <= 0)
                                requestPerClientMap.Remove(request);
                            ServicesToRun.ServerKrytpState = false;
                        }
                        else
                            ServicesToRun.ServerKrytpState = false;
                    }
                    else if (requesttype == "aoFJf9sqeQYwONvVAvEfHg==")
                    {
                        //remove old requests
                        foreach (var serial in requestPerClientMap.Keys)
                        {
                            (from date in requestPerClientMap[serial] where DateTime.Compare(date.Add(clientPingTimeout), DateTime.UtcNow) < 0 select date).ToList().ForEach(date =>
                            {
                                requestPerClientMap[serial].Remove(date);
                            });
                            if (requestPerClientMap[serial].Count == 0)
                                requestPerClientMap.Remove(serial);
                        }

                        //add new request
                        bool bValidLicense = false;
                        if (string.IsNullOrEmpty(request))
                            ServicesToRun.ServerKrytpState = true;
                        else if (ServicesToRun.ValidSerialNumbers == null || !ServicesToRun.ValidSerialNumbers.Contains(request))
                        {
                            if (DateTime.UtcNow > (ServicesToRun.ValidSerialNumbersTimestamp.AddMilliseconds(Properties.Settings.Default.ValidSerialNumbersMaxTime)))
                            {
                                ServicesToRun.UpdateRemovedMap();
                                if (ServicesToRun.ValidSerialNumbers != null && !ServicesToRun.ValidSerialNumbers.Contains(request))
                                    ServicesToRun.ServerKrytpState = true;
                                else
                                    bValidLicense = true;
                            }
                            else
                                ServicesToRun.ServerKrytpState = true;
                        }
                        else
                            bValidLicense = true;

                        if (bValidLicense && !requestPerClientMap.ContainsKey(request))
                        {
                            requestPerClientMap[request] = new List<DateTime>() { DateTime.UtcNow };
                            ServicesToRun.ServerKrytpState = false;
                        }
                        else if (bValidLicense)
                        {
                            requestPerClientMap[request].Add(DateTime.UtcNow);
                            int warnigNumber = Properties.Settings.Default.MaxClientPerSerialWarning;
                            int alarmNumber = Properties.Settings.Default.MaxClientPerSerialAlarm;
                            int instanceNumber = Properties.Settings.Default.MaxClientPerSerial;
                            if (ServicesToRun.MultiInstanceMap != null && ServicesToRun.MultiInstanceMap.ContainsKey(request))
                                instanceNumber = ServicesToRun.MultiInstanceMap[request];

                            warnigNumber = warnigNumber + instanceNumber;
                            alarmNumber = alarmNumber + instanceNumber;
                            bool onAlert = false;
                            string message = string.Empty;
                            string subject = string.Empty;
                            if (requestPerClientMap[request].Count >= alarmNumber)
                            {
                                requestPerClientMap[request].RemoveAt(requestPerClientMap[request].Count - 1);
                                ServicesToRun.ServerKrytpState = true;
                                onAlert = true;
                                message = string.Format(Properties.Resources.LicenseDuplicated, request);
                                message = message + Environment.NewLine + $"{Properties.Resources.ReportDetails}";
                                message = message + Environment.NewLine + $"requestID: {requestID}";
                                message = message + Environment.NewLine + $"requestID2: {requestID2}";
                                subject = Properties.Resources.LicenseDuplicatedMailSubject;
                            }
                            else if (requestPerClientMap[request].Count >= warnigNumber)
                            {
                                ServicesToRun.ServerKrytpState = false;
                                onAlert = true;
                                message = string.Format(Properties.Resources.LicensePreWarningDuplicated, request);
                                message = message + Environment.NewLine + $"{Properties.Resources.ReportDetails}";
                                message = message + Environment.NewLine + $"requestID: {requestID}";
                                message = message + Environment.NewLine + $"requestID2: {requestID2}";
                                subject = Properties.Resources.LicensePreWarningDuplicatedMailSubject;
                            }
                            else
                                ServicesToRun.ServerKrytpState = false;


                            bool sendEmail = false;
                            if (onAlert)
                            {
                                if (!requestPerSerialMail.ContainsKey(request))
                                {
                                    requestPerSerialMail.Add(request, DateTime.UtcNow);
                                    sendEmail = true;
                                }
                                else if (DateTime.Compare(requestPerSerialMail[request].Add(freezeEmailAtLeastFor), DateTime.UtcNow) < 0)
                                {
                                    requestPerSerialMail[request] = DateTime.UtcNow;
                                    sendEmail = true;
                                }
                                if (sendEmail)
                                {
                                    LogServer(message);
                                    SendWarningEmail(subject, message);
                                }
                                if (!isRedirected)
                                {
                                    Console.WriteLine($"{Environment.NewLine}{Environment.NewLine}----------Exception Request----------{Environment.NewLine}");
                                    Console.WriteLine(message);
                                    Console.WriteLine($"{Environment.NewLine}-------------------------------------");
                                }
                            }
                        }
                    }
                }
                else
                    ServicesToRun.ServerKrytpState = false;

#if !DEBUG
                if (Properties.Settings.Default.EnableVerboseLog)
#endif
                {
                    if (request != "0" && (requesttype == "aoFJf9sqeQYwONvVAvEfHg==" || requesttype == "GggXJhcbIDBw8PDMdtXGPw=="))
                    {
                        StringBuilder log = new StringBuilder($"{Environment.NewLine}{Environment.NewLine}--------------Request--------------");
                        log.Append($"{Environment.NewLine}RequestType: {requesttype}");
                        log.Append($"{Environment.NewLine}RequestID: {requestID}");
                        log.Append($"{Environment.NewLine}RequestID2: {requestID2}");
                        log.Append($"{Environment.NewLine}Request: {request}");
                        log.Append($"{Environment.NewLine}--------------Result--------------");
                        log.Append($"{Environment.NewLine}ServerCrypt: {ServicesToRun.ServerKrytpState} - {DateTime.Now}");
                        log.Append($"{Environment.NewLine}Clients numbers: {requestPerClientMap.Count}");
                        if (requestPerClientMap.ContainsKey(request))
                            log.Append($"{Environment.NewLine}Request per SN:{request} => {requestPerClientMap[request].Count()}");
                        else
                            log.Append($"{Environment.NewLine}Request per SN:{request} => {0}");
                        log.Append($"{Environment.NewLine}------------------------------------------");

                        LogServer(log.ToString());

                        if (!isRedirected)
                            Console.WriteLine(log.ToString());
                    }
                }
            };

#if !DEBUG
            if (new List<String>(args).Contains("-noservice"))
#endif
            {
                var callingProcessTerminated = new ManualResetEvent(false);
                if (!isRedirected && Console.In != System.IO.StreamReader.Null)
                {
                    bool isStopping = false;
                    Console.TreatControlCAsInput = false;
                    Console.CancelKeyPress += (s, e) =>
                    {
                        e.Cancel = true;
                        if (isStopping)
                            return;
                        isStopping = true;

                        ServicesToRun.StopService();
                        callingProcessTerminated.Set();
                    };

                    ServicesToRun.StartService(args);
                    Console.WriteLine(Properties.Resources.PressEnter);
                }
                else
                    ServicesToRun.StartService(args);

                callingProcessTerminated.WaitOne();
            }
#if !DEBUG
            else
                ServiceBase.Run(ServicesToRun);
#endif
        }

        private static void SendWarningEmail(string subject, string body)
        {
            MailMessage mailObj = new MailMessage();
            mailObj.From = Properties.Settings.Default.FromMail;
            mailObj.To = Properties.Settings.Default.ToMail;
            mailObj.Subject = subject;
            mailObj.Body = body;
            mailObj.Priority = MailPriority.High;
            mailObj.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpserver", Properties.Settings.Default.SmptServer);
            mailObj.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpserverport", Properties.Settings.Default.SmptServerPort);
            mailObj.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendusing", Properties.Settings.Default.SendUsing);
            mailObj.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate", Properties.Settings.Default.SmtpAuthenticate);
            mailObj.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendusername", Properties.Settings.Default.SendUsername);
            mailObj.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendpassword", Properties.Settings.Default.SendPassword);

            SmtpMail.SmtpServer = Properties.Settings.Default.SmptServer;

            try
            {
                SmtpMail.Send(mailObj);
            }
            catch (Exception ex)
            {
               
            }
        }

        internal static void LogServer(string logmessage)
        {
            lock (staticLock)
            {
                string currlog = logFile + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
                string curfolder = System.IO.Path.GetDirectoryName(currlog);
                if (!System.IO.Directory.Exists(curfolder))
                    System.IO.Directory.CreateDirectory(curfolder);

                using (System.IO.StreamWriter writeFile = new System.IO.StreamWriter(currlog, true))
                {
                    writeFile.WriteLine(logmessage);
                }
            }
        }

#if !DEBUG
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogServer(Properties.Resources.LoggerSource + " - " + e.ExceptionObject.ToString() + " - " + System.Diagnostics.EventLogEntryType.Error + " - " + LoggerDestination.License);
            if (Environment.UserInteractive)
            {
                System.Windows.Forms.MessageBox.Show(e.ExceptionObject.ToString());
            }
        }
#endif
    }
}
