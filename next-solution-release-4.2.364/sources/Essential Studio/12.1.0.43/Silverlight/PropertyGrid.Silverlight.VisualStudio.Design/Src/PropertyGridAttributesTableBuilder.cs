#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Features;
using System.ComponentModel;
using Syncfusion.Windows.PropertyGrid;

namespace Syncfusion.PropertyGrid.Silverlight.VisualStudio.Design
{
    internal class PropertyGridAttributesTableBuilder : AttributeTableBuilder
    {
        public PropertyGridAttributesTableBuilder()
            : base()
        {
            AddRichTextRibbonUIAttributes();
        }

        private void AddRichTextRibbonUIAttributes()
        {
            AddCallback(typeof(Syncfusion.Windows.PropertyGrid.PropertyGrid), delegate(AttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes(new Attribute[]{
                          new FeatureAttribute(typeof(PropertyGridInitializer))
                         ,new ComplexBindingPropertiesAttribute("", "")
                         ,new LookupBindingPropertiesAttribute("", "", "", "")
                    }
                );
            });
        }
    }
}
