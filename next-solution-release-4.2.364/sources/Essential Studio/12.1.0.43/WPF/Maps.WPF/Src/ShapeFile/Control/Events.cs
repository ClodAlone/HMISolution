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

    #region ZoomEvent Handler
    /// <summary>
    ///  ZoomEventHandler which invokes the Zooming method
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">An <see cref="T:Syncfusion.Windows.Controls.Map.ZoomEventArgs"/> that contains the event
    /// data.</param>
    public delegate void ZoomEventHandler(object sender, ZoomEventArgs args);

    /// <summary>
    ///  The Argument of Zooming Event
    /// </summary>
    public class ZoomEventArgs : RoutedEventArgs
    {
#if SILVERLIGHT
        bool m_handled = false;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.Map.ZoomEventArgs"/> class.
        /// </summary>
        /// <param name="latitude">Latitude</param>
        /// <param name="longitude">Longitude</param>
        /// <param name="zoomfactor"></param>
        public ZoomEventArgs(double latitude, double longitude, double zoomfactor)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.ZoomFactor = zoomfactor;
        }

        /// <summary>
        /// Gets or sets Value of Logtitude .
        /// </summary>
        public double Longitude
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets Value of Latitude.
        /// </summary>
        public double Latitude
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets ZoomFactor that denotes the which value to be zoomed.
        /// </summary>
        public double ZoomFactor
        {
            get;
            private set;
        }
#if SILVERLIGHT
        /// <summary>
        /// Gets or sets Value whether Event is handled or not
        /// </summary>
        public bool Handled
        {
            get
            {

                return m_handled;
            }
            set
            {
                m_handled = value;
            }
        }
#endif
    }
    #endregion

    #region PanEventHadler
    /// <summary>
    ///  PanEventHandler which invokes the Panning method
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">An <see cref="T:Syncfusion.Windows.Controls.Map.PanEventArgs"/> that contains the event
    /// data.</param>
    public delegate void PanEventHandler(object sender, PanEventArgs args);

    /// <summary>
    ///  The argument which contains Pan event
    /// </summary>
    public class PanEventArgs : RoutedEventArgs
    {
#if SILVERLIGHT
        private bool m_handled = false;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.Map.PanEventArgs"/> class.
        /// </summary>
        /// <param name="latitude">Latitude</param>
        /// <param name="longitude">Longitude</param>
        /// <param name="panmode">PanMode</param>
        public PanEventArgs(double latitude, double longitude, PanMode panmode)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.PanningDirection = panmode;
        }

        /// <summary>
        /// Gets or sets Value of Lontitude.
        /// </summary>
        public double Longitude
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets Value of Latitude.
        /// </summary>
        public double Latitude
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets Direction of Panning. Whether Left or Right Direction
        /// </summary>
        public PanMode PanningDirection
        {
            get;
            private set;
        }


#if SILVERLIGHT
        /// <summary>
        /// Gets or sets Value whether Event is handled or not
        /// </summary>

        public bool Handled
        {
            get
            {

                return m_handled;
            }
            set
            {
                m_handled = value;
            }
        }
#endif
    }
    #endregion

    #region SelectionEvents

    /// <summary>
    ///  SelectionEventHandler Invokes when Selection Operation is occured
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">An <see cref="T:Syncfusion.Windows.Controls.Map.SelectionEventArgs"/> that contains the
    /// event data.</param>
    public delegate void SelectionEventHandler(object sender, SelectionEventArgs args);

    /// <summary>
    ///  SelectionEventArgs that contains the detailed argument of selection event
    /// </summary>
    public class SelectionEventArgs : RoutedEventArgs
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.Map.SelectionEventArgs"/> class.
        /// </summary>
        /// <param name="item">SelectionEventArgs</param>
        public SelectionEventArgs(object item)
        {
            this.Item = item;
        }

        #endregion

        #region Properties

        #region Item

        /// <summary>
        /// Gets or sets Item which is selected in the Map. the selected item may anything
        /// </summary>
        public object Item
        {
            get;
            internal set;
        }


        #endregion

        #endregion

    }


    #endregion
#region ShapesLoadedEventHandler

    /// <summary>
    ///  ShapeLoadedEventHandler invokes when shapefilelayer is loaded
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">An <see cref="T:Syncfusion.Windows.Controls.Map.ShapesLoadedEventArgs"/> that contains
    /// the event data.</param>
    public delegate void ShapesLoadedEventHandler(object sender, ShapesLoadedEventArgs args);

    /// <summary>
    ///  This contains argument of ShapeLoadedEvent
    /// </summary>
    public class ShapesLoadedEventArgs : RoutedEventArgs
    {
        #region Constrctor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.Map.ShapesLoadedEventArgs"/> class.
        /// </summary>
        /// <param name="shapes">Shapes</param>
        public  ShapesLoadedEventArgs(object shapes)
        {
            this.Shapes = shapes;
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets Shapes of the Map.
        /// </summary>
        public object Shapes
        {
            get;
            internal set;
        }

        #endregion


    }

#endregion
}
