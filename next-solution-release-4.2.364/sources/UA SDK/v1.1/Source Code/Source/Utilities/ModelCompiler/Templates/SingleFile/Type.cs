// ***START***
#region _TypeName_ Class
/// <summary>
/// Represents the _BrowseName_ _NodeClass_ in the address space.
/// </summary>
/// <exclude />
[System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
public partial class _TypeName_<NewT> : _NodeClass_Source<BaseT>
{
    #region Constructors
    /// <summary>
    /// Initializes the object with default values.
    /// </summary>
    public _TypeName_(IServerInternal server) : base(server)
    {
        Initialize(
            new NodeId(_NamespaceCodePath_._NodeClass_s._BrowseName_, GetNamespaceIndex(_NamespaceUri_)),
            new QualifiedName(_BrowseNameNamespaceCodePath_.BrowseNames._BrowseName_, GetNamespaceIndex(_BrowseNameNamespaceUri_)),
            new NodeId(_BaseTypeNamespaceCodePath_._NodeClass_s._BaseType_, GetNamespaceIndex(_BaseTypeNamespaceUri_)));
                
        server.TypeSources.SetTypeSource(this.NodeId, this);
    }

    /// <summary>
    /// Finds the source for the type definition (creates it if it does not exist).
    /// </summary>
    public static new _TypeName_<NewT> FindSource(IServerInternal server)
    {
        _TypeName_<NewT> type = null;
                
        lock (server.TypeSources.SyncRoot)
        {
            NodeId typeId = new NodeId(_NamespaceCodePath_._NodeClass_s._BrowseName_, server.NamespaceUris.GetIndexOrAppend(_NamespaceUri_));

            type = server.TypeSources.FindTypeSource(typeId) as _TypeName_<NewT>;

            if (type != null)
            {
                return type;
            }

            type = new _TypeName_<NewT>(server);
        }

        return type;
    }
    #endregion
         
    #region ICloneable Members
    /// <summary cref="NodeSource.Clone(NodeSource)" />
    public override NodeSource Clone(NodeSource parent)
    {
        lock (DataLock)
        {
            _TypeName_<NewT> clone = new _TypeName_<NewT>(Server);
            clone.Initialize(this);
            return clone;
        }
    }
    #endregion
  
    #region Public Properties
    // ListOfChildrenForType
    #endregion
    // DeclareMethods

    #region Overridden Methods
    /// <summary cref="NodeSource.Initialize(NodeSource)" />
    public override void Initialize(NodeSource source)
    {
        lock (DataLock)
        {
            base.Initialize(source);
            
            _TypeName_<NewT> type = source as _TypeName_<NewT>;

            // ListOfCloneChildForType
        }
    }

    /// <summary cref="NodeSource.InitializeChildren" />
    protected override void InitializeChildren()
    {
        base.InitializeChildren();
            
        // ListOfFieldInitializersForType
    }
    #endregion

    #region Private Fields
    // ListOfFieldsForType
    #endregion
}
#endregion

#region _ClassName_ Class
/// <summary>
/// Represents an instance of the _BrowseName_ _NodeClass_ in the address space.
/// </summary>
/// <exclude />
[System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
public partial class _ClassName_<NewT> : _BaseClass_
{
    #region Constructors
    /// <summary>
    /// Initializes the object with default values.
    /// </summary>
    protected _ClassName_(IServerInternal server, NodeSource parent) 
    : 
        base(server, parent)
    {
        m_typeDefinition = _TypeName_<NewT>.FindSource(server);
    }

    /// <summary>
    /// Creates a new instance of the node.
    /// </summary>
    public static new _ClassName_<NewT> Construct(
        IServerInternal server, 
        NodeSource      parent, 
        NodeId          referenceTypeId,
        NodeId          nodeId,
        QualifiedName   browseName,
        uint            numericId)
    {
        _ClassName_<NewT> instance = new _ClassName_<NewT>(server, parent);
        instance.Initialize(referenceTypeId, nodeId, browseName, numericId, instance.m_typeDefinition.NodeId);
        return instance;
    }

    /// <summary>
    /// Creates a new instance of the node without any parent.
    /// </summary>
    public static new _ClassName_<NewT> Construct(IServerInternal server)
    {
        _ClassName_<NewT> instance = new _ClassName_<NewT>(server, (NodeSource)null);
        instance.Initialize(null, null, null, 0, instance.m_typeDefinition.NodeId);
        return instance;
    }
    #endregion
       
    #region ICloneable Members
    /// <summary cref="NodeSource.Clone(NodeSource)" />
    public override NodeSource Clone(NodeSource parent)
    {
        lock (DataLock)
        {
            _ClassName_<NewT> clone = new _ClassName_<NewT>(Server, parent);
            clone.Initialize(this);
            return clone;
        } 
    }
    #endregion

    #region Public Properties
    // ListOfChildren
    #endregion

    #region Overridden Methods
    /// <summary cref="NodeSource.Initialize(NodeSource)" />
    public override void Initialize(NodeSource source)
    {
        lock (DataLock)
        {            
            _ClassName_<NewT> instance = source as _ClassName_<NewT>;

            if (instance != null)
            {
                base.Initialize(source);
            }

            _TypeName_<NewT> type = source as _TypeName_<NewT>;

            // ListOfCloneChild
        }
    }

    /// <summary cref="NodeSource.InitializeChildren" />
    protected override void InitializeChildren()
    {
        base.InitializeChildren();
            
        // ListOfFieldInitializers
    }
    // CreateChildren
    // UpdateChildren
    #endregion

    #region Private Fields
    private _TypeName_<NewT> m_typeDefinition;
    // ListOfFields
    #endregion
}
#endregion
// ***END***
