using System;
using System.ComponentModel;
using System.Windows.Input;
using UFUserEditor.Document;

namespace UFUserEditor.Controls
{
    internal class MergeRepositoryModel : INotifyPropertyChanged
    {
        #region Constructors
        public MergeRepositoryModel()
        { }
        #endregion

        #region Properties
        bool copyAllSettings;
        public bool CopyAllSettings
        {
            get
            {
                return copyAllSettings;
            }
            set
            {
                if (value == copyAllSettings)
                    return;
                copyAllSettings = value;
                OnPropertyChanged("CopyAllSettings");
            }
        }

        bool copyUsersAndGroups = true;
        public bool CopyUsersAndGroups
        {
            get
            {
                return copyUsersAndGroups;
            }
            set
            {
                if (value == copyUsersAndGroups)
                    return;
                copyUsersAndGroups = value;
                OnPropertyChanged("CopyUsersAndGroups");
            }
        }

        bool replaceExistingsUsers;
        public bool ReplaceExistingsUsers
        {
            get
            {
                return replaceExistingsUsers;
            }
            set
            {
                if (value == replaceExistingsUsers)
                    return;
                replaceExistingsUsers = value;
                OnPropertyChanged("ReplaceExistingsUsers");
            }
        }
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        #endregion
    }
}
