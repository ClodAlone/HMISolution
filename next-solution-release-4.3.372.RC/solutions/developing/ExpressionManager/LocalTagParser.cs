using System;
using OPCUAViewModel;

namespace ExpressionManager
{
    public class LocalTagParser : ILocalTagParser
    {
        public OPCUAEntityReference Parse(string tag)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tag))
                    return null;

                var splitVariable = tag.Split('.');
                if (splitVariable.Length != 2)
                    return null;

                var appName = splitVariable[0];
                var dataSinkInterface = OPCUAEntityReference.GetDataSinkInterface(appName);
                if (dataSinkInterface == null)
                    return null;

                var folderSeparator = dataSinkInterface.FolderSeparator;
                var tagName = splitVariable[1].Replace("\\", folderSeparator);
                return dataSinkInterface.GetReference(tagName);
            }
            catch (Exception _)
            {
                return null;
            }
        }
    }
}