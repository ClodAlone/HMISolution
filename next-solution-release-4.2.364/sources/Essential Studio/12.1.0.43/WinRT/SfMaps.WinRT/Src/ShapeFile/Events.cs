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
    using System.Windows;
#if WINRT
    using Windows.UI.Xaml;
#endif
    #region ZoomEvent Handler

    /// <summary>
    /// Represent a delegate to route the ZoomEvents in the map.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">An <see cref="T:Syncfusion.UI.Xaml.Maps.ZoomEventArgs">ZoomEventArgs</see> that contains the event data.</param>
    /// <remarks></remarks>
    //[ClassReference(IsReviewed = false)]
    public delegate void ZoomEventHandler(object sender, ZoomEventArgs args);

    /// <summary>
    /// Represents the ZoomEventArgs in map.Inherites from the RoutedEventArgs
    /// </summary>
    //[ClassReference(IsReviewed = false)]
    public class ZoomEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.UI.Xaml.Maps.ZoomEventArgs">ZoomEventArgs</see> class. 
        /// </summary>
        /// <param name="latitude">Latitude when Zooming</param>
        /// <param name="longitude">Longitude when zooming</param>
        /// <param name="zoomLevel">Current Zoom level</param>
        //[ClassReference(IsReviewed = false)]
        public ZoomEventArgs(double latitude, double longitude, Int32 zoomLevel)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.ZoomLevel = zoomLevel;
        }

        /// <summary>
        /// Gets the Latitude of the ZoomEventArgs.
        /// </summary>
        /// <value>
        /// Type :<see cref="double"/>
        /// </value>
        /// <remarks>
        /// Latitude is the read only property passed when zoom the map.
        /// </remarks>
        //[ClassReference(IsReviewed = false)]
        public double Longitude
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the Longitude of the ZoomEventArgs.
        /// </summary>
        /// <value>
        /// Type :<see cref="double"/>
        /// </value>
        /// <remarks>
        /// Longitude is the read only property passed when zoom the map.
        /// </remarks>
        //[ClassReference(IsReviewed = false)]
        public double Latitude
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the ZoomLevel of the ZoomEventArgs.
        /// </summary>
        /// <value>
        /// Type :<see cref="double"/>
        /// </value>
        /// <remarks>
        /// ZoomLevel is the read only property passed when zoom the map.
        /// </remarks>
        //[ClassReference(IsReviewed = false)]
        public Int32 ZoomLevel
        {
            get;
            private set;
        }

    }
    #endregion

    #region PanEventHadler

    /// <summary>
    /// Represent a delegate to route the Pan Events in the map.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">An <see cref="T:Syncfusion.UI.Xaml.Maps.PanEventArgs">PanEventArgs</see> that contains the event data.</param>
    //[ClassReference(IsReviewed = false)]
    public delegate void PanEventHandler(object sender, PanEventArgs args);

    /// <summary>
    /// Represent the PanEventArgs in the SfMap.Inherited from the RoutedEventArgs. 
    /// </summary>
    public class PanEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.UI.Xaml.Maps.PanEventArgs">PanEventArgs</see> class. 
        /// </summary>
        /// <param name="latitude">Latitude when Zooming</param>
        /// <param name="longitude">Longitude when zooming</param>
        /// <param name="panmode">Current ZoomLevel</param>       
        //[ClassReference(IsReviewed = false)]
        public PanEventArgs(double latitude, double longitude, PanMode panmode)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.PanningDirection = panmode;
        }

        /// <summary>
        /// Gets the Longitude of the PanEventArgs.
        /// </summary>
        /// <value>
        /// Type :<see cref="double"/>
        /// </value>
        /// <remarks>
        /// Longitude is the read only property passed when pan the map.
        /// </remarks>
        //[ClassReference(IsReviewed = false)]
        public double Longitude
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the Latitude of the PanEventArgs.
        /// </summary>
        /// <value>
        /// Type :<see cref="double"/>
        /// </value>
        /// <remarks>
        /// Latitude is the read only property passed when pan the map.
        /// </remarks>
        //[ClassReference(IsReviewed = false)]
        public double Latitude
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the PanningDirection of the ZoomEventArgs.
        /// </summary>
        /// <value>
        /// Type :<see cref="PanMode"/>
        /// </value>
        /// <remarks>
        /// Panning Direction is the read only property passed when pan the map.
        /// </remarks>
        //[ClassReference(IsReviewed = false)]
        public PanMode PanningDirection
        {
            get;
            private set;
        }

    }
    #endregion

    #region SelectionEvents

    /// <summary>
    /// Represent a delegate to route the Selection Events in the map.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">An <see cref="T:Syncfusion.UI.Xaml.Maps.SelectionEventArgs">SelectionEventArgs</see> that contains the event data.</param>
    //[ClassReference(IsReviewed = false)]
    public delegate void SelectionEventHandler(object sender, SelectionEventArgs args);

    /// <summary>
    /// Represents the SelectionEventArgs in the SfMap.
    /// </summary>
    //[ClassReference(IsReviewed = false)]
    public class SelectionEventArgs : RoutedEventArgs
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.UI.Xaml.Maps.SelectionEventArgs">SelectionEventArgs</see> class. 
        /// </summary>
        /// <param name="items">Items to be selected or unselected</param>
        //[ClassReference(IsReviewed = false)]
        public SelectionEventArgs(object items)
        {
            this.Items = items;
        }

        #endregion

        #region Properties

        #region Items

        /// <summary>
        /// Gets the Items of the SelectionEvent.
        /// </summary>
        /// <value>
        /// Type :<see cref="object"/>
        /// </value>
        /// <remarks>
        /// Items is the read only property to read the items when selection and un selection events occurred.
        /// </remarks>
        //[ClassReference(IsReviewed = false)]
        public object Items
        {
            get;
            internal set;
        }


        #endregion

        #endregion
    }


    #endregion

}
