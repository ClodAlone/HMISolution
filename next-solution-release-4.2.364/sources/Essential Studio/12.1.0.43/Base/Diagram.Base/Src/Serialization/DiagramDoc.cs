#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.ComponentModel.Design.Serialization;
using System.Runtime.Serialization;
using Syncfusion.Windows.Forms.Diagram;
using System.Reflection;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// The DiagramDocument class implements a serializable document type that encapsulates the model and view data 
    /// for the diagram.
    /// </summary>
    /// <remarks>
    /// This document class is used by the Essential Diagram DiagramControl 
    /// and DiagramWebControl controls for persisting their state information.
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Model"/>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.View"/>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.ViewInfo"/>
    /// </remarks>
    [Serializable]
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
    public class DiagramDocument : ISerializable
    {
        #region Class members
        /// <summary>
        /// The model.
        /// </summary>
        protected Model dgmModel = null;

        /// <summary>
        /// The view.
        /// </summary>
        protected View dgmView = null;

        /// <summary>
        /// The view info.
        /// </summary>
        protected ViewInfo dgmViewInfo = null;

        /// <summary>
        /// The diagram model.
        /// </summary>
        protected Model m_dgmModelComponent = null;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the diagram model.
        /// </summary>
        public Model Model
        {
            get { return this.dgmModel; }
        }

        /// <summary>
        /// Gets the diagram view.
        /// </summary>
        public View View
        {
            get
            {
                if ((this.dgmView == null) && (this.dgmViewInfo != null))
                    this.dgmView = new View(this.dgmViewInfo);
                return this.dgmView;
            }
        }

        /// <summary>
        /// Gets the ViewInfo.
        /// </summary>
        public ViewInfo ViewInfo
        {
            get
            {
                if ((this.dgmViewInfo == null) && (this.dgmView != null))
                    this.dgmViewInfo = new ViewInfo(this.dgmView);
                return this.dgmViewInfo;
            }
        }

        /// <summary>
        /// Gets or sets the reference to the existing component in designer.
        /// </summary>
        public Model ModelComponent
        {
            get { return m_dgmModelComponent; }
            set { m_dgmModelComponent = value; }
        }
        #endregion

        #region Class initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="DiagramDocument"/> class.
        /// </summary>
        /// <param name="dgmModel">The diagram model.</param>
        /// <param name="dgmViewInfo">The diagram ViewInfo.</param>
        public DiagramDocument(Model dgmModel, ViewInfo dgmViewInfo)
        {
            this.dgmModel = dgmModel;
            this.dgmViewInfo = dgmViewInfo;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagramDocument"/> class.
        /// </summary>
        /// <param name="dgmModel">The diagram model.</param>
        /// <param name="dgmView">The diagram view.</param>
        /// <remarks>
        /// This method is retained only for backward compatibility and should not be used.
        /// </remarks>
        public DiagramDocument(Model dgmModel, View dgmView)
        {
            this.dgmModel = dgmModel;
            this.dgmView = dgmView;
            if (dgmView != null)
                this.dgmViewInfo = new ViewInfo(this.dgmView);
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Initializes a new instance of the <see cref="DiagramDocument"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected DiagramDocument(SerializationInfo info, StreamingContext context)
        {
            string version = String.Empty;
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "version":
                        version = (string)info.GetValue("version", typeof(string));
                        break;
                    case "dgmModel":
                        this.dgmModel = (Model)info.GetValue("dgmModel", typeof(Model));
                        break;
                    case "dgmView":
                        this.dgmView = (View)info.GetValue("dgmView", typeof(View));
                        break;
                }
            }
        }
        #endregion

        #region ISerializable Implementation
        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("version", "3.0");
            info.AddValue("dgmModel", dgmModel);
            info.AddValue("dgmView", dgmView);
        }
        #endregion
    }
    /// <summary>
    /// Allows user to control old type diagram/palette loading.
    /// </summary>
    public sealed class OldToNewDeserializationBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Type typeToDeserialize;

            // For each assemblyName/typeName that you want to deserialize to
            // a different type, set typeToDeserialize to the desired type.
            string assem = Assembly.GetExecutingAssembly().FullName;

            if (assemblyName.IndexOf("Syncfusion.Diagram") != -1 && assemblyName.IndexOf("Version", 0) != -1)
            {
                // find "Version" substring
                int nIdxStart = assemblyName.IndexOf("Version", 0);
                int nIdxEnd = assemblyName.IndexOf(",", nIdxStart);

                int nIdxRplStart = assem.IndexOf("Version", 0);
                int nIdxRplEnd = assem.IndexOf(",", nIdxRplStart);

                // replace whole "Version" substring
                assemblyName = assemblyName.Replace(
                    assemblyName.Substring(nIdxStart, nIdxRplStart + (nIdxEnd - nIdxStart)),
                    assem.Substring(nIdxRplStart, nIdxRplStart + (nIdxRplEnd - nIdxRplStart)));
            }

            if (typeName.IndexOf("LineNode") != -1)
            {
                // types binding to ensure loading previous versions
                typeName = typeName.Replace("LineNode", "Line");
            }

            // The following line of code returns the type.
            typeToDeserialize = Type.GetType(String.Format("{0}, {1}", typeName, assemblyName));

            return typeToDeserialize;
        }
    }
}
