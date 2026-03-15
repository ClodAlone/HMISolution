// <copyright file="DomainUpDown.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// DomainUpDown class that provides the UpDown control
    /// </summary>
    /// <remarks>
    /// Represents an up-down control that displays string values.
    /// <para>A DomainUpDown control displays a single string value that is selected from the collection of items
    /// by pressing the up or down buttons on keyboard or scrolling the mouse wheel. The user can also enter text in the control.</para>
    /// <para>To create a collection of items to display in the DomainUpDown control, user can add or remove
    /// the items individually by using the <see cref="Add"/> and <see cref="Remove"/> methods. This can be called in
    /// an event handler, such as the <see cref="Syncfusion.Windows.Shared.DomainUpDown"/> event of a button.</para>
    /// </remarks>
    /// <para/>
    /// <list type="table">
    /// <listheader>
    /// <term>Help Page</term>
    /// <description>Syntax</description>
    /// </listheader>
    /// <example>
    /// <list type="table">
    /// <listheader>
    /// <description>C#</description>
    /// </listheader>
    /// <example><code>public class DomainUpDown : Control</code></example>
    /// </list>
    /// <para/>
    /// <list type="table">
    /// <listheader>
    /// <description>XAML Object Element Usage</description>
    /// </listheader>
    /// <example>
    /// <code>
    /// <Window x:Class="WpfApplication10.Window1"
    /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf"
    /// Title="Window1" Height="300" Width="300">
    /// <Grid>
    /// <syncfusion:DomainUpDown Name="domainUpDown1"/>
    /// </Grid>
    /// </Window>
    /// </code>
    /// </example>
    /// </list>
    /// </example>
    /// </list>
    /// <example>
    /// <para/>The following example shows how to create a <see cref="DomainUpDown"/> in C#.
    /// <code>
    /// using System.Windows;
    /// using System.Windows.Controls;
    /// using Syncfusion.Windows.Tools.Controls;
    /// using Syncfusion.Windows.Tools;
    /// namespace UpDownControlSample
    /// {
    ///     public partial class Window1 : Window
    ///     {
    ///         // Counter used for adding items
    ///         int myCounter = 1;<para></para>
    ///          // Create a new instance of the UpDown
    ///         DomainUpDown domainUpDown1 = new DomainUpDown();
    ///         <para></para>
    ///         public Window1()
    ///         {
    ///             InitializeComponent();
    ///             <para></para>
    ///             // Add UpDown to grid
    ///             grid1.Children.Add( domainUpDown1 );
    ///         }
    ///         <para></para>
    ///         private void AddItem_Click(object sender, RoutedEventArgs e)
    ///         {
    ///             // Add item to domainUpDown
    ///             domainUpDown1.Add( "item " + myCounter.ToString() );
    ///              <para></para>
    ///             // Increment the counter variable.
    ///             myCounter++;
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class DomainUpDown : Control
    {
        #region Class constants

        /// <summary>
        /// Default animation speed of value change.
        /// </summary>
        protected const double DEF_ANIMATION_SPEED = 0.2d;

        #endregion Class constants

        #region Private Members

        /// <summary>
        /// Command for down button.
        /// </summary>
        public static RoutedCommand m_downValue;

        /// <summary>
        /// Command for down button.
        /// </summary>
        public static RoutedCommand m_upValue;

        /// <summary>
        /// First block for the animation of value change.
        /// </summary>
        private TextBox m_firstBlock;

        /// <summary>
        /// Second block for the animation of value change.
        /// </summary>
        private TextBox m_secondBlock;

        /// <summary>
        /// Textbox used for text processing.
        /// </summary>
        private TextBox m_textBox;

        /// <summary>
        /// Indicates whether control is animated.
        /// </summary>
        private bool m_isAnimated;

        /// <summary>
        /// Indicates direction for the animation of value change.
        /// </summary>
        private bool m_isUp;

        /// <summary>
        /// Previous item's index.
        /// </summary>
        private int m_exIndex;

        /// <summary>
        /// Item's index.
        /// </summary>
        private int m_index;

        /// <summary>
        /// Items list for the control.
        /// </summary>
        private ArrayList m_list;

        #endregion Private Members

        #region Properties

        /// <summary>
        /// Gets the <see cref="System.String"/> at the specified index.
        /// </summary>
        /// <param name="index">The index of the item</param>
        /// <value>Index of the item</value>
        public string this[int index]
        {
            get
            {
                return (string)m_list[index];
            }
        }

        /// <summary>
        /// Gets or sets the value of the control's value. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="string"/>
        /// Default value is null.
        /// </value>
        /// <seealso cref="string"/>
        public string Value
        {
            get
            {
                return (string)GetValue(ValueProperty);
            }

            set
            {
                SetValue(ValueProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the animation shift.
        /// </summary>
        /// <value>The animation shift.</value>
        public double AnimationShift
        {
            get
            {
                return (double)GetValue(AnimationShiftProperty);
            }

            set
            {
                SetValue(AnimationShiftProperty, value);
            }
        }

        #endregion Properties

        #region Dependency Properties

        /// <summary>
        /// Identifies <see cref="Value"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), typeof(DomainUpDown), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnValueChanged)));

        /// <summary>
        /// Identifies <see cref="AnimationShift"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AnimationShiftProperty =
            DependencyProperty.Register("AnimationShift", typeof(double), typeof(DomainUpDown), new UIPropertyMetadata(1d));

        #endregion Dependency Properties

        #region Class Initialize/Finalize methods

        /// <summary>
        /// Initializes static members of the <see cref="DomainUpDown"/> class.
        /// </summary>
        static DomainUpDown()
        {
            //// This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            //// This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DomainUpDown), new FrameworkPropertyMetadata(typeof(DomainUpDown)));
            m_downValue = new RoutedCommand();
            m_upValue = new RoutedCommand();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainUpDown"/> class.
        /// </summary>
        public DomainUpDown()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            m_list = new ArrayList();
            m_exIndex = 0;
            m_index = 0;
            CommandBinding downValueBinding = new CommandBinding(m_downValue);
            downValueBinding.Executed += new ExecutedRoutedEventHandler(ChangeDownValue);
            CommandBinding upValueBinding = new CommandBinding(m_upValue);
            upValueBinding.Executed += new ExecutedRoutedEventHandler(ChangeUpValue);
            CommandBindings.Add(downValueBinding);
            CommandBindings.Add(upValueBinding);
        }

        #endregion Class Initialize/Finalize methods

        #region Public Methods

        /// <summary>
        /// Adds the specified item to the collection.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>
        /// The index at which the value has been added.
        /// </returns>
        public int Add(string item)
        {
            if (m_list != null && (m_list.Count == 0 && m_textBox != null))
            {
                int result = m_list.Add(item);
                m_textBox.Text = (string)m_list[0];
                return result;
            }

            return m_list != null ? m_list.Add(item) : 0;
        }

        /// <summary>
        /// Removes item at the specified index from the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Index is less than zero or index is equal to or greater than <see cref="System.Collections.ArrayList.Count"/>.
        /// </exception>
        public void RemoveAt(int index)
        {
            if (m_list != null && m_list.Count > index) 
                m_list.RemoveAt(index);
        }

        /// <summary>
        /// Removes the first occurrence of a specific item from the collection.
        /// </summary>
        /// <param name="item">The item to remove from the collection.</param>
        public void Remove(string item)
        {
            if (m_list != null && item != null && m_list.Contains(item)) 
                m_list.Remove(item);
        }

        /// <summary>
        /// Adds the elements of an <see cref="System.Collections.ICollection"/> to the end
        /// of the collection.
        /// </summary>
        /// <param name="range">The <see cref="System.Collections.ICollection"/> whose elements
        /// should be added to the end of the collection. It cannot be
        ///  null itself, but it can contain elements that are null. </param>
        /// <exception cref="System.ArgumentNullException">range is null.</exception>
        public void AddRange(string[] range)
        {
            if (m_list != null && (m_list.Count == 0 && m_textBox != null))
            {
                if (range != null) m_list.AddRange(range);
                m_textBox.Text = (string)m_list[0];
            }
            else
            {
                if (m_list != null && range != null) 
                    m_list.AddRange(range);
            }
        }

        #endregion Public Methods

        #region Events

        /// <summary>
        /// Event that is raised when <see cref="Value"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ValueChanged;

        #endregion Events

        #region Static Methods

        /// <summary>
        /// Calls OnValueChanged method of the instance, notifies of the
        /// dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DomainUpDown instance = (DomainUpDown)d;
            if (instance != null) instance.OnValueChanged(e);
        }

        #endregion Static Methods

        #region Internals

        /// <summary>
        /// Builds the current template's visual tree if necessary.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_textBox = GetTemplateChild("TextBox") as TextBox;
            if (m_textBox != null)
            {
                m_textBox.FontSize = FontSize;
                if (m_list != null && m_list.Count != 0)
                {
                    m_textBox.Text = (string)m_list[0];
                }
            }

            m_firstBlock = GetTemplateChild("FirstBlock") as TextBox;
            if (m_firstBlock != null) 
                m_firstBlock.FontSize = FontSize;
            m_secondBlock = GetTemplateChild("SecondBlock") as TextBox;
            if (m_secondBlock != null) 
                m_secondBlock.FontSize = FontSize;
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.KeyDown"/>�attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs"/> that contains the event data.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (Key.Up == e.Key)
            {
                UpdateCounter(true);
                e.Handled = true;
            }

            if (Key.Down == e.Key)
            {
                UpdateCounter(false);
                e.Handled = true;
            }

            base.OnKeyDown(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseWheel"/>�attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseWheelEventArgs"/> that contains the event data.</param>
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                UpdateCounter(true);
            }

            if (e.Delta < 0)
            {
                UpdateCounter(false);
            }

            base.OnMouseWheel(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="ValueChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnValueChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, e);
            }
        }

        /// <summary>
        /// Changes the down value.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private void ChangeDownValue(object sender, ExecutedRoutedEventArgs e)
        {
            UpdateCounter(false);
        }

        /// <summary>
        /// Changes the up value.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private void ChangeUpValue(object sender, ExecutedRoutedEventArgs e)
        {
            UpdateCounter(true);
        }

        /// <summary>
        /// Updates the counter.
        /// </summary>
        /// <param name="isUp">Indicates direction of value change.</param>
        private void UpdateCounter(bool isUp)
        {
            if (isUp)
            {
                if (m_list != null && m_index < m_list.Count - 1)
                {
                    m_index++;
                }
                else
                {
                    m_index = 0;
                }

                m_isUp = true;
            }
            else
            {
                if (m_index > 0)
                {
                    m_index--;
                }
                else
                {
                    if (m_list != null) 
                        m_index = m_list.Count - 1;
                }

                m_isUp = false;
            }

            if (m_list != null && (m_textBox != null && ((m_list.Count > m_exIndex && m_textBox.Text != (string)m_list[m_exIndex]) && !m_isAnimated)))
            {
                m_list[m_exIndex] = m_textBox.Text;
            }

            Animation();
        }

        /// <summary>
        /// Animates value changes when the up or down buttons are clicked.
        /// </summary>
        private void Animation()
        {
            if (!m_isAnimated)
            {
                if (m_textBox != null) 
                    m_textBox.Visibility = Visibility.Hidden;
                if (m_firstBlock != null) 
                    m_firstBlock.Visibility = Visibility.Visible;
                if (m_secondBlock != null)
                    m_secondBlock.Visibility = Visibility.Visible;

                DoubleAnimation animation = new DoubleAnimation
                    {
                        Duration = new Duration(TimeSpan.FromSeconds(DEF_ANIMATION_SPEED))
                    };
                animation.Completed += new EventHandler(Animation_Completed);

                if (m_isUp)
                {
                    animation.From = 0;
                    if (m_secondBlock != null)
                    {
                        animation.To = m_secondBlock.ActualHeight*-1;
                        m_secondBlock.Text = (string)m_list[m_index];
                    }
                    if (m_firstBlock != null) 
                        m_firstBlock.Text = (string)m_list[m_exIndex];
                }
                else
                {
                    if (m_secondBlock != null)
                    {
                        animation.From = m_secondBlock.ActualHeight * -1;
                        m_secondBlock.Text = (string)m_list[m_exIndex];
                    }
                    animation.To = 0;
                    if (m_firstBlock != null) 
                        m_firstBlock.Text = (string)m_list[m_index];
                }

                m_isAnimated = true;
                m_exIndex = m_index;
                this.BeginAnimation(DomainUpDown.AnimationShiftProperty, animation);
            }
        }

        /// <summary>
        /// Handles the <see cref="Animation_Completed"/> event of the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> that contains the event data.</param>
        private void Animation_Completed(object sender, EventArgs e)
        {
            m_isAnimated = false;
            if (m_list != null)
            {
                if (m_list.Count > m_index) 
                    Value = (string)m_list[m_index];
                if (m_secondBlock != null && (m_firstBlock != null && (((m_list.Count > m_index && (string)m_list[m_index] != m_firstBlock.Text) && !m_isUp) || ((m_list.Count > m_index && (string)m_list[m_index] != m_secondBlock.Text) && m_isUp))))
                {
                    Animation();
                }
                else
                {
                    if (m_textBox != null) 
                        m_textBox.Visibility = Visibility.Visible;
                    if (m_firstBlock != null) 
                        m_firstBlock.Visibility = Visibility.Hidden;
                    if (m_secondBlock != null) 
                        m_secondBlock.Visibility = Visibility.Hidden;
                }
            }
        }

        #endregion Internals
    }
}