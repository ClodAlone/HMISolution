using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using TranslationHelpers;
using Utilities.Converters;
using UFRecipeExecutionContext;

namespace RecipeAuditTrace.Converters
{
    public class RecipeCommandToStringConverter : IMultiValueConverter
    {
        #region IMultiValueConverter
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(String))
                throw new InvalidOperationException("The target must be a String");

            if (values == null || values.Length == 0 || !(values[0] is RecipeCommandType))
                return null;

            IDictionary<string, string> stringIds = null;
            if (values.Length > 1)
                stringIds = values[1] as IDictionary<string, string>;

            var commandType = (RecipeCommandType)values[0];
            var original = commandType.ToString();

            var resourceConverter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(RecipeCommandType)) as ResourceEnumConverter;
            if (resourceConverter != null)
                original = resourceConverter.ConvertTo(commandType, targetType) as String;

            switch (commandType)
            {
                case RecipeCommandType.Show:
                    return TranslationHelper.TranlslateText($"_{RecipeAuditTraceViewModel.stringPlaceolder}_CommandTypeShow", stringIds, original);
                case RecipeCommandType.Load:
                    return TranslationHelper.TranlslateText($"_{RecipeAuditTraceViewModel.stringPlaceolder}_CommandTypeLoad", stringIds, original);
                case RecipeCommandType.Save:
                    return TranslationHelper.TranlslateText($"_{RecipeAuditTraceViewModel.stringPlaceolder}_CommandTypeSave", stringIds, original);
                case RecipeCommandType.Remove:
                    return TranslationHelper.TranlslateText($"_{RecipeAuditTraceViewModel.stringPlaceolder}_CommandTypeRemove", stringIds, original);
                case RecipeCommandType.Activate:
                    return TranslationHelper.TranlslateText($"_{RecipeAuditTraceViewModel.stringPlaceolder}_CommandTypeActivate", stringIds, original);
                case RecipeCommandType.Read:
                    return TranslationHelper.TranlslateText($"_{RecipeAuditTraceViewModel.stringPlaceolder}_CommandTypeRead", stringIds, original);
                case RecipeCommandType.Export:
                    return TranslationHelper.TranlslateText($"_{RecipeAuditTraceViewModel.stringPlaceolder}_CommandTypeExport", stringIds, original);
                case RecipeCommandType.Import:
                    return TranslationHelper.TranlslateText($"_{RecipeAuditTraceViewModel.stringPlaceolder}_CommandTypeImport", stringIds, original);
                default:
                    return original;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
