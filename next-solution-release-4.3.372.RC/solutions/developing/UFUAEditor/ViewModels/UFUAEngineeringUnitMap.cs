using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFUAModel;
using Utilities;

namespace UFUAEditor.ViewModels
{
#if !NET_STANDARD
    public class UFUAEngineeringUnitMap: ClassMap<ImportEngineeringUnit>
    {
        public const string UNECECodeName = "UNECECode";
        public const string UnitNameName = "DisplayName";

        public UFUAEngineeringUnitMap()
        {
            Map(x => x.UNECECode).Name(UNECECodeName);
            Map(x => x.Description);
            Map(x => x.UnitId);
            Map(x => x.UnitName).Name(UnitNameName);
        }
    }
#endif
}
