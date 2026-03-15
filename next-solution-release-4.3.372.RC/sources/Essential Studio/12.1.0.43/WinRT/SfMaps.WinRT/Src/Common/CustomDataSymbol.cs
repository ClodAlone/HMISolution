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
using Windows.UI.Xaml;
using System.Threading.Tasks;
using Windows.Foundation;
#else
using System.Windows;
#endif


namespace Syncfusion.UI.Xaml.Maps
{

    /// <summary>
    /// Represent the CustomDataSymbol class in the SfMap. It is inherited from <see cref="DependencyObject"/>
    /// </summary>
    /// <remarks>
    /// CustomDataSymbol is used to present the custom symbols in the map. Custom Symbols are generated from CustomDataSource of <see cref="ShapeFileLayer"/>
    /// <para>This class was internally used for creating custom data symbols.</para>
    /// <para>CustomDataSymbols are internally generated and added into the CustomDataSymbols of <see cref="ShapeFileLayer"/></para>
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public class CustomDataSymbol : DependencyObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.UI.Xaml.Maps.CustomDataSymbol">CustomDataSymbol</see> class. 
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public CustomDataSymbol()
        {
        }
        #region Properties

        internal Point midpoint= new Point();

        #region Latitude


        /// <summary>
        /// Gets the Latitude values for CustomDataSymbol.
        /// </summary>
        /// <remarks>
        /// CustomDataSymbols are arranged based on  Latitude and Longitude values. This property used to set the Latitude of the CustomDataSymbol.
        /// </remarks>
        /// <value>
        /// Type :<see cref="double"/>
        /// </value>
        [ClassReference(IsReviewed = false)]
        public double Latitude
        {
            get { return (double)GetValue(LatitudeProperty); }
            internal set { SetValue(LatitudeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Latitude.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LatitudeProperty =
            DependencyProperty.Register("Latitude", typeof(double), typeof(CustomDataSymbol), new PropertyMetadata(0d));


        /// <summary>
        /// Gets  Longitude values for CustomDataSymbol.
        /// </summary>
        /// <remarks>
        /// CustomDataSymbols are arranged based on  Longitude and Longitude values. This property used to set the Longitude of the CustomDataSymbol.
        /// </remarks>
        /// <value>
        /// Type :<see cref="double"/>
        /// </value>
        [ClassReference(IsReviewed = false)]
        public double Longitude
        {
            get { return (double)GetValue(LongitudeProperty); }
            internal set { SetValue(LongitudeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Longitude.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LongitudeProperty =
            DependencyProperty.Register("Longitude", typeof(double), typeof(CustomDataSymbol), new PropertyMetadata(0d));







        #endregion


        /// <summary>
        /// Gets the Template for the custom data symbols of the shape file layer.
        /// </summary>
        /// <value>
        /// Type :<see cref="DataTemplate"/>
        /// </value>
        /// <remarks>
        /// CustomDataSymbolTemplate is read only property  and  used to internally set the template for custom data symbols.
        /// </remarks>
        [ClassReference(IsReviewed = false)]
        public DataTemplate CustomDataSymbolTemplate
        {
            get { return (DataTemplate)GetValue(CustomDataSymbolTemplateProperty); }
            internal set { SetValue(CustomDataSymbolTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CustomDataSymbolTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CustomDataSymbolTemplateProperty =
            DependencyProperty.Register("CustomDataSymbolTemplate", typeof(DataTemplate), typeof(CustomDataSymbol), new PropertyMetadata(null));


        #region Margin



        /// <summary>
        /// Gets the margin for custom data symbol.
        /// </summary>
        /// <value>
        /// Type :<see cref="Thickness"/>
        /// </value>
        /// <remarks>
        /// This is read only property. This property is used to internally set the margin for the CustomDataSymbols.
        /// </remarks>
        [ClassReference(IsReviewed = false)]
        public Thickness Margin
        {
            get { return (Thickness)GetValue(MarginProperty); }
            internal set { SetValue(MarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Margin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarginProperty =
            DependencyProperty.Register("Margin", typeof(Thickness), typeof(CustomDataSymbol), new PropertyMetadata(new Thickness(0, 0, 0, 0)));



        /// <summary>
        /// Gets the DataContext for the CustomDataSymbol .
        /// </summary>
        /// <value>
        /// Type :<see cref="Object"/>
        /// </value>
        /// <remarks>
        /// This is the read only property to internally set the under bound object value for the CustomDataSymbol.
        /// </remarks>
        [ClassReference(IsReviewed = false)]
        public Object Data
        {
            get { return (Object)GetValue(DataProperty); }
            internal set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DataContext.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(Object), typeof(CustomDataSymbol), new PropertyMetadata(null));




        #endregion



        #endregion

    }
}
