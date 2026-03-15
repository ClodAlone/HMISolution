using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace StartupWelcome.Model
{
    public class RecentInfo : INotifyPropertyChanged
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

        public String Path
        {
            get
            {
                try
                {
                    var uriPath = ProjectPath?.GetPathString();
                    if (XpoHelpers.XpoHelper.IsDataSource(uriPath))
                        return XpoHelpers.XpoHelper.GetConnectionStringWithoutPassword(uriPath);
               
                    return System.IO.Path.GetFullPath(uriPath);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public String Name
        {
            get
            {
                try
                {
                    var uriPath = ProjectPath?.GetPathString();
                    if (XpoHelpers.XpoHelper.IsDataSource(uriPath))
                        return XpoHelpers.XpoHelper.GetDataSourceTitle(uriPath);

                    return System.IO.Path.GetFileNameWithoutExtension(uriPath);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

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
