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

using System;
using System.Diagnostics;

using Syncfusion.Documentation;
using Syncfusion.Styles;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <internalonly/>
    [DocumentationExclude()]
    internal class ChartBaseStyleIdentity : StyleInfoIdentityBase
    {
        private ChartBaseStylesMap styleInfoMap;

        /// <summary>
        /// Releases all resources used by the component.
        /// </summary>
        /// <override/>
        public override void Dispose()
        {
            styleInfoMap = null;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartBaseStyleIdentity"/> class.
        /// </summary>
        /// <param name="styleInfoMap">The style info map.</param>
        public ChartBaseStyleIdentity(ChartBaseStylesMap styleInfoMap)
        {
            this.styleInfoMap = styleInfoMap;
        }

        /// <summary>
        /// Gets the base styles map.
        /// </summary>
        /// <value>The base styles map.</value>
        public ChartBaseStylesMap BaseStylesMap
        {
            get
            {
                return styleInfoMap;
            }
        }

        /// <summary>
        /// Returns an array with base styles for the specified style object.
        /// </summary>
        /// <param name="thisStyleInfo">The style object.</param>
        /// <returns>
        /// An array of style objects that are base styles for the current style object.
        /// </returns>
        /// <override/>
        public override IStyleInfo[] GetBaseStyles(IStyleInfo thisStyleInfo)
        {
            if (styleInfoMap != null)
            {
                return styleInfoMap.GetBaseStyles(thisStyleInfo as ChartStyleInfo);
            }

            return null;
        }
    }

    /// <summary>
    /// BaseStyles are styles that are used to uniformly affect an arbitrary set of styles that they are applied to.
    /// BaseStyles are applied to a style by associating them with a style using its BaseStyle property (<see cref="ChartStyleInfo.BaseStyle"/>).
    /// </summary>
    public class ChartBaseStyleInfo : ChartStyleInfo
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name" type="string">
        ///     <para>
        ///     The name of this base style.
        ///     </para>
        /// </param>
        public ChartBaseStyleInfo(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Indicates whether this base style is a system registered base style. System registered base styles are
        /// preregistered by the charting style system and are needed for the proper functioning of Essential
        /// Chart.
        /// </summary>
        /// <value><c>true</c> if system; otherwise, <c>false</c>.</value>
        public bool System
        {
            get
            {
                return this._System;
            }

            set
            {
                this._System = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has local value of System property.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has local value of System property; otherwise, <c>false</c>.
        /// </value>
        /// <internalonly/>
        [DocumentationExclude()]
        public bool HasSystem
        {
            get
            {
                return this._HasSystem;
            }
        }

        /// <summary>
        /// Gets or sets the name for this base style object.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return this._Name;
            }

            set
            {
                this._Name = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has name.
        /// </summary>
        /// <value><c>true</c> if this instance has name; otherwise, <c>false</c>.</value>
        /// <internalonly/>
        [DocumentationExclude()]
        public bool HasName
        {
            get
            {
                return this._HasName;
            }
        }

        /// <summary>
        /// Gets  ChartBaseStylesMap object. Base styles are registered with and managed by a <see cref="ChartBaseStylesMap"/> object at the chart level.
        /// </summary>
        /// <value>The base styles map.</value>
        public ChartBaseStylesMap BaseStylesMap
        {
            get
            {
                ChartBaseStyleIdentity chartBaseStyleIdentity = this.Identity as ChartBaseStyleIdentity;

                if (chartBaseStyleIdentity != null)
                {
                    return chartBaseStyleIdentity.BaseStylesMap;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}