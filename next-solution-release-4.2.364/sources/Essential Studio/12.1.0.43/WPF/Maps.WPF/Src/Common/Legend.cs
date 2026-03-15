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
    /// Legend is a key to symbolism used on a map usually containing swatches of symbols with descriptions.
    /// </summary>
    public class Legend : ItemsControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.Map.Legend">Legend</see> class. 
        /// </summary>
        /// <remarks></remarks>
        public Legend()
        {
            this.DefaultStyleKey = typeof(Legend);
        }

        #region Properties

        #region Legend Text

        /// <summary>
        /// Gets or sets Text value of legend.
        /// </summary>
        /// <value>
        /// string
        /// </value>
        public string LegendText
        {
            get { return (string)GetValue(LegendTextProperty); }
            set { SetValue(LegendTextProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for LegendText.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LegendTextProperty =
            DependencyProperty.Register("LegendText", typeof(string), typeof(Legend), new PropertyMetadata(""));

        #endregion

        #region Legend Symbol

        /// <summary>
        /// Gets or sets Symbol of the legend.
        /// </summary>
        /// <value>
        /// DependencyObject
        /// </value>
        public DependencyObject  LegendSymbol
        {
            get { return (DependencyObject )GetValue(LegendSymbolProperty); }
            set { SetValue(LegendSymbolProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for LegendSymbol.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LegendSymbolProperty =
            DependencyProperty.Register("LegendSymbol", typeof(DependencyObject ), typeof(Legend), new PropertyMetadata(null));

        #endregion

      

        #endregion

        #region HelperMethods

        private void ChangePosition()
        {

        }

        #endregion
    }
}
