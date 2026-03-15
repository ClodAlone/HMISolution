using System;
using System.Collections.Generic;
using System.Linq;

using System.Windows.Controls;

namespace StandardWizard
{
    /// <summary>
    /// Interaction logic for ProjectWizardPathAndType.xaml
    /// </summary>
    public partial class Step1 : UserControl, IWizardElement
    {
        public Step1()
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

    }
}
