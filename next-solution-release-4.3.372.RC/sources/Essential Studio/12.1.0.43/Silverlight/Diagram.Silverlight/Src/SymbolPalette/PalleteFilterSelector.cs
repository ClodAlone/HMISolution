// <copyright file="PalleteFilterSelector.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;

namespace Syncfusion.Windows.Diagram
{
    /// <summary>
    /// Represents internal PalleteFilterSelector control.
    /// </summary>
#if !SyncfusionFramework3_5
    [DesignTimeVisible(false)]
#endif
    public class PalleteFilterSelector : Control
    {
        #region Fields
       
        /// <summary>
        /// Represents the drop down button.
        /// </summary>
        private FrameworkElement m_dropDownButton;
        
        /// <summary>
        /// Represents the popup
        /// </summary>
        private Popup m_popup;

        /// <summary>
        /// Is used for correct popup opening and closing.
        /// </summary>
        private Stack<object> m_stack = new Stack<object>();
       
        private TextBlock textblock;

        /// <summary>
        /// Drop down button border
        /// </summary>
        private Border dropdownborder;
        #endregion

        #region Dependency properties
      
        /// <summary>
        /// Defines the collection of filters.
        /// </summary>
        public static readonly DependencyProperty FiltersProperty =
            DependencyProperty.Register("Filters", typeof(ObservableCollection<SymbolPaletteFilter>), typeof(PalleteFilterSelector), new PropertyMetadata(null));
       
        /// <summary>
        /// Defines the selected filter.
        /// </summary>
        public static readonly DependencyProperty SelectedFilterProperty =
            DependencyProperty.Register("SelectedFilter", typeof(SymbolPaletteFilter), typeof(PalleteFilterSelector), new PropertyMetadata(null));
      
        /// <summary>
        /// Defines whether dropdown is open.
        /// </summary>
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(PalleteFilterSelector), new PropertyMetadata(false, new PropertyChangedCallback(OnIsDropDownOpenChanged)));

        public static readonly DependencyProperty FilterParentProperty =
      DependencyProperty.Register("FilterParent", typeof(SymbolPalette), typeof(PalleteFilterSelector), new PropertyMetadata(null));

        #endregion

        #region Properties
       
        /// <summary>
        /// Gets or sets the collection of filters.
        /// </summary>
        public ObservableCollection<SymbolPaletteFilter> Filters
        {
            get
            {
                return (ObservableCollection<SymbolPaletteFilter>)GetValue(FiltersProperty);
            }

            set
            {
                SetValue(FiltersProperty, value);
            }
        }
     
        /// <summary>
        /// Gets or sets the selected filter.
        /// </summary>
        public SymbolPaletteFilter SelectedFilter
        {
            get
            {
                return (SymbolPaletteFilter)GetValue(SelectedFilterProperty);
            }

            set
            {
                SetValue(SelectedFilterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is drop down open.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is drop down open; otherwise, <c>false</c>.
        /// </value>
        public bool IsDropDownOpen
        {
            get
            {
                return (bool)GetValue(IsDropDownOpenProperty);
            }

            set
            {
                SetValue(IsDropDownOpenProperty, value);
            }
        }

        internal SymbolPalette FilterParent
        {
            get
            {
                return (SymbolPalette)GetValue(FilterParentProperty);
            }

            set
            {
                SetValue(FilterParentProperty, value);
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="PalleteFilterSelector"/> class.
        /// </summary>
        static PalleteFilterSelector()
        {
#if WPF
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PalleteFilterSelector), new FrameworkPropertyMetadata(typeof(PalleteFilterSelector)));
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PalleteFilterSelector"/> class.
        /// </summary>
        public PalleteFilterSelector()
        {
#if SILVERLIGHT
            DefaultStyleKey = typeof(PalleteFilterSelector);
#endif
           
           // Mouse.AddPreviewMouseDownOutsideCapturedElementHandler(this, OnPreviewMouseDownOutsideCapturedElement);
        }
        #endregion

        #region Implementation
      
        /// <summary>
        /// Invoked whenever application code or internal processes call this OnApplyTemplate() method.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_dropDownButton = GetTemplateChild("PART_DropDownButton") as FrameworkElement;
            dropdownborder = GetTemplateChild("PART_DropDownBorder") as Border;


            if (m_dropDownButton != null)
            {
                m_dropDownButton.MouseLeftButtonDown += new MouseButtonEventHandler(DropDownButton_MouseDown);
#if WPF
                m_dropDownButton.MouseLeftButtonUp += new MouseButtonEventHandler(m_dropDownButton_MouseLeftButtonUp);
#endif
            }
#if WPF
            IsDropDownOpen = true;
#endif
            m_popup = GetTemplateChild("PART_Popup") as Popup;

            textblock = GetTemplateChild("PART_text") as TextBlock;
            if (m_popup != null)
            {
                m_popup.Opened += new EventHandler(m_popup_Opened);
                m_popup.Closed += new EventHandler(m_popup_Closed);
            }
            this.Loaded += new RoutedEventHandler(PalleteFilterSelector_Loaded);

            if (m_dropDownButton != null)
            {
                m_dropDownButton.MouseEnter += new MouseEventHandler(dropdownborder_MouseEnter);
                m_dropDownButton.MouseLeave += new MouseEventHandler(dropdownborder_MouseLeave);
                m_dropDownButton.MouseLeftButtonUp += new MouseButtonEventHandler(dropdownborder_MouseLeftButtonUp);
            }
        }

        void m_popup_Opened(object sender, EventArgs e)
        {
            if (IsDropDownOpen)
            {
                VisualStateManager.GoToState(this, "MouseOver", true);
            }
        }

        void dropdownborder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
#if WPF
            if (IsDropDownOpen)
            {
                this.m_popup.IsOpen = true;
            }
            IsDropDownOpen = false;
#endif
        }
        

        void dropdownborder_MouseLeave(object sender, MouseEventArgs e)
        {           
                if (textblock != null)
                {
                    textblock.Foreground = this.FilterParent.FilterSelectorForeground;
#if SILVERLIGHT
                    if (!IsDropDownOpen)
                    {
                        VisualStateManager.GoToState(this, "Normal", true);
                    }
#endif
                }            
        }

        void dropdownborder_MouseEnter(object sender, MouseEventArgs e)
        {
            if (textblock != null)
            {
                textblock.Foreground = this.FilterParent.FilterSelectorMouseOverForeground;
#if SILVERLIGHT           
                VisualStateManager.GoToState(this, "MouseOver", true);
#endif
            }
        }

        void m_popup_Closed(object sender, EventArgs e)
        {
#if WPF
            IsDropDownOpen = true;            
#endif
            
#if SILVERLIGHT
                VisualStateManager.GoToState(this, "Normal", true);
#endif            
        }
#if WPF  
        void m_dropDownButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
        }
#endif
        void PalleteFilterSelector_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement ele = (FrameworkElement)VisualTreeHelper.GetParent(this);

            this.FilterParent = PalleteFilterSelector.GetSymbolPalette(ele);
        }

        internal static SymbolPalette GetSymbolPalette(FrameworkElement element)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(element);
            while (parent != null)
            {
                if (parent is SymbolPalette)
                {
                    return parent as SymbolPalette;
                }

                DependencyObject temp = VisualTreeHelper.GetParent(parent);
                if (temp == null && parent is FrameworkElement)
                {
                    parent = (parent as FrameworkElement).Parent;
                }
                else
                {
                    parent = temp;
                }
            }

            return null;
        }

        //private bool IsMouseOver=false;       

        /// <summary>
        /// Handles the MouseDown event of the m_dropDownButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void DropDownButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
#if SILVERLIGHT
            IsDropDownOpen = !IsDropDownOpen;
            
#endif
        }
              
        /// <summary>
        /// Calls OnIsDropDownOpenChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PalleteFilterSelector instance = (PalleteFilterSelector)d;
            instance.OnIsDropDownOpenChanged(e);
        }
       
        /// <summary>
        /// Updates property value cache and raises IsDropDownOpenChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnIsDropDownOpenChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsDropDownOpenChanged != null)
            {
                IsDropDownOpenChanged(this, e);
            }
        }
           
        /// <summary>
        /// Invoked when an unhandled Mouse.LostMouseCapture attached
        /// event reaches an element in its route that is derived from
        /// this class. Implement this method to add class handling for
        /// this event.
        /// </summary>
        /// <param name="e">TheMouseEventArgs that contains event data.</param>
        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            m_stack.Push(e.OriginalSource);
            base.OnLostMouseCapture(e);
        }
     
        /// <summary>
        /// Event that is raised when IsDropDownOpen property is changed.
        /// </summary>
        public event PropertyChangedCallback IsDropDownOpenChanged;
   
        #endregion
    }
}
