class _Name_{
// ***START***
#region _ChildName_
/// <summary>
/// _Description_
/// </summary>
public _ClassName_ _ChildName_
{
	get 
    {
        lock (DataLock)
        {      
            return _FieldName_; 
        }
    }

    protected set
    {
        lock (DataLock)
        {      
            if (_FieldName_ != null)
            {
                RemoveChild(_FieldName_);
            }

            _FieldName_ = value; 
        }
    }
}

/// <summary>
/// Whether the _ChildName_ node is specified for the node.
/// </summary>
public bool _ChildName_Specified
{
	get 
    { 
        lock (DataLock)
        {
            return _FieldName_ != null; 
        }
    }
}

/// <summary>
/// Specifies the optional child.
/// </summary>
public void Specify_ChildName_(_ClassName_ replacement)
{
    CheckNodeManagerState();

    lock (DataLock)
    {
        if (_ChildName_Specified)
        {
            _ChildName_ = (_ClassName_)DeleteChild(_FieldName_);
        }

        if (replacement != null)
        {       
            _ChildName_ = replacement;

            _ChildName_.Create(
                this.NodeId, 
                new NodeId(_ReferenceTypeNamespaceCodePath_.ReferenceTypes._ReferenceType_, GetNamespaceIndex(_ReferenceTypeNamespaceUri_)), 
                null,
                new QualifiedName(_BrowseNameNamespaceCodePath_.BrowseNames._BrowseName_, GetNamespaceIndex(_BrowseNameNamespaceUri_)),
                _BrowseNameNamespaceCodePath_._NodeClass_s._ParentTypeName___ChildName_,
                null);
        }
    }
}
#endregion
// ***END***
}