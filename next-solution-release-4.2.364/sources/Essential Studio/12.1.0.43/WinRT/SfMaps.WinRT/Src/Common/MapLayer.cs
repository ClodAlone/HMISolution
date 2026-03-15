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
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Threading.Tasks;
#else
using System.Windows;
using System.Windows.Controls;
#endif


namespace Syncfusion.UI.Xaml.Maps
{

    /// <summary>
    /// Represent the MapLayer in the SfMap. Inherited from the <see cref="Control"/> class
    /// </summary>
    /// <remarks>
    /// MapLayer is the Base class for all layer class in maps.
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public abstract class MapLayer : Control
    {

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.UI.Xaml.Maps.MapLayer">MapLayer</see> class. 
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public MapLayer()
        {

        }

        #endregion

    }
}
