using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WinWrap.Basic;

namespace ScriptExplorer
{
    /// <summary>
    /// Interaction logic for ScriptExplorerUI.xaml
    /// </summary>
    public partial class ScriptExplorerUI : UserControl
    {
        public ScriptExplorerUI()
        {
            InitializeComponent();
        }

        BasicIdeCtl GetCurrentControl()
        {
            var item = TabControl.SelectedItemContent as Grid;
            if (item == null)
                return null;
            var control = item.Children[0] as BasicIdeCtl;
            return control;
        }

        #region Commands

        void OnCutEvent(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            GetCurrentControl().ExecuteMenuCommand(WinWrap.Basic.CommandConstants.EditCut);
        }

        void OnCopyEvent(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            GetCurrentControl().ExecuteMenuCommand(WinWrap.Basic.CommandConstants.EditCopy);
        }

        void CanExecuteSelected(object sender, CanExecuteRoutedEventArgs e)
        {
            var control = GetCurrentControl();
            e.CanExecute = control != null && control.IsMenuCommandEnabled(WinWrap.Basic.CommandConstants.EditCut);
        }

        void OnPasteEvent(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            GetCurrentControl().ExecuteMenuCommand(WinWrap.Basic.CommandConstants.EditPaste);
        }

        void CanExecutePaste(object sender, CanExecuteRoutedEventArgs e)
        {
            var control = GetCurrentControl();
            e.CanExecute = control != null && control.IsMenuCommandEnabled(WinWrap.Basic.CommandConstants.EditPaste);
        }

        void OnUndoEvent(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            GetCurrentControl().ExecuteMenuCommand(WinWrap.Basic.CommandConstants.EditUndo);
        }

        void CanExecuteUndo(object sender, CanExecuteRoutedEventArgs e)
        {
            var control = GetCurrentControl();
            e.CanExecute = control != null && control.IsMenuCommandEnabled(WinWrap.Basic.CommandConstants.EditUndo);
        }

        void OnRedoEvent(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            GetCurrentControl().ExecuteMenuCommand(WinWrap.Basic.CommandConstants.EditRedo);
        }

        void CanExecuteRedo(object sender, CanExecuteRoutedEventArgs e)
        {
            var control = GetCurrentControl();
            e.CanExecute = control != null && control.IsMenuCommandEnabled(WinWrap.Basic.CommandConstants.EditRedo);
        }

        #endregion
    }
}
