// <copyright file="RollingCharacter.cs" company="Syncfusion Software">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using Syncfusion.Windows.Shared;
using System.Globalization;
using Syncfusion.Licensing;
using System.Windows.Data;
namespace Syncfusion.Windows.Gauge
{
    /// <summary>
    /// Represents Segment of the Rolling Gauge
    /// </summary>
    /// <example>
    /// <code lang="XAML">
    /// <Window x:Class="CharacterFourteenSample.Window1" Title="CharacterFourteenSample" Height="400"
    /// Width="400" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    /// xmlns:syncfusion="http://schemas.syncfusion.com/wpf">
    ///     <Grid>
    ///         <syncfusion:RollingGauge SegmentBackground="Firebrick" SpaceBetWeenSegment="2" 
    ///                            Value="01" SegmentCount="2" Unit="KM" UnitPosition="End">
    ///             <syncfusion:RollingGauge.Segments>
    ///                 <syncfusion:RollingCharacter CharacterIndex="0" Background="Gray" Value="1" Margin="2"/>
    ///             </syncfusion:RollingGauge.Segments> 
    ///         </syncfusion:RollingGauge>
    ///     </Grid>
    /// </Window>
    /// </code>
    /// <code lang="C#">
    /// using System;
    /// using System.Collections.Generic;
    /// using System.Linq;
    /// using System.Text;
    /// using System.Windows;
    /// using System.Windows.Media;
    /// using System.Windows.Controls;
    /// using System.Windows.Data;
    /// using Syncfusion.Windows.Shared;
    /// using Syncfusion.Windows.Gauge;<para/>
    /// namespace CharacterRollingGaugeSample
    /// {
    ///         public partial class Window1 : Window
    ///         {
    ///             private DigitalGauge digitalGauge1;
    ///             public Window1()
    ///             {                
    ///                 InitializeComponent();<para/>
    ///                 RollingGauge rollingGauge = new RollingGauge();
    ///                 rollingGauge.SpaceBetWeenSegment = new Thickness(1);
    ///                 rollingGauge.Value = "11";
    ///                 rollingGauge.Unit = "KM";
    ///                 rollingGauge.UnitPosition = UnitPosition.End;
    ///                 RollingCharacter character = new RollingCharacter();
    ///                 character.Background = Brushes.Gray;
    ///                 character.BorderThickness = new Thickness(1);
    ///                 character.Margin = new Thickness(1);
    ///                 character.BorderBrush = Brushes.Black;
    ///                 character.Value = '0';
    ///                 character.CharacterIndex = 0;
    ///                 rollingGauge.Segments.Add(character);
    ///                 this.Content = rollingGauge;
    ///             }
    ///         }
    /// }   
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [DesignTimeVisible(false)]
#endif
    public class RollingCharacter : ContentControl
    {
        #region Private Members
        
        ///<summary>
        ///Parent to Rolling Character
        ///</summary>
        internal FrameworkElement GaugeParent;
        #endregion private Members

        #region Events
        /// <summary>
        /// Event that is raised when <see cref="Value"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ValueChanged;

        /// <summary>
        /// Event that is raised when <see cref="CharacterIndex"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback CharacterIndexChanged;

        #endregion Events

        #region  Dependency Properties

        /// <summary>
        /// Identifies the <see cref="CharacterIndex"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CharacterIndexProperty =
            DependencyProperty.Register("CharacterIndex", typeof(int), typeof(RollingCharacter), new FrameworkPropertyMetadata(-1, new PropertyChangedCallback(OnCharacterIndexChanged)));

        /// <summary>
        /// Identifies the <see cref="Value"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
          DependencyProperty.Register("Value", typeof(char), typeof(RollingCharacter), new FrameworkPropertyMetadata('\0', new PropertyChangedCallback(OnValueChanged)));

        /// <summary>
        /// Identifies the <see cref="CornerRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
          DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(RollingCharacter), new FrameworkPropertyMetadata(new CornerRadius()));
        #endregion Dependency Properties

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="RollingCharacter"/> class.
        /// </summary>
        static RollingCharacter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RollingCharacter), new FrameworkPropertyMetadata(typeof(RollingCharacter)));
        }
        #endregion Initialization

        #region CLR Getters & Setters

        /// <summary>
        /// Gets or sets the Value
        /// </summary>
        /// <value>Type: <see cref="char"/></value>
        public char Value
        {
            get { return (char)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Gets or sets the CharacterIndex
        /// </summary>
        /// <value>Type: <see cref="int"/></value>
        public int CharacterIndex
        {
            get { return (int)GetValue(CharacterIndexProperty); }
            set { SetValue(CharacterIndexProperty, value); }
        }

        /// <summary>
        /// Gets or sets the CornerRadius
        /// </summary>
        /// <value>Type: <see cref="double"/></value>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        #endregion CLR Getters & Setters

        #region Overrides

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }


        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            //contentpresenter = this.Template.FindName("ContentPresenter", this) as ContentPresenter;
            //border = this.Template.FindName("SegmentBorder", this) as Border;

            //if (contentpresenter != null)
            //{
            //Binding bind = new Binding();
            //bind.Source = this.GaugeParent;
            //bind.Path = new PropertyPath("SegmentTemplate");
            //this.contentpresenter.SetBinding(ContentPresenter.ContentTemplateProperty, bind);
            //}
        }
        #endregion Overrides

        #region Implementation

        /// <summary>
        /// Called when [value changed].
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RollingCharacter instance = (RollingCharacter)d;
            instance.OnValueChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:ValueChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnValueChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, e);
            }
        }

        /// <summary>
        /// Called when [character index changed].
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCharacterIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RollingCharacter instance = (RollingCharacter)d;
            instance.OnCharacterIndexChanged(e);
        }


        /// <summary>
        /// Raises the <see cref="E:CharacterIndexChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnCharacterIndexChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CharacterIndexChanged != null)
            {
                CharacterIndexChanged(this, e);
            }
        }
        #endregion Implementation

    }
}
