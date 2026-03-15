using System.Globalization;

namespace Mindscape.WpfElements
{
  internal interface IFilteringTextBoxModel
  {
    CultureInfo Culture { get; set; }
    string Text { get; set; }
  }
}
