#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 

#endregion

#region file using directives
using System;
using System.Collections;
using System.IO;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Collection of types of attributes changing.
    /// Holds attributes and reaction types of controls when they change.
    /// </summary>
    internal class ReactionCollection : ICloneable
    {
        #region Class members
        /// <summary>
        /// Holds types of sender's attributes changing.
        /// </summary>
        private Hashtable m_types;
        #endregion

        #region Class properties
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the ReactionCollection class
        /// </summary>
        public ReactionCollection()
        {
            m_types = new Hashtable();
        }
        #endregion

        #region Class Public Methods
        /// <summary>
        /// Adds information of changing some attribute to collection if it does not exist;
        /// changes its attribute name if defined typeName already exists.
        /// </summary>
        /// <param name="typeName">Type name of sender event.</param>
        /// <param name="name">Name of the parameter.</param>
        /// <param name="reaction">Type of reaction.</param>
        public void Add(string typeName, string name, ReactType reaction)
        {
            if (typeName == null)
                throw new ArgumentNullException("typeName");

            if (typeName.Length == 0)
                throw new ArgumentException("typeName - string can not be empty");

            if (name == null)
                throw new ArgumentNullException("name");

            if (name.Length == 0)
                throw new ArgumentException("name - string can not be empty");

            // get holder of attributes on specified type.
            Hashtable typeHash;

            if (!m_types.Contains(typeName))
            {
                typeHash = new Hashtable();
                m_types[typeName] = typeHash;
            }
            else
            {
                typeHash = m_types[typeName] as Hashtable;
            }

            typeHash[name] = reaction;
        }

        /// <summary>
        /// Returns the type of reaction on changing the attribute.
        /// </summary>
        /// <param name="typeName">Type name of the attribute holder.</param>
        /// <param name="name">Name of the attribute.</param>
        /// <returns>
        /// Type of reaction on changing the attribute; None 
        /// if type or attribute with such name was not found.
        /// </returns>
        public ReactType GetReaction(string typeName, string name)
        {
            if (typeName == null)
                throw new ArgumentNullException("typeName");

            if (typeName.Length == 0)
                throw new ArgumentException("typeName - string can not be empty");

            if (name == null)
                throw new ArgumentNullException("name");

            if (name.Length == 0)
                throw new ArgumentException("name - string can not be empty");

            if (m_types.Contains(typeName))
            {
                Hashtable typeHash = m_types[typeName] as Hashtable;

                if (typeHash.Contains(name))
                {
                    return (ReactType)typeHash[name];
                }
            }

            return ReactType.None;
        }

        /// <summary>
        /// Overloaded. Indicates whether the collection contains reactions on attributes of this type.
        /// </summary>
        /// <param name="type">Type of holder attribute.</param>
        /// <returns>A boolean variable</returns>
        public bool ContainsType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return ContainsType(type.ToString());
        }

        /// <summary>
        /// Indicates whether the collection contains reactions on attributes of this type.
        /// </summary>
        /// <param name="typeName">Name of the attribute holder type.</param>
        /// <returns>A boolean variable</returns>
        public bool ContainsType(string typeName)
        {
            if (typeName == null)
                throw new ArgumentNullException("typeName");

            if (typeName.Length == 0)
                throw new ArgumentException("typeName - string can not be empty");

            return m_types.Contains(typeName);
        }

        /// <summary>
        /// Overloaded. Indicates whether the specified attribute has any reaction in the defined holder.
        /// </summary>
        /// <param name="type">Type of attribute holder.</param>
        /// <param name="attrName">Name of the attribute.</param>
        /// <returns>True if it contains; false otherwise.</returns>
        public bool ContainsAttribute(Type type, string attrName)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (attrName == null)
                throw new ArgumentNullException("attrName");

            if (attrName.Length == 0)
                throw new ArgumentException("attrName - string can not be empty");

            return ContainsAttribute(type.ToString(), attrName);
        }

        /// <summary>
        /// Indicates whether the specified attribute has any reaction in the defined holder.
        /// </summary>
        /// <param name="typeName">Type Name of the attribute holder.</param>
        /// <param name="attrName">Name of the attribute.</param>
        /// <returns>True if it contains; false otherwise.</returns>
        public bool ContainsAttribute(string typeName, string attrName)
        {
            if (typeName == null)
                throw new ArgumentNullException("typeName");

            if (typeName.Length == 0)
                throw new ArgumentException("typeName - string can not be empty");

            if (attrName == null)
                throw new ArgumentNullException("attrName");

            if (attrName.Length == 0)
                throw new ArgumentException("attrName - string can not be empty");

            if (m_types.Contains(typeName))
            {
                Hashtable typeHash = m_types[typeName] as Hashtable;

                return typeHash.Contains(attrName);
            }

            return false;
        }
        #endregion

        #region ICloneable Members

        /// <summary>
        /// Clones object.
        /// </summary>
        /// <returns>Cloned object.</returns>
        public object Clone()
        {
            ReactionCollection collection = (ReactionCollection)this.MemberwiseClone();

            collection.m_types = new Hashtable();

            foreach (string typeName in m_types.Keys)
            {
                collection.m_types[typeName] = (m_types[typeName] as Hashtable).Clone();
            }

            return collection;
        }
        #endregion
    }
}
