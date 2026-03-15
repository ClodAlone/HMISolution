using Pads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Utilities.WPF;

namespace RecipeAuditTrace
{
    /// <summary>
    /// Interaction logic for AuditTraceComment.xaml
    /// </summary>
    public partial class AuditTraceComment : UserControl
    {
        public AuditTraceComment()
        {
            InitializeComponent();
        }

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