using UFWinRTClient.DataModel;
using DevExpress.Core;
using DevExpress.UI.Xaml.Layout;

namespace UFWinRTClient.ViewModel
{
    //A View Model for a GroupDetailPage
    public class GroupDetailViewModel : BindableBase, ISupportSaveLoadState
    {
        SampleDataGroup group;
        public GroupDetailViewModel() { }
        public SampleDataGroup Group
        {
            get { return group; }
            private set { SetProperty<SampleDataGroup>(ref group, value); }
        }

        void ISupportSaveLoadState.LoadState(object navigationParameter, PageStateStorage storage)
        {
            SampleDataGroup group = SampleDataSource.GetGroup((string)navigationParameter);
            Group = SampleDataSource.GetGroup((string)navigationParameter);
        }

        void ISupportSaveLoadState.SaveState(PageStateStorage storage)
        {
        }
    }
}
