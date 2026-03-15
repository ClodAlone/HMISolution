// <copyright file="SymbolPaletteFilter.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.ComponentModel;

namespace Syncfusion.Windows.Diagram
{
    /// <summary>
    /// Represents SymbolPaletteFilter class.
    /// </summary>
    /// <remarks>
    /// A Symbol Palette filter can be added to the <see cref="SymbolPalette"/> control, using the SymbolFilters property, so that only desired Symbol Palette groups get displayed. The SetFilterIndexes property is used to specify the index value of the filters for which the group is to be displayed.
    /// <para/>
    /// The filter names are specified integer values,  with the first filter index starting from 0. Based on the filter indexes specified for that particular group, the visibility of the group is controlled. So the group gets displayed only when any of the specified filter names are selected.
    /// </remarks>
    /// <example>
    /// C#:
    /// <code language="C#">
    /// using Syncfusion.Windows.Diagram;
    /// namespace WpfApplication1
    /// {
    /// public partial class Window1 : Window
    /// {
    /// public DiagramControl Control;
    /// public DiagramModel Model;
    /// public DiagramView View;
    /// public Window1 ()
    /// {
    /// InitializeComponent ();
    /// Control = new DiagramControl ();
    /// Model = new DiagramModel ();
    /// View = new DiagramView ();
    /// Control.View = View;
    /// Control.Model = Model;
    /// View.Bounds = new Thickness(0, 0, 1000, 1000);
    /// //SymbolPaletteFilter creates a filter for the palette groups.
    /// SymbolPaletteFilter sfilter = new SymbolPaletteFilter();
    /// sfilter.Label = "Custom";
    /// Control.SymbolPalette.SymbolFilters.Add(sfilter);
    /// //SymbolPaletteGroup creates a group and assigns a specific filter index.
    /// SymbolPaletteGroup s = new SymbolPaletteGroup();
    /// s.Label = "Custom";
    /// SymbolPalette.SetFilterIndexes(s, new Int32Collection(new int[] { 0, 5 }));
    /// Control.SymbolPalette.SymbolGroups.Add(s);
    /// }
    /// }
    /// }
    /// </code>
    /// </example>
#if !SyncfusionFramework3_5
    [DesignTimeVisible(false)]
#endif
    public class SymbolPaletteFilter : DependencyObject
    {
        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolPaletteFilter"/> class.
        /// </summary>
        public SymbolPaletteFilter()
        {
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the label of the filter.
        /// </summary>
        /// Type: <see cref="string"/>
        /// Text that names the SymbolPalette filter.
        /// <example>
        /// C#:
        /// <code language="C#">
        /// using Syncfusion.Windows.Diagram;
        /// namespace WpfApplication1
        /// {
        /// public partial class Window1 : Window
        /// {
        /// public DiagramControl Control;
        /// public DiagramModel Model;
        /// public DiagramView View;
        /// public Window1 ()
        /// {
        /// InitializeComponent ();
        /// Control = new DiagramControl ();
        /// Model = new DiagramModel ();
        /// View = new DiagramView ();
        /// Control.View = View;
        /// Control.Model = Model;
        /// View.Bounds = new Thickness(0, 0, 1000, 1000);
        /// //SymbolPaletteFilter creates a filter for the palette groups.
        /// SymbolPaletteFilter sfilter = new SymbolPaletteFilter();
        /// sfilter.Label = "Custom";
        /// Control.SymbolPalette.SymbolFilters.Add(sfilter);
        /// }
        /// }
        /// }
        /// </code>
        /// </example>
        public string Label
        {
            get
            {
                return (string)GetValue(LabelProperty);
            }

            set
            {
                SetValue(LabelProperty, value);
            }
        }

        internal int[] Indices
        {
            get
            {
                return (int[])GetValue(IndicesProperty);
            }

            set
            {
                SetValue(IndicesProperty, value);
            }
        }
        #endregion

        #region Dependency properties

        /// <summary>
        /// Defines label of the filter.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(SymbolPaletteFilter), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty IndicesProperty =
           DependencyProperty.Register("Indices", typeof(int[]), typeof(SymbolPaletteFilter), new PropertyMetadata(new int[]{0}));
        #endregion
    }
}
