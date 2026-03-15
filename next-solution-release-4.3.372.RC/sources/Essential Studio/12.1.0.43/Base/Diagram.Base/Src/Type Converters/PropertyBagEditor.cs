#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Collections;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Property value Editor for Propertybag type.
    /// </summary>
    public class PropertyBagEditor : System.Drawing.Design.UITypeEditor
    {
        #region Constants
        /// <summary>
        /// Dynamic property data 
        /// </summary>
        protected const string c_strProperties = "PropertyBag";        
        #endregion

        #region Class members
        private IWindowsFormsEditorService editorService = null;
        #endregion

        #region Class utility methods
        /// <summary>
        /// Updates the value  of the property
        /// </summary>
        /// <param name="editingInstance">The editing instance</param>
        /// <param name="editor">The editor</param>
        /// <returns></returns>
        private static object UpdateReturnValue(Dictionary<string, object> editingInstance, PropertyBagDialog editor)
        {
            editingInstance = editor.PropertyBag;
            return editingInstance;
        }

        /// <summary>
        /// Sets the editor properties.
        /// </summary>
        /// <param name="editingInstance">The editing instance.</param>
        /// <param name="editor">The editor.</param>
        protected virtual void SetEditorProps(Dictionary<string, object> editingInstance, PropertyBagDialog editor)
        {
            editor.PropertyBag = editingInstance;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Edits the specified object's value using the editor style indicated by the <see cref="M:System.Drawing.Design.UITypeEditor.GetEditStyle"/> method.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that can be used to gain additional context information.</param>
        /// <param name="provider">An <see cref="T:System.IServiceProvider"/> that this editor can use to obtain services.</param>
        /// <param name="value">The object to edit.</param>
        /// <returns>
        /// The new value of the object. If the value of the object has not changed, this should return the same object it was passed.
        /// </returns>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context != null
                && context.Instance != null
                && provider != null)
            {
                editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (editorService != null)
                {
                    if (context.Instance is Array)
                    {
                        Array ctxtArray = context.Instance as Array;
                        if (ctxtArray.Length > 0)
                        {
                            Dictionary<string, object>[] propertyArray = new Dictionary<string, object>[ctxtArray.Length];
                            int i = 0;
                            foreach (object o in ctxtArray)
                            {
                                Dictionary<string, object> property = null;

                                // get exact property name
                                // -----------------------
                                // PropertiesInfo is used for defining BackgroundStyle also

                                property = (Dictionary<string, object>)TypeDescriptor.GetProperties(o, false)[context.PropertyDescriptor.Name].GetValue(o);
                                if (property != null)
                                {
                                    propertyArray[i] = property;
                                    i++;
                                }
                            }
                            if (propertyArray.Length > 0)
                            {
                                PropertyBagDialog propertyBagDialog = new PropertyBagDialog();
                                SetEditorProps(propertyArray[0], propertyBagDialog);
                                if (DialogResult.OK == editorService.ShowDialog(propertyBagDialog))
                                {
                                    foreach (Dictionary<string, object> property in propertyArray)
                                    {
                                        value = UpdateReturnValue(property, propertyBagDialog);
                                    }
                                    foreach (object obj in ctxtArray)
                                    {
                                        TypeDescriptor.GetProperties(obj, false)[context.PropertyDescriptor.Name].SetValue(obj, value);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // get exact property name
                        // -----------------------
                        // PropertyInfo is used for defining BackgroundStyle also

                        Dictionary<string, object> propertyBag = (Dictionary<string, object>)TypeDescriptor.GetProperties(
                            context.Instance, false)[context.PropertyDescriptor.Name].GetValue(context.Instance);

                        PropertyBagDialog propertyBagDialog = new PropertyBagDialog();
                        SetEditorProps(propertyBag, propertyBagDialog);

                        if (DialogResult.OK == editorService.ShowDialog(propertyBagDialog))
                        {
                            value = UpdateReturnValue(propertyBag, propertyBagDialog);
                            TypeDescriptor.GetProperties(context.Instance)[context.PropertyDescriptor.Name].SetValue(context.Instance, value);
                        }
                    }
                }
            }

            return value;
        }

        /// <summary>
        /// Gets the editor style used by the <see cref="M:System.Drawing.Design.UITypeEditor.EditValue(System.IServiceProvider,System.Object)"/> method.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that can be used to gain additional context information.</param>
        /// <returns>
        /// A <see cref="T:System.Drawing.Design.UITypeEditorEditStyle"/> value that indicates the style of editor used by the <see cref="M:System.Drawing.Design.UITypeEditor.EditValue(System.IServiceProvider,System.Object)"/> method. If the <see cref="T:System.Drawing.Design.UITypeEditor"/> does not support this method, then <see cref="M:System.Drawing.Design.UITypeEditor.GetEditStyle"/> will return <see cref="F:System.Drawing.Design.UITypeEditorEditStyle.None"/>.
        /// </returns>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context != null && context.Instance != null)
            {
                return UITypeEditorEditStyle.Modal;
            }
            return base.GetEditStyle(context);
        }
        #endregion
    }

    #region class Descriptors
    /// <summary>
    /// Instance for dynamic custom type information for an object
    /// </summary>
    public class DictionaryPropertyGridAdapter : ICustomTypeDescriptor
    {
        #region members
        IDictionary propertyDictionary;
        #endregion

        /// <summary>
        /// Initializes a new instance of the DictionaryPropertyGridAdapter class.
        /// </summary>
        /// <param name="dictionary"></param>
        public DictionaryPropertyGridAdapter(IDictionary dictionary)
        {
            propertyDictionary = dictionary;
        }

        /// <summary>
        /// Returns the name of this instance of a component.
        /// </summary>
        /// <returns>The name of the object, or null if the object does not have a name.</returns>
        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        /// <summary>
        /// Returns the default event for this instance of a component.
        /// </summary>
        /// <returns>An System.ComponentModel.EventDescriptor that represents the default event for this object, or null if this object does not have events.</returns>
        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        /// <summary>
        /// Returns the class name of this instance of a component.
        /// </summary>
        /// <returns>The class name of the object, or null if the class does not have a name.</returns>
        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        /// <summary>
        /// Returns the events for this instance of a component using the specified attribute array as a filter.
        /// </summary>
        /// <param name="attributes">An array of type System.Attribute that is used as a filter.</param>
        /// <returns>An System.ComponentModel.EventDescriptorCollection that represents the filtered events for this component instance.</returns>
        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        /// <summary>
        /// Returns the events for this instance of a component.
        /// </summary>
        /// <returns>An System.ComponentModel.EventDescriptorCollection that represents the events for this component instance.</returns>
        EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        /// <summary>
        /// Returns a type converter for this instance of a component.
        /// </summary>
        /// <returns>A System.ComponentModel.TypeConverter that is the converter for this object, or null if there is no System.ComponentModel.TypeConverter for this object.</returns>
        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        /// <summary>
        /// Returns an object that contains the property described by the specified property descriptor.
        /// </summary>
        /// <param name="pd"> A System.ComponentModel.PropertyDescriptor that represents the property whose owner is to be found.</param>
        /// <returns>An System.Object that represents the owner of the specified property.</returns>
        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return propertyDictionary;
        }

        /// <summary>
        /// Returns a collection of custom attributes for this instance of a component.
        /// </summary>
        /// <returns>An System.ComponentModel.AttributeCollection containing the attributes for this object</returns>
        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        /// <summary>
        /// Returns an editor of the specified type for this instance of a component.
        /// </summary>
        /// <param name="editorBaseType">A System.Type that represents the editor for this object.</param>
        /// <returns>An System.Object of the specified type that is the editor for this object</returns>
        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        /// <summary>
        /// Returns the default property for this instance of a component.
        /// </summary>
        /// <returns>A System.ComponentModel.PropertyDescriptor that represents the default property  for this object, or null if this object does not have properties.</returns>
        public PropertyDescriptor GetDefaultProperty()
        {
            return null;
        }

        /// <summary>
        /// Returns the properties for this instance of a component.
        /// </summary>
        /// <returns>A System.ComponentModel.PropertyDescriptorCollection that represents the properties for this component instance.</returns>
        PropertyDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetProperties()
        {
            return ((ICustomTypeDescriptor)this).GetProperties(new Attribute[0]);
        }

        /// <summary>
        /// Returns the properties for this instance of a component using the attribute array as a filter.
        /// </summary>
        /// <param name="attributes"> An array of type System.Attribute that is used as a filter.</param>
        /// <returns>A System.ComponentModel.PropertyDescriptorCollection that represents the filtered properties for this component instance.</returns>
        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            ArrayList properties = new ArrayList();
            foreach (DictionaryEntry e in propertyDictionary)
            {
                properties.Add(new DictionaryPropertyDescriptor(propertyDictionary, e.Key));
            }

            PropertyDescriptor[] props =
                (PropertyDescriptor[])properties.ToArray(typeof(PropertyDescriptor));

            return new PropertyDescriptorCollection(props);
        }
    }

    /// <summary>
    /// PropertyDescriptor instance for user defined property collection
    /// </summary>
    public class DictionaryPropertyDescriptor : PropertyDescriptor
    {
        #region members
        IDictionary propertyDictionary;
        object propertyKey;
        #endregion

        /// <summary>
        /// Initializes a new instance of the System.ComponentModel.DictionaryPropertyDescriptor class with the name and attributes in the specified System.ComponentModel.MemberDescriptor.
        /// </summary>
        /// <param name="dictionary">The Dictionary object</param>
        /// <param name="key">key</param>
        internal DictionaryPropertyDescriptor(IDictionary dictionary, object key)
            : base(key.ToString(), null)
        {
            propertyDictionary = dictionary;
            propertyKey = key;
        }

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        public override Type PropertyType
        {
            get { return propertyDictionary[propertyKey].GetType(); }
        }

        /// <summary>
        /// Sets the value of the component to a different value.
        /// </summary>
        /// <param name="component">The component with the property value that is to be set.</param>
        /// <param name="value">The new value</param>
        public override void SetValue(object component, object value)
        {
            propertyDictionary[propertyKey] = value;
        }

        /// <summary>
        /// Gets the current value of the property on a component.
        /// </summary>
        /// <param name="component">The component with the property for which to retrieve the value.</param>
        /// <returns>The value of a property for a given component.</returns>
        public override object GetValue(object component)
        {
            return propertyDictionary[propertyKey];
        }

        /// <summary>
        /// Gets a value indicating whether this property is read-only.
        /// </summary>
        public override bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the type of the component this property is bound to.
        /// </summary>
        public override Type ComponentType
        {
            get { return null; }
        }

        /// <summary>
        /// Returns whether resetting an object changes its value.
        /// </summary>
        /// <param name="component">The component to test for reset capability.</param>
        /// <returns>true if resetting the component changes its value; otherwise, false.</returns>
        public override bool CanResetValue(object component)
        {
            return false;
        }

        /// <summary>
        /// sets the value of the component to a different value.
        /// </summary>
        /// <param name="component">The component with the property value that is to be set.</param>
        public override void ResetValue(object component)
        {
        }

        /// <summary>
        /// Determines a value indicating whether the value of this property needs to be persisted.
        /// </summary>
        /// <param name="component">The component with the property to be examined for persistence.</param>
        /// <returns>true if the property should be persisted; otherwise, false.</returns>
        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
    #endregion
}
