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

namespace Syncfusion.PdfViewer.Base
{
    abstract class PIGraphicsSurface
    {
        public abstract void Initialize(float width, float height);
        public abstract PIGraphicsState Save();
        public abstract void Restore();
        public abstract void Restore(PIGraphicsState state);
        public abstract void TranslateTransform(float dx, float dy);
        public abstract void ScaleTransform(float sx, float sy);
        public abstract void RotateTransform(float angle);
        public abstract void DrawPath(PIPen pen, PIPath path);
        public abstract void FillPath(PIBrush brush, PIPath path);
        public abstract void DrawRectangle(PIPen pen, float x, float y, float width, float height);
        public abstract void FillRectangle(PIBrush brush, float x, float y, float width, float height);
        public abstract void DrawLine(PIPen pen, float x1, float y1, float x2, float y2);
    }


}
