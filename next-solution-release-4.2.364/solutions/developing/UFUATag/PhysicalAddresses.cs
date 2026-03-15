using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UFUAModel
{
    public class PhysicalAddresses : Dictionary<string, object>
    {
        public override bool Equals(Object obj)
        {
            if (this == obj)
                return true;

            var dictionary1 = this as IDictionary<string, object>;
            var dictionary2 = obj as IDictionary<string, object>;

            if (dictionary1 == null || dictionary2 == null || dictionary1.Count != dictionary2.Count)
                return false;

            foreach (var driverName in dictionary1.Keys)
            {
                if (!dictionary2.ContainsKey(driverName))
                    return false;

                var driverDynamicSettings1 = dictionary1[driverName] as DriverDynamicSettings;
                var driverDynamicSettings2 = dictionary2[driverName] as DriverDynamicSettings;

                if (driverDynamicSettings1 == null || driverDynamicSettings2 == null)
                    return false;

                var dynamicSettings1 = String.IsNullOrWhiteSpace(driverDynamicSettings1.DynamicSettingsForEditing) ? null : driverDynamicSettings1.DynamicSettingsForEditing;
                var dynamicSettings2 = String.IsNullOrWhiteSpace(driverDynamicSettings2.DynamicSettingsForEditing) ? null : driverDynamicSettings2.DynamicSettingsForEditing;
                if (dynamicSettings1 != dynamicSettings2)
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
