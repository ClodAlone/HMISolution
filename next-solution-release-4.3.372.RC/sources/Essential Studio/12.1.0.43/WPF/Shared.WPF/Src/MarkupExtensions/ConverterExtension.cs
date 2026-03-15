
// <copyright file="ConverterExtension.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Represents markup extension for work with <see cref="IValueConverter"/> objects.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [MarkupExtensionReturnType(typeof(IValueConverter))]
    public class ConverterExtension : MarkupExtension
    {       
        #region Private members

        /// <summary>
        /// Type name for conversion
        /// </summary>        
        private string m_typeName;

        /// <summary>
        /// Declare the arguments
        /// </summary>
        private object[] m_arguments;

        /// <summary>
        /// Declare the converters
        /// </summary>
        private static Dictionary<string, IValueConverter> s_converters = new Dictionary<string, IValueConverter>();

        #endregion

        #region Initialization
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ConverterExtension"/> class.
        /// </summary>
        /// <param name="type">The type of converter.</param>
        public ConverterExtension(string type)
        {
            if (type == null || type.Length == 0)
            {
                throw new ArgumentNullException("type");
            }

            m_typeName = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConverterExtension"/> class.
        /// </summary>
        /// <param name="type">The type of converter.</param>
        /// <param name="args">The args of converter.</param>
        public ConverterExtension(string type, params object[] args)
            : this(type)
        {
            m_arguments = args;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConverterExtension"/> class.
        /// </summary>
        /// <param name="type">The type of converter.</param>
        /// <param name="arg1">The arg1 of converter.</param>
        public ConverterExtension(string type, object arg1) : this(type)
        {
            m_arguments = new object[] { arg1 };
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Provides converter.
        /// </summary>
        /// <param name="serviceProvider">Object that provides services for the markup extension.</param>
        /// <returns>Returns <see cref="IValueConverter"/> object.</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            lock (s_converters)
            {
                if (s_converters.ContainsKey(m_typeName))
                {
                    return s_converters[m_typeName];
                }
            }

            IXamlTypeResolver service = serviceProvider.GetService(typeof(IXamlTypeResolver)) as IXamlTypeResolver;

            if (service == null)
            {
                throw new InvalidOperationException("IXamlTypeResolver service not found.");
            }

            Type type = service.Resolve(m_typeName);

            if (type == null)
            {
                throw new InvalidOperationException("The specified type can not be found: " + m_typeName);
            }

            if (type.IsSubclassOf(typeof(IValueConverter)))
            {
                throw new InvalidOperationException("The specified type does not inherit from IValueConverter: " + m_typeName);
            }

            object objConverter = (m_arguments != null) ? Activator.CreateInstance(type, m_arguments) : Activator.CreateInstance(type);
            IValueConverter converter = objConverter as IValueConverter;

            if (converter == null)
            {
                throw new InvalidOperationException("The specified converter can not be created: " + m_typeName);
            }

            lock (s_converters)
            {
                s_converters[m_typeName] = converter;
            }

            return converter;
        }

        #endregion
    }
}
