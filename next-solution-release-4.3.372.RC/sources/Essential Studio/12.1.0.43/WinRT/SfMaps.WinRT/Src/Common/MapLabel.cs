#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.UI.Xaml.Maps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
#if !WINDOWSPHONE_8
    using Windows.UI.Xaml;
#else
    using System.Windows;
#endif

    public class MapLabel : DependencyObject
    {
        #region Constructor
        public MapLabel()
        {
        }
        #endregion

        #region Internal Fields


        internal int index;

        #endregion


        #region Properties

        #region Setting



        public LabelSetting Setting
        {
            get { return (LabelSetting)GetValue(SettingProperty); }
            internal set { SetValue(SettingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Setting.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SettingProperty =
            DependencyProperty.Register("Setting", typeof(LabelSetting), typeof(MapLabel), new PropertyMetadata(null));



        #endregion

        #region Label

        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(MapLabel), new PropertyMetadata(string.Empty));

        #endregion

        #region LabelTemplate



        public DataTemplate LabelTemplate
        {
            get { return (DataTemplate)GetValue(LabelTemplateProperty); }
            set { SetValue(LabelTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelTemplateProperty =
            DependencyProperty.Register("LabelTemplate", typeof(DataTemplate), typeof(MapLabel), new PropertyMetadata(null));




        #endregion

        #region LabelMargin



        public Thickness LabelMargin
        {
            get { return (Thickness)GetValue(LabelMarginProperty); }
            set { SetValue(LabelMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelMarginProperty =
            DependencyProperty.Register("LabelMargin", typeof(Thickness), typeof(MapLabel), new PropertyMetadata(new Thickness(0,0,0,0)));



        #endregion


        #endregion
    }

}
