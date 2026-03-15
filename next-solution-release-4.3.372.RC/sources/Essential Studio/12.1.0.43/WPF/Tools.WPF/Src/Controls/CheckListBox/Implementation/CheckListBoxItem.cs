// <copyright file="CheckListBoxItem.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Syncfusion.Licensing;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// CheckListBox Items are hosted inside CheckListBox Control. They have the support
    /// for the drag and drop of the items.
    /// </summary>
    /// <example>
    ///  <code lang="XAML">
    /// <Window x:Class="Checklistbox.Window1"
    ///     xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    ///     xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    ///     Title="Window1" Height="300" Width="300"
    /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf">
    ///     <Grid>
    ///         <syncfusion:CheckListBox Name="myCheckListBox" >
    ///             <syncfusion:CheckListBoxItem Content="Mexico"/>
    ///             <syncfusion:CheckListBoxItem Content="Canada" />
    ///             <syncfusion:CheckListBoxItem Content="Bermuda" />
    ///             <syncfusion:CheckListBoxItem Content="Belize" />
    ///             <syncfusion:CheckListBoxItem Content="Panama" />
    ///             <syncfusion:CheckListBoxItem Content="Costa Rica" />
    ///             <syncfusion:CheckListBoxItem Content="Brazil" />
    ///             <syncfusion:CheckListBoxItem Content="Argentina" />
    ///             <syncfusion:CheckListBoxItem Content="Colombia" />
    ///         </syncfusion:CheckListBox>
    ///     </Grid>
    /// </Window>
    /// </code>
    /// <code lang="C#">
    /// using System;
    /// using System.Collections.Generic;
    /// using System.Linq;
    /// using System.Text;
    /// using System.Windows;
    /// using System.Windows.Controls;
    /// using System.Windows.Data;
    /// using System.Windows.Documents;
    /// using System.Windows.Input;
    /// using System.Windows.Media;
    /// using System.Windows.Media.Imaging;
    /// using System.Windows.Navigation;
    /// using System.Windows.Shapes;
    /// using Syncfusion.Windows.Tools.Controls;
    /// namespace checklistbox
    /// {
    ///     /// <summary>
    ///     /// Interaction logic for Window1.xaml
    ///     /// </summary>
    ///     public partial class Window1 : Window
    ///     {
    ///         public Window1()
    ///         {
    ///             InitializeComponent();
    ///             CheckListBox checklistbox = new CheckListBox();
    ///             CheckListBoxItem item = new CheckListBoxItem();
    ///             checklistbox.Items.Add(item);
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class CheckListBoxItem : ContentControl, ICloneable
    {
        #region fields

        /// <summary>
        /// Presents the FirstClick value
        /// </summary>
        private bool m_isFirstClick = true;

        #endregion fields

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="CheckListBoxItem"/> class.
        /// </summary>
        static CheckListBoxItem()
        {
            EnvironmentTest.ValidateLicense(typeof(CheckListBoxItem));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CheckListBoxItem), new FrameworkPropertyMetadata(typeof(CheckListBoxItem)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckListBoxItem"/> class.
        /// </summary>
        public CheckListBoxItem()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (Application.Current != null && !Syncfusion.Windows.Shared.BindingUtils.GetEnableBindingErrors(Application.Current.MainWindow))
                    System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
            }
        }

        #endregion Initialization

        #region Dependency Properties

        /// <summary>
        /// Represents the IsSelected Dependency property
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty = Selector.IsSelectedProperty.AddOwner(typeof(CheckListBoxItem), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(CheckListBoxItem.OnIsSelectedChanged)));

        #endregion Dependency Properties

        #region Properties Setter/Getter

        /// <summary>
        /// Gets or sets a value indicating whether this instance is first click.
        /// </summary>
        /// <value>
        /// true if this instance is first click; otherwise, false.
        /// </value>
        internal bool IsFirstClick
        {
            get { return m_isFirstClick; }
            set { m_isFirstClick = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        /// true if this instance is selected; otherwise, false.
        /// </value>
        [Bindable(true), Category("Appearance")]
        public bool IsSelected
        {
            get
            {
                return (bool)base.GetValue(IsSelectedProperty);
            }

            set
            {
                base.SetValue(IsSelectedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the check box object.
        /// </summary>
        /// <value>The check box object.</value>
        internal CheckBox CheckBoxObject
        {
            get;
            set;
        }

        #endregion Properties Setter/Getter

        #region Events

        /// <summary>
        /// Represents the selected Event of the checklistbox items
        /// </summary>
        public static readonly RoutedEvent SelectedEvent = Selector.SelectedEvent.AddOwner(typeof(CheckListBoxItem));

        /// <summary>
        /// Represents the Unselected Event of the checklistbox items
        /// </summary>
        public static readonly RoutedEvent UnselectedEvent = Selector.UnselectedEvent.AddOwner(typeof(CheckListBoxItem));

        /// <summary>
        /// Occurs when [selected].
        /// </summary>
        public event RoutedEventHandler Selected
        {
            add
            {
                base.AddHandler(SelectedEvent, value);
            }

            remove
            {
                base.RemoveHandler(SelectedEvent, value);
            }
        }

        /// <summary>
        /// Occurs when [unselected].
        /// </summary>
        public event RoutedEventHandler Unselected
        {
            add
            {
                base.AddHandler(UnselectedEvent, value);
            }

            remove
            {
                base.RemoveHandler(UnselectedEvent, value);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:Selected"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnSelected(RoutedEventArgs e)
        {
            this.HandleIsSelectedChanged(true, e);
        }

        /// <summary>
        /// Raises the <see cref="E:Unselected"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnUnselected(RoutedEventArgs e)
        {
            this.HandleIsSelectedChanged(false, e);
        }

        /// <summary>
        /// Handles the is selected changed.
        /// </summary>
        /// <param name="newValue">if set to <c>true</c> [new value].</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void HandleIsSelectedChanged(bool newValue, RoutedEventArgs e)
        {
            base.RaiseEvent(e);
        }

        /// <summary>
        /// Called when [is selected changed].
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs<see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CheckListBoxItem container = d as CheckListBoxItem;
            bool newValue = (bool)e.NewValue;
            if (newValue)
            {
                container.OnSelected(new RoutedEventArgs(Selector.SelectedEvent, container));
            }
            else
            {
                container.OnUnselected(new RoutedEventArgs(Selector.UnselectedEvent, container));
            }
        }

        #endregion Events

        #region Overriden methods

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        /// <summary>
        /// Invoked whenever an unhandled <see cref="E:System.Windows.UIElement.GotFocus"/> event reaches this element in its route.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.UIElement.LostFocus"/> routed event by using the event data that is provided.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.RoutedEventArgs"/> that contains event data. This event data must contain the identifier for the <see cref="E:System.Windows.UIElement.LostFocus"/> event.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.LostMouseCapture"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains event data.</param>
        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            base.OnLostMouseCapture(e);
        }

        #endregion Overriden methods

        #region ICloneable Members

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            CheckListBoxItem clone = new CheckListBoxItem();
            clone.Content = this.Content;
            clone.Tag = this.Tag;
            clone.Background = this.Background;
            clone.BorderBrush = this.BorderBrush;
            clone.BorderThickness = this.BorderThickness;
            clone.FontSize = this.FontSize;
            clone.FontStyle = this.FontStyle;
            clone.FontStretch = this.FontStretch;
            clone.FontWeight = this.FontWeight;
            clone.HorizontalAlignment = this.HorizontalAlignment;
            clone.HorizontalContentAlignment = this.HorizontalContentAlignment;
            clone.IsSelected = this.IsSelected;
            clone.Width = this.Width;
            clone.Height = this.Height;

            return clone;
        }

        #endregion ICloneable Members
    }
}