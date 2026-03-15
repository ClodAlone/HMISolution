using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using DocumentManager.ComponentService;
using UFInterfaces;
using Utilities.Converters;

namespace UnitConverterManager.ComponentService
{
    public interface IUnitConverterEditorManager : IUFInterfaceBase
    {
        IEnumerable<String> GetListAvailableConverters(IDocument parent);
        IDictionary<String, String> GetListIDsForCulture(IDocument parent, String culture);
        IEnumerable<String> GetListUnitConverterIDs(IDocument parent);

        bool AddListUnitConverterId(IDocument parent, IList<String> list);

        String GetActiveConverter(IDocument parent);
        event EventHandler CurrentConverterChanged;

        String GetUnitLabel(IDocument parent, string id, string converter);
        String GetInputExpression(IDocument parent, string id, string converter);
        String GetOutputExpression(IDocument parent, string id, string converter);

        String GetConnectionStringFromFile(String file);
        UserControl GetUnitConverterEditor(IDocument parent);
    }
}
