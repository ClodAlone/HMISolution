using DocumentManager.ComponentService;
using ScriptManager.MenuControls;
using System.Windows.Input;
using UFInterfaces;
using WPFUtilities;

namespace ScriptManager
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
    }
}
