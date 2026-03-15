using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBControls
{
    public class ColumnItem : INotifyPropertyChanged
    {
        #region Constructors
        public ColumnItem()
        { }

        public ColumnItem(ColumnItem instance)
        {
            if (instance == null)
                return;

            ColName = instance.ColName;
            IsVisible = instance.IsVisible;
            IsEditable = instance.IsEditable;
            Caption = instance.Caption;
        }
        #endregion

        #region Private Members
        private String colname;
        private Boolean isvisible = true;
        private Boolean iseditable = false;
        //private CaptionAlignmentEnum imageCaption = CaptionAlignmentEnum.Hidden;
        private String caption;
        public Boolean isimage = false;
        #endregion

        #region Public Properties
        public String ColName
        {
            get { return colname; }
            set
            {
                if (colname == value)
                    return;
                colname = value;
                OnPropertyChanged("ColName");
            }
        }
        public Boolean IsVisible
        {
            get { return isvisible; }
            set
            {
                if (isvisible == value)
                    return;
                isvisible = value;
                OnPropertyChanged("IsVisible");
            }
        }
        public Boolean IsEditable
        {
            get { return iseditable; }
            set
            {
                if (iseditable == value)
                    return;
                iseditable = value;
                OnPropertyChanged("IsEditable");
            }
        }
        //public CaptionAlignmentEnum ImageCaption
        //{
        //    get { return imageCaption; }
        //    set
        //    {
        //        if (imageCaption == value)
        //            return;
        //        imageCaption = value;
        //        OnPropertyChanged("ImageCaption");
        //    }
        //}
        public String Caption
        {
            get { return caption; }
            set
            {
                if (caption == value)
                    return;
                caption = value;
                OnPropertyChanged("Caption");
            }
        }
        #endregion

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

    public enum CaptionAlignmentEnum
    {
        Hidden,
        Top,
        Right,
        Bottom,
        Left
    }
}
