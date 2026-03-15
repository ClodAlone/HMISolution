// <copyright file="RadialMenuItem.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
#if !(WINDOWS_PHONE_7 || Silverlight4)
using System.Threading.Tasks;
#endif
#if WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Syncfusion.WP.Utils;
using Syncfusion.WP.Controls.Navigation;
using Syncfusion.WP.Primitives;
using System.Windows.Data;

namespace Syncfusion.WP.Controls.Navigation
#elif SILVERLIGHT
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Syncfusion.Tools.Utils;
using Syncfusion.Tools.Primitives;
using System.Windows.Data;

namespace Syncfusion.Tools.Controls.Navigation
#elif WPF
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Syncfusion.Windows.Utils;
using Syncfusion.Windows.Primitives;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
namespace Syncfusion.Windows.Controls.Navigation
#else
using Windows.Foundation;
using Syncfusion.UI.Xaml.Utils;
using Syncfusion.UI.Xaml.Primitives;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using System.Windows.Input;
using Windows.UI.Xaml.Data;

namespace Syncfusion.UI.Xaml.Controls.Navigation
#endif
{
    /// <summary>
    /// <para>Represents a selectable RadialMenuItem inside a <see
    /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.RadialMenu"/> that are associated
    /// with the commands.</para>
    /// </summary>
    /// <remarks>
    /// RadialMenuItem is a <see
    /// cref="T:Syncfusion.UI.Xaml.Primitives.HeaderedItemsControl"/>.
    /// </remarks>
    /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Navigation.RadialMenu"/>
    [ClassReference(IsReviewed = false)]
#if WINDOWS_PHONE_7
    public class SfRadialMenuItem :Syncfusion.WP.Primitives.HeaderedItemsControl
#elif WPF
    public class SfRadialMenuItem : System.Windows.Controls.HeaderedItemsControl
#else
    public class SfRadialMenuItem :HeaderedItemsControl
#endif
    {

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.RadialMenuItem"/> class.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public SfRadialMenuItem()
        {
            DefaultStyleKey = typeof(SfRadialMenuItem);
#if WPF
            compositeTransform = new RotateTransform();
#else
            compositeTransform = new CompositeTransform();
#endif
            this.RenderTransform = compositeTransform;
#if WINRT || WINDOWS_PHONE||WINDOWS_PHONE_7||WPF
			this.Loaded -= RadialMenuItem_Loaded;
            this.Loaded+=RadialMenuItem_Loaded;
#endif
            Binding binding = new Binding();
            binding.Path = new PropertyPath("Visibility");
            binding.Source = this;
            binding.Mode = BindingMode.TwoWay;

            this.SetBinding(InternalVisibilityProperty, binding);
        }

        #endregion      
#if WINRT || WINDOWS_PHONE||WINDOWS_PHONE_7||WPF

        void RadialMenuItem_Loaded(object sender, RoutedEventArgs e)
        {
			IsEnabledChanged -= RadialMenuItem_IsEnabledChanged;
            this.LayoutUpdated -= SfRadialMenuItem_LayoutUpdated;
            IsEnabledChanged+=RadialMenuItem_IsEnabledChanged;
            this.LayoutUpdated+=SfRadialMenuItem_LayoutUpdated;
#if WINRT || WINDOWS_PHONE||WINDOWS_PHONE_7||WPF
            if (Command != null)
            {
                EventHandler CanExecuteChangedEventhandler = new EventHandler(CanExecuteChangedHandler);
                Command.CanExecuteChanged += CanExecuteChangedEventhandler;
                if (Command is DelegateCommand)
                    (Command as DelegateCommand).UpdateCanExecute();
            }
#endif
#if WINRT
            if (this.DataContext!=null && radialMenu!=null && radialMenu.menuitem!=null && this.DataContext == radialMenu.menuitem && this.radialMenuItem==null && radialMenu.DrillDownItem!=radialMenu)
            {
                if (radialMenu.DrillDownItem is SfRadialMenuItem)
                    this.radialMenuItem = radialMenu.DrillDownItem as SfRadialMenuItem;
                if (!radialMenu.navigateback)
                    radialMenu.DrillDownItem = this;
            }
            else if (this.HasItems)
            {
                foreach (var item in this.Items)
                {
                    if (item == radialMenu.menuitem && !radialMenu.navigateback)
                        radialMenu.DrillDownItem = this;
                }
            }
            if (radialMenu!=null && radialMenu.DrillDownItem is SfRadialMenuItem && this.radialMenuItem == null && (radialMenu.navigateback || radialMenu.menuitem == null))
                this.radialMenuItem = radialMenu.DrillDownItem as SfRadialMenuItem;    
#endif
            UpdateVisualState();
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
            UpdateIsChecked();
#endif
            if (radialMenu != null && radialMenu.DrillDownItem != null)
                this.parentItem = radialMenu.DrillDownItem;
            UpdateVisibility();
        }
        void SfRadialMenuItem_LayoutUpdated(object sender, object e)
        {
            if (radialMenu != null && contentPresenter != null)
            {
                double leftMargin = 0d, rightMargin = 0d, topMargin = 0d, bottomMargin = 0d;
                if (radialMenu.DrillDownItem is SfRadialMenu)
                {
                    var x = radialMenu.RadiusX / radialMenu.Items.Count;
                    var y = radialMenu.RadiusY / radialMenu.Items.Count;

                    if (radialMenu.Items.Count > 2)
                        leftMargin = rightMargin = topMargin = bottomMargin = x / 3.14;
                    else if (!IsRadialSlider(radialMenu))
                         topMargin = bottomMargin = radialMenu.RadiusX / 4;                  
                }
                else
                {
                    var radialMenuItem = radialMenu.DrillDownItem as SfRadialMenuItem;
                    var x = radialMenu.RadiusX / radialMenuItem.Items.Count;
                    var y = radialMenu.RadiusY / radialMenuItem.Items.Count;
                    if (!IsRadialSlider(radialMenuItem))
                    {
                        if (radialMenuItem.Items.Count > 2)
                            leftMargin = rightMargin = topMargin = bottomMargin = y / 3.14;
                        else
                            topMargin = bottomMargin = radialMenu.RadiusX / 4;
                    }
                }

                if (this.HorizontalContentAlignment ==HorizontalAlignment.Left)
                {
                    leftMargin = -10;
                }
                else if (this.HorizontalContentAlignment == HorizontalAlignment.Right)
                {
                    rightMargin = -10;
                }
                else if(this.HorizontalContentAlignment ==HorizontalAlignment.Stretch)
                {
                    leftMargin = rightMargin = 0;
                }
                else
                    leftMargin=rightMargin=-5;

                if (this.VerticalContentAlignment == VerticalAlignment.Top)
                {
                   topMargin = -10;
                }
                else if (this.VerticalContentAlignment ==VerticalAlignment.Bottom)
                {
                   bottomMargin =-10;
                }
                else if(this.VerticalContentAlignment == VerticalAlignment.Stretch)
                {
                    topMargin = bottomMargin = 0;
                }
                else
                    topMargin=bottomMargin=-5;

                bool IsSlider = false;
                if (radialMenu.DrillDownItem is SfRadialMenu)
                    IsSlider = IsRadialSlider(radialMenu);
                else
                    IsSlider = IsRadialSlider(radialMenu.DrillDownItem as SfRadialMenuItem);
                if (!IsSlider) 
                contentPresenter.Margin = new Thickness(leftMargin, topMargin, rightMargin, bottomMargin);
            }
        }
        private void RadialMenuItem_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            UpdateVisualState();
        }

#if WINRT || WINDOWS_PHONE||WINDOWS_PHONE_7||WPF
        private void CanExecuteChangedHandler(object sender, EventArgs e)
        {
            if (!Command.CanExecute(CommandParameter))
                IsEnabled = false;
            else
                IsEnabled = true;
        }
#endif
        private void UpdateVisualState()
        {
            if (!IsEnabled)
                VisualStateManager.GoToState(this, "Disabled", true);
            else
                VisualStateManager.GoToState(this, "Normal", true);
        }
#endif

#if WINRT
        /// <summary>
        /// Initial Template is applied
        /// </summary>
        protected override void OnApplyTemplate()
#else
        /// <summary>
        /// Initial Template is applied.
        /// </summary>
        public override void OnApplyTemplate()
#endif
        {
            contentPresenter = GetTemplateChild("PART_MenuItem") as ContentPresenter;
            base.OnApplyTemplate();
        }

        internal bool IsRadialSlider(object obj)
        {
            if (obj is SfRadialMenuItem)
            {
                SfRadialMenuItem radialMenuItem = obj as SfRadialMenuItem;
                foreach (var item in radialMenuItem.Items)
                {
                    if (item is SfRadialSlider)
                    {
                        (item as SfRadialSlider).Height = double.NaN;
                        (item as SfRadialSlider).Width = double.NaN;
                        return true;
                    }
                    else if (item is SfRadialColorItem)
                        return true;
                }
            }
            else if (obj is SfRadialMenu)
            {
                SfRadialMenu radialMenuItem = obj as SfRadialMenu;
                foreach (var item in radialMenuItem.Items)
                {
                    if (item is SfRadialSlider)
                    {
                        (item as SfRadialSlider).Height = double.NaN;
                        (item as SfRadialSlider).Width = double.NaN;
                        return true;
                    }
                    else if (item is SfRadialColorItem)
                        return true;
                }
            }
            return false;
        }

        #region Variables
#if WPF
        internal RotateTransform compositeTransform;
#else
        internal CompositeTransform compositeTransform;
#endif
        internal SfRadialMenu radialMenu;

        internal SfRadialMenuItem radialMenuItem;
      
        internal OuterRimItem checkableRimItem;

        private ContentPresenter contentPresenter;

        private ObservableCollection<object> children;

        private int itemIndex = -1;

        internal double previousAngle;

        private object parentItem = null;

        /// <summary>
        /// Gets a collection of children for the parent item.
        /// </summary>
        public ObservableCollection<object> Children
        {
            get
            {
                if (children == null)
                {
                    children = new ObservableCollection<object>();
#if WINDOWS_PHONE
                    foreach (var item in Children)
#else
                    foreach (var item in Items)
#endif
                    {
                        children.Add(item);
                    }
                }
                return children;
            }
        }

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Gets or sets the icon that appears in a RadialMenuItem
        /// </summary>
        /// <value>
        /// The default value is <see cref="N:Windows.UI.Xaml.Media.ImageSource"/>.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(SfRadialMenuItem), new PropertyMetadata(null));

#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)

        /// <summary>
        /// Gets or sets whether the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.RadialMenuItem"/> is checked.
        /// </summary>
        /// <remarks>
        /// Used to define the Item as checked.
        /// </remarks>
        /// <value>
        /// <c>true</c> if this instance is checked; otherwise, <c>false</c>.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Navigation.RadialMenuItem.IsCheckable"/>
        [ClassReference(IsReviewed = false)]
        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsChecked.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(SfRadialMenuItem), new PropertyMetadata(false, new PropertyChangedCallback(OnIsCheckedChanged)));
#endif
        /// <summary>
        /// Gets the command that will be executed when the command source is invoked.
        /// </summary>
        /// <remarks>
        /// Used to bound to a command, that could performs an action.
        /// </remarks>
        /// <value>
        /// The default value is <see cref="T:Syncfusion.UI.Xaml.Utils.DelegateCommand"/>.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Navigation.RadialMenuItem.CommandParameter"/>
        [ClassReference(IsReviewed = false)]
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(SfRadialMenuItem), new PropertyMetadata(null));



        /// <summary>
        /// Represents a user defined data value that can be passed to the <see
        /// cref="P:Syncfusion.UI.Xaml.Controls.Navigation.RadialMenuItem.Command"/> when it
        /// is executed.
        /// </summary>
        /// <remarks>
        /// Normally, the commandparameter is used to pass specific information to the
        /// command when it is executed. The type of the data is defined by the command.
        /// </remarks>
        /// <value>
        /// The default value is null.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(SfRadialMenuItem), new PropertyMetadata(null));

        /// <summary>
        /// Gets a value indicating whether <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.RadialMenuItem"/> has items.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has items; otherwise, <c>false</c>.
        /// </value>
        [ClassReference(IsReviewed = false)]
#if WPF
        public new bool HasItems
#else
        public bool HasItems
#endif
        {
            get { return (bool)GetValue(HasItemsProperty); }
            internal set { SetValue(HasItemsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HasItems.  This enables animation, styling, binding, etc...
        /// </summary>


#if WPF
        public new static readonly DependencyProperty HasItemsProperty =
#else 
        public static readonly DependencyProperty HasItemsProperty =
#endif
            DependencyProperty.Register("HasItems", typeof(bool), typeof(SfRadialMenuItem), new PropertyMetadata(false, new PropertyChangedCallback(OnHasItemsChanged)));

        /// <summary>
        /// Gets a string for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.RadialMenuItem"/> has a group name.
        /// </summary>
        public string GroupName
        {
            get { return (string)GetValue(GroupNameProperty); }
            set { SetValue(GroupNameProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for GroupName.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty GroupNameProperty =
            DependencyProperty.Register("GroupName", typeof(string), typeof(SfRadialMenuItem), new PropertyMetadata(string.Empty));

#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
        /// <summary>
        /// Sets the CheckMode for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.RadialMenuItem"/> control.
        /// </summary>
        /// <value>
        /// The default value is <seealso cref="P:Syncfusion.UI.Xaml.Controls.Navigation.CheckMode"/>
        /// </value>
        public CheckMode CheckMode
        {
            get { return (CheckMode)GetValue(CheckModeProperty); }
            set { SetValue(CheckModeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CheckMode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CheckModeProperty =
            DependencyProperty.Register("CheckMode", typeof(CheckMode), typeof(SfRadialMenuItem), new PropertyMetadata(CheckMode.CheckBox, new PropertyChangedCallback(OnCheckedModeChanged)));

#endif

        /// <summary>
        /// Gets a value indicating whether <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.RadialMenuItem"/> has closed on execution.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has items; otherwise, <c>false</c>.
        /// </value>
        public bool CloseOnExecute
        {
            get { return (bool)GetValue(CloseOnExecuteProperty); }
            set { SetValue(CloseOnExecuteProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CloseOnExecute.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CloseOnExecuteProperty =
            DependencyProperty.Register("CloseOnExecute", typeof(bool), typeof(SfRadialMenuItem), new PropertyMetadata(false));
        public Visibility InternalVisibility
        {
            get { return (Visibility)GetValue(InternalVisibilityProperty); }
            set { SetValue(InternalVisibilityProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ToolTip.  This enables animation, styling, binding, etc...
        /// </summary>
#if WPF
        public new static readonly DependencyProperty InternalVisibilityProperty =            
#else
        public static readonly DependencyProperty InternalVisibilityProperty =
#endif
        DependencyProperty.Register("InternalVisibility", typeof(Visibility), typeof(SfRadialMenuItem), new PropertyMetadata(Visibility.Visible, OnInternalVisibilityChanged));


#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
        /// <summary>
        /// Gets an object for tooltip for the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.RadialMenuItem"/>
        /// </summary>
#if WPF
        public new object ToolTip
#else
        public object ToolTip
#endif
        {
            get { return (object)GetValue(ToolTipProperty); }
            set { SetValue(ToolTipProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ToolTip.  This enables animation, styling, binding, etc...
        /// </summary>
#if WPF
        public new static readonly DependencyProperty ToolTipProperty =            
#else
        public static readonly DependencyProperty ToolTipProperty =     
#endif
            DependencyProperty.Register("ToolTip", typeof(object), typeof(SfRadialMenuItem), new PropertyMetadata(null));


    
        /// <summary>
        /// Sets the tooltip placement for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.RadialMenuItem"/>
        /// </summary>
        public ToolTipPlacement ToolTipPlacement
        {
            get { return (ToolTipPlacement)GetValue(ToolTipPlacementProperty); }
            set { SetValue(ToolTipPlacementProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ToolTipPlacement.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ToolTipPlacementProperty =
            DependencyProperty.Register("ToolTipPlacement", typeof(ToolTipPlacement), typeof(SfRadialMenuItem), new PropertyMetadata(ToolTipPlacement.None));

#endif

        #endregion

        #region Override Methods

#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
        /// <summary>
        /// Sets the focus to the control when pointer is entered.
        /// </summary>
        /// <param name="e"></param>
#if WPFSILVERLIGHT
            protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e) 
#else
            protected override void OnPointerEntered(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif

        {
#if WPF
            bool isSlider =false;
            Point point = e.GetPosition(this);
            if (point.X == 0 && point.Y == 0)
                isSlider = true;
            if (this.ToolTip != null && this.radialMenu != null && this.radialMenu.PART_ToolTipPopup != null && this.radialMenu.PART_ToolTipContent != null && this.ToolTipPlacement != Navigation.ToolTipPlacement.None&&!isSlider)
#else
            if (this.ToolTip != null && this.radialMenu != null && this.radialMenu.PART_ToolTipPopup != null && this.radialMenu.PART_ToolTipContent != null && this.ToolTipPlacement != Navigation.ToolTipPlacement.None)
#endif
            {
                this.radialMenu.PART_ToolTipPopup.IsOpen = true;
                FrameworkElement fElement = this.radialMenu.PART_ToolTipPopup.Child as FrameworkElement;
                this.radialMenu.PART_ToolTipContent.Content = this.ToolTip;

                  if (this.ToolTipPlacement == Navigation.ToolTipPlacement.Top)
                  {
#if WPFSILVERLIGHT
                      this.radialMenu.PART_ToolTipPopup.HorizontalAlignment = HorizontalAlignment.Center;
                      this.radialMenu.PART_ToolTipPopup.VerticalAlignment = VerticalAlignment.Top;
#else
                      this.radialMenu.PART_ToolTipPopup.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;
                      this.radialMenu.PART_ToolTipPopup.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top;
#endif  
                      this.radialMenu.PART_ToolTipPopup.UpdateLayout();
#if WPF
                      this.radialMenu.PART_ToolTipPopup.VerticalOffset = - 10;
                      this.radialMenu.PART_ToolTipPopup.HorizontalOffset = -((radialMenu.DesiredSize.Width / 2) - (fElement.ActualWidth / 2));
                      this.radialMenu.PART_ToolTipPopup.Placement = PlacementMode.Top;
                      this.radialMenu.PART_ToolTipPopup.PlacementTarget = this.radialMenu;
#else
                      this.radialMenu.PART_ToolTipPopup.VerticalOffset = -fElement.ActualHeight - 10;
                      this.radialMenu.PART_ToolTipPopup.HorizontalOffset = -(fElement.ActualWidth / 2);
#endif
                  }
                  if (this.ToolTipPlacement == Navigation.ToolTipPlacement.Bottom)
                  {
#if WPFSILVERLIGHT
                      this.radialMenu.PART_ToolTipPopup.HorizontalAlignment = HorizontalAlignment.Center;
                      this.radialMenu.PART_ToolTipPopup.VerticalAlignment = VerticalAlignment.Bottom;
#else
                      this.radialMenu.PART_ToolTipPopup.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;
                      this.radialMenu.PART_ToolTipPopup.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Bottom; 
#endif
                      this.radialMenu.PART_ToolTipPopup.UpdateLayout();
                      this.radialMenu.PART_ToolTipPopup.VerticalOffset =10;
#if WPF
                      this.radialMenu.PART_ToolTipPopup.HorizontalOffset = -((radialMenu.DesiredSize.Width / 2) - (fElement.ActualWidth / 2));
                      this.radialMenu.PART_ToolTipPopup.Placement = PlacementMode.Bottom;
                      this.radialMenu.PART_ToolTipPopup.PlacementTarget = this.radialMenu;
#else
                      this.radialMenu.PART_ToolTipPopup.HorizontalOffset = -(fElement.ActualWidth / 2);
#endif

                  }
                  if (this.ToolTipPlacement == Navigation.ToolTipPlacement.Right)
                  {
#if WPFSILVERLIGHT
                      this.radialMenu.PART_ToolTipPopup.HorizontalAlignment = HorizontalAlignment.Right;
                      this.radialMenu.PART_ToolTipPopup.VerticalAlignment = VerticalAlignment.Center;
#else
                      this.radialMenu.PART_ToolTipPopup.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Right;
                      this.radialMenu.PART_ToolTipPopup.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center;
#endif
                      this.radialMenu.PART_ToolTipPopup.UpdateLayout();
                      this.radialMenu.PART_ToolTipPopup.HorizontalOffset = 10;
#if WPF
                      this.radialMenu.PART_ToolTipPopup.VerticalOffset = (radialMenu.DesiredSize.Height / 2) - (fElement.ActualHeight / 2);
                      this.radialMenu.PART_ToolTipPopup.Placement = PlacementMode.Right;
                      this.radialMenu.PART_ToolTipPopup.PlacementTarget = this.radialMenu;
#else                      
                      this.radialMenu.PART_ToolTipPopup.VerticalOffset = -(fElement.ActualHeight / 2);
#endif
                  }
                  if (this.ToolTipPlacement == Navigation.ToolTipPlacement.Left)
                  {
#if WPFSILVERLIGHT
                      this.radialMenu.PART_ToolTipPopup.HorizontalAlignment = HorizontalAlignment.Left;
                      this.radialMenu.PART_ToolTipPopup.VerticalAlignment = VerticalAlignment.Center;
#else
                      this.radialMenu.PART_ToolTipPopup.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left;
                      this.radialMenu.PART_ToolTipPopup.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center;
#endif
                      this.radialMenu.PART_ToolTipPopup.UpdateLayout();
#if WPF
                      this.radialMenu.PART_ToolTipPopup.HorizontalOffset = - 10;
                      this.radialMenu.PART_ToolTipPopup.VerticalOffset = (radialMenu.DesiredSize.Height / 2) - (fElement.ActualHeight / 2);
                      this.radialMenu.PART_ToolTipPopup.Placement = PlacementMode.Left;
                      this.radialMenu.PART_ToolTipPopup.PlacementTarget = this.radialMenu;
#else
                      this.radialMenu.PART_ToolTipPopup.HorizontalOffset = -fElement.ActualWidth - 10;
                      this.radialMenu.PART_ToolTipPopup.VerticalOffset = -(fElement.ActualHeight / 2);
#endif
                  }
            }

            if (this.checkableRimItem != null && this.CheckMode != Navigation.CheckMode.None)
            {
                VisualStateManager.GoToState(checkableRimItem, "PointerOver", true);
            }
#if WPFSILVERLIGHT
            base.OnMouseEnter(e);
#else
            base.OnPointerEntered(e);
#endif

        }
#endif

#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
        /// <summary>
        /// Removes the focus when the pointer exits
        /// </summary>
        /// <param name="e"></param>
#if WPFSILVERLIGHT
        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
#else
        protected override void OnPointerExited(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {
            if (this.radialMenu != null && this.radialMenu.PART_ToolTipPopup != null  && this.ToolTipPlacement != Navigation.ToolTipPlacement.None)
            {
                this.radialMenu.PART_ToolTipPopup.IsOpen = false;
            }
            if (this.checkableRimItem != null && !this.checkableRimItem.IsChecked)
            {
                VisualStateManager.GoToState(checkableRimItem, "Normal", true);
            }
#if WPFSILVERLIGHT
            base.OnMouseLeave(e);
#else
            base.OnPointerExited(e);
#endif
        }
#endif
        /// <summary>
        /// Checks for any items.
        /// </summary>
        /// <param name="e"></param>
#if !WINRT
        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
#else
        protected override void OnItemsChanged(object e)
#endif
        {
            if (Items.Count > 0)
            {
                HasItems = true;
            }
            else
            {
                HasItems = false;
            }
            base.OnItemsChanged(e);
        }
                
#if WINDOWS_PHONE||WINDOWS_PHONE_7

        internal double Angle { get; set; }
        /// <summary>
        /// Sets the focus to the control when the mouse left button is down
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if(radialMenu != null)
                radialMenu.AnimateSelectionRim(0,0.5,Angle);
            base.OnMouseLeftButtonDown(e);
        }

#endif

        /// <summary>
        /// Removes the focus when the pointer is released and sets to the parent item.
        /// </summary>
        /// <param name="e"></param>
#if !WINRT
       protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
#else
        protected override void OnPointerReleased(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {

            if (!radialMenu.manipulationStarted)
            {
                if (Click != null)
                {
                    Click(this, e);
                }
                if (Command != null && Command.CanExecute(CommandParameter))
                {
                    Command.Execute(CommandParameter);
                }
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
                if (CheckMode != Navigation.CheckMode.None)
                {                  
                    IsChecked = !IsChecked;                   
                }
#endif
                if (CloseOnExecute && this.radialMenu != null && !this.HasItems)
                {
                    this.radialMenu.IsOpen = false;                   
                    this.radialMenu.DrillDownItem = this.radialMenu;
                }
            }
#if WINDOWS_PHONE||WINDOWS_PHONE_7

            if (this.Items.Count>0)
                radialMenu.AnimateIn(1, 0,0.2,0.2,System.Windows.Media.Animation.EasingMode.EaseIn, TimelineCompleted);
            if(radialMenu != null && radialMenu.PART_SelectionRim.Visibility == Visibility.Visible && CloseOnExecute)
                radialMenu.AnimateSelectionRim(0.5,0,Angle);
            base.OnMouseLeftButtonUp(e);
#elif WPFSILVERLIGHT
            base.OnMouseLeftButtonUp(e);
#else
            base.OnPointerReleased(e);
#endif
        }

#if WINDOWS_PHONE||WINDOWS_PHONE_7
       internal  void TimelineCompleted(object sender, object args)
        {
            radialMenu.PART_SelectionRim.Visibility = Visibility.Collapsed;
            radialMenu.DrillDownItem = this;
            radialMenu.AnimateIn(0, 1, 0.2, 0.2, EasingMode.EaseOut, null);
        }
#endif
        #endregion

        #region Callback Methods

#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
        private static void OnIsCheckedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as SfRadialMenuItem).OnIsCheckedChanged(args);
        }
#endif
        private static void OnHasItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SfRadialMenuItem control = sender as SfRadialMenuItem;
            if(control != null)
            {
                //if (control.HasItems)
                //{
                //    control.expanderRimItem.Visibility = Visibility.Visible;
                //    control.arrowRimItem.Visibility = Visibility.Visible;
                //}
                //else
                //{
                //    control.expanderRimItem.Visibility = Visibility.Collapsed;
                //    control.arrowRimItem.Visibility = Visibility.Collapsed;
                //}
            }
        }

        #if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
        private static void OnCheckedModeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is SfRadialMenuItem)
                (sender as SfRadialMenuItem).UpdateIsChecked();
        }
        #endif

        private static void OnInternalVisibilityChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is SfRadialMenuItem)
                (sender as SfRadialMenuItem).UpdateVisibility();
        }

        void UpdateVisibility()
        {
            if (this.Visibility == Visibility.Collapsed)
            {
                if (radialMenu != null && parentItem!=null )
                {
                    if (parentItem is SfRadialMenu)
                    {
                        var menu = (parentItem as SfRadialMenu);
                        itemIndex = menu.Items.IndexOf(this);
                        menu.Items.Remove(this);
                    }
                    else if (parentItem is SfRadialMenuItem)
                    {
                        var menuItem = (parentItem as SfRadialMenuItem);
                        itemIndex = menuItem.Items.IndexOf(this);
                        menuItem.Items.Remove(this);
                    }
                    radialMenu.UpdateLayout();
                    radialMenu.circularpanel.UpdateLayout();
                }
            }
            else
            {
                if (parentItem != null && itemIndex!=-1)
                {
                    if (parentItem is SfRadialMenu && !(parentItem as SfRadialMenu).Items.Contains(this))
                        (parentItem as SfRadialMenu).Items.Insert(itemIndex,this);
                    else if (parentItem is SfRadialMenuItem && !(parentItem as SfRadialMenuItem).Items.Contains(this))
                        (parentItem as SfRadialMenuItem).Items.Insert(itemIndex, this);

                }
                //radialMenu.UpdateLayout();
                //radialMenu.circularpanel.UpdateLayout();
            }
        }

        #endregion

        #region Helper Methods


#if WPF
        public void PrepareHeaderedItemsControlContainer(object item, ItemsControl parentItemsControl)
        {
            var parentDataTemplate = parentItemsControl.ItemTemplate;

            if (parentDataTemplate != null)
            {
                ItemTemplate = parentDataTemplate;
            }

            var template = parentDataTemplate as System.Windows.HierarchicalDataTemplate;

            if (template != null)
            {
                if (template.ItemTemplate != null)
                {
                    ItemTemplate = template.ItemTemplate;
                }
                else if (template.Template != null)
                {
                    ItemTemplate = parentItemsControl.ItemTemplate;
                }

                if (template.ItemsSource != null)
                {
                    Binding bind = template.ItemsSource as Binding;
                    
                    var binding = new Binding()
                    {
                        Source = Header,
                        Path = bind.Path,
                        Mode = bind.Mode,
                        Converter = bind.Converter,
                        ConverterParameter = bind.ConverterParameter,
                        ConverterCulture = bind.ConverterCulture,
                    };
                    this.SetBinding(ItemsSourceProperty, binding);
                }

                if (template.ItemContainerStyle != null)
                {
                    this.ItemContainerStyle = template.ItemContainerStyle;
                }
                if (parentDataTemplate != null)
                {
                    HeaderTemplate = parentDataTemplate;
                }
            }
        }
#endif

        private void UpdateCheckState(SfRadialMenuItem item)
        {
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
            if (item != null && item != this && item.CheckMode == Navigation.CheckMode.RadioButton)
            {
                if (item.GroupName != string.Empty && this.GroupName != string.Empty)
                {
                    if (item.GroupName.Equals(this.GroupName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        item.IsChecked = false;
                    }
                }
            }
#endif
        }

#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
        private void OnIsCheckedChanged(DependencyPropertyChangedEventArgs args)
        {
            UpdateIsChecked();
        }

        private void UpdateIsChecked()
        {
            if (this.IsChecked)
            {
                if (this.checkableRimItem != null)
                    VisualStateManager.GoToState(checkableRimItem, "Checked", true);
                if (this.CheckMode == Navigation.CheckMode.RadioButton)
                {
                    if (Checked != null)
                    {
                        Checked(this, new RoutedEventArgs());
                    }
                    if (this.Parent is SfRadialMenu)
                    {
                        SfRadialMenu rmenu = this.Parent as SfRadialMenu;
                        if (rmenu.ItemsSource != null)
                            for (int i = 0; i < rmenu.Items.Count; i++)
                                UpdateCheckState(rmenu.ItemContainerGenerator.ContainerFromIndex(i) as SfRadialMenuItem);
                        else
                            for (int i = 0; i < rmenu.Items.Count; i++)
                                UpdateCheckState(rmenu.Items[i] as SfRadialMenuItem);
                    }
                    if (this.Parent is SfRadialMenuItem)
                    {
                        SfRadialMenuItem rmenuitem = this.Parent as SfRadialMenuItem;
                        if (rmenuitem.ItemsSource != null)
                            for (int i = 0; i < rmenuitem.Items.Count; i++)
                                UpdateCheckState(rmenuitem.ItemContainerGenerator.ContainerFromIndex(i) as SfRadialMenuItem);
                        else
                            for (int i = 0; i < rmenuitem.Items.Count; i++)
                                UpdateCheckState(rmenuitem.Items[i] as SfRadialMenuItem);
                    }
                }
            }
            else
            {
                if (this.checkableRimItem != null)
                    VisualStateManager.GoToState(checkableRimItem, "UnChecked", true);
                if (UnChecked != null)
                {
                    UnChecked(this, new RoutedEventArgs());
                }
            }
        }
#endif
        #endregion

        #region Events
        /// <summary>
        /// Occurs when <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.RadialMenuItem"/> is clicked.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public event RoutedEventHandler Click;
#if !(WINDOWS_PHONE_7 || WINDOWS_PHONE)
        /// <summary>
        /// Occurs when the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.RadialMenuItem"/> is checked.
        /// </summary>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Navigation.RadialMenuItem.IsChecked"/>
        [ClassReference(IsReviewed = false)]
        public event RoutedEventHandler Checked;

        /// <summary>
        /// Occurs when the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.RadialMenuItem"/> is unchecked.
        /// </summary>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Navigation.RadialMenuItem.IsChecked"/>
        [ClassReference(IsReviewed = false)]
        public event RoutedEventHandler UnChecked;
#endif

        #endregion  
    }
}
