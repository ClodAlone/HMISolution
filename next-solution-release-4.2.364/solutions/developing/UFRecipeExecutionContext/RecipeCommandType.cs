using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Utilities.Converters;

namespace UFRecipeExecutionContext
{
#if !WINDOWS_UWP && !NET_STANDARD
    [TypeConverter(typeof(LocalizedEnumConverter))]
#endif
    public enum RecipeCommandType
    {
        Show,
        Load,
        Save,
        Remove,
        Activate,
        Read,
        Export,
        Import
    }

    public class LocalizedEnumConverter : ResourceEnumConverter
    {
        public LocalizedEnumConverter(Type type)
            : base(type, Properties.Resources.ResourceManager)
        {

        }
    }
}
