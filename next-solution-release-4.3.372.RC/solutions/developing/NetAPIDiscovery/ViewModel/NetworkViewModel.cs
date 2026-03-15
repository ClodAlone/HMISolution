using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
using ViewModelLib;
using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Windows.Threading;
using Utilities;
using System.Threading;
using System.Windows.Input;

namespace NetAPIDiscovery
{
    public class NetworkViewModel : TreeViewItemViewModel
    {
        #region Constructor
        public NetworkViewModel(Dispatcher dispatcher)
            : base(null, false)
        {
            m_dispatcher = dispatcher;

            NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;
            NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;

            Title = Properties.Resource.NetworkDiscovery;
            LastMessage = String.Format("{0} - {1}", Title, Properties.Resource.NetworkDiscoveryStarted);
        }
        #endregion

        #region Properties
        [Browsable(false)]
        bool CanRestartDiscovery
        {
            get { return NetApiLib.NetworkAvailable; }
        }
        #endregion

        void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            RestartDiscovery();
        }

        void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            RestartDiscovery();
        }

        protected override void IdleExecution()
        {
            //if (System.Windows.MessageBox.Show("Do You want to discover Local Network ?", "Network Discovering",
            //    System.Windows.MessageBoxButton.YesNo) != System.Windows.MessageBoxResult.Yes)
            //    return;

            LastMessage = String.Format("{0} - {1}", Title, Properties.Resource.NetworkDiscovering);

            try
            {
                int i = 0;
                string[] domains = NetApiLib.EnumDomains();
                foreach (string domain in domains)
                {
                    if (IsIdleExecutionCancelled() == true)
                        return;

                    LastMessage = String.Format("{0} - {1} {2}", Title, Properties.Resource.NetworkDiscovered, domain);

                    lock (lockObject)
                    {
                        Children.Add(new DomainViewModel(m_dispatcher, domain, this));
                    }

                    ReportProgress(++i * 100 / domains.Length);
                }
            }
            catch (Exception ex)
            {
                LastMessage = String.Format("Error discovering on {0} - {1}, error {2}", 
                    Title, Properties.Resource.NetworkDiscovering, ex.Message);

                Children.Add(new DomainViewModel(m_dispatcher, LastMessage, this));
            }
        }


        #region Methods
        public void RestartDiscovery()
        {
            LastMessage = String.Format("{0} - {1}", Title, Properties.Resource.NetworkDiscoveringRestarted);

            bool hasChildren = HasChildren;

            lock (lockObject)
            {
                CancelPendingIdleExecution();
                if (hasChildren)
                {
                    foreach (var v in Children)
                        v.Dispose();
                    Children.Clear();
                }

                IsExpanded = false;
                PromoteIdleExecution(DispatcherPriority.Invalid);
            }
        }

        #endregion

        #region IDisposable Members
        protected override void OnDispose()
        {
            base.OnDispose();

            bool hasChildren = HasChildren;
            lock (lockObject)
            {
                if (hasChildren)
                {
                    foreach (var v in Children)
                        v.Dispose();
                    Children.Clear();
                }

                NetworkChange.NetworkAddressChanged -= NetworkChange_NetworkAddressChanged;
                NetworkChange.NetworkAvailabilityChanged -= NetworkChange_NetworkAvailabilityChanged;
            }
        }
        #endregion

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
        #endregion

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
    }
}
