// <copyright file="OuterRimPanel.cs" company="Syncfusion">
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

namespace Syncfusion.WP.Controls.Navigation
#elif SILVERLIGHT
using System.Windows.Controls;
using System.Windows;

namespace Syncfusion.Tools.Controls.Navigation
#elif WPF
using System.Windows.Controls;
using System.Windows;
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
    /// Represents a Panel for the outer rim that allows the user to select an item in
    /// <see cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfRadialSlider"/> control.
    /// </summary>
    /// <remarks>
    /// <para></para>
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [ClassReference(IsReviewed = false, ShouldInclude = false)]
    public sealed class OuterRimPanel : Panel
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.OuterRimPanel"/> class.
        /// </summary>
        public OuterRimPanel()
        {
            this.Loaded += OnLoaded;
        }

        #endregion

        #region Variables

        private OuterRim parentItemsControl;

        internal OuterRim ParentItemsControl
        {
            get
            {
                if (parentItemsControl == null)
                {
                    parentItemsControl = ItemsControl.GetItemsOwner(this) as OuterRim;
                }
                return parentItemsControl;
            }
        }

        #endregion

        #region Helper Methods

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (ParentItemsControl != null)
            {
                ParentItemsControl.rimPanel = this;
            }
        }

        private double DegToRad(double deg)
        {
            return deg * Math.PI / 180;
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
            foreach (UIElement element in Children)
            {
                element.Measure(availableSize);
            }

            return base.MeasureOverride(availableSize);
        }

        /// <summary>
        /// Gets and sets the size of the item 
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            double angularSpace;
            if (Children.Count > 1)
                angularSpace = (360.0 / Children.Count);
            else
                angularSpace = (360.0 / 2);

            double arrowangle = 0;
            double angle = -angularSpace / 2;

            double radiusx = finalSize.Width / 2;
            double radiusy = finalSize.Height / 2;

            OuterRim colorRim = ItemsControl.GetItemsOwner(this) as OuterRim;
#if WPF
            var children = Children.Cast<UIElement>().ToList();
#else
            var children = Children.ToList(); 
#endif
            if (colorRim != null && colorRim.radialMenu != null && colorRim.radialMenu.DrillDownItem is SfRadialColorItem)
            {
                SfRadialColorItem radialColorItem = colorRim.radialMenu.DrillDownItem as SfRadialColorItem;
                if (radialColorItem != null && radialColorItem.Items.Count > 0 && radialColorItem.Parent != null)
                {
                    int parentIndex = (radialColorItem.Parent as ItemsControl).Items.IndexOf(radialColorItem);
                    int i = 0;
                    foreach (SfRadialColorItem ri in radialColorItem.Items)
                    {
                        if (ri.Color == radialColorItem.Color)
                            break;
                        i++;
                    }
                    if (i < radialColorItem.Items.Count && parentIndex < radialColorItem.Items.Count)
                    {   UIElement tempEle = children[parentIndex];
                        children[parentIndex] = children[i];
                        children[i] = tempEle;

                       
                    }
                }
            }
           
                foreach (UIElement element in children)
                {
                    double startAngle = angle;
                    double endAngle = angle + angularSpace - 0.5;
                    double _endangle = angle + angularSpace + 1;

                    if (element is OuterRimItem)
                    {
                        OuterRimItem rimItem = element as OuterRimItem;
                        double x = radiusx + Math.Cos(DegToRad(endAngle))*radiusx;
                        double y = radiusy + Math.Sin(DegToRad(endAngle))*radiusy;

                        double _x = radiusx + Math.Cos(DegToRad(_endangle))*radiusx;
                        double _y = radiusy + Math.Sin(DegToRad(_endangle))*radiusy;

                        double startX = radiusx + Math.Cos(DegToRad(startAngle))*radiusx;
                        double startY = radiusy + Math.Sin(DegToRad(startAngle))*radiusy;

                        if (rimItem.PART_Arrow != null)
                        {
                            double arrowX = (radiusx + Math.Cos(DegToRad(arrowangle))*radiusx) -
                                            (rimItem.PART_Arrow.Width/2);
                            double arrowY = (radiusy + Math.Sin(DegToRad(arrowangle))*radiusy) -
                                            (rimItem.PART_Arrow.Height/2);
                            rimItem.ArrowPoint = new Point(arrowX, arrowY);
                        }

                        rimItem.StartPoint = new Point(startX, startY);
                        rimItem.RimSize = new Size(radiusx, radiusy);
                        rimItem.RimPoint = new Point(x, y);
                        rimItem.Point = new Point(_x, _y);
                        rimItem.Angle = angle + (angularSpace/2);
                    }
                    element.Arrange(new Rect(0, 0, element.DesiredSize.Width, element.DesiredSize.Height));

                    angle += angularSpace;
                    arrowangle += angularSpace;
                    if (angle > 359)
                        angle = 0;
                    if (arrowangle > 359)
                        arrowangle = 0;
                }
           
            return finalSize;
        }

        #endregion
        
    }
}
