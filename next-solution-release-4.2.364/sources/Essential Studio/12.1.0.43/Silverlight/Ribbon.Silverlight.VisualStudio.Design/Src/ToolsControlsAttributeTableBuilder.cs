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
using Syncfusion.Ribbon.Silverlight.VisualStudio.Design.Tools;
using Syncfusion.Windows.Tools.Controls;
using Microsoft.Windows.Design.Features;
using System.ComponentModel;

namespace Syncfusion.Ribbon.Silverlight.VisualStudio.Design
{
    internal class ToolsControlsAttributeTableBuilder: AttributeTableBuilder
    {
        public ToolsControlsAttributeTableBuilder()
            : base()
        {
            AddAllRibbonAttributes();
        }

        private void AddAllRibbonAttributes()
        {
            AddRibbonControlAttributes();
        }

        private void AddRibbonControlAttributes()
        {
            AddCallback(typeof(Syncfusion.Windows.Tools.Controls.Ribbon), delegate(AttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes(
                      new FeatureAttribute(typeof(RibbonInitializer))
                    , new ComplexBindingPropertiesAttribute("", "")
                    , new LookupBindingPropertiesAttribute("", "", "", "")
                    );
            });

            AddCallback(typeof(RibbonBar), delegate(AttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes(
                      new FeatureAttribute(typeof(RibbonBarInitializer))
                    , new ComplexBindingPropertiesAttribute("", "")
                    , new LookupBindingPropertiesAttribute("", "", "", "")
                    );
            });

            AddCallback(typeof(RibbonTab), delegate(AttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes(
                      new FeatureAttribute(typeof(RibbonTabInitializer))
                    , new ComplexBindingPropertiesAttribute("", "")
                    , new LookupBindingPropertiesAttribute("", "", "", "")
                    );
            });
        }
    }
}
