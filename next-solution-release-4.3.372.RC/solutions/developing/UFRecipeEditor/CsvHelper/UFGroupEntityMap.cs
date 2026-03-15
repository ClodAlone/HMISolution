using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using OPCUAViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFRecipeSettings.UFRecipeModel;
using Utilities;

namespace UFRecipeEditor.CsvHelper
{
    internal class UFGroupEntityMap : UFBaseEntityMap<UFGroupEntity>
    {
        public UFGroupEntityMap()
        {
            Map(m => m.GroupName).Name(String.Format("{0}GroupName", ClassMapConstant.HeaderStartingText));
            Map(m => m.Description);
            Map(m => m.StartingAddress).Name("DeviceStartingAddress");
        }

        public void Merge(UFGroupEntity csvEntity, UFGroupEntity entity)
        {
            if (csvEntity.Name != entity.Name)
                throw new InvalidOperationException(String.Format(Properties.Resources.CsvImportGroupNameNotMatch, csvEntity.Name));

            Validate(csvEntity);

            entity.Description = csvEntity.Description;
            entity.StartingAddress = csvEntity.StartingAddress;
        }
    }
}
