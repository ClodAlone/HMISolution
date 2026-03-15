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
using Syncfusion.Windows.Forms.Diagram;

namespace Syncfusion.Windows.Forms.Diagram
{
	/// <summary>
	/// The port bases class that use to connect end point to node.
	/// Collected in <see cref="Syncfusion.Windows.Forms.Diagram.EndPointCollection"/> collection in Node class.
	/// </summary>
	[ Serializable ]
	[ TypeConverter( typeof( ExpandableObjectConverter ) ) ]
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
	public class ConnectionPoint
		: AnchoringPrimitive
	{
		#region Class members
		private int m_nConnectionsLimit;
		private EndPointCollection m_connections;
		private GraphicsPath m_gpPortShape;
		private ConnectionPointType m_portType;
		private PortVisualType m_pvType = PortVisualType.XPort;
		private ConnectionPointSize m_cpSize = ConnectionPointSize.Medium;
		private string m_strName;
		private LineStyle m_styleLine;
		private FillStyle m_styleFill;
        private object m_tag = null;
        private bool m_bVisible = true;
        private int m_iSize = 9;
		#endregion
		
		#region Class initialize/finalize methods
		/// <summary>
		/// Initializes a new instance of the <see cref="ConnectionPoint"/> class.
		/// </summary>
		public ConnectionPoint()
		{
			this.VisualType = PortVisualType.XPort;
			this.ConnectionPointSize = ConnectionPointSize.Medium;
			//set no limitation count
			m_nConnectionsLimit = int.MaxValue;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="ConnectionPoint"/> class.
		/// </summary>
		/// <param name="container">The node container.</param>
		/// <param name="gpPortShape">The port path shape.</param>
		public ConnectionPoint( Node container, GraphicsPath gpPortShape )
			: base( container )
		{
			if( gpPortShape == null )
			{
				this.VisualType = PortVisualType.XPort;
				this.ConnectionPointSize = ConnectionPointSize.Medium;
			}
			else
			{
				this.GraphicsPath = gpPortShape;
			}

			//set no limitation count
			m_nConnectionsLimit = int.MaxValue;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="ConnectionPoint"/> class.
		/// </summary>
		/// <param name="src">The source instance.</param>
		public ConnectionPoint( ConnectionPoint src )
			: base( src )
		{
            if(src.m_gpPortShape != null)
                m_gpPortShape = ( GraphicsPath ) src.m_gpPortShape.Clone();
			m_pvType = src.VisualType;
			m_cpSize = src.ConnectionPointSize;
            if (src.m_styleLine != null)
                m_styleLine = (LineStyle)src.LineStyle.Clone();
			m_nConnectionsLimit = src.m_nConnectionsLimit;
            m_tag = src.m_tag;
			m_strName = src.m_strName;
            if (src.m_styleFill != null)
                m_styleFill = (FillStyle)src.FillStyle.Clone();
			m_portType = src.m_portType;
            m_iSize = src.m_iSize;
            m_bVisible = src.m_bVisible;
      }
		/// <summary>
		/// Initializes a new instance of the <see cref="ConnectionPoint"/> class.
		/// </summary>
		/// <param name="info">The serialization info.</param>
		/// <param name="context">The serialization context.</param>
		public ConnectionPoint( SerializationInfo info, StreamingContext context )
			: base( info, context )
		{
            m_pvType = PortVisualType.XPort;

            m_styleLine = new LineStyle();
            m_styleLine.LineColor = Color.Blue;
            m_styleLine.LineWidth = 0;

            m_styleFill = new FillStyle();
            m_styleFill.Color = Color.Transparent;


            bool connectionPointSizeEntry = false;
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
			{
                    case "pvType":
				m_pvType = (PortVisualType)info.GetValue("pvType", typeof(PortVisualType));
                        break;
                    case "name":
						m_strName = info.GetString("name");
                        break;
                    case "lineStyle":
                        m_styleLine = (LineStyle)info.GetValue("lineStyle", typeof(LineStyle));
                        break;
                    case "fillStyle":
                        m_styleFill = (FillStyle)info.GetValue("fillStyle", typeof(FillStyle));
                        break;
                    case "cpsSize":
                        this.ConnectionPointSize = (ConnectionPointSize)info.GetValue("cpsSize", typeof(ConnectionPointSize));
                        connectionPointSizeEntry = true;
                        break;
                    case "ConnectionsLimit":
                        m_nConnectionsLimit = info.GetInt32("ConnectionsLimit");
                        break;
                    case "connections":
                        m_connections = (EndPointCollection)info.GetValue("connections", typeof(EndPointCollection));
                        m_connections.Owner = this;
                        break;
                    case "tag":
                        m_tag = (object)info.GetValue("tag", typeof(object));
                        break;
					case "portType":
						m_portType = (ConnectionPointType)info.GetValue("portType", typeof(ConnectionPointType));
						break;
                    case "size":
                        m_iSize = info.GetInt32("size");
                        break;
                    case "visible":
                        m_bVisible = info.GetBoolean("visible");
                        break;
                
			}
			}

            
			switch( m_pvType )
			{
				case PortVisualType.Custom:
					PointF[] pathPoints = ( PointF[] ) info.GetValue( "pathPoints", typeof( PointF[] ) );
					byte[] pathTypes = ( byte[] ) info.GetValue( "pathTypes", typeof( byte[] ) );
					m_gpPortShape = new GraphicsPath( pathPoints, pathTypes );
					break;
				default:
					this.VisualType = m_pvType;
					break;
			}

            if (!connectionPointSizeEntry)
				this.ConnectionPointSize = ConnectionPointSize.Medium;
			}
		#endregion

		#region Class properties
        /// <summary>
        /// Gets or sets the unique connection point full name.
        /// </summary>
        /// <value>The unique port full name.</value>
        [Description("Connection Point's full name")]
        public string FullName
        {
            get
            {
                if (this.Container == null)
                    return this.Name;
                else
                    return this.Container.FullName + "." + this.Name;
            }          
        }
		/// <summary>
		/// Gets or sets the unique connection point name.
		/// </summary>
		/// <value>The unique port name.</value>
        [ Description( "Connection Point's name" ) ]
        public string Name
        {
            get { return m_strName; }
            set
            {
                if( m_strName != value && OnPropertyChanging( DPN.Name, value ) )
                {
                    // make history entry
                    RecordPropertyChanged( DPN.Name );
                    // set new value
                    m_strName = value;
                    // raise property changed event
                    OnPropertyChanged( DPN.Name );
                }
            }
        }
		/// <summary>
		/// Gets the collection of connected EndPoint's.
		/// </summary>
		/// <value>The EndPoint collection.</value>
		[ Browsable( false ) ]
		public EndPointCollection Connections
		{
			get
			{
				if( m_connections == null )
				{
					m_connections = new EndPointCollection( this );
				}
				
				return m_connections;
			}
		}
		/// <summary>
		/// Gets or sets the <see cref="ConnectionPointType"/> type of the connection point.
		/// Determine if port can connect to <see cref="HeadEndPoint"/> or <see cref="TailEndPoint"/> or both.
		/// </summary>
		/// <value>The type of the connection point.</value>
		[ Browsable( true ) ]
		[ DefaultValue( ConnectionPointType.IncomingOutgoing ) ]
		[ Description( "Specifies type of connection connection point can be connected with." ) ]
		public ConnectionPointType ConnectionPointType
		{
			get{ return m_portType; }
			set
			{
				if( m_portType != value )
				{
					m_portType = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets the connections limit. Determine max connections count.
		/// </summary>
		/// <value>The connections limit.</value>
		[ Browsable( true ) ]
		[ DefaultValue( int.MaxValue ) ]
		[ Description( "Defines connections limit." ) ]
		public int ConnectionsLimit
		{
			get{ return m_nConnectionsLimit; }
			set
			{
				if( m_nConnectionsLimit != value )
				{
					m_nConnectionsLimit = value;
				}
			}
		}
		/// <summary>
		/// User-defined data associated with the object.
		/// </summary>
		/// <value>The <see cref="System.Object"/>.</value>
        [Browsable(true)]
        [Description("User-defined data associated with the object.")]
        public object Tag
        {
            get { return m_tag; }
            set
            {
                if (m_tag != value)
                {
                    m_tag = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the connection point size.
        /// </summary>
        /// <value>The size of the port.</value>
        [Description("Connection Point's size")]
        public int Size
        {
            get { return m_iSize; }
            set 
            {
                if (value != m_iSize && value >= 6 && OnPropertyChanging(DPN.PortSize, value))
                {
                    m_iSize = value;
                    UpdatePortSizeType();                    
                    this.SetSizedGraphicsPath(m_pvType, m_cpSize);                    
                    // raise property changed event
                    OnPropertyChanged(DPN.PortSize);
                }
            }
        }
		/// <summary>
		/// VisualType contains <see cref="PortVisualType"/> for current ConnectionPoint.
		/// </summary>
		/// <value>The type of the visual shape.</value>
	    [ Browsable( true ) ]
		[ DefaultValue( PortVisualType.XPort ) ]
		[ Description( "Specifies connection point visual representation." ) ]
		public PortVisualType VisualType
		{
			get { return m_pvType; }
            set
            {
                SetSizedGraphicsPath(value, m_cpSize);
               
                if (m_pvType != value && OnPropertyChanging(DPN.PortVisualType, value))
                {
                    m_pvType = value;
                    // raise property changed event
                    OnPropertyChanged(DPN.PortVisualType);
                }
            }
		}
		/// <summary>
		/// Gets or sets the graphics path. Return cloned graphics path instance.
		/// </summary>
		/// <value>The graphics path.</value>
		[ Browsable( false ) ]
		public GraphicsPath GraphicsPath
		{
            get
            {
                if (m_gpPortShape != null)
                {
                    return (GraphicsPath)m_gpPortShape.Clone();
                }
                else
                {
                    return new GraphicsPath();
                }
            }
			set
			{
				if( value == null ) throw new ArgumentNullException( "value" );
				
				if( this.VisualType != PortVisualType.Custom )
				{
					this.VisualType = PortVisualType.Custom;
				}
				
				m_gpPortShape = ( GraphicsPath ) value.Clone();
			}
		}
		/// <summary>
		/// Size of the current ConnectionPoint. Calculated from port path shape.
		/// </summary>
		/// <value>The size of the connection point.</value>
		[Browsable(true)]
		[DefaultValue(ConnectionPointSize.Medium)]
		[Description("Specifies connection point size.")]
		public ConnectionPointSize ConnectionPointSize
		{
			get { return m_cpSize; }
			set
			{
                if (m_cpSize != value && OnPropertyChanging(DPN.ConnectionPointSize, value))
                {
                    //RectangleF bounds = m_gpPortShape.GetBounds();
                    m_cpSize = value;
                    UpdateSize();  
                    this.SetSizedGraphicsPath(m_pvType, value);
                    
                    // raise property changed event
                    OnPropertyChanged(DPN.ConnectionPointSize);
                }

			}
		}
		/// <summary>
		/// Line drawing properties for this connection point.
		/// </summary>
		/// <value>The line style.</value>
		/// <remarks>
		/// The line style determines the configuration of the pen used to
		/// render lines.
		/// <seealso cref="Syncfusion.Windows.Forms.Diagram.LineStyle"/>
		/// </remarks>
		[Browsable(true)]
		[TypeConverter(typeof(LineStyleConverter))]
		[Category("Appearance")]
		[Description("Properties of the pen used for drawing lines.")]
		public LineStyle LineStyle
		{
			get
			{
				if (m_styleLine == null)
				{
					m_styleLine = new LineStyle();
					m_styleLine.LineColor = Color.Blue;
					m_styleLine.LineWidth = 0;
				}

				return m_styleLine;
			}
		}
		/// <summary>
		/// Properties used to fill the interior.
		/// </summary>
		/// <value>The fill style.</value>
		/// <remarks>
		/// 	<para>
		/// The fill style is used to create brushes for painting interior
		/// </para>
		/// 	<seealso cref="Syncfusion.Windows.Forms.Diagram.FillStyle"/>
		/// </remarks>
		[Browsable(true)]
		[TypeConverter(typeof(FillStyleConverter))]
		[Category("Appearance")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("Properties of the brush used to fill interior regions.")]
		public FillStyle FillStyle
		{
			get
			{
				if (m_styleFill == null)
				{
					m_styleFill = new FillStyle();
					m_styleFill.Color = Color.Transparent;
				}

				return m_styleFill;
			}
		}
        [Browsable(true)]
        [DefaultValue(true)]
        [Category("Appearance")]
        [Description("Visibility of a port")]
        public bool Visible
        {
            get
            {
                return m_bVisible;
            }
            set
            {
                if (m_bVisible != value)
                    m_bVisible = value;
            }
        }
		#endregion
		
		#region Class utility methods
		/// <summary>
		/// Determines whether this instance can connect the specified end point.
		/// </summary>
		/// <param name="endPoint">The end point.</param>
		/// <returns>
		/// 	<c>true</c> if this instance can connect the specified end point; otherwise, <c>false</c>.
		/// </returns>
		public bool CanConnect( EndPoint endPoint )
		{
			bool bSuccess = ( endPoint != null );
			
			if( bSuccess )
				bSuccess = CheckType( endPoint );
			
			if( bSuccess && endPoint.Container != null && this.Container != null  )
			{
                bSuccess = (endPoint.Container.Name != this.Container.Name && !string.IsNullOrEmpty(this.Container.Name));
			}

			// check connections limit
			if( bSuccess )
				bSuccess = CheckConnectionsLimit();
			
			return bSuccess;
		}
		/// <summary>
		/// Tries the connect to specified end point.
		/// </summary>
		/// <param name="endPoint">The end point.</param>
		/// <returns>
		/// 	<c>true</c> if this instance can connect the specified end point; otherwise, <c>false</c>.
		/// </returns>
		public bool TryConnect( EndPoint endPoint )
		{
			if( endPoint == null )
				throw new ArgumentNullException();
			
			bool bSuccess = CanConnect( endPoint );
			
			if( bSuccess )
			{
				Model model = (this.Container != null) ? this.Container.Root : null;

              	if (model != null)
					model.BeginUpdate();


				// set new port and synchronize to port
				endPoint.Port = this;
				int result = this.Connections.Add( endPoint );
                if (result == -1)
                    endPoint.Port = null;

				if (model != null)
					model.EndUpdate();
              
			}

			return bSuccess;
		}
		/// <summary>
		/// Connect the specified end point.
		/// </summary>
		/// <param name="endPoint">The end point.</param>
		public void Connect( EndPoint endPoint )
		{
			if( endPoint == null )
				throw new ArgumentNullException();

			if( !CheckType( endPoint ) )
				throw new InvalidOperationException( "port can not connect with EndPont of this type" );
			
			if( !CheckConnectionsLimit() )
				throw new InvalidOperationException( "no more connections can be accepted" );
			
			// reset edit style
			bool bRotate = true;
            if (CanConnect(endPoint))
            {
                if (endPoint.Container != null)
                {
                    bRotate = endPoint.Container.EditStyle.AllowRotate;
                    endPoint.Container.EditStyle.AllowRotate = true;
                }

                this.Connections.Add(endPoint);
                endPoint.Port = this;

                // restore edit style
                if (endPoint.Container != null)
                {
                    endPoint.Container.EditStyle.AllowRotate = bRotate;
                }
            }
		}
		/// <summary>
		/// Disconnect from the specified end point.
		/// </summary>
		/// <param name="endPoint">The end point.</param>
		public void Disconnect( EndPoint endPoint )
		{
			if( endPoint == null )
				throw new ArgumentNullException();
			
			if( this.Connections.Contains( endPoint ) )
			{
				this.Connections.Remove( endPoint );
                Model model = (this.Container != null) ? this.Container.Root : null;

                if (model != null && model.EventSink != null)
                    model.EventSink.Pause();

                if(!this.Connections.Contains(endPoint))
                    endPoint.Port = null;

                if (model != null && model.EventSink != null)
                    model.EventSink.Resume();
			}
		}
		/// <summary>
		/// Disconnects from all connected endpoints.
		/// </summary>
		public void DisconnectAll()
		{
            if (!this.Connections.IsEmpty)
            {
                Model model = (this.Container != null) ? this.Container.Root : null;

                if (model != null && model.EventSink != null)
                    model.EventSink.Pause();

                foreach (EndPoint endPoint in this.Connections)
                {
                    endPoint.Port = null;
                }

                if (model != null && model.EventSink != null)
                    model.EventSink.Resume();

                this.Connections.Clear();
            }
		}
		#endregion
		
		#region Class overrides
		/// <summary>
		/// Calling when primitive position is changed.
		/// </summary>
		/// <param name="fOffsetX">The offset by X axis.</param>
		/// <param name="fOffsetY">The offset by Y axis.</param>
		protected override void PositionChange( float fOffsetX, float fOffsetY )
		{
			base.PositionChange( fOffsetX, fOffsetY );

			if ( this.Container != null )
			{
				Model model = this.Container.Root;

				if ( model != null )
				{
					// update connections
					foreach ( EndPoint connection in this.Connections )
					{
						model.LinkManager.SynchronizeEndPoint( connection );
					}
				}
			}
		}
		/// <summary>
		/// Gets the name of the property container.
		/// </summary>
		/// <returns></returns>
		protected override string GetPropertyContainerName()
		{
			return null;
		}
		/// <summary>
		/// Gets the object data to serialize instance.
		/// </summary>
		/// <param name="info">The serialization info.</param>
		/// <param name="context">The serialization context.</param>
		protected override void GetObjectData( SerializationInfo info, StreamingContext context )
		{
			base.GetObjectData( info, context );
			
			// Since GraphicsPath is node Serializable -- serialize
			// its PathPoints and PathTypes
			info.AddValue( "pvType", m_pvType);
		    info.AddValue( "name", m_strName );
			info.AddValue("cpsSize", m_cpSize);
			info.AddValue("lineStyle", m_styleLine);
			info.AddValue("fillStyle", m_styleFill);
			info.AddValue("portType", m_portType);
			
			if (m_pvType == PortVisualType.Custom)
			{
				info.AddValue( "pathPoints", m_gpPortShape.PathPoints );
				info.AddValue( "pathTypes", m_gpPortShape.PathTypes );
			}

			info.AddValue( "ConnectionsLimit", m_nConnectionsLimit );
			
			if( m_connections != null && m_connections.Count > 0 )
				info.AddValue( "connections", m_connections );
            info.AddValue("tag", m_tag);
            info.AddValue("size", m_iSize);
            info.AddValue("visible", m_bVisible);
		}
		/// <summary>
		/// Renders the primitive object to specified graphics.
		/// </summary>
		/// <param name="gfx">Graphics to draw on.</param>
		protected override void Render( Graphics gfx )
		{
            if (!this.Visible || gfx == null)
                return;
			// calc port position
			PointF ptPrimitivePosition = GetPosition();
			// exclude rotation
			float fParentsRotation = HandlesHitTesting.GetParentsRotation( this.Container );
			Matrix matrixParentRotate = new Matrix( 1, 0, 0, 1, ptPrimitivePosition.X, ptPrimitivePosition.Y );
			matrixParentRotate.RotateAt( -fParentsRotation - this.Container.RotationAngle, ptPrimitivePosition, MatrixOrder.Append );

			// save graphics state
			GraphicsState stateSave = gfx.Save();
			gfx.PixelOffsetMode = PixelOffsetMode.Half;
			
			gfx.MultiplyTransform( matrixParentRotate );

			// draw port
			using( Pen penPortOutline = this.LineStyle.CreatePen())
			{
				GraphicsPath path = this.GraphicsPath;
				Matrix matrix = new Matrix(1f, 0, 0, 1f, 0, 0);
				path.Transform(matrix);

				using (Brush fillBrush = FillStyle.CreateBrush(gfx, path.GetBounds()))
				{
					gfx.FillPath(fillBrush, path);
					gfx.DrawPath(penPortOutline, path);
				}
			}

			// restore graphics state
			gfx.Restore( stateSave );
		}
		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>
		/// A new object that is a copy of this instance.
		/// </returns>
		public override object Clone()
		{
			return new ConnectionPoint( this );
		}
		/// <summary>
		/// Updates the service references from service provider.
		/// </summary>
		/// <param name="provider">The service provider.</param>
		public override void UpdateServiceReferences(IServiceReferenceProvider provider)
		{
			base.UpdateServiceReferences ( provider );

			this.Connections.UpdateServiceReferences( provider );

            if(this.FillStyle != null)
                this.FillStyle.UpdateServiceReferences(provider);

            //if (provider == null)
            //{
            //    this.m_styleFill = null;
            //}
		}
		#endregion
		
		#region Class helper methods
        private void UpdatePortSizeType()
        {
            //check port Size and update the port size type
            if (this.Size == 6)
                this.m_cpSize = Diagram.ConnectionPointSize.Small;
            else if (this.Size == 9)
                this.m_cpSize = Diagram.ConnectionPointSize.Medium;
            else if (this.Size == 12)
                this.m_cpSize = Diagram.ConnectionPointSize.Large;
            else
                this.m_cpSize = Diagram.ConnectionPointSize.Custom;
        }

        private void UpdateSize()
        {
            //check port size type and update the Size
            if (m_cpSize == ConnectionPointSize.Large)
                m_iSize = 12;
            else if (m_cpSize == ConnectionPointSize.Medium)
                m_iSize = 9;
            else if (m_cpSize == ConnectionPointSize.Small)
                m_iSize = 6;
        }

		public bool CheckType( EndPoint endPoint )
		{
			bool bSuccess = true;
			
			// check port type
			if( endPoint is HeadEndPoint && (this.ConnectionPointType == Diagram.ConnectionPointType.Outgoing || 
                this.ConnectionPointType == ConnectionPointType.Reject))
			{
				bSuccess = false;
			}
			
			if( endPoint is TailEndPoint && (this.ConnectionPointType == Diagram.ConnectionPointType.Incoming ||
                this.ConnectionPointType == ConnectionPointType.Reject))
			{
				bSuccess = false;
			}           

			return bSuccess;
		}
		private bool CheckConnectionsLimit()
		{

		    // return this.Connections.Count < this.ConnectionsLimit;
              return this.Connections.Count <= this.ConnectionsLimit;
	}
		
		private void SetSizedGraphicsPath(PortVisualType pvType, ConnectionPointSize cpSize )
		{
			float scale = (float)this.Size/6;
            //switch (cpSize)
            //{
            //    case ConnectionPointSize.Large:
            //        scale = 2f;
            //        break;
            //    case ConnectionPointSize.Medium:
            //        scale = 1.5f;
            //        break;
            //    default:
            //        break;
            //}

			switch (pvType)
			{
				case PortVisualType.CirclePort:
					m_gpPortShape = (GraphicsPath)PortVisuals.CirclePort.Clone();
					break;
				case PortVisualType.SquarePort:
					m_gpPortShape = (GraphicsPath)PortVisuals.SquarePort.Clone();
					break;
				case PortVisualType.Triangleport:
					m_gpPortShape = (GraphicsPath)PortVisuals.TrianglePort.Clone();
					break;
				case PortVisualType.XPort:
					m_gpPortShape = (GraphicsPath)PortVisuals.XPort.Clone();
					break;
				case PortVisualType.RhombPort:
					m_gpPortShape = (GraphicsPath)PortVisuals.RhombPort.Clone();
					break;
                case PortVisualType.Custom:
                    m_gpPortShape = this.GraphicsPath;
                    break;
			}

			m_gpPortShape.Transform(new Matrix(scale, 0, 0, scale, 0, 0));

		}
		#endregion
	}
}