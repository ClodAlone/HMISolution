using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using Infralution.Licensing;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Controls.Primitives;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A color picker control for editing the hue, saturation and value (brightness) of a color using a <see cref="ColorSquare"/> and a hue slider.
  /// </summary>
  [LicenseProvider(typeof(PublicEncryptedLicenseProvider))]
  public class HsvColorPicker : NotifyingColorPickerBase
  {
    static HsvColorPicker()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(HsvColorPicker),
        new FrameworkPropertyMetadata(typeof(HsvColorPicker)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HsvColorPicker"/> class.
    /// </summary>
    public HsvColorPicker()
    {
      // Licensing
      //new WpfElementsCore(Assembly.GetCallingAssembly());
      //LicenseHelper.Attach(this, Assembly.GetCallingAssembly());
      // End licensing
    }

    /// <summary>
    /// Called by the framework when the control template is applied.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      Slider slider = GetTemplateChild("PART_Slider") as Slider;
      if (slider != null)
      {
        slider.PreviewMouseDown += new MouseButtonEventHandler(Slider_MouseDown);
        slider.MouseUp += new MouseButtonEventHandler(Slider_MouseUp);
        slider.MouseMove += new MouseEventHandler(Slider_MouseMove);
      }
    }

    private void Slider_MouseMove(object sender, MouseEventArgs e)
    {
      Slider slider = sender as Slider;
      if (Mouse.Captured == slider)
      {
        Point point = e.GetPosition(slider);
        UpdateSlider(slider, point);
      }
    }

    private void Slider_MouseUp(object sender, MouseButtonEventArgs e)
    {
      Slider slider = sender as Slider;
      slider.ReleaseMouseCapture();
    }

    private void Slider_MouseDown(object sender, MouseButtonEventArgs e)
    {
      Thumb thumb = VisualTreeUtils.FindAncestor<Thumb>(Mouse.DirectlyOver as DependencyObject);
      if (thumb == null)
      {
        Slider slider = sender as Slider;
        Point point = e.GetPosition(slider);
        UpdateSlider(slider, point);
        slider.CaptureMouse();
      }
    }

    private void UpdateSlider(Slider slider, Point point)
    {
      double logicalSize = slider.Maximum - slider.Minimum;
      double physicalSize = slider.ActualHeight;
      double ratio = logicalSize / physicalSize;
      double physical = Math.Max(0, Math.Min(physicalSize, point.Y));
      double logical = physical * ratio;
      slider.Value = logical;
    }
  }
}
