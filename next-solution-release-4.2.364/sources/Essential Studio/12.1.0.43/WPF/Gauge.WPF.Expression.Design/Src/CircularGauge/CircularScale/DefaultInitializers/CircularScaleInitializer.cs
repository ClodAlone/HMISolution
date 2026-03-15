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
using Syncfusion.Windows.Gauge;
using System.Windows.Media;
using System.Windows;

namespace Syncfusion.Gauge.WPF.Expression.Design
{
    internal class CircularScaleInitializer : DefaultInitializer
    {
        public CircularScaleInitializer()
        {
        }

        public override void InitializeDefaults( ModelItem item )
        {
            using( ModelEditingScope scope = item.BeginEdit( ) )
            {
                item.Properties["Name"].SetValue("circularscale1");
            }

           

        }
    }
}
