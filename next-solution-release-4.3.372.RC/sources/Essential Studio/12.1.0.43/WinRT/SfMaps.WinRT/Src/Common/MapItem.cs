#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
#if WINRT
using Windows.UI.Xaml;
using Windows.Foundation;
#else
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Linq;
using System.Text;
#endif

namespace Syncfusion.UI.Xaml.Maps
{
    /// <summary>
    /// Used to represent the MapItem in the SfMap.
    /// </summary>
    /// <remarks>
    /// This class is used to represent the MapItems which are generated from the ItemsSource of the <see cref="ShapeFileLayer"/>.Collection of this objects of this class will be added internally in MapItems of <see cref="ShapeFileLayer"/>
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public class MapItem : DependencyObject
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.UI.Xaml.Maps.MapItem">MapItem</see> class. 
        /// </summary>
        /// <remarks>
        ///  Create the new instance for the MapItem.
        /// </remarks>
        [ClassReference(IsReviewed = false)]
        public MapItem()
        {
            DBFData = new Dictionary<string, string>();
        } 

        #endregion

        #region Internal Fields

        internal int index;
        internal Point mapItemPoint;

        #endregion

        #region Properties

        #region Settting
        /// <summary>
        /// Gets the Customization Setting for the MapItem.
        /// </summary>
        /// <value>
        /// Type :<see cref="MapItemSetting"/>
        /// </value>
        /// <remarks>
        /// This property is read only property. Value for this property will be internally set from MapItemSetting of <see cref="ShapeFileLayer"/>.
        /// Setting will be applied on map items when default template is applied on the map item.
        /// </remarks>
        [ClassReference(IsReviewed = false)]
        public MapItemSetting Setting
        {
            get { return (MapItemSetting)GetValue(SettingProperty); }
            internal set { SetValue(SettingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Setting.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SettingProperty =
            DependencyProperty.Register("Setting", typeof(MapItemSetting), typeof(MapItem), new PropertyMetadata(null));
        #endregion

        #region MapItemValue
        /// <summary>
        /// Gets the display value on the MapItems.
        /// </summary>
        /// <value>
        /// Type :<see cref="Object"/>
        /// </value>
        /// <remarks>
        /// MapItemValue is the read only property to get the display value of the MapItem. Value for this property will be set internally.
        /// </remarks>
        [ClassReference(IsReviewed = false)]
        public object MapItemValue
        {
            get { return GetValue(MapItemValueProperty); }
            internal set { SetValue(MapItemValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SymbolValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MapItemValueProperty =
            DependencyProperty.Register("MapItemValue", typeof(object), typeof(MapItem), new PropertyMetadata(null));
        #endregion

        #region Margin
        /// <summary>
        /// Gets the margin for the MapItem in the map.
        /// </summary>
        /// <value>
        /// Type :<see cref="Thickness"/>
        /// </value>
        /// <remarks>
        /// Margin property is used to arrange the MapItems on the map.
        /// </remarks>
        [ClassReference(IsReviewed = false)]
        public Thickness Margin
        {
            get { return (Thickness)GetValue(MarginProperty); }
            internal set { SetValue(MarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SymbolMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarginProperty =
            DependencyProperty.Register("Margin", typeof(Thickness), typeof(MapItem), new PropertyMetadata(new Thickness(0, 0, 0, 0)));
        #endregion

        #region Template
        /// <summary>
        /// Gets the Template for the MapItem.
        /// </summary>
        /// <value>
        /// Type :<see cref="DataTemplate"/>
        /// </value>
        /// <remarks>
        /// Template is the read only property to get the template for MapItem. Value for this property is set internally from "ItemsTemplate" of the <see cref="ShapeFileLayer"/>
        /// </remarks>
        [ClassReference(IsReviewed = false)]
        public DataTemplate Template
        {
            get { return (DataTemplate)GetValue(TemplateProperty); }
            set { SetValue(TemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SymbolTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TemplateProperty =
            DependencyProperty.Register("Template", typeof(DataTemplate), typeof(MapItem), new PropertyMetadata(null));
        #endregion

        #region Data
        /// <summary>
        /// Gets the Under bound object value for the MapItem
        /// </summary>
        /// <value>
        /// Type :<see cref="Object"/>
        /// </value>
        /// <remarks>
        /// Data is the read only property to get the under bound object for the MapItem. Value for this property will be set internally.
        /// </remarks>
        [ClassReference(IsReviewed = false)]
        public object Data
        {
            get { return GetValue(DataProperty); }
            internal set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DataItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(MapItem), new PropertyMetadata(null));
        #endregion

        #region DBFData
        public Dictionary<string, string> DBFData
        {
            get { return (Dictionary<string, string>)GetValue(DBFDataProperty); }
            internal set { SetValue(DBFDataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DBFData.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DBFDataProperty =
            DependencyProperty.Register("DBFData", typeof(Dictionary<string, string>), typeof(MapItem), new PropertyMetadata(null)); 
        #endregion
        
        #endregion
    }
}
