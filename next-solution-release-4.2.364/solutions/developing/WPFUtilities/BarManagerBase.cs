using DevExpress.Xpf.Bars;
using DocumentManager.ComponentService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UFInterfaces;

namespace WPFUtilities
{
    public abstract class BarManagerBase : BarManager, IToolbar, INotifyPropertyChanged
    {
        #region Declarations
        bool bIsToolbarShown;
        IDocumentManager document;
        IWorkspace workspace;
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Public Properties
        public bool IsToolbarShown
        {
            get
            {
                return bIsToolbarShown;
            }
            set
            {
                if (value != bIsToolbarShown)
                {
                    bIsToolbarShown = value;
                    OnPropertyChanged("IsToolbarShown");
                }
            }
        }
        bool bIsMenuShown = true;
        public bool IsMenuShown
        {
            get
            {
                return bIsMenuShown;
            }
            set
            {
                if (value != bIsMenuShown)
                {
                    bIsMenuShown = value;
                    OnPropertyChanged("IsMenuShown");
                }
            }
        }
        protected readonly BarManager Toolbar;
        public System.Windows.FrameworkContentElement ToolbarMenuItem { get; set; }
        #endregion

        #region Ctor
        public BarManagerBase()
        {

        }
        public BarManagerBase(IDocumentManager doc, IWorkspace ws, BarManager toolbarControl = null)
        {
            document = doc;
            workspace = ws;
            Toolbar = toolbarControl;

            DataContextChanged += (o, e) =>
            {
                Toolbar.DataContext = DataContext;
            };
        }
        #endregion

        #region CommandBindings
        protected void CanAlwaysExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        protected void OnShowToolbar(object sender, ExecutedRoutedEventArgs e)
        {
            if (!IsToolbarShown)
                Show();
            else
                Hide();
        }
        #endregion

        #region IToolbar
        public void Show()
        {
            if (workspace == null || document == null || Toolbar == null)
                return;

            if (ToolbarMenuItem != null)
                ((BarItem)ToolbarMenuItem).IsVisible = true;

            if (IsToolbarShown)
                return;

            workspace.AddBarManagerItem(Toolbar, null, document.TypeTitle, document.TypeScheme);
            IsToolbarShown = true;
        }

        public void Hide()
        {
            if (workspace == null || document == null || Toolbar == null)
                return;
            
            if (!IsToolbarShown)
                return;

            workspace.RemoveBarManagerItem(Toolbar, null, document.TypeScheme);
            IsToolbarShown = false;
        }

        public void ShowMenuItem()
        {
            if (ToolbarMenuItem == null || workspace == null || document == null || Toolbar == null)
                return;

            ((BarItem)ToolbarMenuItem).IsVisible = true;
            IsMenuShown = true;
        }

        public bool HideMenuItem()
        {
            if (ToolbarMenuItem == null || workspace == null || document == null || Toolbar == null)
                return ToolbarMenuItem != null;

            ((BarItem)ToolbarMenuItem).IsVisible = false;
            IsMenuShown = false;
            return true;
        }

        bool IToolbar.IsVisible()
        {
            return IsToolbarShown;
        }

        CommandBindingCollection IToolbar.BarCommandBindings
        {
            get
            {
                return GetCommandBindings();
            }
        }

        protected abstract CommandBindingCollection GetCommandBindings();
        #endregion

        #region INotifyPropertyChanged
        void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
