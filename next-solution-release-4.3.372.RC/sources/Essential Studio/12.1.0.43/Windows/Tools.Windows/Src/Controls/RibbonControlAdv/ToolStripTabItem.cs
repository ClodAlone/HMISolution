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
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;
using System.Drawing;

namespace Syncfusion.Windows.Forms.Tools
{
	[System.Windows.Forms.Design.ToolStripItemDesignerAvailability( System.Windows.Forms.Design.ToolStripItemDesignerAvailability.None )]
	public class ToolStripTabItem
		: ToolStripButton
		, IToolStripTabItem
		, IShortcutSupport
		, ICustomTypeDescriptor
	{
		#region Constants
		const int DEFAULT_PADDING = 1;
		#endregion

		#region Constructors
		public ToolStripTabItem()
		{
			this.Padding = new Padding( DEFAULT_PADDING );
			this.CheckOnClick = !this.Checked;
		}
		#endregion
		#region Overrides
       
		protected override void Dispose(bool disposing)
		{
			if(disposing)
				this.Panel=null;
			base.Dispose(disposing);
		}
		protected override void OnCheckStateChanged( EventArgs e )
		{
			base.OnCheckStateChanged( e );
			this.CheckOnClick = !this.Checked;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnTextChanged( EventArgs e )
		{
			base.OnTextChanged( e );
            TabItemAssignment(this, this.Panel);
			this.Panel.Text = this.Text;
		}

		public override System.Drawing.Size GetPreferredSize(System.Drawing.Size constrainingSize)
		{
			RibbonControlAdvHeader header = this.Parent as RibbonControlAdvHeader;
            if (header != null && (header.RibbonStyle == RibbonStyle.Office2010 || header.RibbonStyle == RibbonStyle.Office2013) && this.ImageScaling != ToolStripItemImageScaling.None)
			{
				int width = TextRenderer.MeasureText(this.Text, this.Font, constrainingSize).Width;
				int height = TextRenderer.MeasureText(this.Text, this.Font, constrainingSize).Height;
				int Padding = 10;
				width += 2 * Padding;
                if (!header.MenuButtonVisible)
                {
                    if (height > DEF_HEIGHT)
                        return base.GetPreferredSize(constrainingSize);
                    else
                        return new Size(width, DEF_HEIGHT);
                }
                else if (height > DEF_HEIGHT)
					return base.GetPreferredSize(constrainingSize);
				else
                    return new Size(width, header.MenuButton.Height);
			}
			return base.GetPreferredSize(constrainingSize);
		}

		#endregion
        
		#region IShortcutSupport Implementation
		void IShortcutSupport.ProcessShortcut()
		{
			this.Checked = true;
		}
		#endregion

		#region ICustomTypeDescriptor implementation
		AttributeCollection ICustomTypeDescriptor.GetAttributes()
		{
			return TypeDescriptor.GetAttributes( this, true );
		}
		string ICustomTypeDescriptor.GetClassName()
		{
			return TypeDescriptor.GetClassName( this, true );
		}
		string ICustomTypeDescriptor.GetComponentName()
		{
			return TypeDescriptor.GetComponentName( this, true );
		}
		TypeConverter ICustomTypeDescriptor.GetConverter()
		{
			return TypeDescriptor.GetConverter( this, true );
		}
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent( this, true );
		}
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty( this, true );
		}
		object ICustomTypeDescriptor.GetEditor( Type editorBaseType )
		{
			return TypeDescriptor.GetEditor( this, editorBaseType, true );
		}
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
		{
			return TypeDescriptor.GetEvents( this, true );
		}
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents( Attribute[] attributes )
		{
			return TypeDescriptor.GetEvents( this, attributes, true );
		}
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
		{
			return GetProperties( TypeDescriptor.GetProperties( this, true ) );
		}
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties( Attribute[] attributes )
		{
			return GetProperties( TypeDescriptor.GetProperties( this, attributes, true ) );
		}
		object ICustomTypeDescriptor.GetPropertyOwner( PropertyDescriptor pd )
		{
			return this;
		}

		PropertyDescriptorCollection GetProperties( PropertyDescriptorCollection pdCol )
		{
			List<PropertyDescriptor> list = new List<PropertyDescriptor>( pdCol.Count );

			foreach( PropertyDescriptor pd in pdCol )
			{
				if( IsHidden( pd.Name ) )
				{
					Attribute[] attributes = new Attribute[]
					{
						new BrowsableAttribute(false),
						new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)
					};
					list.Add( new CustomDescriptor( pd, attributes ) );
				}
				else
					list.Add( pd );
			}

			return new PropertyDescriptorCollection( list.ToArray() );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sName"></param>
		/// <returns></returns>
		private static bool IsHidden( string sName )
		{
			for( int i = 0, len = HiddenProperties.Length; i < len; i++ )
			{
				if( sName == HiddenProperties[ i ] )
				{
					return true;
				}
			}
			return false;
		}
		#endregion
        private static void TabItemAssignment(ToolStripTabItem tabItem, Control control)
        {
            if (control != null)
            {
                foreach (Control subControl in control.Controls)
                {
                    ToolStripEx ribbonGroup = subControl as ToolStripEx;
                    if (ribbonGroup != null && ribbonGroup.TabItem == null)
                    {
                        ribbonGroup.TabItem = tabItem;
                    }

                    TabItemAssignment(tabItem, subControl);
                }
            }
        }
		#region ShouldSerialize & Reset methods
		bool ShouldSerializePadding()
		{
			return this.Padding.All != DEFAULT_PADDING;
		}
		new void ResetPadding()
		{
			this.Padding = new Padding( DEFAULT_PADDING );
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the zero based index position of Tab items.
		/// </summary>
		[
		Browsable(false)        
		]
		public int Position
		{
			get
			{
				RibbonControlAdvHeader header = this.Parent as RibbonControlAdvHeader;
				if (header != null)
				{
					return header.MainItems.IndexOf(this);
				}
				else
					return -1;
			}
			set
			{
				if (this.Position != value)
				{
					RibbonControlAdvHeader header = this.Parent as RibbonControlAdvHeader;
					if (header != null && this.Position != value)
					{
						if (header.MainItems.Count <= value || value <0)
							throw new ArgumentOutOfRangeException("Position", "Position is a zero based index. Position can not be set less than zero or greater than the collection length");
						OnPositionChanged(header, value);
					}
				}
			}
		}

		private void OnPositionChanged(RibbonControlAdvHeader header, int position)
		{
			header.InsertMainItem(this, position);

		}
		
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Content )]
		public RibbonPanel Panel
		{
			get
			{
				if( m_panel == null )
				{
					m_panel = new RibbonPanel( this );
					m_bAutoGeneratedPanel = true;
				}
				return m_panel;
			}
			set
			{
				if (m_panel != value)
				{
					if (m_panel != null)
					{
						if (m_bAutoGeneratedPanel)
						{
							m_panel.Dispose();
							m_bAutoGeneratedPanel = false;
						}
						else m_panel.TabItem = null;
					}
					
					m_panel = value;

					if (m_panel != null)
					{
						m_panel.TabItem = this;
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public override Padding Padding
		{
			get
			{
				return base.Padding;
			}
			set
			{
				base.Padding = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public override bool Selected
		{
			get
			{
				return m_bSelected;
			}
		}
		/// <summary>
		/// Gets/Sets value if TabItem is selected.
		/// </summary>
		internal bool SelectedInternal
		{
			get
			{
				return m_bSelected;
			}
			set
			{
				if( m_bSelected != value )
				{
					m_bSelected = value;
					Invalidate();
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal bool ShowPanel
		{
			get
			{
				return m_bShowPanel;
			}
			set
			{
				if (m_bShowPanel != value)
				{
					m_bShowPanel = value;
					
					if (this.Panel != null)
					{
						this.Panel.Visible = m_bShowPanel && this.Checked;
					}
				}
			}
		}
		#endregion

		#region Fields
		RibbonPanel m_panel;

		/// <summary>
		/// Default width of the ToolstripTabItem in Office 2010 Style.
		/// </summary>
		const int DEF_WIDTH = 53;
		/// <summary>
		/// Default height of the ToolstripTabItem in Office 2010 Style.
		/// </summary>
		const int DEF_HEIGHT = 25;
		static string[] HiddenProperties = { "Checked", "CheckState", "CheckOnClick", };
		/// <summary>
		/// Indicates if TabItem is selected.
		/// </summary>
		private bool m_bSelected = false;
		/// <summary>
		/// Indicates if RibbonPanel was auto generated
		/// </summary>
		private bool m_bAutoGeneratedPanel = false;
		/// <summary>
		/// 
		/// </summary>
		private bool m_bShowPanel = true;
		#endregion
	}

	#region *** CustomDescriptor
	class CustomDescriptor : PropertyDescriptor
	{
		#region Constructors
		public CustomDescriptor( PropertyDescriptor baseDescriptor )
			: this( baseDescriptor, new Attribute[] { } )
		{
		}
		public CustomDescriptor( PropertyDescriptor baseDescriptor, Attribute[] attributes )
			: base( baseDescriptor, attributes )
		{
			m_baseDescriptor = baseDescriptor;
		}
		#endregion

		#region Properties
		public override Type ComponentType
		{
			get
			{
				return m_baseDescriptor.ComponentType;
			}
		}
		public override Type PropertyType
		{
			get
			{
				return m_baseDescriptor.PropertyType;
			}
		}
		public override bool IsReadOnly
		{
			get
			{
				return m_baseDescriptor.IsReadOnly;
			}
		}
		public override bool IsBrowsable
		{
			get
			{
				return m_baseDescriptor.IsBrowsable;
			}
		}
		#endregion

		#region Overrides
		public override object GetValue( object component )
		{
			return m_baseDescriptor.GetValue( component );
		}
		public override void SetValue( object component, object value )
		{
			m_baseDescriptor.SetValue( component, value );
		}
		public override bool CanResetValue( object component )
		{
			return m_baseDescriptor.CanResetValue( component );
		}
		public override void ResetValue( object component )
		{
			m_baseDescriptor.ResetValue( component );
		}
		public override bool ShouldSerializeValue( object component )
		{
			return m_baseDescriptor.ShouldSerializeValue( component );
		}
		#endregion

		#region Fields
		PropertyDescriptor m_baseDescriptor;
		#endregion
	}
	#endregion
}
#endif