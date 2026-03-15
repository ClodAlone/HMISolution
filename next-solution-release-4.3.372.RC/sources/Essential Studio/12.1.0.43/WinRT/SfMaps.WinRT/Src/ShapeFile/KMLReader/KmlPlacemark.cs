#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
#if WINRT
using System;
using System.Collections.ObjectModel;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls; 
#else
using System.Windows;
#endif

namespace Syncfusion.UI.Xaml.Maps
{
    #region KmlPlacemark

    internal class KmlPlacemark : DependencyObject
    {
        #region Constructor

        internal KmlPlacemark()
        {
            Polygons = new List<KmlPolygon>();
            Points = new List<KmlPoint>();
            BalloonStyle = new BalloonStyle();
        } 

        #endregion

        #region Internal Fields

        internal bool hasStyleMapStyle; 

        #endregion

        #region Dependency Properties

        #region Name
        internal object Name
        {
            get { return (object)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Name.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(object), typeof(KmlPlacemark), new PropertyMetadata(null));
        #endregion

        #region Description
        internal object Description
        {
            get { return (object)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(object), typeof(KmlPlacemark), new PropertyMetadata(null));
        #endregion

        #region NormalStyle
        internal KmlStyle NormalStyle
        {
            get { return (KmlStyle)GetValue(NormalStyleProperty); }
            set { SetValue(NormalStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NormalStyle.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty NormalStyleProperty =
            DependencyProperty.Register("NormalStyle", typeof(KmlStyle), typeof(KmlPlacemark), new PropertyMetadata(null, OnStyleChanged));

        private static void OnStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is KmlStyle)
            {
                (e.NewValue as KmlStyle).SetAttributes();
            }
        }
        #endregion

        #region HighlightStyle
        internal KmlStyle HighlightStyle
        {
            get { return (KmlStyle)GetValue(HighlightStyleProperty); }
            set { SetValue(HighlightStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HighlightStyle.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty HighlightStyleProperty =
            DependencyProperty.Register("HighlightStyle", typeof(KmlStyle), typeof(KmlPlacemark), new PropertyMetadata(null, OnStyleChanged));
        #endregion

        #region Polygons
        internal List<KmlPolygon> Polygons
        {
            get { return (List<KmlPolygon>)GetValue(PolygonsProperty); }
            set { SetValue(PolygonsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Polygons.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty PolygonsProperty =
            DependencyProperty.Register("Polygons", typeof(List<KmlPolygon>), typeof(KmlPlacemark), new PropertyMetadata(null));
        #endregion

        #region Points
        internal List<KmlPoint> Points
        {
            get { return (List<KmlPoint>)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Points.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register("Points", typeof(List<KmlPoint>), typeof(KmlPlacemark), new PropertyMetadata(null));
        #endregion

        #region ExtendedData
        internal Dictionary<string, KmlData> ExtendedData
        {
            get { return (Dictionary<string, KmlData>)GetValue(ExtendedDataProperty); }
            set { SetValue(ExtendedDataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExtendedData.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ExtendedDataProperty =
            DependencyProperty.Register("ExtendedData", typeof(Dictionary<string, KmlData>), typeof(KmlPlacemark), new PropertyMetadata(null));
        #endregion

        #region BalloonVisibility
        internal Visibility BalloonVisibility
        {
            get { return (Visibility)GetValue(BalloonVisibilityProperty); }
            set { SetValue(BalloonVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BalloonVisibility.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty BalloonVisibilityProperty =
            DependencyProperty.Register("BalloonVisibility", typeof(Visibility), typeof(KmlPlacemark), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region BalloonStyle
        internal BalloonStyle BalloonStyle
        {
            get { return (BalloonStyle)GetValue(BalloonStyleProperty); }
            set { SetValue(BalloonStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BalloonStyle.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty BalloonStyleProperty =
            DependencyProperty.Register("BalloonStyle", typeof(BalloonStyle), typeof(KmlPlacemark), new PropertyMetadata(null));
        #endregion 

        #endregion

        #region Implementation

        internal void SetBalloonStyle()
        {
            if (hasStyleMapStyle && HighlightStyle != null)
            {
                BalloonStyle = HighlightStyle.BalloonStyle != null ? HighlightStyle.GetBalloonStyle() :
                    (Name != null || Description != null ? new BalloonStyle() : null);
            }
            else if (NormalStyle != null)
            {
                BalloonStyle = NormalStyle.BalloonStyle != null ? NormalStyle.GetBalloonStyle() :
                     (Name != null || Description != null ? new BalloonStyle() : null);
            }
        } 

        #endregion
    } 

    #endregion

    #region KmlData

    internal struct KmlData
    {
        internal string DisplayName;
        internal object Value;
    } 

    #endregion

}
