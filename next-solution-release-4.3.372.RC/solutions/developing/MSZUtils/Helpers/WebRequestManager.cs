using MSZServiceCMS;
using MSZUtilsServiceHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MSZUtils.Helpers
{
    internal class WebRequestManager
    {
        #region Declarations
        object lockwtcpObject = new object();
        private IMSZWUServiceCMS clientWTcp;
        private TimeSpan operationTimeOut = Properties.Settings.Default.OperationTimeOut;
        private  String RequestID1
        {
            get
            {
                return WPFUtilities.CryptString.CryptString.EncryptString($"{System.Environment.MachineName}@" +
                                                                          $"{System.Diagnostics.Process.GetCurrentProcess().Id}@" +
                                                                          $"{System.Diagnostics.Process.GetCurrentProcess().SessionId}" + '@');
            }
        }
        private  String RequestID2
        {
            get
            {
                try
                {
                    return WPFUtilities.CryptString.CryptString.EncryptString(System.Security.Principal.WindowsIdentity.GetCurrent().Name);

                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
        }
        internal  string[] allowedRequests;
        internal ClientAutenticationCredentials LoginInfo { get; set; }
        DispatcherTimer timer;

        #endregion

        #region ctor
        internal WebRequestManager()
        {
            allowedRequests = Enum.GetNames(typeof(MSZUtilsServiceHelper.RequestType));
            LoginInfo = new ClientAutenticationCredentials();
            UpdateTimer();
        }
        #endregion

        #region WebSerive
        MSZUWResponse MSZUtilsWRequest(RequestType requestType, string request)
        {
            UpdateTimer();
            MSZUWResponse response = new MSZUWResponse();
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                lock (lockwtcpObject)
                {
                    if (clientWTcp == null)
                        try
                        {
                            ChannelFactory<IMSZWUServiceCMS> myChannelFactory = new ChannelFactory<IMSZWUServiceCMS>("MSZUtilsWServiceEndPoint");
                            clientWTcp = myChannelFactory.CreateChannel();
                        }
                        catch(Exception ex)
                        {
                            OnError(ex.Message);
                            return null;
                        }
                }
                ((IContextChannel)clientWTcp).OperationTimeout = operationTimeOut;

                try
                {
                    var RequestID = Guid.NewGuid();
                    MSZUWRequest mszuiRequest = new MSZUWRequest()
                    {
                        RequestID = RequestID,
                        RequestID1 = RequestID1,
                        RequestID2 = RequestID2,
                        RequestID3 = WPFUtilities.CryptString.CryptString.EncryptString(LoginInfo.ToString()),
                        RequestType = WPFUtilities.CryptString.CryptString.EncryptString(requestType.ToString()),
                        Request = WPFUtilities.CryptString.CryptString.EncryptString(request)
                    };

                    if (clientWTcp != null)
                    {
                        response = clientWTcp.Request(mszuiRequest);
                        if (response == null)
                            return new MSZUWResponse();
                        while(response.ContinuationPoint != -1)
                        {
                            mszuiRequest.ContinuationPoint = response.ContinuationPoint;
                            var ret = clientWTcp.Request(mszuiRequest);
                            if(ret != null)
                            {
                                response.Response.AddRange(ret.Response);
                                response.ContinuationPoint = ret.ContinuationPoint;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CleanClientWTcp();
                    OnError(ex.Message);
                    return null;
                }
            }
            return response;
        }

        internal bool GetSerialHistoryLogOptions(string value, List<LogLicenceInfo> retLogInfos)
        {
            var res = MSZUtilsWRequest(RequestType.GetSerialLogOptions, value);
            if (res == null || res.Response == null || res.Response.Count == 0)
                return false;
            try
            {
                res.Response.ForEach(rawData =>
                {
                    if (string.IsNullOrEmpty(rawData))
                        return;

                    var ret = GetStringValue(rawData);
                    if (ResIsValid(ret))
                    {
                        try
                        {
                            string[] values = ret.Split(itemSeparator);
                            var option = LogLicenceInfo.FromString(ret);
                            if (option != null)
                                retLogInfos.Add(option);
                        }
                        catch (Exception)
                        {
                        }
                    }

                });
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        internal  void CloseNetworkServerTcp(bool forceclosing = false)
        {
            try
            {
                CleanClientWTcp(forceclosing);
            }
            catch
            { }
        }
        void CleanClientWTcp(bool forceclosing = false)
        {
            lock (lockwtcpObject)
            {
                if (clientWTcp != null)
                {
                    var proxy = clientWTcp as ICommunicationObject;

                    //Done with the service, let's close it.
                    try
                    {
                        if (forceclosing)
                        {
                            proxy.Abort();
                        }
                        else if (proxy.State != CommunicationState.Faulted)
                        {
                            proxy.Close();
                        }
                        else
                        {
                            proxy.Abort();
                        }
                    }
                    catch (Exception ex)
                    {
                        proxy.Abort();
                    }

                    try
                    {
                        if (clientWTcp is IDisposable)
                            (clientWTcp as IDisposable).Dispose();
                    }
                    catch (Exception ex)
                    {
                    }
                    clientWTcp = null;
                }
            }
        }
         string LocalIPAddress()
        {
            string localIP = string.Empty;
            try
            {
                IPHostEntry host;
                host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        localIP = ip.ToString();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return localIP;
        }
         string GetPublicIP()
        {
            string a4 = string.Empty;
            try
            {
                string url = "http://checkip.dyndns.org";
                System.Net.WebRequest req = System.Net.WebRequest.Create(url);
                using (System.Net.WebResponse resp = req.GetResponse())
                {
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream()))
                    {
                        string response = sr.ReadToEnd().Trim();
                        string[] a = response.Split(':');
                        string a2 = a[1].Substring(1);
                        string[] a3 = a2.Split('<');
                        a4 = a3[0];
                    }
                }
            }
            catch
            { }

            return a4;
        }
        #endregion

        #region Events
        public event EventHandler InactivityElapsed;
        protected void OnInactivityElapsed()
        {
            InactivityElapsed?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler<ErrorEventArgs> Error;
        protected void OnError(string error)
        {
            Error?.Invoke(this, new ErrorEventArgs() { ErrorMessage = error });
        }
        public class ErrorEventArgs : EventArgs
        {
            public string ErrorMessage { get; set; }
        }
        #endregion

        #region Methods
        object lockobject = new object();
        void UpdateTimer()
        {
            bool exitTimer = false;
            lock(lockobject)
            {
                exitTimer = bTimerElapsed;
            }

            if (exitTimer)
                return;
            if (timer == null)
            {
                timer = new DispatcherTimer();
                timer.Tick += timer_Tick;
                timer.Interval = Properties.Settings.Default.ExpiredLogin;
                timer.Start();
            }
            else
            {
                timer.Stop();
                timer.Start();
            }
        }

        internal LicenceType UpdateLicTypeOptions(LicenceType licType, List<BoolOption> bOptions, List<IntOption> iOptions)
        {
            StringBuilder sBOptions = new StringBuilder();
            bOptions?.ForEach(o =>
            {
                if (sBOptions.Length > 0)
                    sBOptions.Append(lineSeparator);
                sBOptions.Append(o.ToString());
            });
            StringBuilder sIOptions = new StringBuilder();
            iOptions?.ForEach(o =>
            {
                if (sIOptions.Length > 0)
                    sIOptions.Append(lineSeparator);
                sIOptions.Append(o.ToString());
            });
            try
            {
                var res = MSZUtilsWRequest(RequestType.InserOrUpdateLicTypeOptions, $"{licType.ToString()}{optionsSeparator}{sBOptions.ToString()}{optionsSeparator}{sIOptions.ToString()}");
                if (res == null || res.Response == null || res.Response.Count == 0)
                    return null;
                if (string.IsNullOrEmpty(res.Response[0]))
                    return null;
                var ret = GetStringValue(res.Response[0]);
                if (ResIsValid(ret))
                    return LicenceType.FromString(ret);
                else
                    return null;
            }
            catch (Exception)
            {
            }
            return null;
        }

        bool bTimerElapsed;
        private void timer_Tick(object sender, EventArgs e)
        {
            lock (lockobject)
            {
               if (bTimerElapsed)
                    return;
                bTimerElapsed = true;
                if (timer != null)
                {
                    timer.Stop();
                    timer.Tick -= timer_Tick;
                    timer = null;
                }
            }
            OnInactivityElapsed();
        }

        internal  bool ValidateUser(ClientAutenticationCredentials loginInfo)
        {
            LoginInfo.UserName = loginInfo.UserName;
            LoginInfo.Password = loginInfo.Password;
            LoginInfo.UserID = -1;
            LoginInfo.UserType = -1;
            int ret = -1;
            int retType = -1;
            var res = MSZUtilsWRequest(RequestType.UserLogOn, string.Empty);
            if (res == null || res.Response == null || res.Response.Count == 0)
                return false;
            try
            {
                string response = GetStringValue(res.Response[0]);
                if(ResIsValid(response))
                {
                    string[] values = response.Split('|');
                    int.TryParse(values[0], out ret);
                    int.TryParse(values[1], out retType);
                    if (ret != -1)
                    {
                        LoginInfo.UserID = loginInfo.UserID = ret;
                        LoginInfo.UserType = loginInfo.UserType = retType;
                        return true;
                    }
                    else
                    {
                        LoginInfo = loginInfo = new ClientAutenticationCredentials();
                        return false;
                    }
                }
                else
                {
                    LoginInfo = loginInfo = new ClientAutenticationCredentials();
                    return false;
                }
            }
            catch (Exception)
            {
            }
            LoginInfo = loginInfo = new ClientAutenticationCredentials();
            return false;
        }

        internal  int GetNextSerialAvailable()
        {
            try
            {
                string request = string.Empty;
                int ret = -1;
                var res = MSZUtilsWRequest(RequestType.InitSerial, string.Empty);
                if (res == null || res.Response == null || res.Response.Count == 0 || string.IsNullOrEmpty(res.Response[0]))
                    return -1;

                string response = GetStringValue(res.Response[0]);
                if (ResIsValid(response))
                {
                    if(int.TryParse(response, out ret))
                        return ret;
                    else
                        return -1;
                }
                else
                    return -1;
            }
            catch
            {
                return -1;
            }
        }
        internal static string GetStringValue(string value)
        {
            return WPFUtilities.CryptString.CryptString.DecryptString(value);
        }
        internal  bool ResIsValid(string res)
        {
            return !ResIsInError(res) && !ResIsIllegal(res);
        }

        internal  bool ResIsInError(string res)
        {
            return res.StartsWith(RequestType.Error.ToString());
        }

        internal  bool ResIsIllegal(string res)
        {
            return res.StartsWith(RequestType.Illegal.ToString());
        }

        internal  void LogOffUser()
        {
            MSZUtilsWRequest(RequestType.UserLogOff, string.Empty);
            LoginInfo = new ClientAutenticationCredentials();
        }
        const char optionsSeparator = '☺';
        const char lineSeparator = '↨';
        const char itemSeparator = '§';
        internal List<LicenceType> GetLicTypeList(bool hiddenIncluded = false)
        {
            List<LicenceType> retLicenceType = new List<LicenceType>();
            var res = MSZUtilsWRequest(RequestType.GetLicTypeList, string.Empty);
            if (res == null || res.Response == null || res.Response.Count == 0)
                return retLicenceType;

            try
            {
                res.Response.ForEach(rawData =>
                {
                    if (string.IsNullOrEmpty(rawData))
                        return;

                    var ret = GetStringValue(rawData);
                    if (ResIsValid(ret))
                    {
                        try
                        {
                            var lic = LicenceType.FromString(ret);
                            if (hiddenIncluded && lic.Hidden || !lic.Hidden)
                                retLicenceType.Add(lic);
                        }
                        catch (Exception)
                        {
                        }
                    }

                });
            }
            catch (Exception)
            {
            }

            return retLicenceType;
        }

        internal List<IntOption> GetNumericOptions(int idType)
        {
            List<IntOption> retOptions = new List<IntOption>();
            var res = MSZUtilsWRequest(RequestType.GetNumericOptionsList, idType.ToString());
            if (res == null || res.Response == null || res.Response.Count == 0)
                return retOptions;

            try
            {
                res.Response.ForEach(rawData =>
                {
                    if (string.IsNullOrEmpty(rawData))
                        return;

                    var ret = GetStringValue(rawData);
                    if (ResIsValid(ret))
                    {
                        try
                        {
                            retOptions.Add(IntOption.FromString(ret));
                        }
                        catch (Exception)
                        {
                        }
                    }

                });
            }
            catch (Exception)
            {
            }
            return retOptions;
        }

        internal List<BoolOption> GetBoolOptions(int idType)
        {
            List<BoolOption> retOptions = new List<BoolOption>();
            var res = MSZUtilsWRequest(RequestType.GetBoolOptionsList, idType.ToString());
            if (res == null || res.Response == null || res.Response.Count == 0)
                return retOptions;
            try
            {
                res.Response.ForEach(rawData =>
                {
                    if (string.IsNullOrEmpty(rawData))
                        return;

                    var ret = GetStringValue(rawData);
                    if (ResIsValid(ret))
                    {
                        try
                        {
                            retOptions.Add(BoolOption.FromString(ret));
                        }
                        catch (Exception)
                        {
                        }
                    }

                });
            }
            catch (Exception)
            {
            }
            return retOptions;
        }
        internal bool InsertOrUpdateCustomer(Customer customer)
        {
            var res = MSZUtilsWRequest(RequestType.InsertOrUpdateCustomer, customer.ToString());
            if (res == null || res.Response == null)
                return true;
            return false;
        }
        internal List<Customer> GetCustomerList()
        {
            List<Customer> retCustomers = new List<Customer>();
            var res = MSZUtilsWRequest(RequestType.GetCustomerList, string.Empty);
            if (res == null || res.Response == null || res.Response.Count == 0)
                return retCustomers;
            try
            {
                res.Response.ForEach(rawData =>
                {
                    if (string.IsNullOrEmpty(rawData))
                        return;

                    var ret = GetStringValue(rawData);
                    if (ResIsValid(ret))
                    {
                        try
                        {
                            retCustomers.Add(Customer.FromString(ret));
                        }
                        catch (Exception)
                        {
                        }
                    }

                });
            }
            catch (Exception)
            {
            }
            return retCustomers;
        }

        internal List<LicenceInfo> GetSerialListInfo()
        {
            List<LicenceInfo> retInfo = new List<LicenceInfo>();
            var res = MSZUtilsWRequest(RequestType.GetSerialListInfo, string.Empty);
            if (res == null || res.Response == null || res.Response.Count == 0)
                return retInfo;
            try
            {
                res.Response.ForEach(rawData =>
                {
                    if (string.IsNullOrEmpty(rawData))
                        return;

                    var ret = GetStringValue(rawData);
                    if (ResIsValid(ret))
                    {
                        try
                        {
                            retInfo.Add(LicenceInfo.FromString(ret));
                        }
                        catch (Exception ex)
                        {
                        }
                    }

                });
            }
            catch (Exception)
            {
            }
            return retInfo;
        }
        internal List<BoolOption> InitBoolOptions()
        {
            List<BoolOption> retOptions = new List<BoolOption>();
            var res = MSZUtilsWRequest(RequestType.GetBoolOptionsList, string.Empty);
            if (res == null || res.Response == null || res.Response.Count == 0)
                return retOptions;
            try
            {
                res.Response.ForEach(rawData =>
                {
                    if (string.IsNullOrEmpty(rawData))
                        return;

                    var ret = GetStringValue(rawData);
                    if (ResIsValid(ret))
                    {
                        try
                        {
                            retOptions.Add( BoolOption.FromString(ret));
                        }
                        catch (Exception)
                        {
                        }
                    }

                });
            }
            catch (Exception)
            {
            }
            return retOptions;
        }

        internal List<IntOption> InitNumericOptions()
        {
            List<IntOption> retOptions = new List<IntOption>();
            var res = MSZUtilsWRequest(RequestType.GetNumericOptionsList, string.Empty);
            if (res == null || res.Response == null || res.Response.Count == 0)
                return retOptions;
            try
            {
                res.Response.ForEach(rawData =>
                {
                    if (string.IsNullOrEmpty(rawData))
                        return;

                    var ret = GetStringValue(rawData);
                    if (ResIsValid(ret))
                    {
                        try
                        {
                            retOptions.Add(IntOption.FromString(ret));
                        }
                        catch (Exception)
                        {
                        }
                    }

                });
            }
            catch (Exception)
            {
            }
            return retOptions;
        }

        internal LicenceInfo GetSerialInfo(string value)
        {
            LicenceInfo retInfo = null;
            var res = MSZUtilsWRequest(RequestType.GetSerialInfo, value);
            if (res == null || res.Response == null || res.Response.Count == 0)
                return retInfo;
            try
            {
                res.Response.ForEach(rawData =>
                {
                    if (string.IsNullOrEmpty(rawData))
                        return;

                    var ret = GetStringValue(rawData);
                    if (ResIsValid(ret))
                    {
                        try
                        {
                            retInfo = LicenceInfo.FromString(ret);
                        }
                        catch (Exception)
                        {
                        }
                    }

                });
            }
            catch (Exception)
            {
            }
            return retInfo;
        }

        internal bool GetSerialOptions(string value, List<BoolOption> retBOptions, List<IntOption> retNOptions)

        {
            var res = MSZUtilsWRequest(RequestType.GetSerialOptions, value);
            if (res == null || res.Response == null || res.Response.Count == 0)
                return false;
            try
            {
                res.Response.ForEach(rawData =>
                {
                    if (string.IsNullOrEmpty(rawData))
                        return;

                    var ret = GetStringValue(rawData);
                    if (ResIsValid(ret))
                    {
                        try
                        {
                            string[] values = ret.Split(itemSeparator);
                            var bOption = retBOptions.Find(x => x.ID == int.Parse(values[0]));
                            var nOption = retNOptions.Find(x => x.ID == int.Parse(values[0]));
                            if (bOption != null)
                                bOption.Value = bool.Parse(values[1]);
                            if (nOption != null)
                                nOption.Value = uint.Parse(values[1]);
                        }
                        catch (Exception)
                        {
                        }
                    }

                });
            }
            catch (Exception)
            {
            }
            return true;
        }

        internal LicenceInfo UpdateSerialOptions(LicenceInfo _licenseInfo, List<BoolOption> bOptions, List<IntOption> iOptions)
        {
            StringBuilder sBOptions = new StringBuilder();
            bOptions?.ForEach(o =>
            {
                if (sBOptions.Length > 0)
                    sBOptions.Append(lineSeparator);
                sBOptions.Append(o.ToString());
            });
            StringBuilder sIOptions = new StringBuilder();
            iOptions?.ForEach(o =>
            {
                if (sIOptions.Length > 0)
                    sIOptions.Append(lineSeparator);
                sIOptions.Append(o.ToString());
            });
            try
            {
                var res = MSZUtilsWRequest(RequestType.InserOrUpdateSerialOptions, $"{_licenseInfo.ToString()}{optionsSeparator}{sBOptions.ToString()}{optionsSeparator}{sIOptions.ToString()}");
                if (res == null || res.Response == null || res.Response.Count == 0)
                    return null;
                if (string.IsNullOrEmpty(res.Response[0]))
                    return null;
                var ret = GetStringValue(res.Response[0]);
                if (ResIsValid(ret))
                    return LicenceInfo.FromString(ret);
                else
                    return null;
            }
            catch (Exception)
            {
            }
            return null;
        }

        internal List<AreaGeoID> GetAreeGeoIDList()
        {
            List<AreaGeoID> retAreaGeoIds = new List<AreaGeoID>();
            var res = MSZUtilsWRequest(RequestType.GetAreeGeoIDList, string.Empty);
            if (res == null || res.Response == null || res.Response.Count == 0)
                return retAreaGeoIds;
            try
            {
                res.Response.ForEach(rawData =>
                {
                    if (string.IsNullOrEmpty(rawData))
                        return;

                    var ret = GetStringValue(rawData);
                    if (ResIsValid(ret))
                    {
                        try
                        {
                            retAreaGeoIds.Add(AreaGeoID.FromString(ret));
                        }
                        catch (Exception)
                        {
                        }
                    }

                });
            }
            catch (Exception)
            {
            }
            return retAreaGeoIds;
        }
        #endregion
    }
}
