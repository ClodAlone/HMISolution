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
using DocumentManager.ComponentService;
using Utilities;

namespace UFRecipeEditor.CsvHelper
{
    internal class UFDataValueEntityMap : UFBaseEntityMap<UFDataValueEntity>
    {
        public UFDataValueEntityMap()
        {
            Map(m => m.DataValueName).Name(String.Format("{0}DataValueName", ClassMapConstant.HeaderStartingText));
            Map(m => m.Description);
            Map(m => m.DataType);
            Map(m => m.ArrayDimension);
            Map(m => m.DefaultValue);
            Map(m => m.InheritDataTypeFromTag).Optional().Default(true);
            Map(m => m.AllowNull);
            Map(m => m.UseInCommunication);
            Map(m => m.StartingAddress).Name("DeviceStartingAddress");
            Map(m => m.Encoding).Name("DeviceStringEncondig");
            Map(m => m.BytesStringValueSize).Name("DeviceStringSize");
            Map(m => m.TagDataValue).TypeConverter<TagConverter>();
            Map(m => m.TagIODataValue).TypeConverter<TagConverter>();
            Map(m => m.UFGroupAss).TypeConverter<GroupConverter>().Name("ParentGroupName");
            Map(m => m.EditControlType);
            Map(m => m.EnumOptions).TypeConverter<StringArrayConverter>();
            Map(m => m.EngineeringUnit);
            Map(m => m.UnitName);
            Map(m => m.MinValue);
            Map(m => m.MaxValue);
            Map(m => m.MaxLength);
            Map(m => m.DecimalDigits);
        }

        public void Merge(UFDataValueEntity csvEntity, UFDataValueEntity entity)
        {
            if (csvEntity.Name != entity.Name)
                throw new InvalidOperationException(String.Format(Properties.Resources.CsvImportDataValueNameNotMatch, csvEntity.Name));

            Validate(csvEntity);

            entity.Description = csvEntity.Description;
            entity.DataType = csvEntity.DataType;
            entity.ArrayDimension = csvEntity.ArrayDimension;
            entity.DefaultValue = csvEntity.DefaultValue;           
            entity.AllowNull = csvEntity.AllowNull;
            entity.UseInCommunication = csvEntity.UseInCommunication;
            entity.StartingAddress = csvEntity.StartingAddress;
            entity.Encoding = csvEntity.Encoding;
            entity.BytesStringValueSize = csvEntity.BytesStringValueSize;
            entity.EditControlType = csvEntity.EditControlType;
            entity.EnumOptions = csvEntity.EnumOptions;
            entity.EngineeringUnit= csvEntity.EngineeringUnit;
            entity.UnitName = csvEntity.UnitName;
            entity.MinValue = csvEntity.MinValue;
            entity.MaxValue = csvEntity.MaxValue;
            entity.MaxLength = csvEntity.MaxLength;
            entity.DecimalDigits = csvEntity.DecimalDigits;
            entity.TagDataValue = csvEntity.TagDataValue;
            entity.TagIODataValue = csvEntity.TagIODataValue;
            entity.InheritDataTypeFromTag = csvEntity.InheritDataTypeFromTag;
        }

        internal UFDataValueEntity TryToBuildTagDataValues(UFDataValueEntity csvEntity, UFDataValueEntity entity, bool useCache)
        {
            try
            {
                IDocument document = null;
                if (entity.UFRecipeAss != null)
                    document = entity.UFRecipeAss.Document;
                else if (entity.UFGroupAss != null && entity.UFGroupAss.UFRecipeAss != null)
                    document = entity.UFGroupAss.UFRecipeAss.Document;

                if (document != null)
                {
                    if (csvEntity.TagDataValue != null)
                    {
                        var tag = GetTagEntityReference(document, csvEntity.TagDataValue, useCache);
                        if (tag != null)
                            csvEntity.TagDataValue = tag.FromXml<OPCUAEntityReference>();
                    }
                    if (csvEntity.TagIODataValue != null)
                    {
                        var tag = GetTagEntityReference(document, csvEntity.TagIODataValue, useCache);
                        if (tag != null)
                            csvEntity.TagIODataValue = tag.FromXml<OPCUAEntityReference>();
                    }
                }
            }
            catch { }

            return csvEntity;
        }
    }
}
