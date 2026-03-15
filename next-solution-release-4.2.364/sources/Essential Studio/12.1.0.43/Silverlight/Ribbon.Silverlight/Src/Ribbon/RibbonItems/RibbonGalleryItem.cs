#region Copyright
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

namespace Syncfusion.Windows.Tools.Controls 
{
	/// <summary>
	/// Represents ribbon galery's item.
	/// </summary>
    [TemplateVisualState(GroupName = "RibbonItemStates", Name = "Checked")]
    [TemplateVisualState(GroupName = "RibbonItemStates", Name = "SelectChecked")]
    public class RibbonGalleryItem : RibbonItemBase, IRibbonControl
    {
        #region Constructor
        
        /// <summary>
		/// Initializes a new instance of the <see cref="RibbonGalleryItem"/> class.
		/// </summary>
		public RibbonGalleryItem()
		{
			this.DefaultStyleKey = typeof(RibbonGalleryItem);
        }

        #endregion

        #region Properties

        #region VisualState

        /// <summary>
        /// Gets the state of the visual.
        /// </summary>
        /// <value>The state of the visual.</value>
        internal override string VisualState
        {
            get
            {
                if (!this.MouseDown)
                {
                    if (this.Checked && !this.IsMouseOver)
                    {
                        return "Checked";
                    }
                    else if (this.Checked && this.IsMouseOver)
                    {
                        return "SelectChecked";
                    }
                }

                return base.VisualState;
            }
        }

        #endregion

        #region Checked

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RibbonGalleryItem"/> is checked.
        /// </summary>
        /// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
        internal bool Checked
        {
            get
            {
                return this.bChecked;
            }

            set
            {
                if (this.bChecked != value)
                {
                    this.bChecked = value;

                    this.UpdateVisualState();
                }
            }
        }

        #endregion

        #endregion

        #region Fields

        private bool bChecked = false;

        #endregion
    }
}
