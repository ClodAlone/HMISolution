using System;
using System.Collections.Generic;
using System.Linq;
using Utilities;
using System.Windows.Controls;

namespace ImportWizardPlugin
{
    /// <summary>
    /// Interaction logic for ProjectWizardPathAndType.xaml
    /// </summary>
    public partial class EndPage : UserControl, IWizardElement
    {
        public EndPage()
        {
            InitializeComponent();
            InitLabels();
        }

        void InitLabels()
        {
            var cultInfo = new System.Globalization.CultureInfo(System.Globalization.CultureInfo.CurrentCulture.Name, false).TextInfo;
            toCancel.Content = cultInfo.ToTitleCase(Properties.Resources.ToCancel);
            toFinish.Content = cultInfo.ToTitleCase(Properties.Resources.ToFinish);
        }

        public bool Execute()
        {
            return false;
        }
    }
}
