using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Bornander.UI.Commands.Tolerant;

namespace Bornander.UI.Commands
{
    public sealed class ExecuteSilent
    {
        public object Parameter { get; private set; }
        public bool IgnoreAllWarnings { get; private set; } 

        public ExecuteSilent(object parameter, bool ignoreAllWarnings)
        {
            Parameter = parameter;
            IgnoreAllWarnings = ignoreAllWarnings;
        }
    }

    public class TolerantCommand<T> : ICommand
    {
        private readonly IDialogDisplayer dialogDisplayer;
        private readonly IWarningRepository<T> repository;

        public Predicate<object> CanExecutePredicate { get; private set; }
        public Action<object, IEnumerable<T>> ExecuteAction { get; private set; }

        public TolerantCommand(IDialogDisplayer dialogDisplayer, 
                               IWarningRepository<T> repository, 
                               Predicate<object> canExecute, 
                               Action<object, IEnumerable<T>> execute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            this.dialogDisplayer = dialogDisplayer;
            this.repository = repository;

            CanExecutePredicate = canExecute;
            ExecuteAction = execute;
        }

        public bool CanExecute(object parameter)
        {
            if (CanExecutePredicate == null)
                return true;

            return CanExecutePredicate(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            bool isSilent = parameter is ExecuteSilent;
            object actualParameter = isSilent ? ((ExecuteSilent)parameter).Parameter : parameter;
            IList<T> localIgnorableWarnings = new List<T>(
                repository != null ? 
                    repository.Ignored : new T[0]);

            while (true)
            {
                try
                {
                    // Execute the comamnd
                    ExecuteAction(actualParameter, 
                        isSilent && ((ExecuteSilent)parameter).IgnoreAllWarnings ? 
                            null : 
                            localIgnorableWarnings);
                    return;
                }
                catch (CommandWarningException warning)
                {
                    if (isSilent)
                        return;
                    // If a warning is thrown, show a dialog.
                    // If the user accepts the warning the
                    // command is executed again, this time with the
                    // warning ignored
                    switch (dialogDisplayer.ShowWarning(warning))
                    {
                        case DialogResult.No:
                            return;
                        case DialogResult.YesAndRememberMyDecision:
                            // Persist the preference
                            if (repository != null)
                                repository.Ignore((T)warning.Warning);
                            break;
                    }
                    localIgnorableWarnings.Add((T)warning.Warning);
                }
                catch (CommandRetryableErrorException error)
                {
                    if (isSilent)
                        return;

                    if (dialogDisplayer.ShowError(error) == DialogResult.No)
                        return;
                }
            }
        }

        public static bool IsWarningIgnored(T warning, IEnumerable<T> ignorableWarnings)
        {
            return ignorableWarnings == null || ignorableWarnings.Contains(warning);
        }
    }
}
