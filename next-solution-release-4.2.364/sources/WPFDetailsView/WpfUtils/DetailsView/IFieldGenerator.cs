using System.Collections.Generic;

namespace Monogram.WpfUtils
{
  public interface IFieldGenerator
  {
    IEnumerable<DetailsViewRow> GenerateFields(object dataItem, DetailsView container);
  }
}
