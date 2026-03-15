#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Summary description for GradientPanelExtDesigner.
	/// </summary>
	public class GradientPanelExtDesigner : System.Windows.Forms.Design.ParentControlDesigner
	{
		#region Class Members

		/// <summary>
		/// Selected primitive.
		/// </summary>
		private Primitive m_selectedPrimitve = null;

		/// <summary>
		/// Distance from a mouse and bounds of the primitive.
		/// </summary>
		private Size MouseOffset = Size.Empty;

		private ISelectionService SelectionService = null;

		#endregion

		#region Class Properties

		/// <summary>
		/// Gets or sets selected primitive.
		/// </summary>
		private Primitive SelectedPrimitive
		{
			get
			{
				return m_selectedPrimitve;
			}
			set
			{
				if( value != m_selectedPrimitve )
				{
					OnBeforeSelectedPrimitive();
					m_selectedPrimitve = value;
					OnAfterSelectedPrimitive();
				}
			}
		}


		/// <summary>
		/// Gets owner control.
		/// </summary>
		private GradientPanelExt OwnerControl
		{
			get
			{
				return this.Control as GradientPanelExt;
			}
		}

		#endregion

		#region Class Initialize/Finalize methods

		public GradientPanelExtDesigner() : base()
		{}

		public override void Initialize( IComponent component )
		{
			base.Initialize( component );

			this.SelectionService = ( ISelectionService )GetService( typeof( ISelectionService ) );

			if( this.SelectionService != null )
			{
				this.SelectionService.SelectionChanged += new EventHandler( SelectionService_SelectionChanged );
			}
		}

		#endregion

		#region Class Override Methods

		protected override void OnMouseDragMove( int x, int y )
		{
			if( this.SelectedPrimitive == null )
			{
				// moves control
				base.OnMouseDragMove( x, y );
			}
			else
			{
				// moves primitive
				Point location = OwnerControl.PointToClient( new Point( x, y ) );
				Point primitiveLocation = new Point( location.X - MouseOffset.Width, 
					location.Y - MouseOffset.Height );

				this.OwnerControl.SuspendLayout();

				SetAlignmentPrimitive( this.SelectedPrimitive, primitiveLocation );
				SetPositionPrimitive( this.SelectedPrimitive, primitiveLocation );

				this.OwnerControl.ResumeLayout();
			}
		}


		private HostPrimitive GetHostPrimitiveForDrag( Control dragContro )
		{
			HostPrimitive primitive = null;

			if( dragContro != null && this.OwnerControl != null && this.OwnerControl.Primitives != null )
			{
				foreach( Primitive item in this.OwnerControl.Primitives )
				{
					HostPrimitive hostItem = item as HostPrimitive;
					
					if( hostItem != null && hostItem.IsContainControl( dragContro ) )
					{
						primitive = hostItem;
						break;
					}
				}
			}

			return primitive;
		}

		protected override void OnMouseDragEnd( bool cancel )
		{
			base.OnMouseDragEnd( cancel );

			this.OwnerControl.MakeDirty();
		}

		protected override void OnMouseDragBegin( int x, int y )
		{
			Primitive primitive = GetDragPrimitive( x, y );

			this.SelectedPrimitive = primitive;

			if( this.SelectedPrimitive != null )
			{
				Point location = OwnerControl.PointToClient( new Point( x, y ) );
				MouseOffset = new Size( location.X - this.SelectedPrimitive.Bounds.X,
					location.Y - this.SelectedPrimitive.Bounds.Y );
			}

			base.OnMouseDragBegin( x, y );

			if( this.SelectedPrimitive != null && this.SelectionService != null
				&& this.SelectionService.PrimarySelection != this.SelectedPrimitive )
			{
				ArrayList arrSelect = new ArrayList();
				arrSelect.Add( this.SelectedPrimitive );
				this.SelectionService.SetSelectedComponents( arrSelect );
			}
		}

		protected override void Dispose( bool disposing )
		{
			base.Dispose( disposing );

			if( this.SelectionService != null )
			{
				this.SelectionService.SelectionChanged -= new EventHandler( SelectionService_SelectionChanged );
			}
		}


		#endregion

		#region Class Utility Method

		/// <summary>
		/// Gets rectangle where can location primitives.
		/// </summary>
		private Rectangle GetLocationRect( Alignment alignment )
		{
			Rectangle rect = Rectangle.Empty;

			int offset = this.OwnerControl.Border.Left + this.OwnerControl.CornerRadius;

			if( !( this.OwnerControl != null && this.OwnerControl.Collapsed &&
				this.OwnerControl.CollapseAlignment != alignment ) )
			{
				switch( alignment )
				{
					case Alignment.Top :
					{
						rect = new Rectangle( this.OwnerControl.ClientRectangle.X + offset, 
							this.OwnerControl.ClientRectangle.Y,
							this.OwnerControl.ClientRectangle.Width - 2 * offset, offset );
						break;
					}
					case Alignment.Bottom :
					{
						rect = new Rectangle( this.OwnerControl.ClientRectangle.X + offset, 
							this.OwnerControl.ClientRectangle.Bottom - offset,
							this.OwnerControl.ClientRectangle.Width - 2 * offset, offset );
						break;
					}
					case Alignment.Left :
					{
						rect = new Rectangle( this.OwnerControl.ClientRectangle.X, 
							this.OwnerControl.ClientRectangle.Y + offset,
							offset, this.OwnerControl.ClientRectangle.Height - 2 * offset );
						break;
					}
					case Alignment.Right :
					{
						rect = new Rectangle( this.OwnerControl.ClientRectangle.Right - offset, 
							this.OwnerControl.ClientRectangle.Y + offset,
							offset, this.OwnerControl.ClientRectangle.Height - 2 * offset );
						break;
					}
				}
			}

			return rect;
		}

		
		/// <summary>
		/// Sets alignment of primitive.
		/// </summary>
		private void SetAlignmentPrimitive( Primitive primitive, Point mouseLocation )
		{
			if( primitive != null && mouseLocation != Point.Empty 
				&& OwnerControl != null )
			{
				Alignment primitiveAlignment = primitive.Alignment;

				if( GetLocationRect( Alignment.Top ).Contains( mouseLocation ) )
				{
					primitiveAlignment = Alignment.Top;
				} 
				else if( GetLocationRect( Alignment.Bottom ).Contains( mouseLocation ) )
				{
					primitiveAlignment = Alignment.Bottom;
				} 
				else if( GetLocationRect( Alignment.Left ).Contains( mouseLocation ) )
				{
					primitiveAlignment = Alignment.Left;
				}
				else if( GetLocationRect( Alignment.Right ).Contains( mouseLocation ) )
				{
					primitiveAlignment = Alignment.Right;
				}

				primitive.Alignment = primitiveAlignment;
			}
		}

		
		/// <summary>
		/// Sets position of primitive.
		/// </summary>
		private void SetPositionPrimitive( Primitive primitive, Point mouseLocation )
		{
			if( primitive != null && mouseLocation != Point.Empty 
				&& OwnerControl != null )
			{
				int minLocation = 0;
				int maxLocation = 0;
				int primitivePosition = primitive.Position;
				
				if( primitive.Alignment == Alignment.Top || primitive.Alignment == Alignment.Bottom )
				{
					maxLocation = this.OwnerControl.GetEndPosition( primitive.Alignment ).X;
					maxLocation -= primitive.Bounds.Width;
					minLocation = this.OwnerControl.GetStartPosition( primitive.Alignment ).X;
				}
				else
				{
					maxLocation = this.OwnerControl.GetEndPosition( primitive.Alignment ).Y;
					maxLocation -= primitive.Bounds.Height;
					minLocation = this.OwnerControl.GetStartPosition( primitive.Alignment ).Y;
				}

				switch( primitive.Alignment )
				{
					case Alignment.Top : 
					{
						if( mouseLocation.X > minLocation && mouseLocation.X < maxLocation )
						{
							primitivePosition = mouseLocation.X - primitive.Bounds.Left;
							primitivePosition += primitive.Position;
						}

						break;
					}
					case Alignment.Bottom : 
					{
						if( mouseLocation.X > minLocation && mouseLocation.X < maxLocation )
						{
							primitivePosition = mouseLocation.X - primitive.Bounds.Left;
							primitivePosition += primitive.Position;
						}

						break;
					}
					case Alignment.Left : 
					{
						if( mouseLocation.Y > minLocation && mouseLocation.Y < maxLocation )
						{
							primitivePosition = mouseLocation.Y - primitive.Bounds.Top;
							primitivePosition += primitive.Position;
						}

						break;
					}
					case Alignment.Right : 
					{
						if( mouseLocation.Y > minLocation && mouseLocation.Y < maxLocation )
						{
							primitivePosition = mouseLocation.Y - primitive.Bounds.Top;
							primitivePosition += primitive.Position;
						}

						break;
					}
				}

				primitive.Position = primitivePosition;
				primitive.Invalidate();
			}
		}

		
		/// <summary>
		/// Gets draged primitive.
		/// </summary>
		private Primitive GetDragPrimitive( int x, int y )
		{
			Primitive primitive = null;

			if( OwnerControl != null )
			{
				Point location = OwnerControl.PointToClient( new Point( x, y ) );

				foreach( Primitive item in OwnerControl.Primitives )
				{
					if( item.Bounds.Contains( location ) )
					{
						primitive = item;
						break;
					}
				}
			}

			return primitive;
		}

		
		private void OnBeforeSelectedPrimitive()
		{
			if( this.m_selectedPrimitve != null )
			{
				this.m_selectedPrimitve.Selected = false;
			}
		}

		private void OnAfterSelectedPrimitive()
		{
			if( this.m_selectedPrimitve != null )
			{
				this.m_selectedPrimitve.Selected = true;
			}
		}


		#endregion

		#region Class Event Handlers

		private void SelectionService_SelectionChanged( object sender, EventArgs e )
		{
			Primitive primitive = this.SelectionService.PrimarySelection as Primitive;

			if( primitive == null )
			{
				if( this.SelectionService.PrimarySelection != this.OwnerControl )
				{
					if( this.OwnerControl.IsControlHostControl( this.SelectionService.PrimarySelection as Control ) )
					{
						this.SelectedPrimitive = this.GetHostPrimitiveForDrag( this.SelectionService.PrimarySelection as Control );
						
						ArrayList arrComponent = new ArrayList();
						arrComponent.Add( this.SelectedPrimitive );
						this.SelectionService.SetSelectedComponents( arrComponent );
					}
					else
					{
						this.SelectedPrimitive = null;
					}
				}
			}
		}

		#endregion
	}	
}
