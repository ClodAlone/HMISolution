// <copyright file="RegistryLevel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Threading;
using System.Windows.Threading;
using Microsoft.Win32;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <property name="flag" value="Finished" />
    /// <summary>
    /// This class represents levels for auto-complete, where the
    /// source is Registry.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    internal class RegistryLevel : Object, IAutocompleteLevel
    {
        #region Constants

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This constant presents string value pipe.
        /// </summary>
        private const string StrPipe = @"\";

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This constant presents the name of the new thread for async
        /// load items.
        /// </summary>
        private const string ThreadName = "AutocompleteItemsAsyncLoaderThread";

        #endregion Constants

        #region Private members

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This member contains level's text.
        /// </summary>
        private readonly string m_Text = String.Empty;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This member contains RegistryKey for this level.
        /// </summary>
        private readonly RegistryKey m_RegistryKey = null;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This member contains level's parent.
        /// </summary>
        private readonly RegistryLevel m_Parent = null;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This member contains full way to level.
        /// </summary>
        private string m_fullPath = String.Empty;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This member indicates whether level's items were loaded.
        /// </summary>
        private bool m_isItemsLoaded = false;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This member contains the main thread.
        /// </summary>
        private Thread m_threadGUI = null;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This member contains level's items.
        /// </summary>
        private AutocompleteItemCollection m_items = new AutocompleteItemCollection();

        #endregion Private members

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryLevel"/> class.
        /// </summary>
        /// <param name="items">The items.</param>
        internal RegistryLevel(AutocompleteItemCollection items)
        {
            m_items = items;
            m_isItemsLoaded = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryLevel"/> class.
        /// </summary>
        /// <param name="registryKey">The registry key.</param>
        internal RegistryLevel(RegistryKey registryKey)
        {
            m_RegistryKey = registryKey;
            m_Text = registryKey.Name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryLevel"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="subName">Name of the sub.</param>
        internal RegistryLevel(RegistryLevel parent, string subName)
        {
            m_Parent = parent;
            m_Text = subName;

            try
            {
                m_RegistryKey = parent.m_RegistryKey.OpenSubKey(subName, false);
            }
            catch (SecurityException ex)
            {
                Debug.Print(ex.Message);
            }
        }

        #endregion Initialization

        #region Event

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Event that occurs when level's items were loaded.
        /// </summary>
        public event EventHandler ItemsLoaded;

        #endregion Event

        #region Properties

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets a value indicating whether the m_isItemsLoaded member.
        /// </summary>
        public bool IsItemsLoaded
        {
            get
            {
                return m_isItemsLoaded;
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets the value of the m_Splitter member.
        /// </summary>
        public string Splitter
        {
            get
            {
                return StrPipe;
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets the value of the m_items member.
        /// </summary>
        public AutocompleteItemCollection Items
        {
            get
            {
                return m_items;
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets the value of the m_Text member.
        /// </summary>
        public string Text
        {
            get
            {
                return m_Text;
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets the value of the m_Parent member.
        /// </summary>
        public IAutocompleteLevel Parent
        {
            get
            {
                return m_Parent;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is created correct.
        /// </summary>
        internal bool IsCreateedCorrect
        {
            get
            {
                return null != m_RegistryKey;
            }
        }

        #endregion Properties

        #region Public methods

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method loads items.
        /// </summary>
        public void LoadItems()
        {
            if (!IsItemsLoaded)
            {
                try
                {
                    int subKeyAmount = m_RegistryKey.SubKeyCount;
                    int valuesAmount = m_RegistryKey.ValueCount;

                    if (0 < subKeyAmount)
                    {
                        String[] subKeys = m_RegistryKey.GetSubKeyNames();

                        for (int i = 0; i < subKeyAmount; ++i)
                        {
                            RegistryLevel regLevel = new RegistryLevel(this, subKeys[i]);

                            if (regLevel.IsCreateedCorrect)
                            {
                                m_items.Add(regLevel);
                            }
                            else
                            {
                                Items.Add(new RegistryItem(subKeys[i]));
                            }
                        }
                    }

                    if (0 < valuesAmount)
                    {
                        String[] values = m_RegistryKey.GetValueNames();

                        for (int i = 0; i < valuesAmount; ++i)
                        {
                            m_items.Add(new RegistryItem(values[i]));
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

                OnItemsLoaded();
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method loads async items.
        /// </summary>
        public void LoadItemsAsync()
        {
            InfoForNewThreadForRegistry infoForNewTread = new InfoForNewThreadForRegistry(this, m_RegistryKey, new CallBackFromAsyncLoad(ResultCallBack));

            m_threadGUI = Thread.CurrentThread;

            Thread newThread = new Thread(new ThreadStart(infoForNewTread.LoadNewItems))
            {
                Name = ThreadName
            };
            newThread.Start();
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method gets full way to this item.
        /// </summary>
        /// <returns>
        /// Full way to this level.
        /// </returns>
        public string GetFullPath()
        {
            if (String.IsNullOrEmpty(m_fullPath))
            {
                string thisPath = String.Empty;

                if (!String.IsNullOrEmpty(m_Text))
                {
                    thisPath = m_Text + StrPipe;
                }

                if (null != m_Parent)
                {
                    m_fullPath = m_Parent.GetFullPath() + thisPath;
                }
                else
                {
                    m_fullPath = thisPath;
                }
            }

            return m_fullPath;
        }

        #endregion Public methods

        #region Impelemenation

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method assist return value from new thread, when async
        /// is loaded.
        /// </summary>
        /// <param name="items">The list of items which were loaded in
        /// different thread.</param>
        internal void ResultCallBack(AutocompleteItemCollection items)
        {
            ThreadStart callToOtherThread = delegate
            {
                m_items = items;
                OnItemsLoaded();
            };

            Dispatcher realDispatcher = Dispatcher.FromThread(m_threadGUI);

            if (realDispatcher != null)
            {
                realDispatcher.BeginInvoke(DispatcherPriority.Send, callToOtherThread);
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method raises ItemsLoaded event.
        /// </summary>
        private void OnItemsLoaded()
        {
            m_isItemsLoaded = true;

            if (null != ItemsLoaded)
            {
                ItemsLoaded(this, EventArgs.Empty);
            }
        }

        #endregion Impelemenation
    }
}