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
using System.Windows;
using System.Windows.Media;
#endif

namespace Syncfusion.UI.Xaml.Maps
{
    public class ColorMapping : DependencyObject
    {
        #region Color
               
        //[ClassReference(IsReviewed = false)]
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Color.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(ColorMapping), new PropertyMetadata(Colors.White));

        public virtual bool Validate(object value)
        {
            return false;
        }
        
        #endregion
    }   


    /// <summary>
    /// Represents the RangeColorMapping class in the map.Inherited from the <see cref="DependencyObject"/>
    /// </summary>
    /// <remarks>
    /// RangeColorMapping class contains the members to provide the TreeMap like support for Bubbles and MapShapes.
    /// </remarks>
    /// <example>
    /// <para>Refer the following code to know how to define the range color mapping</para>
    /// <code language="C#">
    /// using Syncfusion.UI.Xaml.Maps;
    /// using System;
    /// using System.Collections.Generic;
    /// using System.IO;
    /// using System.Linq;
    /// using Windows.Foundation;
    /// using Windows.Foundation.Collections;
    /// using Windows.UI;
    /// using Windows.UI.Xaml;
    /// using Windows.UI.Xaml.Controls;
    /// using Windows.UI.Xaml.Controls.Primitives;
    /// using Windows.UI.Xaml.Data;
    /// using Windows.UI.Xaml.Input;
    /// using Windows.UI.Xaml.Media;
    /// using Windows.UI.Xaml.Navigation;
    /// 
    /// 
    /// namespace MapApp
    /// {
    ///     public sealed partial class MainPage : Page
    ///     {
    ///         public MainPage()
    ///         {
    ///             this.InitializeComponent();
    ///             SfMap syncMap = new SfMap();
    ///             ViewModel viewModel = new ViewModel();
    ///             ShapeFileLayer layer = new ShapeFileLayer();
    ///             layer.Uri = "MapApp.world1.shp";           
    ///             BubbleMarkerSetting bubbleSetting = new BubbleMarkerSetting();
    ///             bubbleSetting.AutoFillColor = false;
    ///             bubbleSetting.ColorMappings.Add(new RangeColorMapping { Range=0, Color=Colors.White });
    ///             bubbleSetting.ColorMappings.Add(new RangeColorMapping { Range = 50, Color = Colors.Thistle });
    ///             bubbleSetting.ColorMappings.Add(new RangeColorMapping { Range = 100, Color = Colors.Violet });
    ///             bubbleSetting.ValuePath = "AverageHighTemperature";
    ///             bubbleSetting.Stroke = new SolidColorBrush(Colors.Black);
    ///             bubbleSetting.StrokeThickness = 5;
    ///             bubbleSetting.MaxSize = 500;
    ///             bubbleSetting.MinSize = 100;
    ///             layer.BubbleMarkerSetting = bubbleSetting;
    ///             syncMap.Layers.Add(layer);
    ///            
    /// 
    ///         }       
    ///        
    ///     }    
    /// }
    /// </code>
    /// </example>    
    //[ClassReference(IsReviewed = false)]
    public class RangeColorMapping : ColorMapping
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.UI.Xaml.Maps.RangeColorMapping">RangeColorMapping</see> class. 
        /// </summary>
        //[ClassReference(IsReviewed = false)]
        public RangeColorMapping()
        {
        }

        #region Properties

        /// <summary>
        /// Gets or sets the range for the RangeColorMapping.
        /// </summary>
        /// <value>
        /// Type:<see cref="double"/>
        /// </value>
        /// <remarks>
        /// Use Range property to set the range in the RangeColorMapping when providing Tree map like support for bubble or map shape.
        /// </remarks>
        /// <example>
        /// <code language="C#">
        /// using Syncfusion.UI.Xaml.Maps;
        /// using System;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Linq;
        /// using Windows.Foundation;
        /// using Windows.Foundation.Collections;
        /// using Windows.UI;
        /// using Windows.UI.Xaml;
        /// using Windows.UI.Xaml.Controls;
        /// using Windows.UI.Xaml.Controls.Primitives;
        /// using Windows.UI.Xaml.Data;
        /// using Windows.UI.Xaml.Input;
        /// using Windows.UI.Xaml.Media;
        /// using Windows.UI.Xaml.Navigation;
        /// 
        /// 
        /// namespace MapApp
        /// {
        ///     public sealed partial class MainPage : Page
        ///     {
        ///         public MainPage()
        ///         {
        ///             this.InitializeComponent();
        ///             RangeColorMapping mapping = new RangeColorMapping();
        ///             mapping.Range = 50;
        ///             mapping.Color = Colors.Orange;
        /// 
        ///         }       
        ///        
        ///     }
        ///   
        /// }
        /// 
        /// </code>
        /// </example>
        //[ClassReference(IsReviewed = false)]
        public double Range
        {
            get { return (double)GetValue(RangeProperty); }
            set { SetValue(RangeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Range.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RangeProperty =
            DependencyProperty.Register("Range", typeof(double), typeof(RangeColorMapping), new PropertyMetadata(0d));

        #endregion

        public override bool Validate(object value)
        {
            return base.Validate(value);
        }
    }

    public class EqualsColorMapping : ColorMapping
    {
        public EqualsColorMapping()
        {
        }

        #region Properties

        //[ClassReference(IsReviewed = false)]
        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Range.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(EqualsColorMapping), new PropertyMetadata(null));
        
        #endregion

        public override bool Validate(object value)
        {
            if (value == null && this.Value == null)
            {
                return true;
            }
            else if(value != null && value.Equals(this.Value))
            {
                return true;
            }

            return false;
        }
    }
}
