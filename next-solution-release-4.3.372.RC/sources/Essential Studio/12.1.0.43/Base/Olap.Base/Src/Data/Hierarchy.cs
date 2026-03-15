//-------------------------------------------------------------------------------------------------
// <copyright file="Hierarchy.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

#if !SILVERLIGHT
namespace Syncfusion.Olap.Data
{
    /// <summary>
    /// Represents a dimension hierarchy contained by a dimension or set.
    /// </summary>
    [Serializable]
    public class Hierarchy : IDisposable
    {
#else
using System.Runtime.Serialization;

namespace Syncfusion.OlapSilverlight.Data
{
    /// <summary>
    /// Represents a dimension hierarchy contained by a dimension or set. 
    /// </summary>
    [DataContract]
    public class Hierarchy : IDisposable
    {
#endif
        #region Private Properties
        /// <summary>
        /// Gets or sets the default member unique name
        /// </summary>
        /// <value>The default name of the member unique.</value>
        [DefaultValue(""), Description("Gets or sets the default member unique name.")]
        string _DefaultMemberUniqueName;
#if !SILVERLIGHT
        [NonSerialized]
#endif
        PropertyCollection _Properties;

        /// <summary>
        /// Gets or sets the name of the unique.
        /// </summary>
        /// <value>The name of the unique.</value>
        [Description("Gets or sets the unique name."), DefaultValue("")]
        string _UniqueName;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Hierarchy"/> class.
        /// </summary>
        public Hierarchy()
        {
            this.Caption = string.Empty;
            this.DefaultLevelName = string.Empty;
            this.DefaultMemberUniqueName = string.Empty;
            this.Description = string.Empty;
            this.DisplayFolder = string.Empty;
            this.Name = string.Empty;
            this.UniqueName = string.Empty;
            Levels = new LevelCollection(this);
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the caption.
        /// </summary>
        /// <value>The caption.</value>
        [DefaultValue(""), Description("Gets or sets the caption.")]
#if SILVERLIGHT
        [DataMember]
#endif
        public string Caption { get; set; }

        /// <summary>
        /// Gets or sets the default name of the level.
        /// </summary>
        /// <value>The default name of the level.</value>
        [DefaultValue(""), Description("Gets or sets the default level name.")]
#if SILVERLIGHT
        [DataMember]
#endif
        public string DefaultLevelName { get; set; }

#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the default name of the level unique.
        /// </summary>
        /// <value>The default name of the level unique.</value>
        public string DefaultLevelUniqueName { get; set; }

#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the default name of the member unique.
        /// </summary>
        /// <value>The default name of the member unique.</value>
        public string DefaultMemberUniqueName 
        { 
            get 
            {
                return _DefaultMemberUniqueName; 
            } 

            set 
            {
                _DefaultMemberUniqueName = value.Replace(".[(All)]", string.Empty); 
            } 
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [DefaultValue(""), Description("Gets or sets the hierarchy description.")]
#if SILVERLIGHT
        [DataMember]
#endif
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the display folder.
        /// </summary>
        /// <value>The display folder.</value>
        [DefaultValue(""), Description("Gets or sets the hierarchy display folder name. Nested folders are separated with '' characters.")]
#if SILVERLIGHT
        [DataMember]
#endif
        public string DisplayFolder { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is attribute hierarchy.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is attribute hierarchy; otherwise, <c>false</c>.
        /// </value>
        [Description("Gets the flag that indicates if hierarchy is an attribute hierarchy."), DefaultValue(false)]
#if SILVERLIGHT
       [DataMember]
        public bool IsAttributeHierarchy { get; set; }
#else
        public bool IsAttributeHierarchy { get; internal set; }
#endif
        /// <summary>
        /// Gets or sets the levels.
        /// </summary>
        /// <value>The levels.</value>
        [Description("Gets a collection of hierarchy levels."), DefaultValue((string)null)]
#if SILVERLIGHT
        [DataMember]
#endif
        public LevelCollection Levels { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>Contains the name.</value>
        [DefaultValue(""), Description("Gets or sets the hierarchy name.")]
#if SILVERLIGHT
        [DataMember]
#endif
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the parent dimension.
        /// </summary>
        /// <value>The parent dimension.</value>
#if !SILVERLIGHT
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#else
       [IgnoreDataMember]
#endif
        [Description("Gets or sets the dimension that this hierarchy belongs to."), Browsable(false), DefaultValue((string)null)]
        public Dimension ParentDimension { get; set; }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>The properties.</value>
        [DefaultValue((string)null), Description("Gets properties collection")]
        [XmlIgnore]
#if SILVERLIGHT
       [DataMember]
#endif
        public PropertyCollection Properties
        {
            get
            {
                if (_Properties == null)
                {
                    this._Properties = new PropertyCollection();
                }

                return _Properties;
            }

#if !SILVERLIGHT
            private set
            {
                _Properties = value;
            }
#else
            set
            {
                _Properties = value;
            }
#endif
        }

#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the unique name.
        /// </summary>
        /// <value>The unique name of the hierarchy.</value>
        public string UniqueName 
        { 
            get 
            { 
                return _UniqueName; 
            } 

            set 
            {
                _UniqueName = value.Replace(".[(All)]", string.Empty); 
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            foreach (Level level in Levels)
            {
                level.Dispose();
            }
        }

        /// <summary>
        /// Gets the default level.
        /// </summary>
        /// <returns>Level object</returns>
        public Level GetDefaultLevel()
        {
            if (this.Levels.Count > 0)
            {
                if (this.DefaultLevelName.Length > 0)
                {
                    foreach (Level level in Levels)
                    {
                        if (level.Name == this.DefaultLevelName || level.UniqueName == this.DefaultLevelName)
                        {
                            return level;
                        }
                    }
                }
                else
                {
                    foreach (Level level in Levels)
                    {
                        if (level.Visible && level.LevelType == LevelTypeEnum.All)
                        {
                            return level;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            if (Caption.Length > 0)
            {
                return Caption;
            }

            return Name;
        }
        #endregion
    }
}
