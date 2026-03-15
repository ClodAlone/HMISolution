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
using System.Xml.Serialization;

namespace Syncfusion.Windows.Forms.InternalMenus
{
    /// <exclude/>
    /// <summary>Internal structure used for menu items</summary>
    public class MenuItemStruct
	{
		/// <exclude/>
		/// <summary>Text to display</summary>
		public string text;
		/// <exclude/>
		/// <summary>Resource of the icon</summary>
		public string iconResource;
		/// <exclude/>
		/// <summary>Tooltip information</summary>
		public string shortcutText;
		/// <exclude/>
		/// <summary>obsolete</summary>
		public string parent;
		/// <exclude/>
		/// <summary>obsolete</summary>
		public bool hasChildren;
		/// <exclude/>
		/// <summary>String format of Shortcut. e.g. CtrlN</summary>
		public string shortcut;
		/// <exclude/>
		/// <summary>Name of the event handler</summary>
		public string eventHandler;
		/// <exclude/>
		/// <summary>Children of the menu item</summary>
		public MenuItemStructCollection children;
	}

    /// <exclude/>
    /// <summary>Internal structure used for toolbar items</summary>
    public class ToolBarItemStruct
	{
		/// <exclude/>
		/// <summary>Resource of the icon</summary>
		public string iconResource;
		/// <exclude/>
		/// <summary>Tooltip information</summary>
		public string shortcutText;
		/// <exclude/>
		/// <summary>Text to display</summary>
		public string text;
		/// <exclude/>
		/// <summary>Name of the event handler</summary>
		public string eventHandler;
		/// <summary>
		/// Availabe styles are Button,ToggleButton,Radio,CheckBox,TextBox and ComboBox
		/// </summary>
		[XmlAttribute("Style")]
		public string style = "Button";
		/// <exclude/>
		/// <summary>Width of the button if custom drawn</summary>
		[XmlAttribute("Width")]
		public int width = 23;
		/// <exclude/>
		/// <summary>Height of the button if custom drawn</summary>
		[XmlAttribute("Height")]
		public int height = 23;
	}
}
