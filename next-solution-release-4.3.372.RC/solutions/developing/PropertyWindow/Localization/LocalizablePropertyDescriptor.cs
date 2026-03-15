using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PropertyControl.Localization
{
    /// <summary>
    /// LocalizablePropertyDescriptor enhances the base class bay obtaining the display name for a property
	/// from the resource.
	/// </summary>
    public sealed class LocalizablePropertyDescriptor : PropertyDescriptor
    {
        #region Declarations
        readonly PropertyDescriptor basePropertyDescriptor;
        Type attributeToFind;
        #endregion

        #region Constructors
        public LocalizablePropertyDescriptor(PropertyDescriptor basePropertyDescriptor)
            : base(basePropertyDescriptor)
        {
            this.basePropertyDescriptor = basePropertyDescriptor;
        }

        public LocalizablePropertyDescriptor(PropertyDescriptor basePropertyDescriptor, Type attributeToFind)
            : base(basePropertyDescriptor)
        {
            this.basePropertyDescriptor = basePropertyDescriptor;
            this.attributeToFind = attributeToFind;
        }
        #endregion

        #region Properties
        public string ResourceFileName
        {
            get { return basePropertyDescriptor.ComponentType.FullName; }
        }

        public PropertyDescriptor BasePropertyDescriptor
        {
            get
            {
                return basePropertyDescriptor;
            }
        }

        bool forceReadOnly;
        public bool ForceReadOnly
        {
            get
            {
                return forceReadOnly;
            }
            internal set
            {
                if (forceReadOnly == value)
                    return;
                forceReadOnly = value;
            }
        }
        #endregion

        #region Overrides abstracts
        //
        // Summary:
        //     When overridden in a derived class, returns whether resetting an object changes
        //     its value.
        //
        // Parameters:
        //   component:
        //     The component to test for reset capability.
        //
        // Returns:
        //     true if resetting the component changes its value; otherwise, false.
        public override bool CanResetValue(object component)
        {
            return basePropertyDescriptor.CanResetValue(component);
        }

        // Summary:
        //     When overridden in a derived class, gets the type of the component this property
        //     is bound to.
        //
        // Returns:
        //     A System.Type that represents the type of component this property is bound
        //     to. When the System.ComponentModel.PropertyDescriptor.GetValue(System.Object)
        //     or System.ComponentModel.PropertyDescriptor.SetValue(System.Object,System.Object)
        //     methods are invoked, the object specified might be an instance of this type.
        public override Type ComponentType
        {
            get { return basePropertyDescriptor.ComponentType; }
        }

        //
        // Summary:
        //     When overridden in a derived class, gets a value indicating whether this
        //     property is read-only.
        //
        // Returns:
        //     true if the property is read-only; otherwise, false.
        public override bool IsReadOnly
        {
            get { return ForceReadOnly || basePropertyDescriptor.IsReadOnly; }
        }

        //
        // Summary:
        //     When overridden in a derived class, gets the type of the property.
        //
        // Returns:
        //     A System.Type that represents the type of the property.
        public override Type PropertyType
        {
            get { return basePropertyDescriptor.PropertyType; }
        }

        //
        // Summary:
        //     When overridden in a derived class, resets the value for this property of
        //     the component to the default value.
        //
        // Parameters:
        //   component:
        //     The component with the property value that is to be reset to the default
        //     value.
        public override void ResetValue(object component)
        {
            try
            {
                basePropertyDescriptor.ResetValue(component);
            }
            catch (Exception)
            {
            }
        }

        //
        // Summary:
        //     When overridden in a derived class, gets the current value of the property
        //     on a component.
        //
        // Parameters:
        //   component:
        //     The component with the property for which to retrieve the value.
        //
        // Returns:
        //     The value of a property for a given component.
        public override object GetValue(object component)
        {
            return basePropertyDescriptor.GetValue(component);
        }

        //
        // Summary:
        //     When overridden in a derived class, sets the value of the component to a
        //     different value.
        //
        // Parameters:
        //   component:
        //     The component with the property value that is to be set.
        //
        //   value:
        //     The new value.
        public override void SetValue(object component, object value)
        {
            try
            {
                basePropertyDescriptor.SetValue(component, value);
            }
            catch(Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }
        
        //
        // Summary:
        //     When overridden in a derived class, determines a value indicating whether
        //     the value of this property needs to be persisted.
        //
        // Parameters:
        //   component:
        //     The component with the property to be examined for persistence.
        //
        // Returns:
        //     true if the property should be persisted; otherwise, false.
        public override bool ShouldSerializeValue(object component)
        {
            return basePropertyDescriptor.ShouldSerializeValue(component);
        }
        #endregion

        #region Overrides virtuals
        string displayName;
        //
        // Summary:
        //     Gets the name that can be displayed in a window, such as a Properties window.
        //
        // Returns:
        //     The name to display for the member.
        public override string DisplayName
        {
            get
            {
                if (displayName == null)
                {
                    if (attributeToFind != null)
                    {
                        string nameToFind = GetAttributeName();
                        if (nameToFind != String.Empty)
                            displayName = LocalizablePropertyDescriptorCollection.GetString(this, String.Format("{0}_{1}", basePropertyDescriptor.Name, nameToFind));
                        else
                            displayName = LocalizablePropertyDescriptorCollection.GetString(this, basePropertyDescriptor.Name);
                    }
                    else
                        displayName = LocalizablePropertyDescriptorCollection.GetString(this, basePropertyDescriptor.Name);

                    if (displayName == null)
                        displayName = basePropertyDescriptor.DisplayName;
                }

                return displayName;
            }
        }

        string description;
        //
        // Summary:
        //     Gets the description of the member, as specified in the System.ComponentModel.DescriptionAttribute.
        //
        // Returns:
        //     The description of the member. If there is no System.ComponentModel.DescriptionAttribute,
        //     the property value is set to the default, which is an empty string ("").
        public override string Description
        {
            get
            {
                if (description == null)
                {
                    if (attributeToFind != null)
                    {
                        string nameToFind = GetAttributeName();
                        if (nameToFind != String.Empty)
                            description = String.Format("{0}_Help", String.Format("{0}_{1}", basePropertyDescriptor.Name, nameToFind));
                        else
                            description = String.Format("{0}_Help", basePropertyDescriptor.Name);
                    }
                    else
                        description = String.Format("{0}_Help", basePropertyDescriptor.Name);

                    description = LocalizablePropertyDescriptorCollection.GetString(this, description);
                    if (description == null)
                        description = LocalizablePropertyDescriptorCollection.GetString(this, String.Format("{0}_Help", basePropertyDescriptor.Name));
                    if (description == null)
                        description = basePropertyDescriptor.Description;
                    if (description == null)
                        description = basePropertyDescriptor.DisplayName;
                }

                return description;
            }
        }

        string category;
        //
        // Summary:
        //     Gets the name of the category to which the member belongs, as specified in
        //     the System.ComponentModel.CategoryAttribute.
        //
        // Returns:
        //     The name of the category to which the member belongs. If there is no System.ComponentModel.CategoryAttribute,
        //     the category name is set to the default category, Misc.
        public override string Category
        {
            get
            {
                if (category == null)
                {
                    //category = basePropertyDescriptor.Category;
                    //if (String.IsNullOrEmpty(category) || category == CategoryAttribute.Default.Category)
                    
                    if (attributeToFind != null)
                    {
                        string nameToFind = GetAttributeName();
                        if (nameToFind != String.Empty)
                            category = String.Format("{0}_Category", String.Format("{0}_{1}", basePropertyDescriptor.Name, nameToFind));
                        else
                            category = String.Format("{0}_Category", basePropertyDescriptor.Name);
                    }
                    else
                        category = String.Format("{0}_Category", basePropertyDescriptor.Name);

                    category = LocalizablePropertyDescriptorCollection.GetString(this, category);
                    if (category == null)
                        category = LocalizablePropertyDescriptorCollection.GetString(this, String.Format("{0}_Category", basePropertyDescriptor.Name));
                    if (category == null)
                        category = LocalizablePropertyDescriptorCollection.GetString(this, basePropertyDescriptor.Category);
                    if (category == null && basePropertyDescriptor.Category == CategoryAttribute.Default.Category)
                        category = Properties.Resources.DefaultCategoryDisplayName;
                    else if (category == null)
                        category = basePropertyDescriptor.Category;
                }

                return category;
            }
        }
        #endregion

        #region Helpers
        internal void ChangeDisplayName(string value)
        {
            displayName = value;
        }

        internal void ChangeDescription(string value)
        {
            description = value;
        }

        internal void ChangeCategory(string value)
        {
            category = value;
        }

        internal string GetAttributeName()
        {
            if (attributeToFind != null)
            {
                foreach (Attribute att in Attributes)
                {
                    if (att.GetType() == attributeToFind)
                        return att.ToString();
                }
            }
            return String.Empty;
        }

        internal void UpdateAttributeToFind(Type attrType)
        {
            attributeToFind = attrType;
        }
        #endregion

        #region Helpers
        internal void UpdateLocalizationResourceFiles()
        {
            var resourceManager = new PropertyResourceManager(ResourceFileName);
            var items = resourceManager.LoadFromXml();

            var key = "";
            var propertyName = "";
            var attName = GetAttributeName();
            if (attName == String.Empty)
                propertyName = BasePropertyDescriptor.Name;
            else
                propertyName = String.Format("{0}_{1}", BasePropertyDescriptor.Name, attName);

            if (!String.IsNullOrEmpty(DisplayName))
                items[propertyName] = DisplayName;
            else
                items[propertyName] = BasePropertyDescriptor.DisplayName;

            key = String.Format("{0}_Help", propertyName);
            if (!String.IsNullOrEmpty(Description))
                items[key] = Description;
            else
                items[key] = items[BasePropertyDescriptor.Name];

            key = String.Format("{0}_Category", propertyName);
            if (!String.IsNullOrEmpty(Category))
                items[key] = Category;
            else
                items.Remove(key);

            //foreach (Attribute attribute in Attributes)
            //{
            //    if (attribute is Utilities.DisplayNameExtension)
            //    {
            //        items[attribute.ToString()] = DisplayName;
            //        items[String.Format("{0}_Help", attribute.ToString())] = Description;
            //        items[String.Format("{0}_Category", attribute.ToString())] = Category;
            //    }
            //}

            resourceManager.WriteToXml(items);
        }
        #endregion
    }
}
