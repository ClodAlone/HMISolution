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
/// Replaces the child with another node.
/// </summary>
public void Replace_ChildName_(_ClassName_ replacement)
{
    if (replacement == null) throw new ArgumentNullException("replacement");
    
    CheckNodeManagerState();

    lock (DataLock)
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
#endregion
// ***END***
}