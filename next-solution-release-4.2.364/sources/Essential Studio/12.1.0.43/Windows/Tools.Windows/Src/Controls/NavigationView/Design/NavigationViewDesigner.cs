#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Text;
using System.Windows.Forms.Design;

namespace Syncfusion.Windows.Forms.Tools.Navigation.Design
{
    internal class NavigationViewDesigner :
        ControlDesigner
    {
    }

    internal class NavigationViewTypeDescriptionProvider :
        TypeDescriptionProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationViewTypeDescriptionProvider"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public NavigationViewTypeDescriptionProvider(Type type) :
            base(TypeDescriptor.GetProvider(type.BaseType))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationViewTypeDescriptionProvider"/> class.
        /// </summary>
        public NavigationViewTypeDescriptionProvider() :
            base(TypeDescriptor.GetProvider(typeof(NavigationView)))
        {
        }

        /// <summary>
        /// Gets a custom type descriptor for the given type and object.
        /// </summary>
        /// <param name="objectType">The type of object for which to retrieve the type descriptor.</param>
        /// <param name="instance">An instance of the type. Can be null if no instance was passed to the <see cref="T:System.ComponentModel.TypeDescriptor"/>.</param>
        /// <returns>
        /// An <see cref="T:System.ComponentModel.ICustomTypeDescriptor"/> that can provide metadata for the type.
        /// </returns>
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            ICustomTypeDescriptor typeDesc = base.GetTypeDescriptor(objectType, instance);

            if (typeDesc != null)
            {
                typeDesc = new NavigationViewTypeDescriptor(typeDesc, (NavigationView)instance);
            }

            return typeDesc;
        }
    }

    internal class NavigationViewTypeDescriptor :
        CustomTypeDescriptor
    {
        private NavigationView _instance = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationViewTypeDescriptor"/> class.
        /// </summary>
        /// <param name="parent">The parent custom type descriptor.</param>
        /// <param name="instance">Navigation View</param>
        public NavigationViewTypeDescriptor(ICustomTypeDescriptor parent, NavigationView instance) :
            base(parent)
        {
            _instance = instance;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationViewTypeDescriptor"/> class.
        /// </summary>
        /// <param name="parent">The parent custom type descriptor.</param>
        public NavigationViewTypeDescriptor(ICustomTypeDescriptor parent) :
            base(parent)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationViewTypeDescriptor"/> class.
        /// </summary>
        public NavigationViewTypeDescriptor()
        {
        }

        /// <summary>
        /// Returns a filtered collection of property descriptors for the object represented by this type descriptor.
        /// </summary>
        /// <param name="attributes">An array of attributes to use as a filter. This can be null.</param>
        /// <returns>
        /// A <see cref="T:System.ComponentModel.PropertyDescriptorCollection"/> containing the property descriptions for the object represented by this type descriptor. The default is <see cref="F:System.ComponentModel.PropertyDescriptorCollection.Empty"/>.
        /// </returns>
        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            PropertyDescriptorCollection baseProps = base.GetProperties(attributes);

            return FilterProperties(baseProps);
        }

        /// <summary>
        /// Returns a collection of property descriptors for the object represented by this type descriptor.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.ComponentModel.PropertyDescriptorCollection"/> containing the property descriptions for the object represented by this type descriptor. The default is <see cref="F:System.ComponentModel.PropertyDescriptorCollection.Empty"/>.
        /// </returns>
        public override PropertyDescriptorCollection GetProperties()
        {
            PropertyDescriptorCollection baseProps = base.GetProperties();

            return FilterProperties(baseProps);
        }

        private PropertyDescriptorCollection FilterProperties(PropertyDescriptorCollection baseProps)
        {
            PropertyDescriptorCollection props = baseProps;

            if (_instance != null && _instance.VisualStyle == VisualStyles.Vista)
            {
                List<PropertyDescriptor> list = new List<PropertyDescriptor>(baseProps.Count);

                foreach (PropertyDescriptor pd in baseProps)
                {
                    if (pd.Name != "BorderColor" && pd.Name != "Office2007ColorTheme")
                    {
                        list.Add(pd);
                    }
                }

                props = new PropertyDescriptorCollection(list.ToArray());
            }

            return props;
        }
    }
}
