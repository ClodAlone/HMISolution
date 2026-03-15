#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Pdf.JPEG2000.util
{
    internal class MathUtil
    {
        public static int log2(int x)
        {
            int y, v;
            if (x <= 0)
            {
                throw new System.ArgumentException("" + x + " <= 0");
            }
            v = x;
            y = -1;
            while (v > 0)
            {
                v >>= 1;
                y++;
            }
            return y;
        }
        public static int lcm(int x1, int x2)
        {
            if (x1 <= 0 || x2 <= 0)
            {
                throw new System.ArgumentException("Cannot compute the least " + "common multiple of two " + "numbers if one, at least," + "is negative.");
            }
            int max, min;
            if (x1 > x2)
            {
                max = x1;
                min = x2;
            }
            else
            {
                max = x2;
                min = x1;
            }
            for (int i = 1; i <= min; i++)
            {
                if ((max * i) % min == 0)
                {
                    return i * max;
                }
            }
            return -1;
            //throw new System.ApplicationException("Cannot find the least common multiple of numbers " + x1 + " and " + x2);
        }
        public static int lcm(int[] x)
        {
            if (x.Length < 2)
            {
                //throw new System.ApplicationException("Do not use this method if there are less than" + " two numbers.");
            }
            int tmp = lcm(x[x.Length - 1], x[x.Length - 2]);
            for (int i = x.Length - 3; i >= 0; i--)
            {
                if (x[i] <= 0)
                {
                    throw new System.ArgumentException("Cannot compute the least " + "common multiple of " + "several numbers where " + "one, at least," + "is negative.");
                }
                tmp = lcm(tmp, x[i]);
            }
            return tmp;
        }
        public static int gcd(int x1, int x2)
        {
            if (x1 < 0 || x2 < 0)
            {
                throw new System.ArgumentException("Cannot compute the GCD " + "if one integer is negative.");
            }
            int a, b, g, z;
            if (x1 > x2)
            {
                a = x1;
                b = x2;
            }
            else
            {
                a = x2;
                b = x1;
            }
            if (b == 0)
                return 0;
            g = b;
            while (g != 0)
            {
                z = a % g;
                a = g;
                g = z;
            }
            return a;
        }
        public static int gcd(int[] x)
        {
            if (x.Length < 2)
            {
                //throw new System.ApplicationException("Do not use this method if there are less than" + " two numbers.");
            }
            int tmp = gcd(x[x.Length - 1], x[x.Length - 2]);
            for (int i = x.Length - 3; i >= 0; i--)
            {
                if (x[i] < 0)
                {
                    throw new System.ArgumentException("Cannot compute the least " + "common multiple of " + "several numbers where " + "one, at least," + "is negative.");
                }
                tmp = gcd(tmp, x[i]);
            }
            return tmp;
        }
    }
}