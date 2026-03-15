// ***START***
#region _TypeName_MethodSource Class
/// <summary>
/// Implements a method which may be used by many nodes.
/// </summary>
public partial class _TypeName_MethodSource : MethodSource
{
    #region Constructors
    /// <summary>
    /// Initializes the object with default values.
    /// </summary>
    public _TypeName_MethodSource(IServerInternal server, NodeSource parent) : base(server, parent)
    {
        Arguments = CreateArguments();
    }
    
    /// <summary>
    /// Creates a new instance of the node.
    /// </summary>
    public static new _TypeName_MethodSource Construct(
        IServerInternal server, 
        NodeSource      parent, 
        NodeId          referenceTypeId,
        NodeId          nodeId,
        QualifiedName   browseName,
        uint            numericId)
    {
        _TypeName_MethodSource instance = new _TypeName_MethodSource(server, parent);
        instance.Initialize(referenceTypeId, nodeId, browseName, numericId, null);
        return instance;
    }
    #endregion
     
    #region ICloneable Members
    /// <summary cref="NodeSource.Clone(NodeSource)" />
    public override NodeSource Clone(NodeSource parent)
    {
        lock (DataLock)
        {
            _TypeName_MethodSource clone = new _TypeName_MethodSource(Server, parent);
            clone.Initialize(this);
            return clone;
        }
    }
    #endregion

    #region Public Interface
    /// <summary>
    /// Calls the _TypeName_ method.
    /// </summary>
    public void _TypeName_(OperationContext context, NodeSource target)
    {    
        List<object> inputArguments = new List<object>();
        List<ServiceResult> argumentErrors = new List<ServiceResult>();
        List<object> outputArguments = new List<object>();
        
        // CopyInputArguments

        ServiceResult result = Call(
            context, 
            NodeId, 
            null, 
            Parent.NodeId, 
            inputArguments, 
            argumentErrors, 
            outputArguments);

        if (ServiceResult.IsBad(result))
        {
            throw new ServiceResultException(result);
        }
            
        // CopyOutputArguments
    }
    #endregion
    
    #region Protected Methods
    /// <summary>
    /// Called when the _TypeName_ method is called.
    /// </summary>
    protected override void Call(
        OperationContext     context, 
        NodeSource           target,
        Delegate             methodToCall,
        IList<object>        inputArguments,
        IList<ServiceResult> argumentErrors,
        IList<object>        outputArguments)
    {
        _TypeName_MethodHandler Callback = methodToCall as _TypeName_MethodHandler;

        if (Callback == null)
        {
            base.Call(context, target, methodToCall, inputArguments, argumentErrors, outputArguments);
            return;
        }

        // AssignInputArguments
        // DeclareOutputArguments

        // InvokeCallback
        // AssignOutputArguments
    }

    /// <summary>
    /// Creates the arguments for the _TypeName_ method.
    /// </summary>
    protected MethodArguments CreateArguments()
    {
        MethodArguments arguments = new MethodArguments();

        Argument argument = null;
        
        // InputArgumentList
        
        // OutputArgumentList

        return arguments;
    }
    #endregion
}

/// <summary>
/// A delegate used to receive notifications when the method is called.
/// </summary>
public delegate void _TypeName_MethodHandler();
#endregion
// ***END***
