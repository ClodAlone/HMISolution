using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace SmartTagsHelper.EditableAdapter
{
    /// <summary>
    /// Provides internal methods for creating property descriptors.  This class should not be used directly.
    /// </summary>
    internal class InternalPropertyDescriptorFactory : TypeConverter
    {
        public static PropertyDescriptor CreatePropertyDescriptor<TComponent, TProperty>(string name, Func<TComponent, TProperty> getter, Action<TComponent, TProperty> setter)
        {
            return new GenericPropertyDescriptor<TComponent, TProperty>(name, getter, setter);
        }

        public static PropertyDescriptor CreatePropertyDescriptor<TComponent, TProperty>(string name, Func<TComponent, TProperty> getter)
        {
            return new GenericPropertyDescriptor<TComponent, TProperty>(name, getter);
        }

        public static PropertyDescriptor CreatePropertyDescriptor(string name, Type componentType, Type propertyType, Func<object, object> getter, Action<object, object> setter)
        {
            return new GenericPropertyDescriptor(name, componentType, propertyType, getter, setter);
        }

        public static PropertyDescriptor CreatePropertyDescriptor(string name, Type componentType, Type propertyType, Func<object, object> getter)
        {
            return new GenericPropertyDescriptor(name, componentType, propertyType, getter);
        }

        protected class GenericPropertyDescriptor<TComponent, TProperty> : TypeConverter.SimplePropertyDescriptor
        {
            Func<TComponent, TProperty> getter;
            Action<TComponent, TProperty> setter;

            public GenericPropertyDescriptor(string name, Func<TComponent, TProperty> getter, Action<TComponent, TProperty> setter)
                : base(typeof(TComponent), name, typeof(TProperty))
            {
                if (getter == null)
                {
                    throw new ArgumentNullException("getter");
                }
                if (setter == null)
                {
                    throw new ArgumentNullException("setter");
                }

                this.getter = getter;
                this.setter = setter;
            }

            public GenericPropertyDescriptor(string name, Func<TComponent, TProperty> getter)
                : base(typeof(TComponent), name, typeof(TProperty))
            {
                if (getter == null)
                {
                    throw new ArgumentNullException("getter");
                }

                this.getter = getter;
            }

            public override bool IsReadOnly
            {
                get
                {
                    return this.setter == null;
                }
            }

            public override object GetValue(object target)
            {
                TComponent component = (TComponent)target;
                TProperty value = this.getter(component);
                return value;
            }

            public override void SetValue(object target, object value)
            {
                if (!this.IsReadOnly)
                {
                    TComponent component = (TComponent)target;
                    TProperty newValue = (TProperty)value;
                    this.setter(component, newValue);
                }
            }
        }

        protected class GenericPropertyDescriptor : TypeConverter.SimplePropertyDescriptor
        {
            Func<object, object> getter;
            Action<object, object> setter;

            public GenericPropertyDescriptor(string name, Type componentType, Type propertyType, Func<object, object> getter, Action<object, object> setter)
                : base(componentType, name, propertyType)
            {
                if (getter == null)
                {
                    throw new ArgumentNullException("getter");
                }
                if (setter == null)
                {
                    throw new ArgumentNullException("setter");
                }

                this.getter = getter;
                this.setter = setter;
            }

            public GenericPropertyDescriptor(string name, Type componentType, Type propertyType, Func<object, object> getter)
                : base(componentType, name, propertyType)
            {
                if (getter == null)
                {
                    throw new ArgumentNullException("getter");
                }

                this.getter = getter;
            }

            public override bool IsReadOnly
            {
                get
                {
                    return this.setter == null;
                }
            }

            public override object GetValue(object target)
            {
                object value = this.getter(target);
                return value;
            }

            public override void SetValue(object target, object value)
            {
                if (!this.IsReadOnly)
                {
                    object newValue = (object)value;
                    this.setter(target, newValue);
                }
            }
        }
    }
}
