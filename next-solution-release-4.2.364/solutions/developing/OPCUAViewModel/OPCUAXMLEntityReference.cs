using System;
using System.ComponentModel;
using Utilities;

namespace OPCUAViewModel
{
    public class OPCUAXMLEntityReference : INotifyPropertyChanged
    {
        #region Private Members;
        private String tagreferenceXml;
        #endregion

        #region Public Properties

        public String TagReferenceXml
        {
            get { return tagreferenceXml; }
            set
            {
                if (tagreferenceXml == value)
                    return;
                tagreferenceXml = value;
                OnPropertyChanged("TagReferenceXml");
            }
        }

        OPCUAEntityReference tagReference;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public OPCUAEntityReference TagReference
        {
            get
            {
                try
                {
                    if (tagReference == null && !String.IsNullOrEmpty(tagreferenceXml))
                        tagReference = tagreferenceXml.FromXml<OPCUAEntityReference>();
                }
                catch
                { }

                return tagReference;
            }
            set
            {
                if (value != null)
                {
                    TagReferenceXml = value.ToXml();
                    tagReference = null;
                    OnPropertyChanged("TagReference");
                }
                else
                {
                    TagReferenceXml = string.Empty;
                    tagReference = null;
                    OnPropertyChanged("TagReference");
                }
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
}
