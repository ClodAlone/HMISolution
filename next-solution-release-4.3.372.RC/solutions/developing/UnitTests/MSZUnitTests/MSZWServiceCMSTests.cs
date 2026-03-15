using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using MSZServiceCMS;
using System.ServiceModel;
using System.Net;
using System.Net.Sockets;

namespace MSZUnitTests
{
    /// <summary>
    /// Summary description for MSZWServiceCMSTest
    /// </summary>
    [TestClass]
    public class MSZWServiceCMSTests
    {
        public MSZWServiceCMSTests()
        {
            //
            // TODO: Add constructor logic here
            //
            InitializeLicense();
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        #region Generals
        public void InitializeLicense()
        {
            var result = MSZ.MSZView.CheckState(true);
        }

        static string LocalIPAddress()
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

        static void CleanClientWTcp(ICommunicationObject proxy)
        {
            if (proxy != null)
            {
                //Done with the service, let's close it.
                try
                {
                    if (proxy.State != CommunicationState.Faulted)
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
                    if (proxy is IDisposable)
                        (proxy as IDisposable).Dispose();
                }
                catch (Exception ex)
                { }
            }
        }
        #endregion 

        #region Properties
        private static SecurityMode netTcpSecurityMode = SecurityMode.None;
        private static SecurityMode NetTcpSecurityMode
        {
            get
            {
                return netTcpSecurityMode;
            }
            set
            {
                netTcpSecurityMode = value;
            }
        }
        private static long maxReceivedMessageSize = 524288; // 512 KB
        private static long MaxReceivedMessageSize
        {
            get
            {
                return maxReceivedMessageSize;
            }
        }
        private static long maxReceivedWMessageSize = 524288; // 512 KB
        private static long MaxReceivedWMessageSize
        {
            get
            {
                return maxReceivedWMessageSize;
            }
        }
        private static String netTcpServiceAddress = "net.tcp://{0}:{1}/MSZService";
        private static String NetTcpServiceAddress
        {
            get
            {
                return netTcpServiceAddress;
            }
        }
        private static String netTcpWServiceAddress = "http://{0}:{1}/MSZWService";
        private static String NetTcpWServiceAddress
        {
            get
            {
                return netTcpWServiceAddress;
            }
        }

        private static String wHostName = "10.70.168.12";
        private static String WHostName
        {
            get
            {
                return wHostName;
            }
        }

        private static String wServerPort = "62873";
        private static String WServerPort
        {
            get
            {
                return wServerPort;
            }
        }
        private static String requestID = "{0}@{1}@{2}";
        private static String RequestID
        {
            get
            {
                return WPFUtilities.CryptString.CryptString.EncryptString(string.Format(requestID,
                System.Environment.MachineName,
                System.Diagnostics.Process.GetCurrentProcess().Id,
                System.Diagnostics.Process.GetCurrentProcess().SessionId));
            }
        }
        private static String requestID2 = "{0}@{1}";
        private static String RequestID2
        {
            get
            {
                try
                {
                    string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    return WPFUtilities.CryptString.CryptString.EncryptString(string.Format(requestID2,
                    MSZ.MSZUtils.GetPrevious(),
                    userName));

                }
                catch (Exception)
                {
                    return WPFUtilities.CryptString.CryptString.EncryptString(MSZ.MSZUtils.GetPrevious());
                }
            }
        }
        private static TimeSpan timeout = TimeSpan.FromSeconds(5);
        private static TimeSpan Timeout
        {
            get
            {
                return timeout;
            }
        }
        private static TimeSpan wTimeout = TimeSpan.FromSeconds(5);
        private static TimeSpan WTimeout
        {
            get
            {
                return wTimeout;
            }
        }
        #endregion

        //[TestMethod]
        public void TestGetRemovedListRequest()
        {
            string ret = string.Empty;
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                IMSZWServiceCMS clientWTcp = ChannelFactory<IMSZWServiceCMS>.CreateChannel(
                    new BasicHttpBinding() { MaxReceivedMessageSize = MaxReceivedWMessageSize },
                    new EndpointAddress(String.Format(NetTcpWServiceAddress, WHostName, wServerPort)));
                ((IContextChannel)clientWTcp).OperationTimeout = WTimeout;

                string host = string.Empty;
                try
                {
                    host = WPFUtilities.CryptString.CryptString.EncryptString("local IP " + LocalIPAddress());
                }
                catch (Exception ex)
                {
                }

                try
                {
                    var id2 = MSZ.MSZUtils.GetPrevious();
                    ret = clientWTcp.Request(new MSZWRequest() { RequestID = RequestID, Request = host, RequestID2 = id2, RequestType = "CdQqW2FK/EEX0tL1T8Qfnw==" });
                    if (!string.IsNullOrEmpty(ret))
                    {
                        string _RequestID = ret.Substring(0, RequestID.Length);
                        if (_RequestID != null && _RequestID == RequestID)
                        {
                            ret = ret.Substring(RequestID.Length);
                        }
                        else
                        {
                            ret = string.Empty;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
                finally
                {
                    CleanClientWTcp(clientWTcp as ICommunicationObject);
                }
            }
        }

        //[TestMethod]
        public void TestCheckFakeDongleRequest()
        {
            string ret = string.Empty;
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                IMSZWServiceCMS clientWTcp = ChannelFactory<IMSZWServiceCMS>.CreateChannel(
                    new BasicHttpBinding() { MaxReceivedMessageSize = MaxReceivedWMessageSize },
                    new EndpointAddress(String.Format(NetTcpWServiceAddress, WHostName, wServerPort)));
                ((IContextChannel)clientWTcp).OperationTimeout = WTimeout;

                try
                {
                    var serial = MSZ.MSZView.GetSerial();
                    if (serial == "0")
                    {
                        ret = string.Empty;
                    }

                    ret = clientWTcp.Request(new MSZWRequest()
                    {
                        RequestID = RequestID,
                        Request = WPFUtilities.CryptString.CryptString.EncryptString(serial),
                        RequestID2 = MSZ.MSZUtils.GetPrevious(),
                        RequestType = "aoFJf9sqeQYwONvVAvEfHg=="
                    });
                    if (!string.IsNullOrEmpty(ret))
                    {
                        string _RequestID = ret.Substring(0, RequestID.Length);
                        if (_RequestID != null && _RequestID == RequestID)
                        {
                            ret = ret.Substring(RequestID.Length);
                        }
                        else
                        {
                            ret = string.Empty;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
                finally
                {
                    CleanClientWTcp(clientWTcp as ICommunicationObject);
                }
            }
        }
    }
}
