using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Utilities;

namespace OPCUAViewModel.Converters
{
    public class UriConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI. 
        /// </summary>
        /// <param name="value">The source data being passed to the target </param>
        /// <param name="targetType">The Type of data expected by the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>The value to be passed to the target dependency property. </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //if (targetType != typeof(String))
            //    throw new InvalidOperationException("The target must be a String");
            try
            {
                if (value != null && value is OPCUAEntityReference)
                {
                    OPCUAEntityReference uri = value as OPCUAEntityReference;
                    //return uri.HumanReadable;
                    //return OpcuaEntityReference.HumanReadable;
                    // if (uri.ReadablePath == null || uri.AppName == null || String.IsNullOrEmpty(uri.ReadablePath))
                    if (uri.ReadablePath != null && uri.ReadablePath.Contains("&") && !String.IsNullOrEmpty(uri.AppName))
                        return string.Format("{0} ({1})", (uri.ReadablePath).Replace('&', '\\'), uri.AppName);
                    else
                        return uri.StringRepresentationWithProject;

                    /*
                    if (uri.ResolvedNodeId == null && !String.IsNullOrEmpty(uri.RelativePath) &&
                        !uri.RelativePath.Contains(":"))
                        return uri.RelativePath;

                    //return OpcuaEntityReference.HumanReadable;
                    Opc.Ua.NamespaceTable n = new Opc.Ua.NamespaceTable();
                    UInt16 ns = (UInt16)(n.Count + 2 - 1);
                    string oldChars = string.Format("{0}:", ns);
                    return string.Format("{0} ({1})", (uri.ReadablePath).Replace(oldChars, "").Replace('&', '\\'), uri.AppName);
                    */
                }
                else if (value != null && value is OPCUAXMLEntityReference)
                {
                    OPCUAXMLEntityReference xmluri = value as OPCUAXMLEntityReference;
                    OPCUAEntityReference uri = xmluri.TagReference;
                    //return uri.HumanReadable;
                    //return OpcuaEntityReference.HumanReadable;
                    //if (uri.ReadablePath == null || uri.AppName == null || String.IsNullOrEmpty(uri.ReadablePath))
                    //    return uri.HumanReadable;

                    if (uri.ReadablePath != null && uri.ReadablePath.Contains("&") && !String.IsNullOrEmpty(uri.AppName))
                        return string.Format("{0} ({1})", (uri.ReadablePath).Replace('&', '\\'), uri.AppName);
                    else
                        return uri.StringRepresentationWithProject;

                    //if (uri.ResolvedNodeId == null && !String.IsNullOrEmpty(uri.RelativePath))
                    //    return uri.RelativePath;

                    ////return OpcuaEntityReference.HumanReadable;
                    //Opc.Ua.NamespaceTable n = new Opc.Ua.NamespaceTable();
                    //UInt16 ns = (UInt16)(n.Count + 2 - 1);
                    //string oldChars = string.Format("{0}:", ns);
                    //return string.Format("{0} ({1})", (uri.ReadablePath).Replace(oldChars, "").Replace('&', '\\'), uri.AppName);
                }
                else if (value != null && value is string)
                {
                    try
                    {
                        string xml = value as string;
                        if (!string.IsNullOrEmpty(xml))
                        {
                            OPCUAEntityReference uri = null;
                            uri = xml.FromXml<OPCUAEntityReference>();
                            if(uri != null)
                            {
                                if (uri.ReadablePath != null && uri.ReadablePath.Contains("&") && !String.IsNullOrEmpty(uri.AppName))
                                    return string.Format("{0} ({1})", (uri.ReadablePath).Replace('&', '\\'), uri.AppName);
                                else
                                    return uri.StringRepresentationWithProject;
                                ////return uri.HumanReadable;
                                ////return OpcuaEntityReference.HumanReadable;
                                //if (uri.ReadablePath == null || uri.AppName == null || String.IsNullOrEmpty(uri.ReadablePath))
                                //    return uri.HumanReadable;

                                ////return OpcuaEntityReference.HumanReadable;
                                //Opc.Ua.NamespaceTable n = new Opc.Ua.NamespaceTable();
                                //UInt16 ns = (UInt16)(n.Count + 2 - 1);
                                //string oldChars = string.Format("{0}:", ns);
                                //return string.Format("{0} ({1})", (uri.ReadablePath).Replace(oldChars, "").Replace('&', '\\'), uri.AppName);
                            }
                            else
                                return String.Empty;
                        }
                        else
                            return String.Empty;
                    }
                    catch (Exception)
                    {
                        return String.Empty;
                    }
                }

                return String.Empty;

            }
            catch
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object. This method is called only in TwoWay bindings. 
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The Type of data expected by the source object.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic. </param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>The value to be passed to the source object.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
