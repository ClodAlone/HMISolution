using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartupWelcome.Model
{
    public class HelpInfo : INotifyPropertyChanged
    {
        #region Properties
        private Uri _ProjectPath;

        /// <summary>
        /// Gets or sets the ProjectPath.
        /// </summary>
        /// <value>The ProjectPath.</value>
        public Uri ProjectPath
        {
            get
            {
                return _ProjectPath;
            }
            set
            {
                _ProjectPath = value;
                RaisePropertyChanged("ProjectPath");
            }
        }

        private String _Title;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the ProjectPath.
        /// </summary>
        /// <value>The ProjectPath.</value>
        public String Title
        {
            get
            {
                return _Title;
            }
            set
            {
                _Title = value;
                RaisePropertyChanged("Title");
            }
        }

        public override int GetHashCode()
        {
            return this.ProjectPath.GetHashCode();
        }

        #endregion

        public void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
