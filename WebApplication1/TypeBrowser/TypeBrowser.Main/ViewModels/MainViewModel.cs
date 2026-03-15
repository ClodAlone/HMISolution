using DevExpress.Mvvm.POCO;

namespace TypeBrowser.Main.ViewModels
{
    public class MainViewModel
    {
        public static MainViewModel Create()
        {
            return ViewModelSource.Create(() => new MainViewModel());
        }
    }
}
