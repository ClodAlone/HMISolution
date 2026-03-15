// <copyright file="ChartMaterialInfo.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart._3DChart
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    ///  Initializes internal class ChartMaterialInfo
    /// </summary>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    internal class ChartMaterialInfo
    {
        #region Constants
        /// <summary>
        /// Declares DEF_SPECULAR
        /// </summary>
        private const double DEF_SPECULAR = 18;
        #endregion

        #region Members
        /// <summary>
        /// Initializes m_material
        /// </summary>
        private MaterialGroup m_material = new MaterialGroup();

        /// <summary>
        /// Initializes m_emissive
        /// </summary>
        private EmissiveMaterial m_emissive = null;

        /// <summary>
        /// Initializes m_specular
        /// </summary>
        private SpecularMaterial m_specular = null;

        /// <summary>
        /// Initializes m_diffuse
        /// </summary>
        private DiffuseMaterial m_diffuse = null;
        #endregion      

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ChartMaterialInfo class
        /// </summary>       
        public ChartMaterialInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ChartMaterialInfo class
        /// </summary>
        /// <param name="brush">The brush value</param>
        public ChartMaterialInfo(Brush brush)
        {
            this.Diffuse = new DiffuseMaterial(brush);
            this.Emissive = new EmissiveMaterial(brush);
            this.Specular = new SpecularMaterial(brush, DEF_SPECULAR);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the MaterialGroup value
        /// </summary>
        public MaterialGroup Material
        {
            get
            {
                return this.m_material;
            }
        }

        /// <summary>
        /// Gets or sets the EmissiveMaterial value
        /// </summary>
        public EmissiveMaterial Emissive
        {
            get
            {
                return this.m_emissive;
            }

            set
            {
                if (this.m_emissive != value)
                {
                    if (value != null)
                    {
                        this.m_material.Children.Remove(this.m_emissive);
                    }

                    this.m_emissive = value;
                    if ((this.m_emissive != null) && (!this.m_material.Children.Contains(this.m_emissive)))
                    {
                        this.m_material.Children.Add(this.m_emissive);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the SpecularMaterial value
        /// </summary>
        public SpecularMaterial Specular
        {
            get
            {
                return this.m_specular;
            }

            set
            {
                if (this.m_specular != value)
                {
                    if (value != null)
                    {
                        this.m_material.Children.Remove(this.m_specular);
                    }

                    this.m_specular = value;

                    if ((this.m_specular != null) && (!this.m_material.Children.Contains(this.m_specular)))
                    {
                        this.m_material.Children.Add(this.m_specular);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the DiffuseMaterial value
        /// </summary>
        public DiffuseMaterial Diffuse
        {
            get
            {
                return this.m_diffuse;
            }

            set
            {
                if (this.m_diffuse != value)
                {
                    if (value != null)
                    {
                        this.m_material.Children.Remove(this.m_diffuse);
                    }

                    this.m_diffuse = value;
                    if ((this.m_diffuse != null) && (!this.m_material.Children.Contains(this.m_diffuse)))
                    {
                        this.m_material.Children.Add(this.m_diffuse);
                    }
                }
            }
        }
        #endregion
    }
}
