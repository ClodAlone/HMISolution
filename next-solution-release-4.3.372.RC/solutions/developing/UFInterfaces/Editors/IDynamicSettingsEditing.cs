using System;
using System.Collections.Generic;

namespace UFInterfaces.Editors
{
    public interface IDynamicSettingsEditing
    {
        string Name { get; }
        string FolderPath { get; }
        string TagOwnerPath { get; }
        bool IsMethod { get; }
        bool IsObjectType { get; }
        string DynamicSettingsForEditing { get; set; }
        int DataType { get; set; }
        // FOGBUGZ 9836
        uint ArrayDimension { get; set; }
        IList<IDynamicSettingsEditing> Members { get; }
    }
}
