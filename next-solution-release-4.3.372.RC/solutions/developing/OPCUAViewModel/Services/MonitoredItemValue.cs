namespace OPCUAViewModel.Services
{
    public class MonitoredItemValue
    {
        public MonitoredItemValue(MonitoredItemViewModel viewModel, object value = null)
        {
            ViewModel = viewModel;
            Value = value;
        }

        public MonitoredItemViewModel ViewModel { get; }
        public object Value { get; }
    }
}