using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Client;
using ServerEditor.Services;

namespace ServerEditor.ViewModels
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
        
        public static OpcNodeViewModel CreateError(string message)
        {
            var model = new OpcNodeViewModel();
            model.DisplayName = message;
            return model;
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
                    SelectionService.Instance.Select(this);
                }
            }
        }

        private bool _childrenLoaded = false;

        public async Task LoadChildrenAsync()
        {
            if (_childrenLoaded || _session == null || !_session.Connected) return;

            try
            {
                var refs = await Task.Run(() =>
                {
                    var browser = new Browser(_session);
                    browser.BrowseDirection = BrowseDirection.Forward;
                    browser.ReferenceTypeId = ReferenceTypeIds.HierarchicalReferences;
                    browser.IncludeSubtypes = true;
                    browser.NodeClassMask = (int)(Opc.Ua.NodeClass.Object | Opc.Ua.NodeClass.Variable | Opc.Ua.NodeClass.Method);
                    browser.ResultMask = (uint)BrowseResultMask.All;

                    return browser.Browse(NodeId);
                });

                // Clear dummy
                Children.Clear();

                foreach (var refDesc in refs)
                {
                    Children.Add(new OpcNodeViewModel(refDesc, _session));
                }
                
                _childrenLoaded = true;
            }
            catch (ServiceResultException se)
            {
                System.Diagnostics.Debug.WriteLine($"Browse Error: {se.Message}");
            }
        }
    }
}