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
using System.Reflection;
using System.Collections;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using System.Resources;
using System.IO;

namespace Syncfusion.Windows.Forms.InternalMenus
{
    /// <exclude/>
    /// <summary>
    /// Abstract base class for menu implementation
    /// </summary>
    public abstract class MenuImp
	{
		/// <exclude/>
		protected ResourceManager resourceManager;
		
		/// <exclude/>
		public MenuImp()
		{
		}

		#region Factory Methods
		/// <summary>Derived classes must override.</summary>
		public abstract object[] CreateMenus(ResourceManager manager,MenuItemStructCollection[] structCollectionArray);
		#endregion

		/// <summary>
		///     returns a Shortcut based on the string representation (e.g. CtrlN)
		/// </summary>
		/// <param name="stringRepresentation" type="string">
		///     <para>
		///			the string to convert into a Shortcut
		///     </para>
		/// </param>
		/// <returns>
		///     A System.Windows.Forms.Shortcut value...
		/// </returns>
		protected Shortcut GetShortcutByStringRep(string stringRepresentation)
		{
			object oReturn = null;
			try
			{
				Type sType = Shortcut.F1.GetType();
				oReturn = sType.InvokeMember(stringRepresentation, BindingFlags.GetField, null, Shortcut.F1, new object[0]);
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex.ToString());
				return Shortcut.None;
			}
			return (Shortcut)oReturn;
		}
		
	}
}
