#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#region File Using
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
#endregion

namespace Syncfusion.Windows.Forms.ComponentModel
{
    public abstract class CustomPropertiesTypeConverter : TypeConverter
    {
        #region Class Static Members
        /// <summary>Empty attributes array for optimization.</summary>
		protected static readonly Attribute[] EmptyAttributes = new Attribute[0];
        #endregion

		#region Class Initialize/Finalize Methods
		/// <summary>Hide default constructor. Allow to see it only inheritors.</summary>
		protected CustomPropertiesTypeConverter()
		{
		}
		#endregion

        #region Class Overrides
        protected abstract Attribute[] GetPropertyAttributes(Component component, PropertyDescriptor property);

        public override bool GetPropertiesSupported( ITypeDescriptorContext context )
        {
            return true;
        }

        public override PropertyDescriptorCollection GetProperties( ITypeDescriptorContext context, object value, Attribute[] filter )
        {
            if( value != null )
            {
                Component componet = ( Component )value;
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties( value, true );
                ArrayList list = new ArrayList( properties.Count );

                foreach( PropertyDescriptor property in properties )
                {
                    Attribute[] propertyAttributes = GetPropertyAttributes( componet, property );
                    AttributeCollection attributeCollection = FromExisting( property.Attributes, propertyAttributes );

                    if( attributeCollection.Contains( filter ) )
                    {
                        Attribute[] attrs = new Attribute[ attributeCollection.Count ];
                        attributeCollection.CopyTo( attrs, 0 );
                        
                        list.Add( new CustomPropertyDescriptor( property, attrs ) );
                    }
                }

                PropertyDescriptor[] pd = ( PropertyDescriptor[] )list.ToArray( typeof( PropertyDescriptor ) );

                return new PropertyDescriptorCollection( pd );
            }

            return TypeDescriptor.GetProperties( typeof( Component ), filter, true );
        }

        private AttributeCollection FromExisting( AttributeCollection existing, params Attribute[] newAttributes )
        {
            if( existing == null )
                throw new ArgumentNullException( "existing" );

            if( newAttributes == null )
            {
                newAttributes = new Attribute[ 0 ];
            }

            Attribute[] array = new Attribute[ existing.Count + newAttributes.Length ];
            int length = existing.Count;
            existing.CopyTo( array, 0 );

            for( int i = 0; i < newAttributes.Length; i++ )
            {
                if( newAttributes[i] == null )
                    throw new ArgumentNullException( "newAttributes" );

                bool flag = false;

                for( int j = 0; j < existing.Count; j++ )
                {
                    if( array[j].TypeId.Equals(newAttributes[i].TypeId) )
                    {
                        flag = true;
                        array[j] = newAttributes[i];
                        break;
                    }
                }

                if( !flag )
                {
                    array[length++] = newAttributes[i];
                }
            }

            Attribute[] destinationArray = null;

            if( length < array.Length )
            {
                destinationArray = new Attribute[length];
                Array.Copy( array, 0, destinationArray, 0, length );
            }
            else
            {
                destinationArray = array;
            }

            return new AttributeCollection( destinationArray );
        }
        #endregion Overrides
    }

    public abstract class PropertyDescriptorAdv : PropertyDescriptor
    {
        #region Class Members
        private Type m_componentType;
        private Type m_propertyType;
        #endregion

        #region Class Initialize/Finalize Methods
        protected PropertyDescriptorAdv( Type componentType, string name, Type propertyType )
            : base( name, null )
        {
        }

        protected PropertyDescriptorAdv( Type componentType, string name, Type propertyType, Attribute[] attributes )
            : base( name, attributes )
        {
            m_componentType = componentType;
            m_propertyType = propertyType;
        }
        #endregion

        #region Class Overrides

        #region Methods
        public override bool CanResetValue( object component )
        {
            DefaultValueAttribute attribute = (DefaultValueAttribute)this.Attributes[typeof(DefaultValueAttribute)];

            if (attribute == null)
            {
                return false;
            }

            return attribute.Value.Equals(this.GetValue(component));
        }

        public override void ResetValue( object component )
        {
            DefaultValueAttribute attribute = (DefaultValueAttribute)this.Attributes[typeof(DefaultValueAttribute)];

            if (attribute != null)
            {
                this.SetValue(component, attribute.Value);
            }
        }

        public override bool ShouldSerializeValue( object component )
        {
            return false;
        }
        #endregion

        #region Properties
        public override Type ComponentType
        {
            get
            {
                return m_componentType;
            }
        }

        public override bool IsReadOnly
        {
            get
            {
                return this.Attributes.Contains( ReadOnlyAttribute.Yes );
            }
        }
        
        public override Type PropertyType
        {
            get
            {
                return m_propertyType;
            }
        }
        #endregion

        #endregion
    }

    public sealed class CustomPropertyDescriptor : PropertyDescriptorAdv
    {
        #region Class Members
        private readonly PropertyDescriptor m_inner;
        #endregion

        #region Class Properties
        private PropertyDescriptor InnerPropertyDescriptor
        {
            [DebuggerStepThrough]
            get
            {
                return m_inner;
            }
        }
        #endregion

        #region Class Initialize/Finalize Methods
        public CustomPropertyDescriptor( PropertyDescriptor inner, Attribute[] attributes )
            : base( inner != null ? inner.ComponentType : null,
                    inner != null ? inner.Name : null,
                    inner != null ? inner.PropertyType : null,
                    attributes )
        {
            if( inner == null )
                throw new ArgumentNullException( "inner" );

            m_inner = inner;
        }
        #endregion

        #region Class Overrides
        public override object GetValue( object component )
        {
            return InnerPropertyDescriptor.GetValue( component );
        }

        public override void SetValue( object component, object value )
        {
            InnerPropertyDescriptor.SetValue( component, value );
        }
        #endregion
    }
}
