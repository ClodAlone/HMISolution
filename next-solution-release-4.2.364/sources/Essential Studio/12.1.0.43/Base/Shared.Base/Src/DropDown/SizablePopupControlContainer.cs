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
using System.Drawing;
using System.Windows.Forms;
using Syncfusion.Runtime.InteropServices;
using System.Drawing.Design;
using System.ComponentModel;
using System.Diagnostics;

namespace Syncfusion.Windows.Forms.Tools
{

	/// <summary>
	/// Modified PopupControlContainer that takes a child control and
	/// adds a sizable grip so that the window can be sized.
	/// </summary>
	[ToolboxItem(false)]
	public class SizablePopupControlContainer : PopupControlContainer
	{

		/// <summary>
		/// The child control that is to be embedded.
		/// </summary>
		private Control childControl;

		/// <summary>
		/// Indicates whether the container has already been initialized.
		/// </summary>
		private bool isInitialized;

        /// <summary>
        /// Indicates whether size is dependant of child control size.
        /// </summary>
        private bool m_bFitToChildSize = false;

		/// <summary>
		/// Creates an object of type SizablePopupControlContainer.
		/// </summary>
		/// <param name="childControl"></param>
		public SizablePopupControlContainer(Control childControl)
		{
			this.childControl = childControl;
			this.isInitialized = false;
		}

		/// <override/>
		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				this.childControl = null;
			}
			base.Dispose(disposing);
		}

        protected override PopupHost CreatePopupHost()
        {
            return new SizablePopupHost(this.childControl);
        }

		/// <summary>
		/// Displays the popup control.
		/// </summary>
		/// <param name="location">The location at which the popup's left top position will appear.</param>
		public override void ShowPopup(Point location)
		{
            this.EnsurePopupHost();

            SizablePopupHost spgPopupHost = this.PopupHost as SizablePopupHost;
            if (spgPopupHost != null)
            {
                spgPopupHost.ShowCloseButton = m_bShowCloseButton;
                spgPopupHost.ShowGripper = m_bShowGripper;

			    spgPopupHost.RightToLeft = GetIsMirrored() ? RightToLeft.Yes : RightToLeft.No;
			}

			base.ShowPopup(location);
		}

        /// <summary>
        /// Indicates whether size is dependant of child control size.
        /// </summary>
        public bool FitToChildControlSize
        {
            get
            {
                return m_bFitToChildSize;
            }
            set
            {
                m_bFitToChildSize = value;
            }
        }

		private bool GetIsMirrored()
		{
			bool bIsMirrored = false;

			Control ctrlParent = this.ParentControl;
			if (null != ctrlParent)
			{
				bIsMirrored = (RightToLeft.Yes == ctrlParent.RightToLeft);
			}

			return bIsMirrored;
		}

		/// <summary>
		/// Initializes the container control.
		/// </summary>
		public virtual void InitializeContainer()
		{
			if(this.isInitialized == false)
			{
				this.SuspendLayout();

				this.BorderStyle = System.Windows.Forms.BorderStyle.None;
				this.Location = new System.Drawing.Point(8, 16);
				this.Name = "dropDownContainer";
				this.Size = new System.Drawing.Size(184, 200);
				this.TabIndex = 0;
				this.ResumeLayout(false);

				this.Controls.Add(this.childControl);
				this.isInitialized = true;
			}
		}

		private bool m_bShowCloseButton = true;

		public bool ShowCloseButton
		{
			get
			{
				return m_bShowCloseButton;
			}
			set
			{
				if( value != m_bShowCloseButton )
				{
					m_bShowCloseButton = value;
				}
			}
		}

		private bool m_bShowGripper = true;

		public bool ShowGripper
		{
			get
			{
				return m_bShowGripper;
			}
			set
			{
				if( value != m_bShowGripper )
				{
					m_bShowGripper = value;
				}
			}
		}

		public override void ConfirmDeactivate()
		{
			// Do not call base.ConfirmDeactivate 
			// for disabled close container with mainFrameBarManadger
		}

        /// <summary>
        /// Adjusts bounds of the popupHost.
        /// </summary>
        public void AdjustPopupHostBounds()
        {
            this.PopupHost.ComputeLayout();
        }

	}
}