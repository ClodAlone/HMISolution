using UFWinRTClient.DataModel;
using DevExpress.Core;
using System.Collections.Generic;

namespace UFWinRTClient.ViewModel
{
    //A View Model for a GroupedItemsPage
    public class GroupedItemsViewModel : BindableBase, ISupportSaveLoadState
    {
        IEnumerable<SampleDataGroup> groups;
        public GroupedItemsViewModel() { }
        public IEnumerable<SampleDataGroup> Groups
        {
            get { return groups; }
            private set { SetProperty<IEnumerable<SampleDataGroup>>(ref groups, value); }
        }

        void ISupportSaveLoadState.LoadState(object navigationParameter, PageStateStorage pageState)
        {
            Groups = SampleDataSource.GetGroups((string)navigationParameter);
        }

        void ISupportSaveLoadState.SaveState(PageStateStorage pageState)
        {
        }
    }
}
