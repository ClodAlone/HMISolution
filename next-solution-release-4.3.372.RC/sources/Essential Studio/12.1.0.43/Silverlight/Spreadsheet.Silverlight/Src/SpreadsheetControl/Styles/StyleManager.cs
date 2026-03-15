#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
using System.Windows.Media;
using Syncfusion.Windows.Tools.Controls;
using System.Windows.Controls;
using System.ComponentModel;
using System;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Controls.Spreadsheet
{
    /// <summary>
    /// 
    /// </summary>
    public class TabStyleManager : INotifyPropertyChanged
    {
        public TabStyleManager()
        {

        }
        
        private Brush _Background;
        public Brush Background
        {
            get { return _Background; }
            set
            {
                _Background = value;
                OnPropertyChanged("Background");
            }
        }
#if SILVERLIGHT
        private TabStripPlacement _TabStripPlacement = TabStripPlacement.Bottom;
        public TabStripPlacement TabStripPlacement
        {
            get { return _TabStripPlacement; }
            set
            {
                _TabStripPlacement = value;
                OnPropertyChanged("TabStripPlacement");
            }
        }
#else
        private Dock _Dock = Dock.Bottom;
        public Dock Dock
        {
            get { return _Dock; }
            set
            {
                _Dock = value;
                OnPropertyChanged("Dock");
            }
        }
#endif
        private bool _ShowTabItemContextMenu = true;
        public bool ShowTabItemContextMenu
        {
            get { return _ShowTabItemContextMenu; }
            set
            {
                _ShowTabItemContextMenu = value;
                OnPropertyChanged("ShowTabItemContextMenu");
            }
        }
#if !SILVERLIGHT
        private ContextMenu _TabItemContextMenu;
        public ContextMenu TabItemContextMenu
        {
            get { return _TabItemContextMenu; }
            set
            {
                _TabItemContextMenu = value;
                OnPropertyChanged("TabItemContextMenu");
            }
        }
#else
        private ContextMenuAdv _TabItemContextMenu;
        public ContextMenuAdv TabItemContextMenu
        {
            get { return _TabItemContextMenu; }
            set
            {
                _TabItemContextMenu = value;
                OnPropertyChanged("TabItemContextMenu");
            }
        }
#endif

        private TabVisualStyle _TabVisualStyle = TabVisualStyle.ExcelBlue;
        public TabVisualStyle TabVisualStyle
        {
            get
            {
                return _TabVisualStyle;
            }
            set
            {
                _TabVisualStyle = value;
                OnPropertyChanged("TabVisualStyle");
            }
        }

        private bool _IsCustomTabItemContextMenuEnabled = false;
        public bool IsCustomTabItemContextMenuEnabled
        {
            get { return _IsCustomTabItemContextMenuEnabled; }
            set
            {
                _IsCustomTabItemContextMenuEnabled = value;
                OnPropertyChanged("IsCustomTabItemContextMenuEnabled");
            }
        }

        public Brush formulaBarBackground;
        public Brush FormulaBarBackground
        {
            get { return formulaBarBackground;  }
            set
            {
                formulaBarBackground = value;
                OnPropertyChanged("FormulaBarBackground");
            }
        }

        #region INotifyPropertyChanged Members

        private void OnPropertyChanged(string PropertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
