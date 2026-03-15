#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syncfusion.Windows.Controls.Grid;

#if !SILVERLIGHT
using Syncfusion.PivotAnalysis.Base;
using System.Windows;
namespace Syncfusion.Windows.Controls.PivotGrid
#else
using Syncfusion.PivotAnalysis.Base.Silverlight;
using System.Windows;
namespace Syncfusion.Silverlight.Controls.PivotGrid
#endif
{
    /// <summary>
    /// Class that holds members for PivotGridStyleInfoIdentity
    /// </summary>
    public class PivotGridStyleInfoIdentity : GridStyleInfoIdentity
    {
        /// <summary>
        /// Contructor for PivotGridStyleInfoIdentity class
        /// </summary>
        /// <param name="baseStyleIdentity">GridSytle information identity</param>
        public PivotGridStyleInfoIdentity(GridStyleInfoIdentity baseStyleIdentity)
            : base(baseStyleIdentity)
        {

        }
        /// <summary>
        /// Gets or sets whether the group is expanded or not
        /// </summary>
        public bool IsExpanded { get; set; }
        /// <summary>
        /// Gets or sets the Pivot Cell information
        /// </summary>
        public PivotCellInfo PivotCellInfo { get; set; }
        /// <summary>
        /// Gets or sets the style of cell
        /// </summary>
        public Style Style { get; set; }
        /// <summary>
        /// Gets or sets whether the cell is hyperlink cell or not
        /// </summary>
        public bool IsHyperlinkCell { get; set; }
        /// <summary>
        /// Gets or sets whether contexMenu is enabled or not
        /// </summary>
        public bool EnableContextMenu { get; set; }
        /// <summary>
        /// Gets or sets whether ToolTip is enabled or not
        /// </summary>
        public bool ToolTipEnabled { get; set; }
    }
}
