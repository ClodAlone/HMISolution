using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Markup;
using System.ComponentModel;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Windows.Data;
using System.Windows.Media;

namespace Monogram.WpfUtils
{
  /// <summary>
  /// Interaction logic for DetailsViewxaml.xaml
  /// </summary>
  [ContentProperty("Fields")]
  public class DetailsView : Control
  {
    #region constants

    public const string LAYOUT_GRID_NAME = "PART_LayoutGrid";
    public const string HEADER_TEMPALATE = "DV_HeaderTemplate";
    public const string DETAILSVIEW_TEMPALATE_KEY = "DV_Template";
    
    #endregion

    #region public properties

    public List<DetailsViewRow> Fields
    {
      get { return (List<DetailsViewRow>)GetValue(FieldsProperty); }
      set { SetValue(FieldsProperty, value); }
    }
    public static readonly DependencyProperty FieldsProperty = DependencyProperty.Register("Fields", typeof(List<DetailsViewRow>), typeof(DetailsView));

    public bool AutoGenerateFields
    {
      get { return (bool)GetValue(AutoGenerateFieldsProperty); }
      set { SetValue(AutoGenerateFieldsProperty, value); }
    }
    public static readonly DependencyProperty AutoGenerateFieldsProperty = DependencyProperty.Register("AutoGenerateFields", typeof(bool), typeof(DetailsView), new PropertyMetadata { DefaultValue = true });

    public FieldGenerator FieldGenerator
    {
      get { return (FieldGenerator)GetValue(FieldGeneratorProperty); }
      set { SetValue(FieldGeneratorProperty, value); }
    }
    public static readonly DependencyProperty FieldGeneratorProperty = DependencyProperty.Register("FieldGenerator", typeof(FieldGenerator), typeof(DetailsView), new PropertyMetadata(new FieldGenerator()));


    public bool AllowEdit
    {
      get { return (bool)GetValue(AllowEditProperty); }
      set { SetValue(AllowEditProperty, value); }
    }
    public static readonly DependencyProperty AllowEditProperty = DependencyProperty.Register("AllowEdit", typeof(bool), typeof(DetailsView), new PropertyMetadata
    {
      PropertyChangedCallback = (sender, e) =>
      {
        ((DetailsView)sender).AllowEditChanged(sender, e);
      }
    });

    public DataTemplateSelector CellTemplateSelector
    {
      get { return (DataTemplateSelector)GetValue(CellTemplateSelectorProperty); }
      set { SetValue(CellTemplateSelectorProperty, value); }
    }
    public static readonly DependencyProperty CellTemplateSelectorProperty = DependencyProperty.Register("CellTemplateSelector", typeof(DataTemplateSelector), typeof(DetailsView), new UIPropertyMetadata(DetailsViewCellTemplateSelector.Instance));

    public DataTemplate HeaderTemplate
    {
      get
      {
        if (_HeaderTemplate == null) _HeaderTemplate = (DataTemplate)TryFindResource(HEADER_TEMPALATE);
        return _HeaderTemplate;
      }
      set { _HeaderTemplate = value; }
    }
    private DataTemplate _HeaderTemplate;


    public static DependencyProperty GetBindProperty(DependencyObject obj)
    {
      return (DependencyProperty)obj.GetValue(BindPropertyProperty);
    }
    public static void SetBindProperty(DependencyObject obj, DependencyProperty value)
    {
      obj.SetValue(BindPropertyProperty, value);
    }
    public static readonly DependencyProperty BindPropertyProperty = DependencyProperty.RegisterAttached("BindProperty", typeof(DependencyProperty), typeof(DetailsView), new UIPropertyMetadata(FrameworkElement.DataContextProperty));

    #endregion

    #region private fields

    private List<DetailsViewRow> allFields = new List<DetailsViewRow>();

    private Grid LayoutGrid;

    #endregion


    static DetailsView()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(DetailsView),
       new FrameworkPropertyMetadata(typeof(DetailsView)));
    }

    public DetailsView()
    {
      Fields = new List<DetailsViewRow>();


      //if (WpfHelper.IsDesignMode) return;

      DataContextChanged += new DependencyPropertyChangedEventHandler(DetailsView_DataContextChanged);
    }


    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      //if (!WpfHelper.IsDesignMode)
      {
        this.LayoutGrid = (Grid)WpfHelper.FindVisualChild(this, e => e.Name == LAYOUT_GRID_NAME, -1);
        createGrid();
      }
    }

    void DetailsView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (LayoutGrid != null)
        createGrid();
    }

    void AllowEditChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (LayoutGrid != null && DataContext != null)
        createGrid();
    }

    private void createGrid()
    {


      LayoutGrid.Children.Clear();
      LayoutGrid.RowDefinitions.Clear();
      allFields.Clear();


      if (AutoGenerateFields == true && DataContext != null)
      {
        //generate fields from datacontext object
        allFields.AddRange(this.FieldGenerator.GenerateFields(DataContext, this));
      }




      //add items defined in Fields collection (contentproperty in xaml) if any
      allFields.AddRange(Fields);
      allFields = allFields.OrderBy(f => f.OrderNumber).ToList();

      FrameworkElement visualTreeHack = new FrameworkElement();
      LayoutGrid.Children.Add(visualTreeHack);

      int row = 0;
      foreach (DetailsViewRow field in allFields)
      {
        LayoutGrid.RowDefinitions.Add(new RowDefinition() { Height = field.Height });

        //create header
        var headerTemplate = field.HeaderTemplate != null ? field.HeaderTemplate : this.HeaderTemplate;
        var header = (FrameworkElement)headerTemplate.LoadContent();
        if (field.HeaderContainerStyle != null) header.Style = field.HeaderContainerStyle;
        header.DataContext = field;

        Grid.SetColumn(header, 0);
        Grid.SetRow(header, row);
        LayoutGrid.Children.Add(header);



        if (CellTemplateSelector is DetailsViewCellTemplateSelector)
        {
          ((DetailsViewCellTemplateSelector)CellTemplateSelector).VisualTreeHack = visualTreeHack;
        }

        //create celll
        var cellTemplate = CellTemplateSelector.SelectTemplate(field, this);
        if (cellTemplate != null)
        {
          var cell = (FrameworkElement)(cellTemplate).LoadContent();
          if (field.CellContainerStyle != null) cell.Style = field.CellContainerStyle;


          if (DataContext != null && field.DisplayMemberBinding != null)
          {
            var contentElement = cell.FindLogicalChild<FrameworkElement>("PART_Content");
            if (contentElement == null) contentElement = cell;
            var prop = GetBindProperty(contentElement);
            cell.SetBinding(prop, field.DisplayMemberBinding);
          }

          Grid.SetColumn(cell, 1);
          Grid.SetRow(cell, row);
          LayoutGrid.Children.Add(cell);
        }
        row++;
      }

      LayoutGrid.Children.Remove(visualTreeHack);
    }
  }
}
