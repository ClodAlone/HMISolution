#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using Syncfusion.Windows.Shared;
using System.Globalization;
using System.Windows.Data;
namespace Syncfusion.Windows.Gauge
{
    /// <summary>
    /// Represents the Segment of the RollingGauge.
    /// </summary>
    /// <remarks>
    /// 	<para></para>
    /// 	<example>
    /// 		<para>The following example shows how to create a <see cref="RollingGauge">RollingGauge</see> in C#.</para>
    /// 		<para></para>
    /// 		<para></para>
    /// 		<para></para>
    /// 		<para></para>
    /// 		<list type="table">
    /// 			<listheader><term>C# :</term></listheader>
    /// 			<item>
    /// 				<description>
    /// 					<para></para>
    /// 					<para>using Syncfusion.Windows.Gauge;</para>
    /// 					<para>namespace RollingGaugeDemo</para>
    /// 					<para>{</para>
    /// 					<para>public partial class MainPage : UserControl</para>
    /// 					<para>{</para>
    /// 					<para>public MainPage()</para>
    /// 					<para>{</para>
    /// 					<para>InitializeComponent();</para>
    /// 					<para>RollingGauge rollingGauge = new RollingGauge();</para>
    /// 					<para>rollingGauge.SegmentCount = 4;</para>
    /// 					<para>rollingGauge.Value = "1000";</para>
    /// 					<para>rollingGauge.Unit = "KM";</para>
    /// 					<para>rollingGauge.UnitPosition = UnitPosition.End;</para>
    /// 					<para>RollingCharacter character = new RollingCharacter();</para>
    /// 					<para>character.CharacterIndex = 0;</para>
    /// 					<para>character.Value = "0";</para>
    /// 					<para>character.Margin = new Thickness(1);</para>
    /// 					<para>rollingGauge.Segments.Add(character);</para>
    /// 					<para>rollingGauge.Background = new SolidColorBrush(Colors.Blue);</para>
    /// 					<para>this.Content = rollingGauge;</para>
    /// 					<para>}</para>
    /// 					<para>}</para>
    /// 					<para>}</para>
    /// 				</description>
    /// 			</item>
    /// 		</list>
    /// 	</example>
    /// 	<para></para>
    /// </remarks>
    
    public class RollingCharacter : Control
    {
        #region Private Members
        ///<summary>
        ///Content of the segmentStackPanel
        ///</summary>
        private ContentPresenter contentpresenter;


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
            DependencyProperty.Register("CharacterIndex", typeof(int), typeof(RollingCharacter), new  PropertyMetadata(0, new PropertyChangedCallback(OnCharacterIndexChanged)));

        /// <summary>
        /// Identifies the <see cref="Value"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
          DependencyProperty.Register("Value", typeof(string), typeof(RollingCharacter), new PropertyMetadata("\0", new PropertyChangedCallback(OnValueChanged)));

        /// <summary>
        /// Identifies the <see cref="CornerRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
          DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(RollingCharacter), new PropertyMetadata(new CornerRadius()));
        #endregion Dependency Properties

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="RollingCharacter"/> class.
        /// </summary>
        static RollingCharacter()
        {
            if (DesignerProperties.IsInDesignTool)
            {
                LoadDependentAssemblies load = new LoadDependentAssemblies();
                load = null;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RollingCharacter"/> class.
        /// </summary>
        public RollingCharacter()
        {
            DefaultStyleKey = typeof(RollingCharacter);
        }

        #endregion Initialization

        #region CLR Getters & Setters

        /// <summary>
        /// Gets or sets the Value
        /// </summary>
        /// <value>Type: <see cref="char"/></value>
        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
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
        /// Builds the current template's visual tree if necessary.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            contentpresenter = this.GetTemplateChild("PART_ContentPresenter") as ContentPresenter;
            if (contentpresenter != null)
            {
                Binding bind = new Binding();
                bind.Source = this.GaugeParent;
                bind.Path = new PropertyPath("SegmentTemplate");
                this.contentpresenter.SetBinding(ContentPresenter.ContentTemplateProperty, bind);
            }
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
