class _Name_{
// ***START***
#region Variable/DataType Conversion Functions.
/// <summary cref="VariableSource.UpdateValueFromChild" />
public override void UpdateValueFromChild(VariableSource child)
{
    lock (DataLock)
    {
        _DataType_ value = RawValue;

        if (value == null)
        {
            value = new _DataType_();
        }

        switch (child.NumericId)
        {
            // ListOfChildrenToRead

            default:
            {
                return;
            }
        }

        // triggers a data change.
        Value = value;
    }
}

/// <summary cref="VariableSource.UpdateChildrenFromValue" />
public override void UpdateChildrenFromValue()
{
    lock (DataLock)
    {
        _DataType_ value = RawValue;

        if (value == null)
        {
            RawValue = value = new _DataType_();
        }

        // ListOfChildrenToWrite
    }
}
#endregion
// ***END***
}