class _Name_ {
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
/// Calls the _BrowseName_ method.
/// </summary>
public void _BrowseName_(OperationContext context)
{     
    lock (DataLock)
    {     
        Call(context, this);
    }
}
    
/// <summary>
/// Sets the callback to use when the _BrowseName_ method is called.
/// </summary>
public void Set_BrowseName_Callback(_MethodName_MethodHandler callback)
{
    lock (DataLock)
    {  
        _ChildName_.SetCallback(this, callback);
    }
}
#endregion
// ***END***
}