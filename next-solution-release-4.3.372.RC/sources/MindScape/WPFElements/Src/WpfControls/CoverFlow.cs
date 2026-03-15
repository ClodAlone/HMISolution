using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using System.Windows.Media.Animation;
using System.ComponentModel;
using Infralution.Licensing;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// A control that arranges items in a row such that the selected object
  /// is displayed in the center of the control, with the other items arranged to
  /// either side in a pseudo-perspective view, and animates the transition
  /// when the selection changes.
  /// </summary>
  public class CoverFlow : Selector
  {
    static CoverFlow()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(CoverFlow), 
        new FrameworkPropertyMetadata(typeof(CoverFlow)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CoverFlow"/> item.
    /// </summary>
    public CoverFlow()
    {
      SizeChanged += new SizeChangedEventHandler(CoverFlow_SizeChanged);

      AddHandler(CoverFlowItem.CoverFlowItemMouseActionEvent, new EventHandler<CoverFlowItem.CoverFlowItemMouseActionEventArgs>(OnCoverFlowItemMouseAction));
    }

    private void CoverFlow_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      RefreshUI();
    }

    #region Sizing properties

    /// <summary>
    /// Gets or sets the height of the items within the <see cref="CoverFlow"/>.
    /// This represents the size of the selected item; the <see cref="ScaleX"/> and
    /// <see cref="ScaleY"/> properties are applied to non-selected items.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ItemHeightProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double ItemHeight
    {
      get { return (double)GetValue(ItemHeightProperty); }
      set { SetValue(ItemHeightProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ItemHeight"/> property.
    /// </summary>
    public static readonly DependencyProperty ItemHeightProperty =
      DependencyProperty.Register("ItemHeight", typeof(double), typeof(CoverFlow),
      new FrameworkPropertyMetadata(150.0, OnItemSizingPropertyChanged));

    /// <summary>
    /// Gets or sets the width of the items within the <see cref="CoverFlow"/>.
    /// This represents the size of the selected item; the <see cref="ScaleX"/> and
    /// <see cref="ScaleY"/> properties are applied to non-selected items.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ItemWidthProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double ItemWidth
    {
      get { return (double)GetValue(ItemWidthProperty); }
      set { SetValue(ItemWidthProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ItemWidth"/> property.
    /// </summary>
    public static readonly DependencyProperty ItemWidthProperty =
      DependencyProperty.Register("ItemWidth", typeof(double), typeof(CoverFlow),
      new FrameworkPropertyMetadata(180.0, OnItemSizingPropertyChanged));

    private static void OnItemSizingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((CoverFlow)d).OnLayoutPropertyChanged();
    }

    #endregion

    #region Selection properties

    /// <summary>
    /// Gets or sets how the user can select items using the mouse.
    /// The default is <see cref="CoverFlowMouseSelectionMode.MousePressed"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="MouseSelectionModeProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public CoverFlowMouseSelectionMode MouseSelectionMode
    {
      get { return (CoverFlowMouseSelectionMode)GetValue(MouseSelectionModeProperty); }
      set { SetValue(MouseSelectionModeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="MouseSelectionMode"/> property.
    /// </summary>
    public static readonly DependencyProperty MouseSelectionModeProperty =
      DependencyProperty.Register("MouseSelectionMode", typeof(CoverFlowMouseSelectionMode), typeof(CoverFlow),
      new FrameworkPropertyMetadata(CoverFlowMouseSelectionMode.MousePressed));

    #endregion

    #region Item layout properties

    /// <summary>
    /// Gets or sets the vertical shear of the items within this <see cref="CoverFlow"/>, in degrees.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ShearAngleProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double ShearAngle
    {
      get { return (double)GetValue(ShearAngleProperty); }
      set { SetValue(ShearAngleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ShearAngle"/> property.
    /// </summary>
    public static readonly DependencyProperty ShearAngleProperty =
      DependencyProperty.Register("ShearAngle", typeof(double), typeof(CoverFlow),
      new FrameworkPropertyMetadata(10.0, OnShearAngleChanged));

    private static void OnShearAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((CoverFlow)d).OnShearAngleChanged();
    }

    private void OnShearAngleChanged()
    {
      OnLayoutPropertyChanged();
    }

    /// <summary>
    /// Gets or sets the separation between the selected item and the items adjacent to it.
    /// This is instead of, not additional to, the <see cref="ItemSeparation"/> property,
    /// and should usually be larger than it.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="SelectedItemSeparationProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double SelectedItemSeparation
    {
      get { return (double)GetValue(SelectedItemSeparationProperty); }
      set { SetValue(SelectedItemSeparationProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="SelectedItemSeparation"/> property.
    /// </summary>
    public static readonly DependencyProperty SelectedItemSeparationProperty =
      DependencyProperty.Register("SelectedItemSeparation", typeof(double), typeof(CoverFlow),
      new FrameworkPropertyMetadata(200.0, OnSelectedItemSeparationChanged));

    private static void OnSelectedItemSeparationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((CoverFlow)d).OnSelectedItemSeparationChanged();
    }

    private void OnSelectedItemSeparationChanged()
    {
      OnLayoutPropertyChanged();
    }

    /// <summary>
    /// Gets or sets the item spacing within the <see cref="CoverFlow"/>.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ItemSeparationProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double ItemSeparation
    {
      get { return (double)GetValue(ItemSeparationProperty); }
      set { SetValue(ItemSeparationProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ItemSeparation"/> property.
    /// </summary>
    public static readonly DependencyProperty ItemSeparationProperty =
      DependencyProperty.Register("ItemSeparation", typeof(double), typeof(CoverFlow),
      new FrameworkPropertyMetadata(120.0, OnItemSeparationChanged));

    private static void OnItemSeparationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((CoverFlow)d).OnItemSeparationChanged();
    }

    private void OnItemSeparationChanged()
    {
      OnLayoutPropertyChanged();
    }


    /// <summary>
    /// Gets or sets the horizontal scaling applied to the non-selected items.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ScaleXProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double ScaleX
    {
      get { return (double)GetValue(ScaleXProperty); }
      set { SetValue(ScaleXProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ScaleX"/> property.
    /// </summary>
    public static readonly DependencyProperty ScaleXProperty =
      DependencyProperty.Register("ScaleX", typeof(double), typeof(CoverFlow),
      new FrameworkPropertyMetadata(0.75, OnScaleXChanged));

    private static void OnScaleXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((CoverFlow)d).OnScaleXChanged();
    }

    private void OnScaleXChanged()
    {
      OnLayoutPropertyChanged();
    }

    /// <summary>
    /// Gets or sets the vertical scaling applied to the non-selected items.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ScaleYProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double ScaleY
    {
      get { return (double)GetValue(ScaleYProperty); }
      set { SetValue(ScaleYProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ScaleY"/> property.
    /// </summary>
    public static readonly DependencyProperty ScaleYProperty =
      DependencyProperty.Register("ScaleY", typeof(double), typeof(CoverFlow),
      new FrameworkPropertyMetadata(0.75, OnScaleYChanged));

    private static void OnScaleYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((CoverFlow)d).OnScaleYChanged();
    }

    private void OnScaleYChanged()
    {
      OnLayoutPropertyChanged();
    }

    #endregion

    #region Reflection properties

    /// <summary>
    /// Gets or sets the vertical scale applied to the reflection.
    /// This must be between 0 and 1.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ReflectionScaleYProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public double ReflectionScaleY
    {
      get { return (double)GetValue(ReflectionScaleYProperty); }
      set { SetValue(ReflectionScaleYProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ReflectionScaleY"/> property.
    /// </summary>
    public static readonly DependencyProperty ReflectionScaleYProperty =
      DependencyProperty.Register("ReflectionScaleY", typeof(double), typeof(CoverFlow),
      new FrameworkPropertyMetadata(0.6));

    /// <summary>
    /// Gets or sets whether the <see cref="CoverFlow"/> shows reflections below each item.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ShowReflectionProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public bool ShowReflection
    {
      get { return (bool)GetValue(ShowReflectionProperty); }
      set { SetValue(ShowReflectionProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ShowReflection"/> property.
    /// </summary>
    public static readonly DependencyProperty ShowReflectionProperty =
      DependencyProperty.Register("ShowReflection", typeof(bool), typeof(CoverFlow),
      new FrameworkPropertyMetadata(true, OnShowReflectionChanged));

    private static void OnShowReflectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((CoverFlow)d).OnShowReflectionChanged();
    }

    private void OnShowReflectionChanged()
    {
      RepositionItems();
    }

    /// <summary>
    /// Gets or sets the <see cref="Brush"/> used in the reflection of each <see cref="CoverFlowItem"/>.
    /// This brush is overlaid on the reflection and should typically contain a gradient from a solid
    /// color (equal to the background color of the <see cref="CoverFlow"/>) to a translucent version of that color.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="ReflectionBrushProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Brush ReflectionBrush
    {
      get { return (Brush)GetValue(ReflectionBrushProperty); }
      set { SetValue(ReflectionBrushProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ReflectionBrush"/> property.
    /// </summary>
    public static readonly DependencyProperty ReflectionBrushProperty =
      DependencyProperty.Register("ReflectionBrush", typeof(Brush), typeof(CoverFlow));

    #endregion

    #region TransitionDuration Property

    /// <summary>
    /// Gets or sets the <see cref="Duration"/> of the coverflow animations.
    /// This is a dependency property.
    /// </summary>
    /// <remarks>
    /// <strong>Dependency Property Information</strong>
    /// <table>
    ///   <tr><td>Identifier field</td><td><see cref="TransitionDurationProperty"/></td></tr>
    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
    /// </table>
    /// </remarks>
    public Duration TransitionDuration
    {
      get { return (Duration)GetValue(TransitionDurationProperty); }
      set { SetValue(TransitionDurationProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="TransitionDuration"/> property.
    /// </summary>
    public static readonly DependencyProperty TransitionDurationProperty =
      DependencyProperty.Register("TransitionDuration", typeof(Duration), typeof(CoverFlow),
      new FrameworkPropertyMetadata(new Duration(new TimeSpan(0,0,0,0,200)), OnTransitionDurationChanged));

    private static void OnTransitionDurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((CoverFlow)d).OnTransitionDurationChanged();
    }

    private void OnTransitionDurationChanged()
    {
    }

    #endregion // TransitionDuration Property

    private void OnLayoutPropertyChanged()
    {
      RepositionItems();
      RefreshUI();
    }

    private void RepositionItems()
    {
      foreach (object item in Items)
      {
        CoverFlowItem coverFlowItem = ItemContainerGenerator.ContainerFromItem(item) as CoverFlowItem;
        if (coverFlowItem != null)
        {
          coverFlowItem.SetItemHeightWidth(ItemWidth, ItemHeight, ShowReflection);
        }
      }
    }

    private bool _isAnimating;

    private void RefreshUI()
    {
      _isAnimating = false;

      int zIndexMultiplier = (ShearAngle < -5 ? 1 : -1);
      int childCount = Items.Count;
      int selectedIndex = SelectedIndex;
      double centre = (int)((ActualWidth - ItemWidth) / 2.0);

      _isAnimating = true;

      for (int i = 0; i < childCount && _isAnimating; ++i)
      {
        CoverFlowItem cover = ItemContainerGenerator.ContainerFromIndex(i) as CoverFlowItem;
        
        if (cover != null)
        {
          if (i < selectedIndex)
          {
            cover.SetZIndex(i * -zIndexMultiplier);

            int indexRelativeToSelected = (selectedIndex - 1) - i;
            double left = Math.Round(centre - (SelectedItemSeparation + indexRelativeToSelected * ItemSeparation));

            cover.MoveTo(ScaleX, ScaleY, -ShearAngle, left, ItemWidth, TransitionDuration);
          }
          else if (i > selectedIndex)
          {
            cover.SetZIndex(i * zIndexMultiplier);

            int indexRelativeToSelected = (i - selectedIndex - 1);
            double left = Math.Round(centre + (SelectedItemSeparation + indexRelativeToSelected * ItemSeparation));

            cover.MoveTo(ScaleX, ScaleY, ShearAngle, left, ItemWidth, TransitionDuration);
          }
          else /* i == SelectedIndex */
          {
            cover.SetZIndex(childCount + 1);

            cover.MoveTo(1, 1, 0, centre, ItemWidth, TransitionDuration);
          }
        }
      }

      _isAnimating = false;
    }

    private void OnCoverFlowItemMouseAction(object sender, CoverFlowItem.CoverFlowItemMouseActionEventArgs e)
    {
      if (e.Matches(MouseSelectionMode))
      {
        CoverFlowItem coverFlowItem = e.OriginalSource as CoverFlowItem;
        if (coverFlowItem != null)
        {
          SelectedItem = ItemContainerGenerator.ItemFromContainer(coverFlowItem);
        }
      }

      UIElement element = Mouse.DirectlyOver as UIElement;
      if (element != null)
      {
        Control control = VisualTreeUtils.FindAncestor<Control>(element);
        if (control is CoverFlowItem)
        {
          Focus();
        }
      }
    }

    /// <summary>
    /// Responds to a <see cref="CoverFlow"/> selection change by raising 
    /// a <see cref="Selector.SelectionChanged"/> event.
    /// </summary>
    /// <param name="e">Provides data for <see cref="SelectionChangedEventArgs"/></param>
    protected override void OnSelectionChanged(SelectionChangedEventArgs e)
    {
      RefreshUI();

      base.OnSelectionChanged(e);
    }

    #region Override item type

    /// <summary>
    /// Creates or identifies the element that is used to display the given item.
    /// </summary>
    /// <returns>The element that is used to display the given item.</returns>
    protected override DependencyObject GetContainerForItemOverride()
    {
      return new CoverFlowItem();
    }

    /// <summary>
    /// Determines if the specified item is (or is eligible to be) its own container.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <returns>true if the item is (or is eligible to be) its own container; otherwise, false.</returns>
    protected override bool IsItemItsOwnContainerOverride(object item)
    {
      return item is CoverFlowItem;
    }

    /// <summary>
    /// Prepares the specified element to display the specified item.
    /// </summary>
    /// <param name="element">Element used to display the specified item.</param>
    /// <param name="item">Specified item.</param>
    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
      base.PrepareContainerForItemOverride(element, item);

      CoverFlowItem flowItem = (CoverFlowItem)element;
      flowItem.SetItemHeightWidth(ItemWidth, ItemHeight, ShowReflection);
    }

    #endregion

  }
}
