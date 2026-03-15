// <copyright file="SimpleMenuButton.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;
using System.Windows.Threading;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents a SimpleMenuButton control.
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
    /// <example><code>public class SimpleMenuButton : ButtonBase</code></example>
    /// </list>
    /// <para/>
    /// <list type="table">
    /// <listheader>
    /// <description>XAML Object Element Usage</description>
    /// </listheader>
    /// <example><code><![CDATA[<ribbon:SimpleMenuButton Name="button" />]]></code></example>
    /// </list>
    /// </example>
    /// </list>
    /// <remarks>
    /// SimpleMenuButton class represents button control that can be places inside of ApplicationMenu control.
    /// </remarks>
    /// <example>
    /// <para/>This example shows how to create a ButtonPanel in XAML.
    /// <code>
    /// <![CDATA[
    /// <ribbon:SimpleMenuButton Label="Mark as Final" Description="Let readers know the document is final and make it read-only"  Icon="SampleImages/FinalMark32.png"/>
    /// ]]>
    /// </code>
    /// <para/>This example shows how to create a ButtonPanel in C#.
    /// <code>        
    /// SimpleMenuButton menuButton = new SimpleMenuButton();
    /// menuButton.Description = "Let readers know the document is final and make it read-only";
    /// menuButton.Label= "Mark as Final";
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class SimpleMenuButton : ButtonBase, IRibbonControl
    {
        #region Fileds
        /// <summary>
        /// represents the application menu
        /// </summary>
        ApplicationMenu appmenu = null;
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="SimpleMenuButton"/> class.
        /// </summary>
        static SimpleMenuButton()
        {
            EnvironmentTest.ValidateLicense(typeof(SimpleMenuButton));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SimpleMenuButton), new FrameworkPropertyMetadata(typeof(SimpleMenuButton)));
        }
        #endregion

        #region Dependency properties
        /// <summary>
        /// Defines the description of the SimpleMenuButton.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(SimpleMenuButton), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDescriptionChanged)));

        /// <summary>
        /// Defines the text that labels SimpleMenuButton.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(SimpleMenuButton), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnLabelChanged)));

        /// <summary>
        ///  Defines the icon that appears in SimpleMenuButton.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(ImageSource), typeof(SimpleMenuButton), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnIconChanged)));

        /// <summary>
        ///  Defines the iconsize that appears in SimpleMenuButton.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty IconSizeProperty = DependencyProperty.Register("IconSize", typeof(Size), typeof(SimpleMenuButton), new FrameworkPropertyMetadata(new Size(20d, 20d), new PropertyChangedCallback(OnIconSizeChanged)));
        
        // Using a DependencyProperty as the backing store for SmallIconProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SmallIconProperty =DependencyProperty.Register("SmallIcon", typeof(ImageSource), typeof(SimpleMenuButton), new FrameworkPropertyMetadata(null,new PropertyChangedCallback(OnSmallIconChanged)));

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the description of the SimpleMenuButton.
        /// </summary>
        /// <value>
        /// Type: <see cref="String"/>
        /// String value that will be displayed in the SimpleMenuButton.
        /// </value>
        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }

            set { SetValue(DescriptionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the text that labels SimpleMenuButton.
        /// </summary>
        /// <value>
        /// Type: <see cref="String"/>
        /// Text that labels the SimpleMenuButton. The default is empty string.
        /// </value>        
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
        /// Gets or sets the icon that appears in SimpleMenuButton.
        /// </summary>
        /// <remarks>
        /// Many controls have more than just text in the element. Often there is an icon. 
        /// </remarks>
        /// <seealso cref="ImageSource"/>
        public ImageSource Icon
        {
            get
            {
                return (ImageSource)GetValue(IconProperty);
            }

            set
            {
                SetValue(IconProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the size of the icon.
        /// </summary>
        /// <value>The size of the icon.</value>
        public Size IconSize
        {
            get
            {
                return (Size)GetValue(IconSizeProperty);
            }

            set
            {
                SetValue(IconSizeProperty, value);
            }
        }
        
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

        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when Description property is changed.
        /// </summary>
        public event PropertyChangedCallback DescriptionChanged;

        /// <summary>
        /// Event that is raised when Icon property is changed.
        /// </summary>
        public event PropertyChangedCallback IconChanged;

        /// <summary>
        /// Event that is raised when Iconsize property is changed.
        /// </summary>
        public event PropertyChangedCallback IconSizeChanged;

        /// <summary>
        /// Event that is raised when Label property is changed.
        /// </summary>
        public event PropertyChangedCallback LabelChanged;

        public event PropertyChangedCallback SmallIconChanged;

        #endregion

        #region Implementation
        /// <summary>
        /// Calls OnDescriptionChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnDescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SimpleMenuButton instance = (SimpleMenuButton)d;
            instance.OnDescriptionChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises DescriptionChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnDescriptionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DescriptionChanged != null)
            {
                DescriptionChanged(this, e);
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
            SimpleMenuButton instance = (SimpleMenuButton)d;
            instance.OnLabelChanged(e);
        }

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
        }

        /// <summary>
        /// Calls OnIconChanged method of the instance, notifies of the
        /// dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SimpleMenuButton instance = (SimpleMenuButton)d;
            instance.OnIconChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises IconChanged event.
        /// </summary>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        protected virtual void OnIconChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IconChanged != null)
            {
                IconChanged(this, e);
            }
        }

        /// <summary>
        /// Called when [icon size changed].
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIconSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SimpleMenuButton instance = (SimpleMenuButton)d;
            instance.OnIconSizeChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:IconSizeChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnIconSizeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IconSizeChanged != null)
            {
                IconSizeChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnLabelChanged method of the instance, notifies of the
        /// dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnSmallIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SimpleMenuButton instance = (SimpleMenuButton)d;
            instance.OnSmallIconChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises LabelChanged event.
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

        #endregion

        #region Overrides
        /// <summary>
        /// Creates AutomationPeer for ribbon button.
        /// </summary>
        /// <returns>
        /// An appropriate SimpleMenuButtonAutomationPeer for this control as
        /// part of the WPF infrastructure.
        /// </returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new SimpleMenuButtonAutomationPeer(this);
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            GetSynchronizedCommand();

            if (Command != null)
            {
                if (RibbonCommandManager.CommandDictionary.ContainsKey(Command))
                {
                    if (RibbonCommandManager.CommandDictionary[Command].SmallIcon != null)
                    {
                        Icon = RibbonCommandManager.CommandDictionary[Command].SmallIcon;
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
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Controls.Primitives.ButtonBase.Click"/> routed event.
        /// </summary>
        protected override void OnClick()
        {
            appmenu = (ApplicationMenu)VisualUtils.FindSomeParent(this, typeof(ApplicationMenu));

            if (appmenu != null && appmenu.AppMenuPopUp != null)
            {
                appmenu.AppMenuPopUp.Closed += new EventHandler(AppMenuPopUp_Closed);
            }
            else
            {
                base.OnClick();
            }
        }

        /// <summary>
        /// Checks the click.
        /// </summary>
        internal void CheckClick()
        {
            base.OnClick();
        }
        /// <summary>
        /// Handles the Closed event of the AppMenuPopUp control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void AppMenuPopUp_Closed(object sender, EventArgs e)
        {
            base.OnClick();
            appmenu.AppMenuPopUp.Closed -= new EventHandler(AppMenuPopUp_Closed);
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
                    if (item is ButtonBase && (item as ButtonBase).Command != null)
                        this.Command = (item as ButtonBase).Command;
                    else if (item is SplitButton && (item as SplitButton).Command == null)
                        this.Command = (item as SplitButton).Command;
                    else if (item is SplitMenuButton && (item as SplitMenuButton).Command == null)
                        this.Command = (item as SplitMenuButton).Command;
                }
            }
        }
        #endregion

        #region IRibbonControl Members

        

        #endregion

        
    }      
   
}
