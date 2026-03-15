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
using EmptyProjectWizardPlugin.ComponentService;
using Utilities;

namespace EmptyProjectWizardPlugin
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
            var helpProvider = EmptyProjectWizardPluginComponent.ProjectView.helpProvider;
            if (helpProvider != null)
                helpProvider.OpenDialogHelpPage("EmptyProjectWizard", true, true);
        }
        #endregion
        public Uri ProjectUri;
        public ProjectViewModel ProjectView;
        public string StratingVFSFolder { get; protected set; }
        public string StratingTempFolder { get; protected set; }
        bool bLoaded = false;
#if !CONNEXT
        const int StepNumber = 2;
        List<String> StepTitles = new List<String>() { Properties.Resources.Step1, Properties.Resources.Step2, Properties.Resources.Stop };
        List<String> StepTooltips = new List<String>() { Properties.Resources.Step1Comment, Properties.Resources.Step2Comment, Properties.Resources.Stop };
#else 
        const int StepNumber = 1;
        List<String> StepTitles = new List<String>() { Properties.Resources.Step2, Properties.Resources.Stop };
        List<String> StepTooltips = new List<String>() { Properties.Resources.Step2Comment, Properties.Resources.Stop };
#endif
        public ProjectWizardUI()
        {
            InitializeComponent();
            StratingVFSFolder = "Projects";
            StratingTempFolder = string.Format("{0}\\{1}", Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "WzrdTmp");

#if CONNEXT
            wizardControl.Groups.Remove(ArchitectureItem);
#endif

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
                    headerControl.Image = EmptyProjectWizardPluginComponent.GetControlImage("WPEmptyProjectWizardPlugin");
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

        private void finishButton_Click(object sender, RoutedEventArgs e)
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

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            var wnd = this.FindParent<Window>();
            if (wnd == null)
                return;
            wnd.DialogResult = false;
        }
    }
    public class MyCommands
    {
        public static readonly ICommand CloseCommand = new ButtonCommand(o => ((Window)o).Close());
    }

    public class ButtonCommand : ICommand
    {

        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;

        public ButtonCommand(Action<object> execute)
            : this(execute, null)
        {

        }

        public ButtonCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentException("execute");
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}
