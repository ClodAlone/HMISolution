using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewModelLib;
using Opc.Ua;
using Utilities;
#if !WINDOWS_UWP && !NET_STANDARD
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
#endif
using System.Windows.Input;
using System.Reflection;
using System.IO;
using UFInterfaces;

namespace OPCUAViewModel
{
    public class ApplicationDescriptionViewModel : TreeViewItemViewModel, IEntityReference
    {
#region Members
        public ApplicationDescription applicationDescription { get; private set; }
        public ApplicationConfiguration configuration { get; private set; }
        public String Workstation { get; private set; }

#endregion

#region Constructor
        public ApplicationDescriptionViewModel(ApplicationDescription ap, 
                                               String workStation, TreeViewItemViewModel parent,
                                               ApplicationConfiguration ac)
            : base(parent, false)
        {
            if (ap == null)
                throw new ArgumentNullException("ApplicationDescription");

            applicationDescription = ap;
            Title = String.Format("{0}", ap.ApplicationName);

            configuration = ac;

            if (!String.IsNullOrEmpty(workStation))
            {
                Workstation = workStation;
#if WINDOWS_UWP || NET_STANDARD
                PromoteIdleExecution(100);
#else
                PromoteIdleExecution(DispatcherPriority.Invalid);
#endif
            }
        }
#endregion

        protected override void IdleExecution()
        {
            foreach (string discoveryUrl in applicationDescription.DiscoveryUrls)
            {
                if (IsIdleExecutionCancelled())
                    return;

                Uri url = Utils.ParseUri(discoveryUrl);

                if (url != null)
                {
                    LastMessage = String.Format("{0} - {1} {2}", Title, Properties.Resource.OPCUADiscoveringEndpoint, url);

                    DiscoveryClient client = null;
                    try
                    {
                        EndpointConfiguration endpointConfiguration = EndpointConfiguration.Create(configuration);
                        endpointConfiguration.OperationTimeout = 5000;

                        client = DiscoveryClient.Create(url, endpointConfiguration);

                        if (IsIdleExecutionCancelled())
                            return;

#if WINDOWS_UWP
                        var hostname = NetworkHelpers.GetHostName();
#else
                        var hostname = System.Net.Dns.GetHostName();
#endif
                        bool bLocalHost = (String.Compare(url.Host, "localhost", StringComparison.OrdinalIgnoreCase) == 0 ||
                                           String.Compare(url.Host, hostname, StringComparison.OrdinalIgnoreCase) == 0);

                        if (!bLocalHost && url.Scheme == "net.pipe")
                            continue;
                    
                        EndpointDescriptionCollection endpoints = client.GetEndpoints(null);
                        lock (lockObject)
                        {
                            foreach (EndpointDescription epd in endpoints)
                            {
                                if (!bLocalHost)
                                {
                                    if (epd.EndpointUrl.IndexOf("net.pipe:", StringComparison.OrdinalIgnoreCase) != 0)
                                        Children.Add(new EndpointDescriptionViewModel(epd, this));
                                }
                                else
                                    Children.Add(new EndpointDescriptionViewModel(epd, this));
                            }
                        }
                        break;
                    }
                    catch (Exception ex)
                    {
                        LastMessage = String.Format("{0} - {1} {2} {3}", Title, Properties.Resource.OPCUADiscoveringEndpointNotFound, url, ex);

                        Utils.Trace(ex, Properties.Resource.OPCUADiscoveringEndpointNotFound); 
                    }
                    finally
                    {
                        if (client != null)
                            client.Close();
                    }
                }
            }
        }

#region Commands
        RelayCommand _refreshCommand;
        public ICommand RefreshCommand
        {
            get
            {
                if (_refreshCommand == null)
                {
                    _refreshCommand = new RelayCommand(
                        param => RestartDiscovery(),
                        param => CanRestartDiscovery
                        );
                }
                return _refreshCommand;
            }
        }

        bool CanRestartDiscovery
        {
            get { return true; }
        }
#endregion

#region Properties
        public LocalizedText ApplicationName
        {
            get
            {
                return applicationDescription.ApplicationName;
            }
            set
            {
                if (value == applicationDescription.ApplicationName)
                    return;

                applicationDescription.ApplicationName = value;
                OnPropertyChanged("ApplicationName");
            }
        }

        public ApplicationType ApplicationType
        {
            get
            {
                return applicationDescription.ApplicationType;
            }
            set
            {
                if (value == applicationDescription.ApplicationType)
                    return;

                applicationDescription.ApplicationType = value;
                OnPropertyChanged("ApplicationType");
            }
        }

        public string ApplicationUri
        {
            get
            {
                return applicationDescription.ApplicationUri;
            }
            set
            {
                if (value == applicationDescription.ApplicationUri)
                    return;

                applicationDescription.ApplicationUri = value;
                OnPropertyChanged("ApplicationUri");
            }
        }
#endregion

#region Methods
        public void RestartDiscovery()
        {
            LastMessage = String.Format("{0} - {1}", Title, Properties.Resource.OPCUADiscoveringEndpointRestarted);

            bool hasChildren = HasChildren;
            lock (lockObject)
            {
                CancelPendingIdleExecution();
                if (hasChildren)
                    Children.Clear();
                IsExpanded = false;
#if WINDOWS_UWP || NET_STANDARD
                PromoteIdleExecution(100);
#else
                PromoteIdleExecution(DispatcherPriority.Invalid);
#endif
            }
        }
        #endregion

#if !WINDOWS_UWP && !NET_STANDARD
#region Validations
        public override string Error
        {
            get
            {
                return null;
            }
        }

        public override string this[string propertyName]
        {
            get
            {
                return PerformValidation(propertyName);
            }
        }
#endregion
#endif

#region IEntityReference Members

        public ImageSource CollapsedImageSource
        {
            get
            {
#if !WINDOWS_UWP && !NET_STANDARD
                //BitmapImage bm = new BitmapImage();
                //bm.BeginInit();
                //Assembly assembly = Assembly.GetExecutingAssembly();

                //String str = String.Format("pack://application:,,,/{0};component/Images/apps_16x16.png",
                //    Path.GetFileNameWithoutExtension(assembly.Location));
                //bm.UriSource = new Uri(str);
                //bm.EndInit();
                var bm = SharedResources.Helpers.ResourceManager.GetCommonImage(Properties.Settings.Default.TypeLabel, $"OPCUAVMApps", false);

                return bm;
#else
                return null;
#endif
            }
        }

        public ImageSource ExpandedImageSource
        {
            get { return null; }
        }

        public ContextMenu contextMenu
        {
            get { return null; }
        }

        public object Tooltip
        {
            get { return null; }
        }

        public object ContainedObject
        {
            get { return applicationDescription; }
        }

        public object EntityParent
        {
            get { return Parent; }
        }

        public String TypeDefinitionString
        {
            get { return null; }
        }

#endregion
    }
}
