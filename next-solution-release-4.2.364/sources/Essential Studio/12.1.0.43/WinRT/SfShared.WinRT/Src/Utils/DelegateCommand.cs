// <copyright file="DelegateCommand.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
#if WINDOWS_PHONE || WINDOWS_PHONE_7
namespace Syncfusion.WP.Utils
#else
#if SILVERLIGHT
namespace Syncfusion.Tools.Utils
#else
#if WPF
namespace Syncfusion.Windows.Utils
#else
namespace Syncfusion.UI.Xaml.Utils
#endif
#endif
#endif
{
    /// <summary>
    /// Represents a class for the delegate command
    /// </summary>
    [ClassReference(IsReviewed = false, ShouldInclude = false)]
    public class DelegateCommand : ICommand
    {
        #region Variables

        readonly Predicate<Object> _canExecute = null;
        readonly Action<Object> _executeAction = null;

        #endregion

        #region Public DelegateCommand
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Utils.DelegateCommand"/> class.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Utils.DelegateCommand"/>
        /// <seealso cref="N:Syncfusion.UI.Xaml.Utils">Syncfusion.UI.Xaml.Utils
        /// Namespace</seealso>
        /// <param name="executeAction"></param>
        /// <param name="canExecute"></param>
        [ClassReference(IsReviewed = false)]
        public DelegateCommand(Action<object> executeAction, Predicate<Object> canExecute)
        {
            _executeAction = executeAction;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Utils.DelegateCommand"/> class.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Utils.DelegateCommand"/>
        /// <seealso cref="N:Syncfusion.UI.Xaml.Utils">Syncfusion.UI.Xaml.Utils
        /// Namespace</seealso>
        /// <param name="executeAction"></param>
        public DelegateCommand(Action<object> executeAction)
            : this(executeAction, null)
        {
            _executeAction = executeAction;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Invokes event if can execute
        /// </summary>
        public void UpdateCanExecute()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, new EventArgs());
        }

        /// <summary>
        /// Returns a value if can execute
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }


        /// <summary>
        /// Executes the parameter command
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            if (_executeAction != null)
                _executeAction(parameter);
            UpdateCanExecute();
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when value of CanExecute as changed
        /// </summary>
        public event EventHandler CanExecuteChanged;

        #endregion
    }
}
