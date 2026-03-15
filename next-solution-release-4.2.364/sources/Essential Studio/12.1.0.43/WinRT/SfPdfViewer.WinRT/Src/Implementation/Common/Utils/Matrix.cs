#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;

namespace Syncfusion.Windows.PdfViewer
{
	internal struct GraphicsMatrix
	{
		private MatrixTypes type;
		public static GraphicsMatrix Identity
		{
			get
			{
				return new GraphicsMatrix(1.0, 0.0, 0.0, 1.0, 0.0, 0.0);
			}
		}
		public double Determinant
		{
			get
			{
				switch (this.type)
				{
				case MatrixTypes.Identity:
				case MatrixTypes.Translation:
					return 1.0;
				case MatrixTypes.Scaling:
				case (MatrixTypes)3:
					return this.M11 * this.M22;
				default:
					return this.M11 * this.M22 - this.M12 * this.M21;
				}
			}
		}
		public double M11
		{
			get;
			set;
		}
		public double M12
		{
			get;
			set;
		}
		public double M21
		{
			get;
			set;
		}
		public double M22
		{
			get;
			set;
		}
		public double OffsetX
		{
			get;
			set;
		}
		public double OffsetY
		{
			get;
			set;
		}
        public GraphicsMatrix(double m11, double m12, double m21, double m22, double offsetX, double offsetY)
		{
            this = default(GraphicsMatrix);
			this.M11 = m11;
			this.M12 = m12;
			this.M21 = m21;
			this.M22 = m22;
			this.OffsetX = offsetX;
			this.OffsetY = offsetY;
			this.type = MatrixTypes.Unknown;
			this.CheckMatrixType();
		}
        public static GraphicsMatrix operator *(GraphicsMatrix matrix1, GraphicsMatrix matrix2)
		{
            return new GraphicsMatrix(matrix1.M11 * matrix2.M11 + matrix1.M12 * matrix2.M21, matrix1.M11 * matrix2.M12 + matrix1.M12 * matrix2.M22, matrix1.M21 * matrix2.M11 + matrix1.M22 * matrix2.M21, matrix1.M21 * matrix2.M12 + matrix1.M22 * matrix2.M22, matrix1.OffsetX * matrix2.M11 + matrix1.OffsetY * matrix2.M21 + matrix2.OffsetX, matrix1.OffsetX * matrix2.M12 + matrix1.OffsetY * matrix2.M22 + matrix2.OffsetY);
		}
        public static bool operator ==(GraphicsMatrix a, GraphicsMatrix b)
		{
			return a.M11 == b.M11 && a.M21 == b.M21 && a.M12 == b.M12 && a.M22 == b.M22 && a.OffsetX == b.OffsetX && a.OffsetY == b.OffsetY;
		}
        public static bool operator !=(GraphicsMatrix a, GraphicsMatrix b)
		{
			return !(a == b);
		}
		public bool IsIdentity()
		{
            return this == GraphicsMatrix.Identity;
		}
        public GraphicsMatrix Translate(double offsetX, double offsetY)
		{
			if (this.type == MatrixTypes.Identity)
			{
				this.SetMatrix(1.0, 0.0, 0.0, 1.0, offsetX, offsetY, MatrixTypes.Translation);
			}
			else
			{
				if (this.type == MatrixTypes.Unknown)
				{
					this.OffsetX += offsetX;
					this.OffsetY += offsetY;
				}
				else
				{
					this.OffsetX += offsetX;
					this.OffsetY += offsetY;
					this.type |= MatrixTypes.Translation;
				}
			}
			return this;
		}
        public GraphicsMatrix Scale(double scaleX, double scaleY, double centerX = 0.0, double centerY = 0.0)
		{
            this = new GraphicsMatrix(scaleX, 0.0, 0.0, scaleY, centerX, centerY) * this;
			return this;
		}
        public GraphicsMatrix ScaleAppend(double scaleX, double scaleY, double centerX = 0.0, double centerY = 0.0)
		{
            this *= new GraphicsMatrix(scaleX, 0.0, 0.0, scaleY, centerX, centerY);
			return this;
		}
        public GraphicsMatrix Rotate(double angle, double centerX = 0.0, double centerY = 0.0)
		{
            GraphicsMatrix matrix = default(GraphicsMatrix);
			angle = 3.1415926535897931 * angle / 180.0;
			double num = Math.Sin(angle);
			double num2 = Math.Cos(angle);
			double offsetX = centerX * (1.0 - num2) + centerY * num;
			double offsetY = centerY * (1.0 - num2) - centerX * num;
			matrix.SetMatrix(num2, num, -num, num2, offsetX, offsetY, MatrixTypes.Unknown);
			this = matrix * this;
			return this;
		}
        public bool Equals(GraphicsMatrix value)
		{
			return this.M11 == value.M11 && this.M12 == value.M12 && this.M21 == value.M21 && this.M22 == value.M22 && this.OffsetX == value.OffsetX && this.OffsetY == value.OffsetY && this.type.Equals(value.type);
		}
		public double Transform(double d)
		{
			double val = Math.Sqrt(Math.Pow(this.M11 + this.M21, 2.0) + Math.Pow(this.M12 + this.M22, 2.0));
			double val2 = Math.Sqrt(Math.Pow(this.M11 - this.M21, 2.0) + Math.Pow(this.M12 - this.M22, 2.0));
			return d * Math.Max(val, val2);
		}
		public override bool Equals(object obj)
		{
            return obj != null && obj is GraphicsMatrix && this.Equals((GraphicsMatrix)obj);
		}
		public override string ToString()
		{
			return string.Format("{0} {1} 0 | {2} {3} 0 | {4} {5} 1", new object[]
			{
				this.M11,
				this.M12,
				this.M21,
				this.M22,
				this.OffsetX,
				this.OffsetY
			});
		}
		private void CheckMatrixType()
		{
			this.type = MatrixTypes.Identity;
			if (this.M21 != 0.0 || this.M12 != 0.0)
			{
				this.type = MatrixTypes.Unknown;
				return;
			}
			if (this.M11 != 1.0 || this.M22 != 1.0)
			{
				this.type = MatrixTypes.Scaling;
			}
			if (this.OffsetX != 0.0 || this.OffsetY != 0.0)
			{
				this.type |= MatrixTypes.Translation;
			}
			if ((this.type & (MatrixTypes)3) == MatrixTypes.Identity)
			{
				this.type = MatrixTypes.Identity;
			}
		}
		private void SetMatrix(double m11, double m12, double m21, double m22, double offsetX, double offsetY, MatrixTypes type)
		{
			this.M11 = m11;
			this.M12 = m12;
			this.M21 = m21;
			this.M22 = m22;
			this.OffsetX = offsetX;
			this.OffsetY = offsetY;
			this.type = type;
		}
	}
}
