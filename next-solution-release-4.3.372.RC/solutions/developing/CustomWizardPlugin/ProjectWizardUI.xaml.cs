using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WPFUtilities;
using Utilities.WPF;
using WizardSettings;
using ViewModelLib;
using System.Windows.Media;
using CustomWizardPlugin.ComponentService;
using Utilities;

namespace CustomWizardPlugin
{
    /// <summary>
    /// Interaction logic for ProjectWizardUI.xaml
    /// </summary>
    public partial class ProjectWizardUI : UserControl
    {
        // Fields...
        #region command
        private void footerControl_Next(object sender, EventArgs e)
        {
            int activegroupIndex = wizardControl.Groups.IndexOf(wizardControl.ActiveGroup);
            int nextgroupIndex = activegroupIndex + 1 < wizardControl.Groups.Count ? activegroupIndex + 1 : wizardControl.Groups.Count - 1;

            headerControl.SelectedStep = nextgroupIndex;
            footerControl.SelectedStep = nextgroupIndex;

            wizardControl.SelectedGroup = wizardControl.Groups[nextgroupIndex];
        }
        private void footerControl_Prev(object sender, EventArgs e)
        {
            int activegroupIndex = wizardControl.Groups.IndexOf(wizardControl.ActiveGroup);
            int nextgroupIndex = activegroupIndex - 1 >= 0 ? activegroupIndex - 1 : 0;

            headerControl.SelectedStep = nextgroupIndex;
            footerControl.SelectedStep = nextgroupIndex;

            wizardControl.SelectedGroup = wizardControl.Groups[nextgroupIndex];
        }
        private void footerControl_Cancel(object sender, EventArgs e)
        {
            var wnd = this.FindParent<Window>();
            if (wnd == null)
                return;
            wnd.DialogResult = false;
        }
        private void footerControl_Finish(object sender, EventArgs e)
        {
            var wnd = this.FindParent<Window>();
            if (wnd == null)
                return;

            var _path = PathAndType;

            if (_path == null)
                return;

            String _ppath = string.Empty;
            String _pname = string.Empty;

            if (!_path.CheckConsistency())
                wnd.DialogResult = true;
        }
        private void footerControl_Help(object sender, EventArgs e)
        {
            var helpProvider = CustomWizardPluginComponent.ProjectView.helpProvider;
            if (helpProvider != null)
                helpProvider.OpenDialogHelpPage("CustomProjectWizard", true, true);
        }
        #endregion
        public Uri ProjectUri;
        public ProjectViewModel ProjectView;
        public string StratingVFSFolder { get; protected set; }
        public string StratingTempFolder { get; protected set; }
        bool bLoaded = false;
        const int StepNumber = 4;
        List<String> StepTitles = new List<String>() { Properties.Resources.Step1, Properties.Resources.Step2, Properties.Resources.Step3, Properties.Resources.Step4, Properties.Resources.Stop };
        List<String> StepTooltips = new List<String>() { Properties.Resources.Step1Comment, Properties.Resources.Step2Comment, Properties.Resources.Step3Comment, Properties.Resources.Step4Comment, Properties.Resources.Stop };
        public ProjectWizardUI()
        {
            InitializeComponent();
            StratingVFSFolder = "Projects";
            StratingTempFolder = string.Format("{0}\\{1}", Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "WzrdTmp");

            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    bLoaded = true;
                    var currentSkin = Utilities.ApplicationPropertiesHelper.GetProperty<String>("CurrentSkin");
                    WPFUtilities.ThemeHelper.SetTheme(this, currentSkin);
                    headerControl.HeaderTitle = Properties.Resources.PluginTitle;
                    headerControl.StepNumber = StepNumber;
                    headerControl.StepTitles = StepTitles;
                    headerControl.StepTooltips = StepTooltips;
                    headerControl.MinWidth = 650;
                    headerControl.MinHeight = 85;
                    headerControl.Image = CustomWizardPluginComponent.GetControlImage("WPCustomWizardPlugin");
                    footerControl.StepNumber = StepNumber;
                }
            };
            Unloaded += (o, e) =>
            {
                if (bLoaded)
                {
                    bLoaded = false;
                }
            };
        }
    }
}
