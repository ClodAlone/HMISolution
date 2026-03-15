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

namespace UFUserEditor.Controls
{
    /// <summary>
    /// Interaction logic for MergeRepositoryDialog.xaml
    /// </summary>
    public partial class MergeRepositoryDialog : UserControl
    {
        #region Declarations
        MergeRepositoryModel viewModel;

        bool bLoaded;
        #endregion

        #region Constructors
        public MergeRepositoryDialog()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;

                viewModel = DataContext as MergeRepositoryModel;
                var wnd = this.FindParent<Window>();
                if (wnd != null)
                {
                    wnd.Closing += (s, c) =>
                    {

                    };
                }
            };
        }
        #endregion
    }
}
