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
using System.ComponentModel;
using System.Diagnostics;

namespace Syncfusion.Windows.Styles
{
	/// <summary>
	/// This is an abstract base class that provides identity information
	/// for <see cref="StyleInfoBase"/>  objects.
	/// </summary>
	[DebuggerStepThrough()]
	public abstract class StyleInfoIdentityBase : IDisposable
	{
		StyleInfoIdentityBase innerIdentity;
        bool isDisposable = true;

		/// <summary>
		/// Gets / sets another identity object to be used for determining base styles.
		/// GetBaseStyle will call InnerIdentity.GetBaseStyle if this object is not NULL.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public StyleInfoIdentityBase InnerIdentity
		{
			get
			{
				return innerIdentity;
			}
			set
			{
				innerIdentity = value;
			}
		}
		/// <summary>
		/// Loops through all base styles until it finds a style that has a specific property initialized.
		/// </summary>
		/// <param name="thisStyleInfo"></param>
        /// <param name="sip">A <see cref="StyleInfoProperty"/> that identifies the property to operate on.</param>
		/// <returns>A <see cref="StyleInfoBase"/> that has the property initialized.</returns>
		public virtual StyleInfoBase GetBaseStyle(IStyleInfo thisStyleInfo, StyleInfoProperty sip)
		{
			if (innerIdentity != null)
			{
				return innerIdentity.GetBaseStyle(thisStyleInfo, sip );
			}

			IStyleInfo[] baseStyles = GetBaseStyles(thisStyleInfo);
			if (baseStyles != null)
			{
				foreach (StyleInfoBase style in baseStyles)
				{
					if (style != null && style.Store != null && style.HasValue(sip))
						return style;
				}
			}
			return null;
		}

        /// <summary>
        /// Gets or sets a value indicating whether this object is disposable.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this object is disposable; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisposable
        {
            get { return isDisposable; }
            set { isDisposable = value; }
        }

		/// <summary>
		/// Releases all resources used by the component.
		/// </summary>
		public virtual void Dispose()
		{
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Loops through all base styles until it finds an expandable <see cref="StyleInfoSubObjectBase"/>
		/// that has one or more properties initialized.
		/// </summary>
		/// <param name="thisStyleInfo">The style object.</param>
		/// <param name="sip">A <see cref="StyleInfoProperty"/> that identifies the property to operate on.</param>
		/// <returns>A <see cref="StyleInfoBase"/> that has the property initialized.</returns>
		public virtual StyleInfoBase GetBaseStyleNotEmptyExpandable(IStyleInfo thisStyleInfo, StyleInfoProperty sip)
		{
			if (!sip.IsExpandable)
				return null;

			IStyleInfo[] baseStyles = GetBaseStyles(thisStyleInfo);
			if (baseStyles != null)
			{
				foreach (StyleInfoBase style in baseStyles)
				{
					if (style.HasValue(sip) && !((IStyleInfo) style.GetValue(sip)).IsEmpty)
						return style;
				}
			}
			return null;
		}

		/// <summary>
		/// Returns an array with base styles for the specified style object.
		/// </summary>
		/// <param name="thisStyleInfo">The style object.</param>
		/// <returns>An array of style objects that are base styles for the current style object.</returns>
		public abstract IStyleInfo[] GetBaseStyles(IStyleInfo thisStyleInfo);

		/// <summary>
		/// Occurs when a property in the <see cref="StyleInfoBase"/> has changed.
		/// </summary>
		/// <param name="style">The <see cref="StyleInfoBase"/> instance that has changed.</param>
		/// <param name="sip">A <see cref="StyleInfoProperty"/> that identifies the property to operate on.</param>
		public virtual void OnStyleChanged(StyleInfoBase style, StyleInfoProperty sip)
		{
		}

		/// <summary>
		/// Occurs before a property in the <see cref="StyleInfoBase"/> is changing.
		/// </summary>
		/// <param name="style">The <see cref="StyleInfoBase"/> instance that is changed.</param>
		/// <param name="sip">A <see cref="StyleInfoProperty"/> that identifies the property to operate on.</param>
		public virtual void OnStyleChanging(StyleInfoBase style, StyleInfoProperty sip)
		{
		}
	}

}
