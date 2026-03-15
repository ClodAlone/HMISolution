using System;

namespace Bornander.UI.Commands.Tolerant
{
    public enum DialogResult
    {
        Yes,
        YesAndRememberMyDecision,
        No
    }

    public interface IDialogDisplayer
    {
        DialogResult ShowWarning(CommandWarningException warning);

        DialogResult ShowError(CommandRetryableErrorException error);
    }
}
