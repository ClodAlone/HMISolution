using HelpProvider.ComponentService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UIMsgBoxAlertService.ComponentService;
using Utilities;

namespace CommonControls
{
    public static class ConnectionWizardDialog
    {
        public static bool GetConnectionString(ref string connectionString, string helpLink, Window owner, IUIMsgBoxAlertService uIInterface, IHelpProvider helpProvider)
        {
            if (connectionString == null)
                connectionString = string.Empty;

            var wizard = new ConnectionWizard(uIInterface, helpProvider)
            {
                ConnectionString = connectionString
            };
            GeneralDialogContent Dialog = new GeneralDialogContent(wizard)
            {
                Owner = owner,
                HelpLink = helpLink
            };

            Dialog.Closing += (o, ea) =>
            {
                if (Dialog.DialogResult == true)
                    ea.Cancel = wizard.IsValid(uIInterface);
            };

            if (Dialog.ShowDialog() == true)
            {
                connectionString = wizard.ConnectionString;
                return true;
            }

            return false;
        }
    }
}
