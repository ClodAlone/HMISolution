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
using System.Collections.Generic;
using Syncfusion.Windows.Tools.Controls.Resources;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public static class RibbonCommandManager
    {
        /// <summary>
        /// Dictionary of (Command / Provider) pairs. 
        /// </summary>
        private static Dictionary<ICommand, RibbonCommandProvider> m_commandDictionary;

        /// <summary>
        /// Dictionary of (Group names / Command dictionary) pairs. 
        /// </summary>
        private static Dictionary<string, Dictionary<ICommand, RibbonCommandProvider>> m_groupDictionary;

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
            m_groupDictionary.Clear();
        }

        /// <summary>
        /// Unregisters connections for all <see cref="System.Windows.Input.ICommand" />
        /// objects.
        /// </summary>
        public static void UnregisterAll()
        {
            m_commandDictionary.Clear();
        }

        /// <summary>
        /// Initializes static members of the <see cref="RibbonCommandManager"/> class.
        /// </summary>
        static RibbonCommandManager()
        {
            m_commandDictionary = new Dictionary<ICommand, RibbonCommandProvider>();
            m_groupDictionary = new Dictionary<string, Dictionary<ICommand, RibbonCommandProvider>>();
            string allcommands = (new ResourceWrapper()).QATAllCommands;
            m_groupDictionary.Add(allcommands, null);
        }
        
    }
}
