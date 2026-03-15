using DocumentManager.ComponentService;
using Mindscape.WpfElements.PropertyEditing;
using ScreenManager.ComponentService;
using StringManager.ComponentService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using UFInterfaces;
using UFInterfaces.Editors;
using Utilities;
using Utilities.WPF;

namespace WPFUtilities.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for TextPropertyEditor.xaml
    /// </summary>
    public partial class StringFormatPropertyEditor : UserControl
    {
        #region Declarations
        IScreenManager screenManager;
        bool bDirty;
        bool bInit;
        bool bIsToolTip;
        #endregion

        #region Constructors

        public StringFormatPropertyEditor()
        {
            InitializeComponent();
        }

        #endregion
    }
}
