using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using UFInterfaces.Constants;
using System.ComponentModel;
using UFInterfaces;
using MenuSettings.Documents;
using System.Windows.Media;

namespace MenuSettings.MenuModel
{
    [DataContract(Name = "MenuEntity", Namespace = Namespaces.UriProgea)]
    public class UFMenuEntity : INotifyPropertyChanged, IDataErrorInfo, ICloneable, IEntityReference
    {
        #region Declarations
        internal UFMenuDocument Document;
        #endregion

        #region Persistance
        [DataMember]
        Guid _NodeId;
        [DataMember]
        List<UFMenuItemEntity> _MenuItems;

        [OnDeserialized]
        private void PostInitialize(StreamingContext context)
        {
            // ensure a valid recipe association value and oid value
            int oid = 0;
            var values = MenuItems.OrderBy(o => o.OID);
            foreach (var value in values)
            {
                value.UFMenuAss = this;
                value.OID = oid++;
            }
        }
        #endregion

        #region Properties
        string _Name;
        [ReadOnly(true)]
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                if (_Name == value)
                    return;

                _Name = value;
                OnPropertyChanged("Name");
            }
        }
        [ReadOnly(true)]
        public Guid NodeId
        {
            get
            {
                return _NodeId;
            }
            set
            {
                if (_NodeId == value)
                    return;

                _NodeId = value;
                OnPropertyChanged("NodeId");
            }
        }

        [Browsable(false)]
        public List<UFMenuItemEntity> MenuItems
        {
            get
            {
                if (_MenuItems == null)
                    _MenuItems = new List<UFMenuItemEntity>();

                return _MenuItems;
            }
        }

        #endregion

        Object containedObject;
        internal void SetContainedObject(Object o)
        {
            containedObject = o;
        }

        #region IEntityReference Members

        [Browsable(false)]
        public ImageSource CollapsedImageSource
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        public ImageSource ExpandedImageSource
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        public System.Windows.Controls.ContextMenu contextMenu
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        public object Tooltip
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        public object ContainedObject
        {
            get
            {
                return containedObject;
            }
        }

        [Browsable(false)]
        public object EntityParent
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        public string TypeDefinitionString
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            return MemberwiseClone() as UFMenuEntity;
        }

        #endregion ICloneable Members

        #region IDataErrorInfo

        [Browsable(false)]
        public string Error
        {
            get
            {
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null);
                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

                return !System.ComponentModel.DataAnnotations.Validator.TryValidateObject(this, context, results)
                    ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                    : null;
            }
        }

        [Browsable(false)]
        public string this[string propertyName]
        {
            get
            {
                String s = PerformValidation(propertyName);
                if (!String.IsNullOrEmpty(s))
                    return s;
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null)
                {
                    MemberName = propertyName
                };

                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                var propertyInfo = GetType().GetProperty(propertyName);
                if (propertyInfo != null)
                {

                    var value = propertyInfo.GetValue(this, null);

                    return !System.ComponentModel.DataAnnotations.Validator.TryValidateProperty(value, context, results)
                        ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                        : null;
                }

                return null;
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

        #region Methods
        public int GetFirstValidMenuItemOrderId(UFMenuItemEntity root = null)
        {
            var menuitems = new List<UFMenuItemEntity>();
            if (root != null)
                menuitems.AddRange(root.MenuItems);
            else
                menuitems.AddRange(MenuItems);

            var values = (from c in menuitems where c.OID >= 0 orderby c.OID descending select c.OID).ToList();
            if (values.Count > 0)
                return values[0];
            else
                return -1;
        }

        protected String PerformValidation(String propertyName)
        {
            return null;
        }
        #endregion
    }
}
