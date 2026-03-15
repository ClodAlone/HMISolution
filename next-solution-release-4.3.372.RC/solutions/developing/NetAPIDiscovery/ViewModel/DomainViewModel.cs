using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
using ViewModelLib;
using System.ComponentModel;
using System.Windows.Threading;
using Utilities;
using System.Threading;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.IO;
using UFInterfaces;
using System.Windows.Controls;

namespace NetAPIDiscovery
{
    public class DomainViewModel : TreeViewItemViewModel, IEntityReference
    {
        #region Declarations
        string lastUserName;
        string lastPassword;
        #endregion

        #region Constructor
        public DomainViewModel(Dispatcher dispatcher, String domain, TreeViewItemViewModel parent)
            : base(parent, false)
        {
            m_dispatcher = dispatcher;
            Title = domain;

            LastMessage = String.Format("{0} - {1}", Title, Properties.Resource.WorkstationDiscovering);

            PromoteIdleExecution(DispatcherPriority.Invalid);
        }
        #endregion

        protected override void IdleExecution()
        {
            if (this.HasDummyChild)
            {
                lock (lockObject)
                {
                    Children.Remove(DummyChild);
                }
            }

            int i = 0;
            try
            {
                string[] workstations = NetApiLib.EnumComputers(Title, lastUserName, lastPassword);
                foreach (string workstation in workstations)
                {
                    if (IsIdleExecutionCancelled() == true)
                        return;

                    LastMessage = String.Format("{0} - {1} {2}", Title, Properties.Resource.WorkstationDiscovered, workstation);

                    lock (lockObject)
                    {
                        Children.Add(new WorkstationViewModel(m_dispatcher, workstation, this));
                    }

                    ReportProgress(++i * 100 / workstations.Length);
                }
            }
            catch (Exception ex)
            {
                LastMessage = String.Format("Error discovering on {0} - {1}, error {2}",
                    Title, Properties.Resource.WorkstationDiscovering, ex.Message);

                Children.Add(new WorkstationViewModel(m_dispatcher, LastMessage, this));
            }
        }

        protected override void LoadChildren()
        {
            using (new WaitCursor())
            {
                while (IsIdleExecutionBusy())
                    Thread.Sleep(1);
            }
        }

        public void RestartDiscovery()
        {
            RestartDiscovery(lastUserName, lastPassword);
        }

        public void RestartDiscovery(string username, string password)
        {
            lastUserName = username;
            lastPassword = password;

            LastMessage = String.Format("{0} - {1}", Title, Properties.Resource.WorkstationDiscoveringRestarted);

            lock (lockObject)
            {
                CancelPendingIdleExecution();
                foreach (var v in Children)
                    v.Dispose();
                Children.Clear();
                IsExpanded = false;
                PromoteIdleExecution(DispatcherPriority.Invalid);
            }
        }

        [Browsable(false)]
        public bool CanRestartDiscovery
        {
            get { return NetApiLib.NetworkAvailable; }
        }

        #region IDisposable Members
        protected override void OnDispose()
        {
            base.OnDispose();

            lock (lockObject)
            {
                foreach (var v in Children)
                    v.Dispose();
                Children.Clear();
            }
        }
        #endregion

        #region Commands
        RelayCommand _connectCommand;
        public ICommand CommandCommand
        {
            get
            {
                if (_connectCommand == null)
                {
                    _connectCommand = new RelayCommand(
                        param => RestartDiscovery(),
                        param => CanRestartDiscovery
                        );
                }
                return _connectCommand;
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

        #region IEntityReference Members

        public ImageSource CollapsedImageSource
        {
            get
            {
                var bm = ViewModel.ViewModelHelper.GetControlImage("NAPIDntwrk");
                return bm;
            }
        }

        [Browsable(false)]
        public ImageSource ExpandedImageSource
        {
            get { return null; }
        }

        [Browsable(false)]
        public ContextMenu contextMenu
        {
            get { return null; }
        }

        [Browsable(false)]
        public object Tooltip
        {
            get { return null; }
        }

        [Browsable(false)]
        public object ContainedObject
        {
            get { return null; }
        }

        [Browsable(false)]
        public object EntityParent
        {
            get { return Parent; }
        }

        [Browsable(false)]
        public String TypeDefinitionString
        {
            get { return null; }
        }

        #endregion
    }
}
