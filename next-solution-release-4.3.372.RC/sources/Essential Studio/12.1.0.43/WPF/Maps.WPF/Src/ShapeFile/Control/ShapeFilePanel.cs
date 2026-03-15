#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Map
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    ///  ShapeFilePanel is a Panel that arranges the Shapes in the Map
    /// </summary>
    public class ShapeFilePanel : Panel
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.Map.ShapeFilePanel"/> class.
        /// </summary>
        public ShapeFilePanel()
        {
           
        }
        internal bool zoomCall=false;
        internal int callCount = 0;

        /// <summary>
        /// Gets or sets a value indicating whether this instance Suspend or not.
        /// </summary>
        /// <value>
        /// 	<see langword="true"/> if this instance ; otherwise, <see langword="false"/>.
        /// </value>
        public bool IsInSuspend
        {
            get;
            internal set;
        }

#if WPF
        /// <summary>
        /// Starts the initialization process for this element.
        /// </summary>
        public override void BeginInit()
#else
        /// <summary>
        /// Starts the initialization process for this element.
        /// </summary>
        public void BeginInit()
#endif
        {
#if WPF
            base.BeginInit();
#endif
            if (!this.IsInSuspend)
            {
                this.IsInSuspend = true;
            }
        }

#if WPF
        /// <summary>
        /// Indicates that the initialization process for the element is complete.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException"><see cref="M:System.Windows.FrameworkElement.EndInit"/> was called without <see cref="M:System.Windows.FrameworkElement.BeginInit"/> having previously been
        /// called on the element.</exception>
        public override void EndInit()
#else
            /// <summary>
        /// Indicates that the initialization process for the element is complete.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException"><see cref="M:System.Windows.FrameworkElement.EndInit"/> was called without <see cref="M:System.Windows.FrameworkElement.BeginInit"/> having previously been
        /// called on the element.</exception>
        public void EndInit()
#endif
        {
#if WPF
            base.EndInit();
#endif
            if (this.IsInSuspend)
            {
                this.IsInSuspend = false;
#if WPF
                this.InvalidateVisual();
#else
                this.InvalidateMeasure();
                this.InvalidateArrange();
#endif
            }
        }

        /// <summary>
        /// When overridden in a derived class, measures the size in layout required for
        /// child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement"/>-derived class.
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its
        /// calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(System.Windows.Size constraint)
        {           
            if (this.IsInSuspend)
            {
                return new Size();
            }
             Size availableSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
             for (int j = 0; j < this.Children.Count; j++)
                {
                    if (this.Children[j] != null)
                    {
                        this.Children[j].Measure(availableSize);                        
                    }
                }           
                     
            return new Size();
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a
        /// size for a <see cref="T:System.Windows.FrameworkElement"/> derived class.
        /// </summary>
        /// <param name="arrangeSize">Size</param>
        /// <returns>
        /// The actual size used.
        /// </returns>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            if (this.IsInSuspend)
            {
                return arrangeSize;
            }                       

                foreach (UIElement element in this.Children)
                {
                    if (element == null)
                    {
                        continue;
                    }

                    double x = 0.0;
                    double y = 0.0;
                    element.Arrange(new Rect(new Point(x, y), element.DesiredSize));
                }
         
            return arrangeSize;
        }
      
    }
}
