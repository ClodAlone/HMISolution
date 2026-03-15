using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows;
using System.Windows.Media.Effects;

namespace Mindscape.WpfElements.Charting
{
  internal class Watermark : FrameworkElement
  {
    internal const string _watermarkText = "Mindscape WPF Charts";

    private readonly TextBlock _watermark;

    internal Watermark(TextBlock watermark)
    {
      _watermark = watermark;
    }

    internal static void AddWatermark(Canvas canvas)
    {
      AddWatermark(canvas, false);
    }

    internal static void AddWatermark(Canvas canvas, bool centerAlignment)
    {
      if (canvas != null)
      {
        TextBlock watermark = new TextBlock();
        watermark.Foreground = new SolidColorBrush(Colors.LightGray);
        watermark.Text = _watermarkText;
        watermark.IsHitTestVisible = false;
        Canvas.SetLeft(watermark, canvas.ActualWidth - 126);
        if (centerAlignment)
        {
          Canvas.SetLeft(watermark, canvas.ActualWidth / 2 - 63);
        }
        Canvas.SetTop(watermark, canvas.ActualHeight - 15);
        Canvas.SetZIndex(watermark, 1000000);
        canvas.Children.Add(watermark);

        Watermark wm = new Watermark(watermark);

        Binding visibilityBinding = new Binding("Visibility");
        visibilityBinding.Source = watermark;
        wm.SetBinding(Watermark.WatermarkVisibilityProperty, visibilityBinding);

        Binding opacityBinding = new Binding("Opacity");
        opacityBinding.Source = watermark;
        wm.SetBinding(Watermark.WatermarkOpacityProperty, opacityBinding);

        Binding marginBinding = new Binding("Margin");
        marginBinding.Source = watermark;
        wm.SetBinding(Watermark.WatermarkMarginProperty, marginBinding);

        Binding paddingBinding = new Binding("Padding");
        paddingBinding.Source = watermark;
        wm.SetBinding(Watermark.WatermarkPaddingProperty, paddingBinding);

        Binding renderTransformBinding = new Binding("RenderTransform");
        renderTransformBinding.Source = watermark;
        wm.SetBinding(Watermark.WatermarkRenderTransformProperty, renderTransformBinding);

        Binding textBinding = new Binding("Text");
        textBinding.Source = watermark;
        wm.SetBinding(Watermark.WatermarkTextProperty, textBinding);

        Binding clipBinding = new Binding("Clip");
        clipBinding.Source = watermark;
        wm.SetBinding(Watermark.WatermarkClipProperty, clipBinding);

        Binding effectBinding = new Binding("Effect");
        effectBinding.Source = watermark;
        wm.SetBinding(Watermark.WatermarkEffectProperty, effectBinding);

        Binding flowDirectionBinding = new Binding("FlowDirection");
        flowDirectionBinding.Source = watermark;
        wm.SetBinding(Watermark.WatermarkFlowDirectionProperty, flowDirectionBinding);

        Binding fontFamilyBinding = new Binding("FontFamily");
        fontFamilyBinding.Source = watermark;
        wm.SetBinding(Watermark.WatermarkFontFamilyProperty, fontFamilyBinding);

        Binding fontSizeBinding = new Binding("FontSize");
        fontSizeBinding.Source = watermark;
        wm.SetBinding(Watermark.WatermarkFontSizeProperty, fontSizeBinding);
      }
    }

    #region WatermarkVisibility property

    /// <summary>
    /// Gets or sets the WatermarkVisibility.
    /// This is a dependency property.
    /// </summary>
    public Visibility WatermarkVisibility
    {
      get { return (Visibility)GetValue(WatermarkVisibilityProperty); }
      set { SetValue(WatermarkVisibilityProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="WatermarkVisibility"/> property.
    /// </summary>
    public static readonly DependencyProperty WatermarkVisibilityProperty =
      DependencyProperty.Register("WatermarkVisibility", typeof(Visibility), typeof(Watermark),
      new PropertyMetadata(new PropertyChangedCallback(OnWatermarkVisibilityChanged)));

    private static void OnWatermarkVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Watermark)d).OnWatermarkVisibilityChanged();
    }

    private void OnWatermarkVisibilityChanged()
    {
      if (_watermark.Visibility != Visibility.Visible)
      {
        _watermark.Visibility = Visibility.Visible;
      }
    }

    #endregion // WatermarkVisibility property

    #region WatermarkOpacity property

    /// <summary>
    /// Gets or sets the WatermarkOpacity.
    /// This is a dependency property.
    /// </summary>
    public double WatermarkOpacity
    {
      get { return (double)GetValue(WatermarkOpacityProperty); }
      set { SetValue(WatermarkOpacityProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="WatermarkOpacity"/> property.
    /// </summary>
    public static readonly DependencyProperty WatermarkOpacityProperty =
      DependencyProperty.Register("WatermarkOpacity", typeof(double), typeof(Watermark),
      new PropertyMetadata(new PropertyChangedCallback(OnWatermarkOpacityChanged)));

    private static void OnWatermarkOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Watermark)d).OnWatermarkOpacityChanged();
    }

    private void OnWatermarkOpacityChanged()
    {
      if (_watermark.Opacity != 1)
      {
        _watermark.Opacity = 1;
      }
    }

    #endregion // WatermarkOpacity property

    #region WatermarkMargin property

    /// <summary>
    /// Gets or sets the WatermarkMargin.
    /// This is a dependency property.
    /// </summary>
    public Thickness WatermarkMargin
    {
      get { return (Thickness)GetValue(WatermarkMarginProperty); }
      set { SetValue(WatermarkMarginProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="WatermarkMargin"/> property.
    /// </summary>
    public static readonly DependencyProperty WatermarkMarginProperty =
      DependencyProperty.Register("WatermarkMargin", typeof(Thickness), typeof(Watermark),
      new PropertyMetadata(new PropertyChangedCallback(OnWatermarkMarginChanged)));

    private static void OnWatermarkMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Watermark)d).OnWatermarkMarginChanged();
    }

    private void OnWatermarkMarginChanged()
    {
      if (!(_watermark.Margin.Equals(new Thickness(0))))
      {
        _watermark.Margin = new Thickness(0);
      }
    }

    #endregion // WatermarkMargin property

    #region WatermarkPadding property

    /// <summary>
    /// Gets or sets the WatermarkPadding.
    /// This is a dependency property.
    /// </summary>
    public Thickness WatermarkPadding
    {
      get { return (Thickness)GetValue(WatermarkPaddingProperty); }
      set { SetValue(WatermarkPaddingProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="WatermarkPadding"/> property.
    /// </summary>
    public static readonly DependencyProperty WatermarkPaddingProperty =
      DependencyProperty.Register("WatermarkPadding", typeof(Thickness), typeof(Watermark),
      new PropertyMetadata(new PropertyChangedCallback(OnWatermarkPaddingChanged)));

    private static void OnWatermarkPaddingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Watermark)d).OnWatermarkPaddingChanged();
    }

    private void OnWatermarkPaddingChanged()
    {
      if (!(_watermark.Padding.Equals(new Thickness(0))))
      {
        _watermark.Padding = new Thickness(0);
      }
    }

    #endregion // WatermarkPadding property

    #region WatermarkRenderTransform property

    /// <summary>
    /// Gets or sets the WatermarkRenderTransform.
    /// This is a dependency property.
    /// </summary>
    public Transform WatermarkRenderTransform
    {
      get { return (Transform)GetValue(WatermarkRenderTransformProperty); }
      set { SetValue(WatermarkRenderTransformProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="WatermarkRenderTransform"/> property.
    /// </summary>
    public static readonly DependencyProperty WatermarkRenderTransformProperty =
      DependencyProperty.Register("WatermarkRenderTransform", typeof(Transform), typeof(Watermark),
      new PropertyMetadata(new PropertyChangedCallback(OnWatermarkRenderTransformChanged)));

    private static void OnWatermarkRenderTransformChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Watermark)d).OnWatermarkRenderTransformChanged();
    }

    private void OnWatermarkRenderTransformChanged()
    {
      if (_watermark.RenderTransform != null)
      {
        _watermark.RenderTransform = null;
      }
    }

    #endregion // WatermarkRenderTransform property

    #region WatermarkText property

    /// <summary>
    /// Gets or sets the WatermarkText.
    /// This is a dependency property.
    /// </summary>
    public string WatermarkText
    {
      get { return (string)GetValue(WatermarkTextProperty); }
      set { SetValue(WatermarkTextProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="WatermarkText"/> property.
    /// </summary>
    public static readonly DependencyProperty WatermarkTextProperty =
      DependencyProperty.Register("WatermarkText", typeof(string), typeof(Watermark),
      new PropertyMetadata(new PropertyChangedCallback(OnWatermarkTextChanged)));

    private static void OnWatermarkTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Watermark)d).OnWatermarkTextChanged();
    }

    private void OnWatermarkTextChanged()
    {
      if (!_watermark.Text.Equals(_watermarkText))
      {
        _watermark.Text = _watermarkText;
      }
    }

    #endregion // WatermarkText property

    #region WatermarkClip property

    /// <summary>
    /// Gets or sets the WatermarkClip.
    /// This is a dependency property.
    /// </summary>
    public Geometry WatermarkClip
    {
      get { return (Geometry)GetValue(WatermarkClipProperty); }
      set { SetValue(WatermarkClipProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="WatermarkClip"/> property.
    /// </summary>
    public static readonly DependencyProperty WatermarkClipProperty =
      DependencyProperty.Register("WatermarkClip", typeof(Geometry), typeof(Watermark),
      new PropertyMetadata(new PropertyChangedCallback(OnWatermarkClipChanged)));

    private static void OnWatermarkClipChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Watermark)d).OnWatermarkClipChanged();
    }

    private void OnWatermarkClipChanged()
    {
      _watermark.Clip = null;
    }

    #endregion // WatermarkClip property

    #region WatermarkEffect property

    /// <summary>
    /// Gets or sets the WatermarkEffect.
    /// This is a dependency property.
    /// </summary>
    public Effect WatermarkEffect
    {
      get { return (Effect)GetValue(WatermarkEffectProperty); }
      set { SetValue(WatermarkEffectProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="WatermarkEffect"/> property.
    /// </summary>
    public static readonly DependencyProperty WatermarkEffectProperty =
      DependencyProperty.Register("WatermarkEffect", typeof(Effect), typeof(Watermark),
      new PropertyMetadata(new PropertyChangedCallback(OnWatermarkEffectChanged)));

    private static void OnWatermarkEffectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Watermark)d).OnWatermarkEffectChanged();
    }

    private void OnWatermarkEffectChanged()
    {
      _watermark.Effect = null;
    }

    #endregion // WatermarkEffect property

    #region WatermarkFlowDirection property

    /// <summary>
    /// Gets or sets the WatermarkFlowDirection.
    /// This is a dependency property.
    /// </summary>
    public FlowDirection WatermarkFlowDirection
    {
      get { return (FlowDirection)GetValue(WatermarkFlowDirectionProperty); }
      set { SetValue(WatermarkFlowDirectionProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="WatermarkFlowDirection"/> property.
    /// </summary>
    public static readonly DependencyProperty WatermarkFlowDirectionProperty =
      DependencyProperty.Register("WatermarkFlowDirection", typeof(FlowDirection), typeof(Watermark),
      new PropertyMetadata(new PropertyChangedCallback(OnWatermarkFlowDirectionChanged)));

    private static void OnWatermarkFlowDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Watermark)d).OnWatermarkFlowDirectionChanged();
    }

    private void OnWatermarkFlowDirectionChanged()
    {
      _watermark.FlowDirection = System.Windows.FlowDirection.LeftToRight;
    }

    #endregion // WatermarkFlowDirection property

    #region WatermarkFontFamily property

    /// <summary>
    /// Gets or sets the WatermarkFontFamily.
    /// This is a dependency property.
    /// </summary>
    public FontFamily WatermarkFontFamily
    {
      get { return (FontFamily)GetValue(WatermarkFontFamilyProperty); }
      set { SetValue(WatermarkFontFamilyProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="WatermarkFontFamily"/> property.
    /// </summary>
    public static readonly DependencyProperty WatermarkFontFamilyProperty =
      DependencyProperty.Register("WatermarkFontFamily", typeof(FontFamily), typeof(Watermark),
      new PropertyMetadata(new PropertyChangedCallback(OnWatermarkFontFamilyChanged)));

    private static void OnWatermarkFontFamilyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Watermark)d).OnWatermarkFontFamilyChanged();
    }

    private void OnWatermarkFontFamilyChanged()
    {
      if (!new TextBlock().FontFamily.Source.Equals(_watermark.FontFamily.Source))
      {
        _watermark.FontFamily = new TextBlock().FontFamily;
      }
    }

    #endregion // WatermarkFontFamily property

    #region WatermarkFontSize property

    /// <summary>
    /// Gets or sets the WatermarkFontSize.
    /// This is a dependency property.
    /// </summary>
    public double WatermarkFontSize
    {
      get { return (double)GetValue(WatermarkFontSizeProperty); }
      set { SetValue(WatermarkFontSizeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="WatermarkFontSize"/> property.
    /// </summary>
    public static readonly DependencyProperty WatermarkFontSizeProperty =
      DependencyProperty.Register("WatermarkFontSize", typeof(double), typeof(Watermark),
      new PropertyMetadata(new PropertyChangedCallback(OnWatermarkFontSizeChanged)));

    private static void OnWatermarkFontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Watermark)d).OnWatermarkFontSizeChanged();
    }

    private void OnWatermarkFontSizeChanged()
    {
      _watermark.FontSize = new TextBlock().FontSize;
    }

    #endregion // WatermarkFontSize property
  }
}
