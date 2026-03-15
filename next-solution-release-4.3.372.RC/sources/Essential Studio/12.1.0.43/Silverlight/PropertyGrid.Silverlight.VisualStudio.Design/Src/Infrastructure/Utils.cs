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
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Services;

namespace Syncfusion.PropertyGrid.Silverlight.VisualStudio.Design.Infrastructure
{
    internal class Utils
    {
        internal static void SparseSetValue(ModelProperty property, object value)
        {
            if (object.Equals(property.DefaultValue, value))
            {
                if (property.IsSet)
                {
                    property.ClearValue();
                }
            }
            else
            {
                property.SetValue(value);
            }
        }

        internal static void InvalidateProperty(
            ModelItem item,
            Microsoft.Windows.Design.Metadata.PropertyIdentifier propertyIdentifier)
        {
            item.Context.Services.GetRequiredService<ValueTranslationService>().InvalidateProperty(item, propertyIdentifier);
        }
    }
}
