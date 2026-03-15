using System;
using Bornander.UI.Commands;
using Bornander.UI.Commands.Tolerant;

namespace Bornander.UI.Commands.Test
{
    public class AcknowledgableDialogDisplayer : IDialogDisplayer
    {
        public DialogResult ShowWarning(CommandWarningException warning)
        {
            AcknowledgableDialog dialog = new AcknowledgableDialog("Warning", warning.Message, true);
            dialog.ShowDialog();
            return dialog.Result;
        }

        public DialogResult ShowError(CommandRetryableErrorException error)
        {
            AcknowledgableDialog dialog = new AcknowledgableDialog("Error", error.Message, false);
            dialog.ShowDialog();
            return dialog.Result;
        }
    }
}
