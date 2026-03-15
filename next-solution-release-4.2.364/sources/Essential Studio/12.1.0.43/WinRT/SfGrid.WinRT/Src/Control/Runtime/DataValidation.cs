#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Syncfusion.Data.Extensions;
using System.Text;
using System.Reflection;
using Syncfusion.Data;
using System.Windows;
#if WinRT
using Windows.UI.Xaml;
#endif

namespace Syncfusion.UI.Xaml.Grid
{
    public static class DataValidation 
    {
#if !WinRT

        internal static bool ValidateRow(object dataModel)
        {
            var dataValidation = dataModel as IDataErrorInfo;
            if (dataValidation != null)
                if (string.IsNullOrEmpty(dataValidation.Error))
                    return false;
                else
                    return true;
            return false;

        }
#endif

        public static bool Validate(GridCell currentCell,string propertyName, object dataModel)
        {
            bool hasError = false;
#if !WinRT
            var dataValidation = dataModel as IDataErrorInfo;
            if (dataValidation != null)
            {
                string errormessage = dataValidation[propertyName];
                hasError = !String.IsNullOrEmpty(errormessage);
                if (hasError)
                    currentCell.bindingErrorMessage = errormessage;

                currentCell.ApplyValidationVisualState();
                return !hasError;
            }
#endif
#if SyncfusionFramework4_5 || SILVERLIGHT
            hasError = !ValidateINotifyDataErrorInfo(currentCell, propertyName, dataModel);
#endif
            return !hasError;
        }

#if SyncfusionFramework4_5 || SILVERLIGHT
        internal static bool ValidateRowINotifyDataErrorInfo(object dataModel)
        {
            var dataErrorValidation = dataModel as INotifyDataErrorInfo;
            return dataErrorValidation.HasErrors;
        }

        internal static bool ValidateINotifyDataErrorInfo(GridCell currentCell,string propertyName, object dataModel)
        {
            bool hasError = false;
            var dataErrorValidation = dataModel as INotifyDataErrorInfo;
            currentCell.bindingErrorMessage = string.Empty;
            if (dataErrorValidation != null)
            {
                var errorList = dataErrorValidation.GetErrors(propertyName);
                if (errorList != null)
                {
                    var errormessage = errorList.Cast<string>().FirstOrDefault();
                    hasError = !String.IsNullOrEmpty(errormessage);
                    if (hasError)
                        currentCell.bindingErrorMessage = errormessage;
                }
                currentCell.ApplyValidationVisualState();
            }
            return !hasError;
        }
#endif
        private static void ChangeCellErrorState(GridCell element, string propertyName, object datamodel, TextAlignment alignment, bool OnEditing)
        {
            //var dataValidation = datamodel as IDataErrorInfo;
            //if (dataValidation != null)
            //{
            //    var validator = element;
            //    if (validator != null)
            //    {
            //        string errormessage = dataValidation[propertyName];
            //        bool hasError = !String.IsNullOrEmpty(errormessage);
            //        var _args = new ValidationEventArgs
            //        {
            //            TextAlignment = alignment,
            //            ErrorMessage = errormessage,
            //            HasError = hasError
            //        };
            //        validator.Validate(_args);
            //    }
            //}
        }

        public static void ValidateData(GridCell element, string propertyName, object datamodel,bool isInEditing)
        {
            //if (!Validate(propertyName, datamodel))
            //{
            //    ChangeCellErrorState(element, propertyName, datamodel, alignment, isInEditing);
            //}
            //else if ((this.DataGrid.GridValidationMode != GridValidationMode.None && this.DataGrid.GridValidationMode != GridValidationMode.InEdit) || (isInEditing && this.DataGrid.GridValidationMode != GridValidationMode.None))
            //{
            //    ChangeCellErrorState(element, propertyName, datamodel, alignment, isInEditing);
            //}
        }

    }


}
