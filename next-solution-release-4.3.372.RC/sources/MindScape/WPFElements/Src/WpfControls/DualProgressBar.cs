using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using Infralution.Licensing;
using System.Reflection;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A progress bar that can display two indicators, one for the overall progress, and another for
  /// the progress of the current sub-operation.
  /// </summary>
  [LicenseProvider(typeof(PublicEncryptedLicenseProvider))]
  public class DualProgressBar : ProgressBar
  {
    private Border _track;
    private Border _indicator;
    private Border _nestedIndicator;

    static DualProgressBar()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(DualProgressBar),
        new FrameworkPropertyMetadata(typeof(DualProgressBar)));
      MaximumProperty.OverrideMetadata(typeof(DualProgressBar),
        new FrameworkPropertyMetadata(100.0));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DualProgressBar"/> class.
    /// </summary>
    public DualProgressBar()
    {
      // Licensing
      //new WpfElementsCore(Assembly.GetCallingAssembly());
      //LicenseHelper.Attach(this, Assembly.GetCallingAssembly());
      // End licensing

      ValueChanged += DualProgressBar_ValueChanged;
      SizeChanged += new SizeChangedEventHandler(DualProgressBar_SizeChanged);
    }

    private void DualProgressBar_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      UpdateControlPositions();
    }

    private void DualProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      UpdateControlPositions();
    }

    /// <summary>
    /// Called when the value of the Minimum property changes.
    /// </summary>
    /// <param name="oldMinimum">The old property value.</param>
    /// <param name="newMinimum">The new property value.</param>
    protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
    {
      base.OnMinimumChanged(oldMinimum, newMinimum);

      UpdateControlPositions();
    }

    /// <summary>
    /// Called when the value of the Maximum property changes.
    /// </summary>
    /// <param name="oldMaximum">The old property value.</param>
    /// <param name="newMaximum">The new property value.</param>
    protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
    {
      base.OnMaximumChanged(oldMaximum, newMaximum);

      UpdateControlPositions();
    }

    /// <summary>
    /// Called by the framework when the control template is applied.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      _track = GetTemplateChild("PART_Track") as Border;
      _indicator = GetTemplateChild("PART_Indicator") as Border;
      _nestedIndicator = GetTemplateChild("PART_NestedIndicator") as Border;
      Dispatcher.BeginInvoke(new Action(UpdateControlPositions));
    }

    private void UpdateControlPositions()
    {
      if (_track != null)
      {
        double size = _track.ActualWidth;
        if (Orientation == Orientation.Vertical)
        {
          size = _track.ActualHeight;
        }
        double logicalWidth = Maximum - Minimum;
        double ratio = Value / logicalWidth;
        double physicalValue = size * ratio;

        double nestedSize = physicalValue * (NestedPercentage / 100);

        if (Orientation == Orientation.Horizontal)
        {
          if (_indicator != null)
          {
            _indicator.Width = physicalValue;
          }
          if (_nestedIndicator != null)
          {
            _nestedIndicator.Width = nestedSize;
          }
        }
        else if (Orientation == Orientation.Vertical)
        {
          if (_indicator != null)
          {
            _indicator.Height = physicalValue;
          }
          if (_nestedIndicator != null)
          {
            _nestedIndicator.Height = nestedSize;
          }
        }
      }
    }

    #region CenterContent property

    /// <summary>
    /// Gets or sets the content to be displayed in the center of the <see cref="DualProgressBar"/>.
    /// This is a dependency property.
    /// </summary>
    public object CenterContent
    {
      get { return GetValue(CenterContentProperty); }
      set { SetValue(CenterContentProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="CenterContent"/> property.
    /// </summary>
    public static readonly DependencyProperty CenterContentProperty =
      DependencyProperty.Register("CenterContent", typeof(object), typeof(DualProgressBar),
      null);

    #endregion // CenterContent property

    #region StartContent property

    /// <summary>
    /// Gets or sets the content to be displayed at the start of the <see cref="DualProgressBar"/>.
    /// This is a dependency property.
    /// </summary>
    public object StartContent
    {
      get { return GetValue(StartContentProperty); }
      set { SetValue(StartContentProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="StartContent"/> property.
    /// </summary>
    public static readonly DependencyProperty StartContentProperty =
      DependencyProperty.Register("StartContent", typeof(object), typeof(DualProgressBar),
      null);

    #endregion // StartContent property

    #region EndContent property

    /// <summary>
    /// Gets or sets the content to be displayed at the end of the <see cref="DualProgressBar"/>.
    /// This is a dependency property.
    /// </summary>
    public object EndContent
    {
      get { return GetValue(EndContentProperty); }
      set { SetValue(EndContentProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="EndContent"/> property.
    /// </summary>
    public static readonly DependencyProperty EndContentProperty =
      DependencyProperty.Register("EndContent", typeof(object), typeof(DualProgressBar),
      null);

    #endregion // EndContent property

    #region NestedPercentage property

    /// <summary>
    /// Gets or sets the percentage of the current progress bar to be displayed as complete.
    /// This is a dependency property.
    /// </summary>
    public double NestedPercentage
    {
      get { return (double)GetValue(NestedPercentageProperty); }
      set { SetValue(NestedPercentageProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="NestedPercentage"/> property.
    /// </summary>
    public static readonly DependencyProperty NestedPercentageProperty =
      DependencyProperty.Register("NestedPercentage", typeof(double), typeof(DualProgressBar),
      new PropertyMetadata(100.0, OnNestedPercentageChanged));

    private static void OnNestedPercentageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((DualProgressBar)d).OnNestedPercentageChanged();
    }

    private void OnNestedPercentageChanged()
    {
      UpdateControlPositions();
    }

    #endregion // NestedPercentage property
  }
}
