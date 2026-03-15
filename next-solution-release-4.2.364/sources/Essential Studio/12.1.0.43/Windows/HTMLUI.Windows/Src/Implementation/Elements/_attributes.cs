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
using System.Reflection;

using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// RaiseEventHandler delegate
    /// </summary>
    /// <param name="e">EventArgs instance</param>
    public delegate void RaiseEventHandler(EventArgs e);

    /// <summary>
    /// Attribute indicates that element event is available for user.
    /// </summary>
    [AttributeUsage(AttributeTargets.Event)]
    public sealed class ElementEventAttribute : Attribute
    {
        #region Class members

        /// <summary>
        /// Event of element.
        /// </summary>
        private EventInfo m_event;

        /// <summary>
        /// Name of the raiser method.
        /// </summary>
        private string m_strRaiserName;

        /// <summary>
        /// Method for raising.
        /// </summary>
        private MethodInfo m_method;

        #endregion

        #region Class Properties

        /// <summary>
        /// Gets the name of the raiser method.
        /// </summary>
        public string RaiserName
        {
            get
            {
                return m_strRaiserName;
            }
        }

        /// <summary>
        /// Gets or sets the raiser method.
        /// </summary>
        public MethodInfo RaiserMethod
        {
            get
            {
                return m_method;
            }
            set
            {
                m_method = value;
            }
        }

        /// <summary>
        /// Gets or sets the event of the element.
        /// </summary>
        public EventInfo Event
        {
            get
            {
                return m_event;
            }
            set
            {
                m_event = value;
            }
        }

        #endregion

        #region Class Initialize/Finalize methods

        /// <summary>
        /// Prevents a default instance of the ElementEventAttribute class from being created
        /// </summary>
        private ElementEventAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ElementEventAttribute class
        /// </summary>
        /// <param name="raiser">a string value</param>
        public ElementEventAttribute(string raiser)
            : base()
        {
            m_strRaiserName = raiser;
        }
        #endregion
    }

    /// <summary>
    /// Attributes for TAG element classes, which are responsible for the corresponding HTML tags.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ElementTagAttribute : Attribute
    {
        #region Class members

        /// <summary>
        /// Name of the tag.
        /// </summary>
        private string m_strTagName;
        #endregion

        #region Class Properties

        /// <summary>
        /// Gets the name of the tag.
        /// </summary>
        public string Name
        {
            get
            {
                return m_strTagName;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods

        /// <summary>
        /// Prevents a default instance of the ElementTagAttribute class from being created
        /// </summary>
        private ElementTagAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ElementTagAttribute class
        /// </summary>
        /// <param name="name">Name of the tag.</param>
        public ElementTagAttribute(string name)
            : base()
        {
            m_strTagName = name;
        }
        #endregion
    }

    /// <summary>
    /// Defines the type of control reaction on changing the attribute or format property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    internal sealed class ReactionTypeAttribute : Attribute
    {
        #region Class members

        /// <summary>
        /// Name of the attribute.
        /// </summary>
        private string m_attributeName;

        /// <summary>
        /// Type of reaction on attribute changing.
        /// </summary>
        private ReactType m_reactType;
        #endregion

        #region Class properties

        /// <summary>
        /// Gets the name of the attribute which has been changed.
        /// </summary>
        public string Name
        {
            get
            {
                return m_attributeName;
            }
        }

        /// <summary>
        /// Gets the type of the reaction on attribute changing.
        /// </summary>
        public ReactType Reaction
        {
            get
            {
                return m_reactType;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods

        /// <summary>
        /// Prevents a default instance of the ReactionTypeAttribute class from being created
        /// </summary>
        private ReactionTypeAttribute()
        {
            m_attributeName = string.Empty;
            m_reactType = ReactType.None;
        }

        /// <summary>
        /// Initializes a new instance of the ReactionTypeAttribute class
        /// </summary>
        /// <param name="name">Name of the attribute.</param>
        /// <param name="reaction">Type of reaction on attribute changing.</param>
        public ReactionTypeAttribute(string name, ReactType reaction)
            : this()
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (name.Length == 0)
                throw new ArgumentException("name - string can not be empty");

            m_attributeName = name;
            m_reactType = reaction;
        }
        #endregion
    }

    /// <summary>
    /// Defines the holder of attributes which can raise events when its changing.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AttributeHolderAttribute : Attribute
    {
        #region Class members

        /// <summary>
        /// Type of attribute holder.
        /// </summary>
        private string m_typeName;
        #endregion

        #region Class properties

        /// <summary>
        /// Gets the type of attribute holder.
        /// </summary>
        public string TypeName
        {
            get
            {
                return m_typeName;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods

        /// <summary>
        /// Prevents a default instance of the AttributeHolderAttribute class from being created
        /// </summary>
        private AttributeHolderAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the AttributeHolderAttribute class
        /// </summary>
        /// <param name="typeName">Type of the holder of attributes.</param>
        public AttributeHolderAttribute(Type typeName)
        {
            if (typeName == null)
                throw new ArgumentNullException("typeName");

            m_typeName = typeName.ToString();
        }
        #endregion
    }
}
