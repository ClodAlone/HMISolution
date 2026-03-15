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
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Collections.ObjectModel;
    using System.Windows.Media.Imaging;
    using System.Windows.Data;
    using System.Windows.Markup;
    using System.Xml;

    /// <summary>
    ///  Symbol Pallete ia set of Colors that is applied for the Map symbols
    /// </summary>
    public class SymbolPalette : Control
    {

        #region Private Fields

        internal SymbolPalettePanel symbolsPanel = new SymbolPalettePanel();
        private ResourceDictionary mapResource = new ResourceDictionary();
        private Grid symbolPaletteGrid;
        ScrollViewer SymbolPaletteScrollViewer = new ScrollViewer();
        private MapControl mapControl;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.Map.SymbolPalette"/> class.
        /// </summary>
        public SymbolPalette()
        {
            this.DefaultStyleKey = typeof(SymbolPalette);
            this.SymbolPaletteItems = new ObservableCollection<SymbolPaletteItem>();
            this.SymbolPaletteItems.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(SymbolPaletteItems_CollectionChanged);
                       
        }

        #endregion

        #region Properties

        #region SymbolPaletteItems

        /// <summary>
        /// Gets or sets Collection os Symbol Palette Items.
        /// </summary>
        public ObservableCollection<SymbolPaletteItem> SymbolPaletteItems
        {
            get { return (ObservableCollection<SymbolPaletteItem>)GetValue(SymbolPaletteItemsProperty); }
            set { SetValue(SymbolPaletteItemsProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for SymbolPaletteItems.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SymbolPaletteItemsProperty =
            DependencyProperty.Register("SymbolPaletteItems", typeof(ObservableCollection<SymbolPaletteItem>), typeof(SymbolPalette), new PropertyMetadata(null));

        #endregion

        #endregion

        #region Override Methods
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or
        /// internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

#if SILVERLIGHT
       mapResource.Source = new Uri("/Syncfusion.Maps.Silverlight;component/Themes/SymbolShapes.xaml", UriKind.RelativeOrAbsolute);
#endif
#if WPF
            mapResource.Source = new Uri("/Syncfusion.Maps.Wpf;component/Themes/SymbolShapes.xaml", UriKind.RelativeOrAbsolute);

#endif
            this.mapControl = MapControl.FindParent<MapControl>(this);
            symbolPaletteGrid = this.GetTemplateChild("PART_SymbolPaletteGrid") as Grid;
            this.SymbolPaletteScrollViewer.Content = symbolsPanel;
            symbolPaletteGrid.Children.Add(this.SymbolPaletteScrollViewer);
            Grid.SetRow(this.SymbolPaletteScrollViewer, 1);
            if (this.symbolsPanel != null)
            {
                Path path;

                foreach (object pathObj in mapResource.Keys)
                {
                    object obj;
                    if (mapResource[pathObj.ToString()] is Path)
                    {
                        path = mapResource[pathObj.ToString()] as Path;
                        if (path != null)
                        {
#if SILVERLIGHT
                          string pathXaml;
                        pathXaml = "<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" Data=\"" + path.Data + "\"/>";
                        obj = (Path)System.Windows.Markup.XamlReader.Load(pathXaml);
                        (obj as Path).Stretch = Stretch.Uniform;
#endif
#if WPF
                            obj = this.mapControl.Clone(path, 0) as Path;
#endif
                            (obj as Path).Fill = path.Fill;
                       
                               this.SymbolPaletteItems.Add(new SymbolPaletteItem { PaletteItem = obj });
                        }
                    }
                    else
                    {
                        obj = this.mapControl.Clone(mapResource[pathObj.ToString()], 0);
                        if (obj is Image)
                        {
                            (obj as Image).Stretch = Stretch.Fill;
                        }

                        this.SymbolPaletteItems.Add(new SymbolPaletteItem { PaletteItem = obj });
                    }
                }
            }

        }

        #endregion

        #region EventHanders


        void SymbolPaletteItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (object obj in e.OldItems)
                {
                    this.symbolsPanel.Children.Remove(obj as UIElement);
                }
            }
            if (e.NewItems != null)
            {
                foreach (object obj in e.NewItems)
                {
                    if ((this.symbolsPanel.Children.Contains(obj as UIElement)))
                    {
                        this.symbolsPanel.Children.Remove(obj as UIElement);
                    }
                    ((obj as SymbolPaletteItem).PaletteItem as FrameworkElement).Height = 25;
                    ((obj as SymbolPaletteItem).PaletteItem as FrameworkElement).Width = 25;
                    this.symbolsPanel.Children.Add(obj as UIElement);

                }
            }
        }
        #endregion

    }
}
