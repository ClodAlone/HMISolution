using DevExpress.XtraEditors;
using System;
using System.Windows.Forms;

namespace AutoUpdaterDotNET
{
    internal partial class RemindLaterForm : XtraForm
    {
        public RemindLaterFormat RemindLaterFormat { get; private set; }

        public int RemindLaterAt { get; private set; }

        public RemindLaterForm()
        {
            InitializeComponent();

            SetControlImage(AutoUpdater.IsDarkSkin());
            if (AutoUpdater.DefaultFont != null)
            {
                Font = AutoUpdater.DefaultFont;
                labelTitle.Font = new System.Drawing.Font(Font, labelTitle.Font.Style);
                labelDescription.Font = new System.Drawing.Font(Font, labelDescription.Font.Style);
                radioButtonNo.Font = new System.Drawing.Font(Font, radioButtonNo.Font.Style);
                radioButtonYes.Font = new System.Drawing.Font(Font, radioButtonYes.Font.Style);
                comboBoxRemindLater.Font = new System.Drawing.Font(Font, comboBoxRemindLater.Font.Style);
                buttonOK.Font = new System.Drawing.Font(Font, buttonOK.Font.Style);
            }
        }

        private void RemindLaterFormLoad(object sender, EventArgs e)
        {
            comboBoxRemindLater.SelectedIndex = 0;
            radioButtonYes.Checked = true;
        }
        private void SetControlImage(bool isDark)
        {
            if (isDark)
            {
                this.IconOptions.Image = global::AutoUpdaterDotNET.Properties.Resources.Update_Light;
                this.pictureBoxIcon.Image = global::AutoUpdaterDotNET.Properties.Resources.clock_go_32_Light;
                this.buttonOK.Image = global::AutoUpdaterDotNET.Properties.Resources.clock_play_Light;
            }
            else
            {
                this.IconOptions.Image = global::AutoUpdaterDotNET.Properties.Resources.Update_Dark;
                this.pictureBoxIcon.Image = global::AutoUpdaterDotNET.Properties.Resources.clock_go_32_Dark;
                this.buttonOK.Image = global::AutoUpdaterDotNET.Properties.Resources.clock_play_Dark;
            }
        }
        private void ButtonOkClick(object sender, EventArgs e)
        {
            if (radioButtonYes.Checked)
            {
                switch (comboBoxRemindLater.SelectedIndex)
                {
                    case 0:
                        RemindLaterFormat = RemindLaterFormat.Minutes;
                        RemindLaterAt = 30;
                        break;
                    case 1:
                        RemindLaterFormat = RemindLaterFormat.Hours;
                        RemindLaterAt = 12;
                        break;
                    case 2:
                        RemindLaterFormat = RemindLaterFormat.Days;
                        RemindLaterAt = 1;
                        break;
                    case 3:
                        RemindLaterFormat = RemindLaterFormat.Days;
                        RemindLaterAt = 2;
                        break;
                    case 4:
                        RemindLaterFormat = RemindLaterFormat.Days;
                        RemindLaterAt = 4;
                        break;
                    case 5:
                        RemindLaterFormat = RemindLaterFormat.Days;
                        RemindLaterAt = 8;
                        break;
                    case 6:
                        RemindLaterFormat = RemindLaterFormat.Days;
                        RemindLaterAt = 10;
                        break;
                }
                DialogResult = DialogResult.OK;
            }
            else
            {
                DialogResult = DialogResult.Abort;
            }
        }

        private void RadioButtonYesCheckedChanged(object sender, EventArgs e)
        {
            comboBoxRemindLater.Enabled = radioButtonYes.Checked;
        }
    }
}
