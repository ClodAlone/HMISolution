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
            if (_ChildName_Replaced)
            {
                return _FieldName_;
            }

            if (m_typeDefinition != null)
            {
                return m_typeDefinition._ChildName_;
            }

            return null;
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
/// Whether the shared _ChildName_ node has been replaced for node.
/// </summary>
public bool _ChildName_Replaced
{
	get 
    { 
        lock (DataLock)
        {
            return _FieldName_ != null && _FieldName_.Parent == this; 
        }
    }
}

/// <summary>
/// Replaces the shared child with another node.
/// </summary>
public void Replace_ChildName_(_ClassName_ replacement)
{
    CheckNodeManagerState();

    lock (DataLock)
    {
        if (_ChildName_Replaced)
        {
            _ChildName_ = (_ClassName_)DeleteReplacedChild(_FieldName_);
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