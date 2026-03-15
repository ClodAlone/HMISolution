using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace UFRecipeLayout.LayoutItemControls
{
    public interface IDataValueUI
    {
        void SetBinding(string path, UFUAModel.DataType valueType, IValueConverter valueConverter = null);
        void ClearBinding();
        void SetValue(string value);
        void SetEnumOptions(IEnumerable value);
        object Value { get; set; }
        string UnitName { get; set; }
        decimal MinValue { get; set; }
        decimal MaxValue { get; set; }
        int DecimalDigits { get; set; }
        int MaxLength { get; set; }
        bool AllowEdit { get; set; }
        Dictionary<string, List<Tuple<string, string>>> MapWrongValues { get; set; }
    }
}
