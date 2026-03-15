#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Tools.Controls
{
    using System;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    /// Represents a BrushSelector Control, used for picking the Brush in solid or
    /// gradient mode.
    /// </summary>
    /// <example>
    /// The control can be added to the application in the following ways. 
    /// <para></para>
    /// <para></para>
    /// <list type="table">
    /// <listheader>
    /// <term>Xaml</term></listheader>
    /// <item>
    /// <description>&lt;syncfusion:BrushSelector Height=&quot;150&quot;
    /// Width=&quot;300&quot;  Name=&quot;brushselector'
    /// VisualizationStyle=&quot;HSV&quot; SelectedBrush=&quot;BlueViolet&quot; /&gt; 
    /// <para>                                </para></description></item></list>
    /// <list type="table">
    /// <listheader>
    /// <term>C#</term></listheader>
    /// <item>
    /// <description>BrushSelector brushselector=new BrushSelector; 
    /// <para>brushselector.VisualizationStyle=ColorSelectionMode.HSV;</para></description></item></list>
    /// </example>
    /// 

    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
     Type = typeof(BrushSelector), XamlResource = "/Syncfusion.Theming.Blend;component/BrushSelector.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
        Type = typeof(BrushSelector), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/BrushSelector.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
        Type = typeof(BrushSelector), XamlResource = "/Syncfusion.Theming.Office2007Black;component/BrushSelector.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
        Type = typeof(BrushSelector), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/BrushSelector.xaml")]
[Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
        Type = typeof(BrushSelector), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/BrushSelector.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
        Type = typeof(BrushSelector), XamlResource = "/Syncfusion.Theming.Office2007Black;component/BrushSelector.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
        Type = typeof(BrushSelector), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/BrushSelector.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
        Type = typeof(BrushSelector), XamlResource = "/Syncfusion.Theming.Default;component/BrushSelector.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Windows7,
      Type = typeof(BrushSelector), XamlResource = "/Syncfusion.Theming.Default;component/BrushSelector.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2003,
        Type = typeof(BrushSelector), XamlResource = "/Syncfusion.Theming.Office2003;component/BrushSelector.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
       Type = typeof(BrushSelector), XamlResource = "/Syncfusion.Theming.Metro;component/BrushSelector.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
    Type = typeof(BrushSelector), XamlResource = "/Syncfusion.Theming.VS2010;component/BrushSelector.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Transparent ,
Type = typeof(BrushSelector), XamlResource = "/Syncfusion.Theming.Transparent;component/BrushSelector.xaml")]

    public class BrushSelector : Control, IDisposable
    {
        #region Variables
        private ToggleButton tbn;
        private TextBlock text;
        private BrushEdit cedit;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Tools.Controls.BrushSelector">BrushSelector</see> class
        /// </summary>
        public BrushSelector()
        {
            this.DefaultStyleKey = typeof(BrushSelector);            
        }

        /// <summary>
        /// Initializes the <see cref="BrushSelector"/> class.
        /// </summary>
        static BrushSelector()
        {
            if (System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
                Syncfusion.Windows.Shared.LoadDependentAssemblies load = new Syncfusion.Windows.Shared.LoadDependentAssemblies();
                load = null;
            }

        }

        #endregion

        #region Dependency Properties
        /// <summary>
        /// BrushMode dependency property.The BrushMode can be Gradient or Solid mode
        /// </summary>
        /// <returns>
        /// Type: <see
        /// cref="T:Syncfusion.Windows.Tools.Controls.BrushModes">BrushModes</see>
        /// </returns>
        public static readonly DependencyProperty BrushModeProperty =
         DependencyProperty.Register("BrushMode", typeof(BrushModes), typeof(BrushSelector), new PropertyMetadata(BrushModes.Gradient, new PropertyChangedCallback(OnBrushModeChanged)));
       
        /// <summary>
        /// EnableGradientToSolidSwitch dependency property.It enables the swtich to toggle
        /// between Solid and Gradient.
        /// </summary>
        /// <returns>
        /// Type:<see cref="T:System.Boolean">bool </see>
        /// </returns>
        public static readonly DependencyProperty EnableGradientToSolidSwitchProperty =
        DependencyProperty.Register("EnableGradientToSolidSwitch", typeof(bool), typeof(BrushSelector), new PropertyMetadata(true, new PropertyChangedCallback(OnEnableGradientToSolidSwitchChanged)));

        /// <summary>
        /// SelectedBrush dependency property.The SelectedBrush can be set as Gradient or
        /// Solid.
        /// </summary>
        /// <returns>
        /// Type:<see cref="T:System.Windows.Media.Brush">Brush </see>
        /// </returns>
        public static readonly DependencyProperty SelectedBrushProperty =
           DependencyProperty.Register("SelectedBrush", typeof(Brush), typeof(BrushSelector), new PropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnSelectedBrushChanged)));

        /// <summary>
        /// VisualizationStyle dependency property.The VisualizationStyle can be HSV or RGB
        /// mode
        /// </summary>
        /// <returns>
        /// Type:<see
        /// cref="T:Syncfusion.Windows.Tools.Controls.ColorSelectionMode">ColorSelectionMode</see>
        /// </returns>
        public static readonly DependencyProperty VisualizationStyleProperty =
           DependencyProperty.Register("VisualizationStyle", typeof(ColorSelectionMode), typeof(BrushSelector), new PropertyMetadata(ColorSelectionMode.HSV, new PropertyChangedCallback(OnVisualizationStyleChanged)));
        
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the value of the BrushMode dependency property.
        /// </summary>
        /// <value>
        /// Type: <see
        /// cref="T:Syncfusion.Windows.Tools.Controls.BrushModes">BrushModes</see>
        /// </value>
        public BrushModes BrushMode
        {
            get
            {
                return (BrushModes)GetValue(BrushModeProperty);
            }

            set
            {
                SetValue(BrushModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether EnableGradientToSolidSwitch dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="T:System.Boolean">bool </see>
        /// </value>
        public bool EnableGradientToSolidSwitch
        {
            get
            {
                return (bool)GetValue(EnableGradientToSolidSwitchProperty);
            }

            set
            {
                SetValue(EnableGradientToSolidSwitchProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the Selected Brush dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="T:System.Windows.Media.Brush">Brush </see>
        /// </value>
        public Brush SelectedBrush
        {
            get
            {
                return (Brush)GetValue(SelectedBrushProperty);
            }

            set
            {
                SetValue(SelectedBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the Visualization dependency property.
        /// </summary>
        /// <value>
        /// Type: <see
        /// cref="T:Syncfusion.Windows.Tools.Controls.ColorSelectionMode">ColorSelectionMode</see>
        /// </value>
        public ColorSelectionMode VisualizationStyle
        {
            get
            {
                return (ColorSelectionMode)GetValue(VisualizationStyleProperty);
            }

            set
            {
                SetValue(VisualizationStyleProperty, value);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Applies the Template for the control
        /// </summary>
        public override void OnApplyTemplate()
        {
            this.LayoutUpdated -= new EventHandler(BrushSelector_LayoutUpdated);
           // Application.Current.RootVisual.MouseLeftButtonDown -= new MouseButtonEventHandler(RootVisual_MouseLeftButtonDown);
            if (tbn != null)
            {
                tbn.Checked -= new RoutedEventHandler(tbn_Checked);
            }
            base.OnApplyTemplate();
            cedit = this.GetTemplateChild("BrushEdit") as BrushEdit;           
            text = this.GetTemplateChild("colortext") as TextBlock;
            tbn = this.GetTemplateChild("colorToggleButton") as ToggleButton;
            if (this.BrushMode == BrushModes.Gradient)
            {
                text.Text = "Gradient Brush";
            }
            else
            {
                text.Text = "Solid Color";
            }

            this.LayoutUpdated += new EventHandler(BrushSelector_LayoutUpdated);
            tbn.Checked += new RoutedEventHandler(tbn_Checked);
            this.Loaded += new RoutedEventHandler(BrushSelector_Loaded);
            this.Unloaded += new RoutedEventHandler(BrushSelector_Unloaded);
        }

        void BrushSelector_Loaded(object sender, RoutedEventArgs e)
        {
            Application.Current.RootVisual.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(RootVisual_MouseLeftButtonDown), true);
        }

        void BrushSelector_Unloaded(object sender, RoutedEventArgs e)
        {
            Dispose();
        }

        /// <summary>
        /// Handles the Checked event of the tbn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void tbn_Checked(object sender, RoutedEventArgs e)
        {
            if (cedit != null)
            {
                cedit.Width = d;
            }
        }
        double d;
        /// <summary>
        /// Handles the LayoutUpdated event of the BrushSelector control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void BrushSelector_LayoutUpdated(object sender, EventArgs e)
        {
            if(cedit!=null )
            {
               // cedit.Height = this.ActualHeight;
              d= this.ActualWidth;
              //cedit.Width = d;
            }
        }     

        /// <summary>
        /// Calls OnBrushModeChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnBrushModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BrushSelector instance = (BrushSelector)d;
            instance.OnBrushModeChanged(e);
        }

        /// <summary>
        /// Calls OnBrushModeChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private void OnBrushModeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (BrushModeChanged != null)
            {
                BrushModeChanged(this, e);
            }

            if (cedit != null)
            {
                cedit.BrushMode = (BrushModes)e.NewValue;
                if (this.BrushMode == BrushModes.Gradient)
                {
                    text.Text = "Gradient Brush";
                }
                else
                {
                    text.Text = "Solid Color";
                }
            }
        }

        /// <summary>
        /// Calls OnEnableGradientToSolidSwitchChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private void OnEnableGradientToSolidSwitchChanged(DependencyPropertyChangedEventArgs e)
        {
            if (EnableGradientToSolidSwitchChanged != null)
            {
                EnableGradientToSolidSwitchChanged(this, e);
            }

            if (cedit != null)
            {
                cedit.EnableGradientToSolidSwitch = (bool)e.NewValue;
            }
        }
        
        /// <summary>
        /// Calls OnEnableGradientToSolidSwitchChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnEnableGradientToSolidSwitchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BrushSelector instance = (BrushSelector)d;
            instance.OnEnableGradientToSolidSwitchChanged(e);
        }

        /// <summary>
        /// Calls OnSelectedBrushChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSelectedBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BrushSelector instance = (BrushSelector)d;
            instance.OnSelectedBrushChanged(e);
        }

        /// <summary>
        /// Calls OnSelectedBrushChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary> 
        private void OnSelectedBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SelectedBrushChanged != null)
            {
                SelectedBrushChanged(this, e);
            }

            if (cedit != null)
            {
                ResourceDictionary rs = new ResourceDictionary { Source = new Uri(@"/Syncfusion.Shared.Silverlight;component/Themes/generic.xaml", UriKind.RelativeOrAbsolute) };
                cedit.SelectedBrush = (Brush)e.NewValue;
                if (e.NewValue == null)
                {
                    ((Rectangle)this.GetTemplateChild("selectedColorRect")).Fill = rs["NullBrush"] as ImageBrush;
                }

                else 
                {
                    if (e.NewValue is SolidColorBrush)
                    {
                        Color selectedcolor = (Color)cedit.SelectedBrush.GetValue(SolidColorBrush.ColorProperty);
                        string a = selectedcolor.A.ToString();
                        if (a == "0")
                        {
                            

                            ImageBrush brush1 = rs["TransparentBrush"] as ImageBrush;
                            ((Rectangle)this.GetTemplateChild("selectedColorRect")).Fill = brush1;

                          


                        }
                        else
                        {
                            ((Rectangle)this.GetTemplateChild("selectedColorRect")).Fill = (Brush)e.NewValue;
                        }
                    }
                    else
                    {
                        ((Rectangle)this.GetTemplateChild("selectedColorRect")).Fill = (Brush)e.NewValue;
                    }
                 

                  

                }

               
                   
            }            
        }

        /// <summary>
        /// Calls OnVisualizationStyleChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private void OnVisualizationStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (VisualizationStyleChanged != null)
            {
                VisualizationStyleChanged(this, e);
            }

            if (cedit != null)
            {
                cedit.VisualizationStyle = (ColorSelectionMode)e.NewValue;
            }
        }

        /// <summary>
        /// Calls OnVisualizationStyleChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>        
        private static void OnVisualizationStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BrushSelector instance = (BrushSelector)d;
            instance.OnVisualizationStyleChanged(e);
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the RootVisual control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void RootVisual_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            tbn.IsChecked = false;
        }  
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when SelectedBrush property is changed.
        /// </summary>
        public event PropertyChangedCallback SelectedBrushChanged;

        /// <summary>
        /// Event that is raised when Brushmode property is changed.
        /// </summary>
        public event PropertyChangedCallback BrushModeChanged;

        /// <summary>
        /// Event that is raised when EnableGradientToSolidSwitch property is changed.
        /// </summary>
        public event PropertyChangedCallback EnableGradientToSolidSwitchChanged;

        /// <summary>
        /// Event that is raised when VisualizationStyle property is changed.
        /// </summary>
        public event PropertyChangedCallback VisualizationStyleChanged;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Application.Current.RootVisual.RemoveHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(RootVisual_MouseLeftButtonDown));
        }
    }
}
