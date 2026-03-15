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
using Syncfusion.Windows.Design;

namespace Syncfusion.Gauge.WPF.Expression.Design
{
    public class CircularScaleAdornerProvider : PrimarySelectionAdornerProviderBase
	{
		protected override SmartTagBase CreateSmartTag( ModelItem item )
		{
            CircularScaleSmartTag circularGaugeSmartTag = new CircularScaleSmartTag();

            circularGaugeSmartTag.ModelItem = item;
            circularGaugeSmartTag.Context = base.Context;

            return circularGaugeSmartTag;
		}
	}
}
