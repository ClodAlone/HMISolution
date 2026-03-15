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
using System.Collections.ObjectModel;
using System.ComponentModel;
using UFInterfaces.Commandable;
using Utilities;
using Utilities.WPF;
using System.Collections;

namespace CommandExplorer
{
    /// <summary>
    /// Interaction logic for CommandEditorUI.xaml
    /// </summary>
    public partial class CommandEditorUI : UserControl, IDisposable
    {
        Controls.GridControl gridControl;
        Controls.PropertyControl propertyControl;

        readonly bool bCheckUnload;
        public CommandEditorUI(bool bCU = true, bool isWebHMIProject = false)
        {
            InitializeComponent();

            bCheckUnload = bCU;
            gridControl = new Controls.GridControl(this, isWebHMIProject);
            propertyControl = new Controls.PropertyControl(gridControl, this);
            gridControl.propertyControl = propertyControl;

            left.Content = gridControl;
            right.Content = propertyControl;
        }

        bool _Synchronous = false;
        public bool Synchronous 
        {
            get { return _Synchronous; }
            set { _Synchronous = value; }
        }

        public void PropagateChanges()
        {
            gridControl.PropagateChanges();
        }

        public bool CheckErrorBindings(bool showerror = true)
        {
            var valid = propertyControl.ValidateBindings();

            return valid;
        }
        public void ForcePropertyControlUIFocus()
        {
            if (propertyControl != null)
            {
                propertyControl.Focusable = true;
                propertyControl.Focus();
                FocusManager.SetFocusedElement(Window.GetWindow(propertyControl), propertyControl);
            }
        }

        private void control_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            gridControl.DataContext = DataContext;
        }

        private void control_Unloaded(object sender, RoutedEventArgs e)
        {
            if(!Synchronous && bCheckUnload)
                PropagateChanges();
        }

        public void Dispose()
        {
            gridControl.Dispose();
            gridControl.propertyControl = null;
        }
    }
}
