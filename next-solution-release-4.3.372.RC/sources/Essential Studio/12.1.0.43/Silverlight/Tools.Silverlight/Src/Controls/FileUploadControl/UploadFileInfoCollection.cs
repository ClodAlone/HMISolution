#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Tools.Controls
{
    using System;
    using System.Collections.ObjectModel;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
using System.Windows.Data;

    /// <summary>
    /// Represents UploadFileInfo Collection
    /// </summary>
    public class UploadFileInfoCollection : ObservableCollection<UploadFileInfo>
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public class VisibilityConverter : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool flag = (bool)value;
            if (flag)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class FileUploadCommand : ICommand
    {
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler CanExecuteChanged;

        readonly Predicate<Object> _canExecute = null;
        readonly Action<Object> _executeAction = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="executeAction"></param>
        /// <param name="canExecute"></param>
        public FileUploadCommand(Action<object> executeAction, Predicate<Object> canExecute)
        {
            _executeAction = executeAction;
            _canExecute = canExecute;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="executeAction"></param>
        public FileUploadCommand(Action<object> executeAction)
            : this(executeAction, null)
        {
            _executeAction = executeAction;
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateCanExecute()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, new EventArgs());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            if (_executeAction != null)
                _executeAction(parameter);
            UpdateCanExecute();
        }
    }

}
