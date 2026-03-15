// <copyright file="RibbonButton.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;
using System.Windows.Data;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents RibbonButton control.
    /// </summary>
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
    /// <example><code>public class RibbonButton : ButtonBase</code></example>
    /// </list>
    /// <para/>
    /// <list type="table">
    /// <listheader>
    /// <description>XAML Object Element Usage</description>
    /// </listheader>
    /// <example><code><![CDATA[<ribbon:RibbonButton Name="button" />]]></code></example>
    /// </list>
    /// </example>
    /// </list>
    /// <remarks>
    /// RibbonButton class represents a Ribbon button control.
    /// </remarks>
    /// <example>
    /// <para/>This example shows how to create a RibbonButton in XAML.
    /// <code>
    /// <![CDATA[<ribbon:RibbonButton SizeForm="Small" Label="Item1" SmallIcon="SampleImages/Document32.png"/>]]></code>
    /// <para/>This example shows how to create a RibbonButton in C#.
    /// <code>
    /// RibbonButton button = new RibbonButton();
    /// button.Label = "Item";
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class RibbonButton : ButtonBase, ICollapsable
    {
        #region	Initialization

        /// <summary>
        /// Represent to store the Large Label of Ribbon Button.
        /// </summary>
        internal string tempLabel = null;

        SystemGesture m_systemGesture;

        /// <summary>
        /// Initializes static members of the <see cref="RibbonButton"/> class.
        /// </summary>
        static RibbonButton()
        {
           // EnvironmentTest.ValidateLicense(typeof(RibbonButton));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonButton), new FrameworkPropertyMetadata(typeof(RibbonButton)));
            CornerRadiusProperty = Border.CornerRadiusProperty.AddOwner(typeof(RibbonButton), new FrameworkPropertyMetadata(Border.CornerRadiusProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.None));
            FocusableProperty.OverrideMetadata(typeof(RibbonButton), new FrameworkPropertyMetadata(true));
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonButton"/> class.
        /// </summary>
        public RibbonButton()
        {
            if (EnvironmentTestTools.IsSecurityGranted)
            {
                EnvironmentTestTools.StartValidateLicense(typeof(RibbonButton));
            }
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                if (Application.Current != null && !BindingUtils.GetEnableBindingErrors(Application.Current.MainWindow))
                    System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
            }

            Focusable = false;
            //SetRibbonButtonDefaults();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value that represents the degree to which the corners of a <see cref="RibbonButton"/> are rounded.
        /// </summary>
        /// <value>
        /// Type: <see cref="CornerRadius"/>
        /// The CornerRadius that describes the degree to which corners are rounded. This property has no default value.
        /// </value>
        /// <remarks>
        /// Although the property name suggests that you can use only singular values, CornerRadius also supports non-uniform radii. Radius values that are too large are scaled so that they blend smoothly from corner to corner.
        /// </remarks>
        /// <example>
        /// <code>
        /// RibbonButton button = new RibbonButton();
        /// button.Label = "Button";        
        /// button.CornerRadius = new CornerRadius(3);
        /// </code>
        /// </example>
        /// <seealso cref="RibbonButton"/>
        /// <seealso cref="CornerRadius"/>
        public CornerRadius CornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(CornerRadiusProperty);
            }

            set
            {
                SetValue(CornerRadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the small icon that appears in a <see cref="RibbonButton"/>.
        /// </summary>
        /// <remarks>
        /// Many controls have more than just text in the element. Often there is an icon. 
        /// </remarks>
        /// <example>
        /// <code>
        /// // Create the image element.
        /// RibbonButton button = new RibbonButton();
        /// // Create source.
        /// BitmapImage bimage = new BitmapImage();
        /// // BitmapImage.UriSource must be in a BeginInit/EndInit block.
        /// bimage.BeginInit();
        /// bimage.UriSource = new Uri(@"/sampleImages/sample.jpg",UriKind.RelativeOrAbsolute);
        /// bimage.EndInit();
        /// // Set the image source.
        /// button.SmallIcon = bimage;
        /// </code>
        /// </example>
        /// <seealso cref="RibbonButton"/>
        /// <seealso cref="ImageSource"/>
        public ImageSource SmallIcon
        {
            get
            {
                return (ImageSource)GetValue(SmallIconProperty);
            }

            set
            {
                SetValue(SmallIconProperty, value);
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether this instance is menu item.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is menu item; otherwise, <c>false</c>.
        /// </value>
        public bool IsMenuItem
        {
            get { return (bool)GetValue(IsMenuItemProperty); }
            set { SetValue(IsMenuItemProperty, value); }
        }  

        /// <summary>
        /// Gets or sets the large icon that appears in a <see cref="RibbonButton"/>.
        /// </summary>
        /// <remarks>
        /// Many controls have more than just text in the element. Often there is an icon. 
        /// </remarks>
        /// <example>
        /// <code>
        /// // Create the image element.
        /// RibbonButton button = new RibbonButton();
        /// // Create source.
        /// BitmapImage bimage = new BitmapImage();
        /// // BitmapImage.UriSource must be in a BeginInit/EndInit block.
        /// bimage.BeginInit();
        /// bimage.UriSource = new Uri(@"/sampleImages/sample.jpg",UriKind.RelativeOrAbsolute);
        /// bimage.EndInit();
        /// // Set the image source.
        /// button.LargeIcon = bimage;
        /// </code>
        /// </example>
        /// <seealso cref="RibbonButton"/>
        /// <seealso cref="ImageSource"/>
        public ImageSource LargeIcon
        {
            get
            {
                return (ImageSource)GetValue(LargeIconProperty);
            }

            set
            {
                SetValue(LargeIconProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the text that labels the <see cref="RibbonButton"/>.
        /// </summary>
        /// <value>
        /// Type: <see cref="String"/>
        /// Text that labels the <see cref="RibbonButton"/>. The default is empty string.
        /// </value>
        /// <example>
        /// <code>
        /// RibbonButton button = new RibbonButton();
        /// button.Label = "Button";        
        /// button.CornerRadius = new CornerRadius(3);
        /// </code>
        /// </example>
        /// <seealso cref="RibbonButton"/>
        /// <seealso cref="string"/>
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
        /// Gets and Sets a value indicating whether Label text can be wrraped to next line.
        /// </summary>
        public bool IsMultiLine
        {
            get 
            { 
                return (bool)GetValue(IsMultiLineProperty);
            }
            set
            {
                SetValue(IsMultiLineProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the size form.<see cref="RibbonButton"/> size.
        /// </summary>
        /// <value>
        /// Type: <see cref="SizeForm"/>
        /// Enumeration that specifies type of the <see cref="RibbonButton"/> size.
        /// </value>
        /// <example>
        /// <code>
        /// RibbonButton button = new RibbonButton();
        /// button.Label = "Button";        
        /// button.SizeForm = SizeForm.Large;
        /// </code>
        /// </example>
        /// <remarks>
        /// Changing <see cref="RibbonButton"/> SizeForm property changes it's visual representation.  Text is only rendered in Large and Small variants.
        /// </remarks>
        /// <seealso cref="RibbonButton"/>
        /// <seealso cref="SizeForm"/>
        public SizeForm SizeForm
        {
            get
            {
                return (SizeForm)GetValue(SizeFormProperty);
            }

            set
            {
                SetValue(SizeFormProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected. <see cref="RibbonButton"/> is selected.   
        /// </summary>
        /// <value>
        /// Type: <see cref="Boolean"/>
        /// true if the <see cref="RibbonButton"/> is selected; false if the <see cref="RibbonButton"/> is not selected.  The default is false.
        /// </value>
        /// <example>
        /// <code>
        /// RibbonButton button = new RibbonButton();
        /// button.Label = "Button";  
        /// button.IsToggle = true;
        /// button.IsSelected = true;
        /// </code>
        /// </example>
        /// <seealso cref="RibbonButton"/>
        /// <seealso cref="Boolean"/>
        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(IsSelectedProperty);
            }

            set
            {
                SetValue(IsSelectedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="RibbonButton"/> is toggle button.
        /// </summary>
        /// <value>
        /// Type: <see cref="Boolean"/>
        /// true if the <see cref="RibbonButton"/> is toggle; false if the <see cref="RibbonButton"/> is not toggle.  The default is false.
        /// </value>
        /// <example>
        /// <code>
        /// RibbonButton button = new RibbonButton();
        /// button.Label = "Button";  
        /// button.IsToggle = true;
        /// button.IsSelected = true;
        /// </code>
        /// </example>
        /// <seealso cref="RibbonButton"/>
        /// <seealso cref="Boolean"/>
        public bool IsToggle
        {
            get
            {
                return (bool)GetValue(IsToggleProperty);
            }

            set
            {
                SetValue(IsToggleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance small image is set.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is small image visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsSmallImageVisible
        {
            get
            {
                return (bool)GetValue(IsSmallImageVisibleProperty);
            }

            set
            {
                SetValue(IsSmallImageVisibleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance large image is set.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is large image visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsLargeImageVisible
        {
            get
            {
                return (bool)GetValue(IsLargeImageVisibleProperty);
            }

            set
            {
                SetValue(IsLargeImageVisibleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [split label into two line].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [split label into two line]; otherwise, <c>false</c>.
        /// </value>
        public bool SplitLabelIntoTwoLine
        {
            get
            {
                return (bool)GetValue(SplitLabelIntoTwoLineProperty);
            }

            set
            {
                SetValue(SplitLabelIntoTwoLineProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the collapse label.
        /// </summary>
        /// <value>The collapse label.</value>
        public string CollapseLabel
        {
            get { return (string)GetValue(CollapseLabelProperty); }
            set { SetValue(CollapseLabelProperty, value); }
        }        

        #endregion

        #region Events
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Event that is raised when SmallIcon property is changed.
        /// </summary>
        public event PropertyChangedCallback SmallIconChanged;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Event that is raised when LargeIcon property is changed.
        /// </summary>
        public event PropertyChangedCallback LargeIconChanged;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Event that is raised when Label property is changed.
        /// </summary>
        public event PropertyChangedCallback LabelChanged;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Event that is raised when SizeForm property is changed.
        /// </summary>
        public event PropertyChangedCallback SizeFormChanged;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Event that is raised when IsSelected property is changed.
        /// </summary>
        public event PropertyChangedCallback IsSelectedChanged;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Event that is raised when IsToggle property is changed.
        /// </summary>
        public event PropertyChangedCallback IsToggleChanged;

        /// <summary>
        /// Occurs when [is multi line changed].
        /// </summary>
        public event PropertyChangedCallback IsMultiLineChanged;

        /// <summary>
        /// Occurs when [split label into two line changed].
        /// </summary>
        public event PropertyChangedCallback SplitLabelIntoTwoLineChanged;

        #endregion

        #region Dependency Properties
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Defines corner radius of button. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Defines small button icon. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty SmallIconProperty =
            DependencyProperty.Register("SmallIcon", typeof(ImageSource), typeof(RibbonButton), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSmallIconChanged)));

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Defines large button icon. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty LargeIconProperty =
            DependencyProperty.Register("LargeIcon", typeof(ImageSource), typeof(RibbonButton), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnLargeIconChanged)));

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Defines button label. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(RibbonButton), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnLabelChanged),new CoerceValueCallback(CoerceOnLabel)));

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Defines size of the button. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty SizeFormProperty =
            DependencyProperty.Register("SizeForm", typeof(SizeForm), typeof(RibbonButton), new FrameworkPropertyMetadata(SizeForm.Small, new PropertyChangedCallback(OnSizeFormChanged)));

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Defines when button is selected. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(RibbonButton), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsSelectedChanged)));

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Defines when button is toggle. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty IsToggleProperty =
            DependencyProperty.Register("IsToggle", typeof(bool), typeof(RibbonButton), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsToggleChanged)));

        /// <summary>
        /// Identifies the small icon is set in the control.
        /// </summary>
        protected internal static readonly DependencyProperty IsSmallImageVisibleProperty =
            DependencyProperty.Register("IsSmallImageVisible", typeof(bool), typeof(RibbonButton), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Identifies the large icon is set in the control.
        /// </summary>
        protected internal static readonly DependencyProperty IsLargeImageVisibleProperty =
            DependencyProperty.Register("IsLargeImageVisible", typeof(bool), typeof(RibbonButton), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Identifies whether multiline is set to the Label property.
        /// </summary>
        protected static readonly DependencyProperty IsMultiLineProperty =
           DependencyProperty.Register("IsMultiLine", typeof(bool), typeof(RibbonButton), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsMultiLineChanged), new CoerceValueCallback(CoerceOnIsMultiline)));

        /// <summary>
        /// Identifies the SplitLabelIntoTwoLine Property.
        /// </summary>
        protected static readonly DependencyProperty SplitLabelIntoTwoLineProperty =
          DependencyProperty.Register("SplitLabelIntoTwoLine", typeof(bool), typeof(RibbonButton), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnSplitLabelIntoTwoLineChanged)));

        // Using a DependencyProperty as the backing store for CollapseLabel.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Get or Sets the Collapse Label of Ribbon Button. It is a dependency property.
        /// </summary>
        public static readonly DependencyProperty CollapseLabelProperty =
            DependencyProperty.Register("CollapseLabel", typeof(string), typeof(RibbonButton), new UIPropertyMetadata(null));

        // Using a DependencyProperty as the backing store for IsMenuItem.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Used to know about this is MenuItem or not.
        /// </summary>
        public static readonly DependencyProperty IsMenuItemProperty =
            DependencyProperty.Register("IsMenuItem", typeof(bool), typeof(RibbonButton), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies the Label Itemscontrol.
        /// </summary>
        private ItemsControl m_label = null;

        #endregion

        #region	Implementation
        /// <summary>
        /// Calls OnSmallIconChanged method of the instance, notifies of
        /// the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnSmallIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonButton instance = (RibbonButton)d;
            instance.IsSmallImageVisible = (instance.SmallIcon != null);
            instance.OnSmallIconChanged(e);            
        }

        /// <summary>
        /// Updates property value cache and raises SmallIconChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnSmallIconChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SmallIconChanged != null)
            {
                SmallIconChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnLargeIconChanged method of the instance, notifies of
        /// the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnLargeIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonButton instance = (RibbonButton)d;
            instance.IsLargeImageVisible = (instance.LargeIcon != null);
            instance.OnLargeIconChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises LargeIconChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnLargeIconChanged(DependencyPropertyChangedEventArgs e)
        {
            if (LargeIconChanged != null)
            {
                LargeIconChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnLabelChanged method of the instance, notifies of the
        /// dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonButton instance = (RibbonButton)d;
            instance.OnLabelChanged(e);
            
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Updates property value cache and raises LabelChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnLabelChanged(DependencyPropertyChangedEventArgs e)
        {
            if (LabelChanged != null)
            {
               
                LabelChanged(this, e);
            }

            //var tag = this.Tag as string;


            if (this.Tag== null || !this.Tag.ToString().Equals("Resized", StringComparison.OrdinalIgnoreCase))
            {
                this.tempLabel = this.Label;
            }
        }

        /// <summary>
        /// Called when [is multi line changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsMultiLineChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonButton instance = (RibbonButton)d;
          
            instance.OnIsMultiLineChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:IsMultiLineChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnIsMultiLineChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsMultiLineChanged != null)
            {
                IsMultiLineChanged(this, e);
            }
        }

        /// <summary>
        /// Called when [is multi line changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSplitLabelIntoTwoLineChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonButton instance = (RibbonButton)d;

            instance.OnSplitLabelIntoTwoLineChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:IsMultiLineChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnSplitLabelIntoTwoLineChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SplitLabelIntoTwoLineChanged != null)
            {
                SplitLabelIntoTwoLineChanged(this, e);
            }
        }


        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Calls OnSizeFormChanged method of the instance, notifies of
        /// the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnSizeFormChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonButton instance = (RibbonButton)d;
            instance.OnSizeFormChanged(e);
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Updates property value cache and raises SizeFormChanged
        /// event.
        /// </summary>
        /// <param name="e">Property change details, such as old value
        /// and new value.</param>
        protected virtual void OnSizeFormChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SizeFormChanged != null)
            {
                SizeFormChanged(this, e);
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Calls OnIsSelectedChanged method of the instance, notifies of
        /// the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonButton instance = (RibbonButton)d;
            instance.OnIsSelectedChanged(e);
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Updates property value cache and raises IsSelectedChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnIsSelectedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsSelectedChanged != null)
            {
                IsSelectedChanged(this, e);
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Calls OnIsToggleChanged method of the instance, notifies of
        /// the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value
        /// and new value.</param>
        private static void OnIsToggleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonButton instance = (RibbonButton)d;
            instance.OnIsToggleChanged(e);
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Updates property value cache and raises IsToggleChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnIsToggleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsToggleChanged != null)
            {
                IsToggleChanged(this, e);
            }
        }
        
        /// <summary>
        /// Gets the event handlers.
        /// </summary>
        /// <param name="eventNames">The event names.</param>
        /// <param name="eventHandlers">The event handlers.</param>
        internal void GetEventHandlers(ref string[] eventNames, ref Delegate[] eventHandlers)
        {
            PropertyChangedCallback propertyEvents = (PropertyChangedCallback)PropertyChangedCallback.Combine(
                new PropertyChangedCallback[] 
                { 
                    IsSelectedChanged, 
                    IsToggleChanged, 
                    LabelChanged, 
                    LargeIconChanged, 
                    SizeFormChanged 
                });

            eventNames = new string[] 
            { 
                    IsSelectedChanged != null ? string.Format("{0} {1}", "IsSelectedChanged", IsSelectedChanged.Method.Name) : null, 
                    IsToggleChanged != null ? string.Format("{0} {1}", "IsToggleChanged", IsToggleChanged.Method.Name) : null,
                    LabelChanged != null ? string.Format("{0} {1}", "LabelChanged", LabelChanged.Method.Name) : null,
                    LargeIconChanged != null ? string.Format("{0} {1}", "LargeIconChanged", LargeIconChanged.Method.Name) : null,
                    SizeFormChanged != null ? string.Format("{0} {1}", "SizeFormChanged", SizeFormChanged.Method.Name) : null
            };

            if (propertyEvents != null)
            {
                eventHandlers = propertyEvents.GetInvocationList();
            }
        }

        /// <summary>
        /// Coerces the on label.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="baseValue">The base value.</param>
        /// <returns></returns>
        public static object CoerceOnLabel(DependencyObject d, object baseValue)
        {
            RibbonButton instance = (RibbonButton)d;
            return instance.CoerceOnLabel(baseValue);
        }

        /// <summary>
        /// Coerces the on label.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected virtual object CoerceOnLabel(object value)
        {
       
            string coerceValue = (string)value;
            if (coerceValue != null)
            {
                if (IsMultiLine)
                {
                    coerceValue = coerceValue.Replace("\\n", "\n");
                }
                else
                {
                    coerceValue = coerceValue.Replace("\n", "\n");
                }
                if (SizeForm == SizeForm.Large)
                    coerceValue = coerceValue.Replace("\\t", " ");
            }  
            return coerceValue;
        }

        /// <summary>
        /// Coerces the on is multiline.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="baseValue">The base value.</param>
        /// <returns></returns>
        public static object CoerceOnIsMultiline(DependencyObject d, object baseValue)
        {
            RibbonButton instance = (RibbonButton)d;
            return instance.CoerceOnIsMultiline(baseValue);
        }

        /// <summary>
        /// Coerces the on is multiline.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected virtual object CoerceOnIsMultiline(object value)
        {
            
            bool coerceValue = (bool)value;

            if (coerceValue)
            {
                if (Label != null)
                { 
                    Label = Label.Replace("\\n", "\n");
                }
            }
            else
            {
                if (Label != null)
                {
                    Label = Label.Replace("\n", "\\n");
                }
            }
           

            return coerceValue;
        }

      
        #endregion

        #region Overrides
        
        /// <summary>
        /// When overridden in a derived class, is invoked whenever
        /// application code or internal processes call ApplyTemplate.
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (SizeForm == SizeForm.Large)
            {
                m_label = (ItemsControl)GetTemplateChild("PART_Label");
            }
            GetSynchronizedCommand();

            if (Command != null)
            {
                if (RibbonCommandManager.CommandDictionary.ContainsKey(Command))
                {
                    if (RibbonCommandManager.CommandDictionary[Command].SmallIcon != null)
                    {
                        SmallIcon = RibbonCommandManager.CommandDictionary[Command].SmallIcon;
                    }

                    if (RibbonCommandManager.CommandDictionary[Command].Label != null)
                    {
                        Label = RibbonCommandManager.CommandDictionary[Command].Label;
                    }

                    if (RibbonCommandManager.CommandDictionary[Command].ToolTip != null)
                    {
                        ToolTip = RibbonCommandManager.CommandDictionary[Command].ToolTip;
                    }
                }
                
            }

            Binding binding = new Binding("Label");
            binding.Source = this;
            TextWrappingConverter conv = new TextWrappingConverter();
            binding.Converter = conv;
            binding.ConverterParameter = SplitLabelIntoTwoLine;

            if (m_label != null)
            {
                m_label.SetBinding(ItemsControl.ItemsSourceProperty, binding);
            }
            base.OnApplyTemplate();
        }

       

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Controls.Primitives.ButtonBase.Click"/> routed event.
        /// </summary>
        protected override void OnClick()
        {
            if (IsToggle)
            {
                IsSelected = !IsSelected;
            }

            base.OnClick();
        }

        /// <summary>
        /// Checks the click.
        /// </summary>
        internal void CheckClick()
        {
            this.OnClick();
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Creates AutomationPeer for ribbon button.
        /// </summary>
        /// <returns>
        /// An appropriate RibbonButtonAutomationPeer for this control as
        /// part of the WPF infrastructure.
        /// </returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RibbonButtonAutomationPeer(this);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseRightButtonUp"/>�routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the right mouse button was released.</param>
        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(e);

            Ribbon ribbon = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
            FrameworkElement fe = VisualUtils.FindRootVisual(this) as FrameworkElement;
            while (ribbon == null && fe != null && fe.GetType() == VisualUtils.RootPopupType)
            {
                Popup popup = fe.Parent as Popup;
                fe = popup.TemplatedParent as FrameworkElement;
                if (fe != null)
                {
                    if (fe is Ribbon)
                        ribbon = fe as Ribbon;
                    else
                        ribbon = VisualUtils.FindAncestor(fe, typeof(Ribbon)) as Ribbon;
                    fe = VisualUtils.FindRootVisual(fe) as FrameworkElement;
                }
            }

            if (ribbon != null)
            {
                RibbonContextMenu.CreateContextMenu(this);

                e.Handled = true;
            }

            
        }

         #if !SyncfusionFramework3_5
        protected override void OnTouchUp(TouchEventArgs e)
        {
            if (m_systemGesture == SystemGesture.RightTap)
            {
                Ribbon ribbon = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
                FrameworkElement fe = VisualUtils.FindRootVisual(this) as FrameworkElement;
                while (ribbon == null && fe != null && fe.GetType() == VisualUtils.RootPopupType)
                {
                    Popup popup = fe.Parent as Popup;
                    fe = popup.TemplatedParent as FrameworkElement;
                    if (fe != null)
                    {
                        if (fe is Ribbon)
                            ribbon = fe as Ribbon;
                        else
                            ribbon = VisualUtils.FindAncestor(fe, typeof(Ribbon)) as Ribbon;
                        fe = VisualUtils.FindRootVisual(fe) as FrameworkElement;
                    }
                }

                if (ribbon != null && ribbon.EnableTouch)
                {
                    RibbonContextMenu.CreateContextMenu(this);

                    e.Handled = true;
                }
                base.OnTouchUp(e);
            }
        }
        #endif

        protected override void OnStylusSystemGesture(StylusSystemGestureEventArgs e)
        {
            base.OnStylusSystemGesture(e);
            m_systemGesture = e.SystemGesture;
        }

        private void GetSynchronizedCommand()
        {
            string itemName = RibbonCommandManager.GetSynchronizedItem(this);
            Dictionary<string, FrameworkElement> syncItemColl = RibbonCommandManager.SynchronizedItemCollection;

            if (itemName != null && syncItemColl.ContainsKey(itemName))
            {
                FrameworkElement item = syncItemColl[itemName];
                if (this.Command == null)
                {
                    if (item is ButtonBase && (item as ButtonBase).Command != null && ((item is RibbonButton) && !(item as RibbonButton).IsMenuItem)
                        && (this is RibbonButton && (this as RibbonButton) != null && !(this as RibbonButton).IsMenuItem))
                        this.Command = (item as ButtonBase).Command;
                    else if (item is SplitButton && (item as SplitButton).Command == null)
                        this.Command = (item as SplitButton).Command;
                    else if (item is SplitMenuButton && (item as SplitMenuButton).Command == null)
                        this.Command = (item as SplitMenuButton).Command;
                }
            }
        }
        #endregion
       
    }
   
}

