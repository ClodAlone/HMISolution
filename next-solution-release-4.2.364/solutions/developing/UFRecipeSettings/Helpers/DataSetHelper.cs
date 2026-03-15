using System;
using System.Collections.Generic;
using Opc.Ua;
using UFRecipeSettings.UFRecipeModel;
using UFUAModel.Extensions;
using System.Data;
using System.Text.RegularExpressions;
using UFProjectManager.ComponentService;
using Utilities;
using UFRecipeEditor.ComponentService;
using DocumentManager.ComponentService;

namespace UFRecipeSettings.Helpers
{
    public sealed class DataSetHelper
    {
        const String TableNameMacth = @"^[\p{L}0-9_\s]+$";

        public static String TableName(object obj)
        {
            if (obj is UFRecipeModel.UFRecipeEntity)
                return TableName(obj as UFRecipeModel.UFRecipeEntity);
            else if (obj is UFRecipeModel.UFGroupEntity)
                return TableName(obj as UFRecipeModel.UFGroupEntity);
            else if (obj is UFRecipeModel.UFDataValueEntity)
            {
                if ((obj as UFRecipeModel.UFDataValueEntity).UFRecipeAss != null)
                    return TableName((obj as UFRecipeModel.UFDataValueEntity).UFRecipeAss);
                else
                    return TableName((obj as UFRecipeModel.UFDataValueEntity).UFGroupAss);
            }
            else
                throw new ArgumentException(String.Format("Invalid input argument '{0}' !", obj.GetType()));
        }

        public static String PrimaryKeyName(object obj)
        {
            if (obj is UFRecipeModel.UFRecipeEntity)
                return PrimaryKeyName(obj as UFRecipeModel.UFRecipeEntity);
            else if (obj is UFRecipeModel.UFGroupEntity)
                return PrimaryKeyName(obj as UFRecipeModel.UFGroupEntity);
            else
                throw new ArgumentException(String.Format("Invalid input argument '{0}' !", obj.GetType()));
        }

        public static Type PrimaryKeyType(object obj)
        {
            if (obj is UFRecipeModel.UFRecipeEntity)
                return PrimaryKeyType(obj as UFRecipeModel.UFRecipeEntity);
            else if (obj is UFRecipeModel.UFGroupEntity)
                return PrimaryKeyType(obj as UFRecipeModel.UFGroupEntity);
            else
                throw new ArgumentException(String.Format("Invalid input argument '{0}' !", obj.GetType()));
        }

        public static String TableName(UFRecipeModel.UFRecipeEntity recipe)
        {
            if (!String.IsNullOrWhiteSpace(recipe.TableName) && IsValidTableName(recipe.TableName))
                return recipe.TableName;

#if !NET_STANDARD
            bool bUseFolder = false;
            var UFProjectManager = recipe.Document?.GetService(typeof(IUFProjectManager)) as IUFProjectManager;
            if (UFProjectManager != null)
            {
                Version version;
                var projectVersion = UFProjectManager.GetProjectVersion(recipe.Document);
                if (Version.TryParse(projectVersion, out version))
                {
                    try
                    {
                        var checkVersion = new Version(Properties.Settings.Default.UseUniqueRecipeNameVersion);
                        bUseFolder = version >= checkVersion;
                    }
                    catch
                    { }
                }
            }
            if (bUseFolder)
#endif
            {
                IDocumentManager recipeEditor = recipe.Document?.GetService(typeof(IRecipeEditorManager)) as IDocumentManager;
                if (recipeEditor != null)
                {
                    var search = String.Format("{0}{1}{0}", System.IO.Path.DirectorySeparatorChar, recipeEditor.TypeLabel);
                    var index = recipe.Document.Folder.LastIndexOf(search);
                    if (index != -1)
                    {
                        var folder = recipe.Document.Folder.Substring(index + search.Length);
                        if (!String.IsNullOrEmpty(folder))
                            return String.Format("{0}_{1}",
                                folder.Replace(System.IO.Path.DirectorySeparatorChar, '_'),
                                recipe.RecipeName);
                    }
                }
            }
            
            return recipe.RecipeName;
        }

        public static bool IsValidTableName(string name)
        {
            if (String.IsNullOrEmpty(name))
                return false;

            return Regex.IsMatch(name, TableNameMacth);
        }

        public static String PrimaryKeyName(UFRecipeModel.UFRecipeEntity recipe)
        {
            return String.Format("{0}_ID", recipe.RecipeName);
        }

        public static Type PrimaryKeyType(UFRecipeModel.UFRecipeEntity recipe)
        {
            return typeof(Guid);
        }

        public static String CreationDateTimeColumnName(UFRecipeModel.UFRecipeEntity recipe)
        {
            return String.Format("{0}_CreationDateTime", recipe.RecipeName);
        }

        public static String ActivationDateTimeColumnName(UFRecipeModel.UFRecipeEntity recipe)
        {
            return String.Format("{0}_ActivationDateTime", recipe.RecipeName);
        }

        public static String TableName(UFRecipeModel.UFGroupEntity group)
        {
            if (group.UFRecipeAss == null)
                throw new InvalidOperationException(String.Format("Invalid state for Group '{0}' !", group.Name));

            return String.Format("{0}_{1}", TableName(group.UFRecipeAss), group.GroupName);
        }

        public static String PrimaryKeyName(UFRecipeModel.UFGroupEntity group)
        {
            if (group.UFRecipeAss == null)
                throw new InvalidOperationException(String.Format("Invalid state for Group '{0}' !", group.Name));

            return PrimaryKeyName(group.UFRecipeAss);
        }

        public static Type PrimaryKeyType(UFRecipeModel.UFGroupEntity group)
        {
            if (group.UFRecipeAss == null)
                throw new InvalidOperationException(String.Format("Invalid state for Group '{0}' !", group.Name));

            return PrimaryKeyType(group.UFRecipeAss);
        }

        public static String ColumnName(UFRecipeModel.UFRecipeEntity value)
        {
            return value.RecipeName;
        }

        public static String ColumnName(UFRecipeModel.UFDataValueEntity value)
        {
            return value.DataValueName;
        }

        public static VariantCollection GetDBValues(IList<UFDataValueEntity> writabledatavalues, DataSet ds, Guid guid, bool bEncodingString = true)
        {
            DataView view = new DataView();
            var values = new VariantCollection();

            foreach (var datavalue in writabledatavalues)
            {
                var tablename = DataSetHelper.TableName(datavalue);
                if (view.Table == null || view.Table.TableName != tablename)
                {
                    view.Table = ds.Tables[tablename];
                    view.RowFilter = String.Format("[{0}]='{1}'", view.Table.PrimaryKey[0].ColumnName, guid);
                }

                object value = view.Count > 0 ? view[0].Row[DataSetHelper.ColumnName(datavalue)] : datavalue.DefaultValue;
                if (bEncodingString && datavalue.DataType == UFUAModel.DataType.String)
                {
                    if (datavalue.BytesStringValueSize > 0)
                    {
                        // special case for String who should be encoding from String to String
                        if (datavalue.ArrayDimension == 0)
                        {
                            var sval = value as String;
                            if (sval == null)
                                sval = string.Empty;
                            values.Add(new Variant(EncodingStringToBytesValue(sval, datavalue.Encoding, datavalue.BytesStringValueSize)));
                        }
                        else
                        {
                            var stringArray = DataTypeExtensions.ChangeType(value, datavalue.DataType, (int)datavalue.ArrayDimension, true) as String[];
                            if (stringArray != null)
                            {
                                foreach (var stringValue in stringArray)
                                    values.Add(new Variant(EncodingStringToBytesValue(stringValue, datavalue.Encoding, datavalue.BytesStringValueSize)));
                            }
                        }
                    }
                }
                else if (bEncodingString && datavalue.DataType == UFUAModel.DataType.Byte && datavalue.ArrayDimension > 0)
                {
                    var byteArray = DataTypeExtensions.ChangeType(value, datavalue.DataType, (int)datavalue.ArrayDimension, true) as byte[];
                    if (byteArray != null)
                    {
                        foreach (var byteValue in byteArray)
                            values.Add(new Variant(byteValue));
                    }
                }
                else
                {
                    if(datavalue.DataType == UFUAModel.DataType.String)
                    {
                        var sVal = value as String;
                        if(sVal == null)
                            value = String.Empty;
                    }

                    values.Add(new Variant(DataTypeExtensions.ChangeType(value, datavalue.DataType, (int)datavalue.ArrayDimension, true)));
                }
                    
            }

            return values;
        }

        public static void SetDBValues(IList<UFDataValueEntity> writabledatavalues, DataSet ds, VariantCollection values, bool bEncodingString = true)
        {
            DataView view = new DataView();

            int counter = 0;
            foreach (var datavalue in writabledatavalues)
            {
                var tablename = DataSetHelper.TableName(datavalue);
                if (view.Table == null || view.Table.TableName != tablename)
                    view = ds.Tables[tablename].DefaultView;

                if (view.Count > 0 && values.Count > counter)
                {
                    if (bEncodingString && datavalue.DataType == UFUAModel.DataType.String)
                    {
                        if (datavalue.BytesStringValueSize > 0)
                        {
                            // special case for String who should be encoding from Byte[] to String
                            if (datavalue.ArrayDimension == 0)
                            {
                                view[0].Row[DataSetHelper.ColumnName(datavalue)] =
                                    EncodingBytesToStringValue(values[counter].Value as byte[], datavalue.Encoding);
                            }
                            else
                            {
                                var stringArray = new string[datavalue.ArrayDimension];
                                for (int ii = 0; ii < datavalue.ArrayDimension; ii++)
                                {
                                    if (values.Count > counter + ii)
                                        stringArray[ii] = EncodingBytesToStringValue(values[counter + ii].Value as byte[], datavalue.Encoding);
                                    else
                                        stringArray[ii] = datavalue.DefaultValue;
                                }

                                view[0].Row[DataSetHelper.ColumnName(datavalue)] = new Opc.Ua.Variant(stringArray).ToString(null, System.Globalization.CultureInfo.InvariantCulture);
                                counter += (int)datavalue.ArrayDimension;
                                continue;
                            }
                        }
                    }
                    else if (bEncodingString && datavalue.DataType == UFUAModel.DataType.Byte && datavalue.ArrayDimension > 0)
                    {
                        var byteArray = new byte[datavalue.ArrayDimension];
                        for (int ii = 0; ii < datavalue.ArrayDimension; ii++)
                        {
                            if (values.Count > counter + ii && values[counter + ii].Value is byte)
                                byteArray[ii] = (byte)values[counter + ii].Value;
                        }

                        view[0].Row[DataSetHelper.ColumnName(datavalue)] = new Opc.Ua.Variant(byteArray).ToString(null, System.Globalization.CultureInfo.InvariantCulture);
                        counter += (int)datavalue.ArrayDimension;
                        continue;
                    }
                    else if (datavalue.ArrayDimension > 0)
                        view[0].Row[DataSetHelper.ColumnName(datavalue)] = values[counter].ToString(null, System.Globalization.CultureInfo.InvariantCulture);
                    else
                        view[0].Row[DataSetHelper.ColumnName(datavalue)] = values[counter].Value ?? DBNull.Value;
                }
                else if (view.Count > 0)
                    view[0].Row[DataSetHelper.ColumnName(datavalue)] = datavalue.DefaultValue;

                counter++;
            }
        }

        public static byte[] EncodingStringToBytesValue(String value, StringEncodingType type, int size)
        {
            if (value.Length > size)
                value = value.Substring(0, size);
            else
                value = value.PadRight(size, '\0');
            switch (type)
            {
                case StringEncodingType.ASCII:
                    {
                        int bytesize = System.Text.Encoding.ASCII.GetByteCount(value);
                        byte[] result = new byte[bytesize];
                        System.Text.Encoding.ASCII.GetBytes(value, 0, size, result, 0);
                        return result;
                    }
                case StringEncodingType.UTF32:
                    {
                        int bytesize = System.Text.Encoding.UTF32.GetByteCount(value);
                        byte[] result = new byte[bytesize];
                        System.Text.Encoding.UTF32.GetBytes(value, 0, size, result, 0);
                        return result;
                    }
                case StringEncodingType.UTF7:
                    {
                        int bytesize = System.Text.Encoding.UTF7.GetByteCount(value);
                        byte[] result = new byte[bytesize];
                        System.Text.Encoding.UTF7.GetBytes(value, 0, size, result, 0);
                        return result;
                    }
                case StringEncodingType.UTF8:
                    {
                        int bytesize = System.Text.Encoding.UTF8.GetByteCount(value);
                        byte[] result = new byte[bytesize];
                        System.Text.Encoding.UTF8.GetBytes(value, 0, size, result, 0);
                        return result;
                    }
                default:
                    {
                        int bytesize = System.Text.Encoding.Unicode.GetByteCount(value);
                        byte[] result = new byte[bytesize];
                        System.Text.Encoding.Unicode.GetBytes(value, 0, size, result, 0);
                        return result;
                    }
            }
        }

        public static String EncodingBytesToStringValue(byte[] value, StringEncodingType type)
        {
            if (value == null && value.Length == 0)
                return String.Empty;

            switch (type)
            {
                case StringEncodingType.ASCII:
                    return System.Text.Encoding.ASCII.GetString(value).TrimEnd('\0');
                case StringEncodingType.UTF32:
                    return System.Text.Encoding.UTF32.GetString(value).TrimEnd('\0');
                case StringEncodingType.UTF7:
                    return System.Text.Encoding.UTF7.GetString(value).TrimEnd('\0');
                case StringEncodingType.UTF8:
                    return System.Text.Encoding.UTF8.GetString(value).TrimEnd('\0');
                default:
                    return System.Text.Encoding.Unicode.GetString(value).TrimEnd('\0');
            }
        }
    }
}
