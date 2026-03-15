using System.Windows.Input;
using WPFUtilities;

namespace ReportManager
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MenuControl : BarManagerBase
    {
        public MenuControl()
        {
            InitializeComponent();
        }

        protected override CommandBindingCollection GetCommandBindings()
        {
            return CommandBindings;
        }

        //public void OnAddNewReport_Click(object sender, ItemClickEventArgs e)
        //{
        //    var pm = comp.ProjectManager;
        //    if (pm != null)
        //    {
        //        var activeProject = pm.GetActiveProjects().FirstOrDefault();
        //        if (activeProject != null)
        //        {
        //            Uri path = new Uri(activeProject);
        //            var doc = (pm as IDocumentManager).GetDocument(path);
        //            comp.Edit(path, doc);
        //        }
        //    }
        //}
    }
}
