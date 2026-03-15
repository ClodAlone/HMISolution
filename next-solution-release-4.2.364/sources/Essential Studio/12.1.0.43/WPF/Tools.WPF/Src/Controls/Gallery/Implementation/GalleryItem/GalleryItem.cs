// <copyright file="GalleryItem.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Data;
using System.Reflection;
using System.ComponentModel;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Diagnostics;
using System.Windows.Interop;
using System.Threading;
using System.Windows.Documents;
using Syncfusion.Windows.Shared;
using Syncfusion.Licensing;
using System.Windows.Threading;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents gallery item class.
    /// </summary>
    /// <remarks>
    /// UI framework element based on
    /// <see cref="System.Windows.Controls.ContentControl"/> class. 
    /// Control is used for content hosting.
    /// </remarks>    
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
    /// <example><code>public partial class GalleryItem : ContentControl</code></example>
    /// </list>
    /// <para/>
    /// <list type="table">
    /// <listheader>
    /// <description>XAML Object Element Usage</description>
    /// </listheader>
    /// <example><code><![CDATA[<local:GalleryItem Name="galleryItem" />]]></code></example>
    /// </list>
    /// </example>
    /// </list>
    /// <example>
    /// <para/>This example shows how to create a GalleryItems in XAML.
    /// <code>
    /// <![CDATA[
    /// <Window x:Class="Gallery.Window1" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
    /// Title="Gallery" Height="300" Width="300">
    /// <StackPanel HorizontalAlignment="Center">
    ///     <local:Gallery Name="gallery">
    ///         <local:GalleryGroup>
    ///             <local:GalleryItem Name="item">
    ///             </local:GalleryItem>
    ///         </local:GalleryGroup>
    ///     </local:Gallery>
    /// </StackPanel>
    /// </Window>
    /// ]]>
    /// </code>
    /// <para/>This example shows how to create a GalleryItem in C#.
    /// <code>
    /// using System.Windows;
    /// using System.Windows.Controls;
    /// namespace Sample1
    /// {
    ///     public partial class Window1 : Window
    ///     {
    ///        public Window1()
    ///        {
    ///             InitializeComponent();
    ///             Gallery gallery = new Gallery();
    ///             stackPanel.Children.Add( gallery );
    ///             GalleryGroup group = new GalleryGroup();
    ///             gallery.Items.Add( group );
    ///             GalleryItem item = new GalleryItem();
    ///             group.Items.Add( item );
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Office2007Blue,
   Type = typeof(GalleryItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/Gallery/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Office2007Black,
    Type = typeof(GalleryItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/Gallery/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Office2007Silver,
    Type = typeof(GalleryItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/Gallery/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Office2003,
     Type = typeof(GalleryItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/Gallery/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.ShinyBlue,
     Type = typeof(GalleryItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/Gallery/Themes/ShinyBlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.ShinyRed,
     Type = typeof(GalleryItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/Gallery/Themes/ShinyRedStyle.xaml")]
    [SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.SyncOrange,
     Type = typeof(GalleryItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/Gallery/Themes/SyncOrangeStyle.xaml")]
    [SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Blend,
    Type = typeof(GalleryItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/Gallery/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Default,
    Type = typeof(GalleryItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/Gallery/Themes/Generic.xaml")]
    [SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Metro,
   Type = typeof(GalleryItem), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/Gallery/Themes/MetroStyle.xaml")]  
    public class GalleryItem : ContentControl
    {
        #region Private fields
        /// <summary>
        /// Used to indicate dragging.
        /// </summary>
        private Point m_startPoint;

        /// <summary>
        /// Used for drag-&amp;-drop.
        /// </summary>
        private DataObject m_dataObject;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="GalleryItem"/> class.
        /// Add new RoutedEvents and override PropertyMetadata.
        /// </summary>
        static GalleryItem()
        {
            //EnvironmentTest.ValidateLicense(typeof(GalleryItem));

            VisibilityProperty.OverrideMetadata(typeof(GalleryItem), new FrameworkPropertyMetadata(Visibility.Visible, new PropertyChangedCallback(OnVisibilityChanged)));

            // FocusableProperty.OverrideMetadata( typeof( GalleryItem ), new FrameworkPropertyMetadata( false ) );
            AllowDropProperty.OverrideMetadata(typeof(GalleryItem), new FrameworkPropertyMetadata(true));

            DefaultStyleKeyProperty.OverrideMetadata(typeof(GalleryItem), new FrameworkPropertyMetadata(typeof(GalleryItem)));

            IsSelectedChangedEvent = EventManager.RegisterRoutedEvent("IsSelectedChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GalleryItem));
            VisibilityChangedEvent = EventManager.RegisterRoutedEvent("VisibilityChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GalleryItem));
            DraggedEvent = EventManager.RegisterRoutedEvent("DraggedEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GalleryItem));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GalleryItem"/> class.
        /// </summary>
        public GalleryItem()
        {
            if (EnvironmentTestTools.IsSecurityGranted)
            {
                EnvironmentTestTools.StartValidateLicense(typeof(GalleryItem));
            }

            WrapPanelExt.SetRunningAnimation(this, WrapPanelExt.RunningAnimation.New);
            FocusVisualStyle = null;
        }
        #endregion

        #region	Events
        /// <summary>
        /// Event that is raised when <see cref="Caption"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback CaptionChanged;

        /// <summary>
        /// Event that is raised when <see cref="Width"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback WidthChanged;

        /// <summary>
        /// Event that is raised when <see cref="Height"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback HeightChanged;

        /// <summary>
        /// Event that is raised when <see cref="Description"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback DescriptionChanged;

        /// <summary>
        /// Event that is raised when <see cref="IsSelected"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback IsSelectedChanged;

        /// <summary>
        /// Event that is raised when <see cref="Visibility"/> property is changed
        /// </summary>
        public event PropertyChangedCallback VisibilityChanged;

        /// <summary>
        /// Event that is raised when <see cref="VisualMode"/> property is changed
        /// </summary>
        public event PropertyChangedCallback VisualModeChanged;

        /// <summary>
        /// Event that is raised when <see cref="HasFocus"/> property is changed
        /// </summary>
        public event PropertyChangedCallback HasFocusChanged;

        /// <summary>
        /// Event that is raised when <see cref="IsAlwaysShownCaption"/> property is changed
        /// </summary>
        public event PropertyChangedCallback IsAlwaysShownCaptionChanged;
        #endregion

        #region DP getters & setters
        /// <summary>
        /// Gets or sets the string value that represents caption of item. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="String"/>
        /// <para/>
        /// Default value is empty string.
        /// </value>
        public string Caption
        {
            get
            {
                return (string)GetValue(CaptionProperty);
            }

            set
            {
                SetValue(CaptionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the string value that represents description of item. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="String"/>
        /// <para/>
        /// Default value is empty string.
        /// </value>
        public string Description
        {
            get
            {
                return (string)GetValue(DescriptionProperty);
            }

            set
            {
                SetValue(DescriptionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether some other item is dragging over it. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// <para/>
        /// True if another item is dragging over it, otherwise false.
        /// </value>
        public bool IsDragOver
        {
            get
            {
                return (bool)GetValue(IsDragOverProperty);
            }

            protected internal set
            {
                SetValue(IsDragOverProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the item is selected. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// <para/>
        /// True if item is selected, otherwise false.
        /// </value>
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
        /// Gets or sets a value indicating whether the item has focus. 
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// <para/>
        /// True if item has focus, otherwise false.
        /// </value>        
        public bool HasFocus
        {
            get
            {
                return (bool)GetValue(HasFocusProperty);
            }

            set
            {
                SetValue(HasFocusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents visual mode of item. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="GalleryVisualMode"/>
        /// <para/>
        /// Default value is Standard.
        /// </value>
        /// <seealso cref="GalleryVisualMode"/> enum.
        public GalleryVisualMode VisualMode
        {
            get
            {
                return (GalleryVisualMode)GetValue(VisualModeProperty);
            }

            set
            {
                SetValue(VisualModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the caption and description always should be displayed. 
        /// This is dependency property.
        /// <value>
        /// Type: <see cref="bool"/>
        /// <para/>
        /// Default value is False.
        /// </value>
        /// </summary>
        public bool IsAlwaysShownCaption
        {
            get
            {
                return (bool)GetValue(IsAlwaysShownCaptionProperty);
            }

            set
            {
                SetValue(IsAlwaysShownCaptionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents alignment of caption in gallery item. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="CaptionAlignment"/>
        /// <para/>
        /// Default value is Top.
        /// </value>
        /// <remarks>
        /// Alignment occurs only in Standard mode when <see cref="IsAlwaysShownCaption"/> is true. 
        /// It doesn't affect captions in Detailed mode.
        /// </remarks>
        /// <seealso cref="GalleryVisualMode"/>
        public CaptionAlignment CaptionAlignment
        {
            get
            {
                return (CaptionAlignment)GetValue(GalleryGroup.CaptionAlignmentProperty);
            }

            set
            {
                SetValue(GalleryGroup.CaptionAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents alignment of description in gallery item. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="CaptionAlignment"/>
        /// <para/>
        /// Default value is Bottom.
        /// </value>
        /// <remarks>
        /// Alignment occurs only in Standard mode when <see cref="IsAlwaysShownCaption"/> is true. 
        /// It doesn't affect descriptions in Detailed mode.
        /// </remarks>
        /// <seealso cref="GalleryVisualMode"/>
        public CaptionAlignment DescriptionAlignment
        {
            get
            {
                return (CaptionAlignment)GetValue(GalleryGroup.DescriptionAlignmentProperty);
            }

            set
            {
                SetValue(GalleryGroup.DescriptionAlignmentProperty, value);
            }
        }
        #endregion

        #region Implementation

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        /// <summary>
        /// Called to remeasure a control.
        /// </summary>
        /// <param name="constraint">The maximum size that the method can return.</param>
        /// <returns>
        /// The size of the control, up to the maximum specified by <paramref name="constraint"/>.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            return base.MeasureOverride(constraint);
        }

        /// <summary>
        /// Called to arrange and size the content of a <see cref="T:System.Windows.Controls.Control"/> object.
        /// </summary>
        /// <param name="arrangeBounds">The computed size that is used to arrange the content.</param>
        /// <returns>The size of the control.</returns>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            return base.ArrangeOverride(arrangeBounds);
        }

        /// <summary>
        /// Invoked when parent of item is changed
        /// </summary>
        /// <param name="oldParent">Dependency object which represents
        /// old parent.</param>
        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);

            Type objClass = this.GetType();

            FieldInfo[] fields = objClass.GetFields(BindingFlags.Static | BindingFlags.Public);

            for (int i = 0, count = fields.Length; i < count; i++)
            {
                FieldInfo field = fields[i];

                if (field.FieldType == typeof(DependencyProperty))
                {
                    DependencyProperty dpField = field.GetValue(null) as DependencyProperty;

                    if (dpField != null)
                    {
                        Binding binding = BindingOperations.GetBinding(this, dpField);
                        if (binding != null)
                        {
                            BindingOperations.ClearBinding(this, dpField);
                            BindingOperations.SetBinding(this, dpField, binding);
                        }
                    }
                }
            }

            Binding bindingdata = BindingOperations.GetBinding(this, MarginProperty);
            if (bindingdata != null)
            {
                BindingOperations.ClearBinding(this, MarginProperty);
                BindingOperations.SetBinding(this, MarginProperty, bindingdata);
            }
        }

        /// <summary>
        /// Invoked when <see cref="E:System.Windows.Input.Mouse.MouseDown"/> event is raised.
        /// </summary>
        /// <param name="e">
        /// The instance that contains the event data.</param>               
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            GalleryGroup group = Parent as GalleryGroup;

            if (group == null)
            {
                group = GetParent();
            }
            Gallery gallery = (Gallery)Gallery.FindAncestor(typeof(Gallery), group);

            if (group != null && gallery.CanDragDrop && EnvironmentTest.IsSecurityGranted)
            {
                m_startPoint = e.GetPosition(FindRootContainer());
                m_dataObject = new DataObject("Item", this);
            }

            base.OnMouseDown(e);
        }

		/// <summary>
		/// Invoked when an unhandled System.Windows.Input.Mouse.MouseUp is raised.
		/// </summary>
		/// <param name="e">The MouseButtonEventArgs that contains the event data.</param>
		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			m_startPoint = new Point(0, 0);
			m_dataObject = null;
			base.OnMouseUp(e);
		}

        /// <summary>
        /// Invoked when <see cref="E:System.Windows.Input.Mouse.MouseMove"/> event is raised.
        /// </summary>
        /// <param name="e">
        /// The instance that contains the event data.</param>                
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPoint = e.GetPosition(FindRootContainer());
                if (Math.Abs(currentPoint.X - m_startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(currentPoint.Y - m_startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    if (m_dataObject != null)
                    {
                        List<GalleryItem> itemsList = Gallery.M_dragArray;
                        int arrayCount = itemsList.Count;
                        if (arrayCount == 0)
                        {
                            Opacity = 0.5;
                            HasFocus = false;
                        }
                        else
                        {
                            for (int i = 0; i < arrayCount; i++)
                            {
                                itemsList[i].Opacity = 0.5;
                                itemsList[i].HasFocus = false;
                            }
                        }

						DragDrop.DoDragDrop(this, m_dataObject, DragDropEffects.Move);
						m_dataObject = null;
						m_startPoint = new Point(0, 0);
                    }
                }
            }

            e.Handled = true;
        }

        /// <summary>
        /// Invoked when <see cref="E:System.Windows.Input.Keyboard.GotKeyboardFocus"/> is raised.
        /// </summary>
        /// <param name="e">The instance that contains the event data.</param>                
        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);
            if (!e.NewFocus.GetType().Equals(typeof(System.Windows.Controls.TextBox)))
            {
                HasFocus = true;
            }
        }

        /// <summary>
        /// Invoked when <see cref="E:System.Windows.Input.Keyboard.LostKeyboardFocus"/> is raised.
        /// </summary>
        /// <param name="e">The instance that contains the event data.</param>               
        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);
            HasFocus = false;
        }

        /// <summary>
        /// Invoked when <see cref="E:System.Windows.DragDrop.QueryContinueDrag"/> is raised.
        /// </summary>
        /// <param name="e">
        /// The instance that contains the event data.</param>               
        protected override void OnQueryContinueDrag(QueryContinueDragEventArgs e)
        {
            if (e.KeyStates != DragDropKeyStates.LeftMouseButton || e.EscapePressed)
            {
                List<GalleryItem> itemsList = Gallery.M_dragArray;
                int arrayCount = itemsList.Count;
                if (arrayCount == 0)
                {
                    Opacity = 1;
                    IsSelected = !IsSelected;
                }
                else
                {
                    for (int i = 0; i < arrayCount; i++)
                    {
                        itemsList[i].Opacity = 1;
                        itemsList[i].IsSelected = !IsSelected;
                    }
                }

                WrapPanelExt.SetRunningAnimation(this, WrapPanelExt.RunningAnimation.Drag);

                RoutedEventArgs args = new RoutedEventArgs(DraggedEvent, this);
                RaiseEvent(args);
            }

            base.OnQueryContinueDrag(e);
        }

        /// <summary>
        /// Invoked when <see cref="E:System.Windows.DragDrop.DragEnter"/> is raised.
        /// </summary>
        /// <param name="e">
        /// The instance that contains the event data.</param>             
        protected override void OnDragEnter(DragEventArgs e)
        {
            if (e.Data.GetDataPresent("Item"))
            {
                IsDragOver = true;
            }

            this.OnDragOver(e);
        }

        /// <summary>
        /// Invoked when <see cref="E:System.Windows.DragDrop.DragLeave"/> is raised.
        /// </summary>
        /// <param name="e">
        /// The instance that contains the event data.</param>
        protected override void OnDragLeave(DragEventArgs e)
        {
            IsDragOver = false;

            base.OnDragLeave(e);
        }

        /// <summary>
        /// Calls OnCaptionChanged method of the instance, notifies of
        /// the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnCaptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GalleryItem instance = (GalleryItem)d;
            instance.OnCaptionChanged(e);
        }

        /// <summary>
        /// Called when [width changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GalleryItem instance = (GalleryItem)d;
            instance.OnWidthChanged(e);
        }

        /// <summary>
        /// Called when [Height changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GalleryItem instance = (GalleryItem)d;
            instance.OnHeightChanged(e);
        }

        /// <summary>
        /// Calls OnDescriptionChanged method of the instance, notifies
        /// of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnDescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GalleryItem instance = (GalleryItem)d;
            instance.OnDescriptionChanged(e);
        }

        /// <summary>
        /// Calls OnIsSelectedChanged method of the instance, notifies of
        /// the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GalleryItem instance = (GalleryItem)d;
            instance.OnIsSelectedChanged(e);
        }

        /// <summary>
        /// Calls OnVisibilityChanged method of the instance, notifies of
        /// the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GalleryItem instance = (GalleryItem)d;
            instance.OnVisibilityChanged(e);
        }

        /// <summary>
        /// Calls OnVisualModeChanged method of the instance, notifies of
        /// the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnVisualModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GalleryItem instance = (GalleryItem)d;
            instance.OnVisualModeChanged(e);
        }

        /// <summary>
        /// Calls OnIsAlwaysShownCaptionChanged method of the instance, notifies of
        /// the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value and new value.</param>
        private static void OnIsAlwaysShownCaptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GalleryItem instance = (GalleryItem)d;
            instance.OnIsAlwaysShownCaptionChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises CaptionChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private void OnCaptionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CaptionChanged != null)
            {
                CaptionChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:WidthChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (WidthChanged != null)
            {
                WidthChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:HeightChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (HeightChanged != null)
            {
                HeightChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises DescriptionChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private void OnDescriptionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DescriptionChanged != null)
            {
                DescriptionChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises IsSelectedChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private void OnIsSelectedChanged(DependencyPropertyChangedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs(IsSelectedChangedEvent, this);
            RaiseEvent(args);

            if (IsSelectedChanged != null)
            {
                IsSelectedChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="VisibilityChanged"/>
        /// event.
        /// </summary>
        /// <param name="e">        
        /// Property changes details, such as old value
        /// and new value.</param>
        private void OnVisibilityChanged(DependencyPropertyChangedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs(VisibilityChangedEvent, this);
            RaiseEvent(args);

            if (VisibilityChanged != null)
            {
                VisibilityChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnHasFocusChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnHasFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GalleryItem instance = (GalleryItem)d;
            instance.OnHasFocusChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="HasFocusChanged"/> event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnHasFocusChanged(DependencyPropertyChangedEventArgs e)
        {
            if (HasFocus)
            {
                Keyboard.Focus(this);
            }

            if (HasFocusChanged != null)
            {
                HasFocusChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="VisualModeChanged"/>
        /// event.
        /// </summary>
        /// <param name="e">        
        /// Property changes details, such as old value
        /// and new value.</param>
        private void OnVisualModeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (VisualModeChanged != null)
            {
                VisualModeChanged(this, e);
            }
        }

        /// <summary>
        /// Raises <see cref="IsAlwaysShownCaptionChanged"/> event.
        /// </summary>
        /// <param name="e">        
        /// Property changes details, such as old value and new value.</param>
        private void OnIsAlwaysShownCaptionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsAlwaysShownCaptionChanged != null)
            {
                IsAlwaysShownCaptionChanged(this, e);
            }
        }

        /// <summary>
        /// Used for drag-and-drop.Instance of the
        /// FrameworkElement class.
        /// </summary>
        /// <returns>
        /// Top container where item is located.
        /// </returns>       
        private IInputElement FindRootContainer()
        {
            FrameworkElement frameworkElement = null;

            if (Parent != null)
            {
                frameworkElement = (FrameworkElement)Parent;
            }
            else
            {
                frameworkElement = GetParent();
            }

            while (frameworkElement.Parent != null)
            {
                frameworkElement = (FrameworkElement)frameworkElement.Parent;
            }

            return (IInputElement)frameworkElement;
        }

        /// <summary>
        /// Gets parent.
        /// </summary>
        /// <returns>
        /// GalleryGroup class instance.
        /// </returns>
        internal GalleryGroup GetParent()
        {
            return (GalleryGroup)Gallery.FindAncestor(typeof(GalleryGroup), this);
        }

        #endregion

        #region	Dependency properties

        /// <summary>
        /// Identifies <see cref="Width"/> dependency property. 
        /// </summary>
        //SU I78477
        /*public static readonly DependencyProperty WidthProperty =
            DependencyProperty.RegisterAttached("Width", typeof(double), typeof(double), new FrameworkPropertyMetadata(Double.NaN,new PropertyChangedCallback(OnWidthChanged)));*/
        public new static readonly DependencyProperty WidthProperty =
            DependencyProperty.RegisterAttached("Width", typeof(double), typeof(double), new FrameworkPropertyMetadata(Double.NaN, new PropertyChangedCallback(OnWidthChanged)));

        /// <summary>
        /// Identifies <see cref="Height"/> dependency property. 
        /// </summary>
        //SU I78477
        /*public static readonly DependencyProperty HeightProperty =
            DependencyProperty.RegisterAttached("Height", typeof(double), typeof(double), new FrameworkPropertyMetadata(Double.NaN, new PropertyChangedCallback(OnHeightChanged)));*/         
        public new static readonly DependencyProperty HeightProperty =
            DependencyProperty.RegisterAttached("Height", typeof(double), typeof(double), new FrameworkPropertyMetadata(Double.NaN, new PropertyChangedCallback(OnHeightChanged)));

        /// <summary>
        /// Identifies <see cref="Caption"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register("Caption", typeof(string), typeof(GalleryItem), new FrameworkPropertyMetadata("Caption", new PropertyChangedCallback(OnCaptionChanged)));

        /// <summary>
        /// Identifies <see cref="Description"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(GalleryItem), new FrameworkPropertyMetadata("Description", new PropertyChangedCallback(OnDescriptionChanged)));

        /// <summary>
        /// Identifies <see cref="IsDragOver"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty IsDragOverProperty =
            DependencyProperty.Register("IsDragOver", typeof(bool), typeof(GalleryItem), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Identifies <see cref="IsSelected"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(GalleryItem), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsSelectedChanged)));

        /// <summary>
        /// Identifies <see cref="HasFocus"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty HasFocusProperty =
            DependencyProperty.Register("HasFocus", typeof(bool), typeof(GalleryItem), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnHasFocusChanged)));

        /// <summary>
        /// Identifies <see cref="VisualMode"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty VisualModeProperty =
            DependencyProperty.Register("VisualMode", typeof(GalleryVisualMode), typeof(GalleryItem), new FrameworkPropertyMetadata(GalleryVisualMode.Standard, new PropertyChangedCallback(OnVisualModeChanged)));

        /// <summary>
        /// Identifies <see cref="IsAlwaysShownCaption"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty IsAlwaysShownCaptionProperty =
          DependencyProperty.Register("IsAlwaysShownCaption", typeof(bool), typeof(GalleryItem), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsAlwaysShownCaptionChanged)));
        #endregion

        #region Routed events
        /// <summary>
        /// Occurs when <see cref="IsSelected"/> property is changed.
        /// </summary>
        public static readonly RoutedEvent IsSelectedChangedEvent;

        /// <summary>
        /// Occurs when <see cref="E:System.Windows.UIElement.Visibility"/> property is changed.
        /// </summary>
        public static readonly RoutedEvent VisibilityChangedEvent;

        /// <summary>
        /// Occurs when item is dragged.
        /// </summary>
        public static readonly RoutedEvent DraggedEvent;
        #endregion
    }
}
