using System;
using Microsoft.Windows.Design.Metadata;
using System.Reflection;
using System.Windows;
using Microsoft.Windows.Design;

#if VS2010
[assembly: ProvideMetadata(typeof(Mindscape.WpfElements.Design.MetadataRegistrar))]
#endif

namespace Mindscape.WpfElements.Design
{
#if VS2010
  internal class MetadataRegistrar : IProvideAttributeTable
#else
  internal class MetadataRegistrar : IRegisterMetadata
#endif
  {
    private static readonly Assembly TargetAssembly = typeof(MaskedTextBox).Assembly;

    private static readonly ToolboxEntry[] ToolboxControls = new ToolboxEntry[]
    {
      ToolboxEntries.AutoCompleteBoxToolboxEntry,
      ToolboxEntries.ChannelColorPickerToolboxEntry,
      ToolboxEntries.ChartToolboxEntry,
      ToolboxEntries.ColorPickerToolboxEntry,
      ToolboxEntries.CoverFlowToolboxEntry,
      ToolboxEntries.CurrencyTextBoxToolboxEntry,
      ToolboxEntries.DataGridToolboxEntry,
      ToolboxEntries.DateTimePickerToolboxEntry,
      ToolboxEntries.DropDownColorPickerToolboxEntry,
      ToolboxEntries.DropDownDateTimePickerToolboxEntry,
      ToolboxEntries.DropDownToolboxEntry,
      ToolboxEntries.DualProgressBarToolboxEntry,
      ToolboxEntries.DualSliderToolboxEntry,
      ToolboxEntries.HsvColorPickerToolboxEntry,
      ToolboxEntries.IntegerTextBoxToolboxEntry,
      ToolboxEntries.MaskedTextBoxToolboxEntry,
      ToolboxEntries.MonthCalendarToolboxEntry,
      ToolboxEntries.MulticolumnTreeViewToolboxEntry,
      ToolboxEntries.NumericTextBoxToolboxEntry,
      ToolboxEntries.OutlookBarToolboxEntry,
      ToolboxEntries.PieChartToolboxEntry,
      ToolboxEntries.PolarChartToolboxEntry,
      ToolboxEntries.PromptDecoratorToolboxEntry,
      ToolboxEntries.PropertyGridToolboxEntry,
      ToolboxEntries.ProportionalStackPanelToolboxEntry,
      ToolboxEntries.RibbonToolboxEntry,
      ToolboxEntries.RichTextToolBarToolboxEntry,
      ToolboxEntries.SchedulerToolboxEntry,
      ToolboxEntries.SpinDecoratorToolboxEntry,
      ToolboxEntries.SpinToolboxEntry,
      ToolboxEntries.SplitButtonToolboxEntry,
      ToolboxEntries.TimePickerToolboxEntry,
      ToolboxEntries.TimeSpanPickerToolboxEntry,
    };

    private AttributeTableBuilder _builder = new AttributeTableBuilder();

    private void InitialiseBuilder()
    {
      DefaultAllComponentTypesToNonToolbox();

      foreach (ToolboxEntry toolboxEntry in ToolboxControls)
      {
        toolboxEntry.AddTo(_builder);
      }
    }

#if VS2010
    public AttributeTable AttributeTable
    {
      get
      {
        InitialiseBuilder();
        return _builder.CreateTable();
      }
    }
#else
    public void Register()
    {
      InitialiseBuilder();
      MetadataStore.AddAttributeTable(_builder.CreateTable());
    }
#endif

    private void DefaultAllComponentTypesToNonToolbox()
    {
      foreach (Type exportedType in TargetAssembly.GetExportedTypes())
      {
        if (typeof(UIElement).IsAssignableFrom(exportedType) && !IsToolboxControlType(exportedType))
        {
          _builder.AddCustomAttributes(exportedType, new ToolboxBrowsableAttribute(false));
        }
      }
    }

    private static bool IsToolboxControlType(Type exportedType)
    {
      foreach (ToolboxEntry toolboxEntry in ToolboxControls)
      {
        if (toolboxEntry.ComponentType == exportedType)
        {
          return true;
        }
      }

      return false;
    }
  }
}
