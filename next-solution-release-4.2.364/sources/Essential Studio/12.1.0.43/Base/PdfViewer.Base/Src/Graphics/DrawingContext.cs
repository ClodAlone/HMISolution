#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;

#if WINDOWS
using Syncfusion.PdfViewer.Windows;
#else
using Syncfusion.Windows.PdfViewer;
#endif

namespace Syncfusion.PdfViewer.Base
{
    class DrawingContext
    {
        PIGraphicsSurface _gfxSurface;
        bool _initalized;

        Stack<PIGraphicsState> _gfxStates;


        public DrawingContext()
        {
            _gfxStates = new Stack<PIGraphicsState>();
        }

        public PIGraphicsSurface Graphics
        {
            get
            {
                return _gfxSurface;
            }
        }

        public void InitializeGraphics(float width, float height)
        {
#if WINDOWS
            _gfxSurface = new WindowsSurface();
            _gfxSurface.Initialize(width, height);
#else
            _gfxSurface = new WPFSurface();
            _gfxSurface.Initialize(width, height);
#endif
            _initalized = true;
        }

        

        public void DrawElements(List<string[]> contentElements, PdfPageResources resources)
        {
            if(!_initalized)
                throw new ArgumentNullException("Drawing surface is not initialized.");
        }
    }

    
}
