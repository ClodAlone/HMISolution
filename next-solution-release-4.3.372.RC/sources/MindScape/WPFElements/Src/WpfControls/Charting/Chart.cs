using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Markup;
using System.Linq;
using System.Windows.Media.Effects;
using System.Windows.Data;
using Infralution.Licensing;
using System.Windows.Threading;
using System.Reflection;

namespace Mindscape.WpfElements.Charting
{
  /// <summary>
  /// A control for displaying data series on a chart.
  /// </summary>
  [ContentProperty("Series")]
  [TemplatePart(Name = ForegroundCanvasPartName, Type = typeof(Canvas))]
  [TemplatePart(Name = ChartCanvasPartName, Type = typeof(Canvas))]
  [TemplatePart(Name = MouseCanvasPartName, Type = typeof(Canvas))]
  [TemplatePart(Name = ChartAreaPartName, Type = typeof(Border))]
  [TemplatePart(Name = ChartRootPartName, Type = typeof(Border))]
  [LicenseProvider(typeof(PublicEncryptedLicenseProvider))]
  public class Chart : Control
  {
    private const string ForegroundCanvasPartName = "PART_ForegroundCanvas";
    private const string ChartCanvasPartName = "PART_ChartCanvas";
    private const string MouseCanvasPartName = "PART_MouseCanvas";
    private const string ChartAreaPartName = "PART_ChartArea";
    private const string ChartRootPartName = "PART_ChartRoot";
    private const string ForegroundRootPartName = "PART_ForegroundRoot";

    private Canvas _canvas = new Canvas(); // This Canvas instance is useful when working in testing environments.
    private readonly ObservableCollection<DataSeries> _series;
    private readonly ObservableCollection<ChartAxis> _alternateYAxes = new ObservableCollection<ChartAxis>();
    private readonly ObservableCollection<ChartAxis> _alternateXAxes = new ObservableCollection<ChartAxis>();
    private readonly ObservableCollection<UIElement> _foregroundElements = new ObservableCollection<UIElement>();
    private readonly ObservableCollection<UIElement> _backgroundElements = new ObservableCollection<UIElement>();
    private readonly LegendItemCollection _legendItems = new LegendItemCollection();
    private Border _chartArea;
    private Border _chartRoot;
    private Border _foregroundRoot;
    private Grid _axisGrid;

    // Zooming:
    private Canvas _foregroundCanvas;
    private Canvas _mouseCanvas;
    private readonly Rectangle _zoomBorder;
    private bool _isMouseDown;
    private bool _isMouseOver;
    private Point _mouseDownPoint;
    private Point _logicalMouseDown;
    private Point _mouseMovePoint;

    static Chart()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(Chart),
        new FrameworkPropertyMetadata(typeof(Chart)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Chart"/> class.
    /// </summary>
    public Chart()
    {
      // Licensing
      //new WpfElementsCore(Assembly.GetCallingAssembly());
      //LicenseHelper.Attach(this, Assembly.GetCallingAssembly());
      // End licensing

      CommandBindings.Add(new CommandBinding(ChartCommands.ResetZoom, ResetZoom_Executed, ResetZoom_CanExecute));
      InputBindings.Add(new InputBinding(ChartCommands.ResetZoom, new KeyGesture(Key.Home)));

      _zoomBorder = new Rectangle();
      _zoomBorder.Style = ZoomBoxStyle;

      _series = new ObservableCollection<DataSeries>();
      _series.CollectionChanged += new NotifyCollectionChangedEventHandler(Series_CollectionChanged);
      _alternateYAxes.CollectionChanged += new NotifyCollectionChangedEventHandler(AlternateYAxes_CollectionChanged);
      _alternateXAxes.CollectionChanged += new NotifyCollectionChangedEventHandler(AlternateXAxes_CollectionChanged);
      
      Loaded += new RoutedEventHandler(Chart_Loaded);
    }

    private void Chart_Loaded(object sender, RoutedEventArgs e)
    {
      CheckSeriesTitles();

      // TODO: the first 2 conditions here are a kludge for the time explorer.
      if (XAxis == null)
      {
        XAxis = new ChartAxis() { Title = "X Axis" };
      }
      else if (XAxis == _defaultXAxis && BindingOperations.GetBinding(this, XAxisProperty) == null)
      {
        XAxis = _defaultXAxis;
      }
      if (XAxis.IsMinimumAuto && XAxis.IsMaximumAuto)
      {
        XAxis.IsAuto = true;
        foreach (DataSeries series in Series)
        {
          series.OnXAxisChanged();
        }
      }
      // TODO: the first 2 conditions here are a kludge for the time explorer.
      if (YAxis == null)
      {
        YAxis = new ChartAxis() { Title = "Y Axis" };
      }
      else if (YAxis == _defaultYAxis && BindingOperations.GetBinding(this, YAxisProperty) == null)
      {
        YAxis = _defaultYAxis;
      }
      if (YAxis.IsMinimumAuto && YAxis.IsMaximumAuto)
      {
        YAxis.IsAuto = true;
        foreach (DataSeries series in Series)
        {
          series.OnYAxisChanged();
        }
      }
      foreach (ChartAxis axis in AlternativeYAxes)
      {
        if (axis.IsMinimumAuto && axis.IsMaximumAuto)
        {
          axis.IsAuto = true;
        }
      }
      foreach (ChartAxis axis in AlternativeXAxes)
      {
        if (axis.IsMinimumAuto && axis.IsMaximumAuto)
        {
          axis.IsAuto = true;
        }
      }

      /*foreach (DataSeries series in _series)
      {
        series.AnalyseSeries();
      }
      AutoSetAxisRange();

      BuildChart();*/
      AnalyseAndRebuildAllLater();
    }

    /// <summary>
    /// Called by the framework when the control template is applied.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      _foregroundCanvas = GetTemplateChild(ForegroundCanvasPartName) as Canvas;
      _canvas.Children.Clear();
      _canvas = GetTemplateChild(ChartCanvasPartName) as Canvas;
      _mouseCanvas = GetTemplateChild(MouseCanvasPartName) as Canvas;
      _chartArea = GetTemplateChild(ChartAreaPartName) as Border;
      _chartRoot = GetTemplateChild(ChartRootPartName) as Border;
      _foregroundRoot = GetTemplateChild(ForegroundRootPartName) as Border;

      _axisGrid = GetTemplateChild("PART_AxisGrid") as Grid;
      if (_axisGrid != null)
      {
        if (XAxis != null && XAxis.Parent == null)
        {
          XAxis.ChartCanvas = _canvas;
          AddAxisToAxisGrid(XAxis);
          //_axisGrid.Children.Add(XAxis);
        }
        if (YAxis != null && YAxis.Parent == null)
        {
          YAxis.ChartCanvas = _canvas;
          AddAxisToAxisGrid(YAxis);
          //_axisGrid.Children.Add(YAxis);
        }
        foreach (ChartAxis axis in AlternativeYAxes)
        {
          //if (axis.Parent == null) // TODO: probably don't need these parent checks anymore now that we call AddAxisToAxisGrid.
          {
            axis.ChartCanvas = _canvas;
            AddAxisToAxisGrid(axis);
            //_axisGrid.Children.Add(axis);
          }
        }
        foreach (ChartAxis axis in AlternativeXAxes)
        {
          //if (axis.Parent == null)
          {
            axis.ChartCanvas = _canvas;
            AddAxisToAxisGrid(axis);
            //_axisGrid.Children.Add(axis);
          }
        }
        UpdateAxisArrangement();
      }

      if (_chartRoot != null)
      {
        _chartRoot.SizeChanged += new SizeChangedEventHandler(ChartRoot_SizeChanged);
      }
      if (_canvas != null)
      {
        _canvas.SizeChanged += new SizeChangedEventHandler(Canvas_SizeChanged);

        _canvas.MouseLeftButtonDown += new MouseButtonEventHandler(Canvas_MouseLeftButtonDown);
        _canvas.MouseLeftButtonUp += new MouseButtonEventHandler(Canvas_MouseLeftButtonUp);
        _canvas.MouseRightButtonDown += new MouseButtonEventHandler(Canvas_MouseRightButtonDown);
        _canvas.MouseRightButtonUp += new MouseButtonEventHandler(Canvas_MouseRightButtonUp);
        _canvas.MouseMove += new MouseEventHandler(Canvas_MouseMove);
        _canvas.MouseWheel += new MouseWheelEventHandler(Canvas_MouseWheel);
        _canvas.MouseEnter += new MouseEventHandler(Canvas_MouseEnter);
        _canvas.MouseLeave += new MouseEventHandler(Canvas_MouseLeave);
      }
      else
      {
        _canvas = new Canvas();
      }
      if (_mouseCanvas != null)
      {
        _mouseCanvas.IsHitTestVisible = ZoomMode != ZoomMode.None;
      }
      foreach (DataSeries series in _series)
      {
        series.XAxis = GetXAxis(series.XAxisTitle);
        series.YAxis = GetYAxis(series.YAxisTitle);
        series.Canvas = _canvas;
        series.ForegroundCanvas = _foregroundCanvas;
      }
      AutoSetAxisRange();
    }

    private void Canvas_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
      Point position = e.GetPosition(_mouseCanvas);
      double logicalX = XAxis.ConvertPhysicalToLogical(position.X);
      double logicalY = YAxis.ConvertPhysicalToLogical(position.Y);
      OnChartMouseRightButtonUp(new ChartMouseEventArgs(position, new Point(logicalX, logicalY), BuildConstrainedLogicalPoint(logicalX, logicalY), false, _isMouseOver));
    }

    private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
      Point position = e.GetPosition(_mouseCanvas);

      if (CanDeselectOnClickNothing && IsRightClickSelectionEnabled && Mouse.DirectlyOver is Canvas)
      {
        foreach (DataSeries series in Series)
        {
          series.IsSelected = false;
          series.DeselectAll();
        }
      }

      double logicalX = XAxis.ConvertPhysicalToLogical(position.X);
      double logicalY = YAxis.ConvertPhysicalToLogical(position.Y);
      OnChartMouseRightButtonDown(new ChartMouseEventArgs(position, new Point(logicalX, logicalY), BuildConstrainedLogicalPoint(logicalX, logicalY), true, _isMouseOver));
    }

    private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      if (XAxis.Visibility == Visibility.Collapsed || YAxis.Visibility == Visibility.Collapsed)
      {
        BuildChart();
      }
    }

    private void ChartRoot_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      NormalizeSizes();
    }

    private bool _normalizeSizesLocked;

    private void NormalizeSizes()
    {
      if (!_normalizeSizesLocked && _chartArea != null && _chartRoot != null)
      {
        _normalizeSizesLocked = true;
        _chartArea.Width = Math.Floor(_chartRoot.ActualWidth);
        _chartArea.Height = Math.Floor(_chartRoot.ActualHeight);
        if (_foregroundRoot != null)
        {
          double padding = IsChartClipped ? 0 : 5;
          RectangleGeometry clip = new RectangleGeometry(new Rect(-padding, -padding, Math.Floor(_chartRoot.ActualWidth) + (padding * 2), Math.Floor(_chartRoot.ActualHeight) + (padding * 2)));
          _foregroundRoot.Clip = IsChartClipped ? clip : null;
        }
        double chartPadding = 0;
        if (!IsChartClipped)
        {
          chartPadding = 1;
        }
        _chartRoot.Clip = new RectangleGeometry(new Rect(-chartPadding, 0, Math.Floor(_chartRoot.ActualWidth) + (chartPadding *2), Math.Floor(_chartRoot.ActualHeight) + chartPadding));
        XAxis.Width = _chartArea.Width;
        YAxis.Height = _chartArea.Height;
        XAxis.RenderTransform = null;
        YAxis.RenderTransform = null;

        Dictionary<object, IList<ChartAxis>> stacks = new Dictionary<object, IList<ChartAxis>>();
        if (YAxis.StackIdentifier != null)
        {
          IList<ChartAxis> stack;
          stacks.TryGetValue(YAxis.StackIdentifier, out stack);
          if (stack == null)
          {
            stack = new List<ChartAxis>();
            stacks[YAxis.StackIdentifier] = stack;
          }
          stack.Add(YAxis);
        }
        foreach (ChartAxis axis in AlternativeYAxes)
        {
          if (axis.StackIdentifier != null)
          {
            IList<ChartAxis> stack;
            stacks.TryGetValue(axis.StackIdentifier, out stack);
            if (stack == null)
            {
              stack = new List<ChartAxis>();
              stacks[axis.StackIdentifier] = stack;
            }
            stack.Add(axis);
          }
          else
          {
            axis.Height = _chartArea.Height;
          }
          axis.RenderTransform = null;
        }
        foreach (ChartAxis axis in AlternativeXAxes)
        {
          axis.Width = _chartArea.Width;
          axis.RenderTransform = null;
        }

        foreach (object key in stacks.Keys)
        {
          IList<ChartAxis> stack = stacks[key];
          foreach (ChartAxis axis in stack)
          {
            axis.Height = _chartArea.Height / stack.Count;
          }
        }

        if (_firstRightAxis != null)
        {
          if (_firstRightAxis.StackIdentifier != null)
          {
            IList<ChartAxis> stack;
            stacks.TryGetValue(_firstRightAxis.StackIdentifier, out stack);
            foreach (ChartAxis stackedAxis in stack)
            {
              stackedAxis.RenderTransform = new TranslateTransform(-(_chartRoot.ActualWidth - _chartArea.Width), 0);
            }
          }
          else
          {
            _firstRightAxis.RenderTransform = new TranslateTransform(-(_chartRoot.ActualWidth - _chartArea.Width), 0);
          }
        }
        if (_firstTopAxis != null)
        {
          _firstTopAxis.RenderTransform = new TranslateTransform(0, _chartRoot.ActualHeight - _chartArea.Height);
        }
        _normalizeSizesLocked = false;
      }
    }

    #region Interaction

    private void Canvas_MouseLeave(object sender, MouseEventArgs e)
    {
      if (XAxis == null || YAxis == null)
      {
        return;
      }
      _isMouseOver = false;
      Point position = e.GetPosition(_mouseCanvas);
      double logicalX = XAxis.ConvertPhysicalToLogical(position.X);
      double logicalY = YAxis.ConvertPhysicalToLogical(position.Y);
      CheckMouseLeaveHighlights();

      OnChartMouseLeave(new ChartMouseEventArgs(position, new Point(logicalX, logicalY), BuildConstrainedLogicalPoint(logicalX, logicalY), _isMouseDown, _isMouseOver));
    }

    private void CheckMouseLeaveHighlights()
    {
      foreach (DataSeries series in Series)
      {
        series.UnHighlightDataPoints();
      }
    }

    private void Canvas_MouseEnter(object sender, MouseEventArgs e)
    {
      _isMouseOver = true;
      Point position = e.GetPosition(_mouseCanvas);
      double logicalX = XAxis.ConvertPhysicalToLogical(position.X);
      double logicalY = YAxis.ConvertPhysicalToLogical(position.Y);

      OnChartMouseEnter(new ChartMouseEventArgs(position, new Point(logicalX, logicalY), BuildConstrainedLogicalPoint(logicalX, logicalY), _isMouseDown, _isMouseOver));
    }

    private void Canvas_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
    {
      if (ZoomMode != ZoomMode.None && !_isMouseDown)
      {
        Point position = e.GetPosition(_mouseCanvas);
        double leftRatio = position.X / _mouseCanvas.ActualWidth;
        double topRatio = position.Y / _mouseCanvas.ActualHeight;

        double delta = e.Delta > 0 ? 1 : -1;
        double xRange = XAxis.ActualMaximumValue - XAxis.ActualMinimumValue;
        double yRange = YAxis.ActualMaximumValue - YAxis.ActualMinimumValue;
        double zoomRatio = 0.9;
        if (delta < 0)
        {
          zoomRatio = 1.1;
        }
        double newXRange = xRange * zoomRatio;
        double newYRange = yRange * zoomRatio;
        double xRangeDiff = xRange - newXRange;
        double yRangeDiff = yRange - newYRange;

        if (ZoomMode != ZoomMode.Vertical)
        {
          double ratio = XAxis.IsReversed ? 1 - leftRatio : leftRatio;
          XAxis.ActualMinimumValue += xRangeDiff * ratio;
          XAxis.ActualMaximumValue -= xRangeDiff - (xRangeDiff * ratio);

          foreach (ChartAxis alternativeAxis in AlternativeXAxes)
          {
            double alternativeXRange = alternativeAxis.ActualMaximumValue - alternativeAxis.ActualMinimumValue;
            double newAlternativeXRange = alternativeXRange * zoomRatio;
            double alternativeXRangeDiff = alternativeXRange - newAlternativeXRange;
            double alternativeRatio = alternativeAxis.IsReversed ? 1 - leftRatio : leftRatio;
            alternativeAxis.ActualMinimumValue += alternativeXRangeDiff * alternativeRatio;
            alternativeAxis.ActualMaximumValue -= alternativeXRangeDiff - (alternativeXRangeDiff * alternativeRatio);
          }
        }
        if (ZoomMode != ZoomMode.Horizontal)
        {
          double ratio = YAxis.IsReversed ? 1 - topRatio : topRatio;
          YAxis.ActualMinimumValue += yRangeDiff - (yRangeDiff * ratio);
          YAxis.ActualMaximumValue -= yRangeDiff * ratio;

          foreach (ChartAxis alternativeAxis in AlternativeYAxes)
          {
            double alternativeYRange = alternativeAxis.ActualMaximumValue - alternativeAxis.ActualMinimumValue;
            double newAlternativeYRange = alternativeYRange * zoomRatio;
            double alternativeYRangeDiff = alternativeYRange - newAlternativeYRange;
            double alternativeRatio = alternativeAxis.IsReversed ? 1 - topRatio : topRatio;
            alternativeAxis.ActualMinimumValue += alternativeYRangeDiff - (alternativeYRangeDiff * alternativeRatio);
            alternativeAxis.ActualMaximumValue -= alternativeYRangeDiff * alternativeRatio;
          }
        }
        OnChartZoomed();
      }
    }

    // Zooming:
    private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      Canvas canvas = sender as Canvas;
      _isMouseDown = false;
      Point position = e.GetPosition(_mouseCanvas);
      double logicalX = XAxis.ConvertPhysicalToLogical(position.X);
      double logicalY = YAxis.ConvertPhysicalToLogical(position.Y);

      if (MouseMode == ChartMouseMode.Zoom && _zoomBorder.Width > 5 && _zoomBorder.Height > 5 && ZoomMode != ZoomMode.None && _foregroundCanvas.Children.Contains(_zoomBorder))
      {
        double logicalStartX = Math.Min(_logicalMouseDown.X, logicalX);
        double logicalEndX = Math.Max(_logicalMouseDown.X, logicalX);
        double logicalStartY = Math.Min(_logicalMouseDown.Y, logicalY);
        double logicalEndY = Math.Max(_logicalMouseDown.Y, logicalY);

        if (ZoomMode != ZoomMode.Vertical)
        {
          // TODO: these Min Max constraints limit the case where data is dynamically added while the zoom box is being used in a way that causes the zoom box to be larger than the viewport. This is a minor issue though.
          XAxis.ActualMinimumValue = Math.Max(XAxis.ActualMinimumValue, logicalStartX);
          XAxis.ActualMaximumValue = Math.Min(XAxis.ActualMaximumValue, logicalEndX);

          foreach (ChartAxis alternativeAxis in AlternativeXAxes)
          {
            double logicalMouseDownX = alternativeAxis.ConvertPhysicalToLogical(_mouseDownPoint.X);
            double logicalMouseUpX = alternativeAxis.ConvertPhysicalToLogical(position.X);
            alternativeAxis.ActualMinimumValue = Math.Max(alternativeAxis.ActualMinimumValue, Math.Min(logicalMouseDownX, logicalMouseUpX));
            alternativeAxis.ActualMaximumValue = Math.Min(alternativeAxis.ActualMaximumValue, Math.Max(logicalMouseDownX, logicalMouseUpX));
          }
        }
        if (ZoomMode != ZoomMode.Horizontal)
        {
          YAxis.ActualMinimumValue = Math.Max(YAxis.ActualMinimumValue, logicalStartY);
          YAxis.ActualMaximumValue = Math.Min(YAxis.ActualMaximumValue, logicalEndY);

          foreach (ChartAxis alternativeAxis in AlternativeYAxes)
          {
            double logicalMouseDownY = alternativeAxis.ConvertPhysicalToLogical(_mouseDownPoint.Y);
            double logicalMouseUpY = alternativeAxis.ConvertPhysicalToLogical(position.Y);
            alternativeAxis.ActualMinimumValue = Math.Max(alternativeAxis.ActualMinimumValue, Math.Min(logicalMouseDownY, logicalMouseUpY));
            alternativeAxis.ActualMaximumValue = Math.Min(alternativeAxis.ActualMaximumValue, Math.Max(logicalMouseDownY, logicalMouseUpY));
          }
        }
        OnChartZoomed();
      }

      _foregroundCanvas.Children.Remove(_zoomBorder);
      canvas.ReleaseMouseCapture();

      OnChartMouseLeftButtonUp(new ChartMouseEventArgs(position, new Point(logicalX, logicalY), BuildConstrainedLogicalPoint(logicalX, logicalY), false, _isMouseOver));
    }

    // Zooming:
    private void Canvas_MouseMove(object sender, MouseEventArgs e)
    {
      if (XAxis == null || YAxis == null)
      {
        return;
      }
      Point position = e.GetPosition(_mouseCanvas);
      double logicalX = XAxis.ConvertPhysicalToLogical(position.X);
      double logicalY = YAxis.ConvertPhysicalToLogical(position.Y);

      if (_isMouseDown)
      {
        double mouseDownX = XAxis.ConvertLogicalToPhysical(_logicalMouseDown.X);
        double mouseDownY = _canvas.ActualHeight - YAxis.ConvertLogicalToPhysical(_logicalMouseDown.Y);

        double deltaX = position.X - mouseDownX;
        double deltaY = position.Y - mouseDownY;

        if (MouseMode == ChartMouseMode.Zoom && ZoomMode != ZoomMode.None)
        {
          if (ZoomMode != ZoomMode.Vertical)
          {
            Canvas.SetLeft(_zoomBorder, Math.Min(position.X, mouseDownX));
          }
          if (ZoomMode != ZoomMode.Horizontal)
          {
            Canvas.SetTop(_zoomBorder, Math.Min(position.Y, mouseDownY));
          }
          if (deltaX > 0 && ZoomMode != ZoomMode.Vertical)
          {
            _zoomBorder.Width = Math.Max(0, Math.Min(deltaX, _canvas.ActualWidth - mouseDownX));
          }
          else if (ZoomMode != ZoomMode.Vertical)
          {
            _zoomBorder.Width = Math.Max(0, Math.Abs(deltaX));
          }
          if (deltaY > 0 && ZoomMode != ZoomMode.Horizontal)
          {
            _zoomBorder.Height = Math.Max(0, Math.Min(deltaY, _canvas.ActualHeight - mouseDownY));
          }
          else if (ZoomMode != ZoomMode.Horizontal)
          {
            _zoomBorder.Height = Math.Max(0, Math.Abs(deltaY));
          }
        }
        else if (MouseMode == ChartMouseMode.Pan)
        {
          if (_mouseMovePoint == null)
          {
            _mouseMovePoint = _mouseDownPoint;
          }
          double logicalDeltaX = XAxis.ConvertPhysicalToLogicalSize(position.X - _mouseMovePoint.X);
          double logicalDeltaY = YAxis.ConvertPhysicalToLogicalSize(position.Y - _mouseMovePoint.Y);
          if (XAxis.IsReversed)
          {
            logicalDeltaX = -logicalDeltaX;
          }
          if (YAxis.IsReversed)
          {
            logicalDeltaY = -logicalDeltaY;
          }
          XAxis.Pan(-logicalDeltaX);
          YAxis.Pan(logicalDeltaY);

          foreach (ChartAxis alternativeAxis in AlternativeYAxes)
          {
            double alternativeLogicalDeltaY = alternativeAxis.ConvertPhysicalToLogicalSize(position.Y - _mouseMovePoint.Y);
            if (alternativeAxis.IsReversed)
            {
              alternativeLogicalDeltaY = -alternativeLogicalDeltaY;
            }
            alternativeAxis.Pan(alternativeLogicalDeltaY);
          }

          foreach (ChartAxis alternativeAxis in AlternativeXAxes)
          {
            double alternativeLogicalDeltaX = alternativeAxis.ConvertPhysicalToLogicalSize(_mouseMovePoint.X - position.X);
            if (alternativeAxis.IsReversed)
            {
              alternativeLogicalDeltaX = -alternativeLogicalDeltaX;
            }
            alternativeAxis.Pan(alternativeLogicalDeltaX);
          }
        }
      }
      else
      {
        CheckMouseMoveHighlights(position);
      }
      _mouseMovePoint = position;

      OnChartMouseMove(new ChartMouseEventArgs(position, new Point(logicalX, logicalY), BuildConstrainedLogicalPoint(logicalX, logicalY), _isMouseDown, _isMouseOver));
    }

    private void CheckMouseMoveHighlights(Point mousePosition)
    {
      double logicalX = XAxis.ConvertPhysicalToLogical(mousePosition.X);
      foreach (DataSeries series in Series)
      {
        series.HighlightClosestDataPoint(logicalX);
      }
    }

    // Zooming:
    private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      Canvas canvas = sender as Canvas;
      _isMouseDown = true;
      Point position = e.GetPosition(_mouseCanvas);
      double logicalX = XAxis.ConvertPhysicalToLogical(position.X);
      double logicalY = YAxis.ConvertPhysicalToLogical(position.Y);
      _mouseDownPoint = position;
      _logicalMouseDown = new Point(logicalX, logicalY);
      _mouseMovePoint = position;

      MouseMode = KeyboardUtils.IsHoldingCtrl ? ChartMouseMode.Pan : ChartMouseMode.Zoom;

      if (MouseMode == ChartMouseMode.Zoom && ZoomMode != ZoomMode.None)
      {
        Canvas.SetLeft(_zoomBorder, ZoomMode == ZoomMode.Vertical ? 0 : position.X);
        Canvas.SetTop(_zoomBorder, ZoomMode == ZoomMode.Horizontal ? 0 : position.Y);
        _zoomBorder.Width = ZoomMode == ZoomMode.Vertical ? _mouseCanvas.ActualWidth : 0;
        _zoomBorder.Height = ZoomMode == ZoomMode.Horizontal ? _mouseCanvas.ActualHeight : 0;
        if (_zoomBorder.Parent == null)
        {
          _foregroundCanvas.Children.Add(_zoomBorder);
        }
      }

      if (CanDeselectOnClickNothing && Mouse.DirectlyOver is Canvas)
      {
        foreach (DataSeries series in Series)
        {
          series.IsSelected = false;
          series.DeselectAll();
        }
      }

      canvas.CaptureMouse();
      Focus();

      OnChartMouseLeftButtonDown(new ChartMouseEventArgs(position, new Point(logicalX, logicalY), BuildConstrainedLogicalPoint(logicalX, logicalY), true, _isMouseOver));
    }

    private Point BuildConstrainedLogicalPoint(double logicalX, double logicalY)
    {
      return new Point(Math.Max(XAxis.ActualMinimumValue, Math.Min(XAxis.ActualMaximumValue, logicalX)), Math.Max(YAxis.ActualMinimumValue, Math.Min(YAxis.ActualMaximumValue, logicalY)));
    }

    /// <summary>
    /// Called when a key is pressed while the <see cref="Chart"/> has focus.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnKeyDown(KeyEventArgs e)
    {
      base.OnKeyDown(e);

      if (e.Key == Key.Escape && MouseMode == ChartMouseMode.Zoom && _isMouseDown)
      {
        MouseMode = ChartMouseMode.None;
        _foregroundCanvas.Children.Remove(_zoomBorder);
        OnZoomCanceled();
      }
    }

    #endregion // Interaction

    #region Commands

    private void ResetZoom_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      XAxis.ActualMinimumValue = XAxis.MinimumValue;
      XAxis.ActualMaximumValue = XAxis.MaximumValue;
      YAxis.ActualMinimumValue = YAxis.MinimumValue;
      YAxis.ActualMaximumValue = YAxis.MaximumValue;
      foreach (ChartAxis axis in AlternativeYAxes)
      {
        axis.ActualMinimumValue = axis.MinimumValue;
        axis.ActualMaximumValue = axis.MaximumValue;
      }
      foreach (ChartAxis axis in AlternativeXAxes)
      {
        axis.ActualMinimumValue = axis.MinimumValue;
        axis.ActualMaximumValue = axis.MaximumValue;
      }
    }

    private void ResetZoom_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      bool canExecute = false;
      canExecute = canExecute || XAxis.ActualMinimumValue != XAxis.MinimumValue;
      canExecute = canExecute || XAxis.ActualMaximumValue != XAxis.MaximumValue;
      canExecute = canExecute || YAxis.ActualMinimumValue != YAxis.MinimumValue;
      canExecute = canExecute || YAxis.ActualMaximumValue != YAxis.MaximumValue;
      foreach (ChartAxis axis in AlternativeYAxes)
      {
        canExecute = canExecute || axis.ActualMinimumValue != axis.MinimumValue;
        canExecute = canExecute || axis.ActualMaximumValue != axis.MaximumValue;
      }
      foreach (ChartAxis axis in AlternativeXAxes)
      {
        canExecute = canExecute || axis.ActualMinimumValue != axis.MinimumValue;
        canExecute = canExecute || axis.ActualMaximumValue != axis.MaximumValue;
      }
      e.CanExecute = canExecute;
    }

    #endregion // Commands

    #region Events

    /// <summary>
    /// Raised when a zoom operation is canceled (the user presses the Esc key while dragging a zoom marquee).
    /// </summary>
    public event EventHandler ZoomCanceled;

    private void OnZoomCanceled()
    {
      EventHandler handler = ZoomCanceled;
      if (handler != null)
      {
        handler(this, new EventArgs());
      }
    }

    /// <summary>
    /// Raised when the mouse is moved over the chart area.
    /// </summary>
    public event EventHandler<ChartMouseEventArgs> ChartMouseMove;

    /// <summary>
    /// Raised when the mouse enters the chart area.
    /// </summary>
    public event EventHandler<ChartMouseEventArgs> ChartMouseEnter;

    /// <summary>
    /// Raised when the mouse leaves the chart area.
    /// </summary>
    public event EventHandler<ChartMouseEventArgs> ChartMouseLeave;

    /// <summary>
    /// Raised when the left mouse button is released over the the chart area.
    /// </summary>
    public event EventHandler<ChartMouseEventArgs> ChartMouseLeftButtonUp;

    /// <summary>
    /// Raised when the left mouse button is pressed over the chart area.
    /// </summary>
    public event EventHandler<ChartMouseEventArgs> ChartMouseLeftButtonDown;

    /// <summary>
    /// Raised when the right mouse button is released over the the chart area.
    /// </summary>
    public event EventHandler<ChartMouseEventArgs> ChartMouseRightButtonUp;

    /// <summary>
    /// Raised when the right mouse button is pressed over the chart area.
    /// </summary>
    public event EventHandler<ChartMouseEventArgs> ChartMouseRightButtonDown;

    private void OnChartMouseRightButtonDown(ChartMouseEventArgs args)
    {
      EventHandler<ChartMouseEventArgs> handler = ChartMouseRightButtonDown;
      if (handler != null)
      {
        handler(this, args);
      }
    }

    private void OnChartMouseRightButtonUp(ChartMouseEventArgs args)
    {
      EventHandler<ChartMouseEventArgs> handler = ChartMouseRightButtonUp;
      if (handler != null)
      {
        handler(this, args);
      }
    }

    private void OnChartMouseLeftButtonDown(ChartMouseEventArgs args)
    {
      EventHandler<ChartMouseEventArgs> handler = ChartMouseLeftButtonDown;
      if (handler != null)
      {
        handler(this, args);
      }
    }

    private void OnChartMouseLeftButtonUp(ChartMouseEventArgs args)
    {
      EventHandler<ChartMouseEventArgs> handler = ChartMouseLeftButtonUp;
      if (handler != null)
      {
        handler(this, args);
      }
    }

    private void OnChartMouseEnter(ChartMouseEventArgs args)
    {
      EventHandler<ChartMouseEventArgs> handler = ChartMouseEnter;
      if (handler != null)
      {
        handler(this, args);
      }
    }

    private void OnChartMouseLeave(ChartMouseEventArgs args)
    {
      EventHandler<ChartMouseEventArgs> handler = ChartMouseLeave;
      if (handler != null)
      {
        handler(this, args);
      }
    }

    private void OnChartMouseMove(ChartMouseEventArgs args)
    {
      EventHandler<ChartMouseEventArgs> handler = ChartMouseMove;
      if (handler != null)
      {
        handler(this, args);
      }
    }

    /// <summary>
    /// Raised whenever the user either uses the mouse wheel or the zoom box to change the zoom level of the chart.
    /// </summary>
    public event EventHandler<ChartZoomEventArgs> ChartZoomed;

    private void OnChartZoomed()
    {
      double xZoom = (XAxis.ActualMaximumValue - XAxis.ActualMinimumValue) / (XAxis.MaximumValue - XAxis.MinimumValue);
      double yZoom = (YAxis.ActualMaximumValue - YAxis.ActualMinimumValue) / (YAxis.MaximumValue - YAxis.MinimumValue);
      OnChartZoomed(new ChartZoomEventArgs(xZoom, yZoom));
    }

    private void OnChartZoomed(ChartZoomEventArgs args)
    {
      EventHandler<ChartZoomEventArgs> handler = ChartZoomed;
      if (handler != null)
      {
        handler(this, args);
      }
    }

    #endregion // Events

    #region Series Management

    private void Series_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.OldItems != null)
      {
        foreach (DataSeries series in e.OldItems.OfType<DataSeries>())
        {
          series.LegendItemsChanged -= new EventHandler(DataSeries_LegendItemsChanged);
          series.SelectedDataPointChanged -= new EventHandler<SelectedDataPointChangedEventArgs>(DataSeries_SelectedDataPointChanged);
          series.YAxisTitleChanged -= new EventHandler(DataSeries_YAxisTitleChanged);
          series.XAxisTitleChanged -= new EventHandler(DataSeries_XAxisTitleChanged);
          series.AnalyseAndRebuildAll -= new EventHandler(DataSeries_AnalyseAndRebuildAll);
          series.UpdateAxisRanges -= new EventHandler(DataSeries_UpdateAxisRanges);
          series.IsSelectedChanged -= new EventHandler<EventArgs>(series_IsSelectedChanged);
          series.Chart = null;
        }
      }
      if (e.NewItems != null)
      {
        foreach (DataSeries series in e.NewItems.OfType<DataSeries>())
        {
          series.Series = Series;
          series.XAxis = GetXAxis(series.XAxisTitle);
          series.YAxis = GetYAxis(series.YAxisTitle);
          series.Canvas = _canvas;
          series.ForegroundCanvas = _foregroundCanvas;
          series.Chart = this;

          series.LegendItemsChanged += new EventHandler(DataSeries_LegendItemsChanged);
          series.SelectedDataPointChanged += new EventHandler<SelectedDataPointChangedEventArgs>(DataSeries_SelectedDataPointChanged);
          series.YAxisTitleChanged += new EventHandler(DataSeries_YAxisTitleChanged);
          series.XAxisTitleChanged += new EventHandler(DataSeries_XAxisTitleChanged);
          series.AnalyseAndRebuildAll += new EventHandler(DataSeries_AnalyseAndRebuildAll);
          series.Rebuild += new EventHandler(DataSeries_Rebuild);
          series.UpdateAxisRanges += new EventHandler(DataSeries_UpdateAxisRanges);
          series.IsSelectedChanged += new EventHandler<EventArgs>(series_IsSelectedChanged);

          if (series.Title == null && series.GetBindingExpression(DataSeries.TitleProperty) == null && IsLoaded)
          {
            int index = 0;
            if (Series[e.NewStartingIndex] == series)
            {
              index = e.NewStartingIndex;
            }
            else
            {
              index = Series.IndexOf(series);
            }
            series.Title = "Series " + (index + 1);
          }
        }
      }
      AnalyseAndRebuildAllLater();
      //_legendItems.Reload(this);
      _isLegendDirty = true;
    }

    // This is called when the chart is loaded. When a chart is setup in a DataTemplate, none of the properties are set on the series at the time the Series_CollectionChanged method is called.
    private void CheckSeriesTitles()
    {
      int index = 1;
      foreach (DataSeries series in Series)
      {
        if (series.Title == null && series.GetBindingExpression(DataSeries.TitleProperty) == null)
        {
          series.Title = "Series " + index;
        }
        index++;
      }
    }

    private bool _isLegendDirty = false;

    private void UpdateLegend()
    {
      if (_isLegendDirty)
      {
        _legendItems.Reload(this);
        _isLegendDirty = false;
      }
    }

    private DataSeries _previousSelectedDataSeries;
    private bool _seriesSelectionLock;

    private void series_IsSelectedChanged(object sender, EventArgs e)
    {
      if (!_seriesSelectionLock)
      {
        DataSeries series = sender as DataSeries;
        if (series != _selectedDataSeries && _selectedDataSeries != null)
        {
          _seriesSelectionLock = true;
          _selectedDataSeries.IsSelected = false;
          _seriesSelectionLock = false;
        }

        if (series.IsSelected)
        {
          _selectedDataSeries = series;
        }
        else
        {
          _selectedDataSeries = null;
        }
        if (_selectedDataSeries != null && series.IsSelected)
        {
          OnSelectedDataSeriesChanged(new SelectedDataSeriesChangeddEventArgs(_selectedDataSeries, _previousSelectedDataSeries));
        }
        else
        {
          OnSelectedDataSeriesChanged(new SelectedDataSeriesChangeddEventArgs(null, _previousSelectedDataSeries));
        }
        _previousSelectedDataSeries = series;
      }
    }

    private void DataSeries_Rebuild(object sender, EventArgs e)
    {
      BuildChartLater();
    }

    private void DataSeries_UpdateAxisRanges(object sender, EventArgs e)
    {
      AutoSetAxisRange();
    }

    private void DataSeries_YAxisTitleChanged(object sender, EventArgs e)
    {
      DataSeries series = sender as DataSeries;
      series.YAxis = GetYAxis(series.YAxisTitle);
      AnalyseAndRebuildAllLater();
    }

    private void DataSeries_XAxisTitleChanged(object sender, EventArgs e)
    {
      DataSeries series = sender as DataSeries;
      series.XAxis = GetXAxis(series.XAxisTitle);
      AnalyseAndRebuildAllLater();
    }

    private DataSeries _selectedDataSeries;

    private void DataSeries_SelectedDataPointChanged(object sender, SelectedDataPointChangedEventArgs e)
    {
      DataSeries previousSeries = _selectedDataSeries;
      DataSeries series = sender as DataSeries;
      if (_selectedDataSeries != null && series != _selectedDataSeries && SelectionMode == DataSeriesSelectionMode.Single)
      {
        if (series.SelectedDataPoint != null && series.SelectedDataPoint.IsSelected)
        {
          _selectedDataSeries.IsSelected = false;
          if (_selectedDataSeries != null)
          {
            _selectedDataSeries.DeselectAll();
          }
        }
      }
      if (_selectedDataSeries == series && series.SelectedDataPoint == null)
      {
        _selectedDataSeries = null;
      }
      else if (series.SelectedDataPoint != null && series.SelectedDataPoint.IsSelected)
      {
        _selectedDataSeries = series;
      }

      if (previousSeries != _selectedDataSeries)
      {
        OnSelectedDataSeriesChanged(new SelectedDataSeriesChangeddEventArgs(_selectedDataSeries, previousSeries));
      }
      _previousSelectedDataSeries = _selectedDataSeries;
    }

    private void DataSeries_AnalyseAndRebuildAll(object sender, EventArgs e)
    {
      AnalyseAndRebuildAllLater();
    }

    private void DataSeries_LegendItemsChanged(object sender, EventArgs e)
    {
      _legendItems.Reload(this);
    }

    #endregion // Series Management

    #region Axis Management

    private void RemoveAxisFromAxisGrid(ChartAxis axis)
    {
      if (_axisGrid != null)
      {
        _axisGrid.Children.Remove(axis);
        UpdateAxisArrangement();
      }
    }

    private void AddAxisToAxisGrid(ChartAxis axis)
    {
      if (_axisGrid != null && axis.Parent == null && axis.StackIdentifier == null)
      {
        if (IsChartClipped || _axisGrid.Children.Count <= 1)
        {
          _axisGrid.Children.Add(axis);
        }
        else
        {
          _axisGrid.Children.Insert(0, axis);
        }
        UpdateAxisArrangement();
      }
    }

    private ChartAxis _firstRightAxis;
    private ChartAxis _firstTopAxis;

    private void UpdateAxisArrangement()
    {
      NormalizeSizes();
      if (_axisGrid != null)
      {
        _firstRightAxis = null;
        _firstTopAxis = null;
        IList<ChartAxis> leftAxes = new List<ChartAxis>();
        IList<ChartAxis> bottomAxes = new List<ChartAxis>();
        IList<ChartAxis> rightAxes = new List<ChartAxis>();
        IList<ChartAxis> topAxes = new List<ChartAxis>();
        IList<ChartAxis> horizontalOverlays = new List<ChartAxis>();

        Dictionary<object, IList<ChartAxis>> stacks = new Dictionary<object, IList<ChartAxis>>();

        // Sort each axis to the appropriate side of the chart canvas:
        if (XAxis != null)
        {
          if (XAxis.Placement == AxisPlacement.Auto || XAxis.Placement == AxisPlacement.Left || XAxis.Placement == AxisPlacement.Right)
          {
            XAxis.Placement = AxisPlacement.Bottom; // TODO: should probably set a flag to prevent this property setter calling this method again.
          }
          if (XAxis.Placement == AxisPlacement.Top)
          {
            topAxes.Add(XAxis);
          }
          else
          {
            bottomAxes.Add(XAxis);
          }
        }
        if (YAxis != null)
        {
          bool skip = false;
          if (YAxis.StackIdentifier != null)
          {
            IList<ChartAxis> stack;
            stacks.TryGetValue(YAxis.StackIdentifier, out stack);
            if (stack == null)
            {
              stack = new List<ChartAxis>();
              stacks[YAxis.StackIdentifier] = stack;
            }
            else
            {
              skip = true;
            }
            YAxis.StackIndex = stack.Count;
            stack.Add(YAxis);
          }
          if (YAxis.Placement == AxisPlacement.Auto || YAxis.Placement == AxisPlacement.Top || YAxis.Placement == AxisPlacement.Bottom)
          {
            YAxis.Placement = AxisPlacement.Left; // TODO: should probably set a flag to prevent this property setter calling this method again.
          }
          if (!skip)
          {
            if (YAxis.Placement == AxisPlacement.Overlay)
            {
              horizontalOverlays.Add(YAxis);
            }
            else if (YAxis.Placement == AxisPlacement.Right)
            {
              rightAxes.Add(YAxis);
            }
            else
            {
              leftAxes.Add(YAxis);
            }
          }
        }
        foreach (ChartAxis axis in AlternativeYAxes)
        {
          bool skip = false;
          if (axis.StackIdentifier != null)
          {
            IList<ChartAxis> stack;
            stacks.TryGetValue(axis.StackIdentifier, out stack);
            if (stack == null)
            {
              stack = new List<ChartAxis>();
              stacks[axis.StackIdentifier] = stack;
            }
            else
            {
              skip = true;
            }
            axis.StackIndex = stack.Count;
            stack.Add(axis);
          }
          if (axis.Placement == AxisPlacement.Auto || axis.Placement == AxisPlacement.Top || axis.Placement == AxisPlacement.Bottom)
          {
            axis.Placement = AxisPlacement.Right; // TODO: should probably set a flag to prevent this property setter calling this method again.
          }
          if (!skip)
          {
            if (axis.Placement == AxisPlacement.Overlay)
            {
              horizontalOverlays.Add(axis);
            }
            else if (axis.Placement == AxisPlacement.Left)
            {
              leftAxes.Add(axis);
            }
            else
            {
              rightAxes.Add(axis);
            }
          }
        }
        foreach (ChartAxis axis in AlternativeXAxes)
        {
          if (axis.Placement == AxisPlacement.Auto || axis.Placement == AxisPlacement.Left || axis.Placement == AxisPlacement.Right)
          {
            axis.Placement = AxisPlacement.Top; // TODO: should probably set a flag to prevent this property setter calling this method again.
          }
          if (axis.Placement == AxisPlacement.Bottom)
          {
            bottomAxes.Add(axis);
          }
          else
          {
            topAxes.Add(axis);
          }
        }
        // Clear grid definitions:
        _axisGrid.ColumnDefinitions.Clear();
        _axisGrid.RowDefinitions.Clear();
        // Left axes:
        for (int i = leftAxes.Count - 1; i >= 0; i--)
        {
          _axisGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
          ChartAxis axis = leftAxes[i];

          if (axis.StackIdentifier != null)
          {
            IList<ChartAxis> stack;
            stacks.TryGetValue(axis.StackIdentifier, out stack);
            if (stack != null)
            {
              Grid grid = new Grid();
              int count = 0;
              foreach (ChartAxis stackedAxis in stack)
              {
                Grid parentGrid = stackedAxis.Parent as Grid;
                if (parentGrid != null)
                {
                  parentGrid.Children.Remove(stackedAxis);
                }
                grid.Children.Add(stackedAxis);
                Grid.SetRow(stackedAxis, stack.Count - 1 - count);
                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                count++;
              }
              _axisGrid.Children.Add(grid); // TODO: this grid is currently never removed. This method is not called often, so not a huge concern right now.
              Grid.SetColumn(grid, leftAxes.Count - 1 - i);
              Grid.SetRow(grid, topAxes.Count);
              grid.VerticalAlignment = VerticalAlignment.Bottom;
              grid.HorizontalAlignment = HorizontalAlignment.Right;
              grid.Margin = new Thickness(0, 0, -1, 0);
            }
          }
          else
          {
            Grid.SetColumn(axis, leftAxes.Count - 1 - i);
            Grid.SetRow(axis, topAxes.Count);
            axis.VerticalAlignment = VerticalAlignment.Bottom;
            axis.HorizontalAlignment = HorizontalAlignment.Right;
            axis.Margin = new Thickness(0, 0, -1, 0);
          }
        }
        // Chart canvas column slot and overlay axis slots:
        for (int i = 0; i < horizontalOverlays.Count + 1; i++)
        {
          _axisGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
        }
        // Right axes:
        for (int i = 0; i < rightAxes.Count; i++)
        {
          _axisGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
          ChartAxis axis = rightAxes[i];
          if (i == 0)
          {
            _firstRightAxis = axis;
          }

          if (axis.StackIdentifier != null)
          {
            IList<ChartAxis> stack;
            stacks.TryGetValue(axis.StackIdentifier, out stack);
            if (stack != null)
            {
              Grid grid = new Grid();
              int count = 0;
              foreach (ChartAxis stackedAxis in stack)
              {
                Grid parentGrid = stackedAxis.Parent as Grid;
                if (parentGrid != null)
                {
                  parentGrid.Children.Remove(stackedAxis);
                }
                grid.Children.Add(stackedAxis);
                Grid.SetRow(stackedAxis, stack.Count - 1 - count);
                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                count++;
              }
              _axisGrid.Children.Add(grid); // TODO: this grid is currently never removed. This method is not called often, so not a huge concern right now.
              Grid.SetColumn(grid, i + 1 + leftAxes.Count + horizontalOverlays.Count);
              Grid.SetRow(grid, topAxes.Count);
              grid.VerticalAlignment = VerticalAlignment.Bottom;
              grid.HorizontalAlignment = HorizontalAlignment.Left;
              grid.Margin = new Thickness(-1, 0, 0, 0);
            }
          }
          else
          {
            Grid.SetColumn(axis, i + 1 + leftAxes.Count + horizontalOverlays.Count);
            Grid.SetRow(axis, topAxes.Count);
            axis.VerticalAlignment = VerticalAlignment.Bottom;
            axis.HorizontalAlignment = HorizontalAlignment.Left;
            axis.Margin = new Thickness(-1, 0, 0, 0);
          }
        }

        // Top axes:
        for (int i = 0; i < topAxes.Count; i++)
        {
          _axisGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
          ChartAxis axis = topAxes[i];
          if (i == 0)
          {
            _firstTopAxis = axis;
          }
          Grid.SetRow(axis, i);
          Grid.SetColumn(axis, leftAxes.Count);
          Grid.SetColumnSpan(axis, horizontalOverlays.Count + 1);
          axis.HorizontalAlignment = HorizontalAlignment.Left;
          axis.VerticalAlignment = VerticalAlignment.Bottom;
          axis.Margin = new Thickness(0, 0, 0, -1);
        }
        // Chart canvas row slot:
        _axisGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
        // Bottom axes:
        for (int i = 0; i < bottomAxes.Count; i++)
        {
          _axisGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
          ChartAxis axis = bottomAxes[i];
          Grid.SetRow(axis, i + 1 + topAxes.Count);
          Grid.SetColumn(axis, leftAxes.Count);
          Grid.SetColumnSpan(axis, horizontalOverlays.Count + 1);
          axis.HorizontalAlignment = HorizontalAlignment.Left;
          axis.VerticalAlignment = VerticalAlignment.Top;
          axis.Margin = new Thickness(0, -1, 0, 0);
        }
        // Overlay axes
        for (int i = 0; i < horizontalOverlays.Count; i++)
        {
          ChartAxis axis = horizontalOverlays[i];
          Grid.SetRow(axis, topAxes.Count);
          Grid.SetColumn(axis, leftAxes.Count + i);
          axis.HorizontalAlignment = HorizontalAlignment.Right;
          axis.VerticalAlignment = VerticalAlignment.Bottom;
          axis.IsHitTestVisible = false;
        }

        // Position the chart canvas:
        if (_chartRoot != null)
        {
          Grid.SetRow(_chartRoot, topAxes.Count);
          Grid.SetColumn(_chartRoot, leftAxes.Count);
          Grid.SetColumnSpan(_chartRoot, horizontalOverlays.Count + 1);
        }
        if (_foregroundRoot != null)
        {
          Grid.SetRow(_foregroundRoot, topAxes.Count);
          Grid.SetColumn(_foregroundRoot, leftAxes.Count);
          Grid.SetColumnSpan(_foregroundRoot, horizontalOverlays.Count + 1);
        }
      }
    }

    // This method calculates the minimum and maximum values of all the used axes and overrides their current values.
    // TODO: Test. In particular, make sure this method is called in the appropriate situations.
    private void AutoSetAxisRange()
    {
      Dictionary<ChartAxis, Point> valueMap = new Dictionary<ChartAxis, Point>();
      foreach (DataSeries series in Series)
      {
        if (series.ItemsSource != null && series.ItemsSource.Count > 0 && series.Visibility != Visibility.Collapsed)
        {
          ChartAxis dependentAxis = series.ReverseAxes ? series.XAxis : series.YAxis;
          ChartAxis independentAxis = series.ReverseAxes ? series.YAxis : series.XAxis;
          double dependentMinimum = series.DependentMinimum;
          double dependentMaximum = series.DependentMaximum;
          if (independentAxis != null)
          {
            Point minMax = GetMinMax(independentAxis, valueMap);

            if (independentAxis.IsMinimumAuto)
            {
              // TODO: DependentDataBuffer property?
              minMax.X = Math.Min(minMax.X, Math.Floor(series.IndependentMinimum));
            }
            if (independentAxis.IsMaximumAuto)
            {
              minMax.Y = Math.Max(minMax.Y, Math.Ceiling(series.IndependentMaximum));
            }
            valueMap[independentAxis] = minMax;
          }
          if (dependentAxis != null)
          {
            Point minMax = GetMinMax(dependentAxis, valueMap);

            if (dependentAxis.IsMinimumAuto)
            {
              if (series.YAxisDataBuffer >= 1)
              {
                dependentMinimum = Math.Floor(dependentMinimum);
              }
              if (series.AlwaysShowYAxisZero)
              {
                dependentMinimum = Math.Min(0, dependentMinimum);
                if (dependentMinimum < 0)
                {
                  dependentMinimum -= series.YAxisDataBuffer;
                }
              }
              else
              {
                dependentMinimum -= series.YAxisDataBuffer;
              }
              minMax.X = Math.Min(minMax.X, dependentMinimum);
            }
            if (dependentAxis.IsMaximumAuto)
            {
              minMax.Y = Math.Max(minMax.Y, (series.YAxisDataBuffer >= 1 ? Math.Round(dependentMaximum) : dependentMaximum) + series.YAxisDataBuffer);
            }
            valueMap[dependentAxis] = minMax;
          }
        }
      }

      foreach (ChartAxis axis in valueMap.Keys)
      {
        Point minMax = valueMap[axis];

        if (axis.IsMaximumAuto)
        {
          axis.SetMaximumInternal(minMax.Y);
        }
        else if (IsLoaded)
        {
          axis.UpdateMaximumValueFromMaximum();
        }

        if (axis.IsMinimumAuto)
        {
          axis.SetMinimumInternal(minMax.X);
        }
        else if (IsLoaded)
        {
          axis.UpdateMinimumValueFromMinimum();
        }
      }
    }

    private Point GetMinMax(ChartAxis axis, Dictionary<ChartAxis, Point> valueMap)
    {
      if (!valueMap.ContainsKey(axis))
      {
        return new Point(Double.MaxValue, Double.MinValue);
      }
      return valueMap[axis];
    }

    private readonly IList<ChartAxis> _alternateYAxisCache = new List<ChartAxis>();

    private void AlternateYAxes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.OldItems != null)
      {
        foreach (ChartAxis axis in e.OldItems)
        {
          _alternateYAxisCache.Remove(axis);
          axis.TitleChanged -= new EventHandler(YAxis_TitleChanged);
          DestroyAxis(axis, true);
        }
      }
      if (e.NewItems != null)
      {
        foreach (ChartAxis axis in e.NewItems)
        {
          _alternateYAxisCache.Add(axis);
          axis.Orientation = Orientation.Vertical;
          axis.TitleChanged += new EventHandler(YAxis_TitleChanged);
          PrepareAxis(axis);
        }
      }
      if (e.Action == NotifyCollectionChangedAction.Reset)
      {
        if (_alternateYAxisCache != null)
        {
          foreach (ChartAxis axis in _alternateYAxisCache)
          {
            axis.TitleChanged -= new EventHandler(YAxis_TitleChanged);
            DestroyAxis(axis, true);
          }
          _alternateYAxisCache.Clear();
        }
      }
      foreach (DataSeries series in Series)
      {
        series.YAxis = GetYAxis(series.YAxisTitle);
      }
      AnalyseAndRebuildAllLater();
    }

    private readonly IList<ChartAxis> _alternateXAxisCache = new List<ChartAxis>();

    private void AlternateXAxes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.OldItems != null)
      {
        foreach (ChartAxis axis in e.OldItems)
        {
          _alternateXAxisCache.Remove(axis);
          DestroyAxis(axis, true);
        }
      }
      if (e.NewItems != null)
      {
        foreach (ChartAxis axis in e.NewItems)
        {
          _alternateXAxisCache.Add(axis);
          axis.Orientation = Orientation.Horizontal;
          PrepareAxis(axis);
        }
      }
      if (e.Action == NotifyCollectionChangedAction.Reset)
      {
        if (_alternateXAxisCache != null)
        {
          foreach (ChartAxis axis in _alternateXAxisCache)
          {
            DestroyAxis(axis, true);
          }
          _alternateXAxisCache.Clear();
        }
      }
      foreach (DataSeries series in Series)
      {
        series.XAxis = GetXAxis(series.XAxisTitle);
      }
      AnalyseAndRebuildAllLater();
    }

    private ChartAxis GetYAxis(string name)
    {
      if (String.IsNullOrEmpty(name))
      {
        return YAxis;
      }
      foreach (ChartAxis axis in _alternateYAxes)
      {
        if (name.Equals(axis.Title))
        {
          return axis;
        }
      }
      return YAxis;
    }

    private ChartAxis GetXAxis(string name)
    {
      if (String.IsNullOrEmpty(name))
      {
        return XAxis;
      }
      foreach (ChartAxis axis in _alternateXAxes)
      {
        if (name.Equals(axis.Title))
        {
          return axis;
        }
      }
      return XAxis;
    }

    private void DestroyAxis(ChartAxis axis, bool updateArrangement)
    {
      axis.ValueConverterChanged -= new EventHandler(Axis_ValueConverterChanged);
      axis.AxisUpdated -= new EventHandler(Axis_AxisUpdated);
      axis.IsMinimumAutoChanged -= new EventHandler(Axis_IsMinimumAutoChanged);
      axis.IsMaximumAutoChanged -= new EventHandler(Axis_IsMaximumAutoChanged);
      axis.PlacementChanged -= new EventHandler(Axis_PlacementChanged);
      if (updateArrangement)
      {
        RemoveAxisFromAxisGrid(axis);
      }
      else if (_axisGrid != null)
      {
        _axisGrid.Children.Remove(axis);
      }
    }

    private void PrepareAxis(ChartAxis axis)
    {
      if (Double.NaN.Equals(axis.Minimum) || Double.NaN.Equals(axis.Maximum) && IsLoaded)
      {
        axis.IsAuto = true;
      }
      axis.ChartCanvas = _canvas;
      axis.ValueConverterChanged += new EventHandler(Axis_ValueConverterChanged);
      axis.AxisUpdated += new EventHandler(Axis_AxisUpdated);
      axis.IsMinimumAutoChanged += new EventHandler(Axis_IsMinimumAutoChanged);
      axis.IsMaximumAutoChanged += new EventHandler(Axis_IsMaximumAutoChanged);
      axis.PlacementChanged += new EventHandler(Axis_PlacementChanged);
      AddAxisToAxisGrid(axis);
    }

    private void Axis_IsMaximumAutoChanged(object sender, EventArgs e)
    {
      ChartAxis axis = sender as ChartAxis;
      if (axis.IsMaximumAuto)
      {
        AutoSetAxisRange();
      }
    }

    private void Axis_IsMinimumAutoChanged(object sender, EventArgs e)
    {
      ChartAxis axis = sender as ChartAxis;
      if (axis.IsMinimumAuto)
      {
        AutoSetAxisRange();
      }
    }

    private void Axis_PlacementChanged(object sender, EventArgs e)
    {
      UpdateAxisArrangement();
      NormalizeSizes();
    }

    private void Axis_AxisUpdated(object sender, EventArgs e)
    {
      BuildChartLater();
    }

    private void Axis_ValueConverterChanged(object sender, EventArgs e)
    {
      AnalyseAndRebuildAllLater();
    }

    private void YAxis_TitleChanged(object sender, EventArgs e)
    {
      foreach (DataSeries series in Series)
      {
        series.YAxis = GetYAxis(series.YAxisTitle);
      }
      AutoSetAxisRange();
    }

    #endregion // Axis Management

    #region Analyse and Render

#if DEBUG
    private readonly Stopwatch _stopwatch = new Stopwatch();
#endif

    private bool _analyseSeriesQueued;

    private void AnalyseAndRebuildAllLater()
    {
#if DEBUG
      _stopwatch.Start();
#endif
      if (!_analyseSeriesQueued)
      {
        _analyseSeriesQueued = true;
        Dispatcher.BeginInvoke(new Action(AnalyseAndRebuildAll));
      }
    }

    /// <summary>
    /// Forces the chart to auto-calculate the axis range and render the data.
    /// This method is useful for printing and exporting charts that are not added to a visual tree and
    /// generally should not be used in normal application operations.
    /// </summary>
    public void ForceRender()
    {
      if (_canvas != null)
      {
        foreach (DataSeries series in Series)
        {
          series.AnalyseSeries();
        }
        AutoSetAxisRange();
        BuildChart();
      }
    }

    private void AnalyseAndRebuildAll()
    {
      if (IsLoaded)
      {
        foreach (DataSeries series in Series)
        {
          series.AnalyseSeries();
        }
        AutoSetAxisRange();
        if (_canvas != null)
        {
          BuildChartLater();
        }
      }
      _analyseSeriesQueued = false;
      AddWatermark();
    }

    private bool _buildChartQueued;

    private void BuildChartLater()
    {
      if (!_buildChartQueued && IsLoaded)
      {
        _buildChartQueued = true;
        Dispatcher.BeginInvoke(new Action(BuildChart), DispatcherPriority.Render);
      }
    }

    /*internal bool IsBuildChartQueued
    {
      get { return _buildChartQueued; }
    }*/

    //private bool _skipLock;/
    //internal bool SkipFirstBuild { get; set; }

    private void BuildChart()
    {
      /*if (SkipFirstBuild && !_skipLock)
      {
        _skipLock = true;
        _buildChartQueued = false;
        SkipFirstBuild = false;
        return;
      }*/
      UpdateLegend();
      if (Visibility == Visibility.Visible)
      {
        foreach (DataSeries series in Series)
        {
          series.UpdateOptimizationProperties();
        }
      }
      UpdateMinDelta();

      _canvas.Children.Clear();
      if (_foregroundCanvas != null)
      {
        _foregroundCanvas.Children.Clear();
        UpdateZoomBox();
      }
      if (Visibility == Visibility.Visible)
      {
        foreach (DataSeries series in Series)
        {
          series.BuildChart();
        }
        AddWatermark();
      }
      _buildChartQueued = false;
      //Debug.WriteLine("Finished plotting data");
      OnFinishedPlottingData();
#if DEBUG
      _stopwatch.Stop();
      Debug.WriteLine("Time: " + _stopwatch.ElapsedMilliseconds);
      _stopwatch.Reset();
#endif
    }

    private void UpdateZoomBox()
    {
      if (_isMouseDown && MouseMode == ChartMouseMode.Zoom)
      {
        _foregroundCanvas.Children.Add(_zoomBorder);

        double mouseDownX = XAxis.ConvertLogicalToPhysical(_logicalMouseDown.X);
        double mouseDownY = _canvas.ActualHeight - YAxis.ConvertLogicalToPhysical(_logicalMouseDown.Y);

        double deltaX = _mouseMovePoint.X - mouseDownX;
        double deltaY = _mouseMovePoint.Y - mouseDownY;

        if (MouseMode == ChartMouseMode.Zoom && ZoomMode != ZoomMode.None)
        {
          if (ZoomMode != ZoomMode.Vertical)
          {
            Canvas.SetLeft(_zoomBorder, Math.Min(_mouseMovePoint.X, mouseDownX));
          }
          if (ZoomMode != ZoomMode.Horizontal)
          {
            Canvas.SetTop(_zoomBorder, Math.Min(_mouseMovePoint.Y, mouseDownY));
          }
          if (deltaX > 0 && ZoomMode != ZoomMode.Vertical)
          {
            _zoomBorder.Width = Math.Max(0, Math.Min(deltaX, _canvas.ActualWidth - mouseDownX));
          }
          else if (ZoomMode != ZoomMode.Vertical)
          {
            _zoomBorder.Width = Math.Max(0, Math.Abs(deltaX));
          }
          if (deltaY > 0 && ZoomMode != ZoomMode.Horizontal)
          {
            _zoomBorder.Height = Math.Max(0, Math.Min(deltaY, _canvas.ActualHeight - mouseDownY));
          }
          else if (ZoomMode != ZoomMode.Horizontal)
          {
            _zoomBorder.Height = Math.Max(0, Math.Abs(deltaY));
          }
        }
      }
    }

    private void UpdateMinDelta()
    {
      Dictionary<ChartAxis, double> _minDeltaMap = new Dictionary<ChartAxis,double>();
      foreach (DataSeries series in Series)
      {
        if (series.MinDelta != 0)
        {
          double tempMinDelta = series.MinDelta * Math.Max(1.0, series.GetIndexStep());
          if (series.ReverseAxes)
          {
            double verticalMinDelta = 0;
            _minDeltaMap.TryGetValue(series.YAxis, out verticalMinDelta);
            verticalMinDelta = verticalMinDelta == 0 ? tempMinDelta : Math.Min(verticalMinDelta, series.MinDelta * Math.Max(1.0, series.GetIndexStep()));
            _minDeltaMap[series.YAxis] = verticalMinDelta;
          }
          else
          {
            double horizontalMinDelta = 0;
            _minDeltaMap.TryGetValue(series.XAxis, out horizontalMinDelta);
            horizontalMinDelta = horizontalMinDelta == 0 ? tempMinDelta : Math.Min(horizontalMinDelta, series.MinDelta * Math.Max(1.0, series.GetIndexStep()));
            _minDeltaMap[series.XAxis] = horizontalMinDelta;
          }
        }
      }

      double minDelta = 0;
      _minDeltaMap.TryGetValue(XAxis, out minDelta);
      XAxis.MinDelta = minDelta;// == 0 ? 1 : minDelta;
      _minDeltaMap.TryGetValue(YAxis, out minDelta);
      YAxis.MinDelta = minDelta;// == 0 ? 1 : minDelta;
      foreach (ChartAxis axis in AlternativeYAxes)
      {
        _minDeltaMap.TryGetValue(axis, out minDelta);
        axis.MinDelta = minDelta;
      }
      foreach (ChartAxis axis in AlternativeXAxes)
      {
        _minDeltaMap.TryGetValue(axis, out minDelta);
        axis.MinDelta = minDelta;
      }
    }

    //internal double HorizontalMinDelta { get; private set; }
    //internal double VerticalMinDelta { get; private set; }

    /// <summary>
    /// Raised when the chart has finished plotting/rendering the data.
    /// </summary>
    public event EventHandler FinishedPlottingData;

    private void OnFinishedPlottingData()
    {
      EventHandler handler = FinishedPlottingData;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    [Conditional("TRIAL")]
    private void AddWatermark()
    {
      Watermark.AddWatermark(_canvas);
    }

    #endregion // Analyse and Render

    #region XAxis property

    private ChartAxis _defaultXAxis;

    /// <summary>
    /// Gets or sets the X (horizontal) axis of the <see cref="Chart"/>.  This can be
    /// used to configure axis settings.
    /// This is a dependency property.
    /// </summary>
    public ChartAxis XAxis
    {
      get
      {
        ChartAxis axis = (ChartAxis)GetValue(XAxisProperty);
        if (axis == null)
        {
          if (_defaultXAxis == null)
          {
            _defaultXAxis = new ChartAxis() { Title = "X Axis", IsAuto = true };
            PrepareXAxis(_defaultXAxis);
          }
          axis = _defaultXAxis;
        }
        else
        {
          if (_defaultXAxis != null)
          {
            DestroyAxis(_defaultXAxis, false);
            _defaultXAxis = null;
          }
        }
        return axis;
      }
      set { SetValue(XAxisProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="XAxis"/> property.
    /// </summary>
    public static readonly DependencyProperty XAxisProperty =
      DependencyProperty.Register("XAxis", typeof(ChartAxis), typeof(Chart),
      new PropertyMetadata(new PropertyChangedCallback(OnXAxisChanged)));

    private static void OnXAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Chart)d).OnXAxisChanged(e);
    }

    private void OnXAxisChanged(DependencyPropertyChangedEventArgs e)
    {
      ChartAxis oldAxis = e.OldValue as ChartAxis;
      if (oldAxis != null)
      {
        DestroyAxis(oldAxis, true);
      }
      if (XAxis != null)
      {
        PrepareXAxis(XAxis);
      }

      EventHandler handler = XAxisChanged;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    private void PrepareXAxis(ChartAxis axis)
    {
      axis.Orientation = Orientation.Horizontal;
      foreach (DataSeries series in Series)
      {
        series.XAxis = GetXAxis(series.XAxisTitle);
      }
      PrepareAxis(axis);
    }

    internal event EventHandler XAxisChanged;

    #endregion // XAxis property

    #region YAxis property

    private ChartAxis _defaultYAxis;

    /// <summary>
    /// Gets or sets the YAxis used by the <see cref="Chart"/>.
    /// This is a dependency property.
    /// </summary>
    public ChartAxis YAxis
    {
      get
      {
        ChartAxis axis = (ChartAxis)GetValue(YAxisProperty);
        if (axis == null)
        {
          if (_defaultYAxis == null)
          {
            _defaultYAxis = new ChartAxis() { Title = "Y Axis", IsAuto = true };
            PrepareYAxis(_defaultYAxis);
          }
          axis = _defaultYAxis;
        }
        else
        {
          if (_defaultYAxis != null)
          {
            DestroyAxis(_defaultYAxis, false);
            _defaultYAxis = null;
          }
        }
        return axis;
      }
      set { SetValue(YAxisProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="YAxis"/> property.
    /// </summary>
    public static readonly DependencyProperty YAxisProperty =
      DependencyProperty.Register("YAxis", typeof(ChartAxis), typeof(Chart),
      new PropertyMetadata(new PropertyChangedCallback(OnYAxisChanged)));

    private static void OnYAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Chart)d).OnYAxisChanged(e);
    }

    private void OnYAxisChanged(DependencyPropertyChangedEventArgs e)
    {
      ChartAxis oldAxis = e.OldValue as ChartAxis;
      if (oldAxis != null)
      {
        oldAxis.TitleChanged -= new EventHandler(YAxis_TitleChanged);
        DestroyAxis(oldAxis, true);
      }
      if (YAxis != null)
      {
        PrepareYAxis(YAxis);
        AnalyseAndRebuildAllLater();
      }

      EventHandler handler = YAxisChanged;
      if (handler != null)
      {
        handler(this, EventArgs.Empty);
      }
    }

    private void PrepareYAxis(ChartAxis axis)
    {
      axis.Orientation = Orientation.Vertical;
      foreach (DataSeries series in Series)
      {
        series.YAxis = GetYAxis(series.YAxisTitle);
      }
      axis.TitleChanged += new EventHandler(YAxis_TitleChanged);
      PrepareAxis(axis);
    }

    internal event EventHandler YAxisChanged;

    #endregion // YAxis property

    #region TitleTemplate property

    /// <summary>
    /// Gets or sets the <see cref="DataTemplate"/> applied to the title of the <see cref="Chart"/>.
    /// This is a dependency property.
    /// </summary>
    public DataTemplate TitleTemplate
    {
      get { return (DataTemplate)GetValue(TitleTemplateProperty); }
      set { SetValue(TitleTemplateProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="TitleTemplate"/> property.
    /// </summary>
    public static readonly DependencyProperty TitleTemplateProperty =
      DependencyProperty.Register("TitleTemplate", typeof(DataTemplate), typeof(Chart),
      new PropertyMetadata(null));

    #endregion // TitleTemplate property

    #region ZoomBoxStyle property

    /// <summary>
    /// Gets or sets the <see cref="Style"/> to be applied to the zoom box.
    /// This is a dependency property.
    /// </summary>
    public Style ZoomBoxStyle
    {
      get { return (Style)GetValue(ZoomBoxStyleProperty); }
      set { SetValue(ZoomBoxStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ZoomBoxStyle"/> property.
    /// </summary>
    public static readonly DependencyProperty ZoomBoxStyleProperty =
      DependencyProperty.Register("ZoomBoxStyle", typeof(Style), typeof(Chart),
      new PropertyMetadata(BuildDefaultZoomBoxStyle(), new PropertyChangedCallback(OnZoomBoxStyleChanged)));

    private static void OnZoomBoxStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Chart)d).OnZoomBoxStyleChanged();
    }

    private void OnZoomBoxStyleChanged()
    {
      _zoomBorder.Style = ZoomBoxStyle;
    }

    private static Style BuildDefaultZoomBoxStyle()
    {
      Style style = new Style(typeof(Rectangle));
      style.Setters.Add(new Setter(Rectangle.FillProperty, new SolidColorBrush(new Color() { A = 150, R = 132, G = 132, B = 200 })));
      style.Setters.Add(new Setter(Rectangle.StrokeThicknessProperty, 2.0));
      style.Setters.Add(new Setter(Rectangle.StrokeProperty, new SolidColorBrush(new Color() { A = 150, R = 32, G = 32, B = 50 })));
      return style;
    }

    #endregion // ZoomBoxStyle property

    #region LegendStyle property

    /// <summary>
    /// Gets or sets the <see cref="Style"/> applied to the <see cref="HeaderedItemsControl"/> that displays the legend items.
    /// This is a dependency property.
    /// </summary>
    public Style LegendStyle
    {
      get { return (Style)GetValue(LegendStyleProperty); }
      set { SetValue(LegendStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="LegendStyle"/> property.
    /// </summary>
    public static readonly DependencyProperty LegendStyleProperty =
      DependencyProperty.Register("LegendStyle", typeof(Style), typeof(Chart),
      new PropertyMetadata(null));

    #endregion // LegendStyle property

    #region SelectionMode property

    /// <summary>
    /// Gets or sets the selection mode of the <see cref="Chart"/>.
    /// This is a dependency property.
    /// </summary>
    public DataSeriesSelectionMode SelectionMode
    {
      get { return (DataSeriesSelectionMode)GetValue(SelectionModeProperty); }
      set { SetValue(SelectionModeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="SelectionMode"/> property.
    /// </summary>
    public static readonly DependencyProperty SelectionModeProperty =
      DependencyProperty.Register("SelectionMode", typeof(DataSeriesSelectionMode), typeof(Chart),
      new PropertyMetadata(new PropertyChangedCallback(OnSelectionModeChanged)));

    private static void OnSelectionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Chart)d).OnSelectionModeChanged();
    }

    private void OnSelectionModeChanged()
    {
      if (SelectionMode == DataSeriesSelectionMode.Single)
      {
        foreach (DataSeries series in Series)
        {
          series.DeselectAll();
        }
      }
    }

    #endregion // SelectionMode property

    #region ZoomMode property

    /// <summary>
    /// Gets or sets the current <see cref="ZoomMode"/>.
    /// This is a dependency property.
    /// </summary>
    public ZoomMode ZoomMode
    {
      get { return (ZoomMode)GetValue(ZoomModeProperty); }
      set { SetValue(ZoomModeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ZoomMode"/> property.
    /// </summary>
    public static readonly DependencyProperty ZoomModeProperty =
      DependencyProperty.Register("ZoomMode", typeof(ZoomMode), typeof(Chart),
      new PropertyMetadata(ZoomMode.None, new PropertyChangedCallback(OnZoomModeChanged)));

    private static void OnZoomModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Chart)d).OnZoomModeChanged();
    }

    private void OnZoomModeChanged()
    {
      if (_mouseCanvas != null)
      {
        if (ZoomMode == ZoomMode.None)
        {
          _mouseCanvas.IsHitTestVisible = false;
        }
        else
        {
          _mouseCanvas.IsHitTestVisible = true;
        }
      }
    }

    #endregion // ZoomMode property

    #region LegendPosition property

    /// <summary>
    /// Gets or sets the position on the <see cref="Chart"/> control where the legend is displayed.
    /// This is a dependency property.
    /// </summary>
    public LegendPosition LegendPosition
    {
      get { return (LegendPosition)GetValue(LegendPositionProperty); }
      set { SetValue(LegendPositionProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="LegendPosition"/> property.
    /// </summary>
    public static readonly DependencyProperty LegendPositionProperty =
      DependencyProperty.Register("LegendPosition", typeof(LegendPosition), typeof(Chart),
      new PropertyMetadata(LegendPosition.Right, new PropertyChangedCallback(OnLegendPositionChanged)));

    private static void OnLegendPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Chart)d).OnLegendPositionChanged();
    }

    private bool _isUsingDefaultCenterLegendStyle;

    private void OnLegendPositionChanged()
    {
      if (LegendPosition == LegendPosition.Center && LegendStyle == null)
      {
        _isUsingDefaultCenterLegendStyle = true;
        Style centerLegendStyle = new Style(typeof(Legend));
        centerLegendStyle.Setters.Add(new Setter(Legend.HorizontalAlignmentProperty, HorizontalAlignment.Right));
        centerLegendStyle.Setters.Add(new Setter(Legend.VerticalAlignmentProperty, VerticalAlignment.Top));
        LegendStyle = centerLegendStyle;
      }
      else if (LegendPosition != LegendPosition.Center && _isUsingDefaultCenterLegendStyle)
      {
        LegendStyle = null;
      }
    }

    #endregion // LegendPosition property

    #region Title property

    /// <summary>
    /// Gets or sets the title of the <see cref="Chart"/>.
    /// This is a dependency property.
    /// </summary>
    public string Title
    {
      get { return (string)GetValue(TitleProperty); }
      set { SetValue(TitleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Title"/> property.
    /// </summary>
    public static readonly DependencyProperty TitleProperty =
      DependencyProperty.Register("Title", typeof(string), typeof(Chart),
      new PropertyMetadata(null));

    #endregion // Title property

    #region CanToggleSelection Property

    /// <summary>
    /// Gets or sets whether or not the user can toggle the selection of a series or data point.
    /// When true, clicking a series or data point will toggle the selection.
    /// When false, clicking a selected series or data point will not deselect it.
    /// The default is true.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="CanToggleSelectionProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool CanToggleSelection
    {
      get { return (bool)GetValue(CanToggleSelectionProperty); }
      set { SetValue(CanToggleSelectionProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="CanToggleSelection"/> property.
    /// </summary>
    public static readonly DependencyProperty CanToggleSelectionProperty =
      DependencyProperty.Register("CanToggleSelection", typeof(bool), typeof(Chart),
      new FrameworkPropertyMetadata(true, OnCanToggleSelectionChanged));

    private static void OnCanToggleSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Chart)d).OnCanToggleSelectionChanged();
    }

    private void OnCanToggleSelectionChanged()
    {
    }

    #endregion // CanToggleSelection Property

    #region CanDeselectOnClickNothing Property

    /// <summary>
    /// Gets or sets whether or not clicking nothing (an empty area on the chart) will deselect the current selection. The default is false.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="CanDeselectOnClickNothingProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool CanDeselectOnClickNothing
    {
      get { return (bool)GetValue(CanDeselectOnClickNothingProperty); }
      set { SetValue(CanDeselectOnClickNothingProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="CanDeselectOnClickNothing"/> property.
    /// </summary>
    public static readonly DependencyProperty CanDeselectOnClickNothingProperty =
      DependencyProperty.Register("CanDeselectOnClickNothing", typeof(bool), typeof(Chart),
      new FrameworkPropertyMetadata(false, OnCanDeselectOnClickNothingChanged));

    private static void OnCanDeselectOnClickNothingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Chart)d).OnCanDeselectOnClickNothingChanged();
    }

    private void OnCanDeselectOnClickNothingChanged()
    {
    }

    #endregion // CanDeselectOnClickNothing Property

    #region IsRightClickSelectionEnabled Property

    /// <summary>
    /// Gets or sets whether or not the right mouse button participates in selection. The default is false.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsRightClickSelectionEnabledProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsRightClickSelectionEnabled
    {
      get { return (bool)GetValue(IsRightClickSelectionEnabledProperty); }
      set { SetValue(IsRightClickSelectionEnabledProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsRightClickSelectionEnabled"/> property.
    /// </summary>
    public static readonly DependencyProperty IsRightClickSelectionEnabledProperty =
      DependencyProperty.Register("IsRightClickSelectionEnabled", typeof(bool), typeof(Chart),
      new FrameworkPropertyMetadata(false, OnIsRightClickSelectionEnabledChanged));

    private static void OnIsRightClickSelectionEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Chart)d).OnIsRightClickSelectionEnabledChanged();
    }

    private void OnIsRightClickSelectionEnabledChanged()
    {
    }

    #endregion // IsRightClickSelectionEnabled Property

    #region IsChartClipped Property

    /// <summary>
    /// Gets or sets whether or not the chart visuals are clipped within the axis region.
    /// If false, the chart can overlap the axes. This is useful for chart symbols from line series etc.
    /// The default is true.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="IsChartClippedProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool IsChartClipped
    {
      get { return (bool)GetValue(IsChartClippedProperty); }
      set { SetValue(IsChartClippedProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsChartClipped"/> property.
    /// </summary>
    public static readonly DependencyProperty IsChartClippedProperty =
      DependencyProperty.Register("IsChartClipped", typeof(bool), typeof(Chart),
      new FrameworkPropertyMetadata(true, OnIsChartClippedChanged));

    private static void OnIsChartClippedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Chart)d).OnIsChartClippedChanged();
    }

    private void OnIsChartClippedChanged()
    {
      if (_axisGrid != null && _foregroundRoot != null)
      {
        int index = _axisGrid.Children.IndexOf(_foregroundRoot);
        if (index >= 0)
        {
          _axisGrid.Children.RemoveAt(index);
          if (IsChartClipped)
          {
            _axisGrid.Children.Insert(0, _chartRoot);
          }
          else
          {
            _axisGrid.Children.Add(_chartRoot);
          }
        }
      }
    }

    #endregion // IsChartClipped Property

    #region Basic Properties

    /// <summary>
    /// Gets the collection of the <see cref="DataSeries"/> plotted on the chart.
    /// </summary>
    public ObservableCollection<DataSeries> Series { get { return _series; } }

    /// <summary>
    /// Gets a collection of alternative Y axes.
    /// </summary>
    /// <remarks>Multiple Y axes allow for plotting data on different scales
    /// on the same graph.</remarks>
    public Collection<ChartAxis> AlternativeYAxes
    {
      get { return _alternateYAxes; }
    }

    /// <summary>
    /// Gets a collection of alternative X axes.
    /// </summary>
    /// <remarks>Multiple X axes allow for plotting data on different scales
    /// on the same graph.</remarks>
    public Collection<ChartAxis> AlternativeXAxes
    {
      get { return _alternateXAxes; }
    }

    /// <summary>
    /// Gets the items to be displayed in the legend.
    /// </summary>
    public ReadOnlyCollection<LegendItem> LegendItems
    {
      get
      {
        UpdateLegend();
        return _legendItems;
      }
    }

    /// <summary>
    /// Gets the collection of <see cref="UIElement"/> objects displayed in front of the chart.
    /// </summary>
    public Collection<UIElement> ForegroundElements
    {
      get { return _foregroundElements; }
    }

    /// <summary>
    /// Gets the collection of <see cref="UIElement"/> objects displayed behind the chart.
    /// </summary>
    public Collection<UIElement> BackgroundElements
    {
      get { return _backgroundElements; }
    }

    /// <summary>
    /// Gets the current <see cref="ChartMouseMode"/>.
    /// </summary>
    public ChartMouseMode MouseMode { get; internal set; }

    #endregion // Basic Properties

    /// <summary>
    /// Raised when the selected data series changes.
    /// </summary>
    public event EventHandler<SelectedDataSeriesChangeddEventArgs> SelectedDataSeriesChanged;

    private void OnSelectedDataSeriesChanged(SelectedDataSeriesChangeddEventArgs args)
    {
      EventHandler<SelectedDataSeriesChangeddEventArgs> handler = SelectedDataSeriesChanged;
      if (handler != null)
      {
        handler(this, args);
      }
    }
  }
}
