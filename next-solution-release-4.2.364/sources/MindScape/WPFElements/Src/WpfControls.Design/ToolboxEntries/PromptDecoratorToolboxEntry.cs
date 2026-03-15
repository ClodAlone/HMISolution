using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Mindscape.WpfElements.Design
{
  internal partial class ToolboxEntries
  {
    internal static readonly ToolboxEntry PromptDecoratorToolboxEntry = new ToolboxEntry(

      typeof(PromptDecorator),
      "Adds a prompt overlay to a control such as a TextBox",

      new DependencyProperty[] {
        PromptDecorator.IsPromptVisibleProperty,
      },

      new PropertyCategory[] {
        new PropertyCategory("Behavior", new DependencyProperty[] {
          PromptDecorator.ChildContentPropertyNameProperty,
        }),
        new PropertyCategory("Appearance", new DependencyProperty[] {
          PromptDecorator.PromptProperty,
          PromptDecorator.PromptTemplateProperty,
          PromptDecorator.ShowIfFocusedProperty,
        }),
      }

      );
  }
}
