using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace CustomWizardPlugin
{
    public enum ArchType: int
    {
        local = 0,
        distributed = 1,
        redundancy = 2,
    }
    public class ProjectWizardModel : INotifyPropertyChanged
    {
        private static bool _UseFileSystemProvider = false;
        public bool UseFileSystemProvider
        {
            get { return _UseFileSystemProvider; }
            set
            {
                //if(value)
                //    ProjectPath = string.Empty;
                _UseFileSystemProvider = value;
                OnPropertyChanged("UseFileSystemProvider");
            }
        }

        private static string _ProjectPath;
        public string ProjectPath
        {
            get
            {
                //if(string.IsNullOrEmpty(_ProjectPath))
                //    _ProjectPath =  ApplicationPropertiesHelper.GetProperty<String>("ProjectFolder");

                return _ProjectPath;
            }
            set
            {
                _ProjectPath = value;
                OnPropertyChanged("ProjectPath");
            }
        }

        private static string _ProjectName = string.Empty;
        public string ProjectName
        {
            get { return _ProjectName; }
            set
            {
                _ProjectName = value;
                OnPropertyChanged("ProjectName");
            }
        }

        private static string _Transport = string.Empty;
        public string Transport
        {
            get { return _Transport; }
            set
            {
                _Transport = value;
                OnPropertyChanged("Transport");
            }
        }

        private static string _Server = string.Empty;
        public string Server
        {
            get { return _Server; }
            set
            {
                _Server = value;
                OnPropertyChanged("Server");
            }
        }

        private int _Port;
        public int Port
        {
            get { return _Port; }
            set
            {
                _Port = value;
                OnPropertyChanged("Port");
            }
        }

        private static List<string> _ServerList = new List<string>();
        public List<string> ServerList
        {
            get { return _ServerList; }
            set
            {
                _ServerList = value;
                OnPropertyChanged("ServerList");
            }
        }

        private static ArchType _Architecture;
        public ArchType Architecture
        {
            get { return _Architecture; }
            set
            {
                _Architecture = value;
                OnPropertyChanged("Architecture");
            }
        }


        public DateTime RedundancyFullSynchronizationStartTime { get; set; }
        public TimeSpan RedundancyFullSynchronizationTimeSpan { get; set; }


        #region INotifyPropertyChanged Members
        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                //DispatcherObject dispatcherObject = handler.Target as DispatcherObject;

                var e = new PropertyChangedEventArgs(propertyName);
                //// If the subscriber is a DispatcherObject and different thread
                //if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                //{
                //    // Invoke handler in the target dispatcher's thread
                //    dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
                //}
                //else // Execute handler as is
                handler(this, e);
            }
        }
        #endregion
    }
}
