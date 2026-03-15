using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Utilities
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public sealed class Exportable : Attribute
    {
        public string ExportPropertyName;
        [Required(ErrorMessage = "Key is required.")]
        public string[] RequiredKeys;
        public string ImportFolderInfo;
        public string ImportFolderPrototype;
        public string ImportTagOwnerInfo;
        public string ImportTagProtoNameInfo;
        public string ExternalRefInfo;
        public string ExternalRefValue;
        public string[] UseExternalRef;
        public string[] AggregatedProperties;
        public Exportable()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool IsDefaultAttribute()
        {
            return true;
        }
    }
   
}
