#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;

#if SILVERLIGHT
using Syncfusion.Windows.Controls.Theming;
#endif

namespace Syncfusion.Windows.PropertyGrid
{
    public class ResourceManager
    {
        public ResourceManager()
        {
        }
#if SILVERLIGHT
        public static DataTemplate GetValueTemplate(Type type, String visualstyle)
        {
#if SyncfusionFramework3_5

#endif

           
            ResourceDictionary resources = new ResourceDictionary()
            {
                Source = new Uri("/Syncfusion.PropertyGrid.Silverlight;component/Themes/DataTemplates.xaml", UriKind.RelativeOrAbsolute)
            };
            String templalte = visualstyle + "Template";
            return resources[templalte] as DataTemplate;
        }
#endif

#if WPF
        public static DataTemplate GetValueTemplate(Type type)
        {
#if SyncfusionFramework3_5

#endif
            try
            {

                ResourceDictionary resources = new ResourceDictionary();

                resources.Source = new Uri("/Syncfusion.PropertyGrid.WPF;component/Themes/DataTemlates.xaml", UriKind.RelativeOrAbsolute);

                return resources["DefaultTemplate"] as DataTemplate;
            }
            catch
            {
                return null;
            }

        }
#endif
    }
}
