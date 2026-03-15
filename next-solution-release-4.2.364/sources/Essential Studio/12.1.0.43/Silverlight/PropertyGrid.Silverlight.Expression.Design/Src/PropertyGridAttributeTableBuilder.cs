#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.PropertyEditing;
using Microsoft.Windows.Design;
using System.Windows;
using Syncfusion.Windows.PropertyGrid;

namespace Syncfusion.PropertyGrid.Silverlight.Expression.Design
{
    internal class PropertyGridAttributeTableBuilder : AttributeTableBuilder
    {
        public PropertyGridAttributeTableBuilder()
            : base()
        {
            AddAllToolsAttributes();
        }

        private void AddAllToolsAttributes()
        {
            AddToolsControlAttributes();
        }

        private void AddToolsControlAttributes()
        {
            AddCallback(typeof(Syncfusion.Windows.PropertyGrid.PropertyGrid), delegate(AttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes(
                      new FeatureAttribute(typeof(PropertyGridInitializer))
                    , new ComplexBindingPropertiesAttribute("", "")
                    , new LookupBindingPropertiesAttribute("", "", "", "")
                    , new NewItemTypesAttribute(typeof(string))
                    );
            });
        }
    }
}
