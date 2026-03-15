using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using OPCUAViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using UFRecipeSettings.UFRecipeModel;
using Utilities;
using Utilities.WPF;

namespace UFRecipeEditor.CsvHelper
{
    internal class UFRecipeEntityMap : UFBaseEntityMap<UFRecipeEntity>
    {
        public UFRecipeEntityMap()
        {
            Map(m => m.RecipeName).Name(String.Format("{0}RecipeName", ClassMapConstant.HeaderStartingText));
            Map(m => m.Description);
            Map(m => m.TableName);
            Map(m => m.ReadableConnectionString).Name("AdoDotNetConnection");
            Map(m => m.MaxLength).Name("AdoDotNetMaxStringSize");
            Map(m => m.SkipCheckColumnsType).Name("AdoDotNetSkipColumnsTypeCheck");
            Map(m => m.StartingAddress).Name("DeviceStartingAddress");
            Map(m => m.TagRecipeList).TypeConverter<TagConverter>();
            Map(m => m.TagRecipeIndex).TypeConverter<TagConverter>();
            Map(m => m.TagRecipeState).TypeConverter<TagConverter>();
            Map(m => m.TagRecipeLoad).TypeConverter<TagConverter>();
            Map(m => m.TagRecipeSave).TypeConverter<TagConverter>();
            Map(m => m.TagRecipeDelete).TypeConverter<TagConverter>();
            Map(m => m.TagRecipeWrite).TypeConverter<TagConverter>();
            Map(m => m.TagRecipeRead).TypeConverter<TagConverter>();
            Map(m => m.AuditTraceEnabled);
            Map(m => m.EnterCommentOnAudit);
            Map(m => m.EnterPasswordOnAudit);
            Map(m => m.MinAccessLevelRequiredOnAudit);
            Map(m => m.UserAccessLevel);
        }

        public void Merge(UFRecipeEntity csvEntity, UFRecipeEntity entity)
        {
            if (csvEntity.Name != entity.Name)
                throw new InvalidOperationException(String.Format(Properties.Resources.CsvImportRecipeNameNotMatch, csvEntity.Name));

            Validate(csvEntity);

            entity.Description = csvEntity.Description;
            entity.TableName = csvEntity.TableName;
            entity.ReadableConnectionString = csvEntity.ReadableConnectionString;
            entity.StartingAddress = csvEntity.StartingAddress;
            entity.MaxLength = csvEntity.MaxLength;
            entity.SkipCheckColumnsType = csvEntity.SkipCheckColumnsType;
            entity.AuditTraceEnabled = csvEntity.AuditTraceEnabled;
            entity.EnterCommentOnAudit = csvEntity.EnterCommentOnAudit;
            entity.EnterPasswordOnAudit = csvEntity.EnterPasswordOnAudit;
            entity.MinAccessLevelRequiredOnAudit = csvEntity.MinAccessLevelRequiredOnAudit;
            entity.UserAccessLevel = csvEntity.UserAccessLevel;

            if (entity.Document != null)
            {
                if (csvEntity.TagRecipeList != null)
                {
                    var tag = GetTagEntityReference(entity.Document, csvEntity.TagRecipeList);
                    if (tag != null)
                        entity.TagRecipeList = tag.FromXml<OPCUAEntityReference>();
                }
                if (csvEntity.TagRecipeIndex != null)
                {
                    var tag = GetTagEntityReference(entity.Document, csvEntity.TagRecipeIndex);
                    if (tag != null)
                        entity.TagRecipeIndex = tag.FromXml<OPCUAEntityReference>();
                }
                if (csvEntity.TagRecipeState != null)
                {
                    var tag = GetTagEntityReference(entity.Document, csvEntity.TagRecipeState);
                    if (tag != null)
                        entity.TagRecipeState = tag.FromXml<OPCUAEntityReference>();
                }
                if (csvEntity.TagRecipeLoad != null)
                {
                    var tag = GetTagEntityReference(entity.Document, csvEntity.TagRecipeLoad);
                    if (tag != null)
                        entity.TagRecipeLoad = tag.FromXml<OPCUAEntityReference>();
                }
                if (csvEntity.TagRecipeSave != null)
                {
                    var tag = GetTagEntityReference(entity.Document, csvEntity.TagRecipeSave);
                    if (tag != null)
                        entity.TagRecipeSave = tag.FromXml<OPCUAEntityReference>();
                }
                if (csvEntity.TagRecipeDelete != null)
                {
                    var tag = GetTagEntityReference(entity.Document, csvEntity.TagRecipeDelete);
                    if (tag != null)
                        entity.TagRecipeDelete = tag.FromXml<OPCUAEntityReference>();
                }
                if (csvEntity.TagRecipeWrite != null)
                {
                    var tag = GetTagEntityReference(entity.Document, csvEntity.TagRecipeWrite);
                    if (tag != null)
                        entity.TagRecipeWrite = tag.FromXml<OPCUAEntityReference>();
                }
                if (csvEntity.TagRecipeRead != null)
                {
                    var tag = GetTagEntityReference(entity.Document, csvEntity.TagRecipeRead);
                    if (tag != null)
                        entity.TagRecipeRead = tag.FromXml<OPCUAEntityReference>();
                }
            }
        }
    }
}
