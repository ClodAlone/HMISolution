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
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Controls;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents ChartAxisLayoutPanel 
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class ChartAxisLayoutPanel:Panel
    {
        #region properties
        /// <summary>
        /// Get or Set AxisLayout property
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public ILayoutCalculator AxisLayout
        {
            get;
            set;
        }

        #endregion

        #region methods
        /// <summary>
        /// Provides the behavior for the Measure pass of Silverlight layout. Classes can override this method to define their own Measure pass behavior.
        /// </summary>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of the allocated sizes for child objects; or based on other considerations, such as a fixed container size.
        /// </returns>
       /// <param name="availableSize"></param>
        protected override Size MeasureOverride(Size availableSize)
        {
            availableSize = ChartLayoutUtils.CheckSize(availableSize);
            if (AxisLayout != null)
            {
                AxisLayout.Measure(availableSize);
            }
            return availableSize;
        }
        /// <summary>
        /// Provides the behavior for the Arrange pass of Silverlight layout. Classes can override this method to define their own Arrange pass behavior.
        /// </summary>
        /// <returns>
        /// The actual size that is used after the element is arranged in layout.
        /// </returns>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (AxisLayout != null)
            {
                AxisLayout.Arrange(finalSize);
            }
            return finalSize;
        }

        #endregion
    }
}
