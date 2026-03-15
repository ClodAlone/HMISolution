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
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.ComponentModel.Design.Serialization;
using System.CodeDom;
using Syncfusion.Windows.Forms.Collections;

namespace Syncfusion.Windows.Forms.Tools
{
	#region *** ToolStripTabGroup
	/// <summary>
	/// Visual group of tab items.
	/// </summary>
	public class ToolStripTabGroup
	{
		#region Fields
		/// <summary>
		/// Name of group.
		/// </summary>
		private string m_name;
		/// <summary>
		/// Color of group.
		/// </summary>
		private Color m_color;
		/// <summary>
		/// List of group bounds to draw at the header.
		/// </summary>
		private List<Rectangle> m_boundsList;
		/// <summary>
		/// 
		/// </summary>
		private bool m_bVisible = true;
        /// <summary>
        /// 
        /// </summary>
        private Font m_font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets name of group.
		/// </summary>
		public string Name
		{
			get
			{
				return m_name;
			}
			set
			{
				if( m_name != value )
				{
					string oldName = m_name;
					m_name = value;

					if( NameChanged != null )
					{
						NameChanged( this,  EventArgs.Empty);
					}
				}
			}
		}
        /// <summary>
        /// 
        /// </summary>
        public Font Font
        {
            get
            {
                return m_font;
            }
            set
            {
                if (m_font != value)
                {
                    m_font = value;

                    if (FontChanged != null)
                    {
                        FontChanged(this, EventArgs.Empty);
                    }
                }
            }
        }
		/// <summary>
		/// Gets or sets color of group.
		/// </summary>
		public Color Color
		{
			get
			{
				return m_color;
			}
			set
			{
				if( m_color != value )
				{
					Color oldColor = m_color;
					m_color = value;

					if( ColorChanged != null )
					{
						ColorChanged( this, EventArgs.Empty);
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public bool Visible
		{
			get
			{
				return m_bVisible;
			}
			set
			{
				if( m_bVisible != value )
				{
					m_bVisible = value;

					if( VisibilityChanged != null )
					{
						VisibilityChanged( this, EventArgs.Empty );
					}
				}
			}
		}
		#endregion

		#region Initialization
		/// <summary>
		/// Creates new instance of ToolStripTabGroup.
		/// </summary>
		public ToolStripTabGroup()
		{
			m_boundsList = new List<Rectangle>();
		}
		#endregion

		#region Internal Properties
		/// <summary>
		/// Gets list of group bounds to draw at the header.
		/// </summary>
		internal List<Rectangle> BoundsList
		{
			get
			{
				return m_boundsList;
			}
		}
		#endregion

		#region Overrides
		/// <summary>
		/// Returns name of group.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return m_name;
		}
		#endregion

		#region Events
		public event EventHandler NameChanged;
		public event EventHandler ColorChanged;
		public event EventHandler VisibilityChanged;
        public event EventHandler FontChanged;
		#endregion
	}
	#endregion

	#region *** TabGroupCollection
	/// <summary>
	/// Collection of tab groups. Also provides TabGroup extended property.
	/// </summary>
	[ProvideProperty( "TabGroup", typeof( ToolStripItem ) )]
	[DesignerSerializer( typeof( TabGroupCollectionSerializer ), typeof( CodeDomSerializer ) )]
	public class TabGroupCollection
		: ObservableList<ToolStripTabGroup>
		, IExtenderProvider
	{
		#region Fields
		/// <summary>
		/// Underlying RibbonControlAdvHeader.
		/// </summary>
		private RibbonControlAdvHeader m_header;
		#endregion

		#region Internal Properties
		/// <summary>
		/// Gets underlying RibbonControlAdvHeader control.
		/// </summary>
		internal RibbonControlAdvHeader Header
		{
			get
			{
				return m_header;
			}
		}
		#endregion

		#region Initialization
		/// <summary>
		/// Creates and initializes new instance of TabGroupCollection.
		/// </summary>
		/// <param name="header">Instance of underlying RibbonControlAdvHeader control.</param>
		internal TabGroupCollection( RibbonControlAdvHeader header )
		{
			if( header == null )
				throw new ArgumentNullException( "header" );

			m_header = header;
		}
		#endregion

		#region IExtenderProvider Members
		/// <summary>
		/// Returns true if component should be extended with TabGroup property.
		/// </summary>
		/// <param name="extendee"></param>
		/// <returns></returns>
		public bool CanExtend( object extendee )
		{
			return ( extendee is ToolStripItem
				&& ( ( ToolStripItem )extendee ).Owner == m_header
				&& !( extendee is IQuickItem ) );
		}
		/// <summary>
		/// Getter of TabGroup extended property.
		/// </summary>
		/// <param name="component"></param>
		/// <returns></returns>
		[Description( "Tab group of ToolStripItem." )]
		[TypeConverter( typeof( TabGroupTypeConverter ) )]
		public ToolStripTabGroup GetTabGroup( Component component )
		{
			ToolStripItem item = component as ToolStripItem;

			return m_header.GetItemGroup( item );
		}
		/// <summary>
		/// Getter of TabGroup extended property.
		/// </summary>
		/// <param name="component"></param>
		/// <param name="value"></param>
		public void SetTabGroup( Component component, ToolStripTabGroup value )
		{
			ToolStripItem item = component as ToolStripItem;

			m_header.SetItemGroup( item, value );
		}
		#endregion
	}
	#endregion

	#region *** TabGroupEventArgs
	/// <summary>
	/// Event arguments for delegates related to tab groups.
	/// </summary>
	public class TabGroupEventArgs
	{
		#region Public Fields
		/// <summary>
		/// Tab group.
		/// </summary>
		public ToolStripTabGroup TabGroup;
		#endregion

		#region Initialization
		/// <summary>
		/// Creates and initializes new instance of TabGroupEventArgs.
		/// </summary>
		/// <param name="group">Tab group.</param>
		public TabGroupEventArgs( ToolStripTabGroup group )
		{
			this.TabGroup = group;
		}
		#endregion
	}
	#endregion

	#region *** TabGroupsEventHandler
	/// <summary>
	/// Delegate for events related to tab groups.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="args"></param>
	public delegate void TabGroupEventHandler( object sender, TabGroupEventArgs args );
	#endregion

	#region *** TabGroupTypeConverter
	/// <summary>
	/// Type converter for TabGroup property,
	/// </summary>
	internal class TabGroupTypeConverter
		: TypeConverter
	{
		#region Constants
		/// <summary>
		/// String corresponding to null property value.
		/// </summary>
		private const string STR_NONE = "(none)";
		#endregion

		#region Overrides
		/// <summary>
		/// Allows converting from strings.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="sourceType"></param>
		/// <returns></returns>
		public override bool CanConvertFrom( ITypeDescriptorContext context, Type sourceType )
		{
			bool result = ( sourceType == typeof( string ) );

			return result | base.CanConvertFrom( context, sourceType );
		}
		/// <summary>
		/// Converts from strings.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="culture"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public override object ConvertFrom( ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value )
		{
			object result = null;

			if( value is string )
			{
				string str = ( ( string )value ).Trim();

				if( str != STR_NONE )
				{
					ToolStripTabItem item = context.Instance as ToolStripTabItem;

					if( item != null )
					{
						RibbonControlAdvHeader header = item.Owner as RibbonControlAdvHeader;

						if( header != null )
						{
							foreach( ToolStripTabGroup group in header.Groups )
							{
								if( group.Name == str )
								{
									result = group;
								}
							}
						}
					}
				}
			}
			else
			{
				result = base.ConvertFrom( context, culture, value );
			}

			return result;
		}
		/// <summary>
		/// Converts to string.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="culture"></param>
		/// <param name="value"></param>
		/// <param name="destinationType"></param>
		/// <returns></returns>
		public override object ConvertTo(
			ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType )
		{
			object result = null;

			if( destinationType == typeof( string ) )
			{
				if( value == null )
				{
					result = STR_NONE;
				}
				else if( value is ToolStripTabGroup )
				{
					result = ( ( ToolStripTabGroup )value ).Name;
				}
			}

			if( result == null )
			{
				result = base.ConvertTo( context, culture, value, destinationType );
			}

			return result;
		}
		/// <summary>
		/// Fills list of standard values.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public override TypeConverter.StandardValuesCollection GetStandardValues( ITypeDescriptorContext context )
		{
			ArrayList result = new ArrayList();
			result.Add( null );

			ToolStripItem item = context.Instance as ToolStripItem;

			if( item != null )
			{
				RibbonControlAdvHeader header = item.Owner as RibbonControlAdvHeader;

				if( header != null )
				{
					foreach( ToolStripTabGroup group in header.Groups )
					{
						result.Add( group.Name );
					}
				}
			}

			return new TypeConverter.StandardValuesCollection( result );
		}
		/// <summary>
		/// Enables standard values.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public override bool GetStandardValuesSupported( ITypeDescriptorContext context )
		{
			return true;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public override bool GetStandardValuesExclusive( ITypeDescriptorContext context )
		{
			return true;
		}
		#endregion
	}
	#endregion

	#region *** TabGroupCollectionSerializer
	/// <summary>
	/// Serializer for TabGroupCollection. Serializes TabGroup extended proeprty.
	/// </summary>
	internal class TabGroupCollectionSerializer
		: CodeDomSerializer
	{
		#region Overrides
		/// <summary>
		/// Performs serialization.
		/// </summary>
		/// <param name="manager"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public override object Serialize( IDesignerSerializationManager manager, object value )
		{
			object result = null;

			if( value is TabGroupCollection )
			{
				TabGroupCollection groups = ( TabGroupCollection )value;

				CodeDomSerializer baseClassSerializer =
					manager.GetSerializer( typeof( TabGroupCollection ).BaseType, typeof( CodeDomSerializer ) ) as CodeDomSerializer;

				if( baseClassSerializer != null )
				{
					object codeObject = baseClassSerializer.Serialize( manager, value );

					ExpressionContext expr = manager.Context.Current as ExpressionContext;

					if( expr != null && codeObject is CodeStatementCollection )
					{
						CodeStatementCollection statements = ( CodeStatementCollection )codeObject;

						foreach ( KeyValuePair<ToolStripItem, ToolStripTabGroup> entry in groups.Header.TabGroupsHash)
						{
							string sItem = manager.GetName(entry.Key);
							if (sItem != null)
							{
								string sGroup = manager.GetName(entry.Value);
								if (sGroup != null)
								{
									CodeVariableReferenceExpression itemExpr = new CodeVariableReferenceExpression(sItem);
									CodeVariableReferenceExpression groupExpr = new CodeVariableReferenceExpression(sGroup);
									
									CodeExpression[] parameters = new CodeExpression[] { itemExpr, groupExpr };
									statements.Add(new CodeMethodInvokeExpression(expr.Expression, "SetTabGroup", parameters));
								}
							}
						}
					}

					result = codeObject;
				}
			}

			return result;
		}
		#endregion
	}
	#endregion
}
#endif