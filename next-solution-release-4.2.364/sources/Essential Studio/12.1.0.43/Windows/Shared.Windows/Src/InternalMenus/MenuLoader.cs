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
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Reflection;
using System.Xml.Serialization;
using System.Resources;
using System.Collections;

namespace Syncfusion.Windows.Forms.InternalMenus
{
    /// <exclude/>
	/// <summary>Type of menus to add</summary>
    public enum FactoryType
	{
		/// <exclude/>
		/// <summary>Standard WinForms menus</summary>
		WinFormsMenuFactory = 0,
		/// <exclude/>
		/// <summary>XPMenus from Essential Tools if available</summary>
		XPMenuFactory = 1,
		/// <exclude/>
		/// <summary>New Whidbey menus</summary>
		WhidbeyMenuFactory = 2
	};
    /// <exclude/>
    /// <summary>
	/// MenuLoader contains static implementation of MenuFactories
	/// </summary>
	public class MenuLoader
	{
		#region Members
		private static ResourceManager resManager;
		private static FactoryType factoryType;
		private static MenuFactory menuFactory;
		private static string eventActionNamespace;
		private static ToolBarItemStructCollection[] toolbarItemStructCollections;
		private static MenuItemStructCollection[] menuItemStructCollections;
		private static object parentObject;
		#endregion

		#region CreateFactory Methods
		/// <summary>
		///     Creates a MenuFactory
		/// </summary>
		/// <param name="defaultFactoryType" type="Syncfusion.Windows.Forms.InternalMenus.FactoryType">
		///     <para>
		///         Type of factory to create
		///     </para>
		/// </param>
		/// <param name="eventActionNamespace" type="string">
		///     <para>
		///         The fully qualified namespace that contains the menu actions
		///     </para>
		/// </param>
		/// <param name="toolbarDefinitionXmlResource" type="string[]">
		///     <para>
		///         String array of qualified names for toolbar resources that contain ToolbarDefinitions
		///     </para>
		/// </param>
		/// <param name="menuDefinitionXmlResource" type="string[]">
		///     <para>
		///         String array of qualified names for menu resources that contain MenuDefinitions
		///     </para>
		/// </param>
		/// <param name="resManager" type="System.Resources.ResourceManager">
		///     <para>
		///         The ResourceManager that contains the image/icon information
		///     </para>
		/// </param>
		/// <param name="parent" type="object">
		///     <para>
		///         The parent form/object that contains the menu
		///     </para>
		/// </param>
		/// <returns>
		///     A Syncfusion.Windows.Forms.InternalMenus.MenuFactory value...
		/// </returns>
		public static MenuFactory CreateFactory(FactoryType defaultFactoryType, string eventActionNamespace, string[] toolbarDefinitionXmlResource,
			string[] menuDefinitionXmlResource,ResourceManager resManager, object parent)
		{
			ParentObject = parent;
			MenuItemStructCollection[] menuList = GetMenuItemStructCollections(menuDefinitionXmlResource);//,typeof(MenuItemStructCollection));
			ToolBarItemStructCollection[] toolbarList = GetToolBarItemStructCollections(toolbarDefinitionXmlResource);//,typeof(ToolBarItemStructCollection));
			return CreateFactory(defaultFactoryType,eventActionNamespace,toolbarList,
				menuList,resManager,parent);
		}

		/// <summary>
		///     Creates a MenuFactory
		/// </summary>
		/// <param name="defaultFactoryType" type="Syncfusion.Windows.Forms.InternalMenus.FactoryType">
		///     <para>
		///         Type of MenuFactory to create
		///     </para>
		/// </param>
		/// <param name="eventActionNamespace" type="string">
		///     <para>
		///         Fully qualified namespace that contains the action items
		///     </para>
		/// </param>
		/// <param name="toolbarItemStructXmlResource" type="string">
		///     <para>
		///         Resource in parent assembly that contains a ToolbarItemStructCollection
		///     </para>
		/// </param>
		/// <param name="menuItemStructXmlResource" type="string">
		///     <para>
		///         Resource in parent assembly that contains a MenuItemStructCollection
		///     </para>
		/// </param>
		/// <param name="resManager" type="System.Resources.ResourceManager">
		///     <para>
		///         ResourceManager that contains the icon/image resources
		///     </para>
		/// </param>
		/// <param name="parent" type="object">
		///     <para>
		///         The parent form/object that contains the menu.
		///     </para>
		/// </param>
		/// <returns>
		///     A Syncfusion.Windows.Forms.InternalMenus.MenuFactory value...
		/// </returns>
		internal static MenuFactory CreateFactory(FactoryType defaultFactoryType, string eventActionNamespace, string toolbarItemStructXmlResource,
			string menuItemStructXmlResource,ResourceManager resManager, object parent)
		{
			ParentObject = parent;
			return CreateFactory(defaultFactoryType,eventActionNamespace,(ToolBarItemStructCollection)SetItemStructCollection(toolbarItemStructXmlResource,typeof(ToolBarItemStructCollection)),
				(MenuItemStructCollection)SetItemStructCollection(menuItemStructXmlResource,typeof(MenuItemStructCollection)),resManager,parent);
		}

		/// <summary>
		///     Creates a MenuFactory
		/// </summary>
		/// <param name="defaultFactoryType" type="Syncfusion.Windows.Forms.InternalMenus.FactoryType">
		///     <para>
		///         Type of MenuFactory to create
		///     </para>
		/// </param>
		/// <param name="eventActionNamespace" type="string">
		///     <para>
		///         Fully qualified namespace that contains the action items
		///     </para>
		/// </param>
		/// <param name="toolbarItemCollectionXmlStream" type="System.IO.Stream">
		///     <para>
		///         Stream that contains the ToolbarItemStructCollection
		///     </para>
		/// </param>
		/// <param name="menuItemCollectionXmlStream" type="System.IO.Stream">
		///     <para>
		///         Stream that contains the MenuItemStructCollection
		///     </para>
		/// </param>
		/// <param name="resManager" type="System.Resources.ResourceManager">
		///     <para>
		///         ResourceManager containing the icon/image resources for the menu
		///     </para>
		/// </param>
		/// <param name="parent" type="object">
		///     <para>
		///         Parent form/object that contains the menu
		///     </para>
		/// </param>
		/// <returns>
		///     A Syncfusion.Windows.Forms.InternalMenus.MenuFactory value...
		/// </returns>
		internal static MenuFactory CreateFactory(FactoryType defaultFactoryType, string eventActionNamespace, System.IO.Stream toolbarItemCollectionXmlStream,
			System.IO.Stream menuItemCollectionXmlStream, ResourceManager resManager, object parent)
		{
			ParentObject = parent;
			return CreateFactory(defaultFactoryType,eventActionNamespace,(ToolBarItemStructCollection)SetItemStructCollection(toolbarItemCollectionXmlStream,typeof(ToolBarItemStructCollection)),
				(MenuItemStructCollection)SetItemStructCollection(menuItemCollectionXmlStream,typeof(MenuItemStructCollection)),resManager,parent);
		}
		
		/// <summary>
		///     Creates a MenuFactory
		/// </summary>
		/// <param name="defaultFactoryType" type="Syncfusion.Windows.Forms.InternalMenus.FactoryType">
		///     <para>
		///         Type of MenuFactory to create
		///     </para>
		/// </param>
		/// <param name="eventActionNamespace" type="string">
		///     <para>
		///         Fully qualified namespace containing the action items
		///     </para>
		/// </param>
		/// <param name="toolbarItemStructCollection" type="Syncfusion.Windows.Forms.InternalMenus.ToolBarItemStructCollection">
		///     <para>
		///         The ToolbarItemStructCollection to create
		///     </para>
		/// </param>
		/// <param name="menuItemStructCollection" type="Syncfusion.Windows.Forms.InternalMenus.MenuItemStructCollection">
		///     <para>
		///         The MenuItemStructCollection to create
		///     </para>
		/// </param>
		/// <param name="resManager" type="System.Resources.ResourceManager">
		///     <para>
		///         The ResourceManager that contains the icon/image resources
		///     </para>
		/// </param>
		/// <param name="parent" type="object">
		///     <para>
		///         Parent form/object that contains the menu
		///     </para>
		/// </param>
		/// <returns>
		///     A Syncfusion.Windows.Forms.InternalMenus.MenuFactory value...
		/// </returns>
		internal static MenuFactory CreateFactory(FactoryType defaultFactoryType, string eventActionNamespace, ToolBarItemStructCollection toolbarItemStructCollection,
			MenuItemStructCollection menuItemStructCollection, ResourceManager resManager, object parent)
		{

			return CreateFactory(defaultFactoryType,eventActionNamespace,new ToolBarItemStructCollection[]{toolbarItemStructCollection},
				new MenuItemStructCollection[]{menuItemStructCollection},resManager,parent);
		}

		/// <summary>
		///     Creates a MenuFactory
		/// </summary>
		/// <param name="defaultFactoryType" type="Syncfusion.Windows.Forms.InternalMenus.FactoryType">
		///     <para>
		///         Type of MenuFactory to create
		///     </para>
		/// </param>
		/// <param name="eventActionNamespace" type="string">
		///     <para>
		///         Fully qualified namespace that contains the action items
		///     </para>
		/// </param>
		/// <param name="toolbarItemStructs" type="Syncfusion.Windows.Forms.InternalMenus.ToolBarItemStructCollection[]">
		///     <para>
		///         Array of ToolBarItemStructCollections to create multiple toolbars
		///     </para>
		/// </param>
		/// <param name="menuItemStructs" type="Syncfusion.Windows.Forms.InternalMenus.MenuItemStructCollection[]">
		///     <para>
		///         Array of MenuItemStructCollections to create multiple menus
		///     </para>
		/// </param>
		/// <param name="resManager" type="System.Resources.ResourceManager">
		///     <para>
		///         ResourceManager containing the image resources
		///     </para>
		/// </param>
		/// <param name="parent" type="object">
		///     <para>
		///         Parent form/object that contains the menu
		///     </para>
		/// </param>
		/// <returns>
		///     A Syncfusion.Windows.Forms.InternalMenus.MenuFactory value...
		/// </returns>
		internal static MenuFactory CreateFactory(FactoryType defaultFactoryType,string eventActionNamespace,ToolBarItemStructCollection[] toolbarItemStructs,
			MenuItemStructCollection[] menuItemStructs,ResourceManager resManager, object parent)
		{
			CurrentToolBarItemStructCollection = toolbarItemStructs;
			CurrentMenuItemStructCollection = menuItemStructs;
			EventActionNamespace = eventActionNamespace;
			CurrentResourceManager = resManager;
			ParentObject = parent;

			return CreateFactory(defaultFactoryType);
		}


		/// <summary>
		///     Creates the MenuFactory based on toolbar/menu items provided by the public constructors
		/// </summary>
		/// <param name="fType" type="Syncfusion.Windows.Forms.InternalMenus.FactoryType">
		///     <para>
		///         The type of MenuFactory to create.
		///     </para>
		/// </param>
		/// <returns>
		///     A Syncfusion.Windows.Forms.InternalMenus.MenuFactory value...
		/// </returns>
		private static MenuFactory CreateFactory(FactoryType fType)
		{
			CurrentFactoryType = fType;
			switch(CurrentFactoryType)
			{
				case FactoryType.WinFormsMenuFactory:
					//this is the default fallback menu. if this doesn't work, nothing will.
					CurrentMenuFactory = new WinFormsMenuFactory();
					break;
                case FactoryType.WhidbeyMenuFactory:
                    try
                    {
#if !(SyncfusionFramework1_0 || SyncfusionFramework1_1)
                        CurrentMenuFactory = new WhidbeyMenuFactory();
#else
                        CreateFactory(FactoryType.WinFormsMenuFactory);
#endif
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.ToString());
                        CreateFactory(FactoryType.WinFormsMenuFactory);
                    }
                    break;
                default:
                    CurrentMenuFactory = new WinFormsMenuFactory();
                    break;
			}
			return CurrentMenuFactory;
		}
		#endregion

		#region Retrieval Methods
		/// <summary>
		///     Constructs a MenuItemStructCollection[] based on MenuDefinition resources in the parent's assembly
		/// </summary>
		/// <param name="menuDefinitionsInParent" type="string[]">
		///     <para>
		///         Qualified resource name that contains the MenuDefinitions
		///     </para>
		/// </param>
		/// <returns>
		///     A Syncfusion.Windows.Forms.InternalMenus.MenuItemStructCollection[] value...
		/// </returns>
		private static MenuItemStructCollection[] GetMenuItemStructCollections(string[] menuDefinitionsInParent)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(MenuDefinition));
			Type type = ParentObject.GetType();
			Stream stream = null;
			ArrayList list = new ArrayList();
			foreach(string menuResource in menuDefinitionsInParent)
			{
				stream = type.Module.Assembly.GetManifestResourceStream(menuResource);
				MenuDefinition menu = (MenuDefinition)serializer.Deserialize(stream);
				stream.Close();
				//ensure name is proper
				menu.MenuItems.MenuName = menu.MenuName;
				list.Add(menu.MenuItems);
			}
			return (MenuItemStructCollection[])list.ToArray(typeof(MenuItemStructCollection));
		}

		/// <summary>
		///     Constructs a ToolBarItemStructCollection[] based on ToolBarDefinition resources in the parent's assembly
		/// </summary>
		/// <param name="toolbarDefinitionsInParent" type="string[]">
		///     <para>
		///         Qualified resource name that contains the ToolBarDefinitions
		///     </para>
		/// </param>
		/// <returns>
		///     A Syncfusion.Windows.Forms.InternalMenus.ToolBarItemStructCollection[] value...
		/// </returns>
		private static ToolBarItemStructCollection[] GetToolBarItemStructCollections(string[] toolbarDefinitionsInParent)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(ToolBarDefinition));
			Type type = ParentObject.GetType();
			Stream stream = null;
			ArrayList list = new ArrayList();
			foreach(string toolbarResource in toolbarDefinitionsInParent)
			{
				stream = type.Module.Assembly.GetManifestResourceStream(toolbarResource);
				ToolBarDefinition toolbar = (ToolBarDefinition)serializer.Deserialize(stream);
				stream.Close();
				//ensure name is proper
				toolbar.ToolBarItems.ToolBarName = toolbar.ToolBarName;
				list.Add(toolbar.ToolBarItems);
			}
			return (ToolBarItemStructCollection[])list.ToArray(typeof(ToolBarItemStructCollection));
		}

		/// <summary>
		///     Returns an ArrayList of either MenuItemStructCollections or ToolBarItemStructCollections
		/// </summary>
		/// <param name="resourcesInParent" type="string[]">
		///     <para>
		///         The resources in the parent's assembly
		///     </para>
		/// </param>
		/// <param name="itemStructCollectionType" type="System.Type">
		///     <para>
		///         typeof(ToolBarItemStructCollection) or typeof(MenuItemStructCollection)
		///     </para>
		/// </param>
		/// <returns>
		///     A System.Collections.ArrayList value...
		/// </returns>
		private static ArrayList SetItemStructCollections(string[] resourcesInParent,Type itemStructCollectionType)
		{
			ArrayList list = new ArrayList();
			foreach(string resource in resourcesInParent)
			{
				list.Add(SetItemStructCollection(resource,itemStructCollectionType));
			}
			return list;
		}
		/// <summary>
		///     Returns either a MenuItemStructCollection or a ToolBarItemStructCollection
		/// </summary>
		/// <param name="xmlStream" type="System.IO.Stream">
		///     <para>
		///         The stream to read the collection from
		///     </para>
		/// </param>
		/// <param name="itemStructCollectionType" type="System.Type">
		///     <para>
		///        typeof(ToolBarItemStructCollection) or typeof(MenuItemStructCollection) 
		///     </para>
		/// </param>
		/// <returns>
		///     A object value...
		/// </returns>
		private static object SetItemStructCollection(Stream xmlStream,Type itemStructCollectionType)
		{
			XmlSerializer serializer = new XmlSerializer(itemStructCollectionType);
			object returnVal = serializer.Deserialize(xmlStream);
			xmlStream.Close();
			return returnVal;
		}

		/// <summary>
		///     Returns either a MenuItemStructCollection or a ToolBarItemStructCollection
		/// </summary>
		/// <param name="resourceInParent" type="string">
		///     <para>
		///         The fully qualified resource name in the parent's assembly
		///     </para>
		/// </param>
		/// <param name="itemStructCollectionType" type="System.Type">
		///     <para>
		///         typeof(ToolBarItemStructCollection) or typeof(MenuItemStructCollection) 
		///     </para>
		/// </param>
		/// <returns>
		///     A object value...
		/// </returns>
		private static object SetItemStructCollection(string resourceInParent,Type itemStructCollectionType)
		{
			Type type = ParentObject.GetType();
			Stream stream = type.Module.Assembly.GetManifestResourceStream(resourceInParent);
			return SetItemStructCollection(stream,itemStructCollectionType);
		}
		#endregion

		#region Static Properties
		internal static object ParentObject
		{
			get
			{
				return parentObject;
			}
			set
			{
				parentObject = value;
			}
		}
		internal static FactoryType CurrentFactoryType
		{
			get
			{
				return factoryType;
			}
			set
			{
				factoryType = value;
			}
		}

		internal static ResourceManager CurrentResourceManager
		{
			get
			{
				return resManager;
			}
			set
			{
				resManager = value;
			}
		}
		internal static MenuFactory CurrentMenuFactory
		{
			get
			{
				return menuFactory;
			}
			set
			{
				menuFactory = value;
			}
		}

		internal static string EventActionNamespace
		{
			get
			{
				return eventActionNamespace;
			}
			set
			{
				eventActionNamespace = value;
			}
		}

        internal static ToolBarItemStructCollection[] CurrentToolBarItemStructCollection
        {
            get
            {
                return toolbarItemStructCollections;
            }
            set
            {
                toolbarItemStructCollections = value;
            }
        }

        internal static MenuItemStructCollection[] CurrentMenuItemStructCollection
        {
            get
            {
                return menuItemStructCollections;
            }
            set
            {
                menuItemStructCollections = value;
            }
        }
		#endregion
	}
}
