using System;
using System.ComponentModel;
using UFUAModel.Converters;

namespace UFUAModel
{
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum ThreeStateType
    {
        UseDefault,
        ForceFalse,
        ForceTrue
    }
}
