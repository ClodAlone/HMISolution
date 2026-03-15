using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Utilities.WPF;

namespace AuditTrace
{
    /// <summary>
    /// Interaction logic for AuditTraceComment.xaml
    /// </summary>
    public partial class AuditTraceComment : UserControl
    {
        #region Constructors
        public AuditTraceComment()
        {
            InitializeComponent();
        }
        #endregion

        #region Methods
        private void EditAuditComment_Click(object sender, RoutedEventArgs e)
        {
            var auditComment = Pads.Pads.ShowAlphaNumericPad(txtAuditComment.Text, this.FindParent<Window>());
            if (!String.IsNullOrEmpty(auditComment))
            {
                txtAuditComment.Text = auditComment;
                txtAuditComment.SelectAll();
            }
            txtAuditComment.Focus();
        }
        #endregion
    }
}
