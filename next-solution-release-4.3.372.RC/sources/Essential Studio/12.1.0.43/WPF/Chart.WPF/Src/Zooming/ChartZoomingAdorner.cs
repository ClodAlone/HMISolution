// <copyright file="ChartZoomingAdorner.cs" company="Syncfusion">
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
  using System.Globalization;
  using System.Text;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Controls.Primitives;
  using System.Windows.Data;
  using System.Windows.Documents;
  using System.Windows.Input;
  using System.Windows.Media;
  using System.Windows.Shapes;
  using Syncfusion.Windows.Shared;
    using System.Windows.Automation.Peers;

  /// <summary>
  /// Represents the zooming toolkit that is shown on <see cref="ChartArea" /> when
  /// <see cref="ChartAreaCommands.SwitchZooming" /> command is executed.
  /// </summary>
  /// <remarks>
  /// In the Zooming mode, a Zooming toolkit is displayed at the top-left corner of
  /// the ChartArea. Using the buttons in the Zooming toolkit, ChartSeries can be
  /// zoomed in, out, reset or closed (to exit zoom mode).
  /// </remarks>
    /// <seealso cref="ChartZoomingToolkit"/>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
  public class ChartZoomingToolkit : Control
  {
      private class ChartZoomingToolkitAutomationPeer : FrameworkElementAutomationPeer
      {
          public ChartZoomingToolkitAutomationPeer(ChartZoomingToolkit control)
              : base(control)
          {
          }

          protected override string GetClassNameCore()
          {
              return "ChartZoomingToolkit";
          }

          protected override AutomationControlType GetAutomationControlTypeCore()
          {
              return AutomationControlType.ListItem;
          }

          public override object GetPattern(PatternInterface patternInterface)
          {
             
                  return this;
              
          }


          private ChartZoomingToolkit MyOwner
          {
              get
              {
                  return (ChartZoomingToolkit)base.Owner;
              }
          }
      }

      /// <summary>
      /// Returns class-specific <see cref="T:System.Windows.Automation.Peers.AutomationPeer"/> implementations for the Windows Presentation Foundation (WPF) infrastructure.
      /// </summary>
      /// <returns>
      /// The type-specific <see cref="T:System.Windows.Automation.Peers.AutomationPeer"/> implementation.
      /// </returns>
      protected override AutomationPeer OnCreateAutomationPeer()
      {
          return new ChartZoomingToolkitAutomationPeer(this);
      }

    #region dependency properties
    /// <summary>
    /// Identifies the ZoomingToolkitVisibility dependency property.
    /// </summary>
    public static readonly DependencyProperty ZoomingToolkitVisibilityProperty =
            DependencyProperty.RegisterAttached("ZoomingToolkitVisibility", typeof(Visibility), typeof(ChartZoomingToolkit), new FrameworkPropertyMetadata(Visibility.Hidden,new PropertyChangedCallback(OnValueChanged)));

    private static void OnValueChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs args)
    {
        ChartArea area = dpObj as ChartArea;
        if (area != null && area.m_areaPresenter != null)
        {
            area.SetAreaPresenter(area.m_areaPresenter);
        }
    }
    /// <summary>
    /// Identifies the ZoomInButtonVisibility dependency property.
    /// </summary>
    public static readonly DependencyProperty ZoomInButtonVisibilityProperty =
            DependencyProperty.RegisterAttached("ZoomInButtonVisibility", typeof(Visibility), typeof(ChartZoomingToolkit), new FrameworkPropertyMetadata(Visibility.Visible));

    /// <summary>
    /// Identifies the ZoomOutButtonVisibility dependency property.
    /// </summary>
    public static readonly DependencyProperty ZoomOutButtonVisibilityProperty =
            DependencyProperty.RegisterAttached("ZoomOutButtonVisibility", typeof(Visibility), typeof(ChartZoomingToolkit), new FrameworkPropertyMetadata(Visibility.Visible));

    /// <summary>
    /// Identifies the ZoomCloseButtonVisibility dependency property.
    /// </summary>
    public static readonly DependencyProperty ZoomCloseButtonVisibilityProperty =
            DependencyProperty.RegisterAttached("ZoomCloseButtonVisibility", typeof(Visibility), typeof(ChartZoomingToolkit), new FrameworkPropertyMetadata(Visibility.Visible));

    /// <summary>
    /// Identifies the ZoomResetButtonVisibility dependency property.
    /// </summary>
    public static readonly DependencyProperty ZoomResetButtonVisibilityProperty =
            DependencyProperty.RegisterAttached("ZoomResetButtonVisibility", typeof(Visibility), typeof(ChartZoomingToolkit), new FrameworkPropertyMetadata(Visibility.Visible));


    /// <summary>
    /// Identifies the ZoomPanningButtonVisibility dependency property.
    /// </summary> 
    public static readonly DependencyProperty ZoomPanningButtonVisibilityProperty =
        DependencyProperty.RegisterAttached("ZoomPanningButtonVisibility", typeof(Visibility), typeof(ChartZoomingToolkit), new FrameworkPropertyMetadata(Visibility.Visible));

    /// <summary>
    /// Identifies the ZoomInIconTemplate dependency property.
    /// </summary> 
    public static readonly DependencyProperty ZoomInIconTemplateProperty =
       DependencyProperty.RegisterAttached("ZoomInIconTemplate", typeof(DataTemplate), typeof(ChartZoomingToolkit), new FrameworkPropertyMetadata(null));

    /// <summary>
    /// Identifies the ZoomOutIconTemplate dependency property.
    /// </summary> 
    public static readonly DependencyProperty ZoomOutIconTemplateProperty =
       DependencyProperty.RegisterAttached("ZoomOutIconTemplate", typeof(DataTemplate), typeof(ChartZoomingToolkit), new FrameworkPropertyMetadata(null));

    /// <summary>
    /// Identifies the ZoomCloseIconTemplate dependency property.
    /// </summary> 
    public static readonly DependencyProperty ZoomCloseIconTemplateProperty =
       DependencyProperty.RegisterAttached("ZoomCloseIconTemplate", typeof(DataTemplate), typeof(ChartZoomingToolkit), new FrameworkPropertyMetadata(null));

    /// <summary>
    /// Identifies the ZoomResetIconTemplate dependency property.
    /// </summary> 
    public static readonly DependencyProperty ZoomResetIconTemplateProperty =
       DependencyProperty.RegisterAttached("ZoomResetIconTemplate", typeof(DataTemplate), typeof(ChartZoomingToolkit), new FrameworkPropertyMetadata(null));

    /// <summary>
    /// Identifies the ZoomPannaingIconTemplate dependency property.
    /// </summary> 
    public static readonly DependencyProperty ZoomPanningIconTemplateProperty =
       DependencyProperty.RegisterAttached("ZoomPanningIconTemplate", typeof(DataTemplate), typeof(ChartZoomingToolkit), new FrameworkPropertyMetadata(null));

    /// <summary>
    /// Identifies the Owner dependency property.
    /// </summary>
    public static readonly DependencyProperty OwnerProperty =
            DependencyProperty.Register("Owner", typeof(ChartArea), typeof(ChartZoomingToolkit), new UIPropertyMetadata(null));

    /// <summary>
    /// Identifies the Parent dependency property.
    /// </summary>
    public static readonly DependencyProperty ParentProperty =
            DependencyProperty.Register("Parent", typeof(ChartArea), typeof(ChartZoomingToolkit), new UIPropertyMetadata(null));
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets owning <see cref="ChartArea"/>. This is a dependency property.
    /// </summary>
    public ChartArea Owner
    {
      get { return (ChartArea)GetValue(OwnerProperty); }
      set { SetValue(OwnerProperty, value); }
    }

    /// <summary>
    /// Get and Set ParentProperty
    /// </summary>
    public new ChartArea Parent
    {
        get { return (ChartArea)GetValue(ParentProperty); }
        set { SetValue(ParentProperty, value); }
    }
    #endregion

    #region Constructor
    /// <summary>
    /// Initializes static members of the <see cref="ChartZoomingToolkit"/> class.
    /// </summary>
    static ChartZoomingToolkit()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ChartZoomingToolkit), new FrameworkPropertyMetadata(typeof(ChartZoomingToolkit)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChartZoomingToolkit"/> class.
    /// </summary>
    /// <param name="area">The area value.</param>
    public ChartZoomingToolkit(ChartArea area)
    {
      SetBinding(FlowDirectionProperty, new Binding("FlowDirection") { Source = area });
      this.Owner = area;
      if (area.SyncChartArea is SyncChartAreas)
      {
          this.Parent = area.SyncChartArea;
      }
      else 
      {
          this.Parent = area;
      }     
    }
    #endregion

    #region Implementation
    /// <summary>
    /// Gets the value of the ZoomResetButtonVisibility attached dependency property.
    /// </summary>
    /// <param name="obj">The DependencyObject obj</param>
    /// <returns>Returns the Visibility value</returns>
    public static Visibility GetZoomResetButtonVisibility(DependencyObject obj)
    {
      return (Visibility)obj.GetValue(ZoomResetButtonVisibilityProperty);
    }

    /// <summary>
    /// Sets the value of the ZoomResetButtonVisibility attached dependency property.
    /// </summary>
    /// <param name="obj">The DependencyObject obj</param>
    /// <param name="value">The Visibility value</param>
    /// <seealso cref="ChartZoomingToolkit"/>
    public static void SetZoomResetButtonVisibility(DependencyObject obj, Visibility value)
    {
        if (obj is ChartArea)
        {
            ChartArea area = obj as ChartArea;
            if (area.IsSync == true)
            {
                (area.ChartAreaParent).SetValue(ZoomResetButtonVisibilityProperty, value);
            }
            else
            {
                obj.SetValue(ZoomResetButtonVisibilityProperty, value);
            }
        }  
    }

    /// <summary>
    /// Gets the value of the ZoomPanningButtonVisibility attached dependency property.
    /// </summary>
    /// <param name="obj">The DependencyObject obj</param>
    /// <returns>Returns the Visibility value</returns>
    public static Visibility GetZoomPanningButtonVisibility(DependencyObject obj)
    {
        return (Visibility)obj.GetValue(ZoomPanningButtonVisibilityProperty);
    }

    /// <summary>
    /// Sets the value of the ZoomPanningButtonVisibility attached dependency property.
    /// </summary>
    /// <param name="obj">The DependencyObject obj</param>
    /// <param name="value">The Visibility value</param>

    public static void SetZoomPanningButtonVisibility(DependencyObject obj, Visibility value)
    {
        if (obj is ChartArea)
        {
            ChartArea area = obj as ChartArea;
            if (area.IsSync == true)
            {
                (area.ChartAreaParent).SetValue(ZoomPanningButtonVisibilityProperty, value);
            }
            else
            {
                obj.SetValue(ZoomPanningButtonVisibilityProperty, value);
            }
        }  
    }
    /// <summary>
    /// Gets the value of the ZoomCloseButtonVisibility attached dependency property.
    /// </summary>
    /// <param name="obj">The DependencyObject obj</param>
    /// <returns>Returns the Visibility value</returns>
    public static Visibility GetZoomCloseButtonVisibility(DependencyObject obj)
    {
      return (Visibility)obj.GetValue(ZoomCloseButtonVisibilityProperty);
    }

    /// <summary>
    /// Sets the value of the ZoomCloseButtonVisibility attached dependency property.
    /// </summary>
    /// <param name="obj">The DependencyObject obj</param>
    /// <param name="value">The Visibility value</param>
    public static void SetZoomCloseButtonVisibility(DependencyObject obj, Visibility value)
    {
          if (obj is ChartArea)
          {
              ChartArea area = obj as ChartArea;
              if (area.IsSync == true)
              {
                  (area.ChartAreaParent).SetValue(ZoomCloseButtonVisibilityProperty, value);
              }
              else
              {
                  obj.SetValue(ZoomCloseButtonVisibilityProperty, value);
              }
          }
    }

    /// <summary>
    /// Gets the value of the ZoomOutButtonVisibility attached dependency property.
    /// </summary>
    /// <param name="obj">The DependencyObject obj</param>
    /// <returns>Returns the Visibility value</returns>
    public static Visibility GetZoomOutButtonVisibility(DependencyObject obj)
    {
      return (Visibility)obj.GetValue(ZoomOutButtonVisibilityProperty);
    }

      /// <summary>
      /// Sets the value of the ZoomOutButtonVisibility attached dependency property.
      /// </summary>
      /// <param name="obj">The DependencyObject obj</param>
      /// <param name="value">The Visibility value</param>
      /// <seealso>
      ///     <cref>ChartZoomingToolKit</cref>
      /// </seealso>
      public static void SetZoomOutButtonVisibility(DependencyObject obj, Visibility value)
    {
        if (obj is ChartArea)
        {
            ChartArea area = obj as ChartArea;
            if (area.IsSync == true)
            {
                (area.ChartAreaParent).SetValue(ZoomOutButtonVisibilityProperty, value);
            }
            else
            {
                obj.SetValue(ZoomOutButtonVisibilityProperty, value);
            }
        }  
    }

    /// <summary>
    /// Gets the value of the ZoomingToolkitVisibility attached dependency property.
    /// </summary>
    /// <param name="obj">The DependencyObject obj</param>
    /// <returns>Returns the Visibility value</returns>
    public static Visibility GetZoomingToolkitVisibility(DependencyObject obj)
    {
      return (Visibility)obj.GetValue(ZoomingToolkitVisibilityProperty);
    }

      /// <summary>
      /// Sets the value of the ZoomingToolkitVisibility attached dependency property.
      /// </summary>
      /// <param name="obj">The DependencyObject obj</param>
      /// <param name="value">The Visibility value</param>
      /// <seealso>
      ///     <cref>ChartZoomingToolKit</cref>
      /// </seealso>
      public static void SetZoomingToolkitVisibility(DependencyObject obj, Visibility value)
    {
      obj.SetValue(ZoomingToolkitVisibilityProperty, value);
    }

    /// <summary>
    /// Gets the value of the ZoomInButtonVisibility attached dependency property.
    /// </summary>
    /// <param name="obj">The DependencyObject obj</param>
    /// <returns>Returns the Visibility value</returns>
    public static Visibility GetZoomInButtonVisibility(DependencyObject obj)
    {
      return (Visibility)obj.GetValue(ZoomInButtonVisibilityProperty);
    }

    /// <summary>
    /// Sets the value of the ZoomInButtonVisibility attached dependency property.
    /// </summary>
    /// <param name="obj">The DependencyObject obj</param>
    /// <param name="value">The Visibility value</param>
    /// <seealso cref="ChartZoomingAdorner"/>
    public static void SetZoomInButtonVisibility(DependencyObject obj, Visibility value)
    {
      if (obj is ChartArea)
          {
              ChartArea area = obj as ChartArea;
              if (area.IsSync == true)
              {
                  (area.ChartAreaParent).SetValue(ZoomInButtonVisibilityProperty, value);
              }
              else
              {
                  obj.SetValue(ZoomInButtonVisibilityProperty, value);
              }
          }
    }

    /// <summary>
    /// Gets the value of the ZoomInIconTemplate attached dependency property.
    /// </summary>
    /// <param name="obj">The DependencyObject obj</param>
    /// <returns>Returns the Icon Template</returns>
    public static DataTemplate GetZoomInIconTemplate(DependencyObject obj)
    {
        return (DataTemplate)obj.GetValue(ZoomInIconTemplateProperty);
    }

    /// <summary>
    /// Sets the value of the ZoomInIconTemplate attached dependency property.
    /// </summary>
    /// <param name="obj">The DependencyObject obj</param>
    /// <param name="value">The Data Template value</param>
    public static void SetZoomInIconTemplate(DependencyObject obj, DataTemplate value)
    {
        obj.SetValue(ZoomInIconTemplateProperty, value as DataTemplate);
    }

    /// <summary>
    /// Gets the value of the ZoomOutIconTemplate attached dependency property.
    /// </summary>
    /// <param name="obj">The DependencyObject obj</param>
    /// <returns>Returns the Icon Template</returns>
    public static DataTemplate GetZoomOutIconTemplate(DependencyObject obj)
    {
        return (DataTemplate)obj.GetValue(ZoomOutIconTemplateProperty);
    }

    /// <summary>
    /// Sets the value of the ZoomOutIconTemplate attached dependency property.
    /// </summary>
    /// <param name="obj">The DependencyObject obj</param>
    /// <param name="value">The Data Template value</param>
    public static void SetZoomOutIconTemplate(DependencyObject obj, DataTemplate value)
    {
        obj.SetValue(ZoomOutIconTemplateProperty, value as DataTemplate);
    }

    /// <summary>
    /// Gets the value of the ZoomCloseIconTemplate attached dependency property.
    /// </summary>
    /// <param name="obj">The DependencyObject obj</param>
    /// <returns>Returns the Icon Template</returns>
    public static DataTemplate GetZoomCloseIconTemplate(DependencyObject obj)
    {
        return (DataTemplate)obj.GetValue(ZoomCloseIconTemplateProperty);
    }

    /// <summary>
    /// Sets the value of the ZoomCloseIconTemplate attached dependency property.
    /// </summary>
    /// <param name="obj">The DependencyObject obj</param>
    /// <param name="value">The Data Template value</param>
    public static void SetZoomCloseIconTemplate(DependencyObject obj, DataTemplate value)
    {
        obj.SetValue(ZoomCloseIconTemplateProperty, value as DataTemplate);
    }

    /// <summary>
    /// Gets the value of the ZoomResetIconTemplate attached dependency property.
    /// </summary>
    /// <param name="obj">The DependencyObject obj</param>
    /// <returns>Returns the Icon Template</returns>
    public static DataTemplate GetZoomResetIconTemplate(DependencyObject obj)
    {
        return (DataTemplate)obj.GetValue(ZoomResetIconTemplateProperty);
    }

    /// <summary>
    /// Sets the value of the ZoomResetIconTemplate attached dependency property.
    /// </summary>
    /// <param name="obj">The DependencyObject obj</param>
    /// <param name="value">The Data Template value</param>
    public static void SetZoomResetIconTemplate(DependencyObject obj, DataTemplate value)
    {
        obj.SetValue(ZoomResetIconTemplateProperty, value as DataTemplate);
    }

    /// <summary>
    /// Gets the value of the ZoomPanningIconTemplate attached dependency property.
    /// </summary>
    /// <param name="obj">The DependencyObject obj</param>
    /// <returns>Returns the Icon Template</returns>
    public static DataTemplate GetZoomPanningIconTemplate(DependencyObject obj)
    {
        return (DataTemplate)obj.GetValue(ZoomPanningIconTemplateProperty);
    }

    /// <summary>
    /// Sets the value of the ZoomPanningIconTemplate attached dependency property.
    /// </summary>
    /// <param name="obj">The DependencyObject obj</param>
    /// <param name="value">The Data Template value</param>
    public static void SetZoomPanningIconTemplate(DependencyObject obj, DataTemplate value)
    {
        obj.SetValue(ZoomPanningIconTemplateProperty, value as DataTemplate);
    }
    #endregion
  }

  /// <summary>
  /// Represents chart zooming scrollbar.
  /// </summary>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    [TemplatePart(Name = "PART_ScrollBar", Type = typeof(ScrollBar))]
  public class ChartZoomingScrollBar : Control, IDisposable
  {
    #region Internal types
    /// <summary>
    /// Represents ZoomFactorToViewportSizeConverter
    /// </summary>
    private class ZoomFactorToViewportSizeConverter : IValueConverter
    {
      #region IValueConverter Members
      /// <summary>
      /// Converts a value.
      /// </summary>
      /// <param name="value">The value produced by the binding source.</param>
      /// <param name="targetType">The type of the binding target property.</param>
      /// <param name="parameter">The converter parameter to use.</param>
      /// <param name="culture">The culture to use in the converter.</param>
      /// <returns>
      /// A converted value. If the method returns null, the valid null value is used.
      /// </returns>
      /// <seealso cref="ZoomFactorToViewportSizeConverter"/>
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
        return 2 * (double)value;
      }

      /// <summary>
      /// Converts a value.
      /// </summary>
      /// <param name="value">The value that is produced by the binding target.</param>
      /// <param name="targetType">The type to convert to.</param>
      /// <param name="parameter">The converter parameter to use.</param>
      /// <param name="culture">The culture to use in the converter.</param>
      /// <returns>
      /// A converted value. If the method returns null, the valid null value is used.
      /// </returns>
      /// <seealso cref="ZoomFactorToViewportSizeConverter"/>
      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
        return 0.5 * (double)value;
      }
      #endregion
    }

    /// <summary>
    /// Represents ZoomFactorToVisibilityConverter
    /// </summary>
    private class ZoomFactorToVisibilityConverter : IMultiValueConverter
    {
      #region IMultiValueConverter Members
      /// <summary>
      /// Converts source values to a value for the binding target. The data binding engine calls this method when it propagates the values from source bindings to the binding target.
      /// </summary>
      /// <param name="values">The array of values that the source bindings in the <see cref="T:System.Windows.Data.MultiBinding"/> produces. The value <see cref="F:System.Windows.DependencyProperty.UnsetValue"/> indicates that the source binding has no value to provide for conversion.</param>
      /// <param name="targetType">The type of the binding target property.</param>
      /// <param name="parameter">The converter parameter to use.</param>
      /// <param name="culture">The culture to use in the converter.</param>
      /// <returns>
      /// A converted value.
      /// If the method returns null, the valid null value is used.
      /// A return value of <see cref="T:System.Windows.DependencyProperty"/>.<see cref="F:System.Windows.DependencyProperty.UnsetValue"/> indicates that the converter did not produce a value, and that the binding will use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"/> if it is available, or else will use the default value.
      /// A return value of <see cref="T:System.Windows.Data.Binding"/>.<see cref="F:System.Windows.Data.Binding.DoNothing"/> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"/> or the default value.
      /// </returns>
      /// <seealso cref="ZoomFactorToVisibilityConverter"/>
      public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
      {

          if (!object.Equals(values[0], 1d) && object.Equals(values[6], Visibility.Visible))
          {
              if (object.Equals(values[4], true))
              {
                        //if (values[3] is int && object.Equals(values[2], (int)values[3] - 1))
                        if (values[3] is int && object.Equals((parameter as ChartArea).index, (int)values[3] - 1))
                        {
                      return Visibility.Visible;
                  }
                  else
                      return Visibility.Collapsed;
              }
              else
              {
                  if (object.Equals(values[2], 0))
                  {
                      return Visibility.Visible;
                  }
                  else
                      return Visibility.Collapsed;
              }
          }
          else
              return Visibility.Collapsed;
          //return !object.Equals(values[0], 1d) /*&& (bool)values[1]*/ ? Visibility.Visible : Visibility.Collapsed;
      }

      /// <summary>
      /// Converts a binding target value to the source binding values.
      /// </summary>
      /// <param name="value">The value that the binding target produces.</param>
      /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
      /// <param name="parameter">The converter parameter to use.</param>
      /// <param name="culture">The culture to use in the converter.</param>
      /// <returns>
      /// An array of values that have been converted from the target value back to the source values.
      /// </returns>
      /// <seealso cref="ZoomFactorToVisibilityConverter"/>
      public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
      {
        throw new NotSupportedException("Conversion back is not supported for visibility multibinding");
      }
      #endregion
    }

    /// <summary>
    /// Represents ZoomPositionToValueConverter
    /// </summary>
    private class ZoomPositionToValueConverter : IValueConverter
    {
      #region Members
      /// <summary>
      /// Initializes m_zoomingScrollBar
      /// </summary>
      private ChartZoomingScrollBar m_zoomingScrollBar;
      #endregion

      #region Constructor
      /// <summary>
      /// Initializes a new instance of the <see cref="ZoomPositionToValueConverter"/> class.
      /// </summary>
      /// <param name="zoomingScrollBar">The zooming scroll bar.</param>
      public ZoomPositionToValueConverter(ChartZoomingScrollBar zoomingScrollBar)
      {
        m_zoomingScrollBar = zoomingScrollBar;
      }
      #endregion

      #region IValueConverter Members
      /// <summary>
      /// Converts a value. The data binding engine calls this method when it propagates a value from the binding source to the binding target.
      /// </summary>
      /// <param name="value">The value produced by the binding source.</param>
      /// <param name="targetType">The type of the binding target property.</param>
      /// <param name="parameter">The converter parameter to use.</param>
      /// <param name="culture">The culture to use in the converter.</param>
      /// <returns>
      /// A converted value.If the method returns null, the valid null value is used.A return value of <see cref="T:System.Windows.DependencyProperty"></see>.<see cref="F:System.Windows.DependencyProperty.UnsetValue"></see> indicates that the converter produced no value and that the binding uses the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"></see>, if available, or the default value instead.A return value of <see cref="T:System.Windows.Data.Binding"></see>.<see cref="F:System.Windows.Data.Binding.DoNothing"></see> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"></see> or default value.
      /// </returns>
      /// <seealso cref="ZoomPositionToValueConverter"/>
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
        bool isVert = (m_zoomingScrollBar.Orientation == Orientation.Vertical) ^ m_zoomingScrollBar.IsInversed;
        double actualValue = (double)value / (1d - m_zoomingScrollBar.ZoomFactor);
        actualValue = double.IsNaN(actualValue) ? 0 : actualValue;
        return isVert ? 1 - actualValue : actualValue;
      }

      /// <summary>
      /// Converts a value. The data binding engine calls this method when it propagates a value from the binding target to the binding source.
      /// </summary>
      /// <param name="value">The value that is produced by the binding target.</param>
      /// <param name="targetType">The type to convert to.</param>
      /// <param name="parameter">The converter parameter to use.</param>
      /// <param name="culture">The culture to use in the converter.</param>
      /// <returns>
      /// A converted value.If the method returns null, the valid null value is used.A return value of <see cref="T:System.Windows.DependencyProperty"></see>.<see cref="F:System.Windows.DependencyProperty.UnsetValue"></see> indicates that the converter produced no value and that to the binding uses the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"></see>, if available, or the default value instead.A return value of <see cref="T:System.Windows.Data.Binding"></see>.<see cref="F:System.Windows.Data.Binding.DoNothing"></see> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"></see> or default value.
      /// </returns>
      /// <seealso cref="ZoomPositionToValueConverter"/>
      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
        bool isVert = (m_zoomingScrollBar.Orientation == Orientation.Vertical) ^ m_zoomingScrollBar.IsInversed;

        return (1d - m_zoomingScrollBar.ZoomFactor) * (isVert ? 1d - (double)value : (double)value);
      }
      #endregion
    }
    #endregion

    #region Members
    /// <summary>
    /// Initializes m_scrollBar
    /// </summary>
    private ScrollBar m_scrollBar;
    #endregion

    #region Dependency properties
    /// <summary>
    /// Using a DependencyProperty as the backing store for ZoomFactor.  This enables animation, styling, binding, etc...
    /// </summary>
    public static readonly DependencyProperty ZoomFactorProperty =
        DependencyProperty.Register("ZoomFactor", typeof(double), typeof(ChartZoomingScrollBar), new UIPropertyMetadata(0.5d, new PropertyChangedCallback(OnZoomFactorChanged)));

    /// <summary>
    /// Using a DependencyProperty as the backing store for ZoomPosition.  This enables animation, styling, binding, etc...
    /// </summary>
    public static readonly DependencyProperty ZoomPositionProperty =
        DependencyProperty.Register("ZoomPosition", typeof(double), typeof(ChartZoomingScrollBar), new UIPropertyMetadata(0d));

    /// <summary>
    /// Using a DependencyProperty as the backing store for IsInversed.  This enables animation, styling, binding, etc...
    /// </summary>
    public static readonly DependencyProperty IsInversedProperty =
        DependencyProperty.Register("IsInversed", typeof(bool), typeof(ChartZoomingScrollBar), new UIPropertyMetadata(false, new PropertyChangedCallback(OnZoomFactorChanged)));

    /// <summary>
    /// Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
    /// </summary>
    public static readonly DependencyProperty OrientationProperty =
        DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ChartZoomingScrollBar), new UIPropertyMetadata(Orientation.Horizontal));

    /// <summary>
    /// Using a DependencyProperty as the backing store for ZoomSwitched.  This enables animation, styling, binding, etc...
    /// </summary>
    public static readonly DependencyProperty ZoomSwitchedProperty =
        DependencyProperty.Register("ZoomSwitched", typeof(bool), typeof(ChartZoomingScrollBar), new UIPropertyMetadata(false));


    
   
    /// <summary>
    ///  Identifies the HorizontalBarLargeChange dependency property.
    /// </summary>
    public static readonly DependencyProperty HorizontalBarLargeChangeProperty =
            DependencyProperty.RegisterAttached("HorizontalBarLargeChange", typeof(double), typeof(ChartZoomingScrollBar), new FrameworkPropertyMetadata(0.5d));

    /// <summary>
    /// Identifies the HorizontalBarSmallChange dependency property.
    /// </summary>
    public static readonly DependencyProperty HorizontalBarSmallChangeProperty =
   DependencyProperty.RegisterAttached("HorizontalBarSmallChange", typeof(double), typeof(ChartZoomingScrollBar), new FrameworkPropertyMetadata(0.1d));

    /// <summary>
    ///  Identifies the VerticalBarLargeChange dependency property.
    /// </summary>
    public static readonly DependencyProperty VerticalBarLargeChangeProperty =
   DependencyProperty.RegisterAttached("VerticalBarLargeChange", typeof(double), typeof(ChartZoomingScrollBar), new FrameworkPropertyMetadata(0.5d));

    /// <summary>
    ///  Identifies the VerticalBarSmallChange dependency property.
    /// </summary>
    public static readonly DependencyProperty VerticalBarSmallChangeProperty =
   DependencyProperty.RegisterAttached("VerticalBarSmallChange", typeof(double), typeof(ChartZoomingScrollBar), new FrameworkPropertyMetadata(0.1d));

    /// <summary>
    /// Identifies the VerticalBarVisibility dependency property.
    /// </summary>
    public static readonly DependencyProperty VerticalBarVisibilityProperty =
DependencyProperty.RegisterAttached("VerticalBarVisibility", typeof(Visibility), typeof(ChartZoomingScrollBar), new FrameworkPropertyMetadata(Visibility.Visible));

    /// <summary>
    ///  Identifies the HorizontalBarVisibility dependency property.
    /// </summary>
    public static readonly DependencyProperty HorizontalBarVisibilityProperty =
   DependencyProperty.RegisterAttached("HorizontalBarVisibility", typeof(Visibility), typeof(ChartZoomingScrollBar), new FrameworkPropertyMetadata(Visibility.Visible));

    #endregion

    #region Properties

    /// <summary>
    /// Return HorizontalBarLargeChange value from the given area
    /// </summary>
    /// <param name="area"></param>
    /// <returns></returns>
    public static double GetHorizontalBarLargeChange(ChartArea area)
    {
        return (double)area.GetValue(HorizontalBarLargeChangeProperty);
    }

    /// <summary>
    /// Set HorizontalBarLargeChangevalue from the given value
    /// </summary>
    /// <param name="area"></param>
    /// <param name="value"></param>
    public static void SetHorizontalBarLargeChange(ChartArea area, double value)
    {
        area.SetValue(HorizontalBarLargeChangeProperty, value);       
    }
    

    /// <summary>
    /// return HorizontalBarSmallChange value from the given area
    /// </summary>
    /// <param name="area"></param>
    /// <returns></returns>
    public static double GetHorizontalBarSmallChange(ChartArea area)
    {
        return (double)area.GetValue(HorizontalBarSmallChangeProperty);
    }


    /// <summary>
    /// Set the HorizontalBarSmallChange value from the given double value
    /// </summary>
    /// <param name="area"></param>
    /// <param name="value"></param>
    public static void SetHorizontalBarSmallChange(ChartArea area, double value)
    {
        area.SetValue(HorizontalBarSmallChangeProperty, value);      
    }

    /// <summary>
    /// Return VerticalBarLargeChange value from given value
    /// </summary>
    /// <param name="area"></param>
    /// <returns></returns>
    public static double GetVerticalBarLargeChange(ChartArea area)
    {
        return (double)area.GetValue(VerticalBarLargeChangeProperty);
    }

    

    /// <summary>
    /// Set VerticalBarLargeChange value to the corresponding given area from the given double value
    /// </summary>
    /// <param name="area"></param>
    /// <param name="value"></param>
    public static void SetVerticalBarLargeChange(ChartArea area, double value)
    {
        area.SetValue(VerticalBarLargeChangeProperty, value);      
    }

    /// <summary>
    /// Return VerticalBarSmallChange value from the given area
    /// </summary>
    /// <param name="area"></param>
    /// <returns></returns>
    public static double GetVerticalBarSmallChange(ChartArea area)
    {
        return (double)area.GetValue(VerticalBarSmallChangeProperty);
    }


    /// <summary>
    /// Set the VerticalBarSmallChange value to the corresponding given area from the given double value.
    /// </summary>
    /// <param name="area"></param>
    /// <param name="value"></param>
    public static void SetVerticalBarSmallChange(ChartArea area, double value)
    {
        area.SetValue(VerticalBarSmallChangeProperty, value);       
    }

    /// <summary>
    /// Return HorizontalBarVisibility value from the given area
    /// </summary>
    /// <param name="area"></param>
    /// <returns></returns>
    public static Visibility GetHorizontalBarVisibility(ChartArea area)
    {
        return (Visibility)area.GetValue(HorizontalBarVisibilityProperty);
    }


    /// <summary>
    /// Set HorizontalBarVisibility value to the corresponding given area from the given value.
    /// </summary>
    /// <param name="area"></param>
    /// <param name="value"></param>
    public static void SetHorizontalBarVisibility(ChartArea area, Visibility value)
    {
        area.SetValue(HorizontalBarVisibilityProperty, value);
    }

    /// <summary>
    /// Return VerticalBarVisibility value from the given area
    /// </summary>
    /// <param name="area"></param>
    /// <returns></returns>
    public static Visibility GetVerticalBarVisibility(ChartArea area)
    {
        return (Visibility)area.GetValue(VerticalBarVisibilityProperty);
    }



    /// <summary>
    /// Set the VerticalBarVisibility value to the corresponding given area from the given value
    /// </summary>
    /// <param name="area"></param>
    /// <param name="value"></param>
    public static void SetVerticalBarVisibility(ChartArea area, Visibility value)
    {
        area.SetValue(VerticalBarVisibilityProperty, value);
    }
    /// <summary>
    /// Gets or sets a value indicating whether zoom is switched.
    /// </summary>
    /// <value><c>true</c> if zoom is active; otherwise, <c>false</c>.</value>
    public bool ZoomSwitched
    {
      get { return (bool)GetValue(ZoomSwitchedProperty); }
      set { SetValue(ZoomSwitchedProperty, value); }
    }

    /// <summary>
    /// Gets or sets the zoom factor.
    /// </summary>
    /// <value>The zoom factor.</value>
    public double ZoomFactor
    {
      get { return (double)GetValue(ZoomFactorProperty); }
      set { SetValue(ZoomFactorProperty, value); }
    }

    /// <summary>
    /// Gets or sets the zoom position.
    /// </summary>
    /// <value>The zoom position.</value>
    public double ZoomPosition
    {
      get { return (double)GetValue(ZoomPositionProperty); }
      set { SetValue(ZoomPositionProperty, value); }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is inversed.
    /// </summary>
    /// <value>
    ///  <c>true</c> if this instance is inversed; otherwise, <c>false</c>.
    /// </value>
    public bool IsInversed
    {
      get { return (bool)GetValue(IsInversedProperty); }
      set { SetValue(IsInversedProperty, value); }
    }

    /// <summary>
    /// Gets or sets the orientation.
    /// </summary>
    /// <value>The orientation.</value>
    public Orientation Orientation
    {
      get { return (Orientation)GetValue(OrientationProperty); }
      set { SetValue(OrientationProperty, value); }
    }
    #endregion

    #region Constructor
    /// <summary>
    /// Initializes static members of the <see cref="ChartZoomingScrollBar"/> class.
    /// </summary>
    static ChartZoomingScrollBar()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ChartZoomingScrollBar), new FrameworkPropertyMetadata(typeof(ChartZoomingScrollBar)));
    }
    #endregion

    #region Implemntation
    /// <summary>
    /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"></see>.
    /// </summary>
    /// <seealso cref="ChartZoomingScrollBar"/>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      m_scrollBar = this.GetTemplateChild("PART_ScrollBar") as ScrollBar;
      m_scrollBar.Scroll += new ScrollEventHandler(m_scrollBar_Scroll);     
        DependencyObject a = new DependencyObject();
        DependencyObject chartArea = new DependencyObject();
        DependencyObject syncChartArea = new DependencyObject();
        a = VisualTreeHelper.GetParent(this);
        while (a != null)
        {
            if (VisualTreeHelper.GetParent(a) is SyncChartAreas == true)
            {
                syncChartArea = VisualTreeHelper.GetParent(a);
                break;
            }
            if (VisualTreeHelper.GetParent(a) is ChartArea == true)
            {
                chartArea = VisualTreeHelper.GetParent(a);
            }
            a = VisualTreeHelper.GetParent(a);
        }

        if (m_scrollBar != null)
        {
            Binding Hor_bar_visi = new Binding();
            Hor_bar_visi.Path = new PropertyPath(ChartZoomingScrollBar.HorizontalBarVisibilityProperty);
            Hor_bar_visi.Source = chartArea as ChartArea;
            Hor_bar_visi.Mode = BindingMode.TwoWay;
            Binding Ver_bar_visi = new Binding();
            Ver_bar_visi.Path = new PropertyPath(ChartZoomingScrollBar.VerticalBarVisibilityProperty);
            Ver_bar_visi.Source = chartArea as ChartArea;
            Ver_bar_visi.Mode = BindingMode.TwoWay;

            MultiBinding visibilityBinding = new MultiBinding();
            Binding valueBinding = new Binding();
            Binding viewportSizeBinding = new Binding();

            visibilityBinding.Converter = new ZoomFactorToVisibilityConverter();
            visibilityBinding.Bindings.Add(new Binding("ZoomFactor") { Source = this });
            visibilityBinding.Bindings.Add(new Binding("ZoomSwitched") { Source = this });
            visibilityBinding.Bindings.Add(new Binding("ChartAreaIndex") { Source = chartArea as ChartArea});
            visibilityBinding.Bindings.Add(new Binding("ChartAreaCount") { Source = chartArea as ChartArea });
            visibilityBinding.Bindings.Add(new Binding("IsSyncChartArea") { Source = syncChartArea as SyncChartAreas});
            visibilityBinding.Bindings.Add(new Binding("Orientation") { Source = this });
            visibilityBinding.ConverterParameter = chartArea as ChartArea;


        viewportSizeBinding.Source = this;
        viewportSizeBinding.Converter = new ZoomFactorToViewportSizeConverter();
        viewportSizeBinding.Path = new PropertyPath(ChartZoomingScrollBar.ZoomFactorProperty);

        
        valueBinding.Source = this;
        valueBinding.Converter = new ZoomPositionToValueConverter(this);
        valueBinding.Path = new PropertyPath(ChartZoomingScrollBar.ZoomPositionProperty);
        valueBinding.Mode = BindingMode.TwoWay;

        Binding Hor_lar_chng = new Binding();
        Hor_lar_chng.Path = new PropertyPath(ChartZoomingScrollBar.HorizontalBarLargeChangeProperty);
        Hor_lar_chng.Source = chartArea as ChartArea;
        Hor_lar_chng.Mode = BindingMode.TwoWay;
        Binding Hor_small_chng = new Binding();
        Hor_small_chng.Path = new PropertyPath(ChartZoomingScrollBar.HorizontalBarSmallChangeProperty);
        Hor_small_chng.Source = chartArea as ChartArea;
        Hor_small_chng.Mode = BindingMode.TwoWay;
       //double d = ChartZoomingScrollBar.GetHorizontalBarSmallChange(chartArea as ChartArea);
        Binding Ver_lar_chng = new Binding();
        Ver_lar_chng.Path = new PropertyPath(ChartZoomingScrollBar.VerticalBarLargeChangeProperty);
        Ver_lar_chng.Source = chartArea as ChartArea;
        Ver_lar_chng.Mode = BindingMode.TwoWay;
        Binding Ver_small_chng = new Binding();
        Ver_small_chng.Path = new PropertyPath(ChartZoomingScrollBar.VerticalBarSmallChangeProperty);
        Ver_small_chng.Source = chartArea as ChartArea;
        Ver_small_chng.Mode = BindingMode.TwoWay;




        
       
        m_scrollBar.SetBinding(ScrollBar.ViewportSizeProperty, viewportSizeBinding);
        m_scrollBar.SetBinding(ScrollBar.ValueProperty, valueBinding);

        BindingUtils.SetBinding(m_scrollBar, this, ScrollBar.OrientationProperty, ChartZoomingScrollBar.OrientationProperty);
        if (m_scrollBar.Orientation == System.Windows.Controls.Orientation.Vertical)
        {
            m_scrollBar.SetBinding(ScrollBar.LargeChangeProperty, Ver_lar_chng);
            m_scrollBar.SetBinding(ScrollBar.SmallChangeProperty, Ver_small_chng);            
            visibilityBinding.Bindings.Add(Ver_bar_visi);   
        }
        else
        {
            m_scrollBar.SetBinding(ScrollBar.LargeChangeProperty, Hor_lar_chng);
            m_scrollBar.SetBinding(ScrollBar.SmallChangeProperty, Hor_small_chng);
            visibilityBinding.Bindings.Add(Hor_bar_visi);
        }
        m_scrollBar.SetBinding(ScrollBar.VisibilityProperty, visibilityBinding);
      }
    }

    ChartArea m_chartArea;
    void m_scrollBar_Scroll(object sender, ScrollEventArgs e)
    {
        if (this.m_chartArea == null)
        {
            m_chartArea = GetChartArea(sender as ScrollBar) as ChartArea;
            if (m_chartArea != null && !m_chartArea.m_scrolling)
            {
                m_chartArea.m_scrolling = true;
            }
        }
        else
        {
            this.m_chartArea.m_scrolling = false;
        }
    }

    object GetChartArea(ScrollBar SB)
    {
        var obj = VisualTreeHelper.GetParent(SB);
        while (!(obj is ChartArea))
        {
            obj = VisualTreeHelper.GetParent(obj);
        }
        return obj;
    }

            
   

    /// <summary>
    /// Called when zoom factor is changed.
    /// </summary>
    /// <param name="dObj">The d obj.</param>
    /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    private static void OnZoomFactorChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs args)
    {
      ChartZoomingScrollBar cZmSrBr = dObj as ChartZoomingScrollBar;

      if (cZmSrBr != null && cZmSrBr.m_scrollBar != null)
      {
        BindingExpression expression = BindingOperations.GetBindingExpression(cZmSrBr.m_scrollBar, ScrollBar.ValueProperty);
          if(expression != null)
        expression.UpdateTarget();
      }
    }
    #endregion

      /// <summary>
      /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
      /// </summary>
      /// <filterpriority>2</filterpriority>
      public void Dispose()
    {
        if(m_scrollBar != null)
        m_scrollBar.Scroll -= new ScrollEventHandler(m_scrollBar_Scroll);     
    }
  }

  /// <summary>
  /// Represents chart zooming adorner class that provides selecting operation in zooming mode.
  /// </summary>
  /// <exclude/>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
  internal class ChartZoomingAdorner : Adorner, IDisposable
  {
    #region Members
    /// <summary>
    /// Initializes m_chartArea
    /// </summary>
    private ChartArea m_chartArea = null;

    /// <summary>
    /// Initializes m_canvas
    /// </summary>
    private Canvas m_canvas = new Canvas();

    /// <summary>
    /// Initializes m_zoomRectangle
    /// </summary>
    private Rectangle m_zoomRectangle = new Rectangle();

    /// <summary>
    /// Initializes m_zoomingToolBar
    /// </summary>
    private ChartZoomingToolkit m_zoomingToolBar;

    /// <summary>
    /// Initializes m_elements
    /// </summary>
    private UIElementCollection m_elements = null;

    /// <summary>
    /// Initializes m_startZoomBarPoint
    /// </summary>
    private Point m_startZoomBarPoint = new Point();

    /// <summary>
    /// Initializes m_endZoomBarPoint
    /// </summary>
    private Point m_endZoomBarPoint = new Point();

    /// <summary>
    /// Initializes m_isZooming
    /// </summary>
    private bool m_isZooming;
    #endregion

    #region Properties
    /// <summary>
    /// Gets the number of visual child elements within this element.
    /// </summary>
    /// <value></value>
    /// <returns>The number of visual child elements for this element.</returns>
    protected override int VisualChildrenCount
    {
      get
      {
        return m_elements.Count;
      }
    }
    #endregion

    #region Constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="ChartZoomingAdorner"/> class.
    /// </summary>
    /// <param name="area">The area value.</param>
    /// <param name="container">The container.</param>
    public ChartZoomingAdorner(ChartArea area, UIElement container)
      : base(container)
    {
      m_chartArea = area;
      m_elements = new UIElementCollection(this, this);

      m_zoomingToolBar = new ChartZoomingToolkit(area);

            if (area.IsSync == true)
            {
                SyncChartAreas syncChartArea = area.ChartAreaParent;
                syncChartArea.MouseDown += new MouseButtonEventHandler(syncChartArea_MouseDown);
                syncChartArea.MouseMove += new MouseEventHandler(syncChartArea_MouseMove);
                syncChartArea.MouseUp += new MouseButtonEventHandler(syncChartArea_MouseUp);
            }
            else
            {
                area.MouseDown += new MouseButtonEventHandler(this.OnChartAreaMouseDown);
                area.MouseMove += new MouseEventHandler(this.OnChartAreaMouseMove);
                area.MouseUp += new MouseButtonEventHandler(this.OnChartAreaMouseUp);
            }

      m_zoomRectangle.Visibility = Visibility.Collapsed;
      m_zoomRectangle.Fill = new SolidColorBrush(Color.FromArgb(0xC0, 0xFF, 0xFF, 0xFF));
      m_zoomRectangle.Stroke = Brushes.Black;
      m_zoomRectangle.Stretch = Stretch.Fill;
      m_canvas.Children.Add(m_zoomRectangle);

      BindingUtils.SetBinding(m_canvas, m_chartArea, Control.MarginProperty, ChartArea.AxesThicknessProperty);
      BindingUtils.SetBinding(m_zoomingToolBar, m_chartArea, Control.MarginProperty, ChartArea.AxesThicknessProperty);

      m_elements.Add(m_canvas);
      m_elements.Add(m_zoomingToolBar);
    }
        void syncChartArea_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_isZooming)
            {
                Point pt = e.GetPosition(sender as SyncChartAreas);
                SyncChartAreas syncChartArea = sender as SyncChartAreas;
                ChartArea area = syncChartArea.Areas[syncChartArea.Areas.Count - 1] as ChartArea;
                double m_canvasTop = 0d;
                double m_canvasLeft = 0d;
                if (syncChartArea.Areas.Count > 0 && syncChartArea.Areas[0] != null && syncChartArea.Areas[0].m_areaPresenter != null)
                {
                    DependencyObject obj = VisualTreeHelper.GetChild(syncChartArea.Areas[0].m_areaPresenter, 0);
                    obj = VisualTreeHelper.GetChild(obj, 1);
                    obj = VisualTreeHelper.GetChild(obj, 2);
                    m_canvasTop = e.GetPosition(syncChartArea).Y - e.GetPosition(syncChartArea.Areas[0].m_areaPresenter).Y;
                    m_canvasLeft = (obj is Grid) ? e.GetPosition(syncChartArea).X - e.GetPosition(obj as Grid).X : m_canvasLeft;
                }
                pt.X = ChartMath.MinMax(pt.X, m_canvasLeft, syncChartArea.ActualWidth);
                pt.Y = ChartMath.MinMax(pt.Y, m_canvasTop, syncChartArea.ActualHeight);
                m_endZoomBarPoint = pt;
                Rect zoomRect = new Rect(m_startZoomBarPoint, m_endZoomBarPoint);
                Canvas.SetLeft(m_zoomRectangle, zoomRect.X - m_canvasLeft);
                m_zoomRectangle.Width = zoomRect.Width;
                if (area != null)
                {
                    double value = area.PointToValue(area.SecondaryAxis, e.GetPosition(area));
                    if (value > area.SecondaryAxis.VisibleRange.Start)
                    {
                        int count = syncChartArea.Areas.Count - 1;
                        double Height = 0d;
                        foreach (ChartArea sarea in syncChartArea.Areas)
                        {
                            DependencyObject obj = VisualTreeHelper.GetChild(sarea.m_areaPresenter, 0) as DependencyObject;
                            obj = VisualTreeHelper.GetChild(obj, 1);
                            obj = VisualTreeHelper.GetChild(obj, 2);
                            Grid grid = obj as Grid;
                            Height = Height + grid.ActualHeight;
                        }
                        if (syncChartArea.Areas[0].PrimaryAxis.ZoomFactor == 1)
                        {
                            Height = Height + 17;
                        }

                        m_zoomRectangle.Height = Height + syncChartArea.Areas[count].ValueToPoint(syncChartArea.Areas[count].SecondaryAxis, syncChartArea.Areas[count].SecondaryAxis.VisibleRange.End);// syncChartArea.ActualHeight - AreaBottomHeight;
                    }
                }
            }
        }



        void syncChartArea_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && !(e.Device.Target is ChartLegend) && !(e.Device.Target is ContentPresenter))
            {
                double axesLeft = 0;
                double axesBottom = 0, axesTop = 0, axesRight = 0, areaAll = 0;
                double xStart = 0, xEnd = 0, start = 0, areaHeight, end, syncTop = 0;
                m_zoomRectangle.Height = 0;
                m_zoomRectangle.Width = 0;
                Point pt = e.GetPosition(sender as SyncChartAreas);
                SyncChartAreas syncChartArea = sender as SyncChartAreas;
                xEnd = syncChartArea.ActualWidth;

                foreach (ChartArea SyncArea in syncChartArea.Areas)
                {
                    axesLeft = axesLeft > SyncArea.AxesThickness.Left ? axesLeft : SyncArea.AxesThickness.Left;
                    axesBottom = SyncArea.ValueToPoint(SyncArea.SecondaryAxis, SyncArea.SecondaryAxis.VisibleRange.Start) + areaAll;
                    axesRight = axesRight > SyncArea.AxesThickness.Right ? axesRight : SyncArea.AxesThickness.Right;
                    areaAll = areaAll + SyncArea.ActualHeight;
                    xStart = xStart > SyncArea.ValueToPoint(SyncArea.PrimaryAxis, SyncArea.PrimaryAxis.VisibleRange.Start) ? xStart : SyncArea.ValueToPoint(SyncArea.PrimaryAxis, SyncArea.PrimaryAxis.VisibleRange.Start);
                    xEnd = xEnd < SyncArea.ValueToPoint(SyncArea.PrimaryAxis, SyncArea.PrimaryAxis.VisibleRange.End) ? xEnd : SyncArea.ValueToPoint(SyncArea.PrimaryAxis, SyncArea.PrimaryAxis.VisibleRange.End);
                }
                syncTop = syncChartArea.ActualHeight - areaAll;
                axesTop = syncChartArea.Areas[0].ValueToPoint(syncChartArea.Areas[0].SecondaryAxis, syncChartArea.Areas[0].SecondaryAxis.VisibleRange.End);
                areaAll = 0;


                foreach (ChartArea area in syncChartArea.Areas)
                {
                    start = area.ValueToPoint(area.SecondaryAxis, area.SecondaryAxis.VisibleRange.Start);
                    end = area.ValueToPoint(area.SecondaryAxis, area.SecondaryAxis.VisibleRange.End);
                    areaHeight = area.Height;

                    if ((areaHeight - start > 0 && pt.Y < start + areaAll + syncTop && pt.Y > end + areaAll + syncTop) && (pt.X > xStart && pt.X < xEnd + axesRight) && syncChartArea.EnableMouseDragZooming)
                    {
                        pt.X = ChartMath.MinMax(pt.X, xStart, xEnd + axesRight);
                        pt.Y = ChartMath.MinMax(pt.Y, end + areaAll, start + areaAll);
                        syncChartArea.CaptureMouse();
                        m_startZoomBarPoint = pt;
                        if ((syncChartArea.Cursor != Cursors.Hand) && (syncChartArea.EnableMouseDragZooming))
                        {
                            m_zoomRectangle.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            m_zoomRectangle.Visibility = Visibility.Collapsed;
                        }

                        m_isZooming = true;
                        break;
                    }
                    areaAll += areaHeight;
                }

            }

        }
        void syncChartArea_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (m_isZooming)
            {

                m_zoomRectangle.Visibility = Visibility.Collapsed;
                SyncChartAreas syncChartArea = sender as SyncChartAreas;
                syncChartArea.ReleaseMouseCapture();

                Point zp1 = syncChartArea.TranslatePoint(m_startZoomBarPoint, syncChartArea);
                Point zp2 = syncChartArea.TranslatePoint(m_endZoomBarPoint, syncChartArea);
                Rect rect = new Rect(zp1, zp2);
                if (rect.Width > 0 && rect.Height > 0)
                {
                    syncChartArea.IsMouseDragZooming = true;
                    ChartAreaCommands.ZoomSector.Execute(rect, m_chartArea);
                    syncChartArea.IsMouseDragZooming = false;
                }
            }
            m_zoomRectangle.Height = 0;
            m_zoomRectangle.Width = 0;

            m_isZooming = false;
        }
    #endregion

    #region Implementation
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      if (m_chartArea != null)
      {
          
              m_chartArea.MouseDown -= new MouseButtonEventHandler(this.OnChartAreaMouseDown);
              m_chartArea.MouseMove -= new MouseEventHandler(this.OnChartAreaMouseMove);
              m_chartArea.MouseUp -= new MouseButtonEventHandler(this.OnChartAreaMouseUp);
        
        m_chartArea = null;
      }

      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement"></see> derived class.
    /// </summary>
    /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
    /// <returns>The actual size used.</returns>
    protected override Size ArrangeOverride(Size finalSize)
    {
      m_canvas.Arrange(new Rect(finalSize));
      m_zoomingToolBar.Arrange(new Rect(finalSize));

      return base.ArrangeOverride(finalSize);
    }

    /// <summary>
    /// Overrides <see cref="M:System.Windows.Media.Visual.GetVisualChild(System.Int32)"></see>, and returns a child at the specified index from a collection of child elements.
    /// </summary>
    /// <param name="index">The zero-based index of the requested child element in the collection.</param>
    /// <returns>
    /// The requested child element. This should not return null; if the provided index is out of range, an exception is raised.
    /// </returns>
    protected override Visual GetVisualChild(int index)
    {
      return m_elements[index];
    }

    /// <summary>
    /// Occurs On ChartAreaMouseDown
    /// </summary>
    /// <param name="sender">The object sender</param>
    /// <param name="e">The MouseButtonEvent Arguments e</param>
    private void OnChartAreaMouseDown(object sender, MouseButtonEventArgs e)
    {
      if (e.ChangedButton == MouseButton.Left && !m_chartArea.AllowSegmentDragDrop)
      {
        Point mPt = e.GetPosition(m_canvas);

        mPt.X = ChartMath.MinMax(mPt.X, 0, m_canvas.ActualWidth);
        mPt.Y = ChartMath.MinMax(mPt.Y, 0, m_canvas.ActualHeight);

        
        if ((m_chartArea.Cursor != Cursors.Hand)&&(m_chartArea.IsSync!=true)&&(m_chartArea.EnableMouseDragZooming))
        {
            m_chartArea.CaptureMouse();
            m_startZoomBarPoint = mPt;

            Canvas.SetLeft(m_zoomRectangle, m_startZoomBarPoint.X);
            Canvas.SetTop(m_zoomRectangle, m_startZoomBarPoint.Y);

            m_zoomRectangle.Width = 0;
            m_zoomRectangle.Height = 0;
            m_zoomRectangle.Visibility = Visibility.Visible;
        }
        else
        {
            m_zoomRectangle.Visibility = Visibility.Collapsed;
        }
        m_isZooming = true;
      }
    }

    /// <summary>
    /// Occurs on ChartAreaMouseMove
    /// </summary>
    /// <param name="sender">The object sender</param>
    /// <param name="e">The MouseEvent Arguments e</param>
    private void OnChartAreaMouseMove(object sender, MouseEventArgs e)
    {
      if (m_isZooming)
      {
         ChartArea area = sender as ChartArea;

         Point mPt = e.GetPosition(m_canvas);
         Rect canvasRect = new Rect(m_canvas.RenderSize);

         double Xaxisvalue = area.PointToValue(area.SecondaryAxis, e.GetPosition(sender as ChartArea));
         double Yaxisvalue = area.PointToValue(area.PrimaryAxis, e.GetPosition(sender as ChartArea));

         mPt.X = ChartMath.MinMax(mPt.X, 0, m_canvas.ActualWidth);
         mPt.Y = ChartMath.MinMax(mPt.Y, 0, m_canvas.ActualHeight);
         m_endZoomBarPoint = mPt;

         Rect zoomRect = new Rect(m_startZoomBarPoint, m_endZoomBarPoint);
         if (area.SecondaryAxis.VisibleRange.Start <= (Xaxisvalue))
         {
			Canvas.SetTop(m_zoomRectangle, zoomRect.Y);
			m_zoomRectangle.Height = zoomRect.Height;
         }
         if (area.PrimaryAxis.VisibleRange.End >= Yaxisvalue)
         {
			Canvas.SetLeft(m_zoomRectangle, zoomRect.X);
			m_zoomRectangle.Width = zoomRect.Width;
         }
      }
    }

    /// <summary>
    /// Occurs on ChartAreaMouseUp
    /// </summary>
    /// <param name="sender">The object sender</param>
    /// <param name="e">The MouseButtonEvent Arguments e</param>
    private void OnChartAreaMouseUp(object sender, MouseButtonEventArgs e)
    {
      if (m_isZooming)
      {
        m_zoomRectangle.Visibility = Visibility.Collapsed;
        m_chartArea.ReleaseMouseCapture();

        Point zp1 = m_canvas.TranslatePoint(m_startZoomBarPoint, m_chartArea);
        Point zp2 = m_canvas.TranslatePoint(m_endZoomBarPoint, m_chartArea);
        Rect rect = new Rect(zp1, zp2);

        if (rect.Width > 0 && rect.Height > 0 && m_chartArea !=null)
        {
          m_chartArea.IsMouseDragZooming = true;
          ChartAreaCommands.ZoomSector.Execute(rect, m_chartArea);
          m_chartArea.IsMouseDragZooming = false;
        }
      }

      m_isZooming = false;
    }

    #endregion
  }
}
