#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System.Collections;
using System.Diagnostics;
using System.Drawing;

using Syncfusion.Documentation;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// This class acts as a repository for common styles (base styles). Such styles are registered and held in this repository.
    /// This enables them to be referenced by their registered names. When changes are made to registered base styles, they are
    /// propagated through the system.
    /// </summary>
    public class ChartBaseStylesMap
    {
        #region Constants
        private const int c_maxLevel = 16;
        internal const string StandardName = "Standard";
        #endregion

        #region Members
        private Hashtable m_table = new Hashtable();
        #endregion

        #region Properties
        /// <summary>
        /// Returns the ChartBaseStyleInfo object registered with the specified name.
        /// </summary>
        public ChartBaseStyleInfo this[string name]
        {
            get
            {
                return this.Lookup(name);
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor.
        /// </summary>
        public ChartBaseStylesMap()
        {
            this.Register(new ChartBaseStyleInfo(StandardName));
        }
        #endregion

        #region Implementation
        /// <summary>
        ///     Registers the specified base style with the styles map.
        /// </summary>
        /// <param name="style" type="Syncfusion.Windows.Forms.Chart.ChartBaseStyleInfo">
        ///     <para>
        ///     The style that is to be registered. The <see cref="ChartBaseStyleInfo.Name"/> property will be used as the registration name.
        ///     </para>
        /// </param>
        public void Register(ChartBaseStyleInfo style)
        {
            m_table[style.Name] = style;
            style.Identity = new ChartBaseStyleIdentity(this);
        }

        /// <summary>
        ///     Look ups and returns the base style with the specified name.
        /// </summary>
        /// <param name="name" type="string">
        ///     <para>
        ///     Name to look for.
        ///     </para>
        /// </param>
        /// <returns>
        ///     A base style if look up is successful; NULL otherwise.
        /// </returns>
        public ChartBaseStyleInfo Lookup(string name)
        {
            if (m_table.ContainsKey(name))
            {
                ChartBaseStyleInfo style = m_table[name] as ChartBaseStyleInfo;

                if (style != null)
                {
                    style.Identity = new ChartBaseStyleIdentity(this);
                }

                return style;
            }

            return null;
        }

        /// <summary>
        ///    Removes the base style registered under the specified name from this repository.
        /// </summary>
        /// <param name="name" type="string">
        ///     <para>
        ///      Name of base style to remove.
        ///     </para>
        /// </param>
        public void Remove(string name)
        {
            m_table.Remove(name);
        }

        /// <summary>
        ///  Remove references to all registered styles.
        /// </summary>
        public void Clear()
        {
            m_table.Clear();
        }

        /// <summary>
        /// Gets the base styles.
        /// </summary>
        /// <param name="styleInfo">The style info.</param>
        /// <returns>Returns ChartStyleInfo array.</returns>
        /// <internalonly/>
        [DocumentationExclude()]
        internal ChartStyleInfo[] GetBaseStyles(ChartStyleInfo styleInfo)
        {
            ArrayList baseStyles = new ArrayList();

            while (styleInfo.HasBaseStyle)
            {
                styleInfo = this.Lookup(styleInfo.BaseStyle);
                baseStyles.Add(styleInfo);
            }

            baseStyles.Add(this.Lookup(StandardName));

            return baseStyles.ToArray(typeof(ChartStyleInfo)) as ChartStyleInfo[];
        }

        /// <summary>
        /// Gets the sub base styles.
        /// </summary>
        /// <param name="styleInfo">The style info.</param>
        /// <param name="baseStyleInfo">The base style info.</param>
        /// <returns>Returns ChartStyleInfo array.</returns>
        /// <internalonly/>
        [DocumentationExclude()]
        internal ChartStyleInfo[] GetSubBaseStyles(ChartStyleInfo styleInfo, ChartStyleInfo baseStyleInfo)
        {
            ArrayList baseStyles = new ArrayList();

            while (styleInfo.HasBaseStyle)
            {
                styleInfo = this.Lookup(styleInfo.BaseStyle);
                baseStyles.Add(styleInfo);
            }

            baseStyles.Add(baseStyleInfo);
            baseStyles.AddRange(GetBaseStyles(baseStyleInfo));

            return baseStyles.ToArray(typeof(ChartStyleInfo)) as ChartStyleInfo[];
        }

        /// <summary>
        /// Gets the sub base styles.
        /// </summary>
        /// <param name="styleInfo">The style info.</param>
        /// <param name="styles">The styles.</param>
        /// <returns>Returns ChartStyleInfo array.</returns>
        /// <internalonly/>
        [DocumentationExclude()]
        internal ChartStyleInfo[] GetSubBaseStyles(ChartStyleInfo styleInfo, ChartStyleInfo[] styles)
        {
            ArrayList baseStyles = new ArrayList();

            while (styleInfo.HasBaseStyle)
            {
                styleInfo = this.Lookup(styleInfo.BaseStyle);
                baseStyles.Add(styleInfo);
            }

            for (int i = 0; i < styles.Length; i++)
            {
                styleInfo = styles[i];

                baseStyles.Add(styleInfo);

                while (styleInfo.HasBaseStyle)
                {
                    styleInfo = this.Lookup(styleInfo.BaseStyle);
                    baseStyles.Add(styleInfo);
                }
            }

            baseStyles.Add(this.Lookup(StandardName));

            return baseStyles.ToArray(typeof(ChartStyleInfo)) as ChartStyleInfo[];
        }
        #endregion
    }
}