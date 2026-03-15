using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataValidation
{
    public class Column
    {
        public string HeaderName { get; set; }
        public string FieldName { get; set; }
        public string BindingExpression => $"DataRow[{FieldName}]";
    }
}
