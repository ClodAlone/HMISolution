class _Name_{
// ***START***
/// <summary>
/// _Description_
/// </summary>
// _SerializationAttribute_
public _TypeName_ _BrowseName_
{
	get 
    { 
        return _FieldName_;  
    }
	
    set 
    { 
        _FieldName_ = value; 

        if (value == null)
        {
            _FieldName_ = _DefaultValue_;
        }
    }
}
// ***END***
}