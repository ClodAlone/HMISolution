// <copyright file="KeyTip.cs" company="Syncfusion">
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
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <property name="flag" value="Finished" />
    /// <summary>
    /// This class encapsulates KeyTip information.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    internal class KeyTip
    {
        #region Private members

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Host element.
        /// </summary>
        private UIElement m_host;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// KeyTip text.
        /// </summary>
        private string m_text;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// KeyTip sublevel.
        /// </summary>
        private Dictionary<string,KeyTip> m_subLevel;

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Defines whether KeyTip is complex or not.
        /// </summary>
        private bool m_isComplex;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is complex.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is complex; otherwise, <c>false</c>.
        /// </value>
        public bool IsComplex
        {
            get
            {
                return m_isComplex;
            }

            set
            {
                m_isComplex = value;
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets or sets sublevel of key tips.
        /// </summary>
        public Dictionary<string,KeyTip> SubLevel
        {
            get
            {
                return m_subLevel;
            }

            set
            {
                m_subLevel = value;
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets or sets host element for KeyTip.
        /// </summary>
        public UIElement Host
        {
            get
            {
                return m_host;
            }

            set
            {
                m_host = value;
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets or sets KeyTip text.
        /// </summary>
        public string Text
        {
            get
            {
                return m_text;
            }

            set
            {
                m_text = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        public bool IsActive
        {
            get
            {
                if (m_host != null)
                {
                    if (m_host.IsVisible && m_host.IsEnabled)
                    {
                        if (m_host is RibbonBar && ((m_host as FrameworkElement).Parent is QuickAccessToolBar))
                        {
                            return true;
                        }
                        else if (m_host is RibbonBar)
                        {
                            if (IsCollapsedElement)
                                return true;

                            RibbonButton launcher = (m_host as RibbonBar).LauncherButton;
                            if (launcher.IsEnabled && launcher.IsVisible)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else if (m_host.IsEnabled && !m_host.IsVisible && !(m_host is RibbonBar))
                    {
                        RibbonBar bar = VisualUtils.FindSomeParent(m_host as FrameworkElement, typeof(RibbonBar)) as RibbonBar;
                        if (bar != null && bar.PanelState == RibbonBarState.Collapsed)
                        {
                            return true;
                        }
                        return false;
                    }
                    else
                    {
                        return false;
                    }
                }

                return false;
            }
        }

        internal bool IsCollapsedElement
        {
            get
            {
                if (Host != null)
                {
                    if (Host is RibbonBar && (Host as RibbonBar).PanelState == RibbonBarState.Collapsed)
                        return true;
                    else
                        return false;
                }
                return false;
            }
        }

        #endregion

        #region Static methods
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Defines whether the specified element has an attached KeyTip.
        /// </summary>
        /// <param name="el">element which is checking for KeyTip
        /// presence.</param>
        /// <returns>
        /// Returns true if KeyTip is present, otherwise false.
        /// </returns>
        public static bool HasKeyTip(UIElement el)
        {
            if (el == null)
            {
                ////throw new ArgumentNullException("el");
                return false;
            }

            
            string keyTip = Ribbon.GetKeyTip(el);
            return keyTip != null && keyTip != string.Empty;
        }


        /// <summary>
        /// Determines whether [has split menu key tip] [the specified el].
        /// </summary>
        /// <param name="el">The el.</param>
        /// <returns>
        /// 	<c>true</c> if [has split menu key tip] [the specified el]; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasSplitMenuKeyTip(UIElement el)
        {
            if (el == null)
            {
                //throw new ArgumentNullException("el");
                return false;
            }

            string keyTip = Ribbon.GetSplitMenuKeyTip(el);
            return keyTip != null && keyTip != string.Empty;
        }


        /// <summary>
        /// Gets the key tip.
        /// </summary>
        /// <param name="el">The el element.</param>
        /// <returns>GeyTip Value</returns>
        public static string GetKeyTip(UIElement el)
        {
            if (el == null)
            {
                throw new ArgumentNullException("el");
            }

            return Ribbon.GetKeyTip(el);
        }

        /// <summary>
        /// Gets the split menu key tip.
        /// </summary>
        /// <param name="el">The el.</param>
        /// <returns></returns>
        public static string GetSplitMenuKeyTip(UIElement el)
        {
            if (el == null)
            {
                throw new ArgumentNullException("el");
            }

            return Ribbon.GetSplitMenuKeyTip(el);
        }
        #endregion

        #region Initialize

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyTip"/> class..
        /// </summary>
        /// <param name="host">KeyTip host.</param>
        /// <param name="keyTip">KeyTip text.</param>
        public KeyTip(UIElement host, string keyTip)
        {
            m_host = host;
            m_text = keyTip;
            if (m_text.Length > 1)
            {
                m_isComplex = true;
            }
        }

        #endregion
    }
}
