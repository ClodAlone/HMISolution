using System;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Services.InstanceManagerSvc;
using Utilities;
using log4net;
using System.ServiceModel;
using System.ServiceModel.Discovery;

namespace UFSolutionNext
{
    class InstanceManager : IInstanceManager
    {
        static readonly ILog logGeneral = LogManager.GetLogger(Properties.Resources.GeneralLog);

        static Mutex m_Mutex;
        static ServiceHost m_Host;
    
        #region Interface Implementation

        public delegate void ActivateWndDelegate();
        public void ActivateApplication(String[] commands)
        {
            ActivateWndDelegate methodToActivate = delegate 
                                        {
                                            if (MainWindow.WindowState == WindowState.Minimized)
                                                MainWindow.WindowState = WindowState.Normal;
                                            MainWindow.Activate();

                                            if (MainWindow is UFMainWindow)
                                                (MainWindow as UFMainWindow).CheckCommandArguments(commands);
                                            else if (MainWindow is RuntimeWindow)
                                                (MainWindow as RuntimeWindow).UpdateCommandArguments(commands);
                                        };

            MainWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal, methodToActivate);
        }
        #endregion

        public static Window MainWindow { get; set; }


        public static void Initialize()
        {
            if (!IsFirstInstance())
                return;

            Application.Current.Exit += OnExit;
            Application.Current.Dispatcher.BeginInvokeAsynchronouslyInApplicationIdle(() =>
                {
                    try
                    {
                        HostActivationMonitor();
                    }
                    catch(Exception ex)
                    {
                        logGeneral.Error(Properties.Resources.ErrorActivatingInstanceService, ex);
                    }
                });
        }

        static bool createdNew;
        public static bool IsFirstInstance()
        {
            string name = Assembly.GetEntryAssembly().FullName;

            if (m_Mutex == null)
                m_Mutex = new Mutex(true, name, out createdNew);
            return createdNew;
            // return m_Mutex.WaitOne(TimeSpan.Zero, false);
        }
        static void OnExit(object sender, EventArgs args)
        {
            if (m_Host == null)
                return;
            try
            {
                if (m_Host.State != CommunicationState.Faulted)
                    m_Host.Close();
                m_Mutex.ReleaseMutex();
                m_Mutex.Close();
            }
            catch
            {

            }
        }
        static void HostActivationMonitor()
        {
            m_Host = new ServiceHost(typeof(InstanceManager));

            m_Host.AddServiceEndpoint(typeof(IInstanceManager), new NetNamedPipeBinding(), MonitorNetPipeServiceAddress);

            // m_Host.AddServiceEndpoint(typeof(IInstanceManager), new BasicHttpBinding(), MonitorBasicHttpServiceAddress);

            #region Discovery Settings

            ServiceDiscoveryBehavior discoveryBehavior = new ServiceDiscoveryBehavior();
            m_Host.Description.Behaviors.Add(discoveryBehavior);
            m_Host.AddServiceEndpoint(new UdpDiscoveryEndpoint());
            discoveryBehavior.AnnouncementEndpoints.Add(new UdpAnnouncementEndpoint());

            #endregion

            m_Host.Open();
        }

        public static void ActivateFirstInstance(String[] commands)
        {
            IInstanceManager monitor = ChannelFactory<IInstanceManager>.CreateChannel(new NetNamedPipeBinding(), new EndpointAddress(MonitorNetPipeServiceAddress));
            using (monitor as IDisposable)
            {
                monitor.ActivateApplication(commands);
                ICommunicationObject proxy = monitor as ICommunicationObject;

                //Done with the service, let's close it.
                try
                {
                    if (proxy.State != CommunicationState.Faulted)
                    {
                        proxy.Close();
                    }
                }
                catch (Exception)
                {
                    proxy.Abort();
                    throw;
                }
            }
        }

        static string MonitorNetPipeServiceAddress
        {
            get
            {
                Assembly assembly = Assembly.GetEntryAssembly();
                string pipeName = assembly.FullName;
                return String.Format("net.pipe://localhost/{0}", pipeName);
            }
        }

        static string MonitorBasicHttpServiceAddress
        {
            get
            {
                Assembly assembly = Assembly.GetEntryAssembly();
                string pipeName = assembly.FullName;
                return String.Format("http://localhost:65535/{0}", pipeName);
            }
        }
    }
}
