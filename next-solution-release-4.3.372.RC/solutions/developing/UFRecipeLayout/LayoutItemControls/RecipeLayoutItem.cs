using DevExpress.Xpf.LayoutControl;
using System;
using System.ComponentModel;
using System.Text;
using UFInterfaces.PropertyControl;

namespace UFRecipeLayout.LayoutItemControls
{
    public class RecipeLayoutItem : LayoutItem, INotifyPropertyVisibilityChanged
    {
        #region Virtual Methods
        public virtual void SetContentValues()
        {

        }

        protected virtual bool CanShowProperty(string propertyName)
        {
            if (propertyName == "Visibility")
            {
                return false;
            }

            return true;
        }
        #endregion

        #region INotifyPropertyVisibilityChanged Members

        /// <summary>
        /// Gets the visibility state for the property with the given name.
        /// </summary>
        /// <param name="propertyName">The property name that you want konw the current visibility state.</param>
        /// <returns></returns>
        bool INotifyPropertyVisibilityChanged.this[string propertyName]
        {
            get
            {
                return CanShowProperty(propertyName);
            }
        }

        /// <summary>
        /// Raised when a property visibility state on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyVisiblityChanged;

        /// <summary>
        /// Raises this object's PropertyVisiblityChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has changed his value and has triggered the change of visibility.</param>
        protected void OnPropertyVisiblityChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyVisiblityChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        #endregion
    }
}
