//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Windows.Controls;
//using System.Windows;
//using System.Windows.Data;

//namespace Mindscape.WpfElements
//{
//  /// <summary>
//  /// Represents a collapsed <see cref="OutlookBarItem"/>.
//  /// </summary>
//  public class CollapsedOutlookBarItem : ContentControl
//  {
//    private readonly OutlookBarItem _item;

//    static CollapsedOutlookBarItem()
//    {
//      DefaultStyleKeyProperty.OverrideMetadata(typeof(CollapsedOutlookBarItem),
//        new FrameworkPropertyMetadata(typeof(CollapsedOutlookBarItem)));
//    }

//    internal CollapsedOutlookBarItem(OutlookBarItem item)
//    {
//      _item = item;

//      Binding selectionBinding = new Binding("IsSelected") { Source = _item, Mode = BindingMode.TwoWay };
//      BindingOperations.SetBinding(this, IsSelectedProperty, selectionBinding);

//      Binding contentBinding = new Binding("CollapsedHeader") { Source = _item };
//      BindingOperations.SetBinding(this, ContentProperty, contentBinding);

//      // TODO
//      /*Binding ContentTemplateBinding = new Binding("CollapsedHeaderTemplate") { Source = _item };
//      BindingOperations.SetBinding(this, ContentTemplateProperty, ContentTemplateBinding);

//      Binding ContentTemplateSelectorBinding = new Binding("CollapsedHeaderTemplateSelector") { Source = _item };
//      BindingOperations.SetBinding(this, ContentTemplateSelectorProperty, ContentTemplateSelectorBinding);*/
//    }

//    #region IsSelected Property

//    /// <summary>
//    /// Gets or sets whether or not the item is selected.
//    /// This is a dependency property.
//    /// </summary>
//    /// <remarks>
//    /// <strong>Dependency Property Information</strong>
//    /// <table>
//    ///   <tr><td>Identifier field</td><td><see cref="IsSelectedProperty"/></td></tr>
//    ///   <tr><td>Metadata properties set to true</td><td>None</td></tr>
//    /// </table>
//    /// </remarks>
//    public bool IsSelected
//    {
//      get { return (bool)GetValue(IsSelectedProperty); }
//      set { SetValue(IsSelectedProperty, value); }
//    }

//    /// <summary>
//    /// Identifies the <see cref="IsSelected"/> property.
//    /// </summary>
//    public static readonly DependencyProperty IsSelectedProperty =
//      DependencyProperty.Register("IsSelected", typeof(bool), typeof(CollapsedOutlookBarItem),
//      new FrameworkPropertyMetadata(OnIsSelectedChanged));

//    private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
//    {
//      ((CollapsedOutlookBarItem)d).OnIsSelectedChanged(e);
//    }

//    private void OnIsSelectedChanged(DependencyPropertyChangedEventArgs e)
//    {
//    }

//    #endregion // IsSelected Property
//  }
//}
