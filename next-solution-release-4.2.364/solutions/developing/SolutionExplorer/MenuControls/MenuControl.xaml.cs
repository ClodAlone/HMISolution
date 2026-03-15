using DevExpress.Xpf.Bars;
using DocumentManager.ComponentService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using UFInterfaces;
using UFProjectManager.ComponentService;
using UFProjectManager.MenuControls;
using Utilities;
using WPFUtilities;

namespace UFProjectManager
{
    /// <summary>
    /// Interaction logic for MenuControl.xaml
    /// </summary>
    public partial class MenuControl : BarManagerBase
    {
        #region Properties
        public List<GeneralCommand> Cmds
        {
            get;
            private set;
        } = new List<GeneralCommand>();
        #endregion Properties

        public MenuControl(IDocumentManager comp, IWorkspace ws) : base(comp, ws, new ToolbarControl())
        {
            InitializeComponent();
            ToolbarMenuItem = CommonBarItems.toolbarMenuItem;

            var docManagers = (from IDocumentManager m in (comp as UFProjectManagerComponent).UriRisolver.GetListInstalledDocumentManagers() where m != null && m != comp orderby m.TypeScheme select m).ToList();
            foreach (var docm in docManagers) {
                var docBar = new BarSubItem() { Tag = docm.TypeScheme, Content = docm.TypeTitle, MergeType = BarItemMergeType.MergeItems };
                biProjectUFProject.Items.Add(docBar);
                if (docm.isMultipleResource) //Adding "Add New" barbuttonitem as documentmanager's barsubitem first item
                {
                    var cmd = CreateAddNewCommand(docm.TypeLabel);
                    var btnItem = new BarButtonItem();
                    ToolTipService.SetShowOnDisabled(btnItem, true);
                    btnItem.Content = Properties.Resources.AddResourceText;

                    btnItem.Glyph = UFProjectManagerComponent.GetControlImage("UFPRJDocumentAdd");
                    btnItem.Command = cmd;
                    btnItem.CommandParameter = docm.TypeScheme;
                    btnItem.MergeOrder = 10;
                    docBar.Items.Insert(0, btnItem);
                    Cmds.Add(cmd);

                    if(docm is ScreenManager.ComponentService.IScreenManager)
                    {
                        cmd = UIGeneralCommands.AddNewScreenFromTemplate;
                        btnItem = new BarButtonItem();
                        ToolTipService.SetShowOnDisabled(btnItem, true);
                        btnItem.Content = Properties.UICommandResource.AddNewScreenFromTemplateName;

                        btnItem.Glyph = UFProjectManagerComponent.GetControlImage("UFPRJDocumentAdd");
                        btnItem.Command = cmd;
                        btnItem.CommandParameter = docm.TypeScheme;
                        btnItem.MergeOrder = 10;
                        docBar.Items.Insert(1, btnItem);
                        Cmds.Add(cmd);
                    }
                }
            }
        }

        protected override CommandBindingCollection GetCommandBindings()
        {
            return CommandBindings;
        }

        static GeneralCommand CreateAddNewCommand(string typeLabel)
        {
            try
            {
                ResourceSet rSet = Properties.UICommandResource.ResourceManager.GetResourceSet(System.Globalization.CultureInfo.CurrentUICulture, false, false);
                return new GeneralCommand(
                    rSet.GetString(String.Format("{0}{1}{2}", "AddNew", typeLabel, "Name")),
                    rSet.GetString(String.Format("{0}{1}{2}", "AddNew", typeLabel, "Text")),
                    rSet.GetString(String.Format("{0}{1}{2}", "AddNew", typeLabel, "Gestures")),
                    rSet.GetString(String.Format("{0}{1}{2}", "AddNew", typeLabel, "GesturesDisplayText")),
                    rSet.GetString(String.Format("{0}{1}{2}", "AddNew", typeLabel, "Tooltip")),
                    rSet.GetString(String.Format("{0}{1}{2}", "AddNew", typeLabel, "Description")),
                    typeof(UIGeneralCommands));
            }
            catch
            {
                return new GeneralCommand(
                    String.Format("{0}{1}{2}", "AddNew", typeLabel, "Text"),
                    String.Format("{0}{1}{2}", "AddNew", typeLabel, "Name"),
                    typeof(UIGeneralCommands)
                );
            }
        }
    }
}
