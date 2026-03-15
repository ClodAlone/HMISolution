// <copyright file="RibbonCommandManager.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Syncfusion.Windows.Tools.Controls.Resources;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Registers connection between command and their <see cref="Syncfusion.Windows.Tools.Controls.RibbonCommandProvider"/>.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public static class RibbonCommandManager
    {

        #region Private members

        /// <summary>
        /// refers ResourceDictionary instance
        /// </summary>
        static ResourceWrapper lang = new ResourceWrapper();

        /// <summary>
        /// Dictionary of (Command / Provider) pairs. 
        /// </summary>
        private static Dictionary<ICommand, RibbonCommandProvider> m_commandDictionary;

        /// <summary>
        /// Dictionary of (Group names / Command dictionary) pairs. 
        /// </summary>
        private static Dictionary<string, Dictionary<ICommand, RibbonCommandProvider>> m_groupDictionary;

        /// <summary>
        /// Dictionary of (string / Frameworkelement) pairs.
        /// </summary>
        private static Dictionary<string, FrameworkElement> m_synchronizedItemCollection;

        /// <summary>
        /// QATMenuItem instance.
        /// </summary>
        internal static FrameworkElement qATMenuItem;

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the synchronized item collection.
        /// </summary>
        /// <value>The synchronized item collection.</value>
        internal static Dictionary<string,FrameworkElement> SynchronizedItemCollection
        {
            get
            {
                return m_synchronizedItemCollection;
            }
        }

        /// <summary>
        /// Gets or sets the QAT menu item.
        /// </summary>
        /// <value>The QAT menu item.</value>
        internal static FrameworkElement QATMenuItem
        {
            get 
            {
                return qATMenuItem;
            }
            set 
            {
                qATMenuItem = value;
            }
        }
     
        /// <summary>
        /// Gets the command dictionary.
        /// </summary>
        /// <value>The command dictionary.</value>
        public static Dictionary<ICommand, RibbonCommandProvider> CommandDictionary
        {
            get
            {
                return m_commandDictionary;
            }
        }

        /// <summary>
        /// Gets dictionary of (Group names / Command dictionary) pairs. 
        /// </summary>
        internal static Dictionary<string, Dictionary<ICommand, RibbonCommandProvider>> GroupDictionary
        {
            get
            {
                return m_groupDictionary;
            }
        }
        
        #endregion

        #region Attached Property

        /// <summary>
        /// Defines Synchronizing item for Ribbon controls.
        /// </summary>
        public static readonly DependencyProperty SynchronizedItemProperty =
            DependencyProperty.RegisterAttached("SynchronizedItem", typeof(string), typeof(RibbonCommandManager), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSynchronizedItemChanged), new CoerceValueCallback(CoerceOnSynchronisedItem)));

        #endregion

        #region DP Getters and Setters

        /// <summary>
        /// Gets the synchronized item.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static string GetSynchronizedItem(DependencyObject obj)
        {
            return (string)obj.GetValue(SynchronizedItemProperty);
        }

        /// <summary>
        /// Sets the synchronized item.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetSynchronizedItem(DependencyObject obj, string value)
        {
            obj.SetValue(SynchronizedItemProperty, value);
        }

        #endregion
      
        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="RibbonCommandManager"/> class.
        /// </summary>
        static RibbonCommandManager()
        {
            m_synchronizedItemCollection = new Dictionary<string, FrameworkElement>();
            m_commandDictionary = new Dictionary<ICommand, RibbonCommandProvider>();
            m_groupDictionary = new Dictionary<string, Dictionary<ICommand, RibbonCommandProvider>>();
            m_groupDictionary.Add(lang.QATAllCommandsCaption, null);
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Called when [synchronized item changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSynchronizedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            string itemname = (string)e.NewValue;
            FrameworkElement item = (FrameworkElement)d;

            if (!item.Equals(QATMenuItem) && !m_synchronizedItemCollection.ContainsKey(itemname))
            {
                m_synchronizedItemCollection.Add(itemname, item);
            }

            else if (!item.Equals(QATMenuItem))
            {
                if (item is ICommandSource)
                {
                    ICommandSource btn = item as ICommandSource;
                    if (btn.Command != null)
                    {
                        FrameworkElement syncitem = m_synchronizedItemCollection[itemname];
                        m_synchronizedItemCollection.Remove(itemname);
                        m_synchronizedItemCollection.Add(itemname, item);
                    }
                }
            }

            QATMenuItem = new FrameworkElement();
        }



        /// <summary>
        /// Coerces the on synchronised item.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="baseValue">The base value.</param>
        /// <returns></returns>
        private static object CoerceOnSynchronisedItem(DependencyObject d, object baseValue)
        {
            string val = (baseValue as string);

            if (val != null)
                return val.ToUpper();
            

            return baseValue;
        }
        /// <summary>
        /// Adds to sync collection.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="item">The item.</param>
        internal static void AddToSyncCollection(string name, FrameworkElement item)
        {
            if (!m_synchronizedItemCollection.ContainsKey(name))
            {
                m_synchronizedItemCollection.Add(name, item);
            }
        }

        /// <summary>
        /// Removes from sync collection.
        /// </summary>
        /// <param name="name">The name.</param>
        internal static void RemoveFromSyncCollection(string name)
        {
            if (m_synchronizedItemCollection.ContainsKey(name))
            {
                m_synchronizedItemCollection.Remove(name);
            }
        }

        /// <summary>
        /// Registers connection between <see cref="Syncfusion.Windows.Tools.Controls.RibbonCommandProvider"/>
        /// and specified <see cref="System.Windows.Input.ICommand"/>.
        /// </summary>
        /// <param name="command">The <see cref="System.Windows.Input.ICommand"/>
        /// that will receive the UI provider.</param>
        /// <param name="provider"><see cref="Syncfusion.Windows.Tools.Controls.RibbonCommandProvider"/>that
        /// will apply to the command.</param>
        public static void Register(ICommand command, RibbonCommandProvider provider)
        {
            string groupName = provider.GroupName;

            if (command == null)
            {
                throw new ArgumentNullException("command", "command can not be null");
            }
            else
            {
                provider.Command = command;
            }

            if ((provider.Label == null) && (command is RoutedUICommand))
            {
                provider.Label = ((RoutedUICommand)command).Text;
            }

            if (groupName != null && groupName != string.Empty)
            {
                if (!m_groupDictionary.ContainsKey(groupName))
                {
                    m_groupDictionary.Add(groupName, new Dictionary<ICommand, RibbonCommandProvider>());
                    m_groupDictionary[groupName].Add(command, provider);
                }
                else
                {
                    m_groupDictionary[groupName].Add(command, provider);
                }
            }

            m_commandDictionary.Add(command, provider);
        }

        /// <summary>
        /// Unregisters connection between UI provider and specified <see cref="System.Windows.Input.ICommand" />.
        /// </summary>
        /// <param name="command">The <see cref="System.Windows.Input.ICommand" />
        /// which needs to be unregistered.</param>                                          
        public static void Unregister(ICommand command)
        {
            m_commandDictionary.Remove(command);
        }

        /// <summary>
        /// Unregisters connections for all <see cref="System.Windows.Input.ICommand" />
        /// objects.
        /// </summary>
        public static void UnregisterAll()
        {
            m_groupDictionary.Clear();
            m_commandDictionary.Clear();
        }

        #endregion
    }
}
