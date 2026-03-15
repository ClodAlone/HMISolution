using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartTagsControl.ComponentService;
using DocumentManager.ComponentService;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using SmartTagsHelper.EditableAdapter;

namespace SmartTagsControl.ViewModel
{
    internal class SmartTagsViewModel : IEditableObject, INotifyPropertyChanged, IDisposable
    {
        #region Properties

        object selectedObject;
        public object SelectedObject
        {
            get
            {
                return selectedObject;
            }
            set
            {
                if (selectedObject == value)
                    return;

                selectedObject = value;
                OnPropertyChanged("SelectedObject");
            }
        }

        List<UserControl> editableObjects;
        public List<UserControl> EditableObjects
        {
            get
            {
                if (editableObjects == null)
                    editableObjects = new List<UserControl>();

                return editableObjects;
            }
            internal set
            {
                if (editableObjects == value)
                    return;

                // confirm changes before to upate list
                EndEdit();

                // call dispose for every old object in the list
                FreeEditableObjectsList();

                // upate the oject list with new value
                editableObjects = value;

                OnPropertyChanged("EditableObjects");
                OnPropertyChanged("IsAnyEditableObjects");
                OnPropertyChanged("IsMoreOneEditableObjects");
            }
        }

        public bool IsAnyEditableObjects
        {
            get
            {
                return EditableObjects.Count > 0;
            }
        }

        public bool IsMoreOneEditableObjects
        {
            get
            {
                return EditableObjects.Count > 1;
            }
        }
        
        #endregion

        #region Methods

        void FreeEditableObjectsList()
        {
            foreach (UserControl control in EditableObjects)
            {
                if (control is IDisposable)
                    (control as IDisposable).Dispose();
            }

            EditableObjects.Clear();
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

        #region IEditableObject Members

        Dictionary<UserControl, EditableAdapter<object>> mapEditableAdapter;
        public void BeginEdit()
        {
            if (mapEditableAdapter == null)
                mapEditableAdapter = new Dictionary<UserControl, EditableAdapter<object>>();

            foreach (UserControl control in EditableObjects)
            {
                if (mapEditableAdapter.ContainsKey(control))
                    continue;

                mapEditableAdapter[control] = new EditableAdapter<Object>(control.DataContext);
                mapEditableAdapter[control].BeginEdit();
            }

        }

        public void CancelEdit()
        {
            if (mapEditableAdapter == null)
                return;

            foreach (UserControl control in EditableObjects)
            {
                if (!mapEditableAdapter.ContainsKey(control))
                    continue;

                mapEditableAdapter[control].CancelEdit();
                mapEditableAdapter.Remove(control);
            }
        }

        public void EndEdit()
        {
            if (mapEditableAdapter == null)
                return;

            foreach (UserControl control in EditableObjects)
            {
                if (!mapEditableAdapter.ContainsKey(control))
                    continue;

                mapEditableAdapter[control].EndEdit();
                mapEditableAdapter.Remove(control);
            }
        }

        #endregion

        #region IDisposable Members

        bool bDisposed;
        void IDisposable.Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            FreeEditableObjectsList();
        }

        #endregion
        
    }
}
