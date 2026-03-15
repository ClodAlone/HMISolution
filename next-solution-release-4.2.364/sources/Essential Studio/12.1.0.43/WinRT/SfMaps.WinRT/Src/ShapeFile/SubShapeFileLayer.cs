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

#if WINRT
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
#else
using System.Windows.Media;
#endif


namespace Syncfusion.UI.Xaml.Maps
{
    public class SubShapeFileLayer : ShapeFileLayer
    {       

        public SubShapeFileLayer()
        {
            this.isBaseLayer = false;
            this.Background = new SolidColorBrush(Colors.Transparent);
        }
    }
}
