// <copyright file="CircularPanel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
#if !(WINDOWS_PHONE_7 || Silverlight4)
using System.Threading.Tasks;
#endif

#if WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows.Controls;
using System.Windows;
using Syncfusion.WP.Controls.Navigation;
using System.Windows.Media;

namespace Syncfusion.WP.Controls.Navigation
#elif SILVERLIGHT
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace Syncfusion.Tools.Controls.Navigation
#elif WPF
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
namespace Syncfusion.Windows.Controls.Navigation
#else
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Syncfusion.UI.Xaml.Controls.Navigation
#endif
{
    /// <summary>
    /// Represents an Panel that contains the items the user can select from.
    /// <see cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfRadialSlider"/>
    /// </summary>
    /// <remarks>
    /// <para></para>
    /// </remarks>
    [ClassReference(IsReviewed = false, ShouldInclude = false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class CircularPanel : Panel
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.CircularPanel"/> class.
        /// </summary>
        public CircularPanel()
        {
            this.Loaded += CircularPanel_Loaded;
        }

        #endregion

        #region Variables

        double maxWidth = 0.0;
        double maxHeight = 0.0;

        private SfRadialMenu parentItemsControl;

        internal SfRadialMenu ParentItemsControl
        {
            get
            {
                if (parentItemsControl == null)
                {
                    parentItemsControl = ItemsControl.GetItemsOwner(this) as SfRadialMenu;
                }
                return parentItemsControl;
            }
        }

        #endregion    

        #region Helper Methods

        void CircularPanel_Loaded(object sender, RoutedEventArgs e)
        {
            if (ParentItemsControl != null)
            {
                ParentItemsControl.circularpanel = this;
            }
            else
            {
                parentItemsControl = GetParentItem(this as DependencyObject) as SfRadialMenu;
                parentItemsControl.circularpanel = this;
            }
        }

        private object GetParentItem(DependencyObject obj)
        {
            var item = obj;
            while (VisualTreeHelper.GetParent(item) != null && !(VisualTreeHelper.GetParent(item) is SfRadialMenu))
            {
                item = VisualTreeHelper.GetParent(item);
            }
            if (VisualTreeHelper.GetParent(item) is SfRadialMenu)
                return VisualTreeHelper.GetParent(item);
            return item;
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Gets the size for an overrided item
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            maxWidth = 0d;
            maxHeight = 0d;
#if WINDOWS_PHONE||WINDOWS_PHONE_7
            itemIndex = 0;
#endif
            foreach (UIElement element in Children)
            {
                element.Measure(availableSize);
                maxWidth = Math.Max(maxWidth, element.DesiredSize.Width);
                maxHeight = Math.Max(maxHeight, element.DesiredSize.Height);
            }

            return base.MeasureOverride(availableSize);
        }
#if WINDOWS_PHONE||WINDOWS_PHONE_7
        private int itemIndex = 0;
#endif
#if SILVERLIGHT||WINDOWS_PHONE_7||WPF
        bool isset;
        Size size;
#endif
        bool isRadialColorItemHost = false;
        /// <summary>
        /// Gets the size for an overrided item
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
           
            double colorItemAngle = 0d;
         
            double colorItemAngularSpace = 0d;
            if (Children.Count > 1)
                colorItemAngularSpace = (360.0 / Children.Count);
            else
                colorItemAngularSpace = (360.0 / 2);

            colorItemAngle = (-colorItemAngularSpace/2);

#if WINDOWS_PHONE||WINDOWS_PHONE_7
            double menuItemradiusx, menuItemradiusy, menuItemAngle, menuItemAngularSpace;
            if (!isRadialColorItemHost)
            {
                menuItemradiusx= (finalSize.Width/2);
                menuItemradiusy = (finalSize.Height/2);
                menuItemAngle = 180d;
                menuItemAngularSpace = (360.0/Children.Count);
            }
            else
            {
                menuItemradiusx = (finalSize.Width / 2) - maxWidth;
                menuItemradiusy = (finalSize.Height / 2) - maxHeight;
                menuItemAngle = (360.0 / Children.Count);
                menuItemAngularSpace = (360.0 / Children.Count) * Math.PI / 180;
            }
#else
            double menuItemradiusx, menuItemradiusy;
            if (isRadialColorItemHost)
            {
                 menuItemradiusx= (finalSize.Width/2.4);
                 menuItemradiusy = (finalSize.Height/2.4);
            }
            else
            {
                menuItemradiusx = (finalSize.Width / 2) - ((maxWidth / 2));
                menuItemradiusy = (finalSize.Height / 2) - ((maxHeight / 2));
            }
            double menuItemAngle = 0d;
            double menuItemAngularSpace = (360.0 / Children.Count) * (Math.PI / 180); ;
#endif
            double colorItemradiusx = (finalSize.Width/2);
            double colorItemradiusy = (finalSize.Height/2);
            double margin = 0d;
            InnerRim colorRim = ItemsControl.GetItemsOwner(this) as InnerRim;
            if (colorRim != null && colorRim.radialMenu != null && colorRim.radialMenu.DrillDownItem is SfRadialColorItem)
            {
                SfRadialColorItem radialColorItem = colorRim.radialMenu.DrillDownItem as SfRadialColorItem;
                if (radialColorItem != null && radialColorItem.Items.Count > 0)
                {
                    margin = radialColorItem.StrokeThickness/2.0;
                    int parentIndex = (radialColorItem.Parent as ItemsControl).Items.IndexOf(radialColorItem);
                    int i = 0;
                    foreach(dynamic item in radialColorItem.Items)
                    {
#if SILVERLIGHT||WINDOWS_PHONE_7
                        if (item is SfRadialColorItem && (item as SfRadialColorItem).Color == radialColorItem.Color)
#else
                        if (item is SfRadialColorItem && item.Color == radialColorItem.Color)
#endif
                        {
                            break;
                        }
                        i++;
                    }
#if WPF
                    var children = Children.Cast<UIElement>().ToList();
#else
                    var children = Children.ToList();
#endif
                    if (i < radialColorItem.Items.Count && !colorRim.isInnerItemsHost && parentIndex < radialColorItem.Items.Count)
                    {
                        UIElement tempEle = children[parentIndex];
                        children[parentIndex] = children[i];
                        children[i] = tempEle;
                    }
                    foreach (UIElement element in children)
                    {
#if SILVERLIGHT||WINDOWS_PHONE_7||WPF
                        if (element == children[0] && !isset)
                        {
                            isset = true;
                            size = finalSize;
                        }
#endif
                        if (element is SfRadialColorItem)
                        {
#if SILVERLIGHT||WINDOWS_PHONE_7||WPF
                            ArrangeColorItem(element, size, ref colorItemAngle, ref colorItemAngularSpace,
                                            ref colorItemradiusx, ref colorItemradiusy);
#else
                            ArrangeColorItem(element, finalSize, ref colorItemAngle, ref colorItemAngularSpace,
                                             ref colorItemradiusx, ref colorItemradiusy);
#endif
                        }
                        else if(element is SfRadialMenuItem)
                        {
#if SILVERLIGHT||WINDOWS_PHONE_7||WPF
                            ArrangeColorItem(new SfRadialColorItem(), size, ref colorItemAngle,
                                          ref colorItemAngularSpace,
                                          ref colorItemradiusx, ref colorItemradiusy);
#else
                            ArrangeColorItem(new SfRadialColorItem(), finalSize, ref colorItemAngle,
                                          ref colorItemAngularSpace,
                                          ref colorItemradiusx, ref colorItemradiusy);
#endif
                        }
                    }
                }
                this.Margin = new Thickness(margin);
            }
            else
            {
                isRadialColorItemHost = false;     
                foreach (UIElement element in Children)
                {
#if SILVERLIGHT||WINDOWS_PHONE_7||WPF
                    if (element == Children[0] && !isset)
                    {
                        isset = true;
                        size = finalSize;
                    }
#endif
                    if (element is SfRadialColorItem)
                        isRadialColorItemHost = true;
                }

                foreach (UIElement element in Children)
                {
#if SILVERLIGHT||WINDOWS_PHONE_7||WPF
                    if (element == Children[0] && !isset)
                    {
                        isset = true;
                        size = finalSize;
                    }
#endif
                    if (element is SfRadialColorItem)
                    {
#if SILVERLIGHT||WINDOWS_PHONE_7||WPF
                        ArrangeColorItem(element, size, ref colorItemAngle, ref colorItemAngularSpace,
                                        ref colorItemradiusx, ref colorItemradiusy);
#else
                        ArrangeColorItem(element, finalSize, ref colorItemAngle, ref colorItemAngularSpace,
                                         ref colorItemradiusx, ref colorItemradiusy);
#endif
                        margin = (element as SfRadialColorItem).StrokeThickness / 2.0;
                        this.Margin = new Thickness(margin);
                    }
                    else
                    {
                        if (element is SfRadialMenuItem || element is OuterRimItem)
                        {
                            ArrangeMenuItem(element, finalSize, ref menuItemAngle, menuItemAngularSpace, menuItemradiusx,
                                            menuItemradiusy);
                            if (element is SfRadialMenuItem)
                            {
#if SILVERLIGHT||WINDOWS_PHONE_7||WPF
                                ArrangeColorItem(new SfRadialColorItem(), size, ref colorItemAngle,
                                                 ref colorItemAngularSpace,
                                                 ref colorItemradiusx, ref colorItemradiusy);
#else
                                ArrangeColorItem(new SfRadialColorItem(), finalSize, ref colorItemAngle,
                                                 ref colorItemAngularSpace,
                                                 ref colorItemradiusx, ref colorItemradiusy);
#endif
                            }
                        }
                    }
                }
                this.Margin = new Thickness(margin);
            }
           
            return finalSize;
        }

        private void ArrangeMenuItem(UIElement element,Size finalSize,ref double angle,double angularSpace, double radiusx, double radiusy)
        {
#if WINDOWS_PHONE||WINDOWS_PHONE_7
            if (!isRadialColorItemHost)
            {
                double width = element.DesiredSize.Width/2.0;
                double height = element.DesiredSize.Height/2.0;
                double itemangle = (angularSpace*itemIndex++) + angle;

                if (itemangle > 360)
                    itemangle = (itemangle%360);

                if (element is SfRadialMenuItem)
                    (element as SfRadialMenuItem).Angle = itemangle;

                double x = (radiusx - element.DesiredSize.Width/2d)*Math.Cos((Math.PI*itemangle)/180.0);
                double y = (radiusy - element.DesiredSize.Height/2d)*Math.Sin((Math.PI*itemangle)/180.0);

                element.Arrange(new Rect((x + radiusx - width),
                                         (y + radiusy - height),
                                         element.DesiredSize.Width,
                                         element.DesiredSize.Height));
            }
            else
            {
#endif
				Point point = new Point(Math.Cos(angle) * radiusx, -Math.Sin(angle) * radiusy);
                Point actualChildPoint = new Point(finalSize.Width / 2 + point.X - element.DesiredSize.Width / 2, finalSize.Height / 2 + point.Y - element.DesiredSize.Height / 2);
                element.Arrange(new Rect(actualChildPoint.X, actualChildPoint.Y, element.DesiredSize.Width, element.DesiredSize.Height));

                OuterRimItem rimItem = element as OuterRimItem;
                if (rimItem != null)
                {
                    rimItem.Angle = -(angle / (Math.PI / 180));
                }
                //Clockwise
                angle -= angularSpace;
                //Counter-Clockwise
                //angle += angularSpace;
#if WINDOWS_PHONE||WINDOWS_PHONE_7
            }
#endif
            return;
          
        }
        
        private void ArrangeColorItem(UIElement element,Size finalSize,ref double angle,ref double angularSpace,ref double radiusx, ref double radiusy)
        {
            
                double startAngle = angle;
                double endAngle = angle + angularSpace - 0.5;
                double _endangle = angle + angularSpace + 1;

                if (element is SfRadialColorItem)
                {
                    SfRadialColorItem rimItem = element as SfRadialColorItem;
                    double x = radiusx + Math.Cos(DegToRad(endAngle)) * radiusx;
                    double y = radiusy + Math.Sin(DegToRad(endAngle)) * radiusy;

                    double _x = radiusx + Math.Cos(DegToRad(_endangle)) * radiusx;
                    double _y = radiusy + Math.Sin(DegToRad(_endangle)) * radiusy;

                    double startX = radiusx + Math.Cos(DegToRad(startAngle)) * radiusx;
                    double startY = radiusy + Math.Sin(DegToRad(startAngle)) * radiusy;

                    rimItem.StartPoint = new Point(startX, startY);
                    rimItem.RimSize = new Size(radiusx, radiusy);
                    rimItem.RimPoint = new Point(x, y);
                    rimItem.Point = new Point(_x, _y);
                    rimItem.Angle = angle + (angularSpace / 2);
#if SILVERLIGHT||WINDOWS_PHONE_7||WPF
                    element.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
#else
                    element.Arrange(new Rect(0, 0,  element.DesiredSize.Width, element.DesiredSize.Height));
#endif
                    angle += angularSpace;
                    if (angle > 359)
                        angle = 0;
               
            }

           
        }

        private double DegToRad(double deg)
        {
            return deg * Math.PI / 180;
        }

        #endregion
    }
}
