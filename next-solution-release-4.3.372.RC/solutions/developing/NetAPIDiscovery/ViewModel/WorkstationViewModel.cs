using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using ViewModelLib;
using System.ComponentModel;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.IO;
using System.Windows.Controls;
using NetAPIDiscovery.ContextMenuKey;
using UFInterfaces;

namespace NetAPIDiscovery
{
    public class WorkstationViewModel : TreeViewItemViewModel, IEntityReference
    {
        #region Declaration
        #endregion

        #region Constructor
        public WorkstationViewModel(Dispatcher dispatcher, String workstation, TreeViewItemViewModel parent)
            : base(parent, false)
        {
            m_dispatcher = dispatcher;
            Title = workstation;
        }
        #endregion


        protected override void IdleExecution()
        {
        }

        #region IDisposable Members
        protected override void OnDispose()
        {
            base.OnDispose();
        }
        #endregion

        #region Validations

        protected override String PerformValidation(String propertyName)
        {
            if (propertyName == "Title")
            {
                if (String.IsNullOrEmpty(Title))
                    return Properties.Resource.WorkStationName_Invalid;
            }

            return base.PerformValidation(propertyName);
        }

        public override string Error
        {
            get
            {
                return null;
            }
        }

        public override string this[string propertyName]
        {
            get
            {
                return PerformValidation(propertyName);
            }
        }
        #endregion

        #region IEntityReference Members

        [Browsable(false)]
        public ImageSource CollapsedImageSource
        {
            get
            {
                var bm = ViewModel.ViewModelHelper.GetControlImage("NAPIDhd");
                return bm;
            }
        }

        [Browsable(false)]
        public ContextMenu contextMenu
        {
            get
            {
                WorkstationMenu wm = new WorkstationMenu();
                wm.InitializeComponent();

                return wm["ContextMenuKey"] as ContextMenu;
            }
        }

        [Browsable(false)]
        public Object Tooltip
        {
            get
            {
                return new NetAPIDiscovery.UserControls.WorkstationViewModel();
            }
        }

        [Browsable(false)]
        public ImageSource ExpandedImageSource
        {
            get { return null; }
        }

        [Browsable(false)]
        public object ContainedObject
        {
            get { return null; }
        }

        [Browsable(false)]
        public object EntityParent
        {
            get { return Parent; }
        }

        [Browsable(false)]
        public String TypeDefinitionString
        {
            get { return null; }
        }

        #endregion
    }
}
