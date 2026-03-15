// <copyright file="Clipper.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if !(Silverlight4 || WINDOWS_PHONE_7)
using System.Threading.Tasks;
#endif
#if WINDOWS_PHONE || WINDOWS_PHONE_7
using System.Windows.Controls;
using System.Windows;

namespace Syncfusion.WP.Primitives  
#else
#if WPF
using System.Windows;
using System.Windows.Controls;
namespace Syncfusion.Windows.Primitives
#else
#if SILVERLIGHT
using System.Windows;
using System.Windows.Controls;
namespace Syncfusion.Tools.Primitives
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Syncfusion.UI.Xaml.Primitives  
#endif
#endif
#endif
{
    /// <summary>
    /// Clips a ratio of its content.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [ClassReference(IsReviewed = false, ShouldInclude = false)]
    public abstract class Clipper : ContentControl
    {
        #region Dependency Properties
        /// <summary>
        /// Gets or sets the percentage of the item visible.
        /// </summary>
        public double RatioVisible
        {
            get { return (double)GetValue(RatioVisibleProperty); }
            set { SetValue(RatioVisibleProperty, value); }
        }

        /// <summary>
        /// Identifies the RatioVisible dependency property.
        /// </summary>
        public static readonly DependencyProperty RatioVisibleProperty =
            DependencyProperty.Register(
                "RatioVisible",
                typeof(double),
                typeof(Clipper),
                new PropertyMetadata(1.0, OnRatioVisibleChanged));
        

        #endregion 

        #region Callback Methods

        /// <summary>
        /// RatioVisibleProperty property changed handler.
        /// </summary>
        /// <param name="d">PartialView that changed its RatioVisible.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnRatioVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clipper source = (Clipper)d;
            double oldValue = (double)e.OldValue;
            double newValue = (double)e.NewValue;
            source.OnRatioVisibleChanged(oldValue, newValue);
        }

        /// <summary>
        /// RatioVisibleProperty property changed handler.
        /// </summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>        
        protected virtual void OnRatioVisibleChanged(double oldValue, double newValue)
        {
            if (newValue >= 0.0 && newValue <= 1.0)
            {
                ClipContent();
            }
            else
            {
                if (newValue < 0.0)
                {
                    this.RatioVisible = 0.0;
                }
                else if (newValue > 1.0)
                {
                    this.RatioVisible = 1.0;
                }
            }
        }

        #endregion

        #region Helper Methods
        /// <summary>
        /// Initializes a new instance of the Clipper class.
        /// </summary>
        protected Clipper()
        {
            this.SizeChanged += delegate { ClipContent(); };
        }

        /// <summary>
        /// Updates the clip geometry.
        /// </summary>
        protected abstract void ClipContent();

        #endregion

        
    }
}
