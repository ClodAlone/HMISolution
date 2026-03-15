using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace Monogram.WpfUtils
{
  [ContentProperty("Header")]
  public class DetailsViewRow : DependencyObject
  {
    public Binding DisplayMemberBinding
    {
      get;
      set;
    }


    public Style CellContainerStyle
    {
      get { return (Style)GetValue(CellContainerStyleProperty); }
      set { SetValue(CellContainerStyleProperty, value); }
    }
    public static readonly DependencyProperty CellContainerStyleProperty = DependencyProperty.Register("CellContainerStyle", typeof(Style), typeof(DetailsViewRow));


    public DataTemplate CellTemplate
    {
      get { return (DataTemplate)GetValue(CellTemplateProperty); }
      set { SetValue(CellTemplateProperty, value); }
    }
    public static readonly DependencyProperty CellTemplateProperty = DependencyProperty.Register("CellTemplate", typeof(DataTemplate), typeof(DetailsViewRow));


    public DataTemplateSelector CellTemplateSelector
    {
      get { return (DataTemplateSelector)GetValue(CellTemplateSelectorProperty); }
      set { SetValue(CellTemplateSelectorProperty, value); }
    }
    public static readonly DependencyProperty CellTemplateSelectorProperty = DependencyProperty.Register("CellTemplateSelector", typeof(DataTemplateSelector), typeof(DetailsViewRow));



    public Style HeaderContainerStyle
    {
      get { return (Style)GetValue(HeaderContainerStyleProperty); }
      set { SetValue(HeaderContainerStyleProperty, value); }
    }
    public static readonly DependencyProperty HeaderContainerStyleProperty = DependencyProperty.Register("HeaderContainerStyle", typeof(Style), typeof(DetailsViewRow), new UIPropertyMetadata(null));


    public object Header
    {
      get { return (object)GetValue(HeaderProperty); }
      set { SetValue(HeaderProperty, value); }
    }
    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(DetailsViewRow), new UIPropertyMetadata(null));



    public int OrderNumber
    {
      get { return (int)GetValue(OrderNumberProperty); }
      set { SetValue(OrderNumberProperty, value); }
    }
    public static readonly DependencyProperty OrderNumberProperty = DependencyProperty.Register("OrderNumber", typeof(int), typeof(DetailsViewRow), new UIPropertyMetadata(0));



    public GridLength Height
    {
      get { return (GridLength)GetValue(HeightProperty); }
      set { SetValue(HeightProperty, value); }
    }
    public static readonly DependencyProperty HeightProperty = DependencyProperty.Register("Height", typeof(GridLength), typeof(DetailsViewRow));




    public DataTemplate HeaderTemplate
    {
      get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
      set { SetValue(HeaderTemplateProperty, value); }
    }
    public static readonly DependencyProperty HeaderTemplateProperty =  DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(DetailsViewRow), new UIPropertyMetadata(null));


    public override string ToString()
    {
      if (Header is string) return Header.ToString();
      else if (DisplayMemberBinding != null && DisplayMemberBinding.Path != null) return DisplayMemberBinding.Path.ToString();
      return base.ToString();
    }
  }
}
