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
using Syncfusion.Windows.Shared;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Syncfusion.Shared.WPF.VisualStudio.Design
{
    internal class CarouselDefaultInitializer : DefaultInitializer
    {
           public CarouselDefaultInitializer()
        {
        }

           public override void InitializeDefaults(ModelItem item)
           {
               using (ModelEditingScope scope = item.BeginEdit())
               {
                   //Carousel  carousel = new Carousel();
                   CarouselItem carouselitem = new CarouselItem();
                   //carousel.Items.Add(carouselitem);
                   CarouselItem carouselitem1 = new CarouselItem();
                   //carousel.Items.Add(carouselitem1);
                   CarouselItem carouselitem2 = new CarouselItem();
                   CarouselItem carouselitem3 = new CarouselItem();
                   CarouselItem carouselitem4 = new CarouselItem();
                 
                                                  
                   
                   Border border = new Border();
                   border.Height = 50;
                   border.Background = Brushes.Gray; 
                   border.Width = 50;
                   carouselitem.Content = border;

                   //carousel.Items.Add(border);

                   Border border1 = new Border();
                   border1.Height = 50;
                   border1.Background = Brushes.Gray;
                   border1.Width = 50;
                   border1.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                   border1.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                   carouselitem1.Content = border1;
                   //carousel.Items.Add(border1);

                   Border border2 = new Border();
                   border2.Height = 50;
                   border2.Background = Brushes.Gray;
                   border2.Width = 50;
                   border2.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                   border2.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                   carouselitem2.Content = border2;
                  // carousel.Items.Add(border2);

                   Border border3 = new Border();
                   border3.Height = 50;
                   border3.Background = Brushes.Gray;
                   border3.Width = 50;
                   border3.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                   border3.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                   carouselitem3.Content = border3;
                   //carousel.Items.Add(border3);

                   Border border4 = new Border();
                   border4.Height = 50;
                   border4.Background = Brushes.Gray;
                   border4.Width = 50;
                   border4.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                   border4.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                   //carousel.Items.Add(border4);
                   carouselitem4.Content = border4;

                   item.Properties["Height"].SetValue(100d);
                   item.Properties["Width"].SetValue(100d);
                   item.Properties["RadiusX"].SetValue(100d);
                   item.Properties["RadiusY"].SetValue(-50d);
                   item.Properties["VerticalAlignment"].SetValue(System.Windows.VerticalAlignment.Center);
                   item.Properties["HorizontalAlignment"].SetValue(System.Windows.HorizontalAlignment.Center);
                   item.Properties["ItemsPerPage"].SetValue(3);
                   item.Properties["SelectedIndex"].SetValue(0);

                   //item.Properties["Items"].Collection.Add(carouselitem);
                   //item.Properties["Items"].Collection.Add(carouselitem1);
                   //item.Properties["Items"].Collection.Add(carouselitem2);
                   //item.Properties["Items"].Collection.Add(carouselitem3);
                   //item.Properties["Items"].Collection.Add(carouselitem4);

                   //item.Properties["Items"].Collection[0].Properties["Content"].SetValue(border);
                   //item.Properties["Items"].Collection[1].Properties["Content"].SetValue(border1);
                   //item.Properties["Items"].Collection[2].Properties["Content"].SetValue(border2);
                   //item.Properties["Items"].Collection[3].Properties["Content"].SetValue(border3);
                   //item.Properties["Items"].Collection[4].Properties["Content"].SetValue(border4);

                  

                   
                   scope.Complete();
               }
           }
    }
}
