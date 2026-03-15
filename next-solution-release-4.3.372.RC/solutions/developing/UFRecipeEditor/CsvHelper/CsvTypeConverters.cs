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

namespace UFRecipeEditor.CsvHelper
{
    internal class ClassMapConstant
    {
        public static string HeaderStartingText = "//";
        public static char ArraySeparatorChar = '&';
    }

    internal class TagConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            return new OPCUAEntityReference() { HumanReadable = text };
        }

        public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            var member = value as OPCUAEntityReference;
            if (member == null)
                return String.Empty;

            return member.StringRepresentation;
        }
    }

    internal class GroupConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            return new UFGroupEntity() { GroupName = text };
        }

        public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            var member = value as UFGroupEntity;
            if (member == null)
                return String.Empty;

            return member.GroupName;
        }
    }

    internal class StringArrayConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (String.IsNullOrEmpty(text))
                return null;

            return text.Split(ClassMapConstant.ArraySeparatorChar);
        }

        public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            var member = value as string[];
            if (member == null)
                return String.Empty;

            return String.Join(ClassMapConstant.ArraySeparatorChar.ToString(), member);
        }
    }
}
