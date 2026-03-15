using System;
using System.Collections.Generic;
using System.Linq;
using Utilities;
using System.Windows.Controls;

namespace NewDriverWizard
{
    /// <summary>
    /// Interaction logic for ProjectWizardPathAndType.xaml
    /// </summary>
    public partial class EndPage : UserControl, IWizardElement
    {
        public EndPage()
        {
            InitializeComponent();
        }

        public bool Execute()
        {
            try
            {
                return false;
            }
            catch (Exception ex)
            {
                return true;
            }
        }

        public bool CanShowed()
        {
            return true;
        }
    }
}
