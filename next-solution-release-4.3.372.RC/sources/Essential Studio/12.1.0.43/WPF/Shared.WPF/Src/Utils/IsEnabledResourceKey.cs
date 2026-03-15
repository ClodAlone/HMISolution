// <copyright file="IsEnabledResourceKey.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Data;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Contains <see cref="ResourceKey"/> states.
    /// </summary>
    public enum ResourceKeyState
    {
        /// <summary>
        /// Resource is enabled.
        /// </summary>
        Enabled,

        /// <summary>
        /// Resource is disabled.
        /// </summary>
        Disabled
    }

    /// <summary>
    /// Struct for Enable or Disable the resources.
    /// </summary>
    internal struct EnabledDisabledResources
    {
        /// <summary>
        /// Enabled resource.
        /// </summary>
        public object EnabledResource;

        /// <summary>
        /// Disabled resource.
        /// </summary>
        public object DisabledResource;
    }

    /// <summary>
    /// Converter to convert IsEnabled(bool) type into Resource.
    /// </summary>
    internal class IsEnabledToResourceConverter : IValueConverter
    {
        /// <summary>
        /// Resources struct.
        /// </summary>
        private EnabledDisabledResources m_resources;

        /// <summary>
        /// Initializes a new instance of the <see cref="IsEnabledToResourceConverter"/> class.
        /// </summary>
        /// <param name="resources">The resources.</param>
        internal IsEnabledToResourceConverter(EnabledDisabledResources resources)
        {
            m_resources = resources;
        }

        #region IValueConverter Members

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool bvalue = (bool)value;

            return bvalue ? m_resources.EnabledResource : m_resources.DisabledResource;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// Markup extension that provides resource accordingly <see cref="IsEnabledResourceExtension"/>.
    /// </summary>
    public class IsEnabledResourceExtension : MarkupExtension
    {
        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="IsEnabledResourceExtension"/> class.
        /// </summary>
        /// <param name="id">The id for resource.</param>
        public IsEnabledResourceExtension(string id)
        {
            ID = id;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the value that represents resource key.
        /// </summary>
        public string ID
        {
            get;
            set;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Creates <see cref="Binding"/> 
        /// that provides an object that is set as the value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">Object that can provide services for the markup extension.</param>
        /// <returns>The object value to set on the property where the extension is applied.</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            object objEnabled;
            object objDisabled;

            try
            {
                objEnabled = GetEnabledResource(serviceProvider);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Resource for enabled state can not be found.", e);
            }

            try
            {
                objDisabled = GetDisabledResource(serviceProvider);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Resource for disabled state can not be found.", e);
            }

            EnabledDisabledResources resources = new EnabledDisabledResources();
            resources.DisabledResource = objDisabled;
            resources.EnabledResource = objEnabled;

            IsEnabledToResourceConverter converter = new IsEnabledToResourceConverter(resources);

            Binding binding = new Binding();
            binding.Path = new PropertyPath(FrameworkElement.IsEnabledProperty);
            binding.RelativeSource = new RelativeSource(RelativeSourceMode.Self);
            binding.Converter = converter;

            return binding.ProvideValue(serviceProvider);
        }

        /// <summary>
        /// Gets the enabled resource.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>Object of enabled resource</returns>
        private object GetEnabledResource(IServiceProvider serviceProvider)
        {
            IsEnabledResourceKeyExtension key = new IsEnabledResourceKeyExtension(ResourceKeyState.Enabled, ID);
            StaticResourceExtension res = new StaticResourceExtension(key.ProvideValue(serviceProvider));

            return res.ProvideValue(serviceProvider);
        }

        /// <summary>
        /// Gets the disabled resource.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>Disabled resource as object.</returns>
        private object GetDisabledResource(IServiceProvider serviceProvider)
        {
            IsEnabledResourceKeyExtension key = new IsEnabledResourceKeyExtension(ResourceKeyState.Disabled, ID);
            StaticResourceExtension res = new StaticResourceExtension(key.ProvideValue(serviceProvider));

            return res.ProvideValue(serviceProvider);
        }
        #endregion
    }

    /// <summary>
    /// Markup extension for work with resources.
    /// </summary>
    public class IsEnabledResourceKeyExtension : ResourceKey
    {
        #region Private members

        /// <summary>
        /// Assembly reference.
        /// </summary>
        private System.Reflection.Assembly m_asm;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the value that represents accessibility of <see cref="ResourceKey"/>.   
        /// </summary>
        public ResourceKeyState State
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="String"/> value that represents key of resource.
        /// </summary>
        public string ID
        {
            get;
            set;
        }
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="IsEnabledResourceKeyExtension"/> class.
        /// </summary>
        public IsEnabledResourceKeyExtension()
            : this(ResourceKeyState.Disabled, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IsEnabledResourceKeyExtension"/> class.
        /// </summary>
        /// <param name="state">Given resource key state.</param>
        /// <param name="id">Given string id of resource key.</param>
        public IsEnabledResourceKeyExtension(ResourceKeyState state, string id)
        {
            ID = id;
            State = state;
        }
        #endregion

        #region Overrides

        /// <summary>
        /// Determines whether the specified <see cref="Object"/> is equal to the current <see cref="Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="Object"/> to compare with the current <see cref="Object"/>.</param>
        /// <returns>True if the specified <see cref="Object"/> is equal to the current <see cref="Object"/>; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            IsEnabledResourceKeyExtension key = (IsEnabledResourceKeyExtension)obj;

            return key.ID == ID && key.State == State;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current <see cref="Object"/>.</returns>
        public override int GetHashCode()
        {
            return ID.GetHashCode() ^ State.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="String"/> that represents the current markup extension.
        /// </summary>
        /// <returns>A <see cref="String"/> that represents the current markup extension.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("ID=");
            builder.Append(ID);
            builder.Append(";State=");
            builder.Append(State.ToString());

            return builder.ToString();
        }

        /// <summary>
        /// Returns <see cref="ResourceKey"/> that is used as a key in a dictionary.
        /// </summary>
        /// <param name="serviceProvider">Object that can provide services for the markup extension.</param>
        /// <returns>The object value to set on the property where the extension is applied.</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IUriContext context = serviceProvider.GetService(typeof(IUriContext)) as IUriContext;

            if (context != null)
            {
                Uri uri = context.BaseUri;
                string asmName = uri.Segments[1];
                int indexSeparator = asmName.IndexOf(";");

                ////System.Reflection.Assembly.
                if (indexSeparator >= 0)
                {
                    ////if( indexSeparator
                    asmName = asmName.Remove(indexSeparator);
                    System.Reflection.Assembly asm = null;

                    foreach (System.Reflection.Assembly asmCheck in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        if (asmCheck.GetName().Name == asmName)
                        {
                            asm = asmCheck;
                            break;
                        }
                    }

                    m_asm = asm;
                }
                else
                {
                    m_asm = Application.ResourceAssembly;
                }
            }

            return base.ProvideValue(serviceProvider);
        }

        /// <summary>
        /// Gets the value that represents an <see cref="Assembly"/> where is the resources is located.
        /// </summary>
        public override System.Reflection.Assembly Assembly
        {
            get
            {
                if (m_asm == null)
                {
                    throw new InvalidOperationException("Target resource assembly can not be found.");
                }

                return m_asm;
            }
        }
        #endregion
    }
}
