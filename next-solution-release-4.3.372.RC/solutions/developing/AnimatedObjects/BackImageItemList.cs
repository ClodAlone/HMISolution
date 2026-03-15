using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using Utilities;

namespace AnimatedObjects
{
    public class BackImageItemList : ObservableCollection<BackImage>
    {
        #region Constructors
        public BackImageItemList()
        { }

        public BackImageItemList(BackImageItemList instance)
        {
            if (instance == null)
                return;

            foreach (var item in instance)
                Add(new BackImage(item));
        }
        #endregion

        public List<object> ToDictionary()
        {
            List<string> list = new List<string>();
            List<object> res = new List<object>();
            this.ToList().ForEach(p => res.Add(p.ToDictionary(list)));
            return res;
        }
    }

    public partial class BackImage : INotifyPropertyChanged
    {
        #region Constructors
        public BackImage()
        { }

        public BackImage(BackImage instance)
        {
            if (instance == null)
                return;

            Value = instance.Value;
        }
        #endregion

        Uri _value;
        public Uri Value
        {
            get { return _value; }
            set
            {
                if (_value == value)
                    return;
                _value = value;
                OnPropertyChanged("Value");
            }
        }

        public Dictionary<string, object> ToDictionary(List<string> usedIDs)
        {
            Dictionary<string, object> res = new Dictionary<string, object>();
            res.Add("Value", Value?.GetPathString());
            return res;
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
        #endregion}
    }
}
