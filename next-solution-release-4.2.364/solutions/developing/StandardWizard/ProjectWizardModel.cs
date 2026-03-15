using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace StandardWizard
{
    public class ProjectWizardModel
    {
        private static bool _IsDynamic = false;
        public bool IsDynamic
        {
            get { return _IsDynamic; }
            set {
                    if(value)
                        ProjectPath = string.Empty;
                    _IsDynamic = value; 
                }
        }
        private static bool _IsStatic = true;
        public bool IsStatic
        {
            get { return _IsStatic; }
            set {
                    if (value)
                        ProjectPath = ApplicationPropertiesHelper.GetProperty<String>("ProjectFolder");
                    _IsStatic = value; 
                }
        }

        private static string _ProjectPath = ApplicationPropertiesHelper.GetProperty<String>("ProjectFolder");
        public string ProjectPath
        {
            get { return _ProjectPath; }
            set { 
                    _ProjectPath = value;
                    if (null != this.PropertyChanged)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("ProjectPath"));
                    }
                }
        }

        private static string _ProjectName = string.Empty;
        public string ProjectName
        {
            get { return _ProjectName; }
            set { 
                    _ProjectName = value; 
                    if (null != this.PropertyChanged)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("ProjectName"));
                    }
                }
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
