using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Client;

namespace WpfApp3.ViewModels
{
    public class OpcNodeViewModel : ViewModelBase
    {
        private readonly Session _session;

        public Session Session => _session;

        private bool _isExpanded;
        private bool _isSelected;

        public OpcNodeViewModel(ReferenceDescription reference, Session session)
        {
            _session = session;
            Reference = reference;
            DisplayName = reference.DisplayName.Text;
            NodeId = ExpandedNodeId.ToNodeId(reference.NodeId, session.NamespaceUris);

            // Add dummy item to support expansion
            // Use a specialized dummy instance
            Children.Add(new OpcNodeViewModel()); 
        }

        // Constructor for dummy item
        private OpcNodeViewModel()
        {
            _session = null!; 
            Reference = null!;
            DisplayName = "Loading...";
            NodeId = null!;
        }

        public string DisplayName { get; set; }
        public NodeId NodeId { get; }
        public ReferenceDescription Reference { get; }
        
        public string NodeClass => Reference?.NodeClass.ToString() ?? "";
        public string BrowseName => Reference?.BrowseName.ToString() ?? "";
        public string TypeDefinition => Reference?.TypeDefinition.ToString() ?? "";

        public ObservableCollection<OpcNodeViewModel> Children { get; } = new();

        public string Icon
        {
            get
            {
                if (Reference == null) return "\uE12B"; // Loading/Refresh

                switch (Reference.NodeClass)
                {
                    case Opc.Ua.NodeClass.Object:
                        return "\uE8B7"; // Folder
                    case Opc.Ua.NodeClass.Variable:
                        return "\uEA37"; // Tag
                    case Opc.Ua.NodeClass.Method:
                        return "\uE768"; // Play
                    case Opc.Ua.NodeClass.ObjectType:
                    case Opc.Ua.NodeClass.VariableType:
                    case Opc.Ua.NodeClass.DataType:
                    case Opc.Ua.NodeClass.ReferenceType:
                        return "\uE943"; // Type
                    case Opc.Ua.NodeClass.View:
                        return "\uE890"; // Eye
                    default:
                        return "\uE9CE"; // Unknown
                }
            }
        }

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (SetProperty(ref _isExpanded, value) && value)
                {
                    _ = LoadChildrenAsync();
                }
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (SetProperty(ref _isSelected, value) && value)
                {
                    WpfApp3.Services.SelectionService.Instance.Select(this);
                }
            }
        }

        private bool _childrenLoaded = false;

        public async Task LoadChildrenAsync()
        {
            if (_childrenLoaded || _session == null) return;

            try
            {
                var references = await Task.Run(() =>
                {
                    if (!_session.Connected)
                    {
                        return new ReferenceDescriptionCollection();
                    }

                    var browser = new Browser(_session)
                    {
                        BrowseDirection = BrowseDirection.Forward,
                        ReferenceTypeId = ReferenceTypeIds.HierarchicalReferences,
                        IncludeSubtypes = true,
                        NodeClassMask = 0,
                        ContinueUntilDone = false,
                    };

                    return browser.Browse(NodeId);
                });

                // Update UI on main thread
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    Children.Clear();
                    foreach (var rd in references)
                    {
                        Children.Add(new OpcNodeViewModel(rd, _session));
                    }
                });

                _childrenLoaded = true;
            }
            catch (System.Exception ex)
            {
                // Handle errors
                System.Diagnostics.Debug.WriteLine($"Error browsing {NodeId}: {ex.Message}");
            }
        }
    }
}
