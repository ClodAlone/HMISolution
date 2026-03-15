class _Name_{ void X() {
// ***START***      
_ChildName_ = (_ClassName_)InitializeSharedChild(
    (m_typeDefinition != null)?m_typeDefinition._ChildName_:null,
    new ConstructInstanceDelegate(_ClassName_.Construct), 
    new NodeId(_ReferenceTypeNamespaceCodePath_.ReferenceTypes._ReferenceType_, GetNamespaceIndex(_ReferenceTypeNamespaceUri_)), 
    new QualifiedName(_BrowseNameNamespaceCodePath_.BrowseNames._BrowseName_, GetNamespaceIndex(_BrowseNameNamespaceUri_)),
    _BrowseNameNamespaceCodePath_._NodeClass_s._ParentTypeName___ChildName_,
    configuration);
// ***END***
}}