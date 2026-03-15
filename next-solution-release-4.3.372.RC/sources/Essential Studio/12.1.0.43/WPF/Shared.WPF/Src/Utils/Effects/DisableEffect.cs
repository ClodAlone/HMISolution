#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Effects;
using System.ComponentModel;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// 
    /// </summary>
    public class DisableEffect : ShaderEffect
    {
        /// <summary>
        /// Dependency property for Input
        /// </summary>
        public static readonly DependencyProperty InputProperty =
            RegisterPixelShaderSamplerProperty("Input", typeof(DisableEffect), 0);

        /// <summary>
        /// Dependency property for FilterColor
        /// </summary>
        public static readonly DependencyProperty FilterColorProperty =
            DependencyProperty.Register("FilterColor", typeof(Color), typeof(DisableEffect),
            new UIPropertyMetadata(Color.FromArgb(255, 255, 255, 255), PixelShaderConstantCallback(0)));

        /// <summary>
        /// Default constructor
        /// </summary>
        public DisableEffect()
        {
            PixelShader pixelShader = new PixelShader();
            var prop = DesignerProperties.IsInDesignModeProperty;

            bool isInDesignMode = (bool)DependencyPropertyDescriptor.FromProperty(prop, typeof(FrameworkElement)).Metadata.DefaultValue;
            //pixelShader.UriSource = new Uri(@"/Syncfusion.Tools.WPF;component/Framework/Ribbon/Implementation/Effects/Disable.ps", UriKind.RelativeOrAbsolute);
            pixelShader.UriSource = new Uri(@"/Syncfusion.Shared.WPF;component/Utils/Effects/Disable.ps", UriKind.RelativeOrAbsolute);
            PixelShader = pixelShader;

            UpdateShaderValue(InputProperty);
            UpdateShaderValue(FilterColorProperty);
        }

        /// <summary>
        /// Impicit input
        /// </summary>
        public Brush Input
        {
            get
            {
                return ((Brush)(GetValue(InputProperty)));
            }
            set
            {
                SetValue(InputProperty, value);
            }
        }

        /// <summary>
        /// The color used to tint the input.
        /// </summary>
        public Color FilterColor
        {
            get
            {
                return ((Color)(GetValue(FilterColorProperty)));
            }
            set
            {
                SetValue(FilterColorProperty, value);
            }
        }
    }
}
