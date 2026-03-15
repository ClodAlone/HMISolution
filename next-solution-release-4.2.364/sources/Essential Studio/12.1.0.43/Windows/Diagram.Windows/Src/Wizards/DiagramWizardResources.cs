#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Diagram
{
    sealed class DiagramWizardResources
    {
        private static readonly Image d_wizardBackImage;
        private static readonly Image d_btnNormalImage;
        private static readonly Image d_btnHoverImage;
        private static readonly Image d_btnImage;
        private static readonly Image d_btnDiagramBuilderImage;
        private static readonly Image d_btnSymbolBuilderImage;

        static DiagramWizardResources()
        {
            Type type = typeof(DiagramWizardResources);

            d_wizardBackImage = new Bitmap(type, "Wizards.Resources.BG.jpg");
            d_btnNormalImage = new Bitmap(type, "Wizards.Resources.btnNormal.png");
            d_btnHoverImage = new Bitmap(type, "Wizards.Resources.btnHover.png");
            d_btnImage = new Bitmap(type, "Wizards.Resources.btnPlain.png");
            d_btnDiagramBuilderImage = new Bitmap(type, "Wizards.Resources.btnDiagramBuilder.png");
            d_btnSymbolBuilderImage = new Bitmap(type, "Wizards.Resources.btnSymbolBuilder.png");
        }

        public static Image WizardBackImage
        {
            get
            {
                return d_wizardBackImage;
            }
        }

        public static Image ButtonNormalImage
        {
            get
            {
                return d_btnNormalImage;
            }
        }

        public static Image ButtonHoverImage
        {
            get
            {
                return d_btnHoverImage;
            }
        }

        public static Image ButtonImage
        {
            get
            {
                return d_btnImage;
            }
        }

        public static Image ButtonDbImage
        {
            get
            {
                return d_btnDiagramBuilderImage;
            }
        }

        public static Image ButtonSbImage
        {
            get
            {
                return d_btnSymbolBuilderImage;
            }
        }        
    }
}
