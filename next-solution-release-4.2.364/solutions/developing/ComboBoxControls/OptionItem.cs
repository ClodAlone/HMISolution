using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComboBoxControls
{
    public class OptionItem : INotifyPropertyChanged
    {
        #region Constructors
        public OptionItem()
        { }

        public OptionItem(OptionItem instance)
        {
            if (instance == null)
                return;

            OptionContent = instance.OptionContent;
            UntranslatedOptionContent = instance.UntranslatedOptionContent;
            OptionValue = instance.OptionValue;
        }
        #endregion

        #region Private Members
        private String optioncontent;
        private String untranslatedoptioncontent;
        private String optionvalue;
        #endregion

        #region Public Properties
        public String OptionContent
        {
            get { return optioncontent; }
            set
            {
                if (optioncontent == value)
                    return;
                optioncontent = value;
                OnPropertyChanged("OptionContent");
            }
        }
        public String UntranslatedOptionContent
        {
            get {
                if (untranslatedoptioncontent == null)
                    UntranslatedOptionContent = optioncontent;
                return untranslatedoptioncontent;
            }
            set
            {
                if (untranslatedoptioncontent == value)
                    return;
                untranslatedoptioncontent = value;
                OptionContent = value;
                OnPropertyChanged("UntranslatedOptionContent");
            }
        }
        public String OptionValue
        {
            get { return optionvalue; }
            set
            {
                if (optionvalue == value)
                    return;
                optionvalue = value;
                OnPropertyChanged("OptionValue");
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
