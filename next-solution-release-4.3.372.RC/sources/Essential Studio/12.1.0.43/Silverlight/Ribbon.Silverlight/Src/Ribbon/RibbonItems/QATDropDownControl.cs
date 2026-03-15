#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using Syncfusion.Windows.Tools.Controls.Resources;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class QATDropDownControl : Control,IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public QATDropDownControl()
        {
            this.DefaultStyleKey = typeof(QATDropDownControl);
            
        }

        void QATDropDownControl_Loaded(object sender, RoutedEventArgs e)
        {            
            this.Unloaded += new RoutedEventHandler(QATDropDownControl_Unloaded);
        }

        void QATDropDownControl_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }

        private RibbonMenuItem PART_MoreCommands;
        private RibbonMenuItem PART_ShowBelowtheRibbon;
        internal RibbonMenuItem PART_MinimizeRibbon;
        private RibbonMenuGroup PART_RibbonMenuGroup;
        internal RibbonDropDown _dropdown;
        private Grid PART_ToggleButton;
        internal ResourceWrapper ResourceWrapperKeys;

        /// <summary>
        /// Gets or sets the QAT button caption.
        /// </summary>
        /// <value>The QAT button caption.</value>
        public string QATButtonCaption
        {
            get { return (string)GetValue(QATButtonCaptionProperty); }
            set { SetValue(QATButtonCaptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for QATButtonCaption.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty QATButtonCaptionProperty =
            DependencyProperty.Register("QATButtonCaption", typeof(string), typeof(QATDropDownControl), new PropertyMetadata("Show Below the Ribbon"));

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
        {

            if (this.PART_ToggleButton != null)
            {
                this.PART_ToggleButton.MouseLeftButtonDown -= new MouseButtonEventHandler(PART_ToggleButton_MouseLeftButtonDown);
                this.PART_ToggleButton.MouseEnter -= new MouseEventHandler(PART_ToggleButton_MouseEnter);
                this.PART_ToggleButton.MouseLeave -= new MouseEventHandler(PART_ToggleButton_MouseLeave);
            }
            if (_dropdown != null)
            {
                _dropdown.IsOpenChanged -= new EventHandler(_dropdown_IsOpenChanged);
            }
            if (this.PART_MoreCommands != null)           
                this.PART_MoreCommands.Click -= new RoutedEventHandler(PART_MoreCommands_MouseLeftButtonUp);
            if (this.PART_ShowBelowtheRibbon != null)
                this.PART_ShowBelowtheRibbon.Click -= new RoutedEventHandler(PART_ShowBelowtheRibbon_MouseLeftButtonUp);
            if (this.PART_MinimizeRibbon != null)
                this.PART_MinimizeRibbon.Click -= new RoutedEventHandler(PART_MinimizeRibbon_MouseLeftButtonUp);

            if (ResourceWrapperKeys == null)
                ResourceWrapperKeys = new ResourceWrapper();

            this.PART_ToggleButton = this.GetTemplateChild("PART_ToggleButton") as Grid;
            this.PART_RibbonMenuGroup = this.GetTemplateChild("PART_RibbonMenuGroup") as RibbonMenuGroup;
            if (this.PART_RibbonMenuGroup != null)
                this.PART_RibbonMenuGroup.Header = ResourceWrapperKeys.CustomizeQuickAccessToolbar;
            this.PART_MoreCommands = this.GetTemplateChild("PART_MoreCommands") as RibbonMenuItem;
            if (this.PART_MoreCommands != null)
                this.PART_MoreCommands.Header = ResourceWrapperKeys.MoreCommands;    
            this.PART_ShowBelowtheRibbon = this.GetTemplateChild("PART_ShowBelowtheRibbon") as RibbonMenuItem;
            if (this.PART_ShowBelowtheRibbon != null)
                this.PART_ShowBelowtheRibbon.Header = ResourceWrapperKeys.QATShowBelow;
            this.PART_MinimizeRibbon = this.GetTemplateChild("PART_MinimizeRibbon") as RibbonMenuItem;
            if (this.PART_MinimizeRibbon != null)
                this.PART_MinimizeRibbon.Header = ResourceWrapperKeys.MinimizeRibbon;
            _dropdown = GetTemplateChild("PART_RibbonDropDown") as RibbonDropDown;
            this.Loaded +=new RoutedEventHandler(QATDropDownControl_Loaded);
            this.SubscribeClickEvents();
        }

        private void SubscribeClickEvents()
        {
            if (this.PART_MoreCommands != null)
                this.PART_MoreCommands.Click += new RoutedEventHandler(PART_MoreCommands_MouseLeftButtonUp);
            if (this.PART_ShowBelowtheRibbon != null)
                this.PART_ShowBelowtheRibbon.Click += new RoutedEventHandler(PART_ShowBelowtheRibbon_MouseLeftButtonUp);
            if (this.PART_MinimizeRibbon != null)
                this.PART_MinimizeRibbon.Click += new RoutedEventHandler(PART_MinimizeRibbon_MouseLeftButtonUp);
            if (this.PART_ToggleButton != null)
            {
                this.PART_ToggleButton.MouseLeftButtonDown += new MouseButtonEventHandler(PART_ToggleButton_MouseLeftButtonDown);
                this.PART_ToggleButton.MouseEnter += new MouseEventHandler(PART_ToggleButton_MouseEnter);
                this.PART_ToggleButton.MouseLeave += new MouseEventHandler(PART_ToggleButton_MouseLeave);
            }
            if (_dropdown != null)
            {
                _dropdown.IsOpenChanged += new EventHandler(_dropdown_IsOpenChanged);
            }
        }

        void PART_ToggleButton_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_dropdown != null && !this._dropdown.IsOpen)
                VisualStateManager.GoToState(this, "Normal", true);
        }

        void PART_ToggleButton_MouseEnter(object sender, MouseEventArgs e)
        {
            if (this._dropdown != null && !this._dropdown.IsOpen)
                VisualStateManager.GoToState(this, "MouseOver", true);
        }

        void PART_ToggleButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_dropdown != null)
            {
                _dropdown.IsOpen = (_dropdown.IsOpen == true) ? false : true;
            }
        }

        private void _dropdown_IsOpenChanged(object sender, EventArgs e)
        {
            if (_dropdown != null)
            {
                if (_dropdown.IsOpen)
                {
                    VisualStateManager.GoToState(this, "Pressed", false);
                }
                else
                {
                    VisualStateManager.GoToState(this, "Normal", false);
                }
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler OnMoreCommandsClick = delegate { };
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler OnShowBelowtheRibbonClick = delegate { };
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler OnMinimizeRibbonClick = delegate { };

        private void PART_MinimizeRibbon_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            OnMinimizeRibbonClick(sender, e);
        }

        private void PART_ShowBelowtheRibbon_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            OnShowBelowtheRibbonClick(sender, e);
        }

        private void PART_MoreCommands_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            OnMoreCommandsClick(sender, e);
        }


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            PART_MinimizeRibbon = null;
            PART_MoreCommands = null;
            PART_ShowBelowtheRibbon = null;
            PART_ToggleButton.Children.Clear();
            _dropdown = null;
            this.Loaded -= new RoutedEventHandler(QATDropDownControl_Loaded);
            this.Unloaded -= new RoutedEventHandler(QATDropDownControl_Unloaded);
        }
    }
}
