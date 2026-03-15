// <copyright file="InfoForNewThreadForFilePath.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Collections.Generic;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <property name="flag" value="Finished" />
    /// <summary>
    /// This class creates new method which runs in new thread for
    /// async load items, where source is FilePath.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    internal class InfoForNewThreadForFilePath : Object
    {
        #region Constants

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This constant presents length start LAN text length (&quot;\\&quot;).
        /// </summary>
        private const int LANBeginTextLenght = 2;

        #endregion Constants

        #region Private members.

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This member contains a value indicating whether this level
        /// belongs to LAN part.
        /// </summary>
        private readonly bool m_IsLAN = false;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This is the delegate to call back method.
        /// </summary>
        /// Note
        /// Used for returning the value from the thread to the main
        /// thread.
        private readonly CallBackFromAsyncLoad m_CallBackFromNewTreadForFilePath;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This member contains current level.
        /// </summary>
        private readonly FilePathLevel m_CurrentLevel = null;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// If this level belong to LAN part then this member contains
        /// net-point.
        /// </summary>
        private readonly NetResource m_NetResource = new NetResource();

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// If this level's parent belong to LAN part then this member
        /// contains net-point.
        /// </summary>
        private readonly NetResource m_ParentNetResource = new NetResource();

        #endregion Private members.

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="InfoForNewThreadForFilePath"/> class.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="isLAN">if set to <c>true</c> [is LAN].</param>
        /// <param name="netResource">The net resource.</param>
        /// <param name="paretnNetResource">The parent net resource.</param>
        /// <param name="callBackFromNewTreadForFilePath">The call back from new tread for file path.</param>
        internal InfoForNewThreadForFilePath(FilePathLevel level, bool isLAN, NetResource netResource, NetResource paretnNetResource, CallBackFromAsyncLoad callBackFromNewTreadForFilePath)
        {
            m_CurrentLevel = level;
            m_IsLAN = isLAN;
            m_NetResource = netResource;
            m_ParentNetResource = paretnNetResource;
            m_CallBackFromNewTreadForFilePath = callBackFromNewTreadForFilePath;
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

            if (m_IsLAN)
            {
                if (null == m_CurrentLevel.Parent)
                {
                    List<NetResource> list = FilePathInfo.GetResources(m_NetResource);

                    for (int i = 0, cnt = list.Count; i < cnt; ++i)
                    {
                        items.Add(new FilePathLevel(m_CurrentLevel, list[i].RemoteName.Substring(LANBeginTextLenght), list[i]));
                    }
                }
                else if (ResourceDisplayType.DOMAIN == m_ParentNetResource.DisplayType)
                {
                    List<NetResource> list = FilePathInfo.GetResources(m_NetResource);
                    string fullPath = m_CurrentLevel.GetFullPath();

                    for (int i = 0, cnt = list.Count; i < cnt; ++i)
                    {
                        items.Add(new FilePathLevel(m_CurrentLevel, list[i].RemoteName.Substring(fullPath.Length), list[i]));
                    }
                }
                else
                {
                    items = FilePathInfo.GetDirectoryList(m_CurrentLevel);
                }
            }
            else
            {
                items = FilePathInfo.GetDirectoryList(m_CurrentLevel);
            }

            m_CallBackFromNewTreadForFilePath(items);
        }

        #endregion Implementation
    }
}