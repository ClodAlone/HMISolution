using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Reflection;

namespace SmartTagsHelper.EditableAdapter
{
    public class EditableAdapter<T> : IEditableObject, ICustomTypeDescriptor, INotifyPropertyChanged
    {
        /// <summary>
        /// The wrapped object.
        /// </summary>
        public T Target { get; set; }

        Memento<T> memento;

        public EditableAdapter(T target)
        {
            this.Target = target;
        }

        #region IEditableObject Members

        public void BeginEdit()
        {
            if (this.memento == null)
            {
                this.memento = new Memento<T>(this.Target);
            }
        }

        public void CancelEdit()
        {
            if (this.memento != null)
            {
                this.memento.Restore(this.Target);
                this.memento = null;
            }
        }

        public void EndEdit()
        {
            this.memento = null;
        }

        #endregion

        #region ICustomTypeDescriptor Members

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            IList<PropertyDescriptor> propertyDescriptors = new List<PropertyDescriptor>();

            var readonlyPropertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                 .Where(p => p.CanRead && !p.CanWrite);

            var writablePropertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                 .Where(p => p.CanRead && p.CanWrite);

            foreach (var property in readonlyPropertyInfos)
            {
                var propertyCopy = property; // Need this copy of property for use in the closure

                var propertyDescriptor = PropertyDescriptorFactory.CreatePropertyDescriptor(
                    property.Name,
                    typeof(T),
                    property.PropertyType,
                    (component) => propertyCopy.GetValue(((EditableAdapter<T>)component).Target, null));

                propertyDescriptors.Add(propertyDescriptor);
            }

            foreach (var property in writablePropertyInfos)
            {
                var propertyCopy = property; // Need this copy of property for use in the closure

                var propertyDescriptor = PropertyDescriptorFactory.CreatePropertyDescriptor(
                    property.Name,
                    typeof(T),
                    property.PropertyType,
                    (component) => propertyCopy.GetValue(((EditableAdapter<T>)component).Target, null),
                    (component, value) => propertyCopy.SetValue(((EditableAdapter<T>)component).Target, value, null));

                propertyDescriptors.Add(propertyDescriptor);
            }

            return new PropertyDescriptorCollection(propertyDescriptors.ToArray());
        }

        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            throw new NotImplementedException();
        }

        string ICustomTypeDescriptor.GetClassName()
        {
            throw new NotImplementedException();
        }

        string ICustomTypeDescriptor.GetComponentName()
        {
            throw new NotImplementedException();
        }

        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            throw new NotImplementedException();
        }

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            throw new NotImplementedException();
        }

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            throw new NotImplementedException();
        }

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            throw new NotImplementedException();
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            throw new NotImplementedException();
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            throw new NotImplementedException();
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            throw new NotImplementedException();
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            throw new NotImplementedException();
        }

        #endregion

        private void NotifyPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, e);
            }
        }

        #region INotifyPropertyChanged Members

        private event PropertyChangedEventHandler PropertyChanged;
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                if (this.Target is INotifyPropertyChanged)
                {
                    this.PropertyChanged += value;
                    ((INotifyPropertyChanged)this.Target).PropertyChanged += this.NotifyPropertyChanged;
                }
            }

            remove
            {
                if (this.Target is INotifyPropertyChanged)
                {
                    this.PropertyChanged -= value;
                    ((INotifyPropertyChanged)this.Target).PropertyChanged -= this.NotifyPropertyChanged;
                }
            }
        }

        #endregion
    }
}
