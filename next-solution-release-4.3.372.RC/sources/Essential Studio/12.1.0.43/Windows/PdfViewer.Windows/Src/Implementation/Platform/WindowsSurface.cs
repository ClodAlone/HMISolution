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
using Syncfusion.PdfViewer.Base;
using System.Drawing;

namespace Syncfusion.PdfViewer.Windows
{
    class WindowsSurface : PIGraphicsSurface
    {
        Graphics _graphics;

        public override void Initialize(float width, float height)
        {
            Bitmap bmp = new Bitmap((int)width, (int)height);
            _graphics = Graphics.FromImage(bmp);
            _graphics.FillRectangle(Brushes.White, new Rectangle(0, 0, (int)width, (int)height));
        }
        public override PIGraphicsState Save()
        {
            throw new NotImplementedException();
        }

        public override void Restore()
        {
            throw new NotImplementedException();
        }

        public override void Restore(PIGraphicsState state)
        {
            throw new NotImplementedException();
        }

        public override void TranslateTransform(float dx, float dy)
        {
            throw new NotImplementedException();
        }

        public override void ScaleTransform(float sx, float sy)
        {
            throw new NotImplementedException();
        }

        public override void RotateTransform(float angle)
        {
            throw new NotImplementedException();
        }

        public override void DrawPath(PIPen pen, PIPath path)
        {
            throw new NotImplementedException();
        }

        public override void FillPath(PIBrush brush, PIPath path)
        {
            throw new NotImplementedException();
        }

        public override void DrawRectangle(PIPen pen, float x, float y, float width, float height)
        {
            throw new NotImplementedException();
        }

        public override void FillRectangle(PIBrush brush, float x, float y, float width, float height)
        {
            throw new NotImplementedException();
        }

        public override void DrawLine(PIPen pen, float x1, float y1, float x2, float y2)
        {
            throw new NotImplementedException();
        }
    }
}
