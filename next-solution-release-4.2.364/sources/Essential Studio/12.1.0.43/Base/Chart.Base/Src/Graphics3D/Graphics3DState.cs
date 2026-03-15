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

namespace Syncfusion.Windows.Forms.Chart
{
	/// <summary>
	/// Provide the representation settings of <see cref="Graphics3D"/>.
	/// </summary>
	public class Graphics3DState
	{
      #region Members
    private bool m_autoPerspective = true;
    private bool m_perpective = true;
    private bool m_light = true;
    private Vector3D m_lightPosition = new Vector3D( 0, 0, 1 );
    private float m_lightCoef = 16;
    private float m_zDist = 200;
    #endregion
 
      #region Events
		/// <summary>
		/// Occurs when settings is changed.
		/// </summary>
    public event EventHandler Changed;
    #endregion

    #region Properties
		/// <summary>
		/// Gets or sets the light direction.
		/// </summary>
		/// <value>The light direction.</value>
    public Vector3D LightPosition
    {
      get
      {
        return m_lightPosition;
      }

      set
      {
        if( m_lightPosition != value )
        {
          m_lightPosition = value;
          RaiseChanged();
        }
      }
    } 

		/// <summary>
		/// Gets or sets the light coefficient.
		/// </summary>
		/// <value>The light coefficient.</value>
    [ ChartTemplate( ChartTemplateSet.Simple ), DefaultValue( 16 ) ]
    public float LightCoeficient
    {
      get
      {
        return m_lightCoef;
      }

      set
      {
        if( m_lightCoef != value )
        {
          m_lightCoef = value;
          RaiseChanged();
        }
      }
    }
        
        /// <summary>
		/// Gets or sets a value indicating whether perspective is enabled.
		/// </summary>
		/// <value><c>True</c> If perspective is enabled; otherwise, <c>false</c>.</value>
    [ ChartTemplate( ChartTemplateSet.Simple ), DefaultValue( true ) ]
    public bool Perspective
    {
      get
      {
        return m_perpective;
      }

      set
      {
        if( m_perpective != value )
        {
          m_perpective = value;
          RaiseChanged();
        }
      }
    }

		/// <summary>
		/// Gets or sets a value indicating whether perspective is computed automatically.
		/// </summary>
		/// <value><c>True</c> If perspective is computed automatically; otherwise, <c>false</c>.</value>
    [ ChartTemplate( ChartTemplateSet.Simple ), DefaultValue( true ) ]
    public bool AutoPerspective
    {
      get
      {
        return m_autoPerspective;
      }

      set
      {
        if( m_autoPerspective != value )
        {
          m_autoPerspective = value;
          RaiseChanged();
        }
      }
    }

		/// <summary>
		/// Gets or sets a value indicating whether light is enabled.
		/// </summary>
		/// <value><c>True</c> If light is enabled; otherwise, <c>false</c>.</value>
    [ ChartTemplate( ChartTemplateSet.Simple ), DefaultValue( true ) ]
    public bool Light
    {
      get
      {
        return m_light;
      }

      set
      {
        if ( m_light != value )
        {
          m_light = value;
          RaiseChanged();
        }
      }
    }

		/// <summary>
		/// Gets or sets the distance from eye to the chart. This value is used for computing of perspective.
		/// </summary>
		/// <value>The depth distant.</value>
    [ ChartTemplate( ChartTemplateSet.Simple ), DefaultValue( 200f ) ]
    public float ZDistant
    {
      get
      {
        return m_zDist;
      }

      set
      {
        if( m_zDist != value )
        {
          m_zDist = value;
          RaiseChanged();
        }
      }
    }
    #endregion

    #region Implementation
		/// <summary>
		/// Raises the changed.
		/// </summary>
    private void RaiseChanged()
    {
      if( Changed != null )
      {
        Changed( this, EventArgs.Empty );
      }
    }
    #endregion
	}
}
