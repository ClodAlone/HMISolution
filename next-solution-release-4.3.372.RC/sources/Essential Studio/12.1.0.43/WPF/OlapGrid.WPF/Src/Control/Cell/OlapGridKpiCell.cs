#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT
namespace Syncfusion.Windows.Grid.Olap
#else
namespace Syncfusion.Silverlight.Grid.Olap
#endif
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;    
    using System.ComponentModel;
#if SILVERLIGHT
    using Syncfusion.OlapSilverlight.Engine;
#else
    using Syncfusion.Olap.Engine;
#endif

    /// <summary>
    /// OlapGrid Cell
    /// </summary>
    /// <remarks>
    /// This cell displays the KPI images, based on image type specified in the CellDescriptor
    /// </remarks>
    /// 
#if SyncfusionFramework4_0
    [DesignTimeVisible(false)]
#endif
#if !SILVERLIGHT
    public class OlapGridKpiCell : Label
#else
    public class OlapGridKpiCell : System.Windows.Controls.Control
#endif
    {
        #region Dependency Property declaration

        public static readonly DependencyProperty CellDescriptorProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("CellDescriptor", typeof(PivotCellDescriptor), typeof(OlapGridKpiCell), new UIPropertyMetadata(null));
#else
            DependencyProperty.Register("CellDescriptor", typeof(PivotCellDescriptor), typeof(OlapGridKpiCell), new PropertyMetadata(null));
#endif

#if SILVERLIGHT
        public static readonly DependencyProperty ImageSourceProperty =
         DependencyProperty.Register("ImageSource", typeof(string), typeof(OlapGridKpiCell), new PropertyMetadata(""));
#endif


        #endregion

        #region Initilize/Finalize

#if !SILVERLIGHT
        /// <summary>
        /// Initializes the <see cref="OlapGridKpiCell"/> class.
        /// </summary>
        static OlapGridKpiCell()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OlapGridKpiCell), new FrameworkPropertyMetadata(typeof(OlapGridKpiCell)));
        }
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="OlapGridKpiCell"/> class.
        /// </summary>
        public OlapGridKpiCell()
        {
            DefaultStyleKey = typeof(OlapGridKpiCell);
        }
#endif
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the cell descriptor.
        /// </summary>
        /// <value>The cell descriptor.</value>
        public PivotCellDescriptor CellDescriptor
        {
            get { return (PivotCellDescriptor)GetValue(CellDescriptorProperty); }
            set { SetValue(CellDescriptorProperty, value); }
        }

#if SILVERLIGHT
        /// <summary>
        /// Kpi cell images uri
        /// </summary>
        public string ImageSource
        {
            get { return (string)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }
#endif
        #endregion
    }
}
