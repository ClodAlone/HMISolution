// <copyright file="ScreenTip.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Diagnostics;
using Syncfusion.Windows.Shared;


namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Control that extends possibilities of standard tooltip.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ScreenTip : ToolTip
    {
        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="ScreenTip"/> class.
        /// </summary>
        static ScreenTip()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ScreenTip), new FrameworkPropertyMetadata(typeof(ScreenTip)));
            IsOpenProperty.OverrideMetadata(typeof(ScreenTip), new FrameworkPropertyMetadata(OnIsOpenChanged, CoerceIsOpen));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenTip"/> class.
        /// </summary>
        public ScreenTip()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the description of the <see cref="ScreenTip"/>.
        /// </summary>
        /// <value>
        /// Type: <see cref="String"/>
        /// Text that will be displayed as tooltip description, default is empty string.
        /// </value>
        /// <example>
        /// <code>
        /// ScreenTip screenTip;
        /// // ....
        /// screenTip.Description = "Description";
        /// </code>
        /// </example>
        /// <seealso cref="String"/>
        /// <seealso cref="ScreenTip"/>
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
        /// Gets or sets the help text.
        /// </summary>
        /// <value>The help text.</value>
        public string HelpText
        {
            get
            {
                return (string)GetValue(HelpTextProperty);
            }

            set
            {
                SetValue(HelpTextProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets the image that appears in a <see cref="ScreenTip"/>.
        /// </summary>
        /// <remarks>
        /// Many controls have more than just text in the element. Often there is an image. 
        /// </remarks>
        /// <example>
        /// <code>
        /// ScreenTip screenTip = new ScreenTip();
        /// // Create source.
        /// BitmapImage bimage = new BitmapImage();
        /// // BitmapImage.UriSource must be in a BeginInit/EndInit block.
        /// bimage.BeginInit();
        /// bimage.UriSource = new Uri(@"/sampleImages/sample.jpg",UriKind.RelativeOrAbsolute);
        /// bimage.EndInit();
        /// // Set the image source.
        /// screenTip.ImageSource = bimage;
        /// </code>
        /// </example>
        /// <seealso cref="ScreenTip"/>
        /// <seealso cref="ImageSource"/>
        public ImageSource ImageSource
        {
            get
            {
                return (ImageSource)GetValue(ImageSourceProperty);
            }

            set
            {
                SetValue(ImageSourceProperty, value);
            }
        }
        public ImageSource HelpIcon
        {
            get
            {
                return (ImageSource)GetValue(HelpIconProperty);
            }

            set
            {
                SetValue(HelpIconProperty, value);
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when Description property is changed.
        /// </summary>
        public event PropertyChangedCallback DescriptionChanged;

        /// <summary>
        /// Occurs when [help text changed].
        /// </summary>
        public event PropertyChangedCallback HelpTextChanged;

        /// <summary>
        /// Event that is raised when ImageSource property is changed.
        /// </summary>
        public event PropertyChangedCallback ImageSourceChanged;

        /// <summary>
        /// Event that is raised when HelpIamge property is changed.
        /// </summary>
        public event PropertyChangedCallback HelpImageChanged;

        /// <summary>
        /// Event that is raised when IsOpen property is changed.
        /// </summary>
        public event PropertyChangedCallback IsOpenChanged;
        #endregion

        #region Dependency properties
        /// <summary>
        /// Defines description of tooltip. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(ScreenTip), new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback(OnDescriptionChanged)));

        /// <summary>
        /// Represents the Help text, This is a Dependency property
        /// </summary>
        public static readonly DependencyProperty HelpTextProperty =
           DependencyProperty.Register("HelpText", typeof(string), typeof(ScreenTip), new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback(OnHelpTextChanged)));

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Defines ScreenTip image. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(ScreenTip), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnImageSourceChanged)));

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Defines HelpTextIcon. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty HelpIconProperty =
            DependencyProperty.Register("HelpIcon", typeof(ImageSource), typeof(ScreenTip), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnImageSourceChanged)));
        #endregion

        #region Implementation

        /// <summary>
        /// Calls OnDescriptionChanged method of the instance, notifies
        /// of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnDescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScreenTip instance = (ScreenTip)d;
            instance.OnDescriptionChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises DescriptionChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnDescriptionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DescriptionChanged != null)
            {
                DescriptionChanged(this, e);
            }
        }

        /// <summary>
        /// Called when [help text changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnHelpTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScreenTip instance = (ScreenTip)d;
            instance.OnHelpTextChanged(e);
               
        }

        /// <summary>
        /// Raises the <see cref="E:HelpTextChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnHelpTextChanged(DependencyPropertyChangedEventArgs e)
        {
            if (HelpTextChanged != null)
            {
                HelpTextChanged(this, e);
            }
        }
        /// <summary>
        /// Calls OnImageSourceChanged method of the instance, notifies
        /// of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScreenTip instance = (ScreenTip)d;
            instance.OnImageSourceChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises ImageSourceChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnImageSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ImageSourceChanged != null)
            {
                ImageSourceChanged(this, e);
            }
        }
        private static void OnHelpImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScreenTip instance = (ScreenTip)d;
            instance.OnHelpImageChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises ImageSourceChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnHelpImageChanged(DependencyPropertyChangedEventArgs e)
        {
            if (HelpImageChanged != null)
            {
                HelpImageChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnIsOpenChanged method of the instance, notifies
        /// of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScreenTip instance = (ScreenTip)d;
            instance.OnIsOpenChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises IsOpenChanged
        /// event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnIsOpenChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsOpenChanged != null)
            {
                IsOpenChanged(this, e);
            }
        }

        /// <summary>
        /// Coerces the is open.
        /// </summary>
        /// <param name="d">The d param value.</param>
        /// <param name="baseValue">The base value.</param>
        /// <returns>returns the base value</returns>
        private static object CoerceIsOpen(DependencyObject d, object baseValue)
        {
            ScreenTip screenTip = (ScreenTip)d;           

            if (screenTip.PlacementTarget != null && !((screenTip.PlacementTarget as FrameworkElement).Parent is QuickAccessToolBar) )
             {
                
                FrameworkElement element = screenTip.PlacementTarget as FrameworkElement;
                RibbonBar parent = null;
                bool verticalOffsetSet = false;


                parent = VisualUtils.FindAncestor(element, typeof(RibbonBar)) as RibbonBar;
                double offset = 0;
				
				if (element is RibbonGallery && (element as RibbonGallery).IsDropDownOpen)
                    return false; 
					
                if (parent != null)
                {
                    offset = StartPoint(parent).Y + parent.ActualHeight - (StartPoint(element).Y + element.ActualHeight) + 3;
                }
                else
                {
                    if (element is RibbonMenuItem)
                    {
                        bool fl = false;
                        ItemsControl elementItems = element.Parent as ItemsControl;
                        if (elementItems != null)
                        {
                            foreach (FrameworkElement elem in elementItems.Items)
                            {
                                if (elem.Equals(element))
                                {
                                    fl = true;
                                }
                                else if (fl)
                                {
                                    offset += elem.ActualHeight;
                                }
                            }
                        }
                    }
                    else
                    {
                        FrameworkElement currentElement = element as FrameworkElement;
                        if (currentElement is RibbonBar)
                        {
                            parent = currentElement as RibbonBar;
                        }
                        else
                        {
                            while (currentElement != null)
                            {
                                if (currentElement.Parent is RibbonBar)
                                {
                                    parent = currentElement.Parent as RibbonBar;
                                    break;
                                }
                                currentElement = currentElement.Parent as FrameworkElement;
                            }
                        }
                        double totalHeight = 0;
                        if (parent != null)
                        {
                            foreach (FrameworkElement elem in parent.Items)
                            {
                                if (elem is RibbonGallery && (elem as RibbonGallery).IsDropDownOpen)
                                    return baseValue;
                                if (totalHeight < (StartPoint(elem).Y + elem.ActualHeight))
                                {
                                    totalHeight = (StartPoint(elem).Y + elem.ActualHeight);
                                    if (elem is ButtonPanel)
                                    {
                                        totalHeight += 10;
                                    }
                                }
                            }
                        }
                        if (totalHeight != 0)
                            offset = totalHeight - (StartPoint(element).Y + element.ActualHeight) + 18;
                        else
                            offset = 18;
                    }
                }

                if (screenTip.Placement == PlacementMode.Mouse || screenTip.Placement == PlacementMode.Bottom)
                {
                    screenTip.Placement = PlacementMode.Bottom;
                    screenTip.VerticalOffset = offset;
                }

                if (screenTip.Placement == PlacementMode.Top)
                {
                    if (!verticalOffsetSet)
                        screenTip.VerticalOffset = -offset;
                }
                
            }
            else
                screenTip.VerticalOffset = 0;

            screenTip.FlowDirection = screenTip.PlacementTarget != null ? FrameworkElement.GetFlowDirection(screenTip.PlacementTarget) : FlowDirection.LeftToRight;
            return baseValue;
        }

        /// <summary>
        /// Starts the point.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        private static Point StartPoint(FrameworkElement element)
        {         
            Point startPoint = new Point(0, 0);
            if (element != null && element.IsVisible)
            {
                startPoint = element.PointToScreen(new Point(0, 0));
            }      
            return startPoint;
        }

        #endregion
    }
}
