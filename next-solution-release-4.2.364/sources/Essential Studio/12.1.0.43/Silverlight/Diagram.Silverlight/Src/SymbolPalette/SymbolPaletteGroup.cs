#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Diagram
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.ComponentModel;

    /// <summary>
    /// A control having a set of ColorGroupItems
    /// </summary>
#if !SyncfusionFramework3_5
    [DesignTimeVisible(false)]
#endif
    public class SymbolPaletteGroup : ItemsControl
    {       
        /// <summary>
        /// Identifies the DataSource dependency property
        /// </summary>
        public static readonly DependencyProperty DataSourceProperty = DependencyProperty.Register("DataSource", typeof(ObservableCollection<SymbolPaletteItem>), typeof(SymbolPaletteGroup), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the HeaderVisibility dependency property
        /// </summary>
        public static readonly DependencyProperty HeaderVisibilityProperty = DependencyProperty.Register("HeaderVisibility", typeof(Visibility), typeof(SymbolPaletteGroup), new PropertyMetadata(Visibility.Visible));
        
        /// <summary>
        /// Identifies the PanelVisibility dependency property
        /// </summary>
        public static readonly DependencyProperty PanelVisibilityproperty = DependencyProperty.Register("PanelVisibility", typeof(Visibility), typeof(SymbolPaletteGroup), new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Identifies the <see cref="SymbolPaletteGroupBackgroundProperty"/>  dependency property.
        /// </summary>
        internal static readonly DependencyProperty SymbolPaletteGroupBackgroundProperty = DependencyProperty.Register("SymbolPaletteGroupBackground", typeof(Brush), typeof(SymbolPaletteGroup), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 139, 69, 19))));

        /// <summary>
        /// Identifies the <see cref="SymbolPaletteGroupForegroundProperty"/>  dependency property.
        /// </summary>
        internal static readonly DependencyProperty SymbolPaletteGroupForegroundProperty = DependencyProperty.Register("SymbolPaletteGroupForeground", typeof(Brush), typeof(SymbolPaletteGroup), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 50, 21, 110))));



        internal Thickness SymbolPaletteGroupBorderThickness
        {
            get { return (Thickness)GetValue(SymbolPaletteGroupBorderThicknessProperty); }
            set { SetValue(SymbolPaletteGroupBorderThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SymbolPaletteGroupBorderThickness.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty SymbolPaletteGroupBorderThicknessProperty =
            DependencyProperty.Register("SymbolPaletteGroupBorderThickness", typeof(Thickness), typeof(SymbolPaletteGroup ), new PropertyMetadata(new Thickness(0,0,0,1)));

        

        static SymbolPaletteGroup()
        {
#if WPF
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SymbolPaletteGroup), new FrameworkPropertyMetadata(typeof(SymbolPaletteGroup)));
#endif
        }


        /// <summary>
        /// Initializes a new instance of the SymbolPaletteGroup class
        /// </summary>
        public SymbolPaletteGroup()
        {
            if (_parent == null)
            {
                _parent = FindPArent(this);
            }
            this.Loaded += new RoutedEventHandler(SymbolPaletteGroup_Loaded);
#if SILVERLIGHT
            DefaultStyleKey = typeof(SymbolPaletteGroup);
#endif
        }

        internal DependencyObject _parent;
        internal SymbolPalette FindPArent(DependencyObject symbolitem)
        {
            DependencyObject symbolgroup = (DependencyObject)VisualTreeHelper.GetParent(symbolitem);
            if (symbolgroup == null)
            {
                return null;
            }
            if (symbolgroup is SymbolPalette)
            {
                return symbolgroup as SymbolPalette;
            }
            else
            {
                return FindPArent(symbolgroup);
            }

        }
        void SymbolPaletteGroup_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

#if SILVERLIGHT
        /// <summary>
        /// Gets or sets the value of the HeaderName dependency property
        /// </summary>
        public string HeaderName
        {
            get
            {
                return (string)GetValue(HeadNameProperty);
            }

            set
            {
                SetValue(HeadNameProperty, value);
            }
        }

        /// <summary>
        /// Identifies the HeaderName dependency property
        /// </summary>
        public static readonly DependencyProperty HeadNameProperty = DependencyProperty.Register("HeaderName", typeof(string), typeof(SymbolPaletteGroup), new PropertyMetadata("Theme Colors", new PropertyChangedCallback(IsHeaderChanged)));
#endif
#if WPF
        internal string HeaderName { get { return Label; } set { SetValue(SymbolPaletteGroup.LabelProperty, value); } }
        

        /// <summary>
        /// Gets or sets the value of the HeaderName dependency property
        /// </summary>
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

        /// <summary>
        /// Identifies the HeaderName dependency property
        /// </summary>
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(SymbolPaletteGroup), new PropertyMetadata("Theme Colors", new PropertyChangedCallback(IsHeaderChanged)));
#endif
        /// <summary>
        /// Gets or sets the value of the HeaderVisibility dependency property
        /// </summary>
        public Visibility HeaderVisibility
        {
            get
            {
                return (Visibility)GetValue(HeaderVisibilityProperty);
            }

            set
            {
                SetValue(HeaderVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the PanelVisibility dependency property.
        /// </summary>        
        /// <value>
        /// Type : Visibility
        /// </value>
        public Visibility PanelVisibility
        {
            get
            {
                return (Visibility)GetValue(PanelVisibilityproperty);
            }

            set
            {
                SetValue(PanelVisibilityproperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the ThemeBackGround dependency property.
        /// </summary>
        internal Brush SymbolPaletteGroupBackground
        {
            get
            {
                return (Brush)GetValue(SymbolPaletteGroupBackgroundProperty);
            }

            set
            {
                SetValue(SymbolPaletteGroupBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the ThemeForeGround dependency property.
        /// </summary>
        internal Brush SymbolPaletteGroupForeground
        {
            get
            {
                return (Brush)GetValue(SymbolPaletteGroupForegroundProperty);
            }

            set
            {
                SetValue(SymbolPaletteGroupForegroundProperty, value);
            }
        }

        /// <summary>
        /// Method called when IsHeaderChanged event is raised
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value</param>
        protected virtual void IsHeaderChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Event raised when HeaderName is changed
        /// </summary>
        /// <param name="o">SymbolPaletteGroup object where the change occures on</param>
        /// <param name="e">Property change details, such as old value and new value</param>
        private static void IsHeaderChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SymbolPaletteGroup g = (SymbolPaletteGroup)o;
            g.IsHeaderChanged(e);
        }

        /// <summary>
        /// Applies the Template for the control
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
    }
}
