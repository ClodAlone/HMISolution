// <copyright file="InfoForNewThreadForRegistry.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <property name="flag" value="Finished" />
    /// <summary>
    /// This class creates new method which runs in new thread for
    /// async load items, where source is Registry.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    internal class InfoForNewThreadForRegistry : Object
    {
        #region Private member

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This member contains current level.
        /// </summary>
        private readonly RegistryLevel m_CurrentLevel;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This member contains RegistryKey for current level.
        /// </summary>
        private readonly RegistryKey m_RegistryKey;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This is delegate to call back method.
        /// </summary>
        /// Note
        /// Used for returning the value from the thread to the main
        /// thread.
        private readonly CallBackFromAsyncLoad m_CallBackFromNewTread;

        #endregion Private member

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="InfoForNewThreadForRegistry"/> class.
        /// </summary>
        /// <param name="currentLevel">The current level.</param>
        /// <param name="registryKey">The registry key.</param>
        /// <param name="callBackFromNewTreadForFilePath">The call back from new tread for file path.</param>
        internal InfoForNewThreadForRegistry(RegistryLevel currentLevel, RegistryKey registryKey, CallBackFromAsyncLoad callBackFromNewTreadForFilePath)
        {
            m_CurrentLevel = currentLevel;
            m_RegistryKey = registryKey;
            m_CallBackFromNewTread = callBackFromNewTreadForFilePath;
        }

        #endregion Initialization

        #region Implementation

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method starts in new thread. Loads items for current
        /// level.
        /// </summary>
        internal void LoadNewItems()
        {
            AutocompleteItemCollection items = new AutocompleteItemCollection();

            try
            {
                int subKeyAmount = m_RegistryKey.SubKeyCount;
                int valuesAmount = m_RegistryKey.ValueCount;

                if (0 < subKeyAmount)
                {
                    String[] subKeys = m_RegistryKey.GetSubKeyNames();

                    for (int i = 0; i < subKeyAmount; ++i)
                    {
                        RegistryLevel regLevel = new RegistryLevel(m_CurrentLevel, subKeys[i]);

                        if (regLevel.IsCreateedCorrect)
                        {
                            items.Add(regLevel);
                        }
                        else
                        {
                            items.Add(new RegistryItem(subKeys[i]));
                        }
                    }
                }

                if (0 < valuesAmount)
                {
                    String[] values = m_RegistryKey.GetValueNames();

                    for (int i = 0; i < valuesAmount; ++i)
                    {
                        items.Add(new RegistryItem(values[i]));
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Debug.Print(ex.Message);
            }
            catch (IOException ex)
            {
                Debug.Print(ex.Message);
            }
            catch (ObjectDisposedException ex)
            {
                Debug.Print(ex.Message);
            }

            m_CallBackFromNewTread(items);
        }

        #endregion Implementation
    }
}