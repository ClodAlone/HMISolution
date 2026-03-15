using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace ImportWizardPlugin
{
    public class ProjectWizardModel : INotifyPropertyChanged
    {
        private static bool _IsDynamic = false;
        public bool IsDynamic
        {
            get { return _IsDynamic; }
            set
            {
                //if(value)
                //    ProjectPath = string.Empty;
                _IsDynamic = value;
                OnPropertyChanged("IsDynamic");
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

        private static Uri _SourceProject;
        public Uri SourceProject
        {
            get
            {
                return _SourceProject;
            }
            set
            {
                _SourceProject = value;
                OnPropertyChanged("SourceProject");
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
