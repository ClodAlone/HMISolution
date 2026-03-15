using CustomActions;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Forms;

namespace CustomActions.Tests
{
    [TestClass()]
    public class CustomActionsTest
    {
        [TestMethod()]
        public void InitializeTestDOMAINUser()
        {
            var domain = ShowDialog("Domain name:", "CFR21 DOMAIN management");
            var user = ShowDialog("User name:", "CFR21 DOMAIN management");
            var password = ShowDialog("Password:", "CFR21 DOMAIN management", true);
#if DEBUG
            CustomActions.InitSessionValues("CUSTOM", "REMOTE", domain, user, password, "", "", "", "");
            Assert.AreEqual(CustomActions.Initialize(), WixToolset.Dtf.WindowsInstaller.ActionResult.Success);
#endif
        }

        [TestMethod()]
        public void InitializeTestLOCALUser()
        {

#if DEBUG
            CustomActions.InitSessionValues("DEFAULT", "LOCAL", "", "", "", "", "", "", "");
            Assert.AreEqual(CustomActions.Initialize(), WixToolset.Dtf.WindowsInstaller.ActionResult.Success);
#endif

#if DEBUG
            CustomActions.InitSessionValues("CUSTOM", "LOCAL", "", "", "", "", "", "", "");
            Assert.AreEqual(CustomActions.Initialize(), WixToolset.Dtf.WindowsInstaller.ActionResult.Success);
#endif
        }

        [TestMethod()]
        public void FinalizeTestLOCALUser()
        {
            var sqlInstance = ShowDialog("Server name:", "SQLServer settings");
            var sqlUser = ShowDialog("User name:", "SQLServer settings");
            var sqlPassword = ShowDialog("Password:", "SQLServer settings", true);

#if DEBUG
            CustomActions.InitSessionValues("DEFAULT", "LOCAL", "", "", "", "TrustedConnection", sqlInstance, "", "");
            Assert.AreEqual(CustomActions.Finalize(), WixToolset.Dtf.WindowsInstaller.ActionResult.Success);

            CustomActions.InitSessionValues("CUSTOM", "LOCAL", "", "", "", "", sqlInstance, sqlUser, sqlPassword);
            Assert.AreEqual(CustomActions.Finalize(), WixToolset.Dtf.WindowsInstaller.ActionResult.Success);
#endif
        }

        [TestMethod()]
        public void FinalizeTestDOMAINUser()
        {
            var sqlInstance = ShowDialog("Server name:", "SQLServer settings");
            var sqlUser = ShowDialog("User name:", "SQLServer settings");
            var sqlPassword = ShowDialog("Password:", "SQLServer settings", true);

            var domain = ShowDialog("Domain name:", "CFR21 DOMAIN management");
            var user = ShowDialog("User name:", "CFR21 DOMAIN management");
            var password = ShowDialog("Password:", "CFR21 DOMAIN management", true);
#if DEBUG
            CustomActions.InitSessionValues("CUSTOM", "REMOTE", domain, user, password, "TrustedConnection", "", "", "");
            Assert.AreEqual(CustomActions.Finalize(), WixToolset.Dtf.WindowsInstaller.ActionResult.Success);

            CustomActions.InitSessionValues("CUSTOM", "REMOTE", domain, user, password, "", sqlInstance, sqlUser, sqlPassword);
            Assert.AreEqual(CustomActions.Finalize(), WixToolset.Dtf.WindowsInstaller.ActionResult.Success);
#endif
        }
        static string ShowDialog(string text, string caption,bool maskText = false)
        {
            Form prompt = new Form();
            prompt.Width = 400;
            prompt.Height = 150;
            prompt.Text = caption;
            prompt.MinimizeBox = false;
            prompt.MaximizeBox = false;
            prompt.FormBorderStyle = FormBorderStyle.FixedSingle;
            prompt.StartPosition = FormStartPosition.WindowsDefaultLocation;
            Label textLabel = new Label() { Left = 20, Top = 10, Text = text };
            TextBox textBox = null;
            MaskedTextBox maskedTextBox = null;
            if (maskText)
            {
                maskedTextBox = new MaskedTextBox() { Left = 20, Top = 35, Width = 340, Height = 80 };
                maskedTextBox.PasswordChar = '*';
                prompt.Controls.Add(maskedTextBox);
            }
            else
            {
                textBox = new TextBox() { Left = 20, Top = 35, Width = 340, Height = 80 };
                prompt.Controls.Add(textBox);
            }

            Button confirmation = new Button() { Text = "Ok", Left = 140, Width = 120, Top = 72 };
            confirmation.PreviewKeyDown += (o, e) => { if (e.KeyData == Keys.Enter) prompt.Close(); };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.PreviewKeyDown += (o, e) => { if (e.KeyData == Keys.Enter) prompt.Close(); };
            prompt.ShowDialog();
            if (maskText)
            {
                return maskedTextBox?.Text;
            }
            else
            {
                return textBox?.Text;
            }
        }
    }
}
