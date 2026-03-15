// <copyright file="MenuButton.cs" company="Syncfusion">
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
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Automation.Peers;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents a MenuButton control.
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
    /// <example><code>public class MenuButton : MenuButtonBase</code></example>
    /// </list>
    /// <para/>
    /// <list type="table">
    /// <listheader>
    /// <description>XAML Object Element Usage</description>
    /// </listheader>
    /// <example><code><![CDATA[<ribbon:MenuButton Name="button" />]]></code></example>
    /// </list>
    /// </example>
    /// </list>
    /// <remarks>
    /// MenuButton class represents menu button control that can be used in ApplicationMenu control.
    /// </remarks>
    /// <example>
    /// <para/>This example shows how to create a MenuButton in XAML.
    /// <code>
    /// <![CDATA[
    /// <ribbon:MenuButton Label="Send" Icon="SampleImages/Send32.png">
    ///     <ribbon:ApplicationMenuGroup Header="Send a copy of the document">
    ///         <ribbon:SimpleMenuButton Label="E-mail" Description="Send a copy of the document in an e-mail message as an attachment."  Icon="SampleImages/Email.png"/>
    ///         <ribbon:SimpleMenuButton Label="Internet Fax" Description="Use an internet fax service to fax the document." Icon="SampleImages/Fax.png"/>
    ///     </ribbon:ApplicationMenuGroup>
    /// </ribbon:MenuButton>
    /// ]]>
    /// </code>
    /// <para/>This example shows how to create a ButtonPanel in C#.
    /// <code>    
    /// SimpleMenuButton button1;
    /// SimpleMenuButton button2;
    /// MenuButton menuButton = new MenuButton();
    /// menuButton.Items.Add(button1);
    /// menuButton.Items.Add(button2);
    /// </code>
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class MenuButton : MenuButtonBase
    {
        #region Properties
        /// <summary>
        /// Gets or sets the description of the MenuButton.
        /// </summary>
        /// <value>
        /// Type: <see cref="String"/>
        /// String value that will be displayed in the MenuButton.
        /// </value>
        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }
        #endregion

        #region Dependency properties
        /// <summary>
        /// Defines the description of the MenuButton.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(MenuButton), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDescriptionChanged)));
        #endregion

        #region Implementation
        /// <summary>
        /// Calls OnDescriptionChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnDescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MenuButton instance = (MenuButton)d;
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
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="MenuButton"/> class.
        /// </summary>
        static MenuButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MenuButton), new FrameworkPropertyMetadata(typeof(MenuButton)));
        }
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when Description property is changed.
        /// </summary>
        public event PropertyChangedCallback DescriptionChanged;
        #endregion

        #region Overrides

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new MenuButtonAutomationPeer(this);
        }
        
        #endregion
    }    
}
