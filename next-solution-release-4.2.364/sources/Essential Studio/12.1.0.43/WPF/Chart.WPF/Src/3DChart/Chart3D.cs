// <copyright file="Chart3D.cs" company="Syncfusion">
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
    using System.ComponentModel;
    using System.Text;
    using System.Windows;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Represents Chart 3D class to set 3D settings
    /// </summary>
    /// <remarks>
    /// Chart3D class supports various properties for making a chart appear in 3D mode.
    /// 3D mode can be easily enabled on a ChartArea using the <b>View3DMode</b>
    /// property
    /// </remarks>
    /// <example>
    /// XAML:
    /// <code language="XAML">
    /// &lt;sfchart:Chart Name="chart1"&gt; 
    ///   &lt;sfchart:ChartArea View3DMode="True"&gt; 
    ///     &lt;sfchart:ChartSeries Label="Sales" DataSource="{Binding
    /// Source={StaticResource myXmlData}, XPath=Products/Product}" BindingPathX="Month"
    /// BindingPathsY="Sales" Type="Column"  /&gt;
    ///   &lt;/sfchart:ChartArea&gt;
    /// &lt;/sfchart:Chart&gt;
    /// </code>
    /// C#:
    /// <code language="C#">
    /// Chart chart = new Chart();
    /// chart.Areas.Add(new ChartArea());
    /// chart.Areas[0].View3DMode= true;
    /// </code>
    /// </example>
    /// <seealso cref="Chart"/>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
  public class Chart3D : DependencyObject
  {
    #region Dependency properties
    /// <summary>
    ///  Using a DependencyProperty as the backing store for ShowTopWall.  This enables animation, styling, binding, etc...
    /// </summary>  
      /// <seealso cref="Chart3D"/>
    public static readonly DependencyProperty ShowTopWallProperty =
        DependencyProperty.Register("ShowTopWall", typeof(bool), typeof(Chart3D), new ChartPropertyMetadata(false, ChartPropertyMetadataOptions.AffectsRedraw));
    
      /// <summary>
    ///   Using a DependencyProperty as the backing store for ShowRightWall.  This enables animation, styling, binding, etc...
    /// </summary>    
    /// <seealso cref="Chart3D"/>
    public static readonly DependencyProperty ShowRightWallProperty =
        DependencyProperty.Register("ShowRightWall", typeof(bool), typeof(Chart3D), new ChartPropertyMetadata(false, ChartPropertyMetadataOptions.AffectsRedraw));
    
      /// <summary>
    ///   Using a DependencyProperty as the backing store for ShowBackWall.  This enables animation, styling, binding, etc...
    /// </summary>   
    /// <seealso cref="Chart3D"/>
    public static readonly DependencyProperty ShowBackWallProperty =
        DependencyProperty.Register("ShowBackWall", typeof(bool), typeof(Chart3D), new ChartPropertyMetadata(true, ChartPropertyMetadataOptions.AffectsRedraw));
    
      /// <summary>
    ///   Using a DependencyProperty as the backing store for ShowLeftWall.  This enables animation, styling, binding, etc...
    /// </summary>   
    /// <seealso cref="Chart3D"/>
    public static readonly DependencyProperty ShowLeftWallProperty =
        DependencyProperty.Register("ShowLeftWall", typeof(bool), typeof(Chart3D), new ChartPropertyMetadata(true, ChartPropertyMetadataOptions.AffectsRedraw));
    
      /// <summary>
    ///   Using a DependencyProperty as the backing store for ShowBottonWall.  This enables animation, styling, binding, etc...
    /// </summary>   
    /// <seealso cref="Chart3D"/>
    public static readonly DependencyProperty ShowBottomWallProperty =
        DependencyProperty.Register("ShowBottomWall", typeof(bool), typeof(Chart3D), new ChartPropertyMetadata(true, ChartPropertyMetadataOptions.AffectsRedraw));
    
      /// <summary>
    ///   Using a DependencyProperty as the backing store for ShowPrimaryAxis.  This enables animation, styling, binding, etc...
    /// </summary>    
    /// <seealso cref="Chart3D"/>
    public static readonly DependencyProperty ShowPrimaryAxisProperty =
        DependencyProperty.Register("ShowPrimaryAxis", typeof(bool), typeof(Chart3D), new ChartPropertyMetadata(true, ChartPropertyMetadataOptions.AffectsRedraw));
    
      /// <summary>
    ///   Using a DependencyProperty as the backing store for ShowSecondaryAxis.  This enables animation, styling, binding, etc...
    /// </summary>    
    /// <seealso cref="Chart3D"/>
    public static readonly DependencyProperty ShowSecondaryAxisProperty =
        DependencyProperty.Register("ShowSecondaryAxis", typeof(bool), typeof(Chart3D), new ChartPropertyMetadata(true, ChartPropertyMetadataOptions.AffectsRedraw));
    
    /// <summary>
    ///   Using a DependencyProperty as the backing store for ShowDepthAxis.  This enables animation, styling, binding, etc...
    /// </summary>    
    /// <seealso cref="Chart3D"/>
    public static readonly DependencyProperty ShowDepthAxisProperty =
        DependencyProperty.Register("ShowDepthAxis", typeof(bool), typeof(Chart3D), new ChartPropertyMetadata(true, ChartPropertyMetadataOptions.AffectsRedraw));

      /// <summary>
    ///  Using a DependencyProperty as the backing store for ChartLight.  This enables animation, styling, binding, etc...
    /// </summary>   
    /// <seealso cref="Chart3D"/>
    public static readonly DependencyProperty ChartLightProperty =
        DependencyProperty.Register("ChartLight", typeof(Model3D), typeof(Chart3D), new ChartPropertyMetadata(null, ChartPropertyMetadataOptions.AffectsRedraw));
    
      /// <summary>
    ///   Using a DependencyProperty as the backing store for TopWallBackground.  This enables animation, styling, binding, etc...
    /// </summary>
    /// <seealso cref="Chart3D"/>
    public static readonly DependencyProperty TopWallBackgroundProperty =
        DependencyProperty.Register("TopWallBackground", typeof(Brush), typeof(Chart3D), new ChartPropertyMetadata(new SolidColorBrush(Color.FromRgb(211, 211, 211)), ChartPropertyMetadataOptions.AffectsRedraw));
    
      /// <summary>
    ///   Using a DependencyProperty as the backing store for RightWallBackground.  This enables animation, styling, binding, etc...
    /// </summary>    
    /// <seealso cref="Chart3D"/>     
    public static readonly DependencyProperty RightWallBackgroundProperty =
        DependencyProperty.Register("RightWallBackground", typeof(Brush), typeof(Chart3D), new ChartPropertyMetadata(new SolidColorBrush(Color.FromRgb(211, 211, 211)), ChartPropertyMetadataOptions.AffectsRedraw));
    
      /// <summary>
    ///   Using a DependencyProperty as the backing store for LeftWallBackground.  This enables animation, styling, binding, etc...
    /// </summary>    
    /// <seealso cref="Chart3D"/>
    public static readonly DependencyProperty LeftWallBackgroundProperty =
        DependencyProperty.Register("LeftWallBackground", typeof(Brush), typeof(Chart3D), new ChartPropertyMetadata(new SolidColorBrush(Color.FromRgb(211, 211, 211)), ChartPropertyMetadataOptions.AffectsRedraw));
    
      /// <summary>
    ///   Using a DependencyProperty as the backing store for BottomWallBackground.  This enables animation, styling, binding, etc...
    /// </summary>  
    /// <seealso cref="Chart3D"/>
    public static readonly DependencyProperty BottomWallBackgroundProperty =
        DependencyProperty.Register("BottomWallBackground", typeof(Brush), typeof(Chart3D), new ChartPropertyMetadata(new SolidColorBrush(Color.FromRgb(211, 211, 211)), ChartPropertyMetadataOptions.AffectsRedraw));
    
      /// <summary>
    ///   Using a DependencyProperty as the backing store for BackWallBackground.  This enables animation, styling, binding, etc...
    /// </summary> 
    /// <seealso cref="Chart3D"/>
    public static readonly DependencyProperty BackWallBackgroundProperty =
        DependencyProperty.Register("BackWallBackground", typeof(Brush), typeof(Chart3D), new ChartPropertyMetadata(new SolidColorBrush(Color.FromRgb(211,211,211)), ChartPropertyMetadataOptions.AffectsRedraw));
    
      /// <summary>
    ///   Using a DependencyProperty as the backing store for ViewDefaultTurn.  This enables animation, styling, binding, etc...
    /// </summary>    
    /// <seealso cref="Chart3D"/>
    public static readonly DependencyProperty ViewDefaultTurnProperty =
        DependencyProperty.Register("ViewDefaultTurn", typeof(double), typeof(Chart3D), new UIPropertyMetadata(0d));
    
      /// <summary>
    ///   Using a DependencyProperty as the backing store for ViewDefaultRotate.  This enables animation, styling, binding, etc...
    /// </summary>    
    /// <seealso cref="Chart3D"/>
    public static readonly DependencyProperty ViewDefaultRotateProperty =
        DependencyProperty.Register("ViewDefaultRotate", typeof(double), typeof(Chart3D), new UIPropertyMetadata(10d));
    
      /// <summary>
    ///  Using a DependencyProperty as the backing store for ViewDefaultTilt.  This enables animation, styling, binding, etc...
    /// </summary> 
    /// <seealso cref="Chart3D"/>
    public static readonly DependencyProperty ViewDefaultTiltProperty =
        DependencyProperty.Register("ViewDefaultTilt", typeof(double), typeof(Chart3D), new UIPropertyMetadata(10d));
    
      /// <summary>
    /// Identifies the CameraProjection Dependency property
    /// </summary>
    /// <seealso cref="Chart3D"/>
    public static readonly DependencyProperty CameraProjectionProperty =
        DependencyProperty.Register("CameraProjection", typeof(CameraProjection), typeof(Chart3D), new ChartPropertyMetadata(CameraProjection.Orthographic, new PropertyChangedCallback(OnCameraProjectionChanged), ChartPropertyMetadataOptions.AffectsRedraw));
    
      /// <summary>
    ///  Using a DependencyProperty as the backing store for BackWallThickness.  This enables animation, styling, binding, etc...
    /// </summary>    
    /// <seealso cref="Chart3D"/>
    public static readonly DependencyProperty BackWallThicknessProperty =
        DependencyProperty.Register("BackWallThickness", typeof(double), typeof(Chart3D), new ChartPropertyMetadata(0.02d, ChartPropertyMetadataOptions.AffectsRedraw));
    
      /// <summary>
    ///  Using a DependencyProperty as the backing store for LeftWallThickness.  This enables animation, styling, binding, etc...
    ///  <seealso cref="Chart3D"/>
    /// </summary>    
    public static readonly DependencyProperty LeftWallThicknessProperty =
        DependencyProperty.Register("LeftWallThickness", typeof(double), typeof(Chart3D), new ChartPropertyMetadata(0.02d, ChartPropertyMetadataOptions.AffectsRedraw));
    
      /// <summary>
    ///  Using a DependencyProperty as the backing store for BottomWallThickness.  This enables animation, styling, binding, etc...
    /// </summary>    
    /// <seealso cref="Chart3D"/>
    public static readonly DependencyProperty BottomWallThicknessProperty =
        DependencyProperty.Register("BottomWallThickness", typeof(double), typeof(Chart3D), new ChartPropertyMetadata(0.02d, ChartPropertyMetadataOptions.AffectsRedraw));
    
      /// <summary>
    ///  Using a DependencyProperty as the backing store for RightWallThickness.  This enables animation, styling, binding, etc...
    /// </summary>  
    /// <seealso cref="Chart3D"/>
    public static readonly DependencyProperty RightWallThicknessProperty =
        DependencyProperty.Register("RightWallThickness", typeof(double), typeof(Chart3D), new ChartPropertyMetadata(0.02d, ChartPropertyMetadataOptions.AffectsRedraw));
    
      /// <summary>
    ///  Using a DependencyProperty as the backing store for RotateOnMouseDown.  This enables animation, styling, binding, etc...
    /// </summary>  
    /// <seealso cref="Chart3D"/>
    public static readonly DependencyProperty RotateOnMouseDownProperty =
        DependencyProperty.Register("RotateOnMouseDown", typeof(bool), typeof(Chart3D), new ChartPropertyMetadata(true, ChartPropertyMetadataOptions.AffectsRedraw));
    
      /// <summary>
    /// Using a DependencyProperty as the backing store for TopWallThickness.  This enables animation, styling, binding, etc...
    /// </summary>
    /// <seealso cref="Chart3D"/>
    public static readonly DependencyProperty TopWallThicknessProperty =
        DependencyProperty.Register("TopWallThickness", typeof(double), typeof(Chart3D), new ChartPropertyMetadata(0.02, ChartPropertyMetadataOptions.AffectsRedraw));
    #endregion

    #region Members
    /// <summary>
    ///  Initializes ChartArea
    /// </summary> 
    private ChartArea m_area;
    #endregion

    #region Events
    /// <summary>
    /// Event that is raised when CameraProjection property is changed.
    /// </summary>
    public event PropertyChangedCallback CameraProjectionChanged;
    #endregion

    #region Properties
       /// <summary>
    /// Gets or sets a value indicating whether Show/Hide Top wall
    /// </summary>
    public bool ShowTopWall
    {
      get
      {
        return (bool)GetValue(ShowTopWallProperty);
      }

      set
      {
        SetValue(ShowTopWallProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether Show/Hide Right wall
    /// </summary>
    public bool ShowRightWall
    {
      get
      {
        return (bool)GetValue(ShowRightWallProperty);
      }

      set
      {
        SetValue(ShowRightWallProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether Show/Hide Back wall
    /// </summary>
    public bool ShowBackWall
    {
      get
      {
        return (bool)GetValue(ShowBackWallProperty);
      }

      set
      {
        SetValue(ShowBackWallProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether Show/Hide Left wall
    /// </summary>
    public bool ShowLeftWall
    {
      get
      {
        return (bool)GetValue(ShowLeftWallProperty);
      }

      set
      {
        SetValue(ShowLeftWallProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether Show/Hide Bottom wall
    /// </summary>
    public bool ShowBottomWall
    {
      get
      {
        return (bool)GetValue(ShowBottomWallProperty);
      }

      set
      {
        SetValue(ShowBottomWallProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether Show/Hide Primary Axis
    /// </summary>
    public bool ShowPrimaryAxis
    {
      get
      {
        return (bool)GetValue(ShowPrimaryAxisProperty);
      }

      set
      {
        SetValue(ShowPrimaryAxisProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether Show/Hide Secondary Axis
    /// </summary>
    public bool ShowSecondaryAxis
    {
      get
      {
        return (bool)GetValue(ShowSecondaryAxisProperty);
      }

      set
      {
        SetValue(ShowSecondaryAxisProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether Show/Hide Secondary Axis
    /// </summary>
    public bool ShowDepthAxis
    {
        get
        {
            return (bool)GetValue(ShowDepthAxisProperty);
        }

        set
        {
            SetValue(ShowDepthAxisProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets ChartLight for 3D mode
    /// </summary>
    public Model3D ChartLight
    {
      get
      {
        return (Model3D)GetValue(ChartLightProperty);
      }

      set
      {
        SetValue(ChartLightProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets the top wall background brush
    /// </summary>
    /// <value>The top wall background.</value>
    public Brush TopWallBackground
    {
      get
      {
        return (Brush)GetValue(TopWallBackgroundProperty);
      }

      set
      {
        SetValue(TopWallBackgroundProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets the right wall background brush
    /// </summary>
    /// <value>The right wall background.</value>
    public Brush RightWallBackground
    {
      get
      {
        return (Brush)GetValue(RightWallBackgroundProperty);
      }

      set
      {
        SetValue(RightWallBackgroundProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets the left wall background brush
    /// </summary>
    /// <value>The left wall background.</value>
    public Brush LeftWallBackground
    {
      get
      {
        return (Brush)GetValue(LeftWallBackgroundProperty);
      }

      set
      {
        SetValue(LeftWallBackgroundProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets the bottom wall background brush
    /// </summary>
    /// <value>The bottom wall background.</value>
    public Brush BottomWallBackground
    {
      get
      {
        return (Brush)GetValue(BottomWallBackgroundProperty);
      }

      set
      {
        SetValue(BottomWallBackgroundProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets the back wall background brush
    /// </summary>
    /// <value>The back wall background.</value>
    public Brush BackWallBackground
    {
      get
      {
        return (Brush)GetValue(BackWallBackgroundProperty);
      }

      set
      {
        SetValue(BackWallBackgroundProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets the view default turn.
    /// </summary>
    /// <value>The view default turn.</value>
    public double ViewDefaultTurn
    {
      get
      {
        return (double)GetValue(ViewDefaultTurnProperty);
      }

      set
      {
        SetValue(ViewDefaultTurnProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets the view default rotate.
    /// </summary>
    /// <value>The view default rotate.</value>
    public double ViewDefaultRotate
    {
      get
      {
        return (double)GetValue(ViewDefaultRotateProperty);
      }

      set
      {
        SetValue(ViewDefaultRotateProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets the view default tilt.
    /// </summary>
    /// <value>The view default tilt.</value>
    public double ViewDefaultTilt
    {
      get
      {
        return (double)GetValue(ViewDefaultTiltProperty);
      }

      set
      {
        SetValue(ViewDefaultTiltProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets the value of the CameraProjection dependency property.
    /// </summary>
    public CameraProjection CameraProjection
    {
      get
      {
        return (CameraProjection)GetValue(CameraProjectionProperty);
      }

      set
      {
        SetValue(CameraProjectionProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets the left wall thickness.
    /// </summary>
    /// <value>The left wall thickness.</value>
    public double LeftWallThickness
    {
      get
      {
        return (double)GetValue(LeftWallThicknessProperty);
      }

      set
      {
        SetValue(LeftWallThicknessProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets the back wall thickness.
    /// </summary>
    /// <value>The back wall thickness.</value>
    public double BackWallThickness
    {
      get
      {
        return (double)GetValue(BackWallThicknessProperty);
      }

      set
      {
        SetValue(BackWallThicknessProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets the bottom wall thickness.
    /// </summary>
    /// <value>The bottom wall thickness.</value>
    public double BottomWallThickness
    {
      get
      {
        return (double)GetValue(BottomWallThicknessProperty);
      }

      set
      {
        SetValue(BottomWallThicknessProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets the top wall thickness.
    /// </summary>
    /// <value>The top wall thickness.</value>
    public double TopWallThickness
    {
      get
      {
        return (double)GetValue(TopWallThicknessProperty);
      }

      set
      {
        SetValue(TopWallThicknessProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets the right wall thickness.
    /// </summary>
    /// <value>The right wall thickness.</value>
    public double RightWallThickness
    {
      get
      {
        return (double)GetValue(RightWallThicknessProperty);
      }

      set
      {
        SetValue(RightWallThicknessProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [rotate on mouse down].
    /// </summary>
    /// <value><c>true</c> if [rotate on mouse down]; otherwise, <c>false</c>.</value>
    public bool RotateOnMouseDown
    {
      get
      {
        return (bool)GetValue(RotateOnMouseDownProperty);
      }

      set
      {
        SetValue(RotateOnMouseDownProperty, value);
      }
    }

    /// <summary>
    /// Sets the parent
    /// </summary>
    /// <value>The parent.</value>
    internal ChartArea Parent
    {
        set
        {
           this.m_area = value;
        }
    }
    #endregion   

    #region Implementation
    
    /// <summary>
    /// Updates property value cache and raises CameraProjectionChanged event.
    /// </summary>
    /// <param name="e">Property change details, such as old value and new value.</param>
    protected virtual void OnCameraProjectionChanged(DependencyPropertyChangedEventArgs e)
    {
      if (this.CameraProjectionChanged != null)
      {
        this.CameraProjectionChanged(this, e);
      }
    }

    /// <summary>
    /// Invoked whenever the effective value of any dependency property on this <see cref="T:System.Windows.DependencyObject"/> has been updated. The specific dependency property that changed is reported in the event data.
    /// </summary>
    /// <param name="e">Event data that will contain the dependency property identifier of interest, the property metadata for the type, and old and new values.</param>
    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
      if (this.m_area != null)
      {
        ChartPropertyMetadata propertyMetadata = e.Property.GetMetadata(this) as ChartPropertyMetadata;
        if (propertyMetadata != null && propertyMetadata.Options == ChartPropertyMetadataOptions.AffectsRedraw)
        {
          this.m_area.UpdateArea();
        }
      }

      base.OnPropertyChanged(e);
    }

    /// <summary>
    /// Calls OnCameraProjectionChanged method of the instance, notifies of the depencency property value changes.
    /// </summary>
    /// <param name="d">Dependency object, the change occures on.</param>
    /// <param name="e">Property change details, such as old value and new value.</param>
    private static void OnCameraProjectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        Chart3D instance = (Chart3D)d;
        instance.OnCameraProjectionChanged(e);
    }
    #endregion
  }
}
