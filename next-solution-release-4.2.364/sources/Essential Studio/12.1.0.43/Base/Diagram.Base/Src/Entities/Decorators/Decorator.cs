#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
//  Author: Jeff Boenig
//
#endregion

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Syncfusion.Windows.Forms.Diagram
{
	/// <summary>
	/// Base class for line decorators.
	/// </summary>
	/// <remarks>
	/// <para>
	/// A line decorator is an object that adorns an endpoint of a line or other shape.
	/// </para>
	/// </remarks>
	[ Serializable ]
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
	public abstract class Decorator
		: PropertyContainer
	{
		#region Class members
		private GraphicsPath m_pathShape;
		private SizeF m_szDecorator;
		private DecoratorShape m_decoratorShape;
		private Node m_nodeContainer;
		private LineStyle m_styleLine;
		private FillStyle m_styleFill;
		private bool m_bClosedPath;
		private UpdateCallback m_pathChangedCallback;
		#endregion

		#region Class initialize/finalize methods
		/// <summary>
		/// Default constructor.
		/// </summary>
		public Decorator()
		{}
		/// <summary>
		/// Construct a line decorator given a graphics path for the visual.
		/// </summary>
		/// <param name="grfxPath">Visual representation of the decorator.</param>
		public Decorator( GraphicsPath grfxPath )
		{
			Load( grfxPath );
		}
		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="src">Source object to copy.</param>
		public Decorator( Decorator src )
		{
			if( src.m_decoratorShape != DecoratorShape.None
                && ( src.m_decoratorShape != Diagram.DecoratorShape.Custom && src.m_pathShape != null ) )
				m_pathShape = ( GraphicsPath ) src.m_pathShape.Clone();
			
			m_bClosedPath = src.m_bClosedPath;
            m_szDecorator = src.Size;
			m_decoratorShape = src.m_decoratorShape;
			m_styleFill = ( FillStyle )src.FillStyle.Clone();
			m_styleLine = ( LineStyle )src.LineStyle.Clone();
		}
		/// <summary>
		/// Serialization constructor for line decorators.
		/// </summary>
		/// <param name="info">Serialization state information.</param>
		/// <param name="context">Streaming context information.</param>
		protected Decorator( SerializationInfo info, StreamingContext context )
		{
            PointF[] pathPts = null;
            byte[] pathTypes = null;

            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "decoratorShape":
			m_decoratorShape = ( DecoratorShape )info.GetValue( "decoratorShape", typeof( DecoratorShape ) );
                        break;
                    case "pathPoints":
                        pathPts = ( PointF[] ) info.GetValue( "pathPoints", typeof( PointF[] ) );
                        break;
                    case "pathTypes":
                        pathTypes = (byte[])info.GetValue("pathTypes", typeof(byte[]));
                        break;
                    case "lineStyle":
                        m_styleLine = (LineStyle)info.GetValue("lineStyle", typeof(LineStyle));
                        break;
                    case "fillStyle":
                        m_styleFill = (FillStyle)info.GetValue("fillStyle", typeof(FillStyle));
                        break;
                }
            }
            if (m_decoratorShape != DecoratorShape.None && pathPts.Length>0 && pathTypes.Length>0)
			{
				// create GraphicsPath
				m_pathShape = new GraphicsPath( pathPts, pathTypes );
				// init size
				m_szDecorator = m_pathShape.GetBounds().Size;
				// init bClosedPath
				m_bClosedPath = IsClosedPath( m_pathShape );
			}
		}
		#endregion

		#region Class Properties
		/// <summary>
		/// Gets or sets the decorator shape.
		/// </summary>
		/// <value>The decorator shape.</value>
		[ Browsable( true ) ]
		[ DefaultValue( DecoratorShape.None ) ]
		[ Description( "Defines decorator visual representation's shape." ) ]
		public DecoratorShape DecoratorShape
		{
			get{ return m_decoratorShape; }
			set
			{
				if( m_decoratorShape != value && OnPropertyChanging( DPN.DecoratorShape, value ) )
				{
					// make history record
					RecordPropertyChanged( DPN.DecoratorShape );
					// assign new value
					m_decoratorShape = value;
					
					if( value == DecoratorShape.None )
					{
						m_bClosedPath = false;
						m_pathShape = null;
					}
					else if( value != DecoratorShape.Custom )
					{
						m_pathShape = DecoratorFactory.CreateDecorator( value );
					}

					if( m_pathShape != null )
					{
						m_bClosedPath = IsClosedPath( m_pathShape );
						m_szDecorator = m_pathShape.GetBounds().Size;
					}
                    
					if( m_pathChangedCallback != null )
					{
						m_pathChangedCallback();
					}
					
					// raise property changed event
					OnPropertyChanged( DPN.DecoratorShape );
				}
			}
		}
		/// <summary>
		/// Gets the graphics path.
		/// </summary>
		/// <value>The graphics path.</value>
		[ Browsable( false ) ]
		public GraphicsPath GraphicsPath
		{
			get { return ( m_pathShape == null )? null:( GraphicsPath ) m_pathShape.Clone(); }
		}
		/// <summary>
		/// Reference to the container node the decorator is attached to.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This can be any type of node that supports the
		/// <see cref="Syncfusion.Windows.Forms.Diagram.IEndPointContainer"/> interface.
		/// </para>
		/// <seealso cref="Syncfusion.Windows.Forms.Diagram.IEndPointContainer"/>
		/// </remarks>
		[ Browsable( false ) ]
		[ DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ) ]
		public Node Container
		{
			get { return m_nodeContainer; }
			set
			{
				if( m_nodeContainer != value )
				{
					m_nodeContainer = value;

					if( m_nodeContainer != null )
					{
						Delegate del1 = Delegate.CreateDelegate( typeof( UpdateCallback )
						                                         , m_nodeContainer, "UpdateHelperRegion" );

						Delegate del2 = Delegate.CreateDelegate( typeof( UpdateCallback )
						                                         , m_nodeContainer, "UpdateRefreshRect" );

						m_pathChangedCallback = ( UpdateCallback ) Delegate.Combine( del1, del2 );
					}
					else
					{
						m_pathChangedCallback = null;
					}
				}
			}
		}
		/// <summary>
		/// Line drawing properties for this decorator.
		/// </summary>
		/// <remarks>
		/// The line style determines the configuration of the pen used to
		/// outline decorator.
		/// <seealso cref="Syncfusion.Windows.Forms.Diagram.LineStyle"/>
		/// </remarks>
		[ Browsable( true ) ]
		[ TypeConverter( typeof( LineStyleConverter ) ) ]
		[ Category( "Appearance" ) ]
		[ DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ) ]
		[ Description( "Properties of the pen used for decorator outline." ) ]
		public LineStyle LineStyle
		{
			get
			{
				if( m_styleLine == null )
				{
					m_styleLine = new LineStyle();
				}

				return m_styleLine;
			}
		}
		/// <summary>
		/// Properties for creating a brush to fill the decorator with.
		/// </summary>
		[ Browsable( true ) ]
		[ TypeConverter( typeof( FillStyleConverter ) ) ]
		[ Category( "Appearance" ) ]
		[ DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ) ]
		[ Description( "Properties of the brush used to fill decorator's interior." ) ]
		public FillStyle FillStyle
		{
			get
			{
				if( m_styleFill == null )
				{
					m_styleFill = new FillStyle();
				}

				return m_styleFill;
			}
		}
		/// <summary>
		/// Width and height of the decorator.
		/// </summary>
		[ Browsable( true ) ]
		[ TypeConverter( typeof( SizeFConverter ) ) ]
		[ DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ) ]
		[ Category( "Appearance" ) ]
		[ Description( "Horizontal size of the arrow decorator." ) ]
		public SizeF Size
		{
			get{ return m_szDecorator; }
			set
			{
				if( m_szDecorator != value && OnPropertyChanging( DPN.Size, value ) )
				{
					if( m_pathShape != null )
					{
						Matrix mtxScale = new Matrix();

						float fScaleWidthFactor = ( m_szDecorator.Width != 0 ) ? value.Width / m_szDecorator.Width : 1.0f;
						float fScaleHeightFactor = ( m_szDecorator.Height != 0 ) ? value.Height / m_szDecorator.Height : 1.0f;

						mtxScale.Scale( fScaleWidthFactor, fScaleHeightFactor, MatrixOrder.Append );
				
						m_pathShape.Transform( mtxScale );
					}

					// make history record
					RecordPropertyChanged( DPN.Size );
					
					m_szDecorator = value;

					// tell container node to update its refresh rect
					if( m_pathChangedCallback != null )
					{
						m_pathChangedCallback();
					}

					// raise property changed event
					OnPropertyChanged( DPN.Size );
				}
			}
		}
		/// <summary>
		/// Gets a value indicating whether this instance is path closed.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is path closed; otherwise, <c>false</c>.
		/// </value>
		protected bool IsPathClosed
		{
			get{ return m_bClosedPath; }
		}
		#endregion

		#region Class public methods
		/// <summary>
		/// Load the endpoint decorator with a GraphicsPath.
		/// </summary>
		/// <param name="grfxPath">GraphicsPath to load.</param>
		/// <returns>True if successful; otherwise False.</returns>
		public bool Load( GraphicsPath grfxPath )
		{
			bool bSuccess = false;

			if( grfxPath != null )
			{
				GraphicsPath gpTemp = ( GraphicsPath )grfxPath.Clone();

				// make graphipath bounds begin in ( 0, 0 )
				RectangleF rectPathBounds = gpTemp.GetBounds();
                // update path
				Matrix matrix = new Matrix( 1, 0, 0, 1, -rectPathBounds.X, 0 );
				gpTemp.Transform( matrix );
				// set new graphics path
				m_pathShape = gpTemp;
				
				// set decorator size
				m_szDecorator = rectPathBounds.Size;
				m_bClosedPath = IsClosedPath( m_pathShape );
				// set decorator type to custom
				m_decoratorShape = DecoratorShape.Custom;
				// update container's hittesting region
				if( m_pathChangedCallback != null )
				{
					m_pathChangedCallback();
				}
				
				bSuccess = true;
			}

			return bSuccess;
		}

		/// <summary>
		/// Renders the line decorator onto a System.Drawing.Graphics object.
		/// </summary>
		/// <param name="grfx">Graphics context to render onto.</param>
		public virtual void Draw( Graphics grfx )
		{
			if ( m_pathShape != null)
			{
				if( this.IsPathClosed )
				{
					RectangleF rectFillBounds = new RectangleF( PointF.Empty, this.Size );
					using( Brush brush = this.FillStyle.CreateBrush( grfx, rectFillBounds ) )
					{
						grfx.FillPath( brush, m_pathShape );
					}
				}
				
				using( Pen pen = this.LineStyle.CreatePen() )
				{
					grfx.DrawPath( pen, m_pathShape );
				}
			}
		}
		#endregion

		#region Class overrides
		/// <summary>
		/// Populates a SerializationInfo with the data needed to
		/// serialize the target object.
		/// </summary>
		/// <param name="info">SerializationInfo object to populate.</param>
		/// <param name="context">Destination streaming context.</param>
		protected override void GetObjectData( SerializationInfo info, StreamingContext context )
		{
			base.GetObjectData( info, context );

			if( this.m_pathShape != null && m_decoratorShape != DecoratorShape.None )
			{
				info.AddValue( "pathPoints", this.m_pathShape.PathPoints );
				info.AddValue( "pathTypes", this.m_pathShape.PathTypes );
			}

			info.AddValue( "lineStyle", m_styleLine );
			info.AddValue( "fillStyle", m_styleFill );
			info.AddValue( "decoratorShape", m_decoratorShape );
		}

        /// <summary>
        /// Gets the name of the property container.
        /// </summary>
        /// <returns></returns>
		protected override string GetPropertyContainerName()
		{
			return "EndPontDecorator";
		}

        /// <summary>
        /// Updates the service references.
        /// </summary>
        /// <param name="provider">The provider.</param>
		public override void UpdateServiceReferences( IServiceReferenceProvider provider )
		{
			base.UpdateServiceReferences( provider );

			this.FillStyle.UpdateServiceReferences( this );
			this.LineStyle.UpdateServiceReferences( this );
		}

        /// <summary>
        /// Get the service reference from provider.
        /// </summary>
        /// <param name="typeHandle">The type handle.</param>
        /// <returns></returns>
        public override object ProvideServiceReference(RuntimeTypeHandle typeHandle)
        {
            object objToReturn = base.ProvideServiceReference(typeHandle);

            if (typeHandle.Equals(typeof(IPropertyContainer).TypeHandle))
            {
                objToReturn = this;
            }

            return objToReturn;
        }

        /// <summary>
        /// Gets the container of the property by name.
        /// </summary>
        /// <param name="strPropertyName">Name of the property.</param>
        /// <returns></returns>
		public override object GetPropertyContainerByName( string strPropertyName )
		{
			object objToReturn = base.GetPropertyContainerByName( strPropertyName );

			if( strPropertyName == "FillStyle" )
			{
				objToReturn = this.FillStyle;
			}
			else if( strPropertyName == "LineStyle" )
			{
				objToReturn = this.LineStyle;
			}

			return objToReturn;
		}
		#endregion
		
		#region Class helper methods
		private bool IsClosedPath( GraphicsPath path )
		{
			if( path == null ) throw new ArgumentNullException( "path" );
				
			bool bClosePath = false;
			
			if( path.PathData.Points.Length > 0 )
			{
				// get path points types
				byte[] pathTypes = path.PathTypes;
			
				if( ( pathTypes[ pathTypes.Length - 1 ] & 128 ) == 128 )
				{
					bClosePath = true;
				}
			}
			
			return bClosePath;
		}
		#endregion
	}
}