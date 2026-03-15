using DocumentManager.ComponentService;
using System.Windows.Input;
using UFInterfaces;
using UFUAEditor.MenuControls;
using WPFUtilities;
using System.Collections.Generic;

namespace UFUAEditor
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MenuControl : BarManagerBase
    {
        public MenuControl(IDocumentManager comp, IWorkspace ws) : base(comp, ws, new ToolbarControl())
        {
            InitializeComponent();
            ToolbarMenuItem = menuItems.toolbarMenuItem;
        }

        protected override CommandBindingCollection GetCommandBindings()
        {
            return CommandBindings;
        }

        public ToolbarControl GetToolbar()
        {
            return Toolbar as ToolbarControl;
        }
    }
}
