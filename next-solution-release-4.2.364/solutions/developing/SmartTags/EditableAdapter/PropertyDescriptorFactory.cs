using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections;

namespace SmartTagsHelper.EditableAdapter
{
    /// <summary>
    /// Provides methods for easily creating property descriptors.
    /// </summary>
    public static class PropertyDescriptorFactory
    {
        /// <summary>
        /// Creates a custom property descriptor.
        /// </summary>
        /// <typeparam name="TComponent">The component type.</typeparam>
        /// <typeparam name="TProperty">The parameter type.</typeparam>
        /// <param name="name">The name of the property.</param>
        /// <param name="getter">A function that takes a component and gets this property's value.</param>
        /// <param name="setter">An action that takes a component and sets this property's value.</param>
        /// <returns>A customer property descriptor.</returns>
        public static PropertyDescriptor CreatePropertyDescriptor<TComponent, TProperty>(string name, Func<TComponent, TProperty> getter, Action<TComponent, TProperty> setter)
        {
            return InternalPropertyDescriptorFactory.CreatePropertyDescriptor<TComponent, TProperty>(name, getter, setter);
        }

        /// <summary>
        /// Creates a custom read-only property descriptor.
        /// </summary>
        /// <typeparam name="TComponent">The component type.</typeparam>
        /// <typeparam name="TProperty">The parameter type.</typeparam>
        /// <param name="name">The name of the read-only property.</param>
        /// <param name="getter">A function that takes a component and gets this property's value.</param>
        /// <returns>A customer property descriptor.</returns>
        public static PropertyDescriptor CreatePropertyDescriptor<TComponent, TProperty>(string name, Func<TComponent, TProperty> getter)
        {
            return InternalPropertyDescriptorFactory.CreatePropertyDescriptor<TComponent, TProperty>(name, getter);
        }

        /// <summary>
        /// Creates a custom property descriptor.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="componentType">A System.Type that represents the type of component to which this property descriptor binds.</param>
        /// <param name="propertyType">A System.Type that represents the data type for this property.</param>
        /// <param name="getter">A function that takes a component and gets this property's value.</param>
        /// <param name="setter">An action that takes a component and sets this property's value.</param>
        /// <returns>A customer property descriptor.</returns>
        public static PropertyDescriptor CreatePropertyDescriptor(string name, Type componentType, Type propertyType, Func<object, object> getter, Action<object, object> setter)
        {
            return InternalPropertyDescriptorFactory.CreatePropertyDescriptor(name, componentType, propertyType, getter, setter);
        }

        /// <summary>
        /// Creates a custom read-only property descriptor.
        /// </summary>
        /// <param name="name">The name of the read-only property.</param>
        /// <param name="componentType">A System.Type that represents the type of component to which this property descriptor binds.</param>
        /// <param name="propertyType">A System.Type that represents the data type for this property.</param>
        /// <param name="getter">A function that takes a component and gets this property's value.</param>
        /// <returns>A customer property descriptor.</returns>
        public static PropertyDescriptor CreatePropertyDescriptor(string name, Type componentType, Type propertyType, Func<object, object> getter)
        {
            return InternalPropertyDescriptorFactory.CreatePropertyDescriptor(name, componentType, propertyType, getter);
        }
    }

    
}
