#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Syncfusion.Windows.Shared.Olap
{
    #region Delegate Command

    /// <summary>
    /// Delegate command to interact with the binding command.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DelegateCommand<T> : ICommand, ISuppressCommand
    {
        #region Members

        private readonly Action _execute = null;
        private readonly Func<bool> _canExecute = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="executeMethod">The execute method.</param>
        public DelegateCommand(Action executeMethod)
            : this(executeMethod, null)//, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="executeMethod">The execute method.</param>
        /// <param name="CanExecute">The can execute.</param>
        public DelegateCommand(Action executeMethod, Func<bool> CanExecute)
        //:this(executeMethod, CanExecute, false)
        {
            if (executeMethod == null)
            {
                throw new ArgumentNullException("executeMethod");
            }
            _execute = executeMethod;
            _canExecute = CanExecute;
        }

        #endregion

        #region ICommand Members

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            return this.CanExecute();
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(object parameter)
        {
            this.Execute();
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        #endregion

        #region ISuppressCommand Members

        /// <summary>
        /// Determines whether this instance can execute.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance can execute; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        ///     This one is a suppress implementation of CanExecute in ICommand.
        /// </remarks>
        public bool CanExecute()
        {
            if (_canExecute != null)
                return _canExecute();

            return true;
        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <remarks>
        ///     This one is a suppress implementation of Execute in ICommand.
        /// </remarks>
        public void Execute()
        {
            if (_execute != null)
            {
                _execute();
            }
        }

        #endregion
    }

    #endregion

    #region ISuppressCommand Interface

    /// <summary>
    /// Used for suppressing the object parameter in ICommand interface.
    /// </summary>
    public interface ISuppressCommand
    {
        #region Methods

        bool CanExecute();
        void Execute();

        #endregion
    }

    #endregion
}
