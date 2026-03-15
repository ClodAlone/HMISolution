// <copyright file="ChartTargetCamera.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
  using System;
  using System.Collections.Generic;
  using System.Text;
  using System.Windows.Media;
  using System.Windows.Media.Media3D;
  using System.Windows;

  /// <summary>
  /// Represents ChartTargetCameraController
  /// </summary>
  /// <exclude/>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    public class ChartTargetCameraController : DependencyObject
  {
    #region Members
    /// <summary>
    /// Initializes m_camera
    /// </summary>
    private ProjectionCamera m_camera = null;

    /// <summary>
    /// Initializes m_target
    /// </summary>
    private Vector3D m_target = new Vector3D();    
     
      /// <summary>
    /// Initializes m_length
    /// </summary>
    private double m_length = 3.5;
    #endregion

    #region Deendency properties
    /// <summary>
    /// Identifies the Tilt dependency property.
    /// </summary>
    /// <seealso cref="ChartTargetCameraController"/>
    public static readonly DependencyProperty TiltProperty =
        DependencyProperty.Register("Tilt", typeof(double), typeof(ChartTargetCameraController), new UIPropertyMetadata(10d));
   
      /// <summary>
    /// Identifies the Turn dependency property.
    /// </summary>
    /// <seealso cref="ChartTargetCameraController"/>
    public static readonly DependencyProperty TurnProperty =
        DependencyProperty.Register("Turn", typeof(double), typeof(ChartTargetCameraController), new UIPropertyMetadata(0d));
   
      /// <summary>
    /// Identifies the Rotate dependency property.
    /// </summary>
    /// <seealso cref="ChartTargetCameraController"/>
    public static readonly DependencyProperty RotateProperty =
        DependencyProperty.Register("Rotate", typeof(double), typeof(ChartTargetCameraController), new UIPropertyMetadata(10d));
    #endregion

    #region Public methods
    /// <summary>
    /// Initializes a new instance of the ChartTargetCameraController class
    /// </summary>
    /// <param name="camera">The camera value</param>
    public ChartTargetCameraController(ProjectionCamera camera)
    {
      this.m_camera = camera;
      this.Calculate();
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets the Camera value
    /// </summary>
    public ProjectionCamera Camera
    {
      get
      {
        return this.m_camera;
      }
    }
  
      /// <summary>
    /// Gets or sets the Rotate value. This is a dependency property.
    /// </summary>
    public double Rotate
    {
      get { return (double)GetValue(RotateProperty); }
      set { SetValue(RotateProperty, value); }
    }
  
      /// <summary>
    /// Gets or sets the Tilt value. This is a dependency property.
    /// </summary>
    public double Tilt
    {
      get { return (double)GetValue(TiltProperty); }
      set { SetValue(TiltProperty, value); }
    }
   
      /// <summary>
    /// Gets or sets the Tilt value. This is a dependency property.
    /// </summary>
    public double Turn
    {
      get { return (double)GetValue(TurnProperty); }
      set { SetValue(TurnProperty, value); }
    }
  
      /// <summary>
    /// Gets or sets the Length value
    /// </summary>
    public double Length
    {
      get
      {
        return this.m_length;
      }

      set
      {
        if (this.m_length != value)
        {
          this.m_length = value;
          this.Calculate();
        }
      }
    }

    /// <summary>
    /// Gets or sets the target.
    /// </summary>
    /// <value>The target.</value>
    public Vector3D Target
    {
      get
      {
        return this.m_target;
      }

      set
      {
        if (this.m_target != value)
        {
          this.m_target = value;
          this.Calculate();
        }
      }
    }
    #endregion

    #region Implementation
    /// <summary>
    /// The Calculate method
    /// </summary>
    private void Calculate()
    {
      double tilt = this.Tilt;
      double turn = this.Turn;
      double rotate = this.Rotate;
      double cosRotate = Math.Cos(rotate * ChartMath.ToRadial);
      double sinRotate = Math.Sin(rotate * ChartMath.ToRadial);
      double cosRotatePI2 = Math.Cos(rotate * ChartMath.ToRadial + Math.PI / 2);
      double sinRotatePI2 = Math.Sin(rotate * ChartMath.ToRadial + Math.PI / 2);
      double cosTilt = Math.Cos(tilt * ChartMath.ToRadial);
      double sinTilt = Math.Sin(tilt * ChartMath.ToRadial);
      double cosTurn = Math.Cos(turn * ChartMath.ToRadial);
      double sinTurn = Math.Sin(turn * ChartMath.ToRadial);

      Vector3D leftVector = new Vector3D(sinRotatePI2, 0, cosRotatePI2);
      Vector3D vector = new Vector3D(sinRotate * cosTilt, sinTilt, cosRotate * cosTilt);
      Vector3D upvector = Vector3D.CrossProduct(vector, leftVector);

      this.m_camera.Position = (Point3D)(this.m_target + vector * this.m_length);
      this.m_camera.LookDirection = -vector;
      this.m_camera.UpDirection = cosTurn * upvector - leftVector * sinTurn;
    }
   
      /// <summary>
    /// Invoked whenever the effective value of any dependency property on this <see cref="T:System.Windows.DependencyObject"/> has been updated. The specific dependency property that changed is reported in the event data.
    /// </summary>
    /// <param name="e">Event data that will contain the dependency property identifier of interest, the property metadata for the type, and old and new values.</param>
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
      Calculate();
      base.OnPropertyChanged(e);
    }

    #endregion
  }
}
