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
    using System.Reflection;
    using System.Text;
    using System.Windows.Markup;
    using System.Xml;

    /// <summary>
    ///  SymbolPaletteItem is a Object that holds the Symbol's Properties
    /// </summary>
    public class SymbolPaletteItem : ContentControl
    {
        #region PrivateFields

        private ResourceDictionary mapResource = new ResourceDictionary();
        private MapControl mapControl;
        private SymbolPalette symp;
        private Border ItemBorder;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.Map.SymbolPaletteItem"/> class.
        /// </summary>
        public SymbolPaletteItem()
        {
            this.DefaultStyleKey = typeof(SymbolPaletteItem);
#if SILVERLIGHT
            mapResource.Source = new Uri("/Syncfusion.Maps.Silverlight;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);
#endif
#if WPF
            mapResource.Source = new Uri("/Syncfusion.Maps.Wpf;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);

#endif
            this.MouseLeftButtonDown += new MouseButtonEventHandler(SymbolPaletteItem_MouseLeftButtonDown);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(SymbolPaletteItem_MouseLeftButtonUp);
            this.MouseEnter += new MouseEventHandler(SymbolPaletteItem_MouseEnter);
            this.MouseLeave += new MouseEventHandler(SymbolPaletteItem_MouseLeave);
        }
        #endregion

        #region Event Handlers

        void SymbolPaletteItem_MouseLeave(object sender, MouseEventArgs e)
        {
            this.IsHovered = false;
        }

        void SymbolPaletteItem_MouseEnter(object sender, MouseEventArgs e)
        {
            this.IsHovered = true;
        }

        void SymbolPaletteItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.mapControl.isMouseUp = true;
            this.mapControl.isMouseDown = false;
        }

        void SymbolPaletteItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            foreach (SymbolPaletteItem item in this.mapControl.SymbolPalette.SymbolPaletteItems)
            {
                if (item.IsSelected)
                {
                    item.IsSelected = false;
                }
            }
            this.IsSelected = true;
            object obj;
            this.mapControl.isMouseDown = true;
            this.mapControl.isMouseUp = false;
#if SILVERLIGHT
               if (this.PaletteItem is Path)
               {

                   string pathXaml = "<Path xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" Data=\"" + (this.PaletteItem as Path).Data.ToString() + "\"/>";
                   obj = (Path)System.Windows.Markup.XamlReader.Load(pathXaml);
                   obj = this.mapControl.ClonePath(this.PaletteItem, obj) as Path;
                   (obj as Path).Stretch = Stretch.Fill;

               }
               else
               {
                   obj = (object)this.mapControl.Clone(this.PaletteItem, 0);
               }
#endif
#if WPF
            if (this.PaletteItem is Path)
            {
                obj = this.mapControl.Clone(this.PaletteItem, 0);
            }
            else
            {
                obj = this.mapControl.Clone(this.PaletteItem, 0);
            }

#endif

            this.mapControl.SelectedSymbolItem = obj;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets Item of SymbolPalette.
        /// </summary>
        public object PaletteItem
        {
            get { return (object)GetValue(PaletteItemProperty); }
            set { SetValue(PaletteItemProperty, value); }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for PaletteItem.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PaletteItemProperty =
            DependencyProperty.Register("PaletteItem", typeof(object), typeof(SymbolPaletteItem), new PropertyMetadata(new object()));

        #region IsSelected 
        

        internal bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(SymbolPaletteItem), new PropertyMetadata(false));



        #endregion

        #region IsHovered

        internal bool IsHovered
        {
            get { return (bool)GetValue(IsHoveredProperty); }
            set { SetValue(IsHoveredProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty IsHoveredProperty =
            DependencyProperty.Register("IsHovered", typeof(bool), typeof(SymbolPaletteItem), new PropertyMetadata(false));

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
            this.mapControl = MapControl.FindParent<MapControl>(this);
            symp = MapControl.FindParent<SymbolPalette>(this);
            this.ItemBorder = this.GetTemplateChild("PART_ItemBorder") as Border;
        }


        #endregion

    }
}
