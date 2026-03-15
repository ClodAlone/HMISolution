using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WPFUtilities;
using Utilities.WPF;
using ViewModelLib;
using NewDriverWizard.ComponentService;
using Utilities;

namespace NewDriverWizard
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
            var selGroup = (wizardControl.SelectedGroup as DevExpress.Xpf.NavBar.NavBarGroup);
            if (selGroup != null && (selGroup.Content as IWizardElement) != null)
                (selGroup.Content as IWizardElement).Execute();

            int nextgroupIndex = wizardControl.Groups.IndexOf(wizardControl.ActiveGroup);
            while (++nextgroupIndex < wizardControl.Groups.Count)
            {
                var wizard = wizardControl.Groups[nextgroupIndex].Content as IWizardElement;
                if (wizard == null || wizard.CanShowed())
                    break;
            }

            if (nextgroupIndex >= wizardControl.Groups.Count)
                nextgroupIndex = wizardControl.Groups.IndexOf(wizardControl.ActiveGroup);

            headerControl.SelectedStep =
            footerControl.SelectedStep = nextgroupIndex;

            wizardControl.SelectedGroup = wizardControl.Groups[nextgroupIndex];
        }
        private void footerControl_Prev(object sender, EventArgs e)
        {
            int activegroupIndex = wizardControl.Groups.IndexOf(wizardControl.ActiveGroup);

            int prevgroupIndex = wizardControl.Groups.IndexOf(wizardControl.ActiveGroup);
            while (--prevgroupIndex >= 0)
            {
                var wizard = wizardControl.Groups[prevgroupIndex].Content as IWizardElement;
                if (wizard == null || wizard.CanShowed())
                    break;
            }

            if (prevgroupIndex < 0)
                prevgroupIndex = wizardControl.Groups.IndexOf(wizardControl.ActiveGroup);

            headerControl.SelectedStep =
            footerControl.SelectedStep = prevgroupIndex;

            wizardControl.SelectedGroup = wizardControl.Groups[prevgroupIndex];
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
            wnd.DialogResult = true;
        }
        private void footerControl_Help(object sender, EventArgs e)
        {
            var helpProvider = NewDriverWizardComponent.ProjectView.helpProvider;
            if (helpProvider != null)
            {
                string helpLink = NewDriverWizardComponent.wizardPluginComponent.GetHelpLink(this);
                helpProvider.OpenDialogHelpPage(helpLink, true, true);
            }
        }
        #endregion
        public Uri ProjectUri;
        bool bLoaded = false;
        const int StepNumber = 4;
        List<String> StepTitles = new List<String>() { Properties.Resources.Step1, Properties.Resources.Step2, Properties.Resources.Step3, Properties.Resources.Step4, Properties.Resources.Stop };
        List<String> StepTooltips = new List<String>() { Properties.Resources.Step1Comment, Properties.Resources.Step2Comment, Properties.Resources.Step3Comment, Properties.Resources.Step4Comment, Properties.Resources.Stop };

        public ProjectWizardUI()
        {
            InitializeComponent();

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
                    headerControl.Image = NewDriverWizardComponent.GetControlImage("WPNewDriverWizardPlugin");
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
        int oldindex = -1;
        private void finishButton_Click(object sender, RoutedEventArgs e)
        {
            var wnd = this.FindParent<Window>();
            if (wnd == null)
                return;
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
    //public class MyCommands
    //{
    //    public static readonly ICommand CloseCommand = new ButtonCommand(o => ((Window)o).Close());
    //}

    //public class ButtonCommand : ICommand
    //{

    //    readonly Action<object> _execute;
    //    readonly Predicate<object> _canExecute;

    //    public ButtonCommand(Action<object> execute)
    //        : this(execute, null)
    //    {

    //    }

    //    public ButtonCommand(Action<object> execute, Predicate<object> canExecute)
    //    {
    //        if (execute == null)
    //            throw new ArgumentException("execute");
    //        _execute = execute;
    //        _canExecute = canExecute;
    //    }

    //    public bool CanExecute(object parameter)
    //    {
    //        return _canExecute == null ? true : _canExecute(parameter);
    //    }

    //    public event EventHandler CanExecuteChanged;

    //    public void Execute(object parameter)
    //    {
    //        _execute(parameter);
    //    }
    //}
}
