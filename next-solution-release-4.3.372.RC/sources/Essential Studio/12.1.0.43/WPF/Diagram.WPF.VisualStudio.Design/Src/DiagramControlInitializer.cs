#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design;
using Syncfusion.Windows.Diagram;
using Microsoft.Windows.Design.Services;
using System.Windows.Media;
using System.Windows;


namespace Syncfusion.Diagram.WPF.VisualStudio.Design
{
    internal class DiagramControlInitializer : DefaultInitializer
    {
        public DiagramControlInitializer()
            : base() 
        {
        }

        public override void InitializeDefaults(ModelItem item)
        {
            //DiagramModel model = new DiagramModel();
            //DiagramView view = new DiagramView();
            //view.ClearValue(DiagramView.PageProperty);
            Utils.SparseSetValue(item.Properties["Background"], Brushes.White);
            Utils.SparseSetValue(item.Properties["BorderBrush"], Brushes.Black);
            Utils.SparseSetValue(item.Properties["BorderThickness"], new Thickness(1));
        }
    }

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
    }
}
