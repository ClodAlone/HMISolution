// <copyright file="FilePathLevel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Threading;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <property name="flag" value="Finished" />
    /// <summary>
    /// This class represents levels for auto-complete, where the
    /// source is FilePath.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    internal class FilePathLevel : DispatcherObject, IAutocompleteLevel
    {
        #region Constants

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This constant presents length start LAN text length (&quot;\\&quot;).
        /// </summary>
        private const int LANBeginTextLenght = 2;

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
        /// This member contains level's splitter.
        /// </summary>
        private readonly string m_Splitter = String.Empty;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This member contains a value indicating whether this level
        /// belongs to LAN part.
        /// </summary>
        private readonly bool m_IsLAN = false;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This member contains level's parent.
        /// </summary>
        private readonly FilePathLevel m_Parent = null;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// If this level belongs to LAN part then this member contains
        /// net-point.
        /// </summary>
        private NetResource m_netResource;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Specifies whether net-point has been resolved.
        /// </summary>
        private bool m_bNetResourceLoaded = false;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This member contains the full way to level.
        /// </summary>
        private string m_fullPath = String.Empty;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This member indicates whether level's items were loaded.
        /// </summary>
        private bool m_isItemsLoaded = false;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This member contains main thread.
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
        /// Initializes a new instance of the <see cref="FilePathLevel"/> class.
        /// </summary>
        /// <param name="text">The text value.</param>
        /// <param name="splitter">The splitter value.</param>
        internal FilePathLevel(string text, string splitter)
        {
            if (String.IsNullOrEmpty(text))
            {
                m_IsLAN = true;
                m_bNetResourceLoaded = false;
            }
            else
            {
                m_bNetResourceLoaded = true;
                m_netResource = new NetResource();
                m_Text = text;
            }

            m_Splitter = splitter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilePathLevel"/> class.
        /// </summary>
        /// <param name="parent">The parent value.</param>
        /// <param name="text">The text value.</param>
        /// <param name="netRosource">The net resource value.</param>
        internal FilePathLevel(FilePathLevel parent, string text, NetResource netRosource)
            : this(parent, text)
        {
            m_netResource = netRosource;
            m_bNetResourceLoaded = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilePathLevel"/> class.
        /// </summary>
        /// <param name="parent">The parent level.</param>
        /// <param name="text">The text value.</param>
        internal FilePathLevel(FilePathLevel parent, string text)
        {
            m_Parent = parent;
            m_Text = text;
            m_IsLAN = m_Parent.m_IsLAN;
            m_Splitter = StrPipe;
            m_netResource = new NetResource();
            m_bNetResourceLoaded = true;
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

        /// <summary>
        /// Gets a value indicating whether for Item loaded Returns true if items are already loaded, otherwise, false. This property indicates whether the level items were loaded.
        /// </summary>
        /// <value></value>
        /// <property name="flag" value="Finished"/>
        public bool IsItemsLoaded
        {
            get
            {
                return m_isItemsLoaded;
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
        /// Gets the value of the m_Splitter member.
        /// </summary>
        public string Splitter
        {
            get
            {
                return m_Splitter;
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

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets the currently used network resource, initializes it if needed.
        /// </summary>
        private NetResource CurrentNetResource
        {
            get
            {
                if (!m_bNetResourceLoaded)
                {
                    InitializeNetResource();
                }

                return m_netResource;
            }
        }

        #endregion Properties

        #region Puclic method

        /// <summary>
        /// This method synchronously loads items.
        /// </summary>
        /// <property name="flag" value="Finished"/>
        public void LoadItems()
        {
            if (!IsItemsLoaded)
            {
                if (m_IsLAN)
                {
                    if (null == m_Parent)
                    {
                        List<NetResource> list = FilePathInfo.GetResources(CurrentNetResource);

                        for (int i = 0, cnt = list.Count; i < cnt; ++i)
                        {
                            m_items.Add(new FilePathLevel(this, list[i].RemoteName.Substring(LANBeginTextLenght), list[i]));
                        }
                    }
                    else if (ResourceDisplayType.DOMAIN == m_Parent.CurrentNetResource.DisplayType)
                    {
                        List<NetResource> list = FilePathInfo.GetResources(CurrentNetResource);
                        string fullPath = GetFullPath();

                        for (int i = 0, cnt = list.Count; i < cnt; ++i)
                        {
                            m_items.Add(new FilePathLevel(this, list[i].RemoteName.Substring(fullPath.Length), list[i]));
                        }
                    }
                    else
                    {
                        m_items = FilePathInfo.GetDirectoryList(this);
                    }
                }
                else
                {
                    m_items = FilePathInfo.GetDirectoryList(this);
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
            NetResource parentNetResource = (null != m_Parent) ? m_Parent.CurrentNetResource : new NetResource();

            InfoForNewThreadForFilePath info = new InfoForNewThreadForFilePath(this, m_IsLAN, CurrentNetResource, parentNetResource, new CallBackFromAsyncLoad(ResultCallBack));

            m_threadGUI = Thread.CurrentThread;

            Thread newThread = new Thread(new ThreadStart(info.LoadNewItems)) { Name = ThreadName };
            newThread.Start();
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method gets the full way to this item.
        /// </summary>
        /// <returns>
        /// Full way to this level.
        /// </returns>
        public string GetFullPath()
        {
            if (String.IsNullOrEmpty(m_fullPath))
            {
                if (null != m_Parent)
                {
                    m_fullPath = m_Parent.GetFullPath() + m_Text + m_Splitter;
                }
                else
                {
                    m_fullPath = m_Text + m_Splitter;
                }
            }

            return m_fullPath;
        }

        #endregion Puclic method

        #region Implementation

        /// <summary>
        /// Initializes the net resource.
        /// </summary>
        protected void InitializeNetResource()
        {
            List<NetResource> domain = FilePathInfo.GetDomain(new NetResource(), new List<NetResource>());
            string currentWorkGroup = FilePathInfo.GetCurrentWorkGroup();

            for (int i = 0, cnt = domain.Count; i < cnt; ++i)
            {
                if (currentWorkGroup == domain[i].RemoteName)
                {
                    m_netResource = domain[i];
                    break;
                }
            }

            m_bNetResourceLoaded = true;
        }

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

        #endregion Implementation
    }
}