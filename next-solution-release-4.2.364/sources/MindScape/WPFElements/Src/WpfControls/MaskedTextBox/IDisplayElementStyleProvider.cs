using System.Windows;

namespace Mindscape.WpfElements
{
  internal interface IDisplayElementStyleProvider
  {
    Style InputStyle { get; }
    Style LiteralStyle { get; }
    Style PromptStyle { get; }
    IPromptCharDisplaySelector PromptCharDisplaySelector { get; }
  }
}
