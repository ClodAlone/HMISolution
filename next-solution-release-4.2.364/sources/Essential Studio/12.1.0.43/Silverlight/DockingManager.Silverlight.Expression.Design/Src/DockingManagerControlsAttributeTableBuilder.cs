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
using Syncfusion.DockingManager.Silverlight;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Features;
using Syncfusion.Windows.Tools.Controls;
using System.ComponentModel;

namespace Syncfusion.DockingManager.Silverlight.Expression.Design
{
    class DockingManagerControlsAttributeTableBuilder : AttributeTableBuilder
    {
        public DockingManagerControlsAttributeTableBuilder()
            : base()
        {
            AddCallback(typeof(Syncfusion.Windows.Tools.Controls.DockingManager), delegate(AttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes(
                      new FeatureAttribute(typeof(DockManagerInitializer))
                    , new ComplexBindingPropertiesAttribute("", "")
                    , new LookupBindingPropertiesAttribute("", "", "", "")
                    );
            });
        }
      
    
    }
}
