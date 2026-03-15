using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if !NET_STANDARD
using System.Windows;
#endif
#if !WINDOWS_UWP
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Xml;
#endif

namespace Utilities
{
    /// <remark>
    /// This helper is intended to provide the base for a helper,
    /// which simplifies use of the Application.Properties property.
    /// The obvious next step in extending the class is to add
    /// argument validation.
    /// </remark>
    public static class ApplicationPropertiesHelper
    {
        static Dictionary<Object, Object> mapProperties = new Dictionary<object, object>();

#if !WINDOWS_UWP
        #region IsolatedStorage
        
        public static void SaveApplicationProperties(IsolatedStorageFile isoStorage, String storeFileName)
        {
            try
            {
                if (null == isoStorage)
                    return;

                using (var stream = new IsolatedStorageFileStream(storeFileName, FileMode.Create, isoStorage))
                {
                    var settings = new XmlWriterSettings
                    {
                        Indent = true,
                        OmitXmlDeclaration = false,
                        Encoding = Encoding.UTF8
                    };

                    var map = new Dictionary<object, object>();
#if !NET_STANDARD
                    if (Application.Current != null)
                    {
                        foreach (var name in Application.Current.Properties.Keys)
                        {
                            if (Application.Current.Properties[name].GetType().IsSerializable)
                                map[name] = Application.Current.Properties[name];
                        }
                    }
                    else
#endif
                    {
                        foreach (var name in mapProperties.Keys)
                        {
                            if (mapProperties[name].GetType().IsSerializable)
                                map[name] = mapProperties[name];
                        }
                    }
                    
                    using (var writer = XmlWriter.Create(stream, settings))
                    {
                        try
                        {
                            var serializer = new DataContractSerializer(typeof(Dictionary<object, object>));
                            serializer.WriteObject(writer, map);
                        }
                        catch (Exception ex)
                        {
                            writer.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static void LoadApplicationProperties(IsolatedStorageFile isoStorage, String storeFileName)
        {
            try
            {
                if (null == isoStorage)
                    return;

                using (var stream = new IsolatedStorageFileStream(storeFileName, FileMode.OpenOrCreate, isoStorage))
                {
                    XmlReaderSettings settings = new XmlReaderSettings
                    {
                        ConformanceLevel = ConformanceLevel.Document,
                        CloseInput = true
                    };

                    using (var reader = XmlReader.Create(stream, settings))
                    {
                        var serializer = new DataContractSerializer(typeof(Dictionary<object, object>));
                        var map = serializer.ReadObject(reader) as Dictionary<object, object>;
#if !NET_STANDARD
                        if (Application.Current != null)
                        {
                            foreach (var name in map.Keys)
                                Application.Current.Properties[name] = map[name];
                        }
                        else
#endif
                        {
                            foreach (var name in map.Keys)
                                mapProperties[name] = map[name];
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
#endregion
#endif

        /// <summary>
        /// Tries to retrieve a property from the Application's Properties
        /// collection. If the object with the specified key cannot be found,
        /// the default value for the supplied type is returned.
        /// </summary>
        /// <typeparam name="T">The type of object to retrieve.</typeparam>
        /// <param name="key">The key with which the object was stored.</param>
        /// <returns>If the specified key exists, then the associated
        /// value is returned, otherwise the default value for the
        /// specified type.</returns>
        public static T GetProperty<T>(object key)
        {
            return GetProperty<T>(key, default(T));
        }

        /// <summary>
        /// Tries to retrieve a property from the Application's Properties
        /// collection. If the object with the specified key cannot be found,
        /// the default value for the supplied type is returned.
        /// </summary>
        /// <typeparam name="T">The type of object to retrieve.</typeparam>
        /// <param name="key">The key with which the object was stored.</param>
        /// <param name="defaultValue">The default value for if the specified key does not exist.</param>
        /// <returns>If the specified key exists, then the associated
        /// value is returned, otherwise the default value.</returns>
        public static T GetProperty<T>(object key, T defaultValue)
        {
#if WINDOWS_UWP || NET_STANDARD
            if (mapProperties.ContainsKey(key)
                && mapProperties[key] is T)
            {
                return (T)mapProperties[key];
            }
#else
            if (Application.Current != null && Application.Current.Properties.Contains(key)
                && Application.Current.Properties[key] is T)
            {
                return (T)Application.Current.Properties[key];
            }
            else if (mapProperties.ContainsKey(key)
                && mapProperties[key] is T)
                return (T)mapProperties[key];
#endif
            return defaultValue;
        }

        /// <summary>
        /// Retrieves the property associated with the given key.
        /// </summary>
        /// <param name="key">The key with which the object was stored.</param>
        /// <returns>If the specified key exists, the associated
        /// value is returned, otherwise the return value is null.</returns>
        public static object GetProperty(object key)
        {
#if WINDOWS_UWP || NET_STANDARD
            if (mapProperties.ContainsKey(key))
            {
                return mapProperties[key];
            }
#else
            if (Application.Current == null)
                return mapProperties[key];
            else if (Application.Current.Properties.Contains(key))
            {
                return Application.Current.Properties[key];
            }
#endif
            return null;
        }

        /// <summary>
        /// Adds a value to the Application's properties collection,
        /// indexed by the supplied key.
        /// </summary>
        /// <param name="key">
        /// The key against which the value should be stored.</param>
        /// <param name="value">The value to be stored.</param>
        public static void SetProperty(object key, object value)
        {
#if WINDOWS_UWP || NET_STANDARD
            mapProperties[key] = value;
#else
            if (Application.Current == null)
                mapProperties[key] = value;
            else
                Application.Current.Properties[key] = value;
#endif
        }
    }
}
