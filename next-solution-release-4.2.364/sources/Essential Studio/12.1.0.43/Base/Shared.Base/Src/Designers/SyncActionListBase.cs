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

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Text;
using System.Reflection;


namespace Syncfusion.Windows.Forms.Design
{
	public abstract class SyncActionListBase<ComponentType> :
		DesignerActionList where ComponentType : Component
	{
		private ComponentType designerComponentType = null;
		private DesignerActionService designerActionService = null;

		DesignerActionItemCollection actonListItems = null;


		public SyncActionListBase( IComponent component )
			: base( component )
		{
			this.designerComponentType = component as ComponentType;
			this.designerActionService = GetService( typeof( DesignerActionService ) ) as DesignerActionService;
		}

		public ComponentType Control
		{
			get
			{
				return this.designerComponentType;
			}
		}

		protected PropertyDescriptor GetProperty( String propertyName )
		{
			PropertyDescriptor pd = TypeDescriptor.GetProperties( this.designerComponentType )[ propertyName ];
			if( pd == null )
			{
				throw new ArgumentException( "Property " + propertyName + " not found in " + typeof( ComponentType ).Name );
			}
			else
				return pd;
		}

		protected void SetValue( string propertyName, object value )
		{
			GetProperty( propertyName ).SetValue( this.designerComponentType, value );

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			DesignerActionUIService daUIService = GetService(typeof(DesignerActionUIService)) as DesignerActionUIService;
			
			if (daUIService != null)
			{
				daUIService.Refresh(this.Component);
			}
#endif
		}

		protected object GetValue( string propertyName )
		{
			return GetProperty( propertyName ).GetValue( this.designerComponentType );
		}

		protected virtual void InitializeActionList()
		{
		}

		private Control control
		{
			get
			{
				return this.designerComponentType as Control;
			}
		}

		public sealed override DesignerActionItemCollection GetSortedActionItems()
		{
			this.actonListItems = new DesignerActionItemCollection();
			InitializeActionList();
			return this.actonListItems;
		}

		protected void AddDesignerActionHeaderItem( string headerText )
		{
			this.actonListItems.Add( new DesignerActionHeaderItem( headerText ) );
		}

		protected void AddDesignerActionPropertyItem( string propertyName, string displayText, string categoryText, string descriptionText )
		{
			this.actonListItems.Add( new DesignerActionPropertyItem( propertyName, displayText, categoryText, descriptionText ) );
		}

		protected void AddDesignerActionTextItem( string text, string categoryText )
		{
			this.actonListItems.Add( new DesignerActionTextItem( text, categoryText ) );
		}

		protected void AddDesignerActionMethodItem( string methodName, string displayText, string categoryText, string descriptionText )
		{
			this.actonListItems.Add( new DesignerActionMethodItem( this, methodName, displayText, categoryText, descriptionText ) );
		}
	}
}
#endif